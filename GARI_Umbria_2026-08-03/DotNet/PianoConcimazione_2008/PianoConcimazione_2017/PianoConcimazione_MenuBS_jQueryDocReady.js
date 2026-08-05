$('.DatePicker').kendoDatePicker();

$(document).ready(function () {
    docReady();
});

async function docReady() {
    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            MenuBSResx.unshift(readResxFile(resxSinglePath, "PianoConcimazione_MenuBS_jQueryDocReady.js"));
        });
    }

    Enum_Metodo = {
        Bilancio: { value: 1, name: TraduzioneMultiResx(MenuBSResx, "Bilancio", "Bilancio"), code: 1 },
        Schede: { value: 2, name: TraduzioneMultiResx(MenuBSResx, "Semplificato", "Semplificato"), code: 2 }
    }

    FiltroDate();
    FiltroDatePD();
    kendo_PianiConcimazione_LeggiAjax();
    GrigliaKendoConcimazione('kendo_PianiConcimazione');
    window_Distribuzione = $("#window_PianiDistribuzione");
    //aggiornaIconeAllegati();

    window_Distribuzione.kendoWindow({
        width: "80%",
        height: "80%",
        title: TraduzioneMultiResx(MenuBSResx, "PianiDistribuzione", "Piani Distribuzione"),
        visible: false,
        actions: [
            "Minimize",
            "Maximize",
            "Close"
        ],
        animation: {
            open: {
                duration: 300
            },
        },
        close: onClose
    }).data("kendoWindow").center();//.open();

    let resp = await leggixPratiche_PianiConcimazione_LeggiAjax();
    let jsResp = JSON.parse(resp.RispostaStringa);
    if (jsResp.msg !== "") {
        kendo.confirm(jsResp.msg).then(function () {
            creaPratiche(JSON.stringify(jsResp.anni), jsResp.servizio_cod);
        }, function () {
            //kendo.alert("You chose to Cancel action.");
        });
    }
}
