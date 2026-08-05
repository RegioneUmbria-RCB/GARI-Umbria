
// ************************************************* Crea il Kendo Grid *************************************************
async function popolaGrigliaRichiestaDocumenti(IDControllo) {

    let abilitato = utenteAbilitatoModifica();
    var uteAbilitatoScrit = false;

    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: RichiestaDocumenti_CaricaConfigurazioniDaDB,
        funzioneSubmit: { funzione: SubmitGrid_RichiestaDocumenti, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true,
    };

    var colonna_editabile = false;

    var colonneCustomKendoGrid;

    if (abilitato) {
        colonneCustomKendoGrid = [
            {
                command:
                    [
                        {
                            template: "<div class='btn btn-info fa fa-upload btnNuovoDocumento' style='display:block;width:25px;border:0px;' onclick=NuovoDocumento(this.closest('tr'),this.closest('.k-grid'))></div>",

                            visible: function (dataItem) {
                                return (dataItem.Nr_Documenti > dataItem.Nr_Documenti_Presenti || dataItem.Nr_Documenti === 0) && dataItem.Autorizzato === 1;
                            }
                        }
                    ], title: Traduzione(gestioneCarbResx, "NuovoDocumento", "Nuovo Documento"), width: "120px", headerAttributes: { style: "text-align: center; vertical-align: top;" }
            },
        ]
    }

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "Alert_Entita";
    var campiKendoModel = CaricaCampiKendoModel(true)
    var colonneKendoGrid = CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: false,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        // editable: false,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = null;

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditRichiestaDocumentiConfig, funzioneDaChiamareDopoDataBound: autoFitAllColumns, funzioneDaChiamareDopoDelete: null };

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
}


function autoFitAllColumns(e) {
    var grid = $("#grdRichiestaDocumenti").data("kendoGrid");
    for (i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].width === undefined) {
            grid.autoFitColumn(i);
        }
    }
}

//ritorna 1 se sono impostati correttamente
function CampiObbligatoriSonoImpostati(rigaDati) {
    return 1 //rigaDati.Id_Schema_Template > 0 && (rigaDati.Ambito > -15 || rigaDati.Ambito < -14) && (rigaDati.Fase > -21 || rigaDati.Fase < -16) && rigaDati.Tipologia !== null && rigaDati.Nr_Documenti !== null;
}

function RigaCaricataDalServer(riga) {
    return !riga.Modificabile;
}

function CampoNonModificabile(elem) {
    return elem.hasClass("edit_onInsert");
}




function ControlloCampiObbligatoriImpostati(data, index, tipoElem) {
    let campiInvalidi = []

    if (data.Ambito == null || data.Ambito >= -14 && data.Ambito <= -15)
        campiInvalidi.push("Ambito");

    if (data.Fase == null || data.Fase >= -16 && data.Fase <= -21)
        campiInvalidi.push("Fase");

    if (data.Tipologia == null)
        campiInvalidi.push("Tipologia");

    if (data.Nr_Documenti == null)
        campiInvalidi.push("Nr_Documenti");

    if (campiInvalidi.length === 0)
        return "";
    else if (campiInvalidi.length === 1)
        return tipoElem + "Index " + (index + 1) + ". Campo: " + campiInvalidi[0] + " non contiene un valore valido.";
    else
        return tipoElem + "Index " + (index + 1) + ". Campi: " + campiInvalidi.join(", ") + " non contengono un valore valido.";
}

function ImpostaCampiDefault(data) {
    //if (data.Servizio_Cod == null)
    //    data.Servizio_Cod = 0;
}


async function SubmitGrid_RichiestaDocumenti(options) {
    var grid = $("#grdRichiestaDocumenti").data("kendoGrid");

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            var errMess = ControlloCampiObbligatoriImpostati(currentData[i], i, "Nuovi elementi. ");

            if (errMess === "") {
                ImpostaCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            } else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        } else if (currentData[i].dirty) {
            var errMess = ControlloCampiObbligatoriImpostati(currentData[i], i, "Elementi modificati. ");

            if (errMess === "") {
                ImpostaCampiDefault(currentData[i]);
                updatedRecords.push(currentData[i].toJSON());
            } else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        ImpostaCampiDefault(grid.dataSource._destroyed[i]);
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    //***************** Salvataggio righe *****************//
    //if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

    //    await InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

    //    let grid = $("#grdRichiestaDocumenti").data("kendoGrid");
    //    grid.dataSource.read();
    //    grid.refresh();
    //}
}


//function RichiestaDocumenti_MessaggioCampiUguali(elem) {
//    let elemUguali = TraduciLavorazioni("elementiUguali", "Sono stati trovati due elementi uguali:");
//    let id_schema = TraduciLavorazioni("Id_Schema_Template", "Template Schema");


//    return elemUguali + id_schema + "=" + elem.Id_Schema_Template;
//}

//function RichiestaDocumenti_sonoUguali(p1, p2) {
//    return p1.Id_Schema_Template === p2.Id_Schema_Template;
//}

function CaricaCampiKendoModel(colonna_editabile) {
    return {
        Id_Schema_Template: { editable: false, type: "number", defaultValue: 0 },

        Nr_Documenti_Presenti: { editable: false, type: "number", defaultValue: 0 },
        Allegati_Documenti_Cod: { editable: false, type: "number", defaultValue: 0 },
        Richiesta_Cod: { editable: false, type: "number", defaultValue: 0 },
        Pratica_Cod: { editable: false, type: "number", defaultValue: 0 },
        Tipologia_Des: { editable: false, type: "string", defaultValue: "" },
        Ambito_Des: { editable: false, type: "string", defaultValue: "" },
        Fase_Des: { editable: false, type: "string", defaultValue: "" },
        Firmato: { editable: false, type: "string", defaultValue: "" },
        Obbligatorio: { editable: false, type: "string", defaultValue: "" },
        Autorizzato: { editable: false, type: "number", defaultValue: 0 },
        WAnagraficaStati_Cod: { editable: false, type: "number", defaultValue: 0 },
        WAnagraficaStati_Des: { editable: false, type: "string", defaultValue: "" },



        Ambito: { editable: false, type: "number", defaultValue: 0 },
        Servizio_Cod: { editable: false, type: "number", defaultValue: 0 },
        Stato_Da: { editable: false, type: "number", defaultValue: 0 },
        Stato_A: { editable: false, type: "number", defaultValue: 0 },
        Fase: { editable: false, type: "number", defaultValue: 0 },
        Ordine: { editable: false, type: "number", defaultValue: 0 },
        Tipologia: { editable: false, type: "number", defaultValue: 0 },
        Suffisso_File: { editable: false, type: "string", defaultValue: "" },
        Descrizione: { editable: false, type: "string", defaultValue: "" },
        Flag_Obbligatorio: { editable: false, type: "number", defaultValue: 0 },
        Flag_Firmato_Digit: { editable: false, type: "number", defaultValue: 0 },
        Nome_Modello: { editable: false, type: "string", defaultValue: "" },
        Nr_Documenti: { editable: false, type: "number", defaultValue: 0 },


        inviato: { editable: false, type: "number", defaultValue: 0 },
        datainvio: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Data_Creazione: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazone: { editable: false, type: "string", defaultValue: "" },
        Username_Modifica: { editable: false, type: "string", defaultValue: "" },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/1/1") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("2100/12/31") }
    };
}

function CaricaColonneKendoGrid() {
    return [
        {
            field: "Obbligatorio",
            title: TraduciCampi("Obbligatorio", "Obbligatorio"),
            filterable: { multi: true, search: true }
        },
        {
            field: "Tipologia_Des",
            title: "Tipologia",
            filterable: { multi: true, search: true }
        },
        { field: "Descrizione", title: TraduciCampi("Descrizione", "Descrizione"), filterable: false },

        {
            field: "Firmato",
            title: "Richiesta Firma Digitale",
            filterable: { multi: true, search: true }
        },
        {
            field: "Nr_Documenti_Presenti",
            title: "Numero Documenti Presenti"
        }
        //{
        //    field: "WAnagraficaStati_Des",
        //    title: "Stato Pratica",
        //    filterable: { multi: true, search: true }
        //}
    ];
}

function TraduciCampi(chiave, testoAlternativo) {
    return TraduzioneMultiResx(confRichiestaDocumentiResx, chiave, testoAlternativo);
}

function onEditRichiestaDocumentiConfig(e) {
    //if (CampiObbligatoriSonoImpostati(e.model) && RigaCaricataDalServer(e.model)) {
    //    if (CampoNonModificabile($(e.container[0]))) {
    //        e.sender.closeCell();
    //    }
    //}
}

function utenteAbilitatoModifica(fase) {

    var abilitato = false;
    let inCompilazione = false
    switch ($("#stato_pratica_cod").val()) {
        case "":
        case In_Compilazione.toString():
            if (permesso_richiesta) {
                abilitato = true;
            }
            inCompilazione = true

            break;
        case Verifica_In_Corso.toString():
            if (permesso_approvazione_richiesta) {
                abilitato = true;
            }
            break;
        case In_Compilazione.toString():
            if (permesso_rendicontazione) {
                abilitato = true;
            }
            inCompilazione = true

            break;
        case Verifica_In_Corso.toString():
            if (permesso_approvazione_rendicontazione) {
                abilitato = true;
            }
            break;
        case Compilazione_Alla_Data_Completata_e_Verificata.toString():
            abilitato = false;
            break;
        case Verifica_Intermedia_Completata_Con_Successo.toString():
            abilitato = false;
            break;
        default:
            abilitato = false;
            break;
    }

    if (QS_Avanzamento == 1 && inCompilazione) {
        //Se sono in rendicontazione e lo stato è In Compilazione, leggo la data per inserire i documenti
        abilitato = Leggi_Termine_Ultimo_Rendicontazione()
    }


    return abilitato
}

function CaricaAllegati(e) {

    //var url = "../Scadenzario/Scad_lista.aspx?type=doc" + "&richiesta_cod=" + QS_Richiesta + "&area_provenienza=7" + "&origine_nc=../CarburantiUMA/RichiestaCarburanti.aspx" + "&p=" + QS_Piva;

    var url = GetUrlDocAgenda2010(QS_Piva, 0, richiesta_Cod, 0, 0, "Read", DOCUMENTALE)

    apriKendoWindowTestataGriglia(url, "Documenti U.M.A. Carburanti");
}


function NuovoDocumento(e) {

    var ID_Alert_Entita = -1;
    var ID_Elenco = -1;
    var Richiesta_Cod = richiesta_cod;
    var Modalita = "doc";
    var grid = $("#grdRichiestaDocumenti").data("kendoGrid");
    var dataItem = grid.dataItem($(e).closest("tr"));
    var Id_Schema_Template = dataItem.Id_Schema_Template;
    var Tipologia = dataItem.Tipologia;
    var Pratica = dataItem.Pratica_Cod;
    var Piva = KendoDDL("ddlAzienda").value();

    var url = GetUrlDocAgenda2010(Piva, Tipologia, Richiesta_Cod, Pratica, Id_Schema_Template, "Add", DOCUMENTALE);

    var param = kendo.stringify({ 'Piva': Piva, 'ID_Elenco': ID_Elenco, 'TipoOperazione': 1, 'ID_Alert_Entita': ID_Alert_Entita, 'Richiesta_Cod': Richiesta_Cod, 'Tipologia': Tipologia, 'Id_Schema_Template': Id_Schema_Template, 'Pratica_Cod': Pratica });
    ////var param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 1, 'ID_Alert_Entita': ID_Alert_Entita });
    //var url = "../Scadenzario/Scad_CreaModificaItem.aspx?scadstr=" + param + "&type=" + Modalita + "&origine_nc=../CarburantiUMA/RichiestaCarburanti.aspx" + "&p=" + QS_Piva + "&rc=" + QS_Richiesta;

    apriKendoWindowTestataGriglia(url, "Documenti U.M.A. Carburanti");

}


