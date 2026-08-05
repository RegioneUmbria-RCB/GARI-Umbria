import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {DrawerComponent} from "@progress/kendo-angular-layout";
import {
  GiasDropDownTemplateSComponent,
  GiasDropDownTemplateService
} from "gias-ui-kit";
import {TranslocoService} from "@jsverse/transloco";
import {CentriAziendaliService, LeggiCentriConFiltroUtente} from "../../../Service/Anagrafica/centri.service";
import {CentroItem, SpecieItem} from "gias-kendo-grid";
import {ObjParametriAgendaService} from "../../../Service/obj-parametri-agenda.service";
import {AnagraficaClient, LeggiSpecieQdC, Varieta} from "../../../Service/api.service";
import {map, Observable, Subscription, take} from "rxjs";
import {AGRODATAINIZIO,  AGRODATAFINE, NessunaSpecieQdC} from "../../../Model/CostantiPersonalizzate";
import {faClose, faMagnifyingGlass, faSliders} from "@fortawesome/free-solid-svg-icons";
import {
  FiltriTrappole,
  ScadenzaReinnescoTrappoleService
} from 'app/scadenza-reinnesco-trappole/scadenza-reinnesco-trappole.service';
import {DropdownListFormItem} from "gias-ui-kit/lib/utils/models";
import {UtilityFunctions} from "../../../Utility/UtilityFunctions";
import {cloneDeep} from "lodash";
import {NgxCookieService} from "../../../Service/ngx-cookie.service";
import {CookieService} from "../../../Service/cookie.service";
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {Utente} from "../../../Model/utente/utente";

@Component({
  standalone: false,
  selector: 'app-filtri-trappole',
  templateUrl: './filtri-trappole.component.html',
  styleUrl: './filtri-trappole.component.scss',
  providers: [GiasDropDownTemplateService]
})
export class FiltriTrappoleComponent implements OnInit,OnDestroy {

  constructor(private objParametriAgendaService: ObjParametriAgendaService,
              private anagraficaClient: AnagraficaClient,
              private translocoService: TranslocoService,
              private centriService: CentriAziendaliService,
              public scadenzaReinnescoTrappoleService: ScadenzaReinnescoTrappoleService,
              private ddlService: GiasDropDownTemplateService,
              private giasCookieService: CookieService,
              private permessiUtenteService: PermessiUtenteService) {
  }

  @ViewChild('drawer') drawer: DrawerComponent;

  public CentriList: CentroItem[] = null;

  public SpecieList: SpecieItem[] = null;

  public defaultSpecie: SpecieItem = { veg_des: this.translocoService.translate('TutteLeSpecie'), veg_cod: '-1' };

  public defaultCentri: CentroItem = { sa_nome: this.translocoService.translate('TuttiICentriAziendali'), sa_cod: '0' };

  public MinInizio = AGRODATAINIZIO;

  public MaxFine = AGRODATAFINE;

  public Subs: Subscription = new Subscription();

  public utente: Utente;

  ngOnInit() {

    this.utente = this.permessiUtenteService.getCurrentUser();

    this.setDefault();

    this.Subs.add(this.ddlService.currentDropDownValueObject.subscribe((ddlElem: DropdownListFormItem)=>{
      switch(ddlElem.FormControlName){
        case "CentroAziendale":
          this.CaricaSpecie().subscribe(x=>{
            this.scadenzaReinnescoTrappoleService.FiltriForm.get("Specie").patchValue(this.defaultSpecie);
          });

          break;
      }
    }));
  }

  private setDefault() {

    let filtriTrappole: FiltriTrappole = new FiltriTrappole(this.defaultCentri,this.defaultSpecie);

    this.DefaultFromCookie(filtriTrappole);

    this.scadenzaReinnescoTrappoleService.FiltriForm.patchValue(filtriTrappole);
  }

  private DefaultFromCookie(filtriTrappole: FiltriTrappole){

    let centroCookieValueStr: string = this.giasCookieService.getCookie(this.getCookieKeyCentroAziendale());

    if(centroCookieValueStr != "")
      filtriTrappole.CentroAziendale = JSON.parse(centroCookieValueStr);

    let specieCookieValueStr: string = this.giasCookieService.getCookie(this.getCookieKeySpecie());

    if(specieCookieValueStr != "")
      filtriTrappole.Specie = JSON.parse(specieCookieValueStr);

    let daCookieValueStr: string = this.giasCookieService.getCookie(this.getCookieKeyDa());

    if(daCookieValueStr != "")
      filtriTrappole.Da = new Date(+ daCookieValueStr);

    let aCookieValueStr: string = this.giasCookieService.getCookie(this.getCookieKeyA());

    if(aCookieValueStr != "")
      filtriTrappole.A = new Date(+ aCookieValueStr);
  }

  private CaricaSpecie(): Observable<SpecieItem[]>{

    const objParams = this.objParametriAgendaService.getObjParamValue();

    const centroItem: CentroItem = this.scadenzaReinnescoTrappoleService.FiltriForm.get("CentroAziendale").getRawValue();

    const leggiSpecie: LeggiSpecieQdC = {
      centroAziendale: {
        primaryKey: {
          codice: + centroItem.sa_cod,
          partitaIva: objParams.Piva
        }
      },
      data: new Date(),
      impresa: {
        partitaIva: objParams.Piva,
        ragioneSociale: objParams.RagSoc
      },
      consideraTerrenoNudo: true,
      soloAttiviAllaData: true,
      lavorazioni: []
    };

    return this.anagraficaClient.anagraficaLeggiSpecieVegetaliQdC(leggiSpecie).pipe(
      map(x=>{

        const nessunaSpecie: SpecieItem = { veg_cod: NessunaSpecieQdC.toString(), veg_des: this.translocoService.translate('NessunaSpecie') };

        const result: SpecieItem[] = [this.defaultSpecie,nessunaSpecie];

        for (const utilizzoTerreno of x.RispostaStringa) {
          if (utilizzoTerreno.classType === 'Varieta') {
            const varieta = utilizzoTerreno as Varieta
            result.push({ veg_cod: varieta.specie.codice.toString(), veg_des: varieta.specie.descrizione });
          } else {
            // Destinazione d'uso --> necessario attivare flag_TerrenoNudo in filtri di ricerca per leggere
            // correttamente le operazioni. Il '-' è stato messo per non confondere il record con le varietà.
            result.push({ veg_cod: '-' + utilizzoTerreno.codice.toString(), veg_des: utilizzoTerreno.descrizione });
          }
        }

        this.SpecieList = cloneDeep(result);

        return this.SpecieList;
      })
    );
  }

  private CaricaCentri(): Observable<CentroItem[]>{

    const leggiCentri: LeggiCentriConFiltroUtente = {
      PrimaRiga_Flag: false,
      PrimaRiga_Text: "",
      PrimaRiga_Value: "",
      Piva: this.objParametriAgendaService.getObjParamValue().Piva,
      Flag_SoloCentriAttivi: false,
      Tipo_Value: 2
    };

    return this.centriService.LeggiCentriConFiltroUtente(leggiCentri).pipe(
      map((x: CentroItem[])=>{
        x.unshift(this.defaultCentri);

        this.CentriList = cloneDeep(x);

        return this.CentriList;
      })
    );
  }

  onOpenFilters() {
    this.drawer.toggle();
  }

  onCancel() {
    if (!!this.drawer.expanded) {
      this.drawer.toggle();
    }
  }

  public ApplicaFiltri(){
    this.scadenzaReinnescoTrappoleService.applicaFiltri = true;

    this.drawer.toggle();

    this.saveCookies();
  }

  public openDdlFiltri(ddlEl: GiasDropDownTemplateSComponent, formName: string) {
    switch (formName) {
      case 'CentroAziendale':
        UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefinedObs(ddlEl, this.CaricaCentri());
        break;
      case 'Specie':
        UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefinedObs(ddlEl, this.CaricaSpecie());
        break;
    }
  }

  private saveCookies(){

    let filtriTrappole: FiltriTrappole = this.scadenzaReinnescoTrappoleService.FiltriForm.getRawValue();

    if(filtriTrappole.CentroAziendale && filtriTrappole.CentroAziendale.sa_cod !== this.defaultCentri.sa_cod){

      const key: string = this.getCookieKeyCentroAziendale();

      this.giasCookieService.deleteCookie(key);

      this.giasCookieService.setCookie({
        name: key,
        value: JSON.stringify(filtriTrappole.CentroAziendale)
      });
    }


    if(filtriTrappole.Specie && filtriTrappole.Specie.veg_cod !== this.defaultSpecie.veg_cod){

      const key: string = this.getCookieKeySpecie();

      this.giasCookieService.deleteCookie(key);

      this.giasCookieService.setCookie({
        name: key,
        value: JSON.stringify(filtriTrappole.Specie)
      });
    }

    if(filtriTrappole.Da !== undefined  && filtriTrappole.Da !== null){

      let inizio:number = filtriTrappole.Da.getTime();

      if(inizio != AGRODATAINIZIO.getTime() && inizio != AGRODATAFINE.getTime()){

        const key: string = this.getCookieKeyDa();

        this.giasCookieService.deleteCookie(key);

        this.giasCookieService.setCookie({
          name: key,
          value: inizio.toString()
        });
      }
    }

    if(filtriTrappole.A !== undefined  && filtriTrappole.A !== null){
      let fine:number = filtriTrappole.A.getTime();

      if(fine != AGRODATAINIZIO.getTime() && fine != AGRODATAFINE.getTime()){

        const key: string = this.getCookieKeyA();

        this.giasCookieService.deleteCookie(key);

        this.giasCookieService.setCookie({
          name: key,
          value: fine.toString()
        });
      }
    }

  }

  private getCookieKeyCentroAziendale(){

    return `GridTrappole_Centro_${this.utente.Username}`;
  }

  private getCookieKeySpecie(){

    return `GridTrappole_Specie_${this.utente.Username}`;
  }

  private getCookieKeyDa(){

    return `GridTrappole_Da_${this.utente.Username}`;
  }

  private getCookieKeyA(){

    return `GridTrappole_A_${this.utente.Username}`;
  }


  protected readonly faFilters = faSliders;

  public faClose = faClose;
  protected readonly faSearch = faMagnifyingGlass;

  ngOnDestroy(): void {
    this.Subs.unsubscribe();
  }

}
