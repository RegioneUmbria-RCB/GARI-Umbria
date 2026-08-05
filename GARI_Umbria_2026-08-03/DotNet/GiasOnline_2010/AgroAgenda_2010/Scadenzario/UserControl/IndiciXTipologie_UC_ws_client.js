var indirizzohttp = "./Scad_Anagrafiche.aspx";
//function RiempiDdlAzienda(options) {

//    var ddl = $('#InXTip_UC_ddlAzienda').data("kendoDropDownList");
//    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });

//    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtente",
//        parametri,
//        false,
//        function (risposta) {
//            risp = JSON.parse(risposta.RispostaStringa);
//            options.success(risp);
//        }, null);
//}

function RiempiListBoxIndice() {
    let listaIndici = [];
   
    //Estraggo la partita iva selezionata dalla listbox delle aziende
    var piva = $(cIdPiva).val()

    if (piva == undefined || piva == "") {
        return;
    }

    var param = kendo.stringify({ "objP_server": objP_server, "piva": piva, "soloPrivate": false });

    ajaxAgronicaSync(indirizzohttp + "/Carica_Alert_Indici_ListBox", param, false,
        function (risposta) {
            listaIndici = JSON.parse(risposta.RispostaStringa);
        }, null);

    return listaIndici;
}

function InXTip_UC_LsbAree_Load() {
    //pulisco le listbox
    $("#InXTip_UC_LsbAree").find('option').remove();
    $("#InXTip_UC_LsbTipologie").find('option').remove();

    //Pulisco gli altri controlli (non essendo selezionato nulla, la funzione ripulisce tutto)
    InXTip_UC_LsbAree_onchange();

    ////Estraggo la partita iva selezionata
    //var piva = $('#InXTip_UC_ddlAzienda').val();

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
            $("#InXTip_UC_LsbAree").html(strHtml);

        }, null);


}

//Se seleziono l'area di una categoria, carico le sue tipologie
function InXTip_UC_LsbAree_onchange() {

    //Estraggo nome e valore di ciò che è selezionato
    var nomeArea = $("#InXTip_UC_LsbAree").find('option:selected').text();
    valoreArea_IndiciXTipologie = $("#InXTip_UC_LsbAree").val();

    //Elimino gli elementi delle due listbox quando cambio area e disattivo lo switch obbligatorio
    let listBoxI = $("#Indici").data("kendoListBox");

    //Nascondo lo switch e la label di obbligo
    $("#Switch_Campo_Obbligatorio").hide();

    if (listBoxI !== undefined) {
         listBoxI.remove(listBoxI.items());
    }

    let listBoxIxT = $("#IndiciXTipo").data("kendoListBox");
    if (listBoxIxT !== undefined) {
        listBoxIxT.remove(listBoxIxT.items());
    }

    if ($("#cb_obbligatorio_liv").data("kendoSwitch") !== undefined) {
        $("#cb_obbligatorio_liv").data("kendoSwitch").enable(false);
        $("#cb_obbligatorio_liv").data("kendoSwitch").check(false);
    }

    //Imposto le aree in base alla selezione
    if (valoreArea_IndiciXTipologie != null) {

        //Carico le tipologie, senza controllare le autorizzazioni dell'utente
        var parametri = kendo.stringify({
            "objP_server": objP_server,
            "piva": "",
            "soloPrivate": false,
            "id_area": valoreArea_IndiciXTipologie,
            "controllaSeUtenteAutorizzato": false,
            "tipoPermessoDaControllare": 2,
            "listaIndici": "",
            "xMultiSelect": false,
            "id_tipologia": 0
        });

        ajaxAgronicaSync(pathCoreWS + 'AgronicaCoreScadenziario/Alert_Tipologia.asmx/Leggi_Tipologie', parametri, false,
            function (risposta) {
                var lista = JSON.parse(risposta.RispostaStringa);

                $("#InXTip_UC_LsbTipologie").find('option').remove();

                $("#Indici").find('option').remove();
                $("#IndiciXTipo").find('option').remove();

                //genero il codice HTML
                var strHtml = "";
                for (var i = 0; i < lista.length; i++) {
                    strHtml += "<option value='" + lista[i]["id_tipologia"] + "'>" + lista[i]["nome"] + "</option>";
                }

                //aggiungo l'HTML al controllo
                $("#InXTip_UC_LsbTipologie").html(strHtml);

            }, null);

        //Se è stato selezionato qualcosa...
        $("#InXTip_UC_TxtNomeArea").val(nomeArea);
    }
    else {

        //pulisco la combo
        $("#LsbTipologie").find('option').remove();

        //Se non è stato selezionato nulla...
        $("#TxtNomeArea").val("");

        $("#BtnAree_Edit").hide();
        $("#BtnAree_Del").hide();
    }

    //Pulisco le tipologie
    $("#TxtNomeTipologia").val("");

}


//Richiamo il web method per caricare le righe sulla listbox IndiciXTipo 
function LeggiIndicixTipologia(ID_Indice) {  

    let listaIndiciXTipo = [];

    if (ID_Indice === null) {
        ID_Indice = 0;
    }

    var param = kendo.stringify({ "objP_server": objP_server,"ID_Tipologia": valoreTipologia_IndiciXTipologie, "ID_Area": valoreArea_IndiciXTipologie, "ID_IndiceXTipo": ID_Indice });

    ajaxAgronicaSync(indirizzohttp + "/LeggiIndicexTipologia", param, false,
        function (risposta) {
           listaIndiciXTipo = JSON.parse(risposta.RispostaStringa);
        }, null);

    return listaIndiciXTipo;
}


//Richiamo il web method per la scrittura su Alert_indicexTipologia
function Insert_IndiceXTipologia(e) {
    //Estraggo la partita iva selezionata dalla listbox delle aziende
    var obj = e.dataItems[0];
    valoreArea_IndiciXTipologie = $("#InXTip_UC_LsbAree").find('option:selected').val(); 
    valoreTipologia_IndiciXTipologie = $("#InXTip_UC_LsbTipologie").find('option:selected').val()

    var param = kendo.stringify({
        "objP_server": objP_server, "Id_Indice": obj.ID_Indice, "ChkObbligatorio": obj.ChkObbligatorio_Tipologia, "ID_Tipologia": valoreTipologia_IndiciXTipologie, "ID_Area": valoreArea_IndiciXTipologie
    });

    ajaxAgronicaSync(indirizzohttp + "/Insert_Alert_IndicexTipologia", param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                //Rileggo il datasource della griglia con i campi tolti
                if ($("#grid_tipologiexindice").data("kendoGrid") !== undefined && $("#grid_tipologiexindice").data("kendoGrid") !== "") {
                    $("#grid_tipologiexindice").data("kendoGrid").dataSource.read();
                }
            }
        }, null);
}


//Richiamo il web method per la modifica dell'ordinamento su Alert_indicexTipologia
function Modifica_Ordine_Alert_IndicexTipologia( lista) {
    //Estraggo la partita iva selezionata dalla listbox delle aziende
    var ArrayRigheAggiornate = kendo.stringify(lista);

    var param = kendo.stringify({ "objP_server": objP_server,"lista": ArrayRigheAggiornate });
    ajaxAgronicaSync(indirizzohttp + "/Modifica_Ordine_Alert_IndicexTipologia", param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                //Rileggo il datasource della griglia con i campi tolti
                if ($("#grid_tipologiexindice").data("kendoGrid") !== undefined && $("#grid_tipologiexindice").data("kendoGrid") !== "") {
                    $("#grid_tipologiexindice").data("kendoGrid").dataSource.read();
                }
            }
        }, null);
}

//Richiamo il web method per la modifica dell' obbligo su Alert_indicexTipologia
function Modifica_Obbligo_Alert_IndicexTipologia(obbligo,ID_Indice) {

    var param = kendo.stringify({
        "objP_server": objP_server,"Id_Indice": ID_Indice, "ID_Tipologia": valoreTipologia_IndiciXTipologie, "ID_Area": valoreArea_IndiciXTipologie, "Obbligatorio": obbligo
    });
    ajaxAgronicaSync(indirizzohttp + "/Modifica_Obbligo_Alert_IndicexTipologia", param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                //Rileggo il datasource della griglia con i campi tolti
                if ($("#grid_tipologiexindice").data("kendoGrid") !== undefined && $("#grid_tipologiexindice").data("kendoGrid") !== "") {
                    $("#grid_tipologiexindice").data("kendoGrid").dataSource.read();
                }
            }
    }, null);
}


//Richiamo il web method per la cancellazione su Alert_indicexTipologia
function Delete_IndiceXTipologia(e) {
    var obj = e.dataItems[0];

    //Anna 17/05/22: aggiunto limite alla cancellazione di legame tra Tipologie x Indici riservati (solo SuperUser)
    if (!(obj.ID_Indice < 0 && valoreTipologia_IndiciXTipologie < 0) || $("input[name$='hf_UtenteAbilitatoModificaTipologiexIndiciProtetti']").val() == "True") {

        var param = kendo.stringify({ "objP_server": objP_server, "Id_Indice": obj.ID_Indice, "ID_Tipologia": valoreTipologia_IndiciXTipologie, "ID_Area": valoreArea_IndiciXTipologie });

        ajaxAgronicaSync(indirizzohttp + "/Cancella_Alert_IndicexTipologia", param, false,
            function (risposta) {
                if (risposta.RispostaOK) {
                    //Rileggo il datasource della griglia con i campi tolti
                    if ($("#grid_tipologiexindice").data("kendoGrid") !== undefined && $("#grid_tipologiexindice").data("kendoGrid") !== "") {
                        $("#grid_tipologiexindice").data("kendoGrid").dataSource.read();
                    }
                }
            }, null);

    } else {
        kendo.alert(TraduciIndiciXTipologieUC("AssociazioneTipologiaIndiceProtetta", "Questa associazione tra Tipologia e Indice è protetta."))
        e.preventDefault();
    }
}

//Carico la griglia 
function CaricoGrigliaIndicixTipologia(options) {

    let tipologia = 0;
    let area = 0;

    //Carico tutti gli indicixtipologie

    var param = kendo.stringify({ "objP_server": objP_server, "ID_Area": area, "ID_Tipologia": tipologia });

    ajaxAgronica(indirizzohttp + "/Carica_Griglia_IndiciXTipologie", param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

}