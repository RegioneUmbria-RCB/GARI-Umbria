import { Inject, Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { ColumnComponent } from '@progress/kendo-angular-grid';
import { GiasMessageService } from '../../../../shared/gias-message.service';
import { cloneDeep } from 'lodash';
import { CookieService } from 'ngx-cookie-service';
import { Observable, of, Subject } from 'rxjs';
import { map, skip, tap } from 'rxjs/operators';
import { DropdownList, DropdownListItem, GRID_HTTP_TOKEN, KendoGridColumn, DropdownListItem as DropdownItem } from '../../models/grid.model';
import { AbstractGridConfigService } from '../../services/grid-config.service';
import { KendoGridService } from '../../services/kendo-grid.service';
import { CustomizeDropdownService } from './dropdown/customization-dropdown.service';
import { ChiaveVista, CustomizationRequest, DropdownChanged, InMemoryView, SalvaVisteWrapper, ServerView } from './model';
import { parseJson } from './utility';
import { Router } from "@angular/router";
import { GridPublicService } from '../../services/grid-public.service';
import { FiltroJSONService } from '../../services/filtro-json.service';
import { CELL_TYPES, ConversionService, GIAS_API_SERVICE_TOKEN, GIAS_USERNAME_TOKEN, IGiasApiService, IUsernameService, RispostaStandard } from 'gias-ui-kit';


@Injectable()
export class GridCustomizationsService {
  public customizationRequest: Subject<CustomizationRequest> = new Subject();

  private _inMemoryViews: InMemoryView[] = [];
  private _serverViews: InMemoryView[] = [];

  public username: string;

  public permessoPubblica: boolean = false;
  public switchValue: boolean = false;

  selectedDDLItem: DropdownListItem;

  public viewToApply: Subject<DropdownListItem> = new Subject();

  constructor(
    @Inject(GIAS_USERNAME_TOKEN) userService: IUsernameService,
    @Inject(GIAS_API_SERVICE_TOKEN) private ajaxAgronicaAPIService: IGiasApiService,
    public dropdownService: CustomizeDropdownService,
    private cookies: CookieService,
    private giasMessageService: GiasMessageService,
    private privateGridService: KendoGridService,
    private transloco: TranslocoService,
    private conversionService: ConversionService,
    private gridPublicService: GridPublicService,
    private filtroJSONService: FiltroJSONService,
    @Inject(GRID_HTTP_TOKEN) public conf: AbstractGridConfigService<any>, // GridNoteServerResult
    private router: Router) {
    this.username = userService.getCurrentUser()?.Username;
  }

  public checkIfThereIsAForm() {
    return this.gridPublicService.thereIsAForm && this.filtroJSONService.isFirstApplyView;
  }

  public setFormFields(view: InMemoryView) {
    if (this.gridPublicService.thereIsAForm) {
      this.filtroJSONService.isFirstApplyView = false;
      //this.persistView(this.selectedDDLItem);
      this.gridPublicService.applyViewFiltroJSON.next(view);
    }
  }

  public get inMemoryViews(): InMemoryView[] {
    return cloneDeep(this._inMemoryViews);
  }

  inMemViewsLoaded = false;
  cacheViewWasApplied = false;

  public init(kendoGrid: any) {
    if (!this.inMemViewsLoaded) {
      this.loadInMemoryViews(kendoGrid);
    }
  }

  private loadInMemoryViews(kendoGrid: any) {
    const chiave = this.CreateNewKeyView(null, this.username);

    this.ajaxAgronicaAPIService.ajaxAPIPost('Shared/VisteGriglia_CaricaViste', chiave).pipe(map(viste => {
      this.mapViews(kendoGrid, viste.RispostaStringa as ServerView[]);
      this.handleCacheItem(this._serverViews);
    })).subscribe()
  }

  public initViews() {
  }

  private mapViews(kendoGrid: any, views: ServerView[]): void {
    this._inMemoryViews = views.map((view) => {
      let c = JSON.parse(view['Colonne']);
      let s = JSON.parse(view['Stato']);
      c = this.conversionService.ConversionDateInObject(c);
      s = this.conversionService.ConversionDateInObject(s);
      return new InMemoryView({
        IdVista: view['IdVista'],
        NomeUtente: view['NomeUtente'],
        NomeVista: view['NomeVista'],
        GridId: view['GridId'],
        Predefinita: view['Predefinita'],
        IsModified: false,
        IsNew: false,
        Colonne: c,
        Stato: s,
        FiltroJSON: view['FiltroJSON'],
        FlagPubblica: view['FlagPubblica']
      })
    });

    if (!this._inMemoryViews.find(v => v.Predefinita)) {
      const baseView = new InMemoryView({
        NomeVista: this.transloco.translate('giasgrid.Base'),
        Predefinita: true,
        NomeUtente: this.username,
        IsNew: true,
        GridId: this.CreateNewGridIdforView(),
        IdVista: this.dropdownService.generateUID()
      } as InMemoryView);

      this.fillCurrentView(baseView, kendoGrid._gridCtx, kendoGrid._gridCtx.columns);
      this._inMemoryViews.push(baseView);
    }

    this._serverViews = cloneDeep(this._inMemoryViews);
    this.inMemViewsLoaded = true;
  }

  private handleCacheItem(viste: InMemoryView[]) {
    const item = this.loadCacheItem(viste);

    const initializer = this.transformData(viste, item);
    this.dropdownService.next(initializer);

    this.cacheViewWasApplied = true;
  }


  private loadCacheItem(viste: InMemoryView[]): DropdownListItem {

    let cache_view: DropdownItem = null;

    //se voglio che venga caricata la vista in cache
    if (this.filtroJSONService.applyLoadCacheFirstTime) {

      cache_view = this.SearchCookie();

      //Se la vista memorizzata nei Cookie non è più presente nell'elenco non la propongo
      if (cache_view) {
        let index = viste.findIndex(v => v.IdVista === cache_view.id);

        if (index === -1)
          cache_view = null;
      }

    }

    if (!cache_view) {
      const defaultItem = this._serverViews.find(x => x.Predefinita);
      if (defaultItem != null) {
        cache_view = {
          id: defaultItem.IdVista,
          name: defaultItem.NomeVista,
          data: defaultItem
        }
      }
    }

    //Add Additional Info to Item
    if (cache_view && cache_view.data) {

      if (!cache_view.data.NomeUtente || cache_view.data.NomeUtente === "") {
        cache_view.data.NomeUtente = this.username;
      }

      if (!cache_view.data.GridId || cache_view.data.GridId === "") {
        cache_view.data.GridId = this.CreateNewGridIdforView();
      }

    }

    return cache_view;
  }

  private findExistingCookie() {
    return this.SearchCookie();
  }

  private SearchCookie(): DropdownItem {
    const Search = (keyTofind: string) => {
      const json = this.cookies.getAll();

      for (const key in json) {
        if (key.includes(keyTofind)) {
          return parseJson(json[key]);
        }
      }

      return null;
    };

    //Prima cerco il cookie con il nuovo GridId (nomeServizioGriglia|PartefinaleURL)
    //Se non lo trovo cerco per il vecchio GridId(nomeServizioGriglia)

    let cookie_value: DropdownItem = null;

    // cookie_value = Search(this.CreateNewGridIdforView() + '-'+this.username+ '-item-');
    cookie_value = Search(this.CreateNewGridIdforView() + '-' + this.username);

    if (!cookie_value) {
      cookie_value = Search(this.conf.gridId + '-' + this.username);
    }


    return cookie_value;

  }

  public saveAllViews() {

    const postData: SalvaVisteWrapper = this.prepareToSaveViews(this.inMemoryViews);

    if (!postData.VisteModificate.length && !postData.VisteNuove.length) {
      return;
    }

    this.ajaxAgronicaAPIService.ajaxAPIPost('Shared/VisteGriglia_ScriviViste', postData).subscribe((dati) => {
      this.inMemoryViews.forEach((vista) => {
        vista.IsModified = false;
        vista.IsNew = false;
      });

      let messaggio = this.transloco.translate('giasgrid.VisteSalvate', {});
      this.giasMessageService.infoMessagge(messaggio);
    });

  }


  public saveCurrentViewObs(ddlViewItem): Observable<any> {

    const postData: SalvaVisteWrapper = this.prepareToSaveViews(this.inMemoryViews, ddlViewItem.id);
    let find = false;
    let view = postData.VisteModificate.find(v => v.IdVista === ddlViewItem.id);
    if (view) { // La vista corrente è tra quelle modificate
      postData.VisteNuove = [];
      postData.VisteModificate = [view];
      find = true;
    } else {
      view = postData.VisteNuove.find(v => v.IdVista === ddlViewItem.id);
      if (view) { // La vista corrente è tra quelle nuove
        postData.VisteModificate = [];
        postData.VisteNuove = [view];
        find = true;
      }
    }

    if (view && view.FlagPubblica && !this.permessoPubblica) {
      let messaggio = this.transloco.translate('giasgrid.EditVistaPubblicaWarning', {});
      this.giasMessageService.warningMessage(messaggio);
      return of({});
    }

    if (!postData.VisteModificate.length && !postData.VisteNuove.length) {
      return of({});
    }

    if (!find) {
      postData.VisteModificate = [];
      postData.VisteNuove = [];
      return of({});
    }

    let filtroJSON = view.FiltroJSON;

    return this.ajaxAgronicaAPIService.ajaxAPIPost('Shared/VisteGriglia_ScriviViste', postData)
      .pipe(tap((dati) => {
        let v = this._inMemoryViews.find(v => v.IdVista === view.IdVista);
        v.IsModified = false;
        v.IsNew = false;

        // update server views
        const index = this._serverViews.findIndex(s => s.IdVista === v.IdVista);
        if (index == -1) {
          this._serverViews.push(v);
        } else {
          this._serverViews.splice(index, 1, v);
        }

        let messaggio = this.transloco.translate('giasgrid.VisteSalvate', {});
        this.giasMessageService.infoMessagge(messaggio);

        //segnalo il cambiamento alla griglia
        this.gridPublicService.afterSaveView.next(filtroJSON);

        if (view.Predefinita) {
          const dropdown = this.dropdownService.value;
          dropdown.selected = dropdown.dropdown.defaultValue; // also restore predefinita
          dropdown.selected.data.IsNew = false;
          this.dropdownService.next(dropdown);
        }
      })
      );
  }

  public saveCurrentView(ddlViewItem) {
    this.saveCurrentViewObs(ddlViewItem).subscribe();
  }

  public deleteCustomization(selected: DropdownListItem) {
    const view = this.inMemoryViews.filter(s => s.IdVista === selected.id)[0];
    if (!selected?.data?.Predefinita) {
      this._inMemoryViews = this.inMemoryViews.filter(v => v.IdVista !== selected.id);
      this._serverViews = this._serverViews.filter(v => v.IdVista !== selected.id);
    }

    if (!view.IsNew) {
      this.ajaxAgronicaAPIService.ajaxAPIPost('Shared/VisteGriglia_CancellaVista', this.CreateNewKeyView(view.IdVista, view.NomeUtente, view.GridId), true)
        .subscribe((risp: RispostaStandard) => {
          if (risp.RispostaOK) {
            this.removeInMemoryItem(selected);

            let messaggio = this.transloco.translate('giasgrid.VistaCancellata', {});
            this.giasMessageService.infoMessagge(messaggio);
          }
        });
    } else {
      this.removeInMemoryItem(selected);
    }

    this.cleanupCookie(selected);

    if (view.Predefinita) {
      this._inMemoryViews.find(x => x.Predefinita).IsNew = true;
      this._serverViews.find(x => x.Predefinita).IsNew = true;
      
      const dropdown = this.dropdownService.value;
      dropdown.selected.data.IsNew = true;
      this.dropdownService.next(dropdown);
    }
  }

  private cleanupCookie(selected: DropdownListItem) {
    const id = this.dropdownService.getItemId(selected.data.GridId, selected.id, this.username);
    this.cookies.delete(id, '/');
  }


  private removeInMemoryItem(selectedItem: DropdownListItem) {
    const dropdown = this.dropdownService.value.dropdown;
    const position = dropdown.data.findIndex((d: DropdownListItem) => d.id === selectedItem.id);
    if (position > -1 && !selectedItem.data.Predefinita) {
      dropdown.data.splice(position, 1);
      this.dropdownService.next({ dropdown: dropdown, selected: null });
    }
  }


  /**
   * Creates or loads the view to be applied.
   * @param item the selected item of the dropdown.
   * @returns the view to be applied to the grid.
   */
  public getView(item: DropdownListItem, forceUpdateView: boolean) {
    const findInMemoryView = (item) => item?.data?.Predefinita ?
      this.inMemoryViews.find(mv => mv.Predefinita) :
      this.inMemoryViews.find(view => view.IdVista === item.id);

    let view = findInMemoryView(item);

    if (!view || forceUpdateView) {
      // calls 'this.mapGridSettingsAndSaveThem'
      this.customizationRequest.next({ dropdownItem: item, isNewView: view ? view.IsNew : true });
      view = this.getView(item, false);

      let cols = [];
      let cols1 = [];
      if (view && view.Colonne) {
        cols = view.Colonne.filter(x => x.field && x.field !== "");
        cols1 = view.Colonne.sort((a, b) => a.orderIndex - b.orderIndex);
        view.Colonne = cols1;
      }

      this.privateGridService.idealRead(false, false, cols1);
    }
    return view;
  }

  public getServerView(item: DropdownListItem | null): InMemoryView {
    const view = item?.data?.Predefinita ?
      this._serverViews.find(mv => mv.Predefinita) :
      this._serverViews.find(view => view.IdVista === item.id);

    return cloneDeep(view);
  }

  public mapGridSettingsAndSaveThem(
    thatGrid: any,
    item: DropdownListItem,
    isNew: boolean) {

    const view = this.saveGridSettings(thatGrid, item, isNew);
    this.saveInMemoryCustomization(view);
  }

  private saveGridSettings(that: any, chiave: DropdownListItem, isNewView: boolean) {
    const gridConfig: InMemoryView = {
      IdVista: chiave.id,
      NomeVista: chiave.name,
      GridId: chiave.data.GridId,
      Predefinita: chiave.data?.Predefinita ?? false,
      IsNew: isNewView,
      NomeUtente: (this.switchValue) ? chiave.data?.NomeUtente : this.username
    } as InMemoryView;

    this.fillCurrentView(gridConfig, that, that.grid.columns.toArray());

    return gridConfig;
  }

  private fillCurrentView(view: InMemoryView, that: any, kendoColumns: KendoGridColumn[]): void {
    const columns = kendoColumns
      .flatMap(x => 'children' in x ? (x as any).children.toArray() : x);

    view.Stato = that.pagination.gridState;
    view.IsModified = true;
    view.FiltroJSON = (this.gridPublicService.thereIsAForm) ? this.gridPublicService.filtroJSONstring : '';
    view.FlagPubblica = this.switchValue;
    view.Colonne = columns.map(columnComponent => {
      const savedColumn: KendoGridColumn = new KendoGridColumn({ field: '', title: '' });

      Object.keys(savedColumn).forEach((key: string) => {
        savedColumn[key] = columnComponent[key];

        if (!(columnComponent instanceof ColumnComponent))
          return;

        if (key === 'filter') {
          this.manageCellTypes(columnComponent as ColumnComponent, savedColumn, key, that.columns);
        }
      });
      return savedColumn;
    });
  }

  /**
   * Save the previously created view in memory, to be displayed to the user.
   * @param gridParentName the name of the component using the the grid.
   * @param customView the view to save.
   */
  public saveInMemoryCustomization(customView: InMemoryView) {
    this.updateInMemoryView(customView);
  }

  private updateInMemoryView(customView: InMemoryView) {

    let colMapped = customView.Colonne.map((c) => {
      let c1 = { ...c };

      for (var variableKey in c) {
        if (c.hasOwnProperty(variableKey)) {
          delete c[variableKey];
        }
      }

      c.width = c1.width;
      c.field = c1.field;
      c.title = c1.title;
      c.hidden = c1.hidden;
      c.orderIndex = c1.orderIndex;
      c.customParent = c1.customParent;
      c.customFooter = c1.customFooter;
      return c;
    })
    const cols = customView.Colonne.sort((a, b) => a.orderIndex - b.orderIndex);
    customView.Colonne = cols;

    const index = this.inMemoryViews.findIndex(s => s.IdVista === customView.IdVista);

    if (index == -1) {
      this._inMemoryViews.push(customView);
    } else {
      this._inMemoryViews.splice(index, 1, customView);
    }
  }


  /** Set the column type of the saved column
   * @param columns the columns passed to KendoServerModel.
   * @param savedColumn the column it is being processed.
   * @param columnComponent
   */
  public manageCellTypes(
    columnComponent: ColumnComponent, savedColumn: KendoGridColumn,
    key: string, columns: KendoGridColumn[]) {

    if (key === 'filter') {
      const attachedColumn = this.findCurrentlyAttachedGridColumn(columnComponent, columns);
      const columnType = this.privateGridService.getColumnType(attachedColumn.field);
      if (attachedColumn) {

        savedColumn.showHTMLAsString = attachedColumn.showHTMLAsString;

        switch (columnType) {
          case CELL_TYPES.BOOLEAN:
            savedColumn.boolean = attachedColumn.boolean;
            break;
          case CELL_TYPES.DATE:
            savedColumn.date = attachedColumn.date;
            break;
          case CELL_TYPES.DATETIME:
            savedColumn.date = attachedColumn.date;
            break;
          case CELL_TYPES.DROPDOWNLIST:
            savedColumn.ddl = attachedColumn.ddl;
            break;
          case CELL_TYPES.MULTI_DROPDOWNLIST:
            savedColumn.ddl = attachedColumn.ddl;
            break;
          case CELL_TYPES.NUMBER:
            savedColumn.numeric = attachedColumn.numeric;
            break;
          case CELL_TYPES.STRING:
            savedColumn.string = attachedColumn.string;
            break;
          case CELL_TYPES.CUSTOM:
          case CELL_TYPES.OBJECT:
            break;
          default:
            throw new Error("Column type not yet implemented");
        }
      }
    }
  }

  public persistView(selected: DropdownListItem) {
    if (this.defaultIsSelected(selected)) {
      this.cleanupCookies();
      return;
    }

    // se la vista è nuova, questa non sarà ricavata dal server al caricamento
    // della pagina. Non ha senso memorizzarla in cache.
    if (this.viewIsNew(selected)) {
      return;
    }

    this.cleanupCookies();

    if (this.defaultIsSelected(selected)) {
      return;
    }

    this.storeCurrentViewChoice(selected);
  }

  private storeCurrentViewChoice(selected: DropdownListItem) {
    const id = this.dropdownService.getItemId(selected.data.GridId, selected.id, this.username);
    const json = JSON.stringify(selected);
    this.cookies.set(id, json, { path: '/' });
  }
  private defaultIsSelected(selected: DropdownListItem) {
    return !selected || selected.data?.Predefinita;
  }
  private cleanupCookies() {
    let cookieKey = null;
    if (cookieKey = this.findExistingCookie()) {
      this.cookies.delete(cookieKey, '/');
    }
  }
  private viewIsNew(item: DropdownListItem) {
    const view = this.inMemoryViews.find(view => view.IdVista === item?.id);

    return view?.IsNew ?? true;
  }

  private transformData(viste: InMemoryView[], cacheItem: DropdownListItem) {
    const items = this.dropdownService.extractDropdownItems(viste);
    const id = this.dropdownService.generateDropdownId(this.CreateNewGridIdforView());
    const ddl = new DropdownList(id, items.filter(item => !item.data.Predefinita), items.find(item => item.data.Predefinita));

    //const shouldApplyView = viste.find(view => view.Predefinita) != null;
    let selectedItem = computeSelectedItem(items, cacheItem);
    return new DropdownChanged(ddl, selectedItem ?? cacheItem);
  }


  private prepareToSaveViews(inMemoryViews: InMemoryView[], idViewOptional: string = ''): SalvaVisteWrapper {

    let filtroJSON: string = this.gridPublicService.filtroJSONstring;
    let flagPubblica: boolean = this.switchValue;

    if (this.gridPublicService.thereIsAForm) {
      let index: number = inMemoryViews.findIndex(view => view.IdVista === idViewOptional);
      if (index > -1 && (inMemoryViews[index].FiltroJSON !== filtroJSON || inMemoryViews[index].FlagPubblica !== flagPubblica)) {
        inMemoryViews[index].IsModified = true;
        this._inMemoryViews[index].FiltroJSON = filtroJSON;
        this._inMemoryViews[index].FlagPubblica = flagPubblica;
      }
    }

    const newViews = inMemoryViews.filter(
      view => view.IsNew)
      .map((view: InMemoryView) => new ServerView({
        IdVista: view.IdVista,
        GridId: view.GridId,
        Predefinita: view.Predefinita,
        NomeUtente: view.NomeUtente,
        NomeVista: view.NomeVista,
        Colonne: JSON.stringify(view.Colonne),
        Stato: JSON.stringify(view.Stato),
        FiltroJSON: (!view.Predefinita) ? filtroJSON : '',
        FlagPubblica: flagPubblica
      }));

    const modifiedViews = inMemoryViews.filter(
      view => !view.IsNew && view.IsModified
    ).map((view: InMemoryView) => new ServerView({
      IdVista: view.IdVista,
      GridId: view.GridId,
      Predefinita: view.Predefinita,
      NomeUtente: view.NomeUtente,
      NomeVista: view.NomeVista,
      Colonne: JSON.stringify(view.Colonne),
      Stato: JSON.stringify(view.Stato),
      FiltroJSON: (!view.Predefinita) ? filtroJSON : '',
      FlagPubblica: flagPubblica
    }));

    return new SalvaVisteWrapper({
      VisteNuove: newViews,
      VisteModificate: modifiedViews
    });

  }

  findCurrentlyAttachedGridColumn(
    columnComponent: ColumnComponent, attchedColumns: KendoGridColumn[]): KendoGridColumn {
    const currentlyAttachedCol = attchedColumns.find(col => {
      if (columnComponent instanceof ColumnComponent) {
        return col.field === columnComponent.field;
      }
      return false;
    });

    if (!currentlyAttachedCol)
      throw Error(`Il campo ${columnComponent.field} non è stato trovato. Lo deve essere impostato un valore.`);

    return currentlyAttachedCol;
  }

  CreateNewKeyView(IdVista: string, NomeUtente: string, GridId: string = ""): ChiaveVista {

    if (NomeUtente === undefined || NomeUtente === null) {
      NomeUtente = "null";
    }

    if (GridId === "") {
      GridId = this.CreateNewGridIdforView();
    }

    return new ChiaveVista(GridId, IdVista, NomeUtente);
  }

  CreateNewGridIdforView(): string {

    let GridId: string = this.conf.gridId;

    if (this.router.url && this.router.url !== "") {
      GridId += "|" + this.router.url.split('/').pop().split('?')[0];
    }

    if (this.gridPublicService.thereIsAForm && this.gridPublicService.keyViewString.getValue())
      GridId += this.gridPublicService.keyViewString.getValue();

    return GridId;
  }

  setCurrentViewModified(): void {
    const currentViewId = this.dropdownService.value.selected.id;
    const currentView = this._inMemoryViews.find(v => v.IdVista === currentViewId);
    if (currentView != null) {
      currentView.IsModified = true;
      currentView['HasCurrentPending'] = true;
    }
  }
}
function computeSelectedItem(items: DropdownListItem[], cacheItem: DropdownListItem) {
  if (cacheItem.data?.Predefinita)
    return items.find(view => view.data?.Predefinita);
  return null;
}

