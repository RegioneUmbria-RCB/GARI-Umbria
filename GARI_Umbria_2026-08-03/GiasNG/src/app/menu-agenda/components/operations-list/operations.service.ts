import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FavoriteLinks, Operation, Operation2, Operations } from './utils';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';


@Injectable({providedIn: 'root'})
export class OperationsService {
    operationsSubject: Subject<Operation[]> = new Subject<Operation[]>();
    drawerZIndex: Subject<number> = new Subject();

    constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService) {

    }

    /*public LoadFavorites_Old() {
        this.http.post_legacy(
            FavoriteLinks.OperazioniPreferite,
            this.FavoritesParams.bind(this),
            false
        ).subscribe((data: Operation[]) => {
            this.operationsSubject.next(data);
        });
    }*/

    public LoadFavorites() {
        this.ajaxAgronicaAPIService.ajaxAPIGet<any, any>(
            FavoriteLinks.OperazioniPreferite,
            this.FavoritesParams.bind(this)
        ).subscribe(data => {
            let result: Operation[] = data.RispostaStringa;
            this.operationsSubject.next(result);
        });
    }

    /*public LoadFavoritesPerRicette_Old() {
        this.http.post_legacy(
            FavoriteLinks.OperazioniPerRicette,
            this.FavoritesParams.bind(this),
            false
        ).subscribe((data: Operation[]) => {
            this.operationsSubject.next(data);
        });
    }*/

    public LoadFavoritesPerRicette() {
        this.ajaxAgronicaAPIService.ajaxAPIGet<any, any>(
            FavoriteLinks.OperazioniPerRicette,
            this.FavoritesParams.bind(this)
        ).subscribe(data => {
            let result: Operation[] = data.RispostaStringa;
            this.operationsSubject.next(result);
        });
    }

    LoadFavoritesBrogliaccio() {
        // TODO_RV da aggiungere la chiamata che carica le operazioni favorite.
        this.operationsSubject.next([]);
    }

    private FavoritesParams(...args) {
        const master = args[1];
        const server = master.ObjParametri_Server;
        const utenti = master.ObjParametri_Utenti;
        return { objP_server: server, objP_utenti: utenti };
    }

}


