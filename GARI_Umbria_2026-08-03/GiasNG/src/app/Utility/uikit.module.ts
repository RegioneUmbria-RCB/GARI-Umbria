import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DateInputsModule, DateRangeModule } from '@progress/kendo-angular-dateinputs';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { LabelModule } from '@progress/kendo-angular-label';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { SliderModule } from '@progress/kendo-angular-inputs';
import { ExcelModule, GridModule, PDFModule } from '@progress/kendo-angular-grid';
import { CodiciTemplateComponent } from './Template/codici-template/codici-template.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { A11yModule } from '@angular/cdk/a11y';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { PortalModule } from '@angular/cdk/portal';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CdkStepperModule } from '@angular/cdk/stepper';
import { CdkTableModule } from '@angular/cdk/table';
import { CdkTreeModule } from '@angular/cdk/tree';
import { MenusModule } from '@progress/kendo-angular-menu';
import { TranslocoRootModule } from '../transloco/transloco-root.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { KendoTreeContainerComponent } from './Template/kendo-tree/tree-container/tree-container.component';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { TreeViewComponent } from './Template/kendo-tree/tree/tree.component';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import DraggableDirective from './Template/kendo-tree/draggable/draggable.directive';
import { DraggableComponent } from './Template/kendo-tree/draggable/draggable.component';
import { AnagraficaTreeFiltersComponent } from './Template/kendo-tree/filters/anagrafica-tree-filters.component';
import { ScrollingModule as ExperimentalScrollingModule } from '@angular/cdk-experimental/scrolling';
import { ListViewModule } from "@progress/kendo-angular-listview";
import { GiasBreadcrumbsComponent } from './Template/breadcrumbs/breadcrumbs.component';
import { GisTreeFiltersComponent } from './Template/kendo-tree/filters/gis-tree-filters.component';
import { SvgIconSpeciesComponent } from "./Template/svg-icon-species/svg-icon-species.component";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';
import {GiasAddressTemplateComponent} from './Template/address/gias-address-template/gias-address-template.component';


export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});


@NgModule({
  declarations: [
    CodiciTemplateComponent,
    KendoTreeContainerComponent,
    TreeViewComponent,
    DraggableComponent,
    DraggableDirective,
    AnagraficaTreeFiltersComponent,
    GisTreeFiltersComponent,
    GiasBreadcrumbsComponent,
    SvgIconSpeciesComponent,
    GiasAddressTemplateComponent
  ],
  exports: [
    CodiciTemplateComponent,
    A11yModule,
    ClipboardModule,
    CdkStepperModule,
    CdkTableModule,
    CdkTreeModule,
    DragDropModule,
    PortalModule,
    ScrollingModule,
    GridModule,
    TooltipModule,
    NavigationModule,
    KendoTreeContainerComponent,
    TreeViewModule,
    HttpClientModule,
    HttpClientJsonpModule,
    DialogsModule,
    DropDownsModule,
    SliderModule,
    GiasBreadcrumbsComponent,
    ListViewModule,
    LayoutModule,
    SvgIconSpeciesComponent,
    GiasAddressTemplateComponent
  ],
  imports: [
    A11yModule,
    ClipboardModule,
    CdkStepperModule,
    CdkTableModule,
    CdkTreeModule,
    DragDropModule,
    CommonModule,
    GridModule,
    PDFModule,
    ExcelModule,
    PDFModule,
    DropDownsModule,
    LabelModule,
    ButtonsModule,
    InputsModule,
    LayoutModule,
    DateInputsModule,
    FormsModule,
    ReactiveFormsModule,
    PortalModule,
    ScrollingModule,
    MenusModule,
    TranslocoRootModule,
    FontAwesomeModule,
    TooltipModule,
    NavigationModule,
    TreeViewModule,
    HttpClientModule,
    HttpClientJsonpModule,
    ScrollingModule,
    DialogsModule,
    ExperimentalScrollingModule,
    DateRangeModule,
    ListViewModule,
    GiasKendoGridModule,
    GiasUikitModule
  ],
  providers: [
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'uikit',
        loader
      }
    }
  ],
})
export class UikitModule {
}
