function ScritturaOK() {
    $('.dialogAvvertimento').get(0).innerHTML = "<table class='centerTab'><tr><td>Operazione Eseguita con Successo!<br>" +
                "</td>" +
                '<td><img alt="Errore" src="<%= ResolveClientUrl("~/img/ValidazioneSI.ico") %>" /></td></tr></table>';
//        $('.dialogAvvertimento').dialog('open');
    setTimeout(function () { $('.dialogAvvertimento').dialog('open'); }, 500);
}
function MessaggioErrore(str) {
    $('.dialogErrore').html("<table class='centerTab'><tr><td>" + str + "<br>" +
                "</td>" +
                '<td><img alt="Errore" src="<%= ResolveClientUrl("~/img/IconError.jpg") %>" /></td></tr></table>');

    setTimeout(function () { $('.dialogErrore').dialog('open'); }, 500);
}