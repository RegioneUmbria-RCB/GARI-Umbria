function Carica_ParamEntrataXSpecieVarieta() {
    $.logThis("DocReady - ParamEntrataXSpecieVarieta: INIZIO");

    //inizializzazione della pagina la prima volta che viene caricata
    $(".anagArea").show();
    RicercaSpecie(piva);
    popolaParamEntrata("tab_parametri_specie_varieta");
    RicercaTipoRiferimentoPrezzi();

    $.logThis("DocReady - ParamEntrataXSpecieVarieta: FINE");
}