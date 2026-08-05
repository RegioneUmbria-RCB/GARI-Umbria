function GruppiReferenzeUC_DocReady() {
    IDControllo = 'tab_gruppi_referenze';

    Carica_ModuloGenerazione();
    RicercaSpecie(piva);
    $('.anagArea').show();
    Carica_GruppiReferenze();
}