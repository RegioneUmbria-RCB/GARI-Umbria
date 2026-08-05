import { Component } from '@angular/core';
import { FiltriNuovaAnalisiConformitaComponent } from 'app/qualita-tracciabilita/components/filtri-nuova-analisi-conformita/filtri-nuova-analisi-conformita.component';
import { RiepilogoRichiesteComponent } from 'app/qualita-tracciabilita/components/riepilogo-richieste/riepilogo-richieste.component';
import { RisultatiAnalisiConformitaComponent } from 'app/qualita-tracciabilita/components/risultati-analisi-conformita/risultati-analisi-conformita.component';

@Component({
  standalone: true,
  selector: 'app-verifica-conformita',
  imports: [FiltriNuovaAnalisiConformitaComponent, RisultatiAnalisiConformitaComponent, RiepilogoRichiesteComponent],
  templateUrl: './verifica-conformita.component.html',
  styleUrl: './verifica-conformita.component.css'
})
export class VerificaConformitaComponent {

}
