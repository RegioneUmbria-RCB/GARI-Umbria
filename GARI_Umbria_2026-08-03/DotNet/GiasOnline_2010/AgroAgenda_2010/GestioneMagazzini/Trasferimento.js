
//function Azione_Indietro_LavorazioniFF() {
//    let piva = getParameterByName('p');
//    window.location = "./RicercaLavorazioni.aspx?p=" + piva;
//}

function tabClick(e, elem) {

    if ($(elem).hasClass('disabled')) {
        e.preventDefault();

        $("<div></div>").kendoAlert({
            title: TraduzioneMultiResx(trasferimentoResx, "OperazioneNonConsentita", "Operazione non consentita"),
            content: TraduzioneMultiResx(trasferimentoResx, "ConfermareOAnnullareLeModifiche", "Confermare o annullare le modifiche.")
        }).data("kendoAlert").open();

        return false;
    }

}

/* START Da DDT di vendita */

//function popolaGrigliaRigheVendita(IDControllo) {
    
//    var funzioniCRUD = {
//        funzioneRead: RicercaRigheVendite,
//        checkBoxFunction: SelezionaRigheVendite
//    };
//    var idModel = "Id_Mov_Det";
//    var campiKendoModel = {
//        Id_Agenda: { type: "number" },
//        Id_Mov_Det: { type: "number" },
//        Lav_Cod: { type: "number" },
//        Sa_Cod: { type: "number" },
//        Des_Lib: { type: "string" },
//        Note: { type: "string" },
//        Causale_Trasporto: { type: "string" },
//        Soggetto_Codice: { type: "string" },
//        Soggetto_Piva: { type: "string" },
//        Soggetto_RagioneSociale: { type: "string" },
//        Soggetto_Rapporto: { type: "string" },
//        Stato: { type: "string" },
//        Regione: { type: "string" },
//        Provincia: { type: "string" },
//        Comune: { type: "string" },
//        Destinazione: { type: "string" },
//        Stato_Dest: { type: "string" },
//        Regione_Dest: { type: "string" },
//        Provincia_Dest: { type: "string" },
//        Comune_Dest: { type: "string" },
//        Vettore_RagioneSociale: { type: "string" },
//        Agente_RagioneSociale: { type: "string" },
//        Agente_Codice: { type: "string" },
//        Capoarea_RagioneSociale: { type: "string" },
//        Capoarea_Codice: { type: "string" },
//        Data_Movimento: { type: "date" },
//        Anno_Movimento: { type: "string" },
//        Mese_Movimento: { type: "string" },
//        Tipo_Documento: { type: "string" },
//        Scadenza: { type: "date" },
//        Numero_Movimento: { type: "string" },
//        Riga: { type: "string" },
//        Referenza_Codice: { type: "number" },
//        Referenza_Descr: { type: "string" },
//        Categoria_Prodotto: { type: "string" },
//        Categoria_Commerciale: { type: "string" },
//        Note_Prodotto: { type: "string" },
//        Lotto: { type: "string" },
//        Unita_Misura_Sigla: { type: "string" },
//        Unita_Misura_Secondaria_Sigla: { type: "string" },
//        Qta: { type: "number" },
//        Qta_Evasa: { type: "number" },
//        Qta_Residua: { type: "number" },
//        Qta_Extra: { type: "number" },
//        Kg_Netti: { type: "number" },
//        Qta_Netta_Evasa: { type: "number" },
//        Qta_Netta_Residua: { type: "number" },
//        Tara_Totale: { type: "number" },
//        Qta_Tara_Evasa: { type: "number" },
//        Qta_Tara_Residua: { type: "number" },
//        Kg_Lordi: { type: "number" },
//        Qta_Lorda_Evasa: { type: "number" },
//        Qta_Lorda_Residua: { type: "number" },
//        Nr_Imballi: { type: "number" },
//        Qta_Imballi_Evasa: { type: "number" },
//        Qta_Imballi_Residua: { type: "number" },
//        Nr_Contenitori: { type: "number" },
//        Qta_Contenitori_Evasa: { type: "number" },
//        Qta_Contenitori_Residua: { type: "number" },
//        Nr_Confezioni: { type: "number" },
//        Prezzo_Netto: { type: "number" },
//        Prezzo_Riferito_A: { type: "string" },
//        Imponibile: { type: "number" },
//        Sconto_Perc: { type: "number" },
//        Sconto: { type: "number" },
//        Imponibile_Netto: { type: "number" },
//        Iva: { type: "number" },
//        Importo: { type: "number" },
//        Provvigione: { type: "number" },
//        Provvigione_Calcolata: { type: "number" }
//    };

//    for (let i in elencoParametriQualitativi) {
//        if (elencoParametriQualitativi[i].Tabella_ID != 0) {
//            campiKendoModel[elencoParametriQualitativi[i].Tabella_Cod_Des + "_Descrizione"] = { type: "string" };
//        }
//    }

//    var colonneKendoGrid = [
//        { field: "Lavorato", title: TraduzioneMultiResx(trasferimentoResx, "Lavorato", "Lavorato"), filterable: { multi: true, search: true } },
//        { field: "Soggetto_RagioneSociale", title: TraduzioneMultiResx(trasferimentoResx, "Cliente", "Cliente"), filterable: { multi: true, search: true } },
//        { field: "Soggetto_Rapporto", title: TraduzioneMultiResx(trasferimentoResx, "TipoRapportoContabileAbbr", "Tipo Rap. Contab."), filterable: { multi: true, search: true } },
//        { field: "Data_Movimento", title: TraduzioneMultiResx(trasferimentoResx, "DataDocumentoAbbr", "Data Doc."), format: "{0:dd/MM/yyyy}", attributes: { style: "text-align:center;" }, groupHeaderTemplate: "Data Doc.: #= kendo.toString(value,'dd/MM/yyyy') #" },
//        { field: "Tipo_Documento", title: TraduzioneMultiResx(trasferimentoResx, "TipoDocumentoAbbr", "Tipo Doc."), filterable: { multi: true, search: true } },
//        { field: "Numero_Movimento", title: "Nr. Doc.", attributes: { style: "text-align:center;" } }, //i18n Quale abbreviazione di 'Numero' usare: 'N.', 'Nr.', 'Num.'?
//        { field: "Riga", title: TraduzioneMultiResx(trasferimentoResx, "Riga", "Riga"), attributes: { style: "text-align:center;" } },
//        { field: "Referenza_Descr", title: TraduzioneMultiResx(trasferimentoResx, "Prodotto", "Prodotto"), filterable: { multi: true, search: true } },
//        { field: "Unita_Misura_Sigla", title: TraduzioneMultiResx(trasferimentoResx, "UnitàDiMisuraAbbr", "UdM"), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true } },
//        { field: "Qta", title: TraduzioneMultiResx(trasferimentoResx, "RisorsaQuantità", "Quantità"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
//        { field: "Kg_Netti", title: TraduzioneMultiResx(trasferimentoResx, "QuantitàTotale", "Quantità Totale"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 150 },
//        { field: "Prezzo_Netto", title: TraduzioneMultiResx(trasferimentoResx, "PrezzoNetto", "Prezzo Netto"), format: "{0:n2}", attributes: { style: "text-align:right;" } },
//        { field: "Imponibile_Netto", title: TraduzioneMultiResx(trasferimentoResx, "ImponibileNetto", "Imponibile Netto"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
//        { field: "Iva", title: TraduzioneMultiResx(trasferimentoResx, "PartitaIVA", "Iva"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
//        { field: "Importo", title: TraduzioneMultiResx(trasferimentoResx, "Importo", "Importo"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
//        { field: "Soggetto_Codice", title: TraduzioneMultiResx(trasferimentoResx, "CodiceClienteAbbr", "Cod. Cliente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Stato", title: TraduzioneMultiResx(trasferimentoResx, "Stato", "Stato"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Regione", title: TraduzioneMultiResx(trasferimentoResx, "Regione", "Regione"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Provincia", title: TraduzioneMultiResx(trasferimentoResx, "Provincia", "Provincia"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Comune", title: TraduzioneMultiResx(trasferimentoResx, "Comune", "Comune"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Destinazione", title: TraduzioneMultiResx(trasferimentoResx, "Destinazione", "Destinazione"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
//        { field: "Stato_Dest", title: TraduzioneMultiResx(trasferimentoResx, "StatoDestinazioneAbbr", "Stato destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Regione_Dest", title: TraduzioneMultiResx(trasferimentoResx, "RegioneDestinazioneAbbr", "Regione destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Provincia_Dest", title: TraduzioneMultiResx(trasferimentoResx, "ProvinciaDestinazioneAbbr", "Provincia destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Comune_Dest", title: TraduzioneMultiResx(trasferimentoResx, "ComuneDestinazioneAbbr", "Comune destinaz."), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Vettore_RagioneSociale", title: TraduzioneMultiResx(trasferimentoResx, "Vettore", "Vettore"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
//        { field: "Agente_Codice", title: TraduzioneMultiResx(trasferimentoResx, "CodiceAgenteAbbr", "Cod. Agente"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Agente_RagioneSociale", title: TraduzioneMultiResx(trasferimentoResx, "Agente", "Agente"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
//        { field: "Capoarea_Codice", title: TraduzioneMultiResx(trasferimentoResx, "CodiceCapoAreaAbbr", "Cod. Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Capoarea_RagioneSociale", title: TraduzioneMultiResx(trasferimentoResx, "CapoArea", "Capo Area"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
//        { field: "Anno_Movimento", title: TraduzioneMultiResx(trasferimentoResx, "Anno", "Anno"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Mese_Movimento", title: TraduzioneMultiResx(trasferimentoResx, "AnnoEMese", "Anno-Mese"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Causale_Trasporto", title: TraduzioneMultiResx(trasferimentoResx, "Causale", "Causale"), filterable: { multi: true, search: true }, hidden: true, width: 200 },
//        { field: "Des_Lib", title: TraduzioneMultiResx(trasferimentoResx, "Descrizione", "Descrizione"), hidden: true, width: 150 },
//        { field: "Note", title: TraduzioneMultiResx(trasferimentoResx, "Note", "Note"), hidden: true, width: 150 },
//        { field: "Referenza_Codice", title: TraduzioneMultiResx(trasferimentoResx, "CodiceProdotto", "Codice Prodotto"), hidden: true, width: 150 },
//        { field: "Categoria_Prodotto", title: TraduzioneMultiResx(trasferimentoResx, "CategoriaProdotto", "Categoria Prodotto"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Categoria_Commerciale", title: "Categoria Commerciale", filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Note_Prodotto", title: "Descrizione Addizionale", hidden: true, width: 150 },
//        { field: "Lotto", title: TraduzioneMultiResx(trasferimentoResx, "Lotto2", "Lotto"), filterable: { multi: true, search: true }, hidden: true, width: 150 },
//        { field: "Unita_Misura_Secondaria_Sigla", title: TraduzioneMultiResx(trasferimentoResx, "UnitàDiMisuraSecondariaAbbr", "UdM Sec."), attributes: { style: "text-align:center;" }, filterable: { multi: true, search: true }, hidden: true, width: 100 },
//        { field: "Qta_Extra", title: "Litri/Kg", format: "{0:n2}", attributes: { style: "text-align:right;" }, hidden: true, width: 100 }, // i18n
//        { field: "Tara_Totale", title: TraduzioneMultiResx(trasferimentoResx, "Tara", "Tara"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
//        { field: "Kg_Lordi", title: "Kg Lordi", format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 }, //i18n
//        { field: "Nr_Imballi", title: TraduzioneMultiResx(trasferimentoResx, "NrImballi", "Nr. Imballi"), attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
//        { field: "Nr_Contenitori", title: TraduzioneMultiResx(trasferimentoResx, "NrContenitori", "Nr. Contenitori"), attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
//        // { field: "Nr_Confezioni", title: "Nr. Confezioni", attributes: { style: "text-align:right;" }, format: "{0:n0}", hidden: true, width: 100 },
//        { field: "Prezzo_Riferito_A", title: TraduzioneMultiResx(trasferimentoResx, "PrezzoRiferitoA", "Prezzo Rif. A"), filterable: { multi: true, search: true }, hidden: true, width: 100 },
//        { field: "Imponibile", title: TraduzioneMultiResx(trasferimentoResx, "Imponibile", "Imponibile"), format: "{0:n2}", hidden: true, width: 100 },
//        { field: "Sconto_Perc", title: TraduzioneMultiResx(trasferimentoResx, "Sconto", "Sconto") + " %", format: "{0:n2}", hidden: true, width: 100 },
//        { field: "Sconto", title: TraduzioneMultiResx(trasferimentoResx, "Sconto", "Sconto"), format: "{0:n2}", hidden: true, width: 100 },
//        { field: "Provvigione", title: TraduzioneMultiResx(trasferimentoResx, "Provvigione", "Provvigione") + " %", format: "{0:n2}", attributes: { style: "text-align:right;" }, hidden: true, width: 100 },
//        { field: "Provvigione_Calcolata", title: TraduzioneMultiResx(trasferimentoResx, "Provvigione", "Provvigione"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", hidden: true, width: 100 },
//        { field: "Scadenza", title: TraduzioneMultiResx(trasferimentoResx, "Scadenza", "Scadenza"), template: '#= (kendo.toString(Scadenza, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Scadenza, "dd/MM/yyyy" ) #', attributes: { style: "text-align:center;" }, groupHeaderTemplate: 'Scadenza: #= (kendo.toString(value, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(value, "dd/MM/yyyy" ) #', hidden: true, width: 100 }
//    ];

//    for (let i in elencoParametriQualitativi) {
//        if (elencoParametriQualitativi[i].Tabella_ID != 0) {
//            colonneKendoGrid.push({ field: elencoParametriQualitativi[i].Tabella_Cod_Des + "_Descrizione", title: elencoParametriQualitativi[i].Tabella_Cod_Des, hidden: true, width: 100 });
//        }
//    }

//    var parametriPerLettura = null;
//    var parametriDataSource = {
//        serverFiltering: false,
//        aggregate: [
//            { field: "Qta", aggregate: "sum" },
//            { field: "Qta_Evasa", aggregate: "sum" },
//            { field: "Qta_Residua", aggregate: "sum" },
//            { field: "Kg_Netti", aggregate: "sum" },
//            { field: "Tara_Totale", aggregate: "sum" },
//            { field: "Kg_Lordi", aggregate: "sum" },
//            { field: "Imponibile_Netto", aggregate: "sum" },
//            { field: "Iva", aggregate: "sum" },
//            { field: "Importo", aggregate: "sum" },
//            { field: "Provvigione_Calcolata", aggregate: "sum" }
//        ]
//    };
//    var parametriKendoGrid = {
//        columnMenu: true,
//        editable: false,
//        groupable: true,
//        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
//        pageable: { pageSizes: [5, 10, 20, 50, 100] },
//        reorderable: true,
//        pdf: false,
//        toolbarCommands: ["CreaLav"]
//    };

//    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundingRigheVendite, funzioneDaChiamarePrimaDiExcelExport: onExportExcel };
//    var mostraRigheCancellate = false;
//    var colonneDisabilitateSoloInModifica = null;

//    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
//        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
//        idModel, // chiave riga 
//        campiKendoModel, // campi modello
//        colonneKendoGrid, // colonne da mostrare
//        parametriPerLettura, // parametri da passare alla lettura
//        parametriDataSource, // parametri data source { chiave - valore}
//        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
//        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
//        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
//        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
//    );

//}

//function SelezionaRigheVendite(e) {

//    var row = $(this).parents("tr");
//    var grid = $("#tab_righe_vendita").data("kendoGrid");
//    var dataItem = grid.dataItem(row);

//    if (dataItem.Lavorato == "NO") {
//        var checked = this.checked;
//        dataItem.Selected = checked;
//        rowKendoGridSelected(row, checked)
//    } else {
//        this.checked = false;
//        // kendo.alert("Riga già lavorata");
//    }
    
//}

//function onDataBoundingRigheVendite(e) {
//    var gridId = e.sender.element[0].id;
//    var grid = $("#" + gridId).data("kendoGrid");
//    for (var i = 0; i < grid.columns.length; i++) {
//        grid.autoFitColumn(i);
//    }    
//}

//function creaLavorazioni() {

//    var grid = $("#tab_righe_vendita").data("kendoGrid");

//    if (grid.select().length === 0) {

//        kendo.alert(TraduzioneMultiResx(trasferimentoResx, "SelezionareAlmenoUnaRiga", "Selezionare almeno una riga"));

//    } else {

//        var allOk = true;
//        var idAgenda = $('input[name$="hdId_Agenda"]').val();

//        var righeDDT = [];
//        grid.select().each(function () {
//            righeDDT.push(grid.dataItem(this));
//        });

//        // se nuova lavorazione creo la testata da DDT
//        if (idAgenda === "") {

//            var lavorazione = GeneraOggettoLavorazioneDaDDT(righeDDT);
//            if (lavorazione === undefined || lavorazione === null) return;
//            var lav = kendoEscapeOggetto(lavorazione);
//            var ris = InserisciTestataLavorazione($(cIdPiva).val(), lav, true);

//            if (ris !== undefined && ris.RispostaOK === true) {

//                MessaggioTuttoOK_Bootstrap(TraduzioneMultiResx(trasferimentoResx, "SalvataggioEffettuatoCorrettamente", "Salvataggio effettuato correttamente"), "DIV_Messaggi");

//                //valorizzo l'hidden field
//                $(cIdAgenda).val(ris.RispostaStringa);
//                impostaTestataSolaLettura(true);

//                //devo eseguire le letture che si sarebbero dovute fare in PageLoad se avessi avuto una IdAgenda in QueryString
//                RicercaLavorazione("./Lavorazioni_WS.aspx", $(cIdPiva).val(), $(cIdAgenda).val());
//                RicercaScarichi("./Trasferimento.aspx", "tab_elenco_scarichi", $(cIdPiva).val(), $(cIdAgenda).val(), null);
//                // RicercaCarichi("./Trasferimento.aspx", "tab_elenco_carichi", $(cIdPiva).val(), $(cIdAgenda).val(), null);
//                RiempiLavorazione();

//            } else if (ris !== undefined && ris.RispostaOK === false) {
//                MessaggioErrore_Bootstrap(ris.RispostaStringa + "<br/>" + TraduzioneMultiResx(trasferimentoResx, "ErroreDuePunti_", "Errore: ") + ris.Errore, "DIV_Messaggi");
//                return;
//            } else {
//                MessaggioErrore_Bootstrap(TraduzioneMultiResx(trasferimentoResx, "ErroreSalvataggio", "Errore salvataggio"), "DIV_Messaggi");
//                return;
//            }
//        }

//        // inserisco le righe DDT nelle uscite lavorazioni
//        for (var i = 0; i < righeDDT.length; i++) {

//            destinazioni = RicercaMovimentiDestinazioni(righeDDT[i].Id_Agenda, righeDDT[i].Id_Mov, righeDDT[i].Id_Mov_Det);

//            if (destinazioni.length > 1) {

//                MessaggioErrore_Bootstrap(
//                    TraduzioneMultiResx(
//                        trasferimentoResx,
//                        "CreareRigaLavorazioneDaUscitePerUtilizzareProdottoProvenienteDaPiùCelle",
//                        "Non è possibile creare la lavorazione da una riga di vendita con prodotto consegnato da più di una cella. Creare la riga di lavorazione a mano da Uscite e successivamente modificare la riga di vendita per utilizzare il prodotto ottenuto dalla lavorazione"
//                    ),
//                    "DIV_Messaggi"
//                );
//                allOk = false;

//            } else {

//                var lavorazioneI = CreaLavorazioneDaDDT(righeDDT[i], destinazioni.length > 0 ? destinazioni[0] : null);

//                if (lavorazioneI !== undefined && lavorazioneI !== null) {

//                    var lavI = kendoEscapeOggetto(lavorazioneI);
//                    var risIns = InserisciCaricoLavorazione(lavorazioneI.piva, lavI, true);

//                    if (risIns !== undefined && risIns.RispostaOK === true) {
//                        MessaggioTuttoOK_Bootstrap(TraduzioneMultiResx(trasferimentoResx, "InserimentoEffettuatoCorrettamente", "Inserimento effettuato correttamente"), "DIV_Messaggi");
//                    } else if (risIns !== undefined && risIns.RispostaOK === false) {
//                        allOk = false;
//                        MessaggioErrore_Bootstrap(risIns.RispostaStringa + "<br/>" + TraduzioneMultiResx(trasferimentoResx, "ErroreDuePunti_", "Errore: ") + risIns.Errore, "DIV_Messaggi");
//                    } else {
//                        allOk = false;
//                        MessaggioErrore_Bootstrap(TraduzioneMultiResx(trasferimentoResx, "ErroreInserimento", "Errore inserimento"), "DIV_Messaggi");
//                    }
//                }
//            }

//        }

//        grid.dataSource.read();

//        if (allOk) {
//            // popolaGrigliaRigheVendita("tab_righe_vendita");
//            RicercaCarichi("./Trasferimento.aspx", "tab_elenco_carichi", $(cIdPiva).val(), $(cIdAgenda).val());
//            $('#tabs a[href="#tabCaricoDaLavorazione"]').tab("show");
//        }

//    }

//}

//function GeneraOggettoLavorazioneDaDDT(ddt) {

//    //Controllo conformità dei dati???
//    var contesto = TraduzioneMultiResx(trasferimentoResx, "SalvataggioLavorazione", "Salvataggio lavorazione");

//    var data_ddt = null;
//    var prodotti = KendoDDL("idProdotto_TLav").dataItems();
//    var prodotto_ddt = { Mat_Cod: 0 };
//    var linea_ddt = 0;

//    // ricavo data e prodotto da righe ddt
//    for (let i = 0; i < ddt.length; i++) {
//        if (data_ddt === null || ddt[i].Data_Movimento > data_ddt) {
//            data_ddt = ddt[i].Data_Movimento;
//        }
//        if (prodotto_ddt.Mat_Cod == 0) {
//            prodotto_ddt = { Elem_Cod: ddt[i].Elem_Cod, Mat_Cod: ddt[i].Mat_Cod, Mat_Des: ddt[i].Referenza_Descr, Lotto: ddt[i].Lotto };
//            for (var j = 0; j < prodotti.length; j++) {
//                if (ddt[i].Elem_Cod == prodotti[j].Elem_Cod && ddt[i].Mat_Cod == prodotti[j].Mat_Cod) {
//                    linea_ddt = prodotti[j].Linea_Cod;
//                    break;
//                }
//            }
//        }
//    }

//    // tipologia lavorazione
//    var codice_generazione = 0;
//    if (KendoDDL("idTipologia_TLav").value() !== "")
//        codice_generazione = parseInt(KendoDDL("idTipologia_TLav").value());
//    if (codice_generazione === 0) {
//        // TODO Migliorare questa ricerca del confezionamento
//        // Cerco la lavorazione di confezionamento standard
//        for (let i = 0; i < KendoDDL("idTipologia_TLav").dataSource._data.length; i++) {
//            if (KendoDDL("idTipologia_TLav").dataSource._data[i].Preparazione_Cod === "-137") {
//                Set_KendoDDLValue("idTipologia_TLav", "-137"); // Confezionamento
//                codice_generazione = -137;
//            }
//        }
//        if (codice_generazione === 0) {
//            for (let i = 0; i < KendoDDL("idTipologia_TLav").dataSource._data.length; i++) {
//                if (KendoDDL("idTipologia_TLav").dataSource._data[i].Preparazione_Des.includes("Confezionamento")) {
//                    Set_KendoDDLValue("idTipologia_TLav", KendoDDL("idTipologia_TLav").dataSource._data[i].Preparazione_Cod);
//                    codice_generazione = parseInt(KendoDDL("idTipologia_TLav").dataSource._data[i].Preparazione_Cod);
//                    break;
//                }
//            }
//        }
//    }
//    if (codice_generazione === 0) {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "AssegnareUnaTipologiaLavorazione", "Assegnare una tipologia lavorazione"));
//        return;
//    }

//    // data lavorazione
//    var data_tmp = KendoDate("idDataLavorazione_TLav").value();
//    if (data_tmp != null && data_tmp < data_ddt) data_ddt = data_tmp;
//    var data_lav = formattedReverseDate(data_ddt, "-"); // get_data("idDataLavorazione_TLav");
//    if (data_lav === null) {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "AssegnareUnaDataAllaLavorazione", "Assegnare una data alla lavorazione"));
//        return;
//    }

//    // fornitore
//    var fornitore_cod = parseInt(KendoDDL("idFornitore_TLav").value());
//    if (fornitore_cod === 0) {
//        var fornitori = KendoDDL("idFornitore_TLav").dataItems();
//        for (let i = 0; i < fornitori.length; i++) {
//            if (fornitori[i].Cod_Contatto == $(cIdPivaSuperuser).val()) {
//                fornitore_cod = fornitori[i].Cod_RisUm;
//                break;
//            }
//        }
//    }
//    if (fornitore_cod === 0) {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "AssegnareUnFornitorePerProdottoInUscita", "Assegnare un fornitore per prodotto in uscita"));
//        return;
//    }

//    // prodotto uscita
//    var prodotto = KendoDDL("idProdotto_TLav").dataItem();
//    if (prodotto.Mat_Cod === 0) prodotto = prodotto_ddt;
//    if (prodotto.Mat_Cod === 0) {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "AssegnareUnProdottoInUscita", "Assegnare un prodotto in uscita"));
//        return;
//    }

//    // linea prodotto
//    var linea_cod = parseInt(KendoDDL("idLinea_TLav").value());
//    if (linea_cod === 0) linea_cod = linea_ddt;
//    if (linea_cod === 0) {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "AssegnareUnaLineaProdotto", "Assegnare una linea prodotto"));
//        return;
//    }

//    // cella destinazione
//    var cella = KendoDDL("idCella_TLav").dataItem();
//    if (cella.Id_Destinazione === 0) {
//        var celle = KendoDDL("idCella_TLav").dataItems();
//        for (let i = 0; i < celle.length; i++) {
//            if (celle[i].Id_Destinazione != 0) {
//                cella = celle[i];
//                break;
//            }
//        }
//    }
//    if (cella.Id_Destinazione === 0) {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "AssegnareUnaCellaAllaLavorazione", "Assegnare una cella alla lavorazione"));
//        return;
//    }

//    // lotto lavorazione
//    var lottoAss = String($("#idLottoCaricato_TLav").val());
//    if (lottoAss === "") lottoAss = formattedDate(data_ddt, "-");
//    if (lottoAss === "") {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "AssegnareUnLottoAlProdottoInUscita", "Assegnare un lotto al prodotto in uscita"), "idLottoCaricato_TLav");
//        return;
//    }

//    var preparazione = FF_CercaPreparazioneDaLinea(codice_generazione, 0, linea_cod);
//    if (preparazione === null || preparazione === undefined) {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "PreparazioneNonPresente", "Preparazione non presente"));
//        return;
//    }
//    var preparazione_cod = preparazione.Preparazione_Cod;
//    var paramsID = ["idCalibro_TLav", "idQualita_TLav", "idCertificazione_TLav", "idRugginosita_TLav", "idProvenienza_TLav", "idImballo_TLav", "idContenitore_TLav", "idConfezione_TLav"];
//    var objParams = prendiParametriQualitativi(paramsID, false, "idTaraImballi_TLav", "idTaraContenitori_TLav", "idTaraConfezioni_TLav");

//    var forn = new Object();
//    forn.Tabella_Nome = "ofornitore";
//    forn.Param_ID = fornitore_cod;
//    forn.Tara = 0;
//    objParams.arrParams.push(forn);

//    if (objParams.flagConfezioni && !objParams.flagContenitori) {
//        MessaggioErrore_Term(contesto, TraduzioneMultiResx(trasferimentoResx, "ImpossibileScegliereConfezioneSenzaContenitore", "Non è possibile scegliere una confezione senza scegliere un contenitore"));
//        return;
//    }

//    var lavorazione = new Object();
//    lavorazione.dataMov = data_lav;
//    lavorazione.descrizioneAggiuntiva = $("#idDescrizione_TLav").val();
//    lavorazione.saCod = cella.Sa_Cod;
//    lavorazione.tipoLavorazioneDes = KendoDDL("idTipologia_TLav").text();
//    lavorazione.lineaCod = linea_cod;
//    lavorazione.preparazioneCod = preparazione_cod;
//    lavorazione.elemCod = prodotto.Elem_Cod;
//    lavorazione.matCod = prodotto.Mat_Cod;
//    lavorazione.matDes = prodotto.Mat_Des;
//    lavorazione.listMatPriCampValor = kendoEscapeOggetto(objParams.arrParams);
//    lavorazione.lotto = lottoAss;
//    lavorazione.idDestinazione = cella.Id_Destinazione;
//    lavorazione.tipoDestinazione = cella.Tipo_Destinazione;
//    lavorazione.nrContenitori = 0;
//    lavorazione.nrConfezioni = 0;
//    if (objParams.flagContenitori) {
//        lavorazione.nrContenitori = KendoNumericValue("idNrContenitori_TLav");
//        if (objParams.flagConfezioni)
//            lavorazione.nrConfezioni = KendoNumericValue("idNrConfezioni_TLav");
//    }

//    var confezionamento = new Object();
//    confezionamento.Cod_Imballaggio = Get_KendoDDLValue("idImballo_TLav");
//    confezionamento.Nr_Imballaggi = 1;
//    confezionamento.Cod_Contenitore = Get_KendoDDLValue("idContenitore_TLav");
//    confezionamento.Nr_Contenitori = lavorazione.nrContenitori;
//    confezionamento.Cod_Confezione = Get_KendoDDLValue("idConfezione_TLav");
//    confezionamento.Nr_Confezioni = lavorazione.nrConfezioni;

//    if (!controllaConfezionamentoAperturaLavorazione(confezionamento)) {
//        MessaggioErrore_Term(
//            TraduzioneMultiResx(trasferimentoResx, "CreazioneLavorazione", "Creazione lavorazione"),
//            TraduzioneMultiResx(trasferimentoResx, "ErroreImpostazioniImballaggio", "Errore impostazioni Imballaggio/Contenitore/Confezione")
//        );
//        return;
//    }

//    return lavorazione;
//}

//function CreaLavorazioneDaDDT(rigaDDT, destinazione) {

//    var lavorazione = new Object();

//    lavorazione.piva = $(cIdPiva).val();
//    lavorazione.idAgenda = $(cIdAgenda).val();
//    lavorazione.tipoLavorazioneDes = "";
//    lavorazione.elemCod = parseInt(rigaDDT.Elem_Cod);
//    lavorazione.matCod = rigaDDT.Mat_Cod;
//    //lavorazione.matDes = rigaDDT.Referenza_Descr;
//    lavorazione.lotto = rigaDDT.Lotto;
//    lavorazione.calCod = rigaDDT.Cal_Cod;
//    // lavorazione.udmCod = rigaDDT.Unita_Misura;

//    var numTotImballi = parseInt(rigaDDT.Nr_Imballi);
//    var numTotContenitori = parseInt(rigaDDT.Nr_Contenitori);
//    var numTotConfezioni = parseInt(rigaDDT.Nr_Confezioni);

//    var pesoNetto = parseFloat(rigaDDT.Kg_Netti);
//    var pesoLordo = parseFloat(rigaDDT.Kg_Lordi);
//    var taraTotale = parseFloat(rigaDDT.Tara_Totale); //pesoLordo - pesoNetto;

//    if (numTotConfezioni > 0) {
//        //38 = numero
//        lavorazione.udmCod = 38;
//        lavorazione.qta = numTotConfezioni;
//        lavorazione.qtaExtra = pesoNetto / numTotConfezioni;
//    } else {
//        //2 = kg
//        lavorazione.udmCod = 2;
//        lavorazione.qta = pesoNetto;
//        lavorazione.qtaExtra = 1;
//    }

//    lavorazione.qtaExtraTotale = pesoNetto;
//    lavorazione.tara = taraTotale;
//    lavorazione.numContenitori = numTotContenitori;
//    lavorazione.numImballaggi = numTotImballi;

//    if (destinazione != null && destinazione.Id_Destinazione != 0) {
//        lavorazione.tipoDestinazione = destinazione.Tipo_Destinazione;
//        lavorazione.saCod = destinazione.Sa_Cod;
//        lavorazione.idDestinazione = destinazione.Id_Destinazione;
//    }

//    //lavorazione.FF_imballaggio_Tipo_Cod = rigaDDT.imballaggio_Cod;
//    //lavorazione.FF_contenitore_Tipo_Cod = rigaDDT.contenitore_Cod;
//    //lavorazione.FF_confezione_Tipo_Cod = rigaDDT.confezione_Cod;
//    //lavorazione.FF_calibro_Tipo_Cod = rigaDDT.calibro_Cod;
//    //lavorazione.FF_grammatura_Tipo_Cod = rigaDDT.grammatura_Cod;
//    //lavorazione.FF_caratteristica_Tipo_Cod = rigaDDT.caratteristica_Cod;
//    //lavorazione.FF_qualità_Tipo_Cod = rigaDDT.imballaggio_Cod;
//    //lavorazione.FF_fornitore_Tipo_Cod = rigaDDT.fornitore_Cod;

//    /* var arrParams = creaArrayCampionature(rigaDDT);
//    if (arrParams !== undefined && arrParams !== null && arrParams.length > 0) {
//        lavorazione.listMatPriCampValor = kendoEscapeOggetto(arrParams);
//    } */

//    return lavorazione;
//}

/* END Da DDT di vendita */

function onExportExcel(e) {
    let rows = e.workbook.sheets[0].rows;
    for (let ri = 0; ri < rows.length; ri++) {
        let row = rows[ri];
        if (row.type === "group-footer" || row.type === "footer") {
            for (let ci = 0; ci < row.cells.length; ci++) {
                let cell = row.cells[ci];
                /* if (Object.prototype.toString.call(cell.value) === "[object Date]") {
                    let d = new Date(cell.value);
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

function CalcolaKgIngressiUscite() {

    let kg_ingressi = 0;
    let kendo_ingressi = JSON.parse($('input[name$="hdKendo_Scarichi"]').val());
    let ingressi = kendo_ingressi.kendo_rows;
    for (let i = 0; i < ingressi.length; i++) {
        kg_ingressi += ingressi[i].KgNetti;
    } 

    let kg_uscite = 0;
    let kendo_uscite = JSON.parse($('input[name$="hdKendo_Carichi"]').val());
    let uscite = kendo_uscite.kendo_rows;
    for (let i = 0; i < uscite.length; i++) {
        kg_uscite += uscite[i].KgNetti;
    }

    //if (FF_gest_materiale_vivaistico) {
        $("#a_tabScaricoSuLavorazione").html(kendo.format(TraduzioneMultiResx(trasferimentoResx, "UsciteQuantitàN", "Uscite (Q.tà: {0})"), kg_uscite));
    $("#a_tabCaricoDaLavorazione").html(kendo.format(TraduzioneMultiResx(trasferimentoResx, "IngressiQuantitàN", "Ingressi (Q.tà: {0})"), kg_ingressi));

    //} else {
    //    $("#a_tabScaricoSuLavorazione").html(kendo.format(TraduzioneMultiResx(trasferimentoResx, "UsciteNumKg", "Uscite ({0} Kg)"), kg_uscite));
    //    $("#a_tabCaricoDaLavorazione").html(kendo.format(TraduzioneMultiResx(trasferimentoResx, "IngressiNumKg", "Ingressi ({0} Kg)"), kg_ingressi));
    //}

}

function CreaOggettoInserisciTrasferimentoScarico(riga, bdistanza) {

    let trasferimento = new Object();
    trasferimento.Piva = riga.chiave_giacenze.split("_")[0];

    trasferimento.MagazzinoScarico = {
        TipoDestinazione: parseInt(riga.chiave_giacenze.split("_")[2]),
        SaCod: parseInt(riga.Sa_Cod),
        IdDestinazione: parseInt(riga.Id_Destinazione)
    };

    let cellaDest = KendoDDL("cmbDestinazioneDef").dataItem();
    trasferimento.MagazzinoCarico = {
        TipoDestinazione: cellaDest.Tipo_Destinazione,
        SaCod: cellaDest.Sa_Cod,
        IdDestinazione: cellaDest.Id_Destinazione
    };

    //trasferimento.SaCod = parseInt(riga.Sa_Cod);
    //trasferimento.TipoDestinazione = parseInt(riga.chiave_giacenze.split("_")[2]);
    //trasferimento.IdDestinazione = parseInt(riga.Id_Destinazione);

    trasferimento.ElemCod = parseInt(riga.Cat_Cod);
    trasferimento.ProCod = parseInt(riga.Pro_Cod);
    trasferimento.MatCod = parseInt(riga.Mat_Cod);
    trasferimento.MatDes = riga.Pro_Des;
    trasferimento.Lotto = riga.Lotto;
    trasferimento.CalCod = parseInt(riga.Cal_Cod);

    trasferimento.IdAgenda = parseInt($(cIdAgenda).val());


    let pesoNetto = kendo.parseFloat(riga.Netto_Mov);
    let pesoLordo = kendo.parseFloat(riga.Lordo_Mov);
    let qta = kendo.parseFloat(riga.Qta_Mov);

    let numConfezioni = parseInt(riga.NrConfezioni_Mov);
    let numContenitori = parseInt(riga.NrContenitori_Mov);
    let numImballaggi = parseInt(riga.NrImballaggi_Mov);

    let modalitaUdm = getModalitaUdm(riga);

    switch (modalitaUdm) {
        case ENUM_MOD_UDM.KG_CONF:

            // Posso scegliere la confezione solo con udm = KG (su db è sempre scritto udm = NR)

            //TODO: devono essere valorizzate numConfezioni e peso netto
            if (numConfezioni <= 0)
                throw new Error("numConfezioni <= 0");

            //38 = numero
            trasferimento.UdmCod = 38;
            trasferimento.Qta = numConfezioni;
            trasferimento.QtaExtra = pesoNetto / numConfezioni;
            trasferimento.UdmCodExtra = 2;

            break;

        case ENUM_MOD_UDM.KG:

            //Sono nella vecchia situazione: movimento a kg + eventuali contenitori e/o imballi

            //TODO: deve essere valorizzato peso netto
            if (pesoNetto <= 0)
                throw new Error("pesoNetto <= 0");

            //2 = kg
            trasferimento.UdmCod = 2;
            trasferimento.Qta = pesoNetto;
            trasferimento.QtaExtra = 1;

            if (numContenitori > 0 || numImballaggi > 0) {
                trasferimento.UdmCodExtra = 2;
            } else {
                trasferimento.UdmCodExtra = 0;
            }

            break;

        case ENUM_MOD_UDM.ALTRA_UDM:
        default:

            //Ho movimentato a numero o altre Udm

            trasferimento.UdmCod = parseInt(riga.Udm_Cod);
            trasferimento.Qta = qta;
            trasferimento.QtaExtra = 1;
            trasferimento.UdmCodExtra = 2;

            //Qta_Mov diventerà di fatto il Peso Netto
            pesoNetto = qta;

            break;
    }

    trasferimento.QtaExtraTotale = pesoNetto;
    trasferimento.Tara = pesoLordo - pesoNetto;


    switch (bdistanza) {
        case true:

            trasferimento.Distanza_Trasporto_UDM = KendoDDL("ddlUnitaMisuraTrasporto").dataItem().UDM_COD;
            trasferimento.Distanza_Trasporto = kendo.parseFloat($("input[name$='ntbDistanzaTrasporto']").val());

            if (isNaN(parseInt(trasferimento.Distanza_Trasporto))) {
                trasferimento.Distanza_Trasporto = 0;
            }

            break;
        default:

            trasferimento.Distanza_Trasporto_UDM = 0
            trasferimento.Distanza_Trasporto = 0

            break;
    }




    //if (numConfezioni > 0) {
    //    //38 = numero
    //    trasferimento.UdmCod = 38;
    //    trasferimento.Qta = numConfezioni;
    //    trasferimento.QtaExtra = pesoNetto / numConfezioni;
    //} else {
    //    //2 = kg
    //    trasferimento.UdmCod = 2;
    //    trasferimento.Qta = pesoNetto;
    //    trasferimento.QtaExtra = 1;
    //}

    trasferimento.NumContenitori = numContenitori;
    trasferimento.NumImballaggi = numImballaggi;

    trasferimento.DataMovimento = KendoDate("inDataEmissione").value();

    let w_modulo_anagrafe_log = moduloFromElemCod(trasferimento.ElemCod);
    trasferimento.ModuloGias = w_modulo_anagrafe_log;

    //TODO: devo vedere se sono gestiti oppure no!!! (se non sono gestiti, di sicuro non saranno > 0)

    //Questo mi serve per creare contestualmente i trasferimenti dei confezionamenti vuoti
    if (numImballaggi > 0 || numContenitori > 0) {
        trasferimento.Confezionamenti = new Object();

        if (numImballaggi > 0) {
            trasferimento.Confezionamenti.FF_imballaggio_Tipo_Cod = riga.FF_imballaggio_Tipo_Cod;
            trasferimento.Confezionamenti.FF_imballaggio_Descrizione = riga.FF_imballaggio_Descrizione;
            trasferimento.Confezionamenti.NrImballaggi = numImballaggi;
            trasferimento.Confezionamenti.FF_imballaggio_Codice_Generazione_Link = riga.FF_imballaggio_Codice_Generazione_Link;
            trasferimento.Confezionamenti.FF_imballaggio_Mat_Cod_Generazione_Link = riga.FF_imballaggio_Mat_Cod_Generazione_Link;
            //trasferimento.Confezionamenti.FF_imballaggio_Mat_Cod = 0;
            trasferimento.Confezionamenti.FF_imballaggio_Tara_Campionatura = riga.FF_imballaggio_Tara_Campionatura;
        }

        if (numContenitori > 0) {
            trasferimento.Confezionamenti.FF_contenitore_Tipo_Cod = riga.FF_contenitore_Tipo_Cod;
            trasferimento.Confezionamenti.FF_contenitore_Descrizione = riga.FF_contenitore_Descrizione;
            trasferimento.Confezionamenti.NrContenitori = numContenitori;
            trasferimento.Confezionamenti.FF_contenitore_Codice_Generazione_Link = riga.FF_contenitore_Codice_Generazione_Link;
            trasferimento.Confezionamenti.FF_contenitore_Mat_Cod_Generazione_Link = riga.FF_contenitore_Mat_Cod_Generazione_Link;
            //trasferimento.Confezionamenti.FF_contenitore_Mat_Cod: 0;
            trasferimento.Confezionamenti.FF_contenitore_Tara_Campionatura = riga.FF_contenitore_Tara_Campionatura;
        }
    }

    return trasferimento;

}

function CreaOggettoModificaTrasferimentoScarico(daModificare, rigaOriginale, bdistanza) {

    let trasferimento = new Object();

    trasferimento.Piva = daModificare.key_mov_dett.split("_")[0];
    trasferimento.SaCod = parseInt(daModificare.key_mov_dett.split("_")[1]);
    trasferimento.IdAgenda = parseInt(daModificare.key_mov_dett.split("_")[2]);
    trasferimento.IdMov = parseInt(daModificare.key_mov_dett.split("_")[3]);
    trasferimento.IdMovDet = parseInt(daModificare.key_mov_dett.split("_")[4]);
    trasferimento.TipoDestinazione = parseInt(daModificare.key_mov_dett.split("_")[6]);
    trasferimento.IdDestinazione = parseInt(daModificare.key_mov_dett.split("_")[8]);


    trasferimento.MagazzinoScarico = {
        TipoDestinazione: trasferimento.TipoDestinazione,
        SaCod: trasferimento.SaCod,
        IdDestinazione: trasferimento.IdDestinazione
    };

    trasferimento.MagazzinoCarico = {
        TipoDestinazione: daModificare.Trasf_Tipo_Destinazione_2,
        SaCod: daModificare.Trasf_Sa_Cod_2,
        IdDestinazione: daModificare.Trasf_Id_Destinazione_2
    };

    trasferimento.ElemCod = parseInt(daModificare.Cat_Cod);
    trasferimento.ProCod = parseInt(daModificare.Pro_Cod);  //??? per il momento in realtà non mi serve
    trasferimento.MatCod = parseInt(daModificare.Mat_Cod);
    trasferimento.Lotto = daModificare.Lotto;
    trasferimento.CalCod = parseInt(daModificare.Cal_Cod);

    let numImballaggi = parseInt(daModificare.NrImballaggi);
    let numContenitori = parseInt(daModificare.NrContenitori);
    let numConfezioni = parseInt(daModificare.NrConfezioni);

    let pesoNetto = kendo.parseFloat(daModificare.KgNetti);
    let pesoLordo = kendo.parseFloat(daModificare.KgLordi);
    let qta = kendo.parseFloat(daModificare.Qta);

    let modalitaUdm = getModalitaUdm(daModificare);

    switch (modalitaUdm) {
        case ENUM_MOD_UDM.KG_CONF:

            // Posso scegliere la confezione solo con udm = KG (su db è sempre scritto udm = NR)

            //TODO: devono essere valorizzate numConfezioni e peso netto
            if (numConfezioni <= 0)
                throw new Error();

            //38 = numero
            trasferimento.UdmCod = 38;
            trasferimento.Qta = numConfezioni;
            trasferimento.QtaExtra = pesoNetto / numConfezioni;
            trasferimento.UdmCodExtra = 2;

            break;

        case ENUM_MOD_UDM.KG:

            //Sono nella vecchia situazione: movimento a kg + eventuali contenitori e/o imballi

            //TODO: deve essere valorizzato peso netto
            if (pesoNetto <= 0)
                throw new Error();

            //2 = kg
            trasferimento.UdmCod = 2;
            trasferimento.Qta = pesoNetto;
            trasferimento.QtaExtra = 1;

            if (numContenitori > 0 || numImballaggi > 0) {
                trasferimento.UdmCodExtra = 2;
            } else {
                trasferimento.UdmCodExtra = 0;
            }

            break;

        case ENUM_MOD_UDM.ALTRA_UDM:
        default:

            //Ho movimentato a numero o altre Udm

            trasferimento.UdmCod = parseInt(daModificare.Udm_Cod);
            trasferimento.Qta = qta;
            trasferimento.QtaExtra = 1;
            trasferimento.UdmCodExtra = 2;

            //Qta_Mov diventerà di fatto il Peso Netto
            pesoNetto = qta;

            break;
    }
    

    let taraTotale = pesoLordo - pesoNetto;


    switch (bdistanza) {
        case true:

            trasferimento.Distanza_Trasporto_UDM = KendoDDL("ddlUnitaMisuraTrasporto").dataItem().UDM_COD;
            trasferimento.Distanza_Trasporto = kendo.parseFloat($("input[name$='ntbDistanzaTrasporto']").val());

            if (isNaN(parseInt(trasferimento.Distanza_Trasporto))) {
                trasferimento.Distanza_Trasporto = 0;
            }

            break;
        default:

            trasferimento.Distanza_Trasporto_UDM = 0
            trasferimento.Distanza_Trasporto = 0

            break;
    }

    //if (numTotConfezioni > 0) {
    //    //38 = numero
    //    trasferimento.UdmCod = 38;
    //    trasferimento.Qta = numTotConfezioni;
    //    trasferimento.QtaExtra = pesoNetto / numTotConfezioni;
    //} else {
    //    //2 = kg
    //    trasferimento.UdmCod = 2;
    //    trasferimento.Qta = pesoNetto;
    //    trasferimento.QtaExtra = 1;
    //}

    trasferimento.NumContenitori = numContenitori;
    trasferimento.NumImballaggi = numImballaggi;

    trasferimento.QtaExtraTotale = pesoNetto;
    trasferimento.Tara = taraTotale;

    trasferimento.DataMovimento = KendoDate("inDataEmissione").value();

    let w_modulo_anagrafe_log = moduloFromElemCod(trasferimento.ElemCod);
    trasferimento.ModuloGias = w_modulo_anagrafe_log;
    trasferimento.DataLettura = daModificare.DataOraUltimaLettura;

    trasferimento.MovDettaglioRif = {
        Piva: trasferimento.Piva,
        Sa_Cod: 0,
        Id_Agenda: trasferimento.IdAgenda,
        Id_Mov: daModificare.Trasf_Rif_Id_Mov,
        Id_Mov_Det: daModificare.Trasf_Rif_Id_Mov_Det,
        Lav_Cod: 1033,
        Cau_Mov: daModificare.Trasf_Rif_Cau_Mov,
        Piva_Rif: trasferimento.Piva,
        Sa_Cod_Rif: 0,
        Id_Agenda_Rif: trasferimento.IdAgenda,
        Id_Mov_Rif: daModificare.Trasf_Rif_Id_Mov_Rif,
        Id_Mov_Det_Rif: daModificare.Trasf_Rif_Id_Mov_Det_Rif,
        Lav_Cod_Rif: 1033,
        Cau_Mov_Rif: daModificare.Trasf_Rif_Cau_Mov_Rif
    };

    if (rigaOriginale !== null && rigaOriginale.length === 1) {
        trasferimento.KgPrecedenti = kendo.parseFloat(rigaOriginale[0].KgNetti);
    }

    return trasferimento;
}

function CreaOggettoModificaTrasferimentoCarico(daModificare, rigaOriginale) {

    //TODO: per il momento, potendo cambiare solo il magazzino, mi sto portando dietro molta roba che probabilmente non mi serve, ma per il momento la lascio

    let trasferimento = new Object();

    trasferimento.Piva = daModificare.key_mov_dett.split("_")[0];
    trasferimento.SaCod = parseInt(daModificare.key_mov_dett.split("_")[1]);
    trasferimento.IdAgenda = parseInt(daModificare.key_mov_dett.split("_")[2]);
    trasferimento.IdMov = parseInt(daModificare.key_mov_dett.split("_")[3]);
    trasferimento.IdMovDet = parseInt(daModificare.key_mov_dett.split("_")[4]);
    trasferimento.TipoDestinazione = parseInt(daModificare.key_mov_dett.split("_")[6]);
    trasferimento.IdDestinazione = parseInt(daModificare.key_mov_dett.split("_")[8]);

    trasferimento.MagazzinoCarico = {
        TipoDestinazione: parseInt(daModificare.key_Dest.split("_")[0]),
        SaCod: parseInt(daModificare.key_Dest.split("_")[1]),
        IdDestinazione: parseInt(daModificare.key_Dest.split("_")[2])
    };

    trasferimento.ElemCod = parseInt(daModificare.Cat_Cod);
    trasferimento.ProCod = parseInt(daModificare.Pro_Cod);  //??? per il momento in realtà non mi serve
    trasferimento.MatCod = parseInt(daModificare.Mat_Cod);
    trasferimento.Lotto = daModificare.Lotto;
    trasferimento.CalCod = parseInt(daModificare.Cal_Cod);

    let numImballaggi = parseInt(daModificare.NrImballaggi);
    let numContenitori = parseInt(daModificare.NrContenitori);
    let numConfezioni = parseInt(daModificare.NrConfezioni);

    let pesoNetto = kendo.parseFloat(daModificare.KgNetti);
    let pesoLordo = kendo.parseFloat(daModificare.KgLordi);
    let qta = kendo.parseFloat(daModificare.Qta);

    let modalitaUdm = getModalitaUdm(daModificare);

    switch (modalitaUdm) {
        case ENUM_MOD_UDM.KG_CONF:

            // Posso scegliere la confezione solo con udm = KG (su db è sempre scritto udm = NR)

            //TODO: devono essere valorizzate numConfezioni e peso netto
            if (numConfezioni <= 0)
                throw new Error();

            //38 = numero
            trasferimento.UdmCod = 38;
            trasferimento.Qta = numConfezioni;
            trasferimento.QtaExtra = pesoNetto / numConfezioni;
            trasferimento.UdmCodExtra = 2;

            break;

        case ENUM_MOD_UDM.KG:

            //Sono nella vecchia situazione: movimento a kg + eventuali contenitori e/o imballi

            //TODO: deve essere valorizzato peso netto
            if (pesoNetto <= 0)
                throw new Error();

            //2 = kg
            trasferimento.UdmCod = 2;
            trasferimento.Qta = pesoNetto;
            trasferimento.QtaExtra = 1;

            if (numContenitori > 0 || numImballaggi > 0) {
                trasferimento.UdmCodExtra = 2;
            } else {
                trasferimento.UdmCodExtra = 0;
            }

            break;

        case ENUM_MOD_UDM.ALTRA_UDM:
        default:

            //Ho movimentato a numero o altre Udm

            trasferimento.UdmCod = parseInt(riga.Udm_Cod);
            trasferimento.Qta = qta;
            trasferimento.QtaExtra = 1;
            trasferimento.UdmCodExtra = 2;

            //Qta_Mov diventerà di fatto il Peso Netto
            pesoNetto = qta;

            break;
    }


    let taraTotale = pesoLordo - pesoNetto;

    //if (numConfezioni > 0) {
    //    //38 = numero
    //    trasferimento.UdmCod = 38;
    //    trasferimento.Qta = numConfezioni;
    //    trasferimento.QtaExtra = pesoNetto / numConfezioni;
    //} else {
    //    //2 = kg
    //    trasferimento.UdmCod = 2;
    //    trasferimento.Qta = pesoNetto;
    //    trasferimento.QtaExtra = 1;
    //}

    trasferimento.NumContenitori = numContenitori;
    trasferimento.NumImballaggi = numImballaggi;

    trasferimento.QtaExtraTotale = pesoNetto;
    trasferimento.Tara = taraTotale;

    trasferimento.DataMovimento = KendoDate("inDataEmissione").value();

    let w_modulo_anagrafe_log = moduloFromElemCod(trasferimento.ElemCod);
    trasferimento.ModuloGias = w_modulo_anagrafe_log;
    trasferimento.DataLettura = daModificare.DataOraUltimaLettura;

    trasferimento.MovDettaglioRif = {
        Piva: trasferimento.Piva,
        Sa_Cod: 0,
        Id_Agenda: trasferimento.IdAgenda,
        Id_Mov: daModificare.Trasf_Rif_Id_Mov,
        Id_Mov_Det: daModificare.Trasf_Rif_Id_Mov_Det,
        Lav_Cod: 1033,
        Cau_Mov: daModificare.Trasf_Rif_Cau_Mov,
        Piva_Rif: trasferimento.Piva,
        Sa_Cod_Rif: 0,
        Id_Agenda_Rif: trasferimento.IdAgenda,
        Id_Mov_Rif: daModificare.Trasf_Rif_Id_Mov_Rif,
        Id_Mov_Det_Rif: daModificare.Trasf_Rif_Id_Mov_Det_Rif,
        Lav_Cod_Rif: 1033,
        Cau_Mov_Rif: daModificare.Trasf_Rif_Cau_Mov_Rif
    };

    if (rigaOriginale !== null && rigaOriginale.length === 1) {

        trasferimento.MagazzinoCaricoPrecedente = {
            TipoDestinazione: parseInt(rigaOriginale[0].key_mov_dett.split("_")[6]),
            SaCod: parseInt(rigaOriginale[0].key_mov_dett.split("_")[7]),
            IdDestinazione: parseInt(rigaOriginale[0].key_mov_dett.split("_")[8])
        };

    }

    return trasferimento;
}

function CreaOggettoCancellazioneTrasferimentoScarico(daCancellare) {

    let trasferimento = new Object();

    trasferimento.Piva = daCancellare.key_mov_dett.split("_")[0];
    trasferimento.SaCod = parseInt(daCancellare.key_mov_dett.split("_")[1]);
    trasferimento.IdAgenda = parseInt(daCancellare.key_mov_dett.split("_")[2]);
    trasferimento.IdMov = parseInt(daCancellare.key_mov_dett.split("_")[3]);
    trasferimento.IdMovDet = parseInt(daCancellare.key_mov_dett.split("_")[4]);

    trasferimento.MagazzinoCarico = {
        TipoDestinazione: daCancellare.Trasf_Tipo_Destinazione_2,
        SaCod: daCancellare.Trasf_Sa_Cod_2,
        IdDestinazione: daCancellare.Trasf_Id_Destinazione_2
    };

    trasferimento.ElemCod = parseInt(daCancellare.Cat_Cod);
    trasferimento.ProCod = parseInt(daCancellare.Pro_Cod);  //??? per il momento in realtà non mi serve
    trasferimento.MatCod = parseInt(daCancellare.Mat_Cod);
    trasferimento.Lotto = daCancellare.Lotto;
    trasferimento.CalCod = parseInt(daCancellare.Cal_Cod);

    let numImballaggi = parseInt(daCancellare.NrImballaggi);
    let numContenitori = parseInt(daCancellare.NrContenitori);
    let numConfezioni = parseInt(daCancellare.NrConfezioni);

    let pesoNetto = kendo.parseFloat(daCancellare.KgNetti);
    let pesoLordo = kendo.parseFloat(daCancellare.KgLordi);
    let qta = kendo.parseFloat(daCancellare.Qta_Mov);

    let modalitaUdm = getModalitaUdm(daCancellare);

    switch (modalitaUdm) {
        case ENUM_MOD_UDM.KG_CONF:

            // Posso scegliere la confezione solo con udm = KG (su db è sempre scritto udm = NR)

            //TODO: devono essere valorizzate numConfezioni e peso netto
            if (numConfezioni <= 0)
                throw new Error();

            //38 = numero
            trasferimento.UdmCod = 38;
            trasferimento.Qta = numConfezioni;
            trasferimento.QtaExtra = pesoNetto / numConfezioni;
            trasferimento.UdmCodExtra = 2;

            break;

        case ENUM_MOD_UDM.KG:

            //Sono nella vecchia situazione: movimento a kg + eventuali contenitori e/o imballi

            //TODO: deve essere valorizzato peso netto
            if (pesoNetto <= 0)
                throw new Error();

            //2 = kg
            trasferimento.UdmCod = 2;
            trasferimento.Qta = pesoNetto;
            trasferimento.QtaExtra = 1;

            if (numContenitori > 0 || numImballaggi > 0) {
                trasferimento.UdmCodExtra = 2;
            } else {
                trasferimento.UdmCodExtra = 0;
            }

            break;

        case ENUM_MOD_UDM.ALTRA_UDM:
        default:

            //Ho movimentato a numero o altre Udm

            trasferimento.UdmCod = parseInt(riga.Udm_Cod);
            trasferimento.Qta = qta;
            trasferimento.QtaExtra = 1;
            trasferimento.UdmCodExtra = 2;

            //Qta_Mov diventerà di fatto il Peso Netto
            pesoNetto = qta;

            break;
    }

    let taraTotale = pesoLordo - pesoNetto;

    //if (numConfezioni > 0) {
    //    //38 = numero
    //    trasferimento.UdmCod = 38;
    //    trasferimento.Qta = numConfezioni;
    //    trasferimento.QtaExtra = pesoNetto / numConfezioni;
    //} else {
    //    //2 = kg
    //    trasferimento.UdmCod = 2;
    //    trasferimento.Qta = pesoNetto;
    //    trasferimento.QtaExtra = 1;
    //}

    trasferimento.NumContenitori = numContenitori;
    trasferimento.NumImballaggi = numImballaggi;

    trasferimento.QtaExtraTotale = pesoNetto;
    trasferimento.Tara = taraTotale;

    trasferimento.DataMovimento = KendoDate("inDataEmissione").value();

    let w_modulo_anagrafe_log = moduloFromElemCod(trasferimento.ElemCod);
    trasferimento.ModuloGias = w_modulo_anagrafe_log;
    trasferimento.DataLettura = daCancellare.DataOraUltimaLettura;


    trasferimento.MovDettaglioRif = {
        Piva: trasferimento.Piva,
        Sa_Cod: 0,
        Id_Agenda: trasferimento.IdAgenda,
        Id_Mov: daCancellare.Trasf_Rif_Id_Mov,
        Id_Mov_Det: daCancellare.Trasf_Rif_Id_Mov_Det,
        Lav_Cod: 1033,
        Cau_Mov: daCancellare.Trasf_Rif_Cau_Mov,
        Piva_Rif: trasferimento.Piva,
        Sa_Cod_Rif: 0,
        Id_Agenda_Rif: trasferimento.IdAgenda,
        Id_Mov_Rif: daCancellare.Trasf_Rif_Id_Mov_Rif,
        Id_Mov_Det_Rif: daCancellare.Trasf_Rif_Id_Mov_Det_Rif,
        Lav_Cod_Rif: 1033,
        Cau_Mov_Rif: daCancellare.Trasf_Rif_Cau_Mov_Rif
    };

    return trasferimento;
}


function inDataEmissione_change(e) {
    //mantengo sincronizzata la data di giacenza con la data emissione
    let newDataEm = KendoDate("inDataEmissione").value();
    KendoDate("txt_DataRif_GiacenzeMagazzino").value(newDataEm);
}

function moduloFromElemCod(elem_cod) {
    // ModuloGenerazione
    //let Modulo_Cantine = 1;
    let Modulo_FreshFood = 2;
    //let Modulo_Tabacco = 3;
    let Modulo_Zoo = 5;

    let w_modulo_anagrafe_log = 0;
    if (elem_cod === TRASFORMATI_VEGETALI &&
        modulo_anagrafe_log.includes(Modulo_FreshFood))
        w_modulo_anagrafe_log = Modulo_FreshFood;
    else
        if (elem_cod === TRASFORMATI_ANIMALI &&
            modulo_anagrafe_log.includes(Modulo_Zoo))
            w_modulo_anagrafe_log = Modulo_Zoo;

    return w_modulo_anagrafe_log;
}