var indirizzohttp = "./MenuG2G.aspx";

//function popolaConfigurazioni() {
//    $("#" + ddlConfigurazioni_ClientID).kendoDropDownList({
//        filter: "contains",
//        dataSource: { transport: { read: ElencoConfigurazioni } },
//        dataTextField: "descrizione",
//        dataValueField: "codice",
//        change: function (e) {
//            var dataItem = e.sender.dataItem();
//            LeggiConfigurazione();
//        } // ,value: $(cIdPiva).val()
//    });
//}

function popolaConfigurazioni() {
    $("#" + ddlConfigurazioni_ClientID).kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: ElencoConfigurazioni } },
        dataTextField: "descrizione",
        dataValueField: "codice"
    });
}

function elaboraLog() {

    var log = $("#txtLog").val();
    var piva = "";
    var logErrori = "";
    var pivaErrori = "";
    var logImprese = "";
    var numImprese = 0;
    var lines = log.split('\n');

    for (var line = 0; line < lines.length; line++) {
        var errore = lines[line].toLowerCase().indexOf("error") != -1;
        if (lines[line].indexOf("----- Impresa") != -1) {
            if (logImprese.toLowerCase().indexOf("error") != -1) {
                logErrori += logImprese + "\n";
                if (piva != "") pivaErrori += (pivaErrori != "" ? "," : "") + "\"" + piva + "\"";
                piva = "";
                logImprese = "";
                numImprese++;
            }
            var tokens = lines[line].split(" - ");
            piva = tokens[1];
            logImprese = lines[line] + "\n";
        } else if (errore) {
            logImprese += lines[line] + "\n";
        }
    }

    if (logImprese.toLowerCase().indexOf("error") != -1) {
        logErrori += logImprese + "\n";
        if (piva != "") pivaErrori += (pivaErrori != "" ? ", " : "") + "\"" + piva + "\"";
        numImprese++;
    }

    if (pivaErrori != "") {
        logErrori = numImprese + " Imprese con errori: " + pivaErrori + "\n\n" + logErrori;
    }

    $("#txtLogErrori").val(logErrori);
}