
//DOCUMENT READY
$(document).ready(function () {
    docR();
});

async function docR() {
    //if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
    //    // Carico i files resx per le traduzioni
    //    resxArrPath.forEach(function (resxSinglePath) {
    //        gestioneCarbResx.push(readResxFile(resxSinglePath, "gestionecosti_jQueryDocReady.js"));
    //    });
    //}
    if (QS_Avanzamento == 0)
        $("#legenda").hide();
    await ddlAzienda_Load();
    //await StatiPratiche_Load();
    //await conto_create();
    //KendoDDL("conto").value("2");
    await Prov_Load();
    KendoDDL("prov").value("-1");
    await Citta_Load();
    KendoDDL("citta").value("-1");
    //var Cmb_Pratiche = KendoDDL("statoPratica")
    //Cmb_Pratiche.value("-1");

    var Cmb_Imprese = KendoDDL("ddlAzienda")
    if (QS_Piva === "") {
        Cmb_Imprese.value("-1");
        ddlAzienda_Change();
    }

    if (QS_Piva !== "") {
        Cmb_Imprese.value(QS_Piva);
        ddlAzienda_Change();
    }
    /*var gestOraInizioFine = RicercaUtenti_Impostazioni(enum_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE, 2);
    if (gestOraInizioFine === "notSet")
        gestOraInizioFine = "1";
    if (gestOraInizioFine === undefined || gestOraInizioFine === "0")
        hiddenOraInizioFine = true;
    else
        hiddenOraInizioFine = false;*/

    $('#anno')[0].defaultValue = getYear();

    let promises = new Array();
    promises.push(Agro_LeggiPermessoUtente(username_master, 455, 2));
    promises.push(Agro_LeggiPermessoUtente(username_master, 456, 2));
    promises.push(Agro_LeggiPermessoUtente(username_master, 457, 2));
    promises.push(Agro_LeggiPermessoUtente(username_master, 458, 2));

    let resp = await Promise.all(promises);

    permesso_richiesta = resp[0];
    permesso_rendicontazione = resp[1];
    permesso_approvazione_richiesta = resp[2];
    permesso_approvazione_rendicontazione = resp[3];
    if (QS_Avanzamento == 1)
        checkPerc($("#legendaPerc"));
    (function ($, kendo) {
        $.extend(true, kendo.ui.validator, {
            rules: { // custom rules
                timevalidation: function (input, params) {

                    if ($(input).data("kendoTimePicker") !== undefined &&
                        $(input).data("kendoTimePicker").value() === null)
                        return false;

                    return true;
                },
            },
            messages: {
                timevalidation: function (input) {
                    return (TraduzioneMultiResx(gestioneCarbResx, "OraNonValida", "Ora non valida"));
                },
            }
        });
    })(jQuery, kendo);



    // Se vengo da campagna il default va su Risorse
    if ($('input[name$="hdId_Agenda"]').val() !== "" &&
        $('input[name$="hdId_Agenda"]').val() !== "0") {
        $('#a_tabTestata').tab('show');
    }

    // Evita l'utilizzo dell'invio
    // TODO Stefano
    //$(window).keydown(function (event) {
    //    if (event.keyCode == 13) {
    //        event.preventDefault();
    //        return false;
    //    }
    //});

    //    $('#aspnetForm').change(function () {
    //        controlla_form();
    //    });

    $.logThis("DocReady: INIZIO");

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    tabstrip = $("#tabstrip_elenco").kendoTabStrip({
        animation: false,
        //select: onSelect,
        activate: onActivate
        //show: onShow,
    }).data("kendoTabStrip");


    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";


    //Nascondo pulsante Refresh
    if (($(cId_Mov_Det).val() == "0") && ($(cId_Agenda).val() !== "0" || $(cId_Agenda_CDG).val() !== "0")) {
        $("#btn_refresh").attr("style", "display:none");
    }


    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    //eventi di click pulsanti
    $("#btn_trova_impianti").click(
        function () {
            TrovaImpianti();
        });

    $("#btn_ripartizione").click(
        function () {
            RipartizioneAutomatica();
        });


    $("#btn_salva").click(
        function () {
            AggiornaDati(false);
        });


    $("#btn_elimina_cdg").click(
        function () {
            Conferma_Delete_CDG();
        });


    $("#btn_refresh").click(
        function () {
            ConfiguraGrigliaDettagliZoo("tab_griglia_dettagliZoo");
        });


    //eventi di click pulsanti
    $("#btn_salva_esci").click(
        function () {
            AggiornaDati(true);
        });


    //eventi di click pulsanti
    $("#btn_avanti").click(
        function () {
            Avanti(true);
        });

    //eventi di click pulsanti
    $("#btn_indietro").click(
        function () {
            Indietro(true);
        });

    //eventi di click pulsanti
    $("#btn_trova_no_cdg").click(
        function () {
            Prossimo(true);
        });


    $("#btn_squadre").click(
        function () {

            var piva = getParameterByName('p');

            var url = "../AnalisiCostiProduzione/ImputazioneSquadre.aspx?p=" + piva + "&veg_cod=" + $(cVeg_Cod).val() + "&id_attivita=" + ID_Attivita_Base + "&lav_cod=" + $(cLav_Cod).val() + "&data_movimento=" + $('input[name$="txt_dataop"]').val() + "&origine=./GestioneCosti.aspx";

            $("#PaginaSquadre").attr("src", url);
            $("#iFrameSquadre").modal('toggle');

        });


    // Pulsanti non mostrati in visualizzazione
    if ($("input[name$='hf_UtenteAbilitatoScrittura']").val() == "False") {
        $("#btn_salva_esci").hide();
        $("#btn_salva").hide();
        $("#btn_refresh").hide();
        $("#btn_ripartizione").hide();
        $("#btn_trova_impianti").hide();
        $("#btn_elimina_cdg").hide();
    }

    function onActivate(e) {
        //kendoConsole.log("Activated: " + $(e.item).find("> .k-link").text());
        var selectedIndex = $(e.item).index();
        //kendoConsole.log("selectedIndex: " + selectedIndex);

        if (selectedIndex == 4 && bZooInizializato == false) {


            bZooALL = true;
            bZooInizializato = true;

            //if (($(cId_Agenda_CDG).val() == "0" && $(cId_Agenda).val() == "0") || $(cId_Mov_Det).val() !== "0")  {
            ConfiguraGrigliaDettagliZoo("tab_griglia_dettagliZoo");
            //}
        }
    }

    //inizializzazione della pagina la prima volta che viene caricata

    $(".preArea").show();
    $(".testataArea").show();
    $(".dettagliArea").show();

    //$(".blocco")[0].children[1].innerHTML = (QS_Avanzamento != 0) ? "ELENCO RENDICONTAZIONI" : "ELENCO RICHIESTE"

    $.logThis("DocReady: FINE");
}