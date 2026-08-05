function Conferimento_GestionePianiReparti_DocReady() {
    IDControllo = 'tab_celle_reparti_piani';

    LeggiCentriAziendali();
    $('.anagArea').show();
    PopolaRepartiPiani(IDControllo);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
}