var cmbTipoSorgente;
var cmbOrigineDati;

var ddlPeriodoSeminaColturaPrincipale;
var ddlPeriodoRaccoltaColturaPrincipale;
var ddlPeriodoInterramentoResiduiPrecessione;


$(document).ready(function () {
    docReady();
});

async function docReady() {
    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            PianoNutrizionaleResx.unshift(readResxFile(resxSinglePath, "PianoConcimazione_MenuBS_jQueryDocReady.js"));
        });
    }

    Qs_Piva = $(hdPiva).val();
    Qs_Operazione = $(hdOperazione).val();
    Qs_Tipo = $(hdTipo).val();
    QS_AnalisiNG = $(hdAnalisiNG).val();

    $('.DatePicker').kendoDatePicker();

    $(ddlRegolamento).change(function () {
        AggiornaDescrizione();
    });

    $('#chkSelezionaTuttiImpianti').click(function () {
        SelezionaDeselezionaTutti();
    });

    $(ddlDoseP).change(function () {
        ddlDoseP_Click();
    });

    $(ddlDoseK).change(function () {
        ddlDoseK_Click();
    });

    $('.Chk_SelDecrementi_N').click(function () {
        Chk_SelDecrementi_N_Click($(this).find('input'), false);
    });

    $('.Chk_SelIncrementi_N').click(function () {
        Chk_SelIncrementi_N_Click($(this).find('input'), false);
    });

    $('.Chk_SelDecrementi_P').click(function () {
        Chk_SelDecrementi_P_Click($(this).find('input'), false);
    });

    $('.Chk_SelIncrementi_P').click(function () {
        Chk_SelIncrementi_P_Click($(this).find('input'), false);
    });

    $('.Chk_SelDecrementi_K').click(function () {
        Chk_SelDecrementi_K_Click($(this).find('input'), false);
    });

    $('.Chk_SelIncrementi_K').click(function () {
        Chk_SelIncrementi_K_Click($(this).find('input'), false);
    });

    $(DDL_P2O5).change(function () {
        ddlP_Click();
    });

    $(DDL_K2O).change(function () {
        ddlK_Click();
    });

    $(ddlMasS).change(function () {
        CalcolaDoseN();
    });

    $(ddlMasB).change(function () {
        $(Btn_Bilancio).click();
    });

    $('#iFrameGeneric').on('hidden.bs.modal', function () {
        iFrame_Chiusura();

    });

    let resp = await SportelloPCB();

    if (!resp.Sportello_Aperto) {
        $("#MainContent_Div_BTNSalva").hide();
        if (Qs_Operazione == "1") {
            kendo.alert(TraduzioneMultiResx(PianoNutrizionaleResx, "ImpossibileCrearePianoNutrizionaleSportelloChiuso", "Non è possibile creare il Piano di Concimazione. Sportello chiuso."));
        }
    }

    //$('.DatePicker').kendoDatePicker();
    $(Txt_ValiditaInizio).kendoDatePicker({
        min: resp.Validita_Inizio,
        max: resp.Validita_Fine
    });

    $(Txt_ValiditaInizio).kendoDateInput({
        min: resp.Validita_Inizio,
        max: resp.Validita_Fine
    });

    $(Txt_ValiditaFine).kendoDatePicker({
        min: resp.Validita_Inizio,
        max: resp.Validita_Fine
    });

    $(Txt_ValiditaFine).kendoDateInput({
        min: resp.Validita_Inizio,
        max: resp.Validita_Fine
    });

    $(Meteo_ChkAgenda).val(0);

    cmbTipoSorgente = $("#cmbTipoSorgente").kendoDropDownList({
        autoBind: true,
        autoWidth: true,
        filter: null,
        dataTextField: "sorgente_des",
        dataValueField: "sorgente_cod",
        dataSource: RicaricaTipoSorgente(),
        optionLabel: TraduzioneMultiResx(PianoNutrizionaleResx, "Seleziona", "Seleziona") & "...",
        change: function (e) {

            $(Meteo_TipoSorgente).val(this.value());
            $(Meteo_TipoSorgente_Real).val(this.value());

            //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)
            RicaricaOrigineDati();
        }
    }).data("kendoDropDownList");

    let versioneKendo = kendo.version.split('.')[0];
    var template = "";
    if (versioneKendo <= 2021) {
        template += "<div class='k-state-default'>";
        template += "   <span style='float:left;'>";
        template += "       #: data.nome_stazione #";
        template += "   </span>";
        template += "   <span style='float:right;'>";
        template += "       #: data.distanza #";
        template += "   </span>";
        template += "</div>";
    } else {

        let tmplt_r2 = "<span style='flex-grow:1;'>" +
            "               #: data.nome_stazione #" +
            "           </span>" +
            "           <span>" +
            "               #: data.distanza #" +
            "           </span>";
        template = "<div style='display:inline-block; width:100%;'>" +
            "           <div style='display:flex; font-size:12px; padding-bottom:3px;'>" +
            tmplt_r2 +
            "           </div>" +
            "       </div>"
    }

    cmbOrigineDati = $("#cmbOrigineDati").kendoDropDownList({
        autoBind: true,
        autoWidth: true,
        dataTextField: "nome_stazione",
        dataValueField: "id_stazione",
        filter: "contains",
        optionLabel: TraduzioneMultiResx(PianoNutrizionaleResx, "Seleziona", "Seleziona") & "...",
        template: template,
        change: function (e) {

            $(Meteo_TipoSorgente_Real).val(e.sender.dataItem().tipo_sorgente);
            $(Meteo_Sorgente).val(this.value());
        }
    }).data("kendoDropDownList");

    //setto il default 
    if ($(Meteo_TipoSorgente).val() !== '0') {
        cmbTipoSorgente.value($(Meteo_TipoSorgente).val());
        cmbTipoSorgente.trigger("change");
    }
    if ($(Meteo_Sorgente).val() !== '0') {
        cmbOrigineDati.value($(Meteo_Sorgente).val());
    }


    let viewModel = kendo.observable({
        checkboxChecked: false,
        clickHandler: function (e) {
            if (e.data.checkboxChecked) {
                $(Meteo_ChkAgenda).val(1);
                cmbTipoSorgente.enable(false);
                cmbOrigineDati.enable(false);
            } else {
                $(Meteo_ChkAgenda).val(0);
                cmbTipoSorgente.enable(true);
                cmbOrigineDati.enable(true);
            }
        }
    });
    kendo.bind($("#opRegGiasChk"), viewModel);

    caricaCmbMesi()

    AggiornaDescrizione()

    if (Qs_Operazione == '0') { //LETTURA
        ddlPeriodoSeminaColturaPrincipale.enable(false)
        ddlPeriodoRaccoltaColturaPrincipale.enable(false)
        ddlPeriodoInterramentoResiduiPrecessione.enable(false)

        $("#Btn_Meteo").prop("disabled", !this.checked);
        cmbOrigineDati.enable(false)
        cmbTipoSorgente.enable(false)
        $("#opRegGiasChk").prop("disabled", !this.checked);
    }

    if (QS_AnalisiNG == "True") {
        window.addEventListener('message', event => {
            if (verificaOriginSecondaria(window, window.origin, event) &&
                (event != null && event.data != null) && (event.data.messaggio != null) &&
                event.data.messaggio.includes("chiudiWindowGiasNG")) {
                ChiusuraModale();
            }
        });
    }
}


function caricaCmbMesi() {
    let data = new Array();
    data.push({ descrizione: "1 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Gennaio", "Gennaio"), id: "1", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "2 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Febbraio", "Febbraio"), id: "2", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "3 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Marzo", "Marzo"), id: "3", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "4 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Aprile", "Aprile"), id: "4", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "5 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Maggio", "Maggio"), id: "5", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "6 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Giugno", "Giugno"), id: "6", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "7 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Luglio", "Luglio"), id: "7", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "8 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Agosto", "Agosto"), id: "8", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "9 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Settembre", "Settembre"), id: "9", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "10 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Ottobre", "Ottobre"), id: "10", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "11 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Novembre", "Novembre"), id: "11", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });
    data.push({ descrizione: "12 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Dicembre", "Dicembre"), id: "12", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "DatiAnnoPrecedente", "DATI ANNO PRECEDENTE") });

    data.push({ descrizione: "13 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Gennaio", "Gennaio"), id: "13", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "14 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Febbraio", "Febbraio"), id: "14", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "15 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Marzo", "Marzo"), id: "15", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "16 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Aprile", "Aprile"), id: "16", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "17 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Maggio", "Maggio"), id: "17", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "18 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Giugno", "Giugno"), id: "18", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "19 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Luglio", "Luglio"), id: "19", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "20 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Agosto", "Agosto"), id: "20", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "21 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Settembre", "Settembre"), id: "21", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "22 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Ottobre", "Ottobre"), id: "22", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "23 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Novembre", "Novembre"), id: "23", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });
    data.push({ descrizione: "24 - " + TraduzioneMultiResx(PianoNutrizionaleResx, "Dicembre", "Dicembre"), id: "24", raggruppamento: TraduzioneMultiResx(PianoNutrizionaleResx, "Media5AnniAnnoCorrente", "MEDIA 5 ANNI + ANNO CORRENTE") });

    ddlPeriodoSeminaColturaPrincipale = $("#ddlPeriodoSeminaColturaPrincipale").kendoDropDownList({
        autoBind: true,
        autoWidth: true,
        filter: null,
        dataTextField: "descrizione",
        dataValueField: "id",
        dataSource: { data: data, group: "raggruppamento" },
        optionLabel: TraduzioneMultiResx(PianoNutrizionaleResx, "Seleziona", "Seleziona") & "...",
        change: function (e) {

            $(cmb_PeriodoSeminaColturaPrincipale).val(this.value());
        }
    }).data("kendoDropDownList");

    ddlPeriodoRaccoltaColturaPrincipale = $("#ddlPeriodoRaccoltaColturaPrincipale").kendoDropDownList({
        autoBind: true,
        autoWidth: true,
        filter: null,
        dataTextField: "descrizione",
        dataValueField: "id",
        dataSource: { data: data, group: "raggruppamento" },
        optionLabel: TraduzioneMultiResx(PianoNutrizionaleResx, "Seleziona", "Seleziona") & "...",
        change: function (e) {

            $(cmb_PeriodoRaccoltaColturaPrincipale).val(this.value());
        }
    }).data("kendoDropDownList");

    ddlPeriodoInterramentoResiduiPrecessione = $("#ddlPeriodoInterramentoResiduiPrecessione").kendoDropDownList({
        autoBind: true,
        autoWidth: true,
        filter: null,
        dataTextField: "descrizione",
        dataValueField: "id",
        dataSource: { data: data, group: "raggruppamento" },
        optionLabel: TraduzioneMultiResx(PianoNutrizionaleResx, "Seleziona", "Seleziona") & "...",
        change: function (e) {

            $(cmb_PeriodoInterramentoResiduiPrecessione).val(this.value());
        }
    }).data("kendoDropDownList");

    //setto il default 
    setMese(cmb_PeriodoSeminaColturaPrincipale, ddlPeriodoSeminaColturaPrincipale)
    setMese(cmb_PeriodoRaccoltaColturaPrincipale, ddlPeriodoRaccoltaColturaPrincipale)
    setMese(cmb_PeriodoInterramentoResiduiPrecessione, ddlPeriodoInterramentoResiduiPrecessione)
}

function setMese(cmb, ddl) {
    if ($(cmb).val() !== '') {
        ddl.value($(cmb).val());
        ddl.trigger("change");
    } else {
        ddl.value(1);
        ddl.trigger("change");
    }
}