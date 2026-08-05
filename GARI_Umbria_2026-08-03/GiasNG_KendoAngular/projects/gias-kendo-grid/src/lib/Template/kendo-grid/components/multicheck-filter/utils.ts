
import { faCog, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { TranslocoService } from '@jsverse/transloco';
import { PopupCloseEvent } from '@progress/kendo-angular-grid';
import { CompositeFilterDescriptor, FilterDescriptor } from '@progress/kendo-data-query';
import { DropdownListItem } from '../../models/grid.model';
import { ElementRef } from '@angular/core';

export class DateFilters {
    start: Date;
    end: Date;
    filterDescriptors: FilterDescriptor | CompositeFilterDescriptor;
}

export enum FILTER_MODES {
    BASIC_FILTERS,
    ADVANCED_FILTERS
}

export class filtersManager {
    public readonly basicFilteringMode = FILTER_MODES.BASIC_FILTERS;
    public readonly extendedFilteringMode = FILTER_MODES.ADVANCED_FILTERS;
    public activeMode: FILTER_MODES = FILTER_MODES.BASIC_FILTERS;
    public advancedModeActive = false;
    faCog: IconDefinition = faCog;
    switchColor: 'cornflowerblue' | '#dc3545' = '#dc3545';
    changingSettingsMode = false;

    public switchActiveMode() {
        this.activeMode = this.activeMode === this.basicFilteringMode ?
            this.extendedFilteringMode : this.basicFilteringMode;
        this.switchColor = this.switchColor === 'cornflowerblue' ?
            '#dc3545' : 'cornflowerblue';
        this.changingSettingsMode = true;
    }

    /**
     * Prevents popup from closing if the filtering mode has been recently
     * changed. Strangely the popup would normally close when the
     * switchActiveMode() method is called. This has to do with the use of
     * the ng-template #settingsFooter in the view.
    */
    public preventPopupCloseIfRequired(e: PopupCloseEvent) {
        if (this.changingSettingsMode) {
            this.changingSettingsMode = false;
            e.preventDefault();
        }
    }
}

export class FormValueManager {

    private readonly firstOpDefault = { id: 'gte', name: 'Greater than or equal to' };
    private readonly secondOpDefault = { id: 'lte', name: 'Less than or equal to' };
    private readonly logicOpDefault = { id: 'and', name: 'And' };

    public start: Date;
    public end: Date;
    public operators: DropdownListItem[];
    public logicOperators: DropdownListItem[];
    public firstOperator: DropdownListItem = this.firstOpDefault;
    public secondOperator: DropdownListItem = this.secondOpDefault;
    public logicOperator: DropdownListItem = this.logicOpDefault;

    constructor(transloco: TranslocoService) {
        this.operators = this.translateOperators(transloco);
        this.logicOperators = this.translateLogicalOperators(transloco);
    }

    hasFilters() {
        return this.start != null || this.end != null;
    }

    clearFilters() {
        this.start = null;
        this.end = null;
    }

    resetOperators() {
        this.firstOperator = this.firstOpDefault;
        this.secondOperator = this.secondOpDefault;
        this.logicOperator = this.logicOpDefault;
    }

    private translateOperators(transloco: TranslocoService) {
        let eq = transloco.translate('giasgrid.eq');
        let neq = transloco.translate('giasgrid.neq');
        let isnull = transloco.translate('giasgrid.isnull');
        let isnotnull = transloco.translate('giasgrid.isnotnull');
        let lt = transloco.translate('giasgrid.lt');
        let lte = transloco.translate('giasgrid.lte');
        let gt = transloco.translate('giasgrid.gt');
        let gte = transloco.translate('giasgrid.gte');

        return [
            { id: 'eq', name: eq },
            { id: 'neq', name: neq },
            { id: 'isnull', name: isnull },
            { id: 'isnotnull', name: isnotnull },
            { id: 'lt', name: lt },
            { id: 'lte', name: lte },
            { id: 'gt', name: gt },
            { id: 'gte', name: gte }
        ]
    }

    private translateLogicalOperators(transloco: TranslocoService) {
        let and = transloco.translate('giasgrid.and');
        let or = transloco.translate('giasgrid.or');

        return [
            { id: 'and', name: and },
            { id: 'or', name: or }
        ]
    }
}
