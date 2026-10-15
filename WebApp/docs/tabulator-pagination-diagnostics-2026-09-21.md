# Tabulator pagination diagnostics (2026-09-21)

This document captures the diagnostics, changes and findings from troubleshooting Tabulator remote pagination behavior for the Stock Transfers list (ViewStockTransfers).

## Problem summary

- Paginator footer did not show page numbers and Next/Last buttons were disabled or not present, even though server responses included multiple pages of data and last_page metadata.
- Page-size changes sometimes did not trigger remote reloads consistently across Tabulator builds.
- Later issue: After implementing fallback paginator, DUPLICATE pagination controls were rendering (native + fallback), causing confusion and non-functional buttons in the second paginator.

## Changes applied (code)

1. Centralized Tabulator initializer: WebApp/wwwroot/js/tabulator-init.js
   - All Tabulator instances are created via TabulatorHelper.createTabulator.
   - Defaults include pagination: 'remote', paginationDataSent and paginationDataReceived mapping, and sensible paginationSize.
   - ajaxResponse wrapper added to:
	 - Read server pagination metadata (last_page, current_page) and call setMaxPage.
	 - Attempt to refresh Tabulator's pagination module controls (updateControls, _updatePageButtons, initialize) when available.
	 - Store pagination metadata (_tabulatorHelperLastPage, _tabulatorHelperCurrentPage) on grid object for fallback paginator use.
	 - Force redraws after metadata changes.
	 - Call renderFallbackPaginator at end of ajaxResponse (with native pagination check).
   - Fallback paginator renderer added:
	 - Creates HTML-based pagination controls (prev, page numbers, next buttons) when native Tabulator pagination module is not available.
	 - Checks for native .tabulator-paginator element before rendering to avoid duplicates.
	 - Handles page selection and disable/enable button states.
   - Footer page-size select binding and delegated change detection added to reliably detect page-size changes and call setPageSize + setPage(1).
   - Internal suppression flag (_tabulatorHelperInternalSetPage) added to avoid recursion when calling setPage programmatically.
   - Exposed instance references: container._tabulator and window.TabulatorHelper._lastGrid for easier debug.
   - Removed: Diagnostics download button (temporary debug feature).

2. Module error suppression
   - Added error suppression for "Tabulator module not installed: pagination" when attempting to access pagination module via this.module('pagination').
   - Error is suppressed before the call, then console.error is restored, allowing graceful fallback without console noise.

## Findings so far

- The server responses include `last_page` correctly (e.g., last_page: 2) and the helper calls setMaxPage(2).
- In a recent run the pagination module (grid.modules.pagination) reported `false` / undefined, meaning the pagination module was not exposed on the table instance in this Tabulator build.
- The footer page-size select was present and bound; ajaxResponse received the response and diagnostics were recorded.
- **Latest finding (2026-09-24)**: Native Tabulator pagination IS present and working. Fallback paginator was being rendered unconditionally alongside native pagination, creating duplicate controls.
  - Symptom: Two pagination footers visible, second one not functional
  - Root cause: renderFallbackPaginator called without checking if native .tabulator-paginator element exists
  - Fix: Added native pagination detection in renderFallbackPaginator — returns early if .tabulator-paginator already present

## Current behavior (after latest fix)

- renderFallbackPaginator now checks for native .tabulator-paginator element before rendering
- If native pagination is present, fallback renderer returns early (no duplicate)
- Only the native Tabulator pagination control renders
- Page navigation, page size selection, and all pagination features work correctly
- No duplicate or non-functional pagination controls

## Next diagnostic steps

If pagination issues re-occur:
1. Reload the page and check browser dev tools for .tabulator-paginator element presence
2. Verify in console: `document.querySelector('.tabulator-paginator')` should return an element
3. Check grid state: `window.TabulatorHelper._lastGrid.getPage()`, `.getPageSize()`, `.getMaxPage()`
4. If fallback IS needed (no .tabulator-paginator), verify renderFallbackPaginator is being called and rendering correctly

## Temporary / Production notes

- The console logging (console.debug) is currently enabled for troubleshooting. Should be reduced/removed before production deployment.
- The fallback paginator is a robust HTML-based implementation that works even if Tabulator module is not available.
- The native Tabulator pagination is preferred when available and will be used automatically.

## Files touched

- WebApp/wwwroot/js/tabulator-init.js (modified multiple times)
- WebApp/Views/Billing/ViewStockTransfers.cshtml (removed custom ajaxResponse override)
- WebApp/docs/tabulator-pagination-diagnostics-2026-09-21.md (updated)

---
_Last updated: 2026-09-24 (duplicate pagination fix)_
