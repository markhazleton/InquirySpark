// Chart Builder TypeScript Module
// Manages state and interactions for the chart definition builder interface

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

export class ChartBuilderManager {
    private state: ChartBuilderState = {
        currentDefinition: null,
        isDirty: false,
        validationErrors: []
    };

    private previewChart: any = null;

    constructor(
        private chartCanvas: HTMLCanvasElement,
        private onStateChange?: (state: ChartBuilderState) => void
    ) {
        this.initializeEventListeners();
    }

    /**
     * Initialize event listeners for form fields
     */
    private initializeEventListeners(): void {
        // Monitor changes to form fields
        const form = document.querySelector('form') as HTMLFormElement;
        if (form) {
            form.addEventListener('input', () => {
                this.state.isDirty = true;
                this.notifyStateChange();
            });
        }

        // JSON payload validation on blur
        this.attachJsonValidation('FilterPayload');
        this.attachJsonValidation('VisualPayload');
        this.attachJsonValidation('CalculationPayload');
    }

    /**
     * Attach JSON validation to textarea fields
     */
    private attachJsonValidation(fieldName: string): void {
        const field = document.getElementById(fieldName) as HTMLTextAreaElement;
        if (field) {
            field.addEventListener('blur', () => {
                this.validateJsonField(field, fieldName);
            });
        }
    }

    /**
     * Validate JSON syntax in a field
     */
    private validateJsonField(field: HTMLTextAreaElement, fieldName: string): boolean {
        const value = field.value.trim();
        if (!value) return true; // Empty is valid

        try {
            JSON.parse(value);
            this.clearFieldError(field);
            return true;
        } catch (e) {
            this.setFieldError(field, `Invalid JSON in ${fieldName}: ${(e as Error).message}`);
            return false;
        }
    }

    /**
     * Set error message for a field
     */
    private setFieldError(field: HTMLElement, message: string): void {
        field.classList.add('is-invalid');
        
        // Remove existing error span if present
        const existingError = field.parentElement?.querySelector('.invalid-feedback');
        if (existingError) {
            existingError.remove();
        }

        // Add new error span
        const errorSpan = document.createElement('div');
        errorSpan.className = 'invalid-feedback d-block';
        errorSpan.textContent = message;
        field.parentElement?.appendChild(errorSpan);
    }

    /**
     * Clear error message for a field
     */
    private clearFieldError(field: HTMLElement): void {
        field.classList.remove('is-invalid');
        const errorSpan = field.parentElement?.querySelector('.invalid-feedback');
        if (errorSpan) {
            errorSpan.remove();
        }
    }

    /**
     * Load chart definition into the builder
     */
    public loadDefinition(definition: ChartDefinition): void {
        this.state.currentDefinition = definition;
        this.state.isDirty = false;
        this.state.validationErrors = [];
        this.notifyStateChange();
        
        // Update preview if visual payload exists
        if (definition.visualPayload) {
            this.updatePreview(definition.visualPayload);
        }
    }

    /**
     * Update chart preview with new configuration
     */
    public updatePreview(visualPayload: string): void {
        if (!this.chartCanvas) return;

        try {
            const config = JSON.parse(visualPayload);
            
            // Destroy existing chart if present
            if (this.previewChart) {
                this.previewChart.destroy();
            }

            // Create new chart
            // @ts-ignore - Chart.js loaded globally
            this.previewChart = new Chart(this.chartCanvas.getContext('2d'), config);
            
            this.clearFieldError(document.getElementById('VisualPayload') as HTMLElement);
        } catch (e) {
            console.error('Failed to render chart preview:', e);
            this.setFieldError(
                document.getElementById('VisualPayload') as HTMLElement, 
                `Preview failed: ${(e as Error).message}`
            );
        }
    }

    /**
     * Validate all form fields before submission
     */
    public validateForm(): boolean {
        this.state.validationErrors = [];

        const filterPayloadField = document.getElementById('FilterPayload') as HTMLTextAreaElement;
        const visualPayloadField = document.getElementById('VisualPayload') as HTMLTextAreaElement;
        const calculationPayloadField = document.getElementById('CalculationPayload') as HTMLTextAreaElement;

        let isValid = true;

        if (filterPayloadField && !this.validateJsonField(filterPayloadField, 'FilterPayload')) {
            isValid = false;
        }

        if (visualPayloadField && !this.validateJsonField(visualPayloadField, 'VisualPayload')) {
            isValid = false;
        }

        if (calculationPayloadField && !this.validateJsonField(calculationPayloadField, 'CalculationPayload')) {
            isValid = false;
        }

        this.notifyStateChange();
        return isValid;
    }

    /**
     * Notify state change callback
     */
    private notifyStateChange(): void {
        if (this.onStateChange) {
            this.onStateChange(this.state);
        }
    }

    /**
     * Check if form has unsaved changes
     */
    public hasUnsavedChanges(): boolean {
        return this.state.isDirty;
    }

    /**
     * Get current builder state
     */
    public getState(): ChartBuilderState {
        return { ...this.state };
    }
}

// Auto-initialize on page load if canvas is present
document.addEventListener('DOMContentLoaded', () => {
    const canvas = document.getElementById('previewCanvas') as HTMLCanvasElement;
    if (canvas) {
        const manager = new ChartBuilderManager(canvas, (state) => {
            console.log('Chart builder state changed:', state);
        });

        // Attach to window for global access
        (window as any).chartBuilderManager = manager;

        // Add form submission validation
        const form = document.querySelector('form') as HTMLFormElement;
        if (form) {
            form.addEventListener('submit', (e) => {
                if (!manager.validateForm()) {
                    e.preventDefault();
                    alert('Please fix validation errors before submitting.');
                }
            });
        }

        // Warn about unsaved changes
        window.addEventListener('beforeunload', (e) => {
            if (manager.hasUnsavedChanges()) {
                e.preventDefault();
                e.returnValue = 'You have unsaved changes. Are you sure you want to leave?';
            }
        });
    }
});
