
//DOCUMENT READY
$(document).ready(function() {
    // non c'è nulla, tutto spostato nella funzione impostaBeniConfezionamentoUC, perché sia il chiamante a gestire quando farla
});

function impostaBeniConfezionamentoUC(conGrigliaEredita, conGrigliaCarico, conGrigliaScarico) {

    $.logThis("EMBEDDED BeniConfezionamentoUC_jQueryDocReady: INIZIO");

    console.log("conGrigliaEredita = " + conGrigliaEredita);
    console.log("conGrigliaCarico = " + conGrigliaCarico);
    console.log("conGrigliaScarico = " + conGrigliaScarico);

    if (resxObj !== null && resxObj !== undefined) {
        resxBeniConfezionamentoUC = resxObj;
        resxBeniConfezionamentoUC.unshift(readResxFile("GestioneMagazzini/App_LocalResources/BeniConfezionamentoUC.ascx.resx"));
    }
    else {
        resxBeniConfezionamentoUC.push(readResxFile("GestioneMagazzini/App_LocalResources/BeniConfezionamentoUC.ascx.resx"));
        resxBeniConfezionamentoUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx"));
    }

    //lasciare qui, prima di convertire in tabStrip
    if ($(cCau_Mov_BC_Principale).val() === CAU_SCARICO) {
        $("#tabImballiIN").text(TraduzioneMultiResx(resxBeniConfezionamentoUC, "ImballiResi", "IMBALLI IN USCITA"));
    }

    tabStrip_BeniConfezionamento = $("#tabstrip_tabBeniConfezionamento").kendoTabStrip({
        animation: false
        //select: onSelect,
        //show: onShow,
    }).data("kendoTabStrip");
  
    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelAreaBeniConfezionamento").style.opacity = "1";

    if (conGrigliaCarico || conGrigliaScarico)
        elencoMagazziniOmni = RicercaFabbricatiOmni(false, $(cIdPiva).val(), parseInt(Qs_SaCod));

    if (conGrigliaCarico) {
        creaKendoDropDownList("cmbDestinazione", { read: RiempiCmbDestinazione }, "Fabbricato_Des_Estesa", "Key1").bind("change", cmbDestinazione_change);
        cmbDestinazione_change(); //Faccio scattare il click
    }

    if (conGrigliaScarico) {
        creaKendoDropDownList("cmbProvenienza", { read: RiempiCmbProvenienza }, "Fabbricato_Des_Estesa", "Key1").bind("change", cmbProvenienza_change);
        cmbProvenienza_change(); //Faccio scattare il click

        $("#inNumDocScarico").kendoNumericTextBox({ format: "0", min: 0, decimals: 0, default: 0, change: inNumDocScarico_change });
        Set_KendoNumTBValue("inNumDocScarico", 0);

        //TODO: è da fare solo per alcuni lav_cod (se doc ricevuti non deve comparire DDL)
        creaKendoDropDownList("inNumDocDDLScarico", { read: GetNumeratoriScarico },
            "NumeratoreTipo_Des", "Numeratore_Tipo", null, null, null, true,
            kendo.template($("#templateNumDocDDLScarico").html())).bind("change", inNumDocDDLScarico_change);

        creaKendoSwitch("inNumDocLockScarico",
            "<i class='fa fa-lg fa-lock'></i>",
            "<i class='fa fa-lg fa-unlock'></i>",
            false, inNumDocLockScarico_change, "65", "12px");
        //mi tocca fare così per fare in modo che in hoover compaia il tooltip esplicativo
        if ($("#inNumDocLockScarico").parent(".kendoSwitch").length === 1)
            $("#inNumDocLockScarico").parent(".kendoSwitch")[0].title = TraduzioneMultiResx(resxBeniConfezionamentoUC, "PermettiModificaNumeroDoc", "Permetti modifica numero documento");
    }

    if (conGrigliaCarico) {
        ConfiguraGrigliaCarico("tab_griglia_carico");
        BloccaMagazzinoCarico();
    }

    if (conGrigliaEredita)
        ConfiguraGrigliaCaricoEredita("tab_griglia_carico_eredita");

    if (conGrigliaScarico) {
        ConfiguraGrigliaScarico("tab_griglia_scarico");

        ImpostaDefaultNumeratore(parseInt($(cLav_Cod_Scarico).val()), tipoOpTestataScarico, "inNumDocDDLScarico", "inNumDocSinScarico", "inNumDocDesScarico");

        BloccaMagazzinoScarico("#tab_griglia_scarico");
    }

    //TODO: devo nascondere le tab/controlli che non servono e se c'è solo eredita cambiare la scritta della tab
    if (conGrigliaScarico === false) {
        //visto che non c'è nient'altro nascondo direttamente tutta la tab
        //$(tabStrip_BeniConfezionamento.items()[index_tabStrip_BeniConfezionamento_Scarico]).attr("style", "display:none");
        $(tabStrip_BeniConfezionamento.items()[index_tabStrip_BeniConfezionamento_Scarico]).hide();
        tabStrip_BeniConfezionamento.select(index_tabStrip_BeniConfezionamento_Carico);
    }

    if (conGrigliaCarico === false) {
        $("#id_riga_destinazione").hide();
        $("#id_riga_carico").hide();
    }

    if (conGrigliaEredita === false) {
        $("#id_riga_lblImballiEredita").hide();
        $("#id_riga_carico_eredita").hide();
    }

    $.logThis("EMBEDDED BeniConfezionamentoUC_jQueryDocReady: FINE");

}
