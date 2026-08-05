function Carica_Lotto_AssegnaxRisumSpeVarQualCert() {
    $.logThis("DocReady - Lotto_AssegnaxRisumSpeVarQualCert: INIZIO");

    //inizializzazione della pagina la prima volta che viene caricata
    $(".anagArea").show();

    RicercaValoriParametriQualitativiQualita();
    RicercaValoriParametriQualitativiCertif();
    RicercaFornitori(true, piva);

    RicercaSpecie(piva);

    $("#azioni_CampionamentoConferito").hide();
    popolaLotto_Assegna("tab_lotto_assegna");

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $.logThis("DocReady - Lotto_AssegnaxRisumSpeVarQualCert: FINE");
}

