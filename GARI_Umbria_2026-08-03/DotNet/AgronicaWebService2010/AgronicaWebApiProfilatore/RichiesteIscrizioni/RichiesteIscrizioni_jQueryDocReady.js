function Registrazione() {
    var req = {
        Nome: $("#txtNome").val(),
        Cognome: $("#txtCognome").val(),
        RagioneSociale: $("#txtRagioneSociale").val(),
        PartitaIva: $("#txtPartitaIva").val(),
        Email: $("#txtEmail").val(),
    };

    ajaxAgronicaSync(indirizzohttp + "/Registrazione",
        kendo.stringify(req),
        false,
        function (risposta) {
            ResetForm();
            //kendo.alert('Registrazione completata');
            var NuovoLink = '/AgronicaWebApiProfilatore/RichiesteIscrizioni/RichiesteIscrizioniCompletata.aspx';
            document.location = NuovoLink;
        }, null);
}


function ResetForm() {
    $("#txtNome").val("");
    $("#txtCognome").val("");
    $("#txtRagioneSociale").val("");
    $("#txtPartitaIva").val("");
    $("#txtEmail").val("");
}
