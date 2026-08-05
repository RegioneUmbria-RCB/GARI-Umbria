function LeggiConfigurazioneModulo() {
    if (Preparazione_Cod_Conferimento_LottiUC != undefined &&
        Preparazione_Cod_Conferimento_LottiUC != null &&
        Preparazione_Cod_Conferimento_LottiUC > 0) {
        var param = "{ piva: '" + piva + "', generazioneModulo: " + Preparazione_Cod_Conferimento_LottiUC + " }";
        ajaxAgronicaSync(indirizzohttp + "/LeggiConfigurazioneModulo",
            param,
            false,
            function (risposta) {
                var configurazione = JSON.parse(risposta.RispostaStringa);
                var separatore = Separatori_Conferimento_LottiUC.find(val => val.Separatore_Config_Des == configurazione.separatore);
                $(hfData).val(configurazione.parametriScelti);

                if (separatore == undefined || separatore == null) {
                    Separatore_Config_Conferimento_LottiUC = 0;
                }
                else {
                    Separatore_Config_Conferimento_LottiUC = separatore.Separatore_Config;
                }
                Set_KendoDDLValue("ddlSeparatoreParametri_Conferimento_LottiUC", Separatore_Config_Conferimento_LottiUC);
            },
            null);
    }
}

function LeggiTipologieXConferimento_LottiUC(options) {

    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/LeggiTipologieConferimento",
        param,
        false,
        function (risposta) {
            risultato_lettura_FF = JSON.parse(risposta.RispostaStringa);
            if (risultato_lettura_FF.length == 1) {
                Preparazione_Cod_Conferimento_LottiUC = risultato_lettura_FF[0].Modulo_Cod;
                LeggiConfigurazioneModulo();
            }
            else if (risultato_lettura_FF.length > 1) {
                var objVuoto = {
                    "Modulo_Des": "",
                    "Modulo_Cod": 0
                };
                risultato_lettura_FF.unshift(objVuoto);
                Preparazione_Cod_Conferimento_LottiUC = objVuoto.Modulo_Cod;
                Separatore_Config_Conferimento_LottiUC = 0;
            }
            options.success(risultato_lettura_FF);
        },
        null);

}

function LeggiParametriXConferimento_LottiUC(options) {
    return ParametriLotto_Conferimento_LottiUC;
}

function LeggiSeparatoreParametriXConferimento_LottiUC(options) {
    options.success(Separatori_Conferimento_LottiUC);
}

function ddlTipologia_Conferimento_LottiUC_change(options) {
    Preparazione_Cod_Conferimento_LottiUC = parseInt(Get_KendoDDLValue("ddlTipologia_Conferimento_LottiUC"));
    LeggiConfigurazioneModulo();
    creaKendoMultiselectParametriSceltiLottoList();
    abilitaDisabilitaModifica();
}

function ddlSeparatoreParametri_Conferimento_LottiUC_change(options) {
    Separatore_Config_Conferimento_LottiUC = parseInt(Get_KendoDDLValue("ddlSeparatoreParametri_Conferimento_LottiUC"));
    abilitaDisabilitaModifica();
}

function OnRemovelstParametriLottoScelti_Conferimento_Lotti(e) {
    creaKendoMultiselectParametriLottoList();
}

function Salva_Configurazione_Lotto() {
    let ddlTipologia_Lavorazione = $("#ddlTipologia_Conferimento_LottiUC").data("kendoDropDownList");

    if (ddlTipologia_Lavorazione === undefined || ddlTipologia_Lavorazione === null || ddlTipologia_Lavorazione === "")
        return;

    let listboxParametriLottoScelti = $("#lstParametriLottoScelti_Conferimento_LottiUC").data("kendoListBox");

    if (listboxParametriLottoScelti === undefined || listboxParametriLottoScelti === null || listboxParametriLottoScelti === "")
        return;

    let ddlSeparatore = $("#ddlSeparatoreParametri_Conferimento_LottiUC").data("kendoDropDownList");

    if (ddlSeparatore === undefined || ddlSeparatore === null || ddlSeparatore === "")
        return;

    console.log("Separatore")
    let generazioneModulo = parseInt(Get_KendoDDLValue("ddlTipologia_Conferimento_LottiUC"));

    let ArrayParametriScelti = [];

    for (var x = 0; x < listboxParametriLottoScelti.items().length; x++) {
        ArrayParametriScelti.push(listboxParametriLottoScelti.dataItem(listboxParametriLottoScelti.items()[x]).Algoritmo_Config);
    }

    let separatore = parseInt(Get_KendoDDLValue("ddlSeparatoreParametri_Conferimento_LottiUC"));

    let Msg_Errore = Controlli_Prima_Di_Salva_Configurazione_Lotto(ArrayParametriScelti, generazioneModulo);

    if (Msg_Errore === "") {
        var separatoreDesc = Separatori_Conferimento_LottiUC.find(val => val.Separatore_Config == separatore);
        console.log("Separatore: [" + separatoreDesc.Separatore_Config_Des + "]")
        var parametriScelti = JSON.stringify(ArrayParametriScelti);
        var param = "{ piva: '" + piva + "', generazioneModulo: " + generazioneModulo + ", separatore: '" + separatoreDesc.Separatore_Config_Des + "', parametriScelti: " + parametriScelti + " }";
        console.log(param);
        ajaxAgronicaSync(indirizzohttp + "/SalvaConfigurazioneLotto",
            param,
            false,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            },
            null);
    } else {
        MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");
    }

}