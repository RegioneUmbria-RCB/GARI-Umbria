//-----------------------------------------------------------------------------------------------------------------------------------
//KENDO
//-----------------------------------------------------------------------------------------------------------------------------------

function popolaGrigliaScadenze(IDControllo, selezioneMultipla) {

    var templatePulsanti = "";
    if (UtenteAbilitatoScrittura) {

        if ($(cAllegato_Permesso_Cancellazione).val() === "True") {

            templatePulsanti = [
                {
                    command: {
                        template: "<span class='fa fa-2x fa-pencil-square-o edit_elem' title='" + TraduzioneMultiResx(scadListaResx, "Modifica", "Modifica") + "' onclick=modificaElemento(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-2x fa-trash-o del_elem' title='" + TraduzioneMultiResx(scadListaResx, "RisorsaCancella", "cancella") + "' onclick=confermaEliminaElemento(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-2x fa-info info_elem' title='Info' onclick=infoElemento(this.closest('tr'),this.closest('.k-grid'))></span>"
                    }, title: TraduzioneMultiResx(scadListaResx, "Azioni", "Azioni")//, widthfisso: true //width: "135px" - no style in span
                }
            ];
        } else {
            templatePulsanti = [
                {
                    command: {
                        template: "<span class='fa fa-2x fa-pencil-square-o edit_elem' title='" + TraduzioneMultiResx(scadListaResx, "Modifica", "Modifica") + "' onclick=modificaElemento(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-2x fa-info info_elem' title='Info' onclick=infoElemento(this.closest('tr'),this.closest('.k-grid'))></span>"
                    }, title: TraduzioneMultiResx(scadListaResx, "Azioni", "Azioni")//, widthfisso: true //width: "135px" - no style in span
                }
            ];
        }
    } else {
        templatePulsanti = [
            {
                command: {
                    template: "<span class='fa fa-2x fa-info info_elem' title='Info' onclick=infoElemento(this.closest('tr'),this.closest('.k-grid'))></span>"
                }, title: TraduzioneMultiResx(scadListaResx, "Azioni", "Azioni")//, widthfisso: true //width: "135px" - no style in span
            }
        ];
    }



    var funzioniCRUD = { funzioneRead: kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };

    var idModel = "ID_Elenco";
    var campiKendoModel = kReadValorizzazione_mod();
    var colonneKendoGrid = kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {};

    if (bcheckstoricoabilitato == false) {

        var parametriKendoGrid = {
            columnMenu: true,
            pdf: false,
            groupable: false,
            salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
            reorderable: true,
            pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
            colonneCustomKendoGrid: templatePulsanti
        };
    } else {
        var parametriKendoGrid = {
            columnMenu: true,
            pdf: false,
            groupable: false,
            reorderable: true,
            pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
            colonneCustomKendoGrid: templatePulsanti
        };
    }

    if (selezioneMultipla !== undefined && selezioneMultipla == true) {

        funzioniCRUD.checkBoxFunction = kEventoSelezionaRiga;

        parametriKendoGrid.toolbarCommands = [];

        if ($(cAllegato_Permesso_Storicizzazione).val() === "True") {
            parametriKendoGrid.toolbarCommands.push("templateBtnStoricoSI");
            parametriKendoGrid.toolbarCommands.push("templateBtnStoricoNO");
        }

        if ($(cAllegato_Permesso_Cancellazione).val() === "True") {
            parametriKendoGrid.toolbarCommands.push("templateBtnEliminazioneMassiva");
        }

        if ($(cAllegato_Permesso_Cancellazione).val() === "True" && $(cAllegato_Permesso_Storicizzazione).val() === "True") {
            parametriKendoGrid.toolbarCommands.push("templateBtnScaricaAllegatiSelezionati");
        }

        if (workflowAbilitato && $(cAllegato_Validazione).val() === "True") {
            parametriKendoGrid.toolbarCommands.push("templateBtnAvanzamentoStato");
        }

    } else if ($(cSito_Provenienza).val() == '7') { //AUDIT

        parametriKendoGrid.toolbarCommands = ["templateBtnScaricaAllegatiAudit"];

        if (workflowAbilitato && $(cAllegato_Validazione).val() === "True") {
            parametriKendoGrid.toolbarCommands.push("templateBtnAvanzamentoStato");
            funzioniCRUD.checkBoxFunction = kEventoSelezionaRiga;
        }

        if (workflowAbilitato && $(cAllegato_Permesso_Storicizzazione).val() === "True") {
            parametriKendoGrid.toolbarCommands.push("templateBtnStoricoSI");
            parametriKendoGrid.toolbarCommands.push("templateBtnStoricoNO");
        }


    }

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe, funzioneDaChiamareDopoEdit: onEditRighe };
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

    //Visualizzazione Validazione
    if ($(cAllegato_Permesso_Cancellazione).val() === "True" || $(cAllegato_Permesso_Storicizzazione).val() === "True") {
        $("#GestioneStorico").show();
    }

    //Visualizzazione filtri validazione
    if (workflowAbilitato) {
        $("#id_validazione").hide()
        $("#id_ChkValidazione").hide()
    }

}

function GestionePannelli() {
    var listaPnl = "";
    if ($(cModalita).val() == "doc") {
        listaPnl = ["pnlScadenze"];
        $("#pnlDocumenti").show();
    } else {
        listaPnl = ["pnlDocumenti"];
        $("#pnlScadenze").show();

    }
    for (var i in listaPnl) {
        $('#' + listaPnl[i]).hide();
    }
}

function CmbAzienda_change(options) {

    var piva = KendoDDL("ddlAzienda").dataItem().piva;

    if (piva == "") {
        if (UtenteAbilitatoScrittura) {
            Elenco_Aree = Elenco_Aree_Riempi(piva);
        } else {
            Elenco_Aree = "";
        }

        Elenco_Tipologie = "";
    } else {
        Elenco_Aree = Elenco_Aree_Riempi(piva);
    }

    $("#cmbArea").data("kendoDropDownList").dataSource.read();
    if (KendoDDL("cmbArea").dataSource._data.length === 1) {
        Set_KendoDDLValue("cmbArea", KendoDDL("cmbArea").dataSource._data[0].ID_Area);
        CmbArea_change();
    }

    //E' stata cambiata l'azienda con lo switch 'Azienda Corrente Attivo' --> lo disabilito
    if (getKendoSwitch("chkAziendaCorrente") && Get_KendoDDLValue("ddlAzienda") != currentPiva)
        setKendoSwitch('chkAziendaCorrente', false)

}

function CmbArea_change(options) {
    var id_area = 0;
    if (KendoDDL("cmbArea").dataItem() !== undefined) {
        if (KendoDDL("cmbArea").dataSource._data.length > 0) {
            id_area = KendoDDL("cmbArea").dataItem().ID_Area;
        }
    }

    if (id_area == 0) {
        Elenco_Tipologie = "";
    } else {
        Elenco_Tipologie = Elenco_Tipologie_Riempi(id_area);
    }

    $("#cmbTipologia").data("kendoMultiSelect").dataSource.read();
}

function RiempicmbArea(options) {
    options.success(Elenco_Aree);
}

function RiempicmbTipologia(options) {
    options.success(Elenco_Tipologie);
}

//Anna 29/04/22: aggiunti campi al filtro di ricerca
function RiempicmbValidazione(options) {
    options.success(Validazioni_Filtro);
}

function RiempiDDLStorico(options) {
    options.success(Elenco_Storico);
}

function apriFormDialog(url) {
    $("#PaginaGeneric").attr("src", url);
    $("#iFrameGeneric").modal('toggle');
}

function chiudidialog() {
    $('#dialog').modal('hide');
}

function onEditRighe(e) {
    //se clicco sulla validazione in caso di assenza di documento --> chiuso la cella
    var fieldName = e.container.find("input").attr("name");
    var grid = $("#tabella_scadenze").data("kendoGrid");

    if (fieldName == "Validazione_Flag" && parseInt(e.model.Validazione_Flag) == -100) {

        kendo.alert(TraduzioneMultiResx(scadListaResx, "DocumentoAssentePerRegistrazione", "Documento assente per questa registrazione."));
        grid.closeCell();

    } else {
        //Controllo Switch
        if (!getKendoSwitch("ChkValidazione")) {
            if ($(cSito_Provenienza).val() !== '7')
                kendo.alert(TraduzioneMultiResx(scadListaResx, "AttivareValidazioneAttiva", "Attivare la modalità 'Validazione Attiva'."));
            grid.closeCell();
        }
    }
}

function kReadValorizzazione_rows(options) {
    var data = $('#hdKendo_Valorizzazione').val();

    jSonParsed_Kendo = JSON.parse(data);

    var risultato = jSonParsed_Kendo.kendo_rows.filter(function (x) { return x.Duplicato === "0"; })

    risultato.sort((a, b) => (a.Data_Scadenza_Data > b.Data_Scadenza_Data ? 1 : -1))

    options.success(risultato);
}

function kReadValorizzazione_col() {
    var data = $('#hdKendo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    if ($(cAllegato_Permesso).val() === "True") {
        jSonParsed_Kendo.kendo_columns.splice(0, 0,
            {
                field: "Allegati_documenti_nomefile", title: TraduzioneMultiResx(scadListaResx, "NomeAllegato", "Nome allegato"), filterable: { multi: true, search: true }
            });

        jSonParsed_Kendo.kendo_columns.splice(0, 0, {

            template: function (dataItem) {
                if (dataItem.Allegati_documenti_nomefile !== "") {
                    let icon = CreaIconaDownloadDocumento(dataItem.Allegati_documenti_estensione);
                    return "<span class='fa fa-2x " + icon + " ' onclick=Leggi_Doc_Allegato(" + dataItem.Allegati_Documenti_Cod + "," + Boolean(parseInt(dataItem.CompressoDaGIAS)) + ")></span>";
                } else {
                    return "";
                }
            },
            title: TraduzioneMultiResx(scadListaResx, "ApriDocumento", "Apri Doc.")
        });
    }

    workflowAbilitato = !(jSonParsed_Kendo.kendo_columns.find(x => x.title === 'Stato Attuale') === undefined);
    if ($(cAllegato_Validazione_Visibilita).val() === "True" && !workflowAbilitato) {
        jSonParsed_Kendo.kendo_columns.splice(0, 0, {
            field: "Validazione_Des", title: TraduzioneMultiResx(scadListaResx, "Validazione", "Validazione"), editor: validazione_Template, filterable: { multi: true, search: true }
        });
    }

    return jSonParsed_Kendo.kendo_columns;
}

function validazione_Template(container, options) {
    creaDropDownEditor(container, "Validazione_Des", "Validazione_Flag", Elenco_Validazioni, changeValidazione);
}

function changeValidazione(e) {

    //Aggiornamento Validazione         
    var dataItem = e.sender.dataItem();
    var grid = $("#tabella_scadenze").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");


    model.Validazione_Flag = dataItem.Validazione_Flag;
    model.Validazione_Des = dataItem.Validazione_Des;

    var allegati_documenti_cod = model.Allegati_Documenti_Cod;
    var validazione_flag = model.Validazione_Flag;

    //chiamo il web service
    var strObjJSON = JSON.stringify({
        "allegati_documenti_cod": allegati_documenti_cod,
        "validazione_flag": validazione_flag
    });

    var parametri = JSON.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "strObjJSON": strObjJSON });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert.asmx/Modifica_Validazione",
        parametri,
        false,
        function (risposta) {

        }, null);

    kendoFastRedrawRow(grid, row);
}

function kReadValorizzazione_mod() {
    var data = $('#hdKendo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function onDataBoundRighe(e) {

    nascondiPulsanti("#" + e.sender.element[0].id, e);

    var griglia = $('#' + e.sender.element[0].id).data("kendoGrid");

    griglia.autoFitColumn(0);

    coloraRigheAlert("#tabella_scadenze", e);

    var wrapper = griglia.wrapper;
    var header = wrapper.find(".k-grid-header");

    function resizeFixedRigheTestata() {
        var wrapperWidth = wrapper.width();
        if (wrapperWidth !== 0) {
            var paddingRight = parseInt(header.css("padding-right"));
            header.css("width", wrapperWidth - paddingRight);
        } else {
            // Nel caso l'evento venga eseguito mentre la griglia è nascosta, imposto una variabile per ricalcolare correttamente la width
            // alla prima occorrenza dell'evento "scroll" in quanto più frequente
            header.css("width", "auto");
            header.data("fix_width", 1);
        }
    }

    function scrollFixedRigheTestata() {
        // Nel caso l'evento venga eseguito mentre la griglia è nascosta, rimuovo la classe
        var wrapperHeight = wrapper.height();

        if (header.data("fix_width") === 1 && wrapperHeight !== 0) {
            var paddingRight = parseInt(header.css("padding-right"));
            header.css("width", wrapper.width() - paddingRight);
            header.data("fix_width", 0);
        }

        var offset = $(this).scrollTop(),
            tableOffsetTop = wrapper.offset().top,
            tableOffsetBottom = tableOffsetTop + wrapperHeight - header.height();

        if (offset < tableOffsetTop || offset > tableOffsetBottom || wrapperHeight === 0) {
            header.removeClass("fixed-header");
        } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !header.hasClass("fixed")) {
            header.addClass("fixed-header");
        }

    }

    resizeFixedRigheTestata();

    $(window).resize(resizeFixedRigheTestata);
    $(window).scroll(scrollFixedRigheTestata);

    //Anna 05/05/22 Autofit columns
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].width === undefined)
            grid.autoFitColumn(i);
    }
}

//tod DOCUMENTALE
function nascondiPulsanti(grid_elem, eventArgs) {

    var grid = $(grid_elem).data('kendoGrid');
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);
        var valido = workflowAbilitato ? (dataItem.Stato_Attuale == 401 || dataItem.Stato_Attuale == 403 || dataItem.Stato_Attuale == 405) : (dataItem.Validazione_Flag == 1 || dataItem.Validazione_Flag == 2)

        if (UtenteAbilitatoScrittura) {

            if ($(cAllegato_Permesso_Cancellazione).val() === "True") {

                if (valido || parseInt(dataItem.Autorizzazione) !== 1 || utenteAbilitatoModificaUMA(dataItem.Pratica_Stato_Cod, dataItem.Avanzamento_Richiesta) == false) {
                    $(this.querySelector(".del_elem")).hide();
                }

            }

            if (valido || parseInt(dataItem.Autorizzazione) !== 1 || utenteAbilitatoModificaUMA(dataItem.Pratica_Stato_Cod, dataItem.Avanzamento_Richiesta) == false) {
                $(this.querySelector(".edit_elem")).hide();
            } else {
                $(this.querySelector(".info_elem")).hide();
            }
        }

    });

    if (items.length == 0) {
        $("#btn_ScaricaAllegatiSelezionati").hide()
        $("#btn_ScaricaAllegatiAudit").hide()
    } else {
        $("#btn_ScaricaAllegatiSelezionati").show()
        $("#btn_ScaricaAllegatiAudit").show()
    }
}

function utenteAbilitatoModificaUMA(Pratica_Stato_Cod, Avanzamento_Richiesta) {

    var abilitato = false;

    switch (parseInt(Pratica_Stato_Cod)) {

        case 0:

            //no UMA
            abilitato = true;
            break;

        case 2001:

            if (parseInt(Avanzamento_Richiesta) == 0 && $(cAllegato_Permesso_Richiesta_Modifica).val() === "True") {
                abilitato = true;
            }

            if (parseInt(Avanzamento_Richiesta) == 1 && $(cAllegato_Permesso_Rendcontazione_Modifica).val() === "True") {
                abilitato = true;
            }

            break;

        case 2002:

            if (parseInt(Avanzamento_Richiesta) == 0 && $(cAllegato_Permesso_Approvazione_Richiesta_Modifica).val() === "True") {
                abilitato = true;
            }

            if (parseInt(Avanzamento_Richiesta) == 1 && $(cAllegato_Permesso_Approvazione_Rendcontazione_Modifica).val() === "True") {
                abilitato = true;
            }

            break;

        case 2005:

            abilitato = false;
            break;

        case 2007:

            abilitato = false;
            break;

        default:

            abilitato = true;
            break;
    }

    return abilitato;
}

//INFO ELEMENTO
function infoElemento(tr_elem, grid_elem) {

    if ($(cArea_Provenienza).val() == 0) {
        //Anna Salvataggio filtri/personalizzazioni griglia
        Salva_Filtri();
    }

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    //Se cerco di aprire un patentino senza allegato, blocco.
    if (datiRiga.ID_Tipologia == -1 && datiRiga.Allegati_documenti_nomefile == "") {
        var msg = TraduzioneMultiResx(scadListaResx, ("ImpossibileAprirePatentinoVaiAnagraficaContattoX"), "Non è possibile aprire questo documento, perchè privo di allegato. <br> Accedi all'anagrafica del contatto {0} per visualizzare i dettagli del patentino.").format(datiRiga.Contatto)
        kendo.alert(msg)
        return false
    }

    var ID_Alert_Entita = datiRiga.ID_Alert_Entita;
    var ID_Elenco = datiRiga.ID_Elenco;
    var param = ""

    if ($(cRichiesta_Cod).val() !== "0") {
        param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 0, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'Piva': $(cPiva).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Richiesta_Cod': $(cRichiesta_Cod).val() });
    } else if ($(cSito_Provenienza).val() == '7') { //AUDIT
        param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 0, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'Piva': $(cPiva).val(), 'sito_provenienza': $(cSito_Provenienza).val(), 'cod_Documenti': $(cxFiltroDocumenti).val() });
    }
    //ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
    else if ($(cCod_Contatto).val() !== '') { //Patentini da Edit_Contatto
        param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 0, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'Piva': $(cPiva).val(), 'sito_provenienza': $(cSito_Provenienza).val(), 'Cod_Contatto': $(cCod_Contatto).val() });
    }
    else if ($(cAnalisi_Testata_Cod).val() !== '') {
        param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 0, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'Piva': $(cPiva).val(), 'sito_provenienza': $(cSito_Provenienza).val(), 'Analisi_Testata_Cod': $(cAnalisi_Testata_Cod).val() });
    }
    else {
        param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 0, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'Piva': $(cPiva).val() });
    }

    window.location = "./Scad_CreaModificaItem.aspx?scadstr=" + param;

}

//MODIFICA ELEMENTO
function modificaElemento(tr_elem, grid_elem) {

    if ($(cArea_Provenienza).val() == 0) {
        //Anna Salvataggio filtri/personalizzazioni griglia
        Salva_Filtri();
    }

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    //Se cerco di aprire un patentino senza allegato, blocco.
    if (datiRiga.ID_Tipologia == -1 && datiRiga.Allegati_documenti_nomefile == "") {
        var msg = TraduzioneMultiResx(scadListaResx, ("ImpossibileAprirePatentinoVaiAnagraficaContattoX"), "Non è possibile aprire questo documento, perchè privo di allegato. <br> Accedi all'anagrafica del contatto {0} per visualizzare i dettagli del patentino.").format(datiRiga.Contatto)
        kendo.alert(msg)
        return false
    }

    var ID_Alert_Entita = datiRiga.ID_Alert_Entita;
    var ID_Elenco = datiRiga.ID_Elenco;
    //<% --$("#<%= Master.lblDescrGeneric_ClientID %>").text("Modifica Scadenza"); --%>
    var param = ""

    if ($(cRichiesta_Cod).val() !== "0" || $(cAnalisi_Testata_Cod).val() !== "0" || $(cIdAgenda).val() !== "0" || $(cRicetta).val() !== "0" || $(cCod_Contatto).val() !== "" || $(cMac_Cod).val() !== "0") {
        if ($(cRichiesta_Cod).val() !== "0") {
            param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Richiesta_Cod': $(cRichiesta_Cod).val(), 'Piva': $(cPiva).val() });
        }
        else if ($(cAnalisi_Testata_Cod).val() !== "0") {
            param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Analisi_Testata_Cod': $(cAnalisi_Testata_Cod).val(), 'Piva': $(cPiva).val() });
        }
        else if ($(cIdAgenda).val() !== "0") {
            param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Id_Agenda': $(cIdAgenda).val(), 'Piva': $(cPiva).val() });
        }
        else if ($(cRicetta).val() !== "0") {
            param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Ricetta_Operazione_Cod': $(cRicetta).val(), 'Piva': $(cPiva).val() });
        }
        //ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
        else if ($(cCod_Contatto).val() !== "") {
            param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Cod_Contatto': $(cCod_Contatto).val(), 'Piva': $(cPiva).val() });
        }
        // DCA20260122 - passo il Mac_Cod in queryString se questo è diverso da 0
        else if ($(cMac_Cod).val() !== "0") {
            param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Mac_Cod': $(cMac_Cod).val(), 'Piva': $(cPiva).val() });
        }
    } else if ($(cSito_Provenienza).val() == '7') { //AUDIT
        param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Piva': $(cPiva).val(), 'sito_provenienza': $(cSito_Provenienza).val(), 'cod_Documenti': $(cxFiltroDocumenti).val() });
    } else {
        param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'type': $(cModalita).val(), 'area_provenienza': $(cArea_Provenienza).val(), 'Piva': $(cPiva).val() });
    }

    window.location = "./Scad_CreaModificaItem.aspx?scadstr=" + param;
}

//CONFERMA CANCELLAZIONE DELL'ELEMENTO
function confermaEliminaElemento(tr_elem, grid_elem) {

    var dlg = $("<div></div>").kendoConfirm({
        content: TraduzioneMultiResx(scadListaResx, "SeiSicuroDiEliminareQuestoElemento", "Sei sicuro di voler eliminare l'elemento?"),
        messages: { okText: TraduzioneMultiResx(scadListaResx, "Si", "Sì"), cancel: TraduzioneMultiResx(scadListaResx, "No", "No") },
        title: TraduzioneMultiResx(scadListaResx, "Conferma", "Conferma")
    }).data("kendoConfirm");

    dlg.result.done(function () { eliminaElemento(tr_elem, grid_elem); });
    dlg.open();

};

//CANCELLAZIONE DELL'ELEMENTO
function eliminaElemento(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var id_elenco = datiRiga.ID_Elenco;
    var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti, 'id_elenco': id_elenco });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Cancella", param,
        function (risposta) {

            let risp = risposta.RispostaStringa;
            if ($(cModalita).val() !== '') {
                risp = TraduzioneMultiResx(scadListaResx, "DocumentoCancellatoCorrettamente", "Documento cancellato correttamente");
            }

            $("<div></div>").kendoAlert({ content: risp, title: TraduzioneMultiResx(scadListaResx, "Salvataggio", "Salvataggio") }).data("kendoAlert").open();
            //kendo.alert(risposta.RispostaStringa);

            //Ricarico le NC
            eseguiRicercaScadenze();
        }, null);

    if ($(cArea_Provenienza).val() == 0) {
        //Anna Salvataggio filtri/personalizzazioni griglia
        Salva_Filtri();
    }
}




//IMPORTA SCADENZE
function importaScadenzeDialog() {

    if ($('#importaScadenzeDialog').data("kendoDialog") === undefined) {

        var content = '<p>' + TraduzioneMultiResx(scadListaResx, 'ScegliereQualiScadenzeImportare', 'Scegliere quali scadenze importare:') + '</p>';
        // content += '<div><input type="checkbox" id="ID-1" value="-1" class="k-checkbox" checked="checked"><label class="k-checkbox-label" for="ID-1">Patentino Trattamenti</label></div>';
        content += '<div><input type="checkbox" id="ID-2" value="-2" class="k-checkbox" checked="checked"><label class="k-checkbox-label" for="ID-2">' + TraduzioneMultiResx(scadListaResx, 'TaraturaUgelli', 'Taratura Ugelli') + '</label></div>';
        content += '<div><input type="checkbox" id="ID-4" value="-4" class="k-checkbox" checked="checked"><label class="k-checkbox-label" for="ID-4">' + TraduzioneMultiResx(scadListaResx, 'AnalisiDelTerreno', 'Analisi del Terreno') + '</label></div>';
        content += '<div><input type="checkbox" id="ID-26-27" value="-26|-27" class="k-checkbox" checked="checked"><label class="k-checkbox-label" for="ID-26-27">' + TraduzioneMultiResx(scadListaResx, 'AffittiParticelle', 'Affitti Particelle') + '</label></div>';

        $('#importaScadenzeDialog').kendoDialog({
            title: TraduzioneMultiResx(scadListaResx, "ImportazioneDati", "Importazione dati"),
            closable: false,
            modal: true,
            content: content,
            actions: [
                { text: TraduzioneMultiResx(scadListaResx, "Annulla", "Annulla") },
                { text: TraduzioneMultiResx(scadListaResx, "Importa", "Importa"), primary: true, action: importaScadenze }
            ]
        });

    } else {
        $('#importaScadenzeDialog').data("kendoDialog").open();
    }
}

//CARICA DATI APP
function CaricaDatiApp() {

    $("#BtnImportaApp").addClass("disabled");

    var parametri = JSON.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert.asmx/Carica_DatiApp",
        parametri,
        false,
        function (risposta) {
            $("#BtnImportaApp").removeClass("disabled");
            if (risposta.RispostaOK) {
                if (risposta.RispostaStringa !== "") {
                    kendo.alert(risposta.RispostaStringa);
                } else {
                    kendo.alert("Non ci sono documenti da importare");
                }
            } else {
                kendo.alert("Si è verificato un problema durante l'importazione dei dati<br><br>" + risposta.Errore);
            }
        }, null);

    eseguiRicercaScadenze();
}

function CaricaDocumentiApp() {

    $("#BtnImportaApp").addClass("disabled");

    ajaxAgronica(pathCoreWS + "GiasApp/SincroDatiApp.asmx/CaricaDatiApp",

        kendo.stringify({ "tipo": "40", "piva": "", "objP_super_server": objP_super_server, "objP_server": objP_server, "objP_utenti": objP_utenti }),

        function (risposta) {

            $("#BtnImportaApp").removeClass("disabled");

            if (risposta.RispostaOK) {
                if (risposta.RispostaStringa !== "") {
                    kendo.alert(risposta.RispostaStringa);
                    //Ricarico dati griglia
                    eseguiRicercaScadenze();
                } else {
                    kendo.alert("Non ci sono dati da importare");
                }
            } else {
                kendo.alert("Si è verificato un problema durante l'importazione dei dati<br><br>" + risposta.Errore);
            }

        }, null);

}

function kEventoSelezionaRiga(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#tabella_scadenze").data("kendoGrid"),
        dataItem = grid.dataItem(row);


    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}

function coloraRigheAlert(grid_elem, eventArgs) {

    var grid = $(grid_elem).data('kendoGrid');
    var items = eventArgs.sender.items();

    items.each(function (index) {

        var dataItem = grid.dataItem(this);

        if (dataItem.ChkStorico == 1) {
            //Coloro gli indici riservati
            this.className += " kendoRiga_Arancione";
        };

        let fieldValidazione
        if (workflowAbilitato)
            fieldValidazione = dataItem.Descrizione_Stato
        else
            fieldValidazione = dataItem.Validazione_Des

        //Colori validazione
        switch (fieldValidazione) {
            case "Documento valido":
                this.className = " kendoRiga_Verde";
                break;
            case "Documento non valido":
                this.className = " kendoRiga_Rossa";
                break;
            case "Documento valido autocontrollo":
                this.className = " kendoRiga_VerdeScuro";
                break;
            case "Documento non valido autocontrollo":
                this.className = " kendoRiga_Rossa";
                break;
            case "Documento valido ufficio":
                this.className = " kendoRiga_Verde";
                break;
            case "Documento non valido ufficio":
                this.className = " kendoRiga_Rossa";
                break;
            default:
        };

        if (dataItem.ChkStorico == 1) {
            //Coloro gli indici riservati
            this.className = " kendoRiga_Arancione";
        };

    });

}



//CONFERMA CANCELLAZIONE MASSIVA
function confermaEliminazioneMassivaElemento() {

    var elenco = ElencoSelezioneCancellazione();
    var numero = 0;

    if (elenco == "") {
        alert("Nessun elemento selezionato valido per la cancellazione");
    } else {

        numero = parseInt(elenco.split(',').length) - 1;

        var dlg = $("<div></div>").kendoConfirm({
            content: TraduzioneMultiResx(scadListaResx, "SeiSicuroDiEliminareGliElementiSelezionati", "Sei sicuro di voler eliminare " + numero + " elementi selezionati?"),
            messages: { okText: TraduzioneMultiResx(scadListaResx, "Si", "Sì"), cancel: TraduzioneMultiResx(scadListaResx, "No", "No") },
            title: TraduzioneMultiResx(scadListaResx, "Conferma", "Conferma")
        }).data("kendoConfirm");

        dlg.result.done(function () { eliminazioneMassivaElemento(elenco); });
        dlg.open();
    };
}

//CANCELLAZIONE MASSIVA
function eliminazioneMassivaElemento(elenco) {

    var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti, 'elenco': elenco });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/CancellaMassivo", param,
        function (risposta) {
            $("<div></div>").kendoAlert({ content: risposta.RispostaStringa, title: TraduzioneMultiResx(scadListaResx, "Cancellazione", "Cancellazione") }).data("kendoAlert").open();

            //Ricarico le NC
            eseguiRicercaScadenze();

        }, null);
};

//CONFERMA STORICIZZAZIONE
function confermaStoricizzazione(valore) {

    if ($(cArea_Provenienza).val() == 0) {
        //Anna Salvataggio filtri/personalizzazioni griglia
        Salva_Filtri();
    }

    var elenco = ElencoSelezione(valore);
    var numero = 0;
    var str = "";
    var str2 = "";

    if (parseInt(valore) == 1) {
        str = "Storicizzare"
        str2 = "Nessun elemento selezionato valido per la storicizzazione"
    } else {
        str = "Annullare La Storicizzazione Di"
        str2 = "Nessun elemento selezionato è storicizzato e quindi valido per l'annullamento"
    }

    if (elenco == "") {

        alert(str2);

    } else {

        numero = parseInt(elenco.split(',').length) - 1;

        var dlg = $("<div></div>").kendoConfirm({
            content: TraduzioneMultiResx(scadListaResx, "SeiSicuroDi" + str + "ElementiSelezionati", "Sei sicuro di voler " + str.toLowerCase() + " " + numero + " elementi selezionati?"),
            messages: { okText: TraduzioneMultiResx(scadListaResx, "Si", "Sì"), cancel: TraduzioneMultiResx(scadListaResx, "No", "No") },
            title: TraduzioneMultiResx(scadListaResx, "Conferma", "Conferma")
        }).data("kendoConfirm");

        dlg.result.done(function () { Storicizzazione(valore, elenco); });
        dlg.open();

    };
}

function Storicizzazione(valore, elenco) {

    var param = kendo.stringify({ 'objP_server': objP_server, 'elenco': elenco, 'valore': valore });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Aggiorna_Storicizzazione", param,
        function (risposta) {

            if (valore == 1) {
                alert("Storicizzazione effettuata correttamente");
            } else {
                alert("Storicizzazione annullata correttamente");
            }

            //Ricarico le NC
            eseguiRicercaScadenze();

        }, null);
}

function ElencoSelezione(valore) {

    var grid = $("#tabella_scadenze").data("kendoGrid");
    var elenco = ""
    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {

        if (currentData[i].Selected == true && (currentData[i].ChkStorico !== valore || valore == -1)) {

            elenco += currentData[i].ID_Alert_Entita + ", ";

        }

    }

    return elenco;

}

function ElencoSelezioneCancellazione() {

    var grid = $("#tabella_scadenze").data("kendoGrid");
    var elenco = ""
    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {

        if (currentData[i].Selected == true) {
            var validato = workflowAbilitato ? currentData[i].Stato_Attuale == 401 || currentData[i].Stato_Attuale == 403 || currentData[i].Stato_Attuale == 405 : currentData[i].Validazione_Flag == 1 || currentData[i].Validazione_Flag == 2;

            //Controllo possibilità di cancellare l'elemento
            if (validato || parseInt(currentData[i].Autorizzazione) !== 1 || utenteAbilitatoModificaUMA(currentData[i].Pratica_Stato_Cod, currentData[i].Avanzamento_Richiesta) == false) {

                //Elemento Non Cancellabile
            } else {

                //Elenco per cancellazione --> ID_Elenco
                elenco += currentData[i].ID_Elenco + ", ";
            }
        }
    }

    return elenco;
}

async function ddlAzienda_Load(piva) {

    //$('#ddlAzienda').kendoDropDownList({
    //    filter: "contains",
    //    dataSource: {
    //        transport: {
    //            read: function (options) {
    //                RiempiDdlAzienda(options, piva);
    //            }
    //        }
    //    },
    //    dataTextField: "rag_soc",
    //    dataValueField: "piva",
    //    optionLabel: { "rag_soc": TraduzioneMultiResx(scadListaResx, "", "").toUpperCase(), "piva": "" },
    //    autoWidth: true,
    //    dataBound: ddlAzienda_OnDataBound
    //});

    //let optionLabel = { "rag_soc": TraduzioneMultiResx(scadListaResx, "", "").toUpperCase(), "piva": "" };

    let t = {
        read: function (options) {
            RiempiDdlAzienda(options, piva);
        }
    }
    creaKendoDropDownList(
        "ddlAzienda",
        t,
        "rag_soc", "piva",
        undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true).bind("change", CmbAzienda_change);
    KendoDDL("ddlAzienda").bind("dataBound", ddlAzienda_OnDataBound)

    // Se c'è solo un valore ed il parametro qualitativo è obbligatorio lo imposto in automatico
    if (KendoDDL("ddlAzienda").dataSource._data.length === 2) {
        await Set_KendoDDLValueVirtual("ddlAzienda", KendoDDL("ddlAzienda").dataSource._data[1].piva);
    }
}

function RiempiDdlAzienda(options, piva) {

    var chksoloattive = 0;
    if (getKendoSwitch("ChkSoloAttive")) {
        chksoloattive = 1;
    }

    var parametri = ""
    var pathCaricaCmb = "";

    if (piva !== undefined && piva !== null && piva !== "") {
        parametri = kendo.stringify({ "piva": piva });
        pathCaricaCmb = "./Scad_CreaModificaItem.aspx/LeggiRagione_Sociale";
    } else {
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "solo_aziende_attive": chksoloattive });
        pathCaricaCmb = pathCoreWS + "Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtenteCodiceSocio";
    }

    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            objVuoto = {
                "piva": "",
                "rag_soc": ""
            };

            risp.unshift(objVuoto);

            options.success(risp);

        }, null);
}

function ddlAzienda_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //Anna al momento da errore, non esiste onChange, commento to do.....
        ddlAzienda.onchange(); //forzo l'evento di onchange
    } else if ($("input[name$='hdPiva']").val() != "") {
        //KendoDDL("ddlAzienda").value($("input[name$='hdPiva']").val());
        Set_KendoDDLValue("ddlAzienda", $("input[name$='hdPiva']").val())
    }

    //if (getKendoSwitch("chkAziendaCorrente")) {
    //    if (currentPiva != '')
    //        if (ds.filter((ds) => ds.piva == currentPiva))
    //            Set_KendoDDLValue("ddlAzienda", currentPiva)
    //}

}

// #region KENDOUPLOAD

function windowCompressoDaGIAS(Allegati_Documenti_Cod) {
    if (UploadMultiploAllegatiAbilitato == true) {

        let win = $("#tmplWindowCompressoDaGias").html().trim();
        $("#containerWindowCompressoDaGias").append(win);

        win_CompressoDaGIAS = $("#winCompressoDaGIAS").kendoWindow({
            width: "640px",
            height: "80%",
            modal: true,
            draggable: false,
            title: Traduzione(scadListaResx, "VisualizzaAllegati", "Visualizza Allegati"),
            visible: true,
            closable: true,
            close: onClose,
            open: async function (e) {
                this.center();
                await getFileCompressoDaGIAS(Allegati_Documenti_Cod);
            }
        }).data("kendoWindow");


        $(".btn_scarica_file").click(
            function () {
                scaricaZip(false);
                //$("btn_scarica_file").enable(false);
                $(this).prop("disabled", true);
            }
        );

        $(".btn_scarica_ZIP").click(
            function () {
                scaricaZip(true);
            }
        );

    } else {
        initialFiles = [];
        listaDocumenti = [];
        creaScaricaZip(Allegati_Documenti_Cod)
    }
}

function creaScaricaZip(Allegati_Documenti_Cod) {
    getFileCompressoDaGIAS(Allegati_Documenti_Cod);
}

function onClose() {
    initialFiles = [];
    listaDocumenti = [];
    $("#winCompressoDaGIAS").data("kendoWindow").destroy();
}

function chiudiWindowCompressoDaGIAS() {
    $("#winCompressoDaGIAS").data("kendoWindow").close();
}

function getFileCompressoDaGIAS(Allegati_Documenti_Cod) {

    var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti, 'piva': '', 'allegati_documenti_cod': Allegati_Documenti_Cod });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_File_Allegato", param,
        async function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            if (risp !== undefined && risp !== "") {
                let fileDati = risp[0].File_Allegato_DB;
                let nomeFile = risp[0].Allegati_Documenti_NomeFile;
                let estensione = risp[0].Allegati_Documenti_Estensione;

                decompressaFile(fileDati);
                await carica_InitialFiles();
                if (UploadMultiploAllegatiAbilitato == true) {

                    $(".k-action-buttons").hide();
                    $("button[name='rimuoviElemento']").hide();
                    $(".k-dropzone").hide();

                } else {
                    console.log(listaDocumenti);
                    zippaFiles();
                }
            }
        }, null);

}

function inizializza_KendoUpload(files) {
    let $upload = $("#files");

    var upload = $upload.kendoUpload({

        //Per poter caricare > 1 allegato
        multiple: true,

        //Template personalizzato kendo upload
        template: kendo.template($('#fileTemplate').html()),

        //Proprietà obbligatoria per poter usare il trascinamento e pulsante Rimuovi Tutto
        async: {
            saveUrl: "save",
            removeUrl: "remove",
            autoUpload: false // = true --> ogni file viene caricato nel server / con la ajaxcall dichiarati nelle proprietà sopra
        },

        files: files
    }).getKendoUpload();

    return upload;
}

//DECOMPRESSIONE SE CompressoDaGIAS = TRUE
function decompressaFile(fileCompresso) {
    let binaryArray = _base64ToArrayBuffer(fileCompresso);
    let fileDecompresso = pako.inflate(binaryArray, { to: 'string' })
    imposta_ListaInitialFiles_da_FileDecompresso(JSON.parse(fileDecompresso));
}

//CONVERSIONE FILE COMPRESSO IN ARRAY BINARIO PER LA DECOMPRESSIONE
function _base64ToArrayBuffer(base64) {
    var binary_string = window.atob(base64).split(",");
    var len = binary_string.length;
    var bytes = new Uint8Array(len);
    for (var i = 0; i < len; i++) {
        bytes[i] = parseInt(binary_string[i]);
    }
    return bytes.buffer;
}

//AGGIUNTA DEI FILE DECOMPRESSI ALLA LISTA PER GLI INITIALFILES
function imposta_ListaInitialFiles_da_FileDecompresso(fileDecompresso) {

    for (i = 0; i < fileDecompresso.length; i++) {
        var allegato = new Object();
        allegato.name = fileDecompresso[i].nome;
        allegato.extension = fileDecompresso[i].estensione;
        allegato.size = fileDecompresso[i].file.length;
        allegato.rawFile = new File([fileDecompresso[i].file], fileDecompresso[i].nome);

        allegato.uid = generaFileUID();

        initialFiles.push(allegato);
    }
}

//GENERO L'UID DEL FILE PER POTER USARE IL REMOVE SU SINGOLO ELEMENTO
function generaFileUID() {

    if (initialFiles_UID === undefined || initialFiles_UID === null) {
        initialFiles_UID = 1;
    }

    let fileUID = initialFiles_UID + "-uidFileCaricato"
    initialFiles_UID++;

    return fileUID;
}

//CARICAMENTO INITIALFILES NEL KENDOUPLOAD
async function carica_InitialFiles() {
    let upload = inizializza_KendoUpload([]);
    let sourceInput;

    if (UploadMultiploAllegatiAbilitato == true) {
        for (let i = 0; i < initialFiles.length; i++) {
            var name = $.map(initialFiles[i], function (item) {
                return item.name;
            }).join(", ");

            sourceInput = upload._module.element.find("input[type='file']").last();
            let dummy = [];
            dummy[0] = initialFiles[i];
            var file = upload._enqueueFile(name, { relatedInput: sourceInput, fileNames: dummy });
            upload._fileAction(file, "remove");
        }
    }

    await aggiungi_InitialFile_A_ListaDocumenti()
}

//AGGIUNTA INITIALFILES ALLA LISTADOCUMENTI
function aggiungi_InitialFile_A_ListaDocumenti() {
    return new Promise((resolve, reject) => {
        let arrProm = [];
        for (let i = 0; i < initialFiles.length; i++) {
            if (initialFiles[i]) {
                arrProm.push(leggiFile(initialFiles[i]));
            }
        }

        if (listaDocumenti.length > 0) {
            $(".k-action-buttons").hide();
            $(".k-dropzone").hide();
        }

        Promise.all(arrProm).then((values) => {
            resolve()
        });

    })
}

function leggiFile(file) {
    return new Promise((resolve, reject) => {
        var reader = new FileReader();
        reader.onload = function (readerEvt) {
            var binaryString = readerEvt.target.result;
            var allegato = new Object();

            allegato.nome = file.name;
            allegato.file = btoa(binaryString);

            let estensione = file.name.split('.');
            if (estensione.length > 0) {
                allegato.estensione = estensione[estensione.length - 1]
            } else {
                allegato.estensione = "";
                //To do 19/05/22.... alert messaggio estensione non valida :)
            }

            allegato.UID = file.uid;
            listaDocumenti.push(allegato);
            resolve();
        };

        reader.readAsBinaryString(file.rawFile);
    });
}

//CLICK ICONA DOCUMENTO
function Apri_Doc_Allegato(nomeFile) {
    let allegato = [];
    allegato = listaDocumenti.slice();

    //let allegatoFile = "";
    for (let i = 0; i < allegato.length; i++) {
        if (nomeFile === allegato[i].nome) {
            let UID = allegato[i].UID;
            let nome = allegato[i].nome
            let file = allegato[i].file;
            let estensione = allegato[i].estensione;

            if (UID.indexOf("-uidFileCaricato") > 0) {
                file = atob(file)
            }

            SaveAndOpenFileByteArray(nome, file, estensione);
            break;
        }
    }
}

//DIMENSIONE FILE PER DESCRIZIONE ALLEGATO
function grandezzaFile(bytes) {

    let result = "";
    let KB = Math.round((bytes / 1024) * 100) / 100;
    let MB = Math.round((KB / 1024) * 100) / 100;

    if (MB >= 1) {
        result = MB + " MB";
    } else if (KB >= 1) {
        result = KB + " KB";
    } else {
        result = bytes + " bytes";
    }

    return result;
}

function scaricaZip(scaricaZip) {
    if (scaricaZip == true) {
        zippaFiles();
    } else {
        for (i = 0; i < listaDocumenti.length; i++) {
            Apri_Doc_Allegato(listaDocumenti[i].nome);
        }
    }
};

async function zippaFiles() {
    let allegati = listaDocumenti.slice();

    if (UploadMultiploAllegatiAbilitato == false) {
        for (let i = 0; i < allegati.length; i++) {
            allegati[i].file = atob(allegati[i].file);
        }
    }

    var zip = new JSZip();
    deferreds = [];
    allegati.forEach(function (allegati) {
        if (UploadMultiploAllegatiAbilitato == false) {
            deferreds.push(zip.file(allegati.nome, (allegati.file), { base64: true }));
        } else {
            deferreds.push(zip.file(allegati.nome, atob(allegati.file), { base64: true }));
        }
    });

    //jszip 2.6.1
    var content = zip.generate({ type: "blob" });
    saveAs(content, allegati[0].nome.split(".")[0]);
}

// #endregion

// #region SCARICA ALLEGATI MULTIPLO

var ScaricaAllegati_listaErrori_FileNonTrovati = []
var ScaricaAllegati_listaDocumenti = []
var ScaricaAllegati_initialFiles = []

async function ScaricaAllegati(tipo) {

    //Resetto la lista
    ScaricaAllegati_listaDocumenti = []
    ScaricaAllegati_listaErrori_FileNonTrovati = []
    ScaricaAllegati_initialFiles = []

    //1 = SELEZIONATI
    // 2 = TUTTI (AUDIT)
    var tuttiDocumenti = (tipo == 2 ? true : false)

    var listaAllegatiCod = []
    var items = KendoGrid("tabella_scadenze").dataSource.data()



    WaitFrame.show()

    items.forEach(function (item) {
        var checked = Boolean(item.Selected)

        if (tuttiDocumenti || checked == true) {
            listaAllegatiCod.push(item.Allegati_Documenti_Cod)
        }
    }, this);

    if (listaAllegatiCod.length == 0) {
        kendo.alert("Non è stato selezionato nessun documento!")
        WaitFrame.hide()
        return
    }

    if (tipo == 1 && listaAllegatiCod.length > 50) {
        kendo.alert("Selezionare meno di 50 documenti.")
        WaitFrame.hide()
        return
    }

    await ScaricaAllegati_getListaDocumenti_da_File_Allegati(listaAllegatiCod)

    univocizzaFileName()
    aggiungiTagStoricizzatoInvalido()

    if (ScaricaAllegati_listaDocumenti.length > 0)
        ScaricaAllegati_zippaFiles(true);

    if (ScaricaAllegati_listaErrori_FileNonTrovati.length > 0)
        kendo.alert("Impossibile scaricare i seguenti allegati, file non trovati: <br>" + ScaricaAllegati_listaErrori_FileNonTrovati.join("<br>"))

    WaitFrame.hide()

}

function univocizzaFileName() {
    var counterNomi = []

    ScaricaAllegati_listaDocumenti.forEach(function (element, index, array) {
        let results = counterNomi.filter(item => item.nome === element.nome);

        if (results.length == 0) {
            counterNomi.push({ nome: element.nome, counter: 0 })
        } else {
            results[0].counter += 1
            inserisciNumeroIncrementaleNomeFile(array[index], results[0].counter)
        }
    })
}

function aggiungiTagStoricizzatoInvalido() {

    ScaricaAllegati_listaDocumenti.forEach(function (element, index, array) {
        if (element.CompressoDaGias) {
            let InvalidStoricizzato = ""

            if (workflowAbilitato) {
                if (element.validazione == 404 || element.validazione == 406)
                    InvalidStoricizzato += "[INVALID FILE]"
            } else {
                if (element.validazione == -1 || element.validazione == -2)
                    InvalidStoricizzato += "[INVALID FILE]"
            }

            if (element.storicizzato == 1)
                InvalidStoricizzato += "[ARCHIVED FILE]"

            if (InvalidStoricizzato !== "")
                array[index].nome = InvalidStoricizzato + " " + array[index].nome
        }
    })
}

function inserisciNumeroIncrementaleNomeFile(file, counter) {
    var idx = file.nome.lastIndexOf(".");
    if (idx > -1)
        file.nome = file.nome.substr(0, idx) + "(" + counter.toString() + ")" + file.nome.substr(idx);
}

async function ScaricaAllegati_getListaDocumenti_da_File_Allegati(listaAllegatiCod) {

    let risposta = await WS_getListaDocumenti_da_File_Allegati(listaAllegatiCod);

    let risp = JSON.parse(risposta.RispostaStringa);
    var documenti = risp.Dt
    ScaricaAllegati_listaErrori_FileNonTrovati = risp.errori

    if (documenti !== undefined && documenti !== "") {

        documenti.forEach(function (r) {
            var CompressoDaGias = Boolean(r.CompressoDaGIAS)

            var fileDati = r.File_Allegato_DB;
            var nomeFile = r.Allegati_Documenti_NomeFile;
            var estensione = r.Allegati_Documenti_Estensione;

            var storicizzato = r.ChkStorico;
            var validazione = workflowAbilitato ? r.Stato_Attuale : r.Validazione_Flag;

            if (CompressoDaGias) {
                ScaricaAllegati_decompressaFile(fileDati, storicizzato, validazione);
            } else {
                var allegato = new Object();
                allegato.nome = nomeFile;
                allegato.file = fileDati;
                allegato.estensione = estensione;

                allegato.uid = generaFileUID();

                allegato.CompressoDaGIAS = false

                ScaricaAllegati_listaDocumenti.push(allegato);
            }
        }, this);

        await ScaricaAllegati_aggiungi_InitialFile_A_ListaDocumenti()

    }
}

function WS_getListaDocumenti_da_File_Allegati(listaAllegatiCod) {
    return new Promise((resolve, reject) => {
        var param = kendo.stringify({
            'objP_server': objP_server,
            'objP_utenti': objP_utenti,
            'listaAllegatiCod': listaAllegatiCod.join(","),
            "fromAudit": ($(cSito_Provenienza).val() == '7' ? true : false)
        });

        ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_Lista_File_Allegati",
            param,
            function (risposta) {
                resolve(risposta);
            },
            null, //errore
            null, null, null,  //accessToken, gestioneWaitFrame, deferred,
            false //compressione --> FALSE, comprimere byte array degli allegati ne aumenta la dimensione
        );
    });
}

//DECOMPRESSIONE SE CompressoDaGIAS = TRUE
function ScaricaAllegati_decompressaFile(fileCompresso, storicizzato, validazione) {
    let binaryArray = _base64ToArrayBuffer(fileCompresso);
    let fileDecompresso = pako.inflate(binaryArray, { to: 'string' })
    ScaricaAllegati_imposta_ListaInitialFiles_da_FileDecompresso(JSON.parse(fileDecompresso), storicizzato, validazione);
}

//AGGIUNTA DEI FILE DECOMPRESSI ALLA LISTA PER GLI INITIALFILES
function ScaricaAllegati_imposta_ListaInitialFiles_da_FileDecompresso(fileDecompresso, storicizzato, validazione) {

    for (i = 0; i < fileDecompresso.length; i++) {
        var allegato = new Object();
        allegato.name = fileDecompresso[i].nome;
        allegato.extension = fileDecompresso[i].estensione;
        allegato.size = fileDecompresso[i].file.length;
        allegato.rawFile = new File([fileDecompresso[i].file], fileDecompresso[i].nome);

        allegato.uid = generaFileUID();
        allegato.CompressoDaGIAS = true

        allegato.storicizzato = storicizzato;
        allegato.validazione = validazione;

        ScaricaAllegati_initialFiles.push(allegato);
    }
}

async function ScaricaAllegati_zippaFiles(scegliNome) {
    let allegati = ScaricaAllegati_listaDocumenti.slice();

    if (UploadMultiploAllegatiAbilitato == false) {
        for (let i = 0; i < allegati.length; i++) {
            allegati[i].file = atob(allegati[i].file);
        }

    }

    var zip = new JSZip();
    deferreds = [];
    allegati.forEach(function (allegato) {
        if (UploadMultiploAllegatiAbilitato == false || allegato.CompressoDaGIAS == false) {
            deferreds.push(zip.file(allegato.nome, (allegato.file), { base64: true }));
        } else {
            deferreds.push(zip.file(allegato.nome, atob(allegato.file), { base64: true }));
        }
    });

    var nome = allegati[0].nome.split(".")[0]
    if (scegliNome) {
        if ($(cSito_Provenienza).val() == '7') {
            let azienda = KendoGrid("tabella_scadenze").dataSource.data()[0].Azienda
            let categoria = KendoGrid("tabella_scadenze").dataSource.data()[0].Categoria_1

            nome = "Checklist_" + azienda + "_" + categoria
        } else {
            let azienda = ""
            if (KendoDDL("ddlAzienda").text() != "") {
                azienda = "_" + KendoDDL("ddlAzienda").text().split(" (Codice Socio:")[0]
            }

            let categoria = ""
            if (KendoDDL("cmbArea").text() != "") {
                categoria = "_" + KendoDDL("cmbArea").text()
            }

            let data = new Date(),
                dataDes = data.getFullYear().toString() + ("0" + data.getMonth().toString()).slice(-2) + ("0" + data.getDay().toString()).slice(-2)

            nome = "DownloadDocument_" + dataDes + azienda + categoria
        }
    }

    nome = nome.replaceAll('.', "")
    //jszip 2.6.1
    var content = zip.generate({ type: "blob" });
    saveAs(content, nome);
}

//AGGIUNTA INITIALFILES ALLA LISTADOCUMENTI
function ScaricaAllegati_aggiungi_InitialFile_A_ListaDocumenti() {
    return new Promise((resolve, reject) => {
        let arrProm = [];
        for (let i = 0; i < ScaricaAllegati_initialFiles.length; i++) {
            if (ScaricaAllegati_initialFiles[i]) {
                arrProm.push(ScaricaAllegati_leggiFile(ScaricaAllegati_initialFiles[i]));
            }
        }

        Promise.all(arrProm).then((values) => {
            resolve()
        });

    })
}

function ScaricaAllegati_leggiFile(file) {
    return new Promise((resolve, reject) => {
        var reader = new FileReader();
        reader.onload = function (readerEvt) {
            var binaryString = readerEvt.target.result;
            var allegato = new Object();

            allegato.nome = file.name;
            allegato.file = btoa(binaryString);

            let estensione = file.name.split('.');
            if (estensione.length > 0) {
                allegato.estensione = estensione[estensione.length - 1]
            } else {
                allegato.estensione = "";
                //To do 19/05/22.... alert messaggio estensione non valida :)
            }

            allegato.UID = file.uid;

            allegato.storicizzato = file.storicizzato;
            allegato.validazione = file.validazione;
            allegato.CompressoDaGias = true

            ScaricaAllegati_listaDocumenti.push(allegato);
            resolve();
        };

        reader.readAsBinaryString(file.rawFile);
    });
}

// #endregion

// #region AVANZAMENTO DI STATO

function AvanzamentoStato() {
    if ($(cArea_Provenienza).val() == 0) {
        //Anna Salvataggio filtri/personalizzazioni griglia
        Salva_Filtri();
    }

    var listaAllegatiCod = []
    var items = KendoGrid("tabella_scadenze").dataSource.data()

    items.forEach(function (item) {
        var checked = Boolean(item.Selected)

        if (checked == true) {
            listaAllegatiCod.push(item.Allegati_Documenti_Cod)
        }
    }, this);

    var parametri = {
        lista_allegati: JSON.stringify(listaAllegatiCod)
    }

    $.ajax({
        type: "POST",
        url: "Scad_Lista.aspx/GestionePassaggioDiStato",
        data: JSON.stringify(parametri),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d.Errore.length > 0)
                kendo.alert(msg.d.Errore)
            else
                apriGestionePassaggioDiStato(msg.d.RispostaStringa);
        }
    });

}

function apriGestionePassaggioDiStato(url) {
    $(document.body).append('<div id="GestionePassaggioDiStatoWindowDocumentale"></div>');
    $('#GestionePassaggioDiStatoWindowDocumentale').kendoWindow({
        title: "Avanzamento Pratica",
        modal: true,
        resizable: true,
        iframe: true,
        width: "70%",
        height: "60%",
        content: url,
        close: function () {
            setTimeout(function () {
                $('#GestionePassaggioDiStatoWindowDocumentale').kendoWindow('destroy');
                //$("#btn_CercaPratiche").trigger("click");
            }, 200);
        }
    }).data('kendoWindow').center();
}

function chiudiWindowPassaggioDiStato() {

    setTimeout(function () {
        $('#alert').kendoWindow('destroy');
        $('#GestionePassaggioDiStatoWindowDocumentale').kendoWindow('destroy');
        $("#btn_Ricerca").trigger("click");
    }, 200);

}

// #end region

// #region FILTRI/PERSONALIZZAZIONI GRIGLIA

function Applica_Personalizzazioni_Griglie() {
    if (personalizzazioniGriglie != null && personalizzazioniGriglie != undefined && personalizzazioniGriglie != "") {

        for (var i = 0; i < personalizzazioniGriglie.length; i++) {

            var obj = personalizzazioniGriglie[i];
            var idControllo = obj.IdControllo;
            var personalizzazioni = obj.Personalizzazioni;
            var grid = $("#" + idControllo).data("kendoGrid");
            if (grid != null && grid != undefined && personalizzazioni != null && personalizzazioni != undefined && personalizzazioni != "") {
                setPersonalizzazioniGrigliaKendo(grid, JSON.parse(personalizzazioni));
                personalizzazioniGriglie[i].Personalizzazioni = null;
            }
        }
    }
}

function controllaRangeDate(da, a) {
    let giorno = setGiorno(da);
    let mese = setMese(da);
    let anno = setAnno(da);
    let unMeseDopo = new Date(anno, mese - 1, giorno);
    unMeseDopo = aggiungiUnMese(unMeseDopo)

    //Per comparare le date con gli operatori < >, queste devono avere il formato YYYY/MM/GG
    let unMese_DopoFormattato = formattaData(unMeseDopo)
    let dataDa_formattata = formattaData(da)
    let dataA_formattata = formattaData(a)

    if (dataA_formattata > unMese_DopoFormattato) {
        return false
    }

    return true
}

function proseguiConRicerca() {
    var prosegui = false;

    let _azienda = KendoDDL("ddlAzienda").value()
    if (_azienda != '') {
        prosegui = true
    }

    if (prosegui == false) {
        let _uploadDal = $('input[name$="txt_Inizio_upload"]').val()
        let _uploadAl = $('input[name$="txt_Fine_upload"]').val()

        if (_uploadDal != '' && _uploadAl != '') {
            prosegui = controllaRangeDate(_uploadDal, _uploadAl)
        }

        if (prosegui == false) {
            let _inizioScadenza = $('input[name$="txt_Validita_Inizio"]').val()
            let _fineScadenza = $('input[name$="txt_Validita_Fine"]').val()

            if (_inizioScadenza != '' && _fineScadenza != '') {
                prosegui = controllaRangeDate(_inizioScadenza, _fineScadenza)
            }

        }
    }

    return prosegui
}

function aggiungiUnMese(data) {
    data.setDate(data.getDate() + 31)
    let unMeseDopo = data.toLocaleDateString('en-GB');
    return unMeseDopo
}

function formattaData(data) {
    let giorno = setGiorno(data);
    let mese = setMese(data);
    let anno = setAnno(data);

    //return mese + '-' + giorno + '-' + anno
    return anno + '-' + mese + '-' + giorno
}

function setGiorno(Data) {
    return Data.slice(0, 2)
}

function setMese(Data) {
    return Data.slice(3, 5)
}

function setAnno(Data) {
    return Data.slice(6, 10)
}

async function Personalizza_Report(parametri) {

    //Inizializzato a true, ma se è stato salvato lo switch 'Azienda Corrente', ci basiamo sull'azienda selezionata sulla dashboard
    let prendiPivaDaPersonalizzazione = true

    if (parametri._chkSoloAziendeAttive !== undefined) {
        ChkSoloAttiveOld = KendoSwitch("ChkSoloAttive").value();
        ChkSoloAttiveNew = parametri._chkSoloAziendeAttive;
        KendoSwitch("ChkSoloAttive").value(ChkSoloAttiveNew);
        if (ChkSoloAttiveOld != ChkSoloAttiveNew) {
            await ddlAzienda_Load($(cPiva).val());
        }
    }

    if (parametri._chkAziendaCorrente !== undefined) {
        KendoSwitch("chkAziendaCorrente").value(parametri._chkAziendaCorrente);
        if (parametri._chkAziendaCorrente) {
            if (currentPiva != '') {
                let ds = KendoDDL("ddlAzienda").dataSource._data
                if (ds.filter((ds) => ds.piva == currentPiva)) {
                    prendiPivaDaPersonalizzazione = false
                    await Set_KendoDDLValueVirtual("ddlAzienda", currentPiva)
                }
            }
        }
    }

    if (prendiPivaDaPersonalizzazione)
        if (parametri._azienda !== undefined) {
            // nel chiamante la funzione dopo controlla che ddlAzienda sia valorizzato, quindi await per assicurarsi che il valore sia impostato
            await Set_KendoDDLValueVirtual("ddlAzienda", parametri._azienda)
        }

    if (parametri._area !== undefined) {
        KendoDDL("cmbArea").value(parametri._area);
        CmbArea_change();

        if (parametri._tipologie !== undefined)
            KendoMultisel("cmbTipologia").value(parametri._tipologie.split("|"));
    }

    if (parametri._validazione !== undefined)
        KendoDDL("cmbValidazione").value(parametri._validazione);
    if (parametri._chkValidazioneAttiva !== undefined)
        KendoSwitch("ChkValidazione").value(parametri._chkValidazioneAttiva);

    if (parametri._storico !== undefined)
        KendoDDL("ddlStorico").value(parametri._storico);
    if (parametri._chkGestioneStorico !== undefined)
        KendoSwitch("ChkGestioneStorico").value(parametri._chkGestioneStorico);

    if (parametri._chkSoloMieiAllegati !== undefined)
        KendoSwitch("chkUtenteUpload").value(parametri._chkSoloMieiAllegati);

    if (parametri._uploadDal != "" && parametri._uploadDal != null)
        set_dataTime("txt_Inizio_upload", parametri._uploadDal, null);
    if (parametri._uploadAl != "" && parametri._uploadAl != null)
        set_dataTime("txt_Fine_upload", parametri._uploadAl, null);

    if (parametri._inizioScadenza != "" && parametri._inizioScadenza != null)
        set_data("txt_Validita_Inizio", parametri._inizioScadenza, null);
    if (parametri._fineScadenza != "" && parametri._fineScadenza != null)
        set_data("txt_Validita_Fine", parametri._fineScadenza, null);

    personalizzazioniGriglie = parametri._griglia;
}

function CreaNuovoOggettoGriglia(idControllo, valore) {
    var griglia = new Object();
    griglia.IdControllo = idControllo;
    griglia.Personalizzazioni = valore;

    return griglia;
}

function getParametriGrigliaTestata(pagina, nomeDiv) {
    var jsonDaSalvare = "";
    var grid = $('#' + nomeDiv).data('kendoGrid');
    if (grid !== undefined) {
        var dataSource = grid.dataSource;
        var columns = grid.columns;
        var pageSize = dataSource.pageSize();
        var sort = dataSource.sort();
        var filter = dataSource.filter();
        var group = dataSource.group();
        var page = dataSource.page();

        //ultimaRigaSelezionataGrigliaTestata.page = page;
        var currentRow = 0; //ultimaRigaSelezionataGrigliaTestata;
        var options = { pagina: pagina, nomeDiv: nomeDiv, columns: columns, page: page, pageSize: pageSize, sort: sort, filter: filter, group: group, currentRow: currentRow };
        jsonDaSalvare = kendo.stringify(fixParametriGrigliaKendo(options));
    }
    return jsonDaSalvare;
}

// #endregion


