var grdRichiestaDocumenti = "grdRichiestaDocumenti";
var DdlCausalidelnonutilizzo;


function TraduciLavorazioni(chiave, testoAlternativo) {
    return TraduzioneMultiResx(confRichiestaDocumentiResx, chiave, testoAlternativo);
}

//DOCUMENT READY
$(document).ready(function () {
    docR();
});


function onActivate(e) {


    //kendoConsole.log("Activated: " + $(e.item).find("> .k-link").text());
    var selectedIndex = $(e.item).index();

    //kendoConsole.log("selectedIndex: " + selectedIndex);

    if (selectedIndex == 3 && bZooInizializato == false && richiesta_cod > 0 && QS_Type == 0) {
        bZooInizializato = true;

        ucUmaAllevamenti_initGrid();
    }


    //per tab DOCUMENTI
    var tabStrip = $("#tabstrip_elenco").kendoTabStrip();
    tabStrip.select(e.item);
    var elemento = tabStrip.select().index();

    if (!$("#grdRichiestaDocumenti").getKendoGrid() && $('#ddlAzienda').data("kendoDropDownList").value() != '') {
        $("#btn_carica_allegati").show();

        WaitFrame.show();
        popolaGrigliaRichiestaDocumenti("grdRichiestaDocumenti");

        WaitFrame.hide();
    }
    if (selectedIndex == 5) {

    }
    else {
        var grid = $("#grdRichiestaDocumenti").data("kendoGrid");
        if (grid != null && grid != undefined) {
            for (i = 0; i < grid.columns.length; i++) {
                grid.autoFitColumn(i);
            }
        }
    }
}

async function docR() {
    $.logThis("DocReady: INIZIO");
    //inizializzazione della pagina la prima volta che viene caricata
    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            gestioneCarbResx.push(readResxFile(resxSinglePath, "gestionecosti_jQueryDocReady.js"));
        });
    }

    modifica_richiesto = false;
    modifica_assegnato = false;

    let promises = new Array();
    promises.push(Agro_LeggiPermessoUtente(username_master, 455, 2));
    promises.push(Agro_LeggiPermessoUtente(username_master, 456, 2));
    promises.push(Agro_LeggiPermessoUtente(username_master, 457, 2));
    promises.push(Agro_LeggiPermessoUtente(username_master, 458, 2));
    promises.push(Agro_LeggiPermessoUtente(username_master, 455, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 456, 0));

    let resp = await Promise.all(promises);

    permesso_richiesta = resp[0];
    permesso_rendicontazione = resp[1];
    permesso_approvazione_richiesta = resp[2];
    permesso_approvazione_rendicontazione = resp[3];
    let permesso_lettura_richiesta = resp[4];
    let permesso_lettura_rendicontazione = resp[5];

    TxtRimanenza_Gasolio = $("#TxtRimanenza_Gasolio").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        /*change: function () { },*/
        spinners: false
    }).data("kendoNumericTextBox");

    TxtRimanenza_Benzina = $("#TxtRimanenza_Benzina").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        /* change: function () { },*/
        spinners: false
    }).data("kendoNumericTextBox");

    TxtRimanenza_Gasolio_Serra = $("#TxtRimanenza_Gasolio_Serra").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        change: function () { },
        spinners: false
    }).data("kendoNumericTextBox");

    gasolioTerz = $("#gasolioTerzisti").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        change: function () { },
        spinners: false
    }).data("kendoNumericTextBox");

    benzinaTerz = $("#benzinaTerzisti").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        change: function () { },
        spinners: false
    }).data("kendoNumericTextBox");

    gasolioSerraTerz = $("#gasolioSerraTerzisti").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        change: function () { },
        spinners: false
    }).data("kendoNumericTextBox");

    benzinaTerzAppro = $("#benzinaTerzistiAppro").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        change: function () { },
        spinners: false
    }).data("kendoNumericTextBox");

    gasolioTerzAppro = $("#gasolioTerzistiAppro").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        change: function () { },
        spinners: false
    }).data("kendoNumericTextBox");

    gasolioSerraTerzAppro = $("#gasolioSerraTerzistiAppro").kendoNumericTextBox({
        min: 0,
        format: "{0:##.####}",
        decimals: 0,
        change: function () { },
        spinners: false
    }).data("kendoNumericTextBox");

    creaKendoSwitch();

    if (QS_Type == 0 && !(permesso_richiesta || permesso_approvazione_richiesta || permesso_lettura_richiesta)) {
        return;
    }

    if (QS_Type == -1 && !(permesso_rendicontazione || permesso_approvazione_rendicontazione || permesso_lettura_rendicontazione)) {
        return;
    }

    //if (QS_Avanzamento == 1 && QS_Richiesta == 0) {
    if (QS_Richiesta == 0 && QS_Anticipo == 0) {
        $("#row_Fascicolo").show();
        await ddlFascicoli_Load('#fascicolo');
    } else {
        $("#row_Fascicolo").hide();
        //$(".btn_carica_richieste")[0].children[1].innerHTML = (QS_Avanzamento != 0) ? "ELENCO RENDICONTAZIONI" : "ELENCO RICHIESTE"
    }

    if (QS_Anticipo == 1) {
        $("#tab1").hide();
        $("#tab2").hide();
        $("#tab4").hide();
        $("#tab5").hide();
        $("#tab3").hide();  //nasconde la tab di documenti dobbiamo verificare
        $("#tab6").hide();

        $("#mainContainer").hide(); //Da togliere se vogliamo mostrare la griglia dei documenti
    }

    if (QS_Type == -1 && QS_Avanzamento == 0) {
        $("#tab6").show();
    } else {
        $("#tab6").hide();
    }

    await ddlAzienda_Load();

    if (QS_Richiesta > 0) {
        $("#btn_carica_richieste").hide();
        $("#row_Fascicolo").hide();
    } else {
        $("#btn_carica_richieste").show();

    }

    if (QS_Avanzamento == 1) {
        $("#tab1").html("Rendicontazione per gruppo colturale/lavorazione");
    }

    var gestOraInizioFine = RicercaUtenti_Impostazioni(enum_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE, 2);
    if (gestOraInizioFine === "notSet")
        gestOraInizioFine = "1";
    if (gestOraInizioFine === undefined || gestOraInizioFine === "0")
        hiddenOraInizioFine = true;
    else
        hiddenOraInizioFine = false;

    (function ($, kendo) {
        $.extend(true, kendo.ui.validator, {
            rules: { // custom rules
                timevalidation: function (input, params) {

                    if ($(input).data("kendoTimePicker") !== undefined &&
                        $(input).data("kendoTimePicker").value() === null)
                        return false;

                    return true;
                },
            },
            messages: {
                timevalidation: function (input) {
                    return (TraduzioneMultiResx(gestioneCarbResx, "OraNonValida", "Ora non valida"));
                },
            }
        });
    })(jQuery, kendo);

    // Se vengo da campagna il default va su Risorse
    if ($('input[name$="hdId_Agenda"]').val() !== "" &&
        $('input[name$="hdId_Agenda"]').val() !== "0") {
        $('#a_tabTestata').tab('show');
    }

    $(".preArea").show();
    $(".testataArea").show();
    $(".dettagliArea").show();

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    tabstrip = $("#tabstrip_dettagli").kendoTabStrip({
        animation: false,
        //select: onSelect,
        activate: onActivate
        //show: onShow,
    }).data("kendoTabStrip");


    $("#Doc").click(
        function () {
            document.getElementsByClassName(GIAS_K_STATE_ACTIVE).removeClass(GIAS_K_STATE_ACTIVE);
            document.getElementsByClassName("Doc").addClass(GIAS_K_STATE_ACTIVE)
            document.getElementsByClassName("dettagliImpianti").style("display: none");
            document.getElementsByClassName("documenti").style("display: block");
        }
    );


    //eventi di click pulsanti
    $("#btn_carica_allegati").click(
        function () {
            CaricaAllegati();
        });

    //eventi di click pulsanti
    $("#btn_nuovo_allegato").click(
        function () {
            NuovoAllegato();
        });

    //eventi di click pulsanti
    $("#btn_aggiungi_macchina").click(
        function () {
            AggiungiMacchina();
        });
    $("#btn_apri_frame_analisi_terreno").click(
        function () {
            ApriFrameAnalisiTerreno();
        });

    //PopolaElencoLavUMA();
    if (QS_Anticipo == 0) {
        PopolaElencoCarb();
        //PopolaTabellaCalcoloCosti();
        //RiempiElencoLavorazioniValidita();
    }

    //if (QS_Type == -1) {
    //    $("#lbl_Richiesta").html("Richiesta iniziale");
    //}

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    //Nascondo Tab Linee Produzioni
    //$($("#tabstrip_dettagli").data("kendoTabStrip").items()[2]).attr("style", "display:none");

    //Nascondo pulsante Refresh
    if (($(cId_Mov_Det).val() == "0") && ($(cId_Agenda).val() !== "0" || $(cId_Agenda_CDG).val() !== "0")) {
        $("#btn_refresh").attr("style", "display:none");
    }



    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    //$(".btn_carica_richieste")[0].children[1].innerHTML = (QS_Avanzamento != 0) ? "ELENCO RENDICONTAZIONI" : "ELENCO RICHIESTE"

    //eventi di click pulsanti
    $("#btn_carica_richieste").click(
        async function () {

            //alert(QS_Richiesta);
            if ($("#anno").val() !== "") {
                await PopolaValori_Setup($("#anno").val());
            }

            if (QS_Richiesta <= 0) {
                var anno = $("#anno").val();
                var piva = KendoDDL("ddlAzienda").value()

                setYear(anno);
                if (piva == "") {
                    kendo.alert("Selezionare un'azienda.");
                    return;
                }
                if (isNumeric(anno) && parseInt(anno) && parseInt(anno) < 2025) {
                    if ($("#fascicolo")[0].value == "" && QS_Anticipo != 1 && QS_Type == 0) {
                        kendo.alert("Selezionare un fascicolo.");
                        return;
                    }
                }
                if (nIscrizioneCameraDiCommercio[piva] == 0 && KendoDDL("ddlAzienda").dataItem().flagPubblica != 1) {
                    kendo.alert((QS_Type == 0 ? "L'azienda non risulta iscritta" : "Il terzista non risulta iscritto") + " alla camera di commercio");
                    return;
                }

                if (isNumeric(anno) && parseInt(anno) >= 1900 && parseInt(anno) <= 2100) {
                    var numeroRichieste = await ContaRichiesteWS(anno, QS_Avanzamento, QS_Type == -1)
                    numeroRichieste = parseInt(numeroRichieste.length);

                    //spostate da riga 359
                    let rendicontazioniAnnoCorrente = await ContaRichiesteWS(anno, 1, QS_Type == -1);
                    let numRendicontazioniAnnoCorrente = parseInt(rendicontazioniAnnoCorrente.length);
                    let RendicontazioniAnnoCorrenteInCompilazione = rendicontazioniAnnoCorrente.filter(r => r.Stato_Cod == In_Compilazione);
                    let numRendicontazioniAnnoCorrenteInCompilazione = parseInt(RendicontazioniAnnoCorrenteInCompilazione.length);

                    let rendicontazioniAnnoPrecedente = await ContaRichiesteWS(anno - 1, 1, QS_Type == -1);
                    let rendicontazioniApprovate = rendicontazioniAnnoPrecedente.filter(r => r.Stato_Cod == Verifica_Intermedia_Completata_Con_Successo || r.Stato_Cod == Inserimento_Completato_Per_Il_Periodo_Di_Competenza || r.Stato_Cod == Verifica_Completata);
                    let cessata = rendicontazioniAnnoPrecedente.filter(r => r.Rinuncia_Nuova_Richiesta_Anno_Successivo == 1).length > 0;
                    let numRendicontazioniApprovate = parseInt(rendicontazioniApprovate.length);
                    let richiesteAnnoPrecedente = await ContaRichiesteWS(anno - 1, 0, QS_Type == -1);
                    let numRichiesteApprovateAnnoPrecedente = richiesteAnnoPrecedente.filter(r => r.Stato_Cod == Verifica_Intermedia_Completata_Con_Successo || r.Stato_Cod == Inserimento_Completato_Per_Il_Periodo_Di_Competenza || r.Stato_Cod == Verifica_Completata).length;
                    //fine spostamento da riga 359
                    var salvataggioanticipo = true


                    //aggiunto controllo x anticipi

                    var data = new Date();
                    data.setHours(0, 0, 0, 0);
                    //var dataoggi = data.getDate() + "/" + data.getMonth() + 1 + "/" + data.getFullYear();

                    if (QS_Anticipo == 1) {
                        var datafineinserimento = new Date(Leggi_Data_Fine_Inserimento_Richieste_Anticipo())
                        //if (valido) {
                        if (data <= datafineinserimento) {
                            var numeroRichiesteXAnt = await ContaRichiesteWS(anno, -2, QS_Type == -1, true) //-2 indica anticipi e richieste nella lettura delle testate
                            numeroRichiesteXAnt = parseInt(numeroRichiesteXAnt.length);

                            if (numeroRichiesteXAnt > 0) {
                                salvataggioanticipo = false
                                kendo.alert("Esiste già almeno una richiesta per l'anno in corso, non è possibile proseguire.");
                                return;
                            }
                            //else if (numRendicontazioniApprovate == 0) {
                            //    salvataggioanticipo = false
                            //    kendo.alert("Non è possibile richiedere un'anticipo se non esistono richieste per l'anno precedente.");
                            //    return;
                            //}

                            let consentiAnticipo = await VerificaConsentiAnticipo();
                            if (!consentiAnticipo) {
                                salvataggioanticipo = false
                                kendo.alert("Impossibile procedere con la richiesta di anticipo: non è presente nessuna richiesta approvata successiva all'ultimo anticipo concesso (nell'anno " + ($("#anno").val() - 1).toString() + ").");
                                return;
                            }


                            if (salvataggioanticipo == true) {

                                //Mostro il div degli anticipi
                                $("#raccoglitore_Anticipo").css('display', '')

                                if (cessata) {
                                    kendo.alert("Non è possibile inserire un anticipo poichè è stato dichiarato che l'azienda è cessata");
                                    return;
                                } else {

                                    await PercentualeAnticipo(Percentuale_Anticipo_Carb);
                                    if (!proseguiAnticipo) return false

                                    await creaRichiesta(anno, richiesta_cod, QS_Type == -1, false, numeroRichieste, 0);

                                    $('#panel-carburantiTerzisti').show();

                                    //$("#tabstrip_dettagli").data("kendoTabStrip").remove(".Verb");
                                    //$("#tabstrip_dettagli").data("kendoTabStrip").remove(".Ric");
                                    //$("#tabstrip_dettagli").data("kendoTabStrip").remove(".Terz");
                                    //$("#tabstrip_dettagli").data("kendoTabStrip").remove(".Mac");
                                    //$("#tabstrip_dettagli").data("kendoTabStrip").select("Doc");
                                    $("#tabstrip_dettagli").data("kendoTabStrip").remove("Doc");
                                }
                            }
                        } else {

                            if (datafineinserimento.format('dd/MM/yyyy') == '01/01/1900') {
                                kendo.alert("Non è possibile inserire una richiesta di anticipo per l'anno " + $("#anno").val() + ", la data di Termine Presentazione Richieste di Anticipo non è impostata correttamente. ")
                            } else {
                                kendo.alert("Non è possibile inserire una richiesta di anticipo per l'anno " + $("#anno").val() + " oltre il giorno " + datafineinserimento.format('dd/MM/yyyy'))
                            }
                            return;
                        }

                    } else {
                        salvataggioanticipo = false;
                        $("#lbl_Richiesta").html("Richiesta iniziale");
                    }
                    //fine aggiunta controllo x anticipi


                    if (numeroRichieste > 0 && QS_Avanzamento == 0 && salvataggioanticipo == false) { //aggiunto gloria salvataggioanticipo == false

                        if (numRendicontazioniAnnoCorrente > 0) {
                            if (numRendicontazioniAnnoCorrente === numRendicontazioniAnnoCorrenteInCompilazione) {
                                if (!await kendoConfirm_Promise("È già stata inserita una rendicontazione in compilazione, procedere?")) {
                                    return;
                                }
                            }
                            else {
                                kendo.alert("Non è più possibile inserire richieste dopo aver inserito una rendicontazione non in compilazione");
                                return;
                            }
                        }

                        if (cessata) {
                            kendo.alert("Non è possibile inserire una nuova richiesta poichè è stato dichiarato che l'azienda è cessata");
                            return;
                        }

                        let richiesteAnnoCorrente = await ContaRichiesteWS(anno, 0, QS_Type == -1);
                        let numRichiesteAnnoCorrenteApprovate = parseInt(richiesteAnnoCorrente.filter(r => r.Stato_Cod == Verifica_Intermedia_Completata_Con_Successo || r.Stato_Cod == Inserimento_Completato_Per_Il_Periodo_Di_Competenza || r.Stato_Cod == Verifica_Completata).length);
                        if (numRichiesteAnnoCorrenteApprovate === richiesteAnnoCorrente.length) {
                            if (await kendoConfirm_Promise("Esiste già una richiesta per l'anno in corso, verrà creata una nuova richiesta integrativa, procedere?")) {
                                var abilitato = true
                                if (QS_Type == -1 && QS_TipoAzienda == Azienda_Terzista) abilitato = await terzistaIntegrazione(anno)
                                if (abilitato) {
                                    WaitFrame.show();
                                    if (QS_Type == 0 && QS_Anticipo == 1) {
                                        await creaRichiesta(anno, richiesta_cod, QS_Type == -1, false, numeroRichieste, 1);

                                    } else {
                                        await creaRichiesta(anno, richiesta_cod, QS_Type == -1, false, numeroRichieste, 1);
                                        if (QS_Type != -1) {
                                            $("#fascicolo").data("kendoDropDownList").enable(false);
                                        }
                                    }
                                    if (QS_Anticipo == 0) {
                                        $("#btn_PassaggioDiStato").show();
                                    } else {
                                        $("#btn_PassaggioDiStato").hide();
                                    }
                                    $("#btn_carica_richieste").hide();
                                    $("#row_Fascicolo").hide();
                                    //$("#ddlAzienda").data("kendoDropDownList").enable(false);

                                    $("#anno").prop('disabled', true);
                                    $("#lbl_rimanenze").val("Rimanenze anno precedente");
                                    WaitFrame.hide();

                                    if (QS_Anticipo != 1) ///aggiunto gloria
                                        mostraRichieste();

                                } else {
                                    //I numeri dall'80 all'81 hanno bisogno di un articolo diverso rispetto agli altri numeri
                                    let perc = (percentualePrelievo >= 80 && percentualePrelievo <= 89) ? "l'" + percentualePrelievo.toString() : "il " + percentualePrelievo.toString()
                                    kendo.alert("Prima di creare una nuova richiesta integrativa è necessario aver prelevato almeno " + perc + "% del carburante richiesto");
                                    return;
                                }
                            } else {
                                return;
                            }
                        } else {
                            kendo.alert("Prima di creare una nuova richiesta integrativa è necessario che tutte le precedenti richieste siano state approvate");
                            return;
                        }


                    } else if (numeroRichieste > 0 && QS_Avanzamento == 1 && salvataggioanticipo == false) { //&& salvataggioanticipo == false gloria
                        kendo.alert("Non è possibile creare più rendicontazioni nello stesso anno.");
                        return;
                    } else {
                        let copia = false
                        //let rendicontazioniAnnoPrecedente = await ContaRichiesteWS(anno - 1, 1, QS_Type == -1);
                        //let rendicontazioniApprovate = rendicontazioniAnnoPrecedente.filter(r => r.Stato_Cod == 2005 || r.Stato_Cod == 2008 || r.Stato_Cod == 2003);
                        //let numRendicontazioniAnnoPrecedente = parseInt(rendicontazioniAnnoPrecedente.length);
                        //let numRendicontazioniApprovate = parseInt(rendicontazioniApprovate.length);
                        //let richiesteAnnoPrecedente = await ContaRichiesteWS(anno - 1, 0, QS_Type == -1);
                        //let numRichiesteApprovateAnnoPrecedente = richiesteAnnoPrecedente.filter(r => r.Stato_Cod == 2005 || r.Stato_Cod == 2008 || r.Stato_Cod == 2003).length;
                        //if (QS_Type == 0 && QS_Avanzamento == 0 && numRendicontazioniApprovate > 0) {
                        //    //copia = await dialogNuovaRichiestaDaRendicontazionePromise("Richiesta da Rendicontazione Precedente");
                        //} else if (QS_Type == -1 && QS_Avanzamento == 0 && numRendicontazioniApprovate == 0 && numRendicontazioniAnnoPrecedente > 0) {
                        //    kendo.alert("Esistono rendicontazioni dell’anno precedente non approvate. Non è possibile procedere con una nuova domanda ");
                        //    return;
                        //if (QS_Avanzamento == 0 && numRendicontazioniAnnoPrecedente == 0) {
                        //    kendo.alert("Non esistono rendicontazioni dell’anno precedente. Non è possibile procedere con una nuova domanda ");
                        //    return;
                        //}
                        //} else if (QS_Type == -1 && QS_Avanzamento == 0 && numRendicontazioniApprovate > 0 && numRendicontazioniAnnoPrecedente == numRendicontazioniApprovate) {
                        //    //copia = await dialogTerzistiPromise("Creazione richiesta come copia di rendicontazione precedente", "Verrà creata una nuova richiesta sulla base della rendicontazione nr. " + rendicontazioniApprovate[0].Numero + " dell’anno " + (anno - 1) + " ", "Ok");
                        //}
                        let richiesteEsistenti = await ContaRichiesteWS(anno, 0, QS_Type == -1);
                        let numRichiesteEsistenti = richiesteEsistenti.filter(r => r.Stato_Cod == Verifica_Intermedia_Completata_Con_Successo || r.Stato_Cod == Inserimento_Completato_Per_Il_Periodo_Di_Competenza || r.Stato_Cod == Verifica_Completata).length;
                        if (numRichiesteEsistenti < 1 && QS_Avanzamento == 1 && salvataggioanticipo == false) {
                            kendo.alert("Non è possibile creare una rendicontazione se non esistono richieste approvate associate.")  //a quest" + (QS_Type != -1 ? "a azienda" : "o terzista") + ". ");
                            return;
                        }
                        if (!copia && numRendicontazioniApprovate > 0) {
                            richiesteEsistenti = 0;
                        }

                        if (QS_Avanzamento == 1) {
                            //Sono in rendicontazione
                            Leggi_Date_Ins_Rendicontazione()
                            if (!consentitoAggiungereModificareRendicontazione_daSetup) {
                                if (Data_Inizio_Rendicontazione !== "" && Data_Fine_Rendicontazione !== "") {
                                    if ((new Date().getTime()) < (EUdate(Data_Inizio_Rendicontazione).getTime())) {
                                        await kendo.confirm("Non è possibile creare una rendicontazione prima del " + Data_Inizio_Rendicontazione + " a meno che non sia un caso di azienda cessata. Procedendo confermi che l'azienda ha cessato la sua attività")
                                            .done(function () {
                                                setKendoSwitch("noRichiestaAnnoProxCheck", true);
                                            })
                                            .fail(function () {
                                                return;
                                            });
                                    } else {
                                        kendo.alert("Non è possibile creare una rendicontazione prima del " + Data_Inizio_Rendicontazione + " e dopo il " + Data_Fine_Rendicontazione + " per l'anno " + $("#anno").val());
                                        return;
                                    }
                                }
                            }

                            if (get_TipoAzienda() === Azienda_Agricola_Privata) {
                                controllaPermesso_InserimentoRendicontazione_ContoProprio()
                                if (!consentitoInserireRendicontazioneContoProprio) {
                                    //kendo.alert("Il CUAA selezionato risulta presente in Rendicontazioni di terzisti / coop. Sarà possibile inserire una rendicontazione dopo il .....")  //a quest" + (QS_Type != -1 ? "a azienda" : "o terzista") + ". ");
                                    kendo.alert(nonConsentitoInserireRendicontazioneContoProprio_Mess)
                                    return
                                }
                            }
                        }

                        if (QS_Avanzamento == 0) {
                            //Sono in richiesta
                            if (cessata) {
                                kendo.alert("Non è possibile inserire una nuova richiesta poichè è stato dichiarato che l'azienda è cessata");
                                return;
                            }
                        }

                        if (salvataggioanticipo == false) {  //aggiunto Gloria
                            await creaRichiesta(anno, richiesta_cod, QS_Type == -1, copia, richiesteEsistenti, 0);
                            if (richiesta_cod > -1 && QS_Type == -1) {
                                await AggiornaRichiesteCarburanti(KendoDDL("ddlAzienda").value(), $("#benzinaTerzisti")[0].value, $("#gasolioTerzisti")[0].value, $("#gasolioSerraTerzisti")[0].value, $("#gasolioTerzistiAppro")[0].value, $("#benzinaTerzistiAppro")[0].value, $("#gasolioSerraTerzistiAppro")[0].value);
                            }
                            if (copia) {
                                Azione_Indietro();
                            }
                            mostraRichieste();
                            if (QS_Avanzamento == 1) {
                                $("#lbl_rimanenze").val("Carburante rimasto");
                            } else {
                                $("#lbl_rimanenze").val("Rimanenze anno precedente");
                            }

                            $("#btn_carica_richieste").hide();
                            $("#row_Fascicolo").hide();
                            $("#anno").prop('disabled', true);
                            $("#ddlAzienda").data("kendoDropDownList").enable(false);
                            if (QS_Anticipo == 0) {
                                if ($("#fascicolo").data("kendoDropDownList") != undefined) {
                                    $("#fascicolo").data("kendoDropDownList").enable(false);
                                }
                                $("#btn_PassaggioDiStato").show();
                            } else {
                                $("#btn_PassaggioDiStato").hide();
                            }

                            //Se siamo entrati in rendicontazione cooperativa/terzisti leggo i CUAA per cui ho fatto delle richieste
                            if (QS_Avanzamento == 1 && (QS_TipoAzienda == Cooperativa_Agricola || QS_TipoAzienda == Azienda_Terzista)) get_ListaCUAA_Richiesti(QS_TipoAzienda)
                        }

                    }
                } else {
                    kendo.alert("Specificare anno della richiesta");
                }
            } else {
                mostraRichieste();
                if (QS_Anticipo == 0) {
                    $("#btn_PassaggioDiStato").show();
                } else {
                    $("#btn_PassaggioDiStato").hide();
                }
            }

            let Pagina_Arrivo = Request_QueryString("fp");
            //if (Pagina_Arrivo != null && Pagina_Arrivo == "85" && QS_Avanzamento == 1) {
            //    $("#btn_ConsultaLavorazioniTerzisti").show();
            //} else {
            //    $("#btn_ConsultaLavorazioniTerzisti").hide();
            //}
            if (Pagina_Arrivo != null && Pagina_Arrivo == "85"
                && QS_Avanzamento == 1 && QS_Type == -1
                && QS_TipoAzienda == Cooperativa_Agricola || QS_TipoAzienda == Azienda_Terzista) {
                $("#btn_ConsultaLavorazioniTerzisti").hide();
            } else {
                $("#btn_ConsultaLavorazioniTerzisti").show();
            }
            if (gestioneRimanenzeAbilitata() === true) {
                $("#tab7").show();

                modifica_rimanenze = true;
                modifica_rimanenze_conferma = true;

                let pratiche_successive = false;
                if (QS_Richiesta > 0) {
                    if (modifica_assegnato === true || modifica_richiesto === true) {
                        dtPraticheSuccessive = null;
                        await LeggiPraticheSuccessive();
                        if (dtPraticheSuccessive !== null && dtPraticheSuccessive.length > 0) {
                            pratiche_successive = true;
                            let msgAvanz = "";
                            if (dtPraticheSuccessive[0].avanzamento_richiesta === 1) {
                                msgAvanz = "rendicontazione " + dtPraticheSuccessive[0].Anno + "/" + dtPraticheSuccessive[0].Numero
                            } else {
                                msgAvanz = "richiesta " + dtPraticheSuccessive[0].Anno + "/" + dtPraticheSuccessive[0].Numero
                            }
                            console.log("Esistono pratiche successive chiuse (" + msgAvanz + "): scheda gestione rimanenze non modificabile")
                        }
                    }
                }

                if (modifica_assegnato === false || pratiche_successive === true) {

                    modifica_rimanenze_conferma = false;

                    $("#TxtriassegnabiliGasolioConferma").attr('disabled', 'disabled');
                    $("#TxtriassegnabiliBenzinaConferma").attr('disabled', 'disabled');
                    $("#TxtriassegnabiliGasolio_SerraConferma").attr('disabled', 'disabled');

                    $("#TxtrecuperoacciseGasolioConferma").attr('disabled', 'disabled');
                    $("#TxtrecuperoacciseBenzinaConferma").attr('disabled', 'disabled');
                    $("#TxtrecuperoacciseGasolio_SerraConferma").attr('disabled', 'disabled');

                    $("#btn_ValorizzaConferma").hide();

                } else {

                    $("#TxtriassegnabiliGasolioConferma").removeAttr("disabled");
                    $("#TxtriassegnabiliBenzinaConferma").removeAttr("disabled");
                    $("#TxtriassegnabiliBenzinaConferma").removeAttr("disabled");

                    $("#TxtrecuperoacciseGasolioConferma").removeAttr("disabled");
                    $("#TxtrecuperoacciseBenzinaConferma").removeAttr("disabled");
                    $("#TxtrecuperoacciseGasolio_SerraConferma").removeAttr("disabled");

                    $("#btn_ValorizzaConferma").show();
                    $("#btn_ValorizzaConferma").click(
                        function () {
                            CopiaDatidaConfermare();
                        });

                }

                if (modifica_richiesto === false || pratiche_successive === true) {

                    modifica_rimanenze = false;

                    $("#TxtNonUtilizzatoGasolio").attr('disabled', 'disabled');
                    $("#TxtNonUtilizzatoBenzina").attr('disabled', 'disabled');
                    $("#TxtNonUtilizzatoGasolio_Serra").attr('disabled', 'disabled');

                    if (KendoMultisel("DdlCausalidelnonutilizzo") !== undefined) {
                        KendoMultisel("DdlCausalidelnonutilizzo").enable(false);
                    }

                    $("#TxtriassegnabiliGasolio").attr('disabled', 'disabled');
                    $("#TxtriassegnabiliBenzina").attr('disabled', 'disabled');
                    $("#TxtriassegnabiliGasolio_Serra").attr('disabled', 'disabled');

                    $("#TxtrecuperoacciseGasolio").attr('disabled', 'disabled');
                    $("#TxtrecuperoacciseBenzina").attr('disabled', 'disabled');
                    $("#TxtrecuperoacciseGasolio_Serra").attr('disabled', 'disabled');

                } else {

                    $("#TxtNonUtilizzatoGasolio").removeAttr("disabled");
                    $("#TxtNonUtilizzatoBenzina").removeAttr("disabled");
                    $("#TxtNonUtilizzatoGasolio_Serra").removeAttr("disabled");

                    if (KendoMultisel("DdlCausalidelnonutilizzo") !== undefined) {
                        KendoMultisel("DdlCausalidelnonutilizzo").enable(true);
                    }

                    $("#TxtriassegnabiliGasolio").removeAttr("disabled");
                    $("#TxtriassegnabiliBenzina").removeAttr("disabled");
                    $("#TxtriassegnabiliGasolio_Serra").removeAttr("disabled");

                    $("#TxtrecuperoacciseGasolio").removeAttr("disabled");
                    $("#TxtrecuperoacciseBenzina").removeAttr("disabled");
                    $("#TxtrecuperoacciseGasolio_Serra").removeAttr("disabled");

                }

                $("#panel-rimanenze").hide();

                Grigliarestituzioni("grdRestituzioni");
                GrigliaTrasferiti("grdTrasferimenti");

                if (QS_Avanzamento === 0) {
                    $("#panel-rimanenaze-riassegnabili-anno-prec").hide();
                    $("#lblsubTitolotraferimenti").html("Solo per trasferire quote di eccedenza anticipo.");
                }
                else
                    $("#lblsubTitolotraferimenti").html("Non si devono reinserire i trasferimenti già inseriti e approvati in richiesta.");
            }
            else
                $("#tab7").hide();
        });



    ////AGGIUNTO GLORIA
    $("#btn_cerca").hide();
    $("#row_Fascicolo1").attr("style", "display:none");

    $("#btn_proseguiAnticipo").hide();
    $("#row_percentuale").attr("style", "display:none");

    $("#btn_DettaglioAppezzamenti").click(
        function () {
            $("#btn_cerca").show();
            $("#row_Fascicolo1").attr("style", "display:block");
            DettaglioAppezzamenti();
        });

    //$("#btn_SintesiRichieste").click(
    //    function () {
    //        DettaglioSintesiRendicontazione();
    //    });
    $("#btn_SintesiUMA").click(
        function () {
            DettaglioSintesiUMA();
        });


    $("#btn_ConsultaLavorazioniTerzisti").click(
        function () {
            LeggiLavorazioniTerzista()
        });




    ////FINE AGGIUNTO GLORIA

    $("#btn_ripartizione").click(
        function () {
            RipartizioneAutomatica();
        });

    $("#btn_PassaggioDiStato").click(
        function () {
            gestionePassaggioDiStato();
        });

    $("#btn_anticipazioniTerzisti").click(
        function () {
            RecuperoAnticipazioni();
        }
    );

    $("#btn_aggiungi_colture").click(
        function () {
            aggiungiRigheFascicolo();
        });

    $("#btn_elimina_colture").click(
        function () {
            eliminaTuttoUF();
        });

    window.addEventListener('message', event => {
        if (verificaOriginSecondaria(window, window.origin, event)) {
            if ((typeof event.data == "string") && event.data.includes("RispostaStringa")) {
                let respMsg = JSON.parse(event.data);
                switch (respMsg.Tipo) {
                    case 'Profilazione_PassaggioStato':

                        $("#alertPassaggio").kendoAlert({
                            content: respMsg.RispostaStringa,
                            messages: {
                                okText: "OK"
                            },
                            actions: [{
                                text: "OK",
                                action: function (e) {
                                    chiudiWindowPassaggioDiStato("")
                                }
                            }]
                        }).data("kendoAlert").open();
                        break;
                    default:
                        console.log("Evento non gestito")
                }
            } else if ((typeof event.data == "string")  && (event.data.includes("FrameDocumentale"))) {
                $('#tab_documentale').data('kendoWindow').close()
            }
        }
    }       //alert("message")
    );


    $("#btn_crea_richiesta").hide();

    //$("#btn_crea_richiesta").click(
    //    function () {
    //        CreaNuovaRichiesta();
    //        $("#btn_crea_richiesta").hide();
    //    });

    $("#btn_salva").click(
        function () {
            AggiornaDati();
        });

    $("#btn_elimina_cdg").click(
        function () {
            Conferma_Delete_CDG();
        });

    $("#btn_refresh").click(
        function () {
            ConfiguraGrigliaDettagliZoo("tab_griglia_dettagliZoo");
        });

    //eventi di click pulsanti
    $("#btn_salva_esci").click(
        function () {
            AggiornaDati(true);
        });

    //eventi di click pulsanti
    $("#btn_avanti").click(
        function () {
            Avanti(true);
        });

    //eventi di click pulsanti
    $("#btn_indietro").click(
        function () {
            Indietro(true);
        });

    //eventi di click pulsanti
    $("#btn_trova_no_cdg").click(
        function () {
            Prossimo(true);
        });

    $("#btn_squadre").click(
        function () {

            var piva = getParameterByName('p');

            var url = "../AnalisiCostiProduzione/ImputazioneSquadre.aspx?p=" + piva + "&veg_cod=" + $(cVeg_Cod).val() + "&id_attivita=" + ID_Attivita_Base + "&lav_cod=" + $(cLav_Cod).val() + "&data_movimento=" + $('input[name$="txt_dataop"]').val() + "&origine=./GestioneCosti.aspx";

            $("#PaginaSquadre").attr("src", url);
            $("#iFrameSquadre").modal('toggle');
        });

    if (QS_Avanzamento == 1) {
        $("#lbl_rimanenze").html("Carburante rimasto");
        $("#ricButt").html("Nuova Rendicontazione");
        $("#noRichiestaBox").show();
    } else {
        $("#lbl_rimanenze").html("Rimanenze anno precedente");
        $("#ricButt").html("Inserisci Richiesta");
    }

    // Pulsanti non mostrati in visualizzazione
    if ($("input[name$='hf_UtenteAbilitatoScrittura']").val() == "False") {
        $("#btn_salva_esci").hide();
        $("#btn_salva").hide();
        $("#btn_refresh").hide();
        $("#btn_ripartizione").hide();
        $("#btn_trova_impianti").hide();
        $("#btn_elimina_cdg").hide();
    }


    if (QS_Piva !== "") {
        inizializzaAnno()

        KendoDDL("ddlAzienda").value(QS_Piva);
        KendoDDL("ddlAzienda").trigger("change");
        ddlAzienda_Change();
        if (QS_Richiesta > 0) {
            KendoDDL("ddlAzienda").enable(false);
        }
    } else {
        inizializzaAnno()

        // Elimino la tab per la stampa del verbale di istruttoria, in quanto sono in inserimento della richiesta
        tabstrip.remove(".Verb");
    }


    //Nascondo pulsante Allegati
    if (richiesta_cod == -1) {
        //    $("#btn_carica_allegati").attr("style", "display:none");
        //    $("#btn_nuovo_allegato").attr("style", "display:none");
    }



    $("#divDati").show();

    if (QS_Type == -1) {
        if (QS_Avanzamento == 0) {
            $("#panel-carburantiTerzisti").show();
        }
        tabstrip.remove("li:nth-child(1)");
        tabstrip.remove(".All"); // Rimuovo la tab degli Allevamenti, in quanto non gestiti dalla richiesta conto terzi
        $(".Terz").addClass(GIAS_K_STATE_ACTIVE).addClass("k-tab-on-top");
        $(".terzisti").addClass(GIAS_K_STATE_ACTIVE)

        document.getElementsByClassName("terzisti")[0].style.display = "block";
        //$("#btn_carica_richieste").click();

        document.getElementById("CTRL_Azienda").innerHTML = (QS_TipoAzienda == Azienda_Terzista ? "Terzista" : "Cooperativa")
    } else {
        $("#carburantiTerzisti").hide();
        tabstrip.remove("li:nth-child(2)");
        $("#tab_griglia_terzisti").hide();
        $("#tab_griglia_terzistiparziale").hide(); //gloria
        //$("#btn_carica_richieste").click();
    }


    //per tab DOCUMENTI
    WaitFrame.show();

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            RichiestaDocumenti.push(readResxFile(resxSinglePath, "richiestaCarburanti_jQueryDocReady.js"));
        });
    }

    //let docRDataPrecaricata = await ws_leggiConfigurazione();

    let tabstrip1 = $("#tabstrip_elenco").kendoTabStrip({
        select: onActivate,
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    }).data("kendoTabStrip");

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    //Nascondo Tab Linee Produzioni
    $("#tabstrip_elenco").show();

    WaitFrame.hide();

    //eventi di click pulsanti
    $("#btnNuovoDocumento").click(
        function () {
            NuovoDocumento();
        }
    );

    //COMMENTATO...LO FACCIO VEDERE SOLO QUANDO CARICO EFFETTIVAMENTE LA GRIGLIA DEI DOCUMENTI
    //$("#btn_carica_allegati").show();

    CaricaLavNoteObbligatorie();

    //eventi di click pulsanti
    $("#btn_stampa_rich_rendicon").on("click", btnStampaRichRendiconClick);

    if ($(cAbilitato_AnalisiTerreno).val().toLowerCase() == "true" && QS_Type !== -1)
        //Se l'utente ha il permesso mostro il pulsante
        $("#btn_apri_frame_analisi_terreno").show()

    console.log("DocReady1 FINE")
}

async function mostraRichieste() {

    if (richiesta_cod > 0) {
        await PopolaValori_Setup($("#anno").val());
        if (QS_Type != -1) {
            WaitFrame.show();
            $("#tab_griglia_dettagliImpianti").show();
            await LeggiRichiesta(false);
            //await LeggiRichiestaDettaglio(false);
            await ConfiguraGrigliaDettagliImpianti("tab_griglia_dettagliImpianti", false);
            await PopolaGrigliaMacchine(KendoDDL("ddlAzienda").value(), richiesta_cod);
            ucUmaAllevamenti_initGrid();
            await ddlFascicoli_Load('#ddlFascicolo_UF')
            if ($("#anno").val() >= 2025) {
                $("#ddlFascicolo_UF").data("kendoDropDownList").value(-1)
            }
            leggiGrigliaColtureUF("tab_griglia_UFcolture")
            if (QS_Anticipo == 1) {//todo da controllare 
                $("#tab_griglia_dettagliImpianti").hide();
                $('#Div_UMA_Terzisti1').hide();
                $('#Div_UMA_Macchine').hide();
                $('#Div_UMA_Allevamenti').hide();
                $('#Div_UMA_Documenti').hide(); //aggiunto Gloria
                $("#tab_griglia_documenti").hide();
                $("Div_UMA_TerzistiParziale").hide();


            } else {
                $('#Div_UMA_Terzisti1').show();

            }

            //$("#btn_carica_richieste").hide();
        } else {
            $("#panel-permesso-acqua").hide();
            if (showPanels == "true" && QS_Avanzamento == 0) {
                $("#panel-carburantiTerzisti").show();
                if (QS_Anticipo == 0) {
                    $("#panel-carburantiTerzisti-Approvato").show();
                }
            }
            $("#tab_griglia_terzisti").show();
            if (QS_Anticipo == 0) {
                await LeggiRichiesteTerzista(KendoDDL("ddlAzienda").value());
            }

            //Se siamo entrati in rendicontazione cooperativa/terzisti leggo i CUAA per cui ho fatto delle richieste
            //.log("prima cuaa2")
            if (QS_Avanzamento == 1 && (QS_TipoAzienda == Cooperativa_Agricola || QS_TipoAzienda == Azienda_Terzista)) get_ListaCUAA_Richiesti(QS_TipoAzienda)
            if (QS_Avanzamento == 1) Leggi_Date_Ins_Rendicontazione()

            //ddlAzienda_Change();
            if (!(modifica_richiesto || modifica_assegnato))
                $("#btn_salva").hide();
            else
                $("#btn_salva").show();
            await PopolaGrigliaMacchine(KendoDDL("ddlAzienda").value(), richiesta_cod);
            //await NuovaRichiestaTerzistiGriglia();

            //INIZIO aggiunta x far visualizzare inserimento lavorazioni GLORIA
            if (QS_Avanzamento == 0 && QS_TipoAzienda != Cooperativa_Agricola) {
                if ($(".Terz").hasClass(GIAS_K_STATE_ACTIVE)) {
                    tabstrip.remove(".Terz");
                    $(".TerzParz").addClass(GIAS_K_STATE_ACTIVE).addClass("k-tab-on-top");

                    $(".terzistilavparziali").addClass(GIAS_K_STATE_ACTIVE)
                    document.getElementsByClassName("terzistilavparziali")[0].style.display = "block";
                    //tabstrip.remove("li:nth-child(1)");

                    await popolaGrigliaDettagliLavorazioniParzialiTerzisti(KendoDDL("ddlAzienda").value(), richiesta_cod);
                    //WaitFrame.show();

                    $("#Div_UMA_TerzistiParziale").show();
                }
                //} else if (QS_Avanzamento == 0 && QS_TipoAzienda == Cooperativa_Agricola) {
                //    await popolaGrigliaDettagliLavorazioniTerzisti(KendoDDL("ddlAzienda").value(), richiesta_cod);

                //    $("#Div_UMA_Terzisti1").show();
            } else {
                await NuovaRichiestaTerzistiGriglia(listaCUAARendicontati, consentitoAggiungereModificareRendicontazione_daSetup);
                tabstrip.remove(".TerzParz");
            }
            //FINE

        }

        /*TxtRimanenza_Gasolio.value(dtRichiestaTestata.Rimanenza_Gasolio);
        TxtRimanenza_Benzina.value(dtRichiestaTestata.Rimanenza_Benzina);
        TxtRimanenza_Gasolio_Serra.value(dtRichiestaTestata.Rimanenza_Gasolio_Serra);*/

        showPanels = await isPrimaRichiesta($("#anno").val(), richiesta_cod);

        if (showPanels == "false") {
            if (QS_Type == -1) {
                $("#panel-carburantiTerzisti").show();
                if (QS_Anticipo == 0) {
                    $("#panel-carburantiTerzisti-Approvato").show();
                }
            } else {
                $("#panel-carburantiTerzisti").hide();
                $("#panel-carburantiTerzisti-Approvato").hide();
            }
            $("#panel-rimanenze").hide();
            $("#panel-anticipazioni-colturali").hide();
            $("#panel-rimanenze-anno-prec").hide();
            gasolioTerz.value(0);
            benzinaTerz.value(0);
            gasolioSerraTerz.value(0);
            gasolioTotTerz = 0;
            benzinaTotTerz = 0;
            gasolioSerraTotTerz = 0;
        }

        if (QS_Avanzamento == 1) {
            $("#lbl_rimanenze").html("Carburante rimasto");
        } else {
            $("#lbl_rimanenze").html("Rimanenze anno precedente");
        }

        gestioneBottoniAllegati();

        switch ($("#stato_pratica_cod").val()) {
            case In_Compilazione.toString():
                $("#panel-info-calcolato").hide();
                break;
            default:
                if (Percentuale_Decurtamento == 0) {
                    $("#panel-info-calcolato").hide();
                }
                else {
                    $("#info-calcolato-p").html("Il carburante calcolato viene decurtato del " + Percentuale_Decurtamento + "%")
                    if (QS_Anticipo != 1)
                        $("#panel-info-calcolato").show();
                }
                break;
        }

        var piva = KendoDDL("ddlAzienda").value();
        var pivaReale = await CercaPivaReale(piva);
        if (pivaReale != "")
            $('#piva').val(pivaReale)
        else
            $('#piva').val(piva)

        if (piva !== "") {
            var dettagliAzienda = await CercaDettagliAzienda(piva, QS_Richiesta);
            var esisteRichiesta = (QS_Richiesta > 0 || dettagliAzienda[0].cod > 0);
            nIscrizioneCameraDiCommercio[piva] = LeggiNumeroIscrizioneCdC(piva);
            if (nIscrizioneCameraDiCommercio[piva] == 0 && KendoDDL("ddlAzienda").dataItem().flagPubblica != 1) {
                //$(".notCdC").show();
                $(".errorInfo").show();
                $(".errorInfo").html("Iscrizione alla camera di commercio non trovata");
            } else {
                //$(".notCdC").hide();
                if (!$(".errorInfo").html().includes("modifica"))
                    $(".errorInfo").hide();
            }
            var descrAzienda = $('#anno');
            descrAzienda[0].defaultValue = esisteRichiesta ? dettagliAzienda[0].anno : new Date().getFullYear();
            var descrAzienda = $('#nDichiarazione');
            if (descrAzienda[0].defaultValue == "")
                descrAzienda[0].defaultValue = esisteRichiesta ? dettagliAzienda[0].nDichiarazione : "";
            var stato = $("#stato_pratica");
            stato.html(esisteRichiesta ? dettagliAzienda[0].stato_des : "");
            var stato_cod = $("#stato_pratica_cod");
            stato_cod[0].defaultValue = esisteRichiesta ? dettagliAzienda[0].stato_cod : "";

            var descrAzienda = $('#CUAA');
            descrAzienda[0].defaultValue = dettagliAzienda[0].CUAA;
            var descrAzienda = $('#citta');
            descrAzienda[0].defaultValue = dettagliAzienda[0].comDes;
            var descrAzienda = $('#piva');
            if (pivaReale != "")
                descrAzienda[0].defaultValue = pivaReale;
            else
                descrAzienda[0].defaultValue = piva;

            $("#stato-richiesta-par").html(dettagliAzienda[0].stato_des);

            TxtRimanenza_Gasolio.value(dettagliAzienda[0].Rimanenza_Gasolio);
            TxtRimanenza_Benzina.value(dettagliAzienda[0].Rimanenza_Benzina);
            TxtRimanenza_Gasolio_Serra.value(dettagliAzienda[0].Rimanenza_Gasolio_Serra);

            $("#TxtRimanenza_Gasolio_prec").val(dettagliAzienda[0].Rimanenza_Gasolio_prec);
            $("#TxtRimanenza_Benzina_prec").val(dettagliAzienda[0].Rimanenza_Benzina_prec);
            $("#TxtRimanenza_Gasolio_Serra_prec").val(dettagliAzienda[0].Rimanenza_Gasolio_Serra_prec);

            $("#TxtriassegnabiliGasolio").val(dettagliAzienda[0].Rim_Riass_Gasolio);
            $("#TxtriassegnabiliBenzina").val(dettagliAzienda[0].Rim_Riass_Benzina);
            $("#TxtriassegnabiliGasolio_Serra").val(dettagliAzienda[0].Rim_Riass_Gasolio_Serra);

            $("#TxtriassegnabiliGasolioConferma").val(dettagliAzienda[0].Rim_Riass_Conf_Gasolio);
            $("#TxtriassegnabiliBenzinaConferma").val(dettagliAzienda[0].Rim_Riass_Conf_Benzina);
            $("#TxtriassegnabiliGasolio_SerraConferma").val(dettagliAzienda[0].Rim_Riass_Conf_Gasolio_Serra);

            $("#TxtrecuperoacciseGasolio").val(dettagliAzienda[0].Rec_Acc_Dich_Gasolio);
            $("#TxtrecuperoacciseBenzina").val(dettagliAzienda[0].Rec_Acc_Dich_Benzina);
            $("#TxtrecuperoacciseGasolio_Serra").val(dettagliAzienda[0].Rec_Acc_Dich_Gasolio_Serra);

            $("#TxtrecuperoacciseGasolioConferma").val(dettagliAzienda[0].Rec_Acc_Conf_Gasolio);
            $("#TxtrecuperoacciseBenzinaConferma").val(dettagliAzienda[0].Rec_Acc_Conf_Benzina);
            $("#TxtrecuperoacciseGasolio_SerraConferma").val(dettagliAzienda[0].Rec_Acc_Conf_Gasolio_Serra);

            //$("#TxtNonUtilizzatoGasolio").val(dettagliAzienda[0].Rimanenza_Gasolio_prec - dettagliAzienda[0].Acquisto_Gasolio + dettagliAzienda[0].Gasolio_Tot);
            //$("#TxtNonUtilizzatoBenzina").val(dettagliAzienda[0].Rimanenza_Benzina_prec - dettagliAzienda[0].Acquisto_Benzina + dettagliAzienda[0].Benzina_Tot);
            //$("#TxtNonUtilizzatoGasolio_Serra").val(dettagliAzienda[0].Rimanenza_Gasolio_Serra_prec - dettagliAzienda[0].Acquisto_Gasolio_Serra + dettagliAzienda[0].Gasolio_Serra_Tot);

            $("#TxtNonUtilizzatoGasolio").val(dettagliAzienda[0].Rim_Dich_Gasolio);
            $("#TxtNonUtilizzatoBenzina").val(dettagliAzienda[0].Rim_Dich_Benzina);
            $("#TxtNonUtilizzatoGasolio_Serra").val(dettagliAzienda[0].Rim_Dich_Gasolio_Serra);

            if (KendoMultisel("DdlCausalidelnonutilizzo") === undefined) {
                creaKendoMultiselect("DdlCausalidelnonutilizzo", { read: Leggi_Causali_UMA }, "Causale_Des", "Causale_Cod");
                if (dettagliAzienda[0].Causale_Non_Utilizzo != null) {
                    KendoMultisel("DdlCausalidelnonutilizzo").value(dettagliAzienda[0].Causale_Non_Utilizzo.split("|"));
                }
            }
            if (modifica_rimanenze == false) {
                KendoMultisel("DdlCausalidelnonutilizzo").enable(false);
            }

            gasolioTotTerz = dettagliAzienda[0].Gasolio_Tot;
            benzinaTotTerz = dettagliAzienda[0].Benzina_Tot;
            gasolioSerraTotTerz = dettagliAzienda[0].Gasolio_Serra_Tot;

            gasolioTerz.value(dettagliAzienda[0].gasolio);
            benzinaTerz.value(dettagliAzienda[0].benzina);
            gasolioSerraTerz.value(dettagliAzienda[0].gasolioSerra);

            gasolioTerzAppro.value(dettagliAzienda[0].gasolioAppro);
            benzinaTerzAppro.value(dettagliAzienda[0].benzinaAppro);
            gasolioSerraTerzAppro.value(dettagliAzienda[0].gasolioSerraAppro);
            //nel caso di anticipo devo prendere i valori da delle variabili grlobali

            if (QS_Anticipo == 1) {

                if (QS_TipoOp == 2) {
                    var objSommeLt = {
                        somma_LtGasolioAssegnati: 0,
                        somma_LtGasolioSerraAssegnati: 0,
                        somma_LtGasolioBenzinaAssegnati: 0
                    }
                    var piva = QS_Piva;//$('#piva')[0];
                    leggiSommeCarburanti_daDB(objSommeLt, 0, "0000", piva)
                    carburante_Richiesto_Gasolio = objSommeLt.somma_LtGasolioAssegnati;
                    carburante_Richiesto_Benzina = objSommeLt.somma_LtGasolioBenzinaAssegnati;
                    carburante_Richiesto_Gasolio_Serra = objSommeLt.somma_LtGasolioSerraAssegnati;

                    $("#panel-carburantiTerzisti").show();

                    var anno = $("#anno").val();
                    var richieste = await ContaRichiesteWS(anno, 0, QS_Type == -1);
                    if (richieste.filter((r) => r.Stato_Cod != 2001).length == 0)
                    {
                        var perc = parseFloat(dettagliAzienda[0].Percentuale_Anticipo_Richiesta_da_Azienda * 100).toFixed(2);
                        $("#txtVarPercentualeAnticipoTerzisti").val(perc);

                        leggiAcquistatoAnnoPrecedente();

                        //modifica_richiesto = true;

                        $("#btn_salva").show();
                    }
                }

                gasolioTerz.value(carburante_Richiesto_Gasolio);
                benzinaTerz.value(carburante_Richiesto_Benzina);
                gasolioSerraTerz.value(carburante_Richiesto_Gasolio_Serra);
            }
            //else {

            /* }*/

            switch ($("#stato_pratica_cod").val()) {
                case "":
                case In_Compilazione.toString():
                    if (permesso_richiesta && consentitoAggiungereModificareRendicontazione_daSetup) {
                        //modifica_richiesto = true;
                        modifica_assegnato = false;
                        $("#btn_salva").show();
                    }
                    break;
                case Verifica_In_Corso.toString():
                    if (permesso_approvazione_richiesta) {
                        modifica_richiesto = false;
                        //modifica_assegnato = true;
                        $("#btn_AssegnaAutomaticamenteCarburante").show();
                        $("#btn_salva").show();
                    }
                    break;
                case Rinuncia.toString():
                    richiestaRinuncia = true;
                    break;
                default:
                    break;
            }

            if (QS_Anticipo != 1) {
                if (modifica_richiesto) {
                    gasolioTerz.enable(true);
                    benzinaTerz.enable(true);
                    gasolioSerraTerz.enable(true);
                } else {
                    gasolioTerz.enable(false);
                    benzinaTerz.enable(false);
                    gasolioSerraTerz.enable(false);
                }
            }

            if (QS_Anticipo == 1) {
                gasolioTerz = carburante_Richiesto_Gasolio;
                benzinaTerz = carburante_Richiesto_Benzina;
                gasolioSerraTerz = carburante_Richiesto_Gasolio_Serra;
            }

            if (modifica_assegnato) {
                benzinaTerzAppro.enable(true);
                gasolioTerzAppro.enable(true);
                gasolioSerraTerzAppro.enable(true);
            } else {
                benzinaTerzAppro.enable(false);
                gasolioTerzAppro.enable(false);
                gasolioSerraTerzAppro.enable(false);
            }
        }
    }
}

function creaRichiesta(anno, richiesta_cod, isTerz, copia, richiesteEsistenti, integrativa) {
    return new Promise(async function (resolve, reject) {
        if (integrativa == undefined || integrativa < 0) {
            integrativa = 0;
        }
        let recuperaRimanenzeAnnoPrecedente = false;
        if (richiesteEsistenti == 0 && QS_Avanzamento == 0 && integrativa == 0) {
            recuperaRimanenzeAnnoPrecedente = true;
        }

        //await CreaNuovaRichiestaWS(parseInt(anno), QS_Avanzamento, isTerz, recuperaRimanenzeAnnoPrecedente, integrativa);
        //var richiesta_temp = await CreaNuovaRichiestaWS(parseInt(anno), QS_Avanzamento, isTerz, recuperaRimanenzeAnnoPrecedente, integrativa);
        var richiesta_cod = await CreaNuovaRichiestaWS(parseInt(anno), QS_Avanzamento, isTerz, recuperaRimanenzeAnnoPrecedente, integrativa);

        let numero_richiesta = await LeggiNumeroRichiesta(richiesta_cod);

        if (copia) {
            await ws_CopiaRichiesta(richiesta_cod);
        }

        $('#nDichiarazione')[0].defaultValue = numero_richiesta;
        $("#anno").attr("disabled", "disabled");
        //if (QS_Anticipo == "1")
        //    QS_Richiesta = richiesta_temp;
        //else
        QS_Richiesta = richiesta_cod;

        await ddlAzienda_Change();
        if (isTerz && QS_Anticipo == 0) {
            $("#tab_griglia_terzisti").show();
            await LeggiRichiesteTerzista(KendoDDL("ddlAzienda").value());
            $("#btn_salva").show();
            await PopolaGrigliaMacchine(KendoDDL("ddlAzienda").value(), richiesta_cod);
            NuovaRichiestaTerzistiGriglia();
            setKendoSwitch("noRichiestaAnnoProxCheck", getKendoSwitch("noRichiestaAnnoProxCheck"));
            if (QS_Avanzamento == 0) {
                /* $("#tab_griglia_terzistiparzile").show();*/
                //await LeggiRichiesteTerzistaParziale(KendoDDL("ddlAzienda").value());
                //NuovaRichiestaTerzistiParzialeGriglia();
                //await detailInitGrigliaDettagliLavorazioniParzialeTerzisti;
                /*await popolaGrigliaDettagliLavorazioniParzialiTerzisti();*/
                await popolaGrigliaDettagliLavorazioniParzialiTerzisti(KendoDDL("ddlAzienda").value(), richiesta_cod);
            }
        } else {

            if (QS_Anticipo != 1) {//aggiunta id Gloria 

                $("#tab_griglia_dettagliImpianti").show();
                await LeggiRichiesta(true, copia);

                ConfiguraGrigliaDettagliImpianti("tab_griglia_dettagliImpianti", false);
                PopolaGrigliaMacchine(KendoDDL("ddlAzienda").value(), richiesta_cod);
            }
            //$("#btn_carica_richieste").hide();
        }

        ///aggiunto gloria per visualizazione nuova tab

        //fine aggiunta gloria

        var dettagliAzienda = await CercaDettagliAzienda(KendoDDL("ddlAzienda").value(), richiesta_cod);

        if (QS_Anticipo == 1) {

            $("#lbl_Richiesta").html("Anticipo (" + (dettagliAzienda[0].Percentuale_Anticipo_Richiesta_da_Azienda * 100).toString() + "% rispetto al prelevato dell'anno precedente).");
        }

        TxtRimanenza_Gasolio.value(dettagliAzienda[0].Rimanenza_Gasolio);
        TxtRimanenza_Benzina.value(dettagliAzienda[0].Rimanenza_Benzina);
        TxtRimanenza_Gasolio_Serra.value(dettagliAzienda[0].Rimanenza_Gasolio_Serra);
        //TxtRimanenza_Gasolio_prec.value(dettagliAzienda[0].gas);
        if (QS_Avanzamento == 1) {
            TxtRimanenza_Gasolio.enable(true);
            TxtRimanenza_Benzina.enable(true);
            TxtRimanenza_Gasolio_Serra.enable(true);
        }

        //Blocco la DDL Azienda
        KendoDDL("ddlAzienda").enable(false)

        resolve();
    });
}

function gestioneRimanenze() {
    TxtRimanenza_Gasolio.enable(false);
    TxtRimanenza_Benzina.enable(false);
    TxtRimanenza_Gasolio_Serra.enable(false);
}

function gestioneBottoniAllegati() {
    if (modifica_richiesto) {
        // $("#btn_nuovo_allegato").show();
    }

    $("#btn_carica_allegati").show();
}

function inizializzaAnno() {
    if ($('#anno').val() == "") {
        $('#anno')[0].defaultValue = new Date().getFullYear();
        anno_Change();
    }
}

function EUdate(data) {
    let eudate = data.split("/");
    let resDate = new Date();
    resDate.setYear(eudate[2])
    resDate.setMonth(eudate[1] - 1)
    resDate.setDate(eudate[0])
    return resDate;
}