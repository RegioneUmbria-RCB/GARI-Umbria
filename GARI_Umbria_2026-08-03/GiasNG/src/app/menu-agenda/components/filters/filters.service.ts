import { DatePipe } from '@angular/common';
import { Injectable, OnDestroy } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO, NessunaSpecieQdC } from 'app/Model/CostantiPersonalizzate';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Menu_Agenda_NG_Mode } from '../../../Model/TipiEnumerativi';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { MasterService } from 'app/Service/master.service';
import { HttpService } from 'app/Service/http.service';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { DestinazioneUso, ObjParametriAgenda } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { ActivatedRoute } from '@angular/router';
import { AgendaClient, LeggiSpecieQdC, UtilizzoTerreno, ChiaveImpianto, Varieta } from 'app/Service/api.service';
import { ImpostazioniApp, CentroItem, SpecieItem, OperazioneItem, ImpiantoItem, DDLs, IMPIANTI_LNK, OPERAZIONI_LNK, SPECI_LNK, CENTRI_LNK, ImpiantiParams, OperazioniParams, GetOperazioniParams, CentriParams, GetCentriParams, CentriParams_NG, GetCentriParams_NG } from 'gias-kendo-grid';
import { BehaviorSubject, Subscription, forkJoin, take, Observable, map, switchMap, of } from 'rxjs';
import { FiltersConfig } from '../models';
import { Filters } from "../utils";
import { NgxCookieService } from "../../../Service/ngx-cookie.service";

//const LinkImpostazioniApp_Old = '/Agenda/MenuBS_WS.asmx/CaricaImpostazioniApp';

const LinkImpostazioniApp = 'Agenda/CaricaImpostazioniApp';

@Injectable({ providedIn: 'root' })
export class FiltersService implements OnDestroy {
    // Filters' form group
    public fg: FormGroup = new FormGroup({
        CentroAziendale: new FormControl(''),
        Specie: new FormControl(''),
        TipoOperazione: new FormControl([]),
        Impianti: new FormControl([]),
        Da: new FormControl(''),
        A: new FormControl('')
    });

    public nascondiFiltri: boolean = true;
    public impostazioniApp: ImpostazioniApp;
    public reloadFromAgenda: BehaviorSubject<Boolean> = new BehaviorSubject(false);
    private filters: BehaviorSubject<FiltersConfig> = new BehaviorSubject(null);

    //
    public CentriBS: BehaviorSubject<CentroItem[]> = new BehaviorSubject<CentroItem[]>([]);
    public SpecieBS: BehaviorSubject<SpecieItem[]> = new BehaviorSubject<SpecieItem[]>([]);
    public OperazioniBS: BehaviorSubject<OperazioneItem[]> = new BehaviorSubject<OperazioneItem[]>([]);
    public ImpiantiBS: BehaviorSubject<ImpiantoItem[]> = new BehaviorSubject<ImpiantoItem[]>([]);
    //public TipiVisitaBS: BehaviorSubject<TipoVisitaItem[]> = new BehaviorSubject<TipoVisitaItem[]>([]);

    // Liste DropDown
    public Centri: CentroItem[];
    public Specie: SpecieItem[];
    public Operazioni: OperazioneItem[];
    public Impianti: ImpiantoItem[];
    //public TipiVisita: TipoVisitaItem[];

    private objParametriAgenda: ObjParametriAgenda;
    private user;

    // Valori selezionati
    private da: Date;
    private a: Date;
    private centro: CentroItem;
    private specie: SpecieItem;
    private operazioni: OperazioneItem[];
    private impianti: ImpiantoItem[];

    /*
    * @description: Modalità con cui è stato aperto il menu agenda
    * */
    private _Mode: BehaviorSubject<enum_Menu_Agenda_NG_Mode> = new BehaviorSubject<enum_Menu_Agenda_NG_Mode>(enum_Menu_Agenda_NG_Mode.Standard);

    constructor(
        private route: ActivatedRoute,
        private objParametriAgendaService: ObjParametriAgendaService,
        private agendaClient: AgendaClient,
        private httpService: HttpService,
        private PermessiUtenteService: PermessiUtenteService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        public datepipe: DatePipe,
        private masterService: MasterService,
        private transloco: TranslocoService,
        private ngxCookieService: NgxCookieService
    ) {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.user = this.PermessiUtenteService.getCurrentUser();
        this.init();
        this.reloadFromAgenda = new BehaviorSubject<Boolean>(false);
        this.handleReload();
        this.filters.next(this.inizializzaFilterConfig());
        this.handleSubscriptions();
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }

    private retry;
    private sub: Subscription;
    private handleReload() {
        let success = true;
        try {
            this.sub = this.reloadFromAgenda.subscribe((val: boolean) => {
                if (!val) return;
                this.init();
            });
        } catch (e) {
            success = false;
            if (!this.retry) this.retry = setInterval(() => {
                this.handleReload();
            }, 500);
        }
        if (success && !!this.retry) clearInterval(this.retry);
    }

    get Mode() {
        return this._Mode.getValue();
    }
    set Mode(value: enum_Menu_Agenda_NG_Mode) {
        this._Mode.next(value);
    }

    // Cookies' name generator
    get _validitaInizio() { return `MenuAgenda_Da_${this.user?.Username ?? 'guest'}`; }
    get _validitaFine() { return `MenuAgenda_A_${this.user?.Username ?? 'guest'}`; }
    get _centro() { return `MenuAgenda_Centro_${this.user?.Username ?? 'guest'}`; }
    get _specie() { return `MenuAgenda_Specie_${this.user?.Username ?? 'guest'}`; }
    get _operazioni() { return `MenuAgenda_Op_${this.user?.Username ?? 'guest'}`; }
    get _impianti() { return `MenuAgenda_Impianti_${this.user?.Username ?? 'guest'}`; }

    private get filtersSource() {
        let src = this.filters.value;
        if (src === undefined || src === null) {
            src = this.inizializzaFilterConfig();
        }
        return src;
    }

    public init(): void {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        if (this.objParametriAgenda.Piva !== "") {
            if (this.nascondiFiltri
                && this.objParametriAgenda.Veg_Cod === 0 && this.objParametriAgenda.Id_Cod === 0
                && this.objParametriAgenda.Impianti?.length > 0) {
                this.leggiUtilizzoTerrenoImpianto();
            } else {
                this.completaInit();
            }
        }
    }

    public restoreCurrentValues() {
        this.fg.patchValue({
            CentroAziendale: this.centro,
            Specie: this.specie,
            TipoOperazione: this.operazioni,
            Impianti: this.impianti,
            Da: this.da,
            A: this.a
        });
    }

    public applyFilters(newConfig: FiltersConfig = this.filters.getValue()) {
        this.filters.next(newConfig);
    }

    public subscribeToFiltersChange() {
        return this.filters.asObservable();
    }

    public filtersGetValue() {
        return this.filters.value;
    }

    public set_filters(val: FiltersConfig) {
        this.filters.next(val);
    }

    public emptySelectedImpianti() { this.impianti = []; }
    public emptySelectedOperazioni() { this.operazioni = []; }

    public inizializzaFilterConfig(): FiltersConfig {
        return {
            Da: AGRODATAINIZIO,
            A: AGRODATAFINE,
            CentroAziendale: { sa_nome: '', sa_cod: '' },
            Specie: { veg_cod: '', veg_des: '' },
            TipoOperazione: [],
            Impianti: [],
            MostraDDT: false,
            TipoVisita: null
        };
    }

    public getQdCTable(filtersConfig?: FiltersConfig | undefined): Filters {
        let src: FiltersConfig = null;

        if(filtersConfig !== undefined){
          src = filtersConfig;
        }else{
          src = this.filtersSource;
        }

        let adjustedVegCod = src.Specie.veg_cod;
        let flag_TerrenoNudo: boolean = false;
        let flag_NessunaSpecieQdC: boolean = false;

        if (adjustedVegCod === (NessunaSpecieQdC).toString()) {
            adjustedVegCod = "0";
            flag_NessunaSpecieQdC = true;
        } else if (adjustedVegCod.startsWith('-') && adjustedVegCod != '-1') {
            adjustedVegCod = adjustedVegCod.replace('-', '');
            flag_TerrenoNudo = true;
        }

        const filters = new Filters({
            TipoGriglia: '2',
            xFiltroAggiuntivo_colturali: '',
            txt_Data1: this.datepipe.transform(src.Da, 'dd/MM/yyyy'),
            txt_Data2: this.datepipe.transform(src.A, 'dd/MM/yyyy'),
            flag_TerrenoNudo: flag_TerrenoNudo,
            sa_cod: src.CentroAziendale.sa_cod,
            veg_cod: adjustedVegCod,
            mode: this.Mode,
            ricetta_cod: this.objParametriAgendaService.getObjParamValue().Ricetta_Cod.toString(),
            tipoOperazione: src.TipoOperazione.map(s => s.gru_cod),
            impianti: src.Impianti?.map(s => s.chiave),
            flag_NessunaSpecieQdC: flag_NessunaSpecieQdC
        });

        return filters;
    }

    public getFiltriBrogliaccio() {
        let src = this.filtersSource;
        return {
            txt_Data1: this.datepipe.transform(src.Da, 'dd/MM/yyyy'),
            txt_Data2: this.datepipe.transform(src.A, 'dd/MM/yyyy'),
            sa_cod: src.CentroAziendale.sa_cod,
            veg_cod: src.Specie.veg_cod,
            stato: 301
        };
    }

    public getFiltriRicette() {
        let src = this.filtersSource;
        return {
            txt_Data1: this.datepipe.transform(src.Da, 'dd/MM/yyyy'),
            txt_Data2: this.datepipe.transform(src.A, 'dd/MM/yyyy'),
            sa_cod: src.CentroAziendale.sa_cod,
            veg_cod: src.Specie.veg_cod,
            stato: 300,
            tipoOperazione: src.TipoOperazione.map(s => s.gru_cod),
            impianti: src.Impianti.map(s => s.chiave)
        };
    }

    public getFiltriZoo() {
        let src = this.filtersSource;
        return {
            txt_Data1: this.datepipe.transform(src.Da, 'dd/MM/yyyy'),
            txt_Data2: this.datepipe.transform(src.A, 'dd/MM/yyyy'),
        };
    }

    /// Caricamento dropdown lists.
    public CaricaDDLs() {
        const centri = this.CaricaCentri();
        const speci = this.CaricaSpeci();
        const tipoOperazione = this.CaricaTipoOperazione();
        const impianti = this.CaricaImpianti();
        // const tipiVisita = this.CaricaTipiVisita();
        const arr = [centri, speci, tipoOperazione, impianti];

        forkJoin(arr).pipe(take(1)).subscribe((data: DDLs) => {
            this.Centri = data[0] as CentroItem[];
            this.Specie = data[1] as SpecieItem[];
            this.Operazioni = data[2] as OperazioneItem[];
            this.Impianti = data[3] as ImpiantoItem[];
            //this.TipiVisita = data[4] as TipoVisitaItem[];
            // this.TipiVisita = this.getArrayTipiVisita();

            this.CentriBS.next(this.Centri);
            this.SpecieBS.next(this.Specie);
            this.OperazioniBS.next(this.Operazioni);
            this.ImpiantiBS.next(this.Impianti);
            // this.TipiVisitaBS.next(this.TipiVisita);
        });
    }

    public CaricaImpianti(): Observable<ImpiantoItem[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost(
            IMPIANTI_LNK,
            this.ImpiantiParams(this.objParametriAgendaService, this.masterService)
        ).pipe(map(r => r.RispostaStringa)) as Observable<ImpiantoItem[]>;
    }

    public readCookies() {
        const defaultSpecie: SpecieItem = { veg_des: 'Tutte le Specie', veg_cod: '-1' };
        const defaultCentri: CentroItem = { sa_nome: 'Tutti i Centri Aziendali', sa_cod: '0' };
        this.da = AGRODATAINIZIO;
        this.a = AGRODATAFINE;

        this.centro = {
            sa_cod: this.objParametriAgenda.Sa_Cod.toString(),
            sa_nome: this.objParametriAgenda.SaNome
        };
        this.specie = {
            veg_cod: this.objParametriAgenda.Veg_Cod.toString(),
            veg_des: this.objParametriAgenda.Veg_Des?.toString()
        };

        if (this.impianti.length == 0) { // leggo i dati dai cookies
            //let cookie: string = this.cookieService.getCookie(this._validitaInizio);

            let cookie: string = this.ngxCookieService.get(this._validitaInizio);
            if (cookie) {
                this.da = new Date(cookie);
            }
            cookie = this.ngxCookieService.get(this._validitaFine);
            if (cookie) {
                this.a = new Date(cookie);
            }
            if (this.da <= AGRODATAINIZIO && this.a >= AGRODATAFINE) {
                let actualDate = new Date();
                let aBridge = new Date(actualDate.getFullYear(), actualDate.getMonth(), 1, 0, 0, 0, 0);
                aBridge.setMonth(aBridge.getMonth() + 1);
                aBridge.setDate(aBridge.getDate() - 1);
                this.a = aBridge;
                this.da = new Date(actualDate.getFullYear(), 0, 1, 0, 0, 0, 0);
            }

            cookie = this.ngxCookieService.get(this._centro);
            if (cookie) {
                this.centro = JSON.parse(cookie);
            }

            cookie = this.ngxCookieService.get(this._impianti);
            if (cookie) {
                this.impianti = JSON.parse(cookie);
            }

            cookie = this.ngxCookieService.get(this._specie);
            if (cookie) {
                this.specie = JSON.parse(cookie);
            } else this.specie = defaultSpecie;

            cookie = this.ngxCookieService.get(this._operazioni);
            if (cookie) {
                this.operazioni = JSON.parse(cookie);
            }
        }

        if (!this.centro || parseInt(this.centro.sa_cod) == 0) {
            this.centro = defaultCentri;
        }
        if (!this.specie || parseInt(this.specie.veg_cod) === -1) {
            this.specie = defaultSpecie;
        }
        if (!this.operazioni) {
            this.operazioni = [];
        }
    }

    // PRIVATE Functions

    private handleSubscriptions() {
        // Carica dropdown quando la pagina viene caricata.
        this.CentriBS.pipe(take(1)).subscribe((data: CentroItem[]) => this.Centri = data);
        this.SpecieBS.pipe(take(1)).subscribe((data: SpecieItem[]) => this.Specie = data);
        this.OperazioniBS.pipe(take(1)).subscribe((data: OperazioneItem[]) => this.Operazioni = data);
        this.ImpiantiBS.pipe(take(1))
            .subscribe((data: ImpiantoItem[]) => {
                this.Impianti = data.sort((a, b) => a.des < b.des ? -1 : 1);
                this.preparaImpiantiConDescrizione(data, true)
                let impSel = this.fg.get("Impianti").getRawValue();
                let impiantiArr = [];
                impSel.forEach((el) => {
                    let impianto = this.Impianti.find((i) => i.chiave == el.chiave);
                    impiantiArr.push(impianto);
                })
                this.fg.get("Impianti").setValue(impiantiArr);
            });
    }

    private CaricaTipoOperazione(): Observable<OperazioneItem[]> {
        return this.httpService.post_legacy_API(
            OPERAZIONI_LNK,
            this.OperazioniParams.bind(this),
            false
        ).pipe(take(1)) as Observable<OperazioneItem[]>;
    }

    private CaricaSpeci(): Observable<SpecieItem[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiSpecieQdC, UtilizzoTerreno[]>(
            SPECI_LNK,
            this.SpeciParams(this.objParametriAgendaService, this.masterService)
        ).pipe(map(r => {
            const result: SpecieItem[] = [];
            for (const utilizzoTerreno of r.RispostaStringa) {
                if (utilizzoTerreno.classType === 'Varieta') {
                    const varieta = utilizzoTerreno as Varieta
                    result.push({ veg_cod: varieta.specie.codice.toString(), veg_des: varieta.specie.descrizione });
                } else {
                    // Destinazione d'uso --> necessario attivare flag_TerrenoNudo in filtri di ricerca per leggere
                    // correttamente le operazioni. Il '-' è stato messo per non confondere il record con le varietà.
                    result.push({ veg_cod: '-' + utilizzoTerreno.codice.toString(), veg_des: utilizzoTerreno.descrizione });
                }
            }
            result.unshift({ veg_cod: NessunaSpecieQdC.toString(), veg_des: this.transloco.translate('NessunaSpecie') });
            return result;
        }));
    }

    private CaricaCentri(): Observable<CentroItem[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
            CENTRI_LNK,
            this.CentriParams_NG(this.objParametriAgendaService, this.masterService)
        ).pipe(take(1), map((r) => r.RispostaStringa)) as Observable<CentroItem[]>;
    }

    /// Configurazione parametri della chiamata http
    private ImpiantiParams(...args): ImpiantiParams {
        const agenda = args[0];
        const master = args[1];
        const fg = this.fg.getRawValue();
        const piva = agenda.getObjParamValue().Piva;
        const centro = fg?.CentroAziendale;
        const specie = fg?.Specie;
        const objPServer = master.ObjParametri_Server;
        const params = this.GetImpiantiParams(objPServer, piva, centro, specie);
        return params;
    }

    private GetImpiantiParams(objPServer: string, piva: string, centro: CentroItem, specie: SpecieItem): ImpiantiParams {
        const dataInizio = this.filters.value?.Da ?? AGRODATAINIZIO;
        const dataFine = this.filters.value?.A ?? AGRODATAFINE;
        if (specie == null) {
            specie = { veg_cod: '0', veg_des: '' };
        }

        let sa_cod = '0';
        let veg_cod = '0';
        if (centro != undefined && centro.sa_cod != undefined) {
            sa_cod = centro.sa_cod;
        }
        if (specie != undefined && specie.veg_cod != undefined) {
            veg_cod = specie.veg_cod;
        }

        return {
            piva: piva,
            Sa_Cod: sa_cod,
            Veg_Cod: veg_cod,
            Data_Inizio: dataInizio,
            Data_Fine: dataFine
        };
    }

    private OperazioniParams(...args): OperazioniParams {
        const master = args[1];
        const objServer = master.ObjParametri_Server;
        const params = GetOperazioniParams(objServer);
        return params;
    }

    private SpeciParams(...args): LeggiSpecieQdC {
        const objParams = this.objParametriAgendaService.getObjParamValue();
        const impresa = new Impresa();
        impresa.partitaIva = objParams.Piva;
        impresa.ragioneSociale = objParams.RagSoc;
        const centroAziendale = new CentroAziendale({ codice: objParams.Sa_Cod, partitaIva: objParams.Piva });

        return {
            centroAziendale: centroAziendale as any,
            data: new Date(),
            impresa: impresa as any,
            consideraTerrenoNudo: true,
            soloAttiviAllaData: false
        } as LeggiSpecieQdC;
    }

    private CentriParams_NG(...args): CentriParams_NG {
        const agenda = args[0];
        const master = args[1];
        const piva = agenda.getObjParamValue().Piva;
        return GetCentriParams_NG(master.ObjParametri_Server, piva);
    }

    CaricaParametriImpostazioniApp() {
        this.ajaxAgronicaAPIService.ajaxAPIGet<any, ImpostazioniApp>(LinkImpostazioniApp, "")
            .pipe(take(1))
            .subscribe(resp => this.impostazioniApp = resp.RispostaStringa);
    }

    private leggiUtilizzoTerrenoImpianto() {
        const payload = {
            piva: this.objParametriAgenda.Piva,
            saCod: this.objParametriAgenda.Sa_Cod,
            appezza: this.objParametriAgenda.Appezza,
            idReg: this.objParametriAgenda.Id_Reg
        } as ChiaveImpianto;

        this.agendaClient
            .agendaLeggiUtilizzoTerrenoImpianto(payload)
            .subscribe(result => {
                let utilizzoTerreno = result.RispostaStringa;
                if (utilizzoTerreno.classType === "Varieta") {
                    const varieta = <Varieta>utilizzoTerreno;
                    this.objParametriAgenda.Veg_Cod = varieta.specie.codice;
                } else {
                    const destinazioneUso = <DestinazioneUso>utilizzoTerreno;
                    this.objParametriAgenda.Id_Cod = destinazioneUso.codice;
                }
                this.completaInit();
            });
    }

    private completaInit() {
        let impianti: ImpiantoItem[] = [];
        this.preparaImpiantiConDescrizione(impianti, false);
        this.impianti = impianti;

        this.readCookies();
        this.initializeQueryFilters(this.centro, this.specie, this.operazioni, this.impianti, this.da, this.a);
        this.CaricaDDLs();
    }

    private preparaImpiantiConDescrizione(impianti: ImpiantoItem[], descrizione: boolean): void {
        if (!this.objParametriAgenda.Impianti || !this.nascondiFiltri) return;

        const getPlotCode = (impianto) => `${impianto.Piva}_${impianto.Sa_Cod}_${impianto.Appezza}_${impianto.Id_Reg}`;
        this.objParametriAgenda.Impianti.forEach(impianto => {
            impianti[this.objParametriAgenda.Impianti.indexOf(impianto)] = {
                chiave: getPlotCode(impianto),
                des: descrizione ? this.Impianti.find(t => t.chiave == getPlotCode(impianto))?.des : ""
            } as ImpiantoItem;
        });
    }

    private initializeQueryFilters(centro: any, specie: any, tipoOperazione: any, impianti: any, da: any, a: any): void {
        this.route.queryParams.subscribe(data => {
            this.fg.get('CentroAziendale').patchValue(centro);
            this.fg.get('Specie').patchValue(specie);
            this.fg.get('TipoOperazione').patchValue(tipoOperazione);
            this.fg.get('Impianti').patchValue(impianti);
            this.fg.get('Da').patchValue(da);
            this.fg.get('A').patchValue(a);
            const values = this.fg.value;
            this.applyFilters(values);
        });
    }

}
