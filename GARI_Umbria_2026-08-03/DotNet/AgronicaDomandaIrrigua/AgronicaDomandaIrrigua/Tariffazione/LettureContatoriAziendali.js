function InizializzaVideata() {
    Abilitazioni();
    Ricerca();
}

function Abilitazioni() {
    if (writeRule === "True") {
        $("#btnSaveData").show();
    } else {
        $("#btnSaveData").hide();
    }
}

function CaricaDatiVideata() {
    let data = JSON.parse($(cIdDatiLetture).val());
    if (data.elencoContatori.length <= 0) {
        kendo.alert("Attenzione non risultano contatori validi nell'anno " + $("#txtAnno").data("kendoNumericTextBox").value() + ". Impossibile inserire nuove letture.");
        writeRule = "False";
        Abilitazioni();
    }
    $("#txtRagSoc").val(data.datiAzienda.RagioneSociale);

    popolaGrigliaAnagraficheContatori("divKendoGridAnagContatori")

    popolaGrigliaLetture("divKendoGridLetture");

}

function ReadElencoAnagraficheContatori(options) {
    var data = JSON.parse($(cIdDatiLetture).val());
    options.success(data.elencoContatori);
}

function ControlFormatDate(data) {
    if (data == null) {
        return '';
    } else {
        let dateConv = kendo.toString(kendo.parseDate(data, 'yyyy-MM-ddTHH:mm'), 'dd/MM/yyyy');
        if (dateConv === '01/01/1900' || dateConv === '31/12/2100') {
            return '';
        } else {
            return dateConv;
        }
    }
}

function popolaGrigliaAnagraficheContatori(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: ReadElencoAnagraficheContatori
    };

    var idModel = "id_contatore";
    var campiKendoModel = {
        id_contatore: { editable: false, type: "number" },
        matricola: { editable: false, type: "string" },
        descrizione: { editable: false, type: "string" },
        ValidoDal: { editable: false, type: "string" },
        ValidoAl: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        { field: "id_contatore", title: "Codice contatore", hidden: true },
        { field: "matricola", title: "Matricola", width: 30 },
        { field: "descrizione", title: "Descrizione libera", width: 60, hidden: true },
        { field: "ValidoDal", title: "Utilizzo dal", template: "#= ControlFormatDate(ValidoDal) #", width: 30, filterable: { multi: true, search: true } },
        { field: "ValidoAl", title: "Utilizzo Al", template: "#=  ControlFormatDate(ValidoAl) #", width: 30, filterable: { multi: true, search: true } },
    ];

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: true,
        groupable: false,
        reorderable: false,
        columnMenu: true,
        selectable: false,
        pdf: false,
        scrollable: true,
        resizable: true,
        pageable: false,
        btnEliminaTuttiFiltri: false
    };

    var funzioniPrimaDopoEventi = {};
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

function ReadElencoLetture(options) {
    var data = JSON.parse($(cIdDatiLetture).val());
    options.success(data.elencoLetture);
}

function popolaGrigliaLetture(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: ReadElencoLetture
    };

    var idModel = "id";
    var campiKendoModel = {
        id: { editable: false, type: "number" },
        id_contatore: { editable: false, type: "number" },
        datadettura: { editable: false, type: "string" },
        valore: { editable: false, type: "number"}
    };
    var colonneKendoGrid = [
        { field: "id", title: "Id lettura", hidden: true },
        { field: "id_contatore", title: "Id contatore", hidden: true },
        { field: "datalettura", title: "Data Ora lettura", template: "#=  (datalettura == null)? '' : kendo.toString(kendo.parseDate(datalettura, 'yyyy-MM-ddTHH:mm'), 'dd/MM/yyyy HH:mm') #", width: 30, filterable: { multi: true, search: true }, editable: function () { return false; } },
        { field: "valore", title: "Valore rilevato (mc)", format: '{0:##,#.0}', filterable: { multi: true, search: true }, width: 30, attributes: { style: "text-align:right;" } }
    ];

    if (writeRule === "True") {
        colonneKendoGrid.unshift({
            command: [
                //{
                //    template: "<span class='fa fa-trash fa-xs' title='Elimina' style='margin: 5px; vertical-align: middle;' onclick=chiediConfermaEliminazione(this.closest('tr'), this.closest('.k-grid'))></span>"
                //}
                {
                    iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "EliminaLettura", text: "", click: chiediConfermaEliminazione
                }
            ], title: "Azioni", width: "10px"
        });
        var toolbars = ["customToolbar"];
    }else {
        var toolbars = [];
    }

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        toolbarCommands: toolbars,
        editable: true,
        groupable: false,
        reorderable: false,
        columnMenu: true,
        selectable: false,
        pdf: false,
        scrollable: true,
        resizable: true,
        pageable: false,
        btnEliminaTuttiFiltri: false
    };

    var funzioniPrimaDopoEventi = { };
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

function chiediConfermaEliminazione(e) {
    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    $("#confermaEliminazioneDialog").kendoDialog({
        width: "400px",
        title: "Letture contatore",
        closable: false,
        modal: true,
        visible: false,
        content: "<p>Eliminare l'elemento selezionato?<p>",
        actions: [
            { text: "Conferma", action: function (e) { EliminaLettura(dataItem); } },
            { text: "Annulla", primary: true }
        ]
    });
    $("#confermaEliminazioneDialog").data("kendoDialog").open();
}

function EliminaLettura(dataItem) {
    var grid = $("#divKendoGridLetture").data('kendoGrid');
    var dataSource = grid.dataSource;
    dataSource.remove(dataItem);
    if (dataItem.id !== -1) {
        EliminaLettureContatori(dataItem.id, "", 0, "1900-01-01", "2100-12-31")
    }
}

function ChiediConfermaInserimentoLettura(AValue, BValue, AData, BData) {
    let testo = "<div>Confermi l'inserimento?</div>";

    if (AValue > 0 && BValue <= 0) {
        testo = "<div>La nuova lettura inserita deve essere maggiore di " + AValue + " mc (" + AData.toLocaleDateString() + " " + AData.toLocaleTimeString()+") </div>" + testo;
    }
    if (AValue > 0 && BValue > 0) {
        testo = "<div>La nuova lettura inserita non è compresa tra " + AValue + " mc (" + AData.toLocaleDateString() + " " + AData.toLocaleTimeString() + ")  e " + BValue + " mc (" + BData.toLocaleDateString() + " " + BData.toLocaleTimeString() +") .</div>" + testo;
    }
    if (AValue <= 0 && BValue > 0) {
        testo = "<div>La nuova lettura inserita deve essere minore di " + BValue + " mc (" + BData.toLocaleDateString() + " " + BData.toLocaleTimeString() +") .</div>" + testo;
    }

    kendo.confirm(testo).then(function () {
        return true;
    }, function () {
        return false;
    });
}

function AggiungiNuovaLettura() {
    var grid = $("#divKendoGridLetture").data('kendoGrid');
    popup_NuovaLettura(grid, $(cIdIdContatore).val());
}

function GetDefaultDate() {
    let newDate;
    if ($("#txtAnno").data("kendoNumericTextBox") == null ) {
        newDate= new Date();
    } else {
        if ($("#txtAnno").data("kendoNumericTextBox").value() !== new Date().getFullYear()) {
            newDate= new Date($("#txtAnno").data("kendoNumericTextBox").value(), 0, 1);
        } else {
            newDate= new Date();
        }
    }
    LeggiAnagraficaContatore($(cIdPiva).val(), newDate);
    return newDate;
}

function popup_NuovaLettura(grid,id_contatore) {

    let win_el = document.createElement("div");
    document.body.appendChild(win_el);
    let $win_el = $(win_el);

    let Anno = parseInt($("#txtAnno").val());

    let content = "<div style='margin-left:10px;margin-right:20px;'>";

    content += "<div style='padding-bottom: 10px'> ";
    content += " <label class='lbl_required'>Data lettura:</label>";
    content += " <input id='txt_data_lettura' name='txt_data_lettura' style='width:100%;' MaxLength='10' />";
    content += "</div>";

    content += "<div style='padding-bottom: 10px'> ";
    content += " <label class='lbl_required'>Valore (mc):</label>";
    content += " <input id='txt_valore_lettura' name='txt_valore_lettura' style='width:100%;' MaxLength='10' />";
    content += "</div>";

    content += "</div>";

    $win_el.kendoDialog({
        title: "Inserisci nuova lettura contatore",
        closable: false,
        modal: true,
        visible: false,
        content: content,
        width: "40%",
        open: function () {

            $("#txt_data_lettura").kendoDateTimePicker({
                footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
                min: new Date(Anno, 0, 01, 0, 0, 0),
                max: new Date(Anno, 11, 31, 23, 59, 59),
                value: GetDefaultDate(),
                change: function () {
                    console.log(this.value());
                    if (this.value() !== null) {
                        LeggiAnagraficaContatore($(cIdPiva).val(), this.value());
                    }
                }
            });
            $('#txt_valore_lettura').kendoNumericTextBox({ format: "{0:n1}", decimals: 1 });
        },
        actions: [
            {
                text: 'Salva',
                cssClass: "",
                action: function (e) {

                    var data_new_lettura = $("#txt_data_lettura").data("kendoDateTimePicker").value();
                    var val_new_lettura = $("#txt_valore_lettura").val();
                    let msg = "";

                    if (data_new_lettura == null) {
                        msg += "<div>Inserire una data ed ora corretti</div>";
                    }
                    if (val_new_lettura == null || val_new_lettura ==="" || val_new_lettura<0) {
                        msg += "<div>Inserire un valore corretto per la lettura</div>";
                    }

                    if (msg !== "") {
                        kendo.alert(msg);
                        return false;
                    }

                    var check = InsertNewValueToCorrectPosition(grid, data_new_lettura, val_new_lettura);
                    if (check === true) {
                        grid.refresh();
                    }
                    return check;
                }
            },
            {
                text: "Annulla",
                action: function (e) {
                    this.close();
                }
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}


function InsertNewValueToCorrectPosition(grid, data_new_lettura, val_new_lettura) {
    var righe = grid.dataSource.data();

    if (righe.length > 0) {

        for (var i = 0; i < righe.length; i++) {
            if (i + 1 < righe.length) {
                
                if (kendo.parseDate(righe[i].datalettura, 'yyyy-MM-ddTHH:mm') < data_new_lettura) {
                    if (i === 0) {
                        if (CheckValoreProgressivo(val_new_lettura, righe[i].valore, GetPrimoValoreAnnoSuccessivo()) === false) {
                            if (ChiediConfermaInserimentoLettura(righe[i].valore, GetPrimoValoreAnnoSuccessivo(), kendo.parseDate(righe[i].datalettura, 'yyyy-MM-ddTHH:mm'), GetDataPrimoValoreAnnoSuccessivo()) === true) {
                                grid.dataSource.insert(i, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                            }
                        } else {
                            grid.dataSource.insert(i, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                        }
                    } else {
                        if (CheckValoreProgressivo(val_new_lettura, righe[i + 1].valore, righe[i].valore) === false) {
                            if (ChiediConfermaInserimentoLettura(righe[i + 1].valore, righe[i].valore, kendo.parseDate(righe[i+1].datalettura, 'yyyy-MM-ddTHH:mm'), kendo.parseDate(righe[i].datalettura, 'yyyy-MM-ddTHH:mm')) === true) {
                                grid.dataSource.insert(i, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                            }
                        } else {
                            grid.dataSource.insert(i, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                        }
                    }
                    break;
                }
                if (kendo.parseDate(righe[i].datalettura, 'yyyy-MM-ddTHH:mm') >= data_new_lettura && kendo.parseDate(righe[i+1].datalettura, 'yyyy-MM-ddTHH:mm') <= data_new_lettura) {
                    if (CheckValoreProgressivo(val_new_lettura, righe[i + 1].valore, righe[i].valore) === true) {
                        grid.dataSource.insert(i + 1, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                    } else {
                        if (ChiediConfermaInserimentoLettura(righe[i + 1].valore, righe[i].valore, kendo.parseDate(righe[i + 1].datalettura, 'yyyy-MM-ddTHH:mm'), kendo.parseDate(righe[i].datalettura, 'yyyy-MM-ddTHH:mm')) === true) {
                            grid.dataSource.insert(i + 1, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                        }
                    }
                    break;
                }
            } else {
                if (kendo.parseDate(righe[i].datalettura, 'yyyy-MM-ddTHH:mm') >= data_new_lettura) {
                    if (CheckValoreProgressivo(val_new_lettura, GetUltimoValoreAnnoPrecedente(), righe[i].valore) === true) {
                        grid.dataSource.insert(i + 1, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                    } else {
                        if (ChiediConfermaInserimentoLettura(GetUltimoValoreAnnoPrecedente(), righe[i].valore, GetDataUltimoValoreAnnoPrecedente(), kendo.parseDate(righe[i].datalettura, 'yyyy-MM-ddTHH:mm')) === true) {
                            grid.dataSource.insert(i + 1, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                        }
                    }
                } else {
                    if (CheckValoreProgressivo(val_new_lettura, righe[i].valore, GetPrimoValoreAnnoSuccessivo()) === true) {
                        grid.dataSource.insert(i, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                    } else {
                        if (ChiediConfermaInserimentoLettura(righe[i].valore, GetPrimoValoreAnnoSuccessivo(), kendo.parseDate(righe[i].datalettura, 'yyyy-MM-ddTHH:mm'), GetDataPrimoValoreAnnoSuccessivo()) === true) {
                            grid.dataSource.insert(i, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
                        }
                    }
                }
                break;
            }
        }
    } else {
        if (CheckValoreProgressivo(val_new_lettura, GetUltimoValoreAnnoPrecedente(), GetPrimoValoreAnnoSuccessivo())){
            grid.dataSource.insert(0, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
        } else {
            if (ChiediConfermaInserimentoLettura(GetUltimoValoreAnnoPrecedente(), GetPrimoValoreAnnoSuccessivo(), GetDataUltimoValoreAnnoPrecedente(), GetDataPrimoValoreAnnoSuccessivo()) === true) {
                grid.dataSource.insert(0, { id: -1, id_contatore: $(cIdIdContatore).val(), datalettura: data_new_lettura, valore: val_new_lettura });
            }
        }
    }
    return true;
}

function GetPrimoValoreAnnoSuccessivo() {
    if ($(cIdDatiPrimaLetturaAnnoSuccessivo).val() === "") {
        return 0;
    } else {
        let data = JSON.parse($(cIdDatiPrimaLetturaAnnoSuccessivo).val())
        if (data.elencoLetture == null) {
            return 0;
        }
        return data.elencoLetture[0].valore;
    }    
}

function GetUltimoValoreAnnoPrecedente() {
    if ($(cIdDatiUltimaLetturaAnnoPrecedente).val() === "") {
        return 0;
    } else {
        let data = JSON.parse($(cIdDatiUltimaLetturaAnnoPrecedente).val())
        if (data.elencoLetture == null) {
            return 0;
        }
        return data.elencoLetture[0].valore;
    }
}

function GetDataPrimoValoreAnnoSuccessivo() {
    if ($(cIdDatiPrimaLetturaAnnoSuccessivo).val() === "") {
        return new Date(CurrentDate.getFullYear()+1,0,1,0,0,0);
    } else {
        let data = JSON.parse($(cIdDatiPrimaLetturaAnnoSuccessivo).val())
        if (data.elencoLetture == null) {
            return new Date(CurrentDate.getFullYear() + 1, 0, 1, 0, 0, 0);
        }
        return kendo.parseDate(data.elencoLetture[0].datalettura, 'yyyy-MM-ddTHH:mm');
    }
}

function GetDataUltimoValoreAnnoPrecedente() {
    if ($(cIdDatiUltimaLetturaAnnoPrecedente).val() === "") {
        return new Date(CurrentDate.getFullYear() - 1, 11, 31, 23, 59, 59);
    } else {
        let data = JSON.parse($(cIdDatiUltimaLetturaAnnoPrecedente).val())
        if (data.elencoLetture == null) {
            return new Date(CurrentDate.getFullYear() - 1, 11, 31, 23, 59, 59);
        }
        return kendo.parseDate(data.elencoLetture[0].datalettura,'yyyy-MM-ddTHH:mm');
    }
}

function CheckValoreProgressivo(newValue, Value1, Value2) {
    let CurrValue1 = (Value1 === null) ? 0 : parseFloat(Value1);
    let CurrValue2 = (Value2 === null) ? 0 : parseFloat(Value2);
    let CurrValue = parseFloat(newValue);

    if (CurrValue2 === 0) {
        if (CurrValue >= CurrValue1) {
            return true;
        } else {
            return false;
        }
    }

    if (CurrValue <= CurrValue2 && CurrValue >= CurrValue1) {
        return true;
    } else {
        return false;
    }
}

function Salva() {
    let check = CheckSalvaOK($("#divKendoGridLetture").data('kendoGrid').dataSource.data());
    if (check === false) {
        kendo.alert("Inserire almeno una lettura prima di salvare. ");
        return false;
    }
    ScriviAggiornaLettureContatori();
    return true;
}

function CheckSalvaOK(data) {
    if (data.length>0) {
        return true;
    } else {
        return false;
    }
}

function Ricerca() {
    if (CheckIsModify(true) === false) {
        LeggiUltimoValoreAnnoPrecedente($(cIdPiva).val(), $("#txtAnno").data("kendoNumericTextBox").value()-1);
        LeggiPrimoValoreAnnoSuccessivo($(cIdPiva).val(), $("#txtAnno").data("kendoNumericTextBox").value()+1);
        LeggiLettureContatori($(cIdPiva).val(), 0, $("#txtAnno").data("kendoNumericTextBox").value());
    }
}

function CheckIsModify(showMessage) {
    if ($("#divKendoGridLetture").data("kendoGrid") === undefined) {
        return false;
    }
    var data = $("#divKendoGridLetture").data("kendoGrid").dataSource.data();
    var chk = false;
    for (var i = 0; i < data.length; i++) {
        if (data[i].dirty === true || data[i].id===-1) {
            chk = true;
            break;
        }
    }
    if (chk === true) {
        if (showMessage == null || showMessage === true) {
            kendo.alert("<div>Attenzione salvare le modifiche prima di proseguire</div>");
        }
    }
    return chk;
    
}

function Esci(){
    if (CheckIsModify(false) === true) {
        kendo.confirm("<div>Attenzione le modifiche non salvate andranno perse. Proseguire?</div>")
            .done(function () {
                window.history.go(-1);
            })
            .fail(function () {
            });
    } else {
        window.history.go(-1);
    }
}