// PER CARICARE GLI STATI
function ddlStato_Load(ddlStato_ClientID) {
    ajaxAgronicaSync("PannelloDiControllo_ScriptService_Stati.asmx/Leggi_Stati", null, false,
                    function (risposta) {
                        var lista = risposta.RispostaStringa;
                        var strHtml = ""
                        for (var i = 0; i < lista.length; i++) {
                            strHtml += "<option value='" + lista[i]["Value"] + "'>" + lista[i]["Text"] + "</option>"
                        }
                        $('#' + ddlStato_ClientID).html(strHtml);
                        $('#' + ddlStato_ClientID).selectpicker('refresh');
                    }, null);
}

// PER CARICARE GLI UTENTI
function ddlUtente_Load(ddlUtente_ClientID) {
    ajaxAgronicaSync("PannelloDiControllo_ScriptService.asmx/Leggi_Utenti", null, false,
                function (risposta) {
                    var lista = risposta.RispostaStringa;
                    var strHtml = ""
                    for (var i = 0; i < lista.length; i++) {
                        strHtml += "<option value='" + lista[i]["Value"] + "'>" + lista[i]["Text"] + "</option>"
                    }
                    $('#' + ddlUtente_ClientID).html(strHtml);
                    $('#' + ddlUtente_ClientID).selectpicker('refresh');
                }, null);
}

//PER REIMPOSTARE I LINK SUL MENU DEGLI ALLEGATI
function PnlDet_ddlAllegati_Change(ddlAllegati_ClientID, hfID_ListaAllegati_ClientID) {
    //estraggo i tag d'interesse
    var tagA_Modifica = $(ddlAllegati_ClientID).parent().find('a:contains("Modifica")');
    var tagA_Elimina = $(ddlAllegati_ClientID).parent().find('a:contains("Elimina")');

    //IDAllegato
    var allSelezionato = $(ddlAllegati_ClientID).parent().children('select').val();
    tagA_Modifica.attr('pnlCtrl-alldetIDAll', allSelezionato);
    tagA_Elimina.attr('pnlCtrl-alldetIDAll', allSelezionato);

    //IDListaAllegati
    var idLista = $('#' + hfID_ListaAllegati_ClientID).val();
    tagA_Modifica.attr('pnlCtrl-alldetIDLista', idLista);
    tagA_Elimina.attr('pnlCtrl-alldetIDLista', idLista);

    //attivo o disattivo i pulsanti modifica/cancella in base a se ho selezioanto qualcosa o no
    if (allSelezionato == -1) {
        tagA_Modifica.parent().addClass("disabled");
        tagA_Elimina.parent().addClass("disabled");

        //tagA_Modifica.parent().attr('title', "Selezionare prima un allegato");
        //tagA_Elimina.parent().attr('title', "Selezionare prima un allegato");
    }
    else {
        tagA_Modifica.parent().removeClass("disabled");
        tagA_Elimina.parent().removeClass("disabled");

        //tagA_Modifica.parent().attr('title', "");
        //tagA_Elimina.parent().attr('title', "");
    }

}