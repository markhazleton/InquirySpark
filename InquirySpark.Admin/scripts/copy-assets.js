const fs = require('fs');
const path = require('path');

// Ensure wwwroot/lib directory exists
const libDir = path.join(__dirname, '..', 'wwwroot', 'lib');
if (!fs.existsSync(libDir)) {
    fs.mkdirSync(libDir, { recursive: true });
}

// Get the library to copy from command line argument
const library = process.argv[2];

// Define copy configurations
const copyConfigs = {
    'jquery': {
        source: path.join(__dirname, '..', 'node_modules', 'jquery', 'dist'),
        dest: path.join(libDir, 'jquery'),
        files: ['jquery.js', 'jquery.min.js', 'jquery.min.map']
    },
    'bootstrap': {
        source: path.join(__dirname, '..', 'node_modules', 'bootstrap', 'dist', 'js'),
        dest: path.join(libDir, 'bootstrap', 'js'),
        files: ['bootstrap.bundle.js', 'bootstrap.bundle.min.js', 'bootstrap.bundle.min.js.map']
    },
    'bootstrap-css': {
        source: path.join(__dirname, '..', 'node_modules', 'bootstrap', 'dist', 'css'),
        dest: path.join(libDir, 'bootstrap', 'css'),
        files: ['bootstrap.min.css', 'bootstrap.min.css.map']
    },
    'datatables': [
        // Core DataTables
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['dataTables.min.js', 'dataTables.js', 'dataTables.min.mjs', 'dataTables.mjs']
        },
        // DataTables Bootstrap 5 Integration
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-bs5', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['dataTables.bootstrap5.min.js', 'dataTables.bootstrap5.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-bs5', 'css'),
            dest: path.join(libDir, 'datatables', 'css'),
            files: ['dataTables.bootstrap5.min.css', 'dataTables.bootstrap5.css']
        },
        // DataTables Buttons
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-buttons', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['dataTables.buttons.min.js', 'buttons.colVis.min.js', 'buttons.html5.min.js', 'buttons.print.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-buttons-bs5', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['buttons.bootstrap5.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-buttons-bs5', 'css'),
            dest: path.join(libDir, 'datatables', 'css'),
            files: ['buttons.bootstrap5.min.css']
        },
        // DataTables Responsive
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-responsive', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['dataTables.responsive.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-responsive-bs5', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['responsive.bootstrap5.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-responsive-bs5', 'css'),
            dest: path.join(libDir, 'datatables', 'css'),
            files: ['responsive.bootstrap5.min.css']
        },
        // DataTables SearchPanes
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-searchpanes', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['dataTables.searchPanes.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-searchpanes-bs5', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['searchPanes.bootstrap5.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-searchpanes-bs5', 'css'),
            dest: path.join(libDir, 'datatables', 'css'),
            files: ['searchPanes.bootstrap5.min.css']
        },
        // DataTables Select
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-select', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['dataTables.select.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-select-bs5', 'js'),
            dest: path.join(libDir, 'datatables', 'js'),
            files: ['select.bootstrap5.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'datatables.net-select-bs5', 'css'),
            dest: path.join(libDir, 'datatables', 'css'),
            files: ['select.bootstrap5.min.css']
        },
        // JSZip for Excel export
        {
            source: path.join(__dirname, '..', 'node_modules', 'jszip', 'dist'),
            dest: path.join(libDir, 'jszip'),
            files: ['jszip.min.js']
        },
        // PDFMake for PDF export
        {
            source: path.join(__dirname, '..', 'node_modules', 'pdfmake', 'build'),
            dest: path.join(libDir, 'pdfmake'),
            files: ['pdfmake.min.js', 'vfs_fonts.js']
        }
    ],
    'bootstrap-icons': {
        source: path.join(__dirname, '..', 'node_modules', 'bootstrap-icons', 'font'),
        dest: path.join(libDir, 'bootstrap-icons', 'font'),
        copyAll: true
    },
    'validation': [
        {
            source: path.join(__dirname, '..', 'node_modules', 'jquery-validation', 'dist'),
            dest: path.join(libDir, 'jquery-validation', 'dist'),
            files: ['jquery.validate.js', 'jquery.validate.min.js']
        },
        {
            source: path.join(__dirname, '..', 'node_modules', 'jquery-validation-unobtrusive', 'dist'),
            dest: path.join(libDir, 'jquery-validation-unobtrusive'),
            files: ['jquery.validate.unobtrusive.js', 'jquery.validate.unobtrusive.min.js']
        }
    ]
};

// Helper function to copy files
function copyFiles(sourceDir, destDir, files, rename = {}) {
    if (!fs.existsSync(destDir)) {
        fs.mkdirSync(destDir, { recursive: true });
    }

    if (files && files.length > 0) {
        files.forEach(file => {
            const sourcePath = path.join(sourceDir, file);
            const targetFileName = rename[file] || file;
            const destPath = path.join(destDir, targetFileName);
            
            if (fs.existsSync(sourcePath)) {
                fs.copyFileSync(sourcePath, destPath);
                console.log(`Copied: ${file} -> ${destDir}/${targetFileName}`);
            } else {
                console.warn(`Warning: File not found: ${sourcePath}`);
            }
        });
    }
}

// Helper function to copy entire directory
function copyDirectory(sourceDir, destDir) {
    if (!fs.existsSync(destDir)) {
        fs.mkdirSync(destDir, { recursive: true });
    }

    const files = fs.readdirSync(sourceDir);
    files.forEach(file => {
        const sourcePath = path.join(sourceDir, file);
        const destPath = path.join(destDir, file);
        
        if (fs.statSync(sourcePath).isDirectory()) {
            copyDirectory(sourcePath, destPath);
        } else {
            fs.copyFileSync(sourcePath, destPath);
        }
    });
    console.log(`Copied directory: ${sourceDir} -> ${destDir}`);
}

// Process the requested library
if (!library) {
    console.error('Error: No library specified');
    process.exit(1);
}

const config = copyConfigs[library];

if (!config) {
    console.error(`Error: Unknown library: ${library}`);
    process.exit(1);
}

console.log(`\nCopying ${library}...`);

if (Array.isArray(config)) {
    // Handle multiple copy operations
    config.forEach(cfg => {
        if (cfg.copyAll) {
            copyDirectory(cfg.source, cfg.dest);
        } else {
            copyFiles(cfg.source, cfg.dest, cfg.files, cfg.rename);
        }
    });
} else {
    // Handle single copy operation
    if (config.copyAll) {
        copyDirectory(config.source, config.dest);
    } else {
        copyFiles(config.source, config.dest, config.files, config.rename);
    }
}

console.log(`? ${library} copied successfully\n`);
