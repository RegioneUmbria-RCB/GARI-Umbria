import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { KENDO_INPUTS } from "@progress/kendo-angular-inputs";
import { KENDO_BUTTONS } from "@progress/kendo-angular-buttons";
import { KENDO_ICONS } from "@progress/kendo-angular-icons";
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus, faMinus, faArrowRight } from '@fortawesome/free-solid-svg-icons';
import { Subject } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { CustomComponent } from 'gias-kendo-grid';
import { Enum_DBTypeOperation, Tipo_Attivita, ObjParametriAgenda } from 'gias-ui-kit';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { enum_PagineGiasNG } from 'app/Model/TipiEnumerativi';
import { RelatedOperation } from 'app/qualita-tracciabilita/models/related-operation.model';
import { MasterService } from 'app/Service/master.service';
import { VerificaDisciplinariService } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';

@Component({
  standalone: true,
  selector: 'app-related-operations-cell',
  templateUrl: './related-operations-cell.component.html',
  styleUrl: './related-operations-cell.component.css',
  imports: [FormsModule, ReactiveFormsModule, KENDO_INPUTS, KENDO_BUTTONS, KENDO_ICONS, FontAwesomeModule],
})
export class RelatedOperationsCellComponent implements OnInit, OnDestroy, CustomComponent {
  @Input() input: any;
  @Input() edit: boolean;
  @Input() field: string;
  @Input() formGroup: FormGroup;

  protected faPlus = faPlus;
  protected faMinus = faMinus;
  protected faArrowRight = faArrowRight;
  protected showOperationsDetails: boolean = false;
  /** Array of related operations with their details */
  protected relatedOperations: RelatedOperation[] = [];
  private _signal$ = new Subject<void>();

  constructor(
    private gestioneRichieste: GestioneRichiesteService,
    private analisiService: VerificaDisciplinariService,
    private agenda: ObjParametriAgendaService,
    private master: MasterService,
    private transloco: TranslocoService,
  ) { }

  get canReadQdc() {
    return this.analisiService.canReadQdc;
  }

  get expandIcon() {
    return this.showOperationsDetails ? faMinus : faPlus;
  }

  ngOnInit(): void {
    this.relatedOperations = this.input[this.field];
  }

  ngOnDestroy(): void {
    this._signal$.next();
    this._signal$.complete();
  }

  toggleOperationsDetails() {
    this.showOperationsDetails = !this.showOperationsDetails;
  }

  getRelatedOperationString(operation: RelatedOperation): string {
    const date = new Date(operation.DataOperazione).toLocaleDateString();
    const desLib = operation.Descrizione;
    return `[${date}] - ${desLib}`;
  }

  /**
   * @param operation the related operation as {Item1: Id_Agenda, Item2: Des_Lib, Item3: Data_Movimeto}
   */
  goToOperation(operation: RelatedOperation) {
    this.master.set_isLoading({ isLoading: true });

    const Parametri: Array<ParametriAggiuntivi_QueryString> = [];
    const objAgenda: ObjParametriAgenda = this.agenda.getObjParamValue();
    objAgenda.Sa_Cod = 0;
    objAgenda.Data = operation.DataOperazione;
    objAgenda.Id_Agenda = operation.IdAgenda;
    objAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    objAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
    this.agenda.changeObjParametriAgenda(objAgenda);

    this.gestioneRichieste.gestionePassaggioStessoSito_Aperto_in_Iframe(
      enum_PagineGiasNG.Pagina_Edit_Attivita, Parametri, -1,
      this.transloco.translate("ModificaOperazione"), true
    );
  }
}
