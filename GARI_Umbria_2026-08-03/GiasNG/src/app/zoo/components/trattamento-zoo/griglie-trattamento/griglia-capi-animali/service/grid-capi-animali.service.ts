import { enum_UnitaMisura } from "app/Model/TipiEnumerativi";
import { GiasMessageService } from "app/Service/gias-message.service";
import { ConversionService, Enum_DBTypeOperation } from "gias-ui-kit";
import { effect, Injectable, Injector, OnDestroy } from "@angular/core";
import { RisorsaPersona } from "app/Model/attivita/risorse/RisorsaPersona";
import { CellCloseEvent, SelectionEvent } from "@progress/kendo-angular-grid";
import { enum_TipoPrescrizione } from "app/zoo/models/tipo-prescrizione.enum";
import { CapiAnimaliConfigService, CapoAnimale } from "./capi-animali-config.service";
import { TrattamentoZooFormService } from "../../../service/trattamento-zoo-form.service";
import { Observable, catchError, map, of, Subject, takeUntil, merge, switchMap } from "rxjs";
import { enum_FarmacoCategoria, enum_Tipo_RisorsaUmana, enum_TypeTab_Zootecnia, enum_UdM_Dose } from "app/zoo/models/tipi-enumerativi-zoo";
import { LeggiGiacenzeZooDto, LeggiGiacenzeZooFirstSommDto, OperazioniZooClient, RispostaStandard } from "app/Service/net-core6-api.service";
import { AbstractGridConfigService, CommandsColumnSettings, CommandsDropDownSettings, ConfigTemplate, EditingMode, GiasKendoGridComponent, GridPublicService, HttpAction, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, RendererGridEvent, SelectableSettings } from "gias-kendo-grid";

export class GridCapiAnimaliServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class GridCapiAnimaliService extends AbstractGridConfigService<GridCapiAnimaliServerResult> implements OnDestroy {

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    isEditing: boolean = true;

    rowId: string = 'idCapiAnimali';
    gridId: string = 'GridCapiAnimaliId';

    signal: Subject<void> = new Subject();

    CFproprietario: string = "";

    isTotEditable: boolean = false;
    totaleFarmacoDaSomministrare: number = 0;

    public numericFormat: string;

    constructor(
        injector: Injector,
        public gridpublicService: GridPublicService,
        public trattamentoFormService: TrattamentoZooFormService,
        private giasMessageService: GiasMessageService,
        private capiAnimaliConfigService: CapiAnimaliConfigService,
        private zooService: OperazioniZooClient,
        private conversionService: ConversionService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        // Se prescrizione di tipo Veterinaria o Indicazione Terapeutica, il campo Quantita non è editabile
        this.capiAnimaliConfigService.kendoColumnsCapiAnimali.forEach(col => {
            if (col.field === 'Quantita')  col.editable = !this.trattamentoFormService.checkIsVeterinaryOrIndTerapeuticPrescription();
        });

        this.resizable.autoFitColumns = true;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false,
            onDisableInfoBtn: () => false,
            width: 10
        });

        this.cmdDropDown = new CommandsDropDownSettings({
            removeBtn: false, infoBtn: false
        });

        merge(
              this.trattamentoFormService.ddlSelectionSubject,
              this.trattamentoFormService.ddlSelectionRaggruppamentoSubject
        ).pipe(
            takeUntil(this.signal)
        ).subscribe(() =>
            this.gridpublicService.refresh(true)
        );

        if (!this.trattamentoFormService.isInfoMode
            && !this.trattamentoFormService.checkIsVeterinaryOrIndTerapeuticPrescription()) {
            this.selectable.selectable = new SelectableSettings({
                checkboxOnly: true,
                enabled: true
            });

            this.selectable.shouldShowCheckbox = true;
            this.selectable.columnSettings.title = ' ';
            this.selectable.preselectedRows.selectionChangeFn = this.selectionChangeFn;
            this.selectable.columnSettings.showSelectAll = (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Update);
            this.selectable.columnSettings.reorderable = true;
            this.selectable.columnSettings.includeInChooser = false;
        }

        this.groups.groupable.enabled = false;

        this.onCellClose = (event: CellCloseEvent) => {
            if (event.column.field === "Quantita") {
                this.totaleFarmacoDaSomministrare = 0;
                this.gridpublicService.getValue().data.rows
                    .filter((row: any) => row.Selected)
                    .forEach(item =>
                        this.totaleFarmacoDaSomministrare += item['Quantita']
                    );
                this.totaleFarmacoDaSomministrare = Math.ceil(this.totaleFarmacoDaSomministrare); // approssima per eccesso
            }
        };

        // verifica che mi sia stato passato il proprietario
        let risorsa: RisorsaPersona = this.trattamentoFormService.attivitaDaChiamante?.risorse.find(item => item.classType == 'RisorsaPersona' && item?.risorsaUmana?.contatto?.tipo === enum_Tipo_RisorsaUmana.Proprietario) ?? null;
        this.CFproprietario = risorsa?.risorsaUmana?.contatto?.codiceFiscale ?? "";

        effect(() => {
            //effect per gestire la signal changeDrugSelected
            const value = this.trattamentoFormService.changeDrugSelected();
            this.calculateNumericFormat(value.udm);
            if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Update)
                this.deselezionaCapiAnimali();
        });
    }

    public selectionChangeFn = (event: SelectionEvent, component: GiasKendoGridComponent) => {
        const selectedRows = event.selectedRows;
        const deselectedRows = event.deselectedRows;

        selectedRows.forEach(row => {
            // let arrotondamentoPeso = this.trattamentoFormService.dettagliProtocollo.arrotondamentoPeso;
            row.dataItem['selezionato'] = 'S';
            if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Update
                || (this.trattamentoFormService.isFirstSomm && !this.trattamentoFormService.hasSuccessiveConfermate)) {
                row.dataItem['Quantita'] = this.calcolaDoseDaSomministrare(
                    this.getPesoStimatoOrArrotondato(row.dataItem['Incremento_Teorico_Calcolato']), // (arrotondamentoPeso !== 0) ? (Math.ceil(row.dataItem['Incremento_Teorico_Calcolato'] / arrotondamentoPeso) * arrotondamentoPeso) : row.dataItem['Incremento_Teorico_Calcolato'],
                    this.trattamentoFormService.dettagliProtocollo.qtaDose,
                    this.trattamentoFormService.dettagliProtocollo.udmDose
                );
            } else {
                const capoAnimale = this.trattamentoFormService.attivitaDaChiamante?.centriDiCosto.
                    filter(itemCentro => itemCentro.classType == 'CapoAnimaleCDC')?.
                    find(itemCentro => itemCentro.capoAnimale.codice == row.dataItem.Cod_Animale);
                row.dataItem['Quantita'] = capoAnimale?.qtaSomministrata ?? 0;
            }

            if (this.trattamentoFormService.arrayFarmCat.some(item => item == enum_FarmacoCategoria.Antibiotici)
                && row.dataItem['OnAntibiotico'])
                this.giasMessageService.warningMessage(this.transloco.translate('zoo.CapoAnimaleOnAntibiotico', [row.dataItem['Matricola']]));

            if (this.trattamentoFormService.arrayFarmCat.some(item => item == enum_FarmacoCategoria.Antinfiammatori)
                && row.dataItem['OnAntinfiammatorio'])
                this.giasMessageService.warningMessage(this.transloco.translate('zoo.CapoAnimaleOnAntifiammatori', [row.dataItem['Matricola']]));

        });

        deselectedRows.forEach(row => {row.dataItem['Quantita'] = 0; row.dataItem['selezionato'] = 'N';});

        // ricalcolo della quantità totale da somministrare
        if (!this.trattamentoFormService.dettagliProtocollo.massivo) {
            this.totaleFarmacoDaSomministrare = 0;
            component.rows.forEach(item => {
                if (item['Selected']) this.totaleFarmacoDaSomministrare += item['Quantita'];
            });
            // approssima per eccesso
            this.totaleFarmacoDaSomministrare = Math.ceil(this.totaleFarmacoDaSomministrare); 
        } else {
            let numCapiAnimali = component.rows.filter((row: any) => row.Selected).length;
            component.rows.forEach(item => {
                if (item['Selected']) item['Quantita'] = this.totaleFarmacoDaSomministrare/numCapiAnimali;
            });
        }
    }

    calculateNumericFormat(UnitaDiMisura: string | number): void {
        if (UnitaDiMisura == null || UnitaDiMisura == undefined) return;

        let udm: string = '';
        this.numericFormat = '#0.00';

        if (typeof UnitaDiMisura === 'number') {
            switch(UnitaDiMisura) {
                //case enum_UdM_Dose.mg_su_Kg:
                case enum_UnitaMisura.Milligrammi:
                    udm = 'mg';
                    break;
                case enum_UdM_Dose.ml_su_100Kg:
                case enum_UdM_Dose.ml_su_Capo:
                case enum_UnitaMisura.Millilitri:
                    udm = 'ml';
                    break;
                case enum_UdM_Dose.g_su_100Kg:
                    udm = 'g';
                    break;
                default:
                    udm = '';
                    break;
            }
        } else {
            udm = UnitaDiMisura;
        }

        this.numericFormat += ' ' + udm;
    }

    private getPesoStimatoOrArrotondato(pesoTeoricoCalcolato: any): number {
        let arrotondamentoPeso = this.trattamentoFormService.dettagliProtocollo.arrotondamentoPeso;
        if (arrotondamentoPeso !== 0) 
            return Math.ceil(pesoTeoricoCalcolato / arrotondamentoPeso) * arrotondamentoPeso;
        else
            return pesoTeoricoCalcolato;
    }

    read(options?: any): Observable<GridCapiAnimaliServerResult> { 
        this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

        if (this.trattamentoFormService.isInfoMode) {
            this.totaleFarmacoDaSomministrare = 0;

            //se sono in modalità info, non faccio la chiamata al servizio, ma prendo i dati già passati
            let capiAnimaliInfo: Array<KendoGridRow> = [];

            this.trattamentoFormService.attivitaDaChiamante.centriDiCosto
                .filter(itemCentro => itemCentro.classType == 'CapoAnimaleCDC')
                .forEach(itemCentro => {
                    let itemCapoAnimale = new CapoAnimale(
                        itemCentro.capoAnimale.codice,
                        'S',
                        itemCentro.capoAnimale.matricola,
                        itemCentro.sottogruppoStalla_ingresso.nome,
                        this.trattamentoFormService.formTrattamentoZoo.get('Stalla').value,
                        '',
                        itemCentro.capoAnimale.razza.descrizione,
                        itemCentro.capoAnimale.sesso,
                        itemCentro.capoAnimale.statiAccrescimento[0].descrizione,
                        itemCentro.capoAnimale.validita.inizio,
                        itemCentro.capoAnimale.validita.fine,
                        itemCentro.capoAnimale.pesoStimato,
                        this.getPesoStimatoOrArrotondato(itemCentro.capoAnimale.pesoStimato), //Math.ceil(itemCentro.capoAnimale.pesoStimato / this.trattamentoFormService.dettagliProtocollo.arrotondamentoPeso) * this.trattamentoFormService.dettagliProtocollo.arrotondamentoPeso,
                        itemCentro.qtaSomministrata
                    );
                    capiAnimaliInfo.push(itemCapoAnimale);

                    this.totaleFarmacoDaSomministrare += itemCentro.qtaSomministrata;
                });

            this.totaleFarmacoDaSomministrare = Math.ceil(this.totaleFarmacoDaSomministrare * 100) / 100;

            this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

            return of(new GridCapiAnimaliServerResult(
                capiAnimaliInfo,
                this.capiAnimaliConfigService.kendoColumnsCapiAnimali,
                this.capiAnimaliConfigService.kendoModelCapiAnimali
            ));
        } else {
            return this.getObservableForRows()
                .pipe(
                    catchError(() => {
                        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
                        return of();
                    }
                ),
                    map((r:any) => {
                        this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
                        this.totaleFarmacoDaSomministrare = 0;

                        let gridCapiAnimaliServerResult = new GridCapiAnimaliServerResult(
                            this.setRowGrid(r),
                            this.capiAnimaliConfigService.kendoColumnsCapiAnimali,
                            this.capiAnimaliConfigService.kendoModelCapiAnimali
                        );

                        return gridCapiAnimaliServerResult;
                    })
                );
        }
    }

    private setRowGrid(responseRows): Array<KendoGridRow> {
        let rows: Array<KendoGridRow> = [];
        let indexCapiAnimali = 0;

        const centriDiCosto = this.trattamentoFormService.attivitaDaChiamante?.centriDiCosto
            .filter(item => item.classType === 'CapoAnimaleCDC') || [];

        const isFixedQta = this.trattamentoFormService.checkIsVeterinaryOrIndTerapeuticPrescription();
        const fixedQtaTot = this.trattamentoFormService.dettagliProtocollo.quantitaTotaleReale ?? 0;

        // Filtra le righe se ci sono dati nei centri di costo
        const filteredRows = (centriDiCosto.length > 0 && (!this.trattamentoFormService.isFirstSomm || this.trattamentoFormService.hasSuccessiveConfermate))
            ? responseRows.filter(itemOfRows =>
                centriDiCosto.some(itemCentro => itemCentro.capoAnimale.codice === itemOfRows.Cod_Animale))
            : responseRows;

        let numSelectedCapi: number = centriDiCosto.length;
        let fixedQuantityPerCapo: number = isFixedQta && numSelectedCapi > 0 ? fixedQtaTot / numSelectedCapi : 0;

        for (let itemOfRows of filteredRows) {
            indexCapiAnimali++;
            itemOfRows.idCapiAnimali = indexCapiAnimali;

            const capoAnimale = this.trattamentoFormService.attivitaDaChiamante?.centriDiCosto.
                filter(itemCentro => itemCentro.classType == 'CapoAnimaleCDC')?.
                find(itemCentro => itemCentro.capoAnimale.codice == itemOfRows.Cod_Animale);
            let peso: number = 0;

            // if (this.trattamentoFormService.dettagliProtocollo.arrotondamentoPeso !== 0) 
            //     peso = Math.ceil(itemOfRows['Incremento_Teorico_Calcolato'] / this.trattamentoFormService.dettagliProtocollo.arrotondamentoPeso) * this.trattamentoFormService.dettagliProtocollo.arrotondamentoPeso;
            // else
            //     peso = itemOfRows['Incremento_Teorico_Calcolato'];

            peso = this.getPesoStimatoOrArrotondato(itemOfRows['Incremento_Teorico_Calcolato']);

            itemOfRows['Peso_Arrotondato'] = peso;

            //se entro in modifica, sicuramente ci sono dei capi selezionati
            itemOfRows.Selected = (capoAnimale)? true : false;
            itemOfRows.selezionato = (capoAnimale)? 'S' : 'N';
            if (isFixedQta && itemOfRows.Selected) {
                itemOfRows.Quantita = fixedQuantityPerCapo;
            } else {
                itemOfRows.Quantita = itemOfRows.Selected
                    ? this.getDosePerCapoAnimale(filteredRows.length, capoAnimale?.qtaSomministrata ?? 0, peso)
                    : 0;
            }
            this.totaleFarmacoDaSomministrare += itemOfRows.Quantita;

            rows.push(itemOfRows);
        }

        this.totaleFarmacoDaSomministrare = Math.ceil(this.totaleFarmacoDaSomministrare * 100) / 100;

        rows.sort((a, b) => (b['Selected'] ? 1 : 0) - (a['Selected'] ? 1 : 0));

        return rows;
    }

    /* * Metodo per calcolare la dose per capo animale
     * * @description
     * * Calcola la dose per capo animale, considerando se è in modalità massiva o meno.
     * * Se è in modalità massiva, la dose è calcolata come quantità totale divisa per il numero di capi animali selezionati.
     * * Se non è in modalità massiva, la dose è calcolata come quantità calcolata in base al dosaggio e al peso dell'animale, a meno che
     * * non sia stata specificata a mano dall'utente.
     * @returns {number} Dose per capo animale
    */
    private getDosePerCapoAnimale(numCapiAnimali: number, qtaSomministrata: number, peso: number): number {
        if (this.trattamentoFormService.typeTab_Zootecnia == enum_TypeTab_Zootecnia.FuturePrescriptions
            && this.trattamentoFormService.dettagliProtocollo.massivo) {
            // Se siamo nel caso di prescrizione massiva e siamo nel caso di una somministrazione futura,
            // la dose per capo animale è calcolata come quantità totale divisa per il numero di capi animali selezionati
            return this.trattamentoFormService.dettagliProtocollo.quantitaTotaleReale / numCapiAnimali;
        } else {
            if (qtaSomministrata > 0)
                //Se la quantità è stata specificata a mano dall'utente, la dose per capo animale è quella
                return qtaSomministrata;
            else
                //Altrimenti, la dose per capo animale è calcolata in base al dosaggio e al peso dell'animale
                return this.calcolaDoseDaSomministrare(
                    peso,
                    this.trattamentoFormService.dettagliProtocollo.qtaDose,
                    this.trattamentoFormService.dettagliProtocollo.udmDose
                );
        }
    }

    private getObservableForRows(): Observable<RispostaStandard> {
        let obs: Observable<RispostaStandard>;
        let param: LeggiGiacenzeZooDto | LeggiGiacenzeZooFirstSommDto;
        let formValues = this.trattamentoFormService.formTrattamentoZoo.getRawValue();

        if (this.trattamentoFormService.typeTab_Zootecnia == enum_TypeTab_Zootecnia.Prescriptions) {
            let DataGiacenza:Date = formValues?.Data;
            DataGiacenza.setHours(23,59,59,999); // imposto alla fine della giornata
            param = {
                Id_Pres: this.trattamentoFormService.dettagliProtocollo.idProtocol,
                Piva: this.trattamentoFormService.objParametriAgenda.Piva,
                CodCentro: formValues?.CentroAziendale?.codice ?? 0,
                CodStalla: formValues?.Stalla?.STA_NUM ?? 0,
                CodRaggruppamento: formValues?.Raggruppamento?.codice ?? 0,
                CodAnimale: 0,
                Matricola: '',
                DataGiacenza: DataGiacenza,
                Istantanea: true,
                MostraPesate: true,
                FiltraFornitori: true,
                ListaCodAnimali: [],
                /** Mostra data primo giorno caricamento e giorni in stalla da quella data */
                MostraGGPrimoCaricamento: false,
                CFproprietario: this.CFproprietario
            };

            obs = this.zooService.operazioniZooGetGiacenzeZooFirstSomm(param)
                .pipe(
                    map((resp) => {
                        let r;
                        if (resp.RispostaOK)
                            r = JSON.parse(resp.RispostaStringa);
                        else
                            r = [];
                        (r as any[]).forEach(element => {
                            element.selezionato = 'N';
                        });
                        return r;
                    }),
                    switchMap((r) => of(this.conversionService.ConversionDateInObject(r)))
                );
        } else {
            let DataGiacenza:Date = formValues?.Data;
            DataGiacenza.setHours(23,59,59,999); // imposto alla fine della giornata
            param = {
                Piva: this.trattamentoFormService.objParametriAgenda.Piva,
                CodCentro: formValues?.CentroAziendale?.codice ?? 0,
                CodStalla: formValues?.Stalla?.STA_NUM ?? 0,
                CodRaggruppamento: formValues?.Raggruppamento?.codice ?? 0,
                CodAnimale: 0,
                Matricola: '',
                DataGiacenza: DataGiacenza,
                Istantanea: true,
                MostraPesate: true,
                FiltraFornitori: true,
                ListaCodAnimali: [],
                /** Mostra data primo giorno caricamento e giorni in stalla da quella data */
                MostraGGPrimoCaricamento: false,
                CFproprietario: this.CFproprietario
            };

            obs = this.zooService.operazioniZooGetGiacenzeZoo(param)
                .pipe(
                    map((resp) => {
                        let r;
                        if (resp.RispostaOK)
                            r = JSON.parse(resp.RispostaStringa);
                        else
                            r = [];
                        (r as any[]).forEach(element => {
                            element.selezionato = 'N';
                        });
                        return r;
                    }),
                    switchMap((r) => of(this.conversionService.ConversionDateInObject(r)))
                );
        }

        return obs;
    }

    private calcolaDoseDaSomministrare(pesoAnimale: number, dose: number, udmDose: enum_UdM_Dose ): number {

        let doseDaSomministrare: number = 0;
        switch (udmDose) {
            // case enum_UdM_Dose.mg_su_Kg:
            //     doseDaSomministrare = pesoAnimale * dose;
            //     break;
            case enum_UdM_Dose.ml_su_100Kg:
                doseDaSomministrare = pesoAnimale * dose / 100;
                if (!this.trattamentoFormService.dettagliProtocollo.massivo)
                    doseDaSomministrare = Math.ceil(doseDaSomministrare);
                break;
            case enum_UdM_Dose.ml_su_Capo:
                doseDaSomministrare = dose;
                break;
            case enum_UdM_Dose.n_su_Capo:
                doseDaSomministrare = dose;
                break;
            case enum_UdM_Dose.g_su_100Kg:
                doseDaSomministrare = pesoAnimale * dose / 100;
                if (!this.trattamentoFormService.dettagliProtocollo.massivo)
                    doseDaSomministrare = Math.ceil(doseDaSomministrare);
                break;
            default:
                doseDaSomministrare = 0;
        }

        return doseDaSomministrare;
    }

    private deselezionaCapiAnimali() {
        let refValue = this.gridpublicService.getValue()?.data ?? null;
        this.totaleFarmacoDaSomministrare = 0;
        if (refValue) {
            refValue.rows
                .filter(item => item['Selected'])
                .forEach(item => {
                    item['Selected'] = false;
                    item['Quantita'] = 0;
                    item['selezionato'] = 'N';
                });
        }
    }

    modificaTotEDistribuisci() {
        let qtaDaDistrubuire = this.totaleFarmacoDaSomministrare / this.gridpublicService.getValue().data.rows
            .filter((row: any) => row.Selected).length;
        this.gridpublicService.getValue().data.rows
            .filter(item => item['Selected'])
            .forEach(item => item['Quantita'] = qtaDaDistrubuire);
        this.isTotEditable = false;
        this.giasMessageService.infoMessagge('zoo.ModificaTotaleDistribuzioneCapiAnimali', false, true);
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
        throw new Error("Method not implemented.");
    }

}