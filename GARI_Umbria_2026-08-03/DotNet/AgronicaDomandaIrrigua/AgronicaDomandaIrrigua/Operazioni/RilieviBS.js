function InserisciSupTrattata(valore) {}
function InserisciSupTotale(valore) {}

//Funzione eseguita sulla operazioneBootstrap_master dopo il check di un impianto
function AggiornaDopo_SupTrattata() {

    var lav_cod = $(hdLav_Cod_clientID).val();

    var hd = "";
    switch (lav_cod) {
        case LAVCOD_RILIEVOINDICIMATURITA:
            hd = '#hdPreset_RilievoIndiciMaturita';
            break;
        case LAVCOD_RILIEVOAVVERSITAINCAMPO:
            hd = '#hdPreset_RilievoAvversitaInCampo';
            break;
        case LAVCOD_RILIEVOINDICIRESERACCOLTA:
            hd = '#hdPreset_RilievoIndiciReseRaccolta';
            break;
        case LAVCOD_FASIFENOLOGICHE:
            hd = '#hdPreset_RilievoFasiFenologiche';
            break;
        case LAVCOD_RILIEVOERBEINFESTANTI:
            hd = '#hdPreset_RilievoErbeInfestanti';
            break;
        case LAVCOD_RILIEVODANNIALLARACCOLTA:
            hd = '#hdPreset_RilievoDanniRaccolta';
            break;
        case LAVCOD_VISITA:
            hd = '#hdPreset_Visita';
            break;
    }

    var lista = $(hd).val();

    if (lista !== undefined && lista !== "") {

        var listaPreset = JSON.parse(lista);

        if (listaPreset !== undefined && listaPreset.length > 0) {
            switch (lav_cod) {
                case LAVCOD_RILIEVOINDICIMATURITA:
                    AggiungiRilievoIndiciMaturita_PerPreset(listaPreset);
                    break;
                case LAVCOD_RILIEVOINDICIRESERACCOLTA:
                    AggiungiRilievoIndiciReseRaccolta_PerPreset(listaPreset);
                    break;
                case LAVCOD_RILIEVOAVVERSITAINCAMPO:
                    AggiungiRilievoAvversitaInCampo_PerPreset(listaPreset);
                    break;
                case LAVCOD_FASIFENOLOGICHE:
                    AggiungiRilievoFasiFenologiche_PerPreset(listaPreset);
                    break;
                case LAVCOD_RILIEVOERBEINFESTANTI:
                    AggiungiRilievoErbeInfestanti_PerPreset(listaPreset);
                    break;
                case LAVCOD_RILIEVODANNIALLARACCOLTA:
                    AggiungiRilievoDanniRaccolta_PerPreset(listaPreset);
                    break;
                case LAVCOD_VISITA:
                    //for (let k = 0; k < listaPreset.length; k++) {

                    //}
                    //$(hdSelezioneCategorieVisite_clientID).val("");
                    //selezionaCategorieVisite();

                    for (let j = 0; j < listaPreset.length; j++) {
                        let pre7 = [listaPreset[j]];
                        switch (listaPreset[j].Lav_cod.toString()) {
                            case LAVCOD_RILIEVOINDICIMATURITA:
                                AggiungiRilievoIndiciMaturita_PerPreset(pre7);
                                break;
                            case LAVCOD_RILIEVOINDICIRESERACCOLTA:
                                AggiungiRilievoIndiciReseRaccolta_PerPreset(pre7);
                                break;
                            case LAVCOD_RILIEVOAVVERSITAINCAMPO:
                                AggiungiRilievoAvversitaInCampo_PerPreset(pre7);
                                break;
                            case LAVCOD_FASIFENOLOGICHE:
                                AggiungiRilievoFasiFenologiche_PerPreset(pre7);
                                break;
                            case LAVCOD_RILIEVOERBEINFESTANTI:
                                AggiungiRilievoErbeInfestanti_PerPreset(pre7);
                                break;
                            case LAVCOD_RILIEVODANNIALLARACCOLTA:
                                AggiungiRilievoDanniRaccolta_PerPreset(pre7);
                                break;
                        }
                    }
                    break;
            }
        }

    }

}


function leggiPreset() {

    //Leggo da WS l'eventuale preset
    ajaxAgronicaSync(indirizzohttp + "/CaricaListaPreset", null, true,
        function (risposta) {

            var lista = risposta.RispostaStringa;

            var hd = "";
            var subPanel_ClientID = "";
            var lav_cod = $(hdLav_Cod_clientID).val();

            switch (lav_cod) {
                case LAVCOD_RILIEVOINDICIMATURITA:
                    hd = '#hdPreset_RilievoIndiciMaturita';
                    subPanel_ClientID = panelBarIndiciMaturita_clientID;
                    break;
                case LAVCOD_RILIEVOINDICIRESERACCOLTA:
                    hd = '#hdPreset_RilievoIndiciReseRaccolta';
                    break;
                case LAVCOD_RILIEVOAVVERSITAINCAMPO:
                    hd = '#hdPreset_RilievoAvversitaInCampo';
                    subPanel_ClientID = panelBarAvversitaInCampo_clientID;
                    //Il preset lo leggo solo se è impostato 'nessun disciplinare', in caso contrario svuoto la griglia ed esco
                    if ($(comboDisciplinari_clientID).val() !== '0') {
                        if ($("#divRilievi").data('kendoGrid') !== undefined) {
                            $("#divRilievi").data('kendoGrid').dataSource.data([]);
                        }
                        $(hd).val("");
                        return;
                    }
                    break;
                case LAVCOD_FASIFENOLOGICHE:
                    hd = '#hdPreset_RilievoFasiFenologiche';
                    subPanel_ClientID = panelBarFasiFenologiche_clientID;
                    break;
                case LAVCOD_RILIEVOERBEINFESTANTI:
                    hd = '#hdPreset_RilievoErbeInfestanti';
                    subPanel_ClientID = panelBarErbeInfestanti_clientID;
                    break;
                case LAVCOD_RILIEVODANNIALLARACCOLTA:
                    hd = '#hdPreset_RilievoDanniRaccolta';
                    subPanel_ClientID = panelBarDanniRaccolta_clientID;
                    break;
                case LAVCOD_VISITA:
                    //Il preset lo leggo solo se è impostato 'nessun disciplinare', in caso contrario svuoto la griglia ed esco
                    hd = '#hdPreset_Visita';
                    if ($(comboDisciplinari_clientID).val() !== '0') {
                        if ($("#divRilievi").data('kendoGrid') !== undefined) {
                            $("#divRilievi").data('kendoGrid').dataSource.data([]);
                        }
                        $(hd).val("");
                        return;
                    }
                    break;
            }

            $(hd).val(lista);

            //Se ci sono elementi chiudo il pannello
            if (subPanel_ClientID !== "") {
                var panelBar = $("#panelBarVisite").data("kendoPanelBar");
                var listaPreset = JSON.parse(lista);

                if (listaPreset !== undefined && listaPreset.length > 0)
                    panelBar.collapse($(subPanel_ClientID));
                else
                    panelBar.expand($(subPanel_ClientID));
            }

        });

}

function ComboOperazioniChange() {
    $("#comboRilievoAvversitaInCampo").data("kendoDropDownList").dataSource.read();
    $("#comboRilievoIndiciMaturita").data("kendoDropDownList").dataSource.read();
    $("#comboRilievoFasiFenologiche").data("kendoDropDownList").dataSource.read();
    $("#comboRilievoErbeInfestanti").data("kendoDropDownList").dataSource.read();
    $("#comboRilievoDanniRaccolta").data("kendoDropDownList").dataSource.read();
    $("#comboRilievoIndiciReseRaccolta").data("kendoDropDownList").dataSource.read();
}

/**
 * Aggiunge i dati in tabella, impianto per impianto
 */
function AggiungiVisitaPers() {

    //cicla gli impianti selezionati
    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        let Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        var visitaSelezionata_val = $("#comboVisitePers").data("kendoDropDownList").value();
        var visitaSelezionata_text = $("#comboVisitePers").data("kendoDropDownList").text();
        var Descrizione = $("#txtVisitaPers_Descr").val();
        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }
        if (Descrizione === undefined || Descrizione === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareDescrizioneVisitaPersonalizzata", "Specificare una Descrizione sulla visita personalizzata"), "DIV_Messaggi");
            return;
        }
        if (visitaSelezionata_val === undefined || visitaSelezionata_val === "" || visitaSelezionata_text === undefined || visitaSelezionata_text === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareVisitaPersonalizzata", "Specificare una visita personalizzata"), "DIV_Messaggi");
            return;
        }


        if (impiantiSelezionati === undefined || impiantiSelezionati.length <= 0) {
            //SE NON SI SELEZIONANO DEGLI IMPIANTI
            //MessaggioErrore("Selezionare almeno un impianto", "DIV_Messaggi");
            //return;

            var numCentroAz = parseInt($('.ComboCentroAziendale.selectpicker option:selected').val());

            //Controllo che il rilievo non sia già stato inserito per quelle aziende/centri nella stessa operazione
            let piva = pivaAziendaSelezionata.toString();
            let sa_cod = numCentroAz.toString();

            let dataSource = grid.dataSource.data();

            for (let j = 0; j < dataSource.length; j++) {
                let riga = dataSource[j];

                if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && 0 === riga.appezza && 0 === riga.id_reg && visitaSelezionata_val.toString() === riga.lav_cod.toString()) {                    
                    MessaggioErrore(
                        kendo.format(Traduzione(rilieviBSResxLocal, "ErroreEsisteGiàVisitaPerAziendaCentro", "Errore: esite già una visita '{0}' per questa azienda/centro aziendale"), visitaSelezionata_text),
                        "DIV_Messaggi");
                    return;
                }

            }

            //Aggiungo la visita
            let strAzCentro = "";
            if (numCentroAz !== 0) {
                strAzCentro = kendo.format(
                    Traduzione(rilieviBSResxLocal, "VisitaSuCentroPlaceholder", "<Visita sul Centro Aziendale: {0}>"),
                    $('.ComboCentroAziendale.selectpicker option:selected').text()
                );
            } else {
                strAzCentro = Traduzione(rilieviBSResxLocal, "VisitaSuImpresaWrapped", "<Visita sull'Impresa>");
            }

            let o = {
                Codice: "i_" + 1,
                Impianto: strAzCentro,
                piva: pivaAziendaSelezionata,
                sa_cod: numCentroAz,
                appezza: 0,
                id_reg: 0,
                Data: Data,
                av_cod: 0,
                udm_cod: 0,
                ff_cod: 0,
                Descrizione: visitaSelezionata_text,
                Dato: Descrizione,
                Dato_Testuale: Descrizione,
                Cul_Des: "",
                Qta2: 0,
                lav_cod: visitaSelezionata_val,
                Operazione: Traduzione(rilieviBSResxLocal, "VisitePersonalizzate", "Visite Personalizzate"),
                Veg_Cod: 0,
                ind_mat_cod: 0,
                dr_cod: 0
            };

            AggiungiRilievoRiga(grid, o);


        } else {
            //SE SI SELEZIONANO DEGLI IMPIANTI
            var veg_cod = $(comboSpecie_clientID).val();

            //Controllo che il rilievo non sia già stato inserito per quegli impianti nella stessa operazione
            for (var h = 0; h < impiantiSelezionati.length; h++) {
                let piva = impiantiSelezionati[h].Piva.toString();
                let sa_cod = impiantiSelezionati[h].Sa_Cod.toString();
                let appezza = impiantiSelezionati[h].Appezza.toString();
                let id_reg = impiantiSelezionati[h].ID_Reg.toString();

                let dataSource = grid.dataSource.data();

                for (let j = 0; j < dataSource.length; j++) {
                    let riga = dataSource[j];

                    if (piva === riga.piva && sa_cod === riga.sa_cod && appezza === riga.appezza && id_reg === riga.id_reg && visitaSelezionata_val.toString() === riga.lav_cod.toString()) {
                        MessaggioErrore(
                            kendo.format(Traduzione(rilieviBSResxLocal, "ErroreEsisteGiàVisitaPerImpianto", "Errore: esiste già una visita '{0}' per l'impianto '{1}'"), visitaSelezionata_text, ImpiantoRicavaDescrizione(impiantiSelezionati[h])),
                            "DIV_Messaggi");
                        return;
                    }

                    if (riga.Veg_Cod.toString() !== "0" && veg_cod !== "0" && veg_cod !== riga.Veg_Cod.toString()) {
                        MessaggioErrore(
                            Traduzione(rilieviBSResxLocal, "ErroreImpossibileAggiungereVisiteSecondaSpecie", "Errore: Impossibile aggiungere visite per più di una specie vegetale. Creare una nuova Operazione per la seconda specie."),
                            "DIV_Messaggi");
                        return;
                    }

                }
            }

            //Aggiungo la visita
            for (let i = 0; i < impiantiSelezionati.length; i++) {

                let o = {
                    Codice: "i_" + i.toString(),
                    Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                    piva: impiantiSelezionati[i].Piva,
                    sa_cod: impiantiSelezionati[i].Sa_Cod,
                    appezza: impiantiSelezionati[i].Appezza,
                    id_reg: impiantiSelezionati[i].ID_Reg,
                    Data: Data,
                    av_cod: 0,
                    udm_cod: 0,
                    ff_cod: 0,
                    Descrizione: visitaSelezionata_text,
                    Dato: Descrizione,
                    Dato_Testuale: Descrizione,
                    Cul_Des: impiantiSelezionati[i].Cul_Des,
                    Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                    lav_cod: visitaSelezionata_val,
                    Operazione: Traduzione(rilieviBSResxLocal, "Visite Personalizzate", "Visite Personalizzate"),
                    Veg_Cod: veg_cod,
                    ind_mat_cod: 0,
                    dr_cod: 0
                };

                AggiungiRilievoRiga(grid, o);

            } //impiantiSelezionati

        }

    }
}


/**
 * Aggiunge i dati in tabella, impianto per impianto
 */
function AggiungiRilievoAvversitaInCampo() {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        var Descrizione = $("#comboRilievoAvversitaInCampo").data("kendoDropDownList").text();
        var DatoRilevato = $("#txtQtaRilevataAvversitaInCampo").val();

        var DatoRilevatoTestuale = DatoRilevato;
        if ($("#comboQtaRilevataAvversitaInCampo").data('kendoDropDownList') !== undefined && $("#comboQtaRilevataAvversitaInCampo").data('kendoDropDownList').text() !== "")
            DatoRilevatoTestuale = $("#comboQtaRilevataAvversitaInCampo").data('kendoDropDownList').text();

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var rilievoSelezionato = $("#comboRilievoAvversitaInCampo").data("kendoDropDownList").value();
        var vRilievoSelezionato = rilievoSelezionato.split("|");

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }
        if (Descrizione === undefined || Descrizione === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnRilievo", "Specificare un rilievo"), "DIV_Messaggi");
            return;
        }
        //if (DatoRilevato === undefined || DatoRilevato === "") {
        //    MessaggioErrore("Specificare una quantità rilevata valida", "DIV_Messaggi");
        //    return;
        //}
        if (impiantiSelezionati === undefined || impiantiSelezionati.length <= 0) {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SelezionareAlmenoUnImpianto", "Selezionare almeno un impianto"), "DIV_Messaggi");
            return;
        }

        var veg_cod = $(comboSpecie_clientID).val();
        var dataSource = grid.dataSource.data();

        //Controllo che il rilievo non sia già stato inserito per quegli impianti nella stessa operazione
        for (var h = 0; h < impiantiSelezionati.length; h++) {
            var piva = impiantiSelezionati[h].Piva.toString();
            var sa_cod = impiantiSelezionati[h].Sa_Cod.toString();
            var appezza = impiantiSelezionati[h].Appezza.toString();
            var id_reg = impiantiSelezionati[h].ID_Reg.toString();

            for (var j = 0; j < dataSource.length; j++) {
                var riga = dataSource[j];

                if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && vRilievoSelezionato[0].toString() === riga.av_cod.toString()) {
                    MessaggioErrore(
                        kendo.format(Traduzione(rilieviBSResxLocal, "ErroreEsisteGiàRilievoPerImpianto", "Errore: esiste già un rilievo '{0}' per l'impianto '{1}'"), Descrizione, ImpiantoRicavaDescrizione(impiantiSelezionati[h])),
                        "DIV_Messaggi");
                    return;
                }

                if (riga.Veg_Cod.toString() !== "0" && veg_cod !== "0" && veg_cod !== riga.Veg_Cod.toString()) {
                    MessaggioErrore(
                        Traduzione(rilieviBSResxLocal, "ErroreImpossibileAggiungereRilieviSecondaSpecie", "Errore: Impossibile aggiungere rilievi per più di una specie vegetale. Creare una nuova Operazione per la seconda specie."),
                        "DIV_Messaggi");
                    return;
                }

            }
        }

        //Aggiungo i rilievi
        for (var i = 0; i < impiantiSelezionati.length; i++) {

            var o = {
                Codice: "i_" + i.toString(),
                Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                piva: impiantiSelezionati[i].Piva,
                sa_cod: impiantiSelezionati[i].Sa_Cod,
                appezza: impiantiSelezionati[i].Appezza,
                id_reg: impiantiSelezionati[i].ID_Reg,
                Data: Data,
                av_cod: vRilievoSelezionato[0],
                udm_cod: vRilievoSelezionato[2],
                ff_cod: 0,
                Descrizione: Descrizione,
                Dato: DatoRilevato,
                Dato_Testuale: DatoRilevatoTestuale,
                Cul_Des: impiantiSelezionati[i].Cul_Des,
                Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                lav_cod: LAVCOD_RILIEVOAVVERSITAINCAMPO,
                Operazione: Traduzione(rilieviBSResxLocal, "RilieviAvversitàInCampo", "Rilievi Avversità In Campo"),
                Veg_Cod: veg_cod,
                ind_mat_cod: 0,
                dr_cod: 0,
            };

            AggiungiRilievoRiga(grid, o);

        } //impiantiSelezionati


    }

}

function AggiungiRilievoAvversitaInCampo_PerPreset(listaPreset) {
    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var veg_cod = $(comboSpecie_clientID).val();
        var dataSource = grid.dataSource.data();

        for (var j = 0; j < listaPreset.length; j++) {

            var Descrizione = listaPreset[j].Descrizione;
            var av_cod = listaPreset[j].av_cod.toString();
            var udm_cod = listaPreset[j].udm_cod;

            //Aggiungo i rilievi
            for (var i = 0; i < impiantiSelezionati.length; i++) {
                var piva = impiantiSelezionati[i].Piva.toString();
                var sa_cod = impiantiSelezionati[i].Sa_Cod.toString();
                var appezza = impiantiSelezionati[i].Appezza.toString();
                var id_reg = impiantiSelezionati[i].ID_Reg.toString();

                //Se il rilievo esiste già, non lo aggiungo
                var esisteGia = false;
                for (var h = 0; h < dataSource.length; h++) {
                    var riga = dataSource[h];
                    if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && av_cod.toString() === riga.av_cod.toString() && udm_cod.toString() === riga.udm_cod.toString()) {
                        esisteGia = true;
                        break;
                    }
                }

                if (esisteGia)
                    continue;

                var o = {
                    Codice: "i_" + i.toString(),
                    Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                    piva: impiantiSelezionati[i].Piva,
                    sa_cod: impiantiSelezionati[i].Sa_Cod,
                    appezza: impiantiSelezionati[i].Appezza,
                    id_reg: impiantiSelezionati[i].ID_Reg,
                    Data: Data,
                    av_cod: av_cod,
                    udm_cod: udm_cod,
                    ff_cod: 0,
                    Descrizione: Descrizione,
                    Dato: "",
                    Dato_Testuale: "",
                    Cul_Des: impiantiSelezionati[i].Cul_Des,
                    Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                    lav_cod: LAVCOD_RILIEVOAVVERSITAINCAMPO,
                    Operazione: Traduzione(rilieviBSResxLocal, "RilieviAvversitàInCampo", "Rilievi Avversità In Campo"),
                    Veg_Cod: veg_cod,
                    ind_mat_cod: 0,
                    dr_cod: 0
                };

                AggiungiRilievoRiga(grid, o);

            } //impiantiSelezionati

        }
    }

}

function AggiungiRilievoIndiciMaturita() {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        var Descrizione = $("#comboRilievoIndiciMaturita").data("kendoDropDownList").text();
        var DatoRilevato = $("#txtQtaRilevataIndiciMaturita").val();

        var DatoRilevatoTestuale = DatoRilevato;
        if ($("#comboQtaRilevataIndiciMaturita").data('kendoDropDownList') !== undefined && $("#comboQtaRilevataIndiciMaturita").data('kendoDropDownList').text() !== "")
            DatoRilevatoTestuale = $("#comboQtaRilevataIndiciMaturita").data('kendoDropDownList').text();

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var rilievoSelezionato = $("#comboRilievoIndiciMaturita").data("kendoDropDownList").value();
        var vRilievoSelezionato = rilievoSelezionato.split("|");

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }
        if (Descrizione === undefined || Descrizione === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnRilievo", "Specificare un rilievo"), "DIV_Messaggi");
            return;
        }
        //if (DatoRilevato === undefined || DatoRilevato === "") {
        //    MessaggioErrore("Specificare una quantità rilevata valida", "DIV_Messaggi");
        //    return;
        //}
        if (impiantiSelezionati === undefined || impiantiSelezionati.length <= 0) {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SelezionareAlmenoUnImpianto", "Selezionare almeno un impianto"), "DIV_Messaggi");
            return;
        }

        var veg_cod = $(comboSpecie_clientID).val();
        var dataSource = grid.dataSource.data();

        //Controllo che il rilievo non sia già stato inserito per quegli impianti nella stessa operazione
        for (var h = 0; h < impiantiSelezionati.length; h++) {
            var piva = impiantiSelezionati[h].Piva.toString();
            var sa_cod = impiantiSelezionati[h].Sa_Cod.toString();
            var appezza = impiantiSelezionati[h].Appezza.toString();
            var id_reg = impiantiSelezionati[h].ID_Reg.toString();

            for (var j = 0; j < dataSource.length; j++) {
                var riga = dataSource[j];

                if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && vRilievoSelezionato[0].toString() === riga.ind_mat_cod.toString()) {
                    MessaggioErrore(
                        kendo.format(Traduzione(rilieviBSResxLocal, "ErroreEsisteGiàRilievoPerImpianto", "Errore: esiste già un rilievo '{0}' per l'impianto '{1}'"), Descrizione, ImpiantoRicavaDescrizione(impiantiSelezionati[h])),
                        "DIV_Messaggi");
                    return;
                }

                if (riga.Veg_Cod.toString() !== "0" && veg_cod !== "0" && veg_cod !== riga.Veg_Cod.toString()) {
                    MessaggioErrore(
                        Traduzione(rilieviBSResxLocal, "ErroreImpossibileAggiungereRilieviSecondaSpecie", "Errore: Impossibile aggiungere rilievi per più di una specie vegetale. Creare una nuova Operazione per la seconda specie."),
                        "DIV_Messaggi");
                    return;
                }

            }
        }

        //Aggiungo i rilievi
        for (var i = 0; i < impiantiSelezionati.length; i++) {

            var o = {
                Codice: "i_" + i.toString(),
                Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                piva: impiantiSelezionati[i].Piva,
                sa_cod: impiantiSelezionati[i].Sa_Cod,
                appezza: impiantiSelezionati[i].Appezza,
                id_reg: impiantiSelezionati[i].ID_Reg,
                Data: Data,
                av_cod: 0,
                udm_cod: vRilievoSelezionato[1],
                ff_cod: 0,
                Descrizione: Descrizione,
                Dato: DatoRilevato,
                Dato_Testuale: DatoRilevatoTestuale,
                Cul_Des: impiantiSelezionati[i].Cul_Des,
                Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                lav_cod: LAVCOD_RILIEVOINDICIMATURITA,
                Operazione: Traduzione(rilieviBSResxLocal, "RilieviIndiciMaturità", "Rilievi Indici Maturità"),
                Veg_Cod: veg_cod,
                ind_mat_cod: vRilievoSelezionato[0],
                dr_cod: 0
            };

            AggiungiRilievoRiga(grid, o);

            } //impiantiSelezionati

    }
}

function AggiungiRilievoIndiciMaturita_PerPreset(listaPreset) {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var dataSource = grid.dataSource.data();
        var veg_cod = $(comboSpecie_clientID).val();

        //grid.dataSource.data([]);

        for (var j = 0; j < listaPreset.length; j++) {

            var Descrizione = listaPreset[j].Descrizione;
            var ind_mat_cod = listaPreset[j].ind_mat_cod.toString();
            var udm_cod = listaPreset[j].udm_cod;

            //Aggiungo i rilievi
            for (var i = 0; i < impiantiSelezionati.length; i++) {

                var piva = impiantiSelezionati[i].Piva.toString();
                var sa_cod = impiantiSelezionati[i].Sa_Cod.toString();
                var appezza = impiantiSelezionati[i].Appezza.toString();
                var id_reg = impiantiSelezionati[i].ID_Reg.toString();

                //Se il rilievo esiste già, non lo aggiungo
                var esisteGia = false;
                for (var h = 0; h < dataSource.length; h++) {
                    var riga = dataSource[h];
                    if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && ind_mat_cod.toString() === riga.ind_mat_cod.toString()) {
                        esisteGia = true;
                        break;
                    }
                }

                if (esisteGia)
                    continue;

                var o = {
                    Codice: "i_" + i.toString(),
                    Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                    piva: impiantiSelezionati[i].Piva,
                    sa_cod: impiantiSelezionati[i].Sa_Cod,
                    appezza: impiantiSelezionati[i].Appezza,
                    id_reg: impiantiSelezionati[i].ID_Reg,
                    Data: Data,
                    av_cod: 0,
                    udm_cod: udm_cod,
                    ff_cod: 0,
                    Descrizione: Descrizione,
                    Dato: "",
                    Dato_Testuale: "",
                    Cul_Des: impiantiSelezionati[i].Cul_Des,
                    Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                    lav_cod: LAVCOD_RILIEVOINDICIMATURITA,
                    Operazione: Traduzione(rilieviBSResxLocal, "RilieviIndiciMaturità", "Rilievi Indici Maturità"),
                    Veg_Cod: veg_cod,
                    ind_mat_cod: ind_mat_cod,
                    dr_cod: 0
                };

                AggiungiRilievoRiga(grid, o);

            } //impiantiSelezionati

        }

    }
}

function AggiungiRilievoDanniRaccolta() {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        var Descrizione = $("#comboRilievoDanniRaccolta").data("kendoDropDownList").text();
        var DatoRilevato = $("#txtQtaRilevataDanniRaccolta").val();

        var DatoRilevatoTestuale = DatoRilevato;
        if ($("#comboQtaRilevataDanniRaccolta").data('kendoDropDownList') !== undefined && $("#comboQtaRilevataDanniRaccolta").data('kendoDropDownList').text() !== "")
            DatoRilevatoTestuale = $("#comboQtaRilevataDanniRaccolta").data('kendoDropDownList').text();

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var rilievoSelezionato = $("#comboRilievoDanniRaccolta").data("kendoDropDownList").value();
        var vRilievoSelezionato = rilievoSelezionato.split("|");

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }
        if (Descrizione === undefined || Descrizione === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnRilievo", "Specificare un rilievo"), "DIV_Messaggi");
            return;
        }
        //if (DatoRilevato === undefined || DatoRilevato === "") {
        //    MessaggioErrore("Specificare una quantità rilevata valida", "DIV_Messaggi");
        //    return;
        //}
        if (impiantiSelezionati === undefined || impiantiSelezionati.length <= 0) {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SelezionareAlmenoUnImpianto", "Selezionare almeno un impianto"), "DIV_Messaggi");
            return;
        }

        var veg_cod = $(comboSpecie_clientID).val();

        //Controllo che il rilievo non sia già stato inserito per quegli impianti nella stessa operazione
        for (var h = 0; h < impiantiSelezionati.length; h++) {
            var piva = impiantiSelezionati[h].Piva.toString();
            var sa_cod = impiantiSelezionati[h].Sa_Cod.toString();
            var appezza = impiantiSelezionati[h].Appezza.toString();
            var id_reg = impiantiSelezionati[h].ID_Reg.toString();

            var dataSource = grid.dataSource.data();

            for (var j = 0; j < dataSource.length; j++) {
                var riga = dataSource[j];

                if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && vRilievoSelezionato[0].toString() === riga.ind_mat_cod.toString()) {
                    MessaggioErrore(
                        kendo.format(Traduzione(rilieviBSResxLocal, "ErroreEsisteGiàRilievoPerImpianto", "Errore: esiste già un rilievo '{0}' per l'impianto '{1}'"), Descrizione, ImpiantoRicavaDescrizione(impiantiSelezionati[h])),
                        "DIV_Messaggi");
                    return;
                }

                if (riga.Veg_Cod.toString() !== "0" && veg_cod !== "0" && veg_cod !== riga.Veg_Cod.toString()) {
                    MessaggioErrore(
                        Traduzione(rilieviBSResxLocal, "ErroreImpossibileAggiungereRilieviSecondaSpecie", "Errore: Impossibile aggiungere rilievi per più di una specie vegetale. Creare una nuova Operazione per la seconda specie."),
                        "DIV_Messaggi");
                    return;
                }

            }
        }

        //Aggiungo i rilievi
        for (var i = 0; i < impiantiSelezionati.length; i++) {

            var o = {
                Codice: "i_" + i.toString(),
                Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                piva: impiantiSelezionati[i].Piva,
                sa_cod: impiantiSelezionati[i].Sa_Cod,
                appezza: impiantiSelezionati[i].Appezza,
                id_reg: impiantiSelezionati[i].ID_Reg,
                Data: Data,
                av_cod: 0,
                udm_cod: vRilievoSelezionato[1],
                ff_cod: 0,
                Descrizione: Descrizione,
                Dato: DatoRilevato,
                Dato_Testuale: DatoRilevatoTestuale,
                Cul_Des: impiantiSelezionati[i].Cul_Des,
                Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                lav_cod: LAVCOD_RILIEVODANNIALLARACCOLTA,
                Operazione: Traduzione(rilieviBSResxLocal, "RilieviDanniAllaRaccolta", "Rilievi Danni Alla Raccolta"),
                Veg_Cod: veg_cod,
                ind_mat_cod: 0,
                dr_cod: vRilievoSelezionato[0]
            };

            AggiungiRilievoRiga(grid, o);

        } //impiantiSelezionati
    }
}

function AggiungiRilievoDanniRaccolta_PerPreset(listaPreset) {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var veg_cod = $(comboSpecie_clientID).val();
        var dataSource = grid.dataSource.data();

        for (var j = 0; j < listaPreset.length; j++) {

            var Descrizione = listaPreset[j].Descrizione;
            var dr_cod = listaPreset[j].dr_cod.toString();
            var udm_cod = listaPreset[j].udm_cod;

            for (var i = 0; i < impiantiSelezionati.length; i++) {
                var piva = impiantiSelezionati[i].Piva.toString();
                var sa_cod = impiantiSelezionati[i].Sa_Cod.toString();
                var appezza = impiantiSelezionati[i].Appezza.toString();
                var id_reg = impiantiSelezionati[i].ID_Reg.toString();

                //Se il rilievo esiste già, non lo aggiungo
                var esisteGia = false;
                for (var h = 0; h < dataSource.length; h++) {
                    var riga = dataSource[h];
                    if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && dr_cod.toString() === riga.dr_cod.toString()) {
                        esisteGia = true;
                        break;
                    }
                }

                if (esisteGia)
                    continue;

                var o = {
                    Codice: "i_" + i.toString(),
                    Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                    piva: impiantiSelezionati[i].Piva,
                    sa_cod: impiantiSelezionati[i].Sa_Cod,
                    appezza: impiantiSelezionati[i].Appezza,
                    id_reg: impiantiSelezionati[i].ID_Reg,
                    Data: Data,
                    av_cod: 0,
                    udm_cod: udm_cod,
                    ff_cod: 0,
                    Descrizione: Descrizione,
                    Dato: "",
                    Dato_Testuale: "",
                    Cul_Des: impiantiSelezionati[i].Cul_Des,
                    Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                    lav_cod: LAVCOD_RILIEVODANNIALLARACCOLTA,
                    Operazione: Traduzione(rilieviBSResxLocal, "RilieviDanniAllaRaccolta", "Rilievi Danni Alla Raccolta"),
                    Veg_Cod: veg_cod,
                    ind_mat_cod: 0,
                    dr_cod: dr_cod
                };

                AggiungiRilievoRiga(grid, o);

            } //impiantiSelezionati

        }
    }

}

function AggiungiRilievoFasiFenologiche() {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        var Descrizione = $("#comboRilievoFasiFenologiche").data("kendoDropDownList").text();
        var DatoRilevato = $("#txtDataRilevataFasiFenologiche").val();

        var DatoRilevatoTestuale = DatoRilevato;
        if ($("#comboDataRilevataFasiFenologiche").data('kendoDropDownList') !== undefined && $("#comboDataRilevataFasiFenologiche").data('kendoDropDownList').text() !== "")
            DatoRilevatoTestuale = $("#comboDataRilevataFasiFenologiche").data('kendoDropDownList').text();

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var rilievoSelezionato = $("#comboRilievoFasiFenologiche").data("kendoDropDownList").value();
        var vRilievoSelezionato = rilievoSelezionato.split("|");

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }
        if (Descrizione === undefined || Descrizione === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaFaseFenologica", "Specificare una fase fenologica"), "DIV_Messaggi");
            return;
        }
        //if (DatoRilevato === undefined || DatoRilevato === "") {
        //    MessaggioErrore("Specificare una data valida", "DIV_Messaggi");
        //    return;
        //}
        if (kendo.parseDate(DatoRilevato) === null) {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }


        if (impiantiSelezionati === undefined || impiantiSelezionati.length <= 0) {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SelezionareAlmenoUnImpianto", "Selezionare almeno un impianto"), "DIV_Messaggi");
            return;
        }

        var veg_cod = $(comboSpecie_clientID).val();
        var dataSource = grid.dataSource.data();

        //Controllo che il rilievo non sia già stato inserito per quegli impianti nella stessa operazione
        for (var h = 0; h < impiantiSelezionati.length; h++) {
            var piva = impiantiSelezionati[h].Piva.toString();
            var sa_cod = impiantiSelezionati[h].Sa_Cod.toString();
            var appezza = impiantiSelezionati[h].Appezza.toString();
            var id_reg = impiantiSelezionati[h].ID_Reg.toString();


            for (var j = 0; j < dataSource.length; j++) {
                var riga = dataSource[j];

                if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && vRilievoSelezionato[0].toString() === riga.ff_cod.toString()) {
                    MessaggioErrore(
                        kendo.format(Traduzione(rilieviBSResxLocal, "ErroreEsisteGiàRilievoPerImpianto", "Errore: esiste già un rilievo '{0}' per l'impianto '{1}'"), Descrizione, ImpiantoRicavaDescrizione(impiantiSelezionati[h])),
                        "DIV_Messaggi");
                    return;
                }

                if (riga.Veg_Cod.toString() !== "0" && veg_cod !== "0" && veg_cod !== riga.Veg_Cod.toString()) {
                    MessaggioErrore(
                        Traduzione(rilieviBSResxLocal, "ErroreImpossibileAggiungereRilieviSecondaSpecie", "Errore: Impossibile aggiungere rilievi per più di una specie vegetale. Creare una nuova Operazione per la seconda specie."),
                        "DIV_Messaggi");
                    return;
                }

            }
        }

        //Aggiungo i rilievi
        for (var i = 0; i < impiantiSelezionati.length; i++) {

            var o = {
                Codice: "i_" + i.toString(),
                Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                piva: impiantiSelezionati[i].Piva,
                sa_cod: impiantiSelezionati[i].Sa_Cod,
                appezza: impiantiSelezionati[i].Appezza,
                id_reg: impiantiSelezionati[i].ID_Reg,
                Data: Data,
                av_cod: 0,
                udm_cod: 0,
                ff_cod: vRilievoSelezionato[0],
                Descrizione: Descrizione,
                Dato: DatoRilevato,
                Dato_Testuale: DatoRilevatoTestuale,
                Cul_Des: impiantiSelezionati[i].Cul_Des,
                Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                lav_cod: LAVCOD_FASIFENOLOGICHE,
                Operazione: Traduzione(rilieviBSResxLocal, "RilieviFasiFenologiche", "Rilievi Fasi Fenologiche"), 
                Veg_Cod: veg_cod,
                ind_mat_cod: 0,
                dr_cod: 0
            };

            AggiungiRilievoRiga(grid, o);

        } //impiantiSelezionati
    }
}

function AggiungiRilievoFasiFenologiche_PerPreset(listaPreset) {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var veg_cod = $(comboSpecie_clientID).val();
        var dataSource = grid.dataSource.data();

        for (var j = 0; j < listaPreset.length; j++) {

            var Descrizione = listaPreset[j].Descrizione;
            var ff_cod = listaPreset[j].ff_cod.toString();

            //Aggiungo i rilievi
            for (var i = 0; i < impiantiSelezionati.length; i++) {
                var piva = impiantiSelezionati[i].Piva.toString();
                var sa_cod = impiantiSelezionati[i].Sa_Cod.toString();
                var appezza = impiantiSelezionati[i].Appezza.toString();
                var id_reg = impiantiSelezionati[i].ID_Reg.toString();

                //Se il rilievo esiste già, non lo aggiungo
                var esisteGia = false;
                for (var h = 0; h < dataSource.length; h++) {
                    var riga = dataSource[h];
                    if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && ff_cod.toString() === riga.ff_cod.toString()) {
                        esisteGia = true;
                        break;
                    }
                }

                if (esisteGia)
                    continue;

                var o = {
                    Codice: "i_" + i.toString(),
                    Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                    piva: impiantiSelezionati[i].Piva,
                    sa_cod: impiantiSelezionati[i].Sa_Cod,
                    appezza: impiantiSelezionati[i].Appezza,
                    id_reg: impiantiSelezionati[i].ID_Reg,
                    Data: Data,
                    av_cod: 0,
                    udm_cod: 0,
                    ff_cod: ff_cod ,
                    Descrizione: Descrizione,
                    Dato: "",
                    Dato_Testuale: "",
                    Cul_Des: impiantiSelezionati[i].Cul_Des,
                    Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                    lav_cod: LAVCOD_FASIFENOLOGICHE,
                    Operazione: Traduzione(rilieviBSResxLocal, "RilieviFasiFenologiche", "Rilievi Fasi Fenologiche"),
                    Veg_Cod: veg_cod,
                    ind_mat_cod: 0,
                    dr_cod: 0
                };

                AggiungiRilievoRiga(grid, o);

            } //impiantiSelezionati

        }
    }
}

function AggiungiRilievoErbeInfestanti() {

    var grid = $("#divRilievi").data("kendoGrid");

    //if (grid) {

    //    var Data = "";

    //    if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
    //        Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
    //    } else {
    //        Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
    //    }

    //    var Descrizione = $("#comboRilievoErbeInfestanti").data("kendoDropDownList").text();
    //    var DatoRilevato = $("#txtDataRilevataFasiFenologiche").val();

    //    var DatoRilevatoTestuale = DatoRilevato;
    //    if ($("#comboQtaRilevataErbeInfestanti").data('kendoDropDownList') !== undefined && $("#comboQtaRilevataErbeInfestanti").data('kendoDropDownList').text() !== "")
    //        DatoRilevatoTestuale = $("#comboQtaRilevataErbeInfestanti").data('kendoDropDownList').text();

    //    var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

    //    var rilievoSelezionato = $("#comboRilievoFasiFenologiche").data("kendoDropDownList").value();
    //    var vRilievoSelezionato = rilievoSelezionato.split("|");

    //    //Controllo che i dati siano stati impostati
    //    if (Data === undefined || Data === "") {
    //        MessaggioErrore("Specificare una data valida", "DIV_Messaggi");
    //        return;
    //    }
    //    if (Descrizione === undefined || Descrizione === "") {
    //        MessaggioErrore("Specificare una fase fenologica", "DIV_Messaggi");
    //        return;
    //    }
    ////    if (DatoRilevato === undefined || DatoRilevato === "") {
    ////        MessaggioErrore("Specificare una data valida", "DIV_Messaggi");
    ////        return;
    ////    }
    //    if (kendo.parseDate(DatoRilevato) === null) {
    //        MessaggioErrore("Specificare una data valida", "DIV_Messaggi");
    //        return;
    //    }


    //    if (impiantiSelezionati === undefined || impiantiSelezionati.length <= 0) {
    //        MessaggioErrore("Selezionare almeno un impianto", "DIV_Messaggi");
    //        return;
    //    }

    //    var veg_cod = $(comboSpecie_clientID).val();

    //    //Controllo che il rilievo non sia già stato inserito per quegli impianti nella stessa operazione
    //    for (var h = 0; h < impiantiSelezionati.length; h++) {
    //        var piva = impiantiSelezionati[h].Piva.toString();
    //        var sa_cod = impiantiSelezionati[h].Sa_Cod.toString();
    //        var appezza = impiantiSelezionati[h].Appezza.toString();
    //        var id_reg = impiantiSelezionati[h].ID_Reg.toString();

    //        var dataSource = grid.dataSource.data();

    //        for (var j = 0; j < dataSource.length; j++) {
    //            var riga = dataSource[j];

    //            if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && vRilievoSelezionato[0].toString() === riga.ff_cod.toString()) {

    //                MessaggioErrore("Errore: esiste già un rilievo '" + Descrizione + "' per l'impianto '" + ImpiantoRicavaDescrizione(impiantiSelezionati[h]) + "'", "DIV_Messaggi");
    //                return;
    //            }

    //            if (riga.Veg_Cod.toString() !== "0" && veg_cod !== "0" && veg_cod !== riga.Veg_Cod.toString()) {

    //                MessaggioErrore("Errore: Impossibile aggiungere rilievi per più di una specie vegetale. Creare una nuova Operazione per la seconda specie.", "DIV_Messaggi");
    //                return;
    //            }

    //        }
    //    }

    //    //Aggiungo i rilievi
    //    for (var i = 0; i < impiantiSelezionati.length; i++) {

    //        var o = {
    //            Codice: "i_" + i.toString(),
    //            Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
    //            piva: impiantiSelezionati[i].Piva,
    //            sa_cod: impiantiSelezionati[i].Sa_Cod,
    //            appezza: impiantiSelezionati[i].Appezza,
    //            id_reg: impiantiSelezionati[i].ID_Reg,
    //            Data: Data,
    //            av_cod: 0,
    //            udm_cod: 0,
    //            ff_cod: vRilievoSelezionato[0],
    //            Descrizione: Descrizione,
    //            Dato: DatoRilevato,
    //            Dato_Testuale: DatoRilevatoTestuale,
    //            Cul_Des: impiantiSelezionati[i].Cul_Des,
    //            Qta2: impiantiSelezionati[i].Qta2.toString.replace(",","."),
    //            lav_cod: LAVCOD_FASIFENOLOGICHE,
    //            Operazione: "Rilievi Fasi Fenologiche",
    //            Veg_Cod: veg_cod,
    //            ind_mat_cod: 0,
    //            dr_cod: 0
    //        };

    //        AggiungiRilievoRiga(grid, o);

    //    } //impiantiSelezionati
    //}
}

function AggiungiRilievoErbeInfestanti_PerPreset(listaPreset) {}

function AggiungiRilievoIndiciReseRaccolta() {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        var Descrizione = $("#comboRilievoIndiciReseRaccolta").data("kendoDropDownList").text();
        var DatoRilevato = $("#txtQtaRilevataIndiciReseRaccolta").val();

        var DatoRilevatoTestuale = DatoRilevato;
        if ($("#comboQtaRilevataIndiciReseRaccolta").data('kendoDropDownList') !== undefined && $("#comboQtaRilevataIndiciReseRaccolta").data('kendoDropDownList').text() !== "")
            DatoRilevatoTestuale = $("#comboQtaRilevataIndiciReseRaccolta").data('kendoDropDownList').text();

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var rilievoSelezionato = $("#comboRilievoIndiciReseRaccolta").data("kendoDropDownList").value();
        var vRilievoSelezionato = rilievoSelezionato.split("|");

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }
        if (Descrizione === undefined || Descrizione === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnRilievo", "Specificare un rilievo"), "DIV_Messaggi");
            return;
        }
        //if (DatoRilevato === undefined || DatoRilevato === "") {
        //    MessaggioErrore("Specificare una quantità rilevata valida", "DIV_Messaggi");
        //    return;
        //}
        if (impiantiSelezionati === undefined || impiantiSelezionati.length <= 0) {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SelezionareAlmenoUnImpianto", "Selezionare almeno un impianto"), "DIV_Messaggi");
            return;
        }

        var veg_cod = $(comboSpecie_clientID).val();
        var dataSource = grid.dataSource.data();

        //Controllo che il rilievo non sia già stato inserito per quegli impianti nella stessa operazione
        for (var h = 0; h < impiantiSelezionati.length; h++) {
            var piva = impiantiSelezionati[h].Piva.toString();
            var sa_cod = impiantiSelezionati[h].Sa_Cod.toString();
            var appezza = impiantiSelezionati[h].Appezza.toString();
            var id_reg = impiantiSelezionati[h].ID_Reg.toString();

            for (var j = 0; j < dataSource.length; j++) {
                var riga = dataSource[j];

                if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && vRilievoSelezionato[0].toString() === riga.ind_mat_cod.toString()) {
                    MessaggioErrore(
                        kendo.format(Traduzione(rilieviBSResxLocal, "ErroreEsisteGiàRilievoPerImpianto", "Errore: esiste già un rilievo '{0}' per l'impianto '{1}'"), Descrizione, ImpiantoRicavaDescrizione(impiantiSelezionati[h])),
                        "DIV_Messaggi");
                    return;
                }

                if (riga.Veg_Cod.toString() !== "0" && veg_cod !== "0" && veg_cod !== riga.Veg_Cod.toString()) {
                    MessaggioErrore(
                        Traduzione(rilieviBSResxLocal, "ErroreImpossibileAggiungereRilieviSecondaSpecie", "Errore: Impossibile aggiungere rilievi per più di una specie vegetale. Creare una nuova Operazione per la seconda specie."),
                        "DIV_Messaggi");
                    return;
                }

            }
        }

        //Aggiungo i rilievi
        for (var i = 0; i < impiantiSelezionati.length; i++) {

            var o = {
                Codice: "i_" + i.toString(),
                Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                piva: impiantiSelezionati[i].Piva,
                sa_cod: impiantiSelezionati[i].Sa_Cod,
                appezza: impiantiSelezionati[i].Appezza,
                id_reg: impiantiSelezionati[i].ID_Reg,
                Data: Data,
                av_cod: 0,
                udm_cod: vRilievoSelezionato[1],
                ff_cod: 0,
                Descrizione: Descrizione,
                Dato: DatoRilevato,
                Dato_Testuale: DatoRilevatoTestuale,
                Cul_Des: impiantiSelezionati[i].Cul_Des,
                Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                lav_cod: LAVCOD_RILIEVOINDICIRESERACCOLTA,
                Operazione: Traduzione(rilieviBSResxLocal, "RilieviIndiciReseRaccolta", "Rilievi Indici Rese/Raccolta"),
                Veg_Cod: veg_cod,
                ind_mat_cod: vRilievoSelezionato[0],
                dr_cod: 0
            };

            AggiungiRilievoRiga(grid, o);

        } //impiantiSelezionati

    }
}

function AggiungiRilievoIndiciReseRaccolta_PerPreset(listaPreset) {

    var grid = $("#divRilievi").data("kendoGrid");

    if (grid) {

        var Data = "";

        if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
            Data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
        } else {
            Data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
        }

        //Controllo che i dati siano stati impostati
        if (Data === undefined || Data === "") {
            MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareUnaDataValida", "Specificare una data valida"), "DIV_Messaggi");
            return;
        }

        var impiantiSelezionati = JSON.parse($(id_hdKendo_Impianti_Selezione).val());

        var dataSource = grid.dataSource.data();
        var veg_cod = $(comboSpecie_clientID).val();

        for (var j = 0; j < listaPreset.length; j++) {

            var Descrizione = listaPreset[j].Descrizione;
            var ind_mat_cod = listaPreset[j].ind_mat_cod.toString();
            var udm_cod = listaPreset[j].udm_cod;

            //Aggiungo i rilievi
            for (var i = 0; i < impiantiSelezionati.length; i++) {

                var piva = impiantiSelezionati[i].Piva.toString();
                var sa_cod = impiantiSelezionati[i].Sa_Cod.toString();
                var appezza = impiantiSelezionati[i].Appezza.toString();
                var id_reg = impiantiSelezionati[i].ID_Reg.toString();

                //Se il rilievo esiste già, non lo aggiungo
                var esisteGia = false;
                for (var h = 0; h < dataSource.length; h++) {
                    var riga = dataSource[h];
                    if (piva === riga.piva.toString() && sa_cod === riga.sa_cod.toString() && appezza === riga.appezza.toString() && id_reg === riga.id_reg.toString() && ind_mat_cod.toString() === riga.ind_mat_cod.toString()) {
                        esisteGia = true;
                        break;
                    }
                }

                if (esisteGia)
                    continue;

                var o = {
                    Codice: "i_" + i.toString(),
                    Impianto: ImpiantoRicavaDescrizione(impiantiSelezionati[i]),
                    piva: impiantiSelezionati[i].Piva,
                    sa_cod: impiantiSelezionati[i].Sa_Cod,
                    appezza: impiantiSelezionati[i].Appezza,
                    id_reg: impiantiSelezionati[i].ID_Reg,
                    Data: Data,
                    av_cod: 0,
                    udm_cod: udm_cod,
                    ff_cod: 0,
                    Descrizione: Descrizione,
                    Dato: "",
                    Dato_Testuale: "",
                    Cul_Des: impiantiSelezionati[i].Cul_Des,
                    Qta2: impiantiSelezionati[i].Qta2.toString().replace(",", "."),
                    lav_cod: LAVCOD_RILIEVOINDICIRESERACCOLTA,
                    Operazione: Traduzione(rilieviBSResxLocal, "RilieviIndiciReseRaccolta", "Rilievi Indici Rese/Raccolta"),
                    Veg_Cod: veg_cod,
                    ind_mat_cod: ind_mat_cod,
                    dr_cod: 0
                };

                AggiungiRilievoRiga(grid, o);

            } //impiantiSelezionati

        }

    }
}

function ImpiantoRicavaDescrizione(impianto) {

    return $.grep([impianto.Sa_Nome, impianto.Campo_Des, impianto.App_Nome, impianto.Cul_Des], Boolean).join(" - ");

}

function AggiungiRilievoRiga(grid, o) {

    var lav_cod = $(hdLav_Cod_clientID).val();
    o.Descrizione_Unica = (lav_cod === LAVCOD_VISITA) ? o.Operazione + " - <i>" + o.Impianto + "</i> - " + o.Descrizione : "<i>" + o.Impianto + "</i> - " + o.Descrizione;

    //this logic creates a new item in the datasource/datagrid
    var dataSource = grid.dataSource;
    var total = dataSource.data().length;
    dataSource.insert(total, o);
    dataSource.page(dataSource.totalPages());
    //grid.editRow(grid.tbody.children().last());

}

function ComboCategorieVisiteLiv1Popola(e) {

    var param = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });

    ajaxAgronica(pathCoreWS + "AgronicaCoreVisite/Visite.asmx/LeggiCategorieVisiteLiv1",
        param,
        function (risposta) {
            var lista = JSON.parse(risposta.RispostaStringa);
            e.success(lista);
        }, null);

}

function ComboCategorieVisiteLiv2Popola(e) {

    var param = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreVisite/Visite.asmx/LeggiCategorieVisiteLiv2",
        param, true,
        function (risposta) {
            var lista = JSON.parse(risposta.RispostaStringa);
            e.success(lista);
        }, null);

}

function ComboRilievoAvversitaInCampoPopola(e) {
    ComboRilievoPopola(e, LAVCOD_RILIEVOAVVERSITAINCAMPO, IMPOSTAZIONE_PERSON_RILIEVO_AVVERSITA);
}

function ComboRilievoErbeInfestantiPopola(e) {
    ComboRilievoPopola(e, LAVCOD_RILIEVOERBEINFESTANTI, IMPOSTAZIONE_PERSON_RILIEVO_ERBE_INFESTANTI);
}

function ComboRilievoIndiciMaturitaPopola(e) {
    ComboRilievoPopola(e, LAVCOD_RILIEVOINDICIMATURITA, IMPOSTAZIONE_PERSON_RILIEVO_INDICI_MATURITA);
}

function ComboRilievoDanniRaccoltaPopola(e) {
    ComboRilievoPopola(e, LAVCOD_RILIEVODANNIALLARACCOLTA, IMPOSTAZIONE_PERSON_RILIEVO_DANNI_RACCOLTA);
}

function ComboRilievoFasiFenologichePopola(e) {
    ComboRilievoPopola(e, LAVCOD_FASIFENOLOGICHE, IMPOSTAZIONE_PERSON_RILIEVO_FASI_FENOLOGICHE);
}

function ComboRilievoIndiciReseRaccoltaPopola(e) {
    ComboRilievoPopola(e, LAVCOD_RILIEVOINDICIRESERACCOLTA, IMPOSTAZIONE_PERSON_RILIEVO_INDICI_RESE_RACCOLTA);
}

function ComboRilievoPopola(e, lavCod, impostazione_cod) {

    var specie = $(comboSpecie_clientID).val();

    //Controllo per il caso dei terreni nudi e dei valori strani...
    if (!$.isNumeric(specie)) {
        return;
    }

    var dpiCod = "0";
    var idRcdpi = "0";
    var dpiPubblicoPrivato = "0";
    var personalizzate = false;

    $(labelSoglie_clientID).hide();

    var DPI_selezionato = $(comboDisciplinari_clientID).val();
    if (DPI_selezionato !== undefined && DPI_selezionato !== "0") {

        var vDPI_selezionato = DPI_selezionato.split("/");

        dpiCod = vDPI_selezionato[0];
        if (vDPI_selezionato.length > 4) {
            idRcdpi = vDPI_selezionato[1];
            dpiPubblicoPrivato = vDPI_selezionato[4];
            $(labelSoglie_clientID).show();
        }
    }

    //Leggo l'impostazione utente personalizzate
    var parametri = kendo.stringify({ "objP_Utenti": objP_utenti, "impostazione_cod": impostazione_cod });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_Impostazioni",
        parametri, true,
        function (risposta) {
            var res = risposta.RispostaStringa;
            if (res === "1")
                personalizzate = true;

            var paramList = "{ lavCod: '" + lavCod +
                "', vegCod: '" + specie +
                "', dpiCod: '" + dpiCod +
                "', idRcdpi: '" + idRcdpi +
                "', dpiPubblicoPrivato: '" + dpiPubblicoPrivato +
                "', personalizzate: " + personalizzate +
                " , objP_Super_Server: '" + objP_super_server +
                "', objP_Server: '" + objP_server + 
                "', objP_Utenti: '" + objP_utenti + "'}";

            ajaxAgronicaSync(pathCoreWS + "Metaschema/Rilievi.asmx/PopolaRilievo",
                paramList, true,
                function (risposta) {
                    var lista = JSON.parse(risposta.RispostaStringa);
                    e.success(lista);
                }, null);

        }, null);

}


function CaricaValoriComboMisuraXAvversitaInputData() {

    var rilievoSelezionato = $("#comboRilievoAvversitaInCampo").data("kendoDropDownList").value();
    var vRilievoSelezionato = rilievoSelezionato.split("|");
    var av_cod = vRilievoSelezionato[0];
    var av_gru = vRilievoSelezionato[1];
    var udm_cod = vRilievoSelezionato[2];
    var veg_cod = $(comboSpecie_clientID).val();

    //Controllo che i campi selezionati abbiano valore
    if (!$.isNumeric(av_cod) || !$.isNumeric(veg_cod)) {
        return;
    }

    var param = kendo.stringify({ 'MxAV_Cod': '0', 'av_cod': av_cod, 'udm_cod': udm_cod, 'veg_cod': veg_cod });

    ajaxAgronica(indirizzohttp + "/CaricaComboMisuraXAvversitaInputData", param,
        function (risposta) {

            var x = risposta.RispostaStringa;

            var ds = JSON.parse(x);

            //Pulisco la textbox
            $('#txtQtaRilevataAvversitaInCampo').val("");

            if (ds.length > 0) {

                //Nascondo la textbox
                $('#txtQtaRilevataAvversitaInCampo').hide();

                //Creo la combo
                $('#comboQtaRilevataAvversitaInCampo').kendoDropDownList({
                    dataSource: ds,
                    dataTextField: "anag_des",
                    dataValueField: "anag_valore",
                    autoWidth: true,
                    change: function (e) {
                        //Assegno il valore della combo nella textbox così salvo sempre quello
                        $('#txtQtaRilevataAvversitaInCampo').val(e.sender.value());
                    },
                    dataBound: function (e) {
                        //Assegno il valore della combo nella textbox così salvo sempre quello
                        $('#txtQtaRilevataAvversitaInCampo').val(e.sender.value());
                    }
                });

                //Mostro la dropdown
                let dropdownlist = $("#comboQtaRilevataAvversitaInCampo").data("kendoDropDownList");
                //dropdownlist.wrapper.show();
                $(dropdownlist.wrapper).removeAttr('style', 'display:none !important');

            } else {

                let dropdownlist = $("#comboQtaRilevataAvversitaInCampo").data("kendoDropDownList");
                if (dropdownlist !== undefined && dropdownlist !== null) {
                    //Pulisco la combo
                    $('#comboQtaRilevataAvversitaInCampo').kendoDropDownList({});
                    //Nascondo la combo
                    //dropdownlist.wrapper.hide();
                    $(dropdownlist.wrapper).attr('style', 'display:none !important');
                }

                //Mostro la textbox
                $('#txtQtaRilevataAvversitaInCampo').show();

            }

        }, null);

}

function CaricaValoriComboMisuraXIndiciMaturitaInputData() {

    var rilievoSelezionato = $("#comboRilievoIndiciMaturita").data("kendoDropDownList").value();
    var vRilievoSelezionato = rilievoSelezionato.split("|");
    var ind_mat_cod = vRilievoSelezionato[0];
    var udm_cod = vRilievoSelezionato[1];
    var veg_cod = $(comboSpecie_clientID).val();

    //Controllo che i campi selezionati abbiano valore
    if (!$.isNumeric(ind_mat_cod) || !$.isNumeric(veg_cod)) {
        return;
    }
    var param = kendo.stringify({ 'Ind_Mat_Cod': '0',  'udm_cod': udm_cod, 'veg_cod': veg_cod });

    //var param = kendo.stringify({ 'Ind_Mat_Cod': '0', 'ind_mat_cod': ind_mat_cod, 'udm_cod': udm_cod, 'veg_cod': veg_cod });

    ajaxAgronica(indirizzohttp + "/CaricaComboMisuraXIndiciMaturitaInputData", param,
        function (risposta) {

            var x = risposta.RispostaStringa;

            var ds = JSON.parse(x);

            //Pulisco la textbox
            $('#txtQtaRilevataIndiciMaturita').val("");

            if (ds.length > 0) {

                //Nascondo la textbox
                $('#txtQtaRilevataIndiciMaturita').hide();

                //Creo la combo
                $('#comboQtaRilevataIndiciMaturita').kendoDropDownList({
                    dataSource: ds,
                    dataTextField: "anag_des",
                    dataValueField: "anag_valore",
                    autoWidth: true,
                    change: function (e) {
                        //Assegno il valore della combo nella textbox così salvo sempre quello
                        $('#txtQtaRilevataIndiciMaturita').val(e.sender.value());
                    },
                    dataBound: function (e) {
                        //Assegno il valore della combo nella textbox così salvo sempre quello
                        $('#txtQtaRilevataIndiciMaturita').val(e.sender.value());
                    }
                });

                //Mostro la dropdown
                let dropdownlist = $("#comboQtaRilevataIndiciMaturita").data("kendoDropDownList");
                //dropdownlist.wrapper.show();
                $(dropdownlist.wrapper).removeAttr('style', 'display:none !important');

            } else {

                let dropdownlist = $("#comboQtaRilevataIndiciMaturita").data("kendoDropDownList");
                if (dropdownlist !== undefined && dropdownlist !== null) {
                    //Pulisco la combo
                    $('#comboQtaRilevataIndiciMaturita').kendoDropDownList({});
                    //Nascondo la combo
                    //dropdownlist.wrapper.hide();
                    $(dropdownlist.wrapper).attr('style', 'display:none !important');
                }

                //Mostro la textbox
                $('#txtQtaRilevataIndiciMaturita').show();

            }

        }, null);

}
