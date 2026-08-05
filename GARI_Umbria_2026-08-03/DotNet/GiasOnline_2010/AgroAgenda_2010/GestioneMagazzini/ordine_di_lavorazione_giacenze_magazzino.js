

function popolaGrigliaGiacenzeMagazzino(IDControllo, _mostraCampiInput) {


    var funzioniCRUD = {
        funzioneRead: kReadGiacenze_rows,
        funzioneUpdate: SubmitInserisciNuoviScarichi,
        //funzioneSubmit: { funzione: SubmitInserisciNuoviScarichi, flagInsert: false, flagUpdate: true, flagDelete: false },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True"
    };
    if (!_mostraCampiInput)
        funzioniCRUD.funzioneUpdate = null;
    var idModel = "chiave_giacenze";
    var campiKendoModel = kReadGiacenze_mod();
    var colonneKendoGrid = kReadGiacenze_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        pagesize: 10,
        aggregate: [
            { field: "NrImballaggi", aggregate: "sum" },
            { field: "NrContenitori", aggregate: "sum" },
            { field: "NrConfezioni", aggregate: "sum" },
            { field: "KgLordi", aggregate: "sum" },
            { field: "TaraTotaleAssoluta", aggregate: "sum" },
            { field: "KgNetti", aggregate: "sum" }
        ]
    }; 

    var parametriKendoGrid = {

        editable: {
            mode: "inline"
        },

        colonneCustomKendoGrid: [
            {
                command: [
                    {
                        iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: "", update: "", cancel: "" }
                    },
                    {
                        iconClass: "fa fa-arrow-down fa-xs", name: "interaRiga", text: "", click: aggiungiInteraRiga, visible: function (dataItem) { return $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True"; }
                    }
                ], title: "Operazioni"/*, locked: true*/ //i18n
            }
        ],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"] },
        columnMenu: true,
        reorderable: true,
        pdf: false
        //,
         //filterable: {
         //    mode: "row"
         //},
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: onDataBoundRigheGiacenza,
        funzioneDaChiamareDopoEdit: onEditGiacenzeMagazzino
    };
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

function aggiungiInteraRiga(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var grid = $("#tab_giacenze").data("kendoGrid");
    //i18n
    kendo.confirm("Vuoi utilizzare l'intera quantità?").then(function () {
        dataItem.NrImballaggi_Mov = dataItem.NrImballaggi;
        dataItem.NrContenitori_Mov = dataItem.NrContenitori;
        dataItem.NrConfezioni_Mov = dataItem.NrConfezioni;
        dataItem.Lordo_Mov = dataItem.KgLordi;
        dataItem.Tara_Mov = dataItem.Tara;
        dataItem.Netto_Mov = dataItem.KgNetti;
        dataItem.Qta_Mov = dataItem.Qta;
        dataItem.dirty = true;
        // grid.dataSource.sync();
        grid.saveChanges();
    }, function () { });
}

function kReadGiacenze_rows(options) {

    let data = $('input[name$="hdKendo_Giacenze"]').val();
    let jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadGiacenze_col() {

    var data = $('input[name$="hdKendo_Giacenze"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    if ($("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True") {
        for (var i = 0; i < jSonParsed_Kendo.kendo_columns.length; i++) {
            if (jSonParsed_Kendo.kendo_columns[i].field.endsWith("_Mov") &&
                (jSonParsed_Kendo.kendo_columns[i].format === "{0:n0}" ||
                    jSonParsed_Kendo.kendo_columns[i].format === "{0:n1}" ||
                    jSonParsed_Kendo.kendo_columns[i].format === "{0:n2}" ||
                    jSonParsed_Kendo.kendo_columns[i].format === "{0:n3}" ||
                    jSonParsed_Kendo.kendo_columns[i].format === "{0:n4}" ||
                    jSonParsed_Kendo.kendo_columns[i].format === "{0:n5}")) {
                jSonParsed_Kendo.kendo_columns[i].editor = editKendoNumericTextBoxForGridInline;
            }
        }
    }

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadGiacenze_mod() {

    var data = $('input[name$="hdKendo_Giacenze"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}

function onDataBoundRigheGiacenza(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    kendo_AggiustaDimensioneColonne("#" + gridId);
    //for (var i = 0; i < grid.columns.length; i++) {
    //    if (grid.columns[i].field !== undefined && grid.columns[i].field == "Referenza")
    //        grid.autoFitColumn(i);
    //}

 
    var wrapperGiacenze = grid.wrapper;
    var headerGiacenze = wrapperGiacenze.find(".k-grid-header");

    function resizeFixedGiacenze() {
        var paddingRight = parseInt(headerGiacenze.css("padding-right"));
        headerGiacenze.css("width", wrapperGiacenze.width() - paddingRight);
        }

    function scrollFixedGiacenze() {
            var offset = $(this).scrollTop(),
                tableOffsetTop = wrapperGiacenze.offset().top,
                tableOffsetBottom = tableOffsetTop + wrapperGiacenze.height() - headerGiacenze.height();
            if (offset < tableOffsetTop || offset > tableOffsetBottom) {
                headerGiacenze.removeClass("fixed-header");
            } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !headerGiacenze.hasClass("fixed")) {
                headerGiacenze.addClass("fixed-header");
            }
        }

    resizeFixedGiacenze();
    $(window).resize(resizeFixedGiacenze);
    $(window).scroll(scrollFixedGiacenze);

    //ResizeColonneGridGiacenze();

}
function ResizeColonneGridGiacenze() {

    var grid = $("#tab_giacenze").data("kendoGrid");
    for (i = 0; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    }

}

function ddlSpecie_change(e) {
    var ddlSpecie = KendoDDL("ddlSpecie");
    var multiselVarieta = KendoMultisel("multiselVarieta");

    multiselVarieta.dataSource.options.transport.data.Veg_Cod = ddlSpecie.value();
    multiselVarieta.dataSource.read();
}

function onEditGiacenzeMagazzino(e) {
    var input = e.container.find(".k-input");
    var value = input.val();
    input.keyup(function () {
        value = input.val();
    });

    var model = e.model; //reference to the model that is about the be edited
    var container = e.container; //reference to the editor container

    let inputs = e.container.find("input[data-role='numerictextbox']");
    let arraySblocco = [];

    let modalitaUdm = getModalitaUdm(model);

    switch (modalitaUdm) {
        case ENUM_MOD_UDM.KG_CONF:

            // Posso scegliere la confezione solo con udm = KG (su db è sempre scritto udm = NR)
            // ==> Sblocco: "Netto_Mov" + "NrConfezioni_Mov" + ("NrContenitori_Mov", "NrImballaggi_Mov") + "Lordo_Mov"
            arraySblocco = ["Netto_Mov", "NrConfezioni_Mov", "Lordo_Mov"];

            break;

        case ENUM_MOD_UDM.KG:

            //Sono nella vecchia situazione: movimento a kg + eventuali contenitori e/o imballi
            // ==> Sblocco: "Netto_Mov" + ("NrContenitori_Mov", "NrImballaggi_Mov") + "Lordo_Mov"
            arraySblocco = ["Netto_Mov", "Lordo_Mov"];

            break;

        case ENUM_MOD_UDM.ALTRA_UDM:
        default:

            //Ho movimentato a numero o altre Udm
            // ==> Sblocco: "Qta_Mov" + ("NrContenitori_Mov", "NrImballaggi_Mov") + "NUOVO CAMPO TARA_TOTALE?!?"
            arraySblocco = ["Qta_Mov", "Tara_Mov"];

            // -- Qta_Mov diventerà di fatto il Peso Netto
    }

    if ((model.NrContenitori !== undefined && model.NrContenitori !== null && model.NrContenitori !== 0) ||
        (model.FF_contenitore_Tipo_Cod !== undefined && model.FF_contenitore_Tipo_Cod !== null && model.FF_contenitore_Tipo_Cod !== 0)) {
        arraySblocco.push("NrContenitori_Mov");
    }

    if ((model.NrImballaggi !== undefined && model.NrImballaggi !== null && model.NrImballaggi !== 0) ||
        (model.FF_imballaggio_Tipo_Cod !== undefined && model.FF_imballaggio_Tipo_Cod !== null && model.FF_imballaggio_Tipo_Cod !== 0)) {
        arraySblocco.push("NrImballaggi_Mov");
    }

    SbloccaControlliPesi(inputs, arraySblocco);

    input.change(function (e) {
        aggiornaQtaGiacenze(e);
    });
}

function aggiornaQtaGiacenze(e) {

    var grid = $("#tab_giacenze").getKendoGrid();
    var row = $(e.target).closest("tr");
    var dataItem = grid.dataItem(row);

    //per ricalcoli
    var NrImbMask = dataItem.NrImballaggi_Mov === undefined ? 0 : dataItem.NrImballaggi_Mov;
    var NrContenitoriMask = dataItem.NrContenitori_Mov === undefined ? 0 : dataItem.NrContenitori_Mov;
    var NrConfezioniMask = dataItem.NrConfezioni_Mov === undefined ? 0 : dataItem.NrConfezioni_Mov;
    var KgLordiMask = dataItem.Lordo_Mov === undefined ? 0.0 : dataItem.Lordo_Mov;
    var KgNettiMask = dataItem.Netto_Mov === undefined ? 0.0 : dataItem.Netto_Mov;
    var TaraMask = dataItem.Tara_Mov === undefined ? 0.0 : dataItem.Tara_Mov;
    var QtaMask = dataItem.Qta_Mov === undefined ? 0.0 : dataItem.Qta_Mov;

    var TaraUnitImballiMask = dataItem.FF_imballaggio_Tara_Campionatura === undefined ? 0.0 : dataItem.FF_imballaggio_Tara_Campionatura;
    var TaraUnitContenitoriMask = dataItem.FF_contenitore_Tara_Campionatura === undefined ? 0.0 : dataItem.FF_contenitore_Tara_Campionatura;
    var TaraUnitConfezioniMask = dataItem.FF_confezione_Tara_Campionatura === undefined ? 0.0 : dataItem.FF_confezione_Tara_Campionatura;

    var CampoModificato = "";
    var campoMod = $(e.target).data().bind;

    if (e.target.value === "")
        e.target.value = 0;

    var ImballoCod = dataItem.FF_imballaggio_Tipo_Cod === undefined ? 0 : TrovaMatCodPerBeneConfezionamento(4, dataItem.FF_imballaggio_Tipo_Cod);
    var ContenitoreCod = dataItem.FF_contenitore_Tipo_Cod === undefined ? 0 : TrovaMatCodPerBeneConfezionamento(8, dataItem.FF_contenitore_Tipo_Cod);
    var ConfezioneCod = dataItem.FF_confezione_Tipo_Cod === undefined ? 0 : TrovaMatCodPerBeneConfezionamento(5, dataItem.FF_confezione_Tipo_Cod);

    var NrImbTotRiferimento = dataItem.NrImballaggi === undefined ? 0 : dataItem.NrImballaggi;
    var NrContenitoriTotRiferimento = dataItem.NrContenitori === undefined ? 0 : dataItem.NrContenitori;
    var NrConfezioniTotRiferimento = dataItem.NrConfezioni === undefined ? 0 : dataItem.NrConfezioni;
    var KgLordiTotRiferimento = dataItem.KgLordi === undefined ? 0.0 : dataItem.KgLordi;
    var KgNettiTotRiferimento = dataItem.KgNetti === undefined ? 0.0 : dataItem.KgNetti;
    var QtaTotRiferimento = dataItem.Qta === undefined ? 0.0 : dataItem.Qta;
    var TaraProdottoTotRiferimento = dataItem.TaraProdotto === undefined ? 0.0 : dataItem.TaraProdotto;
    var TaraAssolutaTotRiferimento = dataItem.TaraTotaleAssoluta === undefined ? 0.0 : dataItem.TaraTotaleAssoluta;

    let modalitaUdm = getModalitaUdm(dataItem);

    switch (modalitaUdm) {
        case ENUM_MOD_UDM.KG_CONF:
        case ENUM_MOD_UDM.KG:

            //sono nella versione movimentazione a Kg (con o senza confezioni)
            
            if (campoMod === "value:NrImballaggi_Mov") {
                CampoModificato = "NrImballaggi";
                NrImbMask = kendo.parseInt(e.target.value);
            }

            if (campoMod === "value:NrContenitori_Mov") {
                CampoModificato = "NrContenitori";
                NrContenitoriMask = kendo.parseInt(e.target.value);
            }

            if (campoMod === "value:NrConfezioni_Mov") {
                CampoModificato = "NrConfezioni";
                NrConfezioniMask = kendo.parseInt(e.target.value);
            }

            if (campoMod === "value:Lordo_Mov") {
                CampoModificato = "KgLordi";
                KgLordiMask = kendo.parseFloat(e.target.value);
            }

            if (campoMod === "value:Netto_Mov") {
                CampoModificato = "KgNetti";
                KgNettiMask = kendo.parseFloat(e.target.value);
            }

            //ByPass per far passare la qta come kg netti
            QtaMask = KgNettiMask;

            if (CampoModificato !== "") {

                let risultato = calcolaQuantitaInCascata(
                    $(cIdPiva).val(), dataItem.Mat_Cod, ImballoCod, ContenitoreCod, ConfezioneCod,
                    // NomeCampoImbMask, NomeCampoContenitoriMask, NomeCampoConfezioniMask, NomeCampoKgLordiMask, NomeCampoKgNettiMask,
                    // NomeCampoTaraImballiMask, NomeCampoTaraContenitoriMask, NomeCampoTaraConfezioniMask,
                    NrImbMask, NrContenitoriMask, NrConfezioniMask, KgLordiMask, KgNettiMask,
                    TaraUnitImballiMask, TaraUnitContenitoriMask, TaraUnitConfezioniMask,
                    NrImbTotRiferimento, NrContenitoriTotRiferimento, NrConfezioniTotRiferimento, KgLordiTotRiferimento, KgNettiTotRiferimento,
                    CampoModificato, false);

                dataItem.NrImballaggi_Mov = risultato.NrImbMask;
                dataItem.NrContenitori_Mov = risultato.NrContenitoriMask;
                dataItem.NrConfezioni_Mov = risultato.NrConfezioniMask;
                dataItem.Lordo_Mov = risultato.KgLordiMask;
                dataItem.Netto_Mov = risultato.KgNettiMask;
                dataItem.Tara_Mov = risultato.KgTaraMask;

                dataItem.Qta_Mov = risultato.KgNettiMask;

                if (KendoNumTB("NrImballaggi_Mov") !== undefined)
                    KendoNumTB("NrImballaggi_Mov").value(risultato.NrImbMask);

                if (KendoNumTB("NrContenitori_Mov") !== undefined)
                    KendoNumTB("NrContenitori_Mov").value(risultato.NrContenitoriMask);

                if (KendoNumTB("NrConfezioni_Mov") !== undefined)
                    KendoNumTB("NrConfezioni_Mov").value(risultato.NrConfezioniMask);

                if (KendoNumTB("Lordo_Mov") !== undefined)
                    KendoNumTB("Lordo_Mov").value(risultato.KgLordiMask);

                if (KendoNumTB("Netto_Mov") !== undefined)
                    KendoNumTB("Netto_Mov").value(risultato.KgNettiMask);

                if (KendoNumTB("Tara_Mov") !== undefined)
                    KendoNumTB("Tara_Mov").value(risultato.KgTaraMask);

                if (KendoNumTB("Qta_Mov") !== undefined)
                    KendoNumTB("Qta_Mov").value(risultato.KgNettiMask);

                dataItem.dirty = true;

            }

            break;

        default:
            //sono nella nuova versione di movimentazione con altra udm

            //TODO: in questo caso non ricalcolo nulla, perché ho il nuovo campo Qta_Mov?!?

            if (campoMod === "value:NrImballaggi_Mov") {
                CampoModificato = "NrImballaggi";
                NrImbMask = kendo.parseInt(e.target.value);
            }

            if (campoMod === "value:NrContenitori_Mov") {
                CampoModificato = "NrContenitori";
                NrContenitoriMask = kendo.parseInt(e.target.value);
            }

            if (campoMod === "value:Tara_Mov") {
                CampoModificato = "Tara";
                TaraMask = kendo.parseFloat(e.target.value);
            }

            //ByPass per far passare la qta come kg netti
            if (campoMod === "value:Qta_Mov") {
                CampoModificato = "Qta";
                QtaMask = kendo.parseFloat(e.target.value);
                KgNettiMask = kendo.parseFloat(e.target.value);
            }

            if (CampoModificato !== "") {

                let risultato = {};
                risultato.QtaMask = QtaMask;
                risultato.NrImbMask = NrImbMask;
                risultato.NrContenitoriMask = NrContenitoriMask;
                risultato.NrConfezioniMask = NrConfezioniMask;
                if (CampoModificato === "Tara") {
                    risultato.KgTaraMask = TaraMask;
                } else {
                    let newTaraProdotto = 0;
                    if (QtaTotRiferimento !== 0) {
                        newTaraProdotto = (TaraProdottoTotRiferimento / QtaTotRiferimento) * QtaMask;
                    }
                    let newTaraImballi = (TaraUnitContenitoriMask * NrContenitoriMask) + (TaraUnitImballiMask * NrImbMask);
                    risultato.KgTaraMask = newTaraProdotto + newTaraImballi;
                }
                risultato.KgNettiMask = QtaMask;
                risultato.KgLordiMask = risultato.KgNettiMask + risultato.KgTaraMask;



                dataItem.NrImballaggi_Mov = risultato.NrImbMask;
                dataItem.NrContenitori_Mov = risultato.NrContenitoriMask;
                dataItem.NrConfezioni_Mov = risultato.NrConfezioniMask;
                dataItem.Lordo_Mov = risultato.KgLordiMask;
                dataItem.Netto_Mov = risultato.KgNettiMask;
                dataItem.Tara_Mov = risultato.KgTaraMask;
                dataItem.Qta_Mov = risultato.QtaMask;
                
                if (KendoNumTB("NrImballaggi_Mov") !== undefined)
                    KendoNumTB("NrImballaggi_Mov").value(risultato.NrImbMask);

                if (KendoNumTB("NrContenitori_Mov") !== undefined)
                    KendoNumTB("NrContenitori_Mov").value(risultato.NrContenitoriMask);

                if (KendoNumTB("NrConfezioni_Mov") !== undefined)
                    KendoNumTB("NrConfezioni_Mov").value(risultato.NrConfezioniMask);

                if (KendoNumTB("Lordo_Mov") !== undefined)
                    KendoNumTB("Lordo_Mov").value(risultato.KgLordiMask);

                if (KendoNumTB("Netto_Mov") !== undefined)
                    KendoNumTB("Netto_Mov").value(risultato.KgNettiMask);

                if (KendoNumTB("Tara_Mov") !== undefined)
                    KendoNumTB("Tara_Mov").value(risultato.KgTaraMask);

                if (KendoNumTB("Qta_Mov") !== undefined)
                    KendoNumTB("Qta_Mov").value(risultato.QtaMask);

                dataItem.dirty = true;

            }

            break;
    }
    
}