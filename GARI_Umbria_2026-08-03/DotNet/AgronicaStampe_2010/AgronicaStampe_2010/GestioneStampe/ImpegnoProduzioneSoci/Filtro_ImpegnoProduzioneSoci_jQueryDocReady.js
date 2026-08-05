//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31),
        format: "d"
    });

    let oggi = new Date();
    let anno = oggi.getFullYear();
    set_data("dpValiditaInizio", new Date(anno, 0, 1), null);

    let kddlSocio = creaKendoDropDownList("ddlSocio", { read: ddlSocioRead }, "rag_soc", "piva", null, null, null, false).data("kendoDropDownList");
    kddlSocio.one("dataBound", ddlSocioDataBound);
    kddlSocio.dataSource.read();

    let kddlImpianti = creaKendoDropDownList("ddlImpianti", { read: ddlImpiantiRead }, "Desc", "Value").data("kendoDropDownList");
    kddlImpianti.bind("change", ddlImpiantiChange);
    $("#infoImpiantiAttiviAnno").show();
    $("#infoImpiantiNatiAnno").hide();

    creaKendoDropDownList("ddlTipoArchivio", { read: ddlTipoArchivioRead }, "Desc", "Value");

    $("#btnStampa").on("click", AvviaStampa);

    $.logThis("DocReady: FINE");
});

function ddlSocioRead(options) {
    var companiesList = RicercaImprese(false);//.slice()
    //companiesList.unshift({
    //    piva: "",
    //    rag_soc: ""
    //});
    options.success(companiesList);
}

function ddlSocioDataBound(e) {
    // Pre-seleziono l'eventuale azienda che l'utente aveva scelto dal menu principale
    e.sender.select(function (dataItem) {
        return dataItem.piva === $(cIdPiva).val();
    });
}

function ddlImpiantiRead(options) {
    options.success([
        { Value: 0, Desc: "attivi nell'anno selezionato" },
        { Value: 1, Desc: "nati nell'anno selezionato" },
    ]);
}

function ddlImpiantiChange(e) {
    let val = parseInt(e.sender.value());
    $("#infoImpiantiAttiviAnno").toggle(val === 0);
    $("#infoImpiantiNatiAnno").toggle(val === 1);
}

function ddlTipoArchivioRead(options) {
    options.success([
        { Value: 0, Desc: "Previsionale" },
        { Value: 1, Desc: "Consuntivo" },
    ]);
}

function AvviaStampa() {
    let dataItemSocio = KendoDDL("ddlSocio").dataItem();
    let socioPiva = "";
    let socioRagSoc = "";
    if (dataItemSocio !== undefined && dataItemSocio !== null) {
        socioPiva = dataItemSocio.piva;
        socioRagSoc = dataItemSocio.rag_soc;
    }

    let params = {
        Piva: socioPiva,
        RagSocSocio: socioRagSoc,
        TipoArchivio: Get_KendoDDLValue("ddlTipoArchivio", 0),
        Impianti: Get_KendoDDLValue("ddlImpianti", 0),
        ValiditaInizio: get_data("dpValiditaInizio"),
    };

    let endpoint = "Filtro_ImpegnoProduzioneSoci.aspx/CostruisciLinkStampa";

    ajaxAgronica(
        endpoint,
        kendo.stringify({ params: kendo.stringify(params) }),
        function (risposta) {
            //console.log("ok: " + risposta.RispostaStringa);
            try {
                var rispJson = JSON.parse(risposta.RispostaStringa);

                window.open(
                    rispJson.paginaDaRichiamare + "?" + rispJson.queryString,
                    "_blank"
                );
            } catch (e) {
                // Si verifica errore nel caso in cui RispostaStringa non sia un oggetto json, ma un messaggio, come per esempio "report selezionato dismesso"
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax '" + endpoint + "', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                var jsonArrErrori = JSON.parse(risposta.Errore);
                var stringArrErrori = jsonArrErrori.join("<br/>");
                MessaggioErrore_Bootstrap("Si sono verificati i seguenti errori:<br/>" + stringArrErrori, "DIV_Messaggi");
            }
        }
    );

}