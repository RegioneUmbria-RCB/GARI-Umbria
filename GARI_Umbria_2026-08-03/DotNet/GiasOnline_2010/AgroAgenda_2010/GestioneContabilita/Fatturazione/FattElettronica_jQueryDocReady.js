
//DOCUMENT READY
$(document).ready(function () {
     
    $.logThis("DocReady: INIZIO");

    // $(".kendoSwitch").kendoMobileSwitch({ onLabel: "SI", offLabel: "NO" });
    
    //Controllo se ha i permessi di lettura e scrittura sulla pagina
    //UtenteAbilitatoLettura = $("input[name$='hf_UtenteAbilitatoLettura']").val() === "True";
    //UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    $(".searchArea").show();
    $(".elencoFattElettronica").hide();

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    var today = new Date();
    var month = today.getMonth();
    var year = today.getFullYear();

    set_data("txt_DataDal", formattedDate(new Date(year, month, 1), "/"), null);
    set_data("txt_DataAl", formattedDate(today, "/"), null);

    //eventi di click pulsanti
    $("#btn_ricerca").click(function () {
        popolaGrigliaFattElettronica();
    });

    wnd = $("#details")
        .kendoWindow({
            title: "Fatturazione Elettronica",
            modal: true,
            visible: false,
            resizable: true,
            height: "80%",
            width: "80%"
        }).data("kendoWindow");

    detailsTemplate = kendo.template($("#template").html());

    $("#dialogSessioneScaduta").on("show.bs.modal", function (event) {
        impostaRedirectStart();
    });

    popolaComboImprese();

    $.logThis("DocReady: FINE");

});