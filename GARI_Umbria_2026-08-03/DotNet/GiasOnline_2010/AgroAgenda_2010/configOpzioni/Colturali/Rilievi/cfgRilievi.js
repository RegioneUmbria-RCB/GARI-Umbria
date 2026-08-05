

function valoriSelezioneAttiva(rigaSelezionata) {

    console.log(rigaSelezionata);

    var grid = $("#divCfgRilievi").data("kendoGrid"),
        model = grid.dataItem(rigaSelezionata);

    $(id_lblmTabellaDettagli).html("Valori Per selezione guidata: " + model.Av_Des_Vol + " - " + model.Udm_Des + " - " + model.FF_Des);

    valoriSelezioneAttivaCarica(model.COD)
}

/**
 * carica i dettagli di anagrafica per la selezione attiva
 * @param {string} cod codice di misuraXavversita
 */
function valoriSelezioneAttivaCarica(cod) {

    $("#hdCurrentCOD").val(cod);

    $('#mTabellaDettagli').modal('show');

    letturaDaticfgRilieviAnagKendo(cod);


}