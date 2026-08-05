// TODO
function tabClick(e, elem) {

    if ($(elem).hasClass('disabled')) {
        e.preventDefault();

        $("<div></div>").kendoAlert({
            title: TraduzioneMultiResx(resxContabileDettagliUC, "OperazioneNonConsentita", "Operazione non consentita"),
            content: TraduzioneMultiResx(resxContabileDettagliUC, "ConfermareOAnnullareLeModifiche", "Confermare o annullare le modifiche.")
        }).data("kendoAlert").open();

        return false;
    }

}

// TODO ancora da richiamare
function ControllaGiacenza(item) {
    let bloccaPerSottoGiacenza = false;
    // Blocco se non permesso sotto giacenza 
    if (item.Qta > item.Giacenza) {
        if (impostazioni_Categorie_Giacenza !== undefined && impostazioni_Categorie_Giacenza !== null &&
            impostazioni_Categorie_Giacenza.length > 0) {

            //TODO: se si dovrà richiamare occorre usare una versione di getGestioneGiacenza() in cui si passa l'elem_cod
            // perché in quella funzione c'è la gestione corretta con bypass per l'ordine
            //quindi non si dovrà usare l'arrayLookup direttamente qui...

            let itemCat = arrayLookup(impostazioni_Categorie_Giacenza, "Elem_Cod", item.Elem_Cod);
            if (itemCat !== undefined && itemCat !== null &&
                itemCat.Impostazione_Valore === enum_Gestione_Giacenze_SoloPresenti) {
                bloccaPerSottoGiacenza = true;
            }

        }

        if (!bloccaPerSottoGiacenza && impostazioni_Blocca_SottoGiacenza === true) {
            bloccaPerSottoGiacenza = true;
        }
    }

    return bloccaPerSottoGiacenza;
}

//function impostaTestataSolaLettura(flag) {

//    testataLavorazioneUC_solaLettura(flag);
//    if (flag) {

//        $('#a_tabTestataLavorazione').removeClass('disabled');
//        $('#a_tabScaricoSuLavorazione').removeClass('disabled');
//        $('#a_tabCaricoDaLavorazione').removeClass('disabled');
//        $('#a_tabRiepilogo').removeClass('disabled');

//        $("#idRowSalvaAnnulla").hide();
//        $("#idRowAbilitaModifiche").show();

//    } else {

//        $('#a_tabTestataLavorazione').addClass('disabled');
//        $('#a_tabScaricoSuLavorazione').addClass('disabled');
//        $('#a_tabCaricoDaLavorazione').addClass('disabled');
//        $('#a_tabRiepilogo').addClass('disabled');

//        $("#idRowSalvaAnnulla").show();
//        $("#idRowAbilitaModifiche").hide();

//    }

//}

//function abilitaModificheClick() {
//    impostaTestataSolaLettura(false);
//}

//function getDatiLavorazione() {
//    var data = $('input[name$="hdKendo_TestataLavorazione"]').val();
//    jSonParsed_Kendo = JSON.parse(data);
//    return jSonParsed_Kendo.kendo_rows[0];
//}

//function annullaModificheClick() {
//    var idAgenda = $('input[name$="hdId_Agenda"]').val();
//    if (idAgenda === "") {

//        var piva = getParameterByName('p');
//        window.location = "./RicercaLavorazioni.aspx?p=" + piva;

//    } else {

//        RiempiLavorazione();
//        impostaTestataSolaLettura(true);

//    }
//}

//function salvaModificheClick() {

//    var flagModifica = true;
//    var idAgenda = $('input[name$="hdId_Agenda"]').val();
//    if (idAgenda === "")//devo inserire una nuova lavorazione...
//        flagModifica = false;

//    var lavorazione = GeneraOggettoLavorazione(flagModifica);

//    if (lavorazione === undefined || lavorazione === null)
//        return;

//    if (flagModifica) {

//        var data = getDatiLavorazione();

//        if (data !== undefined && data !== null) {
//            lavorazione.idAgenda = data.Id_Agenda;
//            lavorazione.idMov = data.Id_Mov_AperturaLav;
//            lavorazione.idMovDet = data.Id_Mov_Det_AperturaLav;
//            lavorazione.calCod = data.Cal_Cod_AperturaLav;
//            lavorazione.dataLettura = kendo.parseDate(data.DataOraUltimaLettura);
//        }

//        var lav = kendoEscapeOggetto(lavorazione);

//        var ris = ModificaTestataLavorazione($(cIdPiva).val(), lav, true);

//        if (ris !== undefined && ris.RispostaOK === true) {
//            MessaggioTuttoOK_Bootstrap("Aggiornamento effettuato correttamente", "DIV_Messaggi");
//            impostaTestataSolaLettura(true);

//            //modifico la testata e i carichi/scarichi per i default dei valori in griglia
//            RicercaLavorazione("./Lavorazioni_WS.aspx", $(cIdPiva).val(), parseInt($(cIdAgenda).val()));
//            RicercaScarichi("./Gestione_Lavorazioni.aspx", "tab_elenco_scarichi", $(cIdPiva).val(), parseInt($(cIdAgenda).val()));
//            RicercaCarichi("./Gestione_Lavorazioni.aspx", "tab_elenco_carichi", $(cIdPiva).val(), parseInt($(cIdAgenda).val()));

//        } else if (ris !== undefined && ris.RispostaOK === false) {
//            MessaggioErrore_Bootstrap(ris.RispostaStringa + "<br/>" + "Errore: " + ris.Errore, "DIV_Messaggi");
//        } else {
//            MessaggioErrore_Bootstrap("Errore aggiornamento", "DIV_Messaggi");
//        }
//    } else {

//        var lav = kendoEscapeOggetto(lavorazione);

//        var ris = InserisciTestataLavorazione($(cIdPiva).val(), lav, true);

//        if (ris !== undefined && ris.RispostaOK === true) {
//            MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
//            //valorizzo l'hidden field
//            $(cIdAgenda).val(ris.RispostaStringa);
//            impostaTestataSolaLettura(true);

//            //devo eseguire le letture che si sarebbero dovute fare in PageLoad se avessi avuto una IdAgenda in QueryString
//            RicercaLavorazione("./Lavorazioni_WS.aspx", $(cIdPiva).val(), parseInt($(cIdAgenda).val()));
//            RicercaScarichi("./Gestione_Lavorazioni.aspx", "tab_elenco_scarichi", $(cIdPiva).val(), parseInt($(cId_Agenda).val()), null);
//            RicercaCarichi("./Gestione_Lavorazioni.aspx", "tab_elenco_carichi", $(cIdPiva).val(), parseInt($(cId_Agenda).val()), null);

//        } else if (ris !== undefined && ris.RispostaOK === false) {
//            MessaggioErrore_Bootstrap(ris.RispostaStringa + "<br/>" + "Errore: " + ris.Errore, "DIV_Messaggi");
//        } else {
//            MessaggioErrore_Bootstrap("Errore salvataggio", "DIV_Messaggi");
//        }
//    }
//}