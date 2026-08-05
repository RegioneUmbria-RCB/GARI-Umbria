import {Injectable} from "@angular/core";
import {DettaglioTrattamento} from "../../Model/attivita/dettagli/DettaglioTrattamento";
import {AvversitaGruppo} from "../../Model/metaschema/avversita/AvversitaGruppo";
import {UtilizzoTerreno} from "../../Model/metaschema/utilizzi/UtilizzoTerreno";
import {AjaxAgronicaAPIService} from "../ajax-agronica.api.service";
import {Disciplinare} from "../../Model/metaschema/Disciplinari";



export class CheckTrattamento{
  dettaglioTrattamento: DettaglioTrattamento;
  avversitaGruppo: AvversitaGruppo;
  utilizzoTerreno: UtilizzoTerreno;
  disciplinare: Disciplinare;
}
@Injectable({
  providedIn: 'root'
})
export class ChatGPTService {

  constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService){}

  public CheckTrattamento(checkTrattamento: CheckTrattamento){
    const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<CheckTrattamento,string>('AgronicaChatGPT/CheckTrattamento',checkTrattamento,true);

    return Obs;
  }

}
