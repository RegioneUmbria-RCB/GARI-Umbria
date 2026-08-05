var dataFiltroValidita = new Date();

$(document).ready(function () {
    $.logThis("DocReady: INIZIO");
    VisualizzaFinestra(false);

    creaKendoDropDownList("ddlYear", { read: Leggi_Anni }, "year_cod", "year_cod");
    KendoDDL("ddlYear").value(dataFiltroValidita.getFullYear());

    $('#txtImportoTotale').kendoNumericTextBox({ spinners: false, format: "c2", min: 0 });
    $('#txtBaseImp').kendoNumericTextBox({ spinners: false, format: "c2", min: 0 });
    $('#txtDeltaImportoT2').kendoNumericTextBox({ spinners: false, format: "c2", min: 0 });
    $('#txtDeltaImportoT3').kendoNumericTextBox({ spinners: false, format: "c2", min: 0 });

    var onActivate = function (e) {
        // access the selected item via e.item (Element)
        //if (e.item.id === "Dati_PrelieviOsservatiEffettivi") {

        //}

        // detach select event handler via unbind()
        tabStrip.unbind("activate", onActivate);
    };

    var tabStrip = $("#tabstrip").kendoTabStrip({
        animation: {
            open: {
                effects: "fadeIn"
            }
        },
        activate: onActivate
    });

    ShowTab(1, false);

    $("#btn_elabora").click(function () {
        if ($(cIdUtenteAbilitatoScrittura).val() === "True") {
            ElaboraDati();
        } else {
            kendo.alert("<div>Utente non abilitato.</div> <div>Operazione non permessa</div>")
        }
    });

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
    VisualizzaFinestra(true);
    $.logThis("DocReady: FINE");
});

