// Variabili globali della pagina
var elemCod;
var dataSourceClientiFornitori = null;
var enumEstrazioniConf = {};
/** Aggiungere a questa variabile proprietà per i filtri che si vuole rendere obbligatori, sempre od in determinate condizioni */
//var filtriObbligatori = {}; // Variabile da usare in caso occorra aggiungere controlli di validità lato client
var riepilogoFiltriStampaMassiva = {};

$(document).ready(function () {

    // Tracciabilita: new GenericOption(187, "Export Tracciabilità Conferimenti"), // obsoleto, rimpiazzato con Esportazione Excel Conferimenti + check tracciabilità impianti
    enumEstrazioniConf = {
        ConfXArticolo: new GenericOption(220, "Riepilogo Conferimenti per Articolo"), 
        EC_Bolle: new GenericOption(221, "Estratto Conto Bolle Conferimento"), // Conf_EC_Bolle
        EC_Imballi: new GenericOption(184, "Estratto Conto Beni Confezionamento"), // Conf_EC_Imballi
        Saldo_Imballi: new GenericOption(185, "Saldo Imballi"), // Conf_Saldo_Imballi     
        Esportazione_BolleFF_XLS: new GenericOption(217, "Esportazione Excel Conferimenti"), // Conf_Esportazione_BolleFF_XLS
        Esportazione_Trasportatori_XLS: new GenericOption(219, "Esportazione Excel Trasportatori"), // Conf_Esportazione_BolleFF_XLS
        Stampa_Massiva_Bolle: new GenericOption(175, "Stampa Massiva Bolle Conferimento"),
        Stampa_Massiva_Pomodoro: new GenericOption(222, "Stampa Massiva Certificati Pomodoro"),
        Esportazione_Pomodoro_XLS: new GenericOption(223, "Esportazione Excel Certificati Pomodoro"),
        Riepilogo_Conferimenti: new GenericOption(186, "Riepilogo Conferimenti"), // Conf_Riepilogo_Conferimenti
        Report_CSV: new GenericOption(666, "Esportazione Conferimenti su file CSV"),
        Comunicazione_Credito: new GenericOption(229, "Lettera con Elenco Conferimenti")
        };

    $(".datepicker2").datepicker({ format: 'dd/mm/yyyy', changeYear: true, changeMonth: true }); // Controlli legacy

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31),
        format: "dd/MM/yyyy"
    });

    var today = new Date();
    today.setHours(0, 0, 0);
    set_data("dpDataInizio", today, null);
    set_data("dpDataFine", today, null);
    set_data("dpDataGiacenza", today, null);


    creaKendoSwitch(); // In questo modo creo una switch per ogni elemento che ha la classe "kendoSwitch"

    // "on" è un metodo di JQuery, non di Kendo, quindi sto utilizzando l'evento di change legato all'input html "#ddlEstrazioni"
    var ddlEstrazioni = creaKendoDropDownList("ddlEstrazioni", { read: ddlEstrazioniRead }, "Desc", "Value");
    ddlEstrazioni.on("change", ddlEstrazioniChange);

    creaKendoDropDownList("ddlMagazzini", { read: ddlMagazziniRead }, "Ubic_Des", "key_Dest").on("change", ddlMagazziniChange);
        //.data("kendoDropDownList").bind("dataBound", function(e) { console.log(e); if (e.sender.dataSource.data().length) e.sender.data("kendoDropDownList").select(1); });

    // La funzione 'ServerFiltering' a differenza della 'creaKendoDropDownList' ha come valore di ritorno l'oggetto kendo creato, quindi lo sfrutto per richiamare il metodo bind
    creaKendoDropDownListServerFiltering("ddlProdotti", "Prodotto_Des", "Prodotto_Cod", ddlProdottiRead, ddlProdottiChange, 2, "", "", null)
        .bind("filtering", ddlFiltering);

    creaKendoDropDownList("ddlRapportiContabili", { read: ddlRapportiContabiliRead }, "Desc", "Value").on("change", ddlRapportiContabiliChange);

    creaKendoDropDownList("ddlCentriAziendali", { read: ddlCentriAziendaliRead }, "sa_nome", "sa_cod", null, null, null, true).on("change", ddlCentriAziendaliChange);

    var kddlPrimiCessionari = creaKendoDropDownList("ddlPrimiCessionari", { read: ddlPrimiCessionariRead }, "Rag_Soc_Progressivo", "Cod_RisUm", "contains", [{ field: "Rag_Soc_Completa" }, { field: "Cod_Contatto" }, { field: "Attivita_Des" }, { field: "Progressivo" }], null, false).data("kendoDropDownList");
    kddlPrimiCessionari.bind("change", ddlPrimiCessionariChange);
    kddlPrimiCessionari.bind("open", ddlArrayFilterOpen);

    var kddlSecCessionari = creaKendoDropDownList("ddlSecondiCessionari", { read: ddlSecondiCessionariRead }, "Rag_Soc_Progressivo", "Cod_RisUm", "contains", [{ field: "Rag_Soc_Completa" }, { field: "Cod_Contatto" }, { field: "Attivita_Des" }, { field: "Progressivo" }], null, false).data("kendoDropDownList");
    kddlSecCessionari.bind("change", ddlSecondiCessionariChange);
    kddlSecCessionari.bind("open", ddlArrayFilterOpen);

    var kddlProduttori = creaKendoDropDownList("ddlProduttori", { read: ddlProduttoriRead }, "Rag_Soc_Progressivo", "Cod_RisUm", "contains", [{ field: "Rag_Soc_Completa" }, { field: "Cod_Contatto" }, { field: "Attivita_Des" }, { field: "Progressivo" }], null, true).data("kendoDropDownList");
    kddlProduttori.bind("change", ddlProduttoriChange);
    kddlProduttori.bind("open", ddlArrayFilterOpen);

    creaKendoDropDownList("ddlSpecie", { read: ddlSpecieRead }, "Veg_Des", "Veg_Cod").on("change", ddlSpecieChange);

    creaKendoDropDownList("ddlVarieta", { read: ddlVarietaRead }, "Cul_Des", "Cul_Cod").on("change", ddlVarietaChange);

    creaKendoDropDownList("ddlDocPrefisso", { read: ddlDocPrefissoRead }, "Doc_Numero_Sin", "Doc_Numero_Sin");

    // NB: Il campo su db è float, ma permetto all'utente di inserire solo numeri interi
    $("#tbDocPrimoNumero").kendoNumericTextBox({
        decimals: 0,
        format: "n0",
        restrictDecimals: true,
        value: 0
    });

    creaKendoDropDownList("ddlDocSuffisso", { read: ddlDocSuffissoRead }, "Doc_Numero_Des", "Doc_Numero_Des");

    $("#tbDocUltimoNumero").kendoNumericTextBox({
        decimals: 0,
        format: "n0",
        restrictDecimals: true,
        value: 0
    });

    creaKendoDropDownList("ddlStampanti", { read: ddlStampantiRead }, "Desc", "Value");

    $("#tbNumeroCopieStampe").kendoNumericTextBox({
        decimals: 0,
        format: "n0",
        restrictDecimals: true,
        value: 1
    });

    $("#btnConfermaStampaMassiva").on("click", function (e) { return true; });
    $("#btnAnnullaStampaMassiva").on("click", function (e) { return false; });

    // Se ho l'impostazione della gerarchia disattivata, per l'utente il significato di conferente e produttore cambia
    if (!parseInt($(cIdSuperUserAccGerarchia).val())) {
        $("#lblConferente").text("Fornitore (P.Iva / Ragione Sociale)");
        $("#lblProduttore").text("Provenienza (P.Iva / Ragione Sociale)");
    }

    ddlEstrazioni.trigger("change"); // Aggiorno la ui in base al primo valore assunto di default dalla select

    $("#btnEseguiEstrazione").click(GestioneEstrazioni);

    $(".boxDate, .boxStampeMassive, .boxCentriMagazzini, .boxSoggetti, .boxProdotti").show(); // Mostro i box contenenti i vari filtri, in quanto li inizializzo nascosti

    $("#divgridProdotto").hide();

    popolaGrigliaConferenti("grigliaConferenti");
    // Inizializzo le ddl seguenti come nascoste in quanto verranno gestite dalla grigliaConferenti
    abilitaPrimoCessionario(false);
    abilitaSecondoCessionario(false);
    abilitaProduttore(false);
});