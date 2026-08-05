//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            reportBioResx.push(readResxFile(resxSinglePath, "Filtro_ReportBiologico_jQueryDocReady.js"));
        });
    }

    elencoTipiAppezzamento = [
        { "Codice": 1, "Descrizione": TraduzioneMultiResx(reportBioResx, "Convenzionale", "Convenzionale") },
        { "Codice": 2, "Descrizione": TraduzioneMultiResx(reportBioResx, "InConversione", "In Conversione") },
        { "Codice": 3, "Descrizione": TraduzioneMultiResx(reportBioResx, "Biologico", "Biologico") },
    ];

    // Uhalid 02/25, il codice che crea il kendoButtonGroup era gia' wrappato in $(function () {codice}), cosi pero' alla fine del esecuzione del docready il codice non era ancora stato eseguito
    // visto che ho neccesita di eseguire del codice dopo che questo e' stato eseguito e non volendo fare modifiche che possono creare danni collaterali l ho riwrappato in una promise
    // e quando risolta mi permette di preselezionare la stampa con cui sono arrivato. Il bind e' alla fine del docReady in modo da garantire che anche la ddlEstrazione sia gia' stata configurata
    var promiseTipoOutput = new Promise((resolve, reject) => {
        $(function () {
            try {
                $("#TipoOutput").kendoButtonGroup({
                    //index: 0,
                    selection: "single",
                    select: GestioneTipoOutput
                });
                resolve(true);  
            } catch (error) {
                reject(error);  
            }
        });
    });

    $(".kendoTextBox").kendoTextBox();

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31),
        format: "d"
    });

    var today = new Date();
    set_data("dpDataInizio", kendo.date.firstDayOfMonth(today), null);
    set_data("dpDataFine", kendo.date.lastDayOfMonth(today), null);

    creaKendoSwitch(); // In questo modo creo una switch per ogni elemento che ha la classe "kendoSwitch"

    var kSwitchRegione = KendoSwitch("kSwitchRegione");
    kSwitchRegione.check(true);
    kSwitchRegione.bind("change", kSwitchRegioneChange);

    KendoSwitch("kSwitchSezioneA").check(true);
    KendoSwitch("kSwitchSezioneB").check(true);

    //$("[name='radioEstrazione']").change(radioEstrazioniChange);
    var kddlEstrazioni = creaKendoDropDownList("ddlEstrazioni", { read: ddlEstrazioniRead }, "Desc", "Value", null, null, null, false).data("kendoDropDownList");
    kddlEstrazioni.bind("change", ddlEstrazioniChange);
    //kddlEstrazioni.bind("dataBound", ddlEstrazioniDataBound); // NOTA: utilizzabile solo con il parametro autobind a false

    var kddlImprese = creaKendoDropDownList("ddlImprese", { read: ddlImpreseRead }, "rag_soc", "piva", null, null, null, false).data("kendoDropDownList");
    kddlImprese.bind("change", ddlImpreseChange);
    kddlImprese.one("dataBound", ddlImpreseDataBound);
    // Collegando questo evento, se l'utente effettua un filtro, vengono mantenute visibili le sole imprese trovate(*):
    // si ha il vantaggio che se le imprese erano molte, la ddl diviene più veloce, mentre
    // si ha lo svantaggio che l'utente ha meno voci suggerite,
    // ma può comunque cercare imprese nascoste
    // (*questo perché la ddl all'open da parte dell'utente riesegue una ricerca sul suo datasource con filtro vuoto e con la funzione ddlFiltering lo evito)
    kddlImprese.bind("filtering", ddlFiltering);

    var kddlCentriAz = creaKendoDropDownList("ddlCentriAziendali", { read: ddlCentriAziendaliRead }, "sa_nome", "sa_cod", null, null, null, false).data("kendoDropDownList");
    kddlCentriAz.bind("change", ddlCentriAziendaliChange);
    //kddlCentriAz.one("dataBound", ddlCentriAziendaliDataBound);

    var kddlMagazzini = creaKendoDropDownList("ddlMagazzini", { read: ddlMagazziniRead }, "Ubic_Des", "Id_Destinazione", null, null, null, false).data("kendoDropDownList");
    kddlMagazzini.bind("dataBound", ddlMagazziniDataBound);

    creaKendoMultiselect("msCategorieMagazzino", { read: msCategorieMagazzinoRead }, "NomeComune", "Elem_Cod"); // Manca la possibilità di impostare l'autobind a false
    creaKendoMultiselect("msTipoAppezzamento", { read: msTipiAppezzamento }, "Descrizione", "Codice"); 

    var kddlClassiProdotto = creaKendoDropDownList("ddlClassiProdotto", { read: ddlClassiProdottoRead }, "Linea_Classe_Des", "Linea_Classe_Cod", null, null, null, false).data("kendoDropDownList");
    kddlClassiProdotto.bind("change", ddlClassiProdottoChange);

    var kddlArrotondamenti = creaKendoDropDownList("ddlArrotondamenti", { read: ddlArrotondamentiRead }, "Desc", "Value", null, null, null, false).data("kendoDropDownList");
    kddlArrotondamenti.one("dataBound", ddlArrotondamentiDataBound);

    creaKendoDropDownList("ddlStampeLotto", { read: ddlStampeLottoRead }, "Desc", "Value");
    creaKendoDropDownListServerFiltering("ddlContatti", "Rag_Soc_Completa", "Cod_RisUm", ddlContattiRead, ddlContattiChange, 3, "", "", null)
        .bind("filtering", ddlFiltering);
    creaKendoDropDownListServerFiltering("ddlProdotti", "Prodotto_Des", "Prodotto_Cod", ddlProdottiRead, ddlProdottiChange, 3, "", "", null)
        .bind("filtering", ddlFiltering);

    creaKendoMultiselectServerFiltering("msFornitore", { read: msFornitoriRead }, 3, "Rag_Soc", "cod_risum");
    KendoMultisel("msFornitore").bind("filtering", ddlFiltering);

    var kddlRegioni = creaKendoDropDownList("ddlRegioni", { read: ddlRegioniRead }, "Regione_Des", "REG", null, null, null, false).data("kendoDropDownList");
    kddlRegioni.one("dataBound", ddlRegioniDataBound);
    KendoDDL("ddlRegioni").dataSource.read(); // In questo modo, tramite il dataBound seleziono per prima la regione fittizia

    var kddlTipiMovimenti = creaKendoDropDownList("ddlTipiMovimenti", { read: ddlTipiMovimentiRead }, "Desc", "Value").data("kendoDropDownList");
    kddlTipiMovimenti.bind("change", ddlTipiMovimentiChange);
    var kddlOrigineDatiCauScarichi = creaKendoDropDownList("ddlOrigineDatiCauScarichi", { read: ddlOrigineDatiCauScarichiRead }, "Desc", "Value").data("kendoDropDownList");
    kddlOrigineDatiCauScarichi.one("dataBound", ddlOrigineDatiCauScarichiDataBound);

    creaKendoMultiselectServerFiltering("msProdotti", { read: msProdottiRead }, 3, "Prodotto_Des", "Prodotto_Cod"); // La funzione non restituisce un oggetto
    KendoMultisel("msProdotti").bind("filtering", ddlFiltering);
    creaKendoDropDownList("ddlLivelliDettaglio", { read: ddlLivelliDettaglioRead }, "Desc", "Value");

    var kddlSemilav = creaKendoDropDownList("ddlSemilavoratiTrasformati", { read: ddlSemilavoratiTrasformatiRead }, "Desc", "Value", null, null, null, false).data("kendoDropDownList");
    kddlSemilav.one("dataBound", ddlSemilavoratiTrasformatiDataBound);
    kddlSemilav.bind("change", ddlSemilavoratiTrasformatiChange);
    var kddlPreparazioni = creaKendoDropDownList("ddlPreparazioni", { read: ddlPreparazioniRead }, "Desc", "Value", null, null, null, false).data("kendoDropDownList");
    kddlPreparazioni.bind("dataBound", ddlPreparazioniDataBound);

    // Lancio manualmente delle read con autobind a false, per poter sfruttare l'evento dataBound
    kddlImprese.dataSource.read();
    kddlArrotondamenti.dataSource.read();

    $(".searchArea").show();

    var mailTo = $(cIdMailTo).val();
    var subjectMailAssistenza = "Segnalazione: fitofarmaci/fertilizzanti biologici ma visualizzati nella scheda materie prime come convenzionali"; // i18n ?
    var mailToAssistenza = kendo.format("mailto:{0}?subject={1}", mailTo, subjectMailAssistenza);
    $("#anchorAssistenza").prop("href", mailToAssistenza);

    //eventi di click pulsanti
    $("#btn_ricerca").click(GestioneEstrazioni);

    if (cReportSelezionato == enumTipiReport.SchedaMateriePrime || cReportSelezionato == enumTipiReport.SchedaVendite) {
        promiseTipoOutput
            .then(() => {
                let tipoOutput = $("#TipoOutput").data("kendoButtonGroup");
                tipoOutput.select(0); 
                tipoOutput.trigger('select');
                kddlEstrazioni.value(cReportSelezionato);
                kddlEstrazioni.trigger("change");
            })
            .catch((error) => {
                console.error("Failed to initialize TipoOutput:", error);
            });
    }

    $.logThis("DocReady: FINE");

});