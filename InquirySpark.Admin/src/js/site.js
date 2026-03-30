// Import all CSS dependencies
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.min.css';
import 'datatables.net-buttons-bs5/css/buttons.bootstrap5.min.css';
import 'datatables.net-select-bs5/css/select.bootstrap5.min.css';
import 'datatables.net-searchpanes-bs5/css/searchPanes.bootstrap5.min.css';
import '../css/site.css';

// Import JavaScript dependencies
import $ from 'jquery';
import 'bootstrap';
import 'datatables.net';
import 'datatables.net-bs5';
import 'datatables.net-buttons';
import 'datatables.net-buttons-bs5';
import 'datatables.net-responsive';
import 'datatables.net-responsive-bs5';
import 'datatables.net-select';
import 'datatables.net-select-bs5';
import 'datatables.net-searchpanes';
import 'datatables.net-searchpanes-bs5';

// Import DataTables button extensions
import 'datatables.net-buttons/js/buttons.html5.js';
import 'datatables.net-buttons/js/buttons.print.js';
import 'datatables.net-buttons/js/buttons.colVis.js';

// Make jQuery globally available
window.$ = window.jQuery = $;

// DataTables Configuration with Bootstrap 5 Best Practices
$(document).ready(function () {
    // Check if DataTables is available
    if (typeof $.fn.DataTable === 'undefined') {
        console.warn('DataTables is not loaded');
        return;
    }

    // Default DataTables configuration
    const defaultConfig = {
        // Responsive configuration
        responsive: true,

        // Bootstrap 5 styling
        dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
             '<"row"<"col-sm-12"tr>>' +
             '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',

        // Pagination options
        pageLength: 25,
        lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],

        // Language configuration
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search records...",
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            infoEmpty: "No entries to show",
            infoFiltered: "(filtered from _MAX_ total entries)",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        },

        // Performance optimization
        deferRender: true,

        // Column visibility and ordering
        columnDefs: [
            { targets: 'no-sort', orderable: false },
            { targets: 'no-search', searchable: false }
        ],

        // State saving
        stateSave: true,
        stateDuration: 60 * 60 * 24, // 24 hours

        // Auto width
        autoWidth: false
    };

    // Initialize DataTables on tables with .table class
    $('.table').each(function() {
        const $table = $(this);

        // Skip if already initialized
        if ($.fn.DataTable.isDataTable($table)) {
            return;
        }

        // Check if table has data-datatable="false" attribute
        if ($table.data('datatable') === false) {
            return;
        }

        // Merge custom config from data attributes
        const customConfig = $table.data('datatables-config') || {};
        const config = $.extend(true, {}, defaultConfig, customConfig);

        // Initialize DataTable
        try {
            $table.DataTable(config);
        } catch (error) {
            console.error('Error initializing DataTable:', error);
        }
    });

    // Advanced DataTables with Buttons (Export functionality)
    // Usage: Add class "datatable-export" to enable export buttons
    $('.datatable-export').each(function() {
        const $table = $(this);

        if ($.fn.DataTable.isDataTable($table)) {
            return;
        }

        const exportConfig = $.extend(true, {}, defaultConfig, {
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                 '<"row"<"col-sm-12"B>>' +
                 '<"row"<"col-sm-12"tr>>' +
                 '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            buttons: [
                {
                    extend: 'copy',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-clipboard"></i> Copy'
                },
                {
                    extend: 'csv',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-filetype-csv"></i> CSV'
                },
                {
                    extend: 'excel',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-file-earmark-excel"></i> Excel'
                },
                {
                    extend: 'pdf',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-file-earmark-pdf"></i> PDF'
                },
                {
                    extend: 'print',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-printer"></i> Print'
                }
            ]
        });

        try {
            $table.DataTable(exportConfig);
        } catch (error) {
            console.error('Error initializing DataTable with export:', error);
        }
    });

    // Data Explorer DataTables Configuration
    // Usage: Add class "datatable-export-data-explorer" for data explorer grids
    // Includes custom page lengths, summary banner support, and JSZip/PDFMake bundles
    $('.datatable-export-data-explorer').each(function() {
        const $table = $(this);

        if ($.fn.DataTable.isDataTable($table)) {
            return;
        }

        const dataExplorerConfig = $.extend(true, {}, defaultConfig, {
            // Custom DOM layout with summary banner placeholder
            dom: '<"row"<"col-sm-12 col-md-4"l><"col-sm-12 col-md-4 text-center"<"summary-banner">><"col-sm-12 col-md-4"f>>' +
                 '<"row"<"col-sm-12"B>>' +
                 '<"row"<"col-sm-12"tr>>' +
                 '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',

            // Extended page length options for large datasets
            pageLength: 100,
            lengthMenu: [[25, 50, 100, 250, 500], [25, 50, 100, 250, 500]],

            // Server-side processing (configured in grid.ts)
            processing: true,
            serverSide: true,

            // Disable state saving (handled by UserPreferenceService)
            stateSave: false,

            // Export buttons with enhanced options
            buttons: [
                {
                    extend: 'copy',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-clipboard"></i> Copy',
                    exportOptions: {
                        modifier: {
                            page: 'current'
                        }
                    }
                },
                {
                    extend: 'csv',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-filetype-csv"></i> CSV',
                    exportOptions: {
                        modifier: {
                            page: 'all',
                            search: 'applied'
                        }
                    },
                    title: function() {
                        const chartName = $table.data('chart-name') || 'data-export';
                        const timestamp = new Date().toISOString().slice(0, 10);
                        return `${chartName}-${timestamp}`;
                    }
                },
                {
                    extend: 'excel',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-file-earmark-excel"></i> Excel',
                    exportOptions: {
                        modifier: {
                            page: 'all',
                            search: 'applied'
                        }
                    },
                    title: function() {
                        const chartName = $table.data('chart-name') || 'data-export';
                        const timestamp = new Date().toISOString().slice(0, 10);
                        return `${chartName}-${timestamp}`;
                    },
                    customize: function(xlsx) {
                        // Add custom styling to Excel export
                        const sheet = xlsx.xl.worksheets['sheet1.xml'];
                        
                        // Add metadata header
                        $('row:first c', sheet).each(function() {
                            $(this).attr('s', '2'); // Bold style
                        });

                        // Add watermark/metadata row
                        const exportDate = new Date().toLocaleString();
                        const filters = $table.data('current-filters');
                        const filterText = filters ? `Filters: ${JSON.stringify(filters)}` : 'No filters applied';
                        
                        const metadataRow = `<row>
                            <c t="inlineStr"><is><t>Exported: ${exportDate}</t></is></c>
                            <c t="inlineStr"><is><t>${filterText}</t></is></c>
                        </row>`;
                        
                        $('sheetData', sheet).prepend(metadataRow);
                    }
                },
                {
                    extend: 'pdf',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-file-earmark-pdf"></i> PDF',
                    exportOptions: {
                        modifier: {
                            page: 'current' // PDF limited to current page for performance
                        }
                    },
                    title: function() {
                        const chartName = $table.data('chart-name') || 'data-export';
                        const timestamp = new Date().toISOString().slice(0, 10);
                        return `${chartName}-${timestamp}`;
                    },
                    customize: function(doc) {
                        // Add export metadata to PDF footer
                        doc.content.splice(0, 0, {
                            text: [
                                { text: 'InquirySpark Data Explorer Export\n', fontSize: 16, bold: true },
                                { text: `Exported: ${new Date().toLocaleString()}\n`, fontSize: 10 },
                                { text: 'Read-Only Data View - Do Not Modify Source Data\n\n', fontSize: 8, italics: true, color: '#999' }
                            ],
                            alignment: 'center',
                            margin: [0, 0, 0, 12]
                        });

                        // Watermark for read-only reminder
                        doc.watermark = { 
                            text: 'READ ONLY', 
                            color: 'red', 
                            opacity: 0.1, 
                            fontSize: 40 
                        };
                    }
                },
                {
                    extend: 'print',
                    className: 'btn btn-sm btn-secondary',
                    text: '<i class="bi bi-printer"></i> Print',
                    exportOptions: {
                        modifier: {
                            page: 'current'
                        }
                    },
                    customize: function(win) {
                        // Add custom print styles
                        $(win.document.body)
                            .css('font-size', '10pt')
                            .prepend(
                                '<div style="text-align:center;margin-bottom:20px;">' +
                                '<h3>InquirySpark Data Explorer</h3>' +
                                '<p>Exported: ' + new Date().toLocaleString() + '</p>' +
                                '<p style="color:#999;font-style:italic;">Read-Only Data View</p>' +
                                '</div>'
                            );
                        
                        $(win.document.body).find('table')
                            .addClass('compact')
                            .css('font-size', 'inherit');
                    }
                }
            ],

            // Custom language for data explorer context
            language: {
                processing: '<i class="bi bi-hourglass-split"></i> Loading data...',
                search: '_INPUT_',
                searchPlaceholder: 'Search rows...',
                lengthMenu: 'Show _MENU_ rows',
                info: 'Showing _START_ to _END_ of _TOTAL_ rows',
                infoEmpty: 'No data available',
                infoFiltered: '(filtered from _MAX_ total rows)',
                loadingRecords: 'Loading...',
                zeroRecords: 'No matching records found',
                emptyTable: 'No data available',
                paginate: {
                    first: '<i class="bi bi-chevron-bar-left"></i>',
                    previous: '<i class="bi bi-chevron-left"></i>',
                    next: '<i class="bi bi-chevron-right"></i>',
                    last: '<i class="bi bi-chevron-bar-right"></i>'
                }
            }
        });

        // Note: DataExplorerGrid class in grid.ts handles actual initialization
        // This config is available for fallback or simpler tables
        try {
            $table.DataTable(dataExplorerConfig);
        } catch (error) {
            console.error('Error initializing Data Explorer DataTable:', error);
        }
    });
});

// Global DataTables utility functions
window.DataTableUtils = {
    // Reload table data
    reload: function(tableSelector) {
        const table = $(tableSelector).DataTable();
        if (table) {
            table.ajax.reload();
        }
    },

    // Clear search and filters
    clearFilters: function(tableSelector) {
        const table = $(tableSelector).DataTable();
        if (table) {
            table.search('').columns().search('').draw();
        }
    },

    // Get selected rows
    getSelectedRows: function(tableSelector) {
        const table = $(tableSelector).DataTable();
        return table ? table.rows({ selected: true }).data().toArray() : [];
    }
};
