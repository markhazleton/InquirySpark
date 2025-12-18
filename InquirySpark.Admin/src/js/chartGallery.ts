// Chart Gallery TypeScript Module
// Manages DataTables integration and chart asset browsing

export interface ChartAsset {
    chartAssetId: number;
    chartDefinitionId: number;
    displayName: string;
    description?: string;
    tags?: string;
    generationDt: string;
    approvalStatus: string;
    cdnBaseUrl?: string;
}

export class ChartGalleryManager {
    private dataTable: any = null;

    constructor(private tableElement: HTMLTableElement) {
        this.initializeDataTable();
    }

    /**
     * Initialize DataTables with custom configuration
     */
    private initializeDataTable(): void {
        if (!this.tableElement) return;

        // @ts-ignore - DataTables loaded globally via jQuery
        this.dataTable = $(this.tableElement).DataTable({
            responsive: true,
            pageLength: 25,
            order: [[3, 'desc']], // Sort by generation date descending
            columnDefs: [
                { targets: 'no-sort', orderable: false },
                { targets: [3], type: 'date' }
            ],
            language: {
                search: '_INPUT_',
                searchPlaceholder: 'Search charts...',
                lengthMenu: 'Show _MENU_ charts per page',
                info: 'Showing _START_ to _END_ of _TOTAL_ charts',
                infoEmpty: 'No charts available',
                infoFiltered: '(filtered from _MAX_ total charts)'
            },
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                 '<"row"<"col-sm-12"tr>>' +
                 '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            stateSave: true,
            stateDuration: 60 * 60 * 24 // 24 hours
        });

        this.attachCustomFilters();
    }

    /**
     * Attach custom filter controls
     */
    private attachCustomFilters(): void {
        // Tag filter
        const tagFilter = document.getElementById('tagFilter') as HTMLSelectElement;
        if (tagFilter && this.dataTable) {
            tagFilter.addEventListener('change', () => {
                const tag = tagFilter.value;
                if (tag) {
                    this.dataTable.column(2).search(tag).draw(); // Assuming tags are in column 2
                } else {
                    this.dataTable.column(2).search('').draw();
                }
            });
        }

        // Status filter
        const statusFilter = document.getElementById('statusFilter') as HTMLSelectElement;
        if (statusFilter && this.dataTable) {
            statusFilter.addEventListener('change', () => {
                const status = statusFilter.value;
                if (status) {
                    this.dataTable.column(5).search(status).draw(); // Assuming status is in column 5
                } else {
                    this.dataTable.column(5).search('').draw();
                }
            });
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
     * Clear all filters
     */
    public clearFilters(): void {
        if (this.dataTable) {
            this.dataTable.search('').columns().search('').draw();
        }
    }

    /**
     * Export table data
     */
    public exportData(format: 'csv' | 'excel' | 'pdf'): void {
        // DataTables buttons extension will handle this
        console.log(`Exporting data as ${format}`);
    }
}

// Auto-initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    const table = document.querySelector('table.datatable-export') as HTMLTableElement;
    if (table) {
        const manager = new ChartGalleryManager(table);
        (window as any).chartGalleryManager = manager;
    }
});
