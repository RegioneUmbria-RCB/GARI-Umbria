var CurrentDate = new Date();
var indirizzohttp = "./LettureContatoriAziendali_WS.aspx";
var writeRule = false;
var oldAnno = 0;

$(document).ready(function () {
    $.logThis("DocReady: INIZIO");
    $('#txtAnno').kendoNumericTextBox({ spinners: false, format: "#", decimals: 0 });
    $('#txtAnno').data("kendoNumericTextBox").bind('change', onChange_Anno);
    $("#txtAnno").data("kendoNumericTextBox").value(new Date().getFullYear());
    $("#txtAnno").data("kendoNumericTextBox").trigger('change');
    writeRule = $(cIdUtenteAbilitatoScrittura).val();
    InizializzaVideata();
    

    $.logThis("DocReady: FINE");
});

function onChange_Anno(e) {
    if ($("#divKendoGridLetture").data("kendoGrid") == null) {
        oldAnno = this.value();
        return;
    }
    var data = $("#divKendoGridLetture").data("kendoGrid").dataSource.data();
    var chk = false;
    for (var i = 0; i < data.length; i++) {
        if (data[i].dirty === true || data[i].id===-1) {
            chk = true;
            break;
        }
    }
    if (chk === true) {
        kendo.alert("<div>Attenzione salvare le modifiche prima di cambiare anno!!!</div>");
        this.value(oldAnno);
    } else {
        oldAnno = this.value();
    }
    
}