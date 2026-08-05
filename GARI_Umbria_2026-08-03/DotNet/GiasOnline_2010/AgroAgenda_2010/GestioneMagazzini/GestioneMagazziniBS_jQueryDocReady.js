var gestioneMagazziniResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "GestioneMagazzini/App_LocalResources/GestioneMagazziniBS.aspx.resx"
];

// Tipi Magazzino
var ESSICCATORIO = 222;
var MAGAZZINO = 20;
var STALLA = 15;
var CELLA = 16;
var FABBRICATI_NO_STALLE = -999;

var isModuloFFAttivo = false;
var listaCauCarico = ["7300", "4100", "7920"];
var listaCauScarico = ["7350", "4200", "7900"];

//DOCUMENT READY
jQuery(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            gestioneMagazziniResx.push(readResxFile(resxSinglePath, "GestioneMagazziniBS_jQueryDocReady.js"));
        });
    }

    isModuloFFAttivo = isModuloFF();

    // nella nuova versione il panel è sostituito dalla sidebar destra
    if (GiasVersioneMaster !== "2022") {
        $("#AccordMenu").kendoPanelBar();
    }
    //Dialog Errore Kendo
    $("#dialogErrorKendo").kendoDialog({
        title: "", content: "", visible: false
    });
    //Sono costretta a fare così per avere dell'html nel titolo
    $(".k-dialog-title").append("<h4 class='modal-title'><i class='fa fa-exclamation-triangle fa-3x'></i> Si è verificato un problema</h4>");

    //Dialog OK Kendo
    $("#dialogOkKendo").kendoDialog({
        title: "OK", content: "", visible: false
    });


    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31),
        format: "dd/MM/yyyy"
    });

    //Kendo Switch
    let yesLabel = TraduzioneMultiResx(gestioneMagazziniResx, "Si", "si").toUpperCase();
    let noLabel = TraduzioneMultiResx(gestioneMagazziniResx, "No", "No").toUpperCase();
    creaKendoSwitch("CheckBoxCarichi", yesLabel, noLabel, true, undefined);
    creaKendoSwitch("CheckBoxScarichi", yesLabel, noLabel, true, undefined);
    creaKendoSwitch("CheckBoxGiacenze0", yesLabel, noLabel, false, clickGiacenze0);
    creaKendoSwitch("CheckBoxValorizzaProdotto", yesLabel, noLabel, false, clickValorizzaProdotto);
    if ($("input[name$='hf_UtenteAbilitatoGestionePrezziLettura']").val() !== "True") {
        $("#groupValorizzaProdotto").hide();
    }
    creaKendoSwitch("CheckBoxFiltraPerLotto", yesLabel, noLabel, false, undefined);

    $("#TxtLotto").on("change", function (ev) {

        if (ev.currentTarget.value.length > 0) {
            setKendoSwitch("CheckBoxFiltraPerLotto", true);
        }
        else {
            setKendoSwitch("CheckBoxFiltraPerLotto", false);
        }

    });

    creaKendoDropDownListWithData("ddlArrotondamento", [
        { value: 0, text: TraduzioneMultiResx(gestioneMagazziniResx, "AdIntero.Text", "All&#39;Intero") },
        { value: 1, text: TraduzioneMultiResx(gestioneMagazziniResx, "UnaCifra.Text", "1 cifra (0.1)") },
        { value: 2, text: TraduzioneMultiResx(gestioneMagazziniResx, "DueCifre.Text", "2 cifre (0.01)") },
        { value: 3, text: TraduzioneMultiResx(gestioneMagazziniResx, "TreCifre.Text", "3 cifre (0.001)") },
        { value: 4, text: TraduzioneMultiResx(gestioneMagazziniResx, "QuattroCifre.Text", "4 cifre (0.0001)") }
    ]);
    Set_KendoDDLValue("ddlArrotondamento", 2);

    creaKendoDropDownList("ddlCategoria", { read: LeggiCategorie }, "NomeComune", "Elem_Cod", null, null, null, true);
    creaKendoDropDownList("ddlCentroAziendale", { read: LeggiCentri }, "sa_nome", "sa_cod", null, null, null, true).bind("change", ddlCentroAziendale_change);

    LeggiMagazzini();
    creaKendoDropDownList("ddlMagazzini", { read: RiempiElencoCelleEMagazzini }, "Ubic_Des", "key_Dest");

    MostraFiltriGiacenze();

    var flag_footer = 0;

    $("#AccordMenu").on("shown.bs.collapse", function () {
        if (flag_footer === 0) {
            // ricalcolaPosizioneFooter();
            flag_footer = 1;
        }
    });

    $(".ricerca_btn").click(function () {
        if (($(this).attr("aria-expanded") === "true") || !$(this).attr("aria-expanded"))
            $(this).find("i").removeClass("fa-plus-circle").addClass("fa-minus-circle");
        else
            $(this).find("i").removeClass("fa-minus-circle").addClass("fa-plus-circle");
    });

    var visualizzazione_mode = parseInt($(cIdhdVisualizzazioneMode).val());

    ImpostaDefaultFiltri();

    //Salvo i filtri solo se arrivo nella pagina effettiva dei magazzini
    if (!visualizzazione_mode > 0) {
        //salvo i valori di default
        SalvaParametriDefault();

        //cancella se diverso da pagina precedente
        CancellaSediversoDa_paginaprecedente("#frmInput", "menu.aspx");

        //imposto eventualmente i parametri precedenti
        SetParametri();
    }

    if (KendoDDL("ddlMagazzini").dataItems().length === 1) { //ho un solo elemento, lo seleziono
        KendoDDL("ddlMagazzini").select(0);
    } else if (KendoDDL("ddlMagazzini").select() === -1) { //check whether any item is selected
        Set_KendoDDLValue("ddlMagazzini", 0);
    }


    var tab_richiesto = parseInt($(cIdhdTabRichiesto).val());

    if (tab_richiesto > 0) {
        $("#tabMovimenti").show();
        $("#tabGiacenze").hide();
        $("#tabs").hide();
        popolaMovimenti("tabMovimenti", null);
    }

    if (visualizzazione_mode > 0) {
        $("#DivToolbar").hide();
        $("#DivFiltri").hide();
        $("#StampaMovimenti").hide();
        $("#xoRicercaDocToggleFiltri").hide();
    }
});

function ImpostaDefaultFiltri() {

    Set_KendoDDLValue("ddlCategoria", filtriDefault.Elem_Cod);
    Set_KendoDDLValue("ddlCentroAziendale", filtriDefault.Sa_Cod);
    Set_KendoDDLValue("ddlMagazzini", filtriDefault.Chiave_Magazzino, "");

    //TODO: setta valore default magazzino

    $("#TxtCodProdotto").val(filtriDefault.Prodotto);

    let dataGia = kendo.parseDate(filtriDefault.DataGiacenza);
    if (dataGia !== undefined && dataGia !== null && dataGia !== AGRODATAINIZIO) {
        KendoDate("txt_DataOperazioneAlGiorno").value(dataGia);
    }

    let dataDa = kendo.parseDate(filtriDefault.DataOpDa);
    if (dataDa !== undefined && dataDa !== null && dataDa !== AGRODATAINIZIO) {
        KendoDate("txt_DataOperazioneDa").value(dataDa);
    }
    let dataA = kendo.parseDate(filtriDefault.DataOpA);
    if (dataA !== undefined && dataA !== null && dataA !== AGRODATAFINE) {
        KendoDate("txt_DataOperazioneA").value(dataA);
    }
}

function LeggiMagazzini() {

    var filtro = "";
    //if (solo magazzino, no celle) {
    //    filtro = "M";
    //}

    let saCod = parseInt(getSaCod(true));

    elencoCelleMagazzini = RicercaCelleEMagazzini_Sync(true, FABBRICATI_NO_STALLE, $(cIdPiva).val(), saCod, filtro,
        TraduzioneMultiResx(gestioneMagazziniResx, "TuttiIMagazzini", "Tutti i magazzini").toUpperCase()
    );

    elencoCelle = [];
    elencoMagazzini = [];

    if (Array.isArray(elencoCelleMagazzini)) {
        // Le divido addirittura qui senza fare due letture separate
        // Userò le sole celle solo in caso di F&F + Trasformati vegetali; i soli magazzini in caso contrario
        elencoCelle = elencoCelleMagazzini.filter(function (x) {
            return (x.Tipo_Destinazione !== MAGAZZINO);
        });
        elencoMagazzini = elencoCelleMagazzini.filter(function (x) {
            return (x.Tipo_Destinazione !== CELLA);
        });
    }
}