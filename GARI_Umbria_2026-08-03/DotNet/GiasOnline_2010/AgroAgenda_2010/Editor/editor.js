
function riempiTesto(IDControllo) {

    var autorizzazioni = {
        UtenteAbilitatoInserimentoModifica: UtenteAbilitatoScrittura,
    };

    var editorTools = {
        //insertFile: true
    };

    var value = $('input[name$="hdTesto"]').val();
    creaKendoEditor(IDControllo,  
        value,   
        editorTools,
        autorizzazioni
    );
}
