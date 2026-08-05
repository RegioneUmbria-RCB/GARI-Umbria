var resxObj = [];
var resxArrPath = [
    "RecuperaCredenziali/App_LocalResources/RichiesteIscrizioni.aspx.resx",
    "App_GlobalResources/AgronicaAgenda_2010.resx"
];
var pathCoreWS = "";
var allegatoInBase64 = null;
var elencoindici = null;
var elencodettagli = null;

$(document).ready(function () {
    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj.push(readResxFile(resxSinglePath, "RichiesteIscrizioni_jQueryDocReady.js"));
        });
    }

    pathCoreWS = $("[name='hfPathCoreWS']").val();

    $("#Form1").on("submit", function (ev) {
        ev.preventDefault();
        return false;
    });

    var jsonTemplIscr = $("#hfTemplateIscr").val();
    var arrTemplIscr = JSON.parse(jsonTemplIscr);
    if (arrTemplIscr.length > 0) {
        var objTemplIscr = arrTemplIscr[0];
        $("#btnScaricaTemplIscr").on("click", objTemplIscr, btnScaricaTemplIscrClick);
    }
    else {
        $("#infoPagina").hide();
        $("#btnScaricaTemplIscr").hide();
    }

    $(".kendoTextBox").kendoTextBox();
    
    if (window.File && window.FileReader && window.FileList && window.Blob) {
        document.getElementById('fileCaricaDocumento').addEventListener("change", fileCaricaDocumentoChange, false);
    } else {
        alert('Il tuo browser non supporta alcune API necessarie al caricamento degli allegati.'); // i18n Non tradotto in quanto messaggio tecnico
    }
    $("#btnCaricaDocumento").on("click", function (ev) {
        $("#fileCaricaDocumento").trigger("click");
    });
    creaKendoDropDownList("ddlTipoFirma", { read: ddlTipoFirmaRead }, "Des", "Value", null, null, null, true).data("kendoDropDownList");

    $("#txtAreaNote").kendoTextArea({
        rows: 3
    });

    $("#btnProcediRichiesta").on("click", btnProcediRichiestaClick);

    var idAreaCategoria = 8;
    var idTipologia = -13;
    CreaIndici(idAreaCategoria, idTipologia);

    $(".container").removeClass("nascosto");
});