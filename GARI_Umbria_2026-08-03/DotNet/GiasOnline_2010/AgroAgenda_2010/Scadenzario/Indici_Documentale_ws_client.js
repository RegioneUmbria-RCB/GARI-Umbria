

//////////////////////////////////////////////////////////
// Griglia Imputazione
//////////////////////////////////////////////////////////

var indirizzohttp = "./Indici_Documentale.aspx";

function EmptyRead(options) { }
function EmptySubmit(options) { }

function Ricerca_Dettagli(options, parametriPerLettura) {

    if (parseInt($(cId_Indice).val()) !== 0) {

        ajaxAgronicaSync(indirizzohttp + "/Ricerca_Dettagli",
            "{ piva: '" + $(cIdPiva).val() + "', " +
            "  id_indice: " + $(cId_Indice).val() + "}",
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);
            }, null);
    }

}


function Elenco_Aree_Riempi(flagVuoto) {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Aree",
        "{ piva: '" + $(cIdPiva).val() + "'}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            if (flagVuoto === true) {
                objVuoto = {
                    "ID_Area": 0,
                    "Nome": ""
                };
                risp.unshift(objVuoto);
            };

            risultato_lettura = risp;
        }, null);


    return risultato_lettura;
}




function Elenco_Tipologie_Riempi(id_area) {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Tipologie",
        "{ piva: '" + $(cIdPiva).val() + "', " +
        "  id_area: " + id_area + "}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            risultato_lettura = risp;
        }, null);


    return risultato_lettura;
}




function Elenco_Tipologie_Bloccate_Riempi(Id_Indice) {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Tipologie_Bloccate",
        "{ piva: '" + $(cIdPiva).val() + "', " +
        "  id_indice: " + Id_Indice + "}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            risultato_lettura = risp;
        }, null);


    return risultato_lettura;
}





function Elenco_Tipo_Riempi(flagVuoto) {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Elenco_Tipo",
        "{ piva: '" + $(cIdPiva).val() + "'}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            if (flagVuoto === true) {
                objVuoto = {
                    "ID_Area": 0,
                    "Nome": ""
                };
                risp.unshift(objVuoto);
            };

            risultato_lettura = risp;
        }, null);


    return risultato_lettura;
}




function Elenco_Udm_Riempi() {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Udm",
        "{}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            risultato_lettura = risp;
        }, null);


    return risultato_lettura;
}





function Leggi() {

    if (parseInt($(cId_Indice).val()) !== 0) {

        ajaxAgronicaSync(indirizzohttp + "/Leggi_Indice",
            "{piva: '" + $(cIdPiva).val() + "'," +
            " id_indice: " + $(cId_Indice).val() + "}",
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                //Impostazione controlli
                if (risp.length > 0) {

                    if (parseInt(risp[0].ID_Indice) < 0) {
                        setKendoSwitch("ChkRiservato", true);
                    }
                    else {
                        setKendoSwitch("ChkRiservato", false);
                    }

                    if (parseInt(risp[0].ChkObbligatorio) == 1) {
                        setKendoSwitch("ChkObbligatorio", true);
                    }
                    else {
                        setKendoSwitch("ChkObbligatorio", false);
                    }

                    if (parseInt(risp[0].ChkIndice_Speciale) == 1) {
                        setKendoSwitch("ChkSpeciale", true);
                    }
                    else {
                        setKendoSwitch("ChkSpeciale", false);
                    }


                    $('input[name$="txt_titolo_indice"]').val(risp[0].TitoloIndice);

                    $('input[name$="txt_Validita_Inizio"]').val(formattedDate(risp[0].Validita_Inizio, '/'));
                    $('input[name$="txt_Validita_Fine"]').val(formattedDate(risp[0].Validita_Fine, '/'));


                    //var TipologiaSplit = "";

                    //for (x = 0; x < risp.length; x++) {

                    //    item = risp[x];   
                    //    TipologiaSplit = TipologiaSplit + String(item.ID_Tipologia) + "|";

                    //}

                    var risposta2 = risp[0];

                    //KendoDDL("cmbArea").select(risposta2.ID_Area);
                    //CmbArea_change();

                    //KendoMultisel("cmbTipologia").value(TipologiaSplit.split("|"));

                    KendoDDL("cmbTipo").select(risposta2.TipoCampo);
                    CmbTipo_change();

                    var chiave = "";
                    if (risposta2.Elenco_Tipo == 9) //Macchine specifiche
                        chiave = parseInt(risposta2.Elenco_Tipo) + "_" + risposta2.Elenco_Cod_String;
                    else
                        chiave = parseInt(risposta2.Elenco_Tipo) + "_" + parseInt(risposta2.Elenco_Cod);


                    Set_KendoDDLValue("cmbElenco", chiave);
                    CmbElenco_change();



                    //Set_KendoDDLValue("cmbElenco_Valore", risposta2.Elenco_Val);
                    //CmbElenco_Valore_change();
                    Set_KendoDDLValue("cmbLibero", risposta2.TipoDato);
                    CmbLibero_change();


                }

            }, null);



    }
}



function controllaRigheValidePerSubmitGrid_Dettagli(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];

        if (item.Valore === undefined || item.Valore === "") {

            if (errMess != "")
                errMess += " <br/>";
            errMess += TraduzioneMultiResx(resxObj, "ValoriNonImpostatiCorrettamente", "Valori non impostati correttamente");

        }

    }

    return errMess;
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////// SUBMIT GENERALE  /////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//eventi di click pulsanti
function AggiornaEffettivo(FlagEsci) {

    var allOk = false;

    var grid_dettagli = $("#tab_griglia_dettagli").data("kendoGrid");

    //var elenco_tipologie = KendoMultisel("cmbTipologia").value().join(",");

    //var id_area = KendoDDL("cmbArea").dataItem().ID_Area;

    var tipodato = "";
    if (KendoDDL("cmbLibero") !== undefined) {
        if (KendoDDL("cmbLibero").dataItem() !== undefined) {
            tipodato = KendoDDL("cmbLibero").dataItem().Libero_Cod;
        }
    }

    var elenco_tipo = 0;
    var elenco_cod = 0;
    var elenco_cod_string = "";
    if (KendoDDL("cmbElenco") !== undefined) {
        if (KendoDDL("cmbElenco").dataItem() !== undefined) {
            //if (parseInt(KendoDDL("cmbElenco").dataItem().Elenco_Cod) > 0) {
            elenco_tipo = KendoDDL("cmbElenco").dataItem().Elenco_Tipo;
            elenco_cod = KendoDDL("cmbElenco").dataItem().Elenco_Cod;
            elenco_cod_string = KendoDDL("cmbElenco").dataItem().Elenco_Cod_String;
        }
    }


    var elenco_val = "";
    if (KendoDDL("cmbElenco_Valore") !== undefined && Elenco_Valore !== "") {
        if (KendoDDL("cmbElenco_Valore").dataItem() !== undefined) {
            elenco_val = KendoDDL("cmbElenco_Valore").dataItem().Elenco_Valore_Cod;
        }
    }


    var titoloindice = $('input[name$="txt_titolo_indice"]').val();
    var Descrizione = titoloindice.replace("'", "");
    var validita_inizio = $('input[name$="txt_Validita_Inizio"]').val();
    var validita_fine = $('input[name$="txt_Validita_Fine"]').val();


    var param = "{piva: '" + $(cIdPiva).val() + "'," +
        " id_indice: " + parseInt($(cId_Indice).val()) + ", " +
        " titoloindice: '" + Descrizione + "'," +
        " chkriservato: " + getKendoSwitch("ChkRiservato") + "," +
        " chkobbligatorio: " + getKendoSwitch("ChkObbligatorio") + "," +
        " chkindice_speciale: " + getKendoSwitch("ChkSpeciale") + "," +
        " tipocampo: " + parseInt(KendoDDL("cmbTipo").dataItem().TipoCampo) + "," +
        " tipodato: '" + tipodato + "'," +
        " elenco_tipo: " + elenco_tipo + "," +
        " elenco_cod: " + elenco_cod + "," +
        " elenco_cod_string: '" + elenco_cod_string + "'," +
        " elenco_val: '" + elenco_val + "'," +
        " validita_inizio: '" + validita_inizio + "'," +
        " validita_fine: '" + validita_fine + "'," +
        " righeInseriteGrid_Dettagli: '" + righeInseriteGrid_Dettagli + "', righeModificateGrid_Dettagli: '" + righeModificateGrid_Dettagli + "', righeCancellateGrid_Dettagli: '" + righeCancellateGrid_Dettagli + "'}";


    ajaxAgronicaSync(indirizzohttp + "/AggiornaIndice",
        param, false,
        function (risposta) {

            MessaggioTuttoOK_Bootstrap(TraduzioneMultiResx(resxObj, "IndiceDocumentaleInseritoCorrettamente", "Indice documentale inserito correttamente"), "DIV_Messaggi");

            if (FlagEsci) {
                window.location.href = $(cPaginaRedirect).val() + "?p=" + $(cPiva_Codificata).val() + "&tab_default=tab_indici";
            }
            else {

                //Reset Chiave
                $(cId_Indice).val(0);

                //Reset Variabili Globali
                bBloccaControlli = false;
                righeInseriteGrid_Dettagli = "";
                righeModificateGrid_Dettagli = "";
                righeCancellateGrid_Dettagli = "";

                ConfiguraGrigliaDettagli("tab_griglia_dettagli", false);
            }

        }, null);

    //if (!allOk) {
    //    erroreSubmitCDG(grid_dettagliImpianti);


    //}
    //else {
    //    MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
    //}
}
