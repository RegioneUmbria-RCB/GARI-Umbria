var tipoPesoRiepilogo = 0;
var resxRiepilogoPesiUC = [];

//DOCUMENT READY
$(document).ready(function () {

    $.logThis("RiepilogoPesiUC_jQueryDocReady: INIZIO");

    if (resxObj !== null && resxObj !== undefined) {
        resxRiepilogoPesiUC = resxObj;
    }
    else {
        resxRiepilogoPesiUC.push(readResxFile("GestioneContabilita/App_LocalResources/DocContabile.aspx.resx"));
        resxRiepilogoPesiUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx"));
    }

    //KENDO NumericTextBox***********************************
    $("#inPesoTotaleRiepilogo").kendoNumericTextBox({ decimals: 0, format: "n0", value: 0, change: inPesoTotaleRiepilogo_change });   //TxtPeso_totale
    $("#inTaraVeicoloRiepilogo").kendoNumericTextBox({ decimals: 0, format: "n0", value: 0, change: inTaraVeicoloRiepilogo_change }); //TxtTara_Veicolo
    $("#inImballiVuotiRiepilogo").kendoNumericTextBox({ decimals: 0, format: "n0", value: 0, change: riepilogoPesi_change });
    $("#inPesoLordoRiepilogo").kendoNumericTextBox({ decimals: 0, format: "n0", value: 0, change: riepilogoPesi_change });            //TxtPeso_Lordo
    $("#inTaraImballiRiepilogo").kendoNumericTextBox({ decimals: 0, format: "n0", value: 0, change: riepilogoPesi_change });          //TxtTaraImballi
    $("#inPesoNettoRiepilogo").kendoNumericTextBox({ decimals: 0, format: "n0", value: 0, change: riepilogoPesi_change });            //TxtPeso_Netto
    //$("#inDegradoRiepilogo").kendoNumericTextBox({ decimals: 0, format: "n0", value: 0, change: riepilogoPesi_change });            //TxtVariazione_Percentuale
    $("#inValoreDegradoRiepilogo").kendoNumericTextBox({ min: 0, decimals: 0, format: "n0", value: 0, change: riepilogoPesi_change });//TxtVariazione
    $("#inPesoPagamentoRiepilogo").kendoNumericTextBox({ decimals: 0, format: "n0", value: 0, change: riepilogoPesi_change });        //Peso_Effettivo
    //*******************************************************

    $.logThis("RiepilogoPesiUC_jQueryDocReady: FINE");

});
