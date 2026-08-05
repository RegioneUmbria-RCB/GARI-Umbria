var indirizzohttp = "./RichiestaCarburanti.aspx";


function LeggiDataRichiestaDocumenti(options) {
    var param = kendo.stringify({
        piva: $(cIdPiva).val()
    });

    ajaxAgronica(indirizzohttp + "/LeggiDataRichiestaDocumenti",
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp.length > 0)
                options.success(risp);
        }, null);
}

//function InviaRigheModificate(newRecords, updatedRecords, deletedRecords) {
//    return new Promise((resolve, reject) => {
//        var param = {
//            righeInserite: JSON.stringify(newRecords),
//            righeModificate: JSON.stringify(updatedRecords),
//            righeCancellate: JSON.stringify(deletedRecords)
//        };
//        var jsonData = JSON.stringify(param);

//        ajaxAgronica(indirizzohttp + "/RichiestaDocumenti_SalvaGriglia",
//            jsonData,
//            function (risposta) {
//                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
//                resolve(true);
//            }, null, null, false);
//    });
//}

function RichiestaDocumenti_Caricale(options) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            piva: Get_KendoDDLValue("ddlAzienda"),
            Richiesta_Cod: richiesta_cod
        });

        ajaxAgronica(indirizzohttp + "/RichiestaDocumenti_CaricaElenco", parametri,
            function (risposta) {
                rows = JSON.parse(risposta.RispostaStringa);
                if (rows.length > 0)
                    options.success(rows);
                resolve(rows);
            }, null, null, false);
    });
}

function RichiestaDocumenti_CaricaConfigurazioniDaDB(options) {
    var parametri = kendo.stringify({
        piva: Get_KendoDDLValue("ddlAzienda"),
        Richiesta_Cod: richiesta_cod
    });

    ajaxAgronica(indirizzohttp + "/CaricaConfigurazioniRichiestaDocumenti", parametri,
        function (risposta) {
            rows = JSON.parse(risposta.RispostaStringa);
            if (rows.length > 0)
                options.success(rows);
        }, null, null, false);

}

////è stata selezionata una cella
//function modificaRichiestaDocumenti(tr_elem, grid_elem) {
//    return new Promise((resolve, reject) => {
//        let datiGriglia = $(grid_elem).data('kendoGrid');
//        let datiRiga = datiGriglia.dataItem(tr_elem);
//        var parametri = kendo.stringify({
//            data: JSON.stringify(datiRiga)
//        });
//    });
//}



function GetUrlDocAgenda2010(piva, id_Tipologia, richiesta_Cod, pratica_Cod, id_Schema_Template, operazione, Documentale1_Anagrafica2) {
    var parametri = kendo.stringify({
        "piva": piva,
        "id_Tipologia": id_Tipologia,
        "richiesta_Cod": richiesta_Cod,
        "pratica_Cod": pratica_Cod,
        "id_Schema_Template": id_Schema_Template,
        "operazione": operazione,
        "Documentale1_Anagrafica2": Documentale1_Anagrafica2
    });

    var risp = "";

    ajaxAgronicaSync(indirizzohttp + "/GetUrlDocAgenda2010", parametri, false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        }, null);

    return risp;
}

function GetUrlAnalisi2010(piva) {
    var parametri = kendo.stringify({
        "piva": piva
    });

    var risp = "";

    ajaxAgronicaSync(indirizzohttp + "/GetUrlAnalisi2010", parametri, false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        }, null);

    return risp;
}
function Leggi_Termine_Ultimo_Rendicontazione() {
    var parametri = kendo.stringify({
        "anno": $("#anno").val(),
        "tipo_azienda": QS_TipoAzienda
    });


    ajaxAgronicaSync(indirizzohttp + "/Leggi_Termine_Ultimo_Rendicontazione",
        parametri, false,
        function (risposta) {
            //risp = JSON.parse(risposta.RispostaStringa);

            let res = JSON.parse(risposta.RispostaStringa)
            if (res.split("|")[0].toLowerCase() == 'true') {
                consentitoEditareDocumenti_daSetup = true
            } else {
                consentitoEditareDocumenti_daSetup = false
            }

            Termine_Ultimo_Rendicontazione = res.split("|")[1]

        }, null);

    return consentitoEditareDocumenti_daSetup;
}