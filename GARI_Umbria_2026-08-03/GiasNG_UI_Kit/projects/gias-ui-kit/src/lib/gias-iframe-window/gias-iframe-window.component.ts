import { Component, ElementRef, Input, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { getOrigins } from '../utils/get-origins';

@Component({
  standalone: false,
  selector: 'gias-iframe-window',
  templateUrl: './gias-iframe-window.component.html',
  styleUrls: ['./gias-iframe-window.component.css']
})
export class GiasIFrameWindowComponent implements OnInit {
  @Input() url: string;

  urlObs: Observable<string>;

  constructor(private elementRef: ElementRef) {
  }

  ngOnInit(): void {
    setTimeout(() => this.urlObs = of(this.url), 50);
  }

  postMessageToParent(message: any) {
    const iframeElement = this.elementRef.nativeElement.querySelector('iframe');

    iframeElement.onload = () => {
      const iframeWindow = iframeElement.contentWindow;
      if (iframeWindow) {
        message.height = iframeElement.height;
        // Ora l'iframe è completamente caricato, puoi fare la post
        iframeWindow.postMessage(message, getOrigins());
      } else {
        console.error('contentWindow non definito');
      }
    };
  }

}
