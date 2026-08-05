import {Component, EventEmitter, forwardRef, Input, OnInit, Output, ViewChild, ViewEncapsulation} from '@angular/core';
import {
  ButtonFillMode,
  ButtonRounded,
  ButtonSize,
  ButtonThemeColor,
  DropDownButtonComponent,
  PreventableEvent
} from '@progress/kendo-angular-buttons';
import {SVGIcon} from '@progress/kendo-svg-icons';
import {faEllipsis, IconDefinition} from '@fortawesome/free-solid-svg-icons';
import {NG_VALUE_ACCESSOR} from '@angular/forms';
import {PopupSettings} from '@progress/kendo-angular-dropdowns';

@Component({
  standalone: false,
  selector: 'app-drop-down-button',
  templateUrl: './gias-drop-down-button.component.html',
  styleUrls: ['./gias-drop-down-button.component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => GiasDropDownButtonComponent),
      multi: true
    }
  ]

})
export class GiasDropDownButtonComponent implements OnInit {
  @Input() item: any | undefined = undefined;
  @Input() iconDefinition: IconDefinition = faEllipsis; // icon shown by the DropDownButton

  @Input() arrowIcon: boolean;
  @Input() buttonAttributes: { [key: string]: string; };
  @Input() buttonClass: any;
  @Input() data: GiasDropDownButtonActionItem[];
  @Input() disabled: boolean;
  @Input() fillMode: ButtonFillMode;
  @Input() icon: string;
  @Input() iconClass: string;
  @Input() imageUrl: string;
  @Input() popupSettings: PopupSettings;
  @Input() rounded: ButtonRounded;
  @Input() size: ButtonSize;
  @Input() svgIcon: SVGIcon;
  @Input() tabIndex: number;
  @Input() textField: string;
  @Input() themeColor: ButtonThemeColor;

  @Output() close: EventEmitter<PreventableEvent>;
  @Output() itemClick: EventEmitter<any>;
  @Output() blur: EventEmitter<any>;
  @Output() focus: EventEmitter<any>;
  @Output() open: EventEmitter<PreventableEvent>;

  @ViewChild('dropDownButton') ddb: DropDownButtonComponent;

  constructor() { }

  ngOnInit(): void {
    // this.ddb.isOpen;
    //
    // this.ddb.focus();
    // this.ddb.toggle(false);
    // this.ddb.togglePopupVisibility();
  }
}

export interface GiasDropDownButtonActionItem {
  text: string;
  disabled: boolean;
  icon: string;
  click: (...data) => any;
}
