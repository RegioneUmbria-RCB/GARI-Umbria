var indirizzohttp = "./UtilityModificaChiavi.aspx";

//###### INIZIO TAB Utility Cambio Piva

function Leggi_Impresa_con_PIVA_cambio_piva_UtilityModificaChiavi(options) {

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Impresa_con_PIVA_cambio_piva_UtilityModificaChiavi",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            let objVuoto = {
                "Piva": "",
                "Rag_Soc_Piva": "SELEZIONA..."
            };
            risp.unshift(objVuoto);

            options.success(risp);
        }, null, undefined, true, undefined);
}


function ControlliPreliminari_cambio_piva_UtilityModificaChiavi() {

    return new Promise((resolve, reject) => {

        let piva_selezionata = Get_KendoDDLValue("ddl_impresa_cambio_piva_UtilityModificaChiavi");

        if (piva_selezionata === undefined || piva_selezionata === null || piva_selezionata === "") {

            let Msg_Errore = "Selezionare l'Impresa a cui si vuole cambiare la Partita IVA.";

            MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

            resolve(null);

        }

        let nuova_piva = $("#txt_nuova_PIVA_cambio_piva_UtilityModificaChiavi").val();

        var param = kendo.stringify(
            {
                Piva_Old: piva_selezionata,
                Piva_New: nuova_piva
            });

        ajaxAgronica(indirizzohttp + "/ControlliPreliminari_cambio_piva_UtilityModificaChiavi",
            param,
            function (risposta) {
                resolve(risposta);
            },
            function (risposta) {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
                resolve(risposta);
            }, true);

    });

}

function Salva_cambio_piva_UtilityModificaChiavi() {

    let piva_selezionata = Get_KendoDDLValue("ddl_impresa_cambio_piva_UtilityModificaChiavi");

    if (piva_selezionata === undefined || piva_selezionata === null || piva_selezionata === "") {

        let Msg_Errore = "Selezionare l'Impresa a cui si vuole cambiare la Partita IVA.";

        MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

        return;

    }

    let nuova_piva = $("#txt_nuova_PIVA_cambio_piva_UtilityModificaChiavi").val();

    var param = kendo.stringify(
        {
            Piva_Old: piva_selezionata,
            Piva_New: nuova_piva
        });

    ajaxAgronica(indirizzohttp + "/Salva_cambio_piva_UtilityModificaChiavi",
        param,
        function (risposta) {
            //Ricarico la ddl delle imprese e la imposto sull'Impresa di cui ho cambiato la Piva
            $("#ddl_impresa_cambio_piva_UtilityModificaChiavi").data("kendoDropDownList").dataSource.read();

            Set_KendoDDLValue("ddl_impresa_cambio_piva_UtilityModificaChiavi", $("#txt_nuova_PIVA_cambio_piva_UtilityModificaChiavi").val());

            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

        }, null, true);

}
//###### FINE TAB Utility Cambio Piva



//###### INIZIO TAB Utility Cambio CF/Piva Contatto

function Leggi_Impresa_con_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi(options) {

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Impresa_con_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            let objVuoto = {
                "PIVA": "",
                "rag_soc": "SELEZIONA..."
            };
            risp.unshift(objVuoto);

            options.success(risp);

        }, null, undefined, true, undefined);
}

function Leggi_Contatto_cambio_CF_piva_contatto_UtilityModificaChiavi(options) {

    let piva_selezionata = Get_KendoDDLValue("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi")

    if (piva_selezionata === undefined || piva_selezionata === null || piva_selezionata === "") {

        let Msg_Errore = "Selezionare l'Impresa a cui si vuole cambiare il Codice Fiscale.";

        MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

        return;

    }


    var param = kendo.stringify(
        {
            Piva: piva_selezionata
        });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Contatto_cambio_CF_piva_contatto_UtilityModificaChiavi",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            let objVuoto = {
                "Cod_Contatto": "",
                "Rag_Soc": "SELEZIONA..."
            };

            risp.unshift(objVuoto);

            options.success(risp);
        }, null, undefined, true, undefined);
}

function ControlliPreliminari_cambio_CF_piva_contatto_UtilityModificaChiavi() {

    return new Promise((resolve, reject) => {

        let piva_selezionata = Get_KendoDDLValue("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi");

        if (piva_selezionata === undefined || piva_selezionata === null || piva_selezionata === "") {

            let Msg_Errore = "Selezionare l'Impresa.";

            MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

            resolve(null);

        }

        let contatto_selezionato = Get_KendoDDLValue("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi");

        if (contatto_selezionato === undefined || contatto_selezionato === null || contatto_selezionato === "") {

            let Msg_Errore = "Selezionare il contatto.";

            MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

            resolve(null);

        }

        let nuovo_cod_contatto = $("#txt_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").val();

        var param = kendo.stringify(
            {
                Piva: piva_selezionata,
                CodContatto_Old: contatto_selezionato,
                CodContatto_New: nuovo_cod_contatto
            });

        ajaxAgronica(indirizzohttp + "/ControlliPreliminari_cambio_CF_piva_contatto_UtilityModificaChiavi",
            param,
            function (risposta) {
                resolve(risposta);
            },
            function (risposta) {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
                resolve(risposta);
            },true);
    });

}

function Salva_cambio_CF_piva_contatto_UtilityModificaChiavi() {

    let piva_selezionata = Get_KendoDDLValue("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi");

    if (piva_selezionata === undefined || piva_selezionata === null || piva_selezionata === "") {

        let Msg_Errore = "Selezionare l'Impresa.";

        MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

        return;

    }

    let contatto_selezionato = Get_KendoDDLValue("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi");

    if (contatto_selezionato === undefined || contatto_selezionato === null || contatto_selezionato === "") {

        let Msg_Errore = "Selezionare il contatto.";

        MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

        return;

    }

    let nuovo_cod_contatto = $("#txt_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").val();

    var param = kendo.stringify(
        {
            Piva: piva_selezionata,
            CodContatto_Old: contatto_selezionato,
            CodContatto_New: nuovo_cod_contatto
        });

    ajaxAgronica(indirizzohttp + "/Salva_cambio_CF_piva_contatto_UtilityModificaChiavi",
        param,
        function (risposta) {
            //Ricarico la ddl dei Contatti e la imposto sull'Utente di cui ho cambiato il Codice Fiscale
            $("#ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").data("kendoDropDownList").dataSource.read();

            Set_KendoDDLValue("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi", nuovo_cod_contatto);

            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

        }, null, true);
}

//###### FINE TAB Utility Cambio CF/Piva Contatto


//###### INIZIO TAB Utility Cambio CF Utente

function Leggi_Username_con_Codice_Fiscale_cambio_CF_utente_UtilityModificaChiavi(options) {

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Username_con_Codice_Fiscale_cambio_CF_utente_UtilityModificaChiavi",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            let objVuoto = {
                "UserName": "",
                "CodFisc": "",
                "UserName_CodFisc": "SELEZIONA..."
            };
            risp.unshift(objVuoto);

            options.success(risp);
        }, null, undefined, true, undefined);
}


function ControlliPreliminari_cambio_CF_utente_UtilityModificaChiavi() {

    return new Promise((resolve, reject) => {


        let utente_selezionato = Get_KendoDDLValue("ddl_utente_cambio_CF_utente_UtilityModificaChiavi");

        let ddl_dataItem = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem();

        let codice_fiscale_selezionato = null;

        let flag_azienda_persona = null;

        if (utente_selezionato === undefined || utente_selezionato === null || utente_selezionato === "" ||
            ddl_dataItem === undefined || ddl_dataItem === null || ddl_dataItem === "" ||
            ddl_dataItem.CodFisc === undefined || ddl_dataItem.CodFisc === null || ddl_dataItem.CodFisc === "" ||
            ddl_dataItem.Flag_Azienda_Persona === undefined || ddl_dataItem.Flag_Azienda_Persona === null || ddl_dataItem.Flag_Azienda_Persona === "") {

            let Msg_Errore = "Selezionare l'Utente a cui si vuole cambiare il Codice Fiscale.";

            MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

            return;

        }

        codice_fiscale_selezionato = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem().CodFisc;

        flag_azienda_persona = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem().Flag_Azienda_Persona;

        var param = kendo.stringify(
            {
                Codice_Fiscale_Old: codice_fiscale_selezionato,
                Codice_Fiscale_New: $("#txt_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").val(),
                Flag_Azienda_Persona: flag_azienda_persona
            });

        ajaxAgronica(indirizzohttp + "/ControlliPreliminari_cambio_CF_utente_UtilityModificaChiavi",
            param,
            function (risposta) {
                resolve(risposta);
            },
            function (risposta) {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
                resolve(risposta);
            }, true);
    });

}

function Salva_cambio_CF_utente_UtilityModificaChiavi() {

    let utente_selezionato = Get_KendoDDLValue("ddl_utente_cambio_CF_utente_UtilityModificaChiavi");

    let ddl_dataItem = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem();

    let codice_fiscale_selezionato = null;

    let flag_azienda_persona = null;

    if (utente_selezionato === undefined || utente_selezionato === null || utente_selezionato === "" ||
        ddl_dataItem === undefined || ddl_dataItem === null || ddl_dataItem === "" ||
        ddl_dataItem.CodFisc === undefined || ddl_dataItem.CodFisc === null || ddl_dataItem.CodFisc === "" ||
        ddl_dataItem.Flag_Azienda_Persona === undefined || ddl_dataItem.Flag_Azienda_Persona === null || ddl_dataItem.Flag_Azienda_Persona === "") {

        let Msg_Errore = "Selezionare l'Utente a cui si vuole cambiare il Codice Fiscale.";

        MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

        return;

    }

    codice_fiscale_selezionato = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem().CodFisc;

    flag_azienda_persona = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem().Flag_Azienda_Persona;

    var param = kendo.stringify(
        {
            Codice_Fiscale_Old: codice_fiscale_selezionato,
            Codice_Fiscale_New: $("#txt_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").val(),
            Flag_Azienda_Persona: flag_azienda_persona
        });

    ajaxAgronica(indirizzohttp + "/Salva_cambio_CF_utente_UtilityModificaChiavi",
        param,
        function (risposta) {
            //Ricarico la ddl degli Utenti e la imposto sull'Utente di cui ho cambiato il Codice Fiscale
            $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataSource.read();

            Set_KendoDDLValue("ddl_utente_cambio_CF_utente_UtilityModificaChiavi", utente_selezionato);

            MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

            if (risposta.ParametroDue) {
                kendo.alert(risposta.ParametroDue_stringa);
            }

        }, null, true);

}

//###### FINE TAB Utility Cambio CF Utente

