import { Injectable } from "@angular/core";
import { AggiornaOreMinuti_In, CaratteristicheMacchina_In, DefaultGenerali_In, DefaultGeneraliColtura_In, DefaultPianiColturaliClient, DistintaProduzione_In, MetaschemaClient, NoteClient, ProfilazioneImpreseClient, ProfilazioneMacchineClient, SalvaGruppoNoteCompleto_In, SalvaNote_In, SalvaProfilazione_In, SpecieVegetali_In } from "app/Service/net-core6-api.service";
import { BehaviorSubject, catchError, map, Observable, of, Subject } from "rxjs";

@Injectable()
export class ProfilazioneImpreseService {
  globalSubject = new BehaviorSubject<boolean>(true);
  reloadData = new Subject<void>();
  defaultPianiColturaliSpecieSubject = new BehaviorSubject<number>(0);

  constructor(
    private metaschemaClient: MetaschemaClient,
    private profilazioneImpreseClient: ProfilazioneImpreseClient,
    private profilazioneMacchineClient: ProfilazioneMacchineClient,
    private noteClient: NoteClient,
    private defaultPianiColturaliClient: DefaultPianiColturaliClient
  ) { }

  public getSpecieGlobali(): Observable<Specie[]> {
    return this.metaschemaClient
      .metaschemaGetSpecieFiltroUtente({ Gru_Cod: 0, Veg_Cod: 0 })
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa)?.DataTable ?? []) as Specie[]),
        catchError(() => of([]))
      );
  }

  public getSpecieAziendali(piva: string): Observable<Specie[]> {
    return this.metaschemaClient
      .metaschemaLeggiSpecieAziendali(piva)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as Specie[]),
        catchError(() => of([]))
      );
  }

  public getOperazioni(): Observable<Lavorazione[]> {
    return this.metaschemaClient
      .metaschemaGetOperazioniFiltroUtente({ GruppoOperazioni: [], Operazioni: [] })
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa)?.DataTable ?? []) as Lavorazione[]),
        catchError(() => of([]))
      );
  }

  public getGruppiOperazione(): Observable<GruppoOperazione[]> {
    return this.metaschemaClient
      .metaschemaGetGruppiOperazione()
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as GruppoOperazioneBackend[]),
        map(types => types
          .map(x => x.Tipo)
          .filter((v, i, arr) => arr.indexOf(v) === i)
          .map(x => ProfilazioneImpreseService.getDescrizioneFromTipoGruppoOperazione(x))
          .filter(x => x != null)
        ),
        catchError(() => of([]))
      );
  }

  public getLavorazioniPerTipo(tipo: string): Observable<Lavorazione[]> {
    return this.metaschemaClient
      .metaschemaLeggiOperazioniPerTipo(tipo)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as Lavorazione[]),
        catchError(() => of([]))
      );
  }

  public leggiParcoMacchine(piva: string): Observable<Macchina[]> {
    return this.metaschemaClient
      .metaschemaLeggiParcoMacchine(piva)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as Macchina[]),
        catchError(() => of([]))
      );
  }

  public leggiContatti(piva: string): Observable<Contatto[]> {
    return this.metaschemaClient
      .metaschemaLeggiContatti(piva)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as Contatto[]),
        catchError(() => of([]))
      );
  }

  public salvaImpresa(body: SalvaProfilazione_In): Observable<boolean> {
    return this.profilazioneImpreseClient
      .profilazioneImpreseSalvaProfilazione(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public leggiProfilazioneMacchineXContatti(piva: string, vegCod: number): Observable<MacchineXContatti[]> {
    return this.profilazioneImpreseClient
      .profilazioneImpreseLeggiProfilazioneMacchineXContatti(piva, vegCod)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as MacchineXContatti[]),
        catchError(() => of([]))
      );
  }

  public eliminaProfilazione(idGruppo: string, notaUtilizzoCod: number, lavCod: number, vegCod: number, piva: string): Observable<boolean> {
    return this.profilazioneImpreseClient
      .profilazioneImpreseEliminaProfilazione(idGruppo, notaUtilizzoCod, lavCod, vegCod, piva)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public propagaSuTutteLeLavorazioni(lavCod: number, vegCod: number, piva: string): Observable<boolean> {
    return this.profilazioneImpreseClient
      .profilazioneImpresePropagaSuTutteLeLavorazioni(lavCod, vegCod, piva)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public aggiornaOreMinuti(body: AggiornaOreMinuti_In): Observable<boolean> {
    return this.profilazioneImpreseClient
      .profilazioneImpreseAggiornaOreMinuti(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public aggiornaMacchinaCaratteristiche(body: CaratteristicheMacchina_In): Observable<boolean> {
    return this.profilazioneMacchineClient
      .profilazioneMacchineUpdateCaratteristicheMacchina(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public leggiMacchine(idProfilazione: number): Observable<MacchinaDettaglio[]> {
    return this.profilazioneMacchineClient
      .profilazioneMacchineLeggi(idProfilazione)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as MacchinaDettaglio[]),
        catchError(() => of([]))
      );
  }

  public leggiNoteIntervento(): Observable<NoteIntervento[]> {
    return this.noteClient
      .noteLeggiNoteInterventoUtilizzo()
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as NoteIntervento[]),
        catchError(() => of([]))
      );
  }

  public leggiProfilazioneNote(notaUtilizzoCod: number): Observable<NotaProfilazione[]> {
    return this.noteClient
      .noteLeggiProfilazioneNote(notaUtilizzoCod)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as NotaProfilazione[]),
        catchError(() => of([]))
      );
  }

  public salvaNote(body: SalvaNote_In): Observable<boolean> {
    return this.noteClient
      .noteSalvaNote(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public leggiNote(vegCod: number, lavCod: number, piva: string): Observable<Nota[]> {
    return this.noteClient
      .noteLeggiNote(vegCod, lavCod, piva)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as Nota[]),
        catchError(() => of([]))
      );
  }

  public leggiGruppoNota(): Observable<GruppoNota[]> {
    return this.noteClient
      .noteLeggiGruppoNote()
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as GruppoNota[]),
        catchError(() => of([]))
      );
  }

  public leggiNotePerGruppoNota(notaGruppoCod: number): Observable<NotaXGruppo[]> {
    return this.noteClient
      .noteLeggiNoteXUtilizzo(0, notaGruppoCod)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as NotaXGruppo[]),
        catchError(() => of([]))
      );
  }

  public salvaGruppoNote(newGroupDialogDes: string): Observable<boolean> {
    return this.noteClient
      .noteSalvaGruppoNote({ NotaGruppoCod: 0, NotaGruppoDes: newGroupDialogDes })
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public aggiornaGruppoNote(groupDialog: GruppoNota): Observable<boolean> {
    return this.noteClient
      .noteAggiornaGruppoNote({ NotaGruppoCod: groupDialog.NotaGruppo_Cod, NotaGruppoDes: groupDialog.NotaGruppo_Des })
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public cancellaGruppoNote(notaGruppoCod: number): Observable<boolean> {
    return this.noteClient
      .noteCancellaGruppoNote(notaGruppoCod)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public aggiungiNota(notaDes: string, gruppoNotaCod: number): Observable<boolean> {
    return this.noteClient
      .noteAggiungiNota(notaDes, gruppoNotaCod)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }
  
  public cancellaNota(notaCod: number): Observable<boolean> {
    return this.noteClient
      .noteCancellaNota(notaCod)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public leggiNoteInterventoUtilizzoGruppi(notaGruppoCod: number): Observable<NotaXGruppo[]> {
    return this.noteClient
      .noteLeggiNoteInterventoUtilizzoGruppi(notaGruppoCod, 0)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as NotaXGruppo[]),
        catchError(() => of([]))
      );
  }

  public salvaGruppoNoteCompleto(body: SalvaGruppoNoteCompleto_In): Observable<boolean> {
    return this.noteClient
      .noteSalvaGruppoNoteCompleto(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public getAllVarieta(specie: number): Observable<Varieta[]> {
    return this.metaschemaClient
      .metaschemaLeggiVarieta(0, specie)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as Varieta[]),
        catchError(() => of([]))
      );
  }

  public getFilteredVarieta(piva: string, specie: number): Observable<Varieta[]> {
    return this.metaschemaClient
      .metaschemaLeggiVarietaFiltered(piva, 0, specie)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as Varieta[]),
        catchError(() => of([]))
      );
  }

  public getDistintaDiProduzione(piva: string, vegCod: number): Observable<DefaultDistintaProduzione> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliLeggiDefaultDistintaDiProduzione(piva, vegCod)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? {}) as DefaultDistintaProduzione),
        catchError(() => of({} as DefaultDistintaProduzione))
      );
  }

  public getDefaultGeneraleSpecie(piva: string): Observable<DefaultSpecie[]> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliLeggiDefaultGeneraleSpecie(piva)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as DefaultSpecie[]),
        catchError(() => of([]))
      );
  }

  public scriviDefaultSpecie(body: SpecieVegetali_In): Observable<boolean> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliScriviDefaultSpecie(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public cancellaDefaultSpecie(piva: string | undefined, vegCod: number): Observable<boolean> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliCancellaDefaultSpecie(piva, vegCod)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public scriviParametriGenerali(body: DefaultGeneraliColtura_In): Observable<boolean> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliScriviDefaultGenerali(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public getParametriGeneraliColtura(piva: string): Observable<DefaultGenerale[]> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliLeggiDefaultGenerali(piva)
      .pipe(
        map(x => (JSON.parse(x.RispostaStringa) ?? []) as DefaultGenerale[]),
        catchError(() => of([]))
      );
  }
  public scriviDefaultGenerale(body: DefaultGenerali_In): Observable<boolean> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliAggiornaDefaultGenerali(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public cancellaDefaultGenerale(piva: string, vegCod: number, culCod: number): Observable<boolean> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliCancellaDefaultGenerali(piva, vegCod, culCod)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  public salvaDefaultDistintaDiProduzione(body: DistintaProduzione_In): Observable<boolean> {
    return this.defaultPianiColturaliClient
      .defaultPianiColturaliSalvaDefaultDistintaDiProduzione(body)
      .pipe(
        map(x => x.RispostaStringa == "true"),
        catchError(() => of(false))
      );
  }

  private static getDescrizioneFromTipoGruppoOperazione(tipoGruppoOperazione: string): GruppoOperazione | null {
    switch (tipoGruppoOperazione) {
      case "C":
        return { text: "Colturali", value: tipoGruppoOperazione };
      case "E":
        return { text: "Economiche", value: tipoGruppoOperazione };
      case "P":
        return { text: "Parco macchine", value: tipoGruppoOperazione };
      case "V":
        return { text: "Visite ispettive", value: tipoGruppoOperazione };
      case "Z":
        return { text: "Zootecniche", value: tipoGruppoOperazione };
      default:
        return null;
    }
  }
}

export interface Specie {
  Veg_Cod: number;
  Veg_Des: string;
}

export interface Lavorazione {
  LAV_COD: number;
  LAV_DES: string;
  GRU_OP: number;
  GRU_COD: number;
  GRU_DES: string;
}
export interface GruppoOperazioneBackend {
  GRU_COD: number;
  GRU_DES: string;
  Tipo: string;
}

export interface GruppoOperazione {
  text: string;
  value: string;
}

export interface Macchina {
  Mac_Cod: number;
  Mac_Des: string;
  Ditta_Des: string;
  Modello: string;
  Referente: string;
}

export interface Contatto {
  Cod_RisUm: number;
  Nome: string;
  Cognome: string;
  Rapporto_Des: string;
  description: string
}

export interface MacchineXContatti {
  Id_Profilo_Dati: number;
  lav_cod: number;
  lav_des: string;
  veg_cod: number;
  Veg_des: string;
  mac_des: string;
  mac_cods: string;
  cont_des: string;
  cont_cods: string;
  ore: number;
  minuti: number;
}

export interface MacchinaDettaglio {
  Id_Profilo_Dati: number;
  mac_cod: number;
  class_code: string;
  mac_des: string;
  dettagli: string;
  caratteristiche: string;
  elenco_mac_car: string;
}

export interface NoteIntervento {
  NotaUtilizzo_Cod: number;
  NotaUtilizzo_Des: string;
}

export interface NotaProfilazione {
  Nota_Cod: number;
  NotaGruppo_Des: string;
  Nota_Des: string;
}

export class Nota {
  notautilizzo_des: string;
  notautilizzo_cod: number;
  nota_des: string;
  nota_cods: string;
  Lav_cod: number;
  Lav_des: string;
  Veg_cod: number;
  Veg_des: string;
}

export class GruppoNota {
  NotaGruppo_Cod: number;
  NotaGruppo_Des: string;
  visibile: boolean
}

export class NotaXGruppo {
  Nota_Cod: number;
  Nota_Des: string;
  NotaGruppo_Cod: number;
  NotaGruppo_Des: string;
  NotaUtilizzo_Cod: number;
  NotaUtilizzo_Des: string;
  Visibile: boolean;
  VisibileGruppo: boolean;
}

export class Varieta {
  Cul_Cod: number;
  Cul_Des: string;
}

export class DefaultSpecie {
  Veg_Cod: number;
  Cul_Cod: number;
  Veg_Des: string;
  Cul_des: string;
  tipo_maturazione: string;
  Soglia_Minima: number;
  resa_stabilimento: number;
  peso_sgocciolato: number;
}

export class TipoMaturazione {
  text: string;
  value: string
}

export class DefaultGenerale {
  Veg_Cod: number;
  Cul_Cod: number;
  Veg_Des: string;
  Cul_des: string;
  unita_calore: number;
  gg: number;
  resa: number;
  dosi_ha: number;
  dosi_ha_udm: number;
}

export class DefaultDistintaProduzione {
  GruCod: number;
  NCicli: number;
  Data: Object[];
}