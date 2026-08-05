import { Injectable } from '@angular/core';
import { NotificationService } from "@progress/kendo-angular-notification";
import { Appezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { CatastoCentroAziendale } from 'app/Model/anagrafiche/CatastoCentroAziendale';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { rispostaStandard, RispostaStandard } from './master.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { BudgetService } from './Budget/budget.service';
import { TranslocoService } from '@jsverse/transloco';
import { Dialog_Type, GiasDialogService } from "./gias-dialog.service";
import { ValutazioneTestata } from 'app/Model/valutazioni/ValutazioneTestata';
import {GiasIstatService} from './istat/gias-istat.service';

@Injectable({
  providedIn: 'root',
})
export class DeleteMessageService {

  constructor(
    private istatService: GiasIstatService,
    private GiasMessageService: GiasMessageService,
    private GiasDialogService: GiasDialogService,
    private notificationService: NotificationService,
    private budgetService: BudgetService,
    private translocoService: TranslocoService) {
  }

  campiDeleteMsg_Succ(rispostaStd: RispostaStandard) {
    let obj = JSON.parse(rispostaStd.RispostaStringa);
    if (this.sonoInBudget()) {
      this.GiasMessageService.successMessage(this.translocoService.translate('Campo') + ': ' + obj.ElementoAnagrafico.descrizione + ' ' + this.translocoService.translate('EliminatoCorrettamente'));
    } else {
      this.GiasMessageService.successMessage(this.translocoService.translate('Campo') + ': ' + obj.descrizione + ' ' + this.translocoService.translate('EliminatoCorrettamente'));
    }
  }

  private sonoInBudget() {
    return this.budgetService.getBudget().activeBudget == true;
  }

  campiDeleteMsg_Fail(rispostaStd: RispostaStandard) {
    let obj = JSON.parse(rispostaStd.RispostaStringa);
    this.GiasDialogService.alertMessage(this.translocoService.translate('ErroreEliminazioneCampo') + ": " + obj.descrizione, () => { return true; }, Dialog_Type.error);
  }

  macchineDeleteMsg_Succ(rispostaStd: RispostaStandard) {
    let obj = JSON.parse(rispostaStd.RispostaStringa);
    this.GiasMessageService.successMessage(this.translocoService.translate('MacchinaAttrezzatura') + ": " + obj.descrizione + " " + this.translocoService.translate('EliminataCorrettamente'));
  }

  macchineDeleteMsg_Fail(rispostaStd: RispostaStandard) {
    let obj = JSON.parse(rispostaStd.RispostaStringa);
    this.GiasDialogService.alertMessage(this.translocoService.translate('ErroreEliminazioneMacchina') + " " + obj.descrizione, () => { return true; }, Dialog_Type.error);
  }

  appezzamentoDeleteMsg_Succ(rispostaStd: rispostaStandard<Appezzamento>) {
    let app = JSON.parse(rispostaStd.RispostaStringa.toString());
    this.notificationService.show({
      content: this.translocoService.translate('Appezzamento') + ': ' + app.descrizione + " " + this.translocoService.translate('EliminatoCorrettamente'),
      cssClass: 'button-notification',
      animation: { type: 'slide', duration: 400 },
      position: { horizontal: 'right', vertical: 'top' },
      type: { style: 'success', icon: true },
      closable: true,
    });
  }

  appezzamentoDeleteMsg_Fail(rispostaStd: rispostaStandard<Appezzamento>) {
    let app = JSON.parse(rispostaStd.RispostaStringa.toString());
    this.GiasDialogService.alertMessage('Errore durante eliminazione appezzamento: ' + app.descrizione, () => { return true; }, Dialog_Type.error);
  }

  catastoDeleteMsg_Succ(cat: CatastoCentroAziendale) {
    let provincia = this.istatService.getProvincie().filter((p) => p.Istat_Prov == cat.particella.primaryKey.Prov);

    this.istatService.leggiComuni(cat.particella.primaryKey.Prov).then((el) => {
      let comune = el.filter((c) => c.codice == cat.particella.primaryKey.Com);
      this.GiasMessageService.successMessage(this.translocoService.translate('ParticellaCatastale') + ": " + this.translocoService.translate('Provincia') + provincia[0].Provincia_Des + ' (' + cat.particella.primaryKey.Prov + ')'
        + ', ' + this.translocoService.translate('Comune') + comune[0].descrizione + ' (' + cat.particella.primaryKey.Com + ')'
        + ', ' + this.translocoService.translate('Sezione') + cat.particella.primaryKey.Sezione
        + ', ' + this.translocoService.translate('Foglio') + cat.particella.primaryKey.Foglio
        + ', ' + this.translocoService.translate('Numero') + cat.particella.primaryKey.Numero
        + ', ' + this.translocoService.translate('Subalterno') + cat.particella.primaryKey.Subalterno + this.translocoService.translate('EliminataCorrettamente'));
    })
  }

  catastoDeleteMsg_Fail(cat: CatastoCentroAziendale) {
    let provincia = this.istatService.getProvincie().filter((p) => p.Istat_Prov == cat.particella.primaryKey.Prov);

    this.istatService.leggiComuni(cat.particella.primaryKey.Prov).then((el) => {
      let comune = el.filter((c) => c.codice == cat.particella.primaryKey.Com);
      let content = this.translocoService.translate('ErroreEliminazioneMacchina') + ": " + this.translocoService.translate('Provincia') + provincia[0].Provincia_Des + ' (' + cat.particella.primaryKey.Prov + ')'
        + ', ' + this.translocoService.translate('Comune') + comune[0].descrizione + ' (' + cat.particella.primaryKey.Com + ')'
        + ', ' + this.translocoService.translate('Sezione') + cat.particella.primaryKey.Sezione
        + ', ' + this.translocoService.translate('Foglio') + cat.particella.primaryKey.Foglio
        + ', ' + this.translocoService.translate('Numero') + cat.particella.primaryKey.Numero
        + ', ' + this.translocoService.translate('Subalterno') + cat.particella.primaryKey.Subalterno
      //this.GiasMessageService.errorMessage(content);
      this.GiasDialogService.alertMessage(content, () => { return true; }, Dialog_Type.error);
    })
  }

  impreseDeleteMsg_Succ(r: rispostaStandard<Impresa>) {
    let imp = r.RispostaStringa
    this.GiasMessageService.successMessage(this.translocoService.translate('Impresa') + ": " + imp.ragioneSociale + " " + this.translocoService.translate('EliminataCorrettamente'));
  }

  valutazionieleteMsg_Succ(r: rispostaStandard<ValutazioneTestata>) {
    let imp = r.RispostaStringa
    this.GiasMessageService.successMessage(this.translocoService.translate('Valutazione') + ": " + imp.Ragione_Sociale + " " + this.translocoService.translate('EliminataCorrettamente'));
  }

  valutazioniDeleteMsg_Succ(imp: any) {
    this.GiasMessageService.successMessage(this.translocoService.translate('Valutazione') + ": " + imp.primaryKey.Id_Testata + " " + this.translocoService.translate('EliminataCorrettamente'));
  }

  impreseDeleteMsg_Fail(r: rispostaStandard<Impresa>) {
    let imp = r.RispostaStringa
    let content = this.translocoService.translate('ErroreEliminazioneImpresa') + ": " + imp.ragioneSociale;
    //this.GiasMessageService.errorMessage(content)
    //this.GiasDialogService.alertMessage(content, ()=> { return true;}, Dialog_Type.error);
  }

  valutazioniDeleteMsg_Fail(r: rispostaStandard<ValutazioneTestata>) {
    let imp = r.RispostaStringa
    let content = this.translocoService.translate('ErroreEliminazioneValutazione') + ": " + imp.Ragione_Sociale;
    //this.GiasMessageService.errorMessage(content)
    //this.GiasDialogService.alertMessage(content, ()=> { return true;}, Dialog_Type.error);
  }

}
