var indirizzohttp = "./Scad_Anagrafiche.aspx";

//Carico la griglia Utenti
function CaricoGrigliaUtenti(options) {

    ajaxAgronica(indirizzohttp + "/Carica_Griglia_Utenti",
        "{}",
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null,null, true);

}

//Carico la griglia Categoria/Tipologia
function CaricoGrigliaCategoriaTipologia(options) {
    ajaxAgronica(indirizzohttp + "/Carica_Griglia_Categoria_Tipologia",
        "{}",
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null,null, true);
}


// Carico la griglia Categoria/TipologiaxUtenti
function CaricoGrigliaCategoriaTipologiaXUtenti(options) {

    let piva = $(cIdPiva).val();

    if (piva === null || piva === "" || piva === undefined)
        return;

    let param = "{piva:'" + piva + "'}";

    ajaxAgronica(indirizzohttp + "/Carica_Griglia_Categoria_TipologiaXUtenti",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, null, true);
}

// Carico le Autorizzazioni degli Utenti
function CaricoAutorizzazioneUtentixCategoriaTipologia() {

    let autorizzazioni = [];

    let piva = $(cIdPiva).val();

    if (piva === null || piva === "" || piva === undefined)
        return;

    let param = "{piva:'" + piva + "'}";

    ajaxAgronicaSync(indirizzohttp + "/Carico_Autorizzazione_UtentixCategoriaTipologia",
        param,false,
        function (risposta) {
            autorizzazioni = JSON.parse(risposta.RispostaStringa);
        }, null,null,true);

    return autorizzazioni;
}

//Quando clicco sul bottone "Autorizza" scrivo sulla grid popolaGridcategtipodocxut_UC_griglia_CategTipxUtenti
function Scrivi_Griglia_Categoria_TipologiaXUtenti(tipo_permesso_documentale) {

    let piva = $(cIdPiva).val();

    if (piva === null || piva === "" || piva === undefined ||
        tipo_permesso_documentale === null || tipo_permesso_documentale === "" || tipo_permesso_documentale === undefined || tipo_permesso_documentale<=0)
        return;

    let ModelUtenti = new Object();
    let ModelCategoriaTipologia = new Object();

    ModelUtenti = kendoEscapeOggetto(Righe_Griglia_Utenti_CategTipolDocumentoXUtenti);
    ModelCategoriaTipologia = kendoEscapeOggetto(Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti);

    let param = "{piva: '" + piva + "' ,modelutenti:'" + ModelUtenti + "' , modelcategoriatipologia:'" + ModelCategoriaTipologia + "' , Autorizzato:" + tipo_permesso_documentale + "}";

    ajaxAgronica(indirizzohttp + "/Scrivi_Griglia_Categoria_TipologiaXUtenti",
        param,
        function (risposta) {
            //Rileggo il datasource della griglia Categoria/Tipologia Utenti
            let gridCategTipxUtenti = $("#categtipodocxut_UC_griglia_CategTipxUtenti").data("kendoGrid");
            if (gridCategTipxUtenti !== undefined && gridCategTipxUtenti !== "") {
                //Utilizzo il setDatasource per aggiornare i filtri della colonna se precedentemente erano
                //stati selezionati.
                let vecchio_ds = gridCategTipxUtenti.dataSource;

                let ds = new kendo.data.DataSource({ data: [] });
                gridCategTipxUtenti.setDataSource(ds);

                gridCategTipxUtenti.setDataSource(vecchio_ds);
                gridCategTipxUtenti.dataSource.read();

                DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();
            }
        },
        function (risposta) {

            erroriNelSubmit = "";
            if (risposta.RispostaStringa !== undefined && risposta.RispostaStringa !== null && risposta.RispostaStringa !== "") {
                erroriNelSubmit = risposta.RispostaStringa;
            } else if (risposta.Errore !== undefined && risposta.Errore !== null && risposta.Errore !== "") {
                erroriNelSubmit = risposta.Errore;
            }

            MessaggioErrore_Bootstrap(erroriNelSubmit, "DIV_Messaggi");
            DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();
        }, null, true);
}


//Quando clicco sul bottone "Togli Autorizzazione" modifica la grid popolaGridcategtipodocxut_UC_griglia_CategTipxUtenti
$("#btn_togli_autorizza_grid_tipologiexindice").click(function () {

    let piva = $(cIdPiva).val();

    if (piva === null || piva === "" || piva === undefined)
        return;

    let gridUtenti = $("#categtipodocxut_UC_griglia_utenti").data("kendoGrid");
    let gridCategoriaTipologia = $("#categtipodocxut_UC_griglia_CategTip").data("kendoGrid");

    gridUtenti.saveChanges();
    gridCategoriaTipologia.saveChanges();

    let MsgErrore = Controlla_Righe_Selezionate_Grid_Utenti_Grid_Categoria_Tipologia();

    if (MsgErrore !== "") {
        MessaggioErrore_Bootstrap(MsgErrore, "DIV_Messaggi");
        return;
    }

    let container = document.getElementById("btn_togli_autorizza_grid_tipologiexindice");
    let id_dialog = creaNewRowDiv("id_dialog_togli_autorizzazione");
    container.appendChild(id_dialog);

    let MsgDialog = "";

    if (Righe_Griglia_Utenti_CategTipolDocumentoXUtenti.length > 1 || Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti.length > 1) {
        MsgDialog = "Si desidera davvero eliminare le autorizzazioni selezionate per l'azienda " + $(Rag_Soc_Azienda).val() + " ?";
    } else {
        MsgDialog = "Si desidera davvero eliminare l'autorizzazione selezionata per l'azienda " + $(Rag_Soc_Azienda).val() + " ?";
    }

    $("#id_dialog_togli_autorizzazione").kendoDialog({
        title: "Conferma Cancellazione",
        closable: false,
        modal: {
            preventScroll: true
        },
        content: MsgDialog,
        actions: [{
            text: 'No',
            primary: true,
            action: function (e) {

                DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();

                $("#id_dialog_togli_autorizzazione").remove();
            }
        },
        {
            text: 'Sì',
            action: function (e) {

                let ModelUtenti = new Object();
                let ModelCategoriaTipologia = new Object();

                ModelUtenti = kendoEscapeOggetto(Righe_Griglia_Utenti_CategTipolDocumentoXUtenti);
                ModelCategoriaTipologia = kendoEscapeOggetto(Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti);

                let param = "{piva: '" + piva + "' ,modelutenti:'" + ModelUtenti + "' , modelcategoriatipologia:'" + ModelCategoriaTipologia + "', modelcategoriatipologiaxutenti:''}";

                ajaxAgronica(indirizzohttp + "/Modifica_Griglia_Categoria_TipologiaXUtenti",
                    param,
                    function (risposta) {
                        //Rileggo il datasource della griglia Categoria/Tipologia Utenti
                        let gridCategTipxUtenti = $("#categtipodocxut_UC_griglia_CategTipxUtenti").data("kendoGrid");
                        if (gridCategTipxUtenti !== undefined && gridCategTipxUtenti !== "") {
                            //Utilizzo il setDatasource per aggiornare i filtri della colonna se precedentemente erano
                            //stati selezionati.
                            let vecchio_ds = gridCategTipxUtenti.dataSource;

                            let ds = new kendo.data.DataSource({ data: [] });
                            gridCategTipxUtenti.setDataSource(ds);

                            gridCategTipxUtenti.setDataSource(vecchio_ds);
                            gridCategTipxUtenti.dataSource.read();

                            DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();
                        }
                    },
                    function (risposta) {
                        erroriNelSubmit = "";
                        if (risposta.RispostaStringa !== undefined && risposta.RispostaStringa !== null && risposta.RispostaStringa !== "") {
                            erroriNelSubmit = risposta.RispostaStringa;
                        } else if (risposta.Errore !== undefined && risposta.Errore !== null && risposta.Errore !== "") {
                            erroriNelSubmit = risposta.Errore;
                        }

                        MessaggioErrore_Bootstrap(erroriNelSubmit, "DIV_Messaggi");
                        DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();
                    },null, true);

                $("#id_dialog_togli_autorizzazione").remove();

            },
        }]
    });
})

//Al change della Dropdown Autorizzato della griglia Categoria_TipologiaXUtenti viene cambiato il campo Autorizzato
function Salva_Modifiche_Autorizzato(objGriglia) {

    let modelCategTipoXUt = kendoEscapeOggetto(objGriglia);
    let piva = $(cIdPiva).val();

    if (piva === null || piva === "" || piva === undefined)
        return;
    let param = "{piva: '" + piva + "' ,modelutenti:'' , modelcategoriatipologia:'', modelcategoriatipologiaxutenti:'" + modelCategTipoXUt+"'}";

    ajaxAgronicaSync(indirizzohttp + "/Modifica_Griglia_Categoria_TipologiaXUtenti",
        param,false,
        function (risposta) {

            if (risposta.RispostaOK) {
                //Rileggo il datasource della griglia Categoria/Tipologia Utenti
                let gridCategTipxUtenti = $("#categtipodocxut_UC_griglia_CategTipxUtenti").data("kendoGrid"); 
                if (gridCategTipxUtenti !== undefined && gridCategTipxUtenti !== "") {
                    //Utilizzo il setDatasource per aggiornare i filtri della colonna se precedentemente erano
                    //stati selezionati.
                    let vecchio_ds = gridCategTipxUtenti.dataSource;

                    let ds = new kendo.data.DataSource({ data: [] });
                    gridCategTipxUtenti.setDataSource(ds);

                    gridCategTipxUtenti.setDataSource(vecchio_ds);
                    gridCategTipxUtenti.dataSource.read();

                    DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();

                }
            }
            else {
                erroriNelSubmit = risposta.Errore;
                MessaggioErrore_Bootstrap(erroriNelSubmit, "DIV_Messaggi");
            }
        }, null, null, true);

    Righe_Griglia_Utenti_CategTipolDocumentoXUtenti = "";
    Righe_Griglia_CategoriaTipologia_CategTipolDocumentoXUtenti = "";
}

//Cancella la riga selezionata
function Cancella_Griglia_CategTipxUtenti(rigaEliminata) {
    let modelCategTipoXUt = kendoEscapeOggetto(rigaEliminata);
    let piva = $(cIdPiva).val();

    if (piva === null || piva === "" || piva === undefined)
        return;
    let param = "{piva: '" + piva + "' , modelcategoriatipologiaxutenti:'" + modelCategTipoXUt + "'}";

    ajaxAgronica(indirizzohttp + "/Cancella_Griglia_Categoria_TipologiaXUtenti",
        param,
        function (risposta) {

            if (risposta.RispostaOK) {
                //Rileggo il datasource della griglia Categoria/Tipologia Utenti
                let gridCategTipxUtenti = $("#categtipodocxut_UC_griglia_CategTipxUtenti").data("kendoGrid");
                if (gridCategTipxUtenti !== undefined && gridCategTipxUtenti !== "") {
                    //Utilizzo il setDatasource per aggiornare i filtri della colonna se precedentemente erano
                    //stati selezionati.
                    let vecchio_ds = gridCategTipxUtenti.dataSource;

                    let ds = new kendo.data.DataSource({ data: [] });
                    gridCategTipxUtenti.setDataSource(ds);

                    gridCategTipxUtenti.setDataSource(vecchio_ds);
                    gridCategTipxUtenti.dataSource.read();

                    DeselezionaTutteleRigheGriglieUtentieCategoriaTipologia();
                }
            }
            else {
                erroriNelSubmit = risposta.Errore;
                MessaggioErrore_Bootstrap(erroriNelSubmit, "DIV_Messaggi");
            }
        }, null, null, true);
}