function ElencoValoriParametriQualitativiUC_DocReady() {
    IDControllo = 'tab_elenco_valori_parametri_qualitativi';

    Carica_ModuloGenerazione_ElencoValoriParametriQualitativi();
    $('.anagArea').show();
	Carica_ElencoValoriParametriQualitativi();
    Carica_ElencoValori_ParametriQualitativi();

}