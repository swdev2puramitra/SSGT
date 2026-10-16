# FireWorks Billing Audit

## Purpose

This document records the FireWorks billing business requirements and the WebApp implementation completed for invoice splitting and billing audit work.

## Business requirements

1. Estimates are created before invoices. Direct invoice creation is rejected when no estimate exists.
2. Converting an estimate to an invoice changes `FW_Bills.IsEstimateInvoice` from `E` to `I`.
3. An invoice can be split by selected items only after it has been converted to an invoice.
4. A bill may have multiple invoice records. Invoice identity is based on `InvoiceId`, not `BillId`.
5. Splitting creates a new invoice with its own invoice number and moves the selected items to that invoice. The source invoice retains the remaining items.
6. Split invoices must retain correct item totals, taxable amounts, GST, discount, amount paid, and balance amount.
7. Split metadata is mandatory: owner/driver name, vehicle number, and E-Way Bill number.
8. Invoice GST is determined by state of supply:
   - In-state: 9% CGST plus 9% SGST.
   - Out-of-state: 18% IGST.
9. Invoice discount uses the invoice-level `DiscountPercentage` value.
10. Estimate quantity displays must show the numeric quantity only where required by the estimate layout.
11. Editing an invoice or split invoice must use `InvoiceId` and preserve the invoice item relationship.
12. Repeated or concurrent split/save requests must not create duplicate records or corrupt totals.
13. Print previews must show the correct invoice number, delivery metadata, discount, GST percentages, tax amounts, totals, paid amount, and balance.
14. A split operation must be recorded so the same invoice cannot be split repeatedly.

## Implemented WebApp behavior

- Added invoice splitting by selected items.
- Added invoice split history through `FW_Invoice_Split_History` and the script in `WebApp/Database/CREATE_FW_Invoice_Split_History.sql`.
- Added invoice-aware edit, list, split, and print-preview routing.
- Added invoice identity to billing requests and invoice item persistence.
- Added payment allocation between source and split invoices.
- Added fixed invoice GST rules for both the billing screen and server-side persistence.
- Updated invoice PDF rendering for invoice-level discount and state-specific GST.
- Added mandatory delivery fields to both billing and split-invoice screens.
- Added duplicate-submit protection to billing and split-invoice saves.
- Added transactional and validation checks around invoice conversion and splitting.
- Updated estimate conversion to mark the originating bill as invoiced.

## Database prerequisites

Run the following script against the application database before using invoice splitting:

```text
WebApp/Database/CREATE_FW_Invoice_Split_History.sql
```

The script creates the split-history table, foreign keys, and lookup indexes.

## Acceptance checklist

- [ ] Create an estimate with one or more items.
- [ ] Dispatch/load all estimate items.
- [ ] Convert the estimate to an invoice.
- [ ] Confirm `FW_Bills.IsEstimateInvoice = 'I'`.
- [ ] Confirm the generated invoice number is shown in View Bills.
- [ ] Open invoice edit and confirm it loads by `InvoiceId`.
- [ ] Split selected items into a new invoice.
- [ ] Confirm View Bills shows the source and split invoices as separate rows.
- [ ] Confirm the source invoice retains only its remaining items.
- [ ] Edit the split invoice and save it again.
- [ ] Confirm owner/driver name, vehicle number, and E-Way Bill number are required.
- [ ] Verify in-state GST is 9% CGST plus 9% SGST.
- [ ] Verify out-of-state GST is 18% IGST.
- [ ] Verify discount uses `DiscountPercentage`.
- [ ] Verify amount paid, balance, and total bill amount after splitting and editing.
- [ ] Open print preview for both in-state and out-of-state invoices.
- [ ] Click Generate/Save twice and confirm only one request is processed.
- [ ] Attempt to split the same invoice again and confirm the operation is rejected.

## Verification status

The WebApp builds successfully with zero errors. Automated test execution completes successfully; the checklist above is the remaining browser/database acceptance verification.

