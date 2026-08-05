import { Component, EventEmitter, HostListener, Inject, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, TemplateRef } from '@angular/core';
import { Router } from "@angular/router";
import { PanelBarCollapseEvent, PanelBarExpandEvent } from "@progress/kendo-angular-layout";
import { BehaviorSubject, Subscription } from "rxjs";
import { GIAS_USERNAME_TOKEN, IUsernameService } from '../utils/username.service';
import { GiasExpansionPanelService, IPanelCookie } from '../gias-expansion-panel/gias-expansion-panel.service';

@Component({
  standalone: false,
  selector: 'gias-panelbar',
  templateUrl: './gias-panelbar.component.html',
  styleUrls: ['./gias-panelbar.component.css']
})
export class GiasPanelBar implements OnInit, OnDestroy, OnChanges {

  @Input() id: string;
  @Input() disabled: boolean;
  @Input() expand: boolean = false;
  @Input() headerTemplate: TemplateRef<any> = null;
  @Input() contentTemplate: TemplateRef<any> = null;
  @Input() willStoreCookies = true;

  //Se dentro il panelbar c'è una grid con l'autofitcolumn è meglio impostare  keepItemContent = false
  //per evitare problemi con la larghezza delle colonne
  @Input() keepItemContent = false;

  @Output() OnExpandPanel = new EventEmitter<PanelBarExpandEvent>();

  public IsExpanded = new BehaviorSubject(false);

  @Input() loadContent: boolean = false;

  Subs = new Subscription();

  constructor(
    private service: GiasExpansionPanelService,
    @Inject(GIAS_USERNAME_TOKEN) private utenteService: IUsernameService,
    private router: Router
  ) { }

  @HostListener('window:beforeload')
  async ngOnInit() {
    if (!this.id || this.id.length === 0) {
      this.willStoreCookies = false;
    }

    if (this.willStoreCookies) {
      await this.applyCookies();
    }

    //Carico il contenuto del panel solo quando viene aperto
    //se keepItemContent = false allora il componente dentro viene ricaricato ogni volta che si
    //apre il pannello
    //se keepItemContent = true allora il componente dentro viene ricaricato solo la prima volta che si
    //apre il pannello
    this.Subs.add(this.IsExpanded.subscribe(value => {
      if (value) {
        if (!this.keepItemContent) {
          this.loadContent = true
        } else {
          if (!this.loadContent) {
            this.loadContent = true;
          }
        }
      }
    }));
  }

  private async applyCookies(): Promise<void> {
    this.id = this.getCookieToken();

    const settings: IPanelCookie = await this.service.getConfigCookies(this.id);

    if (settings) {
      this.IsExpanded.next(settings.isExpanded);

      this.expand = this.IsExpanded.getValue();
    }
  }

  private getCookieToken(): string {
    const currentUser = this.utenteService.getCurrentUser();

    const user = currentUser ?? { Username: 'NoUser' };
    return `PanelBar_${user.Username}_${this.router.url}_${this.id}`;
  }

  @HostListener('window:beforeunload')
  async ngOnDestroy() {
    if (this.willStoreCookies) {
      const settings: IPanelCookie = {
        isExpanded: this.IsExpanded.getValue()
      };

      await this.service.storeConfigCookie(this.id, settings);
    }
  }

  onCollapse(event: PanelBarCollapseEvent) {
    this.IsExpanded.next(false);
  }

  onExpand(event: PanelBarExpandEvent) {
    this.IsExpanded.next(true);

    this.OnExpandPanel.emit(event);
  }

  //Utilizzo l'ngOnChanges così ogni volta che viene cambiato l'@Input() expand aggiorno anche il
  //Behaviour Subject
  ngOnChanges(changes: SimpleChanges) {
    if (this.IsExpanded.getValue() !== this.expand) {
      this.IsExpanded.next(this.expand);
    }
  }
}
