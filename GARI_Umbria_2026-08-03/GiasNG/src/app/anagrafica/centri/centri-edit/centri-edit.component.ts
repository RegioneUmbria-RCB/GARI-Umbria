import { Location } from '@angular/common';
import {Component, DestroyRef, inject, OnDestroy, OnInit} from '@angular/core';
import { ControlContainer, FormBuilder, FormGroup, FormGroupDirective } from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import {
  AGRODATAFINE,
  AGRODATAINIZIO
} from 'app/Model/CostantiPersonalizzate';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { MasterService } from 'app/Service/master.service';
import {
  ObjParametriAgendaService
} from 'app/Service/obj-parametri-agenda.service';
import {Enum_DBTypeOperation, messaggioPostMessage} from 'gias-ui-kit';
import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import {
  Subject,
  take,
  takeUntil
} from 'rxjs';
import { EditCentroStore } from './services/centri-store.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {CodiceAnagrafe} from '../../../Model/anagrafiche/CodiceAnagrafe';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Component({
  standalone: false,
  selector: 'app-centri-edit',
  templateUrl: './centri-edit.component.html',
  styleUrls: ['./centri-edit.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class CentroEditComponent implements OnDestroy, OnInit {
  tipoOperazione: Enum_DBTypeOperation;
  impresaSelected: boolean;
  mainForm: FormGroup;
  id: string;

  isFormDisabled: boolean = false;
  dataInizio: Date = AGRODATAINIZIO;
  dataFine: Date = AGRODATAFINE;

  protected saveAndNew: boolean = false;
  protected inFrame: boolean = false;

  private signal:  Subject<void> = new Subject();
  private _destroyRef = inject(DestroyRef);

  constructor(
    private objAgenda: ObjParametriAgendaService,
    private giasMessageService: GiasMessageService,
    public store: EditCentroStore,
    private masterService: MasterService,
    private location: Location,
    private treeContainer: TreeContainerService,
    private router: Router,
    private translocoService: TranslocoService,
    private funzioniComuniService: FunzioniComuniService,
    private fb: FormBuilder,
    private route: ActivatedRoute
  ) {
    this.mainForm = this.store.getForm(fb);
    this.mainForm.get('indirizzi');

    this.objAgenda.currentObjParametriAgenda.pipe(takeUntil(this.signal))
      .subscribe(this.leggiCentro);
  }

  ngOnInit() {
    this.handleQueryParams();
  }

  ngOnDestroy(): void {
    this.store.resetForm();
    this.signal.next();
    this.signal.complete();
  }

  saveCenter(): void {
    this.mainForm.markAllAsTouched();
    if (
      this.mainForm.valid && !this.funzioniComuniService.codiciSovrapposti(
        this.store.codiciPubService.value.data.rows.map(r => {
          return {
            codiceAnagrafe: {
              codice: r['codice'],
              descrizione: r['descrizione']
            } as CodiceAnagrafe,
            validita: new IntervalloTemporale(r['dal'], r['al']), valore: r['valore']
          };
        })
      )
    ) {
      this.onSubmit().pipe(
        take(1)
      ).subscribe(s => {
        this.giasMessageService.successMessage(this.translocoService.translate('CentroAziendaleCorrettamenteModificato'));
        this.masterService.set_isLoading({ message: '', isLoading: false });
        this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
        if (this.inFrame) {
          console.log(s);
          window.parent.postMessage(messaggioPostMessage.chiudiWindowGiasNG, this.funzioniComuniService.getOrigins());
        } else {
          this.location.back();
        }
      });
    }
  }

  saveAndNewCenter(): void {
    this.mainForm.markAllAsTouched();
    if (
      this.mainForm.valid &&
      !this.funzioniComuniService.codiciSovrapposti(
        this.store.codiciPubService.value.data.rows.map(r => {
          return {
            codiceAnagrafe: {
              codice: r['codice'],
              descrizione: r['descrizione']
            } as any,
            validita: new IntervalloTemporale(r['dal'], r['al']), valore: r['valore']
          }
        })
      )
    ) {
      this.onSubmit().pipe(
        take(1)
      ).subscribe(() => {
        this.giasMessageService.successMessage(this.translocoService.translate('CentroAziendaleCorrettamenteModificato'));
        this.masterService.set_isLoading({ message: '', isLoading: false });
        this.treeContainer.selectedImpresaChangedSoUpdateTree = true;

        let objParametriAgenda: ObjParametriAgenda = this.objAgenda.getObjParamValue();
        objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        objParametriAgenda.Sa_Cod = 0;
        this.objAgenda.changeObjParametriAgenda(objParametriAgenda);

        let currentUrl: string = this.router.url;
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.router.onSameUrlNavigation = 'reload';
        this.objAgenda.changeObjParametriAgenda(objParametriAgenda);
        this.router.navigate([currentUrl]);
      });
    }
  }


  private leggiCentro = (agenda: ObjParametriAgenda) => {
    this.tipoOperazione = agenda.TipoOperazioneDB;
    this.impresaSelected = isAgendaSelected(agenda);
    this.abilitaDisabilitaForm(agenda);

    if (this.impresaSelected) {
      this.store.leggiDati();
    }
  };

  private abilitaDisabilitaForm(agenda: ObjParametriAgenda): void {
    if (agenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
      this.mainForm.disable();
    } else if (agenda.TipoOperazioneDB == Enum_DBTypeOperation.Update ||
      agenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      this.mainForm.enable();
    }
  }

  private onSubmit() {
    if(this.mainForm.valid) {
      let centroAziendale: CentroAziendale = this.mainForm.value;
      centroAziendale.codici = this.store.value.Centro.codici;
      centroAziendale.rubricaVoci = this.store.value.Centro.rubricaVoci;
      return this.store.saveCentroAziendale(this.mainForm.value);
    }
  }

  private handleQueryParams(): void {
    this.route.queryParams.pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe(params => {
        if (params.seFrame == 1) {
          this.saveAndNew = false;
          this.inFrame = true;
        }
      });
  }
}

function isAgendaSelected(agenda: ObjParametriAgenda): boolean {
  return agenda.Piva != null && agenda.Piva !== '';
}
