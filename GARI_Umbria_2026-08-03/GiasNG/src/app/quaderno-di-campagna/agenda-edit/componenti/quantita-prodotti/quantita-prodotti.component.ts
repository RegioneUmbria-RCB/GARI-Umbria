import { Component, OnDestroy, OnInit } from '@angular/core';
import { debounceTime, delay, map, startWith, Subscription, tap } from 'rxjs';
import { QdCService } from '../../service/qdc.service';
import { ProdottoDaTrattareCDC } from 'app/Model/attivita/centri_di_costo/ProdottoDaTrattareCDC';
import { MisceleService } from '../../service/miscele.service';

@Component({
  standalone: false,
  selector: 'app-quantita-prodotti',
  templateUrl: './quantita-prodotti.component.html',
  styleUrls: ['./quantita-prodotti.component.css']
})
export class QuantitaProdottiComponent implements OnInit, OnDestroy {

  public autocorrect = true;
  public loaded = false;

  private subs: Subscription = new Subscription();

  constructor(
    public qdcservice: QdCService,
    private misceleService: MisceleService
  ) { }

  ngOnInit(): void {
    if (this.qdcservice.Sola_Lettura_QdCForm()) {
      this.qdcservice.QuantitaForm.get("Qta_Trattata").disable({ emitEvent: false });
    } else {
      this.qdcservice.QuantitaForm.get("Qta_Trattata").enable({ emitEvent: false });
    }

    this.subs.add(this.qdcservice.ProdottiDaTrattareSelezionatiFormArray
      .valueChanges
      .pipe(
        debounceTime(100),
        startWith(this.qdcservice.ProdottiDaTrattareSelezionatiFormArray.value),
        map((prodotti: ProdottoDaTrattareCDC[]) => {
          // const prodotti = this.qdcservice.ProdottiDaTrattareSelezionatiFormArray.value;

          let qtaSelezionata = 0;
          let qtaTrattata = 0;
          for (const prodotto of prodotti) {
            qtaSelezionata += prodotto.giacenzaMagazzino.Qta;
            qtaTrattata += prodotto.qtaTrattata;
          }

          this.qdcservice.QuantitaForm.get("Qta_Selezionata").setValue(qtaSelezionata);
          this.loaded = true;

          return qtaTrattata
        }),
        // Adding small delay because UI is not updating in time
        delay(100),
        tap(qtaTrattata => this.qdcservice.QuantitaForm.get("Qta_Trattata").setValue(qtaTrattata)),
        tap(() => this.misceleService.calcoloMiscele('Sup_Trattata', null, 0, this.qdcservice.QuantitaForm.get("Qta_Trattata").value))
      )
      .subscribe());
  }

  supTrattataValueChange(): void {
    if (!this.qdcservice.QuantitaForm.get('Qta_Trattata').valid) {
      return;
    }

    this.misceleService.calcoloMiscele('Sup_Trattata', null, 0, this.qdcservice.QuantitaForm.get("Qta_Trattata").value);

    const newkendoserver = this.qdcservice.ripartizionaQtaTrattata();
    this.qdcservice.GridProdottiDaTrattarePublicService.refresh(false, newkendoserver.data);
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
}
