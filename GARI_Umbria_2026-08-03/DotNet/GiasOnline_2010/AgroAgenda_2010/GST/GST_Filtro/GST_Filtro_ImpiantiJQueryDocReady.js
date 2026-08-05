
$(document).ready(function () {

    $("#" + ImgBtn_Interferenze_ClientID).on("click", clickInterferenze);

    $("#btnSalvaPreventivo").kendoButton({
        click: function () {
            SalvaPreventivo();
        }
    });
    $("#btnSalvaConsuntivo").kendoButton({
        click: function () {
            SalvaConsuntivo();
        }
    });

    mostraSportello();
    VerificaPermessiEstrazione();
});

