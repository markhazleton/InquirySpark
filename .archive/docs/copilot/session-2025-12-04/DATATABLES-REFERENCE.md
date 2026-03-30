# DataTables Quick Reference - InquirySpark.Admin

## Overview
This document provides quick examples and reference for using DataTables in the InquirySpark.Admin application with Bootstrap 5 styling.

## Basic Table Setup

### Standard DataTable
```html
<table class="table table-striped table-hover">
    <thead>
        <tr>
            <th>Name</th>
            <th>Position</th>
            <th>Office</th>
            <th>Start Date</th>
            <th>Salary</th>
            <th class="no-sort">Actions</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>John Doe</td>
            <td>Developer</td>
            <td>New York</td>
            <td>2023-01-15</td>
            <td>$85,000</td>
            <td>
                <a href="#" class="btn btn-sm btn-primary">Edit</a>
                <a href="#" class="btn btn-sm btn-danger">Delete</a>
            </td>
        </tr>
        <!-- More rows... -->
    </tbody>
</table>
```

**Features enabled:**
- ? Sorting (except Actions column)
- ? Searching
- ? Pagination (25 per page default)
- ? Responsive design
- ? State saving (24 hours)

### Table with Export Buttons
```html
<div class="card">
    <div class="card-header">
        <h5>Employee Report</h5>
    </div>
    <div class="card-body">
        <table class="table table-striped datatable-export">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Department</th>
                    <th>Phone</th>
                </tr>
            </thead>
            <tbody>
                <!-- Data rows -->
            </tbody>
        </table>
    </div>
</div>
```

**Additional features:**
- ? Copy to clipboard
- ? Export to CSV
- ? Export to Excel
- ? Export to PDF
- ? Print view

## Column Configuration

### Disable Sorting on Specific Columns
```html
<th class="no-sort">Actions</th>
<th class="no-sort">Select</th>
```

### Disable Searching on Specific Columns
```html
<th class="no-search">Hidden ID</th>
<th class="no-search">Internal Notes</th>
```

### Combine Both
```html
<th class="no-sort no-search">Static Column</th>
```

## Custom Configuration Examples

### Custom Page Length
```html
<table class="table" 
       data-datatables-config='{"pageLength": 50}'>
    <!-- Table content -->
</table>
```

### Custom Sort Order
```html
<!-- Sort by 3rd column descending, then 1st column ascending -->
<table class="table" 
       data-datatables-config='{"order": [[2, "desc"], [0, "asc"]]}'>
    <!-- Table content -->
</table>
```

### Disable State Saving
```html
<table class="table" 
       data-datatables-config='{"stateSave": false}'>
    <!-- Table content -->
</table>
```

### Custom Page Length Options
```html
<table class="table" 
       data-datatables-config='{"lengthMenu": [[5, 10, 25, 50], [5, 10, 25, 50]]}'>
    <!-- Table content -->
</table>
```

## Responsive Priority

Control which columns hide first on smaller screens using `data-priority`:

```html
<thead>
    <tr>
        <th data-priority="1">Name</th>        <!-- Never hides -->
        <th data-priority="2">Email</th>       <!-- Hides last -->
        <th data-priority="3">Phone</th>       <!-- Hides second to last -->
        <th>Department</th>                    <!-- Hides first -->
        <th data-priority="1">Actions</th>     <!-- Never hides -->
    </tr>
</thead>
```

## Bootstrap Integration

### Bootstrap Table Classes
DataTables works seamlessly with Bootstrap table classes:

```html
<table class="table table-striped table-hover table-bordered table-sm">
    <!-- Combines Bootstrap styling with DataTables functionality -->
</table>
```

### Contextual Classes
```html
<tbody>
    <tr class="table-success">
        <td>Active User</td>
    </tr>
    <tr class="table-warning">
        <td>Pending User</td>
    </tr>
    <tr class="table-danger">
        <td>Inactive User</td>
    </tr>
</tbody>
```

## JavaScript API Examples

### Get DataTable Instance
```javascript
const table = $('#myTable').DataTable();
```

### Search Programmatically
```javascript
// Global search
table.search('search term').draw();

// Clear search
table.search('').draw();

// Column-specific search (0-indexed)
table.column(0).search('search term').draw();
```

### Reload Data (AJAX)
```javascript
table.ajax.reload();
```

### Add Row Dynamically
```javascript
table.row.add([
    'John Doe',
    'Developer',
    'New York',
    '2023-01-15',
    '$85,000'
]).draw();
```

### Remove Row
```javascript
table.row('#rowId').remove().draw();
```

### Update Cell
```javascript
table.cell('#rowId', 1).data('New Value').draw();
```

### Get Selected Rows
```javascript
const selected = table.rows({ selected: true }).data().toArray();
console.log(selected);
```

### Clear All Filters
```javascript
table.search('').columns().search('').draw();
```

## Server-Side Processing

For large datasets (>10,000 rows), use server-side processing:

```html
<table class="table" 
       data-datatables-config='{
           "processing": true,
           "serverSide": true,
           "ajax": {
               "url": "/api/data",
               "type": "POST"
           }
       }'>
    <!-- Table structure -->
</table>
```

## Common Patterns

### Master-Detail Pattern
```html
<table class="table" id="masterTable">
    <thead>
        <tr>
            <th></th> <!-- Expand icon -->
            <th>Name</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr data-detail="Additional information here">
            <td class="dt-control"></td>
            <td>Item 1</td>
            <td>Summary</td>
        </tr>
    </tbody>
</table>

<script>
$(document).ready(function() {
    const table = $('#masterTable').DataTable();
    
    $('#masterTable tbody').on('click', 'td.dt-control', function() {
        const tr = $(this).closest('tr');
        const row = table.row(tr);
        
        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
        } else {
            row.child('<div class="p-3">' + tr.data('detail') + '</div>').show();
            tr.addClass('shown');
        }
    });
});
</script>
```

### Editable Table
```html
<table class="table" id="editableTable">
    <!-- Table structure -->
</table>

<script>
$('#editableTable tbody').on('click', 'td', function() {
    const cell = table.cell(this);
    const originalValue = cell.data();
    
    // Make cell editable
    $(this).html('<input type="text" value="' + originalValue + '">');
    
    $(this).find('input').on('blur', function() {
        const newValue = $(this).val();
        cell.data(newValue).draw();
        
        // Save to server
        $.post('/api/update', { value: newValue });
    });
});
</script>
```

## Troubleshooting

### Table Not Initializing
1. Check browser console for errors
2. Verify jQuery is loaded before DataTables
3. Ensure table has `<thead>` and `<tbody>` elements
4. Check for JavaScript errors in site.js

### Export Buttons Not Working
- Verify JSZip is loaded for Excel export
- Verify PDFMake is loaded for PDF export
- Check that buttons extension CSS/JS are loaded
- Use class `datatable-export` on table

### Responsive Not Working
- Ensure responsive CSS is loaded
- Add `data-priority` attributes to important columns
- Check viewport meta tag in layout

### State Not Saving
- Check browser localStorage is enabled
- Verify stateSave option is true
- Clear browser cache if corrupted

## Performance Tips

1. **Use `deferRender`**: Already enabled by default in site.js
2. **Limit initial rows**: Set reasonable `pageLength`
3. **Server-side processing**: For 10,000+ rows
4. **Optimize HTML**: Remove unnecessary nested elements
5. **Lazy loading**: Load data via AJAX when possible

## Bootstrap Icons Integration

Use Bootstrap Icons in your tables:

```html
<td>
    <a href="#" class="btn btn-sm btn-outline-primary">
        <i class="bi bi-pencil"></i> Edit
    </a>
    <a href="#" class="btn btn-sm btn-outline-danger">
        <i class="bi bi-trash"></i> Delete
    </a>
</td>
```

Common icons:
- `bi-pencil` - Edit
- `bi-trash` - Delete
- `bi-eye` - View
- `bi-plus-circle` - Add
- `bi-check-circle` - Approve
- `bi-x-circle` - Reject
- `bi-download` - Download
- `bi-upload` - Upload

## Resources

- [DataTables Official Documentation](https://datatables.net/)
- [Bootstrap 5 Examples](https://datatables.net/examples/styling/bootstrap5.html)
- [DataTables API Reference](https://datatables.net/reference/api/)
- [Bootstrap Icons](https://icons.getbootstrap.com/)
- [jQuery Documentation](https://api.jquery.com/)

## Support

For issues specific to InquirySpark.Admin implementation:
1. Check browser console for errors
2. Verify all required files are loaded in correct order
3. Review site.js for configuration
4. Check NPM-BUILD.md for setup instructions
