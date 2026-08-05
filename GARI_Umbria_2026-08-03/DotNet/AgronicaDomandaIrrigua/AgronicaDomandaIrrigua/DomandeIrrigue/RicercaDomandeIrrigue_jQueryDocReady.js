var CurrentDate = new Date();
var indirizzohttp = "./DomandaIrrigua_WS.aspx";
var Elenco_Aziende = [];
var minYear = new Date().getFullYear() - 10;
var maxYear = new Date().getFullYear() + 10;

$(document).ready(function () {
    $.logThis("DocReady: INIZIO");

    creaKendoMultiselect("cmbAziende", { read: LeggiAziende }, "rag_soc", "piva");
    $('#txtDaAnno').kendoNumericTextBox({ spinners: false, format: "##", min: minYear, max: maxYear });
    $('#txtAAnno').kendoNumericTextBox({ spinners: false, format: "##", min: minYear, max: maxYear });

    $('#txtDaAnno').data("kendoNumericTextBox").bind('change', onChange_DaAnno);
    $('#txtAAnno').data("kendoNumericTextBox").bind('change', onChange_AAnno);

    //---- postback ----
    if ($(cIdDatiRicerca).val() !== "") {

        $('#txtDaAnno').data("kendoNumericTextBox").value($(cIdDaAnno).val());
        $('#txtAAnno').data("kendoNumericTextBox").value($(cIdAAnno).val());
        KendoMultisel("cmbAziende").value(JSON.parse( $(cIdElencoPiva).val()));
        LeggiElencoDomanderrigue()
    } else {
        $('#txtDaAnno').data("kendoNumericTextBox").value(new Date().getFullYear());
    }

    $.logThis("DocReady: FINE");
});