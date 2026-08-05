import { Component, EventEmitter, HostListener, Inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ExpansionPanelActionEvent } from '@progress/kendo-angular-layout';
import { GiasExpansionPanelService, IPanelCookie } from './gias-expansion-panel.service';
import { GIAS_USERNAME_TOKEN, IUsernameService } from '../utils/username.service';

@Component({
  standalone: false,
  selector: 'gias-expansionpanel',
  styleUrls: ['./gias-expansion-panel.component.css'],
  templateUrl: './gias-expansion-panel.component.html',
})
export class GiasExpansionPanelComponent implements IPanelCookie, OnInit, OnDestroy {
  /**
 * Per impostare il header occore utilizzare il selettore CSS 'headerContent'
 * come nel esempio. Queste righe di codice saranno utilizzate dentro
 * kendoExpansionPanelTitleDirective.
 * <div headerContent class="header-content">
 *   <span><strong> Titolo </strong></span>
 * </div>
 */

  @Input() id: string;
  @Input() disabled: boolean;
  @Input() isExpanded = false;
  @Input() willStoreCookies = true;

  @Output() action = new EventEmitter<ExpansionPanelActionEvent>();

  constructor(
    @Inject(GIAS_USERNAME_TOKEN) private userService: IUsernameService,
    private service: GiasExpansionPanelService,
    private router: Router) { }

  @HostListener('window:beforeload')
  async ngOnInit() {
    if (!this.id || this.id.length === 0) {
      this.willStoreCookies = false;
    }

    if (this.willStoreCookies) {
      await this.applyCookies();
    }
  }

  private async applyCookies(): Promise<void> {
    this.id = this.getCookieToken();

    const settings: IPanelCookie = await this.service.getConfigCookies(this.id);
    Object.keys(settings).forEach(key => this[key] = settings[key]);
  }

  private getCookieToken(): string {
    const currentUser = this.userService.getCurrentUser();

    const user = currentUser ?? { Username: 'NoUser' };
    return `ExpPanel_${user.Username}_${this.router.url.split('?')[0]}_${this.id}`;
  }

  @HostListener('window:beforeunload')
  async ngOnDestroy() {
    if (this.willStoreCookies) {
      const settings: IPanelCookie = {
        isExpanded: this.isExpanded
      };

      await this.service.storeConfigCookie(this.id, settings);
    }
  }

  onAction(event: ExpansionPanelActionEvent) {
    this.action.emit(event);
  }
}
