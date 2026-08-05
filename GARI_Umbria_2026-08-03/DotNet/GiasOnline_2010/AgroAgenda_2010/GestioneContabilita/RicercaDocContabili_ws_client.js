//////////////////////////////////////////////////////////
// Report AggiornaCuboReportVendite
//////////////////////////////////////////////////////////

var indirizzohttp = "./RicercaDocContabili.aspx";
var indirizzohttp_Ricerca_Documenti = "Contab/RicercaDocumenti.asmx";
var indirizzohttp_Attivita = "Contab/Attivita.asmx";
var indirizzoHttp_DocContabile_WS = "../GestioneContabilita/DocContabile_WS.aspx";

function RicercaDocumenti(type, doc_type, dettaglio, piva, report, cubo, descrizione,
    docNumeroSin, docNumero, docNumeroDes, nrRiga, dataRegDal, dataRegAl, centriAziendali,
    clienti, agenti, causali, specie, varieta, prodotti, categorie,
    categcommerciali, soloDDTNonFatturati, soloOrdiniNonSpediti, modalitaFatturazione, utenteAbilitatoLettura, utenteAbilitatoScrittura, options) {

    var param = kendo.stringify({
        type: type,
        doc_type: doc_type,
        tutteLeCausali: kendoEscapeOggetto(elencoCausali),
        dettaglio: dettaglio,
        piva: piva,
        report: report,
        cubo: cubo,
        _descrizione: descrizione,
        _docNumeroSin: docNumeroSin,
        _docNumero: docNumero,
        _docNumeroDes: docNumeroDes,
        _nrRiga: nrRiga,
        _dataRegDal: dataRegDal,
        _dataRegAl: dataRegAl,
        _centriAziendali: centriAziendali,
        _clienti: clienti,
        _agenti: agenti,
        _causali: causali,
        _specie: specie,
        _varieta: varieta,
        _prodotti: prodotti,
        _categorie: categorie,
        _categcommerciali: categcommerciali,
        _soloDDTNonFatturati: soloDDTNonFatturati,
        _soloOrdiniNonSpediti: soloOrdiniNonSpediti,
        _modalitaFatturazione: modalitaFatturazione,
        _utenteAbilitatoLettura: utenteAbilitatoLettura,
        _utenteAbilitatoScrittura: utenteAbilitatoScrittura,
        objP_server: objP_server,
        objP_utenti: objP_utenti
    });

    ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Ricerca_Documenti + "/LeggiDocumenti",
        param,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
            Applica_Personalizzazioni_Griglie();

            utente_conflittoPermessiWorkflow = "";

            if (Array.isArray(risposta.ErroriGias)) {
                risposta.ErroriGias.forEach(errGias => {
                    // Il tipo 2 rappresenta ConflittoPermessi
                    if (errGias.tipo === 2) {
                        utente_conflittoPermessiWorkflow = errGias.messaggio;
                        MessaggioAttenzione_Bootstrap(errGias.messaggio, "DIV_Messaggi", 0);
                    }
                });
            }
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });

}  

function RicercaReportTestata(options) {

    var piva = $(cIdPiva).val();
    var docNr = 0;
    var causali = KendoMultisel("multiselCausale").value().join("|");
    if ($('input[name$="TxtDocNumero"]').val() !== null && $('input[name$="TxtDocNumero"]').val() !== "")
        docNr = parseInt($('input[name$="TxtDocNumero"]').val().replace(".", ""));
    var _descrizione = $('input[name$="txt_operazione"]').val();
    var report = "";
    var cubo = false;
    var _type = ParametroType();
    var _doc_type = ParametroDocType();
    var _docNumeroSin = $('input[name$="TxtDocNumeroSin"]').val();
    var _docNumero = docNr;
    var _docNumeroDes = $('input[name$="TxtDocNumeroDes"]').val();
    var _nrRiga = $('input[name$="TxtNrRiga"]').val();
    var _dataRegDal = $('input[name$="Txt_DataRegDal"]').val();
    var _dataRegAl = $('input[name$="Txt_DataRegAl"]').val();
    var _clienti = KendoMultisel("multiselContatti").value().join("|");
    var _agenti = KendoMultisel("multiselAgenti").value().join("|");
    var _causali = causali;
    var _soloDDTNonFatturati = getKendoSwitch("cb_ddt_non_fatturati");
    var _soloOrdiniNonSpediti = getKendoSwitch("cb_ordini_non_Spediti");
    var _centriAziendali = KendoMultisel("multiselCentroAziendale").value().join("|");
    var _modalitaFatturazione = IsFatturazione();

    RicercaDocumenti(_type, _doc_type, false, piva, report, cubo, _descrizione,
        _docNumeroSin, _docNumero, _docNumeroDes, _nrRiga, _dataRegDal, _dataRegAl,
        _centriAziendali, _clienti, _agenti, _causali, "", "", "", "", "",
        _soloDDTNonFatturati, _soloOrdiniNonSpediti, _modalitaFatturazione, true, utenteAbilitatoInserimentoModifica, options);
  
}

function FiltraContatti(options) {

    var elencoContatti = "";
    var testo = JSON.stringify(options.data.filter.filters);
    var piva = $(cIdPiva).val();
    var url = GetUrlLetturaTabelleGestionali() + indirizzohttp_Ricerca_Documenti + "/ListaContatti";

    var param = kendo.stringify(
        {
            objP_server: objP_server,
            Piva: piva,
            Cod_Contatto: '',
            Cod_RisUm: 0,
            FlagPubblico: true,
            ID_CF: -99,
            Cod_RisUm_Origine: 0,
            Piva_SuperUser_Origine: '',
            Progressivo: '',
            testoRicerca: testo,
            xOrderBy: ' Contatti.Rag_Soc ASC, Contatti.Cognome ASC, Contatti.Nome ASC '
        });

    ajaxAgronica(url, param,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });
}

function RicercaReportDettaglio(options) {

    var piva = $(cIdPiva).val();
    var docNr = 0;
    var causali = KendoMultisel("multiselCausale").value().join("|");
    if ($('input[name$="TxtDocNumero"]').val() !== null && $('input[name$="TxtDocNumero"]').val() !== "")
        docNr = parseInt($('input[name$="TxtDocNumero"]').val().replace(".", ""));
    var report = "";
    var cubo = false;
    var _type = ParametroType();
    var _doc_type = ParametroDocType();
    var _descrizione = $('input[name$="txt_operazione"]').val();
    var _docNumeroSin = $('input[name$="TxtDocNumeroSin"]').val();
    var _docNumero = docNr;
    var _docNumeroDes = $('input[name$="TxtDocNumeroDes"]').val();
    var _nrRiga = $('input[name$="TxtNrRiga"]').val();
    var _dataRegDal = $('input[name$="Txt_DataRegDal"]').val();
    var _dataRegAl = $('input[name$="Txt_DataRegAl"]').val();
    var _clienti = KendoMultisel("multiselContatti").value().join("|");
    var _agenti = KendoMultisel("multiselAgenti").value().join("|");
    var _causali = causali;
    var _specie = KendoMultisel("multiselSpecie").value().join("|");
    var _varieta = KendoMultisel("multiselVarieta").value().join("|");
    var _prodotti = KendoMultisel("multiselProdotti").value().join("|");
    var _categorie = KendoMultisel("multiselCategorie").value().join("|");
    var _categcommerciali = KendoMultisel("multiselCategCommle").value().join("|");
    var _soloDDTNonFatturati = getKendoSwitch("cb_ddt_non_fatturati");
    var _soloOrdiniNonSpediti = getKendoSwitch("cb_ordini_non_Spediti");
    var _centriAziendali = KendoMultisel("multiselCentroAziendale").value().join("|");
    var _modalitaFatturazione = IsFatturazione();

    RicercaDocumenti(_type, _doc_type, true, piva, report, cubo, _descrizione,
        _docNumeroSin, _docNumero, _docNumeroDes, _nrRiga, _dataRegDal,
        _dataRegAl, _centriAziendali, _clienti, _agenti, _causali, _specie, _varieta, _prodotti,
        _categorie, _categcommerciali,
        _soloDDTNonFatturati, _soloOrdiniNonSpediti, _modalitaFatturazione, true, utenteAbilitatoInserimentoModifica, options);

}


function RiempiCategorie(options) {
    var elencoCategorie = Leggi_Categorie_Magazzino();
    options.success(elencoCategorie);
}

function RiempiCategorieCommerciali(options) {
    var elencoCategorieCommerciali = Leggi_Categorie_Commerciali();
    options.success(elencoCategorieCommerciali);
}

function RiempiSpecie(options) {
    var elencoSpecie = RicercaSpecie($(cIdPiva).val());
    options.success(elencoSpecie);
}

function RiempiVarieta(options) {
    var multisel = KendoMultisel("multiselVarieta");
    var veg_cod = multisel.dataSource.options.transport.data.Veg_Cod;
    var elencoVarieta = RicercaVarieta($(cIdPiva).val(), veg_cod);
    options.success(elencoVarieta);
}

function RiempiGruppiDocumento(options) {

    var gruppi = [];
    var tuttiGruppi = [
        { "TYPE": "A", "DOC_TYPE": "O", "LAV_COD": "2004", "GRP_DES": TraduzioneMultiResx(resxObj, "OrdiniAcquisto", "Ordini Acquisto") },
        { "TYPE": "A", "DOC_TYPE": "C", "LAV_COD": "1025;1075;1077", "GRP_DES": TraduzioneMultiResx(resxObj, "ConsegneAcquisto", "Consegne Acquisto") },
        { "TYPE": "A", "DOC_TYPE": "F", "LAV_COD": "1000;1002", "GRP_DES": TraduzioneMultiResx(resxObj, "ConsegneFattureNoteCreditoAcquisto", "Consegne Fatture / Note Credito Acquisto") },
        { "TYPE": "V", "DOC_TYPE": "O", "LAV_COD": "2002", "GRP_DES": TraduzioneMultiResx(resxObj, "OrdiniVendita", "Ordini Vendita") },
        { "TYPE": "V", "DOC_TYPE": "C", "LAV_COD": "1031;1069", "GRP_DES": TraduzioneMultiResx(resxObj, "ConsegneVendita", "Consegne Vendita") },
        { "TYPE": "V", "DOC_TYPE": "F", "LAV_COD": "1001;1003", "GRP_DES": TraduzioneMultiResx(resxObj, "FattureNoteCreditoVendita", "Fatture / Note Credito Vendita") },
        { "TYPE": "C", "DOC_TYPE": "C", "LAV_COD": "1054;1076;1078", "GRP_DES": TraduzioneMultiResx(resxObj, "Conferimenti", "Conferimenti") }
    ];

    // Se parametro type arriva vuoto non faccio nessun filtro altrimenti filtro per Type e DocType
    if (ParametroType() == "")
        gruppi = tuttiGruppi;
    else
    {
        gruppi = tuttiGruppi.filter(function (g) {
            return g.TYPE == ParametroType() &&
                g.DOC_TYPE == ParametroDocType();
        });
    }

    options.success(gruppi.sort((a, b) => (a.GRP_DES > b.GRP_DES) ? 1 : -1));
}

function RiempiClienti(options) {
    // options.success(RicercaClienti($(cIdPiva).val()));
    options.success(Elenco_Contatti_Clienti());
}

function RiempiAgenti(options) {
    // options.success(RicercaClienti($(cIdPiva).val()));
    options.success(Elenco_Contatti_Agenti());
}

function RiempiListaReport(options) {    
    options.success(Lista_Report());
}

function RiempiCausali(options) {

    var causali = [];
    // Se parametro type arriva vuoto non faccio nessun filtro altrimenti filtro per Type e DocType
    if (ParametroType() == "")
        causali = elencoCausali;
    else {
        causali = elencoCausali.filter(function (g) {
            return g.TYPE == ParametroType() &&
                g.DOC_TYPE == ParametroDocType();
        });
    }

    options.success(causali.sort((a, b) => (a.LAV_DES > b.LAV_DES) ? 1 : -1));
}

function RiempiTipoAnalisi(options) {
    // i18n Funzione non usata
    var elencoAnalisi = [
        { "cod": "Report_Vendite", "desc": "Report Vendite" },
        { "cod": "Report_Ordini_Vendita", "desc": "Report Ordini Vendita" }
    ];
    options.success(elencoAnalisi);
}

function RicercaProdottiCompleto(options) {

    if (options.data.filter == undefined || options.data.filter.filters.length !== 0) {
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

        let elemCodArray = [];
        if (KendoMultisel("multiselCategorie").value().length > 0)
            elemCodArray = KendoMultisel("multiselCategorie").value();
        var type = ParametroType();
        let xFiltroAggiuntivoMateriePrime = "";
        // Se è entrata da conferimento (ovvero da novembre 2020 tutti i prodotti Core Business)
        // filtro per chiave conferimento
        if (type === "C") {
            elemCodArray = [TRASFORMATI_VEGETALI, TRASFORMATI_ANIMALI];
            xFiltroAggiuntivoMateriePrime = $(hf_filtroMateriePrimeConferimento).val();
        } else {
            if (type === "A" && is_FF_FormProdottoUC()) {
                xFiltroAggiuntivoMateriePrime = " (Materie_Prime.ELEM_COD NOT IN (" + TRASFORMATI_VEGETALI + ", " + TRASFORMATI_ANIMALI + ")) "
            }
        }

        // -----------------  Richiamo lettura prodotti ----------
        // Faccio la chiamata diversa perchè se nel caso di multicategoria la ricerca è meno veloce
        if (elemCodArray.length === 0) {
            elencoProdottiCompleto = RicercaElencoCompletoProdotti(objP_super_server, objP_server, objP_utenti, $(cIdPiva).val(),
                Sa_Cod, Fabbricato_Cod, TipoDestinazione,
                Elem_Cod, soloInGiacenza, JSON.stringify(options.data.filter.filters),
                "", Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, false, Flag_QtaNoZero,
                xTipoPUARegolamento, null, null, xFiltroAggiuntivoMateriePrime, false, -1);
        } else {
            elencoProdottiCompleto = RicercaElencoCompletoProdottiMultiCategoria(objP_super_server, objP_server, objP_utenti, $(cIdPiva).val(),
                Sa_Cod, Fabbricato_Cod, TipoDestinazione,
                elemCodArray, null, null,
                soloInGiacenza, JSON.stringify(options.data.filter.filters),
                "", Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, false, Flag_QtaNoZero,
                xTipoPUARegolamento, xFiltroAggiuntivoMateriePrime);
        }
        // -----------------  Richiamo lettura prodotti ----------

        let elencoProdottiLettoJSon = JSON.parse(elencoProdottiCompleto);

        // Se F&F/ZOO/Tabacco
        if (is_FF_FormProdottoUC()) {
            let elencoProdottiDaConsiderare = [];

            if (Array.isArray(elencoProdottiLettoJSon)) {
                if (type === "C") {
                    // In accettazione conferimento considero solo i Trasformati Vegetali / Animali legati a linea con partita iva = piva 
                    // dell'azienda che sta emettendo il documento
                    // (quindi come se i prodotti fossero tutti privati mentre in realtà sono stati creati pubblici ma sono legati a linee private)
                    // più i Trasformati Vegetali / Animali non legati a linea
                    elencoProdottiDaConsiderare = elencoProdottiLettoJSon.filter(function (x) {
                        return (
                            ((x.Elem_Cod === TRASFORMATI_VEGETALI || x.Elem_Cod === TRASFORMATI_ANIMALI) &&
                                x.LegatoALinea === 1 && $(cIdPiva).val() === x.Piva) ||
                            ((x.Elem_Cod === TRASFORMATI_VEGETALI || x.Elem_Cod === TRASFORMATI_ANIMALI) &&
                                x.LegatoALinea !== 1)
                        );
                    });

                } else {
                    // In tutti gli altri casi considero solo i Trasformati Vegetali / Animali legati a linea con partita iva = piva 
                    // dell'azienda che sta emettendo il documento
                    // (quindi come se i prodotti fossero tutti privati mentre in realtà sono stati creati pubblici ma sono legati a linee private)
                    // più i Trasformati Vegetali / Animali non legati a linea
                    // più tutti i prodotti diversi da Trasformati Vegetali / Animali
                    elencoProdottiDaConsiderare = elencoProdottiLettoJSon.filter(function (x) {
                        return (
                            ((x.Elem_Cod === TRASFORMATI_VEGETALI || x.Elem_Cod === TRASFORMATI_ANIMALI) &&
                                x.LegatoALinea === 1 && $(cIdPiva).val() === x.Piva) ||
                            ((x.Elem_Cod === TRASFORMATI_VEGETALI || x.Elem_Cod === TRASFORMATI_ANIMALI) &&
                                x.LegatoALinea !== 1) ||
                            (x.Elem_Cod !== TRASFORMATI_VEGETALI && x.Elem_Cod !== TRASFORMATI_ANIMALI)
                        );
                    });
                }
            }

            let elencoProdottiFinale = [];
            for (let i = 0; i < elencoProdottiDaConsiderare.length; i++) {
                let found = false;
                if (elencoProdottiDaConsiderare[i].Elem_Cod === TRASFORMATI_VEGETALI &&
                    (elencoProdottiDaConsiderare[i].Mat_Cod_OMNI === 0 ||
                        elencoProdottiDaConsiderare[i].Mat_Cod_OMNI === elencoProdottiDaConsiderare[i].Prodotto_Cod * - 1)) {
                    // Ho trovato un prodotto OMNI
                    for (let y = 0; y < elencoProdottiDaConsiderare.length; y++) {
                        if (elencoProdottiDaConsiderare[y].Elem_Cod === TRASFORMATI_VEGETALI &&
                            elencoProdottiDaConsiderare[i].Prodotto_Cod !== elencoProdottiDaConsiderare[y].Prodotto_Cod &&
                            elencoProdottiDaConsiderare[i].Prodotto_Cod === elencoProdottiDaConsiderare[y].Mat_Cod_OMNI * - 1) {
                            // Devo scartare il prodotto perchè c'èuna referenza con lo stesso prodotto
                            found = true;
                            break;
                        }

                    }

                }

                if (!found)
                    elencoProdottiFinale.push(elencoProdottiDaConsiderare[i]);

            }

            // Essendo una multiselect ripasserà solo la chiave, quindi devo avere anche Elem_Cod per poter poi fare la lettura corretta
            for (var i = 0; i < elencoProdottiFinale.length; i++) {
                elencoProdottiFinale[i].Prodotto_Cod = elencoProdottiFinale[i].Elem_Cod + "_" + elencoProdottiFinale[i].Prodotto_Cod;
            }
            options.success(elencoProdottiFinale);
        } else {
            // Non F&F

            // Essendo una multiselect ripasserà solo la chiave, quindi devo avere anche Elem_Cod per poter poi fare la lettura corretta
            for (var i = 0; i < elencoProdottiLettoJSon.length; i++) {
                elencoProdottiLettoJSon[i].Prodotto_Cod = elencoProdottiLettoJSon[i].Elem_Cod + "_" + elencoProdottiLettoJSon[i].Prodotto_Cod;
            }
            options.success(elencoProdottiLettoJSon);
        }
    }
}

function Elenco_Contatti_Clienti() {

    var risultato_lettura;
    var testo = "";
    
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Contatti_Clienti_Agenti",
        "{ piva: '" + $(cIdPiva).val() + "', testoRicerca:'" + testo + "', cliente: 1, fornitore: 0, agente: 0}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Elenco_Contatti_Agenti() {

    var risultato_lettura;
    var testo = "";

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Contatti_Clienti_Agenti",
        "{ piva: '" + $(cIdPiva).val() + "', testoRicerca:'" + testo + "', cliente: 0, fornitore: 0, agente: 1}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Parametri_Qualitativi() {

    var risultato_lettura;
    var param = "{ piva: '" + $(cIdPiva).val() + "'}";
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Parametri_Qualitativi",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Categorie_Magazzino() {

    var risultato_lettura;

    let isFFZooTabacco = 0;
    if (is_FF_FormProdottoUC())
        isFFZooTabacco = 1;

    let param = kendo.stringify({
        type: ParametroType(),
        isFFZooTabacco: isFFZooTabacco 
    });
     
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Categorie_Magazzino",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Categorie_Commerciali() {

    var param = "{ piva: '" + $(cIdPiva).val() + "'}";
    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Categorie_Commerciali",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
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
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Varieta(filtro_specie) {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Varieta",
        "{ filtro_specie: '" + filtro_specie + "'}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function stampa_documento(Id_Agenda, Lav_Cod, Modulo, Tipo_Accettazione)
{
        var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        Id_Agenda: Id_Agenda,
        Lav_Cod: Lav_Cod,
        Modulo: Modulo,
        Tipo_Accettazione: Tipo_Accettazione
    });
    var risp = "";

    ajaxAgronica(indirizzoHttp_DocContabile_WS + "/StampaDocumento",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            var win = window.open(risp);
            win.focus();
        }, function (rispostaErrore)
        {
            MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
        });
}

function stampa_barCode(dataItem, Dettaglio) {

    var Mat_Cod = "";
    var Cal_Cod = "";
    var Lotto = "";
    if (Dettaglio)
    {
        Mat_Cod = dataItem.Mat_Cod != undefined ? dataItem.Mat_Cod: "";
        Cal_Cod = dataItem.Cal_Cod != undefined ? dataItem.Cal_Cod : "";
        Lotto = dataItem.Lotto != undefined ? dataItem.Lotto : "";
    }

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        Id_Agenda: dataItem.Id_Agenda,
        Lav_Cod: dataItem.Lav_Cod,
        Mat_Cod: Mat_Cod,
        Cal_Cod: Cal_Cod,
        Lotto: Lotto,
        Modulo: dataItem.Modulo,
        Tipo_Accettazione: dataItem.Tipo_Accettazione,
        Dettaglio: Dettaglio
    });
    var risp = "";

    ajaxAgronica(indirizzoHttp_DocContabile_WS + "/StampaBarCode",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            var win = window.open(risp);
            win.focus();
        }, function (rispostaErrore) {
            MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
        });
}


function aprimodifica_documento(Id_Agenda, Lav_Cod, Tipo_Operazione, Data_Movimento) {
    //Ho cambiato questa parte perchè così, quando richiamo questa funzione da RisultatoLiquidazione
    //prende l'indirizzo giusto per il webmethod
    let indirizzohttpPagina = "";

    if ($("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== undefined &&
        $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== null &&
        $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== "") {
        indirizzohttpPagina = "../DocContabile_WS.aspx";
    }
    else {
        Salva_Filtri();
        indirizzohttpPagina = indirizzoHttp_DocContabile_WS;
    }


    var Modalita_Protetta = 0;

    if (Tipo_Operazione === 2) {

        if (Data_Movimento == undefined || Data_Movimento == null) {
            Data_Movimento = AGRODATAINIZIO
        }

        var paramcheck = kendo.stringify({
            piva: $(cIdPiva).val(),
            Id_Agenda: Id_Agenda,
            Id_Mov: 0,
            Id_Mov_Det: 0,
            Lav_Cod: Lav_Cod,
            Tipo_Operazione: Tipo_Operazione,
            IgnoraAvvisoWarning: false,
            ModuloGiasLicenziato: 0,
            flagAggiornaConteggi: false,
            dataMov: Data_Movimento
        });

        ajaxAgronicaSync(indirizzohttpPagina + "/VerificaEliminaModificaDocumento",
            paramcheck,
            false,
            function (risposta) {

                let risp = JSON.parse(risposta.RispostaStringa);
                Modalita_Protetta = risp.ModalitaProtetta;
                modifica_documento(Id_Agenda, Lav_Cod, Tipo_Operazione, Modalita_Protetta);

            },
            function (risposta) {
                if (risposta.RispostaConferma === true) {

                    risp = JSON.parse(risposta.RispostaStringa);
                    Modalita_Protetta = risp.ModalitaProtetta;

                    $("#confermaEliminazioneDialog").kendoDialog({
                        width: "400px",
                        title: TraduzioneMultiResx(resxObj, "GestioneModificaDocumenti", "Gestione Modifica Documenti"),
                        closable: false,
                        modal: true,
                        visible: false,
                        content: "<p>" + risp.RispostaStringa + "<p>",
                        actions: [
                            { text: TraduzioneMultiResx(resxObj, "Conferma", "Conferma"), action: function (e) { modifica_documento(Id_Agenda, Lav_Cod, Tipo_Operazione, Modalita_Protetta); } },
                            { text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla"), primary: true }
                        ]
                    });
                    $("#confermaEliminazioneDialog").data("kendoDialog").open();
                }

                else {
                    kendo.alert(risposta.Errore);
                }

            });


    } else {
        //modalità protetta a 0, tanto non serve
        modifica_documento(Id_Agenda, Lav_Cod, Tipo_Operazione, Modalita_Protetta);
    }

}



function modifica_documento(Id_Agenda, Lav_Cod, Tipo_Operazione, Modalita_Protetta) {

    //Ho cambiato questa parte perchè così, quando richiamo questa funzione da RisultatoLiquidazione
    //prende l'indirizzo giusto per il mio webmethod in RisultatoLiquidazione.ascx.vb

    if ($("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== undefined &&
        $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== null &&
        $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== "") {

        var param = kendo.stringify({
            piva: $(cIdPiva).val(),
            Tipo_Operazione: Tipo_Operazione,
            Lav_Cod: Lav_Cod,
            Id_Agenda: Id_Agenda,
            Modalita_Protetta: Modalita_Protetta
        });

        let url = "";

        let indirizzohttpPagina = "../Liquidazione/TestataAnagraficaLiquidazioni.aspx";

        ajaxAgronicaSync(indirizzohttpPagina+"/ComponiURLDocContabile",
            param,
            false,
            function (risposta) {
                url = risposta.RispostaStringa;
            }, null);

        //Prendo l'url per l'iframe di RisultatoLiquidazione
        if (Tipo_Operazione == 0)
            apriKendoWindowDocContabileRisultatoLiquidazioneUC(url, TraduzioneMultiResx(resxObj, "InformazioniLiquidazione", "Informazioni sulla Liquidazione"));
        else if (Tipo_Operazione == 2)
            apriKendoWindowDocContabileRisultatoLiquidazioneUC(url, TraduzioneMultiResx(resxObj, "ModificaLiquidazione", "Modifica Liquidazione"));
    }
    else {
        var param = kendo.stringify({
            piva: $(cIdPiva).val(),
            Id_Agenda: Id_Agenda,
            Lav_Cod: Lav_Cod,
            Tipo_Operazione: Tipo_Operazione,
            Modalita_Protetta: Modalita_Protetta
        });
        var risp = "";

        ajaxAgronica(indirizzohttp + "/ApriModificaDocumento",
            param,
            function (risposta) {
                risp = risposta.RispostaStringa;
                //var win = window.open(risp);
                //win.focus();
                window.location.href = risp;
            }, null);
    }
}

function nuovo_documento() {

    var Lav_Cod = $("#ddl_nuovoOrdine").data("kendoDropDownList").value();
    if (Lav_Cod === "0")
        Lav_Cod = $("#ddl_nuovoDocAcquisto").data("kendoDropDownList").value();
    if (Lav_Cod === "0")
        Lav_Cod = $("#ddl_nuovoDocVendita").data("kendoDropDownList").value();

    if (Lav_Cod === "0") {
        kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareTipoDocumento", "Selezionare il tipo documento!"));
    }
    else {

        Salva_Filtri();

        $('#modalNuovo').modal('hide');

        var param = "{ piva: '" + $(cIdPiva).val() + "', Id_Agenda:" + 0 + ", Lav_Cod:" + Lav_Cod + "}";
        var risp = "";
        ajaxAgronica(indirizzohttp + "/NuovoDocumento",
            param,
            function (risposta) {
                risp = risposta.RispostaStringa;
                window.location.href = risp;
            }, null);

    }
}

function elimina_documento(Id_Agenda, Lav_Cod, IgnoraAvvisoWarning, Data_Movimento) {

    Salva_Filtri();

    if (Data_Movimento == undefined || Data_Movimento == null) {
        Data_Movimento = AGRODATAINIZIO
    }

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        Id_Agenda: Id_Agenda,
        Id_Mov: 0,
        Id_Mov_Det: 0,
        Lav_Cod: Lav_Cod,
        Tipo_Operazione: 3,
        IgnoraAvvisoWarning: IgnoraAvvisoWarning,
        ModuloGiasLicenziato: 0,
        flagAggiornaConteggi: false,
        dataMov: Data_Movimento
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/VerificaEliminaModificaDocumento",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);   
            $("<div></div>").kendoAlert({ title: TraduzioneMultiResx(resxObj, "Cancellazione", "Cancellazione"), content: risp.RispostaStringa }).data("kendoAlert").open();
            Esegui_Report(false);
        },
        function (risposta) {
            if (risposta.RispostaConferma === true) {

                let risp = JSON.parse(risposta.RispostaStringa);                

                $("#confermaEliminazioneDialog").kendoDialog({
                    width: "400px",
                    title: TraduzioneMultiResx(resxObj, "GestioneCancellazioneDocumenti", "Gestione Cancellazione Documenti"),
                    closable: false,
                    modal: true,
                    visible: false,
                    content: "<p>" + risp.RispostaStringa + "<p>",
                    actions: [
                        { text: TraduzioneMultiResx(resxObj, "Conferma", "Conferma"), action: function (e) { elimina_documento(Id_Agenda, Lav_Cod, true, Data_Movimento); } },
                        { text: TraduzioneMultiResx(resxObj, "Annulla", "Annulla"), primary: true }
                    ]
                });
                $("#confermaEliminazioneDialog").data("kendoDialog").open();
            }

            else {
                kendo.alert(risposta.Errore);
            }

        });
    
}

function sblocca_documento(piva, saCod, idAgenda) {

    Salva_Filtri();

    var param = kendo.stringify({ piva: piva, saCod: saCod, idAgenda: idAgenda });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/SbloccaDocumento",
        param,
        false,
        function (risposta) {
            kendo.alert(TraduzioneMultiResx(resxObj, "DocumentoSbloccato", "Documento sbloccato"));
            Esegui_Report(false);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });
}

function blocca_documento(piva, saCod, idAgenda) {

    Salva_Filtri();

    var param = kendo.stringify({ piva: piva, saCod: saCod, idAgenda: idAgenda });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/BloccaDocumento",
        param,
        false,
        function (risposta) {
            kendo.alert(TraduzioneMultiResx(resxObj, "DocumentoBloccato", "Documento bloccato"));
            Esegui_Report(false);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });
}


function getParametriGrigliaTestata(pagina, nomeDiv) {
    var jsonDaSalvare = "";
    var grid = $('#' + nomeDiv).data('kendoGrid');
    if (grid !== undefined) {
        var dataSource = grid.dataSource;
        var columns = grid.columns;
        var pageSize = dataSource.pageSize();
        var sort = dataSource.sort();
        var filter = dataSource.filter();
        var group = dataSource.group();
        var page = dataSource.page();

        //ultimaRigaSelezionataGrigliaTestata.page = page;
        var currentRow = 0; //ultimaRigaSelezionataGrigliaTestata;
        var options = { pagina: pagina, nomeDiv: nomeDiv, columns: columns, page: page, pageSize: pageSize, sort: sort, filter: filter, group: group, currentRow: currentRow };
        jsonDaSalvare = kendo.stringify(fixParametriGrigliaKendo(options));
    }
    return jsonDaSalvare;
}

function getParametriGrigliaDettaglio(pagina, nomeDiv) {
    var jsonDaSalvare = "";
    var grid = $('#' + nomeDiv).data('kendoGrid');
    if (grid !== undefined) {
        var dataSource = grid.dataSource;
        var columns = grid.columns;
        var pageSize = dataSource.pageSize();
        var sort = dataSource.sort();
        var filter = dataSource.filter();
        var group = dataSource.group();
        var page = dataSource.page();

        //ultimaRigaSelezionataGrigliaDettaglio.page = page;
        var currentRow = 0; //ultimaRigaSelezionataGrigliaDettaglio;
        var options = { pagina: pagina, nomeDiv: nomeDiv, columns: columns, page: page, pageSize: pageSize, sort: sort, filter: filter, group: group, currentRow: currentRow };
        jsonDaSalvare = kendo.stringify(fixParametriGrigliaKendo(options));
    }
    return jsonDaSalvare;
}

function Lista_Report() {
    var risultato_lettura;
    ajaxAgronicaSync(indirizzohttp + "/ListaReport",
        "{ }",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            var objVuoto = { "cod": "", "desc": "" };
            risp.unshift(objVuoto);
            risultato_lettura = risp;
        }, null);
    return risultato_lettura;
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
        dataSource.columns(options.columns);
        dataSource.rows(options.rows);
        dataSource.measures(options.measures);
        dataSource.filter(options.filters);
        personalizzazioniPivot = null;
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
        for (i = 0; i < savedColumns.length; i++) {
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
        for (i = savedColumns.length - 1; i >= 0; i--) {
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
            for (i = ordinam.length - 1; i >= 0; i--) {
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
            for (i = filtri.length - 1; i >= 0; i--) {
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
            for (i = raggrup.length - 1; i >= 0; i--) {
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

// i18n Questa e le due seguenti funzioni sono utilizzate solo dalla funzionalità di creazione nuovo documento, attualmente non utilizzata
function leggiDocumentiOrdini(options) {

    var elencoCausali = [
        { "LAV_COD": "2004", "LAV_DES": "Ordine di acquisto" },
        { "LAV_COD": "2002", "LAV_DES": "Ordine di vendita" }
    ];
    options.success(elencoCausali.sort((a, b) => (a.LAV_DES > b.LAV_DES) ? 1 : -1));
}

function leggiDocumentiAcquisti(options) {

    var elencoCausali = [
        { "LAV_COD": "1075", "LAV_DES": "Distinta Carico" },
        { "LAV_COD": "1025", "LAV_DES": "DDT Ricevuto" },
        { "LAV_COD": "1077", "LAV_DES": "Auto DDT Emesso" }
    ];
    options.success(elencoCausali.sort((a, b) => (a.LAV_DES > b.LAV_DES) ? 1 : -1));
}

function leggiDocumentiVendite(options) {

    var elencoCausali = [
        { "LAV_COD": "1001", "LAV_DES": "Fatture" },
        { "LAV_COD": "1020", "LAV_DES": "Corrispettivi di vendita" },
        { "LAV_COD": "1053", "LAV_DES": "Ricevute fiscali" },
        { "LAV_COD": "1028", "LAV_DES": "Autoconsumo" },
        { "LAV_COD": "1069", "LAV_DES": "DDT corrispettivi" },
        { "LAV_COD": "1031", "LAV_DES": "DDT" },
        { "LAV_COD": "1071", "LAV_DES": "MVV" },
        { "LAV_COD": "2002", "LAV_DES": "Ordini da evadere" },
        { "LAV_COD": "1003", "LAV_DES": "Note di accredito" }
    ];
    options.success(elencoCausali.sort((a, b) => (a.LAV_DES > b.LAV_DES) ? 1 : -1));
}

/*
function Salva_Filtri() {

    var idControlloTestata = DammiIDControlloGriglia(false);
    var idControlloDettaglio = DammiIDControlloGriglia(true);
    var griglie = [];
    var keys = [];

    var parametriGrigliaTestata = getParametriGrigliaTestata(location.pathname, idControlloTestata);
    var parametriGrigliaDettaglio = getParametriGrigliaDettaglio(location.pathname, idControlloDettaglio);

    griglie.push(CreaNuovoOggettoGrilia(idControlloTestata, parametriGrigliaTestata));
    griglie.push(CreaNuovoOggettoGrilia(idControlloDettaglio, parametriGrigliaDettaglio));

    var grigliaCorrente = $("#TipoOutput").data("kendoButtonGroup").current().index();

    var prodottiSelezionati = KendoMultisel("multiselProdotti").dataItems();
    var prodotti = [];
    for (var i = 0; i < prodottiSelezionati.length; i += 1) {
        var prodotto = prodottiSelezionati[i];
        prodotti.push({
            Prodotto_Cod: prodotto.Prodotto_Cod,
            Prodotto_Des: prodotto.Prodotto_Des
        });
    }

    var contattiSelezionati = KendoMultisel("multiselContatti").dataItems();
    var contatti = [];
    for (var i = 0; i < contattiSelezionati.length; i += 1) {
        var contatto = contattiSelezionati[i];
        contatti.push({
            cod_contatto: contatto.cod_contatto,
            nome: contatto.nome
        });
    }

    var type = ParametroType();
    var doc_type = ParametroDocType();
    var keyPrincipale = type + "-" + doc_type;

    var filtro = new Object();
    filtro._chiave = keyPrincipale;
    filtro._griglia = grigliaCorrente;
    filtro._dataRegDal = $('input[name$="Txt_DataRegDal"]').val();
    filtro._dataRegAl = $('input[name$="Txt_DataRegAl"]').val();
    filtro._gruppiDocumenti = KendoMultisel("multiselGruppoDocumento").value().join("|");
    filtro._descrizione = $('input[name$="txt_operazione"]').val();
    filtro._causali = KendoMultisel("multiselCausale").value().join("|");
    filtro._clienti = contatti;
    filtro._agenti = KendoMultisel("multiselAgenti").value().join("|");
    filtro._causali = KendoMultisel("multiselCausale").value().join("|");
    filtro._specie = KendoMultisel("multiselSpecie").value().join("|");
    filtro._varieta = KendoMultisel("multiselVarieta").value().join("|");
    filtro._prodotti = prodotti;
    filtro._categorie = KendoMultisel("multiselCategorie").value().join("|");
    filtro._categcommerciali = KendoMultisel("multiselCategCommle").value().join("|");
    filtro._griglie = griglie;

    keys.push(filtro); 
    var oggettoPrincipale = new Object();
    oggettoPrincipale._versione = versioneJsonFiltri;
    oggettoPrincipale.Keys = keys;

    var parametriReport = kendo.stringify(oggettoPrincipale);

    //var parametriReport = kendo.stringify({
    //    "_versione": versioneJsonFiltri,
    //    "_griglia": grigliaCorrente,
    //    "_dataRegDal": $('input[name$="Txt_DataRegDal"]').val(),
    //    "_dataRegAl": $('input[name$="Txt_DataRegAl"]').val(),
    //    "_gruppiDocumenti": KendoMultisel("multiselGruppoDocumento").value().join("|"),
    //    "_descrizione": $('input[name$="txt_operazione"]').val(),
    //    "_causali": KendoMultisel("multiselCausale").value().join("|"),
    //    "_clienti": contatti, 
    //    "_agenti": KendoMultisel("multiselAgenti").value().join("|"),
    //    "_causali": KendoMultisel("multiselCausale").value().join("|"),
    //    "_specie": KendoMultisel("multiselSpecie").value().join("|"),
    //    "_varieta": KendoMultisel("multiselVarieta").value().join("|"),
    //    "_prodotti": prodotti, 
    //    "_categorie": KendoMultisel("multiselCategorie").value().join("|"),
    //    "_categcommerciali": KendoMultisel("multiselCategCommle").value().join("|"),
    //    "_griglie": griglie
    //});

    var parametri = kendo.stringify({ "parametri": parametriReport });
    ajaxAgronica(indirizzohttp + '/SalvaFiltri', parametri,
        function (risposta) {
            if (risposta.RispostaOK) {
                var risp = risposta.RispostaStringa;
            }
            else console.log("Errore nel salvataggio dei filtri per la ricerca documenti: " + risposta.Errore);
        }, null);

}
*/

function Salva_Filtri() {

    var idControlloTestata = DammiIDControlloGriglia(false);
    var idControlloDettaglio = DammiIDControlloGriglia(true);
    var griglie = [];

    var parametriGrigliaTestata = getParametriGrigliaTestata(location.pathname, idControlloTestata);
    var parametriGrigliaDettaglio = getParametriGrigliaDettaglio(location.pathname, idControlloDettaglio);

    griglie.push(CreaNuovoOggettoGrilia(idControlloTestata, parametriGrigliaTestata));
    griglie.push(CreaNuovoOggettoGrilia(idControlloDettaglio, parametriGrigliaDettaglio));

    var grigliaCorrente = $("#TipoOutput").data("kendoButtonGroup").current().index();

    var prodottiSelezionati = KendoMultisel("multiselProdotti").dataItems();
    var prodotti = [];
    for (let i = 0; i < prodottiSelezionati.length; i += 1) {
        var prodotto = prodottiSelezionati[i];
        prodotti.push({
            Prodotto_Cod: prodotto.Prodotto_Cod,
            Prodotto_Des: prodotto.Prodotto_Des
        });
    }

    var contattiSelezionati = KendoMultisel("multiselContatti").dataItems();
    var contatti = [];
    for (let i = 0; i < contattiSelezionati.length; i += 1) {
        var contatto = contattiSelezionati[i];
        contatti.push({
            cod_contatto: contatto.cod_contatto,
            nome: contatto.nome
        });
    }

    var parametriReport = kendo.stringify({
        "_versione": versioneJsonFiltri,
        "_griglia": grigliaCorrente,
        "_dataRegDal": $('input[name$="Txt_DataRegDal"]').val(),
        "_dataRegAl": $('input[name$="Txt_DataRegAl"]').val(),
        "_gruppiDocumenti": KendoMultisel("multiselGruppoDocumento").value().join("|"),
        "_descrizione": $('input[name$="txt_operazione"]').val(),
        "_causali": KendoMultisel("multiselCausale").value().join("|"),
        "_centriAziendali": KendoMultisel("multiselCentroAziendale").value().join("|"),
        "_clienti": contatti,
        "_agenti": KendoMultisel("multiselAgenti").value().join("|"),
        "_specie": KendoMultisel("multiselSpecie").value().join("|"),
        "_varieta": KendoMultisel("multiselVarieta").value().join("|"),
        "_prodotti": prodotti,
        "_categorie": KendoMultisel("multiselCategorie").value().join("|"),
        "_categcommerciali": KendoMultisel("multiselCategCommle").value().join("|"),
        "_griglie": griglie
    });

    var parametri = kendo.stringify({ "parametri": parametriReport });
    ajaxAgronica(indirizzohttp + '/SalvaFiltri', parametri,
        function (risposta) {
            if (risposta.RispostaOK) {
                var risp = risposta.RispostaStringa;
            }
            else console.log("Errore nel salvataggio dei filtri per la ricerca documenti: " + risposta.Errore);
        }, null);

}


function Leggi_Filtri() {

    var param = kendo.stringify(
        {
            versione: versioneJsonFiltri
        });

    ajaxAgronicaSync(indirizzohttp + '/LeggiFiltri', param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                if (risposta.RispostaStringa !== "") {
                    var parametri = JSON.parse(risposta.RispostaStringa);
                    Personalizza_Report(parametri);
               } 
            } else console.log("Errore nella lettura impostazioni report vendite: " + risposta.Errore);
        }, null);
}

function Personalizza_Report(parametri) {

    set_data("Txt_DataRegDal", parametri._dataRegDal, null);
    set_data("Txt_DataRegAl", parametri._dataRegAl, null);
    $('input[name$="txt_operazione"]').val(parametri._descrizione);

    if (parametri._causali !== undefined)
        KendoMultisel("multiselCausale").value(parametri._causali.split("|"));

    if (parametri._centriAziendali !== undefined)
        KendoMultisel("multiselCentroAziendale").value(parametri._centriAziendali.split("|"));

    if (parametri._agenti !== undefined)
        KendoMultisel("multiselAgenti").value(parametri._agenti.split("|"));

    if (parametri._causali !== undefined)
        KendoMultisel("multiselCausale").value(parametri._causali.split("|"));

    if (parametri._specie !== undefined) {
        KendoMultisel("multiselSpecie").value(parametri._specie.split("|"));
        SpecieChange();
        if (parametri._varieta !== undefined)
            KendoMultisel("multiselVarieta").value(parametri._varieta.split("|"));
    }

    if (parametri._gruppiDocumenti !== undefined)
        KendoMultisel("multiselGruppoDocumento").value(parametri._gruppiDocumenti.split("|"));

    var prodottiSelezionati = parametri._prodotti;
    var prodotti = [];
    for (let i = 0; i < prodottiSelezionati.length; i += 1) {
        var prodotto = prodottiSelezionati[i];
        prodotti.push(prodotto.Prodotto_Cod);
    }
    KendoMultisel("multiselProdotti").dataSource.data(prodottiSelezionati);
    KendoMultisel("multiselProdotti").value(prodotti);

    var contattiSelezionati = parametri._clienti;
    var contatti = [];
    for (let i = 0; i < contattiSelezionati.length; i += 1) {
        var contatto = contattiSelezionati[i];
        contatti.push(contatto.cod_contatto);
    }
    KendoMultisel("multiselContatti").dataSource.data(contattiSelezionati);
    KendoMultisel("multiselContatti").value(contatti);

    if (parametri._categorie !== undefined)
        KendoMultisel("multiselCategorie").value(parametri._categorie.split("|"));
    if (parametri._categcommerciali !== undefined)
        KendoMultisel("multiselCategCommle").value(parametri._categcommerciali.split("|"));
    

    var TipoOutput = $("#TipoOutput").kendoButtonGroup().data("kendoButtonGroup");
    TipoOutput.select(parametri._griglia);
    buttonGroupSelected = parametri._griglia;
    TipoOutput.trigger("select");

    personalizzazioniGriglie = parametri._griglie;

}

function pulisci_filtri()
{
    var kendoConfirm = $("<div></div>").kendoConfirm({
        title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
        messages: { okText: TraduzioneMultiResx(resxObj, "Si", "Sì"), cancel: TraduzioneMultiResx(resxObj, "No", "No") },
        content: TraduzioneMultiResx(resxObj, "EffettuarePuliziaFiltri", "Effettuare la pulizia dei filtri ?")
    }).data("kendoConfirm");
    kendoConfirm.result.done(function () {
        var parametri = "{ }";
        ajaxAgronica(indirizzohttp + '/PulisciFiltri', parametri,
            function (risposta) {
                if (risposta.RispostaOK)
                {
                    KendoMultisel("multiselCausale").value(""); 
                    var startYearDate = formattedDate(new Date(new Date().getFullYear(), 0, 1), "/");
                    set_data("Txt_DataRegDal", startYearDate, null);
                    set_data("Txt_DataRegAl", "", null);
                    //KendoMultisel("multiselClienti").value("");
                    KendoMultisel("multiselGruppoDocumento").value("");
                    KendoMultisel("multiselContatti").value("");
                    KendoMultisel("multiselAgenti").value("");
                    KendoMultisel("multiselSpecie").value("");
                    KendoMultisel("multiselVarieta").value("");

                    KendoMultisel("multiselCategorie").value("");
                    KendoMultisel("multiselCategCommle").value("");
                    KendoMultisel("multiselProdotti").value("");

                    personalizzazioniGriglie = null;
                    Esegui_Report(false);

                }
                else console.log("Errore nella lettura impostazioni report vendite: " + risposta.Errore);
            }, null);
    });

    kendoConfirm.open();

}

function CreaNuovoOggettoGrilia(idControllo, valore)
{
    var griglia = new Object();
    griglia.IdControllo = idControllo;
    griglia.Personalizzazioni = valore;
    return griglia;
}


function RiempiCentri(options) {
    var centriAziendali = LeggiCentri();
    options.success(centriAziendali);
}

function LeggiCentri() {

    var centriAziendali = [];
    var piva = $(cIdPiva).val();
    var parametri = kendo.stringify({ "objP_server": objP_server, "PrimaRiga_Flag": false, "PrimaRiga_Text": "", "PrimaRiga_Value": "", "Piva": piva, "Flag_SoloCentriAttivi": false, "Tipo_Value": 2 });
    ajaxAgronicaSync(pathCoreWS + "Anagrafica/CentroAziendale.asmx/LeggiCentriConFiltroUtente",
        parametri, false,
        function (risposta) {
            centriAziendali = JSON.parse(risposta.RispostaStringa);
        }, null);
    return centriAziendali;
}

function UrlGestioneCampionamento(id_mov_det)
{
    var piva = $(cIdPiva).val();
    var url = "";


    //Ho cambiato questa parte perchè così, quando richiamo questa funzione da RisultatoLiquidazione
    //prende l'indirizzo giusto per il webmethod
    let indirizzohttpPagina = "";
    let ChiamatodaRisLiqu = "";
    if ($("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== undefined &&
        $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== null &&
        $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== "") {
        indirizzohttpPagina = $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val();
        ChiamatodaRisLiqu = "True";
    }else {
        indirizzohttpPagina = indirizzohttp;
        ChiamatodaRisLiqu = "False";
    }


    var param = kendo.stringify(
        {
            piva: piva,
            id_mov_det: id_mov_det,
            chiamatodaRisLiqu: ChiamatodaRisLiqu
        });

    ajaxAgronicaSync(indirizzohttpPagina + "/UrlGestioneCampionamento",
        param,
        false,
        function (risposta) {
            risp = risposta.RispostaStringa
            url = risp;
        }, function (errore) { });

    return url;
}


function Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(Id_Mov_Det) {

    var result = false;
    var param = "{ piva: '" + $(cIdPiva).val() +
        "',  Id_Mov_Det: " + Id_Mov_Det +
        "}";

    var risp = "";

    //Ho cambiato questa parte perchè così, quando richiamo questa funzione da RisultatoLiquidazione
    //prende l'indirizzo giusto per il webmethod
    let indirizzohttpPagina = "";
    if ($("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== undefined &&
        $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== null &&
        $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val() !== "")
        indirizzohttpPagina = $("input[name$='hdindirizzohttpRicercaDocContabiliRisLiqUC']").val();
    else
        indirizzohttpPagina = indirizzohttp;

    ajaxAgronicaSync(indirizzohttpPagina + "/Leggi_Id_Testata_Griglia_Da_Movim_Conferimento",
        param, false,
        function (risposta) {
            result = true;
        }, function (risposta) {
            kendo.alert(risposta.Errore);
            //MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        }
    );
    return result;
}

function Schedula_Fatturazione(ids_agende, ids_mov_det, parametriRottura, dataFatturazione)
{

    var params2 = "{ inData: '" + kendo.stringify(parametriRottura) + "' }";

    var params = "{ lav_cod_destinazione: " + LAV_COD_DESTINAZIONE.FATTURE_EMESSE + ", " +
        " ids_agende: '" + ids_agende.join(",") + "', " +
        " ids_mov_det: '" + ids_mov_det.join(",") + "', " +
        " parametriRottura: '" + kendo.stringify(parametriRottura) + "', " +
        " data: '" + dataFatturazione + "' } ";

    // Eseguo Check Preliminari
    ajaxAgronicaSync(indirizzohttp + "/Salva_Schedulazione_Fatture",
        params, false,
        function (risposta) {
            MessaggioTuttoOK_Bootstrap(TraduzioneMultiResx(resxObj, "SchedulazioneDatiFatturazioneEseguitaConSuccesso",
                "Schedulazione dati x fatturazione eseguita con successo."), "DIV_Messaggi");
            return true;
        },
        function (risposta) {
            errori = risposta.RispostaStringa;
            MessaggioErrore_Bootstrap(errori, "DIV_Messaggi");
            return false;
        });
    
}




function esegui_Fatturazione() {

    var param = kendo.stringify({
        ID: "0"
    });
    var risp = "";

    ajaxAgronica(indirizzoHttp_DocContabile_WS + "/Scheduling_Documenti_Contabili",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            alert(risp);

            //Nuova Ricerca
            Esegui_Report(false);

            //var win = window.open(risp);
            //win.focus();
        }, function (rispostaErrore) {
        MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
    });

    
    
}

function LinkPassaggioDiStato(arrPraticheCod) {
    var urlPagina = "";

    let _praticheCod = "";
    if (Array.isArray(arrPraticheCod)) {
        _praticheCod = arrPraticheCod.join("_");
    }

    var params = kendo.stringify({
        praticheCod: _praticheCod
    });

    // TODO: da spostare in indirizzoHttp_DocContabile_WS
    ajaxAgronicaSync(indirizzohttp + "/LinkPassaggioDiStato",
        params,
        false,
        // callback success
        function (risposta) {
            urlPagina = risposta.RispostaStringa;
        },
        // callback error
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'RicercaDocContabili_LinkPassaggioDiStato', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore server nell'esecuzione della chiamata ajax 'RicercaDocContabili_LinkPassaggioDiStato': " +
                    risposta.Errore,
                    "DIV_Messaggi");
            }
        });

    return urlPagina;
}

function VerificaPermessoVisibilitaGruppiMerce(Id_Agenda, Lav_Cod) {

    var mesErroreServer = "";

    if (Array.isArray(setupGestioneGruppiMerce) && setupGestioneGruppiMerce.length > 0) {

        var param = kendo.stringify({
            piva: $(cIdPiva).val(),
            Id_Agenda: Id_Agenda,
            Lav_Cod: Lav_Cod
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/VerificaPermessoVisibilitaGruppiMerce",
            param,
            false,
            function (rispServer) {
                // In caso il documento sia visibile il server risponde con la proprietà RispostaOK = true
                mesErroreServer = "";
            },
            function (rispServer) {
                mesErroreServer = rispServer.Errore;
            });
    }

    return mesErroreServer;
}

/**
 * 
 * @param {any} listaDocumenti [ {Piva, Id_Agenda, Lav_Cod} ]
 */
function VerificaPermessoVisibilitaGruppiMerceMultiDocumento(listaDocumenti) {

    var mesErroreServer = "";

    if (Array.isArray(setupGestioneGruppiMerce) && setupGestioneGruppiMerce.length > 0) {

        var param = kendo.stringify({
            listaDocumenti: kendo.stringify(listaDocumenti)
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/VerificaPermessoVisibilitaGruppiMerceMultiDocumento",
            param,
            false,
            function (rispServer) {
                // In caso tutti i documenti siano visibili il server risponde con la proprietà RispostaOK = true
                mesErroreServer = "";
            },
            function (rispServer) {
                mesErroreServer = rispServer.Errore;
            });
    }

    return mesErroreServer;
}

function LeggiLogInvioDettaglio(arrAgende) {
    var dtLogInvio = null;

    var params = kendo.stringify({
        piva: $(cIdPiva).val(),
        idAgende: arrAgende
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/LeggiLogInvioDettaglio",
        params,
        false,
        // callback success
        function (risposta) {

            try {
                //console.log(risposta);
                dtLogInvio = JSON.parse(risposta.RispostaStringa);
                //console.log(dtLogInvio);

            } catch (e) {
                console.log(e);
            }

        },
        // callback error
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'RicercaDocContabili_LeggiLogInvioDettaglio', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore server nell'esecuzione della chiamata ajax 'RicercaDocContabili_LeggiLogInvioDettaglio': " +
                    risposta.Errore,
                    "DIV_Messaggi");
            }
        });

    return dtLogInvio;
}



function ValorizzazioneConferimenti() {

    let isDettaglio = false;
    if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 1) {
        isDettaglio = true;
    } else if ($("#TipoOutput").data("kendoButtonGroup").current().index() === 0) {
        isDettaglio = false;
    }

    let kGrid = KendoGrid(DammiIDControlloGriglia(isDettaglio));
    let dsKGrid = kGrid.dataSource;

    let docSelezionati = [];

    // Non uso la funzione select() della kendogrid per ottenere le righe in quanto questa restituisce solo quelle selezionate nella pagina corrente
    for (let riga of dsKGrid.data()) {
        if (riga.Selected === true) {
            docSelezionati.push(riga);
        }
    }

    if (docSelezionati.length === 0) {
        return;
    }

    let setIdAgenda = new Set(); // Il set rappresenta una lista univoca, utile in caso di selezione con la griglia dei dettagli

    for (let i = 0; i < docSelezionati.length; i++) {

        setIdAgenda.add(docSelezionati[i].Id_Agenda);

    }

    var risultato_lettura;
    var testo = "";

    ajaxAgronicaSync(indirizzohttp + "/ValorizzazioneConferimenti",
        "{ piva: '" + $(cIdPiva).val() + "', elenco:'" + Array.from(setIdAgenda) + "'}",
        false,
        function (risposta) {
            risp = risposta.RispostaStringa;
            kendo.alert(risp);
        }, function (rispostaErrore) {
            MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
        });


    return risultato_lettura;


    //let dsRisControlli = LeggiLogInvioDettaglio(Array.from(setIdAgenda));

    //if (dsRisControlli === null || dsRisControlli === undefined) {
    //    return;
    //}

    //if (dsRisControlli.length === 0) {
    //    $("<div></div>").kendoAlert({
    //        content: "I documenti selezionati possono essere esportati senza errori",
    //        title: "Risultato controlli"
    //    }).data("kendoAlert").open();
    //}
    //else {
    //    MostraRisultatiControlliInvio(dsRisControlli);
    //}
}
