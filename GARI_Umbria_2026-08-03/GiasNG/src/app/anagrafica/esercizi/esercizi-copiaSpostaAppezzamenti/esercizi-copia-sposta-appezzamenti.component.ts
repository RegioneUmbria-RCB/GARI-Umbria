import {ChangeDetectorRef, Component, OnDestroy, OnInit} from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ImpreseFilterService } from 'app/Master/menu-contestuale/imprese-filter/imprese-filter.service';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { OperationsService } from 'app/menu-agenda/components/operations-list/operations.service';
import {map, Observable, of, Subject, switchMap, take, takeUntil, tap} from 'rxjs';
import { CopiaSpostaAppezzamenti, EserciziCopiaSpostaAppezzamentiService } from './esercizi-copia-sposta-appezzamenti.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { MasterService } from 'app/Service/master.service';

@Component({
  standalone: false,
  selector: 'app-esercizi-copia-sposta-appezzamenti',
  templateUrl: './esercizi-copia-sposta-appezzamenti.component.html',
  styleUrls: ['./esercizi-copia-sposta-appezzamenti.component.css']
})
export class EserciziCopiaSpostaAppezzamentiComponent implements OnInit, OnDestroy {

  mostraPopup: boolean;

  listaImprese: any[];
  listaCentri: any[];

  private signal = new Subject<void>();

  public virtual: any = {
    itemHeight: 28,
  };

  formCopiaSpostaAppezzamenti: FormGroup = new FormGroup({
    centri: new FormControl([]),
    imprese: new FormControl([]),
    copiasposta: new FormControl(true),
    copiaRifCatastali: new FormControl(true)
  });

  constructor(private operations: OperationsService,
              private impreseFilterService: ImpreseFilterService,
              private centriService: CentriAziendaliService,
              private objParametriAgendaService: ObjParametriAgendaService,
              private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
              private copiaSpostaService: EserciziCopiaSpostaAppezzamentiService,
              public changeDetector: ChangeDetectorRef,
              private giasDialogService: GiasDialogService,
              private translocoService: TranslocoService,
              private masterService: MasterService) { }

  ngOnDestroy(): void {
    this.signal.next()
    this.signal.complete();
  }

  ngOnInit(): void {
    this.copiaSpostaService.getMostraPopup().pipe(takeUntil(this.signal)).subscribe(r => {
      this.mostraPopup = r;
    });
    var objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    //Carico l'impresa preselezionata (E' la stessa dell'utente).
    this.formCopiaSpostaAppezzamenti.controls['imprese'].patchValue({codice: objParametriAgenda.Piva, descrizione: objParametriAgenda.RagSoc});

    //Creo un objPAgenda "artificiale" con la piva dell'impresa selezionata nella dropdown.
    let objPAgendaxCentri = this.objParametriAgendaService.getObjParamValue();
    objPAgendaxCentri.Piva = this.formCopiaSpostaAppezzamenti.controls['imprese'].value.codice;

    //Leggo il centro default, ovvero il primo collegato all'impresa default.
    this.centriService.leggiCentri(objPAgendaxCentri).pipe(map(R => {
      if(R != undefined) {
        this.listaCentri = R;
        let centroDefault = R.map(t => {return {codice: t.sa_cod, descrizione: t.sa_nome}})[0];
        this.formCopiaSpostaAppezzamenti.controls['centri'].patchValue(centroDefault);
      }
    })).subscribe()

    this.formCopiaSpostaAppezzamenti.controls['imprese'].valueChanges.pipe(
      switchMap(impresa => {
        return this.leggiCentri(impresa.codice);
      }),
      tap((listaCentri) => {
        if (listaCentri.length > 0){
          this.formCopiaSpostaAppezzamenti.controls['centri'].setValue(listaCentri[0]);
          this.formCopiaSpostaAppezzamenti.controls['centri'].patchValue(listaCentri[0]);
        } else {
          this.formCopiaSpostaAppezzamenti.controls['centri'].setValue({codice: 0, descrizione: ''});
          this.formCopiaSpostaAppezzamenti.controls['centri'].patchValue({codice: 0, descrizione: ''});
          listaCentri.push({codice: 0, descrizione: ''});
        }
        if (this.ddlCentro != undefined) {
          this.ddlCentro.listItems = listaCentri;
        }
        this.formCopiaSpostaAppezzamenti.controls['centri'].updateValueAndValidity();
        this.changeDetector.detectChanges();
      })
    ).subscribe();

  }
  private ddlCentro: GiasDropDownTemplateSComponent;
  async openDdl(ddlEl: GiasDropDownTemplateSComponent) {
    switch (ddlEl.giasFormControlName) {
      case 'imprese':
        if (this.listaImprese == undefined){
          ddlEl.loading = true;
          this.impreseFilterService.filtraImprese("").pipe(take(1), map(R => {
            ddlEl.loading = false;
            this.listaImprese = R.map(t => {
              return {codice:t.partitaIva, descrizione:t.ragioneSociale}
            })
            ddlEl.listItems = this.listaImprese;
          })).subscribe();
        } else {
          ddlEl.listItems = this.listaImprese
        }
        break;
      case 'centri':
        this.ddlCentro = ddlEl;
        this.leggiCentri(this.formCopiaSpostaAppezzamenti.controls['imprese'].value.codice).pipe(
          take(1),
          tap(listCentri => {
            ddlEl.listItems = listCentri;
          })).subscribe();
        break;
    }
  }

  leggiCentri(piva: string): Observable<{codice: number, descrizione: string}[]> {
    let objPAgendaxCentri = this.objParametriAgendaService.getObjParamValue();
    objPAgendaxCentri.Piva = piva;
    this.masterService.set_isLoading({message: '', isLoading: true});
    return this.centriService.leggiCentri(objPAgendaxCentri).pipe(switchMap(R => {
      this.listaCentri = R;
      this.masterService.set_isLoading({message: '', isLoading: false});
      return of(R.map(t => {return {codice: <number>t.sa_cod, descrizione: <string>t.sa_nome}}));
    }));
  }

  closeRicette(action: 'back' | 'create') {
    this.operations.drawerZIndex.next(1);
    console.log("closeRicette");
    if (action === 'create') {
      this.masterService.set_isLoading({message: '', isLoading: true});
      this.ajaxAgronicaAPIService.ajaxAPIPost<CopiaSpostaAppezzamenti, any>('AnagraficaNG/CopiaSpostaAppezzamenti', {
        Appezzamenti: this.copiaSpostaService.getAppezzamenti(),
        NuovoCentro: this.getNuovoCentro(),
        SpostaEliminaOrigine: this.formCopiaSpostaAppezzamenti.controls['copiasposta'].value,
        CopiaCatasto: this.formCopiaSpostaAppezzamenti.controls['copiaRifCatastali'].value
      }).subscribe({
        error: e => {
          this.copiaSpostaService.setMostraPopup(false);
          this.copiaSpostaService.setRicaricaImpianti(true);
          this.masterService.set_isLoading({message: '', isLoading: false});
        },
        next: r => {
          if (r.RispostaOK) {
            if (r.ErroriGias.length != 0) {
              this.giasDialogService.baseError('ErroreDuranteOperazione_', r.ErroriGias[0].messaggio.replace('</br>', '\n'));
            } else {
              if (this.formCopiaSpostaAppezzamenti.controls['copiasposta'].value) {
                this.giasDialogService.baseSuccess('ImpiantoSpostatoConSuccesso', '');
              } else {
                this.giasDialogService.baseSuccess('ImpiantoCopiatoConSuccesso', '');
              }
            }
          } else {
            this.giasDialogService.baseError('ErroreDuranteOperazione_', null);
          }
        },
        complete: () => {
          this.copiaSpostaService.setMostraPopup(false);
          this.copiaSpostaService.setRicaricaImpianti(true);
          this.masterService.set_isLoading({message: '', isLoading: false});
        }
      })
    } else {
      this.copiaSpostaService.setMostraPopup(false);
    }

  }

  private getNuovoCentro() {
    let centroSelezionato = this.listaCentri.find(c => c.sa_cod == this.formCopiaSpostaAppezzamenti.controls['centri'].value.codice);
    let centro: CentroAziendale = new CentroAziendale({codice: centroSelezionato.sa_cod, partitaIva: centroSelezionato.Piva});
    return centro;
  }
}
