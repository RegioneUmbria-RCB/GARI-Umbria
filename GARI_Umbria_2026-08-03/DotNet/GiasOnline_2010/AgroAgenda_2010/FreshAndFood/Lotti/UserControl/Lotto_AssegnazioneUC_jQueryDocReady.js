function Lotto_AssegnazioneUCDocReady() {

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    //inizializzazione della pagina la prima volta che viene caricata

    var Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    $(".anagArea").show();

    if (Tipo_Lotto === "E") {

        indirizzohttp_Lotto_AssegnazioneUC = "./Lotto_AssegnaxRisumSpeVarQualCert.aspx";

        RicercaValoriParametriQualitativiQualita();
        RicercaValoriParametriQualitativiCertif();
        RicercaFornitori(true, cIdPiva);
    }
    else if (Tipo_Lotto === "L") {

        indirizzohttp_Lotto_AssegnazioneUC = "./Lavorazione_Config.aspx";

        RicercaTipologieLavorazioni();
    }

    RicercaSpecie(cIdPiva);

    $("#azioni_CampionamentoConferito").hide();


    popolaLotto_Assegna("tab_lotto_assegna");

    //fine controlli

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

}