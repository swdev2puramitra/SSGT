// Shared Tabulator initializer helper
window.TabulatorHelper = (function(){
    function ensureLoaded(){
        return new Promise(function(resolve){
            if(window.Tabulator) return resolve();
            // wait until Tabulator script loads
            var check = setInterval(function(){ if(window.Tabulator){ clearInterval(check); resolve(); } }, 50);
            // timeout after 5s
            setTimeout(function(){ clearInterval(check); resolve(); }, 5000);
        });
    }

    function createTabulator(selectorOrElement, options){
        return ensureLoaded().then(function(){
            var container = (typeof selectorOrElement === 'string') ? document.querySelector(selectorOrElement) : selectorOrElement;
            if(!container) throw new Error('Tabulator container not found: ' + selectorOrElement);

            // create wrapper if not present
            var wrapper = container.closest('.tabulator-wrapper');
            if(!wrapper){ wrapper = document.createElement('div'); wrapper.className = 'tabulator-wrapper'; container.parentNode.insertBefore(wrapper, container); wrapper.appendChild(container); }

            var defaults = {
                layout: 'fitColumns',
                layoutColumnsOnNewData: true,
                placeholder: 'No data',
                // sensible pagination defaults
                pagination: 'remote',
                paginationSize: 10,
                paginationSizeSelector: [15,30,45],
                // ensure page and size are sent using common names
                paginationDataSent: { page: 'page', size: 'size' },
                // tell Tabulator how to read common response shapes
                paginationDataReceived: { data: 'data', last_page: 'last_page', current_page: 'page' },
                ajaxConfig: 'GET',
                ajaxResponse: function(url, params, response){
                    // response expected: { data: [...], last_page: N }
                    try{
                        if(response && (response.last_page !== undefined)){
                            // set max page so paginator buttons enable correctly
                            this.setMaxPage(parseInt(response.last_page) || 1);
                        }
                            else{
                                // pagination module not available for this Tabulator build - render DOM fallback
                                try{
                                    console.debug('TabulatorHelper: pagination module not found, invoking fallback renderer', { currentPage: currentPage, lastPage: lastPage });
                                    renderFallbackPaginator(container, this, currentPage || (params && params.page) || 1, lastPage || 1);
                                }catch(e){ console.debug('TabulatorHelper: fallback render failed', e); }
                            }
                    }catch(e){ /* ignore */ }
                    if(response && response.data) return response.data;
                    return response || [];
                }
            };

            var cfg = Object.assign({}, defaults, options || {});

            // Fallback paginator renderer - define BEFORE ajaxResponse so it's available when needed
            function renderFallbackPaginator(containerEl, gridEl, currentPage, lastPage){
                try{
                    // Check if native Tabulator pagination is already present and working
                    var wrapperEl = containerEl.closest('.tabulator-wrapper') || containerEl.parentNode || document;
                    var nativePaginator = wrapperEl.querySelector('.tabulator-paginator');
                    if(nativePaginator){
                        // Native pagination exists, don't render fallback
                        return;
                    }

                    var footer = wrapperEl.querySelector('.tabulator-footer .tabulator-paginator, .tabulator-footer');
                    if(!footer){
                        // create a simple footer area
                        footer = document.createElement('div');
                        footer.className = 'tabulator-footer';
                        footer.style.padding = '6px';
                        footer.style.display = 'flex';
                        footer.style.alignItems = 'center';
                        footer.style.gap = '6px';
                        try{ wrapperEl.appendChild(footer); }catch(e){ containerEl.parentNode.appendChild(footer); }
                    }

                    // remove previous fallback pager if present
                    var existing = footer.querySelector('.tabulator-fallback-pager');
                    if(existing) existing.parentNode.removeChild(existing);

                    var pager = document.createElement('div');
                    pager.className = 'tabulator-fallback-pager';
                    pager.style.display = 'inline-flex';
                    pager.style.alignItems = 'center';
                    pager.style.gap = '4px';

                    // Prev
                    var prev = document.createElement('button'); prev.type = 'button'; prev.textContent = 'Prev'; prev.className = 'btn btn-sm btn-light';
                    prev.disabled = !(currentPage > 1);
                    prev.addEventListener('click', function(){ if(gridEl && gridEl.setPage) gridEl.setPage(Math.max(1, (currentPage||1)-1)); });
                    pager.appendChild(prev);

                    // page numbers (limit to reasonable count)
                    var maxButtons = 7;
                    var start = 1, end = lastPage || 1;
                    if(end > maxButtons){
                        var half = Math.floor(maxButtons/2);
                        start = Math.max(1, (currentPage||1) - half);
                        end = Math.min(lastPage, start + maxButtons - 1);
                        if(end - start < maxButtons - 1) start = Math.max(1, end - maxButtons + 1);
                    }
                    for(var i=start;i<=end;i++){
                        var b = document.createElement('button'); b.type='button'; b.textContent = String(i); b.className='btn btn-sm ' + ((i===currentPage) ? 'btn-primary' : 'btn-light');
                        (function(page){ b.addEventListener('click', function(){ if(gridEl && gridEl.setPage) gridEl.setPage(page); }); })(i);
                        pager.appendChild(b);
                    }

                    // Next
                    var next = document.createElement('button'); next.type='button'; next.textContent = 'Next'; next.className='btn btn-sm btn-light';
                    next.disabled = !(currentPage < (lastPage || 1));
                    next.addEventListener('click', function(){ if(gridEl && gridEl.setPage) gridEl.setPage(Math.min((lastPage||1), (currentPage||1)+1)); });
                    pager.appendChild(next);

                    footer.appendChild(pager);
                    console.debug('TabulatorHelper: rendered fallback paginator', { currentPage: currentPage, lastPage: lastPage, footer: footer });
                }catch(e){ /* ignore */ }
            }

            // Wrap ajaxResponse ONLY if user provided one - otherwise let Tabulator handle pagination natively
            var userAjaxResponse = cfg.ajaxResponse;
            if(typeof userAjaxResponse === 'function'){
                // User has custom ajaxResponse - wrap it to ensure pagination metadata is processed
                cfg.ajaxResponse = function(url, params, response){
                    var grid = this;  // Capture grid context

                    // Get pagination metadata from response before user handler
                    var paginationReceived = cfg.paginationDataReceived || { data: 'data', last_page: 'last_page', current_page: 'page' };
                    var lastPage = response && response[paginationReceived.last_page] ? parseInt(response[paginationReceived.last_page]) : undefined;
                    var currentPage = response && response[paginationReceived.current_page] ? parseInt(response[paginationReceived.current_page]) : (params && params.page ? parseInt(params.page) : undefined);

                    // Store for fallback paginator use
                    try{ this._tabulatorHelperLastPage = lastPage; } catch(e){}
                    try{ this._tabulatorHelperCurrentPage = currentPage; } catch(e){}

                    // Call user's custom ajaxResponse
                    var result = userAjaxResponse.apply(this, [url, params, response]);

                    // After user's handler, ensure pagination metadata is applied
                    if(lastPage !== undefined && typeof this.setMaxPage === 'function'){
                        try{ this.setMaxPage(lastPage); } catch(e){}
                    }

                    // Render fallback paginator if needed
                    try{
                        var mod = this.modules && this.modules.pagination;
                        if(!mod && lastPage !== undefined && lastPage > 1){
                            renderFallbackPaginator(container, this, currentPage || 1, lastPage);
                        }
                    }catch(e){}

                    return result;
                };
            }
            // If NO custom ajaxResponse, don't override - let Tabulator use paginationDataReceived natively

            var grid = new Tabulator(container, cfg);

            // listen for Tabulator dataLoaded event to re-apply paginator metadata after Tabulator internal work
            try{
                if(grid.on){
                    grid.on('dataLoaded', function(){
                        try{
                            var last = grid._tabulatorHelperLastPage;
                            var cur = grid._tabulatorHelperCurrentPage;
                            if(last !== undefined && last !== null){
                                try{ console.debug('TabulatorHelper: dataLoaded handler re-applying lastPage/currentPage', { last: last, current: cur }); }catch(e){}
                                try{ if(typeof grid.setMaxPage === 'function') grid.setMaxPage(last); }catch(e){}
                                try{ if(typeof grid.redraw === 'function') grid.redraw(true); }catch(e){}

                                // attempt module refresh or fallback render
                                try{
                                    var pg = null;
                                    try{
                                        // Suppress Tabulator's "module not installed" error before attempting to access pagination module
                                        var origErrorFn = window.console.error;
                                        try{ window.console.error = function(){} } catch(e){}
                                        try{
                                            if(typeof grid.module === 'function') pg = grid.module('pagination');
                                            else if(grid.table && typeof grid.table.module === 'function') pg = grid.table.module('pagination');
                                        }finally{
                                            try{ window.console.error = origErrorFn; } catch(e){}
                                        }
                                    }catch(e){ pg = null; }
                                    if(pg){
                                        try{ if(typeof pg.updateControls === 'function') pg.updateControls(); else if(typeof pg._updatePageButtons === 'function') pg._updatePageButtons(); }catch(e){}
                                        try{ if(cur) grid.setPage(cur); }catch(e){}
                                    } else {
                                        try{ renderFallbackPaginator(container, grid, cur || 1, last); }catch(e){}
                                    }
                                }catch(e){}
                            }
                        }catch(e){ /* ignore */ }
                    });
                }
            }catch(e){ /* ignore */ }

            // Expose instance on the container for easier debugging and cross-version access
            try{
                container._tabulator = grid;
                container._tabulatorHelper = grid;
                window.TabulatorHelper = window.TabulatorHelper || {};
                window.TabulatorHelper._lastGrid = grid;
            }catch(e){}

            // Post-init diagnostic and paginator refresh attempt (runs shortly after init)
            try{
                setTimeout(function(){
                    try{
                        console.debug('TabulatorHelper: post-init diagnostic for container', container);
                        if(grid && typeof grid.getPage === 'function') console.debug('TabulatorHelper: getPage ->', grid.getPage());
                        if(grid && typeof grid.getPageSize === 'function') console.debug('TabulatorHelper: getPageSize ->', grid.getPageSize());
                        if(grid && typeof grid.getMaxPage === 'function') console.debug('TabulatorHelper: getMaxPage ->', grid.getMaxPage());
                        var mod = grid && grid.modules && grid.modules.pagination;
                        console.debug('TabulatorHelper: pagination module present?', !!mod, mod);
                        if(mod){
                            if(typeof mod.updateControls === 'function') try{ mod.updateControls(); } catch(e){}
                            else if(typeof mod._updatePageButtons === 'function') try{ mod._updatePageButtons(); } catch(e){}
                        } else {
                            // No pagination module - render fallback paginator
                            try{
                                var currentPage = (typeof grid.getPage === 'function') ? grid.getPage() : 1;
                                var maxPage = (typeof grid.getMaxPage === 'function') ? grid.getMaxPage() : 1;
                                if(maxPage > 1){
                                    renderFallbackPaginator(container, grid, currentPage, maxPage);
                                }
                            }catch(e){}
                        }
                    }catch(e){}
                }, 250);
            }catch(e){}

            // Ensure page size changes trigger a reload (some Tabulator builds do not auto-refresh remote pages)
            try{
                if(grid.on){
                    grid.on('pageSizeChanged', function(newSize){
                        try{ grid.setPage(1); } catch(e){}
                    });
                }
            } catch(e){ /* ignore if event not supported */ }

            // Fallback: Tabulator footer select may not emit events in some builds; attach change listener to the footer select
            (function attachFooterSelectListener(){
                var wrapper = container.closest('.tabulator-wrapper') || container.parentNode;
                if(!wrapper) wrapper = document;
                var attempts = 0;
                var maxAttempts = 60; // ~3s
                var iv = setInterval(function(){
                    attempts++;
                    // look for paginator/size select inside this grid's wrapper
                    var sel = wrapper.querySelector('.tabulator-footer select, .tabulator-paginator select, .tabulator-footer .tabulator-paginator select, select.tabulator-page-size, select.pagination-size');
                    if(sel){
                        // avoid double-binding
                        if(!sel._tabulatorBound){
                            sel._tabulatorBound = true;
                            console.debug('TabulatorHelper: bound footer page-size select for container', container);
                            sel.addEventListener('change', function(){
                                try{
                                    var newSize = parseInt(this.value) || (grid.getPageSize ? grid.getPageSize() : undefined);
                                    console.debug('TabulatorHelper: footer select change detected, newSize=', newSize);
                                    if(newSize && grid.setPageSize) grid.setPageSize(newSize);
                                    // ensure remote reload
                                    if(grid.setPage){
                                        // prevent ajaxResponse wrapper from re-applying paginator updates while we programmatically change page
                                        try{ grid._tabulatorHelperInternalSetPage = true; } catch(e){}
                                        try{ grid.setPage(1); } catch(e){}
                                        try{ grid._tabulatorHelperInternalSetPage = false; } catch(e){}
                                    }
                                    // As extra fallback, try calling replaceData when available
                                    try{ if(grid.replaceData && typeof grid.replaceData === 'function') grid.replaceData(); } catch(e){}
                                }catch(e){ console.error('TabulatorHelper footer select handler error', e); }
                            });
                        }
                        clearInterval(iv);
                    }
                    if(attempts >= maxAttempts) clearInterval(iv);
                }, 50);
            })();

            // Delegated fallback: listen for change events on any select within the container
            // This helps when Tabulator injects controls in a non-standard place
            try{
                container.addEventListener('change', function(e){
                    var t = e.target;
                    if(t && t.tagName === 'SELECT'){
                        // Heuristic: check if select looks like a page-size selector (contains numeric options)
                        var opts = Array.from(t.options || []).map(function(o){ return o.value; }).filter(Boolean);
                        var numeric = opts.some(function(v){ return /^\d+$/.test(v); });
                        if(numeric){
                            console.debug('TabulatorHelper: delegated select change detected, treating as page-size change', t, opts);
                            var newSize = parseInt(t.value) || (grid.getPageSize ? grid.getPageSize() : undefined);
                            if(newSize && grid.setPageSize) grid.setPageSize(newSize);
                            if(grid.setPage) grid.setPage(1);
                        }
                    }
                });
            } catch(e){ /* ignore */ }

            // delay redraw to avoid inline-style race conditions
            setTimeout(function(){ try{ grid.redraw(true); } catch(e){} }, 120);

            return grid;
        });
    }

    return { createTabulator: createTabulator };
})();
