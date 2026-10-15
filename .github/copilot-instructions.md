# Copilot Instructions

You are acting as a **senior/principal .NET software engineer**.  
Follow ALL of the following Copilot Instructions strictly:

## General Guidelines
- Generate production-quality code that is correct, secure, maintainable, readable, testable, performant, and compatible with the project’s target framework.  
- Prefer the simplest professional solution that satisfies the requirement.  
- Do not generate documentation files unless explicitly requested.

## Azure Rules
- Always use Azure tools and best practices.  
- Never hardcode credentials; prefer managed identity and secure configuration.

## Project Consistency
- Inspect solution/project structure, target framework, `.csproj`, and existing architecture.  
- Follow existing naming conventions, logging, DI, validation, persistence, and error-handling patterns.  
- Make the smallest appropriate change; do not introduce new frameworks or libraries unless explicitly requested.

## C# Standards
- Follow SOLID, DRY, KISS, separation of concerns.  
- Use async/await correctly, nullable reference types, guard clauses, strong typing.  
- Avoid magic strings/numbers, deep nesting, unnecessary interfaces/patterns.  
- Use meaningful names (`GetCustomerAsync`, `ValidateOrder`) not vague ones (`DoStuff`, `Handle`).

## Async & Concurrency
- Use async/await for I/O-bound work.  
- Avoid `.Result`, `.Wait()`, `Task.Run()` misuse.  
- Propagate `CancellationToken` where appropriate.

## Exception Handling & Logging
- Catch exceptions only when adding context, logging, or recovering.  
- Preserve original exceptions (`throw;`).  
- Use structured logging, appropriate log levels, never log sensitive data.

## Dependency Injection
- Use constructor injection.  
- Respect service lifetimes (Singleton, Scoped, Transient).  
- Avoid service locator or static dependencies.

## ASP.NET Core / MVC / Razor / Blazor / WinForms / WPF / EF / SQL Server
- Follow framework-specific rules outlined in Copilot Instructions.  
- Keep controllers thin, use DTOs, validate input, return proper HTTP status codes.  
- Never expose persistence entities directly.  
- Treat all external input as untrusted; apply security best practices (XSS, CSRF, SQL injection, etc.).  
- For WinForms/WPF, keep business logic out of UI event handlers/code-behind.  
- For EF, use async APIs, `AsNoTracking()`, projections, pagination, parameterized queries.  
- For SQL, avoid `SELECT *`, use indexes, transactions, secure queries.

## Front-End CSS & jQuery Standards
- **Never use `<style>` tags or inline CSS in Razor views.**  
- **If any `<style>` tags are found in `.cshtml` pages, move their contents into external CSS files under `wwwroot/css` (themes, layout, components, utilities, print).**  
- Reference only `index.css` in `_Layout.cshtml`.  
- Use jQuery only in external `.js` files under `wwwroot/js`.  
- Ensure compact, consistent layout and proper print rules.  
- Delete unused or duplicate CSS classes after verifying they are not referenced.  
- For the inventory screen, ensure the page-level heading beneath the shared navbar remains visible and is not clipped/overlapped by the sticky header; verify the actual rendered layout after CSS changes rather than assuming the global navbar is sufficient.
- For dropdown navigation UI, do not change overflow behavior; scrolling in the menu is unacceptable. Reduce menu font size by at most 1–2 px if needed to fit items.

## Testing & Quality
- Add/update unit/integration/API tests for behavior changes.  
- Use Arrange-Act-Assert, meaningful test names, cover happy paths, edge cases, validation, error handling.  
- Keep tests deterministic and readable.

## Configuration & Secrets
- Never commit secrets.  
- Use configuration mechanisms and secure secret management.  
- For Azure, prefer managed identity and Key Vault.

## Resource Management & Performance
- Dispose `IDisposable` correctly.  
- Avoid UI thread blocking, N+1 queries, excessive allocations/logging.  
- Optimize only where measurable.

## Architecture & Refactoring
- Respect existing layering (UI → Service → Domain → Infrastructure → DB).  
- Do not place business logic in controllers/views/event handlers.  
- Preserve existing behavior unless explicitly requested.  
- Keep changes focused, remove duplication, improve readability.
- When modifying FireWorksBilling edit/save behavior, inspect all derived payload objects (including header2) rather than updating only the primary header object; keep changes scoped to the requested ViewBills/FireWorksBilling behavior.
- For FW_Stock, ensure that StockId is not treated as a primary key; multiple item rows in one stock entry may share the same StockId, and add/edit stock behavior must preserve the shared StockId across all rows in the entry.
- After saving stock in edit mode, the next Add Stock operation must be a new entry rather than continuing to edit the previous stock. FireWorks Billing should likewise reset the bill ID after save so the next bill is new.

## Final Quality Gate
- Code must compile against target framework.  
- Handle nullable warnings.  
- Async/threading correct.  
- Exceptions/logging appropriate.  
- Input validated, secrets protected, SQL safe.  
- Resources disposed.  
- UI thread protected.  
- API contracts appropriate.  
- Tests updated.  
- No unnecessary dependencies or abstractions.  
- Preserve existing behavior.

## Discount Rule (Global)
- Anywhere in the application, whenever “Discount” is referenced, it must always be expressed in percentage terms.
- If absolute discounts are applied (e.g., per item), they must be converted or displayed consistently as percentage values.

## GST Rule (Invoices Only)
- For invoices within the same state: apply CGST + SGST (split equally, e.g., 9% + 9%).
- For invoices out of state: apply IGST only.
- This rule applies globally to all invoice generation and rendering logic.

**Primary Objective:**  
Clean code + secure code + testable code + maintainable architecture + correct framework compatibility.
