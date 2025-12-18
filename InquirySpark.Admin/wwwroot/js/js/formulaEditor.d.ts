/**
 * Formula Editor Module for Chart Builder
 * Provides guided formula editing with function picker, column autocomplete, and inline validation
 */
export declare class FormulaEditor {
    private textarea;
    private validationEndpoint;
    private chartType;
    private availableColumns;
    private supportedFunctions;
    private validationTimeout;
    constructor(textareaId: string, chartType?: string, columns?: string[]);
    private initialize;
    private loadSupportedFunctions;
    private getFunctionDetails;
    private createToolbar;
    private createValidationPanel;
    private createAutocompleteDropdown;
    private onInputChange;
    private onKeyDown;
    private checkForAutocomplete;
    private showColumnAutocomplete;
    private showFunctionAutocomplete;
    private showAutocompleteItems;
    private hideAutocomplete;
    private navigateAutocomplete;
    private selectAutocompleteItem;
    private insertAutocompleteValue;
    private getCursorCoordinates;
    private showFunctionPicker;
    private showColumnPicker;
    private insertAtCursor;
    private validateFormula;
    private displayValidationResult;
    private showValidationError;
    private hideValidationPanel;
    setChartType(chartType: string): void;
    setAvailableColumns(columns: string[]): void;
}
//# sourceMappingURL=formulaEditor.d.ts.map