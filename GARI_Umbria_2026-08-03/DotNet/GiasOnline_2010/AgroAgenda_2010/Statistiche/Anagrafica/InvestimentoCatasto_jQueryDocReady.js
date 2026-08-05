

/* InvestimentoCatasto_jQueryDocReady.js  */
var resxObj = [];
var resxArrPath = [];


$(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj.push(readResxFile(resxSinglePath, "RegCaricoScaricoPassaporti_jQueryDocReady.js"));
        });
    }

    InizializzaPagina_InvestimentoCatasto();

    InvestimentoCatastoEventiBind();

    InvestimentoCatastoPopolaDatiDefault();


});

function InvestimentoCatastoPopolaDatiDefault() {
    var d = new Date();
    $("#Txt_Data_DA").data("kendoDatePicker").value(d);
    $("#Txt_Data_A").data("kendoDatePicker").value(d);
}


function InvestimentoCatastoEventiBind() {
    $("#BtnFtiltraAppezzamenti").click(function () {
        if (usaFiltroRicercaNG) {
            FiltraImpiantiConFiltroRicercaNG();
        } else {
            $("#" + id_Btn_FiltraImpianti).click();
        }
        SalvaParametriDiv("#frmTestata", false);
    });

    $("#BtnElaboraDati").click(function () {
        letturaDatiInvestimentoCatastoKendoGridKendo();
    });

}

function InizializzaPagina_InvestimentoCatasto() {
    initSwitch();
    SetParametriDiv("#frmTestata", false);
    showHide_CheckSintesiCUAA();
    showHide_CheckTuttoIlCatasto();
    enable_BtnElaboraDati();
}

function initSwitch() {
    
    //$("#chk_Switch").kendoSwitch({
    //    change: switchFunction,
    //    messages: {
    //        checked: "",
    //        unchecked: ""
    //    },
    //    checked: false        
    //});


    let chkbox = document.getElementById("switch-app-campi");
    chkbox.checked = true;

    $(chkbox).on('change', switchFunction);

    $(chkbox).trigger("change");
}

function switchFunction() {
    if ($("#switch-app-campi").is(":checked")) {
        $("#lbl_RipartoAppezzamenti").text("Campi con Riparto");
        $("#lbl_AppezzaSenzaRiparto").text("Campi senza Riparto"); 

        $("#lblBtnFtiltraAppezzamenti").text("Seleziona i Campi");
        $("#estrazionePerCampi").text("Seleziona i Campi");

        $("#estrazionePerCampi").prop('value', 'campo');

        clickTuttoIlCatastoInArchivio();

    } else {
        $("#lbl_RipartoAppezzamenti").text("Appezzamenti con Riparto");
        $("#lbl_AppezzaSenzaRiparto").text("Appezzamenti senza Riparto");

        $("#lblBtnFtiltraAppezzamenti").text("Seleziona gli Appezzamenti");

        $("#estrazionePerCampi").prop('value', 'impianto');

        $("#divBtnFtiltraAppezzamenti").show();
    }
    
    showHide_CheckSintesiCUAA();
    showHide_CheckTuttoIlCatasto();
}

$("#chk_RipartoAppezzamenti").click(function () {
    uncheckCampiCheckBoxes();
    showHide_CheckSintesiCUAA();
    showHide_CheckTuttoIlCatasto();
});

$("#chk_AppezzaSenzaRiparto").click(function () {
    uncheckCampiCheckBoxes();
    showHide_CheckSintesiCUAA();
    showHide_CheckTuttoIlCatasto();
});

$("#chk_ParticelleSenzaRiparto").click(function () {
    uncheckCampiCheckBoxes();
    showHide_CheckSintesiCUAA();
    showHide_CheckTuttoIlCatasto();
});

function showHide_CheckSintesiCUAA() {
    if ($("#chk_RipartoAppezzamenti").is(":checked") &&
        !$("#chk_AppezzaSenzaRiparto").is(":checked") &&
        !$("#chk_ParticelleSenzaRiparto").is(":checked")) {

        $("#div_sintesiCUAAEstremiCatastali").show();
    } else {        
        $("#div_sintesiCUAAEstremiCatastali").hide();
        $("#chk_sintesiCUAAEstremiCatastali").prop('checked', false);
    }
}

function showHide_CheckTuttoIlCatasto() {
    //var show = $("#chk_Switch").is(":checked");
    var show = $("#switch-app-campi").is(":checked");

    if (show) {
        $("#div_tuttoIlCatastoInArchivio").show();
    } else {
        $("#div_tuttoIlCatastoInArchivio").hide();
        $("#div_tuttoIlCatastoInArchivio").prop('checked', false);
    }
}

function enable_BtnElaboraDati() {
    if ($("#chk_tuttoIlCatastoInArchivio").is(":checked")) {
        $("#BtnElaboraDati").attr('disabled', false);
    } else {
        if ($("#enableBtnElaboraDati").val() != 'False') { //estrazionePerCampi
            $("#BtnElaboraDati").attr('disabled', false);
        } else {
            $("#BtnElaboraDati").attr('disabled', true);
        }
    }
}

$("#chk_tuttoIlCatastoInArchivio").click(function () {
    clickTuttoIlCatastoInArchivio();
    enable_BtnElaboraDati();
});

function clickTuttoIlCatastoInArchivio() {
    if ($("#chk_tuttoIlCatastoInArchivio").is(":checked")) {
        $("#divBtnFtiltraAppezzamenti").hide();
    } else {
        $("#divBtnFtiltraAppezzamenti").show();
    }
}

function uncheckAppezzamentoCheckBoxes() {
    $("#chk_RipartoAppezzamenti").prop('checked', false);
    $("#chk_AppezzaSenzaRiparto").prop('checked', false);
    $("#chk_ParticelleSenzaRiparto").prop('checked', false);
}

function uncheckCampiCheckBoxes() {
    $("#chk_campiConRiparto").prop('checked', false);
    $("#chk_campiSenzaRiparto").prop('checked', false);
    $("#chk_particelleInConduzioneSenzaRipartoSuiCampi").prop('checked', false);
}

// ####################### VISTE #######################
$("#btn_esegui").click(function () {
    var nome_vista = KendoDDL("selListaViste").value();
    if (nome_vista && nome_vista !== "") Leggi_Vista_Investimento(nome_vista, true);
    else kendo.alert("Nessuna vista selezionata");
});

$("#btn_salva").click(function () {
    var nome_vista = KendoDDL("selListaViste").value();
    kendo.prompt("Inserire il nome della vista (se già presente verrà sovrascritta)", nome_vista).then(function (data) {
        if (data && data !== "") Salva_Vista_Investimento(data);
        else kendo.alert("Nome vista non inserito");
    });
});

$("#btn_elimina").click(function () {
    var nome_vista = KendoDDL("selListaViste").value();
    if (nome_vista && nome_vista !== "") {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: "Vista Investimento Progetti",
            messages: { okText: "Sì", cancel: "No" },
            content: "Vuoi eliminare la vista \"" + nome_vista + "\""
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () { Cancella_Vista_Investimento(nome_vista); });
        kendoConfirm.open();
    } else kendo.alert("Selezionare una vista da cancellare.");
});

creaKendoDropDownList("id_selListaViste",
    { read: RiempiListaViste },
    "desc", "cod"
).bind("change", CambiaListaViste_Investimento);
