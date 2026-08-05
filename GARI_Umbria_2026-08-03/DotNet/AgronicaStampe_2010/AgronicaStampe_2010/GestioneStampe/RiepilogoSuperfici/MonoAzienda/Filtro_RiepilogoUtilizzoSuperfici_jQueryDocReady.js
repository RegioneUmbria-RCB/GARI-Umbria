$(document).ready(function () {

    // ----------- Inizializzo i controlli -----------------------------

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31),
        format: "dd/MM/yyyy"
    });

    var today = new Date();
    today.setHours(0, 0, 0);
    set_data("dpDataRif", today, null);

    var kddlImprese = creaKendoDropDownList("ddlImprese", { read: ddlImpreseRead }, "rag_soc", "piva", null, null, null, false, null, null, false).data("kendoDropDownList");
    kddlImprese.bind("change", ddlImpreseChange);
    kddlImprese.one("dataBound", ddlImpreseDataBound);

    creaKendoDropDownList("ddlCentriAziendali", { read: ddlCentriAziendaliRead }, "sa_nome", "sa_cod", null, null, null, false);

    $("#btnStampa").on("click", StampaRiepilogoUtilizzoSuperficiMono);

    kddlImprese.dataSource.read();

    // ----------- Fine Inizializzo i controlli -----------------------------
});