
//DOCUMENT READY
$(document).ready(function () {
   
     // non c'è nulla, tutto spostato nella funzione impostaImputazioneImpiantiUC, perché sia il chiamante a gestire quando farla

});


function impostaImputazioneImpiantiUC() {

    if (resxObj !== null && resxObj !== undefined) {
        resxSceltaImpiantiUC = resxObj;
        resxSceltaImpiantiUC.unshift(readResxFile("GestioneContabilita/UserControl/App_LocalResources/ImputazioneImpiantiUC.ascx.resx"));
    }
    else {
        resxSceltaImpiantiUC.push(readResxFile("GestioneContabilita/UserControl/App_LocalResources/ImputazioneImpiantiUC.ascx.resx"));
        resxSceltaImpiantiUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx"));
    }

    Inizializza_Elenco_Ripartizione(resxSceltaImpiantiUC);
    Inizializza_Elenco_FiltroImpianti(resxSceltaImpiantiUC);

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelAreaImputazioneImpianti").style.opacity = "1";

    Piva = $(cIdPiva).val();

    creaKendoSwitch("ChkImpiantiIndefiniti", "S", "N", false, function (e) {
        if (e.checked) {
            //Nascondo la Griglia
            $("#tab_griglia_impianti").attr("style", "display:none");
            $("#id_ripartizione").attr("style", "display:none");
            $("#id_filtroimpianti").attr("style", "display:none");
            $("#id_note_raccolta").attr("style", "display:none");
            $("#warningImpiantiIndefiniti").show();
        }
        else {
            //Griglia Visibile
            $("#tab_griglia_impianti").attr("style", "display:inline-block");
            $("#id_ripartizione").attr("style", "display:inline-block");
            $("#id_filtroimpianti").attr("style", "display:inline-block");
            $("#id_note_raccolta").attr("style", "display:inline-block");
            $("#warningImpiantiIndefiniti").hide();
        }
    });

    // Modifica del 10/06/2022: Non permetto più l'inserimento della raccolta se non ho trovato degli impianti, quindi non serve più far visualizzare il check all'utente;
    // al suo posto se non trovo impianti mostro un messaggio che indica che la raccolta non verrà creata
    $("#boxChkImpiantiIndefiniti").hide();
    $("#warningImpiantiIndefiniti").hide();

    creaKendoDropDownList("cmbRipartizione", { read: RiempiCmbRipartizione }, "Tipo_Des_Ripartizione", "Tipo_Ripartizione").bind("change", cmbRipartizione_change);
    creaKendoDropDownList("cmbFiltroImpianti", { read: RiempicmbFiltroImpianti }, "Tipo_Filtro_Des", "Tipo_Filtro").bind("change", cmbFiltroImpianti_change);
        


    KendoDDL("cmbRipartizione").enable(StatoNonLetturaBoolean());
    KendoDDL("cmbFiltroImpianti").enable(StatoNonLetturaBoolean());
    $("#txt_note_raccolta").attr("readonly", !StatoNonLetturaBoolean());   
    $("#ChkImpiantiIndefiniti").data("kendoSwitch").enable(StatoNonLetturaBoolean());

    //Impostazione Default
    Modalita_Ripartizione = LeggiImprese_Impostazioni(865, -1);
    Obbligo_Ripartizione = LeggiImprese_Impostazioni(866, -1);

}
