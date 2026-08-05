
//
// ReportPercorsi_ws_client.js
//

var indirizzohttp = "./ReportPercorsiWS.aspx";

function KendoDettaglioGPFonDataBindingRighe() {

}

function letturaDatiKendoDettaglioGPFKendo() {

    var ParametriFiltroXSessione = [];

    let grigliaPercorsiDaMemorizzare = "divkendoGPF";

    if (tipoReportGPF === enum_tipoReportGPF.UltimaPosizione) {
        grigliaPercorsiDaMemorizzare = "divkendoGPF_POS";
    }

    var filteredData = kGetElementiSelezionati("#" + grigliaPercorsiDaMemorizzare);

    $.each(filteredData, function (idx, dataItem) {
        ParametriFiltroXSessione.push("'" + dataItem.id + "'");
    });

    var pp = '{ FiltroTestata: " ' + ParametriFiltroXSessione.join(",") + ' " } ';


    ajaxAgronica(indirizzohttp + "/ReportGPFDettaglio",
        pp,
        function (risposta) {

            $("#hdKendoDettaglioGPF").val(risposta.RispostaStringa);
            KendoDettaglioGPFKendo("divKendoDettaglioGPF");

        }, null);

}

function kReadKendoDettaglioGPF_rows(options) {

    var data = $('#hdKendoDettaglioGPF').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadKendoDettaglioGPF_col() {

    var data = $('#hdKendoDettaglioGPF').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadKendoDettaglioGPF_mod() {

    var data = $('#hdKendoDettaglioGPF').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}

function KendoDettaglioGPFKendo(divKendoDettaglioGPF) {

    var funzioniCRUD = {
        funzioneRead: kReadKendoDettaglioGPF_rows
    };
    var idModel = "kendoKey";
    var campiKendoModel = kReadKendoDettaglioGPF_mod();
    var colonneKendoGrid = kReadKendoDettaglioGPF_col();
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        pagesize: 20,
        groupable: false,
        scrollable: false,
        sortable: true,
        resizable: false,
        filterable: { mode: "row" },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        pdf: false,
        excel: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBinding: KendoDettaglioGPFonDataBindingRighe };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["kendoKey"];

    creaKendoGrid(divKendoDettaglioGPF, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}


function onDataBindingRighe() {

}

var tipoReportGPF;

function letturaDatikendoGPFKendo() {


    var dataInizio = $("#Txt_Data_DA").data("kendoDatePicker").value();
    var dataFine = $("#Txt_Data_A").data("kendoDatePicker").value();

    let grigliaPercorsiDaMemorizzare;

    if (tipoReportGPF === enum_tipoReportGPF.UltimaPosizione) {
        grigliaPercorsiDaMemorizzare = "divkendoGPF_POS";
    }

    switch (tipoReportGPF) {
        case enum_tipoReportGPF.PosizioniRilevate:
            grigliaPercorsiDaMemorizzare = "divkendoPosizioniRilevate";
            break;
        case enum_tipoReportGPF.Percorsi:
            grigliaPercorsiDaMemorizzare = "divkendoGPF";
            break;
        case enum_tipoReportGPF.UltimaPosizione:
            grigliaPercorsiDaMemorizzare = "divkendoGPF_POS";
            break;
        default:
            break;
    }


    gridDestroy("#divkendoPosizioniRilevate");
    gridDestroy("#divkendoGPF");
    gridDestroy("#divkendoGPF_POS");


    var webApi = "";

    var param = JSON.stringify({ DataInizio: dataInizio, DataFine: dataFine, UtenteCorrente: true });

    switch (tipoReportGPF) {
        case enum_tipoReportGPF.Percorsi:
            webApi = "/ReportGPF";
            break;

        case enum_tipoReportGPF.UltimaPosizione:
            webApi = "/UltimaPosizione";
            break;

        case enum_tipoReportGPF.PosizioniRilevate:
            webApi = "/PosizioniRilevate";
            break;

        default:
            webApi = "/ReportGPF";

    }



    ajaxAgronica(indirizzohttp + webApi,
        param,
        function (risposta) {

            $("#hdkendoGPF").val(risposta.RispostaStringa);
            kendoGPFKendo(grigliaPercorsiDaMemorizzare);

        }, null);
}


/**
 * Rimuove la kendo Grid
 * @param {any} jQuerySel selettore jquery con ID (#)
 */
function gridDestroy(jQuerySel) {
    var gridPos = $(jQuerySel).data("kendoGrid");
    if (gridPos !== undefined) {
        gridPos.destroy();
        $(jQuerySel).html("");
    }
}

function kReadkendoGPF_rows(options) {

    var data = $('#hdkendoGPF').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadkendoGPF_col() {

    var data = $('#hdkendoGPF').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadkendoGPF_mod() {

    var data = $('#hdkendoGPF').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}

function kendoGPFonDataBindingRighe() {

}





//per chiamata Standard Ajax
function ApriGIS() {

    var ParametriFiltroXSessione = [];

    let grigliaPercorsiDaMemorizzare;

    switch (tipoReportGPF) {
        case enum_tipoReportGPF.PosizioniRilevate:
            grigliaPercorsiDaMemorizzare = "divkendoPosizioniRilevate";
            break;
        case enum_tipoReportGPF.Percorsi:
            grigliaPercorsiDaMemorizzare = "divkendoGPF";
            break;
        case enum_tipoReportGPF.UltimaPosizione:
            grigliaPercorsiDaMemorizzare = "divkendoGPF_POS";
            break;
        default:
            break;
    }

    var filteredData = kGetElementiSelezionati("#" + grigliaPercorsiDaMemorizzare);

    if (filteredData.length === 0) {
        kendo.alert("Nessuna selezione.");
        return;
    }


    salvaPersonalizzazioniGrigliaKendo(pathCoreWS, "/AgronicaAgenda_2010/DataAnalisiBI/ReportPercorsi/ReportPercorsi.aspx", grigliaPercorsiDaMemorizzare, false);

    $.each(filteredData, function (idx, dataItem) {

        switch (tipoReportGPF) {

            case enum_tipoReportGPF.Percorsi:
                ParametriFiltroXSessione.push(" geodata.elementoGrafico_Des like '%vehicle_id§ " + dataItem.id.split("_")[0] + "%' " +
                    "AND geodata.elementoGrafico_Des like '%from_time§ " + kendo.toString(dataItem.DataOraPartenza, 'dd/MM/yyyy HH:mm:ss') + "%'");
                break;
            case enum_tipoReportGPF.PosizioniRilevate:
                ParametriFiltroXSessione.push(dataItem.id.toString());
                break;
            case enum_tipoReportGPF.UltimaPosizione:
                //ParametriFiltroXSessione.push(" geodata.elementoGrafico_Des like '%vehicle_id§ " + dataItem.id.split("_")[0] + "%|isStop§ 2' ");
                ParametriFiltroXSessione.push(dataItem.id.toString());
                break;
            default:

        }


    });

    var dataInizio = $("#Txt_Data_DA").data("kendoDatePicker").value();
    var dataFine = $("#Txt_Data_A").data("kendoDatePicker").value();

    var jp = " or ";
    if (tipoReportGPF === enum_tipoReportGPF.PosizioniRilevate || tipoReportGPF === enum_tipoReportGPF.UltimaPosizione) {
        jp = ",";
    }

    var pp1 = {
        ParametriFiltroXSessione: ParametriFiltroXSessione.join(jp),
        tipo: tipoReportGPF.value,
        DataInizio: dataInizio,
        DataFine: dataFine
    };

    //var pp = '{ ParametriFiltroXSessione: " ' + ParametriFiltroXSessione.join(" or ") + ' ", tipo: '  + tipoReportGPF.value +  ' } ';

    ajaxAgronica(indirizzohttp + "/RedirGIS", JSON.stringify(pp1),
        function (risposta) {
            var srcRedir = "../../Gis/Gis.aspx";
            window.location = srcRedir;
        }, null);
}


function kEventoSelezionaRiga() {

    let grigliaPercorsiDaMemorizzare;

    switch (tipoReportGPF) {
        case enum_tipoReportGPF.PosizioniRilevate:
            grigliaPercorsiDaMemorizzare = "divkendoPosizioniRilevate";
            break;
        case enum_tipoReportGPF.Percorsi:
            grigliaPercorsiDaMemorizzare = "divkendoGPF";
            break;
        case enum_tipoReportGPF.UltimaPosizione:
            grigliaPercorsiDaMemorizzare = "divkendoGPF_POS";
            break;
        default:
            break;
    }

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#" + grigliaPercorsiDaMemorizzare).data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}

function kendoGPFKendo(divkendoGPF) {

    var funzioniCRUD = {
        funzioneRead: kReadkendoGPF_rows,
        checkBoxFunction: kEventoSelezionaRiga
    };
    var idModel = "id";
    var campiKendoModel = kReadkendoGPF_mod();
    var colonneKendoGrid = kReadkendoGPF_col();
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        pagesize: 20,
        groupable: false,
        impostaColonneKendoGridDaCookie: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        toolbarCommands: ["templateBtn_ApriGIS", "templateBtn_ApriDettaglio"],
        scrollable: false,
        sortable: true,
        resizable: false,
        filterable: { multi: true, search: true },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        pdf: false,
        excel: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBinding: kendoGPFonDataBindingRighe };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["id"];

    creaKendoGrid(divkendoGPF, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    $("#Btn_ApriGIS").click(function () {
        ApriGIS();
    });

    $("#Btn_ApriDettaglio").click(function () {
        letturaDatiKendoDettaglioGPFKendo();
    });
}

var enum_tipoReportGPF = {
    Percorsi: { value: 1, name: "Percorsi", code: 1 },
    UltimaPosizione: { value: 2, name: "UltimaPosizione", code: 2 },
    PosizioniRilevate: { value: 3, name: "PosizioniRilevate", code: 3 }
}

function ReportGPF() {
    tipoReportGPF = enum_tipoReportGPF.Percorsi;
    letturaDatikendoGPFKendo();
}

function ReportPosizioniRilevate() {
    tipoReportGPF = enum_tipoReportGPF.PosizioniRilevate;
    letturaDatikendoGPFKendo();
}

function ElaboraPosizioneAttualeGPF() {
    tipoReportGPF = enum_tipoReportGPF.UltimaPosizione;
    letturaDatikendoGPFKendo();
}

// parte di lettura dei percorsi con raccolte ... 

function ReportPercorsi() {
    ajaxAgronica(indirizzohttp + "/ReportPercorsi", JSON.stringify({}),
        function (risposta) {


            var dd = jQuery.parseJSON(risposta.RispostaStringa);



            AgroWA_Table_sistemaDati(dd); //aggiusto i dati in base al tipo
            $('#tabellaPercorsi').html('');
            waTablePercorsi = $('#tabellaPercorsi').WATable({
                pageSize: 3,
                pageSizes: [3, 10, 15, 20, 30, 40, 50],
                columnPicker: true,
                filter: true,
                preFill: true,
                types: {
                    string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                    date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                    number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                    bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                }
            }).data('WATable').setData(dd);
            InitWaTable("#tabellaPercorsi", waTablePercorsi);


        }, null);
}