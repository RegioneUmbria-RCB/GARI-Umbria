import { Inject, Injectable, LOCALE_ID } from "@angular/core";
import { QdCService } from "./qdc.service";
import {
  GridImpiantoSelezionatoModel, Key_Parametri_Aggiuntivi_Attivita, Obj_Errore_Gias_QdC,
  Parametri_Aggiuntivi_Attivita,
  Sezione_Prodotto_Fertilizzanti,
  Sezione_Prodotto_Formulati,
  Sezione_Prodotto_Raccolta,
  Sezione_Prodotto_Sementi, Sezione_Rilievi,
  Testata
} from "../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import { FERTILIZZANTI, FORMULATI, INSETTI, SEMENTI, TRASFORMATI_VEGETALI } from '../../../Model/CostantiPersonalizzate';
import { enum_LAVCOD, enum_SEMINA_TIPO } from "../../../Model/TipiEnumerativi";
import { enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity } from "../../../Service/master.service";
import { TranslocoService } from "@jsverse/transloco";
import { Lavorazione } from "../../../Model/attivita/Lavorazione";
import { DettaglioRaccolta } from "app/Model/attivita/dettagli/DettaglioRaccolta";
import { DestinazioneUso } from "../../../Model/metaschema/utilizzi/DestinazioneUso";
import { Specie } from "../../../Model/metaschema/utilizzi/Specie";
import { QdCTestataService } from "./testata/testata.service";
import { Tipo_Ricetta } from "../../../Model/attivita/Attivita";
import {GiasKendoGridComponent, KendoGridRow} from 'gias-kendo-grid';
import { Tipo } from "app/Model/attivita/centri_di_costo/CentroDiCosto";
import { Tipo_Attivita, Stati } from 'gias-ui-kit';
import { DatePipe } from "@angular/common";
import { IrrigazioneAcquaService } from "./irrigazione-acqua.service";
import {Attivita_Con_Parametri_Aggiuntivi} from "../../../Service/Agenda/Agenda.service";
import {RibaltamentoTypes} from "../../../menu-agenda/components/utils";

@Injectable()

export class QdCControlliSalvataggioService {

    list_Warning_QdC: Parametri_Aggiuntivi_Attivita[] = [{
        key: Key_Parametri_Aggiuntivi_Attivita.mostraWarning_InviaRicettaAdAPP,
        value: "true",
        operazione: null
    },
    {
        key: Key_Parametri_Aggiuntivi_Attivita.mostraWarning_Dichiarazione_non_Utilizzo,
        value: "true",
        operazione: null
    }];

    constructor(private qdcservice: QdCService,
        private translocoService: TranslocoService,
        private testataService: QdCTestataService,
        private datePipe: DatePipe,
        @Inject(LOCALE_ID) private locale_id: string,
        private irrigazioneAcquaService: IrrigazioneAcquaService
    ) {
    }

    public ControllaQdC(): Promise<ErroreGias[]> {
        return new Promise<ErroreGias[]>((resolve, reject) => {
            let listerroriGias: ErroreGias[] = [];
            this.GestisciWarning(listerroriGias);

            let Operazioni = this.qdcservice.TestataForm.get("Operazioni").getRawValue() as Array<Lavorazione>;
            let flagVisita = this.qdcservice.TestataForm.get("flagVisita").value;
            let attivitaPersonalizzata = this.qdcservice.TestataForm.get("Attivita_Personalizzata").value;
            let specieSelezionata = this.qdcservice.TestataForm.get("Specie").value;

            if (listerroriGias.length === 0) {
                if (!flagVisita && (!Operazioni || Operazioni.length === 0)) {
                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("SelezionareAlmenoUnOperazione")
                    });
                }
            }

            //aggiunta condizione per la quale se siamo una visita e non è stata scelta alcuna specie, allora alert
            // if(listerroriGias.length === 0) {
            //     if (flagVisita && ((specieSelezionata.codice === 0) || (specieSelezionata.codice === -1))) {
            //             listerroriGias.push(<ErroreGias>{
            //                 severity: ErroreGias_Severity.Bloccante,
            //                 tipo:  enum_ErroreGias_Tipo.Generico,
            //                 messaggio: this.translocoService.translate("SelezionareUnaSpecie")
            //             });
            //     }
            // }

            //aggiunto controllo sull'inserimento di un numero razionale nelle Ore Totali
            if (listerroriGias.length === 0) {
                if (flagVisita && this.qdcservice.TestataVisitaForm.get("impOrarioFine_Visita").value === 0 && !isNumeroPositivoValido(this.qdcservice.TestataVisitaForm.get("NrOreTotali_Visita").value)) {
                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("InserireOreTotaliValido")
                    });
                }
            }

            if (listerroriGias.length === 0) {
                if (flagVisita && this.qdcservice.TestataVisitaForm.get("Operatore_Visita").value == null) {
                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("SelezionareOperatoreVisita")
                    });
                }
            }

            if (listerroriGias.length === 0) {
                if (flagVisita && this.qdcservice.TestataVisitaForm.get("Azienda_Visita").value == null) {
                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("SelezionareAziendaVisita")
                    });
                }
            }

            //aggiunta condizione per la quale se siamo una visita ed è stato scelta "Altre operazioni", deve essere necessariamente specificata un'attività
            if (listerroriGias.length === 0) {
                if (flagVisita && (Operazioni.findIndex((element) => element.primaryKey.codice === enum_LAVCOD.VISITA.toString()) >= 0)) {
                    if (!attivitaPersonalizzata) {
                        listerroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.Bloccante,
                            tipo: enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate("SelezionareAlmenoUnAttivita")
                        });
                    }
                }
            }

            //Controllo Grid Operatori righe non salvate
            if (listerroriGias.length === 0) {
                if (this.qdcservice.GridOperatoriPublicService) {
                    let giasGridComponent: GiasKendoGridComponent = this.qdcservice.GridOperatoriPublicService.giasGridComponent;

                    if (giasGridComponent && giasGridComponent.inLineModificaIsDisabled) {
                        listerroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.Bloccante,
                            tipo: enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate("CisonoOperatoriNonSalvati")
                        });
                    }
                }
            }

            //Controllo Grid Macchine righe non salvate
            if (listerroriGias.length === 0) {
                if (this.qdcservice.GridMacchinePublicService) {
                    let giasGridComponent: GiasKendoGridComponent = this.qdcservice.GridMacchinePublicService.giasGridComponent;

                    if (giasGridComponent && giasGridComponent.inLineModificaIsDisabled) {
                        listerroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.Bloccante,
                            tipo: enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate("CisonoMacchineNonSalvate")
                        });
                    }
                }
            }

            if(listerroriGias.length === 0){
              this.Controlli_Ribaltamento_In_Agenda(this.qdcservice.TestataForm.getRawValue(),listerroriGias);
            }

            let Sezioni_ProdottoFormArrayValue: Array<Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta> = this.qdcservice.Sezioni_ProdottoFormArray.getRawValue();
            if (listerroriGias.length === 0) {
                let Operazioni_senza_Sezione_Prodotto: Array<string> = [];
                Operazioni.forEach(o => {
                    let Categoria_Magazzino = this.qdcservice.getCategoria_Magazzino(+ o.primaryKey.codice);
                    switch (Categoria_Magazzino) {
                        case INSETTI:
                        case FORMULATI:
                        case FERTILIZZANTI:
                        case SEMENTI:
                        case TRASFORMATI_VEGETALI:
                            if (+ o.primaryKey.codice === enum_LAVCOD.RACCOLTA) {
                                if (!Sezioni_ProdottoFormArrayValue ||
                                    Sezioni_ProdottoFormArrayValue.findIndex(s => + s.Operazione.primaryKey.codice === enum_LAVCOD.RACCOLTA) === -1) {
                                    Operazioni_senza_Sezione_Prodotto.push(o.descrizione);
                                }
                            } else {
                                if (!Sezioni_ProdottoFormArrayValue ||
                                    Sezioni_ProdottoFormArrayValue.findIndex(s => s.Operazione.primaryKey.codice === o.primaryKey.codice) === -1) {
                                    Operazioni_senza_Sezione_Prodotto.push(o.descrizione);
                                }
                            }
                            break;
                    }
                });

                if (Operazioni_senza_Sezione_Prodotto.length > 0) {
                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("qdc.IndicareUnProdottoNellaSezione", { OperazioniList: Operazioni_senza_Sezione_Prodotto.join(",") })
                    });
                }
            }

            //Controllo frazionamento e modifica appezzamenti semina con una operazione
            if (listerroriGias.length === 0 && Operazioni.length > 1 && Sezioni_ProdottoFormArrayValue.length > 0) {
                let Sezione_Semina = Sezioni_ProdottoFormArrayValue.find(s => this.qdcservice.Elenco_Operazioni_Sementi.includes(+ s.Operazione.primaryKey.codice)) as Sezione_Prodotto_Sementi;

                if (Sezione_Semina && Sezione_Semina.Opzioni_Semina &&
                    (Sezione_Semina.Opzioni_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default ||
                        Sezione_Semina.Opzioni_Semina.codice === enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default)
                ) {
                    let Operazioni_Senza_Semina = Operazioni.filter(o => this.qdcservice.Elenco_Operazioni_Sementi.includes(+ o.primaryKey.codice) === false);
                    if (Sezione_Semina.Opzioni_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default) {
                        listerroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.Bloccante,
                            tipo: enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate("qdc.FrazionamentoSeminaMultiOperazione")
                        });
                    } else if (Sezione_Semina.Opzioni_Semina.codice === enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default) {
                        listerroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.Bloccante,
                            tipo: enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate("qdc.ModificaAnagraficaSeminaMultiOperazione")
                        });
                    }
                }
            }

            if (listerroriGias.length === 0) {
                if (this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare) {
                    this.Controlla_Grid_Prodotti_Magazzino(this.qdcservice.TestataForm.getRawValue(), this.qdcservice.ProdottiDaTrattareSelezionatiFormArray.getRawValue(), listerroriGias);
                } else {
                    this.Controlla_Grid_Impianti(this.qdcservice.TestataForm.getRawValue(), this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue(), listerroriGias);
                }
            }

            if (listerroriGias.length === 0)
                this.Controlla_Specie(this.qdcservice.TestataForm.getRawValue(), listerroriGias);

            if (listerroriGias.length === 0) {
                for (let op of Operazioni) {
                    let lav_cod: number = + op.primaryKey.codice;
                    let Sezione_ProdottoFormArrayValue: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta = Sezioni_ProdottoFormArrayValue.find(
                        (s: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta) => {
                            return + s.Operazione.primaryKey.codice === lav_cod;
                        }
                    );

                    if (Sezione_ProdottoFormArrayValue) {
                        this.Controlla_Sezioni_Prodotti(Sezione_ProdottoFormArrayValue, listerroriGias);
                    }
                }
            }

            let Sezioni_Senza_ProdottoFormArrayValue: Array<Sezione_Rilievi> = this.qdcservice.Sezioni_Senza_ProdottoFormArray.getRawValue();

            if(listerroriGias.length === 0 && Sezioni_Senza_ProdottoFormArrayValue && Sezioni_Senza_ProdottoFormArrayValue.length > 0){
              for (let op of Operazioni) {
                let lav_cod: number = + op.primaryKey.codice;
                let Sezione_Senza_ProdottoFormArrayValue: Sezione_Rilievi = Sezioni_Senza_ProdottoFormArrayValue.find(
                  (s: Sezione_Rilievi) => {
                    return + s.Operazione.primaryKey.codice === lav_cod;
                  }
                );

                if (Sezione_Senza_ProdottoFormArrayValue) {
                  this.Controlla_Sezioni_Senza_Prodotti(Sezione_Senza_ProdottoFormArrayValue, listerroriGias);
                }
              }
            }

            //Mostro gli errori a video
            if (listerroriGias.length > 0) {
                //listerroriGias[0].messaggio = this.translocoService.translate("AttenzioneLOperazioneNonÈStataRegistrataBr") + "" + listerroriGias[0].messaggio;

                this.qdcservice.gestisci_ErroriGias(listerroriGias, true, true).then(async (res: Obj_Errore_Gias_QdC) => {
                    if (res) {
                        switch (res.severity) {
                            case ErroreGias_Severity.Bloccante:
                                resolve(res.list_errori_filtrati);
                                break;
                            case ErroreGias_Severity.WarningBloccante:
                                resolve(res.list_errori_filtrati);
                                break;
                            case ErroreGias_Severity.Warning:
                                if (res.result.returnObj === true) {

                                    //Imposto il value a false del warning per non mostrarlo di nuovo se l'utente vuole continuare con il salvataggio
                                    let distinct_warning: ErroreGias[] = [...new Map(res.list_errori_filtrati.map(item => [item["ex"], item])).values()];

                                    if (distinct_warning && this.list_Warning_QdC) {
                                        for (let v of this.list_Warning_QdC) {
                                            for (let w of distinct_warning) {
                                                if (v.key === w.ex) {
                                                    v.value = "false";
                                                }
                                            }
                                        }
                                    }

                                    //Rifaccio i controlli saltando il warning che l'utente ha confermato
                                    res.list_errori_filtrati = await this.ControllaQdC()

                                    resolve(res.list_errori_filtrati);

                                } else {
                                    resolve(res.list_errori_filtrati);
                                }
                                break;
                        }
                    }
                });
            } else {
                resolve(listerroriGias);
            }
        });
    }

    //Se tutte le Operazioni sono di non utilizzo allora sono nel caso di non utilizzo
    private Operazioni_Non_Utilizzo_Scelte(Operazioni: Array<Lavorazione>): boolean {
        if (this.qdcservice.TestataForm.get('flagVisita').value)
            return true;

        let NonUtilizzo = false;
        let Operazioni_Non_Utilizzo = Operazioni.filter(o => this.qdcservice.Elenco_Operazioni_Non_Utilizzo.includes(+ o.primaryKey.codice));
        if (Operazioni_Non_Utilizzo && Operazioni_Non_Utilizzo.length > 0 && Operazioni_Non_Utilizzo.length === Operazioni.length) {
            NonUtilizzo = true;
        }
        return NonUtilizzo;
    }

    private Controlla_Specie(Testata: Testata, listerroriGias: ErroreGias[]) {
        let specie: Specie | DestinazioneUso = Testata.Specie;
        if ((!specie || specie.codice < 0) &&
            this.Operazioni_Non_Utilizzo_Scelte(Testata.Operazioni) === false &&
            !this.qdcservice.RilievoSenzaImpianti()
        ) {
            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("SelezionareUnaSpecieVegetale")
            });
        }
    }

    private Controlla_Grid_Impianti(Testata: Testata, Grid_Impianti_Selezionati: Array<GridImpiantoSelezionatoModel>, listerroriGias: ErroreGias[]) {
        if (Grid_Impianti_Selezionati.length === 0) {
            if (this.Operazioni_Non_Utilizzo_Scelte(Testata.Operazioni) === false && !this.qdcservice.RilievoSenzaImpianti()) {
                listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.Bloccante,
                    tipo: enum_ErroreGias_Tipo.Generico,
                    messaggio: this.translocoService.translate("SelezionareAlmenoUnImpiantoColturale")
                });
                return;

            } else {
                if (this.qdcservice.RilievoSenzaImpianti()) {
                    //I rilievi senza impianti devono avere almeno un centro aziendale selezionato per compatibilità con l'APP
                    if (!Testata.Centro_Aziendale || Testata.Centro_Aziendale.primaryKey.codice === 0) {
                        listerroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.Bloccante,
                            tipo: enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate("SelezionareAlmenoUnCentroAziendale")
                        });

                        return;
                    }
                } else {
                    if (!Testata.flagVisita && (!this.testataService.Array_CentriAziendale || this.testataService.Array_CentriAziendale.length <= 1)) {
                        listerroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.Bloccante,
                            tipo: enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate("CreaCentroAziendaleAssociatoAdImpresa")
                        });
                        return;

                    } else {
                        const hasRilievo = Testata.Operazioni.some(item => this.qdcservice.Elenco_Operazioni_Rilievi.includes(+item.primaryKey.codice));
                        if (((Testata.flagVisita && hasRilievo) || !Testata.flagVisita) && Testata.Specie && Testata.Specie.codice > 0) {
                            listerroriGias.push(<ErroreGias>{
                                severity: ErroreGias_Severity.Bloccante,
                                tipo: enum_ErroreGias_Tipo.Generico,
                                messaggio: this.translocoService.translate("SelezionareAlmenoUnImpiantoColturale")
                            });

                            return;
                        }
                    }
                }
            }
        } else{

          if(this.qdcservice.flag_Reinnesco){
            let Msg: string = this.GetMsgImpiantiNonValidiReinnesco();

            if(Msg !== ""){
              listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo:  enum_ErroreGias_Tipo.Generico,
                messaggio: Msg
              });

              return;
            }
          }else{
            if (Grid_Impianti_Selezionati.length > 0 && Testata.Operazioni && Testata.Operazioni.length > 0
              && Testata.Operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE || +o.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) > -1) { //irrigazioni e fertirrigazioni

              let erroreOreIrrigazione: boolean = false;
              let erroreDataFineIrrigazione: boolean = false;

              let erroreIrrigazionePrimaDellaValiditaEsercizio: boolean = false;
              let erroreIrrigazioneDopoLaValiditaEsercizio: boolean = false;
              let erroreFrequenzaIrrigazione: boolean = false;

              let messaggioErroreIrrigazionePrimaDellaValiditaEsercizio: string = "";
              let messaggioErroreIrrigazioneDopoLaValiditaEsercizio: string = "";
              let messaggioErroreFrequenzaIrrigazione: string = "";

              Grid_Impianti_Selezionati.forEach((impianto) => {
                if (impianto.OreIrrigazione > 24) {
                  erroreOreIrrigazione = true;
                }
                if (!impianto.DataFineIrrigazione) {
                  erroreDataFineIrrigazione = true;
                }

                if (impianto.DataInizioIrrigazione < impianto.Validita_Inizio_Distinta) {
                  erroreIrrigazionePrimaDellaValiditaEsercizio = true;
                  messaggioErroreIrrigazionePrimaDellaValiditaEsercizio += this.translocoService.translate("qdc.DataInizioIrrigazioneDeveEssereMaggioreDiDataInizio",
                    {
                      impianto: impianto.App_Nome,
                      inizioIrrigazione: this.datePipe.transform(impianto.DataInizioIrrigazione,'dd/MM/yyyy',undefined,this.locale_id),
                      inizioImpianto: this.datePipe.transform(impianto.Validita_Inizio_Distinta,'dd/MM/yyyy',undefined,this.locale_id)
                    }) + "\n";
                }

                if (impianto.DataFineIrrigazione && impianto.DataFineIrrigazione > impianto.Validita_Fine_Distinta) {
                  erroreIrrigazioneDopoLaValiditaEsercizio = true;
                  messaggioErroreIrrigazioneDopoLaValiditaEsercizio += this.translocoService.translate("qdc.DataFineIrrigazioneDeveEssereMinoreDiDataFine",
                    {
                      impianto: impianto.App_Nome,
                      fineIrrigazione: this.datePipe.transform(impianto.DataFineIrrigazione,'dd/MM/yyyy',undefined,this.locale_id),
                      fineImpianto: this.datePipe.transform(impianto.Validita_Fine_Distinta,'dd/MM/yyyy',undefined,this.locale_id)
                    }) + "\n";
                }

                if (impianto.DataFineIrrigazione) {
                  let daysDifference = this.irrigazioneAcquaService.getDaysDifference(impianto.DataInizioIrrigazione, impianto.DataFineIrrigazione);
                  if (impianto.FrequenzaIrrigazioneMedia > daysDifference){
                    erroreFrequenzaIrrigazione = true;
                    messaggioErroreFrequenzaIrrigazione += this.translocoService.translate("qdc.FrequenzaIrrigazioneMaggioreDiDifferenzaGiorni",
                      {
                        impianto: impianto.App_Nome,
                      }) + "\n";
                  }
                }
              });

              if (erroreOreIrrigazione) {
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.Bloccante,
                  messaggio: this.translocoService.translate("qdc.OreIrrigazioneMaggioriDi24"),
                });
              }

              if (erroreDataFineIrrigazione){
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.Bloccante,
                  messaggio: this.translocoService.translate("qdc.DataFineIrrigazioneNonValorizzata"),
                });
              }

              if (erroreIrrigazionePrimaDellaValiditaEsercizio){
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.Bloccante,
                  messaggio: messaggioErroreIrrigazionePrimaDellaValiditaEsercizio,
                });
              }

              if (erroreIrrigazioneDopoLaValiditaEsercizio){
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.Bloccante,
                  messaggio: messaggioErroreIrrigazioneDopoLaValiditaEsercizio,
                });
              }

              if (erroreFrequenzaIrrigazione){
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.Bloccante,
                  messaggio: messaggioErroreFrequenzaIrrigazione,
                });
              }

              let erroreDoseAcquaGiornaliera: boolean = Grid_Impianti_Selezionati.some(i => i.DoseAcquaGiornaliera <= 0);
              if (erroreDoseAcquaGiornaliera) {
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.Bloccante,
                  messaggio: this.translocoService.translate("qdc.DoseAcquaGiornalieraUgualeAZero"),
                });
              }
            }
          }



        }
    }

    private Controlla_Grid_Prodotti_Magazzino(Testata: Testata, Grid_ProdottiDaTrattare_Selezionati: Array<any>, listerroriGias: ErroreGias[]) {
        if (Grid_ProdottiDaTrattare_Selezionati.length > 0) {
            return;
        }

        if (this.Operazioni_Non_Utilizzo_Scelte(Testata.Operazioni) === false) {
            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("SelezionareAlmenoUnProdottoDaTrattare")
            });
            return;
        }

        if (!Testata.flagVisita && (!this.testataService.Array_CentriAziendale || this.testataService.Array_CentriAziendale.length <= 1)) {
            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("CreaCentroAziendaleAssociatoAdImpresa")
            });
            return;
        }

        if ((Testata.flagVisita && Testata.Operazioni.findIndex(item => this.qdcservice.Elenco_Operazioni_Rilievi.includes(+item.primaryKey.codice)) > -1)
            || !Testata.flagVisita) {

            if (Testata.Specie && Testata.Specie.codice > 0) {
                listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.Bloccante,
                    tipo: enum_ErroreGias_Tipo.Generico,
                    messaggio: this.translocoService.translate("SelezionareAlmenoUnProdottoDaTrattare")
                });
                return;
            }
        }
    }

    private Controlla_Sezioni_Prodotti(Sezione_ProdottoFormArrayValue: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta, listerroriGias: ErroreGias[]) {
        let elem_cod: number = Sezione_ProdottoFormArrayValue.Categoria_Magazzino;
        switch (elem_cod) {
            case FORMULATI:
            case INSETTI:
                this.Controlla_Sezione_Formulati(Sezione_ProdottoFormArrayValue as Sezione_Prodotto_Formulati, listerroriGias);
                break;
            case FERTILIZZANTI:
                this.Controlla_Sezione_Fertilizzanti(Sezione_ProdottoFormArrayValue as Sezione_Prodotto_Fertilizzanti, listerroriGias);
                break;
            case SEMENTI:
                this.Controlla_Sezione_Sementi(Sezione_ProdottoFormArrayValue as Sezione_Prodotto_Sementi, listerroriGias);
                break;
            case TRASFORMATI_VEGETALI:
                if (Sezione_ProdottoFormArrayValue.Operazione.primaryKey.codice === enum_LAVCOD.RACCOLTA.toString()) {
                    this.Controlla_Sezione_Raccolta(Sezione_ProdottoFormArrayValue as Sezione_Prodotto_Raccolta, listerroriGias);
                }
        }
    }

    private Controlla_Sezione_Formulati(Sezione_Formulati: Sezione_Prodotto_Formulati, listerroriGias) {
        let lav_cod = + Sezione_Formulati.Operazione.primaryKey.codice;
        this.Controlla_Grid_Dosi_Prodotti(FORMULATI, lav_cod, Sezione_Formulati.DosiProdotti, listerroriGias);
        if (listerroriGias.length > 0)
            return;
    }

    private Controlla_Sezione_Fertilizzanti(Sezione_Fertilizzanti: Sezione_Prodotto_Fertilizzanti, listerroriGias) {
        let lav_cod = + Sezione_Fertilizzanti.Operazione.primaryKey.codice;
        this.Controlla_Grid_Dosi_Prodotti(FERTILIZZANTI, lav_cod, Sezione_Fertilizzanti.DosiProdotti, listerroriGias);
        if (listerroriGias.length > 0)
            return;
    }

    private Controlla_Sezione_Sementi(Sezione_Sementi: Sezione_Prodotto_Sementi, listerroriGias: ErroreGias[]) {
        let Grid_Dosi_Prodotti: Array<any> = Sezione_Sementi.DosiProdotti;
        let lav_cod = + Sezione_Sementi.Operazione.primaryKey.codice;
        this.Controlla_Grid_Dosi_Prodotti(SEMENTI, lav_cod, Grid_Dosi_Prodotti, listerroriGias);
        if (listerroriGias.length > 0)
            return;

        let Operazione: Lavorazione = Sezione_Sementi.Operazione;
        let Opzione_Semina = this.qdcservice.getOpzione_Semina_Model(Operazione);

        if (Opzione_Semina) {
            if (Opzione_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default) {
                let Sementi_Con_Sup_Calcolata_0 = Grid_Dosi_Prodotti.filter(d => d.Sup_Calcolata === 0);

                if (Sementi_Con_Sup_Calcolata_0.length > 0) {
                    let list_prodotti: Array<string> = [];

                    Sementi_Con_Sup_Calcolata_0.forEach(s => {
                        let msg: string = s.Fr_Des;
                        if (s.Fabbricato_Des && s.Fabbricato_Des !== "") msg += " - " + s.Fabbricato_Des;
                        if (s.Lotto && s.Lotto !== "") msg += " - " + s.Lotto;
                        list_prodotti.push(msg);
                    });

                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("qdc.ProdotticonSupCalcolataNonValorizzata", { ProdottiList: list_prodotti.join(",") })
                    });
                    return;
                }
                this.qdcservice.Controllo_Sup_Calcolata(Opzione_Semina, null, Grid_Dosi_Prodotti, listerroriGias); // TODO M

            } else if (Opzione_Semina.codice === enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default) {
                //Posso seminare prodotti solo con lo stesso Mat_Cod (e lotto diverso)
                let distinct_mat_cod = [];

                Grid_Dosi_Prodotti.forEach(d => {
                    if (!distinct_mat_cod.includes(d.Fr_Cod))
                        distinct_mat_cod.push(d.Fr_Cod);
                });

                if (distinct_mat_cod.length !== 1) {
                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("qdc.SelezionareunaSementeperModificareGliImpianti")
                    });

                    return;
                }
            }
        }
    }

    private Controlla_Sezione_Raccolta(Sezione_Raccolta: Sezione_Prodotto_Raccolta, listerroriGias: ErroreGias[]) {
        let Prodotti_Raccolti = Sezione_Raccolta.ProdottiRaccolti;
        // TODO: Aggiungere controllo carenze(?)
        this.Controlla_Unita_Di_Misura(Prodotti_Raccolti, listerroriGias);
    }

    private Controlla_Unita_Di_Misura(raccolta: DettaglioRaccolta[], err: ErroreGias[]) {
        let filtered, udm;
        let codes = raccolta.map(prod => prod.prodotto.codice);
        codes = codes.filter((elem, index) => codes.indexOf(elem) === index);

        for (let code of codes) {
            filtered = raccolta.filter(rac => rac.prodotto.codice === code);
            udm = filtered.map(f => f.unitaDiMisura.codice);
            udm = udm.filter((elem, index) => udm.indexOf(elem) === index);

            if (udm.length > 1) err.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("UnitaMisuraNonConforme")
            });
        }
    }

    public Controlla_Grid_Dosi_Prodotti(elem_cod: number, lav_cod: number, Grid_Dosi_Prodotti: Array<any>, listerroriGias: ErroreGias[], from_salvataggio: boolean = true) {
        //Per la Raccolta non ho la grid Dosi Prodotti
        if (lav_cod !== enum_LAVCOD.RACCOLTA) {
            //Posso salvare una semina fast(senza prodotti) mentre negli altri casi ci devono essere i prodotti
            if (elem_cod !== SEMENTI) {
                if (!Grid_Dosi_Prodotti || Grid_Dosi_Prodotti.length === 0) {
                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("SelezionareAlmenoUnProdotto")
                    });
                    return;

                } else {
                    this.Controlla_Se_Ci_Sono_Righe_Non_Salvate(Grid_Dosi_Prodotti, from_salvataggio, listerroriGias);
                    if (listerroriGias.length > 0)
                        return;
                }
            } else if (Grid_Dosi_Prodotti && Grid_Dosi_Prodotti.length > 0) {
                this.Controlla_Se_Ci_Sono_Righe_Non_Salvate(Grid_Dosi_Prodotti, from_salvataggio, listerroriGias);
                if (listerroriGias.length > 0)
                    return;
            }
        }

        if (listerroriGias.length === 0) {
            let Prodotti_Con_Magazzini_Errati_List: Array<string> = [];
            for (let row of Grid_Dosi_Prodotti) {
                if (!this.qdcservice.Controlla_Magazzino(row) && !Prodotti_Con_Magazzini_Errati_List.includes(row.Prodotto.Descrizione_Concatenata)) {
                    Prodotti_Con_Magazzini_Errati_List.push(row.Prodotto.Descrizione_Concatenata);
                }
            }

            if (Prodotti_Con_Magazzini_Errati_List.length > 0) {
                let Msg_Errore = "<br> - " + Prodotti_Con_Magazzini_Errati_List.join("<br> - ");
                listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.Bloccante,
                    tipo: enum_ErroreGias_Tipo.Generico,
                    messaggio: this.translocoService.translate("qdc.SelezionareUnMagazzinoAziendaPerProdotti", { nomeAzienda: this.qdcservice.objParametriAgenda.RagSoc }) + Msg_Errore
                });
            }
        }
    }

    /*
    *@description: Controllo Prodotti nella grid Dosi
    * Posso salvare una semina fast(senza prodotti) mentre negli altri casi ci devono essere i prodotti
    * (Se nella semina ho cercato un prodotto ma non l'ho confermato comujnque ti blocco, per salvare una semina fast non devo aver selezionato nessun prodotto)
     */
    public Controlla_Se_Ci_Sono_Righe_Non_Salvate(Grid_Dosi_Prodotti: Array<any>, from_salvataggio: boolean, listerroriGias: ErroreGias[]) {
        let Grid_Dosi_Prodotti_Con_Righe_non_Salvate = Grid_Dosi_Prodotti.filter(d => d.Riga_Salvata === false);

        if (Grid_Dosi_Prodotti_Con_Righe_non_Salvate.length > 0) {
            let descrizione_prodotti_non_salvati = [];
            let msg = "";
            let Grid_Dosi_Prodotti_Con_Righe_non_Salvate_Semine = Grid_Dosi_Prodotti_Con_Righe_non_Salvate.filter(d => d.Categoria_Magazzino === SEMENTI);

            if (Grid_Dosi_Prodotti_Con_Righe_non_Salvate_Semine.length > 0) {
                descrizione_prodotti_non_salvati = Grid_Dosi_Prodotti_Con_Righe_non_Salvate_Semine.filter(d => d.Prodotto && d.Prodotto.Codice_Concatenato !== "" && d.Prodotto.Descrizione_Concatenata !== "").map(d => d.Prodotto.Descrizione_Concatenata);

                if (descrizione_prodotti_non_salvati.length > 0) {
                    let descrizioni = descrizione_prodotti_non_salvati.join(",");
                    if (from_salvataggio) {
                        msg = this.translocoService.translate("qdc.PrimaConfermaIProdotti", { ProdottiList: descrizioni });
                    } else {
                        msg = this.translocoService.translate("qdc.SalvaIProdotti", { ProdottiList: descrizioni });
                    }
                }
            } else {
                descrizione_prodotti_non_salvati = Grid_Dosi_Prodotti_Con_Righe_non_Salvate.map(d => d.Prodotto?.Descrizione_Concatenata);
                if (descrizione_prodotti_non_salvati.findIndex(d => !d) > -1) {
                    let operazioni = Grid_Dosi_Prodotti_Con_Righe_non_Salvate.map(d => d.Operazione.descrizione).join(",");
                    if (from_salvataggio) {
                        msg = this.translocoService.translate("qdc.PerSalvareScegliereUnProdottoNelleSezioni", { OperazioniList: operazioni });
                    } else {
                        msg = this.translocoService.translate("qdc.ScegliereUnProdottoNelleSezioni", { OperazioniList: operazioni });
                    }

                } else if (Grid_Dosi_Prodotti_Con_Righe_non_Salvate.filter(d => d.Categoria_Magazzino !== SEMENTI).length === Grid_Dosi_Prodotti_Con_Righe_non_Salvate.length) {
                    let descrizioni = descrizione_prodotti_non_salvati.join(",");
                    if (from_salvataggio) {
                        msg = this.translocoService.translate("qdc.PrimaConfermaIProdotti", { ProdottiList: descrizioni });
                    } else {
                        msg = this.translocoService.translate("qdc.SalvaIProdotti", { ProdottiList: descrizioni });
                    }
                }
            }

            if (msg !== "") {
                listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.Bloccante,
                    tipo: enum_ErroreGias_Tipo.Generico,
                    messaggio: msg
                });
            }
            return;
        }
    }

    GestisciWarning(listerroriGias: ErroreGias[]) {
        let testata: Testata = this.qdcservice.TestataForm.getRawValue();
        const hasFertilizzantiOFormulati = (o: number) => this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(o) ||
            this.qdcservice.Elenco_Operazioni_Formulati.includes(o);

        if (testata.Tipo === Tipo_Attivita.Ricetta && testata.TipoRicetta === Tipo_Ricetta.Standard_Destinazioni && testata.Stato === Stati.Da_Eseguire) {
            this.GestisciWarning_Ricette(testata, listerroriGias);
        }

        if (testata.Operazioni && testata.Operazioni.map(o => +o.primaryKey.codice).some(o => hasFertilizzantiOFormulati(o))) {
            let impiantiSelezionati: Array<GridImpiantoSelezionatoModel> = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();
            if (impiantiSelezionati && impiantiSelezionati.length > 0) {
                this.GestisciWarningDichiarazione_non_Utilizzo(testata, impiantiSelezionati, listerroriGias);
            }
        }
        return listerroriGias;
    }

    GestisciWarning_Ricette(testata: Testata, listerroriGias: ErroreGias[]) {
        const foundIndex = this.list_Warning_QdC.findIndex(w => w.key === Key_Parametri_Aggiuntivi_Attivita.mostraWarning_InviaRicettaAdAPP && w.value === "true");
        if (testata.InviaRicetta === true && foundIndex > -1) {
            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Warning,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("ImpossibileModificareRibaltareRicetteDopoSalvataggio"),
                ex: Key_Parametri_Aggiuntivi_Attivita.mostraWarning_InviaRicettaAdAPP
            });
        }
        return listerroriGias;
    }

    GestisciWarningDichiarazione_non_Utilizzo(testata: Testata, impiantiSelezionati: Array<GridImpiantoSelezionatoModel>, listerroriGias: ErroreGias[]) {
        const foundIndex = this.list_Warning_QdC.findIndex(w => w.key === Key_Parametri_Aggiuntivi_Attivita.mostraWarning_Dichiarazione_non_Utilizzo && w.value === "true");
        if (foundIndex > -1 && listerroriGias.length === 0) {
            let msg: string = "";

            if (testata.Operazioni.findIndex(o => this.qdcservice.Elenco_Operazioni_Formulati.includes(+ o.primaryKey.codice)) > -1 &&
                testata.Operazioni.findIndex(o => this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(+ o.primaryKey.codice)) > -1 &&
                impiantiSelezionati.findIndex(i => i.Dichiarazione_Non_Utilizzo_Trattamenti) > -1 &&
                impiantiSelezionati.findIndex(i => i.Dichiarazione_Non_Utilizzo_Fertilizzazioni) > -1) {
                msg = this.translocoService.translate("qdc.AttenzioneImpiantiConDichiarazioniDiNonUtilizzo");

            } else if (testata.Operazioni.findIndex(o => this.qdcservice.Elenco_Operazioni_Formulati.includes(+ o.primaryKey.codice)) > -1 &&
                impiantiSelezionati.findIndex(i => i.Dichiarazione_Non_Utilizzo_Trattamenti) > -1) {
                msg = this.translocoService.translate("qdc.AttenzioneImpiantiConDichiarazioniDiNonUtilizzoTrattamenti");

            } else if (testata.Operazioni.findIndex(o => this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(+ o.primaryKey.codice)) > -1 &&
                impiantiSelezionati.findIndex(i => i.Dichiarazione_Non_Utilizzo_Fertilizzazioni) > -1) {
                msg = this.translocoService.translate("qdc.AttenzioneImpiantiConDichiarazioniDiNonUtilizzoFertilizzazioni");
            }

            if (msg !== "") {
                listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.Warning,
                    tipo: enum_ErroreGias_Tipo.Generico,
                    messaggio: msg,
                    ex: Key_Parametri_Aggiuntivi_Attivita.mostraWarning_Dichiarazione_non_Utilizzo
                });
            }
        }
        return listerroriGias;
    }

    //** @description Controllo anche se non ci sono impianti validi selezionati alla data
    public GetMsgImpiantiNonValidiReinnesco(): string {

      let msg: string = "";

      if(this.qdcservice.GridImpiantiPublicService && this.qdcservice.GridImpiantiPublicService.getValue()){

        let Rows: KendoGridRow[] = this.qdcservice.GridImpiantiPublicService.getValue()?.data?.rows;

        if(Rows) {

          let righeSelezionate: KendoGridRow[] = Rows.filter(r => r["Selected"]);

          if (righeSelezionate) {

            let righeSelezionateFormArray: GridImpiantoSelezionatoModel[] = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();

            if (righeSelezionateFormArray && righeSelezionateFormArray.length > 0) {

              let ImpiantiNonValidi: number = righeSelezionateFormArray.length - righeSelezionate.length;

              if(ImpiantiNonValidi > 0){

                let righeSelezionateFormArray: GridImpiantoSelezionatoModel[] = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();

                let data: Date = this.qdcservice.TestataForm.get("Data").getRawValue();

                let data_str: string = this.qdcservice.datepipe.transform(data,'dd/MM/yyyy',undefined,this.qdcservice.locale_id);

                let AttivitaLetta: Attivita_Con_Parametri_Aggiuntivi[] = this.qdcservice.getListaAttivita();

                if(AttivitaLetta && AttivitaLetta.length > 0){
                  let Installazionetrappole = AttivitaLetta.find(a=> +a.job.primaryKey.codice === enum_LAVCOD.INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA);

                  if(Installazionetrappole) {
                    let descrizione_attivita: string = Installazionetrappole.descrizione + " " + this.qdcservice.datepipe.transform(Installazionetrappole.inizio, 'dd/MM/yyyy', undefined, this.qdcservice.locale_id) + " (ID: "+ Installazionetrappole.codice +")";

                    msg = this.qdcservice.translocoService.translate("CiSonoImpiantiNonValidi", {
                      DataStr: data_str,
                      Descrizione_Op: descrizione_attivita
                    });
                  }
                }
              }
            }
          }
        }
      }

      return msg;
    }

    private Controlla_Sezioni_Senza_Prodotti(Sezione_Senza_ProdottoFormArrayValue: Sezione_Rilievi, listerroriGias: ErroreGias[]) {
      let lav_cod: number = + Sezione_Senza_ProdottoFormArrayValue.Operazione.primaryKey.codice;

      if(lav_cod === enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE){
        if(!Sezione_Senza_ProdottoFormArrayValue.DettagliRilievi || Sezione_Senza_ProdottoFormArrayValue.DettagliRilievi.length === 0){
          listerroriGias.push(<ErroreGias>{
            severity: ErroreGias_Severity.Bloccante,
            tipo: enum_ErroreGias_Tipo.Generico,
            messaggio: this.translocoService.translate("SelezionareUnaAvversità")
          });
          return listerroriGias;
        }
      }
    }

    //**
    // @description:
    // In fase di ribaltamento non posso salvare un'Operazione di Agenda con data futura da un Brogliaccio o da una Ricetta
    // *//
    private Controlli_Ribaltamento_In_Agenda(testata: Testata, listerroriGias: ErroreGias[]){

      if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda ||
        this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda){

        let data_operazione: Date = testata.Data;

        let data_operazione_number: number = data_operazione.getTime();

        let data_oggi_number: number = new Date().setHours(0, 0, 0, 0);

        if(data_operazione_number > data_oggi_number){

          let data_str: string = this.qdcservice.datepipe.transform(data_operazione,'dd/MM/yyyy',undefined,this.qdcservice.locale_id);

          let msg:string = "";

          if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda){
            msg = this.qdcservice.translocoService.translate("NonRibaltareBrogliaccioFuturo", {
              DataStr: data_str
            });
          }else{
            msg = this.qdcservice.translocoService.translate("NonRibaltareRicettaFutura", {
              DataStr: data_str
            });
          }

          listerroriGias.push(<ErroreGias>{
            severity: ErroreGias_Severity.Bloccante,
            tipo: enum_ErroreGias_Tipo.Generico,
            messaggio: msg
          });
          return listerroriGias;

        }
      }
    }
}

/**
 * Utilizza una regular expression per verificare se la stringa è un numero positivo con o senza virgola
 * e testa la stringa con il pattern
 */
function isNumeroPositivoValido(input: string): boolean {
    const numeroPattern = /^(0|[1-9]\d*)(\.\d+)?$/;
    return numeroPattern.test(input);
}
