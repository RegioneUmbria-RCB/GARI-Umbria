/**
 * Genera un nuovo controllo kendo Dialog
 * @param {string} selector Selettore JQuery per il div da trasformare
 * @param {any} titolo Titolo della dialog
 * @param {any} conferma funzione di conferma
 */
function GeneraKendoDialog(selector, titolo, conferma) {
    $(selector).kendoDialog({
        width: "400px",
        title: titolo,
        closable: true,
        modal: true,
        visible: false,
        actions: [{ text: 'Conferma', action: conferma }]
    });
}

/**
 * Genera nuovo controllo kendo windows
 * @param {string} selector
 * @param {string} titolo
 * @param {string} widthPercent
 * @param {string} heigthPercent
 */
function GeneraKendoWindow(selector, titolo, widthPercent, heigthPercent) {
    $(selector).kendoWindow({
        width: widthPercent,
        height: heigthPercent,
        title: titolo,
        pinned: false,
        visible: false,
        actions: ["Pin", "Minimize", "Maximize", "Close"]
    }).data("kendoWindow");

    //"Pin": tenere presente per impostare a fisso l'albero...
}

function generaAlberoAnagrafica(IDControllo) {

    var cfg = CaricaConfigurazioneAlberoAnagrafica();

    var funzioniCRUD = { funzioneRead: GetNodesAlberoAnagrafe };

    var campiKendoModel = { id: "id", hasChildren: true, children: "items" };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareSelect: generaAlberoAnagraficaSelect,
        funzioneDaChiamareCheck: generaAlberoAnagraficaCheck,
        funzioneDaChiamareExpand: generaAlberoAnagraficaExpand,
        agroDialogTreeViewFilter_onAfterOkClick: agroDialogTreeViewFilter_onAfterOkClick
    };

    var parametriPerLettura = {};
    var parametriDataSource = {};
    var parametriKendoTreeView = {};

    var AlberoAnagraficaSearchTreeView = creaAlberoAnagrafica2017(
        cfg,
        IDControllo,
        funzioniCRUD,
        "id",
        campiKendoModel,
        parametriPerLettura,
        parametriDataSource,
        parametriKendoTreeView,
        funzioniPrimaDopoEventi);
}

function agroDialogTreeViewFilter_onAfterOkClick() {
    AnagraficaRiassunto(true);
}

function generaAlberoAnagraficaSelect(e) {
    var idToPass = $("#GIS_treeview").data("kendoTreeView").dataItem(e.node).id;
    selezioneDaIdAlbero(idToPass, false);
}

var generaAlberoAnagraficaCheck_EventoSimulato = false;

function generaAlberoAnagraficaCheck(e) {

    var checkbox = $(e.node).find(":checkbox");
    var checked = checkbox.prop("checked");

    if (checked) {
        var getitem = $("#GIS_treeview").data("kendoTreeView").dataItem(e.node)
        var idToPass = getitem.id;

        var treeview = $("#GIS_treeview").data("kendoTreeView");
        var selectitem = treeview.findByUid(getitem.uid);
        treeview.select(selectitem);

        if (!generaAlberoAnagraficaCheck_EventoSimulato) {
            selezioneDaIdAlbero(idToPass, false);
        }

        generaAlberoAnagraficaCheck_EventoSimulato = false;
    }
}
function generaAlberoAnagraficaExpand() {
}

function DialogOnOff(selector) {
}

function AlberoAnagrafica2017letturaPivaSaCod() {

    var lPiva = $('#' + hidden_azienda_ClientID).val();
    var lSaCod = $("#" + hidden_sa_cod_ClientID).val();

    console.log("Richiesta Caricamento Albero 2017 su p.iva: " + lPiva + ", sa_Cod: " + lSaCod);

    var cfg = JSON.parse(CaricaConfigurazioneAlberoAnagrafica());
    cfg.Piva = lPiva;
    cfg.Sa_Cod = lSaCod;
    SalvaConfigurazioneAlberoAnagrafica(cfg);
}

function AlberoAnagrafica2017lettura() {

    var gisTree = $('#GIS_treeview').data('kendoTreeView');

    if (gisTree !== undefined) {
        gisTree.dataSource.read();
    } else {
        generaAlberoAnagrafica("GIS");
    }
}

function CaricaConfigurazioneAlberoAnagrafica() {
    return $("#" + hdAlberoAnagrafica2017cfg_ClientID).val();
}

function SalvaConfigurazioneAlberoAnagrafica(cfg) {
    $("#" + hdAlberoAnagrafica2017cfg_ClientID).val(JSON.stringify(cfg))
}

function OggettiGiasInizializza() {
    //GeneraKendoWindow("#albero", "Seleziona", "77%", "66%");
    generaAlberoAnagrafica("GIS");

    if (RichiediPercorsiInizializza) {
        PercorsiInizializza();
    }
}

function OggettiGiasAttivaDisattiva() {
    ApriWindow("#albero");
}

function GetNodesAlberoAnagrafe(options) {

    //ad ogni lettura aggiorno Piva, Sa_cod in oggetto cfgSerialized
    AlberoAnagrafica2017letturaPivaSaCod();

    var cfgSerialized = CaricaConfigurazioneAlberoAnagrafica();
    var id = "0";

    //var lSaCod = $("#" + hidden_sa_cod_ClientID).val();
    //if (lSaCod == "0") {
    //    var idle = [{ id: "a", text: "Nessuna Selezione", items: [] }];
    //    options.success(idle);
    //    WaitFrame.hide();
    //    return;
    //}

    ajaxAgronica(pathCoreWS + "AgronicaControlli_2010/AlberoAnagrafica2017.aspx/GetNodesAlberoAnagrafe",
        "{ cfgSerialized: '" + cfgSerialized + "', id: '" + id + "', PathRoot: '', objP_server: '" + objP_server + "', objP_utenti: '" + objP_utenti + "' }",
        function (risposta) {
            //console.log(risposta.RispostaStringa);

            var hDati = JSON.parse(risposta.RispostaStringa);
            options.success(hDati);

        }, null);

}