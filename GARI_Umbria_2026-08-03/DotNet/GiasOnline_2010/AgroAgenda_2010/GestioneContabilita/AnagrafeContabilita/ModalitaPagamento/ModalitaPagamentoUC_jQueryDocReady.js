function ModalitaPagamento_DocReady() {
    IDControllo = 'tab_modalita_pagamento';
    $('.anagArea').show();
    CaricaElenchiModalitaPagamento();
    PopolaModalitaPagamento(IDControllo);

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
}