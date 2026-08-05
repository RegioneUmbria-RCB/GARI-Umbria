var specievegetali;
var dati_storici;
var modify = false;
var dataFiltroValidita = new Date();

$(document).ready(function () {
    $.logThis("DocReady: INIZIO");

    creaKendoDropDownList("ddlSpecie", { read: Leggi_Specie }, "veg_des", "veg_cod");
    creaKendoDropDownList("ddlYear", { read: Leggi_Anni }, "year_cod", "year_cod");
    creaKendoDropDownList("dllGruppoConsegna", { read: Leggi_GruppiConsegna }, "desc", "id");

    KendoDDL("ddlSpecie").bind("change", onChange_Specie);
    KendoDDL("ddlYear").bind("change", onChange_Year);
    KendoDDL("dllGruppoConsegna").bind("change", onChange_GruppoConsegna);

    SetModifyState(false);

    KendoDDL("ddlYear").value(dataFiltroValidita.getFullYear());

    $.logThis("DocReady: FINE");

});