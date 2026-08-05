import { ElementRef, Injectable } from '@angular/core';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { BehaviorSubject } from 'rxjs';
import { enum_TreeContext } from "../enum/tree-context";

@Injectable({ providedIn: 'root' })
export class TreeContainerService {
    public expander: BehaviorSubject<boolean | null> = new BehaviorSubject(false);
    public disabled: boolean;
    public selectedImpresaChangedSoUpdateTree = true;
    public drawerRef: ElementRef;
    public treeContainerContext: enum_TreeContext;

    constructor(public master: ObjParametriAgendaService) { }

    isContextAnagrafiche(){
        return this.treeContainerContext === enum_TreeContext.Anagrafiche;
    }

    isContextGis(){
        return this.treeContainerContext === enum_TreeContext.Gis;
    }

}
