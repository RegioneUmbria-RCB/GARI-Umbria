import { Component, Input, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { WindowService } from '@progress/kendo-angular-dialog';
import { GiasWindowsService } from 'gias-ui-kit';
import { MasterService } from 'app/Service/master.service';
import { Subscription } from 'rxjs';

@Component({
    standalone: false,
    selector: 'gias-master-window-messaggio-errore',
    templateUrl: './window-messaggio-errore.component.html',
    styleUrls: ['./window-messaggio-errore.component.scss']
})
export class WindowMessaggioErroreComponent implements OnInit,OnDestroy {

    // Replica il funzionamento del MessaggioErrore nell'AgronicaAgenda

    constructor(private windowService: GiasWindowsService,private masterService:MasterService) { }

    @ViewChild('windowTitleBar', { static: true }) windowTitleBar: TemplateRef<any>;

    @ViewChild('windowContent', { static: true }) windowContent: TemplateRef<any>;

    public messaggio: string;

    public showWarningIcon: boolean;

    public TitleBarMessage: string;

    public show_window_messaggio_errore: boolean = false;

    public Sub:Subscription;

    ngOnInit(): void {
        this.Sub = this.masterService.currentWindowErrorMsgSource.subscribe((el)=> {
            this.show_window_messaggio_errore = el.show;
            if(this.show_window_messaggio_errore){
                this.messaggio = el.msg;
                this.showWarningIcon = el.showWarningIcon;
                this.TitleBarMessage = el.TitleBarMessage;
                this.showWindowMessaggioErrore();
            }
        });
    }

    ngOnDestroy(): void {
        this.Sub.unsubscribe();
    }

    showWindowMessaggioErrore(){
        this.windowService.open({
            content: this.windowContent,
            titleBarContent: this.windowTitleBar
        });
    }

}
