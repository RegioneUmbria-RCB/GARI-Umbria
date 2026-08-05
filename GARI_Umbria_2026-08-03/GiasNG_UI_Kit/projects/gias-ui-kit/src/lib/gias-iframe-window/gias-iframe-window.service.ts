import { Injectable, TemplateRef, ViewContainerRef } from '@angular/core';
import { Messages, WindowCloseResult, WindowRef, WindowState } from '@progress/kendo-angular-dialog';
import { GiasIFrameWindowComponent } from './gias-iframe-window.component';
import { BehaviorSubject } from 'rxjs';
import { contestoPostMessage, messaggioPostMessage, PostMessageSelezioneFiltrone } from '../utils/models';
import { GiasWindowsService } from './gias-windows.service';

export class ButtonSettings {
  public showMinimize: boolean = false;
  public showMaximize: boolean = false;
  public showRestore: boolean = false;
  public showClose: boolean = true;
}

@Injectable({
  providedIn: 'root'
})
export class GiasIFrameWindowService {

  public titleSub: BehaviorSubject<string> = new BehaviorSubject<string>('');
  public showMinimizeSub: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  public showMaximaizeSub: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  public showRestoreSub: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  public showCloseSub: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);

  /**
   * @usageNotes Evento scaturito dal codice <code>window.parent.postMessage(objPostMessage, '*')</code>.
   * Per passaggio dati filtrone, esempio in Filtrone_Nuovo.js
   */
  public postMessageContextResult: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  window: WindowRef;
  private templateRef: TemplateRef<any>;
  private functionMessageEventListener: any;

  constructor(
    private giasWindowsService: GiasWindowsService
  ) { }

  public open(settings: IFrameWindowSettings, FunctionToCallAfterClose?: () => void, buttonSettings: ButtonSettings = new ButtonSettings()) {

    this.titleSub.next(settings.title);
    this.showMinimizeSub.next(buttonSettings.showMinimize);
    this.showMaximaizeSub.next(buttonSettings.showMaximize);
    this.showRestoreSub.next(buttonSettings.showRestore);
    this.showCloseSub.next(buttonSettings.showClose);

    if (this.window !== undefined) {
      this.window.close();
      this.window.window.destroy();
    }

    this.window = this.giasWindowsService.open({
      title: settings.title,
      content: GiasIFrameWindowComponent,
      height: settings.height,
      width: settings.width,
      keepContent: true,
    }, false);

    //--------------------------------------------------------------------------------
    // Per chiudere la window dopo che si è verificato un determinato evento
    //--------------------------------------------------------------------------------
    // ATTENZIONE!!!
    // Il messageEventListener è stato gestito anche nella GiasWindowsService,
    // per cui per poter utilizzare correttamente il codice a seguire è necessario
    // disabilitarlo nella giasWindowsService.open (messageEventListener=false)
    //--------------------------------------------------------------------------------

    this.functionMessageEventListener = (event => {
      if (event.data.messaggio) {
        if (event.data.messaggio === messaggioPostMessage.chiudiWindowGiasNG || event.data.messaggio === messaggioPostMessage.chiudiFinestra) {
          if (event.data.contesto) {
            this.gestisciContestoPostMessage(event.data);
          }
          this.close(event.data.messaggio);
          if (FunctionToCallAfterClose) {
            FunctionToCallAfterClose();
          }
        }
      } else {
        if (event.data && (event.data === messaggioPostMessage.chiudiWindowGiasNG || event.data === messaggioPostMessage.chiudiFinestra)) {
          this.close(event.data);
          if (FunctionToCallAfterClose) {
            FunctionToCallAfterClose();
          }
        }
      }
    }).bind(this);

    window.addEventListener('message', this.functionMessageEventListener);

    //--------------------------------------------------------------------------------

    this.window.content.instance.url = settings.content;

    // Intercettazione chiusura finestra

    this.window.result.subscribe((result: any) => {
      // console.log(messaggio);
      console.log('result iFrameWindowService');
      console.log(result)
      window.removeEventListener('message', this.functionMessageEventListener);
      if (result instanceof WindowCloseResult && FunctionToCallAfterClose) {
        FunctionToCallAfterClose();
      }
    });

    return this.window;
  }

  public close(messaggio: string) {
    this.window.close(messaggio);
    this.window.window.destroy();
  }

  public setTemplateRef(t: TemplateRef<any>) {
    this.templateRef = t;
  }

  private gestisciContestoPostMessage(eventData: any) {
    if (eventData.contesto === contestoPostMessage.FiltroneImpianti) {
      if (eventData.inData && eventData.inData.tipo && eventData.inData.chiavi && eventData.inData.parametri) {
        let objSelezioneFiltrone = new PostMessageSelezioneFiltrone();
        objSelezioneFiltrone.contestoPostMessage = eventData.contesto;
        objSelezioneFiltrone.tipoChiavi = eventData.inData.tipo;
        objSelezioneFiltrone.elencoChiavi = eventData.inData.chiavi.slice();
        objSelezioneFiltrone.parametri = eventData.inData.parametri.slice();
        this.postMessageContextResult.next(objSelezioneFiltrone);
      }
    }
  }

}

export class IFrameWindowSettings {
  /**
   * Defines a predicate that verifies if the closing of the Window should be prevented.
   * It applies to clicking the **Close** button of the title bar, pressing the **Esc** key or calling the `close` method.
   * Returning true from the predicate prevents the Window from closing.
   * If the **Close** button of the title bar is clicked or **Esc** is pressed, a [WindowCloseResult]({% slug api_dialog_windowcloseresult %}) instance is passed.
   * @param {any} ev
   * @param {WindowRef} [windowRef] - provided only when the window is created using a component.
   * @returns
   */
  preventClose?: (ev: any, windowRef?: WindowRef) => boolean;
  /**
   * Sets the title of the Window.
   */
  title?: string;
  /**
   * Defines the content of the Window.
   */
  content?: string;
  /**
   * Defines the content of the title bar.
   */
  titleBarContent?: TemplateRef<any>;
  /**
   * Defines the text of the labels that are shown within the Window.
   * Used primarily for localization.
   */
  messages?: Messages;
  /**
   * Specifies if the content of the Window is persisted in the DOM
   * when the Window is minimized.
   */
  keepContent?: boolean;
  /**
   * Specifies the width of the Window.
   */
  width?: number;
  /**
   * Specifies the minimum width of the Window.
   */
  minWidth?: number;
  /**
   * Specifies the height of the Window.
   */
  height?: number;
  /**
   * Specifies the minimum height of the Window.
   */
  minHeight?: number;
  /**
   * Specifies the left offset of the Window.
   */
  left?: number;
  /**
   * Specifies the top offset of the Window.
   */
  top?: number;
  /**
   * Specifies is the Window is draggable.
   */
  draggable?: boolean;
  /**
   * Sets the custom CSS classes that will be rendered on the Window wrapper element.
   * Supports the union type of values that NgClass accepts [ngClass]({{ site.data.urls.angular['ngclassapi'] }}).
   */
  cssClass?: any;
  /**
   * Sets the HTML attributes of the Window wrapper element.
   * The property accepts string key-value based pairs.
   */
  htmlAttributes?: {
    [key: string]: string;
  };
  /**
   * Specifies if the Window is resizable.
   */
  resizable?: boolean;
  /**
   * Specifies the initial state of the Window.
   */
  state?: WindowState;
  /**
   * Defines the container in which the Window will be inserted.
   * Specifying this option changes the place in the page hierarchy where the Window will be inserted.
   */
  appendTo?: ViewContainerRef;
  /**
   * Sets the focused element query selector.
   */
  autoFocusedElement?: string;
}
