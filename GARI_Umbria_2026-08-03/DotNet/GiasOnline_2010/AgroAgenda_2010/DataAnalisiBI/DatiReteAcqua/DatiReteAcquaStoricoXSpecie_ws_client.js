var indirizzohttp = "./DatiReteAcqua_WS.aspx";


function Ricerca() {
    LeggiDatiCaricaGriglia();
    //SetModifyState(false);
}

function AnnullaTutto() {
    LeggiDatiCaricaGriglia();
    SetModifyState(false);
}

function SalvaTutto() {
    let kendoStr = JSON.stringify($("#divKendoGridData").data("kendoGrid").dataSource.data());
    var parametri = kendo.stringify({ "kendoGrid": kendoStr });

    ajaxAgronicaSync(indirizzohttp + "/AggiornaDatiStorici",
        parametri,
        false,
        function (risposta) {
            LeggiDatiCaricaGriglia();
            SetModifyState(false);
        }, null);
}

function AggiungiSpecie() {
    var veg_cod = $('#ddlSpecie').val()

    var anno = $('#ddlYear').val();
    var gruppoconsegna = $('#dllGruppoConsegna').val();

    var parametri = kendo.stringify({ "veg_cod": veg_cod ,"anno": anno, "gruppoconsegna": gruppoconsegna});


    ajaxAgronicaSync(indirizzohttp + "/AggiungiSpecieDatiStorici",
        parametri,
        false,
        function (risposta) {
            LeggiDatiCaricaGriglia();
        }, 
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
            console.log("Errore: " + risposta.Errore);
        }, null);
}

function LeggiDatiStorici(options) {
    var veg_cod = $('#ddlSpecie').val();
    var anno = $('#ddlYear').val();
    var gruppoconsegna = $('#dllGruppoConsegna').val();

    var parametri = kendo.stringify({ "veg_cod": veg_cod , "settimana" : 0, "anno" : anno , "gruppoconsegna" : gruppoconsegna});

    ajaxAgronicaSync(indirizzohttp + "/Leggi_DatiStorici",
        parametri,
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            $("#" + hdKendoColumsClientID).val(JSON.stringify(JSON.parse(risp.columns)));
            $("#" + hdKendoRowsClientID).val(JSON.stringify(JSON.parse(risp.data)));
            $("#" + hdKendoModelClientID).val(JSON.stringify(JSON.parse(risp.model)));
        },null);
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
function Leggi_Anni(options) {
    var obj =
        [
            { "year_cod": "" },
            { "year_cod": 2018 },
            { "year_cod": 2019 },
            { "year_cod": 2020 },
            { "year_cod": 2021 },
            { "year_cod": 2022 },
            { "year_cod": 2023 },
            { "year_cod": 2024 },
            { "year_cod": 2025 },
            { "year_cod": 2026 },
            { "year_cod": 2027 },
            { "year_cod": 2028 },
            { "year_cod": 2029 },
            { "year_cod": 2030 }
        ];
    options.success(obj);
}

function Leggi_GruppiConsegna(options) {
    var parametri = kendo.stringify({});

    ajaxAgronicaSync(indirizzohttp + "/ElencoGruppiConsegna",
        parametri,
        false,
        function (risposta) {
            let elenco = risposta.RispostaStringa.elenco;
            objVuoto = { "id": "", "desc": "" };
            elenco.unshift(objVuoto);
            options.success(elenco);
        }, null);
}

//function CaricaElencoSpecieVegetali() {
//    var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "Gru_Cod": 0, "LetteraIniziale": "", "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

//    ajaxAgronica(pathCoreWS + "Metaschema/SpecieVegetali.asmx/CaricaComboSpecieVegetali_conFiltroUtente",
//        parametri,
//        function (risposta) {
//            specievegetali = JSON.parse(risposta.RispostaStringa);

//            // ordina i risultati per descrizione
//            specievegetali.sort(function (a, b) {
//                if (a.veg_des > b.veg_des) return 1;
//                else if (a.veg_des < b.veg_des) return -1;
//                return 0;
//            });

//            objVuoto = { "veg_cod": "", "veg_des": "" };
//            specievegetali.unshift(objVuoto);

//        }, null, null, false);
//}

//function LeggiDatiStorici() {
//    var parametri = {};

//    ajaxAgronicaSync(indirizzohttp + "/LeggiDatiStorici",
//        parametri,
//        false,
//        function (risposta) {
//            dati_storici= JSON.parse(risposta.RispostaStringa);
//        }, null);
//}