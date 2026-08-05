import { FiltriAnalisiConformita, FiltriAnalisiConformitaDes } from "./filtri-nuova-analisi-conformita.model";
import { LeggiAgendaHubQDCNew, AnalysisRequest, AnalysisRequest_Status } from "app/Service/qdca-compliance-api.service";
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';

export class AnalysisRequestDataItem implements AnalysisRequest {
  IdTestata: number;
  IntervalloInizio: Date;
  IntervalloFine: Date;
  DataRichiesta: Date;
  Piva: string;
  RagSoc: string;
  SaCod: number;
  SaNome: string;
  VegCod: number;
  VegDes: string;
  Impianti: string;
  ImpiantiDes: string;
  Operazioni: string;
  OperazioniDes: string;
  Dpi: string;
  DpiDes: string;
  IAF: boolean;
  UserSettings: boolean;
  Storage: boolean;
  Regulations: boolean;
  RequestStatus: AnalysisRequest_Status;
  Status: string;
  ControlloRiduzioneDiserbo?: boolean;
  Origin: number;
  OriginDes: string = "";
  UsernameCreazione: string;
  StorageCompliance: number;
  StorageComplianceStr: string;
  RegulationCompliance: number;
  RegulationComplianceStr: string;

  public static FromFilters(filters: FiltriAnalisiConformita) {
    const res = new AnalysisRequestDataItem();
    res.IntervalloInizio = filters.data_da;
    res.IntervalloFine = filters.data_a;
    res.DataRichiesta = filters.data_richiesta;
    res.Piva = filters.piva;
    res.RagSoc = filters['rag_soc'] ?? '';
    res.SaCod = filters.sa_cod.length > 0 ? filters.sa_cod[0] : 0;
    res.SaNome = filters['sa_nome'] ?? '';
    res.VegCod = filters.veg_cod.length > 0 ? filters.veg_cod[0] : 0;
    res.VegDes = filters['veg_des'] ?? '';
    res.Impianti = filters.impianti.join(",");
    res.ImpiantiDes = filters['impianti_des'] ?? '';
    res.Operazioni = filters.operazioni.join(",");
    res.OperazioniDes = filters['operazioni_des'] ?? '';
    res.Dpi = filters.dpi ? filters.dpi : '0';
    res.DpiDes = filters['dpi_des'] ?? '';
    res.IAF = filters.flagIaf;
    res.UserSettings = filters.flagImpostazioni;
    res.Storage = filters.flagMagazzino;
    return res;
  }

  public static toLeggiAgendaHubQDCNew(toParse: AnalysisRequestDataItem): LeggiAgendaHubQDCNew {
    // Helper to parse a delimited string of numbers (e.g., "1, 2;3  4")
    const parseNumberList = (str?: string | null): number[] | null => {
      if (!str) return null;
      const nums = str
        .split(/[,;|\s]+/g)
        .map(x => x.trim())
        .filter(x => x.length > 0)
        .map(x => Number(x))
        .filter(x => Number.isFinite(x));
      return nums.length > 0 ? nums : [0];
    };
    return {
      idTestata: toParse.IdTestata,
      piva: toParse.Piva,
      saCod: Number.isFinite(toParse.SaCod) ? [toParse.SaCod] : [0],
      specieVegetale: Number.isFinite(toParse.VegCod) ? [toParse.VegCod] : [0],
      operazioni: parseNumberList(toParse.Operazioni),
      intervallo: new IntervalloTemporale(toParse.IntervalloInizio, toParse.IntervalloFine),
      verificaIAF: toParse.IAF ?? false,
      verificaSoloControlliUtente: toParse.UserSettings ?? false,
      verificaMagazzino: toParse.Storage ?? false
    };
  }

}