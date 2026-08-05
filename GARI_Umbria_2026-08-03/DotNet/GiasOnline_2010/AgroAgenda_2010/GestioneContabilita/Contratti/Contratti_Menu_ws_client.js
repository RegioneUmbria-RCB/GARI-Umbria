

//////////////////////////////////////////////////////////
// Griglia Imputazione
//////////////////////////////////////////////////////////

var indirizzohttp = "./Contratti_Menu.aspx";
var indirizzohttpWSGenerali = "./Contratti_Menu.aspx";

function EmptyRead(options) { }
function EmptySubmit(options) { }



function RicercaContratti(options, parametriPerLettura) {

  
    var filtro_anno = KendoMultisel("multiselAnno").value().join(",");
    var filtro_conferente = KendoMultisel("multiselConferente").value().join(",");
    var filtro_prodotto = KendoMultisel("multiselProdotto").value().join(",");

    var numero = ""
    if ($("#TxtNumero").val() !== undefined) {
        numero = $("#TxtNumero").val();
    }



    ajaxAgronicaSync(indirizzohttp + "/Leggi_Contratti",
        "{ piva: '" + $(cIdPiva).val() + "', " +
        "  cau_contratto: '" + $(cCau_Contratto).val() + "', " +
        "  contratto_numero: '" + numero + "', " +
        "  filtro_anno:  '" + filtro_anno + "', " +
        "  filtro_conferente:  '" + filtro_conferente + "', " +
        "  filtro_prodotto:  '" + filtro_prodotto + "', " +
        "  veg_cod: 52}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);


    }
        


function controllaRigheValidePerSubmitGrid_Clausole(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];

        if (item.Cau_Contratto == undefined || item.Cau_Contratto == "") {

            if (errMess != "")
                errMess += " <br/>";
            errMess += "Causali non impostate correttamente";

            bDatiNecessariInseriti = false;
        }

        if (item.Clausola_Numero == undefined || item.Clausola_Numero == "") {

            if (errMess != "")
                errMess += " <br/>";
            errMess += "Numero clausole contrattuali non impostate correttamente";

            bDatiNecessariInseriti = false;
        }

    }

    return errMess;
}

function Elenco_Anno_Riempi(flagVuoto) {

    var risultato_lettura;
    var bok = false;

    if (Elenco_Anno == null) {
        bok = true;
    }
    else
        risultato_lettura = Elenco_Anno;


    
    if (bok) {


        var numero = ""
        if ($('input[name$="TxtNumero"]').val() !== undefined) {
            numero = $('input[name$="TxtNumero"]').val()
        }

        ajaxAgronicaSync(indirizzohttp + "/Leggi_Contratti_Anno",
            "{ piva: '" + $(cIdPiva).val() + "', " +
            "  cau_contratto: '" + $(cCau_Contratto).val() + "', " +
            "  contratto_numero: '" + numero + "', " +
            "  cod_contatto: '', " +
            "  veg_cod: 52}",

            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    objVuoto = {
                        "Anno": 0,
                        "Anno_Des": ""
                    };
                    risp.unshift(objVuoto);
                };
                risultato_lettura = risp;
            }, null);
    }
    return risultato_lettura;
}



function Elenco_Conferente_Riempi(flagVuoto) {

    var risultato_lettura;
    var bok = false;

    if (Elenco_Conferente == null) {
        bok = true;
    }
    else
        risultato_lettura = Elenco_Conferente;



    if (bok) {

        ajaxAgronicaSync(indirizzohttp + "/Leggi_Contratti_Conferenti",
            "{ piva: '" + $(cIdPiva).val() + "', " +
            "  tipo_rapporto: 6}",

            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    objVuoto = {
                        "Cod_RisUm": 0,
                        "Rag_Soc_Contatto": ""
                    };
                    risp.unshift(objVuoto);
                };
                risultato_lettura = risp;
            }, null);
    }
    return risultato_lettura;
}



function Elenco_Prodotto_Riempi(flagVuoto) {

    var risultato_lettura;
    var bok = false;

    if (Elenco_Prodotto == null) {
        bok = true;
    }
    else
        risultato_lettura = Elenco_Prodotto;



    if (bok) {

        ajaxAgronicaSync(indirizzohttp + "/Leggi_Contratti_Prodotti",
            "{ piva: '" + $(cIdPiva).val() + "', " +
            "  veg_cod: 52, " +
            "  elem_cod: 210}",

            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    objVuoto = {
                        "Mat_Cod": 0,
                        "Mat_Des": ""
                    };
                    risp.unshift(objVuoto);
                };
                risultato_lettura = risp;
            }, null);
    }
    return risultato_lettura;
}



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////// SUBMIT GENERALE  /////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//eventi di click pulsanti
function AggiornaEffettivoClausole(flagEsci) {

    //Aggiornamento Effettivo
    var allOk = false;

    var grid_dettagliClausole = $("#tab_griglia_clausole").data("kendoGrid");    

    var param = "{piva: '" + $(cIdPiva).val() + "'," +               
        " righeInseriteGrid_Clausole: '" + righeInseriteGrid_Clausole + "', righeModificateGrid_Clausole: '" + righeModificateGrid_Clausole + "' , righeCancellateGrid_Clausole: '" + righeCancellateGrid_Clausole + "'}";

    ajaxAgronicaSync(indirizzohttp + "/AggiornaClausole",
        param, false,
        function (risposta) {

            var Dummy;
            Dummy = parseInt(risposta.RispostaStringa);

            //if (flagEsci !== undefined && flagEsci && $(paginaRedirect).val() !== undefined && $(paginaRedirect).val() !== "") {
            //    window.location.href = $(paginaRedirect).val() + "?p=" + $(cPiva_Codificata).val();
            //}

            if (flagEsci !== true) {
                allOk = true;
            }
        }, null);

    if (!allOk) {
        //erroreSubmitCDG(grid_dettagliImpianti);        
    }
    else {
       
        MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");

        ConfiguraGrigliaClausole("tab_griglia_clausole", false);
    }
}





function RicercaClausole(options, parametriPerLettura) {


    //var filtro_distinta = KendoMultisel("multiselDis").value().join(",");

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Clausole",
        "{ piva: '" + $(cIdPiva).val() + "', " +
        "  cau_contratto: '" + $(cCau_Contratto).val() + "'}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);


}




function Elimina_Contratto(contratto_cod) {

    var risultatoCancellazione;

    var param = "{piva: '" + $(cIdPiva).val() + "'," +
        " contratto_cod: " + contratto_cod + "}";

    ajaxAgronicaSync(indirizzohttp + "/Delete_Contratto",
        param,
        false,
        function (risposta) {
            risultatoCancellazione = risposta.Errore;
        },
        function (risposta) {
            risultatoCancellazione = risposta.Errore;
        });

    return risultatoCancellazione;

}