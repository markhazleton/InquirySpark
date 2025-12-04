# Bootstrap 5 Table Template - Best Practices

## Standard Table Pattern

Use this template for all Index.cshtml views:

```razor
@model IEnumerable<YourModel>

@{
    ViewData["Title"] = "Your Title";
}

<div class="card border-0 shadow-sm">
    <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">
            <i class="bi bi-[icon-name] me-2"></i>
            Your Title
        </h5>
        <a asp-action="Create" class="btn btn-light btn-sm">
            <i class="bi bi-plus-circle me-1"></i>
            Create New
        </a>
    </div>
    <div class="card-body p-0">
        <div class="table-responsive">
            <table class="table table-striped table-hover align-middle mb-0">
                <thead class="table-light">
                    <tr>
                        <th><i class="bi bi-[icon] me-1"></i>Column Name</th>
                        <!-- Add more columns -->
                        <th class="text-center no-sort" style="width: 180px;">
                            <i class="bi bi-gear me-1"></i>Actions
                        </th>
                    </tr>
                </thead>
                <tbody>
@foreach (var item in Model) {
                    <tr>
                        <td>@Html.DisplayFor(modelItem => item.Property)</td>
                        <!-- Add more cells -->
                        <td class="text-center">
                            <div class="btn-group btn-group-sm" role="group">
                                <a asp-action="Details" asp-route-id="@item.Id" 
                                   class="btn btn-outline-info" title="View Details">
                                    <i class="bi bi-eye"></i>
                                </a>
                                <a asp-action="Edit" asp-route-id="@item.Id" 
                                   class="btn btn-outline-primary" title="Edit">
                                    <i class="bi bi-pencil"></i>
                                </a>
                                <a asp-action="Delete" asp-route-id="@item.Id" 
                                   class="btn btn-outline-danger" title="Delete">
                                    <i class="bi bi-trash"></i>
                                </a>
                            </div>
                        </td>
                    </tr>
}
                </tbody>
            </table>
        </div>
    </div>
    <div class="card-footer text-muted d-flex justify-content-between align-items-center">
        <small>
            <i class="bi bi-info-circle me-1"></i>
            Total Records: <strong>@Model.Count()</strong>
        </small>
        <small>
            <i class="bi bi-lightning-charge me-1"></i>
            Powered by DataTables
        </small>
    </div>
</div>
```

## Bootstrap 5 Classes Used

### Card Component
- `card` - Main card container
- `border-0` - Remove borders for modern look
- `shadow-sm` - Subtle shadow for depth

### Card Header
- `card-header` - Header section
- `bg-primary text-white` - Primary color background with white text
- `d-flex justify-content-between align-items-center` - Flexbox layout

### Card Body
- `card-body` - Body section
- `p-0` - Remove padding to let table fill entire space

### Table Classes
- `table` - Base table class (required for DataTables)
- `table-striped` - Zebra striping (theme-aware)
- `table-hover` - Hover effects (theme-aware)
- `align-middle` - Vertical alignment
- `mb-0` - Remove bottom margin
- `table-responsive` - Mobile responsive wrapper

### Table Header
- `table-light` - Light background for header (theme-aware)

### Button Groups
- `btn-group` - Button group container
- `btn-group-sm` - Small button size
- `btn-outline-*` - Outline button variants (theme-aware)

### Utility Classes
- `text-center` - Center align
- `text-muted` - Muted text color (theme-aware)
- `badge bg-*` - Bootstrap badges (theme-aware)
- `me-1`, `me-2` - Margin end (right) spacing
- `no-sort` - Custom class to disable DataTables sorting

## Bootstrap Icons Recommendations

### By Category

**General:**
- `bi-table` - Tables
- `bi-list` - Lists
- `bi-grid` - Grid view
- `bi-layout-text-window` - Layout

**Actions:**
- `bi-plus-circle` - Create/Add
- `bi-pencil` - Edit
- `bi-trash` - Delete
- `bi-eye` - View/Details
- `bi-gear` - Settings
- `bi-check-circle` - Approve
- `bi-x-circle` - Reject
- `bi-download` - Download
- `bi-upload` - Upload

**Data Types:**
- `bi-tag` - Name/Label
- `bi-card-text` - Description
- `bi-database` - Database
- `bi-code` - Code
- `bi-123` - Numbers
- `bi-calendar-event` - Date/Time
- `bi-person` - User/Person
- `bi-people` - Users/Groups

**Business:**
- `bi-building` - Company
- `bi-briefcase` - Application
- `bi-question-circle` - Question
- `bi-bar-chart` - Chart/Report
- `bi-clipboard-data` - Survey
- `bi-file-earmark-text` - Document
- `bi-envelope` - Email
- `bi-bell` - Notification

**Status:**
- `bi-check-circle-fill` - Success
- `bi-x-circle-fill` - Error
- `bi-exclamation-triangle-fill` - Warning
- `bi-info-circle-fill` - Info
- `bi-lightning-charge` - Active/Powered

## DataTables Integration

DataTables automatically initializes on tables with class="table".

### Disable Sorting on Columns
Add `no-sort` class to `<th>`:
```html
<th class="no-sort">Actions</th>
```

### Disable Searching on Columns
Add `no-search` class to `<th>`:
```html
<th class="no-search">Internal ID</th>
```

### Custom Configuration
Add data attribute to table:
```html
<table class="table" data-datatables-config='{"pageLength": 50}'>
```

## Light/Dark Mode Compatibility

All Bootstrap 5 classes are theme-aware:

**Automatic Support:**
- `bg-primary`, `bg-secondary`, `bg-success`, `bg-danger`, etc.
- `text-white`, `text-muted`, `text-dark`, `text-light`
- `table-striped`, `table-hover`, `table-light`
- `btn-outline-*` variants
- `badge` variants
- `card` components

**Best Practices:**
- ? Use Bootstrap utility classes only
- ? Use Bootstrap color variants (primary, secondary, success, etc.)
- ? Avoid custom CSS colors
- ? Avoid inline styles
- ? Avoid hardcoded hex colors

## Responsive Design

### Mobile-First Approach
The pattern includes:
1. `table-responsive` wrapper for horizontal scrolling
2. `btn-group-sm` for compact action buttons
3. `card` layout that works on all screen sizes
4. DataTables responsive extension (automatically enabled)

### Column Priority
On mobile, DataTables hides less important columns. The actions column always remains visible.

## Accessibility

All patterns include:
- Semantic HTML
- ARIA labels via title attributes
- Keyboard navigation support (via Bootstrap)
- Screen reader friendly structure
- High contrast support (via theme)

## Examples by Entity Type

### For Lookup Tables (Lu* prefix)
Use `bg-primary` header with appropriate icon.

### For Main Tables (Applications, Surveys, etc.)
Consider `bg-success` or `bg-info` header for differentiation.

### For Settings/Config Tables
Consider `bg-warning` header with `bi-gear` icon.

### For Status Tables
Use badge colors to indicate status:
- `badge bg-success` - Active/Approved
- `badge bg-danger` - Inactive/Rejected
- `badge bg-warning` - Pending
- `badge bg-secondary` - Neutral

## Performance Considerations

1. DataTables handles large datasets efficiently
2. State saving enabled (user preferences persist)
3. Responsive extension reduces initial render
4. Deferred rendering improves performance

## Implementation Checklist

For each Index.cshtml view:

- [ ] Wrap in `card` with `border-0 shadow-sm`
- [ ] Add `card-header` with icon and title
- [ ] Add "Create New" button in header
- [ ] Wrap table in `table-responsive` div
- [ ] Add Bootstrap table classes
- [ ] Add `table-light` to `<thead>`
- [ ] Add Bootstrap Icons to column headers
- [ ] Use `btn-group` for action buttons
- [ ] Use Bootstrap Icons in action buttons
- [ ] Add `no-sort` class to Actions column
- [ ] Add `card-footer` with record count
- [ ] Remove all inline styles
- [ ] Remove all custom CSS classes
- [ ] Test light/dark mode switching
- [ ] Verify DataTables functionality

## Files Updated

Based on this template, update these files:

1. ? InquirySpark.Admin\Areas\Inquiry\Views\LuApplicationTypes\Index.cshtml
2. ? InquirySpark.Admin\Areas\Inquiry\Views\LuQuestionTypes\Index.cshtml
3. ? InquirySpark.Admin\Areas\Inquiry\Views\LuUnitOfMeasures\Index.cshtml
4. InquirySpark.Admin\Areas\Inquiry\Views\LuReviewStatus\Index.cshtml
5. InquirySpark.Admin\Areas\Inquiry\Views\LuSurveyResponseStatus\Index.cshtml
6. InquirySpark.Admin\Areas\Inquiry\Views\LuSurveyTypes\Index.cshtml
7. InquirySpark.Admin\Areas\Inquiry\Views\Roles\Index.cshtml
8. InquirySpark.Admin\Areas\Inquiry\Views\QuestionGroups\Index.cshtml
9. InquirySpark.Admin\Areas\Inquiry\Views\ImportHistories\Index.cshtml
10. ... (all other Index.cshtml files)

## Summary

This template provides:
- ? 100% Bootstrap 5 classes
- ? Bootstrap Icons throughout
- ? DataTables with sorting & paging
- ? Full light/dark mode support
- ? Mobile responsive
- ? Accessible
- ? No custom CSS
- ? No inline styles
- ? Theme-aware colors
- ? Modern card-based layout
