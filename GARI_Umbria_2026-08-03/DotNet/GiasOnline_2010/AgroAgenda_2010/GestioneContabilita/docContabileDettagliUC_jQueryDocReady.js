var resxContabileDettagliUC = [];

//DOCUMENT READY
$(document).ready(function () {

    if (resxObj !== null && resxObj !== undefined) {
        resxContabileDettagliUC = resxObj;
        resxContabileDettagliUC.unshift(readResxFile("GestioneContabilita/App_LocalResources/DocContabileDettagliUC.ascx.resx"));
    }
    else {
        resxContabileDettagliUC.push(readResxFile("GestioneContabilita/App_LocalResources/DocContabileDettagliUC.ascx.resx"));
        resxContabileDettagliUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx"));
    }

    creaKendoSwitch("chkGiacenzePositive");
    setKendoSwitch("chkGiacenzePositive", true); 

    //KENDOWINDOW
    $.logThis(" Popup ricerca: INIZIO");
    $("#tab_ricercaArea").kendoWindow({
        actions: [
            //"Pin",
            //"Minimize",
            //"Maximize",
            "Close"
        ],
        visible: false,
        draggable: false,
        height: "90%",
        width: "90%",
        modal: true,
        resizable: true,
        title: TraduzioneMultiResx(resxContabileDettagliUC, "Ricerca", "Ricerca"),
        open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            $("body").addClass("ob-no-scroll");
        },
        close: function (e) {
            $("body").removeClass("ob-no-scroll");
        },
        activate: function (e) {
            var h = $("#tab_ricercaArea").height();
            var footH = $("#tab_ricercaArea .window-footer").outerHeight(true);
            var contH = h - footH;
            $("#tab_ricercaArea .container").height(contH).css("overflow", "auto");
        }
    });

    $('#tab_ricercaArea').parent().find('.k-window-title').css('text-align', 'center');

    /* altezza barra del titolo della kendoWindow
        .k-window-titlebar {
            height: 35px;
            line-height: 35px;
            vertical-align: middle;
        }
    */

    $('#tab_ricercaArea').parent().css('padding-top', '48px');
    $('#tab_ricercaArea').parent().find('.k-window-titlebar').css('margin-top', '-48px');
    $('#tab_ricercaArea').parent().find('.k-window-titlebar').css('height', '35px');
    $('#tab_ricercaArea').parent().find('.k-window-titlebar').css('line-height', '35px');
    $('#tab_ricercaArea').parent().find('.k-window-titlebar').css('vertical-align', 'middle');

    $.logThis(" Popup ricerca: FINE");
    //FINEWINDOW

});

