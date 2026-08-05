var url_MenuBS_WS = "MenuBS_WS.aspx";

//xFiltroAggiuntivo_colturali: "agenda.id_agenda in (" + ids + ")",
function KendoOperazioni_leggi(options, data_inizio, data_fine, sa_cod, veg_cod, deferred, TipoOperazione, impianti) {
    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var ricetta_cod = $(ricetta_cod_ClientID).val();

    var filtro = {
        TipoGriglia: "2",
        xFiltroAggiuntivo_colturali: "",
        txt_Data1: data_inizio,
        txt_Data2: data_fine,
        flag_TerrenoNudo: false,
        sa_cod: sa_cod,
        veg_cod: veg_cod,
        mode: mode,
        ricetta_cod: ricetta_cod,
        tipoOperazione: TipoOperazione,
        impianti: impianti
    };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaOperazioni", parametri,
        function (risposta) {

            $('#hdKendo_Valorizzazione').val(risposta.RispostaStringa);
            KendoOperazioni_inizializza("divKendoOperazioni");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}


function KendoOperazioniDestinazioneDettagli_leggi(options, data_inizio, data_fine, sa_cod, veg_cod, deferred, TipoOperazione, impianti, RaggruppaPerCampo) { 
    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var ricetta_cod = $(ricetta_cod_ClientID).val();

    var filtro = {
        TipoGriglia: "2",
        xFiltroAggiuntivo_colturali: "",
        txt_Data1: data_inizio,
        txt_Data2: data_fine,
        flag_TerrenoNudo: false,
        sa_cod: sa_cod,
        veg_cod: veg_cod,
        mode: mode,
        ricetta_cod: ricetta_cod,
        tipoOperazione: TipoOperazione,
        impianti: impianti
    };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "', RaggruppaPerCampo: " + RaggruppaPerCampo + " }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaOperazioniDettagliDestinazioni", parametri,
        function (risposta) {

            $('#hdKendoOperazioniDettagliDestinazioni_Valorizzazione').val(risposta.RispostaStringa);
            KendoOperazioniDettagliDestinazioni_inizializza("divKendoOperazioniDettagliDestinazioni");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}

function KendoRicette_leggi(options, data_inizio, data_fine, sa_cod, veg_cod, deferred, TipoOperazione, impianti) {

    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var filtro = {
        txt_Data1: data_inizio,
        txt_Data2: data_fine,
        sa_cod: sa_cod,
        veg_cod: veg_cod,
        stato: 300,
        tipoOperazione: TipoOperazione,
        impianti: impianti
    };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaRicette", parametri,
        function (risposta) {

            $('#hdKendoRicette_Valorizzazione').val(risposta.RispostaStringa);
            KendoRicette_inizializza("divKendoRicette");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}

function KendoBrogliaccio_leggi(options, data_inizio, data_fine, sa_cod, veg_cod, deferred) {

    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var filtro = {
        txt_Data1: data_inizio,
        txt_Data2: data_fine,
        sa_cod: sa_cod,
        veg_cod: veg_cod,
        stato: 301
    };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaRicette", parametri,
        function (risposta) {

            $('#hdKendoBrogliaccio_Valorizzazione').val(risposta.RispostaStringa);
            KendoBrogliaccio_inizializza("divKendoBrogliaccio");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}

function KendoColturali_leggi(options, data_inizio, data_fine, sa_cod, veg_cod, deferred) {

    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var filtro = {
        txt_Data1: data_inizio,
        txt_Data2: data_fine,
        sa_cod: sa_cod,
        veg_cod: veg_cod
    };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaColturali", parametri,
        function (risposta) {

            $('#hdKendoColturali_Valorizzazione').val(risposta.RispostaStringa);
            KendoColturali_inizializza("divKendoColturali");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}

function KendoMagCont_leggi(options, data_inizio, data_fine, deferred) {

    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var filtro = { txt_Data1: data_inizio, txt_Data2: data_fine };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaMagCont", parametri,
        function (risposta) {

            $('#hdKendoMagCont_Valorizzazione').val(risposta.RispostaStringa);
            KendoMagCont_inizializza("divKendoMagCont");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}

function KendoAudit_leggi(options, data_inizio, data_fine, deferred) {

    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var filtro = { txt_Data1: data_inizio, txt_Data2: data_fine };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaAudit", parametri,
        function (risposta) {

            $('#hdKendoAudit_Valorizzazione').val(risposta.RispostaStringa);
            KendoAudit_inizializza("divKendoAudit");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}

function KendoMacchine_leggi(options, data_inizio, data_fine, deferred) {

    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var filtro = { txt_Data1: data_inizio, txt_Data2: data_fine };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaMacchine", parametri,
        function (risposta) {

            $('#hdKendoMacchine_Valorizzazione').val(risposta.RispostaStringa);
            KendoMacchine_inizializza("divKendoMacchine");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}

function KendoZoo_leggi(options, data_inizio, data_fine, deferred, sa_cod) {

    var GestioneWaitFrame = true;
    if (deferred !== undefined) { GestioneWaitFrame = false; }

    var filtro = {
        TipoGriglia: "2",
        txt_Data1: data_inizio,
        txt_Data2: data_fine,
        sa_cod: sa_cod
    };

    var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";
    ajaxAgronica(
        url_MenuBS_WS + "/CaricaZoo", parametri,
        function (risposta) {

            $('#hdKendoZoo_Valorizzazione').val(risposta.RispostaStringa);
            KendoZoo_inizializza("divKendoZoo");
            if (deferred !== undefined) {
                deferred.resolve();
            }
        }, null, undefined, GestioneWaitFrame, deferred
    );

}

function RiempiDdlOperazioni(options, Tipo_GruppoOperazioni) {
    let storage_key = "RiempiDdlOperazioni_" + Tipo_GruppoOperazioni;
    if (!storageExistItem(storage_key)) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "FiltraImpostazioniUtente": true, "Tipo_GruppoOperazioni": Tipo_GruppoOperazioni });

        ajaxAgronica(pathCoreWS + PAGINA_CORE_METASCHEMA_OPERAZIONI + "/CaricaComboLavorazioni",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                storageSetItem(storage_key, risposta.RispostaStringa);
                options.success(risp);
            }, null, undefined, false);
    } else {
        options.success(JSON.parse(storageGetItem(storage_key)));
    }
}

function RiempiDdlOperazioni_PerRicette(options) {
    let storage_key = "RiempiDdlOperazioni_PerRicette";
    if (!storageExistItem(storage_key)) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "FiltraImpostazioniUtente": true, "Tipo_GruppoOperazioni": "" });

        ajaxAgronica(pathCoreWS + PAGINA_CORE_METASCHEMA_OPERAZIONI + "/CaricaComboLavorazioni_PerRicette",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                storageSetItem(storage_key, risposta.RispostaStringa);
                options.success(risp);
            }, null, undefined, false);
    } else {
        options.success(JSON.parse(storageGetItem(storage_key)));
    }

}

function RiempiDdlOperazioni_PerBrogliaccio(options) {
    let storage_key = "RiempiDdlOperazioni_PerBrogliaccio";
    if (!storageExistItem(storage_key)) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "FiltraImpostazioniUtente": true, "Tipo_GruppoOperazioni": "" });

        ajaxAgronica(pathCoreWS + PAGINA_CORE_METASCHEMA_OPERAZIONI + "/CaricaComboLavorazioni_PerBrogliaccio",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                storageSetItem(storage_key, risposta.RispostaStringa);
                options.success(risp);
            }, null, undefined, false);
    } else {
        options.success(JSON.parse(storageGetItem(storage_key)));
    }

}

function RiempiDdlOperazioni_PerZoo(options) {
    let storage_key = "RiempiDdlOperazioni_PerZoo";
    if (!storageExistItem(storage_key)) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "FiltraImpostazioniUtente": true, "Tipo_GruppoOperazioni": "" });

        ajaxAgronica(pathCoreWS + PAGINA_CORE_METASCHEMA_OPERAZIONI + "/CaricaComboLavorazioni_PerZoo",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                storageSetItem(storage_key, risposta.RispostaStringa);
                options.success(risp);
            }, null, undefined, false);
    } else {
        options.success(JSON.parse(storageGetItem(storage_key)));
    }
}

function RiempiDdlCentri(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "PrimaRiga_Flag": false, "PrimaRiga_Text": "", "PrimaRiga_Value": "", "Piva": pivaAziendaSelezionata, "Flag_SoloCentriAttivi": false, "Tipo_Value": 2 });

    ajaxAgronica(pathCoreWS + "Anagrafica/CentroAziendale.asmx/LeggiCentriConFiltroUtente",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, undefined, false);
}

function RiempiDdlSpecie() {
    return new Promise((resolve, reject) => {
        var sa_cod = 0;
        //if ($('#ddlCentri').val() !== ""){
        //    sa_cod = $('#ddlCentri').val();
        //}
        var data_da = "01/01/1900";
        var data_a = "12/31/2100";
        var parametri = kendo.stringify({ "objP_server": objP_server, "PrimaRiga_Flag": false, "PrimaRiga_Text": "", "PrimaRiga_Value": "", "Piva": pivaAziendaSelezionata, "Sa_Cod": sa_cod, "Data_Da": data_da, "Data_A": data_a, "ConsideraTerrenoNudo": true, "leggiAncheBloccati": true });

        ajaxAgronica(pathCoreWS + "AgronicaCoreUtility/CaricaListControl.asmx/LeggiSpecieColtivate",
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp);
            }, null, undefined, false);
    });
}

function RiempiDdlTipoOperazione(options) {
    return new Promise((resolve, reject) => {
        let storage_key = "RiempiDdlTipoOperazione_1_0_0_1";

        //objP_utenti: objP_utenti,
        if (!storageExistItem(storage_key)) {
            var parametri = kendo.stringify({
                objP_server: objP_server,
                Flag_OpColturali: true,
                Flag_OpZoo: false,
                Flag_OpMacchine: false,
                Flag_OpContabili: true
            });

            ajaxAgronica(pathCoreWS + "Metaschema/Operazioni.asmx/Leggi_GruppoOperazioni",
                parametri,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    storageSetItem(storage_key, risposta.RispostaStringa);
                    resolve(risp);
                }, null, undefined, false);
        } else {
            resolve(JSON.parse(storageGetItem(storage_key)));
        }

    });

}

function RiempiDdlImpianti(options) {

    let kendoSpecie = $("#ddlSpecie").data("kendoDropDownList");
    let kendoCentri = $("#ddlCentri").data("kendoDropDownList");
    let resp = new Array();
    if (kendoSpecie !== undefined) {
        if (
            (kendoSpecie.value() !== "0" || kendoSpecie.value() !== "") &&
            (kendoCentri.value() !== "0" || kendoCentri.value() !== "")) {

            if (operazioniImpianto && kendoSpecie.value() == "-1") {
                return options.success(resp);
            }

            let sa_cod = kendoCentri.value();
            let veg_cod = kendoSpecie.value();
            let dataInizio = kendo.parseDate($(data_inizio_ClientID).val());
            let dataFine = kendo.parseDate($(data_fine_ClientID).val());

            if (dataInizio == null) {
                dataInizio = kendo.parseDate("01/01/1900");
            }

            if (dataFine == null) {
                dataFine = kendo.parseDate("31/12/2100");
            }

            if (veg_cod == '-1') {
                veg_cod = '0';
            }

            var parametri = kendo.stringify({
                piva: pivaAziendaSelezionata,
                Sa_Cod: sa_cod,
                Veg_Cod: veg_cod,
                Data_Inizio: dataInizio,
                Data_Fine: dataFine
            });

            ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/LeggiImpianti",
                parametri,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    //risp.unshift({
                    //    "gru_cod": 0,
                    //    "gru_des": "Tutte",
                    //    "tipo": ""
                    //});
                    options.success(risp);
                }, null, undefined, false);

        }
    }
    options.success(resp);
}

function Leggi_Operazioni_Preferite(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
    ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, undefined, false);

}

function Leggi_Operazioni_Preferite_PerGruppoOperazioni(options, gruppoOperazioni) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "gruppoOperazioni": gruppoOperazioni });
    ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite_PerGruppoOperazioni",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, undefined, false);

}

function Leggi_Operazioni_Preferite_PerRicette(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
    ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite_PerRicette",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, undefined, false);

}

function Leggi_Operazioni_Preferite_PerBrogliaccio(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
    ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite_PerBrogliaccio",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, undefined, false);

}

function Leggi_Operazioni_Preferite_PerZoo(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
    ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite_PerZoo",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, undefined, false);
}

function Salva_Operazioni_Preferite(nuovoElemento) {
    let idLblPreferiti = estraiIDControlloLSB();
    var lsb = $(idLblPreferiti).data("kendoListBox");
    var preferiti = lsb.dataItems();
    if (nuovoElemento !== undefined) {
        preferiti.push({ "lav_cod": nuovoElemento });
    }

    var valore = $.map(preferiti, function (obj) {
        return obj.lav_cod;
    }).join('|');

    if (valore === "") {
        valore = "notSet";
    }

    var impostazione_cod = 60; //enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE
    var parametri = kendo.stringify({ "objP_Utenti": objP_utenti, "impostazione_cod": impostazione_cod, "valoreDaSalvare": valore });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_W.asmx/Set_Impostazioni",
        parametri, true,
        function (risposta) {
            risp = risposta.RispostaStringa;

            //Ricarico i preferiti
            lsb.dataSource.read();
            //Ricarico la dropdown delle operazioni
            $("#ddlOperazioni").data("kendoDropDownList").dataSource.read();

            //Distruggo e resetto le listbox degli altri preferiti
            distruggiRicreaListboxKendo("#lsbPreferitiZoo", "#pnlPreferitiZoo", '<select id="lsbPreferitiZoo"></select>');

        }, null);
}

function Salva_OperazioniZoo_Preferite(nuovoElemento) {
    let idLblPreferiti = estraiIDControlloLSB();
    var lsb = $(idLblPreferiti).data("kendoListBox");
    var preferiti = lsb.dataItems();

    preferiti.push({ "lav_cod": nuovoElemento });

    var valore = $.map(preferiti, function (obj) {
        return obj.lav_cod;
    }).join('|');

    if (valore === "") {
        valore = "notSet";
    }

    var impostazione_cod = 207; //enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_ZOO_PREFERITE
    var parametri = kendo.stringify({ "objP_Utenti": objP_utenti, "impostazione_cod": impostazione_cod, "valoreDaSalvare": valore });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_W.asmx/Set_Impostazioni",
        parametri, true,
        function (risposta) {
            risp = risposta.RispostaStringa;

            //Ricarico i preferiti
            lsb.dataSource.read();

            //Ricarico la dropdown delle operazioni
            let storage_key = "RiempiDdlOperazioni_PerZoo";
            if (storageExistItem(storage_key)) {
                storageRemoveItem(storage_key);
            }
            $("#ddlOperazioniZoo").data("kendoDropDownList").dataSource.read();
            $("#ddlOperazioniZoo").data("kendoDropDownList").select(-1);
            $("#ddlOperazioniZoo").data("kendoDropDownList").trigger("change");

            //Distruggo e resetto le listbox degli altri preferiti
            distruggiRicreaListboxKendo("#lsbPreferiti", "#pnlPreferiti", '<select id="lsbPreferiti"></select>');
        }, null);
}

function apriLogImportazione(msgErrore) {
    $(document.body).append('<div id="windowImportazioneDatiAPP"></div>');
    var kendoWindow = $('#windowImportazioneDatiAPP').kendoWindow({
        title: "Importazione Dati APP",
        modal: true,
        resizable: true,
        width: 600,
        height: 300,
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () {
                $('#windowImportazioneDatiAPP').kendoWindow('destroy');
            }, 200);
        }
    }).data("kendoWindow");
    kendoWindow.content(msgErrore);
    kendoWindow.center().open();
}

function ImportaAgendaDaTabelleAPP(importaSoloAziendaSelezionata) {

    $("#btnImportaAgendaDaTabelleAPP").addClass("disabled");

    var o;
    var endPointCarica;

    if (importaSoloAziendaSelezionata) {

        endPointCarica = "ImportaAgendaAziendaDaTabelleAPP";
        o = {
            piva: pivaAziendaSelezionata,
            objP_server: objP_server
        };

       
    } else {

        endPointCarica = "ImportaAgendaDaTabelleAPP";
        o = {            
            objP_server: objP_server
        };
    }

    ajaxAgronica(pathCoreWS + "Contab/Ricette.asmx/" + endPointCarica,
        kendo.stringify(o),
        function (risposta) {

            $("#btnImportaAgendaDaTabelleAPP").removeClass("disabled");


            var risp = risposta.RispostaStringa;
            apriLogImportazione(risp + "<br>Verranno ora ricaricate le operazioni di agenda.");

            //Pulisco le griglie delle op. di agenda
            distruggiGrigliaKendo('#divKendoOperazioni');

            //Ricarico la griglia alla luce dei nuovi caricamento
            CaricaDatiGriglia();

        }, null);

}

function CaricaAgendaDaTabelleAPP(importaSoloAziendaSelezionata) {

    $("#btnImportaAgendaDaTabelleAPP").addClass("disabled");

    let piva = importaSoloAziendaSelezionata ? pivaAziendaSelezionata : "";

    ajaxAgronica(pathCoreWS + "GiasApp/SincroDatiApp.asmx/CaricaDatiApp",

        kendo.stringify({ "tipo": "20,50,51", "piva": piva, "objP_super_server": objP_super_server, "objP_server": objP_server, "objP_utenti": objP_utenti }),

        function (risposta) {

            $("#btnImportaAgendaDaTabelleAPP").removeClass("disabled");

            var risp = risposta.RispostaStringa;
            apriLogImportazione(risp + "<br>Verranno ora ricaricate le operazioni di agenda.");

            //Pulisco le griglie delle op. di agenda
            distruggiGrigliaKendo('#divKendoOperazioni');

            //Ricarico la griglia alla luce dei nuovi caricamento
            CaricaDatiGriglia();

        }, null);

}

function CaricaRicetteDaTabelleAPP(importaSoloAziendaSelezionata) {

    $("#btnImportaRicetteDaTabelleAPP").addClass("disabled");

    let piva = importaSoloAziendaSelezionata ? pivaAziendaSelezionata : "";

    ajaxAgronica(pathCoreWS + "GiasApp/SincroDatiApp.asmx/CaricaDatiApp",
        
        kendo.stringify({ "tipo": "10,12", "piva": piva, "objP_super_server": objP_super_server, "objP_server": objP_server, "objP_utenti": objP_utenti }),

        function (risposta) {

            $("#btnImportaRicetteDaTabelleAPP").removeClass("disabled");

            var risp = risposta.RispostaStringa;
            apriLogImportazione(risp + "<br>Verranno ora ricaricate le operazioni di brogliaccio.");

            //Pulisco le griglie delle ricette
            distruggiGrigliaKendo('#divKendoRicette');
            distruggiGrigliaKendo('#divKendoBrogliaccio');

            //Ricarico la griglia alla luce dei nuovi caricamento
            CaricaDatiGriglia();

        }, null);
}

function ImportaRicetteDaTabelleAPP(importaSoloAziendaSelezionata) {

    $("#btnImportaRicetteDaTabelleAPP").addClass("disabled");

    if (importaSoloAziendaSelezionata) {

        ajaxAgronica(pathCoreWS + "Contab/Ricette.asmx/ImportaRicetteAziendaDaTabelleAPP",
            kendo.stringify({ "piva": pivaAziendaSelezionata, "objP_server": objP_server }),
            function (risposta) {

                $("#btnImportaRicetteDaTabelleAPP").removeClass("disabled");

                var risp = risposta.RispostaStringa;
                apriLogImportazione(risp + "<br>Verranno ora ricaricate le operazioni di brogliaccio.");

                //Pulisco le griglie delle ricette
                distruggiGrigliaKendo('#divKendoRicette');
                distruggiGrigliaKendo('#divKendoBrogliaccio');

                //Ricarico la griglia alla luce dei nuovi caricamento
                CaricaDatiGriglia();

            }, null);

    } else {

        ajaxAgronica(pathCoreWS + "Contab/Ricette.asmx/ImportaRicetteDaTabelleAPP",
            kendo.stringify({ "objP_server": objP_server }),
            function (risposta) {

                $("#btnImportaRicetteDaTabelleAPP").removeClass("disabled");

                var risp = risposta.RispostaStringa;
                kendo.alert(risp + "<br>Verranno ora ricaricate le operazioni di brogliaccio.");

                //Pulisco le griglie delle ricette
                distruggiGrigliaKendo('#divKendoRicette');
                distruggiGrigliaKendo('#divKendoBrogliaccio');

                //Ricarico la griglia alla luce dei nuovi caricamento
                CaricaDatiGriglia();

            }, null);
    }

}

function LeggiNumeroOperazioniInTab(deferred) {

    //var GestioneWaitFrame = true;
    //if (deferred !== undefined) { GestioneWaitFrame = false; }

    //var data_inizio = $(data_inizio_ClientID).val();
    //var data_fine = $(data_fine_ClientID).val();
    //var sa_cod = $('#ddlCentri').val();
    //var veg_cod = $('#ddlSpecie').val();

    //var filtro = { txt_Data1: data_inizio, txt_Data2: data_fine, sa_cod: sa_cod, veg_cod: veg_cod};
    //var parametri = "{ filtro: '" + JSON.stringify(filtro) + "' }";

    //ajaxAgronica(url_MenuBS_WS + "/LeggiNumeroOperazioniInTab",
    //    parametri,
    //    function (risposta) {
    //        let risp = risposta.RispostaStringa;
    //        let lista = JSON.parse(risp);

    //        for (var k in lista) {
    //            let nomeSpan = "";
    //            switch (lista[k].nome_tab) {
    //                case "op_TutteQdc":
    //                    nomeSpan = "#numElem_Qdc";
    //                    break;
    //                case "op_Colturali":
    //                    nomeSpan = "#numElem_opColturali";
    //                    break;
    //                case "op_MagCont":
    //                    nomeSpan = "#numElem_opMagCont";
    //                    break;
    //                case "op_Audit":
    //                    nomeSpan = "#numElem_opAudit";
    //                    break;
    //                case "op_Macchine":
    //                    nomeSpan = "#numElem_opMacchine";
    //                    break;
    //                case "op_Zoo":
    //                    nomeSpan = "#numElem_opZoo";
    //                    break;
    //                case "op_Ricette":
    //                    nomeSpan = "#numElem_Ricette";
    //                    break;
    //                case "op_Brogliaccio":
    //                    nomeSpan = "#numElem_Brogliaccio";
    //                    break;
    //                default:
    //                    continue;
    //            }
    //            //$(nomeSpan).text(" (" + lista[k].conteggio + ")");
    //            $(nomeSpan).text(lista[k].conteggio);
    //        }

    if (deferred !== undefined) {
        deferred.resolve();
    }

    //    }, null, undefined, GestioneWaitFrame, deferred);

}

//function LeggiPermessoUtenteP(username, id_attivita, id_operazione) {
//    return new Promise((resolve, reject) => {
//        var ActualDate = new Date();
//        var stringData = ActualDate.toLocaleDateString();
//        var param = {
//            UserName: username,
//            Id_Servizio: 5,
//            Id_Attivita: id_attivita,
//            Id_Operazione: id_operazione,
//            DataOraControllo: ActualDate,
//            xFiltroAggiuntivo: '',
//            objP_utenti: objP_utenti
//        };

//        ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/Controlla_Permessi_UtenteR", JSON.stringify(param),
//            function (risposta) {
//                resolve(JSON.parse(risposta.RispostaStringa.toLowerCase()));
//            }, null, null, false);
//    });
//}

function ImportaProdottiInterventiAPP(importaSoloAziendaSelezionata) {

    $("#btnImportaProdottiInterventiAPP").addClass("disabled");

    ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/LeggiProdottiInterventiAPP",
        kendo.stringify({ "piva": importaSoloAziendaSelezionata ? pivaAziendaSelezionata : "" }),
        function (risposta) {

            $("#btnImportaProdottiInterventiAPP").removeClass("disabled");

            var risp = risposta.RispostaStringa;

            if (risp == "") kendo.alert("Non ci sono prodotti nuovi da importare");
            else {
                kendo.confirm(risp + "<br><br>Vuoi procedere con la mappatura?").then(function () {
                    CodificaProdottiAPP();
                }, null);
            }

        }, null);

}

function CodificaProdottiAPP() {

    ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/CodificaProdottiAPP",
        kendo.stringify({ "piva": pivaAziendaSelezionata }),
        function (risposta) {
            // document.location = risposta.RispostaStringa;
            apriCodificaProdottiAPP(risposta.RispostaStringa);
        }, null);

}

function apriCodificaProdottiAPP(url) {
    $(document.body).append('<div id="codificaProdottiWindow"></div>');
    $('#codificaProdottiWindow').kendoWindow({
        title: "Codifica Prodotti APP",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () {
                $('#codificaProdottiWindow').kendoWindow('destroy');
            }, 200);
        }
    }).data('kendoWindow').center();
}

function chiudiCodificaProdottiAPP() {
    $('#codificaProdottiWindow').data('kendoWindow').close();
}

function check_permessoOperazione(ID_Attivita, ID_Operazione) {
    var permesso = true

    $.ajax({
        type: 'POST',
        url: "MenuBS_Agenda_Nuovo.aspx/check_permessoOperazione",
        data: kendo.stringify({ "ID_Attivita": ID_Attivita, "ID_Operazione": ID_Operazione }),
        contentType: 'application/json; charset=utf-8',
        cache: false, dataType: 'json', async: false,
        success: function (r) {
            if (!r.d.RispostaOK) {
                permesso = false
                MessaggioErrore(r.d.Errore);
            }
        }
    });

    return permesso
}
