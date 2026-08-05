var elencoAziende;
var dtRichiestaTerzista = "";
var pivaInsertGrid = "";

function funzioneDaChiamareDopoDeleteParziale(e) {
    if ($(e.target).closest("[data-role='grid']")[0].id == "tab_griglia_terzistiparziale") {
        var grid = $("#tab_griglia_terzistiparziale").getKendoGrid();
        var row = $(e.target).closest("tr");
        dataItem = grid.dataItem(row);
        dataItem.cancellato = true;
    }
}

async function popolaGrigliaDettagliLavorazioniParzialiTerzisti(piva, richiesta_cod) {
    var IDControllo = "tab_griglia_terzistiparziale"
    var omettiAnnulla = false;

    var omettiSalva = true;

    //Coltivazioni sotto serra
    //if (id_gruppo == 1034) {
    //    mesiVisibili = true;
    //}

    var funzioniCRUD = {
        funzioneRead: RicercaLavorazioniParzialiTerzisti,
        funzioneSubmit: {
            funzione: InsertLavorazioneParzialiTerzisti,
            flagInsert: !richiestaRinuncia,
            flagUpdate: true,
            flagDelete: !richiestaRinuncia
        },
        UtenteAbilitatoInserimentoModifica: modifica_richiesto,
        UtenteAbilitatoCancellazione: modifica_richiesto,
        omettiPulsantiSalva: omettiSalva,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var idModel = "Lavorazione_Parziale_Cod";
    var campiKendoModel = {
        Richiesta_Cod: { editable: false, type: "number" },
        Piva: { editable: false, type: "string" },
        Lavorazione_Parziale_Cod: { editable: false, type: "string" },
        LavUMA: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        LavGIAS: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        LAV_COD: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        TipoCarb: { editable: !richiestaRinuncia && modifica_richiesto, type: "string", validation: { required: true } },
        SupMaggiorazioneTrasferimenti: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        fabbisognoCalc: { editable: false, type: "number", validation: { required: true } },
        ltrichiesto: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        ltAssegnato: { editable: !richiestaRinuncia && modifica_assegnato, type: "number", validation: { required: true } },
        nLavPreviste: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        nLavRichieste: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        piuLavPreviste: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        piuRaccoltiPrevisti: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Superficie_Trattata: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Sup_A: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Sup_B: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoNormale: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoMedio: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        TerrenoTenace: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        Udm_Alt: { editable: !richiestaRinuncia && modifica_richiesto, type: "string" },
        Qta_Manuale: { editable: !richiestaRinuncia && modifica_richiesto, type: "number" },
        Validita_Inizio: { editable: !richiestaRinuncia && modifica_richiesto, type: "date" },
        Regolamento_Cod: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: false }, defaultValue: 1 },
        Regolamento_Check: { editable: !richiestaRinuncia && modifica_richiesto, type: "boolean", validation: { required: false } }
    }

    var footerTemplateStringRichiesto = "#=calcTotaleColonna('" + "ltrichiesto" + "', " + IDControllo + ")#";
    var footerTemplateStringAssegnato = "#=calcTotaleColonna('" + "ltAssegnato" + "', " + IDControllo + ")#";
    var colonneKendoGrid = [
        { field: "LavUMA", width: "250px", title: TraduzioneMultiResx(gestioneCarbResx, "LavUMA", "Lavorazione U.M.A."), filterable: { multi: true, search: true }, editor: lavUMA_DropDownEditor_TerzistiParziale },
        //{ field: "LavGIAS", width: "200px", title: TraduzioneMultiResx(gestioneCarbResx, "LavGIAS", "Lavorazione GIAS"), headerAttributes: { style: styleInt }, editor: lavGIAS_DropDownEditor_TerzistiParziale },
        { field: "TipoCarb", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "TipoCarb", "Tipo Carburante"), filterable: { multi: true, search: true }, editor: Carburanti_DropDownEditor_TerzistiParziale },
        { field: "Qta_Manuale", width: "100px", title: "Qta. Manuale", attributes: { class: "Lav_Alt" }, editor: NumberEditorNoSpin4Decimals },
        { field: "Udm_Alt", width: "100px", title: "UdM", filterable: { multi: true, search: true } }
    ]

    if (gestioneBiologico) {
        colonneKendoGrid.splice(0, 0, {
            field: "Regolamento_Check",
            //template: '<input type=\"checkbox\" # if(Regolamento_Cod > 1){ # checked #} # />',
            template: "#=(Regolamento_Cod === 4 ? 'Si' : 'No')#",
            title: "Biologico",
            width: 110,
            attributes: { class: "k-text-center checkboxCustom edit_onInsert editBio" },
            editor: booleanEditor,
        })
    }

    colonneKendoGrid.push({
        field: "SupMaggiorazioneTrasferimenti",
        width: "100px",
        title: TraduzioneMultiResx(gestioneCarbResx, "SupMaggiorazioneTrasferimenti", "Superficie Maggiorazione Trasferimenti"),
        filterable: { multi: true, search: true },
        editor: MaggiorazioneTerzisti_DropDownEditorParziale,
        template: '#= (SupMaggiorazioneTrasferimenti == 1) ? "Sì" : "No" #'
    });
    colonneKendoGrid.push({ field: "fabbisognoCalc", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "fabbisognoCalc", "Fabbisogno Calcolato (lt.)"), format: "{0:n0}", editor: NumberEditorNoSpinInteger })
    colonneKendoGrid.push({ field: "ltrichiesto", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "richiesto", "Richiesto (lt.)"), footerTemplate: footerTemplateStringRichiesto, format: "{0:n0}", editor: NumberEditorNoSpinInteger })
    colonneKendoGrid.push({ field: "ltAssegnato", width: "100px", title: TraduzioneMultiResx(gestioneCarbResx, "ltAssegnato", "Assegnato (lt.)"), footerTemplate: footerTemplateStringAssegnato, format: "{0:n0}", editor: NumberEditorNoSpinInteger })
    colonneKendoGrid.push({ field: "nLavPreviste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "nLavPreviste", "Numero Lavorazioni Previste"), editor: NumberEditorNoSpinInteger })
    colonneKendoGrid.push({ field: "nLavRichieste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "nLavRichieste", "Numero Lavorazioni Richieste"), editor: NumberEditorNoSpinInteger })
    colonneKendoGrid.push({ field: "piuLavPreviste", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "piuLavPreviste", "Piu Lavorazioni Previste"), editor: NumberEditorNoSpinInteger })

    colonneKendoGrid.push({ field: "piuRaccoltiPrevisti", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "piuRaccoltiPrevisti", "Piu Raccolti Previsti"), editor: NumberEditorNoSpinInteger })
    colonneKendoGrid.push({ field: "Superficie_Trattata", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "Superficie_Trattata", "Superficie Trattata"), format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
    colonneKendoGrid.push({ field: "Sup_A", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "Sup_A", "Zona Pendenza A (ha)"), format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
    colonneKendoGrid.push({ field: "Sup_B", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "Sup_B", "Zona Pendenza B (ha)"), format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
    colonneKendoGrid.push({ field: "TerrenoNormale", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoNormale", "Zona Tessitura Normale (ha)"), format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
    colonneKendoGrid.push({ field: "TerrenoMedio", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoMedio", "Zona Tessitura Media (ha)"), format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })
    colonneKendoGrid.push({ field: "TerrenoTenace", width: "72px", title: TraduzioneMultiResx(gestioneCarbResx, "TerrenoTenace", "Zona Tessitura Tenace (ha)"), format: "{0:n4}", editor: NumberEditorNoSpin4Decimals })

    if (QS_Avanzamento == 1) {
        colonneKendoGrid.push({
            field: "Validita_Inizio",
            width: "92px",
            title: "Data",
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #'
        });
    }

    var colonneCustomKendoGrid = new Array();
    //if (!richiestaRinuncia && (modifica_richiesto)) {
    //    colonneCustomKendoGrid.push({
    //        command: {
    //            template: "<div class='btn btn-info btnInfo btnDettaglio' style='width:25px;border:0px;' onclick=LavorazioniMultipleTerzistiParziale(this.closest('tr'),this.closest('.k-grid'))><span class='fa fa-plus lampeggiante'></span></div>"
    //        },
    //        title: "Inserisci Multiple", width: "68px"
    //    });
    //}

    var parametriKendoGrid = {
        excel: true, pdf: false,
        groupable: false,
        columnMenu: false,
        filterable: false,
        lockCancella: false,
        pageable: { pageSizes: [50] },
        //pageable: false,//{ pageSizes: [5, 10, 20, 50, 100] },
        btnEliminaTuttiFiltri: false,
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };

    var parametriPerLettura = [piva, richiesta_cod];
    var parametriDataSource = {};

    var funzioniPrimaDopoEventi = {
        //funzioneDaChiamareDopoSave: InsertLavorazioneParzialiTerzisti,
        //funzioneDaChiamareDopoEdit: InsertLavorazioneTerzisti,
        ////funzioneDaChiamareDopoDataBound: App_onDataBoundLavorazioniTerzisti,
        funzioneDaChiamareDopoEdit: onEditGrigliaDettagliLavorazioniTerzistiParziale,
        funzioneDaChiamareDopoDelete: funzioneDaChiamareDopoDeleteParziale
    };
    var mostraRigheCancellate = true;
    //TODO perchè le dropDown sono comunque modificabili anche se non in inserimento?
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

    var grid = $("#" + IDControllo).data("kendoGrid");
    //var indx = IDControllo.split("_")[1];
    //gasolioTotTerzLAV[indx] = 0;
    //grid._data.filter((x) => { return x.TipoCarb == "Gasolio" }).forEach((x) => { gasolioTotTerzLAV[indx] += x.ltrichiesto });
    //gasolioTotTerzLAV[indx].toFixed(4);

    grid.bind("cellClose", grid_cellCloseTerzParz);

    $("#" + IDControllo).on("mousedown", ".k-grid-cancel-changes", function (e) {
        colt_elem_added[richiesta_cod] = 0;
    });

    $("#" + IDControllo + " .k-grid-content").on("change", "input.k-checkbox", function (e) {
        var grid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
        var row = this.closest("tr")
        var model = grid.dataItem(row)
        if (model.id === "" && model.LavUMA === "") {
            model.Regolamento_Cod = this.checked ? 4 : 1;
            model.Regolamento_Check = model.Regolamento_Cod > 1 ? true : false;
            model.dirty = true;
            grid.refresh();
        } else {
            this.checked = !this.checked;
        }
        //PopolaElencoLavUMA(false, "", 1, model.Regolamento_Cod);
        //KendoDDL("id_selEstrazione").setDataSource(new kendo.data.DataSource({ data: elencoLavUMA }));
    });
}

function booleanEditor(container, options) {
    var guid = kendo.guid();
    $('<input class="k-checkbox" id="' + guid + '" type="checkbox" name="' + options.field + '" data-type="boolean" data-bind="checked:' + options.field + '">').appendTo(container);
    $('<label class="k-checkbox-label" for="' + guid + '">&#8203;</label>').appendTo(container);
}

function grid_cellCloseTerzParz(e) {
    if (e.model.dirtyFields != undefined) {
        var gridID = e.sender.element[0].id;
        var grid = $("#" + gridID).data("kendoGrid");

        //controllo superficie trattata maggiore di zero e calcolo fabbisogno carburante 
        if (e.model.dirtyFields.Superficie_Trattata == true) {
            //if (e.model.Superficie_Trattata < 0 || e.model.Superficie_Trattata == undefined || e.model.Superficie_Trattata > parentRowItem.sup_tot) e.model.Superficie_Trattata = parentRowItem.sup_tot;
            //if (e.model.Superficie_Trattata < 0 || e.model.Superficie_Trattata == undefined ) e.model.Superficie_Trattata = parentRowItem.sup_tot;
            /*aggiornataSup_TotaleLavorazioni_Edit_TerzistiParziale(e, parentItem);*/  //commentato perchè prende i valori dalla parent riga che non ho
            calcoloFabbisogno_TerzistiParziale("", e.model, e.sender);
            e.model.dirtyFields.Superficie_Trattata = false;
            grid.refresh();
        } else
            //controllo lavRichieste maggiore di zero
            if (e.model.dirtyFields.nLavRichieste == true) {
                if (e.model.nLavRichieste < 0 || e.model.nLavRichieste == undefined) e.model.nLavRichieste = 0;
                calcoloFabbisogno_TerzistiParziale("", e.model, e.sender);
                e.model.dirtyFields.nLavRichieste = false;
            }
        //Controllo superfici pendenza maggiore di zero e calcolo fabbisogno carburante
        if (e.model.dirtyFields.Sup_A == true || e.model.dirtyFields.Sup_B == true) {
            if (e.model.dirtyFields.Sup_A == true) {
                if (e.model.Sup_A < 0 || e.model.Sup_A == undefined) e.model.Sup_A = 0;
                e.model.Sup_B = e.model.Superficie_Trattata - e.model.Sup_A;
            } else if (e.model.dirtyFields.Sup_B == true) {
                if (e.model.Sup_B < 0 || e.model.Sup_B == undefined) e.model.Sup_B = 0;
                e.model.Sup_A = e.model.Superficie_Trattata - e.model.Sup_B;
            }
            e.model.dirtyFields.Sup_A = false;
            e.model.dirtyFields.Sup_B = false;
        }
        //controllo litri richiesti maggiore di zero e correttezza quantità
        if (e.model.dirtyFields.ltrichiesto == true) {
            if (e.model.ltrichiesto < 0 || e.model.ltrichiesto == undefined) e.model.ltrichiesto = 0;
            grid.refresh();
        } else
            //controllo litri assegnati inferiori di quelli richiesti
            if (e.model.dirtyFields.ltAssegnato == true) {
                if (e.model.ltAssegnato < 0 || e.model.ltAssegnato == undefined) e.model.ltAssegnato = 0;
                let RichiestoDecurtato = e.model.ltrichiesto - (e.model.ltrichiesto * Percentuale_Decurtamento / 100);
                if (e.model.ltAssegnato > RichiestoDecurtato) e.model.ltAssegnato = RichiestoDecurtato
                grid.refresh();
            } else
                //Controllo superfici tessitura maggiore di zero e calcolo fabbisogno carburante
                if (e.model.dirtyFields.TerrenoNormale == true || e.model.dirtyFields.TerrenoMedio == true || e.model.dirtyFields.TerrenoTenace == true) {
                    if (e.model.TerrenoMedio != undefined && e.model.TerrenoNormale != undefined && e.model.TerrenoTenace != undefined) {
                        //e.model.Superficie_Trattata = e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace
                    }
                    if (e.model.TerrenoTenace < 0 || e.model.TerrenoTenace == undefined) e.model.TerrenoTenace = 0;
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

                    e.model.dirtyFields.TerrenoNormale = false;
                    e.model.dirtyFields.TerrenoMedio = false;
                    e.model.dirtyFields.TerrenoTenace = false;
                }
        if (e.model.Sup_A < 0 || e.model.Sup_A == undefined) e.model.Sup_A = 0;
        if (e.model.Sup_B < 0 || e.model.Sup_B == undefined) e.model.Sup_B = 0;
        if (e.model.TerrenoNormale < 0 || e.model.TerrenoNormale == undefined) e.model.TerrenoNormale = 0;
        if (e.model.TerrenoMedio < 0 || e.model.TerrenoMedio == undefined) e.model.TerrenoMedio = 0;
        if (e.model.TerrenoTenace < 0 || e.model.TerrenoTenace == undefined) e.model.TerrenoTenace = 0;
        //aggiornataSup_TotaleLavorazioni_Edit_TerzistiParziale(e, parentItem);
        //let richiesto = e.model.ltrichiesto;
        if (!e.model.dirtyFields.ltAssegnato == true && e.model.dirty == true)
            calcoloFabbisogno_TerzistiParziale("", e.model, e.sender);
        //if (richiesto < e.model.ltrichiesto && richiesto > 0)
        //    e.model.ltrichiesto = richiesto;
        var date = Date.parse(e.model.Validita_Inizio);
        grid.refresh();
        e.model.Validita_Inizio = new Date(date);
        if (UpdateVal[e.model.Piva] == undefined) UpdateVal[e.model.Piva] = {}
        if (UpdateVal[e.model.Piva][e.model.Programmazione_Cod] == undefined) UpdateVal[e.model.Piva][e.model.Programmazione_Cod] = {}
        if (UpdateVal[e.model.Piva][e.model.Programmazione_Cod][e.model.Macrouso_UMA_Cod] == undefined) UpdateVal[e.model.Piva][e.model.Programmazione_Cod][e.model.Macrouso_UMA_Cod] = {}
        UpdateVal[e.model.Piva][e.model.Programmazione_Cod][e.model.Macrouso_UMA_Cod][e.model.LAV_COD] = date
        //let updV = UpdateVal[e.model.Piva][e.model.Programmazione_Cod][e.model.Macrouso_UMA_Cod];
        let rows = $("#" + gridID).data("kendoGrid").dataSource.data();
    }
    if (permesso_approvazione_richiesta == true && $("#stato_pratica_cod").val() == "2002" && e.model.dirtyFields.ltAssegnato == true && e.model.ltAssegnato != tempAssegnato) {
        let diff = tempAssegnato - e.model.ltAssegnato;
        switch (e.model.Car_Cod) {
            case "2": if (gasolioTerzAppro.value() > diff)
                gasolioTerzAppro.value(gasolioTerzAppro.value() - diff);
                break;

            case "3": if (benzinaTerzAppro.value() > diff)
                benzinaTerzAppro.value(benzinaTerzAppro.value() - diff);
                break;

            case "8": if (gasolioSerraTerzAppro.value() > diff)
                gasolioSerraTerzAppro.value(gasolioSerraTerzAppro.value() - diff);
                break;

            default:
        }
    }

    coloraRighe_Lavorazioni_Terzisti_Parziali("#tab_griglia_terzistiparziale", e);
}

function coloraRighe_Lavorazioni_Terzisti_Parziali(grid_elem, e) {
    var grid = $(grid_elem).data('kendoGrid');

    var indexColumnSupA_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "Sup_A" + "]").index();
    var indexColumnSupB_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "Sup_B" + "]").index();

    var indexColumnTerrenoNormale_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoNormale" + "]").index();
    var indexColumnTerrenoMedio_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoMedio" + "]").index();
    var indexColumnTerrenoTenace_Edit = grid.wrapper.find(".k-grid-header [data-field=" + "TerrenoTenace" + "]").index();

    var indexColumnLav = grid.wrapper.find(".k-grid-header [data-field=" + "LavUMA" + "]").index();

    var rows = e.sender.tbody.children();
    //controllo per ogni riga se sono presenti degli errori
    for (var j = rows.length - 1; j >= 0; j--) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        //controllo se la somma delle superfici in pendenza A e B supera la superficie totale dichiarata
        var totPend = parseFloat((dataItem.Sup_A + dataItem.Sup_B).toFixed(4));
        if (dataItem.Superficie_Trattata < totPend - 0.0001) {
            AddErrorClass(row, indexColumnSupA_Edit, errorCell,
                "La somma delle superfici in Zona A e Zona B supera la superficie totale di " + (totPend - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
            AddErrorClass(row, indexColumnSupB_Edit, errorCell,
                "La somma delle superfici in Zona A e Zona B supera la superficie totale di " + (totPend - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
        } else {
            if (dataItem.Sup_A < 0) {
                AddErrorClass(row, indexColumnSupA_Edit, errorCell, SuperficieNegativa);
            }

            if (dataItem.Sup_B < 0) {
                AddErrorClass(row, indexColumnSupB_Edit, errorCell, SuperficieNegativa);
            }
        }

        //controllo se la somma delle superfici in tessitura normale, media e tenace supera la superficie totale dichiarata
        var totTes = parseFloat((dataItem.TerrenoNormale + dataItem.TerrenoMedio + dataItem.TerrenoTenace).toFixed(4));
        if (dataItem.Superficie_Trattata < totTes - 0.0001) {
            AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell,
                SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) supera la superficie totale dichiarata di " + (totTes - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
            AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell,
                SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) supera la superficie totale dichiarata di " + (totTes - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
            AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell,
                SommaVariTipi + totTes + "ha (Normale, Medio, Tenace) supera la superficie totale dichiarata di " + (totTes - dataItem.Superficie_Trattata).toFixed(4) + "ha.");
        } else {
            if (dataItem.TerrenoNormale < 0) {
                AddErrorClass(row, indexColumnTerrenoNormale_Edit, errorCell, SuperficieNegativa);
            }
            if (dataItem.TerrenoMedio < 0) {
                AddErrorClass(row, indexColumnTerrenoMedio_Edit, errorCell, SuperficieNegativa);
            }
            if (dataItem.TerrenoTenace < 0) {
                AddErrorClass(row, indexColumnTerrenoTenace_Edit, errorCell, SuperficieNegativa);
            }
        }

        //controllo la presenza di lavorazioni in contrasto tra loro e/o un numero eccessivo di una specifica lavorazione
        //indexChanged = CheckLavorazioniAlternative(row, parentItem, dataItem, indexColumnLav, lav_Alt_Spec, lav_NO_Spec, indexChanged);
        CheckMaxNum_Op_Parziali(row, dataItem, grid, indexColumnLav);

    }
}

function aggiornataSup_TotaleLavorazioni_Edit_TerzistiParziale(e, parentRow) {
    var sup_UMA = parentRow.sup_UMA;
    var sup_UMA_A = parentRow.sup_UMA_A;
    var sup_UMA_B = parentRow.sup_UMA_B;
    var TerrenoTenace = parentRow.tessitura_Tenace;
    var TerrenoMedio = parentRow.tessitura_Media;
    var TerrenoNormale = parentRow.tessitura_Norm;

    if (e.model.Superficie_Trattata != (e.model.Sup_A + e.model.Sup_B)) {
        if (e.model.Sup_A == 0) {
            e.model.Sup_B = e.model.Superficie_Trattata;
        } else if (e.model.Sup_B == 0) {
            e.model.Sup_A = e.model.Superficie_Trattata;
        } else {
            //e.model.Sup_B = parseFloat(((e.model.Superficie_Trattata * sup_UMA_B) / sup_UMA).toFixed(4));
            //e.model.Sup_A = parseFloat((e.model.Superficie_Trattata - e.model.Sup_B).toFixed(4));
        }
    }

    if (e.model.Superficie_Trattata != (e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace).toFixed(4)) {
        if (e.model.TerrenoNormale == 0 && e.model.TerrenoMedio == 0) {
            e.model.TerrenoTenace = e.model.Superficie_Trattata;
        } else if (e.model.TerrenoMedio == 0 && e.model.TerrenoTenace == 0) {
            e.model.TerrenoNormale = e.model.Superficie_Trattata;
        } else if (e.model.TerrenoNormale == 0 && e.model.TerrenoTenace == 0) {
            e.model.TerrenoMedio = e.model.Superficie_Trattata;
        } else if (e.model.TerrenoNormale > e.model.TerrenoTenace && e.model.TerrenoNormale > e.model.TerrenoMedio) {
            e.model.TerrenoNormale = e.model.Superficie_Trattata;
            e.model.TerrenoMedio = 0;
            e.model.TerrenoTenace = 0;
        } else if (e.model.TerrenoMedio > e.model.TerrenoTenace && e.model.TerrenoMedio > e.model.TerrenoNormale) {
            e.model.TerrenoMedio = e.model.Superficie_Trattata;
            e.model.TerrenoNormale = 0;
            e.model.TerrenoTenace = 0;
        } else if (e.model.TerrenoTenace > e.model.TerrenoNormale && e.model.TerrenoTenace > e.model.TerrenoMedio) {
            e.model.TerrenoTenace = e.model.Superficie_Trattata;
            e.model.TerrenoNormale = 0;
            e.model.TerrenoMedio = 0;
        }
        if (e.model.Superficie_Trattata != (e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace).toFixed(4)) {
            if (e.model.TerrenoNormale > 0) {
                e.model.TerrenoNormale = Math.max(0, e.model.TerrenoNormale - ((e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace).toFixed(4) - e.model.Superficie_Trattata))
            } else if (e.model.TerrenoMedio > 0) {
                e.model.TerrenoMedio = Math.max(0, e.model.TerrenoMedio - ((e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace).toFixed(4) - e.model.Superficie_Trattata))
            } else if (e.model.TerrenoTenace > 0) {
                e.model.TerrenoTenace = Math.max(0, e.model.TerrenoTenace - ((e.model.TerrenoNormale + e.model.TerrenoMedio + e.model.TerrenoTenace).toFixed(4) - e.model.Superficie_Trattata))
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

function calcoloFabbisogno_TerzistiParziale(rowParent, row, grid) {
    if (row.nLavRichieste == 0) {
        row.nLavRichieste = 1;
    }
    let costo = tabellaCalcoloCosti.filter((elem) => {
        //return elem.Macrouso_UMA_Cod == rowParent.Macrouso_UMA_Cod && elem.Lav_UMA_Cod == row.Lav_UMA_Cod && elem.Id_Attivita == row.Attivita_Cod;
        return elem.Lav_UMA_Cod == row.Lav_UMA_Cod && (elem.Regolamento_Cod == row.Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi);
    }).sort(function (a, b) { return parseFloat(b.Gasolio_Lt) - parseFloat(a.Gasolio_Lt); })[0];
    if (costo != undefined && row.Superficie_Trattata != undefined && row.Sup_B != undefined && row.TerrenoMedio != undefined && row.TerrenoNormale != undefined && row.TerrenoTenace != undefined) {
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
            //if (rowParent.Macrouso_UMA_Cod == "1034") {
            //    row.fabbisognoCalc = row.Qta_Manuale * row.Mesi * costoCarburante;
            //} else {
            row.fabbisognoCalc = row.Qta_Manuale * costoCarburante;
            //}
        }

        row.ltrichiesto = row.fabbisognoCalc;
        if (row.Validita_Inizio != "") {
            var date = Date.parse(row.Validita_Inizio);
            grid.refresh();
            row.Validita_Inizio = new Date(date);
        }
    }

    aggiornaRichiestaInizialeParz()
}

function aggiornaRichiestaInizialeParz() {
    var totaleGas = parseFloat($("#gasolioTerzisti")[0].value)
    var totaleBenz = parseFloat($("#benzinaTerzisti")[0].value)
    var totaleSerra = parseFloat($("#gasolioSerraTerzisti")[0].value)
    var totaleGasAppro = parseFloat($("#gasolioTerzistiAppro")[0].value)
    var totaleBenzAppro = parseFloat($("#benzinaTerzistiAppro")[0].value)
    var totaleSerraAppro = parseFloat($("#gasolioSerraTerzistiAppro")[0].value)

    if (isNaN(totaleGas)) {
        totaleGas = 0;
    }
    if (isNaN(totaleBenz)) {
        totaleBenz = 0;
    }
    if (isNaN(totaleSerra)) {
        totaleSerra = 0;
    }
    if (isNaN(totaleGasAppro)) {
        totaleGasAppro = 0;
    }
    if (isNaN(totaleBenzAppro)) {
        totaleBenzAppro = 0;
    }
    if (isNaN(totaleSerraAppro)) {
        totaleSerraAppro = 0;
    }

    var totLtGas = parseInt(calcTotaleColonna("ltrichiesto", $("#tab_griglia_terzistiparziale")[0], false, 2));
    if (totLtGas > totaleGas && QS_Avanzamento == 0) {
        gasolioTerz.value(gasolioTerz.value() + parseInt(totLtGas - totaleGas))
        totaleGas += parseInt(totLtGas - totaleGas)
    }
    var totLtBenz = parseInt(calcTotaleColonna("ltrichiesto", $("#tab_griglia_terzistiparziale")[0], false, 3));
    if (totLtBenz > totaleBenz && QS_Avanzamento == 0) {
        benzinaTerz.value(benzinaTerz.value() + parseInt(totLtBenz - totaleBenz))
        totaleBenz += parseInt(totLtBenz - totaleBenz)
    }
    var totLtSerra = parseInt(calcTotaleColonna("ltrichiesto", $("#tab_griglia_terzistiparziale")[0], false, 8));
    if (totLtSerra > totaleSerra && QS_Avanzamento == 0) {
        gasolioSerraTerz.value(gasolioSerraTerz.value() + parseInt(totLtSerra - totaleSerra))
        totaleSerra += parseInt(totLtSerra - totaleSerra)
    }
    if (totaleGasAppro > totaleGas * (100 - Percentuale_Decurtamento) / 100) {
        gasolioTerzAppro.value(Math.round(totaleGas * (100 - Percentuale_Decurtamento) / 100));
    }
    if (totaleBenzAppro > totaleBenz * (100 - Percentuale_Decurtamento) / 100) {
        benzinaTerzAppro.value(Math.round(totaleBenz * (100 - Percentuale_Decurtamento) / 100));
    }
    if (totaleSerraAppro > totaleSerra * (100 - Percentuale_Decurtamento) / 100) {
        gasolioSerraTerzAppro.value(Math.round(totaleSerra * (100 - Percentuale_Decurtamento) / 100));
    }

    if (isNaN(parseFloat($("#gasolioTerzisti")[0].value))) {
        gasolioTerz.value(0)
    }
    if (isNaN(parseFloat($("#benzinaTerzisti")[0].value))) {
        benzinaTerz.value(0)
    }
    if (isNaN(parseFloat($("#gasolioSerraTerzisti")[0].value))) {
        gasolioSerraTerz.value(0)
    }
}

function onEditGrigliaDettagliLavorazioniTerzistiParziale(e) {
    tempAssegnato = e.model.ltAssegnato;
    if (e.model.LAV_COD == "") {
        let parentRow = $($(e.container).parents(".k-detail-row")[0]).prev();
        let parentGrid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
        var parentRowItem = parentGrid.dataItem(parentRow);
        //let codCar = parentRowItem.Macrouso_UMA_Cod == "1034" ? "8" : "2";
        let codCar = "2";
        //parentRowItem.Macrouso_UMA_Cod == "1034" ? "8" : "2";
        //}
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
        ////e.model.Qta_Manuale = 0;
        e.model.Udm_Alt = "";
        if (QS_Avanzamento == 1)
            e.model.Validita_Inizio = '';

        if (parseInt($("#TxtRimanenza_Gasolio_prec").val()) + parseInt($("#TxtRimanenza_Benzina_prec").val()) + parseInt($("#TxtRimanenza_Gasolio_Serra_prec").val()) != 0)
            e.model.Validita_Inizio = new Date(new Date(e.model.Validita_Inizio.setDate(1)).setMonth(0));
        else
            e.model.Validita_Inizio = dataPrimoAcquisto;

        ////e.model.Macrouso_UMA_Cod = parentRowItem.Macrouso_UMA_Cod;

        e.model.Richiesta_Cod = richiesta_cod;
        e.model.Piva = e.model.piva;
        ////e.model.Programmazione_Cod = parentRowItem.Programmazione_Cod;
        //e.model.Piva = piva;
        ////e.model.Superficie_Trattata = parentRowItem.sup_tot;
        ////e.model.Sup_A = parentRowItem.supA;
        ////e.model.Sup_B = parentRowItem.supB;
        ////e.model.TerrenoNormale = parentRowItem.tessitura_Norm_Edit;
        ////e.model.TerrenoMedio = parentRowItem.tessitura_Media_Edit;
        ////e.model.TerrenoTenace = parentRowItem.tessitura_Tenace_Edit;
        ////e.model.dirty = true;
        ////colt_elem_added[parentRowItem.Macrouso_UMA_Cod]++;
        ////if (e.model.Validita_Inizio != "") {
        ////    var date = Date.parse(e.model.Validita_Inizio);
        ////    e.sender.refresh()
        ////    e.model.Validita_Inizio = new Date(date);
        ////}
    } else {
        if (($(e.container[0]).hasClass("editBio") && e.model.Lav_UMA_Cod !== "")) {
            e.sender.closeCell();
        }
        if (e.model.Lav_UMA_Cod != "" &&
            e.model.LAV_COD != 0) {
            let costo = tabellaCalcoloCosti.filter((elem) => {
                return elem.Lav_UMA_Cod == e.model.Lav_UMA_Cod && (elem.Regolamento_Cod == e.model.Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi);
            })[0];
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
            }
            //else {
            //    e.sender.closeCell();
            //}
        }
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

function CheckMaxNum_Op_Parziali(row, dataItem, grid, indexColumnLav) {

    if (dataItem.Lav_UMA_Cod) {
        let nOpPreviste = recupera_nLavPreviste_Parziale(dataItem.Lav_UMA_Cod, dataItem.Regolamento_Cod);
        //Prima filtro gli elementi con il Lav_UMA_Cod che mi interessa
        //Poi calcolo la somma degli ettari lavorati
        let arr = grid.dataSource.data().filter((el) => {
            return (el.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && (!el.deleted));
        });
        //let arr = grid.dataSource.data().filter((el) => { return (el.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && (!el.deleted) && el.CUAA == null); }); //aggiunto Gloria la condizione el.CUAA==null per escludere dal conteggio quelli che hanno la cuaa valorizzata

        if (arr.length > nOpPreviste) {

            AddErrorClass(row,
                indexColumnLav,
                warningCell,
                "Lavorazione già presente in questa richiesta");
        }
        //if (dataItem.Lav_UMA_Cod === "10233") {
        //    if (grid.dataSource.data().filter((el) => {
        //        return (el.Lav_UMA_Cod == dataItem.Lav_UMA_Cod && (!el.deleted))
        //    }).length > 1) {
        //        AddErrorClass(row,
        //            indexColumnLav,
        //            errorCell,
        //            "Massimo " + nOpPreviste + " operazioni previste per " + dataItem.LavUMA);
        //        indexChanged = true;
        //    }
        //}
    }
}

async function InsertLavorazioneParzialiTerzisti(e) {

    aggiornaRichiestaInizialeParz();

    if ($("#tab_griglia_terzistiparziale").find(".errorCell").length != 0) {
        kendo.alert("Verificare i dati segnalati prima di salvare");
        return false;
    }

    WaitFrame.show();

    var data = $("#tab_griglia_terzistiparziale").data("kendoGrid").dataSource.data();

    //await AggiornaRichieste_UMA_Terzisti(pivaSelezionata, richiesta_cod, JSON.stringify(data))

    let modificheFatte = false;
    let piva = '0';
    let progr_cod = 0;
    let Macrouso_UMA_Cod = "";
    let lav_par_cod = "";

    if (e.data.created.length > 0) {
        modificheFatte = true;
        piva = e.data.created[0].Piva;
        progr_cod = e.data.created[0].Programmazione_Cod;
        //Macrouso_UMA_Cod = e.data.created[0].Macrouso_UMA_Cod;
    }
    if (e.data.updated.length > 0) {
        modificheFatte = true;
        piva = e.data.updated[0].Piva;
        progr_cod = e.data.updated[0].Programmazione_Cod;
        lav_par_cod = e.data.updated[0].Lavorazione_Parziale_Cod;
        // Macrouso_UMA_Cod = e.data.updated[0].Macrouso_UMA_Cod;
    }
    if (e.data.destroyed.length > 0) {
        modificheFatte = true;
        piva = e.data.destroyed[0].Piva;
        progr_cod = e.data.destroyed[0].Programmazione_Cod;
        lav_par_cod = e.data.updated[0].Lavorazione_Parziale_Cod;
        //Macrouso_UMA_Cod = e.data.destroyed[0].Macrouso_UMA_Cod;
    }
    if (modificheFatte) {
        await ws_Inserisci_Lavorazioni_Parziali(piva, richiesta_cod, e.data.created, e.data.updated, e.data.destroyed);
        //await LeggiRichiesteTerzista(false);
        //$("#btn_nuova_richiesta_terzista").trigger("click");
    }

    await LeggiRichiesteTerzista(true);
    $("#btn_nuova_richiesta_terzista").trigger("click");

    WaitFrame.hide();

}

function lavUMA_DropDownEditor_TerzistiParziale(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let grid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    //let parentRow = $($(container).parents(".k-detail-row")[0]).prev();
    //let parentGrid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    //var parentRowItem = parentGrid.dataItem(parentRow);

    PopolaElencoLavUMA(false, "", 1, row.Regolamento_Cod);

    creaDropDownEditor(container, "LavUMA", "Lav_UMA_Cod", elencoLavUMA, changelavUMA_TerzistiParziale);
}

function changelavUMA_TerzistiParziale(e) {
    //let rowHtml = $(container).parents("tr")[0];
    let grid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    //let row = grid.dataItem(rowHtml);

    var dataItem = e.sender.dataItem();
    //var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    //var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    //var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    //parentRow = e.sender.element.parents(".k-detail-row").prev();
    //parentGrid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    //parentItem = parentGrid.dataItem(parentRow);
    //var parentRowItem = parentGrid.dataItem(parentRow);
    model.LavUMA = dataItem.LavUMA;
    model.Lav_UMA_Cod = dataItem.Lav_UMA_Cod;

    ////parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    ////parentGrid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    ////var parentRowItem = parentGrid.dataItem(parentRow);

    //al cambio della lavorazione ricompilo tutti i campi della riga
    PopolaElencoLavGIAS(false, " ", model.Lav_UMA_Cod, model.Regolamento_Cod);

    if (elencoLavGIAS.length > 0) {
        model.LavGIAS = elencoLavGIAS[0].LavGIAS;
        model.LAV_COD = elencoLavGIAS[0].LAV_COD;
    }
    ////    PopolaElencoAttivitaGIAS(false, parentRowItem.Macrouso_UMA_Cod, model.Lav_UMA_Cod, model.LAV_COD, parentRowItem.Regolamento_Cod).then(
    ////        elencoAttivita => {
    ////            if (elencoAttivita.length == 0) {
    ////                elencoAttivita.push({
    ////                    Attivita_Cod: 0,
    ////                    Attivita_Des: ''
    ////                });
    ////            }
    ////            model.Attivita_Des = elencoAttivita[0].Attivita_Des;
    ////            model.Attivita_Cod = elencoAttivita[0].Attivita_Cod;

    let costo = tabellaCalcoloCosti.filter((elem) => {
        return elem.Lav_UMA_Cod == model.Lav_UMA_Cod && (elem.Regolamento_Cod == model.Regolamento_Cod || elem.Regolamento_Cod == RegolamentoEntrambi);
        //return elem.Macrouso_UMA_Cod == parentRowItem.Macrouso_UMA_Cod && elem.Lav_UMA_Cod == model.Lav_UMA_Cod && elem.Id_Attivita == model.Attivita_Cod;
    })[0];

    if (costo.Udm_Alternativa && costo.Udm_Alternativa !== "") {
        model.Udm_Alt = costo.Udm_Alternativa;
        model.Qta_Manuale = 0;
    }

    grid.refresh();
    ////        }
    ////    );
    //}

    model.nLavPreviste = recupera_nLavPreviste_Parziale(model.Lav_UMA_Cod, model.Regolamento_Cod);
    model.nLavRichieste = 1; //model.nLavPreviste
    //model.Superficie_Trattata = dataitem.supA_Edit + dataitem.supB_Edit
    //model.Sup_A = dataitem.supA_Edit
    //model.Sup_B = dataitem.supB_Edit
    //model.TerrenoMedio = dataitem.tessitura_Media_Edit
    //model.TerrenoNormale = dataitem.tessitura_Norm_Edit
    //model.TerrenoTenace = dataitem.tessitura_Tenace_Edit
    model.dirty = true;
    calcoloFabbisogno_TerzistiParziale("", model, grid);
    grid.refresh();
    //kendoFastRedrawRow(grid, row);
}

function Carburanti_DropDownEditor_TerzistiParziale(container, options) {

    creaDropDownEditor(container, "TipoCarb", "Car_Cod", elencoCarburanti, changeCarb_TerzistiParziale);
}

function changeCarb_TerzistiParziale(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.TipoCarb = dataItem.TipoCarb;
    model.Car_Cod = dataItem.Car_Cod;
    model.dirty = true;

    calcoloFabbisogno_TerzistiParziale(parentRowItem, model, grid);
    //kendoFastRedrawRow(grid, row);
}

function lavGIAS_DropDownEditor_TerzistiParziale(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    PopolaElencoLavGIAS(false, row.Macrouso_UMA_Cod, row.Lav_UMA_Cod, row.Regolamento_Cod)

    creaDropDownEditor(container, "LavGIAS", "LAV_COD", elencoLavGIAS, changelavGIASParziale);
}

function changelavGIASParziale(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.LavGIAS = dataItem.LavGIAS;
    model.LAV_COD = dataItem.LAV_COD;
    model.dirty = true;
    //kendoFastRedrawRow(grid, row);
    calcoloFabbisogno_TerzistiParziale(parentRowItem, model, grid);
}

function MaggiorazioneTerzisti_DropDownEditorParziale(container, options) {
    creaDropDownEditor(container, "valore", "codice", [{ codice: 0, valore: "No" }, { codice: 1, valore: "Sì" }], changeMaggiorazioneTerzistiParziale);
}

function changeMaggiorazioneTerzistiParziale(e) {
    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    if (parentGrid == undefined) {
        parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    }
    var parentRowItem = parentGrid.dataItem(parentRow);

    model.SupMaggiorazioneTrasferimenti = dataItem.codice;
    model.dirty = true;

    calcoloFabbisogno_TerzistiParziale(parentRowItem, model, grid);
}

async function LavorazioniMultipleTerzistiParziale(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    $(document.body).append('<div id="griglia_LavorazioniMultiple"></div>');
    /*macro_cod = datiRiga.Macrouso_UMA_Cod;
    program_cod = datiRiga.Programmazione_Cod;
    Veg_Cod = datiRiga.Veg_Cod;
    Id_Cod = datiRiga.Id_Cod;
    if (macro_cod != undefined && macro_cod != "") {
        GrigliaDettagliColture("griglia_dettagliColture", true);*/
    lavorazioni = await PopolaElencoLavUMAMultiple(false, datiRiga.Macrouso_UMA_Cod, datiRiga.Regolamento_Cod);
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
            InsertLavorazioniMultipleTerzistiParziale(grid_elem, datiRiga)
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

function InsertLavorazioniMultipleTerzistiParziale(IDgriglia, rigaOrigin) {
    var riga;
    var griglia = $(IDgriglia).data('kendoGrid');
    var parentRow = $(IDgriglia).parents(".k-detail-row").prev();
    var parentGrid = $("#tab_griglia_terzistiparziale").data("kendoGrid");
    if (parentGrid == undefined) {
        parentGrid = $("#tab_griglia_terzisti").data("kendoGrid");
    }
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
            calcoloFabbisogno_TerzistiParziale(parentRowItem, riga, griglia);
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

function ucUmaLavParziali_reloadGrid() {
    var kGrid = KendoGrid("tab_griglia_terzistiparziale");
    if (kGrid !== undefined) {
        kGrid.dataSource.read();
    }
}