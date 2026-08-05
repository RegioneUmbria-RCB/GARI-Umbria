function get_valueTemplate(versioneKendo, tmplt_r1, tmplt_r2) {
    let valueTemplate = ""

    if (versioneKendo <= 2021) {
        valueTemplate = "<div style='display: flex;'>";
        valueTemplate += tmplt_r1;
        valueTemplate += "</div>";
        valueTemplate += "<div style='display:flex; font-size:11px;'>";
        valueTemplate += tmplt_r2;
        valueTemplate += "</div>";
    } else {
        valueTemplate = "<span style='width:100%; display:inline-block;'>" + tmplt_r1 +
            "            </span>" +
            "            <div style='display:flex; font-size:11px;'>" + tmplt_r2 +
            "            </div> ";
    }

    return valueTemplate
}

function get_listTemplate(versioneKendo, tmplt_r1, tmplt_r2) {

    let listTemplate = ""

    if (versioneKendo <= 2021) {
        listTemplate = "<div style='display: flex; padding-top:3px;'>";
        listTemplate += tmplt_r1;
        listTemplate += "</div>";
        listTemplate += "<div style='display:flex; font-size:11px; padding-bottom:3px;'>";
        listTemplate += tmplt_r2;
        listTemplate += "</div>";
    } else {
        listTemplate = "<span style='width:100%; display:inline-block; padding-top:3px;'>" + tmplt_r1 +
            "           </span>" +
            "           <div style='display:flex; font-size:11px; padding-bottom:3px;'>" + tmplt_r2 +
            "           </div>";

        //    $("ul#cmbOrigineDati_listbox > li > span.k-list-item-text").css("flex-color", "auto");
        //    $("ul#cmbOrigineDati_listbox > li > span.k-list-item-text").css("padding-right", "3px");
    }

    return listTemplate
}


function get_tmplt_r2(versioneKendo) {
    let tmplt_r2 = ""

    if (versioneKendo <= 2021) {
        tmplt_r2 = "<span style='flex-grow:1;'>#= (data.ultimo_aggiornamento) ? kendo.toString(new Date(data.ultimo_aggiornamento), 'd') : '&nbsp;' #</span>";
        tmplt_r2 += "<span>#= (data.rif_fornitore) ? '[' + data.rif_fornitore + ']' : '&nbsp;' #</span>";
    } else {
        tmplt_r2 = "<span style='flex-grow:1;'>" +
            "               #= (data.ultimo_aggiornamento) ? 'Aggiornamento ' + kendo.toString(new Date(data.ultimo_aggiornamento), 'd') : '&nbsp;' #" +
            "           </span>" +
            "           <span>" +
            "               #= (data.rif_fornitore) ? '[' + data.rif_fornitore + ']' : '&nbsp;' #" +
            "           </span>";
    }

    return tmplt_r2
}

function get_tmplt_r1(versioneKendo) {
    let tmplt_r1 = ""

    if (versioneKendo <= 2021) {
        tmplt_r1 = "<span style='font-weight:bold; overflow:hidden; text-overflow:ellipsis; white-space:nowrap; flex-grow:1;'>#= (data.nome_stazione) ? data.nome_stazione : '&nbsp;' #</span>";
        tmplt_r1 += "<span style='font-size:11px; color:\\#9e9e9e;'>#= (data.fornitore) ? data.fornitore : '&nbsp;' #</span>";
    } else {
        tmplt_r1 = "<span style='width:75%; display:inline-block; font-weight:bold; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;'>" +
            "           #= (data.nome_stazione) ? data.nome_stazione : '&nbsp;' #" +
            "       </span>" +
            "       <span style='width:25%; display:inline-block; text-align:right; float:right; font-size:11px; color:\\#9e9e9e; '>" +
            "           #= (data.fornitore) ? data.fornitore : '&nbsp;' #" +
            "       </span>";
    }

    return tmplt_r1
}

function get_tmplt_r1_disp(versioneKendo) {
    let tmplt_r1 = ""

    if (versioneKendo <= 2021) {
        tmplt_r1 = "<span style='font-weight:bold; overflow:hidden; text-overflow:ellipsis; white-space:nowrap; flex-grow:1;'>#= (data.nome_dispositivo) ? data.nome_dispositivo : '&nbsp;' #</span>";
        tmplt_r1 += "<span style='font-size:11px; color:\\#9e9e9e;'>#= (data.fornitore) ? data.fornitore : '&nbsp;' #</span>";
    } else {
        tmplt_r1 = "<span style='width:75%; display:inline-block; font-weight:bold; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;'>" +
            "           #= (data.nome_dispositivo) ? data.nome_dispositivo : '&nbsp;' #" +
            "       </span>" +
            "       <span style='width:25%; display:inline-block; text-align:right; float:right; font-size:11px; color:\\#9e9e9e; '>" +
            "           #= (data.fornitore) ? data.fornitore : '&nbsp;' #" +
            "       </span>";
    }

    return tmplt_r1
}

function LayoutSelezioneHyperMeteo(owner) {
    let posAlert = document.createElement("div");
    posAlert.style.position = "absolute";
    posAlert.style.left = "0";
    posAlert.style.width = "100%";
    posAlert.style.top = "0";
    posAlert.style.height = "100%";
    posAlert.style.display = "flex";
    posAlert.style.alignItems = "center";
    posAlert.style.justifyContent = "center";
    posAlert.style.color = "#9e9e9e";
    posAlert.style.fontSize = "14px";
    owner.span.append(posAlert);

    let spanText1 = document.createElement("span");
    spanText1.innerText = "Clicca";
    posAlert.appendChild(spanText1);

    let spanIcon = document.createElement("span");
    spanIcon.style.fontSize = "20px";
    spanIcon.className = "fa fa-globe fa-fw";
    posAlert.appendChild(spanIcon);

    let spanText2 = document.createElement("span");
    spanText2.innerText = "per geolocalizzare";
    posAlert.appendChild(spanText2);
}

function popolaGriglieDatiAnagrafici(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: ReadAnagrafe
    };

    var idModel = "";
    var campiKendoModel = {
        Id: { editable: false, type: "string" },
        Value: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        { field: "Id", title: "Attributo", hidden: false, filterable: { multi: true, search: true } },
        { field: "Value", title: "Valore", hidden: false, filterable: { multi: true, search: true } }
    ];

    var toolbars = [];

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: false,
        groupable: false,
        reorderable: false,
        columnMenu: false,
        selectable: false,
        pdf: false,
        excel: false,
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

function ReadRows(options) {
    var data = $('#' + hdKendoRowsClientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo);
}

function popolaGrigliaRisultati(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: ReadRows
    };

    var dataModel = $('#' + hdKendoModelClientID).val();
    var dataColumns = $('#' + hdKendoColumsClientID).val();


    var idModel = "";
    var campiKendoModel = JSON.parse(dataModel); // KendoDataSet.kendo_model;
    var colonneKendoGrid = JSON.parse(dataColumns); // KendoDataSet.kendo_columns;

    var toolbars = [];

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: false,
        groupable: false,
        reorderable: false,
        columnMenu: false,
        selectable: false,
        pdf: false,
        scrollable: true,
        resizable: true,
        pageable: false,
        btnEliminaTuttiFiltri: false
    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe_Risultato };
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

function onDataBoundRighe_Risultato(e) {
    //var gridId = e.sender.element[0].id;
    //var grid = $("#" + gridId).data("kendoGrid");
    //for (var i = 0; i < grid.columns.length; i++) {
    //    if (grid.columns[i].width === undefined)
    //        grid.autoFitColumn(i);
    //}
}

function ShowTab(idtab,flag) {
    let tabstrip = $("#tabstrip").getKendoTabStrip();

    let tabToShow = tabstrip.tabGroup.children().eq(idtab);

    tabstrip.enable(tabToShow, flag);

    if (!flag) {
        $(tabstrip.items()[idtab]).addClass("hidden");
    } else {
        $(tabstrip.items()[idtab]).removeClass("hidden");
        tabstrip.select(tabToShow);
    }
}

function VisualizzaFinestra(flag) {
    if (flag === false) {
        $("#id_MainContainer").addClass("transparent");
    } else {
        $("#id_MainContainer").removeClass("transparent");
    }
}

function ShowComponent(flag,idcontrollo) {
    if (flag === false) {
        $("#" + idcontrollo).addClass("hidden");
    } else {
        $("#" + idcontrollo).removeClass("hidden");
    }
}

function ResetImage() {
    $("#_img").remove();
}

function praprazioneSelezioneMeteo() {
    ShowComponent(true,'stazione_meteo');
    creaKendoDropDownList("ddlTipoSorgente", { read: LoadSorgentiMeteo }, "sorgente_des", "sorgente_cod");
    KendoDDL("ddlTipoSorgente").bind('change', onChange_TipoSorgente);
    KendoDDL("ddlTipoSorgente").select(2);
    KendoDDL("ddlTipoSorgente").trigger('change');
}

function onChange_TipoSorgente(e) {
    creaKendoDropDownList("ddlOrigineDati", { read: LeggiStazioniXSorgente }, "id_stazione", "nome_stazione", null, null, null, null, valueTemplate, listTemplate);
    KendoDDL("ddlOrigineDati").enable(true);
    if (KendoDDL("ddlTipoSorgente").value() === "4") {
        KendoDDL("ddlOrigineDati").enable(false);
        var coords = $("#geo-pos-edit-meteo").data("geoPosEdit").getLatLngDec();

        if (coords === null) {
            LayoutSelezioneHyperMeteo(KendoDDL("ddlOrigineDati"));
        }
        
    }
}

function onChange_GruppoConsegna(e) {
    LocalizzazGruppoConsegna();    
}

function ReadRisultatoCalcolo(options) {
    var data = $('#' + hdKendoRowsCalcoloClientID).val();
    options.success(JSON.parse(data));
}

function onChange_chkUseLatLngContatore(e) {
    RiportaCoordinateGruppoConsegna(e, 'geo-pos-edit-meteo');
}

function RiportaCoordinateGruppoConsegna(e, idcontrollo) {
    if (lat !== null && lng !== null) {
        if (e.checked === true) {
            $('#' + idcontrollo).data("geoPosEdit").setLatLngDec(lat, lng);
            KendoDDL("ddlTipoSorgente").trigger('change');
        }
        ShowComponent(!e.checked, 'geo-pos-meteo');
        
    } else {
        kendo.alert("Nessuna coordinata per il gruppo di consegna. Operazione non ammessa");
        e.checked = false;
    }
}

function popolaGrigliaRisultatiCalcolo(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: ReadRisultatoCalcolo
    };

    var dataModel = $('#' + hdKendoModelCalcoloClientID).val();
    var dataColumns = $('#' + hdKendoColumsCalcoloClientID).val();


    var idModel = "";
    var campiKendoModel = JSON.parse(dataModel); // KendoDataSet.kendo_model;
    var colonneKendoGrid = JSON.parse(dataColumns); // KendoDataSet.kendo_columns;

    var toolbars = [];

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: false,
        groupable: false,
        reorderable: false,
        columnMenu: false,
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