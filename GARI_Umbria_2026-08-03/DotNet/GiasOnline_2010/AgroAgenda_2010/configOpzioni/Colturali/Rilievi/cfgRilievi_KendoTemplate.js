


//#Region template per colonne con combo
function MisureUdm_Template(container, options) {

    $('<input required data-text-field="Udm_Des" data-value-field="Udm_Cod" data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        autoBind: true,
        filter: "startswith",
        dataSource: MisureUdm_Leggi(),
        open: kendoDropDownAdjustWidth,
        dataBound: kendoDropDownAdjustWidth,
        change: function (e) {

            // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
            var dataItem = e.sender.dataItem();
            var grid = $("#divCfgRilievi").data("kendoGrid"),
                model = grid.dataItem(this.element.closest("tr"));

            model.Udm_Cod = dataItem.Udm_Cod;
            model.Udm_Des = dataItem.Udm_Des;


            //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

        }

    });

}

function MisureSpecie_Template(container, options) {

    $('<input required data-text-field="Veg_Des" data-value-field="Veg_Cod" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: MisureSpecie_Leggi(),
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#divCfgRilievi").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.Veg_Cod = dataItem.Veg_Cod;
                model.Veg_Des = dataItem.Veg_Des;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}

function MisureAvversita_Template(container, options) {

    $('<input required data-text-field="Av_Des_Vol" data-value-field="Av_Cod" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: MisureAvversita_Leggi(),
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#divCfgRilievi").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.Av_Cod = dataItem.Av_Cod;
                model.Av_Des_Vol = dataItem.Av_Des_Vol;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}

function MisureFF_Template(container, options) {

    
    var grid = $("#divCfgRilievi").data("kendoGrid");
    var model = grid.dataItem(container.closest("tr"));


    $('<input required data-text-field="FF_Des" data-value-field="FF_Cod" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: MisureFF_Leggi(model.Veg_Cod),
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                model.FF_Cod = dataItem.FF_Cod;
                model.FF_Des = dataItem.FF_Des;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}
function MisureUdm_Leggi() {

    var jsonParsed = JSON.parse( $("#hdAnagrafiche").val() );

    return jsonParsed.UnitaMisura;

}


function MisureSpecie_Leggi() {

    var jsonParsed = JSON.parse($("#hdAnagrafiche").val());

    return jsonParsed.Specie;

}


function MisureAvversita_Leggi() {

    var jsonParsed = JSON.parse($("#hdAnagrafiche").val());

    return jsonParsed.Avversita;

}


function MisureFF_Leggi(vegCod) {

    var jsonParsed = JSON.parse($("#hdAnagrafiche").val());

    var rval = [];

    if (jsonParsed !== null && Array.isArray(jsonParsed.FasiFenologiche)) {
        rval = jsonParsed.FasiFenologiche.filter(function (x) { return (x.Veg_Cod == vegCod) });
    }

    rval.splice(0, 0, { FF_Cod: 0, FF_Des: "Non Specificata", Veg_Cod: 0})
    return rval;

}