import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MenuEntry } from 'app/Service/permessi-utente.service';

@Component({
  standalone: false,
  selector: 'app-preferito-card',
  templateUrl: './preferito-card.component.html',
  styleUrls: ['./preferito-card.component.css']
})
export class PreferitoCardComponent {

  @Input() preferito: MenuEntry;
  @Output() clickPreferitoEvent = new EventEmitter<MenuEntry>();

  clickPreferito(pref: MenuEntry) {
    this.clickPreferitoEvent.emit(pref);
  }
}
