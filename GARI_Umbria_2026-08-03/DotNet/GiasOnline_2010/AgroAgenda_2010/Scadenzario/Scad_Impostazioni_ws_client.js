
// FUNZIONE PER AGGIUNGERE UN AVVISO
function aggiungiAvviso() {
    //Leggo le variabili
    var ID_Area = $("#DdlArea").data("kendoDropDownList").value();

    var ID_Tipologia = $("#cmbTipologia").data("kendoDropDownList").value();
    if (ID_Tipologia == undefined || ID_Tipologia == "") {
        ID_Tipologia = 0;
    }

    var filtro_rapcon = KendoMultisel("cmbRapCon").value().join(",");

    var ID_Evento = -1;
    var GGAttesa = $("#TxbGGAttesa").data("kendoNumericTextBox").value();
    var MailMittente = $("#TxbMailMittente").val();

    // In caso di calorizzazione dei rapporti contabili, il destinatario verrà calcolato al momento dell'invio effettivo dell'email.
    var MailA = "";
    if (filtro_rapcon === "") {
        MailA = $("#TxbMailA").val();
    }
    
    var MailCC = $("#TxbMailCC").val();

    var param = kendo.stringify({ 'objP_server': objP_server, 'ID_Area': ID_Area, 'ID_Tipologia': ID_Tipologia, 'Filtro_RapCon': filtro_rapcon, 'ID_Evento': ID_Evento, 'GGAttesa': GGAttesa, 'MailMittente': MailMittente, 'MailA': MailA, 'MailCC': MailCC });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Avvisi.asmx/Aggiungi", param, false,
        function (risposta) {
            eseguiRicercaAvvisi();
            svuotaControlliAvviso();
        }, null);

}

// FUNZIONE PER MODIFICARE UN AVVISO
function modificaAvviso() {

    //Leggo le variabili
    var Old_ID_Avviso = $("#ID_Avviso").val();
    var New_ID_Area = $("#DdlArea").data("kendoDropDownList").value();

    var New_ID_Tipologia = $("#cmbTipologia").data("kendoDropDownList").value();
    if (New_ID_Tipologia == undefined || New_ID_Tipologia == "") {
        New_ID_Tipologia = 0;
    }

    var New_Filtro_RapCon = KendoMultisel("cmbRapCon").value().join(",");

    var New_ID_Evento = -1;
    var New_GGAttesa = $("#TxbGGAttesa").data("kendoNumericTextBox").value();
    var New_MailMittente = $("#TxbMailMittente").val();

    // In caso di calorizzazione dei rapporti contabili, il destinatario verrà calcolato al momento dell'invio effettivo dell'email.
    var New_MailA = "";
    if (New_Filtro_RapCon === "") {
        New_MailA = $("#TxbMailA").val();
    }
    
    var New_MailCC = $("#TxbMailCC").val();

    var param = kendo.stringify({ 'objP_server': objP_server, 'Old_ID_Avviso': Old_ID_Avviso, 'New_ID_Area': New_ID_Area, 'New_ID_Tipologia': New_ID_Tipologia, 'New_Filtro_RapCon': New_Filtro_RapCon, 'New_ID_Evento': New_ID_Evento, 'New_GGAttesa': New_GGAttesa, 'New_MailMittente': New_MailMittente, 'New_MailA': New_MailA, 'New_MailCC': New_MailCC });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Avvisi.asmx/Modifica", param, false,
        function (risposta) {
            eseguiRicercaAvvisi();
            svuotaControlliAvviso();
        }, null);

}


//FUNZIONE CHE MOSTRA LA TABELLA
function eseguiRicercaAvvisi() {
    var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Avvisi.asmx/Leggi_Avvisi_ToKendoGrid", param,
        function (risposta) {

            $('#hdKendo_Valorizzazione').val(risposta.RispostaStringa);
            popolaGrigliaScadenze("tabella_avvisi");

        }, null);
}


function modificaElemento(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var ID_Avviso = datiRiga.ID_Avviso;

    var param = "{objP_server: '" + objP_server + "', ID_Avviso: '" + ID_Avviso + "' }";

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Avvisi.asmx/LeggiElem", param, false,
        function (risposta) {
            var avv = risposta.RispostaStringa;
            impostaControlliAvviso(avv.ID_Avviso, avv.ID_Area, avv.ID_Tipologia, avv.Filtro_RapCon, avv.ID_Evento, avv.GGAttesa, avv.MailMittente, avv.MailA, avv.MailCC);
        }, null);

    //tolgo la classe per l'evidenziazione da tutta la griglia
    $('#tabella_avvisi tr').removeClass("evidenziato");

    //aggiungo la classe per l'evidenziazione sulla riga interessata
    $(tr_elem).addClass("evidenziato");
}


//CANCELLAZIONE DELL'ELEMENTO
function eliminaElemento(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var ID_Avviso = datiRiga.ID_Avviso;

    var param = "{objP_server: '" + objP_server + "', ID_Avviso: '" + ID_Avviso + "' }";

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Avvisi.asmx/Cancella", param,
        function (risposta) {
            eseguiRicercaAvvisi();
        }, null);

    svuotaControlliAvviso();
}

//TEST MAIL
function testElemento(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var ID_Avviso = datiRiga.ID_Avviso;

    if (datiRiga.Filtro_RapCon === "") {

        var param = "{objP_server: '" + objP_server + "', objP_utenti: '" + objP_utenti + "', ID_Avviso: '" + ID_Avviso + "' }";

        ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Avvisi.asmx/TestMail", param,
            function (risposta) {
                if (risposta.Errore !== "") {
                    alert(risposta.Errore);
                }
            }, null);

        svuotaControlliAvviso();


    }
    else {

        alert("Non è possibile inviare un'email di prova se sono impostati i rapporti contabili");
    }

}





function Elenco_RapCon_Riempi(flagVuoto) {

    var risultato_lettura;

    ajaxAgronicaSync("./Scad_Impostazioni.aspx/Leggi_Rapporti_Contabili",
        "{ piva: ''}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            if (flagVuoto === true) {
                objVuoto = {
                    "Cod_Rapporto": 0,
                    "Rapporto_Des": ""
                };
                risp.unshift(objVuoto);
            };

            risultato_lettura = risp;
        }, null);


    return risultato_lettura;
}
