import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { IconsModule } from '@progress/kendo-angular-icons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { UikitModule } from '../Utility/uikit.module';
import { BottoniTestataComponent } from './agenda-edit/bottoni/bottoni-testata/bottoni-testata.component';
import { RilevaFaseEpocaComponent } from './agenda-edit/bottoni/rileva-fase-epoca/rileva-fase-epoca.component';
import { AcquaComponent } from './agenda-edit/componenti/acqua/acqua.component';
import { GridDosiProdottiComponent } from './agenda-edit/componenti/grid-dosi-prodotti/grid-dosi-prodotti.component';
import { GridImpiantiComponent } from './agenda-edit/componenti/grid-impianti/grid-impianti.component';
import { ProdottiComponent } from './agenda-edit/componenti/prodotti/prodotti.component';
import { SuperficiImpiantiComponent } from './agenda-edit/componenti/superfici-impianti/superfici-impianti.component';
import { TestataComponent } from './agenda-edit/componenti/testata/testata.component';
import { QuadernoDiCampagnaComponent } from './agenda-edit/quaderno-di-campagna.component';
import { TrattamentoComponent } from './agenda-edit/trattamenti/trattamento.component';
import { QuadernoDiCampagnaRoutingModule } from './quaderno-di-campagna-routing.module';
import { GridMacchineComponent } from './agenda-edit/componenti/grid-macchine/grid-macchine.component';
import { GridOperatoriComponent } from './agenda-edit/componenti/grid-operatori/grid-operatori.component';
import { CostiAccessoriComponent } from './agenda-edit/componenti/costi-accessori/costi-accessori.component';
import { MultiOperazioneComponent } from './agenda-edit/componenti/testata/multi-operazione/multi-operazione.component';
import { GridNoteComponent } from './agenda-edit/componenti/grid-note/grid-note.component';
import { FormulatiComponent } from './agenda-edit/componenti/prodotti/sezioni/formulati/formulati.component';
import { FertilizzantiComponent } from './agenda-edit/componenti/prodotti/sezioni/fertilizzanti/fertilizzanti.component';
import { SementiComponent } from './agenda-edit/componenti/prodotti/sezioni/sementi/sementi.component';
import { MagazzinoLottoComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/magazzino-lotto/magazzino-lotto.component';
import { UnitaDiMisuraComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/unita-di-misura/unita-di-misura.component';
import { DosiComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/dosi/dosi.component';
import { FlagVisualizzaGiacenzeZeroComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/flag-visualizza-giacenze-zero/flag-visualizza-giacenze-zero.component';
import { AvversitaComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/avversita/avversita.component';
import { OpzioniSeminaComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/opzioni-semina/opzioni-semina.component';
import { NotaTestualeComponent } from './agenda-edit/componenti/nota-testuale/nota-testuale.component';
import { BottoniSezioneProdottiComponent } from './agenda-edit/bottoni/bottoni-sezione-prodotti/bottoni-sezione-prodotti.component';
import { RicercaFormulatiComponent } from './agenda-edit/componenti/prodotti/sezioni/formulati/ricerca-formulati/ricerca-formulati.component';
import { RicercaFertilizzantiComponent } from './agenda-edit/componenti/prodotti/sezioni/fertilizzanti/ricerca-fertilizzanti/ricerca-fertilizzanti.component';
import { RicercaSementiComponent } from './agenda-edit/componenti/prodotti/sezioni/sementi/ricerca-sementi/ricerca-sementi.component';
import { BottoniSalvataggioComponent } from './agenda-edit/bottoni/bottoni-salvataggio/bottoni-salvataggio.component';
import { DisciplinareComponent } from './agenda-edit/componenti/testata/disciplinare/disciplinare.component';
import { RaccoltaComponent } from './agenda-edit/componenti/prodotti/sezioni/raccolta/raccolta.component';
import { OpzioniRaccoltaComponent } from './agenda-edit/componenti/prodotti/sezioni/raccolta/opzioni-raccolta/opzioni-raccolta.component';
import { GridRaccoltaAutoComponent } from './agenda-edit/componenti/prodotti/sezioni/raccolta/grid-raccolta-auto/grid-raccolta-auto.component';
import { RipartizioneManualeComponent } from './agenda-edit/componenti/prodotti/sezioni/raccolta/ripartizione-manuale/ripartizione-manuale.component';
import { GridRipartizioneManualeComponent } from './agenda-edit/componenti/prodotti/sezioni/raccolta/ripartizione-manuale/grid-ripartizione-manuale/grid-ripartizione-manuale.component';
import { GISModule } from 'app/GIS/GIS.module';
import { GisService } from 'app/GIS/GIS.service';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { DataLayerStyleService } from 'app/GIS/services/data-layer-style.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { LayerStyleService } from 'app/GIS/services/layer-style.service';
import { PolygonLabelService } from 'app/GIS/services/polygon-label.service';
import { TreeGisService } from 'app/Utility/Template/kendo-tree/services/tree-gis.service';
import { GiasUikitModule, LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { WKTService } from 'app/GIS/services/wkt.service';
import { WmsService } from 'app/GIS/services/wms.service';
import { GisToolbarService } from 'app/GIS/GIS-toolbar/gis-toolbar.service';
import { MeasureDistanceService } from 'app/GIS/services/measure-distance.service';
import { PositionService } from 'app/GIS/services/position.service';
import { ProjectDocumentaleDirective } from 'app/menu-agenda/components/directives/projectDocumentale.directive';
import { MenuAgendaModule } from 'app/menu-agenda/menu-agenda.module';
import { QdCLoadedGuard } from 'app/menu-agenda/guards/transloco-qdc-guard.service';
import { GestioneOperazioniComponent } from './gestione-operazioni/gestione-operazioni.component';
import { DataOperazioneComponent } from './agenda-edit/componenti/testata/data-operazione/data-operazione.component';
import { SpecieComponent } from './agenda-edit/componenti/testata/specie/specie.component';
import { BottoneRicercaProdotti } from './agenda-edit/bottoni/bottoni-sezione-prodotti/bottone-ricerca-prodotti/bottone-ricerca-prodotti';
import { DettagliFormulatiComponent } from './agenda-edit/componenti/prodotti/sezioni/formulati/dettagli-formulati/dettagli-formulati.component';
import { BottoniGestioneProdottoComponent } from './agenda-edit/bottoni/bottoni-sezione-prodotti/bottoni-gestione-prodotto/bottoni-gestione-prodotto.component';
import { DettagliFertilizzantiComponent } from './agenda-edit/componenti/prodotti/sezioni/fertilizzanti/dettagli-fertilizzanti/dettagli-fertilizzanti.component';
import { DettagliSementiComponent } from './agenda-edit/componenti/prodotti/sezioni/sementi/dettagli-sementi/dettagli-sementi.component';
import { BottoneCreaProdottoComponent } from './agenda-edit/bottoni/bottoni-sezione-prodotti/bottone-crea-prodotto/bottone-crea-prodotto.component';
import { GestioneNoteComponent } from './agenda-edit/componenti/grid-note/gestione-note/gestione-note.component';
import { BottoneAggiuntaProdottoComponent } from './agenda-edit/bottoni/bottoni-sezione-prodotti/bottone-aggiunta-prodotto/bottone-aggiunta-prodotto.component';
import { TestataRicettaComponent } from './agenda-edit/componenti/testata/testata-ricetta/testata-ricetta.component';
import { RilieviComponent } from './agenda-edit/componenti/sezioni-no-prodotto/rilievi/rilievi.component';
import { GridRilieviComponent } from './agenda-edit/componenti/sezioni-no-prodotto/rilievi/grid-rilievi/grid-rilievi.component';
import { SezioniNoProdottoComponent } from './agenda-edit/componenti/sezioni-no-prodotto/sezioni-no-prodotto.component';
import { FlagMagazziniAgenzieComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/flag-magazzini-agenzie/flag-magazzini-agenzie.component';
import { RilieviAvversitaComponent } from './rilievi-avversita/rilievi-avversita.component';
import { GoogleMapDataService } from '../GIS/services/google.maps-services/google-map-data.service';
import { RetinaturaService } from '../GIS/services/retinatura.service';
import { TestataVisitaComponent } from './agenda-edit/componenti/testata/testata-visita/testata-visita.component';
import { PolygonLabelInfowindowService } from '../GIS/services/polygon-label-infowindow.service';
import { OperatoreComponent } from './agenda-edit/componenti/testata/testata-visita/operatore/operatore.component';
import { AziendaComponent } from './agenda-edit/componenti/testata/testata-visita/azienda/azienda.component';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { ImpreseParametriGHGComponent } from './imprese-parametri-ghg/imprese-parametri-ghg.component';
import { ConfigurazioneOperazioniCulturaliComponent } from './configurazione-operazioni-culturali/configurazione-operazioni-culturali/configurazione-operazioni-culturali.component';
import { InfowindowClustererService } from '../GIS/infowindow-clusterer/infowindow-clusterer.service';
import { SpecieAnimaleComponent } from './agenda-edit/componenti/testata/specie-animale/specie-animale.component';
import { GoogleMapGeoJsonLazyService } from 'app/GIS/services/google.maps-services/google-map-geojson-lazy.service';
import { GoogleMapFeatureService } from 'app/GIS/services/google.maps-services/google-map-feature.service';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';
import { EditFeatureWindowService } from 'app/GIS/services/edit-feature-window.service';
import { EditFeatureService } from 'app/GIS/services/edit-feature.service';
import { FlagMagazziniEsterniComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/flag-magazzini-esterni/flag-magazzini-esterni.component';
import { ModalitaApplicazioneComponent } from './agenda-edit/componenti/prodotti/controlli-comuni/modalita-applicazione/modalita-applicazione.component';
import { AbbattimentoImpiantiComponent } from './agenda-edit/componenti/sezioni-no-prodotto/abbattimento-impianti/abbattimento-impianti.component';
import { GridProdottiDaTrattareComponent } from './agenda-edit/componenti/grid-prodotti-da-trattare/grid-prodotti-da-trattare.component';
import { QuantitaProdottiComponent } from './agenda-edit/componenti/quantita-prodotti/quantita-prodotti.component';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { AnagraficheIndiciMaturitaComponent } from './anagrafiche-indici-maturita/anagrafiche-indici-maturita.component';
import { AnagraficheRilieviAvversitaComponent } from './anagrafiche-rilievi-avversita/anagrafiche-rilievi-avversita.component';
import { OperazioneCausaleComponent } from './operazione-causale/operazione-causale.component';
import { RilievoPioggeComponent } from './agenda-edit/componenti/rilievo-piogge/rilievo-piogge.component';
import { GridProdottiImpiantiComponent } from './agenda-edit/componenti/prodotti/sezioni/formulati/gestione-trappole/grid-prodotti-impianti/grid-prodotti-impianti.component';
import { RisultatiAnalisiConformitaComponent } from 'app/qualita-tracciabilita/components/risultati-analisi-conformita/risultati-analisi-conformita.component';
import {ExportCartographyDataService} from '../GIS/GIS-toolbar/services/export-cartography-data.service';
import {AggiungiComponent} from "./agenda-edit/bottoni/aggiungi/aggiungi.component";
import {
  UdmQtaInnescoComponent
} from "./agenda-edit/componenti/prodotti/sezioni/formulati/gestione-trappole/udm-qta-innesco/udm-qta-innesco.component";

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] = () => import(`./i18n/${lang}.json`);
    return acc;
}, {});

@NgModule({
    declarations: [
        QuadernoDiCampagnaComponent,
        TrattamentoComponent,
        TestataComponent,
        GridImpiantiComponent,
        RilievoPioggeComponent,
        BottoniTestataComponent,
        RilevaFaseEpocaComponent,
        SuperficiImpiantiComponent,
        ProdottiComponent,
        AcquaComponent,
        GridDosiProdottiComponent,
        GridMacchineComponent,
        GridOperatoriComponent,
        CostiAccessoriComponent,
        MultiOperazioneComponent,
        GridNoteComponent,
        FormulatiComponent,
        FertilizzantiComponent,
        SementiComponent,
        MagazzinoLottoComponent,
        UnitaDiMisuraComponent,
        DosiComponent,
        FlagVisualizzaGiacenzeZeroComponent,
        AvversitaComponent,
        OpzioniSeminaComponent,
        NotaTestualeComponent,
        BottoniSezioneProdottiComponent,
        RicercaFormulatiComponent,
        RicercaFertilizzantiComponent,
        RicercaSementiComponent,
        BottoniSalvataggioComponent,
        ProjectDocumentaleDirective,
        DisciplinareComponent,
        RaccoltaComponent,
        OpzioniRaccoltaComponent,
        GridRaccoltaAutoComponent,
        RipartizioneManualeComponent,
        GridRipartizioneManualeComponent,
        GestioneOperazioniComponent,
        DataOperazioneComponent,
        SpecieComponent,
        BottoneRicercaProdotti,
        DettagliFormulatiComponent,
        BottoniGestioneProdottoComponent,
        DettagliFertilizzantiComponent,
        DettagliSementiComponent,
        BottoneCreaProdottoComponent,
        GestioneNoteComponent,
        BottoneAggiuntaProdottoComponent,
        TestataRicettaComponent,
        RilieviComponent,
        GridRilieviComponent,
        SezioniNoProdottoComponent,
        FlagMagazziniAgenzieComponent,
        RilieviAvversitaComponent,
        TestataVisitaComponent,
        OperatoreComponent,
        ImpreseParametriGHGComponent,
        ConfigurazioneOperazioniCulturaliComponent,
        AziendaComponent,
        SpecieAnimaleComponent,
        FlagMagazziniEsterniComponent,
        ModalitaApplicazioneComponent,
        AbbattimentoImpiantiComponent,
        GridProdottiDaTrattareComponent,
        QuantitaProdottiComponent,
        OperazioneCausaleComponent,
        AnagraficheRilieviAvversitaComponent,
        AnagraficheIndiciMaturitaComponent,
        GridProdottiImpiantiComponent,
        AggiungiComponent,
        UdmQtaInnescoComponent
    ],
    exports: [
        QuadernoDiCampagnaComponent
    ],
    imports: [
        CommonModule,
        QuadernoDiCampagnaRoutingModule,
        HttpClientModule,
        UikitModule,
        FormsModule,
        ReactiveFormsModule,
        TranslocoRootModule,
        FontAwesomeModule,
        LayoutModule,
        IconsModule,
        LabelModule,
        InputsModule,
        ButtonsModule,
        GISModule,
        MenuAgendaModule,
        DateInputsModule,
        GiasKendoGridModule,
        GiasUikitModule,
        RisultatiAnalisiConformitaComponent
    ],
    providers: [
        QdCLoadedGuard,
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'qdc',
                loader
            }
        },
        DatePipe,
        DecimalPipe,
        GisService,
        GoogleMapGeoJsonService,
        GoogleMapGeoJsonLazyService,
        GoogleMapService,
        GoogleMapDataService,
        RetinaturaService,
        DataLayerStyleService,
        LayerStyleService,
        PolygonLabelService,
        PolygonLabelInfowindowService,
        FeatureService,
        InfowindowClustererService,
        TreeGisService,
        WKTService,
        WmsService,
        GisToolbarService,
        ExportCartographyDataService,
        MeasureDistanceService,
        GiasMultiSelectTemplateService,
        GiasDropDownTemplateService,
        PositionService,
        GoogleMapFeatureService,
        FeatureInformationService,
        EditFeatureWindowService,
        EditFeatureService,
        { provide: LOADING_TOKEN, useClass: LoadingService },
    ]
})
export class QuadernoDiCampagnaModule { }
