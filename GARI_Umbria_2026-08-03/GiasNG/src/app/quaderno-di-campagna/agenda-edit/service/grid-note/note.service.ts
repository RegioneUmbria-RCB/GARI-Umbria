import {Injectable} from '@angular/core';
import { Tipo_Attivita} from 'gias-ui-kit';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {BaseCodeDescr} from 'app/Model/baseClass/baseCodeDescr';
import {Specie} from 'app/Model/metaschema/utilizzi/Specie';
import {AjaxAgronicaAPIService} from 'app/Service/ajax-agronica.api.service';
import {ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {KendoGridRow} from 'gias-kendo-grid';
import {BehaviorSubject, map, Observable, take} from 'rxjs';
import {GruppoNoteModel, NotaInterventoDdlItem} from '../../componenti/grid-note/note.model';
import {QdCService} from '../qdc.service';
import {GiasDialogService} from "../../../../Service/gias-dialog.service";

export class LeggiNote {
    tipoAttivita: Tipo_Attivita;
    lavorazioni: Lavorazione = new Lavorazione('0');
    specieVegetale: Specie = new Specie(0);
    piva: string;
    parametriAggiuntivi: Array<{item1: string; item2: string}> = [];
}

const idConsigli = "idConsigli";
export enum Enum_Tipo_Gruppo_Note {
    Multi = 0, Singolo = 1
}

@Injectable()
export class NoteService {

    public foundGroups: BehaviorSubject<GruppoNoteModel[]> = new BehaviorSubject(null);
    public defaultNoteSelezionate: NotaInterventoDdlItem[] = [
        { id: 1, descrizione: 'Non trattare in caso di pioggia', NotaGruppo_Cod: 1 },
        { id: 2, descrizione: 'Curare bene  la bagnatura', NotaGruppo_Cod: 1 },
        { id: -2, descrizione: 'Sereno', NotaGruppo_Cod: -1 },
        { id: 29, descrizione: 'Punto di attingimento Pozzo-Canale', NotaGruppo_Cod: 6 },
        { id: 123, descrizione: 'abc', NotaGruppo_Cod: -20}
    ];
    public isUsingPresets = true;
    public lastLeggiNote: LeggiNote = null;

    private Id_Agenda = "0";
    private defaultGruppiRadio = [
        "Meteo", "Orario",
    ];

    constructor(
        private qdcService: QdCService,
        private agendaPrams: ObjParametriAgendaService,
        private apiService: AjaxAgronicaAPIService,
        private dialog: GiasDialogService
    ) {
        //TODO Multicentro capire come fare
            if(this.qdcService.TestataForm.get('Codici_Attivita').value && this.qdcService.TestataForm.get('Codici_Attivita').value.length > 0)
                this.Id_Agenda = this.qdcService.TestataForm.get('Codici_Attivita').value[0].CodiceAttivita;
    }

    /**
     * Carica da server le note visibili relative a un gruppo specifico.
     *
     * @param row la riga della griglia di cui si vogliono prendere le note
     * @returns una promessa con la lista di note possibili per la riga indicata
     */
    public leggiNote(row: KendoGridRow | GruppoNoteModel, p: LeggiNote): Observable<NotaInterventoDdlItem[]> {
        let currRow = row as GruppoNoteModel;
        return this.apiService.ajaxAPIPost<LeggiNote, any[]>(
          'Agenda/LeggiNote', p
        ).pipe(
          take(1),
          map(result => result.RispostaOK ? result.RispostaStringa as any[] : []),
          map((R: any[]) => R.filter(nota =>
              currRow.Gruppo_Cod.includes(nota.noteInterventoGruppi.codice)
            ).map(nota => new NotaInterventoDdlItem(
                nota.codice,
                nota.descrizione,
                nota.noteInterventoGruppi.codice
            ))
          )
        );
    }


    /**
     * Carica da server i gruppi visibili.
     * @returns una promessa con la lista dei gruppi visibili
     */
    public leggiGruppi(p: LeggiNote): Observable<GruppoNoteModel[]> {
        return this.apiService.ajaxAPIPost<LeggiNote, any[]>(
            'Agenda/LeggiGruppi', p
        ).pipe(
          map(R => R.RispostaOK ? R.RispostaStringa as any[] : []),
          map(R => R.map((gr, id) => {
                let group = new GruppoNoteModel(
                    id.toString(), gr.codice, gr.descrizione,
                    (gr.tipo === Enum_Tipo_Gruppo_Note.Singolo)
                );
                if (gr.presets.length) group.Defaults = gr.presets;
                return group;
          }))
        );
    }

    // private peekGruppi(ris: any) {
    //     let note: GruppoNoteModel[] = [];
    //     let id = 0;
    //     ris.forEach(nota => {
    //         let gruppo = note.find(gr => gr.Gruppo_Cod === nota.noteInterventoGruppi.codice)
    //         if (gruppo === undefined) {
    //             gruppo = new GruppoNoteModel(
    //                 id.toString(),
    //                 nota.noteInterventoGruppi.codice,
    //                 nota.noteInterventoGruppi.descrizione
    //             );
    //             note.push(gruppo);
    //             id++;
    //         }
    //         gruppo.Nota_Cod.push(nota.codice);
    //         gruppo.Nota_Des.push(nota.descrizione);
    //     })
    //     return note;
    // }

    /**
     * Carica le note selezionate al caricamento della pagina,
     * smistandole nel gruppo di appartenenza.
     *
     * @param noteAttive le note già selezionate da mostrare in tabella
     * @returns un array con i gruppi note e i valori associati
     */
    public caricaNoteIniziali(noteAttive: NotaInterventoDdlItem[]): Observable<GruppoNoteModel[]> {
        const p = this.getLeggiNote();
        return this.leggiGruppi(p)
            .pipe(map(gruppi => {
                gruppi.unshift(new GruppoNoteModel(idConsigli, 0, "Gruppi non visibili"));
                let usePreset = !this.isSameAsLast(p) && this.isUsingPresets;
                this.attivaNote(noteAttive, gruppi, usePreset);
                // Se il gruppo consigli è vuoto lo rimuovo
                if (gruppi.at(0).Nota_Cod.length === 0) {
                  gruppi.splice(0, 1);
                }
                this.lastLeggiNote = p;
                this.foundGroups.next(gruppi);
                return gruppi || [];
            }));
    }

  /**
   * @param listaDefault i codice delle note da impostare come default
   */
  public saveDefaults(listaDefault: number[]) {
        const parametri = this.getLeggiNote();
        parametri.parametriAggiuntivi = listaDefault.map(nota => ({
            item1: "nota_cod",
            item2: nota.toString()
        }));
        this.apiService.ajaxAPIPost<LeggiNote, any>('Agenda/AggiornaDefaultNote', parametri)
            .pipe(take(1), map(R => R.RispostaOK))
            .subscribe(ok => {
              if (ok) void this.dialog.salvataggioOk();
              else this.dialog.baseError("_Errore", "SiÈVerificatoUnErroreDuranteLaFaseDiSalvat");
            });
    }

    /**
     * @param showHidden se true, mostra anche le note non visibili (usato per la gestione delle note)
     * @returns un set di parametri standard per la lettura delle note
     */
    public getLeggiNote(showHidden: boolean = false): LeggiNote {
        let params = new LeggiNote();
        params.tipoAttivita = this.qdcService.TestataForm.get("Tipo").value;
        params.lavorazioni = this.qdcService.TestataForm.get("Operazioni").value;
        params.specieVegetale = this.qdcService.TestataForm.get('Specie').value;
        params.piva = this.agendaPrams.getObjParamValue().Piva;
        if (showHidden) {
          params.parametriAggiuntivi.push({item1: 'showHidden', item2: 'true'});
        }
        return params;
    }

    private isSameAsLast(newData: LeggiNote): boolean {
        if (!this.lastLeggiNote) return false;
        return JSON.stringify(newData) === JSON.stringify(this.lastLeggiNote);
    }

  /**
   * Attiva le note selezionate dall'utente (pesenti nel form) o i preset se le condizioni sono soddisfatte.
   * @private
   * @history
   * - (< 22/08/2024): Condizione per l'applicazione dei preset era `this.Id_Agenda === '0' && !note.length`.
   * Dava problemi in chiusura/riapertura della scheda note se volevo lasciarle vuote.
   * - (22/08/2024): aggiornata la condizione per l'uso dei preset: aggiunto paramento in input.
   * Nel chiamante il nuovo parametro è da valorizzare tenendo conto del fatto che le note siano state toccate o meno
   * si stanno caricando le note per una stessa casistica due volte di fila o meno.
   */
    private attivaNote(note: NotaInterventoDdlItem[], gruppi: GruppoNoteModel[], usePresets: boolean = true) {
        if (this.Id_Agenda === '0' && usePresets) {
            this.usePresets(gruppi);
            this.isUsingPresets = true;
            this.qdcService.AggiornaFormArrayNote();
            return;
        }
        note.forEach(nota => {
            let gruppo = gruppi.find(gr => gr.Gruppo_Cod.includes(nota.NotaGruppo_Cod));
            if (gruppo === undefined) {
                gruppo = gruppi.find(gr => gr.id === idConsigli);
                gruppo.Gruppo_Cod.push(nota.NotaGruppo_Cod);
            }
            gruppo.Nota_Cod.push(nota.id);
            gruppo.Nota_Des.push(nota.descrizione);
        });
    }

    private usePresets(gruppi: GruppoNoteModel[]) {
        gruppi.filter(gr => gr.Defaults.length)
            .forEach(group =>
                group.Defaults.forEach((pre: BaseCodeDescr) => {
                    group.Nota_Cod.push(pre.codice);
                    group.Nota_Des.push(pre.descrizione);
                })
            );
    }

}
