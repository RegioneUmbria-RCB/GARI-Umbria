var indirizzohttp = "./ReportQdC.aspx";

function RicercaReportQdC(options) {

    var param = kendo.stringify({
        piva_Aziende: KendoMultisel("multiselAzienda").value(),
        centri_Aziendali: KendoMultisel("multiselCentroAzienda").value(),
        specie: KendoMultisel("Specie").value(),
        dataInizioOp: $('input[name$="Txt_DataOpDal"]').val(),
        dataFineOp: $('input[name$="Txt_DataOpAl"]').val(),
        periodoDataOperazioni: parseInt($('#periodoGiorniOperazioni')[0].value),
        dataInizioImp: $('input[name$="Txt_DataImpDal"]').val(),
        dataFineImp: $('input[name$="Txt_DataImpAl"]').val(),
        periodoData: parseInt($("#periodoGiorni")[0].value),
        tipoOperazioni: KendoMultisel("selTipoOperazione").value(),
        operazioni: KendoMultisel("multiselOperazione").value(),
        nazioni: KendoMultisel("multiselNazione").value(),
        regioni: KendoMultisel("multiselContea").value(),
        province: KendoMultisel("multiselSottocontea").value(),
        comuni: KendoMultisel("multiselDistretto").value(),
        impianti: getKendoSwitch("SwitchEstraiImpianti"),
        prodotti: getKendoSwitch("SwitchEstraiProdotti"),
        operazioniColt: KendoDDL("id_selTipoAnalisi").value() == 0,
        estrazione: KendoDDL("id_selEstrazione").value(),
        aziende_referenti: KendoMultisel("multiselReferente").value(),
        datiTecnici: false
    })
       
    var risp = "";
    ajaxAgronica(indirizzohttp + "/CaricaGrigliaReportQdC",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
            if (parseInt(Get_KendoDDLValue("TipoOutput", 0)) === 0) {
                //AggiornaGrafico();
                //Personalizza_Griglia();
            } else {
                 //AggiornaGraficoPivot();
                 Personalizza_Griglia_Pivot();
            }
        }, null);
}

function RiempiCategorie(options) {
    var elencoCategorie = Leggi_Categorie_Magazzino();
    options.success(elencoCategorie);
}

function RiempiCategorieCommerciali(options) {
    var elencoCategorieCommerciali = Leggi_Categorie_Commerciali();
    options.success(elencoCategorieCommerciali);
}

//function RiempiSpecie(options) {
//    var elencoSpecie = RicercaSpecie($(cIdPiva).val());
//    options.success(elencoSpecie);
//}

function RiempiVarieta(options) {
    var multisel = KendoMultisel("multiselVarieta");
    var veg_cod = multisel.dataSource.options.transport.data.Veg_Cod;
    var elencoVarieta = RicercaVarieta($(cIdPiva).val(), veg_cod);
    options.success(elencoVarieta);
}

function RiempiRapportiContabili(options)
{
    options.success(Elenco_Rapporti_Contabili());
}

function RiempiNazioni(options)
{
    options.success(Elenco_Nazioni());
}

function RiempiClienti(options) {
    options.success(Elenco_Contatti_Clienti());
}

function RiempiAgenti(options) {
    options.success(Elenco_Contatti_Agenti());
}

function RiempiListaReport(options) {    
    options.success(Lista_Report());
}

function CambiaListaReport(e) {
    Svuota_Parametri();
    var nome_report = KendoDDL("selListaReport").value();
    if (nome_report && nome_report !== "") {
        Leggi_Report(nome_report, false);
    }
    else {
        KendoDDL("TipoOutput").enable(true);
    }
}

function RiempiTipoGrafico(options) {
    var descrVenAcq = "Venduto";
    if ($(cIdType).val() === "A") {
        descrVenAcq = "Acquistato";
    }
    var elencoGrafici = [
        { "cod": "Pivot", "desc": "Grafico Pivot" },
        { "cod": "Anno", "desc": descrVenAcq + " per Anno" },
        { "cod": "Regione", "desc": descrVenAcq + " per Regione" }
    ];
    options.success(elencoGrafici);
}

function RiempiCampiGrafico(options) {

    var titleSoggettoRagSoc = "Cliente";
    var mesDestProv = "Destinazione";
    if ($(cIdType).val() === "A") {
        titleSoggettoRagSoc = "Fornitore";
        mesDestProv = "Provenienza"
    }

    var elencoCampi = [
        { "cod": "Anno_Movimento", "desc": "Anno Documento" },
        { "cod": "Mese_Movimento", "desc": "Mese Documento" },
        { "cod": "Data_Movimento", "desc": "Data Documento" },
        { "cod": "Tipo_Documento", "desc": "Tipo Documento" },
        { "cod": "Soggetto_RagioneSociale", "desc": titleSoggettoRagSoc },
        { "cod": "Destinazione", "desc": mesDestProv },
        { "cod": "Agente_RagioneSociale", "desc": "Agente" },
        { "cod": "Capoarea_RagioneSociale", "desc": "Capo Area" },
        { "cod": "Vettore_RagioneSociale", "desc": "Vettore" },
        { "cod": "Stato", "desc": "Stato" },
        { "cod": "Regione", "desc": "Regione" },
        { "cod": "Provincia", "desc": "Provincia" },
        { "cod": "Comune", "desc": "Comune" },
        { "cod": "Referenza_Descr", "desc": "Prodotto" },
        { "cod": "Categoria_Prodotto", "desc": "Categoria Prodotto" },
        { "cod": "Categoria_Commerciale", "desc": "Categoria Commerciale" }
    ];
    options.success(elencoCampi);
}

function RiempiValoriGrafico(options) {
    var elencoValori = [
        { "cod": "Qta", "desc": "Quantità" },
        { "cod": "Kg_Netti", "desc": "Quantità Totale" },
        { "cod": "Tara_Totale", "desc": "Tara" },
        { "cod": "Kg_Lordi", "desc": "Kg Lordi" },
        { "cod": "Imponibile_Netto", "desc": "Imponibile Netto" },
        { "cod": "Iva", "desc": "Iva" },
        { "cod": "Importo", "desc": "Importo" },
        { "cod": "Provvigione_Calcolata", "desc": "Provvigione" }
    ];
    options.success(elencoValori);
}

function AggiornaGrafico() {
    var campo = KendoDDL("selCampiGrafico").value();
    var valore = KendoDDL("selValoriGrafico").value();
    var titolo = KendoDDL("selTipoOperazione").text();
    var grid = $("#tab_testata_griglia_report_vendite").data("kendoGrid");
    if (grid !== undefined) {
        popolaGrafico("graficoReportVendite", grid.dataSource.data(), campo, valore, titolo);
    }
}

function AggiornaGraficoPivot() {
    var tipoGrafico = KendoDDL("selTipoGrafico").value();
    if (tipoGrafico === "Pivot") {
        popolaGraficoPivot("grafico_tab_riepilogo");
    } else {
        popolaGraficoReportVendite("grafico_tab_riepilogo", tipoGrafico);
    }
}

function EsportaGrafico(IDGrafico, tipo) {
    var chart = $("#" + IDGrafico).getKendoChart();
    if (chart !== undefined) exportChart("#" + IDGrafico, tipo); 
}

//function RiempiProdotti(options) {
//    var elencoVuoto = [{}];
//    var elencoProdotti = RicercaProdottiCompleto(options);
//    if (elencoProdotti !== null) options.success(elencoProdotti);
//    else options.success(elencoVuoto);
//}

function RicercaProdottiCompleto(options) {

    var Elem_Cod = 0;
    var soloInGiacenza = false;
    var Sa_Cod = 0;
    var Fabbricato_Cod = 0;
    var TipoDestinazione = 0;
    var Cau_Mov = "7300";
    var xPUARegolamento = 0;
    var xLottoAccettazione = "";
    var Data_Movimento = formattedDate(new Date(), "/");
    var Flag_QtaNoZero = false;
    var xTipoPUARegolamento = 0;

    var elencoProdottiCompleto = RicercaElencoCompletoProdotti(objP_super_server, objP_server, objP_utenti, $(cIdPiva).val(),
        Sa_Cod, Fabbricato_Cod, TipoDestinazione,
        Elem_Cod, soloInGiacenza, JSON.stringify(options.data.filter.filters),
        "", Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, false, Flag_QtaNoZero, xTipoPUARegolamento, null, null, "", false, -1);

    var risp = JSON.parse(elencoProdottiCompleto);

    // Essendo una multiselect ripasserà solo la chiave, quindi devo avere anche Elem_Cod per poter poi fare la lettura corretta
    for (var i = 0; i < risp.length; i++) {
        risp[i].Prodotto_Cod = risp[i].Elem_Cod + "_" + risp[i].Prodotto_Cod;
    }
    options.success(risp);

}

function Elenco_Rapporti_Contabili()
{
    return LeggiRapportiContabili(true, false);
}

function Elenco_Nazioni()
{
    var nazioni = LeggiNazioni(false, false);
    return nazioni.sort((a, b) => (a.Descrizione > b.Descrizione) ? 1 : -1);
}

function Elenco_Contatti_Clienti() {

    var risultato_lettura;

    var flagCliente = 1;
    var flagFornitore = 0;
    if ($(cIdType).val() === "A") {
        flagCliente = 0;
        flagFornitore = 1;
    }

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Contatti_Clienti_Agenti",
        "{ piva: '" + $(cIdPiva).val() + "', cliente: " + flagCliente + ", agente: 0, fornitore: " + flagFornitore + "}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Elenco_Contatti_Agenti() {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Contatti_Clienti_Agenti",
        "{ piva: '" + $(cIdPiva).val() + "', cliente: 0, agente: 1, fornitore: 0}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function AggiornaCuboReportVendite() {
    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        dataDal: $('input[name$="Txt_DataOpDal"]').val(),
        dataAl: $('input[name$="Txt_DataOpAl"]').val()
    });
    
    ajaxAgronica(indirizzohttp + "/AggiornaCuboReportVendite",
        param,
        function (risposta) {
            kendo.alert(risposta.RispostaStringa);
        }, null);
}

function Leggi_Dati_Grafico(options) {
    var tipo = KendoDDL("selTipoGrafico").value();
    var includiCorrispettivi = false;
    if ($(cIdType).val() === "V") {
        includiCorrispettivi = getKendoSwitch("opt_corrispettivi");
    }
    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        report: KendoDDL("selTipoOperazione").value(),
        cubo: $(cIdCubo).val(),
        tipo: tipo,
        _dataRegDal: $('input[name$="Txt_DataOpDal"]').val(),
        _dataRegAl: $('input[name$="Txt_DataOpAl"]').val(),
        _clienti: KendoMultisel("multiselClienti").value().join("|"),
        _agenti: KendoMultisel("multiselAgenti").value().join("|"),
        _causali: KendoMultisel("multiselOperazione").value().join("|"),
        _specie: KendoMultisel("multiselSpecie").value().join("|"),
        _varieta: KendoMultisel("multiselVarieta").value().join("|"),
        _prodotti: KendoMultisel("multiselAzienda").value().join("|"),
        _categorie: KendoMultisel("multiselReferente").value().join("|"),
        _categcommerciali: KendoMultisel("multiselCentroAzienda").value().join("|"),
        _includiCorrispettivi: includiCorrispettivi
    });
    ajaxAgronica(indirizzohttp + "/Leggi_Dati_Grafico",
        param,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
            //kendo.alert(risposta.RispostaStringa);
        }, null);
}

function Leggi_Categorie_Magazzino() {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Categorie_Magazzino",
        "{ }",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Categorie_Commerciali() {

    var param = kendo.stringify({ piva: $(cIdPiva).val() });
    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Categorie_Commerciali",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Specie() {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Specie",
        "{ }",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Operazioni(tipoOp) {

    var param = kendo.stringify({
        tipoOp: tipoOp.length == 0 ? [0] : tipoOp
    })

    ajaxAgronicaSync(indirizzohttp + "/LeggiOperazioni",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            Elenco_Operazioni = risp;
        }, null);

}

function LeggiNazioni(flagVuoto, gestioneWaitFrame) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = true;

    if (Elenco_Nazioni === undefined || Elenco_Nazioni === null) {
        var param = kendo.stringify({
            objP_server: objP_server
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Nazioni + "/Leggi",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Codice": "",
                        "Descrizione": ""
                    };
                    risp.unshift(objVuoto);
                }
                Elenco_Nazioni = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Nazioni");
            },
            null,
            gestioneWaitFrame);
    }

    return Elenco_Nazioni;
}

function LeggiRegioni() {

    let nazioni = KendoMultisel("multiselNazione").value();

    if (nazioni.length > 0) {

        var valo = "";
        nazioni.forEach(x => valo = valo.concat(x).concat("|"))
        var param = kendo.stringify({ nazioni: valo });

        ajaxAgronicaSync(indirizzohttp + "/LeggiRegioni",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);

                Elenco_Regioni = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Nazioni");
            },
            null);
        KendoMultisel("multiselContea").enable(true);

    } else {
        Elenco_Centri = [{ "REG": 0, "Regione_Des": "Tutti" }];
        KendoMultisel("multiselContea").enable(false);
        KendoMultisel("multiselContea").value([]);
    }

    return Elenco_Regioni;
}

function LeggiProvince() {

    let regioni = KendoMultisel("multiselContea").value();

    if (regioni.length > 0) {

        var valo = "";
        regioni.forEach(x => valo = valo.concat(x).concat("|"))
        var param = kendo.stringify({ regioni: valo });

        ajaxAgronicaSync(indirizzohttp + "/LeggiProvince",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);

                Elenco_Province = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Nazioni");
            },
            null);
        KendoMultisel("multiselSottocontea").enable(true);

    } else {
        Elenco_Centri = [{ "PROV": 0, "PROVINCIA": "Tutti" }];
        KendoMultisel("multiselSottocontea").enable(false);
        KendoMultisel("multiselSottocontea").value([]);
    }

    return Elenco_Province;
}

function LeggiComuni() {

    let province = KendoMultisel("multiselSottocontea").value();

    if (province.length > 0) {

        var valo = "";
        province.forEach(x => valo = valo.concat(x).concat("|"))
        var param = kendo.stringify({ province: valo });

        ajaxAgronicaSync(indirizzohttp + "/LeggiComuni",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);

                Elenco_Comuni = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Nazioni");
            },
            null);
        KendoMultisel("multiselDistretto").enable(true);

    } else {
        Elenco_Comuni = [{ "COM": 0, "LOCALITA": "Tutti" }];
        KendoMultisel("multiselDistretto").enable(false);
        KendoMultisel("multiselDistretto").value([]);
    }

    return Elenco_Comuni;
}

function LeggiReferenti() {


    var param = kendo.stringify({ });

    ajaxAgronicaSync(indirizzohttp + "/LeggiReferenti",
        param,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);

            Elenco_Referenti = risp;
        },
        function (risposta) {
            var msgError = mostraErrore(risposta, "Errore WS Referenti");
        },
        null);


    return Elenco_Referenti;
}

function LeggiAziende() {

    var valo = "";

    if (KendoMultisel("multiselReferente") != undefined) {
        let referenti = KendoMultisel("multiselReferente").value()
        referenti.forEach(x => valo = valo.concat(x).concat("|"))
    }

    var param = kendo.stringify({ referenti: valo });

    ajaxAgronicaSync(indirizzohttp + "/LeggiAziende",
        param,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);

            Elenco_Aziende = risp;
        },
        function (risposta) {
            var msgError = mostraErrore(risposta, "Errore WS Aziende");
        },
        null);


    return Elenco_Aziende;
}

function LeggiCentri() {

    let aziende = KendoMultisel("multiselAzienda").value();

    if (aziende.length > 0) {

        var valo = "";
        aziende.forEach(x => valo = valo.concat(x).concat("|"))
        var param = kendo.stringify({ aziende: valo });

        ajaxAgronicaSync(indirizzohttp + "/LeggiCentriAziendali",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);

                Elenco_Centri = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Centri");
            },
            null);
        KendoMultisel("multiselCentroAzienda").enable(true);

    } else {
        Elenco_Centri = [{ "PivaSa": 0, "sa_nome": "Tutti" }];
        KendoMultisel("multiselCentroAzienda").enable(false);
        KendoMultisel("multiselCentroAzienda").value([]);
    }

    return Elenco_Centri;
}

function stampa_documento(pivaImpresa, Id_Agenda, Lav_Cod, Modulo, Tipo_Accettazione) {
    var param = kendo.stringify({ piva: pivaImpresa, Id_Agenda: Id_Agenda, Lav_Cod: Lav_Cod, Modulo: Modulo, Tipo_Accettazione: Tipo_Accettazione });
    var risp = "";
    ajaxAgronica(indirizzohttp + "/StampaDocumento",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            var win = window.open(risp);
            win.focus();
        }, null);
}

function getParametriGriglia(pagina, nomeDiv) {
    var jsonDaSalvare = "";
    var grid = $('#' + nomeDiv).data('kendoGrid');
    if (grid !== undefined) {
        var dataSource = grid.dataSource;
        var columns = grid.columns;
        var pageSize = dataSource.pageSize();
        var sort = dataSource.sort();
        var filter = dataSource.filter();
        var group = dataSource.group();
        var options = { pagina: pagina, nomeDiv: nomeDiv, columns: columns, pageSize: pageSize, sort: sort, filter: filter, group: group };
        jsonDaSalvare = kendo.stringify(options);
    }
    return jsonDaSalvare;
}

function getParametriGrigliaPivot(pagina, nomeDiv) {
    var jsonDaSalvare = "";
    var grid = $('#' + nomeDiv).data('kendoPivotGrid');
    if (grid !== undefined) {
        var dataSource = grid.dataSource;
        var columns = dataSource.columns();
        var rows = dataSource.rows();
        var measures = dataSource.measures();
        var filters = [];
        if (dataSource.filter() !== undefined) {
            filters = dataSource.filter().filters;
        }        
        rows.forEach(function (e, i) { e.expand = true; });
        columns.forEach(function (e, i) { e.expand = true; });
        var options = { pagina: pagina, nomeDiv: nomeDiv, columns: columns, rows: rows, measures: measures, filters: filters };
        jsonDaSalvare = kendo.stringify(options);
    }
    return jsonDaSalvare;
}

function Salva_Report(nomeReport) {

    nomeReport = SanitizeTesto_MantieniVirgoletteECaratteriAccentati(nomeReport);
    if (nomeReport.trim() == "") {
        kendo.alert("Inserire un nome report valido");
        return false
    }

    var tipoOutput = "Griglia";
    var tipoOutputVal = parseInt(Get_KendoDDLValue("TipoOutput", 0));
    if (tipoOutputVal === 1)
        tipoOutput = "Pivot";
    else
        if (tipoOutputVal === 2)
            tipoOutput = "Report";
    var parametriGriglia = getParametriGriglia(location.pathname, "tab_testata_griglia_report_vendite");
    var parametriGrigliaPivot = getParametriGrigliaPivot(location.pathname, "pivot_tab_riepilogo");

    var parametriReport = {
        nome: nomeReport,
        _tipoAnalisi: KendoDDL("id_selTipoAnalisi").value(),
        _tipoEstrazione: KendoDDL("id_selEstrazione").value(),
        "tipoOutput": tipoOutput,
        _specie: KendoMultisel("multiselSpecie").value().join("|"),
        _filtroDateOperazioni: KendoDDL("ddlFiltroDateOperazione").value(),
        _dataOpDal: $('input[name$="Txt_DataOpDal"]').val(),
        _dataOpAl: $('input[name$="Txt_DataOpAl"]').val(),
        _periodoGiorniOperazioni: $('input[name$="periodoGiorniOperazioni"]').val(),
        _filtroDate: KendoDDL("ddlFiltroDateImpianto").value(),
        _dataImpDal: $('input[name$="Txt_DataImpDal"]').val(),
        _dataImpAl: $('input[name$="Txt_DataImpAl"]').val(),
        _periodoGiorni: $('input[name$="periodoGiorni"]').val(),
        _referenti: KendoMultisel("multiselReferente").value().join("|"),
        _aziende: KendoMultisel("multiselAzienda").value().join("|"),
        _centri: KendoMultisel("multiselCentroAzienda").value().join("|"),
        _nazioni: KendoMultisel("multiselNazione").value().join("|"),
        _regioni: KendoMultisel("multiselContea").value().join("|"),
        _province: KendoMultisel("multiselSottocontea").value().join("|"),
        _comuni: KendoMultisel("multiselDistretto").value().join("|"),
        _impianti: getKendoSwitch("SwitchEstraiImpianti"),
        _prodotti: getKendoSwitch("SwitchEstraiProdotti"),
        personalizzazioni: parametriGriglia,
        personalizzazioniPivot: parametriGrigliaPivot,
    };

    var parametri = kendo.stringify({ "nome": nomeReport, "report": kendo.stringify(parametriReport) });
    ajaxAgronica(indirizzohttp + '/SalvaReport', parametri,
        function (risposta) {
            var risp = risposta.RispostaStringa;
            var listaReport = KendoDDL("selListaReport");
            var dataSource = listaReport.dataSource;
            var trovaReport = dataSource.data().filter(function (dataItem) { return dataItem.cod === nomeReport; });
            if (trovaReport.length === 0) dataSource.add({ cod: nomeReport, desc: nomeReport });
            listaReport.select(function (dataItem) { return dataItem.cod === nomeReport; });
            var ddlOutput = KendoDDL("TipoOutput");
            ddlOutput.enable(false);
            // kendo.alert("Report salvato correttamente.");
        },
        function(risposta) {
            console.log("Errore nel salvataggio impostazioni report vendite: " + risposta.Errore);
        });
     
    
}

function Lista_Report() {
    var risultato_lettura;
    ajaxAgronicaSync(indirizzohttp + "/ListaReport",
        null,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            var objVuoto = { "cod": "", "desc": "" };
            risp.unshift(objVuoto);
            risultato_lettura = risp;
        }, null);
    return risultato_lettura;
}

function Leggi_Report(nomeReport, esegui) {

    var parametri = kendo.stringify({ "nome": nomeReport });

    ajaxAgronica(indirizzohttp + '/LeggiReport', parametri,
        function (risposta) {

            if (risposta.RispostaOK) {
                if (risposta.RispostaStringa !== "") {

                    var params = JSON.parse(risposta.RispostaStringa);
                    Personalizza_Report(params); 

                    //if (esegui) Esegui_Report();

                } else kendo.alert("Nessuna personalizzazione report");

            } else console.log("Errore nella lettura impostazioni report vendite: " + risposta.Errore);

        }, null);
}

function Cancella_Report(nomeReport) {
    var parametri = kendo.stringify({ "nome": nomeReport });
    ajaxAgronica(indirizzohttp + '/CancellaReport', parametri,
        function (risposta) {
            var risp = risposta.RispostaStringa;
            KendoDDL("selListaReport").dataSource.read();
            // KendoDDL("selListaReport").value(-1);
            KendoDDL("selListaReport").refresh();
            // kendo.alert("Report eliminato correttamente.");
        },
        function(risposta) {
            console.log("Errore nella cancellazione report vendite: " + risposta.Errore);
        });
}

// ripristina personalizzazioni griglia report vendite
function Personalizza_Griglia() {
    var grid = $('#tab_testata_griglia_report_vendite').data('kendoGrid');
    if (grid !== undefined && personalizzazioni && personalizzazioni !== "") {
        setPersonalizzazioniGrigliaKendo(grid, JSON.parse(personalizzazioni));
        personalizzazioni = null;
    }
}

// ripristina personalizzazioni griglia pivot report vendite
function Personalizza_Griglia_Pivot() {
    var grid = $('#pivot_tab_riepilogo').data('kendoPivotGrid');
    if (grid !== undefined && personalizzazioniPivot && personalizzazioniPivot !== "") {
        var options = JSON.parse(personalizzazioniPivot);
        var dataSource = grid.dataSource;
        dataSource._columns = options.columns;
        dataSource._rows = options.rows;
        dataSource._measures = options.measures;
        dataSource._filter = options.filters;
        //personalizzazioniPivot = null;
    }
}

// ripristina personalizzazioni griglia kendo (valutare se spostarla nel base)
function setPersonalizzazioniGrigliaKendo(grid, options) {

    try {

        var dataSource = grid.dataSource;
        var savedColumns = options.columns;

        //NUMERO DI RIGHE PER PAGINA
        if (options.pageSize)
            dataSource.pageSize(options.pageSize);

        //RIORDINAMENTO COLONNE
        var indOrd = 0;
        for (let i = 0; i < savedColumns.length; i++) {
            let col;

            //Cerco la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
            if (savedColumns[i].field && savedColumns[i].field != null) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].field == savedColumns[i].field; });
            } else if (savedColumns[i].title) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].title == savedColumns[i].title; });
            }

            //Se ho trovato la colonna...
            if (col) {
                //Sposto la colonna in testa
                if (savedColumns[i].hidden != true)
                    grid.reorderColumn(indOrd, col);
                indOrd++;
            }

        }

        //MOSTRO O NASCONDO COLONNE
        for (let i = savedColumns.length - 1; i >= 0; i--) {
            let col;

            //Cerco la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
            if (savedColumns[i].field && savedColumns[i].field != null) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].field == savedColumns[i].field; });
            } else if (savedColumns[i].title) {
                col = grid.columns.find(function (v, index) { return grid.columns[index].title == savedColumns[i].title; });
            }

            //Se ho trovato la colonna...
            if (col) {
                if (savedColumns[i].hidden == true) {
                    grid.hideColumn(col);
                } else { //else if (savedColumns[i].hidden == false) 
                    grid.showColumn(col);
                }
            }
        }

        //ORDINAMENTO (ASC/DESC) DELLE COLONNE
        if (options.sort) {

            var ordinam = options.sort;
            for (let i = ordinam.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == ordinam[i].field; });

                if (col == undefined) {
                    ordinam.splice(i, 1);
                }
            }

            if (ordinam.length > 0)
                dataSource.sort(options.sort);
        }

        //FILTRI PER COLONNE
        if (options.filter) {

            var filtri = options.filter.filters;
            for (let i = filtri.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == filtri[i].field; });

                if (col == undefined) {
                    filtri.splice(i, 1);
                }

                // Sistemazione della data che era stata memorizzata a db in formato GMT
                if (col.field !== undefined && grid.dataSource.options.schema.model.fields[col.field].type === "date")
                    filtri[i].value = kendo.parseDate(filtri[i].value);
            }

            if (filtri.length > 0)
                dataSource.filter(options.filter);
        }

        //RAGGRUPPAMENTO IN TOOLBAR
        if (options.group) {

            var raggrup = options.group;
            for (let i = raggrup.length - 1; i >= 0; i--) {
                let col = grid.columns.find(function (v, index) { return grid.columns[index].field == raggrup[i].field; });

                if (col == undefined) {
                    raggrup.splice(i, 1);
                }
            }

            if (raggrup.length > 0)
                dataSource.group(options.group);
        }

    } catch (err) {
        console.log(err);
    }
}


function RiempiDdlTipoOperazione(options) {
    return new Promise((resolve, reject) => {
        let storage_key = "RiempiDdlTipoOperazione_1_0_0_1";

        //objP_utenti: objP_utenti,
        if (!storageExistItem(storage_key)) {
            var parametri = kendo.stringify({
                objP_server: objP_server,
                Flag_OpColturali: true,
                Flag_OpZoo: false,
                Flag_OpMacchine: false,
                Flag_OpContabili: true
            });

            ajaxAgronica(pathCoreWS + "Metaschema/Operazioni.asmx/Leggi_GruppoOperazioni",
                parametri,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    storageSetItem(storage_key, risposta.RispostaStringa);
                    resolve(risp);
                }, null, undefined, false);
        } else {
            resolve(JSON.parse(storageGetItem(storage_key)));
        }

    });

}


function Lancia_Report_PDF() {

    var docNr = 0;
    var analisi = KendoDDL("selTipoOperazione").value();
    var causali = KendoMultisel("multiselOperazione").value().join("|");

    var arrLivelli = [];

    if ($("#livelliScelti").data("kendoListBox") !== undefined &&
        $("#livelliScelti").data("kendoListBox").dataSource !== undefined &&
        $("#livelliScelti").data("kendoListBox").dataSource._data !== undefined &&
        $("#livelliScelti").data("kendoListBox").dataSource._data.length > 0 &&
        $("#livelliScelti").data("kendoListBox").items().length > 0) {

        for (var x = 0; x < $("#livelliScelti").data("kendoListBox").items().length; x++) {
            var uid = $($("#livelliScelti").data("kendoListBox").items()[x]).attr("data-uid");
            if (uid !== undefined) {
                for (var y = 0; y < $("#livelliScelti").data("kendoListBox").dataSource._data.length; y++) {
                    if (uid === $("#livelliScelti").data("kendoListBox").dataSource._data[y].uid) {
                        arrLivelli.push($("#livelliScelti").data("kendoListBox").dataSource._data[y].value);
                    }
                }
            }
        }
    }
    var livelli = arrLivelli.join("|");
    var confronto_anno = "0";
    if (KendoDDL("sel_tipo_report_vendite").value() === enum_Tipo_Report_PDF.Statistica_mese_anno_confronto_fra_anni || 
        KendoDDL("sel_tipo_report_vendite").value() === enum_Tipo_Report_PDF.Statistica_anno_con_scostamento_e_previsioni) {
        confronto_anno = $("#Txt_confronto_anno").val();
    }

    var tipo_valore = KendoDDL("sel_tipo_valore").value();
    var numeroDecimaliQta = 0;
    if (tipo_valore === "0")
        numeroDecimaliQta = KendoDDL("sel_decimali").value();

    var includiCorrispettivi = false;
    if ($(cIdType).val() === "V") {
        includiCorrispettivi = getKendoSwitch("opt_corrispettivi");
    }

    if (analisi === "Report_Ordini_Vendita") causali = "";
    if ($('input[name$="TxtDocNumero"]').val() !== null && $('input[name$="TxtDocNumero"]').val() !== "")
        docNr = parseInt($('input[name$="TxtDocNumero"]').val().replace(".", ""));
    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        analisi: analisi,
        cubo: $(cIdCubo).val(),
        _docNumeroSin: $('input[name$="SwitchImpianti"]').val(),
        _docNumero: docNr,
        _docNumeroDes: $('input[name$="TxtDocNumeroDes"]').val(),
        _nrRiga: $('input[name$="TxtNrRiga"]').val(),
        _dataRegDal: $('input[name$="Txt_DataOpDal"]').val(),
        _dataRegAl: $('input[name$="Txt_DataOpAl"]').val(),
        _specie: KendoMultisel("multiselSpecie").value().join("|"),
        _varieta: KendoMultisel("multiselVarieta").value().join("|"),
        _prodotti: KendoMultisel("multiselAzienda").value().join("|"),
        _categorie: KendoMultisel("multiselReferente").value().join("|"),
        _categcommerciali: KendoMultisel("multiselCentroAzienda").value().join("|"),
        _clienti: KendoMultisel("multiselClienti").value().join("|"),
        _agenti: KendoMultisel("multiselAgenti").value().join("|"),
        _causali: causali,
        tipo_report: KendoDDL("sel_tipo_report_vendite").value(),
        titolo: kendoEscapeOggetto($("#TxtTitolo").val()),
        livelli: livelli,
        da_mese: $("#Txt_da_mese").val(),
        da_anno: $("#Txt_da_anno").val(),
        a_mese: $("#Txt_a_mese").val(),
        a_anno: $("#Txt_da_anno").val(),
        confronto_anno: confronto_anno,
        tipo_valore: tipo_valore,
        tipo_secondo_valore: KendoDDL("sel_tipo_secondo_valore").value(),
        decimali_qta: numeroDecimaliQta,
        scost_totale: true,
        salto_pag_1_liv: getKendoSwitch("cb_salto_pag_1_liv"),
        cb_ordin_x_valore: getKendoSwitch("cb_ordin_x_valore"),
        mese_prev_scost: $("#Txt_mese_prev_scost").val(),
        _rapportiContabili: KendoMultisel("multiselSpecie").value().join("|"),
        _nazioniFatturazione: KendoMultisel("multiselNazioni").value().join("|"),
        _includiCorrispettivi: includiCorrispettivi
    });

    var risp = "";
    ajaxAgronica(indirizzohttp + "/Lancia_Report_PDF",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            var win = window.open(risp);
            win.focus();
        }, null);
     
}
