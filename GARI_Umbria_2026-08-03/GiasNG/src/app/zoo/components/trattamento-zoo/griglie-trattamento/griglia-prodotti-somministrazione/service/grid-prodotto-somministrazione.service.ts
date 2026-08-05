import { Injectable, Injector, OnDestroy } from "@angular/core";
import { SelectionEvent } from "@progress/kendo-angular-grid";
import { AbstractGridConfigService, CommandsColumnSettings, CommandsDropDownSettings, ConfigTemplate, EditingMode, GiasKendoGridComponent, GridPublicService, HttpAction, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, SelectableSettings } from "gias-kendo-grid";
import { Observable, catchError, map, of, Subject, takeUntil } from "rxjs";
import { ProdottoSomministrazioneConfigService } from "./prodotto-somministrazione-config.service";
import { LeggiGiacenzaFarmaci, OperazioniZooClient } from "app/Service/net-core6-api.service";
import { TrattamentoZooFormService } from "../../../service/trattamento-zoo-form.service";
import { GiasDialogService } from "gias-ui-kit";
import { enum_TipoPrescrizione } from "app/zoo/models/tipo-prescrizione.enum";
import { enum_UnitaMisura } from "app/Model/TipiEnumerativi";
import { enum_UdM_Dose } from "app/zoo/models/tipi-enumerativi-zoo";

class Farmaco {
    Chiave: number;
    Codice: number;
    Descrizione: string;
    CodiceAIC: string;
    Lotto: string;
    UdM: string;
    Qta: number;
    QtaTot: number;

    constructor(chiave: number, codice: number, descrizione: string, codiceAIC: string, lotto: string, udm: string, qta: number, qtaTot: number) {
        this.Chiave = chiave;
        this.Codice = codice;
        this.Descrizione = descrizione;
        this.CodiceAIC = codiceAIC;
        this.Lotto = lotto;
        this.UdM = udm;
        this.Qta = qta;
        this.QtaTot = qtaTot;
    }
}

export class GridProdottiSomministrazioneServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class GridProdottiSomministrazioneService extends AbstractGridConfigService<GridProdottiSomministrazioneServerResult> implements OnDestroy {
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;

    rowId: string = 'Chiave';
    gridId: string = 'ProdottiDellaSomministrazioneId';

    signal: Subject<void> = new Subject();

    proCod: number = 0;
    codiceAIC: string[] = [];

    // lottiPerProdotto = new Map<string, Set<string>>();

    constructor(injector: Injector,
                public gridpublicService: GridPublicService,
                private trattamentoFormService: TrattamentoZooFormService,
                private zooService: OperazioniZooClient,
                private prodottoConfigService: ProdottoSomministrazioneConfigService,
                private giasDialogService: GiasDialogService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

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

        if (!this.trattamentoFormService.isInfoMode) {
            this.selectable.selectable = new SelectableSettings({
                checkboxOnly: true,
                enabled: true /*,
                mode: 'single'*/
            });

            this.selectable.preselectedRows.selectionChangeFn = this.selectionChangeFn;
            this.selectable.columnSettings.showSelectAll = false;
            this.selectable.shouldShowCheckbox = true;
            this.selectable.columnSettings.title = ' ';
        }

        this.groups.groupable.enabled = false;

        this.trattamentoFormService.ddlSelectionSubject.pipe(takeUntil(this.signal)).subscribe(value => {
            this.gridpublicService.refresh(true);
        });

        this.trattamentoFormService.attivitaDaChiamante.risorse.forEach(itemRis =>  {
            if (itemRis.classType == 'DettaglioRegistroSomministrazioni') {
                // this.proCod = itemRis.prodotto.codice;
                this.codiceAIC.push(itemRis['codiceAIC']);
            }
        });
    }

    public selectionChangeFn = (event: SelectionEvent, component: GiasKendoGridComponent) => {
        this.trattamentoFormService.withDiffAICs = new Set(component.rows.filter(row => (row as any).Selected).map(row => (row as any).CodiceAIC)).size > 1;

        const selectedRows = event.selectedRows;
        if (selectedRows.length > 0) {
            selectedRows.forEach(item => {
                const dataItem = item.dataItem as Farmaco;

                // if (dataItem.QtaTot < dataItem.Qta) {
                //     this.giasDialogService.dialogMessageObs_Result(
                //             this.transloco.translate('Attenzione'),
                //             this.transloco.translate('zoo.GiacenzaDataSalvataggioInferiore')
                //         ).pipe(
                //             map(result => !!result['returnObj'])
                //         )
                //         .subscribe(result => {
                //             if (!result) item['Selected'] = false;
                //     });
                // }
                const selectedCodice = dataItem.Codice;
                const selectedUdm = dataItem.UdM;
            });
        }
    };

    read(options?: any): Observable<GridProdottiSomministrazioneServerResult> {
        this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

        let farmaciInfo: Array<KendoGridRow> = [];

        if (this.trattamentoFormService.isInfoOrUpdateMode) {
            // se sono in questo caso, ho le info sui farmaci passati dal chiamante

            let index = 0;

            this.trattamentoFormService.attivitaDaChiamante.risorse
            .filter(item => item.classType == 'DettaglioRegistroSomministrazioni')
            .forEach(itemRis => {
                index++;
                let risorsaProdotto = this.trattamentoFormService.attivitaDaChiamante.risorse
                                    .filter(item => item.classType == 'RisorsaProdotto');

                let prodottoSelezionato = risorsaProdotto.filter((el) => { return (el.prodotto.elemCod = itemRis.prodotto.elemCod) && (itemRis.prodotto.codice == el.prodotto.codice) });

                let farmaco = new Farmaco(index, itemRis.prodotto.codice, itemRis.prodotto.descrizione, itemRis.codiceAIC, '', itemRis.unitaDiMisura.simbolo, itemRis.quantitaTotaleReale, itemRis.QtaTot);
                let magazzino, Piva, sa_cod, Fabbricato_Cod, Fabbricato_Des;

                if (prodottoSelezionato.length > 0){
                    prodottoSelezionato.forEach(element => {
                       if (element.MagazziniMovimentazioni != null && element.MagazziniMovimentazioni.length > 0){
                            magazzino = element.MagazziniMovimentazioni[0]?.Magazzino;
                            Piva = magazzino?.primaryKey?.centroAziendalePK?.partitaIva;
                            sa_cod = magazzino?.primaryKey?.centroAziendalePK?.codice;
                            Fabbricato_Cod = magazzino?.primaryKey?.codice;
                            Fabbricato_Des = magazzino?.descrizione;
                            farmaco.Lotto = element.MagazziniMovimentazioni[0].Lotto;
                        }
                        
                        farmaco.Qta = element.quantitaTotaleReale;
                        farmaco.UdM = element.unitaDiMisura.simbolo;

                        console.log(magazzino);
                        farmaciInfo.push(
                            { ...farmaco, 
                                Piva:Piva, 
                                Sa_Cod: sa_cod,
                                Fabbricato_Cod: Fabbricato_Cod,
                                Fabbricato_Des: Fabbricato_Des,
                                DataScadenza: element?.MagazziniMovimentazioni?.[0]?.DataScadenza
                                    ?? element?.MagazziniMovimentazioni?.[0]?.Data_Scadenza
                                    ?? element?.MagazziniMovimentazioni?.[0]?.dataScadenza
                                    ?? null,
                                Selected: true
                            }
                        ); 
                    });
                }
            });

            if (this.trattamentoFormService.isInfoMode) {

                this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

                return of(new GridProdottiSomministrazioneServerResult(
                    farmaciInfo,
                    this.prodottoConfigService.kendoColumnsProdottoSomministrazione,
                    this.prodottoConfigService.kendoModelProdottoSomministrazione
                ));
            }

        }
        
        if (!this.trattamentoFormService.isInfoMode) {

            // return this.ajaxAgronicaAPIService.ajaxAPIPost('Anagrafica/LeggiGiacenzaFarmaci', param).pipe(catchError((err) => {
            return this.zooService.operazioniZooLeggiGiacenzaFarmaci(this.getParam()).pipe(catchError((err) => {
                this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
                return of();
            }), map((r:any) => {

                this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

                let respobj = JSON.parse(r.RispostaStringa);

                //array che mi servirà in fase di salvataggio
                this.trattamentoFormService.arrayDettagliProdottiSomministrazione = [...respobj, 
                    ...this.trattamentoFormService.attivitaDaChiamante.risorse.filter(item => item.classType == 'DettaglioRegistroSomministrazioni')
                                                                              .map(itemRis => { 
                                                                                                    let risorsaProdotto = this.trattamentoFormService.attivitaDaChiamante.risorse
                                                                                                                            .filter(item => item.classType == 'RisorsaProdotto')
                                                                                                                            .filter((el) => { return (el.prodotto.elemCod = itemRis.prodotto.elemCod) && (itemRis.prodotto.codice == el.prodotto.codice) });
                                                                                                                            
                                                                                                    if (risorsaProdotto.length > 0) {
                                                                                                        itemRis.MagazziniMovimentazioni = risorsaProdotto[0]?.MagazziniMovimentazioni ?? null;
                                                                                                        itemRis.MagazziniMovimentazioni[0].udm = itemRis.unitaDiMisura;
                                                                                                    }
                                                                                                    return itemRis;
                                                                              })];

                let gridProdottiSomministrazioneServerResult = new GridProdottiSomministrazioneServerResult(
                    this.setRowGrid(respobj, farmaciInfo),
                    this.prodottoConfigService.kendoColumnsProdottoSomministrazione,
                    this.prodottoConfigService.kendoModelProdottoSomministrazione
                );

                return gridProdottiSomministrazioneServerResult;
            }));
        }

    }

    private getParam(): LeggiGiacenzaFarmaci {

        let formValues = this.trattamentoFormService.formTrattamentoZoo.getRawValue();
        let codiceFiscaleProprietarioCapi;

        let proprietario = this.trattamentoFormService?.attivitaDaChiamante?.risorse?.filter((r) => { 
            let risorsaUmana = r.classType == "RisorsaPersona" ;
            let prop = r.risorsaUmana?.contatto?.tipo == "proprietario";
            return risorsaUmana && prop;
        })

        if (proprietario.length > 0) {
            codiceFiscaleProprietarioCapi = proprietario[0].risorsaUmana?.contatto?.codiceFiscale
        }

        let udmCod: number = -1;

        switch (this.trattamentoFormService.dettagliProtocollo?.udmDose) {
            case enum_UdM_Dose.ml_su_100Kg:
            case enum_UdM_Dose.ml_su_Capo:
                udmCod = enum_UnitaMisura.Millilitri;
            break;
            case enum_UdM_Dose.g_su_100Kg:
                udmCod = enum_UnitaMisura.Grammi;
            break;
            case enum_UdM_Dose.n_su_Capo:
                udmCod = enum_UnitaMisura.Numero;
            break;
            default:
                udmCod = enum_UdM_Dose.undefined;
            break;
        }

        return {
            Piva: this.trattamentoFormService.objParametriAgenda.Piva,
            SaCod: formValues?.CentroAziendale?.codice ?? 0,
            codiceBDN: formValues?.Stalla?.BDN_Codice_Azienda ?? '',
            UdmCod: udmCod,
            codFiscaleProprietario: codiceFiscaleProprietarioCapi ?? '',
            ProCod: this.proCod,
            codiceAIC: this.codiceAIC.map(item => item.toString().substring(0, 6)),
            ValiditaFine: formValues?.Data
        } as LeggiGiacenzaFarmaci;
    }

    private setRowGrid(responseRows, farmaciInfo: Array<KendoGridRow>): Array<KendoGridRow> {
        let rows: Array<KendoGridRow> = [];
        let indexProdottiSomministrazione = 0;

        for (let itemOfRows of responseRows) {
            for (let movMagazzino of itemOfRows.MagazziniMovimentazioni) {
                indexProdottiSomministrazione++;
                let item = new Farmaco(indexProdottiSomministrazione, itemOfRows.prodotto.codice, itemOfRows.prodotto.descrizione, itemOfRows.codiceAIC, movMagazzino.Lotto, itemOfRows.unitaDiMisura.simbolo, movMagazzino.Qta, movMagazzino.QtaTot);

                let magazzino = movMagazzino.Magazzino;
                let Piva = magazzino.primaryKey.centroAziendalePK.partitaIva;
                let sa_cod = magazzino.primaryKey.centroAziendalePK.codice;
                let Fabbricato_Cod = magazzino.primaryKey.codice;
                let Fabbricato_Des = magazzino.descrizione;

                let farmItemInfo = farmaciInfo.find(farmaco => farmaco['Codice'] === item.Codice && farmaco['Lotto'] === item.Lotto);

                if (farmItemInfo) {
                    // Add back the quantity already discharged by this treatment so that
                    // both the selectable stock (Qta) and the long-term stock (QtaTot)
                    // reflect the real available amount when editing the treatment.
                    item.Qta += farmItemInfo['Qta'];
                    item.QtaTot += farmItemInfo['Qta'];
                    item['Selected'] = true;
                }

                if (this.checkIfPushDrug(item, farmaciInfo))
                    rows.push(
                        { ...item, 
                            Piva:Piva, 
                            Sa_Cod: sa_cod,
                            Fabbricato_Cod: Fabbricato_Cod,
                            Fabbricato_Des: Fabbricato_Des,
                            DataScadenza: movMagazzino?.DataScadenza
                                ?? movMagazzino?.Data_Scadenza
                                ?? movMagazzino?.dataScadenza
                                ?? null
                        }
                    );
            }
        }

        // Add drugs from the existing treatment that have no current stock in the warehouse
        // (e.g. because fQtaGreaterZero filtered them out). Set QtaTot = Qta so they are
        // not incorrectly flagged as risky and pass the stock alert when in Write mode.
        rows.push(...farmaciInfo
            .filter(farmaco => !rows.some(row => row['Codice'] === farmaco['Codice'] && row['Lotto'] === farmaco['Lotto']))
            .map(farmaco => ({ ...farmaco, QtaTot: farmaco['QtaTot'] ?? farmaco['Qta'] }))
        );

        rows.sort((a, b) => {
            if (b['Selected'] !== a['Selected']) return b['Selected'] ? 1 : -1;

            const aIsRisky = a['QtaTot'] < a['Qta'];
            const bIsRisky = b['QtaTot'] < b['Qta'];

            if (aIsRisky !== bIsRisky) return aIsRisky ? 1 : -1;

            return a['Lotto'].localeCompare(b['Lotto']);
        });

        if (rows.length > 0) {
            const alreadySelected = rows.some(r => r['Selected']);

            if (!alreadySelected) {
                // Select the first row.
                // Thanks to sorting, this is the "safest" row available.
                const firstRow = rows[0];
                firstRow['Selected'] = true;
            }
            // if (!rows[0]['Selected']) {
            //     rows[0]['Selected'] = true; // Se non c'è nessun elemento selezionato, seleziona il primo elemento
            //     // this.trattamentoFormService.signalChangeDrugSelected(rows[0]['Codice'], rows[0]['UdM']);
            // }
        } else {
            if (this.trattamentoFormService.isInfoOrUpdateMode) {
                let message = 'zoo.GiacenzaNonTrovataModificaNonPossibile';
                this.giasDialogService.alertMessage(this.transloco.translate((message))).subscribe(value => {
                    if (value['returnObj']) {
                        this.trattamentoFormService.goToGridPage();
                    }
                });
            }
        }

        return rows;
    }

    private checkIfPushDrug(item: any, farmaciInfo: Array<KendoGridRow>): boolean {
        // Controlla se il farmaco è già presente nella lista
        if (!this.trattamentoFormService.checkIfIsEditingFromVeterinaryPrescripton())
            return true;
        else
            //nel caso di prescrizione per indicazione terapeutica, posso solo selezionare farmaci con lotto diverso
            // ma stesso codice, per cui li aggiungo alla grid solo in questi casi
            if (farmaciInfo.length > 0) {
                return (farmaciInfo.findIndex(farm => farm['Codice'] === item.Codice) > -1);
            } else 
                return true; // Se il farmaco non è presente, lo aggiunge alla lista
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }
    
    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
        throw new Error("Method not implemented.");
    }

}