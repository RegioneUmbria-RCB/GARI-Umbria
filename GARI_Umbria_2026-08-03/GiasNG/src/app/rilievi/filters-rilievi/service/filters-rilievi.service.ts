import { Injectable, OnDestroy } from "@angular/core";
import { FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { enum_LAVCOD } from "app/Model/TipiEnumerativi";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { DestinazioneUso } from "app/Model/metaschema/utilizzi/DestinazioneUso";
import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { UtilizzoTerreno } from "app/Model/metaschema/utilizzi/UtilizzoTerreno";
import { Varieta } from "app/Model/metaschema/utilizzi/Varieta";
import { CentriAziendaliService } from "app/Service/Anagrafica/centri.service";
import { ImpreseService } from "app/Service/Anagrafica/imprese.service";
import { DestinazioneUsoService } from "app/Service/Metaschema/destinazioneUso.service";
import { SpecieVegetaliService } from "app/Service/Metaschema/specie-vegetali.service";
import { CentroAziendale, Impresa } from "app/Service/api.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { ObjParametriAgenda } from 'gias-ui-kit';
import { BehaviorSubject, Subject, forkJoin, map, takeUntil } from "rxjs";

class FiltersRilieviConfig {
    AziendaRilievi: Impresa;
    CentroAziendaleRilievi: CentroAziendale;
    TipoRilievi: BaseCodeDescr;
    SpecieRilievi: UtilizzoTerreno | Specie;
    Da: Date;
    A: Date;
}


@Injectable()
export class FiltersRilieviService implements OnDestroy {

    FiltersRilieviForm: FormGroup = CreateFormGroup(this.fb, InitializeFilters());

    public filters: BehaviorSubject<FiltersRilieviConfig> = new BehaviorSubject(null);

    public aziendeRilievi: Impresa[] = [];
    public specieRilievi: UtilizzoTerreno[] | Specie[] = [];
    public centriAziendaliRilievi = [];

    signal: Subject<void> = new Subject<void>();

    objParametriAgenda: ObjParametriAgenda;

    public listaRilievi: BaseCodeDescr[] = [
        new BaseCodeDescr(enum_LAVCOD.FASI_FENOLOGICHE, this.translocoService.translate('RilievoFiorituraFasiFenologiche')),
        new BaseCodeDescr(enum_LAVCOD.RILIEVO_ERBE_INFESTANTI, this.translocoService.translate('RilievoErbeInfestanti')),
        new BaseCodeDescr(enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO, this.translocoService.translate('RilievoAvversitaCampo')),
        new BaseCodeDescr(enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE, this.translocoService.translate('RilievoAvversitaTrappole')),
        new BaseCodeDescr(enum_LAVCOD.RILIEVO_INDICI_MATURITA, this.translocoService.translate('RilievoIndiciMaturita')),
        new BaseCodeDescr(enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA, this.translocoService.translate('RilievoRaccolta')),
        new BaseCodeDescr(enum_LAVCOD.DANNI_RACCOLTA, this.translocoService.translate('RilievoDanniRaccolta')),
        new BaseCodeDescr(enum_LAVCOD.RILIEVO_PIOGGE, this.translocoService.translate('RilievoPiogge'))
    ];

    constructor(private fb: FormBuilder,
                private impreseService: ImpreseService,
                private specieService: SpecieVegetaliService,
                private destUsoService: DestinazioneUsoService,
                private agendaService: ObjParametriAgendaService,
                private centriservice: CentriAziendaliService,
                private translocoService: TranslocoService
    ) {
        // this.caricaAziende();

        this.objParametriAgenda = this.agendaService.getObjParamValue();

        // this.menuClient.menuRicercaAzienda('%').subscribe((data) => { 
        //     console.log(data.RispostaStringa);
        //     this.aziendeRilievi = data.RispostaStringa;
        // });

        this.impreseService.caricaCmbImprese()
            .pipe(map(
                data => data.map(item => ({
                    partitaIva: item.piva,
                    ragioneSociale: item.rag_soc
                })
                )
            )
            )
            .subscribe((data) => {
                this.aziendeRilievi = data;
            });

        this.combinaSpecieEDestUso().subscribe(data => {
            this.specieRilievi = data;
        });

        // this.caricaDDLCentriAziendali(null, null);

        this.FiltersRilieviForm.get('AziendaRilievi').valueChanges.pipe(takeUntil(this.signal)).subscribe(value => {
            this.caricaDDLCentriAziendali(null, { partitaIva: value.partitaIva, ragioneSociale: value.ragioneSociale });
        });
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    combinaSpecieEDestUso() {
        return forkJoin({
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

                }
            )
        );
    }

    async caricaDDLCentriAziendali(specieScelta: Specie,
        AziendaScelta: Impresa) {

        let selectedAzienda = { partitaIva: '', ragioneSociale: '' } as Impresa;

        if (AziendaScelta.partitaIva !== "-1") {

            selectedAzienda.partitaIva = AziendaScelta.partitaIva;
            selectedAzienda.ragioneSociale = AziendaScelta.ragioneSociale;

            let LeggiCentriAziendali;

            LeggiCentriAziendali = {
                impresa: selectedAzienda,
                data: new Date(),
                utilizzoTerreno: this.getUtilizzoTerreno(specieScelta)
            };

            this.centriAziendaliRilievi = await this.centriservice.leggiCentriAziendaliModelloQdC(LeggiCentriAziendali, false);
        } else
            this.centriAziendaliRilievi.splice(0, this.centriAziendaliRilievi.length);

    }

    getUtilizzoTerreno(specieScelta: Specie): UtilizzoTerreno {

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

}


const DA = AGRODATAINIZIO;
const A = AGRODATAFINE;

function InitializeFilters(vals?: Partial<FiltersRilieviConfig>): FiltersRilieviConfig {
    return {
        AziendaRilievi: vals?.AziendaRilievi ?? null,
        CentroAziendaleRilievi: vals?.CentroAziendaleRilievi ?? null,
        TipoRilievi: vals?.TipoRilievi ?? null,
        SpecieRilievi: vals?.SpecieRilievi ?? null,
        Da: vals?.Da ?? DA,
        A: vals?.A ?? A
    };
}

function CreateFormGroup(fb: FormBuilder, mask: FiltersRilieviConfig) {
    return fb.group({
        AziendaRilievi: new FormControl(mask.AziendaRilievi),
        CentroAziendaleRilievi: new FormControl(mask.CentroAziendaleRilievi),
        TipoRilievi: new FormControl(mask.TipoRilievi),
        SpecieRilievi: new FormControl(mask.SpecieRilievi),
        Da: mask.Da,
        A: mask.A
    });
}