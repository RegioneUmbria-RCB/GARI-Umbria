import { Component } from '@angular/core';
import { MenuClient, InformazioniAssistenza } from 'app/Service/api.service';

@Component({
  standalone: false,
  selector: 'gias-sidemenu-footer',
  templateUrl: './sidemenu-footer.component.html',
  styleUrls: ['./sidemenu-footer.component.scss']
})
export class SideMenuFooterComponent {

  info: InformazioniAssistenza | null = null;

  constructor(private menuClient: MenuClient) {
    this.menuClient
      .menuOttieniInformazioniAssistenza()
      .subscribe(x => this.info = x.RispostaStringa);
  }
}
