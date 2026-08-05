function CaricaControlli() {

    $("#Data_Irrigazione").kendoDatePicker({
        change: Data_Irrigazione_change
    });

    let Operazione = parseInt($(TipoOperazione).val());
    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = "";
    let Irrigazioni = "";
    let Sup_Selezionata = 0;
    let NoteIntervento = "";
    let MsgSportello = "";

    if (Dati !== null && Dati !== undefined && Dati !== "") {
        objParametriAgenda = JSON.parse(Dati.objParametriAgenda);
        Irrigazioni = JSON.parse(Dati.irrigazioni);
        Sup_Selezionata = JSON.parse(Dati.Sup_Selezionata);
        NoteIntervento = Dati.NoteIntervento;
        MsgSportello = Dati.MsgSportello;
    }


    //Imposto il DatePicker
    let Data = kendo.toString(kendo.parseDate(new Date()), 'dd/MM/yyyy');
    if (objParametriAgenda.Data !== undefined)
        Data = kendo.toString(kendo.parseDate(objParametriAgenda.Data), 'dd/MM/yyyy');

    $("#Data_Irrigazione").data("kendoDatePicker").value(Data);

    creaKendoDropDownList("ddl_Centro_Aziendale_Irrigazione", { read: Leggi_CentriAziendali }, "Sa_Nome", "Sa_Cod").bind("change", ddl_Centro_Aziendale_Irrigazione_change);;

    if (objParametriAgenda.Sa_Cod !== undefined && objParametriAgenda.Sa_Cod !== null && objParametriAgenda.Sa_Cod !== "") {
        Set_KendoDDLValue("ddl_Centro_Aziendale_Irrigazione", parseInt(objParametriAgenda.Sa_Cod));

        //Se l'objParametriAgenda.Sa_Cod non è un valore corretto imposto Tutti i Centri Aziendali
        let ddl_Centro_Aziendale_Value = Get_KendoDDLValue("ddl_Centro_Aziendale_Irrigazione");

        if (ddl_Centro_Aziendale_Value === undefined || ddl_Centro_Aziendale_Value === null || ddl_Centro_Aziendale_Value === "") {
            Set_KendoDDLValue("ddl_Centro_Aziendale_Irrigazione", 0);
        }
    }


    //Aggiungo un attributo al tag html della DDL Tipo Irrigazione 
    $("#ddl_Tipo_Irrigazione").attr("last_selected_imp_cod", -2);

    creaKendoDropDownList("ddl_Tipo_Irrigazione", { read: Leggi_TipoIrrigazione }, "Imp_DES", "Imp_COD").bind("change", ddl_Tipo_Irrigazione_change);

    //Aggiungo un attributo al tag html della DDL della Specie per sapere qual'era la specie scelta prima di quella attuale
    $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod", objParametriAgenda.Veg_Cod);

    creaKendoDropDownList("ddl_Specie_Irrigazione", { read: Leggi_Specie }, "Veg_Des", "Veg_Cod").bind("change", ddl_Specie_Irrigazione_change);

    if (objParametriAgenda.Veg_Cod !== undefined && objParametriAgenda.Veg_Cod !== null && objParametriAgenda.Veg_Cod !== "") {

        Set_KendoDDLValue("ddl_Specie_Irrigazione", objParametriAgenda.Veg_Cod);

        //Se l'objParametriAgenda.Veg_Cod non è un valore corretto imposto valore vuoto
        let dd_Specie_Value = Get_KendoDDLValue("ddl_Specie_Irrigazione");

        if (dd_Specie_Value === undefined || dd_Specie_Value === null || dd_Specie_Value === "") {
            Set_KendoDDLValue("ddl_Specie_Irrigazione", "-1");
            $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod", "-1");
        }
    }

    $("#Txt_SupSelezionataIrrigazione").val(kendo.toString(Sup_Selezionata, "n4"));

    creaKendoDropDownList("ddl_Udm_Irrigazione", { read: Leggi_Udm }, "Udm_Des", "Udm_Cod").bind("change", ddl_Udm_Irrigazione_change);

    if (Irrigazioni.length > 0 && Irrigazioni[0].UDM_Dose !== undefined)
        Set_KendoDDLValue("ddl_Udm_Irrigazione", parseInt(Irrigazioni[0].UDM_Dose));

    //Eseguo il databound solo la prima volta che viene creata la grid e quando si cambiano dei valori della maschera,
    //altrimenti il databound viene fatto ogni volta che si fa grid.refresh() o che si richiama la funzione di creazione 
    //della creazione della grid "kendoGrid_Impianti_Irrigazione()".

    if (Controlli_Prima_Di_creare_la_kendoGrid_Impianti_Irrigazione() === true) {
        $("#grid_Impianti_Irrigazione").attr("dataBounded", "false");
        //Carico la grid
        kendoGrid_Impianti_Irrigazione("grid_Impianti_Irrigazione");
    }



    //Imposto la tab delle Note 
    $("#Txt_Note").val(NoteIntervento);

    GestioneTabDivNote();

    //Rilievi Pioggie
    $("#Data_Da_Irrigazione").kendoDatePicker();

    $("#Data_A_Irrigazione").kendoDatePicker();


    if (objParametriAgenda.TipoOperazioneAgenda !== undefined &&
        (parseInt(objParametriAgenda.TipoOperazioneAgenda) === enum_tipoOperazione_Agenda.Ricetta ||
            parseInt(objParametriAgenda.TipoOperazioneAgenda) === enum_tipoOperazione_Agenda.RicettaBrogliaccio)) {

        Configuratore_Maschera_RicettaBrogliaccio(objParametriAgenda);
    }


    //Disabilita i Controlli in base all'Operazione
    switch (Operazione) {
        case enum_tipoOperazione.Scrittura:
            disabilitaControlliNUOVO_Irrigazione(objParametriAgenda);
            break;
        case enum_tipoOperazione.Modifica:
            disabilitaControlliMODIFICA_Irrigazione(objParametriAgenda);
            break;
        case enum_tipoOperazione.Lettura:
            disabilitaControlliINFO_Irrigazione(objParametriAgenda);
            break;
    }



    //Se lo sportello è chiuso in questa data nascondo i pulsanti di salvataggio e
    //mostro il messaggio.
    if (MsgSportello !== "") {

        $("#divSalva").hide();
        $("#divSalvaNuovo").hide();
        $("#divSalvaDuplica").hide();

        MessaggioErrore(MsgSportello, "DIV_Messaggi");
    }
}


function Configuratore_Maschera_RicettaBrogliaccio(objParametriAgenda) {


    if (objParametriAgenda.TipoOperazioneAgenda !== undefined &&
        parseInt(objParametriAgenda.TipoOperazioneAgenda) === enum_tipoOperazione_Agenda.Ricetta) {
        //Date Ricetta
        $("#Data_Inizio_ricetta_Irrigazione").kendoDatePicker();

        $("#Data_Fine_ricetta_Irrigazione").kendoDatePicker();
    }


    if (objParametriAgenda.TipoOperazioneAgenda !== undefined &&
        parseInt(objParametriAgenda.TipoOperazioneAgenda) === enum_tipoOperazione_Agenda.RicettaBrogliaccio) { //ricetta || brogliaccio

        if ((window.location.search.indexOf("?r=") !== -1 || window.location.search.indexOf("&r=") !== -1) &&
            (objParametriAgenda.TipoRicetta !== undefined && parseInt(objParametriAgenda.TipoRicetta) !== 9)) { //Escludo il PUA dove la testata della ricetta esiste già
            $("#divSalva").show();
            $("#divSalvaDuplica").hide();

            $("#divSalva").removeClass("col-lg-8").addClass("col-lg-12");

        }

    }


    if (objParametriAgenda_VisualizzaSoloBottoneSalvaEsci === true) { //ricetta || brogliaccio

        $("#divSalvaNuovo").hide();
        $("#divSalvaDuplica").hide();

        $("#divSalva").removeClass("col-lg-8").addClass("col-lg-12");


    }

}


function disabilitaControlliNUOVO_Irrigazione(objParametriAgenda) {


    if (parseInt(objParametriAgenda.TipoOperazioneAgenda) !== undefined &&
        parseInt(objParametriAgenda.TipoOperazioneAgenda) === enum_tipoOperazione_Agenda.Ricetta) { //ricetta + operazione ricettabile

        $("#divSalva").hide();
        $("#divSalvaNuovo").hide();
        $("#divSalvaDuplica").hide();

        $("#divSalvaRicetta").show();

        if (objParametriAgenda.TipoRicetta !== undefined &&
            objParametriAgenda.TipoRicetta !== 9) {
            $("#divTestataRicetta_Irrigazione").show();
        }
        if ($(Ricetta_Cod).val() !== "0") {
            var ricettaJSON = caricaTestataRicetta_Irrigazione(parseInt($(Ricetta_Cod).val()));
            popolaTestataRicetta_Irrigazione(ricettaJSON);
        }
        popolaMenuRicette();
        $("#menuRicette_Irrigazione").kendoMenu();
        $("#divRicetteSalvaEVai").show();

    } else {

        $("#divSalvaRicetta").hide();
        $("#divTestataRicetta_Irrigazione").hide();
        $("#divRicetteSalvaEVai").hide();

    }
}


function disabilitaControlliMODIFICA_Irrigazione(objParametriAgenda) {

    $("#divSalvaNuovo").hide();
    $("#divSalvaDuplica").hide();

    $("#divSalva").removeClass("col-lg-8").addClass("col-lg-12");

    $("#divSalvaRicetta").hide();
    $("#divTestataRicetta_Irrigazione").hide();

    //Centro Aziendale sempre disabilitato in MODIFICA
    if ($("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") !== undefined &&
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") !== null &&
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") !== "")

        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList").enable(false);


    if (parseInt(objParametriAgenda.TipoOperazioneAgenda) !== undefined &&
        parseInt(objParametriAgenda.TipoOperazioneAgenda) === enum_tipoOperazione_Agenda.Ricetta) { //ricetta

        $("#divSalva").hide();
        $("#divSalvaRicetta").show();
        $("#divSalvaRicetta").removeClass("col-lg-4").addClass("col-lg-12");

        if (parseInt(objParametriAgenda.TipoRicetta) !== undefined &&
            parseInt(objParametriAgenda.TipoRicetta) !== 9) {
            $("#divTestataRicetta_Irrigazione").show();
        }
        var ricettaJSON = caricaTestataRicetta_Irrigazione(parseInt($(Ricetta_Cod).val()));
        popolaTestataRicetta_Irrigazione(ricettaJSON);

        popolaMenuRicette();
        $("#menuRicette_Irrigazione").kendoMenu();
        $("#divRicetteSalvaEVai").show();
    }
    else {
        //Mostro solo il bottono Salva ed Esci gli altri due li nascondo
        $("#divSalva").show();
        $("#divSalva").removeClass("col-lg-8").addClass("col-lg-12");
        $("#divSalvaNuovo").hide();
        $("#divSalvaDuplica").hide();
    }

}


function disabilitaControlliINFO_Irrigazione(objParametriAgenda) {

    $("#Note_grid_Impianti_Irrigazione").show();

    $("#divSalva").hide();
    $("#divSalvaNuovo").hide();
    $("#divSalvaDuplica").hide();

    $("#divSalvaRicetta").hide();


    if ($("#Data_Irrigazione").data("kendoDatePicker") !== undefined &&
        $("#Data_Irrigazione").data("kendoDatePicker") !== null &&
        $("#Data_Irrigazione").data("kendoDatePicker") !== "")

        $("#Data_Irrigazione").data("kendoDatePicker").enable(false);


    if ($("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== undefined &&
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== null &&
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== "")

        $("#ddl_Specie_Irrigazione").data("kendoDropDownList").enable(false);

    if ($("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") !== undefined &&
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") !== null &&
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") !== "")

        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList").enable(false);


    if ($("#ddl_Udm_Irrigazione").data("kendoDropDownList") !== undefined &&
        $("#ddl_Udm_Irrigazione").data("kendoDropDownList") !== null &&
        $("#ddl_Udm_Irrigazione").data("kendoDropDownList") !== "")

        $("#ddl_Udm_Irrigazione").data("kendoDropDownList").enable(false);


    $("#chkMicroirr").attr("disabled", true);


    if ($("#ddl_Tipo_Irrigazione").data("kendoDropDownList") !== undefined &&
        $("#ddl_Tipo_Irrigazione").data("kendoDropDownList") !== null &&
        $("#ddl_Tipo_Irrigazione").data("kendoDropDownList") !== "")

        $("#ddl_Tipo_Irrigazione").data("kendoDropDownList").enable(false);

    disabilita_tutte_TabDivNote();

}


function disabilita_tutte_TabDivNote() {
    $('div#checkbox_a_tabGiust input[type=checkbox]').each(function () {
        $(this).attr("disabled", true);
    });

    $('#Txt_Note').prop('disabled', true);

    $('div#checkbox_a_tabMeteo input[type=checkbox]').each(function () {
        $(this).attr("disabled", true);
    });


    $('div#checkbox_a_tabVentoIntensita input[type=checkbox]').each(function () {
        $(this).attr("disabled", true);
    });


    $('div#checkbox_a_tabVentoDirezione input[type=checkbox]').each(function () {
        $(this).attr("disabled", true);
    });


    $('div#checkbox_a_tabTemperatura input[type=checkbox]').each(function () {
        $(this).attr("disabled", true);
    });


    $('div#checkbox_a_tabOrario input[type=checkbox]').each(function () {
        $(this).attr("disabled", true);
    });


    $('div#checkbox_a_tabMotivazioni input[type=checkbox]').each(function () {
        $(this).attr("disabled", true);
    });
}

function Controlli_Prima_Di_creare_la_kendoGrid_Impianti_Irrigazione() {

    let genera_grid = true;

    if ($("#ddl_Specie_Irrigazione").data("kendoDropDownList") === undefined ||
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList") === null ||
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList") === "" ||
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === undefined ||
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === null ||
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === "" ||
        $("#Data_Irrigazione").data("kendoDatePicker") === undefined ||
        $("#Data_Irrigazione").data("kendoDatePicker") === null ||
        $("#Data_Irrigazione").data("kendoDatePicker") === "")
        genera_grid = false;


    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = "";


    if (Dati !== null && Dati !== undefined && Dati !== "") {
        objParametriAgenda = JSON.parse(Dati.objParametriAgenda);
        Irrigazioni = kendoEscapeOggetto(JSON.parse(Dati.irrigazioni));
    }


    if ((objParametriAgenda !== "") &&
        (objParametriAgenda.Lav_Cod === undefined || objParametriAgenda.Disciplinare === undefined ||
            objParametriAgenda.Impianti === undefined || objParametriAgenda.Cul_Cod === undefined))
        genera_grid = false;


    var data_irrigazione_value = $("#Data_Irrigazione").data("kendoDatePicker").value();

    if (data_irrigazione_value === undefined ||
        data_irrigazione_value === null)
        genera_grid = false;


    return genera_grid;
}

function kendoGrid_Impianti_Irrigazione(divKendoGrid_Impianti_Irrigazione) {

    //Genero la griglia lato_server
    CreaKendoGrid_Impianti_Irrigazione();

    //Messaggio "Dati Irrigazione Impianti" sopra la grid Impianti irrigazione
    $("#Mess_grid_Impianti_Irrigazione").html("");
    $("#Mess_grid_Impianti_Irrigazione").empty();
    $("#Mess_grid_Impianti_Irrigazione").html("<strong>Dati Irrigazione Impianti</strong>");

    var Operazione = parseInt($(TipoOperazione).val());

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var InserimentoModifica = false;
    var Cancellazione = false;

    var funzioniCRUD = {};

    //Se l'Utente ha il permesso di modifica allora aggiungo la colonna con i check altrimenti no
    if (UteAbilitatoInsMod === true && UteAbilitatoCanc === true) {
        InserimentoModifica = true;
    }


    funzioniCRUD.funzioneRead = kReadImpianti_Irrigazione;   //kendo_rows
    funzioniCRUD.funzioneSubmit = { funzione: SubmitGrigliaImpianti_Irrigazione, flagInsert: false, flagDelete: false };
    funzioniCRUD.UtenteAbilitatoInserimentoModifica = InserimentoModifica;
    funzioniCRUD.UtenteAbilitatoCancellazione = Cancellazione;
    funzioniCRUD.omettiPulsantiSalva = true;
    funzioniCRUD.omettiPulsantiAnnulla = true;
    funzioniCRUD.checkBoxFunction = kEventoSelezionaRigaGrid_Impianti_Irrigazione;


    var ToolbarCommands = ["templateBtnFiltraColonneGrid_Impianti_Irrigazione"];

    //Se sono in Lettura nascondo il bottone della griglia che permette di impostare i valori nella grid
    if (Operazione !== enum_tipoOperazione.Lettura)
        ToolbarCommands.push("templateBtnImpostaValoriGrid_Impianti_Irrigazione");

    var idModel = "Id_Grid_Irrigazione";
    var campiKendoModel = kReadImpianti_Irrigazione_mod(); //kendo_model
    var colonneKendoGrid = kReadImpianti_Irrigazione_col(); //kendo_columns
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: false,
        columnMenu: false,
        toolbarCommands: ToolbarCommands,
        reorderable: true,
        excel: false,
        pdf: false,
        groupable: false,
        btnEliminaTuttiFiltri: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDataBound: grid_impianti_irrigazione_beforedataBound,
        funzioneDaChiamarePrimaDiSelectAllRows: grid_impianti_irrigazione_beforeSelectAllRows,
        funzioneDaChiamareDopoSelectAllRows: grid_impianti_irrigazione_afterSelectAllRows
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = ["kendoKey", "Id_Grid_Irrigazione"];

    creaKendoGrid(divKendoGrid_Impianti_Irrigazione, // rappresenta l'ID del div a cui si associa la griglia
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

    let grid_impianti_irrigazione = $("#" + divKendoGrid_Impianti_Irrigazione).data("kendoGrid");

    grid_impianti_irrigazione.bind("cellClose", grid_impianti_irrigazione_cellClose);

    grid_impianti_irrigazione.bind("beforeEdit", grid_impianti_irrigazione_beforeEdit);

    RicalcolaSuperficieTotaleIrrigazione();

    Abilita_Disabilita_Resto();

    Imposta_nome_Colonna_Dose_Nella_grid_Impianti_Irrigazione();

    //Svuoto l'hf della kendo grid
    $(KendoGridmpianti_Irrigazione).val("");
}


function kReadImpianti_Irrigazione(options) {

    if ($(KendoGridmpianti_Irrigazione).val() === undefined ||
        $(KendoGridmpianti_Irrigazione).val() === "" ||
        $(KendoGridmpianti_Irrigazione).val() === null)
        return;

    var data = $(KendoGridmpianti_Irrigazione).val();

    jSonParsed_Kendo = JSON.parse(data);

    var grid_rows = jSonParsed_Kendo.kendo_rows;

    //Calcolo la colonna "Quantità Totale di acqua nel periodo"
    for (var x = 0; x < grid_rows.length; x++) {

        let dataItem = new Object();

        if (grid_rows[x].Data_Inizio !== undefined && grid_rows[x].Data_Inizio !== null && grid_rows[x].Data_Inizio !== "")
            dataItem.Data_Inizio = kendo.parseDate(grid_rows[x].Data_Inizio.toString(), "dd/MM/yyyy");
        else
            dataItem.Data_Inizio = null;

        if (grid_rows[x].Data_Fine !== undefined && grid_rows[x].Data_Fine !== null && grid_rows[x].Data_Fine !== "")
            dataItem.Data_Fine = kendo.parseDate(grid_rows[x].Data_Fine.toString(), "dd/MM/yyyy");
        else
            dataItem.Data_Fine = null;

        dataItem.Qta_Totale = grid_rows[x].Qta_Totale;
        dataItem.Frequenza = grid_rows[x].Frequenza;
        dataItem.App_Nome = grid_rows[x].App_Nome;

        grid_rows[x].Qta_Totale_Acqua_Periodo = 0;

        let Qta_Totale_Acqua_Periodo = Calcola_Qta_Totale_Acqua_Periodo(dataItem);

        if (Qta_Totale_Acqua_Periodo !== undefined && Qta_Totale_Acqua_Periodo !== null) {
            grid_rows[x].Qta_Totale_Acqua_Periodo = Qta_Totale_Acqua_Periodo;
        }

    }

    options.success(grid_rows);
}

function kReadImpianti_Irrigazione_mod() {

    if ($(KendoGridmpianti_Irrigazione).val() === undefined ||
        $(KendoGridmpianti_Irrigazione).val() === "" ||
        $(KendoGridmpianti_Irrigazione).val() === null)
        return;


    var data = $(KendoGridmpianti_Irrigazione).val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}


function kReadImpianti_Irrigazione_col() {

    if ($(KendoGridmpianti_Irrigazione).val() === undefined ||
        $(KendoGridmpianti_Irrigazione).val() === "" ||
        $(KendoGridmpianti_Irrigazione).val() === null)
        return;


    var data = $(KendoGridmpianti_Irrigazione).val();
    jSonParsed_Kendo = JSON.parse(data);

    var grid_columns = jSonParsed_Kendo.kendo_columns;


    for (var x = 0; x < grid_columns.length; x++) {

        //Aggiungo l'editor per modificare il tipo di irrigazione alla colonna per la Dropdown
        if (grid_columns[x].field === "ModifTipoIrriUtilizzata_Des") {
            grid_columns[x].editor = ModificaTipoIrrigazioneUtilizzata_DropdownEditor;
        }

        //Imposto anche le numericTextBox delle rispettive colonne con il formato corretto
        if (grid_columns[x].field === "Qta2" || grid_columns[x].field === "Qta_Totale") {
            grid_columns[x].editor = numberEditor4decimals;
        }

        if (grid_columns[x].field === "Ore" || grid_columns[x].field === "Dose" || grid_columns[x].field === "Qta_Acqua_Consiglio_Custom") {
            grid_columns[x].editor = numberEditor2decimals;
        }

        if (grid_columns[x].field === "Portata" || grid_columns[x].field === "Frequenza") {
            grid_columns[x].editor = numberEditor0decimals_Grid_Irrigazione;
        }

        if (grid_columns[x].field === "Descrizione_DSS_Irrigazione") {
            grid_columns[x].editor = DSS_Irrigazione_DropdownEditor;
            grid_columns[x].defaultValue = 0;
            grid_columns[x].width = 280;
        }
    }

    AddComnandCoumn(grid_columns);

    return grid_columns;
}

function AddComnandCoumn(grid_columns) {

    let index_after_DSS_Irrigazione = grid_columns.findIndex(col => col.field === "Descrizione_DSS_Irrigazione") + 1;

    let comandcolumn = {
        command: [
            { text: "Apri DSS Irrigazione", click: OpenDSS, className: "btn-DSS" },
            { text: "Visualizza tutti i Turni Salvati", click: ShowTurni, className: "btn-DSS" }
        ],
        title: " ",
        width: "180px",
        attributes: { style: "text-align:center; vertical-align:middle;" },
        headerAttributes: { style: "text-align:center;" } 
    };

    grid_columns.splice(index_after_DSS_Irrigazione, 0, comandcolumn);
}

function OpenDSS(e) {

    let tr_elem = e.currentTarget.closest('tr');

    let dataItem = this.dataItem(tr_elem);

    if (dataItem !== null && dataItem.PIVA !== "" && dataItem.SA_COD !== 0 && dataItem.APPEZZA !== 0 && dataItem.ID_REG !== 0) {

        let impStr = dataItem.PIVA + "_" + dataItem.SA_COD + "_" + dataItem.APPEZZA + "_" + dataItem.ID_REG;

        let url = "../DataAnalisiBI/DSS_Irrigazione/DSS_Irrigazione.aspx?ifr=1&imp=" + impStr ;

        $(document.body).append('<div id="dss_irrigazione"></div>');
        $('#dss_irrigazione').kendoWindow({
            title: "DSS Irrigazione",
            modal: true,
            resizable: true,
            iframe: true,
            width: "80%",
            height: "80%",
            content: url,
            actions: ["Maximize", "Close"],
            close: function () {
                $('#dss_irrigazione').kendoWindow('destroy');
            }
        }).data('kendoWindow').center();

    }


}

function ShowTurni(e) {

    let tr_elem = e.currentTarget.closest('tr');

    let dataItem = this.dataItem(tr_elem);

    if (dataItem !== null) {

        if (dataItem.ID_DSS_Irrigazione === null || dataItem.ID_DSS_Irrigazione === 0 || dataItem.Min_Data_Turno === null || dataItem.Max_Data_Turno === null) {
            kendo.alert("Scegliere un Consiglio con Turni");
            return;
        }

        let turni = Leggi_Turni_Salvati(dataItem.ID_DSS_Irrigazione);

        if (turni !== null && turni.length > 0) {

            Turni_Salvati = turni;

            $(document.body).append('<div id="turni_irrigazione"></div>');

            $("#turni_irrigazione").append('<div class="row" style="margin: 5px; width: 250px;"><span>Turni Salvati per il Consiglio di Irrigazione</span> <span style="font-weight: bold;">' + dataItem.Descrizione_DSS_Irrigazione +'</span></div>');

            $("#turni_irrigazione").append('<div class="row" style="align-items: center; margin: 5px;" id="div_DataFiltro"></div>');

            $("#div_DataFiltro").append('<label class="input-group-addon alert-info" for="DataFiltro">Data</label> <input id="DataFiltro" class="form-control">');

            $("#DataFiltro").kendoDatePicker({
                change: DataFiltroTurni_change
            });

            $("#turni_irrigazione").append('<div class="row" style="margin: 5px;"><select id="listbox_turni" style="height: 200px; width: 250px;"></select></div>');

            $("#listbox_turni").kendoListBox({
                dataSource: turni,
                template: kendo.template($("#tmp_listbox_turni").html())
            });

            $('#turni_irrigazione').kendoWindow({
                title: "Turni Salvati",
                modal: true,
                resizable: false,
                width: "auto",
                height: "auto",
                actions: ["Close"],
                close: function () {
                    Turni_Salvati = [];
                    $('#turni_irrigazione').kendoWindow('destroy');
                }
            }).data('kendoWindow').center();
        }


    }

}

function DataFiltroTurni_change(e) {

    if (Turni_Salvati !== null && Turni_Salvati.length > 0) {

        let Turni_Filtrati = JSON.parse(JSON.stringify(Turni_Salvati));

        let DataFiltro = $("#DataFiltro").data("kendoDatePicker").value();

        //Filtro i Turni per Data
        if (DataFiltro !== undefined && DataFiltro !== null)
            Turni_Filtrati = Turni_Filtrati.filter(t => { return CompareOnlyDatePart(new Date(t.Data_Turno), new Date(DataFiltro));});
        

        $("#listbox_turni").data("kendoListBox").setDataSource(new kendo.data.DataSource({
            data: Turni_Filtrati
        }));
    }
}

function CompareOnlyDatePart(date1, date2) {

    let date1WithoutTime = new Date(date1.getFullYear(), date1.getMonth(), date1.getDate());

    let date2WithoutTime = new Date(date2.getFullYear(), date2.getMonth(), date2.getDate());

    return date1WithoutTime.getTime() === date2WithoutTime.getTime();
}



function timeEditor_Grid_Irrigazione(container, options) {
    $('<input data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '"/>')
        .appendTo(container)
        .kendoTimePicker({
            change: function () {
                var prev = this.options.previous;
                var value = this.value();
                if (value === null) {
                    // In caso di errori
                    this.value(new Date(1900, 1, 1, 0, 0));
                }
            },
            min: new Date(1900, 1, 1, 0, 0),
            max: new Date(2100, 12, 31, 23, 30)
        });
}

function numberEditor0decimals_Grid_Irrigazione(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            format: "{0:0}",
            decimals: 0
        });
}

function Calcola_Qta_Totale_Acqua_Periodo(dataItem) {

    if (dataItem === undefined || dataItem === null) {
        return null;
    } else {

        let Inizio = dataItem.Data_Inizio;

        let Fine = dataItem.Data_Fine;

        if (Inizio !== null && Fine !== null) {

            let result = 0;

            let giorni_irrigati = Get_Giorni_Irrigati(Inizio, Fine, dataItem.Frequenza);

            if (giorni_irrigati !== null) {
                result = giorni_irrigati * dataItem.Qta_Totale;

                if (isNaN(result) || result === Infinity)
                    result = 0;
            }


            return result;
        }

    }
}

function Get_Giorni_Irrigati(Inizio, Fine, Frequenza) {

    let Difference_In_Time = Fine.getTime() - Inizio.getTime();

    //Aggiungo un + 1 al giorno perchè così se irrigo solo un giorno
    //esempio inizio 01/01/2021 fine 01/01/2021 viene contato anche questo come giorno
    let Difference_In_Days = (Difference_In_Time / (1000 * 3600 * 24)) + 1;

    //Arrotondo per difetto i giorni in cui effettivamente si è irrigato
    return Math.floor(Difference_In_Days / Frequenza);
}


function grid_impianti_irrigazione_beforedataBound(e) {
    //Se SelezionaImpianto = true allora seleziono la riga corrispondente

    let Operazione = parseInt($(TipoOperazione).val());

    if ($("#grid_Impianti_Irrigazione").attr("dataBounded") === "false") {
        var ds = $("#grid_Impianti_Irrigazione").data("kendoGrid").dataSource.data();
        let count = 0;

        for (var x = 0; x < ds.length; x++) {
            if (ds[x].SelezionaImpianto === true || ds[x].SelezionaImpianto === "true") {
                ds[x].Selected = true;
                count++;
            }
        }

        //Se tutte le righe sono selezionate allora checko la checkbox della colonna
        if (ds.length > 0 && ds.length === count)
            $("#grid_Impianti_Irrigazione-header-chb").prop("checked", true);

        $("#grid_Impianti_Irrigazione").attr("dataBounded", "true");
    }

    //Disabilito la colonna delle checkbox della grid se sono in Lettura oppure se non ho il permesso di scrittura
    let UtenteAbilitatoInScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    if (Operazione === enum_tipoOperazione.Lettura || UtenteAbilitatoInScrittura === false) {
        $("#grid_Impianti_Irrigazione-header-chb").attr('disabled', true);
        var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");
        var allRows = grid.items();
        $.each(allRows, function (index, value) {
            $(value).find(".k-checkbox").attr('disabled', true);
        })
    }
}

function grid_impianti_irrigazione_cellClose(e) {

    if (e.type == "save" && e.model.Selected === true) {

        const grid_Impianti_Irrigazione = $("#grid_Impianti_Irrigazione").data("kendoGrid");

        input = e.container.find("input[name='Data_Fine']").data("kendoDatePicker");
        if (input != undefined) {
            if (input.value() == "" || input.value() == undefined || input.value() == null)
                e.model.Data_Fine = null;
        }

        input = e.container.find("input[name='Data_Inizio']").data("kendoDatePicker");
        if (input != undefined) {
            if (input.value() == "" || input.value() == undefined || input.value() == null)
                e.model.Data_Inizio = null;
        }

        let input_DDL_DSS_Irrigazione = e.container.find("input[name='ID_DSS_Irrigazione']").data("kendoDropDownList");

        if (input_DDL_DSS_Irrigazione !== undefined) {
            if (input_DDL_DSS_Irrigazione.value() != "" && input_DDL_DSS_Irrigazione.value() != undefined && input_DDL_DSS_Irrigazione.value() != null) {
                let obj_DSS_Irrigazione = input_DDL_DSS_Irrigazione.dataItem();

                if (obj_DSS_Irrigazione !== undefined && obj_DSS_Irrigazione !== null && parseInt(obj_DSS_Irrigazione.ID_DSS_Irrigazione) > 0) {

                    let Udm_Cod_DSS = parseInt(obj_DSS_Irrigazione.Udm_Cod_DSS_Irrigazione);

                    if (Udm_Cod_DSS > 0 && $("#ddl_Udm_Irrigazione").data("kendoDropDownList").dataSource.data().filter(x => x.Udm_Cod === Udm_Cod_DSS).length > 0) {

                        Set_KendoDDLValue("ddl_Udm_Irrigazione", Udm_Cod_DSS);

                        ddl_Udm_Irrigazione_change();

                        e.model.Data_Consiglio_Custom = new Date(obj_DSS_Irrigazione.Data_Consiglio);

                        //Se sono valorizzate le date minime e massime del turno vuol dire che ho dei turni.
                        //E quindi per ottenre la Dose prendo la Qta di Acqua che ho dal consiglio e la divido per i giorni effetivi che ho irrigato
                        if (obj_DSS_Irrigazione.Min_Data_Turno !== null && obj_DSS_Irrigazione.Max_Data_Turno !== null) {

                            e.model.Data_Inizio = new Date(obj_DSS_Irrigazione.Min_Data_Turno);
                            e.model.Data_Fine = new Date(obj_DSS_Irrigazione.Max_Data_Turno);
                            e.model.Frequenza = 1;

                            let Dose = Calcola_Dose_from_Turni(obj_DSS_Irrigazione, e.model.Frequenza);

                            e.model.Dose = Dose;
                            e.model.Qta_Acqua_Consiglio_Custom = Dose;


                        } else {

                            e.model.Data_Inizio = null;
                            e.model.Data_Fine = null;
                            e.model.Frequenza = 0;

                            e.model.Dose = obj_DSS_Irrigazione.Qta_Acqua_DSS_Irrigazione;
                            e.model.Qta_Acqua_Consiglio_Custom = obj_DSS_Irrigazione.Qta_Acqua_DSS_Irrigazione;


                        }

                        grid_impianti_Dose_CellClose(e.model);
                    }

                } else {
                    e.model.Dose = 0;
                    e.model.Qta_Acqua_Consiglio_Custom = 0;
                    e.model.Data_Inizio = null;
                    e.model.Data_Fine = null;
                    e.model.Data_Consiglio_Custom = null;
                    e.model.Frequenza = 0;
                }
            }
        }

        input = e.container.find("input[name='Dose']").data("kendoNumericTextBox");

        if (input != undefined) {
            if (input.value() !== "" && input.value() !== undefined && input.value() !== null && e.model.dirty === true) {
                grid_impianti_Dose_CellClose(e.model);
            }
        }

        input = e.container.find("input[name='Ore']").data("kendoNumericTextBox");
        if (input != undefined) {
            if (input.value() !== "" && input.value() !== undefined && e.model.dirty === true) {

                let nuovo_model = OreCambiataIrrigazione(e.model);

                if (nuovo_model !== undefined && nuovo_model !== null &&
                    nuovo_model.Qta_Totale !== undefined && nuovo_model.Qta_Totale !== null &&
                    nuovo_model.Dose !== undefined && nuovo_model.Dose !== null) {

                    e.model.Qta_Totale = nuovo_model.Qta_Totale;
                    e.model.Dose = nuovo_model.Dose;
                }
            }
        }

        input = e.container.find("input[name='Portata']").data("kendoNumericTextBox");
        if (input != undefined) {
            if (input.value() !== "" && input.value() !== undefined && input.value() !== null && e.model.dirty === true && e.model.Ore !== null) {


                let nuovo_model = PortataCambiataIrrigazione(e.model);

                if (nuovo_model !== undefined && nuovo_model !== null &&
                    nuovo_model.Qta_Totale !== undefined && nuovo_model.Qta_Totale !== null &&
                    nuovo_model.Dose !== undefined && nuovo_model.Dose !== null) {

                    e.model.Qta_Totale = nuovo_model.Qta_Totale;
                    e.model.Dose = nuovo_model.Dose;

                }
            }
        }

        input = e.container.find("input[name='Qta_Totale']").data("kendoNumericTextBox");
        if (input != undefined) {
            if (input.value() !== "" && input.value() !== undefined && input.value() !== null && e.model.dirty === true) {
                let Dose = QtaTotCambiataIrrigazione(e.model);

                //aggiorno la dose e aggiorno le ore e la portata
                e.model.Dose = Dose;

                e.model.Ore = 0;
                e.model.Portata = 0;
            }
        }

        input = e.container.find("input[name='Qta2']").data("kendoNumericTextBox");
        if (input != undefined) {
            if (input.value() !== "" && input.value() !== undefined && input.value() !== null && e.model.dirty === true) {
                let nuovo_Qta2 = SuperficieTrattataCambiataIrrigazione(e.model);

                if (nuovo_Qta2 !== undefined && nuovo_Qta2 !== null)
                    e.model.Qta2 = nuovo_Qta2;
            }
        }

        input = e.container.find("input[name='Data_Consiglio_Custom']").data("kendoDatePicker");
        if (input != undefined) {
            if (input != undefined) {
                if (input.value() == "" || input.value() == undefined || input.value() == null)
                    e.model.Data_Consiglio_Custom = null;
            }
        }

        input = e.container.find("input[name='Qta_Acqua_Consiglio_Custom']").data("kendoNumericTextBox");
        if (input != undefined) {
            if (input.value() !== "" && input.value() !== undefined && input.value() !== null && e.model.dirty === true) {

                e.model.Dose = input.value();

                grid_impianti_Dose_CellClose(e.model);
            }
        }


        // Aggiorno Qta_Totale_Acqua_Periodo
        let dataItem = new Object();

        dataItem.Data_Inizio = e.model.Data_Inizio;
        dataItem.Data_Fine = e.model.Data_Fine;
        dataItem.Qta_Totale = e.model.Qta_Totale;
        dataItem.Frequenza = e.model.Frequenza;
        dataItem.App_Nome = e.model.App_Nome;

        //Prima controllo la validità delle date
        if (dataItem.Data_Inizio !== null && dataItem.Data_Fine !== null) {

            if (dataItem.Data_Fine >= dataItem.Data_Inizio) {
                let nuova_Qta_Totale_Acqua_Periodo = Calcola_Qta_Totale_Acqua_Periodo(dataItem);

                if (nuova_Qta_Totale_Acqua_Periodo !== undefined && nuova_Qta_Totale_Acqua_Periodo !== null) {
                    let Qta_Totale_Acqua_Periodo = nuova_Qta_Totale_Acqua_Periodo;

                    e.model.Qta_Totale_Acqua_Periodo = Qta_Totale_Acqua_Periodo;
                }
            }
            else {
                MessaggioErrore("L'Appezzamento '" + dataItem.App_Nome + "' ha una Data di Fine Irrigazione '" + kendo.toString(dataItem.Data_Fine, "dd/MM/yyyy") + "' che è antecedente alla Data di Inizio Irrigazione '" + kendo.toString(dataItem.Data_Inizio, "dd/MM/yyyy") + "'.", "DIV_Messaggi");
            }

        }
        else {
            e.model.Qta_Totale_Acqua_Periodo = 0;
        }


        grid_Impianti_Irrigazione.refresh();
    }
}

function grid_impianti_Dose_CellClose(model) {
    let nuova_Qta_Totale = DoseCambiataIrrigazione(model);

    if (nuova_Qta_Totale !== null && nuova_Qta_Totale !== undefined) {

        model.Qta_Totale = nuova_Qta_Totale;

        //aggiorno ore e portata

        model.Ore = 0;
        model.Portata = 0;
    }
}

function grid_impianti_irrigazione_beforeEdit(e) {
    //Disabilito il controllo per l'inserimento del campo se la riga non è selezionata
    if (e.model.Selected === false)
        e.preventDefault();

    let grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    grid.bind("edit", function (e) {
        //Se voglio editare la Qta_Totale_Acqua_Periodo disabilito la kendoNumericTextBox perchè è in sola lettura
        var numeric = e.container.find("input[name=Qta_Totale_Acqua_Periodo]").data("kendoNumericTextBox");
        if (numeric != undefined) {
            numeric.enable(false);
        }

        //Se ho scelto un consiglio da DSS non posso editare la Data e Dose Consiglio Custom
        let ID_DSS_Irrigazione = e.model.ID_DSS_Irrigazione;

        let abilita_valori_custom = true;

        if (ID_DSS_Irrigazione > 0) 
            abilita_valori_custom = false;
        
        let data_consiglio_date = e.container.find("input[name='Data_Consiglio_Custom']").data("kendoDatePicker");
        if (data_consiglio_date != undefined && data_consiglio_date != null) {
            data_consiglio_date.enable(abilita_valori_custom);
        }

        let qta_acqua_numeric = e.container.find("input[name=Qta_Acqua_Consiglio_Custom]").data("kendoNumericTextBox");
        if (qta_acqua_numeric != undefined && qta_acqua_numeric != null) {
            qta_acqua_numeric.enable(abilita_valori_custom);
        }

        grid.unbind("edit");
    });
}

function DoseCambiataIrrigazione(model) {

    //Aggiorno la QUANTITÀ TOTALE DI ACQUA [M3](se presente la portata)
    let Dose = model.Dose;
    let Superficie = model.Qta2;

    if ($("#ddl_Udm_Irrigazione").data("kendoDropDownList") !== undefined && $("#ddl_Udm_Irrigazione").data("kendoDropDownList") !== null) {
        let UDM = parseInt(Get_KendoDDLValue("ddl_Udm_Irrigazione"));

        if (UDM == 18) {//millimetri
            Dose = Dose * 10;
        } else if (UDM != 90) {//diverso da m3/Ha
            Dose = 0;
        }
    }


    let Qta_Totale = Dose * Superficie;

    let result = roundNumber(Qta_Totale, 4)

    if (isNaN(result) || result === Infinity)
        result = 0;

    return result;

}

function AggiornaDoseIrrigazione(riga) {
    let Dose = 0;
    if (riga !== undefined && riga !== null) {
        let Superficie = riga.Qta2;
        let QtaTot = riga.Qta_Totale;

        let DoseMcSuHa = QtaTot / Superficie;

        if ($("#ddl_Udm_Irrigazione").data("kendoDropDownList") !== undefined && $("#ddl_Udm_Irrigazione").data("kendoDropDownList") !== null) {
            let UDM = parseInt(Get_KendoDDLValue("ddl_Udm_Irrigazione"));

            if (UDM == 18) {//millimetri
                Dose = DoseMcSuHa / 10;
            } else if (UDM == 90) {//m3/Ha
                Dose = DoseMcSuHa;
            } else {//non impostato
                Dose = 0;
            }

            Dose = roundNumber(Dose, 4);
        }
    }

    let result = Dose;

    if (isNaN(result) || result === Infinity)
        result = 0;

    return result;
}

function OreCambiataIrrigazione(model) {

    //aggiorno la QtaTotale (se presente la portata)
    //let Ore = model.Ore_Number;
    let Ore = model.Ore;
    let Portata = model.Portata;
    let QtaTot = Ore * Portata / 1000;

    model.Qta_Totale = roundNumber(QtaTot, 4);

    //aggiorno la dose
    let Dose = AggiornaDoseIrrigazione(model);
    model.Dose = Dose;

    if (isNaN(model.Qta_Totale) || model.Qta_Totale === Infinity)
        model.Qta_Totale = 0;

    return model;
}

function PortataCambiataIrrigazione(model) {

    //aggiorno la QtaTotale (se presente le ore)
    let Ore = model.Ore;
    let Portata = model.Portata;
    let QtaTot = Ore * Portata / 1000;

    model.Qta_Totale = roundNumber(QtaTot, 4);

    //aggiorno la dose
    let Dose = AggiornaDoseIrrigazione(model);
    model.Dose = Dose;

    if (isNaN(model.Qta_Totale) || model.Qta_Totale === Infinity)
        model.Qta_Totale = 0;

    return model;

}

function QtaTotCambiataIrrigazione(riga) {

    //ricavo la dose
    let Dose = AggiornaDoseIrrigazione(riga);

    return Dose;

}


function SuperficieTrattataCambiataIrrigazione(model) {

    //Verifico che la superficie trattata non superi la superficie dell'impianto
    let Qta2 = model.Qta2;
    if (model.Sup_Imp < model.Qta2) {
        MessaggioErrore("La Superficie d'impiego deve essere minore o uguale alla superficie totale.", "DIV_Messaggi");
        Qta2 = model.Sup_Imp;
        return Qta2;
    }

}


function grid_impianti_irrigazione_beforeSelectAllRows(e) {

    $("#grid_Impianti_Irrigazione").attr("click_checkbox_column_header", true);


    if ($("#grid_Impianti_Irrigazione-header-chb").is(":checked") === false) {
        //Controllo se c'erano delle righe che sono state deselezionate dall'utente
        let righe_da_non_selezionare = [];

        let ds_grid = $("#grid_Impianti_Irrigazione").data("kendoGrid").dataSource.data();

        for (var x = 0; x < ds_grid.length; x++) {
            if (ds_grid[x].Selected === false) {
                righe_da_non_selezionare.push(ds_grid[x].Id_Grid_Irrigazione);
            }
        }

        Dialog_Deseleziona_grid_Impianti_Irrigazione(null, null, true, righe_da_non_selezionare);


    } else if ($("#grid_Impianti_Irrigazione-header-chb").is(":checked") === true) {

        Inserisci_ValoreSuperficieCoinvolta_ImpiantoIrrigazione(true, null);

        $("#grid_Impianti_Irrigazione").data("kendoGrid").refresh();

    }

    Abilita_Disabilita_Resto();

}

function grid_impianti_irrigazione_afterSelectAllRows(e) {
    //Aggiorno la textbox della Superficie Selezionata con tutte le righe selezionate
    if (($("#grid_Impianti_Irrigazione").attr("click_checkbox_column_header") === "true" ||
        $("#grid_Impianti_Irrigazione").attr("click_checkbox_column_header") === true) &&
        $("#grid_Impianti_Irrigazione-header-chb").is(":checked") === true) {

        RicalcolaSuperficieTotaleIrrigazione();

    }

    $("#grid_Impianti_Irrigazione").removeAttr("click_checkbox_column_header");

    Abilita_Disabilita_Resto();
}

function kEventoSelezionaRigaGrid_Impianti_Irrigazione(e) {

    if ($("#grid_Impianti_Irrigazione").attr("click_checkbox_column_header") !== "true" &&
        $("#grid_Impianti_Irrigazione").attr("click_checkbox_column_header") !== true) {

        var checked = this.checked,
            row = $(this).parents("tr"),
            grid = $("#grid_Impianti_Irrigazione").data("kendoGrid"),
            dataItem = grid.dataItem(row);

        dataItem.Selected = checked;


        if (checked) {
            if (!row.hasClass(GIAS_K_STATE_SELECTED))
                //-select the row
                Seleziona_grid_Impianti_Irrigazione(false, row, null);

            Inserisci_ValoreSuperficieCoinvolta_ImpiantoIrrigazione(false, dataItem);

            $("#grid_Impianti_Irrigazione").data("kendoGrid").refresh();

        } else {
            if (row.hasClass(GIAS_K_STATE_SELECTED)) {
                Dialog_Deseleziona_grid_Impianti_Irrigazione(row, dataItem, false);
            }
        }

        RicalcolaSuperficieTotaleIrrigazione();

        Abilita_Disabilita_Resto();

    }

}

function Seleziona_grid_Impianti_Irrigazione(tutte_le_righe_selezionate, row, righe_da_non_selezionare) {

    if (tutte_le_righe_selezionate === undefined || tutte_le_righe_selezionate === null || tutte_le_righe_selezionate === "")
        return;

    let grid_Impianti_Irrigazione = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid_Impianti_Irrigazione === undefined || grid_Impianti_Irrigazione === null || grid_Impianti_Irrigazione === "")
        return;

    if (tutte_le_righe_selezionate === true) {

        if (righe_da_non_selezionare === undefined || righe_da_non_selezionare === null || righe_da_non_selezionare === "")
            return;



        $('.row-checkbox').each(function (idx, item) {
            let riga = $(item).closest('tr');

            let dataItem = grid.dataItem(riga);

            if ($.inArray(dataItem.Id_Grid_Irrigazione, righe_da_non_selezionare) > -1 === false) {
                if (!(riga.is('.' + GIAS_K_STATE_SELECTED))) {
                    //-select the row
                    riga.addClass(GIAS_K_STATE_SELECTED);
                }
            }

        });

        let ds_grid_Impianti_Irrigazione = grid_Impianti_Irrigazione.dataSource.data();

        for (var x = 0; x < ds_grid_Impianti_Irrigazione.length; x++) {
            if ($.inArray(ds_grid_Impianti_Irrigazione[x].Id_Grid_Irrigazione, righe_da_non_selezionare) > -1 === false) {
                ds_grid_Impianti_Irrigazione[x].Selected = true;
            }

        }
    } else if (tutte_le_righe_selezionate === false) {

        if (row === undefined || row === null || row === "")
            return;

        if (!row.is('.' + GIAS_K_STATE_SELECTED))
            row.addClass(GIAS_K_STATE_SELECTED);
    }
}

function Deseleziona_grid_Impianti_Irrigazione(tutte_le_righe_deselezionate, row) {

    if (tutte_le_righe_deselezionate === undefined || tutte_le_righe_deselezionate === null || tutte_le_righe_deselezionate === "")
        return;

    let grid_Impianti_Irrigazione = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (tutte_le_righe_deselezionate === true) {
        $('.row-checkbox').each(function (idx, item) {
            if (($(item).closest('tr').is(GIAS_K_STATE_SELECTED))) {
                //-remove selection
                $(item).closest('tr').removeClass(GIAS_K_STATE_SELECTED);
            }
        });

        let ds_grid_Impianti_Irrigazione = grid_Impianti_Irrigazione.dataSource.data();

        for (var x = 0; x < ds_grid_Impianti_Irrigazione.length; x++) {
            ds_grid_Impianti_Irrigazione[x].Selected = false;
        }
    } else if (tutte_le_righe_deselezionate === false) {

        if (row === undefined || row === null || row === "")
            return;

        if (row.is(GIAS_K_STATE_SELECTED))
            row.removeClass(GIAS_K_STATE_SELECTED);
    }
}


function Dialog_Deseleziona_grid_Impianti_Irrigazione(riga, dataItem_riga, tutte_le_righe_selezionate, righe_da_non_selezionare) {

    var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid === undefined || grid === null || grid === "")
        return;

    if (tutte_le_righe_selezionate === undefined || tutte_le_righe_selezionate === null || tutte_le_righe_selezionate === "")
        return;

    var msgText = "";
    if (tutte_le_righe_selezionate === true)
        msgText = "Si desidera davvero deselezionare tutte le righe?(Verranno resettati tutti i valori delle righe precedentemente inseriti)";
    else if (tutte_le_righe_selezionate === false)
        msgText = "Si desidera davvero deselezionare la riga?(Verranno resettati tutti i valori precedentemente inseriti)";

    //Avviso l'utente che verra resettata la riga se viene deselezionata
    let container = document.getElementById("grid_Impianti_Irrigazione");
    let id_dialog = creaNewRowDiv("id_dialog_deseleziona_grid_Impianti_Irrigazione");
    container.appendChild(id_dialog);

    $("#id_dialog_deseleziona_grid_Impianti_Irrigazione").kendoDialog({
        title: "Conferma Deselezione",
        closable: false,
        modal: {
            preventScroll: true
        },
        content: msgText,
        actions: [{
            text: 'No',
            primary: true,
            action: function (e) {

                WaitFrame.show();
                if (tutte_le_righe_selezionate === true) {

                    if (righe_da_non_selezionare === undefined || righe_da_non_selezionare === null || righe_da_non_selezionare === "") {
                        return;
                    }

                    Seleziona_grid_Impianti_Irrigazione(true, null, righe_da_non_selezionare);

                    $("#grid_Impianti_Irrigazione-header-chb").prop("checked", true);

                    $("#grid_Impianti_Irrigazione").removeAttr("click_checkbox_column_header");
                }
                else if (tutte_le_righe_selezionate === false) {

                    if ((riga === undefined || riga === null || riga === "") ||
                        (dataItem_riga === undefined || dataItem_riga === null || dataItem_riga === "")) {
                        return;
                    }


                    Seleziona_grid_Impianti_Irrigazione(false, riga);

                    dataItem_riga.Selected = true;

                    //Riseleziono la checkbox della riga
                    $("#grid_Impianti_Irrigazione_" + dataItem_riga.Id_Grid_Irrigazione).prop("checked", true);


                }


                grid.refresh();

                RicalcolaSuperficieTotaleIrrigazione();

                WaitFrame.hide();

                $("#id_dialog_deseleziona_grid_Impianti_Irrigazione").remove();
            }
        },
        {
            text: 'Sì',
            action: function (e) {

                WaitFrame.show();
                if (tutte_le_righe_selezionate === true) {
                    Deseleziona_grid_Impianti_Irrigazione(true, null);

                    Resetta_riga_Impianti_Irrigazione(null, true);

                    $("#grid_Impianti_Irrigazione").removeAttr("click_checkbox_column_header");
                }
                else if (tutte_le_righe_selezionate === false) {

                    if ((riga === undefined || riga === null || riga === "") ||
                        (dataItem_riga === undefined || dataItem_riga === null || dataItem_riga === ""))
                        return;

                    Resetta_riga_Impianti_Irrigazione(dataItem_riga, false);

                    Deseleziona_grid_Impianti_Irrigazione(false, riga);

                }

                grid.refresh();

                RicalcolaSuperficieTotaleIrrigazione();

                WaitFrame.hide();

                $("#id_dialog_deseleziona_grid_Impianti_Irrigazione").remove();
            },
        }]
    });

}

function Resetta_riga_Impianti_Irrigazione(riga, resetta_tutte_le_righe) {

    if (resetta_tutte_le_righe === false) {

        if (riga === undefined || riga === null || riga === "")
            return;

        riga.Qta2 = 0;
        riga.Dose = 0;
        riga.Ore = 0;
        riga.Portata = 0;
        riga.Qta_Totale = 0;
        riga.Data_Inizio = null;
        riga.Data_Fine = null;
        riga.Frequenza = 0;
        riga.Qta_Totale_Acqua_Periodo = 0;
        riga.ModifTipoIrriUtilizzata_Cod = -1;
        riga.ModifTipoIrriUtilizzata_Des = "-Quella Dell'impianto-";
        riga.ID_DSS_Irrigazione = Empty_obj_DSS_Irrigazione.ID_DSS_Irrigazione;
        riga.Descrizione_DSS_Irrigazione = Empty_obj_DSS_Irrigazione.Descrizione_DSS_Irrigazione;
        riga.Qta_Acqua_DSS_Irrigazione = Empty_obj_DSS_Irrigazione.Qta_Acqua_DSS_Irrigazione;
        riga.Udm_Cod_DSS_Irrigazione = Empty_obj_DSS_Irrigazione.Udm_Cod_DSS_Irrigazione;
        riga.Qta_Acqua_Consiglio_Custom = 0;
        riga.Data_Consiglio_Custom = null;
        riga.Min_Data_Turno = Empty_obj_DSS_Irrigazione.Min_Data_Turno;
        riga.Max_Data_Turno = Empty_obj_DSS_Irrigazione.Max_Data_Turno;

    } else if (resetta_tutte_le_righe === true) {

        let grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

        if (grid === undefined || grid === null || grid === "")
            return;

        let ds_grid = grid.dataSource.data();

        for (var x = 0; x < ds_grid.length; x++) {

            ds_grid[x].Qta2 = 0;
            ds_grid[x].Dose = 0;
            ds_grid[x].Ore = 0;
            ds_grid[x].Portata = 0;
            ds_grid[x].Qta_Totale = 0;
            ds_grid[x].Data_Inizio = null;
            ds_grid[x].Data_Fine = null;
            ds_grid[x].Frequenza = 0;
            ds_grid[x].Qta_Totale_Acqua_Periodo = 0;
            ds_grid[x].ModifTipoIrriUtilizzata_Cod = -1;
            ds_grid[x].ModifTipoIrriUtilizzata_Des = "-Quella Dell'impianto-";
            ds_grid[x].ID_DSS_Irrigazione = Empty_obj_DSS_Irrigazione.ID_DSS_Irrigazione;
            ds_grid[x].Descrizione_DSS_Irrigazione = Empty_obj_DSS_Irrigazione.Descrizione_DSS_Irrigazione;
            ds_grid[x].Qta_Acqua_DSS_Irrigazione = Empty_obj_DSS_Irrigazione.Qta_Acqua_DSS_Irrigazione;
            ds_grid[x].Udm_Cod_DSS_Irrigazione = Empty_obj_DSS_Irrigazione.Udm_Cod_DSS_Irrigazione;
            ds_grid[x].Qta_Acqua_Consiglio_Custom = 0;
            ds_grid[x].Data_Consiglio_Custom = null;
            ds_grid[x].Min_Data_Turno = Empty_obj_DSS_Irrigazione.Min_Data_Turno;
            ds_grid[x].Max_Data_Turno = Empty_obj_DSS_Irrigazione.Max_Data_Turno;
        }

    }

}


function Inserisci_ValoreSuperficieCoinvolta_ImpiantoIrrigazione(inserisci_in_tutte_le_righe, riga) {
    //Imposto il valore della Superficie trattata uguale alla Superficie totale

    let grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid === undefined || grid === null || grid === "")
        return;

    if (inserisci_in_tutte_le_righe === undefined || inserisci_in_tutte_le_righe === null || inserisci_in_tutte_le_righe === "")
        return;

    if (inserisci_in_tutte_le_righe === true) {


        let ds_grid = grid.dataSource.data();

        for (var x = 0; x < ds_grid.length; x++) {
            if (ds_grid[x].Qta2 === 0 && ds_grid[x].Sup_Imp !== 0)
                ds_grid[x].Qta2 = ds_grid[x].Sup_Imp;
        }

    }
    else if (inserisci_in_tutte_le_righe === false) {
        if (riga.Qta2 === 0 && riga.Sup_Imp !== 0)
            riga.Qta2 = riga.Sup_Imp;
    }

}


//Funzione per l'aggiornamento della superficie Selezionata
function RicalcolaSuperficieTotaleIrrigazione() {

    let SupTot = 0.00;

    let grid_impiantiIrrigazione = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid_impiantiIrrigazione !== undefined && grid_impiantiIrrigazione !== null && grid_impiantiIrrigazione !== "") {

        let ds_grid_impiantiIrrigazione = grid_impiantiIrrigazione.dataSource.data();

        for (var x = 0; x < ds_grid_impiantiIrrigazione.length; x++) {
            if (ds_grid_impiantiIrrigazione[x].Selected === true) {
                SupTot += parseFloat(ds_grid_impiantiIrrigazione[x].Sup_Imp);
            }
        }

        var app = kendo.toString(SupTot, "n4");

        $("#Txt_SupSelezionataIrrigazione").val(app);
    }
}


function ModificaTipoIrrigazioneUtilizzata_DropdownEditor(container) {

    creaDropDownEditor(container, "ModifTipoIrriUtilizzata_Des", "ModifTipoIrriUtilizzata_Cod", { transport: { read: Leggi_TipoIrrigazioneKendoGridImpianti } }, ModificaTipoIrrigazioneUtilizzata_change);
}

function ModificaTipoIrrigazioneUtilizzata_change(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.ModifTipoIrriUtilizzata_Cod = dataItem.ModifTipoIrriUtilizzata_Cod;
    model.ModifTipoIrriUtilizzata_Des = dataItem.ModifTipoIrriUtilizzata_Des;
}

function filtraColonne_Impianti_Irrigazione() {
    //Se non è ancora stata generata creo le checkbox relative alle colonne e le seleziono,
    //se quella colonna è visibile.

    $('#dialogImpostazioniColonneGrid_Impianti_Irrigazione').modal('show');

    if ($("#elenco_colonneGrid_Impianti_Irrigazione").find("div").length === 0) {

        if ($(ElencoColonneKendoGrid_Impianti_Irrigazione).val() === undefined ||
            $(ElencoColonneKendoGrid_Impianti_Irrigazione).val() === "" ||
            $(ElencoColonneKendoGrid_Impianti_Irrigazione).val() === null)
            return;

        let ElencoColonneGrid = JSON.parse($(ElencoColonneKendoGrid_Impianti_Irrigazione).val());



        for (var x = 0; x < ElencoColonneGrid.length; x++) {
            let content_dialog = "";
            content_dialog += "<div class='row'>";
            content_dialog += "<div class='col-lg-6 col-md-6 col-sm-12' style='padding-top: 10px'>";

            if (ElencoColonneGrid[x].Mostra === true || ElencoColonneGrid[x].Mostra === "True")
                content_dialog += "<input id='chk" + ElencoColonneGrid[x].id + "' type='checkbox' class='k-checkbox' checked>";
            else if (ElencoColonneGrid[x].Mostra === false || ElencoColonneGrid[x].Mostra === "False")
                content_dialog += "<input id='chk" + ElencoColonneGrid[x].id + "' type='checkbox' class='k-checkbox' >";

            content_dialog += "<label class='k-checkbox-label' for='chk" + ElencoColonneGrid[x].id + "'>" + ElencoColonneGrid[x].text + "</label>";
            content_dialog += "</div>";
            content_dialog += "</div>";
            $("#elenco_colonneGrid_Impianti_Irrigazione").html($("#elenco_colonneGrid_Impianti_Irrigazione").html() + content_dialog);
        }


    }

}

function impostaValori_Impianti_Irrigazione() {

    var udm = 0;

    if ($("#ddl_Udm_Irrigazione").data("kendoDropDownList") === undefined ||
        $("#ddl_Udm_Irrigazione").data("kendoDropDownList") === null ||
        $("#ddl_Udm_Irrigazione").data("kendoDropDownList") === "")
        return;

    udm = parseInt(Get_KendoDDLValue("ddl_Udm_Irrigazione"));

    $('#dialogImpostaValoriGrid_Impianti_Irrigazione').modal('show');


    let content_dialog = "";

    //################### SWITCH SOVRASCRIVI VALORI INSERITI ###################//
    content_dialog += "<div class='row'>";
    content_dialog += "<div class='col-lg-6 col-md-6 col-sm-12'>";
    content_dialog += "<div class='input-group'>";
    content_dialog += "<label class='lbl_required' id='lbl_sovrascrivivalIrrigazione' for='switch_sovrascrivivalIrrigazione'>Sovrascrivi valori inseriti:</label>";
    content_dialog += "<input type='checkbox' id='switch_sovrascrivivalIrrigazione' class='kendoSwitch'/>";
    content_dialog += "<div id='Mess_No_sovrascrivivalIrrigazione'>";
    content_dialog += "</div>";
    content_dialog += "</div>";
    content_dialog += "</div>";
    content_dialog += "</div>";
    //#########################################################//


    //################### TEXTBOX DOSE ACQUA ###################//
    if (GiasVersioneMaster === "2022") {
        content_dialog += "<div id='div_dose' class='row' style='padding-top: 15px;'>";
            content_dialog += "<input type='radio' name='valore_da_inserireIrrigazione' id='valore_da_inserireIrrigazioneDose' class='k-radio'>" ;
            content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
                content_dialog += "<div class='input-group'>";
        content_dialog += "<label id='lblDoseIrrigazione' for ='Txt_DoseIrrigazione' class='input-group-addon DoseAcquaIrrigazione'>Dose Acqua [" + $("#ddl_Udm_Irrigazione").data("kendoDropDownList").text() + "]";
                    content_dialog += "</label>";
                    content_dialog += "<input type='number' class='form-control' id='Txt_DoseIrrigazione' min='0'/>";
                content_dialog += "</div>";
            content_dialog += "</div>";
        content_dialog += "</div>";
    } else {
        content_dialog += "<div id='div_dose' class='row' style='padding-top: 15px;'>";
        content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
        content_dialog += "<div class='input-group'>";
        content_dialog += "<label id='lblDoseIrrigazione' for ='Txt_DoseIrrigazione' class='input-group-addon DoseAcquaIrrigazione'>";
        content_dialog += "<input type='radio' name='valore_da_inserireIrrigazione' id='valore_da_inserireIrrigazioneDose' class='k-radio'> Dose Acqua [" + $("#ddl_Udm_Irrigazione").data("kendoDropDownList").text() + "]";
        content_dialog += "</label>";
        content_dialog += "<input type='number' class='form-control' id='Txt_DoseIrrigazione' min='0'/>";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";
    }
    //#########################################################//

    //################### TEXTBOX ORE E TEXTBOX PORTATA ###################//
    if (GiasVersioneMaster === "2022") {
        content_dialog += "<div id='div_ore_portata'  style='padding-top: 10px;'>";
        content_dialog += "<input type='radio' name='valore_da_inserireIrrigazione' id='valore_da_inserireIrrigazioneOrePortata' class='k-radio'>";
        content_dialog += "<div class='row'>";
        content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
        content_dialog += "<div class='input-group'>";
        content_dialog += "<label id='lblOreIrrigazione' class='input-group-addon OrePortataIrrigazione'>Ore di Irrigazione [H]</label>";
        content_dialog += "<input type='number' class='form-control' id='Txt_OreIrrigazione' min='0'/>";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "<div class='row'>";
        content_dialog += "</div>";
        content_dialog += "<div class='row'>";
        content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
        content_dialog += "<div class='input-group'>";
        content_dialog += "<label id='lblPortataIrrigazione' class='input-group-addon OrePortataIrrigazione'>Portata [l/H]</label>";
        content_dialog += "<input type='number' class='form-control' id='Txt_PortataIrrigazione' min='0' />";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";
    } else {
        content_dialog += "<div id='div_ore_portata'  style='padding-top: 10px;'>";
        content_dialog += "<div class='row'>";
        content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
        content_dialog += "<div class='input-group'>";
        content_dialog += "<label id='lblOreIrrigazione' class='input-group-addon OrePortataIrrigazione'>Ore di Irrigazione [H]</label>";
        content_dialog += "<input type='number' class='form-control' id='Txt_OreIrrigazione' min='0'/>";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "<div class='row'>";
        content_dialog += "<input type='radio' name='valore_da_inserireIrrigazione' id='valore_da_inserireIrrigazioneOrePortata' class='k-radio'>";
        content_dialog += "</div>";
        content_dialog += "<div class='row'>";
        content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
        content_dialog += "<div class='input-group'>";
        content_dialog += "<label id='lblPortataIrrigazione' class='input-group-addon OrePortataIrrigazione'>Portata [l/H]</label>";
        content_dialog += "<input type='number' class='form-control' id='Txt_PortataIrrigazione' min='0' />";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";
    }
    //#########################################################//

    //################### TEXTBOX QUANTITA TOTALE DI ACQUA ###################//
    if (GiasVersioneMaster === "2022") {
        content_dialog += "<div class='row' id='totaleAcqua' style='padding-top: 10px;'>";
        content_dialog += "<input type='radio' name='valore_da_inserireIrrigazione' id='valore_da_inserireIrrigazioneQta_Totale' class='k-radio'>";
        content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
        content_dialog += "<div class='input-group'>";
        content_dialog += "<label id='lblQta_TotaleIrrigazione' for='Txt_Qta_TotaleIrrigazione' class='input-group-addon QtaTotaleAcquaIrrigazione'> Quantità Totale di Acqua [M3]";
        content_dialog += "</label>";
        content_dialog += "<input type='number' class='form-control' id='Txt_Qta_TotaleIrrigazione' min='0' />";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";
    } else {
        content_dialog += "<div class='row' style='padding-top: 10px;'>";
        content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
        content_dialog += "<div class='input-group'>";
        content_dialog += "<label id='lblQta_TotaleIrrigazione' for='Txt_Qta_TotaleIrrigazione' class='input-group-addon QtaTotaleAcquaIrrigazione'>";
        content_dialog += "<input type='radio' name='valore_da_inserireIrrigazione' id='valore_da_inserireIrrigazioneQta_Totale' class='k-radio'> Quantità Totale di Acqua [M3]";
        content_dialog += "</label>";
        content_dialog += "<input type='number' class='form-control' id='Txt_Qta_TotaleIrrigazione' min='0' />";
        content_dialog += "</div>";
        content_dialog += "</div>";
        content_dialog += "</div>";

    }
    //#########################################################//

    //################### DATA INIZIO E FINE IRRIGAZIONE ###################//
    content_dialog += "<div class='row' style='padding-top: 10px;'>";
    if (GiasVersioneMaster !== "2022") content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
    if (GiasVersioneMaster === "2022") content_dialog += "<div class='col-lg-6 col-md-6 col-sm-12'>";
    content_dialog += "<div class='input-group'>";
    content_dialog += "<label class='input-group-addon alert-info' for='Data_Inizio_Irrigazione'>Data Inizio Irrigazione</label>";
    content_dialog += "<input id='Data_Inizio_Irrigazione' title='Data Inizio Irrigazione' class='form-control' />";
    content_dialog += "</div>";
    content_dialog += "</div>";
    if (GiasVersioneMaster !== "2022") content_dialog += "</div>";

    if (GiasVersioneMaster !== "2022") content_dialog += "<div class='row' style='padding-top: 10px;'>";
    if (GiasVersioneMaster !== "2022") content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
    if (GiasVersioneMaster === "2022") content_dialog += "<div class='col-lg-6 col-md-6 col-sm-12'>";
    content_dialog += "<div class='input-group'>";
    content_dialog += "<label class='input-group-addon alert-info' for='Data_Fine_Irrigazione'>Data Fine Irrigazione</label>";
    content_dialog += "<input id='Data_Fine_Irrigazione' title='Data Fine Irrigazione' class='form-control' />";
    content_dialog += "</div>";
    content_dialog += "</div>";
    if (GiasVersioneMaster !== "2022") content_dialog += "</div>";
    //#########################################################//


    //################### TEXTBOX FREQUENZA ###################//
    if (GiasVersioneMaster !== "2022") content_dialog += "<div class='row' style='padding-top: 10px;'>";
    if (GiasVersioneMaster !== "2022") content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
    if (GiasVersioneMaster === "2022") content_dialog += "<div class='col-lg-12 col-md-12 col-sm-12'>";
    content_dialog += "<div class='input-group'>";
    content_dialog += "<label id='lblFrequenzaIrrigazione' class='input-group-addon alert-info'>Frequenza Irrigazione Media [GG]</label>";
    content_dialog += "<input type='number' class='form-control' id='Txt_FrequenzaIrrigazione' min='0' />";
    content_dialog += "</div>";
    content_dialog += "</div>";
    if (GiasVersioneMaster !== "2022") content_dialog += "</div>";
    //#########################################################//

    //################### DROPDOWN IRRIGAZIONE UTILIZZATA ###################//
    if (GiasVersioneMaster !== "2022") content_dialog += "<div class='row' style='padding-top: 10px;'>";
    if (GiasVersioneMaster !== "2022") content_dialog += "<div class='col-lg-9 col-md-9 col-sm-12'>";
    if (GiasVersioneMaster === "2022") content_dialog += "<div class='col-lg-12 col-md-12 col-sm-12'>";
    content_dialog += "<div class='form-horizontal'>";
    content_dialog += "<div class='form-group'>";
    content_dialog += "<div class='input-group'>";
    content_dialog += "<label class='input-group-addon alert-info' for='ddl_CambiaModifTipoIrriUtilizzata'>Irrigazione Utilizzata</label>";
    content_dialog += "<input id='ddl_CambiaModifTipoIrriUtilizzata' title='Irrigazione Utilizzata' class='form-control' />";
    content_dialog += "</div>";
    content_dialog += "</div>";
    content_dialog += "</div>";
    content_dialog += "</div>";
    content_dialog += "</div>";
    //#########################################################//

    $("#controlliGrid_Impianti_Irrigazione").html($("#controlliGrid_Impianti_Irrigazione").html() + content_dialog);

    //Reimpoisto i valori che era stati impostati precedentemente prima di chiudere la boostrap dialog
    creaKendoSwitch($("#switch_sovrascrivivalIrrigazione").data("kendoSwitch"), undefined, undefined, false, switch_sovrascrivivalIrrigazione_change, undefined, undefined);

    let switch_value = true;

    if ($("#controlliGrid_Impianti_Irrigazione").attr("switch_value") !== undefined) {
        switch_value = ($("#controlliGrid_Impianti_Irrigazione").attr("switch_value") === 'true');
    }


    $("#switch_sovrascrivivalIrrigazione").data("kendoSwitch").value(switch_value);
    Messaggio_switch_sovrascrivivalIrrigazione(switch_value);

    $("#Data_Inizio_Irrigazione").kendoDatePicker();

    if ($("#controlliGrid_Impianti_Irrigazione").attr("data_inizio_value") !== undefined) {
        $("#Data_Inizio_Irrigazione").data("kendoDatePicker").value(new Date($("#controlliGrid_Impianti_Irrigazione").attr("data_inizio_value")));
    }

    $("#Data_Fine_Irrigazione").kendoDatePicker();

    if ($("#controlliGrid_Impianti_Irrigazione").attr("data_fine_value") !== undefined) {
        $("#Data_Fine_Irrigazione").data("kendoDatePicker").value(new Date($("#controlliGrid_Impianti_Irrigazione").attr("data_fine_value")));
    }


    $("#Txt_DoseIrrigazione").kendoNumericTextBox({ format: "n2", decimals: 2 });

    if ($("#controlliGrid_Impianti_Irrigazione").attr("dose_value") !== undefined) {
        $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").value($("#controlliGrid_Impianti_Irrigazione").attr("dose_value"));
    }

    $("#Txt_OreIrrigazione").kendoNumericTextBox({ format: "n2", decimals: 2 });

    if ($("#controlliGrid_Impianti_Irrigazione").attr("ore_value") !== undefined) {
        $("#Txt_OreIrrigazione").data("kendoNumericTextBox").value($("#controlliGrid_Impianti_Irrigazione").attr("ore_value"));
    }

    $("#Txt_PortataIrrigazione").kendoNumericTextBox({ format: "0", decimals: 0 });

    if ($("#controlliGrid_Impianti_Irrigazione").attr("portata_value") !== undefined) {
        $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").value($("#controlliGrid_Impianti_Irrigazione").attr("portata_value"));
    }

    $("#Txt_Qta_TotaleIrrigazione").kendoNumericTextBox({ format: "n4", decimals: 4 });

    if ($("#controlliGrid_Impianti_Irrigazione").attr("qta_totale_value") !== undefined) {
        $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").value($("#controlliGrid_Impianti_Irrigazione").attr("qta_totale_value"));
    }

    $("#Txt_FrequenzaIrrigazione").kendoNumericTextBox({ format: "0", decimals: 0 });

    if ($("#controlliGrid_Impianti_Irrigazione").attr("frequenza_value") !== undefined) {
        $("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox").value($("#controlliGrid_Impianti_Irrigazione").attr("frequenza_value"));
    }

    creaKendoDropDownList("ddl_CambiaModifTipoIrriUtilizzata", { read: Leggi_CambiaValoreTipoIrrigazioneKendoGridImpianti }, "ModifTipoIrriUtilizzata_Des", "ModifTipoIrriUtilizzata_Cod");

    $("#ddl_CambiaModifTipoIrriUtilizzata").data("kendoDropDownList").bind("open", Openddl_CambiaModifTipoIrriUtilizzata);

    if ($("#controlliGrid_Impianti_Irrigazione").attr("ModifTipoIrriUtilizzata") !== undefined) {
        $("#ddl_CambiaModifTipoIrriUtilizzata").data("kendoDropDownList").value(parseInt($("#controlliGrid_Impianti_Irrigazione").attr("ModifTipoIrriUtilizzata")));
    }

    if ($("#controlliGrid_Impianti_Irrigazione").attr("valore_da_inserireIrrigazione_value") !== undefined) {
        let radiobuttonscelto = $("#controlliGrid_Impianti_Irrigazione").attr("valore_da_inserireIrrigazione_value");

        if (radiobuttonscelto === "valore_da_inserireIrrigazioneDose" ||
            radiobuttonscelto === "valore_da_inserireIrrigazioneOrePortata" ||
            radiobuttonscelto === "valore_da_inserireIrrigazioneQta_Totale") {
            $("#" + radiobuttonscelto + "").prop("checked", true);
        } else {
            if (udm == 18) {//millimetri

                //Default selezionato Dose
                $("#valore_da_inserireIrrigazioneDose").prop("checked", true);

            } else if (udm == 90) {//m3/Ha
                //Default selezionato Ore Portata
                $("#valore_da_inserireIrrigazioneOrePortata").prop("checked", true);
            } else {//non impostato

            }
        }

    } else {

        if (udm == 18) {//millimetri
            //Default selezionato Dose
            $("#valore_da_inserireIrrigazioneDose").prop("checked", true);
        } else if (udm == 90) {//m3/Ha
            //Default selezionato Ore Portata
            $("#valore_da_inserireIrrigazioneOrePortata").prop("checked", true);
        } else {//non impostato

        }
    }


    //Disabilito / Abilito le textbox in baso al radio button selezionato

    if ($("#valore_da_inserireIrrigazioneDose").is(":checked")) {

        Abilita_Disabilita_Controlli_RadioButton("valore_da_inserireIrrigazioneDose");
    }

    if ($("#valore_da_inserireIrrigazioneOrePortata").is(":checked")) {

        Abilita_Disabilita_Controlli_RadioButton("valore_da_inserireIrrigazioneOrePortata");

    }

    if ($("#valore_da_inserireIrrigazioneQta_Totale").is(":checked")) {

        Abilita_Disabilita_Controlli_RadioButton("valore_da_inserireIrrigazioneQta_Totale");

    }


    let minHeight = 640;
    let minWidth = 640;

    $("#div_ore_portata").show();
    $("#div_dose").show();


    //Radio button che vengono mostrati quando si clicca sul bottone 'Imposta Valori in tutte le righe'
    $("input[name='valore_da_inserireIrrigazione']").bind("change", impostaValori_Impianti_Irrigazione_RadioButton_change);



    //Imposto la dialog resizable e draggable
    $('#dialogImpostaValoriGrid_Impianti_Irrigazione_content').resizable({
        minHeight: minHeight,
        minWidth: minWidth
    });
    $('#dialogImpostaValoriGrid_Impianti_Irrigazione_dialog').draggable();

    $('#dialogImpostaValoriGrid_Impianti_Irrigazione').on('show.bs.modal', function () {
        $(this).find('.modal-body').css({
            'max-height': '100%'
        });
    });


}

function Openddl_CambiaModifTipoIrriUtilizzata(e) {
    $(document).off('focusin.bs.modal');
}


function impostaValori_Impianti_Irrigazione_RadioButton_change(e) {

    if ($(this).is(":checked")) {

        Abilita_Disabilita_Controlli_RadioButton($(this)[0].id);

    }

}

function Abilita_Disabilita_Controlli_RadioButton(id) {

    if (id === 'valore_da_inserireIrrigazioneDose') {

        $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").enable(true);

        $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").enable(false);

        $("#Txt_OreIrrigazione").data("kendoNumericTextBox").enable(false);

        $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").enable(false);

        $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").value(null);

        $("#Txt_OreIrrigazione").data("kendoNumericTextBox").value(null);

        $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").value(null);
    }

    if (id === 'valore_da_inserireIrrigazioneOrePortata') {

        $("#Txt_OreIrrigazione").data("kendoNumericTextBox").enable(true);

        $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").enable(true);

        $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").enable(false);

        $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").enable(false);

        $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").value(null);

        $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").value(null);

    }

    if (id === 'valore_da_inserireIrrigazioneQta_Totale') {

        $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").enable(true);

        $("#Txt_OreIrrigazione").data("kendoNumericTextBox").enable(false);

        $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").enable(false);

        $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").enable(false);

        $("#Txt_OreIrrigazione").data("kendoNumericTextBox").value(null);

        $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").value(null);

        $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").value(null);

    }

}

function switch_sovrascrivivalIrrigazione_change(e) {
    let switch_value = $("#switch_sovrascrivivalIrrigazione").data("kendoSwitch").value();

    Messaggio_switch_sovrascrivivalIrrigazione(switch_value);
}


function Messaggio_switch_sovrascrivivalIrrigazione(switch_value) {
    //Se lo switch è stato impostato su No mostro il alrimenti lo nascondo
    if (switch_value === false) {
        $("#Mess_No_sovrascrivivalIrrigazione").html("<strong>Le righe con valori già inseriti non verranno modificate.</strong>");
    }
    else {
        $("#Mess_No_sovrascrivivalIrrigazione").html("");
        $("#Mess_No_sovrascrivivalIrrigazione").empty();
    }

}


$('#dialogImpostaValoriGrid_Impianti_Irrigazione').on('hidden.bs.modal', function () {
    //Distruggo il contenuto del modal
    if ($("#controlliGrid_Impianti_Irrigazione").find("div").length > 0) {

        let switch_value = null;
        let data_inizio_value = null;
        let data_fine_value = null;
        let dose_value = null;
        let ore_value = null;
        let portata_value = null;
        let qta_totale_value = null;
        let frequenza_value = null;
        let ModifTipoIrriUtilizzata = null;

        //Memorizzo i valori precedentemente inseriti prima di cancellare tutti i div
        if ($("#switch_sovrascrivivalIrrigazione").data("kendoSwitch") !== undefined &&
            $("#switch_sovrascrivivalIrrigazione").data("kendoSwitch") !== null) {

            switch_value = $("#switch_sovrascrivivalIrrigazione").data("kendoSwitch").value();

            $("#controlliGrid_Impianti_Irrigazione").attr("switch_value", switch_value);
        }


        if ($("#Data_Inizio_Irrigazione").data("kendoDatePicker") !== undefined &&
            $("#Data_Inizio_Irrigazione").data("kendoDatePicker") !== null) {

            data_inizio_value = $("#Data_Inizio_Irrigazione").data("kendoDatePicker").value();

            $("#controlliGrid_Impianti_Irrigazione").attr("data_inizio_value", data_inizio_value);
        }


        if ($("#Data_Fine_Irrigazione").data("kendoDatePicker") !== undefined &&
            $("#Data_Fine_Irrigazione").data("kendoDatePicker") !== null) {

            data_fine_value = $("#Data_Fine_Irrigazione").data("kendoDatePicker").value();

            $("#controlliGrid_Impianti_Irrigazione").attr("data_fine_value", data_fine_value);
        }


        if ($("#Txt_DoseIrrigazione").data("kendoNumericTextBox") !== undefined &&
            $("#Txt_DoseIrrigazione").data("kendoNumericTextBox") !== null) {

            dose_value = $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").value();

            $("#controlliGrid_Impianti_Irrigazione").attr("dose_value", kendo.toString(dose_value, "n2"));
        }



        if ($("#Txt_OreIrrigazione").data("kendoNumericTextBox") !== undefined &&
            $("#Txt_OreIrrigazione").data("kendoNumericTextBox") !== null) {

            ore_value = $("#Txt_OreIrrigazione").data("kendoNumericTextBox").value();

            $("#controlliGrid_Impianti_Irrigazione").attr("ore_value", kendo.toString(ore_value, "n2"));
        }


        if ($("#Txt_PortataIrrigazione").data("kendoNumericTextBox") !== undefined &&
            $("#Txt_PortataIrrigazione").data("kendoNumericTextBox") !== null) {

            portata_value = $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").value();

            $("#controlliGrid_Impianti_Irrigazione").attr("portata_value", kendo.toString(portata_value, "0"));
        }


        if ($("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox") !== undefined &&
            $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox") !== null) {

            qta_totale_value = $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").value();

            $("#controlliGrid_Impianti_Irrigazione").attr("qta_totale_value", kendo.toString(qta_totale_value, "n4"));
        }


        if ($("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox") !== undefined &&
            $("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox") !== null) {

            frequenza_value = $("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox").value();

            $("#controlliGrid_Impianti_Irrigazione").attr("frequenza_value", kendo.toString(frequenza_value, "0"));
        }


        if ($("#ddl_CambiaModifTipoIrriUtilizzata").data("kendoDropDownList") !== undefined &&
            $("#ddl_CambiaModifTipoIrriUtilizzata").data("kendoDropDownList") !== null) {

            ModifTipoIrriUtilizzata = Get_KendoDDLValue("ddl_CambiaModifTipoIrriUtilizzata");

            $("#controlliGrid_Impianti_Irrigazione").attr("ModifTipoIrriUtilizzata", ModifTipoIrriUtilizzata);
        }


        //Memmorizzo il Radio Button selezionato
        if ($("#valore_da_inserireIrrigazioneOrePortata").is(":checked")) {
            $("#controlliGrid_Impianti_Irrigazione").attr("valore_da_inserireIrrigazione_value", "valore_da_inserireIrrigazioneOrePortata");
        }

        if ($("#valore_da_inserireIrrigazioneDose").is(":checked")) {
            $("#controlliGrid_Impianti_Irrigazione").attr("valore_da_inserireIrrigazione_value", "valore_da_inserireIrrigazioneDose");
        }

        if ($("#valore_da_inserireIrrigazioneQta_Totale").is(":checked")) {
            $("#controlliGrid_Impianti_Irrigazione").attr("valore_da_inserireIrrigazione_value", "valore_da_inserireIrrigazioneQta_Totale");
        }

        //Distuggo tutti i controlli kendo e rimuovo l'html
        $("#switch_sovrascrivivalIrrigazione").data("kendoSwitch").destroy();
        $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").destroy();
        $("#Txt_OreIrrigazione").data("kendoNumericTextBox").destroy();
        $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").destroy();
        $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").destroy();
        $("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox").destroy();
        $("#ddl_CambiaModifTipoIrriUtilizzata").data("kendoDropDownList").destroy();
        $("input[name='valore_da_inserireIrrigazione']").unbind("change");

        $("#controlliGrid_Impianti_Irrigazione").html("");
        $("#controlliGrid_Impianti_Irrigazione").empty();
    }
});

function CopiaValoriGrid_Impianti_Irrigazione() {

    WaitFrame.show();

    let grid_impianti_irrigazione = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid_impianti_irrigazione !== undefined && grid_impianti_irrigazione !== null) {

        let ds_grid_impianti_irrigazione = grid_impianti_irrigazione.dataSource.data();

        if (ds_grid_impianti_irrigazione.length > 0) {

            let dose = "";
            let ore = "";
            let portata = "";
            let qta_totale = "";
            let data_inizio_irrig = "";
            let data_fine_irrig = "";
            let frequenza = "";
            let ModifTipoIrriUtilizzata_Cod = "";
            let ModifTipoIrriUtilizzata_Des = "";

            if ($("#Txt_DoseIrrigazione").data("kendoNumericTextBox").value() !== "" &&
                $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").value() !== null &&
                $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").value() >= 0)
                dose = $("#Txt_DoseIrrigazione").data("kendoNumericTextBox").value();


            if ($("#Txt_OreIrrigazione").data("kendoNumericTextBox").value() !== "" &&
                $("#Txt_OreIrrigazione").data("kendoNumericTextBox").value() >= 0) {

                if ($("#Txt_OreIrrigazione").data("kendoNumericTextBox").value() !== null) {
                    ore = $("#Txt_OreIrrigazione").data("kendoNumericTextBox").value();
                }
            }

            if ($("#Txt_PortataIrrigazione").data("kendoNumericTextBox").value() !== "" &&
                $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").value() !== null &&
                $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").value() >= 0)
                portata = $("#Txt_PortataIrrigazione").data("kendoNumericTextBox").value();

            if ($("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").value() !== "" &&
                $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").value() !== null &&
                $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").value() >= 0)
                qta_totale = $("#Txt_Qta_TotaleIrrigazione").data("kendoNumericTextBox").value();

            if ($("#Data_Inizio_Irrigazione").data("kendoDatePicker").value() !== "" &&
                $("#Data_Inizio_Irrigazione").data("kendoDatePicker").value() !== null)
                data_inizio_irrig = $("#Data_Inizio_Irrigazione").data("kendoDatePicker").value();

            if ($("#Data_Fine_Irrigazione").data("kendoDatePicker").value() !== "" &&
                $("#Data_Fine_Irrigazione").data("kendoDatePicker").value() !== null)
                data_fine_irrig = $("#Data_Fine_Irrigazione").data("kendoDatePicker").value();


            if ($("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox").value() !== "" &&
                $("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox").value() !== null &&
                $("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox").value() >= 0)
                frequenza = $("#Txt_FrequenzaIrrigazione").data("kendoNumericTextBox").value();

            if ($("#ddl_CambiaModifTipoIrriUtilizzata").data("kendoDropDownList") !== undefined &&
                $("#ddl_CambiaModifTipoIrriUtilizzata").data("kendoDropDownList") !== null) {

                let tipo_irrig_utilizzata = parseInt(Get_KendoDDLValue("ddl_CambiaModifTipoIrriUtilizzata"));

                if (tipo_irrig_utilizzata !== -2) {
                    ModifTipoIrriUtilizzata_Cod = parseInt(Get_KendoDDLValue("ddl_CambiaModifTipoIrriUtilizzata"));
                    ModifTipoIrriUtilizzata_Des = $("#ddl_CambiaModifTipoIrriUtilizzata").data("kendoDropDownList").text();
                }

            }

            //Controllo le date impostate 
            if (data_inizio_irrig !== "" && data_fine_irrig !== "") {
                if (data_inizio_irrig > data_fine_irrig) {
                    MessaggioErrore("La Data di Fine Irrigazione è antecedente alla Data di Inizio Irrigazione.", "DIV_Messaggi");
                    WaitFrame.hide();
                    return;
                }
            }


            var sovrascrivi_val_inseriti = $("#switch_sovrascrivivalIrrigazione").data("kendoSwitch").value();

            for (var x = 0; x < ds_grid_impianti_irrigazione.length; x++) {

                //Cambio i campi solo delle righe selezionate
                if (ds_grid_impianti_irrigazione[x].Selected === true) {

                    let modifica_campo = true;

                    //Se lo switch è impostato su No non tocco nessun valore della riga, se almeno un valore di essa è diverso da 0 
                    if (sovrascrivi_val_inseriti === false) {
                        for (var y = 0; y < Object.keys(ds_grid_impianti_irrigazione[x]).length; y++) {
                            if ((Object.keys(ds_grid_impianti_irrigazione[x])[y] === "Dose" ||
                                Object.keys(ds_grid_impianti_irrigazione[x])[y] === "Ore" ||
                                Object.keys(ds_grid_impianti_irrigazione[x])[y] === "Portata" ||
                                Object.keys(ds_grid_impianti_irrigazione[x])[y] === "Frequenza" ||
                                Object.keys(ds_grid_impianti_irrigazione[x])[y] === "Qta_Totale") &&
                                Object.values(ds_grid_impianti_irrigazione[x])[y] > 0) {

                                modifica_campo = false;
                                break;
                            }

                            if (Object.keys(ds_grid_impianti_irrigazione[x])[y] === "ModifTipoIrriUtilizzata_Cod" &&
                                Object.values(ds_grid_impianti_irrigazione[x])[y] !== -1) {

                                modifica_campo = false;
                                break;
                            }
                        }
                    }


                    let dataItem = new Object();

                    if (data_inizio_irrig !== "")
                        dataItem.Data_Inizio = data_inizio_irrig;
                    else
                        dataItem.Data_Inizio = ds_grid_impianti_irrigazione[x].Data_Inizio;

                    if (data_fine_irrig !== "")
                        dataItem.Data_Fine = data_fine_irrig;
                    else
                        dataItem.Data_Fine = ds_grid_impianti_irrigazione[x].Data_Fine;

                    if (frequenza !== "")
                        dataItem.Frequenza = frequenza;
                    else
                        dataItem.Frequenza = ds_grid_impianti_irrigazione[x].Frequenza;

                    if (qta_totale !== "")
                        dataItem.Qta_Totale = qta_totale;
                    else
                        dataItem.Qta_Totale = ds_grid_impianti_irrigazione[x].Qta_Totale;

                    if (ore !== "")
                        dataItem.Ore = ore;
                    else
                        dataItem.Ore = ds_grid_impianti_irrigazione[x].Ore;



                    if (portata !== "")
                        dataItem.Portata = portata;
                    else
                        dataItem.Portata = ds_grid_impianti_irrigazione[x].Portata;

                    if (dose !== "")
                        dataItem.Dose = dose;
                    else
                        dataItem.Dose = ds_grid_impianti_irrigazione[x].Dose;

                    dataItem.App_Nome = ds_grid_impianti_irrigazione[x].App_Nome;
                    dataItem.Qta2 = ds_grid_impianti_irrigazione[x].Qta2;



                    if (ore !== "" && modifica_campo === true) {
                        let nuovo_dataItem = OreCambiataIrrigazione(dataItem);

                        if (nuovo_dataItem !== undefined && nuovo_dataItem !== null &&
                            nuovo_dataItem.Qta_Totale !== undefined && nuovo_dataItem.Qta_Totale !== null &&
                            nuovo_dataItem.Dose !== undefined && nuovo_dataItem.Dose !== null) {


                            ds_grid_impianti_irrigazione[x].Ore = ore;

                            dataItem.Ore = ore;

                            ds_grid_impianti_irrigazione[x].Qta_Totale = nuovo_dataItem.Qta_Totale;

                            dataItem.Qta_Totale = nuovo_dataItem.Qta_Totale;

                            ds_grid_impianti_irrigazione[x].Dose = nuovo_dataItem.Dose;

                            dataItem.Dose = nuovo_dataItem.Dose;
                        }
                    }


                    //Se viene inserita la portata allora richiamo la funzione PortataCambiataIrrigazione
                    if (portata !== "" && modifica_campo === true) {
                        let nuovo_dataItem = PortataCambiataIrrigazione(dataItem);

                        if (nuovo_dataItem !== undefined && nuovo_dataItem !== null &&
                            nuovo_dataItem.Qta_Totale !== undefined && nuovo_dataItem.Qta_Totale !== null &&
                            nuovo_dataItem.Dose !== undefined && nuovo_dataItem.Dose !== null) {

                            ds_grid_impianti_irrigazione[x].Portata = portata;

                            dataItem.Portata = portata;

                            ds_grid_impianti_irrigazione[x].Qta_Totale = nuovo_dataItem.Qta_Totale;

                            dataItem.Qta_Totale = nuovo_dataItem.Qta_Totale;

                            ds_grid_impianti_irrigazione[x].Dose = nuovo_dataItem.Dose;

                            dataItem.Dose = nuovo_dataItem.Dose;

                        }
                    }


                    if (dose !== "" && modifica_campo === true) {
                        let nuova_Qta_Totale = DoseCambiataIrrigazione(dataItem);

                        if (nuova_Qta_Totale !== null && nuova_Qta_Totale !== undefined) {

                            //aggiorno ore e portata
                            ds_grid_impianti_irrigazione[x].Qta_Totale = nuova_Qta_Totale;

                            dataItem.Qta_Totale = nuova_Qta_Totale;

                            ds_grid_impianti_irrigazione[x].Ore = 0;

                            dataItem.Ore = 0;

                            ds_grid_impianti_irrigazione[x].Portata = 0;

                            dataItem.Portata = 0;

                            ds_grid_impianti_irrigazione[x].Dose = dose;

                            dataItem.Dose = dose;

                        }
                    }


                    if (qta_totale !== "" && modifica_campo === true) {
                        let nuova_dose = QtaTotCambiataIrrigazione(dataItem);

                        ds_grid_impianti_irrigazione[x].Qta_Totale = qta_totale;

                        dataItem.Qta_Totale = qta_totale;

                        //aggiorno la dose e aggiorno le ore e la portata
                        ds_grid_impianti_irrigazione[x].Dose = nuova_dose;

                        dataItem.Dose = nuova_dose;

                        ds_grid_impianti_irrigazione[x].Ore = 0;

                        dataItem.Ore = 0;

                        ds_grid_impianti_irrigazione[x].Portata = 0;

                        dataItem.Portata = 0;
                    }


                    if (modifica_campo === true) {
                        if (ModifTipoIrriUtilizzata_Cod !== "" && ModifTipoIrriUtilizzata_Des !== "") {
                            ds_grid_impianti_irrigazione[x].ModifTipoIrriUtilizzata_Cod = ModifTipoIrriUtilizzata_Cod;
                            ds_grid_impianti_irrigazione[x].ModifTipoIrriUtilizzata_Des = ModifTipoIrriUtilizzata_Des;

                        }
                    }


                    if (modifica_campo === true) {
                        //Ricalcolo la Qta_Totale_Acqua_Periodo
                        let nuova_Qta_Totale_Acqua_Periodo = Calcola_Qta_Totale_Acqua_Periodo(dataItem);

                        if (nuova_Qta_Totale_Acqua_Periodo !== null) {
                            ds_grid_impianti_irrigazione[x].Qta_Totale_Acqua_Periodo = nuova_Qta_Totale_Acqua_Periodo;

                            ds_grid_impianti_irrigazione[x].Frequenza = dataItem.Frequenza;

                            ds_grid_impianti_irrigazione[x].Data_Inizio = dataItem.Data_Inizio;
                            ds_grid_impianti_irrigazione[x].Data_Fine = dataItem.Data_Fine;
                        }
                        else {
                            return;
                        }
                    }

                }

            }

            grid_impianti_irrigazione.refresh();
        }
    }

    $('#dialogImpostaValoriGrid_Impianti_Irrigazione').modal('hide');
    WaitFrame.hide();
}


function Data_Irrigazione_change(e) {

    Controlla_Sportello();

    var ddl_specie = $("#ddl_Specie_Irrigazione").data("kendoDropDownList");

    if (ddl_specie !== undefined && ddl_specie !== null && ddl_specie !== "") {
        ddl_specie.dataSource.read();
        ddl_specie.refresh();
        Set_KendoDDLValue("ddl_Specie_Irrigazione", "-1");
    }

    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = "";

    if (Dati !== null && Dati !== undefined && Dati !== "") {
        objParametriAgenda = JSON.parse(Dati.objParametriAgenda);
    }


    if (objParametriAgenda !== "" &&
        (parseInt(objParametriAgenda.TargetOperazione) === enum_tipoOperazione_Agenda_Target.Reale)) {

        var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");


        if (grid !== undefined && grid !== null && grid !== "") {

            if (Controlli_Prima_Di_creare_la_kendoGrid_Impianti_Irrigazione() === true) {
                //Distruggo e ricarico la grid
                grid.destroy();
                $("#grid_Impianti_Irrigazione").empty();

                $("#grid_Impianti_Irrigazione").attr("dataBounded", "false");

                kendoGrid_Impianti_Irrigazione("grid_Impianti_Irrigazione");
            }

        }
    }

}


function ddl_Centro_Aziendale_Irrigazione_change(e) {
    //Aggiorno anche la Dropdown Specie e quella del Campo
    if ($("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== undefined &&
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== null &&
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== "") {

        $("#ddl_Specie_Irrigazione").data("kendoDropDownList").dataSource.read();
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList").refresh();
        Set_KendoDDLValue("ddl_Specie_Irrigazione", "-1");
    }

    var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");
    if (grid !== undefined && grid !== null && grid !== "") {

        if (Controlli_Prima_Di_creare_la_kendoGrid_Impianti_Irrigazione() === true) {
            //Distruggo e ricarico la grid
            grid.destroy();
            $("#grid_Impianti_Irrigazione").empty();

            $("#grid_Impianti_Irrigazione").attr("dataBounded", "false");

            kendoGrid_Impianti_Irrigazione("grid_Impianti_Irrigazione");
        }

    }
}

function ddl_Tipo_Irrigazione_change(e) {

    var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid === undefined || grid === null || grid === "")
        return;

    var ds_grid = grid.dataSource.data();

    if (ds_grid === undefined || ds_grid === null || ds_grid === "")
        return;

    //Chiedo conferma prima di selezionare le righe della griglia , perchè verrano deselezionate
    //le eventuali righe già valorizzate
    let TipoIrr = parseInt(Get_KendoDDLValue("ddl_Tipo_Irrigazione"));
    let mostra_dialog_conferma = false;
    let elenco_righe_valorizzate = [];

    for (var x = 0; x < ds_grid.length; x++) {
        if (parseInt(ds_grid[x].Imp_Cod) !== TipoIrr && ds_grid[x].Selected === true) {
            mostra_dialog_conferma = true;
            elenco_righe_valorizzate.push(ds_grid[x]);
        }
    }


    if (mostra_dialog_conferma === true &&
        elenco_righe_valorizzate.length > 0 &&
        $("#ddl_Tipo_Irrigazione").attr("last_selected_imp_cod") !== undefined) {

        var msgText = "Si desidera davvero cambiare il Tipo di Irrigazione? Verranno deselezionate e resettate le seguenti righe: <br> ";

        for (var x = 0; x < elenco_righe_valorizzate.length; x++) {
            msgText += "- <strong>" + elenco_righe_valorizzate[x].Sa_Nome + "," + elenco_righe_valorizzate[x].App_Nome + "," + elenco_righe_valorizzate[x].Cul_Des + "," + elenco_righe_valorizzate[x].Imp_Des + ".</strong> <br>";
        }

        let container = document.getElementById("grid_Impianti_Irrigazione");
        let id_dialog = creaNewRowDiv("id_dialog_change_tipo_irrigazione");
        container.appendChild(id_dialog);

        $("#id_dialog_change_tipo_irrigazione").kendoDialog({
            title: "Conferma Cambio Tipo di Irrigazione",
            closable: false,
            modal: {
                preventScroll: true
            },
            content: msgText,
            actions: [{
                text: 'No',
                primary: true,
                action: function (e) {
                    Set_KendoDDLValue("ddl_Tipo_Irrigazione", parseInt($("#ddl_Tipo_Irrigazione").attr("last_selected_imp_cod")));

                    $("#id_dialog_change_tipo_irrigazione").remove();
                }
            },
            {
                text: 'Sì',
                action: function (e) {

                    WaitFrame.show();

                    $("#ddl_Tipo_Irrigazione").attr("last_selected_imp_cod", Get_KendoDDLValue("ddl_Tipo_Irrigazione"));

                    for (var x = 0; x < ds_grid.length; x++) {
                        let seleziona_riga = false;

                        if (TipoIrr === -2) {
                            //riga vuota
                            //seleziona_riga = False
                        }
                        else {
                            //controllo tipo irrigazione impianto e selezionola riga se è la stessa di "Seleziona per tipo di Irrigazione"
                            //se non è la stessa resetto la riga e la deseleziono
                            if (parseInt(ds_grid[x].Imp_Cod) === TipoIrr) {
                                seleziona_riga = true;
                                ds_grid[x].Selected = seleziona_riga;
                                Inserisci_ValoreSuperficieCoinvolta_ImpiantoIrrigazione(false, ds_grid[x]);
                            }
                            else {
                                ds_grid[x].Selected = seleziona_riga;
                                Resetta_riga_Impianti_Irrigazione(ds_grid[x], false);
                            }
                        }

                    }

                    let righe_selezionate = 0;
                    let righe_deselezionate = 0;

                    for (var x = 0; x < ds_grid.length; x++) {
                        if (ds_grid[x].Selected === true) {
                            righe_selezionate++;
                        }

                        if (ds_grid[x].Selected === undefined || ds_grid[x].Selected === null || ds_grid[x].Selected === "" || ds_grid[x].Selected === false) {
                            righe_deselezionate++;
                        }
                    }

                    //Se tutte le righe sono selezionate allora checko la checkbox della colonna
                    if (ds_grid.length > 0 && ds_grid.length === righe_selezionate)
                        $("#grid_Impianti_Irrigazione-header-chb").prop("checked", true);

                    //Se tutte le righe sono deselezionate allora checko la checkbox della colonna
                    if (ds_grid.length > 0 && ds_grid.length === righe_deselezionate)
                        $("#grid_Impianti_Irrigazione-header-chb").prop("checked", false);

                    Abilita_Disabilita_Resto();

                    RicalcolaSuperficieTotaleIrrigazione();
                    grid.refresh();

                    WaitFrame.hide();

                    $("#id_dialog_change_tipo_irrigazione").remove();
                },
            }]
        });
    }
    else {
        WaitFrame.show();

        $("#ddl_Tipo_Irrigazione").attr("last_selected_imp_cod", Get_KendoDDLValue("ddl_Tipo_Irrigazione"));

        for (var x = 0; x < ds_grid.length; x++) {
            let seleziona_riga = false;

            if (TipoIrr === -2) {
                //riga vuota
                //seleziona_riga = False
            }
            else {
                //controllo tipo irrigazione impianto e selezionola riga se è la stessa di "Seleziona per tipo di Irrigazione"
                if (parseInt(ds_grid[x].Imp_Cod) === TipoIrr) {
                    seleziona_riga = true;
                    ds_grid[x].Selected = seleziona_riga;
                    Inserisci_ValoreSuperficieCoinvolta_ImpiantoIrrigazione(false, ds_grid[x]);
                }
            }
        }

        let righe_selezionate = 0;
        let righe_deselezionate = 0;

        for (var x = 0; x < ds_grid.length; x++) {
            if (ds_grid[x].Selected === true) {
                righe_selezionate++;
            }

            if (ds_grid[x].Selected === undefined || ds_grid[x].Selected === null || ds_grid[x].Selected === "" || ds_grid[x].Selected === false) {
                righe_deselezionate++;
            }
        }

        //Se tutte le righe sono selezionate allora checko la checkbox della colonna
        if (ds_grid.length > 0 && ds_grid.length === righe_selezionate)
            $("#grid_Impianti_Irrigazione-header-chb").prop("checked", true);

        //Se tutte le righe sono deselezionate allora checko la checkbox della colonna
        if (ds_grid.length > 0 && ds_grid.length === righe_deselezionate)
            $("#grid_Impianti_Irrigazione-header-chb").prop("checked", false);

        Abilita_Disabilita_Resto();

        RicalcolaSuperficieTotaleIrrigazione();
        grid.refresh();

        WaitFrame.hide();
    }
}


function ddl_Specie_Irrigazione_change(e) {

    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = "";

    if (Dati !== null && Dati !== undefined && Dati !== "") {
        objParametriAgenda = JSON.parse(Dati.objParametriAgenda);
    }


    //Mostro il dialog di cambio specie solo se effetivamente sono stati selezionati o
    //deselezionati dei checkbox dall'utente.
    //Al cambio di specie ricarico anche le tab delle note se sono in scrittura.
    let TipoOperazioneDB = parseInt($(TipoOperazione).val());
    if (TipoOperazioneDB === enum_tipoOperazione.Scrittura &&
        $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod") !== undefined &&
        Elenco_Checkbox_Note !== undefined &&
        Elenco_Checkbox_Note !== null &&
        Elenco_Checkbox_Note.length > 0) {

        var Elenco_CheckBox_Letti = Elenco_Checkbox_Note;
        var mostra_dialog_cambio_specie = false;

        if (Elenco_CheckBox_Letti !== undefined && Elenco_CheckBox_Letti !== null && Elenco_CheckBox_Letti.length > 0) {

            let ElencoCheckBox = [];

            $('div#checkbox_a_tabGiust input[type=checkbox]').each(function () {
                ElencoCheckBox.push({ Nota_Cod: parseInt($(this).attr('Nota_Cod')), Checkato: $(this).is(":checked").toString() });
            });


            $('div#checkbox_a_tabMeteo input[type=checkbox]').each(function () {
                ElencoCheckBox.push({ Nota_Cod: parseInt($(this).attr('Nota_Cod')), Checkato: $(this).is(":checked").toString() });
            });


            $('div#checkbox_a_tabVentoIntensita input[type=checkbox]').each(function () {
                ElencoCheckBox.push({ Nota_Cod: parseInt($(this).attr('Nota_Cod')), Checkato: $(this).is(":checked").toString() });
            });


            $('div#checkbox_a_tabVentoDirezione input[type=checkbox]').each(function () {
                ElencoCheckBox.push({ Nota_Cod: parseInt($(this).attr('Nota_Cod')), Checkato: $(this).is(":checked").toString() });
            });


            $('div#checkbox_a_tabTemperatura input[type=checkbox]').each(function () {
                ElencoCheckBox.push({ Nota_Cod: parseInt($(this).attr('Nota_Cod')), Checkato: $(this).is(":checked").toString() });
            });


            $('div#checkbox_a_tabOrario input[type=checkbox]').each(function () {
                ElencoCheckBox.push({ Nota_Cod: parseInt($(this).attr('Nota_Cod')), Checkato: $(this).is(":checked").toString() });
            });


            $('div#checkbox_a_tabMotivazioni input[type=checkbox]').each(function () {
                ElencoCheckBox.push({ Nota_Cod: parseInt($(this).attr('Nota_Cod')), Checkato: $(this).is(":checked").toString() });
            });

            if (ElencoCheckBox.length > 0) {
                for (var x = 0; x < Elenco_CheckBox_Letti.length; x++) {
                    for (var y = 0; y < ElencoCheckBox.length; y++) {
                        if (parseInt(Elenco_CheckBox_Letti[x].Nota_Cod) === parseInt(ElencoCheckBox[y].Nota_Cod) &&
                            Elenco_CheckBox_Letti[x].Checkato !== ElencoCheckBox[y].Checkato) {
                            mostra_dialog_cambio_specie = true;
                            break;
                        }
                    }
                }
            }
        }

        if (mostra_dialog_cambio_specie === true) {
            var msgText = "Si desidera davvero cambiare la Specie selezionata?(Verranno ripristinati i valori di default delle Note)";

            let container = document.getElementById("grid_Impianti_Irrigazione");
            let id_dialog = creaNewRowDiv("id_dialog_change_specie");
            container.appendChild(id_dialog);

            $("#id_dialog_change_specie").kendoDialog({
                title: "Conferma Cambio Specie",
                closable: false,
                modal: {
                    preventScroll: true
                },
                content: msgText,
                actions: [{
                    text: 'No',
                    primary: true,
                    action: function (e) {
                        Set_KendoDDLValue("ddl_Specie_Irrigazione", $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod"));

                        $("#id_dialog_change_specie").remove();
                    }
                },
                {
                    text: 'Sì',
                    action: function (e) {

                        GestioneTabDivNote();

                        $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod", Get_KendoDDLValue("ddl_Specie_Irrigazione"));

                        var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

                        if (grid !== undefined && grid !== null && grid !== "") {

                            if (Controlli_Prima_Di_creare_la_kendoGrid_Impianti_Irrigazione() === true) {

                                //Distruggo e ricarico la grid
                                grid.destroy();
                                $("#grid_Impianti_Irrigazione").empty();

                                $("#grid_Impianti_Irrigazione").attr("dataBounded", "false");

                                kendoGrid_Impianti_Irrigazione("grid_Impianti_Irrigazione");
                            }

                        }

                        $("#id_dialog_change_specie").remove();
                    },
                }]
            });
        }
        else {

            GestioneTabDivNote();

            $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod", Get_KendoDDLValue("ddl_Specie_Irrigazione"));

            if (objParametriAgenda !== "" &&
                (parseInt(objParametriAgenda.TargetOperazione) === enum_tipoOperazione_Agenda_Target.Reale)) {

                var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

                if (grid !== undefined && grid !== null && grid !== "") {

                    if (Controlli_Prima_Di_creare_la_kendoGrid_Impianti_Irrigazione() === true) {

                        //Distruggo e ricarico la grid
                        grid.destroy();
                        $("#grid_Impianti_Irrigazione").empty();

                        $("#grid_Impianti_Irrigazione").attr("dataBounded", "false");

                        kendoGrid_Impianti_Irrigazione("grid_Impianti_Irrigazione");
                    }

                }
            }
        }
    } else {

        $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod", Get_KendoDDLValue("ddl_Specie_Irrigazione"));


        if (objParametriAgenda !== "" &&
            (parseInt(objParametriAgenda.TargetOperazione) === enum_tipoOperazione_Agenda_Target.Reale)) {

            var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

            if (grid !== undefined && grid !== null && grid !== "") {

                if (Controlli_Prima_Di_creare_la_kendoGrid_Impianti_Irrigazione() === true) {
                    //Distruggo e ricarico la grid
                    grid.destroy();
                    $("#grid_Impianti_Irrigazione").empty();

                    $("#grid_Impianti_Irrigazione").attr("dataBounded", "false");

                    kendoGrid_Impianti_Irrigazione("grid_Impianti_Irrigazione");
                }

            }
        }
    }


}

function ddl_Udm_Irrigazione_change() {

    let grid_Irrigazione = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid_Irrigazione !== undefined && grid_Irrigazione !== null) {
        let nuovo_ds_grid_Irrigazione = grid_Irrigazione.dataSource.data();

        for (var x = 0; x < nuovo_ds_grid_Irrigazione.length; x++) {

            if (nuovo_ds_grid_Irrigazione[x].Selected === true) {
                //aggiorno la dose
                nuovo_ds_grid_Irrigazione[x].Dose = QtaTotCambiataIrrigazione(nuovo_ds_grid_Irrigazione[x]);

                //aggiorno ore e portata
                nuovo_ds_grid_Irrigazione[x].Ore = 0;
                nuovo_ds_grid_Irrigazione[x].Portata = 0;
            }

        }

        grid_Irrigazione.refresh();

        Imposta_nome_Colonna_Dose_Nella_grid_Impianti_Irrigazione();
    }
}


function SubmitGrigliaImpianti_Irrigazione(options) {

    var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();

    for (let i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
    }

    if (righeNonCancellate.length > 0) {

        for (let i = 0; i < righeNonCancellate.length; i++) {
            if (righeNonCancellate[i].Selected === "" ||
                righeNonCancellate[i].Selected === undefined ||
                righeNonCancellate[i].Selected === null)

                righeNonCancellate[i].Selected = false;


            let prop = Object.getOwnPropertyNames(righeNonCancellate[i]);

            if(prop != null){
                for (let x = 0; x < prop.length; x++) {
                    if (typeof righeNonCancellate[i][prop[x]] === 'string') {
                        righeNonCancellate[i][prop[x]] = righeNonCancellate[i][prop[x]].replace(/"/g, "$%&£").replace("\\", "$%&£");
                    }
                }
            }
        }

        righeTutteGrid_Impianti_Irrigazione = kendoEscapeOggetto(righeNonCancellate);
    }
}

function kendoGrid_RilieviPioggie_Irrigazione(divKendoGrid_RilieviPioggie_Irrigazione) {

    //Messaggio "Rilievi Piogge Nel Periodo Specificato" sopra la grid Rilievi Pioggie
    $("#Mess_grid_RilieviPioggie_Irrigazione").html("");
    $("#Mess_grid_RilieviPioggie_Irrigazione").empty();
    $("#Mess_grid_RilieviPioggie_Irrigazione").html("<strong>Rilievi Piogge Nel Periodo Specificato.</strong>");

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var InserimentoModifica = false;
    var Cancellazione = false;

    var funzioniCRUD = {};

    //Se l'Utente ha il permesso di modifica allora aggiungo la colonna con i check altrimenti no
    if (UteAbilitatoInsMod === true && UteAbilitatoCanc === true) {
        InserimentoModifica = true;
    }


    funzioniCRUD.funzioneRead = kReadRilieviPioggie_Irrigazione;   //kendo_rows
    funzioniCRUD.funzioneSubmit = { funzione: null, flagInsert: false, flagDelete: false, flagUpdate: false };
    funzioniCRUD.UtenteAbilitatoInserimentoModifica = InserimentoModifica;
    funzioniCRUD.UtenteAbilitatoCancellazione = Cancellazione;
    funzioniCRUD.omettiPulsantiSalva = true;
    funzioniCRUD.omettiPulsantiAnnulla = true;


    var idModel = "Id_Grid_RilieviPioggie";
    var campiKendoModel = kReadRilieviPioggie_Irrigazione_mod(); //kendo_model
    var colonneKendoGrid = kReadRilieviPioggie_Irrigazione_col(); //kendo_columns
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: false,
        toolbarCommands: [],
        reorderable: true,
        excel: true,
        pdf: true,
        groupable: false,
        btnEliminaTuttiFiltri: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
    };
    var funzioniPrimaDopoEventi = {};
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(divKendoGrid_RilieviPioggie_Irrigazione, // rappresenta l'ID del div a cui si associa la griglia
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

function kReadRilieviPioggie_Irrigazione(options) {

    if ($(KendoGridRilieviPioggie_Irrigazione).val() === undefined ||
        $(KendoGridRilieviPioggie_Irrigazione).val() === "" ||
        $(KendoGridRilieviPioggie_Irrigazione).val() === null)
        return;

    var data = $(KendoGridRilieviPioggie_Irrigazione).val();

    jSonParsed_Kendo = JSON.parse(data);

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadRilieviPioggie_Irrigazione_mod() {

    if ($(KendoGridRilieviPioggie_Irrigazione).val() === undefined ||
        $(KendoGridRilieviPioggie_Irrigazione).val() === "" ||
        $(KendoGridRilieviPioggie_Irrigazione).val() === null)
        return;


    var data = $(KendoGridRilieviPioggie_Irrigazione).val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function kReadRilieviPioggie_Irrigazione_col() {

    if ($(KendoGridRilieviPioggie_Irrigazione).val() === undefined ||
        $(KendoGridRilieviPioggie_Irrigazione).val() === "" ||
        $(KendoGridRilieviPioggie_Irrigazione).val() === null)
        return;


    var data = $(KendoGridRilieviPioggie_Irrigazione).val();
    jSonParsed_Kendo = JSON.parse(data);


    return jSonParsed_Kendo.kendo_columns;
}

function BottoneSalvaRicetta(bottone, Ricetta_Cod_p, Ricetta_Operazione_Cod_p, dataOp) {

    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = "";

    if (Dati !== null && Dati !== undefined && Dati !== "") {
        objParametriAgenda = JSON.parse(Dati.objParametriAgenda);
    }


    if (($(Ricetta_Cod).val() === '0' || $(Ricetta_Cod).val() === '') && (parseInt($(TipoOperazione).val()) === enum_tipoOperazione.Scrittura)) {
        //presetto i valori      
        var ricetta_des = '';
        var ricetta_numero = '';
        var data_inizio_ricetta;
        var data_fine_ricetta;


        //ricavo il default del numero ricetta
        ricetta_numero = Ricetta_Irrigazione_numero_default();

        if ($("#ddl_Specie_Irrigazione").data("kendoDropDownList") === undefined ||
            $("#ddl_Specie_Irrigazione").data("kendoDropDownList") === null ||
            $("#ddl_Specie_Irrigazione").data("kendoDropDownList") === "")
            return;

        var specie = $("#ddl_Specie_Irrigazione").data("kendoDropDownList").text();

        if (objParametriAgenda.Lav_Des === undefined)
            return;

        var operazione = objParametriAgenda.Lav_Des;

        data_inizio_ricetta = $("#Data_Irrigazione").val();
        data_fine_ricetta = $("#Data_Irrigazione").val();

        switch (bottone) {
            case 1:
                ricetta_des = operazione + ' ' + specie + ' ' + data_inizio_ricetta;
                break;
            case 2:
                ricetta_des = operazione + ' ' + specie + ' ' + data_inizio_ricetta;
                break;

            case 3:
                ricetta_des = specie;
                break;

            default:
                ricetta_des = specie;
                break;
        }

        if ($("#Txt_Ricetta_Descrizione_Irrigazione").val() == "") {
            $("#Txt_Ricetta_Descrizione_Irrigazione").val(ricetta_des);
        }

        if ($("#Txt_Ricetta_Numero_Irrigazione").val() == "") {
            $("#Txt_Ricetta_Numero_Irrigazione").val(ricetta_numero);
        }

        if ($("#Data_Inizio_ricetta_Irrigazione").val() == "") {
            $("#Data_Inizio_ricetta_Irrigazione").data("kendoDatePicker").value(data_inizio_ricetta);
        }

        if ($("#Data_Fine_ricetta_Irrigazione").val() == "") {
            $("#Data_Fine_ricetta_Irrigazione").data("kendoDatePicker").value(data_fine_ricetta);
        }

        //$('#hd_bottone').text(bottone);


        SalvaRicettaconTestata(bottone);
    } else {

        var tipo_salvataggio = 0;

        switch (bottone) {
            case 1:
                tipo_salvataggio = 1;
                break;

            case 2:
                tipo_salvataggio = 2;
                break;

            case 3:
                tipo_salvataggio = 5;
                break;
        }

        var obj_dettaglio_Irrigazione = null;

        if (Ricetta_Cod_p !== undefined && Ricetta_Operazione_Cod_p !== undefined) {
            obj_dettaglio_Irrigazione = {
                ricetta_cod: Ricetta_Cod_p,
                ricetta_operazione_cod: Ricetta_Operazione_Cod_p,
                data: dataOp
            }
        }

        Salva_Irrigazione(tipo_salvataggio, obj_dettaglio_Irrigazione);

    }
}


function SalvaRicettaconTestata(bottone) {

    var ricetta_des = $("#Txt_Ricetta_Descrizione_Irrigazione").val();
    var ricetta_numero = $("#Txt_Ricetta_Numero_Irrigazione").val();

    if (ricetta_des === '' || ricetta_numero === '') {
        alert("Inserire Descrizione e Numero della Ricetta!");
    } else {

        var tipo_salvataggio = 0;

        switch (bottone) {
            case 1:
                tipo_salvataggio = 1;
                break;

            case 2:
                tipo_salvataggio = 2;
                break;

            case 3:
                tipo_salvataggio = 5;
                break;
        }


        Salva_Irrigazione(tipo_salvataggio, null);
    }
}

function Abilita_Disabilita_Resto() {

    let ddlSpecie = $("#ddl_Specie_Irrigazione").data("kendoDropDownList");

    if (ddlSpecie === undefined ||
        ddlSpecie === null ||
        ddlSpecie === "")
        return;

    let n_impianti = Numero_ImpiantiSelezionatigrid_Irrigazione();


    if (n_impianti !== null) {

        //Disabilito la ddl Specie
        if (n_impianti > 0) {
            ddlSpecie.enable(false);
        }
        //Lascio abilitate la ddl Specie
        else {
            ddlSpecie.enable(true);
        }

    }

}


function Numero_ImpiantiSelezionatigrid_Irrigazione() {
    let grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid === undefined ||
        grid === null ||
        grid === "")
        return null;

    let num_impiantiSelezionati = 0;

    for (var x = 0; x < grid.dataSource.data().length; x++) {

        if (grid.dataSource.data()[x].Selected === true)
            num_impiantiSelezionati++;
    }

    return num_impiantiSelezionati;
}

function popolaTestataRicetta_Irrigazione(ricettaJSON) {

    var ricetta_des = ricettaJSON.Ricetta_Des;
    var ricetta_numero = ricettaJSON.Ricetta_Numero;
    var data_inizio_ricetta = new Date(ricettaJSON.Validita_Inizio).toLocaleDateString();
    var data_fine_ricetta = new Date(ricettaJSON.Valitida_Fine).toLocaleDateString();
    var ricetta_note = ricettaJSON.Note;

    $("#Txt_Ricetta_Descrizione_Irrigazione").val(ricetta_des);
    $("#Txt_Ricetta_Numero_Irrigazione").val(ricetta_numero);

    $("#Data_Inizio_ricetta_Irrigazione").val(data_inizio_ricetta);
    $("#Data_Fine_ricetta_Irrigazione").val(data_fine_ricetta);

    $("#Txt_Ricetta_Nota_Irrigazione").val(ricetta_note);
}

function mostraNascondiIrrigazione(Div) {
    if ($("#" + Div).is(":visible")) {
        $("#" + Div).hide();
        $("#a_Titolo_Note_Irrigazione").removeClass("fa fa-minus-circle");
        $("#a_Titolo_Note_Irrigazione").addClass("fa fa-plus-circle");
    } else {
        $("#" + Div).show();
        $("#a_Titolo_Note_Irrigazione").removeClass("fa fa-plus-circle");
        $("#a_Titolo_Note_Irrigazione").addClass("fa fa-minus-circle");
    }
}

function Imposta_nome_Colonna_Dose_Nella_grid_Impianti_Irrigazione() {

    let grid_Irrigazione = $("#grid_Impianti_Irrigazione").data("kendoGrid");

    if (grid_Irrigazione !== undefined && grid_Irrigazione !== null) {
        //Imposto il nome della colonna dose in base all' Unità di misura della Dose scelta
        if (GiasVersioneMaster === "2022") {
            $("#grid_Impianti_Irrigazione th[data-field=Dose] .k-link").html("<span class=\"k-column-title\">Dose Acqua [" + $("#ddl_Udm_Irrigazione").data("kendoDropDownList").text() + "]</span>");
        } else {
            $("#grid_Impianti_Irrigazione th[data-field=Dose] .k-link").html("Dose Acqua [" + $("#ddl_Udm_Irrigazione").data("kendoDropDownList").text() + "]");
        }
    }

}

function DSS_Irrigazione_DropdownEditor(container,options) {

    let ElencoDSS = Leggi_DSS_Irrigazione(options);

    creaDropDownEditor(container, "Descrizione_DSS_Irrigazione", "ID_DSS_Irrigazione", ElencoDSS, ID_DSS_Irrigazione_change);
}

function ID_DSS_Irrigazione_change(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grid_Impianti_Irrigazione").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.ID_DSS_Irrigazione = dataItem.ID_DSS_Irrigazione;
    model.Descrizione_DSS_Irrigazione = dataItem.Descrizione_DSS_Irrigazione;
    model.Qta_Acqua_DSS_Irrigazione = dataItem.Qta_Acqua_DSS_Irrigazione;
    model.Udm_Cod_DSS_Irrigazione = dataItem.Udm_Cod_DSS_Irrigazione;
    model.Min_Data_Turno = dataItem.Min_Data_Turno;
    model.Max_Data_Turno = dataItem.Max_Data_Turno;
}

function Calcola_Dose_from_Turni(obj_DSS_Irrigazione,Frequenza) {

    let Dose = 0;

    let giorni_irrigati = Get_Giorni_Irrigati(new Date(obj_DSS_Irrigazione.Min_Data_Turno), new Date(obj_DSS_Irrigazione.Max_Data_Turno), Frequenza);

    if (giorni_irrigati > 0) 
        Dose = roundNumber(obj_DSS_Irrigazione.Qta_Acqua_DSS_Irrigazione / giorni_irrigati,4);
    

    let result = Dose;

    if (isNaN(result) || result === Infinity)
        result = 0;

    return result;

}