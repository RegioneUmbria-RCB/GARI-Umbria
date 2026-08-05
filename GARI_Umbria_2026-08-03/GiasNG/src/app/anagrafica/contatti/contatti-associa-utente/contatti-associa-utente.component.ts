
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { WindowService } from '@progress/kendo-angular-dialog';
import { ContattiService } from 'app/Service/Anagrafica/contatti.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { ProfilazioneUtentiService } from 'app/profilazione/services/profilazione-utenti.service';
import { map } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-contatti-associa-utente',
  templateUrl: './contatti-associa-utente.component.html',
  styleUrls: ['./contatti-associa-utente.component.css'],
  providers: [{ provide: GiasDropDownTemplateService}]
})
export class ContattiAssociaUtenteComponent implements OnInit {

  associaUtenteList = [];

  utenteAssociato;

  constructor(
    private fb: FormBuilder,
    private contattiService: ContattiService,
    private giasMessageService: GiasMessageService,
    private translocoService: TranslocoService
    ) { }

  associaUtenteForm: FormGroup = this.fb.group({
    associaUtenteDdl: [],
  });

  ngOnInit(): void {
    const piva = this.getPivaFromContattiService();
    const contattoCod = this.getCodiceContattoFromContattiService();
    this.contattiService.leggiUtenteAssociato(piva, contattoCod).pipe(map(r => {
      this.utenteAssociato = r ? {codice: r.username, descrizione: r.username} : {codice: "", descrizione: ""};
      this.associaUtenteForm.controls['associaUtenteDdl'].patchValue(this.utenteAssociato);
    })).subscribe();
  }

  private getPivaFromContattiService() {
    return (this.contattiService.getUtenteDaAssociare() as any).chiave.split("_")[0];
  }

  private getCodiceContattoFromContattiService() {
    return (this.contattiService.getUtenteDaAssociare() as any).chiave.split("_")[2];
  }

  associaUtenteDdl(ddl: GiasDropDownTemplateSComponent): void {
    ddl.loading = true;
    this.contattiService.leggiUtentiDaAssociare().pipe(map(r => {
      if(this.utenteAssociato.codice != "") {
        this.associaUtenteList.push({codice:"", descrizione:""}, this.utenteAssociato);
      } else {
        this.associaUtenteList.push({codice:"", descrizione:""});
      }
      this.associaUtenteForm.controls['associaUtenteDdl'].patchValue(this.associaUtenteForm.controls['associaUtenteDdl'].value);
      r.map(t => {this.associaUtenteList.push({codice: t.UserName, descrizione: t.UserName + ' (' + t.Cognome + ' ' + t.Nome + ')'})});
      ddl.loading = false;
    })).subscribe();
  }

  onAssocia(): void {
    let utenteSelezionato = this.associaUtenteForm.controls['associaUtenteDdl'].value
    if(utenteSelezionato.codice != this.utenteAssociato.codice) {
        const piva = this.getPivaFromContattiService();
        const contattoCod = this.getCodiceContattoFromContattiService();
        const username = utenteSelezionato.descrizione.split(" ")[0];
        this.contattiService.associaUtente({piva: piva, contatto_cod: contattoCod, username: username}).subscribe();
        const contattoDes = (this.contattiService.getUtenteDaAssociare() as any).Contatto_Des;
        if (utenteSelezionato.codice == '') {
            this.giasMessageService.successMessage(this.translocoService.translate('Contatto') + " " + contattoDes + " " + this.translocoService.translate('DisconnessoConSuccesso'));
        } else {
            this.giasMessageService.successMessage(this.translocoService.translate('Utente')+ " " + utenteSelezionato.descrizione + " " + this.translocoService.translate('AssociatoAlContatto') + " " + contattoDes);
        }
        this.contattiService.getWindowAssociaUtenteRef().close()
    } else {
        // if (utenteSelezionato.codice == '') {
        //     this.giasMessageService.infoMessagge(this.translocoService.translate('SelezionareUtenteDaAssociare'));
        // } else {
        //     this.giasMessageService.infoMessagge(this.translocoService.translate('Utente')+ " " + utenteSelezionato.descrizione + " " + this.translocoService.translate('GiaAssociatoAlContatto') + " " + (this.contattiService.getUtenteDaAssociare() as any).Contatto_Des);
        // }
        this.contattiService.getWindowAssociaUtenteRef().close()
    }
  }

}
