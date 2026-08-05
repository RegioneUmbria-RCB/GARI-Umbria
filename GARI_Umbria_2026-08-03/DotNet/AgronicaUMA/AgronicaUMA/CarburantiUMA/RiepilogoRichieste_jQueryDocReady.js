
//DOCUMENT READY
$(document).ready(function () {
    docR();
});

async function docR() {
    //if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
    //    // Carico i files resx per le traduzioni
    //    resxArrPath.forEach(function (resxSinglePath) {
    //        gestioneCarbResx.push(readResxFile(resxSinglePath, "gestionecosti_jQueryDocReady.js"));
    //    });
    //}
    await ddlAzienda_Load();
    await StatiPratiche_Load();
    await conto_create();
    KendoDDL("conto").value("2");
    await Prov_Load();
    KendoDDL("prov").value("-1");
    await Citta_Load();
    KendoDDL("citta").value("-1");
    var Cmb_Pratiche = KendoDDL("statoPratica")
    Cmb_Pratiche.value("-1");

    var Cmb_Imprese = KendoDDL("ddlAzienda")
    if (cIdPiva === "") {
        Cmb_Imprese.value("-1");
        ddlAzienda_Change();
    }

    if (QS_Piva !== "") {
        Cmb_Imprese.value(QS_Piva);
        ddlAzienda_Change();
    }

    $('#anno')[0].defaultValue = getYear();
    checkPerc($("#legendaPerc"));
    
    
    (function ($, kendo) {
        $.extend(true, kendo.ui.validator, {
            rules: { // custom rules
                timevalidation: function (input, params) {

                    if ($(input).data("kendoTimePicker") !== undefined &&
                        $(input).data("kendoTimePicker").value() === null)
                        return false;

                    return true;
                },
            },
            messages: {
                timevalidation: function (input) {
                    return (TraduzioneMultiResx(gestioneCarbResx, "OraNonValida", "Ora non valida"));
                },
            }
        });
    })(jQuery, kendo);


    $.logThis("DocReady: INIZIO");

    /*$(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });*/

    tabstrip = $("#tabstrip_elenco").kendoTabStrip({
        animation: false,
        //select: onSelect,
        activate: onActivate
        //show: onShow,
    }).data("kendoTabStrip");

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    $("#tabstrip_elenco").show();

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    function onActivate(e) {
        
    }

    //inizializzazione della pagina la prima volta che viene caricata

    $(".preArea").show();
    $(".testataArea").show();
    $(".dettagliArea").show();

    $(".elenco")[0].children[1].innerHTML = "RIEPILOGO RICHIESTE E RENDICONTAZIONI"

   //INIZIO AGGIUNTO DA GLORIA PER APERTURA DEL PULSANTE SINTESI E RNDICONTAZIONE DALLA PAGINA DI NUOVA RICHIESTA 
    if (QS_PagArrivo == "1") {   
        $("#ddlAzienda").data("kendoDropDownList").enable(false);
        $("#citta").data("kendoDropDownList").enable(false);
        $("#prov").data("kendoDropDownList").enable(false);
    }

    //FINE AGGIUNTO DA GLORIA

    $.logThis("DocReady: FINE");
}