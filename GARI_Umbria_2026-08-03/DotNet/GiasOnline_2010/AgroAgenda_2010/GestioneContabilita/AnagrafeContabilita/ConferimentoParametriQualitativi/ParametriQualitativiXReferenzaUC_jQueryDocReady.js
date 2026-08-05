function ParametriQualitativiXReferenzaUC_DocReady() {
    IDControllo = 'tab_parametri_qualitativi_referenza';
    
    Carica_Elenco_GruppiReferenze();
    Carica_Elenco_ParametriQualitativi();
    $('.anagArea').show();
    Carica_ParametriQualitativiXReferenza();
}