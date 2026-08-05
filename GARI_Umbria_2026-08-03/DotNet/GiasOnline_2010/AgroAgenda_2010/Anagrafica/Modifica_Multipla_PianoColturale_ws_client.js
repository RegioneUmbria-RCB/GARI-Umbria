
function FiltraEsercizi() {

    if (usaFiltroRicercaNG) {

        var param = kendo.stringify({
            "piva": Piva,
        });
        ajaxAgronica(indirizzohttp + "/Link_Pagina_FiltroRicercaNG",
            param,
            function (risposta) {
                apriFinestraFiltroRicercaNG(risposta.RispostaStringa)
            }, function (risposta) {
                kendo.alert(risposta.Errore)
            });
    } else {
        ajaxAgronica(indirizzohttp + "/FiltraEsercizi",
            "{ }",
            function (risposta) {
                window.location = risposta.RispostaStringa;
            },
            null);
    }
}

function apriFinestraFiltroRicercaNG(url) {
    window.addEventListener('message', chiudiFinestraFiltroRicercaNG);

    $(document.body).append('<div id="filtro_ricerca_ng"></div>');

    $('#filtro_ricerca_ng').kendoWindow({
        title: "Filtra Esercizi",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#filtro_ricerca_ng').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function chiudiFinestraFiltroRicercaNG(event) {
    let kWin = $('#filtro_ricerca_ng').data("kendoWindow");
    let urlKWin = kWin.options.content.url;

    if (verificaOriginSecondaria(window, urlKWin, event) &&
        (event != null && event.data != null) && (event.data.messaggio != null) &&
        event.data.messaggio.includes("chiudiWindowGiasNG")) {
        chiavi = event.data.inData.chiavi;
        WS_Popola_Griglia_da_Chiavi(chiavi)
        kWin.close();
    }
}

function WS_Popola_Griglia_da_Chiavi(chiavi) {

    //Estraggo le chiavi degli esercizi piva_saCod_appezza_idReg_vegCod_progettoCod
    var progetti = []
    chiavi.forEach((chiave) => progetti.push(chiave.split("_")[5]))

    var param = kendo.stringify({
        "progetti": progetti,
        "chiavi": chiavi
    });

    ajaxAgronica(indirizzohttp + "/WS_Popola_Griglia_da_Chiavi",
        param,
        function (risposta) {
            chiama_popola_griglia(risposta.RispostaStringa)
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        }, null, true);
}


function LeggiDisciplinarePrivato() {
    return new Promise((resolve, reject) => {

        var param = {};

        ajaxAgronica("Modifica_Multipla_PianoColturale.aspx/LeggiDisciplinarePrivato", JSON.stringify(param),
            function (risposta) {
                if (risposta.RispostaStringa == "true") {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, null, false);
    });
}

// ####################################################################################
// ###########################     MDOFICA RESA PREVISTA    ###########################
// ####################################################################################

function editResaPrevista() {
    var chiudi = false;
    WaitFrame.show();
    
    var grid = $("#divKendoModifica_Multipla_PianoColturale").data("kendoGrid");
    var data = grid.dataSource.data();
    let datiselezionati = data.
        filter(x => x.Selected == true && x.Blk_Flag != -1 && x.cul_cod != 0 && x.EsercizioChiusoCod != 1)
        .map(function (item) {
            return {
                PIVA: item.PIVA,
                sa_cod: item.sa_cod,
                Appezza: item.Appezza,
                id_Reg: item.id_Reg,
                Progetto_Cod: item.Progetto_Cod
            }
        })

    var param = kendo.stringify({ esercizi: kendoEscapeOggetto(datiselezionati) });
    
    ajaxAgronica(indirizzohttp + "/editResaPrevista", param,
        function (risposta) {
            if (risposta.RispostaOK) {
                chiudi = true;
                AggiornaDati_NoRefresh(chiavi);
            }

            WaitFrame.hide();
        },
        function (risposta) {
            
            if (risposta.RispostaStringa !== "") {
                /*kendo.alert(risposta.RispostaStringa);*/
                var msg = ""
                if (risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                    let errore = risposta.ErroriGias.map(function (e) { return e.messaggio }).join("<br>")
                    msg = errore;
                }
                else {
                    chiudi = true
                    msg = risposta.RispostaStringa;
                }

                kendo.confirm(msg)
                    .done(function () {
                        // Quando l'utente preme 'OK', esegui AggiornaDati
                        WS_Popola_Griglia_da_Chiavi(chiavi)

                    });
                //Rimuovo il pulsante "NO"
                $("button.k-button.k-button-md.k-rounded-md.k-button-solid.k-button-solid-base ").css("display", "none")
            } else {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }

            WaitFrame.hide();
        },
        null,
        false
    );

    return chiudi;
}

function ModificaMultipla() {
    var chiudi = false
    obj_ModificaMultipla.parametri = parametri_multipli ? Cmb_Parametri.value() : [Cmb_Parametri.value()];
    obj_ModificaMultipla.parametri_modificati = parametri_multipli ? Cmb_Parametri.value() : [Cmb_Parametri.value()];
    if (Cmb_Mod_Esercizi != undefined) obj_ModificaMultipla.mod_esercizi = Cmb_Mod_Esercizi.value();
    if (Txt_Data_Esercizi != undefined) obj_ModificaMultipla.data_esercizi = kendo.toString(Txt_Data_Esercizi.value(), 'd');
    var param = kendo.stringify({ parametri: kendoEscapeOggetto(obj_ModificaMultipla), dati: kendoEscapeOggetto(selected_add) });


    let datiSelezionati = $('#divKendoModifica_Multipla_PianoColturale').data("kendoGrid").dataSource.data().
        filter(x => x.Selected == true && x.Blk_Flag != -1).
        map(function (item) {
            return {
                PIVA: item.PIVA,
                sa_cod: item.sa_cod,
                Appezza: item.Appezza,
                id_Reg: item.id_Reg,
                Progetto_Cod: item.Progetto_Cod,
                EsercizioChiuso: item.EsercizioChiusoCod,
                validita_fine: item.validita_fine
            }
        }).sort(function (a, b) {
            //Li ordino per aver ogni appezzamento con l'ultimo esercizio valido per primo

            // Ordinamento per PIVA (crescente)
            if (a.PIVA !== b.PIVA) return a.PIVA > b.PIVA ? 1 : -1;

            // Ordinamento per sa_cod (crescente)
            if (a.sa_cod !== b.sa_cod) return a.sa_cod > b.sa_cod ? 1 : -1;

            // Ordinamento per Appezza (crescente)
            if (a.Appezza !== b.Appezza) return a.Appezza > b.Appezza ? 1 : -1;

            // Ordinamento per validita_fine (decrescente)
            if (a.validita_fine !== b.validita_fine) return a.validita_fine < b.validita_fine ? 1 : -1;

            return 0; // Se tutti i criteri sono uguali
        });

    var param = kendo.stringify
        ({
            objP_server: objP_server,
            parametri: kendoEscapeOggetto(obj_ModificaMultipla),
            dati: kendoEscapeOggetto(datiSelezionati)
        });

    ajaxAgronica(pathCoreWS + "Anagrafica/Reg_Impianto.asmx/ModificaMultipla",
        param,
        function (risposta) {
            if (risposta.RispostaOK) {

                var msg = ""
                if (risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                    let errore = risposta.ErroriGias.map(function (e) { return e.messaggio }).join("<br>")
                    msg = errore;
                }
                else {
                    chiudi = true
                    msg = risposta.RispostaStringa;
                }

                kendo.confirm(msg)
                    .done(function () {
                        // Quando l'utente preme 'OK', esegui AggiornaDati
                        WS_Popola_Griglia_da_Chiavi(chiavi)
                        if (chiudi)
                            chiudiModificaMultipla();

                    });
                //Rimuovo il pulsante "NO"
                $("button.k-button.k-button-md.k-rounded-md.k-button-solid.k-button-solid-base ").css("display", "none")

            }
        }, function (risposta) {
            if (risposta.RispostaStringa !== "") {
                kendo.alert(risposta.RispostaStringa);
            } else if (risposta.ErroriGias != undefined && risposta.ErroriGias.length > 0) {
                let errore = risposta.ErroriGias.map(function (e) { return e.messaggio }).join("<br>")
                kendo.confirm(errore)
                    .done(function () {
                        WS_Popola_Griglia_da_Chiavi(chiavi)
                    });
                //Rimuovo il pulsante "NO"
                $("button.k-button.k-button-md.k-rounded-md.k-button-solid.k-button-solid-base ").css("display", "none")
            }
            else {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }
        }, null, true);

    return chiudi
}
