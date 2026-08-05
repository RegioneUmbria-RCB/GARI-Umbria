

/* RegCaricoScaricoPassaportiKendo.js */



/**
 * Genera nuovo controllo kendo windows
 * @param {any} selector
 * @param {any} titolo
 * @param {any} widthPercent
 * @param {any} heigthPercent
 * @param {any} functionPin
 * @param {any} functionClose
 * @param {any} functionOpen
 * @param {any} functionResize
 * @param {any} actions
 */
function GeneraKendoWindowMaps(selector, titolo, widthPercent, heigthPercent, functionPin, functionClose, functionOpen, functionResize, actions) {

    let acts;
    if (!actions) {
        acts = [
            "Pin",
            "Minimize",
            "Maximize",
            "Close"
        ];
    } else {
        acts = actions;
    }

    $(selector).kendoWindow({
        width: widthPercent,
        height: heigthPercent,
        title: titolo,
        visible: false,
        close: functionClose,
        open: functionOpen,
        resize: functionResize,
        pin: functionPin,
        actions: acts
    }).data("kendoWindow");

}



function ApriWindow(selector, center) {
    if (center) {
        $(selector).data("kendoWindow").center().open();
    } else {
        $(selector).data("kendoWindow").open();
    }

}


/**
 * Apre una kendo window con all'interno la pagina iframe richiesta dal parametro indirizzo
 * @param {any} indirizzo
 * @param {any} descrizione
 * @param {any} functionOnClose
 * @param {any} width
 * @param {any} height
 * @param {any} left
 * @param {any} top
 */
function KendoWindowGenericApri(indirizzo, descrizione, functionOnClose, width, height, left, top) {


    $("#kendoWindowPaginaGeneric").attr("src", "about:blank");
    $("#kendoWindowPaginaGeneric").attr("src", indirizzo);

    $("#kendoWindowiFrameGeneric").data("kendoWindow").title(descrizione);

    if (width !== undefined) {
        $("#kendoWindowiFrameGeneric").data("kendoWindow").setOptions({
            width: width
        });
    }


    if (height !== undefined) {
        $("#kendoWindowiFrameGeneric").data("kendoWindow").setOptions({
            height: height
        });
    }

    $("#kendoWindowiFrameGeneric").data("kendoWindow").setOptions({
        position: {
            top: top, // or "100px"
            left: left
        }
    });


    ApriWindow("#kendoWindowiFrameGeneric");


}


function passaportiVivaiTipoZona_Template_DataSource() {

    return [
        {
            TipoZona: "PP", TipoZonaDes: TraduzioneMultiResx(resxObj, "PassaportoZonaPP", "Zona Non protetta (PP)")
        },
        {
            TipoZona: "ZP", TipoZonaDes: TraduzioneMultiResx(resxObj, "PassaportoZonaZP", "Zona Protetta (ZP)")
        }];
}


//#Region template per colonne con combo
function passaportiVivaiTipoZona_Template(container, options) {

    $('<input required data-text-field="TipoZonaDes" data-value-field="TipoZona" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: passaportiVivaiTipoZona_Template_DataSource(),
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#kendoRegistroPassaporti").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.TipoZona = dataItem.TipoZona;
                model.TipoZonaDes = dataItem.TipoZonaDes;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}

var passaportiVivaiNomeBotanico_Template_DataSourceArr;

function passaportiVivaiNomeBotanico_Template_DataSource() {

    if (!passaportiVivaiNomeBotanico_Template_DataSourceArr) {

        ajaxAgronicaSync(indirizzohttp + "/ListaSpecieCultivar", JSON.stringify({}), false,
            function (risposta) {
                passaportiVivaiNomeBotanico_Template_DataSourceArr = JSON.parse(risposta.RispostaStringa);
            }, null);
    }

    return passaportiVivaiNomeBotanico_Template_DataSourceArr;
}

function passaportiVivaiNomeBotanico_Template(container, options) {

    $('<input required data-text-field="Veg_Des_Lat" data-value-field="Cul_COD" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: passaportiVivaiNomeBotanico_Template_DataSource(),
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#kendoRegistroPassaporti").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.Cul_COD = dataItem.Cul_COD;
                model.Veg_Des_Lat = dataItem.Veg_Des_Lat;
                model.cul_Des = dataItem.Cultivar;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}

function passaportiVivaiCaricoScarico_Template_DataSource() {
    return [{
        CaricoScarico: "Transport",
        CaricoScaricoDes: "Transport"
    },
    {
        CaricoScarico: "Loading",
        CaricoScaricoDes: "Loading"
    },
    {
        CaricoScarico: "Discharge",
        CaricoScaricoDes: "Discharge"
    },
    {
        CaricoScarico: "Delivery",
        CaricoScaricoDes: "Delivery"
    }];
}

function passaportiVivaiCaricoScarico_Template(container, options) {

    $('<input required data-text-field="CaricoScaricoDes" data-value-field="CaricoScarico" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: passaportiVivaiCaricoScarico_Template_DataSource(),
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#kendoRegistroPassaporti").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.CaricoScarico = dataItem.CaricoScarico;
                model.CaricoScaricoDes = dataItem.CaricoScaricoDes;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}

function passaportiVivaiCausale_Template_DataSource() {
    return [{
        Causale: "Bin Transfer",
        CausaleDes: "Bin Transfer"
    },
    {
        Causale: "Bud Processing",
        CausaleDes: "Bud Processing"
    },
    {
        Causale: "Bud Transfer",
        CausaleDes: "Bud Transfer"
    },
    {
        Causale: "Bud Allocation",
        CausaleDes: "Bud Allocation"
    },
    {
        Causale: "WH Load",
        CausaleDes: "WH Load"
    }];
}

function passaportiVivaiCausale_Template(container, options) {

    $('<input required data-text-field="CausaleDes" data-value-field="Causale" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: passaportiVivaiCausale_Template_DataSource(),
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#kendoRegistroPassaporti").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.Causale = dataItem.Causale;
                model.CausaleDes = dataItem.CausaleDes;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}