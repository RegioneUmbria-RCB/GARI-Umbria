function Liquidita_DocReady() {
    IDControllo = 'tab_liquidita';
    $('.anagArea').show();
    CaricaElenchiLiquidita();
    ElencoIstitutiCredito();
    PopolaLiquidita(IDControllo);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
}