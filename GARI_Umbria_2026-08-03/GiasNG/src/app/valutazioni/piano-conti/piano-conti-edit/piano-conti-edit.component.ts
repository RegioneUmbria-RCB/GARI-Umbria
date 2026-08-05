import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ControlContainer, FormGroupDirective } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ValutazioniService } from 'app/valutazioni/valutazioni/service/valutazioni.service';
import { Subject, switchMap, map, takeUntil, of, take } from 'rxjs';
import { Location } from '@angular/common';
import { PianoContiService } from '../service/pianoconti.service';
import { PianoContixTree, TreeValutazione } from 'app/Model/valutazioni/PianoContixTree';
import { CheckedState, TreeItemDropEvent, TreeItemLookup } from '@progress/kendo-angular-treeview';
import { DialogResult } from '@progress/kendo-angular-dialog';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ObjParametriAgenda } from 'gias-ui-kit';


@Component({
  standalone: false,
  selector: 'app-piano-conti-edit',
  templateUrl: './piano-conti-edit.component.html',
  styleUrls: ['./piano-conti-edit.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }],

})
export class PianoContiEditComponent {

  tipoOperazione: Enum_DBTypeOperation;
  pianoContiSelected: boolean = true;
  mainForm: FormGroup;
  id: string;
  isFormDisabled = false;
  pianoContoAttuale: any;
  ragioneSociale_List: GiasRagioneSocialeDDLItem[];
  submitInProgress: boolean = false;
  private signal: Subject<void> = new Subject();
  currentDate: Date = new Date();
  partitaIvaGenerica: string = "AAAAAAAAAAA";
  tutteValutazioniAttivate: boolean = false;
  key: string = "id";

  public checkedKeys: any[] = [];
  public initialKeysSelect: string[] = [];

  public creationMode: boolean = false;
  public dataTreeView: TreeValutazione[] = [];
  public updateMode: boolean = false;

  public allParentNodes = [];
  public expandedKeys: any[] = this.allParentNodes.slice();

  constructor(private masterService: MasterService,
    private objAgenda: ObjParametriAgendaService,
    private giasMessageService: GiasMessageService,
    private treeContainer: TreeContainerService,
    private valutazioniService: ValutazioniService,
    private router: Router,
    private translocoService: TranslocoService,
    private location: Location,
    fb: FormBuilder,
    private pianoConti: PianoContiService,
    private giasDialogService: GiasDialogService
  ) {

    this.mainForm = this.getForm(fb);

    this.objAgenda.currentObjParametriAgenda.pipe(takeUntil(this.signal))
      .subscribe(this.leggiPianoConti);
  }

  leggiPianoConti = (agenda: ObjParametriAgenda) => {
    this.tipoOperazione = agenda.TipoOperazioneDB;
    this.abilitaDisabilitaForm(agenda);
    this.leggiDati();
  };

  abilitaDisabilitaForm(agenda: ObjParametriAgenda) {
    const operazione = agenda.TipoOperazioneDB;
    if (operazione == Enum_DBTypeOperation.Read) {
      this.mainForm.disable();
      this.isFormDisabled = true;
    } else if (operazione == Enum_DBTypeOperation.Update ||
      operazione == Enum_DBTypeOperation.Write) {
      this.mainForm.enable();
      this.isFormDisabled = false;

      if (operazione == Enum_DBTypeOperation.Update) {
        this.mainForm.controls['ragione_sociale'].disable();
        this.mainForm.controls['validoTutteImprese'].disable();
        this.updateMode = true;
      }
    }

    if (operazione == Enum_DBTypeOperation.Write) {
      this.creationMode = true;
    }
  }

  ngOnDestroy(): void {
    this.resetForm();
    this.signal.next();
    this.signal.complete();
  }

  leggiDati() {

    let piano_conti_lettura = this.pianoConti.getPianoContoValue();

    this.pianoConti.leggiPianoContiContiExTree(piano_conti_lettura.piva, piano_conti_lettura.pianoCod).pipe(

      switchMap((r) => {
        let piano_conto = r[0] as any;

        this.pianoContoAttuale = piano_conto;

        console.log("piano conto attuale");
        console.log(this.pianoContoAttuale);

        if (piano_conti_lettura.pianoCod > 0) {
          this.mainForm.controls['descrizionePianoConti'].setValue(piano_conto.descrizione);
        }

        this.dataTreeView = this.pianoContoAttuale.TreeValutazione;

        this.valutazioniService.leggiRagioneSociale().pipe(
          map(res => {
            let dropdownRagioneSociale: GiasRagioneSocialeDDLItem[] = new Array();
            res.forEach((x) => {
              let responseRagSociale: GiasRagioneSocialeDDLItem = {
                ragioneSociale: x.ragioneSociale,
                partitaIva: x.partitaIva
              };
              dropdownRagioneSociale.push(responseRagSociale);
            });
            return dropdownRagioneSociale;
          })).subscribe(x => {
            this.ragioneSociale_List = x;

            if (this.pianoContoAttuale.pianoCod != 0) {
              this.mainForm.controls['ragione_sociale'].setValue(this.ragioneSociale_List.find(y => y.partitaIva === this.pianoContoAttuale.piva));

              if (this.pianoContoAttuale.piva === this.partitaIvaGenerica) {
                this.mainForm.controls['ragione_sociale'].setValue(this.translocoService.translate('TutteImprese'));
                this.mainForm.controls['validoTutteImprese'].setValue(true);
              }
            }
          });


        this.expandAll();

        if (!this.creationMode) {
          this.setAllElementRead();
          /* this.dataTreeView.forEach(child => {
             this.setCheckBoxFather(child);
           })*/
        } else {
          this.putAllCheckBoxActive();
        }
        this.checkedKeys = Object.create(this.initialKeysSelect);

        //cicle orders of elements
        this.dataTreeView.forEach(child => {
          this.recycleOrder(child);
        });
        console.log(this.dataTreeView);

        return of(piano_conto);
      })
    ).subscribe();
  }

  /*cycle orders */
  recycleOrder(dataItem: any): void {

    if (dataItem.tipo !== "SEZIONE") {
      if (dataItem.children && dataItem.children.length > 0) {
        dataItem.children.forEach(child => {
          this.recycleOrder(child);
        });
      }
    } else {
      if (dataItem.children && dataItem.children.length > 0) {
        let index = 0;
        dataItem.children.forEach(child => {
          console.log(index);
          child.ordine = index;
          index++;
        });
      }
    }
  }




  /*FOr expansion of all node */
  expandAll() {
    this.getAllParentTextProperties(this.dataTreeView);
    this.expandedKeys = this.allParentNodes.slice();
  }

  public getAllParentTextProperties(items: Array<any>) {
    items.forEach(i => {
      if (i.children) {
        this.allParentNodes.push(i.id);
        this.getAllParentTextProperties(i.children);
      }
    });
  }


  /*for creation mode active all checkbox */
  putAllCheckBoxActive() {
    this.dataTreeView.forEach(x => {
      this.checkedKeys.push(x.id);
      this.initialKeysSelect.push(x.id);
      x.flag_selezionato = true;


      if (x.children && x.children.length > 0) {
        x.children.forEach((child: any) => {
          this.putAllCheckBoxActiveRecursive(child);
        });
      }
    });
  }

  putAllCheckBoxActiveRecursive(child: any) {
    this.checkedKeys.push(child.id);
    this.initialKeysSelect.push(child.id);
    child.flag_selezionato = true;


    if (child.children && child.children.length > 0) {
      child.children.forEach((childRecursive: any) => {
        this.putAllCheckBoxActiveRecursive(childRecursive);
      });
    }

  }

  /*set checkbox father */
  setCheckBoxFather(parent: any): boolean {

    console.log("checkboxfather");
    let isCheckedFather: boolean = true;

    if (!parent.flag_selezionato && parent.children && parent.children.length > 0) {

      parent.children.forEach(child => {
        if (this.setCheckBoxFather(child)) {
          this.checkedKeys.push(parent.id);
          this.initialKeysSelect.push(parent.id);
          parent.flag_selezionato = true;
          isCheckedFather = true;
          console.log("checkfather");

          return isCheckedFather;
        }
      });
    }


    if (!parent.flag_selezionato) {
      isCheckedFather = false;
      console.log("uncheckfather");
      return isCheckedFather;
    } else {
      return isCheckedFather;
    }

  }



  /*For Read mode */

  setAllElementRead() {
    this.dataTreeView.forEach(x => {
      if (x.flag_selezionato) {
        this.checkedKeys.push(x.id);
        this.initialKeysSelect.push(x.id);
      }

      if (x.children && x.children.length > 0) {
        x.children.forEach((child: any) => {
          this.setAllElementReadRecursive(child);
        });
      }
    });

  }

  setAllElementReadRecursive(x: any) {
    if (x.flag_selezionato) {
      this.checkedKeys.push(x.id);
      this.initialKeysSelect.push(x.id);
    }

    if (x.children && x.children.length > 0) {
      x.children.forEach((child: any) => {
        this.setAllElementReadRecursive(child);
      });
    }
  }


  /*for flag selezionato in node quando ci si clicca */
  setAllElementFlagSelezionato(parent: any) {

    if (this.checkedKeys.includes(parent.id)) {
      parent.flag_selezionato = true;

      const index = this.initialKeysSelect.indexOf(parent.id, 0);
      if (index == -1) {
        this.initialKeysSelect.push(parent.id);
      }
    } else {
      parent.flag_selezionato = false;

      const index = this.initialKeysSelect.indexOf(parent.id, 0);
      if (index > -1) {
        this.initialKeysSelect.splice(index, 1);
      }
    }

    if (parent.children && parent.children.length > 0) {
      parent.children.forEach((child: any) => {
        this.toggleChildrenCheckState(parent, child);
      });
    }


    console.log("treeView modificato");
    console.log(this.dataTreeView);
  }

  toggleChildrenCheckState(parent: any, childParent: any) {
    if (parent.flag_selezionato) {
      childParent.flag_selezionato = true;

      const index = this.initialKeysSelect.indexOf(childParent.id, 0);

      if (index == -1) {
        this.initialKeysSelect.push(childParent.id);
      }
    } else {
      childParent.flag_selezionato = false;

      const index = this.initialKeysSelect.indexOf(childParent.id, 0);
      if (index > -1) {
        this.initialKeysSelect.splice(index, 1);
      }
    }

    if (childParent.children && childParent.children.length > 0) {
      childParent.children.forEach((child: any) => {
        this.toggleChildrenCheckState(childParent, child);
      });
    };
  }

  public recicleOrders(event: any): void {
    //cicle orders of elements
    this.dataTreeView.forEach(child => {
      this.recycleOrder(child);
    });
    console.log(this.dataTreeView);

    let numValAssociato = this.pianoConti.getNumValAssociato();

    if (this.updateMode && numValAssociato > 0) {
      this.SalvaPianoContiInUpdate();
    }
  }


  onNodeDrop(event: TreeItemDropEvent) {
    console.log("onNodeDrop");
    const node = event.destinationItem;

    // Verifica se il nodo trascinato ha figli, se sì, impedisci il trascinamento
    if (event.dropPosition == 0) {
      event.setValid(false);
    }

    if (node.children && node.children.length > 0) {
      event.setValid(false);
    }

    if (node.parent.item.dataItem.id != event.sourceItem.parent.item.dataItem.id) {
      event.setValid(false);
    }

    if (this.isFormDisabled) {
      event.setValid(false);
    }

  }


  /*funziona in READ mode */
  onCheckChange(event: TreeItemLookup): void {
    console.log(event);

    if (this.isFormDisabled) {
      this.checkedKeys = Object.create(this.initialKeysSelect);
      console.log(this.checkedKeys);
    } else {

      console.log("numvalAssociato");
      let numValAssociato = this.pianoConti.getNumValAssociato();

      if (this.updateMode && numValAssociato > 0) {

        let isChecked = this.isChecked(event.item.dataItem, "");

        let messageWarning = 'WarningNoCheck';

        if (isChecked == "checked") {
          messageWarning = 'WarningCheck';
        }

        const promise = new Promise((resolve, reject) => {
          this.giasDialogService.baseWarning(
            '', this.translocoService.translate(messageWarning), false
          ).then((resp: DialogResult) => {
            if (!resp['returnObj']) {
              this.checkedKeys = Object.create(this.initialKeysSelect);
              return;
            }

            this.setAllElementFlagSelezionato(event.item.dataItem);
            this.SalvaPianoContiInUpdate();

            resolve(true);

          });
        });


      } else {
        this.setAllElementFlagSelezionato(event.item.dataItem);
      }
    }


  }

  changeTutteImpese(event: boolean) {
    this.tutteValutazioniAttivate = event as boolean;

    this.mainForm.controls['ragione_sociale'].enable();
    if (this.tutteValutazioniAttivate) {
      this.mainForm.controls['ragione_sociale'].disable();
      this.mainForm.controls['ragione_sociale'].setValue(this.translocoService.translate('TutteImprese'));
    }
  }

  getForm(fb: FormBuilder): FormGroup {
    if (!this.mainForm)
      this.mainForm = this.createFormGroup(fb);
    return this.mainForm;
  }

  createFormGroup(fb: FormBuilder) {
    return fb.group({
      ragione_sociale: ['', Validators.required],
      descrizionePianoConti: ['', Validators.required],
      validoTutteImprese: [false],
      flag_cancellazione: [''],
      primaryKey: fb.group({
        codice: [''],
        partitaIva: ['']
      }),

      codiceOperatore: fb.control(''),
    });
  }

  resetForm() {
    this.mainForm = null;
  }

  SalvaPianoConti() {
    this.submitInProgress = true;
    this.mainForm.markAllAsTouched();
    this.mainForm.setValue(this.mainForm.getRawValue());
    if (this.mainForm.valid) {
      this.onSubmit().pipe(take(1)).subscribe(s => {
        this.giasMessageService.successMessage(this.translocoService.translate('SalvataggioAvvenutoConSuccesso'));
        this.masterService.set_isLoading({ message: '', isLoading: false });
        this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
        this.location.back();
      });
    } else {
      this.submitInProgress = false;
    }
  }

  SalvaPianoContiInUpdate() {
    this.submitInProgress = true;
    this.mainForm.markAllAsTouched();
    this.mainForm.setValue(this.mainForm.getRawValue());
    if (this.mainForm.valid) {
      this.onSubmit().pipe(take(1)).subscribe(s => {
        this.giasMessageService.successMessage(this.translocoService.translate('SalvataggioAvvenutoConSuccesso'));
        this.masterService.set_isLoading({ message: '', isLoading: false });
        this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
      });
    } else {
      this.submitInProgress = false;
    }
  }

  private onSubmit() {
    if (this.mainForm.valid) {
      let piano_conti_lettura = this.pianoConti.getPianoContoValue();

      let currentObject: PianoContixTree = {
        piva: (this.tutteValutazioniAttivate) ? this.partitaIvaGenerica : (this.mainForm.controls['ragione_sociale'].value.partitaIva ?? this.partitaIvaGenerica),
        descrizione: this.mainForm.controls['descrizionePianoConti'].value,
        codice: piano_conti_lettura.pianoCod,
        id: this.pianoContoAttuale.id,
        flag_collegato: false,
        tipo: this.pianoContoAttuale.tipo,
        TreeValutazione: this.dataTreeView
      };

      console.log("return all object in submit");
      console.log(currentObject);

      this.masterService.set_isLoading({ message: '', isLoading: true });
      return this.pianoConti.scriviPianoContixTree(currentObject);
    }
  }



  // Custom logic handling Indeterminate state when custom data item property is persisted
  public isChecked = (dataItem: any, index: string): CheckedState => {
    if (this.containsItem(dataItem)) {
      return "checked";
    }

    if (this.isIndeterminate(dataItem.children)) {
      return "indeterminate";
    }

    return "none";
  };

  private containsItem(item: any): boolean {
    return this.checkedKeys.indexOf(item[this.key]) > -1;
  }

  private isIndeterminate(items: any[] = []): boolean {
    let idx = 0;
    let item;

    if (items != null) {
      while ((item = items[idx])) {
        if (this.isIndeterminate(item.children) || this.containsItem(item)) {
          return true;
        }

        idx += 1;
      }
    }

    return false;
  }
}



export class GiasRagioneSocialeDDLItem {
  constructor(
    public readonly ragioneSociale: string | number,
    public readonly partitaIva: string) {
  }
}
