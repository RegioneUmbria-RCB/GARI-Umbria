
var indirizzohttp = "./Trasferimento.aspx";   
var FF_gest_materiale_vivaistico = false; // Utilizzo di numero la posto di Kg, tare imballaggi non obbligatoria
var enum_FF_gest_materiale_vivaistico = 848;
var TRASFORMATI_VEGETALI = 210;
var TRASFORMATI_ANIMALI = 310;


var trasferimentoResx = [];
var arrPathResx = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "GestioneMagazzini/App_LocalResources/Trasferimento.aspx.resx"
];

//DOCUMENT READY
$(document).ready(function () {
    
    $.logThis("DocReady: INIZIO");

    // Converto i moduli attivi in integer
    for (let i = 0; i < modulo_anagrafe_log.length; i++) {
        modulo_anagrafe_log[i] = parseInt(modulo_anagrafe_log[i]);
    }

    modalita_trasferimento = true;

    if (Array.isArray(arrPathResx) && arrPathResx.length > 0) {
        arrPathResx.forEach(function (singlePathResx) {
            let resxObj = readResxFile(singlePathResx, "Trasferimento");
            trasferimentoResx.unshift(resxObj);
        });
    }

    // Cerco se siamo in gestione materiale vivaistico
    let imp_848 = RicercaUtenti_Impostazioni(enum_FF_gest_materiale_vivaistico, 2);
    FF_gest_materiale_vivaistico = (imp_848 === "1");

    creaKendoSwitch("chkGiacenzePositive");
    setKendoSwitch("chkGiacenzePositive", true); 

    ////KENDO Tabstrip***********************************************
    $(".kendoTabStrip_GiacenzeMagazzino").kendoTabStrip({
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });
    ////*************************************************************
    var tabstrip_GiacenzeMagazzino = $("#tabstrip_GiacenzeMagazzino").data("kendoTabStrip");
    tabstrip_GiacenzeMagazzino.select(0);

    //Controllo se ha i permessi di lettura e scrittura sulla pagina
    //UtenteAbilitatoLettura = $("input[name$='hf_UtenteAbilitatoLettura']").val() === "True";
    //UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    KendoDate("inDataEmissione").bind("change", inDataEmissione_change);
    KendoDate("txt_DataRif_GiacenzeMagazzino").enable(false);

    if ($(cIdDataOp).val() !== "") {
        set_data("inDataEmissione", formattedDate(JSON.parse($(cIdDataOp).val()), "/"), null);
        set_data("txt_DataRif_GiacenzeMagazzino", formattedDate(JSON.parse($(cIdDataOp).val()), "/"), null);
        KendoDate("inDataEmissione").enable(false);
    } else {
        set_data("inDataEmissione", formattedDate(new Date(), "/"), null);
        set_data("txt_DataRif_GiacenzeMagazzino", formattedDate(new Date(), "/"), null);
    }

    creaKendoDropDownList("cmbDestinazioneDef", { read: LeggiCelle }, "Ubic_Des", "key_Dest");
    
    if ($("input[name$='hf_UtenteAbilitatoGestioneGHG']").val() === "False") {
        $("#groupTrasportoUDM").hide();
        $("#groupTrasportoQTY").hide();
    } else {
        $("#groupTrasportoUDM").show();
        $("#groupTrasportoQTY").show();
    }
    creaKendoDropDownList("ddlUnitaMisuraTrasporto", { read: LeggiUnitaMisuraTrasporto }, "UDM_DES", "UDM_COD");
    $("#ntbDistanzaTrasporto").kendoNumericTextBox({ format: "###,##0.###", min: 0, decimals: 3 });

    if ($(cDistanza_Trasporto_UDM).val() !== "") {
        Set_KendoDDLValue("ddlUnitaMisuraTrasporto", $(cDistanza_Trasporto_UDM).val());
    }

    if ($(cDistanza_Trasporto).val() !== "") {
        Set_KendoNumTBValue("ntbDistanzaTrasporto", $(cDistanza_Trasporto).val());
    }


    $(".areaGiacenzeMagazzino").show();
    $(".areaCarichiScarichi").show();
    
    //eventi di click pulsanti
    $("#btn_ricerca_GiacenzeMagazzino").click(function () {

        let dataDaControllare = $('input[name$="txt_DataRif_GiacenzeMagazzino"]').val();
        let dataValida = true;
        if (dataDaControllare !== "")
            dataValida = isValidDate(dataDaControllare);
        if (!dataValida) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(trasferimentoResx, "DataMovimentoNonValida", "Data movimento non valida"), "DIV_Messaggi");
        }else {
            RicercaGiacenzeMagazzino(indirizzohttp, $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True");
        }
    });
    
    creaKendoDDL_ParametriQualitativi("idCalibro", $(cIdPiva).val(), 1, "ocalibro");
    creaKendoDDL_ParametriQualitativi("idQualita", $(cIdPiva).val(), 3, "oqualità");
    creaKendoDDL_ParametriQualitativi("idCertificazione", $(cIdPiva).val(), 12, "ocertificazioni");
    //creaKendoDDL_ParametriQualitativi("idRugginosita", $(cIdPiva).val(), 22, "orugginosita");
    creaKendoDDL_ParametriQualitativi("idImballaggio", $(cIdPiva).val(), 4, "oimballaggio");
    creaKendoDDL_ParametriQualitativi("idContenitore", $(cIdPiva).val(), 8, "ocontenitore");
    creaKendoDDL_ParametriQualitativi("idConfezione", $(cIdPiva).val(), 5, "oconfezione");
    $("#idCentro").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "sa_nome",
        dataValueField: "sa_cod",
        dataSource: { transport: { read: LeggiCentri } },
        open: kendoDropDownAdjustWidth,
        optionLabel:
        {
            sa_nome: TraduzioneMultiResx(trasferimentoResx, "Tutti", "Tutti"),
            sa_cod: 0
        }
    }).data("kendoDropDownList");

    // TODO Trasferimenti anche trasformati animali
    RicercaProdotti_FF(false, $(cIdPiva).val(), 0, 210, false, false, true);

   //// ImpostaDatiLavorazione();
    //RicercaScarichi(indirizzohttp, "tab_elenco_scarichi", $(cIdPiva).val(), $(cIdAgenda).val());
    //popolaScarichi("tab_elenco_scarichi");
    
    $('a[data-toggle="tab"]').on("shown.bs.tab", function (e) {
        var target = $(e.target).attr("href"); // activated tab
        if (target === "#tabScaricoSuLavorazione")
            popolaScarichi("tab_elenco_scarichi");
        if (target === "#tabCaricoDaLavorazione")
            popolaCarichi("tab_elenco_carichi");
            //popolaCarichi("tab_elenco_carichi", onEditCarichiSuLavorazione);
    });

    var ds = new kendo.data.DataSource({ transport: { read: RiempiSpecie } });
    $('input[name$="ddlSpecie"]').kendoDropDownList({
        filter: "contains",
        dataSource: ds,
        dataTextField: "Veg_Des",
        dataValueField: "Veg_Cod"
    }).bind("change", ddlSpecie_change);

    creaKendoMultiselect("multiselVarieta", { read: RiempiVarieta, data: { Veg_Cod: -1 } }, "Cul_Des", "Cul_Cod");

    // TAB USCITE DA DDT
    //var tabstrip_FiltroVendite = $("#tabstrip_FiltroVendite").kendoTabStrip({ animation: { open: { effects: "fadeIn" } } }).data("kendoTabStrip");
    //tabstrip_FiltroVendite.select(0);
    //// RicercaParametriQualitativi(false, $(cIdPiva).val());
    //creaKendoMultiselect("multiselClienti", { read: RiempiClienti }, "Rag_Soc_Completa", "Cod_Contatto");
    //creaKendoMultiselect("multiselSpecie", { read: RiempiSpecie, data: { Veg_Cod: -1 } }, "Veg_Des", "Veg_Cod", null, null, null, function () {
    //    var multiselVarietaSpecie = KendoMultisel("multiselVarietaSpecie");
    //    multiselVarietaSpecie.autoBind = false;
    //    RiempiVarietaSpecie(multiselVarietaSpecie.dataSource);
    //});
    //creaKendoMultiselect("multiselVarietaSpecie", { read: RiempiVarietaSpecie, data: { Cul_Cod: -1 } }, "Cul_Des", "Cul_Cod", null, null, null, null);
    //creaKendoMultiselectServerFiltering("multiselProdotti", { read: Leggi_Prodotti }, 3, "Prodotto_Des", "Prodotto_Cod");

    //eventi di click pulsanti
    //$("#btn_ricerca_DDTVendita").click(function () {
    //    trovatoErrore = false;

    //    dataDaControllare = $('input[name$="Txt_DataRegDal"]').val();
    //    dataValida = true;
    //    if (dataDaControllare !== "")
    //        dataValida = isValidDate(dataDaControllare);
    //    if (!dataValida) {
    //        MessaggioErrore_Bootstrap(TraduzioneMultiResx(gestioneLavorazioniResx, "DaDataMovimentoNonValida", "Da data movimento non valida"), "DIV_Messaggi");
    //        trovatoErrore = true;
    //    }

    //    dataDaControllare = $('input[name$="Txt_DataRegAl"]').val();
    //    dataValida = true;
    //    if (dataDaControllare !== "")
    //        dataValida = isValidDate(dataDaControllare);
    //    if (!dataValida) {
    //        MessaggioErrore_Bootstrap(TraduzioneMultiResx(gestioneLavorazioniResx, "ADataMovimentoNonValida", "A data movimento non valida"), "DIV_Messaggi");
    //        trovatoErrore = true;
    //    }

    //    if (!trovatoErrore) {
    //        popolaGrigliaRigheVendita("tab_righe_vendita");
    //    }

    //});    //$("#btn_ricerca_DDTVendita").click(function () {
    //    trovatoErrore = false;

    //    dataDaControllare = $('input[name$="Txt_DataRegDal"]').val();
    //    dataValida = true;
    //    if (dataDaControllare !== "")
    //        dataValida = isValidDate(dataDaControllare);
    //    if (!dataValida) {
    //        MessaggioErrore_Bootstrap(TraduzioneMultiResx(gestioneLavorazioniResx, "DaDataMovimentoNonValida", "Da data movimento non valida"), "DIV_Messaggi");
    //        trovatoErrore = true;
    //    }

    //    dataDaControllare = $('input[name$="Txt_DataRegAl"]').val();
    //    dataValida = true;
    //    if (dataDaControllare !== "")
    //        dataValida = isValidDate(dataDaControllare);
    //    if (!dataValida) {
    //        MessaggioErrore_Bootstrap(TraduzioneMultiResx(gestioneLavorazioniResx, "ADataMovimentoNonValida", "A data movimento non valida"), "DIV_Messaggi");
    //        trovatoErrore = true;
    //    }

    //    if (!trovatoErrore) {
    //        popolaGrigliaRigheVendita("tab_righe_vendita");
    //    }

    //});

    $("#dialogSessioneScaduta").on("show.bs.modal", function (event) {
        impostaRedirectStart();
    });

    $("#azioni_LavorazioniFF").show();

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    // $('#a_tabTestataLavorazione').click(function (e) { return tabClick(e, this); });
    $("#a_tabScaricoSuLavorazione").click(function (e) { return tabClick(e, this); });
    $("#a_tabCaricoDaLavorazione").click(function (e) { return tabClick(e, this); });
    //$('#a_tabRiepilogo').click(function (e) { return tabClick(e, this); });

    var idAgenda = getParameterByName("Id_Agenda");
    if (idAgenda !== null && idAgenda !== "") {
        //impostaTestataSolaLettura(true);
        //var data = getDatiLavorazione();
        //if (data.Extra_Int === 1) { //Lavorazione chiusa...
        //    $("#idRowAbilitaModifiche").hide();  // Blocco modifica testata
        //    $("input[name$='hf_UtenteAbilitatoScrittura']").val("False");  // Blocco modifica righe
        //}
        CalcolaKgIngressiUscite();
    } else {
        //impostaTestataSolaLettura(false);
    }

    //carico la tab attiva (lo faccio sempre, tanto ho simulato la lettura lato server anche quando sono in creazione)
    popolaScarichi("tab_elenco_scarichi");
    popolaCarichi("tab_elenco_carichi");

    $.logThis("DocReady: FINE");

});
