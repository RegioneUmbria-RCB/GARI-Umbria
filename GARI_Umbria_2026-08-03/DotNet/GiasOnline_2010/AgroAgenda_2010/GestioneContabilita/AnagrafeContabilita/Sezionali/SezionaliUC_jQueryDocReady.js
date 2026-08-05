function Sezionali_DocReady() {
    IDControllo = 'tab_sezionali';
    $('.anagArea').show();

    CaricaElenchiSezionali();
    Leggi_RegimiFiscale();
    PopolaSezionali(IDControllo);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
}