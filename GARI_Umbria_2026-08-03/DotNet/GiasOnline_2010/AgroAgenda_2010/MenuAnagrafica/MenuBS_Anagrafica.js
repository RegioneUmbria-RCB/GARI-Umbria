var tipoNuovo;
var ddlCentri;
var ddlCentriCampi;
var cmbMeteoDssTipoSorgente;
var cmbMeteoDssOrigineDati;
var kendoDropDownTreeNuovoMeteoDssModelliPrevisionali;
var ddlCampi;
var ddlAppezzamenti;
var ddlFabbricati;
var ddlCategorie_Prodotti;
var elencoCategorieMagazzino = null;
var sa_codSelezionato;

var indirizzohttp = "./MenuBS_Anagrafica.aspx";
var ultimaSelezioneAlbero = '';
var chiave_selezionata = '';
var stringa_id = "";

let isResizing = false;
let lastDownY = 0;

let treePanelWidth;
let dragLeftProp;
let slideInHandleLeftProp;

let container;
let treePanel;
let dragPanel;
let slideInHandle;

function resetResizeSettings() {
    treePanel.css('width', "");
    dragPanel.css('left', "");
    slideInHandle.css('left', "");
}

function storeTreeResizeParams() {
    treePanelWidth = treePanel.css('width');
    dragLeftProp = dragPanel.css('left');
    slideInHandleLeftProp = slideInHandle.css('left');
}

$(function () {
    container = $('#container');
    treePanel = $('#slide-in-share > #container > #top-panel > .k-window');
    dragPanel = $('#drag');
    slideInHandle = $('#slide-in-handle');

    dragPanel.on('mousedown', function (e) {
        e.preventDefault();
        isResizing = true;
        lastDownY = e.clientY;
    });



    $(document).on('mousemove', function (e) {
        // we don't want to do anything if we aren't resizing.
        if (!isResizing)
            return;
        let winWidth = $(window).width();
        if (e.pageX >= 400 && e.pageX <= winWidth - 70) {
            treePanel.css('width', e.pageX + 2);
            dragPanel.css('left', e.pageX + 2);
            slideInHandle.css('left', e.pageX + 2);
        }
    }).on('mouseup', function (e) {
        // stop resizing
        isResizing = false;
    });

    // Dialog Test
    /*$(document).ready(function () {
        $('.xonne-dialog-open').on("click", function () {
            let dialogType = $(this).data("type");
            creaKendoDialog('dialog', dialogType, 'ciao', 'bau');
        });
    });*/

    // Notification Test
    /*$(document).ready(function () {
        creaKendoNotification('xonne-notification');

        $('.xonne-notification-open').on("click", function () {
            let notificationType = $(this).data("type");
            var content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. ";
            $("#xonne-notification").getKendoNotification().show({
                title: notificationType,
                message: content
            }, notificationType);
        });
    });*/
});


//enums

var Enum_ComportamentoPaginaAnagrafica = {
    MeteoDSS: { value: 1, name: "MeteoDSS", code: 1 }
};

//fine enums

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

    //$(nomeDiv + ' tr td' + strColonneDaSaltare).each(function () {
    //    $(this).html('<div class="toggleEspandi" onclick="' + '$(this).parent().parent().find(' + "'td" + strColonneDaSaltare + " div'" + ').toggleClass(' + "'toggleEspandi'" + ');\">' + $(this).html() + '</div>');
    //});
}

function Popup_delete(streelemento, xTipoNodo, xChiave) {

    bootbox.dialog({
        message: streelemento,
        title: Traduzione(menuBSAnagraficaResx, "SeiSicuroDiEliminareQuestoElemento", "Sei sicuro di voler eliminare questo elemento?"),
        buttons: {
            annulla: {
                label: Traduzione(menuBSAnagraficaResx, "Annulla", "Annulla"),
                className: "btn-primary",
                callback: function () {
                }
            },
            Ok: {
                label: Traduzione(menuBSAnagraficaResx, "SiSonoSicuro", "Si sono sicuro"),
                className: "btn-danger",
                callback: function () {
                    WaitFrame.show();
                    $.ajax({
                        type: 'POST',
                        url: './MenuBs_Anagrafica.aspx/DeleteElemento',
                        data: "{xTipoNodo: " + xTipoNodo + ", xChiave:'" + xChiave + "'}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        //dataType: 'json', async: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            WaitFrame.hide();
                            if (r.d.RispostaOK) {
                                kendo.alert(r.d.RispostaStringa);
                                if (xTipoNodo == 1) location.reload();
                                else AggiornaDati();
                            }
                            else {
                                kendo.alert(r.d.Errore);
                            }
                        }
                    });
                }
            }
        }
    });
}



function ImpostaSelezioneVisibilita(qsGruppoEdit) {

    switch (parseInt(qsGruppoEdit)) {
        case Enum_ComportamentoPaginaAnagrafica.MeteoDSS.value:
            NascondiTuttiTab();

            // var tabStrip = $("#tabstrip").kendoTabStrip().data("kendoTabStrip");

            $(tabStrip.items()[13]).show();
            let tab = ImpostaSelezione(13, 1);
            tabStrip.select(tab);
            break;

        default:

    }

}

function NascondiTuttiTab() {

    //var tabStrip = $("#tabstrip").kendoTabStrip().data("kendoTabStrip");
    for (var i = 0; i < tabStrip.items().length; i++) {
        $(tabStrip.items()[i]).hide();
    }


}


function ImpostaSelezione(v, pulisci, keys) {
    if (pulisci == 0) {
        ultimaSelezioneAlbero = 0;
        $('.jstree-clicked').removeClass('jstree-clicked');
    }

    // svuoto il percorso label
    $('#menu_label_navigazione div').empty();


    $('#ElementoSelezionato').val(v);

    return AggiornaDati(keys);
}

function AggiornaDati(keys) {
    //var tabStrip = $("#tabstrip").kendoTabStrip().data("kendoTabStrip");
    //var tabStrip = $("#tabstrip").kendoTabStrip();

    var elemento = $('#ElementoSelezionato').val();
    $.cookie("MenuBS_Anagrafica.tabDati" + getParametroVisibilita(), elemento);
    var tipo = 1;
     if (elemento == "1") {

         if (!$(tabStrip.items()[0]).is(":visible") && !permesso_impresa_read) {
            //ImpostaSelezione(1, 1);
            kendo.alert(Traduzione(menuBSAnagraficaResx, "MancanzaPermessoVisibilitàAnagraficaAziendale", "L'utente non ha il permesso per la visibilità dell'anagrafica aziendale."));
            return true;
        }

        caricaGrigliaAzienda();
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        //tabStrip.select(0);
        return 0;
    }
    if (elemento == "2") {
        if (!$(tabStrip.items()[1]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaCentro(keys);
        //tabStrip.select(1);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 1;
    }
    if (elemento == "3") {

        if (!$(tabStrip.items()[3]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaCampo(keys);
        //tabStrip.select(3);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 3;
    }
    if (elemento == "4") {

        if (!$(tabStrip.items()[4]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaAppezzamento(keys);
        //tabStrip.select(4);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 4;
    }
    if (elemento == "5") {

        if (!$(tabStrip.items()[5]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaImpianto(keys);
        //tabStrip.select(5);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 5;
    }
    if (elemento == "6") {

        if (!$(tabStrip.items()[2]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaCatasto(keys);
        //tabStrip.select(2);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 2;
    }
    if (elemento == "7") {

        if (!$(tabStrip.items()[11]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaRaggruppamentoStalla(keys);
        //tabStrip.select(11);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 11;
    }
    if (elemento == "8") {

        if (!$(tabStrip.items()[10]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaStalla(keys);
        //tabStrip.select(10);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 10;
    }
    if (elemento == "9") {

        if (!$(tabStrip.items()[12]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaZoo(keys);
        //tabStrip.select(12);
        return 12;
    }
    if (elemento == "10") {

        if (!$(tabStrip.items()[7]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaFabbricato(keys);
        //tabStrip.select(7);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 7;
    }
    if (elemento == "11") {

        if (!$(tabStrip.items()[8]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaContatto(keys);
        //tabStrip.select(8);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 8;
    }
    if (elemento == "12") {

        if (!$(tabStrip.items()[9]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaMacchina(keys);
        //tabStrip.select(9);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 9;
    }
    if (elemento == "13") {

        if (!$(tabStrip.items()[13]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaStazioniMeteo(keys);
        //tabStrip.select(13);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 13;
    }
    if (elemento == "14") {

        if (!$(tabStrip.items()[6]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        caricaGrigliaEsercizio(keys);
        //tabStrip.select(6);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 6;
    }
    if (elemento == "15") {

        if (!$(tabStrip.items()[14]).is(":visible")) {
            tabStrip.select(0);
            //ImpostaSelezione(1, 1);
            return true;
        }

        Prodotto_Edit_UC_DocReady_Lista_Anagrafica(keys);
        //caricaGrigliaProdotto(keys);
        //tabStrip.select(14);
        //chiave_selezionata = '';
        //SelezionaRigaDaAlbero();
        return 14;
    }

    WaitFrame.show();
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/AggiornaDati',
        data: "{elemento: " + elemento + ", tipo: " + tipo + "}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {



            var dd = jQuery.parseJSON(r.d);
            console.log(dd);

            // @Paolo
            // Se è stata selezionata una riga dell'albero e la chiave è stata memorizzata
            // ---> prendo la riga della watable selezionata e la posiziono in 1° posizione 

            if (chiave_selezionata != "") {
                var i = 0;
                $(dd.rows).each(function () {
                    if ($(this).attr('tool') == chiave_selezionata) {
                        dd.rows.unshift(dd.rows[i]);
                        dd.rows.splice(i + 1, 1);
                    }
                    i++;
                });

            }


            // Setto la Piva del'azienda per la creazione di nuove strutture
            if (dd.rows != "" && dd.rows != undefined) {
                var Piva = dd.rows[0].tool;
                $('#txt_nuovoPiva').val(Piva.split('_')[0]);
            }

            chiave_selezionata = '';
            SelezionaRigaDaAlbero();
            WaitFrame.hide();
        }
    });
}
function onSelectKendoTab(e) {
    var text = $(e.item).find("> .k-link").text();

    //tabStrip.select(4);
    //$("#tabstrip").data('kendoTabStrip').select(e.item);
    let selectedIndex = $(e.item).index();


    SelectKendoTab(selectedIndex);
    //tabStrip.select(e.item);
}

function SelectKendoTab(elementIndex) {
    //var tabStrip = $("#tabstrip").kendoTabStrip().data("kendoTabStrip");

    //var elemento = $('#ElementoSelezionato').val();
    //var selectedTabElem = $("#tabstrip").data('kendoTabStrip').select();
    //var currentSelectedTabText = $(selectedTabElem).children(".k-link").text();

    let elemento;
    if (elementIndex == null)
        elemento = tabStrip.select().index();
    else
        elemento = elementIndex;
    elemento = elemento + 1;

    switch (elemento) {
        case 1:
            ImpostaSelezione(1, 1);
            break;
        case 2:
            ImpostaSelezione(2, 1);
            break;
        case 3:
            ImpostaSelezione(6, 1);
            break;
        case 6:
            ImpostaSelezione(5, 1);
            break;
        case 4:
            ImpostaSelezione(3, 1);
            break;
        case 5:
            ImpostaSelezione(4, 1);
            break;
        case 8:
            ImpostaSelezione(10, 1);
            break;
        case 11:
            ImpostaSelezione(8, 1);
            break;
        case 12:
            ImpostaSelezione(7, 1);
            break;
        case 13:
            ImpostaSelezione(9, 1);
            break;
        case 9:
            ImpostaSelezione(11, 1);
            break;
        case 10:
            ImpostaSelezione(12, 1);
            break;
        case 14:
            ImpostaSelezione(13, 1);
            break;
        case 7:
            ImpostaSelezione(14, 1);
            break;
        case 15:
            ImpostaSelezione(15, 1);
            break;

    }
}

function Dati_Relativi_Percorso_Selezione2(tipo) {
    var qsVisibilita = Request_QueryString("visibilita");

    if (qsVisibilita == null) {
        qsVisibilita = "0";
    }

    var parametri = {
        tipo: tipo,
        visibilita: qsVisibilita
    };

    ajaxAgronica(indirizzohttp + "/Dati_Relativi_Percorso_Selezione3", JSON.stringify(parametri), function (risposta) {
        var dd = JSON.parse(risposta.RispostaStringa);
        $('#menu_label_navigazione ol').empty();

        for (var i = 0; i < dd.length; i++) {
            $('#menu_label_navigazione ol').append(dd[i]);
        }
    }, null, null, false);
}

function SelezionaRigaGriglia(searchedId, grid, idField) {

    var dataSource = grid.dataSource;
    var filters = dataSource.filter() || {};
    var sort = dataSource.sort() || {};
    var models = dataSource.data();
    var query = new kendo.data.Query(models);
    var rowNum = 0;
    var modelToSelect = null;

    models = query.filter(filters).sort(sort).data;
    for (var i = 0; i < models.length; ++i) {
        var model = models[i];
        if (model[idField] == searchedId) {
            modelToSelect = model;
            rowNum = i;
            break;
        }
    }

    if (modelToSelect != null) {
        var currentPageSize = grid.dataSource.pageSize();
        var pageWithRow = parseInt((rowNum / currentPageSize)) + 1;
        grid.dataSource.page(pageWithRow);
        var row = grid.element.find("tr[data-uid='" + modelToSelect.uid + "']");
        if (row.length > 0) {
            grid.select(row);
            grid.content.scrollTop(grid.select().position().top);
        }
    }

}

function SelezionaAlberoAnagrafica(id) {
    let treeview = AlberoAnagrafica();
    if (treeview != undefined && treeview.dataSource.data().length > 0) {
        // per impianti / esercizi verifico l'id presente nell'albero
        if (id.split("§")[0] == "7") {
            if (treeview.dataSource.get("6" + id.substr(1)) != undefined) {
                id = "6" + id.substr(1);
            } else if (treeview.dataSource.get("7" + id.substr(1)) != undefined) {
                id = "7" + id.substr(1);
            } else if (treeview.dataSource.get("8" + id.substr(1)) != undefined) {
                id = "8" + id.substr(1);
            } else if (treeview.dataSource.get("9" + id.substr(1)) != undefined) {
                id = "9" + id.substr(1);
            }
        }
        let selected = treeview.select();
        let selectedItem = treeview.dataItem(selected);
        if (selectedItem == undefined || selectedItem.id != id) {
            treeview.checkFromId(id);
        }
    }
}

function ImpostaObjP_Agenda(tipo, chiave, reset, callback) {
    return new Promise((resolve, reject) => {
        var parametri = { tipo: tipo, chiave: chiave, reset: reset };
        ajaxAgronica(indirizzohttp + "/ImpostaObjP_Agenda", JSON.stringify(parametri), function (risposta) {
            objP_agenda = risposta.RispostaStringa;
            if (callback !== undefined) {
                callback(risposta);
            }
            resolve();
        }, null, null, false);
    });
}

async function eliminaFiltro(tipo) {
    let ag = JSON.parse(objP_agenda);
    let chiave = "";
    let val = "";
    switch (tipo) {
        //centro
        case 2:
            chiave = ag.Piva;
            val = "centro";
            break;
        //campo
        case 3:
            chiave = ag.Piva & "_" & ag.Sa_Cod;
            val = "campo";
            break;
        //appezzamento
        case 4:
            if (ag.Campo_Cod != "" && ag.Campo_Cod != "0") {
                chiave = ag.Piva & "_" & ag.Sa_Cod & "_" & ag.Campo_Cod;
            } else {
                chiave = ag.Piva & "_" & ag.Sa_Cod;
            }
            val = "appezzamento";
            break;
        //impianto
        case 5:
            if (ag.Campo_Cod != "" && ag.Campo_Cod != "0") {
                chiave = ag.Piva & "_" & ag.Sa_Cod & "_" & ag.Campo_Cod & "_" & ag.Appezza;
            } else {
                chiave = ag.Piva & "_" & ag.Sa_Cod & "_" & ag.Appezza;
            }
            val = "impianto";
            break;
        //stalla
        case 7:
            chiave = ag.Piva + "_" + ag.Sa_Cod;
            val = "stalla";
            break;
        //raggruppamento stalla
        case 8:
            chiave = ag.Piva + "_" + ag.Sa_Cod + "_" + ag.Fabbricato; 
            val = "raggruppamento_stalla";
            break;
    }

    if (val != "") {
        await ImpostaObjP_Agenda(tipo, chiave, true);
        SelectKendoTab();
    }

}

function NuovoElemento() {
    //var tabStrip = $("#tabstrip").kendoTabStrip().data("kendoTabStrip");
    //var selectedIndex = tabStrip.select().index();
    if ($("#tipoNuovo").data("kendoDropDownList") == undefined) {
        $("#tipoNuovo").kendoDropDownList({
            //dataTextField: "text",
            //dataValueField: "value",
            //dataSource: data,
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {
                var t = $('#tipoNuovo').data("kendoDropDownList");
                var v = tipoNuovo.value();
                $('.datiDaNascondere').hide();
                SelezionaNuovoCorretto(parseInt(v));
            }
        });
    }
    //elemento Selezionato
    tipoNuovo = $("#tipoNuovo").data("kendoDropDownList");
    var v = $('#ElementoSelezionato').val();

    switch (v) {
        case "14":
            v = "5"
            break;
        case "8":
            v = "10";
    }

    tipoNuovo.value(v);
    $('#modalNuovo').modal('show');

    //$('#ddl_nuovoCentro-error').remove();
    //$('#ddl_nuovoCentro').css('border', '1px solid #ccc');

    CambiaSelezioneTipoNuovo();



}

function CambiaSelezioneTipoNuovo(e) {
    var t = $('#tipoNuovo').data("kendoDropDownList");
    var v = tipoNuovo.value();
    $('.datiDaNascondere').hide();
    SelezionaNuovoCorretto(parseInt(v));
}

function resetFiltro(tipo, idDiv) {
    ImpostaObjP_Agenda(tipo, "", true, function () {
        if (tipo === 2) {

            try {
                let grid = $("#divKendoCentro").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            try {
                let grid = $("#divKendoCatasto").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            try {
                let grid = $("#divKendoCampo").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            try {
                let grid = $("#divKendoAppezzamento").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            try {
                let grid = $("#divKendoImpianto").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            try {
                let grid = $("#divKendoFabbricato").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            try {
                let grid = $("#divKendoRaggruppamentiStalle").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            try {
                let grid = $("#divKendoContatto").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            try {
                let grid = $("#divKendoMacchina").data("kendoGrid");
                grid.refresh();
            } catch (e) { }

            Dati_Relativi_Percorso_Selezione2(tipo);
        } else {
            let grid = $("#" + idDiv).data("kendoGrid");
            grid.refresh();
            Dati_Relativi_Percorso_Selezione2(tipo);
        }
    });
}

function resetFiltrototale() {
    //dataFiltroValidita = new Date();
    //$("#filterData").data("kendoDatePicker").value(dataFiltroValidita);
    //$.cookie("MenuBS_Anagrafica.filterData", kendo.toString(dataFiltroValidita, 'd'));
    AggiornaAlberoAnagrafica();
    ImpostaObjP_Agenda(2, "", true, function () {
        //var tabStrip = $("#tabstrip").kendoTabStrip().data("kendoTabStrip");
        var selectedIndex = tabStrip.select().index();
        switch (selectedIndex) {
            case 0: //Azienda
                break;
            case 1: //Centri
                caricaGrigliaCentro();
                var grid = $("#divKendoCentro").data("kendoGrid");
                grid.refresh();
                break;
            case 2: //Catasto
                caricaGrigliaCatasto();
                var grid = $("#divKendoCatasto").data("kendoGrid");
                grid.refresh();
                break;
            case 3: //Campi
                caricaGrigliaCampo();
                var grid = $("#divKendoCampo").data("kendoGrid");
                grid.refresh();
                break;
            case 4: //Appezzamenti
                caricaGrigliaAppezzamento();
                var grid = $("#divKendoAppezzamento").data("kendoGrid");
                grid.refresh();
                break;
            case 5: //Impianti
                caricaGrigliaImpianto();
                var grid = $("#divKendoImpianto").data("kendoGrid");
                grid.refresh();
                break;
            case 6: //Esercizi
                caricaGrigliaEsercizio();
                var grid = $("#divKendoEsercizio").data("kendoGrid");
                grid.refresh();
                break;
            case 7: //Fabbricati
                caricaGrigliaFabbricato();
                var grid = $("#divKendoFabbricato").data("kendoGrid");
                grid.refresh();
                break;
            case 8: //Contatti
                caricaGrigliaContatto();
                var grid = $("#divKendoContatto").data("kendoGrid");
                grid.refresh();
                break;
            case 9: //Macchine
                caricaGrigliaMacchina();
                var grid = $("#divKendoMacchina").data("kendoGrid");
                grid.refresh();
                break;
            case 10:
                caricaGrigliaStalla();
                var grid = $("#divKendoStalla").data("kendoGrid");
                grid.refresh();
                break;
            case 11: //Raggruppamenti Stalle
                caricaGrigliaRaggruppamentoStalla();
                var grid = $("#divKendoRaggruppamentiStalle").data("kendoGrid");
                grid.refresh();
                break;
            case 12: //Capi
                caricaGrigliaZoo();
                var grid = $("#divKendoZoo").data("kendoGrid");
                grid.refresh();
                break;
            case 13: //Stazioni Meteo
                caricaGrigliaStazioniMeteo();
                var grid = $("#divKendoStazioniMeteo").data("kendoGrid");
                grid.refresh();
                break;
        }
        Dati_Relativi_Percorso_Selezione2(2);
    });
}

function SelezionaNuovoCorretto(v) {

    switch (parseInt(v)) {
        case 1:
            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Azienda", "Azienda"));

            $('#btn_nuovo').show();
            break;
        case 2:
            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Centro", "Centro"));

            $('#btn_nuovo').show();
            break;
        case 3:
            //CAMPO
            $('#btn_nuovo').hide();

            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Campo", "Campo"));
            CaricaCentriCampi($('#ddl_nuovoCentro_Campo'), false);
            $('.datiCampo').show();
            break;
        case 4:
            //Appezza
            $('#btn_nuovo').hide();

            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Appezzamento", "Appezzamento"));
            CaricaCentri($('#ddl_nuovoCentro'), true);
            CaricaCampi($('#ddl_nuovoCampo'));
            $('.datiAppezza').show();
            break;
        case 5:
            //Impianto
            $('#btn_nuovo').hide();

            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Impianto", "Impianto"));
            CaricaCentri($('#ddl_nuovoCentro'), false);
            CambioCentro();
            //                    CaricaCampi($('#ddl_nuovoCampo'), $('#ddl_nuovoCentro').val());

            $('.datiImpianto').show();
            break;

        case 6:
            //Catasto
            $('#btn_nuovo').hide();

            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Particella", "Particella"));
            CaricaCentri($('#ddl_nuovoCentro'), false);
            $('.datiCatasto').show();
            break;
        case 7:
            //Raggruppamento Stalle
            $('#btn_nuovo').hide();

            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "RaggruppamentoStalla", "Raggruppamento Stalla"));
            CaricaCentri($('#ddl_nuovoCentro'), false, true);
            CaricaStalle($('#ddl_nuovoFabbricatoStalla'), 0);
            $('.datiRaggruppamento').show();
            break;
        case 9:
            //Raggruppamento Stalle
            $('#btn_nuovo').hide();

            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Animale", "Animale"));
            CaricaCentri($('#ddl_nuovoCentro'), false, true);
            CaricaStalle($('#ddl_nuovoFabbricatoStalla'), 0);
            $('.datiRaggruppamento').show();
            break;
        case 10:
            //Fabbricato
            $('#btn_nuovo').hide();

            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Fabbricato", "Fabbricato"));
            CaricaCentri($('#ddl_nuovoCentro'), false);
            $('.datiFabbricato').show();
            break;
        case 11:
            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Contatto", "Contatto"));

            $('#btn_nuovo').show();
            break;
        case 12:
            //Macchine
            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "MacchinaAttrezzatura", "Macchina / Attrezzatura"));

            $('#btn_nuovo').show();
            break;
        case 13:
            //Meteo e DSS
            $('#btn_nuovo').hide();
            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "DSS e Meteo", "DSS e Meteo"));
            $('.datiMeteoDSS').show();
            CaricaCentri($('#ddl_nuovoCentro'), false);
            CaricaCmb_MeteoDssSorgente();
            break;
        case 15:
            // Prodotti
            $('#btn_nuovo').hide();
            $('#lbl_new_item').html(Traduzione(menuBSAnagraficaResx, "NuovoElemento", "Nuovo Elemento") + " - " + Traduzione(menuBSAnagraficaResx, "Prodotto", "Prodotto"));
            $(".datiProdotto").show();
            CaricaCmb_Categorie_Prodottti('ddl_categorie_prodotti');
            break;

    }

}

function CaricaCmb_Categorie_Prodottti(obj) {
    setTimeout(function () {
        ddlCategorie_Prodotti = creaKendoDropDownList(obj, { read: RicercaCategorieMagazzino }, "NomeComune", "Elem_Cod");
        Set_KendoDDLValue(obj, 0);
        ddlCategorie_Prodotti.bind("change", categoriaMagazzino_change);
    }, 200);
}

function RicercaCategorieMagazzino(options) {
    if (elencoCategorieMagazzino === null || elencoCategorieMagazzino == undefined) {

        var tutteLeCategorie = CaricaCategorieMagazzinoXUtente();

        elencoCategorieMagazzino = [];
        let i = 0;
        for (var x = 0; x < tutteLeCategorie.length; x++) {
            if (tutteLeCategorie[x].Tabella === "Materie_Prime" || tutteLeCategorie[x].NomeComune === "" || tutteLeCategorie[x].Tabella === "TipologieSementi") {
                // Escludo mangimi
                var elem_Cod = tutteLeCategorie[x].Elem_Cod;
                if (elem_Cod != 306)
                    elencoCategorieMagazzino.push({ Elem_Cod: elem_Cod, NomeComune: tutteLeCategorie[x].NomeComune });
            }
        }

        var objVuoto = {
            "Elem_Cod": 0,
            "NomeComune": Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona")
        };
        elencoCategorieMagazzino.unshift(objVuoto);
    }

    options.success(elencoCategorieMagazzino);
}

function categoriaMagazzino_change(e) {
    var elem_cod = parseInt(e.target.value);
    if (elem_cod != 0)
        $('#btn_nuovo').show();
    else
        $('#btn_nuovo').hide();
}


function CaricaCmb_MeteoDssSorgente() {

    setTimeout(function () {
        cmbMeteoDssTipoSorgente = $("#ddl_nuovoMeteoDssSorgenteDati").kendoDropDownList({
            autoBind: true,
            filter: null,
            dataTextField: "TipoSorgente_Des",
            dataValueField: "TipoSorgente_Cod",
            dataSource: RicaricaTipoSorgente(),
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona") + "...",
            change: function (e) {

                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

                RicaricaOrigineDati();

            }

        }).data("kendoDropDownList");

        if (cmbMeteoDssOrigineDati === undefined) {
            cmbMeteoDssOrigineDati = $("#ddl_nuovoMeteoDssOrigine").kendoDropDownList({
                autoBind: true,
                width: 300,
                dataTextField: "sorgente_des",
                dataValueField: "sorgente_cod",
                filter: "contains",
                change: function (e) {
                    MenuBS_Anagrafica_AvModAlg_Ricarica(e.sender.dataItem().sorgente_cod);
                    $('#btn_nuovo').show();
                },
                optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona") + "..."

            }).data("kendoDropDownList");

        }

        if (kendoDropDownTreeNuovoMeteoDssModelliPrevisionali === undefined) {
            try {
                kendoDropDownTreeNuovoMeteoDssModelliPrevisionali = $("#kendoDropDownTree_nuovoMeteoDssModelliPrevisionali").kendoDropDownTree({
                    placeholder: Traduzione(menuBSAnagraficaResx, "SelezionareUnaSpecieVegetale", "Selezionare una specie vegetale"),
                    autoClose: false,
                    clearButton: true,
                    dataSource: [],
                    checkboxes: {
                        checkChildren: true
                    }
                }).data("kendoDropDownTree");
            } catch (e) {

            }


        }
    }, 200);

}





function MenuBS_Anagrafica_AvModAlg_Ricarica(origine_cod) {

    if (origine_cod === "") {
        return;
    }

    let objAgenda = JSON.parse(objP_agenda);

    let sa_cod = parseInt($("#ddl_nuovoCentro").data("kendoDropDownList").value());
    let SorgenteDati_Cod = parseInt($("#ddl_nuovoMeteoDssSorgenteDati").data("kendoDropDownList").value());

    let ParametriStazione = { piva: objAgenda.Piva, sa_cod: sa_cod, sorgentedati_cod: SorgenteDati_Cod, origine_cod: origine_cod };

    $("#kendoDropDownTree_nuovoMeteoDssModelliPrevisionali").parent().find(".k-i-close").click();

    ajaxAgronica(indirizzohttp + "/AvModAlg_Inizializza",
        JSON.stringify(ParametriStazione),
        function (risposta) {


            var dataSource = new kendo.data.HierarchicalDataSource({
                data: risposta.RispostaStringa.ModelliPrevisionaliTree
            });


            kendoDropDownTreeNuovoMeteoDssModelliPrevisionali.setDataSource(dataSource);

            kendoDropDownTreeNuovoMeteoDssModelliPrevisionali.value(risposta.RispostaStringa.ModelliPrevisionaliTreeListaCheck);
            kendoDropDownTreeNuovoMeteoDssModelliPrevisionali.trigger("change");

            if (risposta.RispostaStringa.AssociazioneCentroOrigine) {
                AttivaModificaDati(risposta.RispostaStringa.ModelliPrevisionali);
            }

        },
        null
    );

}

function AttivaModificaDati(ModelliPrevisionali) {

    let vModelliKey = [];
    for (var i = 0; i < ModelliPrevisionali.length; i++) {
        if (ModelliPrevisionali[i].Selezionato) {
            vModelliKey.push(ModelliPrevisionali[i].AvModAlg_Cod);
        }
    }

}

function RicaricaOrigineDati() {

    var lat = "0";
    var lng = "0";

    if ($("#txt_latcentro").val() != "") {
        lat = $("#txt_latcentro").val();
    }

    if ($("#txt_loncentro").val() != "") {
        lng = $("#txt_loncentro").val();
    }


    let tipoSorgenteSelezionata = cmbMeteoDssTipoSorgente.value();

    if (tipoSorgenteSelezionata == "") {

        cmbMeteoDssOrigineDati.setDataSource([]);
        cmbMeteoDssOrigineDati.refresh();
        return;

    }

    var param = "{ TipoSorgenteDati: " + tipoSorgenteSelezionata + ", latcentro: " + lat + ", longcentro: " + lng + " }";

    ajaxAgronicaSync("../DataAnalisiBI/Meteo/MeteoWS.aspx/RicaricaOrigineDati2",
        param,
        true,
        function (risposta) {

            let OrigineDati = JSON.parse(risposta.RispostaStringa);

            cmbMeteoDssOrigineDati.setDataSource(OrigineDati);
            cmbMeteoDssOrigineDati.refresh();
            cmbMeteoDssOrigineDati.select(-1);
        },
        null
    );

}

function CaricaCentri(obj, updateCampo, updateStalle) {

    setTimeout(function () {
        if (ddlCentri == undefined) {

            ddlCentri = $(obj).kendoDropDownList({
                dataTextField: "des",
                dataValueField: "val",
                dataSource: { transport: { read: leggiCentri } },
                open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    var ds = this.dataSource.data();
                    var objP_A = JSON.parse(objP_agenda);
                    if (ds.length == 1) {
                        this.select(1);
                        this.trigger("change");
                    } else if (objP_A.Sa_Cod != undefined) {
                        this.value(objP_A.Sa_Cod);
                        this.trigger("change");
                    }
                    kendoDropDownAdjustWidth(e);
                },
                change: function (e) {
                    $('#ddl_nuovoCampo').empty();
                    $('#ddl_nuovoAppezzamento').empty();

                    var sa_cod = this.value();
                    sa_codSelezionato = sa_cod;
                    if (!sa_cod == 0) {
                        $('#ddl_nuovoCentro-error').remove();
                        $('#ddl_nuovoCentro').css('border', '1px solid #ccc');
                    }

                    if (sa_cod === "") {
                        $('#btn_nuovo').hide();
                        return false;
                    }

                    //CambioCampo();
                    //CaricaAppezzamento($('#ddl_nuovoAppezzamento'), sa_cod, 0);
                    let nuovo = ["3", "4", "6", "10"];
                    if (nuovo.indexOf(tipoNuovo.value()) !== -1 && sa_cod !== 0) {
                        $('#btn_nuovo').show();
                    } else {
                        $('#btn_nuovo').hide();
                    }

                    switch (tipoNuovo.value()) {
                        case "4":
                            CaricaCampi($('#ddl_nuovoCampo'));
                            break;
                        case "5":
                            CaricaCampi($('#ddl_nuovoCampo'));
                            CaricaAppezzamento($('#ddl_nuovoAppezzamento'), sa_cod, 0);
                            break;
                        case "7":
                        case "9":
                            CaricaStalle($('#ddl_nuovoFabbricatoStalla'), 0);
                            break;
                    }

                },
                optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona")
            }).data("kendoDropDownList");
        } else {
            ddlCentri.dataSource.read();
            ddlCentri.refresh();
        }
    }, 200)

}

function CaricaCentriCampi(obj, updateCampo, updateStalle) {

    setTimeout(function () {
        if (ddlCentriCampi == undefined) {

            ddlCentriCampi = $(obj).kendoDropDownList({
                dataTextField: "des",
                dataValueField: "val",
                dataSource: { transport: { read: leggiCentri } },
                open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    var ds = this.dataSource.data();
                    var objP_A = JSON.parse(objP_agenda);
                    if (ds.length == 1) {
                        this.select(1);
                        this.trigger("change");
                    } else if (objP_A.Sa_Cod != undefined) {
                        this.value(objP_A.Sa_Cod);
                        this.trigger("change");
                    }
                    kendoDropDownAdjustWidth(e);
                },
                change: function (e) {
                    $('#ddl_nuovoCampo').empty();
                    $('#ddl_nuovoAppezzamento').empty();

                    var sa_cod = this.value();
                    sa_codSelezionato = sa_cod;
                    if (!sa_cod == 0) {
                        $('#ddl_nuovoCentro-error').remove();
                        $('#ddl_nuovoCentro').css('border', '1px solid #ccc');
                    }

                    if (sa_cod === "") {
                        $('#btn_nuovo').hide();
                        return false;
                    }

                    //CambioCampo();
                    //CaricaAppezzamento($('#ddl_nuovoAppezzamento'), sa_cod, 0);
                    let nuovo = ["3", "4", "6", "10"];
                    if (nuovo.indexOf(tipoNuovo.value()) !== -1 && sa_cod !== 0) {
                        $('#btn_nuovo').show();
                    } else {
                        $('#btn_nuovo').hide();
                    }

                    switch (tipoNuovo.value()) {
                        case "4":
                            CaricaCampi($('#ddl_nuovoCampo'));
                            break;
                        case "5":
                            CaricaCampi($('#ddl_nuovoCampo'));
                            CaricaAppezzamento($('#ddl_nuovoAppezzamento'), sa_cod, 0);
                            break;
                        case "7":
                        case "9":
                            CaricaStalle($('#ddl_nuovoFabbricatoStalla'), 0);
                            break;
                    }

                },
                optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona")
            }).data("kendoDropDownList");
        } else {
            ddlCentriCampi.dataSource.read();
            ddlCentriCampi.refresh();
        }
    }, 200)

}

function leggiCentri(options) {
    var parametri = {};

    ajaxAgronica(indirizzohttp + "/CaricaCentro_ddl",
        JSON.stringify(parametri),
        function (risposta) {
            var data = JSON.parse(risposta.RispostaStringa);
            options.success(data);
        },
        null);
}

function CaricaCampi(obj) {
    setTimeout(function () {
        if (ddlCampi == undefined) {
            ddlCampi = $(obj).kendoDropDownList({
                filter: "contains",
                dataTextField: "des",
                dataValueField: "val",
                dataSource: { transport: { read: leggiCampi } },
                open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    var objP_A = JSON.parse(objP_agenda);
                    if (objP_A.Campo_Cod != undefined) {
                        this.value(objP_A.Campo_Cod);
                        this.trigger("change");
                    }
                    kendoDropDownAdjustWidth(e);
                },
                change: function (e) {
                    $('#ddl_nuovoAppezzamento').empty();
                    if (tipoNuovo.value() == "5") {
                        CaricaAppezzamento($('#ddl_nuovoAppezzamento'), $('#ddl_nuovoCentro').val(), $('#ddl_nuovoCampo').val());
                    }
                },
                optionLabel: Traduzione(menuBSAnagraficaResx, "Nessuno", "Nessuno")
            }).data("kendoDropDownList");
        } else {
            ddlCampi.dataSource.read();
            ddlCampi.refresh();
        }
    }, 200);
}

function leggiCampi(options) {
    let sa_cod = ddlCentri.value();
    if (sa_cod !== "" && sa_cod !== 0) {
        var parametri = { sa_cod: sa_cod };
        ajaxAgronica(indirizzohttp + "/CaricaCampo_ddl", JSON.stringify(parametri), function (risposta) {
            var data = JSON.parse(risposta.RispostaStringa);
            options.success(data);
        }, null);
    } else {
        options.success([]);
    }
}

function CaricaAppezzamento(obj, sa_cod, campo_cod) {
    setTimeout(function () {
        if (ddlAppezzamenti == undefined) {
            ddlAppezzamenti = $(obj).kendoDropDownList({
                filter: "contains",
                dataTextField: "des",
                dataValueField: "val",
                dataSource: { transport: { read: leggiAppezzamenti } },
                open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    var objP_A = JSON.parse(objP_agenda);
                    if (objP_A.Appezza != undefined) {
                        this.value(objP_A.Appezza);
                        this.trigger("change");
                    }
                    kendoDropDownAdjustWidth(e);
                },
                change: function (e) {
                    if (this.value() !== "") {
                        $('#btn_nuovo').show();
                    } else {
                        $('#btn_nuovo').hide();
                    }
                },
                optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona")
            }).data("kendoDropDownList");
        } else {
            ddlAppezzamenti.dataSource.read();
            ddlAppezzamenti.refresh();
        }
    }, 200);
}

function leggiAppezzamenti(options) {
    let sa_cod = ddlCentri.value();
    let campo_cod = ddlCampi != null ? ddlCampi.value() : 0;
    if (sa_cod !== "" && sa_cod !== 0) {
        if (campo_cod === "") {
            campo_cod = 0;
        }
        var parametri = { sa_cod: sa_cod, campo_cod: campo_cod };

        ajaxAgronica(indirizzohttp + "/CaricaAppezzamento_ddl", JSON.stringify(parametri), function (risposta) {
            var data = JSON.parse(risposta.RispostaStringa);
            options.success(data);
        }, null);

    } else {
        options.success([]);
    }
}

function CaricaStalle(obj, sa_cod) {

    setTimeout(function () {
        if (ddlFabbricati == undefined) {
            ddlFabbricati = $(obj).kendoDropDownList({
                dataTextField: "des",
                dataValueField: "val",
                dataSource: { transport: { read: leggiStalle } },
                open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    var objP_A = JSON.parse(objP_agenda);
                    if (objP_A.Fabbricato != undefined) {
                        this.value(objP_A.Fabbricato);
                    } else {
                        this.value("");
                    }
                    e.sender.trigger("change");
                    kendoDropDownAdjustWidth(e);
                },
                change: function (e) {
                    if (this.value() !== "") {
                        $('#btn_nuovo').show();
                    } else {
                        $('#btn_nuovo').hide();
                    }
                },
                optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona")
            }).data("kendoDropDownList");
        } else {
            ddlFabbricati.dataSource.read();
            ddlFabbricati.refresh();
        }
    }, 200);
}

function leggiStalle(options) {
    let sa_cod = ddlCentri.value();
    if (sa_cod !== undefined && sa_cod !== "" && sa_cod !== 0) {
        var parametri = { sa_cod: sa_cod };

        ajaxAgronica(indirizzohttp + "/CaricaStalle_ddl",
            JSON.stringify(parametri),
            function (risposta) {
                var data = JSON.parse(risposta.RispostaStringa);
                options.success(data);
            },
            null);
    } else {
        options.success([]);
    }
}

function gestioneNewElementDssMeteo() {

    let objAgenda = JSON.parse(objP_agenda);

    let sa_cod = parseInt($("#ddl_nuovoCentro").data("kendoDropDownList").value());
    let SorgenteDati_Cod = parseInt($("#ddl_nuovoMeteoDssSorgenteDati").data("kendoDropDownList").value());
    let origine1 = $("#ddl_nuovoMeteoDssOrigine").data("kendoDropDownList");
    let origine_cod = parseInt(origine1.value());
    let origine_des = origine1.dataItem().sorgente;

    let vDati = kendoDropDownTreeNuovoMeteoDssModelliPrevisionali.value();
    let vDssModelli = [];
    for (var i = 0; i < vDati.length; i++) {

        let lAvModAlg = vDati[i].split("|");
        let vAvModAlg_Cod = lAvModAlg[0].split("-");
        if (vAvModAlg_Cod.length > 1) {

            let m = {
                AvModAlg_Cod: lAvModAlg[0],
                AvModAlg_Des: "",
                DescrAvversita: "",
                DescrModello: lAvModAlg[1],
                DescrAlgoritmo: "",
                Veg_Cod: 0,
                Veg_Des: "",
                Selezionato: false
            };

            vDssModelli.push(m);

        }
    }

    let ParametriStazione = { piva: objAgenda.Piva, sa_cod: sa_cod, sorgentedati_cod: SorgenteDati_Cod, origine_cod: origine_cod, origine_des: origine_des, ModelliPrevisionali: vDssModelli };


    ajaxAgronica(indirizzohttp + "/DssMeteoMemorizzaNuovaStazione", JSON.stringify(ParametriStazione),
        function (risposta) {

            kendo.alert(risposta.RispostaStringa);
            let tab = ImpostaSelezione(13, 1);
            tabStrip.select(tab);

        }, null);


}

function gotoNewElement(tipo, chiave) {
    WaitFrame.show();

    ajaxAgronicaSync(
        './MenuBs_Anagrafica.aspx/gotoNewElement',
        "{tipo: " + tipo + ", chiave: '" + chiave + "' }",
        true,
        function (risposta) {
            if (risposta.RispostaOK) {
                let parametroVisibilita = riportaParametroVisibilita();
                if (risposta.RispostaStringa.indexOf("?") >= 0) {
                    parametroVisibilita = parametroVisibilita.replace("?", "&")
                }
                window.location = risposta.RispostaStringa + parametroVisibilita;
            } else {
                WaitFrame.hide();
                alert(risposta.Errore);
            }
        }
    );

}

function CambioCentro() {

}

function CambioCampo() {
    var sa_cod = $('#ddl_nuovoCentro').val();
    var campo_cod = 0;
    if (!!$('#ddl_nuovoCampo').val())
        campo_cod = $('#ddl_nuovoCampo').val();
    CaricaAppezzamento($('#ddl_nuovoAppezzamento'), sa_cod, campo_cod);
}

function ValidaxNuovo() {

    var flag = true;

    if ($('#ddl_nuovoCentro').val() == 0) {
        $('#ddl_nuovoCentro').parent().append('<label id="ddl_nuovoCentro-error" class="custom_val error" for="ddl_nuovoCentro" style="position: relative !important; top: 0 !important;">' + Traduzione(menuBSAnagraficaResx, "SelezionareUnaVoce", "Selezionare una voce") + '</label>');
        $('#ddl_nuovoCentro').css('border', '1px solid #D41E1A');
        flag = false;
    }
    else {
        $('#ddl_nuovoCentro-error').remove();
        $('#ddl_nuovoCentro').css('border', '1px solid #ccc');
    }

    if ($('#ddl_nuovoCentro_Campo').val() == 0) {
        $('#ddl_nuovoCentro_Campo').parent().append('<label id="ddl_nuovoCentro-error" class="custom_val error" for="ddl_nuovoCentro" style="position: relative !important; top: 0 !important;">' + Traduzione(menuBSAnagraficaResx, "SelezionareUnaVoce", "Selezionare una voce") + '</label>');
        $('#ddl_nuovoCentro_Campo').css('border', '1px solid #D41E1A');
        flag = false;
    }
    else {
        $('#ddl_nuovoCentro-error').remove();
        $('#ddl_nuovoCentro').css('border', '1px solid #ccc');
    }

    return flag;
}

function disabilita_return_filtro() {
}

function SelezionaRigaDaAlbero() {

    //Cancello le eventuali selezioni dalle griglie
    if ($('#divKendoAzienda').data("kendoGrid") !== undefined)
        $('#divKendoAzienda').data("kendoGrid").clearSelection();
    if ($('#divKendoCentro').data("kendoGrid") !== undefined)
        $('#divKendoCentro').data("kendoGrid").clearSelection();
    if ($('#divKendoCampo').data("kendoGrid") !== undefined)
        $('#divKendoCampo').data("kendoGrid").clearSelection();
    if ($('#divKendoAppezzamento').data("kendoGrid") !== undefined)
        $('#divKendoAppezzamento').data("kendoGrid").clearSelection();
    if ($('#divKendoImpianto').data("kendoGrid") !== undefined)
        $('#divKendoImpianto').data("kendoGrid").clearSelection();
    if ($('#divKendoCatasto').data("kendoGrid") !== undefined)
        $('#divKendoCatasto').data("kendoGrid").clearSelection();
    if ($('#divKendoContatto').data("kendoGrid") !== undefined)
        $('#divKendoContatto').data("kendoGrid").clearSelection();
    if ($('#divKendoMacchina').data("kendoGrid") !== undefined)
        $('#divKendoMacchina').data("kendoGrid").clearSelection();
    if ($('#divKendoFabbricato').data("kendoGrid") !== undefined)
        $('#divKendoFabbricato').data("kendoGrid").clearSelection();
    if ($('#divKendoStalle').data("kendoGrid") !== undefined)
        $('#divKendoStalle').data("kendoGrid").clearSelection();

    if (ultimaSelezioneAlbero == 0) {
        if (stringa_id == "") {
            return false;
        }
        else {
            ultimaSelezioneAlbero = stringa_id;
        }
    }

    var i = 0;
    // Determino la tipologia della riga cliccata
    $('#tabDati li').each(function () {
        if ($(this).hasClass('active'))
            return false;
        i++;
    });

    var tipo = parseInt(ultimaSelezioneAlbero.split("§")[0]);

    var piva = ultimaSelezioneAlbero.split('§')[1];
    var sa_cod = ultimaSelezioneAlbero.split('§')[2];
    var campo_cod = ultimaSelezioneAlbero.split('§')[3];
    var appezza = ultimaSelezioneAlbero.split('§')[4];
    var imp = ultimaSelezioneAlbero.split('§')[5];

    var Part_cod = ultimaSelezioneAlbero.split('§')[6];
    var prov = ultimaSelezioneAlbero.split('§')[7];
    var comune = ultimaSelezioneAlbero.split('§')[8];
    var sezione = ultimaSelezioneAlbero.split('§')[9];
    var foglio = ultimaSelezioneAlbero.split('§')[10];
    var numero = ultimaSelezioneAlbero.split('§')[11];
    var subalterno = ultimaSelezioneAlbero.split('§')[12];

    var fabbricato_cod = ultimaSelezioneAlbero.split('§')[14];

    switch (tipo) {
        case 2:
            //impresa                    
            $('#divKendoAzienda').find('.edit_piva[chiave="' + piva + '"]').parent().parent().addClass('success');
            var nome_azienda = $('#divKendoAzienda').find('.edit_piva[chiave="' + piva + '"]').parent().parent().children('td:nth-child(2)').text();

            var grid = $('#divKendoAzienda').data("kendoGrid");
            var view = grid.dataSource.view();
            var rows = $.grep(view, function (item) {
                nome_azienda = item.rag_soc;
                return item.chiave === piva;
            }).map(function (item) {
                return grid.tbody.find("[data-uid=" + item.uid + "]");
            });

            grid.select(rows[0]);

            $('.div_filtri_attivi').append('<div style="background-color: #052747; padding: 3px; color: #fff; float: left;">' + nome_azienda + '</div>');
            break;
        case 3:
            //centro
            var id_select = piva + "/" + sa_cod;

            $('#divKendoCentro').find('.edit_centro[chiave="' + piva + '_' + sa_cod + '"]').parent().parent().addClass('success');
            break;
        case 4:
            //campo
            $('#divKendoCampo').find('.edit_campo[chiave="' + piva + '_' + sa_cod + "_" + '"]').parent().parent().addClass('success');
            break;
        case 5:
            //appezzamento
            $('#divKendoAppezzamento').find('.edit_appezza[chiave="' + piva + '_' + sa_cod + "_" + appezza + '"]').parent().parent().addClass('success');
            break;
        case 6:
        case 7:
        case 8:
            //impianti
            $('#divKendoImpianto').find('.edit_impianti[chiave="' + piva + '_' + sa_cod + "_" + appezza + "_" + imp + '"]').parent().parent().addClass('success');
            break;
        case 10:
            //catasto
            $('#divKendoCatasto').find('.edit_catasto[chiave="' + piva + '_' + sa_cod + "_" + prov + "_" + comune + "_" + sezione + "_" + foglio + "_" + numero + "_" + subalterno + "_" + Part_cod + '"]').parent().parent().addClass('success');
            break;
        case 22:
            //fabbricati
            $('#divKendoFabbricato').find('.edit_fabbricato[chiave="' + piva + '_' + sa_cod + "_" + fabbricato_cod + '"]').parent().parent().addClass('success');
            break;
    }


    //Salva operazioni Agenda
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/Salva_OperazioniAgenda',
        data: "{ultimaSelezioneAlbero: '" + ultimaSelezioneAlbero + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: false,
        success: function (r) {
            //window.location = "../Anagrafica/Impianto_Edit.aspx";
        }
    });


    //Visualizza il percorso con label
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/Dati_Relativi_Percorso_Selezione',
        data: "{tipo: '" + tipo + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            //window.location = "../Anagrafica/Impianto_Edit.aspx";
            //alert(r.d[0]);
            $('#menu_label_navigazione div').empty();

            for (var i = 0; i < r.d.length; i++) {
                $('#menu_label_navigazione > div').append(r.d[i]);
            }

        }
    });


}

function GotToFiltrino() {
    WaitFrame.show();
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/GetFiltrino',
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            console.log(r);
            window.location = r.d;
        }
    });

}

function normalizeXOption(str) {
    str = str.replace("'", "-");
    return str;
}


function caricaGestioneEsercizi(open) {
    $(document.body).append('<div id="gestioneEserciziWindow"></div>');
    $('#gestioneEserciziWindow').kendoWindow({
        title: Traduzione(menuBSAnagraficaResx, "ChiusuraAperturaEsercizi", "Chiusura/Apertura Esercizi"),
        modal: true,
        resizable: true,
        visible: false,
        iframe: true,
        width: "80%",
        height: "80%",
        content: "../Anagrafica/GestioneEsercizi.aspx?p=" + getParameterByName('p'),
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () { $('#gestioneEserciziWindow').kendoWindow('destroy'); }, 200);
            /* if(!open) {
                var oggi = (new Date()).toISOString().split('T')[0];
                kendo.confirm("Vuoi disattivare l'apertura automatica della finestra per gli esercizi poliennali?").then(function () {
                    $.cookie("MenuBS_Anagrafica.gestioneEserciziWindow", "0", { expires: 10000 });
                }, function () {
                    $.cookie("MenuBS_Anagrafica.gestioneEserciziWindow", oggi);
                });
            } */
        }
    }).data('kendoWindow').center();
    if (open) apriGestioneEsercizi();
}


function apriGestioneEsercizi() {
    $('#gestioneEserciziWindow').data('kendoWindow').open();
}

function chiudiGestioneEsercizi() {
    $('#gestioneEserciziWindow').data('kendoWindow').close();
}

function alberoFiltriCambiati(catastoChanged) {
    if (catastoChanged) {
        let showCatasto = $('#chk_showHideCatasto').is(':checked');
        $.cookie("MenuBS_Anagrafica.filtriCatastoCheckbox", showCatasto);
    }

    let gisTree = $("#gis_treeview").data();
    gisTree.kendoGisTreeView.dataSource.read();
}


/**
 * Carica l'albero dal server
 * @param {object} options
 */
function GetNodesAlberoAnagrafe(options) {

    //ad ogni lettura aggiorno Piva, Sa_cod in oggetto cfgSerialized
    // var lPiva = $('#ddl_azienda_html').data("kendoDropDownList").value();
    // var lSaCod = $('#ddl_Sa_Cod_html').data("kendoDropDownList").value();
    var lPiva = $('#' + hidden_azienda_ClientID).val();
    var lSaCod = $('#AlberoCentriDropdown').val();
    let showCatasto = $('#chk_showHideCatasto').is(':checked');

    var lDataDa = $("#filterData").val();
    var lDataA = $("#filterData").val();

    if (lDataDa !== "" && kendo.parseDate(lDataDa) === null) {
        kendo.alert(Traduzione(menuBSAnagraficaResx, "DataInseritaNonValida", "La data inserita non è valida"));
        return;
    }

    console.log("Richiesta Caricamento Albero 2017 su p.iva: " + lPiva + ", sa_Cod: " + lSaCod);

    var cfg = JSON.parse(CaricaConfigurazioneAlberoAnagrafica());
    cfg.Piva = lPiva;
    cfg.Sa_Cod = lSaCod;
    cfg.Flag_CatastoAziendale = showCatasto;


    let useFilterData = false;
    if (lDataDa !== "" && useFilterData) {
        cfg.dataInizio = kendo.parseDate(lDataDa).toISOString();
    } else {
        cfg.dataInizio = kendo.parseDate("01/01/1900").toISOString();
    }

    if (lDataA !== "" && useFilterData) {
        cfg.dataFine = kendo.parseDate(lDataA).toISOString();
    } else {
        cfg.dataFine = kendo.parseDate("31/12/2100").toISOString();
    }

    SalvaConfigurazioneAlberoAnagrafica(cfg);

    var cfgSerialized = CaricaConfigurazioneAlberoAnagrafica();
    var id = "0";

    ajaxAgronica(pathCoreWS + "AgronicaControlli_2010/AlberoAnagrafica2017.aspx/GetNodesAlberoAnagrafe",
        "{ cfgSerialized: '" + cfgSerialized + "', id: '" + id + "', PathRoot: '', objP_server: '" + objP_server + "', objP_utenti: '" + objP_utenti + "' }",
        function (risposta) {

            var hDati = JSON.parse(risposta.RispostaStringa);

            if (hDati.length === 1) {
                if (hDati[0].id == null) {
                    hDati = [];
                }
            }

            options.success(hDati);

        }, null);
}

function CaricaConfigurazioneAlberoAnagrafica() {
    return $("#" + hdAlberoAnagrafica2017cfg_ClientID).val();
}

function SalvaConfigurazioneAlberoAnagrafica(cfg) {
    $("#" + hdAlberoAnagrafica2017cfg_ClientID).val(JSON.stringify(cfg));
}

function apriAlberoAnagrafica() {
    // $(document.body).append('<div id="apriAlberoWindow"></div>');
    $('#windowAlbero').kendoWindow({
        title: Traduzione(menuBSAnagraficaResx, "AlberoAnagrafica", "Albero Anagrafica"),
        // modal: true,
        resizable: true,
        visible: false,
        iframe: true,
        width: "80%",
        height: "80%",
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () {
                $('#windowAlbero').kendoWindow('destroy');
            }, 200);
        }
    }).data('kendoWindow').center().open();
    let AnagTree = AlberoAnagrafica();
    AnagTree.setVisible(true);
}

function LeggiDisciplinarePrivato() {
    return new Promise((resolve, reject) => {

        var param = {};

        ajaxAgronica(indirizzohttp + "/LeggiDisciplinarePrivato", JSON.stringify(param),
            function (risposta) {
                if (risposta.RispostaStringa === "true") {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, null, false);
    });
}

function StampaReport(report) {

    var parameteri = '{report:"' + report + '"}';

    $.ajax({
        type: 'POST',
        url: indirizzohttp + "/GestioneStampe",
        data: parameteri,
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (risposta) {
            if (risposta.d.RispostaOK) {
                if ((risposta.d.ParametroDue_stringa) && (risposta.d.ParametroDue_stringa != "")) {
                    switch (risposta.d.Tipo) {
                        // Script JS
                        case "1":
                            window.open(risposta.d.ParametroDue_stringa);
                            //$('#cont_script').append(risposta.d.ParametroDue_stringa);
                            break;
                        // Inserimento codice HTML in....
                        case "2":
                            break;
                    }
                }
                else {
                    if (risposta.d.RispostaStringa !== "") {
                        window.location = risposta.d.RispostaStringa;
                        WaitFrame.show();
                    } else {
                        kendo.alert("Operazione non implementata");
                    }
                }
            } else
                alert(risposta.d.Errore);
        }
    });

}

function riportaParametroVisibilita() {
    const urlParams = new URLSearchParams(window.location.search);
    const myParam = urlParams.get('visibilita');
    let returnString = ""
    if (myParam != undefined) {
        returnString = "?visibilita=" + myParam;
    }
    return returnString;
}

function getParametroVisibilita() {
    const urlParams = new URLSearchParams(window.location.search);
    const myParam = urlParams.get('visibilita');
    let returnString = ""
    if (myParam != undefined) {
        returnString = myParam;
    }
    return returnString;
}