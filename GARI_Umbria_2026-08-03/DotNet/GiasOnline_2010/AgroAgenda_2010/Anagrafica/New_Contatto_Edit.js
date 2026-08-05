// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA RAPPORTI CONTABILI
// --------------------------------------------------------------------------------------------------------------------------

/*const { each } = require("jquery");*/

function PopolaGrigliaRapportiContabili(IDControllo, tipoCarica) {

    let UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = ($("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True");

    var ImpedisciCancellazioneRapportiContabili = GetPropertyFromJson($(Controls.OpzioniContatti).val(), "SUPERUSER_IMPEDISCI_ELIMINAZIONE_CONTATTI_E_RISORSE_UMANE");
    if (ImpedisciCancellazioneRapportiContabili == "1")
        UteAbilitatoCanc = false;

    // funzioneSubmit: { funzione: SubmitLottoAssegna, flagInsert: true, flagUpdate: true, flagDelete: true },
    var funzioniCRUD = {
        funzioneRead: Contatto_RapportiContabili,
        funzioneSubmit: { funzione: SubmitGrid_RapportiContabili, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };
    var idModel = "key_rap_cont";
    var campiKendoModel = {
        key_rap_cont: { editable: false, type: "string" },
        Cod_RisUm: { editable: false, type: "string" },
        Qualifica_Cod: { editable: true, type: "string" },
        Settore_Des: { editable: true, type: "string" },
        Rapporto_Des: { editable: true, type: "string", validation: { required: false } },
        Rapporto_Des_Origine: { editable: true, type: "string", validation: { required: false } },
        Cod_Rapporto_Origine: { editable: true, type: "number" },
        Qualifica_Des: { editable: true, type: "string", validation: { required: false } },
        Mansione_Des: { editable: true, type: "string", validation: { required: false } },

        Validita_Inizio:
        {
            editable: true, type: "date", defaultValue: new Date("1900/01/01")
        },
        Validita_Fine:
        {
            editable: true, type: "date", defaultValue: new Date("2100/12/31")
        },

        Attivita_Des: { editable: true, type: "string" },
        TipoRapporto_Des: { editable: true, type: "string" },
        Ore_Settimanali: { editable: true, type: "number" },
        Info_Famiglia: { editable: true, type: "string" },
        Classificazione_Des: { editable: true, type: "string" },
        Descrizione: { editable: true, type: "string" },
        Conto_Descr: { editable: true, type: "string" },
        Descr_Conto_Pat: { editable: true, type: "string" },

    };

    var colonneKendoGrid = [
        {
            field: "Settore_Des", title: TraduzioneMultiResx(contattoEditResxArray, "CodiceContatto", "Codice Contatto"), width: 100
        },
        {
            field: "Rapporto_Des", title: TraduzioneMultiResx(contattoEditResxArray, "RapportoContabileAbbr", "Rapp.Contab."), editor: rapportoContabile_DropDownEditor, width: 250
        },
        {
            field: "Attivita_Des", title: TraduzioneMultiResx(contattoEditResxArray, "AttivitàQualificaBio", "Attività / Qualifica x Bio"), width: 180
        },
        {
            field: "Validita_Inizio", title: TraduzioneMultiResx(contattoEditResxArray, "ValiditàInizio", "Validità Inizio"), format: "{0:dd/MM/yyyy}", width: 125
        },
        {
            field: "Validita_Fine", title: TraduzioneMultiResx(contattoEditResxArray, "ValiditàFine", "Validità Fine"), format: "{0:dd/MM/yyyy}", width: 125
        },
        {
            field: "Qualifica_Des", title: TraduzioneMultiResx(contattoEditResxArray, "QualificaGestioneCosti", "Qualifica (x gestione Costi)"), editor: qualifiche_DropDownEditor, width: 180
        },
        {
            field: "Mansione_Des", title: TraduzioneMultiResx(contattoEditResxArray, "MansioneGestioneCosti", "Mansione (x gestione Costi)"), editor: mansione_DropDownEditor, width: 180
        },
        {
            field: "TipoRapporto_Des", title: TraduzioneMultiResx(contattoEditResxArray, "TipoRapporto", "Tipo Rapporto"), editor: tipoRapporto_DropDownEditor, width: 150
        },
        {
            field: "Ore_Settimanali", title: TraduzioneMultiResx(contattoEditResxArray, "OreSettimanaliAbbr", "Ore Sett."), width: 70
        },
        {
            field: "Classificazione_Des", title: TraduzioneMultiResx(contattoEditResxArray, "Classificazione", "Classificazione"), editor: classificazioneRisUm_DropDownEditor, width: 180
        },
        {
            field: "Info_Famiglia", title: TraduzioneMultiResx(contattoEditResxArray, "InfoFamiglia", "Info Famiglia"), width: "100px"
        },
        {
            field: "Descrizione", title: TraduzioneMultiResx(contattoEditResxArray, "DescrIva", "IVA"), editor: IVA_DropDownEditor, width: 150
        },
        {
            field: "Conto_Descr", title: TraduzioneMultiResx(contattoEditResxArray, "DescrContoEcon", "Conto Economico"), editor: ContoEconomico_DropDownEditor, width: 300
        },
        {
            field: "Descr_Conto_Pat", title: TraduzioneMultiResx(contattoEditResxArray, "DescrContoPat", "Conto Patrimoniale"), editor: ContoPatrimoniale_DropDownEditor, width: 300
        }
    ];

    if (tipoCarica != 1) {
        colonneKendoGrid.unshift(
            {
                command: [{
                    template:
                        "<span class='fa fa-file-text' title='PostIt' onclick=ApriLinkPostit(this.closest('tr'),this.closest('.k-grid'))></span>"
                }], title: "PostIt", width: "60px" //i18n
            }
        );
    }

    var parametriPerLettura = null;
    var parametriDataSource = {};

    //var parametriKendoGrid = {};
    var parametriKendoGrid = {
        //editable: {
        //    mode: "inline"
        //},
        //colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditRapportiContabili, funzioneDaChiamareDopoDataBound: RapportiContabili_DataBound, funzioneDaChiamareDopoDelete: onDeleteRapportoContabile };
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

function grid_cellClose(e) {
    if (e.type == "save") {
        var input = null;
        var gridName = this.element.attr("id");

        switch (gridName) {
            case "griglia_costi":
                griglia_costi_cell_close(e);
                break;
            case "griglia_rapporti_contabili":
            case "griglia_liquidita":
                griglia_rapporti_contabili_cell_close(e);
                break;
        }

    }

}

function griglia_rapporti_contabili_cell_close(e) {
    input = e.container.find("input[name='Validita_Fine']").data("kendoDatePicker");
    if (input != undefined) {
        if (input.value() == "" || input.value() == undefined || input.value() == null)
            e.model.Validita_Fine = new Date("2100/12/31");
    }

    input = e.container.find("input[name='Validita_Inizio']").data("kendoDatePicker");
    if (input != undefined) {
        if (input.value() == "" || input.value() == undefined || input.value() == null)
            e.model.Validita_Inizio = new Date("1900/01/01");
    }

}

function griglia_costi_cell_close(e) {
    input = e.container.find("input[name='FinePrezzo']").data("kendoDatePicker");
    if (input != undefined) {
        if (input.value() == "" || input.value() == undefined || input.value() == null)
            e.model.FinePrezzo = new Date("2100/12/31");
    }

    input = e.container.find("input[name='InizioPrezzo']").data("kendoDatePicker");
    if (input != undefined) {
        if (input.value() == "" || input.value() == undefined || input.value() == null)
            e.model.InizioPrezzo = new Date("1900/01/01");
    }

}

function onDeleteRapportoContabile(e) {
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var row = $(e.target).closest("tr");
    var dataItem = grid.dataItem(row);
    var codRisUm = dataItem.Cod_RisUm;
    if (codRisUm === "") { codRisUm = 0; }

    if (dataItem.deleted === false) {
        var daCancellare = [];
        // Ripristino righe eventualmente cancellate prima
        var gridCosti = $("#griglia_costi").data("kendoGrid");
        var currentData = gridCosti.dataSource._destroyed;
        for (var i = 0; i < currentData.length; i++) {
            var row_Cod_RisUm = currentData[i].Cod_RisUm;
            if (codRisUm === row_Cod_RisUm) {
                currentData[i].deleted = false;
                var rowCosti = gridCosti.tbody.find("tr[data-uid='" + currentData[i].uid + "']");
                rowCosti.removeClass("deletedKendoRow");
            }
            else {
                daCancellare.push(currentData[i]);
            }
        }
        gridCosti.dataSource._destroyed = daCancellare;
    }
    else {
        // Controlla se ci sono movimenti contabili collegati
        var risposta = Check_Movimenti_Collegati_Risorsa_Umana(codRisUm);
        if (risposta != "") {
            kendo.alert(risposta);
            dataItem.deleted = false;
            row.removeClass("deletedKendoRow");
            return false
        }
        // Controlla se la risorsa umana pè assegnata ad una squadra nei cdg
        var risposta = Check_Esistenza_SquadreCdG_Risorsa_Umana(codRisUm);
        if (risposta != "") {
            kendo.alert(risposta);
            dataItem.deleted = false;
            row.removeClass("deletedKendoRow");
            return false
        }

        // Alert sui costi
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: TraduzioneMultiResx(contattoEditResxArray, "Attenzione", "Attenzione"),
            messages: { okText: TraduzioneMultiResx(contattoEditResxArray, "Si", "Sì"), cancel: TraduzioneMultiResx(contattoEditResxArray, "No", "No") },
            content: TraduzioneMultiResx(contattoEditResxArray, "ConfermaEliminazioneRapportoContabile", "Eliminando questo Rapporto Contabile anche i 'Costi' ad esso collegati verranno eliminati. Proseguire?")
        }).data("kendoConfirm");

        kendoConfirm.result.done(function () {
            var gridCosti = $("#griglia_costi").data("kendoGrid");
            var currentData = gridCosti.dataSource.data();
            for (var i = 0; i < currentData.length; i++) {
                var row_Cod_RisUm = currentData[i].Cod_RisUm;
                if (codRisUm === row_Cod_RisUm) {
                    currentData[i].deleted = true;
                    gridCosti.dataSource._destroyed.push(currentData[i]);
                    var rowCosti = gridCosti.tbody.find("tr[data-uid='" + currentData[i].uid + "']");
                    rowCosti.addClass("deletedKendoRow");
                }
            }
        });

        kendoConfirm.result.fail(function () {
            dataItem.deleted = false;
            row.removeClass("deletedKendoRow");;
        });

        kendoConfirm.open();
    }

}

function RapportiContabili_DataBound(e) {
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var currentData = grid.dataSource.data();
    var destroyed = grid.dataSource._destroyed;
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    if (tipoCarica != 1) {
        $("#rapportoPrincipale").hide();
        $("#listaRapportiSelezionati").show();
        $("#rowinforapporticontabili").show();
        PopolaListaRapportiSelezionatiNonAncoraSalvati();
    }
    else {
        if (currentData.length > 1) {
            $("#rapportoPrincipale").hide();
            $("#listaRapportiSelezionati").show();
            $("#rowinforapporticontabili").show();
            PopolaListaRapportiSelezionatiNonAncoraSalvati();
        }
        else {
            $("#rapportoPrincipale").show();
            $("#listaRapportiSelezionati").hide();
            $("#rowinforapporticontabili").hide();
        }
    }

    if (grid.dataSource.total() == 0) {
        $("#ddl_Rapporto_Principale").parent().append('<label id="ddl_Rapporto_Principale-error" class="custom_val error" for="ddl_Rapporto_Principale">'
            + TraduzioneMultiResx(contattoEditResxArray, 'AssegnareAlContattoAlmenoUnRapportoContabile', 'Assegnare al contatto almeno un rapporto contabile')
            + '</label> ');
        $("#ddl_Rapporto_Principale").parent().children(".required").css('border', '1px solid #D41E1A');
        $(".voce_6").show();
        var err_message = "";
        err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
        $('.nav-tabs li.active a').append(err_message);
    }
    else {
        $(".voce_6").hide();
        $("#ddl_Rapporto_Principale").parent().children(".required").css("border", "none");
        $("#ddl_Rapporto_Principale").parent().find("#ddl_Rapporto_Principale-error").remove();
    }

    ResizeColonneRapportiContabili();
}

function KendoGridDataBound(eventArgs) {
    //ResizeColonne();
}

function onShow(e) {
    ResizeColonneRapportiContabili();
}

function ResizeColonneRapportiContabili() {

    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    for (i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].width === undefined) {
            grid.autoFitColumn(i);
        }
    }

}

// Rapporti Contabili
function rapportoContabile_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Rapporto_Des", "Cod_Rapporto", elencoRapportiContabili, changeRapportoContabile);
}

function changeRapportoContabile(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cod_Rapporto = dataItem.Cod_Rapporto;
    model.Rapporto_Des = dataItem.Rapporto_Des;
    model.dirty = true;
    //kendoFastRedrawRow(grid, row);
}

// Qualifiche
function qualifiche_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Qualifica_Des", "Qualifica_Cod", elencoQualifiche, changeQualifica, false);
}

function changeQualifica(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Qualifica_Cod = dataItem.Qualifica_Cod;
    model.Qualifica_Des = dataItem.Qualifica_Des;
    model.dirty = true;
}

// Mansioni
function mansione_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Mansione_Des", "Mansione_Cod", elencoMansioni, changeMansione, false);
}

function changeMansione(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Mansione_Cod = dataItem.Mansione_Cod;
    model.Mansione_Des = dataItem.Mansione_Des;
    model.dirty = true;
}

// Tipi Rapporto
function tipoRapporto_DropDownEditor(container, options) {
    creaDropDownEditor(container, "TipoRapporto_Des", "TipoRapporto_Cod", elencoTipiRapporto, changeTipoRapporto, false);
}

function changeTipoRapporto(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.TipoRapporto_Cod = dataItem.TipoRapporto_Cod;
    model.TipoRapporto_Des = dataItem.TipoRapporto_Des;
    model.dirty = true;
}

// Classificazione Risorse Umane
function classificazioneRisUm_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Classificazione_Des", "Classificazione_Cod", elencoClassificazioniRisUm, changeClassRisUm, false);
}

function changeClassRisUm(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Classificazione_Cod = dataItem.Classificazione_Cod;
    model.Classificazione_Des = dataItem.Classificazione_Des;
    model.dirty = true;
}

function IVA_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Descrizione", "Cod_IVA", elencoIVA_Aliquote, changeIVA, false);
}

function changeIVA(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cod_IVA = dataItem.Cod_IVA;
    model.Descrizione = dataItem.Descrizione;
    model.dirty = true;
}

function ContoEconomico_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Descr_Conto", "Cod_Conto", elencoContoEconomico, changeContoEconomico, false);
}

function changeContoEconomico(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cod_Conto = dataItem.Cod_Conto;
    model.Conto_Descr = dataItem.Descr_Conto;
    model.dirty = true;
}

function ContoPatrimoniale_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Descr_Conto_Pat", "Cod_Conto_Pat", elencoContoPatrimoniale, changeContoPatrimoniale, false);
}

function changeContoPatrimoniale(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cod_Conto_Pat = dataItem.Cod_Conto_Pat;
    model.Descr_Conto_Pat = dataItem.Descr_Conto_Pat;
    model.dirty = true;
}

function onEditRapportiContabili(e) {
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica == 0)
        e.sender.closeCell();
}

function SubmitGrid_RapportiContabili(options) {
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitGrid_RapportiContabili(options.data.created) +
        controllaRigheCompletePerSubmitGrid_RapportiContabili(options.data.updated);


    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "EsisteRigaIncompletaNellaGrigliaProgetti", "Esiste una riga con dati non completi nella griglia Progetti."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(contattoEditResxArray, "EsistonoNRigheIncompleteNellaGrigliaProgetti", "Esistono {0} righe con dati non completi nella griglia Progetti."), nrErr), "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

    errMess = controllaRigheValidePerSubmitGrid_RapportiContabili(options.data.created, "");
    errMess = controllaRigheValidePerSubmitGrid_RapportiContabili(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());

        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Rapporti_Contabili = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Rapporti_Contabili = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_Rapporti_Contabili = kendoEscapeOggetto(deletedRecords);
    }

    righeNonCancellate_Rapporti_Contabili = kendoEscapeOggetto(righeNonCancellate);
}

function controllaRigheCompletePerSubmitGrid_RapportiContabili(righe) {

    var nrErr = 0;
    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return nrErr;
}

function controllaRigheValidePerSubmitGrid_RapportiContabili(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return errMess;
}

// --------------------------------------------------------------------------------------------------------------------------
// FINE GRIGLIA RAPPORTI CONTABILI
// --------------------------------------------------------------------------------------------------------------------------



// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA RUBRICA
// --------------------------------------------------------------------------------------------------------------------------
function PopolaGrigliaRubrica(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    // funzioneSubmit: { funzione: SubmitLottoAssegna, flagInsert: true, flagUpdate: true, flagDelete: true },
    var funzioniCRUD = {
        funzioneRead: Contatto_Rubrica,
        funzioneSubmit: { funzione: SubmitGrid_Rubrica, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };
    var idModel = "Key_Rubrica";
    var campiKendoModel = {
        Key_Rubrica: { editable: false, type: "string" },
        Cod_Rubrica: { editable: false, type: "number" },
        TipoRubrica_Des: { editable: true, type: "string" },
        Numero: { editable: true, type: "string" },
        Descrizione: { editable: true, type: "string", validation: { required: false } },
    };
    var colonneKendoGrid = [
        {
            field: "TipoRubrica_Des", title: TraduzioneMultiResx(contattoEditResxArray, "Tipo", "Tipo"), editor: rubricaTipo_DropDownEditor, daDuplicare: true
        },
        {
            field: "Numero", title: TraduzioneMultiResx(contattoEditResxArray, "TelCellFaxEmailPECecc", "TelCellFaxEmailPECecc"), daDuplicare: true
        },
        {
            field: "Descrizione", title: TraduzioneMultiResx(contattoEditResxArray, "Descrizione", "Descrizione")
        }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};

    //var parametriKendoGrid = {};
    var parametriKendoGrid = {
        //editable: {
        //    mode: "inline"
        //},
        //colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditRubrica, funzioneDaChiamareDopoDataBound: KendoGridDataBound };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];
    //var colonneDisabilitateSoloInModifica = ["Cod_RisUm", "Rag_Soc", "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des", "qualita_cod", "qualita_des", "certif_cod", "certif_des", "validita_inizio", "validita_fine"];
    //var colonneDisabilitateSoloInModifica = ["Cod_RisUm", "Rag_Soc", "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des", "qualita_cod", "qualita_des", "certif_cod", "certif_des"];

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

function onEditRubrica(e) {
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica == 0)
        e.sender.closeCell();
}

function rubricaTipo_DropDownEditor(container, options) {
    creaDropDownEditor(container, "TipoRubrica_Des", "TipoRubrica_Cod", elencoTipiRubrica, changeTipoRubrica);
}

function changeTipoRubrica(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_rubrica").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.TipoRubrica_Cod = dataItem.TipoRubrica_Cod;
    model.TipoRubrica_Des = dataItem.TipoRubrica_Des;

    switch (dataItem.TipoRubrica_Cod) {
        case 0:
            model.Descrizione = TraduzioneMultiResx(contattoEditResxArray, "NumeroTelefonoAbbr", "N. Telefono") + ":";
            break;
        case 1:
            model.Descrizione = TraduzioneMultiResx(contattoEditResxArray, "Fax", "Fax") + ":";
            break;
        case 2:
            model.Descrizione = TraduzioneMultiResx(contattoEditResxArray, "Cellulare", "Cellulare") + ":";
            break;
        case 3:
            model.Descrizione = TraduzioneMultiResx(contattoEditResxArray, "Email", "Email") + ":";
            break;
        case 4:
            model.Descrizione = TraduzioneMultiResx(contattoEditResxArray, "SitoWeb", "Sito Web") + ":";
            break;
        case 5:
            model.Descrizione = TraduzioneMultiResx(contattoEditResxArray, "", "");
            break;

    }
    model.dirty = true;
    kendoFastRedrawRow(grid, row);
}

function SubmitGrid_Rubrica(options) {

    var grid = $("#griglia_rubrica").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitGrid_Rubrica(options.data.created) +
        controllaRigheCompletePerSubmitGrid_Rubrica(options.data.updated);


    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "EsisteRigaIncompletaNellaGrigliaProgetti", "Esiste una riga con dati non completi nella griglia Progetti."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(contattoEditResxArray, "EsistonoNRigheIncompleteNellaGrigliaProgetti", "Esistono {0} righe con dati non completi nella griglia Progetti."), nErr), "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

    errMess = controllaRigheValidePerSubmitGrid_Rubrica(options.data.created, "");
    errMess = controllaRigheValidePerSubmitGrid_Rubrica(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Rubrica = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Rubrica = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_Rubrica = kendoEscapeOggetto(deletedRecords);

    }

}

function controllaRigheCompletePerSubmitGrid_Rubrica(righe) {

    var nrErr = 0;
    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return nrErr;
}

function controllaRigheValidePerSubmitGrid_Rubrica(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return errMess;
}

// --------------------------------------------------------------------------------------------------------------------------
// FINE GRIGLIA RUBRICA
// --------------------------------------------------------------------------------------------------------------------------






// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA CoSTI
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaCosti(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    // funzioneSubmit: { funzione: SubmitLottoAssegna, flagInsert: true, flagUpdate: true, flagDelete: true },
    var funzioniCRUD = {
        funzioneRead: Contatto_Costi,
        funzioneSubmit: { funzione: SubmitGrid_Costi, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };
    var idModel = "Id";
    var campiKendoModel = {
        Id: { editable: false, type: "string" },
        Cod_RisUm: { editable: false, type: "string" },
        Cod_RisUm_Des: { editable: true, type: "string" },
        Udm_Des: { editable: true, type: "string" },
        Prezzo: { editable: true, type: "number" },
        InizioPrezzo: { editable: true, type: "date", defaultValue: new Date("1900/01/01") },
        FinePrezzo: { editable: true, type: "date", defaultValue: new Date("2100/12/31") }

    };
    var colonneKendoGrid = [
        {
            field: "Cod_RisUm_Des", title: TraduzioneMultiResx(contattoEditResxArray, "RapportoContabile", "Rapporto Contabile"), editor: costiRapporto_DropDownEditor, width: 300
        },
        {
            field: "Udm_Des", title: TraduzioneMultiResx(contattoEditResxArray, "Misura", "Misura"), editor: misure_DropDownEditor, width: 100
        },
        {
            field: "Prezzo", title: TraduzioneMultiResx(contattoEditResxArray, "Prezzo", "Prezzo"), width: 100
        },
        {
            field: "InizioPrezzo", title: TraduzioneMultiResx(contattoEditResxArray, "ValiditàInizio", "Validità Inizio"), format: "{0:dd/MM/yyyy}", width: 100
        },
        {
            field: "FinePrezzo", title: TraduzioneMultiResx(contattoEditResxArray, "ValiditàFine", "Validità Fine"), format: "{0:dd/MM/yyyy}", width: 100
        }

    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};

    //var parametriKendoGrid = {};
    var parametriKendoGrid = {
        //editable: {
        //    mode: "inline"
        //},
        //colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditCosti, funzioneDaChiamareDopoDataBound: KendoGridDataBound };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];
    //var colonneDisabilitateSoloInModifica = ["Cod_RisUm", "Rag_Soc", "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des", "qualita_cod", "qualita_des", "certif_cod", "certif_des", "validita_inizio", "validita_fine"];
    //var colonneDisabilitateSoloInModifica = ["Cod_RisUm", "Rag_Soc", "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des", "qualita_cod", "qualita_des", "certif_cod", "certif_des"];

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

function onEditCosti(e) {
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica == 0)
        e.sender.closeCell();
}

function costiRapporto_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Cod_RisUm_Des", "Cod_RisUm", dropDownCostiRapporti, changeCostiRapporto, null);
}
function changeCostiRapporto(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_costi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cod_RisUm = dataItem.Cod_RisUm;
    model.Cod_RisUm_Des = dataItem.Cod_RisUm_Des;
    model.InizioPrezzo = new Date(dataItem.Validita_Inizio);
    model.FinePrezzo = new Date(dataItem.Validita_Fine);
    model.dirty = true;

    kendoFastRedrawRow(grid, row);
}

// Tipi Rapporto
function misure_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Udm_Des", "Udm_Cod", elencoTipiCosto, changeMisure);
}

function changeMisure(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_costi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Udm_Cod = dataItem.Udm_Cod;
    model.Udm_Des = dataItem.Udm_Des;
    model.dirty = true;
}


function SubmitGrid_Costi(options) {

    var grid = $("#griglia_costi").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitGrid_Costi(options.data.created) +
        controllaRigheCompletePerSubmitGrid_Costi(options.data.updated);

    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "EsisteRigaIncompletaNellaGrigliaProgetti", "Esiste una riga con dati non completi nella griglia Progetti."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(contattoEditResxArray, "EsistonoNRigheIncompleteNellaGrigliaProgetti", "Esistono {0} righe con dati non completi nella griglia Progetti."), nErr), "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

    errMess = controllaRigheValidePerSubmitGrid_Costi(options.data.created, "");
    errMess = controllaRigheValidePerSubmitGrid_Costi(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
        var codRisum = 0;
        if (currentData[i].isNew()) {
            codRisum = currentData[i].Cod_RisUm;
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            codRisum = currentData[i].Cod_RisUm;
            updatedRecords.push(currentData[i].toJSON());
        }
        if (codRisum != 0) {
            MarcaRigaRapportoContabileComeDirty(codRisum);
        }

    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        var codRisUm = grid.dataSource._destroyed[i].Cod_RisUm;
        MarcaRigaRapportoContabileComeDirty(codRisUm);
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Costi = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Costi = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_Costi = kendoEscapeOggetto(deletedRecords);
    }

    righeNonCancellate_Costi = kendoEscapeOggetto(righeNonCancellate);
}

function MarcaRigaRapportoContabileComeDirty(codRisum) {
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        if (currentData[i].Cod_RisUm == codRisum) {
            currentData[i].dirty = true;
        }

    }

}

function controllaRigheCompletePerSubmitGrid_Costi(righe) {

    var nrErr = 0;
    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return nrErr;
}

function controllaRigheValidePerSubmitGrid_Costi(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return errMess;
}



// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA INDIRIZZI
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaIndirizzi(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: Contatto_Indirizzi,
        funzioneSubmit: { funzione: SubmitGrid_Indirizzi, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };
    var idModel = "Cod_Indirizzo";
    var campiKendoModel = {
        Cod_Indirizzo: { editable: false, type: "number", default: null },
        Tipo_Indirizzo_Desc: { editable: true, type: "string", default: "" },
        Tipo_Indirizzo: { editable: true, type: "number", default: undefined },
        Via: { editable: true, type: "string", default: "" },
        Provincia_des: { editable: true, type: "string", default: "" },
        Provincia_cod: { editable: true, type: "string", default: "000" },
        Sigla_Prov: { editable: true, type: "string", default: "00" },
        Comune_cod: { editable: true, type: "string", default: "000" },
        Comune_des: { editable: true, type: "string", default: "" },
        Citta_Des: { editable: true, type: "string", default: "" },
        Cap: { editable: true, type: "string", default: "" },
        Frazione: { editable: true, type: "string", default: "" },
        Stato_Des: { editable: true, type: "string", default: "" },
        REG: { editable: false, type: "string", default: "000" },
        Stato: { editable: true, type: "string", default: "" },
        Gestione_Gerarchia_Geografica: { editable: false, type: "number", default: 0 },
        Lingua_Des: { editable: true, type: "string", default: "" },
        Codice_Lingua: { editable: true, type: "string", default: "" },
        Note: { editable: true, type: "string", default: "" }
    };
    var colonneKendoGrid = [
        {
            field: "Stato_Des",
            title: TraduzioneMultiResx(contattoEditResxArray, "Stato", "Stato"),
            editor: Inizializza_Combo_StatoKendo
        },
        {
            field: "Tipo_Indirizzo_Desc",
            title: TraduzioneMultiResx(contattoEditResxArray, "Tipologia", "Tipologia"),
            editor: TipologiaIndirizzo_DropDownEditor
        },
        {
            field: "Via",
            title: TraduzioneMultiResx(contattoEditResxArray, "Indirizzo", "Indirizzo")
        },
        {
            field: "Provincia_des",
            title: TraduzioneMultiResx(contattoEditResxArray, "Provincia", "Provincia"),
            editor: Inizializza_Combo_ProvinceKendo
        },
        {
            field: "Comune_des",
            title: TraduzioneMultiResx(contattoEditResxArray, "Comune", "Comune"),
            editor: Inizializza_Combo_ComuniKendo
        },
        {
            field: "Frazione",
            title: TraduzioneMultiResx(contattoEditResxArray, "Frazione", "Frazione")
        },
        {
            field: "Cap",
            title: TraduzioneMultiResx(contattoEditResxArray, "CAP", "CAP")
        },
        {
            field: "Citta_Des",
            title: TraduzioneMultiResx(contattoEditResxArray, "Città", "Città")
        },
        {
            field: "Lingua_Des",
            title: TraduzioneMultiResx(contattoEditResxArray, "Lingua", "Lingua"),
            editor: Inizializza_Combo_LinguaKendo
        },
        {
            field: "Note",
            title: TraduzioneMultiResx(contattoEditResxArray, "Note", "Note")
        }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    let parametriKendoGrid = {
        pdf: false,
        excel: false,
        //editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        toolbarCommands: ["templateKendoGestioneTipiIndirizzo"]
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditIndirizzi, funzioneDaChiamareDopoDataBound: KendoGridIndirizziDataBound };
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


function SubmitGrid_Indirizzi(options) {

    var grid = $("#grdIndirizzi").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitGrid_Indirizzi(options.data.created) +
        controllaRigheCompletePerSubmitGrid_Indirizzi(options.data.updated);

    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "EsisteRigaIncompletaNellaGrigliaProgetti", "Esiste una riga con dati non completi nella griglia Progetti."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(contattoEditResxArray, "EsistonoNRigheIncompleteNellaGrigliaProgetti", "Esistono {0} righe con dati non completi nella griglia Progetti."), nErr), "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

    errMess = controllaRigheValidePerSubmitGrid_Indirizzi(options.data.created, "");
    errMess = controllaRigheValidePerSubmitGrid_Indirizzi(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
        if (!currentData[i].deleted === true) {
            righeNonCancellate.push(currentData[i].toJSON());
        }
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Indirizzi = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Indirizzi = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_Indirizzi = kendoEscapeOggetto(deletedRecords);
    }

    righeNonCancellateGrid_Indirizzi = kendoEscapeOggetto(righeNonCancellate);

    //// Salvataggio righe //
    //if (righeInseriteGrid_Indirizzi.length > 0 || righeModificateGrid_Indirizzi.length > 0 || righeCancellateGrid_Indirizzi.length > 0 || righeNonCancellate_Indirizzi > 0) {

    //    await InviaRigheModificate(righeInseriteGrid_Indirizzi, righeModificateGrid_Indirizzi, righeCancellateGrid_Indirizzi, righeNonCancellate_Indirizzi);

    //    let grid = $("#grdIndirizzi").data("kendoGrid");
    //    grid.dataSource.read();
    //    grid.refresh();
    //}
}

function controllaRigheCompletePerSubmitGrid_Indirizzi(righe) {

    var nrErr = 0;
    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return nrErr;
}

function controllaRigheValidePerSubmitGrid_Indirizzi(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return errMess;
}

function onEditIndirizzi(e) {

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica == 0) {
        e.sender.closeCell();
        return;
    }

    var fieldName = e.container.find("input").attr("name");
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    var row = grid.tbody.find("tr[data-uid='" + e.model.uid + "']");

    var stato = grid.dataItem(row).Stato.toUpperCase();
    var provincia = grid.dataItem(row).Provincia_des
    if (provincia != undefined && provincia != null)
        provincia = provincia.toUpperCase();


    var GestioneProvince = grid.dataItem(row).Gestione_Gerarchia_Geografica;

    /*if (stato == "IT") {*/
    if (GestioneProvince == 1) {
        switch (fieldName) {
            case "Citta_Des":
                grid.closeCell();
                break;
            case "Comune_cod":
                if (provincia == "" || provincia === null || provincia === undefined || provincia == "NON DEFINITA") {
                    grid.closeCell();
                    alert("Selezionare una Provincia");
                }
                break;
        }

    } else {
        switch (fieldName) {
            case "Provincia_cod":
                grid.closeCell();
                break;
            case "Comune_cod":
                grid.closeCell();
                break;
            case "Frazione":
                grid.closeCell();
                break;
            case "Cap":
                //grid.closeCell();
                break;
        }
    }



}

function KendoGridIndirizziDataBound(e) {
    var CittaIndex = e.sender.wrapper.find(".k-grid-header [data-field=" + "Citta_Des" + "]").index();
    var ComuneIndex = e.sender.wrapper.find(".k-grid-header [data-field=" + "Comune_des" + "]").index();

    var ProvinciaIndex = e.sender.wrapper.find(".k-grid-header [data-field=" + "Provincia_des" + "]").index();

    var FrazioneIndex = e.sender.wrapper.find(".k-grid-header [data-field=" + "Frazione" + "]").index();

    var CapIndex = e.sender.wrapper.find(".k-grid-header [data-field=" + "Cap" + "]").index();


    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);
        if (dataItem.Stato_Des.toLowerCase() == "" || dataItem.Stato_Des.toLowerCase() == undefined || dataItem.Stato_Des.toLowerCase() === null) {
            var cell = row.children().eq(ComuneIndex);
            cell.css("background-color", "Gainsboro");
            cell = row.children().eq(ProvinciaIndex);
            cell.css("background-color", "Gainsboro");
            cell = row.children().eq(FrazioneIndex);
            cell.css("background-color", "Gainsboro");
            cell = row.children().eq(CapIndex);
            cell.css("background-color", "Gainsboro");

            cell = row.children().eq(CittaIndex);
            cell.css("background-color", "Gainsboro");
        }
        else if (dataItem.Gestione_Gerarchia_Geografica == 1) {
            //$('tr[data-uid="' + row.uid + '"] ').css("background-color", "red");
            var cell = row.children().eq(CittaIndex);
            cell.css("background-color", "Gainsboro");
        }
        else if (dataItem.Gestione_Gerarchia_Geografica == 0) {
            var cell = row.children().eq(ComuneIndex);
            cell.css("background-color", "Gainsboro");
            cell = row.children().eq(ProvinciaIndex);
            cell.css("background-color", "Gainsboro");
            cell = row.children().eq(FrazioneIndex);
            cell.css("background-color", "Gainsboro");
            //cell = row.children().eq(CapIndex);
            //cell.css("background-color", "Gainsboro");
        }
    }
}

function TipologiaIndirizzo_DropDownEditor(container, options) {

    var tipo = $(Controls.TipoUtente).find('input:checked').val();
    var cod_contatto = $(Controls.xCodContatto).val();
    CaricaTipologieIndirizziKendo(tipo, cod_contatto);

    creaDropDownEditor(container, "Tipo_Indirizzo_Desc", "Tipo_Indirizzo", dropDownTipologiaIndirizzo, changeTipologiaIndirizzo, null);
}

function changeTipologiaIndirizzo(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grdIndirizzi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Tipo_Indirizzo = dataItem.Tipo_Indirizzo;
    model.Tipo_Indirizzo_Desc = dataItem.Tipo_Indirizzo_Desc;
    model.dirty = true;

    var tipo = ContattoEstero();
    if (tipo) {
        ///*if (model.Stato.toUpperCase() === "IT" || model.Stato.toUpperCase() === "ITALIA") {*/
        //if (model.Gestione_Gerarchia_Geografica == 1) {
        //    if (model.Tipo_Indirizzo === "101" || model.Tipo_Indirizzo === "1" || model.Tipo_Indirizzo === "102" || model.Tipo_Indirizzo === "103") {
        //        kendo.alert(TraduzioneMultiResx(contattoEditResxArray, "StatoItaliaNonValido", "Stato: Italia, non valido"));
        //        model.Stato = "";
        //        model.Stato_Des = "";
        //    }
        //}
        //if (model.Tipo_Indirizzo === "201") {
        //    model.Stato = "IT";
        //    model.Stato_Des = "Italia";
        //}
    }

    kendoFastRedrawRow(grid, row);

    grid.refresh();
}

// --------------------------------------------------------------------------------------------------------------------------
// FINE GRIGLIA INDIRIZZI
// --------------------------------------------------------------------------------------------------------------------------


function AggiornaIndirizziTipo(esci, options) {

    righeInseriteGrid_IndirizzoTipo = "";
    righeModificateGrid_IndirizzoTipo = "";
    righeCancellateGrid_IndirizzoTipo = "";
    righeNonCancellate_IndirizzoTipo = "";

    var griglia = $("#griglia_indirizzi_tipo").data("kendoGrid");
    griglia.saveChanges();

    // Aggiornamento effettivo
    return AggiornaEffettivoIndirizziTipo(esci, options);
}

// SALVATAGGIO
function AggiornaDati(flagEsci, risultato) {

    //Pulizia Variabili

    righeInseriteGrid_Rapporti_Contabili = "";
    righeModificateGrid_Rapporti_Contabili = "";
    righeCancellateGrid_Rapporti_Contabili = "";

    righeInseriteGrid_Costi = "";
    righeModificateGrid_Costi = "";
    righeCancellateGrid_Costi = "";

    righeInseriteGrid_Rubrica = "";
    righeModificateGrid_Rubrica = "";
    righeCancellateGrid_Rubrica = "";

    righeInseriteGrid_Liquidita = "";
    righeModificateGrid_Liquidita = "";
    righeCancellateGrid_Liquidita = "";
    righeNonCancellate_Liquidita = "";

    righeInseriteGrid_Indirizzi = "";
    righeModificateGrid_Indirizzi = "";
    righeCancellateGrid_Indirizzi = "";
    righeNonCancellate_Indirizzi = "";

    var righeInseriteGrid_Conti = "";
    var righeModificateGrid_Conti = "";
    var righeCancellateGrid_Conti = "";
    var righeNonCancellate_Conti = "";

    // Chiamato prima dei rapporti contabili xchè se aggiunto / modificato costo
    // marca come dirty le righe del model dei rapporti contabili
    var griglia_costi = $("#griglia_costi").data("kendoGrid");
    griglia_costi.saveChanges();

    var griglia_rap_Cont = $("#griglia_rapporti_contabili").data("kendoGrid");
    griglia_rap_Cont.saveChanges();

    var griglia_rubrica = $("#griglia_rubrica").data("kendoGrid");
    griglia_rubrica.saveChanges();

    var griglia_indirizzi = $("#grdIndirizzi").data("kendoGrid");
    griglia_indirizzi.saveChanges();

    if (GestisciDatiContabili() == true) {
        var griglia_liquidita = $("#griglia_liquidita").data("kendoGrid");
        griglia_liquidita.saveChanges();

        var griglia_conti = $("#griglia_conti").data("kendoGrid");
        griglia_conti.saveChanges();
    }

    // Aggiornamento effettivo
    return AggiornaEffettivo(flagEsci, risultato);

}

// DA VECCHIA GESTIONE
function pulisci_form_da_validazione() {

    // Azzero tutte le label custom_val
    $(".custom_val").each(function (i, obj) {
        $(this).parent().find('input').css('border', '1px solid #ccc');
        //$(this).parent().children().css('border-color', '#ccc');
        $(this).remove();
    });

    // Resetto la validazione del tab Dati Personali
    $('.nav-tabs li.active a').find('.error-tab').remove();

}

function controlla_form() {

    var flag = true;
    var n_inv = 0;

    var tipo = $(Controls.TipoUtente).find('input:checked').val();
    v.resetForm();
    pulisci_form_da_validazione();

    if (tipo == 0) {

        if ($(Controls.CF).val() == "") {
            $(Controls.CF).parent().append('<label id="Txt_CF-error" class="custom_val error" for="Txt_CF">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $(Controls.CF).parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($(Controls.Cognome).val() == "") {
            $(Controls.Cognome).parent().append('<label id="Txt_Cognome-error" class="custom_val error" for="Txt_Cognome">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $(Controls.Cognome).parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($(Controls.Nome).val() == "") {
            $(Controls.Nome).parent().append('<label id="Txt_Nome-error" class="custom_val error" for="Txt_Nome">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $(Controls.Nome).parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        $("#Txt_Piva-error").hide();
        $("#Txt_Rag_Soc-error").hide();
        $(".voce_4").hide();
        $(".voce_5").hide();
        $(".voce_7").hide();
        $(".voce_8").hide();
        $(".voce_1").show();
        $(".voce_2").show();
        $(".voce_3").show();

    }

    if (tipo == 1) {
        if ($(Controls.PIVA).val() == "") {
            $(Controls.PIVA).parent().append('<label id="Txt_Piva-error" class="custom_val error" for="Txt_Piva">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $(Controls.PIVA).parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($(Controls.RagioneSociale).val() == "") {
            $(Controls.RagioneSociale).parent().append('<label id="Txt_Rag_Soc-error" class="custom_val error" for="Txt_Rag_Soc">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $(Controls.RagioneSociale).parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        $("#Txt_CF-error").hide();
        $("#Txt_Cognome-error").hide();
        $("#Txt_Nome-error").hide();
        $(".voce_1").hide();
        $(".voce_2").hide();
        $(".voce_3").hide();

        // Controllo se italiano o estero
        if (!ContattoEstero()) {
            $(".voce_4").show();
            $(".voce_5").show();
            $(".voce_7").hide();
            $(".voce_8").hide();

        }
        else {
            $(".voce_4").hide();
            $(".voce_5").hide();
            $(".voce_7").show();
            $(".voce_8").show();
        }

    }

    if (v.valid() && flag) {
        return true
    }
    else {
        var err_message = "";
        err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
        $('.nav-tabs li.active a').append(err_message);

        return false;
    }


}

function ValidaxSubmit() {

    var flag = controlla_form();
    if (flag) {

        let risultato = { codContatto: "" };

        // Chiamata a funzioni di salvataggio
        if (AggiornaDati(false, risultato)) {

            kendo.alert(TraduzioneMultiResx(contattoEditResxArray, "SalvataggioEffettuatoCorrettamente", "Salvataggio effettuato correttamente"));

            var apertoDaPopup = $(Controls.AperturaDaPopup).val();
            if (apertoDaPopup === "True") {
                gestisciValore(risultato.codContatto);

                if ($("input[name$='hf_apertodaGiasNG']").val() === "True") {
                    window.parent.postMessage("chiudiWindowGiasNG", ottieniTargetOrigin(window));
                } else {
                    window.close();
                }

            }
            else {
                var paginaRedirect = $(Controls.PaginaRedirect).val();
                var tipoSalva = $(Controls.TipoSalva).val();

                if (tipoSalva == 1) {

                }
                else {
                    if (paginaRedirect !== undefined && paginaRedirect !== "") {
                        RedirectAnagrafica(paginaRedirect);
                        //window.location.href = paginaRedirect;
                    }

                }
            }

        }

    }

}

function valida_indirizzo(stato) {

    var ok = true

    pulisci_form_da_validazione();


    if ($(Controls.Via).val() == "") {
        $(Controls.Via).parent().append('<label id="txt_via-error" class="custom_val error" for="txt_via">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
        $(Controls.Via).parent().children(".required").css('border', '1px solid #D41E1A');
        ok = false;
    }

    var estero = ContattoEstero();

    // Se contatto italiano valido anche Provincia, Comune e CAP altrimenti no
    if (!estero) {

        /*if (stato.toLowerCase() === "it") {*/
        if (stato.Gestione_Gerarchia_Geografica == 1) {
            if ($(Controls.Provincia).val() == "-1") {
                $(Controls.Provincia).parent().append('<label id="ddl_provincia-error" class="custom_val error" for="ddl_provincia">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereSelezionato', 'Il campo deve essere selezionato') + '</label>');
                $(Controls.Provincia).parent().children(".required").css('border', '1px solid #D41E1A');
                ok = false;
            }

            if ($(Controls.Comune).val() == "") {
                $(Controls.Comune).parent().append('<label id="ddl_comune-error" class="custom_val error" for="ddl_comune">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereSelezionato', 'Il campo deve essere selezionato') + '</label>');
                $(Controls.Comune).parent().children(".required").css('border', '1px solid #D41E1A');
                ok = false;
            }
        }

        if ($(Controls.Cap).val() == "") {
            $(Controls.Cap).parent().append('<label id="txt_cap-error" class="custom_val error" for="txt_cap">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $(Controls.Cap).parent().children(".required").css('border', '1px solid #D41E1A');
            ok = false;
        }
    }

    var lingua = KendoDDL("ddl_Lingua").value();
    if (stato.toLowerCase() != "it") {
        if (lingua === "") {
            $("#ddl_Lingua").parent().append('<label id="ddl_lingua-error" class="custom_val error" for="ddl_Lingua">' + TraduzioneMultiResx(contattoEditResxArray, 'IlCampoDeveEssereSelezionato', 'Il campo deve essere selezionato') + '</label>');
            $("#ddl_Lingua").parent().children(".required").css('border', '1px solid #D41E1A');
            ok = false;
        }
    }

    return ok;

}


function generate_CodFisc_click() {
    ; $.ajax({
        type: "POST",
        url: "New_Contatto_Edit.aspx/GetRandomCodFisc",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $(Controls.CF).val(msg.d);
            $(Controls.CF).trigger("keyup");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function generate_piva_click() {
    $.ajax({
        type: "POST",
        url: "New_Contatto_Edit.aspx/GetRandomPiva",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $(Controls.PIVA).val(msg.d);
            $(Controls.PIVA).trigger("keyup");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function aggiungi_costo() {

    var parametri = ""

    parametri += $('#ddl_UdmCod_Prezzo').val() + "|";
    parametri += $('#ddl_UdmCod_Prezzo option:selected').text() + "|";

    parametri += $('#Txt_Inizio_Prezzo').val() + "|";
    parametri += $('#Txt_Fine_Prezzo').val() + "|";
    parametri += $('#Txt_Prezzo').val();



    $.ajax({
        type: 'POST',
        url: 'New_Contatto_Edit.aspx/Aggiungi_Costo',
        data: "{parametri:'" + parametri + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            // Aggiorna la watable indirizzi
            AggiornaTabCosti(JSON.parse(r.d));

        }
    });



}

var kendoDialog_Cambia_PIVA;
var kendoDialog_Cambia_CF;

function AnnullaCambioCF() {
    kendoDialog_Cambia_CF.close();
}

function AnnullaCambiaPIVA() {
    kendoDialog_Cambia_PIVA.close();
}

function cambiaCF() {

}

function cambiaPIVA() {

}

function inserisciCF() {

    if (kendoDialog_Cambia_CF == undefined) {

        kendoDialog_Cambia_CF = $("#window_cambia_CF").kendoDialog({
            width: "400px",
            title: TraduzioneMultiResx(contattoEditResxArray, "CambiaCodiceFiscale", "Cambia Codice Fiscale"),
            closable: true,
            modal: true,
            content: "<row> <div class='col-lg-12 col-md-12 col-xs-12'> <div class='col-lg-6 col-md-6 col-xs-6'> <p>" +
                TraduzioneMultiResx(contattoEditResxArray, "CodiceFiscale", "Codice Fiscale") +
                ":</p> </div> <div class='col-lg-6 col-md-6 col-xs-6'> <input type='text' class='form-control' id='AnnoPratica'> </div> </div> </row>",
            actions: [
                { text: TraduzioneMultiResx(contattoEditResxArray, "Annulla", "Annulla"), action: AnnullaCambioCF },
                { text: TraduzioneMultiResx(contattoEditResxArray, "Conferma", "Conferma"), primary: true, action: cambiaCF }
            ]
        }).data("kendoDialog");
    } else {

        kendoDialog_Cambia_CF.open();

    }


}

function inserisciPiva() {

    if (kendoDialog_Cambia_PIVA == undefined) {

        kendoDialog_Cambia_PIVA = $("#window_cambia_PIVA").kendoDialog({
            width: "400px",
            title: TraduzioneMultiResx(contattoEditResxArray, "CambiaPartitaIva", "Cambia P.IVA"),
            closable: true,
            modal: true,
            content: "<row> <div class='col-lg-12 col-md-12 col-xs-12'> <div class='col-lg-6 col-md-6 col-xs-6'> <p>" +
                TraduzioneMultiResx(contattoEditResxArray, "PartitaIvaAbbr", "P.Iva") +
                ":</p> </div> <div class='col-lg-6 col-md-6 col-xs-6'> <input type='text' class='form-control' id='AnnoPratica'> </div> </div> </row>",
            actions: [
                { text: TraduzioneMultiResx(contattoEditResxArray, "Annulla", "Annulla"), action: AnnullaCambiaPIVA },
                { text: TraduzioneMultiResx(contattoEditResxArray, "Conferma", "Conferma"), primary: true, action: cambiaPIVA }
            ]
        }).data("kendoDialog");

    } else {

        kendoDialog_Cambia_PIVA.open();

    }
}

function EliminaCosto(obj) {
    var id = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'New_Contatto_Edit.aspx/Elimina_Costo',
        data: "{id:'" + id + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            AggiornaTabCosti(JSON.parse(r.d));
        }
    });

}

function CodiceContatto() {
    var tipo = $(Controls.TipoUtente).find('input:checked').val();

    var cod_contatto = "";
    if (tipo == 1) {
        cod_contatto = $(Controls.PIVA).val();
    }
    else {
        cod_contatto = $(Controls.CF).val();
    }

    return cod_contatto;

}

function Id_CF() {
    return $(Controls.IDCF).val();
}


function onChangeTipoContatto(tipoCarica) {

    var returnValue = true;
    var tipo = $(Controls.TipoUtente).find('input:checked').val();
    var cod_contatto = "";
    var x_piva = $(Controls.PivaAzienda).val();
    cod_contatto = $(Controls.xCodContatto).val();

    if (cod_contatto == x_piva) {
        //contatto come impresa speditrice

        // TODO
        //lblCodAccisa.Caption = "Codice accisa Speditore:"
        $("#rowDepFiscale").css("display", "block");
        $("#colCod_UA").css("display", "block");
        $("#colConto_Gar").css("display", "block");
        $("#col_Tipo_Destinazione").css("display", "none");
        $("#col_Orig_Dest").css("display", "block");
        $("#tabConti").css("display", "none");

    }
    else {
        $("#rowDepFiscale").css("display", "none");
        $("#colCod_UA").css("display", "none");
        $("#colConto_Gar").css("display", "none");
        $("#col_Tipo_Destinazione").css("display", "block");
        $("#col_Orig_Dest").css("display", "none");
    }

    pulisci_form_da_validazione();

    if (tipo == 1) {
        $('#datiAzienda').show();
        $('#datiPersona').hide();
        $('#datiPatentino').hide();
    }
    else {
        $('#datiAzienda').hide();
        $('#datiPersona').show();
        $('#datiPatentino').show();
    }

    if (returnValue) {

        Carica_Lista_Rapporti_Contabili();
        var sa_cod = $(Controls.Visibilita).val();
        $(Controls.Visibilita).val(sa_cod);
        // Aggiorno la select Visibilità
        AggiornaSelectVisibilita(tipo);

        // CaricaTipologiaIndirizzo e aggiorna select relativa
        CaricaTipologieIndirizzi(tipo, cod_contatto);

        if ($(Controls.TipoOperazioneContatto).val() == "1") {
            grid = $("#grdIndirizzi").data("kendoGrid");
            grid.dataSource.data().forEach(function (e) {
                e.Tipo_Indirizzo = 0;
                e.Tipo_Indirizzo_Desc = "";
            });
            grid.refresh();
        }
        CaricaTipologieIndirizziKendo(tipo, cod_contatto);

        if (tipoCarica == 1) {
            // Aggiorna Convenevoli
            Inizializza_Combo_Convenevoli();
        }

        controlla_form();
    }

}

function onChangeTipoIndirizzo() {
    var tipoIndirizzo = $(Controls.TipoIndirizzo).val();
    if (ContattoEstero()) {
        // Se tipo = 201 (Stabile Organizzazione) 
        if (tipoIndirizzo === "201") {
            $("#div_prov_com").show();
            SelezionaKendoDropDownItem("ddl_Stato", "IT", "Codice");
            SelezionaKendoDropDownItem("ddl_Lingua", "it", "Codice");
        }
        else {
            $("#div_prov_com").hide();
        }
    }
}

function onChangeProvincia() {

    $(Controls.Comune).parent().children().attr("disabled", false);

    var provincia = $(Controls.Provincia).val();

    if (provincia != "") {
        // ...mostro Comuni relativi
        $.ajax({
            type: 'POST',
            url: 'New_Contatto_Edit.aspx/Carica_Comuni',
            data: "{provincia:'" + provincia + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                $(Controls.Comune).empty();
                $(Controls.Comune).append(r.d[0]);
                $('.selectpicker').selectpicker('refresh');
            }
        });


        // ...salvo codice provincia
        $.ajax({
            type: 'POST',
            url: 'New_Contatto_Edit.aspx/Cambia_provincia_salva_codice',
            data: "{targa:'" + provincia + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {

                $(Controls.ProCodIstat).val(r.d);
                $(Controls.ProvinciaSigla).val(provincia);

            }
        });
    }
    else {
        $(Controls.Comune).empty();
        $(Controls.Comune).parent().children().attr("disabled", true);
        $('.selectpicker').selectpicker('refresh');
    }

}

function onChangeRapportoContabilePrincipale() {
    AggiungiRapportoContabilePrincilapele();
}

function onChangeComune() {

    var provincia = $(Controls.Provincia).val();

    var comune = $(Controls.Comune).find(":selected").val();
    var nome_comune = $(Controls.Comune).find(":selected").text();

    // ...salvo codice provincia
    $.ajax({
        type: 'POST',
        url: 'New_Contatto_Edit.aspx/Cambia_comune_salva_codice',
        data: "{provincia:'" + provincia + "', nome_comune:'" + nome_comune + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            var values = r.d.split('|');
            $(Controls.ComCodIstat).val(values[1]);
        }
    });
}

function onChangeStato(e) {
    var dataItem = e.sender.dataItem();
    var stato = dataItem.Codice.toUpperCase();

    var GestioneProvince = dataItem.Gestione_Gerarchia_Geografica;

    if (GestioneProvince = 1) {
        /*if (stato == "IT") {*/

        if (!ContattoEstero()) {
            $("#div_prov_com").show();
            $("#lbl_frazione").text(TraduzioneMultiResx(contattoEditResxArray, "Frazione", "Frazione"));
        }
        else {
            onChangeTipoIndirizzo();
        }
    }
    else {
        $("#div_prov_com").hide();

        $("#lbl_frazione").text(TraduzioneMultiResx(contattoEditResxArray, "Città", "Città"));
        $(Controls.Frazione).val("");
        $(Controls.Cap).val("");
        $(Controls.Comune).val("");
        $(Controls.ProCodIstat).val("000");
        $(Controls.ComCodIstat).val("000");
        $(Controls.Provincia).val("00");

        var prov = "00";
        $(Controls.Provincia + ' option').each(function () {
            if (prov == $(this).val()) {
                $(this).prop('selected', true);
                $(this).parent().selectpicker('refresh');
            }
        });
        //i18n
        Carica_Comuni(prov, "non definita");
    }
}

function onChangeVisibilita() {
    var sa_cod = $(Controls.Visibilita).val();
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");

    if (grid !== null && grid !== undefined) {
        var currentData = grid.dataSource.data();
        for (var i = 0; i < currentData.length; i++) {
            currentData[i].sa_cod = parseInt(sa_cod);
            currentData[i].dirty = true;
        }
    }

    grid = $("#griglia_rubrica").data("kendoGrid");
    if (grid !== null && grid !== undefined) {
        var currentData = grid.dataSource.data();
        for (var i = 0; i < currentData.length; i++) {
            currentData[i].dirty = true;
        }
    }

}

function Inizializza_Combo_Convenevoli() {

    $("#ddlConvenevoli").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Text",
        dataValueField: "value",
        noDataTemplate: $("#noDataTemplateConvenevoli").html(),
        dataSource: { transport: { read: get_Convenevoli } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Text: "", Value: "" }
    }).data("kendoDropDownList");

}

function Inizializza_Combo_RappFiscale() {

    $("#Ddl_Rappresentante_Fiscale").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Rag_Soc",
        dataValueField: "Cod_RisUm",
        dataSource: { transport: { read: get_RappresentantiFiscali } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Rag_Soc: "", Cod_RisUm: 0 }
    }).data("kendoDropDownList");

}

function Inizializza_Combo_OriginiSpedizione() {

    $("#ddl_Origine_Spedizione").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Codice",
        dataSource: { transport: { read: Leggi_OriginiSpedizione } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Descrizione: "", Codice: 0 }
    }).data("kendoDropDownList");

}

function Inizializza_Combo_TipologieDestinazione() {

    $("#ddl_Cod_Tipo_Destinazione").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Codice",
        dataSource: { transport: { read: Leggi_Tipologie_Destinazione } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Descrizione: "", Codice: 0 }
    }).data("kendoDropDownList");

}

function Inizializza_Combo_UfficiDogane() {
    $("#ddl_Cod_Uff_Dogan").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Codice",
        dataSource: { transport: { read: Leggi_Uffici_Dogane } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Descrizione: "", Codice: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_ItaEste() {
    $("#ddl_ItaEste").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Codice",
        dataSource: { transport: { read: Leggi_ItaEste } },
        open: kendoDropDownAdjustWidth,
    }).data("kendoDropDownList");
}

function Inizializza_Combo_Stato() {
    $("#ddl_Stato").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Codice",
        dataSource: { transport: { read: Leggi_Stati } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Codice: "", Descrizione: TraduzioneMultiResx(contattoEditResxArray, "Seleziona", "Seleziona").toUpperCase() }
    }).data("kendoDropDownList");
}


function Inizializza_Combo_StatoKendo(container, e) {

    if (dropDownStati === null || dropDownStati === undefined)
        Leggi_Statikendo();

    creaDropDownEditor(container, "Stato_Des", "Stato", dropDownStati, changeStati);


}

function changeStati(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grdIndirizzi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Stato = dataItem.Stato;
    model.Stato_Des = dataItem.Stato_Des;
    model.dirty = true;

    model.Provincia_cod = "000";
    model.Sigla_Prov = "00"
    model.Provincia_des = "";
    model.REG = "000";
    dropDownProvincia = null;
    model.Comune_cod = "000";
    model.Comune_des = "";
    dropDownComuni = null;
    model.Frazione = "";
    //model.Indirizzo_Des = null;
    model.Citta_Des = "";
    dropDownCitta = null;
    model.Cap = "";
    model.Gestione_Gerarchia_Geografica = dataItem.Gestione_Gerarchia_Geografica;

    //KendoGridIndirizziDataBound(e);

    grid.refresh();

    // kendoFastRedrawRow(grid, row);
}

function Inizializza_Combo_Rapp_Contabile_Principale() {
    $("#ddl_Rapporto_Principale").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Cod_RisUm_Des",
        dataValueField: "Cod_Rapporto",
        dataSource: {},
        open: kendoDropDownAdjustWidth,
    }).data("kendoDropDownList");
    $("#ddl_Rapporto_Principale").data("kendoDropDownList").bind("change", onChangeRapportoContabilePrincipale);
}

function Inizializza_Combo_Lingua() {
    $("#ddl_Lingua").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Codice",
        dataSource: { transport: { read: Leggi_Lingue } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Codice: "", Descrizione: TraduzioneMultiResx(contattoEditResxArray, "Seleziona", "Seleziona").toUpperCase() }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_LinguaKendo(container) {

    if (dropDownLingua === null || dropDownLingua === undefined)
        Leggi_LinguaKendo();

    creaDropDownEditor(container, "Lingua_Des", "Codice_Lingua", dropDownLingua, changeLingua, false);
}

function changeLingua(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grdIndirizzi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Codice_Lingua = dataItem.Codice_Lingua;
    model.Lingua_Des = dataItem.Lingua_Des;
    model.dirty = true;

    kendoFastRedrawRow(grid, row);

    grid.refresh();
}

function Inizializza_Combo_ProvinceKendo(container) {

    let rowHtml = $(container).parents("tr")[0];
    let grid = $("#grdIndirizzi").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    var stato = row.Stato;


    //if (provincia === null || provincia === undefined)
    //    provincia = "";




    //if (dropDownProvince === null || dropDownProvince === undefined)
    Leggi_ProvinceKendo(stato);

    creaDropDownEditor(container, "Provincia_des", "Provincia_cod", dropDownProvince, changeProvince);
}

function changeProvince(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grdIndirizzi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Provincia_cod = dataItem.Provincia_cod;
    model.Provincia_des = dataItem.Provincia_des;
    model.Sigla_Prov = dataItem.Sigla_Prov;
    model.REG = dataItem.REG;
    model.dirty = true;

    model.Comune_cod = "000";
    model.Comune_des = "";
    dropDownComuni = null;
    model.Frazione = "";
    model.Cap = "";
    grid.refresh();
    //kendoFastRedrawRow(grid, row);
}


function Inizializza_Combo_ComuniKendo(container) {

    let rowHtml = $(container).parents("tr")[0];
    let grid = $("#grdIndirizzi").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    var provincia = row.Provincia_cod;
    var comuni_prov = row.Sigla_Prov;

    if (provincia === null || provincia === undefined)
        provincia = "";

    //if (dropDownComuni === null || dropDownComuni === undefined)
    Leggi_ComuniKendo(provincia, comuni_prov);

    creaDropDownEditor(container, "Comune_des", "Comune_cod", dropDownComuni, changeComuni);
}

function changeComuni(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grdIndirizzi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Comune_cod = dataItem.Comune_cod;
    model.Comune_des = dataItem.Comune_des;
    model.dirty = true;

    // TODO Salvo: inserire codice per modificare CAP

    model.Cap = dataItem.CAP;



    model.Frazione_Des = "";
    kendoFastRedrawRow(grid, row);
}


function Inizializza_Combo_ModalitaPagamento() {
    $("#ddl_modalita_pagamento").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Cau_Pagamento_Sigla",
        dataValueField: "Cau_Pagamento",
        dataSource: { transport: { read: Leggi_ModalitaPagamento } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Cau_Pagamento_Sigla: "", Cau_Pagamento: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_IBAN() {
    $("#ddl_iban_default").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Codice",
        dataSource: { transport: { read: Leggi_Liquidita } },
        open: kendoDropDownAdjustWidth,
        //optionLabel: { Descrizione: "", Codice: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_IVA_Default() {
    $("#ddl_iva_default").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Cod_IVA",
        dataSource: { transport: { read: Leggi_Aliqote_Iva } },
        open: kendoDropDownAdjustWidth
        //optionLabel: { Descrizione: "", Codice: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_ContoEconomico_Default() {
    $("#ddl_contoEco_default").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descr_Conto",
        dataValueField: "Cod_Conto",
        dataSource: { transport: { read: Leggi_Conti_Economici } },
        open: kendoDropDownAdjustWidth
        //optionLabel: { Descrizione: "", Codice: 0 }
    }).data("kendoDropDownList");

}

function Inizializza_Combo_ContoPatrimoniale_Default() {
    $("#ddl_contoPat_default").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descr_Conto_Pat",
        dataValueField: "Cod_Conto_Pat",
        dataSource: { transport: { read: Leggi_Conti_Patrimoniali } },
        open: kendoDropDownAdjustWidth
        //optionLabel: { Descrizione: "", Codice: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_Agente() {
    $("#ddl_agente").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Rag_Soc_Completa",
        dataValueField: "Cod_RisUm",
        dataSource: { transport: { read: Leggi_Agenti } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Rag_Soc_Completa: "", Cod_RisUm: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_referenteConferimento() {
    $("#ddl_referenteConferimento").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Rag_Soc_Completa",
        dataValueField: "Cod_RisUm",
        dataSource: { transport: { read: Leggi_referenteConferimento } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Rag_Soc_Completa: "", Cod_RisUm: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_CapoArea() {
    $("#ddl_capo_area").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Rag_Soc_Completa",
        dataValueField: "Cod_RisUm",
        dataSource: { transport: { read: Leggi_Capi_Area } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Rag_Soc_Completa: "", Cod_RisUm: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_Vettore() {
    $("#ddl_vettore").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Rag_Soc_Completa",
        dataValueField: "Cod_RisUm",
        dataSource: { transport: { read: Leggi_Terzisti } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Rag_Soc_Completa: "", Cod_RisUm: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_IndirizzoFatturazione() {
    $("#ddl_indirizzo_fatturazione").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "IndirizzoTipo_Cod",
        dataSource: { transport: { read: Leggi_Tipi_Indirizzi, data: { Cod_Contatto: CodiceContatto(), Id_CF: Id_CF() } } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Descrizione: "", IndirizzoTipo_Cod: 0 }
    }).data("kendoDropDownList");

}

function Inizializza_Combo_FatturazioneAutomatica() {
    $("#ddl_fatturazione_automatica").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "Codice",
        dataSource: { transport: { read: Leggi_FatturazioneAutomatica } },
        open: kendoDropDownAdjustWidth,
    }).data("kendoDropDownList");
}

function Inizializza_Combo_DocumentoFatturazione() {
    $("#ddl_documento_fatturazione").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "LAV_DES",
        dataValueField: "LAV_COD",
        dataSource: { transport: { read: Leggi_DocumentiFatturazione } },
        open: kendoDropDownAdjustWidth,
    }).data("kendoDropDownList");
}

function Inizializza_Combo_ListinoPrezziAcq() {
    $("#ddl_listino_prezzi_acq").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Listino_Des",
        dataValueField: "Listino_Cod",
        dataSource: { transport: { read: Leggi_Listini, data: { tipo_Classe: 1 } } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Listino_Des: "", Listino_Cod: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_ListinoPrezziVen() {
    $("#ddl_listino_prezzi_ven").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Listino_Des",
        dataValueField: "Listino_Cod",
        dataSource: { transport: { read: Leggi_Listini, data: { tipo_Classe: 2 } } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Listino_Des: "", Listino_Cod: 0 }
    }).data("kendoDropDownList");
}

function Inizializza_Combo_GestioneVettore() {
    $("#ddl_gestione_vettore").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "GestioneVettore_Des",
        dataValueField: "GestioneVettore_Cod",
        dataSource: { transport: { read: Leggi_Gestione_Vettore } },
        open: kendoDropDownAdjustWidth
        //optionLabel: { GestioneVettore_Des: "", CodiGestioneVettore_Cod: 0 }
    }).data("kendoDropDownList");
}

function destinazioneDiversa_change(e) {
    var dataItem = e.sender.dataItem();
    if (dataItem === null || dataItem === undefined)
        return;

    Inizializza_Combo_Indirizzo_Destinazione_Diversa(dataItem.Cod_Contatto, dataItem.Id_CF, true);
}

function Inizializza_Combo_Indirizzo_Destinazione_Diversa(Cod_Contatto, id_CF, SelezionaVuoto) {

    $("#ddl_ind_destinazione_diversa").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Descrizione",
        dataValueField: "IndirizzoTipo_Cod",
        dataSource: { transport: { read: Leggi_Tipi_Indirizzi, data: { Cod_Contatto: Cod_Contatto, Id_CF: id_CF } } },
        open: kendoDropDownAdjustWidth,
        optionLabel: { Descrizione: "", IndirizzoTipo_Cod: 0 }
    }).data("kendoDropDownList");
    if (SelezionaVuoto == true) {
        SelezionaKendoDropDownItem("ddl_ind_destinazione_diversa", 0, "IndirizzoTipo_Cod");
    }

}

function aggiungiNuovoConvenevole(widgetId, value) {


    var widget = KendoDDL(widgetId); //$("#" + widgetId).getKendoDropDownList();
    var dataSource = widget.dataSource;

    if (confirm(TraduzioneMultiResx(contattoEditResxArray, "SeiSicuro", "Sei sicuro?"))) {

        dataSource.add({
            Text: value,
            Value: value
        });

        dataSource.one("sync", function () {
            widget.select(dataSource.view().length - 1);
        });

        dataSource.sync();
        widget.select(function (dataItem) {
            return dataItem.Text === value;
        });

        widget.close();
    }


}



// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA COSTI
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaLiquidita(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    // funzioneSubmit: { funzione: SubmitLottoAssegna, flagInsert: true, flagUpdate: true, flagDelete: true },
    var funzioniCRUD = {
        funzioneRead: Contatto_Liquidita,
        //checkBoxFunction: kEventoSelezionaRiga,
        funzioneSubmit: { funzione: SubmitGrid_Liquidita, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };
    var idModel = "Cod_Liquidita";
    var campiKendoModel = {
        Sa_Cod: { editable: false, type: "number" },
        ChkDefault: { editable: true, type: "boolean" },
        Cod_Liquidita: { editable: false, type: "number" },
        Cod_Contatto: { editable: false, type: "string" },
        Cod_Istituto: { editable: false, type: "number" },
        Istituto_Des: { editable: true, type: "string" },
        Nazione: { editable: true, type: "string" },
        Cifre_Controllo: { editable: true, type: "string" },
        Cin: { editable: true, type: "string" },
        Abi: { editable: true, type: "string" },
        Numero: { editable: true, type: "string" },
        Bic: { editable: true, type: "string" },
        ChkAbilitazione: { editable: false, type: "number", defaultValue: 1 },
        Abilitazione_Des: { editable: true, type: "string", defaultValue: TraduzioneMultiResx(contattoEditResxArray, "PartitaDoppiaPagamentiIncassi", "Partita Doppia/Pagamenti/Incassi") },
        Validita_Inizio: { editable: true, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: true, type: "date", defaultValue: new Date("2100/12/31") },
        Note: { editable: true, type: "string" }

    };

    var colonneKendoGrid = [
        //{
        //    template: '#=dirtyField(data,"ChkDefault")#<input type="checkbox" #= (ChkDefault == true) ? \'checked="checked"\' : "" # class="chkbx" />', title: "Default", width: 110
        //},
        {
            field: "ChkDefault",
            title: "Default",
            template: "#=(ChkDefault ? '" + TraduzioneMultiResx(contattoEditResxArray, "Si", "Si") + "' : '" + TraduzioneMultiResx(contattoEditResxArray, "No", "No") + "')#",
            editor: booleanEditor,
            width: 90,
            attributes: { style: "text-align: center;" }
        },
        {
            field: "Istituto_Des", title: TraduzioneMultiResx(contattoEditResxArray, "IstitutoDiCredito", "Istituto di Credito"), editor: liquidita_istituto_DropDownEditor, width: 300
        },
        {
            field: "Nazione", title: TraduzioneMultiResx(contattoEditResxArray, "Nazione", "Nazione"), width: 80
        },
        {
            field: "Cifre_Controllo", title: TraduzioneMultiResx(contattoEditResxArray, "Cifre", "Cifre"), width: 80
        },
        {
            field: "Cin", title: "Cin", width: 70
        },
        {
            field: "Abi", title: "Abi", width: 70
        },
        {
            field: "Cab", title: "Cab", width: 70
        },
        {
            field: "Numero", title: "Numero C/C", width: 160
        },
        {
            field: "Bic", title: "Bic", width: 70
        },
        {
            field: "Abilitazione_Des", title: TraduzioneMultiResx(contattoEditResxArray, "Abilitazione", "Abilitazione"), editor: liquidita_abilitazione_DropDownEditor, width: 300
        },
        {
            field: "Validita_Inizio", title: TraduzioneMultiResx(contattoEditResxArray, "ValiditàInizio", "Validità Inizio"), format: "{0:dd/MM/yyyy}", width: 100
        },
        {
            field: "Validita_Fine", title: TraduzioneMultiResx(contattoEditResxArray, "ValiditàFine", "Validità Fine"), format: "{0:dd/MM/yyyy}", width: 100
        },
        {
            field: "Note", title: TraduzioneMultiResx(contattoEditResxArray, "Note", "Note"), width: "500px", editor: textAreaEditor
        }

    ];

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditLiquidita, funzioneDaChiamareDopoDataBound: KendoGridDataBound, funzioneDaChiamareDopoDelete: onDeleteLiqudita };
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

function booleanEditor(container, options) {
    var guid = kendo.guid();
    $('<input class="k-checkbox" id="' + guid + '" type="checkbox" name="' + options.field + '" data-type="boolean" data-bind="checked:' + options.field + '">').appendTo(container);
    $('<label class="k-checkbox-label" for="' + guid + '">&#8203;</label>').appendTo(container);
}

function dirtyField(data, fieldName) {
    var hasClass = $("[data-uid=" + data.uid + "]").find(".k-dirty-cell").length < 1;
    if (data.dirty && data.dirtyFields[fieldName] && hasClass) {
        return "<span class='k-dirty'></span>";
    }
    else {
        return "";
    }
}

function onEditLiquidita(e) {
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica == 0)
        e.sender.closeCell();

    e.container.find("input[name='Cin']").attr('maxlength', '1');
    e.container.find("input[name='Nazione']").attr('maxlength', '2');
    e.container.find("input[name='Cifre_Controllo']").attr('maxlength', '2');
    e.container.find("input[name='Abi']").attr('maxlength', '20');
    e.container.find("input[name='Cab']").attr('maxlength', '20');
    e.container.find("input[name='Numero']").attr('maxlength', '50');
    e.container.find("input[name='Bic']").attr('maxlength', '11');
}

function onDeleteLiqudita(e) {
    var grid = $("#griglia_liquidita").data("kendoGrid");
    var row = $(e.target).closest("tr");
    var dataItem = grid.dataItem(row);
    var codLiquidita = dataItem.Cod_Liquidita;
    if (codLiquidita === "") { codLiquidita = 0; }

    if (dataItem.deleted === false) {
    }
    else {

        // Controlla se ci sono movimenti collegati al conto

        var risposta = Check_Movimenti_Collegati_Liquidita(codLiquidita);

        if (risposta != "") {
            kendo.alert(risposta);
            dataItem.deleted = false;
            row.removeClass("deletedKendoRow");
        }
        else {

            var kendoConfirm = $("<div></div>").kendoConfirm({
                title: TraduzioneMultiResx(contattoEditResxArray, "Attenzione", "Attenzione"),
                messages: {
                    okText: TraduzioneMultiResx(contattoEditResxArray, "Si", "Sì"),
                    cancel: TraduzioneMultiResx(contattoEditResxArray, "No", "No")
                },
                content: TraduzioneMultiResx(contattoEditResxArray, "SiDesideraCancellareRisorsaFinanziariaContatto", "Si desidera cancellare la risorsa finanziaria del contatto?")
            }).data("kendoConfirm");
            kendoConfirm.result.done(function () {

            });

            kendoConfirm.result.fail(function () {
                dataItem.deleted = false;
                row.removeClass("deletedKendoRow");
            });

            kendoConfirm.open();
        }
    }
}

function onClickGrigliaLiquidita(e) {


}

function liquidita_istituto_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Istituto_Des", "Cod_Istituto", elencoIstitutiCredito, change_liquidita_istituto, null);
}
function change_liquidita_istituto(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_liquidita").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cod_Istituto = dataItem.Cod_Istituto;
    model.Istituto_Des = dataItem.Istituto_Des;
    model.dirty = true;

    kendoFastRedrawRow(grid, row);
}

function liquidita_abilitazione_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Abilitazione_Des", "ChkAbilitazione", elencoTipiAbilitazioneLiquidita, change_liquidita_abilitazione, null);
}
function change_liquidita_abilitazione(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_liquidita").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.ChkAbilitazione = dataItem.ChkAbilitazione;
    model.Abilitazione_Des = dataItem.Abilitazione_Des;
    model.dirty = true;

    kendoFastRedrawRow(grid, row);
}


function SubmitGrid_Liquidita(options) {
    var grid = $("#griglia_liquidita").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitGrid_Liquidita(options.data.created) +
        controllaRigheCompletePerSubmitGrid_Liquidita(options.data.updated);

    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "EsisteRigaIncompletaNellaGrigliaProgetti", "Esiste una riga con dati non completi nella griglia Progetti."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(contattoEditResxArray, "EsistonoNRigheIncompleteNellaGrigliaProgetti", "Esistono {0} righe con dati non completi nella griglia Progetti."), nErr), "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

    errMess = controllaRigheValidePerSubmitGrid_Liquidita(options.data.created, "");
    errMess = controllaRigheValidePerSubmitGrid_Liquidita(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());

        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Liquidita = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Liquidita = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_Liquidita = kendoEscapeOggetto(deletedRecords);
    }

    righeNonCancellate_Liquidita = kendoEscapeOggetto(righeNonCancellate);

}

function controllaRigheCompletePerSubmitGrid_Liquidita(righe) {

    var nrErr = 0;
    //for (x = 0; x < righe.length; x++) {
    //    item = righe[x];
    //}

    return nrErr;
}

function controllaRigheValidePerSubmitGrid_Liquidita(righe, precMess) {

    var errMess = precMess;

    //for (x = 0; x < righe.length; x++) {
    //    item = righe[x];
    //}

    return errMess;
}

function textAreaEditor(container, options) {
    $('<textarea class="k-textbox" name="' + options.field + '" style="width:100%;height:200px;" />').appendTo(container);
}


function kEventoSelezionaRiga(e) {
    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#griglia_liquidita").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}


function apriGestioneAllegati(url) {
    $(document.body).append('<div id="allegatiWindow"></div>');
    $('#allegatiWindow').kendoWindow({
        title: TraduzioneMultiResx(contattoEditResxArray, "GestioneAllegati", "Gestione Allegati"),
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url + "&fromPatentiniContatto=1",
        close: function () {
            setTimeout(function () {
                $('#allegatiWindow').kendoWindow('destroy');
            }, 200);
        }
    }).data('kendoWindow').center();
}

function chiudiGestioneAllegati() {
    $('#allegatiWindow').data('kendoWindow').close();
}


function apriGestionePostIt(url) {
    $(document.body).append('<div id="postitWindow"></div>');
    $('#postitWindow').kendoWindow({
        title: TraduzioneMultiResx(contattoEditResxArray, "GestionePostIt", "Gestione PostIt"),
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        close: function () {
            setTimeout(function () {
                $('#postitWindow').kendoWindow('destroy');
            }, 200);
        }
    }).data('kendoWindow').center();
}

//function chiudiGestioneAllegati() {
//    $('#postitWindow').data('kendoWindow').close();
//}




// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA CONTI
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaConti(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    // funzioneSubmit: { funzione: SubmitLottoAssegna, flagInsert: true, flagUpdate: true, flagDelete: true },
    var funzioniCRUD = {
        funzioneRead: Contatto_Conti,
        funzioneSubmit: { funzione: SubmitGrid_Conti, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };
    var idModel = "key_conto";
    var campiKendoModel = {
        key_conto: { editable: false, type: "string" },
        Piva: { editable: false, type: "string" },
        Cod_Conto: { editable: false, type: "string" },
        Conto_Descr: { editable: true, type: "string", validation: { required: true } },
        ID_Riclassificazione: { editable: false, type: "string" },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("2100/12/31") },
    };
    var colonneKendoGrid = [
        {
            field: "Cod_Conto", title: "Cod_Conto", width: 50, hidden: true
        },
        {
            field: "ID_Riclassificazione", title: TraduzioneMultiResx(contattoEditResxArray, "Classificazione", "Classificazione"), width: 100
        },
        {
            field: "Conto_Descr", title: TraduzioneMultiResx(contattoEditResxArray, "DescrizioneContoEconomico", "Descrizione Conto Economico"), editor: contoDescr_DropDownEditor, width: 300
        },
        {
            field: "Validita_Inizio", title: TraduzioneMultiResx(contattoEditResxArray, "ValiditàInizio", "Validità Inizio"), format: "{0:dd/MM/yyyy}", width: 100
        },
        {
            field: "Validita_Fine", title: TraduzioneMultiResx(contattoEditResxArray, "ValiditàFine", "Validità Fine"), format: "{0:dd/MM/yyyy}", width: 100
        }
    ];

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditConti, funzioneDaChiamareDopoDataBound: KendoGridDataBound };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["Conto_Descr"];

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

function onEditConti(e) {
    var fieldName = e.container.find("input").attr("name");
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    var row = grid.tbody.find("tr[data-uid='" + e.model.uid + "']");

    if (fieldName === "Cod_Conto") {
        var dataItem = grid.dataItem(row);
        if (!dataItem.isNew()) { grid.closeCell(); }
    }
}

// Rapporti Contabili
function contoDescr_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Conto_Descr", "Cod_Conto", elencoConti, changecontoDescr);
}

function changecontoDescr(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_conti").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cod_Conto = dataItem.Cod_Conto;
    model.Conto_Descr = dataItem.Conto_Descr;
    model.ID_Riclassificazione = dataItem.ID_Riclassificazione;
    model.dirty = true;
    kendoFastRedrawRow(grid, row);
}


function SubmitGrid_Conti(options) {

    var grid = $("#griglia_conti").data("kendoGrid");

    var nrErr = controllaRigheCompletePerSubmitGrid_Conti(options.data.created) +
        controllaRigheCompletePerSubmitGrid_Conti(options.data.updated);


    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "EsisteRigaIncompletaNellaGrigliaProgetti", "Esiste una riga con dati non completi nella griglia Progetti."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(contattoEditResxArray, "EsistonoNRigheIncompleteNellaGrigliaProgetti", "Esistono {0} righe con dati non completi nella griglia Progetti."), nErr), "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

    errMess = controllaRigheValidePerSubmitGrid_Conti(options.data.created, "");
    errMess = controllaRigheValidePerSubmitGrid_Conti(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());

        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Conti = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Conti = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_Conti = kendoEscapeOggetto(deletedRecords);
    }

    righeNonCancellate_Conti = kendoEscapeOggetto(righeNonCancellate);
}

function controllaRigheCompletePerSubmitGrid_Conti(righe) {

    var nrErr = 0;
    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return nrErr;
}

function controllaRigheValidePerSubmitGrid_Conti(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return errMess;
}

function PopolaListaRapportiSelezionati() {
    var listBox = $("#rapportiSelezionati").data("kendoListBox");
    if (listBox !== undefined) {
        listBox.remove(listBox.items());
        listBox.setDataSource({
            data: []
        });
        listBox.destroy();
    }
    $("#rapportiSelezionati").empty();
    var dataSourceListBox = PopolaKendoListRapportiContabiliSelezionati();
    $("#rapportiSelezionati").kendoListBox({
        draggable: false,
        dataTextField: "text",
        dataValueField: "value"
    });
    $("#rapportiSelezionati").data("kendoListBox").setDataSource(new kendo.data.DataSource({
        data: dataSourceListBox
    }));

}

function PopolaListaRapportiSelezionatiNonAncoraSalvati() {
    var rapporti = [];
    var grid = $("#griglia_rapporti_contabili").data("kendoGrid");
    var data = grid.dataSource.data();
    var totalNumber = data.length;

    for (var i = 0; i < totalNumber; i++) {
        var dataItem = data[i];
        var x = new Object();

        var dInizio = dataItem.Validita_Inizio;
        var dFine = dataItem.Validita_Fine;
        x.value = dataItem.Cod_RisUm;
        x.text = kendo.format("{0} - ({1} - {2})",
            dataItem.Rapporto_Des,
            kendo.toString(new Date(dInizio), "dd/MM/yyyy"), kendo.toString(new Date(dFine), "dd/MM/yyyy"));
        rapporti.push(x);
    }

    var listBox = $("#rapportiSelezionati").data("kendoListBox");
    if (listBox !== undefined) {
        listBox.remove(listBox.items());
        listBox.setDataSource({
            data: rapporti
        });
        //listBox.destroy();
    }
    else {
        $("#rapportiSelezionati").kendoListBox({
            draggable: false,
            dataTextField: "text",
            dataValueField: "value"
        });
        $("#rapportiSelezionati").data("kendoListBox").setDataSource(new kendo.data.DataSource({
            data: rapporti
        }));
    }

}

function itaEste_select(e) {
    var indirizziKendoCount = 0;
    let grid = $("#grdIndirizzi").data("kendoGrid");
    if (grid !== null && grid !== undefined)
        indirizziKendoCount = grid.dataSource._data.length;

    if (indirizziKendoCount > 0) {
        var confirmation = confirm(TraduzioneMultiResx(contattoEditResxArray, "ConfermaCambiamentoTipoContatto", "Cambiando il tipo di contatto verranno eliminati gli indirizzi inseriti. Proseguire?"));
        if (!confirmation) {
            e.preventDefault();
            return false;
        }
    }
}

function itaEste_change(e) {

    var dataItem = e.sender.dataItem();
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var apertoDaPopup = $(Controls.AperturaDaPopup).val();

    // 1 Estero
    if (tipoCarica == 1) {
        if (dataItem.Codice === "2") {
            SvuotaIndirizziKendo();
            $(Controls.TipoUtente).find("input[value='1']").prop("checked", true);
            $(Controls.TipoUtente).find("input[value='0']").attr("disabled", "disabled");
        }
        else {
            var idCf = 0;
            if (apertoDaPopup == "True")
                var idCf = $(Controls.IDCF).val();
            SvuotaIndirizziKendo();
            $(Controls.TipoUtente).find("input[value='" + idCf + "']").prop("checked", true);
            $(Controls.TipoUtente).find("input[value='" + idCf + "']").removeAttr("disabled");
        }
    }

    if (dataItem.Codice === "2") {

        $("#lbl_Piva").text("Codice VAT *"); //i18n
        $("#lbl_Rag_Soc").text(TraduzioneMultiResx(contattoEditResxArray, "Intestazione", "Intestazione") + " *");
        $("#row_cf_estero").show();
        //$("#rowLingua").show();
        $("#div_prov_com").hide();
        $("#lbl_frazione").text(TraduzioneMultiResx(contattoEditResxArray, "Localita", "Località"));
    }
    else {

        $("#lbl_Piva").text(TraduzioneMultiResx(contattoEditResxArray, "PartitaIVA", "Partita Iva") + " *");
        $("#lbl_Rag_Soc").text(TraduzioneMultiResx(contattoEditResxArray, "RagioneSociale", "Ragione Sociale") + " *");
        $("#row_cf_estero").show();
        //$("#rowLingua").hide();
        $("#div_prov_com").show();
        $("#lbl_frazione").text(TraduzioneMultiResx(contattoEditResxArray, "Frazione", "Frazione"));
    }

    onChangeTipoContatto($(Controls.TipoOperazioneContatto).val());
    Inizializza_Combo_IndirizzoFatturazione();
}

function ContattoEstero() {
    var ddl = $("#ddl_ItaEste").data("kendoDropDownList");
    if (ddl != null && ddl != undefined) {
        return ddl.value() === "2";
    }
    else {
        return false;
    }
}

function ItalianoEstero() {
    var ddl = $("#ddl_ItaEste").data("kendoDropDownList");
    if (ddl != null && ddl != undefined) {
        return JSON.parse(ddl.value());
    }
    else {
        return 0;
    }
}


function RicalcolaScontoAddizionaleTotale(e) {
    var val1 = 0;
    var val2 = 0;
    var val3 = 0;
    var scontoTotale = 0;

    if (typeof $("#idScontoAdd1").data("kendoNumericTextBox").value() == 'number') {
        val1 = $("#idScontoAdd1").data("kendoNumericTextBox").value();
    }
    if (typeof $("#idScontoAdd2").data("kendoNumericTextBox").value() == 'number') {
        val2 = $("#idScontoAdd2").data("kendoNumericTextBox").value();
    }
    if (typeof $("#idScontoAdd3").data("kendoNumericTextBox").value() == 'number') {
        val3 = $("#idScontoAdd3").data("kendoNumericTextBox").value();
    }

    scontoTotale = (val1 * 100) / 100;
    scontoTotale += ((val2 * (100 - scontoTotale)) / 100)
    scontoTotale += ((val3 * (100 - scontoTotale)) / 100)

    Set_KendoNumTBValue("idScontoAddTot", scontoTotale);

    //TxtScontoAddTotale = Format(Sconto, "##0.00###")
}

function GestisciDatiContabili() {
    // Visibilità della tab non più soggetta all'impostazione SUPERUSER_LIVELLO_GESTIONE_CONTABILITA

    return true;

    /*
    var livGestioneCont = GetPropertyFromJson($(Controls.OpzioniContatti).val(), "SUPERUSER_LIVELLO_GESTIONE_CONTABILITA");
    if (livGestioneCont == "1" || livGestioneCont == "2")
        return true;
    else
        return false;
    */
}

function GestioneTipiIndirizzo() {
    $("#indirizziTipo").show();
    let dialog = $("#nuovoTipoIndirizzoWindow").data("kendoWindow");
    dialog.center().open();

}


// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA INIDIRZZI TIPO
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaIndirizziTipo(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: LeggiIndirizziTipo,
        funzioneSubmit: { funzione: SubmitGrid_IndirizzoTipo, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };
    var idModel = "key_tipo_indirizzo";
    var campiKendoModel = {
        IndirizzoDiSistema: { editable: false, type: "boolean" },
        InidrizzoTipoCod: { editable: false, type: "number" },
        IndirizzoTipoDes: { editable: true, type: "string" },
        ApplicabilitaCod: { editable: true, type: "number", defaultValue: elencoApplicabilitaTipiIndirizzo[0].ApplicabilitaCod },
        ApplicabilitaDes: { editable: true, type: "string", validation: { required: false }, defaultValue: elencoApplicabilitaTipiIndirizzo[0].ApplicabilitaDes },
        Cod_Contatto: { editable: true, type: "string", validation: { required: false } },
        Contatto_Des: { editable: true, type: "string", validation: { required: false } }
    };
    var colonneKendoGrid = [
        {
            field: "IndirizzoTipoDes", title: TraduzioneMultiResx(contattoEditResxArray, "TipoIndirizzoSede", "Tipo Indirizzo/Sede"), width: 200
        },
        {
            field: "ApplicabilitaDes", title: TraduzioneMultiResx(contattoEditResxArray, "Applicabilità", "Applicabilità"), editor: applicabilita_DropDownEditor, width: 140
        },
        {
            field: "Contatto_Des", title: TraduzioneMultiResx(contattoEditResxArray, "Contatto", "Contatto"), width: 200, editor: contattoEditor
        }
    ];

    var parametriPerLettura = null;
    var parametriDataSource = {};

    //var parametriKendoGrid = {};
    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditIndirizzoTipo, funzioneDaChiamareDopoDataBound: InidrizzoTipo_DataBound, funzioneDaChiamareDopoDelete: onDeleteInidrizzoTipo };
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


function contattoEditor(container, options) {
    id_cf_tipi_indirizzo_filtering = options.model.ApplicabilitaCod;

    var ddl = inizializzaComboContattiPerIndirizzoTipo(container, "Contatto_Des", "Cod_Contatto", leggiContattiXIndirizziTipo, change_contattoIndirizzoTipo, 3, "", "", null, options);
    ddl.bind("open", contattoServerFilteringOpen);

}
function contattoServerFilteringOpen(e) {
    var ddl = e.sender;
    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_indirizzi_tipo").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    ddl.unbind("open", contattoServerFilteringOpen);

    if (!model.isNew()) {
        var dataSource = e.sender.dataSource;

        dataSource.add({
            Contatto_Des: model.Contatto_Des,
            Cod_Contatto: model.Cod_Contatto
        });
    }
}

function inizializzaComboContattiPerIndirizzoTipo(container, _dataTextField, _dataValueField, functionRead, functionChange, minLength, defaultValue, defaultText, functionClose, options) {

    var id_cf = options.model.ApplicabilitaCod;

    $('<input required name="' + _dataValueField + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: false,
            autoWidth: true,
            value: defaultValue,
            text: defaultText,
            dataTextField: _dataTextField,
            dataValueField: _dataValueField,
            filter: "contains",
            minLength: minLength,
            open: function (e) {
            },
            change: functionChange,
            close: functionClose,
            dataSource: {
                serverFiltering: true,
                transport: {
                    read: functionRead
                }
            }

        });

    var ddl = $('input[name$="' + _dataValueField + '"]').data("kendoDropDownList");
    return ddl;
}


function change_contattoIndirizzoTipo(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_indirizzi_tipo").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Contatto_Des = dataItem.Contatto_Des;
    model.Cod_Contatto = dataItem.Cod_Contatto;
}

function applicabilita_DropDownEditor(container, options) {

    creaDropDownEditor(container, "ApplicabilitaDes", "ApplicabilitaCod", elencoApplicabilitaTipiIndirizzo, changeApplicabilita);
}

function changeApplicabilita(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_indirizzi_tipo").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.ApplicabilitaCod = dataItem.ApplicabilitaCod;
    model.ApplicabilitaDes = dataItem.ApplicabilitaDes;
    model.Cod_Contatto = "";
    model.Contatto_Des = "";
}

function onDeleteInidrizzoTipo(e) {

    var grid = $("#griglia_indirizzi_tipo").data("kendoGrid");
    var row = $(e.target).closest("tr");
    var dataItem = grid.dataItem(row);

    var tipoIndirizzoCod = dataItem.InidrizzoTipoCod;
    var indirizzoDiSistema = dataItem.IndirizzoDiSistema
    if (tipoIndirizzoCod == "" || tipoIndirizzoCod == undefined) { tipoIndirizzoCod = 0; }
    if (tipoIndirizzoCod == 0)
        return;

    if (indirizzoDiSistema) {
        kendo.alert(TraduzioneMultiResx(contattoEditResxArray, "QuestoTipoDiIndirizzoÈUnPresetGias", "Questo Tipo Indirizzo è un preset GIAS e pertanto non può essere eliminato/modificato"));
        dataItem.deleted = false;
        row.removeClass("deletedKendoRow");
    }

}

function onEditIndirizzoTipo(e) {
    if (e.model.IndirizzoDiSistema) {
        e.sender.closeCell();
    }
}


function InidrizzoTipo_DataBound(e) {

    var grid = $("#griglia_indirizzi_tipo").data("kendoGrid");
    var data = grid.dataSource.data();
    $.each(data, function (i, row) {
        if (row.IndirizzoDiSistema) {
            $('tr[data-uid="' + row.uid + '"] ').css("background-color", "lightgrey");
        } else {
            $('tr[data-uid="' + row.uid + '"] ').css("background-color", "white");
        }
    })

}

function SubmitGrid_IndirizzoTipo(options) {
    var grid = $("#griglia_indirizzi_tipo").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitGrid_InidrizzoTipo(options.data.created) +
        controllaRigheCompletePerSubmitGrid_InidrizzoTipo(options.data.updated);

    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap(
                TraduzioneMultiResx(
                    contattoEditResxArray,
                    "GrigliaTipiIndirizzoRigaConDatiNonCompleti",
                    "Esiste una riga con dati non completi nella griglia Tipi Indirizzo."
                ),
                "DIV_Messaggi"
            );
        else
            MessaggioErrore_Bootstrap(
                kendo.format(
                    TraduzioneMultiResx(
                        contattoEditResxArray,
                        "GrigliaTipiIndirizzoNRigheConDatiNonCompleti",
                        "Esistono {0} righe con dati non completi nella griglia Tipi Indirizzo."
                    ),
                    nrErr
                ),
                "DIV_Messaggi"
            );

        erroreSubmit(grid);
        return;
    }

    var errMess = controllaRigheValidePerSubmitGrid_InidrizzoTipo(options.data.created, "");
    errMess = controllaRigheValidePerSubmitGrid_InidrizzoTipo(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());

        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_IndirizzoTipo = kendoEscapeOggetto(newRecords);
        righeModificateGrid_IndirizzoTipo = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_IndirizzoTipo = kendoEscapeOggetto(deletedRecords);
    }

    righeNonCancellate_IndirizzoTipo = kendoEscapeOggetto(righeNonCancellate);
}

function controllaRigheCompletePerSubmitGrid_InidrizzoTipo(righe) {

    var nrErr = 0;
    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return nrErr;
}

function controllaRigheValidePerSubmitGrid_InidrizzoTipo(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return errMess;
}

function ddlContoEconChange() {         //eventi OnChange delle ddl dei conti. In realtà sono inutili ma forse un giorno serviranno
    CodContoEcon = KendoDDL("ddl_contoEco_default").value();
}

function ddlContoPatChange() {
    CodContoPat = KendoDDL("ddl_contoPat_default").value();
}

function pretty_alert(title, content) {
    $("<div></div>").kendoAlert({
        title: title,
        content: content
    }).data("kendoAlert").open();
}

// #region //ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
function apri_edit_documenti() {
    var piva = $(Controls.PivaAzienda).val();
    var cod_contatto = CodiceContatto();

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    var ok = true

    if (tipoCarica == 1) { //scrittura, bisogna salvare il contatto prima
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: TraduzioneMultiResx(contattoEditResxArray, "Attenzione", "Attenzione"),
            messages: {
                okText: TraduzioneMultiResx(contattoEditResxArray, "Si", "Sì"),
                cancel: TraduzioneMultiResx(contattoEditResxArray, "No", "No")
            },
            content: TraduzioneMultiResx(contattoEditResxArray, "OccorreSalvareIlContattoPerPoterInserirePatentiniEDocumenti", "E' necessario salvare il contatto prima di potere inserire i dati dei patentini e/o altri documenti. Proseguire? ")
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () {
            var flag = controlla_form();
            if (flag) {
                let risultato = { codContatto: -1 };
                if (AggiornaDati(true, risultato)) {
                    var paginaLink = "";
                    var param = kendo.stringify({
                        piva: piva,
                        cod_contatto: risultato.codContatto
                    });

                    ajaxAgronica(indirizzohttp + "/Ottieni_Link_Modifica_Contatto",
                        param,
                        function (risposta) {
                            var apertoDaPopup = $(Controls.AperturaDaPopup).val();
                            if (apertoDaPopup === "True") {
                                gestisciValore(risultato.codContatto);
                            }
                            redirectUrl = risposta.RispostaStringa;
                            window.location.href = redirectUrl;
                        }, null);
                }
            } else {
                ok = false
                kendo.alert(TraduzioneMultiResx(contattoEditResxArray, "InserireIDatiObbligatori", "Inserire i dati obbligatori"));
            }
        });

        kendoConfirm.result.fail(function () {
        });

        kendoConfirm.open();
    } else {
        var url = GetUrlDocAgenda2010(piva, cod_contatto, "Add")
        apriKendoWindowDocumentale(url, "Inserimento Patentino");
    }
}

function apri_ricerca_documenti() {
    var piva = $(Controls.PivaAzienda).val();
    var cod_contatto = CodiceContatto();

    var url = GetUrlDocAgenda2010(piva, cod_contatto, "Read")
    apriKendoWindowDocumentale(url, "Visualizza Patentini");
}

function apriKendoWindowDocumentale(url, title) {

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
            }, 200);
        }
    }).data('kendoWindow').center().maximize();
}
// #endregion