const larghezzaStdCampoNumerico = 120;
const larghezzaStdCampoNumericoRend = 145;

$("#btn_carica_elenco").click(
    async function () {
        if (QS_Avanzamento !== 2 || (QS_Avanzamento === 2 && !(isNaN(parseInt($('#anno')[0].value)) || (KendoDDL("ddlLivelloDettaglio").value() == 1 &&
            (KendoDDL("ddlAllevamento").value() !== "0" && (KendoDDL("ddlLavorazione").value() > 0 || KendoDDL("ddlColtura").value() > 0)))))) {
            ConfiguraGrigliaElenco("tab_griglia_elencoRichieste");
        } else {
            if (isNaN(parseInt($('#anno')[0].value))) {
                kendo.alert("Inserire l'anno");
            } else {
                kendo.alert("Non è consentito specificare sia un allevamento sia una coltura o lavorazione");
            }
        }
            
    });

//calcola il totale di una colonna da mettere nel footer della stessa
function calcTotaleColonna(field, idGriglia) {

    if (field !== undefined) {

        var grid = $("#tab_griglia_elencoRichieste").data("kendoGrid");
        var dataSource = grid.dataSource;

        var filteredDataSource = new kendo.data.DataSource({
            data: dataSource.data(),
            filter: dataSource.filter()
        });

        filteredDataSource.read();
        var datiFiltrati = filteredDataSource.view();

        var totale = 0;

        $.each(datiFiltrati, function (index, model) {
            if (model.get(field) !== undefined && model.get(field) !== null && (model.deleted === undefined || model.deleted === false)) {
                totale += model.get(field);
            }
        });

        return totale.toFixed(0);
    }
}

function ConfiguraGrigliaElenco(IDControllo) {
    checkPerc($("#legendaPerc"));
    var omettiAnnulla = true;
    var funzioneSubmitDaUsare = null;

    funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti, flagInsert: false, flagUpdate: true,*/ flagDelete: true };

    var funzioniCRUD = {
        funzioneRead: (QS_Avanzamento === 1) ? ElencoRendicontazioni : ((QS_Avanzamento === 0) ? ElencoRichieste : RicercaMacrousiLavorazioni),
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: omettiAnnulla
    };
    var idModel = "ID";
    var campiKendoModel = null;

    campiKendoModel = {
        ID: { editable: false, type: "string", validation: { required: true } },
        CUAA: { editable: false, type: "string", validation: { required: true } },
        piva: { editable: false, type: "string", validation: { required: true } },
        azienda: { editable: false, type: "string", validation: { required: true } },
        richiestaCod: { editable: false, type: "string", validation: { required: true } },
        citta: { editable: false, type: "string", validation: { required: true } },
        Prov: { editable: false, type: "string", validation: { required: true } },
        via: { editable: false, type: "string", validation: { required: true } },
        CAP: { editable: false, type: "string", validation: { required: true } },
        calcolato_Gasolio: { editable: false, type: "number", validation: { required: true } },
        calcolato_Benzina: { editable: false, type: "number", validation: { required: true } },
        calcolato_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },
        richiesto_Gasolio: { editable: false, type: "number", validation: { required: true } },
        richiesto_Benzina: { editable: false, type: "number", validation: { required: true } },
        richiesto_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },
        Carburante_Richiesto_Gasolio_Allevamenti: { editable: false, type: "number", validation: { required: true } },
        Carburante_Richiesto_Benzina_Allevamenti: { editable: false, type: "number", validation: { required: true } },
        assegnato_gasolio: { editable: false, type: "number", validation: { required: true } },
        assegnato_benzina: { editable: false, type: "number", validation: { required: true } },
        assegnato_gasolio_serra: { editable: false, type: "number", validation: { required: true } },
        Carburante_Approvato_Gasolio_Allevamenti: { editable: false, type: "number", validation: { required: true } },
        Carburante_Approvato_Benzina_Allevamenti: { editable: false, type: "number", validation: { required: true } },
        nRichiesta: { editable: false, type: "number", validation: { required: true } },
        annoRichiesta: { editable: false, type: "number", validation: { required: true } },
        CodstatoAv: { editable: false, type: "number", validation: { required: true } },
        statoAv: { editable: false, type: "string", validation: { required: true } },
        Pratica_Cod: { editable: false, type: "number", validation: { required: true } },
        Data_Creazione: { editable: false, type: "date", validation: { required: true } },
        ultimo_Avanzamento: { editable: false, type: "date", validation: { required: true } },
        //val_Inizio: { editable: false, type: "Date", validation: { required: true }, defaultValue: new Date(1900, 1, 1, 0, 0) },
        //val_Fine: { editable: false, type: "Date", validation: { required: true }, defaultValue: new Date(2100, 12, 31, 0, 0) },
        tipo_richiesta: { editable: false, type: "string", validation: { required: true } },
        Prima_Richiesta: { editable: false, type: "string", validation: { required: true } },
        Ispettore: { editable: false, type: "string", validation: { required: true } },
        Litri_In_Esubero: { editable: false, type: "string", validation: { required: true } },
        Acquistato_e_Rimanenza_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Richiesto_Netto_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Acquistato_e_Rimanenza_Benzina: { editable: false, type: "number", validation: { required: true } },
        Richiesto_Netto_Benzina: { editable: false, type: "number", validation: { required: true } },
        Acquistato_e_Rimanenza_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },
        Richiesto_Netto_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        //Anna 12/08/21: Aggiunte due nuove colonne per UMA
        Richiedente: { editable: false, type: "string", validation: { required: true } },
        Approvatore: { editable: false, type: "string", validation: { required: true } },

        Tipo_Azienda: { editable: false, type: "number", validation: { required: true } },
        Tipo_Azienda_Des: { editable: false, type: "string", validation: { required: true } },

        //Campi aggiuntivi gestione rimanenze
        Rec_Acc_Conf_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Rec_Acc_Conf_Benzina: { editable: false, type: "number", validation: { required: true } },
        Rec_Acc_Conf_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        //Prima data rilascio pratica
        Data_Rilascio: { editable: false, type: "date", validation: { required: true } },

        Tipo_Pratica: { editable: false, type: "string", validation: { required: true } },
        TipoRiga: { editable: false, type: "string", validation: { required: true } },
        DescrizioneRiga: { editable: false, type: "string", validation: { required: true } },
        Lavorazione: { editable: false, type: "string", validation: { required: true } },
        calcolato_Gasolio_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        calcolato_Benzina_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        calcolato_Gasolio_Serra_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        richiesto_Gasolio_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        richiesto_Benzina_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        richiesto_Gasolio_Serra_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        assegnato_gasolio_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        assegnato_benzina_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        assegnato_gasolio_serra_Dettaglio: { editable: false, type: "number", validation: { required: true } },
        PartitaIvaReale: { editable: false, type: "string", validation: { required: true } },

    };

    var styleElen = /*"background-color: #C4C4EF; */"text-align: center; vertical-align: top";

    var colonneIndirizzo = [];
    colonneIndirizzo.push({ field: "Prov", title: TraduzioneMultiResx(gestioneCarbResx, "Prov", "Prov."), width: 95, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneIndirizzo.push({ field: "citta", title: TraduzioneMultiResx(gestioneCarbResx, "citta", "Citta'"), width: 95, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneIndirizzo.push({ field: "CAP", title: TraduzioneMultiResx(gestioneCarbResx, "CAP", "CAP"), width: 95, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneIndirizzo.push({ field: "via", title: TraduzioneMultiResx(gestioneCarbResx, "via", "Via"), width: 125, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });

    var colonneAllevamenti = [
        { field: "Carburante_Richiesto_Gasolio_Allevamenti", title: TraduzioneMultiResx(gestioneCarbResx, "Carburante_Richiesto_Gasolio_Allevamenti", "Carburante Richiesto Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Carburante_Approvato_Gasolio_Allevamenti", title: TraduzioneMultiResx(gestioneCarbResx, "Carburante_Approvato_Gasolio_Allevamenti", "Carburante Assegnato Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "Carburante_Richiesto_Benzina_Allevamenti", title: TraduzioneMultiResx(gestioneCarbResx, "Carburante_Richiesto_Benzina_Allevamenti", "Carburante Richiesto Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Carburante_Approvato_Benzina_Allevamenti", title: TraduzioneMultiResx(gestioneCarbResx, "Carburante_Approvato_Benzina_Allevamenti", "Carburante Assegnato Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
    ]

    var colonneKendoGrid = [
        { field: "nRichiesta", title: TraduzioneMultiResx(gestioneCarbResx, "nRichiesta", "Numero richiesta"), width: 100, headerAttributes: { style: styleElen } },
        { field: "annoRichiesta", title: TraduzioneMultiResx(gestioneCarbResx, "annoRichiesta", "Anno Richiesta"), width: 100, headerAttributes: { style: styleElen } },
        { field: "tipo_richiesta", title: TraduzioneMultiResx(gestioneCarbResx, "tipo_richiesta", "Tipo Richiesta"), width: 110, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "statoAv", title: TraduzioneMultiResx(gestioneCarbResx, "statoAv", "Stato avanzamento pratica"), width: 135, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Prima_Richiesta", title: TraduzioneMultiResx(gestioneCarbResx, "Prima_Richiesta", "Richiesta iniz. - integr."), width: 135, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "azienda", title: TraduzioneMultiResx(gestioneCarbResx, "azienda", "Azienda"), width: 250, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Tipo_Azienda_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Tipo Azienda", "Tipo Azienda"), width: 180, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "ID", title: TraduzioneMultiResx(gestioneCarbResx, "ID", "Piva"), width: 110, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "calcolato_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "calcolato_Gasolio", "Carburante Calcolato Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "calcolato_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "calcolato_Benzina", "Carburante Calcolato Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "calcolato_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "calcolato_Gasolio_Serra", "Carburante Calcolato Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "richiesto_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto_Gasolio", "Carburante Richiesto Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "richiesto_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto_Benzina", "Carburante Richiesto Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "richiesto_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto_Gasolio_Serra", "Carburante Richiesto Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "assegnato_gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_gasolio", "Carburante Assegnato Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "assegnato_benzina", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_benzina", "Carburante Assegnato Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "assegnato_gasolio_serra", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_gasolio_serra", "Carburante Assegnato Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        //{ title: "Allevamenti", headerAttributes: { style: styleElen }, columns: colonneAllevamenti },
        { field: "Data_Creazione", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Creazione", "Data Apertura Richiesta"), format: "{0:dd/MM/yyyy}", width: 100, headerAttributes: { style: styleElen } },
    ];
    if (QS_Avanzamento !== 2) {
        colonneKendoGrid.push({ field: "Data_Rilascio", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Rilascio", "Data Rilascio"), format: "{0:dd/MM/yyyy}", width: 100, headerAttributes: { style: styleElen } });
    }

    colonneKendoGrid.push({ field: "ultimo_Avanzamento", title: TraduzioneMultiResx(gestioneCarbResx, "ultimo_Avanzamento", "Data Ultimo Avanzamento"), format: "{0:dd/MM/yyyy}", width: 100, headerAttributes: { style: styleElen } });
        //colonneKendoGrid.push(//{ field: "Ispettore", title: TraduzioneMultiResx(gestioneCarbResx, "Ispettore", "Ispettore"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
    colonneKendoGrid.push({ field: "Prov", title: TraduzioneMultiResx(gestioneCarbResx, "Prov", "Prov."), width: 70, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneKendoGrid.push({ field: "citta", title: TraduzioneMultiResx(gestioneCarbResx, "citta", "Citta'"), width: 120, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneKendoGrid.push({ field: "CAP", title: TraduzioneMultiResx(gestioneCarbResx, "CAP", "CAP"), width: 70, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneKendoGrid.push({ field: "via", title: TraduzioneMultiResx(gestioneCarbResx, "via", "Via"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
        //colonneKendoGrid.push(//{ title: TraduzioneMultiResx(gestioneCarbResx, "indirizzo", "Indirizzo"), headerAttributes: { style: "text-align: center;" }, columns: colonneIndirizzo },
        //colonneKendoGrid.push(//{ field: "val_Inizio", title: TraduzioneMultiResx(gestioneCarbResx, "val_Inizio", "Validita Inizio"), format: "{0:dd/MM/yyyy}", width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        //colonneKendoGrid.push(//{ field: "val_Fine", title: TraduzioneMultiResx(gestioneCarbResx, "val_Fine", "Validita Fine"), format: "{0:dd/MM/yyyy}", width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        //colonneKendoGrid.push(//Anna 12/08/21: Aggiunte due nuove colonne per UMA
    colonneKendoGrid.push({ field: "Richiedente", title: TraduzioneMultiResx(gestioneCarbResx, "Richiedente", "Richiedente"), width: 125, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneKendoGrid.push({ field: "Approvatore", title: TraduzioneMultiResx(gestioneCarbResx, "Approvatore", "Approvatore"), width: 125, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    

    if (QS_Avanzamento == 1) {
        colonneKendoGrid.push({ field: "Richiesto_Netto_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Netto_Gasolio", "Rendicontato netto Gasolio"), width: larghezzaStdCampoNumericoRend, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "Acquistato_e_Rimanenza_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_e_Rimanenza_Gasolio", "Acquistato + Rimanenza anno precedente - Rimanenza riass. in rendicontazione (Gasolio)"), width: larghezzaStdCampoNumericoRend, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "Richiesto_Netto_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Netto_Benzina", "Rendicontato netto Benzina"), width: larghezzaStdCampoNumericoRend, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "Acquistato_e_Rimanenza_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_e_Rimanenza_Benzina", "Acquistato + Rimanenza anno precedente - Rimanenza riass. in rendicontazione (Benzina)"), width: larghezzaStdCampoNumericoRend, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "Richiesto_Netto_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Netto_Gasolio_Serra", "Rendicontato netto Gasolio Serra"), width: larghezzaStdCampoNumericoRend, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "Acquistato_e_Rimanenza_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_e_Rimanenza_Gasolio_Serra", "Acquistato + Rimanenza anno precedente - Rimanenza riass. in rendicontazione (Gasolio Serra)"), width: larghezzaStdCampoNumericoRend, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "Litri_In_Esubero", title: TraduzioneMultiResx(gestioneCarbResx, "Litri_In_Esubero", "Litri In Esubero o Recupero Accise"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    } else if (QS_Avanzamento == 0) {
        colonneKendoGrid.push({ field: "Rendicontazine_Originale", title: TraduzioneMultiResx(gestioneCarbResx, "Rendicontazine_Originale", "Rendicontazione Originale"), width: 125, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    } else {
        colonneKendoGrid.unshift({ field: "Tipo_Pratica", title: TraduzioneMultiResx(gestioneCarbResx, "Tipo_Pratica", "Tipo Pratica"), width: 125, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    }

    if (QS_Avanzamento !== 2) {
        colonneKendoGrid.push({ field: "Rec_Acc_Conf_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Rec_Acc_Conf_Gasolio", "Rec. Accise Confermato Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "Rec_Acc_Conf_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Rec_Acc_Conf_Benzina", "Rec. Accise Confermato Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "Rec_Acc_Conf_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Rec_Acc_Conf_Gasolio_Serra", "Rec. Accise Confermato Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
    } else if ($('#ddlLivelloDettaglio').val() == 1) {
        colonneKendoGrid.push({ field: "TipoRiga", title: TraduzioneMultiResx(gestioneCarbResx, "TipoRiga", "Tipo Dettaglio"), width: 125, headerAttributes: { style: styleElen } });
        if (KendoDDL("ddlColtura").value() > 0 || KendoDDL("ddlAllevamento").value() != "0") {
            colonneKendoGrid.push({ field: "DescrizioneRiga", title: TraduzioneMultiResx(gestioneCarbResx, "DescrizioneRiga", "Descrizione"), width: 125, headerAttributes: { style: styleElen } });
        }
        if (KendoDDL("ddlColtura").value() > 0 || KendoDDL("ddlLavorazione").value() > 0) {
            colonneKendoGrid.push({ field: "Lavorazione", title: TraduzioneMultiResx(gestioneCarbResx, "Lavorazione", "Lavorazione"), width: 125, headerAttributes: { style: styleElen } });
        }
        colonneKendoGrid.push({ field: "calcolato_Gasolio_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "calcolato_Gasolio_Dettaglio", "Carburante Calcolato Gasolio Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "calcolato_Benzina_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "calcolato_Benzina_Dettaglio", "Carburante Calcolato Benzina Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "calcolato_Gasolio_Serra_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "calcolato_Gasolio_Serra_Dettaglio", "Carburante Calcolato Gasolio Serra Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "richiesto_Gasolio_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto_Gasolio_Dettaglio", "Carburante Richiesto Gasolio Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "richiesto_Benzina_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto_Benzina_Dettaglio", "Carburante Richiesto Benzina Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "richiesto_Gasolio_Serra_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto_Gasolio_Serra_Dettaglio", "Carburante Richiesto Gasolio Serra Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "assegnato_gasolio_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_gasolio_Dettaglio", "Carburante Assegnato Gasolio Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "assegnato_benzina_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_benzina_Dettaglio", "Carburante Assegnato Benzina Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });
        colonneKendoGrid.push({ field: "assegnato_gasolio_serra_Dettaglio", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_gasolio_serra_Dettaglio", "Carburante Assegnato Gasolio Serra Dettaglio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } });

    }

    if (QS_Avanzamento == 1) colonneKendoGrid.splice(4, 1);

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        //columnMenu: false,
        pdf: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: //"<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoContatto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
                        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaRichiesta(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(gestioneCarbResx, "Modifica", "Modifica") + "</div>" +
                        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px; display:none;' onclick=cancellaRichiesta(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(gestioneCarbResx, "Cancella", "Cancella") + "</div>" +
                        "<div class='btn btn-danger btnRinuncia' style='display:block;width:70px;border:0px; display:none;' onclick=cancellaRichiesta(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(gestioneCarbResx, "Rinuncia", "Rinuncia") + "</div>"
                }, title: Traduzione(gestioneCarbResx, "Azioni", "Azioni"), width: "97px", headerAttributes: { style: "text-align: center; vertical-align: top;" }
            }
        ]
    };

    if (QS_Avanzamento !== 2) {
        parametriKendoGrid.colonneCustomKendoGrid.push(
            {
                command: {
                    template: "<div class='btn btn-info fa fa-print btnStampa' style='display:none;width:25px;border:0px;' onclick=stampaRichiesta(this.closest('tr'),this.closest('.k-grid'))></div>"
                    //"<div class='btn btn-warning fa fa-print btnStampa' style='display:block;width:25px;border:0px;' onclick=stampaRichiesta2(this.closest('tr'),this.closest('.k-grid'))></div>"
                }, title: Traduzione(gestioneCarbResx, "Stampa", "Stampa"), width: "70px", headerAttributes: { style: "text-align: center; vertical-align: top;" }
            })
        parametriKendoGrid.colonneCustomKendoGrid.push(
            {
                command: {
                    template: "<div class='btn btn-info fa fa-object-ungroup btnPassaggioDiStato text-center' style='display:block;width:25px;border:0px;' onclick=passaggioDiStato(this.closest('tr'),this.closest('.k-grid'))></div>"
                    //"<div class='btn btn-warning fa fa-print btnStampa' style='display:block;width:25px;border:0px;' onclick=stampaRichiesta2(this.closest('tr'),this.closest('.k-grid'))></div>"
                }, title: Traduzione(gestioneCarbResx, "PassaggioStato", "Avanzamento Pratica"), width: "100px", headerAttributes: { style: "text-align: center; vertical-align: top;" }
            })
    }

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, */funzioneDaChiamareDopoDataBound: elencoOnDataBound/*, funzioneDaChiamareDopoDelete: HideTabDettagli*/ };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

}

function elencoOnDataBound(e) {
    var richiestePerAzienda = [];

    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        if (e.sender.dataItem(row).CodstatoAv == 2009) {
            var pulsanti = row[0].cells[0].children;
            for (var i = 0; i < pulsanti.length; i++) {
                pulsanti[i].setAttribute("disabled", "true");
            }
        }

        if (((permesso_richiesta && QS_Avanzamento == 0) || (permesso_rendicontazione && QS_Avanzamento == 1)) && QS_Avanzamento !== 2) {
            if (e.sender.dataItem(row).CodstatoAv == 2001) {
                $(row).find(".btnCancella").show();
            } else if (e.sender.dataItem(row).CodstatoAv == 2002 || e.sender.dataItem(row).CodstatoAv == 2007) {
                $(row).find(".btnRinuncia").show();
            }
        }

        // Le richieste e le rendicontazioni devono essere stampate solo nello stato "inserimento in corso, dati non verificati"
        if ([2001].includes(e.sender.dataItem(row).CodstatoAv) &&
            ((permesso_richiesta && QS_Avanzamento == 0) || (permesso_rendicontazione && QS_Avanzamento == 1))) {
            $(row).find(".btnStampa").show();
        }

        if (e.sender.dataItem(row).Litri_In_Esubero == "SI") {
            for (var cell = 0; cell < row[0].children.length; cell++) {
                row[0].children[cell].style.backgroundColor = "#e3c668"
            }
        }

        if (e.sender.dataItem(row).Prima_Richiesta === "Anticipo") {
            let dataItem = e.sender.dataItem(row);
            let piva = dataItem.piva;
            let cuaa = dataItem.CUAA;
            let anno = dataItem.annoRichiesta;
            let isTerz = dataItem.tipo_richiesta.includes("Conto Terzi");
            let tipoAz = dataItem.Tipo_Azienda;

            let richieste = null;
            for (var i = 0; i < richiestePerAzienda; i++) {
                if (richiestePerAzienda.piva == piva) {
                    richieste = richiestePerAzienda.richieste;
                    break;
                }
            }
            if (richieste == null) {
                richieste = ContaRichieste(piva, cuaa, anno, 0, isTerz, tipoAz);
                richiestePerAzienda.push({ "piva": piva, "richieste": richieste });
            }
            for (var i = 0; i < richieste.length; i++) {
                var richiesta = richieste[i];
                if (richiesta.Richiesta_Cod != dataItem.richiestaCod) {
                    if (richiesta.Stato_Cod != 2001) {
                        $(row).find(".btnModifica").hide();
                        break;
                    }
                }
            }
        }

        if (e.sender.dataItem(row).Approvatore !== null && QS_Avanzamento !== 2 && e.sender.dataItem(row).CodstatoAv == 2002) {
            if (!e.sender.dataItem(row).Approvatore.includes(hUtenteUsername)) {
                if (hCoordinaotreAfor.toLowerCase() === 'false') {
                    $(row).find(".btnPassaggioDiStato")[0].setAttribute("disabled", "");
                }
                $(row).find(".btnCancella")[0].setAttribute("disabled", "");
            }
        }
    }
}

function conto_create() {
    $('#conto').kendoDropDownList({
        filter: "contains",
        dataSource: [{ "tipo": "TUTTI", "cod": 2 }, { "tipo": "PROPRIO", "cod": 0 }, { "tipo": "TERZI / COOP", "cod": -1 }],
        dataTextField: "tipo",
        dataValueField: "cod",
        autoWidth: true
    });
}

function tipoPratica_create() {
    $('#ddlTipoPratica').kendoDropDownList({
        filter: "contains",
        dataSource: [{ "tipo": "TUTTE", "cod": 2 }, { "tipo": "Richieste", "cod": 0 }, { "tipo": "Rendicontazioni", "cod": 1 }],
        dataTextField: "tipo",
        dataValueField: "cod",
        autoWidth: true
    });
}

function approvatore_load() {
    $('#ddlApprovatore').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: LeggiApprovatori } },
        dataTextField: "Approvatore",
        dataValueField: "cod",
        optionLabel: { "Approvatore": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTI", "cod": "0" },
        autoWidth: true
    });
}

function allevamenti_load() {
    $('#ddlAllevamento').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: LeggiAllevamenti } },
        dataTextField: "UMA_All_Des",
        dataValueField: "UMA_All_Cod",
        optionLabel: { "UMA_All_Des": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTI", "UMA_All_Cod": "0" },
        autoWidth: true
    });
}

function lavorazioni_load() {
    $('#ddlLavorazione').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: leggiLavorazioni } },
        dataTextField: "Lav_UMA_Des",
        dataValueField: "Lav_UMA_Cod",
        optionLabel: { "Lav_UMA_Des": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTI", "Lav_UMA_Cod": "0" },
        autoWidth: true
    });
}

function colture_load() {
    $('#ddlColtura').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: leggiColture } },
        dataTextField: "Macrouso_UMA_Des",
        dataValueField: "Macrouso_UMA_Cod",
        optionLabel: { "Macrouso_UMA_Des": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTI", "Macrouso_UMA_Cod": "0" },
        autoWidth: true
    });
}

function dettaglio_create() {
    $('#ddlLivelloDettaglio').kendoDropDownList({
        filter: "contains",
        dataSource: [{ "tipo": "Testata", "cod": 0 }, { "tipo": "Righe", "cod": 1 }],
        dataTextField: "tipo",
        dataValueField: "cod",
        autoWidth: true
    });
}

function ddlDettaglio_Change() {
    if ($('#ddlLivelloDettaglio').val() == 0) {
        $('#coltura').hide();
        $('#lavorazione').hide();
        $('#allevamento').hide();
    } else {
        $('#coltura').show();
        $('#lavorazione').show();
        $('#allevamento').show();
    }
}

function StatiPratiche_Load() {

    $('#statoPratica').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiStatiPratiche } },
        dataTextField: "stato",
        dataValueField: "cod",
        optionLabel: { "stato": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTI", "cod": "-1" },
        autoWidth: true,
        dataBound: ddlStatiPratiche_OnDataBound
    });

}

function ddlStatiPratiche_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlStatiPratiche.onchange(); //forzo l'evento di onchange
    }
}

function Citta_Load() {
    WaitFrame.show();

    $('#citta').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiCitta } },
        dataTextField: "citta",
        dataValueField: "proCom",
        optionLabel: { "citta": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTE", "proCom": -1 },
        autoWidth: true,
        dataBound: ddlCitta_OnDataBound
    });

}

function ddlCitta_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlStatiPratiche.onchange(); //forzo l'evento di onchange
    } else if (KendoDDL("prov").value() == -1) {
        this.select(0);
    }
}

function Prov_Load() {

    $('#prov').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiProv } },
        dataTextField: "PROVINCIA",
        dataValueField: "PROV",
        //optionLabel: { "PROVINCIA": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTE", "PROV": -1 },
        autoWidth: true,
        dataBound: ddlProv_OnDataBound
    });

}

function ddlProv_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlProv.onchange(); //forzo l'evento di onchange
    }
}

function ddlProv_Change(e) {
    Citta_Load();
    KendoDDL("citta").value("-1");
    //$("#citta").show();
}

async function ddlAzienda_Load() {

    //var ddlAzienda = await RiempiDdlAzienda();
    $('#ddlAzienda').kendoDropDownList({
        filter: "contains",
        dataSource: {
            transport: {
                read: RiempiDdlAzienda
            }
        },
        dataTextField: "rag_soc",
        dataValueField: "piva",
        mapValueTo: "dataItem",
        dataBound: ddlAzienda_OnDataBound,
        virtual: {
            itemHeight: 26,
            valueMapper: function (options) {
                var val = options.value;
                if (val != "" && val != "-1") {
                    var a = KendoDDL("ddlAzienda").dataSource._pristineData.find((el) =>
                        el.piva == val
                    );
                    options.success(KendoDDL("ddlAzienda").dataSource._pristineData.indexOf(a));
                } else {
                    options.success("");
                }
            }
        },
    });

}

function RiempiDdlAzienda(options) {
    //return new Promise(function (resolve, reject) {
    var pathCaricaCmb = ""
    var parametri = ""
    if (usaNuovaVisibilita) {
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "area": "UMA" });
        pathCaricaCmb = "Anagrafica/Imprese.asmx/Carica_Cmb_Imprese_Area";
    } else {
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
        pathCaricaCmb = "Anagrafica/Imprese.asmx/Carica_Cmb_Imprese";
    }

    ajaxAgronicaSync(pathCoreWS + pathCaricaCmb,
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "piva": "", "rag_soc": "..." };
            if (QS_Piva != "") {
                let tmp = risp.filter(x => x.piva == QS_Piva)[0];
                if (tmp != undefined) {
                    risp.splice(risp.indexOf(tmp), 1);
                    risp.unshift(tmp);
                }
            }
            risp.unshift(objVuoto);
            //options.success(risp.slice(0,100));
            options.success(risp);
            //resolve(risp);
        }, null);

    //});
}

function ddlAzienda_OnDataBound(e) {

    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlAzienda.onchange(); //forzo l'evento di onchange
    } else if (QS_Piva != "" && primo_ddlAzienda_OnDataBound === 0) {
        //this.value("?????");
        this.value(QS_Piva);
        if (this.selectedIndex === -1) {
            this.select(0);
        }
        primo_ddlAzienda_OnDataBound = 1;
    }
}

function ddlAzienda_Change() {

    var piva = KendoDDL("ddlAzienda").value();

    if (piva == "" && QS_Piva != "") {
        piva = QS_Piva;
    }

    if (piva !== "" && piva !== "-1" && KendoDDL("ddlAzienda").text() !== "...") {

        var dettagliAzienda = CercaDettagliAzienda(piva);
        var esisteRichiesta = dettagliAzienda[0].cod > 0;

        var descrAzienda = $('#anno');
        descrAzienda[0].defaultValue = new Date().getFullYear();

        var descrAzienda = $('#nDichiarazione');
        if (descrAzienda[0].defaultValue == "")
            descrAzienda[0].defaultValue = esisteRichiesta ? dettagliAzienda[0].nDichiarazione : "";

        var descrAzienda = $('#piva');
        if (piva > 0)
            if (dettagliAzienda[0].PartitaIvaReale != "")
                descrAzienda[0].defaultValue = dettagliAzienda[0].PartitaIvaReale;
            else
                descrAzienda[0].defaultValue = piva;
        else
            descrAzienda[0].defaultValue = " ";
        /*var descrAzienda = $('#indirizzo');
        if (dettagliAzienda[0].indDes != "")
            descrAzienda[0].defaultValue = dettagliAzienda[0].indDes;
        else
            descrAzienda[0].defaultValue = " ";*/

        var descrAzienda = KendoDDL("prov");
        if (dettagliAzienda[0].pro_cod != "")
            descrAzienda.value(dettagliAzienda[0].pro_cod);
        else
            descrAzienda[0].defaultValue = " ";
        ddlProv_Change();

        var descrAzienda = KendoDDL("citta");
        if (dettagliAzienda[0].comDes != "" && KendoDDL("prov").value() == "-1")
            descrAzienda.value(dettagliAzienda[0].comDes);
        else
            descrAzienda.value(dettagliAzienda[0].comDesIstat);

        var descrAzienda = $('#CUAA');
        if (dettagliAzienda[0].CUAA != "")
            descrAzienda[0].defaultValue = dettagliAzienda[0].CUAA;
        else
            descrAzienda[0].defaultValue = " ";



    } else {
        $('#nDichiarazione')[0].defaultValue = " ";
        //$('#indirizzo')[0].defaultValue = " ";
        $('#piva')[0].defaultValue = " ";
        $('#CUAA')[0].defaultValue = " ";
        KendoDDL("citta").value("-1");
        KendoDDL("prov").value("-1");

    }
}

//è stata selezionata una cella
function modificaRichiesta(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    WaitFrame.show();
    var chiave = datiRiga.piva; //ID
    var richiestaCod = datiRiga.richiestaCod;
    var terz = datiRiga.tipo_richiesta;
    var tipo_azienda = datiRiga.Tipo_Azienda;
    var tipo_pratica = datiRiga.Tipo_Pratica;
    var prima_richiesta = datiRiga.Prima_Richiesta
    ajaxAgronicaSync("./ElencoRichieste.aspx/EditRichiesta",
        "{chiave: '" + chiave + "', " +
        "richiestaCod: " + richiestaCod + ", " +
        "tipo_azienda: '" + tipo_azienda + "' }",
        false,
        function (data) {
            risp = JSON.parse(data.RispostaStringa);
            options.success(risp);
        }, null);

    window.location = "../CarburantiUMA/RichiestaCarburanti.aspx?p=" + risp.piva +
        "&rc=" + risp.richiestaCod +
        "&fp=" + risp.fromPage + (terz.includes("Conto Terzi") ? "&type=-1" : "") +
        "&avanzamento=" + (QS_Avanzamento > 1 ? (tipo_pratica.includes("Richiesta") ? 0 : 1) : QS_Avanzamento) + // : (prima_richiesta == "Anticipo" ? -1 : QS_Avanzamento))
        ((prima_richiesta == "Anticipo") ? "&anticipo=1" : "") +
        "&tipo_azienda=" + risp.tipo_azienda +
        "&tipoOp=" + risp.tipoOp + (QS_Avanzamento == 2 ? "&provenienza=2" : "");

}

function cancellaRichiesta(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    chiaveDEL = datiRiga.piva; //ID
    richiestaCodDEL = datiRiga.richiestaCod;

    if (datiRiga.CodstatoAv == 2001) {

        if (dialogCanc == undefined) {

            dialogCanc = $("#dialogCanc").kendoDialog({
                width: "400px",
                title: Traduzione(gestioneCarbResx, "SeiSicuroDiEliminareQuestoElemento", "Sei sicuro di voler eliminare questo elemento?"),
                buttonLayout: "stretched",
                content: "<p>Stai per cancellare questa richiesta e tutte le lavorazioni ad essa assegnate.<p>",
                actions: [
                    { text: Traduzione(gestioneCarbResx, "Annulla", "Annulla"), primary: true },
                    {
                        text: Traduzione(gestioneCarbResx, "SiSonoSicuro", "Si sono sicuro"),

                        action: function (e) {
                            WaitFrame.show();
                            ajaxAgronicaSync("./ElencoRichieste.aspx/DeleteRichiesta",
                                "{Piva: '" + chiaveDEL + "' , " +
                                "richiestaCod: '" + richiestaCodDEL + "' }",
                                false,
                                function (data) {
                                    //kendo.alert(JSON.parse(data.RispostaStringa));
                                    WaitFrame.hide();
                                    ConfiguraGrigliaElenco("tab_griglia_elencoRichieste", (QS_Avanzamento != 0) ? true : false);
                                }, null);
                            return true;
                        }
                    }
                ]
            });

        }

        dialogCanc.data("kendoDialog").open();

    } else if (datiRiga.CodstatoAv != 2009) {
        if (dialogRinuncia == undefined) {

            dialogRinuncia = $("#dialogRinuncia").kendoDialog({
                width: "400px",
                title: Traduzione(gestioneCarbResx, "SeiSicuroDiRinunciare", "Sei sicuro di voler richiedere la rinuncia di questa richiesta?"),
                buttonLayout: "stretched",
                content: "<p>Stai per richiedere la rinuncia di questa richiesta (i litri relativi ad essa non saranno conteggianti nella rendicontazione).<p>",
                actions: [
                    { text: Traduzione(gestioneCarbResx, "Annulla", "Annulla"), primary: true },
                    {
                        text: Traduzione(gestioneCarbResx, "SiSonoSicuro", "Si sono sicuro"),

                        action: function (e) {
                            WaitFrame.show();
                            var parametri = {
                                Piva: chiaveDEL,
                                Cuaa: datiRiga.CUAA,
                                richiestaCod: datiRiga.richiestaCod,
                                pratica_cod: datiRiga.Pratica_Cod
                            }
                            ajaxAgronicaSync("./ElencoRichieste.aspx/RinunciaRichiesta",
                                JSON.stringify(parametri),
                                false,
                                function (data) {
                                    //kendo.alert(JSON.parse(data.RispostaStringa));
                                    WaitFrame.hide();
                                    ConfiguraGrigliaElenco("tab_griglia_elencoRichieste", (QS_Avanzamento != 0) ? true : false);
                                }, null);
                            return true;
                        }
                    }
                ]
            });

        }

        dialogRinuncia.data("kendoDialog").open();
    }


}

async function stampaRichiesta(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    let linkStampa = await ws_linkStampa(datiRiga.piva, QS_Avanzamento, datiRiga.richiestaCod);

    window.open(linkStampa, "_blank");

    //if (datiRiga.CodstatoAv >= 2007) {
    //    if (dialogRinuncia == undefined) {

    //        dialogRinuncia = $("#dialogRinuncia").kendoDialog({
    //            width: "400px",
    //            title: Traduzione(gestioneCarbResx, "SeiSicuroDiRinunciare", "Sei sicuro di voler richiedere la rinuncia di questa richiesta?"),
    //            buttonLayout: "stretched",
    //            content: "<p>Stai per richiedere la rinuncia di questa richiesta (i litri relativi ad essa non saranno conteggianti nella rendicontazione).<p>",
    //            actions: [
    //                { text: Traduzione(gestioneCarbResx, "Annulla", "Annulla"), primary: true, },
    //                {
    //                    text: Traduzione(gestioneCarbResx, "SiSonoSicuro", "Si sono sicuro"),

    //                    action: function (e) {
    //                        WaitFrame.show();
    //                        ajaxAgronicaSync("./ElencoRichieste.aspx/RinunciaRichiesta",
    //                            "{Piva: '" + chiave + "' , " +
    //                            "richiestaCod: '" + richiestaCod + "' }",
    //                            false,
    //                            function (data) {
    //                                //kendo.alert(JSON.parse(data.RispostaStringa));
    //                                WaitFrame.hide();
    //                                ConfiguraGrigliaElenco("tab_griglia_elencoRichieste");
    //                            }, null);
    //                        return true;
    //                    },
    //                }
    //            ],
    //        });

    //    }

    //    dialogRinuncia.data("kendoDialog").open();
    //}


}

function passaggioDiStato(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var parametri = {
        Richiesta_Cod: datiRiga.richiestaCod
    }
    $.ajax({
        type: "POST",
        url: "RichiestaCarburanti.aspx/gestionePassaggioDiStato",
        data: JSON.stringify(parametri),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            apriGestionePassaggioDiStato(msg.d.RispostaStringa);
        }
    });
}

function apriGestionePassaggioDiStato(url) {
    $(document.body).append('<div id="GestionePassaggioDiStatoWindow"></div>');
    $('#GestionePassaggioDiStatoWindow').kendoWindow({
        title: "Gestione Passaggio Di Stato",
        modal: true,
        resizable: true,
        iframe: true,
        width: "60%",
        height: "45%",
        content: url,
        close: function () {
            setTimeout(function () {
                $('#GestionePassaggioDiStatoWindow').kendoWindow('destroy');
                //$("#btn_CercaPratiche").trigger("click");
            }, 200);
        }
    }).data('kendoWindow').center();
}

function chiudiWindowPassaggioDiStato(msg) {

    setTimeout(function () {
        kendo.alert(msg);
        $('#GestionePassaggioDiStatoWindow').kendoWindow('destroy');
        $("#btn_carica_elenco").trigger("click");
    }, 200);

}

function getYear() {
    if ($.cookie("UMA.Anno") != null && !isNaN($.cookie("UMA.Anno"))) {
        return parseInt($.cookie("UMA.Anno"));
    } else {
        var date = new Date();
        let anno = new Date().getFullYear();
        date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
        $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
        return anno;
    }
}

function setYear(anno) {
    var date = new Date();
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
}