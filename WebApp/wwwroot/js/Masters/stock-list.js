(function () {
    'use strict';

    var config = window.stockListConfig || {};

    function formatStockDate(value) {
        if (!value) return '';
        var date = new Date(value);
        return isNaN(date.getTime()) ? value : date.toLocaleString();
    }

    function getToken() {
        var token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    function getFilters() {
        return {
            stockFrom: document.getElementById('stockFromFilter').value.trim(),
            invoiceNo: document.getElementById('invoiceNoFilter').value.trim(),
            itemId: document.getElementById('itemIdFilter').value || ''
        };
    }

    function reload() {
        if (!window._tabulator_table) return;
        window._tabulator_table.setData(config.listUrl, getFilters());
    }

    window.formatStockDate = formatStockDate;
    window.deleteStock = function (stockId) {
        if (!window.confirm('Delete this stock entry?')) return;
        fetch(config.deleteUrl + '?stockId=' + encodeURIComponent(stockId), {
            method: 'POST',
            headers: { 'RequestVerificationToken': getToken() }
        })
            .then(function (response) {
                return response.json().then(function (result) {
                    if (!response.ok) throw new Error(result.message || 'Unable to delete stock.');
                    return result;
                });
            })
            .then(reload)
            .catch(function (error) { window.alert(error.message); });
    };

    function initialize() {
        var attempts = 0;
        var timer = setInterval(function () {
            attempts += 1;
            if (window._tabulator_table) {
                clearInterval(timer);
                var filterForm = document.getElementById('stockListFilters');
                filterForm.addEventListener('submit', function (event) {
                    event.preventDefault();
                    reload();
                });
                initializeItemAutocomplete();
                document.getElementById('clearStockFilters').addEventListener('click', function () {
                    document.getElementById('stockFromFilter').value = '';
                    document.getElementById('invoiceNoFilter').value = '';
                    document.getElementById('itemFilter').value = '';
                    document.getElementById('itemIdFilter').value = '';
                    var suggestions = document.getElementById('itemFilterSuggestions');
                    if (suggestions) {
                        suggestions.replaceChildren();
                        suggestions.style.display = 'none';
                    }
                    reload();
                });
            } else if (attempts >= 100) {
                clearInterval(timer);
            }
        }, 50);
    }

    function initializeItemAutocomplete() {
        var input = document.getElementById('itemFilter');
        var hidden = document.getElementById('itemIdFilter');
        var list = document.getElementById('itemFilterSuggestions');
        if (!input || !hidden || !list || !config.suggestItemsUrl) return;

        var timer;
        var requestVersion = 0;

        function hideItemSuggestions() {
            list.replaceChildren();
            list.style.display = 'none';
        }

        function choose(option) {
            hidden.value = String(option.id || option.Id || '');
            input.value = option.label || option.Label || option.value || option.Value || '';
            hideItemSuggestions();
            reload();
        }

        input.addEventListener('input', function () {
            hidden.value = '';
            clearTimeout(timer);
            hideItemSuggestions();
            var term = input.value.trim();
            if (term.length < 2) return;
            var version = ++requestVersion;
            timer = setTimeout(function () {
                fetch(config.suggestItemsUrl + '?term=' + encodeURIComponent(term))
                    .then(function (response) { return response.json(); })
                    .then(function (options) {
                        if (version !== requestVersion || input.value.trim() !== term) return;
                        (options || []).forEach(function (option) {
                            var button = document.createElement('button');
                            button.type = 'button';
                            button.textContent = option.label || option.Label || '';
                            button.addEventListener('mousedown', function (event) {
                                event.preventDefault();
                                choose(option);
                            });
                            list.appendChild(button);
                        });
                        if (list.childElementCount) list.style.display = 'block';
                    })
                    .catch(hideItemSuggestions);
            }, 250);
        });

        input.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') hideItemSuggestions();
            if (event.key === 'Enter' && hidden.value) {
                event.preventDefault();
                reload();
                hideItemSuggestions();
            }
        });
        input.addEventListener('blur', function () { setTimeout(hideItemSuggestions, 150); });
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initialize);
    else initialize();
})();
