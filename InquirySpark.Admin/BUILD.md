# Quick Build Guide

## Modern Webpack Build System

This project uses Webpack 5 to bundle all client-side assets into optimized `site.js` and `site.css` files.

## Quick Start

```bash
# Production build (recommended)
npm run build

# Development build with source maps
npm run build:dev

# Watch mode for development
npm run watch

# Clean generated files
npm run clean
```

## What Gets Built

### Input (Source)
- `src/js/site.js` - Main JavaScript entry point
- `src/css/site.css` - Custom styles

### Output (Generated)
- `wwwroot/js/site.min.js` - All JavaScript bundled & minified (420KB)
- `wwwroot/css/site.min.css` - All CSS bundled & minified (358KB)
- `wwwroot/fonts/*` - Bootstrap Icons fonts
- `wwwroot/js/vfs_fonts.js` - PDFMake fonts (separate file)

## Included Libraries

### JavaScript
- jQuery 3.7.1
- Bootstrap 5.3.8
- DataTables.net 2.3.5 (with all extensions)
- JSZip 3.10.1 (Excel export)
- PDFMake 0.2.20 (PDF export)

### CSS
- Bootstrap 5.3.8
- Bootstrap Icons 1.13.1
- DataTables.net (all extensions)

## Project Structure

```
build/              # Webpack configuration files
src/js/            # Source JavaScript
src/css/           # Source CSS
wwwroot/           # Output directory (generated)
  ├── js/          # Bundled JavaScript
  ├── css/         # Bundled CSS
  └── fonts/       # Icon fonts
```

## Layout Integration

The bundles are automatically referenced in `_Layout.cshtml`:

```cshtml
<!-- CSS in <head> -->
<link rel="stylesheet" href="~/css/site.min.css" asp-append-version="true" />

<!-- JS before </body> -->
<script src="~/js/vfs_fonts.js" asp-append-version="true"></script>
<script src="~/js/site.min.js" asp-append-version="true"></script>
```

## Adding Dependencies

1. Install: `npm install package-name`
2. Import in `src/js/site.js`:
   ```javascript
   import 'package-name';
   import 'package-name/dist/styles.css'; // If needed
   ```
3. Rebuild: `npm run build`

## Development Workflow

1. Make changes to `src/js/site.js` or `src/css/site.css`
2. Run `npm run watch` for automatic rebuilds
3. Refresh browser to see changes
4. Run `npm run build` before committing

## Build Configuration

See [build/README.md](build/README.md) for detailed webpack configuration documentation.

## Legacy Build System

The old copy-based build is preserved as `npm run legacy:build` and will be removed in the future.

## Troubleshooting

**Build fails?**
```bash
rm -rf node_modules package-lock.json
npm install
npm run build
```

**Missing icons?** Check that `wwwroot/fonts/bootstrap-icons.woff*` exists.

**DataTables not working?** Check browser console for errors.

For more help, see [build/README.md](build/README.md).
