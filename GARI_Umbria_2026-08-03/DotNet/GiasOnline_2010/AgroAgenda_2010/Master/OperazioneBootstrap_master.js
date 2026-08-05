

//OperazioneBootstrap_master.js



//Lettura dati Centri di Costo

function Centro_DiCosto_Leggi() {

}

function Tipologia_Leggi() {

}

function jQueryAddSelect(selector, selectValues, dataValueField, dataTextField) {


    if (dataValueField !== undefined) {
        $.each(selectValues, function (key, value) {
            kendoDropDown_addNew(selector, key, value, dataValueField, dataTextField);
        });
    } else {
        $.each(selectValues, function (key, value) {
            $(selector)
                .append($("<option></option>")
                    .attr("value", key)
                    .text(value));
        });

    }

}


//#Region "Gestione griglie"

//#End Region ""


//#Region "Gestione tab costi accessori"

function tabCosti_onClick(tabId) {

    //<input type="hidden" id="hdKendo_CostiAccessori_ComboHelper" runat="server" />  
    //<input type="hidden" id="hdKendo_CostiAccessori_ComboHelper_Comando" runat="server" />        
    //<asp:Button ID="btnKendo_CostiAccessori_ComboHelper" runat="server" Visible="false" />

    $(id_hdKendo_CostiAccessori_ComboHelper_Comando).val(tabId);
    $(id_btnKendo_CostiAccessori_ComboHelper).click();

}

//#End Region ""



//#Region "Gestione tab costi accessori"
//#END Region "Gestione tab costi accessori"



//#Region "Integrazione con il gis"

function PuntoGPSFast() {

    GisSmartBsBackGround = true;

    blinkGPS("#btnPosizione a");
    OnOffLineReset();
    $(id_OperazioneBootStrap_txtPosizione).val("");

    requestPosition(PuntoGPSFastOnGeoSuccess, PuntoGPSFastOnGeoError);

}



function PuntoGPSFastOnGeoSuccess(lat, lng) {

    var latLngStr = lat.toString().replace(",", ".") + ', ' + 
        lng.toString().replace(",", ".") 

    
    $(id_OperazioneBootStrap_txtPosizione).val(latLngStr);

    OnOffLineSet(true);

    blinkGPSalt("#btnPosizione a");

    //stop alla richiesta di coordinate..
    window.clearInterval(intervalGPS);
    intervalGPS = 0;    

    if (wpid !== undefined) {
        navigator.geolocation.clearWatch(wpid);
        wpid = undefined;
    }
    
}

function blinkGPS(selector) {
    $(selector).fadeOut(2000, function () {
        $(this).fadeIn(2000, function () {
            blinkGPS(this);
        });
    });
}

function blinkGPSalt(selector) {
    $(selector).stop().fadeTo('slow', 1);
}

function OnOffLineSet(isOnline) {

    if (isOnline) {
        $("#btnPosizione").children("a").removeClass("btnPosizioneOffline");

        $("#btnPosizione").children("a").removeClass("btnPosizioneOnline");
        $("#btnPosizione").children("a").addClass("btnPosizioneOnline");

    } else {
        $("#btnPosizione").children("a").removeClass("btnPosizioneOnline");

        $("#btnPosizione").children("a").removeClass("btnPosizioneOffline");
        $("#btnPosizione").children("a").addClass("btnPosizioneOffline");
    }

}

function OnOffLineReset() {
    $("#btnPosizione").children("a").removeClass("btnPosizioneOnline");
    $("#btnPosizione").children("a").removeClass("btnPosizioneOffline");
}

function PuntoGPSFastOnGeoError(msg) {

    if (msg.message !== "") {
        console.log(msg.message);
    }

    if (msg.code !== 2) {
        OnOffLineSet(false);
    }
    
}

function inizializzazioneModuloGIS() {

    //se non esiste il modulo GIS, allora esco.
    if ($("#GisSmartBSctrl").attr("aria-attivo") === "false") {
        return;
    }

    //inizializzazione variabili
    GisSmartDestinazioneChiaveAlbero = "test";

    //gestione del post salvataggio del punto..
    GisSmartDestinazioneHiddenPuntiOnAfterUpdate = function (deferredV) {

        try {

            $("#GisSmartBSctrl").data("kendoDialog").close();
            $("#gissmartBsAnteprimaSuMappa").data("kendoDialog").close();

            console.log("GisSmartDestinazioneHiddenPuntiOnAfterUpdate: hidden punti da variabile:" + GisSmartDestinazioneHiddenPunti);

            $(id_OperazioneBootStrap_txtPosizione).val(GisSmartDestinazioneHiddenPunti.substring(1, GisSmartDestinazioneHiddenPunti.length - 1  ));

            if (deferredV !== undefined) {
                deferredV.resolve();
            }
        
        } catch (e) {

            if (deferredV !== undefined) {
                deferredV.reject();
            }

        }

    };
}

//#end Region "Integrazione con il gis"