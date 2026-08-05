import { trigger, state, style, transition, animate } from '@angular/animations';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { LinkMenu } from 'app/Service/api.service';

@Component({
  standalone: false,
  selector: 'app-sidemenu-child',
  templateUrl: './sidemenu-child.component.html',
  styleUrls: ['./sidemenu-child.component.scss'],
  animations: [
    trigger('slideDownUp', [
      state('slideDown', style({ height: '*', display: 'block' })),
      state('slideUp', style({ height: '0', display: 'none' })),
      transition('slideDown => slideUp', [
        style({ height: '*' }),
        animate(100, style({ height: '0' }))
      ]),
      transition('slideUp => slideDown', [
        style({ display: 'block' }),
        animate(0, style({ height: '0' })),
        animate(100, style({ height: '*' }))
      ])
    ])
  ]
})
export class SidemenuChildComponent {
  @Input() menus: LinkMenu[] = [];
  @Input() open: boolean = false;

  @Output() onClick = new EventEmitter<LinkMenu>()
  @Output() hideParent = new EventEmitter<boolean>()

  menuClicked(menu: LinkMenu): void {
    this.onClick.emit(menu);
  }
}
