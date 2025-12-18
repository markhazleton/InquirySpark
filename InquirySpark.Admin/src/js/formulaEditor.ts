/**
 * Formula Editor Module for Chart Builder
 * Provides guided formula editing with function picker, column autocomplete, and inline validation
 */

interface FormulaFunction {
    name: string;
    description: string;
    syntax: string;
    example: string;
    category: 'math' | 'aggregate' | 'string' | 'conditional';
}

interface ValidationResult {
    isValid: boolean;
    errors: string[];
    warnings: string[];
    detectedColumns: string[];
    detectedFunctions: string[];
    normalizedFormula: string;
}

export class FormulaEditor {
    private textarea: HTMLTextAreaElement;
    private validationEndpoint: string;
    private chartType: string;
    private availableColumns: string[];
    private supportedFunctions: FormulaFunction[] = [];
    private validationTimeout: number | null = null;

    constructor(textareaId: string, chartType: string = 'bar', columns: string[] = []) {
        this.textarea = document.getElementById(textareaId) as HTMLTextAreaElement;
        if (!this.textarea) {
            throw new Error(`Textarea with id '${textareaId}' not found`);
        }

        this.validationEndpoint = '/api/chartdefinitions/validate-formula';
        this.chartType = chartType;
        this.availableColumns = columns;

        this.initialize();
    }

    private async initialize(): Promise<void> {
        // Load supported functions
        await this.loadSupportedFunctions();

        // Setup event listeners
        this.textarea.addEventListener('input', () => this.onInputChange());
        this.textarea.addEventListener('keydown', (e) => this.onKeyDown(e));

        // Create UI elements
        this.createToolbar();
        this.createValidationPanel();
        this.createAutocompleteDropdown();

        // Initial validation
        this.validateFormula();
    }

    private async loadSupportedFunctions(): Promise<void> {
        try {
            const response = await fetch(`/api/chartdefinitions/formula-functions?chartType=${this.chartType}`);
            const data = await response.json();
            
            // Map function names to full function objects
            this.supportedFunctions = data.functions.map((name: string) => this.getFunctionDetails(name));
        } catch (error) {
            console.error('Failed to load supported functions:', error);
        }
    }

    private getFunctionDetails(name: string): FormulaFunction {
        const functionDefs: { [key: string]: FormulaFunction } = {
            'Sum': { name: 'Sum', category: 'aggregate', description: 'Calculate sum of values', syntax: 'Sum([Column])', example: 'Sum([Sales])' },
            'Avg': { name: 'Avg', category: 'aggregate', description: 'Calculate average', syntax: 'Avg([Column])', example: 'Avg([Price])' },
            'Count': { name: 'Count', category: 'aggregate', description: 'Count rows', syntax: 'Count([Column])', example: 'Count([OrderId])' },
            'Max': { name: 'Max', category: 'aggregate', description: 'Find maximum value', syntax: 'Max([Column])', example: 'Max([Score])' },
            'Min': { name: 'Min', category: 'aggregate', description: 'Find minimum value', syntax: 'Min([Column])', example: 'Min([Score])' },
            'StdDev': { name: 'StdDev', category: 'aggregate', description: 'Standard deviation', syntax: 'StdDev([Column])', example: 'StdDev([Value])' },
            'Round': { name: 'Round', category: 'math', description: 'Round to nearest integer', syntax: 'Round(value)', example: 'Round([Price])' },
            'Abs': { name: 'Abs', category: 'math', description: 'Absolute value', syntax: 'Abs(value)', example: 'Abs([Difference])' },
            'If': { name: 'If', category: 'conditional', description: 'Conditional logic', syntax: 'If(condition, trueValue, falseValue)', example: 'If([Score] > 80, "Pass", "Fail")' },
        };

        return functionDefs[name] || {
            name,
            category: 'math',
            description: name,
            syntax: `${name}(...)`,
            example: `${name}([Column])`
        };
    }

    private createToolbar(): void {
        const toolbar = document.createElement('div');
        toolbar.className = 'formula-toolbar btn-toolbar mb-2';
        toolbar.innerHTML = `
            <div class="btn-group btn-group-sm me-2" role="group">
                <button type="button" class="btn btn-outline-primary" id="btnFunctionPicker">
                    <i class="bi bi-calculator"></i> Functions
                </button>
                <button type="button" class="btn btn-outline-primary" id="btnColumnPicker">
                    <i class="bi bi-table"></i> Columns
                </button>
            </div>
            <div class="btn-group btn-group-sm" role="group">
                <button type="button" class="btn btn-outline-secondary" id="btnValidate">
                    <i class="bi bi-check-circle"></i> Validate
                </button>
            </div>
        `;

        this.textarea.parentElement?.insertBefore(toolbar, this.textarea);

        // Event listeners
        document.getElementById('btnFunctionPicker')?.addEventListener('click', () => this.showFunctionPicker());
        document.getElementById('btnColumnPicker')?.addEventListener('click', () => this.showColumnPicker());
        document.getElementById('btnValidate')?.addEventListener('click', () => this.validateFormula());
    }

    private createValidationPanel(): void {
        const panel = document.createElement('div');
        panel.id = 'formulaValidationPanel';
        panel.className = 'formula-validation-panel mt-2';
        panel.style.display = 'none';
        
        this.textarea.parentElement?.appendChild(panel);
    }

    private createAutocompleteDropdown(): void {
        const dropdown = document.createElement('div');
        dropdown.id = 'formulaAutocomplete';
        dropdown.className = 'formula-autocomplete dropdown-menu';
        dropdown.style.display = 'none';
        dropdown.style.position = 'absolute';
        dropdown.style.maxHeight = '200px';
        dropdown.style.overflowY = 'auto';
        dropdown.style.zIndex = '1050';
        
        this.textarea.parentElement?.appendChild(dropdown);
    }

    private onInputChange(): void {
        // Debounced validation
        if (this.validationTimeout) {
            clearTimeout(this.validationTimeout);
        }

        this.validationTimeout = window.setTimeout(() => {
            this.validateFormula();
            this.checkForAutocomplete();
        }, 300);
    }

    private onKeyDown(e: KeyboardEvent): void {
        // Handle autocomplete navigation
        const dropdown = document.getElementById('formulaAutocomplete');
        if (dropdown && dropdown.style.display !== 'none') {
            if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
                e.preventDefault();
                this.navigateAutocomplete(e.key === 'ArrowDown' ? 1 : -1);
            } else if (e.key === 'Enter' || e.key === 'Tab') {
                e.preventDefault();
                this.selectAutocompleteItem();
            } else if (e.key === 'Escape') {
                this.hideAutocomplete();
            }
        }
    }

    private checkForAutocomplete(): void {
        const cursorPos = this.textarea.selectionStart;
        const textBeforeCursor = this.textarea.value.substring(0, cursorPos);
        
        // Check for column reference trigger [
        const columnMatch = textBeforeCursor.match(/\[([^\]]*?)$/);
        if (columnMatch) {
            this.showColumnAutocomplete(columnMatch[1]);
            return;
        }

        // Check for function trigger
        const functionMatch = textBeforeCursor.match(/\b(\w+)$/);
        if (functionMatch && functionMatch[1].length >= 2) {
            this.showFunctionAutocomplete(functionMatch[1]);
            return;
        }

        this.hideAutocomplete();
    }

    private showColumnAutocomplete(partial: string): void {
        const matches = this.availableColumns.filter(col => 
            col.toLowerCase().includes(partial.toLowerCase())
        );

        if (matches.length === 0) {
            this.hideAutocomplete();
            return;
        }

        this.showAutocompleteItems(matches.map(col => ({
            text: col,
            type: 'column',
            icon: 'bi-table'
        })));
    }

    private showFunctionAutocomplete(partial: string): void {
        const matches = this.supportedFunctions.filter(func => 
            func.name.toLowerCase().startsWith(partial.toLowerCase())
        );

        if (matches.length === 0) {
            this.hideAutocomplete();
            return;
        }

        this.showAutocompleteItems(matches.map(func => ({
            text: func.name,
            detail: func.description,
            type: 'function',
            icon: 'bi-calculator'
        })));
    }

    private showAutocompleteItems(items: any[]): void {
        const dropdown = document.getElementById('formulaAutocomplete')!;
        dropdown.innerHTML = items.map((item, index) => `
            <a class="dropdown-item autocomplete-item ${index === 0 ? 'active' : ''}" 
               href="#" 
               data-index="${index}"
               data-value="${item.text}">
                <i class="bi ${item.icon} me-2"></i>
                <span class="autocomplete-text">${item.text}</span>
                ${item.detail ? `<small class="text-muted d-block">${item.detail}</small>` : ''}
            </a>
        `).join('');

        // Position dropdown
        const rect = this.textarea.getBoundingClientRect();
        const cursorPos = this.getCursorCoordinates();
        dropdown.style.left = `${cursorPos.left}px`;
        dropdown.style.top = `${cursorPos.top + 20}px`;
        dropdown.style.display = 'block';

        // Add click handlers
        dropdown.querySelectorAll('.autocomplete-item').forEach(item => {
            item.addEventListener('click', (e) => {
                e.preventDefault();
                this.insertAutocompleteValue(item.getAttribute('data-value')!);
            });
        });
    }

    private hideAutocomplete(): void {
        const dropdown = document.getElementById('formulaAutocomplete')!;
        dropdown.style.display = 'none';
    }

    private navigateAutocomplete(direction: number): void {
        const dropdown = document.getElementById('formulaAutocomplete')!;
        const items = dropdown.querySelectorAll('.autocomplete-item');
        let activeIndex = Array.from(items).findIndex(item => item.classList.contains('active'));
        
        items[activeIndex]?.classList.remove('active');
        activeIndex = (activeIndex + direction + items.length) % items.length;
        items[activeIndex]?.classList.add('active');
        items[activeIndex]?.scrollIntoView({ block: 'nearest' });
    }

    private selectAutocompleteItem(): void {
        const activeItem = document.querySelector('#formulaAutocomplete .autocomplete-item.active');
        if (activeItem) {
            const value = activeItem.getAttribute('data-value')!;
            this.insertAutocompleteValue(value);
        }
    }

    private insertAutocompleteValue(value: string): void {
        const cursorPos = this.textarea.selectionStart;
        const textBefore = this.textarea.value.substring(0, cursorPos);
        const textAfter = this.textarea.value.substring(cursorPos);

        // Find what to replace
        const columnMatch = textBefore.match(/\[([^\]]*?)$/);
        const functionMatch = textBefore.match(/\b(\w+)$/);

        let newTextBefore = textBefore;
        if (columnMatch) {
            newTextBefore = textBefore.substring(0, textBefore.length - columnMatch[0].length) + `[${value}]`;
        } else if (functionMatch) {
            newTextBefore = textBefore.substring(0, textBefore.length - functionMatch[0].length) + `${value}(`;
        }

        this.textarea.value = newTextBefore + textAfter;
        this.textarea.selectionStart = this.textarea.selectionEnd = newTextBefore.length;
        this.hideAutocomplete();
        this.textarea.focus();
    }

    private getCursorCoordinates(): { left: number; top: number } {
        const rect = this.textarea.getBoundingClientRect();
        return {
            left: rect.left + window.scrollX,
            top: rect.top + window.scrollY + this.textarea.offsetHeight
        };
    }

    private showFunctionPicker(): void {
        // Create modal with function list
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.innerHTML = `
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Formula Functions</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="list-group">
                            ${this.supportedFunctions.map(func => `
                                <a href="#" class="list-group-item list-group-item-action function-item" 
                                   data-function="${func.name}">
                                    <div class="d-flex w-100 justify-content-between">
                                        <h6 class="mb-1">${func.name}</h6>
                                        <small class="text-muted">${func.category}</small>
                                    </div>
                                    <p class="mb-1">${func.description}</p>
                                    <small class="text-muted">
                                        <strong>Syntax:</strong> <code>${func.syntax}</code><br>
                                        <strong>Example:</strong> <code>${func.example}</code>
                                    </small>
                                </a>
                            `).join('')}
                        </div>
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(modal);
        const bsModal = new (window as any).bootstrap.Modal(modal);
        bsModal.show();

        // Handle function selection
        modal.querySelectorAll('.function-item').forEach(item => {
            item.addEventListener('click', (e) => {
                e.preventDefault();
                const funcName = item.getAttribute('data-function')!;
                this.insertAtCursor(`${funcName}(`);
                bsModal.hide();
            });
        });

        modal.addEventListener('hidden.bs.modal', () => {
            modal.remove();
        });
    }

    private showColumnPicker(): void {
        if (this.availableColumns.length === 0) {
            alert('No columns available. Please select a dataset first.');
            return;
        }

        const dropdown = document.getElementById('formulaAutocomplete')!;
        this.showAutocompleteItems(this.availableColumns.map(col => ({
            text: col,
            type: 'column',
            icon: 'bi-table'
        })));
    }

    private insertAtCursor(text: string): void {
        const start = this.textarea.selectionStart;
        const end = this.textarea.selectionEnd;
        const before = this.textarea.value.substring(0, start);
        const after = this.textarea.value.substring(end);

        this.textarea.value = before + text + after;
        this.textarea.selectionStart = this.textarea.selectionEnd = start + text.length;
        this.textarea.focus();
    }

    private async validateFormula(): Promise<void> {
        const formula = this.textarea.value.trim();
        if (!formula) {
            this.hideValidationPanel();
            return;
        }

        try {
            const response = await fetch(this.validationEndpoint, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    formula,
                    chartType: this.chartType,
                    availableColumns: this.availableColumns
                })
            });

            if (!response.ok) {
                throw new Error('Validation request failed');
            }

            const result: ValidationResult = await response.json();
            this.displayValidationResult(result);
        } catch (error) {
            console.error('Formula validation error:', error);
            this.showValidationError('Failed to validate formula');
        }
    }

    private displayValidationResult(result: ValidationResult): void {
        const panel = document.getElementById('formulaValidationPanel')!;
        
        if (result.isValid) {
            panel.className = 'formula-validation-panel alert alert-success mt-2';
            panel.innerHTML = `
                <i class="bi bi-check-circle-fill me-2"></i>
                <strong>Valid formula</strong>
                ${result.warnings.length > 0 ? `
                    <div class="mt-2">
                        <small class="text-warning">
                            <i class="bi bi-exclamation-triangle me-1"></i>
                            ${result.warnings.join('<br>')}
                        </small>
                    </div>
                ` : ''}
            `;
        } else {
            panel.className = 'formula-validation-panel alert alert-danger mt-2';
            panel.innerHTML = `
                <i class="bi bi-x-circle-fill me-2"></i>
                <strong>Invalid formula</strong>
                <ul class="mb-0 mt-2">
                    ${result.errors.map(err => `<li>${err}</li>`).join('')}
                </ul>
            `;
        }

        panel.style.display = 'block';
    }

    private showValidationError(message: string): void {
        const panel = document.getElementById('formulaValidationPanel')!;
        panel.className = 'formula-validation-panel alert alert-warning mt-2';
        panel.innerHTML = `<i class="bi bi-exclamation-triangle me-2"></i>${message}`;
        panel.style.display = 'block';
    }

    private hideValidationPanel(): void {
        const panel = document.getElementById('formulaValidationPanel')!;
        panel.style.display = 'none';
    }

    public setChartType(chartType: string): void {
        this.chartType = chartType;
        this.loadSupportedFunctions();
        this.validateFormula();
    }

    public setAvailableColumns(columns: string[]): void {
        this.availableColumns = columns;
        this.validateFormula();
    }
}

// Export for use in other modules
(window as any).FormulaEditor = FormulaEditor;
