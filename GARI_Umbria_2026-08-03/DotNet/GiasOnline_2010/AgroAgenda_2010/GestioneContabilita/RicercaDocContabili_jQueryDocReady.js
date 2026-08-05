
//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj.push(readResxFile(resxSinglePath, "RicercaDocContabili_jQueryDocReady.js"));
        });
    }

    // Inizializzo template comuni di pulsanti della pagina:
    ValorizzaStringheTraduzioni();

    // Evita l'utilizzo dell'invio
    $(window).keydown(function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            return false;
        }
    });

    // Converto i moduli attivi in integer
    for (let i = 0; i < modulo_anagrafe_log.length; i++) {
        modulo_anagrafe_log[i] = parseInt(modulo_anagrafe_log[i]);
    }

    $("#TipoOutput").kendoButtonGroup({
        index: 0,
        selection: "single",
        select: CampiReportPDF
    });

    tabstrip = $("#tabstrip_Filtri").kendoTabStrip({
        animation: false,
        //select: onSelect,
        activate: onActivate
        //show: onShow,
    }).data("kendoTabStrip");

    $($("#tabstrip_Filtri").data("kendoTabStrip").items()[2]).attr("style", "display:none");
    $("#tabFiltroProdotti").css("display", "none");

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });
    var startYearDate = formattedDate(new Date(new Date().getFullYear(), 0, 1), "/");
    set_data("Txt_DataRegDal", startYearDate, null);

    $(".searchArea").show();
    
    //KENDOWINDOW per apertura in popup pagina campionamento
    $("#campionamentoWindow").kendoWindow({
        actions: ["Close"],
        visible: false,
        draggable: false,
        height: "85%",
        width: "90%",
        modal: true,
        resizable: false,
        title: TraduzioneMultiResx(resxObj, "CampionamentoManuale", "Campionamento manuale"),
        iframe: true,
        open:
            function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                $("body").addClass("ob-no-scroll");
            },
        close: function (e) {
            $("body").removeClass("ob-no-scroll");
        }
    });

    if (IsFatturazione() == false) {
        $("#btn_esegui_Fatturazione").hide();
    }

    //eventi di click pulsanti
    $("#btn_ricerca").click(function () {
        Esegui_Report(false);
    });

    $("#btn_prepara_nuovo_doc").click(function () {
        Prepara_Nuovo_Documento ();
    });

    $("#btn_nuovo").click(function () {
        nuovo_documento();
    });

    $("#btn_pulisci_filtri").click(function () {
        pulisci_filtri();
    });

    $("#btn_esegui_Fatturazione").click(function () {
        esegui_Fatturazione();
    });



    // i18n Pulsante non esistente, dovuto a copia da pagina statistiche
    $("#btn_aggiorna").click(function () {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: "Report Vendite",
            messages: { okText: "Sì", cancel: "No" },
            content: "Vuoi aggiornare i dati del report vendite per il periodo selezionato?<br><br>L'operazione potrebbe richiedere diverso tempo"
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () { AggiornaCuboReportVendite(); });
        kendoConfirm.open();        
    });

    $("#btn_avvia_fatturazione").click(function () {
        schedula_Fatturazione();
    });

    Elenco_Parametri_Qualitativi = Leggi_Parametri_Qualitativi();
    Elenco_Specie = Leggi_Specie();
    //Elenco_Centri_Aziendali = LeggiCentri();
    //Inizializza_Combo_Centri_Aziendali();

    creaKendoMultiselect("multiselCentroAziendale", { read: RiempiCentri, data: { sa_cod: -1 } }, "sa_nome", "sa_cod", null, null, null, null);
    creaKendoMultiselect("multiselSpecie", { read: RiempiSpecie, data: { Veg_Cod: -1 } }, "veg_des", "veg_cod", null, null, null, SpecieChange);
    creaKendoMultiselect("multiselVarieta", { read: RiempiVarieta, data: { Cul_Cod: -1 } }, "Cul_Des", "Cul_Cod", null, null, null, null);

    //creaKendoMultiselect("multiselClienti", { read: RiempiClienti }, "nome", "cod_contatto");
    creaKendoMultiselect("multiselAgenti", { read: RiempiAgenti }, "nome", "cod_contatto");
    creaKendoMultiselect("multiselCausale", { read: RiempiCausali }, "LAV_DES", "LAV_COD");
    creaKendoMultiselect("multiselGruppoDocumento", { read: RiempiGruppiDocumento }, "GRP_DES", "LAV_COD");
    var multiselGruppiDoc = $("#id_multiselGruppoDocumento").data("kendoMultiSelect");
    multiselGruppiDoc.bind("select", multiselectGruppoDocumento_select);
    multiselGruppiDoc.bind("deselect", multiselectGruppoDocumento_deselect);
    creaKendoMultiselect("multiselCategorie", { read: RiempiCategorie }, "Elem_Des", "Elem_Cod");
    creaKendoMultiselect("multiselCategCommle", { read: RiempiCategorieCommerciali }, "Linea_Classe_Des", "Linea_Classe_Cod");
    creaKendoMultiselectServerFiltering("multiselProdotti", { read: RicercaProdottiCompleto }, 3, "Prodotto_Des", "Prodotto_Cod");
    KendoMultisel("multiselProdotti").bind("filtering", multiselProdotti_filtering);
    creaKendoMultiselectServerFiltering("multiselContatti", { read: FiltraContatti }, 3, "nome", "cod_contatto");
    creaKendoSwitch("cb_ddt_non_fatturati", undefined, undefined, false);
    creaKendoSwitch("cb_ordini_non_Spediti", undefined, undefined, false);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    function onActivate(e) {
        var selectedIndex = $(e.item).index();
    }

    Inizializza_DropDown_NuoviOrdini("#ddl_nuovoOrdine");
    Inizializza_DropDown_Nuovi_Documenti_Acquisto("#ddl_nuovoDocAcquisto");
    Inizializza_DropDown_Nuovi_Documenti_Vendita("#ddl_nuovoDocVendita");
    Abilita_Nuovo_Documento(0);
    ImpostaVisibilitaFiltriSwitch();

    Leggi_Filtri();
    Esegui_Report(true);

    $('body').kendoTooltip({
        filter: '.btn[title]'
    })

    $.logThis("DocReady: FINE");


});


function ValorizzaStringheTraduzioni() {

    templateStampaDocumento = "<span class='fa fa-print fa-2x print_elem' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxObj, "StampaDocumento", "Stampa Documento") + "' onclick=StampaDocumento(this.closest('tr'),this.closest('.k-grid'))></span>";

    templateModificaDocumento = "<span class='fa fa-pencil-square-o fa-2x edit_elem' title='" + TraduzioneMultiResx(resxObj, "ModificaDocumento", "Modifica Documento") + "' onclick=ApriModificaDocumento(this.closest('tr'),this.closest('.k-grid'),2)></span>";

    templateEliminaDocumento = "<span class='fa fa-trash-o fa-2x del_elem' title='" + TraduzioneMultiResx(resxObj, "EliminaDocumento", "Elimina Documento") + "' onclick=EliminaDocumento(this.closest('tr'),this.closest('.k-grid'))></span>";

    templateStampaEtichetteDettaglio = "<span class='fa fa-barcode fa-2x print_elem' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxObj, "StampaEtichette", "Stampa Etichette") + "' onclick=StampaBarCode(this.closest('tr'),this.closest('.k-grid'),true)></span>";

    templateStampaEtichetteTestata = "<span class='fa fa-barcode fa-2x print_elem' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxObj, "StampaEtichette", "Stampa Etichette") + "' onclick=StampaBarCode(this.closest('tr'),this.closest('.k-grid'),false)></span>";

    templateVisualizzaDocumento = "<span class='fa fa-info fa-2x info_elem' title='" + TraduzioneMultiResx(resxObj, "VisualizzaDocumento", "Visualizza Documento") + "' onclick=ApriModificaDocumento(this.closest('tr'),this.closest('.k-grid'),0)></span>";

    templateSbloccaDocumento = "<span class='fa fa-unlock fa-2x sblocca_elem' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxObj, "SbloccaDocumento", "Sblocca Documento") + "' onclick=SbloccaDocumento(this.closest('tr'),this.closest('.k-grid'))></span>";

    templateBloccaDocumento = "<span class='fa fa-lock fa-2x blocca_elem' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxObj, "BloccaDocumento", "Blocca Documento") + "' onclick=BloccaDocumento(this.closest('tr'),this.closest('.k-grid'))></span>";

    templateCampionamento = "<span class='fa fa-check-square-o fa-2x' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxObj, "Campionamento", "Campionamento") + "' onclick=ApriCampionamento(this.closest('tr'),this.closest('.k-grid'))></span>";

    templateNuovoAllegato = "<span class='fa fa-paperclip fa-2x info_elem' title='" + TraduzioneMultiResx(resxObj, "NuovoAllegato", "Nuovo Allegato") + "'  onclick=ApriKendoWindowAggiungiNuovoAllegato(this.closest('tr'),this.closest('.k-grid')) ></span>";

    templateGestioneAllegati = "<span class='fa fa-file-text-o fa-2x info_elem' title='" + TraduzioneMultiResx(resxObj, "GestioneAllegati", "Gestione Allegati") + "' onclick=ApriKendoWindowRicercaDocumenti(this.closest('tr'),this.closest('.k-grid'))></span>";


    elencoCausali = [
        { "TYPE": "A", "DOC_TYPE": "F", "LAV_COD": "1000", "LAV_DES": TraduzioneMultiResx(resxObj, "FatturaRicevuta", "Fattura Ricevuta") },
        { "TYPE": "A", "DOC_TYPE": "F", "LAV_COD": "1002", "LAV_DES": TraduzioneMultiResx(resxObj, "NotaAccreditoRicevuta", "Nota Accredito Ricevuta") },
        { "TYPE": "V", "DOC_TYPE": "F", "LAV_COD": "1001", "LAV_DES": TraduzioneMultiResx(resxObj, "FatturaEmessa", "Fattura Emessa") },
        { "TYPE": "V", "DOC_TYPE": "F", "LAV_COD": "1003", "LAV_DES": TraduzioneMultiResx(resxObj, "NotaAccreditoEmessa", "Nota Accredito Emessa") },
        { "TYPE": "A", "DOC_TYPE": "C", "LAV_COD": "1025", "LAV_DES": TraduzioneMultiResx(resxObj, "DDTRicevuto", "DDT Ricevuto") },
        { "TYPE": "V", "DOC_TYPE": "C", "LAV_COD": "1031", "LAV_DES": TraduzioneMultiResx(resxObj, "DDTEmesso", "DDT Emesso") },
        { "TYPE": "V", "DOC_TYPE": "C", "LAV_COD": "1069", "LAV_DES": TraduzioneMultiResx(resxObj, "DDTContabilizzatoEmesso", "DDT Contabilizzato Emesso") },
        { "TYPE": "C", "DOC_TYPE": "C", "LAV_COD": "1054", "LAV_DES": TraduzioneMultiResx(resxObj, "AccettazioneDDTRicevuto", "Accettazione DDT Ricevuto") },
        { "TYPE": "C", "DOC_TYPE": "P", "LAV_COD": "1054", "LAV_DES": TraduzioneMultiResx(resxObj, "AccettazioneDDTRicevuto", "Accettazione DDT Ricevuto") },
        { "TYPE": "A", "DOC_TYPE": "C", "LAV_COD": "1075", "LAV_DES": TraduzioneMultiResx(resxObj, "DistintaDiCarico", "Distinta di Carico") },
        { "TYPE": "C", "DOC_TYPE": "C", "LAV_COD": "1076", "LAV_DES": TraduzioneMultiResx(resxObj, "AccettazioneDistintaDiCarico", "Accettazione Distinta di Carico") },
        { "TYPE": "A", "DOC_TYPE": "C", "LAV_COD": "1077", "LAV_DES": TraduzioneMultiResx(resxObj, "AutoDDTEmesso", "Auto DDT Emesso") },
        { "TYPE": "C", "DOC_TYPE": "C", "LAV_COD": "1078", "LAV_DES": TraduzioneMultiResx(resxObj, "AccettazioneAutoDDTEmesso", "Accettazione Auto DDT Emesso") },
        { "TYPE": "V", "DOC_TYPE": "O", "LAV_COD": "2002", "LAV_DES": TraduzioneMultiResx(resxObj, "OrdineDiVendita", "Ordine di Vendita") },
        { "TYPE": "A", "DOC_TYPE": "O", "LAV_COD": "2004", "LAV_DES": TraduzioneMultiResx(resxObj, "OrdineDiAcquisto", "Ordine di Acquisto") },
        { "TYPE": "CO", "DOC_TYPE": "AF", "LAV_COD": "2006", "LAV_DES": TraduzioneMultiResx(resxObj, "ContrattoDiAffitto", "Contratto di Affitto") }
    ];

}