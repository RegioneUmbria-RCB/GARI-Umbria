import { CdkDragDrop, CdkDragMove, CdkDragStart } from '@angular/cdk/drag-drop';
import {AfterViewInit, Component, EventEmitter, OnInit, Output} from '@angular/core';
import { Aggiorna_Widgets_In, WidgetsClient, Widget, MenuClient } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { WidgetConfigService } from 'app/widget-config/widget-config.service';
import { BehaviorSubject, debounceTime, Subscription } from 'rxjs';
import { getSpanAsNumbers } from './widgets/widget-resizer/widget-resizer.component';
import { tap } from "rxjs/operators";
import { GestioneMultiAziendaService } from 'app/widget-config/gestione-multiazienda.service';
import {IUtenteDTO} from "../profilazione/models/utente-dto.model";
import { MatomoService } from 'app/matomo/matomo.service';
import {CookieOptions, CookieService} from 'ngx-cookie-service';

export interface WidgetData extends Widget {
  aspettoReale: WidgetPositionData;
};

export interface WidgetPositionData {
  position: number;
  gridArea: string;
  minX: number;
  minY: number;
};

export interface Countries_IN {
  code: string;
  descr: string;
};

const ROWS = 30;
const COLS = 5;
const SCROLL_SIZE = 100;
const SCROLL_FACTOR = 0.5;

@Component({
  standalone: false,
  selector: 'app-widgets',
  templateUrl: './widgets.component.html',
  styleUrls: ['./widgets.component.css']
})
export class WidgetsComponent implements AfterViewInit, OnInit {
  @Output() loaded = new EventEmitter<void>();

  piva: string | null = null;
  rag_soc: string | null = null;
  elements: WidgetData[] = [];
  placeholderWidth: number = 0;
  placeholderHeight: number = 0;

  private section: HTMLElement;
  private pivaSub: Subscription;
  private widgetConfigSub: Subscription;
  private widgetPresetSub: Subscription;
  private dragEl: HTMLElement | null;
  private user: IUtenteDTO | null = null;

  private userLoaded = new BehaviorSubject<boolean>(false);

  constructor(
    private widgetsClient: WidgetsClient,
    private menuClient: MenuClient,
    private objParametriAgendaService: ObjParametriAgendaService,
    private widgetConfigService: WidgetConfigService,
    private giasMessageService: GiasMessageService,
    private gestioneMultiAziendaService: GestioneMultiAziendaService,
    private matomoService: MatomoService,
    private cookieService: CookieService
  ) {
    this.piva = this.objParametriAgendaService.getObjParamValue()?.Piva;
    this.rag_soc = this.objParametriAgendaService.getObjParamValue()?.RagSoc;

    this.pivaSub = this.objParametriAgendaService.currentObjParametriAgenda.subscribe(obj => {
      this.piva = obj?.Piva;
      this.rag_soc = obj?.RagSoc;
    });
    this.widgetConfigSub = this.widgetConfigService
      .onConfigurationChange()
      .pipe(
        debounceTime(1000),
        tap(() => {
          this.getData();
        })
      )
      .subscribe();

    this.widgetPresetSub = this.widgetConfigService
      .onSaveWidgetPreset()
      .subscribe(() => this.saveWidgetPreset());

    this.menuClient
      .menuInformazioniUtente()
      .subscribe(user => {
        this.user = <any>user.RispostaStringa;
        this.userLoaded.next(true);
        this.getData();
      });
  }

  ngOnInit(): void {
    // necessary to avoid unmanaged behavior in DSS Difesa page
    const cookieUseDateRangeName = `DSS.Indicatori.Periodo.${this.piva}.UseDateRange`;
    this.cookieService.delete(cookieUseDateRangeName);
  }

  ngAfterViewInit(): void {
    this.section = document.getElementById('widget-section');
  }

  ngOnDestroy(): void {
    this.pivaSub.unsubscribe();
    this.widgetConfigSub.unsubscribe();
    this.widgetPresetSub.unsubscribe();
  }

  onMouseEnter($event: MouseEvent, widget: string | null): void {
    if (widget != null) {
      const resizer = this.getElement($event.target as HTMLElement, '.custom-resizer-handle');
      if (resizer != null) {
        resizer.classList.remove('hide');
      }

      const drag = this.getElement($event.target as HTMLElement, '.custom-drag-handle');
      if (drag != null) {
        drag.classList.remove('hide');
      }
    }

    if (this.dragEl != null) {
      const element = this.getParent($event.target);
      element.classList.add('ghost');
    }
  }

  onMouseLeave($event: MouseEvent, widget: string | null): void {
    if (widget != null) {
      const resizer = this.getElement($event.target as HTMLElement, '.custom-resizer-handle');
      if (resizer != null) {
        resizer.classList.add('hide');
      }

      const drag = this.getElement($event.target as HTMLElement, '.custom-drag-handle');
      if (drag != null) {
        drag.classList.add('hide');
      }
    }

    if (this.dragEl != null) {
      const element = this.getParent($event.target);
      element.classList.remove('ghost');
    }
  }

  cdkDropListDropped(event: CdkDragDrop<any, any>): void {
    // document.elementFromPoint in order to make it work on mobile
    const element = document.elementFromPoint(event.dropPoint.x, event.dropPoint.y);
    const target = this.getParent(element);
    if (target == null) {
      return;
    }

    const pos1 = Array.prototype.indexOf.call(this.section.children, event.item.element.nativeElement);
    const pos2 = Array.prototype.indexOf.call(this.section.children, target);
    if (pos1 == -1) {
      // We are moving the resizer
      return;
    }

    target.classList.remove('ghost');

    const elements = JSON.parse(JSON.stringify(this.elements));
    this.swapElementsPosition(elements, pos1, pos2);
    elements.sort(sortByPosition);

    // Simulate the widget movement
    const matrix = this.createMatrix(elements);
    if (matrix == null) {
      this.giasMessageService.errorMessage("WidgetNonSpostabile", false, true);
      return;
    }

    // Movement ok
    this.swapElementsPosition(this.elements, pos1, pos2);
    this.elements.sort(sortByPosition);

    this.elements = this.createElementsFromMatrix(this.elements.filter(x => x.IdWidget != null), matrix);
    this.saveData();
  }

  private swapElementsPosition(elements: WidgetData[], pos1: number, pos2: number): void {
    const tmp2 = elements[pos1].aspettoReale.position;
    elements[pos1].aspettoReale.position = elements[pos2].aspettoReale.position;
    elements[pos2].aspettoReale.position = tmp2;
  }

  cdkDragStarted($event: CdkDragStart): void {
    this.dragEl = this.getParent($event.event.target);
    this.placeholderWidth = this.dragEl.offsetWidth;
    this.placeholderHeight = this.dragEl.offsetHeight;
  }

  cdkDragEnded(): void {
    this.dragEl = null;
    this.placeholderWidth = 0;
    this.placeholderHeight = 0;
  }

  saveData(): void {
    const elements = this.elements
      .filter(x => x.Codice != null)
      .map(x => {
        x.Aspetto = JSON.stringify(x.aspettoReale)
        return x;
      });

    this.widgetsClient
      .widgetsAggiornaWidgetUtente({ widgets: elements } as Aggiorna_Widgets_In)
      .subscribe();
  }

  cdkDragMoved($event: CdkDragMove<any>): void {
    if ($event.pointerPosition.y < SCROLL_SIZE) {
      const scroll = Math.min(Math.max(-SCROLL_SIZE + (0 + $event.pointerPosition.y), -SCROLL_SIZE), SCROLL_SIZE);
      document.body.scrollBy(0, scroll * SCROLL_FACTOR);
    }

    if (document.body.clientHeight - $event.pointerPosition.y < SCROLL_SIZE) {
      const scroll = Math.max(Math.min(SCROLL_SIZE - (document.body.clientHeight - $event.pointerPosition.y), SCROLL_SIZE), 0);
      document.body.scrollBy(0, scroll * SCROLL_FACTOR);
    }
  }

  getWidgetClass(widget: WidgetData): string {
    if (widget.Codice == null) {
      return 'parent empty-parent';
    }

    let result = `parent`;
    if (widget.Codice == 'Meteo' && widget.aspettoReale?.gridArea?.includes('/ span 1')) {
      result += ' meteo-widget';
    }

    return result;
  }

  private getElement(parent: HTMLElement, selector: string): Element {
    const res = parent.querySelectorAll(selector);
    return res.length > 0 ? res[0] : null;
  }

  private getData(): void {
    let username = this.user?.UserName ?? "";
    this.widgetsClient
      .widgetsElencoWidgets({ Abilitato: true, Visibile: true, UserName: username })
      .subscribe(response => {
        const elements = response.RispostaStringa.map(element => ({ ...element, aspettoReale: element.Aspetto ? JSON.parse(element.Aspetto) : null } as WidgetData)).filter(element => element.MultiAziendale == this.readSwitchMultiAzienda());
        this.handleVoidInput(elements);
        elements.sort(sortByPosition);
        const matrix = this.createMatrix(elements);
        this.elements = this.createElementsFromMatrix(elements.filter(x => x.IdWidget != null), matrix);

        //comunico a Matomo la lista dei widgets
        this.matomoService.pushEnabledWidgets(elements.filter(x => x.IdWidget != null));

        this.loaded.emit();
      });
  }

  private createMatrix(elements: WidgetData[]): number[][] {
    const matrix = []

    for (let i = 0; i < ROWS; i++) {
      matrix[i] = [];
      for (let j = 0; j < COLS; j++) {
        matrix[i][j] = -1;
      }
    }

    for (let index = 0; index < elements.length; index++) {
      const element = elements[index];
      if (element == null) {
        continue;
      }

      let pos = element.aspettoReale.position;
      while (!this.isValidPositionInMatrix(pos, element, matrix)) {
        pos++;
        if (pos > ROWS * COLS) {
          // Error creating the matrix
          return null;
        }
      }

      this.placeElementInMatrix(pos, element, matrix)
    }

    return matrix;
  }

  private isValidPositionInMatrix(position: number, element: WidgetData, matrix: number[][]): boolean {
    const { spanX, spanY } = getSpanAsNumbers(element.aspettoReale.gridArea);
    const row = Math.floor(position / COLS)
    const col = position % COLS;

    for (let i = 0; i < spanY; i++) {
      for (let j = 0; j < spanX; j++) {
        if (row + i >= ROWS || col + j >= COLS || matrix[row + i][col + j] != -1) {
          return false;
        }
      }
    }

    return true;
  }

  private placeElementInMatrix(position: number, element: WidgetData, matrix: number[][]): void {
    const { spanX, spanY } = getSpanAsNumbers(element.aspettoReale.gridArea);
    const row = Math.floor(position / COLS)
    const col = position % COLS;

    for (let i = row; i < row + spanY; i++) {
      for (let j = col; j < col + spanX; j++) {
        matrix[i][j] = element.IdWidget ?? -1;
      }
    }
  }

  private createElementsFromMatrix(elements: WidgetData[], matrix: any): WidgetData[] {
    const result = [];
    const ids = new Set<number>();

    for (let i = 0; i < ROWS; i++) {
      for (let j = 0; j < COLS; j++) {
        const pos = i * COLS + j;

        if (matrix[i][j] == -1) {
          result.push({ aspettoReale: { position: pos, gridArea: "span 1 / span 1" } } as WidgetData)
          continue;
        }

        if (ids.has(matrix[i][j])) {
          continue;
        }

        const element = elements.find(x => x.IdWidget == matrix[i][j]);
        element.aspettoReale.position = pos;
        if (element == null) {
          throw new Error("Error while parsing the matrix");
        }

        result.push(element);
        ids.add(matrix[i][j]);
      }
    }

    return result;
  }

  private handleVoidInput(elements: WidgetData[]): void {
    for (const element of elements) {
      if (element.aspettoReale == null || Object.keys(element.aspettoReale).length === 0) {
        element.aspettoReale = { position: 0, gridArea: 'span 1 / span 1', minX: 1, minY: 1 } as WidgetPositionData
      }
    }
  }

  private getParent(target: EventTarget | null): HTMLElement | null {
    return (target as HTMLElement)?.closest('.parent');
  }

  private saveWidgetPreset(): void {
    const elements = this.elements.filter(x => x.Codice != null);
    // this.widgetsClient // avoid saving because of backend problem, uncomment when fixed
    //   .savePreset(elements)
    //   .subscribe(() => this.giasMessageService.successMessage("ImpostaComePresetSalvataggio", false, true));

    this.giasMessageService.successMessage("ImpostaComePresetSalvataggio", false, true);
  }

  readSwitchMultiAzienda(): boolean {
    return this.gestioneMultiAziendaService.widgetsMultiAzienda;
  }

}

function sortByPosition(a: WidgetData, b: WidgetData): number {
  return a.aspettoReale.position - b.aspettoReale.position;
}
