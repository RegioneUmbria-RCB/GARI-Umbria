var indirizzohttp = "./Filtro_ReportBiologico.aspx";

function LeggiRegioneXCentroAziendale(pivaImpresa, saCod) {
    var codiceRegione = "000"; // Codice di regione 'Non Definita'
    saCod = saCod == -1 ? 0 : parseInt(saCod);
    if (pivaImpresa !== "" && saCod !== 0) {

        var params = {
            piva: pivaImpresa,
            sa_Cod: parseInt(saCod)
        };

        ajaxAgronicaSync(
            indirizzohttp + "/LeggiRegioneXCentroAziendale",
            kendo.stringify(params),
            false,
            function (risposta) {
                codiceRegione = risposta.RispostaStringa;
            },
            function (risposta) {
                if (typeof risposta === "string") {
                    // Errore js
                    MessaggioErrore_Bootstrap(
                        "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'ReportBio.aspx/LeggiRegioneXCentroAziendale', " +
                        "controllare i parametri passati: " +
                        risposta,
                        "DIV_Messaggi");
                } else {
                    // Errore lato server
                    MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
                }
            },
            null,
            false
        );
    }

    return codiceRegione;
}

function LeggiClassiProdotto(pivaImpresa) {
    var classProductsList = [];

    var params = {
        piva: pivaImpresa
    };

    ajaxAgronicaSync(
        indirizzohttp + "/LeggiClassiProdotto",
        kendo.stringify(params),
        false,
        function (risposta) {
            try {
                classProductsList = JSON.parse(risposta.RispostaStringa);

            } catch (e) {
                MessaggioTuttoOK_Bootstrap(e.message + " - " + risposta.RispostaStringa, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'ReportBio.aspx/LeggiClassiProdotto', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }
        },
        null,
        false
    );

    return classProductsList;
}

function LeggiMateriePrimeReport(pivaImpresa) {
    var productsList = [];

    var params = {
        piva: pivaImpresa
    };

    ajaxAgronicaSync(
        indirizzohttp + "/LeggiMateriePrimeReport",
        kendo.stringify(params),
        false,
        function (risposta) {
            try {
                productsList = JSON.parse(risposta.RispostaStringa);

            } catch (e) {
                MessaggioTuttoOK_Bootstrap(e.message + " - " + risposta.RispostaStringa, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'ReportBio.aspx/LeggiMateriePrimeReport', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }
        },
        null,
        false
    );

    return productsList;
}


function LeggiLineeProduzioniPreparazioni(pivaImpresa, matCodScelto) {
    var productsList = [];

    var params = {
        piva: pivaImpresa,
        matCod: parseInt(matCodScelto)
    };

    ajaxAgronicaSync(
        indirizzohttp + "/LeggiLineeProduzioniPreparazioni",
        kendo.stringify(params),
        false,
        function (risposta) {
            try {
                productsList = JSON.parse(risposta.RispostaStringa);

            } catch (e) {
                MessaggioTuttoOK_Bootstrap(e.message + " - " + risposta.RispostaStringa, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'ReportBio.aspx/LeggiLineeProduzioniPreparazioni', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }
        },
        null,
        false
    );

    return productsList;
}

function LeggiCategorieMagazzinoBIO() {
    var categoriesList = [];

    var params = {
        estrazioneScelta: Get_KendoDDLValue("ddlEstrazioni", 0)
    };

    ajaxAgronicaSync(
        indirizzohttp + "/LeggiCategorieMagazzinoBIO",
        kendo.stringify(params),
        false,
        function (risposta) {
            try {
                categoriesList = JSON.parse(risposta.RispostaStringa);

            } catch (e) {
                MessaggioTuttoOK_Bootstrap(e.message + " - " + risposta.RispostaStringa, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'ReportBio.aspx/LeggiCategorieMagazzinoBIO', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }
        },
        null,
        false
    );

    return categoriesList;
}

function LeggiFornitori() {
    var fornitori = [];
    var params = {
        piva: Get_KendoDDLValue("ddlImprese", ""),
        dataInizio: get_data("dpDataInizio"),
        dataFine: get_data("dpDataFine"),
        centro: Get_KendoDDLValue("ddlCentriAziendali", 0),
        prodotti: Get_MultiselString("msProdotti")
    };

    ajaxAgronicaSync(
        indirizzohttp + "/LeggiFornitori",
        kendo.stringify(params),
        false,
        function (risposta) {
            try {
                fornitori = JSON.parse(risposta.RispostaStringa);

            } catch (e) {
                MessaggioTuttoOK_Bootstrap(e.message + " - " + risposta.RispostaStringa, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'ReportBio.aspx/LeggiCategorieMagazzinoBIO', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }
        },
        null,
        false
    );

    return fornitori;
}

function EseguiEstrazione() {
    var ddlRegioniVal = KendoDDL("ddlRegioni").wrapper.children("." + GIAS_K_STATE_DISABLED).length === 0 ? Get_KendoDDLValue("ddlRegioni", "000") : "000";
    var ddlOrigineDatiCauScarichiVal = KendoDDL("ddlOrigineDatiCauScarichi").wrapper.children("." + GIAS_K_STATE_DISABLED).length === 0 ? Get_KendoDDLValue("ddlOrigineDatiCauScarichi", 0) : 0;
    var ddlProdottiVal = KendoDDL("ddlProdotti").wrapper.children("." + GIAS_K_STATE_DISABLED).length === 0 ? Get_KendoDDLValue("ddlProdotti", 0) : 0;

    var ddlTipiMovimentiArr = [];
    var ddlTipiMovimentiVal = Get_KendoDDLValue("ddlTipiMovimenti", "");
    if (ddlTipiMovimentiVal === "-1") {
        ddlTipiMovimentiArr.push("" + enumAgendaCausali.carico);
        ddlTipiMovimentiArr.push("" + enumAgendaCausali.scarico);
    }
    else {
        ddlTipiMovimentiArr.push(ddlTipiMovimentiVal);
    }

    var objFilters = {
        hdPiva: $(cIdPiva).val(),
        tipoOutput: $("#TipoOutput").data("kendoButtonGroup").current().index(),
        ddlEstrazioni: Get_KendoDDLValue("ddlEstrazioni", 0),
        ddlImprese: Get_KendoDDLValue("ddlImprese", ""),
        ddlCentriAziendali: Get_KendoDDLValue("ddlCentriAziendali", 0),
        ddlMagazzini: Get_KendoDDLValue("ddlMagazzini", ""),
        dpDataInizio: get_data("dpDataInizio"),
        dpDataFine: get_data("dpDataFine"),
        msCategorieMagazzino: KendoMultisel("msCategorieMagazzino").value(),
        msTipoAppezzamento: KendoMultisel("msTipoAppezzamento").value(),
        ddlClassiProdotto: Get_KendoDDLValue("ddlClassiProdotto", 0),
        ddlArrotondamenti: Get_KendoDDLValue("ddlArrotondamenti", 0),
        ddlStampeLotto: Get_KendoDDLValue("ddlStampeLotto", 0),
        kSwitchCodiceArticolo: getKendoSwitch("kSwitchCodiceArticolo"),
        kSwitchConsistenzaVasca: getKendoSwitch("kSwitchConsistenzaVasca"),
        ddlProdotti: ddlProdottiVal,
        ddlContatti: Get_KendoDDLValue("ddlContatti", 0),
        ddlRegioni: ddlRegioniVal,
        kSwitchLogoRegione: getKendoSwitch("kSwitchLogoRegione"),
        ddlSemilavoratiTrasformati: Get_KendoDDLValue("ddlSemilavoratiTrasformati"),
        ddlPreparazioni: Get_KendoDDLValue("ddlPreparazioni"),
        kSwitchSezioneA: getKendoSwitch("kSwitchSezioneA"),
        kSwitchSezioneB: getKendoSwitch("kSwitchSezioneB"),
        ddlTipiMovimenti: ddlTipiMovimentiArr, // Valore di default da verificare
        ddlOrigineDatiCauScarichi: ddlOrigineDatiCauScarichiVal,
        kSwitchGiacenzeMagazzino: getKendoSwitch("kSwitchGiacenzeMagazzino"),
        msProdotti: Get_MultiselString("msProdotti"), // Verificare se occorre passare il parametro come stringa, come in questo caso oppure come array di stringhe, con: KendoMultisel("msProdotti").value()
        ddlLivelliDettaglio: Get_KendoDDLValue("ddlLivelliDettaglio", 0),
        msFornitore: Get_MultiselString("msFornitore"),
        kSwitchMostraFirmaODC: getKendoSwitch("kSwitchMostraFirmaODC"),
        kSwitchMostraDataStampa: getKendoSwitch("kSwitchMostraDataStampa"),

        //-------- Dati aggiuntivi oltre ai value degli elementi html:

        //conferentePiva: KendoDDL("ddlConferenti").dataSource.data().length && KendoDDL("ddlConferenti").dataItem().Partita_Iva ?
        //    KendoDDL("ddlConferenti").dataItem().Partita_Iva : "",

        semilavTrasfDes: KendoDDL("ddlSemilavoratiTrasformati").dataSource.data().length && KendoDDL("ddlSemilavoratiTrasformati").dataItem().Desc ?
            KendoDDL("ddlSemilavoratiTrasformati").dataItem().Desc : ""

        //-------  Fine dei dati aggiuntivi
    };

    //console.log(objFilters);

    ajaxAgronica(
        indirizzohttp + "/EseguiEstrazione",
        kendo.stringify({ params: kendo.stringify(objFilters) }),
        function (risposta) {
            try {
                if (risposta.RispostaStringa.IsLink === true) {
                    LanciaReport(risposta.RispostaStringa.Risposta);
                }
                else {
                    dataSourceGrid = JSON.parse(risposta.RispostaStringa.Risposta);

                    switch (risposta.RispostaStringa.TipoReport) {
                        case enumTipiReport.CarichiScarichi:
                            PopolaGrigliaCarichiScarichiBio();
                            break;

                        default:
                            MessaggioErrore_Bootstrap(TraduzioneMultiResx(reportBioResx, "ReportNonRiconosciuto", "Report non riconosciuto"), "DIV_Messaggi");
                            break;
                    }
                }

            } catch (e) {
                MessaggioErrore_Bootstrap(
                    TraduzioneMultiResx(reportBioResx, "ErroreDuranteOperazione_", "Errore durante l'operazione: ")
                    + risposta.RispostaStringa.Risposta,
                    "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'ReportBio.aspx/EseguiEstrazione', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                try {
                    var jsonArrErrori = JSON.parse(risposta.Errore);
                    var stringArrErrori = jsonArrErrori.join("<br/>");
                    MessaggioErrore_Bootstrap(
                        TraduzioneMultiResx(reportBioResx, "SonoStatiRilevatiISeguentiErrori_", "Sono stati rilevati i seguenti errori: ")
                        + "<br/>" + stringArrErrori,
                        "DIV_Messaggi");
                } catch (e) {
                    MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
                }
            }
        }
    );
}


function LanciaReport(urlDaChiamare) {
    window.open(
        urlDaChiamare /* + "?" + objReport.queryString*/,
        "_blank"
    );
}


function PopolaGrigliaCarichiScarichiBio() {
    // Mostro l'elemento contenitore della griglia per il corretto ridimensionamento della stessa
    $("#gridArea").show();

    // Creazione Griglia
    var funzioniCRUD = {
        funzioneRead: kGridCarichiScarichiBioRead,
        UtenteAbilitatoInserimentoModifica: false,
        UtenteAbilitatoCancellazione: false,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true
    };
    var idModel = ""; // Aggiungere
    var campiKendoModel = {
        Piva: { type: "string" },
        Tipo_Prodotto: { type: "string" },
        Tipo_Movimento: { type: "string" },
        Articolo: { type: "string" },
        Descrizione: { type: "string" },
        Classificazione: { type: "string" },
        OP: { type: "string" },
        Fornitore: { type: "number" },
        Fornitore_Ragione_Sociale: { type: "string" },
        DDTNumero: { type: "string" },
        DDTData: { type: "date" },
        DDTOra: { type: "date" },
        Fabbricato_Des: { type: "string" },
        Bio_Convers: { type: "string" },
        App_BIO: { type: "number" },
        Lotto: { type: "string" },
        Data: { type: "date" },
        Ora: { type: "date" }, // Verificare
        Qta: { type: "number" },
        Progetto: { type: "string" },
        Campo: { type: "string" },
        Descrizione_campo: { type: "string" },
        Sup: { type: "number" },
        Azoto: { type: "number" },
        Rame: { type: "number" },
        Fosforo: { type: "number" },
        Giacenza: { type: "number" },
        Matricola: { type: "string" },
        Gen_Cod: { type: "number" },
        Gen_Des: { type: "string" },
        Spe_Cod: { type: "number" },
        Spe_Des: { type: "string" },
        Raz_Cod: { type: "number" },
        Raz_Des: { type: "string" },
        Nome_Animale: { type: "string" },
        Rif_Esterno: { type: "string" },
        Rif_Esterno_2: { type: "string" },
    };
    var colonneKendoGrid = [
        { field: "Piva", title: TraduzioneMultiResx(reportBioResx, "PartitaIvaAbbr", "P. IVA"), filterable: { multi: true, search: true } },
        { field: "Tipo_Prodotto", title: TraduzioneMultiResx(reportBioResx, "TipoProdotto", "Tipo Prodotto"), filterable: { multi: true, search: true } },
        { field: "Tipo_Movimento", title: TraduzioneMultiResx(reportBioResx, "TipoMovimento", "Tipo Movimento"), filterable: { multi: true, search: true } },
        { field: "Articolo", title: TraduzioneMultiResx(reportBioResx, "Articolo", "Articolo"), filterable: { multi: true, search: true } },
        { field: "Descrizione", title: TraduzioneMultiResx(reportBioResx, "Descrizione", "Descrizione"), filterable: { multi: true, search: true } },
        { field: "Classificazione", title: TraduzioneMultiResx(reportBioResx, "Classificazione", "Classificazione"), filterable: { multi: true, search: true } },
        { field: "OP", title: TraduzioneMultiResx(reportBioResx, "OrganizzazioneProduttoreSigla", "OP") },
        { field: "Fornitore", title: TraduzioneMultiResx(reportBioResx, "Fornitore", "Fornitore"), filterable: { multi: true, search: true } },
        { field: "Fornitore_Ragione_Sociale", title: TraduzioneMultiResx(reportBioResx, "RagSocFornitore", "Ragione Sociale Fornitore"), filterable: { multi: true, search: true } },
        { field: "DDTNumero", title: TraduzioneMultiResx(reportBioResx, "NumeroDDT", "Numero DDT"), filterable: { multi: true, search: true } },
        {
            field: "DDTData",
            title: TraduzioneMultiResx(reportBioResx, "DataDDT", "Data DDT"),
            attributes: { style: "text-align:center;" },
            format: "{0:d}",
            groupHeaderTemplate: TraduzioneMultiResx(reportBioResx, "DataDDT", "Data DDT") + ": #= kendo.toString(value, 'd') #"
        },
        {
            field: "DDTOra",
            title: TraduzioneMultiResx(reportBioResx, "OraDDT", "Ora DDT"),
            format: "{0:T}", attributes: { style: "text-align:center;" },
            groupHeaderTemplate: TraduzioneMultiResx(reportBioResx, "OraDDT", "Ora DDT") + ": #= kendo.toString(value, 'd') #"
        },
        { field: "Fabbricato_Des", title: TraduzioneMultiResx(reportBioResx, "Fabbricato", "Fabbricato"), filterable: { multi: true, search: true } },
        { field: "Bio_Convers", title: TraduzioneMultiResx(reportBioResx, "TradizionaleInConversBiologico", "Trad/In Conv/Bio"), filterable: { multi: true, search: true } },
        { field: "App_BIO", title: "App BIO" }, //i18n
        { field: "Lotto", title: TraduzioneMultiResx(reportBioResx, "Lotto2", "Lotto"), filterable: { multi: true, search: true } },
        {
            field: "Data",
            title: TraduzioneMultiResx(reportBioResx, "Data", "Data"),
            format: "{0:d}",
            attributes: { style: "text-align:center;" },
            groupHeaderTemplate: TraduzioneMultiResx(reportBioResx, "Data", "Data") + ": #= kendo.toString(value, 'd') #"
        },
        {
            field: "Ora",
            title: TraduzioneMultiResx(reportBioResx, "Ora", "Ora"),
            format: "{0:T}",
            attributes: { style: "text-align:center;" },
            groupHeaderTemplate: TraduzioneMultiResx(reportBioResx, "Ora", "Ora") + ": #= kendo.toString(value, 'd') #"
        },
        { field: "Qta", title: TraduzioneMultiResx(reportBioResx, "Quantita", "Quantità"), format: "{0:n4}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
        { field: "Progetto", title: TraduzioneMultiResx(reportBioResx, "Progetto", "Progetto"), filterable: { multi: true, search: true } },
        { field: "Campo", title: TraduzioneMultiResx(reportBioResx, "CodiceCampo", "Codice Campo") },
        { field: "Descrizione_campo", title: TraduzioneMultiResx(reportBioResx, "Campo", "Campo"), filterable: { multi: true, search: true } },
        { field: "Sup", title: TraduzioneMultiResx(reportBioResx, "Superficie", "Superficie"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
        { field: "Azoto", title: TraduzioneMultiResx(reportBioResx, "Azoto", "Azoto"), format: "{0:n2}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
        { field: "Rame", title: TraduzioneMultiResx(reportBioResx, "Rame", "Rame"), format: "{0:n4}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
        { field: "Fosforo", title: TraduzioneMultiResx(reportBioResx, "Fosforo", "Fosforo"), format: "{0:n4}", attributes: { style: "text-align:right;" }, aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" },
        { field: "Matricola", title: TraduzioneMultiResx(reportBioResx, "Animale_Matricola", "Matricola"), filterable: { multi: true, search: true } },
        { field: "Nome_Animale", title: TraduzioneMultiResx(reportBioResx, "Animale_Nome", "Nome"), filterable: { multi: true, search: true } },
        { field: "Gen_Des", title: TraduzioneMultiResx(reportBioResx, "Animale_Genere", "Genere"), filterable: { multi: true, search: true } },
        { field: "Spe_Des", title: TraduzioneMultiResx(reportBioResx, "Animale_Specie", "Specie"), filterable: { multi: true, search: true } },
        { field: "Raz_Des", title: TraduzioneMultiResx(reportBioResx, "Animale_Razza", "Razza"), filterable: { multi: true, search: true } }

    ];
    if (getKendoSwitch("kSwitchGiacenzeMagazzino") === true) {
        colonneKendoGrid.push({ field: "Giacenza", title: TraduzioneMultiResx(reportBioResx, "Giacenza", "Giacenza"), format: "{0:n2}", attributes: { style: "text-align:right;" } });
    }

    var parametriPerLettura = [];
    var parametriDataSource = {
        serverFiltering: false,
        aggregate: [
            { field: "Qta", aggregate: "sum" },
            { field: "Sup", aggregate: "sum" },
            { field: "Azoto", aggregate: "sum" },
            { field: "Rame", aggregate: "sum" },
            { field: "Fosforo", aggregate: "sum" }
        ],
        pagesize: 30
    };

    var parametriKendoGrid = {
        excel: true,
        pdf: true,
        editable: false,
        groupable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [30, 50, 100] },
        reorderable: true
    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: kGridCarichiScarichiBioDataBound };

    creaKendoGrid(
        "gridEstrazioneBio", // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD, //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid, // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi // funzioni da chiamare all'inizio e alla fine dei vari eventi
    );
}

function kGridCarichiScarichiBioRead(options) {
    options.success(dataSourceGrid);
}

function kGridCarichiScarichiBioDataBound(e) {

    var gridId = e.sender.element[0].id;

    kendo_AggiustaDimensioneColonne("#" + gridId);

    var grid = $("#" + gridId).data("kendoGrid");

    var wrapperRigheTestata = grid.wrapper;
    var headerRigheTestata = wrapperRigheTestata.find(".k-grid-header");

    function resizeFixedRigheTestata() {
        var wrapperWidth = wrapperRigheTestata.width();
        if (wrapperWidth !== 0) {
            var paddingRight = parseInt(headerRigheTestata.css("padding-right"));
            headerRigheTestata.css("width", wrapperWidth - paddingRight);
        }
        else {
            // Nel caso l'evento venga eseguito mentre la griglia è nascosta, imposto una variabile per ricalcolare correttamente la width
            // alla prima occorrenza dell'evento "scroll" in quanto più frequente
            headerRigheTestata.css("width", "auto");
            headerRigheTestata.data("fix_width", 1);
        }
    }

    function scrollFixedRigheTestata() {
        // Nel caso l'evento venga eseguito mentre la griglia è nascosta, rimuovo la classe
        var wrapperHeight = wrapperRigheTestata.outerHeight();
        if (headerRigheTestata.data("fix_width") === 1 && wrapperHeight !== 0) {
            var paddingRight = parseInt(headerRigheTestata.css("padding-right"));
            headerRigheTestata.css("width", wrapperRigheTestata.width() - paddingRight);
            headerRigheTestata.data("fix_width", 0);
        }

        var headerHeight = $('#headerDashboard').outerHeight();
        var headerGroupHeight = $('.k-grouping-header').outerHeight();
        var headerWidth = $('#headerDashboard').outerWidth();

        if (headerWidth < 997) {
            var offset = $(this).scrollTop(),
                tableOffsetTop = wrapperRigheTestata.offset().top + headerGroupHeight,
                tableOffsetBottom = tableOffsetTop + wrapperHeight - headerRigheTestata.height();
        } else {
            var offset = $(this).scrollTop(),
                tableOffsetTop = wrapperRigheTestata.offset().top + headerHeight + headerGroupHeight,
                tableOffsetBottom = tableOffsetTop + wrapperHeight - headerRigheTestata.height();
        }

        if (offset < tableOffsetTop || offset > tableOffsetBottom || wrapperHeight === 0) {
            headerRigheTestata.removeClass("fixed-header");
        } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !headerRigheTestata.hasClass("fixed")) {
            headerRigheTestata.addClass("fixed-header");
        }

    }

    resizeFixedRigheTestata();

    $(window).resize(resizeFixedRigheTestata);
    $(window).scroll(scrollFixedRigheTestata);

}