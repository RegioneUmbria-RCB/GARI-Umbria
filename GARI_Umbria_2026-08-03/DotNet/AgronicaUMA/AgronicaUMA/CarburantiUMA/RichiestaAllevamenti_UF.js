 
function leggiGrigliaColtureUF(IDControllo) {
    var omettiAnnulla = false;
    var omettiSalva = true;

    var funzioneSubmitDaUsare = {
        funzione: () => { },
        flagInsert: false,
        flagUpdate: false,
        flagDelete: true
    }

    var funzioniCRUD = {
        funzioneRead: LeggiColtureAllevamentiUF,
        funzioneSubmit: funzioneSubmitDaUsare,
        //funzioneInsert: () => { },
        UtenteAbilitatoInserimentoModifica: false,
        UtenteAbilitatoCancellazione: !richiestaRinuncia && (modifica_richiesto),
        omettiPulsantiSalva: omettiSalva,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var styleOut = "vertical-align: middle; text-align: center;";

    var idModel = "ID";
    var campiKendoModel = null;
    var colonneCustomKendoGrid = new Array();
    //if (!richiestaRinuncia && (modifica_richiesto)) {
    //    colonneCustomKendoGrid.push({
    //        command: {
    //            template: "<div class='btn-group-vertical'>" +
    //                "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaRigaColturaAllevamentoUF(this.closest('tr'),this.closest('.k-grid'))>Cancella</div>" +
    //                "</div>"
    //        }, title: "Cancella", width: "97px", headerAttributes: { style: styleOut }
    //    });
    //}
       
    campiKendoModel = {
        ID: { editable: false, type: "number" },
        Richiesta_Cod: { editable: false, type: "number" },
        Macrouso_UMA_Des: { editable: false, type: "String", validation: { required: true } },
        Macrouso_UMA_Cod: { editable: false, type: "string", validation: { required: true } },
        Programmazione_Cod: { editable: false, type: "number", validation: { required: true } },
        Programmazione_Des: { editable: false, type: "string", validation: { required: true } },
        Tipo_Territorio: { editable: false, type: "number", validation: { required: true } },
        TipoTerritorio_Des: { editable: false, type: "string", validation: { required: true } },
        Occupazione_Cod: { editable: false, type: "string", validation: { required: true } },
        Occupazione_Des: { editable: false, type: "string", validation: { required: true } },
        Destinazione_Cod: { editable: false, type: "string", validation: { required: true } },
        Destinazione_Des: { editable: false, type: "string", validation: { required: true } },
        Uso_Cod: { editable: false, type: "string", validation: { required: true } },
        Uso_Des: { editable: false, type: "string", validation: { required: true } },
        Qualita_Cod: { editable: false, type: "string", validation: { required: true } },
        Qualita_Des: { editable: false, type: "string", validation: { required: true } },
        Superf_Fascicolo: { editable: false, type: "number", validation: { required: true } },
        Superf_Calcolo: { editable: !richiestaRinuncia && modifica_richiesto, type: "number", validation: { required: true } },
        validita_Inizio: { editable: false, type: "date"},
        validita_fine: { editable: false, type: "date" },
        Produce_UF: {editable: false, type: "string"}
    };
    
    var footerTemplateStringsupFASC = "#=calcTotaleColonna('" + "Superf_Fascicolo" + "', " + IDControllo + ")#";
    var footerTemplateStringsupCALC = "#=calcTotaleColonna('" + "Superf_Calcolo" + "', " + IDControllo + ")#";
    
    var colonneKendoGrid = [
        {
            field: "Programmazione_Des",
            title: "Fascicolo",
            width: 170,
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true }
        },
        {
            field: "Macrouso_UMA_Des",
            title: TraduzioneMultiResx(gestioneCarbResx, "Macrouso_UMA_Des", "Gruppo colturale U.M.A."),
            width: 200,
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true }
        },
        {
            field: "TipoTerritorio_Des",
            title: TraduzioneMultiResx(gestioneCarbResx, "TipoTerritorio_Des", "Territorio"),
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true }
        },
        {
            field: "Occupazione_Des",
            title: TraduzioneMultiResx(gestioneCarbResx, "Occupazione", "Occupazione"),
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true }
        },
        {
            field: "Destinazione_Des",
            title: TraduzioneMultiResx(gestioneCarbResx, "destinazione", "Destinazione"),
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true }
        },
        {
            field: "Uso_Des",
            title: TraduzioneMultiResx(gestioneCarbResx, "uso", "Uso"),
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true }
        },
        {
            field: "Qualita_Des",
            title: TraduzioneMultiResx(gestioneCarbResx, "qualita", "Qualita"),
            headerAttributes: { style: styleOut },
            filterable: { multi: true, search: true }
        },
        {
            field: "Occupazione_Cod",
            title: TraduzioneMultiResx(gestioneCarbResx, "Occupazione_Cod", "Codice Occupazione del suolo"),
            headerAttributes: { style: styleOut },
            hidden: true,
            filterable: { multi: true, search: true }
        },
        {
            field: "Destinazione_Cod",
            title: TraduzioneMultiResx(gestioneCarbResx, "destinazione_Cod", "Codice Destinazione"),
            headerAttributes: { style: styleOut },
            hidden: true,
            filterable: { multi: true, search: true }
        },
        {
            field: "Uso_Cod",
            title: TraduzioneMultiResx(gestioneCarbResx, "uso_Cod", "Codice Uso"),
            headerAttributes: { style: styleOut },
            hidden: true,
            filterable: { multi: true, search: true }
        },
        {
            field: "Qualita_Cod",
            title: TraduzioneMultiResx(gestioneCarbResx, "qualita_Cod", "Codice Qualita"),
            headerAttributes: { style: styleOut },
            hidden: true,
            filterable: { multi: true, search: true }
        },
        {
            field: "Superf_Fascicolo",
            title: TraduzioneMultiResx(gestioneCarbResx, "sup_FASC", "Superficie Fascicolo (Ha)"),
            headerAttributes: { style: styleOut },
            footerTemplate: footerTemplateStringsupFASC,
            format: "{0:n4}",
            editor: NumberEditorNoSpin4Decimals
        },
        {
            field: "Superf_Calcolo",
            title: TraduzioneMultiResx(gestioneCarbResx, "sup_CALC", "Superficie UF (Ha)"),
            headerAttributes: { style: styleOut },
            footerTemplate: footerTemplateStringsupCALC,
            format: "{0:n4}",
            editor: NumberEditorNoSpin4Decimals
        },
        {
            field: "Produce_UF",
            title: TraduzioneMultiResx(gestioneCarbResx, "Produce_UF", "Produce UF"),
            headerAttributes: { style: styleOut }
        },
    ];
    
    var parametriPerLettura = [];
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
        //funzioneDaChiamarePrimaDelDetailInit: detailInitGrigliaDettagliLavorazioni,
        //funzioneDaChiamareDopoDataBound: App_onDataBoundImpianti,
        //funzioneDaChiamareDopoEdit: onEditGrigliaAllevamentiUF
        //funzioneDaChiamarePrimaDiEdit: onBeforeEditGrigliaImpianti,
        //funzioneDaChiamarePrimaDelDataBinding: funzioneDaChiamarePrimaDelDataBinding
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
    grid.bind("cellClose", onEditGrigliaAllevamentiUF);
    
}

function onEditGrigliaAllevamentiUF(e) {
    if (e.model.Superf_Calcolo > e.model.Superf_Fascicolo || e.model.Superf_Calcolo < 0) {
        e.model.Superf_Calcolo = e.model.Superf_Fascicolo;
    } 
}

function eliminaRigaColturaAllevamentoUF(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    kendo.confirm("Sei sicuro di voler eliminare l'elemento selezionato?")
        .done(() => {
            datiGriglia.dataSource.remove(datiRiga);
            //datiGriglia.dataSource.sync();
            datiGriglia.dataSource.read();
            datiGriglia.refresh();
        })
        .fail(() => { return; });

}

async function SalvaCancellazioniEModificheAllevamentiUF() {
    var grid = $("#tab_griglia_UFcolture").data("kendoGrid")
    var data = grid.dataSource.data();

    let modificati = data.filter((el) => { return el.dirty == true && (el.deleted === undefined && el.cancellato === undefined) });

    let destroyed = data.filter((el) => { return el.deleted == true || el.cancellato == true });

    if (destroyed.length > 0 || modificati.length > 0) {
        await cancellaModificheRigheAllevamentiUF(destroyed, modificati);
        grid.dataSource.read();
        grid.refresh();
    }
}

function eliminaTuttoUF() {
    var grid = $("#tab_griglia_UFcolture").data("kendoGrid");
    var data = grid._data;
    kendo.confirm("Sei sicuro di voler eliminare tutte le righe? (Questa operazione salverà la pratica, prestare attenzione ad eventuali modifiche in corso in altre sezioni)")
        .done(() => {
            for (var i = 0; i < data.length; i++) {
                grid._data[i].deleted = true;
            }
            $("#btn_salva").click()
        })
        .fail(() => { return; });
}

function attivaDisattivaCalcoloCapiAllevabili(enable) {
    if (QS_Type == 0) {
        if (enable) {
            $("#btn_aggiungi_colture").show();
            $("#btn_elimina_colture").show();
            $("#ddlFascicolo_UF").removeAttr("disabled");
            $("#montagnaSwitchCheck").data("kendoSwitch").enable(true)
        } else {
            $("#btn_aggiungi_colture").hide();
            $("#btn_elimina_colture").hide();
            $("#ddlFascicolo_UF").attr("disabled", "disabled");
            $("#montagnaSwitchCheck").data("kendoSwitch").enable(false)
        }
    }
}