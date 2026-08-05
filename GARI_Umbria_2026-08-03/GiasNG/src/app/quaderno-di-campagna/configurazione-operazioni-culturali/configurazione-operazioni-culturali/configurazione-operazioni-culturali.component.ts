import { Component, OnInit } from '@angular/core';
import { faHome, faIndustry } from '@fortawesome/free-solid-svg-icons';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';

@Component({
  standalone: false,
  selector: 'app-configurazione-operazioni-culturali',
  templateUrl: './configurazione-operazioni-culturali.component.html',
  styleUrls: ['./configurazione-operazioni-culturali.component.scss']
})
export class ConfigurazioneOperazioniCulturaliComponent implements OnInit {

  constructor(private permessiUtenteService: PermessiUtenteService) { }

  faHome = faHome;
  faIndustry = faIndustry;

  ngOnInit(): void {
  }

  getPermessi(id: string): Boolean {
    switch(id) {
      case "rilievi":
        return this.permessiUtenteService.getPermesso(enum_Security_Attivita.AgronicaManutenzione_MisuraAvversita, 0)
      case "parametrighg":
        return this.permessiUtenteService.getPermesso(enum_Security_Attivita.Gestione_GHG, 0)
    }
  }
}
