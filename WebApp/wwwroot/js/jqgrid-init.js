// Shared jqGrid initializer helper
// Plug & play grid initialization for consistent grid behavior across pages
// Usage: JqGridHelper.createGrid('#gridId', options, dataUrl, dataParams)

window.JqGridHelper = (function() {
    'use strict';

    /**
     * Ensures jQuery, jQuery UI, and jqGrid are loaded
     */
    function ensureLoaded() {
        return new Promise(function(resolve) {
            if (window.jQuery && window.jQuery.jgrid) {
                return resolve();
            }

            var attempts = 0;
            var maxAttempts = 100; // 5 seconds with 50ms interval
            var check = setInterval(function() {
                attempts++;
                if (window.jQuery && window.jQuery.jgrid) {
                    clearInterval(check);
                    resolve();
                } else if (attempts >= maxAttempts) {
                    clearInterval(check);
                    console.warn('JqGridHelper: jqGrid timeout - libraries may not be fully loaded');
                    resolve();
                }
            }, 50);
        });
    }

    /**
     * Create a jqGrid with plug & play configuration
     * @param {string} selector - jQuery selector for grid element (e.g., '#gridId')
     * @param {object} columnConfig - Array of column definitions
     * @param {string} dataUrl - URL to fetch data from
     * @param {object} options - Additional jqGrid options (optional)
     * @returns {Promise} Resolves when grid is ready
     */
    function createGrid(selector, columnConfig, dataUrl, options) {
        return ensureLoaded().then(function() {
            var $grid = jQuery(selector);

            if ($grid.length === 0) {
                throw new Error('JqGridHelper: Grid container not found: ' + selector);
            }

            // Ensure wrapper for styling
            var $wrapper = $grid.closest('.jqgrid-wrapper');
            if ($wrapper.length === 0) {
                $wrapper = jQuery('<div class="jqgrid-wrapper"></div>');
                $grid.wrap($wrapper);
            }

            // Base configuration - sensible defaults
            var baseConfig = {
                url: dataUrl,
                datatype: 'json',
                mtype: 'GET',
                colModel: columnConfig,
                rowNum: 10,
                rowList: [5, 10, 20, 50],
                pager: selector + '_pager',
                pgbuttons: true,
                pginput: true,
                viewrecords: true,
                height: 'auto',
                autowidth: true,
                shrinkToFit: true,
                rownumbers: false,
                loadonce: false,
                // Do not set global default sort here to keep helper plug-&-play.
                // Per-grid default sort can be provided via options when calling createPaginatedGrid.
                loadError: function(xhr, status, error) {
                    console.error('JqGridHelper: Data load error', { status: status, error: error });
                },
                loadBeforeSend: function(xhr) {
                    // Can be used for adding headers if needed
                },
                gridComplete: function() {
                    // Called after grid data is loaded
                    try {
                        var $g = jQuery(selector);
                        // Resize grid to the container width and keep explicit column widths (shrink=false)
                        var resizeGrid = function() {
                            var containerWidth = $g.closest('.jqgrid-wrapper').width() || $g.parent().width();
                            if (containerWidth) {
                                // false = do not shrink columns, keep explicit widths
                                $g.jqGrid('setGridWidth', containerWidth, false);
                            }
                            // Ensure header columns use the same box-sizing/padding as body cells
                            $g.closest('.ui-jqgrid').find('.ui-jqgrid-htable th div').css({ 'box-sizing': 'border-box' });
                            $g.find('td').css({ 'box-sizing': 'border-box' });
                        };

                        // Defer resize slightly to allow DOM measurements to stabilize
                        setTimeout(function() {
                            resizeGrid();
                            // After sizing is complete, remove or hide the temporary jqgfirstrow spacer so it doesn't remain visible
                            try {
                                var $firstRow = $g.find('tr.jqgfirstrow');
                                if ($firstRow.length) {
                                    $firstRow.remove();
                                }
                                // Also remove any id-based spacer
                                $g.find('#jqg_first_row').remove();
                            } catch (e) {
                                // ignore
                            }
                        }, 50);

                        // Ensure grid resizes correctly when the window/container resizes
                        if (!$g.data('jqgrid-init-resize-bound')) {
                            jQuery(window).on('resize.jqgrid.' + selector.substring(1), function() {
                                try { resizeGrid(); } catch (e) { /* swallow */ }
                            });
                            $g.data('jqgrid-init-resize-bound', true);
                        }
                    } catch (ex) {
                        console.warn('JqGridHelper: grid resize failed', ex);
                    }
                    jQuery(selector).trigger('jqgrid.ready');
                }
            };

            // Merge with user options
            var config = jQuery.extend(true, baseConfig, options || {});

            // Create pager div if it doesn't exist
            if (jQuery(selector + '_pager').length === 0) {
                jQuery('<div id="' + selector.substring(1) + '_pager"></div>').insertAfter(selector);
            }

            // Initialize jqGrid
            $grid.jqGrid(config);

            // Add responsive class for styling
            $grid.closest('.jqgrid-wrapper').addClass('jqgrid-responsive');

            return {
                grid: $grid,
                reload: function(params) {
                    $grid.jqGrid('setGridParam', {
                        url: dataUrl,
                        postData: params || {}
                    }).trigger('reloadGrid', [{ page: 1 }]);
                },
                getSelectedRow: function() {
                    return $grid.jqGrid('getGridParam', 'selrow');
                },
                getSelectedRows: function() {
                    return $grid.jqGrid('getGridParam', 'selarrrow');
                }
            };
        });
    }

    /**
     * Create a grid with remote pagination (recommended approach)
     * @param {string} selector - jQuery selector for grid element
     * @param {object} columnConfig - Array of column definitions
     * @param {string} dataUrl - URL for pagination data
     * @param {object} options - Additional options
     * @returns {Promise} Resolves when grid is ready
     */
    function createPaginatedGrid(selector, columnConfig, dataUrl, options) {
        var paginatedConfig = {
            datatype: 'json',
            rowNum: (options && options.rowNum) || 10,
            rowList: (options && options.rowList) || [5, 10, 20, 50],
            viewrecords: true,
            jsonReader: {
                page: 'page',
                total: 'total',
                records: 'records',
                root: 'data',
                repeatitems: false,
                id: 'transferId' // Primary key - can be overridden in options
            }
        };

        var mergedOptions = jQuery.extend(true, paginatedConfig, options || {});
        return createGrid(selector, columnConfig, dataUrl, mergedOptions);
    }

    /**
     * Format currency values
     */
    function formatCurrency(value) {
        if (!value) return '0.00';
        return parseFloat(value).toFixed(2);
    }

    /**
     * Format date values
     */
    function formatDate(value) {
        if (!value) return '';
        var date = new Date(value);
        if (isNaN(date.getTime())) return value;
        return date.toLocaleDateString('en-IN', { 
            year: 'numeric', 
            month: '2-digit', 
            day: '2-digit' 
        });
    }

    /**
     * Create action button HTML (e.g., Edit, Delete)
     */
    function createActionButton(label, id, cssClass, dataAttribute) {
        var buttonHtml = '<button type="button" class="' + cssClass + '" ' +
                        'data-id="' + id + '"' +
                        (dataAttribute ? ' data-' + Object.keys(dataAttribute)[0] + '="' + dataAttribute[Object.keys(dataAttribute)[0]] + '"' : '') +
                        '>' + label + '</button>';
        return buttonHtml;
    }

    /**
     * Attach event handler to grid action buttons
     * @param {string} selector - Grid selector
     * @param {string} buttonClass - CSS class of the button (e.g., 'edit-transfer')
     * @param {function} callback - Callback function receiving (id, data)
     */
    function attachActionHandler(selector, buttonClass, callback) {
        jQuery(document).off('click', selector + ' .' + buttonClass);
        jQuery(document).on('click', selector + ' .' + buttonClass, function(e) {
            e.preventDefault();
            var id = jQuery(this).data('id');
            var rowData = jQuery(selector).jqGrid('getRowData', id);
            if (callback && typeof callback === 'function') {
                callback(id, rowData);
            }
        });
    }

    /**
     * Public API
     */
    return {
        createGrid: createGrid,
        createPaginatedGrid: createPaginatedGrid,
        formatCurrency: formatCurrency,
        formatDate: formatDate,
        createActionButton: createActionButton,
        attachActionHandler: attachActionHandler,

        // Utility: Get jQuery instance
        getJQuery: function() {
            return jQuery;
        }
    };
})();
