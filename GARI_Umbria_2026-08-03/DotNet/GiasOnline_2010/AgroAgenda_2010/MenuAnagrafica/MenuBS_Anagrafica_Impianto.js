var Cmb_Operazioni;
var windowOperazione;
var Operazione_Sel;

function kendoImpianto_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    //mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoImpianto", 1);

}

function kendoImpianto_inizializza(divKendo, keys) {

    var funzioniCRUD = {
        funzioneRead: kReadValorizzazioneImpianto_rows
    };

    if (permesso_impianto_ereditatore) {
        funzioniCRUD.checkBoxFunction = KendoOperazioni_impianti;
    }

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneImpianto_mod();
    var colonneKendoGrid = kReadValorizzazioneImpianto_col();

    var parametriPerLettura = [];
    var parametriDataSource = {
        pagesize: 50,
        aggregate: [{ field: "sup_imp", aggregate: "sum" }]
    };

    var templateCommands = "<div class='btn-group-vertical'>" +
        "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoImpianto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaImpianto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaImpianto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>" +
        "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoImpianto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span>&nbsp;</button>' +
            '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaImpianto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span>&nbsp;</button>' +
            '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaImpianto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span>&nbsp;</button>';
        widthAzioni = "120px";
    }

    var templateAgenda = "<div class='btn btn-info' style='display:block;width:110px;border:0px;margin-bottom:3px;' onclick=operazioniImpianto(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-calendar-o' aria-hidden='true'></i>Operazioni</div>" +
        "<div class='btn btn-info' style='display:block;width:110px;border:0px;' onclick=creaOperazione(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-plus-square-o' aria-hidden='true'></i>Nuova Op</div>";
    var widthAgenda = "140px";

    if (GiasVersioneMaster === "2022") {
        templateAgenda = "<div title='Operazione' class='btn btn-info btn-icon btn-operation' onclick=operazioniImpianto(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-calendar-o' aria-hidden='true'></i></div>" +
            "<div title='Nuova Operazione' class='btn btn-info btn-icon btn-new-operation' onclick=creaOperazione(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-plus-square-o' aria-hidden='true'></i></div>";
    }

    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        excel: true,
        pdf: false,
        search: true,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: true,
        selectable: "row",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: true,
        btnEliminaTuttiFiltri: false,
        toolbarCommands: ["templateKendoImpianti"],
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: templateCommands
                }, title: Traduzione(menuBSAnagraficaResx, "Azioni", "Azioni"), width: widthAzioni
            },
            {
                command: {
                    template: templateAgenda
                }, title: "Agenda", width: widthAgenda
            }
        ]

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            //Se in mobile mostro solo la colonna unica
            //mostraColonnaUnicaSeInMobile(e, 1);
            autoFitSeMobile(e);

            //Accorcio l'altezza delle righe
            riduciAltezzaRighe(e, 1);

            var objParametri_Agenda = JSON.parse(objP_agenda);
            var arrKeys = new Array();
            arrKeys.push(objParametri_Agenda.Piva + '_' + objParametri_Agenda.Sa_Cod + '_' + objParametri_Agenda.Appezza + '_' + objParametri_Agenda.Id_Imp)
            var grid = $("#" + divKendo).data("kendoGrid");
            var data = grid.dataSource.data();
            for (var i = 0; i < arrKeys.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (data[j].chiave == arrKeys[i]) {
                        var rowUid = data[j].uid;
                        var row = grid.table.find("[data-uid=" + rowUid + "]");
                        grid.select(row);
                    }
                }
            }
            Dati_Relativi_Percorso_Selezione2(5);
            nascondiBottoniImpianti();
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            var campo_cod = dataItem.Campo_Cod != null ? dataItem.Campo_Cod : 0;
            ImpostaObjP_Agenda(5, dataItem.chiave + "_0_" + campo_cod, false, function () {
                Dati_Relativi_Percorso_Selezione2(5);
            });
        }
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoOperazioni = creaKendoGrid(divKendo, // rappresenta l'ID del div a cui si associa la griglia
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

    CreaToolBarImpianti();

}

function KendoOperazioni_impianti(e) {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $('#divKendoImpianto').data("kendoGrid");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    dataItem.dirty = true;
    rowKendoGridSelected(row, checked)
}

function nascondiBottoniImpianti() {
    var grid = $("#divKendoImpianto").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        if (model.blk_flag == "-1") {
            $(this).addClass("red");
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

        var oggi = new Date().setHours(0, 0, 0, 0);
        if (model.Validita_Inizio > oggi || model.Validita_Fine < oggi) {
            $(this).addClass("DimGray");
        }

        if (!permesso_impianto_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

    });
}


function kReadValorizzazioneImpianto_rows(options) {

    var data = $('#hdKendoImpianto_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneImpianto_col() {

    //var data = $('#hdKendoImpianto_Valorizzazione').val();
    //jSonParsed_Kendo = JSON.parse(data);
    //return jSonParsed_Kendo.kendo_columns;
    return [
        {
            "field": "Sa_Nome",
            "title": Traduzione(menuBSAnagraficaResx, "Centro", "Centro"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "Campo_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Campo", "Campo"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "app_nome",
            "title": Traduzione(menuBSAnagraficaResx, "NomeAppezzamento", "Nome Appezzamento"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "Codice_Impianto",
            "title": Traduzione(menuBSAnagraficaResx, "CodiceImpianto", "Codice Impianto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "utilizzo",
            "title": Traduzione(menuBSAnagraficaResx, "Utilizzo", "Utilizzo"),
            "filterable": {
                "multi": true,
                "search": true,
                "checkAll": true
            },
            "width": "200px"
        },
        {
            "field": "varieta",
            "title": Traduzione(menuBSAnagraficaResx, "Varietà", "Varietà"),
            "filterable": {
                "multi": true,
                "search": true,
                "checkAll": true
            },
            "width": "200px"
        },
        {
            "field": "gru_des",
            "title": Traduzione(menuBSAnagraficaResx, "GruppoVegetale", "Gruppo Vegetale"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "grfi_des",
            "title": Traduzione(menuBSAnagraficaResx, "Finalità", "Finalità"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
        {
            "field": "grva_des",
            "title": Traduzione(menuBSAnagraficaResx, "TipologiaVarietale", "Tipologia Varietale"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
        {
            "field": "sup_imp",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieAbbr", "Sup.") + " [Ha]",
            "filterable": {
                operators: {
                    number: {
                        eq: Traduzione(menuBSAnagraficaResx, "UgualeA", "Uguale a"),
                        gte: Traduzione(menuBSAnagraficaResx, "MaggioreDi", "Maggiore di"),
                        lte: Traduzione(menuBSAnagraficaResx, "MinoreDi", "Minore di")
                    }
                }
            },
            "format": "{0:n4}",
            "footerTemplate": Traduzione(menuBSAnagraficaResx, "Totale", "Totale") + ": #: kendo.toString(sum, \"n4\") # ",
            "width": "100px"
        },
        {
            "field": "Validita_Inizio",
            "title": Traduzione(menuBSAnagraficaResx, "InizioValidità", "Inizio Validità"),
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #',
            "width": "100px"
        },
        {
            "field": "Validita_Fine",
            "title": Traduzione(menuBSAnagraficaResx, "FineValidità", "Fine Validità"),
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #',
            "width": "100px"
        },
        {
            "field": "tra_fila_m",
            "title": Traduzione(menuBSAnagraficaResx, "TraFila", "Tra Fila"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "70px"
        },
        {
            "field": "su_fila_m",
            "title": Traduzione(menuBSAnagraficaResx, "SuFila", "Su Fila"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "70px"
        },
        {
            "field": "piante_ha",
            "title": Traduzione(menuBSAnagraficaResx, "PianteHa", "Piante/Ha"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px",
            "hidden": true
        },
        {
            "field": "piante_impianto",
            "title": Traduzione(menuBSAnagraficaResx, "NPianteImpianto", "Piante/Impianto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px",
            "hidden": true
        },
        //{
        //    "field": "port_cod",
        //    "title": "port_cod",
        //    "filterable": {
        //        "multi": true,
        //        "search": true
        //    }
        //},
        {
            "field": "port_des",
            "title": Traduzione(menuBSAnagraficaResx, "Portinnesto", "Portinnesto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        //{
        //    "field": "foral_cod",
        //    "title": "foral_cod",
        //    "filterable": {
        //        "multi": true,
        //        "search": true
        //    }
        //},
        {
            "field": "foral_des",
            "title": Traduzione(menuBSAnagraficaResx, "FormaAllevamento", "Forma Allevamento"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        //{
        //    "field": "cop_cod",
        //    "title": "cop_cod",
        //    "filterable": {
        //        "multi": true,
        //        "search": true
        //    }
        //},
        {
            "field": "cop_des",
            "title": Traduzione(menuBSAnagraficaResx, "Copertura", "Copertura"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "setup_cod",
            "title": Traduzione(menuBSAnagraficaResx, "SeminaTrapianto", "Semina/Trapianto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "CoverB",
            "title": "Cover Crops", //i18n
            "filterable": {
                "multi": true,
                "search": true
            },
            template: '#= CoverB ? "Sì" : "No" #', //i18n
            "width": "60px"
        },
        {
            "field": "MonitoratoB",
            "title": Traduzione(menuBSAnagraficaResx, "Monitorato", "Monitorato"),
            "filterable": {
                "multi": true,
                "search": true
            },
            template: '#= MonitoratoB ? "Sì" : "No" #', //i18n
            "width": "60px"
        },
        {
            "field": "lotto",
            "title": "Lotto",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "descrizione",
            "title": Traduzione(menuBSAnagraficaResx, "Descrizione", "Descrizione"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
        //{
        //    "field": "stato_impianto_des",
        //    "title": Traduzione(menuBSAnagraficaResx, "StatoImpianto", "Stato Impianto"),
        //    "filterable": {
        //        "multi": true,
        //        "search": true
        //    },
        //    "width": "100px"
        //},
        {
            "field": "cod_kpin",
            "title": "KPIN",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px",
            "hidden": true
        },
        {
            "field": "cod_block",
            "title": "BLOCK",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px",
            "hidden": true
        },
        {
            "field": "regolamento",
            "title": Traduzione(menuBSAnagraficaResx, "Regolamento", "Regolamento"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "Data_Inizio_Portinnesto",
            "title": "Messa a dimora Portinnesto",
            filterable: {
                ui: "datepicker"
            },
            template: '#= (Data_Inizio_Portinnesto == null || Data_Inizio_Portinnesto == "" || kendo.toString(Data_Inizio_Portinnesto, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Inizio_Portinnesto, "dd/MM/yyyy" ) #',
            "width": "100px"
        },
        {
            "field": "Data_Modifica",
            "title": Traduzione(menuBSAnagraficaResx, "DataModifica", "Data Modifica"),
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "150px"
        },
        {
            "field": "Utente_Modifica",
            "title": Traduzione(menuBSAnagraficaResx, "UtenteModifica", "Utente Modifica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "Data_Creazione",
            "title": Traduzione(menuBSAnagraficaResx, "DataCreazione", "Data Creazione"),
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "150px"
        },
        {
            "field": "Utente_Creazione",
            "title": Traduzione(menuBSAnagraficaResx, "UtenteCreazione", "Utente Creazione"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        }
    ];
}

function kReadValorizzazioneImpianto_mod() {

    var data = $('#hdKendoImpianto_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaImpianto(keys) {

    var parametri = {};

    //$.ajax({
    //    type: 'POST',
    //    url: 'MenuBs_Anagrafica.aspx/CaricaImpianti',
    //    data: parametri,
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        $('#hdKendoImpianto_Valorizzazione').val(r.d);
    //        kendoImpianto_inizializza("divKendoImpianto");
    //    }
    //});

    ajaxAgronica(indirizzohttp + "/CaricaImpianti", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoImpianto_Valorizzazione').val(risposta.RispostaStringa);
        kendoImpianto_inizializza("divKendoImpianto", keys);
    }, null)

}


function infoImpianto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoImpianto',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Impianto_Edit2.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaImpianto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditImpianto',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Impianto_Edit2.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaImpianto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 5;
    var chiave = datiRiga.chiave;
    var streelemento = "";
    if (datiRiga.Sa_Nome !== undefined) {
        streelemento += Traduzione(menuBSAnagraficaResx, "Centro", "Centro") + ": <b>" + datiRiga.Sa_Nome + "</b>   ";
    }
    if (datiRiga.Campo_Des !== undefined) {
        streelemento += Traduzione(menuBSAnagraficaResx, "Campo", "Campo") + ": <b>" + datiRiga.Campo_Des + "</b>   ";
    }
    if (datiRiga.app_nome !== undefined) {
        streelemento += Traduzione(menuBSAnagraficaResx, "Appezzamento", "Appezzamento") + ": <b>" + datiRiga.app_nome + "</b>   ";
    }
    streelemento += Traduzione(menuBSAnagraficaResx, "Impianto", "Impianto") + ": <b>" + datiRiga.utilizzo + "</b>";

    Popup_delete(streelemento, xTipoNodo, chiave);
}

function operazioniImpianto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/OperazioniImpianto',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = r.d;//"../Menu/MenuBS_Agenda_Nuovo.aspx?OperazioniImpianto=1";
        }
    });

}

function CreaToolBarImpianti() {

    var gridTB = $("#divKendoImpianto").find(".k-grid-toolbar");
    //var $menu = $("<ul id='context-menu'></ul>");

    if (permesso_impianto_ereditatore) {
        let titleModMultipla = Traduzione(menuBSAnagraficaResx, 'MenuBS_Anagrafica_modificaMultiplaImpianti', 'Modifica Multipla Impianti');

        if (GiasVersioneMaster === "2022") {
            gridTB.append(' <div id="btn_ModificaMultiplaImpianti" class="k-button k-button-icontext k-grid--button" data-title="' + titleModMultipla + '" title="' + titleModMultipla + '"><i class="k-icon k-i-list-unordered"></i></div> ');
        } else {
            gridTB.append(' <div id="btn_ModificaMultiplaImpianti" class="k-button k-button-icontext"><i class="k-icon k-i-list-unordered"></i> ' + titleModMultipla + '</div> ');
        }
    }

    //gridTB.append($menu);

    $("#btn_ModificaMultiplaImpianti").click(function () {
        ModificaMultiplaImpianti();
    });

}

function ModificaMultiplaImpianti() {

    creaModificaMultipla(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_modificaImpiantiSelezionati", "Modifica impianti selezionati"));

    var grid = $("#divKendoImpianto").data("kendoGrid");
    var data = grid.dataSource.data();
    selected_add = new Array();

    var veg_cod = "";
    var grfi_cod = "";
    var veg_cod_uguale = true;
    var grfi_cod_uguale = true;

    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected && data[i].blk_flag != -1) {
            if (veg_cod == "") {
                veg_cod = data[i].veg_cod;
            }
            if (grfi_cod == "") {
                grfi_cod = data[i].grfi_cod;
            }
            if (veg_cod != data[i].veg_cod) {
                veg_cod_uguale = false;
            }
            if (grfi_cod != data[i].grfi_cod) {
                grfi_cod_uguale = false;
            }
            selected_add.push(data[i]);
        }
    }

    if (selected_add.length > 0) {
        obj_ModificaMultipla = {};
        obj_ModificaMultipla.anagrafica = "2";
        if (veg_cod_uguale) {
            obj_ModificaMultipla.veg_cod = selected_add[0].veg_cod;
            obj_ModificaMultipla.validita_inizio = selected_add[0].Validita_Inizio;
        }
        if (veg_cod_uguale && grfi_cod_uguale) {
            obj_ModificaMultipla.grfi_cod = selected_add[0].grfi_cod;
        }
        win_ModificaMultipla.open();
    } else {
        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_SelezionareImpiantiNonBloccati", "Selezionare almeno un impianto non bloccato."));
    }
}


async function creaOperazione(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    inizializzaWindowOperazioni();
    let ddlOperazioni = await caricaddlOperazioni(datiRiga.cul_cod);
    inizializzaCmb_Operazioni(ddlOperazioni);
    windowOperazione.center();
    windowOperazione.open();
}

function caricaddlOperazioni(cul_cod) {
    return new Promise((resolve, reject) => {
        let storage_key = "RiempiDdlOperazioni_Impianti";
        if (!storageExistItem(storage_key)) {
            var parametri = kendo.stringify({
                objP_server: objP_server,
                objP_utenti: objP_utenti,
                FiltraImpostazioniUtente: true,
                Tipo_GruppoOperazioni: "'C','E'"
            });

            ajaxAgronica(pathCoreWS + "Metaschema/Operazioni.asmx/CaricaComboLavorazioni",
                parametri,
                function (risposta) {
                    let operazioni = JSON.parse(risposta.RispostaStringa);
                    storageSetItem(storage_key, risposta.RispostaStringa);
                    operazioni = filtraOperazioni_Cul_Cod(operazioni, cul_cod);
                    resolve(operazioni);
                }, null, null, false);
        } else {
            let operazioni = JSON.parse(storageGetItem(storage_key))
            operazioni = filtraOperazioni_Cul_Cod(operazioni, cul_cod);
            resolve(operazioni);
        }

    });
}

function filtraOperazioni_Cul_Cod(operazioni, cul_cod) {
    if (cul_cod == 0) {
        toRemove = [107, 122, 150, 116, 118, 121, 110, 79, 109, 13, 113, 1, 119, 126];
        operazioni = operazioni.filter((el) => !toRemove.includes(el.lav_cod));
    }
    return operazioni;
}

function inizializzaCmb_Operazioni(ddlOperazioni) {
    if (Cmb_Operazioni == undefined) {
        Cmb_Operazioni = $("#Cmb_Operazioni").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "lav_des",
            dataValueField: "lav_cod",
            dataSource: {
                data: ddlOperazioni,
                group: { field: "gru_des" }
            },
            open: kendoDropDownAdjustWidth,
            dataBound: async function (e) {
                let op_Pred = await LeggiImpostazioniUtente(856);
                if (op_Pred !== "" && !isNaN(op_Pred)) {
                    this.value(op_Pred);
                }
            },
            change: function (e) {
                //Operazione_Sel = "0";
            }
        }).data("kendoDropDownList");
    } else {
        //Cmb_Operazioni.setDataSource(ddlOperazioni);
        Cmb_Operazioni.refresh();
    }
}

function inizializzaWindowOperazioni() {
    if ($("#windowOperazione").data("kendoWindow") == undefined) {

        var windowOptions = {
            actions: ["Close"],
            draggable: false,
            resizable: false,
            width: "450px",
            title: "Seleziona Operazione"
        };

        windowOperazione = $("#windowOperazione").kendoWindow(windowOptions).data("kendoWindow");
    }
}

function LeggiImpostazioniUtente(impostazione_cod) {
    return new Promise((resolve, reject) => {
        let storage_key = "LeggiImpostazioniUtente_" + username_master + "_" + impostazione_cod;
        if (!storageExistItem(storage_key)) {
            var param = {
                objP_Utenti: objP_utenti,
                impostazione_cod: impostazione_cod
            }

            ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_Impostazioni", JSON.stringify(param),
                function (risposta) {
                    storageSetItem(storage_key, risposta.RispostaStringa);
                    resolve(risposta.RispostaStringa);
                }, null, null, false);
        } else {
            resolve(storageGetItem(storage_key));
        }

    });
}

function CreaOp() {
    let Lav_Cod = 0
    let Gru_Cod = 0;
    if (Cmb_Operazioni !== undefined) {
        let item = Cmb_Operazioni.dataItem(Cmb_Operazioni.selectedIndex);
        Lav_Cod = item.lav_cod;
        Gru_Cod = item.gru_cod;
    }

    var parametri = kendo.stringify({
        lav_cod: Lav_Cod,
        gru_cod: Gru_Cod
    });

    ajaxAgronica("MenuBS_Anagrafica.aspx/CreaOperazione",
        parametri,
        function (risposta) {
            window.location = risposta.RispostaStringa;
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        }, null, false);
}