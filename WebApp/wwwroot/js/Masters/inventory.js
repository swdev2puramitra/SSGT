(function () {
    'use strict';

    const page = document.querySelector('.inventory-page');
    if (!page) return;

    const urls = {
        inventory: page.dataset.inventoryUrl,
        items: page.dataset.itemsUrl,
        companies: page.dataset.companiesUrl,
        categories: page.dataset.categoriesUrl,
        subcategories: page.dataset.subcategoriesUrl,
        update: page.dataset.updateUrl,
        companyMargins: page.dataset.companyMarginsUrl
    };
    const canUpdatePrices = page.dataset.canUpdatePrices === 'true';
    const resultsElement = document.getElementById('inventoryResults');
    const saveButton = document.getElementById('saveInventoryPrices');
    const itemInput = document.getElementById('itemNameFilter');
    const itemIdInput = document.getElementById('itemIdFilter');
    const companyInput = document.getElementById('companyFilter');
    const companyIdInput = document.getElementById('companyIdFilter');
    const categoryInput = document.getElementById('categoryFilter');
    const categoryIdInput = document.getElementById('categoryIdFilter');
    const subcategoryInput = document.getElementById('subcategoryFilter');
    const subcategoryIdInput = document.getElementById('subcategoryIdFilter');
    const showWholeSale = document.getElementById('showWholeSalePrice');
    const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    let table;
    let filterTimer;
    let totalCount = 0;

    function filters() {
        const params = {};
        if (itemInput.value.trim()) params.itemName = itemInput.value.trim();
        if (itemIdInput.value) params.itemId = itemIdInput.value;
        if (companyIdInput.value) params.companyId = companyIdInput.value;
        if (categoryIdInput.value) params.categoryId = categoryIdInput.value;
        if (subcategoryIdInput.value) params.subcategoryId = subcategoryIdInput.value;
        return params;
    }

    function reloadGrid() {
        if (table) table.setData(urls.inventory, filters());
    }

    function scheduleReload() {
        window.clearTimeout(filterTimer);
        filterTimer = window.setTimeout(reloadGrid, 300);
    }

    function autocomplete(input, idInput, url, options) {
        const settings = options || {};
        $(input).autocomplete({
            minLength: settings.minLength ?? 0,
            delay: 200,
            source: function (request, respond) {
                const requestData = { term: request.term };
                if (settings.categoryId) requestData.categoryId = settings.categoryId();
                $.getJSON(url, requestData).done(respond).fail(function () { respond([]); });
            },
            focus: function (event, ui) {
                event.preventDefault();
                input.value = ui.item.value;
            },
            select: function (event, ui) {
                event.preventDefault();
                input.value = ui.item.value;
                idInput.value = ui.item.id;
                if (settings.onSelect) settings.onSelect(ui.item);
                reloadGrid();
            },
            change: function (_event, ui) {
                if (!ui.item) idInput.value = '';
            }
        });

        input.addEventListener('input', function () {
            idInput.value = '';
            if (settings.onInput) settings.onInput();
            scheduleReload();
        });
    }

    function updateSummary(rows) {
        const totals = new Map();
        rows.forEach(function (row) {
            const parts = String(row.stockPosition || '').trim().split(/\s+/);
            if (parts.length < 2) return;
            const quantity = Number.parseInt(parts[0], 10);
            if (!Number.isFinite(quantity)) return;
            const unit = parts.slice(1).join(' ');
            totals.set(unit, (totals.get(unit) || 0) + quantity);
        });
        const summary = Array.from(totals, function (entry) {
            return entry[1] + ' ' + entry[0];
        });
        const stockSummary = summary.length ? summary.join(' · ') : 'No stock summary for this page.';
        resultsElement.textContent = 'Found ' + totalCount + ' item(s). ' + stockSummary;
    }

    function priceColumn(title, field) {
        return {
            title: title,
            field: field,
            hozAlign: 'right',
            formatter: 'money',
            width: 100,
            formatterParams: { precision: 2 },
            editor: canUpdatePrices ? 'number' : false,
            editorParams: { min: 0, max: 99999999.99, step: 0.01 },
            visible: field !== 'wholeSalePrice' || showWholeSale.checked,
            cellEdited: function (cell) {
                const row = cell.getRow();
                const data = row.getData();
                data._modified = true;
                row.getElement().classList.add('tabulator-row-modified');
                if (field === 'wholeSalePrice') {
                    // Fetch company margins via AJAX and apply percentages to recalc derived prices
                    try {
                        const companyId = data.companyId || 0;
                        if (companyId > 0) {
                            fetch(urls.companyMargins + '?companyId=' + encodeURIComponent(companyId))
                                .then(function (r) { return r.json(); })
                                .then(function (resp) {
                                    if (resp && resp.success) {
                                        const semiPct = Number(resp.data.semiWholeSale || 0) || 0;
                                        const retailPct = Number(resp.data.retail || 0) || 0;
                                        const w = Number(data.wholeSalePrice || 0) || 0;
                                        const newSemi = Math.round(w * (1 + semiPct / 100));
                                        const newRetail = Math.round(w * (1 + retailPct / 100));
                                        row.update({ semiWholeSalePrice: newSemi, retailPrice: newRetail });
                                    } else {
                                        // fallback to margins provided in the row
                                        row.update({
                                            semiWholeSalePrice: Math.round(data.wholeSalePrice * (1 + data.semiWholeSaleMargin / 100)),
                                            retailPrice: Math.round(data.wholeSalePrice * (1 + data.retailMargin / 100))
                                        });
                                    }
                                    if (saveButton) saveButton.disabled = false;
                                })
                                .catch(function () {
                                    row.update({
                                        semiWholeSalePrice: Math.round(data.wholeSalePrice * (1 + data.semiWholeSaleMargin / 100)),
                                        retailPrice: Math.round(data.wholeSalePrice * (1 + data.retailMargin / 100))
                                    });
                                    if (saveButton) saveButton.disabled = false;
                                });
                        } else {
                            row.update({
                                semiWholeSalePrice: Math.round(data.wholeSalePrice * (1 + data.semiWholeSaleMargin / 100)),
                                retailPrice: Math.round(data.wholeSalePrice * (1 + data.retailMargin / 100))
                            });
                        }
                    } catch (err) {
                        row.update({
                            semiWholeSalePrice: Math.round(data.wholeSalePrice * (1 + data.semiWholeSaleMargin / 100)),
                            retailPrice: Math.round(data.wholeSalePrice * (1 + data.retailMargin / 100))
                        });
                    }
                }
                if (saveButton) saveButton.disabled = false;
            }
        };
    }

    function showMessage(message, isError) {
        resultsElement.textContent = message;
        resultsElement.classList.toggle('text-danger', Boolean(isError));
    }

    function showItemDetails(row) {
        const fields = {
            itemCode: 'detailItemCode',
            itemName: 'detailItemName',
            stockPosition: 'detailStockPosition',
            companyName: 'detailCompanyName',
            categoryName: 'detailCategoryName',
            subCategoryName: 'detailSubCategoryName',
            conversionName: 'detailConversionName',
            contains: 'detailContains',
            semiWholeSalePrice: 'detailSemiWholeSalePrice',
            retailPrice: 'detailRetailPrice'
        };

        Object.keys(fields).forEach(function (field) {
            document.getElementById(fields[field]).textContent = row[field] ?? '';
        });

        bootstrap.Modal.getOrCreateInstance(document.getElementById('inventoryItemDetails')).show();
    }

    async function saveChanges() {
        // Ensure any active cell editors are closed before manipulating rows/data.
        try {
            if (table && table.getElement) {
                // blur any inputs inside active editors so Tabulator can clear editors safely
                var activeEditors = table.getElement().querySelectorAll('.tabulator-editing input, .tabulator-editing textarea, .tabulator-editing select');
                activeEditors.forEach(function (el) { try { el.blur(); } catch (e) { } });
                try { if (document.activeElement) document.activeElement.blur(); } catch (e) { }

                // Prefer Tabulator API to cancel any active editor if available to avoid DOM timing issues
                try { if (typeof table.cancelEdit === 'function') table.cancelEdit(); } catch (e) { }

                // wait until Tabulator has cleared any editor DOM nodes to avoid redraw race
                const waitForNoEditors = async function (timeoutMs) {
                    const start = Date.now();
                    while (true) {
                        try {
                            const editing = table.getElement().querySelectorAll('.tabulator-editing');
                            if (!editing || editing.length === 0) break;
                        } catch (e) {
                            // if querying fails, break and proceed to avoid infinite loop
                            break;
                        }
                        if ((Date.now() - start) > timeoutMs) break;
                        // small sleep
                        // eslint-disable-next-line no-await-in-loop
                        await new Promise(function (res) { setTimeout(res, 50); });
                    }
                };
                await waitForNoEditors(1000);
            }
        } catch (e) { /* ignore editor-close failures */ }

        const changedItems = table.getData().filter(function (row) { return row._modified; });
        if (!changedItems.length) return;

        saveButton.disabled = true;
        saveButton.textContent = 'Saving…';
        const payload = changedItems.map(function (row) {
            return {
                itemId: row.itemId,
                wholeSalePrice: Number(row.wholeSalePrice),
                semiWholeSalePrice: Number(row.semiWholeSalePrice),
                retailPrice: Number(row.retailPrice)
            };
        });

        try {
            const response = await fetch(urls.update, {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': antiForgeryToken || ''
                },
                body: JSON.stringify(payload)
            });

            const result = await response.json().catch(function () { return {}; });

            if (response.ok && result && result.success !== false) {
                // success: show alert with server-provided message (or default); reload only after user dismisses alert
                var successMsg = (result && result.message) ? result.message : 'Inventory prices updated successfully.';
                try {
                    alert(successMsg);
                    try { window.location.reload(); } catch (e) { /* ignore reload errors */ }
                } catch (e) {
                    // If alert is blocked, fallback to in-page message and then reload after short delay
                    showMessage(successMsg, false);
                    setTimeout(function () { try { window.location.reload(); } catch (ee) { /* ignore */ } }, 900);
                }
            } else {
                // server returned error details or non-OK status
                const msg = (result && result.message) ? result.message : (response.statusText || 'Unable to save inventory prices.');
                showMessage(msg, true);
            }
        } catch (error) {
            showMessage(error && error.message ? error.message : 'Error saving inventory prices.', true);
        }

        // Always attempt to refresh the grid so UI reflects server state (success or failure)
        try {
            if (table && table.setData) await table.setData(urls.inventory, filters());
        } catch (reloadErr) {
            showMessage('Saved but failed to reload inventory grid: ' + (reloadErr && reloadErr.message ? reloadErr.message : ''), true);
        }

        // restore button state
        saveButton.textContent = 'Save price changes';
        try {
            saveButton.disabled = !(table && table.getData && table.getData().some(function (row) { return row._modified; }));
        } catch (e) {
            saveButton.disabled = true;
        }
    }

    function initialize() {
        autocomplete(itemInput, itemIdInput, urls.items, { minLength: 2 });
        autocomplete(companyInput, companyIdInput, urls.companies);
        autocomplete(categoryInput, categoryIdInput, urls.categories, {
            onSelect: function () {
                subcategoryInput.value = '';
                subcategoryIdInput.value = '';
            },
            onInput: function () {
                subcategoryInput.value = '';
                subcategoryIdInput.value = '';
            }
        });
        autocomplete(subcategoryInput, subcategoryIdInput, urls.subcategories, {
            categoryId: function () { return categoryIdInput.value; }
        });

        TabulatorHelper.createTabulator('#inventoryGrid', {
            ajaxURL: urls.inventory,
            ajaxConfig: 'GET',
            pagination: 'remote',
            paginationSize: 20,
            paginationSizeSelector: [10, 20, 50, 100],
            paginationDataSent: { page: 'page', size: 'pageSize' },
            paginationDataReceived: { data: 'data', last_page: 'totalPages', current_page: 'page' },
            layout: 'fitColumns',
            placeholder: 'No inventory items match these filters.',
            columns: [
                { title: 'Sl. No.', field: 'slNo', width: 35, hozAlign: 'center', headerSort: false },
                { title: 'Item Code', field: 'itemCode', width: 90 },
                { title: 'Item Name', field: 'itemName', minWidth: 210 },
                { title: 'Stock Position', field: 'stockPosition', width: 110 },
                priceColumn('Whole Sale Price', 'wholeSalePrice'),
                priceColumn('Semi Whole Sale Price', 'semiWholeSalePrice'),
                priceColumn('Retail Price', 'retailPrice'),
                { title: 'Company Name', field: 'companyName', minWidth: 100 },
                { title: 'Category', field: 'categoryName', minWidth: 120 },
                { title: 'Subcategory', field: 'subCategoryName', minWidth: 100 },
                { title: 'Conversion Name', field: 'conversionName', minWidth: 100 },
                { title: 'Contains', field: 'contains', minWidth: 100 },
                {
                    title: 'View',
                    width: 80,
                    hozAlign: 'center',
                    headerSort: false,
                    formatter: function () {
                        const button = document.createElement('button');
                        button.type = 'button';
                        button.className = 'btn btn-sm btn-outline-info';
                        button.textContent = 'View';
                        return button;
                    },
                    cellClick: function (_event, cell) {
                        showItemDetails(cell.getRow().getData());
                    }
                }
            ],
            ajaxResponse: function (_url, _params, response) {
                totalCount = Number(response?.total || 0);
                showMessage('Found ' + totalCount + ' item(s).', false);
                return response?.data || [];
            }
        }).then(function (grid) {
            table = grid;
            table.on('dataLoaded', updateSummary);
            if (saveButton) saveButton.disabled = true;
        }).catch(function (error) {
            showMessage('Unable to load the inventory grid.', true);
            console.error('Inventory grid initialization failed', error);
        });

        showWholeSale.addEventListener('change', function () {
            if (!table) return;
            table.toggleColumn('wholeSalePrice');
        });

        if (saveButton) saveButton.addEventListener('click', saveChanges);
        document.getElementById('exportInventory').addEventListener('click', function () {
            if (table) table.download('csv', 'inventory.csv');
        });
        document.getElementById('inventoryFilters').addEventListener('submit', function (event) {
            event.preventDefault();
            reloadGrid();
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize);
    } else {
        initialize();
    }
})();
