
//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    var dataDa = formattedDate($(data_Da).val(), '/');
    var dataA = formattedDate($(data_A).val(), '/');
    var pi = $(piva).val();

    $('input[name$="DaData"]').val(dataDa);
    $('input[name$="AData"]').val(dataA);

    creaKendoSwitch();

    //KENDO DatePicker***************************************
    //$(".kendoCalendar").kendoDatePicker({ footer: false });//Non mostra il footer
    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)//,  Larghezza calendario come il campo di input...
        //        open: function () {
        //            var calendar = this.dateView.calendar;
        //            calendar.wrapper.width(this.wrapper.width() - 6);
        //        }
    });
    //*******************************************************

    creaKendoDropDownListWithData("idTipoReport", [
        { text: "Garanzie di Circolazione", value: "0" },
        { text: "Registro Singole Partite", value: "1"}]
    );
    Set_KendoDDLValue("idLivelloRottura", "Campo");

    //Controllo se ha i permessi di lettura e scrittura sulla pagina
    UtenteAbilitatoLettura = $("input[name$='hf_UtenteAbilitatoLettura']").val() === "True";

    //eventi di click pulsanti
    $("#btn_aggiorna_riepilogo").click(function () {
        popolaGrigliaRiepilogoGaranzie("tab_riepilogo_garanzie");
    });

    popolaGrigliaRiepilogoGaranzie("tab_riepilogo_garanzie");
    $(".modifyArea").show();

    $(".searchArea").show();

    $("#btn_report").click(function () {

        var campiValidi = controllaCampiObbligatori();
        if (campiValidi) {
            var dateValide = controllaDateLancio();

            if (dateValide)
                Report_DAA();
        }
    });

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $.logThis("DocReady: FINE");

});

