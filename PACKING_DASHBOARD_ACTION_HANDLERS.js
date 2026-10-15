// ============================================================
// Packing Dashboard - Action Handlers Implementation Guide
// ============================================================

// This file provides examples for implementing the action handlers
// Integrate this code into WebApp/Views/Home/PackingDashboard.cshtml
// or create a separate WebApp/wwwroot/js/packing-dashboard.js

// ============================================================
// 1. PACKING ACTION
// ============================================================

function handlePacking(billId) {
    console.log('Initiating packing for bill:', billId);

    // Option A: Open packing dialog
    // openPackingDialog(billId);

    // Option B: Navigate to packing page
    // window.location.href = '@Url.Action("Packing", "Home")' + '?billId=' + billId;

    // Option C: Show confirmation dialog then proceed
    if (confirm('Proceed with packing for Bill ID: ' + billId + '?')) {
        // Redirect to packing page
        window.location.href = '/Home/Packing?billId=' + billId;
    }
}

// ============================================================
// 2. EDIT ACTION
// ============================================================

function handleEdit(billId) {
    console.log('Edit bill:', billId);

    // Navigate to bill edit page
    // Adjust the controller/action based on your routing
    window.location.href = '@Url.Action("EditBill", "Bills")' + '?id=' + billId;

    // Alternative: Open in new tab (Shift + Click behavior)
    // window.open('/Bills/EditBill?id=' + billId, '_blank');
}

// ============================================================
// 3. PRINT ACTION
// ============================================================

function handlePrint(billId) {
    console.log('Print bill:', billId);

    // Open print page in new tab
    const printUrl = '@Url.Action("PrintBill", "Bills")' + '?id=' + billId;
    window.open(printUrl, '_blank', 'width=1000,height=800');

    // Alternative: Print directly after page loads
    // const printWindow = window.open(printUrl, '_blank');
    // printWindow.onload = function() {
    //     printWindow.print();
    // };
}

// ============================================================
// 4. GRID ROW SELECTION HANDLER (Optional)
// ============================================================

function setupGridRowSelection() {
    const gridDiv = document.getElementById('pacingGrid');

    // Assuming gridOptions is already defined in PackingDashboard.cshtml
    if (window.gridOptions) {
        window.gridOptions.onRowClicked = function(event) {
            console.log('Row clicked:', event.data);

            // Highlight selected row
            // Grid automatically handles this with selectedRowBackColor

            // Optional: Show bill details in a sidebar or modal
            // showBillDetails(event.data.billId);
        };

        window.gridOptions.onSelectionChanged = function() {
            const selectedRows = window.gridOptions.api.getSelectedRows();
            console.log('Selected rows:', selectedRows);

            if (selectedRows.length > 0) {
                // Enable bulk actions if needed
                // enableBulkPacking(selectedRows.map(r => r.billId));
            }
        };
    }
}

// ============================================================
// 5. BULK PACKING ACTION (Optional Enhancement)
// ============================================================

function enableBulkPacking(billIds) {
    console.log('Bulk packing for bills:', billIds);

    if (billIds.length === 0) {
        alert('Please select at least one bill');
        return;
    }

    if (confirm('Pack ' + billIds.length + ' bills?')) {
        // POST request to backend bulk action
        fetch('@Url.Action("BulkPacking", "Home")', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({ billIds: billIds })
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert('Packing initiated for ' + data.count + ' bills');
                // Refresh grid
                window.location.reload();
            } else {
                alert('Error: ' + data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred during bulk packing');
        });
    }
}

// ============================================================
// 6. FILTER FORM SUBMISSION (Auto-submit on Enter)
// ============================================================

function setupFilterFormBehavior() {
    const filterForm = document.querySelector('.filter-form');

    if (filterForm) {
        // Auto-submit form on Enter key in any input
        filterForm.addEventListener('keypress', function(e) {
            if (e.key === 'Enter' && e.target.tagName !== 'TEXTAREA') {
                e.preventDefault();
                filterForm.submit();
            }
        });

        // Add autocomplete to customer name (optional)
        const nameInput = document.getElementById('name');
        if (nameInput) {
            nameInput.addEventListener('input', function() {
                // Fetch customer suggestions
                // fetchCustomerSuggestions(this.value);
            });
        }
    }
}

// ============================================================
// 7. EXPORT FUNCTIONALITY (Optional)
// ============================================================

function exportToExcel() {
    if (window.gridOptions && window.gridOptions.api) {
        const filename = 'Bills_' + new Date().toISOString().split('T')[0] + '.xlsx';
        window.gridOptions.api.exportDataAsExcel({ fileName: filename });
    }
}

function exportToCsv() {
    if (window.gridOptions && window.gridOptions.api) {
        const filename = 'Bills_' + new Date().toISOString().split('T')[0] + '.csv';
        window.gridOptions.api.exportDataAsCsv({ fileName: filename });
    }
}

// ============================================================
// 8. INITIALIZATION (Call on page load)
// ============================================================

document.addEventListener('DOMContentLoaded', function() {
    console.log('Packing Dashboard initialized');

    // Setup filter form behavior
    setupFilterFormBehavior();

    // Setup grid row selection
    // setupGridRowSelection();

    // Add keyboard shortcuts (optional)
    // setupKeyboardShortcuts();
});

// ============================================================
// 9. BACKEND CONTROLLER EXAMPLES
// ============================================================

/*
// HomeController.cs examples:

[HttpGet]
public IActionResult Packing(long billId)
{
    LoadMenusFromSession();

    try
    {
        var bill = _context.Set<BillsListDto>()
            .FromSqlRaw("EXEC USP_GET_BILL_DETAILS @BillId = {0}", billId)
            .AsNoTracking()
            .FirstOrDefault();

        if (bill == null)
        {
            return NotFound("Bill not found");
        }

        return View(bill);
    }
    catch (Exception ex)
    {
        ViewBag.Error = "Error loading bill details";
        return View();
    }
}

[HttpPost]
public IActionResult BulkPacking([FromBody] List<long> billIds)
{
    try
    {
        if (billIds == null || billIds.Count == 0)
        {
            return Json(new { success = false, message = "No bills selected" });
        }

        // TODO: Implement bulk packing logic
        // Mark bills as packed in database

        return Json(new { success = true, count = billIds.Count });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}
*/

// ============================================================
// 10. UTILITY FUNCTIONS
// ============================================================

function showBillDetails(billId) {
    // Implement modal or sidebar to show bill details
    console.log('Showing details for bill:', billId);
}

function showNotification(message, type = 'info') {
    // Implement toast notification
    const alert = document.createElement('div');
    alert.className = `alert alert-${type}`;
    alert.textContent = message;
    document.body.appendChild(alert);

    setTimeout(() => {
        alert.remove();
    }, 3000);
}

function confirmAction(message) {
    return confirm(message);
}

// ============================================================
// END OF IMPLEMENTATION GUIDE
// ============================================================
