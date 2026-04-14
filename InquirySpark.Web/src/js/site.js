/**
 * InquirySpark.Web — site.js
 * Unified experience bootstrap: imports and initializes shared UI dependencies.
 * Per constitution: no CDN usage; all packages sourced via npm.
 */

// Bootstrap 5
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'bootstrap/dist/css/bootstrap.min.css';

// DataTables with Bootstrap 5 integration
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.min.css';
import 'datatables.net-buttons-bs5/css/buttons.bootstrap5.min.css';
import 'datatables.net-select-bs5/css/select.bootstrap5.min.css';

// Bootstrap Icons
import 'bootstrap-icons/font/bootstrap-icons.css';

// DataTables JS
import DataTable from 'datatables.net-bs5';
import 'datatables.net-responsive-bs5';
import 'datatables.net-buttons-bs5';
import 'datatables.net-select-bs5';

// jQuery (required by DataTables)
import $ from 'jquery';

// jQuery Validation
import 'jquery-validation';
import 'jquery-validation-unobtrusive';

// Auto-initialize DataTables on .table elements (per constitution)
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('table.table:not([data-datatable="false"])').forEach((table) => {
        if (!$.fn.DataTable.isDataTable(table)) {
            new DataTable(table, {
                stateSave: true,
                responsive: true,
                columnDefs: [
                    { orderable: false, targets: '.no-sort' }
                ]
            });
        }
    });
});

window.$ = $;
window.jQuery = $;
