
/* RegCaricoScaricoPassaporti_jQueryDocReady.js  */
var resxObj = [];
var resxArrPath = [
    "GestioneMagazzini/App_LocalResources/RegCaricoScaricoPassaporti.aspx.resx",
    "AgronicaCoreContabBIZ.dll/AgronicaCoreContabBIZ.AgronicaCoreContabBIZ",
    "App_GlobalResources/AgronicaAgenda_2010.resx"
];

$(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj.push(readResxFile(resxSinglePath, "RegCaricoScaricoPassaporti_jQueryDocReady.js"));
        });
    }

    InizializzaPagina_RegCaricoScarico();

});


function InizializzaPagina_RegCaricoScarico() {

    inizializzaDate();

    $("#btn_stampalibera").click(function() {
        stampaPassaporto();
    });

    $("#btn_ricerca").click(function() {

        RicercaRigheRegistroPassaportiLettura(true, false, true);

    });

    $("#btn_recupera").click(function () {
        RicercaRigheRegistroPassaportiLettura(true, true, true);
    });

    
    GeneraKendoWindowMaps("#kendoWindowiFrameGeneric", "Generic", "60%", "95%");

    let w = $(window).width();
    let h = $(window).height();


    $("#pannelloInfo").kendoDialog({
        title: "",
        closable: true,
        open: function () {
            $("#pannelloInfoImg").show();
        },
        close: function () {
            $("#pannelloInfoImg").hide();
        },
        modal: true,
        visible: false,
        width: w * 0.37,
        height: h * 0.87,        
        actions: []
    });

    $("#btn_info").click(function () {
        $("#pannelloInfo").data("kendoDialog").open();
    });

} 

function inizializzaDate() {

    LeggiAnnataAgraria();


}
