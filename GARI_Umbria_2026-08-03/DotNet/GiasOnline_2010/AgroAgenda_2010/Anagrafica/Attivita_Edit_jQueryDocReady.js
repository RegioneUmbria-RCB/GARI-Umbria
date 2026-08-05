/**

DOC READY

**/
jQuery(document).ready(function () {

    var tipoOperazione = $(Controls.xTipoOperazione).val();

    switch (tipoOperazione) {
        case "0":
        case "2":
        case "10":
            Attivita_Edit_UC_DocReady_Attivita();
            break;
        case "1":
            Attivita_Edit_UC_DocReady_Attivita();
            break;
    }

});




