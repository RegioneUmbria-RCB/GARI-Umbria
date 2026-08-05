function kendoEsercizio_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    //mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoEsercizio", 1);

}

function kendoEsercizio_inizializza(divKendo, keys) {

    var funzioniCRUD = {
        funzioneRead: kReadValorizzazioneEsercizio_rows
    };

    if (permesso_impianto_ereditatore) {
        funzioniCRUD.checkBoxFunction = KendoOperazioni_esercizi;
    }

    var idModel = "chiave";
    var campiKendoModel = kReadValorizzazioneEsercizio_mod();
    var colonneKendoGrid = kReadValorizzazioneEsercizio_col();

    var parametriPerLettura = [];
    var parametriDataSource = {
        pagesize: 50
        // aggregate: [{ field: "sup_imp", aggregate: "sum" }]
    };

    var templateCommands = "<div class='btn-group-vertical'>" +
        "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoEsercizio(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaEsercizio(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
        "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoEsercizio(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span>&nbsp;</button>' +
            '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaEsercizio(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span>&nbsp;</button>';
        //widthAzioni = "140px";
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
        //toolbarCommands: ["templateKendoEsercizi"],
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: templateCommands
                }, title: Traduzione(menuBSAnagraficaResx, "Azioni", "Azioni"), width: widthAzioni
            }
        ]

    };
    // x gestione esercizi
    if (permesso_impianto_write && (gestione_esercizi != "" || gestione_esercizi != "0")) {
        parametriKendoGrid.toolbarCommands = ["templateKendoEsercizi"];
    }
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
            nascondiBottoniEsercizi();
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            var campo_cod = dataItem.Campo_Cod != null ? dataItem.Campo_Cod : 0;
            ImpostaObjP_Agenda(5, dataItem.chiave + "_" + campo_cod, false, function () {
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

    CreaToolBarEsercizi();

}

function KendoOperazioni_esercizi(e) {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $('#divKendoEsercizio').data("kendoGrid");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    dataItem.dirty = true;
    rowKendoGridSelected(row, checked)
}

function infoEsercizio(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoImpianto' + riportaParametroVisibilita(),
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Impianto_Edit2.aspx";
        }
    });

}

function modificaEsercizio(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditImpianto' + riportaParametroVisibilita(),
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Impianto_Edit2.aspx";
        }
    });
}

function nascondiBottoniEsercizi() {
    var grid = $("#divKendoEsercizio").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {
        var model = grid.dataItem(this);

        if (!permesso_impianto_write || model.blk_flag == "-1") {
            if (model.blk_flag == "-1") $(this).addClass("red");
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
        }
        var oggi = new Date().setHours(0, 0, 0, 0);
        if (model.Validita_Inizio > oggi || model.Validita_Fine < oggi) {
            $(this).addClass("DimGray");
        }
    });
}

function kReadValorizzazioneEsercizio_mod() {
    var data = $('#hdKendoEsercizio_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaEsercizio(keys) {

    var parametri = {};

    ajaxAgronica(indirizzohttp + "/CaricaEsercizi", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoEsercizio_Valorizzazione').val(risposta.RispostaStringa);
        kendoEsercizio_inizializza("divKendoEsercizio", keys);
    }, null);

}

function kReadValorizzazioneEsercizio_rows(options) {
    var data = $('#hdKendoEsercizio_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneEsercizio_col() {

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
                "search": true
            },
            "width": "200px"
        },
        {
            "field": "cul_des",
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
            "field": "sup_imp",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieAbbr", "Sup.") + " [Ha] " + Traduzione(menuBSAnagraficaResx, "Impianto", "Impianto"),
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
            // "footerTemplate": "Totale: #: kendo.toString(sum, \"n4\") # ",
            "width": "100px"
        },
        {
            "field": "Progetto_Nome",
            "title": Traduzione(menuBSAnagraficaResx, "Lotto","Lotto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "Progetto_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Descrizione", "Descrizione"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
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
            "field": "Resa",
            "title": "Resa [Kg/h]",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        /* {
            "field": "Sup_Prog",
            "title": "Sup. [Ha] esercizio",
            "filterable": {
                operators: {
                    number: {
                        eq: "Uguale a",
                        gte: "Maggiore di",
                        lte: "Minore di"
                    }
                }
            },
            "hidden": true,
            "format": "{0:n4}",
            "width": "100px"
        }, */
        /*{
            "field": "stato_impianto",
            "title": "Stato Impianto",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },*/
        {
            "field": "Distinta_Chiusa",
            "title": Traduzione(menuBSAnagraficaResx, "Chiuso", "Chiuso"),
            "filterable": {
                "multi": true,
                "search": true
            },
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
        { //Anna 21/04/22 - Aggiunte colonne Data_Fioritura, Data_Raccolta, Data_Semina alla griglia esercizi
            "field": "Data_Fioritura_Prevista",
            "title": "Data Fioritura Prevista",
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Data_Fioritura_Prevista, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Data_Fioritura_Prevista, "dd/MM/yyyy" ) #',
            "width": "100px"
        },
        {
            "field": "Data_Raccolta_Prevista",
            "title": "Data Raccolta Prevista",
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Data_Raccolta_Prevista, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Data_Raccolta_Prevista, "dd/MM/yyyy" ) #',
            "width": "100px"
        },
        {
            "field": "Data_Semina_Prevista",
            "title": "Data Semina/Trapianto Prevista",
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Data_Semina_Prevista, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Semina_Prevista, "dd/MM/yyyy" ) #',
            "width": "100px"
        },
        {
            "field": "FlagSecondoRaccolto",
            "title": Traduzione(menuBSAnagraficaResx, "SecondoRaccolto", "Secondo Raccolto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
    ];
}


function CreaToolBarEsercizi() {

    var gridTB = $("#divKendoEsercizio").find(".k-grid-toolbar");
    //var $menu = $("<ul id='context-menu'></ul>");

    if (permesso_impianto_ereditatore) {
        let titleModMultipla = Traduzione(menuBSAnagraficaResx, 'ModificaMultiplaEsercizi', 'Modifica Multipla Esercizi');

        if (GiasVersioneMaster === "2022") {
            gridTB.append(' <div id="btn_ModificaMultiplaEsercizi" class="k-button k-button-icontext k-grid--button" data-title="' + titleModMultipla + '" title="' + titleModMultipla + '"><i class="k-icon k-i-list-unordered"></i></div> ');
        } else {
            gridTB.append(' <div id="btn_ModificaMultiplaEsercizi" class="k-button k-button-icontext"><i class="k-icon k-i-list-unordered"></i> ' + titleModMultipla + '</div> ');
        }
    }

    //gridTB.append($menu);

    $("#btn_ModificaMultiplaEsercizi").click(function () {
        ModificaMultiplaEsercizi();
    });

}

function ModificaMultiplaEsercizi() {

    creaModificaMultipla(Traduzione(menuBSAnagraficaResx, 'ModificaEserciziSelezionati', 'Modifica esercizi selezionati'));

    var grid = $("#divKendoEsercizio").data("kendoGrid");
    var data = grid.dataSource.data();
    selected_add = new Array();

    var veg_cod = "";
    var grfi_cod = "";
    var veg_cod_uguale = true;
    var grfi_cod_uguale = true;

    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected && data[i].blk_flag != -1 && data[i].Distinta_Chiusa != "SI") {
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
        obj_ModificaMultipla.anagrafica = "3";
        if (veg_cod_uguale) {
            obj_ModificaMultipla.veg_cod = selected_add[0].veg_cod;
            obj_ModificaMultipla.validita_inizio = selected_add[0].Validita_Inizio;
        }
        if (veg_cod_uguale && grfi_cod_uguale) {
            obj_ModificaMultipla.grfi_cod = selected_add[0].grfi_cod;
        }
        win_ModificaMultipla.open();
    } else {
        kendo.alert(Traduzione(menuBSAnagraficaResx, "SelezionareAlmenoDueEserciziSbloccati", "Selezionare almeno un esercizio che non sia bloccato o chiuso."));
    }
}
