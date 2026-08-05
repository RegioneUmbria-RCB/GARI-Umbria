
//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");
     
    //Controllo se ha i permessi di lettura sulla pagina
    UtenteAbilitatoLettura = $("input[name$='hf_UtenteAbilitatoLettura']").val() === "True";
    UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    
    $("#btn_salva").click(function () {
            Aggiorna_Testo();
    });
      
    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    RicercaTestoEditor();

    $.logThis("DocReady: FINE");

});

