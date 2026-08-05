import { AfterContentChecked, Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { enum_TipoControllo } from '../../../../shared/TipiEnumerativi';
import { FormControl, FormGroup } from '@angular/forms';
import { AGRODATAINIZIO } from '../../../../shared/CostantiPersonalizzate';
import {
    BooleanSettings,
    DateSettings,
    DropdownListWithForm,
    NumericSettings,
    StringSettings
} from '../../models/grid.model';
import { isBoolean, isNumber, isString } from 'lodash';
import { CustomComponent } from '../../models/custom-component';

export class DynamicInputSettigs {
    /** Il tipo di controllo da usare. */
    controlType: enum_TipoControllo = 0;
    /** Il vero campo contenente il valore. */
    realField: string;
    /** Funzione da eseguire al cambiamento del valore. */
    onEdit? = (newValue: any) => {
        let a = 0; //Commento per funzione vuota SonarQube
    };

    ddl?: DropdownListWithForm = null;
    numeric?: NumericSettings = null;
    boolean?: BooleanSettings = null;
    date?: DateSettings = null;
    string?: StringSettings = null;

    constructor(settings?: DynamicInputSettigs) {
        this.controlType = settings?.controlType || 0;
        this.realField = settings?.realField || undefined;
        this.onEdit = settings?.onEdit || null;
        this.ddl = settings?.ddl || null;
        this.numeric = settings?.numeric || new NumericSettings();
        this.boolean = settings?.boolean || new BooleanSettings();
        this.date = settings?.date || new DateSettings();
        this.string = settings?.string || new StringSettings();
    }
}

/** Per ora utilizzato solo in <code>grid-rilievi.service.ts</code>.
 *  @Warning va in conflitto con le griglie con EditingMode di tipo <code>EditingMode.IN_CELL</code>
 */
@Component({
    standalone: false,
    selector: 'gias-dynamic-input',
    templateUrl: './dynamic-input.component.html',
    styleUrls: ['./dynamic-input.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class DynamicInputComponent implements OnInit, AfterContentChecked, CustomComponent {

    /** Il dataItem rappresentante la riga corrente. */
    @Input() input: any;
    /** Indica il campo in dataItem di tipo {@link DynamicInputSettigs} a cui fare
     *  riferimento per usare il componente.
     *  Necessario specificarlo per ogni riga in modo da rendere la cosa
     *  dinamica.
     */
    @Input() field: any;
    @Input() edit: boolean = false;

    public settings: DynamicInputSettigs;

    public form: FormGroup;
    public enum_TipoControllo = {
        UNDEFINED: 0,
        CASELLA_TESTO: 1,
        AREA_TESTO: 2,
        MENU_DISCESA: 3,
        CASELLA_SPUNTA: 4,
        CALENDARIO: 5,
        ALLEGATO: 6,
        LINK: 7,
        PASSWORD: 8,
        MULTISELECT_ESTESA_SERVER: 9,
        PULSANTE_SCELTA: 10,
        IMMAGINE: 11,
        DDL_ESTESA_CLIENT: 12,
        GIS_VIEWER: 13,
        DDL_ESTESA_SERVER_LIGHT: 14,
        NUMERO_INTERO: 15,
        NUMERO_DECIMALE: 16,
        MULTISELECT: 17
    };
    public AGRODATA_INIZIO: Date = AGRODATAINIZIO;
    public fieldControl: FormControl;

    private _internalChange = false;

    ngOnInit(): void {
        this.settings = new DynamicInputSettigs(this.input[this.field]);
        if (this.input && this.settings.realField) {
            this.createForm();
        }
    }

    public update(ev) {
        switch (this.settings.controlType) {
            case enum_TipoControllo.CALENDARIO:
            case enum_TipoControllo.NUMERO_DECIMALE:
                this.input[this.settings.realField] = this.fieldControl.value;
                break;
            case enum_TipoControllo.CASELLA_SPUNTA:
                this.input[this.settings.realField] = this.fieldControl.value ? 1 : 0;
                break;
            case enum_TipoControllo.NUMERO_INTERO:
                this.fieldControl.patchValue(Math.round(this.fieldControl.value));
                this.input[this.settings.realField] = this.fieldControl.value;
                break;
            case enum_TipoControllo.MENU_DISCESA:
                //const toPatch = this.settings.ddl.valuePrimitive ? ev.data.id : ev.data;
                this.input[this.settings.realField] = ev.data;
                break;
            default:
                console.error("Update handling for type ", this.settings.controlType, " not yet implemented!");
        }
        if (this.settings.onEdit) {
            this.settings.onEdit(this.input);
        }
    }

    private createForm() {
        let property: keyof typeof this.input;
        this.form = new FormGroup([]);
        for (property in this.input) {
            this.form.addControl(property, new FormControl(this.input[property]));
        }
        this.fieldControl = this.form.get(this.settings.realField) as FormControl;
        this.updateFieldControl();
    }

    ngAfterContentChecked(): void {
        // if (this.fieldControl?.value !== this.input[this.settings?.realField]) {
        //     this._internalChange = true;
        //     this.updateFieldControl();
        // }
    }

    /**
     * Patches the value of the field in the control, eventually adapting
     * different values to the expected ones. i.e. boolean values.
     * @private
     */
    private updateFieldControl() {
        if (this.settings.controlType === enum_TipoControllo.CASELLA_SPUNTA
            && !isBoolean(this.input[this.settings.realField])) {
            this.fieldControl.patchValue(this.parseBoolean(this.input[this.settings.realField]));
        } else {
            this.fieldControl.patchValue(this.input[this.settings.realField]);
        }
    }

    private parseBoolean(value: any): boolean {
        if (isNumber(value)) {
            return value !== 0;
        } else if (isString(value)) {
            return !(value === "" || value === "0");
        } else
            return !!value;
    }
}
