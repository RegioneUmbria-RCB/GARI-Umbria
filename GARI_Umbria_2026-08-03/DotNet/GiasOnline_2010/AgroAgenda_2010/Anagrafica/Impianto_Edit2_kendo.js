
function AnagCodiciDistinteValCodData() {
    return [];
}

//#Region template per colonne con combo
function AnagCodiciDistinteValCodTemplate(container, options) {

    $('<input required data-text-field="val_cod_des" data-value-field="val_cod" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "contains",
            dataSource: AnagCodiciDistinteValCodData(),
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#tabCodici").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.val_cod = dataItem.val_cod;                

                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}