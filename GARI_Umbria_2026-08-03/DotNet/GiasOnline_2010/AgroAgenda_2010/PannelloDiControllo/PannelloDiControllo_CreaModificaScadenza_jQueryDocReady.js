//OPERAZIONI ALL'AVVIO
WaitFrame.show();
$(document).ready(function () {

    //verifico prima di tutto che un ID sia stato passato
    if ($('#' + hfID_Elem_ClientID).val() == "") {
        alert("Errore: Campo ID_Elem in sessione vuoto");
        return;
    }
     
    ddlAzienda_Load(); //popolo l'elenco delle aziende
    ddlGravita_Load(); //popolo l'elenco delle gravità

    if ($('#' + hfID_Elem_ClientID).val() == "-1") {//se è una nuova Scadenza
        ddlArea_Load(); 
    } else { //se è una Scadenza esistente
        caricaDatiScadenza($('#' + hfID_Elem_ClientID).val()); //leggo tutti i dati della Scadenza
        CategorieScadenza_Load(); //leggo la categoria ed assegno i dati nella combo
    }


    //GESTORI PULASNTI
    $('#btnSalva').click(function (e) {
        e.preventDefault();
        btnSalva_Click();
    });
});
