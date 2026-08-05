

$(document).ready(function () {

    if (!Array.isArray(datiMeteoResx)) {
        datiMeteoResx = [];
    }
    datiMeteoResx.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "AnalisiRilievi_jQueryDocReady.js"));

    if ($(".AgronicaFooter").is(":visible")) {
        let ftr_h = $(".AgronicaFooter").outerHeight();
        ftr_h = Math.ceil(ftr_h / 10) * 10;
        $("#id_MainContainer").css("margin-bottom", ftr_h + "px");
    }

    WaitFrame.show();

    GlobalMeteoTabstrip = new MeteoTabstrip("#tabstrip");

    $(".kendoCalendar").kendoDatePicker({
        footer: false //"#: kendo.toString(data, 'd') #"
    });

    let oggi = new Date();
    oggi.setHours(0, 0, 0, 0);
    let gen1 = new Date(oggi);
    gen1.setMonth(0);
    gen1.setDate(1);
    $("#txt_DataDa").data("kendoDatePicker").value(gen1);
    $("#txt_DataA").data("kendoDatePicker").value(oggi);

    $("#cmbParametroAnalisi").kendoDropDownList({
        autoBind: true,
        dataTextField: "lav_des",
        dataValueField: "lav_cod",
        dataSource: ParAnalisi_Inizializza()
    });


    $("#btn_analizza").click(function () {
        $("#divKendoOut").data("meteoOutput").clear();
        GlobalMeteoTabstrip.showTabDati(false);
        analizzaRilievi();
    });


    if (!filtroneImpostato) {
        $("#FiltraImpiantiRipulisci").addClass(GIAS_K_STATE_DISABLED);
    }

    $("#FiltraImpiantiRipulisci").click(function () {
        resetFiltro();
        $("#FiltraImpiantiRipulisci").addClass(GIAS_K_STATE_DISABLED);
    });

    $("#FiltraImpianti").click(function () {
        if (usaFiltroRicercaNG) {
            FiltraImpiantiConFiltroRicercaNG();
        } else {
            $("#" + id_Btn_FiltraImpianti).click();
        }

        $("#FiltraImpiantiRipulisci").removeClass(GIAS_K_STATE_DISABLED);
    });

    GlobalMeteoTabstrip.ready();

    WaitFrame.hide();

    $("#divKendoOut").meteoOutput();
});
