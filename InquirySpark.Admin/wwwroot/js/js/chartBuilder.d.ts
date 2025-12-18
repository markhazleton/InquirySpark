export interface ChartDefinition {
    chartDefinitionId: number;
    datasetId: number;
    name: string;
    description?: string;
    tags?: string;
    filterPayload?: string;
    visualPayload?: string;
    calculationPayload?: string;
    versionNumber: number;
    autoApprovedFl: boolean;
}
export interface ChartBuilderState {
    currentDefinition: ChartDefinition | null;
    isDirty: boolean;
    validationErrors: string[];
}
export declare class ChartBuilderManager {
    private chartCanvas;
    private onStateChange?;
    private state;
    private previewChart;
    constructor(chartCanvas: HTMLCanvasElement, onStateChange?: ((state: ChartBuilderState) => void) | undefined);
    /**
     * Initialize event listeners for form fields
     */
    private initializeEventListeners;
    /**
     * Attach JSON validation to textarea fields
     */
    private attachJsonValidation;
    /**
     * Validate JSON syntax in a field
     */
    private validateJsonField;
    /**
     * Set error message for a field
     */
    private setFieldError;
    /**
     * Clear error message for a field
     */
    private clearFieldError;
    /**
     * Load chart definition into the builder
     */
    loadDefinition(definition: ChartDefinition): void;
    /**
     * Update chart preview with new configuration
     */
    updatePreview(visualPayload: string): void;
    /**
     * Validate all form fields before submission
     */
    validateForm(): boolean;
    /**
     * Notify state change callback
     */
    private notifyStateChange;
    /**
     * Check if form has unsaved changes
     */
    hasUnsavedChanges(): boolean;
    /**
     * Get current builder state
     */
    getState(): ChartBuilderState;
}
//# sourceMappingURL=chartBuilder.d.ts.map