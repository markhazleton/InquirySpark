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
export declare class ChartGalleryManager {
    private tableElement;
    private dataTable;
    constructor(tableElement: HTMLTableElement);
    /**
     * Initialize DataTables with custom configuration
     */
    private initializeDataTable;
    /**
     * Attach custom filter controls
     */
    private attachCustomFilters;
    /**
     * Reload table data
     */
    reload(): void;
    /**
     * Clear all filters
     */
    clearFilters(): void;
    /**
     * Export table data
     */
    exportData(format: 'csv' | 'excel' | 'pdf'): void;
}
//# sourceMappingURL=chartGallery.d.ts.map