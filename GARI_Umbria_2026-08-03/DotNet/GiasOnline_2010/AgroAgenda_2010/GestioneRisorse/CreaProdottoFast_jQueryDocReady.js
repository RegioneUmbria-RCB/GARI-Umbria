//////////////////////////////////////////////////////////
//      jQueryDocReady
//////////////////////////////////////////////////////////


//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    //$(".kendoCalendar").kendoDatePicker({
    //    footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
    //    max: new Date(2100, 11, 31)
    //});

    //$(".searchArea").show();


    ////eventi di click pulsanti
    //$("#btn_ricerca").click(function () {

    //    trovatoErrore = false;

    //    dataDaControllare = $('input[name$="Txt_DataRegDal"]').val();
    //    dataValida = true;
    //    if (dataDaControllare !== "")
    //        dataValida = isValidDate(dataDaControllare);
    //    if (!dataValida) {
    //        MessaggioErrore_Bootstrap("Da data movimento non valida", "DIV_Messaggi");
    //        trovatoErrore = true;
    //    }

    //    dataDaControllare = $('input[name$="Txt_DataRegAl"]').val();
    //    dataValida = true;
    //    if (dataDaControllare !== "")
    //        dataValida = isValidDate(dataDaControllare);
    //    if (!dataValida) {
    //        MessaggioErrore_Bootstrap("A data movimento non valida", "DIV_Messaggi");
    //        trovatoErrore = true;
    //    }



    //    if (!trovatoErrore)
    //        popolaGrigliaMovimentiConferimento("tab_righe_conferimento");
    //});

    
    //creaKendoMultiselect("multiselFornitori", { read: RiempiFornitori }, "Rag_Soc", "Cod_Contatto");

    //creaKendoMultiselect("multiselCausale", { read: RiempiCausali }, "LAV_DES", "LAV_COD");


    //fine controlli


    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });


    ////eventi di click pulsanti
    //$("#btn_avvia").click(function () {

    //    trovatoErrore = false;

    //    dataDaControllare = $('input[name$="Txt_DataRegDal"]').val();
    //    dataValida = true;
    //    if (dataDaControllare !== "")
    //        dataValida = isValidDate(dataDaControllare);
    //    if (!dataValida) {
    //        MessaggioErrore_Bootstrap("Da data movimento non valida", "DIV_Messaggi");
    //        trovatoErrore = true;
    //    }

    //    dataDaControllare = $('input[name$="Txt_DataRegAl"]').val();
    //    dataValida = true;
    //    if (dataDaControllare !== "")
    //        dataValida = isValidDate(dataDaControllare);
    //    if (!dataValida) {
    //        MessaggioErrore_Bootstrap("A data movimento non valida", "DIV_Messaggi");
    //        trovatoErrore = true;
    //    }



    //    if (!trovatoErrore)
    //        Esporta_CSV();
    //});



    $.logThis("DocReady: FINE");
});