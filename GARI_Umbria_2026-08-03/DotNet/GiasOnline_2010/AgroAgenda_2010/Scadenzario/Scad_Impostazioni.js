
function popolaGrigliaScadenze(IDControllo) {

    var templatePulsanti = "";
    if (UtenteAbilitatoScrittura) {
        templatePulsanti = [
            {
                command: {
                    template: "<span class='fa fa-2x fa-pencil-square-o edit_elem' title='" + TraduzioneMultiResx(scadImpostazioniResx, "Modifica", "Modifica") + "' onclick=modificaElemento(this.closest('tr'),this.closest('.k-grid'))></span>" +
                        "<span class='fa fa-2x fa-trash-o del_elem' title='" + TraduzioneMultiResx(scadImpostazioniResx, "Cancella", "Cancella") + "' onclick=confermaEliminaElemento(this.closest('tr'),this.closest('.k-grid'))></span>" +
                        "<span class='fa fa-2x fa-envelope-o test_elem' title='" + TraduzioneMultiResx(scadImpostazioniResx, "InviaMailDiProva", "Invia mail di prova") + "' onclick=testElemento(this.closest('tr'),this.closest('.k-grid'))></span>"
                }, title: TraduzioneMultiResx(scadImpostazioniResx, "Azioni", "Azioni") //, widthfisso: true //width: "135px" - no style in span
            }
        ];
    }

    var funzioniCRUD = { funzioneRead: kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
    var idModel = "ID_Avviso";
    var campiKendoModel = kReadValorizzazione_mod();
    var colonneKendoGrid = kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        columnMenu: false,
        pdf: false,
        groupable: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        //sortable: {mode: "multiple", allowUnsort: true, showIndexes: true},
        filterable: { mode: "menu" },
        //scrollable: false
        colonneCustomKendoGrid: templatePulsanti
    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function kReadValorizzazione_rows(options) {

    var data = $('#hdKendo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazione_col() {

    var data = $('#hdKendo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function kReadValorizzazione_mod() {

    var data = $('#hdKendo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function onDataBoundRighe(e) {
    kendo_AggiustaDimensioneColonne("#" + e.sender.element[0].id);
}


// FUNZIONE PER IMPOSTARE I CONTROLLI DELL'AVVISO
function impostaControlliAvviso(ID_Avviso, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Evento, GGAttesa, MailMittente, MailA, MailCC) {

    $('#ID_Avviso').val(ID_Avviso);
    $('#DdlArea').data("kendoDropDownList").select(function (dataItem) { return dataItem.id_area === ID_Area; });

    CmbArea_change();
    $('#cmbTipologia').data("kendoDropDownList").value(ID_Tipologia);

    $('#cmbRapCon').data("kendoMultiSelect").value(Filtro_RapCon.split(","));
    ddlRapCon_Change();

    $('#TxbGGAttesa').data("kendoNumericTextBox").value(GGAttesa);
    $('#TxbMailMittente').val(MailMittente);
    $('#TxbMailA').val(MailA);
    $('#TxbMailCC').val(MailCC);

    if (UtenteAbilitatoScrittura == true) {
        $('#BtnAvviso_Edit').show();
    }
}

// FUNZIONE PER SVUOTARE I CONTROLLI DELL'AVVISO
function svuotaControlliAvviso() {
    $('#ID_Avviso').val("");
    $('#DdlArea').data("kendoDropDownList").select(0);
    $('#cmbTipologia').data("kendoDropDownList").select(0);
    $('#cmbRapCon').data("kendoMultiSelect").select(0);
    ddlRapCon_Change();
    $('#TxbGGAttesa').data("kendoNumericTextBox").value(null);
    $('#TxbMailMittente').val("");
    $('#TxbMailA').val("");
    $('#TxbMailCC').val("");

    $('#BtnAvviso_Edit').hide();

    //tolgo la classe per l'evidenziazione da tutta la griglia
    $('#tabella_avvisi tr').removeClass("evidenziato");
}

//CONFERMA CANCELLAZIONE DELL'ELEMENTO
function confermaEliminaElemento(tr_elem, grid_elem) {

    var dlg = $("<div></div>").kendoConfirm({
        content: TraduzioneMultiResx(scadImpostazioniResx, "SeiSicuroDiEliminareQuestoElemento", "Sei sicuro di voler eliminare l'elemento?"),
        messages: { okText: TraduzioneMultiResx(scadImpostazioniResx, "Si", "Sì"), cancel: TraduzioneMultiResx(scadImpostazioniResx, "No", "No") },
        title: TraduzioneMultiResx(scadImpostazioniResx, "Conferma", "Conferma")
    }).data("kendoConfirm");

    dlg.result.done(function () { eliminaElemento(tr_elem, grid_elem); });
    dlg.open();

};


// FUNZIONE PER CARICARE LE AREE
function ddlArea_Load() {

    $('#DdlArea').kendoDropDownList({
        dataSource: { transport: { read: RiempiDdlArea } },
        dataTextField: "nome",
        dataValueField: "id_area",
        optionLabel: { "nome": TraduzioneMultiResx(scadImpostazioniResx, "Seleziona", "Seleziona").toUpperCase() + "...", "id_area": "" },
        autoWidth: true,
        dataBound: ddlArea_OnDataBound,
        change: CmbArea_change

    });

}

function RiempiDdlArea(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Area.asmx/Leggi_Aree",
        parametri,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}




function ddlArea_OnDataBound(e) {
   
    CmbArea_change(); //forzo l'evento di onchange
}


function RiempicmbTipologia(options) {
    options.success(Elenco_Tipologie);
}


function CmbArea_change(options) {

    var id_area = KendoDDL("DdlArea").dataItem().id_area;

    if (id_area == 0) {
        Elenco_Tipologie = "";
    }
    else {
        Elenco_Tipologie = Elenco_Tipologie_Riempi(id_area);
    }

    $("#cmbTipologia").data("kendoDropDownList").dataSource.read();
}

function Elenco_Tipologie_Riempi(id_area) {


     ajaxAgronicaSync("./Indici_Documentale.aspx/Leggi_Tipologie",
         "{ piva: '', " +
         "  id_area: " + id_area + "}",
         false,
         function (risposta) {
             risp = JSON.parse(risposta.RispostaStringa);

             var objVuoto = { "ID_Tipologia": 0, "Nome": "" };
             risp.unshift(objVuoto);

             risultato_lettura = risp;
         }, null);


    return risultato_lettura;

}




function ddlRapCon_OnDataBound(e) {

}

function ddlRapCon_Change() {

    //Controllo Impostazione Rapporto Contabile --> Mail Non Abilitato
    var Filtro_RapCon = KendoMultisel("cmbRapCon").value().join(",");

    if (Filtro_RapCon !== "") {
        $("#id_MailA").css("display", "none");
    }
    else {
        $("#id_MailA").css("display", "block");
    }

}



function RiempiDdlRapCon(options) {
    options.success(Elenco_RapCon);
}