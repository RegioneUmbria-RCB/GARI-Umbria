
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////     GRIGLIE DETTAGLI   ///////////////////////////////////////////

//const { Alert } = require("bootstrap");

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
let imprese = null;
function anno_Change() {
    $("#row_Fascicolo").show();
    $("#id-panel-fascicolo-UF").show()
    ddlFascicoli_Load('#fascicolo');
    if (QS_Anticipo == 0) {
        checkPerc();
        PopolaTabellaCalcoloCosti();
        RiempiElencoLavorazioniValidita();
    }

    if ($("#anno").val() >= 2025) {
        $("#row_Fascicolo").hide();
        //$("#btn_DettaglioAppezzamenti").hide();
        $("#id-panel-fascicolo-UF").hide()
        if (KendoDDL("ddlAzienda").value() != "") {
            KendoDDL("fascicolo").value(0)
        }
    }
}

function ConfiguraGrigliaDettagliImpianti(IDControllo, AggiornaImpianti) {

    switch ($("#stato_pratica_cod").val()) {
        case "":
        case In_Compilazione.toString():
            if ((permesso_richiesta && QS_Avanzamento == 0) || (permesso_rendicontazione && QS_Avanzamento == 1)) {
                //modifica_richiesto = true;
                modifica_assegnato = false;
                if (QS_Anticipo == 0) {
                    $("#btn_salva").show();
                } else {
                    $("#panel - rimanenze - anno - prec").hide();
                }
            }
            break;
        case Verifica_In_Corso.toString():
            if ((permesso_approvazione_richiesta && QS_Avanzamento == 0) || (permesso_approvazione_rendicontazione && QS_Avanzamento == 1)) {
                modifica_richiesto = false;
                //modifica_assegnato = true;
                if (QS_Anticipo == 0) {
                    $("#btn_AssegnaAutomaticamenteCarburante").show();
                    $("#btn_salva").show();
                } else {
                    $("#panel - rimanenze - anno - prec").hide();

                }
            }
            break;
        case Rinuncia.toString():
            richiestaRinuncia = true;
            break;        
        default:
            modifica_assegnato = false;
            modifica_richiesto = false;
            break;
    }

    var omettiAnnulla = false;
    var omettiSalva = true;
    var omettiSalva = true;

    var funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti, */flagInsert: !richiestaRinuncia, flagUpdate: !richiestaRinuncia /*flagDelete: true*/ };

    var funzioniCRUD = {
        funzioneRead: RicercaAgenda,
        funzioneSubmit: funzioneSubmitDaUsare,
        funzioneInsert: () => { },
        UtenteAbilitatoInserimentoModifica: !richiestaRinuncia && (modifica_richiesto),
        UtenteAbilitatoCancellazione: !richiestaRinuncia && (modifica_richiesto),
        omettiPulsantiSalva: omettiSalva,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var styleOut = "vertical-align: middle; text-align: center;";

    var idModel = "UMA_Cod";
    var campiKendoModel = null;
    var colonneCustomKendoGrid = new Array();
    if (!richiestaRinuncia && (modifica_richiesto)) {
        colonneCustomKendoGrid.push({
            command: {
                template: "<div class='btn-group-vertical'>" +
                    "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaRigaImpianto(this.closest('tr'),this.closest('.k-grid'))>Cancella</div>" +
                    //"<div class='btn btn-warning btnCorreggi' style='display:none;width:70px;border:0px;' onclick=correggiSuperficieRigaImpianto(this.closest('tr'),this.closest('.k-grid'))>Correggi</div>" +
                    "</div>" 
            }, title: "Azioni", width: "97px", headerAttributes: { style: styleOut }
        });
    }

    colonneCustomKendoGrid.push({
        command: {
            template: "<div class='btn btn-info btnInfo btnDettaglio' style='width:25px;border:0px;' onclick=DettaglioColture(this.closest('tr'),this.closest('.k-grid'))><span class='fa fa-list lampeggiante'></span></div>"
        },
        title: "Det.", width: "54px", headerAttributes: { style: styleOut }
    });

    //colonneCustomKendoGrid.push({
    //    command: {
    //        template: "<div class='btn btn-info btnInfo btnDettaglio' style='width:25px;border:0px;' onclick=DettaglioAppezzamenti(this.closest('tr'),this.closest('.k-grid'))><span class='fa fa-list lampeggiante'></span></div>"
    //    },
    //    title: "Det.", width: "54px", headerAttributes: { style: styleOut }

    //});
    //kendo.alert('dopo creazione colonne');


    campiKendoModel = {
        UMA_Cod: { editable: false, type: "string" },
        Richiesta_Cod: { editable: false, type: "number" },
        Gruppo_Colturale_UMA: { editable: !richiestaRinuncia && modifica_richiesto, type: "String", validation: { required: true } },
        Programmazione_Cod: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Programmazione_Des: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        Regolamento_Cod: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: false }, defaultValue: 1 },
        Regolamento_Check: { editable: !richiestaRinuncia && modifica_richiesto, type: "boolean", validation: { required: false } },
        Macrouso_UMA_Cod: { editable: false, type: "String", validation: { required: true } },
        Veg_Cod: { editable: false, type: "string", validation: { required: true } },
        Id_Cod: { editable: false, type: "string", validation: { required: true } },
        sup_UMA: { editable: false, type: "number", validation: { required: true } },
        sup_UMA_A: { editable: false, type: "number", validation: { required: true } },
        sup_UMA_B: { editable: false, type: "number", validation: { required: true } },
        sup_UMA_Edit: { editable: false, type: "number", validation: { required: true } },
        sup_UMA_A_Edit: { editable: false, type: "number", validation: { required: true } },
        sup_UMA_B_Edit: { editable: false, type: "number", validation: { required: true } },
        calcolato: { editable: false, type: "number", validation: { required: true } },
        richiesto: { editable: false, type: "number", validation: { required: true } },
        assegnato: { editable: false, type: "number", validation: { required: true } },
        TerrenoNormale: { editable: false, type: "number", validation: { required: true } },
        TerrenoMedio: { editable: false, type: "number", validation: { required: true } },
        TerrenoTenace: { editable: false, type: "number", validation: { required: true } },
        TerrenoNormale_Edit: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoMedio_Edit: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoTenace_Edit: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } }
    };

    //var footerTemplateStringsupUMA = "Totale superficie dichiarata U.M.A.: <span id='footerPlaceholderSupUMA" + "'>#=calcTotaleColonna('" + "sup_UMA" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupUMA_A = "Totale zona A U.M.A.: <span id='footerPlaceholderSupUMA_A" + "'>#=calcTotaleColonna('" + "sup_UMA_A" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupUMA_B = "Totale zona B U.M.A.: <span id='footerPlaceholderSupUMA_B" + "'>#=calcTotaleColonna('" + "sup_UMA_B" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupFASC = "Totale superficie da fascicolo: <span id='footerPlaceholderSupFASC" + "'>#=calcTotaleColonna('" + "sup_FASC" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupFASC_A = "Totale zona A fascicolo: <span id='footerPlaceholderSupFASC_A" + "'>#=calcTotaleColonna('" + "sup_FASC_A" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupFASC_B = "Totale zona B fascicolo: <span id='footerPlaceholderSupFASC_B" + "'>#=calcTotaleColonna('" + "sup_FASC_B" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringCalcolato = "Totale carburante calcolato: <span id='footerPlaceholderCalcolato" + "'>#=calcTotaleColonna('" + "calcolato" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringRichiesto = "Totale carburante richiesto: <span id='footerPlaceholderRichiesto" + "'>#=calcTotaleColonna('" + "richiesto" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringAssegnato = "Totale carburante assegnato: <span id='footerPlaceholderAssegnato" + "'>#=calcTotaleColonna('" + "assegnato" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringTerrNorm = "Totale terreno normale: <span id='footerPlaceholderTerrNorm" + "'>#=calcTotaleColonna('" + "TerrenoNormale" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringTerrMed = "Totale terreno medio: <span id='footerPlaceholderTerrMed" + "'>#=calcTotaleColonna('" + "TerrenoMedio" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringTerrTen = "Totale terreno tenace: <span id='footerPlaceholderTerrTen" + "'>#=calcTotaleColonna('" + "TerrenoTenace" + "', " + IDControllo + ")#</span>";

    var footerTemplateStringsupUMA = "#=calcTotaleColonna('" + "sup_UMA" + "', " + IDControllo + ")#";
    var footerTemplateStringsupUMA_A = "#=calcTotaleColonna('" + "sup_UMA_A" + "', " + IDControllo + ")#";
    var footerTemplateStringsupUMA_B = "#=calcTotaleColonna('" + "sup_UMA_B" + "', " + IDControllo + ")#";
    var footerTemplateStringsupFASC = "#=calcTotaleColonna('" + "sup_UMA_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringsupFASC_A = "#=calcTotaleColonna('" + "sup_UMA_A_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringsupFASC_B = "#=calcTotaleColonna('" + "sup_UMA_B_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringCalcolato = "#=calcTotaleColonna('" + "calcolato" + "', " + IDControllo + ")#";
    var footerTemplateStringRichiesto = "#=calcTotaleColonna('" + "richiesto" + "', " + IDControllo + ")#";
    var footerTemplateStringAssegnato = "#=calcTotaleColonna('" + "assegnato" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrNorm = "#=calcTotaleColonna('" + "TerrenoNormale_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrMed = "#=calcTotaleColonna('" + "TerrenoMedio_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrTen = "#=calcTotaleColonna('" + "TerrenoTenace_Edit" + "', " + IDControllo + ")#";

    let ricRend = QS_Avanzamento == 0 ? "Richiesto" : "Rendicontato";

    var colonneKendoGrid = [
        {
            field: "Programmazione_Des",
            title: "Fascicolo",
            width: 170,
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true },
            editor: Programmazione_Des_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "Gruppo_Colturale_UMA",
            title: TraduzioneMultiResx(gestioneCarbResx, "Gruppo_Colturale_UMA", "Gruppo colturale U.M.A."),
            width: 200,
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true },
            editor: MacrousoUMA_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },

        //{ field: "sup_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "sup_UMA", "Superficie dichiarata U.M.A. (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupUMA, filterable: { multi: true, search: true } },
        //{ field: "sup_UMA_A", title: TraduzioneMultiResx(gestioneCarbResx, "sup_UMA_A", "Dettaglio zona A (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupUMA_A, filterable: { multi: true, search: true } },
        //{ field: "sup_UMA_B", title: TraduzioneMultiResx(gestioneCarbResx, "sup_UMA_B", "Dettaglio zona B (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupUMA_B, filterable: { multi: true, search: true } },
        { field: "sup_UMA_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "sup_FASC", "Superficie dichiarata U.M.A. (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupFASC, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
        { field: "calcolato", title: TraduzioneMultiResx(gestioneCarbResx, "calcolato", "Carburante Calcolato (lt)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringCalcolato, format: "{0:n0}", editor: NumberEditorNoSpinInteger },
        { field: "richiesto", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto", "Carburante " + ricRend + " (lt)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringRichiesto, format: "{0:n0}", editor: NumberEditorNoSpinInteger },
        { field: "assegnato", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato", "Carburante Assegnato (lt)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringAssegnato, format: "{0:n0}", editor: NumberEditorNoSpinInteger },
        { field: "sup_UMA_A_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "sup_FASC_A", "Dettaglio zona A (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupFASC_A, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
        { field: "sup_UMA_B_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "sup_FASC_B", "Dettaglio zona B (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupFASC_B, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
    ];

    if (gestioneBiologico) {
        colonneKendoGrid.splice(2, 0,
            {
                field: "Regolamento_Check",
                //template: '<input type=\"checkbox\" # if(Regolamento_Cod > 1){ # checked #} # />',
                template: "#=(Regolamento_Cod === 4 ? 'Si' : 'No')#",
                title: "Biologico",
                width: 110,
                headerAttributes: { style: styleOut },
                attributes: { class: "k-text-center checkboxCustom edit_onInsert editBio" },
                editor: booleanEditor,
            })
    }

    var colonneTerreno = [];
    //colonneTerreno.push({ field: "TerrenoNormale", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoNormale", "Normale (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrNorm, filterable: { multi: true, search: true } });
    //colonneTerreno.push({ field: "TerrenoMedio", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoMedio", "Medio (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrMed, filterable: { multi: true, search: true } });
    //colonneTerreno.push({ field: "TerrenoTenace", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoTenace", "Tenace (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrTen, filterable: { multi: true, search: true } });
    colonneKendoGrid.push({ field: "TerrenoNormale_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoNormale", "Normale (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrNorm, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals });
    colonneKendoGrid.push({ field: "TerrenoMedio_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoMedio", "Medio (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrMed, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals });
    colonneKendoGrid.push({ field: "TerrenoTenace_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoTenace", "Tenace (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrTen, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals });

    //colonneKendoGrid.push({
    //    title: TraduzioneMultiResx(gestioneCarbResx, "TipoTerreno", "Tipo di Terreno"),
    //    headerAttributes: { style: styleOut },
    //    columns: colonneTerreno
    //});

    var parametriPerLettura = [AggiornaImpianti];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        columnMenu: true,
        pdf: false,
        pageable: { pageSizes: [100] },
        pageSize: 100,
        groupable: false,
        toolbarCommands: ["templateToolbarDettagliImpianti"],
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDetailInit: detailInitGrigliaDettagliLavorazioni,
        funzioneDaChiamareDopoDataBound: App_onDataBoundImpianti,
        funzioneDaChiamareDopoEdit: onEditGrigliaImpianti,
        funzioneDaChiamarePrimaDiEdit: onBeforeEditGrigliaImpianti,
        funzioneDaChiamarePrimaDelDataBinding: funzioneDaChiamarePrimaDelDataBinding
    };

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

    $("#" + IDControllo).data("kendoGrid").dataSource.pageSize(50);
    var grid = $("#" + IDControllo).data("kendoGrid");
    grid.unbind('cellClose');
    grid.bind("cellClose", gridImpianti_cellClose);

    if (modifica_assegnato) {
        $("#btn_AssegnaAutomaticamenteCarburante").show();
    }

    if (gestioneBiologico) {
        $("#" + IDControllo + " .k-grid-content").on("change", "input.k-checkbox", function (e) {
            //var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
            var grid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
            var row = this.closest("tr")
            var model = grid.dataItem(row)
            if (model.id === "" || model.richiesto === 0) {
                model.Regolamento_Cod = this.checked ? 4 : 1;
                model.Regolamento_Check = model.Regolamento_Cod > 1 ? true : false;
                model.dirty = true;
                grid.refresh();
            } else {
                this.checked = !this.checked;
            }
            //coloraRighe_Impianti("#tab_griglia_dettagliImpianti", e);
        });
    }

}

function booleanEditor(container, options) {
    var guid = kendo.guid();
    $('<input class="k-checkbox" id="' + guid + '" type="checkbox" name="' + options.field + '" data-type="boolean" data-bind="checked:' + options.field + '">').appendTo(container);
    $('<label class="k-checkbox-label" for="' + guid + '">&#8203;</label>').appendTo(container);
}

function NumberEditorNoSpin4Decimals(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            spinners: false,
            decimals: 4
        });
}

function NumberEditorNoSpinInteger(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            spinners: false,
            decimals: 0
        });
}

function funzioneDaChiamarePrimaDelDataBinding(e) {
    if (e.action == "add") {
        var tabelleAperte = $("#tab_griglia_dettagliImpianti").find("div[id^=GrigliaDettagliLavorazioni]");
        if (tabelleAperte.length > 0) {
            kendo.alert("Prima di aggiungere una coltura è necessario salvare la richiesta");
            e.preventDefault();
        }
    }
}



async function onBeforeEditGrigliaImpianti(e) {
    //var tabelleAperte = $("#tab_griglia_dettagliImpianti").find("div[id^=GrigliaDettagliLavorazioni]");
    //if (tabelleAperte.length > 1) {
    //    if (await kendoConfirm_Promise("Prima di aggiungere una coltura è necessario salvare la richiesta, procedere con il salvataggio?")) {
    //       AggiornaDati();
    //    }
    //}
}

function onEditGrigliaImpianti(e) {
    if ($(e.container[0]).hasClass("editBio")) {
        if ((e.container[0].childNodes[0].checked && e.model.Regolamento_Cod !== 4) ||
            (!e.container[0].childNodes[0].checked && e.model.Regolamento_Cod !== 1))
            e.container[0].childNodes[0].checked = e.model.Regolamento_Cod === 4 ? true : false
    }
    if (e.model.UMA_Cod != "" &&
        e.model.Programmazione_Cod != 0 &&
        e.model.Richiesta_Cod != 0) {
        if ($(e.container[0]).hasClass("edit_onInsert") && !(e.model.id === "")) {
            if (!(($(e.container[0]).hasClass("editBio") && e.model.richiesto === 0)))
                e.sender.closeCell();
        }
    } else {
        e.model.Richiesta_Cod = richiesta_cod;
    }
}

function App_onDataBoundImpianti(e) {
    if (QS_Avanzamento === 1) {
        LeggiNoProssimaRichiesta();
    }
    VerificaESegnalaAppezzamenti(e);
    coloraRighe_Impianti("#tab_griglia_dettagliImpianti", e);
}

function coloraRighe_Impianti(grid_elem, e) {
    var grid = $(grid_elem).data('kendoGrid');
    //var items = e.sender.items();
    //var columns = e.sender.columns;
    var indexColumnSupTot_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "sup_UMA_Edit" + "]").index();

    var indexColumnSupA_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "sup_UMA_A_Edit" + "]").index();
    var indexColumnSupB_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "sup_UMA_B_Edit" + "]").index();

    var indexColumnTerrenoNormale_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoNormale_Edit" + "]").index();
    var indexColumnTerrenoMedio_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoMedio_Edit" + "]").index();
    var indexColumnTerrenoTenace_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoTenace_Edit" + "]").index();

    var indexColumnFascicolo = grid.wrapper.find(".k-grid-header [data-field=" + "Programmazione_Des" + "]").index();
    var indexColumnMacrousoUMA = grid.wrapper.find(".k-grid-header [data-field=" + "Gruppo_Colturale_UMA" + "]").index();
    var indexColumnBiologico = grid.wrapper.find(".k-grid-header [data-field=" + "Regolamento_Check" + "]").index();

    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        FascicoloMacrousiUMADoppi(grid, dataItem, row, indexColumnFascicolo, indexColumnMacrousoUMA, indexColumnBiologico);

        if ((dataItem.Programmazione_Cod <= 0 && dataItem.Programmazione_Cod != codiceFascicoloPianoColturale) || dataItem.Programmazione_Cod == undefined) {
            row.find(".btnDettaglio").hide();
        }

        var rowAnomalia = null 
        if (anomalieSuperficiAppezzamenti != null) {
            rowAnomalia = anomalieSuperficiAppezzamenti.find((x) => x.Gruppo_Colturale_UMA_Cod == dataItem.Macrouso_UMA_Cod);
        }

        if (dataItem.sup_UMA < dataItem.sup_UMA_Edit) {
            AddErrorClass(row, indexColumnSupTot_Edit, warningCell, SuperficieOltreFascicolo + (dataItem.sup_UMA_Edit - dataItem.sup_UMA) + " ha");
        } else {
            RemoveErrorClass(row, indexColumnSupTot_Edit, warningCell);
        }

        if (dataItem.sup_UMA_A < dataItem.sup_UMA_A_Edit) {
            AddErrorClass(row, indexColumnSupA_Edit, warningCell, SuperficieOltreFascicolo + (dataItem.sup_UMA_A_Edit - dataItem.sup_UMA_A) + " ha");
        } else {
            RemoveErrorClass(row, indexColumnSupA_Edit, warningCell);
        }

        if (dataItem.sup_UMA_B < dataItem.sup_UMA_B_Edit) {
            AddErrorClass(row, indexColumnSupB_Edit, warningCell, SuperficieOltreFascicolo + (dataItem.sup_UMA_B_Edit - dataItem.sup_UMA_B) + " ha");
        } else {
            RemoveErrorClass(row, indexColumnSupB_Edit, warningCell);
        }

        if (dataItem.TerrenoNormale < dataItem.TerrenoNormale_Edit) {
            AddErrorClass(row, indexColumnTerrenoNormale_Edit, warningCell, SuperficieOltreFascicolo + (dataItem.TerrenoNormale_Edit - dataItem.TerrenoNormale) + " ha");
        } else {
            RemoveErrorClass(row, indexColumnTerrenoNormale_Edit, warningCell);
        }

        if (dataItem.TerrenoMedio < dataItem.TerrenoMedio_Edit) {
            AddErrorClass(row, indexColumnTerrenoMedio_Edit, warningCell, SuperficieOltreFascicolo + (dataItem.TerrenoMedio_Edit - dataItem.TerrenoMedio) + " ha");
        } else {
            RemoveErrorClass(row, indexColumnTerrenoMedio_Edit, warningCell);
        }

        if (dataItem.TerrenoTenace < dataItem.TerrenoTenace_Edit) {
            AddErrorClass(row, indexColumnTerrenoTenace_Edit, warningCell, SuperficieOltreFascicolo + (dataItem.TerrenoTenace_Edit - dataItem.TerrenoTenace) + " ha");
        } else {
            RemoveErrorClass(row, indexColumnTerrenoTenace_Edit, warningCell);
        }

        if (rowAnomalia != null && dataItem.sup_UMA != rowAnomalia.Totale_Superficie_Rilevata) {
            AddErrorClass(row, indexColumnSupTot_Edit, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
        } else {
            RemoveErrorClass(row, indexColumnSupTot_Edit, errorCell);
        }

        if (rowAnomalia != null && (dataItem.sup_UMA_A != rowAnomalia.Zona_Pendenza_A_Rilevata || dataItem.sup_UMA_B != rowAnomalia.Zona_Pendenza_B_Rilevata)) {
            if (dataItem.sup_UMA_A != rowAnomalia.Zona_Pendenza_A_Rilevata)
                AddErrorClass(row, indexColumnSupA_Edit, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
            if (dataItem.sup_UMA_B != rowAnomalia.Zona_Pendenza_B_Rilevata)
                AddErrorClass(row, indexColumnSupB_Edit, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
        }
        else if (dataItem.sup_UMA_Edit.toFixed(4) != parseFloat((dataItem.sup_UMA_A_Edit + dataItem.sup_UMA_B_Edit).toFixed(4))) {
            AddErrorClass(row, indexColumnSupA_Edit, errorCell, "La somma delle superfici in Zona A e Zona B è diversa dalla superficie totale di " + dataItem.sup_UMA_Edit + " ha.");
            AddErrorClass(row, indexColumnSupB_Edit, errorCell, "La somma delle superfici in Zona A e Zona B è diversa dalla superficie totale di " + dataItem.sup_UMA_Edit + " ha.");
        } else {
            RemoveErrorClass(row, indexColumnSupA_Edit, errorCell);
            RemoveErrorClass(row, indexColumnSupB_Edit, errorCell);
        }

        if (rowAnomalia != null && (dataItem.TerrenoNormale != rowAnomalia.Zona_Tessitura_Normale_Rilevata || dataItem.TerrenoMedio != rowAnomalia.Zona_Tessitura_Media_Rilevata || dataItem.TerrenoTenace != rowAnomalia.Zona_Tessitura_Tenace_Rilevata)) {
            if (dataItem.TerrenoNormale != rowAnomalia.Zona_Tessitura_Normale_Rilevata)
                AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
            if (dataItem.TerrenoMedio != rowAnomalia.Zona_Tessitura_Media_Rilevata)
                AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
            if (dataItem.TerrenoTenace != rowAnomalia.Zona_Tessitura_Tenace_Rilevata)
                AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
        }
        else if (dataItem.sup_UMA_Edit.toFixed(4) != parseFloat((dataItem.TerrenoNormale_Edit + dataItem.TerrenoMedio_Edit + dataItem.TerrenoTenace_Edit).toFixed(4))) {
            AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell, SommaVariTipi + parseFloat((dataItem.TerrenoNormale_Edit + dataItem.TerrenoMedio_Edit + dataItem.TerrenoTenace_Edit).toFixed(4)) + "ha (Normale, Medio, Tenace) è diversa dalla superficie totale di " + dataItem.sup_UMA_Edit + " ha.");
            AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell, SommaVariTipi + parseFloat((dataItem.TerrenoNormale_Edit + dataItem.TerrenoMedio_Edit + dataItem.TerrenoTenace_Edit).toFixed(4)) + "ha (Normale, Medio, Tenace) è diversa dalla superficie totale di " + dataItem.sup_UMA_Edit + " ha.");
            AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell, SommaVariTipi + parseFloat((dataItem.TerrenoNormale_Edit + dataItem.TerrenoMedio_Edit + dataItem.TerrenoTenace_Edit).toFixed(4)) + "ha (Normale, Medio, Tenace) è diversa dalla superficie totale di " + dataItem.sup_UMA_Edit + " ha.");
        } else if ((dataItem.dirtyFields.TerrenoNormale_Edit || dataItem.dirtyFields.TerrenoMedio_Edit || dataItem.dirtyFields.TerrenoTenace_Edit) && dataItem.calcolato > 0) {
            AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell, "Sono presenti una o più lavorazioni per questa coltura, non è possibile modificare la tessitura");
            AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell, "Sono presenti una o più lavorazioni per questa coltura, non è possibile modificare la tessitura");
            AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell, "Sono presenti una o più lavorazioni per questa coltura, non è possibile modificare la tessitura");
        } else {
            RemoveErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell);
            RemoveErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell);
            RemoveErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell);
        }

    }
}

async function VerificaESegnalaAppezzamenti(e) {

    if ($("#anno").val() >= 2025 && $("#stato_pratica_cod").val() == In_Compilazione.toString() && modifica_richiesto && !verificaAppezzamentiEffettuata) {
        verificaAppezzamentiEffettuata = true;

        WaitFrame.show();

        await VerificaAppezzamenti();

        if (anomalieSuperficiAppezzamenti != null && anomalieSuperficiAppezzamenti.length > 0) {
            $("#warning_appezzamenti_message").empty();
            $("#warning_appezzamenti").show();

            var warningMsg = "ATTENZIONE: sono state rilevate variazioni di (totale ettari), (pendenze), (tessiture) nel piano colturale rispetto a quanto presente in questa richiesta (rendicontazione). <br />" +
                "E' obbligatorio utilizzare il pulsante \"Allinea Tutti\" prima di poter salvare la richiesta o procedere con l'avanzamento di stato. <br />" +
                "(Le tessiture indicate in modo specifico verranno riportate a quanto risulta da piano colturale e relative analisi del terreno). <br />" +
                "Di seguito il dettaglio dei gruppi colturali su cui verranno applicati gli adeguamenti: <br /><br />";
            for (var i = 0; i < anomalieSuperficiAppezzamenti.length; i++) {                
                var rigaAnomalia = anomalieSuperficiAppezzamenti[i];
                var sep = "";
                var nuovaTessitura = false;

                var aziendaImpianto = "";
                if (QS_Type == -1 && QS_Avanzamento == 1 && dtRichiestaTerzista !== undefined && dtRichiestaTerzista !== "") {
                    let azienda = dtRichiestaTerzista.find(x => x.piva == rigaAnomalia["Piva_Impianto"]);
                    if (azienda !== undefined && azienda !== null) {
                        aziendaImpianto = "Azienda " + azienda.rag_soc + " - ";
                    }
                }

                warningMsg = warningMsg + aziendaImpianto + "Gruppo Colturale " + rigaAnomalia["Gruppo_Colturale_UMA_Des"] + ": "
                if (rigaAnomalia["Totale_Superficie_UMA"] !== rigaAnomalia["Totale_Superficie_Rilevata"]) {
                    warningMsg = warningMsg + "Superficie dichiarata U.M.A. da " + rigaAnomalia["Totale_Superficie_UMA"] + " Ha a " + rigaAnomalia["Totale_Superficie_Rilevata"] + " Ha"
                    sep = ", "
                }
                if (rigaAnomalia["Zona_Pendenza_A_UMA"] !== rigaAnomalia["Zona_Pendenza_A_Rilevata"]) {
                    warningMsg = warningMsg + sep + "Dettaglio Zona A da " + rigaAnomalia["Zona_Pendenza_A_UMA"] + " Ha a " + rigaAnomalia["Zona_Pendenza_A_Rilevata"] + " Ha"
                    sep = ", "
                }
                if (rigaAnomalia["Zona_Pendenza_B_UMA"] !== rigaAnomalia["Zona_Pendenza_B_Rilevata"]) {
                    warningMsg = warningMsg + sep + "Dettaglio Zona B da " + rigaAnomalia["Zona_Pendenza_B_UMA"] + " Ha a " + rigaAnomalia["Zona_Pendenza_B_Rilevata"] + " Ha"
                    sep = ", "
                }
                if (rigaAnomalia["Zona_Tessitura_Normale_UMA"] !== rigaAnomalia["Zona_Tessitura_Normale_Rilevata"]) {
                    warningMsg = warningMsg + sep + "Tessitura Normale da " + rigaAnomalia["Zona_Tessitura_Normale_UMA"] + " Ha a " + rigaAnomalia["Zona_Tessitura_Normale_Rilevata"] + " Ha"
                    sep = ", "
                    nuovaTessitura = true;
                }
                if (rigaAnomalia["Zona_Tessitura_Media_UMA"] !== rigaAnomalia["Zona_Tessitura_Media_Rilevata"]) {
                    warningMsg = warningMsg + sep + "Tessitura Media da " + rigaAnomalia["Zona_Tessitura_Media_UMA"] + " Ha a " + rigaAnomalia["Zona_Tessitura_Media_Rilevata"] + " Ha"
                    sep = ", "
                    nuovaTessitura = true;
                }
                if (rigaAnomalia["Zona_Tessitura_Tenace_UMA"] !== rigaAnomalia["Zona_Tessitura_Tenace_Rilevata"]) {
                    warningMsg = warningMsg + sep + "Tessitura Tenace da " + rigaAnomalia["Zona_Tessitura_Tenace_UMA"] + " Ha a " + rigaAnomalia["Zona_Tessitura_Tenace_Rilevata"] + " Ha"
                    sep = ", "
                    nuovaTessitura = true;
                }
                if (nuovaTessitura && rigaAnomalia["Zona_Tessitura_Aggiornata"]) {
                    warningMsg = warningMsg + " (le tessiture per questo gruppo sono state modificate dall'utente)"
                }
                warningMsg = warningMsg + " <br /><br />"
            }
            $("<h5 style=\"color: red; padding - bottom: 5px\">" + warningMsg + "</h5>").appendTo("#warning_appezzamenti_message");

            $("#btnCorreggiSuperfici").show();

        } else {
            $("#warning_appezzamenti_message").empty();
            $("#warning_appezzamenti").hide();
        }

        WaitFrame.hide();
    }
}

async function CorreggiSuperficiAppezzamenti_Click() {
    let res = await CorreggiSuperficiAppezzamenti();

    if (res) {

        $("#warning_appezzamenti").hide();
        $("#warning_appezzamenti_message").empty();
        $("#btnCorreggiSuperfici").hide();
        verificaAppezzamentiEffettuata = false;

        if (QS_Type == -1) {
            await LeggiRichiesteTerzista(KendoDDL("ddlAzienda").value());

            $("#tab_griglia_terzisti").data('kendoGrid').dataSource.read();
            //$("#tab_griglia_terzisti").data('kendoGrid').refresh();
        }
        else {
            await LeggiRichiesta(false);

            $("#tab_griglia_dettagliImpianti").data('kendoGrid').dataSource.read();
            //$("#tab_griglia_dettagliImpianti").data('kendoGrid').refresh();
        }

    }    
}

function AddErrorClass(row, index, errClass, content) {
    row.children().eq(index).addClass(errClass);
    $(row.children().eq(index)).kendoTooltip({
        content: content,
        position: "top"
    });
}

function RemoveErrorClass(row, index, errClass) {
    if (row.children().eq(index).hasClass(errClass)) {
        row.children().eq(index).removeClass(errClass);
    }
}

function FascicoloMacrousiUMADoppi(grid, dataItem, row, indexColumnFascicolo, indexColumnMacrousoUMA, indexColumnBiologico) {
    let data = grid.dataSource.data();

    let datiFiltrati = data.filter(el => {
        return el.Programmazione_Cod == dataItem.Programmazione_Cod &&
            el.Macrouso_UMA_Cod == dataItem.Macrouso_UMA_Cod
        //&& el.Regolamento_Cod == dataItem.Regolamento_Cod;
    });

    //--------------------------------------------------------------------------------
    // Cambio macrouso colture pluriennali fra 2023 e 2024
    //--------------------------------------------------------------------------------
    // 1006 (PRATI AVVICENDATI ANNI SUCCESSIVI)       => 1044 (PRATI AVVICENDATI)
    // 1018 (ORTIVE PLURIENNALI II ANNO E SUCCESSIVI) => 1045 (ORTIVE PLURIENNALI)
    // 1026 (TARTUFAIE I ANNO)                        => 1046 (TARTUFAIE)
    //--------------------------------------------------------------------------------

    if (datiFiltrati.length === 1 &&
        (
            dataItem.Macrouso_UMA_Cod === "1006" || dataItem.Macrouso_UMA_Cod === "1018" || dataItem.Macrouso_UMA_Cod === "1026" ||
            dataItem.Macrouso_UMA_Cod === "1044" || dataItem.Macrouso_UMA_Cod === "1045" || dataItem.Macrouso_UMA_Cod === "1046"
        )
    ) {
        if (dataItem.Macrouso_UMA_Cod === "1006") {
            macrouso2023 = "1044"
        }
        if (dataItem.Macrouso_UMA_Cod === "1018") {
            macrouso2023 = "1045"
        }
        if (dataItem.Macrouso_UMA_Cod === "1026") {
            macrouso2023 = "1046"
        }
        if (dataItem.Macrouso_UMA_Cod === "1044") {
            macrouso2023 = "1006"
        }
        if (dataItem.Macrouso_UMA_Cod === "1045") {
            macrouso2023 = "1018"
        }
        if (dataItem.Macrouso_UMA_Cod === "1046") {
            macrouso2023 = "1026"
        }
        let datiFiltrati2 = data.filter(el => {
            return el.Programmazione_Cod == dataItem.Programmazione_Cod && el.Macrouso_UMA_Cod == macrouso2023;
        });
        datiFiltrati.length += datiFiltrati2.length;
    }

    //--------------------------------------------------------------------------------

    if (datiFiltrati.length > 1) {
        row.children().eq(indexColumnFascicolo).addClass(errorCell);
        $(row.children().eq(indexColumnFascicolo)).kendoTooltip({
            content: "Non è possibile inserire Fascicolo-Gruppo colturale U.M.A più volte",
            position: "top"
        });
        row.children().eq(indexColumnMacrousoUMA).addClass(errorCell);
        $(row.children().eq(indexColumnMacrousoUMA)).kendoTooltip({
            content: "Non è possibile inserire Fascicolo-Gruppo colturale U.M.A più volte",
            position: "top"
        });
        //row.children().eq(indexColumnBiologico).addClass(errorCell);
        //$(row.children().eq(indexColumnBiologico)).kendoTooltip({
        //    content: "Non è possibile inserire Fascicolo-Gruppo colturale U.M.A-Biologico più volte",
        //    position: "top"
        //});
    }
}

function gridImpianti_cellClose(e) {
    if (e.model.dirtyFields != undefined) {
        if (e.model.dirtyFields.sup_UMA_Edit == true) {
            aggiornataSup_Totale_Edit2(e);
            e.model.dirtyFields.sup_UMA_Edit = false;
            $("#tab_griglia_dettagliImpianti").data('kendoGrid').refresh();
        } else if (e.model.dirtyFields.nLavRichieste == true) {
            e.model.dirtyFields.nLavRichieste = false;
        }
        /*if (e.model.dirtyFields.sup_UMA_A_Edit == true || e.model.dirtyFields.sup_UMA_B_Edit == true) {
        if (e.model.dirtyFields.sup_UMA_A_Edit == true) {
            if (e.model.sup_UMA_A_Edit < 0 || e.model.sup_UMA_A_Edit == null) { e.model.sup_UMA_A_Edit = 0; }
            e.model.sup_UMA_B_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.sup_UMA_A_Edit).toFixed(4));
        } else if (e.model.dirtyFields.sup_UMA_B_Edit == true) {
            if (e.model.sup_UMA_B_Edit < 0 || e.model.sup_UMA_B_Edit == null) { e.model.sup_UMA_B_Edit = 0; }
            e.model.sup_UMA_A_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.sup_UMA_B_Edit).toFixed(4));
            if (e.model.sup_UMA_A_Edit < 0 || e.model.sup_UMA_A_Edit == null) { e.model.sup_UMA_A_Edit = 0; }
        }
        e.model.dirtyFields.sup_UMA_A_Edit = false;
        e.model.dirtyFields.sup_UMA_B_Edit = false;
    } else if (e.model.dirtyFields.TerrenoNormale_Edit == true || e.model.dirtyFields.TerrenoMedio_Edit == true || e.model.dirtyFields.TerrenoTenace_Edit == true) {
        //e.model.sup_UMA_Edit = e.model.TerrenoNormale_Edit + e.model.TerrenoMedio_Edit + e.model.TerrenoTenace_Edit
        if (e.model.dirtyFields.TerrenoNormale_Edit == true) {
            if (e.model.TerrenoNormale_Edit < 0 || e.model.TerrenoNormale_Edit == null) { e.model.TerrenoNormale_Edit = 0; }
            if (e.model.TerrenoMedio_Edit != 0 && e.model.TerrenoTenace_Edit == 0) {
                e.model.TerrenoMedio_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.TerrenoNormale_Edit).toFixed(4));
            } else if (e.model.TerrenoMedio_Edit == 0 && e.model.TerrenoTenace_Edit != 0) {
                e.model.TerrenoTenace_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.TerrenoNormale_Edit).toFixed(4));
            }
        } else if (e.model.dirtyFields.TerrenoMedio_Edit == true) {
            if (e.model.TerrenoMedio_Edit < 0 || e.model.TerrenoMedio_Edit == null) { e.model.TerrenoMedio_Edit = 0; }
            if (e.model.TerrenoNormale_Edit != 0 && e.model.TerrenoTenace_Edit == 0) {
                e.model.TerrenoNormale_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.TerrenoMedio_Edit).toFixed(4));
            } else if (e.model.TerrenoNormale_Edit == 0 && e.model.TerrenoTenace_Edit != 0) {
                e.model.TerrenoTenace_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.TerrenoMedio_Edit).toFixed(4));
            }
        } else {
            if (e.model.TerrenoTenace_Edit < 0 || e.model.TerrenoTenace_Edit == null) { e.model.TerrenoTenace_Edit = 0; }
            if (e.model.TerrenoNormale_Edit != 0 && e.model.TerrenoMedio_Edit == 0) {
                e.model.TerrenoNormale_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.TerrenoTenace_Edit).toFixed(4));
            } else if (e.model.TerrenoNormale_Edit == 0 && e.model.TerrenoMedio_Edit != 0) {
                e.model.TerrenoMedio_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.TerrenoTenace_Edit).toFixed(4));
            }
        }
        e.model.dirtyFields.TerrenoNormale_Edit = false;
        e.model.dirtyFields.TerrenoMedio_Edit = false;
        e.model.dirtyFields.TerrenoTenace_Edit = false;
    }*/
        if (e.model.sup_UMA_A_Edit < 0 || e.model.sup_UMA_A_Edit == null) { e.model.sup_UMA_A_Edit = 0; }
        if (e.model.sup_UMA_B_Edit < 0 || e.model.sup_UMA_B_Edit == null) { e.model.sup_UMA_B_Edit = 0; }
        if (e.model.TerrenoNormale_Edit < 0 || e.model.TerrenoNormale_Edit == null) { e.model.TerrenoNormale_Edit = 0; }
        if (e.model.TerrenoMedio_Edit < 0 || e.model.TerrenoMedio_Edit == null) { e.model.TerrenoMedio_Edit = 0; }
        if (e.model.TerrenoTenace_Edit < 0 || e.model.TerrenoTenace_Edit == null) { e.model.TerrenoTenace_Edit = 0; }
        //aggiornataSup_Totale_Edit(e);
        $("#tab_griglia_dettagliImpianti").data('kendoGrid').refresh();
    }
}

function aggiornataSup_Totale_Edit(e) {
    if (e.model.sup_UMA_Edit != (e.model.sup_UMA_A_Edit + e.model.sup_UMA_B_Edit)) {
        if (e.model.sup_UMA_A_Edit == 0) {
            e.model.sup_UMA_B_Edit = e.model.sup_UMA_Edit;
        } else if (e.model.sup_UMA_B_Edit == 0) {
            e.model.sup_UMA_A_Edit = e.model.sup_UMA_Edit;
        } else {
            //e.model.sup_UMA_B_Edit = parseFloat(((e.model.sup_UMA_Edit * e.model.sup_UMA_B) / e.model.sup_UMA).toFixed(4));
            //e.model.sup_UMA_A_Edit = parseFloat((e.model.sup_UMA_Edit - e.model.sup_UMA_B_Edit).toFixed(4));
        }
    }

    if (e.model.sup_UMA_Edit != (e.model.TerrenoNormale_Edit + e.model.TerrenoMedio_Edit + e.model.TerrenoTenace_Edit)) {
        if (e.model.TerrenoNormale_Edit == 0 && e.model.TerrenoMedio_Edit == 0) {
            e.model.TerrenoTenace_Edit = e.model.sup_UMA_Edit;
        } else if (e.model.TerrenoMedio_Edit == 0 && e.model.TerrenoTenace_Edit == 0) {
            e.model.TerrenoNormale_Edit = e.model.sup_UMA_Edit;
        } else if (e.model.TerrenoNormale_Edit == 0 && e.model.TerrenoTenace_Edit == 0) {
            e.model.TerrenoMedio_Edit = e.model.sup_UMA_Edit;
        } else {
            //if (e.model.TerrenoNormale_Edit == 0 || e.model.TerrenoMedio_Edit == 0 || e.model.TerrenoTenace_Edit == 0) {
            //    if (e.model.TerrenoNormale_Edit == 0) {
            //        e.model.TerrenoTenace_Edit = parseFloat(((e.model.sup_UMA_Edit * e.model.TerrenoTenace) / e.model.sup_UMA).toFixed(4));
            //        e.model.TerrenoMedio_Edit = parseFloat((e.model.sup_UMA_Edit - (e.model.TerrenoTenace_Edit + e.model.TerrenoNormale_Edit)).toFixed(4));
            //    } else if (e.model.TerrenoMedio_Edit == 0) {
            //        e.model.TerrenoTenace_Edit = parseFloat(((e.model.sup_UMA_Edit * e.model.TerrenoTenace) / e.model.sup_UMA).toFixed(4));
            //        e.model.TerrenoNormale_Edit = parseFloat((e.model.sup_UMA_Edit - (e.model.TerrenoTenace_Edit + e.model.TerrenoMedio_Edit)).toFixed(4));
            //    } else if (e.model.TerrenoTenace_Edit == 0) {
            //        e.model.TerrenoMedio_Edit = parseFloat(((e.model.sup_UMA_Edit * e.model.TerrenoMedio) / e.model.sup_UMA).toFixed(4));
            //        e.model.TerrenoNormale_Edit = parseFloat((e.model.sup_UMA_Edit - (e.model.TerrenoTenace_Edit + e.model.TerrenoMedio_Edit)).toFixed(4));
            //    }
            //} else {
            //    e.model.TerrenoTenace_Edit = parseFloat(((e.model.sup_UMA_Edit * e.model.TerrenoTenace) / e.model.sup_UMA).toFixed(4));
            //    e.model.TerrenoMedio_Edit = parseFloat(((e.model.sup_UMA_Edit * e.model.TerrenoMedio) / e.model.sup_UMA).toFixed(4));
            //    e.model.TerrenoNormale_Edit = parseFloat((e.model.sup_UMA_Edit - (e.model.TerrenoTenace_Edit + e.model.TerrenoMedio_Edit)).toFixed(4));
            //}
        }
    }

}

function eliminaRigaImpianto(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    kendo.confirm("Sei sicuro di voler eliminare l'elemento selezionato?")
        .done(() => {
            datiGriglia.dataSource.remove(datiRiga);
            //datiGriglia.dataSource.sync();
            datiGriglia.refresh();
        })
        .fail(() => { return; });

}

//function correggiSuperficieRigaImpianto(tr_elem, grid_elem) {
//    var datiGriglia = $(grid_elem).data('kendoGrid');
//    var datiRiga = datiGriglia.dataItem(tr_elem);

//    CorreggiSuperficiAppezzamenti(datiRiga.Macrouso_UMA_Cod, true);
//}

//calcola il totale di una colonna da mettere nel footer della stessa
function calcTotaleColonna(field, idGriglia, stringReturn = true, carburante = 0, fractDigit = 0) {

    if (field !== undefined) {

        var griglia = "#" + idGriglia["id"];
        var grid = $(griglia).data("kendoGrid");
        var dataSource = grid.dataSource;

        var filteredDataSource = new kendo.data.DataSource({
            data: dataSource.data(),
            filter: dataSource.filter()
        });

        filteredDataSource.read();
        var datiFiltrati = filteredDataSource.view();

        var totale = 0;

        $.each(datiFiltrati, function (index, model) {
            if (model.get(field) !== undefined && model.get(field) !== null && (model.deleted === undefined || model.deleted === false) && carburante == 0 ? true : model.get("Car_Cod") == carburante) {
                totale += parseFloat(model.get(field));
            }
        });

        return stringReturn ? totale.toLocaleString("it-IT", { minimumFractionDigits: 0, maximumFractionDigits: fractDigit }) : totale.toFixed(fractDigit).replace(".", ",");
    }
}

async function InsertLavorazione(e) {

    //if ($("#tab_griglia_dettagliImpianti").find(".errorCell").length != 0) {
    //    kendo.alert("Verificare i dati segnalati prima di salvare");
    //    return false;
    //}

    WaitFrame.show();

    var data = $("#tab_griglia_dettagliImpianti").data("kendoGrid").dataSource.data();

    await AggiornaRichieste_UMA(pivaSelezionata, richiesta_cod, JSON.stringify(data));

    let modificheFatte = false;
    let Richiesta_Cod = 0;
    let Macrouso_UMA_Cod = "";
    let Programmazione_Cod = 0;
    if (e.data.created.length > 0) {
        modificheFatte = true;
        //Richiesta_Cod = e.data.created[0].Richiesta_Cod;
        Macrouso_UMA_Cod = e.data.created[0].Macrouso_UMA_Cod;
        Programmazione_Cod = e.data.created[0].Programmazione_Cod;
    }
    if (e.data.updated.length > 0) {
        modificheFatte = true;
        //Richiesta_Cod = e.data.updated[0].Richiesta_Cod;
        Macrouso_UMA_Cod = e.data.updated[0].Macrouso_UMA_Cod;
        Programmazione_Cod = e.data.updated[0].Programmazione_Cod;
    }
    if (e.data.destroyed.length > 0) {
        modificheFatte = true;
        //Richiesta_Cod = e.data.destroyed[0].Richiesta_Cod;
        Macrouso_UMA_Cod = e.data.destroyed[0].Macrouso_UMA_Cod;
        Programmazione_Cod = e.data.destroyed[0].Programmazione_Cod;
    }
    if (modificheFatte) {
        await ws_Inserisci_Lavorazioni(pivaSelezionata, richiesta_cod, Programmazione_Cod, Macrouso_UMA_Cod, e.data.created, e.data.updated, e.data.destroyed);
        await LeggiRichiesta(false);
        ConfiguraGrigliaDettagliImpianti("tab_griglia_dettagliImpianti", false);
    }

    await LeggiRichiesta(false);
    ConfiguraGrigliaDettagliImpianti("tab_griglia_dettagliImpianti", false);

    WaitFrame.hide();

}

async function detailInitGrigliaDettagliLavorazioni(e) {
    var AggiornaImpianti = true;
    var id_macrouso = e.data.UMA_Cod;
    //var macrouso_UMA = parseInt(e.data.Macrouso_UMA_Cod);
    var Programmazione_Cod = e.data.Programmazione_Cod;
    var Programmazione_CodStr = e.data.Programmazione_Cod;
    var Regolamento_Cod = e.data.Regolamento_Cod;
    if (Programmazione_Cod == codiceFascicoloColtureNonImputabili) {
        Programmazione_CodStr = "_1";
    }
    if (Programmazione_Cod == codiceFascicoloAnticipi) {
        Programmazione_CodStr = "_2";
    }
    if (Programmazione_Cod == codiceFascicoloTrasferimenti) {
        Programmazione_CodStr = "_3";
    }
    if (Programmazione_Cod == codiceFascicoloPianoColturale) {
        Programmazione_CodStr = "_4";
    }
    var id_div = "GrigliaDettagliLavorazioni_" + id_macrouso + "_" + Programmazione_CodStr + "_" + Regolamento_Cod;
    await LeggiLavorazioniAlternative(id_macrouso, Regolamento_Cod);
    /*await LeggiLavorazioniAlternativeLimitate(macrouso_UMA);

    if (lav_alt_lim_sup.length > 0) {
        if (lav_alt_lim[macrouso_UMA] == undefined) lav_alt_lim[macrouso_UMA] = {}
        for (var n = 0; n < lav_alt_lim_sup.length; n++) {
            var x = lav_alt_lim_sup[n]["Lavorazione_UMA"];
            lav_alt_lim[macrouso_UMA][x] = 0;
        }
    }*/
    if (QS_Type == 0 /*&& QS_Avanzamento == 1*/) {
        if (lav_incrociati[id_macrouso] == undefined) {
            lav_incrociati[id_macrouso] = await controlloIncrociato(KendoDDL("ddlAzienda").value(), Programmazione_Cod, id_macrouso, $('#anno')[0].value);
        }
    }
    $("<div id='" + id_div + "' />").appendTo(e.detailCell);
    ElencoTabAperte.push(id_macrouso);
    PopolaElencoLavUMA(false, parseInt(e.data.Macrouso_UMA_Cod), 0, Regolamento_Cod);
    popolaGrigliaDettagliLavorazioni(id_div, AggiornaImpianti, id_macrouso, Programmazione_Cod, Regolamento_Cod);
}

//griglia innestata alla principale
function popolaGrigliaDettagliLavorazioni(IDControllo, AggiornaImpianti, id_gruppo, Programmazione_Cod, Regolamento_Cod) {

    var funzioneSubmit = {};

    funzioneSubmit = {
        funzione: InsertLavorazione,
        flagInsert: !richiestaRinuncia && modifica_richiesto,
        flagUpdate: !richiestaRinuncia,
        flagDelete: modifica_richiesto
    }

    var omettiAnnulla = false;
    var omettiSalva = true;
    let mesiVisibili = false;


    //Coltivazioni sotto serra
    if (id_gruppo == 1034) {
        mesiVisibili = true;
    }
    gruppo_col = id_gruppo;
    prog_cod = Programmazione_Cod;
    var funzioniCRUD = {
        funzioneRead: RicercaLavorazioni,
        funzioneSubmit: funzioneSubmit,
        //UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val().toLowerCase() == (!richiestaRinuncia).toString().toLocaleLowerCase(),
        //UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val().toLowerCase() == (!richiestaRinuncia).toString().toLocaleLowerCase(),
        //UtenteAbilitatoInserimentoModifica: modifica_richiesto || modifica_assegnato,
        //UtenteAbilitatoCancellazione: modifica_richiesto || modifica_assegnato,
        UtenteAbilitatoInserimentoModifica: !richiestaRinuncia && (modifica_richiesto || modifica_assegnato),
        UtenteAbilitatoCancellazione: !richiestaRinuncia && (modifica_richiesto),
        omettiPulsantiSalva: omettiSalva,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var styleInt = "background-color: deepskyblue; vertical-align: top";
    var idModel = "richiestaDettaglioCod";
    var campiKendoModel = {
        richiestaDettaglioCod: { editable: false, type: "number" },
        Richiesta_Cod: { editable: false, type: "number" },
        Piva: { editable: false, type: "string" },
        Macrouso_UMA_Cod: { editable: false, type: "string" },
        Programmazione_Cod: { editable: false, type: "number" },
        LavUMA: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        LavGIAS: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        LAV_COD: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Attivita_Cod: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Attivita_Des: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        TipoCarb: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        SupMaggiorazioneTrasferimenti: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        fabbisognoCalc: { editable: false, type: "number", validation: { required: true } },
        ltrichiesto: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        ltAssegnato: { editable: !richiestaRinuncia && modifica_assegnato, type: "number", validation: { required: true } },
        nLavPreviste: { editable: false, type: "number", validation: { required: true } },
        nLavRichieste: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        piuLavPreviste: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        piuRaccoltiPrevisti: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Superficie_Trattata: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        //Sup_A: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        //Sup_B: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Sup_A: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Sup_B: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoNormale: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoMedio: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoTenace: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        //TerrenoNormale: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        //TerrenoMedio: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        //TerrenoTenace: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },

        Note_Compilatore: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        Note_Approvatore: { editable: !richiestaRinuncia && modifica_assegnato, type: "string" },
        Udm_Alt: { editable: false, type: "string" },
        Qta_Manuale: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Mesi: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Validita_Inizio: { editable: !richiestaRinuncia && modifica_richiesto, type: "date" },
        CUAA: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        rag_soc: { editable: false, type: "string" },
    }

    var footerTemplateStringRichiesto = "#=calcTotaleColonna('" + "ltrichiesto" + "', " + IDControllo + ")#";
    var footerTemplateStringAssegnato = "#=calcTotaleColonna('" + "ltAssegnato" + "', " + IDControllo + ")#";
    var colonneKendoGrid = [
        { field: "LavUMA", width: "250px", title: TraduzioneMultiResx(gestioneCarbResx, "LavUMA", "Lavorazione U.M.A."), headerAttributes: { style: styleInt }, filterable: { multi: true, search: true }, editor: lavUMA_DropDownEditor },
        { field: "LavGIAS", width: "200px", title: TraduzioneMultiResx(gestioneCarbResx, "LavGIAS", "Lavorazione"), headerAttributes: { style: styleInt }, filterable: { multi: true, search: true }, editor: lavGIAS_DropDownEditor, hidden: true },
        { field: "Attivita_Des", width: "200px", title: "Attività", headerAttributes: { style: styleInt }, filterable: { multi: true, search: true }, editor: AttivitaGIAS_DropDownEditor, hidden: true },
        { field: "TipoCarb", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "TipoCarb", "Tipo Carburante"), headerAttributes: { style: styleInt }, filterable: { multi: true, search: true }, editor: Carburanti_DropDownEditor },
        { field: "Qta_Manuale", width: "100px", title: "Qta. Manuale", headerAttributes: { style: styleInt }, attributes: { class: "Lav_Alt" }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
        // { field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA Azienda"), width: 150, headerAttributes: { style: styleInt }, filterable: { multi: true, search: true, } },
        //{ field: "rag_soc", title: TraduzioneMultiResx(gestioneCarbResx, "rag_soc", "Azienda"), width: 200, headerAttributes: { style: styleInt }, filterable: { multi: true, search: true }, editor: azienda_DropDownEditor },

        //{ field: "Udm_Alt", width: "100px", title: "UdM", headerAttributes: { style: styleInt } }
    ];

    if (mesiVisibili) {
        colonneKendoGrid.push({
            field: "Mesi", width: "100px", title: "Mesi", headerAttributes: { style: styleInt }, attributes: { class: "Lav_Alt" }
        });
    }

    colonneKendoGrid.push({
        field: "SupMaggiorazioneTrasferimenti",
        width: "100px",
        title: TraduzioneMultiResx(gestioneCarbResx, "SupMaggiorazioneTrasferimenti", "Superficie Maggiorazione Trasferimenti"),
        headerAttributes: { style: styleInt },
        filterable: { multi: true, search: true },
        editor: Maggiorazione_DropDownEditor,
        template: '#= (SupMaggiorazioneTrasferimenti == 1) ? "Sì" : "No" #'
    });
    colonneKendoGrid.push({ field: "fabbisognoCalc", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "fabbisognoCalc", "Fabbisogno Calcolato (lt.)"), headerAttributes: { style: styleInt }, format: "{0:n0}", editor: NumberEditorNoSpinInteger });
    colonneKendoGrid.push({ field: "ltrichiesto", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto", "Richiesto (lt.)"), footerTemplate: footerTemplateStringRichiesto, headerAttributes: { style: styleInt }, format: "{0:n0}", editor: NumberEditorNoSpinInteger });
    colonneKendoGrid.push({ field: "ltAssegnato", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "ltAssegnato", "Assegnato (lt.)"), footerTemplate: footerTemplateStringAssegnato, headerAttributes: { style: styleInt }, format: "{0:n0}", editor: NumberEditorNoSpinInteger });
    //colonneKendoGrid.push({ field: "nLavPreviste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "nLavPreviste", "Numero Lavorazioni Previste"), headerAttributes: { style: styleInt }, editor: NumberEditorNoSpinInteger });
    //colonneKendoGrid.push({ field: "nLavRichieste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "nLavRichieste", "Numero Lavorazioni Richieste"), headerAttributes: { style: styleInt }, editor: NumberEditorNoSpinInteger });
    //colonneKendoGrid.push({ field: "piuLavPreviste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "piuLavPreviste", "Piu Lavorazioni Previste"), headerAttributes: { style: styleInt }, editor: NumberEditorNoSpinInteger });

    if (!mesiVisibili) {
        //colonneKendoGrid.push({ field: "piuRaccoltiPrevisti", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "piuRaccoltiPrevisti", "Piu Raccolti Previsti"), headerAttributes: { style: styleInt }, editor: NumberEditorNoSpinInteger });
        colonneKendoGrid.push({ field: "Superficie_Trattata", width: "72px", title: "Sup Tot", headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals, attributes: { class: "Lav_Norm" } });
        colonneKendoGrid.push({ field: "Sup_A", width: "72px", title: "Sup A", headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals, attributes: { class: "Lav_Norm" } });
        colonneKendoGrid.push({ field: "Sup_B", width: "72px", title: "Sup B", headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals, attributes: { class: "Lav_Norm" } });
        colonneKendoGrid.push({ field: "TerrenoNormale", width: "72px", title: "Terreno Normale", headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals, attributes: { class: "Lav_Norm" } });
        colonneKendoGrid.push({ field: "TerrenoMedio", width: "72px", title: "Terreno Medio", headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals, attributes: { class: "Lav_Norm" } });
        colonneKendoGrid.push({ field: "TerrenoTenace", width: "72px", title: "Terreno Tenace", headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals, attributes: { class: "Lav_Norm" } });
    }

    if (QS_Avanzamento == 1) {
        colonneKendoGrid.push({
            field: "Validita_Inizio",
            width: "92px",
            title: "Data",
            attributes: { class: "data" },
            headerAttributes: { style: styleInt },
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #'
        });
    }

    colonneKendoGrid.push({ field: "Note_Compilatore", width: "100px", title: "Note Compilatore", headerAttributes: { style: styleInt }, filterable: false });
    colonneKendoGrid.push({ field: "Note_Approvatore", width: "100px", title: "Note Approvatore", headerAttributes: { style: styleInt }, filterable: false });

    if (QS_Avanzamento == 1) {
        colonneKendoGrid.push({ field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA Azienda"), width: 150, headerAttributes: { style: styleInt }, filterable: { multi: true, search: true, } });
        colonneKendoGrid.push({ field: "rag_soc", title: TraduzioneMultiResx(gestioneCarbResx, "rag_soc", "Azienda"), width: 200, headerAttributes: { style: styleInt }, filterable: { multi: true, search: true } });
    }

    var colonneCustomKendoGrid = new Array();
    if (!richiestaRinuncia && (modifica_richiesto)) {
        colonneCustomKendoGrid.push({
            command: {
                template: "<div class='btn btn-info btnInfo btnDettaglio' style='width:25px;border:0px;' onclick=LavorazioniMultiple(this.closest('tr'),this.closest('.k-grid'))><span class='fa fa-plus lampeggiante'></span></div>"
            },
            title: "Inserisci Multiple", width: "68px", headerAttributes: { style: styleInt }
        });
    }

    var parametriKendoGrid = {
        excel: true, pdf: false,
        groupable: false,
        headerAttributes: { style: "background-color: DeepSkyBlue" },
        columnMenu: false,
        filterable: false,
        lockCancella: false,
        pageable: false,//{ pageSizes: [5, 10, 20, 50, 100] },
        btnEliminaTuttiFiltri: false,
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };

    var parametriPerLettura = [id_gruppo];
    var parametriDataSource = {};

    var funzioniPrimaDopoEventi = {
        //funzioneDaChiamareDopoSave: InsertLavorazione,
        //funzioneDaChiamareDopoEdit: InsertLavorazione,
        //funzioneDaChiamareDopoDelete: InsertLavorazione
        funzioneDaChiamareDopoDataBound: App_onDataBoundLavorazioni,
        funzioneDaChiamareDopoEdit: onEditGrigliaDettagliLavorazioni,
        funzioneDaChiamareDopoDelete: funzioneDaChiamareDopoDelete_Lavorazioni
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["LavUMA", "LavGIAS", "TipoCarb", "SupMaggiorazioneTrasferimenti", "Sup_A", "Sup_B"];

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

    var grid = $("#" + IDControllo).data("kendoGrid");
    grid.bind("cellClose", grid_cellClose);

}

function App_onDataBoundLavorazioni(e) {
    coloraRighe_Lavorazioni("#" + e.sender.element[0].id, e);
}

function coloraRighe_Lavorazioni(grid_elem, e) {
    var grid = $(grid_elem).data('kendoGrid');
    var items = e.sender.items();
    var columns = e.sender.columns;

    var indexColumnSupTot_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "Superficie_Trattata" + "]").index();

    var indexColumnSupA_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "Sup_A" + "]").index();
    var indexColumnSupB_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "Sup_B" + "]").index();

    var indexColumnTerrenoNormale_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoNormale" + "]").index();
    var indexColumnTerrenoMedio_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoMedio" + "]").index();
    var indexColumnTerrenoTenace_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoTenace" + "]").index();

    var indexColumnData = grid.wrapper.find(".k-grid-header [data-field=" + "Validita_Inizio" + "]").index();
    var indexColumnCUAA = grid.wrapper.find(".k-grid-header [data-field=" + "CUAA" + "]").index();
    var indexColumnRichiesto = grid.wrapper.find(".k-grid-header [data-field=" + "ltrichiesto" + "]").index();

    var indexColumnNoteComp = grid.wrapper.find(".k-grid-header [data-field=" + "Note_Compilatore" + "]").index();

    var parentRow = e.sender.element.parents(".k-detail-row").prev();
    var parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentItem = parentGrid.dataItem(parentRow);

    var indexColumnLav = grid.wrapper.find(".k-grid-header [data-field=" + "LavUMA" + "]").index();
    var indexChanged = false;

    var indexColumnQtaManuale = grid.wrapper.find(".k-grid-header [data-field=" + "Qta_Manuale" + "]").index();
    lav_NO[parentItem.Macrouso_UMA_Cod] = {};
    var lav_Alt_Spec = lav_alt[parentItem.Macrouso_UMA_Cod];
    var lav_NO_Spec = {};

    var parentArray = [];
    var totRichiestoIrrigazioni = 0;
    var minCoeff = 100;
    parentArray.push(parentItem.sup_tot == undefined ? parentItem.sup_UMA : parentItem.sup_tot);
    parentArray.push(parentItem.supA_Edit == undefined ? parentItem.sup_UMA_A : parentItem.supA_Edit);
    parentArray.push(parentItem.supB_Edit == undefined ? parentItem.sup_UMA_B : parentItem.supB_Edit);
    parentArray.push(parentItem.tessitura_Norm_Edit == undefined ? parentItem.TerrenoNormale : parentItem.tessitura_Norm_Edit);
    parentArray.push(parentItem.tessitura_Media_Edit == undefined ? parentItem.TerrenoMedio : parentItem.tessitura_Media_Edit);
    parentArray.push(parentItem.tessitura_Tenace_Edit == undefined ? parentItem.TerrenoTenace : parentItem.tessitura_Tenace_Edit);

    totRichiestoIrrigazioni = leggiTotaleRichiestoIrrigazioni(null);

    var rows = e.sender.tbody.children();
    for (var j = rows.length - 1; j >= 0; j--) {

        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        //let dato = QS_Avanzamento == 1 ? Date.parse(dataItem.Validita_Inizio) : Date.parse(dataPassaggioDiStato);

        let dato = 0;
        if (QS_Avanzamento == 1) {
            dato = Date.parse(dataItem.Validita_Inizio);
        } else {
            let anno = parseInt($("#anno").val());
            // testUMA da configurazione siti per collaudi su anno successivo
            let testUMA_abilitato = false;
            let testUMA_mese = 0;
            if ($(cTestUMA_Abilitato).val() === '1') {
                testUMA_abilitato = true;
                testUMA_mese = parseInt($(cTestUMA_Mese).val()) - 1;
            }
            let dataInizioValiditaPratica = new Date(anno, 0, 1);
            let testUMA_dataPassaggioDiStato = new Date(anno, testUMA_mese, 1);
            if (testUMA_abilitato === true && Date.parse(dataInizioValiditaPratica) > Date.parse(dataPassaggioDiStato)) {
                dato = Date.parse(testUMA_dataPassaggioDiStato);
            } else {
                dato = Date.parse(dataPassaggioDiStato);
            }
        }

        if (QS_Avanzamento == 1) {
            if (dataItem.dirtyFields.Validita_Inizio == true && dataItem.Validita_Inizio != "") {
                if (parseInt($("#TxtRimanenza_Gasolio_prec").val()) + parseInt($("#TxtRimanenza_Benzina_prec").val()) + parseInt($("#TxtRimanenza_Gasolio_Serra_prec").val()) == 0) {

                    //if (dataItem.Validita_Inizio < dataPrimoAcquisto) {
                    //    AddErrorClass(row, indexColumnData, errorCell, dataPrimoAcquisto.getFullYear() == 2100 ?
                    //        "Non è stato registrato alcun acquisto di carburante al momento" :
                    //        "Data antecedente a " + dataPrimoAcquisto.toLocaleDateString() + " (primo acquisto di carburante dell'anno)");
                    //} else {
                    //    RemoveErrorClass(row, indexColumnData, errorCell)
                    //}

                } else {
                    let fDate = new Date(dataItem.Validita_Inizio);
                    if (dataItem.Validita_Inizio < new Date(new Date(fDate.setDate(1)).setMonth(0))) {
                        AddErrorClass(row, indexColumnData, errorCell, "Anno diverso da quello corrente");
                    } else {
                        RemoveErrorClass(row, indexColumnData, errorCell)
                    }
                }
            } else if (dataItem.richiestaDettaglioCod == 0 && !(dataItem.dirtyFields.Validita_Inizio == true && dataItem.Validita_Inizio != "")) {
                AddErrorClass(row, indexColumnData, "data", "");
                AddErrorClass(row, indexColumnData, errorCell, "È necessario specificare una data all'inserimento di una lavorazione");
            } else {
                RemoveErrorClass(row, indexColumnData, errorCell)
                RemoveErrorClass(row, indexColumnData, "data")
            }
        }

        if (dataItem.deleted || (dataItem.Programmazione_Cod < 0 && dataItem.Programmazione_Cod != -4)) {
            continue;
        }

        //if (QS_Avanzamento === 0 && QS_Type === 0) {
        //    if (dataItem.LavUMA.toLowerCase().includes("irrigazione")) { 
        //        if (($("#permessoAcqua").val() === "0" || $("#permessoAcqua").val() === null || $("#permessoAcqua").val() === undefined) || (
        //            $("#notePermessoAcqua").val() === "" || $("#notePermessoAcqua").val() === null || $("#notePermessoAcqua").val() === undefined))
        //            AddErrorClass(row, indexColumnLav, errorCell, volumeAcqua)
        //        else
        //            RemoveErrorClass(row, indexColumnLav, volumeAcqua)
        //        totRichiestoIrrigazioni = totRichiestoIrrigazioni + parseFloat(dataItem.ltrichiesto)
        //        let costo = filtraTabellaCalcoloCosti(parentItem.Macrouso_UMA_Cod, dataItem.Lav_UMA_Cod, null, dataItem.Attivita_Cod, parentItem.Regolamento_Cod);
        //        if (costo.Coeff_acq_distr < minCoeff) 
        //            minCoeff = costo.Coeff_acq_distr
        //        if (parseFloat($("#permessoAcqua").val()) * minCoeff < totRichiestoIrrigazioni) 
        //            AddErrorClass(row, indexColumnLav, errorCell, volumeAcqua)
        //        else 
        //            RemoveErrorClass(row, indexColumnLav, volumeAcqua)
        //    }
        //}

        if ((($("#stato_pratica_cod").val() == Verifica_In_Corso.toString() && dataItem.ltAssegnato == 0) || ($("#stato_pratica_cod").val() == In_Compilazione && dataItem.ltrichiesto === 0)) && (dataItem.Car_Cod != 9 && dataItem.Car_Cod != 10)) {
            continue;
        }

        //if (dataItem.CUAA == "") {            //aggiunto da gloria per non far eseguire i controlli nel caso in cui il cuaa delle lavorazioni non è valorizzato
        //    continue;
        //}

        indexChanged = CheckLavorazioniAlternative(row, parentItem, dataItem, indexColumnLav, lav_Alt_Spec, lav_NO_Spec, indexChanged);
        CheckMaxUdm_Alt(row, parentItem, dataItem, indexColumnQtaManuale);
        indexChanged = CheckMaxNum_Op(row, parentItem, dataItem, grid, indexColumnLav, indexChanged);

        checkNoteObbligatorie(row, dataItem, indexColumnNoteComp)

        let lavValiditaFiltrato = $.grep(elencoLavValidita, function (e) { return e.Macrouso_UMA_Cod == dataItem.Macrouso_UMA_Cod && e.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && (e.Regolamento_Cod == parentItem.Regolamento_Cod || e.Regolamento_Cod == RegolamentoEntrambi); });

        if (lavValiditaFiltrato.length > 0 && dataItem.Lav_UMA_Cod != "") {
            if (!(!isNaN(dato) && Date.parse(lavValiditaFiltrato[0].Validita_Inizio) <= dato && Date.parse(lavValiditaFiltrato[0].Validita_Fine) >= dato)) {
                if (!isNaN(dato)) {
                    AddErrorClass(row, indexColumnLav, errorCell, "Lavorazione non più valida" + (QS_Avanzamento == 1 ? " alla data inserita" : ""));
                    indexChanged = true;
                }
            }
            else
                if (!indexChanged)
                    RemoveErrorClass(row, indexColumnLav, errorCell);
        }

        if (dataItem.CUAA != '' && dataItem.CUAA != undefined &&
            verifica_terzisti[dataItem.Macrouso_UMA_Cod] != undefined && verifica_terzisti[dataItem.Macrouso_UMA_Cod][dataItem.LavUMA] == dataItem.CUAA) {
            AddErrorClass(row, indexColumnCUAA, errorCell, "Il terzista non ha inserito questa lavorazione nella sua rendicontazione");
        } else
            RemoveErrorClass(row, indexColumnCUAA, errorCell);

        if (parseFloat(dataItem.ltrichiesto.toFixed(4)) > parseFloat(dataItem.fabbisognoCalc.toFixed(4))) {
            AddErrorClass(row, indexColumnRichiesto, errorCell, "Il quantitativo di carburante richiesto non può superare quello calcolato");
        } else {
            RemoveErrorClass(row, indexColumnSupTot_Edit, errorCell);
        }

        checkErroriLavorazioni(parentItem.sup_UMA_Edit, dataItem.Superficie_Trattata, row, indexColumnSupTot_Edit, SuperficieOltre)

        checkErroriLavorazioni(parentItem.sup_UMA_A_Edit, dataItem.Sup_A, row, indexColumnSupA_Edit, SuperficieOltre)

        checkErroriLavorazioni(parentItem.sup_UMA_B_Edit, dataItem.Sup_B, row, indexColumnSupB_Edit, SuperficieOltre)

        checkErroriLavorazioni(parentItem.TerrenoNormale_Edit, dataItem.TerrenoNormale, row, indexColumnTerrenoNormale_Edit, SuperficieOltre)

        checkErroriLavorazioni(parentItem.TerrenoMedio_Edit, dataItem.TerrenoMedio, row, indexColumnTerrenoMedio_Edit, SuperficieOltre)

        checkErroriLavorazioni(parentItem.TerrenoTenace_Edit, dataItem.TerrenoTenace, row, indexColumnTerrenoTenace_Edit, SuperficieOltre)

        var totPend = parseFloat((dataItem.Sup_A + dataItem.Sup_B).toFixed(4));
        if (dataItem.Superficie_Trattata.toFixed(4) < totPend) {
            AddErrorClass(row, indexColumnSupA_Edit, errorCell,
                "La somma delle superfici in Zona A e Zona B supera la superficie totale di " + (totPend - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
            AddErrorClass(row, indexColumnSupB_Edit, errorCell,
                "La somma delle superfici in Zona A e Zona B supera la superficie totale di " + (totPend - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
        } else {
            //row.children().eq(indexColumnSupA_Edit).removeClass(errorCell);
            //row.children().eq(indexColumnSupB_Edit).removeClass(errorCell);
        }

        //if (/*QS_Avanzamento == 1 &&*/ dataItem.CUAA == ""/* && !dataItem.Note_Compilatore.includes("Anticipazioni")*/) {
        //Salvo i record che fanno sovrapposizione in lav_incr_orig
        var lav_Incr_orig = new Array();
        var lav_Incr = new Array();
        if (dataItem.Lav_UMA_Cod != undefined) {
            lav_Incr_orig = $.grep(lav_incrociati[dataItem.Macrouso_UMA_Cod], function (e) { return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod; }); //{ var d = new Date(e.Validita_Inizio); return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod && d.toLocaleDateString() == dataItem.Validita_Inizio.toLocaleDateString(); });
        }
        //creouna copia di essi dentro lav_incr (così se li modifico, quelli originali rimangono intatti)
        lav_Incr_orig.forEach(x => lav_Incr.push(JSON.parse(JSON.stringify(x))))

        if (lav_Incr.length > 0 && $("#stato_pratica_cod").val() == In_Compilazione.toString()) {
            //prepare il messaggio di errore contenente la lista di numero rendicontazione - ragione sociale e lavorazione per rendere comprensibile la sovrapposizione all'utente
            let caus = lav_Incr.reduce(function (stri, current) { return stri + current.numero.toString() + " ( " + current.rag_soc + " " + current.val_cod + " ) " }, "")

            let nOpMax = recupera_nLavPreviste(dataItem.Lav_UMA_Cod, parentItem.Macrouso_UMA_Cod, dataItem.Lav_Cod, parentItem.Regolamento_Cod);

            //in caso il record appena inserito sia una lavorazione "MAX N VOLTE", è necessario un procedimento specifico
            if (nOpMax > 1) {

                //eseguo il controllo solo sull'ultimo record inserito 
                if (dataItem.richiestaDettaglioCod == 0) {
                    for (var g = rows.length - 1; g > j; g--) {
                        //per ogni record con la stessa lavorazione dentro la stessa coltura, lo aggiungo alla lista (aggiungo una copia sempre per mantenere l'originale intatto)
                        let temp = JSON.parse(JSON.stringify(e.sender.dataItem(rows[g])));
                        if (temp.Lav_UMA_Cod == dataItem.Lav_UMA_Cod /*&& temp.richiestaDettaglioCod != 0*/)
                            lav_Incr.push(temp)
                    }
                    //per ogni coppia di record (uno esterno e uno interno) che hanno come somma di superfici la superficie totale del terreno, imposto la loro superficie pari a quella del terreno
                    for (var t = 0; t < Math.floor(lav_Incr.length / 2); t++) {
                        for (var r = lav_Incr.length - 1; r >= Math.floor(lav_Incr.length / 2); r--) {
                            if (parentItem.sup_UMA - 0.0001 < lav_Incr[t].Totale_Superficie_UMA + lav_Incr[r].Superficie_Trattata &&
                                lav_Incr[t].Totale_Superficie_UMA + lav_Incr[r].Superficie_Trattata <= parentItem.sup_UMA) {
                                lav_Incr[r].Superficie_Trattata = parentItem.sup_UMA;
                                lav_Incr[t].Totale_Superficie_UMA = parentItem.sup_UMA;
                                break
                            }
                        }
                    }
                    checkErroriLavorazioniMaxVolte(parentItem.sup_UMA, dataItem.Superficie_Trattata, row, indexColumnSupTot_Edit, LavSovrapposte, lav_Incr, caus, nOpMax, dataItem, indexColumnRichiesto)
                } else
                    checkModificaLavorazioniMaxVolte(parentItem, rows, row, dataItem, indexColumnSupTot_Edit, e, false, nOpMax, indexColumnRichiesto);

            } else {

                let maxFabbisognoCalcolato = Math.max(...lav_Incr.map(item => item.fabbisogno_calcolato || 0));
                let assegnato_Sovrapp = $.grep(lav_Incr, function (e) { return e.Stato_Cod === 2005; }).reduce(function (sum, current) { return sum + current.Fabbisogno_Assegnato }, 0);
                let terreno_Assegnato_Sovrapp = $.grep(lav_Incr, function (e) { return e.Stato_Cod === 2005; }).reduce(function (sum, current) { return sum + current.Totale_Superficie_UMA }, 0);
                let terreno_Assegnabile_Sovrapp = $.grep(lav_Incr, function (e) { return e.Stato_Cod === 2005; }).reduce(function (sum, current) { return sum + current.Totale_Richiedibile}, 0);
                let supA_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Pendenza_A_UMA }, 0);
                let supB_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Pendenza_B_UMA }, 0);
                let supNormale_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Normale_UMA }, 0);
                let supMedia_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Media_UMA }, 0);
                let supTenace_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Tenace_UMA }, 0);

                if (((assegnato_Sovrapp < dataItem.fabbisognoCalc) || (maxFabbisognoCalcolato != dataItem.fabbisognoCalc)) && terreno_Assegnato_Sovrapp == terreno_Assegnabile_Sovrapp && dataItem.Superficie_Trattata == parentItem.sup_UMA) {

                    if (dataItem.ltrichiesto > 0 && (assegnato_Sovrapp > (dataItem.fabbisognoCalc - dataItem.ltrichiesto + (1 - parseFloat("0." + (dataItem.fabbisognoCalc + "").split(".")[1]) || (maxFabbisognoCalcolato != dataItem.fabbisognoCalc)))))
                        AddErrorClass(row, indexColumnRichiesto, errorCell,
                            "E' possibile richiedere solo " + Math.max(Math.round(dataItem.fabbisognoCalc - assegnato_Sovrapp), 0) + " Lt di carburante per questa lavorazione perchè " + Math.round(assegnato_Sovrapp) + " Lt sono già stati richiesti e approvati nelle richieste numero " + caus);
                    else
                        RemoveErrorClass(row, indexColumnRichiesto, errorCell);

                } else {

                    tentativoRipartizioneSovrapposizione(parentItem, dataItem, supA_Sovrapp, supB_Sovrapp, supNormale_Sovrapp, supMedia_Sovrapp, supTenace_Sovrapp);

                    checkErroriLavorazioni(parentItem.sup_UMA_Edit, dataItem.Superficie_Trattata, row, indexColumnSupTot_Edit, LavSovrapposteProprio, lav_Incr.reduce(function (sum, current) { return sum + current.Totale_Superficie_UMA }, 0), caus)

                    checkErroriLavorazioni(parentItem.sup_UMA_A_Edit, dataItem.Sup_A, row, indexColumnSupA_Edit, LavSovrapposteProprio, supA_Sovrapp, caus)

                    checkErroriLavorazioni(parentItem.sup_UMA_B_Edit, dataItem.Sup_B, row, indexColumnSupB_Edit, LavSovrapposteProprio, supB_Sovrapp, caus)

                    checkErroriLavorazioni(parentItem.TerrenoNormale_Edit, dataItem.TerrenoNormale, row, indexColumnTerrenoNormale_Edit, LavSovrapposteProprio, supNormale_Sovrapp, caus)

                    checkErroriLavorazioni(parentItem.TerrenoMedio_Edit, dataItem.TerrenoMedio, row, indexColumnTerrenoMedio_Edit, LavSovrapposteProprio, supMedia_Sovrapp, caus)

                    checkErroriLavorazioni(parentItem.TerrenoTenace_Edit, dataItem.TerrenoTenace, row, indexColumnTerrenoTenace_Edit, LavSovrapposteProprio, supTenace_Sovrapp, caus)

                }
            }
        }
        //}

        var totTess = parseFloat((dataItem.TerrenoNormale + dataItem.TerrenoMedio + dataItem.TerrenoTenace).toFixed(4));
        if (dataItem.Superficie_Trattata.toFixed(4) < totTess) {
            AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell,
                SommaVariTipi + totTess + "ha (Normale, Medio, Tenace) supera la superficie totale di " + (totTess - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
            AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell,
                SommaVariTipi + totTess + "ha (Normale, Medio, Tenace) supera la superficie totale di " + (totTess - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
            AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell,
                SommaVariTipi + totTess + "ha (Normale, Medio, Tenace) supera la superficie totale di " + (totTess - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
        } else {
            if (dataItem.TerrenoNormale < 0) {
                AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell, SuperficieNegativa);
            } else {
                //RemoveErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell)
            }

            if (dataItem.TerrenoMedio < 0) {
                AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell, SuperficieNegativa);
            } else {
                //RemoveErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell)
            }

            if (dataItem.TerrenoTenace < 0) {
                AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell, SuperficieNegativa);
            } else {
                //RemoveErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell)
            }
            //row.children().eq(indexColumnTerrenoNormale_Edit).removeClass(errorCell);
            //row.children().eq(indexColumnTerrenoMedio_Edit).removeClass(errorCell);
            //row.children().eq(indexColumnTerrenoTenace_Edit).removeClass(errorCell);

            /*if (lav_alt_terreni[dataItem.Lav_UMA_Cod] != undefined) {
                var check = -1;
                lav_alt_terreni[dataItem.Lav_UMA_Cod].forEach((innElem, index) => { if (innElem > parentArray[index]) check = index; });
                if (check > -1) {
                    AddErrorClass(row, indexColumnLav, errorCell, "Questa lavorazione contiene ettari in eccesso rispetto al totale se sommati ad altre lavorazioni non consentite con questa");
                    switch (check) {
                        case 0: {
                            AddErrorClass(row, indexColumnSupTot_Edit, errorCell, "Questa lavorazione contiene ettari in eccesso rispetto al totale se sommati ad altre lavorazioni non consentite con questa");
                            break;
                        }
                        case 1: {
                            AddErrorClass(row, indexColumnSupA_Edit, errorCell, "Questa lavorazione contiene ettari in eccesso rispetto al totale se sommati ad altre lavorazioni non consentite con questa");
                            break;
                        }
                        case 2: {
                            AddErrorClass(row, indexColumnSupB_Edit, errorCell, "Questa lavorazione contiene ettari in eccesso rispetto al totale se sommati ad altre lavorazioni non consentite con questa");
                            break;
                        }
                        case 3: {
                            AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell, "Questa lavorazione contiene ettari in eccesso rispetto al totale se sommati ad altre lavorazioni non consentite con questa");
                            break;
                        }
                        case 4: {
                            AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell, "Questa lavorazione contiene ettari in eccesso rispetto al totale se sommati ad altre lavorazioni non consentite con questa");
                            break;
                        }
                        case 5: {
                            AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell, "Questa lavorazione contiene ettari in eccesso rispetto al totale se sommati ad altre lavorazioni non consentite con questa");
                            break;
                        }
                    }
                } else {
                    RemoveErrorClass(row, indexColumnLav, errorCell);
                    RemoveErrorClass(row, indexColumnSupTot_Edit, errorCell);
                    RemoveErrorClass(row, indexColumnSupA_Edit, errorCell);
                    RemoveErrorClass(row, indexColumnSupB_Edit, errorCell);
                    RemoveErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell);
                    RemoveErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell);
                    RemoveErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell);
                }
            }*/

        }

        if (QS_Type === 0) {
            let costo = filtraTabellaCalcoloCosti(parentItem.Macrouso_UMA_Cod, dataItem.Lav_UMA_Cod, null, dataItem.Attivita_Cod, parentItem.Regolamento_Cod);
            if (costo.Lav_Cod === 1) {
                if (costo.Coeff_acq_distr === undefined || costo.Coeff_acq_distr === null)
                    costo.Coeff_acq_distr = 0;
                if (costo.Coeff_acq_distr > 0) {
                    if (permessoAcquaGiaRichiesto == 0 &&
                        ($("#permessoAcqua").val() === "0" || $("#permessoAcqua").val() === null || $("#permessoAcqua").val() === undefined || 
                         $("#notePermessoAcqua").val() === "" || $("#notePermessoAcqua").val() === null || $("#notePermessoAcqua").val() === undefined))
                        AddErrorClass(row, indexColumnLav, errorCell, volumeAcqua)
                    else
                        RemoveErrorClass(row, indexColumnLav, volumeAcqua)
                    //totRichiestoIrrigazioni = totRichiestoIrrigazioni + parseFloat(dataItem.ltrichiesto)
                    if (costo.Coeff_acq_distr < minCoeff)
                        minCoeff = costo.Coeff_acq_distr
                    totPermessoAcqua = (parseFloat($("#permessoAcqua").val()) + permessoAcquaGiaRichiesto) * minCoeff
                    if (Math.round(totPermessoAcqua) < Math.round(totRichiestoIrrigazioni)) {
                        console.log("Permesso acqua: " + totPermessoAcqua + " - Totale lavorazioni: " + totRichiestoIrrigazioni)
                        AddErrorClass(row, indexColumnLav, errorCell, eccessoAcqua + " (" + Math.round(totPermessoAcqua) + " lt)")
                    } else {
                        RemoveErrorClass(row, indexColumnLav, eccessoAcqua + " (" + Math.round(totPermessoAcqua) + " lt)")
                    }
                }
            }
        }

    }
    lav_alt_superfici = {};
    lav_alt_terreni = {};
    /*
    for (var key in lav_alt_lim[parentItem.Macrouso_UMA_Cod]) {
        lav_alt_lim[parentItem.Macrouso_UMA_Cod][key] = 0;
    }*/
}

function tentativoRipartizioneSovrapposizione(parentItem, dataItem, supA_Sovrapp, supB_Sovrapp, supNormale_Sovrapp, supMedia_Sovrapp, supTenace_Sovrapp) {
    let diff = 0
    let diffSup = 0;
    //pendenza
    if (dataItem.Sup_A > 0 && parentItem.sup_UMA_A_Edit < (dataItem.Sup_A + supA_Sovrapp)) {
        diffSup = parseFloat((parentItem.sup_UMA_A_Edit - supA_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.Sup_A - diffSup).toFixed(4));
        if (parentItem.sup_UMA_B_Edit > (dataItem.Sup_B + diff - 0.0001)) {
            dataItem.Sup_A = parseFloat(diffSup);
            dataItem.Sup_B = parseFloat(dataItem.Sup_B + diff);
        }
    }
    if (dataItem.Sup_B > 0 && parentItem.sup_UMA_B_Edit < (dataItem.Sup_B + supB_Sovrapp)) {
        diffSup = parseFloat((parentItem.sup_UMA_B_Edit - supB_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.Sup_B - diffSup).toFixed(4));
        if (parentItem.sup_UMA_A_Edit > (dataItem.Sup_A + diff - 0.0001)) {
            dataItem.Sup_B = parseFloat(diffSup);
            dataItem.Sup_A = parseFloat(dataItem.Sup_A + diff);
        }
    }

    //tessitura
    if (dataItem.TerrenoNormale > 0 && parentItem.TerrenoNormale_Edit < (dataItem.TerrenoNormale + supNormale_Sovrapp)) {
        diffSup = parseFloat((parentItem.TerrenoNormale_Edit - supNormale_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.TerrenoNormale - diffSup).toFixed(4));
        if ((parentItem.TerrenoMedio_Edit + parentItem.TerrenoTenace_Edit) > (dataItem.TerrenoTenace + dataItem.TerrenoMedio + diff - 0.0001)) {
            if (parentItem.TerrenoMedio_Edit > (dataItem.TerrenoMedio + diff - 0.0001)) {
                dataItem.TerrenoNormale = parseFloat(diffSup);
                dataItem.TerrenoMedio = parseFloat(dataItem.TerrenoMedio + diff);
            } else {
                diff = parseFloat((diff - (parentItem.TerrenoMedio_Edit - dataItem.TerrenoMedio).toFixed(4)).toFixed(4));
                if (parentItem.TerrenoTenace_Edit > (dataItem.TerrenoTenace + diff - 0.0001)) {
                    dataItem.TerrenoNormale = parseFloat(diffSup);
                    dataItem.TerrenoMedio = parentItem.TerrenoMedio_Edit;
                    dataItem.TerrenoTenace = parseFloat(dataItem.TerrenoTenace + diff);
                }
            }
        }
    }
    if (dataItem.TerrenoMedio > 0 && parentItem.TerrenoMedio_Edit < (dataItem.TerrenoMedio + supMedia_Sovrapp)) {
        diffSup = parseFloat((parentItem.TerrenoMedio_Edit - supMedia_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.TerrenoMedio - diffSup).toFixed(4));
        if ((parentItem.TerrenoNormale_Edit + parentItem.TerrenoTenace_Edit) > (dataItem.TerrenoTenace + dataItem.TerrenoNormale + diff - 0.0001)) {
            if (parentItem.TerrenoNormale_Edit > (dataItem.TerrenoNormale + diff - 0.0001)) {
                dataItem.TerrenoMedio = parseFloat(diffSup);
                dataItem.TerrenoNormale = parseFloat(dataItem.TerrenoNormale + diff);
            } else {
                diff = parseFloat((diff - (parentItem.TerrenoNormale_Edit - dataItem.TerrenoNormale).toFixed(4)).toFixed(4));
                if (parentItem.TerrenoTenace_Edit > (dataItem.TerrenoTenace + diff - 0.0001)) {
                    dataItem.TerrenoNormale = parentItem.TerrenoNormale_Edit;
                    dataItem.TerrenoMedio = parseFloat(diffSup);
                    dataItem.TerrenoTenace = parseFloat(dataItem.TerrenoTenace + diff);
                }
            }
        }
    }
    if (dataItem.TerrenoTenace > 0 && parentItem.TerrenoTenace_Edit < (dataItem.TerrenoTenace + supTenace_Sovrapp)) {
        diffSup = parseFloat((parentItem.TerrenoTenace_Edit - supTenace_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.TerrenoTenace - diffSup).toFixed(4));
        if ((parentItem.TerrenoNormale_Edit + parentItem.TerrenoMedio_Edit) > (dataItem.TerrenoMedio + dataItem.TerrenoNormale + diff - 0.0001)) {
            if (parentItem.TerrenoNormale_Edit > (dataItem.TerrenoNormale + diff - 0.0001)) {
                dataItem.TerrenoTenace = parseFloat(diffSup);
                dataItem.TerrenoNormale = parseFloat(dataItem.TerrenoNormale + diff);
            } else {
                diff = parseFloat((diff - (parentItem.TerrenoNormale_Edit - dataItem.TerrenoNormale).toFixed(4)).toFixed(4));
                if (parentItem.TerrenoMedio_Edit > (dataItem.TerrenoMedio + diff - 0.0001)) {
                    dataItem.TerrenoNormale = parentItem.TerrenoNormale_Edit;
                    dataItem.TerrenoTenace = parseFloat(diffSup);
                    dataItem.TerrenoMedio = parseFloat(dataItem.TerrenoMedio + diff);
                }
            }
        }
    }
}

function checkErroriLavorazioni(parent, current, row, index, mess, altreLav = 0, cause = "") {
    if (current > 0 && parent < (current + altreLav - 0.0001))
        AddErrorClass(row, index, errorCell,
            mess + ((altreLav + current) - parent).toFixed(4) + " Ha " + (cause != "" ? " Rilevato nelle " + (QS_Avanzamento == 1 ? "rendicontazioni" : "richieste") + " numero: " + cause : ""));
    else
        RemoveErrorClass(row, index, errorCell);
}

//function checkErroriLavorazioniMAXQuattroVolte(parent, current, row, index, mess, altreLav, cause = "") {

//    let err = true;
//    let tot = altreLav.reduce((tot, x) => { return tot + (parseFloat(x.Totale_Superficie_UMA) > 0 ? parseFloat(x.Totale_Superficie_UMA) : parseFloat(x.Superficie_Trattata)) }, 0)
//    let trueTot = altreLav.reduce((tot, x) => {
//        return tot + (parseFloat(x.Zona_Pendenza_A_UMA) >= 0 ?
//            parseFloat(x.Zona_Pendenza_A_UMA) + parseFloat(x.Zona_Pendenza_B_UMA) : parseFloat(x.Sup_A) + parseFloat(x.Sup_B))
//    }, 0)
//    let incrCount = altreLav.reduce((tot, x) => { return tot + (x.rag_soc != undefined ? 1 : 0) }, 0)
//    let terreniLavorati = 0
//    for (var t = 0; t <= incrCount; t++) {
//        for (var r = altreLav.length - 1; r > incrCount; r--) {
//            if ((altreLav[t].Totale_Superficie_UMA == parent &&
//                altreLav[r].Superficie_Trattata == parent)
//                || (parseFloat(altreLav[t].Zona_Pendenza_A_UMA) + parseFloat(altreLav[t].Zona_Pendenza_B_UMA)) == parent
//                || (parseFloat(altreLav[r].Sup_A) + parseFloat(altreLav[r].Sup_B)) == parent) {
//                terreniLavorati++;
//            }
//        }
//    }
//    //se la superficie indicata è compatibile con almeno un'altra lavorazione, allora posso inserirla
//    altreLav.forEach(x => { if (parent >= (current + (x.Totale_Superficie_UMA > 0 ? x.Totale_Superficie_UMA : x.Superficie_Trattata) - 0.0001)) err = false })
//    //in caso le lavorazioni totali siano meno di 4, allora posso inerirne anche una con superficie_Lavorata pari alla superficie totale
//    if ((trueTot + current > parent * 4) || (err && (incrCount > altreLav.length / 2 && altreLav.length >= 4) && terreniLavorati > 3))
//        AddErrorClass(row, index, errorCell,
//            mess + ((tot / altreLav.length + current) - parent).toFixed(4) + " Ha " + (cause != "" ? " Rilevato nelle rendicontazioni numero: " + cause : ""));
//    else
//        RemoveErrorClass(row, index, errorCell);


//}

//function checkModificaLavorazioniMAXQuattroVolte(parentItem, rows, row, dataItem, indexColumnSupTot_Edit, e, isTerz = true) {
//    var lav_Incr_orig = new Array();
//    var lav_Incr = new Array();
//    let parentSup = isTerz ? parentItem.sup_tot_Orig : parentItem.sup_UMA
//    if (dataItem.Lav_UMA_Cod != undefined) {
//        lav_Incr_orig = $.grep(isTerz ? lav_incrociati[dataItem.Piva][dataItem.Macrouso_UMA_Cod] : lav_incrociati[dataItem.Macrouso_UMA_Cod],
//            function (e) { return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod; }); //{ var d = new Date(e.Validita_Inizio); return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod && d.toLocaleDateString() == dataItem.Validita_Inizio.toLocaleDateString(); });
//    }

//    lav_Incr_orig.forEach(x => lav_Incr.push(JSON.parse(JSON.stringify(x))))

//    if (lav_Incr.length > 0) {

//        let caus = lav_Incr.reduce(function (stri, current) { return stri + current.numero.toString() + " ( " + current.rag_soc + " " + current.val_cod + " ) " }, "")

//        for (var g = rows.length - 1; g >= 0; g--) {
//            let temp = JSON.parse(JSON.stringify(e.sender.dataItem(rows[g])));
//            if (temp.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && temp.richiestaDettaglioCod != dataItem.richiestaDettaglioCod)
//                lav_Incr.push(temp)
//        }
//        for (var t = 0; t < Math.floor(lav_Incr.length / 2); t++) {
//            for (var r = lav_Incr.length - 1; r >= Math.floor(lav_Incr.length / 2); r--) {
//                if (parentSup - 1 < lav_Incr[t].Totale_Superficie_UMA + lav_Incr[r].Superficie_Trattata &&
//                    lav_Incr[t].Totale_Superficie_UMA + lav_Incr[r].Superficie_Trattata <= parentSup) {
//                    lav_Incr[r].Superficie_Trattata = parentSup;
//                    lav_Incr[t].Totale_Superficie_UMA = parentSup;
//                    break
//                }
//            }
//        }
//        checkErroriLavorazioniMAXQuattroVolte(parentSup, dataItem.Superficie_Trattata, row, indexColumnSupTot_Edit, LavSovrapposte, lav_Incr, caus)

//    }
//}

function checkErroriLavorazioniMaxVolte(parent, current, row, index, mess, altreLav, cause = "", nOpMax, dataItem, indexLt) {

    let err = true;
    let tot = altreLav.reduce((tot, x) => { return tot + (parseFloat(x.Totale_Superficie_UMA) > 0 ? parseFloat(x.Totale_Superficie_UMA) : parseFloat(x.Superficie_Trattata)) }, 0)
    let trueTot = altreLav.reduce((tot, x) => {
        return tot + (parseFloat(x.Zona_Pendenza_A_UMA) >= 0 ?
            parseFloat(x.Zona_Pendenza_A_UMA) + parseFloat(x.Zona_Pendenza_B_UMA) : parseFloat(x.Sup_A) + parseFloat(x.Sup_B))
    }, 0)
    let incrCount = altreLav.reduce((tot, x) => { return tot + (x.rag_soc != undefined ? 1 : 0) }, 0)
    let terreniLavorati = 0
    for (var t = 0; t <= incrCount; t++) {
        for (var r = altreLav.length - 1; r > incrCount; r--) {
            if ((altreLav[t].Totale_Superficie_UMA == parent &&
                altreLav[r].Superficie_Trattata == parent)
                || (parseFloat(altreLav[t].Zona_Pendenza_A_UMA) + parseFloat(altreLav[t].Zona_Pendenza_B_UMA)) == parent
                || (parseFloat(altreLav[r].Sup_A) + parseFloat(altreLav[r].Sup_B)) == parent) {
                terreniLavorati++;
            }
        }
    }

    //se la superficie indicata è compatibile con almeno un'altra lavorazione, allora posso inserirla
    altreLav.forEach(x => { if (parent >= (current + (x.Totale_Superficie_UMA > 0 ? x.Totale_Superficie_UMA : x.Superficie_Trattata) - 0.0001)) err = false })

    let totCarbEsterno = 0
    altreLav.forEach(x => {
        if (x.Fabbisogno_Assegnato !== undefined)
                totCarbEsterno += x.Fabbisogno_Assegnato;
        else
            totCarbEsterno += x.ltrichiesto;
    })
    let totCarb = totCarbEsterno + dataItem.ltrichiesto - 1
    let richiedibile = Math.round((nOpMax * dataItem.fabbisognoCalc) - totCarbEsterno);

    if (current == parent && totCarb < dataItem.fabbisognoCalc * (nOpMax + 1)) {

        if (totCarb > dataItem.fabbisognoCalc * nOpMax)
            AddErrorClass(row, indexLt, errorCell, "E' possibile richiedere solo " + Math.max(richiedibile, 0) + " Lt di carburante per questa lavorazione per via dei Lt già richiesti in questa pratica e approvati nelle richieste numero " + cause);
        else
            RemoveErrorClass(row, indexLt, errorCell);
        
    } else {
        //in caso le lavorazioni totali siano meno di nOpMax, allora posso inerirne anche una con superficie_Lavorata pari alla superficie totale
        if ((trueTot + current > parent * nOpMax) || (err && (incrCount > altreLav.length / 2 && altreLav.length >= nOpMax) && terreniLavorati > (nOpMax - 1)))
            AddErrorClass(row, index, errorCell,
                mess + ((tot / altreLav.length + current) - parent).toFixed(4) + " Ha " + (cause != "" ? " Rilevato nelle rendicontazioni numero: " + cause : ""));
        else
            RemoveErrorClass(row, index, errorCell);
    }

}

function checkNoteObbligatorie(row, dataItem, indexNoteLav) {
    let anno = parseInt($("#anno").val());
    let dataInizioValiditaPratica = Date.parse(new Date(anno, 0, 1));
    var nota_obbligatoria = $.grep(lav_note_obbligatorie,
        function (e) {
            return e.Lav_UMA_Cod == dataItem.Lav_UMA_Cod &&
                e.Macrouso_UMA_Cod == dataItem.Macrouso_UMA_Cod &&
                dataInizioValiditaPratica >= Date.parse(e.Validita_Inizio) &&
                dataInizioValiditaPratica <= Date.parse(e.Validita_Fine);
        });

    if (nota_obbligatoria.length > 0) {

        if (dataItem.Note_Compilatore.trim() === "") {
            AddErrorClass(row, indexNoteLav, errorCell, "Per questa lavorazione è obbligatoria la compilazione delle note");
        } else {
            RemoveErrorClass(row, indexNoteLav, errorCell);
        }

        if ($("#stato_pratica_cod").val() !== In_Compilazione.toString()) {
            for (var cell = 0; cell < row[0].children.length; cell++) {
                if (!row[0].children[cell].classList.contains("errorCell")) {
                    row[0].children[cell].style.backgroundColor = "#e3c668"
                }
            }
        }
    }
}

function checkModificaLavorazioniMaxVolte(parentItem, rows, row, dataItem, indexColumnSupTot_Edit, e, isTerz = true, nMaxOp, indexLt) {

    var lav_Incr_orig = new Array();
    var lav_Incr = new Array();
    let parentSup = isTerz ? parentItem.sup_tot_Orig : parentItem.sup_UMA

    if (dataItem.Lav_UMA_Cod != undefined) {
        lav_Incr_orig = $.grep(isTerz ? lav_incrociati[dataItem.Piva][dataItem.Macrouso_UMA_Cod] : lav_incrociati[dataItem.Macrouso_UMA_Cod],
            function (e) { return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod; }); //{ var d = new Date(e.Validita_Inizio); return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod && d.toLocaleDateString() == dataItem.Validita_Inizio.toLocaleDateString(); });
    }

    lav_Incr_orig.forEach(x => lav_Incr.push(JSON.parse(JSON.stringify(x))))

    if (lav_Incr.length > 0) {

        let caus = lav_Incr.reduce(function (stri, current) { return stri + current.numero.toString() + " ( " + current.rag_soc + " " + current.val_cod + " ) " }, "")

        for (var g = rows.length - 1; g >= 0; g--) {
            let temp = JSON.parse(JSON.stringify(e.sender.dataItem(rows[g])));
            if (temp.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && temp.richiestaDettaglioCod != dataItem.richiestaDettaglioCod)
                lav_Incr.push(temp)
        }
        for (var t = 0; t < Math.floor(lav_Incr.length / 2); t++) {
            for (var r = lav_Incr.length - 1; r >= Math.floor(lav_Incr.length / 2); r--) {
                if (parentSup - 1 < lav_Incr[t].Totale_Superficie_UMA + lav_Incr[r].Superficie_Trattata &&
                    lav_Incr[t].Totale_Superficie_UMA + lav_Incr[r].Superficie_Trattata <= parentSup) {
                    lav_Incr[r].Superficie_Trattata = parentSup;
                    lav_Incr[t].Totale_Superficie_UMA = parentSup;
                    break
                }
            }
        }

        checkErroriLavorazioniMaxVolte(parentSup, dataItem.Superficie_Trattata, row, indexColumnSupTot_Edit, LavSovrapposte, lav_Incr, caus, nMaxOp, dataItem, indexLt)

    }

}

function CheckLavorazioniAlternative(row, parentItem, dataItem, indexColumnLav, lav_Alt_Spec, lav_NO_Spec, indexChanged) {
    var prevent_add = false;
    //ciclo nativo di js su array associativo con find
    var lav_Attuale = $.grep(lav_Alt_Spec, function (e) { return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod; });
    let supArray = [];
    supArray.push(dataItem.Superficie_Trattata);
    supArray.push(dataItem.Sup_A);
    supArray.push(dataItem.Sup_B);
    supArray.push(dataItem.TerrenoNormale);
    supArray.push(dataItem.TerrenoMedio);
    supArray.push(dataItem.TerrenoTenace);

    //se la lavorazione appena inserita è esclusa da/esclude altre lavorazioni
    if (lav_Attuale.length > 0) {
        //se la lista delle lavorazioni non consentite non è vuota
        if (lav_NO[parentItem.Macrouso_UMA_Cod] != undefined) {
            //se la lavorazione attuale è presente nell'elenco di quelle escluse => errore
            if (lav_NO[parentItem.Macrouso_UMA_Cod][lav_Attuale[0]["Lavorazione_UMA"]] != undefined && lav_alt_superfici[lav_Attuale[0]["Lavorazione_UMA"]] != undefined &&
                lav_alt_superfici[lav_Attuale[0]["Lavorazione_UMA"]] < dataItem.Superficie_Trattata) {

                AddErrorClass(row, indexColumnLav, errorCell, "Questa lavorazione non è consentita sovrapposta a " + lav_NO[parentItem.Macrouso_UMA_Cod][lav_Attuale[0]["Lavorazione_UMA"]]);
                indexChanged = true;
                prevent_add = true;

            } else {
                if (!indexChanged)
                    RemoveErrorClass(row, indexColumnLav, errorCell);
            }
        }
    }

    //nel caso non ci siano errori, aggiorno la lista di lavorazioni escluse
    //la lista è fatta così: lista[codice gruppo colturale UMA][Codice lavorazione UMA della lavorazione attuale] = lavorazione che esclude la lavorazione attuale
    //esempio se nel gruppo colturale xxx la lavorazione 10 esclude la 20 => lista[xxx][20] = 10
    if (lav_Attuale.length > 0 && !prevent_add) {

        lav_NO_Spec = lav_Attuale[0]["Lavorazioni_Alt"].split(" , ");
        if (lav_NO[parentItem.Macrouso_UMA_Cod] == undefined) lav_NO[parentItem.Macrouso_UMA_Cod] = {}

        if (lav_alt_terreni[lav_Attuale[0]["Lavorazione_UMA"]] == undefined)
            lav_alt_terreni[lav_Attuale[0]["Lavorazione_UMA"]] = Array.from(supArray);
        else
            for (cont = 0; cont < lav_alt_terreni[lav_Attuale[0]["Lavorazione_UMA"]].length; cont++) {
                lav_alt_terreni[lav_Attuale[0]["Lavorazione_UMA"]][cont] += supArray[cont];
            }


        for (var n = 0; n < lav_NO_Spec.length; n++) {

            lav_NO[parentItem.Macrouso_UMA_Cod][lav_NO_Spec[n].trimEnd()] = lav_Attuale[0]["Lav_UMA_Des"];
            if (lav_alt_superfici[lav_NO_Spec[n].trimEnd()] == undefined) lav_alt_superfici[lav_NO_Spec[n].trimEnd()] = 0;
            lav_alt_superfici[lav_NO_Spec[n].trimEnd()] = ((lav_alt_superfici[lav_NO_Spec[n].trimEnd()] == 0 ?
                (parentItem.sup_tot == undefined ? parentItem.sup_UMA : parentItem.sup_tot) :
                lav_alt_superfici[lav_NO_Spec[n].trimEnd()]) - dataItem.Superficie_Trattata).toFixed(4);
            if (lav_alt_terreni[lav_NO_Spec[n].trimEnd()] == undefined)
                lav_alt_terreni[lav_NO_Spec[n].trimEnd()] = Array.from(supArray);
            else
                for (cont = 0; cont < lav_alt_terreni[lav_NO_Spec[n].trimEnd()].length; cont++) {
                    lav_alt_terreni[lav_NO_Spec[n].trimEnd()][cont] += supArray[cont];
                }
        }
    }

    return indexChanged;

    //nel caso sia una riga non vuota(appena inserita), controllo se non eccede il numero di lavorazioni previste per quella specifica lavorazione
    /*if (dataItem.LAV_COD != 0) {
        //nel caso sia la prima iterazione, inizializzo un oggetto vuoto
        if (lav_alt_lim[parentItem.Macrouso_UMA_Cod] == undefined) {
            lav_alt_lim[parentItem.Macrouso_UMA_Cod] = {};
        }
        //nel caso sia la prima volta che incontro quella lavorazione, inizializzo il suo contatore a 0
        if (lav_alt_lim[parentItem.Macrouso_UMA_Cod][dataItem.LAV_COD] == undefined) {
            lav_alt_lim[parentItem.Macrouso_UMA_Cod][dataItem.LAV_COD] = 0;
        }
        //per ogni lavorazione trovata incremento di 1 il suo contatore, se questo non è già arrivato al numero massimo consentito in nLavPreviste
        if (lav_alt_lim[parentItem.Macrouso_UMA_Cod][dataItem.LAV_COD] < dataItem.nLavPreviste) {
            lav_alt_lim[parentItem.Macrouso_UMA_Cod][dataItem.LAV_COD] += 1;
            row.children().eq(indexColumnLav).removeClass(errorCell);

        } else if (lav_alt_lim[parentItem.Macrouso_UMA_Cod][dataItem.LAV_COD] != undefined) {

            row.children().eq(indexColumnLav).addClass(errorCell)
            $(row.children().eq(indexColumnLav)).kendoTooltip({
                content: "Questa lavorazione è già stata effettuata " + dataItem.nLavPreviste.toString() + " volte",
                position: "top"
            });

        }
    }*/
}

function CheckMaxUdm_Alt(row, parentItem, dataItem, indexColumnQtaManuale) {
    //let costo = tabellaCalcoloCosti.filter((elem) => {
    //    return elem.Macrouso_UMA_Cod == parentItem.Macrouso_UMA_Cod &&
    //        elem.Lav_UMA_Cod == dataItem.Lav_UMA_Cod &&
    //        elem.Id_Attivita == dataItem.Attivita_Cod;
    //})[0];
    let costo = filtraTabellaCalcoloCosti(parentItem.Macrouso_UMA_Cod, dataItem.Lav_UMA_Cod, null, dataItem.Attivita_Cod, parentItem.Regolamento_Cod);
    if (costo != undefined) {
        if (!(costo.Udm_Alternativa == null || costo.Udm_Alternativa == undefined || costo.Udm_Alternativa == "")) {
            if (costo.Limite_Max == 1) {
                let max = parseFloat((dataItem.Superficie_Trattata * costo.Max_xHa).toFixed(4));
                if (dataItem.Qta_Manuale > max) {
                    AddErrorClass(row, indexColumnQtaManuale, errorCell, "Il valore massimo inseribile è " + max + "" + costo.Udm_Alternativa + " (" + costo.Max_xHa + "" + costo.Udm_Alternativa + "/ha)");
                }
            }
        }
    }
}

function filtraTabellaCalcoloCosti(Macrouso_UMA_Cod, Lav_UMA_Cod, Lav_Cod, Attivita_Cod, Regolamento_Cod) {

    tabellaCalcoloCostiFiltrata = [];

    if (Lav_Cod == null) {

        tabellaCalcoloCostiFiltrata = tabellaCalcoloCosti.filter((elem) => {
            return elem.Macrouso_UMA_Cod == Macrouso_UMA_Cod &&
                (elem.Regolamento_Cod == Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi) &&
                elem.Lav_UMA_Cod == Lav_UMA_Cod &&
                elem.Id_Attivita == Attivita_Cod;
        });

        if (tabellaCalcoloCostiFiltrata.length == 0) {
            tabellaCalcoloCostiFiltrata = tabellaCalcoloCosti.filter((elem) => {
                return elem.Macrouso_UMA_Cod == Macrouso_UMA_Cod &&
                    (elem.Regolamento_Cod == Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi) &&
                    elem.Lav_UMA_Cod == Lav_UMA_Cod;
            });
        }

    } else {

        tabellaCalcoloCostiFiltrata = tabellaCalcoloCosti.filter((elem) => {
            return elem.Macrouso_UMA_Cod == Macrouso_UMA_Cod &&
                (elem.Regolamento_Cod == Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi) &&
                elem.Lav_UMA_Cod == Lav_UMA_Cod &&
                elem.Lav_Cod == Lav_Cod &&
                elem.Id_Attivita == Attivita_Cod;
        });

    }

    return tabellaCalcoloCostiFiltrata[0];
}

function CheckMaxNum_Op(row, parentItem, dataItem, grid, indexColumnLav, indexChanged) {

    if (dataItem.Lav_UMA_Cod /*&& !dataItem.Note_Compilatore.includes("Anticipazioni")*/) {
        let nOpPreviste = recupera_nLavPreviste(dataItem.Lav_UMA_Cod, parentItem.Macrouso_UMA_Cod, dataItem.Lav_Cod, parentItem.Regolamento_Cod);
        let values = [parseFloat((parentItem.sup_UMA_Edit * nOpPreviste).toFixed(4)), parseFloat((parentItem.sup_tot * nOpPreviste).toFixed(4))].filter(value => Number.isFinite(value));
        let max_Ha = Math.max.apply(null, values);
        //Prima filtro gli elementi con il Lav_UMA_Cod che mi interessa
        //Poi calcolo la somma degli ettari lavorati
        let Ha_lavorati = 0;
        let arr = grid.dataSource.data().filter((el) => {
            return (el.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && (!el.deleted) &&
                (($("#stato_pratica_cod").val() == Verifica_In_Corso.toString() && el.ltAssegnato > 0) ||
                    ($("#stato_pratica_cod").val() == In_Compilazione.toString() && el.ltrichiesto > 0) || (el.Car_Cod == 9 || el.Car_Cod == 10)) /*&&
                !el.Note_Compilatore.includes("Anticipazioni")*/);
        });
        //let arr = grid.dataSource.data().filter((el) => { return (el.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && (!el.deleted) && el.CUAA == null); }); //aggiunto Gloria la condizione el.CUAA==null per escludere dal conteggio quelli che hanno la cuaa valorizzata

        for (let i = 0; i < arr.length; i++) {
            Ha_lavorati += arr[i].Superficie_Trattata;
        }
        Ha_lavorati = parseFloat(Ha_lavorati.toFixed(4));

        if (Ha_lavorati > max_Ha  //ESEGUO IL CONTROLLO SOLO IN CONTO PROPRIO
            && parentItem.Macrouso_UMA_Cod !== 9997) {

            AddErrorClass(row,
                indexColumnLav,
                errorCell,
                "Il totale degli Ha nella lavorazione " + dataItem.LavUMA + " non può superare " + max_Ha + " ha (max " + nOpPreviste + " op previste).");
            indexChanged = true;
        }

        if (dataItem.Lav_UMA_Cod === "10233") {
            if (grid.dataSource.data().filter((el) => {
                return (el.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && (!el.deleted))
            }).length > 1) {
                AddErrorClass(row,
                    indexColumnLav,
                    errorCell,
                    "Massimo " + nOpPreviste + " operazioni previste per " + dataItem.LavUMA);
                indexChanged = true;
            }
        }
    }
    return indexChanged;

}

function onEditGrigliaDettagliLavorazioni(e) {
    if (getMaggiorazioneAutomatica(e.model.Macrouso_UMA_Cod, e.model.Lav_UMA_Cod)) {
        if ($(".Lav_Alt, .k-edit-cell").length > 0 && e.container.hasClass("data") === false) {
            e.container.kendoTooltip({
                content: "I litri specificati verranno automaticamente maggiorati fino al punto in cui la decurtazione restituirà il valore inserito",
                position: "top"
            })
        }
    }
    let parentRow = $($(e.container).parents(".k-detail-row")[0]).prev();
    let parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);
    if (e.model.TipoCarb == "") {
        let codCar = parentRowItem.Macrouso_UMA_Cod == "1034" ? "8" : "2";
        let carburante_Default = elencoCarburanti.filter((elem) => elem.Car_Cod == codCar)[0];
        e.model.Car_Cod = carburante_Default.Car_Cod;
        e.model.TipoCarb = carburante_Default.TipoCarb;

        e.model.SupMaggiorazioneTrasferimenti = 0;
        e.model.fabbisognoCalc = 0;
        e.model.ltrichiesto = 0;
        e.model.ltAssegnato = 0;
        e.model.nLavPreviste = 0;
        e.model.nLavRichieste = 0;
        e.model.piuLavPreviste = 0;
        e.model.piuRaccoltiPrevisti = 0;
        e.model.richiestaDettaglioCod = 0;
        e.model.Mesi = 0;
        if (QS_Avanzamento == 1)
            e.model.Validita_Inizio = "";

        e.model.Macrouso_UMA_Cod = parentRowItem.Macrouso_UMA_Cod;
        e.model.Richiesta_Cod = parentRowItem.Richiesta_Cod;
        e.model.Programmazione_Cod = parentRowItem.Programmazione_Cod;

        e.model.Superficie_Trattata = parentRowItem.sup_UMA_Edit;
        e.model.Sup_A = parentRowItem.sup_UMA_A_Edit;
        e.model.Sup_B = parentRowItem.sup_UMA_B_Edit;
        e.model.TerrenoNormale = parentRowItem.TerrenoNormale_Edit;
        e.model.TerrenoMedio = parentRowItem.TerrenoMedio_Edit;
        e.model.TerrenoTenace = parentRowItem.TerrenoTenace_Edit;
        e.model.Piva = parentRow.Piva;
        e.model.dirty = true;
        e.sender.refresh();
    } else {
        if (e.model.Lav_UMA_Cod != "" &&
            e.model.Macrouso_UMA_Cod != "" &&
            e.model.LAV_COD != 0) {
            //let costo = tabellaCalcoloCosti.filter((elem) => {
            //    return elem.Macrouso_UMA_Cod == e.model.Macrouso_UMA_Cod &&
            //        elem.Lav_UMA_Cod == e.model.Lav_UMA_Cod &&
            //        elem.Lav_Cod == e.model.LAV_COD &&
            //        elem.Id_Attivita == e.model.Attivita_Cod;
            //})[0];
            let costo = filtraTabellaCalcoloCosti(e.model.Macrouso_UMA_Cod, e.model.Lav_UMA_Cod, e.model.LAV_COD, e.model.Attivita_Cod, parentRowItem.Regolamento_Cod);
            if (costo) {
                if (costo.Udm_Alternativa == null || costo.Udm_Alternativa == undefined || costo.Udm_Alternativa == "") {
                    if ($(e.container[0]).hasClass("Lav_Alt")) {
                        e.sender.closeCell();
                    }
                } else {
                    if ($(e.container[0]).hasClass("Lav_Norm")) {
                        e.sender.closeCell();
                    }
                }
            } else {
                e.sender.closeCell();
            }
        }
    }

    if (getMaggiorazioneAutomatica(e.model.Macrouso_UMA_Cod, e.model.Lav_UMA_Cod)) {
        if ($(".Lav_Alt, .k-edit-cell").length > 0) {
            e.container.data('kendoTooltip').show();
        }
    }
}

function funzioneDaChiamareDopoDelete_Lavorazioni(e) {
    kendogridId = e.currentTarget.closest("div[data-role='grid']").id;
    $("#" + kendogridId).data("kendoGrid").refresh();
}

function grid_cellClose(e) {
    if (e.model.dirtyFields != undefined) {
        var gridID = e.sender.element[0].id;
        var grid = $("#" + gridID).data("kendoGrid");
        parentRow = e.sender.element.parents(".k-detail-row").prev();
        parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
        parentItem = parentGrid.dataItem(parentRow);
        var daCUAA = TrovaAziendaDaCUAA(e.model.CUAA);
        e.model.rag_soc = daCUAA.Item1;
        var parentRowItem = parentGrid.dataItem(parentRow);
        if (e.model.dirtyFields.Superficie_Trattata == true) {
            if (e.model.Programmazione_Cod > 0 && (e.model.Superficie_Trattata < 0 || e.model.Superficie_Trattata == null || e.model.Superficie_Trattata > parentRowItem.sup_UMA)) { e.model.Superficie_Trattata = parentRowItem.sup_UMA; }
            aggiornataSup_TotaleLavorazioni_Edit(e, parentItem);
            calcoloFabbisogno(parentRowItem, e.model, e.sender);
            e.model.dirtyFields.Superficie_Trattata = false;
            grid.refresh();
        } else if (e.model.dirtyFields.nLavRichieste == true) {
            if (e.model.nLavRichieste < 0 || e.model.nLavRichieste == null) { e.model.nLavRichieste = 1; }
            calcoloFabbisogno(parentRowItem, e.model, e.sender);
            e.model.dirtyFields.nLavRichieste = false;
        } else if (e.model.dirtyFields.Sup_A == true || e.model.dirtyFields.Sup_B == true) {
            if (e.model.dirtyFields.Sup_A == true) {
                if (e.model.Sup_A < 0 || e.model.Sup_A == null) { e.model.Sup_A = 0; }
                e.model.Sup_B = e.model.Superficie_Trattata - e.model.Sup_A;
            } else if (e.model.dirtyFields.Sup_B == true) {
                if (e.model.Sup_B < 0 || e.model.Sup_B == null) { e.model.Sup_B = 0; }
                e.model.Sup_A = e.model.Superficie_Trattata - e.model.Sup_B;
            }
            if (e.model.Sup_A == null) {
                e.model.Sup_A = 0;
            } else if (e.model.Sup_B == null) {
                e.model.Sup_B = 0;
            }
            e.model.dirtyFields.Sup_A = false;
            e.model.dirtyFields.Sup_B = false;
        } else if (e.model.dirtyFields.TerrenoNormale == true || e.model.dirtyFields.TerrenoMedio == true || e.model.dirtyFields.TerrenoTenace == true) {
            //e.model.Superficie_Trattata = e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace;
            if (e.model.dirtyFields.TerrenoNormale == true) {
                if (e.model.TerrenoNormale < 0 || e.model.TerrenoNormale == null) { e.model.TerrenoNormale = 0; }
                if (e.model.TerrenoMedio != 0 && e.model.TerrenoTenace == 0) {
                    e.model.TerrenoMedio = e.model.Superficie_Trattata - e.model.TerrenoNormale;
                } else if (e.model.TerrenoMedio == 0 && e.model.TerrenoTenace != 0) {
                    e.model.TerrenoTenace = e.model.Superficie_Trattata - e.model.TerrenoNormale;
                }
            } else if (e.model.dirtyFields.TerrenoMedio == true) {
                if (e.model.TerrenoMedio < 0 || e.model.TerrenoMedio == null) { e.model.TerrenoMedio = 0; }
                if (e.model.TerrenoNormale != 0 && e.model.TerrenoTenace == 0) {
                    e.model.TerrenoNormale = e.model.Superficie_Trattata - e.model.TerrenoMedio;
                } else if (e.model.TerrenoNormale == 0 && e.model.TerrenoTenace != 0) {
                    e.model.TerrenoTenace = e.model.Superficie_Trattata - e.model.TerrenoMedio;
                }
            } else {
                if (e.model.TerrenoTenace < 0 || e.model.TerrenoTenace == null) { e.model.TerrenoTenace = 0; }
                if (e.model.TerrenoNormale != 0 && e.model.TerrenoMedio == 0) {
                    e.model.TerrenoNormale = e.model.Superficie_Trattata - e.model.TerrenoTenace;
                } else if (e.model.TerrenoNormale == 0 && e.model.TerrenoMedio != 0) {
                    e.model.TerrenoMedio = e.model.Superficie_Trattata - e.model.TerrenoTenace;
                }
            }
            e.model.dirtyFields.TerrenoNormale = false;
            e.model.dirtyFields.TerrenoMedio = false;
            e.model.dirtyFields.TerrenoTenace = false;
        } else if (e.model.dirtyFields.Qta_Manuale) {
            calcoloFabbisogno(parentRowItem, e.model, e.sender);
        } else if (e.model.dirtyFields.Mesi) {
            calcoloFabbisogno(parentRowItem, e.model, e.sender);
        } else if (e.model.dirtyFields.ltrichiesto) {
            if (e.model.ltrichiesto < 0 || e.model.ltrichiesto == null || e.model.ltrichiesto > e.model.fabbisognoCalc) { e.model.ltrichiesto = e.model.fabbisognoCalc; }
        } else if (e.model.dirtyFields.ltAssegnato == true) {
            if (e.model.ltAssegnato < 0 || e.model.ltAssegnato == undefined) e.model.ltAssegnato = 0;
            let RichiestoDecurtato = e.model.ltrichiesto - (e.model.ltrichiesto * Percentuale_Decurtamento / 100);
            if (e.model.ltAssegnato > RichiestoDecurtato) e.model.ltAssegnato = RichiestoDecurtato
            grid.refresh();
        } else if (e.model.dirtyFields.CUAA) {
            if (!checkRendicontazioneTerzista(e.model.Programmazione_Cod, e.model.Macrouso_UMA_Cod, e.model.Lav_UMA_Cod, e.model.CUAA)) {
                verifica_terzisti[e.model.Macrouso_UMA_Cod] = {};
                verifica_terzisti[e.model.Macrouso_UMA_Cod][e.model.LavUMA] = e.model.CUAA;
            }
            else {
                e.model.ltrichiesto = 0;
                e.model.fabbisognoCalc = 0;
                grid.refresh();
            }
        } else if (JSON.stringify(e.model.dirtyFields) == "{}") {
            //Do Nothing
            return;
        } else {
            grid.refresh();
        }
        if (e.model.Sup_A < 0 || e.model.Sup_A == null) { e.model.Sup_A = 0; }
        if (e.model.Sup_B < 0 || e.model.Sup_B == null) { e.model.Sup_B = 0; }
        if (e.model.TerrenoNormale < 0 || e.model.TerrenoNormale == null) { e.model.TerrenoNormale = 0; }
        if (e.model.TerrenoMedio < 0 || e.model.TerrenoMedio == null) { e.model.TerrenoMedio = 0; }
        if (e.model.TerrenoTenace < 0 || e.model.TerrenoTenace == null) { e.model.TerrenoTenace = 0; }
        aggiornataSup_TotaleLavorazioni_Edit(e, parentItem);
        grid.refresh();
    }
}

function aggiornataSup_Totale_Edit2(e) {
    var sup_UMA = e.model.sup_UMA_Edit;
    var sup_UMA_A = e.model.sup_UMA_A_Edit;
    var sup_UMA_B = e.model.sup_UMA_B_Edit;
    var TerrenoTenace = e.model.TerrenoTenace_Edit;
    var TerrenoMedio = e.model.TerrenoMedio_Edit;
    var TerrenoNormale = e.model.TerrenoNormale_Edit;


    var diff = parseFloat(((TerrenoNormale + TerrenoMedio + TerrenoTenace) - sup_UMA).toFixed(4));
    if (diff < 0) {
        diff = (diff * -1);
    }
    //}

    var diffresiduapendenza = 0;
    var diffresidua = 0;
    var diffresiduamedia = 0;

    //****************GESTIONE DELLA PENDENZA DEL TERRENO*************
    if (sup_UMA != sup_UMA_A + sup_UMA_B) {
        if (sup_UMA_A > 0 || sup_UMA_B > 0) {
            if (sup_UMA < (sup_UMA_A + sup_UMA_B)) {
                if (sup_UMA_B >= diff) {
                    e.model.sup_UMA_B_Edit = parseFloat((sup_UMA_B - diff).toFixed(4));
                } else {
                    diffresiduapendenza = (diff - e.model.sup_UMA_B).toFixed(4);
                    e.model.sup_UMA_B_Edit = 0;
                    e.model.sup_UMA_A_Edit = parseFloat((sup_UMA_A - diffresiduapendenza).toFixed(4));
                }
            } else if (sup_UMA > (sup_UMA_A + sup_UMA_B)) {
                if (sup_UMA_A + diff <= e.model.sup_UMA_A) {
                    e.model.sup_UMA_A_Edit = parseFloat((sup_UMA_A + diff).toFixed(4));
                } else {
                    diffresiduapendenza = (diff - sup_UMA_A).toFixed(4);
                    e.model.sup_UMA_A_Edit = sup_UMA_A;
                    if ((e.model.sup_UMA_B_Edit + diffresiduapendenza) <= sup_UMA_B) {
                        e.model.sup_UMA_B_Edit = parseFloat((sup_UMA_B_Edit + diffresiduapendenza));
                    } else {
                        e.model.sup_UMA_B_Edit = sup_UMA_B;
                    }
                }
            }
        }
    }

    //****************FINE GESTIONE DELLA PENDENZA DEL TERRENO*************

    if (sup_UMA != (TerrenoNormale + TerrenoMedio + TerrenoTenace).toFixed(4)) {

        if (sup_UMA < (TerrenoNormale + TerrenoMedio + TerrenoTenace)) {
            if (TerrenoTenace >= diff) {
                e.model.TerrenoTenace_Edit = Math.max(0, TerrenoTenace - diff);
            } else {
                diffresidua = parseFloat((diff - TerrenoTenace).toFixed(4));
                e.model.TerrenoTenace_Edit = 0;
                if (TerrenoMedio == 0) {
                    if (TerrenoNormale > 0) {
                        e.model.TerrenoNormale_Edit = Math.max(0, TerrenoNormale - diffresidua);
                    }
                } else if (TerrenoMedio > 0) {
                    if (TerrenoMedio >= diffresidua) {
                        e.model.TerrenoMedio_Edit = Math.max(0, TerrenoMedio - diffresidua);
                    } else if (TerrenoMedio < diffresidua) {
                        diffresiduamedia = (diffresidua - TerrenoMedio).toFixed(4);
                        e.model.TerrenoMedio_Edit = Math.max(0, TerrenoMedio - diffresidua);
                        if (TerrenoNormale > 0) {
                            if (TerrenoNormale >= diffresiduamedia) {
                                e.model.TerrenoNormale_Edit = Math.max(0, TerrenoNormale - diffresiduamedia);
                            } else if (TerrenoNormale < diffresiduamedia) {
                                e.model.TerrenoNormale_Edit = 0;
                            }
                        }
                    }
                }
            }
        } else if (sup_UMA > (TerrenoNormale + TerrenoMedio + TerrenoTenace)) {
            if ((TerrenoNormale + diff) <= e.model.TerrenoNormale) {
                e.model.TerrenoNormale_Edit = parseFloat((TerrenoNormale + diff).toFixed(4));
            } else {
                diffresidua = (diff - (e.model.TerrenoNormale - TerrenoNormale)).toFixed(4);
                e.model.TerrenoNormale_Edit = e.model.TerrenoNormale;
                if ((TerrenoMedio + parseFloat(diffresidua)) <= e.model.TerrenoMedio) {
                    e.model.TerrenoMedio_Edit = parseFloat((TerrenoMedio + parseFloat(diffresidua)));
                } else {
                    diffresiduamedia = (diffresidua - (TerrenoMedio - e.model.TerrenoMedio)).toFixed(4);
                    e.model.TerrenoMedio_Edit = e.model.TerrenoMedio;
                    if ((TerrenoTenace + parseFloat(diffresiduamedia)) <= e.model.TerrenoTenace) {
                        e.model.TerrenoTenace_Edit = parseFloat((TerrenoTenace + parseFloat(diffresiduamedia)));
                    } else {
                        e.model.TerrenoTenace_Edit = e.model.TerrenoTenace;
                    }
                }
            }
        }


    }
}

function aggiornataSup_TotaleLavorazioni_Edit(e, parentRow) {
    var sup_UMA = parentRow.sup_UMA_Edit;
    var sup_UMA_A = parentRow.sup_UMA_A_Edit;
    var sup_UMA_B = parentRow.sup_UMA_B_Edit;
    var TerrenoTenace = parentRow.TerrenoTenace_Edit;
    var TerrenoMedio = parentRow.TerrenoMedio_Edit;
    var TerrenoNormale = parentRow.TerrenoNormale_Edit;


    var diff = parseFloat(((e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace) - e.model.Superficie_Trattata).toFixed(4));
    if (diff < 0) {
        diff = (diff * -1);
    }
    //}

    var diffresiduapendenza = 0;
    var diffresidua = 0;
    var diffresiduamedia = 0;

    //****************GESTIONE DELLA PENDENZA DEL TERRENO*************
    if (e.model.Superficie_Trattata != e.model.Sup_A + e.model.Sup_B) {
        if (e.model.Sup_A > 0 || e.model.Sup_B > 0) {
            if (e.model.Superficie_Trattata < (e.model.Sup_A + e.model.Sup_B)) {
                if (e.model.Sup_B >= diff) {
                    e.model.Sup_B = parseFloat((e.model.Sup_B - diff).toFixed(4));
                } else {
                    diffresiduapendenza = (diff - e.model.Sup_B).toFixed(4);
                    e.model.Sup_B = 0;
                    e.model.Sup_A = parseFloat((e.model.Sup_A - diffresiduapendenza).toFixed(4));
                }
            } else if (e.model.Superficie_Trattata > (e.model.Sup_A + e.model.Sup_B)) {
                if (e.model.Sup_A + diff <= sup_UMA_A) {
                    e.model.Sup_A = parseFloat((e.model.Sup_A + diff).toFixed(4));
                } else {
                    diffresiduapendenza = (diff - (sup_UMA_A - e.model.Sup_A)).toFixed(4);
                    e.model.Sup_A = sup_UMA_A;
                    if ((e.model.Sup_B + diffresiduapendenza) <= sup_UMA_B) {
                        e.model.Sup_B = parseFloat((e.model.Sup_B + diffresiduapendenza));
                    } else {
                        e.model.Sup_B = sup_UMA_B;
                    }
                }
            }
        }
    }

    //****************FINE GESTIONE DELLA PENDENZA DEL TERRENO*************

    if (e.model.Superficie_Trattata != (e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace).toFixed(4)) {

        if (e.model.Superficie_Trattata < (e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace)) {
            if (e.model.TerrenoTenace >= diff) {
                e.model.TerrenoTenace = Math.max(0, e.model.TerrenoTenace - diff);
            } else {
                diffresidua = parseFloat((diff - e.model.TerrenoTenace).toFixed(4));
                e.model.TerrenoTenace = 0;
                if (e.model.TerrenoMedio == 0) {
                    if (e.model.TerrenoNormale > 0) {
                        e.model.TerrenoNormale = Math.max(0, e.model.TerrenoNormale - diffresidua);
                    }
                } else if (e.model.TerrenoMedio > 0) {
                    if (e.model.TerrenoMedio >= diffresidua) {
                        e.model.TerrenoMedio = Math.max(0, e.model.TerrenoMedio - diffresidua);
                    } else if (e.model.TerrenoMedio < diffresidua) {
                        diffresiduamedia = (diffresidua - e.model.TerrenoMedio).toFixed(4);
                        e.model.TerrenoMedio = Math.max(0, e.model.TerrenoMedio - diffresidua);
                        if (e.model.TerrenoNormale > 0) {
                            if (e.model.TerrenoNormale >= diffresiduamedia) {
                                e.model.TerrenoNormale = Math.max(0, e.model.TerrenoNormale - diffresiduamedia);
                            } else if (e.model.TerrenoNormale < diffresiduamedia) {
                                e.model.TerrenoNormale = 0;
                            }
                        }
                    }
                }
            }
        } else if (e.model.Superficie_Trattata > (e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace)) {
            if ((e.model.TerrenoNormale + diff) <= TerrenoNormale) {
                e.model.TerrenoNormale = parseFloat((e.model.TerrenoNormale + diff).toFixed(4));
            } else {
                diffresidua = (diff - (TerrenoNormale - e.model.TerrenoNormale)).toFixed(4);
                e.model.TerrenoNormale = TerrenoNormale;
                if ((e.model.TerrenoMedio + diffresidua) <= TerrenoMedio) {
                    e.model.TerrenoMedio = parseFloat((e.model.TerrenoMedio + diffresidua));
                } else {
                    diffresiduamedia = (diffresidua - (TerrenoMedio - e.model.TerrenoMedio)).toFixed(4);
                    e.model.TerrenoMedio = TerrenoMedio;
                    if ((e.model.TerrenoTenace + diffresiduamedia) <= TerrenoTenace) {
                        e.model.TerrenoTenace = parseFloat((e.model.TerrenoTenace + diffresiduamedia));
                    } else {
                        e.model.TerrenoTenace = TerrenoTenace;
                    }
                }
            }
        }


    }
}

function lavUMA_DropDownEditor(container, options) {
    let parentRow = $($(container).parents(".k-detail-row")[0]).prev();
    let parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    PopolaElencoLavUMA(false, parentRowItem.Macrouso_UMA_Cod, 0, parentRowItem.Regolamento_Cod);
    //var Elen = elencoLavUMA.filter(function () { return elencoLavUMA.Regolamento_Cod === 4 })
    creaDropDownEditor(container, "LavUMA", "Lav_UMA_Cod", elencoLavUMA, changelavUMA);
}

function changelavUMA(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.LavUMA = dataItem.LavUMA;
    model.Lav_UMA_Cod = dataItem.Lav_UMA_Cod;

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    PopolaElencoLavGIAS(false, parentRowItem.Macrouso_UMA_Cod, model.Lav_UMA_Cod, parentRowItem.Regolamento_Cod);

    if (elencoLavGIAS.length > 0) {
        model.LavGIAS = elencoLavGIAS[0].LavGIAS;
        model.LAV_COD = elencoLavGIAS[0].LAV_COD;
        PopolaElencoAttivitaGIAS(false, parentRowItem.Macrouso_UMA_Cod, model.Lav_UMA_Cod, model.LAV_COD, parentRowItem.Regolamento_Cod).then(
            elencoAttivita => {
                if (elencoAttivita.length == 0) {
                    elencoAttivita.push({
                        Attivita_Cod: 0,
                        Attivita_Des: ''
                    });
                }
                model.Attivita_Des = elencoAttivita[0].Attivita_Des;
                model.Attivita_Cod = elencoAttivita[0].Attivita_Cod;

                //let costo = tabellaCalcoloCosti.filter((elem) => {
                //    return elem.Macrouso_UMA_Cod == parentRowItem.Macrouso_UMA_Cod && elem.Lav_UMA_Cod == model.Lav_UMA_Cod && elem.Id_Attivita == model.Attivita_Cod;
                //})[0];
                let costo = filtraTabellaCalcoloCosti(parentRowItem.Macrouso_UMA_Cod, model.Lav_UMA_Cod, null, model.Attivita_Cod, parentRowItem.Regolamento_Cod);

                if (costo && costo.Udm_Alternativa && costo.Udm_Alternativa !== "") {
                    model.Udm_Alt = costo.Udm_Alternativa;
                    model.Qta_Manuale = 0;
                }

                calcoloFabbisogno(parentRowItem, model, grid);

                grid.refresh();
            }
        );
    }

    model.nLavPreviste = recupera_nLavPreviste(model.Lav_UMA_Cod, parentRowItem.Macrouso_UMA_Cod, model.LAV_COD, parentRowItem.Regolamento_Cod);
    model.nLavRichieste = 1; //model.nLavPreviste
    model.dirty = true;
    calcoloFabbisogno(parentRowItem, model, grid);

    grid.refresh();
    //kendoFastRedrawRow(grid, row);
}

function Maggiorazione_DropDownEditor(container, options) {
    creaDropDownEditor(container, "valore", "codice", [{ codice: 0, valore: "No" }, { codice: 1, valore: "Sì" }], changeMaggiorazione);
}

function changeMaggiorazione(e) {
    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.SupMaggiorazioneTrasferimenti = dataItem.codice;
    model.dirty = true;

    calcoloFabbisogno(parentRowItem, model, grid);
}

function Carburanti_DropDownEditor(container, options) {
    creaDropDownEditor(container, "TipoCarb", "TipoCarb", elencoCarburanti, changeCarb);
}

function changeCarb(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.TipoCarb = dataItem.TipoCarb;
    model.Car_Cod = dataItem.Car_Cod;
    model.dirty = true;

    calcoloFabbisogno(parentRowItem, model, grid);
    //kendoFastRedrawRow(grid, row);
}

function lavGIAS_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    PopolaElencoLavGIAS(false, row.Macrouso_UMA_Cod, row.Lav_UMA_Cod, row.Regolamento_Cod);

    creaDropDownEditor(container, "LavGIAS", "Operazione", elencoLavGIAS, changelavGIAS);

    //Per Altre Lavorazioni carico anche l'attività
    if (elencoLavGIAS.length > 0 && elencoLavGIAS[0].LAV_COD == 162) {
        PopolaElencoAttivitaGIAS(false, row.Macrouso_UMA_Cod, row.Lav_UMA_Cod, elencoLavGIAS[0].LAV_COD, row.Regolamento_Cod).then(
            elencoAttivita => {
                if (elencoAttivita.length == 0) {
                    elencoAttivita.push({
                        Attivita_Cod: 0,
                        Attivita_Des: ''
                    });
                }
                creaDropDownEditor(container, "Attivita_Des", "Attività", elencoAttivita, changeAttivitaGIAS);
            }
        );
    }

}

function changelavGIAS(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.LavGIAS = dataItem.LavGIAS;
    model.LAV_COD = dataItem.LAV_COD;
    model.dirty = true;
    //kendoFastRedrawRow(grid, row);
    calcoloFabbisogno(parentRowItem, model, grid);
}

function AttivitaGIAS_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    PopolaElencoAttivitaGIAS(false, row.Macrouso_UMA_Cod, row.Lav_UMA_Cod, row.LAV_COD, row.Regolamento_Cod).then(
        elencoAttivita => {
            if (elencoAttivita.length == 0) {
                elencoAttivita.push({
                    Attivita_Cod: 0,
                    Attivita_Des: ''
                });
            }

            creaDropDownEditor(container, "Attivita_Des", "Attività", elencoAttivita, changeAttivitaGIAS);
        }
    );
}

function changeAttivitaGIAS(e) {
    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.Attivita_Des = dataItem.Attivita_Des;
    model.Attivita_Cod = dataItem.Attivita_Cod;
    model.dirty = true;
}

function Programmazione_Des_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (row.UMA_Cod != "" &&
            row.Programmazione_Cod != 0 &&
            row.Richiesta_Cod != 0) {
            return;
        }

        WaitFrame.show();

        PopolaElencoProgrammazione_Des().then(
            elencoProgrammazione_Des => {
                WaitFrame.hide();
                creaDropDownEditor(container, "Programmazione_Des", "Programmazione_Des", elencoProgrammazione_Des, changeProgrammazione_Des);
            }
        );
    }
}

function changeProgrammazione_Des(e) {

    var dataItem = e.sender.dataItem();
    //var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Programmazione_Cod = dataItem.Programmazione_Cod;
    model.Programmazione_Des = dataItem.Programmazione_Des;
    model.Macrouso_UMA_Cod = '';
    model.Gruppo_Colturale_UMA = '';
    model.dirty = true;
    grid.refresh();
}

function MacrousoUMA_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (row.UMA_Cod != "" &&
            row.Programmazione_Cod != 0 &&
            row.Richiesta_Cod != 0) {
            return;
        }

        WaitFrame.show();

        Leggi_UMA_Macrousi(row.Programmazione_Cod).then(
            elencoMacrousi_Des => {
                WaitFrame.hide();
                creaDropDownEditor(container, "Macrouso_UMA_Des", "Coltura", elencoMacrousi_Des, changeMacrouso_UMA);
            }
        );
    }
}

function changeMacrouso_UMA(e) {

    var dataItem = e.sender.dataItem();
    //var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    if (dataItem.sup_A != undefined) {
        model.sup_UMA = dataItem.sup_A + dataItem.sup_B;
        model.sup_UMA_Edit = dataItem.sup_A + dataItem.sup_B;

        model.sup_UMA_A = dataItem.sup_A;
        model.sup_UMA_A_Edit = dataItem.sup_A;
        model.sup_UMA_B = dataItem.sup_B;
        model.sup_UMA_B_Edit = dataItem.sup_B;

        model.TerrenoMedio = dataItem.tessitura_Media;
        model.TerrenoMedio_Edit = dataItem.tessitura_Media;
        model.TerrenoNormale = dataItem.tessitura_Norm;
        model.TerrenoNormale_Edit = dataItem.tessitura_Norm;
        model.TerrenoTenace = dataItem.tessitura_Tenace;
        model.TerrenoTenace_Edit = dataItem.tessitura_Tenace;
    }

    model.Macrouso_UMA_Cod = dataItem.Macrouso_UMA_Cod;
    model.UMA_Cod = dataItem.Macrouso_UMA_Cod;
    model.Gruppo_Colturale_UMA = dataItem.Macrouso_UMA_Des;
    model.dirty = true;
    grid.refresh();
}

async function AggiornaDati() {

    if (QS_Anticipo == 1) {
        WaitFrame.show();

        if (ControllaPercentualiCarburanti($("#gasolioTerzisti")[0].value, $("#benzinaTerzisti")[0].value, $("#gasolioSerraTerzisti")[0].value)) {

            $("#row_MsgErroreModifica").attr("style", "display:none");

            await SalvaAnticipi();

            WaitFrame.hide();

            return true;
        }
        else {
            $("#row_MsgErroreModifica").attr("style", "display:block");
            $(".msgErrorAnticipoModifica").show();
            $(".msgErrorAnticipoModifica").html("La percentuale inserita non può essere superiore al " + (Percentuale_Anticipo_Carb * 100) + "%");

            WaitFrame.hide();

            return false
        }
    }

    if (await controlliVari()) {
        WaitFrame.show();

        await aggiornaTestata(KendoDDL("ddlAzienda").value(), richiesta_cod, TxtRimanenza_Gasolio.value(), TxtRimanenza_Benzina.value(), TxtRimanenza_Gasolio_Serra.value());

        if (QS_Type == 0) {
            await aggiornaPermessiAcqua(KendoDDL("ddlAzienda").value(), richiesta_cod, $("#permessoAcqua").val(), $("#notePermessoAcqua").val())
        }

        if (QS_Avanzamento === 1 && getKendoSwitch("noRichiestaAnnoProxCheck") !== Boolean(noRichiestaSuccessiva)) {
            await salvaNoProssimaRichiesta(getKendoSwitch("noRichiestaAnnoProxCheck"));
        }

        if ($("#tab_griglia_macchine").find(".errorCell").length != 0) {
            kendo.alert("Verificare i dati segnalati prima di salvare");
            return false;
        }

        //Salvo richiesta
        if (QS_Type == 0) {

            //--------------------------------------------------------------------------------
            //Conto Proprio
            //--------------------------------------------------------------------------------

            var data = $("#tab_griglia_dettagliImpianti").data("kendoGrid").dataSource.data();

            await AggiornaRichieste_UMA(KendoDDL("ddlAzienda").value(), richiesta_cod, JSON.stringify(data));

            var tabelleAperte = $("#tab_griglia_dettagliImpianti").find("div[id^=GrigliaDettagliLavorazioni]");

            for (var i = 0; i < tabelleAperte.length; i++) {
                var datiA = $("#" + tabelleAperte[i].id).data("kendoGrid").dataSource.data();

                let parentRow = $($(tabelleAperte[i]).parents(".k-detail-row")[0]).prev()
                let parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
                var parentRowItem = parentGrid.dataItem(parentRow);

                let created = datiA.filter((el) => { return el.dirty == true && (el.dirtyFields.Validita_Inizio == true || QS_Avanzamento == 0) && el.richiestaDettaglioCod == 0 && (!el.deleted) });
                let updated = datiA.filter((el) => { return el.dirty == true && el.richiestaDettaglioCod != 0 && (!el.deleted) });
                let destroyed = datiA.filter((el) => { return el.deleted == true });

                let modificheFatte = false;
                let Macrouso_UMA_Cod = "";
                let Programmazione_Cod = 0;
                if (created.length > 0) {
                    modificheFatte = true;
                    Macrouso_UMA_Cod = created[0].Macrouso_UMA_Cod;
                    Programmazione_Cod = created[0].Programmazione_Cod;
                }
                if (updated.length > 0) {
                    modificheFatte = true;
                    Macrouso_UMA_Cod = updated[0].Macrouso_UMA_Cod;
                    Programmazione_Cod = updated[0].Programmazione_Cod;
                }
                if (destroyed.length > 0) {
                    modificheFatte = true;
                    Macrouso_UMA_Cod = destroyed[0].Macrouso_UMA_Cod;
                    Programmazione_Cod = destroyed[0].Programmazione_Cod;
                }
                if (modificheFatte) {
                    await ws_Inserisci_Lavorazioni(KendoDDL("ddlAzienda").value(),
                        richiesta_cod,
                        Programmazione_Cod,
                        Macrouso_UMA_Cod,
                        created,
                        updated,
                        destroyed,
                        parentRowItem.Regolamento_Cod);
                }

            }

            // Salvo Richieste Allevamenti
            await ucUmaAllevamenti_AggiornaAllevamenti().then(
                function (ricaricaGriglia) {
                    // Callback resolve
                    if (ricaricaGriglia === true) {
                        ucUmaAllevamenti_reloadGrid();
                    }
                },
                function (mesErrore) {
                    // Callback reject
                    kendo.alert(mesErrore);
                }
            );

            await LeggiRichiesta(false);
            ConfiguraGrigliaDettagliImpianti("tab_griglia_dettagliImpianti", false);

            //Salva Gestione Rimanenze
            if (gestioneRimanenzeAbilitata() === true) {
                await salvaGestioneRimanenzeTestata();
            }

            await SalvaCancellazioniEModificheAllevamentiUF();
            await SalvaAllevatiInMontagna();
            await ControlloCapiAllevabiliUF();
        }

        else if (QS_Type == -1) {

            //--------------------------------------------------------------------------------
            //Conto Terzi
            //--------------------------------------------------------------------------------

            // Caso 1 : Non è una richiesta OPPURE è una richiesta di azienda cooperativa

            if ((QS_Avanzamento != 0) || (QS_Avanzamento == 0 && QS_TipoAzienda == Cooperativa_Agricola)) {
                if ($("#tab_griglia_terzisti").find(".errorCell").length != 0) {
                    kendo.alert("Verificare i dati segnalati prima di salvare");
                    return false;
                }
                if (richiesta_cod > -1 && QS_Avanzamento == 0) {
                    //CONTROLLO LT RICHIESTI
                    let totaleCarb = parseInt($("#benzinaTerzisti")[0].value) + parseInt($("#gasolioTerzisti")[0].value) + parseInt($("#gasolioSerraTerzisti")[0].value);
                    let totCarbs = parseInt(calcTotaleColonna("ltrichiesto", $("#tab_griglia_terzisti")[0], false));
                    if (totaleCarb < totCarbs) {
                        kendo.alert("Il totale dei litri presenti nella richiesta iniziale sono inferiori di quelli specificati nelle lavorazioni");
                        return false;
                        //gasolioTerz.value(gasolioTerz.value() + (totCarbs - totaleCarb));
                    }
                    let totaleCarbAppro = parseInt($("#gasolioTerzistiAppro")[0].value) + parseInt($("#benzinaTerzistiAppro")[0].value) + parseInt($("#gasolioSerraTerzistiAppro")[0].value);
                    let totCarbsAppro = parseInt(calcTotaleColonna("ltAssegnato", $("#tab_griglia_terzisti")[0], false));
                    if (totaleCarbAppro < totCarbsAppro) {
                        //kendo.alert("Il totale dei litri presenti nella richiesta iniziale approvata sono inferiori di quelli assegnati nelle lavorazioni");
                        //return false;
                        //gasolioTerz.value(gasolioTerz.value() + (totCarbs - totaleCarb));
                    } else if (totaleCarbAppro > Math.round(totaleCarb * (100 - Percentuale_Decurtamento) / 100)) {
                        kendo.alert("Il totale dei litri presenti nella richiesta iniziale approvata sono superiori al totale di quelli richiesti meno il " + Percentuale_Decurtamento + "% ");
                        return false;
                    }
                    await AggiornaRichiesteCarburanti(KendoDDL("ddlAzienda").value(), $("#benzinaTerzisti")[0].value, $("#gasolioTerzisti")[0].value, $("#gasolioSerraTerzisti")[0].value, $("#gasolioTerzistiAppro")[0].value, $("#benzinaTerzistiAppro")[0].value, $("#gasolioSerraTerzistiAppro")[0].value);
                }
                var data = $("#tab_griglia_terzisti").data("kendoGrid").dataSource.data();

                let destroyed = data.filter((el) => { return el.deleted == true || el.cancellato == true });

                let valid = true;
                let created = data.filter((el) => {
                    return el.dirty == true && (el.Richiesta_Cod == -1 || el.id === "") &&
                        (destroyed.filter((d) => { return d.uid == el.uid }).lenght == 0 || destroyed.filter((d) => { return d.uid == el.uid }).lenght == undefined);
                });

                //AGGIUNTO GLORIA PER FORZARE IL SALVATAGGIO DELLA SOLA AZIENDA

                if (QS_Avanzamento == 1 && QS_Type == -1) {
                    created.forEach(cr => { if (cr.Macrouso_UMA_Cod == undefined) cr.Macrouso_UMA_Cod = "0000" })
                    created.forEach(cr => { if (cr.programmazione_cod == undefined) cr.programmazione_cod = 0 })

                    if (created.programmazione_cod == undefined) {
                        created.programmazione_cod = 0;
                    }
                }

                ///FINE

                created.forEach(cr => cr.Macrouso_UMA_Cod == -1 || cr.Macrouso_UMA_Cod == undefined ? valid = false : valid = true)
                if (!valid) {
                    kendo.alert("Specificare il Gruppo Colturale per ogni richiesta prima di proseguire");
                    return false;
                }

                let updated = data.filter((el) => {
                    return el.dirty == true && el.Richiesta_Cod != -1 && el.id !== "" &&
                        (destroyed.filter((d) => { return d.uid == el.uid }).lenght == 0 || destroyed.filter((d) => { return d.uid == el.uid }).lenght == undefined);
                });

                let modificheFatte = false;
                let r_cod = 0;
                if (created.length > 0) {
                    modificheFatte = true;

                    for (var i = 0; i < created.length; i++)
                        created[i].Richiesta_Cod = QS_Richiesta > 0 ? QS_Richiesta : richiesta_cod;
                    r_cod = created[0].Richiesta_Cod;

                    //Richiesta_Cod = e.data.created[0].Richiesta_Cod;
                    //Macrouso_UMA_Cod = e.data.created[0].Macrouso_UMA_Cod;
                }
                if (updated.length > 0) {
                    modificheFatte = true;
                    r_cod = updated[0].Richiesta_Cod;
                    //Richiesta_Cod = e.data.updated[0].Richiesta_Cod;
                    //Macrouso_UMA_Cod = e.data.updated[0].Macrouso_UMA_Cod;
                }
                if (destroyed.length > 0) {
                    modificheFatte = true;
                    r_cod = destroyed[0].Richiesta_Cod;
                    //Richiesta_Cod = e.data.destroyed[0].Richiesta_Cod;
                    //Macrouso_UMA_Cod = e.data.destroyed[0].Macrouso_UMA_Cod;
                }
                if (modificheFatte) {
                    await ws_Inserisci_Richieste_Terzisti(KendoDDL("ddlAzienda").value(), r_cod, created, updated, destroyed);
                }

                var tabelleAperte = $("#tab_griglia_terzisti").find("div[id^=GrigliaDettagliLavorazioni]");

                for (var i = 0; i < tabelleAperte.length; i++) {
                    var datiA = $("#" + tabelleAperte[i].id).data("kendoGrid").dataSource.data();

                    let parentRow = $($(tabelleAperte[i]).parents(".k-detail-row")[0]).prev()
                    let parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
                    var parentRowItem = parentGrid.dataItem(parentRow);

                    let destroyed = datiA.filter((el) => { return el.deleted == true });
                    let created = datiA.filter((el) => {
                        return el.dirty == true && el.richiestaDettaglioCod == 0 && (el.dirtyFields.Validita_Inizio == true || QS_Avanzamento == 0) &&
                            (destroyed.filter((d) => { return d.uid == el.uid }).lenght == 0 || destroyed.filter((d) => { return d.uid == el.uid }).lenght == undefined);
                    });
                    let updated = datiA.filter((el) => {
                        return el.dirty == true && el.richiestaDettaglioCod != 0 &&
                            (destroyed.filter((d) => { return d.uid == el.uid }).lenght == 0 || destroyed.filter((d) => { return d.uid == el.uid }).lenght == undefined);
                    });

                    modificheFatte = false;
                    let piva = '0';
                    let Macrouso_UMA_Cod = "";
                    let progr_cod = 0;
                    if (created.length > 0) {
                        modificheFatte = true;
                        piva = created[0].Piva;
                        Macrouso_UMA_Cod = created[0].Macrouso_UMA_Cod;
                        progr_cod = created[0].Programmazione_Cod;
                        r_cod = created[0].Richiesta_Cod;
                        colt_elem_orig[created[0].Macrouso_UMA_Cod] += colt_elem_added[created[0].Macrouso_UMA_Cod];
                        colt_elem_added[created[0].Macrouso_UMA_Cod] = 0;
                    }
                    if (updated.length > 0) {
                        modificheFatte = true;
                        piva = updated[0].Piva;
                        Macrouso_UMA_Cod = updated[0].Macrouso_UMA_Cod;
                        progr_cod = updated[0].Programmazione_Cod;
                        r_cod = updated[0].Richiesta_Cod;
                    }
                    if (destroyed.length > 0) {
                        modificheFatte = true;
                        piva = destroyed[0].Piva;
                        Macrouso_UMA_Cod = destroyed[0].Macrouso_UMA_Cod;
                        progr_cod = destroyed[0].Programmazione_Cod;
                        r_cod = destroyed[0].Richiesta_Cod;
                        colt_elem_orig[destroyed[0].Macrouso_UMA_Cod] -= destroyed.length;
                    }
                    for (var u = 0; u < updated.length; u++) {
                        var date = undefined //UpdateVal[updated[u].Piva][updated[u].Programmazione_Cod][updated[u].Macrouso_UMA_Cod][updated[u].LAV_COD]
                        updated[u].Validita_Inizio = date != undefined ? new Date(date) : updated[u].Validita_Inizio
                    }
                    if (modificheFatte) {
                        await ws_Inserisci_Lavorazioni(piva, r_cod, progr_cod, Macrouso_UMA_Cod, created, updated, destroyed, parentRowItem.Regolamento_Cod);
                    }
                }

                //Salva Gestione Rimanenze
                if (gestioneRimanenzeAbilitata() === true) {
                    await salvaGestioneRimanenzeTestata();
                }

                if (!consentitoAggiungereNuoviCUAA) {
                    //Resetto la lista dei CUAA ogni volta che l'utente preme salva
                    listaCUAARendicontati = []
                }



            }
            // Caso 2 : E' una richiesta di azienda non cooperativa
            else if (QS_Avanzamento == 0 && QS_TipoAzienda != Cooperativa_Agricola) {      ///inserimento lavorazioni solo con tipo lavorazione senza coltura e uma solo in caso di richiesta e terzista Gloria
                if ($("#tab_griglia_terzistiparziale").find(".errorCell").length != 0) {
                    kendo.alert("Verificare i dati segnalati prima di salvare");
                    return false;
                }
                if (richiesta_cod > -1 && QS_Avanzamento == 0) {
                    //CONTROLLO LT RICHIESTI
                    let totaleCarb = parseInt($("#benzinaTerzisti")[0].value) + parseInt($("#gasolioTerzisti")[0].value) + parseInt($("#gasolioSerraTerzisti")[0].value);
                    let totCarbs = parseInt(calcTotaleColonna("ltrichiesto", $("#tab_griglia_terzistiparziale")[0], false));
                    if (totaleCarb < totCarbs) {
                        kendo.alert("Il totale dei litri presenti nella richiesta iniziale sono inferiori di quelli specificati nelle lavorazioni");
                        return false;
                        //gasolioTerz.value(gasolioTerz.value() + (totCarbs - totaleCarb));
                    }
                    totaleCarb = parseInt($("#gasolioTerzistiAppro")[0].value) + parseInt($("#benzinaTerzistiAppro")[0].value) + parseInt($("#gasolioSerraTerzistiAppro")[0].value);
                    totCarbs = parseInt(calcTotaleColonna("ltAssegnato", $("#tab_griglia_terzistiparziale")[0], false));
                    if (totaleCarb < totCarbs) {
                        kendo.alert("Il totale dei litri presenti nella richiesta iniziale approvata sono inferiori di quelli assegnati nelle lavorazioni");
                        return false;
                        //gasolioTerz.value(gasolioTerz.value() + (totCarbs - totaleCarb));
                    }
                    aggiornaRichiestaInizialeParz();
                    await AggiornaRichiesteCarburanti(KendoDDL("ddlAzienda").value(), $("#benzinaTerzisti")[0].value, $("#gasolioTerzisti")[0].value, $("#gasolioSerraTerzisti")[0].value, $("#gasolioTerzistiAppro")[0].value, $("#benzinaTerzistiAppro")[0].value, $("#gasolioSerraTerzistiAppro")[0].value);
                }

                var tabellaLavorazioniParz = $("#tab_griglia_terzistiparziale").data("kendoGrid").dataSource.data();

                let destroyed = tabellaLavorazioniParz.filter((el) => { return el.deleted == true && el.cancellato == true });

                let valid = true;
                let created = tabellaLavorazioniParz.filter((el) => {
                    return el.dirty == true && el.Lavorazione_Parziale_Cod == 0 &&
                        (destroyed.filter((d) => { return d.uid == el.uid }).lenght == 0 || destroyed.filter((d) => { return d.uid == el.uid }).lenght == undefined);
                });

                let updated = tabellaLavorazioniParz.filter((el) => {
                    return el.dirty == true && el.Lavorazione_Parziale_Cod != 0 &&
                        (destroyed.filter((d) => { return d.uid == el.uid }).lenght == 0 || destroyed.filter((d) => { return d.uid == el.uid }).lenght == undefined);
                });

                let modificheFatte = false;
                let r_cod = 0;
                let r_LavorazioneP = 0;
                if (created.length > 0) {
                    modificheFatte = true;

                    for (var i = 0; i < created.length; i++)
                        created[i].Richiesta_Cod = QS_Richiesta > 0 ? QS_Richiesta : richiesta_cod;
                    r_cod = created[0].Richiesta_Cod;

                    //Richiesta_Cod = e.data.created[0].Richiesta_Cod;
                    //Macrouso_UMA_Cod = e.data.created[0].Macrouso_UMA_Cod;
                }
                if (updated.length > 0) {
                    modificheFatte = true;
                    r_cod = updated[0].Richiesta_Cod;
                    r_LavorazioneP = updated[0].Lavorazione_Parziale_Cod;
                    //Richiesta_Cod = e.data.updated[0].Richiesta_Cod;
                    //Macrouso_UMA_Cod = e.data.updated[0].Macrouso_UMA_Cod;
                }
                if (destroyed.length > 0) {
                    modificheFatte = true;
                    r_cod = destroyed[0].Richiesta_Cod;
                    r_LavorazioneP = destroyed[0].Lavorazione_Parziale_Cod;
                    //Richiesta_Cod = e.data.destroyed[0].Richiesta_Cod;
                    //Macrouso_UMA_Cod = e.data.destroyed[0].Macrouso_UMA_Cod;
                }


                if (modificheFatte) {
                    await ws_Inserisci_Lavorazioni_Parziali(KendoDDL("ddlAzienda").value(), r_cod, created, updated, destroyed);
                }

                //Salva Gestione Rimanenze
                if (gestioneRimanenzeAbilitata() === true) {
                    await salvaGestioneRimanenzeTestata();
                }

                await popolaGrigliaDettagliLavorazioniParzialiTerzisti(KendoDDL("ddlAzienda").value(), richiesta_cod);
                //}
            }

            // Caso 3 : E' una richiesta di azienda cooperativa
            else if (QS_Avanzamento == 0 && QS_TipoAzienda == Cooperativa_Agricola) {      ///inserimento lavorazioni solo con tipo lavorazione senza coltura e uma solo in caso di richiesta e terzista Gloria
                //var tabellaLavorazioni = $("#tab_griglia_terzisti").data("kendoGrid").dataSource.data();

                //let destroyed = tabellaLavorazioni.filter((el) => { return el.deleted == true && el.cancellato == true });

                //let valid = true;
                //let created = tabellaLavorazioni.filter((el) => {
                //    return el.dirty == true && el.id == "" &&
                //        (destroyed.filter((d) => { return d.uid == el.uid }).lenght == 0 || destroyed.filter((d) => { return d.uid == el.uid }).lenght == undefined);
                //});

                //let updated = tabellaLavorazioni.filter((el) => {
                //    return el.dirty == true && el.id != "" &&
                //        (destroyed.filter((d) => { return d.uid == el.uid }).lenght == 0 || destroyed.filter((d) => { return d.uid == el.uid }).lenght == undefined);
                //});

                //let modificheFatte = false;
                //let r_cod = 0;
                //let r_LavorazioneP = 0;
                //if (created.length > 0) {
                //    modificheFatte = true;

                //    for (var i = 0; i < created.length; i++)
                //        created[i].Richiesta_Cod = QS_Richiesta > 0 ? QS_Richiesta : richiesta_cod;
                //    r_cod = created[0].Richiesta_Cod;

                //    //Richiesta_Cod = e.data.created[0].Richiesta_Cod;
                //    //Macrouso_UMA_Cod = e.data.created[0].Macrouso_UMA_Cod;
                //}
                //if (updated.length > 0) {
                //    modificheFatte = true;
                //    r_cod = updated[0].Richiesta_Cod;
                //    r_LavorazioneP = updated[0].Lavorazione_Parziale_Cod;
                //    //Richiesta_Cod = e.data.updated[0].Richiesta_Cod;
                //    //Macrouso_UMA_Cod = e.data.updated[0].Macrouso_UMA_Cod;
                //}
                //if (destroyed.length > 0) {
                //    modificheFatte = true;
                //    r_cod = destroyed[0].Richiesta_Cod;
                //    r_LavorazioneP = destroyed[0].Lavorazione_Parziale_Cod;
                //    //Richiesta_Cod = e.data.destroyed[0].Richiesta_Cod;
                //    //Macrouso_UMA_Cod = e.data.destroyed[0].Macrouso_UMA_Cod;
                //}


                //if (modificheFatte) {
                //    await ws_Inserisci_Lavorazioni(KendoDDL("ddlAzienda").value(), r_cod, 0, created, updated, destroyed);
                //}

                //await popolaGrigliaDettagliLavorazioniTerzisti(KendoDDL("ddlAzienda").value(), richiesta_cod);
                ////}
            }

            ///fine inserimento

            //await LeggiLavorazioniTerzista(KendoDDL("ddlAzienda").value());
            UpdateVal = {};
            $("#btn_nuova_richiesta_terzista").trigger("click");
        }

        //Salvo Macchine
        var macchineSel = $('#tab_griglia_macchine').data("kendoGrid")
            .dataSource.data()
            .filter((el) => { return el.Selected == true; })
            .map((el) => el.chiave);

        await AggiornaMacchine(KendoDDL("ddlAzienda").value(), richiesta_cod, macchineSel);
        setKendoSwitch("noRichiestaAnnoProxCheck", getKendoSwitch("noRichiestaAnnoProxCheck"));
        WaitFrame.hide();
    }

}

async function salvaGestioneRimanenzeTestata() {
    var inconferma = "In compilazione";
    if (modifica_assegnato === true) inconferma = "Conferma";
    SubmitTrasferiti();
    SubmitRestituzioni();
    let res = await aggiornaTestataGestRima(KendoDDL("ddlAzienda").value(), richiesta_cod, inconferma);
    if ($("#stato_pratica_cod").val() === In_Compilazione.toString()) {
        if (res.Item1 === true) {
            checkRecuperoAccise(res, false);
        }
        $("#TxtrecuperoacciseGasolio").val(getValoreZeroSeNegativo(res.Item2))
        $("#TxtrecuperoacciseBenzina").val(getValoreZeroSeNegativo(res.Item3))
        $("#TxtrecuperoacciseGasolio_Serra").val(getValoreZeroSeNegativo(res.Item4))
    }
    if ($("#stato_pratica_cod").val() === Verifica_In_Corso.toString()) {
        if (res.Item1 === true) {
            checkRecuperoAccise(res, true);
        }
        $("#TxtrecuperoacciseGasolioConferma").val(getValoreZeroSeNegativo(res.Item2))
        $("#TxtrecuperoacciseBenzinaConferma").val(getValoreZeroSeNegativo(res.Item3))
        $("#TxtrecuperoacciseGasolio_SerraConferma").val(getValoreZeroSeNegativo(res.Item4))
    }
}

function getValoreZeroSeNegativo(valore) {
    if (valore < 0) {
        valore = 0;
    }
    return valore
}

function checkRecuperoAccise(acciseObj, confermaAccise) {
    let alertText = ""
    let diPiu = ""
    if (confermaAccise) {
        if (parseInt($("#TxtrecuperoacciseGasolioConferma").val()) !== acciseObj.Item2 ||
            parseInt($("#TxtrecuperoacciseBenzinaConferma").val()) !== acciseObj.Item3 ||
            parseInt($("#TxtrecuperoacciseGasolio_SerraConferma").val()) !== acciseObj.Item4) {

            if (parseInt(acciseObj.Item2) > 0) {
                alertText = alertText + "Gasolio " + acciseObj.Item2 + "L"
                diPiu = ", "
            }
            if (parseInt(acciseObj.Item3) > 0) {
                alertText = alertText + diPiu + "Benzina " + acciseObj.Item3 + "L"
                diPiu = ", "
            }
            if (parseInt(acciseObj.Item4) > 0) {
                alertText = alertText + diPiu + "Gasolio Serra " + acciseObj.Item4 + "L"
            }
            if (alertText.length > 0) {
                kendo.alert("Nella gestione rimanenze sono stati ricalcolati i carburanti confermati per il recupero accise: " + alertText + ".")
            }
        }
    } else {
        if (parseInt($("#TxtrecuperoacciseGasolio").val()) !== acciseObj.Item2 ||
            parseInt($("#TxtrecuperoacciseBenzina").val()) !== acciseObj.Item3 ||
            parseInt($("#TxtrecuperoacciseGasolio_Serra").val()) !== acciseObj.Item4) {

            if (parseInt(acciseObj.Item2) > 0) {
                alertText = alertText + "Gasolio " + acciseObj.Item2 + "L"
                diPiu = ", "
            }
            if (parseInt(acciseObj.Item3) > 0) {
                alertText = alertText + diPiu + "Benzina " + acciseObj.Item3 + "L"
                diPiu = ", "
            }
            if (parseInt(acciseObj.Item4) > 0) {
                alertText = alertText + diPiu + "Gasolio Serra " + acciseObj.Item4 + "L"
            }
            if (alertText.length > 0) {
                kendo.alert("Nella gestione rimanenze sono stati ricalcolati i carburanti soggetti a recupero accise: " + alertText + ".")
            }
        }
    }
}

async function controlliVari() {

    if (anomalieSuperficiAppezzamenti != null && $("#stato_pratica_cod").val() == In_Compilazione.toString()) {
        kendo.alert("E' necessario riallineare le superfici dichiarate con quelle effettive degli impianti prima di salvare.")
        return false
    }

    var gridMacchine = $('#tab_griglia_macchine').data("kendoGrid");
    if (gridMacchine != undefined) {
        var dtMacchine = gridMacchine.dataSource.data();
        var N_MacchineSelezionate = dtMacchine.reduce((total, el) => { return (el.Selected) ? total + 1 : total + 0; }, 0);
        if (N_MacchineSelezionate == 0) {
            kendo.alert("Non hai selezionato nessuna macchina.");
            return false;
        }

        //if (N_MacchineSelezionate == 0 && dtMacchine.length > 0) {
        //    if (!(await kendoConfirm_Promise("Non hai selezionato nessuna macchina, vuoi proseguire?"))) {
        //        return false;
        //    }
        //}
    }

    if ($("#tab_griglia_dettagliImpianti").find(".errorCell").length != 0 ||  ///GLORIA 
        $("#tab_griglia_terzisti").find(".errorCell").length != 0 ||
        $("#grdTrasferimenti").find(".errorCell").length != 0 ||
        $("#grdRestituzioni").find(".errorCell").length != 0 ||
        ucUmaAllevamenti_errorGridSolved() === false) {
        kendo.alert("Verificare i dati segnalati prima di salvare");
        return false;
    }

    return true;
}

function kendoConfirm_Promise(text, messages) {
    if (messages == undefined) {
        messages = {
            okText: "Conferma",
            cancel: "Annulla"
        }
    }
    return new Promise(function (resolve, reject) {
        let div = document.createElement("div");
        $(div).kendoConfirm({ content: text, messages: messages }).data("kendoConfirm").open().result.
            done(function () {
                resolve(true);
            })
            .fail(function () {
                resolve(false);
            });;
    });
}

function dialogTerzistiPromise(title, text, ok) {
    return new Promise(function (resolve, reject) {
        let div = document.createElement("div");
        $(div).kendoDialog({
            content: text,
            title: title,
            closable: false,
            actions: [
                {
                    text: ok,
                    primary: true,
                    action: function (e) {
                        resolve(true);
                    },
                }
            ]
        }).data("kendoDialog").open();
    });
}

function dialogNuovaRichiestaDaRendicontazionePromise(title) {
    return new Promise(function (resolve, reject) {
        let container = document.getElementById("btn_carica_richieste");
        let id_dialog = creaNewRowDiv("id_dialogNuovaRichiestaDaRendicontazione");
        container.appendChild(id_dialog);
        $("#id_dialogNuovaRichiestaDaRendicontazione").kendoDialog({
            title: title,
            closable: false,
            modal: {
                preventScroll: true
            },
            width: '55%',
            content: getTemplateUpdateDocumento(),
            open: function (e) {

                let id_File_Allegato = document.getElementById('File_Allegato');

                if (verificaOriginSecondaria(window, window.origin, event) && window.File && window.FileReader && window.FileList && window.Blob) {
                    if (id_File_Allegato.getAttribute('change') !== 'true') {
                        id_File_Allegato.addEventListener('change', handleFileSelect, false);
                        id_File_Allegato.setAttribute('change', 'true');
                    }
                }
            },
            actions: [
                {
                    text: "Sì",
                    primary: true,
                    action: function (e) {

                        ajaxAgronicaWaitFrame(true);

                        if ($('#Txt_Documento_Allegato').val() == "" || $('#Txt_Documento_Allegato').val() == undefined) {
                            ajaxAgronicaWaitFrame(false);
                            kendo.alert("Inserire l'Allegato.");

                            return false;
                        } else {

                            let risp = Controlla_Permessi_Inserimento_Nuovo_Documento($("#piva")[0].value);

                            if (risp === "") {

                                //Controllo Antivirus  in caso di virus verrà segnalato e rimosso l'allegato      
                                var nome_file = $('#Txt_Documento_Allegato').val();
                                var file_check = $('#File_Caricato').val();

                                if (file_check !== undefined && file_check !== null && file_check !== "") {

                                    var esito = checkVirus(nome_file, file_check, objP_super_server);

                                    if (esito !== "") {
                                        ajaxAgronicaWaitFrame(false);
                                        kendo.alert(esito);

                                        return false;
                                    } else {
                                        ajaxAgronicaWaitFrame(false);
                                        resolve(true);
                                    }

                                }

                            } else {
                                kendo.alert(risp);
                                ajaxAgronicaWaitFrame(false);
                                return true;
                            }

                        }

                    }
                },
                {
                    text: "No",
                    primary: false,
                    action: function (e) {
                        resolve(false);
                    }
                }
            ]
        }).data("kendoDialog").open();
    });
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
                var ind = ""
                if (val != "" && val != "-1") {
                    var aziende = KendoDDL("ddlAzienda").dataSource.data()
                    //Se l'azienda con cui sono entrato è nella lista delle aziende selezionabili, la scelgo
                    aziende.forEach(function (item, index) {
                        //console.log(item)
                        if (item !== undefined) {
                            if (item.piva == val) {
                                ind = index
                            }
                        }
                    })
                    options.success(ind);
                    //var a = KendoDDL("ddlAzienda").dataSource._pristineData.find((el) =>
                    //    el.piva == val
                    //);
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
        parametri = kendo.stringify({
            "objP_server": objP_server,
            "objP_utenti": objP_utenti,
            "Tipo_Azienda": (QS_TipoOp == 2 ? 0 : QS_TipoAzienda), //per retrocompatibilità in modifica non controllo il tipo azienda
            "piva": QS_Piva
        });

        pathCaricaCmb = "Anagrafica/Imprese.asmx/Carica_Cmb_Imprese_UMA";

    } else {
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
        pathCaricaCmb = "Anagrafica/Imprese.asmx/Carica_Cmb_Imprese";
    }

    ajaxAgronicaSync(pathCoreWS + pathCaricaCmb,
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "piva": "", "rag_soc": "...", "forma_giuridica": "0", "flagPubblica": -1 };
            if (QS_Piva != "") {
                //Potrei essere entrato con un'azienda NON conforme al tipo richiesta, quindi potrei non averla nella lista delle aziende selezionabili
                let tmp = risp.filter(x => x.piva == QS_Piva)[0];
                if (tmp !== undefined) {
                    risp.splice(risp.indexOf(tmp), 1)
                    risp.unshift(tmp);
                }
            }
            risp.unshift(objVuoto);
            //options.success(risp.slice(0,100));
            options.success(risp);
            //resolve(risp);
        }, null);
}


function ddlAzienda_OnDataBound(e) {
    var ds = this.dataSource.data();
    var index = ds.indexOf(ds.find(x => x.piva === prePiva));
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
    if (index > 0) {
        this.select(index + 1);
        ddlAzienda.onchange();
    }
}

//quando cambia la selezione imposto gli altri valori con quelli dell'azienda selezionata
async function ddlAzienda_Change(loop) {

    //$("#btn_crea_richiesta").show();
    var piva = KendoDDL("ddlAzienda").value();
    var pivaReale = await CercaPivaReale(piva);
    if (pivaReale != "") 
        $('#piva').val(pivaReale)
    else
        $('#piva').val(piva)

    if (piva == "" && QS_Piva != "") {
        piva = QS_Piva;
    }

    //far visualizzare i pulsanti dettagli appezzamenti e sintesi richieste Gloria//
    if (piva != "" && $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True") {
        if (QS_Anticipo == "1" || QS_Type == -1) {
            $("#btn_DettaglioAppezzamenti").hide();
            //$("#btn_SintesiRichieste").show();
            $("#btn_SintesiUMA").show();
        } else {
            $("#btn_DettaglioAppezzamenti").show();
            //$("#btn_SintesiRichieste").show();
            $("#btn_SintesiUMA").show();
        }
    } else {
        $("#btn_DettaglioAppezzamenti").hide();
        //$("#btn_SintesiRichieste").hide();
        $("#btn_SintesiUMA").hide();
    }
    //fine Gloria

    if (piva == "" && QS_Piva != "" && QS_TipoOp == 2) {
        piva = QS_Piva;
    }
    if (piva !== "" && KendoDDL("ddlAzienda").text() !== "...") {
        var dettagliAzienda = await CercaDettagliAzienda(piva, QS_Richiesta);
        //Se entriamo con un'azienda non coerente con il Qs_TipoAzienda, seleziono l'objVuoto
        var forma_giuridica = (KendoDDL("ddlAzienda").dataItem() == undefined ? KendoDDL("ddlAzienda").dataItem(0) : KendoDDL("ddlAzienda").dataItem().forma_giuridica)
        var azienda_pubblica = (KendoDDL("ddlAzienda").dataItem() == undefined ? KendoDDL("ddlAzienda").dataItem(0) : KendoDDL("ddlAzienda").dataItem().flagPubblica == 1 ? true : false)
        integrativa = dettagliAzienda[0].integrativa
        var pivaReale = await CercaPivaReale(piva);

        if (pivaReale != "")
            $('#piva').val(pivaReale)
        else
            $('#piva').val(piva)

        //Se sono entrata come cooperativa, controllo che l'azienda sia effettivamente una coop
        if (QS_TipoAzienda == Cooperativa_Agricola && forma_giuridica != undefined &&
            (forma_giuridica != Società_Cooperativa_a_Mutualità_Prevalente &&
                forma_giuridica != Società_Cooperativa_Diversa &&
                forma_giuridica != Società_Cooperativa_Sociale)) {

            $(".errorInfo").show();
            $(".errorInfo").html("L'azienda selezionata non è una Cooperativa. Verificare l'anagrafica azienda.");
            kendo.alert("L'azienda selezionata non è una Cooperativa. Verificare l'anagrafica azienda.");
            return false
        }


        modifica_richiesto = dettagliAzienda[0].modifica_richiesto && ((permesso_richiesta && QS_Avanzamento == 0) || (permesso_rendicontazione && QS_Avanzamento == 1));
        modifica_assegnato = dettagliAzienda[0].modifica_assegnato && ((permesso_approvazione_richiesta && QS_Avanzamento == 0) || (permesso_approvazione_rendicontazione && QS_Avanzamento == 1));

        var esisteRichiesta = (QS_Richiesta > 0 || dettagliAzienda[0].cod > 0);
        nIscrizioneCameraDiCommercio[piva] = LeggiNumeroIscrizioneCdC(piva);

        var stato = $("#stato_pratica");
        stato.html(esisteRichiesta ? dettagliAzienda[0].stato_des : "");

        var stato_cod = $("#stato_pratica_cod");
        stato_cod[0].defaultValue = esisteRichiesta ? dettagliAzienda[0].stato_cod : "";

        //Controllo l'iscrizione alla CCIA solo per le aziende private
        if (nIscrizioneCameraDiCommercio[piva] == 0 && azienda_pubblica == false) {
            $(".errorInfo").show();
            $(".errorInfo").html("Iscrizione alla camera di commercio non trovata");
            return false
        } else {
            $(".errorInfo").hide();
        }

        var descrAzienda = $('#anno');
        descrAzienda[0].defaultValue = esisteRichiesta ? dettagliAzienda[0].anno : getYear();
        anno_Change();

        var descrAzienda = $('#nDichiarazione');
        if (descrAzienda[0].defaultValue == "")
            descrAzienda[0].defaultValue = esisteRichiesta ? dettagliAzienda[0].nDichiarazione : "";

        var descrAzienda = $('#CUAA');
        descrAzienda[0].defaultValue = dettagliAzienda[0].CUAA;

        /*
        var descrAzienda = $('#indirizzo');
        descrAzienda[0].defaultValue = dettagliAzienda[0].indDes;
        */

        var descrAzienda = $('#citta');
        descrAzienda[0].defaultValue = dettagliAzienda[0].comDes;

        var descrAzienda = $('#piva');
        if (pivaReale != "")
            descrAzienda[0].defaultValue = pivaReale;
        else
            descrAzienda[0].defaultValue = piva;
        
        $("#stato-richiesta-par").html(dettagliAzienda[0].stato_des);

        /*
        var gasolio = $('#gasolioTerzisti');
        gasolio[0].defaultValue = dettagliAzienda[0].gasolio;
        var benzina = $('#benzinaTerzisti');
        benzina[0].defaultValue = dettagliAzienda[0].benzina;
        var gasolioSerra = $('#gasolioSerraTerzisti');
        gasolioSerra[0].defaultValue = dettagliAzienda[0].gasolioSerra;
        */

        let avanzRichiesta = (QS_Avanzamento == 0 ? "RICHIESTE CARBURANTE" : "RENDICONTAZIONE")
        let dummy = (QS_Avanzamento == 0 ? "Richiesta" : "Rendicontazione")
        if (QS_Avanzamento == 1 && QS_TipoOp == 2) {
            Leggi_Date_Ins_Rendicontazione()
        }

        if ($("#stato_pratica_cod").val() == In_Compilazione.toString() && !modifica_richiesto) {

            let err = ""

            if (!consentitoAggiungereModificareRendicontazione_daSetup) {
                Leggi_Termine_Ultimo_Rendicontazione()
                err = "Non è possibile modificare la rendicontazione dopo il " + Data_Fine_Rendicontazione.toString() + " per l'anno " + $("#anno").val() + "."
                if (Data_Fine_Rendicontazione != Termine_Ultimo_Rendicontazione)
                    err += "<br>E' possibile aggiungere documenti fino al " + Termine_Ultimo_Rendicontazione.toString() + "."

                $(".errorInfoFineRendicontazione").show();
                $(".errorInfoFineRendicontazione").html(err);

                gestioneRimanenze()
                attivaDisattivaEditRimanenze(false)
                attivaDisattivaCalcoloCapiAllevabili(false)
            } else {
                err = dummy + " non modificabile causa " + (dettagliAzienda[0].modifica_richiesto == false ? "presenza del documento UMA " + avanzRichiesta : "mancanza dei permessi relativi");
                $(".errorInfo").show();
                $(".errorInfo").html(err);
                attivaDisattivaEditRimanenze(false)
                attivaDisattivaCalcoloCapiAllevabili(false)
            }

            if (err == "") {
                attivaDisattivaEditRimanenze(true)
                attivaDisattivaCalcoloCapiAllevabili(true)
            }

        } else if ($("#stato_pratica_cod").val() == 2002 &&
            (!dettagliAzienda[0].Approvatore.includes(username_master) && username_master !== hSuperUserUsername)) {
            modifica_assegnato = false;
            modifica_richiesto = false;
            $(".errorInfo").show();
            $(".errorInfo").html("Richiesta non modificabile perchè presa in carico da un altro approvatore");
            $("#btn_AssegnaAutomaticamenteCarburante").addClass("disabled");
            $("#btn_salva").addClass("disabled");
            $("#btn_opzioni_stampa_istruttoria").addClass("disabled");
            if (hCoordinatoreAfor == "False") {
                $("#btn_PassaggioDiStato").addClass("disabled");
            }
            attivaDisattivaCalcoloCapiAllevabili(false)
        } else if ($("#stato_pratica_cod").val() == Verifica_In_Corso.toString() && !modifica_assegnato) {
            $(".errorInfo").show();
            let err = dettagliAzienda[0].modifica_assegnato == false ? "presenza del documento APPROVAZIONE UMA " + avanzRichiesta : "mancanza dei permessi relativi";
            $(".errorInfo").html("Richiesta non modificabile causa " + err);
            $("#btn_AssegnaAutomaticamenteCarburante").addClass("disabled");
            attivaDisattivaCalcoloCapiAllevabili(false)
        } else if (!modifica_assegnato && !modifica_richiesto) {
            if (QS_Anticipo == 1) {
                modifica_richiesto = true;
                $(".errorInfo").hide();
            } else {
                $(".errorInfo").show();
                $(".errorInfo").html("Richiesta non modificabile");
            }
            $("#btn_AssegnaAutomaticamenteCarburante").addClass("disabled");
            attivaDisattivaCalcoloCapiAllevabili(false)
        } else {
            $(".errorInfo").hide();
            $("#btn_AssegnaAutomaticamenteCarburante").removeClass("disabled");
            attivaDisattivaEditRimanenze(true)
            attivaDisattivaCalcoloCapiAllevabili($("#stato_pratica_cod").val() == 2001 ? true : false)
        }



        if (QS_Avanzamento == 1) {
            $("#panel-rimanenze").show();
            /*$("#TxtRimanenza_Gasolio_prec").val(dettagliAzienda[0].Rimanenza_Gasolio_prec);
            $("#TxtRimanenza_Benzina_prec").val(dettagliAzienda[0].Rimanenza_Benzina_prec);
            $("#TxtRimanenza_Gasolio_Serra_prec").val(dettagliAzienda[0].Rimanenza_Gasolio_Serra_prec);*/
            if ($("#stato_pratica_cod").val() != In_Compilazione.toString()) {
                gestioneRimanenze();
            }
        }

        dataPrimoAcquisto = new Date(dettagliAzienda[0].Primo_Acquisto)

        if (dataPrimoAcquisto.getFullYear() < 2000) dataPrimoAcquisto.setFullYear(2100);

        TxtRimanenza_Gasolio.value(dettagliAzienda[0].Rimanenza_Gasolio);
        TxtRimanenza_Benzina.value(dettagliAzienda[0].Rimanenza_Benzina);
        TxtRimanenza_Gasolio_Serra.value(dettagliAzienda[0].Rimanenza_Gasolio_Serra);

        $("#TxtRimanenza_Gasolio_prec").val(dettagliAzienda[0].Rimanenza_Gasolio_prec);
        $("#TxtRimanenza_Benzina_prec").val(dettagliAzienda[0].Rimanenza_Benzina_prec);
        $("#TxtRimanenza_Gasolio_Serra_prec").val(dettagliAzienda[0].Rimanenza_Gasolio_Serra_prec);

        if (dettagliAzienda[0].Anticipazioni_Gasolio === 0 &&
            dettagliAzienda[0].Anticipazioni_Benzina === 0 &&
            dettagliAzienda[0].Anticipazioni_Gasolio_Serra === 0) {

            $("#panel-anticipazioni-colturali").hide();
        } else {
            $("#TxtAnticipazioni_Gasolio").val(dettagliAzienda[0].Anticipazioni_Gasolio);
            $("#TxtAnticipazioni_Benzina").val(dettagliAzienda[0].Anticipazioni_Benzina);
            $("#TxtAnticipazioni_Gasolio_Serra").val(dettagliAzienda[0].Anticipazioni_Gasolio_Serra);
        }

        if (QS_Anticipo == 1) {
            //$("#gasolioTerzisti").val(1000); //dettagliAzienda[0].Gasolio_Tot
            //$("#benzinaTerzisti").val(dettagliAzienda[0].Benzina_Tot);
            //$("#gasolioSerraTerzisti").val(dettagliAzienda[0].gasolioSerraTot);
            gasolioTerz.value(dettagliAzienda[0].Gasolio_Tot);
            benzinaTerz.value(dettagliAzienda[0].Benzina_Tot);
            gasolioSerraTerz.value(dettagliAzienda[0].gasolioSerraTot);
        } else {
            gasolioTerz.value(dettagliAzienda[0].gasolio);
            benzinaTerz.value(dettagliAzienda[0].benzina);
            gasolioSerraTerz.value(dettagliAzienda[0].gasolioSerra);
        }

        gasolioTerzAppro.value(dettagliAzienda[0].gasolioAppro);
        benzinaTerzAppro.value(dettagliAzienda[0].benzinaAppro);
        gasolioSerraTerzAppro.value(dettagliAzienda[0].gasolioSerraAppro);

        $("#permessoAcqua").val(dettagliAzienda[0].permessoAcqua);
        $("#notePermessoAcqua").val(dettagliAzienda[0].notePermessoAcqua);
        permessoAcquaGiaRichiesto = parseFloat(dettagliAzienda[0].permessoAcquaGiaRich);

        if (parseInt($("#stato_pratica_cod").val()) !== 2001) {
            $("#permessoAcqua").prop("disabled", true);
            $("#notePermessoAcqua").prop("disabled", true);
        } else {
            $("#permessoAcqua").prop("disabled", false);
            $("#notePermessoAcqua").prop("disabled", false);
        }

        if (integrativa && permessoAcquaGiaRichiesto > 0) {
            $("#permessoAcquaLab").html("Mc aggiuntivi permesso acqua (mc)");
            $("#title-permesso-acqua").html("<b>Permesso attingimento acqua (già richiesti in precedenza " + permessoAcquaGiaRichiesto + " MC)</b>")
        } else {
            $("#permessoAcquaLab").html("Totale permesso acqua (mc)");
            $("#title-permesso-acqua").html("<b>Permesso attingimento acqua</b>")
        }

        if (QS_Anticipo == "0" && QS_Type == 0) {
            if (ddlFascicoli[0] === undefined)
                await ddlFascicoli_Load('#fascicolo')
        }

        if (QS_Richiesta <= 0 && QS_Type != -1 && QS_Anticipo == 0 && $("#anno").val() < 2025) {
            $("#row_Fascicolo").show();
            await ddlFascicoli_Load('#fascicolo');
        }

        if (QS_Richiesta > 0 && QS_Type == 0 && QS_Anticipo == 0 && QS_TipoOp == 2 && $("#anno").val() < 2025) {
            //Se siamo in modifica di una richiesta conto proprio, mostro il fascicolo bloccato
            $("#row_Fascicolo").show();
            $("#fascicolo").data("kendoDropDownList").enable(false);
        }

        if (!(permesso_rendicontazione && permesso_approvazione_rendicontazione))
            $("#noRichiestaAnnoProxCheck").data("kendoSwitch").enable(permesso_rendicontazione && !permesso_approvazione_rendicontazione && dettagliAzienda[0].stato_cod === In_Compilazione)

        if (QS_Type == -1) {
            if (QS_TipoAzienda == Cooperativa_Agricola) {
                $("#tipo-richiesta-par").html((QS_Avanzamento == 0 ? "Richiesta" : "Rendicontazione") + " Cooperativa");
            } else {
                $("#tipo-richiesta-par").html((QS_Avanzamento == 0 ? "Richiesta" : "Rendicontazione") + " Conto Terzi");
            }
        } else {
            $("#tipo-richiesta-par").html("Richiesta anticipo");

            if (QS_Anticipo == 0) {
                $("#tipo-richiesta-par").html((QS_Avanzamento == 0 ? "Richiesta" : "Rendicontazione") + " Conto Proprio");
                setKendoSwitch("montagnaSwitchCheck", dettagliAzienda[0].Allevati_Montagna);
            }

            if (QS_TipoAzienda == Cooperativa_Agricola) {
                $("#tipo-richiesta-par").html((QS_Avanzamento == 0 ? "Richiesta" : "Rendicontazione") + " Cooperativa");
            }
        }

        if ($("#stato_pratica_cod").val() !== In_Compilazione.toString()) {
            $("#legendaNote").show();
        } else {
            $("#legendaNote").hide();
        }

        if (esisteRichiesta) {
            abilitaDisabilitaStampa(dettagliAzienda);

            // Verifico se devo inizializzare la tab ed il pulsante per stampare il verbale di istruttoria relativo alla richiesta 
            if (dettagliAzienda[0].stato_cod === Verifica_In_Corso
                && ((permesso_approvazione_richiesta && QS_Avanzamento == 0) || (permesso_approvazione_rendicontazione && QS_Avanzamento == 1))) {

                if (tabVerbIstrInizializzata === false) {
                    // TODO Valutare se possibile spostare l'inizializzazione al docReady sempre tenendo conto che sono necessarie le variabili dettagliAzienda ed i permessi;
                    // in questa logica, valutare di mantenere come variabile globale il risultato della chiamata "CercaDettagliAzienda"
                    tabVerbIstrInizializzata = true;

                    $("#btn_opzioni_stampa_istruttoria").on("click", btnOpzStampaIstruttoriaClick).show();
                    $("#btn_stampa_istruttoria").on("click", btnStampaIstruttoriaClick);


                    // Configuro la tab per la stampa del verbale di istruttoria
                    creaKendoDropDownList("ddlIstrProprioTerzi", {
                        read: function (options) {
                            options.success([
                                { Val: 0, Des: "" },
                                { Val: 1, Des: "Proprio" },
                                { Val: 2, Des: "Terzi" },
                            ]);
                        }
                    }, "Des", "Val");

                    creaKendoSwitch("ksIstrModDati");
                    creaKendoSwitch("ksIstrSegnMacchine");

                    creaKendoDropDownList("ddlIstrEsito", {
                        read: function (options) {
                            options.success([
                                { Val: 0, Des: "" },
                                { Val: 1, Des: "Con Assegnazione" },
                                { Val: 2, Des: "Senza Assegnazione" },
                            ]);
                        }
                    }, "Des", "Val");

                    $("#txtAreaIstrNote").kendoTextArea({
                        rows: 4
                    });

                    if (QS_Avanzamento == 1) {
                        $(".boxIstrSegnMacchine").hide();
                    }
                }
            } else {
                // Elimino la tab per la stampa del verbale di istruttoria, in quanto lo stato della pratica non è corretto o l'utente non possiede il permesso adeguato
                $("#tabstrip_dettagli").data("kendoTabStrip").remove(".Verb");
            }

        }

        if ((new Date().getTime()) < (EUdate(Data_Inizio_Rendicontazione).getTime())) {
            $("#noRichiestaAnnoProxCheck").data("kendoSwitch").enable(false)
        }

        /*var inputCarburanti = (dettagliAzienda[0].gasolio > 0 || dettagliAzienda[0].gasolioSerra > 0 ||
            dettagliAzienda[0].benzina > 0) 
        document.getElementById('gasolioTerzisti').disabled = inputCarburanti;
        document.getElementById('benzinaTerzisti').disabled = inputCarburanti;
        document.getElementById('gasolioSerraTerzisti').disabled = inputCarburanti;
        if (inputCarburanti)
            document.getElementById("butto").innerHTML = document.getElementById("butto").innerHTML.
                            replace('id="btn_aggiorna_carb"', 'id="btn_aggiorna_carb" disabled=""');
        else
            document.getElementById("butto").innerHTML = document.getElementById("butto").innerHTML.
                            replace('id="btn_aggiorna_carb" disabled=""', 'id="btn_aggiorna_carb"');
                            */
        //if (dettagliAzienda[0].cod > 0) {
        //    richiestaRinuncia = statoRichiesta(piva, dettagliAzienda[0].cod) == 2009
        //}
        //else
        //    richiestaRinuncia = false;

        showPanels = await isPrimaRichiesta($("#anno").val(), dettagliAzienda[0].cod);
        //alert("showPanels  " + showPanels);
        //alert(QS_Richiesta);
        if (showPanels == "false") {
            $("#panel-rimanenze").hide();
            $("#panel-anticipazioni-colturali").hide();
            $("#panel-carburantiTerzisti").hide();
            gasolioTerz.value(0);
            benzinaTerz.value(0);
            gasolioSerraTerz.value(0);
            gasolioTotTerz = 0;
            benzinaTotTerz = 0;
            gasolioSerraTotTerz = 0;
        }

        richiestaRinuncia = false;
        $("#tab_griglia_terzisti").hide();
        $("#btn_salva").hide();
        $("#btn_carica_richieste").show();
        if (QS_Richiesta <= 0) {
            richiesta_cod = dettagliAzienda[0].cod;
            //alert("richiesta_cod -> " + richiesta_cod);
            $("#anno").prop("disabled", false);
        } else {
            $("#btn_carica_richieste").hide();
            $("#row_Fascicolo").hide();
            richiesta_cod = QS_Richiesta;
            //alert("richiesta_cod1 -> " + richiesta_cod);
            if (!loop) {
                $("#btn_carica_richieste").trigger("click");
            }
        }
        RecuperaDataPassaggioDiStato();

        if ($("#grdRichiestaDocumenti").data("kendoGrid") != undefined && $("#grdRichiestaDocumenti").data("kendoGrid") != null) {
            $("#grdRichiestaDocumenti").data("kendoGrid").destroy();
            popolaGrigliaRichiestaDocumenti("grdRichiestaDocumenti");

            Applica_Personalizzazioni_Griglie()

        }
    }
}

function attivaDisattivaEditRimanenze(enable) {
    if (enable) {
        $("#TxtRimanenza_Gasolio").removeAttr("disabled");
        $("#TxtRimanenza_Benzina").removeAttr("disabled");
        $("#TxtRimanenza_Gasolio_Serra").removeAttr("disabled");
    } else {
        $("#TxtRimanenza_Gasolio").attr("disabled", "disabled");
        $("#TxtRimanenza_Benzina").attr("disabled", "disabled");
        $("#TxtRimanenza_Gasolio_Serra").attr("disabled", "disabled");
    }
}


function getYear() {
    var date = new Date();
    return date.getUTCFullYear();
    //if ($.cookie("UMA.Anno") != null && !isNaN($.cookie("UMA.Anno"))) {
    //    alert(parseInt($.cookie("UMA.Anno")));
    //    return parseInt($.cookie("UMA.Anno"));
    //} else {
    //    var date = new Date();
    //    let anno = new Date().getFullYear();
    //    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    //    $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
    //    alert(anno);
    //    return anno;
    //}
}

function setYear(anno) {
    var date = new Date();
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
}

function abilitaDisabilitaStampa(dettagliAzienda) {
    // Le richieste e le rendicontazioni devono essere stampate nello stato "inserimento in corso, dati non verificati"
    $("#btn_stampa_rich_rendicon").toggle(
        (QS_Richiesta > 0 || dettagliAzienda[0].cod > 0)
        && (consentitoAggiungereModificareRendicontazione_daSetup || consentitoEditareDocumenti_daSetup)
        && [2001].includes(dettagliAzienda[0].stato_cod)
        && ((permesso_richiesta && QS_Avanzamento == 0) || (permesso_rendicontazione && QS_Avanzamento == 1))
    );
}

function calcoloFabbisogno(rowParent, row, grid) {
    //let costo = tabellaCalcoloCosti.filter((elem) => {
    //    return elem.Macrouso_UMA_Cod == rowParent.Macrouso_UMA_Cod && elem.Lav_UMA_Cod == row.Lav_UMA_Cod && elem.Id_Attivita == row.Attivita_Cod;
    //})[0];
    let maxAcqua = 0;
    let costo = filtraTabellaCalcoloCosti(rowParent.Macrouso_UMA_Cod, row.Lav_UMA_Cod, null, row.Attivita_Cod, rowParent.Regolamento_Cod);
    if (costo != undefined) {
        let costoCarburante = 0;
        switch (row.Car_Cod) {
            case "1":
            case "2":
                costoCarburante = costo.Gasolio_Lt;
                break;
            case "3":
            case "4":
                costoCarburante = costo.Benzina_Lt;
                break;
            case "8":
                costoCarburante = costo.Gasolio_Lt;
                break;
            case "9":
                costoCarburante = 0;
                break;
            case "10":
                costoCarburante = 0;
                break;
        }
        /*
        if (costo.Lav_UMA_Des.includes("(MAX "))
            costoCarburante /= 4
        */

        if (costo.Udm_Alternativa == null || costo.Udm_Alternativa == undefined || costo.Udm_Alternativa == "") {
            Superficie_Trattata = row.Superficie_Trattata;
            fabbisogno_Superficie_Trattata = 0;
            //if (row.SupMaggiorazioneTrasferimenti != undefined) {
            //    Superficie_Trattata += parseFloat(row.SupMaggiorazioneTrasferimenti);
            //}
            fabbisogno_Superficie_Trattata = (Superficie_Trattata) * (costoCarburante);
            fabbisogno_Superficie_Trattata = fabbisogno_Superficie_Trattata * row.nLavRichieste;

            sup_UMA_B_Edit = row.Sup_B;
            fabbisogno_UMA_B_Edit = 0;
            fabbisogno_UMA_B_Edit = (sup_UMA_B_Edit) * (costoCarburante);
            fabbisogno_UMA_B_Edit = fabbisogno_UMA_B_Edit * row.nLavRichieste;
            fabbisogno_UMA_B_Edit = parseFloat((fabbisogno_UMA_B_Edit * percentualeZonaPendenzaB).toFixed(4));

            TerrenoMedio = row.TerrenoMedio;
            fabbisogno_TerrenoMedio = 0;

            TerrenoTenace = row.TerrenoTenace;
            fabbisogno_TerrenoTenace = 0;
            if (costo.Maggiorazione_Terreno_MedioTenace == 1) {
                fabbisogno_TerrenoMedio = (TerrenoMedio) * (costoCarburante);
                fabbisogno_TerrenoMedio = fabbisogno_TerrenoMedio * row.nLavRichieste;
                fabbisogno_TerrenoMedio = parseFloat((fabbisogno_TerrenoMedio * percentualeTerrenoMedio).toFixed(4));

                fabbisogno_TerrenoTenace = (TerrenoTenace) * (costoCarburante);
                fabbisogno_TerrenoTenace = fabbisogno_TerrenoTenace * row.nLavRichieste;
                fabbisogno_TerrenoTenace = parseFloat((fabbisogno_TerrenoTenace * percentualeTerrenoTenace).toFixed(4));
            }

            let fabbisogno_Trasferimenti = 0;
            if (row.SupMaggiorazioneTrasferimenti == 1) {
                fabbisogno_Trasferimenti = parseFloat((row.Superficie_Trattata * L_Maggiorazione_Trasferimenti).toFixed(4));
            }

            row.fabbisognoCalc = parseFloat((fabbisogno_Superficie_Trattata + fabbisogno_UMA_B_Edit + fabbisogno_TerrenoMedio + fabbisogno_TerrenoTenace + fabbisogno_Trasferimenti).toFixed(4));
        } else {
            if (rowParent.Macrouso_UMA_Cod == "1034") {
                row.fabbisognoCalc = row.Qta_Manuale * row.Mesi * costoCarburante;
            } else {
                row.fabbisognoCalc = row.Qta_Manuale * costoCarburante;
            }
        }

        if (costo.Lav_Cod === 1) {
            if (costo.Coeff_acq_distr === undefined || costo.Coeff_acq_distr === null)
                costo.Coeff_acq_distr = 0;
            if (($("#permessoAcqua").val() === "0" || $("#permessoAcqua").val() === null || $("#permessoAcqua").val() === undefined))
                $("#permessoAcqua").val(0);
            maxAcqua = (parseFloat($("#permessoAcqua").val()) + permessoAcquaGiaRichiesto) * costo.Coeff_acq_distr;
            //if (row.fabbisognoCalc > maxAcqua && !maxAcqua == 0) {
            //    row.fabbisognoCalc = maxAcqua;
            //}

            let totRichiestoIrrigazioni = leggiTotaleRichiestoIrrigazioni(row);
            maxAcqua = maxAcqua - totRichiestoIrrigazioni
            maxAcqua = Math.max(0, maxAcqua);
        }

        row.ltrichiesto = row.fabbisognoCalc;
        if (getMaggiorazioneAutomatica(row.Macrouso_UMA_Cod, row.Lav_UMA_Cod)) {
            row.ltrichiesto = row.ltrichiesto * 100 / (100 - Percentuale_Decurtamento);
            row.fabbisognoCalc = row.ltrichiesto;
        }

        if (row.ltrichiesto > maxAcqua && !maxAcqua == 0) {
            row.ltrichiesto = maxAcqua;
        }

        grid.refresh();
    }
}

function leggiTotaleRichiestoIrrigazioni(escludiLavorazione) {
    let richiesto = 0;
    let parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");

    let tabelleAperte = $("#tab_griglia_dettagliImpianti").find("div[id^=GrigliaDettagliLavorazioni]");
    for (var i = 0; i < tabelleAperte.length; i++) {
        let parentRow = $($(tabelleAperte[i]).parents(".k-detail-row")[0]).prev()
        var parentRowItem = parentGrid.dataItem(parentRow);

        let datiA = $("#" + tabelleAperte[i].id).data("kendoGrid").dataSource.data();
        for (var j = 0; j < datiA.length; j++) {
            let dataItem = datiA[j];
            if (escludiLavorazione === dataItem) {
                continue;
            }
            let costo = filtraTabellaCalcoloCosti(dataItem.Macrouso_UMA_Cod, dataItem.Lav_UMA_Cod, null, dataItem.Attivita_Cod, parentRowItem.Regolamento_Cod);
            if (costo != undefined && costo.Lav_Cod === 1) {
                if (costo.Coeff_acq_distr !== undefined && costo.Coeff_acq_distr !== null && costo.Coeff_acq_distr > 0) {
                    richiesto = richiesto + parseFloat(dataItem.ltrichiesto)
                }
            }
        }
    }

    return richiesto;
}

function recupera_nLavPreviste(Lav_UMA_Cod, Macrouso_UMA_Cod, Lav_Cod, Regolamento_Cod) {
    if (Regolamento_Cod == undefined || Regolamento_Cod == null) {
        Regolamento_Cod = RegolamentoConvenzionale
    }
    let costo = tabellaCalcoloCosti.filter((elem) => {
        if (Lav_Cod) {
            return elem.Macrouso_UMA_Cod == Macrouso_UMA_Cod &&
                (elem.Regolamento_Cod == Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi) &&
                elem.Lav_UMA_Cod == Lav_UMA_Cod &&
                elem.Lav_Cod == Lav_Cod;
        } else {
            return elem.Macrouso_UMA_Cod == Macrouso_UMA_Cod &&
                (elem.Regolamento_Cod == Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi) &&
                elem.Lav_UMA_Cod == Lav_UMA_Cod;
        }
    })[0];
    if (costo != undefined) {
        return costo.N_Max_Operazioni;
    } else {
        return 0;
    }
}

function recupera_nLavPreviste_Parziale(Lav_UMA_Cod, Regolamento_Cod) {
    if (Regolamento_Cod == undefined || Regolamento_Cod == null) {
        Regolamento_Cod = RegolamentoConvenzionale
    }
    let costo = tabellaCalcoloCosti.filter((elem) => {
        //if (Lav_Cod) {
        //    return elem.Lav_UMA_Cod == Lav_UMA_Cod &&
        //        elem.Lav_Cod == Lav_Cod;
        //} else {
        return elem.Lav_UMA_Cod == Lav_UMA_Cod && (elem.Regolamento_Cod == Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi);
        /*}*/
    })[0];
    if (costo != undefined) {
        return costo.N_Max_Operazioni;
    } else {
        return 0;
    }
}


function CaricaAllegati(e) {

    //var url = "../Scadenzario/Scad_lista.aspx?type=doc" + "&richiesta_cod=" + QS_Richiesta + "&area_provenienza=7" + "&origine_nc=../CarburantiUMA/RichiestaCarburanti.aspx" + "&p=" + QS_Piva;

    var url = GetUrlDocAgenda2010(QS_Piva, 0, QS_Richiesta, 0, 0, "Read", DOCUMENTALE)

    apriKendoWindowTestataGriglia(url, "Documenti U.M.A. Carburanti");
}


function NuovoAllegato(e) {

    var ID_Alert_Entita = -1;
    var ID_Elenco = -1;
    var Richiesta_Cod = richiesta_cod;
    var Modalita = "doc";

    var param = kendo.stringify({ 'Piva': pivaSelezionata, 'ID_Elenco': ID_Elenco, 'TipoOperazione': 1, 'ID_Alert_Entita': ID_Alert_Entita, 'Richiesta_Cod': Richiesta_Cod });
    //var param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 1, 'ID_Alert_Entita': ID_Alert_Entita });
    var url = "../Scadenzario/Scad_CreaModificaItem.aspx?scadstr=" + param + "&type=" + Modalita + "&origine_nc=../CarburantiUMA/RichiestaCarburanti.aspx" + "&p=" + QS_Piva + "&rc=" + QS_Richiesta;

    apriKendoWindowTestataGriglia(url, "Documenti U.M.A. Carburanti");

}

function DettaglioColture(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    $(document.body).append('<div id="griglia_dettagliColture"></div>');
    macro_cod = datiRiga.Macrouso_UMA_Cod;
    program_cod = datiRiga.Programmazione_Cod;
    Veg_Cod = datiRiga.Veg_Cod;
    Id_Cod = datiRiga.Id_Cod;
    if (macro_cod != undefined && macro_cod != "") {
        GrigliaDettagliColture("griglia_dettagliColture", true);
        $('#griglia_dettagliColture').kendoWindow({
            title: "Dettaglio Coltura",
            modal: true,
            resizable: true,
            iframe: true,
            width: "90%",
            height: "80%",
            actions: ["Maximize", "Close"],
            close: function () {
                setTimeout(function () {
                    $('#griglia_dettagliColture').kendoWindow('destroy');
                }, 200);
            }
        }).data('kendoWindow').center();//.maximize();
    }
}

var windowdettagli
function DettaglioAppezzamenti() {


    if (windowdettagli == undefined) {

        windowdettagli = $('#griglia_dettagliAppezzamenti').kendoWindow({
            title: "Dettaglio Appezzamenti",
            modal: true,
            resizable: true,
            iframe: true,
            width: "50%",
            height: "80%",
            actions: ["Maximize", "Close"],
            close: function () {
                setTimeout(function () {
                    $('#dettaglio').hide();
                    //$('#griglia_dettagliAppezzamenti').kendoWindow('destroy');
                    //windowdettagli.close();
                }, 200);
            }
        }).data('kendoWindow')//.center();//.maximize();

    } else {
        windowdettagli.open();
    }


    windowdettagli.center();
    if ($("#anno").val() < 2025) {
        Fascicoli_InTemplate();
    } else {
        $("#row_Fascicolo1").hide();
        $("#btn_cerca").hide();
        $("#btn_cerca").click(); 
    }


}




function MostraGrigliaDettaglioAppezzamenti() {


    LeggiAppezzamentiProgrammazione(function (risposta) {
        /*console.log('risposta= ' + risposta)*/
        if (risposta.RispostaOK) {
            $("#grigliadettaglioval").val(risposta.RispostaStringa);
            $('#dettaglio').show();
            DettagliAppezzamenti("dettaglio")

        }
    }
    )

}


var windowpercentuale;
var PercAnticipo
function PercentualeAnticipo(percentualeAnticipo) {

    //Copio il div che contiene la window dell'anticipo
    if (PercAnticipo == undefined)
        PercAnticipo = $("#Imposta_PercAnticipo").clone()

    return new Promise(async function (resolve, reject) {
        if (windowpercentuale != undefined) {
            //Se è la seconda volta che riapro la window (me ne accorgo perchè != da undefined):
            //- faccio la destroy di kendo, che va ad eliminare l'elemento html
            //- resetto la variabile 
            //- rimetto il div al suo posto 
            windowpercentuale.destroy()
            windowpercentuale = undefined

            PercAnticipo.appendTo("#raccoglitore_Anticipo");
        }

        if (windowpercentuale == undefined) {
            windowpercentuale = $('#Imposta_PercAnticipo').kendoWindow({
                title: "Imposta Percentuale Anticipo",
                modal: true,
                resizable: false,
                iframe: true,
                width: "65%",
                height: "45%",
                //actions: ["Maximize"],
                close: function () {
                    resolve(true);
                }
            }).data('kendoWindow')

        } else {
            //Non dovrebbe mai entrare qui...
            $('#Imposta_PercAnticipo').show();
        }

        let consentiAnticipo = await VerificaConsentiAnticipo();
        if (!consentiAnticipo) {
            $("#row_MsgErrore").attr("style", "display:block; color:red");
            $("#lblMsgAliquotaSup").html("Impossibile procedere con la richiesta di anticipo: non è presente nessuna richiesta approvata successiva all'ultimo anticipo concesso, nell'anno " + ($("#anno").val() - 1).toString() + ".");
        }

        if (consentiAnticipo) {
            $("#btn_proseguiAnticipo").show();
        } else {
            $("#txtVarPercentualeAnticipo").attr("disabled", "disabled");
            $("#btn_proseguiAnticipo").hide();
        }
        $("#panel_Anticipo").show();
        $("#row_percentuale").attr("style", "display:block");

        windowpercentuale.center();
        $("#txtVarPercentualeAnticipo").val(percentualeAnticipo * 100);

        if (consentiAnticipo) {
            leggiAcquistatoAnnoPrecedente()
        }

    }).catch(
        err => console.log(err));
}

function VerificaConsentiAnticipo() {
    return new Promise(async function (resolve, reject) {
        let consentiAnticipo = true;

        //Non permettere una richiesta di anticipo se non c’è una richiesta approvata posteriore all'ultimo anticipo concesso.
        let anticipiAnniPrecedenti = await ContaRichiesteWS(0, -1, QS_Type == -1);
        if (anticipiAnniPrecedenti.length > 0) {

            let annoUltimoAnticipo = anticipiAnniPrecedenti[0].Anno;
            for (var i = 1; i < anticipiAnniPrecedenti.length; i++) {
                if (anticipiAnniPrecedenti[i].Anno > annoUltimoAnticipo) {
                    annoUltimoAnticipo = anticipiAnniPrecedenti[i].Anno;
                }
            }
            let richiesteAnnoPrecedente = await ContaRichiesteWS(annoUltimoAnticipo, 0, QS_Type == -1);
            let numeroRichiesteApprovateAnnoPrecedente = richiesteAnnoPrecedente.filter(r => r.Stato_Cod == Verifica_Intermedia_Completata_Con_Successo || r.Stato_Cod == Inserimento_Completato_Per_Il_Periodo_Di_Competenza || r.Stato_Cod == Verifica_Completata).length;

            if (numeroRichiesteApprovateAnnoPrecedente == 0) {
                consentiAnticipo = false;

                let currAnno = $('#anno')[0].value;
                let anno = annoUltimoAnticipo + 1;
                while (anno < currAnno) {
                    richiesteAnnoPrecedente = await ContaRichiesteWS(anno, 0, QS_Type == -1);
                    numeroRichiesteApprovateAnnoPrecedente = richiesteAnnoPrecedente.filter(r => r.Stato_Cod == Verifica_Intermedia_Completata_Con_Successo || r.Stato_Cod == Inserimento_Completato_Per_Il_Periodo_Di_Competenza || r.Stato_Cod == Verifica_Completata).length;

                    if (numeroRichiesteApprovateAnnoPrecedente > 0) {
                        consentiAnticipo = true;
                        break;
                    }

                    anno++;
                }
            }
        }

        resolve(consentiAnticipo);
    });
}


//function VerificaAliquota() {
//    if ((Percentuale_Anticipo_Carb * 100) >= $("#txtVarPercentualeAnticipo")[0].value) {
//    //Percentuale_Anticipo_Carb = $("#txtVarPercentualeAnticipo")[0].value;
//    alert("Valore Aliquota " + Percentuale_Anticipo_Carb)
//}
//    else {
//    alert('Aliquota superiore');
//}
//}

async function ImpostaPercentualeAnticipo() {
    //var percInserita = ($("#txtVarPercentualeAnticipo")[0].value).replace(',', '.')
    //if ((Percentuale_Anticipo_Carb * 100) >= percInserita) 
    if (ControllaPercentualiCarburanti($("#gasolioAnticipo")[0].value, $("#benzinaAnticipo")[0].value, $("#gasolioSerraAnticipo")[0].value)) {
        //Percentuale_Anticipo_Carb = parseFloat((percInserita / 100));
        $("#row_MsgErrore").attr("style", "display:none");
        $('#Imposta_PercAnticipo').hide();
        proseguiAnticipo = true
        windowpercentuale.close();
    }
    else {
        $("#row_MsgErrore").attr("style", "display:block");
        $("#lblMsgAliquotaSup").html("La percentuale inserita non può essere superiore al " + (Percentuale_Anticipo_Carb * 100) + "%");
        //alert('Aliquota superiore');
        proseguiAnticipo = false
        return false;
    }
    //$('#Imposta_PercAnticipo').hide();
}

function ControllaPercentualiCarburanti(gasolio, benzina, gasolioSerra) {
    var qtaGasolio = parseFloat(gasolio);
    var qtaBenzina = parseFloat(benzina);
    var qtaGasolioSerra = parseFloat(gasolioSerra);

    // Caso di acquisto forfetario per nuove aziende 
    if (AcquistatoAnnoPrecedente_Gasolio == 0 && AcquistatoAnnoPrecedente_Benzina == 0 && AcquistatoAnnoPrecedente_Gasolio_Serra == 0) {
        return true;
    }

    if (AcquistatoAnnoPrecedente_Gasolio > 0) {
        if (Percentuale_Anticipo_Carb < (qtaGasolio / AcquistatoAnnoPrecedente_Gasolio)) {
            return false;
        }
    } else {
        if (qtaGasolio > 0) {
            return false;
        }
    }

    if (AcquistatoAnnoPrecedente_Benzina > 0) {
        if (Percentuale_Anticipo_Carb < (qtaBenzina / AcquistatoAnnoPrecedente_Benzina)) {
            return false;
        }
    } else {
        if (qtaBenzina > 0) {
            return false;
        }
    }

    if (AcquistatoAnnoPrecedente_Gasolio_Serra > 0) {
        if (Percentuale_Anticipo_Carb < (qtaGasolioSerra / AcquistatoAnnoPrecedente_Gasolio_Serra)) {
            return false;
        }
    } else {
        if (qtaGasolioSerra > 0) {
            return false;
        }
    }

    return true;
}

function controlloPercAnticipoInserita() {
    var percInserita = parseFloat(($("#txtVarPercentualeAnticipo")[0].value).replace(',', '.')).toFixed(2);
    $("#txtVarPercentualeAnticipo").val(percInserita);

    if ((Percentuale_Anticipo_Carb * 100) >= percInserita) {
        $("#row_MsgErrore").attr("style", "display:none");
    }
    else {
        $("#row_MsgErrore").attr("style", "display:block");
        $("#lblMsgAliquotaSup").html("La percentuale inserita non può essere superiore al " + (Percentuale_Anticipo_Carb * 100) + "%");
    }

    if (AcquistatoAnnoPrecedente_Gasolio > 0)
        $("#gasolioAnticipo").val(Math.floor(AcquistatoAnnoPrecedente_Gasolio * (percInserita / 100))/*.toFixed(4)*/)
    if (AcquistatoAnnoPrecedente_Benzina > 0)
        $("#benzinaAnticipo").val(Math.floor(AcquistatoAnnoPrecedente_Benzina * (percInserita / 100))/*.toFixed(4)*/)
    if (AcquistatoAnnoPrecedente_Gasolio_Serra > 0)
        $("#gasolioSerraAnticipo").val(Math.floor(AcquistatoAnnoPrecedente_Gasolio_Serra * (percInserita / 100))/*.toFixed(4)*/)

}

function controlloPercAnticipoModifica() {
    var percInserita = parseFloat(($("#txtVarPercentualeAnticipoTerzisti")[0].value).replace(',', '.')).toFixed(2);
    $("#txtVarPercentualeAnticipoTerzisti").val(percInserita);

    if ((Percentuale_Anticipo_Carb * 100) >= percInserita) {
        $("#row_MsgErroreModifica").attr("style", "display:none");
    }
    else {
        $("#row_MsgErroreModifica").attr("style", "display:block");
        $(".msgErrorAnticipoModifica").show();
        $(".msgErrorAnticipoModifica").html("La percentuale inserita non può essere superiore al " + (Percentuale_Anticipo_Carb * 100) + "%");
    }

    if (AcquistatoAnnoPrecedente_Gasolio > 0)
        $("#gasolioTerzisti").data("kendoNumericTextBox").value(Math.floor(AcquistatoAnnoPrecedente_Gasolio * (percInserita / 100)))
    if (AcquistatoAnnoPrecedente_Benzina > 0)
        $("#benzinaTerzisti").data("kendoNumericTextBox").value(Math.floor(AcquistatoAnnoPrecedente_Benzina * (percInserita / 100)))
    if (AcquistatoAnnoPrecedente_Gasolio_Serra > 0)
        $("#gasolioSerraTerzisti").data("kendoNumericTextBox").value(Math.floor(AcquistatoAnnoPrecedente_Gasolio_Serra * (percInserita / 100)))

}

function controlloAnticipoGasolioInserito() {
    // Campo percentuale visualizzato ed aggiornato solo in caso di un solo tipo di carburante per l'anticipo. 
    var txtPercentuale = (AcquistatoAnnoPrecedente_Benzina == 0 && AcquistatoAnnoPrecedente_Gasolio_Serra == 0) ? $("#txtVarPercentualeAnticipo") : null;
    controlloAnticipoCarburante($("#gasolioAnticipo"), txtPercentuale, AcquistatoAnnoPrecedente_Gasolio, $("#row_MsgErrore"), $("#lblMsgAliquotaSup"))
}

function controlloAnticipoBenzinaInserito() {
    // Campo percentuale visualizzato ed aggiornato solo in caso di un solo tipo di carburante per l'anticipo. 
    var txtPercentuale = (AcquistatoAnnoPrecedente_Gasolio == 0 && AcquistatoAnnoPrecedente_Gasolio_Serra == 0) ? $("#txtVarPercentualeAnticipo") : null;
    controlloAnticipoCarburante($("#benzinaAnticipo"), txtPercentuale, AcquistatoAnnoPrecedente_Benzina, $("#row_MsgErrore"), $("#lblMsgAliquotaSup"))
}

function controlloAnticipoGasolioSerraInserito() {
    // Campo percentuale visualizzato ed aggiornato solo in caso di un solo tipo di carburante per l'anticipo. 
    var txtPercentuale = (AcquistatoAnnoPrecedente_Gasolio == 0 && AcquistatoAnnoPrecedente_Benzina == 0) ? $("#txtVarPercentualeAnticipo") : null;
    controlloAnticipoCarburante($("#gasolioSerraAnticipo"), txtPercentuale, AcquistatoAnnoPrecedente_Gasolio_Serra, $("#row_MsgErrore"), $("#lblMsgAliquotaSup"))
}

function controlloAnticipoGasolioModifica() {
    // Campo percentuale visualizzato ed aggiornato solo in caso di un solo tipo di carburante per l'anticipo. 
    var txtPercentuale = (AcquistatoAnnoPrecedente_Benzina == 0 && AcquistatoAnnoPrecedente_Gasolio_Serra == 0) ? $("#txtVarPercentualeAnticipoTerzisti") : null;
    controlloAnticipoCarburante($("#gasolioTerzisti"), txtPercentuale, AcquistatoAnnoPrecedente_Gasolio, $("#row_MsgErroreModifica"), $(".msgErrorAnticipoModifica"))
}

function controlloAnticipoBenzinaModifica() {
    // Campo percentuale visualizzato ed aggiornato solo in caso di un solo tipo di carburante per l'anticipo. 
    var txtPercentuale = (AcquistatoAnnoPrecedente_Gasolio == 0 && AcquistatoAnnoPrecedente_Gasolio_Serra == 0) ? $("#txtVarPercentualeAnticipoTerzisti") : null;
    controlloAnticipoCarburante($("#benzinaTerzisti"), txtPercentuale, AcquistatoAnnoPrecedente_Benzina, $("#row_MsgErroreModifica"), $(".msgErrorAnticipoModifica"))
}

function controlloAnticipoGasolioSerraModifica() {
    // Campo percentuale visualizzato ed aggiornato solo in caso di un solo tipo di carburante per l'anticipo. 
    var txtPercentuale = (AcquistatoAnnoPrecedente_Gasolio == 0 && AcquistatoAnnoPrecedente_Benzina == 0) ? $("#txtVarPercentualeAnticipoTerzisti") : null;
    controlloAnticipoCarburante($("#gasolioSerraTerzisti"), txtPercentuale, AcquistatoAnnoPrecedente_Gasolio_Serra, $("#row_MsgErroreModifica"), $(".msgErrorAnticipoModifica"))
}

function controlloAnticipoCarburante(txtCarburante, txtPercentuale, AcquistatoAnnoPrecedente, rowMsgErrore, msgErrore) {
    var qntInserita = parseFloat(txtCarburante[0].value);
    if (qntInserita < 0 || isNaN(qntInserita)) {
        txtCarburante.val(0);

        if (txtPercentuale != null) {
            txtPercentuale.val(0);
        }

    } else if (AcquistatoAnnoPrecedente > 0) {

        qntInserita = Math.floor(qntInserita);
        txtCarburante.val(qntInserita);

        var percentualeCalcolata = qntInserita / AcquistatoAnnoPrecedente
        if (percentualeCalcolata > Percentuale_Anticipo_Carb) {
            rowMsgErrore.attr("style", "display:block");
            msgErrore.html("La percentuale inserita non può essere superiore al " + (Percentuale_Anticipo_Carb * 100) + "%");
        } else {
            rowMsgErrore.attr("style", "display:none");
            msgErrore.html("");
        }

        if (txtPercentuale != null) {
            txtPercentuale.val((percentualeCalcolata * 100).toFixed(2));
        }
    }
}

async function MostraLavorazioneTerzisti(piva, programmazione_cod, gruppo_colturale, anno) {

    let valore = await controlloIncrociato(piva, programmazione_cod, gruppo_colturale, anno, true);
    //let valore = await controlloRichiesteLavorazioni(piva, programmazione_cod, gruppo_colturale, anno);

    //console.log(JSON.stringify(valore))
    //$("#LavorazioniTerzistiDett").val(valore);
    $("#LavorazioniTerzistiDett").val(JSON.stringify(valore));
    console.log($("#LavorazioniTerzistiDett").val())


    DettaglioLavorazioneTerzisti("dettagliolav");

}

async function MostraAnticipazioniTerzisti(piva, anno) {

    let valore = await RecuperaAnticipazioniColturali(piva, anno);

    $("#AnticipazioniDett").val(JSON.stringify(valore));

    DettaglioAnticipazioniTerzisti("dettaglioAnt");

}

var windowdettagliAnt;
function RecuperoAnticipazioni() {
    if (windowdettagliAnt == undefined) {
        MostraAnticipazioniTerzisti(KendoDDL("ddlAzienda").value(), $('#anno')[0].value)
        //controlloIncrociato(piva, 0, 0, $('#anno')[0].value);
        //DettaglioLavorazioneTerzisti(piva, 0, 0, $('#anno')[0].value)

        windowdettagliAnt = $('#griglia_dettagliAnticipazioni').kendoWindow({
            //$('#griglia_dettagliTerzisti').kendoWindow({
            title: "Dettagli Anticipazioni Colturali",
            modal: true,
            resizable: true,
            iframe: true,
            width: "90%",
            height: "80%",
            actions: ["Maximize", "Close"],
            close: function () {
                setTimeout(function () {
                    //$('#griglia_dettagliTerzisti').kendoWindow('destroy');           
                    windowdettagliAnt.close();
                }, 200);
            }
        }).data('kendoWindow') //.center();//.maximize();
    } else {
        windowdettagliAnt.open();
    }

    windowdettagliAnt.center();
}

///AGGIUNTO GLORIA///
function DettaglioSintesiRendicontazione() {

    var piva = KendoDDL("ddlAzienda").value();
    var url = GetUrlSintesiRendicontazione(piva);

    apriKendoWindowSintesiRendicontazione(url, "Sintesi Richieste e Rendicontazioni");

}

function DettaglioSintesiUMA() {

    var piva = KendoDDL("ddlAzienda").value();
    var anno = $('#anno')[0].value;
    var url = GetUrlSintesiUMA(piva, anno, QS_Type);

    apriKendoWindowSintesiRendicontazione(url, "Sintesi UMA");

}

function apriKendoWindowSintesiRendicontazione(url, title) {
    $(document.body).append('<div id="tab_sintesidocumentazione"></div>');
    $('#tab_sintesidocumentazione').kendoWindow({
        title: title,
        modal: true,
        resizable: true,
        iframe: true,
        width: "70%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () {
                $('#tab_sintesidocumentazione').kendoWindow('destroy');



            }, 200);
        }
    }).data('kendoWindow').center();//.maximize();
}

var windowdettagliterz
function LeggiLavorazioniTerzista() {

    var piva = KendoDDL("ddlAzienda").value();


    if (piva != undefined && piva != "") {
        if (windowdettagliterz == undefined) {
            MostraLavorazioneTerzisti(piva, 0, 0, $('#anno')[0].value)
            //controlloIncrociato(piva, 0, 0, $('#anno')[0].value);
            //DettaglioLavorazioneTerzisti(piva, 0, 0, $('#anno')[0].value)

            windowdettagliterz = $('#griglia_dettagliTerzisti').kendoWindow({
                //$('#griglia_dettagliTerzisti').kendoWindow({
                title: "Consulta Lavorazioni Terzisti",
                modal: true,
                resizable: true,
                iframe: true,
                width: "90%",
                height: "80%",
                actions: ["Maximize", "Close"],
                close: function () {
                    setTimeout(function () {
                        //$('#griglia_dettagliTerzisti').kendoWindow('destroy');           
                        windowdettagliterz.close();
                    }, 200);
                }
            }).data('kendoWindow') //.center();//.maximize();
        } else {
            windowdettagliterz.open();
        }

        windowdettagliterz.center();
    }

}

///FINE AGGIUNTO GLORIA

function apriKendoWindowTestataGriglia(url, title) {

    personalizzazioniGrigliaDocumenti = Salva_Filtri()

    $(document.body).append('<div id="tab_documentale"></div>');
    $('#tab_documentale').kendoWindow({
        title: title,
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () {
                $('#tab_documentale').kendoWindow('destroy');

                popolaGrigliaRichiestaDocumenti("grdRichiestaDocumenti");

                ddlAzienda_Change();
            }, 200);
        }
    }).data('kendoWindow').center().maximize();
}

/////AGGIUNTO GLORIA


function Fascicoli_InTemplate() {

    var template_header = kendo.template($("#headerFascicoli").html());
    var template_value = kendo.template($("#valueFascicoli").html());
    var template_template = kendo.template($("#templateFascicoli").html());

    if (ddlFascicoli[0] == undefined) {
        kendo.alert("Nessun fascicolo disponibile");
    } else {
        QS_Programmazione_Cod_Fascicolo = ddlFascicoli[0]['strProgrammazioniCod'].replaceAll("|", "").trim();
        //console.log("QS_Programmazione_Cod_Fascicolo: " + QS_Programmazione_Cod_Fascicolo)

        $("#fascicoloric").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: ddlFascicoli,
            headerTemplate: template_header,
            valueTemplate: template_value,
            template: template_template,
            QS_Programmazione_Cod_Fascicolo: "strProgrammazioneCod",
            //optionLabel: { "text": "SELEZIONA...", "value": "delete" },
            //value: defaultValue,
            change: function (e) {
                QS_Programmazione_Cod_Fascicolo = ddlFascicoli[e.sender.selectedIndex]['strProgrammazioniCod'].replaceAll("|", "").trim();
                //console.log("QS_Programmazione_Cod_Fascicolo: " + QS_Programmazione_Cod_Fascicolo)
            }
            //open: kendoDropDownAdjustWidth,
            //dataBound: kendoDropDownAdjustWidth,
            //change: function (e) {

            //    QS_Programmazione_Cod_Fascicolo = ddlFascicoli[0]['strProgrammazioniCod'].replaceAll("|", "").trim();

            //}

        });
    }
}









function DettagliAppezzamenti(IDControllo) {



    $("#" + IDControllo).html("");

    var funzioniCRUD = { funzioneRead: App_kReadValorizzazione_rows };


    var idModel = "chiave";
    var campiKendoModel = App_kReadValorizzazione_mod();
    var colonneKendoGrid = App_kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        //group: {
        //    field: "utilizzo", aggregates: [
        //        { field: "utilizzo_sup", aggregate: "sum" }
        //    ]
        //},
        aggregate: [{ field: "sup_tot", aggregate: "sum" }]
    };
    //var template = kendo.template($("#popupApp_TemplateRow").html());

    var colonneCustomKendoGrid = [];

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        scrollbars: true,
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        reorderable: true,
        pageable:
        {
            pageSize: 50,
            pageSizes: [5, 10, 20, 50, 100, "all"],
            buttonCount: 3
        },
        filterable: true,
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };

    var funzioniPrimaDopoEventi = {};

    var mostraRigheCancellate = false;
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

    //$("#" + IDControllo).data("kendoGrid").dataSource.pageSize(50);

    divAppezza_maxHeight = window.innerHeight * 0.65;//$('#kendo_Appezzamenti > .k-grid-content').height();
    divAppezza_minHeight = 400;
    if (divAppezza_maxHeight < divAppezza_minHeight) {
        divAppezza_minHeight = divAppezza_maxHeight;
    }
}

function App_kReadValorizzazione_rows(options) {

    var data = $('#grigliadettaglioval').val();

    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function App_kReadValorizzazione_mod() {

    var data = $('#grigliadettaglioval').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;

}

function App_kReadValorizzazione_col() {

    var data = $('#grigliadettaglioval').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function DettaglioAnticipazioniTerzisti(IDControllo) {

    var omettiAnnulla = true;

    var funzioneSubmitDaUsare = {};

    var funzioniCRUD = {
        funzioneRead: VisualizzaAnticipiTerzisti,
        funzioneSubmit: funzioneSubmitDaUsare,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var idModel = "Lavorazione_UMA";
    var campiKendoModel = null;

    campiKendoModel = {
        piva: { editable: false, type: "string" },
        Lavorazione_UMA: { editable: false, type: "number" },
        rag_soc: { editable: false, type: "string" },
        Lav_UMA_Des: { editable: false, type: "string" },
        Totale_Superficie_UMA: { editable: false, type: "number" },
        Fabbisogno_Assegnato: { editable: false, type: "number" },
        Tipo_Carburante: { editable: false, type: "number" },
        Car_Des: { editable: false, type: "string" }
    };

    var styleOut = "vertical-align: middle; text-align: center;";
    var styleRight = "vertical-align: middle; text-align: right;";
    var colonneKendoGrid = [
        //{ field: "Macrouso_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Macrouso_Des", "Gruppo colturale GIAS"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        //{ field: "piva", title: TraduzioneMultiResx(gestioneCarbResx, "Piva", "Piva"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "rag_soc", title: TraduzioneMultiResx(gestioneCarbResx, "rag_soc", "Ragione Sociale Terzista"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Lav_UMA_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Lav_UMA_Des", "Lavorazione U.M.A."), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Car_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Car_Des", "Carburante"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Fabbisogno_Assegnato", title: TraduzioneMultiResx(gestioneCarbResx, "Fabbisogno Assegnato", "Fabbisogno Assegnato (L)"), width: 200, attributes: { style: styleRight }, filterable: { multi: true, search: true } },
        { field: "Totale_Superficie_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "Totale Superficie UMA", "Superficie Lavorata (Ha)"), width: 200, attributes: { style: styleRight }, filterable: { multi: true, search: true }, format: "{0:n4}" },
        //{ field: "Tipo_Carburante", title: TraduzioneMultiResx(gestioneCarbResx, "TIpo_Carburante", "TIpo_Carburante"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }},
        //{ field: "Lavorazione_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "Lavorazione_UMA", "Lavorazione_UMA"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }},
    ];

    var parametriPerLettura = true;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        columnMenu: true,
        pageable: { pageSizes: [100] },
        pageSize: 100,
        groupable: true
        //salvaRipristinaPersonalizzazioni: { url: pathCoreWS }

    };
    var funzioniPrimaDopoEventi = {
        /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: onDataBoundGrigliaDettagliImpianti,*/
        //funzioneDaChiamarePrimaDelDetailInit: detailInitGrigliaDettagliLavorazioniTerzisti,
        /*funzioneDaChiamareDopoDataBound: App_onDataBoundImpianti*/
        /*funzioneDaChiamareDopoDelete: HideTabDettagli*/
    };

    var mostraRigheCancellate = false;
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


function DettaglioLavorazioneTerzisti(IDControllo) {

    var omettiAnnulla = true;

    var funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti, flagInsert: true, flagUpdate: true /*flagDelete: true*/ };

    var funzioniCRUD = {
        funzioneRead: VisualizzaLavTerzisti,
        funzioneSubmit: funzioneSubmitDaUsare,
        //UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        //UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var idModel = "Programmazione_cod";
    var campiKendoModel = null;

    campiKendoModel = {
        Programmazione_Des: { editable: false, type: "string" },
        Macrouso_UMA_Des: { editable: false, type: "string" },
        val_cod: { editable: false, type: "string" },
        rag_soc: { editable: false, type: "string" },
        Lav_UMA_Des: { editable: false, type: "string" },
        Totale_Superficie_UMA: { editable: false, type: "number" },
        Data_Modifica: { editable: false, type: "date" },
        Zona_Pendenza_A_UMA: { editable: false, type: "number" },
        Zona_Pendenza_B_UMA: { editable: false, type: "number" },
        Zona_Tessitura_Normale_UMA: { editable: false, type: "number" },
        Zona_Tessitura_Media_UMA: { editable: false, type: "number" },
        Zona_Tessitura_Tenace_UMA: { editable: false, type: "number" },

    };



    var styleOut = "vertical-align: middle; text-align: center;";
    var colonneKendoGrid = [
        //{ field: "Macrouso_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Macrouso_Des", "Gruppo colturale GIAS"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Programmazione_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Programmazione_Des", "Fascicoli"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Macrouso_UMA_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Macrouso_UMA_Des", "Gruppo colturale U.M.A."), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "val_cod", title: TraduzioneMultiResx(gestioneCarbResx, "val_cod", "CUUA Terzista"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "rag_soc", title: TraduzioneMultiResx(gestioneCarbResx, "rag_soc", "Ragione Sociale Terzista"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Lav_UMA_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Lav_UMA_Des", "Lavorazione U.M.A."), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Totale_Superficie_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "Totale_Superficie_UMA", "Superficie Lavorata (Ha)"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
        { field: "Data_Modifica", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Modifica", "Data Modifica"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, format: "{0:dd/MM/yyyy}" },
        { field: "Zona_Pendenza_A_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "Zona_Pendenza_A_UMA", "Pendenza A"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
        { field: "Zona_Pendenza_B_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "Zona_Pendenza_B_UMA", "Pendenza  B"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
        { field: "Zona_Tessitura_Normale_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "Zona_Tessitura_Normale_UMA", "Tessitura Normale"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
        { field: "Zona_Tessitura_Media_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "Zona_Tessitura_Media_UMA", "Tessitura Media"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
        { field: "Zona_Tessitura_Tenace_UMA", title: TraduzioneMultiResx(gestioneCarbResx, "Zona_Tessitura_Tenace_UMA", "Tessitura Tenace"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals },
    ];

    var parametriPerLettura = true;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        columnMenu: true,
        pageable: { pageSizes: [100] },
        pageSize: 100,
        groupable: true
        //salvaRipristinaPersonalizzazioni: { url: pathCoreWS }

    };
    var funzioniPrimaDopoEventi = {
        /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: onDataBoundGrigliaDettagliImpianti,*/
        //funzioneDaChiamarePrimaDelDetailInit: detailInitGrigliaDettagliLavorazioniTerzisti,
        /*funzioneDaChiamareDopoDataBound: App_onDataBoundImpianti*/
        /*funzioneDaChiamareDopoDelete: HideTabDettagli*/
    };

    var mostraRigheCancellate = false;
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

    //$("#" + IDControllo).data("kendoGrid").dataSource.pageSize(50);
    //var grid = $("#" + IDControllo).data("kendoGrid");
    //grid.bind("cellClose", gridImpianti_cellClose);



}

async function detailInitGrigliaDettagliLavorazioniTerzisti(e) {
    var AggiornaImpianti = true;
    var id_macrouso = e.data.Macrouso_UMA_Cod;
    /*console.log("macrouso " + id_macrouso)*/
    //var macrouso_UMA = parseInt(e.data.Macrouso_UMA_Cod);
    var Programmazione_Cod = e.data.Programmazione_Cod;
    var Programmazione_CodStr = e.data.Programmazione_Cod;
    if (Programmazione_Cod == -1) {
        Programmazione_CodStr = "_1";
    }
    var id_div = "GrigliaDettagliLavorazioni_" + id_macrouso + "_" + Programmazione_CodStr;
    //await LeggiLavorazioniAlternative(id_macrouso);
    /*await LeggiLavorazioniAlternativeLimitate(macrouso_UMA);
 
    if (lav_alt_lim_sup.length > 0) {
        if (lav_alt_lim[macrouso_UMA] == undefined) lav_alt_lim[macrouso_UMA] = {}
        for (var n = 0; n < lav_alt_lim_sup.length; n++) {
            var x = lav_alt_lim_sup[n]["Lavorazione_UMA"];
            lav_alt_lim[macrouso_UMA][x] = 0;
        }
    }*/
    if (QS_Type == 0 && QS_Avanzamento == 1) {
        if (lav_incrociati[id_macrouso] == undefined) {
            lav_incrociati[id_macrouso] = await controlloIncrociato(KendoDDL("ddlAzienda").value(), Programmazione_Cod, id_macrouso, $('#anno')[0].value);
        }
    }
    $("<div id='" + id_div + "' />").appendTo(e.detailCell);
    ElencoTabAperte.push(id_macrouso);
    //PopolaElencoLavUMA(false, parseInt(e.data.Macrouso_UMA_Cod), 0);
    //popolaGrigliaDettagliLavorazioni(id_div, AggiornaImpianti, id_macrouso, Programmazione_Cod);
}



function GrigliaDettagliColture(IDControllo, AggiornaImpianti) {

    var omettiAnnulla = true;

    var funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti, flagInsert: true, flagUpdate: true /*flagDelete: true*/ };

    var funzioniCRUD = {
        funzioneRead: LeggiRichiestaDettaglio,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var idModel = "Cod";
    var campiKendoModel = null;

    campiKendoModel = {
        Cod: { editable: false, type: "string" },
        Richiesta_Cod: { editable: false, type: "number" },
        Programmazione_Cod: { editable: false, type: "number" },
        Macrouso_Des: { editable: false, type: "String", validation: { required: true } },
        Macrouso_Cod: { editable: false, type: "String", validation: { required: true } },
        sup_tot: { editable: false, type: "number", validation: { required: true } },
        sup_A: { editable: false, type: "number", validation: { required: true } },
        sup_B: { editable: false, type: "number", validation: { required: true } },
        /*sup_UMA_Edit: { editable: modifica_richiesto, type: "number", validation: { required: true } },
        sup_UMA_A_Edit: { editable: modifica_richiesto, type: "number", validation: { required: true } },
        sup_UMA_B_Edit: { editable: modifica_richiesto, type: "number", validation: { required: true } },
        /*calcolato: { editable: false, type: "number", validation: { required: true } },
        richiesto: { editable: false, type: "number", validation: { required: true } },
        assegnato: { editable: modifica_assegnato, type: "number", validation: { required: true } },*/
        TerrenoNormale: { editable: false, type: "number", validation: { required: true } },
        TerrenoMedio: { editable: false, type: "number", validation: { required: true } },
        TerrenoTenace: { editable: false, type: "number", validation: { required: true } },

        Destinazione_Cod: { editable: false, type: "string", validation: { required: true } },
        Uso_Cod: { editable: false, type: "string", validation: { required: true } },
        Qualita_Cod: { editable: false, type: "string", validation: { required: true } },
        Occupazione_Cod: { editable: false, type: "string", validation: { required: true } },

        /*TerrenoNormale_Edit: { editable: modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoMedio_Edit: { editable: modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoTenace_Edit: { editable: modifica_richiesto, type: "number", validation: { required: true } },*/
    };

    //var footerTemplateStringsupUMA = "Totale superficie dichiarata U.M.A.: <span id='footerPlaceholderSupUMA" + "'>#=calcTotaleColonna('" + "sup_UMA" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupUMA_A = "Totale zona A U.M.A.: <span id='footerPlaceholderSupUMA_A" + "'>#=calcTotaleColonna('" + "sup_UMA_A" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupUMA_B = "Totale zona B U.M.A.: <span id='footerPlaceholderSupUMA_B" + "'>#=calcTotaleColonna('" + "sup_UMA_B" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupFASC = "Totale superficie da fascicolo: <span id='footerPlaceholderSupFASC" + "'>#=calcTotaleColonna('" + "sup_FASC" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupFASC_A = "Totale zona A fascicolo: <span id='footerPlaceholderSupFASC_A" + "'>#=calcTotaleColonna('" + "sup_FASC_A" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringsupFASC_B = "Totale zona B fascicolo: <span id='footerPlaceholderSupFASC_B" + "'>#=calcTotaleColonna('" + "sup_FASC_B" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringCalcolato = "Totale carburante calcolato: <span id='footerPlaceholderCalcolato" + "'>#=calcTotaleColonna('" + "calcolato" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringRichiesto = "Totale carburante richiesto: <span id='footerPlaceholderRichiesto" + "'>#=calcTotaleColonna('" + "richiesto" + "', " + IDControllo + ")#</span>";
    //var footerTemplateStringAssegnato = "Totale carburante assegnato: <span id='footerPlaceholderAssegnato" + "'>#=calcTotaleColonna('" + "assegnato" + "', " + IDControllo + ")#</span>";
    var footerTemplateStringTerrNorm = "Totale terreno normale: <span id='footerPlaceholderTerrNorm" + "'>#=calcTotaleColonna('" + "TerrenoNormale" + "', " + IDControllo + ")#</span>";
    var footerTemplateStringTerrMed = "Totale terreno medio: <span id='footerPlaceholderTerrMed" + "'>#=calcTotaleColonna('" + "TerrenoMedio" + "', " + IDControllo + ")#</span>";
    var footerTemplateStringTerrTen = "Totale terreno tenace: <span id='footerPlaceholderTerrTen" + "'>#=calcTotaleColonna('" + "TerrenoTenace" + "', " + IDControllo + ")#</span>";

    var footerTemplateStringsupUMA = "#=calcTotaleColonna('" + "sup_tot" + "', " + IDControllo + ", true, 0, 3)#";
    var footerTemplateStringsupUMA_A = "#=calcTotaleColonna('" + "sup_A" + "', " + IDControllo + ", true, 0, 3)#";
    var footerTemplateStringsupUMA_B = "#=calcTotaleColonna('" + "sup_B" + "', " + IDControllo + ", true, 0, 3)#";
    /*var footerTemplateStringsupFASC = "#=calcTotaleColonna('" + "sup_UMA_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringsupFASC_A = "#=calcTotaleColonna('" + "sup_UMA_A_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringsupFASC_B = "#=calcTotaleColonna('" + "sup_UMA_B_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringCalcolato = "#=calcTotaleColonna('" + "calcolato" + "', " + IDControllo + ")#";
    var footerTemplateStringRichiesto = "#=calcTotaleColonna('" + "richiesto" + "', " + IDControllo + ")#";
    var footerTemplateStringAssegnato = "#=calcTotaleColonna('" + "assegnato" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrNorm = "#=calcTotaleColonna('" + "TerrenoNormale_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrMed = "#=calcTotaleColonna('" + "TerrenoMedio_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrTen = "#=calcTotaleColonna('" + "TerrenoTenace_Edit" + "', " + IDControllo + ")#";*/

    var styleOut = "vertical-align: middle; text-align: center;";
    var colonneKendoGrid = [
        { field: "Macrouso_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Macrouso_Des", "Gruppo colturale"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "sup_tot", title: TraduzioneMultiResx(gestioneCarbResx, "sup_tot", "Superficie dichiarata (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupUMA },
        { field: "sup_A", title: TraduzioneMultiResx(gestioneCarbResx, "sup_A", "Dettaglio zona A (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupUMA_A },
        { field: "sup_B", title: TraduzioneMultiResx(gestioneCarbResx, "sup_B", "Dettaglio zona B (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupUMA_B },
        { field: "Destinazione_Cod", title: TraduzioneMultiResx(gestioneCarbResx, "Destinazione_Cod", "Destinazione_Cod"), headerAttributes: { style: styleOut }, hidden: true },
        { field: "Uso_Cod", title: TraduzioneMultiResx(gestioneCarbResx, "Uso_Cod", "Uso_Cod"), headerAttributes: { style: styleOut }, hidden: true },
        { field: "Qualita_Cod", title: TraduzioneMultiResx(gestioneCarbResx, "Qualita_Cod", "Qualita_Cod"), headerAttributes: { style: styleOut }, hidden: true },
        { field: "Occupazione_Cod", title: TraduzioneMultiResx(gestioneCarbResx, "Occupazione_Cod", "Occupazione_Cod"), headerAttributes: { style: styleOut }, hidden: true },
        //{ field: "sup_UMA_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "sup_FASC", "Superficie dichiarata U.M.A. (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupFASC, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals },
        /*{ field: "calcolato", title: TraduzioneMultiResx(gestioneCarbResx, "calcolato", "Carburante Calcolato (lt)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringCalcolato, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals },
        { field: "richiesto", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto", "Carburante Richiesto (lt)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringRichiesto, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals },
        { field: "assegnato", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato", "Carburante Assegnato (lt)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringAssegnato, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals },*/
        //{ field: "sup_UMA_A_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "sup_FASC_A", "Dettaglio zona A (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupFASC_A, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals },
        //{ field: "sup_UMA_B_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "sup_FASC_B", "Dettaglio zona B (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringsupFASC_B, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals },
    ];

    var colonneTerreno = [];
    colonneTerreno.push({ field: "TerrenoNormale", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoNormale", "Normale (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrNorm });
    colonneTerreno.push({ field: "TerrenoMedio", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoMedio", "Medio (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrMed });
    colonneTerreno.push({ field: "TerrenoTenace", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoTenace", "Tenace (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrTen });
    //colonneKendoGrid.push({ field: "TerrenoNormale_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoNormale", "Normale (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrNorm, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals });
    //colonneKendoGrid.push({ field: "TerrenoMedio_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoMedio", "Medio (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrMed, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals });
    //colonneKendoGrid.push({ field: "TerrenoTenace_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoTenace", "Tenace (Ha)"), headerAttributes: { style: styleOut }, footerTemplate: footerTemplateStringTerrTen, filterable: { multi: true, search: true }, format: "{0:n4}", editor: numberEditor4decimals });

    colonneKendoGrid.push({
        title: TraduzioneMultiResx(gestioneCarbResx, "TipoTerreno", "Tipo di Terreno"),
        headerAttributes: { style: styleOut },
        columns: colonneTerreno
    });

    var parametriPerLettura = true;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        columnMenu: true,
        pageable: { pageSizes: [100] },
        pageSize: 100,
        groupable: false,
        /*colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=DettaglioColture(this.closest('tr'),this.closest('.k-grid'))><span class='fa fa-search lampeggiante'></span>" + Traduzione(gestioneCarbResx, "Info", "Info") + "</div>"
                    //"<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaRichiesta(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(gestioneCarbResx, "Modifica", "Modifica") + "</div>" +
                    //"<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=cancellaRichiesta(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(gestioneCarbResx, "Cancella", "Cancella") + "</div>"
                }, title: Traduzione(gestioneCarbResx, "Dettaglio Coltura", "Dettaglio Coltura"), width: "97px", headerAttributes: { style: styleOut }
            }
        ]*/
    };
    var funzioniPrimaDopoEventi = {
        /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: onDataBoundGrigliaDettagliImpianti,*/
        /*funzioneDaChiamarePrimaDelDetailInit: detailInitGrigliaDettagliLavorazioni,
        funzioneDaChiamareDopoDataBound: App_onDataBoundImpianti*/
        /*funzioneDaChiamareDopoDelete: HideTabDettagli*/
    };

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

    $("#" + IDControllo).data("kendoGrid").dataSource.pageSize(50);
    var grid = $("#" + IDControllo).data("kendoGrid");
    grid.bind("cellClose", gridImpianti_cellClose);

}

function popolaGrigliaSpostaAppezzamenti(idDiv, sa_Cod, data) {
    var funzioniCRUD = {
        funzioneRead: function (options) {
            options.success(data);
        }, checkBoxFunction: checkSpostaAppezzamenti
    };

    var idModel = "chiave";
    var campiKendoModel = modelGrigliaSpostaAppezzamento();
    var colonneKendoGrid = colonneGrigliaSpostaAppezzamento();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    //var template = kendo.template($("#popupApp_Template").html());
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        pageable:
        {
            pageSize: 100,
            pageSizes: [5, 10, 20, 50, 100, "all"],
            buttonCount: 3
        },
        colonneCustomKendoGrid: [],
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" }
    };


    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: App_onDataBoundCatastoParticella };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(idDiv, // rappresenta l'ID del div a cui si associa la griglia
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


function modelGrigliaSpostaAppezzamento() {
    var a =
    {
        "chiave": {
            "editable": false,
            "type": "string"
        },
        "Piva": {
            "editable": false,
            "type": "string"
        },
        "Sa_cod": {
            "editable": false,
            "type": "number"
        },
        "Appezza": {
            "editable": false,
            "type": "number"
        },
        "App_Nome": {
            "editable": false,
            "type": "string"
        },
        "Sup_App": {
            "editable": false,
            "type": "number"
        },
        "movimentato": {
            "editable": false,
            "type": "string"
        },
        "bloccato": {
            "editable": false,
            "type": "string"
        },
        "Utilizzo": {
            "editable": false,
            "type": "string"
        }
    };

    return a;
}

function colonneGrigliaSpostaAppezzamento() {
    var a = [
        //{
        //    "field": "chiave",
        //    "title": "chiave",
        //    "filterable": false
        //},
        //{
        //    "field": "Piva",
        //    "title": "Piva",
        //    "filterable": false
        //},
        //{
        //    "field": "Sa_cod",
        //    "title": "Sa_cod",
        //    "filterable": false
        //},
        //{
        //    "field": "Appezza",
        //    "title": "Appezza",
        //    "filterable": false
        //},
        {
            "field": "App_Nome",
            "title": "Nome",
            "filterable": false
        },
        {
            "field": "Utilizzo",
            "title": "Utilizzo",
            "filterable": false
        },
        {
            "field": "Sup_App",
            "title": "Sup [ha]",
            "filterable": false
        }
        //{
        //    "field": "movimentato",
        //    "title": "movimentato",
        //    "filterable": false
        //},

    ];
    return a;
}


//da togliere
function checkSpostaAppezzamenti(e) {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var idDiv;
    var search = true;
    $(this).parents().each(function (index, item) {
        if (search == true && $(item).attr('id') != undefined && $(item).attr('id').includes("appezzamenti_") && $(item).attr('id').split('_').length == 2) {
            idDiv = $(item).attr('id');
            search = false;
        }
    });
    var grid = $('#' + idDiv).data("kendoGrid");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    dataItem.dirty = true;
    rowKendoGridSelected(row, checked)
}

async function AssegnaAutomaticamenteCarburante() {
    WaitFrame.show();
    await WS_AssegnaAutomaticamenteCarburante(richiesta_cod);
    ddlAzienda_Change();
    $("#btn_carica_richieste").trigger("click");
    ucUmaAllevamenti_reloadGrid();
    ucUmaLavParziali_reloadGrid();
    WaitFrame.hide();
}

async function ddlFascicoli_Load(target) {
    return new Promise(async function (resolve, reject) {
        var template_header = kendo.template($("#headerFascicoli").html());
        var template_value = kendo.template($("#valueFascicoli").html());
        var template_template = kendo.template($("#templateFascicoli").html());

        ddlFascicoli = await WS_Fascicoli();
        if (ddlFascicoli.lenght > 0)
            QS_Programmazione_Cod_Fascicolo = ddlFascicoli[0]['strProgrammazioniCod'].replaceAll("|", "").trim();

        $(target).kendoDropDownList({
            filter: "contains",
            dataSource: ddlFascicoli,
            dataTextField: "text",
            dataValueField: "value",
            headerTemplate: template_header,
            valueTemplate: template_value,
            template: template_template,
            QS_Programmazione_Cod_Fascicolo: "strProgrammazioneCod",
            //optionLabel: { "text": "SELEZIONA...", "value": "delete" },
            //value: defaultValue,
            change: function (e) {
                QS_Programmazione_Cod_Fascicolo = ddlFascicoli[e.sender.selectedIndex]['strProgrammazioniCod'].replaceAll("|", "").trim();
            },
            dataBound: function (e) {
                resolve();
            }
        });
        //alert("variabile " + ddlFascicoli[0]['strProgrammazioniCod'].replaceAll("|", "").trim())

        resolve();

    });
}

function PopolaValori_Setup(anno) {
    return new Promise(async function (resolve, reject) {
        let SetupVals = await LeggiSetup(anno);
        /*console.log("LeggiSetup", SetupVals);*/
        if (SetupVals.length > 0) {
            percentualeZonaPendenzaB = parseFloat((SetupVals[0].Per_Mag_Terreno_B / 100).toFixed(4));
            percentualeTerrenoMedio = parseFloat((SetupVals[0].Per_Mag_Terreno_Medio / 100).toFixed(4));
            percentualeTerrenoTenace = parseFloat((SetupVals[0].Per_Mag_Terreno_Tenace / 100).toFixed(4));
            Percentuale_Decurtamento = SetupVals[0].Per_Riduzione;
            gestioneBiologico = SetupVals[0].Gestione_Biologico === 0 ? false : true;
            Percentuale_Anticipo_Carb = parseFloat((SetupVals[0].Percentuale_Richieste_Anticipo / 100).toFixed(4));
            Gestione_Rimanenze = SetupVals[0].Gestione_Rimanenze;
            L_Maggiorazione_Trasferimenti = SetupVals[0].Nr_Litri_Maggiorazione
            if (Percentuale_Decurtamento != 0)
                $("#btn_AssegnaAutomaticamenteCarburante").html(`Assegna automaticamente carburante (Richiesto - ${Percentuale_Decurtamento}%)`);
            else
                $("#btn_AssegnaAutomaticamenteCarburante").html(`Assegna automaticamente carburante Richiesto`);





        }
        resolve();
    });
}
function CopiaDatidaConfermare() {
    $("#TxtriassegnabiliGasolioConferma").val($("#TxtriassegnabiliGasolio").val());
    $("#TxtriassegnabiliBenzinaConferma").val($("#TxtriassegnabiliBenzina").val());
    $("#TxtriassegnabiliGasolio_SerraConferma").val($("#TxtriassegnabiliGasolio_Serra").val());

    $("#TxtrecuperoacciseGasolioConferma").val($("#TxtrecuperoacciseGasolio").val());
    $("#TxtrecuperoacciseBenzinaConferma").val($("#TxtrecuperoacciseBenzina").val());
    $("#TxtrecuperoacciseGasolio_SerraConferma").val($("#TxtrecuperoacciseGasolio_Serra").val());


    var grdRestituzioni = $("#grdRestituzioni").data("kendoGrid");
    var dataItems = grdRestituzioni.dataSource.view();
    for (var i = 0, l = dataItems.length; i < l; i++) {
        grdRestituzioni.dataSource.data()[i].Confermato_Gasolio = grdRestituzioni.dataSource.data()[i].Gasolio;
        grdRestituzioni.dataSource.data()[i].Confermato_Benzina = grdRestituzioni.dataSource.data()[i].Benzina;
        grdRestituzioni.dataSource.data()[i].Confermato_Gasolio_Serra = grdRestituzioni.dataSource.data()[i].Gasolio_Serra;
        grdRestituzioni.dataSource.data()[i].dirty = true;
    }
    grdRestituzioni.refresh();

    var grdTrasferimenti = $("#grdTrasferimenti").data("kendoGrid");
    var dataItemsgrdTrasferimenti = grdTrasferimenti.dataSource.view();

    for (var i = 0, l = dataItemsgrdTrasferimenti.length; i < l; i++) {
        grdTrasferimenti.dataSource.data()[i].Confermato_Gasolio = grdTrasferimenti.dataSource.data()[i].Gasolio;
        grdTrasferimenti.dataSource.data()[i].Confermato_Benzina = grdTrasferimenti.dataSource.data()[i].Benzina;
        grdTrasferimenti.dataSource.data()[i].Confermato_Gasolio_Serra = grdTrasferimenti.dataSource.data()[i].Gasolio_Serra;
        grdTrasferimenti.dataSource.data()[i].Confermato_Tipo_Rich = grdTrasferimenti.dataSource.data()[i].Tipo_Richiesta;
        grdTrasferimenti.dataSource.data()[i].Confermato_Tipo_Rich_Des = grdTrasferimenti.dataSource.data()[i].Tipo_Richiesta_Des;
        grdTrasferimenti.dataSource.data()[i].dirty = true;
    }
    grdTrasferimenti.refresh();

}

async function btnStampaRichRendiconClick(ev) {
    var linkStampa = await StampaRichRendicon(
        KendoDDL("ddlAzienda").value(),
        QS_Avanzamento,
        richiesta_cod,
    );

    window.open(linkStampa, "_blank");
}

function btnOpzStampaIstruttoriaClick(ev) {
    $("#tabstrip_dettagli").data("kendoTabStrip").select(".Verb");
    document.querySelector(".Verb").scrollIntoView({ behavior: "smooth" });
}

async function btnStampaIstruttoriaClick(ev) {
    var valEsito = Get_KendoDDLValue("ddlIstrEsito", 0);
    if (valEsito != 0) {
        var linkStampa = await StampaIstruttoria(
            KendoDDL("ddlAzienda").value(),
            QS_Avanzamento,
            richiesta_cod,
            getKendoSwitch("ksIstrModDati"),
            getKendoSwitch("ksIstrSegnMacchine"),
            valEsito,
            $("#txtAreaIstrNote").data("kendoTextArea").value()
        );

        window.open(linkStampa, "_blank");
    } else {
        kendo.alert("Occorre specificare l'esito dell'istruttoria");
    }
}

function gestionePassaggioDiStato(tr_elem, grid_elem) {

    if (anomalieSuperficiAppezzamenti != null && $("#stato_pratica_cod").val() == In_Compilazione.toString()) {
        kendo.alert("Non è possibile avanzare la richiesta: è necessario prima riallineare le superfici dichiarate alle superfici effetive rilevate per gli appezzamenti.")
        return false
    }

    if (QS_Avanzamento == 1) Leggi_Date_Ins_Rendicontazione()

    //Se sono in richiesta cooperativa, devo controllare che sia stato inserito almeno un CUAA, sennò non ha senso avanzare con la richiesta
    if (QS_Avanzamento == 0 && get_TipoAzienda() == Cooperativa_Agricola) {
        //NB: passo Azienda_Terzista per sfruttare la funzione già esistente
        let ignoraEliminatiRichiesta = true
        listaCUAARendicontati = [] //Resetto la lista ogni volta
        get_ListaCUAA_Richiesti(Cooperativa_Agricola, ignoraEliminatiRichiesta)

        if (listaCUAARendicontati.length == 0) {
            kendo.alert("Non è possibile avanzare la richiesta " + $("#anno").val() + " senza aver inserito almeno un CUAA")
            return false
        }
    }

    if (consentitoAggiungereModificareRendicontazione_daSetup || (permesso_approvazione_richiesta && permesso_approvazione_rendicontazione)) {

        var parametri = {
            Richiesta_Cod: richiesta_cod
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

    } else {
        kendo.alert("Non è possibile avanzare la rendicontazione dell'anno " + $("#anno").val() + " dopo il " + Data_Fine_Rendicontazione)  //a quest" + (QS_Type != -1 ? "a azienda" : "o terzista") + ". ");
        return;
    }
}

function apriGestionePassaggioDiStato(url) {
    $(document.body).append('<div id="GestionePassaggioDiStatoWindow"></div>');
    $('#GestionePassaggioDiStatoWindow').kendoWindow({
        title: "Avanzamento Pratica",
        modal: true,
        resizable: true,
        iframe: true,
        width: "60%",
        height: "45%",
        content: url,
        close: function () {
            setTimeout(function () {
                storage.clear();
                $('#GestionePassaggioDiStatoWindow').kendoWindow('destroy');
                //$("#btn_CercaPratiche").trigger("click");
            }, 200);
        }
    }).data('kendoWindow').center();
}

function chiudiWindowPassaggioDiStato(msg) {
    storage.clear();
    setTimeout(function () {
        $('#GestionePassaggioDiStatoWindow').kendoWindow('destroy');
        if (QS_Richiesta > 0) {
            location.reload();
        } else {
            Azione_Indietro();
        }
    }, 200);

}

// IL BOTTONE FINTO MI SIMULA IL CLICK DEL PULSANTE VERO
function openFileDialogFinto() {
    $("#File_Allegato").click();


}

// SCRIVO IL PERCORSO DEL FILE SULLA TEXTBOX
function scriviPercorsoFileSuTxt() {
    var nomeFile = $("#File_Allegato").val().replace("C:\\fakepath\\", "");
    $('#Txt_Documento_Allegato').val(nomeFile);

}



function getTemplateUpdateDocumento() {

    let anno = parseInt($("#anno").val());

    let anno_precedente = anno - 1;

    let content = '';

    content += '<div id="pnlAllegato">';
    content += '<div class="row">';
    content += '<div class="col-lg-10 col-md-10">';
    content += '<div class="form-horizontal">';
    content += '<div class="form-group">';
    content += '<div id="lbl_NuovaRichiestaDaRendicontazione" class="input-group">';
    content += '<h5>';
    content += "Si vuole creare una richiesta di assegnazione di carburante precompilata i cui dati sono mutuati dalla rendicontazione " + anno_precedente + " precedentemente presentata ed approvata?<br>";
    content += "Se si, è obbligatorio allegare una dichiarazione sostitutiva dell'atto di notorietà con la quale, tra l'altro, si dichiara di non prevedere riduzioni di superficie per l’anno " + anno + ".";
    content += '</h5>';
    content += '</div>';
    content += '</div>';
    content += '</div>';
    content += '</div>';
    content += '</div>';
    content += '<div class="row">';
    content += '<div class="col-lg-10 col-md-10">';
    content += '<div class="form-horizontal">';
    content += '<div class="form-group">';
    content += '<div class="input-group">';
    content += '<span class="input-group-addon alert-info" id="lbl_Documento_Allegato">Allegato</span>';
    content += '<span class="input-group-btn">';
    content += '<button id="Btn_Sfoglia_Allegato" class="btn btn-default" type="button" onclick="openFileDialogFinto();" style="font-size: 18px">';
    content += '<span class="fa fa-folder-open fa-3"></span>';
    content += '</button>';
    content += '</span>';
    content += '<input type="text" id="Txt_Documento_Allegato" class="form-control" readonly="readonly" style="font-size: 18px" />';
    content += '<input type="file" id="File_Allegato" onchange="scriviPercorsoFileSuTxt();" style="display: none;" />';
    content += '<input type="hidden" id="File_Caricato" />';
    content += '</div>';
    content += '</div>';
    content += '</div>';
    content += '</div>';
    content += '</div>';
    content += '</div>';

    return content;
}


var handleFileSelect = function (evt) {
    var file = evt.target.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (readerEvt) {
            var binaryString = readerEvt.target.result;
            $('#File_Caricato').val(btoa(binaryString));
        };
        reader.readAsBinaryString(file);


    }
};

async function LavorazioniMultiple(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    $(document.body).append('<div id="griglia_LavorazioniMultiple"></div>');

    /*macro_cod = datiRiga.Macrouso_UMA_Cod;
    program_cod = datiRiga.Programmazione_Cod;
    Veg_Cod = datiRiga.Veg_Cod;
    Id_Cod = datiRiga.Id_Cod;
    if (macro_cod != undefined && macro_cod != "") {
        GrigliaDettagliColture("griglia_dettagliColture", true);*/

    var parentRow = $(grid_elem).parents(".k-detail-row").prev();
    var parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    lavorazioni = await PopolaElencoLavUMAMultiple(false, parentRowItem.Macrouso_UMA_Cod, 0, parentRowItem.Regolamento_Cod);
    popolaGrigliaLavorazioniMultiple("griglia_LavorazioniMultiple")
    $("#griglia_LavorazioniMultiple").append('<div class="btn btn-success" id="btn_conferma" style="margin-top: 10px" onclick="ConfermaLavorazioniMultiple();"><span class="fa fa-check lampeggiante"></span><span class="lampeggiante">Conferma</span></div>')
    $('#griglia_LavorazioniMultiple').kendoWindow({
        title: "Inserisci Lavorazioni Multiple",
        modal: true,
        resizable: true,
        iframe: true,
        width: "90%",
        height: "80%",
        actions: ["Maximize", "Close"],
        close: function () {
            InsertLavorazioniMultiple(grid_elem, datiRiga)
            lavorazioniSelezionate = [];
            setTimeout(function () {
                $('#griglia_LavorazioniMultiple').kendoWindow('destroy');
            }, 200);
        }
    }).data('kendoWindow').center();//.maximize();

}

function InsertLavorazioniMultiple(IDgriglia, rigaOrigin) {
    var riga;
    var griglia = $(IDgriglia).data('kendoGrid');
    var parentRow = $(IDgriglia).parents(".k-detail-row").prev();
    var parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);
    if (lavorazioniSelezionate.length > 0) {
        lavorazioniSelezionate.forEach(l => {
            if (rigaOrigin.LAV_COD == 0) {
                rigaOrigin.nLavRichieste = 1;
                rigaOrigin.nLavPreviste = 1;
                rigaOrigin.piuLavPreviste = 1;
                rigaOrigin.LAV_COD = l.LAV_COD;
            } else {
                griglia.addRow();
            }
            riga = griglia._data[0];
            riga.LavUMA = l.LavUMA;
            riga.Lav_UMA_Cod = l.Lav_UMA_Cod.toString();
            riga.LavGIAS = l.LavGIAS;
            riga.LAV_COD = l.LAV_COD.toString();
            riga.nLavRichieste = rigaOrigin.nLavRichieste;
            riga.nLavPreviste = rigaOrigin.nLavPreviste;
            riga.piuLavPreviste = rigaOrigin.piuLavPreviste;
            riga.Superficie_Trattata = rigaOrigin.Superficie_Trattata;
            riga.SupMaggiorazioneTrasferimenti = rigaOrigin.SupMaggiorazioneTrasferimenti;
            riga.Sup_B = rigaOrigin.Sup_B;
            riga.Sup_A = rigaOrigin.Sup_A;
            riga.TerrenoMedio = rigaOrigin.TerrenoMedio;
            riga.TerrenoNormale = rigaOrigin.TerrenoNormale;
            riga.TerrenoTenace = rigaOrigin.TerrenoTenace;
            riga.Validita_Inizio = rigaOrigin.Validita_Inizio;
            riga.dirtyFields.Validita_Inizio = true;
            riga.Car_Cod = rigaOrigin.Car_Cod;
            riga.TipoCarb = rigaOrigin.TipoCarb;
            let attivita = TrovaAttivitaGIAS(false, parentRowItem.Macrouso_UMA_Cod, riga.Lav_UMA_Cod, riga.LAV_COD, parentRowItem.Regolamento_Cod)

            if (attivita != undefined) {
                riga.Attivita_Des = attivita.Attivita_Des;
                riga.Attivita_Cod = attivita.Attivita_Cod;
            }

            let n_lav_previste = recupera_nLavPreviste(riga.Lav_UMA_Cod, parentRowItem.Macrouso_UMA_Cod, riga.LAV_COD, parentRowItem.Regolamento_Cod)
            if (n_lav_previste > 1 && n_lav_previste != riga.nLavPreviste) {
                riga.nLavPreviste = n_lav_previste;
            }

            if (l.Lav_UMA_Cod == 10064)
                riga.Udm_Alt = "m"
            calcoloFabbisogno(parentRowItem, riga, griglia);
        });

        griglia.refresh();
    }
}

function AggiungiMacchina(e) {
    var url = GetUrlDocAgenda2010(QS_Piva, 0, 0, 0, 0, "", ANAGRAFICA)
    apriKendoWindowAnagraficaMacchine(url, "Gestione Macchine U.M.A. Carburanti");
}

function apriKendoWindowAnagraficaMacchine(url, title) {

    window.removeEventListener('message', chiudiFinestraCreaMacchina);
    window.addEventListener('message', chiudiFinestraCreaMacchina);

    $(document.body).append('<div id="tab_anagrafica_macchine"></div>');
    $('#tab_anagrafica_macchine').kendoWindow({
        title: title,
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () {
                $('#tab_anagrafica_macchine').kendoWindow('destroy');
                aggiornaGrigliaMacchine()
            }, 200);
        }
    }).data('kendoWindow').center().maximize();
}

function chiudiFinestraCreaMacchina(event) {
    if ((typeof event.data == "string") && event.data.includes("chiudiFinestra")) {
        $('#tab_anagrafica_macchine').data('kendoWindow').close()
        kendo.alert("Macchina salvata con successo!")
        aggiornaGrigliaMacchine()
    }
}

async function aggiornaGrigliaMacchine() {
    var gridMacchine = $("#tab_griglia_macchine").data("kendoGrid"),
        itemsMacchine = gridMacchine.dataSource.view(),
        macchineSelezionate = []

    //Salvo le macchine selezionate 
    itemsMacchine.forEach(function (macchina, ind) {
        if (macchina.Selected == true) {
            macchineSelezionate.push(macchina.chiave)
        }
    }, this);

    //Aggiorno la griglia e le variabili
    await PopolaGrigliaMacchine(KendoDDL("ddlAzienda").value(), richiesta_cod);
    gridMacchine = $("#tab_griglia_macchine").data("kendoGrid")
    itemsMacchine = gridMacchine.dataSource.view()


    //Reimposto le macchine selezionate in precedenza
    macchineSelezionate.forEach(function (selezionate) {
        itemsMacchine.forEach(function (macchina, ind) {
            if (macchina.chiave === selezionate) {

                macchina.Selected = true;
                macchina.dirty = true;

                var rows = gridMacchine.tbody.find("tr"),
                    row = $(rows[ind]);
                row.find("input[type=checkbox]").eq(0).prop("checked", true)

                rowKendoGridSelected(row, true)

                let dataItem = gridMacchine.dataItem(row);

                controllaTargaObbligatoria(row, dataItem);
            }
        }, this);
    }, this);
}

function get_TipoAzienda() {
    if (QS_TipoAzienda != Azienda_Agricola_Privata) {
        //Se sono entrata dai pulsanti Coop o Impresa Agromeccanica, inserisco il tipo azienda passato in QS 
        return QS_TipoAzienda
    } else if (KendoDDL("ddlAzienda").dataItem().forma_giuridica == Consorzio_Di_Bonifica) {
        //Controllo se l'azienda ha impostato la forma giuridica CONSORZIO DI BONIFICA
        return Consorzio_Bonifica_Irrigazione
    } else if (KendoDDL("ddlAzienda").dataItem().flagPubblica == '1') {
        //Controllo se l'azienda ha impostato una forma giuridica di tipo pubblico
        return Azienda_Agricola_Istituzioni_Pubbliche
    } else {
        //Se nessuna delle precedenti, è un'azienda privata
        return Azienda_Agricola_Privata
    }
}


function controlloLtApprovatiInseriti(tipo_carburante) {
    var objSommeLt = {
        somma_LtGasolioAssegnati: 0,
        somma_LtGasolioSerraAssegnati: 0,
        somma_LtGasolioBenzinaAssegnati: 0
    }

    //LEGGO IL MASSIMALE DI LT ASSEGNATI IN GRIGLIA PER OGNI TIPO DI CARBURANTE
    let gridId = (get_TipoAzienda() == Azienda_Terzista ? "tab_griglia_terzistiparziale" : "tab_griglia_terzisti")

    if (get_TipoAzienda() == Azienda_Terzista) {
        if ($("#" + gridId).data("kendoGrid") !== undefined) {
            var lavorazioni = $("#" + gridId).data("kendoGrid"),
                items = lavorazioni.dataSource.data();

            //Scorro tutti gli elementi della griglia e mi salvo i lt totali per ogni carburante
            items.forEach(function (lavorazione) {
                sommaLitri(objSommeLt, lavorazione.Car_Cod, lavorazione.ltAssegnato)
            }, this);
        }
    } else {
        //Se non sono state aperte le griglie delle lavorazioni, la tabella non esiste
        //mi salvo questi ID e vado a cercare su db i valori salvati
        var listaLavorazioniNonTrovate = []

        if ($("#" + gridId).data("kendoGrid") !== undefined) {
            var richieste = $("#" + gridId).data("kendoGrid"),
                itemsR = richieste.dataSource.data();

            itemsR.forEach(function (richiesta) {

                var idGridLavorazione = "GrigliaDettagliLavorazioni_" + richiesta.CUAA + "_" + richiesta.Programmazione_Cod.toString() + "_" + richiesta.Macrouso_UMA_Cod
                var lavorazioni = $("#" + idGridLavorazione).data("kendoGrid")

                if (lavorazioni !== undefined) {
                    var itemsL = lavorazioni.dataSource.data();
                    itemsL.forEach(function (lavorazione) {
                        sommaLitri(objSommeLt, lavorazione.Car_Cod, lavorazione.ltAssegnato)
                    }, this);
                } else {
                    listaLavorazioniNonTrovate.push(idGridLavorazione + '_' + richiesta.piva)
                }
            }, this);

            listaLavorazioniNonTrovate.forEach(function (lavorazione) {
                let split = lavorazione.split("_")
                let Programmazione_Cod = split[2],
                    Macrouso_UMA_Cod = split[3],
                    PivaLavorazione = split[4]

                leggiSommeCarburanti_daDB(objSommeLt, Programmazione_Cod, Macrouso_UMA_Cod, PivaLavorazione)

            })

        }
    }


    var LtInseriti_Gasolio = parseInt($("#gasolioTerzistiAppro")[0].value),
        LtInseriti_GasolioSerra = parseInt($("#gasolioSerraTerzistiAppro")[0].value),
        LtInseriti_Benzina = parseInt($("#benzinaTerzistiAppro")[0].value)


    switch (tipo_carburante) {
        case Gasolio:
            if (LtInseriti_Gasolio > objSommeLt.somma_LtGasolioAssegnati || isNaN(LtInseriti_Gasolio)) {
                gasolioTerzAppro.value(objSommeLt.somma_LtGasolioAssegnati)
            }
            break;
        case Gasolio_Serra:
            if (LtInseriti_GasolioSerra > objSommeLt.somma_LtGasolioSerraAssegnati || isNaN(LtInseriti_GasolioSerra)) {
                gasolioSerraTerzAppro.value(objSommeLt.somma_LtGasolioSerraAssegnati)
            }

            break;
        case Benzina:
            if (LtInseriti_Benzina > objSommeLt.somma_LtGasolioBenzinaAssegnati || isNaN(LtInseriti_Benzina)) {
                benzinaTerzAppro.value(objSommeLt.somma_LtGasolioBenzinaAssegnati)
            }
            break;
    }
}

function sommaLitri(objSomme, tipo_carburante, ltAssegnato) {
    switch (tipo_carburante) {
        case Gasolio.toString():
            objSomme.somma_LtGasolioAssegnati += ltAssegnato
            break;
        case Gasolio_Serra.toString():
            objSomme.somma_LtGasolioSerraAssegnati += ltAssegnato
            break;
        case Benzina.toString():
            objSomme.somma_LtGasolioBenzinaAssegnati += ltAssegnato
            break;
    }
}

function Salva_Filtri() {

    var idControlloTestata = "grdRichiestaDocumenti"
    var griglia = [];

    var parametriGrigliaTestata = getParametriGrigliaTestata(location.pathname, idControlloTestata);

    griglia.push(CreaNuovoOggettoGriglia(idControlloTestata, parametriGrigliaTestata));

    return griglia;
}

function getParametriGrigliaTestata(pagina, nomeDiv) {

    var jsonDaSalvare = "";
    var grid = $('#' + nomeDiv).data('kendoGrid');
    if (grid !== undefined) {
        var dataSource = grid.dataSource;
        var columns = grid.columns;
        var pageSize = dataSource.pageSize();
        var sort = dataSource.sort();
        var filter = dataSource.filter();
        var group = dataSource.group();
        var page = dataSource.page();

        //ultimaRigaSelezionataGrigliaTestata.page = page;
        var currentRow = 0; //ultimaRigaSelezionataGrigliaTestata;
        var options = { pagina: pagina, nomeDiv: nomeDiv, columns: columns, page: page, pageSize: pageSize, sort: sort, filter: filter, group: group, currentRow: currentRow };
        jsonDaSalvare = kendo.stringify(options);
    }
    return jsonDaSalvare;
}

function CreaNuovoOggettoGriglia(idControllo, valore) {
    var griglia = new Object();
    griglia.IdControllo = idControllo;
    griglia.Personalizzazioni = valore;

    return griglia;
}

function Applica_Personalizzazioni_Griglie() {

    if (personalizzazioniGrigliaDocumenti != null && personalizzazioniGrigliaDocumenti != undefined && personalizzazioniGrigliaDocumenti != "") {

        for (var i = 0; i < personalizzazioniGrigliaDocumenti.length; i++) {

            var obj = personalizzazioniGrigliaDocumenti[i];
            var personalizzazioni = obj.Personalizzazioni;
            var grid = $("#" + 'grdRichiestaDocumenti').data("kendoGrid");
            if (grid != null && grid != undefined && personalizzazioni != null && personalizzazioni != undefined && personalizzazioni != "") {
                setPersonalizzazioniGrigliaKendo(grid, JSON.parse(personalizzazioni));
                personalizzazioniGrigliaDocumenti[i].Personalizzazioni = null;
            }
        }
    }
}

function setPersonalizzazioniGrigliaKendo(grid, options) {

    try {

        var dataSource = grid.dataSource;
        var savedColumns = options.columns;

        //NUMERO DI RIGHE PER PAGINA
        if (options.pageSize)
            dataSource.pageSize(options.pageSize);

        if (options.page)
            dataSource.page(options.page);


        //RIORDINAMENTO COLONNE
        var indOrd = 0;
        for (i = 0; i < savedColumns.length; i++) {
            let col;

            //Cerco la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
            if (savedColumns[i].field && savedColumns[i].field != null) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].field == savedColumns[i].field; });
            } else if (savedColumns[i].title) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].title == savedColumns[i].title; });
            }

            //Se ho trovato la colonna...
            if (col) {
                //Sposto la colonna in testa
                if (savedColumns[i].hidden != true)
                    grid.reorderColumn(indOrd, col);
                indOrd++;
            }

        }

        //MOSTRO O NASCONDO COLONNE
        for (i = savedColumns.length - 1; i >= 0; i--) {
            let col;

            //Cerco la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
            if (savedColumns[i].field && savedColumns[i].field != null) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].field == savedColumns[i].field; });
            } else if (savedColumns[i].title) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].title == savedColumns[i].title; });
            }

            //Se ho trovato la colonna...
            if (col) {
                if (savedColumns[i].hidden == true) {
                    grid.hideColumn(col);
                } else { //else if (savedColumns[i].hidden == false) 
                    grid.showColumn(col);
                }
            }
        }

        //ORDINAMENTO (ASC/DESC) DELLE COLONNE
        if (options.sort) {

            var ordinam = options.sort;
            for (i = ordinam.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == ordinam[i].field; });

                if (col == undefined) {
                    ordinam.splice(i, 1);
                }
            }

            if (ordinam.length > 0)
                dataSource.sort(options.sort);
        }

        //FILTRI PER COLONNE
        if (options.filter) {

            var filtri = options.filter.filters;
            for (i = filtri.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == filtri[i].field; });

                if (col == undefined) {
                    filtri.splice(i, 1);
                }

                // Sistemazione della data che era stata memorizzata a db in formato GMT
                if (col.field !== undefined && grid.dataSource.options.schema.model.fields[col.field].type === "date")
                    filtri[i].value = kendo.parseDate(filtri[i].value);
            }

            if (filtri.length > 0)
                dataSource.filter(options.filter);
        }

        //RAGGRUPPAMENTO IN TOOLBAR
        if (options.group) {

            var raggrup = options.group;
            for (i = raggrup.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == raggrup[i].field; });

                if (col == undefined) {
                    raggrup.splice(i, 1);
                }
            }

            if (raggrup.length > 0)
                dataSource.group(options.group);
        }

    } catch (err) {
        console.log(err);
    }
}

function getMaggiorazioneAutomatica(macrousoUmaCod, lavorazioneUmaCod) {
    if (macrousoUmaCod === macrousoCod_EccedenzaAnticipi && lavorazioneUmaCod === lavorazioneCod_QuotaAnticipoEccedente) {
        return true;
    }
    if (macrousoUmaCod === macrousoCod_EccedenzaAnticipi && lavorazioneUmaCod === lavorazioneCod_QuotaAnticipoEccedenteAccisePagate) {
        return true;
    }
    if (macrousoUmaCod === macrousoCod_TrasferimentiEffettuati && lavorazioneUmaCod === lavorazioneCod_QuotaTrasferita) {
        return true;
    }
    return false;
}


function GrigliaTrasferiti(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: CaricatrasferimentiCarburanteDaDB,
        funzioneSubmit: { funzione: () => { }, flagInsert: modifica_rimanenze, flagUpdate: true, flagDelete: modifica_rimanenze },
        UtenteAbilitatoInserimentoModifica: true,
        UtenteAbilitatoCancellazione: true,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false,
    };



    var idModel = "Piva_Ricevente";
    var campiKendoModel = GrigliaTrasferitiCampiKendoModel();
    var colonneKendoGrid = GrigliaTrasferitiColonneKendoGrid();
    var parametriPerLettura = [];
    var parametriDataSource = {
        aggregate: [{ field: "utilizzo_sup", aggregate: "sum" }]
    };

    var colonneCustomKendoGrid = [];

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        scrollbars: true,
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        reorderable: true,

        filterable: true,

    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: App_onDataBoundTrasferimenti,
        funzioneDaChiamareDopoEdit: onEditGrigliaTrasferimenti
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["CUAA"];

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
    var grid = $("#" + IDControllo).data("kendoGrid");
    //$("#" + IDControllo).data("kendoGrid").dataSource.pageSize(50);
    grid.bind("cellClose", grid_cellCloseTrasferiti);
    //grid.bind("remove", grid_remove);

}

function App_onDataBoundTrasferimenti(e) {
    coloraRighe_Trasferimenti("#grdTrasferimenti", e);
}

function onEditGrigliaTrasferimenti(e) {
    if (!e.model.isNew() && $(e.container[0]).hasClass("cpt")) {
        //(e.model.Confermato_Tipo_Rich_Des === e.model.Tipo_Richiesta_Des && e.container.index() === 10)) {
        e.sender.closeCell();
    }
}

//Grigliarestituzioni("grdRestituzioni");
//GrigliaTrasferiti("grdTrasferimenti");

function coloraRighe_Trasferimenti(grid_elem, e) {
    var grid = $(grid_elem).data('kendoGrid');
    //var items = e.sender.items();
    //var columns = e.sender.columns;
    var indexColumnData_Trasferimento = grid.wrapper.find(".k-grid-header [data-field=" + "Data_Trasferimento" + "]").index();
    var indexColumnTipo_Richiesta_Des = grid.wrapper.find(".k-grid-header [data-field=" + "Tipo_Richiesta_Des" + "]").index();
    var indexColumnCUAA = grid.wrapper.find(".k-grid-header [data-field=" + "CUAA" + "]").index();
    var indexColumnConfermato_Tipo_Rich_Des = grid.wrapper.find(".k-grid-header [data-field=" + "Confermato_Tipo_Rich_Des" + "]").index();

    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        if ($("#stato_pratica_cod").val() === In_Compilazione.toString()) {
            ControllaDoppi(grid, dataItem, row, indexColumnCUAA, true);

            if (dataItem.Data_Trasferimento === "" || dataItem.Data_Trasferimento.getTime() === new Date('1900/1/1').getTime()) {
                dataItem.Data_Trasferimento = "";
                AddErrorClass(row, indexColumnData_Trasferimento, errorCell, "Compilare tutti i campi obbligatori");
            } else {
                RemoveErrorClass(row, indexColumnData_Trasferimento, errorCell);
            }

            if (dataItem.Tipo_Richiesta_Des === "") {
                AddErrorClass(row, indexColumnTipo_Richiesta_Des, errorCell, "Compilare tutti i campi obbligatori");
            } else {
                RemoveErrorClass(row, indexColumnTipo_Richiesta_Des, errorCell);
            }
        }
        if ($("#stato_pratica_cod").val() === Verifica_In_Corso.toString()) {
            if (dataItem.Confermato_Tipo_Rich_Des === "") {
                AddErrorClass(row, indexColumnConfermato_Tipo_Rich_Des, errorCell, "Compilare tutti i campi obbligatori");
            } else {
                RemoveErrorClass(row, indexColumnConfermato_Tipo_Rich_Des, errorCell);
            }

        }

    }

}


function ControllaDoppi(grid, dataItem, row, indexColumnCUAA, trasferimenti) {
    let data = grid.dataSource.data();

    let datiFiltrati = {};
    let msgErrore = "";

    if (trasferimenti) {
        datiFiltrati = data.filter(el => {
            return el.CUAA === dataItem.CUAA &&
                el.Tipo_Richiesta_Des === dataItem.Tipo_Richiesta_Des
        });
        msgErrore = "Non è possibile inserire CUAA-Conto destinazione più volte";
    } else {
        datiFiltrati = data.filter(el => {
            return el.Distributore_Des === dataItem.Distributore_Des
        });
        msgErrore = "Non è possibile inserire lo stesso CUAA più volte"
    }

    if (datiFiltrati.length > 1) {
        AddErrorClass(row, indexColumnCUAA, errorCell, msgErrore)
    }
}


async function SubmitTrasferiti(options) {
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var GrRest = $("#grdTrasferimenti").data("kendoGrid");
    var currentData = GrRest.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            currentData[i].Tipo_Richiesta += 1;
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].deleted) {
            deletedRecords.push(currentData[i].toJSON());
        }
    }

    //for (let i = 0; i < GrRest.dataSource._destroyed.length; i++) {
    //    deletedRecords.push(GrRest.dataSource._destroyed[i].toJSON());
    //}

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        let res = await InviaTrasferimentiModificati(newRecords, updatedRecords, deletedRecords);

        if (res) {
            let grid = $("#grdTrasferimenti").data("kendoGrid");
            grid.dataSource.read();
            grid.refresh();
            grid.saveChanges()
        }
    }


    //for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
    //    ImpostaCampiDefault(grid.dataSource._destroyed[i]);
    //    deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    //}

}

function GrigliaTrasferitiCampiKendoModel() {
    return {
        Piva_SuperUser: { editable: true, type: "string", defaultValue: "" },
        Piva: { editable: true, type: "string", defaultValue: KendoDDL("ddlAzienda").value() },
        Richiesta_Cod: { editable: true, type: "number", defaultValue: QS_Richiesta },
        Piva_Ricevente: { editable: true, type: "string", defaultValue: "" },
        Tipo_Richiesta: { editable: true, type: "number" },
        Gasolio: { editable: modifica_rimanenze, type: "number", defaultValue: 0 },
        Benzina: { editable: modifica_rimanenze, type: "number", defaultValue: 0 },
        Gasolio_Serra: { editable: modifica_rimanenze, type: "number", defaultValue: 0 },
        Data_Trasferimento: { editable: modifica_rimanenze, type: "date", defaultValue: new Date("1/1/1900") },
        Confermato_Gasolio: { editable: modifica_rimanenze_conferma, type: "number", defaultValue: 0 },
        Confermato_Benzina: { editable: modifica_rimanenze_conferma, type: "number", defaultValue: 0 },
        Confermato_Gasolio_Serra: { editable: modifica_rimanenze_conferma, type: "number", defaultValue: 0 },
        Confermato_Tipo_Rich: { editable: true, type: "number", defaultValue: -2 },
        inviato: { editable: false, type: "number", defaultValue: 0 },
        datainvio: { editable: false, type: "date", defaultValue: new Date("1/1/1900") },
        Data_Creazione: { editable: false, type: "date", defaultValue: new Date("1/1/1900") },
        Data_Modifica: { editable: false, type: "date", defaultValue: new Date("1/1/1900") },
        Username_Creazione: { editable: false, type: "string", defaultValue: "" },
        Username_Modifica: { editable: false, type: "string", defaultValue: "" },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1/1/1900") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("1/1/1900") },
        CUAA: { editable: modifica_rimanenze, type: "string", defaultValue: "" },
        rag_soc: { editable: false, type: "string", defaultValue: "" },
        Tipo_Richiesta_Des: { editable: modifica_rimanenze, type: "string", defaultValue: "" },
        Confermato_Tipo_Rich_Des: { editable: modifica_rimanenze_conferma, type: "string", defaultValue: "" },

    };
}

function GrigliaTrasferitiColonneKendoGrid() {
    return [
        {
            field: "CUAA",
            title: TraduciLavorazioni("CUAA Ricevente", "CUAA Ricevente"),
        },
        {
            field: "rag_soc",
            title: TraduciLavorazioni("Azienda", "Azienda"),
        },
        {
            field: "Gasolio",
            title: TraduciLavorazioni("Gasolio", "Gasolio"),
        },
        {
            field: "Benzina",
            title: TraduciLavorazioni("Benzina", "Benzina"),
        },
        {
            field: "Gasolio_Serra",
            title: TraduciLavorazioni("Gasolio Serra", "Gasolio Serra"),
        },
        {
            field: "Data_Trasferimento",
            title: TraduciLavorazioni("Data Trasferimento", "Data Trasferimento"),
            format: "{0:dd/MM/yyyy}",
            template: '#= (kendo.toString(Data_Trasferimento, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Trasferimento, "dd/MM/yyyy" ) #',
            validation: { required: true }
        },

        {
            field: "Tipo_Richiesta_Des",
            title: TraduciLavorazioni("Destinato c/proprio o c/terzi", "Destinato c/proprio o c/terzi"),
            editor: Destinatario_CreaDropDownList,
            filterable: { multi: true, search: true },
            attributes: { class: "cpt" },
            validation: { required: true }
        },

        {
            field: "Confermato_Gasolio",
            title: TraduciLavorazioni("Confermato Gasolio", "Confermato Gasolio"),
        },
        {
            field: "Confermato_Benzina",
            title: TraduciLavorazioni("Confermato Benzina", "Confermato Benzina"),
        },
        {
            field: "Confermato_Gasolio_Serra",
            title: TraduciLavorazioni("Confermato Gasolio Serra", "Confermato Gasolio Serra"),
        },
        {
            field: "Confermato_Tipo_Rich_Des",
            title: TraduciLavorazioni("Confermato destinato c/proprio o c/terzi", "Confermato destinato c/proprio o c/terzi"),
            editor: ConfermaDestinatario_CreaDropDownList,
            filterable: { multi: true, search: true },
            validation: { required: true }
        },

    ];
}

function grid_cellCloseTrasferiti(e) {
    //controlli sui campi appena modificati
    if (e.model.dirty === true) {

        var fieldName = e.container.find("input").attr("name");

        var gridID = e.sender.element[0].id;
        var grid = $("#" + gridID).data("kendoGrid");
        var row = e.container.select().closest("tr");
        //superficie totale maggiore di zero
        if (e.model.dirtyFields.sup_tot) {
            if (e.model.sup_tot < 0 || e.model.sup_tot == undefined) {
                e.model.sup_tot = 0;
                e.model.dirtyFields.sup_tot = true;
            }
            //coloraRighe_Richieste($("#" + gridID), e);
        }
        //controllo del CUAA per reperire la rag_soc dell'azienda e il numero di iscrizione alla camera di commercio
        if (fieldName === "CUAA" && e.model.CUAA != "") {

            //var indexColumnCUAA = grid.wrapper.find(".k-grid-header [data-field=" + "CUAA" + "]").index();

            var daCUAA = TrovaAziendaDaCUAA(e.model.CUAA);
            e.model.Piva_Ricevente = daCUAA.Item2
            pivaInsertGrid = daCUAA.Item2
            e.model.rag_soc = daCUAA.Item1
            grid.refresh();

            //nIscrizioneCameraDiCommercio[daCUAA.Item2] = LeggiNumeroIscrizioneCdC(e.model.piva);

        }
        //litri richiesti maggiore di zero

        //coloraRighe_Richieste($("#" + gridID), e)
        grid.refresh();

    }
}

function GrigliaRestituzioniCampiKendoModel() {
    return {
        Piva_SuperUser: { editable: false, type: "string", defaultValue: "" },
        Piva: { editable: false, type: "string", defaultValue: KendoDDL("ddlAzienda").value() },
        Richiesta_Cod: { editable: false, type: "number", defaultValue: QS_Richiesta },
        Piva_Distributore: { editable: false, type: "string", defaultValue: 0 },
        Gasolio: { editable: modifica_rimanenze, type: "number", defaultValue: 0 },
        Benzina: { editable: modifica_rimanenze, type: "number", defaultValue: 0 },
        Gasolio_Serra: { editable: modifica_rimanenze, type: "number", defaultValue: 0 },
        Data_Trasferimento: { editable: modifica_rimanenze, type: "date", defaultValue: new Date("1900/1/1") },
        Confermato_Gasolio: { editable: modifica_rimanenze_conferma, type: "number", defaultValue: 0 },
        Confermato_Benzina: { editable: modifica_rimanenze_conferma, type: "number", defaultValue: 0 },
        Confermato_Gasolio_Serra: { editable: modifica_rimanenze_conferma, type: "number", defaultValue: 0 },
        inviato: { editable: false, type: "number", defaultValue: 0 },
        datainvio: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Data_Creazione: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { editable: false, type: "string", defaultValue: "" },
        Username_Modifica: { editable: false, type: "string", defaultValue: "" },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Distributore_Des: { editable: true, type: "string", defaultValue: "" },
    };
}

function GrigliaRestituzioniColonneKendoGrid() {
    return [
        {
            field: "Distributore_Des",
            title: TraduciLavorazioni("Distributore Carburante", "Distributore Carburante"),
            editor: Distributore_CreaDropDownList,
            filterable: { multi: true, search: true },
            validation: { required: true }
        },
        {
            field: "Gasolio",
            title: TraduciLavorazioni("Gasolio", "Gasolio"),

        },
        {
            field: "Benzina",
            title: TraduciLavorazioni("Benzina", "Benzina"),
        },
        {
            field: "Gasolio_Serra",
            title: TraduciLavorazioni("Gasolio Serra", "Gasolio Serra"),
        },
        {
            field: "Data_Trasferimento",
            title: TraduciLavorazioni("Data Trasferimento", "Data Trasferimento"),
            format: "{0:dd/MM/yyyy}",
            template: '#= (kendo.toString(Data_Trasferimento, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Trasferimento, "dd/MM/yyyy" ) #',

        },

        {
            field: "Confermato_Gasolio",
            title: TraduciLavorazioni("Confermato Gasolio", "Confermato Gasolio"),
        },
        {
            field: "Confermato_Benzina",
            title: TraduciLavorazioni("Confermato Benzina", "Confermato Benzina"),
        },
        {
            field: "Confermato_Gasolio_Serra",
            title: TraduciLavorazioni("Confermato Gasolio Serra", "Confermato Gasolio Serra"),
        },


    ];
}

async function SubmitRestituzioni(options) {

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var GrRest = $("#grdRestituzioni").data("kendoGrid");
    var currentData = GrRest.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            currentData[i].Tipo_Richiesta += 1;
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].deleted) {
            deletedRecords.push(currentData[i].toJSON());
        }
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        let res = await InviaRestituzioniModificati(newRecords, updatedRecords, deletedRecords);

        if (res) {
            let grid = $("#grdRestituzioni").data("kendoGrid");
            grid.dataSource.read();
            grid.refresh();
            grid.saveChanges();
        }
    }

}

function Grigliarestituzioni(IDControllo) {



    $("#" + IDControllo).html("");

    var funzioniCRUD = {
        funzioneRead: CaricaRestituzioniCarburanteDaDB,
        funzioneSubmit: { funzione: () => { }, flagInsert: modifica_rimanenze, flagUpdate: true, flagDelete: modifica_rimanenze },
        UtenteAbilitatoInserimentoModifica: true,
        UtenteAbilitatoCancellazione: true,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false,
    };


    var idModel = "Piva_Distributore";
    var campiKendoModel = GrigliaRestituzioniCampiKendoModel();
    var colonneKendoGrid = GrigliaRestituzioniColonneKendoGrid();
    var parametriPerLettura = null;
    var parametriDataSource = {
        //group: {
        //    field: "utilizzo", aggregates: [
        //        { field: "utilizzo_sup", aggregate: "sum" }
        //    ]
        //},
        aggregate: [{ field: "utilizzo_sup", aggregate: "sum" }]
    };
    //var template = kendo.template($("#popupApp_TemplateRow").html());

    var colonneCustomKendoGrid = [];

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        scrollbars: true,
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        reorderable: true,

        filterable: true,

    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: App_onDataBoundRestituzioni
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["Piva_Distributore"];

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

    var grid = $("#" + IDControllo).data("kendoGrid");
    //$("#" + IDControllo).data("kendoGrid").dataSource.pageSize(50);
    grid.bind("cellClose", grid_cellCloseRestituzioni);

}

function App_onDataBoundRestituzioni(e) {
    coloraRighe_Restituzioni("#grdRestituzioni", e);
}

function coloraRighe_Restituzioni(grid_elem, e) {
    var grid = $(grid_elem).data('kendoGrid');
    //var items = e.sender.items();
    //var columns = e.sender.columns;
    var indexColumnData_Trasferimento = grid.wrapper.find(".k-grid-header [data-field=" + "Data_Trasferimento" + "]").index();
    var indexColumnCUAA = grid.wrapper.find(".k-grid-header [data-field=" + "Distributore_Des" + "]").index();
    //var indexColumnConfermato_Tipo_Rich_Des = grid.wrapper.find(".k-grid-header [data-field=" + "Confermato_Tipo_Rich_Des" + "]").index();

    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        if ($("#stato_pratica_cod").val() === In_Compilazione.toString()) {
            ControllaDoppi(grid, dataItem, row, indexColumnCUAA, false);

            if (dataItem.Data_Trasferimento === "" || dataItem.Data_Trasferimento.getTime() === new Date('1900/1/1').getTime()) {
                dataItem.Data_Trasferimento = "";
                AddErrorClass(row, indexColumnData_Trasferimento, errorCell, "Compilare tutti i campi obbligatori");
            } else {
                RemoveErrorClass(row, indexColumnData_Trasferimento, errorCell);
            }

        }
        if ($("#stato_pratica_cod").val() === Verifica_In_Corso.toString()) {
            //if (dataItem.Confermato_Tipo_Rich_Des === "") {
            //    AddErrorClass(row, indexColumnConfermato_Tipo_Rich_Des, errorCell, "Compilare tutti i campi obbligatori");
            //} else {
            //    RemoveErrorClass(row, indexColumnConfermato_Tipo_Rich_Des, errorCell);
            //}

        }

    }

}

function grid_cellCloseRestituzioni(e) {
    //controlli sui campi appena modificati
    if (e.model.dirty === true) {

        var fieldName = e.container.find("input").attr("name");

        var gridID = e.sender.element[0].id;
        var grid = $("#" + gridID).data("kendoGrid");
        var row = e.container.select().closest("tr");

        if (e.model.dirtyFields.Gasolio) {
            if (e.model.Gasolio < 0 || e.model.Gasolio == undefined) {
                e.model.Gasolio = 0;
                e.model.dirtyFields.Gasolio = true;
            }
        }
        if (e.model.dirtyFields.Benzina) {
            if (e.model.Benzina < 0 || e.model.Benzina == undefined) {
                e.model.Benzina = 0;
                e.model.dirtyFields.Benzina = true;
            }
        }
        if (e.model.dirtyFields.Gasolio_Serra) {
            if (e.model.Gasolio_Serra < 0 || e.model.Gasolio_Serra == undefined) {
                e.model.Gasolio_Serra = 0;
                e.model.dirtyFields.Gasolio_Serra = true;
            }
        }
        if (e.model.dirtyFields.Confermato_Gasolio) {
            if (e.model.Confermato_Gasolio < 0 || e.model.Confermato_Gasolio == undefined) {
                e.model.Confermato_Gasolio = 0;
                e.model.dirtyFields.Confermato_Gasolio = true;
            }
        }
        if (e.model.dirtyFields.Confermato_Benzina) {
            if (e.model.Confermato_Benzina < 0 || e.model.Confermato_Benzina == undefined) {
                e.model.Confermato_Benzina = 0;
                e.model.dirtyFields.Confermato_Benzina = true;
            }
        }
        if (e.model.dirtyFields.Confermato_Gasolio_Serra) {
            if (e.model.Confermato_Gasolio_Serra < 0 || e.model.Confermato_Gasolio_Serra == undefined) {
                e.model.Confermato_Gasolio_Serra = 0;
                e.model.dirtyFields.Confermato_Gasolio_Serra = true;
            }
        }

        //coloraRighe_Richieste($("#" + gridID), e)
        grid.refresh();

    }
}

function Distributore_CreaDropDownList(container, options) {
    if (imprese === null) {
        imprese = RicercaImprese(false);
        imprese = imprese.map(x => { return { Piva_Distributore: x.piva, Distributore_Des: x.rag_soc } });
    }

    creaDropDownEditor(container, "Distributore_Des", "Piva_Distributore", imprese, onChange_DistributoreVenditore);
    if (options.model.isNew() && options.model.Azienda_Venditore_Cod === "" && imprese.length === 1) {
        options.model.Piva_Distributore = imprese[0].Piva_Distributore;
        options.model.Distributore_Des = imprese[0].Distributore_Des;
        grid.closeCell();
    }
}

function onChange_DistributoreVenditore(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#grdRestituzioni").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Piva_Distributore = dataItem.Piva_Distributore;
    model.Distributore_Des = dataItem.Distributore_Des;

}

function Destinatario_CreaDropDownList(container, options) {

    let elencoOrd = new Array();
    elencoOrd.push(
        {
            Tipo_Richiesta_Des: "C/PROPRIO",
            Tipo_Richiesta: -1,
        }
    );
    elencoOrd.push(
        {
            Tipo_Richiesta_Des: "C/TERZI",
            Tipo_Richiesta: -2,
        }
    );
    elencoOrd.unshift(
        {
            Tipo_Richiesta_Des: "",
            Tipo_Richiesta: 0,
        }
    );

    creaDropDownEditor(container, "Tipo_Richiesta_Des", "Tipo_Richiesta", elencoOrd, onChange_Destinatario);

}
function onChange_Destinatario(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#grdTrasferimenti").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Tipo_Richiesta = dataItem.Tipo_Richiesta;
    model.Tipo_Richiesta_Des = dataItem.Tipo_Richiesta_Des;
    model.dirty = true;
    grid.refresh();
}

function ConfermaDestinatario_CreaDropDownList(container, options) {
    let elencoOrd = new Array();
    elencoOrd.push(
        {
            Confermato_Tipo_Rich_Des: "C/PROPRIO",
            Confermato_Tipo_Rich: 0,
        }
    );
    elencoOrd.push(
        {
            Confermato_Tipo_Rich_Des: "C/TERZI",
            Confermato_Tipo_Rich: -1,
        }
    );
    elencoOrd.push(
        {
            Confermato_Tipo_Rich_Des: "",
            Confermato_Tipo_Rich: -2,
        }
    );

    creaDropDownEditor(container, "Confermato_Tipo_Rich_Des", "Confermato_Tipo_Rich", elencoOrd, onChange_ConfermaDestinatario);

}
function onChange_ConfermaDestinatario(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#grdTrasferimenti").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Confermato_Tipo_Rich = dataItem.Confermato_Tipo_Rich;
    model.Confermato_Tipo_Rich_Des = dataItem.Confermato_Tipo_Rich_Des;
    grid.refresh();
}

function gestioneRimanenzeAbilitata() {
    return (Gestione_Rimanenze === 1 || (Gestione_Rimanenze === 2 && QS_Avanzamento === 1))
}

function ApriFrameAnalisiTerreno(e) {
    if (grigliaInModifica()) {
        kendo.alert("Sono state rilevate delle modifiche in corso. Salvare la pratica o annullare le modifiche per poter visualizzare le Analisi del terreno.")
    } else {
        var url = GetUrlAnalisi2010(QS_Piva)
        apriKendoWindowAnalisiTerreno(url, "Analisi Terreno U.M.A. Carburanti");
    }
}

//Controllo se sono presenti delle modifiche in corso sulla griglia principale o 
//una delle sottogriglie delle lavorazioni
function grigliaInModifica() {
    var x = $("#tab_griglia_dettagliImpianti").data("kendoGrid")._data
    var dirties = $.grep(x, function (e) { return e.dirty == true; })

    if (dirties.length > 0) {
        return true;
    } else {
        var tabelleAperte = $("#tab_griglia_dettagliImpianti").find("div[id^=GrigliaDettagliLavorazioni]");

        let modificheFatte = false;
        for (var i = 0; i < tabelleAperte.length; i++) {
            var datiA = $("#" + tabelleAperte[i].id).data("kendoGrid").dataSource.data();

            let dirty = datiA.filter((el) => { return el.dirty == true });

            if (dirty.length > 0) {
                modificheFatte = true;
            }
        }

        if (modificheFatte) {
            return true;
        }
    }
    return false;
}

function chiudiFinestraAnalisiTerreno(event) {
    if (verificaOriginSecondaria(window, window.origin, event) && (typeof event.data != null) && (event.data.messaggio != null) &&
        event.data.messaggio.includes("chiudiWindowGiasNG")) {
        $('#iframe_analisi_terreno').data('kendoWindow').close();
    }
}

function apriKendoWindowAnalisiTerreno(url, title) {

    if (QS_UsaAnalisiTerrenoNG === 1) {
        window.removeEventListener('message', chiudiFinestraAnalisiTerreno);
        window.addEventListener('message', chiudiFinestraAnalisiTerreno);
    }

    $(document.body).append('<div id="iframe_analisi_terreno"></div>');
    $('#iframe_analisi_terreno').kendoWindow({
        title: title,
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () {
                $('#iframe_analisi_terreno').kendoWindow('destroy');
                verificaAppezzamentiEffettuata = false;
                $("#tab_griglia_dettagliImpianti").data("kendoGrid").refresh()
            }, 200);
        }
    }).data('kendoWindow').center().maximize();
}
