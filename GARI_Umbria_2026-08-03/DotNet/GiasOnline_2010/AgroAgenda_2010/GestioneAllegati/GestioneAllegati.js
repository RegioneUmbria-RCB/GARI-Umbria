
function inizializzaCmb_TipoDocumento() {
    Cmb_TipoDocumento = $("#Cmb_TipoDocumento").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "text",
        dataValueField: "value",
        dataSource: { transport: { read: CaricaComboCmb_TipoDocumento } },
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {
            kendoDropDownAdjustWidth(e);
            /* if (onLoad) {
                this.value(obj_Documento_Selezionato.Tipologia);
                this.trigger("change");
                onLoad = false;
            } */
        },
        change: function (e) {
            // obj_Documento_Selezionato.Tipologia = this.value();
        }
    }).data("kendoDropDownList");
}

function inizializzaKendoAllegati(idDiv) {

    var funzioniCRUD = {
        funzioneRead: dataGrigliaAllegati
    };

    var idModel = "ID_Elenco";
    var campiKendoModel = modelGrigliaAllegati();
    var colonneKendoGrid = colonneGrigliaAllegati();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriColonneCustom = [];

    if (operazione != 0) {

        var templateCommand = "<div class='btn btn-success'  onclick=modificaAllegato(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-pencil'></i></div> ";
        templateCommand += "<div class='btn btn-danger'  onclick=cancellaAllegato(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-trash'></i></div>";

        parametriColonneCustom = [
            {
                command: {
                    template: templateCommand
                },
                title: "Azioni",
                width: "97px"
            }
        ];

    }


    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: true,
        editable: false,
        groupable: false,
        reorderable: true,
        //selectable: "row",
        // filterable: { mode: "row" },
        colonneCustomKendoGrid: parametriColonneCustom
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBound };
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
    gridAllegati = $("#" + idDiv).data("kendoGrid");
}

function dataGrigliaAllegati(options) {
    if (kendoServer) {
        var a = JSON.parse($("#kendoAllegati").val());
        options.success(a.kendo_rows);
    } else LeggiAllegati(options);
}

function modelGrigliaAllegati(options) {
    if (kendoServer) {
        var a = JSON.parse($("#kendoAllegati").val());
        return a.kendo_model;
    }
    return {
        ID_Elenco: { type: "number" },
        ID_Alert_Entita: { type: "number" },
        Allegati_Documenti_Cod: { type: "number" },
        Tipologia: { type: "string" },
        Allegati_Documenti_Numero: { type: "string" },
        Data_Scadenza: { type: "date" },
        Allegati_Documenti_NomeFile: { type: "string" },
        Allegati_Documenti_Ente_Des: { type: "string" },
        Validazione_Data: { type: "date" },
        Descrizione_Scadenza: { type: "string" },
        Allegati_Documenti_Estensione: { type: "string" }
    };
}

function colonneGrigliaAllegati(options) {
    if (kendoServer) {
        let a = JSON.parse($("#kendoAllegati").val());
        return a.kendo_columns;
    }

    var kendoColumns = [
        {
            template: function (dataItem) {
                if (dataItem.Allegati_Documenti_NomeFile !== "") {
                    let icon = CreaIconaDownloadDocumento(dataItem.Allegati_Documenti_Estensione);
                    return "<span class='fa fa-2x " + icon + " ' onClick=Leggi_Doc_Allegato(" + dataItem.Allegati_Documenti_Cod + ")></span>";
                } else {
                    return "";
                }
            },
            title: "Apri Doc.",
            width: 100
        },
        {
            field: "Allegati_Documenti_NomeFile",
            title: "Nome Allegato",
            //template: '#=templateFile(data.Allegati_Documenti_Cod, data.Allegati_Documenti_NomeFile)#',
            filterable: { multi: true, search: true }
        },

        { field: "Tipologia", title: "Tipologia", filterable: { multi: true, search: true } },
        { field: "Allegati_Documenti_Numero", title: "N. Documento", filterable: { multi: true, search: true } },
        { field: "Allegati_Documenti_Ente_Des", title: "Ente Rilascio", filterable: { multi: true, search: true } },
        { field: "Validazione_Data", title: "Data Rilascio", template: '#=templateData(data.Validazione_Data)#' },
        { field: "Data_Scadenza", title: "Data Scadenza", template: '#=templateData(data.Data_Scadenza)#' },
        { field: "Descrizione_Scadenza", title: "Descrizione", filterable: { multi: false, search: true } },
    ];

    return kendoColumns;
}

function onDataBound(e) {
    // var data = gridAllegati.dataSource.data();
    /* for (var i = 0; i < data.length; i++) {
        var uid = data[i].uid;
        var row = gridAllegati.tbody.find("tr[data-uid='" + uid + "']");

        if (data[i].Allegati_Documenti_NomeFile != "") {
            row.find(".k-command-cell").contents().last()[0].textContent = data[i].Allegati_Documenti_NomeFile;
        } else {
            row.find(".k-command-cell").contents().hide();
        }
    } */

    //Se non esistono allegati nascondo le colonne che non servono (apertura allegati) e nome file
    var data = gridAllegati.dataSource.data();
    var almenoUno = false
    data.forEach((row) => {
        if (row.Allegati_Documenti_NomeFile != null && row.Allegati_Documenti_NomeFile != '') {
            //Esiste almeno un allegato, lascio le colonne degli allegati visibili e a sx
            almenoUno = true;
            return true;
        }
    })

    if (!almenoUno) {
        gridAllegati.hideColumn(1);
        gridAllegati.hideColumn("Allegati_Documenti_NomeFile");
    }

    gridAllegati.autoFitColumn();
}

function templateData(data) {
    if (data == null || kendo.toString(data, "dd/MM/yyyy") === "01/01/1900" || kendo.toString(data, "dd/MM/yyyy") === "31/12/2100") return "";
    return kendo.toString(data, "dd/MM/yyyy"); // "dd/MM/yyyy HH:mm:ss"
}

//function templateFile(id, nome) {
//    return (nome != null && nome != "" ? ("<a href='javascript:ScaricaAllegato(" + id + ");'>" + nome + "</a>") : "");
//}

async function Leggi_Doc_Allegato(Allegati_Documenti_Cod) {

    var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti, 'piva': '', 'allegati_documenti_cod': Allegati_Documenti_Cod });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_File_Allegato", param,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let fileDati = risp[0].File_Allegato_DB;
            let nomeFile = risp[0].Allegati_Documenti_NomeFile;
            let estensione = risp[0].Allegati_Documenti_Estensione;

            SaveAndOpenFileByteArray(nomeFile, fileDati, estensione);

        }, null);

}

function modificaAllegato(tr_elem, grid_elem) {
    var datiRiga = gridAllegati.dataItem(tr_elem);
    ModificaAllegato(datiRiga);
}

function cancellaAllegato(tr_elem, grid_elem) {
    var datiRiga = gridAllegati.dataItem(tr_elem);
    kendo.confirm("Sei sicuro di voler eliminare l'allegato selezionato?").then(function () {
        CancellaAllegato(datiRiga);
    }, function () { });
}

function validaAllegato(nuovo) {

    let tipo_doc = KendoDDL("Cmb_TipoDocumento").value();
    if (tipo_doc === "" || tipo_doc === undefined || tipo_doc === null) {
        kendo.alert("Inserire il tipo documento. ");
        return false;
    }

    let num_doc = $("#Txt_Num_Documento").val();
    if (num_doc === "" || num_doc === undefined || num_doc === null) {
        kendo.alert("Inserire il numero documento. ");
        return false;
    }

    let ente_doc = $("#Txt_Ente_Rilascio").val();
    /* if (ente_doc === "" || ente_doc === undefined || ente_doc === null) {
        kendo.alert("Inserire l'ente rilascio. ");
        return false;
    } */

    // kendo.toString(data_rilascio, 'd')
    let data_rilascio = kendo.parseDate($("#Txt_Data_Rilascio").val());
    if (data_rilascio === "" || data_rilascio === null) {
        kendo.alert("Inserire una data rilascio. ");
        return false;
    }
    /* if (data_rilascio == null) {
        kendo.alert("Inserire date rilascio .");
        return false;
    } */
    let data_scadenza = kendo.parseDate($("#Txt_Data_Scadenza").val());
    if (data_scadenza === "" || data_scadenza === null) {
        kendo.alert("Inserire una data scadenza. ");
        return false;
    }
    /* if (data_scadenza == null) {
        kendo.alert("Inserire data scadenza.");
        return false;
    }*/

    if (data_rilascio != null && data_scadenza != null && data_scadenza < data_rilascio) {
        kendo.alert("La data scadenza deve essere successiva alla data rilascio.");
        return false;
    }

    let id_elenco = nuovo ? 0 : $("#Txt_ID_Elenco").val();
    let data = gridAllegati.dataSource.data();
    let data_inizio = data_rilascio != null ? data_rilascio : new Date(1900, 1, 1);
    let data_fine = data_scadenza != null ? data_scadenza : new Date(2100, 11, 31);

    let dummyScadenza = null
    for (var i = 0; i < data.length; i++) {
        if (data[i].ID_Tipologia == tipo_doc && data[i].ID_Elenco != id_elenco) {
            if (data_inizio <= data[i].Data_Scadenza && data[i].Validazione_Data <= data_fine) {
                let dummyDate = new Date(data[i].Data_Scadenza)
                if (confirm("Con il salvataggio della nuova data di rilascio del patentino si aggiornerà la data di scadenza del patentino precedente.\nProcedere ugualmente?") == false)
                    return false
                else
                    return true
            }
        }
    }







    ////Antivirus
    //var nome_file = $('#Txt_Documento_Allegato').val();
    //file_check = $('#File_Caricato').val();

    //if (file_check !== undefined && file_check !== null && file_check !== "") {

    //    var esito = checkVirus(nome_file, file_check, objP_super_server);

    //    if (esito !== "") {

    //        kendo.alert(esito);
    //        $('#Txt_Documento_Allegato').val("");
    //        $('#File_Caricato').val("");

    //        return false;

    //    }
    //}


    return true;

}