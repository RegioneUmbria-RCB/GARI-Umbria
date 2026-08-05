import { ChangeDetectorRef, Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { enum_zoomVisualizzazioneTotale } from '../GIS-enum/GIS-zoom';

import {
    GisClient,
    ScriviConfigurazioneGisUtente,
    CfgAlbero_CfgGisUtente,
    ConfigurazioneGisUtente,
    ConfigurazioneGisUtente_enum_TipoRender_ServerSide
} from 'app/Service/api.service';

import { GiasMessageService } from 'app/Service/gias-message.service';
import { MasterService } from 'app/Service/master.service';
import { tap, Observable, finalize, take } from 'rxjs';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { GiasDialogService } from '../../Service/gias-dialog.service';
import { faLessThanEqual } from '@fortawesome/free-solid-svg-icons';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { enum_ClusteringLevel } from '../GIS-enum/GIS-clustering-level';

export class DdlTipoOggettoGrafico {
    sePunto: boolean
    descrizione: string
}

@Component({
    standalone: false,
    selector: 'app-gis-configuration-modal',
    templateUrl: './gis-configuration-modal.component.html',
    styleUrls: ['./gis-configuration-modal.component.css'],
})
export class GISConfigurationModalComponent {

    public isModalOpen = false;
    public config: ConfigurazioneGisUtente;
    public cfgAlbero_cfgGisUtente: CfgAlbero_CfgGisUtente;
    public form: FormGroup | null;

    public autoZoomVisTotChecked = false;

    elencoTipiOggettoGrafici: DdlTipoOggettoGrafico[] = [];

    // private livelloZoomPrec = null;

    constructor(
        private masterService: MasterService,
        private gisClient: GisClient,
        private giasMessageService: GiasMessageService,
        private fb: FormBuilder,
        private translocoService: TranslocoService,
        private sharedDataService: SharedDataService,
        private giasDialogService: GiasDialogService
    ) {

        this.popolaDdlTipoOggettoGrafico();

    }

    private popolaDdlTipoOggettoGrafico() {

        let element = new DdlTipoOggettoGrafico();
        element.sePunto = false;
        element.descrizione = this.translocoService.translate('Poligono');
        this.elencoTipiOggettoGrafici.push(element);

        element = new DdlTipoOggettoGrafico();
        element.sePunto = true;
        element.descrizione = this.translocoService.translate('Punto');
        this.elencoTipiOggettoGrafici.push(element);

    }

    public open() {
        this.isModalOpen = true;
        let sharedCfgAlberoGisUtente = this.sharedDataService.getCfgAlberoGisUtente()[0];
        this.cfgAlbero_cfgGisUtente = sharedCfgAlberoGisUtente;
        this.config = this.cfgAlbero_cfgGisUtente.CfgGisUtente;
        this.buildForm();
    }

    submit(): void {
        this.submitObs().subscribe();
    }

    onToggleChangeZoomVisTotale() {
        this.autoZoomVisTotChecked=!this.autoZoomVisTotChecked;
        if (!this.autoZoomVisTotChecked) {
            this.form.controls.iAutoZoomSuVisualizzazioneTotale.setValue(enum_zoomVisualizzazioneTotale.no);
        } else {
            this.form.controls.iAutoZoomSuVisualizzazioneTotale.setValue(enum_zoomVisualizzazioneTotale.min);
        }
        this.submitObs().subscribe();
    }

    onAfterValueChangedZoomVisTotale() {
        const iAutoZoom = this.form.controls.iAutoZoomSuVisualizzazioneTotale.value;
        let forceAutoZoom = null;

        switch (true) {

            case (iAutoZoom === null):
                forceAutoZoom = enum_zoomVisualizzazioneTotale.min;
                break;

            case (iAutoZoom < enum_zoomVisualizzazioneTotale.min):
                forceAutoZoom = enum_zoomVisualizzazioneTotale.min;
                break;

            case (iAutoZoom > enum_zoomVisualizzazioneTotale.max):
                forceAutoZoom = enum_zoomVisualizzazioneTotale.max;
                break;

            // case (iAutoZoom > enum_zoomVisualizzazioneTotale.no && iAutoZoom < enum_zoomVisualizzazioneTotale.min):
            //     if (this.livelloZoomPrec === enum_zoomVisualizzazioneTotale.min && iAutoZoom === enum_zoomVisualizzazioneTotale.min - 1) {
            //         forceAutoZoom = enum_zoomVisualizzazioneTotale.no;
            //     } else {
            //         forceAutoZoom = enum_zoomVisualizzazioneTotale.min;
            //     }
            //     break;

        }
        if (forceAutoZoom !== null) {
            this.form.controls.iAutoZoomSuVisualizzazioneTotale.setValue(forceAutoZoom);
        }
        // this.livelloZoomPrec = this.form.controls.iAutoZoomSuVisualizzazioneTotale.value;
        this.submitObs().subscribe();
    }

    onAfterValueChangedZoomCluster() {
        let clusterLevel = this.form.controls.LivelloClusterizzazione.value;
        if (
          clusterLevel == null ||
          clusterLevel < enum_ClusteringLevel.min ||
          clusterLevel > enum_ClusteringLevel.max
        ) {
          this.form.controls.LivelloClusterizzazione.setValue(
            enum_ClusteringLevel.default
          );
        }
        this.submitObs().subscribe();
    }

    close(): void {
        this.form = null;
        this.isModalOpen = false;
    }

    private submitObs(): Observable<any> {
        const payload = {
            MemorizzaSistemaDiRiferimentoPredefinito: false,
            MemorizzaOperazioneColturale: false,
            CfgGisUtente: this.form.value
        } as ScriviConfigurazioneGisUtente;

        return this.gisClient
            .gisCfgGISSalva(payload)
            .pipe(
                take(1),
                tap(() => this.masterService.set_isLoading({ isLoading: true })),
                finalize(() => {
                    this.masterService.set_isLoading({ isLoading: false });
                    this.giasMessageService.successMessage(this.translocoService.translate('SalvataggioAvvenutoConSuccesso'));
                    // this.giasDialogService.baseSuccess('Setup Visualizzazione', 'SalvataggioAvvenutoConSuccesso');
                    // Aggiorna configurazione utente
                    this.cfgAlbero_cfgGisUtente.CfgGisUtente = payload.CfgGisUtente;
                    // Aggiorna configurazione albero
                    let ricaricaAlbero = this.determinaRicaricaAlbero(payload);
                    this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Agenda = payload.CfgGisUtente.ckMostraOperazioniAgenda;
                    this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Planning = payload.CfgGisUtente.ckMostraPlanning;
                    this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Ricette = payload.CfgGisUtente.ckMostraRicette;
                    this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Fabbricati = payload.CfgGisUtente.ckMostraFabbricati;
                    this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Analisi = payload.CfgGisUtente.chkMostraAnalisi;
                    this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Anagrafica = payload.CfgGisUtente.chkMostraAnagrafica;
                    this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_CatastoAziendale = payload.CfgGisUtente.chkMostraCatasto;
                    this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_CatastoAppezzamento = payload.CfgGisUtente.chkMostraCatastoAppezzamento;
                    // if (this.sharedDataService.iAutoZoomSuVisTotaleLocalStorage) {
                    //     localStorage.setItem("iAutoZoomSuVisualizzazioneTotale", this.cfgAlbero_cfgGisUtente.CfgGisUtente.iAutoZoomSuVisualizzazioneTotale.toString());
                    //     console.log("Salvataggio iAutoZoomSuVisualizzazioneTotale",this.cfgAlbero_cfgGisUtente.CfgGisUtente.iAutoZoomSuVisualizzazioneTotale.toString());
                    // }
                    // Aggiorna oggetto condiviso
                    this.sharedDataService.setCfgAlberoGisUtente(this.cfgAlbero_cfgGisUtente, ricaricaAlbero);
                })
            );
    }

    private determinaRicaricaAlbero(payload: ScriviConfigurazioneGisUtente): boolean {
        let ricaricaAlbero = false;
        if (this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Anagrafica !== payload.CfgGisUtente.chkMostraAnagrafica ||
            this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_CatastoAziendale !== payload.CfgGisUtente.chkMostraCatasto ||
            this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_CatastoAppezzamento !== payload.CfgGisUtente.chkMostraCatastoAppezzamento ||
            this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Analisi !== payload.CfgGisUtente.chkMostraAnalisi ||
            this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Planning !== payload.CfgGisUtente.ckMostraPlanning ||
            this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Ricette !== payload.CfgGisUtente.ckMostraRicette ||
            this.cfgAlbero_cfgGisUtente.CfgAlbero.Flag_Fabbricati !== payload.CfgGisUtente.ckMostraFabbricati) {
            ricaricaAlbero = true;
        }
        return ricaricaAlbero;
    }

    private buildForm(): void {
        const payload = FunzioniComuniService.getStandardConfigurazioneGisUtente(this.config)
        this.form = this.fb.group<ConfigurazioneGisUtente>(payload);

        if (this.form.controls.iAutoZoomSuVisualizzazioneTotale.value === enum_zoomVisualizzazioneTotale.no) {
            this.autoZoomVisTotChecked = false;
        } else {
            this.autoZoomVisTotChecked = true;
        }
        // this.livelloZoomPrec = this.form.controls.iAutoZoomSuVisualizzazioneTotale.value;
    }

}
