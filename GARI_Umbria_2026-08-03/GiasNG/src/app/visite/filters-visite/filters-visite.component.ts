import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DrawerComponent } from '@progress/kendo-angular-layout';
import { CreateFormGroup } from './utilities';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { faEraser, faMagnifyingGlass, faSliders, faXmark } from '@fortawesome/free-solid-svg-icons';
import { FiltersServiceVisite, TipoVisita } from './filters-visite.service';
import { Subscription } from 'rxjs';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { Operatore } from 'app/Model/metaschema/utilizzi/Operatore';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { MasterService } from 'app/Service/master.service';
import { DropdownListSpecieAnimali } from 'app/quaderno-di-campagna/agenda-edit/service/testata/specie-animali-service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDropDownTemplateService } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-filters-visite',
  templateUrl: './filters-visite.component.html',
  styleUrls: ['./filters-visite.component.css'],
  providers: [GiasDropDownTemplateService, GiasMultiSelectTemplateService],
})
export class FiltersVisiteComponent implements OnInit, OnDestroy {

  displayButton: boolean = true;
  drawerWidth: number;

  private ddlSub: Subscription = new Subscription();

  public shouldApply: boolean = true;

  FiltersVisite: FormGroup = CreateFormGroup(this.fb, this.visiteService.filtersGetValue() /*InitializeFilters()*/);

  @ViewChild('drawer') drawer: DrawerComponent;

  faFilters = faSliders;
  faClose = faXmark;
  faSearch = faMagnifyingGlass;
  faTrash = faEraser;

  defaultOperatore: Operatore;
  defaultAzienda: Impresa;
  defaultSpecie: Specie;
  defaultCentro: CentroAziendale;
  defaultTipoVisita: TipoVisita;
  defaultSpecieAnimale: DropdownListSpecieAnimali;

  objParametriAgenda: ObjParametriAgenda;

  isDisabled: boolean = false;

  constructor(private fb: FormBuilder,
    private visiteService: FiltersServiceVisite,
    private ddlService: GiasDropDownTemplateService,
    private masterService: MasterService,
    private translocoService: TranslocoService) {

    this.defaultOperatore = new Operatore("", this.translocoService.translate("TuttiGliOperatori"), -1, "");

    this.defaultAzienda = new Impresa();
    this.defaultAzienda.partitaIva = "-1";
    this.defaultAzienda.ragioneSociale = this.translocoService.translate("TutteLeAziende");

    this.defaultSpecie = new Specie(-1);
    this.defaultSpecie.descrizione = this.translocoService.translate("TutteLeSpecie");

    this.defaultCentro = CentroAziendale.getEmptyCentroAziendale("-1");
    this.defaultCentro.nome = this.translocoService.translate("TuttiICentriAziendali");

    this.defaultTipoVisita = new TipoVisita(-1, this.translocoService.translate("TuttiGliStatiVisita"));

    this.defaultSpecieAnimale = this.visiteService.caricaDefaultSpecieAnimale();
  }


  get OperatoriVisita() { return this.visiteService.operatoriVisite }

  get AziendeVisita() { return this.visiteService.aziendeVisite }

  get SpecieVisita() { return this.visiteService.specieVisite }

  get CentroAziendaleVisita() { return this.visiteService.centriAziendaliVisite }

  get ImpiantiVisita() { return this.visiteService.impiantiVisite }

  get OperazioniVisita() { return this.visiteService.operazioniVisite }

  get TipiVisita() { return this.visiteService.tipoVisita }

  get SpecieAnimaliVisita() { return this.visiteService.SpecieAnimaliVisita }

  ngOnInit(): void {
    this.visiteService.caricaDDLOperatori();
    this.visiteService.caricaDDLAziende(this.FiltersVisite.get("Aziende_Agenzie").value, this.masterService.objP_utenti.UtenteUsername);
    this.visiteService.caricaDDLOperazioni();
    this.visiteService.caricaDDLTipoVisita();
    this.visiteService.caricaDDLSpecieAnimali();

    this.ddlSub.add(this.ddlService.currentDropDownValueObject
      .subscribe(async ddlElem => {
          switch (ddlElem.FormControlName) {
              case 'OperatoreVisita':
                  this.clearDDLAziende();
                  this.visiteService.caricaDDLAziende(this.FiltersVisite.get("Aziende_Agenzie").value, (ddlElem.Value === undefined) ? this.masterService.objP_utenti.UtenteUsername : ddlElem.Value.username);
                  this.clearDDLSpecie();
                  this.clearDDLImpianti();
              break;
              case 'AziendeVisita':
                if (ddlElem.Value != this.defaultAzienda) {
                  this.clearDDLSpecie();
                  this.visiteService.caricaDDLSpecie(this.FiltersVisite.get("Visualizza_Specie").value, ddlElem.Value?.partitaIva ?? -1, ddlElem.Value?.ragioneSociale ?? "");
                  this.clearDDLCentriAziendali();
                  this.visiteService.caricaDDLCentriAziendali(this.FiltersVisite.get("SpecieVisita").value, this.FiltersVisite.get("AziendeVisita").value);
                  this.clearDDLImpianti();
                  this.clearDDLOperazioni();
                  this.clearDDLTipoVisita();
                }
              break;
              case 'SpecieVisita':
                if (ddlElem.Value != this.defaultSpecie) {
                  this.clearDDLCentriAziendali();
                  this.visiteService.caricaDDLCentriAziendali(this.FiltersVisite.get("SpecieVisita").value, this.FiltersVisite.get("AziendeVisita").value);
                  this.clearDDLImpianti();
                  this.clearDDLOperazioni();
                  this.clearDDLTipoVisita();
                }
              break;
              case 'CentroAziendaleVisita':
                if (ddlElem.Value != this.defaultCentro) {
                  this.clearDDLImpianti();
                  this.visiteService.caricaDDLImpianti(this.FiltersVisite.get("SpecieVisita").value, this.FiltersVisite.get("AziendeVisita").value, this.FiltersVisite.get("CentroAziendaleVisita").value);
                  this.clearDDLOperazioni();
                  this.clearDDLTipoVisita();
                }
              break;
          }


      }));
  }

  ngOnDestroy(): void {
    this.ddlSub.unsubscribe();
  }

  clearDDLOperatori() {
    this.FiltersVisite.get("OperatoreVisita").reset();
  }

  clearDDLAziende() {
    this.FiltersVisite.get("AziendeVisita").reset();
  }

  clearDDLSpecie() {
    this.FiltersVisite.get("SpecieVisita").reset();
  }

  clearDDLCentriAziendali() {
    this.FiltersVisite.get("CentroAziendaleVisita").reset();
  }

  clearDDLImpianti() {
    this.FiltersVisite.get("Impianti").reset();
  }

  clearDDLOperazioni() {
    this.FiltersVisite.get("TipoOperazione").reset();
  }

  clearDDLTipoVisita() {
    this.FiltersVisite.get("TipoVisita").reset();
  }

  clearDDLSpecieAnimali() {
    this.FiltersVisite.get("SpecieAnimaliVisita").reset();
  }

  applyFilters() {
    if (!this.shouldApply) return;
    if (!this.FiltersVisite.get('Da').value)
      this.FiltersVisite.get('Da').patchValue(AGRODATAINIZIO);
    if (!this.FiltersVisite.get('A').value)
      this.FiltersVisite.get('A').patchValue(AGRODATAFINE);
    const values = this.FiltersVisite.value;

    this.visiteService.applyFilters(values);

    this.centerVisual();
    if (!!this.drawer.expanded) {
      this.drawer.toggle();
    }
  }

  public centerVisual(): void {
    document.documentElement.scrollTop = 0;
  }

  changeDominioSpecie(tutteLeSpecie: boolean) {
    this.clearDDLSpecie();
    this.visiteService.caricaDDLSpecie(tutteLeSpecie,
                                        this.FiltersVisite.get("AziendeVisita").value?.partitaIva ?? -1,
                                        this.FiltersVisite.get("AziendeVisita").value?.ragioneSociale ?? "");
  }

  changeDettaglioRilievo(withRilievo: boolean) {

  }

  changeAziendeAgenzie(switchAziende: boolean) {
    this.visiteService.caricaDDLAziende(switchAziende, this.FiltersVisite.get("OperatoreVisita").value.username);
  }

  onCancel() {
    this.displayButton = true;
    this.shouldApply = false;

    if (!!this.drawer.expanded) {
      this.drawer.toggle();
    }
  }

  onClear() {
    this.clearDDLTipoVisita();
    this.clearDDLOperazioni();
    this.clearDDLSpecieAnimali();
    this.clearDDLImpianti();
    this.clearDDLCentriAziendali();
    this.clearDDLSpecie();
    this.clearDDLAziende();
    this.clearDDLOperatori();
  }

  onOpenFilters() {
    this.displayButton = false;
    this.drawer.toggle();
  }

  updateDisabled(newDisabledState: boolean) {
    this.isDisabled = newDisabledState;
  }

}
