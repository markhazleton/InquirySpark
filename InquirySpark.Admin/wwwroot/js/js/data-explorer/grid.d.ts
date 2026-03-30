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
export declare class DataExplorerGrid {
    private tableElement;
    private config;
    private dataTable;
    private currentFilters;
    private savedFilterSets;
    private chartId;
    private apiEndpoint;
    private maxRows;
    constructor(tableElement: HTMLTableElement, config: DataExplorerConfig);
    /**
     * Initialize DataTables with server-side processing
     */
    private initializeDataTable;
    /**
     * Build sort parameter from DataTables order array
     */
    private buildSortParam;
    /**
     * Attach filter controls and event handlers
     */
    private attachFilterControls;
    /**
     * Apply filters and reload table
     */
    applyFilters(filters: Record<string, any>): void;
    /**
     * Clear all filters
     */
    clearFilters(): void;
    /**
     * Save current filter set with name
     */
    private saveCurrentFilterSet;
    /**
     * Load a saved filter set by name
     */
    private loadFilterSet;
    /**
     * Load saved filter sets from user preferences
     */
    private loadSavedFilters;
    /**
     * Update filter set dropdown options
     */
    private updateFilterSetDropdown;
    /**
     * Open filter builder modal
     */
    private openFilterBuilder;
    /**
     * Open export modal
     */
    private openExportModal;
    /**
     * Update summary banner with aggregate statistics
     */
    private updateSummaryBanner;
    /**
     * Format summary value for display
     */
    private formatSummaryValue;
    /**
     * Table draw callback
     */
    private onTableDraw;
    /**
     * Save grid preferences to user preference service
     */
    private savePreferences;
    /**
     * Restore grid preferences from user preference service
     */
    private restorePreferences;
    /**
     * Show error message
     */
    private showError;
    /**
     * Show warning message
     */
    private showWarning;
    /**
     * Show success message
     */
    private showSuccess;
    /**
     * Show toast notification
     */
    private showToast;
    /**
     * Reload table data
     */
    reload(): void;
    /**
     * Destroy the DataTable instance
     */
    destroy(): void;
}
export declare function initializeDataExplorerGrid(tableSelector: string, config: DataExplorerConfig): DataExplorerGrid | null;
//# sourceMappingURL=grid.d.ts.map