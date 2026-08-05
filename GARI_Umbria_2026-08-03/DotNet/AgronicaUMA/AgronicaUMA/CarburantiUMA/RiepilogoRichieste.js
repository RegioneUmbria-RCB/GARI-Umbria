const larghezzaStdCampoNumerico = 120;

$("#btn_carica_elenco").click(
    async function () {

        ConfiguraGrigliaRiepilogo("tab_griglia_riepilogoRichieste");
    });

//calcola il totale di una colonna da mettere nel footer della stessa
function calcTotaleColonna(field, idGriglia) {

    if (field !== undefined) {

        var grid = $("#"+idGriglia.id).data("kendoGrid");
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

        return totale.toLocaleString("it-IT", { minimumFractionDigits: 0, maximumFractionDigits: 0 });
    }
}

function ConfiguraGrigliaRiepilogo(IDControllo) {

    var omettiAnnulla = true;
    var funzioneSubmitDaUsare = null;
    checkPerc($("#legendaPerc"));
    funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti, flagInsert: false, flagUpdate: true, flagDelete: true*/ };

    var funzioniCRUD = {
        funzioneRead: RiepilogoRichieste,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: omettiAnnulla
    };
    var idModel = "id";
    var campiKendoModel = null;
    
    campiKendoModel = {
        id: { editable: false, type: "number", validation: { required: true } },
        Anno: { editable: false, type: "number", validation: { required: true } },
        Conto: { editable: false, type: "string", validation: { required: true } },
        Rag_Soc: { editable: false, type: "string", validation: { required: true } },
        piva: { editable: false, type: "string", validation: { required: true } },
        CUAA: { editable: false, type: "string", validation: { required: true } },

        Anticipo_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Anticipo_Benzina: { editable: false, type: "number", validation: { required: true } },
        Anticipo_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },
        
        Tipo_Azienda: { editable: false, type: "number", validation: { required: true } },
        Tipo_Azienda_Des: { editable: false, type: "string", validation: { required: true } },


        Richiesto_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Richiesto_Benzina: { editable: false, type: "number", validation: { required: true } },
        Richiesto_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        Richiesto_Numero_Lavorazioni_Elettricita: { editable: false, type: "number", validation: { required: true } },
        Richiesto_Numero_Lavorazioni_Non_Agricolo: { editable: false, type: "number", validation: { required: true } },

        richiesto_Gasolio_Totale: { editable: false, type: "number", validation: { required: true } },
        approvato_Gasolio_Totale: { editable: false, type: "number", validation: { required: true } },

        richiesto_Benzina_Totale: { editable: false, type: "number", validation: { required: true } },
        approvato_Benzina_Totale: { editable: false, type: "number", validation: { required: true } },

        Rendicontato_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Rendicontato_Benzina: { editable: false, type: "number", validation: { required: true } },
        Rendicontato_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        Rendicontato_Numero_Lavorazioni_Elettricita: { editable: false, type: "number", validation: { required: true } },
        Rendicontato_Numero_Lavorazioni_Non_Agricolo: { editable: false, type: "number", validation: { required: true } },

        Richiesto_Approvato_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Richiesto_Approvato_Benzina: { editable: false, type: "number", validation: { required: true } },
        Richiesto_Approvato_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        Rendicontato_Approvato_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Rendicontato_Approvato_Benzina: { editable: false, type: "number", validation: { required: true } },
        Rendicontato_Approvato_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        Acquistato_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Acquistato_Benzina: { editable: false, type: "number", validation: { required: true } },
        Acquistato_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        Acquistabile_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Acquistabile_Benzina: { editable: false, type: "number", validation: { required: true } },
        Acquistabile_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        Rimanenza_Iniziale_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Iniziale_Benzina: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Iniziale_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        Rimanenza_Finale_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Finale_Benzina: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Finale_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },

        Data_Presentazione_Rendicontazione: { editable: false, type: "date", validation: { required: true } }
    };

    var styleElen = /*"background-color: #C4C4EF; */"text-align: center; vertical-align: top";

    var footerTemplateStringAnticipo_Gasolio = "#=calcTotaleColonna('" + "Anticipo_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringAnticipo_Benzina = "#=calcTotaleColonna('" + "Anticipo_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringAnticipo_Gasolio_Serra = "#=calcTotaleColonna('" + "Anticipo_Gasolio_Serra" + "', " + IDControllo + ")#";

    var footerTemplateStringRimanenza_Iniziale_Gasolio = "#=calcTotaleColonna('" + "Rimanenza_Iniziale_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringRimanenza_Iniziale_Benzina = "#=calcTotaleColonna('" + "Rimanenza_Iniziale_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringRimanenza_Iniziale_Gasolio_Serra = "#=calcTotaleColonna('" + "Rimanenza_Iniziale_Gasolio_Serra" + "', " + IDControllo + ")#";

    var footerTemplateStringRichiesto_Gasolio = "#=calcTotaleColonna('" + "Richiesto_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringRichiesto_Benzina = "#=calcTotaleColonna('" + "Richiesto_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringRichiesto_Gasolio_Serra = "#=calcTotaleColonna('" + "Richiesto_Gasolio_Serra" + "', " + IDControllo + ")#";

    var footerTemplateStringRichiesto_Gasolio_Totale = "#=calcTotaleColonna('" + "richiesto_Gasolio_Totale" + "', " + IDControllo + ")#";
    var footerTemplateStringApprovato_Gasolio_Totale = "#=calcTotaleColonna('" + "approvato_Gasolio_Totale" + "', " + IDControllo + ")#";

    var footerTemplateStringRichiesto_Benzina_Totale = "#=calcTotaleColonna('" + "richiesto_Benzina_Totale" + "', " + IDControllo + ")#";
    var footerTemplateStringApprovato_Benzina_Totale = "#=calcTotaleColonna('" + "approvato_Benzina_Totale" + "', " + IDControllo + ")#";

    var footerTemplateStringRichiesto_Approvato_Gasolio = "#=calcTotaleColonna('" + "Richiesto_Approvato_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringRichiesto_Approvato_Benzina = "#=calcTotaleColonna('" + "Richiesto_Approvato_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringRichiesto_Approvato_Gasolio_Serra = "#=calcTotaleColonna('" + "Richiesto_Approvato_Gasolio_Serra" + "', " + IDControllo + ")#";

    var footerTemplateStringAcquistato_Gasolio = "#=calcTotaleColonna('" + "Acquistato_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringAcquistato_Benzina = "#=calcTotaleColonna('" + "Acquistato_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringAcquistato_Gasolio_Serra = "#=calcTotaleColonna('" + "Acquistato_Gasolio_Serra" + "', " + IDControllo + ")#";

    var footerTemplateStringAcquistabile_Gasolio = "#=calcTotaleColonna('" + "Acquistabile_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringAcquistabile_Benzina = "#=calcTotaleColonna('" + "Acquistabile_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringAcquistabile_Gasolio_Serra = "#=calcTotaleColonna('" + "Acquistabile_Gasolio_Serra" + "', " + IDControllo + ")#";

    var footerTemplateStringRendicontato_Gasolio = "#=calcTotaleColonna('" + "Rendicontato_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringRendicontato_Benzina = "#=calcTotaleColonna('" + "Rendicontato_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringRendicontato_Gasolio_Serra = "#=calcTotaleColonna('" + "Rendicontato_Gasolio_Serra" + "', " + IDControllo + ")#";

    var footerTemplateStringRendicontato_Approvato_Gasolio = "#=calcTotaleColonna('" + "Rendicontato_Approvato_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringRendicontato_Approvato_Benzina = "#=calcTotaleColonna('" + "Rendicontato_Approvato_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringRendicontato_Approvato_Gasolio_Serra = "#=calcTotaleColonna('" + "Rendicontato_Approvato_Gasolio_Serra" + "', " + IDControllo + ")#";

    var footerTemplateStringRimanenza_Finale_Gasolio = "#=calcTotaleColonna('" + "Rimanenza_Finale_Gasolio" + "', " + IDControllo + ")#";
    var footerTemplateStringRimanenza_Finale_Benzina = "#=calcTotaleColonna('" + "Rimanenza_Finale_Benzina" + "', " + IDControllo + ")#";
    var footerTemplateStringRimanenza_Finale_Gasolio_Serra = "#=calcTotaleColonna('" + "Rimanenza_Finale_Gasolio_Serra" + "', " + IDControllo + ")#";

    var colonneKendoGrid = [
        { field: "Anno", title: TraduzioneMultiResx(gestioneCarbResx, "Anno", "Anno"), width: 80, headerAttributes: { style: styleElen } },
        { field: "Rag_Soc", title: TraduzioneMultiResx(gestioneCarbResx, "Rag_Soc", "Azienda"), width: 250, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Conto", title: TraduzioneMultiResx(gestioneCarbResx, "Conto", "Conto"), width: 110, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
       
        { field: "Rimanenza_Iniziale_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Iniziale_Gasolio", "Gasolio Rimanenza Iniziale"), footerTemplate: footerTemplateStringRimanenza_Iniziale_Gasolio, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Iniziale_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Iniziale_Benzina", "Benzina Rimanenza Iniziale"), footerTemplate: footerTemplateStringRimanenza_Iniziale_Benzina, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Iniziale_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Iniziale_Gasolio_Serra", "Gasolio Serra Rimanenza Iniziale"), footerTemplate: footerTemplateStringRimanenza_Iniziale_Gasolio_Serra, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "Tipo_Azienda_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Tipo_Azienda", "Tipo Azienda"), width: 180, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },

        // { field: "piva", title: TraduzioneMultiResx(gestioneCarbResx, "piva", "Piva"), width: larghezzaStdCampoNumerico, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        //{ field: "Richiesto_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Gasolio", "Carburante Richiesto Gasolio"), footerTemplate: footerTemplateStringRichiesto_Gasolio, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        //{ field: "Richiesto_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Benzina", "Carburante Richiesto Benzina"), footerTemplate: footerTemplateStringRichiesto_Benzina, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Anticipo_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Anticipo_Gasolioe", "Gasolio Anticipo"), footerTemplate: footerTemplateStringAnticipo_Gasolio, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Anticipo_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Anticipo_Benzina", "Benzina Anticipo"), footerTemplate: footerTemplateStringAnticipo_Benzina, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Anticipo_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Anticipo_Gasolio_Serra", "Gasolio Serra Anticipo"), footerTemplate: footerTemplateStringAnticipo_Gasolio_Serra, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "richiesto_Gasolio_Totale", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto_Gasolio_Totale", "Gasolio Richiesto"), footerTemplate: footerTemplateStringRichiesto_Gasolio_Totale, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "richiesto_Benzina_Totale", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto_Benzina_Totale", "Benzina Richiesto"), footerTemplate: footerTemplateStringRichiesto_Benzina_Totale, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Richiesto_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Gasolio_Serra", "Gasolio Serra Richiesto"), footerTemplate: footerTemplateStringRichiesto_Gasolio_Serra, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Richiesto_Numero_Lavorazioni_Elettricita", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Numero_Lavorazioni_Elettricita", "Nr Lav Elettricità"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Richiesto_Numero_Lavorazioni_Non_Agricolo", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Numero_Lavorazioni_Non_Agricolo", "Nr Lav Carburante Non Agricolo"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        //{ field: "Richiesto_Approvato_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Approvato_Gasolio", "Carburante Richiesto Approvato Gasolio"), footerTemplate: footerTemplateStringRichiesto_Approvato_Gasolio, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        //{ field: "Richiesto_Approvato_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Approvato_Benzina", "Carburante Richiesto Approvato Benzina"), footerTemplate: footerTemplateStringRichiesto_Approvato_Benzina, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "approvato_Gasolio_Totale", title: TraduzioneMultiResx(gestioneCarbResx, "approvato_Gasolio_Totale", "Gasolio Approvato"), footerTemplate: footerTemplateStringApprovato_Gasolio_Totale, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "approvato_Benzina_Totale", title: TraduzioneMultiResx(gestioneCarbResx, "approvato_Benzina_Totale", "Benzina Approvato"), footerTemplate: footerTemplateStringApprovato_Benzina_Totale, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Richiesto_Approvato_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Richiesto_Approvato_Gasolio_Serra", "Gasolio Serra Approvato"), footerTemplate: footerTemplateStringRichiesto_Approvato_Gasolio_Serra, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "Acquistato_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_Gasolio", "Gasolio Acquistato"), footerTemplate: footerTemplateStringAcquistato_Gasolio, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Acquistato_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_Benzina", "Benzina Acquistato"), footerTemplate: footerTemplateStringAcquistato_Benzina, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Acquistato_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_Gasolio_Serra", "Gasolio Serra Acquistato"), footerTemplate: footerTemplateStringAcquistato_Gasolio_Serra, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "Acquistabile_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistabile_Gasolio", "Gasolio Acquistabile"), footerTemplate: footerTemplateStringAcquistabile_Gasolio, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Acquistabile_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistabile_Benzina", "Benzina Acquistabile"), footerTemplate: footerTemplateStringAcquistabile_Benzina, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Acquistabile_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistabile_Gasolio_Serra", "Gasolio Serra Acquistabile"), footerTemplate: footerTemplateStringAcquistabile_Gasolio_Serra, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "Rendicontato_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Rendicontato_Gasolio", "Gasolio Rendicontato"), footerTemplate: footerTemplateStringRendicontato_Gasolio, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rendicontato_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Rendicontato_Benzina", "Benzina Rendicontato "), footerTemplate: footerTemplateStringRendicontato_Benzina, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rendicontato_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Rendicontato_Gasolio_Serra", "Gasolio Serra Rendicontato"), footerTemplate: footerTemplateStringRendicontato_Gasolio_Serra, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "Rendicontato_Numero_Lavorazioni_Elettricita", title: TraduzioneMultiResx(gestioneCarbResx, "Rendicontato_Numero_Lavorazioni_Elettricita", "Nr Lav Rendicont. Elettricità"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rendicontato_Numero_Lavorazioni_Non_Agricolo", title: TraduzioneMultiResx(gestioneCarbResx, "RendiconRendicontato_Numero_Lavorazioni_Non_Agricolotato_Lavorazioni_Non_Agricolo", "Nr Lav Rendicont. Carburante Non Agricolo"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "Rendicontato_Approvato_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Rendicontato_Approvato_Gasolio", "Gasolio Rendicontato Approvato"), footerTemplate: footerTemplateStringRendicontato_Approvato_Gasolio, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rendicontato_Approvato_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Rendicontato_Approvato_Benzina", "Benzina Rendicontato Approvato"), footerTemplate: footerTemplateStringRendicontato_Approvato_Benzina, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rendicontato_Approvato_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Rendicontato_Approvato_Gasolio_Serra", "Gasolio Serra Rendicontato Approvato"), footerTemplate: footerTemplateStringRendicontato_Approvato_Gasolio_Serra, width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "Rimanenza_Finale_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Finale_Gasolio", "Gasolio Rimanenza Finale"), format: "{0:n0}", footerTemplate: footerTemplateStringRimanenza_Finale_Gasolio, width: larghezzaStdCampoNumerico, headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Finale_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Finale_Benzina", "Benzina Rimanenza Finale"), format: "{0:n0}", footerTemplate: footerTemplateStringRimanenza_Finale_Benzina, width: larghezzaStdCampoNumerico, headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Finale_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Finale_Gasolio_Serra", "Gasolio Serra Rimanenza Finale"), format: "{0:n0}", footerTemplate: footerTemplateStringRimanenza_Finale_Gasolio_Serra, width: larghezzaStdCampoNumerico, headerAttributes: { style: styleElen } },

        { field: "Data_Presentazione_Rendicontazione", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Presentazione_Rendicontazione", "Data Presentazione Rendicontaz."), format: "{0:dd/MM/yyyy}", width: 120, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
    ];

    var parametriPerLettura = [];
    var parametriDataSource = {
        pageSize: 10
    };

    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100], buttonCount: 4 },
        //columnMenu: false,
        pdf: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: elencoOnDataBound/*, funzioneDaChiamareDopoDelete: HideTabDettagli*/ };

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
    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        if (e.sender.dataItem(row).CodstatoAv == 2009) {
            var pulsanti = row[0].cells[0].children;
            for (var i = 0; i < pulsanti.length; i++) {
                pulsanti[i].setAttribute("disabled", "true");
            }
        }

        if (permesso_richiesta) {
            if (e.sender.dataItem(row).CodstatoAv == 2001) {
                $(row).find(".btnCancella").show();
            } else if (e.sender.dataItem(row).CodstatoAv != 2009) {
                $(row).find(".btnRinuncia").show();
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

function StatiPratiche_Load() {

    $('#statoPratica').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiStatiPratiche } },
        dataTextField: "stato",
        dataValueField: "cod",
        //optionLabel: { "stato": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + "...", "cod": "" },
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
        optionLabel: { "citta": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTE", "proCom": "-1" },
        autoWidth: true,
        dataBound: ddlCitta_OnDataBound
    });

}

function ddlCitta_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlStatiPratiche.onchange(); //forzo l'evento di onchange
    }
}

function Prov_Load() {

    $('#prov').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiProv } },
        dataTextField: "PROVINCIA",
        dataValueField: "PROV",
        //optionLabel: { "PROVINCIA": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + "...", "PROV": "" },
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
    }
    else {
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

async function ddlAzienda_Change() {

    var piva = KendoDDL("ddlAzienda").value();

    if (piva == "" && QS_Piva != "") {
        piva = QS_Piva;
    }
    var pivaReale = await CercaPivaRealeR(piva);
    if (piva !== "" && piva !== "-1" && KendoDDL("ddlAzienda").text() !== "...") {

        var dettagliAzienda = CercaDettagliAzienda(piva);
        var descrAzienda = $('#anno');
        descrAzienda[0].defaultValue = "2021";
        var descrAzienda = $('#nDichiarazione');
        descrAzienda[0].defaultValue = "numero Dichiarazione";
        var descrAzienda = $('#piva');
        if (pivaReale != "") {
            descrAzienda[0].defaultValue = pivaReale;
        }
        else if (piva > 0) {
            descrAzienda[0].defaultValue = piva;
        }
        else {
            descrAzienda[0].defaultValue = " ";
        }
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