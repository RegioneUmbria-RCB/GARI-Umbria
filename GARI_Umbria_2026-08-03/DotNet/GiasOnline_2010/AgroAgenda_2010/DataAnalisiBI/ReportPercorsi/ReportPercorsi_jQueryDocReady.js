

//
//  ReportPercorsi_jQueryDocReady.js
//



$(document).ready(function () {

    $(".export_excel").hide();

    //let d1 = new Date(new Date().getFullYear(), 0, 1);
    //let d2 = new Date(new Date().getFullYear(), 11, 31);

    let d2 = new Date();
    let d1 = new Date();
    d1.setDate(d2.getDate() - 1);
    
    let dp1 = $("#Txt_Data_DA").kendoDatePicker({
        start: "year"
    });
    dp1.data("kendoDatePicker").value(d1);

    let dp2 = $("#Txt_Data_A").kendoDatePicker({
        start: "year"
    });
    dp2.data("kendoDatePicker").value(d2);

    $("#btn_ricerca").click(function () {
        ElaboraReport();        
    });

    $("#btn_PosizioneAttuale").click(function () {
        ElaboraPosizioneAttuale();        
    });

    $("#btn_posizioniRilevate").click(function () {
        ElaboraPosizioniRilevate();        
    });

    $("#btn_raccolte").click(function () {
        ElaboraRaccolte();
        $(".export_excel").show();
    });


    let qryLast = Request_QueryString("last");
    let iQryLast = parseInt(Request_QueryString("last"));
    if (qryLast !== null) {

        d1 = Request_QueryString("DataInizio");
        d2 = Request_QueryString("DataFine");

        dp1.data("kendoDatePicker").value(d1);
        dp2.data("kendoDatePicker").value(d2);

        switch (iQryLast) {
            case enum_tipoReportGPF.Percorsi.value:
                $("#btn_ricerca").click();
                break;
            case enum_tipoReportGPF.UltimaPosizione.value:
                $("#btn_PosizioneAttuale").click();
                break; case enum_tipoReportGPF.PosizioniRilevate.value:
                $("#btn_posizioniRilevate").click();
                break;
            default:
                break;
        }
    }

    $("#exportXls").click(function () {
        var datiEsportati = Esporta();
        var contentType = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,'
        var blob = new Blob([datiEsportati], { 'type': contentType });
        var data = new Date(Date.now());
        var nomeFile = 'ReportPercorsi_' + data.getFullYear() + '-' + data.getMonth() + 1 + '-' + data.getDate() + '_' + data.getHours() + '-' + data.getMinutes() + '-' + data.getSeconds() + '-' + data.getMilliseconds() + '.xls';

        var isIE = !!document.documentMode;
        if (isIE) { //se è IE
            navigator.msSaveOrOpenBlob(blob, nomeFile)
        }
        else { //Se sono gli altri...Chrome...
            var aLink = document.createElement('a');
            var evt = document.createEvent("HTMLEvents");
            evt.initEvent("click", true, false);
            aLink.href = window.URL.createObjectURL(blob);
            aLink.download = nomeFile
            aLink.dispatchEvent(evt);
        }

    });


});


