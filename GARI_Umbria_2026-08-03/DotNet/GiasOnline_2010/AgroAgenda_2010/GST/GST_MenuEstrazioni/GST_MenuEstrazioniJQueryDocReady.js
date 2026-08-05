


$(document).ready(function () {

    $("#kendoWindowiFrameGeneric").css("display", "none");
    let m_btm = $(".AgronicaFooter").innerHeight() + 10;
    $("#grid-container").css("margin-bottom", m_btm + "px");

    creaKendoDropDownList("ddlSelezioneSportello", { read: popolaSelezioneSportello }, "Sementieri_Sportello_Configurazione_des", "Sementieri_Sportello_Configurazione_cod");
    creaKendoDropDownList("ddlSelezioneRegione", { read: popolaSelezioneRegioni }, "regione_des", "regione_cod");

    $("#ddlSelezioneRegione").data("kendoDropDownList").bind("select", function (e) {
        this.value = 1;
        this.text = 'Emilia Romagna';
        e.preventDefault();
    });

    $("#ddlSelezioneRegione").data("kendoDropDownList").select(1);
    $("#ddlSelezioneRegione").data("kendoDropDownList").enable(false);

    $("#btnMostraEstrazioni").kendoButton();
    $("#btnMostraEstrazioni").data("kendoButton").bind("click", function (e) {
        if ($("#ddlSelezioneSportello").data("kendoDropDownList").value() == 0) {
            alert("Selezionare uno sportello.");
            return;
        }
        if ($("#ddlSelezioneRegione").data("kendoDropDownList").value() == 0) {
            alert("Selezionare una regione");
            return;
        }

        $("#top-side").css("display", "block");
    });

    $("#ddlSelezioneSportello").data("kendoDropDownList").bind("change", function (e) {
        if (this.value() != 0) {
            useCache = false;
            var activeBtn = $(".btnAttivo");
            if (activeBtn.length > 0) {
                activeBtn.data("kendoButton").trigger("click");
            }
        } else {
            resetEstrazioneGrid($('#estrazioneKendoGrid'));
            resetBtnStyle();
            $("#top-side").css("display", "none");
        }
    });

    let btnPreventivoColtivazione = $("#btnPreventivoColtivazione");

    if (btnPreventivoColtivazione.length > 0) {
        btnPreventivoColtivazione.kendoButton();

        var button = btnPreventivoColtivazione.data("kendoButton");
        button.bind("click", function (e) {
            if (btnPreventivoColtivazione.hasClass("btnAttivo") && useCache) return;

            var sportello = $("#ddlSelezioneSportello").data("kendoDropDownList").dataItem().Sementieri_Sportello_Configurazione_cod;

            resetEstrazioneGrid($('#estrazioneKendoGrid'));
            CreaGrigliaKendoEstrazioni("PreventivoColtivazione", sportello);
            resetBtnStyle();
            setActiveBtn(btnPreventivoColtivazione);
        });
    }

    let btnInterferenze = $("#btnInterferenze");

    if (btnInterferenze.length > 0) {
        btnInterferenze.kendoButton();

        var button = btnInterferenze.data("kendoButton");
        button.bind("click", function (e) {
            if (btnInterferenze.hasClass("btnAttivo") && useCache) return;

            var sportello = $("#ddlSelezioneSportello").data("kendoDropDownList").dataItem().Sementieri_Sportello_Configurazione_cod;

            resetEstrazioneGrid($('#estrazioneKendoGrid'));
            CreaGrigliaKendoEstrazioni("Interferenze", sportello);
            resetBtnStyle();
            setActiveBtn(btnInterferenze);
        });
    }

    let btnVariazioni = $("#btnVariazioni");

    if (btnVariazioni.length > 0) {
        btnVariazioni.kendoButton();

        var button = btnVariazioni.data("kendoButton");
        button.bind("click", function (e) {
            if (btnVariazioni.hasClass("btnAttivo") && useCache) return;

            var sportello = $("#ddlSelezioneSportello").data("kendoDropDownList").dataItem().Sementieri_Sportello_Configurazione_cod;

            resetEstrazioneGrid($('#estrazioneKendoGrid'));
            CreaGrigliaKendoEstrazioni("Variazioni", sportello);
            resetBtnStyle();
            setActiveBtn(btnVariazioni);
        });
    }

    let btnConsuntivo = $("#btnConsuntivo");

    if (btnConsuntivo.length > 0) {
        btnConsuntivo.kendoButton();

        var button = btnConsuntivo.data("kendoButton");
        button.bind("click", function (e) {
            if (btnConsuntivo.hasClass("btnAttivo") && useCache) return;

            var sportello = $("#ddlSelezioneSportello").data("kendoDropDownList").dataItem().Sementieri_Sportello_Configurazione_cod;

            resetEstrazioneGrid($('#estrazioneKendoGrid'));
            CreaGrigliaKendoEstrazioni("Consuntivo", sportello);
            resetBtnStyle();
            setActiveBtn(btnConsuntivo);
        });
    }

    $("#GST_MenuEstrazioni_Body").css("opacity", "1");

});

function resetEstrazioneGrid(div) {
    if (div.data().kendoGrid != undefined) {
        div.data().kendoGrid.destroy();
        div.empty();
        div.css("display", "none");
    }
}

function ridimensiona() {

    let win_h = $(window).height();
    let ftr_h = $(".AgronicaFooter").outerHeight() + 10;
    let top_h = $("#top-side").position().top + $("#top-side").outerHeight();

    let h = win_h - top_h - ftr_h;
    h = Math.max(500, h);
    $("#grid-container").innerHeight(h);

    let t1 = $("#grid-container").position().top;
    let t2 = $("#placeInterferenzeScroll").position().top;
    h -= (t2 - t1);
    h -= 3; //margin-bottom + border-bottom del k-block
    $("#placeInterferenzeScroll").innerHeight(h);
    $("#placeNotificheScroll").innerHeight(h);

    ridimensionaTabs();
}

function ridimensionaTabs() {

    let gap = $("#placeInterferenze").position().top - $("#placeInterferenzeScroll").position().top;
    let container_h = $("#placeInterferenzeScroll").innerHeight();
    let tab_h = $("#tabs").height() - $($("#tabs .k-content")[0]).height();

    let cont_h = container_h - tab_h - gap;
    //cont_h = Math.floor(cont_h / 10) * 10;

    $("#tabs .k-content").height(cont_h);

}