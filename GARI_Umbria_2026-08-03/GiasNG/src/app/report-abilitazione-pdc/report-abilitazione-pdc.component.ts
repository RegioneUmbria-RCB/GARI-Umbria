import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { ImpreseService } from 'app/Service/Anagrafica/imprese.service';
import { from, map, Subscription, take } from 'rxjs';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { VarietaService } from 'app/Service/Metaschema/varieta.service';
import { GruppoVarietaleService } from 'app/Service/Metaschema/gruppoVarietale.service';
import { AGRODATAFINE, AGRODATAINIZIO, GiasDropDownTemplateService, GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { enum_CodificaStampe } from 'app/Model/TipiEnumerativi';
import { DateToString } from 'gias-kendo-grid';


@Component({
  standalone: false,
  selector: 'app-report-abilitazione-pdc',
  templateUrl: './report-abilitazione-pdc.component.html',
  styleUrls: ['./report-abilitazione-pdc.component.css'],
  providers: [
    GiasMultiSelectTemplateService,
    GiasDropDownTemplateService
  ]
})
export class ReportAbilitazionePdCComponent implements OnInit, OnDestroy {

  Subs: Subscription = new Subscription();

  reportForm: FormGroup;

  filtroImprese: any[] = [];
  
  constructor(private fb: FormBuilder,
    private translocoService: TranslocoService,
    private impreseService: ImpreseService,
    private specieService: SpecieVegetaliService,
    private varietaService: VarietaService,
    private gruppiVarietaliService: GruppoVarietaleService,
    private gestioneRichiesteService: GestioneRichiesteService
  ) { }

  ngOnInit(): void {
    this.reportForm = this.fb.group({
      partitaIva: [],
      ragioneSociale: [],
      codiceSocio: [],
      specie: [],
      gruppoVarietale: [],
      varieta: [],
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }),
      tipologiaReport: []
    });
  }

  onSubmit(initialLoading: boolean) {
  }

  async openDdl(ddlEl: any, formName: string) {
    switch (formName) {
      case 'CodiceSocio':
        this.impreseService.leggiImprese().pipe(map(imprese => {
          ddlEl.listItems = imprese.kendo_rows;
        })).subscribe();
      break;
      case 'Piva':
        this.impreseService.leggiImprese().pipe(map(imprese => {
          ddlEl.listItems = imprese.kendo_rows;
        })).subscribe();
      break;
      case 'Azienda':
        this.impreseService.leggiImprese().pipe(map(imprese => {
          ddlEl.listItems = imprese.kendo_rows;
        })).subscribe();
      break;
      case 'Specie':
        await this.specieService.leggi().then(specie => {
          ddlEl.listItems = specie;
        });
      break;
      case 'GruppoVarietale':
        if(this.reportForm.controls['specie'].value?.codice != 0) {
          await this.gruppiVarietaliService.leggi(this.reportForm.controls['specie'].value).then(gruppiVarietali => {
            ddlEl.listItems = gruppiVarietali;
          });
        }
      break;
      case 'Varieta':
        if(this.reportForm.controls['specie'].value?.codice != 0) {
          await this.varietaService.leggi(this.reportForm.controls['specie'].value).then(varieta => {
            ddlEl.listItems = varieta;
          });
        }
      break;
      case 'TipologiaReport':
        ddlEl.listItems = [{codice: 1, descrizione:'Raggruppa per socio'},
          {codice: 2, descrizione:'Raggruppa per capitolato'}
        ];
      break;
    }
  }

  aggiornaFiltroImprese(value: any, operazione: string): void {
    switch (operazione) {
      case 'Aggiunta':
        this.filtroImprese = value;
      break;
      case 'Rimozione':
        this.filtroImprese.splice(this.filtroImprese.indexOf(value.dataItem), 1);
      break;
    }
    this.reportForm.controls['partitaIva'].patchValue(this.filtroImprese);
    this.reportForm.controls['codiceSocio'].patchValue(this.filtroImprese);
    this.reportForm.controls['ragioneSociale'].patchValue(this.filtroImprese);
  }

  aggiornaFiltroSpecie(): void {
      this.reportForm.controls['gruppoVarietale'].setValue(null);
      this.reportForm.controls['varieta'].setValue(null);
  }

  stampaAbilitazioneXSpecie(): void {

    const pive_selezionate = this.filtroImprese.length > 0 ? 
      this.filtroImprese.map(impresa => impresa.piva).join(",") :
      ""
    const specie = this.reportForm.controls['specie'].value?.codice;
    const gruppo_varietale = this.reportForm.controls['gruppoVarietale'].value?.codice;
    const varieta = this.reportForm.controls['varieta'].value?.codice;
    const validita_inizio = this.reportForm.controls['validita'].value?.inizio;
    const validita_fine = this.reportForm.controls['validita'].value?.fine;
    const tipologiaReport = this.reportForm.controls['tipologiaReport'].value?.codice;

    const parametriStampa: Array<ParametriAggiuntivi_QueryString> = [
      KeyValuePair.Create("stringa_lista_pive", pive_selezionate ?? ""),
      KeyValuePair.Create("specie", specie != null && specie != 0 ? specie : ""),
      KeyValuePair.Create("gruppovarietale", gruppo_varietale != null && gruppo_varietale != 0 ? gruppo_varietale : ""),
      KeyValuePair.Create("varieta", varieta != null && varieta != 0 ? varieta : ""),
      KeyValuePair.Create("anno", (new Date).getFullYear().toString()),
      KeyValuePair.Create("validita_inizio", DateToString(validita_inizio ?? AGRODATAINIZIO)),
      KeyValuePair.Create("validita_fine", DateToString((validita_fine ?? AGRODATAFINE))),
      KeyValuePair.Create("tipologiaReport", tipologiaReport?.toString() ?? "0")
    ]

    from(this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaStampe_2010,
      enum_CodificaStampe.AbilitazioniXSpecie,
      parametriStampa
    )).pipe(take(1))
          .subscribe(link => window.open(link, '_blank'));
  }

  ngOnDestroy(): void {
  }
}