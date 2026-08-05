

var url_MenuBS_WS = "../Menu/MenuBS_WS.aspx";
var indirizzohttp = "./DuplicaOperazione.aspx";


function KendoImpianti_leggi(options) {

    var sParsed_Kendo = $(id_hdKendo_Impianti).val();

    if (sParsed_Kendo != "") {

        var jSonParsed_Kendo = JSON.parse(sParsed_Kendo);

        if (options !== undefined) {
            options.success(jSonParsed_Kendo.kendo_rows);
        }
        //else {
        KendoImpianti_campiKendoModel = jSonParsed_Kendo.kendo_model;
        KendoImpianti_colonneKendoGrid = jSonParsed_Kendo.kendo_columns;
        //}

    }

}

function KendoOperazioni_leggi(options) {

    var filtro = {
        TipoGriglia: "2",
        xFiltroAggiuntivo_colturali: "agenda.id_agenda in (" + ids + ")",
        txt_Data1: "",
        txt_Data2: "",
        mode: "",
        flag_TerrenoNudo: false
    };

    ajaxAgronicaSync(
        url_MenuBS_WS + "/CaricaOperazioni", "{ filtro: '" + JSON.stringify(filtro) + "' }",
        true,
        function (risposta) {

            var jSonParsed_Kendo = JSON.parse(risposta.RispostaStringa);


            jSonParsed_Kendo.kendo_rows.forEach(function (ind) {
                if (ind.Raccoglitore_Cod != "0") {
                    //Mostro la colonna cod_raccoglitore
                    let col_raccoglitore = jSonParsed_Kendo.kendo_columns.find((o) => { return o["field"] === "Raccoglitore_Cod" })
                    col_raccoglitore.hidden = false
                }
            }, this);

            if (options !== undefined) {
                options.success(jSonParsed_Kendo.kendo_rows);
            } else {
                KendoOperazioni_campiKendoModel = jSonParsed_Kendo.kendo_model;
                KendoOperazioni_colonneKendoGrid = jSonParsed_Kendo.kendo_columns;
            }

        }, null);
}

