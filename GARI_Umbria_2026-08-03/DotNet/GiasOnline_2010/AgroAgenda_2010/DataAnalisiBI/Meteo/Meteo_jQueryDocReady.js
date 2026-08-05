

$(document).ready(function () {

    if (!Array.isArray(datiMeteoResx)) {
        datiMeteoResx = [];
    }
    datiMeteoResx.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "Meteo_jQueryDocReady.js"));

    if ($(".AgronicaFooter").is(":visible")) {
        let ftr_h = $(".AgronicaFooter").outerHeight();
        ftr_h = Math.ceil(ftr_h / 10) * 10;
        $("#id_MainContainer").css("margin-bottom", ftr_h + "px");
    }

    WaitFrame.show();

    GlobalMeteoTabstrip = new MeteoTabstrip("#tabstrip");

    GlobalMeteoSourceSelector = new MeteoSourceSelector($("#geo-pos-edit"), $("#cmbTipoSorgente"), $("#cmbOrigineDati"), url_meteo_ws);

    let lat = Request_QueryString("lat");
    let lng = Request_QueryString("lng");

    GlobalMeteoSourceSelector.setLatLng(lat, lng);

    $(".kendoCalendar").kendoDatePicker({
        footer: false //"#: kendo.toString(data, 'd') #"
    });

    let today = new Date();
    today.setHours(0, 0, 0, 0);
    //Primo giorno del mese corrente
    let month1 = new Date(today.valueOf());
    month1.setDate(1);

    let diffDays = Math.round((today.getTime() - month1.getTime()) / (1000 * 3600 * 24));
    if (diffDays < 3) {
        //Primo giorno del mese precedente
        month1.setMonth(month1.getMonth() - 1);
    }

    $("#txt_DataDa").data("kendoDatePicker").value(month1);
    $("#txt_DataA").data("kendoDatePicker").value(today);

    let yy = today.getFullYear();
    $("#serie_storiche").kendoMultiSelect({
        dataSource: Array.from({ length: 10 }, () => --yy)
    });

    let chkbox = document.getElementById("switch-freq-dati");
    chkbox.checked = true;

    $(chkbox).on('change', function () {
        let value = this.checked;

        let $lbl = $("#lbl_serie_storiche");
        let multisel = $("#serie_storiche").getKendoMultiSelect();

        if (value) {

            $lbl.addClass("lbl-disabled");

            multisel.value([]);
        } else {

            $lbl.removeClass("lbl-disabled");
        }

        multisel.enable(!value);

    });

    $(chkbox).trigger("change");

    $("#txt_Soglia_Germinazione").kendoNumericTextBox({
        decimals: 1,
        restrictDecimals: true,
        format: "#.0 °C",
        spinners: false,
        min: 0,
        max: 30,
        value: 6,
        step: 1,
        change: function () {
            if (this.value() === null) {
                this.value(this.min());
            }
        }
    }).data("kendoNumericTextBox");

    $("#txt_Soglia_FabbisognoFreddo").kendoNumericTextBox({
        decimals: 1,
        restrictDecimals: true,
        format: "#.0 °C",
        spinners: false,
        min: 0,
        max: 30,
        value: 7,
        step: 1,
        change: function () {
            if (this.value() === null) {
                this.value(this.min());
            }
        }
    }).data("kendoNumericTextBox");

    $("#btn_ricerca_meteo").click(function () {
        analisiMeteo();
    });

    GlobalMeteoTabstrip.ready();

    WaitFrame.hide();

    $("#divKendoOut").meteoOutput();

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //Impostazioni default
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    let dati = getMeteoStorage();

    if (dati) {

        if (dati.DataInizio !== undefined) {
            let dt = new Date(dati.DataInizio)
            $("#txt_DataDa").data("kendoDatePicker").value(dt);
        }

        if (dati.DataFine !== undefined) {
            let dt = new Date(dati.DataFine)
            $("#txt_DataA").data("kendoDatePicker").value(dt);
        }

        GlobalMeteoSourceSelector.setSorgente(dati.TipoSorgente, dati.Sorgente);

    } else {

        GlobalMeteoSourceSelector.setDefault($(cIdPiva).val());
    }

    if (lat !== null && lat !== undefined && lng !== null && lng !== undefind) {
        GlobalMeteoSourceSelector.setSorgente(1);
    }

    $(".info-forecast").kendoPopover({
        position: "top",
        showOn: "mouseenter",
        body: function (e) {
            return "<div>Impostando una data futura (max +7 gg) si ottengono proiezioni che includono i dati meteo previsionali per i sensori disponibili.</div>";
        }
    });

});

