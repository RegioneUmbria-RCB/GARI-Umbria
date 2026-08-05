
//gissmartBS_Localization


// -- Localisation --
var localResources;
//$.getJSON(urlRoot, function (data) { localResources = data; }).success(function () { alert("second success"); }).error(function () { alert("error"); })
//.complete(function () { alert("complete"); }); ;
var lblListaPoligonipresentiincache;
var lblcoordinateseparateda;
var lblPoligono;
var lblModificaPoligono;
var lblVisualizzazioneRapidadelPoligono;
var lblEliminaPoligono;
var lblCancellaUltimoPunto;
var lblSalvaPoligono;
var lblNuovoEsistente;
var lblricercaimpianto;
var lblAssociaPoligonoa;
var lblErrorepochivertici;

var lblNuovoPuntoDaGPS;
var lblPassaAllaMappa;
var lblAzienda;
var lblCentro;
var lblImpianto;
var lblRete;
var lblOnOffLine;
var lblGpsOnOffLine;
var lblCaricaTutteLeAziende;
var lblTrovatiDatiCartograficiArchivio;
var lblRecuperaPuntiInMemoria;
var lblEliminaTuttiPuntiInMemoria;
var lblDescrizione;
var lblSalvaOnLine;
var lblSalvaOffline;
var lblSelezionaAziendaOScriviPerCercarla;
var lblSelezionaCentroAziendale;
var lblSelezionaImpianto;
var lblSelezionaPunto;



function localizzaDaRisorse() {

    var urlRootGisSmartBsLocalization = pathCoreWS + "/Localization.asmx/RitornaRisorseBS";
    var gissmartBsResourceFile = "Gis/App_LocalResources/gissmart.js.resx";

    ajaxAgronica(urlRootGisSmartBsLocalization,
        JSON.stringify({ objP_Server: objP_server, files: gissmartBsResourceFile, linguaRichiesta: "it-it" }),
        function(risposta) {
            var localResources = risposta.RispostaStringa;

            lblListaPoligonipresentiincache = JSON.parse(localResources).ListaPoligonipresentiincache;
            lblcoordinateseparateda = JSON.parse(localResources).coordinateseparateda;
            lblPoligono = JSON.parse(localResources).Poligono;
            lblModificaPoligono = JSON.parse(localResources).ModificaPoligono;
            lblVisualizzazioneRapidadelPoligono = JSON.parse(localResources).VisualizzazioneRapidadelPoligono;
            lblEliminaPoligono = JSON.parse(localResources).EliminaPoligono;
            lblCancellaUltimoPunto = JSON.parse(localResources).CancellaUltimoPunto;
            lblSalvaPoligono = JSON.parse(localResources).SalvaPoligono;
            lblNuovoEsistente = JSON.parse(localResources).NuovoEsistente;
            lblAssociaPoligonoa = JSON.parse(localResources).AssociaPoligonoa;
            lblErrorepochivertici = JSON.parse(localResources).errorepochivertici;

            lblNuovoPuntoDaGPS = JSON.parse(localResources).lblNuovoPuntoDaGPS;
            lblPassaAllaMappa = JSON.parse(localResources).lblPassaAllaMappa;
            lblAzienda = JSON.parse(localResources).lblAzienda;
            lblCentro = JSON.parse(localResources).lblCentro;
            lblImpianto = JSON.parse(localResources).lblImpianto;
            lblRete = JSON.parse(localResources).lblRete;
            lblOnOffLine = JSON.parse(localResources).lblOnOffLine;
            lblGpsOnOffLine = JSON.parse(localResources).lblGpsOnOffLine;
            lblCaricaTutteLeAziende = JSON.parse(localResources).lblCaricaTutteLeAziende;
            lblTrovatiDatiCartograficiArchivio = JSON.parse(localResources).lblTrovatiDatiCartograficiArchivio;
            lblRecuperaPuntiInMemoria = JSON.parse(localResources).lblRecuperaPuntiInMemoria;
            lblEliminaTuttiPuntiInMemoria = JSON.parse(localResources).lblEliminaTuttiPuntiInMemoria;
            lblDescrizione = JSON.parse(localResources).lblDescrizione;
            lblSalvaOnLine = JSON.parse(localResources).lblSalvaOnLine;
            lblSalvaOffline = JSON.parse(localResources).lblSalvaOffLine;
            lblSelezionaAziendaOScriviPerCercarla = JSON.parse(localResources).lblSelezionaAziendaOScriviPerCercarla;
            lblSelezionaCentroAziendale = JSON.parse(localResources).lblSelezionaCentroAziendale;
            lblSelezionaImpianto = JSON.parse(localResources).lblSelezionaImpianto;


            localizzaDaRisorseImpostaEtichette();            
            
            gisSmartGestioneCombo();

            inizializzaComboDescrizioni();
            inizializzaPoligoni(true);
            RecuperaPoligono();

        },
        null);
}


function localizzaDaRisorseImpostaEtichette() {

    $("#btnAziendaCaricaTutte").text(lblCaricaTutteLeAziende);
    $("#btn_nuovoPunto").text(lblNuovoPuntoDaGPS);
    $("#lbl_Azienda").text(lblAzienda);
    $("#lbl_CentriAziendali").text(lblCentro);
    $("#lbl_AppNome").text(lblImpianto);
    $("#btn_EditViaMappa").text(lblPassaAllaMappa);
    $("#lbl_Descrizione").text(lblDescrizione);
    $("#btnRecuperaPunti").text(lblRecuperaPuntiInMemoria);
    $("#btn_Salva").text(lblSalvaOnLine);
}
