function kendoStalla_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    //mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoStalle", 1);

}

function kendoStalla_inizializza(divKendo, keys) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneStalla_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneStalla_mod();
    var colonneKendoGrid = kReadValorizzazioneStalla_col();

    var templateCommands = "<div class='btn-group-vertical'>" +
        "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoFabbricato(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaFabbricato(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaFabbricato(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>" +
        "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" data-title="' + Traduzione(menuBSAnagraficaResx, "Info", "Info") + '" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoFabbricato(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" data-title="' + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + '" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaFabbricato(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" data-title="' + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + '" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaFabbricato(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
        widthAzioni = "140px";
    }

    var colonneCustomKendoGrid = [
        {
            command: {
                template: templateCommands
            }, title: Traduzione(menuBSAnagraficaResx, "Azioni", "Azioni"), width: widthAzioni
        }
    ];

    if (permesso_bdn_write) {
        var templateBdn = "<div class='btn btn-info' title='" + Traduzione(menuBSAnagraficaResx, "SincronizzaBDN", "Sincronizza BDN") + "' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=sincronizzaBDNClick(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-refresh' aria-hidden='true'></i></i>BDN</div>" +
            "<div class='btn btn-info' title='" + Traduzione(menuBSAnagraficaResx, "ImportCapiMod4", "Import. capi da Mod.4") + "' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=importazioneBDNModello4Click(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-sync' aria-hidden='true'></i></i>BDN</div>" +
            "<div class='btn btn-info' title='" + Traduzione(menuBSAnagraficaResx, "SincronizzaVETINFO", "Sincronizza VETINFO") + "' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=sincronizzaVetInfoClick(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-refresh' aria-hidden='true'></i></i>VETINFO</div>" +
            "<div class='btn btn-info' title='" + Traduzione(menuBSAnagraficaResx, "InviaTrattamentiVetInfo", "Invia Trattamenti VETINFO") + "' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=inviaTrattVetInfoClick(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-refresh' aria-hidden='true'></i></i>VETINFO</div>" +
            "<div class='btn btn-info' title='" + Traduzione(menuBSAnagraficaResx, "GiacenzeGiasVetInfo", "Giacenze Gias VETINFO") + "' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=giacenzeGiasVetInfoClick(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-refresh' aria-hidden='true'></i></i>VETINFO</div>" +
            "<div class='btn btn-info' title='" + Traduzione(menuBSAnagraficaResx, "IngressiBDN", "Ingressi BDN") + "' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=carichiBDNClick(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-up' aria-hidden='true'></i></i>" + Traduzione(menuBSAnagraficaResx, "IngressiBDN", "Ingressi BDN") + "</div>" +
            "<div class='btn btn-info' title='" + Traduzione(menuBSAnagraficaResx, "UsciteBDN", "Uscite BDN") + "' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=scarichiBDNClick(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-down' aria-hidden='true'></i></i>" + Traduzione(menuBSAnagraficaResx, "UsciteBDN", "Uscite BDN") + "</div>";
            //"<div class='btn btn-danger' title='Cancella Ingressi BDN' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=carichiCancBDNClick(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-up' aria-hidden='true'></i></i>Cancella Ingressi BDN</div>" +
            //"<div class='btn btn-danger' title='Cancella Uscite BDN' style='display:block;width:190px;border:0px;margin-bottom:3px;' onclick=scarichiCancBDNClick(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-down' aria-hidden='true'></i></i>Cancella Uscite BDN</div>";
        var widthBdn = "250px";

        if (GiasVersioneMaster === "2022") {
            templateBdn = '<button type="button" class="btn-Bdn k-grid-Bdn k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="' + Traduzione(menuBSAnagraficaResx, 'SincronizzaBDN', 'Sincronizza BDN') + '" onclick=sincronizzaBDNClick(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                '<button type="button" class="btn-Bdn k-grid-BdnModello4 k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="' + Traduzione(menuBSAnagraficaResx, 'ImportCapiMod4', 'Import. capi da Mod.4') + '" onclick=importazioneBDNModello4Click(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                '<button type="button" class="btn-Bdn k-grid-VetInfo k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="' + Traduzione(menuBSAnagraficaResx, 'SincronizzaVETINFO', 'Sincronizza VETINFO') + '" onclick=sincronizzaVetInfoClick(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                '<button type="button" class="btn-Bdn k-grid-VetInfoSend k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="' + Traduzione(menuBSAnagraficaResx, 'InviaTrattamentiVetInfo', 'Invia Trattamenti VETINFO') + '" onclick=inviaTrattVetInfoClick(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                '<button type="button" class="btn-Bdn k-grid-VetInfo k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="' + Traduzione(menuBSAnagraficaResx, 'GiacenzeGiasVetInfo', 'Giacenze GIAS-VETINFO') + '" onclick=giacenzeGiasVetInfoClick(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                '<button type="button" class="btn-Bdn k-grid-IngressiBdn k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="' + Traduzione(menuBSAnagraficaResx, 'IngressiBDN', 'Ingressi BDN') + '" onclick=carichiBDNClick(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                '<button type="button" class="btn-Bdn k-grid-UsciteBdn k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="' + Traduzione(menuBSAnagraficaResx, 'UsciteBDN', 'Uscite BDN') + '" onclick=scarichiBDNClick(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
                //'<button type="button" class="btn-Bdn k-grid-CancIngressiBdn k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="Cancella Ingressi BDN" onclick=carichiCancBDNClick(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                //'<button type="button" class="btn-Bdn k-grid-CancUsciteBdn k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" data-title="Cancella Uscite BDN" onclick=scarichiCancBDNClick(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
            widthBdn = "250px";
        }

        colonneCustomKendoGrid.push({
            command: {
                template: templateBdn
            }, title: "BDN-VETINFO", width: widthBdn
        })
    }

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
        scrollable: true,
        selectable: "row",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: true,
        btnEliminaTuttiFiltri: false,
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: colonneCustomKendoGrid

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            //Se in mobile mostro solo la colonna unica
            //mostraColonnaUnicaSeInMobile(e, 1);
            autoFitSeMobile(e);

            //Accorcio l'altezza delle righe
            riduciAltezzaRighe(e, 1);

            var arrKeys = new Array();
            var objParametri_Agenda = JSON.parse(objP_agenda);
            arrKeys.push(objParametri_Agenda.Piva + '_' + objParametri_Agenda.Sa_Cod + '_' + objParametri_Agenda.Fabbricato)
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
            Dati_Relativi_Percorso_Selezione2(7);
            //NascondiBottoni(grid, e);
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            ImpostaObjP_Agenda(7, dataItem.chiave, false, function () {
                Dati_Relativi_Percorso_Selezione2(7);
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

}

function kReadValorizzazioneStalla_rows(options) {

    var data = $('#hdKendoStalle_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneStalla_col() {

    var data = $('#hdKendoStalle_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadValorizzazioneStalla_mod() {

    var data = $('#hdKendoStalle_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaStalla(keys) {

    var parametri = {};

    ajaxAgronica(indirizzohttp + "/CaricaStalle", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoStalle_Valorizzazione').val(risposta.RispostaStringa);
        kendoStalla_inizializza("divKendoStalle", keys);
    }, null)

}

function infoStalla(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoStalla',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Stalla_Edit.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaStalla(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditStalla',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Stalla_edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaStalla(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 22;
    var chiave = datiRiga.chiave;

    var streelemento = Traduzione(menuBSAnagraficaResx, "Stalla", "Stalla") + ": <b>" + datiRiga.chiave + "</b>";

    Popup_delete(streelemento, xTipoNodo, chiave);
}

function consistenzeZoo(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/consistenzeZoo',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Zoo/Zoo_Animali_Edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function movimentiZoo(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/movimentazioneZoo',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Zoo/Zoo_Carico.aspx";
        }
    });
}

function sincronizzaBDNClick(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave
    }
    ajaxAgronica(indirizzohttp + "/getURLSincronizzaBDN", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null)
}

function importazioneBDNModello4Click(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave
    }
    ajaxAgronica(indirizzohttp + "/getURLImportazioneModello4", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null)
}

function sincronizzaVetInfoClick(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave
    }
    ajaxAgronica(indirizzohttp + "/getURLSincronizzaVetInfo", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null)
}

function inviaTrattVetInfoClick(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave
    };
    ajaxAgronica(indirizzohttp + "/getURLInviaTrattamentiVetInfo", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null);
}

function giacenzeGiasVetInfoClick(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave
    };
    ajaxAgronica(indirizzohttp + "/getURLGiacenzeGiasVetInfo", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null);
}

function carichiBDNClick(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave,
        del: false
    }
    ajaxAgronica(indirizzohttp + "/getURLGestioneCarichiBDN", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null)
}

function scarichiBDNClick(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave,
        del: false
    }
    ajaxAgronica(indirizzohttp + "/getURLGestioneScarichiBDN", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null)
}

function carichiCancBDNClick(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave,
        del: true
    }
    ajaxAgronica(indirizzohttp + "/getURLGestioneCarichiBDN", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null)
}

function scarichiCancBDNClick(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var chiave = datiRiga.chiave;
    var parametri = {
        chiave: chiave,
        del: true
    }
    ajaxAgronica(indirizzohttp + "/getURLGestioneScarichiBDN", JSON.stringify(parametri), function (risposta) {
        window.location = risposta.RispostaStringa;
    }, null)
}