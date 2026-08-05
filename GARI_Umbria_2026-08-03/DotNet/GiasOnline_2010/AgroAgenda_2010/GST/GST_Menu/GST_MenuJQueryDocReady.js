
$(document).ready(function () {

    $("#kendoWindowiFrameGeneric").css("display", "none");
    let m_btm = $(".AgronicaFooter").innerHeight() + 10;
    $("#grid-container").css("margin-bottom", m_btm + "px");

    let btnGestione = $("#" + btnGestioneSportello_ClientID);
    if (btnGestione.length > 0) {
        btnGestione.kendoButton({
            click: gestioneSportello
        });
    }

    //$("#cbSportelliPrecedenti").click(function () {
    //    sportelliPrecedenti(this.checked);
    //});

    let btnStampa = $("#" + btnStampaElenco_ClientID);
    if (btnStampa.length > 0) {
        btnStampa.click(stampaElenco);
    }

    $("#btnNuovoImpianto").kendoButton({
        click: function () {
            nuovoImpianto(true);
        }
    });
    $("#btnMappaturaLibera").kendoButton({
        click: function () {
            nuovoImpianto(false);
        }
    });
    $("#btnVerificaInterferenze").kendoButton({
        click: verificaInterferenze
    });
    $("#btnSalvaVariazioni").kendoButton({
        click: salvaVariazioni
    });

    VerificaPermessiEstrazione();

    $("#btnDistanzeMinime").kendoButton({
        click: distanzeMinime
    });
    $("#btnCodificaSpecie").kendoButton({
        click: codificaSpecie
    });

    $("#ddl_sportello").attr("sportelli-precedenti", "0");

    $("#ddl_sportello").kendoDropDownList({
        autoBind: true,
        autoWidth: true,
        dataTextField: "descr",
        dataValueField: "val",
        dataSource: leggiDDLSportello(),
        change: function (e) {
            impostaPreventivoConsuntivo(this.value());
            mostraMessaggi();
        },
        dataBound: function (e) {
            this.select(0);
            this.trigger("change");
        }
    });

    $("#id_info_preventivo_consuntivo").click(function () {
        InfoSportello();
    });

    $("#GST_Menu_Body").css("opacity", "1");

    $(window).resize(ridimensiona);

    ridimensiona();
});


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