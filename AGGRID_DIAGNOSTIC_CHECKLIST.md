# AG Grid Diagnostic Checklist

Follow these steps to diagnose what's happening:

## Pre-Test Setup

- [ ] Hard refresh browser: `Ctrl+Shift+R`
- [ ] Close all browser tabs showing localhost:5133
- [ ] Open Visual Studio
- [ ] Build solution: `Ctrl+Shift+B`
- [ ] Verify build says "Build successful"

## Step 1: Start the Application

- [ ] Run the application (F5 or Ctrl+F5)
- [ ] Wait for page to fully load
- [ ] Navigate to: `http://localhost:5133/Home/ViewBills`

## Step 2: Open Browser Developer Console

- [ ] Press `F12` to open Developer Tools
- [ ] Click "Console" tab
- [ ] Ensure you're seeing **live console output** (not cached)

## Step 3: Check for Debug Messages

Look in the Console for messages like:

```
=== AG Grid Debug Info ===
1. typeof window.agGrid: object
2. window.agGrid exists? true
3. agGrid is: Object
4. typeof window.agGrid.Grid: function
5. First 30 agGrid properties: Grid, KeyCode, ...
```

### If You See ✅
- [ ] Messages appearing in console
- [ ] typeof window.agGrid shows "object"
- [ ] typeof window.agGrid.Grid shows "function"

**→ Skip to Step 5**

### If You Don't See These Messages ❌
- [ ] Go to Step 4

## Step 4: Diagnose Missing Debug Messages

**In Console, type:**
```javascript
typeof window.agGrid
```

**Then press Enter.**

### What You Should See:

**BEST CASE ✅:** 
```
undefined  (at first, then later becomes)
object
```

**PROBLEM CASE ❌:**
```
undefined  (and stays undefined)
```

---

**If undefined, check these:**

### 4a: Verify File Exists

In Console, type:
```javascript
fetch('/lib/AGGrid/ag-grid-community.min.js')
	.then(r => console.log('Status:', r.status, 'Size:', r.headers.get('content-length')))
```

**Expected:** Status: 200, Size: 2020715

**If you see 404:** File path is wrong or doesn't exist

### 4b: Check File is Loading

- [ ] Press F12
- [ ] Go to "Network" tab
- [ ] Filter for: `ag-grid`
- [ ] Look for: `ag-grid-community.min.js`

**Should show:**
- [ ] Name: `ag-grid-community.min.js`
- [ ] Status: `200` (not 404)
- [ ] Size: `2 MB` (shows as "2 MB" or "~2000 KB")
- [ ] Type: `script`

**If Status is 404:**
- The file doesn't exist at `~/lib/AGGrid/`
- Check the path is correct
- Verify file exists: `F:\Projects\TFS\SSGT\Application\WebApp\wwwroot\lib\AGGrid\ag-grid-community.min.js`

**If Status is 200 but size is small (<100 KB):**
- The file may have been corrupted during download
- Delete it and re-download

---

### 4c: Check CSS Loading

In Network tab, also look for:
- [ ] `ag-grid.min.css` - Status: 200
- [ ] `ag-grid-theme.css` - Status: 200

**If any show 404:**
- Update the path in `_Layout.cshtml` to use correct folder name

---

## Step 5: Check Grid Container

**In Console, type:**
```javascript
document.getElementById('pacingGrid')
```

### What You Should See:

**✅ GOOD:**
```html
<div id="pacingGrid" class="ag-theme-custom" style="height: 600px; width: 100%;"></div>
```

**❌ BAD:**
```
null
```

**If you see null:**
- The div doesn't exist in the HTML
- Check `ViewBills.cshtml` around line 96 has:
  ```html
  <div id="pacingGrid" class="ag-theme-custom" style="height: 600px; width: 100%;"></div>
  ```

---

## Step 6: Check for Grid Creation

Look in Console for this message:
```
✓ Creating AG Grid with 1 rows
```

- [ ] Message appears (within 1 second of page load)
- [ ] Number of rows is correct (should be 1)

### If Yes:
- [ ] Look for success message: `✓✓ AG Grid initialized successfully!`
- [ ] If present → **GRID IS WORKING!** ✅
- [ ] If not present → Grid creation failed, check for error messages

### If No:
- [ ] Look for warning: `⏳ AG Grid not yet loaded, retrying in 500ms...`
- [ ] Look for error: `❌ agGrid.Grid is not a function!`

**If you see the retry message multiple times:**
- Library is loading very slowly
- Wait 2-3 seconds and check if it eventually succeeds
- If it keeps retrying forever, there's a library loading problem

**If you see the "not a function" error:**
- Library loaded but didn't export Grid properly
- Possible wrong version of AG Grid file

---

## Step 7: Check for Other Errors

Scroll through the Console and look for any **red error messages**.

### Common Errors:

**If you see:**
```
GET http://localhost:5133/lib/... 404 (Not Found)
```
→ A file path is wrong. Check file name and folder path.

**If you see:**
```
Uncaught TypeError: Cannot read property 'Grid' of undefined
```
→ `window.agGrid` is undefined. File not loading.

**If you see:**
```
Uncaught TypeError: agGrid.Grid is not a constructor
```
→ Library loaded but Grid not available yet. Check timing.

---

## Step 8: Final Check - Is Grid Visible?

Look at the actual page (not console):

- [ ] Do you see the grid? (below the filter form)
- [ ] Can you see column headers?
- [ ] Can you see data rows?
- [ ] Are the buttons visible (Packing, Edit, Print)?

### If YES ✅
Everything is working! The grid is displayed and functional.

### If NO ❌
- [ ] Grid container is there but empty?
- [ ] Filter form is visible?
- [ ] "Found 1 bill(s)" text is visible?

**If container is there but empty:**
- CSS might be hiding it
- Grid might still be initializing
- Wait 2-3 seconds and check again
- Check if "Grid initialized successfully" appears in console

---

## Reporting Issues

If the grid still doesn't work, provide:

1. **Screenshot of Console output** showing:
   - All debug messages (the `=== AG Grid Debug Info ===` section)
   - Any error messages (red text)
   - Specifically the value of `typeof window.agGrid.Grid`

2. **Screenshot of Network tab** showing:
   - ag-grid-community.min.js (status and size)
   - ag-grid.min.css (status)
   - ag-grid-theme.css (status)

3. **Browser type and version:**
   - (e.g., Chrome 120, Firefox 121, Edge 120)

4. **Page URL:**
   - Exactly what shows in address bar

5. **Console output:**
   - Copy/paste entire console text

---

## Quick Reference: Expected Console Output

```
DOMContentLoaded fired
After 100ms delay:
=== AG Grid Debug Info ===
1. typeof window.agGrid: undefined
2. window.agGrid exists? false

After 500ms delay:
=== AG Grid Debug Info ===
1. typeof window.agGrid: object
2. window.agGrid exists? true
3. agGrid is: Object
4. typeof window.agGrid.Grid: function
5. First 30 agGrid properties: Grid, KeyCode, Constants, ... (truncated)
✓ window.agGrid.Grid is available

Window load event fired
=== AG Grid Debug Info ===
1. typeof window.agGrid: object
2. typeof window.agGrid.Grid: function
✓ window.agGrid.Grid is available

✓ Creating AG Grid with 1 rows
✓✓ AG Grid initialized successfully!
```

If you see this sequence, everything is working perfectly! ✅

---

## Build Verification

Before testing, verify the build was successful:

```
Visual Studio → Build → Build Solution
```

**Should see in build output window:**
```
========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========
```

If build fails, all bets are off!

---

## Common Solutions Quick Reference

| Problem | Solution |
|---------|----------|
| Files show 404 | Wrong path in script/link tags |
| Grid not initialized | Wait longer (check console messages) |
| Grid is null | Check HTML has the div element |
| TypeError: not a constructor | Timing issue (already fixed by defer attribute) |
| Grid visible but no data | Check rowData being passed correctly |
| Grid visible but frozen | CSS issue, check theme CSS |

---

**Start from Step 1 and follow through!**
