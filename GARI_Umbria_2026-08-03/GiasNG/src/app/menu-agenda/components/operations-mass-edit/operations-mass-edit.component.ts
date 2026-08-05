import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { AGRODATAINIZIO, LAVCOD_CATTURE_MASSA, LAVCOD_CONCIA_SEME, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISERBO, LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_DISSECCAMENTO, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_DISTRIBUZIONE_INSETTI, LAVCOD_FERTIRRIGAZIONE, LAVCOD_GEODISINFESTAZIONE, LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_TRATTAMENTO_POST_RACCOLTA } from 'app/Model/CostantiPersonalizzate';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { FabbricatiService, LeggiMagazzini_QdC } from 'app/Service/Anagrafica/fabbricati.service';
import { MasterService } from 'app/Service/master.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { FiltersService } from '../filters/filters.service';
import { OperationEditService } from './operation-edit.service';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GridOpEditSrvice } from './grid-op-edit.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { enum_ModificaMultiplaOperazioni } from '../models';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { cloneDeep } from 'lodash';
import { QdCRow } from '../utils';
import {Impresa} from "../../../Model/anagrafiche/Impresa";
import {enum_LAVCOD} from "../../../Model/TipiEnumerativi";

@Component({
    standalone: false,
    selector: 'app-operations-mass-edit',
    templateUrl: './operations-mass-edit.component.html',
    styleUrls: ['./operations-mass-edit.component.css'],
    providers: [
        ...generateGridProviders(GridOpEditSrvice, OperationsMassEditComponent),
        GiasDropDownTemplateService
    ]
})
export class OperationsMassEditComponent implements OnInit {

    @Input() activities: QdCRow[];

    @ViewChild('gridOp') grid;

    storages: Fabbricato[] = [];
    private _operations: BaseCodeDescr[] = [
        { codice: enum_ModificaMultiplaOperazioni.MODIFICA_TIENI_QTA_TOT, descrizione: this.transloco.translate('EditKeepQtaTot')},
        { codice: enum_ModificaMultiplaOperazioni.MODIFICA_TIENI_QTA_HA, descrizione: this.transloco.translate('EditKeepQtaHa')},
        { codice: enum_ModificaMultiplaOperazioni.ASSEGNA_MAGAZZINO, descrizione: this.transloco.translate('EditAssignStorage')},
        { codice: enum_ModificaMultiplaOperazioni.AGGIUNGI_MACCHINE, descrizione: this.transloco.translate('EditAddMachines')},
        { codice: enum_ModificaMultiplaOperazioni.RIMUOVI_MACCHINE, descrizione: this.transloco.translate('EditRemoveMachines')},
        { codice: enum_ModificaMultiplaOperazioni.AGGIUNGI_CONTATTI, descrizione: this.transloco.translate('EditAddOperators')},
        { codice: enum_ModificaMultiplaOperazioni.RIMUOVI_CONTATTI, descrizione: this.transloco.translate('EditRemoveOperators')},
    ];

    constructor(
        private transloco: TranslocoService,
        private datashare: OperationEditService,
        private fabbricatiService: FabbricatiService,
        private centriService: CentriAziendaliService,
        private master: MasterService,
        private agenda: ObjParametriAgendaService,
        private filtersService: FiltersService,
        private dialogService: GiasDialogService
    ) {   }

    get operations(): BaseCodeDescr[] {
        return this._operations;
    }

    get shouldShowSection(): boolean {
        return !!this.form.get('operazione').value
            && this.form.get('operazione').value > -5;
    }

    get form(): FormGroup {
        return this.datashare.editForm as FormGroup;
    }

    get attivitaSiMagazzino(): number[] {
        return [
            enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO, // 74
            enum_LAVCOD.CONCIA_SEME, // 13
            enum_LAVCOD.DISERBO, // 18
            enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE, // 103
            enum_LAVCOD.DISTRIBUZIONE_INSETTI, // 16
            enum_LAVCOD.CONFUSIONE_SESSUALE, // 118
            enum_LAVCOD.DISORIENTAMENTO_SESSUALE, // 121
            enum_LAVCOD.CATTURE_MASSA, // 122
            enum_LAVCOD.GEODISINFESTAZIONE, // 155
            enum_LAVCOD.DISSECCAMENTO, // 158
            enum_LAVCOD.TRATTAMENTO_POST_RACCOLTA, // 163
            enum_LAVCOD.INSTALLAZIONE_TRAPPOLE, // 107
            enum_LAVCOD.REINNESCO_TRAPPOLE, // 150
            enum_LAVCOD.FERTIRRIGAZIONE, // 26
            enum_LAVCOD.DISTRIBUZIONE_CONCIME, // 14
            enum_LAVCOD.CONCIMAZIONE_FOGLIARE, // 123
            enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI, // 124
            enum_LAVCOD.SARCHIATURA_CONCIMAZIONE, // 156
            enum_LAVCOD.TRATTAMENTO_ANTIBUTTERATURA, // 106,
            enum_LAVCOD.INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA //173
        ];
    }

    ngOnInit(): void {
        this.activities = this.activities.filter(r => r.tipo !== 'E')
        this.datashare.editForm.get('attivita').patchValue(this.activities);
        this.handleEvents();
        this.loadStorages();
    }

    onSave() {
        if (!this.isValid()) return;
        this.datashare.saveCurrent();
    }

    private isValid() {
        if (!this.datashare.editForm.get('operazione').value) {
            this.dialogService.baseError(
                '', 'NessunaOperazioneModifica'
            );
            return false;
        }

        let attivita = this.grid.rows.filter(r => r['Selected']);
        if (!attivita.length) {
            this.dialogService.baseError(
                '', 'SelezionareAlmenoUnOperazione'
            );
            return false;
        }

        let selected;
        switch (this.datashare.editForm.get('operazione').value) {
            case  enum_ModificaMultiplaOperazioni.ASSEGNA_MAGAZZINO:
                let mag: Fabbricato = this.datashare.editForm.get('magazzino').value;

                if ( mag.primaryKey.codice !== 0
                    && mag.primaryKey.centroAziendalePK.codice !== 0
                    && mag.primaryKey.centroAziendalePK.partitaIva !== "")
                    return true;

                    this.dialogService.baseError(
                        '', 'NessunMagazzinoModifica'
                    );
                return false;

            case  enum_ModificaMultiplaOperazioni.AGGIUNGI_MACCHINE:
                selected = this.datashare.editForm.get('macchine')
                               .get('macchine').value
                               .filter(m => m['Selected']);

                if (selected.length > 0) return true;

                this.dialogService.baseError(
                    '', 'SelezionareAlmenoUnaMacchina'
                );
                return false;

            case  enum_ModificaMultiplaOperazioni.AGGIUNGI_CONTATTI:
                selected = this.datashare.editForm.get('manodopera')
                               .get('contatti').value
                               .filter(m => m['Selected']);

                if (selected.length > 0) return true;

                this.dialogService.baseError(
                    '', 'SelezionareAlmenoUnContatto'
                );
                return false;
        }

        return true;
    }

    private loadStorages() {
        const company = this.agenda.getObjParamValue();
        const piva = company.Piva;
        const LeggiMagazzini = <LeggiMagazzini_QdC>{
            lavorazione: new Lavorazione('0'),
            impresa: {partitaIva: piva} as Impresa,
            data: AGRODATAINIZIO
        };
        this.fabbricatiService.Leggi_Magazzini(LeggiMagazzini).subscribe(storages => this.storages = storages);
    }

    private handleEvents() {
        this.form.get('operazione').valueChanges.GiasSubscribe(ch => {
            if (ch === enum_ModificaMultiplaOperazioni.ASSEGNA_MAGAZZINO) {
                let orig = cloneDeep(this.form.get('attivita').value);
                let attivita = this.form.get('attivita').value;
                attivita = attivita.filter(a => {
                    if (a['Selected'] == true){
                        a['Selected'] = this.attivitaSiMagazzino.includes(parseInt(a.Lav_cod));
                    }
                    return a['Selected'];
                });
                if(attivita.length !== orig.length) {
                    this.dialogService.baseInfo('','qdc.ModificaOperazioniNoMagazzino');
                }
            }

            this.form.get('magazzino').patchValue(new Fabbricato({
                codice: 0,
                centroAziendalePK: {codice: 0, partitaIva: ''}
            }));

            this.form.get('macchine').get('macchine').patchValue([]);
            this.form.get('macchine').get('eliminaAssociate').patchValue(false);
            this.form.get('manodopera').get('contatti').patchValue([]);
            this.form.get('manodopera').get('eliminaAssociati').patchValue(false);
        });
    }

}
