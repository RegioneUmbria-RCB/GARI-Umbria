function Cancellazione() {
    var req = {
        Email: $("#txtEmail").val()
    };

    ajaxAgronicaSync(indirizzohttp + "/Cancellazione",
        kendo.stringify(req),
        false,
        function (risposta) {
            ResetForm();
            kendo.alert('Richiesta cancellazione inserita');
        }, null);
}


function ResetForm() {
    $("#txtEmail").val("");
}
