function Conferimento_GestioneCelle_DocReady() {
    IDControllo = 'tab_celle';

    LeggiCentriDiCosto();
    LeggiReparti();
    $('.anagArea').show();
    PopolaCelle(IDControllo);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
}