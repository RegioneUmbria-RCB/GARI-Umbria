
function GestioneTipoOutput() {
    var kddlEstrazioni = KendoDDL("ddlEstrazioni");
    kddlEstrazioni.dataSource.read();
    // NOTA: Nel caso in cui, cambiando il codice le due seguenti istruzioni non vengano eseguite all'apertura della pagina,
    // riabilitare l'evento dataBound alla ddl dell'elenco delle estrazioni disponibili
    kddlEstrazioni.select(0);
    kddlEstrazioni.trigger("change");
}

function GestioneEstrazioni(e) {
    var prosegui = Get_KendoDDLValue("ddlEstrazioni") !== "";
    //$("[name='radioEstrazione']").each(function () {
    //    if ($(this).prop("checked") === true) {
    //        prosegui = true;
    //    }
    //});

    if (prosegui) {
        EseguiEstrazione();
    }
    else {
        MessaggioErrore_Bootstrap(TraduzioneMultiResx(reportBioResx, "SelezionareUnaEstrazione", "Selezionare un'estrazione"), "DIV_Messaggi");
    }
}

function msFornitoriRead(options) {
    var fornitori = [];

    fornitori = LeggiFornitori();
    options.success(fornitori);
}

function GenericOption(value, desc) {
    this.Value = value;
    this.Desc = desc;
}

function ddlFiltering(e) {
    var filter = e.filter;

    if (filter === undefined || filter === null || !filter.value || filter.value.length < e.sender.minLength) {
        e.preventDefault();
    }
}

function ddlEstrazioniRead(options) {
    //console.log("ddlEstrazioni read");
    var extractionsList = [ /*new GenericOption("", "")*/];
    switch ($("#TipoOutput").data("kendoButtonGroup").current().index()) {
        case 0: // Report
            extractionsList.push(new GenericOption(enumTipiReport.SchedaMateriePrime, TraduzioneMultiResx(reportBioResx, "SchedaMateriePrime", "Scheda Materie Prime")));
            extractionsList.push(new GenericOption(enumTipiReport.SchedaVendite, TraduzioneMultiResx(reportBioResx, "SchedaVendite", "Scheda Vendite")));
            extractionsList.push(new GenericOption(enumTipiReport.RegistroPreparazioni, TraduzioneMultiResx(reportBioResx, "RegistroPreparazioni", "Registro Preparazioni")));
            extractionsList.push(new GenericOption(enumTipiReport.ReportTerzisti, TraduzioneMultiResx(reportBioResx, "ReportTerzisti", "Report Terzisti")));
            break;

        case 1: // Griglia
            extractionsList.push(new GenericOption(enumTipiReport.CarichiScarichi, TraduzioneMultiResx(reportBioResx, "CarichiScarichi", "Carichi/Scarichi")));
            break;

        default:
            break;
    }
    options.success(extractionsList);
}

function ddlEstrazioniDataBound(e) {
    e.sender.select(0);
    e.sender.trigger("change");
}

function ddlEstrazioniChange(e) {
    var estrScelta = 0;
    if (e.sender.dataItem() !== null && e.sender.dataItem() !== undefined) {
        estrScelta = parseInt(e.sender.value());
    }

    // L'elemento contenitore delle griglie viene mostrato all'inizio delle funzioni di creazione delle stesse, es: PopolaGrigliaCarichiScarichiBio()
    $("#gridArea").hide();

    MostraNascondiFiltri(estrScelta);

    // Carico la multi select dei magazzini in quanto la ricerca effettuata ed i valori di default sono diversi in base all'estrazione selezionata
    switch (estrScelta) {

        case enumTipiReport.SchedaMateriePrime:
            KendoMultisel("msCategorieMagazzino").dataSource.read();
            Set_MultiselValue("msCategorieMagazzino", categorieMatPrimeDefault);
            break;

        case enumTipiReport.SchedaVendite:
            KendoMultisel("msCategorieMagazzino").dataSource.read();
            Set_MultiselValue("msCategorieMagazzino", categorieVenditeDefault);
            break;

        case enumTipiReport.CarichiScarichi:
            KendoMultisel("msCategorieMagazzino").dataSource.read();
            Set_MultiselValue("msCategorieMagazzino", categorieCarichiScarichiDefault);
            KendoDDL("ddlTipiMovimenti").trigger("change");
            break;

        default:
            // Deseleziono tutto
            KendoMultisel("msCategorieMagazzino").dataSource.read();
            Set_MultiselValue("msCategorieMagazzino", null);
            break;
    }
}

function MostraNascondiFiltri(codEstrazione) {
    var objReportFilter = bindReportFilterAlt[codEstrazione];
    if (objReportFilter !== undefined) {
        $(".boxFiltriRicerca").show();

        for (var prop in objReportFilter) { //restituisce il nome delle proprietà dell'oggetto
            $("." + prop).toggle(objReportFilter[prop]);
        }
        if (codEstrazione == 21 || codEstrazione == 22) {
            setKendoSwitch("kSwitchMostraDataStampa", cFLagDefaultMostraData);
            setKendoSwitch("kSwitchMostraFirmaODC", cFlagMostraFirmaODC);
        } else {
            setKendoSwitch("kSwitchMostraDataStampa", false);
            setKendoSwitch("kSwitchMostraFirmaODC", false);
        }

    } else {
        $(".boxFiltriRicerca").hide();
    }
}

function ddlImpreseRead(options) {
    // Tramite la funzione slice ottengo una copia dell'array globale che la funzione RicercaImprese restituisce per riferimento
    var companiesList = RicercaImprese(false).slice();
    companiesList.unshift({
        piva: "",
        rag_soc: ""
    });
    options.success(companiesList);
}

function ddlImpreseDataBound(e) {
    // Utilizzo il metodo select al posto di value perché in questo modo non causo una ri-esecuzione dell'evento dataBound stesso,
    // al netto del caso specifico per il quale ho collegato l'evento al controllo con la funzione "one"
    e.sender.select(function (dataItem) {
        return dataItem.piva === $(cIdPiva).val();
    });
    e.sender.trigger("change");
}

function ddlImpreseChange(e) {
    if (KendoDDL("ddlMagazzini") !== undefined) {
        KendoDDL("ddlMagazzini").dataSource.data([]); // Prima azzero i magazzini
    }
    if (KendoDDL("ddlCentriAziendali") !== undefined) {
        var kddlCentriAz = KendoDDL("ddlCentriAziendali");
        kddlCentriAz.dataSource.read();
        // NOTA: Nel caso in cui, cambiando il codice le due seguenti istruzioni non vengano eseguite all'apertura della pagina,
        // riabilitare l'evento dataBound alla ddl dei centri aziendali
        kddlCentriAz.select(0);
        kddlCentriAz.trigger("change");
    }

    if (KendoDDL("ddlClassiProdotto") !== undefined) {
        KendoDDL("ddlClassiProdotto").dataSource.read();
    }
    if (KendoDDL("ddlSemilavoratiTrasformati") !== undefined) {
        KendoDDL("ddlSemilavoratiTrasformati").dataSource.read();
    }
}

function ddlCentriAziendaliRead(options) {
    var businessList = [];
    var pivaSel = Get_KendoDDLValue("ddlImprese", "");
    if (pivaSel !== "") {
        elencoCentriAziendali = null; // Forzo la rilettura dei centri.
        businessList = RicercaCentriAziendali(
            pivaSel,
            true,
            2,
            false
        );
        var businessCount = businessList.length;
        if (businessCount === 0) {
            var objVuoto = {
                "sa_cod": 0,
                "sa_nome": TraduzioneMultiResx(reportBioResx, "NessunCentroAziendalePresente", "Nessun centro aziendale presente")
            };
            businessList.unshift(objVuoto);
        }
        else {
            if (businessCount > 1) {
                var objVuoto = {
                    "sa_cod": 0,
                    "sa_nome": TraduzioneMultiResx(reportBioResx, "Tutti", "Tutti")
                };
                businessList.unshift(objVuoto);
            }
        }
        
    }
    options.success(businessList);
}

function ddlCentriAziendaliDataBound(e) {
    var elemToSelect = 0;
    // Codice per selezionare il primo centro aziendale in ordine, al posto dell'opzione "Tutti"
    //if (e.sender.dataSource.data().length > 1) {
    //    elemToSelect = 1;
    //}
    e.sender.select(elemToSelect);
    e.sender.trigger("change");
}

function ddlCentriAziendaliChange(e) {
    KendoDDL("ddlMagazzini").dataSource.read();
    SelezionaRegionePerCentroAz();
}

function ddlMagazziniRead(options) {
    var warehouseList = [];
    var pivaSel = Get_KendoDDLValue("ddlImprese", "");
    var centroAzSel = Get_KendoDDLValue("ddlCentriAziendali", 0);
    if (pivaSel !== "") {
        elencoMagazzini = null;
        warehouseList = RicercaMagazzini(
            false,
            -999,
            pivaSel,
            centroAzSel,
            false
        );
        var warehouseCount = warehouseList.length;
        if (warehouseCount === 0) {
            var objVuoto = {
                "key_Dest": "0",
                "Tipo_Destinazione": 0,
                "Sa_Cod": 0,
                "Id_Destinazione": 0,
                "Ubic_Des": TraduzioneMultiResx(reportBioResx, "NessunMagazzinoPresente", "Nessun magazzino presente")
            };
            warehouseList.unshift(objVuoto);
        }
        else {
            if (warehouseCount > 1) {
                var objVuoto = {
                    "key_Dest": "0",
                    "Tipo_Destinazione": 0,
                    "Sa_Cod": 0,
                    "Id_Destinazione": 0,
                    "Ubic_Des": TraduzioneMultiResx(reportBioResx, "Tutti", "Tutti")
                };
                warehouseList.unshift(objVuoto);
            }
        }

    }
    options.success(warehouseList);
}

function ddlMagazziniDataBound(e) {
    e.sender.select(0);
}

/*async*/ function msCategorieMagazzinoRead(options) {
    var categoriesList = [];
    //categoriesList = await Ricerca_Categorie_Magazzino_Async(true);
    categoriesList = LeggiCategorieMagazzinoBIO();
    options.success(categoriesList);
}

function msTipiAppezzamento(options)
{
    options.success(elencoTipiAppezzamento);
}

function ddlClassiProdottoRead(options) {
    var classList = [];
    var pivaImpresa = Get_KendoDDLValue("ddlImprese", "");
    if (pivaImpresa !== "") {
        classList = LeggiClassiProdotto(pivaImpresa);
        var classCount = classList.length;
        if (classCount === 0) {
            var objVuoto = {
                "Linea_Classe_Cod": 0,
                "Linea_Classe_Des": TraduzioneMultiResx(reportBioResx, "NessunaClassePresente", "Nessuna classe presente"),
                "Linea_Classe_Padre_Cod": -1,
                "Piva": "",
                "Tipo_Classe": 0,
                "Tipo_Produzione": 0
            };
            classList.unshift(objVuoto);
        }
        if (classCount > 0) {
            var objVuoto = {
                "Linea_Classe_Cod": 0,
                "Linea_Classe_Des": TraduzioneMultiResx(reportBioResx, "Nessuno", "Nessuno"),
                "Linea_Classe_Padre_Cod": -1,
                "Piva": "",
                "Tipo_Classe": 0,
                "Tipo_Produzione": 0
            };
            classList.unshift(objVuoto);
        }
    }
    options.success(classList);
}

function ddlClassiProdottoChange(e) {
    if (parseInt(e.sender.value()) !== 0) {
        KendoDDL("ddlProdotti").enable(false);
    }
    else {
        KendoDDL("ddlProdotti").enable(true);
    }
}

function ddlArrotondamentiRead(options) {
    var numbersList = [
        new GenericOption(-1, TraduzioneMultiResx(reportBioResx, "Nessuno", "Nessuno")),
        new GenericOption(0, TraduzioneMultiResx(reportBioResx, "Unita", "Unità")),
        new GenericOption(1, TraduzioneMultiResx(reportBioResx, "UnDecimale", "1 Decimale")),
        new GenericOption(2, kendo.format(TraduzioneMultiResx(reportBioResx, "NDecimali", "{0} Decimali"), 2)),
        new GenericOption(3, kendo.format(TraduzioneMultiResx(reportBioResx, "NDecimali", "{0} Decimali"), 3)),
        new GenericOption(4, kendo.format(TraduzioneMultiResx(reportBioResx, "NDecimali", "{0} Decimali"), 4)),
    ];
    options.success(numbersList);
}

function ddlArrotondamentiDataBound(e) {
    e.sender.value(3);
}

function ddlStampeLottoRead(options) {
    var numbersList = [
        new GenericOption(0, TraduzioneMultiResx(reportBioResx, "StampaSempreIlLotto", "Stampa sempre il lotto")),
        new GenericOption(1, TraduzioneMultiResx(reportBioResx, "StampaDaConfigurazioneProdotto", "Stampa in base alla configurazione del prodotto")),
    ];
    options.success(numbersList);
}


/*async*/ function ddlContattiRead(options) {
    var subjectsList = [];
    var pivaImpresa = Get_KendoDDLValue("ddlImprese", "");
    if (pivaImpresa !== "") {
        var tipoRapporto = 0; // Il codice 0 permette di prelevare i soggetti che sono considerati o clienti o terzisti
        //subjectsList = await RicercaContatti(true, $(cIdPiva).val(), false, false, false, false, false, false, false, false);
        elencoContatti[tipoRapporto] = null; // Forzo la rilettura dei contatti.
        subjectsList = RicercaContattiDocumentoSync(
            true,
            pivaImpresa,
            tipoRapporto,
            true,
            true,
            false,
            0,
            "",
            JSON.stringify(options.data.filter.filters), // L'effettivo filtro di ricerca scritto dall'utente
            0,
            "",
            false
        );
    }
    options.success(subjectsList);
}

function ddlContattiChange(e) {

}

function ddlProdottiRead(options) {
    var productsList = [];

    var pivaImpresa = Get_KendoDDLValue("ddlImprese", "");

    if (pivaImpresa !== "") {
        var soloInGiacenza = false;
        var Sa_Cod = 0;
        var Fabbricato_Cod = 0;
        var TipoDestinazione = 0;
        var Cau_Mov = "7300";
        var xPUARegolamento = 0;
        var xLottoAccettazione = "";
        var Data_Movimento = formattedDate();
        var Flag_QtaNoZero = false;
        var xTipoPUARegolamento = 0;

        var arrElemCod = KendoMultisel("msCategorieMagazzino").value();
        var arrSpecie = [];
        var arrVarieta = [];

        var elencoProdottiCompleto = RicercaElencoCompletoProdottiMultiCategoria(objP_super_server, objP_server, objP_utenti, pivaImpresa,
            Sa_Cod, Fabbricato_Cod, TipoDestinazione, arrElemCod, arrSpecie, arrVarieta, soloInGiacenza, JSON.stringify(options.data.filter.filters),
            "", Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, false, Flag_QtaNoZero, xTipoPUARegolamento, null, null, "");
        productsList = JSON.parse(elencoProdottiCompleto);
    }

    options.success(productsList);
}

function ddlProdottiChange(e) {

}

function ddlRegioniRead(options) {
    options.success(LeggiRegioni(false));
}

function ddlRegioniDataBound(e) {
    e.sender.value("000");
}

function kSwitchRegioneChange(e) {
    if (e.sender.check() === true) {
        KendoDDL("ddlRegioni").enable(true);
        KendoSwitch("kSwitchLogoRegione").enable(true);
    } else {
        KendoDDL("ddlRegioni").enable(false);
        KendoSwitch("kSwitchLogoRegione").enable(false);
    }
}

function SelezionaRegionePerCentroAz() {
    var codiceRegione = LeggiRegioneXCentroAziendale(
        Get_KendoDDLValue("ddlImprese", ""),
        Get_KendoDDLValue("ddlCentriAziendali", 0)
    );
    Set_KendoDDLValue("ddlRegioni", codiceRegione, "000");
}

function ddlSemilavoratiTrasformatiRead(options) {
    var list = [];
    var pivaImpresa = Get_KendoDDLValue("ddlImprese", "");
    if (pivaImpresa !== "") {
        list = LeggiMateriePrimeReport(pivaImpresa);
        if (list.length === 0) {
            var objVuoto = {
                "Desc": TraduzioneMultiResx(reportBioResx, "NessunProdottoPresenteImpossibileStampareReport", "Nessun prodotto presente, impossibile stampare il report"),
                "Value": ""
            };
            list.unshift(objVuoto);
        }
    }
    options.success(list);
}

function ddlSemilavoratiTrasformatiDataBound(e) {
    e.sender.select(0);
    e.sender.trigger("change");
}

function ddlSemilavoratiTrasformatiChange(e) {
    KendoDDL("ddlPreparazioni").dataSource.read();
}

function ddlPreparazioniRead(options) {
    var list = [];
    var pivaImpresa = Get_KendoDDLValue("ddlImprese", "");
    var valSemilavoratiTrasf = Get_KendoDDLValue("ddlSemilavoratiTrasformati", "");
    var matCod = valSemilavoratiTrasf.split("|")[0];
    if (pivaImpresa !== "" && matCod !== "") {
        list = LeggiLineeProduzioniPreparazioni(pivaImpresa, matCod);
        if (list.length === 0) {
            var objVuoto = {
                "Desc": TraduzioneMultiResx(reportBioResx, "NessunaLineaDiProduzioneImpossibileStampareReport", "Nessuna linea di produzione o preparazione presente, impossibile stampare il report"),
                "Value": ""
            };
            list.unshift(objVuoto);
        }
    }
    options.success(list);
}

function ddlPreparazioniDataBound(e) {
    e.sender.select(0);
}

function ddlTipiMovimentiRead(options) {
    var movList = [
        new GenericOption(enumAgendaCausali.carico, TraduzioneMultiResx(reportBioResx, "SoloCarichi", "Solo Carichi")),
        new GenericOption(enumAgendaCausali.scarico, TraduzioneMultiResx(reportBioResx, "SoloScarichi", "Solo Scarichi")),
        new GenericOption(-1, TraduzioneMultiResx(reportBioResx, "Entrambi", "Entrambi")),
    ];
    options.success(movList);
}

function ddlTipiMovimentiChange(e) {
    var kddlOrigineDatiCauScarichi = KendoDDL("ddlOrigineDatiCauScarichi");
    switch (parseInt(e.sender.value())) {
        case enumAgendaCausali.scarico:
        case -1:
            kddlOrigineDatiCauScarichi.enable(true);
            kddlOrigineDatiCauScarichi.wrapper.parent().show();
            break;

        default:
            kddlOrigineDatiCauScarichi.enable(false);
            kddlOrigineDatiCauScarichi.wrapper.parent().hide();
            break;
    }
}

function ddlOrigineDatiCauScarichiRead(options) {
    var optionsList = [
        new GenericOption(0, TraduzioneMultiResx(reportBioResx, "Tutti", "tutti")),
        new GenericOption(1, TraduzioneMultiResx(reportBioResx, "SoloScarichiQdC", "Solo scarichi Quaderno di Campagna")),
        new GenericOption(2, TraduzioneMultiResx(reportBioResx, "SoloScarichiCdG", "Solo scarichi inseriti direttamente sul CdG"))
    ];
    options.success(optionsList);
}

function ddlOrigineDatiCauScarichiDataBound(e) {
    e.sender.select(1);
}

function msProdottiRead(options) {
    var productsList = [];

    var pivaImpresa = Get_KendoDDLValue("ddlImprese", "");
    if (pivaImpresa !== "") {
        var Elem_Cod = 700; // Servizi professionali
        var soloInGiacenza = false;
        var Sa_Cod = 0;
        var Fabbricato_Cod = 0;
        var TipoDestinazione = 0;
        var Cau_Mov = "7300";
        var xPUARegolamento = 0;
        var xLottoAccettazione = "";
        var Data_Movimento = formattedDate();
        var Flag_QtaNoZero = false;
        var xTipoPUARegolamento = 0;

        var elencoProdottiCompleto = RicercaElencoCompletoProdotti(objP_super_server, objP_server, objP_utenti, pivaImpresa,
            Sa_Cod, Fabbricato_Cod, TipoDestinazione,
            Elem_Cod, soloInGiacenza, JSON.stringify(options.data.filter.filters),
            "", Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, false, Flag_QtaNoZero, xTipoPUARegolamento, null, null, "", false, -1);
        productsList = JSON.parse(elencoProdottiCompleto);
    }

    options.success(productsList);
}

function ddlLivelliDettaglioRead(options) {
    var detailsList = [
        new GenericOption(1, TraduzioneMultiResx(reportBioResx, "DettaglioMovimenti", "Dettaglio Movimenti")),
        new GenericOption(2, TraduzioneMultiResx(reportBioResx, "RaggruppamentoPerPeriodo", "Raggruppamento per periodo")),
    ];
    options.success(detailsList);
}
