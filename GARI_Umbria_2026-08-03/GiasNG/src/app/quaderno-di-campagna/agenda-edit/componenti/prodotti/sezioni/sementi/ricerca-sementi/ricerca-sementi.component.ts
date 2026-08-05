import {AfterViewInit, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { QdCProdottiService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti.service';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { CELL_TYPES } from 'gias-ui-kit';
import { ColumnCombobox } from 'gias-ui-kit';
import { MultiColumnComboboxService } from 'gias-ui-kit';
import { from, skip, Subscription } from 'rxjs';
import {UtilityFunctions} from "../../../../../../../Utility/UtilityFunctions";
import { GiasMultiColumnComboboxTemplateComponent } from 'gias-ui-kit';
import {QdCDettagliSementiService} from "../../../../../service/prodotti/dettagli-sementi.service";

@Component({
  standalone: false,
  selector: 'app-ricerca-sementi',
  templateUrl: './ricerca-sementi.component.html',
  styleUrls: ['./ricerca-sementi.component.scss'],
  providers:[ MultiColumnComboboxService ]
})
export class RicercaSementiComponent implements OnInit,OnDestroy,AfterViewInit {

    Subs: Subscription= new Subscription();

    public Categoria_Magazzino: number;

    public Lav_Cod: number;

    public ColumnComboboxProdotti: Array<ColumnCombobox> = [];

    ProdottiForm: FormGroup;

    public ServerFiltering = true;

    @ViewChild("MultiColumnCombobox") MultiColumnCombobox: GiasMultiColumnComboboxTemplateComponent;

  constructor(public parent: FormGroupDirective,
                public prodottiservice: QdCProdottiService,
                public qdcservice: QdCService,
                public qdcdettaglisementiservice: QdCDettagliSementiService,
                private multicolumncomboboxservice: MultiColumnComboboxService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private translocoService: TranslocoService) { }

  ngOnInit(): void {

    this.ProdottiForm = <FormGroup> this.parent.form;

    this.Categoria_Magazzino = this.ProdottiForm.get("Categoria_Magazzino").value;

    this.Lav_Cod = this.ProdottiForm.get("Operazione").value.primaryKey.codice;

    this.Subs.add(this.multicolumncomboboxservice.currentMultiColumnComboboxValueObject.pipe(skip(1)).subscribe(async ddlElem=>{
        switch(ddlElem.FormControlName){
            case 'Prodotto':
                //Va in errore la Ricerca della Sementi se la si lancia la ricerca su tutto senza filtro e senza l'impostazione di andare a amagzzino su un db
                //che ha la creazione delle materie prime in automatico va in out of memory la ricerca perchè vengono caricate migliaia di materie prime senza filtro
                //per Veg_Cod
                await this.qdcdettaglisementiservice.changeSemente(ddlElem.Value);
                break;
        }
    }));

    this.setColumnComboboxSementi();

    this.ServerFiltering = this.prodottiservice.Abilita3CaratteriServerFiltering();
  }

  ngAfterViewInit() {
      this.prodottiservice.MultiColumnComboboxProdotti = this.MultiColumnCombobox;
  }

  ngOnDestroy(): void {
    this.Subs.unsubscribe();
  }

  setColumnComboboxSementi() {
    //Configura le colonne della combobox
    //Per le sementi la colonna del lotto la mostro sempre
      //N.B. Tenere allineato con Obj_Empty_MultiColumnComboboxSemina
    this.ColumnComboboxProdotti.push(
        new ColumnCombobox({
            field: "prodotto.descrizione",
            title: this.translocoService.translate('Prodotto')
        }
        /*,
        {
            width: 200
        }*/
        ),
        new ColumnCombobox(
            {
                field: "codArticolo",
                title: this.translocoService.translate('CodiceArticolo')
            },
            {
                width: 140
            }
        ));

      //Mostro le colonne del Magazzino solo se la GestioneMagazzino è Abilitata
      if (this.prodottiservice.GestioneMagazzino_Abilitata()) {
          this.ColumnComboboxProdotti.push(
              new ColumnCombobox(
                  {
                      field: "MagazziniMovimentazioni",
                      title: this.translocoService.translate('GiacenzaAllaData')
                  },
                  {
                      type: CELL_TYPES.NUMBER,
                      isArray: true,
                      Arrayfield: "MagazziniMovimentazioni.Qta",
                      formatNumbertolocal: true,
                      digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                  }
              ),
              new ColumnCombobox(
                  {
                      field: "QtaTot",
                      title: this.translocoService.translate('GiacenzaTotale')
                  },
                  {
                      type: CELL_TYPES.NUMBER,
                      isArray: true,
                      Arrayfield: "MagazziniMovimentazioni.QtaTot",
                      formatNumbertolocal: true,
                      digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                  }
              ),
              new ColumnCombobox(
                  {
                      field: "udm",
                      title: this.translocoService.translate('um')
                  },
                  {
                      isArray: true,
                      Arrayfield: "MagazziniMovimentazioni.udm.simbolo",
                      width: 60
                  }
              ),
              new ColumnCombobox(
                  {
                      field: "Magazzino_descrizione",
                      title: this.translocoService.translate('Magazzino')
                  },
                  {
                      isArray: true,
                      Arrayfield: "MagazziniMovimentazioni.Magazzino.descrizione",
                  }
              ),
              new ColumnCombobox(
                  {
                      field: "Lotto",
                      title: this.translocoService.translate('Lotto2')
                  },
                  {
                      isArray: true,
                      Arrayfield: "MagazziniMovimentazioni.Lotto",
                      width: 200
                  }
              )
          );
      }
  }

  loadFunctionddlServerFilteringProdotti(filter: string) {

      return from(this.qdcdettaglisementiservice.getArray_Sementi(filter));

  }

    async openddl(ddlEl: GiasMultiColumnComboboxTemplateComponent){

        if(!this.ServerFiltering && !this.qdcdettaglisementiservice.Prima_Apertura_MultiColumn_Semente){
            UtilityFunctions.loadDropDownItems(ddlEl,this.qdcdettaglisementiservice.getArray_Sementi(''));
            this.qdcdettaglisementiservice.Prima_Apertura_MultiColumn_Semente = true;
        }

    }

    public Numero_Sementi(){

      let number = this.prodottiservice?.MultiColumnComboboxProdotti?.GetNumberOfRows();

      let descrizione = "("+this.translocoService.translate('Trovati') + ": "+ number +")"

      return descrizione;
    }

    public filterChangeSementi(filter: string){
      let a = 0; //Commento per funzione vuota SonarQube
    }

}
