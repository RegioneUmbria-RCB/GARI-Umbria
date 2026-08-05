
//###### INIZIO TAB Utility Cambio Piva

function ddl_impresa_cambio_piva_UtilityModificaChiavi_change(e) {

    let piva_selezionata = Get_KendoDDLValue("ddl_impresa_cambio_piva_UtilityModificaChiavi");

    if (piva_selezionata !== undefined && piva_selezionata !== null && piva_selezionata !== "") {
        $("#div_nuova_PIVA_cambio_piva_UtilityModificaChiavi").show();
        $("#divSalva_cambio_piva_UtilityModificaChiavi").show();
    } else {
        $("#div_nuova_PIVA_cambio_piva_UtilityModificaChiavi").hide();
        $("#divSalva_cambio_piva_UtilityModificaChiavi").hide();
    }

    $("#txt_nuova_PIVA_cambio_piva_UtilityModificaChiavi").val("");

}


async function Conferma_cambio_piva_UtilityModificaChiavi() {

    let risposta = await ControlliPreliminari_cambio_piva_UtilityModificaChiavi();

    if (risposta === undefined || risposta === null || risposta === "" ||
        risposta.RispostaOK === undefined || risposta.RispostaOK === null || risposta.RispostaOK === "" ||
        risposta.RispostaStringa === undefined || risposta.RispostaStringa === null)
        return;

    if (risposta.RispostaOK) {

        let msgTextDialog = "";

        msgTextDialog += risposta.RispostaStringa;

        if (msgTextDialog === "") {
            msgTextDialog += "Sei sicuro di voler modificare la Partita Iva ?";
        }

        Crea_kendoDialog_UtilityModificaChiavi(msgTextDialog,"Attenzione", "divSalva_cambio_piva_UtilityModificaChiavi", "id_dialog_cambio_piva", Salva_cambio_piva_UtilityModificaChiavi);

    }
}


//###### FINE TAB Utility Cambio Piva


//###### INIZIO TAB Utility Cambio CF/Piva Contatto

function ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi_change(e) {

    let piva_selezionata = Get_KendoDDLValue("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi");

    if (piva_selezionata !== undefined && piva_selezionata !== null && piva_selezionata !== "") {

        let ddl_contatto = $("#ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").data("kendoDropDownList");

        if (ddl_contatto !== undefined && ddl_contatto !== null && ddl_contatto !== "") {
            ddl_contatto.dataSource.read();
        }
        else {

            creaKendoDropDownList("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi", { read: Leggi_Contatto_cambio_CF_piva_contatto_UtilityModificaChiavi }, "Rag_Soc", "Cod_Contatto").bind("change", ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi_change);
        }

        $("#Note_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").show();
        $("#div_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").show();

        Set_KendoDDLValue("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi", "");


    } else {
        $("#Note_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();
        $("#div_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();
    }

    $("#div_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();
    $("#divSalva_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();

}


function ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi_change(e) {

    let contatto_selezionato = Get_KendoDDLValue("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi");

    if (contatto_selezionato !== undefined && contatto_selezionato !== null && contatto_selezionato !== "") {

        switch (contatto_selezionato.toString().length) {

            case 11:
                $("#lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").html("");
                $("#lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").html("Partita IVA nuova: ");
                break;
            case 16:
                $("#lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").html("");
                $("#lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").html("Codice Fiscale nuovo: ");
                break;
            case 0:
                $("#lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").html("");
                $("#lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").html("Codice Contatto nuovo: ");
                MessaggioErrore_Bootstrap("Selezionare un Contatto!", "DIV_Messaggi");
                break;
             default:
                $("#lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").html("");
                $("#lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").html("Codice Contatto nuovo: ");
                return

        }


        $("#div_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").show();
        $("#divSalva_cambio_CF_piva_contatto_UtilityModificaChiavi").show();
    }
    else {
        $("#div_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();
        $("#divSalva_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();
    }

    $("#txt_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").val("");

}

async function Conferma_cambio_CF_piva_contatto_UtilityModificaChiavi() {

    let risposta = await ControlliPreliminari_cambio_CF_piva_contatto_UtilityModificaChiavi();

    if (risposta === undefined || risposta === null || risposta === "" ||
        risposta.RispostaOK === undefined || risposta.RispostaOK === null || risposta.RispostaOK === "" ||
        risposta.RispostaStringa === undefined || risposta.RispostaStringa === null)
        return

    if (risposta.RispostaOK) {

        let contatto_selezionato = Get_KendoDDLValue("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi");

        let msgTextDialog = "";

        msgTextDialog += risposta.RispostaStringa;

        if (contatto_selezionato !== undefined && contatto_selezionato !== null && contatto_selezionato !== "") {

            Crea_kendoDialog_UtilityModificaChiavi(msgTextDialog,"Attenzione", "divSalva_cambio_CF_piva_contatto_UtilityModificaChiavi", "id_dialog_cambio_CF_piva", Salva_cambio_CF_piva_contatto_UtilityModificaChiavi);

        }
        else {
            return;
        }

    } else {
        return;
    }

}


//###### FINE TAB Utility Cambio CF/Piva Contatto

//###### INIZIO TAB Utility Cambio CF Utente
function ddl_utente_cambio_CF_utente_UtilityModificaChiavi_change(e) {

    let utente_selezionato = Get_KendoDDLValue("ddl_utente_cambio_CF_utente_UtilityModificaChiavi");

    let codice_fiscale_selezionato = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem().CodFisc;

    let flag_azienda_persona = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem().Flag_Azienda_Persona;

    if (utente_selezionato !== undefined && utente_selezionato !== null && utente_selezionato !== "" &&
        codice_fiscale_selezionato !== undefined && codice_fiscale_selezionato !== null && codice_fiscale_selezionato !== "" &&
        flag_azienda_persona !== undefined && flag_azienda_persona !== null && flag_azienda_persona !== "") {

        if (flag_azienda_persona === 1) {
            $("#lbl_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").html("");
            $("#lbl_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").html("Partita IVA Nuova:");
        } else if (flag_azienda_persona === 2) {
            $("#lbl_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").html("");
            $("#lbl_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").html("Codice Fiscale Nuovo:");
        }

        $("#div_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").show();
        $("#divSalva_cambio_CF_utente_UtilityModificaChiavi").show();

    } else {
        $("#div_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").hide();
        $("#divSalva_cambio_CF_utente_UtilityModificaChiavi").hide();
    }

    $("#txt_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").val("");
    
}

async function Conferma_cambio_CF_utente_UtilityModificaChiavi() {

    let risposta = await ControlliPreliminari_cambio_CF_utente_UtilityModificaChiavi();

    if (risposta === undefined || risposta === null || risposta === "" ||
        risposta.RispostaOK === undefined || risposta.RispostaOK === null || risposta.RispostaOK === "" ||
        risposta.RispostaStringa === undefined || risposta.RispostaStringa === null)
        return

    if (risposta.RispostaOK) {

        let msgTextDialog = "";

        let flag_azienda_persona = $("#ddl_utente_cambio_CF_utente_UtilityModificaChiavi").data("kendoDropDownList").dataItem().Flag_Azienda_Persona;

        if (flag_azienda_persona === 1) {
            msgTextDialog = "Sei sicuro di voler modificare la Partita IVA ?";
        } else if (flag_azienda_persona === 2) {
            msgTextDialog = "Sei sicuro di voler modificare il Codice Fiscale ?";
        }


        Crea_kendoDialog_UtilityModificaChiavi(msgTextDialog, "Attenzione", "divSalva_cambio_CF_utente_UtilityModificaChiavi", "id_dialog_cambio_CF", Salva_cambio_CF_utente_UtilityModificaChiavi);

    } else {
        return;
    }

}

//###### FINE TAB Utility Cambio CF Utente

function Crea_kendoDialog_UtilityModificaChiavi(msgTextDialog,title_string,id_container_string,id_dialog_string,function_modifica) {

    if (msgTextDialog === undefined || msgTextDialog === null || msgTextDialog === "" ||
        id_container_string === undefined || id_container_string === null || id_container_string === "" || 
        id_dialog_string === undefined || id_dialog_string === null || id_dialog_string === "" ||
        function_modifica === undefined || function_modifica === null || function_modifica === "" ||
        title_string === undefined || title_string === null)
        return

    let container = document.getElementById(id_container_string);
    let id_dialog = creaNewRowDiv(id_dialog_string);
    container.appendChild(id_dialog);

    $("#" + id_dialog_string).kendoDialog({
        title: title_string,
        closable: false,
        modal: {
            preventScroll: true
        },
        content: msgTextDialog,
        actions: [{
            text: 'Annulla',
            primary: true,
            action: function (e) {

                $("#" + id_dialog_string).remove();
            }
        },
        {
            text: 'Modifica',
            action: function (e) {

                function_modifica();

                $("#" + id_dialog_string).remove();
            }
        }]
    });

}