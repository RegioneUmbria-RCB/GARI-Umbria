var menuBSAnagraficaResx;
var flag_disciplinareprivato;

var permesso_modificaMultipla;
var permesso_ModificaMultipla_write;

var gestioneEserciziResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/GestioneEsercizi.aspx.resx"
];

$(document).ready(function () {

    MenuBSAnagraficaResxLeggi()

    $("#btnFiltraEsercizi").click(function () {
        FiltraEsercizi();

    });

    if (modificaMultipla != "" && modificaMultipla != "0") {
        permesso_modificaMultipla = true;
    } else {
        permesso_modificaMultipla = false;
    }

    chiama_popola_griglia()

    docR();

    //In modalità mono azienda non è possibile filtrare gli esercizi, perchè vengono scelti direttamente dall'anagrafica
    if (modalitaMonoAzienda == "true") {
        $("#btnFiltraEsercizi").hide()
    }

    chiavi = JSON.parse($('#' + idChiavi).val());
});



// ################################################################################################################################################

function MenuBSAnagraficaResxLeggi() {
    var letturaRiuscita = false;
    ajaxAgronicaSync("../Localization.aspx/RitornaRisorseBS", JSON.stringify({ files: "App_GlobalResources/AgronicaAgenda_2010.resx" }), false,
        function (risposta) {
            try {
                menuBSAnagraficaResx = JSON.parse(risposta.RispostaStringa);
                letturaRiuscita = true;
                console.log("MenuBSAnagraficaResxLeggi...letto correttamente.");
            } catch (e) {
                console.log("MenuBSAnagraficaResxLeggi...errori in fase di parse del json.");
            }

        }, function (risposta) {
            console.log("MenuBSAnagraficaResxLeggi...errori in fase di lettura.");
        });
    return letturaRiuscita;
}



async function docR() {
    flag_disciplinareprivato = await LeggiDisciplinarePrivato();
}


function getParameterByName(name, url) {
    if (!url) {
        url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return "";
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}








