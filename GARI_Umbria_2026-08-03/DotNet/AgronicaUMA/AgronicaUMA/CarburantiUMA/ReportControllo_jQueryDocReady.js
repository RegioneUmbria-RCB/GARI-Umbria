
var elencoInaCaricato = false;
var segnalazioneAcciseCaricato = false;
//DOCUMENT READY
$(document).ready(function () {
    docR();
});

async function docR() {
    
    storage.clear();
    // Se vengo da campagna il default va su Risorse
    if ($('input[name$="hdId_Agenda"]').val() !== "" &&
        $('input[name$="hdId_Agenda"]').val() !== "0") {
        $('#a_tabTestata').tab('show');
    }
    
    $.logThis("DocReady: INIZIO");
    
    tabstrip = $("#tabstrip_report").kendoTabStrip({
        animation: false,
        select: onSelect,
        activate: onActivate
        //show: onShow,
    }).data("kendoTabStrip");

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    //document.getElementById("panelArea").style.opacity = "1";
    
    //$($("#tabstrip_report").data("kendoTabStrip").items()[2]).attr("style", "display:none");
    //$("#tabstrip_report").show();
        
        
    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $('#bimestre').kendoDropDownList({
        filter: "contains",
        dataSource: [
            { "tipo": "gennaio-febbraio", "cod": 1 },
            { "tipo": "marzo-aprile", "cod": 2 },
            { "tipo": "maggio-giugno", "cod": 3 },
            { "tipo": "luglio-agosto", "cod": 4 },
            { "tipo": "settembre-ottobre", "cod": 5 },
            { "tipo": "novembre-dicembre", "cod": 6 }
        ],
        dataTextField: "tipo",
        dataValueField: "cod",
        autoWidth: true
    });

    $("#btn_carica_report").click(
        async function () {
            ConfiguraGrigliaElas("tab_griglia_reportELAS");
        }
    );

    $("#btn_esporta_carica").click(
        async function () {
            EsportaECaricaAllegato();
        }
    );

    $("#btn_esporta_Elenco").click(
        async function () {
            EsportaECaricaAllegatoInadempienti();
        }
    );

    $("#btn_esporta_Accise").click(
        async function () {
            EsportaECaricaAllegatoSegnalazioniAccise();
        }
    );

    $("#btn_carica_Elenco").click(
        async function () {
            ConfiguraGrigliaElencoInadempienti("tab_griglia_ElencoInadempienti");
        }
    );
    
    $("#btn_elenco_report_elas").click(
        function () {
            ElencoReportControlloElas();
        }
    );

    $("#btn_elenco_report_inadempienti").click(
        function () {
            ElencoReportControlloInadempienti();
        }
    );

    $("#btn_elenco_report_accise").click(
        function () {
            ElencoReportControlloSegnalazioneAccise();
        }
    );
            
    $("#btn_carica_elenco_trasf").click(
        async function () {
            ConfiguraGrigliaElencoTrasferimenti("tab_griglia_elencoTrasf");
        }
    );

    $("#btn_carica_Accise").click(
        function () {
            if (isNaN(parseInt($('#annoAcc')[0].value))) {
                let div = document.createElement("div");
                $(div).kendoDialog({
                    content: "Selezionare l'anno",
                    title: "Attenzione",
                    closable: false,
                    actions: [
                        {
                            text: "Continua",
                            primary: true,
                            action: function (e) {

                            },
                        }
                    ]
                }).data("kendoDialog").open();
            }
            else {
                ElencoSegnalazioneAccise("tab_griglia_SegnalazioniAccise");
            }
        }
    );

    $("#btn_segnalazione").click(
        function () {
            if ($('#dataSegnAcc')[0].value == '' || isValidDate($('#dataSegnAcc')[0].value) == false) {
                let div = document.createElement("div");
                $(div).kendoDialog({
                    content: "Impostare una data segnalazione valida",
                    title: "Attenzione",
                    closable: false,
                    actions: [
                        {
                            text: "Continua",
                            primary: true,
                            action: function (e) {

                            },
                        }
                    ]
                }).data("kendoDialog").open();
            }
            else {
                Segnala();
            }            
        }
    )

    $("#btn_annulla_segnalazione").click(
        function () {
            AnnullaSelezionati();
        }
    );

    $('#anno')[0].defaultValue = getYear();
    $("#btn_esporta_carica")[0].setAttribute("disabled", "")
    $("#btn_elenco_report_elas")[0].setAttribute("disabled", "")

    function onActivate(e) {
        //kendoConsole.log("Activated: " + $(e.item).find("> .k-link").text());
        var selectedIndex = $(e.item).index();
        //kendoConsole.log("selectedIndex: " + selectedIndex);
        
    }

    //inizializzazione della pagina la prima volta che viene caricata

    $(".preArea").show();
    $(".testataArea").show();
    $(".dettagliArea").show();

    function onSelect(e) {
        let elem = $(e.item).index();
        SeleTab(elem);
    }

    async function SeleTab(index) {
        
        switch (index) {
            case 0:
                //document.getElementsByClassName(GIAS_K_STATE_ACTIVE).removeClass(GIAS_K_STATE_ACTIVE);
                //document.getElementsByClassName("elas").addClass(GIAS_K_STATE_ACTIVE)
                //document.getElementsByClassName("ElencoInadempienti").style("display: none");
                //document.getElementsByClassName("reportELAS").style("display: block");
                $('#anno')[0].defaultValue = getYear();
                $("#btn_esporta_carica")[0].setAttribute("disabled", "")
                break;
            case 1:
                if (!elencoInaCaricato) {
                    await StatiPratiche_Load();
                    await conto_create('#conto');
                    KendoDDL("conto").value("2");
                    await Prov_Load('#prov');
                    KendoDDL("prov").value("-1");
                    await Citta_Load('#citta');
                    KendoDDL("citta").value("-1");
                    $('#annoIna')[0].defaultValue = getYear();
                    $("#btn_esporta_Elenco")[0].setAttribute("disabled", "")
                    $("#btn_elenco_report_inadempienti")[0].setAttribute("disabled", "")                    
                    elencoInaCaricato = true;
                }
                break;
            case 2:
                $('#annoTrasf')[0].defaultValue = getYear();
                break;
            case 3:
                if (!segnalazioneAcciseCaricato) {
                    await conto_create('#contoAcc');
                    KendoDDL("contoAcc").value(2);
                    await Prov_Load('#provAcc');
                    KendoDDL("provAcc").value("-1");
                    await Citta_Load('#cittaAcc');
                    KendoDDL("cittaAcc").value("-1");
                    tipo_pratica_create();
                    gia_segnalate_create();
                    //$('#annoAcc')[0].defaultValue = getYear();
                    $("#btn_esporta_Accise")[0].setAttribute("disabled", "")
                    $("#btn_elenco_report_accise")[0].setAttribute("disabled", "")                    
                    $("#btn_segnalazione")[0].setAttribute("disabled", "")
                    $("#btn_annulla_segnalazione")[0].setAttribute("disabled", "")
                    $(".kendoCalendar").kendoDatePicker({
                        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
                        max: new Date(2100, 12, 31)
                    });
                    $("#segnDaAcc").data("kendoDatePicker").value = new Date();
                    segnalazioneAcciseCaricato = true;
                }
                break;
        }

    }

    function conto_create(tab) {
        $(tab).kendoDropDownList({
            filter: "contains",
            dataSource: [{ "tipo": "TUTTI", "cod": 2 }, { "tipo": "PROPRIO", "cod": 0 }, { "tipo": "TERZI / COOP", "cod": -1 }],
            dataTextField: "tipo",
            dataValueField: "cod",
            autoWidth: true
        });
    }

    function tipo_pratica_create() {
        $("#tipoPraticaAcc").kendoDropDownList({
            filter: "contains",
            dataSource: [{ "tipo": "TUTTE", "cod": 2 }, { "tipo": "Anticipi", "cod": -1 }, { "tipo": "Richieste", "cod": 0 }, { "tipo": "Rendicontazioni", "cod": 1 }],
            dataTextField: "tipo",
            dataValueField: "cod",
            autoWidth: true
        });
    }


    function gia_segnalate_create() {
        $("#giaSegnAcc").kendoDropDownList({
            filter: "contains",
            dataSource: [{ "tipo": "No", "cod": 0 }, { "tipo": "Si", "cod": 1 }],
            dataTextField: "tipo",
            dataValueField: "cod",
            autoWidth: true
        });
    }

    $.logThis("DocReady: FINE");
}