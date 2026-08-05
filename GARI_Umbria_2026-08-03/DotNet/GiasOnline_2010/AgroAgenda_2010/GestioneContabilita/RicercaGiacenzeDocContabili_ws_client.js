//////////////////////////////////////////////////////////
// Report AggiornaCuboReportVendite
//////////////////////////////////////////////////////////

var indirizzohttp = "./RicercaGiacenzeDocContabili.aspx";
var indirizzohttp_Ricerca_Documenti = "Contab/RicercaDocumenti.asmx";
var indirizzoHttp_DocContabile_WS = "../GestioneContabilita/DocContabile_WS.aspx";

function RicercaReportDettaglio(options) {

    var piva = $(cIdPiva).val();
    var docNr = 0;
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
    var _dataRegDal = KendoDate("Txt_DataRegDal").value();
    var _dataRegAl = KendoDate("Txt_DataRegAl").value();
    var _clienti = KendoMultisel("multiselContatti").value().join("|");
    var _specie = KendoMultisel("multiselSpecie").value().join("|");
    var _varieta = KendoMultisel("multiselVarieta").value().join("|");
    var _prodotti = KendoMultisel("multiselProdotti").value().join("|");
    var _dataGiacenza = KendoDate("Txt_DataGiacenza").value();
    var _lotto = $("#Txt_Lotto").data("kendoTextBox").value();
    var inGiacenza = getKendoSwitch("cb_prod_giacenza");
    var _categorie = KendoMultisel("multiselCategorie").value().join("|");
    var _soloDDTNonFatturati = getKendoSwitch("cb_ddt_non_fatturati");
    var _soloOrdiniNonSpediti = getKendoSwitch("cb_ordini_non_Spediti");
    var _centriAziendali = KendoMultisel("multiselCentroAziendale").value().join("|");
    var _fabbricato = Get_KendoDDLValue("id_ddlFabbricato", "0_0_0");

    return RicercaDocumenti(_type, _doc_type, true, piva, report, cubo, _descrizione,
        _docNumeroSin, _docNumero, _docNumeroDes, _nrRiga, _dataRegDal,
        _dataRegAl, _centriAziendali, _fabbricato, _clienti, _specie, _varieta, _prodotti,
        _categorie, _dataGiacenza, _lotto, inGiacenza, _soloDDTNonFatturati, _soloOrdiniNonSpediti, options);
}

function RicercaDocumenti(type, doc_type, dettaglio, piva, report, cubo, descrizione,
    docNumeroSin, docNumero, docNumeroDes, nrRiga, dataRegDal, dataRegAl, centriAziendali,
    fabbricato, clienti, specie, varieta, prodotti, categorie,
    dataGiacenza, lotto, inGiacenza, soloDDTNonFatturati, soloOrdiniNonSpediti, options) {

    var objFilters = kendo.stringify({
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
        _fabbricato: fabbricato,
        _clienti: clienti,
        _specie: specie,
        _varieta: varieta,
        _prodotti: prodotti,
        _dataGiacenza: dataGiacenza,
        _lotto: lotto,
        ProdottiInGiacenza: inGiacenza,
        _categorie: categorie,
        _soloDDTNonFatturati: soloDDTNonFatturati,
        _soloOrdiniNonSpediti: soloOrdiniNonSpediti//,
        //objP_server: objP_server
    });

    ajaxAgronica(indirizzohttp + "/LeggiDocumenti",
        kendo.stringify({ filtriRicerca: objFilters }),
        function (risposta) {
            var rispServer = JSON.parse(risposta.RispostaStringa);
            options.success(rispServer);
            //Applica_Personalizzazioni_Griglie();
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });
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

function RiempiCategorie(options) {
    var elencoCategorie = Leggi_Categorie_Magazzino();
    options.success(elencoCategorie);
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
        { "TYPE": "A", "DOC_TYPE": "O", "LAV_COD": "2004", "GRP_DES": "Ordini Acquisto" },
        { "TYPE": "A", "DOC_TYPE": "C", "LAV_COD": "1025;1075;1077", "GRP_DES": "Consegne Acquisto" },
        { "TYPE": "A", "DOC_TYPE": "F", "LAV_COD": "1000;1002", "GRP_DES": "Consegne Fatture / Note Credito Acquisto" },
        { "TYPE": "V", "DOC_TYPE": "O", "LAV_COD": "2002", "GRP_DES": "Ordini Vendita" },
        { "TYPE": "V", "DOC_TYPE": "C", "LAV_COD": "1031;1069", "GRP_DES": "Consegne Vendita" },
        { "TYPE": "V", "DOC_TYPE": "F", "LAV_COD": "1001;1003", "GRP_DES": "Fatture / Note Credito Vendita" },
        { "TYPE": "C", "DOC_TYPE": "C", "LAV_COD": "1054;1076;1078", "GRP_DES": "Conferimenti" }
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

        var elencoProdottiCompleto = RicercaElencoCompletoProdotti(objP_super_server, objP_server, objP_utenti, $(cIdPiva).val(),
            Sa_Cod, Fabbricato_Cod, TipoDestinazione,
            Elem_Cod, soloInGiacenza, JSON.stringify(options.data.filter.filters),
            "", Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, false, Flag_QtaNoZero, xTipoPUARegolamento, null, null, "", false, -1);

        var risp = JSON.parse(elencoProdottiCompleto);

        let elencoProdottiSoloLinea = [];

        // In tutti i casi  considero solo i prodotti legati a linea e con partita iva = piva dell'azienda che sta emettendo il documento
        // se sono in un Trasformato Vegetale oppure tutti i prodotti diversi da Trasformati Vegetali
        if (Array.isArray(risp)) {
            elencoProdottiSoloLinea = risp.filter(function (x) {
                return (
                    (x.LegatoALinea === 1 && $(cIdPiva).val() === x.Piva && x.Elem_Cod === TRASFORMATI_VEGETALI) ||
                    (x.Elem_Cod !== TRASFORMATI_VEGETALI)
                )
            });
        }

        let elencoProdottiFinale = [];
        for (let i = 0; i < elencoProdottiSoloLinea.length; i++) {
            let found = false;
            if (elencoProdottiSoloLinea[i].Elem_Cod === TRASFORMATI_VEGETALI &&
                (elencoProdottiSoloLinea[i].Mat_Cod_OMNI === 0 ||
                    elencoProdottiSoloLinea[i].Mat_Cod_OMNI === elencoProdottiSoloLinea[i].Prodotto_Cod * - 1)) {
                // Ho trovato un prodotto OMNI
                for (let y = 0; y < elencoProdottiSoloLinea.length; y++) {
                    if (elencoProdottiSoloLinea[y].Elem_Cod === TRASFORMATI_VEGETALI &&
                        elencoProdottiSoloLinea[i].Prodotto_Cod !== elencoProdottiSoloLinea[y].Prodotto_Cod &&
                        elencoProdottiSoloLinea[i].Prodotto_Cod === elencoProdottiSoloLinea[y].Mat_Cod_OMNI * - 1) {
                        // Devo scartare il prodotto perchè c'èuna referenza con lo stesso prodotto
                        found = true;
                        break;
                    }

                }

            }

            if (!found)
                elencoProdottiFinale.push(elencoProdottiSoloLinea[i]);

        }

        // Essendo una multiselect ripasserà solo la chiave, quindi devo avere anche Elem_Cod per poter poi fare la lettura corretta
        for (var i = 0; i < elencoProdottiFinale.length; i++) {
            elencoProdottiFinale[i].Prodotto_Cod = elencoProdottiFinale[i].Elem_Cod + "_" + elencoProdottiFinale[i].Prodotto_Cod;
        }
        options.success(elencoProdottiFinale);
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

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Categorie_Magazzino",
        "{ }",
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

    ajaxAgronica(indirizzohttp + "/StampaBarCode",
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
                        title: "Gestione Modifica Documenti",
                        closable: false,
                        modal: true,
                        visible: false,
                        content: "<p>" + risp.RispostaStringa + "<p>",
                        actions: [
                            { text: "Conferma", action: function (e) { modifica_documento(Id_Agenda, Lav_Cod, Tipo_Operazione, Modalita_Protetta); } },
                            { text: "Annulla", primary: true }
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
            apriKendoWindowDocContabileRisultatoLiquidazioneUC(url, "Informazioni sulla Liquidazione");
        else if (Tipo_Operazione == 2)
            apriKendoWindowDocContabileRisultatoLiquidazioneUC(url, "Modifica Liquidazione");
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
            $("<div></div>").kendoAlert({ title: "Cancellazione", content: risp.RispostaStringa }).data("kendoAlert").open();
            Esegui_Report(false);
        },
        function (risposta) {
            if (risposta.RispostaConferma === true) {

                let risp = JSON.parse(risposta.RispostaStringa);                

                $("#confermaEliminazioneDialog").kendoDialog({
                    width: "400px",
                    title: "Gestione Cancellazione Documenti",
                    closable: false,
                    modal: true,
                    visible: false,
                    content: "<p>" + risp.RispostaStringa + "<p>",
                    actions: [
                        { text: "Conferma", action: function (e) { elimina_documento(Id_Agenda, Lav_Cod, true, Data_Movimento); } },
                        { text: "Annulla", primary: true }
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

            alert("Documento sbloccato");

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

// ripristina personalizzazioni griglia report vendite
function Personalizza_Griglia() {
    var grid = $('#tab_testata_griglia_report_vendite').data('kendoGrid');
    if (grid !== undefined && personalizzazioni && personalizzazioni !== "") {
        setPersonalizzazioniGrigliaKendo(grid, JSON.parse(personalizzazioni));
        personalizzazioni = null;
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

function Salva_Filtri() {

    var idControlloDettaglio = DammiIDControlloGriglia();

    var parametriGrigliaDettaglio = getParametriGrigliaDettaglio(location.pathname, idControlloDettaglio);

    var griglia = CreaNuovoOggettoGrilia(idControlloDettaglio, parametriGrigliaDettaglio);

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
        "_dataRegDal": $('input[name$="Txt_DataRegDal"]').val(),
        "_dataRegAl": $('input[name$="Txt_DataRegAl"]').val(),
        "_descrizione": $('input[name$="txt_operazione"]').val(),
        "_centriAziendali": KendoMultisel("multiselCentroAziendale").value().join("|"),
        "_clienti": contatti,
        "_specie": KendoMultisel("multiselSpecie").value().join("|"),
        "_varieta": KendoMultisel("multiselVarieta").value().join("|"),
        "_prodotti": prodotti,
        "_categorie": KendoMultisel("multiselCategorie").value().join("|"),
        "_griglia": griglia
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
                    ValorizzaFiltriDaParametri(parametri);
                } 
            } else console.log("Errore nella lettura impostazioni report vendite: " + risposta.Errore);
        }, null);
}

function ValorizzaFiltriDaParametri(parametri) {

    var dataRegDal = parametri._dataRegDal;
    var dataRegAl = parametri._dataRegAl;
    var dtDataRegDal = null;
    var dtDataRegAl = null;

    if (dataRegDal !== "") {
        dtDataRegDal = new Date(dataRegDal);
    }
    if (dataRegAl !== "") {
        dtDataRegAl = new Date(dataRegAl);
    }
    // Se la conversione a Date non va a buon fine ottengo un oggetto "invalid date" che è possibile intercettare attraverso la funzione isNaN
    if ((dtDataRegDal === null || isNaN(dtDataRegDal)) && dtDataRegAl !== null && !isNaN(dtDataRegAl)) {
        dtDataRegDal = kendo.date.addDays(dtDataRegAl, -31);
    }

    if (dtDataRegDal !== null && !isNaN(dtDataRegDal)) {
        set_data("Txt_DataRegDal", dtDataRegDal);
    }
    if (dtDataRegAl !== null && !isNaN(dtDataRegAl)) {
        set_data("Txt_DataRegAl", dtDataRegAl);
    }
        
    $('input[name$="txt_operazione"]').val(parametri._descrizione);

    if (parametri._centriAziendali !== "") {
        var arrSaCod = JSON.parse(parametri._centriAziendali)
        KendoMultisel("multiselCentroAziendale").value(arrSaCod);
    }
    
    if (parametri._specie !== "") {
        var arrVegCod = JSON.parse(parametri._specie);
        // Questo filtro, se passato deve essere fisso:
        KendoMultisel("multiselSpecie").value(arrVegCod);
        KendoMultisel("multiselSpecie").enable(false);
        SpecieChange();
        if (parametri._varieta !== "") {
            var arrCulCod = JSON.parse(parametri._varieta);
            KendoMultisel("multiselVarieta").value(arrCulCod);
        }
    }

    if (parametri._gruppiDocumenti !== undefined)
        KendoMultisel("multiselGruppoDocumento").value(parametri._gruppiDocumenti.split("|"));

    // TODO Prodotti e Contatti sono server_filtering, quindi occorre prima impostare il datasource ai valori passati e poi anche selezionarli.
    // questo però causa la necessità che i valori siano oggetti json le cui proprietà rispettino quelle indicate nel controllo kendo
    if (parametri._prodotti !== undefined) {
        var prodottiSelezionati = JSON.parse(parametri._prodotti);
        var prodotti = [];
        for (let i = 0; i < prodottiSelezionati.length; i += 1) {
            var prodotto = prodottiSelezionati[i];
            prodotti.push(prodotto.Prodotto_Cod);
        }
        KendoMultisel("multiselProdotti").dataSource.data(prodottiSelezionati);
        KendoMultisel("multiselProdotti").value(prodotti);
    }
    
    if (parametri._contatti !== "") {
        var contattiSelezionati = JSON.parse(parametri._contatti);
        var contatti = [];
        for (let i = 0; i < contattiSelezionati.length; i += 1) {
            var contatto = contattiSelezionati[i];
            contatti.push(contatto.cod_contatto);
        }
        KendoMultisel("multiselContatti").dataSource.data(contattiSelezionati);
        // Questo filtro, se passato deve essere fisso:
        KendoMultisel("multiselContatti").value(contatti);
        KendoMultisel("multiselContatti").enable(false);
    }

    if (parametri._categorie !== "") {
        var arrElemCod = JSON.parse(parametri._categorie);
        // Questo filtro, se passato deve essere fisso:
        KendoMultisel("multiselCategorie").value(arrElemCod);
        KendoMultisel("multiselCategorie").enable(false);
    }
    
    if (parametri._dataGiacenza !== "") {
        var dtDataGiacenza = new Date(parametri._dataGiacenza);
        if (!isNaN(dtDataGiacenza)) {
            set_data("Txt_DataGiacenza", parametri._dataGiacenza);
            // Questo filtro, se passato deve essere fisso:
            KendoDate("Txt_DataGiacenza").enable(false);
        }
    }

    if (parametri._inGiacenza !== "") {
        var inGiacenza = JSON.parse(parametri._inGiacenza);
        setKendoSwitch("cb_prod_giacenza", inGiacenza);
        KendoSwitch("cb_prod_giacenza").trigger("change");
    }

    if (parametri._fabbricato !== "") {
        Set_KendoDDLValue("id_ddlFabbricato", parametri._fabbricato);
        KendoSwitch("cb_prod_giacenza").enable(false);
        KendoDDL("id_ddlFabbricato").enable(false);
    }
        

    if (parametri._lotto !== "")
        $("#Txt_Lotto").data("kendoTextBox").value(parametri._lotto);

    //personalizzazioniGriglia = parametri._griglia;

}

function pulisci_filtri()
{
    var kendoConfirm = $("<div></div>").kendoConfirm({
        title: "Attenzione",
        messages: { okText: "Sì", cancel: "No" },
        content: "Effettuare la pulizia dei filtri ?"
    }).data("kendoConfirm");
    kendoConfirm.result.done(function () {
        var parametri = "{ }";
        ajaxAgronica(indirizzohttp + '/PulisciFiltri', parametri,
            function (risposta) {
                if (risposta.RispostaOK)
                {
                    var startYearDate = formattedDate(new Date(new Date().getFullYear(), 0, 1), "/");
                    set_data("Txt_DataRegDal", startYearDate, null);
                    set_data("Txt_DataRegAl", "", null);
                    KendoMultisel("multiselContatti").value("");
                    KendoMultisel("multiselSpecie").value("");
                    KendoMultisel("multiselVarieta").value("");

                    KendoMultisel("multiselCategorie").value("");
                    KendoMultisel("multiselProdotti").value("");

                    //personalizzazioniGriglie = null;
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

function RiempiFabbricati(options) {
    var piva = $(cIdPiva).val();
    var saCod = 0;
    var fabbricati = RicercaCelleEMagazzini_Sync(true, -999, piva, saCod, "", "");

    options.success(fabbricati);
}
