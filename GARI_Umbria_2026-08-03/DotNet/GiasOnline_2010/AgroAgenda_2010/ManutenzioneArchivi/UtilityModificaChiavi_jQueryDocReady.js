
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    //Creazione kendo tab strip tab Utility Modifica Chiavi
    $("#tabstripUtilityModificaChiavi").kendoTabStrip({
        select: OnSelect_tabstripUtilityModificaChiavi,
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });

    //Mostro la tab dell'Utility scelta e nascondo le altre
    let Type = parseInt($("input[name$='Type']").val());

    let tabstripUtilityModificaChiavi = $("#tabstripUtilityModificaChiavi").data("kendoTabStrip");

    switch (Type) {

        case enum_Utility.Cambio_Piva_Impresa:

            if ($("input[name$='hf_UtenteAbilitatoScrittura_Cambio_Piva_Impresa']").val() === "True") {

                //Nascondo la textbox per inserire la nuova piva e il bottone salva
                $("#div_nuova_PIVA_cambio_piva_UtilityModificaChiavi").hide();
                $("#divSalva_cambio_piva_UtilityModificaChiavi").hide();

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_utente_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_piva_contatto_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.select("#li_cambio_piva_UtilityModificaChiavi");

            }
            else {

                tabstripUtilityModificaChiavi.remove("#li_cambio_piva_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_piva_contatto_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_utente_UtilityModificaChiavi");

            }

            break;

        case enum_Utility.Cambio_CF_Piva_Contatto:

            if ($("input[name$='hf_UtenteAbilitatoScrittura_Cambio_CF_Piva_Contatto']").val() === "True") {

                //Nascondo i controlli e il bottone salva
                $("#Note_ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi").show();
                $("#Note_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();
                $("#div_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();
                $("#div_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();
                $("#divSalva_cambio_CF_piva_contatto_UtilityModificaChiavi").hide();

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_utente_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_piva_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.select("#li_cambio_CF_piva_contatto_UtilityModificaChiavi");

            }
            else {

                tabstripUtilityModificaChiavi.remove("#li_cambio_piva_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_piva_contatto_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_utente_UtilityModificaChiavi");

            }

            break;

        case enum_Utility.Cambio_CF_Utente:

            if ($("input[name$='hf_UtenteAbilitatoScrittura_Cambio_CF_Utente']").val() === "True") {

                //Nascondo la textbox per inserire il nuovo codice fiscale e il bottone salva
                $("#div_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi").hide();
                $("#divSalva_cambio_CF_utente_UtilityModificaChiavi").hide();

                tabstripUtilityModificaChiavi.remove("#li_cambio_piva_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_piva_contatto_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.select("#li_cambio_CF_utente_UtilityModificaChiavi");

            }
            else {

                tabstripUtilityModificaChiavi.remove("#li_cambio_piva_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_piva_contatto_UtilityModificaChiavi");

                tabstripUtilityModificaChiavi.remove("#li_cambio_CF_utente_UtilityModificaChiavi");

            }

            break;

    }


    $.logThis("DocReady: FINE");
});

function OnSelect_tabstripUtilityModificaChiavi(e) {

    let piva = $("input[name$='hf_Piva']").val();

    switch (e.item.id) {
        case "li_cambio_piva_UtilityModificaChiavi":      
            creaKendoDropDownList("ddl_impresa_cambio_piva_UtilityModificaChiavi", { read: Leggi_Impresa_con_PIVA_cambio_piva_UtilityModificaChiavi }, "Rag_Soc_Piva", "Piva").bind("change", ddl_impresa_cambio_piva_UtilityModificaChiavi_change);

            //Imposto l'impresa  dalla piva che ho nell'hiddenfield
            if (piva !== undefined && piva !== null && piva !== "") {

                Set_KendoDDLValue("ddl_impresa_cambio_piva_UtilityModificaChiavi", piva);

            }


            if (Get_KendoDDLValue("ddl_impresa_cambio_piva_UtilityModificaChiavi") !== undefined &&
                Get_KendoDDLValue("ddl_impresa_cambio_piva_UtilityModificaChiavi") !== null &&
                Get_KendoDDLValue("ddl_impresa_cambio_piva_UtilityModificaChiavi") !== "") {

                $("#div_nuova_PIVA_cambio_piva_UtilityModificaChiavi").show();
                $("#divSalva_cambio_piva_UtilityModificaChiavi").show();

                $("#txt_nuova_PIVA_cambio_piva_UtilityModificaChiavi").val("");

            }

            break;

        case "li_cambio_CF_piva_contatto_UtilityModificaChiavi":
            creaKendoDropDownList("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi", { read: Leggi_Impresa_con_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi }, "rag_soc", "PIVA").bind("change", ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi_change);

            //Imposto l'impresa  dalla piva che ho nell'hiddenfield
            if (piva !== undefined && piva !== null && piva !== "") {

                Set_KendoDDLValue("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi", piva);

            }

            if (Get_KendoDDLValue("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi") !== undefined &&
                Get_KendoDDLValue("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi") !== null &&
                Get_KendoDDLValue("ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi") !== "") {

                creaKendoDropDownList("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi", { read: Leggi_Contatto_cambio_CF_piva_contatto_UtilityModificaChiavi }, "Rag_Soc", "Cod_Contatto").bind("change", ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi_change);

                $("#Note_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").show();
                $("#div_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi").show();

                Set_KendoDDLValue("ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi", "");

            }

            break;
        case "li_cambio_CF_utente_UtilityModificaChiavi":
            creaKendoDropDownList("ddl_utente_cambio_CF_utente_UtilityModificaChiavi", { read: Leggi_Username_con_Codice_Fiscale_cambio_CF_utente_UtilityModificaChiavi }, "UserName_CodFisc", "UserName").bind("change", ddl_utente_cambio_CF_utente_UtilityModificaChiavi_change);
            break;
    }

    $("#tabstripUtilityModificaChiavi").kendoTabStrip().unbind("select", OnSelect_tabstripUtilityModificaChiavi);
}