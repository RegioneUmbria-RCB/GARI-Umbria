
//DOCUMENT READY
$(document).ready(function () {
    docR();
});

async function docR() {
    /*if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            gestioneCarbResx.push(readResxFile(resxSinglePath, "gestionecosti_jQueryDocReady.js"));
        });
    }*/
    //await ddlAzienda_Load();

    $.logThis("DocReady: INIZIO");


    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    //document.getElementById("provante").style.opacity = "1";

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    ddlArea_Load();
    ConfiguraGrigliaVisibilita("griglia_visibilita_area")

    function onActivate(e) {

    }

    $("#btn_salva").click(
        function () {
            AggiornaVisibilita();
        });

    //inizializzazione della pagina la prima volta che viene caricata

    $.logThis("DocReady: FINE");
}