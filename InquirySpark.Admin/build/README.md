# Build Process Documentation

## Overview

This project uses **Webpack 5** to bundle all client-side dependencies into optimized `site.js` and `site.css` files. The build system consolidates Bootstrap 5, Bootstrap Icons, DataTables.net, jQuery, and all related libraries into a single, production-ready bundle.

## Project Structure

```
InquirySpark.Admin/
├── build/                          # Build scripts and configurations
│   ├── webpack.common.js          # Common webpack configuration
│   ├── webpack.dev.js             # Development build configuration
│   ├── webpack.prod.js            # Production build configuration
│   └── README.md                  # This file
├── src/                           # Source files
│   ├── js/
│   │   └── site.js               # Main JavaScript entry point
│   └── css/
│       └── site.css              # Custom site styles
├── wwwroot/                       # Output directory (generated)
│   ├── js/
│   │   ├── site.min.js           # Bundled & minified JavaScript
│   │   └── vfs_fonts.js          # PDFMake fonts (separate file)
│   ├── css/
│   │   └── site.min.css          # Bundled & minified CSS
│   └── fonts/                     # Bootstrap Icons fonts
│       ├── bootstrap-icons.woff
│       └── bootstrap-icons.woff2
├── scripts/                       # Legacy build scripts
│   └── copy-assets.js            # Legacy copy script (kept for reference)
└── package.json                   # NPM configuration
```

## Build Commands

### Production Build (Recommended)
```bash
npm run build
# or explicitly:
npm run build:prod
```

This creates minified, optimized bundles:
- `wwwroot/js/site.min.js` - All JavaScript (jQuery, Bootstrap, DataTables, etc.)
- `wwwroot/css/site.min.css` - All CSS (Bootstrap, Bootstrap Icons, DataTables)
- `wwwroot/fonts/*` - Bootstrap Icons font files
- `wwwroot/js/vfs_fonts.js` - PDFMake virtual fonts

### Development Build
```bash
npm run build:dev
```

Creates unminified bundles with inline source maps for debugging.

### Watch Mode (Development)
```bash
npm run watch
```

Automatically rebuilds when source files change.

### Clean Build Artifacts
```bash
npm run clean
```

Removes generated JavaScript and CSS bundles.

## What's Included

### JavaScript Libraries
- **jQuery 3.7.1** - Required by DataTables and validation
- **Bootstrap 5.3.8** - UI framework
- **DataTables.net 2.3.5** - Table enhancement
  - Bootstrap 5 integration
  - Buttons (export to CSV, Excel, PDF)
  - Responsive tables
  - Search panes
  - Row selection
- **JSZip 3.10.1** - Excel export support
- **PDFMake 0.2.20** - PDF export support

### CSS Libraries
- **Bootstrap 5.3.8** - Core styles
- **Bootstrap Icons 1.13.1** - Icon font
- **DataTables.net** - Table styles (all extensions)

### Custom Code
- DataTables initialization and configuration
- Global utility functions (`window.DataTableUtils`)
- Custom site styles

## How It Works

1. **Entry Point**: `src/js/site.js` imports all dependencies and custom code
2. **CSS Extraction**: Webpack extracts all CSS into separate `.css` files
3. **Font Handling**: Bootstrap Icons fonts are automatically copied to `wwwroot/fonts/`
4. **Minification**:
   - JavaScript: Terser (removes comments, compresses code)
   - CSS: cssnano (removes comments, optimizes selectors)
5. **Source Maps**: Generated for both dev and production builds

## Layout Integration

The bundles are referenced in `_Layout.cshtml`:

```cshtml
<!-- In <head> -->
<link rel="stylesheet" href="~/css/site.min.css" asp-append-version="true" />

<!-- Before </body> -->
<script src="~/js/vfs_fonts.js" asp-append-version="true"></script>
<script src="~/js/site.min.js" asp-append-version="true"></script>
```

## Adding New Dependencies

1. Install the package:
   ```bash
   npm install package-name
   ```

2. Import in `src/js/site.js`:
   ```javascript
   import 'package-name';
   import 'package-name/dist/styles.css'; // If it has CSS
   ```

3. Rebuild:
   ```bash
   npm run build
   ```

## Adding Custom Code

### JavaScript
Add your code to `src/js/site.js` or create new modules and import them.

### CSS
Add styles to `src/css/site.css` - they will be automatically included in the bundle.

## Performance Considerations

- **Bundle Size**: The production bundle is ~420KB JS + ~358KB CSS (gzipped will be smaller)
- **Caching**: Use `asp-append-version="true"` for cache-busting
- **Loading**: Bundle loads synchronously - all libraries available immediately
- **Icons**: Bootstrap Icons fonts load separately (not inlined in CSS)

## Webpack Configuration Details

### Common Configuration (`webpack.common.js`)
- Entry point: `src/js/site.js`
- Output: `wwwroot/js/[name].js` and `wwwroot/css/[name].css`
- CSS extraction with MiniCssExtractPlugin
- Font file handling (Bootstrap Icons)
- PDFMake vfs_fonts.js copying
- No code splitting (single bundle for simplicity)

### Development Configuration (`webpack.dev.js`)
- Mode: development
- Source maps: inline-source-map
- No minification

### Production Configuration (`webpack.prod.js`)
- Mode: production
- Source maps: separate .map files
- Minification: Terser + CSSMinimizer
- Filename: `[name].min.js` and `[name].min.css`

## Legacy Build System

The old copy-based build system is preserved under `legacy:*` scripts:

```bash
npm run legacy:build
```

This can be removed once the webpack build is fully validated.

## Troubleshooting

### Build Fails
1. Delete `node_modules` and `package-lock.json`
2. Run `npm install`
3. Run `npm run build`

### Missing Fonts/Icons
- Bootstrap Icons fonts should be in `wwwroot/fonts/`
- Check the browser console for 404 errors
- Verify the CSS is loading correctly

### DataTables Not Initializing
- Check browser console for JavaScript errors
- Verify jQuery is loading before DataTables
- Ensure `site.min.js` is loaded

### Large Bundle Size Warnings
- These are expected for a comprehensive bundle
- Gzip compression on the server will reduce transfer size significantly
- Consider code splitting if the bundle grows much larger

## Future Enhancements

- Add TypeScript support
- Implement code splitting for better performance
- Add bundle analysis tools
- Create separate vendor and app bundles
- Implement lazy loading for DataTables export features
