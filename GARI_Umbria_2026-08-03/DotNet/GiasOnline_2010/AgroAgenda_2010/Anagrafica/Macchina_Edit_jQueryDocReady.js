var macchinaEditResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/Macchina_Edit.aspx.resx"
];


jQuery(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            macchinaEditResx.push(readResxFile(resxSinglePath, "Macchina_Edit_jQueryDocReady.js"));
        });
    }

    var initRevisioni;

    if (jsRevisioni !== "") {
        initRevisioni = jsRevisioni;
    } else {
        initRevisioni = "";
    }

    if (initRevisioni !== "") {
        AggiornaTabRevisioni(initRevisioni);
    }

    var initCosti;

    if (jsCosti !== "") {
        initCosti = jsCosti;
    } else {
        initCosti = "";
    }

    if (initCosti !== "") {
        AggiornaTabCosti(initCosti);
    }

    $("#Cmb_Finalita").bind("change", Finalita_Change);
    $("#Cmb_Finalita").trigger("change");

    if ($(xContatto_Assegnato).val() === "False")
    {
        $("#rowContatto").hide();
    }

    //            $( "[name='ctl00$MainContent$StatoUtilizzo']" ).change(function(){
    //                
    //                if ($('#<=Opt_Attivo.ClientID %>').attr("checked")) {

    //                    $('#<=Opt_Attivo.ClientID %>').attr("checked", false);
    //                    $('#<=Opt_Dismesso.ClientID %>').attr("checked", true);
    //                    $('#<=Txt_DataDismissione.ClientID %>').prop( "disabled", false );
    //                }

    //                if ($('#<=Opt_Dismesso.ClientID %>').attr("checked")) {
    //                    $('#<=Opt_Attivo.ClientID %>').attr("checked", true);
    //                    $('#<=Opt_Dismesso.ClientID %>').attr("checked", false);
    //                    $('#<=Txt_DataDismissione.ClientID %>').attr("disabled", "disabled");
    //                }


    //            });

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    $('.nav-tabs a').on('shown.bs.tab', function (event) {
        var tabId = event.target.hash;
        OnTabShow(tabId);
    });


    //Se siamo nel popup dell'UMA, nascondo il pulsante di salva e continua
    if ($(isPopUpUMA).val() == 'True') $("[name='btn_SalvaContinua']").css("display", "none");

});