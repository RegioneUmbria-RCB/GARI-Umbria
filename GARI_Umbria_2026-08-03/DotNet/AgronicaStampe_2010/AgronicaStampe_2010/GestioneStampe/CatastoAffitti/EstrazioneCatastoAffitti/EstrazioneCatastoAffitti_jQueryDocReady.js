
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    //Inizializza la multisel da query
    creaKendoMultiselect("multiselCentroAziendale", { read: RiempiCentri, data: { sa_cod: -1 } }, "sa_nome", "sa_cod", null, null, null, null);

    //Eventi di click pulsanti
    $("#btn_ricerca").click(function () {

        dataDaControllare = $('input[name$="txt_DataDal"]').val();
        dataValida = true;

        //Controlla che i valori inseriti per le date siano validi
        if (dataDaControllare !== "")
            dataValida = isValidDate(dataDaControllare);

        if (!dataValida)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(ricercaTrasferimentiResx, "DataDalNonValida", "Data dal non valida"), "DIV_Messaggi");
        else {
            dataDal = dataDaControllare;
            dataDaControllare = $('input[name$="txt_DataAl"]').val();
            dataValida = true;

            if (dataDaControllare !== "")
                dataValida = isValidDate(dataDaControllare);

            if (!dataValida)
                MessaggioErrore_Bootstrap(TraduzioneMultiResx(ricercaTrasferimentiResx, "DataAlNonValida", "Data al non valida"), "DIV_Messaggi");
            else {
                dataAl = dataDaControllare;

                //Popola la griglia in base al periodo inserito
                popolaGrigliaCatastoAffitti("gridCatAff");
            }
        }
    });

    //Setta il campo BioVincolo a checkbox
    $("#grid .k-grid-content").on("change", "input.chkbx", function (e) {
        var grid = $("#grid").data("kendoGrid"),
            dataItem = grid.dataItem($(e.target).closest("tr"));

        dataItem.set("BioVincolo", this.checked);
    });

    $.logThis("DocReady: FINE");

})