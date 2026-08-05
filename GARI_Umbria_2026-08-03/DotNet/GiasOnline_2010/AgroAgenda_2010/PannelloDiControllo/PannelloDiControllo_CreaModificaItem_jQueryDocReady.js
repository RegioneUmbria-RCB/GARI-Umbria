//OPERAZIONI ALL'AVVIO
$(document).ready(function () {

    //verifico prima di tutto che un ID sia stato passato
    if ($('#' + hfID_NC_ClientID).val() == "") {
        alert("Errore: Campo ID_NC in sessione vuoto");
        return;
    }

    ddlAzienda_Load(); //popolo l'elenco delle aziende
    ddlGravita_Load(); //popolo l'elenco delle gravità
    
    if ($('#' + hfID_NC_ClientID).val() == "-1") {//se è una nuova NC
        ddlArea_Load();
    } else { //se è una NC esistente
        caricaDatiNC($('#' + hfID_NC_ClientID).val()); //leggo tutti i dati della NC
        pnlAreaComune_Titolo_Load(); // carico il titoletto del pannello di area comune
        CategorieNC_Load(); //leggo la categoria ed assegno i dati nella combo
    }

    //GESTORI PULASNTI
    $('#btnSalva').click(function (e) {
        e.preventDefault();
        btnSalva_Click();
    });
});
