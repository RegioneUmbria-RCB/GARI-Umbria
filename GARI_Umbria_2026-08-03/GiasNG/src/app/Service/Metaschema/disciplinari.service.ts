import { Injectable } from '@angular/core';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { ImpegniAggiuntiviFacoltativi } from 'app/Model/metaschema/ImpegniAggiuntiviFacoltativi';
import { Macrouso } from 'app/Model/metaschema/Macrouso';
import { Regolamenti } from 'app/Model/metaschema/Regolamenti';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { resolve } from 'dns';
import { from, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard } from '../master.service';
import { LeggiDisciplinari } from '../DPI/dpi.service';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';

export class LeggiDisciplinare {
    constructor(
        public lavorazione?: Lavorazione,
        public specie?: Specie,
        public data?: Date,
        public privato?: boolean,
        public regolamento?: Regolamenti,
        public validita?: IntervalloTemporale,
        public IncludiNessunDisciplinare?: boolean,
        public IncludiBiologico?: boolean
    ) { }
}

export class LeggiIAF {
    disciplinare: Disciplinare;
    specie: Specie;
    data: Date;
    privato: boolean;
}


@Injectable({ providedIn: 'root' })
export class DisciplinariService {

    constructor(
        private ajaxAgronicaService: AjaxAgronicaService,
        private APIService: AjaxAgronicaAPIService,
        private masterService: MasterService
    ) { }

    leggi(leggiDisciplinari: Partial<LeggiDisciplinare>): Observable<Disciplinare[]> {
        return (this.APIService.ajaxAPIPost<object, Disciplinare[]>(
            "AgronicaCoreDPING/CaricaComboDisciplinareModello", leggiDisciplinari, false
        ).pipe(map((risp) => (risp.RispostaStringa))));
    }

    leggiIAF(leggiDisciplinari: LeggiIAF): Observable<ImpegniAggiuntiviFacoltativi[]> {
        if (leggiDisciplinari.specie.codice == 0 || leggiDisciplinari.disciplinare.codice == '0') {
            of([]);
        }
        return (this.APIService.ajaxAPIPost<object, ImpegniAggiuntiviFacoltativi[]>(
            "AgronicaCoreDPING/CaricaComboIAFModello", leggiDisciplinari, false
        ).pipe(map((risp) => (risp.RispostaStringa))));
    }

}
