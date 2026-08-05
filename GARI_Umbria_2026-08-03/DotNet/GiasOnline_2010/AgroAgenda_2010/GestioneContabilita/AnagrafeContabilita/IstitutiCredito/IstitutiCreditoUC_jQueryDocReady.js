function IstitutiCredito_DocReady() {
    IDControllo = 'tab_istituti_credito';
    $('.anagArea').show();

    PopolaIstitutiCredito(IDControllo);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
}