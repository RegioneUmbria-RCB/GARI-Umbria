import { Component, ElementRef, Inject, Input, OnDestroy } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { AnalisiTerrenoFormService } from "../griglia-analisi-terreno/service/analisi-terreno-form.service";
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { AnalisiDocumentsService } from "../griglia-analisi-terreno/service/analisi-documents.service";
import { GISModality } from "app/GIS/GIS-enum/GIS-feature";
import { AnalisiTerrenoService } from "../griglia-analisi-terreno/service/analisi-terreno.service";
import { AnalisiDettaglio, AnalisiEntita_1OfAppezzamento, AnalisiEntita_1OfCampo, AnalisiEntita_1OfCentroAziendale, AnalisiEntita_1OfImpianto, AnalisiEntita_1OfImpresa, AnalisiEntita_1OfCatastoCentroAziendale, AnalisiParametro, LeggiSchemiAnalisi, ScriviAnalisiTerreno, Campione, AnalisiTerreno } from "app/Service/api.service";
import { enum_AnalisiTipo, enum_Entita_Analisi, enum_PagineGiasNG, enum_Security_Attivita, enum_UnitaMisura } from "app/Model/TipiEnumerativi";
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { TranslocoService } from "@jsverse/transloco";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { Location } from '@angular/common';
import { ExternalNavigationService } from "app/Service/external-navigation.service";
import { faTrashCan } from "@fortawesome/free-solid-svg-icons";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { Campo } from "app/Model/anagrafiche/Campo";
import { CentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import { Appezzamento } from "app/Model/anagrafiche/Appezzamento";
import { Impianto } from "app/Model/anagrafiche/Impianto";
import { ParticelleCatastali } from "app/Model/anagrafiche/ParticelleCatastali";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { ConversionService } from "app/Service/conversion.service";
import { CatastoCentroAziendale } from "app/Model/anagrafiche/CatastoCentroAziendale";
import { enum_AnalisiTerrenoGridCommands } from "../utils";
import { GiasDropDownTemplateService, ObjParametriAgenda } from 'gias-ui-kit';
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { GISComponent } from "app/GIS/GIS.component";
import { enum_LayerElementiGraficiStd } from "app/GIS/GIS-enum/GIS-layer-elementi-grafici";
import { GisService } from "app/GIS/GIS.service";
import { UtilizzoTerreno } from "app/Model/metaschema/utilizzi/UtilizzoTerreno";
import { ActivatedRoute } from "@angular/router";
import { Subject, BehaviorSubject, combineLatest, startWith, takeUntil, filter, tap, skip, take, catchError, of, lastValueFrom } from "rxjs";
import { GISGeometrySelectionService } from "app/GIS/services/gis-geometry-selection.service";
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { PivaRealeService } from 'app/anagrafica/imprese/piva-reale.service';


@Component({
    standalone: false,
    selector: 'app-analisi-terreno-edit',
    templateUrl: './analisi-terreno-edit.component.html',
    styleUrls: ['./analisi-terreno-edit.component.scss'],
    providers: [GiasDropDownTemplateService, GiasMultiSelectTemplateService]
})
export class AnalisiTerrenoEditComponent implements OnDestroy {

    @Input() onEdit: boolean = false;

    disabled: boolean = false;

    signal: Subject<void> = new Subject();

    protected objParametriAgenda: ObjParametriAgenda;

    protected analisiTerrenoForm: FormGroup;

    listaLaboratori = [];
    listaSchemiAnalisi;
    listaCentri = [];
    listaCatasto = [];
    listaCompletaParticelle = [];

    listaUdM: any = Object.entries(enum_UnitaMisura)
        .filter(([key, value]) => isNaN(Number(key)) && (value == enum_UnitaMisura.KG || value == enum_UnitaMisura.Grammi || value == enum_UnitaMisura.Litri || value == enum_UnitaMisura.CentimetriCubi || value == enum_UnitaMisura.Millilitri))
        .map(([key, value]) => ({ descrizione: this.transloco.translate(key), codice: +value }));

    ListaEntita: any = Object.entries(enum_Entita_Analisi)
        .filter(([key, value]) => isNaN(Number(key)) && value !== enum_Entita_Analisi.NonDefinito && value !== enum_Entita_Analisi.Fabbricato && value !== enum_Entita_Analisi.EntitaGrafica)
        .map(([key, value]) => ({ nome: this.transloco.translate(key), id: +value }));

    GISModality = GISModality;
    GisComponentSubject = new BehaviorSubject<GISComponent>(null);

    showGridEntita: boolean = false;
    showImpresa: boolean = false;
    showGridParametri: boolean = false;
    disabledDeleteCampione: boolean = false;

    //booleani controllo apertura gias-expansionPanel
    isExpandedParametri: boolean = false;
    isExpandedCampione: boolean = false;
    isExpandedNote: boolean = false;

    openInIFrame: boolean = false;

    infoAzienda: string;

    faTrashCan = faTrashCan;
    permessoScritturaDoc: boolean;
    permessoLetturaDoc: boolean;

    skipChecksAggancioPC_PUA: boolean = false;

    //gestione chiamanti
    fromGIS: boolean = false;
    fromMenuGrid: boolean = false;
    
    marker: google.maps.Marker | null = null;
    gisClickListener: google.maps.MapsEventListener | null = null;
    onInfo: boolean = false;

    get campioni(): FormArray {
        return this.analisiFormService.formAnalisiTerreno.get('campioni') as FormArray;
    }

    get filteredEntitaCoinvolte(): any[] {
        const entitaCoinvolte = this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte')?.value || [];
        return entitaCoinvolte.filter(item => item !== enum_Entita_Analisi.Impresa); // Filtro condizionale
    }

    constructor(
        private fb: FormBuilder,
        private elementRef: ElementRef,
        private objParametriAgendaService: ObjParametriAgendaService,
        private analisiDocumentsService: AnalisiDocumentsService,
        private permessiUtenteService: PermessiUtenteService,
        public analisiFormService: AnalisiTerrenoFormService,
        public analisiDataService: AnalisiTerrenoService,
        private giasDialogService: GiasDialogService,
        private externalNavigationService: ExternalNavigationService,
        private conversionService: ConversionService,
        private location: Location,
        private gisService: GisService,
        public transloco: TranslocoService,
        private route: ActivatedRoute,
        private funzionicomuniservice: FunzioniComuniService,
        private gisGeometrySelectionService: GISGeometrySelectionService,
        private pivaRealeService: PivaRealeService,
        @Inject(LOADING_TOKEN) private loadingService: LoadingService
    ) {
        // Reset fields because they are being updated by google-map-geojson.service upon feature selection
        // when page is reloaded, the objParams are being read from the session storage and could be "dirty".
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.objParametriAgenda.Impianti = [];
        this.objParametriAgenda.Sa_Cod = null;
        this.objParametriAgenda.Appezza = null;
        this.objParametriAgenda.Id_Reg = null;
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

        this.permessoScritturaDoc = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Documentale_Inser, 2);
        this.permessoLetturaDoc = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Documentale_Lista, 0);

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.manageGisIntegration();

        this.analisiFormService.modeAgganciataPC_PUA = false;

        //verifichiamo che non ci sia GenericOBJ_string valorizzato
        if (this.objParametriAgenda.GenericObj_string) {

            this.loadingService.set_isLoading({ isLoading: true, component: this.elementRef });

            let obj = JSON.parse(this.objParametriAgenda.GenericObj_string);

            this.analisiDataService.LeggiAnalisiTerreno({ Analisi_Cod: obj.Analisi_Testata_Cod }).subscribe(r => {

                this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });

                let analisiTerrenoOBJ = r as AnalisiTerreno;

                let objResponse = this.conversionService.ConversionDateInObject(analisiTerrenoOBJ);

                objResponse.validita.inizio = (objResponse.validita.inizio.getTime() == AGRODATAINIZIO.getTime()) ? null : objResponse.validita.inizio;
                objResponse.validita.fine = (objResponse.validita.fine.getTime() == AGRODATAFINE.getTime()) ? null : objResponse.validita.fine;
                this.analisiFormService.formAnalisiTerreno.patchValue(objResponse, { emitEvent: false });

                //il patchValue non filla tutti i campi del Form Array
                this.analisiFormService.fillCampioniArray(objResponse, (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) ? enum_AnalisiTerrenoGridCommands.INFO : enum_AnalisiTerrenoGridCommands.MODIFICA_COMPLETA);
                //mi salvo i parametri sull'array del serviceForm
                if (objResponse.dettagli)
                    this.analisiFormService.arrayParametri = objResponse.dettagli.map(item => {
                        return {
                            ...item.parametro,
                            codiceDettaglio: item.codice,
                            Valore: item.valore1,
                            Errore: item.valore2
                        }
                    })
                        .filter(item => item.codice !== 0);

                if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read)
                    this.analisiFormService.formAnalisiTerreno.disable();

                if (objResponse.analisiUtilizzata) {
                    this.analisiFormService.formAnalisiTerreno.get('laboratorio').disable();
                    this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').disable();
                    this.analisiFormService.modeAgganciataPC_PUA = true;
                }

                this.completeFormSettings();

            });
        } else {
            this.completeFormSettings();
        }

        this.handleQueryParams();

    }

    private async completeFormSettings(): Promise<void> {

        switch (this.objParametriAgenda.Pagina_Provenienza) {
            case enum_PagineGiasNG.Pagina_GIS:
                this.fromGIS = true;
            break;
            case enum_PagineGiasNG.Pagina_Analisi_Terreno_Menu:
                this.fromMenuGrid = true;
            break;
        }

        //per ora, in ogni caso, salto il controllo a BE su aggancio PUA/PC
        this.skipChecksAggancioPC_PUA = true;

        switch (this.objParametriAgenda.TipoOperazioneDB) {
            case Enum_DBTypeOperation.Update:
                this.expandedPanels();
                this.onEdit = true;
                break;

            case Enum_DBTypeOperation.Write:
                this.analisiFormService.formAnalisiTerreno.get('descrizione').patchValue(this.transloco.translate("AnalisiDelTerreno"));
                this.campioni.clear();
                break;

            case Enum_DBTypeOperation.Read:
                this.disabledDeleteCampione = true;
                this.onInfo = true;
                this.expandedPanels();
                break;
        }
        
        let pivaReale = await lastValueFrom(this.pivaRealeService.getPivaReale(this.objParametriAgenda.Piva));
        this.infoAzienda = this.objParametriAgenda.RagSoc + ' (' + this.transloco.translate('PartitaIVA') + ': ' + pivaReale + ')';

        this.analisiDataService.LeggiLaboratori().subscribe(r => {
            r.unshift({
                ragione_Sociale: '',
                risorseUmane: [{ codice: 0 }]
            });
            this.listaLaboratori = r;
        });

        this.loadSchemi();

        this.analisiFormService.formAnalisiTerreno.get('laboratorio').valueChanges.subscribe((value) => {
            this.loadSchemi();
        });

        this.analisiDataService.LeggiCentriEntita(this.objParametriAgenda).subscribe(r => {
            this.listaCentri = r;
        });

        this.analisiDataService.LeggiCatastoEntita(this.objParametriAgenda).subscribe(r => {
            this.listaCompletaParticelle = r.kendo_rows;
            r.kendo_rows.forEach(item => {
                let objCatasto = { descrizione: '', primaryKey: null };
                // objCatasto.chiave = item.chiave;
                let chiaveParticella = new ParticelleCatastali.PK(item.Prov, item.Com, item.SEZIONE, item.FOGLIO, item.NUMERO, item.SUBALTERNO);
                objCatasto.primaryKey = chiaveParticella;
                objCatasto.descrizione = item.Prov + " : " + item.Com + " : " + item.SEZIONE + " : " + item.FOGLIO + " : " + item.NUMERO + "/" + item.SUBALTERNO;
                this.listaCatasto.push(objCatasto);
            });
            let arrCampioni = this.analisiFormService.formAnalisiTerreno.get('campioni') as FormArray;
            arrCampioni.controls.forEach((itemGroup) => {
                let particella = itemGroup.get('particella')?.value;
                if (particella)
                    itemGroup.get('particella').setValue(this.listaCatasto.find(partListaCatasto => JSON.stringify(partListaCatasto.primaryKey) === JSON.stringify(particella.primaryKey)) ?? null);
            });
        });

        this.listaUdM.unshift({ codice: 0, descrizione: '' });

        this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').valueChanges.subscribe((value) => {

            //riabilito le date se sono in un contesto diverso da Info
            if (this.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Read)
                this.analisiFormService.formAnalisiTerreno.get('validita')?.enable({ emitEvent: false });

            if (value?.length > 0) {
                this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').patchValue(value.slice(value.length - 1), { emitEvent: false });

                if (value[value.length - 1] !== enum_Entita_Analisi.Impresa) {
                    this.showImpresa = false;
                    this.analisiFormService.arrayEntita = [];
                    this.reloadGridEntita();
                } else {
                    this.showGridEntita = false;
                    this.showImpresa = true;
                    //aggiorno l'array delle Entita
                    this.analisiFormService.arrayEntita = [];
                    let impresa = new Impresa();
                    impresa.partitaIva = this.objParametriAgenda.Piva;
                    this.analisiFormService.arrayEntita.push(impresa);
                }
            } else {
                this.showGridEntita = false;
                this.showImpresa = false;
            }

            this.gisGeometrySelectionService.resetFromOutside();
        });

        //gestiamo il pregresso nel caso ci siano più entità (analisi multi entità)

        let entitaCoinvolte = this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').getRawValue();
        if (entitaCoinvolte.length > 1) {
            entitaCoinvolte.forEach(item => {
                if (item == enum_Entita_Analisi.Impresa) {
                    this.showImpresa = true;
                } else {
                    this.reloadGridEntita();
                }
            });

            this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').disable({ emitEvent: false });
        } else
            this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte')?.updateValueAndValidity({ emitEvent: true });

        // ricaricare la griglia dei parametri
        this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').valueChanges.subscribe(r => {
            this.reloadGridParametri();
        });

        //stimolo
        this.analisiFormService.formAnalisiTerreno.get('analisiTipologia')?.updateValueAndValidity({ emitEvent: true });

        combineLatest([
            this.GisComponentSubject.asObservable(),
            this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').valueChanges.pipe(startWith(this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value)),
            this.analisiFormService.formAnalisiTerreno.get('validita')?.get('inizio').valueChanges.pipe(startWith(this.startDateForGis)),
            this.analisiFormService.formAnalisiTerreno.get('validita')?.get('fine').valueChanges.pipe(startWith(this.endDateForGis))
        ]).pipe(
            takeUntil(this.signal),
            filter(([gis, entitaCoinvolte, _1, _2]) => gis != null && entitaCoinvolte?.length > 0),
            tap(([_, entitaCoinvolte, startDate, endDate]) => this.handleEntitaCoinvolte(entitaCoinvolte, startDate, endDate))
        ).subscribe();
    }

    private handleQueryParams(): void {
        this.route.queryParams.pipe(takeUntil(this.signal))
            .subscribe(params => {
                if (params.seFrame == 1) {
                    this.openInIFrame = true;
                }
            });
    }

    private manageGisIntegration() {
        this.gisService.updateGeoJsonFilterServiceAnalisiTerreno(this.startDateForGis, this.endDateForGis, '0', '0', { codice: 0 } as UtilizzoTerreno);
        this.gisService.setFilterServiceQdcConPoligoni(true);
        this.gisService.initFullMapAnalisiTerreno();

        combineLatest([this.campioni.valueChanges, this.GisComponentSubject.asObservable().pipe(skip(1))])
            .pipe(takeUntil(this.signal))
            .subscribe(([_, gis]) => this.gisManageUpdateCampioni(gis))
    }

    private gisManageUpdateCampioni(gis: GISComponent): void {
        const campione = this.getCampione();
        if (campione == null) {
            return;
        }

        const gmap = gis.mapService.googleMapWrapper.data.getMap();
        const latLng = new google.maps.LatLng(campione.latitude, campione.longitude);
        this.updateGisMarker(gmap, latLng);

        //apro la tab dei campioni
        this.isExpandedCampione = true;
    }

    private getCampione(): Campione | null {
        if (this.campioni == null) {
            return null;
        }

        const campioni = this.campioni.value;
        if (campioni?.length == 0) {
            return null;
        }

        return campioni[0];
    }

    loadGis(gis: GISComponent): void {
        this.gisClickListener?.remove();

        const gmap = gis.mapService.googleMapWrapper.googleMap;
        this.gisClickListener = google.maps.event.addListener(gmap, "click", ev => this.handleClick(gis, gmap, ev));
        this.GisComponentSubject.next(gis);

        const campione = this.getCampione();
        if (campione != null && campione.latitude != 0 && campione.longitude != 0) {
            this.updateGisMarker(gmap, new google.maps.LatLng(campione.latitude, campione.longitude));
            const bound = new google.maps.LatLngBounds(new google.maps.LatLng(campione.latitude, campione.longitude));
            gis.geoJsonLazyLoaded$.pipe(take(1)).subscribe(() => gis.mapService.googleMapWrapper.fitBounds(bound));
        } else if (this.gisGeometrySelectionService.getCurrentSelection().length > 0) {
            gis.geoJsonLazyLoaded$.pipe(take(1)).subscribe(() => gis.centerMapOnSelectedFeatures());
        }
    }

    private handleClick(gis: GISComponent, gmap: google.maps.Map, ev: google.maps.MapMouseEvent): void {
        if (this.onInfo || !gis.isAnalisiTerrenoMarkerActive) {
            return;
        }

        this.updateCampione(ev.latLng);
        this.updateGisMarker(gmap, ev.latLng);
    }

    private updateCampione(latLng: google.maps.LatLng) {
        let campione = this.getCampione();
        if (campione == null) {
            const form = this.aggiungiCampione()
            campione = form.value;
        }

        campione.latitude = latLng.lat();
        campione.longitude = latLng.lng();
        campione.particella ??= null;

        //gestione pregresso multicampione, aggiornando sempre il primo campione disponibile
        this.campioni.setValue([campione, ...this.campioni.value.slice(1)]);
    }

    private updateGisMarker(gmap: google.maps.Map | null, latLng: google.maps.LatLng) {
        if (gmap == null && this.marker?.getMap() == null) {
            return;
        }

        this.marker?.setVisible(false);
        if (latLng.lat() == 0 && latLng.lng() == 0) {
            return;
        }

        this.marker = new google.maps.Marker({
            map: gmap ?? this.marker?.getMap(),
            draggable: false,
            visible: true,
            icon: {
                url: 'assets/custom-icons/icon-gis-draw/gm_d_gps_plants_red.svg',
                scaledSize: new google.maps.Size(50, 50),
                anchor: new google.maps.Point(25, 25),
            },
            position: latLng,
        });

        this.marker.addListener('click', () => {
            if (!this.onInfo && this.GisComponentSubject.value.isAnalisiTerrenoMarkerActive) {
                this.clearCampioneLatLong();
            }
        });
    }

    expandedPanels() {
        if (this.analisiFormService.formAnalisiTerreno.get('campioni')?.value?.length > 0)
            this.isExpandedCampione = true;
        if (this.analisiFormService.formAnalisiTerreno.get('laboratorio')?.value || this.analisiFormService.formAnalisiTerreno.get('analisiTipologia')?.value || this.analisiFormService.arrayParametri.length > 0)
            this.isExpandedParametri = true;
        if (this.analisiFormService.formAnalisiTerreno.get('note')?.value)
            this.isExpandedNote = true;
    }

    loadSchemi() {

        let paramForSchemiAnalisi = {
            Analisi_Tipologia_Cod: 0,
            Analisi_Tipo: enum_AnalisiTipo.Analisi_Terreno,
            Laboratorio: this.analisiFormService.formAnalisiTerreno.get('laboratorio')?.value?.risorseUmane[0]?.codice ?? 0,
            LeggiMetaschema: (this.analisiFormService.formAnalisiTerreno.get('laboratorio')?.value?.risorseUmane[0]?.codice) ? ((this.analisiFormService.formAnalisiTerreno.get('laboratorio')?.value?.risorseUmane[0]?.codice !== -1) ? false : true) : true
        } as LeggiSchemiAnalisi;

        this.analisiDataService.LeggiSchemiAnalisi(paramForSchemiAnalisi).subscribe(r => {
            r.unshift({ codice: 0, descrizione: '' });
            this.listaSchemiAnalisi = r;

            if (this.listaSchemiAnalisi.findIndex(item => item.codice == (this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').value?.codice ?? 0)) == -1)
                this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').patchValue(null);
        });
    }

    onOpenDocumentsList() {
        this.analisiDocumentsService.ApriKendoWindowRicercaDocumenti(this.analisiFormService.formAnalisiTerreno.get('codice').value, this.objParametriAgenda.Piva);
    }

    onNewDocument() {
        let dataFine = this.analisiFormService.formAnalisiTerreno.get('validita').get('fine').value;
        this.analisiDocumentsService.ApriKendoWindowAggiungiNuovoAllegato(this.analisiFormService.formAnalisiTerreno.get('codice').value, (dataFine) ? dataFine : AGRODATAFINE, this.objParametriAgenda.Piva);
    }

    SalvaEditAnalisiTerreno() {
        //prima di poter salvare l'Analisi, occorre aggiornare i parametri e le entità
        if (!this.analisiFormService.formAnalisiTerreno.valid) {
            this.analisiFormService.formAnalisiTerreno.markAllAsTouched();
            return;
        }

        if (this.analisiFormService.arrayEntita.length == 0 && this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value.length <= 1) {
            this.giasDialogService.baseError("",
                this.transloco.translate('SelezionareUnEntitaAnalisiTerreno')
                    .replace('{0}',
                        this.ListaEntita.find(item => item.id == this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value[0]).nome),
                false
            );
            return;
        }

        let analisiTerreno = this.conversionService.ConversionDateInObject(this.analisiFormService.formAnalisiTerreno.getRawValue());

        //sbianco l'array dei parametri
        analisiTerreno.dettagli = [];

        if (this.analisiFormService.arrayParametri.length > 0) {
            this.analisiFormService.arrayParametri.forEach(item => {
                let paramElem: AnalisiDettaglio = {};

                let analisiParam: AnalisiParametro = {};
                analisiParam.codice = item.codice;
                analisiParam.descrizione = item.descrizione;
                paramElem.parametro = analisiParam;
                paramElem.codice = item.codiceDettaglio;
                paramElem.valore1 = item.Valore.toString().replace(',', '.');
                paramElem.valore2 = item.Errore.toString().replace(',', '.');

                analisiTerreno.dettagli.push(paramElem);
            });
        }

        // sbianco gli array delle Entità, se non sono nel caso delle multientità
        if (analisiTerreno.entitaCoinvolte.length <= 1) {
            analisiTerreno.entitaImprese = [];
            analisiTerreno.entitaCentri = [];
            analisiTerreno.entitaCampi = [];
            analisiTerreno.entitaAppezzamenti = [];
            analisiTerreno.entitaImpianti = [];
            analisiTerreno.entitaFabbricati = [];
            analisiTerreno.entitaParticelleCatastali = [];
        }

        if (this.analisiFormService.arrayEntita.length > 0) {
            this.analisiFormService.arrayEntita.forEach(item => {

                // let chiaveCentroAziendale = new CentroAziendale.PK(item.sa_cod, item.PIVA);

                switch (analisiTerreno.entitaCoinvolte[0]) {    //da specifiche, c'è solo un'entità coinvolta

                    case enum_Entita_Analisi.Impresa:
                        let entitaImpresa: AnalisiEntita_1OfImpresa = {};
                        entitaImpresa.elementoAnagrafico = item;
                        analisiTerreno.entitaImprese.push(entitaImpresa);
                        break;

                    case enum_Entita_Analisi.Campo:
                        let entitaCampo: AnalisiEntita_1OfCampo = {};
                        let chiaveCACampo = new CentroAziendale.PK(item.sa_cod, item.PIVA);
                        let chiaveCampo = new Campo.PK(item.Campo_Cod, chiaveCACampo);
                        entitaCampo.elementoAnagrafico = new Campo(chiaveCampo);
                        analisiTerreno.entitaCampi.push(entitaCampo);
                        break;

                    case enum_Entita_Analisi.Centro:
                        let entitaCentro: AnalisiEntita_1OfCentroAziendale = {};
                        let chiaveCentroAziendale = new CentroAziendale.PK(item.sa_cod, item.Piva);
                        let centroAziendaleForBE = { primaryKey: chiaveCentroAziendale };
                        entitaCentro.elementoAnagrafico = centroAziendaleForBE;
                        // entitaCentro.elementoAnagrafico = new CentroAziendale(chiaveCentroAziendale);
                        analisiTerreno.entitaCentri.push(entitaCentro);
                        break;

                    case enum_Entita_Analisi.Appezzamento:
                        let entitaAppezzamento: AnalisiEntita_1OfAppezzamento = {};
                        let chiaveCAAppezzamento = new CentroAziendale.PK(item.Sa_Cod, item.Piva);
                        let chiaveAppezzamento = new Appezzamento.PK(item.Appezza, chiaveCAAppezzamento);
                        let appezzamentoForBE = { primaryKey: chiaveAppezzamento };
                        entitaAppezzamento.elementoAnagrafico = appezzamentoForBE;
                        // entitaAppezzamento.elementoAnagrafico = new Appezzamento(chiaveAppezzamento) as Appezzamento;
                        analisiTerreno.entitaAppezzamenti.push(entitaAppezzamento);
                        break;

                    case enum_Entita_Analisi.Impianto:
                        let entitaImpianto: AnalisiEntita_1OfImpianto = {};
                        let chiaveCAImpianto = new CentroAziendale.PK(item.SA_COD, item.PIVA);
                        let chiaveAppezzamentoImpianto = new Appezzamento.PK(item.APPEZZA, chiaveCAImpianto);
                        let chiaveImpianto = new Impianto.PK(item.id_Reg, chiaveAppezzamentoImpianto);
                        let impiantoForBE = { primaryKey: chiaveImpianto };
                        entitaImpianto.elementoAnagrafico = impiantoForBE;
                        // entitaImpianto.elementoAnagrafico = new Impianto(chiaveImpianto);
                        analisiTerreno.entitaImpianti.push(entitaImpianto);
                        break;

                    case enum_Entita_Analisi.Particella:
                        let entitaParticella: AnalisiEntita_1OfCatastoCentroAziendale = {};
                        let chiaveParticella = new ParticelleCatastali.PK(item.Prov, item.Com, item.SEZIONE, item.FOGLIO, item.NUMERO, item.SUBALTERNO);
                        let particella = new ParticelleCatastali();
                        particella.primaryKey = chiaveParticella;
                        let chiaveCAParticella = new CentroAziendale.PK(item.Sa_Cod, item.Piva);
                        entitaParticella.elementoAnagrafico = new CatastoCentroAziendale();
                        entitaParticella.elementoAnagrafico.centro = chiaveCAParticella;
                        entitaParticella.elementoAnagrafico.particella = particella;
                        analisiTerreno.entitaParticelleCatastali.push(entitaParticella);
                        break;
                }

            });
        }

        //valorizzo i parametri che non accettano nullable
        if (!analisiTerreno.validita.inizio)
            analisiTerreno.validita.inizio = AGRODATAINIZIO;

        if (!analisiTerreno.validita.fine)
            analisiTerreno.validita.fine = AGRODATAFINE;

        if (analisiTerreno.laboratorio && analisiTerreno.laboratorio.data_Nascita)
            analisiTerreno.laboratorio.data_Nascita = AGRODATAINIZIO;

        if (!analisiTerreno.certificatoAnalisi.codice)
            analisiTerreno.certificatoAnalisi.codice = 0;

        let payload: ScriviAnalisiTerreno = {};
        payload.Piva = this.objParametriAgenda.Piva;
        payload.CUAA = '';
        payload.saltaControlliAggancio = this.skipChecksAggancioPC_PUA;
        payload.AnalisiTerreno = analisiTerreno;

        this.loadingService.set_isLoading({ isLoading: true, component: this.elementRef });
        this.analisiDataService.ScriviAnalisiTerreno(payload).pipe(catchError(error => {
            this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });
            this.giasDialogService.baseError("", FunzioniComuniService.getResponseError(error, this.transloco, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat'), false);
            return of(null);
        })).subscribe(async r => {

            this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });

            //sbianco GenericObj_string una volta salvato
            let objParam = this.objParametriAgendaService.getObjParamValue();
            objParam.GenericObj_string = '';
            this.objParametriAgendaService.changeObjParametriAgenda(objParam);

            if (r.RispostaOK) {

                if (!this.openInIFrame || (this.openInIFrame && this.fromMenuGrid)) {
                    //se siamo in scrittura -> apro documentale
                    if (!this.onEdit && this.permessoScritturaDoc) {
                        let window = await this.analisiDocumentsService.ApriKendoWindowAggiungiNuovoAllegato(r.RispostaStringa.codice, analisiTerreno.validita.fine, this.objParametriAgenda.Piva);

                        window.result.subscribe(closeEv => {
                            this.externalNavigationService.goBack();
                            this.location.back();
                        });

                    } else {
                        //torniamo indietro si siamo in modifica e non abbiamo il permesso per la scrittura del documento (l'utente ha possibilità di aprirlo quando vuole dalla pagina)
                        this.externalNavigationService.goBack();
                        this.location.back();
                    }
                } else {

                    let objPostMessage = {
                        messaggio: "chiudiWindowGiasNG",
                        contesto: 3,
                        inData: null
                    };

                    window.parent.postMessage(objPostMessage, this.funzionicomuniservice.getOrigins());
                }

            }

        });
    }

    getTitlePanel(id: enum_Entita_Analisi): string {
        return this.transloco.translate(this.ListaEntita.find(item => item.id == id).nome);
    }

    SalvaAnalisiTerreno() {
        console.log('SalvaAnalisiTerreno');
    }

    reloadGridEntita() {
        this.showGridEntita = false;
        setTimeout(() => this.showGridEntita = true, 0);
    }

    reloadGridParametri() {
        this.showGridParametri = false;
        setTimeout(() => this.showGridParametri = true, 0);
    }

    removeCampione(index: number): void {
        this.campioni.removeAt(index);

        this.marker?.setVisible(false);
        this.marker = null;
    }

    clearCampioneLatLong(): void {
        const campione = this.getCampione();
        if (campione != null) {
            campione.latitude = 0;
            campione.longitude = 0;
            campione.particella ??= null;

            this.campioni.setValue([campione]);
        }

        this.marker?.setVisible(false);
        this.marker = null;
    }

    aggiungiCampione(): FormGroup {
        const elemFormArr = this.fb.group(
            {
                longitude: [0],
                latitude: [0],
                quantita: [0],
                unitaMisura: [null],
                profondita: [0],
                profonditaMin: [0],
                profonditaMax: [0],
                riferimento1: [null],
                riferimento2: [null],
                riferimento3: [null],
                riferimento4: [null],
                riferimento5: [null],
                note: [null],
                dataPrelievo: [new Date(), Validators.required],
                KeyPiva: [null],
                KeySaCod: [0],
                KeyGrafica: [null],
                particella: [null],
                codice: [0],
                descrizione: [null]
            }
        );

        this.campioni.push(elemFormArr);
        return elemFormArr;
    }

    disabilita_Btn_AggiungiCampione() {
        return (this.campioni.length >= 1 || this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read);
    }

    ngOnDestroy(): void {
        this.analisiFormService.arrayEntita = [];
        this.analisiFormService.arrayParametri = [];

        this.campioni.clear();

        this.unsubscribeFromGIS();
        this.signal.next();
        this.signal.complete();
    }

    private unsubscribeFromGIS(): void {

        this.gisGeometrySelectionService.resetFromOutside();

        const gis = this.GisComponentSubject.value;
        gis?.removeMarkers();
        gis?.abilitaDeselezione();

        this.gisClickListener?.remove();
        this.gisClickListener = null;

        this.marker?.setVisible(false);
        this.marker = null;
    }

    private handleEntitaCoinvolte(entitaCoinvolte: number[], startDate: Date, endDate: Date): void {
        if (entitaCoinvolte.length == 0) {
            return;
        }

        let layer = enum_LayerElementiGraficiStd.IMPIANTI;
        const entita = entitaCoinvolte[entitaCoinvolte.length - 1];
        if (entita == enum_Entita_Analisi.Impresa || entita == enum_Entita_Analisi.Centro) {
            layer = enum_LayerElementiGraficiStd.Centri_Aziendali;
        } else if (entita == enum_Entita_Analisi.Appezzamento) {
            layer = enum_LayerElementiGraficiStd.APPEZZAMENTI;
        } else if (entita == enum_Entita_Analisi.Impianto) {
            layer = enum_LayerElementiGraficiStd.IMPIANTI;
        } else if (entita == enum_Entita_Analisi.Campo) {
            layer = enum_LayerElementiGraficiStd.CAMPI;
        } else if (entita == enum_Entita_Analisi.Particella) {
            layer = enum_LayerElementiGraficiStd.CAMPIONAMENTI;
        }

        this.gisService.updateGeoJsonFilterServiceAnalisiTerreno(startDate, endDate, '0', '0', { codice: 0 } as UtilizzoTerreno, layer);
    }

    onClickBack() {
        if (this.externalNavigationService.isBackInvalid) {
            this.giasDialogService.baseError("",
                this.transloco.translate('NonPuoiUtilizzareQuestaFunzionalitaInQuestoPuntoUsaIlMenuLateralePerLaNavigazione'), false);
            return;
        }
        this.externalNavigationService.goBack();
        this.location.back();
    }

    private get startDateForGis() {
        return this.analisiFormService.formAnalisiTerreno.get('validita')?.get('inizio')?.value ?? new Date(AGRODATAINIZIO);
    }

    private get endDateForGis() {
        return this.analisiFormService.formAnalisiTerreno.get('validita')?.get('fine')?.value ?? new Date(AGRODATAFINE);
    }
}
