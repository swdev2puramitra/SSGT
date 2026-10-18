(function (window) {
    'use strict';

    function quantityOf(item) {
        var value = item && (item.stockQuantity ?? item.StockQuantity ?? item.quantity ?? item.Quantity);
        var quantity = Number(value);
        return Number.isFinite(quantity) ? quantity : 0;
    }

    window.itemAutocompleteStatusClass = function (item) {
        var quantity = quantityOf(item);
        return quantity <= 0 ? 'item-stock-empty' : quantity <= 5 ? 'item-stock-low' : 'item-stock-available';
    };

    window.applyItemStockRowStatus = function (row, data) {
        var element = row && row.getElement ? row.getElement() : null;
        if (!element) return;
        element.classList.remove('item-stock-empty', 'item-stock-low', 'item-stock-available');
        element.classList.add(window.itemAutocompleteStatusClass(data || (row.getData ? row.getData() : {})));
    };

    window.renderStockAutocompleteItem = function (ul, item, label) {
        var li = $('<li>');
        var div = $('<div>')
            .addClass(window.itemAutocompleteStatusClass(item))
            .text(label || item.label || item.value || '');
        li.append(div).appendTo(ul);
        return li;
    };
})(window);
