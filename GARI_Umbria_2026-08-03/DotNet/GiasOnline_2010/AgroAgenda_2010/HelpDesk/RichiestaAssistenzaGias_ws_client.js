// Carica le Combo Aziende

var indirizzohttp = "./RichiestaAssistenzaGias.aspx";
function EmptyRead(options) { }
function EmptySubmit(options) { }

//Carica le aziende visibili dall'utente'
function RiempicmbAzienda(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtente",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}





