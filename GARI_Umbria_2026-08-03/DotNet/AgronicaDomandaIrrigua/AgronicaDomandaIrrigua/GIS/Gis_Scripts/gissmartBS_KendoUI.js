


function kendoDropDownFixWidth(e) {

    var w = $(".tab-pane").width();

    kendoDropDownAdjustWidth(e, w);

}


function kendoGisSmartBsDialog(jQuerySelector) {
    $(jQuerySelector).kendoDialog({        
        title: "Gis",
        visible: false,        
        close: kendoGisSmartBsDialog_onClose,
        open: kendoGisSmartBsDialog_onOpen
    }).data("kendoDialog");
}

function kendoGisSmartBsDialogAnteprima(jQuerySelector) {
    $(jQuerySelector).kendoDialog({        
        title: "Anteprima",        
        visible: false,        
        close: kendoGisSmartBsDialogAnteprima_onClose,
        open: kendoGisSmartBsDialogAnteprima_onOpen
    }).data("kendoDialog");
}

function kendoGisSmartBsDialog_onClose() {

}


function kendoGisSmartBsDialog_onOpen() {
    if (!formGisSmartBsAnagraficaAttiva) {
        $(".formGisSmartBSAnagrafica").hide();
    }
}

function kendoGisSmartBsDialogAnteprima_onOpen() {

    kendoGisSmartBsDialogAnteprima_Ridimensiona();

}

function kendoGisSmartBsDialogAnteprima_Ridimensiona(widthSet, heightSet) {

    if (widthSet === undefined) {
        widthSet = $("#map_canvas").width($(".container").width() - 77);
        console.log("width set:" + widthSet);
    }

    if (heightSet === undefined) {

        var contHeight = $(window).height();
        var contOffSet = 222;

        heightSet = contHeight - contOffSet;

        console.log("container height set:" + contHeight);
        console.log("container offset set:" + contOffSet);
        console.log("height set:" + heightSet);

    }

    $("#map_canvas").width(widthSet);
    $("#map_canvas").height(heightSet);
       

}

function kendoGisSmartBsDialogAnteprima_onClose() {
    if (!GisSmartBsBackGround) {
        $("#GisSmartBSctrl").data("kendoDialog").open();
    }
    
}