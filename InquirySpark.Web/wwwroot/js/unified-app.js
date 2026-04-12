/**
 * unified-app.js — InquirySpark.Web unified bootstrap module (T015)
 * Loaded on every page of the Unified area.
 * Per constitution: no inline styles; relies on Bootstrap utility classes.
 */

(function () {
    'use strict';

    // ── Cross-domain navigation awareness ───────────────────────────────────
    // Highlight the active nav link matching the current path segment.
    function setActiveNavLink() {
        var path = window.location.pathname.toLowerCase();
        document.querySelectorAll('[data-unified-nav]').forEach(function (el) {
            var href = (el.getAttribute('href') || '').toLowerCase();
            if (href && path.startsWith(href)) {
                el.classList.add('active');
                el.setAttribute('aria-current', 'page');
            }
        });
    }

    // ── DataTables export button activation ─────────────────────────────────
    // Tables with .datatable-export get the Buttons extension.
    function initExportTables() {
        if (typeof $.fn.DataTable === 'undefined') return;
        document.querySelectorAll('table.datatable-export').forEach(function (table) {
            if (!$.fn.DataTable.isDataTable(table)) {
                new $.fn.dataTable.Api(table).buttons().container().appendTo(
                    table.closest('.card-header') || table.parentNode
                );
            }
        });
    }

    // ── Bootstrap tooltip init ───────────────────────────────────────────────
    function initTooltips() {
        if (typeof bootstrap === 'undefined' || !bootstrap.Tooltip) return;
        document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
            new bootstrap.Tooltip(el);
        });
    }

    // ── Init ─────────────────────────────────────────────────────────────────
    document.addEventListener('DOMContentLoaded', function () {
        setActiveNavLink();
        initTooltips();
        initExportTables();
    });
}());
