

var ddlRicercaRapida;
function caricaRicercaRapida() {
    creaKendoDropDownList("ddRicercaRapida", { read: caricaDDLRicercaRapida }, "text", "value");

}

async function caricaDDLRicercaRapida(option) {
    let dati = await CaricaRicercaRapida();
    option.success(dati)
}

function iniziallizaTooltips() {
    $("#tooltipiAziende").kendoTooltip({
        width: 350,
        position: "top",
        animation: {
            open: {
                effects: "zoom",
                duration: 150
            }
        },
        show: function (e) {
            e.sender.popup.element.addClass('blue-tooltip');
        },
        content: "L'azienda selezionata comprende anche tutte le figlie"
    }).data("kendoTooltip");

}
function iniziallizaKendoDate() {
  $("#DataValiditaInizio").kendoDatePicker();

  $("#DataValiditaFine").kendoDatePicker();
}


function iniziallizaOnClickEvents() {
    var permessi = LeggiPermessiBottoni();
    $("#btn_elaboraStatistica").bind("click", elaboraStatistiche);
    $("#btn_elencoSinteticoMovimenti").bind("click", getElencoSinteticoMovimenti);
    if (permessi.ReportDettagliServiziAzienda)
        $("#btn_reportDettagli").bind("click", getReportDettagli);
    else
        $("#btn_reportDettagli").hide();
    $("#btn_elencoSinteticoDettagliOperazioni").bind("click", getElencoSinteticoDettagliOperazioni);
}

function getTipoFiltro() {
    const dataCompetenzaRadio = document.getElementById('dataCompetenza');

    if (dataCompetenzaRadio.checked) {
        return dataCompetenzaRadio.value; // Return "1"
    } else {
        return document.getElementById('dataRegistrazione').value; // Return "2"
    }
}

async function elaboraStatistiche() {
    let datiGrafici = await getDataGrafici();
    let dataTransizioni = datiGrafici.transazioni
    let dataAziendeMovimentante = datiGrafici.aziende_movimentante
    creaChart("#chartTransazioni", "N° Operazioni", "N° Transazioni Totali per Servizio", dataTransizioni)
    creaChart("#chartAziendeMovimentante", "N° P.IVA", "N° Aziende Movimentante Totali per Servizio", dataAziendeMovimentante)
}


function creaChart(idDiv, axisTitle, title, series) {
    $(idDiv).kendoChart({
        title: {
            text: title
        },
        legend: {
            position: "right"
        },
        seriesDefaults: {
            type: "column"
        },
        series: series,
        valueAxis: {
            labels: {
                format: "n0"
            },
            line: {
                visible: false
            },
            title: {
                text: axisTitle,
                rotation: 0
            }
        },
        tooltip: {
            visible: true,
            format: "{0}%",
            template: "#= series.name #: #= value #"
        }
    });
}