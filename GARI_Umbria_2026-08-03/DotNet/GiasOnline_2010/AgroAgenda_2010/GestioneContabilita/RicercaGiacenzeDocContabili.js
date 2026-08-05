var utenteAbilitatoInserimentoModifica = $("input[name$='hf_UtenteAbilitatoScrittura']").val();
var utenteAbilitatoCancellazione = $("input[name$='hf_UtenteAbilitatoScrittura']").val();
var utenteAbilitatoPomodoro = $("input[name$='hf_UtenteAbilitatoPomodoro']").val();

function popolaGrigliaReportDettaglio(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: RicercaReportDettaglio,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null,
        checkBoxFunction: kEventoSelezionaRiga,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True"
    };
    var idModel = "Id_Mov_Det";

    //var campiKendoModel = getModelGrigliaReportDettaglio();
    var campiKendoModel = {
        Blocco_Flag: { type: "boolean" },
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
        Importo: { type: "number" },
        Provvigione: { type: "number" },
        Provvigione_Calcolata: { type: "number" },
        Veg_Des: { type: "string" },
        Cul_Des: { type: "string" },
        chiave_giacenze: { type: "string" },
        inGiacenza: { type: "string" }
        //Qta_Utilizzata: { type: "number" },
    };

    for (let i in Elenco_Parametri_Qualitativi) {
        campiKendoModel[Elenco_Parametri_Qualitativi[i].campo] = { type: "string" };
    }

    var colonneKendoGrid = colonneKendoGridDettaglio(IDControllo);
    for (let i in Elenco_Parametri_Qualitativi) {
        colonneKendoGrid.push({ field: Elenco_Parametri_Qualitativi[i].campo, title: Elenco_Parametri_Qualitativi[i].nome, hidden: true, width: 100 });
    }

    //var type = ParametroType();
    //if (utenteAbilitatoInserimentoModifica === "True") {
    //    if (type !== "C") {
    //        colonneKendoGrid.unshift({
    //            command: [
    //                { template: "<span class='fa fa-print fa-2x print_elem' style='cursor: pointer;' title='Stampa Documento' onclick=StampaDocumento(this.closest('tr'),this.closest('.k-grid'))></span>" },
    //                {
    //                    template: "<span class='fa fa-pencil-square-o fa-2x edit_elem' title='Modifica Documento' onclick=ApriModificaDocumento(this.closest('tr'),this.closest('.k-grid'),2)></span>",
    //                    visible: function (dataItem) { return dataItem.Blocco_Flag === false; }
    //                },
    //                {
    //                    template: "<span class='fa fa-trash-o fa-2x del_elem' title='Elimina Documento' onclick=EliminaDocumento(this.closest('tr'),this.closest('.k-grid'))></span>",
    //                    visible: function (dataItem) { return dataItem.Blocco_Flag === false; }
    //                },
    //                { template: "<span class='fa fa-info fa-2x info_elem' title='Visualizza Documento' onclick=ApriModificaDocumento(this.closest('tr'),this.closest('.k-grid'),0)></span>" }
    //            ], title: "Azioni", width: "130px"
    //        });
    //    }
    //    else {
    //        colonneKendoGrid.unshift({
    //            command: [
    //                { template: "<span class='fa fa-print fa-2x print_elem' style='cursor: pointer;' title='Stampa Documento' onclick=StampaDocumento(this.closest('tr'),this.closest('.k-grid'))></span> " },
    //                { template: "<span class='fa fa-barcode fa-2x print_elem' style='cursor: pointer;' title='Stampa Etichette' onclick=StampaBarCode(this.closest('tr'),this.closest('.k-grid'),true)></span>" },
    //                {
    //                    template: "<span class='fa fa-pencil-square-o fa-2x edit_elem' title='Modifica Documento' onclick=ApriModificaDocumento(this.closest('tr'),this.closest('.k-grid'),2)></span> ",
    //                    visible: function (dataItem) { return dataItem.Blocco_Flag === false && (dataItem.Tipo_Accettazione != -1 || utenteAbilitatoPomodoro); }
    //                },
    //                {
    //                    template: "<span class='fa fa-trash-o fa-2x del_elem' title='Elimina Documento' onclick=EliminaDocumento(this.closest('tr'),this.closest('.k-grid'))></span>",
    //                    visible: function (dataItem) { return dataItem.Blocco_Flag === false && (dataItem.Tipo_Accettazione != -1 || utenteAbilitatoPomodoro); }
    //                },
    //                { template: "<span class='fa fa-info fa-2x info_elem' title='Visualizza Documento' onclick=ApriModificaDocumento(this.closest('tr'),this.closest('.k-grid'),0)></span>" },
    //                { template: "<span class='fa fa-check-square-o fa-2x' style='cursor: pointer;' title='Campionamento' onclick=ApriCampionamento(this.closest('tr'),this.closest('.k-grid'))></span>" }

    //            ], title: "Azioni", width: "210px"

    //        });
    //    }
    //} else {
    //    if (type !== "C") {
    //        colonneKendoGrid.unshift({
    //            command: [{
    //                template: "<span class='fa fa-info fa-2x info_elem' title='Visualizza Documento' onclick=ApriModificaDocumento(this.closest('tr'),this.closest('.k-grid'),0)></span>" +
    //                        "<span class='fa fa-print fa-2x print_elem' style='cursor: pointer;' title='Stampa Documento' onclick=StampaDocumento(this.closest('tr'),this.closest('.k-grid'))></span>"
    //            }], title: "Azioni", width: "130px"
    //        });
    //    } else {
    //        colonneKendoGrid.unshift({
    //            command: [
    //                { template: "<span class='fa fa-info fa-2x info_elem' title='Visualizza Documento' onclick=ApriModificaDocumento(this.closest('tr'),this.closest('.k-grid'),0)></span>" },
    //                { template: "<span class='fa fa-print fa-2x print_elem' style='cursor: pointer;' title='Stampa Documento' onclick=stampaPC(this.closest('tr'),this.closest('.k-grid'))></span>" },
    //                {
    //                    template: "<span class='fa fa-barcode fa-2x print_elem' style='cursor: pointer;' title='Stampa Etichette' onclick=StampaBarCode(this.closest('tr'),this.closest('.k-grid'),true)></span>",
    //                    visible: function (dataItem) {
    //                        return (dataItem.Lav_Cod != undefined && dataItem.Lav_Cod == 1078);
    //                    }
    //                }
    //            ], title: "Azioni", width: "165px"
    //        });
    //    }
    //}

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
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        toolbarCommands: ["templateBtnConfermaRigheScelte"],
        reorderable: true,
        excel: true,
        pdf: false
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundingDettaglioDocumenti, funzioneDaChiamarePrimaDiExcelExport: onExportExcel };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

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

function colonneKendoGridDettaglio(IDControllo) {
    var colonneKendoGrid = [];
    var doc_type = ParametroDocType();
    switch (IDControllo) {
        case "tab_dettaglio_griglia_report_vendite":
            colonneKendoGrid = [
                { field: "Des_Lib", title: "Descrizione", width: 400 },
                { field: "Soggetto_RagioneSociale", title: "Cliente", filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Rapporto", title: "Tipo Rap. Contab.", filterable: { multi: true, search: true } },
                { field: "Data_Movimento", title: "Data Doc.", format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Data Doc.: #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Tipo_Documento", title: "Tipo Doc.", filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: "Nr. Doc.", attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Riga", title: "Riga", attributes: { style: "text-align:center;" }, width: 100 },
                { field: "Referenza_Descr", title: "Prodotto", filterable: { multi: true, search: true }, width: 200 },
                { field: "Unita_Misura_Sigla", title: "UdM", attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Qta", title: "Quantità", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Kg_Netti", title: "Quantità Totale", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 150 },
                { field: "Prezzo_Netto", title: "Prezzo Netto", format: "{0:n6}", attributes: { style: "text-align:right;" } },
                { field: "Imponibile_Netto", title: "Imponibile Netto", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Iva", title: "Iva", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Importo", title: "Importo", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Sa_Nome", title: "Centro Aziendale", filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: "Cod. Cliente", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Stato", title: "Stato", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione", title: "Regione", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia", title: "Provincia", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune", title: "Comune", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Destinazione", title: "Destinazione", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Stato_Dest", title: "Stato destinaz.", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione_Dest", title: "Regione destinaz.", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia_Dest", title: "Provincia destinaz.", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune_Dest", title: "Comune destinaz.", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Vettore_RagioneSociale", title: "Vettore", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Agente_Codice", title: "Cod. Agente", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Agente_RagioneSociale", title: "Agente", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Capoarea_Codice", title: "Cod. Capo Area", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Capoarea_RagioneSociale", title: "Capo Area", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Anno_Movimento", title: "Anno", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: "Anno-Mese", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: "Causale", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Note", title: "Note", hidden: true, width: 150 },
                { field: "Referenza_Codice", title: "Codice Prodotto", hidden: true, width: 150 },
                { field: "Categoria_Prodotto", title: "Categoria Prodotto", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Categoria_Commerciale", title: "Categoria Commerciale", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Note_Prodotto", title: "Note Prodotto", hidden: true, width: 150 },
                { field: "Lotto", title: "Lotto", filterable: { multi: true, search: true }, hidden: false, width: 150 },
                { field: "Unita_Misura_Secondaria_Sigla", title: "UdM Sec.", attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true }, hidden: true, width: 100 },
                { field: "Qta_Extra", title: "Litri/Kg", format: "{0:n2}", attributes: { style: "text-align:right;" }, hidden: true, width: 100 },
                { field: "Tara_Totale", title: "Tara", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Kg_Lordi", title: "Kg Lordi", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Nr_Imballi", title: "Nr. Imballi", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Nr_Contenitori", title: "Nr. Contenitori", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                // { field: "Nr_Confezioni", title: "Nr. Confezioni", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Prezzo_Riferito_A", title: "Prezzo Rif. A", filterable: { multi: true, search: true }, hidden: true, width: 100 },
                { field: "Imponibile", title: "Imponibile", format: "{0:n2}", hidden: true, width: 100 },
                { field: "Sconto_Perc", title: "Sconto %", format: "{0:n2}", hidden: true, width: 100 },
                { field: "Sconto", title: "Sconto", format: "{0:n2}", hidden: true, width: 100 },
                { field: "Provvigione", title: "Provvigione %", format: "{0:n2}", attributes: { style: "text-align:right;" }, hidden: true, width: 100 },
                { field: "Provvigione_Calcolata", title: "Provvigione", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Scadenza", title: "Scadenza", template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: 'Scadenza: #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 }
            ];
            if (doc_type === "O")
            {
                colonneKendoGrid.push({ field: "Qta_Evasa", title: "Quantità  Evasa", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" });
                colonneKendoGrid.push({ field: "Qta_Residua", title: "Quantità  Residua", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" });
                colonneKendoGrid.push({ field: "Qta_Netta_Evasa", title: "Quantità  Totale Evasa", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Netta_Residua", title: "Quantità  Totale Residua", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Tara_Evasa", title: "Tara Evasa", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Tara_Residua", title: "Tara Residua", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Lorda_Evasa", title: "Kg Lordi Evasi", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Lorda_Residua", title: "Kg Lordi Residui", attributes: { style: "text-align:right;" }, format: "{0:n2}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Imballi_Evasa", title: "Nr. Imballi Evasi", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Imballi_Residua", title: "Nr. Imballi Residui", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Contenitori_Evasa", title: "Nr. Contenitori Evasi", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
                colonneKendoGrid.push({ field: "Qta_Contenitori_Residua", title: "Nr. Contenitori Residui", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 });
            }
            break;

        case "tab_dettaglio_griglia_report_acquisti":
            colonneKendoGrid = [
                { field: "Des_Lib", title: "Descrizione", width: 400 },
                { field: "Soggetto_RagioneSociale", title: "Fornitore", filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Rapporto", title: "Tipo Rap. Contab.", filterable: { multi: true, search: true } },
                { field: "Data_Movimento", title: "Data Doc.", format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Data Doc.: #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Tipo_Documento", title: "Tipo Doc.", filterable: { multi: true, search: true }, width: 180 },
                { field: "Numero_Movimento", title: "Nr. Doc.", attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Data_Movimento_DDT", title: "Data DDT", format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Data DDT: #= kendo.toString(value,'dd/MM/yyyy') #", width: 100 },
                { field: "Numero_Movimento_DDT", title: "Nr. DDT", attributes: { style: "text-align:center;" }, width: 150 },
                { field: "Riga", title: "Riga", attributes: { style: "text-align:center;" }, width: 100 },
                { field: "Referenza_Descr", title: "Prodotto", filterable: { multi: true, search: true }, width: 200 },
                { field: "Veg_Cod", title: "Cod. Specie", attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Veg_Des", title: "Specie", attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Cul_Cod", title: "Cod. Varietà", attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Cul_Des", title: "Varietà", attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Unita_Misura_Sigla", title: "UdM", attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
                { field: "Qta", title: "Quantità", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Kg_Netti", title: "Quantità Totale", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 150 },
                { field: "Prezzo_Netto", title: "Prezzo Netto", format: "{0:n6}", attributes: { style: "text-align:right;" } },
                { field: "Imponibile_Netto", title: "Imponibile Netto", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Iva", title: "Iva", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Importo", title: "Importo", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Sa_Nome", title: "Centro Aziendale", filterable: { multi: true, search: true }, width: 300 },
                { field: "Soggetto_Codice", title: "Cod. Cliente", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Stato", title: "Stato", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione", title: "Regione", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia", title: "Provincia", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune", title: "Comune", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Destinazione", title: "Destinazione", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Stato_Dest", title: "Stato destinaz.", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Regione_Dest", title: "Regione destinaz.", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Provincia_Dest", title: "Provincia destinaz.", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Comune_Dest", title: "Comune destinaz.", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Vettore_RagioneSociale", title: "Vettore", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Agente_Codice", title: "Cod. Agente", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Agente_RagioneSociale", title: "Agente", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Capoarea_Codice", title: "Cod. Capo Area", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Capoarea_RagioneSociale", title: "Capo Area", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Anno_Movimento", title: "Anno", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Mese_Movimento", title: "Anno-Mese", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Causale_Trasporto", title: "Causale", filterable: { multi: true, search: true }, hidden: true, width: 200 },
                { field: "Note", title: "Note", hidden: true, width: 150 },
                { field: "Referenza_Codice", title: "Codice Prodotto", hidden: true, width: 150 },
                { field: "Categoria_Prodotto", title: "Categoria Prodotto", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Categoria_Commerciale", title: "Categoria Commerciale", filterable: { multi: true, search: true }, hidden: true, width: 150 },
                { field: "Note_Prodotto", title: "Note Prodotto", hidden: true, width: 150 },
                { field: "Lotto", title: "Lotto", filterable: { multi: true, search: true }, hidden: false, width: 150 },
                { field: "Unita_Misura_Secondaria_Sigla", title: "UdM Sec.", attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true }, hidden: true, width: 100 },
                { field: "Qta_Extra", title: "Litri/Kg", format: "{0:n2}", attributes: { style: "text-align:right;" }, hidden: true, width: 100 },
                { field: "Tara_Totale", title: "Tara", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Kg_Lordi", title: "Kg Lordi", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Nr_Imballi", title: "Nr. Imballi", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Nr_Contenitori", title: "Nr. Contenitori", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                // { field: "Nr_Confezioni", title: "Nr. Confezioni", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
                { field: "Prezzo_Riferito_A", title: "Prezzo Rif. A", filterable: { multi: true, search: true }, hidden: true, width: 100 },
                { field: "Imponibile", title: "Imponibile", format: "{0:n2}", hidden: true, width: 100 },
                { field: "Sconto_Perc", title: "Sconto %", format: "{0:n2}", hidden: true, width: 100 },
                { field: "Sconto", title: "Sconto", format: "{0:n2}", hidden: true, width: 100 },
                { field: "Provvigione", title: "Provvigione %", format: "{0:n2}", attributes: { style: "text-align:right;" }, hidden: true, width: 100 },
                { field: "Provvigione_Calcolata", title: "Provvigione", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
                { field: "Scadenza", title: "Scadenza", template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: 'Scadenza: #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 },
                //{ field: "Qta_Utilizzata", title: "Quantità Utilizzata", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Qta_Residua", title: "Quantità Residua", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
                { field: "Qta_Evasa", title: "Quantità Evasa", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
            ];
            break;
    }
    return colonneKendoGrid;

}


function Esegui_Report() {
    trovatoErrore = false;
    dataDaControllare = $('input[name$="Txt_DataRegDal"]').val();
    var dataValida = true;
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
        // Griglia Dettaglio
        $(".gridAreaDettaglio").show();
        IDControllo = DammiIDControlloGriglia();
        popolaGrigliaReportDettaglio(IDControllo);
        //Applica_Personalizzazioni_Griglia();
    }
}

function kEventoSelezionaRiga(e) {
    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#" + DammiIDControlloGriglia()).data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
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
        var paddingRight = parseInt(headerRigheDettaglio.css("padding-right"));
        headerRigheDettaglio.css("width", wrapperRigheDettaglio.width() - paddingRight);
    }

    function scrollFixedRigheDettaglio() {

        var offset = $(this).scrollTop(),
            tableOffsetTop = wrapperRigheDettaglio.offset().top,
            tableOffsetBottom = tableOffsetTop + wrapperRigheDettaglio.height() - headerRigheDettaglio.height();

        if (offset < tableOffsetTop || offset > tableOffsetBottom) {
            headerRigheDettaglio.removeClass("fixed-header");
        } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !headerRigheDettaglio.hasClass("fixed")) {
            headerRigheDettaglio.addClass("fixed-header");
        }

    }

    resizeFixedRigheDettaglio();

    $(window).resize(resizeFixedRigheDettaglio);
    $(window).scroll(scrollFixedRigheDettaglio);

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
function StampaBarCode(tr_elem, grid_elem, dettaglio) {
    Salva_Filtri();

    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    stampa_barCode(dataItem, dettaglio);

}

// Click modifica documento
function ApriModificaDocumento(tr_elem, grid_elem, operazione) {

    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    var Id_Agenda = dataItem.Id_Agenda;
    var Lav_Cod = dataItem.Lav_Cod;
    var Blocco_Flag = dataItem.Blocco_Flag;
    var Data_Movimento = dataItem.Data_Movimento;

    if (Blocco_Flag === true && operazione === 2) {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: "Attenzione",
            messages: { okText: "Sì", cancel: "No" },
            content: "Il documento non è modificabile. Aprirlo in sola consultazione ?"
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () {
            aprimodifica_documento(Id_Agenda, Lav_Cod, 0, Data_Movimento);
        });

        kendoConfirm.open();
    }
    else {
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
        title: "Gestione Cancellazione Documenti",
        closable: false,
        modal: true,
        visible: false,
        content: "<p>Eliminare definitivamente questo documento?<p>",
        actions: [
            { text: "Conferma", action: function (e) { elimina_documento(Id_Agenda, Lav_Cod, false, Data_Movimento); } },
            { text: "Annulla", primary: true }
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
            title: "Gestione Sblocco Documenti",
            closable: false,
            modal: true,
            visible: false,
            content: "<p>Sbloccare questo documento?<p>",
            actions: [
                { text: "Conferma", action: function (e) { sblocca_documento(piva, saCod, idAgenda); } },
                { text: "Annulla", primary: true }
            ]
        });
        $("#confermaSbloccoDialog").data("kendoDialog").open();
    } else {
        MessaggioErrore_Bootstrap("L'utente non dispone dei permessi per sbloccare un documento contabile.", "DIV_Messaggi");
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

function ImpostaVisibilitaFiltriSwitch() {
    var show = false;
    var type = ParametroType();
    var doc_type = ParametroDocType();

    if ((type === "V" && doc_type === "O") || (type === "A" && doc_type === "O")) {
        $("#rowGruppoDocumento").hide();
        $("#rowTipiDocumento").hide();
    }


    if (doc_type === "O" && type === "V")
        $("#div_chk_ordini_non_spediti").show();
    else
        $("#div_chk_ordini_non_spediti").hide();

    if (doc_type !== "O")
        $("#div_chk_ddt_non_fatturati").show();
    else
        $("#div_chk_ddt_non_fatturati").hide();

}

function DammiIDControlloGriglia() {

    var type = ParametroType();
    var IDControllo = "";

    switch (type.toUpperCase()) {
        case "A":
            IDControllo = "tab_dettaglio_griglia_report_acquisti";
            break;
        case "V":
            IDControllo = "tab_dettaglio_griglia_report_vendite";
            break;
        case "C":
            IDControllo = "tab_dettaglio_griglia_report_conferimenti";
            break;
    }

    return IDControllo;
}

function Applica_Personalizzazioni_Griglia() {
    if (personalizzazioniGriglia != null && personalizzazioniGriglia != undefined && personalizzazioniGriglia != "") {           

        var idControllo = personalizzazioniGriglia.IdControllo;
        var personalizzazioni = personalizzazioniGriglia.Personalizzazioni;
        var grid = $("#" + idControllo).data("kendoGrid");
        if (grid != null && grid != undefined && personalizzazioni != null && personalizzazioni != undefined && personalizzazioni != "") {
            setPersonalizzazioniGrigliaKendo(grid, JSON.parse(personalizzazioni));
            personalizzazioniGriglia.Personalizzazioni = null;
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
            sa_nome: "Tutti",
            sa_cod: "-1"
        }
    }).data("kendoDropDownList");
     
}


function ImpostaFiltriDaParametri() {
    var parametri = {};
    parametri._dataRegDal = $(cIdDataInizioRangeDDT).val();
    parametri._dataRegAl = $(cIdDataFineRangeDDT).val();
    parametri._descrizione = "";
    parametri._causali = undefined;
    parametri._centriAziendali = $(cIdSaCod).val();
    parametri._fabbricato = $(cIdFabbricatoCod).val();
    parametri._agenti = undefined; 
    // $(cIdSpecieVeg).val() === "" ? undefined : $(cIdSpecieVeg).val();
    parametri._specie = $(cIdSpecieVeg).val();
    parametri._varieta = $(cIdVarieta).val();
    parametri._gruppiDocumenti = undefined;
    parametri._prodotti = undefined;
    parametri._contatti = $(cIdContatto).val();
    parametri._categorie = $(cIdCategProdotto).val();
    parametri._categcommerciali = undefined;
    parametri._lotto = $(cIdLotto).val();
    parametri._dataGiacenza = $(cIdDataGiacenza).val();
    parametri._inGiacenza = $(cIdInGiacenza).val();

    //parametri._griglia = undefined;

    ValorizzaFiltriDaParametri(parametri);
}


function InviaRigheScelte() {
    var dsGrigliaDettagli = KendoGrid(DammiIDControlloGriglia()).dataSource.data();
    var righeScelteGriglia = $.grep(dsGrigliaDettagli, function (d) {
        return d.Selected === true;
    });
    window.parent.getRighe(righeScelteGriglia);
}


function kEventFiltering(e) {
    var filter = e.filter;

    if (filter === undefined || filter === null || !filter.value || filter.value.length < e.sender.minLength) {
        e.preventDefault();
    }
}

function pulisci_filtri_locale() {
    var kendoConfirm = $("<div></div>").kendoConfirm({
        title: "Attenzione",
        messages: { okText: "Sì", cancel: "No" },
        content: "Effettuare la pulizia dei filtri ?"
    }).data("kendoConfirm");
    kendoConfirm.result.done(function () {
        var dtOggi = kendo.date.today();
        set_data("Txt_DataRegDal", kendo.date.addDays(dtOggi, -31), null);
        set_data("Txt_DataRegAl", dtOggi, null);
        set_data("Txt_DataGiacenza", dtOggi, null);

        var kmsContatti = KendoMultisel("multiselContatti");
        if (kmsContatti.dataSource.data().length > 0) {
            kmsContatti.value([]);
        }
        
        KendoMultisel("multiselSpecie").value("");
        KendoMultisel("multiselVarieta").value("");

        KendoMultisel("multiselCategorie").value("");
        var kmsProdotti = KendoMultisel("multiselProdotti");
        if (kmsProdotti.dataSource.data().length > 0) {
            kmsProdotti.value([]);
        }

        //personalizzazioniGriglia = null;
        Esegui_Report();
    });

    kendoConfirm.open();
}

function cbProdGiacenzaChange(e) {
    KendoDDL("id_ddlFabbricato").enable(e.checked);
}