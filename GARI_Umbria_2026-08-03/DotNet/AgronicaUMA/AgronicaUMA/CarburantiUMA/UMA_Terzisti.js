
var elencoAziende;
var dtRichiestaTerzista = "";
var pivaInsertGrid = "";


var listaCUAARendicontati = []
var listaCUAAPresentiInRichiesta = []
var Data_Limite_INS_Azienda = ""
var consentitoAggiungereNuoviCUAA = null

$("#btn_nuova_richiesta_terzista").click(
    async function () {
        $("#tab_griglia_terzisti").show();
        await LeggiRichiesteTerzista(KendoDDL("ddlAzienda").value());
        //ddlAzienda_Change();
        $("#btn_salva").show();
        NuovaRichiestaTerzistiGriglia()
    });

/*$("#btn_aggiorna_carb").click(
    async function () {
        if (richiesta_cod > -1)
            await AggiornaRichiesteCarburanti(KendoDDL("ddlAzienda").value(), $("#benzinaTerzisti")[0].value, $("#gasolioTerzisti")[0].value, $("#gasolioSerraTerzisti")[0].value);
        //ddlAzienda_Change();
        });*/

async function InsertRichiestaTerzisti(e) {

    if ($("#tab_griglia_terzisti").find(".errorCell").length != 0) {
        kendo.alert("Verificare i dati segnalati prima di salvare");
        return false;
    }

    let modificheFatte = false;
    let r_cod = 0
    if (e.data.created.length > 0) {
        modificheFatte = true;
        r_cod = e.data.created[0].Richiesta_Cod
        //Richiesta_Cod = e.data.created[0].Richiesta_Cod;
        //Macrouso_UMA_Cod = e.data.created[0].Macrouso_UMA_Cod;
    }
    if (e.data.updated.length > 0) {
        modificheFatte = true;
        r_cod = e.data.updated[0].Richiesta_Cod
        //Richiesta_Cod = e.data.updated[0].Richiesta_Cod;
        //Macrouso_UMA_Cod = e.data.updated[0].Macrouso_UMA_Cod;
    }
    if (e.data.destroyed.length > 0) {
        modificheFatte = true;
        r_cod = e.data.destroyed[0].Richiesta_Cod
        //Richiesta_Cod = e.data.destroyed[0].Richiesta_Cod;
        //Macrouso_UMA_Cod = e.data.destroyed[0].Macrouso_UMA_Cod;
    }
    if (modificheFatte) {
        await ws_Inserisci_Richieste_Terzisti(KendoDDL("ddlAzienda").value(), r_cod, e.data.created, e.data.updated, e.data.destroyed);
        await LeggiRichiesteTerzista(KendoDDL("ddlAzienda").value());
        $("#btn_nuova_richiesta_terzista").trigger("click");
    }
}

function NuovaRichiestaTerzistiGriglia(listaCUAARichiesti, consentitoAggiungereModificareRendicontazione_daSetup) {
    listaCUAARichiesti = listaCUAARichiesti;
    if (QS_Avanzamento == 1 && consentitoAggiungereNuoviCUAA == null) {
        Leggi_Data_Limite_INS_Azienda();
    }


    var IDControllo = "tab_griglia_terzisti"
    //controllo sullo stato della pratica per definire i campi editabili
    switch ($("#stato_pratica_cod").val()) {
        case "":
        case "2001":
            if (permesso_richiesta && consentitoAggiungereModificareRendicontazione_daSetup) {
                //modifica_richiesto = true;
                modifica_assegnato = false;
                $("#btn_salva").show();
                $("#gasolioTerzisti").prop('disabled', false);
                $("#benzinaTerzisti").prop('disabled', false);
                $("#gasolioSerraTerzisti").prop('disabled', false);
            }
            break;
        case "2002":
            if (permesso_approvazione_richiesta) {
                modifica_richiesto = false;
                //modifica_assegnato = true;
                $("#btn_AssegnaAutomaticamenteCarburante").show();
                $("#btn_salva").show();
            }
            break;
        case "2009":
            richiestaRinuncia = true;
            break;
        default:
    }

    //recupero la richiesta cod nel caso non ce l'abbia già
    if (richiesta_cod == 0) {
        richiesta_cod = CercaDettagliAzienda(KendoDDL("ddlAzienda").value(), 0);
        if (richiesta_cod.length > 0) {
            richiesta_cod = richiesta_cod[0].cod;
        } else
            richiesta_cod = 0;
    }


    var funzioneSubmitDaUsare = { funzione: InsertRichiestaTerzisti, flagInsert: !richiestaRinuncia, flagUpdate: true, flagDelete: !richiestaRinuncia };

    var funzioniCRUD = {
        funzioneRead: RichiesteTerzista,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: modifica_richiesto,
        UtenteAbilitatoCancellazione: modifica_richiesto,
        omettiPulsantiSalva: !richiestaRinuncia,
        omettiPulsantiAnnulla: richiestaRinuncia
    };

    var idModel = "UMA_Cod";
    var campiKendoModel = null;

    campiKendoModel = {
        UMA_Cod: { editable: false, type: "string", validation: { required: true } },
        Programmazione_Cod: { editable: false, type: "number", validation: { required: true } },
        Programmazione_Des: { editable: true, type: "string", validation: { required: true } },
        piva: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        Richiesta_Cod: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", defaultValue: -1, validation: { required: true } },
        CUAA: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        rag_soc: { editable: false, type: "string", validation: { required: true } },
        Macrouso_UMA_Cod: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        macrouso_UMA_Des: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        sup_tot: { editable: false, type: "number", validation: { required: true } },
        sup_tot_Orig: { editable: false, type: "number", validation: { required: true } },
        supA: { editable: false, type: "number", validation: { required: true } },
        supB: { editable: false, type: "number", validation: { required: true } },
        tessitura_Norm: { editable: false, type: "number", validation: { required: true } },
        tessitura_Media: { editable: false, type: "number", validation: { required: true } },
        tessitura_Tenace: { editable: false, type: "number", validation: { required: true } },
        supA_Edit: { editable: false, type: "number", validation: { required: true } },
        supB_Edit: { editable: false, type: "number", validation: { required: true } },
        tessitura_Norm_Edit: { editable: false, type: "number", validation: { required: true } },
        tessitura_Media_Edit: { editable: false, type: "number", validation: { required: true } },
        tessitura_Tenace_Edit: { editable: false, type: "number", validation: { required: true } },
        fabbisognoCalc: { editable: false, type: "number", validation: { required: true } },
        ltrichiesto: { editable: false, type: "number", validation: { required: true } },
        ltAssegnato: { editable: false, type: "number", validation: { required: true } },
        Regolamento_Cod: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: false }, defaultValue: 1 },
        Regolamento_Check: { editable: !richiestaRinuncia && modifica_richiesto, type: "boolean", validation: { required: false } },
    };

    var footerTemplateStringsupA = "#=calcTotaleColonna('" + "supA_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringsupB = "#=calcTotaleColonna('" + "supB_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringCalcolato = "#=calcTotaleColonna('" + "fabbisognoCalc" + "', " + IDControllo + ")#";
    var footerTemplateStringRichiesto = "#=calcTotaleColonna('" + "ltrichiesto" + "', " + IDControllo + ")#";
    var footerTemplateStringAssegnato = "#=calcTotaleColonna('" + "ltAssegnato" + "', " + IDControllo + ")#";
    var footerTemplateStringTotale = "#=calcTotaleColonna('" + "sup_tot" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrNorm = "#=calcTotaleColonna('" + "tessitura_Norm_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrMed = "#=calcTotaleColonna('" + "tessitura_Media_Edit" + "', " + IDControllo + ")#";
    var footerTemplateStringTerrTen = "#=calcTotaleColonna('" + "tessitura_Tenace_Edit" + "', " + IDControllo + ")#";

    var styleOut = "vertical-align: middle; text-align: center;";
    var colonneKendoGrid = [
        { field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA Azienda"), width: 150, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true, } },
        { field: "rag_soc", title: TraduzioneMultiResx(gestioneCarbResx, "rag_soc", "Azienda"), width: 200, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },  //, editor: azienda_DropDownEditor 
        {
            field: "Programmazione_Des",
            title: "Fascicolo",
            width: 170,
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true },
            editor: Programmazione_Des_DropDownEditor_Terz,
            attributes: { class: "edit_onInsert" }
        },
        { field: "macrouso_UMA_Des", title: TraduzioneMultiResx(gestioneCarbResx, "macrouso_UMA_Des", "Gruppo Colturale UMA"), width: 150, headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, editor: macrouso_DropDownEditor },
        
        { field: "fabbisognoCalc", title: TraduzioneMultiResx(gestioneCarbResx, "fabbisognoCalc", "Fabbisogno Calcolato (lt.)"), width: 120, format: "{0:n0}", editor: numberEditor4decimals, footerTemplate: footerTemplateStringCalcolato, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpinInteger },
        { field: "ltrichiesto", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto", "Richiesto (lt.)"), width: 120, format: "{0:n0}", editor: numberEditor4decimals, footerTemplate: footerTemplateStringRichiesto, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpinInteger },
        { field: "ltAssegnato", title: TraduzioneMultiResx(gestioneCarbResx, "ltAssegnato", "Assegnato (lt.)"), width: 120, format: "{0:n0}", editor: numberEditor4decimals, footerTemplate: footerTemplateStringAssegnato, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpinInteger },
        { field: "sup_tot", title: TraduzioneMultiResx(gestioneCarbResx, "sup_tot", "Superficie Totale (ha)"), format: "{0:n4}", width: 120, editor: numberEditor4decimals, footerTemplate: footerTemplateStringTotale, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpin4Decimals },
        { field: "supA_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "supA_Edit", "Superficie pendenza A (ha)"), format: "{0:n4}", width: 120, editor: numberEditor4decimals, footerTemplate: footerTemplateStringsupA, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpin4Decimals },
        { field: "supB_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "supB_Edit", "Superficie pendenza B (ha)"), format: "{0:n4}", width: 120, editor: numberEditor4decimals, footerTemplate: footerTemplateStringsupB, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpin4Decimals },
        { field: "tessitura_Norm_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "tessitura_Norm_Edit", "Zona Tessitura Normale (ha)"), width: 120, format: "{0:n4}", editor: numberEditor4decimals, footerTemplate: footerTemplateStringTerrNorm, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpin4Decimals },
        { field: "tessitura_Media_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "tessitura_Media_Edit", "Zona Tessitura Media (ha)"), width: 120, format: "{0:n4}", editor: numberEditor4decimals, footerTemplate: footerTemplateStringTerrMed, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpin4Decimals },
        { field: "tessitura_Tenace_Edit", title: TraduzioneMultiResx(gestioneCarbResx, "tessitura_Tenace_Edit", "Zona Tessitura Tenace (ha)"), width: 120, format: "{0:n4}", editor: numberEditor4decimals, footerTemplate: footerTemplateStringTerrTen, headerAttributes: { style: styleOut }, editor: NumberEditorNoSpin4Decimals },
    ]

    if (gestioneBiologico) {
        colonneKendoGrid.splice(4, 0, {
            field: "Regolamento_Check",
            //template: '<input type=\"checkbox\" # if(Regolamento_Cod > 1){ # checked #} # />',
            template: "#=(Regolamento_Cod === 4 ? 'Si' : 'No')#",
            title: "Biologico",
            width: 110,
            attributes: { class: "k-text-center checkboxCustom edit_onInsert editBio" },
            editor: booleanEditor,
        })
    }

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        headerAttributes: { style: styleOut },
        columnMenu: true,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 50] },
        //columnMenu: false
        // colonneCustomKendoGrid: colCustKendoGrid
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDetailInit: detailInitGrigliaDettagliLavorazioniTerzisti,
        /*funzioneDaChiamareDopoSave: HideTabDettagli, */
        funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpiantiTerz, 
        funzioneDaChiamareDopoDataBound: App_onDataBoundRichieste,
        funzioneDaChiamareDopoDelete: funzioneDaChiamareDopoDelete
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["UMA_Cod", "ltrichiesto", "Programmazione_Des", /*"Macrouso_UMA_Cod", "macrouso_UMA_Des" ,*/"CUAA"];

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

    //grid.unbind('cellClose');
    //grid.unbind('remove');

    grid.bind("cellClose", grid_cellCloseRichiesteTerz);
    grid.bind("remove", grid_remove);
    $("#" + IDControllo + " .k-grid-content").on("change", "input.k-checkbox", function (e) {
        //var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
        var grid = $("#" + IDControllo + "").data("kendoGrid");
        var row = this.closest("tr")
        var model = grid.dataItem(row)
        if (model.id === "" || model.ltrichiesto === 0) {
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

function booleanEditor(container, options) {
    var guid = kendo.guid();
    $('<input class="k-checkbox" id="' + guid + '" type="checkbox" name="' + options.field + '" data-type="boolean" data-bind="checked:' + options.field + '">').appendTo(container);
    $('<label class="k-checkbox-label" for="' + guid + '">&#8203;</label>').appendTo(container);
}

function onEditGrigliaDettagliImpiantiTerz(e) {
    if ($(e.container[0]).hasClass("editBio")) {
        if ((e.container[0].childNodes[0].checked && e.model.Regolamento_Cod !== 4) ||
            (!e.container[0].childNodes[0].checked && e.model.Regolamento_Cod !== 1))
            e.container[0].childNodes[0].checked = e.model.Regolamento_Cod === 4 ? true : false
    }
    if (e.model.UMA_Cod != "" &&
        e.model.Programmazione_Cod != 0 &&
        e.model.Richiesta_Cod != 0) {
        if ($(e.container[0]).hasClass("edit_onInsert") && !(e.model.id === "")) {
            if (!(($(e.container[0]).hasClass("editBio") && e.model.ltrichiesto === 0)))
                e.sender.closeCell();
        }
    } else {
        e.model.Richiesta_Cod = richiesta_cod;
    }
}

function grid_cellCloseRichiesteTerz(e) {
    //controlli sui campi appena modificati
    if (e.model.dirty === true) {

        var fieldName = e.container.find("input").attr("name");

        var gridID = e.sender.element[0].id;
        var grid = $("#" + gridID).data("kendoGrid");
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

            var row = this.select().closest("tr");
            //var indexColumnCUAA = grid.wrapper.find(".k-grid-header [data-field=" + "CUAA" + "]").index();

            var daCUAA = TrovaAziendaDaCUAA(e.model.CUAA);
            e.model.piva = daCUAA.Item2
            pivaInsertGrid = daCUAA.Item2
            e.model.rag_soc = daCUAA.Item1
            e.model.Programmazione_Cod = undefined
            e.model.Programmazione_Des = undefined
            e.model.Macrouso_UMA_Cod = undefined
            e.model.macrouso_UMA_Des = undefined
            e.model.tessitura_Norm = undefined
            e.model.tessitura_Media = undefined
            e.model.tessitura_Tenace = undefined
            e.model.tessitura_Norm_Edit = undefined
            e.model.tessitura_Media_Edit = undefined
            e.model.tessitura_Tenace_Edit = undefined
            e.model.supA = undefined
            e.model.supB = undefined
            e.model.supA_Edit = undefined
            e.model.supB_Edit = undefined
            e.model.sup_tot = undefined
            grid.refresh();

            nIscrizioneCameraDiCommercio[daCUAA.Item2] = LeggiNumeroIscrizioneCdC(e.model.piva);

        }
        //litri richiesti maggiore di zero
        if (e.model.dirtyFields.ltrichiesto == true) {
            if (e.model.ltrichiesto < 0 || e.model.ltrichiesto == undefined) e.model.ltrichiesto = 0;
            //coloraRighe_Richieste($("#" + gridID), e)
            grid.refresh();
        }
        //superfici maggiori di zero
        if (e.model.dirtyFields.tessitura_Norm_Edit == true) {
            if (e.model.tessitura_Norm_Edit < 0 || e.model.tessitura_Norm_Edit == undefined) e.model.tessitura_Norm_Edit = 0;
            if (e.model.tessitura_Media_Edit != 0 && e.model.tessitura_Tenace_Edit == 0) {
                e.model.tessitura_Media_Edit = e.model.sup_tot - e.model.tessitura_Norm_Edit;
            } else if (e.model.tessitura_Media_Edit == 0 && e.model.tessitura_Tenace_Edit != 0) {
                e.model.tessitura_Tenace_Edit = e.model.sup_tot - e.model.tessitura_Media_Edit;
            }
        }
        if (e.model.dirtyFields.tessitura_Media_Edit == true) {
            if (e.model.tessitura_Media_Edit < 0 || e.model.tessitura_Media_Edit == undefined) e.model.tessitura_Media_Edit = 0;
            if (e.model.tessitura_Norm_Edit != 0 && e.model.tessitura_Tenace_Edit == 0) {
                e.model.tessitura_Norm_Edit = e.model.sup_tot - e.model.tessitura_Media_Edit;
            } else if (e.model.tessitura_Norm_Edit == 0 && e.model.tessitura_Tenace_Edit != 0) {
                e.model.tessitura_Tenace_Edit = e.model.sup_tot - e.model.tessitura_Media_Edit;
            }
        }
        if (e.model.dirtyFields.tessitura_Tenace_Edit == true) {
            if (e.model.tessitura_Tenace_Edit < 0 || e.model.tessitura_Tenace_Edit == undefined) e.model.tessitura_Tenace_Edit = 0;
            if (e.model.tessitura_Norm_Edit != 0 && e.model.tessitura_Media_Edit == 0) {
                e.model.tessitura_Norm_Edit = e.model.sup_tot - e.model.tessitura_Tenace_Edit;
            } else if (e.model.tessitura_Norm_Edit == 0 && e.model.TerrenoMedio != 0) {
                e.model.tessitura_Media_Edit = e.model.sup_tot - e.model.tessitura_Tenace_Edit;
            }
        }
        if (e.model.dirtyFields.supA_Edit == true) {
            if (e.model.supA_Edit < 0 || e.model.supA_Edit == undefined) {
                e.model.supA_Edit = 0;
            }
            e.model.supB_Edit = e.model.sup_tot - e.model.supA_Edit;
        }
        if (e.model.dirtyFields.supB_Edit == true) {
            if (e.model.supB_Edit < 0 || e.model.supB_Edit == undefined) {
                e.model.supB_Edit = 0;
            }
            e.model.supA_Edit = e.model.sup_tot - e.model.supB_Edit;
        }
        //coloraRighe_Richieste($("#" + gridID), e)
        grid.refresh();
    }
}

function grid_remove(e) {
    console.log('remove', e);
}


function funzioneDaChiamareDopoDelete(e) {
    if ($(e.target).closest("[data-role='grid']")[0].id == "tab_griglia_terzisti") {
        var grid = $("#tab_griglia_terzisti").getKendoGrid();
        var row = $(e.target).closest("tr");
        dataItem = grid.dataItem(row);

        if (isUnicoRecordCUAA(dataItem.CUAA, grid) && !consentitoAggiungereNuoviCUAA &&
            (QS_TipoAzienda == Cooperativa_Agricola || QS_TipoAzienda == Azienda_Terzista) &&
            QS_Avanzamento == 1) {
            var messaggioCancellazione = "<b> Attenzione! Stai eliminando il seguente CUAA: " +
                dataItem.CUAA + " (" + dataItem.rag_soc + ") </b> <br>" +
                "Avendo superato la data limite inserimento CUAA " + Data_Limite_INS_Azienda + ", " +
                "se non inserisci altre righe per il medesimo CUAA prima del salvataggio " +
                "non sarà più possibile utilizzarlo una volta salvata la rendicontazione."
            kendo.confirm(messaggioCancellazione)
                .done(function () {
                    dataItem.cancellato = true;
                })
                .fail(function () {
                    dataItem.cancellato = false;
                    dataItem.deleted = false;
                    row.removeClass("deletedKendoRow")
                });
        } else {
            dataItem.cancellato = true;
        }
    }
}

function isUnicoRecordCUAA(CUAA, grid) {
    var occorrenzeCUAA = 0
    var items = grid.dataSource.data();
    items.forEach(function (row) {
        if (row.CUAA == CUAA && !row.deleted) {
            occorrenzeCUAA += 1
        }
    }, this);

    if (occorrenzeCUAA > 0) {
        return false
    } else {
        return true
    }
}

function App_onDataBoundRichieste(e) {
    if (QS_Avanzamento === 1) {
        LeggiNoProssimaRichiesta();
    }
    VerificaESegnalaAppezzamenti(e);
    coloraRighe_Richieste("#" + e.sender.element[0].id, e);
}

function coloraRighe_Richieste(grid_elem, e) {


    var grid = $(grid_elem).data('kendoGrid');
    //var items = e.sender.items();
    //var columns = e.sender.columns;
    var indexColumnCUAA = grid.wrapper.find(".k-grid-header [data-field=" + "CUAA" + "]").index();

    var indexColumnRagSoc = grid.wrapper.find(".k-grid-header [data-field=" + "rag_soc" + "]").index();
    var indexColumnLtrichiesto = grid.wrapper.find(".k-grid-header [data-field=" + "ltrichiesto" + "]").index();

    var indexColumnSupA = grid.wrapper.find(".k-grid-header [data-field=" + "supA_Edit" + "]").index();
    var indexColumnSupB = grid.wrapper.find(".k-grid-header [data-field=" + "supB_Edit" + "]").index();
    var indexColumnTessNorm = grid.wrapper.find(".k-grid-header [data-field=" + "tessitura_Norm_Edit" + "]").index();
    var indexColumnTessMed = grid.wrapper.find(".k-grid-header [data-field=" + "tessitura_Media_Edit" + "]").index();
    var indexColumnTessTen = grid.wrapper.find(".k-grid-header [data-field=" + "tessitura_Tenace_Edit" + "]").index();

    var indexColumnTotal = grid.wrapper.find(".k-grid-header [data-field=" + "sup_tot" + "]").index();
    var indexColumnFascicolo = grid.wrapper.find(".k-grid-header [data-field=" + "Programmazione_Des" + "]").index();
    var indexColumnMacrousoUMA = grid.wrapper.find(".k-grid-header [data-field=" + "macrouso_UMA_Des" + "]").index();

    var indexColumnBiologico = grid.wrapper.find(".k-grid-header [data-field=" + "Regolamento_Check" + "]").index();

    var rows = e.sender.tbody.children();

    //controlla la correttezza di ogni riga presente
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        CUAAFascicoloMacrousiUMADoppi(grid, dataItem, row, indexColumnCUAA, indexColumnFascicolo, indexColumnMacrousoUMA);
        //if (dataItem.Macrouso_UMA_Cod == "-1") {
        //    //AddErrorClass(row, indexColumnMacrousoUMA, errorCell, "Scegliere un gruppo colturale");
        //} else {
        //    RemoveErrorClass(row, indexColumnMacrousoUMA, errorCell);
        //}

        if (dataItem.Programmazione_Des != descrizioneFascicoloColtureNonImputabili) {
            //presenza CUAA / CUAA non corretto
            if (dataItem.CUAA != "" && dataItem.rag_soc == "") {
                AddErrorClass(row, indexColumnCUAA, errorCell, "CUAA non associato ad una azienda");
            } else {
                let ok = true
                //Azienda non iscritta alla camera di commercio
                let isPubblica = Trova_FormaGiuridica_Da_CUAA(dataItem.CUAA)
                if (nIscrizioneCameraDiCommercio[dataItem.piva] == 0 && !isPubblica) {
                    AddErrorClass(row, indexColumnCUAA, errorCell, "L'azienda non risulta iscritta alla camera di commercio");
                    ok = false
                }

                if (QS_TipoAzienda == Cooperativa_Agricola &&
                    dataItem.CUAA != "" && dataItem.rag_soc != "" &&
                    jQuery.inArray(dataItem.CUAA, listaCUAAPresentiInRichiesta) === -1 &&
                    QS_Avanzamento === 1 && parseInt($('#anno')[0].value) > 2022) {
                    //Le cooperative devono aver fatto richiesta per tutti i CUAA che vogliono inserire in rendicontazione
                    AddErrorClass(row, indexColumnCUAA, errorCell, "Non è possibile inserire la rendicontazione per questo CUAA in quanto non è presente una richiesta nello stesso anno");
                    ok = false
                }

                if (consentitoAggiungereNuoviCUAA == false) {
                    if (listaCUAARendicontati.length == 0) {
                        get_ListaCUAA_Richiesti(QS_TipoAzienda)
                    }


                    //Controllo che sia un CUAA presente in richiesta e che sia già stato rendicontato prima della data limite
                    if (dataItem.CUAA != "" && dataItem.rag_soc != "" &
                        jQuery.inArray(dataItem.CUAA, listaCUAARendicontati) === -1) {
                        if (Data_Limite_INS_Azienda !== "01/01/1900") {
                            AddErrorClass(row, indexColumnCUAA, errorCell, "Non è possibile utilizzare un CUAA che non sia stato precedentemente dichiarato in rendicontazione entro il giorno " + Data_Limite_INS_Azienda);
                            //AddErrorClass(row, indexColumnCUAA, errorCell, "Non è possibile utilizzare un CUAA che non sia stato precedentemente dichiarato in rendicontazione ");
                            ok = false
                        }
                    }
                }

                if (ok == true) {
                    RemoveErrorClass(row, indexColumnCUAA, errorCell);
                }
            }



            //if (dataItem.CUAA != "") {
            //    if (dataItem.rag_soc != "" && KendoDDL("ddlAzienda").text().toLowerCase().aContains(dataItem.rag_soc.toLowerCase())) {
            //        AddErrorClass(row, indexColumnRagSoc, errorCell, "L'azienda deve essere diversa da quella attuale");
            //    } else {
            //        RemoveErrorClass(row, indexColumnRagSoc, errorCell);
            //    }
            //}

            if (dataItem.Programmazione_Cod == 0 && dataItem.Programmazione_Des != "") {
                AddErrorClass(row, indexColumnFascicolo, errorCell, "Scegliere un fascicolo");
            } else {
                RemoveErrorClass(row, indexColumnFascicolo, errorCell);
            }

            //Litri richiesti superano il totale richiesto con la richiesta iniziale 
            if ((parseFloat($("#benzinaTerzisti")[0].value) > 0 || parseFloat($("#gasolioTerzisti")[0].value) > 0 || parseFloat($("#gasolioSerraTerzisti")[0].value) > 0) &&
                calcTotaleColonna('ltrichiesto', tab_griglia_terzisti, false) > (parseFloat($("#benzinaTerzisti")[0].value) + parseFloat($("#gasolioTerzisti")[0].value) +
                    parseFloat($("#gasolioSerraTerzisti")[0].value) + 1)) {

                ///AddErrorClass(row, indexColumnLtrichiesto, errorCell, "I litri richiesti superano il totale richiesto inzialmente");
            } else {
                if (dataItem.ltrichiesto < 0) {
                    AddErrorClass(row, indexColumnLtrichiesto, errorCell, "I litri richiesti sono negativi");
                } else {
                    RemoveErrorClass(row, indexColumnLtrichiesto, errorCell);
                }
            }

            //Controllo superfici inferiori o uguali a quelle massime
            if (dataItem.supA < dataItem.supA_Edit)
                AddErrorClass(row, indexColumnSupA, warningCell, "La superficie indicata è superiore al massimo indicato sul fascicolo");
            else
                RemoveErrorClass(row, indexColumnSupA, warningCell);

            if (dataItem.supB < dataItem.supB_Edit)
                AddErrorClass(row, indexColumnSupB, warningCell, "La superficie indicata è superiore al massimo indicato sul fascicolo");
            else
                RemoveErrorClass(row, indexColumnSupB, warningCell);

            if (dataItem.tessitura_Norm < dataItem.tessitura_Norm_Edit)
                AddErrorClass(row, indexColumnTessNorm, warningCell, "La superficie indicata è superiore al massimo indicato sul fascicolo");
            else
                RemoveErrorClass(row, indexColumnTessNorm, warningCell);

            if (dataItem.tessitura_Media < dataItem.tessitura_Media_Edit)
                AddErrorClass(row, indexColumnTessMed, warningCell, "La superficie indicata è superiore al massimo indicato sul fascicolo");
            else
                RemoveErrorClass(row, indexColumnTessMed, warningCell);

            if (dataItem.tessitura_Tenace < dataItem.tessitura_Tenace_Edit)
                AddErrorClass(row, indexColumnTessTen, warningCell, "La superficie indicata è superiore al massimo indicato sul fascicolo");
            else
                RemoveErrorClass(row, indexColumnTessTen, warningCell);


            var rowAnomalia = null
            if (anomalieSuperficiAppezzamenti != null) {
                rowAnomalia = anomalieSuperficiAppezzamenti.find((x) => x.Gruppo_Colturale_UMA_Cod == dataItem.Macrouso_UMA_Cod);
            }

            if (rowAnomalia != null && (dataItem.supA != rowAnomalia.Zona_Pendenza_A_Rilevata || dataItem.supB != rowAnomalia.Zona_Pendenza_B_Rilevata || dataItem.sup_tot != rowAnomalia.Totale_Superficie_Rilevata)) {
                if (dataItem.supA != rowAnomalia.Zona_Pendenza_A_Rilevata)
                    AddErrorClass(row, indexColumnSupA, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
                if (dataItem.supB != rowAnomalia.Zona_Pendenza_B_Rilevata)
                    AddErrorClass(row, indexColumnSupB, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
                if (dataItem.sup_tot != rowAnomalia.Totale_Superficie_Rilevata)
                    AddErrorClass(row, indexColumnTotal, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
            }
            //somma superfici pendenza non congruente alla superficie totale
            else if (((dataItem.supA_Edit + dataItem.supB_Edit).toFixed(4)) != parseFloat(dataItem.sup_tot).toFixed(4)) {

                AddErrorClass(row, indexColumnSupA, errorCell, "La superficie totale è diversa dalla somma delle zone in pendenza");
                AddErrorClass(row, indexColumnSupB, errorCell, "La superficie totale è diversa dalla somma delle zone in pendenza");
                AddErrorClass(row, indexColumnTotal, errorCell, "La superficie totale è diversa dalla somma delle zone in pendenza");

            } else {
                RemoveErrorClass(row, indexColumnSupA, errorCell);
                RemoveErrorClass(row, indexColumnSupB, errorCell);
                RemoveErrorClass(row, indexColumnTotal, errorCell);
            }

            if (rowAnomalia != null && (dataItem.tessitura_Norm != rowAnomalia.Zona_Tessitura_Normale_Rilevata || dataItem.tessitura_Media != rowAnomalia.Zona_Tessitura_Media_Rilevata || dataItem.tessitura_Tenace != rowAnomalia.Zona_Tessitura_Tenace_Rilevata || dataItem.sup_tot != rowAnomalia.Totale_Superficie_Rilevata)) {
                if (dataItem.tessitura_Norm != rowAnomalia.Zona_Tessitura_Normale_Rilevata)
                    AddErrorClass(row, indexColumnTessNorm, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
                if (dataItem.tessitura_Media != rowAnomalia.Zona_Tessitura_Media_Rilevata)
                    AddErrorClass(row, indexColumnTessMed, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
                if (dataItem.tessitura_Tenace != rowAnomalia.Zona_Tessitura_Tenace_Rilevata)
                    AddErrorClass(row, indexColumnTessTen, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
                if (dataItem.sup_tot != rowAnomalia.Totale_Superficie_Rilevata)
                    AddErrorClass(row, indexColumnTotal, errorCell, "Superficie dichiarata diversa da superficie totale effettiva.");
            }
            //somma superfici tessitura non congruente alla superficie totale
            else if (((dataItem.tessitura_Norm_Edit + dataItem.tessitura_Media_Edit + dataItem.tessitura_Tenace_Edit).toFixed(4)) != parseFloat(dataItem.sup_tot).toFixed(4)) {

                AddErrorClass(row, indexColumnTessMed, errorCell, "La superficie totale è diversa dalla somma delle zone di tessitura");
                AddErrorClass(row, indexColumnTessNorm, errorCell, "La superficie totale è diversa dalla somma delle zone di tessitura");
                AddErrorClass(row, indexColumnTessTen, errorCell, "La superficie totale è diversa dalla somma delle zone di tessitura");
                AddErrorClass(row, indexColumnTotal, errorCell, "La superficie totale è diversa dalla somma delle zone di tessitura");

            } else {
                RemoveErrorClass(row, indexColumnTessMed, errorCell);
                RemoveErrorClass(row, indexColumnTessNorm, errorCell);
                RemoveErrorClass(row, indexColumnTessTen, errorCell);
                RemoveErrorClass(row, indexColumnTotal, errorCell);
            }
        }

    }
}

function CUAAFascicoloMacrousiUMADoppi(grid, dataItem, row, indexColumnCUAA, indexColumnFascicolo, indexColumnMacrousoUMA) {
    let data = grid.dataSource.data();

    let datiFiltrati = data.filter(el => {
        return el.Programmazione_Cod == dataItem.Programmazione_Cod &&
            el.Macrouso_UMA_Cod == dataItem.Macrouso_UMA_Cod &&
            el.CUAA == dataItem.CUAA;
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
            return el.Programmazione_Cod == dataItem.Programmazione_Cod &&
                el.Macrouso_UMA_Cod == macrouso2023 &&
                el.CUAA == dataItem.CUAA;
        });
        datiFiltrati.length += datiFiltrati2.length;
    }

    //--------------------------------------------------------------------------------


    if (datiFiltrati.length > 1) {
        AddErrorClass(row, indexColumnCUAA, errorCell, CUAA_Fasc_GC);
        AddErrorClass(row, indexColumnFascicolo, errorCell, CUAA_Fasc_GC);
        AddErrorClass(row, indexColumnMacrousoUMA, errorCell, CUAA_Fasc_GC);
    }
}

function Programmazione_Des_DropDownEditor_Terz(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0] != undefined ?
        $(container).parents("div[data-role='grid']")[0].id : "tab_griglia_terzisti";

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (row != undefined) {
        if (row.UMA_Cod != "" &&
            row.Programmazione_Cod != -1 &&
            row.Richiesta_Cod != 0) {
            return;
        }

        if (row.id !== "")
            return;

        WaitFrame.show();

        PopolaElencoProgrammazione_Des_Terz(row.piva).then(
            elencoProgrammazione_Des => {
                WaitFrame.hide();
                creaDropDownEditor(container, "Programmazione_Des", "Programmazione_Cod", elencoProgrammazione_Des, changeProgrammazione_Des_Terz);
            }
        )
    }
}

function PopolaElencoProgrammazione_Des_Terz(piva) {
    return new Promise(function (resolve, reject) {
        var param = {
            piva: piva,
            anno: parseInt($('#anno')[0].value)
        }
        ajaxAgronica("./richiestaCarburanti.aspx/Leggi_Fascicoli",
            JSON.stringify(param),
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                let emptyEl = {
                    Programmazione_Des: 'SELEZIONA',
                    Programmazione_Cod: 0
                };
                let baseEl = {
                    Programmazione_Des: descrizioneFascicoloColtureNonImputabili,
                    Programmazione_Cod: codiceFascicoloColtureNonImputabili
                };
                let anticipi = {
                    Programmazione_Des: descrizioneFascicoloAnticipi,
                    Programmazione_Cod: codiceFascicoloAnticipi
                };
                let trasferimenti = {
                    Programmazione_Des: descrizioneFascicoloTrasferimenti,
                    Programmazione_Cod: codiceFascicoloTrasferimenti
                };   
                let pianoColt = {
                    Programmazione_Des: descrizioneFascicoloPianoColturale,
                    Programmazione_Cod: codiceFascicoloPianoColturale
                };    
                let newArr = new Array();
                newArr.push(emptyEl);
                newArr.push(baseEl);
                newArr.push(anticipi);
                if (parseInt($('#anno')[0].value) >= 2025) {
                    newArr.push(pianoColt);
                }
                if (QS_Avanzamento == 1) {
                    newArr.push(trasferimenti);
                }                
                for (let i = 0; i < arr.length; i++) {
                    let prg_cod = arr[i].strProgrammazioniCod.trim().replaceAll("|", "");
                    let prg_des = arr[i].strProgrammazioniDes.trim();
                    newArr.push({
                        Programmazione_Des: prg_des,
                        Programmazione_Cod: parseInt(prg_cod)
                    });
                }
                resolve(newArr);
            }, null);
    });
}

function changeProgrammazione_Des_Terz(e) {

    var dataItem = e.sender.dataItem();
    //var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#tab_griglia_terzisti").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Programmazione_Cod = dataItem.Programmazione_Cod;
    model.Programmazione_Des = dataItem.Programmazione_Des;
    model.Macrouso_UMA_Cod = "-1";
    model.macrouso_UMA_Des = "Seleziona";
    program_cod = dataItem.Programmazione_Cod;
    model.dirty = true;
}


function azienda_DropDownEditor(container, options) {
    let parentRow = $($(container).parents(".k-detail-row")[0]).prev();
    let parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");

    PopolaElencoAziende();

    creaDropDownEditor(container, "rag_soc", "piva", elencoAziende, changeAzienda);
}

function PopolaElencoAziende() {
    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "solo_aziende_attive": 0 });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtenteCodiceSocio",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoAziende = risp;
        }, null);
    return elencoAziende;
}

function changeAzienda(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_terzisti").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.rag_soc = dataItem.rag_soc;
    model.piva = dataItem.piva;
    pivaInsertGrid = model.piva;
    //grid.refresh();
    //kendoFastRedrawRow(grid, row);
}

function macrouso_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#tab_griglia_terzisti").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (row != undefined) {

        if (row.Programmazione_Cod == "" || row.Programmazione_Cod == undefined) {
            return null;
        }

        if (colt_elem_orig[row.Macrouso_UMA_Cod] == undefined && row.Macrouso_UMA_Cod != "" &&
            row.Programmazione_Cod != 0 && row.Richiesta_Cod != 0) {
            pivaSelezionata = row.piva;
            gruppo_col = row.Macrouso_UMA_Cod;
            prog_cod = row.Programmazione_Cod;
            if (row.Richiesta_Cod > 0)
                richiesta_cod = row.Richiesta_Cod;
            let con;
            RicercaLavorazioniTerzisti(con, ["x"], true);
            colt_elem_orig[row.Macrouso_UMA_Cod] = colt_elem;
            if (colt_elem_added[row.Macrouso_UMA_Cod] == undefined)
                colt_elem_added[row.Macrouso_UMA_Cod] = 0;
        }

        if (row.Macrouso_UMA_Cod != "" && colt_elem_orig[row.Macrouso_UMA_Cod] + colt_elem_added[row.Macrouso_UMA_Cod] != 0) {
            colt_elem_added[row.Macrouso_UMA_Cod] = 0;
            return row.macrouso_UMA_Des;
        }

        WaitFrame.show();

        PopolaElencoMacrousi(row.piva, row.Programmazione_Cod).then(
            macroSuperfici => {
                WaitFrame.hide();
                creaDropDownEditor(container, "Macrouso_UMA_Des", "Macrouso_UMA_Cod", macroSuperfici, changeMacrousi);
            }
        );
    }
}

function PopolaElencoMacrousi(piva, programmazione_Cod) {
    return new Promise(function (resolve, reject) {
        if (piva != "") {
            if (letturaMacrosuperfici[piva + "-" + programmazione_Cod.toString()] == undefined) {
                WaitFrame.show();
                
                if (programmazione_Cod != codiceFascicoloColtureNonImputabili.toString() &&
                    programmazione_Cod != codiceFascicoloAnticipi.toString() &&
                    programmazione_Cod != codiceFascicoloTrasferimenti.toString()) {
                    let parametri = {
                        piva: piva,
                        Programmazione_Cod: programmazione_Cod,
                        anno: parseInt($('#anno')[0].value),
                        richiesta_Cod: richiesta_cod,
                        avanzamento: QS_Avanzamento,
                        terzista: QS_Type
                    }

                    ajaxAgronica(indirizzohttp + "/Trova_Macrousi_Superfici_Fascicolo",
                        JSON.stringify(parametri),
                        function (risposta) {
                            risp = JSON.parse(risposta.RispostaStringa);
                            let objVuoto = {
                                Macrouso_UMA_Des: "Seleziona",
                                Macrouso_UMA_Cod: "-1"
                            };
                            risp.unshift(objVuoto);
                            macroSuperfici = risp;
                            WaitFrame.hide();
                            letturaMacrosuperfici[piva + "-" + programmazione_Cod.toString()] = macroSuperfici;
                            resolve(macroSuperfici);
                        }, null);
                } else {
                    let annoSelezionato = getAnnoSelezionato();
                    let parametri = {
                        Programmazione_Cod: programmazione_Cod,
                        Anno: annoSelezionato,
                        CUAA: $('#CUAA')[0].value,
                        Piva: piva,
                        Avanzamento: QS_Avanzamento,
                        Terzista: QS_Type
                    };
                    ajaxAgronicaSync(indirizzohttp + "/LeggiUMA_Macrousi",
                        JSON.stringify(parametri),
                        false,
                        function (risposta) {
                            let arrResp = JSON.parse(risposta.RispostaStringa);
                            arrResp.unshift({
                                Macrouso_UMA_Cod: 0,
                                Macrouso_UMA_Des: 'SELEZIONA'
                            });
                            macroSuperfici = arrResp;
                            letturaMacrosuperfici[piva + "-" + programmazione_Cod.toString()] = macroSuperfici;
                            resolve(macroSuperfici);
                        }, null);
                }
            } else
                resolve(letturaMacrosuperfici[piva + "-" + programmazione_Cod.toString()])
        }
    });
}

function changeMacrousi(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_terzisti").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.cambio = model.Macrouso_UMA_Cod;
    //model.Programmazione_Cod = isNaN(dataItem.Programmazione_Cod) ? -1 : dataItem.Programmazione_Cod;
    model.macrouso_UMA_Des = dataItem.Macrouso_UMA_Des;
    model.Macrouso_UMA_Cod = dataItem.Macrouso_UMA_Cod;
    model.supA = isNaN(dataItem.sup_A) ? 0 : dataItem.sup_A;
    model.supB = isNaN(dataItem.sup_B) ? 0 : dataItem.sup_B;
    model.tessitura_Norm = isNaN(dataItem.tessitura_Norm) ? 0 : dataItem.tessitura_Norm;
    model.tessitura_Media = isNaN(dataItem.tessitura_Media) ? 0 : dataItem.tessitura_Media;
    model.tessitura_Tenace = isNaN(dataItem.tessitura_Tenace) ? 0 : dataItem.tessitura_Tenace;

    model.supA_Edit = isNaN(dataItem.sup_A) ? 0 : dataItem.sup_A;
    model.supB_Edit = isNaN(dataItem.sup_B) ? 0 : dataItem.sup_B;
    model.sup_tot = ((isNaN(dataItem.sup_tot) ? 0 : dataItem.sup_tot) != model.supA_Edit + model.supB_Edit) && (model.supA_Edit + model.supB_Edit > 0)
        ? model.supA_Edit + model.supB_Edit : isNaN(dataItem.sup_tot) ? 0 : dataItem.sup_tot;
    model.tessitura_Norm_Edit = isNaN(dataItem.tessitura_Norm) ? 0 : dataItem.tessitura_Norm;
    model.tessitura_Media_Edit = isNaN(dataItem.tessitura_Media) ? 0 : dataItem.tessitura_Media;
    model.tessitura_Tenace_Edit = isNaN(dataItem.tessitura_Tenace) ? 0 : dataItem.tessitura_Tenace;
    model.dirty = true;
    grid.refresh();
    //kendoFastRedrawRow(grid, row);
}

//function ddlMacrousiTerz_Load() {

//    kendoAziende = $('#ddlMacrousoTerz').kendoDropDownList({
//        filter: "contains",
//        dataSource: { transport: { read: RiempiDdlMacrousi } },
//        dataTextField: "macrouso_UMA_Des",
//        dataValueField: "Macrouso_UMA_Cod",
//        optionLabel: { "macrouso_UMA_Des": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + "...", "macrouso UMA": "" },
//        autoWidth: true,
//        dataBound: ddlAzienda_OnDataBound
//    }).data("kendoDropDownList");
//}


async function detailInitGrigliaDettagliLavorazioniTerzisti(e) {
    var id = ""
    if (e.data.UMA_Cod != undefined && e.data.UMA_Cod != "") {
        id = e.data.UMA_Cod;
    }
    if (e.data.Macrouso_UMA_Cod != undefined && e.data.Macrouso_UMA_Cod != "") {
        id = e.data.Macrouso_UMA_Cod;
    }
    if (e.data.Programmazione_Cod != undefined && e.data.Programmazione_Cod > 0) {
        id = e.data.Programmazione_Cod + "_" + id;
    }

    if (e.data.CUAA != undefined && e.data.CUAA != "") {
        const cuaa = e.data.CUAA + "";
        id = cuaa.trim() + "_" + id;
    }

    if (id != "" && e.data.Macrouso_UMA_Cod != "0000") {
        var id_div = "GrigliaDettagliLavorazioni_" + id;
        $("<div id='" + id_div + "' />").appendTo(e.detailCell);
        ElencoTabAperte.push(id);
        richiesta_cod = e.data.Richiesta_Cod;
        pivaSelezionata = e.data.piva;
        prog_cod = e.data.Programmazione_Cod;
        await LeggiLavorazioniAlternative(e.data.Macrouso_UMA_Cod);
        if (colt_elem_orig[e.data.Macrouso_UMA_Cod] == undefined) {
            pivaSelezionata = e.data.piva;
            gruppo_col = e.data.Macrouso_UMA_Cod;
            prog_cod = e.data.Programmazione_Cod;
            richiesta_cod = e.data.Richiesta_Cod;
            let con;
            RicercaLavorazioniTerzisti(con, ["x"], true);
            colt_elem_orig[e.data.Macrouso_UMA_Cod] = colt_elem;
        }
        colt_elem_added[e.data.Macrouso_UMA_Cod] = 0;
        if (QS_Type == -1 /*&& QS_Avanzamento == 1*/) {
            if (lav_incrociati[e.data.piva] == undefined) {
                lav_incrociati[e.data.piva] = {};
            }
            if (lav_incrociati[e.data.piva][e.data.Macrouso_UMA_Cod] == undefined) {
                lav_incrociati[e.data.piva][e.data.Macrouso_UMA_Cod] = await controlloIncrociato(pivaSelezionata, prog_cod, e.data.Macrouso_UMA_Cod, $('#anno')[0].value);
            }
        }
        popolaGrigliaDettagliLavorazioniTerzisti(id_div, e.data.Macrouso_UMA_Cod);
    }
}

function popolaGrigliaDettagliLavorazioniTerzisti(IDControllo, id_gruppo) {

    gruppo_col = id_gruppo;
    let mesiVisibili = false;

    var omettiAnnulla = false;
    var omettiSalva = true;

    //Coltivazioni sotto serra
    if (id_gruppo == 1034) {
        mesiVisibili = true;
    }
    var funzioniCRUD = {
        funzioneRead: RicercaLavorazioniTerzisti,
        funzioneSubmit: {
            funzione: InsertLavorazioneTerzisti,
            flagInsert: !richiestaRinuncia,
            flagUpdate: true,
            flagDelete: !richiestaRinuncia
        },
        UtenteAbilitatoInserimentoModifica: modifica_richiesto,
        UtenteAbilitatoCancellazione: modifica_richiesto,
        omettiPulsantiSalva: omettiSalva,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var styleInt = "background-color: deepskyblue; vertical-align: top";
    var idModel = "richiestaDettaglioCod";
    var campiKendoModel = {
        richiestaDettaglioCod: { editable: false, type: "number" },
        Richiesta_Cod: { editable: false, type: "number" },
        Programmazione_Cod: { editable: false, type: "number" },
        Piva: { editable: false, type: "string" },
        Macrouso_UMA_Cod: { editable: false, type: "string" },
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
        Sup_A: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Sup_B: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoNormale: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoMedio: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoTenace: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Attivita_Cod: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Attivita_Des: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        Note_Compilatore: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        Note_Approvatore: { editable: !richiestaRinuncia && modifica_assegnato, type: "string" },
        Udm_Alt: { editable: false, type: "string" },
        Qta_Manuale: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Mesi: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Validita_Inizio: { editable: !richiestaRinuncia && modifica_richiesto, type: "date" }
    }

    var footerTemplateStringRichiesto = "#=calcTotaleColonna('" + "ltrichiesto" + "', " + IDControllo + ")#";
    var footerTemplateStringAssegnato = "#=calcTotaleColonna('" + "ltAssegnato" + "', " + IDControllo + ")#";
    var colonneKendoGrid = [
        { field: "LavUMA", width: "250px", title: TraduzioneMultiResx(gestioneCarbResx, "LavUMA", "Lavorazione U.M.A."), headerAttributes: { style: styleInt }, filterable: { multi: true, search: true }, editor: lavUMA_DropDownEditor_Terzisti },
        //{ field: "LavGIAS", width: "200px", title: TraduzioneMultiResx(gestioneCarbResx, "LavGIAS", "Lavorazione GIAS"), headerAttributes: { style: styleInt }, editor: lavGIAS_DropDownEditor_Terzisti },
        { field: "Attivita_Des", width: "200px", title: "Attività", headerAttributes: { style: styleInt }, editor: AttivitaGIAS_DropDownEditor, hidden: true, },
        { field: "TipoCarb", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "TipoCarb", "Tipo Carburante"), headerAttributes: { style: styleInt }, filterable: { multi: true, search: true }, editor: Carburanti_DropDownEditor_Terzisti },
        { field: "Qta_Manuale", width: "100px", title: "Qta. Manuale", headerAttributes: { style: styleInt }, attributes: { class: "Lav_Alt" }, editor: NumberEditorNoSpin4Decimals },
        { field: "Udm_Alt", width: "100px", title: "UdM", headerAttributes: { style: styleInt }, filterable: { multi: true, search: true } }
    ]

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
        editor: MaggiorazioneTerzisti_DropDownEditor,
        template: '#= (SupMaggiorazioneTrasferimenti == 1) ? "Sì" : "No" #'
    });
    colonneKendoGrid.push({ field: "fabbisognoCalc", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "fabbisognoCalc", "Fabbisogno Calcolato (lt.)"), headerAttributes: { style: styleInt }, format: "{0:n0}", editor: NumberEditorNoSpinInteger })
    colonneKendoGrid.push({ field: "ltrichiesto", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto", "Richiesto (lt.)"), footerTemplate: footerTemplateStringRichiesto, headerAttributes: { style: styleInt }, format: "{0:n0}", editor: NumberEditorNoSpinInteger })
    colonneKendoGrid.push({ field: "ltAssegnato", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "ltAssegnato", "Assegnato (lt.)"), footerTemplate: footerTemplateStringAssegnato, headerAttributes: { style: styleInt }, format: "{0:n0}", editor: NumberEditorNoSpinInteger })
    //colonneKendoGrid.push({ field: "nLavPreviste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "nLavPreviste", "Numero Lavorazioni Previste"), headerAttributes: { style: styleInt }, editor: NumberEditorNoSpinInteger })
    //colonneKendoGrid.push({ field: "nLavRichieste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "nLavRichieste", "Numero Lavorazioni Richieste"), headerAttributes: { style: styleInt }, editor: NumberEditorNoSpinInteger })
    //colonneKendoGrid.push({ field: "piuLavPreviste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "piuLavPreviste", "Piu Lavorazioni Previste"), headerAttributes: { style: styleInt }, editor: NumberEditorNoSpinInteger })

    if (!mesiVisibili) {
        //colonneKendoGrid.push({ field: "piuRaccoltiPrevisti", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "piuRaccoltiPrevisti", "Piu Raccolti Previsti"), headerAttributes: { style: styleInt }, editor: NumberEditorNoSpinInteger })
        colonneKendoGrid.push({ field: "Superficie_Trattata", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "Superficie_Trattata", "Superficie Trattata"), headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
        colonneKendoGrid.push({ field: "Sup_A", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "Sup_A", "Zona Pendenza A (ha)"), headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
        colonneKendoGrid.push({ field: "Sup_B", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "Sup_B", "Zona Pendenza B (ha)"), headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
        colonneKendoGrid.push({ field: "TerrenoNormale", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoNormale", "Zona Tessitura Normale (ha)"), headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
        colonneKendoGrid.push({ field: "TerrenoMedio", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoMedio", "Zona Tessitura Media (ha)"), headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
        colonneKendoGrid.push({ field: "TerrenoTenace", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoTenace", "Zona Tessitura Tenace (ha)"), headerAttributes: { style: styleInt }, format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
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

    colonneKendoGrid.push({ field: "Note_Compilatore", width: "100px", title: "Note Compilatore", headerAttributes: { style: styleInt }, filterable: false })
    colonneKendoGrid.push({ field: "Note_Approvatore", width: "100px", title: "Note Approvatore", headerAttributes: { style: styleInt }, filterable: false })

    var colonneCustomKendoGrid = new Array();
    if (!richiestaRinuncia && (modifica_richiesto)) {
        colonneCustomKendoGrid.push({
            command: {
                template: "<div class='btn btn-info btnInfo btnDettaglio' style='width:25px;border:0px;' onclick=LavorazioniMultipleTerzisti(this.closest('tr'),this.closest('.k-grid'))><span class='fa fa-plus lampeggiante'></span></div>"
            },
            title: "Inserisci Multiple", width: "68px", headerAttributes: { style: styleInt }
        });
    }

    var parametriKendoGrid = {
        excel: true, pdf: false,
        groupable: false,
        headerAttributes: { style: styleInt },
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
        //funzioneDaChiamareDopoSave: InsertLavorazioneTerzisti,
        //funzioneDaChiamareDopoEdit: InsertLavorazioneTerzisti,
        funzioneDaChiamareDopoDataBound: App_onDataBoundLavorazioniTerzisti,
        funzioneDaChiamareDopoEdit: onEditGrigliaDettagliLavorazioniTerzisti,
        funzioneDaChiamareDopoDelete: funzioneDaChiamareDopoDelete_Lavorazioni
    };
    var mostraRigheCancellate = true;
    //TODO perchè le dropDown sono comunque modificabili anche se non in inserimento?
    var colonneDisabilitateSoloInModifica = ["LavUMA", "LavGIAS", "TipoCarb", "SupMaggiorazioneTrasferimenti", "fabbisognoCalc"];

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
    var indx = IDControllo.split("_")[1];
    gasolioTotTerzLAV[indx] = 0;
    grid._data.filter((x) => { return x.TipoCarb == "Gasolio" }).forEach((x) => { gasolioTotTerzLAV[indx] += x.ltrichiesto });
    gasolioTotTerzLAV[indx].toFixed(4);

    grid.bind("cellClose", grid_cellCloseTerz);

    $("#" + IDControllo).on("mousedown", ".k-grid-cancel-changes", function (e) {
        colt_elem_added[id_gruppo] = 0;
    });
}

function grid_cellCloseTerz(e) {
    let richiesto = e.model.ltrichiesto;
    if (e.model.dirtyFields != undefined) {
        var gridID = e.sender.element[0].id;
        var grid = $("#" + gridID).data("kendoGrid");
        parentRow = e.sender.element.parents(".k-detail-row").prev();
        parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
        parentItem = parentGrid.dataItem(parentRow);
        var parentRowItem = parentGrid.dataItem(parentRow);

        //controllo superficie trattata maggiore di zero e calcolo fabbisogno carburante 
        if (e.model.dirtyFields.Superficie_Trattata == true) {
            if (e.model.Programmazione_Cod > 0 && (e.model.Superficie_Trattata < 0 || e.model.Superficie_Trattata == undefined || e.model.Superficie_Trattata > parentRowItem.sup_tot)) e.model.Superficie_Trattata = parentRowItem.sup_tot;
            aggiornataSup_TotaleLavorazioni_Edit_Terzisti(e, parentItem);
            calcoloFabbisogno_Terzisti(parentRowItem, e.model, e.sender);
            e.model.dirtyFields.Superficie_Trattata = false;
            grid.refresh();
        } else
            //controllo lavRichieste maggiore di zero
            if (e.model.dirtyFields.nLavRichieste == true) {
                if (e.model.nLavRichieste < 0 || e.model.nLavRichieste == undefined) e.model.nLavRichieste = 0;
                calcoloFabbisogno_Terzisti(parentRowItem, e.model, e.sender);
                e.model.dirtyFields.nLavRichieste = false;
            }
        //Controllo superfici pendenza maggiore di zero e calcolo fabbisogno carburante
        if (e.model.dirtyFields.Sup_A == true || e.model.dirtyFields.Sup_B == true) {
            if (e.model.dirtyFields.Sup_A == true) {
                if (e.model.Sup_A < 0 || e.model.Sup_A == undefined) e.model.Sup_A = 0;
                if (e.model.Superficie_Trattata - e.model.Sup_A < parentItem.supB) e.model.Sup_B = e.model.Superficie_Trattata - e.model.Sup_A;
            } else if (e.model.dirtyFields.Sup_B == true) {
                if (e.model.Sup_B < 0 || e.model.Sup_B == undefined) e.model.Sup_B = 0;
                if (e.model.Superficie_Trattata - e.model.Sup_B < parentItem.supA) e.model.Sup_A = e.model.Superficie_Trattata - e.model.Sup_B;
            }
            e.model.dirtyFields.Sup_A = false;
            e.model.dirtyFields.Sup_B = false;
        }
        //controllo litri richiesti maggiore di zero e correttezza quantità
        if (e.model.dirtyFields.ltrichiesto == true) {
            if (e.model.ltrichiesto < 0 || e.model.ltrichiesto == undefined) e.model.ltrichiesto = 0;
            parentRowItem.ltrichiesto = calcTotaleColonna('ltrichiesto', $("#" + gridID)[0], false);
            grid.refresh();
        } else if (e.model.dirtyFields.ltAssegnato == true) {
            if (e.model.ltAssegnato < 0 || e.model.ltAssegnato == undefined) e.model.ltAssegnato = 0;
            let RichiestoDecurtato = e.model.ltrichiesto - (e.model.ltrichiesto * Percentuale_Decurtamento / 100);
            if (e.model.ltAssegnato > RichiestoDecurtato) e.model.ltAssegnato = RichiestoDecurtato
            grid.refresh();

            if (permesso_approvazione_richiesta == true && $("#stato_pratica_cod").val() == "2002" && e.model.dirtyFields.ltAssegnato == true && e.model.ltAssegnato != tempAssegnato) {
                let diff = Math.round(tempAssegnato - e.model.ltAssegnato, 2);
                switch (e.model.Car_Cod) {
                    case Gasolio.toString():
                        if (gasolioTerzAppro.value() > diff)
                            gasolioTerzAppro.value(gasolioTerzAppro.value() - diff);
                        break;

                    case Benzina.toString():
                        if (benzinaTerzAppro.value() > diff)
                            benzinaTerzAppro.value(benzinaTerzAppro.value() - diff);
                        break;

                    case Gasolio_Serra.toString():
                        if (gasolioSerraTerzAppro.value() > diff)
                            gasolioSerraTerzAppro.value(gasolioSerraTerzAppro.value() - diff);
                        break;

                    default:
                }
            }
        }
        //Controllo superfici tessitura maggiore di zero e calcolo fabbisogno carburante

        if (e.model.dirtyFields.TerrenoNormale == true || e.model.dirtyFields.TerrenoMedio == true || e.model.dirtyFields.TerrenoTenace == true) {
            if (e.model.TerrenoMedio != undefined && e.model.TerrenoNormale != undefined && e.model.TerrenoTenace != undefined) {
                //e.model.Superficie_Trattata = e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace
            }
            if (e.model.dirtyFields.TerrenoNormale == true) {
                if (e.model.TerrenoNormale < 0 || e.model.TerrenoNormale == undefined) e.model.TerrenoNormale = 0;
                if (e.model.TerrenoMedio != 0 && e.model.TerrenoTenace == 0) {
                    e.model.TerrenoMedio = e.model.Superficie_Trattata - e.model.TerrenoNormale;
                } else if (e.model.TerrenoMedio == 0 && e.model.TerrenoTenace != 0) {
                    e.model.TerrenoTenace = e.model.Superficie_Trattata - e.model.TerrenoNormale;
                }
            } else if (e.model.dirtyFields.TerrenoMedio == true) {
                if (e.model.TerrenoMedio < 0 || e.model.TerrenoMedio == undefined) e.model.TerrenoMedio = 0;
                if (e.model.TerrenoNormale != 0 && e.model.TerrenoTenace == 0) {
                    e.model.TerrenoNormale = e.model.Superficie_Trattata - e.model.TerrenoMedio;
                } else if (e.model.TerrenoNormale == 0 && e.model.TerrenoTenace != 0) {
                    e.model.TerrenoTenace = e.model.Superficie_Trattata - e.model.TerrenoMedio;
                }
            } else {
                if (e.model.TerrenoTenace < 0 || e.model.TerrenoTenace == undefined) e.model.TerrenoTenace = 0;
                if (e.model.TerrenoNormale != 0 && e.model.TerrenoMedio == 0) {
                    e.model.TerrenoNormale = e.model.Superficie_Trattata - e.model.TerrenoTenace;
                } else if (e.model.TerrenoNormale == 0 && e.model.TerrenoMedio != 0) {
                    e.model.TerrenoMedio = e.model.Superficie_Trattata - e.model.TerrenoTenace;
                }
            }

        }

        if (e.model.Sup_A < 0 || e.model.Sup_A == undefined) e.model.Sup_A = 0;
        if (e.model.Sup_B < 0 || e.model.Sup_B == undefined) e.model.Sup_B = 0;
        if (e.model.TerrenoNormale < 0 || e.model.TerrenoNormale == undefined) e.model.TerrenoNormale = 0;
        if (e.model.TerrenoMedio < 0 || e.model.TerrenoMedio == undefined) e.model.TerrenoMedio = 0;
        if (e.model.TerrenoTenace < 0 || e.model.TerrenoTenace == undefined) e.model.TerrenoTenace = 0;
        aggiornataSup_TotaleLavorazioni_Edit_Terzisti(e, parentItem);
        e.model.dirtyFields.TerrenoNormale = false;
        e.model.dirtyFields.TerrenoMedio = false;
        e.model.dirtyFields.TerrenoTenace = false;
        // NOT e.model.dirtyFields.ltAssegnato --> se viene sporcato, modifica anche il richiesto che non è corretto
        if (!e.model.dirtyFields.ltAssegnato == true && e.model.dirty == true)
            calcoloFabbisogno_Terzisti(parentRowItem, e.model, e.sender);
        if (richiesto < e.model.ltrichiesto && richiesto > 0)
            e.model.ltrichiesto = richiesto;
        var date = Date.parse(e.model.Validita_Inizio);
        grid.refresh();
        e.model.Validita_Inizio = new Date(date);
        if (UpdateVal[e.model.Piva] == undefined) UpdateVal[e.model.Piva] = {}
        if (UpdateVal[e.model.Piva][e.model.Programmazione_Cod] == undefined) UpdateVal[e.model.Piva][e.model.Programmazione_Cod] = {}
        if (UpdateVal[e.model.Piva][e.model.Programmazione_Cod][e.model.Macrouso_UMA_Cod] == undefined) UpdateVal[e.model.Piva][e.model.Programmazione_Cod][e.model.Macrouso_UMA_Cod] = {}
        UpdateVal[e.model.Piva][e.model.Programmazione_Cod][e.model.Macrouso_UMA_Cod][e.model.LAV_COD] = date
        let updV = UpdateVal[e.model.Piva][e.model.Programmazione_Cod][e.model.Macrouso_UMA_Cod];
        let rows = $("#" + gridID).data("kendoGrid").dataSource.data();
    }
    coloraRighe_Lavorazioni_Terzisti("#" + gridID, e);
}

function aggiornataSup_TotaleLavorazioni_Edit_Terzisti(e, parentRow) {
    var sup_UMA = parentRow.sup_tot;
    var sup_UMA_A = parentRow.supA;
    var sup_UMA_B = parentRow.supB;
    var TerrenoTenace = parentRow.tessitura_Tenace;
    var TerrenoMedio = parentRow.tessitura_Media;
    var TerrenoNormale = parentRow.tessitura_Norm;


    if (e.model.Superficie_Trattata != (e.model.Sup_A + e.model.Sup_B)) {
        if (e.model.Sup_A == 0) {
            e.model.Sup_B = e.model.Superficie_Trattata;
        } else if (e.model.Sup_B == 0) {
            e.model.Sup_A = e.model.Superficie_Trattata;
        } else if (e.model.Sup_A > e.model.Sup_B) {
            e.model.Sup_A = e.model.Superficie_Trattata - e.model.Sup_B;
        } else {
            e.model.Sup_B = e.model.Superficie_Trattata - e.model.Sup_A;
        }
    }

    if (e.model.Superficie_Trattata != (e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace).toFixed(4)) {
        if (e.model.TerrenoNormale == 0 && e.model.TerrenoMedio == 0 && e.model.Superficie_Trattata < TerrenoTenace) {
            e.model.TerrenoTenace = e.model.Superficie_Trattata;
        } else if (e.model.TerrenoMedio == 0 && e.model.TerrenoTenace == 0 && e.model.Superficie_Trattata < TerrenoNormale) {
            e.model.TerrenoNormale = e.model.Superficie_Trattata;
        } else if (e.model.TerrenoNormale == 0 && e.model.TerrenoTenace == 0 && e.model.Superficie_Trattata < TerrenoMedio) {
            e.model.TerrenoMedio = e.model.Superficie_Trattata;
        } 
        if (e.model.Superficie_Trattata != (e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace).toFixed(4)) {
            if (e.model.TerrenoNormale > 0) {
                e.model.TerrenoNormale = Math.max(0, (parseFloat((- e.model.TerrenoMedio - e.model.TerrenoTenace).toFixed(4)) + e.model.Superficie_Trattata))
            } else if (e.model.TerrenoMedio > 0) {
                e.model.TerrenoMedio = Math.max(0, (parseFloat((- e.model.TerrenoNormale - e.model.TerrenoTenace).toFixed(4)) + e.model.Superficie_Trattata))
            } else if (e.model.TerrenoTenace > 0) {
                e.model.TerrenoTenace = Math.max(0, (parseFloat((- e.model.TerrenoNormale - e.model.TerrenoMedio).toFixed(4)) + e.model.Superficie_Trattata))
            }
        }
        if (e.model.TerrenoNormale > TerrenoNormale) {
            if (e.model.TerrenoMedio < TerrenoMedio) {
                e.model.TerrenoMedio = e.model.TerrenoNormale - TerrenoNormale;
                e.model.TerrenoNormale = TerrenoNormale;
            } else {
                e.model.TerrenoTenace = e.model.TerrenoNormale - TerrenoNormale;
                e.model.TerrenoNormale = TerrenoNormale;
            }
        } else if (e.model.TerrenoMedio > TerrenoMedio) {
            if (e.model.TerrenoNormale < TerrenoNormale) {
                e.model.TerrenoNormale = e.model.TerrenoMedio - TerrenoMedio;
                e.model.TerrenoMedio = TerrenoMedio;
            } else {
                e.model.TerrenoTenace = e.model.TerrenoMedio - TerrenoMedio;
                e.model.TerrenoMedio = TerrenoMedio;
            }
        } else if (e.model.TerrenoTenace > TerrenoTenace) {
            if (e.model.TerrenoMedio < TerrenoMedio) {
                e.model.TerrenoMedio = e.model.TerrenoTenace - TerrenoTenace;
                e.model.TerrenoTenace = TerrenoTenace;
            } else {
                e.model.TerrenoNormale = e.model.TerrenoTenace - TerrenoTenace;
                e.model.TerrenoTenace = TerrenoTenace;
            }
        }
       
    }

}

function calcoloFabbisogno_Terzisti(rowParent, row, grid) {
    //let costo = tabellaCalcoloCosti.filter((elem) => {
    //    return elem.Macrouso_UMA_Cod == rowParent.Macrouso_UMA_Cod && elem.Lav_UMA_Cod == row.Lav_UMA_Cod && elem.Id_Attivita == row.Attivita_Cod;
    //})[0];
    let costo = filtraTabellaCalcoloCosti(rowParent.Macrouso_UMA_Cod, row.Lav_UMA_Cod, null, row.Attivita_Cod, rowParent.Regolamento_Cod);
    if (costo != undefined && row.Superficie_Trattata != undefined && row.Sup_B != undefined && row.TerrenoMedio != undefined && row.TerrenoNormale != undefined && row.TerrenoTenace != undefined) {
        let costoCarburante = 0;
        switch (row.Car_Cod) {
            case "1":
            case Gasolio.toString():
                costoCarburante = costo.Gasolio_Lt;
                break;
            case Benzina.toString():
            case "4":
                costoCarburante = costo.Benzina_Lt;
                break;
            case Gasolio_Serra.toString():
                costoCarburante = costo.Gasolio_Lt;
                break;
        }
        /*
        if (costo.Lav_UMA_Des.includes("(MAX "))
            costoCarburante /= 4
        */
        if (costo.Udm_Alternativa == null || costo.Udm_Alternativa == undefined || costo.Udm_Alternativa == "") {
            Superficie_Trattata = row.Superficie_Trattata;
            //fabbisogno_Superficie_Trattata = 0;
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

            row.fabbisognoCalc = parseFloat((fabbisogno_Superficie_Trattata +
                fabbisogno_UMA_B_Edit +
                fabbisogno_TerrenoMedio +
                fabbisogno_TerrenoTenace +
                fabbisogno_Trasferimenti).toFixed(4));

        } else {
            if (rowParent.Macrouso_UMA_Cod == "1034") {
                row.fabbisognoCalc = row.Qta_Manuale * row.Mesi * costoCarburante;
            } else {
                row.fabbisognoCalc = row.Qta_Manuale * costoCarburante;
            }
        }

        row.ltrichiesto = row.fabbisognoCalc;
        if (row.Validita_Inizio != "") {
            var date = Date.parse(row.Validita_Inizio);
            grid.refresh();
            row.Validita_Inizio = new Date(date);
        }
    }
}

function App_onDataBoundLavorazioniTerzisti(e) {
    coloraRighe_Lavorazioni_Terzisti("#" + e.sender.element[0].id, e);
}

function coloraRighe_Lavorazioni_Terzisti(grid_elem, e) {
    var grid = $(grid_elem).data('kendoGrid');
    var items = e.sender.items();
    var columns = e.sender.columns;

    var precOK = true;
    var indexColumnCUAA = grid.wrapper.find(".k-grid-header [data-field=" + "CUAA" + "]").index();
    var indexColumnLtrichiesto = grid.wrapper.find(".k-grid-header [data-field=" + "ltrichiesto" + "]").index();
    var totaleCarb = parseFloat($("#benzinaTerzisti")[0].value) + parseFloat($("#gasolioTerzisti")[0].value) + parseFloat($("#gasolioSerraTerzisti")[0].value)
    var indexColumnSupTot_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "Superficie_Trattata" + "]").index();

    var indexColumnSupA_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "Sup_A" + "]").index();
    var indexColumnSupB_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "Sup_B" + "]").index();

    var indexColumnTerrenoNormale_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoNormale" + "]").index();
    var indexColumnTerrenoMedio_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoMedio" + "]").index();
    var indexColumnTerrenoTenace_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoTenace" + "]").index();

    var indexColumnNoteComp = grid.wrapper.find(".k-grid-header [data-field=" + "Note_Compilatore" + "]").index();

    var indexColumnData = grid.wrapper.find(".k-grid-header [data-field=" + "Validita_Inizio" + "]").index();

    var parentRow = e.sender.element.parents(".k-detail-row").prev();
    var parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    var parentItem = parentGrid.dataItem(parentRow);

    var indexLtRichiestoParent = parentGrid.wrapper.find(".k-grid-header [data-field=" + "ltrichiesto" + "]").index();
    var totLt = parseInt(calcTotaleColonna("ltrichiesto", $("#tab_griglia_terzisti")[0], false));
    var totLtLav = parseInt(calcTotaleColonna("ltrichiesto", $(grid_elem)[0], false));
    var totParent = parentItem.ltrichiesto;
    totLt = totLt - totParent;

    var indexColumnLav = grid.wrapper.find(".k-grid-header [data-field=" + "LavUMA" + "]").index();
    var indexColumnQtaManuale = grid.wrapper.find(".k-grid-header [data-field=" + "Qta_Manuale" + "]").index();
    var lav_Alt_Spec = lav_alt[parentItem.Macrouso_UMA_Cod];
    var lav_NO_Spec = {};
    var indexChanged = false;

    var parentArray = [];
    parentArray.push(parentItem.sup_tot == undefined ? parentItem.sup_UMA : parentItem.sup_tot);
    parentArray.push(parentItem.supA_Edit == undefined ? parentItem.sup_UMA_A : parentItem.supA_Edit);
    parentArray.push(parentItem.supB_Edit == undefined ? parentItem.sup_UMA_B : parentItem.supB_Edit);
    parentArray.push(parentItem.tessitura_Norm_Edit == undefined ? parentItem.TerrenoNormale : parentItem.tessitura_Norm_Edit);
    parentArray.push(parentItem.tessitura_Media_Edit == undefined ? parentItem.TerrenoMedio : parentItem.tessitura_Media_Edit);
    parentArray.push(parentItem.tessitura_Tenace_Edit == undefined ? parentItem.TerrenoTenace : parentItem.tessitura_Tenace_Edit);

    if ((totLt + totLtLav) > totaleCarb && QS_Avanzamento == 0) {

        gasolioTerz.value(gasolioTerz.value() + parseInt((totLt + totLtLav) - totaleCarb))
        totaleCarb += parseInt((totLt + totLtLav) - totaleCarb)
    }

    var rows = e.sender.tbody.children();
    //controllo per ogni riga se sono presenti degli errori
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

        //controllo se la riga padre di questa è stata salvata
        if (parentItem.dirty == true && dataItem.richiestaDettaglioCod == 0) {
            for (var i = 0; i < row.children().length; i++) {
                AddErrorClass(row, i, errorCell, "Prima di proseguire e' necessario salvare la riga precedente");
            }
            precOK = false;
        } else {
            for (var i = 0; i < row.children().length; i++) {
                RemoveErrorClass(row, i, errorCell);
            }
            precOK = true;
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
                    let Val_InizioConfronto = new Date(dataItem.Validita_Inizio);
                    Val_InizioConfronto = new Date(new Date(Val_InizioConfronto.setDate(1)).setMonth(0))
                    if (dataItem.Validita_Inizio < Val_InizioConfronto) {
                        AddErrorClass(row, indexColumnData, errorCell, "Anno diverso da quello corrente");
                    } else {
                        RemoveErrorClass(row, indexColumnData, errorCell)
                    }

                }
            } else if (dataItem.richiestaDettaglioCod == 0 && !dataItem.dirtyFields.Validita_Inizio == true) {
                AddErrorClass(row, indexColumnData, "data", "");
                AddErrorClass(row, indexColumnData, errorCell, "È necessario specificare una data all'inserimento di una lavorazione");
            } else {
                RemoveErrorClass(row, indexColumnData, errorCell);
                RemoveErrorClass(row, indexColumnData, "data");
            }
        }

        if (dataItem.Programmazione_Cod < 0 && dataItem.Programmazione_Cod !== -4)
            continue

        if (precOK) {

            //controllo la presenza di lavorazioni in contrasto tra loro e/o un numero eccessivo di una specifica lavorazione
            indexChanged = CheckLavorazioniAlternative(row, parentItem, dataItem, indexColumnLav, lav_Alt_Spec, lav_NO_Spec, indexChanged);
            CheckMaxUdm_Alt(row, parentItem, dataItem, indexColumnQtaManuale);
            indexChanged = CheckMaxNum_Op(row, parentItem, dataItem, grid, indexColumnLav, indexChanged);

            checkNoteObbligatorie(row, dataItem, indexColumnNoteComp)

            let lavValiditaFiltrato = $.grep(elencoLavValidita, function (e) { return e.Macrouso_UMA_Cod == dataItem.Macrouso_UMA_Cod && e.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && (e.Regolamento_Cod == dataItem.Regolamento_Cod || e.Regolamento_Cod == RegolamentoEntrambi); });

            if (lavValiditaFiltrato.length > 0 && dataItem.LAV_UMA != "") {
                if (!(Date.parse(lavValiditaFiltrato[0].Validita_Inizio) <= dato && Date.parse(lavValiditaFiltrato[0].Validita_Fine) >= dato)) {
                    if (!isNaN(dato)) {
                        AddErrorClass(row, indexColumnLav, errorCell, "Lavorazione non più valida" + (QS_Avanzamento == 1 ? " alla data inserita" : ""));
                        indexChanged = true;
                    }
                }
                else if (!indexChanged)
                    RemoveErrorClass(row, indexColumnLav, errorCell);
            }

            //controllo se litri richiesti maggiore di litri totali richiesti inizialmente
            if ((totaleCarb > 0) &&
                (totLt + totLtLav) > totaleCarb) {

                //gasolioTerz.value(gasolioTerz.value() + parseInt((totLt + totLtLav) - totaleCarb))
                //AddErrorClass(row, indexColumnLtrichiesto, errorCell,
                //  "I litri richiesti superano il totale specificato nella richiesta iniziale di " + parseInt(((totLt + totLtLav) - totaleCarb).toFixed(4)).toString() + " litri");

                //AddErrorClass(parentRow, indexLtRichiestoParent, errorCell,
                //    "I litri richiesti superano il totale richiesto inzialmente di " + ((totLt + totLtLav) - totaleCarb).toFixed(4).toString() + " litri");

            } else {
                //RemoveErrorClass(parentRow, indexLtRichiestoParent, errorCell);
                if (dataItem.ltrichiesto > dataItem.fabbisognoCalc) {
                    AddErrorClass(row, indexColumnLtrichiesto, errorCell, "Litri richiesti superiori a quelli calcolati");
                }
                else {
                    RemoveErrorClass(row, indexColumnLtrichiesto, errorCell);
                }
            }

            parentItem.ltrichiesto = totLtLav;

            //controllo se la superficie lavorata supera la superficie totale dichiarata
            if (parentItem.sup_tot < dataItem.Superficie_Trattata) {
                AddErrorClass(row, indexColumnSupTot_Edit, errorCell, SuperficieOltre + (dataItem.Superficie_Trattata - parentItem.sup_tot) + "ha");
            } else {
                RemoveErrorClass(row, indexColumnSupTot_Edit, errorCell);
            }

            //controllo se la superficie lavorata in zona pendenza A supera la superficie pendenza A totale dichiarata
            checkErroriLavorazioni(parentItem.supA_Edit, dataItem.Sup_A, row, indexColumnSupA_Edit, SuperficieOltre)


            //controllo se la superficie lavorata in zona pendenza B supera la superficie pendenza B totale dichiarata
            checkErroriLavorazioni(parentItem.supB_Edit, dataItem.Sup_B, row, indexColumnSupB_Edit, SuperficieOltre)

            //controllo se la superficie lavorata in zona tessitura normale supera la superficie tessitura normale totale dichiarata
            checkErroriLavorazioni(parentItem.tessitura_Norm_Edit, dataItem.TerrenoNormale, row, indexColumnTerrenoNormale_Edit, SuperficieOltre)

            //controllo se la superficie lavorata in zona tessitura media supera la superficie tessitura media totale dichiarata
            checkErroriLavorazioni(parentItem.tessitura_Media_Edit, dataItem.TerrenoMedio, row, indexColumnTerrenoMedio_Edit, SuperficieOltre)


            //controllo se la superficie lavorata in zona tessitura tenace supera la superficie tessitura tenace totale dichiarata
            checkErroriLavorazioni(parentItem.tessitura_Tenace_Edit, dataItem.TerrenoTenace, row, indexColumnTerrenoTenace_Edit, SuperficieOltre)


            //controllo se la somma delle superfici in pendenza A e B supera la superficie totale dichiarata
            var totPend = parseFloat((dataItem.Sup_A + dataItem.Sup_B).toFixed(4));
            if (dataItem.Superficie_Trattata.toFixed(4) < totPend) {
                AddErrorClass(row, indexColumnSupA_Edit, errorCell,
                    "La somma delle superfici in Zona A e Zona B supera la superficie totale di " + (totPend - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
                AddErrorClass(row, indexColumnSupB_Edit, errorCell,
                    "La somma delle superfici in Zona A e Zona B supera la superficie totale di " + (totPend - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
            } else {
                if (dataItem.Sup_A < 0) {
                    AddErrorClass(row, indexColumnSupA_Edit, errorCell, SuperficieNegativa);
                } else {
                    //RemoveErrorClass(row, indexColumnSupA_Edit, errorCell)
                }

                if (dataItem.Sup_B < 0) {
                    AddErrorClass(row, indexColumnSupB_Edit, errorCell, SuperficieNegativa);
                } else {
                    //RemoveErrorClass(row, indexColumnSupB_Edit, errorCell)
                }
                //row.children().eq(indexColumnSupA_Edit).removeClass(errorCell);
                //row.children().eq(indexColumnSupB_Edit).removeClass(errorCell);
            }

            //controllo se la somma delle superfici in tessitura normale, media e tenace supera la superficie totale dichiarata
            var totTes = parseFloat((dataItem.TerrenoNormale + dataItem.TerrenoMedio + dataItem.TerrenoTenace).toFixed(4));
            if (dataItem.Superficie_Trattata.toFixed(4) < totTes) {
                AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell,
                    SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) supera la superficie totale dichiarata di " + (totTes - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
                AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell,
                    SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) supera la superficie totale dichiarata di " + (totTes - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
                AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell,
                    SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) supera la superficie totale dichiarata di " + (totTes - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
            } else if (dataItem.Superficie_Trattata > totTes + 0.0001) {
                AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell,
                    SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) è inferiore rispetto alla superficie totale dichiarata di " + (dataItem.Superficie_Trattata - totTes).toFixed(4) + "ha.");
                AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell,
                    SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) è inferiore rispetto alla superficie totale dichiarata di " + (dataItem.Superficie_Trattata - totTes).toFixed(4) + "ha.");
                AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell,
                    SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) è inferiore rispetto alla superficie totale dichiarata di " + (dataItem.Superficie_Trattata - totTes).toFixed(4) + "ha.");
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

                if (/*QS_Avanzamento == 1 &&*/
                    dataItem.Macrouso_UMA_Cod != "" /*&&
                    !dataItem.Note_Compilatore.includes("Anticipazioni")*///&&
                    //lav_incrociati[dataItem.Macrouso_UMA_Cod].length > 0
                ) {
                    //vedi richiestaCarburanti.js riga 874 per commenti
                    var lav_Incr_orig = new Array();
                    var lav_Incr = new Array();
                    if (dataItem.Lav_UMA_Cod != undefined) {
                        lav_Incr_orig = $.grep(lav_incrociati[dataItem.Piva][dataItem.Macrouso_UMA_Cod], function (e) { return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod; }); //{ var d = new Date(e.Validita_Inizio); return e.Lavorazione_UMA == dataItem.Lav_UMA_Cod && d.toLocaleDateString() == dataItem.Validita_Inizio.toLocaleDateString(); });
                    }

                    lav_Incr_orig.forEach(x => lav_Incr.push(JSON.parse(JSON.stringify(x))))

                    if (lav_Incr.length > 0) {

                        let caus = lav_Incr.reduce(function (stri, current) { return stri + current.numero.toString() + " ( " + current.rag_soc + " " + current.val_cod + " ) " }, "")

                        let nOpMax = recupera_nLavPreviste(dataItem.Lav_UMA_Cod, parentItem.Macrouso_UMA_Cod, dataItem.Lav_Cod, parentItem.Regolamento_Cod);

                        if (nOpMax > 1) {

                            if (dataItem.richiestaDettaglioCod == 0) {
                                for (var g = rows.length - 1; g > j; g--) {
                                    let temp = JSON.parse(JSON.stringify(e.sender.dataItem(rows[g])));
                                    if (temp.Lav_UMA_Cod == dataItem.Lav_UMA_Cod /*&& temp.richiestaDettaglioCod != 0*/)
                                        lav_Incr.push(temp)
                                }
                                for (var t = 0; t < Math.floor(lav_Incr.length / 2); t++) {
                                    for (var r = lav_Incr.length - 1; r >= Math.floor(lav_Incr.length / 2); r--) {
                                        if (parentItem.sup_tot_Orig - 0.0001 < lav_Incr[t].Totale_Superficie_UMA + lav_Incr[r].Superficie_Trattata &&
                                            lav_Incr[t].Totale_Superficie_UMA + lav_Incr[r].Superficie_Trattata <= parentItem.sup_tot_Orig) {
                                            lav_Incr[r].Superficie_Trattata = parentItem.sup_tot_Orig;
                                            lav_Incr[t].Totale_Superficie_UMA = parentItem.sup_tot_Orig;
                                            break
                                        }
                                    }
                                }
                                checkErroriLavorazioniMaxVolte(parentItem.sup_tot_Orig, dataItem.Superficie_Trattata, row, indexColumnSupTot_Edit, LavSovrapposte, lav_Incr, caus, nOpMax, dataItem, indexColumnLtrichiesto)
                            } else
                                checkModificaLavorazioniMaxVolte(parentItem, rows, row, dataItem, indexColumnSupTot_Edit, e, true, nOpMax);

                        } else {

                            let assegnato_Sovrapp = $.grep(lav_Incr, function (e) { return e.Stato_Cod === 2005; }).reduce(function (sum, current) { return sum + current.Fabbisogno_Assegnato }, 0);
                            let terreno_Assegnato_Sovrapp = $.grep(lav_Incr, function (e) { return e.Stato_Cod === 2005; }).reduce(function (sum, current) { return sum + current.Totale_Superficie_UMA }, 0);
                            let terreno_Assegnabile_Sovrapp = $.grep(lav_Incr, function (e) { return e.Stato_Cod === 2005; }).reduce(function (sum, current) { return sum + current.Totale_Richiedibile }, 0);
                            let supA_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Pendenza_A_UMA }, 0);
                            let supB_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Pendenza_B_UMA }, 0);
                            let supNormale_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Normale_UMA }, 0);
                            let supMedia_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Media_UMA }, 0);
                            let supTenace_Sovrapp = lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Tenace_UMA }, 0);

                            if (assegnato_Sovrapp < dataItem.fabbisognoCalc && terreno_Assegnato_Sovrapp == terreno_Assegnabile_Sovrapp && dataItem.Superficie_Trattata == (parentItem.sup_tot == undefined ? parentItem.sup_UMA : parentItem.sup_tot)) {

                                if (dataItem.ltrichiesto > 0 && assegnato_Sovrapp > (dataItem.fabbisognoCalc - dataItem.ltrichiesto + parseFloat("0." + (dataItem.fabbisognoCalc + "").split(".")[1])))
                                    AddErrorClass(row, indexColumnLtrichiesto, errorCell,
                                        "E' possibile richiedere solo " + Math.round(dataItem.fabbisognoCalc - assegnato_Sovrapp) + " Lt di carburante per questa lavorazione perchè " + Math.round(assegnato_Sovrapp) + " Lt sono già stati richiesti e approvati nelle richieste numero " + caus);
                                else
                                    RemoveErrorClass(row, indexColumnLtrichiesto, errorCell);

                            } else {

                                tentativoRipartizioneSovrapposizioneTerzisti(parentItem, dataItem, supA_Sovrapp, supB_Sovrapp, supNormale_Sovrapp, supMedia_Sovrapp, supTenace_Sovrapp);

                                checkErroriLavorazioni(parentItem.sup_tot_Orig, dataItem.Superficie_Trattata, row, indexColumnSupTot_Edit, LavSovrapposte, lav_Incr.reduce(function (sum, current) { return sum + current.Totale_Superficie_UMA }, 0), caus)

                                checkErroriLavorazioni(parentItem.supA_Edit, dataItem.Sup_A, row, indexColumnSupA_Edit, LavSovrapposte, lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Pendenza_A_UMA }, 0), caus)

                                checkErroriLavorazioni(parentItem.supB_Edit, dataItem.Sup_B, row, indexColumnSupB_Edit, LavSovrapposte, lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Pendenza_B_UMA }, 0), caus)

                                checkErroriLavorazioni(parentItem.tessitura_Norm_Edit, dataItem.TerrenoNormale, row, indexColumnTerrenoNormale_Edit, LavSovrapposte, lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Normale_UMA }, 0), caus)

                                checkErroriLavorazioni(parentItem.tessitura_Media_Edit, dataItem.TerrenoMedio, row, indexColumnTerrenoMedio_Edit, LavSovrapposte, lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Media_UMA }, 0), caus)

                                checkErroriLavorazioni(parentItem.tessitura_Tenace_Edit, dataItem.TerrenoTenace, row, indexColumnTerrenoTenace_Edit, LavSovrapposte, lav_Incr.reduce(function (sum, current) { return sum + current.Zona_Tessitura_Tenace_UMA }, 0), caus)

                            }
                        }
                    }
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
        }
    }

    lav_alt_superfici = {};
    lav_alt_terreni = {};
    //azzero il conteggio delle lavorazioni
    for (var key in lav_alt_lim[parentItem.Macrouso_UMA_Cod]) {
        lav_alt_lim[parentItem.Macrouso_UMA_Cod][key] = 0;
    }
}

function tentativoRipartizioneSovrapposizioneTerzisti(parentItem, dataItem, supA_Sovrapp, supB_Sovrapp, supNormale_Sovrapp, supMedia_Sovrapp, supTenace_Sovrapp) {
    let diff = 0
    let diffSup = 0;
    //pendenza
    if (dataItem.Sup_A > 0 && parentItem.supA < (dataItem.Sup_A + supA_Sovrapp)) {
        diffSup = parseFloat((parentItem.supA - supA_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.Superficie_Trattata - diffSup).toFixed(4));
        if (parentItem.supB > (dataItem.Sup_B + diff - 0.0001)) {
            dataItem.Sup_A = parseFloat(diffSup);
            dataItem.Sup_B = parseFloat(dataItem.Sup_B + diff);
        } else if (dataItem.Sup_B + diff + diffSup == dataItem.Superficie_Trattata) {
            dataItem.Sup_A = parseFloat(diffSup);
        }
    }
    if (dataItem.Sup_B > 0 && parentItem.supB < (dataItem.Sup_B + supB_Sovrapp)) {
        diffSup = parseFloat((parentItem.supB - supB_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.Superficie_Trattata - diffSup).toFixed(4));
        if (parentItem.supA > (dataItem.Sup_A + diff - 0.0001)) {
            dataItem.Sup_B = parseFloat(diffSup);
            dataItem.Sup_A = parseFloat(dataItem.Sup_A + diff);
        } else if (dataItem.Sup_A + diff + diffSup == dataItem.Superficie_Trattata) {
            dataItem.Sup_B = parseFloat(diffSup);
        }
    }

    //tessitura
    if (dataItem.TerrenoNormale > 0 && parentItem.tessitura_Norm < (dataItem.TerrenoNormale + supNormale_Sovrapp)) {
        diffSup = parseFloat((parentItem.tessitura_Norm - supNormale_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.TerrenoNormale - diffSup).toFixed(4));
        if ((parentItem.tessitura_Media + parentItem.tessitura_Tenace) > (dataItem.TerrenoTenace + dataItem.TerrenoMedio + diff - 0.0001)) {
            if (parentItem.tessitura_Media > (dataItem.TerrenoMedio + diff - 0.0001)) {
                dataItem.TerrenoNormale = parseFloat(diffSup);
                dataItem.TerrenoMedio = parseFloat(dataItem.TerrenoMedio + diff);
            } else {
                diff = parseFloat((diff - (parentItem.tessitura_Media - dataItem.TerrenoMedio).toFixed(4)).toFixed(4));
                if (parentItem.tessitura_Tenace > (dataItem.TerrenoTenace + diff - 0.0001)) {
                    dataItem.TerrenoNormale = parseFloat(diffSup);
                    dataItem.TerrenoMedio = parentItem.tessitura_Media;
                    dataItem.TerrenoTenace = parseFloat(dataItem.TerrenoTenace + diff);
                }
            }
        }
    }
    if (dataItem.TerrenoMedio > 0 && parentItem.tessitura_Media < (dataItem.TerrenoMedio + supMedia_Sovrapp)) {
        diffSup = parseFloat((parentItem.tessitura_Media - supMedia_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.TerrenoMedio - diffSup).toFixed(4));
        if ((parentItem.tessitura_Norm + parentItem.tessitura_Tenace) > (dataItem.TerrenoTenace + dataItem.TerrenoNormale + diff - 0.0001)) {
            if (parentItem.tessitura_Norm > (dataItem.TerrenoNormale + diff - 0.0001)) {
                dataItem.TerrenoMedio = parseFloat(diffSup);
                dataItem.TerrenoNormale = parseFloat(dataItem.TerrenoNormale + diff);
            } else {
                diff = parseFloat((diff - (parentItem.tessitura_Norm - dataItem.TerrenoNormale).toFixed(4)).toFixed(4));
                if (parentItem.tessitura_Tenace > (dataItem.TerrenoTenace + diff - 0.0001)) {
                    dataItem.TerrenoNormale = parentItem.tessitura_Norm;
                    dataItem.TerrenoMedio = parseFloat(diffSup);
                    dataItem.TerrenoTenace = parseFloat(dataItem.TerrenoTenace + diff);
                }
            }
        }
    }
    if (dataItem.TerrenoTenace > 0 && parentItem.tessitura_Tenace < (dataItem.TerrenoTenace + supTenace_Sovrapp)) {
        diffSup = parseFloat((parentItem.tessitura_Tenace - supTenace_Sovrapp).toFixed(4));
        diff = parseFloat((dataItem.TerrenoTenace - diffSup).toFixed(4));
        if ((parentItem.tessitura_Norm + parentItem.tessitura_Media) > (dataItem.TerrenoMedio + dataItem.TerrenoNormale + diff - 0.0001)) {
            if (parentItem.tessitura_Norm > (dataItem.TerrenoNormale + diff - 0.0001)) {
                dataItem.TerrenoTenace = parseFloat(diffSup);
                dataItem.TerrenoNormale = parseFloat(dataItem.TerrenoNormale + diff);
            } else {
                diff = parseFloat((diff - (parentItem.tessitura_Norm - dataItem.TerrenoNormale).toFixed(4)).toFixed(4));
                if (parentItem.tessitura_Media > (dataItem.TerrenoMedio + diff - 0.0001)) {
                    dataItem.TerrenoNormale = parentItem.tessitura_Norm;
                    dataItem.TerrenoTenace = parseFloat(diffSup);
                    dataItem.TerrenoMedio = parseFloat(dataItem.TerrenoMedio + diff);
                }
            }
        }
    }
}


function onEditGrigliaDettagliLavorazioniTerzisti(e) {
    tempAssegnato = Math.round(e.model.ltAssegnato, 2);
    let parentRow = e.sender.element.parents(".k-detail-row").prev();
    let parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);
    if (e.model.TipoCarb == "") {
        let codCar = parentRowItem.Macrouso_UMA_Cod == "1034" ? 8 : 2;
        let carburante_Default = elencoCarburanti.filter((elem) => elem.Car_Cod == codCar)[0];
        e.model.Car_Cod = carburante_Default.Car_Cod;
        e.model.TipoCarb = carburante_Default.TipoCarb;

        //inizializzazione riga
        e.model.SupMaggiorazioneTrasferimenti = 0;
        e.model.fabbisognoCalc = 0;
        e.model.ltrichiesto = 0;
        e.model.ltAssegnato = 0;
        e.model.nLavPreviste = 0;
        e.model.nLavRichieste = 0;
        e.model.piuLavPreviste = 0;
        e.model.piuRaccoltiPrevisti = 0;
        e.model.richiestaDettaglioCod = 0;
        e.model.Note_Approvatore = "";
        e.model.Note_Compilatore = "";
        e.model.Mesi = 0;
        e.model.Attivita_Cod = 0;
        e.model.Qta_Manuale = 0;
        e.model.Udm_Alt = "";
        if (QS_Avanzamento == 1)
            e.model.Validita_Inizio = '';

        /*if (parseInt($("#TxtRimanenza_Gasolio_prec").val()) + parseInt($("#TxtRimanenza_Benzina_prec").val()) + parseInt($("#TxtRimanenza_Gasolio_Serra_prec").val()) != 0)
            e.model.Validita_Inizio = new Date(new Date(e.model.Validita_Inizio.setDate(1)).setMonth(0));
        else
            e.model.Validita_Inizio = dataPrimoAcquisto;*/

        e.model.Macrouso_UMA_Cod = parentRowItem.Macrouso_UMA_Cod;
        e.model.Richiesta_Cod = parentRowItem.Richiesta_Cod;
        e.model.Programmazione_Cod = parentRowItem.Programmazione_Cod;
        e.model.Piva = parentRowItem.piva;
        e.model.Superficie_Trattata = parentRowItem.sup_tot;
        e.model.Sup_A = parentRowItem.supA;
        e.model.Sup_B = parentRowItem.supB;
        e.model.TerrenoNormale = parentRowItem.tessitura_Norm_Edit;
        e.model.TerrenoMedio = parentRowItem.tessitura_Media_Edit;
        e.model.TerrenoTenace = parentRowItem.tessitura_Tenace_Edit;
        e.model.dirty = true;
        colt_elem_added[parentRowItem.Macrouso_UMA_Cod]++;
        if (e.model.Validita_Inizio != "") {
            var date = Date.parse(e.model.Validita_Inizio);
            e.sender.refresh()
            e.model.Validita_Inizio = new Date(date);
        }
    } else {
        if (e.model.Lav_UMA_Cod != "" &&
            e.model.Macrouso_UMA_Cod != "" &&
            e.model.LAV_COD != 0) {
            //let costo = tabellaCalcoloCosti.filter((elem) => {
            //    return elem.Macrouso_UMA_Cod == e.model.Macrouso_UMA_Cod &&
            //        elem.Lav_UMA_Cod == e.model.Lav_UMA_Cod &&
            //        elem.Lav_Cod == e.model.LAV_COD &&
            //        elem.Id_Attivita == e.model.Attivita_Cod;
            //})[0]
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
}

async function InsertLavorazioneTerzisti(e) {

    if ($("#tab_griglia_terzisti").find(".errorCell").length != 0) {
        kendo.alert("Verificare i dati segnalati prima di salvare");
        return false;
    }

    WaitFrame.show();

    var data = $("#tab_griglia_terzisti").data("kendoGrid").dataSource.data();
    let parentRow = $($(container).parents(".k-detail-row")[0]).prev();
    let parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);
    //await AggiornaRichieste_UMA_Terzisti(pivaSelezionata, richiesta_cod, JSON.stringify(data))

    let modificheFatte = false;
    let piva = '0';
    let progr_cod = 0;
    let Macrouso_UMA_Cod = "";
    if (e.data.created.length > 0) {
        modificheFatte = true;
        piva = e.data.created[0].Piva;
        progr_cod = e.data.created[0].Programmazione_Cod;
        Macrouso_UMA_Cod = e.data.created[0].Macrouso_UMA_Cod;
    }
    if (e.data.updated.length > 0) {
        modificheFatte = true;
        piva = e.data.updated[0].Piva;
        progr_cod = e.data.updated[0].Programmazione_Cod;
        Macrouso_UMA_Cod = e.data.updated[0].Macrouso_UMA_Cod;
    }
    if (e.data.destroyed.length > 0) {
        modificheFatte = true;
        piva = e.data.destroyed[0].Piva;
        progr_cod = e.data.destroyed[0].Programmazione_Cod;
        Macrouso_UMA_Cod = e.data.destroyed[0].Macrouso_UMA_Cod;
    }
    if (modificheFatte) {
        await ws_Inserisci_Lavorazioni(piva, richiesta_cod, progr_cod, Macrouso_UMA_Cod, e.data.created, e.data.updated, e.data.destroyed);
        await LeggiRichiesteTerzista(false);
        $("#btn_nuova_richiesta_terzista").trigger("click");
    }

    await LeggiRichiesteTerzista(true);
    $("#btn_nuova_richiesta_terzista").trigger("click");

    WaitFrame.hide();

}

function lavUMA_DropDownEditor_Terzisti(container, options) {
    let parentRow = $($(container).parents(".k-detail-row")[0]).prev();
    let parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    PopolaElencoLavUMA(false, parentRowItem.Macrouso_UMA_Cod, 1, parentRowItem.Regolamento_Cod);

    creaDropDownEditor(container, "LavUMA", "Lav_UMA_Cod", elencoLavUMA, changelavUMA_Terzisti);
}

function changelavUMA_Terzisti(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    parentRow = e.sender.element.parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    parentItem = parentGrid.dataItem(parentRow);
    var parentRowItem = parentGrid.dataItem(parentRow);
    model.LavUMA = dataItem.LavUMA;
    model.Lav_UMA_Cod = dataItem.Lav_UMA_Cod;

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    //al cambio della lavorazione ricompilo tutti i campi della riga
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

                if (costo.Udm_Alternativa && costo.Udm_Alternativa !== "") {
                    model.Udm_Alt = costo.Udm_Alternativa;
                    model.Qta_Manuale = 0;
                }

                grid.refresh();
            }
        );
    }

    model.nLavPreviste = recupera_nLavPreviste(model.Lav_UMA_Cod, parentRowItem.Macrouso_UMA_Cod, model.LAV_COD, parentRowItem.Regolamento_Cod);
    model.nLavRichieste = 1; //model.nLavPreviste
    model.Superficie_Trattata = parentRowItem.supA_Edit + parentRowItem.supB_Edit
    model.Sup_A = parentRowItem.supA_Edit
    model.Sup_B = parentRowItem.supB_Edit
    model.TerrenoMedio = parentRowItem.tessitura_Media_Edit
    model.TerrenoNormale = parentRowItem.tessitura_Norm_Edit
    model.TerrenoTenace = parentRowItem.tessitura_Tenace_Edit
    model.dirty = true;
    calcoloFabbisogno_Terzisti(parentRowItem, model, grid);
    //grid.refresh();
    kendoFastRedrawRow(grid, row);
}

function Carburanti_DropDownEditor_Terzisti(container, options) {

    creaDropDownEditor(container, "TipoCarb", "Car_Cod", elencoCarburanti, changeCarb_Terzisti);
}

function changeCarb_Terzisti(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.TipoCarb = dataItem.TipoCarb;
    model.Car_Cod = dataItem.Car_Cod;
    model.dirty = true;

    calcoloFabbisogno_Terzisti(parentRowItem, model, grid);
    //kendoFastRedrawRow(grid, row);
}

function lavGIAS_DropDownEditor_Terzisti(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    PopolaElencoLavGIAS(false, row.Macrouso_UMA_Cod, row.Lav_UMA_Cod, row.Regolamento_Cod)

    creaDropDownEditor(container, "LavGIAS", "LAV_COD", elencoLavGIAS, changelavGIAS);
}

function changelavGIAS(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.LavGIAS = dataItem.LavGIAS;
    model.LAV_COD = dataItem.LAV_COD;
    model.dirty = true;
    //kendoFastRedrawRow(grid, row);
    calcoloFabbisogno_Terzisti(parentRowItem, model, grid);
}

function MaggiorazioneTerzisti_DropDownEditor(container, options) {
    creaDropDownEditor(container, "valore", "codice", [{ codice: 0, valore: "No" }, { codice: 1, valore: "Sì" }], changeMaggiorazioneTerzisti);
}

function changeMaggiorazioneTerzisti(e) {
    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.SupMaggiorazioneTrasferimenti = dataItem.codice;
    model.dirty = true;

    calcoloFabbisogno_Terzisti(parentRowItem, model, grid);
}

async function LavorazioniMultipleTerzisti(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    $(document.body).append('<div id="griglia_LavorazioniMultiple"></div>');
    /*macro_cod = datiRiga.Macrouso_UMA_Cod;
    program_cod = datiRiga.Programmazione_Cod;
    Veg_Cod = datiRiga.Veg_Cod;
    Id_Cod = datiRiga.Id_Cod;
    if (macro_cod != undefined && macro_cod != "") {
        GrigliaDettagliColture("griglia_dettagliColture", true);*/
    lavorazioni = await PopolaElencoLavUMAMultiple(false, datiRiga.Macrouso_UMA_Cod, 1);
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
            InsertLavorazioniMultipleTerzisti(grid_elem, datiRiga)
            lavorazioniSelezionate = [];
            setTimeout(function () {
                $('#griglia_LavorazioniMultiple').kendoWindow('destroy');
            }, 200);
        }
    }).data('kendoWindow').center();//.maximize();

}

function ConfermaLavorazioniMultiple() {
    $("#griglia_LavorazioniMultiple").data('kendoGrid')._data.forEach(r => { if (r.Selected) lavorazioniSelezionate.push(r) })
    $('#griglia_LavorazioniMultiple').kendoWindow('close');
}

function InsertLavorazioniMultipleTerzisti(IDgriglia, rigaOrigin) {
    var riga;
    var griglia = $(IDgriglia).data('kendoGrid');
    var parentRow = $(IDgriglia).parents(".k-detail-row").prev();
    var parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);
    if (lavorazioniSelezionate.length > 0) {
        lavorazioniSelezionate.forEach(l => {
            if (rigaOrigin.LAV_COD == 0) {
                rigaOrigin.nLavRichieste = 1;
                rigaOrigin.nLavPreviste = 1;
                rigaOrigin.piuLavPreviste = 1;
                rigaOrigin.LAV_COD = l.LAV_COD;
            }
            else {
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
            if (rigaOrigin.Validita_Inizio == "") {
                riga.Validita_Inizio = new Date(Date.now());
            } else {
                riga.Validita_Inizio = rigaOrigin.Validita_Inizio;
            }
            riga.dirtyFields.Validita_Inizio = true;
            riga.Car_Cod = rigaOrigin.Car_Cod;
            riga.TipoCarb = rigaOrigin.TipoCarb;
            let attivita = TrovaAttivitaGIAS(false, parentRowItem.Macrouso_UMA_Cod, riga.Lav_UMA_Cod, riga.LAV_COD, parentRowItem.Regolamento_Cod)

            if (attivita != undefined) {
                riga.Attivita_Des = attivita.Attivita_Des;
                riga.Attivita_Cod = attivita.Attivita_Cod;
            }

            if (l.Lav_UMA_Cod == 10064)
                riga.Udm_Alt = "m"
            calcoloFabbisogno_Terzisti(parentRowItem, riga, griglia);
            griglia.refresh();
        });

        griglia.refresh();
    }
}

function popolaGrigliaLavorazioniMultiple(IDControllo) {

    var omettiAnnulla = false;
    var omettiSalva = true;

    var funzioniCRUD = {
        funzioneRead: PopolaLavorazioniMultiple,
        funzioneSubmit: {
            /*funzione: InsertLavorazioneTerzisti,
            flagInsert: !richiestaRinuncia,
            flagUpdate: true,
            flagDelete: !richiestaRinuncia*/
        },
        UtenteAbilitatoInserimentoModifica: modifica_richiesto,
        UtenteAbilitatoCancellazione: modifica_richiesto,
        omettiPulsantiSalva: omettiSalva,
        omettiPulsantiAnnulla: omettiAnnulla,
        checkBoxFunction: KendoCheck_Lavorazioni
    };

    var styleInt = "background-color: deepskyblue; vertical-align: top";
    var idModel = "Lav_UMA_Cod";
    var campiKendoModel = {
        Selected: { editable: false, type: "Boolean" },
        Lav_UMA_Cod: { editable: false, type: "number" },
        LavUMA: { editable: false, type: "string" },
        LavGIAS: { editable: false, type: "string" },
        LAV_COD: { editable: false, type: "number" }
    }

    var colonneKendoGrid = [
        { field: "LavUMA", width: "200px", title: "Lavorazione", headerAttributes: { style: styleInt } },
    ]

    var parametriKendoGrid = {
        excel: false, pdf: false,
        groupable: false,
        headerAttributes: { style: styleInt },
        columnMenu: false,
        filterable: false,
        lockCancella: false,
        pageable: false,//{ pageSizes: [5, 10, 20, 50, 100] },
        btnEliminaTuttiFiltri: false
    };

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var funzioniPrimaDopoEventi = {
        //funzioneDaChiamareDopoSave: InsertLavorazioneTerzisti,
        //funzioneDaChiamareDopoEdit: InsertLavorazioneTerzisti,
        //funzioneDaChiamareDopoDataBound: App_onDataBoundLavorazioniTerzisti,
        //funzioneDaChiamareDopoEdit: onEditGrigliaDettagliLavorazioniTerzisti
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["LavUMA"];

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

    /*var grid = $("#" + IDControllo).data("kendoGrid");
    var indx = IDControllo.split("_")[1];
    gasolioTotTerzLAV[indx] = 0;
    grid._data.filter((x) => { return x.TipoCarb == "Gasolio" }).forEach((x) => { gasolioTotTerzLAV[indx] += x.ltrichiesto });
    gasolioTotTerzLAV[indx].toFixed(4);

    grid.bind("cellClose", grid_cellCloseTerz);

    $("#" + IDControllo).on("mousedown", ".k-grid-cancel-changes", function (e) {
        colt_elem_added[id_gruppo] = 0;
    });*/
}

function KendoCheck_Lavorazioni() {
    if (modifica_richiesto) {
        var checked = this.checked;
        var row = $(this).parents("tr");
        var grid = $('#griglia_LavorazioniMultiple').data("kendoGrid");
        var dataItem = grid.dataItem(row);
        dataItem.Selected = checked;
        dataItem.dirty = true;
        rowKendoGridSelected(row, checked)
    } else {
        this.checked = !this.checked;
    }
}

function PopolaLavorazioniMultiple(options) {
    options.success(lavorazioni);
}