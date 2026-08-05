import { ChangeDetectorRef, Directive, OnInit, OnDestroy } from '@angular/core';
import { DatePickerComponent } from '@progress/kendo-angular-dateinputs';

@Directive({ standalone:false,
  selector: '[clear-input-button]'
})
export class ClearInputButtonDirective implements OnInit, OnDestroy {
  private button: HTMLElement;
  private clickHandler = () => {
    this.component.value = null;
    this.component.valueChange.emit(null);
    this.cdr.markForCheck();
  }

  constructor(private component: DatePickerComponent, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.button = document.createElement('span');
    this.button.className = 'clear-input-button';
    this.button.appendChild(document.createTextNode('X'));
    this.button.addEventListener('click', this.clickHandler);

    this.component.wrapper.nativeElement.appendChild(this.button);
  }

  ngOnDestroy(): void {
    this.button.removeEventListener('click', this.clickHandler);
  }
}
