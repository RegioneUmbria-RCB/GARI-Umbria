import { Injectable } from "@angular/core";
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from "@angular/forms";
import { enum_AnalisiTipo } from "app/Model/TipiEnumerativi";
import { AnalisiTerreno, AnalisiTipologia } from "app/Service/api.service";
import { enum_AnalisiTerrenoGridCommands } from "app/analisi-terreno/utils";

@Injectable()
export class AnalisiTerrenoFormService {

    formAnalisiTerreno = CreateFormGroup(this.fb, this.InitializeFormAnalisiTerreno());

    arrayEntita: Array<any> = [];
    arrayParametri: Array<any> = [];

    modeAgganciataPC_PUA: boolean = false;

    constructor(private fb: FormBuilder) {
    }

    InitializeFormAnalisiTerreno(vals?: Partial<AnalisiTerreno>): AnalisiTerreno {
        return {
            longitude: vals?.longitude ?? null,
            latitude: vals?.latitude ?? null,
            riferimento1: vals?.riferimento1 ?? null,
            riferimento2: vals?.riferimento2 ?? null,
            riferimento3: vals?.riferimento3 ?? null,
            riferimento4: vals?.riferimento4 ?? null,
            riferimento5: vals?.riferimento5 ?? null,
            tessitura: vals?.tessitura ?? null,
            entitaCoinvolte: vals?.entitaCoinvolte ?? [],
            entitaImprese: vals?.entitaImprese ?? [],
            entitaCentri: vals?.entitaCentri ?? [],
            entitaCampi: vals?.entitaCampi ?? [],
            entitaAppezzamenti: vals?.entitaAppezzamenti ?? [],
            entitaImpianti: vals?.entitaImpianti ?? [],
            entitaFabbricati: vals?.entitaFabbricati ?? [],
            entitaParticelleCatastali: vals?.entitaParticelleCatastali ?? [],
            campioni: vals?.campioni ?? [],
            //                             [{
            //                                 longitude: 0,
            //                                 latitude: 0,
            //                                 quantita: 0,
            //                                 unitaMisura: null,
            //                                 profondita: 0,
            //                                 profonditaMin: 0,
            //                                 profonditaMax: 0,
            //                                 riferimento1: null,
            //                                 riferimento2: null,
            //                                 riferimento3: null,
            //                                 riferimento4: null,
            //                                 riferimento5: null,
            //                                 note: null,
            //                                 dataPrelievo: null,
            //                                 KeyPiva: null,
            //                                 KeySaCod: null,
            //                                 KeyGrafica: null,
            //                                 particella: null,
            //                                 codice: 0,
            //                                 descrizione: null
            // }],
            flag_cancellazione: vals?.flag_cancellazione ?? false,
            utente_ultima_modifica: vals?.utente_ultima_modifica ?? null,
            validita: {
                inizio: vals?.validita?.inizio ?? new Date(new Date().getFullYear(), 0, 1),
                fine: vals?.validita?.fine ?? null,
            },
            note: vals?.note ?? null,
            certificatoAnalisi: { 
                                    codice: vals?.certificatoAnalisi?.codice ?? null,
                                    numero_certificato: vals?.certificatoAnalisi?.numero_certificato ?? null
                                },
            laboratorio: vals?.laboratorio ?? { ragione_Sociale: '', 
                                                risorseUmane: [{ codice: -1 }]},
            analisiTipologia: vals?.analisiTipologia ?? null,
            dettagli: vals?.dettagli ?? [],
            AnalisiTipo: vals?.AnalisiTipo ?? {codice: enum_AnalisiTipo.Analisi_Terreno, descrizione: ''},
            codice: vals?.codice ?? 0,
            descrizione: vals?.descrizione ?? ''
        };
    }

    public fillCampioniArray(analisi: AnalisiTerreno, typeCommand: enum_AnalisiTerrenoGridCommands) {

        let campioniArray = this.formAnalisiTerreno.get('campioni') as FormArray;
        
        if (analisi.campioni && analisi.campioni.length > 0) {

            campioniArray.clear();

            analisi.campioni.forEach(item => {
                let itemCampione = this.fb.group({
                                                    longitude: item.longitude,
                                                    latitude: item.latitude,
                                                    quantita: item.quantita,
                                                    unitaMisura: item.unitaMisura,
                                                    profondita: item.profondita,
                                                    profonditaMin: item.profonditaMin,
                                                    profonditaMax: item.profonditaMax,
                                                    riferimento1: item.riferimento1,
                                                    riferimento2: item.riferimento2,
                                                    riferimento3: item.riferimento3,
                                                    riferimento4: item.riferimento4,
                                                    riferimento5: item.riferimento5,
                                                    note: item.note,
                                                    dataPrelievo: [item.dataPrelievo, Validators.required],
                                                    KeyPiva: item.KeyPiva,
                                                    KeySaCod: item.KeySaCod,
                                                    KeyGrafica: item.KeyGrafica,
                                                    particella: item.particella ?? null,
                                                    codice: item.codice,
                                                    descrizione: item.descrizione
                });

                if (typeCommand == enum_AnalisiTerrenoGridCommands.INFO)
                    itemCampione.disable();

                campioniArray.push(itemCampione);
            });

        } else {
            campioniArray.clear();
        }

    }

}

function CreateFormGroup(fb: FormBuilder, mask: AnalisiTerreno) {

   return fb.group({
                    longitude: new FormControl(mask.longitude),
                    latitude: new FormControl(mask.latitude),
                    riferimento1: new FormControl(mask.riferimento1),
                    riferimento2: new FormControl(mask.riferimento2),
                    riferimento3: new FormControl(mask.riferimento3),
                    riferimento4: new FormControl(mask.riferimento4),
                    riferimento5: new FormControl(mask.riferimento5),
                    tessitura: new FormControl(mask.tessitura),
                    entitaCoinvolte: new FormControl(mask.entitaCoinvolte, Validators.required),
                    entitaImprese: new FormControl(mask.entitaImprese),
                    entitaCentri: new FormControl(mask.entitaCentri),
                    entitaCampi: new FormControl(mask.entitaCampi),
                    entitaAppezzamenti: new FormControl(mask.entitaAppezzamenti),
                    entitaImpianti: new FormControl(mask.entitaImpianti),
                    entitaFabbricati: new FormControl(mask.entitaFabbricati),
                    entitaParticelleCatastali: new FormControl(mask.entitaParticelleCatastali),
                    campioni: new FormArray([fb.group(
                        {
                            longitude: [0],
                            latitude: [0],
                            quantita: [0],
                            unitaMisura: [null],
                            profondita: [0],
                            profonditaMin: [0],
                            profonditaMax: [0],
                            riferimento1: [null],
                            riferimento2: [null],
                            riferimento3: [null],
                            riferimento4: [null],
                            riferimento5: [null],
                            note: [null],
                            dataPrelievo: [null],
                            KeyPiva: [null],
                            KeySaCod: [null],
                            KeyGrafica: [null],
                            particella: [null],
                            codice: [0],
                            descrizione: [null]
                        }
                    )]),
                    flag_cancellazione: new FormControl(mask.flag_cancellazione),
                    utente_ultima_modifica: new FormControl(mask.utente_ultima_modifica),
                    validita: fb.group({ 
                                        inizio: new FormControl(mask.validita.inizio),
                                        fine: new FormControl(mask.validita.fine)
                    }),
                    note: new FormControl(mask.note),
                    certificatoAnalisi: fb.group({ 
                                                    codice: new FormControl(mask.certificatoAnalisi.codice),
                                                    numero_certificato: new FormControl(mask.certificatoAnalisi.numero_certificato)
                    }),
                    laboratorio: new FormControl(mask.laboratorio),
                    analisiTipologia: new FormControl(mask.analisiTipologia),
                    dettagli: new FormControl(mask.dettagli),
                    AnalisiTipo: new FormControl(mask.AnalisiTipo),
                    codice: new FormControl(mask.codice),
                    descrizione: new FormControl(mask.descrizione, Validators.required)
   });
}