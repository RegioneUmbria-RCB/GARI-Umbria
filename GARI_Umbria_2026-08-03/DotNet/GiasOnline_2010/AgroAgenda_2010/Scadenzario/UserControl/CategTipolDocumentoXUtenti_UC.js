var resxCategTipolDocumentoUC = [];

function TraduciCategTipolDocumentoUC(chiave, testoAlternativo) {
    if (resxCategTipolDocumentoUC.length === 0) {
        resxCategTipolDocumentoUC.push(readResxFile("Scadenzario/UserControl/App_LocalResources/CategTipolDocumentoXUtenti_UC.ascx.resx", "CategTipolDocumentoXUtenti_UC.js"));
        resxCategTipolDocumentoUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "CategTipolDocumentoXUtenti_UC.js"));
    }
    return TraduzioneMultiResx(resxCategTipolDocumentoUC, chiave, testoAlternativo);
}

// Creazione Kendo Grid Utenti
function popolaGridcategtipodocxut_UC_utenti(IDControllo) {

    let UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    let funzioniCRUD = {};

    //Se l'Utente ha il permesso di modifica allora aggiungo la colonna con i check altrimenti no
    if (UteAbilitatoInsMod === true && UteAbilitatoCanc === true) 
        funzioniCRUD.checkBoxFunction = kSelezionatoUtente;


    funzioniCRUD.funzioneRead = CaricoGrigliaUtenti;
    funzioniCRUD.funzioneSubmit = { funzione: SubmitGrigliaUtenti, flagInsert: false, flagDelete: false };
    funzioniCRUD.UtenteAbilitatoInserimentoModifica = UteAbilitatoInsMod;
    funzioniCRUD.UtenteAbilitatoCancellazione = UteAbilitatoCanc;
    funzioniCRUD.omettiPulsantiSalva = true;
    funzioniCRUD.omettiPulsantiAnnulla = true;

    let idModel = "rowId";
    let campiKendoModel = {
        NOME: { editable: false, type: "string" },
        COGNOME: { editable: false, type: "string" },
        USER: { editable: false, type: "string" },
        GRUPPI_UTENTE_DES: { editable: false, type: "string" }
    };
    let colonneKendoGrid = [ 
        {
            field: "NOME", title: "Nome", filterable: { multi: true, search: true }
        },
        {
            field: "COGNOME", title: "Cognome", filterable: { multi: true, search: true }
        },
        {
            field: "USER", title: TraduciCategTipolDocumentoUC("Utente", "Utente"), filterable: { multi: true, search: true }
        },
        {
            field: "GRUPPI_UTENTE_DES", title: "Gruppo", filterable: { multi: true, search: true }
        }
    ];

    let parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: false,
        columnMenu: false,
        reorderable: true,
        excel: false,
        pdf: false,
        groupable: false,
        btnEliminaTuttiFiltri: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
    };

    let parametriPerLettura = null;
    let parametriDataSource = {};
    let funzioniPrimaDopoEventi = {};

    let mostraRigheCancellate = true;
    let colonneDisabilitateSoloInModifica = [];

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

function kSelezionatoUtente(e) {
    let checked = this.checked;
    let row = $(this).parents("tr");
    let grid = $("#categtipodocxut_UC_griglia_utenti").data("kendoGrid");
    let dataItem = grid.dataItem(row);


    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}

function SubmitGrigliaUtenti(options) {
    let grid = $("#categtipodocxut_UC_griglia_utenti").data("kendoGrid");

    let updatedRecords = [];

    let currentData = grid.dataSource.data();

    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].Selected === true) 
            updatedRecords.push(currentData[i].toJSON());
    }
    if (updatedRecords.length > 0) 
        Righe_Griglia_Utenti_CategTipolDocumentoXUtenti = updatedRecords;
    

}

// Creazione Kendo Grid Categoria/Tipologia
function popolaGridcategtipodocxut_UC_griglia_CategTip(IDControllo) {

    let UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    let funzioniCRUD = {};

    //Se l'Utente ha il permesso di modifica allora aggiungo la colonna con i check altrimenti no
    if (UteAbilitatoInsMod === true && UteAbilitatoCanc === true)
        funzioniCRUD.checkBoxFunction = kSelezionatoCategoriaTipologia;

    funzioniCRUD.funzioneRead = CaricoGrigliaCategoriaTipologia;
    funzioniCRUD.funzioneSubmit = { funzione: SubmitGrigliaCategoriaTipologia, flagInsert: false, flagDelete: false };
    funzioniCRUD.UtenteAbilitatoInserimentoModifica = UteAbilitatoInsMod;
    funzioniCRUD.UtenteAbilitatoCancellazione = UteAbilitatoCanc;
    funzioniCRUD.omettiPulsantiSalva = true;
    funzioniCRUD.omettiPulsantiAnnulla = true;


    let idModel = "rowId";
    let campiKendoModel = {
        ID_Area: { editable: false, type: "number" },
        Nome_Area: { editable: false, type: "string" },
        ID_Tipologia: { editable: false, type: "number" },
        Nome_Tipologia: { editable: false, type: "string" }
    };
    let colonneKendoGrid = [
        {
            field: "Nome_Area", title: TraduciCategTipolDocumentoUC("Categoria", "Categoria"), filterable: { multi: true, search: true }
        },
        {
            field: "Nome_Tipologia", title: TraduciCategTipolDocumentoUC("Tipologia", "Tipologia"), filterable: { multi: true, search: true }
        }
    ];

    let parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: false,
        columnMenu: false,
        reorderable: true,
        excel: false,
        pdf: false,
        groupable: false,
        btnEliminaTuttiFiltri: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
    };

    let parametriPerLettura = null;
    let parametriDataSource = {};
    let funzioniPrimaDopoEventi = {};

    let mostraRigheCancellate = true;
    let colonneDisabilitateSoloInModifica = [];

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


function kSelezionatoCategoriaTipologia(e) {
    let checked = this.checked;
    let row = $(this).parents("tr");
    let grid = $("#categtipodocxut_UC_griglia_CategTip").data("kendoGrid");
    let dataItem = grid.dataItem(row);


    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}

function SubmitGrigliaCategoriaTipologia(options) {
    let grid = $("#categtipodocxut_UC_griglia_CategTip").data("kendoGrid");

    let updatedRecords = [];

    let currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].Selected === true)
            updatedRecords.push(currentData[i].toJSON());
    }
    if (updatedRecords.length > 0) 
        Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti = updatedRecords;

}

// Creazione Kendo Grid Categoria/TipologiaxUtenti
function popolaGridcategtipodocxut_UC_griglia_CategTipxUtenti(IDControllo) {

    let UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    let funzioniCRUD = {
        funzioneRead: CaricoGrigliaCategoriaTipologiaXUtenti,
        funzioneSubmit: { funzione: "", flagInsert: false, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true
    };
    let idModel = "id_username";

    let colonna_editabile = false;

    if (UteAbilitatoInsMod === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    let campiKendoModel = {
        Piva: { editable: false, type: "string" },
        Rag_Soc: { editable: false, type: "string" },
        Nome_Utente: { editable: false, type: "string" },
        Cognome_Utente: { editable: false, type: "string" },
        Username: { editable: false, type: "string" },
        Gruppo_Utente: { editable: false, type: "string" },
        ID_Categoria: { editable: false, type: "number" },
        Nome_Categoria: { editable: false, type: "string" },
        ID_Tipologia: { editable: false, type: "number" },
        Nome_Tipologia: { editable: false, type: "string" },
        Autorizzato: { editable: false, type: "number" },
        Valore_Autorizzato: { editable: false, type: "string" }
    };
    let colonneKendoGrid = [
        {
            field: "Rag_Soc", title: TraduciCategTipolDocumentoUC("Azienda", "Azienda"), filterable: { multi: true, search: true }
        },
        {
            field: "Nome_Utente", title: "Nome", filterable: { multi: true, search: true }
        },
        {
            field: "Cognome_Utente", title: "Cognome", filterable: { multi: true, search: true }
        },
        {
            field: "Username", title: TraduciCategTipolDocumentoUC("Utente", "Utente"), filterable: { multi: true, search: true }
        },
        {
            field: "Gruppo_Utente", title: "Gruppo", filterable: { multi: true, search: true }
        },
        {
            field: "Nome_Categoria", title: TraduciCategTipolDocumentoUC("Categoria", "Categoria"), filterable: { multi: true, search: true }
        },
        {
            field: "Nome_Tipologia", title: TraduciCategTipolDocumentoUC("Tipologia", "Tipologia"), filterable: { multi: true, search: true }
        },
        {
            field: "Valore_Autorizzato", title: "Tipo di Autorizzazione", filterable: { multi: true, search: true }
        }
    ];

    let parametriKendoGrid = {
        columnMenu: false,
        pdf: false,
        groupable: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        reorderable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
    };

    let parametriPerLettura = null;
    let parametriDataSource = {};
    let funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDelete: function (e) {

            let container = document.getElementById("categtipodocxut_UC_griglia_CategTipxUtenti");
            let id_dialog = creaNewRowDiv("id_dialog_cancella_categtipodocxut");
            container.appendChild(id_dialog);

            let grid = $("#categtipodocxut_UC_griglia_CategTipxUtenti").data("kendoGrid");
            let row = $(e.target).closest("tr");
            let dataItem = grid.dataItem(row);

            $("#id_dialog_cancella_categtipodocxut").kendoDialog({
                title: "Conferma Cancellazione",
                closable: false,
                modal: {
                    preventScroll: true
                },
                content: "Si desidera davvero eliminare l'autorizzazione?",
                actions: [{
                    text: 'No',
                    primary: true,
                    action: function (e) {
                        dataItem.deleted = false;
                        row.removeClass("deletedKendoRow");
                        $("#id_dialog_cancella_categtipodocxut").remove();
                    }
                },
                {
                    text: 'Sì',
                    action: function (e) {
                        let ds_grid = grid.dataSource.data();
                        let deletedRecords = [];

                        for (let i = 0; i < ds_grid.length; i++) {
                            if (ds_grid[i].deleted === true) {
                                deletedRecords.push({ Piva: "" + ds_grid[i].Piva + "", Username: "" + ds_grid[i].Username + "", ID_Area: parseInt(ds_grid[i].ID_Categoria), ID_Tipologia: parseInt(ds_grid[i].ID_Tipologia), Autorizzato: parseInt(ds_grid[i].Autorizzato) });
                            }
                        }

                        //Cancello la riga selezionata della griglia CategTipologiaDocumentiXUtenti
                        Cancella_Griglia_CategTipxUtenti(deletedRecords);
                        $("#id_dialog_cancella_categtipodocxut").remove();
                    },
                 }]
            });
        }
    };
    let mostraRigheCancellate = true;
    let colonneDisabilitateSoloInModifica = [];


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

//function Autorizzato_DropDownEditor(container) {
//    let Autorizzato = [{ "Autorizzato": 0, "Valore_Autorizzato": TraduciCategTipolDocumentoUC("No", "No") }, { "Autorizzato": 1, "Valore_Autorizzato": TraduciCategTipolDocumentoUC("Si", "Sì") }];

//    creaDropDownEditor(container, "Valore_Autorizzato", "Autorizzato", Autorizzato, changeAutorizzatoGrigliaCategTipoxUtenti);
//}

//function changeAutorizzatoGrigliaCategTipoxUtenti(e) {
//    var dataItem = e.sender.dataItem();
//    var grid = $("#categtipodocxut_UC_griglia_CategTipxUtenti").data("kendoGrid");
//    var model = grid.dataItem(this.element.closest("tr"));
//    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
//    model.Autorizzato = dataItem.Autorizzato;
//    model.Valore_Autorizzato = dataItem.Valore_Autorizzato;
//    model.dirty = true;

//    //Salvo le modifiche di autorizzato sulla tabella CategTipologiaDocumentiXUtenti
//    let obj = [{ Username: ""+model.Username+"", ID_Area: parseInt(model.ID_Categoria), ID_Tipologia: parseInt(model.ID_Tipologia), Autorizzato: parseInt(model.Autorizzato)}];
//    Salva_Modifiche_Autorizzato(obj);
//}

function DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia() {

    let gridUtenti = $("#categtipodocxut_UC_griglia_utenti").data("kendoGrid");

    let gridCategoriaTipologia = $("#categtipodocxut_UC_griglia_CategTip").data("kendoGrid");

    if (gridUtenti === undefined || gridUtenti === null || gridUtenti === "" ||
        gridCategoriaTipologia === undefined || gridCategoriaTipologia === null || gridCategoriaTipologia === "")
        return;

    $("#categtipodocxut_UC_griglia_utenti-header-chb").prop("checked", false);

    for (let i = 0; i < gridUtenti.dataSource.data().length; i++) {
        if (gridUtenti.dataSource.data()[i].Selected === true) {
            gridUtenti.dataSource.data()[i].Selected = false;
            gridUtenti.dataSource.data()[i].dirty = false;
        }
    }
    gridUtenti.refresh();

    $("#categtipodocxut_UC_griglia_CategTip-header-chb").prop("checked", false);
    for (let i = 0; i < gridCategoriaTipologia.dataSource.data().length; i++) {
        if (gridCategoriaTipologia.dataSource.data()[i].Selected === true) {
            gridCategoriaTipologia.dataSource.data()[i].Selected = false;
            gridCategoriaTipologia.dataSource.data()[i].dirty = false;
        }
    }
    gridCategoriaTipologia.refresh();

    Righe_Griglia_Utenti_CategTipolDocumentoXUtenti = "";
    Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti = "";

}

function Autorizza_grid_tipologiexindice(tipo_permesso_documentale) {
    let piva = $(cIdPiva).val();

    if (piva === null || piva === "" || piva === undefined)
        return;

    let tipo_permesso_documentale_des = "";

    switch (tipo_permesso_documentale) {
        case Tipo_Permesso_Documentale.Gestione_Completa:
            tipo_permesso_documentale_des = "Gestione Completa";
            break;
        case Tipo_Permesso_Documentale.Lettura:
            tipo_permesso_documentale_des = "Sola Lettura";
            break;
    }

    let gridUtenti = $("#categtipodocxut_UC_griglia_utenti").data("kendoGrid");
    let gridCategoriaTipologia = $("#categtipodocxut_UC_griglia_CategTip").data("kendoGrid");

    gridUtenti.saveChanges();
    gridCategoriaTipologia.saveChanges();

    let MsgErrore = "";

    MsgErrore = Controlla_Righe_Selezionate_Grid_Utenti_Grid_Categoria_Tipologia();

    if (MsgErrore === "") {
        MsgErrore = Controlla_Permessi_Inseriti(tipo_permesso_documentale, tipo_permesso_documentale_des);

        if (MsgErrore === "") {
            MsgErrore = Controlla_Autorizzazioni_Presenti(tipo_permesso_documentale, tipo_permesso_documentale_des);
        }

    }

    if (MsgErrore !== "") {
        MessaggioErrore_Bootstrap(MsgErrore, "DIV_Messaggi");
    }


}

function Controlla_Autorizzazioni_Presenti(tipo_permesso_documentale, tipo_permesso_documentale_des) {

    if (Righe_Griglia_Utenti_CategTipolDocumentoXUtenti.length > 0 && Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti.length > 0) {

        WaitFrame.show();

        let MsgDialog = "";

        let autorizzazione_da_cercare = 0;

        switch (tipo_permesso_documentale) {
            case Tipo_Permesso_Documentale.Gestione_Completa:
                autorizzazione_da_cercare_des = "Sola Lettura";
                autorizzazione_da_cercare = Tipo_Permesso_Documentale.Lettura;
                break;
            case Tipo_Permesso_Documentale.Lettura:
                autorizzazione_da_cercare_des = "Gestione Completa";
                autorizzazione_da_cercare = Tipo_Permesso_Documentale.Gestione_Completa;
                break;
        }

        let MsgErrore = "";

        let utenti_array = [];

        let utenti_str = "";

        let autorizzazioni = CaricoAutorizzazioneUtentixCategoriaTipologia();

        if (autorizzazioni !== undefined && autorizzazioni !== null && autorizzazioni.length > 0) {

            for (let x = 0; x < Righe_Griglia_Utenti_CategTipolDocumentoXUtenti.length; x++) {

                for (let y = 0; y < Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti.length; y++) {

                    let obj = "";

                    //Per impedire che vengano inseriti dei permessi doppi con stessa Piva,Utente,Categoria,Tipologia(o Tipologia a 0) e stesso tipo di autorizzazione
                    obj = autorizzazioni.find(o => o.Username.toString() === Righe_Griglia_Utenti_CategTipolDocumentoXUtenti[x].USER.toString() &&
                        parseInt(o.ID_Categoria) === parseInt(Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti[y].ID_Area) &&
                        parseInt(o.Autorizzato) === tipo_permesso_documentale &&
                        (parseInt(o.ID_Tipologia) === parseInt(Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti[y].ID_Tipologia) ||
                         parseInt(o.ID_Tipologia) === 0 || parseInt(Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti[y].ID_Tipologia) === 0));

                    if (obj !== undefined && obj !== null && obj !== "") {

                        MsgErrore += "Errore si sta cercando di inserire un'Autorizzazione già presente per l'Utente "+ Righe_Griglia_Utenti_CategTipolDocumentoXUtenti[x].USER.toString()+".";
                        break;

                    } else {

                        obj = autorizzazioni.find(o => o.Username.toString() === Righe_Griglia_Utenti_CategTipolDocumentoXUtenti[x].USER.toString() &&
                            parseInt(o.ID_Categoria) === parseInt(Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti[y].ID_Area) &&
                            parseInt(o.Autorizzato) === autorizzazione_da_cercare &&
                            (parseInt(o.ID_Tipologia) === parseInt(Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti[y].ID_Tipologia) || o.ID_Tipologia === 0));

                        if (obj !== undefined && obj !== null && obj !== "" &&
                            obj.Username !== undefined && obj.Username !== null && obj.Username !== "" &&
                            utenti_array.includes(obj.Username.toString()) === false) {

                            utenti_str += "- " + obj.Username.toString() + ".<br>";
                            utenti_array.push(obj.Username.toString());

                        }
                    }



                }

                if (MsgErrore !== "")
                    break;

            }
        }

        WaitFrame.hide();

        if (MsgErrore === "") {
            if (utenti_str !== "" && utenti_array.length > 0) {

                let container = document.getElementById("btn_autorizza_grid_tipologiexindice");
                let id_dialog = creaNewRowDiv("id_dialog_autorizza");
                container.appendChild(id_dialog);

                MsgDialog += "<b>Attenzione verrano assegnate le autorizzazioni di " + tipo_permesso_documentale_des + " per le Categorie/Tipologie selezionate ai seguenti Utenti che precedentemente avevano le autorizzazioni di " + autorizzazione_da_cercare_des + " per l'azienda " + $(Rag_Soc_Azienda).val() + ":</b><br>" + utenti_str + "";

                $("#id_dialog_autorizza").kendoDialog({
                    title: "Conferma Autorizzazioni",
                    closable: false,
                    modal: {
                        preventScroll: true
                    },
                    content: MsgDialog,
                    actions: [{
                        text: 'Annulla',
                        primary: true,
                        action: function (e) {

                            DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();

                            $("#id_dialog_autorizza").remove();
                        }
                    },
                    {
                        text: 'Conferma',
                        action: function (e) {

                            Scrivi_Griglia_Categoria_TipologiaXUtenti(tipo_permesso_documentale);

                            $("#id_dialog_autorizza").remove();
                        },
                    }]
                });

            }
            else {

                Scrivi_Griglia_Categoria_TipologiaXUtenti(tipo_permesso_documentale);

            }
        }
        else {
            DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();
        }

        return MsgErrore;

    }

} 

function Controlla_Righe_Selezionate_Grid_Utenti_Grid_Categoria_Tipologia(tipo_permesso_documentale) {

    let MsgErrore = "";

    if (Righe_Griglia_Utenti_CategTipolDocumentoXUtenti === undefined || Righe_Griglia_Utenti_CategTipolDocumentoXUtenti === null ||
        Righe_Griglia_Utenti_CategTipolDocumentoXUtenti === "" || Righe_Griglia_Utenti_CategTipolDocumentoXUtenti.length === 0) {

        MsgErrore += TraduciCategTipolDocumentoUC("ErroreNonHaiSelezionatoNessunaRigaNellaTabellaUtente", "Errore non hai selezionato nessuna riga nella tabella Utente.") + "<br>";

    }

    if (Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti === undefined || Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti === null ||
        Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti === "" || Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti.length === 0) {

        MsgErrore += TraduciCategTipolDocumentoUC("ErroreNonHaiSelezionatoNessunaRigaNellaTabellaCategoriaTipologia", "Errore non hai selezionato nessuna riga nella tabella Categoria Tipologia.") + "<br>";
    }

    if (MsgErrore !== "") {
        DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();
    }

    return MsgErrore;
    
}


function Controlla_Permessi_Inseriti(tipo_permesso_documentale, tipo_permesso_documentale_des) {

    WaitFrame.show();

    let MsgErrore = "";

    if (MsgErrore === "") {

        let ds_grid_categoriatipologia = $("#categtipodocxut_UC_griglia_CategTip").data("kendoGrid").dataSource.data().toJSON();

        let ds_grid_categoriatipologiaxutente = $("#categtipodocxut_UC_griglia_CategTipxUtenti").data("kendoGrid").dataSource.data().toJSON();

        //Controllo che non si stia cercando di inserire tutte le Tipologie di una Categoria ed anche la Categoria con Tipologia a 0

        //let tuttelecategorieselezionate = $.Enumerable.From(Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti).GroupBy(x => x.ID_Area).ToArray();
        let tuttelecategorieselezionate = [];

        for (let item of Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti) {
            let gruppo = tuttelecategorieselezionate.find(grp => grp.ID_Area === item.ID_Area);
            if (!gruppo) {
                gruppo = { ID_Area: item.ID_Area, source: [] };
                tuttelecategorieselezionate.push(gruppo);
            }
            gruppo.source.push(item);
        }

        if (tuttelecategorieselezionate !== undefined && tuttelecategorieselezionate !== null && tuttelecategorieselezionate != "") {
            for (let x = 0; x < tuttelecategorieselezionate.length; x++) {

                let count_tipologia_generale = 0;

                let count_tipologia_specifica = 0;

                if (tuttelecategorieselezionate[x].source !== undefined && tuttelecategorieselezionate[x].source !== null &&
                    tuttelecategorieselezionate[x].source !== "" && tuttelecategorieselezionate[x].source.length > 0) {
                    for (let y = 0; y < tuttelecategorieselezionate[x].source.length; y++) {

                        if (tuttelecategorieselezionate[x].source[y].ID_Tipologia === 0) {
                            count_tipologia_generale++;
                        } else if (tuttelecategorieselezionate[x].source[y].ID_Tipologia !== 0) {
                            count_tipologia_specifica++;
                        }


                        if (count_tipologia_generale > 0 && count_tipologia_specifica > 0) {
                            MsgErrore += "Errore hai selezionato sia la Tipologia specifica sia la riga con la Tipologia vuota della Categoria " + tuttelecategorieselezionate[x].source[y].Nome_Area + ".";
                            break;
                        }

                    }
                }

                if (MsgErrore !== "")
                    break;
            }
        }

    }

    WaitFrame.hide();

    if (MsgErrore !== "") {
        DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();
    }

    return MsgErrore;
}