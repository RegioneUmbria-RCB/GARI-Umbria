
//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");


    wnd = $("#details")
        .kendoWindow({
            title: "Log G2G",
            modal: true,
            visible: false,
            resizable: true,
            height: "80%",
            width: "80%"
        }).data("kendoWindow");

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    //popolaConfigurazioni();

    $.logThis("DocReady: FINE");

});