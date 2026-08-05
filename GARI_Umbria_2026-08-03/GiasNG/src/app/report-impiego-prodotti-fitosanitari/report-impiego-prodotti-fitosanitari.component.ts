import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { FormReportImpiegoProdottiFitosanitariConfig, ImpresaModel, InitializeFormReportImpiegoProdottiFitosanitari, SostanzaAttiva } from './report-impiego-prodotti-fitosanitari-utils';
import { TranslocoService } from '@jsverse/transloco';
import { AnagrafeClient, ComuneDatiIn, MetaschemaClient, ReportImpiegoProdottiFitosanitari_IN } from 'app/Service/net-core6-api.service';
import { Subscription } from 'rxjs';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridReportFitoHttpService } from './service/grid-report-fito-http.service';
import { Inject } from "@angular/core";
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { faMagnifyingGlass, faSliders } from '@fortawesome/free-solid-svg-icons';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { DrawerComponent } from '@progress/kendo-angular-layout';

@Component({
  standalone: false,
  selector: 'app-report-impiego-prodotti-fitosanitari',
  templateUrl: './report-impiego-prodotti-fitosanitari.component.html',
  styleUrls: ['./report-impiego-prodotti-fitosanitari.component.css'],
  providers: [
    ...generateGridProviders(GridReportFitoHttpService, ReportImpiegoProdottiFitosanitariComponent),
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService]
})
export class ReportImpiegoProdottiFitosanitariComponent implements OnInit, OnDestroy, AfterViewInit {
  ListaProvinceSelezionate = new Array();
  inputForm: FormGroup = this.createFormGroup(InitializeFormReportImpiegoProdottiFitosanitari());
  getPrincipiAttiviSubscription: Subscription;
  leggiImpreseSubscription: Subscription;
  imprese: ImpresaModel[] = [];
  sostanzeAttive: SostanzaAttiva[] = [];
  defaultItemSostanzaAttiva: SostanzaAttiva = { Pa_Cod: 0, Pa_Des: '' };
  showGrid: boolean = false;
  faMagnifyingGlass = faMagnifyingGlass;

  shouldApply: boolean = true;
  faSliders = faSliders;
  drawerWidth: number;

  @ViewChild('drawer') drawer: DrawerComponent;

  constructor(private fb: FormBuilder,
    private translocoService: TranslocoService,
    private metaschemaClient: MetaschemaClient,
    private anagrafeClient: AnagrafeClient,
    @Inject(GRID_HTTP_TOKEN) private gridReportFitoHttpService: GridReportFitoHttpService
  ) { }

  ngOnInit(): void {

    const currentYear = new Date().getFullYear();
    const firstDayOfYear = new Date(currentYear, 0, 1);
    const lastDayOfYear = new Date(currentYear, 11, 31);

    this.inputForm.get('Data_Da').patchValue(firstDayOfYear);
    this.inputForm.get('Data_A').patchValue(lastDayOfYear);

    this.getPrincipiAttiviSubscription = this.metaschemaClient.metaschemaGetPrincipiAttivi().subscribe(r => {
      this.sostanzeAttive = JSON.parse(r.RispostaStringa);
    });

    this.leggiImpreseSubscription = this.anagrafeClient.anagrafeLeggiImprese("").subscribe(r => {
      this.imprese = JSON.parse(r.RispostaStringa);
    });

  }

  ngAfterViewInit(): void {
    this.drawer.toggle();
    this.onSubmit(true);
  }

  createFormGroup(mask: FormReportImpiegoProdottiFitosanitariConfig) {
    return this.fb.group({
      Province: new FormControl(mask.Province),
      Comuni: new FormControl(mask.Comuni),
      Data_Da: new FormControl<Date>(mask.Data_Da),
      Data_A: new FormControl<Date>(mask.Data_A),
      ZVN: new FormControl<boolean>(mask.ZVN),
      SostanzeAttiveDropdown: new FormControl(mask.SostanzeAttiveDropdown),
      VisualizzaProdotti: new FormControl<boolean>(mask.VisualizzaProdotti),
      IncludiFertilizzanti: new FormControl<boolean>(mask.IncludiFertilizzanti),
      ImpreseMultiSelect: new FormControl(mask.Imprese)
    });
  }

  onSubmit(initialLoading: boolean) {

    let province = this.inputForm.get('Province').value;
    let comuni = this.inputForm.get('Comuni').value;
    let dataDa = <Date>this.inputForm.get('Data_Da').value;
    let dataA = <Date>this.inputForm.get('Data_A').value;
    let zvn = this.inputForm.get('ZVN').value;
    let sostanzaAttiva = <SostanzaAttiva>this.inputForm.get('SostanzeAttiveDropdown').value;
    let visualizzaProdotti = this.inputForm.get('VisualizzaProdotti').value;
    let includiFertilizzanti = this.inputForm.get('IncludiFertilizzanti').value;
    let imprese = <ImpresaModel[]>this.inputForm.get('ImpreseMultiSelect').value;
    let pive: string[] = [];

    if (imprese.length > 0) {
      pive = imprese.map(i => i.Piva);
    }

    var comuneDatiInArray: ComuneDatiIn[] = comuni.map((c: any) => ({
      PROV: c.PROV,
      COM: c.COM_LOCALITA
    }));

    var provinceIn: string[] = province.map((p: any) => p.PROV);

    let reportImpiegoProdottiFitosanitari_IN: ReportImpiegoProdottiFitosanitari_IN = {
      DataInizio: dataDa,
      DataFine: dataA,
      ZVN: zvn,
      IncludiFertilizzanti: includiFertilizzanti,
      VisualizzaProdotti: visualizzaProdotti,
      Pa_Cod: sostanzaAttiva.Pa_Cod,
      Comuni: comuneDatiInArray,
      Province: provinceIn,
      Imprese: pive,
      InitialLoading: initialLoading
    };

    this.gridReportFitoHttpService.nextParametriIn(reportImpiegoProdottiFitosanitari_IN);
    this.showGrid = true;
  }

  onOpenFilters() {
    this.drawer.toggle();
  }

  ngOnDestroy(): void {
    if (this.getPrincipiAttiviSubscription) {
      this.getPrincipiAttiviSubscription.unsubscribe();
    }

    if (this.leggiImpreseSubscription) {
      this.leggiImpreseSubscription.unsubscribe();
    }
  }
}
