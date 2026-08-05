import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-conformita-widget',
  templateUrl: './conformita-widget.component.html',
  styleUrls: ['./conformita-widget.component.css']
})
export class ConformitaWidgetComponent {
  @Input() params: string | null = null;
}
