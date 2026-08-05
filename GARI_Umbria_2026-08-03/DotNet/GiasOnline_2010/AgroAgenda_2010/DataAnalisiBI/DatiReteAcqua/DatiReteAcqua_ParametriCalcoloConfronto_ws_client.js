var indirizzohttp = "./DatiReteAcqua_WS.aspx";

function SalvaTutto() {
    SalvaParametriGenerali();
    if ($("#divCoeffSpecie").data("kendoGrid") !== undefined) {
        SalvaCoefficientiXSpecie();
    }
    SetModifyState(false);
}

function AnnullaTutto() {
    PopolaGrigliaParametriGenerali();
    if ($('#ddlSpecie').val() !== "" && $('#ddlSpecie').val() !== undefined) {
        LeggiDatiCaricaGrigliaCoeffXSpecie();
    }
    SetModifyState(false);
}

function InizializzaParametriGenerali(options) {
    //let year = $('#ddlYear').val()
    //var parametri = kendo.stringify({ "anno": year });
    var parametri = kendo.stringify({});


    ajaxAgronicaSync(indirizzohttp + "/InizializzaParametriGenerali",
        parametri,
        false,
        function (risposta) {
            PopolaGrigliaParametriGenerali();
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
            console.log("Errore: " + risposta.Errore);
        }, null);
}

function LeggiParametriGenerali() {
    //let year = $('#ddlYear').val()
    var parametri = kendo.stringify({ "settimana": 0 });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_ParametriGenerali",
        parametri,
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            $("#" + hdKendoParsColumnsClientID).val(JSON.stringify(JSON.parse(risp.columns)));
            $("#" + hdKendoParsRowsClientID).val(JSON.stringify(JSON.parse(risp.data)));
            $("#" + hdKendoParsModelClientID).val(JSON.stringify(JSON.parse(risp.model)));
        }, null);
}

function SalvaParametriGenerali() {
    let kendoStr = JSON.stringify($("#divKendoGridDataPars").data("kendoGrid").dataSource.data());
    var parametri = kendo.stringify({ "kendoGrid": kendoStr });

    ajaxAgronicaSync(indirizzohttp + "/ModificaParametriGenerali",
        parametri,
        false,
        function (risposta) {
            PopolaGrigliaParametriGenerali();
        }, null);
}

function LeggiCoefficientiXSpecie() {
    var veg_cod = $('#ddlSpecie').val()
    //let year = $('#ddlYear').val()
    var parametri = kendo.stringify({ "veg_cod": veg_cod,"settimana": 0 });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_CoeffXSpecie",
        parametri,
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            $("#" + hdKendoCoeffXSpecieColumnsClientID).val(JSON.stringify(JSON.parse(risp.columns)));
            $("#" + hdKendoCoeffXSpecieRowsClientID).val(JSON.stringify(JSON.parse(risp.data)));
            $("#" + hdKendoCoeffXSpecieModelClientID).val(JSON.stringify(JSON.parse(risp.model)));
        }, null);
}

function AggiungiCoefficientiXSpecie() {
    var veg_cod = $('#ddlSpecie').val()
    //let year = $('#ddlYear').val()
    var parametri = kendo.stringify({ "veg_cod": veg_cod });


    ajaxAgronicaSync(indirizzohttp + "/AggiungiCoefficientiXSpecie",
        parametri,
        false,
        function (risposta) {
            LeggiDatiCaricaGrigliaCoeffXSpecie();
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
            console.log("Errore: " + risposta.Errore);
        });
}

function SalvaCoefficientiXSpecie() {
    let kendoStr = JSON.stringify($("#divCoeffSpecie").data("kendoGrid").dataSource.data());
    var parametri = kendo.stringify({ "kendoGrid": kendoStr });

    ajaxAgronicaSync(indirizzohttp + "/ModificaCoefficientiXSpecie",
        parametri,
        false,
        function (risposta) {
            LeggiDatiCaricaGrigliaCoeffXSpecie();
        }, function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
            console.log("Errore: " + risposta.Errore);
        });
}

function Leggi_Specie(options) {

    var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "Gru_Cod": 0, "LetteraIniziale": "", "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

    ajaxAgronicaSync(pathCoreWS + "Metaschema/SpecieVegetali.asmx/CaricaComboSpecieVegetali_conFiltroUtente",
        parametri,
        false,
        function (risposta) {
            let specievegetali = JSON.parse(risposta.RispostaStringa);

            // ordina i risultati per descrizione
            specievegetali.sort(function (a, b) {
                if (a.veg_des > b.veg_des) return 1;
                else if (a.veg_des < b.veg_des) return -1;
                return 0;
            });

            objVuoto = { "veg_cod": "", "veg_des": "" };
            specievegetali.unshift(objVuoto);
            options.success(specievegetali);
        }, null);

}

//function Leggi_Anni(options) {
//    var obj =
//        [
//            { "year_cod": 2018 },
//            { "year_cod": 2019 },
//            { "year_cod" : 2020 },
//            { "year_cod" : 2021 },
//            { "year_cod" : 2022 },
//            { "year_cod" : 2023 },
//            { "year_cod" : 2024 },
//            { "year_cod" : 2025 },
//            { "year_cod" : 2026 },
//            { "year_cod" : 2027 },
//            { "year_cod" : 2028 },
//            { "year_cod" : 2029 },
//            { "year_cod" : 2030 }
//        ];
//    options.success(obj);
//}

