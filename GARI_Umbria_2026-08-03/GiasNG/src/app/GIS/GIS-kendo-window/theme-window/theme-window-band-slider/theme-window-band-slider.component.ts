import { AfterViewInit, Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { DatiColoreTema, TemaSelezionato } from "../theme-window.component";

@Component({
  standalone: false,
  selector: 'theme-window-band-slider',
  templateUrl: './theme-window-band-slider.component.html',
  styleUrls: ['./theme-window-band-slider.component.css']
})
export class ThemeWindowBandSliderComponent implements OnChanges, AfterViewInit {
  @Input() theme: TemaSelezionato;
  @Input() scale: DatiColoreTema[] = [];

  @Output() output = new EventEmitter<[number, number]>();

  input1: number = 0;
  input2: number = 0;

  ngOnChanges(changes: SimpleChanges): void {
    // Apply login only when theme changes
    const themeChange = changes['theme'];
    if (themeChange == null || themeChange.previousValue == themeChange.currentValue) {
      return;
    }

    if (this.scale.length == 0) {
      return;
    }

    this.input1 = this.scale[0].valoreMax;
    this.input2 = this.scale[this.scale.length - 1].valoreMax;

    this.setBubbleValue(".range-input1", this.input1, this.input1, this.input2);
    this.setBubbleValue(".range-input2", this.input2, this.input1, this.input2);
  }

  ngAfterViewInit(): void {
    this.createListener(".range-input1");
    this.createListener(".range-input2");
  }

  onChange(): void {
    const min = this.input1 < this.input2 ? this.input1 : this.input2;
    const max = this.input1 >= this.input2 ? this.input1 : this.input2;
    this.output.emit([min, max]);
  }

  private createListener(className: string) {
    const input = document.querySelector(className);
    if (input == null) {
      return;
    }

    const range = input.querySelector(".bubble-input") as HTMLInputElement;
    const bubble = input.querySelector(".bubble") as HTMLOutputElement;
    range.addEventListener("input", () => {
      this.setBubble(bubble, +range.value, +range.min, +range.max);
    });
  }

  private setBubbleValue(className: string, value: number, min: number, max: number) {
    const input = document.querySelector(className);
    if (input == null) {
      return;
    }

    const bubble = input.querySelector(".bubble") as HTMLOutputElement;
    this.setBubble(bubble, value, min, max);
  }

  private setBubble(bubble: HTMLOutputElement, value: number, min: number, max: number): void {
    const percentage = Number(((value - min) * 100) / (max - min));
    bubble.innerHTML = `${value}`;
    bubble.style.left = `${percentage}%`;
  }
}
