//////////////////////////////////////////////////////////
// Griglia Imputazione
//////////////////////////////////////////////////////////

var indirizzohttp = "./Contratti_Pomodoro.aspx";
var indirizzohttpWSGenerali = "./Contratti_Menu.aspx";

function EmptyRead(options) { }
function EmptySubmit(options) { }

function RicercaFasi(options, parametriPerLettura) {
    //var filtro_distinta = KendoMultisel("multiselDis").value().join(",");

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Fasi",
        "{ piva: '" + $(cIdPiva).val() + "', " +
        "  contratto_cod: " + $(cContratto_Cod).val() + "}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}
        
function RicercaClausole(options, parametriPerLettura) {
    //var filtro_distinta = KendoMultisel("multiselDis").value().join(",");

    ajaxAgronicaSync(indirizzohttp + "/Leggi_ContrattoxClausole",
        "{ piva: '" + $(cIdPiva).val() + "', " +
        "  contratto_cod: " + $(cContratto_Cod).val() + "}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////// SUBMIT GENERALE  /////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//eventi di click pulsanti
function AggiornaEffettivo(flagEsci) {

    //Aggiornamento Effettivo
    var allOk = false;

    var grid_fasi = $("#tab_griglia_fasi").data("kendoGrid");   
    var grid_clausole = $("#tab_griglia_clausole").data("kendoGrid");   

    var Listino_Cod = 0;

    if (KendoDDL("cmbListino").dataItem() !== undefined) {
        Listino_Cod = KendoDDL("cmbListino").dataItem().Listino_Cod;
    }



    var param = "{piva: '" + $(cIdPiva).val() + "'," +        
        " contratto_cod: " + $(cContratto_Cod).val() + "," +    
        " data_stipulazione: '" + $('input[name$="txt_Data"]').val() + "'," +
        " anno: " + $('input[name$="txt_anno"]').val() + "," +     
        " superficie_prevista: '" + $('input[name$="txt_superficie"]').val() + "'," +    
        " resa_prevista: '" + $('input[name$="txt_resa_prevista"]').val() + "'," +    
        " cod_risum: " + KendoDDL("cmbConferente").dataItem().Cod_RisUm + "," +   
        " cau_contratto: '" + $(cmbCausale).val() + "', " +
        " listino_cod: '" + Listino_Cod + "'," +    
        " contratto_numero: '" + $("#TxtNumero").val() + "'," +    
        " contratto_nome: '" + $('input[name$="txt_nome"]').val() + "'," +    
        " valore1: '" + getKendoSwitch("ChkPremio") + "'," +    
        " valore2: '" + $('input[name$="txt_premio"]').val() + "'," +    
        " valore3: '" + $('input[name$="txt_tetto"]').val() + "'," +    
        " valore4: '" + $('input[name$="txt_DMA"]').val() + "'," +    
        " valore5: '" + $('input[name$="txt_Franchigia_DMA"]').val() + "'," +               
        " valore6: '" + $('input[name$="txt_DMI"]').val() + "'," +    
        " valore7: '" + $('input[name$="txt_Coefficiente"]').val() + "'," +    
        " valore8: '" + $('input[name$="txt_Limite"]').val() + "'," +    
        " valore9: '" + 0 + "'," +    
        " data_inizio_prevista: '" + $('input[name$="txt_DataPremio"]').val() + "'," +        
        " data_fine_prevista: '" + $('input[name$="txt_DataPremioFine"]').val() + "'," +        
        " valore1_2: '" + getKendoSwitch("ChkPremio2") + "'," +
        " valore2_2: '" + $('input[name$="txt_premio2"]').val() + "'," +
        " valore3_2: '" + $('input[name$="txt_tetto2"]').val() + "'," +    
        " data_inizio_prevista2: '" + $('input[name$="txt_DataPremio2"]').val() + "'," +
        " data_fine_prevista2: '" + $('input[name$="txt_DataPremioFine2"]').val() + "'," +        
        " righeGridFasi_Inserite: '" + righeInseriteGrid_Fasi + "'," +
        " righeGridFasi_Modificate: '" + righeModificateGrid_Fasi + "'," +
        " righeGridFasi_Cancellate: '" + righeCancellateGrid_Fasi + "'," +
        " righeGridClausole: '" + righeInseriteGrid_Clausole + "'," +
        " sa_cod: '" + KendoDDL("cmbCentri").dataItem().sa_cod + "'," +
        " Fabbricato_Cod: '" + KendoDDL("cmbFabbricati").dataItem().Fabbricato_Cod + "'}";

    ajaxAgronicaSync(indirizzohttp + "/AggiornaContrattoPomodoro",
        param, false,
        function (risposta) {

            var Dummy;
            Dummy = parseInt(risposta.RispostaStringa);

            if (flagEsci !== undefined && flagEsci && $(cPaginaRedirect).val() !== undefined && $(cPaginaRedirect).val() !== "") {
                window.location.href = $(cPaginaRedirect).val() + "?p=" + $(cPiva_Codificata).val();
            }

            
             allOk = true;
            
        }, null);

    if (!allOk) {
        //erroreSubmitCDG(grid_fasi);   
        //erroreSubmitCDG(grid_clausole);   
    }
    else {        
        MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
    }
}

function Elenco_Clausola_Riempi(flagVuoto) {

    var risultato_lettura;
    
    var cau_contratto = KendoDDL("cmbCausale").dataItem().Cau_Contratto;

    if (cau_contratto == 0 || cau_contratto == undefined) {
        cau_contratto = "";
    }

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Clausole",
        "{ piva: '" + $(cIdPiva).val() + "'," +        
        "  cau_contratto: '" + cau_contratto + "'}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Clausola_Cod": "-1",
                    "Clausola_Nome": ""
                };
                risp.unshift(objVuoto);
            }

            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Elenco_Specie_Riempi(flagVuoto) {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Specie",
        "{ veg_cod: " + $(cVeg_Cod).val() + "}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "veg_cod": "0",
                    "veg_des": ""
                };
                risp.unshift(objVuoto);
            }

            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Elenco_Prodotto_Riempi(flagVuoto, veg_cod) {

    var risultato_lettura;    

    if (veg_cod == 0 && $(cVeg_Cod).val() !== 0 ) {
        veg_cod = $(cVeg_Cod).val();
    }

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Prodotti",
        "{ piva: '" + $(cIdPiva).val() + "'," +
        "  veg_cod: " + veg_cod + "}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Mat_Cod": "0",
                    "Mat_Des_Esteso": ""
                };
                risp.unshift(objVuoto);
            }

            risultato_lettura = risp;
        }, null);

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

        ajaxAgronicaSync(indirizzohttpWSGenerali + "/Leggi_Contratti_Conferenti",
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

function Elenco_Centri_Riempi(flagVuoto) {

    var risultato_lettura;
    var bok = false;

    if (Elenco_Centri == null) {
        bok = true;
    } else
        risultato_lettura = Elenco_Conferente;

    if (bok) {

        ajaxAgronicaSync(indirizzohttp + "/Leggi_Centri",
            "{ piva: '" + $(cIdPiva).val() + "' }",
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    objVuoto = {
                        "sa_cod": 0,
                        "sa_nome": ""
                    };
                    risp.unshift(objVuoto);
                };
                risultato_lettura = risp;
            }, null);
    }
    return risultato_lettura;
}

function Elenco_Fabbricati_Riempi(flagVuoto) {

    var risultato_lettura = Elenco_Fabbricati;
    var bok = true;

    //if (Elenco_Fabbricati == null) {
    //    bok = true;
    //} else
    //    risultato_lettura = Elenco_Fabbricati;

    if (bok) {

        ajaxAgronicaSync(indirizzohttp + "/Leggi_Fabbricati",
            "{ piva: '" + $(cIdPiva).val() + "', " +
            "  sa_cod: " + $(cmbCentri).val() + " }",

            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    objVuoto = {
                        "Fabbricato_Cod": 0,
                        "Fabbricato_Des": ""
                    };
                    risp.unshift(objVuoto);
                };
                risultato_lettura = risp;
            }, null);
    }
    return risultato_lettura;
}

function Elenco_Listino_Riempi(flagVuoto) {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Listini",
        "{ piva: '" + $(cIdPiva).val() + "', " +
        "  veg_cod: " + $(cVeg_Cod).val() + ", " +
        "  tipo_classe: 1}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Listino_Cod": "0",
                    "Listino_Des": ""
                };
                risp.unshift(objVuoto);
            }

            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function controllaRigheValidePerSubmitGrid_Fasi(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];

        if (item.Mat_Cod == 0) {

            if (errMess != "")
                errMess += " <br/>";
            errMess += "Prodotti non impostati correttamente";

            bDatiNecessariInseriti = false;
        }

    }

    return errMess;
}

function controllaRigheValidePerSubmitGrid_Clausole(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];

        if (item.Clausola_Cod == 0) {

            if (errMess != "")
                errMess += " <br/>";
            errMess += "Clausole contrattuali non impostate correttamente";


            bDatiNecessariInseriti = false;
        }

    }

    return errMess;
}

function LeggiContratto() {

    if (parseInt($(cContratto_Cod).val()) !== 0) {

        ajaxAgronicaSync(indirizzohttp + "/Leggi_Contratto",
            "{piva: '" + $(cIdPiva).val() + "'," +
            " contratto_cod: " + $(cContratto_Cod).val() + "}",
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                //Impostazione controlli
                if (risp.length > 0) {

                    var risposta = risp[0];
                    
                    Set_KendoDDLValue("cmbConferente", risposta.Cod_Risum);
                    cmbConferente_change();

                    Set_KendoDDLValue("cmbCausale", risposta.Cau_Contratto);
                    cmbCausale_change();

                    Set_KendoDDLValue("cmbCentri", risposta.sa_cod);
                    cmbCentri_change();

                    Set_KendoDDLValue("cmbFabbricati", risposta.Fabbricato_Cod);
                    cmbFabbricati_change();

                    $('input[name$="txt_anno"]').val(risposta.Anno);
                    $("#TxtNumero").val(risposta.Contratto_Numero);
                    $('input[name$="txt_nome"]').val(risposta.Contratto_Nome);

                    $('input[name$="txt_superficie"]').val(String(risposta.Superficie_Prevista).replace(".", ","));
                    $('input[name$="txt_resa_prevista"]').val(String(risposta.Resa_Prevista).replace(".", ",")); 
                   
                    $('input[name$="txt_Data"]').val(formattedDate(risposta.Data_Stipulazione, '/'));            

                }

            }, null);

        
    }
}

function LeggiValori() {

    if (parseInt($(cContratto_Cod).val()) !== 0) {
        
        ajaxAgronicaSync(indirizzohttp + "/Leggi_Fasi",
            "{piva: '" + $(cIdPiva).val() + "'," +
            " contratto_cod: " + $(cContratto_Cod).val() + "}",
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                //Impostazione controlli
                if (risp.length > 0) {

                    var risposta = risp[0];                    

                    if (parseInt(risposta.Valore1) == 1) {
                        setKendoSwitch("ChkPremio", true);
                    } else {
                        setKendoSwitch("ChkPremio", false);
                    }

                    if (risposta.Valore1_2 !== null) {

                        if (parseInt(risposta.Valore1_2) == 1) {
                            setKendoSwitch("ChkPremio2", true);
                        }
                        else {
                            setKendoSwitch("ChkPremio2", false);
                        }

                    } else {
                        setKendoSwitch("ChkPremio2", false);
                    }


                    $('input[name$="txt_premio"]').val(String(risposta.Valore2).replace(".", ",")); 
                    $('input[name$="txt_tetto"]').val(String(risposta.Valore3).replace(".", ",")); 

                    if (risposta.Valore2_2 !== null) {
                        $('input[name$="txt_premio2"]').val(String(risposta.Valore2_2).replace(".", ","));
                    }
                    if (risposta.Valore3_2 !== null) {
                        $('input[name$="txt_tetto2"]').val(String(risposta.Valore3_2).replace(".", ","));
                    }

                    $('input[name$="txt_DMA"]').val(String(risposta.Valore4).replace(".", ","));  
                    $('input[name$="txt_Franchigia_DMA"]').val(String(risposta.Valore5).replace(".", ",")); 
                    $('input[name$="txt_DMI"]').val(String(risposta.Valore6).replace(".", ",")); 
                    $('input[name$="txt_Coefficiente"]').val(String(risposta.Valore7).replace(".", ","));
                    $('input[name$="txt_Limite"]').val(String(risposta.Valore8).replace(".", ",")); 

                    $('input[name$="txt_DataPremio"]').val(formattedDate(risposta.Data_Inizio_Prevista, '/'));
                    $('input[name$="txt_DataPremioFine"]').val(formattedDate(risposta.Data_Fine_Prevista, '/'));

                    if (risposta.Data_Inizio_Prevista2 !== null) {
                        $('input[name$="txt_DataPremio2"]').val(formattedDate(risposta.Data_Inizio_Prevista2, '/'));
                    }

                    if (risposta.Data_Fine_Prevista2 !== null) {
                        $('input[name$="txt_DataPremioFine2"]').val(formattedDate(risposta.Data_Fine_Prevista2, '/'));
                    }

                    Set_KendoDDLValue("cmbListino", risposta.Listino_Cod);
                    cmbListino_change();

                }

            }, null);

    }
}