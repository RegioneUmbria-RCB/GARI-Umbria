import {Injectable} from '@angular/core';
import {enum_Impostazioni_Utenti} from 'app/Model/Impostazioni_Utenti.enum';
import {AjaxAgronicaAPIService} from 'app/Service/ajax-agronica.api.service';
import {MasterService} from 'app/Service/master.service';
import {FILTRO_CATEGORIE_MAGAZZINO} from '../impostazioni-utente/impostazioni.model';
import {map, Observable, of} from 'rxjs';
import {BaseCodeDescr} from "../../Model/baseClass/baseCodeDescr";
import {LeggiCategorieMagazzinoImpostazioni as ILeggiCategorieMagazzinoImpostazioni} from "../../Service/api.service";
import {enum_Dati_App} from "../../amministrazione-sistema/dati-app/consulta-sincro-dati-app/consulta-sincro.service";
import { TranslocoService } from '@jsverse/transloco';
import { isNumeric } from '@progress/kendo-data-query/dist/npm/utils';

@Injectable({
  providedIn: 'root'
})
export class CategorieMagazzinoService {
    private linkFiltroCategorieMagazzino = "MetaschemaNG/LeggiCategorieMagazzinoImpostazioni";

    constructor(
      private transloco: TranslocoService,
      private masterService: MasterService,
      private ajaxAgronicaAPIService: AjaxAgronicaAPIService
    ) { }

    leggiCategorieMagazzino(filter?: string) {
        if (filter === undefined) filter = '';
        const alreadyLoading = this.masterService.get_isLoading().isLoading;
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
          "MetaschemaNG/LeggiGenericoCategorieMagazzino",
            {Filtro: filter}, !alreadyLoading
        ).pipe(map((risposta) => risposta.RispostaStringa));
    }

    public leggiUnitaMisura(elemCod: number): Observable<BaseCodeDescr[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any[]>(
          "MetaschemaNG/LeggiUnitaMisuraCategoria", {Elem_Cod: elemCod} // parametri input
        ).pipe(map(result =>
            result.RispostaStringa.map(item => new BaseCodeDescr(item.UDM_Cod, item.UDM_Des))
        ));
    }

    leggiFiltroLottiCategorieMagazzino(params?: ILeggiCategorieMagazzinoImpostazioni) {
        if (!params) {
          params = this.getLeggiCategorieMagazzinoImpostazioni(
            FILTRO_CATEGORIE_MAGAZZINO,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI
          );
        }
        const alreadyLoading = this.masterService.get_isLoading().isLoading;
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
            this.linkFiltroCategorieMagazzino, params, !alreadyLoading
        ).pipe(map((risposta) =>  risposta.RispostaOK ? risposta.RispostaStringa : []));
    }

    leggiFiltroGiacenzeCategorieMagazzino(params?: ILeggiCategorieMagazzinoImpostazioni) {
        if (!params) {
          params = this.getLeggiCategorieMagazzinoImpostazioni(
            FILTRO_CATEGORIE_MAGAZZINO,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI
          );
        }
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any,any>(
            this.linkFiltroCategorieMagazzino, params
        ).pipe(map((risposta) => risposta.RispostaOK ? risposta.RispostaStringa : []));
    }

    getLeggiCategorieMagazzinoImpostazioni(
      filter: string, setting: enum_Impostazioni_Utenti,
      piva: string = "", saCod: number = 0
    ): ILeggiCategorieMagazzinoImpostazioni {
      return ({
        filter: filter,
        Elem_Cod: setting,
        // Piva: piva,
        // Sa_Cod: saCod
      });
    }

    public getRowsForImportApp(): Observable<any[]> {
        return of(Object.keys(enum_Dati_App)
          .filter(k => !isNumeric(k))
          .map(e => ({Elem_Cod: enum_Dati_App[e], NomeComune: this.transloco.translate(e)})));
    }

}
