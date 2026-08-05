let imprese = null;
let contiEsistenti = null;
let anniOpzioni = null;
let tipoCarburanti = null;
let tipoDocumenti = null;
let grid = null;
let dettagliLtAcquistabiliWindow = null;
let detailsContoTerzoInput = null;
let detailsContoProprioInput = null;
let modelPrecedente = null;
let usaNuovaVisibilita = true;

$("#btn_carica_elenco").click(
    async function () {
        popolaGrigliaCarburantiVendita();
    }
);


function popolaGrigliaCarburantiVendita() {
    let utenteAbilitatoScrittura = verificaUtenteAbilitatoScrittura();

    let funzioniCRUD = {
        funzioneRead: CaricaCarburantiVenditaDaDB,
        funzioneSubmit: { funzione: SubmitGrid_Vendite, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: utenteAbilitatoScrittura,
        UtenteAbilitatoCancellazione: utenteAbilitatoScrittura,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    let colonna_editabile = utenteAbilitatoScrittura;

    let IDControllo = "grdVenditeUMACarburanti";
    let campiKendoModel = CaricaCampiKendoModel(colonna_editabile);
    let colonneKendoGrid = CaricaColonneKendoGrid(IDControllo);

    let parametriPerLettura = null;
    let parametriDataSource = {
    };

    var colonneCustomKendoGrid = new Array();

    colonneCustomKendoGrid.push({
        command: {
            template: "<div class='btn btn-info btnInfo btnDettaglio' style='width:25px;border:0px;' onclick=DettaglioLtAcquistabili(this.closest('tr'),this.closest('.k-grid'))><span class='fa fa-list lampeggiante'></span></div>"
        },
        title: "Det.", width: "54px", headerAttributes: { style: "vertical-align: middle; text-align: center;" }
    });

    let parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };

    let mostraRigheCancellate = true;
    let colonneDisabilitateSoloInModifica = null;

    let funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: dopoEdit,
        funzioneDaChiamarePrimaDelDataBound: null,
        funzioneDaChiamareDopoDataBound: onGridDataBound,
        funzioneDaChiamareDopoAnnulla: HideMessaggioErrori
    };


    let idModel = "Id_Vendite";
    creaKendoGrid(IDControllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    grid = $("#" + IDControllo).data("kendoGrid");
    grid.bind("cellClose", grid_onCellClose);
    grid.bind("edit", beforeEdit);
}

function dopoEdit(e) {
    if (e.model.Richiesta_Cod > 0) {
        e.sender.closeCell();
    }
}

function HideMessaggioErrori() {
    setTimeout(function () { $("#DIV_Messaggi").hide(); }, 0);
}

async function CaricaCarburantiVenditaDaDB(options) {
    let anno = isNaN(parseInt($('#anno')[0].value)) ? 0 : parseInt($('#anno')[0].value);

    if (parseInt(anno) > 1900 && parseInt(anno) < 2100) {
        setYear(parseInt(anno));
    }

    let piva = KendoDDL("ddlAzienda").value();
    let params = { anno: anno, piva: piva, nuovaVisibilita: usaNuovaVisibilita };
    WaitFrame.show();
    let carburanti = await EseguireChiamataAjax("/CaricaCarburantiVenditaDaDB", params);
    WaitFrame.hide();
    if (carburanti.length > 0)
        options.success(carburanti);

    $.logThis("Sono stati trovati " + carburanti.length + " carburanti.");
}

async function SubmitGrid_Vendite(options) {
    $("#DIV_Messaggi").hide();
    let updatedRecords = [];
    let newRecords = [];
    let deletedRecords = [];

    let currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {
        let risp = await EseguireChiamataAjaxSenzaMessaggioErrore("/SalvaRigheModificate", {
            righeDaAggiornare: {
                RigheInserite: newRecords == null ? JSON.stringify({}) : JSON.stringify(newRecords),
                RigheModificate: updatedRecords == null ? JSON.stringify({}) : JSON.stringify(updatedRecords),
                RigheCancellate: deletedRecords == null ? JSON.stringify({}) : JSON.stringify(deletedRecords)
            }
        });

        if (risp.RispostaConferma) {
            MostraMessaggioOK(TraduciVenditeCarburanti("OperazioneEfettuataCorettamente", "Operazione effettuata correttamente"));
            grid.dataSource.read();
            grid.refresh();
        }
        else {
            MostraMessaggioErrore(risp.RispostaStringa);
        }
    }
}
function MostraMessaggioOK(messaggio) {
    MessaggioTuttoOK_Bootstrap(messaggio, "DIV_Messaggi");
    $("#DIV_Messaggi").show()
}

function MostraMessaggioErrore(messaggio) {
    MessaggioErrore_Bootstrap(messaggio, "DIV_Messaggi");
    $("#DIV_Messaggi").show()
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
                if (val != "" && val != "-1") {
                    var a = KendoDDL("ddlAzienda").dataSource._pristineData.find((el) =>
                        el.piva == val
                    );
                    options.success(KendoDDL("ddlAzienda").dataSource._pristineData.indexOf(a));
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
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "area": "UMA" });
        pathCaricaCmb = "Anagrafica/Imprese.asmx/Carica_Cmb_Imprese_Area";
    }
    else {
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
        pathCaricaCmb = "Anagrafica/Imprese.asmx/Carica_Cmb_Imprese";
    }
    ajaxAgronicaSync(pathCoreWS + pathCaricaCmb,
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "piva": "", "rag_soc": "..." };
            /*if (QS_Piva != "") {
                let tmp = risp.filter(x => x.piva == QS_Piva)[0];
                risp.splice(risp.indexOf(tmp), 1);
                risp.unshift(tmp);
            }*/
            risp.unshift(objVuoto);
            //options.success(risp.slice(0,100));
            options.success(risp);
            //resolve(risp);
        }, null);

    //});
}


function ddlAzienda_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlAzienda.onchange(); //forzo l'evento di onchange
    }
}


function verificaUtenteAbilitatoScrittura() {
    return $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
}

function onGridDataBound(e) {
    autoFitAllColumns(e);
    verificaNoteObbligatorie(e);
}

function autoFitAllColumns(e) {
    for (i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].width === undefined) {
            grid.autoFitColumn(i);
        }
    }
    let data = grid.dataSource.data();
    for (let j = 0; j < data.length; ++j)
        if (data[j].LtInPrecedenza == null)
            data[j].LtInPrecedenza = data[j].Lt;

    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        if (e.sender.dataItem(row).Richiesta_Cod > 0) {
            $(row).find(".btn-Cancella").hide();
        }
    }
}

function verificaNoteObbligatorie(e) {
    var msgNoteObbligatorie = false;    
    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        if (dataItem.Data_Documento != null && dataItem.Data_Documento != "" && dataItem.Data_Creazione != null && dataItem.Data_Creazione != "") {
            var dataDocumento = dataItem.Data_Documento.getTime();
            var dataCreazione = dataItem.Data_Creazione.setHours(0, 0, 0, 0);
            if (dataDocumento < dataCreazione) {
                for (var cell = 0; cell < row[0].children.length; cell++) {
                    row[0].children[cell].style.backgroundColor = "#DC143C"
                }
                msgNoteObbligatorie = true;
            }
        }        
    }

    if (msgNoteObbligatorie) {
        $(".warningInfo").show();
        $(".warningInfo").html(TraduciVenditeCarburanti("NoteObbligatoriePerDataDocumentoAntecedenteCreazione", msgNoteObbligatoriePerDataDocumento) + "<br/>");
    }
    else {
        $(".warningInfo").hide();
        $(".warningInfo").html("");
    }
}

async function TipoDocumento_CreaDropdownList(container, options) {
    if (options.model.Richiesta_Cod > 0) {
        grid.closeCell();
    } else {
        if (tipoDocumenti === null)
            tipoDocumenti = await EseguireChiamataAjax("/CaricaTipoDocumentiDDL");
        creaDropDownEditor(container, "Tipo_Documento_Des", "Tipo_Documento_Cod", tipoDocumenti, onChange_TipoDocumenti);
    }
}

function onChange_TipoDocumenti(e) {
    let dataItem = e.sender.dataItem();
    let model = getModelFromGrid(this);

    if (model != null) {
        model.set("Tipo_Documento_Cod", dataItem.Tipo_Documento_Cod);
        model.set("Tipo_Documento_Des", dataItem.Tipo_Documento_Des);
    }
}

async function TipoCarburanti_CreaDropdownList(container, options) {
    if (options.model.Richiesta_Cod > 0) {
        grid.closeCell();
    } else {
        if (tipoCarburanti === null)
            tipoCarburanti = await EseguireChiamataAjax("/CaricaTipoCarburantiDDL");
        creaDropDownEditor(container, "Tipo_Carburante_Des", "Tipo_Carburante_Cod", tipoCarburanti, onChange_TipoCarburanti);
    }
}

function onChange_TipoCarburanti(e) {
    let dataItem = e.sender.dataItem();
    let model = getModelFromGrid(this);

    if (model != null) {
        model.set("Tipo_Carburante_Cod", dataItem.Tipo_Carburante_Cod);
        model.set("Tipo_Carburante_Des", dataItem.Tipo_Carburante_Des);
    }
}

async function ContoProprioTerzi_CreaDropdownList(container, options) {
    if (options.model.Richiesta_Cod > 0) {
        grid.closeCell();
    } else {
        if (contiEsistenti === null)
            contiEsistenti = await EseguireChiamataAjax("/OttieneContiPossibiliDellAcquisto");
        creaDropDownEditor(container, "Conto_Proprio_Terzi_Des", "Conto_Proprio_Terzi_Cod", contiEsistenti, onChange_ContoAcquisto);
    }
}

function onChange_ContoAcquisto(e) {
    let dataItem = e.sender.dataItem();
    let model = getModelFromGrid(this);

    if (model != null) {
        model.set("Conto_Proprio_Terzi_Cod", dataItem.Conto_Proprio_Terzi_Cod);
        model.set("Conto_Proprio_Terzi_Des", dataItem.Conto_Proprio_Terzi_Des);
    }
}

async function AziendaVenditore_CreaDropDownList(container, options) {
    if (options.model.Richiesta_Cod > 0) {
        grid.closeCell();
    } else {
        if (imprese === null)
            imprese = await RicercaImprese(false);
        creaDropDownEditor(container, "rag_soc", "piva", imprese, onChange_aziendaVenditore);
        if (options.model.isNew() && options.model.Azienda_Venditore_Cod === "" && imprese.length === 1) {
            options.model.Azienda_Venditore_Cod = imprese[0].piva;
            options.model.Azienda_Venditore_Des = imprese[0].rag_soc;
            grid.closeCell();
        }
    }
}


function onChange_aziendaVenditore(e) {
    let dataItem = e.sender.dataItem();
    let model = getModelFromGrid(this);

    if (model != null) {
        model.set("Azienda_Venditore_Cod", dataItem.piva);
        model.set("Azienda_Venditore_Des", dataItem.rag_soc);
    }

}

async function Anno_CreaDropdownList(container, options) {
    if (options.model.Richiesta_Cod > 0) {
        grid.closeCell();
    } else {
        if (anniOpzioni === null)
            anniOpzioni = await EseguireChiamataAjax("/OttieneAnniValidi");
        creaDropDownEditor(container, "Anno_Des", "Anno_Cod", anniOpzioni, change_anno);
    }
}


function change_anno(e) {
    let cellaSelezionataDDL = e.sender.dataItem();
    let model = getModelFromGrid(this);

    if (model != null) {
        model.set("Anno_Cod", cellaSelezionataDDL.Anno_Cod);
        model.set("Anno_Des", cellaSelezionataDDL.Anno_Des);
    }
}

function getModelFromGrid(that) {
    let elem = that.element.closest("tr");
    let model = grid.dataItem(elem);
    return model;
}


async function grid_onCellClose(e) {
    if (e.type == "save") {
        let input = e.container.find("input[name='Cuaa']");
        if (input.length === 1) {
            if (e.model.Cuaa != null && e.model.Cuaa !== modelPrecedente.Cuaa) {
                let cuaaValido = await GestireModificaCUAA(e);
            }
        }

        input = e.container.find("input[name='Conto_Proprio_Terzi_Cod']");
        if (input.length === 1 && e.model.Conto_Proprio_Terzi_Cod !== modelPrecedente.Conto_Proprio_Terzi_Cod) {
            e.model.Lt = 0;
            e.model.LtInPrecedenza = 0;
            grid.refresh();
        }

        input = e.container.find("input[name='Anno_Cod']");
        if (input.length === 1 && e.model.Anno_Cod !== modelPrecedente.Anno_Cod) {
            e.model.Lt = 0;
            e.model.LtInPrecedenza = 0;
            grid.refresh();
        }

        input = e.container.find("input[name='Tipo_Carburante_Cod']");
        if (input.length === 1 && e.model.Tipo_Carburante_Cod !== modelPrecedente.Tipo_Carburante_Cod) {
            e.model.Lt = 0;
            e.model.LtInPrecedenza = 0;
            grid.refresh();
        }
    }
}

async function AzzeraLt(e) {
    e.model.Lt = 0;
    e.model.LtInPrecedenza = 0;
    grid.refresh();
}


async function AggiornaLt(e) {
    e.model.Lt = 0;
    grid.refresh();
}

async function beforeEdit(e) {
    let input = e.container.find("input[name='Lt']");
    if (input.length === 1) {
        await aggiornaLtSelezionatiInPrecendenza(e);
    }

    modelPrecedente = Object.assign({}, e.model);
}

async function aggiornaLtSelezionatiInPrecendenza(e) {
    let elemStessaCUAA = e.sender.dataSource.data().filter(elem => elem.Cuaa === e.model.Cuaa);
    let totalLtTuttiElem = elemStessaCUAA.reduce((a, b) => a + b.Lt, 0);

    let risultato = EseguireChiamataAjaxSync("/AggiornaLtAssegnati", {
        parametri:
        {
            Anno: e.model.Anno_Cod,
            PivaCliente: e.model.PIVA_Cliente,
            Tipo_Carburante: e.model.Tipo_Carburante_Cod,
            ContoProprioTerzi: e.model.Conto_Proprio_Terzi_Cod
        }
    });
    let ltAssegnati = parseInt(risultato.RispostaStringa, 10);

    if (totalLtTuttiElem <= ltAssegnati)
        e.model.LtInPrecedenza = e.model.Lt;
}

async function GestireModificaCUAA(evento) {
    if (evento.model.Cuaa === "")
        return;

    let success = await OttienePivaClienteERagioneSociale(evento);

    if (success) {
        if (evento.model.Cuaa !== modelPrecedente.Cuaa) {
            await AzzeraLt(evento);
        }

    } else {
        await AzzeraCampi(evento);
    }
}

async function AzzeraCampi(e) {
    e.model.Lt = 0;
    e.model.LtInPrecedenza = 0;
    e.model.PIVA_Cliente = "";
    e.model.Ragione_Sociale = "";
    grid.refresh();
}

async function VerificaPraticaRichiestaCarburanteAperta(event) {
    let success = ControlloreDiParametri.VerificaPraticaApertaParametriSonoImpostati(event);

    if (success) {
        let verifica = await EseguireChiamataAjax("/VerificaPraticaRichiestaCarburanteAperta", {
            filtri: {
                Cuaa: event.model.Cuaa,
                Anno: event.model.Anno_Cod
            }
        });
        if (!verifica.RispostaOK)
            MostraMessaggioErrore(verifica.RispostaStringa);
        return verifica.RispostaOK;
    }
    else {
        MostraMessaggioErrore(errori);
    }
}

async function OttienePivaClienteERagioneSociale(evento) {
    let success = ControlloreDiParametri.VerificaCampiImpostatiOttienePivaCliente(evento);

    if (success) {
        let risp = await EseguireChiamataAjax("/OttienePivaClienteERagioneSociale", {
            filtri: {
                Cuaa: evento.model.Cuaa,
                Anno: evento.model.Anno_Cod
            }
        });
        if (risp.RispostaOK) {
            let result = JSON.parse(risp.RispostaStringa);

            evento.model.PIVA_Cliente = result.Piva_Cliente;
            evento.model.Ragione_Sociale = result.Ragione_Sociale;
            return true;
        }
        else {
            MostraMessaggioErrore(risp.RispostaStringa);
            return false;
        }
    }
}

async function AggiornaRagioneSociale(evento) {
    let risp = await EseguireChiamataAjax("/CaricaRagioneSociale", { cuaa: evento.model.Cuaa });

    if (risp.RispostaOK) {
        evento.model.Ragione_Sociale = risp.RispostaStringa;
        grid.refresh();
    }
}

async function AggiornaLtAssegnati(evento) {
    let ltAssegnatiCampiObblImpostati = evento.model.PIVA_Cliente != null && evento.model.Anno_Cod != 0 && evento.model.Tipo_Carburante_Cod != null;
    if (!ltAssegnatiCampiObblImpostati) {
        $.logThis("Non è possibile aggiornare i lt assegnati: piva_cliente=" + evento.model.PIVA_Cliente +
            ", anno=" + evento.model.Anno_Cod + ", tipo_carburante_cod=" + evento.model.Tipo_Carburante_Cod);
        return;
    }
    let risposta = await EseguireChiamataAjax("/AggiornaLtAssegnati", {
        parametri:
        {
            Anno: evento.model.Anno_Cod,
            PivaCliente: evento.model.PIVA_Cliente,
            Tipo_Carburante: evento.model.Tipo_Carburante_Cod,
            ContoProprioTerzi: evento.model.Conto_Proprio_Terzi_Cod
        }
    });

    if (risposta.RispostaOK) {
        evento.model.Lt_Assegnati = parseInt(risposta.RispostaStringa);
        grid.refresh();
    }
}
async function AggiornaTotaleLtAcquistati(evento) {
    let campiObblImpostati = evento.model.Cuaa != null && evento.model.Anno_Cod != null && evento.model.Tipo_Carburante_Cod != null && evento.model.Conto_Proprio_Terzi_Cod != null;
    if (!campiObblImpostati)
        return;
    let risp = await EseguireChiamataAjax("/AggiornaTotaleLtAcquistati", {
        parametri:
            { cuaa: evento.model.Cuaa, anno: evento.model.Anno_Cod, tipoCarburante: evento.model.Tipo_Carburante_Cod, ContoProprioContoTerzi: evento.model.Conto_Proprio_Terzi_Cod }
    });

    if (risp.RispostaOK) {
        evento.model.Totale_Lt_Acquistati = risp.RispostaStringa;
        grid.refresh();
    }
}

async function AggiornaLtAcquistabili(evento) {
    let ltAcquistabili = evento.model.Lt_Assegnati - evento.model.Totale_Lt_Acquistati;

    evento.model.Lt_Acquistabili = ltAcquistabili;
    grid.refresh();
}

function simpleEditor(container, options) {
    if (options.model.Richiesta_Cod > 0) {
        grid.closeCell();
    } else {
        $('<input required name="' + options.field + '" validationMessage="Campo ddd obbligatorio"/>')
            .appendTo(container)
            .kendoNumericTextBox({
                spinners: false,
                min: 0
            });
        $('<span class="k-invalid-msg" data-for="' + options.field + '"></span>').appendTo(container);
    }
}


function CaricaCampiKendoModel(colonna_editabile) {
    return {
        PIVA_Cliente: { editable: false, type: "string", defaultValue: "" },

        Id_Vendite: { editable: false, type: "number", defaultValue: 0 },
        Azienda_Venditore_Cod: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Azienda_Venditore_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Anno_Cod: { editable: colonna_editabile, type: "number", defaultValue: new Date().getFullYear() },
        Anno_Des: { editable: colonna_editabile, type: "string", defaultValue: new Date().getFullYear() },
        Cuaa: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Ragione_Sociale: { editable: false, type: "string", defaultValue: "" },
        Conto_Proprio_Terzi_Cod: { editable: colonna_editabile, type: "number", defaultValue: -2 },
        Conto_Proprio_Terzi_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Tipo_Carburante_Cod: { editable: colonna_editabile, type: "number", defaultValue: 2 },
        Tipo_Carburante_Des: { editable: colonna_editabile, type: "string", defaultValue: "Gasolio" },
        Lt_Assegnati: { editable: false, type: "number", defaultValue: 0 },
        Totale_Lt_Acquistati: { editable: false, type: "number", defaultValue: 0 },
        Lt_Acquistabili: { editable: false, type: "number", defaultValue: 0 },
        Lt: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Tipo_Documento_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Tipo_Documento_Cod: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Data_Documento: { editabile: false, type: "date", defaultValue: "" },
        Nr_Documento: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Note_Rivenditore: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Data_Creazione: { editable: false, type: "date", defaultValue: "" },
        Data_Modifica: { editable: false, type: "date", defaultValue: "" },
        Richiesta_Cod: { editable: false, type: "number", defaultValue: 0 }

    };
}


function CaricaColonneKendoGrid(IDControllo) {
    var footerTemplateLtCalcolato = "#=calcTotaleColonna('" + "Lt" + "', " + IDControllo + ")#";
    return [
        {
            field: "Azienda_Venditore_Des",
            title: TraduciVenditeCarburanti("AziendaVenditore", "Azienda Venditore"),
            editor: AziendaVenditore_CreaDropDownList,
            filterable: { multi: true, search: true },
            validation: { required: true }
        },
        {
            field: "Anno_Des",
            title: TraduciVenditeCarburanti("Anno", "Anno"),
            filterable: { multi: true, search: true },
            editor: Anno_CreaDropdownList,
        },
        {
            field: "Cuaa",
            filterable: { multi: true, search: true },
            title: TraduciVenditeCarburanti("Cuaa", "Cuaa"),
        },
        {
            field: "Ragione_Sociale",
            filterable: { multi: true, search: true },
            title: TraduciVenditeCarburanti("Ragione_Sociale", "Ragione Sociale"),
        },
        {
            field: "Conto_Proprio_Terzi_Des",
            title: TraduciVenditeCarburanti("Conto_Proprio_Terzi", "Conto Proprio o Terzi"),
            editor: ContoProprioTerzi_CreaDropdownList,
            attributes: { class: "Conto_Proprio_Terzi_Des" },
            filterable: { multi: true, search: true },
            validation: { required: true }
        },
        {
            field: "Tipo_Carburante_Des",
            title: TraduciVenditeCarburanti("Tipo_Carburante", "Tipo Carburante"),
            editor: TipoCarburanti_CreaDropdownList,
            filterable: { multi: true, search: true },
        },
        //{
        //    hidden: true,
        //    field: "Lt_Assegnati",
        //    title: TraduciVenditeCarburanti("Lt_Assegnati", "Lt. Assegnati"),
        //},
        //{
        //    hidden: true,
        //    field: "Totale_Lt_Acquistati",
        //    title: TraduciVenditeCarburanti("Totale_Lt_Acquistati", "Totale Lt. Acquistati"),
        //},
        //{
        //    hidden: true,
        //    field: "Lt_Acquistabili",
        //    title: TraduciVenditeCarburanti("Lt_Acquistabili", "Lt. Acquistabili"),
        //},
        {
            field: "Lt",
            title: TraduciVenditeCarburanti("Lt", "Lt."),
            template: "#:kendo.toString(Lt, '0.00')#",
            headerAttributes: { style: "text-align: right" },
            attributes: { style: "text-align:right;" },
            editor: simpleEditor,
            footerTemplate: footerTemplateLtCalcolato
        },
        {
            field: "Tipo_Documento_Des",
            title: TraduciVenditeCarburanti("Tipo_Documento", "Tipo Documento"),
            filterable: { multi: true, search: true },
            editor: TipoDocumento_CreaDropdownList,
        },
        {
            field: "Data_Documento",
            title: TraduciVenditeCarburanti("Data_Documento", "Data Documento"),
            format: "{0:dd/MM/yyyy}"
        },
        {
            field: "Nr_Documento",
            title: TraduciVenditeCarburanti("Nr_Documento", "Nr Documento"),
        },
        {
            field: "Note_Rivenditore",
            filterable: { multi: true, search: true },
            title: TraduciVenditeCarburanti("Note_Rivenditore", "Note Rivenditore"),
        },
        {
            field: "Data_Creazione",
            title: TraduciVenditeCarburanti("Data_Creazione", "Data Creazione"),
            format: "{0:dd/MM/yyyy HH:mm:ss}"
        },
        {
            field: "Data_Modifica",
            title: TraduciVenditeCarburanti("Data_Modifica", "Data Modifica"),
            format: "{0:dd/MM/yyyy HH:mm:ss}"
        }
    ];
}


function calcTotaleColonna(field, idGriglia) {
    if (field !== undefined) {
        var grid = $("#grdVenditeUMACarburanti").data("kendoGrid");
        var dataSource = grid.dataSource;

        var filteredDataSource = new kendo.data.DataSource({
            data: dataSource.data(),
            filter: dataSource.filter()
        });

        filteredDataSource.read();
        var datiFiltrati = filteredDataSource.view();

        var totale = 0;

        $.each(datiFiltrati, function (index, model) {
            if (model.get(field) !== undefined && model.get(field) !== null && (model.deleted === undefined || model.deleted === false)) {
                totale += model.get(field);
            }
        });

        return kendo.toString(totale, "0.00");
    }
}


//function calcTotaleColonna(field, idGriglia) {
//    if (field !== undefined) {
//        var grid = $("#grdVenditeUMACarburanti").data("kendoGrid");
//        var dataSource = grid.dataSource;

//        var filteredDataSource = new kendo.data.DataSource({
//            data: dataSource.data(),
//            filter: dataSource.filter()
//        });

//        filteredDataSource.read();
//        var datiFiltrati = filteredDataSource.view();

//        var totale = 0;

//        $.each(datiFiltrati, function (index, model) {
//            if (model.get(field) !== undefined && model.get(field) !== null && (model.deleted === undefined || model.deleted === false)) {
//                totale += model.get(field);
//            }
//        });

//        return kendo.toString(totale, "0.00");
//    }
//}

async function DettaglioLtAcquistabili(tr_elem, grid_elem) {
    $("#DIV_Messaggi").hide();
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    //$(document.body).append('<div id="dettagliLtAcquistabili"></div>');

    let campiObblImpostati = datiRiga.Cuaa.length !== 0 && datiRiga.PIVA_Cliente.length !== 0;
    if (!campiObblImpostati) {
        MostraMessaggioErrore(TraduciVenditeCarburanti("DettagliCampiObbligatori", "Alcuni campi obbligatori non sono impostati. Campi obbligatori: anno, cuaa, ragione sociale, tipo carburante."))
        return;
    }

    let risultato = await EseguireChiamataAjax("/DettaglioLtAcquistabili", {
        parametri:
        {
            Anno: datiRiga.Anno_Cod,
            Cuaa: datiRiga.Cuaa,
            PivaCliente: datiRiga.PIVA_Cliente,
            Tipo_Carburante: datiRiga.Tipo_Carburante_Cod,
        }
    });

    if (risultato.RispostaOK) {
        let dataPopup = JSON.parse(risultato.RispostaStringa);

        inizializaWindow_DettagliLtAcquistabili();
        inizializzaData_DettagliLtAcquistabili(dataPopup);
        dettagliLtAcquistabiliWindow.center();
        dettagliLtAcquistabiliWindow.open();
    }
    else {
        MostraMessaggioErrore(risultato.RispostaStringa);
    }
}

function inizializzaData_DettagliLtAcquistabili(data) {
    if (data.ltAcquistabiliProprio == null || data.ltAcquistabiliProprio == "")
        data.ltAcquistabiliProprio = 0;
    if (data.ltAcquistabiliTerzi == null || data.ltAcquistabiliTerzi == "")
        data.ltAcquistabiliTerzi = 0;

    let filteredData1 = kendo.toString(data.ltAcquistabiliProprio, '0.00');
    let filteredData2 = kendo.toString(data.ltAcquistabiliTerzi, '0.00');

    if (detailsContoProprioInput == null || detailsContoProprioInput == null) {
        detailsContoProprioInput = $("#contoProprioTxtBox").kendoNumericTextBox({
            value: filteredData1,
            spinners: false
        }).data("kendoNumericTextBox");
        detailsContoProprioInput.enable(false);
        detailsContoTerzoInput = $("#contoTerziTxtBox").kendoNumericTextBox({
            value: filteredData2,
            spinners: false
        }).data("kendoNumericTextBox");
        detailsContoTerzoInput.enable(false);
    }
    else {
        detailsContoProprioInput.value(filteredData1);
        detailsContoTerzoInput.value(filteredData2);
    }
}
function inizializaWindow_DettagliLtAcquistabili() {
    if ($("#dettagliLtAcquistabili").data("kendoWindow") == undefined) {

        var windowOptions = {
            actions: ["Close"],
            draggable: false,
            modal: true,
            resizable: false,
            width: "600px",
            title: "Dettaglio Lt. Acquistabili",
            actions: ["Close"],
        };

        dettagliLtAcquistabiliWindow = $("#dettagliLtAcquistabili").kendoWindow(windowOptions).data("kendoWindow");
    }
}

class ControlloreDiParametri {
    constructor() {
        this.campiDaControllare = [];
    }

    static VerificaCampiImpostatiOttienePivaCliente(event) {
        let errori = ControlloreDiParametri._verificaCuaaAnnoImpostati(event);
        if (errori.length !== 0)
            MostraMessaggioErrore(errori);
        return errori.length === 0;
    }

    static VerificaPraticaApertaParametriSonoImpostati(event) {
        let errori = ControlloreDiParametri._verificaCuaaAnnoImpostati(event);
        if (errori.length !== 0)
            MostraMessaggioErrore(errori);
        return errori.length === 0;
    }

    static _verificaCuaaAnnoImpostati(event) {
        let verificaParam = new ControlloreDiParametri();
        verificaParam._aggiungiCampo(event.model.Cuaa, "Cuaa");
        verificaParam._aggiungiCampo(event.model.Anno_Cod, "Anno");
        let errori = verificaParam._verificaParametriObbligatoriImpostati();
        return errori;
    }

    _aggiungiCampo(value, fieldname) {
        this.campiDaControllare.push({ value: value, fieldname: fieldname });
    }

    _verificaParametriObbligatoriImpostati() {
        let errori = "";
        this.campiDaControllare.forEach(s => {
            errori += this._nonContieneValorePredefinitoONull(s.value, s.fieldName);
        });

        return errori;
    }
    _toType(obj) {
        return ({}).toString.call(obj).match(/\s([a-zA-Z]+)/)[1].toLowerCase();
    }

    _nonContieneValorePredefinitoONull(value, fieldName) {
        switch (this._toType(value)) {
            case "number":
                if (!value || value === 0) {
                    if (fieldName === "Id_Vendite")
                        return "Salvare la richiesta del carburante prima di continuare. "
                    else
                        return fieldName + " contiene un valore invalido. ";
                }
                break;
            case "string":
                if (!value || value.length === 0)
                    return fieldName + " contiene un valore invalido. ";
                break;
        }
        return "";
    }
}

let TipoAcquisto = {
    PROPRIO: 0,
    TERZI: -1,
    DEFAULT: -2,
}

function getYear() {
    if ($.cookie("UMA.Anno") != null && !isNaN($.cookie("UMA.Anno"))) {
        return parseInt($.cookie("UMA.Anno"));
    } else {
        var date = new Date();
        let anno = new Date().getFullYear();
        date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
        $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
        return anno;
    }
}

function setYear(anno) {
    var date = new Date();
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
}