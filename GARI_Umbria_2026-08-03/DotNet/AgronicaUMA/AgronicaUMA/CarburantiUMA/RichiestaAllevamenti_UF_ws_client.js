var indirizzohttp = "./richiestaCarburanti.aspx";
var indirizzohttpWSGenerali = "./CdG_WS.aspx";

function LeggiColtureAllevamentiUF(options) {
    return new Promise(function (resolve, reject) {
        var param = {
            piva: KendoDDL("ddlAzienda").value(),
            richiestaCod: richiesta_cod,
        }
        
        ajaxAgronicaSync(indirizzohttp + "/UC_Allevamenti_UF_LeggiColtureUF",
            JSON.stringify(param), false,
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa)
                options.success(resp);
                resolve();
            }, null);
    });
}

function aggiungiRigheFascicolo() {
    return new Promise(function (resolve, reject) {
        WaitFrame.show();
        try {
            var index = $("#ddlFascicolo_UF").data("kendoDropDownList").selectedIndex;
            var dataItem = $("#ddlFascicolo_UF").data("kendoDropDownList").dataItem(index);
            var programmazione_cod = "";

            if (index != -1) {
                if (isNumeric(dataItem.strProgrammazioniCod.replaceAll("|", "").trim())) {
                    programmazione_cod = parseInt(dataItem.strProgrammazioniCod.replaceAll("|", "").trim())
                }
            } else {
                if ($("#anno").val() >= 2025) {
                    programmazione_cod = -4;
                }
            }
        } catch (e) {

        }

        var param = {
            piva: KendoDDL("ddlAzienda").value(),
            richiestaCod: richiesta_cod,
            programmazione_cod: programmazione_cod
        }

        ajaxAgronicaSync(indirizzohttp + "/UC_Allevamenti_UF_AggiungiRigheFascicolo",
            JSON.stringify(param), false,
            function (risposta) {
                WaitFrame.hide();
                let resp = JSON.parse(risposta.RispostaStringa);
                $("#tab_griglia_UFcolture").data("kendoGrid").refresh();
                $("#tab_griglia_UFcolture").data("kendoGrid").dataSource.read();
                resolve();
            }, null);
    });
}

function cancellaModificheRigheAllevamentiUF(cancellate, modificate) {
    return new Promise(function (resolve, reject) {
        
        var param = {
            piva: KendoDDL("ddlAzienda").value(),
            richiestaCod: richiesta_cod,
            righeCancellate: JSON.stringify(cancellate),
            righeModificate: JSON.stringify(modificate),
        }

        ajaxAgronicaSync(indirizzohttp + "/UC_Allevamenti_UF_EliminaModificaRighe",
            JSON.stringify(param), false,
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve();
            }, null);
    });
}

function ControlloCapiAllevabiliUF() {
    return new Promise(function (resolve, reject) {

        var param = {
            piva: KendoDDL("ddlAzienda").value(),
            richiestaCod: richiesta_cod,
        }

        ajaxAgronicaSync(indirizzohttp + "/UC_Allevamenti_UF_ControlloCapiAllevabili",
            JSON.stringify(param), false,
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve();
            }, null);
    });
}

function SalvaAllevatiInMontagna() {
    return new Promise(function (resolve, reject) {

        var param = {
            richiestaCod: richiesta_cod,
            allevatoMontagna: Boolean(getKendoSwitch("montagnaSwitchCheck"))
        }

        ajaxAgronicaSync(indirizzohttp + "/UC_Allevamenti_UF_SalvaAllevatiInMontagna",
            JSON.stringify(param), false,
            function (risposta) {
                let resp = risposta.RispostaStringa;
                resolve();
            }, null);
    });
}