import {Component, Inject, LOCALE_ID, OnDestroy, OnInit, ViewEncapsulation} from '@angular/core';
import {generateGridProviders} from "gias-kendo-grid";
import {GridTrappoleService} from "./grid-trappole.service";
import {TranslocoService} from "@jsverse/transloco";
import {DatePipe} from "@angular/common";
import {Enum_DBTypeOperation} from "gias-ui-kit";
import {Gias2010Redirector} from "../../menu-agenda/components/grid-qdc/Gias2010Redirector.service";
import {LeggiOperazioni_IN, OperazioneClient} from "../../Service/net-core6-api.service";
import {enum_LAVCOD} from "../../Model/TipiEnumerativi";
import {RibaltamentoTypes, TabTypes} from "../../menu-agenda/components/utils";
import {GiasDialogService} from "../../Service/gias-dialog.service";

@Component({
  standalone: false,
  selector: 'grid-trappole',
  templateUrl: './grid-trappole.component.html',
  styleUrl: './grid-trappole.component.scss',
  providers: [
    ...generateGridProviders(GridTrappoleService, GridTrappoleComponent)
  ],
  encapsulation: ViewEncapsulation.None
})
export class GridTrappoleComponent implements OnDestroy,OnInit {

  constructor(private translocoService: TranslocoService,
              private datepipe: DatePipe,
              @Inject(LOCALE_ID) public locale_id: string,
              private gias2010Redirector: Gias2010Redirector,
              private operazioneService: OperazioneClient,
              private giasdialogService: GiasDialogService) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  public GetTitleInneschiInScadenza(): string{

    let data_str = this.datepipe.transform(new Date(),'dd/MM/yyyy',undefined,this.locale_id);

    return this.translocoService.translate("InneschiInScadenza",{ Data: data_str});
  }

  public GetTitleInneschiScaduti(): string{

    let data_str = this.datepipe.transform(new Date(),'dd/MM/yyyy',undefined,this.locale_id);

    return this.translocoService.translate("InneschiScaduti",{ Data: data_str});
  }

  public reindirizzaReinnesco(event: any,dataItem: any){

    let Lav_Cod: number = enum_LAVCOD.REINNESCO_TRAPPOLE;

    let param = {
      GruppoOperazioni: [],
      Operazioni: [Lav_Cod]
    } as LeggiOperazioni_IN;

    this.operazioneService.operazioneGetOperazioni(param).subscribe(r => {

      let risp = JSON.parse(r.RispostaStringa);

      let operazioni = risp.DataTable;

      let visibilitaApplicata:boolean = risp.VisibilitaApplicata;

      if(operazioni && operazioni.length === 1){

        this.gias2010Redirector.redirectToQdCfromMenuAgenda(dataItem.Id_Agenda,0,dataItem.Piva,dataItem.Rag_Soc,
          Lav_Cod,operazioni[0].LAV_DES,0,Enum_DBTypeOperation.Write,
          TabTypes.GestioneTrappole, dataItem.Data_Movimento,
          RibaltamentoTypes.Nessuno,0,true);

      }else{
        if(visibilitaApplicata)
          this.giasdialogService.baseError("","UtenteNonEffetuaReinnesco");
      }
    });
  }

}
