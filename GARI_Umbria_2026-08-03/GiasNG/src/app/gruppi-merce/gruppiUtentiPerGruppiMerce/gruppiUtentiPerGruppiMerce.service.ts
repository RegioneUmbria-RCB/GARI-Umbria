import { Injectable } from '@angular/core';
import { DropdownListItem } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { BehaviorSubject, Subject } from 'rxjs';
import { ImpreseDDLCtrlObj, MOSTRA_TUTTO as MOSTRA_TUTTO_ID, PublicServices, RigheSelezionate, ScrivPermessiHttpDto } from './utils';


/** Una cosa molto specifica a questo servizio, quindi definisco qui */
type EnumDictionary<T extends number, U> = {
    [K in T]: U;
};


@Injectable()
export class GruppiUtentiPerGruppiMerceSerivce 
{
    // :public
    public aggiornaGruppiMerceGrid: Subject<void> = new Subject<void>();
    public impreseCtrl: ImpreseDDLCtrlObj = new ImpreseDDLCtrlObj({
        imprese: [],
        source: [],
        defaultItem: {
            name: "Mostra tutte le imprese",
            id: MOSTRA_TUTTO_ID,
        },
        selected: null,
        loading: true,
        modificaAbilitata: false
    });

    public gridServices: EnumDictionary<PublicServices, GridPublicService> = {
        [PublicServices.GruppiUtenti]: null,
        [PublicServices.GruppiMerce]: null,
        [PublicServices.GruppiUtentiPerGruppiMerce]: null
    }

    // :private
    private righeSelezionate: RigheSelezionate = new RigheSelezionate({ MustStillBeInitialized: true });

    constructor()
    {
        this.impreseCtrl.selected = this.impreseCtrl.defaultItem;
    }

    initializePubService(name: PublicServices, service: GridPublicService) 
    {
        this.gridServices[name] = service;
    }


    setRigheSelezionate(righe: RigheSelezionate) 
    {
        this.righeSelezionate = righe;
    }

    prepareSelectedRowsForSending(): ScrivPermessiHttpDto
    {
        return {
            Gruppi_Utente_codici: this.righeSelezionate.GruppiUtenti.map(utente => utente.Gruppi_Utente_cod),
            Ids_Gruppo_Merce: this.righeSelezionate.GruppiMerce.map(merce => merce.Id_Gruppo_Merce)
        }
    }
}
