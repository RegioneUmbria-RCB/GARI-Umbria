import {Component, Inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {FormControl, FormGroup} from "@angular/forms";
import {forkJoin, map, Observable, of, Subject} from "rxjs";
import {take, takeUntil} from "rxjs/operators";
import {IUtenteImpresa, VisibilitaService} from "../services/visibilita.service";
import {ProfilazioneDataShareService} from "../services/profilazione-data-share.service";
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { ProfilazioneUtentiService } from '../services/profilazione-utenti.service';
import { ImpresaDTO } from '../services/impostazioni/impostazioni-aziende-centri.service';
import {ImpresaDto} from "../../Service/api.service";
import {BaseCodeDescrStr} from "../../Model/baseClass/baseCodeDescrStr";
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import {ImpreseService} from "../../Service/Anagrafica/imprese.service";
import {GiasKendoGridComponent} from 'gias-kendo-grid';
import {generateGridProviders} from 'gias-kendo-grid';
import {GridImpreseVisibilitaService} from "./grid-imprese-visibilita.service";
import {MasterService} from "../../Service/master.service";
import { GridPublicService } from 'gias-kendo-grid';
import { indexOf } from 'lodash';
import {GruppiUtentiService} from "../services/gruppi-utenti.service";
import {GruppoUtente} from "../models/gruppi-utenti/GruppoUtente.model";
import {TranslocoService} from "@jsverse/transloco";
import {Utente} from "../models/utente.model";

export function atLeastOneValidator(control: FormGroup) {
  if (control.controls['Impresa']?.value.length > 0 || control.controls['Utente']?.value.length > 0
    || control.controls['Gruppo']?.value.length > 0) {
    return {'atLeastOne': true};
  }
  return null;
}

@Component({
  standalone: false,
  selector: 'app-menu-visibilita',
  templateUrl: './menu-visibilita.component.html',
  styleUrls: ['./menu-visibilita.component.css'],
  providers: [
    ...generateGridProviders(GridImpreseVisibilitaService, MenuVisibilitaComponent),
    GiasMultiSelectTemplateService,
  ]
})
export class MenuVisibilitaComponent implements OnInit, OnDestroy {
  @ViewChild('grid') grid: GiasKendoGridComponent;

  public filters = new FormGroup({
    UtentiImprese: new FormControl<boolean>(false), // Impostato true se filtro per utenti, false altrimenti
    Utente: new FormControl([]),
    Gruppo: new FormControl([]),
    Impresa: new FormControl([])
  });
  public utentiDdl: BaseCodeDescrStr[] = [];
  public impreseDdl: ImpresaDto[] = [];
  public gruppiDdl: GruppoUtente[] = [];
  public virtual: any = {
    itemHeight: 28,
  };
  protected utentiConVisibilitaTotale: BaseCodeDescrStr[] = [];
  protected forceShowGrid = false;
  protected customMsg = "";

  private signal = new Subject();

  constructor(
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseService,
    private kendoGridService: GridPublicService,
    private profilazioneUtentiService: ProfilazioneUtentiService,
    private gruppiService: GruppiUtentiService,
    private visibilitaService: VisibilitaService,
    private datashare: ProfilazioneDataShareService,
    private master: MasterService,
    protected transloco: TranslocoService // usato in HTML
  ) {
    this.filters.setValidators(atLeastOneValidator);
  }

  public get loaded(): boolean {
    return (this.filters.value.UtentiImprese && !!this.datashare.visibilitaUtenti.value)
      || (!this.filters.value.UtentiImprese && !!this.datashare.visibilitaImprese.value);
  }

  ngOnInit(): void {
    this.handleFilters();
    this.handleValueChange();
  }

  ngOnDestroy(): void {
    this.signal.next(true);
    this.signal.complete();
  }

  getStringaUtenti(): string {
    let stringa = '';
    if(this.utentiConVisibilitaTotale.length === 1) {
      stringa =  ' ' + this.utentiConVisibilitaTotale[0].descrizione + ' ';
    } else {
      this.utentiConVisibilitaTotale.forEach(utente => {
        if(indexOf(this.utentiConVisibilitaTotale, utente) + 1 < this.utentiConVisibilitaTotale.length) {
          stringa = stringa.concat(' ' + utente.descrizione + ', ');
        } else {
          stringa = stringa.concat(' ' + utente.descrizione + ' ');
        }
      });
    }
    return stringa;
  }

  /**
   * Chiamata quando il bottone "Cerca" viene premuto. Carica la tabella della visibilità.
   */
  public cercaVisibilita() {
    const afterLoaded = (items: IUtenteImpresa[]) => {
      this.datashare.visibilitaImprese.next(items);
      this.grid.publicService.refresh(true);
      this.master.set_isLoading({isLoading: false, component: this.grid.publicService.gridElRef});
    };
    this.forceShowGrid = false;
    this.master.set_isLoading({isLoading: true, component: this.grid.publicService.gridElRef});

    if (this.filters.value.Utente?.length)
      this.loadGridUtenti().subscribe(R => afterLoaded(R));
    else if (this.filters.value.Gruppo?.length)
      this.loadGridFromGruppi().subscribe(R => afterLoaded(R));
    else
      this.loadGridImprese().subscribe(R => afterLoaded(R));
  }

  /**
   * Gestisce il caricamento delle dropDownList.
   * @private
   */
  async openDdl(ddlEl: GiasDropDownTemplateSComponent) {
    const setItems = (items: any[]) => {
      ddlEl.listItems = items;
      ddlEl.loading = false;
    };
    if (!ddlEl.loading) {
      ddlEl.loading = true;
      switch (ddlEl.giasFormControlName) {
        case 'Impresa':
          this.loadDdlImprese().pipe(take(1)).subscribe(data => setItems(data));
          break;
        case 'Utente':
          this.loadDdlUtenti().pipe(take(1)).subscribe(data => setItems(data));
          break;
        case 'Gruppo':
          this.loadDdlGruppi().pipe(take(1)).subscribe(results => setItems(results));
          break;
      }
    }
  }

  private handleFilters() {
    this.filters.get('Impresa').valueChanges.pipe(takeUntil(this.signal)).subscribe(value => {
      console.log(value);
      this.datashare.visibilitaRichiesta = false;
    });
    this.filters.get('Utente').valueChanges.pipe(takeUntil(this.signal)).subscribe((usernames: string[]) => {
        if (usernames.length > 0 && this.filters.get('Gruppo').enabled)
          this.filters.get('Gruppo').disable();
        else if (usernames.length === 0 && this.filters.get('Gruppo').disabled)
          this.filters.get('Gruppo').enable();
        this.checkVisibilitaTotale(usernames);
        this.datashare.visibilitaRichiesta = false;
    });
    this.filters.get('Gruppo').valueChanges.pipe(takeUntil(this.signal)).subscribe(value => {
        this.forceShowGrid = false;
        if (value.length > 0 && this.filters.get('Utente').enabled)
          this.filters.get('Utente').disable();
        else if (value.length === 0 && this.filters.get('Utente').disabled)
          this.filters.get('Utente').enable();
      });
  }

  private checkVisibilitaTotale(usernames: string[]) {
    let utentiAppoggio: BaseCodeDescrStr[] = [];
    this.visibilitaService.hannoVisibilitaTotale(usernames).subscribe(r => r.forEach(user => {
      if(user.VisibilitaTotale) {
        utentiAppoggio.push(this.utentiDdl.find(item => item.codice === user.Username));
      }
    }));
    this.utentiConVisibilitaTotale = utentiAppoggio;
  }

  private handleValueChange() {
    try {
      this.kendoGridService.changeDetected.pipe(takeUntil(this.signal)).subscribe((event: any) => {
          if(event?.action === 'remove'){
              // this.eliminaAziendaAngular(event);
          }
      });
    } catch (e) {
        console.log('Error:', e);
    }
  }

  private loadDdlImprese(): Observable<ImpresaDto[]> {
    if (this.impreseDdl.length)
      return of(this.impreseDdl);
    else return this.impreseService.caricaCmbImprese().pipe(
      take(1),
      map((r: ImpresaDto[]) => {
        this.impreseDdl = r;
        return this.impreseDdl;
      })
    );
  }

  private loadDdlUtenti(): Observable<BaseCodeDescrStr[]> {
    if (this.utentiDdl.length)
      return of(this.utentiDdl);
    else return this.profilazioneUtentiService.readUtenti(true).pipe(
      take(1),
      map(r => {
        this.utentiDdl = r.map((el: Utente) => {
          let u = new BaseCodeDescrStr(el.UserName, el.Dettagli);
          u['codiciGruppi'] = el.Gruppi.map(g => g.codice);
          return u;
        });
        return this.utentiDdl;
      })
    );
  }

  private loadDdlGruppi(): Observable<GruppoUtente[]> {
    if (this.gruppiDdl.length)
      return of(this.gruppiDdl);
    else return this.gruppiService.readGruppi().pipe(
      take(1),
      map((r: GruppoUtente[]) => {
        this.gruppiDdl = r;
        return this.gruppiDdl;
      })
    );
  }

  /**
   * Carica i dati per le griglie visibilità utenti e imprese.
   * Le chiamate vengono effettuate qui per cercare di ridurre, anche se in minima parte,
   * i tempi di attesa delle risposte.
   * @private
   */
  // private loadData() {
  //   if (!this.datashare.visibilitaRichiesta) {
  //     this.datashare.visibilitaRichiesta = true;
  //     this.visibilitaService.saved$.pipe(takeUntil(this.datashare.exitProfilazione))
  //       .subscribe(saved => this.datashare.visibilitaRichiesta = !saved);
  //     this.visibilitaService.leggiVisibilitaImprese(
  //       this.filters.controls['Impresa'].value.map(impresa => ({piva: impresa.codice}) as ImpresaDTO)
  //     ).subscribe(R => this.datashare.visibilitaImprese.next(R));
  //     this.visibilitaService.leggiVisibilitaUtenti(this.filters.controls['Utente'].value.map(utente => utente.codice))
  //       .subscribe(R => this.datashare.visibilitaUtenti.next(R));
  //   }
  // }

  private loadGridImprese() {
    let imprese = this.filters.value.Impresa;
    if (typeof (imprese) === 'string')
      imprese = [imprese];
    return this.visibilitaService.leggiVisibilitaImprese(imprese.map(piva => new ImpresaDTO(piva)));
  }

  private loadGridUtenti() {
    let utenti = this.filters.value.Utente;
    if (!utenti && this.filters.value.Gruppo)
      utenti = this.getUtentiFromGruppi();
    let imprese = this.filters.value.Impresa || [];
    if (typeof (utenti) === 'string')
      utenti = [utenti];
    if (typeof (imprese) === 'string')
      imprese = [new ImpresaDTO(imprese)];
    else if (!!imprese)
      imprese = imprese.map(piva => new ImpresaDTO(piva));
    return this.visibilitaService.leggiVisibilitaUtenti(utenti, imprese, true);
  }

  private loadGridFromGruppi(): Observable<IUtenteImpresa[]> {
    let groups = this.filters.value.Gruppo;
    let imprese = this.filters.value.Impresa || [];
    if (typeof (imprese) === 'string')
      imprese = [new ImpresaDTO(imprese)];
    else if (!!imprese)
      imprese = imprese.map(piva => new ImpresaDTO(piva));
    return this.visibilitaService.leggiVisibilitaGruppi(groups, imprese, true)
      .pipe(map(R => {
        if (R['fullResult'] === true) {
          this.forceShowGrid = true;
          this.utentiConVisibilitaTotale = R['visibilitaTotale'].map(u => new BaseCodeDescrStr(u.UserName,u.UserName)) || [];
          return R['visibilita'];
        } else {
          this.forceShowGrid = false;
          this.customMsg = this.transloco.translate('prof.ImpossibileCaricarePermessiTroppiUtenti') + " "
          + R['visibilitaTotale'].length + " " + this.transloco.translate('prof.UtentiConVisTotale')+ ". "
          + R['visibilita'].length + " " + this.transloco.translate('prof.UtentiConVisParziale') + ". ";
          return [];
        }
      }));
  }

  private getUtentiFromGruppi() {
    const containsAny = (mainList: any[], toHave: any[]) => {
      for (let item of toHave) {
        if (mainList.includes(item))
          return true;
      }
      return false;
    };
    return this.utentiDdl.filter(u => containsAny(u['codiciGruppi'], this.filters.value.Gruppo))
      .map(u => u.codice);
  }

}
