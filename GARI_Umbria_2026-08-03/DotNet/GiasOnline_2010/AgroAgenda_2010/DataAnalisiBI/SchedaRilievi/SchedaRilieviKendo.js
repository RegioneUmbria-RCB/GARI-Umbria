function letturaDatiRilievi() {

    if ($("#txt_DataDa").val() == "") {
        $("#txt_DataDa").val("01/01/2014");
        $("#txt_DataA").val("01/01/2015");
    }

    var param = "{ DataDa: '" + $("#txt_DataDa").val() + "', DataA: '" + $("#txt_DataA").val() + "', IncludiSoloDatiRilevati: " + $("#chkSelezione").is(":checked") + " }";

    ajaxAgronica(indirizzohttp + "/Ricerca",
        param,
        function (risposta) {

            $("#hidTabellaEsitoRicerca").val(risposta.RispostaStringa);
            kendoRilievi("tabellaEsitoRicerca");

        }, null);
}


function kReadRilievi_rows(options) {

    var data = $('#hidTabellaEsitoRicerca').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadRilievi_col() {

    var data = $('#hidTabellaEsitoRicerca').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadRilievi_mod() {

    var data = $('#hidTabellaEsitoRicerca').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}