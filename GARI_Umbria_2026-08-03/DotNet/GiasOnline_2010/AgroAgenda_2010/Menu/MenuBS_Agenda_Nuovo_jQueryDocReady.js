var permessoBrogliaccio;
var permessoRicetta;
var permessoQdC;
var permessoModello4;

var lav_cod_Modello4 = [3004, 3035, 3037]

var operazioniImpianto = false;
var obj_Agenda; 
//   MenuBS_Agenda_Nuovo_jQueryDocReady.js

$(document).ready(function () {

    docReady();

    TraduzioniDaServerEdInizializzaPaginaMenuBS_Agenda();
    
});


async function docReady() {
    permessoBrogliaccio = await Agro_LeggiPermessoUtente(usernameLoggato, enumBrogliaccio,enumModifica);
    permessoRicetta = await Agro_LeggiPermessoUtente(usernameLoggato, enumRicette,enumModifica);
    permessoQdC = await Agro_LeggiPermessoUtente(usernameLoggato, enumAgenda, enumModifica);
    permessoModello4 = await Agro_LeggiPermessoUtente(usernameLoggato, 99, 2);
}

var MenuBS_Agenda_NuovoResx = [];
var resxArrPath = [
    "Menu/App_LocalResources/MenuBS_Agenda_Nuovo.aspx.resx",
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "AgronicaCoreDataProvider.dll/AgronicaCoreDataProvider.Gias"
];

function TraduzioniDaServerEdInizializzaPaginaMenuBS_Agenda() {
    if (MenuBS_Agenda_NuovoResx.length == 0) {
        MenuBS_Agenda_NuovoResxLeggi();        
    }
}

/*
 * Ottiene le traduzioni dal server
 */
function MenuBS_Agenda_NuovoResxLeggi() {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            MenuBS_Agenda_NuovoResx.push(readResxFile(resxSinglePath, "MenuBS_Agenda_Nuovo_jQueryDocReady.js"));
        });
    }

    inizializzaPaginaMenuBS_Agenda();

}

function inizializzaPaginaMenuBS_Agenda() {


    if (Attiva_Menu_Agenda_Visualizzazione_Dettagli_Operazioni) {
        $("#divTipoGrigliaOperazioniContainer").show();
    }

    window.addEventListener("message", MenuBS_Agenda_Nuovo_ReceiveMessage, false);

    //Per evitare che premendo il tasto invio parta il tasto INDIETRO
    $(document).keypress(function (e) {
        if (e.which === 13 || event.keyCode === 13) {
            e.preventDefault();
        }
    });


    WaitFrame.show();

    obj_Agenda = JSON.parse(objP_agenda)

    if (GetURLParameter("OperazioniImpianto") == "1") {
        operazioniImpianto = true;
    }



    //Sistemo il pannello di filtro delle operazioni
    var dPreferiti = $.Deferred();
    var dPreferitiStampe = $.Deferred();
    var dGrigliaOperazioni = $.Deferred();
    var dCentri = $.Deferred();
    var dSpecie = $.Deferred();
    var dTipoOperazione = $.Deferred();
    var dImpianti = $.Deferred();
    var dNumElemInTab = $.Deferred();

    dPreferiti.done(() => { console.log("dPreferiti DONE"); })
    dPreferitiStampe.done(() => { console.log("dPreferitiStampe DONE"); })
    dGrigliaOperazioni.done(() => { console.log("dGrigliaOperazioni DONE"); })
    dCentri.done(() => { console.log("dCentri DONE"); })
    dSpecie.done(() => { console.log("dSpecie DONE"); })
    dTipoOperazione.done(() => { console.log("dTipoOperazione DONE"); })
    dImpianti.done(() => { console.log("dImpianti DONE"); })
    dNumElemInTab.done(() => { console.log("dNumElemInTab DONE"); })

    $.when(dPreferiti, dPreferitiStampe, dCentri, dSpecie, dGrigliaOperazioni, dNumElemInTab, dTipoOperazione, dImpianti).done(function () {
        WaitFrame.hide();
    });

    $("#panelbarPreferitiUl").kendoPanelBar().data("kendoPanelBar").expand($("#preferitiList"));

    

    impostaTabPreselezionata();
    mostraPannelloSX();
    caricaPreferiti(dPreferiti);
    inizializzaPannelloFiltri();
    ripristinaPreferenzeFiltriDateDaCookie();
    $(data_inizio_ClientID).datepicker();
    $(data_fine_ClientID).datepicker();

    if (operazioniImpianto) {
        $(data_inizio_ClientID).val("")
        $(data_fine_ClientID).val("")
    }

    popolaDdlRicette("#ddlRicette");

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    //Predispongo le combo ma la lettura viene fatta solo se ci si clicca sopra
    if (pivaAziendaSelezionata !== undefined && pivaAziendaSelezionata !== '') {
        popolaDdlOperazioni();
        popolaDdlCentri("#ddlCentri", dCentri);
        popolaDdlTipoOperazione("#ddlTipoOperazione", dTipoOperazione);
        //popolaDdlImpianti("#ddlImpianti", dImpianti);
        $.when(dCentri).done(function () {
            popolaDdlSpecie("#ddlSpecie", dSpecie);
            if (operazioniImpianto) {
                //$("#ddlSpecie").data("kendoDropDownList").enable(false);
            }
        });
        $.when(dSpecie).done(function () {
            popolaDdlImpianti("#ddlImpianti", dImpianti);
        });

        if (operazioniImpianto) {
            //$("#ddlCentri").data("kendoDropDownList").enable(false);
            //$("#ddlTipoOperazione").data("kendoMultiSelect").enable(false);
            //$("#ddlImpianti").data("kendoMultiSelect").enable(false);
            //$(data_inizio_ClientID).prop("disabled", true);
            //$(data_fine_ClientID).prop("disabled", true);
        }

    } else {
        dCentri.resolve();
        dSpecie.resolve();
        dTipoOperazione.resolve();
        dImpianti.resolve();
    }

    getWeather("today"); //Carico il meteo

    Carica_Elenco_Preferiti_Stampe(dPreferitiStampe);//Carico i preferiti delle stampe

    $.when(dSpecie, dTipoOperazione, dImpianti).done(function () {
        CaricaDatiGriglia(dGrigliaOperazioni);
        LeggiNumeroOperazioniInTab(dNumElemInTab);
    });
    
    //Mostro il menu delle utility solo se c'è almeno un elemento
    if ($("#menuUtility ul li").length > 0)
        $("#menuUtility").show();


    if (Request_QueryString("gis") === "true") {
        //nascondo tutto
        $(".agroTestataPersonalizzata").hide();
        $("#IntestazioneMenuBS2017").hide();
        $("#bottoniera").hide();
        $(".AgronicaFooter").hide();
        $("#AgroMeteoContainer").hide();
        $("#linkZoo").hide();
        //' Vanni: 4/6/2020: al momento mostro le schede "Brogliaccio", "Ricette" anche in GIS come chiesto da Fabrizio, poi apriremo anche ricette in modale da GIS come su quaderno di campagna
        //$("#linkBrogliaccio").hide();
        //$("#linkRicette").hide();

        //faccio click sulla scheda tutte .. (potrei aver selezionato una scheda non attiva in modalità GIS)
        $("#linkTutte").click();

        window.parent.MostraPannelloAgendaViaOpacity();

    }

    if (defaultTab == 5) {
        $("#linkMacchine").hide();
        $("#linkAudit").hide();
        $("#linkMagCont").hide();
        $("#linkColturali").hide();
        $("#linkBrogliaccio").hide();
        $("#linkRicette").hide();
        $("#linkTutte").hide();
    }

}


function MenuBS_Agenda_Nuovo_ReceiveMessage(event) {

    var a = window.location.protocol + "//" + window.location.host
    if (event.origin !== a) {
        return;
    }

    //console.log(event);
    if (event.data.tipo !== undefined) {
        switch (event.data.tipo.value) {
            case Enum_comunicazioneTipoMessaggio.CambioAzienda.value:
                btnAggiorna_click();
                break;
            case Enum_comunicazioneTipoMessaggio.AggiornaListaOperazioni.value:
                btnAggiorna_click();
                break;
            case Enum_comunicazioneTipoMessaggio.SelezionaOperazione.value:
                break;
            default:
        }
    }
    return;
}


function GetURLParameter(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
}