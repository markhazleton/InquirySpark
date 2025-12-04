// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

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