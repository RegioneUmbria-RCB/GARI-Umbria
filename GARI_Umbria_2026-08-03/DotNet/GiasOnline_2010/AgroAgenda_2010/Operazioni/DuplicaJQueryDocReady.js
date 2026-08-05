

$(document).ready(function () {

    $('#chkSelezionaTuttiImpianti').click(function () {
        SelezionaDeselezionaTutti();
    });

    panelbarDuplica = $("#panelbarDuplica").kendoPanelBar({
        expandMode: "multiple"
    }).data("kendoPanelBar");


    KendoOperazioni_inizializza("divKendoOperazioni");

    
    var sParsed_Kendo = $(id_hdKendo_Impianti).val();
    
    if (sParsed_Kendo != "") {
        KendoImpianti_inizializza("divKendoImpianti");
        Aggiorna_hdKendo_ImpiantiSelezionaDaHidden();
    }

    event_bind();

    kendo_RipristinaSelezione();

    KendoOperazioni_GestioneRigheSelezionate();

});


/**
 * effettua l'associazione degli eventi, siccome viene chiamata da doc ready e da eventi asp.net eseguo unbind prima di fare il bind (altrimenti scattano 2 volte).
 * */
function event_bind() {

    $("#cBtn_SalvaTutto").unbind("click");
    $("#cBtn_SalvaTutto").click(function () {
            
        var procedi = Aggiorna_hdKendo_Operazioni_Selezione(true);

        procedi = procedi && Aggiorna_hdKendo_ImpiantiSelezionati(true);

        if (procedi) 
            $(".Btn_SalvaTutto").click();
        
    });

    $("#cBtn_FiltraAziende").unbind("click");
    $("#cBtn_FiltraAziende").click(function () {
    
        Aggiorna_hdKendo_Operazioni_Selezione();
        $(".Btn_FiltraAziende").click();
        
    });

    $("#cBtn_FiltraImpianti").unbind("click");
    $("#cBtn_FiltraImpianti").click(function () {
    
        Aggiorna_hdKendo_Operazioni_Selezione();
        $(".Btn_FiltraImpianti").click();
        
    });

    if (isFertirrigazione) {
        $("#cBtn_FiltroneNuovo").hide();
    } else {
        $("#cBtn_FiltroneNuovo").show();
    }

    $("#cBtn_FiltroneNuovo").unbind("click");
    $("#cBtn_FiltroneNuovo").click(function () {

        if (!isFertirrigazione) {
            Aggiorna_hdKendo_Operazioni_Selezione();

            if (usaFiltroRicercaNG) {
                ApriFiltroRicercaNG()
            } else {
                $(".Btn_FiltroneNuovo").click();
            }
        }
        
    });

    $("#cImgBtn_InterventoInserisci").unbind("click");
    $("#cImgBtn_InterventoInserisci").click(function () {

        Aggiorna_hdKendo_Operazioni_Selezione();
        if (KendoImpianti !== undefined) {
            Aggiorna_hdKendo_ImpiantiSelezionati();
        }
        
        $(".ImgBtn_InterventoInserisci").click();
        
    });

    $(".btnElimina").unbind("click");
    $(".btnElimina").click(function () {
        if (KendoImpianti !== undefined) {
            Aggiorna_hdKendo_ImpiantiSelezionati();
        }
    });
}


function kendo_RipristinaSelezione() {

    var sR = $(id_hdOperazioniSelezionate).val();

    if (sR != "") {

        var oR = JSON.parse(sR);
        for (var iCurItem in oR) {

            $("#" + oR[iCurItem].id_agenda ).click();
        }

    }

}

function Aggiorna_hdKendo_Operazioni_Selezione(mostraAlert) {

    var operazioniSelezionate = new Array();

    
    var data = KendoOperazioni.data("kendoGrid").dataSource.data();
    var totalRows = data.length;

    for (var i = 0; i < totalRows; i++) {

        var currentDataItem = data[i];

        if (currentDataItem.Selected) {

            var chiaveSplit = currentDataItem.id.split("-");

            var OperazioneSelezionata = {
                id_agenda: currentDataItem.id_agenda,
                data: kendo.toString(currentDataItem.Data2, "yyyy-MM-dd") + "T00:00:00.000Z",
                note: currentDataItem.note,
                raccoglitore_cod: currentDataItem.Raccoglitore_Cod,
                lav_cod: currentDataItem.Lav_cod
            }

            operazioniSelezionate.push(OperazioneSelezionata);
        }
    }

    if (operazioniSelezionate.length == 0) {
        if (mostraAlert)
            alert("Nessuna Operazione Selezionata.");
        return false;
    } 
    
    $(id_hdOperazioniSelezionate).val(JSON.stringify(operazioniSelezionate));

    return true;

}

function Aggiorna_hdKendo_ImpiantiSelezionaDaHidden() {

    var hdSelezione = $(id_hdImpiantiSelezionati).val();

    if (hdSelezione != "") {

        var ImpiantiSelezionati = JSON.parse(hdSelezione);

        for (var i = 0; i < ImpiantiSelezionati.length; i++) {
            var chiave = "input#" +
                ImpiantiSelezionati[i].piva + "-" +
                ImpiantiSelezionati[i].sa_cod + "-" +
                ImpiantiSelezionati[i].appezza + "-" +
                ImpiantiSelezionati[i].id_reg;

            $(chiave).click();
        }
    }
    
}

function Aggiorna_hdKendo_ImpiantiSelezionati(mostraAlert) {

    var ImpiantiSelezionati = new Array();


    var data = KendoImpianti.data("kendoGrid").dataSource.data();
    var totalRows = data.length;

    for (var i = 0; i < totalRows; i++) {

        var currentDataItem = data[i];

        if (currentDataItem.Selected) {
            
            var chiaveSplit = currentDataItem.id.split("-");

            var ImpiantoSelezionato = {
                piva: chiaveSplit[0],
                sa_cod: chiaveSplit[1],
                appezza: chiaveSplit[2],
                id_reg: chiaveSplit[3],
                rag_soc: currentDataItem.rag_soc,
                sa_nome: currentDataItem.sa_nome,
                sup_imp: currentDataItem.sup_imp,
                descrizione: currentDataItem.descrizione,
                validita_inizio_distinta: currentDataItem.validita_inizio_distinta,
                validita_fine_distinta: currentDataItem.validita_fine_distinta,
                app_nome: currentDataItem.app_nome
            }

            ImpiantiSelezionati.push(ImpiantoSelezionato);
        }
    }

    if (ImpiantiSelezionati.length == 0) {

        if (mostraAlert)
            alert("Nessun impianto Selezionato");
        return false;
    }

    $(id_hdImpiantiSelezionati).val(JSON.stringify(ImpiantiSelezionati));

    return true;


}

