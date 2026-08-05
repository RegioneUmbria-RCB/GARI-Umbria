import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormControl } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { QdCRaccoltaService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti/raccolta.service';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { enum_Ripartizione_Raccolta } from './opzioni-raccolta/opzioni-raccolta.model';
import { ImpostazioniUtentiService } from '../../../../../../profilazione/services/impostazioni/impostazioni-utenti.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { MasterService } from 'app/Service/master.service';
import { DettaglioRaccolta } from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import { QuantitaSuImpianto } from 'app/Model/attivita/dettagli/QuantitaSuImpianto';
import { Tipo_Raccolta } from 'app/Model/attivita/Attivita';
import { RilevamentoDiMagazzino } from 'app/Model/attivita/RilevamentoDiMagazzino';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { enum_UnitaMisura } from 'app/Model/TipiEnumerativi';
import { Prodotto } from 'app/Model/attivita/risorse/Prodotto';
import { ImpostazioniAziendeCentriService } from '../../../../../../profilazione/services/impostazioni/impostazioni-aziende-centri.service';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { TRASFORMATI_VEGETALI } from 'app/Model/CostantiPersonalizzate';
import { Subject, takeUntil } from 'rxjs';
import { GiasDialogService } from 'app/Service/gias-dialog.service';


@Component({
  standalone: false,
  selector: 'app-raccolta',
  templateUrl: './raccolta.component.html',
  styleUrls: ['./raccolta.component.css'],
  providers: [ QdCRaccoltaService ]
})
export class RaccoltaComponent implements OnInit, OnDestroy {

    public readonly MANUALE = enum_Ripartizione_Raccolta.MANUALE;
    public ripartizione = enum_Ripartizione_Raccolta.MANUALE;

    signal: Subject<void> = new Subject<void>();
    private readonly DEBUG_FORCE_CARICHI_ATTIVI = false;

    constructor(
        private qdcservice: QdCService,
        private dialogService: GiasDialogService,
        private raccoltaService: QdCRaccoltaService,
        private translocoService: TranslocoService,
        private USService: ImpostazioniUtentiService,
        private ACService: ImpostazioniAziendeCentriService,
        private masterService: MasterService,
        ) { }

    get Risorse() {
        return this.raccoltaService.sezione_prodotto
                    .get('ProdottiRaccolti') as FormArray;
    }

    public get isInInfoMode() {
        return this.raccoltaService.operationType === Enum_DBTypeOperation.Read;
    }

    ngOnInit(): void {
        let raccolta = this.createRaccolta();
        this.raccoltaService.sezione_prodotto.addControl(
            "FastHarvestTemplate", new FormControl(raccolta),
            {emitEvent: false}
        );

        if (this.qdcservice.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write
            && !this.raccoltaService.sezione_prodotto.value?.ProdottiRaccolti?.length) {
                this.Risorse.push(new FormControl(raccolta));
            }

        this.checkSettingsConsistency();
        this.handleEvents();
        this.handleWarnings();
    }

    ngOnDestroy() {
        this.signal.next();
        this.signal.complete();
        this.raccoltaService.clear()
    }

    public onAdd() {
        this.raccoltaService.addNewRaccoltaAt(0);
    }

    private handleEvents() {
        // // Gestione interfaccia in base ripartizione
        let ctrl = this.raccoltaService.sezione_prodotto.get('Opzioni_Raccolta')?.get('Ripartizione');
        if (ctrl)
            ctrl.valueChanges.pipe(takeUntil(this.signal))
            .subscribe( codice => {
                this.ripartizione = codice;
                if (codice === enum_Ripartizione_Raccolta.MANUALE && !this.Risorse.length) {
                    this.raccoltaService.addNewRaccoltaAt();
                }
            });

        this.raccoltaService.loadEndEvent.pipe(takeUntil(this.signal))
        .subscribe(end => {
            this.ripartizione = this.raccoltaService.sezione_prodotto
                    .get('Opzioni_Raccolta').get('Ripartizione').value;
        });

        // Aggiornamento data raccolta
        ctrl =  this.raccoltaService.sezione_prodotto.get('Opzioni_Raccolta').get('dataRaccolta');
        if (ctrl)
            ctrl.valueChanges.pipe(takeUntil(this.signal))
            .subscribe( codice => {
                this.raccoltaService.updateHarvestDate();
            });
    }

    private createRaccolta(): DettaglioRaccolta {
        let raccolta = new DettaglioRaccolta();

        raccolta.dataIngresso = this.raccoltaService.harvestDate
        raccolta.TipoRaccolta = Tipo_Raccolta.Fast;
        raccolta.quantitaTotaleReale = 0;
        raccolta.QuantitaSuImpianti = [];
        raccolta.prodotto = new Prodotto(0);
        raccolta.unitaDiMisura = new UnitaDiMisura(
                                    enum_UnitaMisura.KG, ''
                                );

        let esercizi = this.qdcservice.GetEserciziCDCSelezionatiModel();
        raccolta.QuantitaSuImpianti = esercizi.map( cdc => {
            let ri = new QuantitaSuImpianto();
            ri.Qta = 0;
            ri.esercizioCDC = cdc;
            return ri;
        });
        raccolta.MagazziniMovimentazioni = [];

        this.raccoltaService.loadEndEvent.pipe(takeUntil(this.signal))
            .subscribe(loaded => {
                if (loaded && this.raccoltaService.Raccolta_Con_Carico_Magazzino) {
                    raccolta.MagazziniMovimentazioni.push(
                        new RilevamentoDiMagazzino()
                    );
                }
            });

        return raccolta;
    }

    private async checkSettingsConsistency() {
        this.loadSettings();

        this.raccoltaService.sezione_prodotto.get('Opzioni_Raccolta')
            .get('CarichiMagazzinoAttivi').patchValue(
                this.raccoltaService.Raccolta_Con_Carico_Magazzino
            );

        this.raccoltaService.loadEndEvent.next( true );

        if (this.isMessageShown()) {
            this.dialogService.baseInfo('', 'qdc.InfoModifica');
            this.removeCarichi();
        }
    }

    private loadSettings() {
        const RaccoltaConCarichi = this.ACService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser(
            this.masterService.objP_utenti.PivaSuperUser,
            0, enum_Impostazioni_Utenti.Raccolta_Con_Carico_Magazzino
        );
        const GestioneMagazzino = this.ACService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(
            this.masterService.objP_utenti.PivaSuperUser, 0,
            enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA,
            TRASFORMATI_VEGETALI
        );
        const FiltroLotti = this.ACService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(
            this.masterService.objP_utenti.PivaSuperUser, 0,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI,
            TRASFORMATI_VEGETALI
        );
        // Default: carichi attivi
        let carichiAttivi = RaccoltaConCarichi === '' || !!parseInt(RaccoltaConCarichi, 10);
        let lottoAttivo = true;
        // 0 = nessuna gestione, 1 = gestione obbligatoria, 2 = gestione facoltativa
        if (FiltroLotti.includes(TRASFORMATI_VEGETALI+'_0') || FiltroLotti == '0') {
            lottoAttivo = false;
        }
        if (GestioneMagazzino.includes(TRASFORMATI_VEGETALI+'_0') || GestioneMagazzino == '0') {
            lottoAttivo = false;
            carichiAttivi = false;
        }

        if (this.DEBUG_FORCE_CARICHI_ATTIVI) {
            lottoAttivo = true;
            carichiAttivi = true;
        }

        this.raccoltaService.Raccolta_Con_Carico_Magazzino = carichiAttivi;
        this.raccoltaService.lottiAttivi = lottoAttivo && carichiAttivi;
    }

    private isMessageShown(): boolean {
        let showMsg = false;

        (this.raccoltaService.sezione_prodotto.get('ProdottiRaccolti')
            .value as DettaglioRaccolta[])
            .forEach(raccolta => {

            if (showMsg) return;

            let tipoRaccolta = raccolta.TipoRaccolta;

            if (!tipoRaccolta) tipoRaccolta = this.deduceTipoRaccolta(raccolta);

            if ( !tipoRaccolta ) showMsg = true;

            if (tipoRaccolta === Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino
                && !this.raccoltaService.Raccolta_Con_Carico_Magazzino) {
                    // Raccolta con carichi magazzino, ma impostazione raccolta
                    // con carichi magazzino disattivata
                    showMsg = true;
            }
            // else: se opzione carichi magazzino attiva non perdiamo alcuna
            // informazione, al massimo ne aggiungiamo
        });

        return showMsg;
    }

    private deduceTipoRaccolta(raccolta: DettaglioRaccolta): Tipo_Raccolta {
        if (!raccolta) return null;

        if (!raccolta.prodotto) return Tipo_Raccolta.Fast;

        if (!raccolta.MagazziniMovimentazioni
            || !raccolta.MagazziniMovimentazioni.length) {
                return Tipo_Raccolta.Leggera;
            }

        return Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino;
    }

    private removeCarichi() {
        let array = this.raccoltaService.sezione_prodotto
                        .get('ProdottiRaccolti') as FormArray;
        let raccolta: DettaglioRaccolta;
        for (let ctrl of array.controls) {
            raccolta = ctrl.value;

            raccolta.MagazziniMovimentazioni = [];
            raccolta.QuantitaSuImpianti.forEach(q => {
                q.Lotto = '';
                q.Magazzino = new Fabbricato({
                    codice: 0, centroAziendalePK: {
                        partitaIva: '', codice: 0
                    }
                });
            });
            ctrl.patchValue(raccolta);
        }
    }


    private handleWarnings() {
        let prevStatus: string;
        this.raccoltaService.sezione_prodotto.statusChanges.GiasSubscribe(status => {
            if (!prevStatus) prevStatus = status;
            if (status === 'VALID') {
                prevStatus = status;
                return;
            }
            // console.log("FORM INVALID", this.raccoltaService.sezione_prodotto.get("ProdottiRaccolti").errors)
            let prodotti = this.raccoltaService.sezione_prodotto.get("ProdottiRaccolti").value;
            let error = this.raccoltaService.sezione_prodotto.get("ProdottiRaccolti").errors;
            if (!error)
                error = this.raccoltaService.sezione_prodotto.get("Opzioni_Raccolta").errors
            if (!error || status === prevStatus) return;
            prevStatus = status;

            // if (error.storageAlreadySelected) {
            //     this.dialogService.baseError(
            //         this.translocoService.translate("Attenzione"),
            //         this.translocoService.translate("WarningProdottoLotto", [
            //             prodotti[error.idx].prodotto.descrizione,
            //             prodotti[error.idx].QuantitaSuImpianti[0].Lotto,
            //             prodotti[error.idx].QuantitaSuImpianti[0].Magazzino.descrizione
            //         ]),
            //         false
            //     );
            // }
        })
    }

}
