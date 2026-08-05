$(document).ready(function () {

    VerificaPermessiEstrazione();

    $("#btnSalvaInterferenze").kendoButton({
        click: function () {
            SalvaInterferenze();
        }
    });
    
});