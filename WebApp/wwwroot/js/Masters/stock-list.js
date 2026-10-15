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
            invoiceNo: document.getElementById('invoiceNoFilter').value.trim()
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
                document.getElementById('clearStockFilters').addEventListener('click', function () {
                    document.getElementById('stockFromFilter').value = '';
                    document.getElementById('invoiceNoFilter').value = '';
                    reload();
                });
            } else if (attempts >= 100) {
                clearInterval(timer);
            }
        }, 50);
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initialize);
    else initialize();
})();
