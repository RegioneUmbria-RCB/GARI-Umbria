//variabili aggiunte in fase di importazione libreria

var Enum_debugMode = {
    Off: { value: 0, name: "Off", code: 0 },
    Soft: { value: 1, name: "Soft", code: 1 },
    Verbose: { value: 2, name: "Verbose", code: 2 }
}

var Debug_Mode = Enum_debugMode.Off;

var enum_TipoNodo = {
    Utente: 1,
    Impresa: 2,
    Centro: 3,
    Campo: 4,
    Serra: 41,
    Appezzamento: 5,
    ImpiantoNudo: 6,
    ImpiantoArborea: 7,
    ImpiantoErbacea: 8,
    ImpiantoOrticola: 9,
    Particella: 10,
    Persona: 11,
    CatastoAziendale: 18,
    Impianto_Generico: 19,
    Fabbricato_Generico: 20,
    f_Abitazione: 21,
    f_Magazzino: 22,
    f_Silos: 23,
    f_CellaFrigorifera: 24,
    f_ImpiantoLavorazione: 25,
    f_Stalla: 26,
    f_Fienile: 33,
    p_PortafoglioProdotti: 27,
    p_Prodotto: 28,
    p_Preparazione: 29,
    x_ConsistenzeAnimali: 30,
    x_MovimentiMagazzino: 31,
    x_PreparazioniAlimentari: 32,
    x_GiacenzeMagazzino: 34,
    x_ParcoMacchine: 35,
    x_Contatti: 36,
    x_ListaFabbricatiAziendali: 37,
    x_Cooperativa: 38,
    x_Consorzio: 39,
    x_OP: 40,
    Analisi_Certificato: 42,
    Analisi_Testata: 43,
    Analisi_Dettaglio: 44,
    Analisi_Campione: 45,
    PianoConcimazione_Testata: 46,
    DistintaDiProduzione: 47,
    x_VariazioniConsistenzeAnimali: 48,
    Cantine_Piani: 49,
    Cantine_Vasche: 50,
    PlanningTestata: 51,
    PlanningEntita: 52,
    PlanningEntitaImpianto: 53,
    Agenda: 54,
    ricette_Testata: 60,
    ricette_dettaglio: 61,
    Anagrafica_Generica: 62,
    AgendaDestinazioni: 63,
    precision: 64
};

//utility.js

var utility = {
    chiaveAlbero_ridotta_to_big: function (chiave) {

        var Enum_CodiciCatastali_ridotti_totali = {
            piva: { value: 1, name: "piva", code: 1 },
            sa_cod: { value: 2, name: "sa_cod", code: 2 },
            part_cod: { value: 3, name: "part_cod", code: 3 },
            prov: { value: 4, name: "prov", code: 4 },
            com: { value: 5, name: "com", code: 5 },
            sezione: { value: 6, name: "sezione", code: 6 },
            foglio: { value: 7, name: "foglio", code: 7 },
            numero: { value: 8, name: "numero", code: 8 },
            subalterno: { value: 9, name: "subalterno", code: 9 }
        };

        var Enum_CodiciCatastali_ridotti_noSezNoSub = {
            piva: { value: 1, name: "piva", code: 1 },
            sa_cod: { value: 2, name: "sa_cod", code: 2 },
            part_cod: { value: 3, name: "part_cod", code: 3 },
            prov: { value: 4, name: "prov", code: 4 },
            com: { value: 5, name: "com", code: 5 },
            foglio: { value: 6, name: "foglio", code: 6 },
            numero: { value: 7, name: "numero", code: 7 }
        };

        var Enum_CodiciCatastali_ridotti_noSezione = {
            piva: { value: 1, name: "piva", code: 1 },
            sa_cod: { value: 2, name: "sa_cod", code: 2 },
            part_cod: { value: 3, name: "part_cod", code: 3 },
            prov: { value: 4, name: "prov", code: 4 },
            com: { value: 5, name: "com", code: 5 },
            foglio: { value: 6, name: "foglio", code: 6 },
            numero: { value: 7, name: "numero", code: 7 },
            subalterno: { value: 8, name: "subalterno", code: 8 }
        };

        var Enum_CodiciCatastali_ridotti_noSubalterno = {
            piva: { value: 1, name: "piva", code: 1 },
            sa_cod: { value: 2, name: "sa_cod", code: 2 },
            part_cod: { value: 3, name: "part_cod", code: 3 },
            prov: { value: 4, name: "prov", code: 4 },
            com: { value: 5, name: "com", code: 5 },
            sezione: { value: 6, name: "sezione", code: 6 },
            foglio: { value: 7, name: "foglio", code: 7 },
            numero: { value: 8, name: "numero", code: 8 },
        };

        var Enum_CodiciCatastali_completi = {
            piva: { value: 1, name: "piva", code: 1 },
            sa_cod: { value: 2, name: "sa_cod", code: 2 },
            part_cod: { value: 6, name: "part_cod", code: 6 },
            prov: { value: 7, name: "prov", code: 7 },
            com: { value: 8, name: "com", code: 8 },
            sezione: { value: 9, name: "sezione", code: 9 },
            foglio: { value: 10, name: "foglio", code: 10 },
            numero: { value: 11, name: "numero", code: 11 },
            subalterno: { value: 12, name: "subalterno", code: 12 }
        };

        var chiave_split = chiave.split("§");
        var tipo = chiave_split[0];

        var nuova_chiave = ['0'];
        for (var i = 0; i < 29 - 1; i++) {
            nuova_chiave.push('§0');
        }
        //modifico i stringavuota
        nuova_chiave[26] = '§';


        for (var i = 0; i < chiave_split.length; i++) {
            chiave_split[i] = '§' + chiave_split[i];
        }

        nuova_chiave[0] = tipo;

        if (Debug_Mode == Enum_debugMode.Verbose) {
            this.log(chiave);
        }

        switch (parseInt(tipo)) {

            case enum_TipoNodo.Centro:
                //Centro
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                break;

            case enum_TipoNodo.Agenda:
                //agenda
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[4] = chiave_split[3];
                nuova_chiave[5] = chiave_split[4];
                nuova_chiave[25] = chiave_split[5];
                break;

            case enum_TipoNodo.Campo:
                //Campo
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[3] = chiave_split[3];
                break;

            case enum_TipoNodo.Appezzamento:
                //Appezzamento
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[3] = chiave_split[3];
                nuova_chiave[4] = chiave_split[4];
                break;
            case enum_TipoNodo.ImpiantoNudo:
                //terreno nudo
                //' VAnni: 26/3/2018: Gestita mancata selezione di terreni nudi quando questi sono associati a campi
                if (chiave_split.length == 6) {
                    nuova_chiave[1] = chiave_split[1];
                    nuova_chiave[2] = chiave_split[2];
                    nuova_chiave[4] = chiave_split[3];
                    nuova_chiave[5] = chiave_split[4];
                }
                else {
                    if (chiave_split.length == 7) {
                        nuova_chiave[1] = chiave_split[1];
                        nuova_chiave[2] = chiave_split[2];
                        nuova_chiave[3] = chiave_split[3];
                        nuova_chiave[4] = chiave_split[4];
                        nuova_chiave[5] = chiave_split[5];
                    }
                    else {
                        alert("orticola/Arborea non gestita: tipo = " + tipo + " - dimensione = " + chiave_split.length + " - chiave: " + chiave_split.join());
                    }
                }
                break;

            case enum_TipoNodo.ricette_Testata:
                //ricetta
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[4] = chiave_split[3];
                nuova_chiave[5] = chiave_split[4];
                nuova_chiave[28] = chiave_split[5];
                break;

            case enum_TipoNodo.Analisi_Testata:
                //analisi
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[17] = chiave_split[3];
                nuova_chiave[18] = chiave_split[4];
                nuova_chiave[19] = chiave_split[5];
                nuova_chiave[20] = chiave_split[6];
                break;

            case enum_TipoNodo.Analisi_Campione:

                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[7] = "§0";
                nuova_chiave[8] = "§0";
                nuova_chiave[18] = chiave_split[3];
                nuova_chiave[20] = chiave_split[4];

                break;

            //case (enum_TipoNodo.ImpiantoOrticola || enum_TipoNodo.ImpiantoArborea || enum_TipoNodo.ImpiantoErbacea):
            case enum_TipoNodo.ImpiantoOrticola:
            case enum_TipoNodo.ImpiantoArborea:
            case enum_TipoNodo.ImpiantoErbacea:
            case enum_TipoNodo.Impianto_Generico:
                //orticole
                if (chiave_split.length == 6) {
                    nuova_chiave[1] = chiave_split[1];
                    nuova_chiave[2] = chiave_split[2];
                    nuova_chiave[4] = chiave_split[3];
                    nuova_chiave[5] = chiave_split[4];
                }
                else {
                    if (chiave_split.length == 7) {
                        nuova_chiave[1] = chiave_split[1];
                        nuova_chiave[2] = chiave_split[2];
                        nuova_chiave[3] = chiave_split[3];
                        nuova_chiave[4] = chiave_split[4];
                        nuova_chiave[5] = chiave_split[5];
                    }
                    else {
                        alert("orticola/Arborea non gestita: tipo = " + tipo + " - dimensione = " + chiave_split.length + " - chiave: " + chiave_split.join());
                    }
                }
                break;


            //        case enum_TipoNodo.ImpiantoErbacea:
            //            //erbacee
            //            nuova_chiave[1] = chiave_split[1];
            //            nuova_chiave[2] = chiave_split[2];
            //            nuova_chiave[3] = chiave_split[3];
            //            nuova_chiave[4] = chiave_split[4];
            //            nuova_chiave[5] = chiave_split[5];
            //            nuova_chiave[13] = chiave_split[6];
            //            nuova_chiave[14] = chiave_split[7];
            //            break;


            case enum_TipoNodo.ricette_dettaglio:
                //orticole
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[3] = chiave_split[3];
                nuova_chiave[4] = chiave_split[4];
                nuova_chiave[5] = chiave_split[5];
                nuova_chiave[13] = chiave_split[6];
                nuova_chiave[14] = chiave_split[7];
                break;

            case enum_TipoNodo.precision:
                //orticole
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[3] = chiave_split[3];
                nuova_chiave[4] = chiave_split[4];
                nuova_chiave[5] = chiave_split[5];
                nuova_chiave[13] = chiave_split[6];
                nuova_chiave[14] = chiave_split[7];
                break;


            case enum_TipoNodo.PlanningEntitaImpianto:
                //orticole
                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[23] = chiave_split[3];
                nuova_chiave[24] = chiave_split[4];
                break;

            case enum_TipoNodo.Particella:
                if (Debug_Mode == Enum_debugMode.Verbose) {
                    this.log("chiave = " + chiave);
                    this.log("chiave_split.length = " + chiave_split.length);
                }

                if (chiave_split.length == 8) {

                    if (Debug_Mode == Enum_debugMode.Verbose) {
                        this.log('//chiave senza "sezione catastale", "subalterno"');
                    }

                    nuova_chiave[Enum_CodiciCatastali_completi.piva.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezNoSub.piva.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.sa_cod.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezNoSub.sa_cod.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.part_cod.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezNoSub.part_cod.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.prov.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezNoSub.prov.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.com.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezNoSub.com.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.foglio.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezNoSub.foglio.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.numero.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezNoSub.numero.value];

                    //nuova_chiave[8] = chiave_split[3];
                    //nuova_chiave[10] = chiave_split[4];
                    //nuova_chiave[11] = chiave_split[5];
                }

                if (chiave_split.length == 9) {

                    if (isNaN(chiave_split[6])) {
                        if (Debug_Mode == Enum_debugMode.Verbose) {
                            this.log('//chiave con il "subalterno" impostato"');
                        }


                        nuova_chiave[Enum_CodiciCatastali_completi.piva.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezione.piva.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.sa_cod.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezione.sa_cod.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.part_cod.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezione.part_cod.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.prov.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezione.prov.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.com.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezione.com.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.foglio.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezione.foglio.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.numero.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSezione.numero.value];
                        let sub1 = chiave_split[Enum_CodiciCatastali_ridotti_noSezione.subalterno.value];
                        if (sub1 === "§") {
                            sub1 = "§0"
                        }
                        nuova_chiave[Enum_CodiciCatastali_completi.subalterno.value] = sub1;


                        //nuova_chiave[7] = chiave_split[2];
                        //nuova_chiave[8] = chiave_split[3];
                        //nuova_chiave[10] = chiave_split[4];
                        //nuova_chiave[11] = chiave_split[5];
                        //nuova_chiave[12] = chiave_split[6]; //subalterno
                    }
                    else {
                        if (Debug_Mode == Enum_debugMode.Verbose) {
                            this.log('//chiave con la "sezione catastale" impostata"');
                        }
                        nuova_chiave[Enum_CodiciCatastali_completi.piva.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSubalterno.piva.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.sa_cod.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSubalterno.sa_cod.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.part_cod.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSubalterno.part_cod.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.prov.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSubalterno.prov.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.com.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSubalterno.com.value];

                        let sez1 = chiave_split[Enum_CodiciCatastali_ridotti_noSubalterno.sezione.value];
                        if (sez1 === "§") {
                            sez1 = "§0"
                        }
                        nuova_chiave[Enum_CodiciCatastali_completi.sezione.value] = sez1;

                        nuova_chiave[Enum_CodiciCatastali_completi.foglio.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSubalterno.foglio.value];
                        nuova_chiave[Enum_CodiciCatastali_completi.numero.value] = chiave_split[Enum_CodiciCatastali_ridotti_noSubalterno.numero.value];

                        //nuova_chiave[7] = chiave_split[2];
                        //nuova_chiave[8] = chiave_split[3];
                        //nuova_chiave[9] = chiave_split[4]; //sezione catastale
                        //nuova_chiave[10] = chiave_split[5];
                        //nuova_chiave[11] = chiave_split[6];
                    }


                }

                if (Debug_Mode == Enum_debugMode.Verbose) {
                    this.log('//chiave completa"');
                }
                if (chiave_split.length == 10) {

                    nuova_chiave[Enum_CodiciCatastali_completi.piva.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.piva.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.sa_cod.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.sa_cod.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.part_cod.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.part_cod.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.prov.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.prov.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.com.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.com.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.sezione.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.sezione.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.foglio.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.foglio.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.numero.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.numero.value];
                    nuova_chiave[Enum_CodiciCatastali_completi.subalterno.value] = chiave_split[Enum_CodiciCatastali_ridotti_totali.subalterno.value];

                    //nuova_chiave[7] = chiave_split[2];
                    //nuova_chiave[8] = chiave_split[3];
                    //nuova_chiave[9] = chiave_split[4]; //sezione catastale
                    //nuova_chiave[10] = chiave_split[5];
                    //nuova_chiave[11] = chiave_split[6];
                    //nuova_chiave[12] = chiave_split[7]; //subalterno
                }
                break;

            case enum_TipoNodo.AgendaDestinazioni:

                if (Debug_Mode == Enum_debugMode.Verbose) {
                    this.log("chiave AgendaDestinazioni= " + chiave);
                    this.log("chiave_split.length = " + chiave_split.length);
                }

                if (chiave_split.length === 8) {
                    nuova_chiave[1] = chiave_split[1];
                    nuova_chiave[2] = chiave_split[2];
                    nuova_chiave[3] = chiave_split[3];
                    nuova_chiave[4] = chiave_split[4];
                    nuova_chiave[5] = chiave_split[5];
                    nuova_chiave[25] = chiave_split[6];
                } else {
                    nuova_chiave[1] = chiave_split[1];
                    nuova_chiave[2] = chiave_split[2];
                    nuova_chiave[4] = chiave_split[3];
                    nuova_chiave[5] = chiave_split[4];
                    nuova_chiave[25] = chiave_split[5];
                }

                break;

            case enum_TipoNodo.Fabbricato_Generico:

                nuova_chiave[1] = chiave_split[1];
                nuova_chiave[2] = chiave_split[2];
                nuova_chiave[14] = chiave_split[3];

                break;

            default:
                if (tipo != 0)
                    if (Debug_Mode == Enum_debugMode.Soft) {
                        this.log("errore " + tipo + " : " + chiave);
                    }
                break;
        }

        if (Debug_Mode == Enum_debugMode.Verbose) {
            this.log('join ' + nuova_chiave.join(''));
        }


        var r = nuova_chiave.join('');
        return r.replace(",", "");
    },
    log: function (txt) {
        console.log(txt);
    }
    ,
    warn: function (str) { console.warn(str); }
    ,
    log_Time: function (descrizione) {
        this.log(descrizione + ": " + this.timeStamp());
    }
    ,
    timeStamp: function () {
        // Create a date object with the current time
        var now = new Date();

        // Create an array with the current month, day and time
        var date = [now.getMonth() + 1, now.getDate(), now.getFullYear()];

        // Create an array with the current hour, minute and second
        var time = [now.getHours(), now.getMinutes(), now.getSeconds()];

        // Determine AM or PM suffix based on the hour
        var suffix = (time[0] < 12) ? "AM" : "PM";

        // Convert hour from military time
        time[0] = (time[0] < 12) ? time[0] : time[0] - 12;

        // If hour is 0, set it to 12
        time[0] = time[0] || 12;

        // If seconds and minutes are less than 10, add a zero
        for (var i = 1; i < 3; i++) {
            if (time[i] < 10) {
                time[i] = "0" + time[i];
            }
        }

        // Return the formatted string
        return date.join("/") + " " + time.join(":") + " " + suffix;
    }
    ,
    sleep:function (milliseconds) {
        var start = new Date().getTime();
        for (var i = 0; i < 1e7; i++) {
            if ((new Date().getTime() - start) > milliseconds) {
                break;
            }
        }
    }
}

export { utility };
