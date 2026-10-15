/*
 * jqGrid locale file - English (United States)
 * Provides localization strings and formatter defaults
 */

if(jQuery.jgrid) {
    jQuery.jgrid = jQuery.jgrid || {};

    // Define default formatter functions
    jQuery.jgrid.formatter = jQuery.jgrid.formatter || {};
    jQuery.jgrid.formatter.integer = function(cellValue, options) {
        return cellValue ? cellValue.toString() : '';
    };
    jQuery.jgrid.formatter.number = function(cellValue, options) {
        if (!cellValue) return '';
        var precision = options.colModel && options.colModel.formatoptions ? options.colModel.formatoptions.decimalPlaces : 2;
        return parseFloat(cellValue).toFixed(precision);
    };
    jQuery.jgrid.formatter.currency = function(cellValue, options) {
        if (!cellValue) return '';
        var prefix = options.colModel && options.colModel.formatoptions ? options.colModel.formatoptions.prefix : '$';
        var precision = options.colModel && options.colModel.formatoptions ? options.colModel.formatoptions.decimalPlaces : 2;
        return prefix + ' ' + parseFloat(cellValue).toFixed(precision);
    };
    jQuery.jgrid.formatter.date = function(cellValue, options) {
        if (!cellValue) return '';
        var newdate = new Date(cellValue);
        if(isNaN(newdate.getTime())) return cellValue;
        return newdate.toLocaleDateString('en-IN', { year: 'numeric', month: '2-digit', day: '2-digit' });
    };
    jQuery.jgrid.formatter.checkbox = function(cellValue, options) {
        return '<input type="checkbox" ' + (cellValue == '1' || cellValue == true ? 'checked' : '') + ' disabled />';
    };
    jQuery.jgrid.formatter.link = function(cellValue, options) {
        return '<a href="' + cellValue + '">' + cellValue + '</a>';
    };
    jQuery.jgrid.formatter.showlink = function(cellValue, options) {
        return '<a href="javascript:void(0);">' + cellValue + '</a>';
    };
    jQuery.jgrid.formatter.email = function(cellValue, options) {
        return '<a href="mailto:' + cellValue + '">' + cellValue + '</a>';
    };
    jQuery.jgrid.formatter.select = function(cellValue, options) {
        return cellValue;
    };
    jQuery.jgrid.formatter.actions = function(cellValue, options) {
        return cellValue;
    };

    // Define unformat functions (for edit mode)
    jQuery.jgrid.unformat = jQuery.jgrid.unformat || {};
    jQuery.jgrid.unformat.integer = function(cellValue, options) {
        return cellValue ? parseInt(cellValue, 10) : 0;
    };
    jQuery.jgrid.unformat.number = function(cellValue, options) {
        return cellValue ? parseFloat(cellValue) : 0;
    };
    jQuery.jgrid.unformat.currency = function(cellValue, options) {
        if (!cellValue) return 0;
        return parseFloat(cellValue.replace(/[$,]/g, ''));
    };
    jQuery.jgrid.unformat.date = function(cellValue, options) {
        return cellValue;
    };
}
