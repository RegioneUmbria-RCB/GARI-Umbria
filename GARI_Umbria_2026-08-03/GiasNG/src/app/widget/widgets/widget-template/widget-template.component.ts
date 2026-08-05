import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { WidgetConfigService } from 'app/widget-config/widget-config.service';
import { WidgetData } from 'app/widget/widgets.component';
import { Subscription } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-widget-template',
  templateUrl: './widget-template.component.html',
  styleUrls: ['./widget-template.component.css']
})
export class WidgetTemplateComponent implements OnDestroy {
  @Input() title: string;
  @Input() widget: WidgetData | null = null;
  @Input() piva: string | null = null;

  //aggiunta bottone info
  @Input() hasInfo: boolean = false;
  @Input() infoTitle: string = '';

  @Output() onSave = new EventEmitter<void>();

  widgetsBlocked: boolean;

  private subscription: Subscription;

  constructor(private widgetConfigService: WidgetConfigService) {
    this.subscription = this.widgetConfigService
      .$widgetsBlocked
      .subscribe(widgetsBlocked => this.widgetsBlocked = widgetsBlocked);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  widgetVisible(): boolean {
    if (this.widget == null) {
      return false;
    }

    if (this.widget.RichiedeAziendaSelezionata === true && (this.piva == '' || this.piva == null)) {
      return false;
    }

    return true;
  }

  save(): void {
    this.onSave.emit();
  }
}
