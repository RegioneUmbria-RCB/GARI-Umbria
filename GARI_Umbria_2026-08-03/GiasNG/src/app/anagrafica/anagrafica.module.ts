import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { WindowModule } from '@progress/kendo-angular-dialog';
import { IconsModule } from '@progress/kendo-angular-icons';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { CampiServiceProvider } from 'app/Service/ServiceFactory/campi.factory.provider';
import { CatastoServiceProvider } from 'app/Service/ServiceFactory/catasto.factory.provider';
import { ContattiServiceProvider } from 'app/Service/ServiceFactory/contatti.factory.provider';
import { ImpiantiServiceProvider } from 'app/Service/ServiceFactory/impianti.factory.provider';
import { ImpreseServiceProvider } from 'app/Service/ServiceFactory/imprese.factory.provider';
import { InvestimentoCatastaleServiceProvider } from 'app/Service/ServiceFactory/investimento-catastale.factory.provider';
import { MacchineServiceProvider } from 'app/Service/ServiceFactory/macchine.factory.provider';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { UikitModule } from '../Utility/uikit.module';
import { AnagraficaRoutingModule } from './anagrafica-routing.module';
import { AnagraficaComponent } from './anagrafica.component';
import { AppezzamentiComponent } from './appezzamenti/appezzamenti.component';
import { CampoModule } from './campi/campi-edit/campo.module';
import { CampiComponent } from './campi/campi.component';
import { CampiShortDescriptionComponent } from './campi/campiShortDescription/campi-short-description.component';
import { InvestimentoCatastaleCampoComponent } from './campi/investimento-catastale-campo/investimento-catastale-campo.component';
import { CatastoComponent } from './catasto/catasto.component';
import { InvestimentoCatastaleComponent } from './catasto/investimento-catastale/investimento-catastale.component';
import { CentriModule } from './centri/centri.module';
import { ContattiComponent } from './contatti/contatti.component';
import { EserciziComponent } from './esercizi/esercizi.component';
import { ImgBase64Component } from './esercizi/imgBase64/img-base64.component';
import { ImpiantoShortDescriptionComponent } from './esercizi/ImpiantoShortDescription/impianto-short-description.component';
import { NuovaOperazioneComponent } from './esercizi/nuova-operazione/nuova-operazione.component';
import { FabbricatiComponent } from './fabbricati/fabbricati.component';
import { SelezionaCentroComponent } from './fabbricati/seleziona-centro/seleziona-centro.component';
import { ImpiantiComponent } from './impianti/impianti.component';
import { ImpreseComponent } from './imprese/imprese.component';
import { MacchinaShortDescriptionComponent } from './macchine/macchinaShortDescription/macchina-short-description.component';
import { MacchineComponent } from './macchine/macchine.component';
import { GisToolbarService } from 'app/GIS/GIS-toolbar/gis-toolbar.service';
import { MeasureDistanceService } from 'app/GIS/services/measure-distance.service';
import { PositionService } from 'app/GIS/services/position.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { EserciziCopiaSpostaAppezzamentiComponent } from './esercizi/esercizi-copiaSpostaAppezzamenti/esercizi-copia-sposta-appezzamenti.component';
import { InvestimentoCatastaleAppezzamentoComponent } from './esercizi/investimento-catastale-appezzamento/investimento-catastale-appezzamento.component';
import { OnCloseEsercizioComponent } from './esercizi/on-close-esercizio/on-close-esercizio.component';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';
import { AgriculturalPlotMachineLinkComponent } from './esercizi/special-edit/agricultural-plots/agricultural-plots-machines-link/agricultural-plot-machine-link.component';
import { AgriculturalPlotsMachinesLinkService } from './esercizi/special-edit/agricultural-plots/agricultural-plots-machines-link/agricultural-plots-machines-link.service';
import { AxpMessageContentComponent } from './esercizi/special-edit/agricultural-plots/agricultural-plots-machines-link/axp-message-content/axp-message-content.component';
import { AgriculturalExerciseContributeLinkComponent } from './esercizi/special-edit/exercices/agricultural-exercise-contribute-link/agricultural-exercise-contribute-link.component';
import {ExportCartographyDataService} from '../GIS/GIS-toolbar/services/export-cartography-data.service';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] = () => import(`./i18n/${lang}.json`);
    return acc;
}, {});

@NgModule({
    declarations: [
        AnagraficaComponent,
        ImpreseComponent,
        AppezzamentiComponent,
        CatastoComponent,
        CampiComponent,
        FabbricatiComponent,
        ContattiComponent,
        MacchineComponent,
        ImpiantiComponent,
        EserciziComponent,
        NuovaOperazioneComponent,
        SelezionaCentroComponent,
        InvestimentoCatastaleComponent,
        InvestimentoCatastaleCampoComponent,
        ImgBase64Component,
        ImpiantoShortDescriptionComponent,
        MacchinaShortDescriptionComponent,
        CampiShortDescriptionComponent,
        EserciziCopiaSpostaAppezzamentiComponent,
        InvestimentoCatastaleAppezzamentoComponent,
        OnCloseEsercizioComponent,
        AgriculturalPlotMachineLinkComponent,
        AxpMessageContentComponent,
        AgriculturalExerciseContributeLinkComponent,
    ],
    exports: [
        AnagraficaComponent,
        ImpreseComponent,
        AppezzamentiComponent,
        CentriModule,
        CampoModule,
        MacchineComponent,
        NavigationModule,
        AgriculturalPlotMachineLinkComponent,
        AgriculturalExerciseContributeLinkComponent,
        ImpiantoShortDescriptionComponent
    ],
    providers: [
        ReactiveFormsModule,
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'anagrafica',
                loader
            }
        },
        ImpreseServiceProvider,
        CatastoServiceProvider,
        CampiServiceProvider,
        ImpiantiServiceProvider,
        ContattiServiceProvider,
        MacchineServiceProvider,
        InvestimentoCatastaleServiceProvider,
        GisToolbarService,
        ExportCartographyDataService,
        MeasureDistanceService,
        PositionService,
        GiasDropDownTemplateService,
        AgriculturalPlotsMachinesLinkService
    ],
    imports: [
        CommonModule,
        AnagraficaRoutingModule,
        HttpClientModule,
        ReactiveFormsModule,
        UikitModule,
        TranslocoRootModule,
        FontAwesomeModule,
        CentriModule,
        CampoModule,
        FormsModule,
        NavigationModule,
        IconsModule,
        ButtonsModule,
        WindowModule,
        LayoutModule,
        GiasKendoGridModule,
        GiasUikitModule
    ]
})

export class AnagraficaModule {
}
