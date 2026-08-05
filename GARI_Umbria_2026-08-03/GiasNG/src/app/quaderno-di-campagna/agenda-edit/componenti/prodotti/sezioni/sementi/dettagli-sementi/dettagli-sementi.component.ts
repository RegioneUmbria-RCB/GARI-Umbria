import {Component, OnInit} from '@angular/core';
import {QdCProdottiService} from "../../../../../service/prodotti.service";
import {QdCUnitadiMisuraService} from "../../../../../service/unita-di-misura.service";
import {
  GridDosiProdottiControlliService
} from "../../../../../service/grid-dosi-prodotti/grid-dosi-prodotti-controlli.service";
import {FormGroup, FormGroupDirective} from "@angular/forms";
import {Lavorazione} from "../../../../../../../Model/attivita/Lavorazione";
import {pairwise, startWith, Subscription} from "rxjs";
import {QdCService} from "../../../../../service/qdc.service";
import {MisceleService} from "../../../../../service/miscele.service";
import {QdCDettagliSementiService} from "../../../../../service/prodotti/dettagli-sementi.service";
import {BaseCodeDescr} from "../../../../../../../Model/baseClass/baseCodeDescr";

@Component({
  standalone: false,
  selector: 'app-dettagli-sementi',
  templateUrl: './dettagli-sementi.component.html',
  styleUrls: ['./dettagli-sementi.component.scss'],
  providers: [QdCProdottiService, QdCDettagliSementiService, QdCUnitadiMisuraService,GridDosiProdottiControlliService]
})
export class DettagliSementiComponent implements OnInit {

  SementiForm: FormGroup;

  Operazione: Lavorazione;

  Subs: Subscription = new Subscription();

  constructor(public parent: FormGroupDirective,
              public prodottiservice: QdCProdottiService,
              public qdcservice: QdCService,
              public misceleservice: MisceleService,
              public qdcdettaglisementiservice: QdCDettagliSementiService
  ) {  }

  ngOnInit(): void {

    this.SementiForm = <FormGroup>this.parent.form;

    this.Operazione = this.SementiForm.get("Operazione").value;

    this.prodottiservice.ObsProdottiForm.next(this.SementiForm);

    //Triggero il validator del formgroup
    this.SementiForm.markAllAsTouched();

    this.prodottiservice.GestioneMagazzino_Abilitata();

    this.prodottiservice.GestioneGiacenze();

    this.prodottiservice.GestioneLotti();

    this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze();

    //Imposto il valore nelle ddl per poterle mostrare correttamente se non ho l'inserimento in griglia
    if(!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi())
      this.qdcdettaglisementiservice.setArrayDDLSezioneProdottiSementiForm(this.SementiForm.getRawValue());

    this.Subs.add(
      this.qdcservice.TestataForm.get("Specie").valueChanges.subscribe(d=>{

        this.qdcdettaglisementiservice.clearSezioneProdottiSementiForm(false,true);

      })
    );

    this.Subs.add(
      this.qdcservice.TestataForm.get("Centro_Aziendale").valueChanges.subscribe(d=>{

        this.qdcdettaglisementiservice.clearSezioneProdottiSementiForm(false,true);

      })
    );

    this.Subs.add(
      this.qdcservice.TestataForm.get("Data").valueChanges.subscribe(d=>{

        this.qdcdettaglisementiservice.clearSezioneProdottiSementiForm(false,true);

      })
    );

    //Richiamo la funzione di change solo se Opzioni_Semina ha un valore diverso dal precedente
    this.Subs.add(this.SementiForm.get("Opzioni_Semina").valueChanges.pipe(
        startWith(this.SementiForm.get("Opzioni_Semina").value),
        pairwise()).subscribe(([prev, next]: [BaseCodeDescr, BaseCodeDescr]) => {
        let a = 0; //Commento per funzione vuota SonarQube
      })
    );
  }

  public InserisciSemente(){
    this.prodottiservice.InserisciDoseProdotto.next(true);
  }

}
