// Data Explorer Grid TypeScript Module
// Manages DataTables server-side mode, virtual scrolling, saved filter sets, and export triggers

export interface DataColumn {
    name: string;
    dataType: string;
}

export interface DataPage {
    totalRows: number;
    filteredRows: number;
    columns: DataColumn[];
    rows: any[][];
    summary?: Record<string, any>;
}

export interface FilterSet {
    name: string;
    filters: Record<string, any>;
}

export interface DataExplorerConfig {
    chartId: number;
    apiEndpoint: string;
    maxRows?: number;
    enableExport?: boolean;
    savedFilters?: FilterSet[];
}

export class DataExplorerGrid {
    private dataTable: any = null;
    private currentFilters: Record<string, any> = {};
    private savedFilterSets: FilterSet[] = [];
    private chartId: number;
    private apiEndpoint: string;
    private maxRows: number;

    constructor(
        private tableElement: HTMLTableElement,
        private config: DataExplorerConfig
    ) {
        this.chartId = config.chartId;
        this.apiEndpoint = config.apiEndpoint || `/api/charts/${this.chartId}/data`;
        this.maxRows = config.maxRows || 100000;
        this.savedFilterSets = config.savedFilters || [];

        this.initializeDataTable();
        this.attachFilterControls();
        this.loadSavedFilters();
    }

    /**
     * Initialize DataTables with server-side processing
     */
    private initializeDataTable(): void {
        if (!this.tableElement) {
            console.error('Table element not found');
            return;
        }

        // @ts-ignore - DataTables loaded globally via jQuery
        this.dataTable = $(this.tableElement).DataTable({
            // Server-side processing
            processing: true,
            serverSide: true,
            ajax: {
                url: this.apiEndpoint,
                type: 'GET',
                data: (d: any) => {
                    return {
                        page: Math.floor(d.start / d.length) + 1,
                        pageSize: d.length,
                        filters: JSON.stringify(this.currentFilters),
                        sort: this.buildSortParam(d.order)
                    };
                },
                dataSrc: (json: any) => {
                    // Handle BaseResponse wrapper
                    if (json.isSuccessful && json.data) {
                        const dataPage: DataPage = json.data;
                        
                        // Update summary banner if present
                        this.updateSummaryBanner(dataPage.summary);
                        
                        // Set total records for pagination
                        json.recordsTotal = dataPage.totalRows;
                        json.recordsFiltered = dataPage.filteredRows;
                        
                        return dataPage.rows;
                    } else {
                        console.error('API error:', json.errors);
                        return [];
                    }
                },
                error: (xhr: any, error: string, code: string) => {
                    console.error('DataTables AJAX error:', error, code);
                    this.showError('Failed to load data. Please try again.');
                }
            },

            // Performance optimization
            deferRender: true,
            scroller: {
                loadingIndicator: true
            },

            // Pagination
            pageLength: 100,
            lengthMenu: [[25, 50, 100, 250, 500], [25, 50, 100, 250, 500]],

            // Responsive
            responsive: true,
            autoWidth: false,

            // Styling
            dom: '<"row"<"col-sm-12 col-md-4"l><"col-sm-12 col-md-4 text-center"<"summary-banner">><"col-sm-12 col-md-4"f>>' +
                 '<"row"<"col-sm-12"tr>>' +
                 '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',

            // Language
            language: {
                processing: '<i class="bi bi-hourglass-split"></i> Loading data...',
                search: '_INPUT_',
                searchPlaceholder: 'Search data...',
                lengthMenu: 'Show _MENU_ rows',
                info: 'Showing _START_ to _END_ of _TOTAL_ rows',
                infoEmpty: 'No data available',
                infoFiltered: '(filtered from _MAX_ total rows)',
                loadingRecords: 'Loading...',
                zeroRecords: 'No matching records found',
                emptyTable: 'No data available in table',
                paginate: {
                    first: '<i class="bi bi-chevron-bar-left"></i>',
                    previous: '<i class="bi bi-chevron-left"></i>',
                    next: '<i class="bi bi-chevron-right"></i>',
                    last: '<i class="bi bi-chevron-bar-right"></i>'
                }
            },

            // State saving
            stateSave: false, // Handled via UserPreferenceService

            // Column definitions
            columnDefs: [
                { targets: 'no-sort', orderable: false },
                { targets: 'no-search', searchable: false }
            ],

            // Callbacks
            drawCallback: () => {
                this.onTableDraw();
            },

            initComplete: () => {
                console.log('Data Explorer Grid initialized');
                this.restorePreferences();
            }
        });
    }

    /**
     * Build sort parameter from DataTables order array
     */
    private buildSortParam(order: any[]): string {
        if (!order || order.length === 0) return '';
        
        return order.map((o: any) => {
            const columnIndex = o.column;
            const direction = o.dir === 'asc' ? 'asc' : 'desc';
            // Get column name from table header
            const columnName = $(this.tableElement).find('th').eq(columnIndex).data('column-name') || columnIndex;
            return `${columnName}:${direction}`;
        }).join(',');
    }

    /**
     * Attach filter controls and event handlers
     */
    private attachFilterControls(): void {
        // Filter builder button
        const filterBtn = document.getElementById('filterBuilderBtn');
        if (filterBtn) {
            filterBtn.addEventListener('click', () => {
                this.openFilterBuilder();
            });
        }

        // Clear filters button
        const clearBtn = document.getElementById('clearFiltersBtn');
        if (clearBtn) {
            clearBtn.addEventListener('click', () => {
                this.clearFilters();
            });
        }

        // Save filter set button
        const saveFilterBtn = document.getElementById('saveFilterSetBtn');
        if (saveFilterBtn) {
            saveFilterBtn.addEventListener('click', () => {
                this.saveCurrentFilterSet();
            });
        }

        // Export button
        const exportBtn = document.getElementById('exportDataBtn');
        if (exportBtn) {
            exportBtn.addEventListener('click', () => {
                this.openExportModal();
            });
        }

        // Saved filter set dropdown
        const filterSetSelect = document.getElementById('filterSetSelect') as HTMLSelectElement;
        if (filterSetSelect) {
            filterSetSelect.addEventListener('change', (e) => {
                const target = e.target as HTMLSelectElement;
                const filterSetName = target.value;
                this.loadFilterSet(filterSetName);
            });
        }
    }

    /**
     * Apply filters and reload table
     */
    public applyFilters(filters: Record<string, any>): void {
        this.currentFilters = filters;
        
        if (this.dataTable) {
            const startTime = performance.now();
            this.dataTable.ajax.reload(() => {
                const endTime = performance.now();
                const duration = endTime - startTime;
                console.log(`Filter applied in ${duration.toFixed(2)}ms`);
                
                // Show warning if over 500ms threshold
                if (duration > 500) {
                    this.showWarning(`Filter took ${duration.toFixed(0)}ms (target: <500ms)`);
                }
            });
        }
    }

    /**
     * Clear all filters
     */
    public clearFilters(): void {
        this.currentFilters = {};
        this.applyFilters({});
        
        // Reset filter builder UI
        const filterBuilder = document.getElementById('filterBuilder');
        if (filterBuilder) {
            const inputs = filterBuilder.querySelectorAll('input, select');
            inputs.forEach((input: any) => {
                if (input.type === 'checkbox' || input.type === 'radio') {
                    input.checked = false;
                } else {
                    input.value = '';
                }
            });
        }
    }

    /**
     * Save current filter set with name
     */
    private saveCurrentFilterSet(): void {
        const nameInput = prompt('Enter a name for this filter set:');
        if (!nameInput || nameInput.trim() === '') return;

        const filterSet: FilterSet = {
            name: nameInput.trim(),
            filters: { ...this.currentFilters }
        };

        // Check if name already exists
        const existingIndex = this.savedFilterSets.findIndex(fs => fs.name === filterSet.name);
        if (existingIndex >= 0) {
            if (!confirm(`Filter set "${filterSet.name}" already exists. Overwrite?`)) {
                return;
            }
            this.savedFilterSets[existingIndex] = filterSet;
        } else {
            this.savedFilterSets.push(filterSet);
        }

        // Save to user preferences via API
        this.savePreferences();

        // Update dropdown
        this.updateFilterSetDropdown();

        this.showSuccess(`Filter set "${filterSet.name}" saved successfully`);
    }

    /**
     * Load a saved filter set by name
     */
    private loadFilterSet(name: string): void {
        if (!name) {
            this.clearFilters();
            return;
        }

        const filterSet = this.savedFilterSets.find(fs => fs.name === name);
        if (filterSet) {
            this.applyFilters(filterSet.filters);
            this.showSuccess(`Loaded filter set "${name}"`);
        }
    }

    /**
     * Load saved filter sets from user preferences
     */
    private loadSavedFilters(): void {
        // Populate dropdown with saved filter sets
        this.updateFilterSetDropdown();
    }

    /**
     * Update filter set dropdown options
     */
    private updateFilterSetDropdown(): void {
        const select = document.getElementById('filterSetSelect') as HTMLSelectElement;
        if (!select) return;

        // Clear existing options except the default
        while (select.options.length > 1) {
            select.remove(1);
        }

        // Add saved filter sets
        this.savedFilterSets.forEach(fs => {
            const option = document.createElement('option');
            option.value = fs.name;
            option.textContent = fs.name;
            select.appendChild(option);
        });
    }

    /**
     * Open filter builder modal
     */
    private openFilterBuilder(): void {
        const modal = document.getElementById('filterBuilderModal');
        if (modal) {
            // @ts-ignore - Bootstrap modal
            const bootstrapModal = new bootstrap.Modal(modal);
            bootstrapModal.show();
        }
    }

    /**
     * Open export modal
     */
    private openExportModal(): void {
        const modal = document.getElementById('exportModal');
        if (modal) {
            // Update export row count
            const rowCount = this.dataTable.page.info().recordsDisplay;
            const exportCount = Math.min(rowCount, this.maxRows);
            
            const countSpan = modal.querySelector('#exportRowCount');
            if (countSpan) {
                countSpan.textContent = exportCount.toString();
            }

            // Show warning if over 50K rows
            const warningDiv = modal.querySelector('#exportWarning');
            if (warningDiv) {
                warningDiv.classList.toggle('d-none', exportCount <= 50000);
            }

            // @ts-ignore - Bootstrap modal
            const bootstrapModal = new bootstrap.Modal(modal);
            bootstrapModal.show();
        }
    }

    /**
     * Update summary banner with aggregate statistics
     */
    private updateSummaryBanner(summary?: Record<string, any>): void {
        const bannerDiv = $(this.tableElement).parent().find('.summary-banner');
        if (!bannerDiv || !summary) return;

        let html = '<div class="badge bg-info text-white">';
        html += '<i class="bi bi-info-circle"></i> ';
        
        const summaryParts: string[] = [];
        for (const [key, value] of Object.entries(summary)) {
            summaryParts.push(`${key}: ${this.formatSummaryValue(value)}`);
        }
        
        html += summaryParts.join(' | ');
        html += '</div>';

        bannerDiv.html(html);
    }

    /**
     * Format summary value for display
     */
    private formatSummaryValue(value: any): string {
        if (typeof value === 'number') {
            return value.toLocaleString();
        }
        return value.toString();
    }

    /**
     * Table draw callback
     */
    private onTableDraw(): void {
        // Add any post-draw customizations here
        // e.g., tooltips, inline charts, etc.
    }

    /**
     * Save grid preferences to user preference service
     */
    private savePreferences(): void {
        const preferences = {
            filterSets: this.savedFilterSets,
            pageLength: this.dataTable?.page.len() || 100,
            currentFilters: this.currentFilters
        };

        // Call UserPreferences API
        fetch('/api/user-preferences', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                surface: `data-explorer-${this.chartId}`,
                preferences: JSON.stringify(preferences)
            })
        }).catch(error => {
            console.error('Failed to save preferences:', error);
        });
    }

    /**
     * Restore grid preferences from user preference service
     */
    private restorePreferences(): void {
        fetch(`/api/user-preferences?surface=data-explorer-${this.chartId}`)
            .then(response => response.json())
            .then(data => {
                if (data.isSuccessful && data.data && data.data.preferences) {
                    const prefs = JSON.parse(data.data.preferences);
                    
                    // Restore filter sets
                    if (prefs.filterSets) {
                        this.savedFilterSets = prefs.filterSets;
                        this.updateFilterSetDropdown();
                    }
                    
                    // Restore page length
                    if (prefs.pageLength && this.dataTable) {
                        this.dataTable.page.len(prefs.pageLength).draw();
                    }
                    
                    // Optionally restore last filters
                    // if (prefs.currentFilters) {
                    //     this.applyFilters(prefs.currentFilters);
                    // }
                }
            })
            .catch(error => {
                console.error('Failed to restore preferences:', error);
            });
    }

    /**
     * Show error message
     */
    private showError(message: string): void {
        this.showToast(message, 'danger');
    }

    /**
     * Show warning message
     */
    private showWarning(message: string): void {
        this.showToast(message, 'warning');
    }

    /**
     * Show success message
     */
    private showSuccess(message: string): void {
        this.showToast(message, 'success');
    }

    /**
     * Show toast notification
     */
    private showToast(message: string, type: 'success' | 'warning' | 'danger' | 'info'): void {
        // Use Bootstrap toast or simple alert
        const toastContainer = document.getElementById('toastContainer');
        if (toastContainer) {
            const toast = document.createElement('div');
            toast.className = `toast align-items-center text-white bg-${type} border-0`;
            toast.setAttribute('role', 'alert');
            toast.setAttribute('aria-live', 'assertive');
            toast.setAttribute('aria-atomic', 'true');
            
            toast.innerHTML = `
                <div class="d-flex">
                    <div class="toast-body">${message}</div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            `;
            
            toastContainer.appendChild(toast);
            
            // @ts-ignore - Bootstrap toast
            const bsToast = new bootstrap.Toast(toast);
            bsToast.show();
            
            // Remove after hidden
            toast.addEventListener('hidden.bs.toast', () => {
                toast.remove();
            });
        } else {
            console.log(`[${type.toUpperCase()}] ${message}`);
        }
    }

    /**
     * Reload table data
     */
    public reload(): void {
        if (this.dataTable) {
            this.dataTable.ajax.reload();
        }
    }

    /**
     * Destroy the DataTable instance
     */
    public destroy(): void {
        if (this.dataTable) {
            this.dataTable.destroy();
            this.dataTable = null;
        }
    }
}

// Export initialization helper
export function initializeDataExplorerGrid(
    tableSelector: string,
    config: DataExplorerConfig
): DataExplorerGrid | null {
    const tableElement = document.querySelector(tableSelector) as HTMLTableElement;
    if (!tableElement) {
        console.error(`Table element not found: ${tableSelector}`);
        return null;
    }

    return new DataExplorerGrid(tableElement, config);
}
