
//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    // Se la pagina è richiamata in modalità finestra
    if ($(cIdMod).val() === "F") {
        // effettuo degli accorgimenti grafici
        $("#kendoWindowiFrameGeneric").css("display", "none");
        $("#girdAreaDettaglio").css("margin-bottom", 0);
        $(".panel-body").css("margin-bottom", 0);
        $("#filterPanel").css("margin", "20px 10px");
    }

    tabstrip = $("#tabstrip_Filtri").kendoTabStrip({
        animation: false,
    }).data("kendoTabStrip");

    $(".kendoTextBox").kendoTextBox();

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });
        
    var dtOggi = kendo.date.today();
    set_data("Txt_DataRegDal", kendo.date.addDays(dtOggi, -31), null);
    set_data("Txt_DataRegAl", dtOggi, null);
    set_data("Txt_DataGiacenza", dtOggi, null);

    $(".searchArea").show();

    //eventi di click pulsanti
    $("#btn_ricerca").click(function () {
        Esegui_Report(false);
    });

    $("#btn_pulisci_filtri").click(pulisci_filtri_locale);

    Elenco_Parametri_Qualitativi = Leggi_Parametri_Qualitativi();
    Elenco_Specie = Leggi_Specie();
    //Elenco_Centri_Aziendali = LeggiCentri();
    //Inizializza_Combo_Centri_Aziendali();

    creaKendoMultiselect("multiselCentroAziendale", { read: RiempiCentri, data: { sa_cod: -1 } }, "sa_nome", "sa_cod", null, null, null, null);
    
    creaKendoMultiselect("multiselSpecie", { read: RiempiSpecie, data: { Veg_Cod: -1 } }, "veg_des", "veg_cod", null, null, null, SpecieChange);
    creaKendoMultiselect("multiselVarieta", { read: RiempiVarieta, data: { Cul_Cod: -1 } }, "Cul_Des", "Cul_Cod", null, null, null, null);

    creaKendoMultiselect("multiselCategorie", { read: RiempiCategorie }, "Elem_Des", "Elem_Cod");
    creaKendoMultiselectServerFiltering("multiselProdotti", { read: RicercaProdottiCompleto }, 3, "Prodotto_Des", "Prodotto_Cod");
    KendoMultisel("multiselProdotti").bind("filtering", kEventFiltering);
    creaKendoMultiselectServerFiltering("multiselContatti", { read: FiltraContatti }, 3, "nome", "cod_contatto");
    KendoMultisel("multiselContatti").bind("filtering", kEventFiltering);
    creaKendoSwitch("cb_ddt_non_fatturati", undefined, undefined, false);
    creaKendoSwitch("cb_ordini_non_Spediti", undefined, undefined, false);
    creaKendoSwitch("cb_prod_giacenza", undefined, undefined, false, cbProdGiacenzaChange);
    var kddlFabbricato = creaKendoDropDownList("id_ddlFabbricato", { read: RiempiFabbricati }, "Ubic_Des", "key_Dest", null, null, null, true).data("kendoDropDownList");
    kddlFabbricato.enable(false);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    // In base all'utilizzo scelto, nascondo filtri non utilizzati
    switch ($(cIdUtilizzo).val()) {
        case elencoUtilizzi.Trattamenti:
            $("#div_chk_ordini_non_spediti").hide();
            $("#div_chk_ddt_non_fatturati").hide();
            break;

        case elencoUtilizzi.CreazioneFattureAcquisto:
            ImpostaVisibilitaFiltriSwitch();
            break;

        case elencoUtilizzi.CreazioneFattureVendita:
            ImpostaVisibilitaFiltriSwitch();
            break;

        case elencoUtilizzi.CreazioneDdtVenditaOrdine:
            ImpostaVisibilitaFiltriSwitch();
            break;

        default:
            $("#div_chk_ordini_non_spediti").hide();
            $("#div_chk_ddt_non_fatturati").hide();
            break;
    }
    //Nascondo il filtro sul lotto perché attualmente non gestito
    $(".boxFiltroLotto").hide();

    //Leggi_Filtri();
    ImpostaFiltriDaParametri();
    Esegui_Report();

    $("#filterPanel").kendoPanelBar();

    $.logThis("DocReady: FINE");

});