
/* StampaPassaporto.js  */


function passaportiVivaiStampanti_Data(options) {

    var parametriChiamata = { piva: pivaVivaistaRiferimento };

    ajaxAgronica(indirizzohttp + "/Stampanti",
        JSON.stringify(parametriChiamata),
        function(risposta) {
            var p = JSON.parse(risposta.RispostaStringa);
            options.success(p);
        },
        null);
}

function passaportiVivaiLingue_Data(options) {

    var parametriChiamata = { piva: pivaVivaistaRiferimento };

    ajaxAgronica(indirizzohttp + "/Lingue",
        JSON.stringify(parametriChiamata),
        function(risposta) {
            var p = JSON.parse(risposta.RispostaStringa);
            options.success(p);
        },
        null);
    }

function passaportiVivaiStampanti_Template(container, options) {

    $('<input required data-text-field="Nome_Per_Stampa" data-value-field="FF_Stampanti_Cod" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: { transport: { read: passaportiVivaiStampanti_Data} },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#kendoRegistroPassaportiStampa").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.FF_Stampanti_Cod = dataItem.FF_Stampanti_Cod;
                model.FF_Stampanti_Des = dataItem.Nome_Per_Stampa;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

    }

function passaportiVivaiLingue_Template(container, options) {

    $('<input required data-text-field="Lingua_Des" data-value-field="Lingua_Cod" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            filter: "startswith",
            dataSource: { transport: { read: passaportiVivaiLingue_Data} },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                var dataItem = e.sender.dataItem();
                var grid = $("#kendoRegistroPassaportiStampa").data("kendoGrid"),
                    model = grid.dataItem(this.element.closest("tr"));

                model.Lingua_Cod = dataItem.Lingua_Cod;
                model.Lingua_Des = dataItem.Lingua_Des;


                //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

            }

        });

}