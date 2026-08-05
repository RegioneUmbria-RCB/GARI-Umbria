var resxCastellettoUC = [];

//DOCUMENT READY
$(document).ready(function() {
    // non c'è nulla, tutto spostato nella funzione impostaCastellettoUC, perché sia il chiamante a gestire quando farla
});

function impostaCastellettoUC(conGrigliaEredita) {

    $.logThis("EMBEDDED CastellettoUC_jQueryDocReady: INIZIO");

    if (resxObj !== null && resxObj !== undefined) {
        resxCastellettoUC = resxObj;
    }
    else {
        resxCastellettoUC.push(readResxFile("GestioneContabilita/App_LocalResources/DocContabile.aspx.resx"));
        resxCastellettoUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx"));
    }

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    /*document.getElementById("panelAreaCastelletto").style.opacity = "1";*/

    $(".nbRiepilogoImporto").attr("readonly", true);
    $(".nbRiepilogoImporto").kendoNumericTextBox({
        decimals: 2,
        format: "n2",
        spinners: false
    });

    ConfiguraGrigliaCastelletto("tab_griglia_castelletto");

    $.logThis("EMBEDDED CastellettoUC_jQueryDocReady: FINE");

}
