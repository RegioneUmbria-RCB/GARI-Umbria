import { Component } from '@angular/core';
import { AgendaClient, CategoriaOperazione, Disciplinare, Job_PK, Lavorazione, LeggiDisciplinari, ModelloClient, PopolaRilievo, RispostaStandard_1OfList_1OfDisciplinare, RispostaStandard_1OfString, Specie, TipiJob } from 'app/Service/api.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { generateGridProviders } from 'gias-kendo-grid';
import { combineLatest, debounceTime, map, Observable, of, switchMap } from 'rxjs';
import { RilieviAvversitaGridConfig } from './rilievi-avversita-grid-config.service';
import { Avversita, RilieviAvversitaService } from './rilievi-avversita.service';

const DEFAULT_LAV_CODE = "113";
const AVVERSITA_LAVORAZIONI = [{
  categoriaOperazione: null as CategoriaOperazione,
  primaryKey: {
    classType: "Lavorazione",
    codice: "113"
  } as Job_PK,
  descrizione: "Rilievi Avversita' in Campo",
  tipo: TipiJob.LAVORAZIONE
}] as Lavorazione[];

@Component({
  standalone: false,
  selector: 'app-rilievi-avversita',
  templateUrl: './rilievi-avversita.component.html',
  styleUrls: ['./rilievi-avversita.component.css'],
  providers: [
    GiasDropDownTemplateService,
    RilieviAvversitaService,
    ...generateGridProviders(RilieviAvversitaGridConfig, RilieviAvversitaComponent)
  ]
})
export class RilieviAvversitaComponent {
  specieSelected$ = this.rilieviAvversitaService.specie$;
  disciplinareSelected$ = this.rilieviAvversitaService.disciplinare$;
  avversitaSelected$ = this.rilieviAvversitaService.avversita$;

  species$: Observable<Specie[]>;
  disciplinari$: Observable<Disciplinare[]>;
  avversita$: Observable<Avversita[]>;
  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  constructor(
    private rilieviAvversitaService: RilieviAvversitaService,
    private modelloClient: ModelloClient,
    private agendaClient: AgendaClient
  ) {
    this.species$ = this.modelloClient
      .modelloGetSpecie()
      .pipe(map(x => x.RispostaStringa.sort((x, y) => x.descrizione.localeCompare(y.descrizione))));

    this.disciplinari$ = this.specieSelected$
      .pipe(
        debounceTime(100),
        switchMap(specie => this.getDisciplinari(specie)),
        map(x => x.RispostaStringa)
      );

    this.avversita$ = combineLatest([
      this.specieSelected$,
      this.disciplinareSelected$
    ])
      .pipe(
        debounceTime(100),
        switchMap(([specie, disciplinare]) => this.getAvversita(specie, disciplinare)),
        map((x: RispostaStandard_1OfString) => (JSON.parse(x.RispostaStringa) as Avversita[]))
      );
  }

  changeSpecie($event: Specie | null): void {
    this.rilieviAvversitaService.nextSpecie($event);

    // Invalidate next steps
    this.rilieviAvversitaService.nextDisciplinare(null);
    this.rilieviAvversitaService.nextAvversita(null);
  }

  changeDisiplinare($event: Disciplinare | null): void {
    this.rilieviAvversitaService.nextDisciplinare($event);

    // Invalidate next steps
    this.rilieviAvversitaService.nextAvversita(null);
  }

  changeAvversita($event: Avversita | null): void {
    this.rilieviAvversitaService.nextAvversita($event);
  }

  private getDisciplinari(specie: Specie): Observable<RispostaStandard_1OfList_1OfDisciplinare> {
    if (specie == null) {
      return of({ RispostaStringa: [] } as RispostaStandard_1OfList_1OfDisciplinare);
    }

    const payload = {
      data: new Date(),
      privato: true,
      specie: specie,
      regolamento: null,
      lavorazioni: AVVERSITA_LAVORAZIONI
    } as LeggiDisciplinari;
    return this.modelloClient.modelloLeggiDisciplinariTestataConRegolamentoConcimazione(payload);
  }

  private getAvversita(specie: Specie, disciplinare: Disciplinare): Observable<RispostaStandard_1OfString> {
    if (specie == null || disciplinare == null) {
      return of({ RispostaStringa: '[]' }); // Il server ritorna una stringa
    }

    const payload = {
      lavCod: DEFAULT_LAV_CODE,
      vegCod: specie.codice.toString(),
      dpiCod: disciplinare.codice,
      idRcdpi: disciplinare.raggruppamentiColturaliDPI?.codice?.toString() ?? '0',
      dpiPubblicoPrivato: disciplinare.disciplinarePubblicoPrivato?.toString() ?? '0',
      personalizzate: false,
    } as PopolaRilievo;
    return this.agendaClient.agendaPopolaRilievo(payload);
  }
}
