import { Injectable, OnDestroy } from "@angular/core";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { Operatore } from "app/Model/metaschema/utilizzi/Operatore";
import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { ImpiantiService, LeggiImpianto } from "app/Service/Anagrafica/impianti.service";
import { SpecieVegetaliService } from "app/Service/Metaschema/specie-vegetali.service";
import { AziendaDDLService, LeggiAziende } from "app/quaderno-di-campagna/agenda-edit/service/testata/azienda-ddl-service";
import { OperatoreDDLService } from "app/quaderno-di-campagna/agenda-edit/service/testata/operatore-ddl-service";
import { BehaviorSubject, forkJoin, lastValueFrom, map } from "rxjs";
import { FiltersConfig } from "./utilities";
import { CentriAziendaliService, LeggiCentriAziendali} from 'app/Service/Anagrafica/centri.service';
import { UtilizzoTerreno } from "app/Model/metaschema/utilizzi/UtilizzoTerreno";
import { Varieta } from "app/Model/metaschema/utilizzi/Varieta";
import { CentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { ImpiantoItem } from "app/menu-agenda/components/utils";
import { TranslocoService } from "@jsverse/transloco";
import { DestinazioneUso } from "app/Model/metaschema/utilizzi/DestinazioneUso";
import { AttivitaPersonalizzataService } from "app/Service/Agenda/attivita-personalizzata.service";
import { AttivitaPersonalizzata } from "app/Model/attivita/AttivitaPersonalizzata";
import { DropdownListSpecieAnimali, SpecieAnimaliService } from "app/quaderno-di-campagna/agenda-edit/service/testata/specie-animali-service";
import { RisorsaZootecnica } from "app/Model/attivita/risorse/RisorsaZootecnica";
import { DestinazioneUsoService } from "app/Service/Metaschema/destinazioneUso.service";

export class TipoVisita {
    key: number;
    value: string;

    constructor(k: number, v: string) {
        this.key = k;
        this.value = v;
    }
}

@Injectable({providedIn: 'root'})
export class FiltersServiceVisite implements OnDestroy {

    public operatoriVisite: Operatore[] = [];
    public aziendeVisite: Impresa[] = [];
    public specieVisite: UtilizzoTerreno[] | Specie[] = [];
    public centriAziendaliVisite: CentroAziendale[] = [];
    public impiantiVisite: ImpiantoItem[] = [];
    public operazioniVisite: AttivitaPersonalizzata[] = [];
    public tipoVisita: TipoVisita[] = [];
    public SpecieAnimaliVisita: DropdownListSpecieAnimali[] = [];

    private filters: BehaviorSubject<FiltersConfig> = new BehaviorSubject(null);

    constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private operatoreService: OperatoreDDLService,
                private aziendaService: AziendaDDLService,
                private specieService: SpecieVegetaliService,
                private destUsoService: DestinazioneUsoService,
                private attivitapersonalizzataservice: AttivitaPersonalizzataService,
                private impiantiservice: ImpiantiService,
                private centriservice: CentriAziendaliService,
                private translocoService: TranslocoService,
                private specieAnimaliService: SpecieAnimaliService) {

                    this.filters.next(this.inizializzaFilterConfig());

    }

    ngOnDestroy(): void {
        throw new Error("Method not implemented.");
    }

    public applyFilters(newConfig: FiltersConfig = this.filters.getValue()) {
        this.filters.next(newConfig);
    }

    public subscribeToFiltersChange() {
          return this.filters.asObservable();
    }

    public inizializzaFilterConfig(): FiltersConfig {
        return {
            OperatoreVisita: null,
            Aziende_Agenzie: null,
            AziendeVisita: null,
            CentroAziendaleVisita: null,
            Visualizza_Specie: null,
            SpecieVisita: null,
            Da: AGRODATAINIZIO,
            A: AGRODATAFINE,
            TipoOperazione: [],
            Impianti: [],
            TipoVisita: null,
            SpecieAnimaliVisita: null,
            WithDettaglioRilievo: false
        };
    }

    public filtersGetValue() {
        return this.filters.value;
    }

    // gli operatori vanno caricati per primi, e in seguito vanno aggiunte le altre
    async caricaDDLOperatori() {
        let result = await lastValueFrom(this.operatoreService.leggiListaTecnici());
        this.operatoriVisite = JSON.parse(JSON.stringify(result));
    }


    async caricaDDLSpecie(tutteLeSpecie: boolean,
                            partitaIva: string,
                            ragSoc: string) {

        let selectedAzienda = new Impresa();

        selectedAzienda.partitaIva = partitaIva;
        selectedAzienda.ragioneSociale = ragSoc;


        const LeggiUtilizzoTerreno = <LeggiImpianto>{
            data: new Date(),
            consideraTerrenoNudo: true,
            dettagliTerrenoNudo: true,
            impresa: selectedAzienda
        };

        if (!tutteLeSpecie) {
            this.specieVisite = await this.impiantiservice.Leggi_SpecieVegetali_Attive_Impianti(LeggiUtilizzoTerreno,true,"NessunaSpecie");
            this.specieVisite.map(item => {
                item.codice = item.codice === -1 ? 0 : item.codice;
                return item;
            });
        } else {
            // this.specieVisite = await lastValueFrom(this.specieService.leggiTutteLeSpecieAPI());

            this.specieVisite = await lastValueFrom(forkJoin({
                                                                specie: this.specieService.leggiTutteLeSpecieAPI(),
                                                                destinazioni: this.destUsoService.leggi()
                                                            }).pipe(
                                                                map(({ specie, destinazioni }) => {
                                                                    let nessunaColtura = specie.find(item => item.codice === 0);
                    
                                                                    //aggiorno la descrizione della coltura "Nessuna coltura" con Nessuna Specie
                                                                    if (nessunaColtura) {
                                                                        nessunaColtura.descrizione = this.translocoService.translate("NessunaSpecie");
                                                                    }

                                                                    let arrCombinedAndSorted = [...specie, ...destinazioni].sort((a, b) => 
                                                                                                                        a.descrizione.localeCompare(b.descrizione));

                                                                    if (nessunaColtura) {
                                                                        const filtered = arrCombinedAndSorted.filter(item => item !== nessunaColtura);
                                                                        return [nessunaColtura, ...filtered];
                                                                    }

                                                                    return arrCombinedAndSorted;
                                                                })
                                                            )
                                                    );

            let destUso = new DestinazioneUso();
            destUso.codice = -2;
            destUso.descrizione = "Utilizzo Terreno";
            this.specieVisite.push(destUso);
        }
    }

    async caricaDDLImpianti(specieScelta: Specie,
                            aziendaScelta: Impresa,
                            centroAziendaleScelto: CentroAziendale) {

        let param: LeggiImpianto = new LeggiImpianto();

        param.veg_cod = specieScelta?.codice ?? -1;
        param.impresa = aziendaScelta;
        param.centroAziendale = centroAziendaleScelto;

        let response = await lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost(
            'Agenda/LeggiImpianti',
            param
        ).pipe(map(r => {
                        return r.RispostaStringa
                    })));

        this.impiantiVisite = JSON.parse(JSON.stringify(response));
    }

    async caricaDDLAziende(switchAziende: boolean, username: string) {
        let param = new LeggiAziende(username);

        let result;

        if(!switchAziende)
            result = await lastValueFrom(this.aziendaService.leggiListaAziende(param));
        else
            result = await lastValueFrom(this.aziendaService.leggiListaAgenzie(param));

        this.aziendeVisite = JSON.parse(JSON.stringify(result));
    }

    async caricaDDLCentriAziendali(specieScelta: Specie,
                                    AziendaScelta: Impresa) {

        let selectedAzienda = new Impresa();

        selectedAzienda.partitaIva = AziendaScelta?.partitaIva ?? "-1";
        selectedAzienda.ragioneSociale = AziendaScelta?.ragioneSociale ?? null;

        let LeggiCentriAziendali;

        LeggiCentriAziendali=<LeggiCentriAziendali>{
            impresa:  selectedAzienda,
            data: new Date(),
            utilizzoTerreno: this.getUtilizzoTerreno(specieScelta)
        };

        this.centriAziendaliVisite = await this.centriservice.leggiCentriAziendaliModelloQdC(LeggiCentriAziendali,false);

    }

    getUtilizzoTerreno(specieScelta: Specie): UtilizzoTerreno {

        // let utilizzoTerrenoClassType = specieScelta.classType;

        let utilizzoTerreno = null;

        if (specieScelta) {
            if (specieScelta.codice != 0) {
                utilizzoTerreno = new DestinazioneUso();
                utilizzoTerreno.codice = specieScelta.codice;
                utilizzoTerreno.descrizione = specieScelta.descrizione;
            } else {
                utilizzoTerreno = new Varieta();
                (<Varieta>utilizzoTerreno).specie = new Specie(specieScelta.codice);
                (<Varieta>utilizzoTerreno).specie.descrizione = specieScelta.descrizione;
                utilizzoTerreno.codice = 0;
                utilizzoTerreno.descrizione = "";
            }
        }

        return utilizzoTerreno;
    }


    async caricaDDLOperazioni() {

        let risp;
        risp = await lastValueFrom(this.attivitapersonalizzataservice.Leggi_AttivitaPersonalizzataVisita());
        this.operazioniVisite = JSON.parse(JSON.stringify(risp));

    }

    caricaDDLTipoVisita() {
        if (this.tipoVisita.length === 0) {
            this.tipoVisita.push(new TipoVisita(400, this.translocoService.translate("Da_Eseguire")));
            this.tipoVisita.push(new TipoVisita(401, this.translocoService.translate("Eseguita")));
        }
    }

    async caricaDDLSpecieAnimali() {
        this.SpecieAnimaliVisita = await lastValueFrom(this.specieAnimaliService.leggiListaSpecieAnimaliDDL());

    }

    caricaDefaultSpecieAnimale() {
        let risorsaZoodefault = new RisorsaZootecnica();
        let defaultSpecieAnimale = this.specieAnimaliService.setDefaultDDLSpecieAnimale(risorsaZoodefault as DropdownListSpecieAnimali, "TutteLeSpecie");
        return defaultSpecieAnimale;
    }
}
