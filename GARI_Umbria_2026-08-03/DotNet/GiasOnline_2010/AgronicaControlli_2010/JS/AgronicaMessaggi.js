$(document).ready(function () {

    if (GiasVersioneMaster === '2022') {
        creaKendoNotification('xonne-notification-success', 3000);
        creaKendoNotification('xonne-notification-error', 0);
    }
    //TestAgroMessagi();
});

function posizionaMessggi(jQuerySelector) {
    try {
        $(jQuerySelector).css("z-index", "100000");
        $(jQuerySelector).css("position", "absolute");
        $(jQuerySelector).css("top", window.pageYOffset.toString() + "px");
        $(jQuerySelector).css("width", $(window).width().toString() + "px");

    } catch (e) {
        console.log("posizionaMessggi non riuscita...");
    }

}

//MESSAGGI SU SCHERMO
function MessaggioErrore_Bootstrap(str, id_div) {
    if (GiasVersioneMaster === '2022') {
        $("#xonne-notification-error").getKendoNotification().show({
            title: "",
            message: str
        }, 'error');
    } else {
        var stringa_html = '<div class="alert alert-danger" role="alert">' + str +
            '<button type="button" class="close chiudi_alert" data-dismiss="alert"><span aria-hidden="true">&times;</span> <span class="sr-only">Close</span></button></div>';
        $('#' + id_div).html(stringa_html);
        posizionaMessggi('#' + id_div);
    }
}

function MessaggioErroreTooltip_Bootstrap(str, id_div, tooltip) {
    MessaggioErrore_Bootstrap(str, id_div);
    $('#' + id_div).attr('title', tooltip);
    posizionaMessggi('#' + id_div);
}

function MessaggioTuttoOK_Bootstrap(str, id_div) {
    if (GiasVersioneMaster === '2022') {
        $("#xonne-notification-success").getKendoNotification().show({
            title: "",
            message: str
        }, 'success');
    } else {
        var stringa_html = '<div class="alert alert-success" role="alert">' + str +
            '<button type="button" class="close chiudi_alert" data-dismiss="alert"><span aria-hidden="true">&times;</span> <span class="sr-only">Close</span></button></div>';
        $('#' + id_div).html(stringa_html);
        posizionaMessggi('#' + id_div);
        setTimeout(function () { $('.chiudi_alert').click(); }, 3000);
    }
}

function MessaggioAttenzione_Bootstrap(str, id_div, timeFadeOut) {
    if (GiasVersioneMaster === '2022') {
        $("#xonne-notification-error").getKendoNotification().show({
            title: "",
            message: str
        }, 'warning');
        // creaKendoNotification(id_div, 'warning', str, timeFadeOut || 0);
    } else {
        var stringa_html = '<div class="alert alert-warning" role="alert">' + str +
            '<button type="button" class="close chiudi_alert" data-dismiss="alert"><span aria-hidden="true">&times;</span> <span class="sr-only">Close</span></button></div>';
        $('#' + id_div).html(stringa_html);
        posizionaMessggi('#' + id_div);
        if (timeFadeOut === undefined || timeFadeOut === null) {
            timeFadeOut = 3000;
        }
        if (timeFadeOut !== 0) {
            setTimeout(function () { $('.chiudi_alert').click(); }, timeFadeOut);
        }
    }
}

function MessaggioErrore(str) {
    $('#dialog_errore').modal('show');
    $('#messaggioErrore').html(str);

    WaitFrame.hide();
}

function TestAgroMessagi()
{
    alert("AgroMessaggiCaricati");
    alert(GiasVersioneMaster);
    MessaggioErrore_Bootstrap("Errore", "DIV_Messaggi");
    MessaggioAttenzione_Bootstrap("Attenzione", "DIV_Messaggi", 0);
    MessaggioTuttoOK_Bootstrap("Tutto OK", "DIV_Messaggi");
}