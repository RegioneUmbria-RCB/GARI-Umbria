var deltayear = 5;
var indirizzohttp = "./CalcoloTariffazione_WS.aspx";

function Leggi_Anni(options) {
    var obj = [];
    for (var i = -deltayear; i <= deltayear; i++) {
        let d = new Date().getFullYear() + i;
        obj.push({ "year_cod": d })
    }
    options.success(obj);
}

function ElaboraDati() {
    let param = kendo.stringify({
        "data": kendo.stringify({
            "Anno": KendoDDL('ddlYear').value(),
            "spesatotale": $('#txtImportoTotale').data('kendoNumericTextBox').value(),
            "spesaquotafissa": $('#txtBaseImp').data('kendoNumericTextBox').value(),
            "incrementofascia2": $('#txtDeltaImportoT2').data('kendoNumericTextBox').value(),
            "incrementofascia3": $('#txtDeltaImportoT3').data('kendoNumericTextBox').value()
        })
    });

    ajaxAgronicaSync(indirizzohttp + "/CalcolaTariffazione",
        param,
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            $(cIdDatiCalcoloTariffazione).val(risp);
            if ($(cIdDatiCalcoloTariffazione).val() !== "") {
                popolaGrigliaRisultati("divKendoOutput");
                ShowTab(1, true);
            }
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
            $(cIdDatiCalcoloTariffazione).val("");
        });
}
