import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {TranslocoService} from '@jsverse/transloco';
import {Subject, takeUntil} from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-on-close-esercizio',
  templateUrl: './on-close-esercizio.component.html',
  styleUrls: ['./on-close-esercizio.component.css']
})
export class OnCloseEsercizioComponent implements OnInit, OnDestroy {
  @Input() dataItem: any;

  replicaForm: FormGroup;
  chiusuraForm: FormGroup;

  protected readonly TODAY: Date = new Date();

  private $signal: Subject<void> = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private transloco: TranslocoService
  ) {  }

  ngOnDestroy(): void {
    this.$signal.next();
  }

  ngOnInit(): void {
    this.chiusuraForm = this.fb.group({
      chiusura: {value: this.chiusuraSwitchDefaultValue(), disabled: this.chiusuraSwitchDefaultValue()}
    });

    this.replicaForm = this.fb.group({
      replica: {value: this.replicaSwitchDefaultValue(), disabled: this.disableReplicaSwitch()}
    });

    this.chiusuraForm.controls['chiusura'].valueChanges.pipe(
      takeUntil(this.$signal)
    ).subscribe(v => this.onChiusuraChange(v));
  }

  showReplicaSwitch(): boolean {
    return !!this.dataItem.replicaGiasPiva && this.dataItem.replicaGiasPiva !== '';
  }

  disableReplicaSwitch(): boolean {
    return !!this.dataItem.codiceImpiantoRibaltato && this.dataItem.codiceImpiantoRibaltato !== '';
  }

  showReplicaAlert(): boolean {
    return this.replicaForm.controls['replica'].value && !this.disableReplicaSwitch();
  }

  onReplicaEsercizioMessage(): string {
    return `${this.transloco.translate('CreaImpiantoEsercizioSuAltraAziendaAlert')} :\n${this.dataItem.replicaGiasRagioneSociale}`;
  }

  isAlreadyClosed(): boolean {
    return !!this.dataItem.Distinta_Chiusa && this.dataItem.Distinta_Chiusa.toLowerCase().trim() === 'si';
  }

  private chiusuraSwitchDefaultValue(): boolean {
    return !!this.dataItem.Distinta_Chiusa && this.dataItem.Distinta_Chiusa.toLowerCase().trim() === 'si';
  }

  private replicaSwitchDefaultValue(): boolean {
    return !!this.dataItem.codiceImpiantoRibaltato && this.dataItem.codiceImpiantoRibaltato !== '';
  }

  private onChiusuraChange(v: boolean): void {
    if (v) {
      this.replicaForm.controls['replica'].disable();
      this.replicaForm.controls['replica'].setValue(v);
    } else if (!this.disableReplicaSwitch()) {
      this.replicaForm.controls['replica'].enable();
      this.replicaForm.controls['replica'].setValue(v);
    }
  }
}
