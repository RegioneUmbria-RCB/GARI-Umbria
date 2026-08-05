
function CaricaComboCmb_TipoDocumento(options) {

    var parametri = kendo.stringify({ "ID_Area": Area, "ID_Tipologia": Tipologia });

    ajaxAgronica("GestioneAllegati.aspx/CaricaCombo_TipoDocumento",
        parametri,
        function (risposta) {
            let data = JSON.parse(risposta.RispostaStringa);
            if (data.length > 1) {
                objVuoto = { "text": "", "value": "" };
                data.unshift(objVuoto);
            }
            options.success(data);
        }, null, null, false);

}

function LeggiAllegati(options) {

    var parametri = kendo.stringify({ "Piva": Piva, "Cod_Contatto": Cod_Contatto, "Analisi_Testata_Cod": Analisi_Testata_Cod, "ID_Elenco": 0 });

    ajaxAgronica("GestioneAllegati.aspx/LeggiAllegati",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}


function ModificaAllegato(objAllegato) {

    var id_elenco = objAllegato.ID_Elenco;

    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "id_elenco": id_elenco });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Entita.asmx/Leggi_con_documenti",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            if (risp.length > 0) {
                let data_rilascio = templateData(kendo.parseDate(risp[0].Validazione_Data));
                let data_scadenza = templateData(kendo.parseDate(risp[0].Data));
                KendoDDL("Cmb_TipoDocumento").value(risp[0].ID_Tipologia);
                $("#Txt_Num_Documento").val(risp[0].Allegati_Documenti_Numero);
                $("#Txt_Ente_Rilascio").val(risp[0].Allegati_Documenti_Ente_Des);
                $("#Txt_Data_Rilascio").data("kendoDatePicker").value(data_rilascio != "" ? risp[0].Validazione_Data : "");
                $("#Txt_Data_Scadenza").data("kendoDatePicker").value(data_scadenza != "" ? risp[0].Data : "");
                $('#Txt_Documento_Allegato').val(risp[0].File_Name);
                $('#Txt_Descrizione').val(risp[0].Testo);
                $('#Txt_ID_Elenco').val(objAllegato.ID_Elenco);
                $('#Txt_ID_Alert_Entita').val(risp[0].Id_Alert_Entita);
                $('#Txt_Allegati_Documenti_Cod').val(risp[0].Allegati_Documenti_Cod);
                $('#File_Caricato').val(risp[0].File_Allegato_DB);
                $('#btn_salva_allegato').hide();
                $('#btn_modifica_allegato').show();
                $('#btn_annulla_allegato').show();
            }
            // options.success(risp);
        }, null);
}
function CancellaAllegato(objAllegato) {

    var id_alert_entita = objAllegato.ID_Alert_Entita;
    var id_elenco = objAllegato.ID_Elenco;

    var parametri = kendo.stringify({ "ID_Elenco": id_elenco });

    ajaxAgronica("GestioneAllegati.aspx/CancellaAllegato",
        parametri,
        function (risposta) {
            if (risposta.RispostaOK) {
                kendo.alert("Allegato cancellato correttamente");
                //inizializzaKendoAllegati('gridAllegati');
                location.reload();
            } else {
                kendo.alert("Errore durante la cancellazione dell'allegato");
            }
        }, null);

}

function SalvaAllegato(nuovo) {

    if (validaAllegato(nuovo)) {

        var id_elenco = nuovo ? 0 : $("#Txt_ID_Elenco").val();
        var tipo_doc = KendoDDL("Cmb_TipoDocumento").value();
        var num_doc = $("#Txt_Num_Documento").val();
        var ente_rilascio = $("#Txt_Ente_Rilascio").val();
        var data_rilascio = $("#Txt_Data_Rilascio").val(); //kendo.parseDate()
        var data_scadenza = $("#Txt_Data_Scadenza").val();
        var nome_file = $('#Txt_Documento_Allegato').val();
        var file_allegato = $('#File_Caricato').val();
        var descrizione = $('#Txt_Descrizione').val();

        var parametri = kendo.stringify({
            "Piva": Piva,
            "Cod_Contatto": Cod_Contatto,
            "Analisi_Testata_Cod": Analisi_Testata_Cod,
            "Tipo_Documento": tipo_doc,
            "Num_Documento": num_doc,
            "Ente_Rilascio": ente_rilascio,
            "Data_Rilascio": data_rilascio,
            "Descrizione": descrizione,
            "Data_Scadenza": data_scadenza,
            "Nome_File": nome_file,
            "File_Allegato": file_allegato,
            "ID_Elenco": id_elenco,
            "Validazione_Flag": 0,
            "Username_Upload": "",
            "Data_Upload": "",
            "Note": "",
            "EntitaxIndici": ""
        });

        ajaxAgronica("GestioneAllegati.aspx/SalvaAllegato",
            parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    // MessaggioTuttoOK_Bootstrap("Allegato salvato correttamente", 'DIV_Messaggi');
                    // kendo.alert("Allegato salvato correttamente");
                    // parent.chiudiGestioneAllegati();
                    location.reload();
                } else {
                    // MessaggioErrore_Bootstrap("Errore durante salvataggio allegato", "DIV_Messaggi");
                    kendo.alert("Errore durante salvataggio allegato");
                }
            }, null);

    }

}

//function ScaricaAllegato(id) {
//    var parametri = kendo.stringify({ "Cod_Documento": id });
//    ajaxAgronica("GestioneAllegati.aspx/ScaricaAllegato",
//        parametri,
//        function (risposta) {
//            if (risposta.RispostaOK) {
//                if (risposta.ParametroDue) {
//                    $('#iframe').attr('src', "GestioneAllegati.aspx?d=" + risposta.ParametroDue_stringa);
//                    $('#iframe').load();
//                }
//            } else {
//                kendo.alert("Errore durante il download dell'allegato");
//            }
//        }, null);
//}

function SalvaPatentino(nuovo) {

    if (validaAllegato(nuovo)) {
        var id_elenco = nuovo ? 0 : $("#Txt_ID_Elenco").val();
        var id_alert_entita = nuovo ? 0 : $("#Txt_ID_Alert_Entita").val();
        var allegati_documenti_cod = nuovo ? 0 : $("#Txt_Allegati_Documenti_Cod").val();
        var tipo_doc = KendoDDL("Cmb_TipoDocumento").value();
        var num_doc = $("#Txt_Num_Documento").val();
        var ente_rilascio = $("#Txt_Ente_Rilascio").val();
        var data_rilascio = $("#Txt_Data_Rilascio").val(); //kendo.parseDate()
        var data_scadenza = $("#Txt_Data_Scadenza").val();
        var nome_file = $('#Txt_Documento_Allegato').val();
        var file_allegato = $('#File_Caricato').val();
        var descrizione = $('#Txt_Descrizione').val();

        var parametri = kendo.stringify({
            "Piva": Piva,
            "Cod_Contatto": Cod_Contatto,
            "Analisi_Testata_Cod": Analisi_Testata_Cod,
            "Tipo_Documento": tipo_doc,
            "Num_Documento": num_doc,
            "Ente_Rilascio": ente_rilascio,
            "Data_Rilascio": data_rilascio,
            "Descrizione": descrizione,
            "Data_Scadenza": data_scadenza,
            "Nome_File": nome_file,
            "File_Allegato": file_allegato,
            "ID_Elenco": id_elenco,
            "ID_Alert_Entita": id_alert_entita,
            "Allegati_Documenti_Cod": allegati_documenti_cod
        });

        ajaxAgronica("GestioneAllegati.aspx/SalvaPatentino",
            parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    location.reload();
                } else {
                    kendo.alert("Errore durante salvataggio allegato");
                }
            }, null);
    }
}
