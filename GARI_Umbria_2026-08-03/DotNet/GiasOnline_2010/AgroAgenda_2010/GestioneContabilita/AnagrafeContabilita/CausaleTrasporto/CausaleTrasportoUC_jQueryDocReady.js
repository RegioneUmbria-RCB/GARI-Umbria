function CausaleTrasporto_DocReady() {
    IDControllo = 'tab_causale_trasporto';
    $('.anagArea').show();

    ElencoTipoCausaliTrasporto();
    PopolaCausaleTrasporto(IDControllo);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
}