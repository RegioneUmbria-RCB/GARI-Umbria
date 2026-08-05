function ParametriQualitativiUC_DocReady() {
    IDControllo = 'tab_parametri_qualitativi';

    ElencoTipoParametri();
    Carica_ModuloGenerazione();
    $('.anagArea').show();
    Carica_ParametriQualitativi();

}