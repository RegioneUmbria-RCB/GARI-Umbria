//Variabili globali
var kendo_grid = true;
var operazioniNonEditabili = "3020,3021,3022,3023";
var ricercaSincrona = false;
var tabellaMag = undefined;
var paginaTabellaMag = 0;
var dimensionePaginaTabellaMag = 0;

function MostraFiltriGiacenze() {
    $("#Div_FiltriGiacenze").show();
    $("#Div_FiltriGiacenze2").show();
    $("#Div_Filtri_Movimenti").hide();
    $("#Div_Filtri_Movimenti2").hide();
}

function MostraFiltriMovimenti() {
    $("#Div_FiltriGiacenze").hide();
    $("#Div_FiltriGiacenze2").hide();
    $("#Div_Filtri_Movimenti").show();
    $("#Div_Filtri_Movimenti2").show();
}

function coloraGiacenze() {
    $("#tabGiacenze").find(".giacenza").each(function () {
        var c = "";
        if ($(this).text().replace(",", ".") > 0) {
            c = "trverde";
        }
        if ($(this).text().replace(",", ".") == 0) {
            c = "trgiallo";
        }
        if ($(this).text().replace(",", ".") < 0) {
            c = "trrosso";
        }
        $(this).parent().addClass(c);
    });
}

function kendo_Giacenze_onDataBoundedRighe(eventArgs) {

    var grid = KendoGrid("tabGiacenze");
    var items = eventArgs.sender.items();

    items.each(function (index) {

        var dataItem = grid.dataItem(this);

        if (dataItem.Qta_Mag > 0) {
            this.className += " k-state-verde";
        } else if (dataItem.Qta_Mag < 0) {
            this.className += " k-state-rosso";
        } else {
            this.className += " k-state-giallo";
        }

    });

    //if (grid.dataSource.data().length > 0) {
    //    kendo_AggiustaDimensioneColonne("#tabGiacenze");
    //    grid.resize();
    //}

    /* for (var i = 0; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    } */

}

function coloraMovimenti() {
    //$('#t_impianto table thead tr.filter th').last().hide();
    $('#tabMovimenti table thead tr th input[ncolonna="cau_mov"]').hide();
    $('#tabMovimenti table thead tr th input[ncolonna="cau_mov"]').parent().hide();
    //$('#t_impianto table thead tr.sort th').last().hide();
    $('#tabMovimenti table thead tr.sort th a:contains("cau_mov")').hide();
    $('#tabMovimenti table thead tr.sort th a:contains("cau_mov")').parent().hide();
    $(".cau_mov").hide();

    //$('#t_impianto table thead tr.filter th').last().hide();
    $('#tabMovimenti table thead tr th input[ncolonna="modificabile"]').hide();
    $('#tabMovimenti table thead tr th input[ncolonna="modificabile"]').parent().hide();
    //$('#t_impianto table thead tr.sort th').last().hide();
    $('#tabMovimenti table thead tr.sort th a:contains("modificabile")').hide();
    $('#tabMovimenti table thead tr.sort th a:contains("modificabile")').parent().hide();
    $(".modificabile").hide();
    $("#tabMovimenti").find('thead tr.sort th:nth-last-child(2)').hide();
    $("#tabMovimenti").find('thead tr.filter th:nth-last-child(2)').hide();

    //$('#t_impianto table thead tr.filter th').last().hide();
    $('#tabMovimenti table thead tr th input[ncolonna="eliminabile"]').hide();
    $('#tabMovimenti table thead tr th input[ncolonna="eliminabile"]').parent().hide();
    //$('#t_impianto table thead tr.sort th').last().hide();
    $('#tabMovimenti table thead tr.sort th a:contains("eliminabile")').hide();
    $('#tabMovimenti table thead tr.sort th a:contains("eliminabile")').parent().hide();
    $(".eliminabile").hide();
    $("#tabMovimenti").find('thead tr.sort th:nth-last-child(1)').hide();
    $("#tabMovimenti").find('thead tr.filter th:nth-last-child(1)').hide();

    $("#tabMovimenti").find('thead tr.filter th:nth-last-child(3)').hide();

    $("#tabMovimenti").find(".cau_mov").each(function () {
        var c = "";
        if (listaCauCarico.includes($(this).text().replace(",", "."))) {
            c = "trverde";
        }
        else {
            c = "trrosso";
        }

        $(this).parent().addClass(c);
    });

}

function kendo_Movimenti_onDataBoundedRighe(eventArgs) {

    var grid = KendoGrid("tabMovimenti");
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);
        if (listaCauCarico.includes(dataItem.Cau_Mov)) {
            this.className += " k-state-verde";
        } else {
            this.className += " k-state-rosso";
        }
    });

    // kendo_AggiustaDimensioneColonne("#tabMovimenti");
    /* for (var i = 0; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    } */

}

function ApriAgenda(data, piva, idAgenda, bloccoFlag, lavCod, vegCod, saCod, operazione) {
    WaitFrame.show();
    ajaxAgronica("../Menu/MenuBS_WS.aspx/Gestione_Operazione",
        JSON.stringify({
            Data: data,
            Piva: piva,
            id_Agenda: idAgenda,
            Blocco_Flag: bloccoFlag,
            Lav_Cod: lavCod,
            veg_cod: vegCod,
            sa_cod: saCod,
            MenuAgenda_SelectedValue: operazione,
            PaginaRitorno: "30"
        }),
        function (risposta) {
            if (risposta.IsLink) {
                WaitFrame.show();
                window.location = risposta.RispostaStringa;
            } else alert(risposta.RispostaStringa.replace("ERR", ""));
        }, null);
}

function popolaGiacenze(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: CercaGiacenze,
        checkBoxFunction: SelezionaGiacenze
    };

    var formatQta = "{0:n" + Get_KendoDDLValue("ddlArrotondamento", 2) + "}";
    var formatQtaSum = "n" + Get_KendoDDLValue("ddlArrotondamento", 2) + "";

    var idModel = "chiave_giacenze";
    var campiKendoModel = {
        Sa_Nome: { type: "string" },
        Fabbricato_Des: { type: "string" },
        Cat_Des: { type: "string" },
        Des_Gruppo_Merce: { type: "string" },
        Pro_Cod: { type: "number" },
        Cod_Articolo: { type: "string" },
        Pro_Des: { type: "string" },
        Lotto_Int: { type: "string" },
        Lotto_Acc: { type: "string" },
        Param_Des: { type: "string" },
        Cal_Des: { type: "string" },
        Udm_Des: { type: "string" },
        Qta_Mag: { type: "number" },
        Valore_Unitario: { type: "number" },
        Valore_Globale: { type: "number" }
    };

    var colonneKendoGrid = [
        {
            title: TraduzioneMultiResx(gestioneMagazziniResx, "MostraMovimenti", "Mostra Movimenti"),
            command: [{
                text: " ",
                iconClass: "fa fa-arrows-v",
                click: MostraMovimenti
            }],
            width: 80
        },
        { field: "Sa_Nome", title: TraduzioneMultiResx(gestioneMagazziniResx, "Centro", "Centro"), width: 113, filterable: { multi: true, search: true } },
        { field: "Fabbricato_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "Magazzino", "Magazzino"), width: 113, filterable: { multi: true, search: true } },
        { field: "Cat_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "Categoria", "Categoria"), width: 116, filterable: { multi: true, search: true } },
        { field: "Des_Gruppo_Merce", title: "Gruppo Merce", width: 325, filterable: { multi: true, search: true }, hidden: true },
        { field: "Pro_Cod", title: TraduzioneMultiResx(gestioneMagazziniResx, "CodiceProdotto", "Codice Prodotto"), width: 98, filterable: { multi: true, search: true } },
        { field: "Cod_Articolo", title: TraduzioneMultiResx(gestioneMagazziniResx, "CodiceArticoloAbbr", "Cod Articolo"), width: 118, filterable: { multi: true, search: true } },
        { field: "Pro_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "Prodotto", "Prodotto"), width: 113, filterable: { multi: true, search: true } },
        { field: "Udm_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "UnitàDiMisura", "Unità di Misura"), width: 96, filterable: { multi: true, search: true } },
        { field: "Qta_Mag", title: TraduzioneMultiResx(gestioneMagazziniResx, "Giacenza", "Giacenza"), width: 112, format: formatQta, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, '" + formatQtaSum + "')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, '" + formatQtaSum + "')#</div>" },
        { field: "Lotto_Int", title: TraduzioneMultiResx(gestioneMagazziniResx, "LottoEsercizio", "Lotto Esercizio"), width: 92, filterable: { multi: true, search: true } },
        { field: "Lotto_Acc", title: TraduzioneMultiResx(gestioneMagazziniResx, "LottoProdotto", "Lotto"), width: 86, filterable: { multi: true, search: true } },
        { field: "Param_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "ParametroQualitativo", "Parametro Qualitativo"), width: 113, filterable: { multi: true, search: true } },
        { field: "Cal_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "Qualità", "Qualità"), width: 113, filterable: { multi: true, search: true } }
    ];

    //Valorizzazione
    var aggregateColumns = [{ field: "Qta_Mag", aggregate: "sum" }];
    if (getKendoSwitch("CheckBoxGiacenze0") === false && getKendoSwitch("CheckBoxValorizzaProdotto") === true) {
        aggregateColumns.push({ field: "Valore_Globale", aggregate: "sum" });
        colonneKendoGrid.push({ field: "Valore_Unitario", title: "Valore Unitario", width: 110, format: "{0:n6}", attributes: { style: "text-align:right;" } });
        colonneKendoGrid.push({ field: "Valore_Globale", title: "Valore Globale", width: 110, format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" });
    }

    //Parametri qualitativi
    if (isModuloFFAttivo === true && isCategoriaFF(null) === true) {
        var paramQualGestiti = RicercaParametriQualitativi(true, $(cIdPiva).val());
        for (let iPar = 0; iPar < paramQualGestiti.length; iPar++) {
            if (paramQualGestiti[iPar].Tabella_ID !== 0) {
                if (paramQualGestiti[iPar].Tipo === 1 || paramQualGestiti[iPar].Tipo === 4) {

                    let field = "FF_" + paramQualGestiti[iPar].Tabella_Cod_Des + "_Descrizione";
                    let titolo = paramQualGestiti[iPar].Tabella_Des;

                    campiKendoModel[field] = { type: "string" };
                    colonneKendoGrid.push({ field: field, title: titolo, width: 120, filterable: { multi: true, search: true } });
                }
                if (paramQualGestiti[iPar].Tipo === 3) {

                    let field = "FF_" + paramQualGestiti[iPar].Tabella_Cod_Des + "_Val_Cod";
                    let titolo = paramQualGestiti[iPar].Tabella_Des;

                    campiKendoModel[field] = { type: "number" };
                    colonneKendoGrid.push({ field: field, title: titolo, width: 120 });
                }
                if (paramQualGestiti[iPar].Tipo === 5) {

                    let field = "FF_" + paramQualGestiti[iPar].Tabella_Cod_Des + "_Val_Cod";
                    let titolo = paramQualGestiti[iPar].Tabella_Des;

                    campiKendoModel[field] = { type: "date" };
                    colonneKendoGrid.push({ field: field, title: titolo, width: 100, format: "{0:dd/MM/yyyy}" });
                }
            }
        }
    }

    var parametriPerLettura = null;
    var parametriDataSource = {
        serverFiltering: false,
        aggregate: aggregateColumns
    };
    var parametriKendoGrid = {
        columnMenu: true,
        editable: false,
        groupable: true,
        pdf: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"] },
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        //selectable: true,
        reorderable: true,
        toolbarCommands: ["tmplAlienaGiacenze", "tmplStampaGiacenze"]
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendo_Giacenze_onDataBoundedRighe,
        funzioneDaChiamarePrimaDiExcelExport: onExportExcel
    };
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

    //Aggancio evento cambio pagina
    var grid = KendoGrid(IDControllo);
    grid.bind("page", tabGiacenze_page);

    //Aggancio evento cambio dimensione pagina
    grid.one("dataBound", function (e) {
        var grid = e.sender;
        var pageSizesDdl = $(grid.pager.element).find("[data-role='dropdownlist']").data("kendoDropDownList");
        pageSizesDdl.bind("change", ddlDimensionePaginaGiacenze_change);
    });

    $("#lbl_dataGiacenze").html($(cIdDataOperazione).val());

    carica_tmplStampa("StampaGiacenze")
}

function tabGiacenze_page(e) {
    SalvaParametriPagina(e.page);
};

function ddlDimensionePaginaGiacenze_change(e) {
    SalvaParametriPagina();
};

function onExportExcel(e) {
    var rows = e.workbook.sheets[0].rows;
    for (var ri = 0; ri < rows.length; ri++) {
        var row = rows[ri];
        if (row.type === "group-footer" || row.type === "footer") {
            for (var ci = 0; ci < row.cells.length; ci++) {
                var cell = row.cells[ci];
                if (cell.value) {
                    cell.value = $(cell.value).text();
                    cell.hAlign = "right";
                }
            }
        }
    }
}

function SelezionaGiacenze(e) {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = KendoGrid("tabGiacenze");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    rowKendoGridSelected(row, checked)
}

// click mostra movimenti
function MostraMovimenti(e) {

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    if (dataItem === null || dataItem === undefined) {
        return true;
    }

    $('#tabs a[href="#tabMovimenti"]').tab("show");

    $("#Div_FiltriGiacenze").hide();
    $("#Div_Filtri_Movimenti").show();
    $("#Div_FiltriGiacenze2").hide();
    $("#Div_Filtri_Movimenti2").show();

    $("#CheckBoxCarichi").prop("checked", true);
    $("#CheckBoxScarichi").prop("checked", true);

    var chiave = dataItem.chiave_giacenze;
    var aChiave = chiave.split("_");

    var Sa_Cod = aChiave[1];
    var Fabbricato_Cod = aChiave[2];
    var Elem_Cod = aChiave[3];
    var Pro_Cod = aChiave[4];
    var Mat_Cod = aChiave[5];
    var Lotto = aChiave[6];
    var Cal_Cod = aChiave[7];
    var Cod_Progetto = aChiave[8];
    var Udm_Cod = aChiave[9];

    var parametri = {
        Sa_Cod: Sa_Cod,
        strFabbricato_Cod: Fabbricato_Cod,
        Tipo_Fabbricato: 20,
        Elem_Cod: Elem_Cod,
        NomeProdotto: "",
        Cod_Articolo: "",
        Pro_Cod: Pro_Cod,
        Mat_Cod: Mat_Cod,
        Lotto: dataItem.Lotto_Acc,
        Cal_Cod: Cal_Cod,
        Cod_Progetto: Cod_Progetto,
        Udm_Cod: Udm_Cod,
        DataInizio: "",
        DataFine: "",
        VisualizzaCarichi: $("#CheckBoxCarichi").is(":checked"),
        VisualizzaScarichi: $("#CheckBoxScarichi").is(":checked"),
        CifreArrotondamento: Get_KendoDDLValue("ddlArrotondamento", 2),
        Kendo: true
    };

    //imposto 
    $("#TxtProdotto").val(dataItem.Pro_Des);
    $("#TxtCodArticolo").val(dataItem.Cod_Articolo);
    $(cIdPro_Cod).val(dataItem.Pro_Cod);
    $("#TxtLotto").val(dataItem.Lotto_Acc);
    setKendoSwitch("CheckBoxFiltraPerLotto", true);
    $(cIdDataInizio).val("");
    $(cIdDataFine).val("");

    //salvo
    SalvaParametri();

    popolaMovimenti("tabMovimenti", parametri);
}

function popolaMovimenti(IDControllo, parametri) {


    var visualizzazione_mode = parseInt($(cIdhdVisualizzazioneMode).val());

    var funzioniCRUD = {
        funzioneRead: CercaMovimenti//,
        //checkBoxFunction: SelezionaMovimenti
    };

    if (visualizzazione_mode === 0) {
        funzioniCRUD.checkBoxFunction = SelezionaMovimenti;
    }

    var formatQta = "{0:n" + Get_KendoDDLValue("ddlArrotondamento", 2) + "}";
    var formatQtaSum = "n" + Get_KendoDDLValue("ddlArrotondamento", 2) + "";

    var idModel = "chiave_movimenti";
    var campiKendoModel = {
        Sa_Nome: { type: "string" },
        Fabbricato_Des: { type: "string" },
        Cat_Des: { type: "string" },
        Des_Gruppo_Merce: { type: "string" },
        Pro_Cod: { type: "number" },
        Cod_Articolo: { type: "string" },
        Pro_Des: { type: "string" },
        Lotto_Int: { type: "string" },
        Lotto_Acc: { type: "string" },
        Mov_Det_Extra_Str: { type: "string" },
        Param_Des: { type: "string" },
        Cal_Des: { type: "string" },
        Udm_Des: { type: "string" },
        Qta: { type: "number" },
        Qta_Dest: { type: "number" },
        Prezzo_Unitario: { type: "number" },
        Totale: { type: "number" },
        Data: { type: "date" },
        Doc_Numero: { type: "string" },
        Contatto: { type: "string" },
        Cau_Mov: { type: "string" },
        Dettagli: { type: "string" },
        Info: { type: "string" },
        Modificabile: { type: "number" },
        Eliminabile: { type: "number" }
    };

    var colonneKendoGrid = [];

    if (visualizzazione_mode === 0) {

        colonneKendoGrid.push(
            {
                title: TraduzioneMultiResx(gestioneMagazziniResx, "Operazioni", "Operazioni"),
                command: [{
                    name: "infomov", text: "",
                    iconClass: "fa fa-info",
                    click: InfoMovimento,
                    visible: function (dataItem) {
                        return !operazioniNonEditabili.includes(dataItem.Lav_Cod);
                    }
                }, {
                    name: "editmov", text: "",
                    iconClass: "fa fa-pencil-square-o",
                    click: EditMovimento,
                    visible: function (dataItem) {
                        return dataItem.Modificabile == "1" && !operazioniNonEditabili.includes(dataItem.Lav_Cod);
                    }
                }, {
                    name: "delmov", text: "",
                    iconClass: "fa fa-trash-o",
                    click: DelMovimento,
                    visible: function (dataItem) {
                        return dataItem.Eliminabile == "1" && dataItem.Lav_Cod != "4500" && !operazioniNonEditabili.includes(dataItem.Lav_Cod);
                    }
                }],
                width: 80
            }
        );
    }

    colonneKendoGrid.push(
        { field: "Sa_Nome", title: TraduzioneMultiResx(gestioneMagazziniResx, "Centro", "Centro"), width: 100, filterable: { multi: true, search: true } },
        { field: "Fabbricato_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "Magazzino", "Magazzino"), width: 200, filterable: { multi: true, search: true } },
        { field: "Cat_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "Categoria", "Categoria"), width: 100, filterable: { multi: true, search: true } },
        { field: "Des_Gruppo_Merce", title: "Gruppo Merce", width: 325, filterable: { multi: true, search: true }, hidden: true },
        { field: "Pro_Cod", title: TraduzioneMultiResx(gestioneMagazziniResx, "CodiceProdotto", "Codice Prodotto"), width: 100, filterable: { multi: true, search: true } },
        { field: "Cod_Articolo", title: TraduzioneMultiResx(gestioneMagazziniResx, "CodiceArticoloAbbr", "Cod Articolo"), width: 100, filterable: { multi: true, search: true } },
        { field: "Pro_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "Prodotto", "Prodotto"), width: 200, filterable: { multi: true, search: true } },
        { field: "Udm_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "UnitàDiMisura", "Unità di Misura"), width: 100, filterable: { multi: true, search: true } },
        { field: "Qta", title: TraduzioneMultiResx(gestioneMagazziniResx, "QuantitàAbbr", "Qta"), format: formatQta, width: 100, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, '" + formatQtaSum + "')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, '" + formatQtaSum + "')#</div>" },
        { field: "Data", title: TraduzioneMultiResx(gestioneMagazziniResx, "Data", "Data"), format: "{0:dd/MM/yyyy}", width: 100 },
        { field: "Dettagli", title: TraduzioneMultiResx(gestioneMagazziniResx, "DescrizioneAttività", "Descrizione Attivita"), width: 200, filterable: { multi: true, search: true } },
        { field: "Info", title: TraduzioneMultiResx(gestioneMagazziniResx, "Note", "Note"), width: 200, filterable: { multi: false, search: true }, hidden: true },
        { field: "Doc_Numero", title: TraduzioneMultiResx(gestioneMagazziniResx, "DocNum", "Doc. Num."), width: 100, filterable: { multi: true, search: true } },
        { field: "Contatto", title: TraduzioneMultiResx(gestioneMagazziniResx, "DocForn", "Doc. Forn."), width: 100, filterable: { multi: true, search: true } },
        { field: "Lotto_Int", title: TraduzioneMultiResx(gestioneMagazziniResx, "LottoEsercizio", "Lotto Esercizio"), width: 100, filterable: { multi: true, search: true } },
        { field: "Lotto_Acc", title: TraduzioneMultiResx(gestioneMagazziniResx, "LottoProdotto", "Lotto"), width: 150, filterable: { multi: true, search: true } },
        { field: "Mov_Det_Extra_Str", title: TraduzioneMultiResx(gestioneMagazziniResx, "DescrizioneAddizionale", "Descrizione Addizionale"), width: 140, filterable: { multi: true, search: true } },
        { field: "Param_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "ParametroQualitativo", "Parametro Qualitativo"), width: 100, filterable: { multi: true, search: true } },
        { field: "Cal_Des", title: TraduzioneMultiResx(gestioneMagazziniResx, "Qualità", "Qualità"), width: 100, filterable: { multi: true, search: true } },
        { field: "Prezzo_Unitario", title: TraduzioneMultiResx(gestioneMagazziniResx, "Prezzo", "Prezzo"), format: "{0:n6}", width: 100, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
        { field: "Totale", title: TraduzioneMultiResx(gestioneMagazziniResx, "Totale", "Totale"), format: "{0:n2}", width: 100, attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" }
    );

    //Parametri qualitativi
    if (isModuloFFAttivo === true && isCategoriaFF(null) === true) {
        var paramQualGestiti = RicercaParametriQualitativi(true, $(cIdPiva).val());
        for (let iPar = 0; iPar < paramQualGestiti.length; iPar++) {
            if (paramQualGestiti[iPar].Tabella_ID !== 0) {
                if (paramQualGestiti[iPar].Tipo === 1 || paramQualGestiti[iPar].Tipo === 4) {

                    let field = "FF_" + paramQualGestiti[iPar].Tabella_Cod_Des + "_Descrizione";
                    let titolo = paramQualGestiti[iPar].Tabella_Des;

                    campiKendoModel[field] = { type: "string" };
                    colonneKendoGrid.push({ field: field, title: titolo, width: 120, filterable: { multi: true, search: true } });
                }
                if (paramQualGestiti[iPar].Tipo === 3) {

                    let field = "FF_" + paramQualGestiti[iPar].Tabella_Cod_Des + "_Val_Cod";
                    let titolo = paramQualGestiti[iPar].Tabella_Des;

                    campiKendoModel[field] = { type: "number" };
                    colonneKendoGrid.push({ field: field, title: titolo, width: 120 });
                }
                if (paramQualGestiti[iPar].Tipo === 5) {

                    let field = "FF_" + paramQualGestiti[iPar].Tabella_Cod_Des + "_Val_Cod";
                    let titolo = paramQualGestiti[iPar].Tabella_Des;

                    campiKendoModel[field] = { type: "date" };
                    colonneKendoGrid.push({ field: field, title: titolo, width: 100, format: "{0:dd/MM/yyyy}" });
                }
            }
        }
    }

    var parametriPerLettura = parametri !== null ? [parametri] : null;
    var parametriDataSource = {
        serverFiltering: false,
        aggregate: [
            { field: "Qta", aggregate: "sum" },
            { field: "Qta_Dest", aggregate: "sum" },
            { field: "Prezzo_Unitario", aggregate: "sum" },
            { field: "Totale", aggregate: "sum" }
        ]
    };
    var parametriKendoGrid = {
        columnMenu: true,
        editable: false,
        groupable: true,
        pdf: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"] },
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        //checkSelezioneRiga:false,
        //selectable: true,
        reorderable: true,
        toolbarCommands: ["tmplTrasferimentoDDT", "tmplStampaMovimenti"]
    };

    //if (visualizzazione_mode === 0) {
    //    parametriKendoGrid.checkSelezioneRiga = { filterable: false, field: null, width: "30px" };
    //}

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendo_Movimenti_onDataBoundedRighe,
        funzioneDaChiamarePrimaDiExcelExport: onExportExcel
    };
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

    //Aggancio evento cambio pagina
    var grid = KendoGrid(IDControllo);
    grid.bind("page", tabMovimenti_page);

    //Aggancio evento cambio dimensione pagina
    grid.one("dataBound", function (e) {
        var grid = e.sender;
        var pageSizesDdl = $(grid.pager.element).find("[data-role='dropdownlist']").data("kendoDropDownList");
        pageSizesDdl.bind("change", ddlDimensionePaginaMovimenti_change);
    });

    var dataDa;
    var dataA;

    var DataInizio = $(cIdDataInizio).val();
    var DataFine = $(cIdDataFine).val();

    dataDa = DataInizio;
    dataA = DataFine;

    $("#lbl_dataMovimenti").html(dataDa + " - " + dataA);

    carica_tmplStampa("StampaMovimenti")
}

function carica_tmplStampa(id_controllo) {
    if (!$("#" + id_controllo).data('kendoDropDownButton')) {

        var items = [
            {
                id: "button1",
                text: TraduzioneMultiResx(gestioneMagazziniResx, "SchedaGiacenze", "Scheda Giacenze").toUpperCase(),
                click: function () { Gestione_Operazione_Menu('7p') }
            },
            {
                id: "button2",
                text: TraduzioneMultiResx(gestioneMagazziniResx, "SchedaMovimentiMagazzino", "Scheda dei Movimenti Magazzino").toUpperCase(),
                click: function () { Gestione_Operazione_Menu('7q') }
            },
            {
                id: "button3",
                text: TraduzioneMultiResx(gestioneMagazziniResx, "SchedaFertilizzanti", "Scheda dei Fertilizzanti").toUpperCase(),
                click: function () { Gestione_Operazione_Menu('7r') }
            },
            {
                id: "button4",
                text:  TraduzioneMultiResx(gestioneMagazziniResx, "SchedaProdottiFitosanitari", "Scheda dei Prodotti Fitosanitari").toUpperCase(),
                click: function () { Gestione_Operazione_Menu('7s') }
            },
            {
                id: "button5",
                text:  TraduzioneMultiResx(gestioneMagazziniResx, "RiepilogoProdottiUtilizzati", "Riepilogo Prodotti Utilizzati").toUpperCase(),
                click: function () { Gestione_Operazione_Menu('7t') }
            }
        ]

        if (permesso_Rintraccio_Operazione_Di_Cura) {
            items.push(
                {
                    enable: false
                },
                {
                    id: "button6",
                    text: TraduzioneMultiResx(gestioneMagazziniResx, "EsportazioneRitiriTracciatiOpta", "Esportazione Ritiri Tracciati Opta").toUpperCase(),
                    click: function () { Gestione_Operazione_Menu('1000') }
                },
                {
                    id: "button7",
                    text: TraduzioneMultiResx(gestioneMagazziniResx, "TestRintracciataColloLotto", "Test Rintracciata Collo/Lotto").toUpperCase(),
                    click: function () { Gestione_Operazione_Menu('1001') }
                }
            )
        }

        $("#" + id_controllo).kendoDropDownButton({
            imageUrl: "../AB_Immagini/icone32/Stampa.ico",
            items: items
        })
    }
}

function tabMovimenti_page(e) {
    SalvaParametriPagina(e.page);
};

function ddlDimensionePaginaMovimenti_change(e) {
    SalvaParametriPagina();
}

function CalcolaTotale() {
    var grid = KendoGrid("tabMovimenti");
    var data = grid.dataSource.data();
    var item, sum = 0;
    for (var idx = 0; idx < data.length; idx++) {
        item = data[idx];
        if (listaCauCarico.includes(item.Cau_Mov)) {
            sum += item.Qta_Dest;
        } else {
            sum -= item.Qta_Dest;
        }
    }
    return "<div style='text-align:right'>" + kendo.toString(sum, "n2") + "</div>";
}

function SelezionaMovimenti(e) {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = KendoGrid("tabMovimenti");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    rowKendoGridSelected(row, checked)
}

// click info movimento
function InfoMovimento(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var chiave = dataItem.chiave_movimenti;
    var aChiave = chiave.split("_");

    var data = aChiave[0];
    if (kendo_grid) data = data.replace(/-/g, "/");
    var piva = aChiave[1];
    var id_Agenda = aChiave[2];
    var blocco_flag = aChiave[3];
    var lav_cod = aChiave[4];
    var veg_cod = aChiave[5];
    //var sa_cod = aChiave[6];
    var sa_cod = 0;

    SalvaParametri();

    ApriAgenda(data, piva, id_Agenda, blocco_flag, lav_cod, veg_cod, sa_cod, "0");
}

// click edit movimento
function EditMovimento(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var chiave = dataItem.chiave_movimenti;
    var aChiave = chiave.split("_");

    var data = aChiave[0];
    if (kendo_grid) data = data.replace(/-/g, "/");
    var piva = aChiave[1];
    var id_Agenda = aChiave[2];
    var blocco_flag = aChiave[3];
    var lav_cod = aChiave[4];
    var veg_cod = aChiave[5];
    var sa_cod = aChiave[6];

    sa_cod = 0;

    SalvaParametri();

    ApriAgenda(data, piva, id_Agenda, blocco_flag, lav_cod, veg_cod, sa_cod, "2");
}

// click del movimento
function DelMovimento(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var chiave = dataItem.chiave_movimenti;
    if (kendo_grid) chiave = chiave.replace(/-/g, "/");

    SalvaParametri();

    var kConfirmElim = $("<div></div>").kendoConfirm({
        title: TraduzioneMultiResx(gestioneMagazziniResx, "Attenzione", "Attenzione"),
        content: TraduzioneMultiResx(gestioneMagazziniResx, "ConfermaEliminazioneMovimento", "Sei sicuro di voler eliminare il movimento selezionato?")
    }).data("kendoConfirm");

    kConfirmElim.result.done(function (data) {
        console.log("Conferma eliminazione");
        EliminazioneConfermata(chiave);
    });

    kConfirmElim.result.fail(function () {
        console.log("Annulla eliminazione");
    });

    kConfirmElim.open();

    //ConfermaControlliSiNo(
    //    TraduzioneMultiResx(gestioneMagazziniResx, "ConfermaEliminazioneMovimento", "Sei sicuro di voler eliminare il movimento selezionato?"),
    //    "del_elem|" + chiave
    //);
}

function MessaggioErrore_Kendo(str) {
    //$('#dialog_errore_Kendo').modal('show');
    //$('#messaggioErrore').html(str);

    let dialog = $("#dialogErrorKendo").data("kendoDialog");
    dialog.content(str);
    dialog.open();

    WaitFrame.hide();
}

function MessaggioOK_Kendo(str) {
    //$('#dialog_ok').modal('show');
    //$('#messaggioOk').html(str);

    let dialog = $("#dialogOkKendo").data("kendoDialog");
    dialog.content(str);
    dialog.open();

    WaitFrame.hide();
}


function clickGiacenze0(e) {
    let value = e.checked;
    if ($("#CheckBoxValorizzaProdotto").length > 0) {
        if (value) {
            setKendoSwitch("CheckBoxValorizzaProdotto", false);
            //$("#CheckBoxValorizzaProdotto").prop("checked", false);
        }
        KendoSwitch("CheckBoxValorizzaProdotto").enable(!value);
        //$("#CheckBoxValorizzaProdotto").prop("disabled", value);
    }
}

function clickValorizzaProdotto(e) {
    let value = e.checked;
    if (value) {
        setKendoSwitch("CheckBoxGiacenze0", false);
        //$("#CheckBoxGiacenze0").prop("checked", false);
    }
}

//function cambiaAnnata(dataInput, anniDaAggiungere) {

//    // se anniDaAggiungere è negativo di fatto sottrae anni
//    var separator = "/";
//    var aoDate = dataInput.split(separator);

//    var day = aoDate[0];
//    var month = aoDate[1];
//    var year = aoDate[2];

//    var yearNew = parseInt(year) + parseInt(anniDaAggiungere);
//    var dataNew = [day, month, yearNew].join(separator);

//    return dataNew;
//}

function SpostaAnnata(aggiungiTogliAnno) {

    var dataInizio = KendoDate("txt_DataOperazioneDa").value();
    var dataFine = KendoDate("txt_DataOperazioneA").value();

    //var dataInizio = kendo.parseDate($(cIdDataInizio).val());
    //var dataFine = kendo.parseDate($(cIdDataFine).val());

    if (dataInizio !== undefined && dataInizio !== null) {
        dataInizio.setFullYear(dataInizio.getFullYear() + parseInt(aggiungiTogliAnno));
        KendoDate("txt_DataOperazioneDa").value(dataInizio);
    }

    if (dataFine !== undefined && dataFine !== null) {
        dataFine.setFullYear(dataFine.getFullYear() + parseInt(aggiungiTogliAnno));
        KendoDate("txt_DataOperazioneA").value(dataFine);
    }

}

function btn_ricerca_giacenze_click() {
    SalvaParametri();
    ricerca_giacenze();
}

function ricerca_giacenze() {

    var fabbricatoCod = getFabbricatoSelezionato(true);

    if (fabbricatoCod === undefined || fabbricatoCod === null) {
        alert(TraduzioneMultiResx(gestioneMagazziniResx, "SelezionareUnMagazzinoDisponibile", "Selezionare un magazzino disponibile."));
        return false;
    } else {
        popolaGiacenze("tabGiacenze");
    }

}

function btn_ricerca_movimenti_click() {
    SalvaParametri();
    ricerca_movimenti();
}

function ricerca_movimenti() {

    var fabbricatoCod = getFabbricatoSelezionato(true);

    if (fabbricatoCod === undefined || fabbricatoCod === null) {
        alert(TraduzioneMultiResx(gestioneMagazziniResx, "SelezionareUnMagazzinoDisponibile", "Selezionare un magazzino disponibile."));
        return false;
    } else {
        popolaMovimenti("tabMovimenti", null);
    }

}

function riposizionamento_pagina(tabellaKendo, pagina, dimensionePagina) {

    ricercaSincrona = true;
    tabellaMag = tabellaKendo;
    paginaTabellaMag = pagina;
    dimensionePaginaTabellaMag = dimensionePagina;

    switch (tabellaKendo) {

        case "tabGiacenze":
            ricerca_giacenze();
            break;

        case "tabMovimenti":
            ricerca_movimenti();
            break;

    }

    ricercaSincrona = false;
    tabellaMag = undefined;
    paginaTabellaMag = 0;
    dimensionePaginaTabellaMag = 0;

}

function ddlCentroAziendale_change() {

    elencoCelleMagazzini = null;
    LeggiMagazzini();
    KendoDDL("ddlMagazzini").dataSource.read();

    if (KendoDDL("ddlMagazzini").dataItems().length === 1) { //ho un solo elemento, lo seleziono
        KendoDDL("ddlMagazzini").select(0);
    } else if (KendoDDL("ddlMagazzini").select() === -1) { //check whether any item is selected
        Set_KendoDDLValue("ddlMagazzini", 0);
    }

    //... ricalcolo i magazzini disponibili

    //ajaxAgronicaSync("GestioneMagazziniBS.aspx/Calcola_Magazzini",
    //    "{ valore: '" + $(cIdSa_Cod + " select").val() + "'}",
    //    false,
    //    function(risposta) {
    //        $(cIdFabbricato_Cod + " select").empty();
    //        $(cIdFabbricato_Cod + " select").append(risposta.RispostaStringa);
    //        $(".selectpicker").selectpicker("refresh");
    //    },
    //    null
    //);

}

function RiempiElencoCelleEMagazzini(options) {
    options.success(elencoCelleMagazzini);
}

function RiempiElencoMagazziniPerTrasferimento(options) {
    //TODO: devo togliere dall'elenco lo 0 e il magazzino di origine
    let magazziniRestanti = elencoMagazzini.filter(function (dataItem) {
        return dataItem.Id_Destinazione !== 0 && dataItem.key_Dest !== KendoDDL("ddlMagazzini").value();
    });

    options.success(magazziniRestanti);
}

function getCentro(isKendo) {
    if (isKendo === true) {
        let ddl = KendoDDL("ddlCentroAziendale");
        if (ddl !== undefined && ddl !== null) {
            return ddl.dataItem();
        } else {
            return null;
        }
    } else {
        return $(cIdSa_Cod + " select").val();
    }
}

function getSaCod(isKendo) {
    let centroSel = getCentro(isKendo);

    if (isKendo === true) {
        //TODO: 
        if (centroSel !== null) {
            return sa_cod = centroSel.sa_cod;
        } else {
            return sa_cod = centroSel;
        }
    } else {
        return sa_cod = centroSel;
    }
}

function getFabbricatoSelezionato(isKendo) {
    if (isKendo === true) {
        let ddl = KendoDDL("ddlMagazzini");
        if (ddl !== undefined && ddl !== null) {
            return ddl.dataItem();
        } else {
            return null;
        }
    } else {
        return $(cIdFabbricato_Cod + " select").val();
    }
}

function getFabbricatoCod(isKendo) {
    let fabSel = getFabbricatoSelezionato(isKendo);
    if (isKendo === true) {
        //TODO: 
        if (fabSel !== null) {
            if (fabSel.key_Dest === "")
                return fabbricato_cod = 0;
            else
                return fabbricato_cod = fabSel.key_Dest.split("_")[2]; //fabSel.Id_Destinazione
        } else {
            return fabbricato_cod = fabSel;
        }
    } else {
        if (fabSel) {
            if (fabSel == 0)
                return fabbricato_cod = 0;
            else
                return fabbricato_cod = fabSel.split("|")[1];
        } else {
            return fabbricato_cod = fabSel;
        }
    }
}

function isModuloFF() {
    return moduliAnagrafe.includes(2) || moduliAnagrafe.includes(3) || moduliAnagrafe.includes(5);
}

function isCategoriaFF(elemCod) {
    if (elemCod === null) {
        elemCod = parseInt(Get_KendoDDLValue("ddlCategoria", 0));
    }
    return [0, 201, 210, 310].includes(elemCod);
}

function SalvaParametriDefault() {
    SalvaParametriDiv("#frmInputRicerca", true);
    SalvaParametriUl("#tabs", true, $(cIdPiva).val());
}

function SalvaParametri() {
    SalvaParametriDiv("#frmInputRicerca", false);
    SalvaParametriUl("#tabs", false, $(cIdPiva).val());
}

function SalvaParametriPagina(paginaCorrente) {
    SalvaParametriUl("#tabs", false, $(cIdPiva).val(), paginaCorrente);
};

function SetParametri() {
    SetParametriDiv("#frmInputRicerca", false);
    SetParametriUl("#tabs", false, $(cIdPiva).val());
}

function ClearParametri() {
    ClearParametriDiv('#frmInputRicerca');
    ClearParametriUl('#tabs');
}