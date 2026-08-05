
var resxScadCategorieTipologieUC = [];

var indirizzohttp = "./Scad_Anagrafiche";

function TraduciScadCategorieTipologieUCWSClient(chiave, testoAlternativo) {
    if (resxScadCategorieTipologieUC.length === 0) {
        resxScadCategorieTipologieUC.push(readResxFile("Scadenzario/UserControl/App_LocalResources/Scad_CategorieTipologie_UC.ascx.resx", "Scad_CategorieTipologie_UC_ws_client.js"));
        resxScadCategorieTipologieUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "Scad_CategorieTipologie_UC_ws_client.js"));
    }
    return TraduzioneMultiResx(resxScadCategorieTipologieUC, chiave, testoAlternativo);
}

//function RiempiDdlAzienda(options) {

//    var ddl = $('#ddlAzienda').data("kendoDropDownList");
//    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });

//    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtente",
//        parametri,
//        false,
//        function (risposta) {
//            risp = JSON.parse(risposta.RispostaStringa);
//            options.success(risp);
//        }, null);
//}

function LsbAree_Load() {

    //pulisco la listbox
    $("#LsbAree").find('option').remove();
    $("#LsbTipologie").find('option').remove();

    //Pulisco gli altri controlli (non essendo selezionato nulla, la funzione ripulisce tutto)
    LsbAree_onchange();

    //Estraggo la partita iva selezionata
    //var piva = $('#ddlAzienda').val();

    var piva = $(cIdPiva).val();
    var Filtro = "";

    if (piva == undefined || piva == "") {
        return;
    }

    var param = kendo.stringify({ "objP_server": objP_server, "piva": piva, "soloPrivate": false, "controllaSeUtenteAutorizzato": false, "tipoPermessoDaControllare": 2, "Filtro": Filtro });

    //Leggo l'anagrafica delle aree, senza controllare le autorizzazioni dell'utente
    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Area.asmx/Leggi_AreePerAzienda", param, false,
        function (risposta) {
            var lista = JSON.parse(risposta.RispostaStringa);

            //genero il codice HTML
            var strHtml = "";
            for (var i = 0; i < lista.length; i++) {
                strHtml += "<option value='" + lista[i]["id_area"] + "'>" + lista[i]["nome"] + "</option>";
            }

            //aggiungo l'HTML al controllo
            $("#LsbAree").html(strHtml);

        }, null);


}

//Se seleziono l'area di una categoria, carico le sue tipologie
function LsbAree_onchange() {

    $("#accessori_tipologia").hide();
    //Estraggo nome e valore di ciò che è selezionato
    var nomeArea = $("#LsbAree").find('option:selected').text();
    var valoreArea = $("#LsbAree").val();

    //Imposto le aree in base alla selezione, senza controllare le autorizzazioni dell'utente
    if (valoreArea != null) {

        //Carico le tipologie
        var parametri = kendo.stringify({
            "objP_server": objP_server, "piva": "",
            "soloPrivate": false,
            "id_area": valoreArea,
            "controllaSeUtenteAutorizzato": false,
            "tipoPermessoDaControllare": 2,
            "listaIndici": "",
            "xMultiSelect": false,
            "id_tipologia": 0
        });

        ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Tipologia.asmx/Leggi_Tipologie', parametri, false,
            function (risposta) {
                var lista = JSON.parse(risposta.RispostaStringa);

                //pulisco la combo
                $("#LsbTipologie").find('option').remove();

                //genero il codice HTML
                var strHtml = "";
                for (var i = 0; i < lista.length; i++) {
                    strHtml += "<option value='" + lista[i]["id_tipologia"] +
                        "' data-utilizzo_giasapp ='" + lista[i]["utilizzo_giasapp"] +
                        "' data-flagdatascadenzaobbligatoria ='" + lista[i]["FlagDataScadenzaObbligatoria"] +
                        "' data-datadefault ='" + lista[i]["DataDefault"]
                        + "'>" + lista[i]["nome"] + "</option>";
                }

                //aggiungo l'HTML al controllo
                $("#LsbTipologie").html(strHtml);

            }, null);

        //Se è stato selezionato qualcosa...
        $("#TxtNomeArea").val(nomeArea);

        //Se  l'Area è maggiore di 100 allora è stata creata da interfaccia altrimenti è un'Area creata da migra
        if (UtenteAbilitatoScrittura && valoreArea > 100) {
            $("#BtnAree_Edit").show();
            $("#BtnAree_Del").show();
        } else {
            $("#BtnAree_Edit").hide();
            $("#BtnAree_Del").hide();
        }

        if (UtenteAbilitatoScrittura == false || valoreArea == 10 || valoreArea == 13) {
            //La tipologie utente documenti contabili e carichi e scarico di magazzino non sono modificabili altrimenti non carica l'id_agenda riferimento / non si può crearne di nuove
            $("#BtnTipologie_Add").hide();
            $("#TxtNomeTipologia").attr("disabled", true);
        } else {
            $("#BtnTipologie_Add").show();
            $("#TxtNomeTipologia").attr("disabled", false);
        }


        $("#Tipologie_utilizzabili_da_app").show();
        $("#accessori_tipologia").show();

        if (UtenteAbilitatoScrittura === false) {
            $("#switch_utilizzabile_da_app").data("kendoSwitch").enable(false);
            $("#TxtNomeArea").attr("disabled", true);
        }

    }
    else {

        //pulisco la combo
        $("#LsbTipologie").find('option').remove();

        //Se non è stato selezionato nulla...
        $("#TxtNomeArea").val("");

        $("#BtnAree_Edit").hide();
        $("#BtnAree_Del").hide();

        //Switch Tipologia Utilizzabile da App
        creaKendoSwitch($("#switch_utilizzabile_da_app").data("kendoSwitch"), undefined, undefined, false, undefined, undefined, undefined);

        $("#switch_utilizzabile_da_app").data("kendoSwitch").bind("change", AccessoriTipologia_change);
        setKendoSwitch("switch_utilizzabile_da_app", false);

        $("#Tipologie_utilizzabili_da_app").hide();
        $("#accessori_tipologia").hide();

        //Anna 14/04/22: aggiunto FlagDataScadenzaObbligatoria e DataDefault
        $("#DataDefault").val("");
        //Switch setta flag scadenza obbligatoria
        creaKendoSwitch($("#FlagDataScadenzaObbligatoria").data("kendoSwitch"), undefined, undefined, false, undefined, undefined, undefined);
        $("#FlagDataScadenzaObbligatoria").data("kendoSwitch").bind("change", AccessoriTipologia_change);
        setKendoSwitch("FlagDataScadenzaObbligatoria", false);

        //if (getKendoSwitch("FlagDataScadenzaObbligatoria")) {
        //    $("#DataDefault").data("kendoDatePicker").enable(true);
        //} else {
        //    $("#DataDefault").data("kendoDatePicker").enable(false);
        //}
    }

    //Pulisco le tipologie
    $("#TxtNomeTipologia").val("");

    //Anna 14/04/22: aggiunto FlagDataScadenzaObbligatoria e DataDefault
    $("#DataDefault").val("");
    $("#FlagDataScadenzaObbligatoria").val(false);

    $("#BtnTipologie_Edit").hide();
    $("#BtnTipologie_Del").hide();
};

//$("#FlagDataScadenzaObbligatoria").kendoSwitch({
//    messages: {
//        checked: "SI",
//        unchecked: "NO"
//    },
//    change: function (e) {
//        //if (getKendoSwitch("FlagDataScadenzaObbligatoria")) {
//        //    $("#DataDefault").data("kendoDatePicker").enable(true);
//        //} else {
//        //    $("#DataDefault").data("kendoDatePicker").enable(false);
//        //};
//    }
//});

//PER MODIFICARE UNA TIPOLOGIA
function modificaTipologia() {

    //Estraggo i controlli dal pannello
    var obj_lsbTipologie = $("#LsbTipologie");
    var obj_txtTipologia = $("#TxtNomeTipologia");
    var obj_ddlAzienda = $("#ddlAzienda");

    //Anna 14/04/22: aggiunto FlagDataScadenzaObbligatoria e DataDefault
    var FlagDataScadenzaObbligatoria = 0;
    var obj_FlagDataScadenzaObbligatoria = getKendoSwitch("FlagDataScadenzaObbligatoria");
    var obj_DataDefault = $('input[name$="DataDefault"]');

    //Estraggo nome e id nuovi
    var idTipologia = $(obj_lsbTipologie).val();
    var nomeTipologia = $(obj_txtTipologia).val();
    //var piva = $(obj_ddlAzienda).val();
    var piva = $(cIdPiva).val();

    //Anna 14/04/22: aggiunto FlagDataScadenzaObbligatoria e DataDefault
    if (obj_FlagDataScadenzaObbligatoria)
        FlagDataScadenzaObbligatoria = 1;

    var DataDefault = null
    if ($(obj_DataDefault).val() != "")
        DataDefault = $(obj_DataDefault).val() + "/1900";

    //Spostato lato client questo messaggio per poter modificare l'utilizzo gias app ancha sulle tipologie di sistema
    if (parseInt(idTipologia) < 0) {
        kendo.alert("Impossibile modificare una Tipologia di sistema.");
        return;
    }

    if (nomeTipologia === "") {
        kendo.alert("Inserire il nome della Tipologia.");
        return;
    }

    var utilizzo_GiasApp = 0;

    if (getKendoSwitch("switch_utilizzabile_da_app"))
        utilizzo_GiasApp = 1;

    //Salvo le modifiche
    var param = kendo.stringify({
        "objP_server": objP_server, "objP_utenti": objP_utenti,
        "ID_Tipologia": idTipologia,
        "Nome_Tipologia": nomeTipologia,
        "Piva": piva,
        "Utilizzo_GiasAPP": utilizzo_GiasApp,
        "DataDefault": DataDefault,
        "FlagDataScadenzaObbligatoria": FlagDataScadenzaObbligatoria
    });
    ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Tipologia.asmx/Modifica', param, false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            pulisciControlli()

            //Aggiorno i controlli
            LsbAree_onchange();
        }, null);

}

//PER AGGIUNGERE UNA TIPOLOGIA
function aggiungiTipologia() {

    //Estraggo i controlli dal pannello
    //var obj_ddlAzienda = $("#ddlAzienda");
    var obj_lsbAree = $("#LsbAree");
    var obj_txtTipologia = $("#TxtNomeTipologia");

    //Anna 14/04/22: aggiunto FlagDataScadenzaObbligatoria e DataDefault
    var FlagDataScadenzaObbligatoria = 0;
    var obj_FlagDataScadenzaObbligatoria = getKendoSwitch("FlagDataScadenzaObbligatoria");
    var obj_DataDefault = $('input[name$="DataDefault"]');

    //Estraggo i nomi nuovi
    var IDArea = $(obj_lsbAree).val();
    var nomeTipologia = $(obj_txtTipologia).val();
    //var piva = $(obj_ddlAzienda).val();

    //Anna 14/04/22: aggiunto FlagDataScadenzaObbligatoria e DataDefault
    if (obj_FlagDataScadenzaObbligatoria)
        FlagDataScadenzaObbligatoria = 1;

    var DataDefault = null
    if ($(obj_DataDefault).val() != "")
        DataDefault = $(obj_DataDefault).val() + "/1900";

    var piva = $(cIdPiva).val();

    if (piva === undefined || piva === "") {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("SelezionareUnAzienda", "Selezionare un'Azienda."));
        return;
    }

    if (nomeTipologia === "") {
        kendo.alert("Inserire il nome della Tipologia.");
        return;
    }

    if (jQuery.isNumeric(IDArea) === false) {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("SelezionareCategoriaPadreDellaNuovaTipologia", "Selezionare una Categoria padre della nuova Tipologia da aggiungere."));
        return;
    }

    var utilizzo_GiasApp = 0;

    if (getKendoSwitch("switch_utilizzabile_da_app"))
        utilizzo_GiasApp = 1;

    //Aggiungo l'elemento
    var param = kendo.stringify({
        "objP_server": objP_server, "objP_utenti": objP_utenti,
        "Piva": piva, "IDArea": IDArea,
        "NomeTipologia": nomeTipologia,
        "Utilizzo_GiasAPP": utilizzo_GiasApp,
        "DataDefault": DataDefault,
        "FlagDataScadenzaObbligatoria": FlagDataScadenzaObbligatoria
    });
    ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Tipologia.asmx/Aggiungi', param, false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            pulisciControlli()

            //Aggiorno i controlli
            LsbAree_onchange();
        }, null);
}

//PER CANCELLARE UNA TIPOLOGIA
function cancellaTipologia() {

    //Estraggo i controlli dal pannello
    //var obj_ddlAzienda = $("#ddlAzienda");
    var obj_lsbTipologie = $("#LsbTipologie");

    //Estraggo nome e id nuovi
    var idTipologia = $(obj_lsbTipologie).val();
    //var piva = $(obj_ddlAzienda).val();

    var piva = $(cIdPiva).val();

    if (piva == undefined || piva == "") {
        return;
    }

    //Salvo le modifiche
    var param = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "ID_Tipologia": idTipologia, "Piva": piva });
    ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Tipologia.asmx/Cancella', param, false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            //Aggiorno i controlli
            LsbAree_onchange();
        }, null);

}

//PER MODIFICARE UNA CATEGORIA
function modificaArea() {

    //Estraggo i controlli dal pannello
    //var obj_ddlAzienda = $("#ddlAzienda");
    var obj_lsbAree = $("#LsbAree");
    var obj_txtArea = $("#TxtNomeArea");

    //Estraggo nome e id nuovi
    var idArea = $(obj_lsbAree).val();
    var nomeArea = $(obj_txtArea).val();
    //var piva = $(obj_ddlAzienda).val();

    var piva = $(cIdPiva).val();

    if (nomeArea === "") {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("InserireNomeCategoria", "Inserire il nome della categoria"));
        return;
    }

    //Salvo le modifiche
    var param = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "ID_Area": idArea, "Nome_Area": nomeArea, "Piva": piva });
    ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Area.asmx/Modifica', param, false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            //rileggo l'anagrafica delle Aree
            LsbAree_Load();
        }, null);

}

//PER AGGIUNGERE UNA CATEGORIA
function aggiungiArea() {

    //Estraggo i controlli dal pannello
    //var obj_ddlAzienda = $("#ddlAzienda");
    var obj_txtArea = $("#TxtNomeArea");

    //Estraggo i nomi nuovi
    var nomeArea = $(obj_txtArea).val();
    //var piva = $(obj_ddlAzienda).val();

    var piva = $(cIdPiva).val();

    if (piva === undefined || piva === "") {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("SelezionareUnAzienda", "Selezionare un'Azienda."));
        return;
    }

    if (nomeArea === "") {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("InserireNomeCategoria", "Inserire il nome della categoria"));
        return;
    }

    //Aggiungo l'elemento
    var param = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "Piva": piva, "NomeArea": nomeArea });
    ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Area.asmx/Aggiungi', param, false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            //rileggo l'anagrafica delle Aree
            LsbAree_Load();
        }, null);

}

//PER CANCELLARE UNA CATEGORIA
function cancellaArea() {

    //Estraggo i controlli dal pannello
    //var obj_ddlAzienda = $("#ddlAzienda");
    var obj_lsbAree = $("#LsbAree");

    //Estraggo nome e id nuovi
    var IDArea = $(obj_lsbAree).val();
    //var piva = $(obj_ddlAzienda).val();

    var piva = $(cIdPiva).val();

    if (piva == undefined || piva == "") {
        return;
    }

    //Salvo le modifiche
    var param = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "ID_Area": IDArea, "Piva": piva });
    ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Area.asmx/Cancella', param, false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            //rileggo l'anagrafica delle Aree
            LsbAree_Load();
        }, null);

}

function ComponiMessaggioDiAvvisoCancellazioneAreaTipologia(ID_Area, ID_Tipologia) {

    //Compongo il messaggio per avvisare che verranno cancellate anche tutte le associazioni e le autorizzazioni
    //legate a quella Categoria/Tipologia.

    var messaggio = "";

    var param = kendo.stringify({ "ID_Area": ID_Area, "ID_Tipologia": ID_Tipologia });
    ajaxAgronicaSync(indirizzohttp + '/ComponiMessaggioDiAvvisoCancellazioneAreaTipologia', param, false,
        function (risposta) {
            messaggio = risposta.RispostaStringa
        }, null);

    return messaggio;
}

function AccessoriTipologia_change(e) {

    var obj_lsbTipologie = $("#LsbTipologie");
    var idTipologia = $(obj_lsbTipologie).val();


    if (UtenteAbilitatoScrittura && idTipologia !== undefined && idTipologia !== null && idTipologia !== "") {
        var piva = $(cIdPiva).val();

        var utilizzo_GiasApp = 0;
        if (getKendoSwitch("switch_utilizzabile_da_app"))
            utilizzo_GiasApp = 1;

        var FlagDataScadenzaObbligatoria = 0;
        if (getKendoSwitch("FlagDataScadenzaObbligatoria"))
            FlagDataScadenzaObbligatoria = 1;

        var DataDefault = ""
        var data = $("#DataDefault").val()
        if (data != "" && data != "day/month") {
            DataDefault = data + "/1900";
        }

        //Salvo le modifiche
        var param = kendo.stringify({
            "objP_server": objP_server,
            "objP_utenti": objP_utenti,
            "ID_Tipologia": idTipologia,
            "Nome_Tipologia": "",
            "Piva": piva,
            "Utilizzo_GiasAPP": utilizzo_GiasApp,
            "DataDefault": DataDefault,
            "FlagDataScadenzaObbligatoria": FlagDataScadenzaObbligatoria
        });

        modifica_Alert_Tipologia(param);

    }
}


function Utilizzabile_da_App_change(e) {

    //Permette di modificare se una tipologia è utilizzabile o meno ( anche le tipologie di sistema) 

    var obj_lsbTipologie = $("#LsbTipologie");

    var idTipologia = $(obj_lsbTipologie).val();

    if (UtenteAbilitatoScrittura && idTipologia !== undefined && idTipologia !== null && idTipologia !== "") {

        var piva = $(cIdPiva).val();

        var utilizzo_GiasApp = 0;

        if (getKendoSwitch("switch_utilizzabile_da_app"))
            utilizzo_GiasApp = 1;

        //Salvo le modifiche
        var param = kendo.stringify({
            "objP_server": objP_server,
            "objP_utenti": objP_utenti,
            "ID_Tipologia": idTipologia,
            "Nome_Tipologia": "",
            "Piva": piva,
            "Utilizzo_GiasAPP": utilizzo_GiasApp,
            "DataDefault": "",
            "FlagDataScadenzaObbligatoria": null
        });
        ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Tipologia.asmx/Modifica', param, false,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

                //Aggiorno i controlli
                LsbAree_onchange();
            }, null);
    }

}


function modifica_Alert_Tipologia(param) {
    ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Tipologia.asmx/Modifica', param, false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            pulisciControlli()

            //Aggiorno i controlli
            LsbAree_onchange();
        }, null);
}
// ##################################################################################################################################################################
// Anna 23/07/21: Aggiunta button per spostare documenti su DB
function spostaTipologia() {

    //Estraggo i controlli dal pannello
    var obj_lsbTipologie = $("#LsbTipologie");
    var obj_lsbAree = $("#LsbAree");
    var obj_txtTipologia = $("#TxtNomeTipologia");

    //Estraggo i nomi e id
    var IDArea = $(obj_lsbAree).val();
    var idTipologia = $(obj_lsbTipologie).val();
    var nomeTipologia = $(obj_txtTipologia).val();

    var piva = $(cIdPiva).val();

    if (piva === undefined || piva === "") {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("SelezionareUnAzienda", "Selezionare un'Azienda."));
        return;
    }

    if (nomeTipologia === "") {
        kendo.alert("Inserire il nome della Tipologia.");
        return;
    }

    if (jQuery.isNumeric(IDArea) === false) {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("SelezionareCategoriaPadreDellaNuovaTipologia", "Selezionare una Categoria padre della nuova Tipologia da aggiungere."));
        return;
    }

    var kendoConfirm = $("<div></div>").kendoConfirm({
        title: "Attenzione",
        messages: { okText: "Sì", cancel: "No" },
        content: ("Tutti i documenti della tipologia " + nomeTipologia.toUpperCase() + " saranno spostati da cartella di rete a database: Proseguire?")
    }).data("kendoConfirm");
    kendoConfirm.result.done(function () {
        var param = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "filtro_area": "", "filtro_tipologia": idTipologia });
        WaitFrame.show();
        ajaxAgronica(GetUrlLetturaTabelleGestionali() + 'AgronicaCoreScadenziario/Alert.asmx/spostaAllegatiSuDatabase',
            param,
            function (risposta) {
                WaitFrame.hide();

                //rileggo l'anagrafica delle Aree
                LsbAree_Load();

                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            }, function (risposta) {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");

                if (risposta.RispostaStringa !== "") {
                    alert("ATTENZIONE: I seguenti file sono stati spostati ma NON cancellati: " + risposta.RispostaStringa);
                }

                WaitFrame.hide();

                //rileggo l'anagrafica delle Aree
                LsbAree_Load();
            });
    });

    kendoConfirm.result.fail(function () {
        kendo.alert("Operazione annullata.");
    });

    kendoConfirm.open();
}

function spostaCategoria() {

    //Estraggo i controlli dal pannello
    var obj_lsbAree = $("#LsbAree");
    var obj_txtArea = $("#TxtNomeArea");

    //Estraggo i nomi e gli ID
    var idArea = $(obj_lsbAree).val();
    var nomeArea = $(obj_txtArea).val();

    var piva = $(cIdPiva).val();

    if (piva === undefined || piva === "") {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("SelezionareUnAzienda", "Selezionare un'Azienda."));
        return;
    }

    if (nomeArea === "") {
        kendo.alert(TraduciScadCategorieTipologieUCWSClient("InserireNomeCategoria", "Inserire il nome della categoria"));
        return;
    }

    var kendoConfirm = $("<div></div>").kendoConfirm({
        title: "Attenzione",
        messages: { okText: "Sì", cancel: "No" },
        content: ("Tutti i documenti della categoria " + nomeArea.toUpperCase() + " saranno spostati da cartella di rete a database: Proseguire?")
    }).data("kendoConfirm");
    kendoConfirm.result.done(function () {
        var param = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "filtro_area": idArea, "filtro_tipologia": "" });
        WaitFrame.show();
        ajaxAgronica(GetUrlLetturaTabelleGestionali() + 'AgronicaCoreScadenziario/Alert.asmx/spostaAllegatiSuDatabase',
            param,
            function (risposta) {
                WaitFrame.hide();

                //rileggo l'anagrafica delle Aree
                LsbAree_Load();

                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
            },
            function (risposta) {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");

                if (risposta.RispostaStringa !== "") {
                    alert("ATTENZIONE: I seguenti file sono stati spostati ma NON cancellati: " + risposta.RispostaStringa);
                }

                WaitFrame.hide();

                //rileggo l'anagrafica delle Aree
                LsbAree_Load();
            });
    });

    kendoConfirm.result.fail(function () {
        kendo.alert("Operazione annullata.");
    });

    kendoConfirm.open();

}

