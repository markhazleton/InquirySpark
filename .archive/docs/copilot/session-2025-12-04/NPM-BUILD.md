# InquirySpark.Admin - NPM Build Process

This project uses npm to manage client-side dependencies instead of LibMan or CDN services.

## Overview

All client-side libraries are managed through npm and automatically copied to `wwwroot/lib` during the build process.

## Prerequisites

- Node.js >= 18.0.0
- npm >= 9.0.0

## Client Libraries Included

### Core Libraries
- **jQuery** v3.7.1 - JavaScript library required for DataTables
- **Bootstrap** v5.3.3 - JavaScript bundle (JS only, CSS from Bootswatch themes)
- **Bootstrap Icons** v1.11.3 - Official Bootstrap icon library
- **jQuery Validation** v1.21.0 - Form validation plugin
- **jQuery Validation Unobtrusive** v4.0.0 - Unobtrusive validation for ASP.NET Core

### DataTables v2.1.8
Powerful table plugin for jQuery with Bootstrap 5 integration:

#### Core Components
- **datatables.net** - Core DataTables library
- **datatables.net-bs5** - Bootstrap 5 styling integration

#### Extensions
- **datatables.net-buttons** - Export and column visibility buttons
  - Copy to clipboard
  - CSV export
  - Excel export
  - PDF export
  - Print view
  - Column visibility toggle
- **datatables.net-responsive** - Responsive table behavior for mobile devices
- **datatables.net-searchpanes** - Advanced filtering with search panes
- **datatables.net-select** - Row and cell selection

#### Export Dependencies
- **jszip** v3.10.1 - Required for Excel export
- **pdfmake** v0.2.14 - Required for PDF export

All extensions include Bootstrap 5 styling variants for seamless theme integration.

## NPM Scripts

### Install Dependencies
```bash
npm install
```
This will install all dependencies and automatically run the build process.

### Build Assets
```bash
npm run build
```
Copies all required library files from `node_modules` to `wwwroot/lib`.

### Clean Assets
```bash
npm run clean
```
Removes the `wwwroot/lib` directory.

### Individual Library Tasks
```bash
npm run copy:jquery           # Copy jQuery files
npm run copy:datatables       # Copy DataTables files
npm run copy:bootstrap-icons  # Copy Bootstrap Icons
npm run copy:validation       # Copy jQuery Validation files
```

## MSBuild Integration

The npm build process is integrated with MSBuild:

- **Before Build**: If `node_modules` doesn't exist, `npm install` runs automatically
- **Before Build**: If `node_modules` exists, `npm run build` runs automatically
- **Before Clean**: The `wwwroot/lib` directory is cleaned

This means you can build the project using:
```bash
dotnet build
```
And npm will handle the client libraries automatically.

## File Structure

```
InquirySpark.Admin/
??? package.json              # npm configuration and dependencies
??? scripts/
?   ??? copy-assets.js        # Custom script to copy library files
??? node_modules/             # npm packages (git ignored)
??? wwwroot/
    ??? lib/                  # Built client libraries (git ignored)
        ??? jquery/
        ?   ??? jquery.js
        ?   ??? jquery.min.js
        ?   ??? jquery.min.map
        ??? bootstrap/
        ?   ??? js/           # JavaScript only (CSS from Bootswatch)
        ?       ??? bootstrap.bundle.js
        ?       ??? bootstrap.bundle.min.js
        ?       ??? bootstrap.bundle.min.js.map
        ??? datatables/
        ?   ??? css/          # All DataTables CSS files
        ?   ?   ??? dataTables.bootstrap5.min.css
        ?   ?   ??? buttons.bootstrap5.min.css
        ?   ?   ??? responsive.bootstrap5.min.css
        ?   ?   ??? select.bootstrap5.min.css
        ?   ?   ??? searchPanes.bootstrap5.min.css
        ?   ??? js/           # All DataTables JavaScript files
        ?       ??? dataTables.min.js
        ?       ??? dataTables.bootstrap5.min.js
        ?       ??? dataTables.buttons.min.js
        ?       ??? buttons.*.min.js (various button types)
        ?       ??? dataTables.responsive.min.js
        ?       ??? dataTables.select.min.js
        ?       ??? dataTables.searchPanes.min.js
        ??? jszip/            # For Excel export
        ?   ??? jszip.min.js
        ??? pdfmake/          # For PDF export
        ?   ??? pdfmake.min.js
        ?   ??? vfs_fonts.js
        ??? bootstrap-icons/
        ?   ??? font/
        ?       ??? bootstrap-icons.css
        ?       ??? bootstrap-icons.min.css
        ?       ??? fonts/ (all font files)
        ??? jquery-validation/
        ?   ??? dist/
        ?       ??? jquery.validate.js
        ?       ??? jquery.validate.min.js
        ??? jquery-validation-unobtrusive/
            ??? jquery.validate.unobtrusive.js
            ??? jquery.validate.unobtrusive.min.js
```

## Advantages Over LibMan/CDN

1. **Offline Development**: No internet required after initial `npm install`
2. **Version Control**: Exact versions locked in `package-lock.json`
3. **Build Reproducibility**: Same versions on all development machines
4. **No CDN Dependencies**: Zero external service dependencies at runtime
5. **Better Performance**: Assets served from same domain (no DNS lookups)
6. **Cache Control**: Full control over asset versioning with `asp-append-version`
7. **Security**: No reliance on third-party CDN availability or integrity
8. **Theme Compatibility**: Bootstrap JavaScript only (CSS from Bootswatch themes)

## Development Workflow

1. **First Time Setup**:
   ```bash
   cd InquirySpark.Admin
   npm install
   ```

2. **Adding a New Library**:
   ```bash
   npm install <package-name> --save
   ```
   Then update `scripts/copy-assets.js` to include the new library.

3. **Updating Libraries**:
   ```bash
   npm update <package-name>
   npm run build
   ```

4. **Building the Project**:
   ```bash
   dotnet build
   ```
   npm build runs automatically as part of MSBuild.

## Troubleshooting

### Libraries Not Copying
If libraries aren't copied after build:
```bash
npm run clean
npm run build
```

### Node Modules Missing
If you get errors about missing modules:
```bash
npm install
```

### Build Fails
Ensure Node.js and npm are installed and in PATH:
```bash
node --version
npm --version
```

## Notes

- The `wwwroot/lib` directory is git-ignored as it's generated during build
- The `node_modules` directory is git-ignored
- **Bootstrap CSS is NOT included** - provided dynamically by WebSpark.Bootswatch theme system
- Only Bootstrap JavaScript bundle is included for component functionality (dropdowns, modals, etc.)
- All assets are self-hosted for better performance and reliability
- Zero CDN dependencies for production deployment

### Bootstrap + Bootswatch Integration

This project uses a hybrid approach for Bootstrap:
- **JavaScript**: Loaded from local npm package (`~/lib/bootstrap/js/`)
- **CSS**: Dynamically loaded from WebSpark.Bootswatch theme system

This allows:
- ? Full Bootstrap JavaScript functionality (dropdowns, modals, tooltips, etc.)
- ? Dynamic theme switching without reloading the page
- ? 28+ Bootswatch themes with light/dark mode support
- ? No CDN dependencies
- ? Complete offline capability

## Using DataTables in Your Application

### Basic Usage

Simply add the `table` class to your HTML table:

```html
<table class="table table-striped table-hover">
    <thead>
        <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        <!-- Your data rows -->
    </tbody>
</table>
```

The table will automatically be enhanced with DataTables features:
- Sorting
- Searching
- Pagination
- Responsive design
- State saving

### Disable DataTables on Specific Tables

To prevent DataTables initialization on a specific table:

```html
<table class="table" data-datatable="false">
    <!-- This table won't be enhanced -->
</table>
```

### Enable Export Buttons

Add the `datatable-export` class to enable export functionality:

```html
<table class="table datatable-export">
    <!-- This table will have Copy, CSV, Excel, PDF, and Print buttons -->
</table>
```

### Custom Configuration

Pass custom DataTables configuration via data attribute:

```html
<table class="table" data-datatables-config='{"pageLength": 50, "order": [[0, "desc"]]}'>
    <!-- Custom configuration -->
</table>
```

### Disable Sorting/Searching on Columns

Use CSS classes on `<th>` elements:

```html
<thead>
    <tr>
        <th>Sortable Column</th>
        <th class="no-sort">Actions (not sortable)</th>
        <th class="no-search">Hidden Data (not searchable)</th>
    </tr>
</thead>
```

### JavaScript API

The `site.js` file provides utility functions:

```javascript
// Reload table data (for AJAX tables)
DataTableUtils.reload('#myTable');

// Clear all filters
DataTableUtils.clearFilters('#myTable');

// Get selected rows (requires select extension)
const selectedData = DataTableUtils.getSelectedRows('#myTable');
```

### Responsive Design

DataTables automatically adapts to mobile screens:
- Columns are hidden based on priority
- Hidden columns can be expanded via a toggle button
- Touch-friendly controls

### Best Practices

1. **Keep tables semantic**: Use proper `<thead>`, `<tbody>`, and `<tfoot>` elements
2. **Use Bootstrap classes**: Combine with `.table-striped`, `.table-hover`, etc.
3. **Optimize data**: For large datasets, consider server-side processing
4. **State saving**: Enabled by default (saves sort, search, and pagination for 24 hours)
5. **Accessibility**: DataTables includes ARIA attributes automatically

### Advanced Features

For more advanced usage, refer to the official DataTables documentation:
- [DataTables Documentation](https://datatables.net/)
- [Bootstrap 5 Integration](https://datatables.net/examples/styling/bootstrap5.html)
- [Extensions Reference](https://datatables.net/extensions/)
- [API Reference](https://datatables.net/reference/api/)
