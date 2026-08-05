
function kendo_Operazioni_onDataBoundedRighe(e) {

    //kendo_AggiustaDimensioneColonne("#divKendoOperazioni");
    var colonne = $("#divKendoOperazioni").data("kendoGrid").options.columns
    colonne.forEach(function (colonna) {
        console.log(colonna)
        let field = colonna.field
        if (field == "Azioni") {
            $("#divKendoOperazioni").data("kendoGrid").autoFitColumn(field)
        }
    }, this);

    coloraRigheOperazioni(e);
    nascondiPulsantiOperazioniAgenda(e);

    //Se in mobile mostro solo la colonna unica
    mostraColonnaUnicaSeInMobile(e, 2);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe(e, 2);

    //var wrapper = e.sender.wrapper,
    //    header = wrapper.find(".k-grid-header");

    //function resizeFixed() {
    //    var paddingRight = parseInt(header.css("padding-right"));
    //    header.css("width", wrapper.width() - paddingRight);
    //}

    //function scrollFixed() {
    //    var offset = $(this).scrollTop(),
    //        tableOffsetTop = wrapper.offset().top,
    //        tableOffsetBottom = tableOffsetTop + wrapper.height() - header.height();
    //    if (offset < tableOffsetTop || offset > tableOffsetBottom) {
    //        header.removeClass("fixed-header");
    //    } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !header.hasClass("fixed")) {
    //        header.addClass("fixed-header");
    //    }
    //}

    //resizeFixed();
    //$(window).resize(resizeFixed);
    //$(window).scroll(scrollFixed);

    VerificaVisibilitaCostiPerIdAgendaInPage();

}

function VerificaVisibilitaCostiPerIdAgendaInPage() {
    var strListIdAgenda = "";
    var countListaIdAgendaInPage = 0;
    var styleBtnVaiAiCosti = "";
    var grid = $("#divKendoOperazioni").data('kendoGrid');
    var model;

    //Recupero lista idAgenda presente in pagina
    grid.tbody.find("tr[role='row']").each(function () {
        model = grid.dataItem(this);

        //Verificare la visibilità del btn VaiAiCosti
        if (!lavCodIntListaOperazioni(model.Lav_cod)) {
            $(this).find(".classVaiAiCosti").each(function (item) {
                $(this).hide();
            });
        }

        if (strListIdAgenda != "") {
            strListIdAgenda += ",";
        }
        strListIdAgenda += model.ID;
        countListaIdAgendaInPage++;
    });

    if (countListaIdAgendaInPage <= 100 && listaIdAgendaInPage != strListIdAgenda) {
        listaIdAgendaInPage = strListIdAgenda;
        resultQueryListaIdAgendaInPage = getDatiEsistonoCostiCollegatiCDG(strListIdAgenda);
        if (resultQueryListaIdAgendaInPage != "") {
            grid.tbody.find("tr[role='row']").each(function () {
                model = grid.dataItem(this);
                if (parseInt(model.Lav_cod) < 1000) {
                    styleBtnVaiAiCosti = getStyleBtnVaiAiCosti(model.id_agenda)
                    $(this).find(".classVaiAiCosti").each(function (item) {
                        $(this).addClass(styleBtnVaiAiCosti);
                    });
                }
            });
        }
    }
}

//Carica lsita operazioni per il controllo del lavCod per i costi
function caricaGrigliaOperazioniWS(filtroAggiuntivo) {
    var parametri = kendo.stringify({
        "filtroAggiuntivo": filtroAggiuntivo,
        "objP_server": objP_server
    });

    ajaxAgronicaSync(pathCoreWS + "Agenda/Agenda.asmx/CaricaGrigliaOperazioni",
        parametri, false,
        function (risposta) {
            resposta = risposta.RispostaStringa;
        }, null);

    return resposta;
}

function lavCodIntListaOperazioni(lavCod) {
    var lavCodIntListaOperazioni = false;
    const res = JSON.parse(grigliaOperazioni);

    for (var i = 0; i < res.length; i++) {
        if (lavCod == res[i].LAV_COD) {
            lavCodIntListaOperazioni = true;
            break;
        }
    }
    return lavCodIntListaOperazioni;
}

function getStyleBtnVaiAiCosti(idAgenda) {
    var styleBtnVaiAiCosti = "btn-success";
    const res = JSON.parse(resultQueryListaIdAgendaInPage);

    for (var i = 0; i < res.length; i++) {
        if (idAgenda == res[i].Id_Agenda) {
            if (res[i].EsistonoCostiCollegatiCDG == true) {
                styleBtnVaiAiCosti = "btn-warning";
                break;
            }
        }
    }
    return styleBtnVaiAiCosti;
}


function kReadValorizzazioneDettagliDestinazioni_rows(options) {

    var data = $('#hdKendoOperazioniDettagliDestinazioni_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadDettagliDestinazioniValorizzazione_mod(jSonParsed_Kendo) {
    return jSonParsed_Kendo.kendo_model;
}

function kReadDettagliDestinazioniValorizzazione_col(jSonParsed_Kendo) {
    return jSonParsed_Kendo.kendo_columns;
}

function kendo_OperazioniDettagliDestinazioni_onDataBoundedRighe(e) {
}

function KendoOperazioniDettagliDestinazioni_inizializza(divKendoOperazioni) {

    var funzioniCRUD = {
        funzioneRead: kReadValorizzazioneDettagliDestinazioni_rows
    };

    var idModel = "Id_Agenda";


    var data = $('#hdKendoOperazioniDettagliDestinazioni_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    var campiKendoModel = kReadDettagliDestinazioniValorizzazione_mod(jSonParsed_Kendo);
    var colonneKendoGrid = kReadDettagliDestinazioniValorizzazione_col(jSonParsed_Kendo);


    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        excel: true,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: { multi: true, search: true }
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendo_OperazioniDettagliDestinazioni_onDataBoundedRighe
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoOperazioni = creaKendoGrid(divKendoOperazioni, // rappresenta l'ID del div a cui si associa la griglia
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

    var griglia = $('#' + divKendoOperazioni).data("kendoGrid");

    griglia.autoFitColumn("Data2");
    //Allargo le caselle di filtro con un min-width
    //$('#' + divKendoOperazioni + ' th input:text').css("min-width", "85px");
    //griglia.refresh();

}


function KendoOperazioni_inizializza(divKendoOperazioni) {

    var funzioniCRUD = {
        funzioneRead: kReadValorizzazione_rows,
        checkBoxFunction: KendoOperazioni_checked
    };

    var idModel = "id_agenda";

    var jsLblInfo = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblInfo", "Info");
    var jsLblModifica = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblModifica", "Modifica");
    var jsLblCancella = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblCancella", "Cancella");
    var jsLblCopia = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblCopia", "Copia");
    var jsLblRicetta = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblRicetta", "Ricetta");

    var btnAzioniWidth = "115px";

    var data = $('#hdKendo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    var campiKendoModel = kReadValorizzazione_mod(jSonParsed_Kendo);
    var colonneKendoGrid = kReadValorizzazione_col(jSonParsed_Kendo);
    var pulsanteCopiaBsAgendaNuovo = "<div class='btn btn-warning btnCopia' style='display:block;width:" + btnAzioniWidth + ";border:0px;margin-bottom:5px;' onclick=duplicaQdC(this.closest('tr'),this.closest('.k-grid'))>" + jsLblCopia + "</div>";
    if (Request_QueryString("gis") === "true") {
        pulsanteCopiaBsAgendaNuovo = "";
    }
    var strBottoni = "<div class='btn btn-info btnInfo' style='display:block;width:" + btnAzioniWidth + ";border:0px;' onclick=infoQdC(this.closest('tr'),this.closest('.k-grid'))>" + jsLblInfo + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:" + btnAzioniWidth + ";border:0px;' onclick=modificaQdC(this.closest('tr'),this.closest('.k-grid'))>" + jsLblModifica + "</div>" +
        "<div class='btn btn-danger btnCancella' style='display:block;width:" + btnAzioniWidth + ";border:0px;' onclick=eliminaQdC(this.closest('tr'),this.closest('.k-grid'))>" + jsLblCancella + "</div>" +
        pulsanteCopiaBsAgendaNuovo;

    strBottoni = strBottoni + "<div class='text-center'> ";
    if ($("input[name$='hf_UtenteAbilitatoGestioneVisualizaAllegato']").val() == "True") {
        strBottoni = strBottoni + "<span class='fa fa-file-text-o fa-2x info_elem btnGestioneDocumenti' title='Gestione Documenti' onclick=ApriKendoWindowRicercaDocumenti(this.closest('tr'),this.closest('.k-grid'))></span>";
    }


    if ($("input[name$='hf_UtenteAbilitatoGestioneNuovoAllegato']").val() == "True") {
        strBottoni = strBottoni + "<span class='fa fa-paperclip fa-2x info_elem btnAggiungiNuovoAllegato' title='Aggiungi nuovo allegato'  onclick=ApriKendoWindowAggiungiNuovoAllegato(this.closest('tr'),this.closest('.k-grid')) ></span>";
    }

    if ($("input[name$='hf_UtenteAbilitatoCostiRicaviDaQdCeCdG']").val() == "True" && $("input[name$='hf_UtenteAbilitatoFlagMostraBtnSalvaCDG']").val() == "True") {
        //var classCostiCollegatiCDG = getClassCostiCollegatiCDG(this.closest('tr'), this.closest('.k-grid'));

        /*   var classCostiCollegatiCDG = "btn - success";*/
        /*var classCostiCollegatiCDG = "";*/
        //if ($("input[name$='hf_esistonoCostiCollegatiCDG']").val().ToLower == 'true') {
        //    classCostiCollegatiCDG = "btn - warning";
        //}
        strBottoni = strBottoni + "<span class='fa fa-line-chart fa-2x info_elem classVaiAiCosti' title='Costi'  onclick=VaiAiCosti(this.closest('tr'),this.closest('.k-grid')) ></span>";
    }

    strBottoni = strBottoni + "</div> ";

    if (mode === 'AggiungiAlPua') {
        strBottoni = "<div class='btn btn-info btnAggiungiAlPua' style='display:block;width:120px;border:0px;' onclick=aggiungialpuaElemento(this.closest('tr'),this.closest('.k-grid'))>Importa nel Pua</div>";
    }

    var colonneCustomKendoGridtmp = [
        {
            command: {
                template: strBottoni
            }, title: "Azioni", width: "97px", field: "Azioni"
        }
    ];

    if (UtenteAbilitatoRicette === true) {
        colonneKendoGrid.push(
            {
                //template: "<div class='btn btn-warning btnCreaRicetta' style='display:block;width:70px;border:0px;' onclick=crea_ricetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-right'></i>" + jsLblRicetta + "</div></div><div class='lblRicetta'>#: Lav_Des #</div>",
                //template: "<div class='lblRicetta'>#: Lav_Des #</div>",
                //command: {
                template: "<div class='btn btn-warning btnCreaRicetta' style='display:block;width:70px;border:0px;' onclick=crea_ricetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-right'></i>" + jsLblRicetta + "</div></div><div class='lblRicetta'>#: Ricetta_Des #</div>",
                //},
                title: "Ricette",
                width: "97px"
            }
        );
    }

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        toolbarCommands: ["templateLegendaMenuAgendaOperazioniTutte"],
        excel: false,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: { mode: "row" },
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: colonneCustomKendoGridtmp
        //colonneCustomKendoGrid: [
        //    {
        //        command: {
        //            template: strBottoni
        //        }, title: "Azioni", width: "97px"//, widthfisso: true //width: "135px" - no style in span
        //    }          
        //]

    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundedRighe
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoOperazioni = creaKendoGrid(divKendoOperazioni, // rappresenta l'ID del div a cui si associa la griglia
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

    var griglia = $('#' + divKendoOperazioni).data("kendoGrid");

    griglia.autoFitColumn("Data2");
    //Allargo le caselle di filtro con un min-width
    //$('#' + divKendoOperazioni + ' th input:text').css("min-width", "85px");
    //griglia.refresh();

}

function getDatiEsistonoCostiCollegatiCDG(listIdAgendaInPage) {
    var resposta = "";

    var parametri = kendo.stringify({
        "Piva": pivaAziendaSelezionata,
        "listaIdAgenda": listIdAgendaInPage,
        "objP_server": objP_server
    });

    ajaxAgronicaSync(pathCoreWS + "Agenda/Agenda.asmx/OperazioneAgenda",
        parametri, false,
        function (risposta) {
            resposta = risposta.RispostaStringa;
        }, null);

    return resposta;
}

function kReadValorizzazione_rows(options) {

    var data = $('#hdKendo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazione_col(jSonParsed_Kendo) {

    return jSonParsed_Kendo.kendo_columns;
}

function kReadValorizzazione_mod(jSonParsed_Kendo) {

    return jSonParsed_Kendo.kendo_model;
}


function KendoOperazioni_checked(e) {

    let checked = this.checked;
    let row = $(this).parents("tr");
    let grid = $("#divKendoOperazioni").data("kendoGrid");
    let dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    dataItem.dirty = true;

    rowKendoGridSelected(row, checked)
}


function VaiAiCosti(tr_elem, grid_elem) {
    var msgError = false;
    var dataItem = GetIdAgendaInElem(tr_elem, grid_elem);
    var lav_cod = (typeof dataItem.Lav_cod === 'undefined') ? 0 : dataItem.Lav_cod;
    if (lav_cod > 1000) {
        msgError = true;
    }
    else {
        if (!lavCodIntListaOperazioni(lav_cod)) {
            msgError = true;
        }
    }

    if (msgError == true) {
        var nonEPossibileInserireCostiPerQuestaOperazione = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "NonEPossibileInserireCostiPerQuestaOperazione", "Non è possibile inserire costi per questa operazione");
        alert(nonEPossibileInserireCostiPerQuestaOperazione);
    }
    else {
        var Id_Area = 11;
        var Ricetta_Operazione_Cod = (typeof dataItem.Ricetta_Operazione_Cod === 'undefined') ? 0 : dataItem.Ricetta_Operazione_Cod;
        var Id_Agenda = (typeof dataItem.id_agenda === 'undefined') ? 0 : dataItem.id_agenda;
        var Piva = (typeof dataItem.Piva === 'undefined') ? dataItem.piva : dataItem.Piva;

        var linkPaginaOrigine = $("input[name$='hf_LinkPaginaOrigine']").val();
        var parametri = kendo.stringify({ "linkPaginaOrigine": linkPaginaOrigine, "id_Agenda": Id_Agenda });

        var url = "";

        ajaxAgronicaSync("MenuBS_Agenda_Nuovo.aspx/VaiAiCosti",
            parametri, false,
            function (risposta) {
                window.location = risposta.RispostaStringa;
            }, null);
    }
}

function infoQdC(tr_elem, grid_elem) {
    infoElemento(tr_elem, grid_elem, enumAgenda)
}

function modificaQdC(tr_elem, grid_elem) {
    modificaElemento(tr_elem, grid_elem, enumAgenda)
}

function eliminaQdC(tr_elem, grid_elem) {
    eliminaElemento(tr_elem, grid_elem, enumAgenda)
}

function duplicaQdC(tr_elem, grid_elem) {
    duplicaElemento(tr_elem, grid_elem, enumAgenda)
}



function ApriKendoWindowRicercaDocumenti(tr_elem, grid_elem) {
    var dataItem = GetIdAgendaInElem(tr_elem, grid_elem);

    var Id_Area = 11;
    var Ricetta_Operazione_Cod = (typeof dataItem.Ricetta_Operazione_Cod === 'undefined') ? 0 : dataItem.Ricetta_Operazione_Cod;
    var Id_Agenda = (typeof dataItem.id_agenda === 'undefined') ? 0 : dataItem.id_agenda;
    var Piva = (typeof dataItem.Piva === 'undefined') ? dataItem.piva : dataItem.Piva;

    switch (parseInt(dataItem.Lav_cod)) {
        case 2004: //Ordine Acquisto           
        case 1025: //DDT Ricevuto              
        case 1054:
        case 1076:
        case 1078: //Conferimento    
        case 1031: //DDT Emesso                
        case 2002: //Ordine Vendita            
        case 1000: //Ordine Vendita
        case 1001: //Fattura emessa            
            Id_Area = 10;
            break;
        case 1022: //Carichi Magazzino
        case 1023: //Scarichi Magazzino
            Id_Area = 13;
            break;
    }

    var url = "../Scadenzario/Scad_lista.aspx?type=doc" + "&area_provenienza=" + Id_Area + "&p=" + Piva + "&id_agenda=" + Id_Agenda + "&Ricetta_Operazione_Cod=" + Ricetta_Operazione_Cod;

    $(document.body).append('<div id="ricerca_documentale"></div>');
    $('#ricerca_documentale').kendoWindow({
        title: "Ricerca Documenti",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#ricerca_documentale').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function GetIdAgendaInElem(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    return dataItem;
}

function ApriKendoWindowAggiungiNuovoAllegato(tr_elem, grid_elem) {
    var dataItem = GetIdAgendaInElem(tr_elem, grid_elem);

    var ID_Alert_Entita = -1;
    var ID_Elenco = -1;
    var Modalita = "doc";

    var Id_Area = 11;
    var Tipologia;
    var Piva = (typeof dataItem.Piva === 'undefined') ? dataItem.piva : dataItem.Piva;
    var Id_Agenda = (typeof dataItem.id_agenda === 'undefined') ? 0 : dataItem.id_agenda;
    var Ricetta_Operazione_Cod = (typeof dataItem.Ricetta_Operazione_Cod === 'undefined') ? 0 : dataItem.Ricetta_Operazione_Cod;

    if (Tipologia == undefined || Tipologia == null || Tipologia == "") {
        switch (parseInt(dataItem.Lav_cod)) {
            case 2004: //Ordine Acquisto           
                Tipologia = -18;
                Id_Area = 10
                break;
            case 1025: //DDT Ricevuto              
                Tipologia = -19;
                Id_Area = 10
                break;
            case 1054:
            case 1076:
            case 1078: //Conferimento    
                Tipologia = -20;
                Id_Area = 10
                break;
            case 1031: //DDT Emesso                
                Tipologia = -21;
                Id_Area = 10
                break;
            case 2002: //Ordine Vendita            
                Tipologia = -22;
                Id_Area = 10
                break;
            case 1000: //Ordine Vendita
                Tipologia = -23;
                Id_Area = 10
                break;
            case 1001: //Fattura emessa            
                Tipologia = -24;
                Id_Area = 10
                break;
            case 1022: //Carico Magazzino
                Tipologia = -28;
                Id_Area = 13
                break;
            case 1023: //Scarico Magazzino
                Tipologia = -29;
                Id_Area = 13
                break;
        }

        Tipologia = (typeof Tipologia === 'undefined') ? 0 : Tipologia;

        //if (Tipologia != undefined && Tipologia != null && Tipologia != "") {
        //    Id_Area = 10;
        //}
    }

    // Mouad : Aggiunto con Lorenzo per resolvere il problema segnalato nel email di Stefano (I: GIAS 01/07/2022 - TEST DOCUMENTALE il giovedì 07/07/2022 14:56)
    if (Id_Area == 11 && Tipologia == 0) {
        Tipologia = -25;
    }

    var param = kendo.stringify({
        'Piva': Piva, 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'Id_Area': Id_Area, 'Tipologia': Tipologia, 'area_provenienza': Id_Area, 'Id_Agenda': Id_Agenda, 'Ricetta_Operazione_Cod': Ricetta_Operazione_Cod
    });
    //var param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 1, 'ID_Alert_Entita': ID_Alert_Entita });
    var url = "../Scadenzario/Scad_CreaModificaItem.aspx?scadstr=" + param + "&type=" + Modalita + "&p=" + dataItem.Piva;

    $(document.body).append('<div id="nuovo_documentale"></div>');
    $('#nuovo_documentale').kendoWindow({
        title: "Nuovo Documento",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#nuovo_documentale').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function aggiungialpuaElemento(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var data = datiRiga.chiave_composita.split("_")[0];
    var id_agenda = datiRiga.chiave_composita.split("_")[1];
    var lav_cod = datiRiga.chiave_composita.split("_")[2];
    var piva = datiRiga.chiave_composita.split("_")[3];
    var sa_cod = datiRiga.chiave_composita.split("_")[4];

    var ricetta_cod = $(ricetta_cod_ClientID).val();

    //Estraggo gli elementi selezionati
    var arraySelected = elementiGrigliaSelezionati(datiGriglia);

    //concateno gli elementi selezionati
    var id_agenda_checked = "";
    var lav_cod_checked = "";

    for (var iRow in arraySelected) {
        id_agenda_checked += arraySelected[iRow].id_agenda + ',';
        lav_cod_checked += arraySelected[iRow].Lav_cod + ',';
    }

    if (id_agenda_checked === "") {
        id_agenda_checked = id_agenda + ",";
    }

    if (lav_cod_checked === "") {
        lav_cod_checked = lav_cod + ",";
    }

    id_agenda_checked += "-1";
    lav_cod_checked += "-1";

    var parametri = kendo.stringify({ "data": data, "id_agenda": id_agenda, "id_agenda_checked": id_agenda_checked,"lav_cod_checked": lav_cod_checked, "lav_cod": lav_cod, "piva": piva, "sa_cod": sa_cod, "ricetta_cod": ricetta_cod });

    ajaxAgronicaSync("MenuBS_Agenda_Nuovo.aspx/aggiungi_al_pua",
        parametri, false,
        function (risposta) {
            window.location = risposta.RispostaStringa;
        }, function (risposta) {
            alert(risposta.Errore);
        });

}

function crea_ricetta(tr_elem, grid_elem) {

    $('#lbl_operazioni_ricetta_da_creare').html('');

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var piva = datiRiga.Piva;
    var sa_cod = datiRiga.sa_cod;

    var sacod_ricetta = sa_cod;

    var veg_cod = datiRiga.Veg_cod;
    var veg_des = datiRiga.Specie_Varieta;

    var stesso_vegcod = true;

    var data = datiRiga.Data2;
    var id_agenda = datiRiga.id_agenda;

    var data_inizio = new Date(data);
    var data_fine = new Date(data);

    $('#piva_ricetta').text(piva);
    $('#sacod_ricetta').text(sa_cod);
    $('#idagenda_ricetta').text(id_agenda);

    ////Estraggo gli elementi selezionati
    var arraySelected = elementiGrigliaSelezionati(datiGriglia);

    //concateno gli elementi selezionati
    var id_agenda_checked = "";
    var operazioni_da_ribaltare = "";

    for (var iRow in arraySelected) {

        if (arraySelected[iRow].Ricetta_Cod === "0" &&
            lav_cod_ricettabili.indexOf(parseInt(arraySelected[iRow].Lav_cod)) > 0) {

            if (arraySelected[iRow].Veg_cod !== veg_cod) {
                stesso_vegcod = false;
            }

            id_agenda_checked += arraySelected[iRow].id_agenda + ',';

            var datatmp = new Date(arraySelected[iRow].Data2);

            if (datatmp < data_inizio) {
                data_inizio = datatmp;
            }
            if (datatmp > data_fine) {
                data_fine = datatmp;
            }

            operazioni_da_ribaltare += '<div class="row"><div class="col-lg-12">' + arraySelected[iRow].Data + ' - ' + arraySelected[iRow].Lav_Des + ' - ' + arraySelected[iRow].Specie_Varieta + '</div></div>';

            if (arraySelected[iRow].sa_cod !== 0 && arraySelected[iRow].sa_cod !== sacod_ricetta) {
                sacod_ricetta = 0;
            }
        }
    }

    if (id_agenda_checked === "") {
        operazioni_da_ribaltare = '<div class="row"><div class="col-lg-12">' + datiRiga.Data + ' - ' + datiRiga.Lav_Des + ' - ' + datiRiga.Specie_Varieta + '</div></div>';
    }

    $(ricetta_datainizio_ClientID).val(kendo.toString(data_inizio, 'dd/MM/yyyy'));
    $(ricetta_datafine_ClientID).val(kendo.toString(data_fine, 'dd/MM/yyyy'));

    $('#datainizio_ricetta_min').text(kendo.toString(data_inizio, 'dd/MM/yyyy'));
    $('#datafine_ricetta_max').text(kendo.toString(data_fine, 'dd/MM/yyyy'));


    var jsLblRicettaConterraOperazioni = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblRicettaConterraOperazioni", "La Ricetta generata conterrà le seguenti operazioni:");

    if (operazioni_da_ribaltare !== '') {
        operazioni_da_ribaltare = '<div class="row"><div class="col-lg-12">' + jsLblRicettaConterraOperazioni + '</div></div>' + operazioni_da_ribaltare;
    }

    $('#lbl_operazioni_ricetta_da_creare').html(operazioni_da_ribaltare);

    var ricetta_numero = '';

    //ricavo il default del numero ricetta
    ajaxAgronicaSync("MenuBS_Agenda_Nuovo.aspx/ricetta_numero_default", JSON.stringify({ "piva": piva, "sa_cod": sa_cod }), false,
        function (risposta) {
            if (risposta.RispostaOK) {
                ricetta_numero = risposta.RispostaStringa;
            }
        }, null);

    $(ricetta_numero_ClientID).val(ricetta_numero);
    $(ricetta_descrizione_ClientID).val(veg_des);

    var ErroreSingolaSpecie = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsMsgPerRicettaSeleziona1solaSp", "Per creare la Ricetta occorre selezionare una sola specie vegetale!");

    if (stesso_vegcod === true) {
        $('#modalCreaRicetta').modal('show');
    } else {
        MessaggioErrore(ErroreSingolaSpecie);
    }

}

$("#btn_nuova_ricetta").click(function () {
    try {
        CreaRicetta();
    } catch (e) {
        console.log("Errore:" + e.message);
    }
});

function CreaRicetta() {

    var ricetta_des = $(ricetta_descrizione_ClientID).val();
    var ricetta_numero = $(ricetta_numero_ClientID).val();

    var data_inizio = $(ricetta_datainizio_ClientID).val();
    var data_fine = $(ricetta_datafine_ClientID).val();
    var nota_des = $(ricetta_nota_ClientID).val();

    var data_inizio_min = $('#datainizio_ricetta_min').text();
    var data_fine_max = $('#datafine_ricetta_max').text();


    if (ricetta_des === '' || ricetta_numero === '') {

        var jsMsgInserireDescrizioneNum = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsMsgInserireDescrizioneNum", "Inserire Descrizione e Numero della Ricetta!");

        alert(jsMsgInserireDescrizioneNum);

    } else {

        var datiGriglia = $('#divKendoOperazioni').data('kendoGrid');

        ////Estraggo gli elementi selezionati
        var arraySelected = elementiGrigliaSelezionati(datiGriglia);

        //concateno gli elementi selezionati
        var id_agenda_checked = "";

        var piva = $('#piva_ricetta').text();
        var sa_cod = $('#sacod_ricetta').text();
        var id_agenda = $('#idagenda_ricetta').text();





        var veg_cod = 0;

        for (var iRow in arraySelected) {
            if (arraySelected[iRow].Ricetta_Cod === "0" &&
                lav_cod_ricettabili.indexOf(parseInt(arraySelected[iRow].Lav_cod)) > 0) {

                id_agenda_checked += arraySelected[iRow].id_agenda + ',';
                veg_cod = arraySelected[iRow].Veg_cod;
            }
        }

        if (id_agenda_checked === "") {
            id_agenda_checked = id_agenda + ",";
        }

        id_agenda_checked += "-1";



        var parametri = kendo.stringify({ "data_inizio": data_inizio, "data_fine": data_fine, "id_agenda_checked": id_agenda_checked, "id_agenda": id_agenda, "piva": piva, "sa_cod": sa_cod, "veg_cod": veg_cod, "ricetta_des": ricetta_des, "ricetta_numero": ricetta_numero, "nota_des": nota_des });

        ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/crea_ricetta",
            parametri,
            function (risposta) {

                if (risposta.RispostaOK === true) {

                    ScritturaOK(risposta.RispostaStringa);
                    //svuoto e chiudo la form

                    $(ricetta_descrizione_ClientID).val('');
                    $(ricetta_numero_ClientID).val('');
                    $(ricetta_nota_ClientID).val('');
                    $(ricetta_datainizio_ClientID).val('');
                    $(ricetta_datafine_ClientID).val('');
                    $('#lbl_operazioni_ricetta_da_creare').html('');
                    $('#piva_ricetta').text('');
                    $('#sacod_ricetta').text('');
                    $('#idagenda_ricetta').text('');

                    $('#modalCreaRicetta').modal('hide');

                    btnAggiorna_click(); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab

                } else {
                    MessaggioErrore(risposta.Errore);
                }

            }, null);

    }

}

function elementiGrigliaSelezionati(datiGriglia) {

    if (datiGriglia === undefined) {
        var idTab = GetIdTab();
        if (idTab == 'linkZoo') {
            datiGriglia = $('#divKendoZoo').data('kendoGrid');
        } else {
            datiGriglia = $('#divKendoOperazioni').data('kendoGrid');
        }
    }

    //Estraggo gli elementi selezionati
    var dati = datiGriglia.dataSource.data();
    var arraySelected = [];
    for (var i = 0; i < dati.length; i++) {
        if (dati[i].Selected) {
            arraySelected.push(dati[i]);
        }
    }

    return arraySelected;
}

function coloraRigheOperazioni(eventArgs) {

    var grid = eventArgs.sender;
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        if (dataItem.contabilizzato < 0) {
            this.className += " kendoRiga_AgendaOperazPianificata";
        }

        if (dataItem.Lav_cod == "3004" || dataItem.Lav_cod == "3035" || dataItem.Lav_cod == "3037") {
            if (dataItem.Tipo_Accettazione) {
                this.className += " kendoRiga_AgendaOperazBloccata";
            } else {
                if (dataItem.blocco_flag == 1) {
                    this.className += " kendoRiga_AgendaOperazDaRevisionare";
                }
            }
            return;
        }

        switch (dataItem.blocco_flag) {
            case "1":
                this.className += " kendoRiga_AgendaOperazBloccata";
                break;
            case "2":
                this.className += " kendoRiga_AgendaOperazDaRevisionare";
                break;
        }

    });

}

function nascondiPulsantiOperazioniAgenda(eventArgs) {

    var grid = eventArgs.sender;
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        $(this.querySelector(".btnRegModello4")).hide();

        //Se l'operazione è bloccata nascondo modifica, cancella 
        //(solo zootecniche => anche genera modello e mostro registrazione)
        if (dataItem.blocco_flag === "1" || dataItem.PermessoModifica === "False") {
            $(this.querySelector(".btnModifica")).hide();
            //$(this.querySelector(".btnCancella")).hide();
            $(this.querySelector(".btnModello4")).hide();
            $(this.querySelector(".btnRegModello4")).show();
        }

        //(solo zootecniche) Se il modello è già stato registrato, nascondo registra
        if (dataItem.Tipo_Accettazione === 1 || dataItem.PermessoModifica === "False") {
            $(this.querySelector(".btnRegModello4")).hide();
        }

        //Se il tipo di operazione non è tra le editabili, nascondo il modifica
        if (lav_cod_non_editabili.indexOf(parseInt(dataItem.Lav_cod)) >= 0) {
            $(this.querySelector(".btnModifica")).hide();
        }

        //Se il tipo di operazione non è tra le copiabili, nascondo il copia
        if (lav_cod_copiabili.indexOf(parseInt(dataItem.Lav_cod)) < 0) {
            $(this.querySelector(".btnCopia")).hide();
        }

        if (mode === 'AggiungiAlPua') {
            if (dataItem.AggiungiAlPua === "false") {
                $(this.querySelector(".btnAggiungiAlPua")).hide();
            }
        }

        //se c'è già una ricetta
        if (dataItem.Ricetta_Cod !== "0") {
            $(this.querySelector(".btnCreaRicetta")).hide();
            $(this.querySelector(".lblRicetta")).show();
        }

        //Se il tipo di operazione non è tra le ricettabili, nascondo il creaRicetta
        if (lav_cod_ricettabili.indexOf(parseInt(dataItem.Lav_cod)) < 0) {
            $(this.querySelector(".btnCreaRicetta")).hide();
            $(this.querySelector(".lblRicetta")).hide();
        }

        if (!permessoModello4) {
            $(this.querySelector(".btnModello4")).hide();
            $(this.querySelector(".btnRegModello4")).hide();
        }

        //(solo zootecniche) Se il tipo di operazione non è di vendita o macello capi, nascondo genera modello
        if (lav_cod_Modello4.indexOf(parseInt(dataItem.Lav_cod)) < 0) {
            $(this.querySelector(".btnModello4")).hide();
            $(this.querySelector(".btnRegModello4")).hide();
        }

        //Se ho un'operazione di Cura permetto solo la cancellazione e nascondo tutti gli altri bottoni
        if (parseInt(dataItem.Lav_cod) === 5004) {
            $(this.querySelector(".btnInfo")).hide();
            $(this.querySelector(".btnCancella")).hide();
            $(this.querySelector(".btnGestioneDocumenti")).hide();
            $(this.querySelector(".btnAggiungiNuovoAllegato")).hide();
        }
    });

}

function mostraColonnaUnicaSeInMobile(eventArgs, nColonneDaSaltare) {
    //Controllo se sono su mobile
    if ($(window).width() <= 767) {
        var grid = eventArgs.sender;

        for (var i = nColonneDaSaltare; i < grid.columns.length; i++) {
            grid.hideColumn(i);
        }
        grid.showColumn('Descrizione_Unica');
    }
}

function riduciAltezzaRighe(eventArgs, nColonneDaSaltare) {
    //Accorcio l'altezza delle righe con switch in caso di click.
    //In pratica aggiungo un div e ci ricopio dentro il contenuto della cella. Su quel div apprico il css per la riduzione delle righe visibili e in caso di click allargo la riga

    ////$('#divKendoOperazioni tr td:not(:first-child):not(:nth-child(2)):visible').each(function (td) {
    //$('#divKendoOperazioni tr td:not(:first-child):not(:nth-child(2))').each(function () {
    //    $(this).html('<div class="toggleEspandi" onclick="' + '$(this).parent().parent().find(' + "'td:not(:first-child):not(:nth-child(2)):visible div'" + ').toggleClass(' + "'toggleEspandi'" + ');\">' + $(this).html() + '</div>');
    //});

    var strColonneDaSaltare = '';
    for (var i = 1; i <= nColonneDaSaltare; i++) {
        strColonneDaSaltare += ':not(:nth-child(' + i + '))';
    }

    let nomeDiv = '#' + eventArgs.sender.wrapper[0].id;

    //tr:not(.k-filter-row) SALTIAMO LA RIGA DI FILTRO
    $(nomeDiv + ' tr:not(.k-filter-row) td' + strColonneDaSaltare).each(function () {
        $(this).html('<div class="toggleEspandi" onclick="' + '$(this).parent().parent().find(' + "'td" + strColonneDaSaltare + " div'" + ').toggleClass(' + "'toggleEspandi'" + ');\">' + $(this).html() + '</div>');
    });
}

function popolaDdlOperazioni() {
    var IDControllo;
    var funzioneLetturaDati;
    var templ;

    var idTab = GetIdTab();

    //carico l'elendo di operazioni corrispondenti
    switch (idTab) {
        case 'linkTutte':
        case 'linkColturali':
        case 'linkMagCont':
        case 'linkAudit':
        case 'linkMacchine':
            IDControllo = "#ddlOperazioni";
            funzioneLetturaDati = function (options) { RiempiDdlOperazioni(options, ""); };

            var templateSenzaStella = "<span class='fa fa-2x fa-star-o' style='display:none;'></span>" + "<span style='margin-left:32px;'>#: lav_des #</span>"
            var templateConStella = "<span class='fa fa-2x fa-star-o' title='" + TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "AggiungiAiPreferiti", "Aggiungi ai preferiti") + "' onclick='Salva_Operazioni_Preferite(#: lav_cod #);' style='cursor:pointer'></span>" + "<span>#: lav_des #</span>"
            templ = "#if(preferito === 1 || permessoImpostazioniUtente === false){#" + templateSenzaStella + "#}else{#" + templateConStella + "#}#";

            break;
        case 'linkZoo':
            IDControllo = "#ddlOperazioniZoo";
            funzioneLetturaDati = RiempiDdlOperazioni_PerZoo;

            var templateSenzaStella = "<span class='fa fa-2x fa-star-o' style='display:none;'></span>" + "<span style='margin-left:32px;'>#: lav_des #</span>"
            var templateConStella = "<span class='fa fa-2x fa-star-o' title='" + TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "AggiungiAiPreferiti", "Aggiungi ai preferiti") + "' onclick='Salva_OperazioniZoo_Preferite(#: lav_cod #);' style='cursor:pointer'></span>" + "<span>#: lav_des #</span>"
            templ = "#if(preferito === 1 || permessoImpostazioniUtente === false){#" + templateSenzaStella + "#}else{#" + templateConStella + "#}#";
            break;
        case 'linkRicette':
            IDControllo = "#ddlOperazioniRicette";
            funzioneLetturaDati = RiempiDdlOperazioni_PerRicette;
            templ = undefined;
            break;
        case 'linkBrogliaccio':
            IDControllo = "#ddlOperazioniBrogliaccio";
            funzioneLetturaDati = RiempiDdlOperazioni_PerBrogliaccio;
            templ = undefined;
            break;
    }

    $(IDControllo).kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: funzioneLetturaDati } },
        dataTextField: "lav_des",
        dataValueField: "lav_cod",
        optionLabel: { "lav_des": TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "Seleziona", "SELEZIONA").toUpperCase() + "...", "lav_cod": "" },
        autoBind: false,
        autoWidth: ($(window).width() >= 768) ? true : false,
        template: templ
    });

}


function popolaDdlCentri(IDControllo, deferred) {

    $(IDControllo).kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiDdlCentri } },
        dataTextField: "sa_nome",
        dataValueField: "sa_cod",
        optionLabel: { "sa_nome": TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "TuttiICentriAziendali", "Tutti i Centri Aziendali"), "sa_cod": "0" },
        autoBind: true,
        dataBound: function (e) {
            var preferenza = $.cookie("MenuBS_Agenda_Nuovo.FiltroCentro");
            if (preferenza !== undefined && operazioniImpianto == false) {
                if (preferenza.split("|")[0] !== pivaAziendaSelezionata) { //se sono su un'altra azienda, tolgo il cookie
                    $.removeCookie("MenuBS_Agenda_Nuovo.FiltroCentro");
                } else {
                    preferenza = preferenza.split("|")[1];
                    e.sender.value(preferenza);
                    if (e.sender.value() === "0") { //Se non ho trovato il valore - probabilmente ho cambiato impresa...
                        $.removeCookie("MenuBS_Agenda_Nuovo.FiltroCentro");
                    }
                }
            } else if (operazioniImpianto == true) {
                e.sender.value(obj_Agenda.Sa_Cod);
            }

            if (deferred !== undefined) {
                deferred.resolve();
            }
        }
    });

}

//function DdlCentri_Change(e) {
//    var sa_cod = e.sender.value();
//    var parametri = kendo.stringify({ "sa_cod": sa_cod });

//    ajaxAgronicaSync("MenuBS_Agenda_Nuovo.aspx/DdlCentri_Change",
//        parametri, false,
//        function (risposta) {
//            risp = JSON.parse(risposta.RispostaStringa);
//        }, null);
//}

async function popolaDdlSpecie(IDControllo, deferred) {
    if ($(IDControllo).data("kendoDropDownList") == undefined) {
        let ddlSpecie = await RiempiDdlSpecie();
        $(IDControllo).kendoDropDownList({
            filter: "contains",
            dataSource: ddlSpecie,
            dataTextField: "veg_des",
            dataValueField: "veg_cod",
            optionLabel: { "veg_des": TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "TutteLeSpecieVegetali", "Tutte le Specie"), "veg_cod": "-1" },
            autoBind: true,
            dataBound: function (e) {
            },
            change: function () {
                popolaDdlImpianti("#ddlImpianti");
            }
        });

        let dropDown = $(IDControllo).data("kendoDropDownList");
        var preferenza = $.cookie("MenuBS_Agenda_Nuovo.FiltroSpecie");
        if (preferenza !== undefined && operazioniImpianto == false) {
            dropDown.value(preferenza);
            //dropDown.trigger("change");
            deferred.resolve();
            if (dropDown.value() === "-1") { //Se non ho trovato il valore - probabilmente ho cambiato impresa...
                $.removeCookie("MenuBS_Agenda_Nuovo.FiltroSpecie");
            }
            return;
        } else if (operazioniImpianto == true) {
            dropDown.value(obj_Agenda.Veg_Cod);
            //dropDown.trigger("change");
            deferred.resolve();
            return;
        }

        if (deferred !== undefined && operazioniImpianto == false) {
            deferred.resolve();
        }

    } else {
        let ddlSpecie = await RiempiDdlSpecie();
        $(IDControllo).data("kendoMultiSelect").setDataSource(ddlSpecie);
        $(IDControllo).data("kendoMultiSelect").refresh();
    }


}

async function popolaDdlTipoOperazione(IDControllo, deferred) {

    if ($(IDControllo).data("kendoMultiSelect") == undefined) {
        let ddlTipoOperazioni = await RiempiDdlTipoOperazione();
        $(IDControllo).kendoMultiSelect({
            filter: "contains",
            dataSource: {
                data: ddlTipoOperazioni,
                group: { field: "tipo" }
            },
            autoClose: false,
            //dataSource: ddlTipoOperazioni,
            dataTextField: "gru_des",
            dataValueField: "gru_cod",
            placeholder: TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "Tutte", "Tutte"),
            autoBind: true,
            dataBound: function (e) {
                //var preferenza = $.cookie("MenuBS_Agenda_Nuovo.FiltroSpecie");
                //if (preferenza !== undefined) {
                //    e.sender.value(preferenza);
                //    if (e.sender.value() === "-1") { //Se non ho trovato il valore - probabilmente ho cambiato impresa...
                //        $.removeCookie("MenuBS_Agenda_Nuovo.FiltroSpecie");
                //    }
                //}

                if (deferred !== undefined) {
                    deferred.resolve();
                }
            }
        });

        let dropDown = $(IDControllo).data("kendoMultiSelect");
        var preferenza = "[" + $.cookie("MenuBS_Agenda_Nuovo.FiltroTipoOperazioni") + "]";
        if (preferenza !== undefined && operazioniImpianto == false) {
            if (isJSONValid(preferenza)) {
                dropDown.value(JSON.parse(preferenza));
                dropDown.trigger("change");
                if (dropDown.value() === "-1") { //Se non ho trovato il valore - probabilmente ho cambiato impresa...
                    $.removeCookie("MenuBS_Agenda_Nuovo.FiltroTipoOperazioni");
                }
            }
        } else if (operazioniImpianto == true) {
            //dropDown.value(obj_Agenda.Veg_Cod);
            //dropDown.trigger("change");
            deferred.resolve();
        }

        if (deferred !== undefined && operazioniImpianto == false) {
            deferred.resolve();
        }
    } else {

    }
}

function popolaDdlImpianti(IDControllo, deferred) {

    if ($(IDControllo).data("kendoMultiSelect") == undefined) {
        $(IDControllo).kendoMultiSelect({
            filter: "contains",
            dataSource: { transport: { read: RiempiDdlImpianti } },
            dataTextField: "des",
            dataValueField: "chiave",
            placeholder: TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "Tutti", "Tutti"),
            autoBind: true,
            autoClose: false,
            dataBound: function (e) {
                //var preferenza = $.cookie("MenuBS_Agenda_Nuovo.FiltroSpecie");
                //if (preferenza !== undefined) {
                //    e.sender.value(preferenza);
                //    if (e.sender.value() === "-1") { //Se non ho trovato il valore - probabilmente ho cambiato impresa...
                //        $.removeCookie("MenuBS_Agenda_Nuovo.FiltroSpecie");
                //    }
                //}
                if (operazioniImpianto) {
                    let chiave = obj_Agenda.Piva + "_" + obj_Agenda.Sa_Cod + "_" + obj_Agenda.Appezza + "_" + obj_Agenda.Id_Imp
                    let arr = new Array();
                    arr.push(chiave);
                    this.value(arr);
                    if (this.value().length > 0) {
                        deferred.resolve();
                    }
                }

                if (deferred !== undefined && operazioniImpianto == false) {
                    deferred.resolve();
                }
            }
        });
    } else {
        $(IDControllo).data("kendoMultiSelect").dataSource.read();
        $(IDControllo).data("kendoMultiSelect").refresh();
    }

}

//function DdlSpecie_Change(e) {
//    var veg_cod = e.sender.value();
//    var parametri = kendo.stringify({ "veg_cod": veg_cod });

//    ajaxAgronicaSync("MenuBS_Agenda_Nuovo.aspx/DdlSpecie_Change",
//        parametri, false,
//        function (risposta) {
//            risp = JSON.parse(risposta.RispostaStringa);
//        }, null);
//}

function SalvaSuCookieCriteriDiRicerca() {

    if (operazioniImpianto == false) {
        var date = new Date();
        date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
        //var date = 0;

        var data_inizio = $(data_inizio_ClientID).val();
        if (data_inizio !== undefined && data_inizio !== "") {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroDataInizio");
            $.cookie("MenuBS_Agenda_Nuovo.FiltroDataInizio", data_inizio, { expires: date, path: '/' });
        }

        var data_fine = $(data_fine_ClientID).val();
        if (data_fine !== undefined && data_fine !== "") {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroDataFine");
            $.cookie("MenuBS_Agenda_Nuovo.FiltroDataFine", data_fine, { expires: date, path: '/' });
        }

        var sa_cod = $('#ddlCentri').val();
        if (sa_cod === "0") {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroCentro");
        } else if (sa_cod !== undefined) {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroCentro");
            $.cookie("MenuBS_Agenda_Nuovo.FiltroCentro", pivaAziendaSelezionata + "|" + sa_cod/*, { expires: date }*/, { path: '/' });
        }

        var veg_cod = $('#ddlSpecie').val();
        if (veg_cod === "-1") {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroSpecie");
            $.cookie("MenuBS_Agenda_Nuovo.FiltroSpecie", veg_cod/*, { expires: date }*/, { path: '/' });
        } else if (veg_cod !== undefined) {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroSpecie");
            $.cookie("MenuBS_Agenda_Nuovo.FiltroSpecie", veg_cod/*, { expires: date }*/, { path: '/' });
        }

        var tipoOperazioneSel = $("#ddlTipoOperazione").data("kendoMultiSelect").value();
        if (tipoOperazioneSel.length == 0) {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroTipoOperazioni");
            $.cookie("MenuBS_Agenda_Nuovo.FiltroTipoOperazioni", tipoOperazioneSel, { path: '/' });
        } else if (tipoOperazioneSel !== undefined) {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroTipoOperazioni");
            $.cookie("MenuBS_Agenda_Nuovo.FiltroTipoOperazioni", tipoOperazioneSel, { path: '/' });
        }

    }

}

function ripristinaPreferenzeFiltriDateDaCookie() {

    var preferenzaDataInizio = $.cookie("MenuBS_Agenda_Nuovo.FiltroDataInizio");
    if (preferenzaDataInizio !== undefined) {
        $(data_inizio_ClientID).val(preferenzaDataInizio);
        if ($(data_inizio_ClientID).val() === "") {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroDataInizio");
        }
    }

    var preferenzaDataFine = $.cookie("MenuBS_Agenda_Nuovo.FiltroDataFine");
    if (preferenzaDataFine !== undefined) {
        $(data_fine_ClientID).val(preferenzaDataFine);
        if ($(data_fine_ClientID).val() === "") {
            $.removeCookie("MenuBS_Agenda_Nuovo.FiltroDataFine");
        }
    }

}

function btnAggiorna_click(deferred) {

    listaIdAgendaInPage = "";

    SalvaSuCookieCriteriDiRicerca();

    //Pulisco tutte le griglie
    distruggiGrigliaKendo('#divKendoOperazioni');
    distruggiGrigliaKendo('#divKendoOperazioniDettagliDestinazioni');
    distruggiGrigliaKendo('#divKendoRicette');
    distruggiGrigliaKendo('#divKendoBrogliaccio');
    distruggiGrigliaKendo('#divKendoColturali');
    distruggiGrigliaKendo('#divKendoMagCont');
    distruggiGrigliaKendo('#divKendoAudit');
    distruggiGrigliaKendo('#divKendoMacchine');
    distruggiGrigliaKendo('#divKendoZoo');

    //Leggo il numero di operazioni per tab
    LeggiNumeroOperazioniInTab();

    //Leggo da cookie l'ultimo tab aperto
    var idTab = GetIdTab();

    //Carico la griglia della tab attiva
    switch (idTab) {
        case 'linkTutte':
            var TipoGrigliaQDC = $("#ddlTipoGrigliaOperazioni").val();
            switch (TipoGrigliaQDC) {
                case "1":
                    CaricaGrigliaOperazioni(deferred);
                    break;
                case "2":
                    CaricaGrigliaOperazioniDestinazioneDettagli(deferred, true);
                    break;
                case "3":
                    CaricaGrigliaOperazioniDestinazioneDettagli(deferred, false);
                    break;
                default:
                    CaricaGrigliaOperazioni(deferred);
                    break;
            }
            break;
        case 'linkRicette':
            CaricaGrigliaRicette(deferred);
            break;
        case 'linkBrogliaccio':
            CaricaGrigliaBrogliaccio(deferred);
            break;
        case 'linkColturali':
            CaricaGrigliaColturali(deferred);
            break;
        case 'linkMagCont':
            CaricaGrigliaMagCont(deferred);
            break;
        case 'linkAudit':
            CaricaGrigliaAudit(deferred);
            break;
        case 'linkMacchine':
            CaricaGrigliaMacchine(deferred);
            break;
        case 'linkZoo':
            CaricaGrigliaZoo(deferred);
            break;
    }

}

function bloccaOperazioni() {

    var arraySelected = elementiGrigliaSelezionati();

    if (arraySelected.length > 0) {

        var msg = 'Sei sicuro di voler bloccare l\'operazione selezionata?';
        if (arraySelected.length > 1) {
            msg = 'Sei sicuro di voler bloccare le ' + arraySelected.length + ' operazioni selezionate?';
        }

        var dlg = $("<div></div>").kendoConfirm({
            content: msg,
            messages: { okText: "Sì", cancel: "No" },
            title: "Conferma"
        }).data("kendoConfirm");

        dlg.result.done(function () { Gestione_Operazione_Menu('45'); });
        dlg.open();

    } else {
        kendo.alert('Selezionare almeno un\'operazione da bloccare');
    }
}

function sbloccaOperazioni() {
    var arraySelected = elementiGrigliaSelezionati();

    if (arraySelected.length > 0) {

        var msg = 'Sei sicuro di voler sbloccare l\'operazione selezionata?';
        if (arraySelected.length > 1) {
            msg = 'Sei sicuro di voler sbloccare le ' + arraySelected.length + ' operazioni selezionate?';
        }

        var dlg = $("<div></div>").kendoConfirm({
            content: msg,
            messages: { okText: "Sì", cancel: "No" },
            title: "Conferma"
        }).data("kendoConfirm");

        dlg.result.done(function () { Gestione_Operazione_Menu('46'); });
        dlg.open();

    } else {
        kendo.alert('Selezionare almeno un\'operazione da sbloccare');
    }
}

function caricaPreferiti(deferred) {

    var IDControllo;
    var funzioneLetturaDati;
    var funzioneRedirect;
    var tool;
    var loadPreferiti = true
    //Leggo da cookie l'ultimo tab aperto
    var idTab = GetIdTab();

    //carico le operazioni corrispondenti
    switch (idTab) {
        case 'linkTutte':
        case 'linkColturali':
        case 'linkMagCont':
        case 'linkAudit':
        case 'linkMacchine':
            IDControllo = "#lsbPreferiti";
            funzioneLetturaDati = Leggi_Operazioni_Preferite;
            funzioneRedirect = redirectOperazionePreferita;
            tool = { position: "top", tools: ["moveUp", "moveDown", "remove"] };
            break;
        case 'linkZoo':
            IDControllo = "#lsbPreferitiZoo";
            funzioneLetturaDati = Leggi_Operazioni_Preferite_PerZoo;
            funzioneRedirect = redirectOperazionePreferita;
            tool = { position: "top", tools: ["moveUp", "moveDown", "remove"] };
            break;
        case 'linkRicette':
            IDControllo = "#lsbPreferitiRicette";
            funzioneLetturaDati = Leggi_Operazioni_Preferite_PerRicette;
            funzioneRedirect = redirectOperazioneRicettaPreferita;
            tool = {};
            if (UtenteAbilitatoRicette == false) //Il componente HTML lsbPreferitiRicette esiste solo se l'utente ha i permessi di scrittura
                loadPreferiti = false
            break;
        case 'linkBrogliaccio':
            IDControllo = "#lsbPreferitiBrogliaccio";
            funzioneLetturaDati = Leggi_Operazioni_Preferite_PerBrogliaccio;
            funzioneRedirect = redirectOperazioneBrogliaccioPreferita;
            tool = {};
            if (UtenteAbilitatoBrogliaccio == false) //Il componente HTML lsbPreferitiBrogliaccio esiste solo se l'utente ha i permessi di scrittura
                loadPreferiti = false
            break;
    }

    if ($(IDControllo).data("kendoListBox") === undefined && loadPreferiti == true) {

        var lsb = $(IDControllo).kendoListBox({
            draggable: { enabled: true },
            toolbar: tool,
            messages: {
                tools: {
                    moveUp: "Sposta Su",
                    moveDown: "Sposta Giù",
                    remove: "Elimina"
                }
            },
            dataSource: { transport: { read: funzioneLetturaDati } },
            dataTextField: "lav_des",
            dataValueField: "lav_cod",
            change: function (e) {
                if ($('#btnSalvaPreferiti').is(":visible") === false) {
                    var element = e.sender.select();
                    var dataItem = e.sender.dataItem(element[0]);
                    funzioneRedirect(dataItem.lav_cod);
                }
            },
            dataBound: function () {
                if ($('#btnSalvaPreferiti').is(":visible") === true) {
                    apriToolbarPreferiti(IDControllo);
                } else {
                    chiudiToolbarPreferiti(IDControllo);
                }

                if (deferred !== undefined) {
                    deferred.resolve();
                }
            },
            reorder: function (e) {
                var dataSource = $.extend({}, e.sender.dataSource);
                var dataItem = $.extend({}, e.dataItems[0]);
                var index = dataSource.indexOf(dataItem) + e.offset;
                dataSource.remove(dataItem);
                dataSource.insert(index, dataItem);
                e.sender.select(e.sender.items()[index]);
            }
        });

    }
    else {

        if (deferred !== undefined) {
            deferred.resolve();
        }
        return;

    }

    //Al primo avvio, nascondo la toolbar
    chiudiToolbarPreferiti(IDControllo);

}

function AggiungiAiPreferiti(lav_cod, lav_des) {
    var lsb = $("#lsbPreferiti").data("kendoListBox");
    lsb.add({ "lav_cod": lav_cod, "lav_des": lav_des });
}

function apriToolbarPreferiti(IDControllo) {
    if (IDControllo == '#lsbPreferitiZoo') {
        $('#pnlPreferitiZoo div.k-listbox-toolbar').show();
        $('#btnApriToolbarPreferiti').hide();
        $('#btnSalvaPreferiti').show();
    } else {
        $('#pnlPreferiti div.k-listbox-toolbar').show();
        $('#btnApriToolbarPreferiti').hide();
        $('#btnSalvaPreferiti').show();
    }
    var lsb = $(IDControllo).data("kendoListBox");
    lsb.options.draggable.enabled = true;
}

function apriToolbarPreferiti_EstraiIDControlloLSB() {
    let IDControllo = estraiIDControlloLSB();
    apriToolbarPreferiti(IDControllo);
}

function chiudiToolbarPreferiti(IDControllo) {
    if (IDControllo == '#lsbPreferitiZoo') {
        $('#btnSalvaPreferiti').hide();
        $('#pnlPreferitiZoo div.k-listbox-toolbar').hide();
        $('#btnApriToolbarPreferiti').show();
    } else {
        $('#btnSalvaPreferiti').hide();
        $('#pnlPreferiti div.k-listbox-toolbar').hide();
        $('#btnApriToolbarPreferiti').show();
    }
    var lsb = $(IDControllo).data("kendoListBox");
    if (lsb !== undefined) {
        lsb.options.draggable.enabled = false;
    }
}

function salvaPreferiti_EstraiIDControlloLSB() {
    let IDControllo = estraiIDControlloLSB();
    salvaPreferiti(IDControllo);
}

function salvaPreferiti(IDControllo) {
    chiudiToolbarPreferiti(IDControllo);

    if (IDControllo === '#lsbPreferitiZoo') {
        Salva_OperazioniZoo_Preferite();
    } else {
        Salva_Operazioni_Preferite();
    }
}

function estraiIDControlloLSB() {
    let IDControllo;

    //Leggo da cookie l'ultimo tab aperto
    var idTab = GetIdTab();

    //carico le operazioni corrispondenti
    switch (idTab) {
        case 'linkTutte':
        case 'linkColturali':
        case 'linkMagCont':
        case 'linkAudit':
        case 'linkMacchine':
            IDControllo = "#lsbPreferiti";
            break;
        case 'linkZoo':
            IDControllo = "#lsbPreferitiZoo";
            break;
        case 'linkRicette':
            IDControllo = "#lsbPreferitiRicette";
            break;
        case 'linkBrogliaccio':
            IDControllo = "#lsbPreferitiBrogliaccio";
            break;
    }

    return IDControllo;
}

function impostaTabPreselezionata() {

    let id = 0;
    switch (defaultTab) {
        case 1:
            id = 'linkTutte';
            break;
        case 2:
            id = 'linkColturali';
            break;
        case 3:
            id = 'linkMagCont';
            break;
        case 4:
            id = 'linkAudit';
            break;
        case 5:
            id = 'linkZoo';
            break;
        case 6:
            id = 'linkMacchine';
            break;
        case 7:
            id = 'linkRicette';
            break;
        case 8:
            id = 'linkBrogliaccio';
            break;
    }

    if (id !== 0) {
        $.cookie("MenuBS_Agenda_Nuovo.tabDati", id);
    }

}

function SalvaTabSuCookie(obj) {
    var id = obj.id;
    $.cookie("MenuBS_Agenda_Nuovo.tabDati", id);
}

function CaricaDatiGriglia(deferred) {

    //Leggo da cookie l'ultimo tab aperto
    var idTab = GetIdTab();

    //attivo il tab selezionato
    $('#' + idTab).tab('show');

    //carico la griglia corrispondente
    switch (idTab) {
        case 'linkTutte':
            CaricaGrigliaOperazioni(deferred);
            break;
        case 'linkRicette':
            CaricaGrigliaRicette(deferred);
            break;
        case 'linkBrogliaccio':
            CaricaGrigliaBrogliaccio(deferred);
            break;
        case 'linkColturali':
            CaricaGrigliaColturali(deferred);
            break;
        case 'linkMagCont':
            CaricaGrigliaMagCont(deferred);
            break;
        case 'linkAudit':
            CaricaGrigliaAudit(deferred);
            break;
        case 'linkMacchine':
            CaricaGrigliaMacchine(deferred);
            break;
        case 'linkZoo':
            CaricaGrigliaZoo(deferred);
            break;
    }

}

function mostraPannelloSX() {

    //Leggo da cookie l'ultimo tab aperto
    var idTab = GetIdTab();

    //mostro e nascondo i controlli corrispondenti
    switch (idTab) {
        case 'linkTutte':
        case 'linkColturali':
        case 'linkMagCont':
        case 'linkAudit':
        case 'linkMacchine':
            $("#pnlSxRicette").hide();
            $("#pnlSxBrogliaccio").hide();

            $("#pnlPreferitiZoo").hide();
            $("#pnlddlOperazioniZoo").hide();

            $("#pnlGestionePreferiti").show();
            $("#pnlSxOperazioni").show();
            $("#pnlPreferiti").show();
            $("#pnlddlOperazioni").show();
            break;
        case 'linkZoo':
            $("#pnlSxRicette").hide();
            $("#pnlSxBrogliaccio").hide();

            $("#pnlGestionePreferiti").show();
            $("#pnlPreferiti").hide();
            $("#pnlddlOperazioni").hide();

            $("#pnlSxOperazioni").show();
            $("#pnlPreferitiZoo").show();
            $("#pnlddlOperazioniZoo").show();
            break;
        case 'linkRicette':
            $("#pnlSxOperazioni").hide();
            $("#pnlSxBrogliaccio").hide();
            $("#pnlSxRicette").show();
            break;
        case 'linkBrogliaccio':
            $("#pnlSxOperazioni").hide();
            $("#pnlSxRicette").hide();
            $("#pnlSxBrogliaccio").show();
            break;
    }

}

function GetIdTab() {

    //Leggo da cookie l'ultimo tab aperto
    var idTab = $.cookie("MenuBS_Agenda_Nuovo.tabDati");

    if (idTab !== undefined && $('#' + idTab).length > 0) {
        return idTab;

    } else {
        //Se non c'è salvato nulla sul cookie, apro il tab 'tutte'
        //TODO: prendere la prima tab..
        return 'linkTutte';
    }
}

function tab_onClick(objTab) {

    //Salvo il fatto che ho aperto la tab su cookie
    SalvaTabSuCookie(objTab);

    //carico la griglia
    CaricaDatiGriglia();

    //Mostro e nascondo il pannello dei preferiti/Altre Operazioni in base alle operazioni colturali o ricette
    mostraPannelloSX();

    //Carico i preferiti
    caricaPreferiti();

    //Carico la dropdown delle operazioni
    popolaDdlOperazioni();
}

function distruggiGrigliaKendo(div_idControllo) {
    if ($(div_idControllo).data("kendoGrid") !== undefined) {
        $(div_idControllo).data("kendoGrid").destroy();
        $(div_idControllo).empty();
    }
}

function distruggiRicreaListboxKendo(lsb_idControllo, divPadre_idControllo, htmlDaRipristinare) {
    if ($(lsb_idControllo).data("kendoListBox") !== undefined) {
        $(lsb_idControllo).data("kendoListBox").destroy();
        $(divPadre_idControllo).html(htmlDaRipristinare);
    }
}

function btnNuovaOperazione_click() {

    //Leggo da cookie l'ultimo tab aperto
    var idTab = GetIdTab();

    var idControllo;

    switch (idTab) {
        case 'linkTutte':
        case 'linkColturali':
        case 'linkMagCont':
        case 'linkAudit':
        case 'linkMacchine':
            idControllo = "#ddlOperazioni";
            break;
        case 'linkZoo':
            idControllo = "#ddlOperazioniZoo";
            break;
    }

    var op = $(idControllo).data("kendoDropDownList");
    var lavcod = op.value();

    if (lavcod === "") {
        alert(TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "SelezionareUnOperazione", 'Selezionare un\'operazione.'));
    } else {
        redirectOperazionePreferita(lavcod);
    }
}

function isJSONValid(text) {
    if (/^[\],:{}\s]*$/.test(text.replace(/\\["\\\/bfnrtu]/g, '@').
        replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
        replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

        return true;

    } else {

        return false;

    }
}

function infoElemento(tr_elem, grid_elem, ID_Attivita) {


    var permessoLettura = true

    if (ID_Attivita !== enumAgenda) //L'agenda comprende anche i movimenti contabili, che hanno dei permessi specifici all'interno della funzione
        permessoLettura = check_permessoOperazione(ID_Attivita, enumLettura);

    if (permessoLettura == true) {

        //    if (tr_elem.length != 1 || grid_elem.length != 1) {
        //        console.log('trovato con i parent + di 1 elemento (prima usavo closest ma non è supportato da IE), riverificare codice JS, forse cambiate primitive...');
        //        return;
        //    }

        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);
        var type = '0'; //0=info - 2=edit

        var data = datiRiga.chiave_composita.split("_")[0];
        var id_agenda = datiRiga.chiave_composita.split("_")[1];
        var lav_cod = datiRiga.chiave_composita.split("_")[2];
        var raccoglitore_cod = datiRiga.Raccoglitore_Cod;
        if (raccoglitore_cod == undefined) {
            raccoglitore_cod = 0;
        }
        //var piva = datiRiga.chiave_composita.split("_")[3];
        //var sa_cod = datiRiga.chiave_composita.split("_")[4];
        var blocco_flag = datiRiga.chiave_composita.split("_")[5];
        var veg_cod = datiRiga.chiave_composita.split("_")[6];

        if (veg_cod === "") {
            veg_cod = "0";
        }

        if (Request_QueryString("gis") === "true") {
            window.parent.preselezioneAgenda(-1, undefined, id_agenda, Enum_TipoOperazioneDB.Lettura.value);
        } else {

            var parametri = kendo.stringify({
                "type": type,
                "data": data,
                "id_agenda": id_agenda,
                "lav_cod": lav_cod,
                "blocco_flag": blocco_flag,
                "veg_cod": veg_cod,
                "ID_Attivita": ID_Attivita,
                "raccoglitore_cod": raccoglitore_cod
            });

            $.ajax({
                type: 'POST',
                url: 'MenuBS_Agenda_Nuovo.aspx/infomodifica_operazione_singola',
                data: parametri,
                contentType: 'application/json; charset=utf-8',
                cache: false, dataType: 'json', async: true,
                success: function (r) {

                    if (r.d.RispostaOK)
                        window.location = r.d.RispostaStringa;
                    else
                        MessaggioErrore(r.d.Errore);
                }
            });
        }
    }
}

function modificaElemento(tr_elem, grid_elem, ID_Attivita) {

    var permessoModifica = true

    if (ID_Attivita !== enumAgenda) //L'agenda comprende anche i movimenti contabili, che hanno dei permessi specifici all'interno della funzione
        permessoModifica = check_permessoOperazione(ID_Attivita, enumModifica);

    if (permessoModifica == true) {
        //    if (tr_elem.length != 1 || grid_elem.length != 1) {
        //        console.log('trovato con i parent + di 1 elemento (prima usavo closest ma non è supportato da IE), riverificare codice JS, forse cambiate primitive...');
        //        return;
        //    }

        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);
        var type = '2'; //0=info - 2=edit

        var data = datiRiga.chiave_composita.split("_")[0];
        var id_agenda = datiRiga.chiave_composita.split("_")[1];
        var lav_cod = datiRiga.chiave_composita.split("_")[2];
        var raccoglitore_cod = datiRiga.Raccoglitore_Cod;
        if (raccoglitore_cod == undefined) {
            raccoglitore_cod = 0;
        }
        //var piva = datiRiga.chiave_composita.split("_")[3];
        //var sa_cod = datiRiga.chiave_composita.split("_")[4];
        var blocco_flag = datiRiga.chiave_composita.split("_")[5];
        var veg_cod = datiRiga.chiave_composita.split("_")[6];


        if (veg_cod === "") {
            veg_cod = "0";
        }

        if (blocco_flag === '1') {
            MessaggioErrore('Impossibile modificare l\'operazione selezionata poichè risulta Bloccata!');

        } else {

            if (Request_QueryString("gis") === "true") {
                window.parent.preselezioneAgenda(-1, undefined, id_agenda, Enum_TipoOperazioneDB.Modifica.value);

            } else {

                var parametri = kendo.stringify({
                    "type": type,
                    "data": data,
                    "id_agenda": id_agenda,
                    "lav_cod": lav_cod,
                    "blocco_flag": blocco_flag,
                    "veg_cod": veg_cod,
                    "ID_Attivita": ID_Attivita,
                    "raccoglitore_cod": raccoglitore_cod
                });

                $.ajax({
                    type: 'POST',
                    url: 'MenuBS_Agenda_Nuovo.aspx/infomodifica_operazione_singola',
                    data: parametri,
                    contentType: 'application/json; charset=utf-8',
                    cache: false, dataType: 'json', async: true,
                    success: function (r) {

                        if (r.d.RispostaOK)
                            window.location = r.d.RispostaStringa;
                        else
                            MessaggioErrore(r.d.Errore);
                    }
                });
            }
            //esiste frame parent
        }
    }
}

function eliminaElemento(tr_elem, grid_elem, ID_Attivita) {

    var permessoCancellazione = true
    if (ID_Attivita !== enumAgenda) //L'agenda comprende anche i movimenti contabili, che hanno dei permessi specifici all'interno della funzione
        permessoCancellazione = check_permessoOperazione(ID_Attivita, enumCancellazione);

    if (permessoCancellazione == true) {

        //    if (tr_elem.length != 1 || grid_elem.length != 1) {
        //        console.log('trovato con i parent + di 1 elemento (prima usavo closest ma non è supportato da IE), riverificare codice JS, forse cambiate primitive...');
        //        return;
        //    }

        var datiGriglia = $(grid_elem).data('kendoGrid');

        //Estraggo gli elementi selezionati
        var arraySelected = elementiGrigliaSelezionati(datiGriglia);

        //concateno gli elementi selezionati
        var chiavi_composite_checked = "";

        var msg = 'Sei sicuro di voler eliminare l\'operazione selezionata?';

        //Se ce ne sono di selezionati, prendo quelli, altrimenti prendo solo l'elemento di riga del click
        if (arraySelected.length > 0) {

            for (var iRow in arraySelected) {

                //verifico che non ci siano operazioni bloccate
                var blocco_flag = arraySelected[iRow].chiave_composita.split("_")[5];
                if (blocco_flag === '1') {
                    MessaggioErrore('Impossibile eliminare: una o più operazioni selezionate risultano bloccate!');
                    return;
                }

                chiavi_composite_checked += arraySelected[iRow].chiave_composita + ',';
            }

            //elimino l'ultima virgola
            if (chiavi_composite_checked !== "") {
                chiavi_composite_checked = chiavi_composite_checked.slice(0, -1);
            }

            if (arraySelected.length > 1)
                msg = 'Sei sicuro di voler eliminare le ' + arraySelected.length + ' operazioni selezionate?';

        } else {

            let datiRiga = datiGriglia.dataItem(tr_elem);

            let blocco_flag = datiRiga.chiave_composita.split("_")[5];
            if (blocco_flag === '1') {
                if (datiRiga.Lav_cod == '3004' || datiRiga.Lav_cod == '3035' || datiRiga.lav_cod == '3037') {
                    if (datiRiga.Tipo_Accettazione == 1) {
                        msg = 'Sei sicuro di voler eliminare l\'operazione selezionata?\nQuesta operazione è legata uno scarico già registrato su BDN';
                    } else {
                        msg = 'Sei sicuro di voler eliminare l\'operazione selezionata?\nQuesta operazione è legata uno scarico generato su BDN';
                    }
                } else {
                    MessaggioErrore('Impossibile eliminare l\'operazione selezionata poichè risulta bloccata!');
                    return;
                }
            }
            chiavi_composite_checked = datiRiga.chiave_composita;
        }

        //Mostro l'alert e nel caso cancello...
        SalvaParametriDiv('#frmInput', false);

        //ConfermaControlliSiNo(msg, 'del_elem|' + chiavi_composite_checked);
        messaggioConfermaKendo(msg, 'del_elem|' + chiavi_composite_checked);

    }
}

function messaggioConfermaKendo(messaggio, str ) {
    kendo.confirm(messaggio)
        .done(function () {
            DoPostBack_ControlliSiNo(str)
        })
        .fail(function () {          
        });
}

function duplicaElemento(tr_elem, grid_elem, ID_Attivita) {

    var permessoCopia = check_permessoOperazione(ID_Attivita, -1); //-1 = Copia Elemento

    if (permessoCopia == true) {
        //    if (tr_elem.length != 1 || grid_elem.length != 1) {
        //        console.log('trovato con i parent + di 1 elemento (prima usavo closest ma non è supportato da IE), riverificare codice JS, forse cambiate primitive...');
        //        return;
        //    }

        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);

        var data = datiRiga.chiave_composita.split("_")[0];
        var id_agenda = datiRiga.chiave_composita.split("_")[1];
        var lav_cod = datiRiga.chiave_composita.split("_")[2];
        var piva = datiRiga.chiave_composita.split("_")[3];
        var sa_cod = datiRiga.chiave_composita.split("_")[4];
        //var blocco_flag = datiRiga.chiave_composita.split("_")[5];
        //var veg_cod = datiRiga.chiave_composita.split("_")[6];

        //Estraggo gli elementi selezionati
        var arraySelected = elementiGrigliaSelezionati(datiGriglia);

        //concateno gli elementi selezionati
        var id_agenda_checked = "";
        var lav_cod_checked = "";

        for (var iRow in arraySelected) {
            id_agenda_checked += arraySelected[iRow].id_agenda + ',';
            lav_cod_checked += arraySelected[iRow].Lav_cod + ',';
        }

        if (id_agenda_checked === "") {
            id_agenda_checked = id_agenda + ",";
        }

        if (lav_cod_checked === "") {
            lav_cod_checked = lav_cod + ",";
        }

        id_agenda_checked += "-1";
        lav_cod_checked += "-1";

        var parametri = kendo.stringify({
            "data": data,
            "id_agenda_checked": id_agenda_checked,
            "id_agenda": id_agenda,
            "lav_cod_checked": lav_cod_checked,
            "lav_cod": lav_cod,
            "piva": piva,
            "sa_cod": sa_cod,
            "ID_Attivita": ID_Attivita
        })

        $.ajax({
            type: 'POST',
            url: 'MenuBS_Agenda_Nuovo.aspx/copia_operazione_singola',
            data: parametri,
            contentType: 'application/json; charset=utf-8',
            cache: false, dataType: 'json', async: true,
            success: function (r) {

                if (r.d.RispostaOK)
                    window.location = r.d.RispostaStringa;
                else
                    MessaggioErrore(r.d.Errore);

            }
        });
    }
}

function inviaModello4(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    //var datiGriglia = $('#divKendoZoo').data('kendoGrid');

    //Estraggo gli elementi selezionati
    var dati = datiGriglia.dataSource.data();
    var operazioniAgenda = [];
    var alerted = false;

    //le uniche operazioni che possono generare un modello 4:
    //  - non devono averne già generato uno (blocco_flag = 0)
    //  - devono essere operazioni di macellazione (3004), vendita (3035) o trasferimento (3037)
    //  - se il tipo di trasporto è S o N, devono avere tutti i dati legati al trasporto presenti
    for (var i = 0; i < dati.length; i++) {
        if (dati[i].Selected) {
            if (dati[i].blocco_flag == 1) {
                alert("Non puoi selezionare operazioni già sincronizzate con un modello 4");
                alerted = true;
                break;
            }

            if (dati[i].Lav_cod == "3004" || dati[i].Lav_cod == "3035" || dati[i].Lav_cod == "3037") {
                operazioniAgenda.push(dati[i].Piva + "_" + dati[i].id_agenda);
            } else {
                alert("Puoi selezionare solo operazioni di vendita o macellazione capi");
                alerted = true;
                break;
            }
        }
    }

    if (!alerted && operazioniAgenda.length > 0) {
        var parametri = {
            chiave: JSON.stringify(operazioniAgenda)
        };

        ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/getURLGeneraModello4", JSON.stringify(parametri), function (risposta) {
            window.location = risposta.RispostaStringa;
        }, null);
    } else if (operazioniAgenda.length == 0 && !alerted) {
        operazioniAgenda.push(datiRiga.Piva + "_" + datiRiga.id_agenda);

        var parametri = {
            chiave: JSON.stringify(operazioniAgenda)
        };

        ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/getURLGeneraModello4", JSON.stringify(parametri), function (risposta) {
            window.location = risposta.RispostaStringa;
        }, null);
    }
}

function registraModello4(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    //var datiGriglia = $('#divKendoZoo').data('kendoGrid');

    //Estraggo gli elementi selezionati
    var dati = datiGriglia.dataSource.data();
    var operazioniAgenda = [];
    var alerted = false;

    //le uniche operazioni che possono registrare un modello 4:
    //  - devono averne già generato uno (blocco_flag = 1)
    //  - devono essere operazioni di macellazione (3004), vendita (3035) o trasferimento (3037)
    //  - se il tipo di trasporto è S o N, devono avere tutti i dati legati al trasporto presenti
    for (var i = 0; i < dati.length; i++) {
        if (dati[i].Selected) {
            if (dati[i].blocco_flag != 1 || dati[i].Tipo_Accettazione == 1) {
                alert("Non puoi selezionare operazioni già sincronizzate con un modello 4");
                alerted = true;
                break;
            }

            if (dati[i].Lav_cod == "3004" || dati[i].Lav_cod == "3035" || dati[i].Lav_cod == "3037") {
                operazioniAgenda.push(dati[i].Piva + "_" + dati[i].id_agenda);
            } else {
                alert("Puoi selezionare solo operazioni di vendita o macellazione capi");
                alerted = true;
                break;
            }
        }
    }

    if (!alerted && operazioniAgenda.length > 0) {
        var parametri = {
            chiave: JSON.stringify(operazioniAgenda)
        };

        ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/getURLRegistraModello4", JSON.stringify(parametri), function (risposta) {
            window.location = risposta.RispostaStringa;
        }, null);
    } else if (operazioniAgenda.length == 0 && !alerted) {
        operazioniAgenda.push(datiRiga.Piva + "_" + datiRiga.id_agenda);

        var parametri = {
            chiave: JSON.stringify(operazioniAgenda)
        };

        ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/getURLRegistraModello4", JSON.stringify(parametri), function (risposta) {
            window.location = risposta.RispostaStringa;
        }, null);
    }
}
