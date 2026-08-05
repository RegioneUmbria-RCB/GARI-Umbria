import { Directive, ElementRef } from "@angular/core";
import { TooltipDirective } from "@progress/kendo-angular-tooltip";

const TIMEOUT = 7000;
const MOBILE_AGENTS = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Mobile|mobile|CriOS/i;

@Directive({ standalone:false, selector: '[gisTooltip]' })
export class GISTooltipDirective extends TooltipDirective {
  timeout: ReturnType<typeof setTimeout> | null = null;
  isMobile: boolean | null = null;

  show(anchor: ElementRef | Element): void {
    this.isMobile ??= MOBILE_AGENTS.test(navigator.userAgent);

    // Add timeout only if on mobile
    if (this.isMobile) {
      this.clear();
      this.timeout = setTimeout(() => this.hide(), TIMEOUT);
    }

    super.show(anchor);
  }

  hide(): void {
    this.clear();
    super.hide();
  }

  private clear(): void {
    if (this.timeout == null) {
      return;
    }

    clearTimeout(this.timeout);
    this.timeout = null;
  }
}
