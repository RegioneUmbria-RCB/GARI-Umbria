/**

DOC READY

**/

jQuery(document).ready(function () {
    var tipoOperazione = $(Controls.xTipoOperazione).val();

    switch (tipoOperazione) {
        case "0":
        case "2":
        case "10":
            Prodotto_Edit_UC_DocReady_Prodotto();
            // Lettura / Modifica / Copia (Duplica)
            break;
        case "1":
            // Scrittura
            Prodotto_Edit_UC_DocReady_Prodotto();
            break;
    }

});




