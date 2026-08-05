import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { MasterService } from '../../Service/master.service';

export class FooterModel {
    visible: boolean;
    logo: string;
    phone: string;
    mail: string;
    phoneImg: string;
    mailImg: string;
}

@Component({
    standalone: false,
    selector: 'gias-master-footer',
    templateUrl: './footer.component.html',
    styleUrls: ['./footer.component.scss']
})
/** footer component*/
export class FooterComponent implements OnInit {
    footerComponent: FooterModel;
    loadComplete: boolean = false;

    link_logo: string;
    link_phoneImg: string;
    link_mailImg: string;

    constructor(private masterService: MasterService) {

    }

    ngOnInit() {
        this.masterService.initialLoadCompleteSource.subscribe((loadComplete) => {
            if (loadComplete) {
                this.masterService.currentFooter.subscribe(
                    subscribe => {
                        this.loadComplete = loadComplete;
                        this.footerComponent = subscribe;
                        this.link_mailImg = this.masterService.link_GiasBase + '/' + this.footerComponent.mailImg;
                        this.link_phoneImg = this.masterService.link_GiasBase + '/' + this.footerComponent.phoneImg;
                        this.link_logo = this.masterService.link_GiasBase + '/' + this.footerComponent.logo;
                    }
                );
            }
        });
    }



}
