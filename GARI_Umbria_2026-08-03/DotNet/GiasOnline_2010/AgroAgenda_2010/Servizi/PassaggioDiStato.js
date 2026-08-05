var UtenteAbilitato_Provisioning_R;
var objLista_DSS_Selezionati;
var controlli = true;
var dialogAvanz;

async function Passaggio_di_Stato(Pratica_Cod, PassaggioDiStato_Cod) {
    PassaggioDiStato(Pratica_Cod, PassaggioDiStato_Cod);
}

async function PassaggioDiStato(Pratica_Cod, PassaggioDiStato_Cod) {
    WaitFrame.show();
    //initPassaggioStato();
    $("#divDSS").hide();

    let objPratica = await leggiDatiPratica(Pratica_Cod);
    let PassaggioDiStato = undefined;

    if (PassaggioDiStato_Cod != 0) {
        PassaggioDiStato = await leggiPassaggioDiStato(PassaggioDiStato_Cod);
    }

    let Servizio_Des;
    let Servizio_Cod;
    let Stato = "";
    let StatoAttuale_Cod;
    let note = "";
    let Data_Riferimento = new Date();
    var PassaggioDiStato_Cod = 0;
    if (PassaggioDiStato !== undefined) {
        PassaggioDiStato_Cod = PassaggioDiStato.PassaggioDiStato_Cod;
        Servizio_Des = objPratica.Servizio_Des;
        Servizio_Cod = objPratica.Servizio_Cod;
        Pratica_Cod = objPratica.Pratica_Cod;
        StatoAttuale_Cod = PassaggioDiStato.Stato_Cod;
        note = PassaggioDiStato.note;
        Data_Riferimento = PassaggioDiStato.Validita_Inizio_Stato;
    } else {
        Servizio_Des = objPratica.Servizio_Des;
        Servizio_Cod = objPratica.Servizio_Cod;
        Pratica_Cod = objPratica.Pratica_Cod;
        StatoAttuale_Cod = objPratica.StatoAttuale_Cod;
        Stato = objPratica.Stato;
        note = "";
        Data_Riferimento = new Date();
    }




    $("#lbl_Servizio").text(TraduzioneMultiResx(resxObj, "Servizio", "Servizio") + ": " + Servizio_Des);
    if (Stato !== undefined && Stato !== "") {
        $("#lbl_StatoAttuale").text(TraduzioneMultiResx(resxObj, "StatoAttuale", "Stato Attuale") + ": " + Stato);
    }
    else {
        $("#lbl_StatoAttuale").hide();
    }


    ddlProcedure = await ws_caricaProcedura(Pratica_Cod);
    CmbProcedura = await Carica_cmb_Procedura(ddlProcedure);
    CmbProcedura.refresh();

    if (PassaggioDiStato_Cod === 0) {
        ddlStatiDestinazione = Servizio_Cod === 2007 ? await ws_caricaStatiDestinazioneUMA(Pratica_Cod, StatoAttuale_Cod, Servizio_Cod) : await ws_caricaStatiDestinazione(StatoAttuale_Cod, Servizio_Cod, Pratica_Cod);
    } else {
        ddlStatiDestinazione = [{ Stato_Des: PassaggioDiStato.Stato_Des, Stato_Cod: StatoAttuale_Cod }];
    }

    if (ddlStatiDestinazione.length > 0) {
        $("#warningNessunPassaggioDiStato").hide();
    }
    else {
        $("#warningNessunPassaggioDiStato").show();
    }

    Cmb_Stato_Destinazione = await Carica_cmb_StatiDestinazione(ddlStatiDestinazione);
    Cmb_Stato_Destinazione.refresh();

    if (Servizio_Cod === 1009 && (StatoAttuale_Cod === 1001 && PassaggioDiStato_Cod === 0 || StatoAttuale_Cod === 1002 && PassaggioDiStato_Cod !== 0)) {
        if (UtenteAbilitato_Provisioning_R === true) {

            ddlPacchettiDSS = await ws_caricaPacchettiDSS();
            DSS_cmb_pacchettiAcquistati = await Carica_DSS_cmb_pacchettiAcquistati(ddlPacchettiDSS);
            DSS_cmb_pacchettiAcquistati.refresh();

            Txt_Data_Scadenza = get_Data_Scadenza();
            Txt_Data_Scadenza.value(new Date());

            if (DSS_cmb_pacchettiAcquistati.dataSource.data().length === 1) {
                DSS_cmb_pacchettiAcquistati.select(1);
            }

            objLista_DSS_Selezionati = await ws_CaricaDSS_PassaggioStato(PassaggioDiStato_Cod);
            creaKendoDSS(divKendoDSS);

            $("#divDSS").show();
        } else {
            kendo.alert(TraduzioneMultiResx(resxObj, "MancanzaPermessiPerServizio", "Non si hanno i permessi per questo tipo di servizio."));
            return false;
        }
    }

    if (CmbProcedura.dataSource.data().length === 1) {
        CmbProcedura.select(1);
    } else {
        $("#rowProcedura").show();
    }

    if (Cmb_Stato_Destinazione.dataSource.data().length === 1) {
        Cmb_Stato_Destinazione.select(1);
    }

    Txt_Data_Riferimento = get_Data_Riferimento();

    Txt_Data_Riferimento.value(Data_Riferimento);

    $("#Txt_Note").val(note);


    if (PassaggioDiStato_Cod !== 0) {
        Cmb_Stato_Destinazione.enable(false);
        CmbProcedura.enable(false);
        $("#rowProcedura").hide();
    } else {
        Cmb_Stato_Destinazione.enable(true);
        CmbProcedura.enable(true);
    }

    WaitFrame.hide();
    //kendoDialogPassaggioStato.open();
    //kendoDialogPassaggioStato.center();
}


function initPassaggioStato() {

    if (kendoDialogPassaggioStato !== undefined) {
        return;
    }

    kendoDialogPassaggioStato = $("#kendoDialogPassaggioStato").kendoDialog({
        width: "670px",
        heigth: "467px",
        maxHeight: "467px",
        modal: false,
        title: TraduzioneMultiResx(resxObj, "PassaggioDiStato", "Passaggio di Stato"),
        closable: true,
        visible: false,
        actions: [
            { text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla") },
            { text: TraduzioneMultiResx(resxObj, "Conferma", "Conferma"), action: CambiaStatoOK1, primary: true }
        ]
    }).data("kendoDialog");

}

async function Carica_cmb_StatiDestinazione(ddlStatiDestinazione) {
    return new Promise((resolve, reject) => {
        if ($("#Cmb_Stato_Destinazione").data("kendoDropDownList") !== undefined && Cmb_Stato_Destinazione !== undefined) {
            Cmb_Stato_Destinazione.destroy();
            $("#Cmb_Stato_Destinazione").html("");
        }
        let onLoad = true;
        $("#Cmb_Stato_Destinazione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Stato_Des",
            dataValueField: "Stato_Cod",
            dataSource: ddlStatiDestinazione,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                resolve(this);
            },
            optionLabel: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona")
        }).data("kendoDropDownList");
    });
}

async function Carica_DSS_cmb_pacchettiAcquistati(ddlPacchettiDSS) {
    return new Promise((resolve, reject) => {
        if (DSS_cmb_pacchettiAcquistati !== undefined) {
            DSS_cmb_pacchettiAcquistati.destroy();
            $("#DSS_cmb_pacchettiAcquistati").html("");
        }
        let onLoad = true;
        $("#DSS_cmb_pacchettiAcquistati").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "ModelliPrevisionaliRaggruppamenti_Des",
            dataValueField: "ModelliPrevisionaliRaggruppamenti_Cod",
            dataSource: ddlPacchettiDSS,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                resolve(this);
            },
            optionLabel: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona")
        }).data("kendoDropDownList");
    });
}

function get_Data_Scadenza() {
    if (Txt_Data_Scadenza === undefined) {
        return $("#Txt_Data_Scadenza").kendoDatePicker({
            dateInput: true
        }).data("kendoDatePicker");
    } else {
        return Txt_Data_Scadenza;
    }
}

function creaKendoDSS(id_div) {

    $("#" + id_div).html("");

    var funzioniCRUD = {
        funzioneRead: ReadDSS,
        //funzioneDelete: deleteDSS,
        //funzioneInsert: deleteDSS,
        funzioneUpdate: deleteDSS
    };
    var idModel = "ModelliPrevisionaliRaggruppamenti_Cod";
    var campiKendoModel = {
        "ModelliPrevisionaliRaggruppamenti_Cod": { "editable": true, "type": "number" },
        "ModelliPrevisionaliRaggruppamenti_Des": { "editable": true, "type": "string" },
        "Data_Scadenza": { "editable": true, "type": "date" }
    };
    var colonneKendoGrid = [
        { "field": "ModelliPrevisionaliRaggruppamenti_Des", "title": TraduzioneMultiResx(resxObj, "ModelloPrevisionale", "Modello"), "filterable": false },
        { "field": "Data_Scadenza", "title": TraduzioneMultiResx(resxObj, "DataScadenza", "Data Scadenza"), "filterable": false, template: '#= (kendo.toString(Data_Scadenza, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Scadenza, "dd/MM/yyyy" ) #' }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: false,
        pdf: false,
        excel: false,
        groupable: false,
        pageable: false,
        filterable: true,
        btnEliminaTuttiFiltri: false,
        colonneCustomKendoGrid: [
            {
                command: [
                    {
                        name: " ", click: deleteDSS, iconClass: "fa fa-close fa-2"
                    }
                ],
                title: TraduzioneMultiResx(resxObj, "Operazioni", "Operazioni"),
                width: "105px"
            }
        ],
        editable: { mode: "inline" }
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: DSSGridEdit,
        funzioneDaChiamareDopoSave: DSSSaveGriglia
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(id_div, // rappresenta l'ID del div a cui si associa la griglia
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

function deleteDSS(e) {
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    for (var i = 0; i < objLista_DSS_Selezionati.length; i++) {
        if (dataItem.ModelliPrevisionaliRaggruppamenti_Cod === objLista_DSS_Selezionati[i].ModelliPrevisionaliRaggruppamenti_Cod) {
            objLista_DSS_Selezionati.splice(i, 1);
            break;
        }
    }
    creaKendoDSS(divKendoDSS);
}

function ReadDSS(options) {
    //jSonParsed_Kendo = JSON.parse(objLista_DSS_Selezionati);
    options.success(objLista_DSS_Selezionati);
}

function DSSGridEdit(e) {

}

function DSSSaveGriglia(e) {

}

function get_Data_Riferimento() {
    if ($("#Txt_Data_Riferimento").data("kendoDatePicker") == undefined || Txt_Data_Riferimento === undefined) {
        return $("#Txt_Data_Riferimento").kendoDatePicker({
            dateInput: true
        }).data("kendoDatePicker");
    } else {
        return Txt_Data_Riferimento;
    }
}

async function CambiaStatoOK() {

    selected_add = new Array();
    //for (let i = 0; i < data.length; i++) {
    //    if (data[i].Selected === true) {
    //        selected_add.push(data[i]);
    //    }
    //}
    if (Number.isInteger(pratica_cod) === false && pratica_cod !== "") {
        let arrPraticheCod = pratica_cod.split("_");
        for (let i = 0; i < arrPraticheCod.length; i++) {
            selected_add.push({ Pratica_Cod: arrPraticheCod[i] });
        }
    }
    else {
        selected_add.push({ Pratica_Cod: pratica_cod });
    }


    if (Cmb_Stato_Destinazione.value() === "") {
        kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareUnoStatoDestinazione", "Selezionare uno stato di destinazione valido."));
        return false;
    }

    if (CmbProcedura.value() === "") {
        kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareUnaProceduraValida", "Selezionare una procedura valida."));
        return false;
    }

    if (Txt_Data_Riferimento.value() === "") {
        kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareUnaDataRiferimento", "Selezionare una data di riferimento valida."));
        return false;
    }


    let PassaggioDiStato_cod = 0;
    if (passaggiodistato_cod != 0) {
        PassaggioDiStato_cod = passaggiodistato_cod;
    }

    if (objLista_DSS_Selezionati === undefined) {
        objLista_DSS_Selezionati = new Array();
    }

    let resp = await ws_PassaggioStato(selected_add, Cmb_Stato_Destinazione.value(), Txt_Data_Riferimento.value(), $("#Txt_Note").val(), CmbProcedura.value(), PassaggioDiStato_cod, controlli, objLista_DSS_Selezionati);

    if (resp.RispostaConferma === true) {
        //window.parent.chiudiWindowPassaggioDiStato(resp.RispostaStringa);
        window.parent.postMessage(JSON.stringify(resp), ottieniTargetOrigin(window));
    } else {
        if (dialogAvanz == undefined) {

            //content: "<p>Il totale dei litri rendicontati (al netto del " + resp.ParametroDue_stringa + "%) è minore di quelli asseganti + rimanenti. I litri in difetto dovranno essere oggetto di recupero accisa.<p>",

            dialogAvanz = $("#dialogAvanz").kendoDialog({
                width: "400px",
                title: TraduzioneMultiResx(resxObj, "ConfermaAvanzamentoPratica", "Vuoi comunque avanzare la pratica?"),
                buttonLayout: "stretched",
                content: resp.Errore,
                actions: [
                    { text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla"), primary: true },
                    {
                        text: TraduzioneMultiResx(resxObj, "Conferma", "Conferma"),
                        action: function (e) {
                            WaitFrame.show();
                            controlli = false;

                            let respo = ws_PassaggioStato(selected_add, Cmb_Stato_Destinazione.value(), Txt_Data_Riferimento.value(), $("#Txt_Note").val(), CmbProcedura.value(), PassaggioDiStato_cod, controlli, objLista_DSS_Selezionati).then(respo => {

                                if (respo.RispostaConferma === true) {
                                    //window.parent.chiudiWindowPassaggioDiStato(resp.RispostaStringa);
                                    window.parent.postMessage(JSON.stringify(respo), ottieniTargetOrigin(window));
                                } else {
                                    kendo.alert(respo.RispostaStringa);
                                }
                                WaitFrame.hide();
                                return true;
                            });
                            return true;
                        }
                    }
                ]
            });

        }

        dialogAvanz.data("kendoDialog").open();
    }

}