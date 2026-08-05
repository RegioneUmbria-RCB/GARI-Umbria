
/* RilieviBSjQueryDocReady.js */
var rilieviBSResxLocal;

$(document).ready(function () {

    if (!rilieviBSResxLocal) {
        RilieviBSResxLeggi();
    }

    $("#IntestazioneMenuBS2017").hide();

    InizializzaPanelBar();

    ImpostaVisibilita();

    var lav_cod = $(hdLav_Cod_clientID).val();

    if (lav_cod === LAVCOD_RILIEVOAVVERSITAINCAMPO || lav_cod === LAVCOD_VISITA)
        RilievoAvversitaInCampoComboInizializza();
    if (lav_cod === LAVCOD_RILIEVOINDICIMATURITA || lav_cod === LAVCOD_VISITA)
        RilievoIndiciMaturitaComboInizializza();
    if (lav_cod === LAVCOD_FASIFENOLOGICHE || lav_cod === LAVCOD_VISITA)
        RilievoFasiFenologicheComboInizializza();
    if (lav_cod === LAVCOD_RILIEVOERBEINFESTANTI || lav_cod === LAVCOD_VISITA)
        RilievoErbeInfestantiComboInizializza();
    if (lav_cod === LAVCOD_RILIEVODANNIALLARACCOLTA || lav_cod === LAVCOD_VISITA)
        RilievoDanniRaccoltaComboInizializza();
    if (lav_cod === LAVCOD_RILIEVOINDICIRESERACCOLTA || lav_cod === LAVCOD_VISITA)
        RilievoIndiciReseRaccoltaComboInizializza();

    CategorieVisiteComboInizializza();

    letturaTabella_Kendo();

    //Se sono in scrittura (no modifica/lettura) e ho già una specie selezionata
    if ($(hdTipoOperazione_clientID).val() === "1" && $(comboSpecie_clientID).val() !== undefined && $(comboSpecie_clientID).val() !== "-1") {
        leggiPreset();
        AggiornaDopo_SupTrattata();
    }

});


function ImpostaVisibilita() {

    $("#main_menu_button").hide();
    $("#divNote").hide();

    $(id_OperazioneBootStrap_ProvenienzaRisorse).hide();
    $(id_UpdateCostiAccessoriKendo).hide();

    var col12 = "col-lg-12 col-md-12 col-sm-12 col-xs-12";
    var col6 = "col-lg-6 col-md-12 col-sm-12 col-xs-12";
    var col4 = "col-lg-4 col-md-12 col-sm-12 col-xs-12";
    var col3 = "col-lg-3 col-md-12 col-sm-12 col-xs-12";

    $("#divFiltri").attr("class", col12 & " nopadding");
    $(id_OperazioneBootStrap_DivUsernameCreazione).attr("class", col6);
    $(id_OperazioneBootStrap_DivPosizione).attr("class", col6);
    $("#divUpdatePanelData").attr("class", col6);
    $("#divUpdatePanelCentroAziendale").attr("class", col6);
    $(id_OperazioneBootStrap_Specie).attr("class", col6);
    $(id_OperazioneBootStrap_DivImpresa).attr("class", col6);
}

function InizializzaPanelBar() {
    $("#panelBarVisite").kendoPanelBar({ expandMode: "multiple" });
    var panelbar = $("#panelBarVisite").data("kendoPanelBar");
    var items = panelbar.element.children();
    panelbar.expand(items, false);
}

function RilieviBSResxLeggi() {
    var letturaRiuscita = false;
    ajaxAgronicaSync("../Localization.aspx/RitornaRisorseBS", JSON.stringify({ files: "Operazioni/App_LocalResources/RilieviBS.aspx.resx" }), false,
        function (risposta) {
            try {
                rilieviBSResxLocal = JSON.parse(risposta.RispostaStringa);
                letturaRiuscita = true;
                console.log("RilieviBSResxLeggi...letto correttamente.");
            } catch (e) {
                console.log("RilieviBSResxLeggi...errori in fase di parse del json di risorse globali.");
            }

        }, function (risposta) {
            console.log("RilieviBSResxLeggi...errori in fase di lettura.");
        });
    return letturaRiuscita;
}