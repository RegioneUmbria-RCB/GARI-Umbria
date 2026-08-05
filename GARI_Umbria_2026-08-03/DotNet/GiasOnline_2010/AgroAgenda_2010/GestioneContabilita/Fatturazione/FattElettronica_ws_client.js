var eseguiFatturazioneElettronica = true;
var indirizzohttp = "./FattElettronica.aspx";   

function RicercaFatture(options) {
    var piva = KendoDDL("ddlAziende").value();
    var param = "{ piva: '" + piva +
        "', dataDal: '" + $('input[name$="txt_DataDal"]').val() +
        "', dataAl: '" + $('input[name$="txt_DataAl"]').val() +
        "', tipoDoc: '" + $('input[name$="groupTipoDoc"]:checked').val() +
        "', filtroLav: '" + $('input[name$="groupFiltroLav"]:checked').val() + "'}";

    ajaxAgronica(indirizzohttp + "/ElencoFatture",
        param,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function LogFattura(idAgenda) {
    var piva = KendoDDL("ddlAziende").value();
    var param = kendo.stringify({ piva: piva, idAgenda: idAgenda });

    ajaxAgronica(indirizzohttp + "/LogFattura",
        param,
        function (risposta) {
            let rows = JSON.parse(risposta.RispostaStringa);
            if (rows.length > 0) {
                var log = "<h4>Log Fatturazione</h4><table class='table table-bordered'>";
                log += "<tr class='active'><th>Data</th><th>Log</th></tr>";
                $.each(rows, function () {
                    log += "<tr><td>" + kendo.toString(kendo.parseDate(this.DataLog), "dd/MM/yyyy HH:mm:ss") + "</td><td>" + this.Messaggio + "</td></tr>";
                });
                log += "</table>";
                $("#log_fatturazione").html(log);
            }
        }, null);
}

function GenerazioneXML() {
    FattElettronicaService("GenerazioneXML");
}

function InvioXMLAttivi() {
    var kendoConfirm = $("<div></div>").kendoConfirm({
        title: "Fatturazione Elettronica",
        messages: { okText: "Sì", cancel: "No" },
        content: "Vuoi inviare i file XML della fatturazione elettronica?"
    }).data("kendoConfirm");
    kendoConfirm.result.done(function () { FattElettronicaService("InvioXMLAttivi"); });
    kendoConfirm.open(); 
}

function VerificaEsiti() {
    FattElettronicaService("VerificaEsiti");
}

function RiceviXMLPassivi() {
    FattElettronicaService("RiceviXMLPassivi");
}

function FattElettronicaService(servizio) {
    var piva = KendoDDL("ddlAziende").value();
    if (piva !== "") {
        // console.log(servizio);
        ajaxAgronica(indirizzohttp + "/" + servizio,
            kendo.stringify({ piva: piva }),
            function (risposta) {
                popolaGrigliaFattElettronica();
                kendo.alert(risposta.RispostaStringa);
            }, null);
    } else {
        kendo.alert("Azienda non selezionata");
    }
}

function ElencoImprese(options) {
    ajaxAgronica(indirizzohttp + "/ElencoImprese",
        "{ }",
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
            LeggiConfigurazione();
            popolaGrigliaFattElettronica();
            // kendo.alert(risposta.RispostaStringa);
        }, null, null, false);
}

function LeggiConfigurazione() {
    var piva = KendoDDL("ddlAziende").value();
    $("#servizi_fatturazione").html("");
    if (piva === "") {
        $("#servizi_fatturazione").html("<h4>Servizio non disponibile per l'azienda selezionata</h4>");
    } else {
        ajaxAgronica(indirizzohttp + "/LeggiConfigurazione",
            kendo.stringify({ piva: piva }),
            function (risposta) {
                var risp = risposta.RispostaStringa;
                if (risp === "") {
                    $("#servizi_fatturazione").html("<h4>Servizio non disponibile per l'azienda selezionata</h4>");
                } else {
                    $("#servizi_fatturazione").html(risp);
                }
            }, null);
    }    
}

function EsportazioneXML() {
    var piva = KendoDDL("ddlAziende").value();
    var grid = $("#tab_fatt_elettronica").data("kendoGrid");
    var files = [];
    var items = grid.dataSource.data().filter(function (dataitem) { return dataitem.Selected == true });
    items.forEach(function (item) {
        if (item.NomeFileXML !== "") {
            files.push(item.IdLog+"|"+item.NomeFileXML);
        }
    });
    if (files.length > 0) {
        ajaxAgronica(indirizzohttp + "/EsportazioneXML",
            "{ piva: '" + piva + "', files: '" + JSON.stringify(files) + "' }",
            function (risposta) {
                popolaGrigliaFattElettronica();
                kendo.alert(risposta.RispostaStringa);
                if (risposta.ParametroDue) {
                    let response = JSON.parse(risposta.ParametroDue_stringa);
                    let fileDati = response.File;
                    let nomeFile = response.NomeFile;
                    let estensione = response.Estensione;

                    SaveAndOpenFileByteArray(nomeFile, fileDati, estensione); 

                    //$("#iframe").attr("src", indirizzohttp + "?p=" + piva + "&d=" + risposta.ParametroDue_stringa);
                    //$("#iframe").load();
                }
            }, null);
    } else {
        kendo.alert("Selezionare i file XML da esportare");
    }
}

function RigenerazioneXML() {
    var piva = $("#ddlAziende").data("kendoDropDownList").value();
    var grid = $("#tab_fatt_elettronica").data("kendoGrid");
    var files = [];
    var items = grid.dataSource.data().filter(function (dataitem) { return dataitem.Selected == true });
    items.forEach(function (item) {
        if (item.NomeFileXML !== "") {
            files.push(item.IdLog + "|" + item.NomeFileXML);
        }
    });
    if (files.length > 0) {
        ajaxAgronica(indirizzohttp + "/RigenerazioneXML",
            "{ piva: '" + piva + "', files: '" + JSON.stringify(files) + "' }",
            function (risposta) {
                popolaGrigliaFattElettronica();
                kendo.alert(risposta.RispostaStringa);
            }, null);
    } else {
        kendo.alert("Selezionare i file XML da rigenerare");
    }
}

function ScaricaXMLPassivi() {
    var piva = $("#ddlAziende").data("kendoDropDownList").value();
    ajaxAgronica(indirizzohttp + "/ScaricaXMLPassivi",
        "{ piva: '" + piva + 
        "', dataDal: '" + $('input[name$="txt_DataDal"]').val() +
        "', dataAl: '" + $('input[name$="txt_DataAl"]').val() + "' }",
        function (risposta) {
            kendo.alert(risposta.RispostaStringa);
            if (risposta.ParametroDue) {
                let response = JSON.parse(risposta.ParametroDue_stringa);
                let fileDati = response.File;
                let nomeFile = response.NomeFile;
                let estensione = response.Estensione;

                SaveAndOpenFileByteArray(nomeFile, fileDati, estensione); 

                //$("#iframe").attr("src", indirizzohttp + "?d=" + risposta.ParametroDue_stringa);
                //$("#iframe").load();
            }
        }, null);
}