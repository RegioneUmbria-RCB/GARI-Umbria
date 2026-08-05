//--------------------------------------------------------------------------------
// GRIGLIE
//--------------------------------------------------------------------------------

function ConfiguraGrigliaRifCatastali(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === StatoNonLettura();
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === StatoNonLettura();

    var funzioniCRUD = {
        funzioneRead: CaricaGrigliaRifCatastali,
        funzioneSubmit: { funzione: SubmitGrid_RifCatastali, flagInsert: StatoNonLetturaBoolean(), flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };

    var idModel = "Id";
    var campiKendoModel = null;

    campiKendoModel = {
        Id: { editable: true, type: "number" },
        Desc_Particella: { editable: true, type: "string" },
        Iden_Particella: { editable: true, type: "string" },
        COMUNI_PROV: { editable: false, type: "string" },
        LOCALITA: { editable: false, type: "string" },
        PROV: { editable: false, type: "string" },
        COM: { editable: false, type: "string" },
        SEZIONE: { editable: false, type: "string" },
        FOGLIO: { editable: false, type: "number" },
        NUMERO: { editable: false, type: "number" },
        SUBALTERNO: { editable: false, type: "string" },
        Superficie_Catastale: { editable: false, type: "number" },
        REDDITO_DOMINICALE_CLASS: { editable: false, type: "number" },
        REDDITO_AGRARIO_CLASS: { editable: false, type: "number" },
        Cod_Particella: { editable: false, type: "string" },
        Cod_Particella_Id: { editable: false, type: "string" },
        Superficie: { editable: true, type: "number", validation: { required: true } },
        Validita_Inizio: { editable: true, type: "date", validation: { required: true } },
        Validita_Fine: { editable: true, type: "date", validation: { required: true } },
        BioVincolo: { editable: true, type: "boolean" }
    };

    const stileAllineatoCentro = { style: "text-align:center;" };
    const stileAllineatoDestra = { style: "text-align:right;" };
    const formatoData = "{0:dd/MM/yyyy}";
    const formatoSuperficie = "{0:n" + decimaliSuperficie + "}";
    const formatoReddito = "{0:n2}";

    let templateParticella = creaTemplateParticella();

    SeCaricaElencoParticelleRifCatastali(true);

    var colonneKendoGrid = [
        { field: "Desc_Particella", title: nomeColonnaParticella, template: templateParticella, width: 400, editor: Particelle_Editor },
        { field: "Superficie_Catastale", title: nomeColonnaHa(nomeColonnaSuperficieCatastale), format: formatoSuperficie, attributes: stileAllineatoDestra, width: 110 },
        { field: "REDDITO_DOMINICALE_CLASS", title: nomeColonnaRedditoDominicale, format: formatoReddito, attributes: stileAllineatoDestra, width: 110 },
        { field: "REDDITO_AGRARIO_CLASS", title: nomeColonnaRedditoAgrario, format: formatoReddito, attributes: stileAllineatoDestra, width: 110 },
        { field: "Cod_Particella", title: nomeColonnaCodiceParticella, width: 100 },
        { field: "Superficie", title: nomeColonnaHa(nomeColonnaSuperficieAffittata), format: formatoSuperficie, attributes: stileAllineatoDestra, editor: editKendoNumericTextBoxForGridInline, width: 100 },
        { field: "Validita_Inizio", title: nomeColonnaDataInizioAffitto, format: formatoData, attributes: stileAllineatoCentro, width: 150 },
        { field: "Validita_Fine", title: nomeColonnaDataFineAffitto, format: formatoData, attributes: stileAllineatoCentro, width: 150 },
        { field: "BioVincolo", title: nomeColonnaBioVincolo, template: '<input type="checkbox" #= BioVincolo ? \'checked="checked"\' : "" # class="k-checkbox" disabled/>', width: 90 }
    ];

    var colCustKendoGrid = "";

    if (StatoNonLettura() === "True") {
        colCustKendoGrid = [
            {
                command: [
                    {
                        iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: " ", update: " ", cancel: " " }
                    },
                    {
                        iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "destroy", text: ""
                    }
                ],
                title: "Operazioni",
                attributes: stileAllineatoCentro,
                width: 160
            }
        ];
    }
    else {
        colCustKendoGrid = [
            {
                command: [],
                title: "Operazioni"
            }
        ];
    }

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        editable: {
            mode: "inline"
        },
        colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        groupable: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
        toolbarCommands: []
    }; 

    if (StatoNonLettura() === "True") {
        parametriKendoGrid.toolbarCommands.push("template_Kendo_Btn_CreaParticella");
    }

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: DataBoundGrigliaRifCatastali,
        funzioneDaChiamareDopoEdit: EditGrigliaRifCatastali,
        funzioneDaChiamareDopoSave: SaveGrigliaRifCatastali
    };

    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  // funzioni js da chiamare per read, insert, update, delete
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

function nomeColonnaHa(nomeColonna) {

    return nomeColonna + " [ha]"

}

function creaTemplateParticella() {

    let templateParticella = "";

    templateParticella += "Comune: <strong>#: LOCALITA # (#: COMUNI_PROV #)</strong></br>";
    templateParticella += "Sezione: <strong>#: SEZIONE #</strong> - Foglio: <strong>#: FOGLIO #</strong></br>";
    templateParticella += "Numero: <strong>#: NUMERO #</strong> - Subalterno: <strong>#: SUBALTERNO #</strong>";

    return templateParticella;

}
 
function DataBoundGrigliaRifCatastali(e) {
    e.sender.columns.find(colonna => colonna.field = 'Cod_Particella').editable = false;
}

function EditGrigliaRifCatastali(e) {

    if (e.model.isNew()) {

        //----------------------------------------
        // Inserimento
        //----------------------------------------

        // Proposta dati

        e.model.set("Id", 0);
        e.model.set("Validita_Inizio", kendo.parseDate($("#inDataInizVal").val()));
        e.model.set("Validita_Fine", kendo.parseDate($("#inDataFineVal").val()));

    } else {

        //----------------------------------------
        // Modifica
        //----------------------------------------

        // Blocco particella

        let row = e.container;
        let inputs = row.find("input[data-role='multicolumncombobox']");
        let arraySblocco = [];
        let arrayBlocco = ["Iden_Particella"];
        SbloccaColonneRifCatastali(inputs, arraySblocco, arrayBlocco);

        // Blocco codice
        // e.container.find("input[name='Cod_Particella']").first().kendoTextBox({ enable: false });
        
    }

}

function SbloccaColonneRifCatastali(inputs, arrayControlliDaSbloccare, arrayControlliDaBloccare) {

    for (let i = 0; i < inputs.length; i++) {

        let nameC = inputs[i].name;

        let inputComboBox = KendoMultiColumnComboBox(nameC);
        let input = undefined;

        if (inputComboBox != undefined) {
            console.log("KendoMultiColumnComboBox", inputComboBox);
            input = inputComboBox;
        }

        if (input != undefined) {

            if (arrayControlliDaSbloccare.includes(nameC)) {

                input.enable(true);

            } else {

                if (arrayControlliDaBloccare !== null && arrayControlliDaBloccare !== undefined) {

                    if (arrayControlliDaBloccare.includes(nameC)) {

                        input.enable(false);
                    }

                } else {

                    input.enable(false);

                }

            }

        }

    }

}

function SaveGrigliaRifCatastali(e) {    
}

function StatoNonLettura() {
    if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
        return "False";
    } else {
        return "True";
    }
}

function StatoNonLetturaBoolean() {
    if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
        return false;
    } else {
        return true;
    }
}

//--------------------------------------------------------------------------------
// RIFERIMENTI CATASTALI
//--------------------------------------------------------------------------------

function Particelle_Editor(container, options) {

    let colonneComboBox = [
          { field: "LOCALITA", headerTemplate: "<strong>Com.</strong>", width: 250 }
        , { field: "COMUNI_PROV", headerTemplate: "<strong>Prov.</strong>", width: 70 }
        , { field: "SEZIONE", headerTemplate: "<strong>Sez.</strong>", width: 70 }
        , { field: "FOGLIO", headerTemplate: "<strong>Fgl.</strong>", width: 70 }
        , { field: "NUMERO", headerTemplate: "<strong>Num.</strong>", width: 70 }
        , { field: "SUBALTERNO", headerTemplate: "<strong>S.</strong>", width: 70 }
        , { field: "Cod_Particella", headerTemplate: "Codice", width: 90 }
        , { field: "Superficie_Catastale", title: "Sup.Catast.", template: '#= kendo.toString(Superficie_Catastale, "n' + decimaliSuperficie + '") #', width: 90 }
    ];

    SeCaricaElencoParticelleRifCatastali(false);

    creaMultiColumnComboBoxEditor(container, "Desc_Particella", "Iden_Particella", Elenco_Particelle_RifCatastali, change_Particella, colonneComboBox);

}

function creaMultiColumnComboBoxEditor(container, _dataTextField, _dataValueField, _dataSource, functionChange, columns) {

    $('<input name="' + _dataValueField + '"/>')
        .appendTo(container)
        .kendoMultiColumnComboBox({
            autoBind: true,
            dataTextField: _dataTextField,
            dataValueField: _dataValueField,
            dataSource: _dataSource,
            filter: "contains",
            columns: columns,
            change: functionChange
        });

    var multiColumnComboBox = $('input[name$="' + _dataValueField + '"]').data("kendoMultiColumnComboBox");

    return multiColumnComboBox;

}

function change_Particella(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined) {

        let grid = $(tabGrigliaRifCatastali).data("kendoGrid");
        let model = grid.dataItem(this.element.closest("tr"));
        let row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

        // Aggiorno campi chiave
        model.PROV = dataItem.PROV;
        model.COM = dataItem.COM;
        model.SEZIONE = dataItem.SEZIONE;
        model.FOGLIO = dataItem.FOGLIO;
        model.NUMERO = dataItem.NUMERO;
        model.SUBALTERNO = dataItem.SUBALTERNO;

        // Aggiorno codice
        model.Cod_Particella = dataItem.Cod_Particella;

        // Rendo modificabile il codice se non è impostato
        if (model.Cod_Particella === "") {
            model.fields["Cod_Particella"].editable = true;
            model.Cod_Particella_Id = "";
        } else {
            model.fields["Cod_Particella"].editable = false;
            model.Cod_Particella_Id = dataItem.Cod_Particella_Id;
        }

        // Aggiorno superficie catastale
        let superficieCatastale = dataItem.ETTARI + dataItem.ARE / 100 + dataItem.CENTIARE / 10000;
        superficieCatastale = Math.round(superficieCatastale * 10000) / 10000;
        model.Superficie_Catastale = superficieCatastale;

        // Aggiorno reddito dominicale
        model.REDDITO_DOMINICALE_CLASS = dataItem.REDDITO_DOMINICALE_CLASS;

        // Aggiorno reddito agrario
        model.REDDITO_AGRARIO_CLASS = dataItem.REDDITO_AGRARIO_CLASS;

        // Ricarica riga dal model e ritorno in EditMode
        grid.refresh();
        grid.editRow(row);

    }

}

//--------------------------------------------------------------------------------
// SUBMIT GRIGLIE
//--------------------------------------------------------------------------------

function SubmitGrid_RifCatastali(options) {

    var grid = $(tabGrigliaRifCatastali).data("kendoGrid");
    var currentData = grid.dataSource.data();

    // Controllo che tutte le righe CREATE e MODIFICATE siano complete

    var elencoErroriInserimento = controllaRigheNonValidePerSubmitGrid(enum_TipoOperazioneRiga.Inserimento, options.data.created);
    var elencoErroriModifica = controllaRigheNonValidePerSubmitGrid(enum_TipoOperazioneRiga.Inserimento, options.data.updated);

    var nrErr = elencoErroriInserimento.nrErr + elencoErroriModifica.nrErr;

    if (nrErr > 0) {
        let messaggioErrore = elencoErroriInserimento.messErr + elencoErroriModifica.messErr;
        MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi");
        return;
    }

    // Controllo modifiche effettuate

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    for (let i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

    }

    for (let j = 0; j < grid.dataSource._destroyed.length; j++) {
        deletedRecords.push(grid.dataSource._destroyed[j].toJSON());
    }

    // Verifico se sono presenti modifiche e in tal caso relative ad una sola riga

    if (newRecords.length === 0 && updatedRecords.length === 0 && deletedRecords.length === 0) {
        MessaggioErrore_Bootstrap("Non sono state effettuate modifiche sulla riga", "DIV_Messaggi");
        return;
    }

    if (newRecords.length + updatedRecords.length + deletedRecords.length > 1) {
        MessaggioErrore_Bootstrap("Sono presenti modifiche su più righe", "DIV_Messaggi");
        return;
    }

    // Se non ci sono errori procedo con l'aggiornamento

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        rigaInserita = new Object();
        rigaModificata = new Object();
        rigaCancellata = new Object();

        if (newRecords.length > 0) {
            rigaInserita = newRecords[0];
        }

        if (updatedRecords.length > 0) {
            rigaModificata = updatedRecords[0];
        }

        if (deletedRecords.length > 0) {
            rigaCancellata = deletedRecords[0];
        }

        if (AggiornaEffettivoRifCatastali() === false && deletedRecords.length > 0) {
            //In caso di errori in fase di cancellazione di un riferimento catastale, ripristino la riga
            grid.cancelChanges();
        }
        
    }
}

function NumeroRighe(Id_Grid) {

    var n = 0;
    var grid = $(Id_Grid).data("kendoGrid");
    if (grid !== undefined && grid !== null) {
        var currentData = grid.dataSource.data();
        n = currentData.length;        
    }

    return n;
}

function ImpostaTipoOpTestataRifCatastali() {

    if (StatoNonLettura() === "False") {

        tipoOpTestataRifCatastali = enum_TipoOperazioneDB.Lettura.value;

    } else if (NumeroRighe(tabGrigliaRifCatastali) === 0) {

        tipoOpTestataRifCatastali = enum_TipoOperazioneDB.Scrittura.value;

    } else {

        tipoOpTestataRifCatastali = enum_TipoOperazioneDB.Modifica.value;

    }

}

function RichiamaCreaParticella() {

    if ( ContaRigheInseritePendenti() > 0 ) {
        let messaggioErrore = "Prima di creare una nuova particella, confermare o annullare la riga che si sta inserendo.";
        MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi");
        return;
    }

    window.removeEventListener('message', chiudiFinestraCreaParticella);
    window.addEventListener('message', chiudiFinestraCreaParticella);

    var url = LeggiLinkPaginaCatastoEdit();

    $(document.body).append('<div id="pagina_edit_castasto"></div>');

    $('#pagina_edit_castasto').kendoWindow({
        title: "Creazione Nuova Particella Aziendale",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#pagina_edit_castasto').kendoWindow('destroy');
            SeCaricaElencoParticelleRifCatastali(true);
        }
    }).data('kendoWindow').center().maximize();
}

function ContaRigheInseritePendenti() {

    var grid = $(tabGrigliaRifCatastali).data("kendoGrid");

    var currentData = grid.dataSource.data();

    var contaNewRecords = 0;

    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            contaNewRecords += 1;
        }
    }

    return contaNewRecords

}

function SeCaricaElencoParticelleRifCatastali(forzaCaricamento) {

    let caricaElenco = false;

    let idAgenda = parseInt($(cIdAgenda).val())

    let Validita_Testata_Iniz = kendo.parseDate($("#inDataInizVal").val());
    let Validita_Testata_Fine = kendo.parseDate($("#inDataFineVal").val());

    if (idAgenda !== 0 && Validita_Testata_Iniz !== null && Validita_Testata_Fine !== null) {

        if (forzaCaricamento) {

            caricaElenco = true;

        } else {

            if (Elenco_Particelle_RifCatastali.length === 0 ||
                Elenco_Particelle_RifCatastali_Validita_Iniz.toLocaleDateString() !== Validita_Testata_Iniz.toLocaleDateString() ||
                Elenco_Particelle_RifCatastali_Validita_Fine.toLocaleDateString() !== Validita_Testata_Fine.toLocaleDateString()) {

                caricaElenco = true;

            }

        }

    }

    if (caricaElenco) {

        Elenco_Particelle_RifCatastali = Elenco_Particelle_Riempi();

        Elenco_Particelle_RifCatastali_Validita_Iniz = Validita_Testata_Iniz;

        Elenco_Particelle_RifCatastali_Validita_Fine = Validita_Testata_Fine;

    }

    return caricaElenco;

}

function chiudiFinestraCreaParticella(event) {
    let kWin = $('#pagina_edit_castasto').data("kendoWindow");
    let urlKWin = kWin.options.content.url;

    if (verificaOriginSecondaria(window, urlKWin, event) && (typeof event.data == "string") && event.data.includes("chiudiFinestra")) {
        kWin.destroy();
        SeCaricaElencoParticelleRifCatastali(true);
    }
}
