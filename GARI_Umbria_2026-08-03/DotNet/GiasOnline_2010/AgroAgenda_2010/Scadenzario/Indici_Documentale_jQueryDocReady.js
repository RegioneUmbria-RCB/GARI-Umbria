
//DOCUMENT READY
$(document).ready(function () {

    // Evita l'utilizzo dell'invio
    // TODO Stefano
    //$(window).keydown(function (event) {
    //    if (event.keyCode == 13) {
    //        event.preventDefault();
    //        return false;
    //    }
    //});

    //    $('#aspnetForm').change(function () {
    //        controlla_form();
    //    });

    $.logThis("DocReady: INIZIO");

    //inizializzazione della pagina la prima volta che viene caricata

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj .unshift(readResxFile(resxSinglePath, "Indici_Documentale_jQueryDocReady.js"));
        });
    }

    Elenco_TipoCampo = [
        { "TipoCampo": 0, "TipoCampo_Des": TraduzioneMultiResx(resxObj , "LiberaImputazione", "Libera Imputazione") },
        { "TipoCampo": 1, "TipoCampo_Des": TraduzioneMultiResx(resxObj , "SceltaValori", "Scelta Valori") },
        { "TipoCampo": 2, "TipoCampo_Des": TraduzioneMultiResx(resxObj , "SceltaElenco", "Scelta Elenco") }
    ];

    Elenco_Libero = [
        { "Libero_Cod": "string", "Libero_Des": TraduzioneMultiResx(resxObj , "Carattere", "Carattere") },
        { "Libero_Cod": "numeric", "Libero_Des": TraduzioneMultiResx(resxObj , "Numerico", "Numerico") },
        { "Libero_Cod": "date", "Libero_Des": TraduzioneMultiResx(resxObj , "Data", "Data") },
        { "Libero_Cod": "boolean", "Libero_Des": TraduzioneMultiResx(resxObj , "Booleano", "Booleano") }
    ];

    $(".preArea").show();
    

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    //Elenco_Aree = Elenco_Aree_Riempi(true);   
    Elenco_Tipo = Elenco_Tipo_Riempi(false);   

   

    //creaKendoDropDownList("cmbArea", { read: RiempicmbArea }, "Nome", "ID_Area").bind("change", CmbArea_change);
    //creaKendoMultiselect("cmbTipologia", { read: RiempicmbTipologia }, "Nome", "ID_Tipologia");
    creaKendoDropDownList("cmbTipo", { read: RiempicmbTipoCampo }, "TipoCampo_Des", "TipoCampo").bind("change", CmbTipo_change);
    creaKendoDropDownList("cmbElenco", { read: RiempicmbElenco }, "Elenco_Des", "Elenco_Key").bind("change", CmbElenco_change);
    creaKendoDropDownList("cmbElenco_Valore", { read: RiempicmbElenco_Valore }, "Elenco_Valore_Des", "Elenco_Valore_Cod");
    creaKendoDropDownList("cmbLibero", { read: RiempicmbLibero }, "Libero_Des", "Libero_Cod").bind("change", CmbLibero_change);
        

    //var multiselect = $("#cmbTipologia").data("kendoMultiSelect");
    //multiselect.bind("deselect", DeselezionaTipologia);

    //if (parseInt($(cId_Indice).val()) !== 0) {
    //    Elenco_Tipologie_Bloccate = Elenco_Tipologie_Bloccate_Riempi($(cId_Indice).val());

    //    if (Elenco_Tipologie_Bloccate !== "") {
    //        //Area bloccata poichè già imputata
    //        KendoDDL("cmbArea").enable(false);
    //    }

    //}


    creaKendoSwitch("ChkSpeciale", TraduzioneMultiResx(resxObj , "Si", "Sì"), TraduzioneMultiResx(resxObj , "No", "No"), false, function (e) {
        //if (e.checked) {

        //}
        //else {

        //}
    });


    creaKendoSwitch("ChkObbligatorio", TraduzioneMultiResx(resxObj , "Si", "Sì"), TraduzioneMultiResx(resxObj , "No", "No"), false, function (e) {
        //if (e.checked) {
           
        //}
        //else {
           
        //}
    });

    creaKendoSwitch("ChkRiservato", TraduzioneMultiResx(resxObj , "Si", "Sì"), TraduzioneMultiResx(resxObj , "No", "No"), false, function (e) {
        //if (e.checked) {

        //    //if (parseInt($(cRiservato).val()) == 1) $("#chkSpeciale").show();
        //    //else $("#chkSpeciale").hide();              
        //}
        //else {
        //    //$("#chkSpeciale").hide();
        //}
    });
    
     
    //Visbilità Indice Riservato
    if (parseInt($(cRiservato).val()) == 0) {

        $("#lblRiservato").hide();
        setKendoSwitchVisible("ChkRiservato", false);

        $("#lblSpeciale").hide();
        setKendoSwitchVisible("ChkSpeciale", false);


        //$("#ChkRiservato").hide();
        //$("#ChkSpeciale").hide();
        

        //Controllo Modifica di Indice Documentale Riservato, 
        //---> Anna 31 / 05 / 22 oppure utente senza permessi di scrittura
        if (parseInt($(cId_Indice).val()) < 0 || $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "False") {

            //Blocco Controlli
            bBloccaControlli = true;

        }

        if (parseInt($(cId_Indice).val()) == 0) {

            //Default nuovo indice
            setKendoSwitch("ChkRiservato", false);

        }
    }

    else {

        

    }
    
    ConfiguraGrigliaDettagli("tab_griglia_dettagli", false);

    var griglia = $("#tab_griglia_dettagli").data("kendoGrid");

    griglia.autoFitColumn(0);

    

    $("#tab_griglia_dettagli").attr("style", "display:none");
    $("#id_elenco").hide();
    $("#id_elenco_valore").hide();
    $("#id_libero").show();

    Leggi();
    


    //Blocco Controlli
    if (bBloccaControlli) {

        $("#ChkObbligatorio").data("kendoSwitch").enable(false);
        $("#ChkRiservato").data("kendoSwitch").enable(false);
        $("#ChkSpeciale").data("kendoSwitch").enable(false);

        $('input[name$="txt_titolo_indice"]').attr("disabled", true);
       // $('input[name$="txt_Validita_Inizio"]').data("kendoCalendar").enable(false);
       // $('input[name$="txt_Validita_Fine"]').enable(false);

        $("#txt_Validita_Inizio").data("kendoDatePicker").enable(false);
        $("#txt_Validita_Fine").data("kendoDatePicker").enable(false);

        //KendoDDL("cmbArea").enable(false);
        //$("#cmbTipologia").data("kendoMultiSelect").enable(false);

        KendoDDL("cmbTipo").enable(false);
        KendoDDL("cmbElenco").enable(false);
        KendoDDL("cmbElenco_Valore").enable(false);
        KendoDDL("cmbLibero").enable(false);

        $("#btn_salva_esci").text(TraduzioneMultiResx(resxObj , "Esci", "Esci"));
    }


    //fine controlli

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $("#btn_salva_esci").click(
        function () {

            if (bBloccaControlli) {
                window.location.href = $(cPaginaRedirect).val() + "?p=" + $(cPiva_Codificata).val() + "&tab_default=tab_indici";
            }
            else {
                SalvaIndice(true);
            }
        });


    ////eventi di click pulsanti
    //$("#saveChanges").click(
    //    function () {

    //        SalvaIndice(false);            

            
    //    });


    $.logThis("DocReady: FINE");

});
