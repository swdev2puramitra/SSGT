(function () {
    'use strict';

    var config = window.stockPageConfig || {};
    var form = document.getElementById('stockReceiptForm');
    if (!form) return;

    var stockId = document.getElementById('stockId');
    var message = document.getElementById('stockFormMessage');
    var itemRequestVersion = 0;

    function showMessage(text, isError) {
        message.textContent = text;
        message.className = 'alert ' + (isError ? 'alert-danger' : 'alert-success');
    }

    function hideMessage() {
        message.textContent = '';
        message.className = 'alert d-none';
    }

    function getToken() {
        var token = form.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    function getGrid() {
        return window._stockReceiptGrid;
    }

    function getItemLabel(item) {
        return (item.itemCode || '') + (item.itemName ? ' - ' + item.itemName : '');
    }

    function getItemTypes(itemId) {
        return fetch(config.itemTypesUrl + '?itemId=' + encodeURIComponent(itemId))
            .then(function (response) { return response.json(); })
            .then(function (result) {
                return (result && result.success ? result.data : []).map(function (type) {
                    return {
                        itemTypeId: Number(type.itemTypeId),
                        typeName: type.typeName || '',
                        qtyPerType: Number(type.qtyPerType) || 1
                    };
                });
            });
    }

    function setItemTypes(row, types, selectedTypeId) {
        var rowData = row.getData();
        var selected = types.find(function (type) { return type.itemTypeId === Number(selectedTypeId); }) || types[0];
        var enteredQuantity = Number(rowData.enteredQuantity);
        if (!Number.isFinite(enteredQuantity) || enteredQuantity <= 0) {
            enteredQuantity = rowData.isExisting && selected
                ? Number(rowData.quantity || 0) / selected.qtyPerType
                : 1;
        }
        enteredQuantity = Math.max(1, Math.round(enteredQuantity));
        var normalizedQuantity = selected
            ? enteredQuantity * selected.qtyPerType
            : Number(rowData.quantity) || 0;
        row.update({
            typeOptions: types,
            itemTypeId: selected ? selected.itemTypeId : 0,
            itemTypeName: selected ? selected.typeName : '',
            qtyPerType: selected ? selected.qtyPerType : 1,
            enteredQuantity: enteredQuantity,
            quantity: normalizedQuantity
        });
        return selected;
    }

    function loadItemTypes(row, itemId, selectedTypeId) {
        if (!itemId) return Promise.resolve([]);
        return getItemTypes(itemId)
            .then(function (types) {
                setItemTypes(row, types, selectedTypeId);
                refreshDetails(row);
                return types;
            })
            .catch(function () {
                row.update({ typeOptions: [], itemTypeId: 0, itemTypeName: '', qtyPerType: 1 });
                return [];
            });
    }

    function refreshDetails(row) {
        var rowData = row.getData();
        var lowestQuantity = Number(rowData.quantity) || 0;
        if (!rowData.itemId || lowestQuantity <= 0 || !config.itemDescriptionUrl) return;

        fetch(config.itemDescriptionUrl + '?itemId=' + encodeURIComponent(rowData.itemId) + '&quantity=' + encodeURIComponent(lowestQuantity))
            .then(function (response) { return response.json(); })
            .then(function (result) {
                row.update({ stockDescription: result.success ? (result.data || '') : '' });
            })
            .catch(function () { row.update({ stockDescription: '' }); });
    }

    function createSuggestionEditor(cell, onRendered, success, cancel) {
        var container = document.createElement('div');
        var input = document.createElement('input');
        var list = document.createElement('div');
        var timer;
        var version = ++itemRequestVersion;
        var selected = false;

        container.style.position = 'relative';
        input.type = 'text';
        input.className = 'form-control form-control-sm';
        input.autocomplete = 'off';
        input.value = cell.getValue() || '';
        list.style.position = 'fixed';
        list.style.zIndex = '2000';
        list.style.backgroundColor = 'var(--bs-body-bg, #fff)';
        list.style.border = '1px solid #ced4da';
        list.style.maxHeight = '220px';
        list.style.overflowY = 'auto';
        list.style.display = 'none';
        container.appendChild(input);
        document.body.appendChild(list);

        function positionList() {
            var bounds = input.getBoundingClientRect();
            list.style.left = bounds.left + 'px';
            list.style.top = bounds.bottom + 'px';
            list.style.width = Math.max(bounds.width, 260) + 'px';
        }

        function hideList() {
            list.replaceChildren();
            list.style.display = 'none';
        }

        function choose(option) {
            var row = cell.getRow();
            var itemId = Number(option.id || option.itemId || option.ItemId);
            var duplicate = getGrid().getData().some(function (existingRow) {
                return existingRow.id !== row.getData().id && Number(existingRow.itemId) === itemId;
            });
            if (duplicate) {
                showMessage('This item has already been added. Select a different item.', true);
                input.value = '';
                selected = false;
                return;
            }

            selected = true;
            hideList();
            var label = option.label || option.Label || '';
            var parts = label.match(/^(.*?)\s*\((.*?)\)\s*$/);
            var itemName = option.itemName || option.ItemName || (parts ? parts[1] : label);
            var itemCode = option.itemCode || option.ItemCode || (parts ? parts[2] : '');
            row.update({
                itemId: itemId,
                itemCode: itemCode,
                itemName: itemName,
                itemLabel: label || getItemLabel({ itemCode: itemCode, itemName: itemName }),
                itemTypeId: 0,
                itemTypeName: '',
                typeOptions: [],
                qtyPerType: 1,
                enteredQuantity: 1,
                quantityConverted: false
            });
            input.value = label || getItemLabel({ itemCode: itemCode, itemName: itemName });
            loadItemTypes(row, row.getData().itemId).then(function () {
                success(input.value);
            });
        }

        function showSuggestions(result, term) {
            if (version !== itemRequestVersion || input.value.trim() !== term) return;
            hideList();
            (result || []).forEach(function (option) {
                var button = document.createElement('button');
                button.type = 'button';
                button.className = 'btn btn-link text-start w-100 p-1 ' + window.itemAutocompleteStatusClass(option);
                button.textContent = option.label || getItemLabel(option);
                button.addEventListener('mousedown', function (event) {
                    event.preventDefault();
                    choose(option);
                });
                list.appendChild(button);
            });
            if (list.childElementCount) {
                positionList();
                list.style.display = 'block';
            }
        }

        input.addEventListener('input', function () {
            selected = false;
            var term = input.value.trim();
            clearTimeout(timer);
            hideList();
            if (term.length < 2) return;
            timer = setTimeout(function () {
                fetch(config.suggestItemsUrl + '?term=' + encodeURIComponent(term))
                    .then(function (response) { return response.json(); })
                    .then(function (result) { showSuggestions(result, term); })
                    .catch(hideList);
            }, 250);
        });

        input.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                event.preventDefault();
                cancel();
            } else if (event.key === 'Enter' && list.firstElementChild) {
                event.preventDefault();
                list.firstElementChild.dispatchEvent(new MouseEvent('mousedown'));
            }
        });

        input.addEventListener('blur', function () {
            setTimeout(function () {
                hideList();
                if (!selected) cancel();
            }, 150);
        });

        onRendered(function () {
            input.focus();
            input.select();
            positionList();
        });
        return container;
    }

    function createItemTypeEditor(cell, onRendered, success, cancel) {
        var select = document.createElement('select');
        var row = cell.getRow();
        var rowData = row.getData();
        select.className = 'form-select form-select-sm';

        function populate(types) {
            select.replaceChildren(new Option('-- Select Type --', '0'));
            types.forEach(function (type) {
                select.appendChild(new Option(type.typeName, type.itemTypeId));
            });
            select.value = String(row.getData().itemTypeId || 0);
            select.disabled = Boolean(row.getData().quantityConverted) || !types.length;
        }

        var types = rowData.typeOptions || [];
        if (types.length) {
            populate(types);
        } else if (rowData.itemId) {
            select.disabled = true;
            getItemTypes(rowData.itemId).then(function (loadedTypes) {
                setItemTypes(row, loadedTypes, rowData.itemTypeId);
                populate(loadedTypes);
                select.disabled = false;
            });
        } else {
            populate([]);
        }

        select.addEventListener('change', function () {
            var selectedType = (row.getData().typeOptions || []).find(function (type) {
                return type.itemTypeId === Number(select.value);
            });
            var enteredQuantity = Number(row.getData().enteredQuantity) || 1;
            var normalizedQuantity = enteredQuantity * (selectedType ? selectedType.qtyPerType : 1);
            row.update({
                itemTypeId: Number(select.value),
                itemTypeName: selectedType ? selectedType.typeName : '',
                qtyPerType: selectedType ? selectedType.qtyPerType : 1,
                quantity: normalizedQuantity,
                stockDescription: ''
            });
            refreshDetails(row);
            success(selectedType ? selectedType.typeName : '');
        });
        select.addEventListener('blur', function () {
            if (select.value) {
                var selectedType = (row.getData().typeOptions || []).find(function (type) {
                    return type.itemTypeId === Number(select.value);
                });
                success(selectedType ? selectedType.typeName : '');
            }
        });
        select.addEventListener('keydown', function (event) { if (event.key === 'Escape') cancel(); });
        onRendered(function () { select.focus(); });
        return select;
    }

    function createQuantityEditor(cell, onRendered, success, cancel) {
        var input = document.createElement('input');
        var committed = false;
        input.type = 'number';
        input.min = '1';
        input.step = '1';
        input.className = 'form-control form-control-sm';
        var row = cell.getRow();
        var rowData = row.getData();
        var qtyPerType = Number(rowData.qtyPerType) || 1;
        input.value = rowData.enteredQuantity || (Number(cell.getValue()) / qtyPerType) || '';
        function commit() {
            if (committed) return;
            committed = true;
            var value = Number(input.value);
            var normalizedQuantity = value * (Number(row.getData().qtyPerType) || 1);
            row.update({ quantity: normalizedQuantity, enteredQuantity: value, quantityConverted: true, stockDescription: '' });
            refreshDetails(row);
            success(normalizedQuantity);
        }
        input.addEventListener('change', commit);
        input.addEventListener('blur', commit);
        input.addEventListener('keydown', function (event) {
            if (event.key === 'Enter') commit();
            if (event.key === 'Escape') cancel();
        });
        onRendered(function () { input.focus(); input.select(); });
        return input;
    }

    function createGrid() {
        if (!window.Tabulator) {
            setTimeout(createGrid, 50);
            return;
        }

        window._stockReceiptGrid = new Tabulator('#stockReceiptGrid', {
            layout: 'fitColumns',
            placeholder: 'No stock items added',
            reactiveData: true,
            index: 'id',
            columns: [
                { title: 'Sl No', formatter: 'rownum', width: 55, hozAlign: 'center' },
                { title: 'Item Code / Name', field: 'itemLabel', editor: createSuggestionEditor, minWidth: 160, widthGrow: 3 },
                { title: 'Item Type', field: 'itemTypeName', editor: createItemTypeEditor, minWidth: 95, widthGrow: 1 },
                { title: 'Quantity', field: 'quantity', editor: createQuantityEditor, validator: ['required', 'integer', 'min:1'], hozAlign: 'right', minWidth: 85, widthGrow: 1 },
                {
                    title: 'Detailed', field: 'stockDescription', minWidth: 150, widthGrow: 2, formatter: function (cell) {
                        var row = cell.getRow().getData();
                        var span = document.createElement('span');
                        span.textContent = row.stockDescription || '-';
                        if (!row.stockDescription && row.itemId && row.quantity && config.itemDescriptionUrl) {
                            fetch(config.itemDescriptionUrl + '?itemId=' + encodeURIComponent(row.itemId) + '&quantity=' + encodeURIComponent(row.quantity))
                                .then(function (response) { return response.json(); })
                                .then(function (result) {
                                    span.textContent = result.success && result.data ? result.data : '-';
                                    row.stockDescription = span.textContent;
                                })
                                .catch(function () { span.textContent = '-'; });
                        }
                        return span;
                    }
                },
                { title: 'Delete', formatter: 'buttonCross', width: 60, hozAlign: 'center', cellClick: function (event, cell) { cell.getRow().delete(); } }
            ],
            rowAdded: function (row) {
                if (!row.getData().id) row.update({ id: Date.now() + Math.random() });
            }
        });
        loadForEdit();
    }

    function addBlankRow() {
        var grid = getGrid();
        grid.addRow({ id: Date.now() + Math.random(), itemId: 0, itemLabel: '', itemTypeId: 0, itemTypeName: '', quantity: 0, enteredQuantity: 1, quantityConverted: false, typeOptions: [] }, true)
            .then(function (row) { row.getCell('itemLabel').edit(); });
    }

    function loadForEdit() {
        if (!config.editStockId) return;
        fetch(config.receiptUrl + '?stockId=' + encodeURIComponent(config.editStockId))
            .then(function (response) { return response.json(); })
            .then(function (row) {
                stockId.value = row.stockId;
                var gridRows = (row.items || []).map(function (item, index) {
                    return {
                        id: row.stockId + '-' + item.itemId + '-' + index,
                        itemId: item.itemId,
                        itemCode: item.itemCode || '',
                        itemName: item.itemName || '',
                        itemLabel: getItemLabel({ itemCode: item.itemCode || '', itemName: item.itemName || '' }),
                        itemTypeId: item.itemTypeId || 0,
                        itemTypeName: item.itemTypeName || '',
                        quantity: item.quantity,
                        enteredQuantity: null,
                        quantityConverted: true,
                        isExisting: true,
                        stockDescription: '',
                        typeOptions: []
                    };
                });
                getGrid().setData(gridRows);
                getGrid().getRows().forEach(function (gridRow, index) {
                    loadItemTypes(gridRow, gridRows[index].itemId, gridRows[index].itemTypeId);
                });
                var godownSelect = document.getElementById('godownId');
                godownSelect.value = row.godownId;
                godownSelect.disabled = true;
                document.getElementById('stockFrom').value = row.stockFrom || '';
                document.getElementById('invoiceNo').value = row.invoiceNo || '';
                document.getElementById('arrivalDate').value = row.arrivalDate || '';
                document.getElementById('comments').value = row.comments || '';
            }).catch(function () { showMessage('Unable to load stock entry.', true); });
    }

    document.getElementById('addStockItemButton').addEventListener('click', function () {
        hideMessage();
        addBlankRow();
    });

    form.addEventListener('submit', function (event) {
        event.preventDefault();
        hideMessage();
        var rows = getGrid().getData();
        var itemIds = {};
        if (!rows.length) {
            showMessage('Add at least one stock item.', true);
            return;
        }
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (!row.itemId || !row.itemTypeId || !Number.isInteger(Number(row.quantity)) || Number(row.quantity) <= 0) {
                showMessage('Complete the item, item type, and positive quantity for every row.', true);
                return;
            }
            if (itemIds[row.itemId]) {
                showMessage('The same item cannot be added more than once.', true);
                return;
            }
            itemIds[row.itemId] = true;
        }

        var payload = {
            stockId: stockId.value ? Number(stockId.value) : null,
            godownId: Number(document.getElementById('godownId').value),
            stockFrom: document.getElementById('stockFrom').value,
            invoiceNo: document.getElementById('invoiceNo').value,
            arrivalDate: document.getElementById('arrivalDate').value,
            comments: document.getElementById('comments').value,
            isBaseQuantity: true,
            items: rows.map(function (row) {
                return {
                    itemId: Number(row.itemId),
                    itemCode: row.itemCode || '',
                    itemName: row.itemName || '',
                    itemTypeId: Number(row.itemTypeId),
                    itemTypeName: row.itemTypeName || '',
                    quantity: Number(row.quantity)
                };
            })
        };

        fetch(config.saveUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getToken() },
            body: JSON.stringify(payload)
        })
            .then(function (response) {
                return response.json().then(function (result) {
                    if (!response.ok) throw new Error(result.message || 'Unable to save stock.');
                    return result;
                });
            })
            .then(function () { showMessage('Stock saved successfully.', false); resetForm(); })
            .catch(function (error) { showMessage(error.message, true); });
    });

    function resetForm() {
        form.reset();
        stockId.value = '';
        getGrid().setData([]);
        document.getElementById('arrivalDate').value = new Date().toISOString().slice(0, 10);
        config.editStockId = '';
        document.getElementById('godownId').disabled = false;
        window.history.replaceState({}, document.title, window.location.pathname);
    }

    document.getElementById('clearStockButton').addEventListener('click', function () {
        resetForm();
        hideMessage();
    });

    document.getElementById('arrivalDate').value = new Date().toISOString().slice(0, 10);
    createGrid();
})();
