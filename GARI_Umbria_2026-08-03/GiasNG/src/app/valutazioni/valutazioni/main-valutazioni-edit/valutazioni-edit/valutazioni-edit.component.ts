import { Component, OnDestroy } from '@angular/core';
import { ControlContainer, FormBuilder, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Subject, map, of, switchMap, take, takeUntil } from 'rxjs';
import { ValutazioniService } from '../../service/valutazioni.service';
import { PKValutazioneTestata, ValutazioneTestata } from 'app/Model/valutazioni/ValutazioneTestata';
import { Location } from '@angular/common';
import { ValutazioneTestataxAnno } from 'app/Model/valutazioni/ValutazioneTestataxAnno';
import { faPlusSquare } from '@fortawesome/free-solid-svg-icons';
import { ObjParametriAgenda } from 'gias-ui-kit';


@Component({
  standalone: false,
  selector: 'app-valutazioni-edit',
  templateUrl: './valutazioni-edit.component.html',
  styleUrls: ['./valutazioni-edit.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }],
})


export class ValutazioniEditComponent implements OnDestroy {
  tipoOperazione: Enum_DBTypeOperation;
  valutazioneSelected: boolean = true;
  mainForm: FormGroup;
  id: string;
  isFormDisabled = false;
  valutazioneTestataAttuale: any;
  pianoConti_List: GiasValutazioniDDLItem[];
  ragioneSociale_List: GiasRagioneSocialeDDLItem[];

  submitInProgress: boolean = false;

  private signal: Subject<void> = new Subject();
  public faPlusSquare = faPlusSquare;

  currentDate: Date = new Date();

  partitaIvaObservable: Subject<string> = new Subject<string>();

  letturaPianoConti$ = this.partitaIvaObservable.pipe(
    switchMap(partitaiva => {
      return this.valutazioniService.leggiPianoConti(partitaiva, 0).pipe(
        map(res => {
          let dropdown: GiasValutazioniDDLItem[] = new Array();
          res.forEach((x) => {
            let response: GiasValutazioniDDLItem = {
              codice: x.primaryKey.Valutazione_Piano_Cod,
              descrizione: x.Valutazione_Piano_Des,
              dati: ""
            };
            dropdown.push(response);
          });
          return dropdown;
        })
      );
    })
  );

  constructor(private masterService: MasterService,
    private objAgenda: ObjParametriAgendaService,
    private giasMessageService: GiasMessageService,
    private treeContainer: TreeContainerService,
    private valutazioniService: ValutazioniService,
    private router: Router,
    private translocoService: TranslocoService,
    private location: Location,
    fb: FormBuilder

  ) {

    this.mainForm = this.getForm(fb);

    this.objAgenda.currentObjParametriAgenda.pipe(takeUntil(this.signal))
      .subscribe(this.leggiValutazione);


  }


  leggiValutazione = (agenda: ObjParametriAgenda) => {
    this.tipoOperazione = agenda.TipoOperazioneDB;
    console.log("Leggi valutazione");
    //this.valutazioneSelected = this.isValutazioneSelected(agenda);
    this.abilitaDisabilitaForm(agenda);

    //if (this.valutazioneSelected) {
    this.leggiDati();
    //}
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
        this.mainForm.controls['piano_conti'].disable();
        this.mainForm.controls['ragione_sociale'].disable();
      }

    }
  }

  /*isValutazioneSelected(agenda: ObjParametriAgenda) {
    return agenda.Piva != null && agenda.Piva !== '';
  }*/

  ngOnDestroy(): void {
    this.resetForm();
    this.signal.next();
    this.signal.complete();
  }

  leggiDati() {

    let testata_lettura = this.valutazioniService.getLeggiTestataValue();

    this.valutazioniService.leggiValutazioneObservable(testata_lettura).pipe(

      switchMap((r) => {
        let val_Testata = r[0] as any;

        if (val_Testata == undefined) {
          val_Testata = new Object();
          val_Testata.Piva = testata_lettura.piva;
          val_Testata.Rag_Soc = this.objAgenda.getObjParamValue().RagSoc;
        }

        this.valutazioneTestataAttuale = val_Testata;

        if (testata_lettura.idTestata != 0) {
          this.mainForm.controls['note'].setValue(val_Testata.Note);
          this.mainForm.controls['dataRedazione'].setValue(val_Testata.Data_Redazione);
          this.mainForm.controls['ragione_sociale'].setValue(val_Testata.Rag_Soc);
        }



        this.valutazioniService.leggiRagioneSociale().pipe(
          map(res => {
            let dropdownRagioneSociale: GiasRagioneSocialeDDLItem[] = new Array();
            res.forEach((x) => {
              console.log(x);
              let responseRagSociale: GiasRagioneSocialeDDLItem = {
                ragioneSociale: x.ragioneSociale,
                partitaIva: x.partitaIva
              };
              dropdownRagioneSociale.push(responseRagSociale);
            });
            return dropdownRagioneSociale;
          })).subscribe(x => {
            this.ragioneSociale_List = x;

          });

        this.mainForm.controls['ragione_sociale'].valueChanges.subscribe(p => {
          if (!this.submitInProgress) {
            if (this.tipoOperazione == Enum_DBTypeOperation.Write) {
              console.log("change val");
              this.mainForm.controls['piano_conti'].setValue(null);
            }

            this.partitaIvaObservable.next(p.partitaIva);
          }
        });

        this.letturaPianoConti$.subscribe(x => {
          this.pianoConti_List = x;

          console.log("change");

          if (this.pianoConti_List.length === 1) {
            this.mainForm.controls['piano_conti'].setValue(this.pianoConti_List[0]);
          }
          else if (testata_lettura.idTestata != 0) {
            this.mainForm.controls['piano_conti'].setValue(this.pianoConti_List.find(y => y.codice == this.valutazioneTestataAttuale.Valutazione_Piano_Cod));
          }
        });

        if (this.tipoOperazione != Enum_DBTypeOperation.Write) {
          this.partitaIvaObservable.next(this.valutazioneTestataAttuale.Piva);
        }


        return of(val_Testata);
      })
    ).subscribe();
  }

  downloadExcel(): void {
    this.valutazioniService.exportToExcel(this.valutazioneTestataAttuale.Piva, this.valutazioneTestataAttuale.Id_Testata, "filename.xlsx");
    this.giasMessageService.successMessage(this.translocoService.translate('DownloadExcelSuccess'));
  }

  getForm(fb: FormBuilder): FormGroup {
    if (!this.mainForm)
      this.mainForm = this.createFormGroup(fb);
    return this.mainForm;
  }

  createFormGroup(fb: FormBuilder) {
    return fb.group({
      note: [''],
      dataRedazione: [this.currentDate, Validators.required],
      piano_conti: ['', Validators.required],
      ragione_sociale: ['', Validators.required],
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

  SalvaValutazione() {
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
    }
    else {
      this.submitInProgress = false;
    }
  }

  private onSubmit() {
    if (this.mainForm.valid) {
      let testata_lettura = this.valutazioniService.getLeggiTestataValue();

      let pk: PKValutazioneTestata = {
        Piva: this.valutazioneTestataAttuale.Piva,
        Id_Testata: testata_lettura.idTestata
      };

      if (this.tipoOperazione == Enum_DBTypeOperation.Write) {
        pk.Piva = this.mainForm.controls['ragione_sociale'].value.partitaIva;
      }

      console.log("submit");

      let currentObject: ValutazioneTestata = {
        primaryKey: pk,
        Ragione_Sociale: this.mainForm.controls['ragione_sociale'].value.ragioneSociale,
        Valutazione_Piano_Cod: this.mainForm.controls['piano_conti'].value.codice,
        Valutazione_Piano_Des: this.mainForm.controls['piano_conti'].value.descrizione,
        Data_Redazione: this.mainForm.controls['dataRedazione'].value,
        Note: this.mainForm.controls['note'].value,
        Valutazione_TestataxAnno: ValutazioneTestataxAnno[3],
        flag_cancellazione: false
      };

      console.log(currentObject);
      this.masterService.set_isLoading({ message: '', isLoading: true });
      return this.valutazioniService.ScriviValutazione(currentObject);
    }
  }

}

export class GiasValutazioniDDLItem {
  constructor(
    public readonly codice: string | number,
    public readonly descrizione: string,
    public dati: string = '',
    numCodice: boolean = false) {
    if (numCodice)
      this.codice = Number.parseInt(this.codice as string);
  }
}

export class GiasRagioneSocialeDDLItem {
  constructor(
    public readonly ragioneSociale: string | number,
    public readonly partitaIva: string) {
  }
}

