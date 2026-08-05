var utenteAbilitatoInserimentoModifica = $("input[name$='hf_UtenteAbilitatoScrittura']").val();
var utenteAbilitatoCancellazione = $("input[name$='hf_UtenteAbilitatoScrittura']").val();
var utenteAbilitatoPomodoro = $("input[name$='hf_UtenteAbilitatoPomodoro']").val();

function hasGestioneWorkflow() {
    let gestioneWorkflow = false;

    if (Array.isArray(setupGestioneWorkflow) && setupGestioneWorkflow.length > 0) {

        let type = ParametroType();
        let doc_type = ParametroDocType();

        let causaliCorr = elencoCausali.filter(causale => causale.TYPE == type && causale.DOC_TYPE == doc_type);

        let i = 0;
        while (i < causaliCorr.length && gestioneWorkflow === false) {
            for (let z = 0; z < setupGestioneWorkflow.length; z++) {
                // Ogni elemento del setup è composto da: lavCod_servizioCod
                // in questo ciclo mi interessa solo il lavCod
                let setupLavCod = setupGestioneWorkflow[z].split("_")[0];
                if (setupLavCod == causaliCorr[i].LAV_COD) {
                    gestioneWorkflow = true;
                    break;
                }
            }
            i++;
        }
    }

    return gestioneWorkflow;
}

/**
 * Restituisce true se il permesso è undefined o null altrimenti true/false in base allo stesso
 * @param {any} permessoDoc
 */
function workflowPermessoModCanc(permessoDoc) {
    if (permessoDoc !== undefined && permessoDoc !== null) return permessoDoc;
    else return true;
}

function popolaGrigliaReportTestata(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: RicercaReportTestata,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null,
        UtenteAbilitatoInserimentoModifica: utenteAbilitatoInserimentoModifica === "True",
        UtenteAbilitatoCancellazione: utenteAbilitatoCancellazione === "True"
    };

    var idModel = "Id_Agenda";
    var campiKendoModel = {
        Stato_Fatturazione: { type: "string" },
        Mov_Non_Fatturati: { type: "string" },
        Mov_Fatturati_Parzialmente: { type: "string" },
        Blocco_Flag: { type: "number" },
        DocumentiPresenti: { type: "boolean" },
        Id_Agenda: { type: "number" },
        Id_Mov_Det: { type: "number" },
        Lav_Cod: { type: "number" },
        Sa_Cod: { type: "number" },
        Modulo: { type: "number" },
        Tipo_Accettazione: { type: "number" },
        Des_Lib: { type: "string" },
        Note: { type: "string" },
        Causale_Trasporto: { type: "string" },
        Soggetto_Codice: { type: "string" },
        Soggetto_Piva: { type: "string" },
        Soggetto_RagioneSociale: { type: "string" },
        Vettore_RagioneSociale: { type: "string" },
        Agente_RagioneSociale: { type: "string" },
        Agente_Codice: { type: "string" },
        Capoarea_RagioneSociale: { type: "string" },
        Capoarea_Codice: { type: "string" },
        Data_Movimento: { type: "date" },
        Data_Movimento_DDT: { type: "date" },
        Ora_Movimento: { type: "string" },
        Anno_Movimento: { type: "string" },
        Mese_Movimento: { type: "string" },
        Tipo_Documento: { type: "string" },
        Scadenza: { type: "date" },
        Numero_Movimento: { type: "string" },
        Importo: { type: "number" },
        Sa_Nome: { type: "string" },
        Peso: { type: "number" },
        Tara_Veicolo: { type: "number" },
        Stato_Pagamento_Des: { type: "string" },
        Pratica_Cod: { type: "number" },
        Pratica_Stato_Cod: { type: "number" },
        Pratica_Stato_Des: { type: "string" },
        Pratica_Stato_Note: { type: "string" },
        Utente_Creazione: { type: "string" },
        Data_Creazione: { type: "date" },
        Utente_Modifica: { type: "string" },
        Data_Modifica: { type: "date" },
        Permesso_Modifica_Workflow: { type: "boolean" },
        Permesso_Cancella_Workflow: { type: "boolean" },
        Data_Iniz_Val_Contratto: { type: "date" },
        Data_Fine_Val_Contratto: { type: "date" },
        Altri_Locatori_Contratto: { type: "string" },
        Riferimento_Ordini_Contratto: { type: "string" }
    };

    var colonneKendoGrid = colonneKendoGridTestata(IDControllo);

    //$.each(colonneKendoGrid, function (index)
    //{
    //    colonneKendoGrid[index].field = colonneKendoGrid[index].field.toLowerCase()
    //});

    var type = ParametroType();
    var doc_type = ParametroDocType();

    if (utenteAbilitatoInserimentoModifica === "True") {

        popolaAzioniTestataInserimentoModifica(colonneKendoGrid,type);

    } else {

        popolaAzioniTestataSolaLettura(colonneKendoGrid, type);

    }

    var parametriPerLettura = null;
    var parametriDataSource = {
        serverFiltering: false,
        aggregate: [
            { field: "Qta", aggregate: "sum" },
            { field: "Qta_Evasa", aggregate: "sum" },
            { field: "Qta_Residua", aggregate: "sum" },
            { field: "Kg_Netti", aggregate: "sum" },
            { field: "Tara_Totale", aggregate: "sum" },
            { field: "Kg_Lordi", aggregate: "sum" },
            { field: "Imponibile_Netto", aggregate: "sum" },
            { field: "Iva", aggregate: "sum" },
            { field: "Importo", aggregate: "sum" },            
            { field: "Provvigione_Calcolata", aggregate: "sum" },
            { field: "Peso", aggregate: "sum" },
            { field: "Tara_Veicolo", aggregate: "sum" }
        ]
    };
    var parametriKendoGrid = {
        columnMenu: true,
        editable: false,
        groupable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
        reorderable: true,
        selectable: "multiselect",
        toolbarCommands: []
    };

    let ricercaConferimenti = DocContab_TipoRicerca_Conferimenti == ParametroType().toUpperCase();
    if (IsFatturazione() || hasGestioneWorkflow() || ricercaConferimenti === true) {
        funzioniCRUD.checkBoxFunction = kEventoSelezionaRiga;

        if (IsFatturazione()) {
            parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_Fatturazione_Testata");
            parametriKendoGrid.checkSelezioneRiga = { filterable: false, field: null, width: "32px" }
        }

        if (hasGestioneWorkflow()) {
            parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_PassaggioStato");
            if (setupIB_Controlli == true) {
                parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_IBControlliInvio");
            }
        }

        if (ricercaConferimenti === true) {
            parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_ValorizzazioneConferimenti");
        }
    }

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: onDataBoundingTestataDocumenti, funzioneDaChiamarePrimaDiExcelExport: onExportExcel
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    var grid = creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

    return grid;
}

function popolaAzioniTestataInserimentoModifica(colonneKendoGrid,type) {

    if (type !== DocContab_TipoRicerca_Conferimenti) {

        colonneKendoGrid.unshift({
            command: [
                {
                    template: templateStampaDocumento,
                    visible: function (dataItem) {
                        return dataItem.Lav_Cod !== lavCod_ContrattoAffitto;
                    }
                },
                {
                    template: templateModificaDocumento,
                    visible: function (dataItem) {
                        return dataItem.Blocco_Flag !== 1 && workflowPermessoModCanc(dataItem.Permesso_Modifica_Workflow);
                    }
                },
                {
                    template: templateEliminaDocumento,
                    visible: function (dataItem) {
                        return dataItem.Blocco_Flag !== 1 && workflowPermessoModCanc(dataItem.Permesso_Cancella_Workflow);
                    }
                },
                {
                    template: templateSbloccaDocumento,
                    visible: function (dataItem) {
                        return dataItem.Blocco_Flag === 1;
                    }
                },
                {
                    template: templateBloccaDocumento,
                    visible: function (dataItem) {
                        return dataItem.Blocco_Flag !== 1;
                    }
                },
                {
                    template: templateVisualizzaDocumento
                },
                {
                    template: templateGestioneAllegati,
                    visible: function (dataItem) {
                        return dataItem.DocumentiPresenti == true;
                    }
                },
                {
                    template: templateNuovoAllegato,
                    visible: function (dataItem) {
                        return $("input[name$='hf_UtenteAbilitatoGestioneNuovoAllegato']").val() == "True";
                    }
                }
            ], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "260px"
        });

    } else {

        colonneKendoGrid.unshift({
            command: [
                { template: templateStampaDocumento },
                { template: templateStampaEtichetteTestata },
                {
                    template: templateModificaDocumento,
                    visible: function (dataItem) { return dataItem.Blocco_Flag !== 1 && workflowPermessoModCanc(dataItem.Permesso_Modifica_Workflow) && (dataItem.Tipo_Accettazione != -1 || utenteAbilitatoPomodoro); }
                },
                {
                    template: templateEliminaDocumento,
                    visible: function (dataItem) { return dataItem.Blocco_Flag !== 1 && workflowPermessoModCanc(dataItem.Permesso_Cancella_Workflow) && (dataItem.Tipo_Accettazione != -1 || utenteAbilitatoPomodoro); }
                },
                {
                    template: templateSbloccaDocumento,
                    visible: function (dataItem) { return dataItem.Blocco_Flag === 1; }
                },
                {
                    template: templateBloccaDocumento,
                    visible: function (dataItem) { return dataItem.Blocco_Flag !== 1; }
                },
                {
                    template: templateVisualizzaDocumento
                },
                {
                    template: templateGestioneAllegati,
                    visible: function (dataItem) {
                        return dataItem.DocumentiPresenti == true;
                    }
                },
                {
                    template: templateNuovoAllegato,
                    visible: function (dataItem) {
                        return $("input[name$='hf_UtenteAbilitatoGestioneNuovoAllegato']").val() == "True";
                    }
                }
            ], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "260px"
        });

    }

}

function popolaAzioniTestataSolaLettura(colonneKendoGrid, type) {

    switch (type.toUpperCase()) {

        case DocContab_TipoRicerca_Contratti:

            colonneKendoGrid.unshift({
                command: [{
                    template: templateVisualizzaDocumento
                }], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "140px"
            });

            break;

        case DocContab_TipoRicerca_Conferimenti:

            colonneKendoGrid.unshift({
                command: [{
                    template: templateVisualizzaDocumento + templateStampaDocumento + templateStampaEtichetteTestata
                }], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "200px"
            });

            break;

        default:

            colonneKendoGrid.unshift({
                command: [{
                    template: templateVisualizzaDocumento + templateStampaDocumento
                }], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "140px"
            });

    }

}

function onChangeGrigliaTestata(e)
{
}

function colonneKendoGridTestata(IDControllo)
{
    var colonneKendoGrid = [];

    switch (IDControllo)
    {

        case "tab_testata_griglia_report_vendite":
            var visibleDocAllegati = VisibleDocAllegatiColumn();
            colonneKendoGrid = [
                { field: "Des_Lib", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), width: 400 },
                { field: "Blocco_Flag", title: TraduzioneMultiResx(resxObj, "Bloccata", "Bloccata"), template: '<input type="checkbox" #= Blocco_Flag === 1 ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, width: 95 },
                { field: "DocumentiPresenti", title: TraduzioneMultiResx(resxObj, "DocAllegati", "Doc. Allegati"), template: '<input type="checkbox" #= DocumentiPresenti ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, hidden: visibleDocAllegati, width: 120 },
                { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Cliente", "Cliente"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Data_Movimento", title: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc."), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc.") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Tipo_Documento", title: TraduzioneMultiResx(resxObj, "TipoDocumentoAbbr", "Tipo Doc."), filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: TraduzioneMultiResx(resxObj, "NrDocumentoAbbr", "Nr. Doc."), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Importo", title: TraduzioneMultiResx(resxObj, "Importo", "Importo"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 100 },
                { field: "Sa_Nome", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: TraduzioneMultiResx(resxObj, "CodiceClienteAbbr", "Cod. Cliente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Vettore_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Vettore", "Vettore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Agente_Codice", title: TraduzioneMultiResx(resxObj, "CodiceAgenteAbbr", "Cod. Agente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Agente_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Agente", "Agente"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Capoarea_Codice", title: TraduzioneMultiResx(resxObj, "CodiceCapoAreaAbbr", "Cod. Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Capoarea_RagioneSociale", title: TraduzioneMultiResx(resxObj, "CapoArea", "Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Anno_Movimento", title: TraduzioneMultiResx(resxObj, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: TraduzioneMultiResx(resxObj, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: TraduzioneMultiResx(resxObj, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Note", title: TraduzioneMultiResx(resxObj, "Note", "Note"), hidden: true, width: 150 },                
                { field: "Scadenza", title: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza"), template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza") + ': #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 }
            ];

            if (ParametroDocType() == DocContab_TipoDoc_Fattura && IsFatturazione() == false) {
                colonneKendoGrid.unshift({ field: "Stato_Pagamento_Des", title: TraduzioneMultiResx(resxObj, "Pagato", "Pagato"), filterable: { multi: true, search: true }, hidden: false, width: 150 });
            }

            if (IsFatturazione()) {
                colonneKendoGrid.unshift({ field: "Mov_Non_Fatturati", title: TraduzioneMultiResx(resxObj, "NonFatturatoSigla", "NF"), filterable: { multi: true, search: true }, width: 100, hidden: true });
                colonneKendoGrid.unshift({ field: "Mov_Fatturati_Parzialmente", title: TraduzioneMultiResx(resxObj, "FatturatoParzialmenteSigla", "FP"), filterable: { multi: true, search: true }, width: 100, hidden: true });
                colonneKendoGrid.unshift({ field: "Stato_Fatturazione", title: TraduzioneMultiResx(resxObj, "StatoFatturazione", "Stato"), filterable: { multi: true, search: true }, width: 150 });
            }            
            
            break;

        case "tab_testata_griglia_report_acquisti":
            var visibleDocAllegati = VisibleDocAllegatiColumn();
            colonneKendoGrid = [
                { field: "Des_Lib", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), width: 400 },
                { field: "Blocco_Flag", title: TraduzioneMultiResx(resxObj, "Bloccata", "Bloccata"), template: '<input type="checkbox" #= Blocco_Flag === 1 ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, width: 95 },
                { field: "DocumentiPresenti", title: TraduzioneMultiResx(resxObj, "DocAllegati", "Doc. Allegati"), template: '<input type="checkbox" #= DocumentiPresenti ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, hidden: visibleDocAllegati, width: 120 },
                { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Fornitore", "Fornitore"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Data_Movimento", title: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc."), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc.") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Tipo_Documento", title: TraduzioneMultiResx(resxObj, "TipoDocumentoAbbr", "Tipo Doc."), filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: TraduzioneMultiResx(resxObj, "NrDocumentoAbbr", "Nr. Doc."), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Importo", title: TraduzioneMultiResx(resxObj, "Importo", "Importo"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 100 },
                { field: "Sa_Nome", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: TraduzioneMultiResx(resxObj, "CodiceFornitoreAbbr", "Cod. Fornitore"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Vettore_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Vettore", "Vettore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Agente_Codice", title: TraduzioneMultiResx(resxObj, "CodiceAgenteAbbr", "Cod. Agente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Agente_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Agente", "Agente"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Capoarea_Codice", title: TraduzioneMultiResx(resxObj, "CodiceCapoAreaAbbr", "Cod. Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Capoarea_RagioneSociale", title: TraduzioneMultiResx(resxObj, "CapoArea", "Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Anno_Movimento", title: TraduzioneMultiResx(resxObj, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: TraduzioneMultiResx(resxObj, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: TraduzioneMultiResx(resxObj, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Note", title: TraduzioneMultiResx(resxObj, "Note", "Note"), hidden: true, width: 150 },
                { field: "Scadenza", title: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza"), template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza") + ': #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 }
            ];

            break;

        case "tab_testata_griglia_report_conferimenti":
            var visibleDocAllegati = VisibleDocAllegatiColumn();
            colonneKendoGrid = [
                { field: "Des_Lib", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), width: 400 },
                { field: "Blocco_Flag", title: TraduzioneMultiResx(resxObj, "Bloccata", "Bloccata"), template: '<input type="checkbox" #= Blocco_Flag === 1 ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, width: 95 },
                { field: "DocumentiPresenti", title: TraduzioneMultiResx(resxObj, "DocAllegati", "Doc. Allegati"), template: '<input type="checkbox" #= DocumentiPresenti ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, hidden: visibleDocAllegati, width: 120 },
                { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Conferente", "Conferente"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Piva", title: TraduzioneMultiResx(resxObj, "PartitaIVA", "Partita IVA"), filterable: { multi: true, search: true }, width: 120 },
                { field: "Soggetto_Cess1_RagioneSociale", title: TraduzioneMultiResx(resxObj, "PrimoCessionarioAbbr", "1° Cessionario"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Cess2_RagioneSociale", title: TraduzioneMultiResx(resxObj, "SecondoCessionarioAbbr", "2° Cessionario"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Destinazione", title: TraduzioneMultiResx(resxObj, "Produttore", "Produttore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Data_Movimento", title: TraduzioneMultiResx(resxObj, "DataEmissione", "Data Emissione"), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Data Conf.: #= kendo.toString(value,'dd/MM/yyyy') #", width: 120 },
                { field: "Ora_Movimento", title: TraduzioneMultiResx(resxObj, "Ora", "Ora"),  attributes: { style: "text-align:center;" },  width: 80 },
                { field: "Tipo_Documento", title: TraduzioneMultiResx(resxObj, "TipoDocumentoAbbr", "Tipo Doc."), filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: TraduzioneMultiResx(resxObj, "NumAccettazione", "N° Accettazione"), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Data_Movimento_DDT", title: TraduzioneMultiResx(resxObj, "DataDDT", "Data DDT"), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataDDT", "Data DDT") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Numero_Movimento_DDT", title: TraduzioneMultiResx(resxObj, "NrDDT", "Nr. DDT"), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Importo", title: TraduzioneMultiResx(resxObj, "Importo", "Importo"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 100 },
                { field: "Sa_Nome", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: TraduzioneMultiResx(resxObj, "CodiceClienteAbbr", "Cod. Cliente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Vettore_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Vettore", "Vettore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Agente_Codice", title: TraduzioneMultiResx(resxObj, "CodiceAgenteAbbr", "Cod. Agente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Agente_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Agente", "Agente"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Capoarea_Codice", title: TraduzioneMultiResx(resxObj, "CodiceCapoAreaAbbr", "Cod. Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Capoarea_RagioneSociale", title: TraduzioneMultiResx(resxObj, "CapoArea", "Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Anno_Movimento", title: TraduzioneMultiResx(resxObj, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: TraduzioneMultiResx(resxObj, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: TraduzioneMultiResx(resxObj, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Note", title: TraduzioneMultiResx(resxObj, "Note", "Note"), hidden: true, width: 150 },
                { field: "Scadenza", title: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza"), template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza") + ': #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 },
                { field: "Peso", title: TraduzioneMultiResx(resxObj, "PesoTotaleKg", "Peso Totale Kg"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 100 },
                { field: "Tara_Veicolo", title: TraduzioneMultiResx(resxObj, "TaraVeicoloKg", "Tara Veicolo Kg"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 130 },
                {
                    field: "Tipo_Accettazione", title: TraduzioneMultiResx(resxObj, "Pomodoro", "Pomodoro"), filterable: { multi: true, search: true }, hidden: true, width: 100,
                    values: [{ text: TraduzioneMultiResx(resxObj, "Si", "SI"), value: -1 }, { text: TraduzioneMultiResx(resxObj, "No", "NO"), value: 2 }]
                },
            ];

            break;

        case "tab_testata_griglia_report_contratti":
            colonneKendoGrid = [
                { field: "Des_Lib", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), width: 400 },
                { field: "Blocco_Flag", title: TraduzioneMultiResx(resxObj, "Bloccata", "Bloccata"), template: '<input type="checkbox" #= Blocco_Flag === 1 ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, width: 95 },
                { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Locatore", "Locatore"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Data_Movimento", title: TraduzioneMultiResx(resxObj, "DataRegistrazioneAbbr", "Data Registr."), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataRegistrazioneAbbr", "Data Registr.") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Tipo_Documento", title: TraduzioneMultiResx(resxObj, "TipoDocumentoAbbr", "Tipo Doc."), hidden: true, filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: TraduzioneMultiResx(resxObj, "NrRegistrazioneAbbr", "Nr. Reg."), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Data_Iniz_Val_Contratto", title: TraduzioneMultiResx(resxObj, "InizioValidità", "Inizio Validità"), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Inizio Val.: #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Data_Fine_Val_Contratto", title: TraduzioneMultiResx(resxObj, "FineValidità", "Fine Validità"), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Fine Val.: #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Importo", title: TraduzioneMultiResx(resxObj, "Importo", "Importo"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 100 },
                { field: "Numero_Movimento_DDT", title: TraduzioneMultiResx(resxObj, "NumInterno", "Num. Interno"), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Sa_Nome", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: TraduzioneMultiResx(resxObj, "CodLocatore", "Cod. Locatore"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Anno_Movimento", title: TraduzioneMultiResx(resxObj, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: TraduzioneMultiResx(resxObj, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: TraduzioneMultiResx(resxObj, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Altri_Locatori_Contratto", title: TraduzioneMultiResx(resxObj, "AltriLocatori", "Altri Locatori"), hidden: true, width: 300 },
                { field: "Riferimento_Ordini_Contratto", title: TraduzioneMultiResx(resxObj, "RiferimentoOrdini", "Riferimento Ordini"), hidden: true, width: 300 },
                { field: "Note", title: TraduzioneMultiResx(resxObj, "Note", "Note"), hidden: true, width: 150 }
            ];

            break;

    }

    SeEliminaColonneGruppoEconomico(colonneKendoGrid);

    SeInserisciColonnaPraticaStatoDes(colonneKendoGrid);

    InserisciColonneFinali(colonneKendoGrid);

    return colonneKendoGrid;

}

function SeEliminaColonneGruppoEconomico(colonneKendoGrid) {

    if ($(cUtenteAbilitatoGestionePrezziLettura).val() !== "True") {

        for (var indice = colonneKendoGrid.length - 1; indice >= 0; indice--) {

            if (colonneKendoGrid[indice].gruppoColonne === gruppoColonneEconomico) {

                colonneKendoGrid.splice(indice, 1);

            }

        }

    }

}

function SeInserisciColonnaPraticaStatoDes(colonneKendoGrid) {

    if (hasGestioneWorkflow()) {

        var indiceRiferimento = colonneKendoGrid.findIndex(colonna => colonna.field === "Numero_Movimento");

        if (indiceRiferimento >= 0) {

            var colonnaPraticaStatoDes = { field: "Pratica_Stato_Des", title: TraduzioneMultiResx(resxObj, "StatoDoc", "Stato Doc."), filterable: { multi: true, search: true }, hidden: false, width: 200 };

            colonneKendoGrid.splice(indiceRiferimento + 1, 0, colonnaPraticaStatoDes);

            var colonnaPraticaStatoNote = { field: "Pratica_Stato_Note", title: TraduzioneMultiResx(resxObj, "NotePassaggioDiStato", "Note passaggio di stato"), filterable: { multi: true, search: true }, hidden: false, width: 200 };

            colonneKendoGrid.splice(indiceRiferimento + 2, 0, colonnaPraticaStatoNote);

        }

    }

}

function InserisciColonneFinali(colonneKendoGrid) {

    colonneKendoGrid.push({ field: "Utente_Creazione", title: TraduzioneMultiResx(resxObj, "UtenteCreazione", "Utente Creazione"), filterable: { multi: true, search: true }, hidden: true, width: 200 });
    colonneKendoGrid.push({ field: "Data_Creazione", format: "{0:dd/MM/yyyy HH:mm:ss}", title: TraduzioneMultiResx(resxObj, "DataCreazione", "Data Creazione"), filterable: { multi: true, search: true }, hidden: true, width: 150 });
    colonneKendoGrid.push({ field: "Utente_Modifica", title: TraduzioneMultiResx(resxObj, "UtenteModifica", "Utente Modifica"), filterable: { multi: true, search: true }, hidden: true, width: 200 });
    colonneKendoGrid.push({ field: "Data_Modifica", format: "{0:dd/MM/yyyy HH:mm:ss}", title: TraduzioneMultiResx(resxObj, "DataModifica", "Data Modifica"), filterable: { multi: true, search: true }, hidden: true, width: 150 });
    colonneKendoGrid.push({ field: "Id_Agenda", title: "Id Agenda", filterable: { multi: true, search: true }, hidden: true, width: 200 });

}

function ApriKendoWindowRicercaDocumenti(tr_elem, grid_elem) {

    var dataItem = GetIdAgendaInElem(tr_elem, grid_elem);

    let mesDocNonVisibileGruppiMerce = VerificaPermessoVisibilitaGruppiMerce(dataItem.Id_Agenda, dataItem.Lav_Cod);

    if (mesDocNonVisibileGruppiMerce !== "") {

        $("<div></div>").kendoAlert({
            title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
            content: mesDocNonVisibileGruppiMerce
        }).data("kendoAlert").open();

        return false;
    }

    let areaProvenienza = "10";

    if (dataItem.Lav_Cod === 2006) {
        areaProvenienza = "12";
    }

    var url = "../Scadenzario/Scad_lista.aspx?type=doc" + "&area_provenienza=" + areaProvenienza + "&p=" + dataItem.PIVA + "&id_agenda=" + dataItem.Id_Agenda;

    $(document.body).append('<div id="ricerca_documentale"></div>');
    $('#ricerca_documentale').kendoWindow({
        title: TraduzioneMultiResx(resxObj, "RicercaDocumenti", "Ricerca Documenti"),
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#ricerca_documentale').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}



function ApriKendoWindowAggiungiNuovoAllegato(tr_elem, grid_elem) {


    var dataItem = GetIdAgendaInElem(tr_elem, grid_elem);

    let mesDocNonVisibileGruppiMerce = VerificaPermessoVisibilitaGruppiMerce(dataItem.Id_Agenda, dataItem.Lav_Cod);

    if (mesDocNonVisibileGruppiMerce !== "") {

        $("<div></div>").kendoAlert({
            title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
            content: mesDocNonVisibileGruppiMerce
        }).data("kendoAlert").open();

        return false;
    }

    var ID_Alert_Entita = -1;
    var ID_Elenco = -1;
    var Modalita = "doc";

    var Id_Schema_Template = dataItem.Id_Schema_Template;
    var Tipologia = dataItem.Tipologia;
    var Pratica = dataItem.Pratica_Cod;
    var Piva = dataItem.PIVA;
    var Id_Agenda = dataItem.Id_Agenda;
    var IdArea = 10;

    if (Tipologia == undefined || Tipologia == null ||Tipologia == "" ) {
        switch (dataItem.Lav_Cod) {
            case 2004: //Ordine Acquisto           
                Tipologia = -18;
                break;
            case 1025: //DDT Ricevuto              
                Tipologia = -19;
                break;
            case 1054:
            case 1076:
            case 1078: //Conferimento    
                Tipologia = -20;
                break;
            case 1031: //DDT Emesso                
                Tipologia = -21;
                break;
            case 2002: //Ordine Vendita            
                Tipologia = -22;
                break;
            case 1000: //Ordine Vendita
                Tipologia = -23;
                break;
            case 1001: //Fattura emessa            
                Tipologia = -24;
                break;
            case 2006: //Contratto d'affitto
                Tipologia = -26;
                IdArea = 12;
                break;
        }
    }


    var param = kendo.stringify({ 'Piva': Piva, 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'Id_Area': IdArea, 'Tipologia': Tipologia, 'area_provenienza': IdArea, 'Id_Agenda': Id_Agenda });
    //var param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 1, 'ID_Alert_Entita': ID_Alert_Entita });
    var url = "../Scadenzario/Scad_CreaModificaItem.aspx?scadstr=" + param + "&type=" + Modalita  + "&p=" + dataItem.PIVA ;



    //var url = "../Scadenzario/Scad_CreaModificaItem.aspx?type=doc" + "&area_provenienza=" + "10" + "&p=" + dataItem.PIVA + "&id_agenda=" + dataItem.Id_Agenda + "&Tipologia=" + dataItem.Tipo;


    $(document.body).append('<div id="nuovo_documentale"></div>');
    $('#nuovo_documentale').kendoWindow({
        title: TraduzioneMultiResx(resxObj, "NuovoAllegato", "Nuovo Allegato"),
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#nuovo_documentale').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function popolaGrigliaReportDettaglio(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: RicercaReportDettaglio,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True"
    };
    var idModel = "Id_Mov_Det";
    var campiKendoModel = {
        Stato_Fatturazione: { type: "string" },
        Mov_Non_Fatturati: { type: "string" },
        Mov_Fatturati_Parzialmente: { type: "string" },
        Blocco_Flag: { type: "number" },
        DocumentiPresenti: { type: "boolean" },
        Id_Agenda: { type: "number" },
        Id_Mov_Det: { type: "number" },
        Lav_Cod: { type: "number" },
        Sa_Cod: { type: "number" },
        Modulo: { type: "number" },
        Tipo_Accettazione: { type: "number" },
        Mat_Cod: { type: "string" },
        Cal_Cod: { type: "string" },
        Lotto: { type: "string" },
        Des_Lib: { type: "string" },
        Note: { type: "string" },
        Causale_Trasporto: { type: "string" },
        Soggetto_Codice: { type: "string" },
        Soggetto_Piva: { type: "string" },
        Soggetto_RagioneSociale: { type: "string" },
        Soggetto_Rapporto: { type: "string" },
        Stato: { type: "string" },
        Regione: { type: "string" },
        Provincia: { type: "string" },
        Comune: { type: "string" },
        Destinazione: { type: "string" },
        Stato_Dest: { type: "string" },
        Regione_Dest: { type: "string" },
        Provincia_Dest: { type: "string" },
        Comune_Dest: { type: "string" },
        Vettore_RagioneSociale: { type: "string" },
        Agente_RagioneSociale: { type: "string" },
        Agente_Codice: { type: "string" },
        Capoarea_RagioneSociale: { type: "string" },
        Capoarea_Codice: { type: "string" },
        Data_Movimento: { type: "date" },
        Data_Movimento_DDT: { type: "date" },
        Ora_Movimento: { type: "string" },
        Anno_Movimento: { type: "string" },
        Mese_Movimento: { type: "string" },
        Tipo_Documento: { type: "string" },
        Scadenza: { type: "date" },
        Numero_Movimento: { type: "string" },
        Riga: { type: "string" },
        Referenza_Codice: { type: "number" },
        Referenza_Descr: { type: "string" },
        Categoria_Prodotto: { type: "string" },
        Categoria_Commerciale: { type: "string" },
        Note_Prodotto: { type: "string" },
        Unita_Misura_Sigla: { type: "string" },
        Unita_Misura_Secondaria_Sigla: { type: "string" },
        Qta: { type: "number" },
        Qta_Evasa: { type: "number" },
        Qta_Residua: { type: "number" },
        StatoEvasione_Des: { type: "string" },
        Qta_Extra: { type: "number" },
        Kg_Netti: { type: "number" },
        Degrado_Perc: { type: "number" },
        Degrado: { type: "number" },
        Netto_Pagamento: { type: "number" },
        Qta_Netta_Evasa: { type: "number" },
        Qta_Netta_Residua: { type: "number" },
        Tara_Totale: { type: "number" },
        Qta_Tara_Evasa: { type: "number" },
        Qta_Tara_Residua: { type: "number" },
        Kg_Lordi: { type: "number" },
        Qta_Lorda_Evasa: { type: "number" },
        Qta_Lorda_Residua: { type: "number" },
        Nr_Imballi: { type: "number" },
        Qta_Imballi_Evasa: { type: "number" },
        Qta_Imballi_Residua: { type: "number" },
        Nr_Contenitori: { type: "number" },
        Qta_Contenitori_Evasa: { type: "number" },
        Qta_Contenitori_Residua: { type: "number" },
        Nr_Confezioni: { type: "number" },
        Prezzo_Netto: { type: "number" },
        Prezzo_Riferito_A: { type: "string" },
        Imponibile: { type: "number" },
        Sconto_Perc: { type: "number" },
        Sconto: { type: "number" },
        Imponibile_Netto: { type: "number" },
        Iva: { type: "number" },
        Cod_Iva: { type: "number" },
        Aliquota_Iva_Des: { type: "string" },
        Importo: { type: "number" },
        Provvigione: { type: "number" },
        Provvigione_Calcolata: { type: "number" },
        Veg_Des: { type: "string" },
        Cul_Des: { type: "string" },
        Codice_Esterno: { type: "string" },
        Pratica_Cod: { type: "number" },
        Pratica_Stato_Cod: { type: "number" },
        Pratica_Stato_Des: { type: "string" },
        Pratica_Stato_Note: { type: "string" },
        Gruppi_Merce: { type: "string" },
        Utente_Creazione: { type: "string" },
        Data_Creazione: { type: "date" },
        Utente_Modifica: { type: "string" },
        Data_Modifica: { type: "date" },
        Permesso_Modifica_Workflow: { type: "boolean" },
        Permesso_Cancella_Workflow: { type: "boolean" },
        Data_Iniz_Val_Contratto: { type: "date" },
        Data_Fine_Val_Contratto: { type: "date" },
        Altri_Locatori_Contratto: { type: "string" },
        Riferimento_Ordini_Contratto: { type: "string" },
        Numero_Ordine: { type: "string" },
        Data_Ordine: { type: "date" }
    };

    for (let i in Elenco_Parametri_Qualitativi) {
        if (Elenco_Parametri_Qualitativi[i].tipo === 3)
            campiKendoModel[Elenco_Parametri_Qualitativi[i].campo] = { type: "number" };
        else
            if (Elenco_Parametri_Qualitativi[i].tipo === 5)
                campiKendoModel[Elenco_Parametri_Qualitativi[i].campo] = { type: "date" };
            else
                campiKendoModel[Elenco_Parametri_Qualitativi[i].campo] = { type: "string" };
    }

    var colonneKendoGrid = colonneKendoGridDettaglio(IDControllo);

    for (let i in Elenco_Parametri_Qualitativi) {

        if (Elenco_Parametri_Qualitativi[i].tipo === 3)
            colonneKendoGrid.push({ field: Elenco_Parametri_Qualitativi[i].campo, title: Elenco_Parametri_Qualitativi[i].nome, hidden: true, width: 100});
        else
            if (Elenco_Parametri_Qualitativi[i].tipo === 5)
                colonneKendoGrid.push({ field: Elenco_Parametri_Qualitativi[i].campo, title: Elenco_Parametri_Qualitativi[i].nome, hidden: true, width: 100, format: "{0:dd/MM/yyyy}" });
            else
                colonneKendoGrid.push({ field: Elenco_Parametri_Qualitativi[i].campo, title: Elenco_Parametri_Qualitativi[i].nome, hidden: true, width: 100, filterable: { multi: true, search: true } });

    }

    var type = ParametroType();

    if (utenteAbilitatoInserimentoModifica === "True") {

        popolaAzioniDettaglioInserimentoModifica(colonneKendoGrid, type);


    } else {

        popolaAzioniDettaglioSolaLettura(colonneKendoGrid, type);

    }





    var parametriPerLettura = null;
    var parametriDataSource = {
        serverFiltering: false,
        aggregate: [
            { field: "Qta", aggregate: "sum" },
            { field: "Qta_Evasa", aggregate: "sum" },
            { field: "Qta_Residua", aggregate: "sum" },
            { field: "Kg_Netti", aggregate: "sum" },
            { field: "Degrado", aggregate: "sum" },
            { field: "Netto_Pagamento", aggregate: "sum" },
            { field: "Tara_Totale", aggregate: "sum" },
            { field: "Kg_Lordi", aggregate: "sum" },
            { field: "Imponibile_Netto", aggregate: "sum" },
            { field: "Iva", aggregate: "sum" },
            { field: "Importo", aggregate: "sum" },
            { field: "Provvigione_Calcolata", aggregate: "sum" }
        ]
    };
    var parametriKendoGrid = {
        columnMenu: true,
        editable: false,
        groupable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
        reorderable: true,
        toolbarCommands: []
    };

    let ricercaConferimenti = DocContab_TipoRicerca_Conferimenti == ParametroType().toUpperCase();
    if (IsFatturazione() || hasGestioneWorkflow() || ricercaConferimenti === true) {
        funzioniCRUD.checkBoxFunction = kEventoSelezionaRiga;

        if (IsFatturazione()) {
            parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_Fatturazione_Dettaglio");
        }

        if (hasGestioneWorkflow()) {
            parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_PassaggioStato");
            if (setupIB_Controlli == true) {
                parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_IBControlliInvio");
            }
        }

        if (ricercaConferimenti === true) {
            parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_ValorizzazioneConferimenti");
        }
    }

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundingDettaglioDocumenti, funzioneDaChiamarePrimaDiExcelExport: onExportExcel };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    var grid = creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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
    ).data("kendoGrid");

    grid.bind("columnMenuInit", gridDettaglio_columnMenuInit);

}

function popolaAzioniDettaglioInserimentoModifica(colonneKendoGrid, type) {

    if (type !== DocContab_TipoRicerca_Conferimenti) {

        colonneKendoGrid.unshift({
            command: [
                {
                    template: templateStampaDocumento,
                    visible: function (dataItem) {
                        return dataItem.Lav_Cod !== lavCod_ContrattoAffitto;
                    }
                },
                {
                    template: templateModificaDocumento,
                    visible: function (dataItem) { return dataItem.Blocco_Flag !== 1 && workflowPermessoModCanc(dataItem.Permesso_Modifica_Workflow); }
                },
                {
                    template: templateEliminaDocumento,
                    visible: function (dataItem) { return dataItem.Blocco_Flag !== 1 && workflowPermessoModCanc(dataItem.Permesso_Cancella_Workflow); }
                },
                {
                    template: templateVisualizzaDocumento
                },
                {
                    template: templateGestioneAllegati,
                    visible: function (dataItem) {
                        return dataItem.DocumentiPresenti == true;
                    }
                },
                {
                    template: templateNuovoAllegato,
                    visible: function (dataItem) {
                        return $("input[name$='hf_UtenteAbilitatoGestioneNuovoAllegato']").val() == "True";
                    }
                }
            ], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "260px"
        });

    } else {

        colonneKendoGrid.unshift({
            command: [
                {
                    template: templateStampaDocumento
                },
                {
                    template: templateStampaEtichetteDettaglio
                },
                {
                    template: templateModificaDocumento,
                    visible: function (dataItem) { return dataItem.Blocco_Flag !== 1 && workflowPermessoModCanc(dataItem.Permesso_Modifica_Workflow) && (dataItem.Tipo_Accettazione != -1 || utenteAbilitatoPomodoro); }
                },
                {
                    template: templateEliminaDocumento,
                    visible: function (dataItem) { return dataItem.Blocco_Flag !== 1 && workflowPermessoModCanc(dataItem.Permesso_Cancella_Workflow) && (dataItem.Tipo_Accettazione != -1 || utenteAbilitatoPomodoro); }
                },
                {
                    template: templateVisualizzaDocumento
                },
                {
                    template: templateCampionamento
                },
                {
                    template: templateGestioneAllegati,
                    visible: function (dataItem) {
                        return dataItem.DocumentiPresenti == true;
                    }
                },
                {
                    template: templateNuovoAllegato,
                    visible: function (dataItem) {
                        return $("input[name$='hf_UtenteAbilitatoGestioneNuovoAllegato']").val() == "True";
                    }
                }
            ], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "260px"
        });

    }

}

function popolaAzioniDettaglioSolaLettura(colonneKendoGrid, type) {

    switch (type.toUpperCase()) {

        case DocContab_TipoRicerca_Contratti:

            colonneKendoGrid.unshift({
                command: [{
                    template: templateVisualizzaDocumento
                }], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "130px"
            });

            break;

        case DocContab_TipoRicerca_Conferimenti:

            colonneKendoGrid.unshift({
                command: [
                    {
                        template: templateVisualizzaDocumento
                    },
                    {
                        template: templateStampaDocumento
                    },
                    {
                        template: templateStampaEtichetteDettaglio,
                        visible: function (dataItem) {
                            return (dataItem.Lav_Cod != undefined && dataItem.Lav_Cod == 1078);
                        }
                    }
                ], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "165px"
            });

            break;

        default:

            colonneKendoGrid.unshift({
                command: [{
                    template: templateVisualizzaDocumento + templateStampaDocumento
                }], title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"), width: "130px"
            });

    }

}

function gridDettaglio_columnMenuInit(e) {

    if (e.field === "Referenza_Descr") {

        var filterMultiCheck = e.container.find(".k-filterable").data("kendoFilterMultiCheck")
        filterMultiCheck.container.empty();
        filterMultiCheck.checkSource.sort({ field: e.field, dir: "asc" });

        filterMultiCheck.checkSource.data(filterMultiCheck.checkSource.view().toJSON());
        filterMultiCheck.createCheckBoxes();

    }
}

function colonneKendoGridDettaglio(IDControllo) {
    var colonneKendoGrid = [];
    var doc_type = ParametroDocType();
    switch (IDControllo) {

        case "tab_dettaglio_griglia_report_vendite":
            var visibleDocAllegati = VisibleDocAllegatiColumn();
            colonneKendoGrid = [
                { field: "Des_Lib", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), width: 400 },
                { field: "DocumentiPresenti", title: TraduzioneMultiResx(resxObj, "DocAllegati", "Doc. Allegati"), template: '<input type="checkbox" #= DocumentiPresenti ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, hidden: visibleDocAllegati, width: 120 },
                { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Cliente", "Cliente"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Rapporto", title: TraduzioneMultiResx(resxObj, "TipoRapportoContabileAbbr", "Tipo Rap. Contab."), filterable: { multi: true, search: true } },
                { field: "Data_Movimento", title: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc."), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc.") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Tipo_Documento", title: TraduzioneMultiResx(resxObj, "TipoDocumentoAbbr", "Tipo Doc."), filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: TraduzioneMultiResx(resxObj, "NrDocumentoAbbr", "Nr. Doc."), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Riga", title: TraduzioneMultiResx(resxObj, "Riga", "Riga"), attributes: { style: "text-align:center;" }, width: 100 },
                { field: "Referenza_Descr", title: TraduzioneMultiResx(resxObj, "Prodotto", "Prodotto"), filterable: { multi: true, search: true }, width: 200 },
                { field: "Unita_Misura_Sigla", title: TraduzioneMultiResx(resxObj, "UnitàDiMisuraAbbr", "UdM"), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Qta", title: TraduzioneMultiResx(resxObj, "RisorsaQuantità", "Quantità"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Kg_Netti", title: TraduzioneMultiResx(resxObj, "QuantitàTotale", "Quantità Totale"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 150 },
                { field: "Prezzo_Netto", title: TraduzioneMultiResx(resxObj, "PrezzoNetto", "Prezzo Netto"), format: "{0:n6}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" } },
                { field: "Imponibile_Netto", title: TraduzioneMultiResx(resxObj, "ImponibileNetto", "Imponibile Netto"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Iva", title: TraduzioneMultiResx(resxObj, "Iva", "Iva"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Aliquota_Iva_Des", title: TraduzioneMultiResx(resxObj, "AliquotaIva", "Aliquota Iva"), filterable: { multi: true, search: true }, width: 200 },
                { field: "Importo", title: TraduzioneMultiResx(resxObj, "Importo", "Importo"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Sa_Nome", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: TraduzioneMultiResx(resxObj, "CodiceClienteAbbr", "Cod. Cliente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Stato", title: TraduzioneMultiResx(resxObj, "Stato", "Stato"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione", title: TraduzioneMultiResx(resxObj, "Regione", "Regione"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia", title: TraduzioneMultiResx(resxObj, "Provincia", "Provincia"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune", title: TraduzioneMultiResx(resxObj, "Comune", "Comune"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Destinazione", title: TraduzioneMultiResx(resxObj, "Destinazione", "Destinazione"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Stato_Dest", title: TraduzioneMultiResx(resxObj, "StatoDestinazioneAbbr", "Stato destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione_Dest", title: TraduzioneMultiResx(resxObj, "RegioneDestinazioneAbbr", "Regione destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia_Dest", title: TraduzioneMultiResx(resxObj, "ProvinciaDestinazioneAbbr", "Provincia destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune_Dest", title: TraduzioneMultiResx(resxObj, "ComuneDestinazioneAbbr", "Comune destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Vettore_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Vettore", "Vettore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Agente_Codice", title: TraduzioneMultiResx(resxObj, "CodiceAgenteAbbr", "Cod. Agente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Agente_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Agente", "Agente"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Capoarea_Codice", title: TraduzioneMultiResx(resxObj, "CodiceCapoAreaAbbr", "Cod. Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Capoarea_RagioneSociale", title: TraduzioneMultiResx(resxObj, "CapoArea", "Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Anno_Movimento", title: TraduzioneMultiResx(resxObj, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: TraduzioneMultiResx(resxObj, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: TraduzioneMultiResx(resxObj, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Note", title: TraduzioneMultiResx(resxObj, "Note", "Note"), hidden: true, width: 150 },
                { field: "Referenza_Codice", title: TraduzioneMultiResx(resxObj, "CodiceProdotto", "Codice Prodotto"), hidden: true, width: 150 },
                { field: "Categoria_Prodotto", title: TraduzioneMultiResx(resxObj, "CategoriaProdotto", "Categoria Prodotto"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Categoria_Commerciale", title: TraduzioneMultiResx(resxObj, "CategoriaCommerciale", "Categoria Commerciale"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Gruppi_Merce", title: TraduzioneMultiResx(resxObj, "GruppoMerce", "Gruppo Merce"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Note_Prodotto", title: TraduzioneMultiResx(resxObj, "DescrizioneAddizionale", "Descrizione Addizionale"), hidden: true, width: 150 },
                { field: "Lotto", title: TraduzioneMultiResx(resxObj, "Lotto", "Lotto"), filterable: { multi: true, search: true }, hidden: false, width: 150 },
                { field: "Unita_Misura_Secondaria_Sigla", title: TraduzioneMultiResx(resxObj, "UnitàDiMisuraSecondariaAbbr", "UdM Sec."), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true }, hidden: true, width: 100 },
                {
                    field: "Qta_Extra",
                    title: TraduzioneMultiResx(resxObj, "Litri", "Litri") + " / kg", format: "{0:n2}", attributes: { style: "text-align:right;" }, hidden: true, width: 100
                },
                { field: "Tara_Totale", title: TraduzioneMultiResx(resxObj, "Tara", "Tara"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Kg_Lordi", title: TraduzioneMultiResx(resxObj, "KgLordi", "Kg Lordi"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Nr_Imballi", title: TraduzioneMultiResx(resxObj, "NrImballi", "Nr. Imballi"), attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Nr_Contenitori", title: TraduzioneMultiResx(resxObj, "NrContenitori", "Nr. Contenitori"), attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                // { field: "Nr_Confezioni", title: "Nr. Confezioni", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Prezzo_Riferito_A", title: TraduzioneMultiResx(resxObj, "PrezzoRiferitoA", "Prezzo Rif. A"), gruppoColonne: gruppoColonneEconomico, filterable: { multi: true, search: true }, hidden: true, width: 100 },
                { field: "Imponibile", title: TraduzioneMultiResx(resxObj, "Imponibile", "Imponibile"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Sconto_Perc", title: TraduzioneMultiResx(resxObj, "ScontoPercentuale", "Sconto %"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Sconto", title: TraduzioneMultiResx(resxObj, "Sconto", "Sconto"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Provvigione", title: TraduzioneMultiResx(resxObj, "ProvvigionePercentuale", "Provvigione %"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, hidden: true, width: 100 },
                { field: "Provvigione_Calcolata", title: TraduzioneMultiResx(resxObj, "Provvigione", "Provvigione"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Scadenza", title: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza"), template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza") + ': #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 }
            ];
            if (doc_type === DocContab_TipoDoc_Ordine)
            {
                colonneKendoGrid.push({ field: "Qta_Evasa", title: TraduzioneMultiResx(resxObj, "QuantitaEvasa", "Quantità Evasa"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" });
                colonneKendoGrid.push({ field: "Qta_Residua", title: TraduzioneMultiResx(resxObj, "QuantitaResidua", "Quantità Residua"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" });
                colonneKendoGrid.push({ field: "StatoEvasione_Des", title: TraduzioneMultiResx(resxObj, "StatoEvasione", "Stato Evasione"), filterable: { multi: true, search: true }, hidden: false, width: 150 });
                //colonneKendoGrid.push({ field: "Qta_Netta_Evasa", title: "Quantità  Totale Evasa", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Netta_Residua", title: "Quantità  Totale Residua", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Tara_Evasa", title: "Tara Evasa", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Tara_Residua", title: "Tara Residua", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Lorda_Evasa", title: "Kg Lordi Evasi", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Lorda_Residua", title: "Kg Lordi Residui", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Imballi_Evasa", title: "Nr. Imballi Evasi", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Imballi_Residua", title: "Nr. Imballi Residui", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Contenitori_Evasa", title: "Nr. Contenitori Evasi", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Contenitori_Residua", title: "Nr. Contenitori Residui", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
            }
            if (IsFatturazione())
            {
                colonneKendoGrid.unshift({ field: "Mov_non_Fatturati", title: TraduzioneMultiResx(resxObj, "NonFatturatoSigla", "NF"), filterable: { multi: true, search: true }, width: 100, hidden: true });
                colonneKendoGrid.unshift({ field: "Mov_Fatturati_Parzialmente", title: TraduzioneMultiResx(resxObj, "FatturatoParzialmenteSigla", "FP"), filterable: { multi: true, search: true }, width: 100, hidden: true });
                colonneKendoGrid.unshift({ field: "Stato_Fatturazione", title: TraduzioneMultiResx(resxObj, "StatoFatturazione", "Stato"), filterable: { multi: true, search: true }, width: 150, hidden: true });
            }
            
            break;

        case "tab_dettaglio_griglia_report_acquisti":
            var visibleDocAllegati = VisibleDocAllegatiColumn();
            colonneKendoGrid = [
                { field: "Des_Lib", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), width: 400 },
                { field: "DocumentiPresenti", title: TraduzioneMultiResx(resxObj, "DocAllegati", "Doc. Allegati"), template: '<input type="checkbox" #= DocumentiPresenti ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, hidden: visibleDocAllegati, width: 120 },
                { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Fornitore", "Fornitore"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Rapporto", title: TraduzioneMultiResx(resxObj, "TipoRapportoContabileAbbr", "Tipo Rap. Contab."), filterable: { multi: true, search: true } },
                { field: "Data_Movimento", title: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc."), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc.") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Tipo_Documento", title: TraduzioneMultiResx(resxObj, "TipoDocumentoAbbr", "Tipo Doc."), filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: TraduzioneMultiResx(resxObj, "NrDocumentoAbbr", "Nr. Doc."), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Riga", title: TraduzioneMultiResx(resxObj, "Riga", "Riga"), attributes: { style: "text-align:center;" }, width: 100 },
                { field: "Referenza_Descr", title: TraduzioneMultiResx(resxObj, "Prodotto", "Prodotto"), filterable: { multi: true, search: true }, width: 200 },
                { field: "Unita_Misura_Sigla", title: TraduzioneMultiResx(resxObj, "UnitàDiMisuraAbbr", "UdM"), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Qta", title: TraduzioneMultiResx(resxObj, "RisorsaQuantità", "Quantità"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Kg_Netti", title: TraduzioneMultiResx(resxObj, "QuantitàTotale", "Quantità Totale"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 150 },
                { field: "Prezzo_Netto", title: TraduzioneMultiResx(resxObj, "PrezzoNetto", "Prezzo Netto"), format: "{0:n6}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" } },
                { field: "Imponibile_Netto", title: TraduzioneMultiResx(resxObj, "ImponibileNetto", "Imponibile Netto"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Iva", title: TraduzioneMultiResx(resxObj, "Iva", "Iva"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Aliquota_Iva_Des", title: TraduzioneMultiResx(resxObj, "AliquotaIva", "Aliquota Iva"), filterable: { multi: true, search: true }, width: 200 },
                { field: "Importo", title: TraduzioneMultiResx(resxObj, "Importo", "Importo"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Sa_Nome", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: TraduzioneMultiResx(resxObj, "CodiceFornitoreAbbr", "Cod. Fornitore"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Stato", title: TraduzioneMultiResx(resxObj, "Stato", "Stato"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione", title: TraduzioneMultiResx(resxObj, "Regione", "Regione"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia", title: TraduzioneMultiResx(resxObj, "Provincia", "Provincia"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune", title: TraduzioneMultiResx(resxObj, "Comune", "Comune"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Destinazione", title: TraduzioneMultiResx(resxObj, "Destinazione", "Destinazione"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Stato_Dest", title: TraduzioneMultiResx(resxObj, "StatoDestinazioneAbbr", "Stato destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione_Dest", title: TraduzioneMultiResx(resxObj, "RegioneDestinazioneAbbr", "Regione destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia_Dest", title: TraduzioneMultiResx(resxObj, "ProvinciaDestinazioneAbbr", "Provincia destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune_Dest", title: TraduzioneMultiResx(resxObj, "ComuneDestinazioneAbbr", "Comune destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Vettore_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Vettore", "Vettore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Agente_Codice", title: TraduzioneMultiResx(resxObj, "CodiceAgenteAbbr", "Cod. Agente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Agente_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Agente", "Agente"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Capoarea_Codice", title: TraduzioneMultiResx(resxObj, "CodiceCapoAreaAbbr", "Cod. Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Capoarea_RagioneSociale", title: TraduzioneMultiResx(resxObj, "CapoArea", "Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Anno_Movimento", title: TraduzioneMultiResx(resxObj, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: TraduzioneMultiResx(resxObj, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: TraduzioneMultiResx(resxObj, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Note", title: TraduzioneMultiResx(resxObj, "Note", "Note"), hidden: true, width: 150 },
                { field: "Referenza_Codice", title: TraduzioneMultiResx(resxObj, "CodiceProdotto", "Codice Prodotto"), hidden: true, width: 150 },
                { field: "Codice_Esterno", title: TraduzioneMultiResx(resxObj, "CodiceEsterno", "Codice Esterno"), hidden: true, width: 150 },
                { field: "Categoria_Prodotto", title: TraduzioneMultiResx(resxObj, "CategoriaProdotto", "Categoria Prodotto"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Categoria_Commerciale", title: TraduzioneMultiResx(resxObj, "CategoriaCommerciale", "Categoria Commerciale"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Gruppi_Merce", title: TraduzioneMultiResx(resxObj, "GruppoMerce", "Gruppo Merce"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Note_Prodotto", title: TraduzioneMultiResx(resxObj, "DescrizioneAddizionale", "Descrizione Addizionale"), hidden: true, width: 150 },
                { field: "Lotto", title: TraduzioneMultiResx(resxObj, "Lotto", "Lotto"), filterable: { multi: true, search: true }, hidden: false, width: 150 },
                { field: "Unita_Misura_Secondaria_Sigla", title: TraduzioneMultiResx(resxObj, "UnitàDiMisuraSecondariaAbbr", "UdM Sec."), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true }, hidden: true, width: 100 },
                {
                    field: "Qta_Extra",
                    title: TraduzioneMultiResx(resxObj, "Litri", "Litri") + " / kg", format: "{0:n2}", attributes: { style: "text-align:right;" }, hidden: true, width: 100
                },
                { field: "Tara_Totale", title: TraduzioneMultiResx(resxObj, "Tara", "Tara"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Kg_Lordi", title: TraduzioneMultiResx(resxObj, "KgLordi", "Kg Lordi"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Nr_Imballi", title: TraduzioneMultiResx(resxObj, "NrImballi", "Nr. Imballi"), attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Nr_Contenitori", title: TraduzioneMultiResx(resxObj, "NrContenitori", "Nr. Contenitori"), attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                // { field: "Nr_Confezioni", title: "Nr. Confezioni", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Prezzo_Riferito_A", title: TraduzioneMultiResx(resxObj, "PrezzoRiferitoA", "Prezzo Rif. A"), gruppoColonne: gruppoColonneEconomico, filterable: { multi: true, search: true }, hidden: true, width: 100 },
                { field: "Imponibile", title: TraduzioneMultiResx(resxObj, "Imponibile", "Imponibile"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Sconto_Perc", title: TraduzioneMultiResx(resxObj, "ScontoPercentuale", "Sconto %"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Sconto", title: TraduzioneMultiResx(resxObj, "Sconto", "Sconto"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Provvigione", title: TraduzioneMultiResx(resxObj, "ProvvigionePercentuale", "Provvigione %"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, hidden: true, width: 100 },
                { field: "Provvigione_Calcolata", title: TraduzioneMultiResx(resxObj, "Provvigione", "Provvigione"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Scadenza", title: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza"), template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza") + ': #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 }
            ];
            if (doc_type === DocContab_TipoDoc_Ordine) {
                colonneKendoGrid.push({ field: "Qta_Evasa", title: TraduzioneMultiResx(resxObj, "QuantitaEvasa", "Quantità Evasa"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" });
                colonneKendoGrid.push({ field: "Qta_Residua", title: TraduzioneMultiResx(resxObj, "QuantitaResidua", "Quantità Residua"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" });
                colonneKendoGrid.push({ field: "StatoEvasione_Des", title: TraduzioneMultiResx(resxObj, "StatoEvasione", "Stato Evasione"), filterable: { multi: true, search: true }, hidden: false, width: 150 });
                //colonneKendoGrid.push({ field: "Qta_Netta_Evasa", title: "Quantità  Totale Evasa", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Netta_Residua", title: "Quantità  Totale Residua", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Tara_Evasa", title: "Tara Evasa", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Tara_Residua", title: "Tara Residua", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Lorda_Evasa", title: "Kg Lordi Evasi", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Lorda_Residua", title: "Kg Lordi Residui", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Imballi_Evasa", title: "Nr. Imballi Evasi", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Imballi_Residua", title: "Nr. Imballi Residui", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Contenitori_Evasa", title: "Nr. Contenitori Evasi", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                //colonneKendoGrid.push({ field: "Qta_Contenitori_Residua", title: "Nr. Contenitori Residui", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
            }
            break;

        case "tab_dettaglio_griglia_report_conferimenti":
            var cess1_2_hidden = ($(ccontattiAcc4ConGerarchia).val() === "False");
            var visibleDocAllegati = VisibleDocAllegatiColumn();
            colonneKendoGrid = [
                { field: "Des_Lib", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), width: 400 },
                { field: "DocumentiPresenti", title: TraduzioneMultiResx(resxObj, "DocAllegati", "Doc. Allegati"), template: '<input type="checkbox" #= DocumentiPresenti ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', attributes: { style: "text-align:center;" }, hidden: visibleDocAllegati, width: 120 },
                { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Conferente", "Conferente"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Piva", title: TraduzioneMultiResx(resxObj, "PartitaIVA", "Partita IVA"), filterable: { multi: true, search: true }, width: 120 },
                { field: "Soggetto_Rapporto", title: TraduzioneMultiResx(resxObj, "TipoRapportoContabileAbbr", "Tipo Rap. Contab."), filterable: { multi: true, search: true } },
                { field: "Soggetto_Cess1_RagioneSociale", title: TraduzioneMultiResx(resxObj, "PrimoCessionarioAbbr", "1° Cessionario"), filterable: { multi: true, search: true }, hidden: cess1_2_hidden, width: 300 },
                { field: "Soggetto_Cess2_RagioneSociale", title: TraduzioneMultiResx(resxObj, "SecondoCessionarioAbbr", "2° Cessionario"), filterable: { multi: true, search: true }, hidden: cess1_2_hidden, width: 300 },
                { field: "Data_Movimento", title: TraduzioneMultiResx(resxObj, "DataEmissione", "Data Emissione"), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Data Conf.: #= kendo.toString(value,'dd/MM/yyyy') #", width: 120 },
                { field: "Ora_Movimento", title: TraduzioneMultiResx(resxObj, "Ora", "Ora"), attributes: { style: "text-align:center;" }, width: 80 },
                { field: "Tipo_Documento", title: TraduzioneMultiResx(resxObj, "TipoDocumentoAbbr", "Tipo Doc."), filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: TraduzioneMultiResx(resxObj, "NumAccettazione", "N° Accettazione"), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Data_Movimento_DDT", title: TraduzioneMultiResx(resxObj, "DataDDT", "Data DDT"), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataDDT", "Data DDT") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Numero_Movimento_DDT", title: TraduzioneMultiResx(resxObj, "NrDDT", "Nr. DDT"), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Riga", title: TraduzioneMultiResx(resxObj, "Riga", "Riga"), attributes: { style: "text-align:center;" }, width: 100 },
                { field: "Referenza_Descr", title: TraduzioneMultiResx(resxObj, "Prodotto", "Prodotto"), filterable: { multi: true, search: true }, width: 200 },
                { field: "Veg_Cod", title: TraduzioneMultiResx(resxObj, "CodiceSpecie", "Codice Specie"), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Veg_Des", title: TraduzioneMultiResx(resxObj, "Specie", "Specie"), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Cul_Cod", title: TraduzioneMultiResx(resxObj, "CodiceVarieta", "Codice Varietà"), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Cul_Des", title: TraduzioneMultiResx(resxObj, "Varietà", "Varietà"), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Codice_Esterno", title: TraduzioneMultiResx(resxObj, "CodiceEsterno", "Codice Esterno"), hidden: true, width: 150 },
                { field: "Kg_Netti", title: TraduzioneMultiResx(resxObj, "KgNetti", "Kg Netti"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 150 },
                { field: "Degrado_Perc", title: TraduzioneMultiResx(resxObj, "PercentualeDegrado", "% Degrado"), format: "{0:n2}", attributes: { style: "text-align:right;" }, width: 150 },
                { field: "Degrado", title: TraduzioneMultiResx(resxObj, "KgDegrado", "Kg Degrado"), format: "{0:n0}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 150 },
                { field: "Netto_Pagamento", title: TraduzioneMultiResx(resxObj, "PesoAPagamentoKg", "Peso A Pagamento Kg"), format: "{0:n0}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", width: 150 },
                { field: "Prezzo_Netto", title: TraduzioneMultiResx(resxObj, "PrezzoNetto", "Prezzo Netto"), format: "{0:n6}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" } },
                { field: "Imponibile_Netto", title: TraduzioneMultiResx(resxObj, "ImponibileNetto", "Imponibile Netto"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Iva", title: TraduzioneMultiResx(resxObj, "Iva", "Iva"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Aliquota_Iva_Des", title: TraduzioneMultiResx(resxObj, "AliquotaIva", "Aliquota Iva"), filterable: { multi: true, search: true }, width: 200 },
                { field: "Importo", title: TraduzioneMultiResx(resxObj, "Importo", "Importo"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Sa_Nome", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: TraduzioneMultiResx(resxObj, "CodiceFornitoreAbbr", "Cod. Fornitore"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Stato", title: TraduzioneMultiResx(resxObj, "Stato", "Stato"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione", title: TraduzioneMultiResx(resxObj, "Regione", "Regione"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia", title: TraduzioneMultiResx(resxObj, "Provincia", "Provincia"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune", title: TraduzioneMultiResx(resxObj, "Comune", "Comune"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Destinazione", title: TraduzioneMultiResx(resxObj, "Produttore", "Produttore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Stato_Dest", title: TraduzioneMultiResx(resxObj, "StatoProduttoreAbbr", "Stato Produt."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione_Dest", title: TraduzioneMultiResx(resxObj, "RegioneProduttoreAbbr", "Regione Produt."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia_Dest", title: TraduzioneMultiResx(resxObj, "ProvinciaProduttoreAbbr", "Provincia Produt."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune_Dest", title: TraduzioneMultiResx(resxObj, "ComuneProduttoreAbbr", "Comune Produt."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Vettore_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Vettore", "Vettore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Agente_Codice", title: TraduzioneMultiResx(resxObj, "CodiceAgenteAbbr", "Cod. Agente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Agente_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Agente", "Agente"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Capoarea_Codice", title: TraduzioneMultiResx(resxObj, "CodiceCapoAreaAbbr", "Cod. Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Capoarea_RagioneSociale", title: TraduzioneMultiResx(resxObj, "CapoArea", "Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Anno_Movimento", title: TraduzioneMultiResx(resxObj, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: TraduzioneMultiResx(resxObj, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: TraduzioneMultiResx(resxObj, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Note", title: TraduzioneMultiResx(resxObj, "Note", "Note"), hidden: true, width: 150 },
                { field: "Referenza_Codice", title: TraduzioneMultiResx(resxObj, "CodiceProdotto", "Codice Prodotto"), hidden: true, width: 150 },
                { field: "Categoria_Prodotto", title: TraduzioneMultiResx(resxObj, "CategoriaProdotto", "Categoria Prodotto"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Categoria_Commerciale", title: TraduzioneMultiResx(resxObj, "CategoriaCommerciale", "Categoria Commerciale"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Gruppi_Merce", title: TraduzioneMultiResx(resxObj, "GruppoMerce", "Gruppo Merce"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Note_Prodotto", title: TraduzioneMultiResx(resxObj, "DescrizioneAddizionale", "Descrizione Addizionale"), hidden: true, width: 150 },
                { field: "Lotto", title: TraduzioneMultiResx(resxObj, "Lotto", "Lotto"), filterable: { multi: true, search: true }, hidden: false, width: 150 },
                { field: "Unita_Misura_Secondaria_Sigla", title: TraduzioneMultiResx(resxObj, "UnitàDiMisuraSecondariaAbbr", "UdM Sec."), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true }, hidden: true, width: 100 },
                { field: "Tara_Totale", title: TraduzioneMultiResx(resxObj, "Tara", "Tara"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Kg_Lordi", title: TraduzioneMultiResx(resxObj, "KgLordi", "Kg Lordi"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Nr_Imballi", title: TraduzioneMultiResx(resxObj, "NrImballi", "Nr. Imballi"), attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Nr_Contenitori", title: TraduzioneMultiResx(resxObj, "NrContenitori", "Nr. Contenitori"), attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                // { field: "Nr_Confezioni", title: "Nr. Confezioni", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Prezzo_Riferito_A", title: TraduzioneMultiResx(resxObj, "PrezzoRiferitoA", "Prezzo Rif. A"), gruppoColonne: gruppoColonneEconomico, filterable: { multi: true, search: true }, hidden: true, width: 100 },
                { field: "Imponibile", title: TraduzioneMultiResx(resxObj, "Imponibile", "Imponibile"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Sconto_Perc", title: TraduzioneMultiResx(resxObj, "ScontoPercentuale", "Sconto %"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Sconto", title: TraduzioneMultiResx(resxObj, "Sconto", "Sconto"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, hidden: true, width: 100 },
                { field: "Provvigione", title: TraduzioneMultiResx(resxObj, "ProvvigionePercentuale", "Provvigione %"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, hidden: true, width: 100 },
                { field: "Provvigione_Calcolata", title: TraduzioneMultiResx(resxObj, "Provvigione", "Provvigione"), format: "{0:n2}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Scadenza", title: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza"), template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "Scadenza", "Scadenza") + ': #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 }
            ];
            break;

        case "tab_dettaglio_griglia_report_contratti":
            colonneKendoGrid = [
                { field: "Des_Lib", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), width: 400 },
                { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(resxObj, "Locatore", "Locatore"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Data_Movimento", title: TraduzioneMultiResx(resxObj, "DataRegistrazioneAbbr", "Data Registr."), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataRegistrazioneAbbr", "Data Registr.") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Tipo_Documento", title: TraduzioneMultiResx(resxObj, "TipoDocumentoAbbr", "Tipo Doc."), hidden: true, filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: TraduzioneMultiResx(resxObj, "NrRegistrazioneAbbr", "Nr. Reg."), attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Data_Iniz_Val_Contratto", title: TraduzioneMultiResx(resxObj, "InizioValidità", "Inizio Validità"), hidden: true, format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Inizio Val.: #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Data_Fine_Val_Contratto", title: TraduzioneMultiResx(resxObj, "FineValidità", "Fine Validità"), hidden: true, format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Fine Val.: #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Numero_Movimento_DDT", title: TraduzioneMultiResx(resxObj, "NumInterno", "Num. Interno"), hidden: true, attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Riga", title: TraduzioneMultiResx(resxObj, "Riga", "Riga"), attributes: { style: "text-align:center;" }, width: 100 },
                { field: "Referenza_Descr", title: TraduzioneMultiResx(resxObj, "DescrizioneProdotto", "Descrizione Prodotto"), filterable: { multi: true, search: true }, width: 200 },
                { field: "Note_Prodotto", title: TraduzioneMultiResx(resxObj, "RiferimentoProdotto", "Riferimento Prodotto"), width: 150 },
                { field: "Prezzo_Netto", title: TraduzioneMultiResx(resxObj, "Prezzo", "Prezzo"), format: "{0:n6}", gruppoColonne: gruppoColonneEconomico, attributes: { style: "text-align:right;" } },
                { field: "Sa_Nome", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: TraduzioneMultiResx(resxObj, "CodLocatore", "Cod. Locatore"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Anno_Movimento", title: TraduzioneMultiResx(resxObj, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: TraduzioneMultiResx(resxObj, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: TraduzioneMultiResx(resxObj, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Altri_Locatori_Contratto", title: TraduzioneMultiResx(resxObj, "AltriLocatori", "Altri Locatori"), hidden: true, width: 300 },
                { field: "Riferimento_Ordini_Contratto", title: TraduzioneMultiResx(resxObj, "RiferimentoOrdini", "Riferimento Ordini"), hidden: true, width: 300 },
                { field: "Note", title: TraduzioneMultiResx(resxObj, "Note", "Note"), hidden: true, width: 150 },
                { field: "Referenza_Codice", title: TraduzioneMultiResx(resxObj, "CodiceProdotto", "Codice Prodotto"), hidden: true, width: 150 }
            ];
            break;

    }

    SeEliminaColonneGruppoEconomico(colonneKendoGrid);

    SeInserisciColonnaPraticaStatoDes(colonneKendoGrid);

    if ([DocContab_TipoRicerca_Acquisti, DocContab_TipoRicerca_Conferimenti, DocContab_TipoRicerca_Vendite].includes(ParametroType()) &&
        [DocContab_TipoDoc_Consegna, DocContab_TipoDoc_Fattura].includes(doc_type)) {

        colonneKendoGrid.push({ field: "Numero_Ordine", title: TraduzioneMultiResx(resxObj, "NumeroOrdine", "Numero Ordine"), filterable: { multi: true, search: true }, width: 80 });
        colonneKendoGrid.push({ field: "Data_Ordine", title: TraduzioneMultiResx(resxObj, "DataOrdine", "Data Ordine"), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: TraduzioneMultiResx(resxObj, "DataOrdine", "Data Ordine") + ": #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 });
    }

    InserisciColonneFinali(colonneKendoGrid);

    return colonneKendoGrid;

}
    
function Prepara_Nuovo_Documento ()
{
    $('#modalNuovo').modal('show');
}

function VisibleDocAllegatiColumn() {
    var result = true;
    if ($("input[name$='hf_UtenteAbilitatoGestioneVisualizaAllegato']").val() == "True") {
        result = false;
    }
    return result;
}

function Inizializza_DropDown_NuoviOrdini(obj) {

    ddlCentri = $(obj).kendoDropDownList({
        autoBind: true,
        filter: "contains",
        dataTextField: "LAV_DES",
        dataValueField: "LAV_COD",
        dataSource: { transport: { read: leggiDocumentiOrdini } },
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {
            kendoDropDownAdjustWidth(e);
        },
        change: function (e) {
            $("#ddl_nuovoDocAcquisto").data("kendoDropDownList").select(0);
            $("#ddl_nuovoDocVendita").data("kendoDropDownList").select(0);

            var lav_cod = $("#ddl_nuovoOrdine").data("kendoDropDownList").value();
            Abilita_Nuovo_Documento(lav_cod);
        },
        optionLabel: {
            LAV_DES: "Seleziona",
            LAV_COD: "0"
        }
    }).data("kendoDropDownList");
}

function Inizializza_DropDown_Nuovi_Documenti_Vendita(obj) {

    ddlCentri = $(obj).kendoDropDownList({
        autoBind: true,
        filter: "contains",
        dataTextField: "LAV_DES",
        dataValueField: "LAV_COD",
        dataSource: { transport: { read: leggiDocumentiVendite } },
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {
            kendoDropDownAdjustWidth(e);
        },
        change: function (e) {
            $("#ddl_nuovoOrdine").data("kendoDropDownList").select(0);
            $("#ddl_nuovoDocAcquisto").data("kendoDropDownList").select(0);

            var lav_cod = $("#ddl_nuovoDocVendita").data("kendoDropDownList").value();
            Abilita_Nuovo_Documento(lav_cod);
        },
        optionLabel: {
            LAV_DES: "Seleziona",
            LAV_COD: "0"
        }
    }).data("kendoDropDownList");
}

function Inizializza_DropDown_Nuovi_Documenti_Acquisto(obj) {

    ddlCentri = $(obj).kendoDropDownList({
        autoBind: true,
        filter: "contains",
        dataTextField: "LAV_DES",
        dataValueField: "LAV_COD",
        dataSource: { transport: { read: leggiDocumentiAcquisti } },
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {
            kendoDropDownAdjustWidth(e);
        },
        change: function (e) {
            $("#ddl_nuovoOrdine").data("kendoDropDownList").select(0);
            $("#ddl_nuovoDocVendita").data("kendoDropDownList").select(0);

            var lav_cod = $("#ddl_nuovoDocAcquisto").data("kendoDropDownList").value();
            Abilita_Nuovo_Documento(lav_cod);

        },
        optionLabel: {
            LAV_DES: "Seleziona",
            LAV_COD: "0"
        }
    }).data("kendoDropDownList");
}

function Abilita_Nuovo_Documento(lav_cod)
{
    if (UtenteAbilitatoScrittura() == false)
        $("#btn_prepara_nuovo_doc").hide();
    else {
        if (lav_cod != "0")
            $("#btn_prepara_nuovo_doc").show();
        else
            $("#btn_prepara_nuovo_doc").hide();
    }
}

function Esegui_Report(daFiltri) {
    trovatoErrore = false;
    stoRipristinandoFiltri = daFiltri;
    dataDaControllare = $('input[name$="Txt_DataRegDal"]').val();
    dataValida = true;
    if (dataDaControllare !== "")
        dataValida = isValidDate(dataDaControllare);
    if (!dataValida) {
        MessaggioErrore_Bootstrap("Da data movimento non valida", "DIV_Messaggi");
        trovatoErrore = true;
    }

    dataDaControllare = $('input[name$="Txt_DataRegAl"]').val();
    dataValida = true;
    if (dataDaControllare !== "")
        dataValida = isValidDate(dataDaControllare);
    if (!dataValida) {
        MessaggioErrore_Bootstrap("A data movimento non valida", "DIV_Messaggi");
        trovatoErrore = true;
    }

    var IDControllo = "";
    if (!trovatoErrore) {
        if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 1) {
            // Griglia Dettaglio
            $(".gridAreaDettaglio").show();
            $(".gridAreaTestata").hide();
            $("#tab3").show();
            ImpostaDefaultFiltri();
            IDControllo = DammiIDControlloGriglia(true);
            popolaGrigliaReportDettaglio(IDControllo);

            $("#" + IDControllo).kendoTooltip({
                filter: 'span[title]',
                position: "top"
            });
            
        }
        else if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 0)
        {
            // Griglia Testata
            $(".gridAreaTestata").show();

            IDControllo = DammiIDControlloGriglia(false);
            var gt = popolaGrigliaReportTestata(IDControllo);
            if (gt != undefined && gt != null)
                gt.data("kendoGrid").bind("change", onChangeGrigliaTestata)

            $(".gridAreaDettaglio").hide();
            $("#tab1").click();
            $("#tab3").hide();

            $("#" + IDControllo).kendoTooltip({
                filter: 'span[title]',
                position: "top"
            });

        } else if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 2)
                {
                    // Report PDF
                    var errMsg = checkReportPDF();
                    
                    if (errMsg === "") { }
                    else
                        MessaggioErrore_Bootstrap(errMsg, "DIV_Messaggi");

        }
        //Applica_Personalizzazioni_Griglie();
    }
}

function checkReportPDF() {
    var errMsg = "";

    if (KendoDDL("sel_tipo_report_vendite").value() === "-1") {
        errMsg += "Selezionare il TIPO REPORT da lanciare <br/>";
    }

    if ($("#livelliScelti").data("kendoListBox").dataSource._data.length === 0) {
        errMsg += "Scegliere almeno un livello da stampare <br/>";
    } 

    if ($("#TxtTitolo").val()  === "") {
        errMsg += "Digitare il titolo <br/>";
    }

    if ($("#Txt_da_mese").val() === 0 ||
        $("#Txt_da_mese").val() === "" ||
        $("#Txt_da_anno").val() === 0 ||
        $("#Txt_da_anno").val() === "") {
        errMsg += "Impostare MESE / ANNO INIZIALE DI RIFERIMENTO <br/>";
    }

    if (getKendoSwitch("cb_confronto_anno")) {
        if ($("#Txt_confronto_anno").val() === 0 ||
            $("#Txt_confronto_anno").val() === "") {
            errMsg += "Impostare il campo A PARTIRE DALL'ANNO <br/>";
        }
    } else {
        $("#Txt_confronto_anno").val(0);
    }

    if (KendoDDL("sel_tipo_valore").value() === "-1") {
        errMsg += "Selezionare il VALORE DA CONSIDERARE <br/>";
    } else {

        // Controllo che non venga scelta la quantità generica (che potrebbe contenere quantitù relative a unità di misura
        // non sommabili fra loro se non viene scelto il livello prodotto
        if ($("#livelliScelti").data("kendoListBox").dataSource._data.length !== 0 && 
            KendoDDL("sel_tipo_valore").value() === "0") {
            var foundProdotto = false;
            for (var y = 0; y < $("#livelliScelti").data("kendoListBox").dataSource._data.length; y++) {
                    if ( $("#livelliScelti").data("kendoListBox").dataSource._data[y].value  === "Prodotto") {
                        foundProdotto = true;
                    }
                }
            
            if (!foundProdotto)
                errMsg += "Non è possibile scegliere come VALORE DA CONSIDERARE la quantità venduta se non si include il Prodotto fra i livelli da stampare <br/>";
                 
        }     
    }



    if (KendoDDL("sel_tipo_report_vendite").value() === "1") {
        if ($("#Txt_mese_prev_scost").val() === 0 ||
            $("#Txt_mese_prev_scost").val() === "") {
            errMsg += "Impostare il campo MESE FINO A CUI CONTROLLARE LO SCOSTAMENTO <br/>";
        }
    }
    
    return errMsg;
}

// click stampa documento
function StampaDocumento(tr_elem, grid_elem) {

    Salva_Filtri();

    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    var Id_Agenda = dataItem.Id_Agenda;
    var Lav_Cod = dataItem.Lav_Cod;
    var Modulo = dataItem.Modulo;
    var Tipo_Accettazione = dataItem.Tipo_Accettazione;
    stampa_documento(Id_Agenda, Lav_Cod, Modulo, Tipo_Accettazione);
}

// click stampa barcode
function StampaBarCode(tr_elem, grid_elem, dettaglio)
{
    Salva_Filtri();

    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    stampa_barCode(dataItem, dettaglio);

}


function GetIdAgendaInElem(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    return dataItem;
}


// Click modifica documento
function ApriModificaDocumento(tr_elem, grid_elem, operazione) {


    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    var Id_Agenda = dataItem.Id_Agenda;
    var Lav_Cod = dataItem.Lav_Cod;
    var Blocco_Flag = dataItem.Blocco_Flag;
    var Data_Movimento = dataItem.Data_Movimento;

    //if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 0) {
    //    ultimaRigaSelezionataGrigliaTestata.rowIndex = -1;
    //    ultimaRigaSelezionataGrigliaTestata.id = Id_Agenda;
    //}
    //else
    //{
    //    var id_mov_det = dataItem.Id_Mov_Det;
    //    ultimaRigaSelezionataGrigliaDettaglio.rowIndex = -1;
    //    ultimaRigaSelezionataGrigliaDettaglio.id = id_mov_det;
    //}

    if (Blocco_Flag === 1 && operazione === 2) {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
            messages: { okText: TraduzioneMultiResx(resxObj, "Sì", "Sì"), cancel: TraduzioneMultiResx(resxObj, "No", "No") },
            content: "Il documento non è modificabile. Aprirlo in sola consultazione ?"
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () {
            aprimodifica_documento(Id_Agenda, Lav_Cod, 0, Data_Movimento);
        });

        kendoConfirm.open();
    }
    else
    {
        aprimodifica_documento(Id_Agenda, Lav_Cod, operazione, Data_Movimento);
    }
    
}

// Click elimina documento
function EliminaDocumento(tr_elem, grid_elem) {

    var dataItem = $(grid_elem).data("kendoGrid").dataItem(tr_elem);
    var Id_Agenda = dataItem.Id_Agenda;
    var Lav_Cod = dataItem.Lav_Cod;
    var Data_Movimento = dataItem.Data_Movimento;

    $("#confermaEliminazioneDialog").kendoDialog({
        width: "400px",
        title: TraduzioneMultiResx(resxObj, "GestioneCancellazioneDocumenti", "Gestione Cancellazione Documenti"),
        closable: false,
        modal: true,
        visible: false,
        content: "<p>" + TraduzioneMultiResx(resxObj, "ConfermaEliminazioneDocumento", "Eliminare definitivamente questo documento?") + "<p>",
        actions: [
            { text: TraduzioneMultiResx(resxObj, "Conferma", "Conferma"), action: function (e) { elimina_documento(Id_Agenda, Lav_Cod, false, Data_Movimento); } },
            { text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla"), primary: true }
        ]
    });
    $("#confermaEliminazioneDialog").data("kendoDialog").open();

}

// Click sblocca documento
function SbloccaDocumento(tr_elem, grid_elem) {

    var dataItem = $(grid_elem).data("kendoGrid").dataItem(tr_elem);
    var piva = dataItem.PIVA;
    var saCod = dataItem.Sa_Cod_Agenda;
    var idAgenda = dataItem.Id_Agenda;

    if ($(cUtenteAbilitatoSblocco).val() === "True") {
        $("#confermaSbloccoDialog").kendoDialog({
            width: "400px",
            title: TraduzioneMultiResx(resxObj, "GestioneSbloccoDocumenti", "Gestione Sblocco Documenti"),
            closable: false,
            modal: true,
            visible: false,
            content: "<p>" + TraduzioneMultiResx(resxObj, "SbloccareQuestoDocumento", "Sbloccare questo documento?") + "<p>",
            actions: [
                { text: TraduzioneMultiResx(resxObj, "Conferma", "Conferma"), action: function(e) { sblocca_documento(piva, saCod, idAgenda); } },
                { text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla"), primary: true }
            ]
        });
        $("#confermaSbloccoDialog").data("kendoDialog").open();
    } else {
        MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "UtenteSenzaPermessiSbloccoDocContabile",
            "L'utente non dispone dei permessi per sbloccare un documento contabile."), "DIV_Messaggi");
    }

}

// Click blocca documento
function BloccaDocumento(tr_elem, grid_elem) {

    var dataItem = $(grid_elem).data("kendoGrid").dataItem(tr_elem);
    var piva = dataItem.PIVA;
    var saCod = dataItem.Sa_Cod_Agenda;
    var idAgenda = dataItem.Id_Agenda;

    if ($(cUtenteAbilitatoBlocco).val() === "True") {
        $("#confermaBloccoDialog").kendoDialog({
            width: "400px",
            title: TraduzioneMultiResx(resxObj, "GestioneBloccoDocumenti", "Gestione Blocco Documenti"),
            closable: false,
            modal: true,
            visible: false,
            content: "<p>" + TraduzioneMultiResx(resxObj, "BloccareQuestoDocumento", "Bloccare questo documento?") + "<p>",
            actions: [
                { text: TraduzioneMultiResx(resxObj, "Conferma", "Conferma"), action: function(e) { blocca_documento(piva, saCod, idAgenda); } },
                { text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla"), primary: true }
            ]
        });
        $("#confermaBloccoDialog").data("kendoDialog").open();
    } else {
        MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "UtenteSenzaPermessiBloccoDocContabile",
            "L'utente non dispone dei permessi per bloccare un documento contabile."), "DIV_Messaggi");
    }

}

function onExportExcel(e) {
    var rows = e.workbook.sheets[0].rows;
    for (var ri = 0; ri < rows.length; ri++) {
        var row = rows[ri];
        if (row.type === "group-footer" || row.type === "footer") {
            for (var ci = 0; ci < row.cells.length; ci++) {
                var cell = row.cells[ci];
                /* if (Object.prototype.toString.call(cell.value) === "[object Date]") {
                    var d = new Date(cell.value);
                    cell.value = kendo.toString(data, "dd/MM/yyyy");
                } */
                if (cell.value) {
                    // Use jQuery.fn.text to remove the HTML and get only the text
                    cell.value = $(cell.value).text();
                    // Set the alignment
                    cell.hAlign = "right";
                }
            }
        }
    }

}

function SelezionaDocumentiDaTestata(e) {

    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $("#tab_testata_griglia_report_vendite").data("kendoGrid");

    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked);

    if (grid.select().length > 0)
        $("#btn_avvia_fatturazione_testata").show();
    else
        $("#btn_avvia_fatturazione_testata").hide();  
}

function SelezionaDocumentiDaDettaglio(e) {

    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $("#tab_dettaglio_griglia_report_vendite").data("kendoGrid");

    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)

    if (grid.select().length > 0)
        $("#btn_avvia_fatturazione_dettaglio").show();
    else
        $("#btn_avvia_fatturazione_dettaglio").hide();
}

function kEventoSelezionaRiga(e) {
    let checked = this.checked,
        row = $(this).parents("tr").eq(0),
        kGrid = row.parents(".k-grid").eq(0).data("kendoGrid"),
        dataItem = kGrid.dataItem(row);

    dataItem.Selected = checked; // dataItem è per riferimento, di conseguenza viene aggiornato il dataSource della griglia

    rowKendoGridSelected(row, checked)

    let isDettaglio = false;
    if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 1) {
        isDettaglio = true;
    } else if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 0) {
        isDettaglio = false;
    }
    
    let dsKGrid = kGrid.dataSource;

    if (IsFatturazione()) {

        // Non uso la funzione select() della kendogrid per ottenere le righe in quanto questa restituisce solo quelle selezionate nella pagina corrente
        let docSelezionati = [];
        for (let riga of dsKGrid.data()) {
            if (riga.Selected === true) {
                docSelezionati.push(riga);
            }
        }

        if (docSelezionati.length > 0) {
            if (isDettaglio) {
                $("#btn_avvia_fatturazione_dettaglio").show();
            }
            else {
                $("#btn_avvia_fatturazione_testata").show();
            }
        }
        else {
            if (isDettaglio) {
                $("#btn_avvia_fatturazione_dettaglio").hide();
            }
            else {
                $("#btn_avvia_fatturazione_testata").hide();
            }
        }
    }

    if (hasGestioneWorkflow()) {

        if (isDettaglio) {
            // Seleziono o De-seleziono anche le altre righe di dettaglio appartenenti allo stesso documento
            for (let i = 0; i < dsKGrid.data().length; i++) {
                if (dsKGrid.at(i).Id_Mov_Det !== dataItem.Id_Mov_Det && dsKGrid.at(i).Id_Agenda === dataItem.Id_Agenda) {
                    dsKGrid.at(i).Selected = checked;
                }
            }

            // Imposto graficamente le altre righe selezionate nella pagina della griglia attualmente visualizzata
            let allRows = kGrid.tbody.children();
            for (let j = 0; j < allRows.length; j++) {
                let rowLoop = $(allRows[j]);
                let dataItemLoop = kGrid.dataItem(rowLoop);
                if (dataItemLoop.Id_Mov_Det !== dataItem.Id_Mov_Det && dataItemLoop.Id_Agenda === dataItem.Id_Agenda) {
                    rowLoop.find("input[type='checkbox'].checkbox-selectionRow").prop("checked", checked);
                    rowLoop.toggleClass(GIAS_K_STATE_SELECTED);
                }
            }
        }

        // Non uso la funzione select() della kendogrid per ottenere le righe in quanto questa restituisce solo quelle selezionate nella pagina corrente
        let docSelezionati = [];
        for (let riga of dsKGrid.data()) {
            if (riga.Selected === true) {
                docSelezionati.push(riga);
            }
        }

        if (docSelezionati.length > 0) {
            kGrid.element.find(".btngrid_passaggiostato").show();
            kGrid.element.find(".btngrid_ibcontrolli").show();
            kGrid.element.find(".btngrid_valorconf").show();
            
        }
        else {
            kGrid.element.find(".btngrid_passaggiostato").hide();
            kGrid.element.find(".btngrid_ibcontrolli").hide();
            kGrid.element.find(".btngrid_valorconf").hide();
        }
    }
}

function grid_Testata_Change(e) {
   
}

function onDataBoundingTestataDocumenti(e) {

    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");

    var wrapperRigheTestata = grid.wrapper;
    var headerRigheTestata = wrapperRigheTestata.find(".k-grid-header");

    function resizeFixedRigheTestata() {
        var wrapperWidth = wrapperRigheTestata.width();
        if (wrapperWidth !== 0) {
            var paddingRight = parseInt(headerRigheTestata.css("padding-right"));
            headerRigheTestata.css("width", wrapperWidth - paddingRight);
        }
        else {
            // Nel caso l'evento venga eseguito mentre la griglia è nascosta, imposto una variabile per ricalcolare correttamente la width
            // alla prima occorrenza dell'evento "scroll" in quanto più frequente
            headerRigheTestata.css("width", "auto");
            headerRigheTestata.data("fix_width", 1);
        }
    }

    function scrollFixedRigheTestata() {
        // Nel caso l'evento venga eseguito mentre la griglia è nascosta, rimuovo la classe
        var wrapperHeight = wrapperRigheTestata.outerHeight();
        if (headerRigheTestata.data("fix_width") === 1 && wrapperHeight !== 0) {
            var paddingRight = parseInt(headerRigheTestata.css("padding-right"));
            headerRigheTestata.css("width", wrapperRigheTestata.width() - paddingRight);
            headerRigheTestata.data("fix_width", 0);
        }

        var headerHeight = $('#headerDashboard').outerHeight();
        var headerGroupHeight = $('.k-grouping-header').outerHeight();
        var headerWidth = $('#headerDashboard').outerWidth();

        if (headerWidth < 997) {
           var offset = $(this).scrollTop(),
                tableOffsetTop = wrapperRigheTestata.offset().top + headerGroupHeight,
                tableOffsetBottom = tableOffsetTop + wrapperHeight - headerRigheTestata.height();
        } else {
            var offset = $(this).scrollTop(),
                tableOffsetTop = wrapperRigheTestata.offset().top + headerHeight + headerGroupHeight,
                tableOffsetBottom = tableOffsetTop + wrapperHeight - headerRigheTestata.height();
        }

        if (offset < tableOffsetTop || offset > tableOffsetBottom || wrapperHeight === 0) {
            headerRigheTestata.removeClass("fixed-header");
        } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !headerRigheTestata.hasClass("fixed")) {
            headerRigheTestata.addClass("fixed-header");
        }

    }

    resizeFixedRigheTestata();

    $(window).resize(resizeFixedRigheTestata);
    $(window).scroll(scrollFixedRigheTestata);

}

function onDataBoundingDettaglioDocumenti(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    for (var i = 3; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    }

    var wrapperRigheDettaglio = grid.wrapper;
    var headerRigheDettaglio = wrapperRigheDettaglio.find(".k-grid-header");

    function resizeFixedRigheDettaglio() {
        var wrapperWidth = wrapperRigheDettaglio.width();
        if (wrapperWidth !== 0) {
            var paddingRight = parseInt(headerRigheDettaglio.css("padding-right"));
            headerRigheDettaglio.css("width", wrapperWidth - paddingRight);
        }
        else {
            // Nel caso l'evento venga eseguito mentre la griglia è nascosta, imposto una variabile per ricalcolare correttamente la width
            // alla prima occorrenza dell'evento "scroll" in quanto più frequente
            headerRigheDettaglio.css("width", "auto");
            headerRigheDettaglio.data("fixed_width", 1);
        }
    }

    function scrollFixedRigheDettaglio() {
        // Nel caso l'evento venga eseguito mentre la griglia è nascosta, rimuovo la classe
        var wrapperHeight = wrapperRigheDettaglio.height();
        if (headerRigheDettaglio.data("fixed_width") === 1 && wrapperHeight !== 0) {
            var paddingRight = parseInt(headerRigheDettaglio.css("padding-right"));
            headerRigheDettaglio.css("width", wrapperRigheDettaglio.width() - paddingRight);
            headerRigheDettaglio.data("fixed_width", 0);
        }

        var offset = $(this).scrollTop(),
            tableOffsetTop = wrapperRigheDettaglio.offset().top,
            tableOffsetBottom = tableOffsetTop + wrapperHeight - headerRigheDettaglio.height();

        if (offset < tableOffsetTop || offset > tableOffsetBottom || wrapperHeight === 0) {
            headerRigheDettaglio.removeClass("fixed-header");
        } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !headerRigheDettaglio.hasClass("fixed")) {
            headerRigheDettaglio.addClass("fixed-header");
        }

    }

    resizeFixedRigheDettaglio();

    $(window).resize(resizeFixedRigheDettaglio);
    $(window).scroll(scrollFixedRigheDettaglio);

    //var data = grid.dataSource.data();
    //var rows = $.grep(data, function (d) {
    //    return d.Id_Mov_Det === ultimaRigaSelezionataGrigliaDettaglio.id;
    //}).map(function (d) {
    //    return grid.tbody.find("[data-uid=" + d.uid + "]");
    //});
    //grid.select(rows[0]);
}

function SpecieChange(e) {

    var filtro_specie = KendoMultisel("multiselSpecie").value().join(",");

    if (filtro_specie !== "")
        Elenco_Varieta = Leggi_Varieta(filtro_specie);
    else
        Elenco_Varieta = "";

    var multiselVarieta = KendoMultisel("multiselVarieta");
    multiselVarieta.autoBind = false;

    RiempiVarieta(multiselVarieta.dataSource);

}

function RiempiSpecie(options) {
    options.success(Elenco_Specie);
}

function RiempiVarieta(options) {
    var elencoVuoto = [{}];
    if (Elenco_Varieta !== "") {
        options.success(Elenco_Varieta);
    } else options.success(elencoVuoto);
}

function CampiReportPDF(e) {

    var index = this.current().index();

    if (buttonGroupSelected !== index)
    {
              
        buttonGroupSelected = index;

        // var tabStrip = $("#tabstrip_Filtri").kendoTabStrip().data("kendoTabStrip");
        
        // Testata
        if (index === 0) {
            $(".gridAreaDettaglio").hide();
            $(".gridAreaTestata").show();
            $("#tab1").click();
            $("#tab3").hide();
        }

        // Dettagli
        if (index === 1) {
            $(".gridAreaDettaglio").show();
            $(".gridAreaTestata").hide();
            $("#row_filtro_categorie").show();
            ImpostaDefaultFiltri();
            $("#tab3").show();

        }
        
    }

}

function multiselectGruppoDocumento_select(e)
{
    causaliDaAggiungere = KendoMultisel("multiselCausale").value();
    var nuoveCausali = e.dataItem.LAV_COD.split(";");
    

    for (var i = 0; i < nuoveCausali.length; i++) {
        var nuovaCausale = elencoCausali.find(obj => {
            return obj.LAV_COD === nuoveCausali[i];
        });

        if (nuovaCausale != undefined) {
            // se presente nella vecchia selezione la aggiungo
            causaliDaAggiungere.push(nuovaCausale.LAV_COD);
        }
    }
    KendoMultisel("multiselCausale").value(causaliDaAggiungere);
}

function multiselectGruppoDocumento_deselect(e)
{

    var causaliGiaPresenti = KendoMultisel("multiselCausale").value();
    var causaliDaRimuovere = e.dataItem.LAV_COD.split(";");
    var causaliDaTenere = [];

    for (var i = 0; i < causaliGiaPresenti.length; i++) {
        var daRimuovere = causaliDaRimuovere.find(obj => {
            return obj === causaliGiaPresenti[i];
        });

        if (daRimuovere == undefined)
            causaliDaTenere.push(causaliGiaPresenti[i]);
    }
    KendoMultisel("multiselCausale").value(causaliDaTenere);

}

function UtenteAbilitatoScrittura()
{
    var abilitato = $("input[name$='hf_UtenteAbilitatoScrittura']").val();
    return JSON.parse(abilitato.toLowerCase());
}

function ParametroType() {
    return $(cType).val();
}

function ParametroDocType() {
    return $(cDocType).val();
}

function IsFatturazione()
{
    return $(cModalitaFatturazione).val() === "True";
}

function ImpostaVisibilitaFiltriSwitch() {
    var show = false;
    var type = ParametroType();
    var doc_type = ParametroDocType();

    if ((type === DocContab_TipoRicerca_Vendite && doc_type === DocContab_TipoDoc_Ordine) || (type === DocContab_TipoRicerca_Acquisti && doc_type === DocContab_TipoDoc_Ordine)) {
        $("#rowGruppoDocumento").hide();
        $("#rowTipiDocumento").hide();
    }

    if (type === DocContab_TipoRicerca_Vendite && doc_type === DocContab_TipoDoc_Ordine)
        $("#div_chk_ordini_non_spediti").show();
    else
        $("#div_chk_ordini_non_spediti").hide();

    if (doc_type !== DocContab_TipoDoc_Ordine && doc_type !== DocContab_TipoDoc_ContrattoAffitto)
        $("#div_chk_ddt_non_fatturati").show();
    else
        $("#div_chk_ddt_non_fatturati").hide();

    //in ogni modo se sono in modalità ricerca per lancio fatturazione imposto opt_chk a true e nascondo
    if (IsFatturazione())
    {
        setKendoSwitch("cb_ddt_non_fatturati", true);
        $("#div_chk_ddt_non_fatturati").hide();
    }
}

function ImpostaDefaultFiltri()
{
    var type = ParametroType();
    if (type === DocContab_TipoRicerca_Conferimenti)
    {
        Set_MultiselValue("multiselCategorie", "210|310");
        $("#id_multiselCategorie").data("kendoMultiSelect").enable(false);
    }
}

function DammiIDControlloGriglia(dettaglio) {

    var type = ParametroType();
    var IDControllo = "";
    if (!dettaglio)
    {
        switch (type.toUpperCase()) {

            case DocContab_TipoRicerca_Acquisti:
                IDControllo = "tab_testata_griglia_report_acquisti";
                break;

            case DocContab_TipoRicerca_Vendite:
                IDControllo = "tab_testata_griglia_report_vendite";
                break;

            case DocContab_TipoRicerca_Conferimenti:
                IDControllo = "tab_testata_griglia_report_conferimenti";
                break;

            case DocContab_TipoRicerca_Contratti:
                IDControllo = "tab_testata_griglia_report_contratti";
                break;

        }
    }
    else
    {
        switch (type.toUpperCase()) {

            case DocContab_TipoRicerca_Acquisti:
                IDControllo = "tab_dettaglio_griglia_report_acquisti";
                break;

            case DocContab_TipoRicerca_Vendite:
                IDControllo = "tab_dettaglio_griglia_report_vendite";
                break;

            case DocContab_TipoRicerca_Conferimenti:
                IDControllo = "tab_dettaglio_griglia_report_conferimenti";
                break;

            case DocContab_TipoRicerca_Contratti:
                IDControllo = "tab_dettaglio_griglia_report_contratti";
                break;

        }

    }

    return IDControllo;
}

function Applica_Personalizzazioni_Griglie() {
    if (personalizzazioniGriglie != null && personalizzazioniGriglie != undefined && personalizzazioniGriglie != "") {

        for (var i = 0; i < personalizzazioniGriglie.length; i++) {

            var obj = personalizzazioniGriglie[i];
            var idControllo = obj.IdControllo;
            var personalizzazioni = obj.Personalizzazioni;
            var grid = $("#" + idControllo).data("kendoGrid");
            if (grid != null && grid != undefined && personalizzazioni != null && personalizzazioni != undefined && personalizzazioni != "") {
                setPersonalizzazioniGrigliaKendo(grid, JSON.parse(personalizzazioni));
                personalizzazioniGriglie[i].Personalizzazioni = null;
            }
        }
    }
}

function Inizializza_Combo_Centri_Aziendali() {

    $("#ddl_Centro_Aziendale").kendoDropDownList({
        dataTextField: "sa_nome",
        dataValueField: "sa_cod",
        dataSource: { transport: { read: function (options) { options.success(Elenco_Centri_Aziendali); } } },
        open: kendoDropDownAdjustWidth,
        optionLabel: {
            sa_nome: TraduzioneMultiResx(resxObj, "Tutti", "Tutti"),
            sa_cod: "-1"
        }
    }).data("kendoDropDownList");
     
}

function ApriCampionamento(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    if (dataItem.Elem_Cod === TRASFORMATI_VEGETALI) {

        let mesDocNonVisibileGruppiMerce = VerificaPermessoVisibilitaGruppiMerce(dataItem.Id_Agenda, dataItem.Lav_Cod);

        if (mesDocNonVisibileGruppiMerce !== "") {

            $("<div></div>").kendoAlert({
                title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
                content: mesDocNonVisibileGruppiMerce
            }).data("kendoAlert").open();

            return false;
        }

        var Id_Mov_Det = dataItem.Id_Mov_Det;

        // Prima di aprire popup con maschera campionamento
        // verifico se esistono dati per il campionamento
        var risp = Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(Id_Mov_Det);
        if (!risp)
            return;


        let url = UrlGestioneCampionamento(Id_Mov_Det);
        if (url === "")
            return;

        //Ho cambiato questa parte perchè così, quando richiamo questa funzione da RisultatoLiquidazione
        //prende l'indirizzo giusto per il mio webmethod in RisultatoLiquidazione.ascx.vb
        if ($("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== undefined &&
            $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== null &&
            $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== "") {
            window.parent.apriKendoWindowDocContabileRisultatoLiquidazioneUC(url, TraduzioneMultiResx(resxObj, "CampionamentoLiquidazione", "Campionamento Liquidazione"));
        }
        else {
            let id = "target_iframe";
            let dialog = $("#campionamentoWindow").data("kendoWindow");

            dialog.center().open();

            $("<form />",
                {
                    action: url,
                    method: "post",
                    target: id
                })
                .hide().appendTo("body")
                .submit().remove();
        }
    }

}

function is_FF_FormProdottoUC() {

    let w_is_FF_FormProdottoUC = false;
    if (modulo_anagrafe_log.includes(Modulo_FreshFood) ||
        modulo_anagrafe_log.includes(Modulo_Tabacco) ||
        modulo_anagrafe_log.includes(Modulo_Zoo)) {
        w_is_FF_FormProdottoUC = true;
    }

    return w_is_FF_FormProdottoUC;
}


function multiselProdotti_filtering(e) {
    var filter = e.filter;

    if (filter === undefined || !filter.value || filter.value.length < 3) {
        //prevent filtering if the filter does not value
        e.preventDefault();
    }
}

function schedula_Fatturazione()
{
    var IDControllo = "";
    var isDettaglio = false;
    if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 1) {
        IDControllo = DammiIDControlloGriglia(true);
        isDettaglio = true;
    } else if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 0) {
        IDControllo = DammiIDControlloGriglia(false);
    }
    var numeroSelezionati = 0;

    if (IDControllo != "") {

        var idsAgenda = [];
        var idsMovDet = [];
        let arrDocPerGruppiMerce = [];

        var grid = $("#" + IDControllo).data("kendoGrid");
        var currentData = grid.dataSource.data();
        for (var i = 0; i < currentData.length; i++) {
            if (currentData[i].Selected) {
                numeroSelezionati++;

                if (isDettaglio) {
                    idsMovDet.push(currentData[i].Id_Mov_Det);
                }
                else {
                    var tuttiGliId = "".concat(currentData[i].Mov_Non_Fatturati, ",", currentData[i].Mov_Fatturati_Parzialmente);
                    idsMovDet.push(...tuttiGliId.split(","));
                }
                if (!idsAgenda.includes(currentData[i].Id_Agenda))
                    idsAgenda.push(currentData[i].Id_Agenda);

                if (Array.isArray(setupGestioneGruppiMerce) && setupGestioneGruppiMerce.length > 0) {

                    if (arrDocPerGruppiMerce.find(element => element.Piva === currentData[i].PIVA && element.Id_Agenda === currentData[i].Id_Agenda) === undefined) {

                        arrDocPerGruppiMerce.push({
                            Piva: currentData[i].PIVA,
                            Id_Agenda: currentData[i].Id_Agenda,
                            Lav_Cod: currentData[i].Lav_Cod
                        });

                    }
                }

            }
        }

        var messaggio = "";

        if (numeroSelezionati === 0) {
            if (isDettaglio)
                messaggio = TraduzioneMultiResx(resxObj, "SelezionareAlmenoUnaRigaDocumento", "Selezionare almeno una riga Documento");
            else
                messaggio = TraduzioneMultiResx(resxObj, "SelezionareAlmenoUnDocumento", "Selezionare almeno un Documento");
        }
        else {
            if (arrDocPerGruppiMerce.length > 0) {
                messaggio = VerificaPermessoVisibilitaGruppiMerceMultiDocumento(arrDocPerGruppiMerce);
            }
        }

        if (messaggio !== "") {

            $("<div></div>").kendoAlert({
                title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
                content: messaggio
            }).data("kendoAlert").open();

            return false;
        }

        var ids = [];
        if (isDettaglio)
            ids = idsMovDet.filter(function (id) { return id != null; });
        else
            ids = idsMovDet.filter(function (id) { return id != ""; }).map(id => parseInt(id.trim()));

        popup_Schedola_Fatturazione(idsAgenda, ids);

    }

}

function popup_Schedola_Fatturazione(ids_agende, ids_mov_det) {

    let win_el = document.createElement("div");
    document.body.appendChild(win_el);
    let $win_el = $(win_el);

    let content = "<div style='margin-left:10px;margin-right:20px;'>";

    content += " <div style='padding-bottom: 10px'> "
    content += " <label class='lbl_required'>" + TraduzioneMultiResx(resxObj, "DataAttribuitaAFatture", "Data attribuita alle fatture che verranno create") + ":</label>";
    content += " <input id='txt_data_fatturazione' name='txt_data_fatturazione' style='width:100%;' MaxLength='10' />";
    content += "</div>";

    content += "<div style='margin-top: 10px; padding-bottom: 10px'>";
    content += " <label class='lbl_required' id='lbl_opt_soggetti_privati' for='opt_soggetti_privati'>" + TraduzioneMultiResx(resxObj, "IncludiSoggettiPrivati", "Includi soggetti privati") + "</label>";
    content += " <input type='checkbox' id='opt_soggetti_privati' name='opt_soggetti_privati' class='kendoSwitch'/>";
    content += "</div>";

    content += "<div style='padding-bottom: 10px'>";
    content += " <label class='lbl_required' id='lbl_opt_sezionali' for='opt_sezionali'>" + TraduzioneMultiResx(resxObj, "FatturazioneDistintaPerSezionali", "Fatturazione distinta per sezionali") + "</label>";
    content += " <input type='checkbox' id='opt_sezionali' name='opt_sezionali' class='kendoSwitch'/>";
    content += "</div>";

    content += "<div>";
    content += " <label class='lbl_required' id='lbl_opt_cessionari_diversi' for='opt_cessionari_diversi'>" + TraduzioneMultiResx(resxObj, "FatturazioneDistintaPerCessionari", "Fatturazione distinta per cessionari diversi") + "</label>";
    content += " <input type='checkbox' id='opt_cessionari_diversi' name='opt_cessionari_diversi' class='kendoSwitch'/>";
    content += "</div>";

    content += "</div>";

    $win_el.kendoDialog({
        title: TraduzioneMultiResx(resxObj, "SceltaParametriFatturazione", "Scelta Parametri Fatturazione"),
        closable: false,
        modal: true,
        visible: false,
        content: content,
        width: "40%",
        open: function () {

            $("#txt_data_fatturazione").kendoDatePicker({
                footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
                max: new Date(2100, 11, 31),
                value: new Date()
            });
            //$("#dataFatturazione").kendoCalendar({
            //    value: new Date(2012, 0, 1)
            //});
            creaKendoSwitch("opt_soggetti_privati", undefined, undefined, false);
            creaKendoSwitch("opt_sezionali", undefined, undefined, false);
            creaKendoSwitch("opt_cessionari_diversi", undefined, undefined, false);
        },
        actions: [
            {
                text: TraduzioneMultiResx(resxObj, "SchedulaFatturazione", 'Schedula Fatturazione'),
                cssClass: "",
                action: function (e) {

                    
                    var dataFatturazione = $('input[name$="txt_data_fatturazione"]').val();
                    if (dataFatturazione === "") 
                    {
                        MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "SpecificareLaDataFatturazione", "Specificare la Data Fatturazione"), "DIV_Messaggi");
                        return false;
                    }
                    if (!isValidDate(dataFatturazione))
                    {
                        MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "DataFatturazioneNonValida", "La Data Fatturazione immessa non è una data valida"), "DIV_Messaggi");
                        return false;
                    }

                    var parametriRottura = new Object();
                    parametriRottura.soggetti_privati = getKendoSwitch("opt_soggetti_privati");
                    parametriRottura.sezionali = getKendoSwitch("opt_sezionali");
                    parametriRottura.cessionari_diversi = getKendoSwitch("opt_cessionari_diversi");
                    Schedula_Fatturazione(ids_agende, ids_mov_det, parametriRottura, dataFatturazione);
                    return true;
                }
            },
            {
                text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla")
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}


function RichiamaPassaggioStato() {

    if (utente_conflittoPermessiWorkflow !== "") {
        kendo.alert(utente_conflittoPermessiWorkflow);
        return;
    }

    let isDettaglio = false;
    if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 1) {
        isDettaglio = true;
    } else if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 0) {
        isDettaglio = false;
    }

    let kGrid = KendoGrid(DammiIDControlloGriglia(isDettaglio));
    let dsKGrid = kGrid.dataSource;
    
    let docSelezionati = [];

    // Non uso la funzione select() della kendogrid per ottenere le righe in quanto questa restituisce solo quelle selezionate nella pagina corrente
    for (let riga of dsKGrid.data()) {
        if (riga.Selected === true) {
            docSelezionati.push(riga);
        }
    }

    let urlPassaggioStato = "";
    if (docSelezionati.length > 0) {

        let setPraticaCod = new Set(); // Il set rappresenta una lista univoca, utile in caso di selezione con la griglia dei dettagli
        let praticheConStatiDiversi = false;
        let docBloccato = false;

        let arrDocPerGruppiMerce = [];

        for (let i = 0; i < docSelezionati.length; i++) {

            if (docSelezionati[i].Blocco_Flag === 1) {
                docBloccato = true;
                break;
            }

            if (i > 0) {
                if (docSelezionati[i].Pratica_Stato_Cod !== docSelezionati[i - 1].Pratica_Stato_Cod) {
                    praticheConStatiDiversi = true;
                    break;
                }
            }

            setPraticaCod.add(docSelezionati[i].Pratica_Cod);

            // Se la gestione dei gruppi merce è attiva, creo un array apposito per effettuare di seguito i controlli di visibilità sui documenti
            if (Array.isArray(setupGestioneGruppiMerce) && setupGestioneGruppiMerce.length > 0) {

                if (arrDocPerGruppiMerce.find(element => element.Piva === docSelezionati[i].PIVA && element.Id_Agenda === docSelezionati[i].Id_Agenda) === undefined) {
                    // Aggiungo ogni agenda una sola volta, utile in caso di dettaglio
                    arrDocPerGruppiMerce.push({
                        Piva: docSelezionati[i].PIVA,
                        Id_Agenda: docSelezionati[i].Id_Agenda,
                        Lav_Cod: docSelezionati[i].Lav_Cod
                    });
                }

            }

        }

        let mesPermessoGruppiMerce = "";

        if (arrDocPerGruppiMerce.length > 0 && docBloccato === false && praticheConStatiDiversi === false) {
            mesPermessoGruppiMerce = VerificaPermessoVisibilitaGruppiMerceMultiDocumento(arrDocPerGruppiMerce);
        }

        if (mesPermessoGruppiMerce === "") {

            let arrPraticaCod = Array.from(setPraticaCod);

            if (docBloccato === true) {
                kendo.alert(TraduzioneMultiResx(resxObj, "CambioDiStatoImpossibileDocBloccato",
                    "Non è possibile cambiare lo stato dei documenti selezionati perché almeno uno di questi è bloccato"));
            }
            else {
                if (praticheConStatiDiversi === false) {
                    // Se i documenti selezionati hanno lo stesso stato,
                    if (arrPraticaCod.every(pratCod => pratCod === 0)) {
                        // ma i loro codici pratica sono sempre 0, allora mando un messaggio all'utente;
                        kendo.alert(TraduzioneMultiResx(resxObj, "CambioDiStatoImpossibileDocSenzaPratica",
                            "Non è possibile effettuare un passaggio di stato sui documenti selezionati perché non hanno pratiche a loro collegate"));
                    }
                    else {
                        // altrimenti proseguo con la creazione del link
                        urlPassaggioStato = LinkPassaggioDiStato(arrPraticaCod);
                    }
                }
                else {
                    // Mando messaggio all'utente che non può effettuare il passaggio di stato perché ha selezionato documenti con stato corrente diverso
                    kendo.alert(TraduzioneMultiResx(resxObj, "CambioDiStatoImpossibileDiversiStatiIniziali",
                        "Non è possibile cambiare lo stato dei documenti selezionati contemporaneamente perché questi sono in stati diversi fra loro"));
                }
            }
        }
        else {
            kendo.alert(mesPermessoGruppiMerce);
        }
        
    }

    if (urlPassaggioStato !== "") {

        window.removeEventListener('message', EvMessageWindowGestionePassaggioDiStato);
        window.addEventListener('message', EvMessageWindowGestionePassaggioDiStato);

        $(document.body).append('<div id="GestionePassaggioDiStatoWindow"></div>');
        $('#GestionePassaggioDiStatoWindow').kendoWindow({
            title: TraduzioneMultiResx(resxObj, "GestionePassaggioDiStato", "Gestione Passaggio Di Stato"),
            modal: true,
            resizable: false,
            iframe: true,
            width: "60%",
            height: "48%",
            content: urlPassaggioStato,
            close: function () {
                setTimeout(function () {
                    $('#GestionePassaggioDiStatoWindow').kendoWindow('destroy');
                    //$("#btn_CercaPratiche").trigger("click");
                }, 200);
            }
        }).data('kendoWindow').center();
    }

}

function EvMessageWindowGestionePassaggioDiStato(event) {
    // Funzione utilizzata dalla pagina chiamata, non cancellare
    var kWinPassaggioStato = $('#GestionePassaggioDiStatoWindow').data("kendoWindow");
    var urlWinPassaggio = kWinPassaggioStato.options.content.url;

    if (verificaOriginSecondaria(window, urlWinPassaggio, event) && (typeof event.data == "string") && event.data.includes("RispostaStringa")) {
        let respMsg = JSON.parse(event.data);
        switch (respMsg.Tipo) {
            case 'Profilazione_PassaggioStato':
                if (respMsg.RispostaConferma === true) {
                    MessaggioTuttoOK_Bootstrap(respMsg.RispostaStringa, "DIV_Messaggi");
                    $('#GestionePassaggioDiStatoWindow').kendoWindow('destroy');
                    Esegui_Report(false);
                }
                else {
                    MessaggioErrore_Bootstrap(respMsg.RispostaStringa, "DIV_Messaggi");
                }
                break;
            default:
                console.log("Evento non gestito");
        }
    }
}

function IB_Controlli_PreInvio() {

    let isDettaglio = false;
    if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 1) {
        isDettaglio = true;
    } else if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 0) {
        isDettaglio = false;
    }

    let kGrid = KendoGrid(DammiIDControlloGriglia(isDettaglio));
    let dsKGrid = kGrid.dataSource;

    let docSelezionati = [];

    // Non uso la funzione select() della kendogrid per ottenere le righe in quanto questa restituisce solo quelle selezionate nella pagina corrente
    for (let riga of dsKGrid.data()) {
        if (riga.Selected === true) {
            docSelezionati.push(riga);
        }
    }

    if (docSelezionati.length === 0) {
        return;
    }

    let setIdAgenda = new Set(); // Il set rappresenta una lista univoca, utile in caso di selezione con la griglia dei dettagli

    for (let i = 0; i < docSelezionati.length; i++) {

        setIdAgenda.add(docSelezionati[i].Id_Agenda);

    }

    let dsRisControlli = LeggiLogInvioDettaglio(Array.from(setIdAgenda));

    if (dsRisControlli === null || dsRisControlli === undefined) {
        return;
    }

    if (dsRisControlli.length === 0) {
        $("<div></div>").kendoAlert({
            content: "I documenti selezionati possono essere esportati senza errori",
            title: "Risultato controlli"
        }).data("kendoAlert").open();
    }
    else {
        MostraRisultatiControlliInvio(dsRisControlli);
    }
}

function MostraRisultatiControlliInvio(dsRisControlli) {

    $("#hf_DS_RisControlli").val(JSON.stringify(dsRisControlli));

    let kGridControlliInvio;
    let kWinControlliInvio = $("#windowControlliInvio").getKendoWindow();
    let kGridTitle = "Risultato controlli";

    if (kWinControlliInvio === undefined) {
        kWinControlliInvio = $("#windowControlliInvio").kendoWindow({
            width: "80%",
            minWidth: "960",
            height: "80%",
            minHeight: "460",
            modal: true,
            actions: ["Maximize", "Close"],
            title: kGridTitle
        }).getKendoWindow();

        // Creazione Griglia
        let funzioniCRUD = {
            funzioneRead: kGridControlliInvioRead,
            UtenteAbilitatoInserimentoModifica: false,
            UtenteAbilitatoCancellazione: false,
            omettiPulsantiSalva: true,
            omettiPulsantiAnnulla: true
        };
        var idModel = "Id_Agenda";
        var campiKendoModel = {
            Data_Operazione: { type: "date" },
        };
        var colonneKendoGrid = [
            { field: "Descrizione_Operazione", title: "Desc Operazione", filterable: { multi: true, search: true }, width: "320px" },
            { field: "Data_Operazione", title: TraduzioneMultiResx(resxObj, "DataDocumentoAbbr", "Data Doc."), format: "{0:dd/MM/yyyy}", width: "100px" },
            { field: "Stato", title: "Stato Doc.", filterable: { multi: true, search: true }, width: "130px" },
            { field: "Mov_Det_Des", title: "Desc Prodotto", filterable: { multi: true, search: true }, width: "270px" },
            { field: "Lotto", title: "Lotto", filterable: { multi: true, search: true }, width: "150px" },
            { field: "Messaggio", title: "Messaggio", filterable: { multi: true, search: true }, width: "250px", encoded: false },
        ];
        //{ field: "Validita_CDC_WBS", title: "Validita_CDC_WBS", filterable: { multi: true, search: true }, width: "250px" },
        //{ field: "Validita_Collegamento_Ordine", title: "Validita_Collegamento_Ordine", filterable: { multi: true, search: true }, width: "250px" },
        //{ field: "Validita_Gruppo_Merce", title: "Validita_Gruppo_Merce", filterable: { multi: true, search: true }, width: "250px" },
        //{ field: "Validita_Indirizzo", title: "Validita_Indirizzo", filterable: { multi: true, search: true }, width: "250px" },
        var parametriPerLettura = [];
        var parametriDataSource = { pagesize: 30 };
        var parametriKendoGrid = {
            editable: false,
            salvaRipristinaPersonalizzazioni: false,
            excel: true,
            pdf: false,
            groupable: false,
            pageable: { pageSizes: [10, 30, 50, 100, "all"], buttonCount: 3 }
        };
        var funzioniPrimaDopoEventi = {};

        kGridControlliInvio = creaKendoGrid(
            "gridControlliInvio", // rappresenta l'ID del div a cui si associa la griglia
            funzioniCRUD, //funzioni js da chiamare per read, insert, update, delete
            idModel, // chiave riga 
            campiKendoModel, // campi modello
            colonneKendoGrid, // colonne da mostrare
            parametriPerLettura, // parametri da passare alla lettura
            parametriDataSource, // parametri data source { chiave - valore}
            parametriKendoGrid, // parametri griglia [{ chiave - valore}]
            funzioniPrimaDopoEventi // funzioni da chiamare all'inizio e alla fine dei vari eventi
        ).data("kendoGrid");
    }
    else {
        // In questo caso non ricreo la griglia ma ne aggiorno i dati
        kGridControlliInvio = KendoGrid("gridControlliInvio");
        kGridControlliInvio.dataSource.read();
        kGridControlliInvio.dataSource.filter([]);
    }

    kWinControlliInvio.open();
    kWinControlliInvio.center();
}

function kGridControlliInvioRead(options) {
    let dt = JSON.parse($("#hf_DS_RisControlli").val());
    options.success(dt);
}





function ValorizzazioneConferimentiConferma() {
    $("#confermaValorizzazioneConferimentiDialog").kendoDialog({
        width: "400px",
        title: TraduzioneMultiResx(resxObj, "ValorizzazioneConferimenti", "Valorizzazione Conferimenti"),
        closable: false,
        modal: true,
        visible: false,
        content: "<p>" + TraduzioneMultiResx(resxObj, "ValorizzazioneConferimenti", "LA PROCEDURA DI VALORIZZAZIONE DEI CONFERIMENTI COMPORTA LA RISCRITTURA E QUINDI LA PERDITA DEI PREZZI PRECEDENTEMENTE SALVATI. CONTINUARE?") + "<p>",
        actions: [
            { text: TraduzioneMultiResx(resxObj, "Conferma", "Conferma"), action: function (e) { ValorizzazioneConferimenti(); } },
            { text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla"), primary: true }
        ]
    });
    $("#confermaValorizzazioneConferimentiDialog").data("kendoDialog").open();

}