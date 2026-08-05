////var lat;
////var lng;

var dataFiltroValidita = new Date();
var modify = false;

$(document).ready(function () {
    $.logThis("DocReady: INIZIO");

    creaKendoDropDownList("ddlSpecie", { read: Leggi_Specie }, "veg_des", "veg_cod");
    /*creaKendoDropDownList("ddlYear", { read: Leggi_Anni }, "year_cod", "year_cod");*/

    KendoDDL("ddlSpecie").bind("change", onChange_Specie);
    //KendoDDL("ddlYear").bind("change", onChange_Year);
    //KendoDDL("ddlYear").value(dataFiltroValidita.getFullYear());
    //KendoDDL("ddlYear").trigger("change");

    $("#btnAddData").hide();
    $("#btnSaveData").hide();
    $("#divCoeffSpecie").hide();

    PopolaGrigliaParametriGenerali();

    SetModifyState(false);

    $("#tabstrip").kendoTabStrip({
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $.logThis("DocReady: FINE");
});