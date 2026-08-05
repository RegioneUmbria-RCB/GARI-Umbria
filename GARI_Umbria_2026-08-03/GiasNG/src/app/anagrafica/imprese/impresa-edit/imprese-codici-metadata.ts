import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { ImpresaPadre } from 'app/Model/anagrafiche/ImpresaPadre';
import { RisorseUmane } from '../../../Model/anagrafiche/RisorseUmane';
import { FormeGiuridiche } from 'app/Model/metaschema/FormeGiuridiche';
import { ObjParametriAgenda } from 'gias-ui-kit';
export class CaricaDatiDto {
    Impresa: Impresa;
    FormeGiuridiche: FormeGiuridiche[];
    Tecnici: Array<Contatto>;
    Odc: Array<RisorseUmane>;
    Padri: Array<ImpresaPadre>;
    ObjParamAgenda: ObjParametriAgenda;
}
