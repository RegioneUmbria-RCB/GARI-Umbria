/* File Created: luglio 14, 2020 */

function GestioneEstrazioni() {
    var estrazioneSel = parseInt(Get_KendoDDLValue("ddlEstrazioni"));

    if (Number.isNaN(estrazioneSel)) {
        return;
    }

    switch (estrazioneSel) {
        case enumEstrazioniConf.Report_CSV.Value:
            EseguiEstrazione({ hdPiva: $(cIdPiva).val(), ddlEstrazioni: estrazioneSel });
            break;

        case 0:
        case undefined:
        case -1:
            // Gestire l'errore
            break;

        default:
            //var arrFiltersNameValue = $("#containerNew").find("input, select").serializeArray();
            //var objFilters = {};
            //arrFiltersNameValue.forEach(function(filter) {
            //objFilters[filter.name] = filter.value;
            //});

            //console.log(arrFiltersNameValue);
            //console.log(objFilters);
            //console.log($("#containerNew").find("input, select").serialize());

            var filtroProdotti = "";

            var righe_selezionate=OttieniRigheSelezionateGridProdotto();

            if (righe_selezionate !== "")
                filtroProdotti = righe_selezionate;

            var arrConferenti = $.grep(KendoGrid("grigliaConferenti").dataSource.data(), function (d) { return d.Selected === true; });

            var confCodRisUm = 0;
            var conferentePiva = "";
            var conferenteRagSoc = "";
            var conferenteProg = "";

            var elenConferenti = "";
            if (arrConferenti.length === 1) {
                confCodRisUm = arrConferenti[0].Cod_RisUm;
                conferentePiva = arrConferenti[0].Partita_Iva;
                conferenteRagSoc = arrConferenti[0].Rag_Soc;
                conferenteProg = arrConferenti[0].Progressivo
            }
            else {
                if (arrConferenti.length > 1) {
                    var arrTemp = [];
                    for (var rigaConferenti of arrConferenti) {
                        //arrTemp.push(
                        //    rigaConferenti.Cod_RisUm + "_" +
                        //    rigaConferenti.Partita_Iva + "_" +
                        //    rigaConferenti.Rag_Soc + "_" +
                        //    rigaConferenti.Progressivo
                        //);
                        arrTemp.push(rigaConferenti.Cod_RisUm);
                    }
                    elenConferenti = arrTemp.join("|");
                }
            }

            var _docPrimoNumero = $("#tbDocPrimoNumero").data("kendoNumericTextBox").value();
            if (_docPrimoNumero === null || _docPrimoNumero === undefined) {
                _docPrimoNumero = 0.1;
            }
            var _docUltimoNumero = $("#tbDocUltimoNumero").data("kendoNumericTextBox").value();
            if (_docUltimoNumero === null || _docUltimoNumero === undefined) {
                _docUltimoNumero = 0.1;
            }
            var _numeroCopieStampe = $("#tbNumeroCopieStampe").data("kendoNumericTextBox").value();
            if (_numeroCopieStampe === null || _numeroCopieStampe === undefined) {
                _numeroCopieStampe = 1;
            }

            var objFilters = {
                hdPiva: $(cIdPiva).val(),
                hdIntConfigurazioneModuli: $(cIdIntConfigurazioneModuli).val(),
                ddlEstrazioni: estrazioneSel,
                dpDataInizio: $('input[name$="dpDataInizio"]').data("kendoDatePicker").value(),
                dpDataFine: $('input[name$="dpDataFine"]').data("kendoDatePicker").value(),
                ddlProdotti: Math.abs(Get_KendoDDLValue("ddlProdotti", 0)),
                ddlMagazzini: Get_KendoDDLValue("ddlMagazzini", ""),
                kSwitchDettaglioStabilimento: getKendoSwitch("kSwitchDettaglioStabilimento"),
                dpDataGiacenza: get_data("dpDataGiacenza"),
                ddlRapportiContabili: Get_KendoDDLValue("ddlRapportiContabili", 0),
                kSwitchTracciabilitaImpianti: getKendoSwitch("kSwitchTracciabilitaImpianti"),
                ddlCentriAziendali: Get_KendoDDLValue("ddlCentriAziendali", 0),
                conferenteCodRisUm: confCodRisUm,
                elencoConferenti: elenConferenti,
                ddlPrimiCessionari: Get_KendoDDLValue("ddlPrimiCessionari", 0),
                ddlSecondiCessionari: Get_KendoDDLValue("ddlSecondiCessionari", 0),
                ddlProduttori: Get_KendoDDLValue("ddlProduttori", 0),
                ddlSpecie: Get_KendoDDLValue("ddlSpecie", 0),
                ddlVarieta: Get_KendoDDLValue("ddlVarieta", 0),
                DocPrefisso: Get_KendoDDLValue("ddlDocPrefisso", ""),
                DocPrimoNumero: _docPrimoNumero,
                DocSuffisso: Get_KendoDDLValue("ddlDocSuffisso", ""),
                DocUltimoNumero: _docUltimoNumero,
                Stampante: Get_KendoDDLValue("ddlStampanti", ""),
                NumeroCopieStampe: _numeroCopieStampe,
                //-------- Dati aggiuntivi oltre ai value degli elementi html:

                conferentePiva: conferentePiva,
                conferenteRagSoc: conferenteRagSoc,
                conferente_Progressivo_SettoreDes: conferenteProg,

                primoCessionarioPiva: KendoDDL("ddlPrimiCessionari").dataSource.data().length && KendoDDL("ddlPrimiCessionari").dataItem() !== undefined &&
                    KendoDDL("ddlPrimiCessionari").dataItem().Partita_Iva ?
                    KendoDDL("ddlPrimiCessionari").dataItem().Partita_Iva : "",
                
                secondoCessionarioPiva: KendoDDL("ddlSecondiCessionari").dataSource.data().length && KendoDDL("ddlSecondiCessionari").dataItem() !== undefined &&
                    KendoDDL("ddlSecondiCessionari").dataItem().Partita_Iva ?
                    KendoDDL("ddlSecondiCessionari").dataItem().Partita_Iva : "",
                
                produttorePiva: KendoDDL("ddlProduttori").dataSource.data().length && KendoDDL("ddlProduttori").dataItem() !== undefined &&
                    KendoDDL("ddlProduttori").dataItem().Partita_Iva ?
                    KendoDDL("ddlProduttori").dataItem().Partita_Iva : "",


                prodottoCodArticolo: KendoDDL("ddlProdotti").dataSource.data().length && KendoDDL("ddlProdotti").dataItem() !== undefined &&
                    KendoDDL("ddlProdotti").dataItem().Cod_Articolo ?
                    KendoDDL("ddlProdotti").dataItem().Cod_Articolo : "",


                prodottoDescrizione: KendoDDL("ddlProdotti").dataSource.data().length && KendoDDL("ddlProdotti").dataItem() !== undefined &&
                    KendoDDL("ddlProdotti").dataItem().Prodotto_Des ?
                    KendoDDL("ddlProdotti").dataItem().Prodotto_Des : "",

                magazzinoDescrizione: KendoDDL("ddlMagazzini").dataSource.data().length && KendoDDL("ddlMagazzini").dataItem() !== undefined &&
                    KendoDDL("ddlMagazzini").dataItem().Ubic_Des ?
                    KendoDDL("ddlMagazzini").dataItem().Ubic_Des : "",

                centroAziendaleDescrizione: KendoDDL("ddlCentriAziendali").dataSource.data().length && KendoDDL("ddlCentriAziendali").dataItem() !== undefined &&
                    KendoDDL("ddlCentriAziendali").dataItem().sa_nome ?
                    KendoDDL("ddlCentriAziendali").dataItem().sa_nome : "",
                
                ElencoProdotti: filtroProdotti
                //-------  Fine dei dati aggiuntivi
            };

            if (estrazioneSel === enumEstrazioniConf.Stampa_Massiva_Bolle.Value ||
                estrazioneSel === enumEstrazioniConf.Stampa_Massiva_Pomodoro.Value) {
                // Mostro una dialog di conferma e di riepilogo
                //riepilogoFiltriStampaMassiva = objFilters;
                var oggDaStampare = estrazioneSel === enumEstrazioniConf.Stampa_Massiva_Pomodoro.Value ? "Certificati" : "Bolle";
                
                //var kWindowConfermaStampaMassiva = $("#confirmStampaMassiva").kendoWindow({
                //    title: "Conferma Stampa Massiva Bolle",
                //    modal: true,
                //    width: "40%",
                //    height: "40%",
                //}).data('kendoWindow').center().open();


                var kendoConfirm = $("<div></div>").kendoConfirm({
                    content: $("#tmplConfirmStampaMassiva").html().trim(),
                    title: "Conferma Stampa Massiva " + oggDaStampare,
                    messages: {
                        okText: "Stampa",
                        cancel: "Annulla"
                    },
                    width: "40%",
                    height: "60%",

                }).data("kendoConfirm");
                kendoConfirm.result.done(function () {
                    EseguiEstrazione(objFilters);
                });

                kendoConfirm.result.fail(function () {
                });

                // Valorizzo gli input all'interno della dialog di conferma

                var riepDocNumero = "";
                if (objFilters.DocPrimoNumero !== 0.1 && objFilters.DocUltimoNumero !== 0.1) {
                    //var strDocPrefisso = objFilters.DocPrefisso != "" ? "di prefisso " + objFilters.DocPrefisso + " " : "";
                    //var strDocSuffisso = objFilters.DocSuffisso != "" ? " e suffisso " + objFilters.DocSuffisso : ""; 
                    //riepDocNumero = "Bolle " + strDocPrefisso + "dalla " + objFilters.DocPrimoNumero + " alla " + objFilters.DocUltimoNumero + strDocSuffisso + "\r\n";
                    var strDocPrefisso = objFilters.DocPrefisso != "" ? objFilters.DocPrefisso : "(vuoto)";
                    var strDocSuffisso = objFilters.DocSuffisso != "" ? objFilters.DocSuffisso : "(vuoto)"; 
                    riepDocNumero = oggDaStampare + " di prefisso '" + strDocPrefisso + "' dalla " + objFilters.DocPrimoNumero + " alla " + objFilters.DocUltimoNumero + " e suffisso '" + strDocSuffisso + "'\r\n";
                }
                if (objFilters.DocPrimoNumero === 0.1 && objFilters.DocUltimoNumero === 0.1) {
                    //var strDocPrefisso = objFilters.DocPrefisso != "" ? "di prefisso " + objFilters.DocPrefisso + " " : "";
                    //var strDocSuffisso = objFilters.DocSuffisso != "" ? " e suffisso " + objFilters.DocSuffisso : ""; 
                    //riepDocNumero = "Bolle " + strDocPrefisso + strDocSuffisso + "\r\n";
                    var strDocPrefisso = objFilters.DocPrefisso != "" ? objFilters.DocPrefisso : "(vuoto)";
                    var strDocSuffisso = objFilters.DocSuffisso != "" ? objFilters.DocSuffisso : "(vuoto)";
                    riepDocNumero = oggDaStampare + " di prefisso '" + strDocPrefisso + "' e suffisso '" + strDocSuffisso + "'\r\n";
                }

                $("#inputConfermaStampante").val(objFilters.Stampante);
                $("#inputConfermaNumeroCopie").val(objFilters.NumeroCopieStampe);

                var riepDate = kendo.format(
                    "Dal: {0:d} al: {1:d}\r\n",
                    $('input[name$="dpDataInizio"]').data("kendoDatePicker").value(),
                    $('input[name$="dpDataFine"]').data("kendoDatePicker").value()
                );

                var riepCentroAz = "";
                if (objFilters.ddlCentriAziendali !== 0) {
                    riepCentroAz = kendo.format("Centro aziendale: {0}\r\n", objFilters.centroAziendaleDescrizione);
                }

                // Se ho l'impostazione della gerarchia disattivata, per l'utente il significato di conferente e produttore cambia
                var strConfFornitore = "conferent";
                if (!parseInt($(cIdSuperUserAccGerarchia).val())) {
                    strConfFornitore = "fornitor";
                }
                var riepConferenti = "";
                var riepPrimoCessionario = "";
                var riepSecondoCessionario = "";
                var riepProduttore = "";
                if (objFilters.conferenteCodRisUm !== 0) {
                    riepConferenti = kendo.format("Un {0}e selezionato\r\n", strConfFornitore);

                    if (objFilters.ddlPrimiCessionari !== 0) {
                        riepPrimoCessionario = "Un primo cessionario selezionato\r\n";
                    }
                    if (objFilters.ddlSecondiCessionari !== 0) {
                        riepSecondoCessionario = "Un secondo cessionario selezionato\r\n";
                    }
                    if (objFilters.ddlProduttori !== 0) {
                        if (!parseInt($(cIdSuperUserAccGerarchia).val())) {
                            riepProduttore = "Una provenienza selezionata\r\n";
                        }
                        else {
                            riepProduttore = "Un fornitore selezionato\r\n";
                        }
                    }
                }

                var nConferentiSelezionati = 0;
                if (objFilters.elencoConferenti !== "") {
                    nConferentiSelezionati = objFilters.elencoConferenti.split("|").length;
                }
                if (nConferentiSelezionati > 1) {
                    riepConferenti = kendo.format("{0} {1}i selezionati\r\n", nConferentiSelezionati, strConfFornitore);
                }

                var riepSpecieVeg = "";
                var riepVarietaVeg = "";
                var riepProdotti = ""
                var nProdottiSel = 0;
                if (objFilters.ElencoProdotti !== "") {
                    nProdottiSel = objFilters.ElencoProdotti.split("|").length;
                }
                if (nProdottiSel === 1) {
                    riepProdotti = "Un prodotto selezionato\r\n";
                }
                else {
                    if (nProdottiSel > 1) {
                        //riepProdotti = kendo.format("Filtro prodotti impostato: ({0})\r\n", objFilters.ElencoProdotti.replaceAll("|", ", "));
                        riepProdotti = kendo.format("{0} prodotti selezionati\r\n", nProdottiSel);
                    }
                    else {
                        if (objFilters.ddlSpecie !== 0) {
                            riepSpecieVeg = kendo.format("Specie vegetale: {0}\r\n", KendoDDL("ddlSpecie").dataItem().Veg_Des);
                        }
                        if (objFilters.ddlVarieta !== 0) {
                            riepVarietaVeg = kendo.format("Varietà: {0}\r\n", KendoDDL("ddlVarieta").dataItem().Cul_Des);
                        }
                    }
                }

                // NB: Commento ad ora il riepilogo completo in quanto di difficile gestione...
                //$("#tAreaConfermaFiltri").val(riepDocNumero + riepDate + riepCentroAz + riepConferenti + riepPrimoCessionario + riepSecondoCessionario + riepProduttore + riepSpecieVeg + riepVarietaVeg + riepProdotti);

                // ...Mantengo il riepilogo indicante i dati fondamentali, specifici della stampa
                $("#tAreaConfermaFiltri").val(riepDocNumero + riepDate);

                kendoConfirm.open();
            }
            else {
                EseguiEstrazione(objFilters);
            }

            break;
    }
}

function GenericOption(value, desc) {
    this.Value = value;
    this.Desc = desc;
}

/**
 * Aggiunge un elemento vuoto all'inizio dell'array di oggetti passato valorizzando le due proprietà specificate e impostando le altre a null.
 * Utile per aggiungere una riga all'inizio di una drop down list e renderla opzionale
 * @param {any} dataSource
 * @param {string} textProperty Nome della proprietà il cui testo è visibile all'utente
 * @param {string} valueProperty Nome della proprietà il cui valore deve essere passato lato server
 * @param {string} text Testo per l'utente
 * @param {any} value Valore per il server
 */
function AggiungiRigaVuota(dataSource, textProperty, valueProperty, text, value) {
    if (dataSource.length > 0) {
        var itemDataSource = Object.create(dataSource[0]); // La funzione crea sostanzialmente un clone dell'oggetto passato
        for (var prop in itemDataSource) { //restituisce il nome delle proprietà dell'oggetto

            // Sfrutto la bracket notation per valorizzare le proprietà dell'oggetto:
            // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Property_Accessors
            if (prop == textProperty) {
                itemDataSource[textProperty] = text;
            }
            else {
                if (prop == valueProperty) {
                    itemDataSource[valueProperty] = value;
                }
                else {
                    itemDataSource[prop] = null;
                }
            }

        }
        
        dataSource.unshift(itemDataSource);
    }
}

function ddlFiltering(e) {
    var filter = e.filter;

    if (filter === undefined || filter === null || !filter.value || filter.value.length < e.sender.minLength) {
        e.preventDefault();
    }
}

function ddlArrayFilterOpen(e) {

    var filters = this.dataSource.filter();

    if (filters) {
        //clear applied filters
        this.dataSource.filter({});
    }

}


function resizeFixed(ev) {
    var header = ev.data.header;
    var wrapper = ev.data.wrapper;

    var wrapperWidth = wrapper.width();
    if (wrapperWidth !== 0) {
        var paddingRight = parseInt(header.css("padding-right"));
        header.css("width", wrapperWidth - paddingRight);
    }
    else {
        // Nel caso l'evento venga eseguito mentre la griglia è nascosta, imposto una variabile per ricalcolare correttamente la width
        // alla prima occorrenza dell'evento "scroll" in quanto più frequente
        header.css("width", "auto");
        header.data("fix_width", 1);
    }
}

function scrollFixed(ev) {
    var header = ev.data.header;
    var wrapper = ev.data.wrapper;

    // Nel caso l'evento venga eseguito mentre la griglia è nascosta, rimuovo la classe
    var wrapperHeight = wrapper.height();

    if (header.data("fix_width") === 1 && wrapperHeight !== 0) {
        var paddingRight = parseInt(header.css("padding-right"));
        header.css("width", wrapper.width() - paddingRight);
        header.data("fix_width", 0);
    }

    var offset = $(this).scrollTop(),
        tableOffsetTop = wrapper.offset().top,
        tableOffsetBottom = tableOffsetTop + wrapperHeight - header.height();
    if (offset < tableOffsetTop || offset > tableOffsetBottom || wrapperHeight === 0) {
        header.removeClass("fixed-header");
    } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !header.hasClass("fixed")) {
        header.addClass("fixed-header");
    }
}

function ddlEstrazioniRead(options) {
    var extractionsList = [
        enumEstrazioniConf.ConfXArticolo,
        enumEstrazioniConf.EC_Bolle,
        enumEstrazioniConf.EC_Imballi,
        enumEstrazioniConf.Saldo_Imballi,
        enumEstrazioniConf.Esportazione_BolleFF_XLS,
        enumEstrazioniConf.Esportazione_Trasportatori_XLS,
        enumEstrazioniConf.Stampa_Massiva_Bolle,
        enumEstrazioniConf.Stampa_Massiva_Pomodoro,
        enumEstrazioniConf.Esportazione_Pomodoro_XLS,
        enumEstrazioniConf.Riepilogo_Conferimenti,
        enumEstrazioniConf.Comunicazione_Credito
        //,enumEstrazioniConf.Tracciabilita // Report dismesso
    ];
    if (parseInt($(cIdAbilitaExportConf).val()) === 1) {
        extractionsList.push(enumEstrazioniConf.Report_CSV);
    }
    options.success(extractionsList);
}

function ddlEstrazioniChange(e) {
    abilitaDate(false);
    abilitaProdotto(false);
    abilitaMagazzino(false);
    abilitaDettaglioStabilimento(false);
    abilitaDataGiacenza(false);
    abilitaRapportoContabile(false);
    abilitaTracciabilitaImpianti(false);
    abilitaCentroAziendale(false);
    abilitaConferente(false);
    abilitaCessionariProduttore(false);
    //abilitaPrimoCessionario(false);
    //abilitaSecondoCessionario(false);
    //abilitaProduttore(false);
    abilitaSpecie(false);
    abilitaVarieta(false);
    abilitaStampanti(false);
    abilitaRangeBolle(false);

    $("#btn_filtro_prodotti").hide();
    $("#divgridProdotto").hide();

    $("#divddlProdotto").hide();

    DeselezionaRigheGridProdotto();

    Set_KendoDDLValue("ddlProduttori", 0);
    Set_KendoDDLValue("ddlPrimiCessionari", 0);
    Set_KendoDDLValue("ddlSecondiCessionari", 0);

    switch (parseInt(e.currentTarget.value)) {

        case enumEstrazioniConf.ConfXArticolo.Value:

            abilitaDate(true);
            abilitaCentroAziendale(true);

            abilitaConferente(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);
            //abilitaRapportoContabile(false);
            //abilitaConferente(false);

            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;

        case enumEstrazioniConf.EC_Bolle.Value:

            abilitaDate(true);
            abilitaCentroAziendale(true);

            abilitaConferente(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);
            //abilitaRapportoContabile(false);
            //abilitaConferente(false);

            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;

        case enumEstrazioniConf.EC_Imballi.Value:
            /* Sì:
             * data inizio
             * data fine
             * ddlCentriAziendali
             * ddlProdotti
             * ddlConferenti
             * grigliaConferenti
             * kSwitchDettaglioStabilimento
             *
             * No:
             * ddlMagazzini
             * data giacenza
             * ddlRapportiContabili
             */
            abilitaDate(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").hide();

            $("#divddlProdotto").show();

            abilitaConferente(true);
            //abilitaCessionariProduttore(true);

            //abilitaMagazzino(true);
            abilitaCentroAziendale(true);
            abilitaDettaglioStabilimento(true);

            //abilitaDataGiacenza(false);
            //abilitaRapportoContabile(false);

            elemCod = 205;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;

        case enumEstrazioniConf.Saldo_Imballi.Value:
            /* Sì:
             * data inizio
             * data fine
             * ddlCentriAziendali
             * ddlProdotti
             * ddlConferenti
             * grigliaConferenti
             * data giacenza
             * kSwitchDettaglioStabilimento
             *
             * No:
             * ddlMagazzini
             * ddlRapportiContabili
             */
            abilitaDate(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").hide();

            $("#divddlProdotto").show();

            abilitaConferente(true);
            //abilitaCessionariProduttore(true);

            abilitaDataGiacenza(true);
            //abilitaMagazzino(true);
            abilitaCentroAziendale(true);
            abilitaDettaglioStabilimento(true);


            //abilitaRapportoContabile(false);


            elemCod = 205;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;

        case enumEstrazioniConf.Esportazione_BolleFF_XLS.Value:
            abilitaDate(true);
            abilitaTracciabilitaImpianti(true);
            abilitaCentroAziendale(true);

            abilitaConferente(true);
            abilitaCessionariProduttore(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);
            //abilitaRapportoContabile(false);
            //abilitaConferente(false);


            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;

        case enumEstrazioniConf.Esportazione_Trasportatori_XLS.Value:
            abilitaDate(true);
            abilitaCentroAziendale(true);

            abilitaConferente(true);
            //abilitaCessionariProduttore(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);
            //abilitaRapportoContabile(false);
            //abilitaConferente(false);

            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;

        case enumEstrazioniConf.Stampa_Massiva_Bolle.Value:

            abilitaDate(true);

            abilitaStampanti(true);
            abilitaRangeBolle(true);

            abilitaCentroAziendale(true);

            abilitaConferente(true);
            abilitaCessionariProduttore(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);
            //abilitaRapportoContabile(false);
            //abilitaConferente(false);

            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            //per caricare menù a tendina delle stampanti:
            //	chiave Lista_Stampanti del configurazione_siti (la | separa le stampanti)

            break;
        
        case enumEstrazioniConf.Stampa_Massiva_Pomodoro.Value:

            abilitaDate(true);

            abilitaStampanti(true);
            abilitaRangeBolle(true);

            abilitaCentroAziendale(true);

            abilitaConferente(true);
            abilitaCessionariProduttore(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);
            //abilitaRapportoContabile(false);
            //abilitaConferente(false);

            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;

        case enumEstrazioniConf.Esportazione_Pomodoro_XLS.Value:
            abilitaDate(true);
            abilitaCentroAziendale(true);

            abilitaConferente(true);
            abilitaCessionariProduttore(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);
            //abilitaRapportoContabile(false);
            //abilitaConferente(false);


            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;


        case enumEstrazioniConf.Riepilogo_Conferimenti.Value:
            /* Sì:
             * data inizio
             * data fine
             * ddlProdotti
             * grigliaConferenti
             * ddlRapportiContabili
             *
             * No:
             * data giacenza
             * ddlMagazzini
             * kSwitchDettaglioStabilimento
             */
            abilitaDate(true);

            abilitaConferente(true);
            //abilitaCessionariProduttore(true);

            abilitaRapportoContabile(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);


            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;

        // Report dismesso
        //case enumEstrazioniConf.Tracciabilita.Value:
            /*(ad ora uguale a 187)
             * Sì:
             * data inizio
             * data fine
             * ddlProdotti
             * grigliaConferenti
             * ddlRapportiContabili
             *
             * No:
             * data giacenza
             * ddlCentriAziendali
             * ddlMagazzini
             * kSwitchDettaglioStabilimento
             */
            //abilitaDate(true);

            //abilitaConferente(true);
            //abilitaCessionariProduttore(true);

            //abilitaRapportoContabile(true);

            //abilitaSpecie(true);
            //abilitaVarieta(true);
            //abilitaProdotto(true);

            //$("#btn_filtro_prodotti").show();

            //$("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);


            //elemCod = 210;
            //KendoDDL("ddlProdotti").dataSource.data([]);

            //break;


        case enumEstrazioniConf.Report_CSV.Value:
            // Mantengo tutti i filtri nascosti
            break;


        case enumEstrazioniConf.Comunicazione_Credito.Value:

            abilitaDate(true);
            abilitaCentroAziendale(true);

            abilitaConferente(true);

            abilitaSpecie(true);
            abilitaVarieta(true);
            abilitaProdotto(true);

            $("#btn_filtro_prodotti").show();

            $("#divddlProdotto").hide();

            //abilitaDataGiacenza(false);
            //abilitaMagazzino(false);
            //abilitaDettaglioStabilimento(false);
            //abilitaRapportoContabile(false);
            //abilitaConferente(false);

            elemCod = 210;
            KendoDDL("ddlProdotti").dataSource.data([]);

            break;


        default:
            console.log(e); // Gestire l'errore
            break;
    }
}

function ddlMagazziniRead(options) {
    var warehouseList = RicercaMagazzini(
        true, 20,
        $(cIdPiva).val(), 0, false
    );
    options.success(warehouseList);
}

function ddlMagazziniChange(e) {

}


function ddlProdottiRead(options) {
    // TODO Elem_Cod è ora calcolato con una variabile globale valorizzata al change dell'estrazione, studiare un metodo migliore, tipo una categorizzazione delle estrazioni
    // 184, 185 → AgronicaCoreUtility.CaricaListControl.BeniConfezionamentoVegetale (BENI_CONFEZ_VEGETALE => 205)
    // 186, 187 → AgronicaCoreUtility.CaricaListControl.ProdottiConferiti (TRASFORMATI_VEGETALI => 210, TRASFORMATI_ANIMALI => 310)
    // Stessi filtri per i due casi, eccetto per un filtro_aggiuntivo 1=1 nel secondo. È corretto?
    // Non utilizzare in un primo momento la gestione legacy sopra elencata, fare riferimento al codice dell'Agenda "FormProdottoUC"

    let xFiltroAggiuntivoMateriePrime = "";
    if (elemCod === 210 || elemCod === 310)
        xFiltroAggiuntivoMateriePrime = $(hf_filtroMateriePrimeConferimento).val();

    var productsList = RicercaElencoCompletoProdotti(
        objP_super_server, // da master
        objP_server, // da master
        objP_utenti, // da master
        $(cIdPiva).val(), // prelevo la piva grazie al campo input hidden valorizzato da vb
        0, //xSa_Cod, serve per un filtro 
        0, //xFabbricato_Cod Utilizzato per SchedaGiacenzeMagazzino
        0, //xTipoDestinazione Non usato dalla funzione (è da correggere?)
        elemCod, //ddlCategorieMagazzinoValue corrisponde ad Elem_Cod → devo passarlo a 210 se sto richiedendo un report da conferimenti, mentre a 205 per i report da imballi
        false, //bloccaPerSottoGiacenza corrisponde a soloInGiacenza è un flag. Viene utilizzato per la funzione ImpostaCausaleEFlagGiacenza e corrisponde parametro Flag_Negativo
        JSON.stringify(options.data.filter.filters), // L'effettivo filtro di ricerca scritto dall'utente, lato server viene controllato sulle colonne Mat_Des e Cod_Articolo
        "", //Qs_Mode utilizzato per elem_cod qua non usati
        "7300", //Qs_CaricoScarico corrisponde a cau_mov, lo devo passare a CAU_CARICO => 7300
        new Date(), //get_data("inDataEmissione"),
        0, //xPuaRegolamento,
        "", //w_LottoAccettazione,
        false, //leggiUMformulati
        false, //Flag_QtaNoZero,
        0, //xTipoPuaRegolamento
        null, null, xFiltroAggiuntivoMateriePrime, false, -1);

    let elencoProdottiJSon = JSON.parse(productsList);

    AggiungiRigaVuota(elencoProdottiJSon, "Prodotto_Des", "Prodotto_Cod", "", 0);

    options.success(elencoProdottiJSon);
}

function ddlProdottiChange(e) {

}


function ddlRapportiContabiliRead(options) {
    var contactsList = [
        new GenericOption(0, "Tutti"),
        new GenericOption(-18, "Conferente"),
        new GenericOption(-24, "Fornitore Ortofrutta"),
    ];
    options.success(contactsList);
}

function ddlRapportiContabiliChange(e) {

}

function ddlCentriAziendaliRead(options) {
    var businessList = RicercaCentriAziendali($(cIdPiva).val(), false, 2, false, false);

    var businessCount = businessList.length;
    if (businessCount === 0) {
        AggiungiRigaVuota(businessList, "sa_nome", "sa_cod", "Nessun centro aziendale presente", 0)
    }
    else {
        if (businessCount > 1) {
            AggiungiRigaVuota(businessList, "sa_nome", "sa_cod", "", 0)
        }
    }

    options.success(businessList);
}

function ddlCentriAziendaliChange(e) {

}

function popolaGrigliaConferenti(idControllo) {
    // Creazione Griglia
    var funzioniCRUD = {
        funzioneRead: gridConferentiRead,
        UtenteAbilitatoInserimentoModifica: false,
        UtenteAbilitatoCancellazione: true,
        checkBoxFunction: gridConferentiCheckRow,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true
    };
    var idModel = "Cod_RisUm";
    var campiKendoModel = {
        Agente_Cod: { type: "number" },
        Attivita_Des: { type: "string" },
        CapoArea_Cod: { type: "number" },
        Cod_Contatto: { type: "string" },
        Cod_RisUm: { type: "number" },
        Cod_Risum_Destinazione_Diversa: { type: "number" },
        Codice_Fiscale: { type: "string" },
        Id_CF: { type: "number" },
        IsImpresaGias: { type: "boolean" },
        Partita_Iva: { type: "string" },
        Piva_Proprietaria: { type: "string" },
        Progressivo: { type: "string" },
        Provvigione: { type: "number" },
        Provvigione_CapoArea: { type: "number" },
        Rag_Soc: { type: "string" },
        Rapporto_Des: { type: "string" },
        Rag_Soc_Completa: { type: "string" },
        Rag_Soc_Progressivo: { type: "string" },
        Sa_Cod: { type: "number" },
        Tipo_Indirizzo_Default: { type: "number" },
        Tipo_Indirizzo_Default_Destinazione_Diversa: { type: "number" },
        Validita_Fine: { type: "date" },
        Validita_Inizio: { type: "date" },
        Vettore_Cod: { type: "number" },
    };
    var colonneKendoGrid = [
        { field: "Rag_Soc", title: "Conferente", filterable: { multi: true, search: true } },
        { field: "Rapporto_Des", title: "Rapporto Contabile", filterable: { multi: true, search: true } },
        { field: "Progressivo", title: "Codice", },
        { field: "Cod_Contatto", title: "P.Iva / Codice Fiscale", filterable: { multi: true, search: true } },
        { field: "Attivita_Des", title: "Attività", filterable: { multi: true, search: true } },
        {
            field: "Validita_Inizio", title: "Inizio Validità", /*format: "{0:dd/MM/yyyy}",*/
            template: '#= kendo.toString(Validita_Inizio, "dd/MM/yyyy") == "01/01/1900" ? "..." : kendo.toString(Validita_Inizio, "dd/MM/yyyy") #'
        },
        {
            field: "Validita_Fine", title: "Fine Validità",
            template: '#= kendo.toString(Validita_Fine, "dd/MM/yyyy") == "31/12/2100" ? "..." : kendo.toString(Validita_Fine, "dd/MM/yyyy") #'
        },
        //{ field: "Agente_Cod", title: "Agente_Cod", hidden: true },
        //{ field: "CapoArea_Cod", title: "CapoArea_Cod", hidden: true },
        //{ field: "Cod_RisUm", title: "Cod_RisUm", hidden: true },
        //{ field: "Cod_Risum_Destinazione_Diversa", title: "Cod_Risum_Destinazione_Diversa", hidden: true },
        //{ field: "Codice_Fiscale", title: "Codice_Fiscale", hidden: true },
        //{ field: "Id_CF", title: "Id_CF", hidden: true },
        //{ field: "IsImpresaGias", title: "IsImpresaGias", hidden: true },
        //{ field: "Partita_Iva", title: "Partita_Iva", hidden: true },
        //{ field: "Piva_Proprietaria", title: "Piva_Proprietaria", hidden: true },
        //{ field: "Provvigione", title: "Provvigione", hidden: true },
        //{ field: "Provvigione_CapoArea", title: "Provvigione_CapoArea", hidden: true },
        //{ field: "Rag_Soc_Completa", title: "Rag_Soc_Completa", hidden: true },
        //{ field: "Rag_Soc_Progressivo", title: "Rag_Soc_Progressivo", hidden: true },
        //{ field: "Sa_Cod", title: "Sa_Cod", hidden: true },
        //{ field: "Tipo_Indirizzo_Default", title: "Tipo_Indirizzo_Default", hidden: true },
        //{ field: "Tipo_Indirizzo_Default_Destinazione_Diversa", title: "Tipo_Indirizzo_Default_Destinazione_Diversa", hidden: true },
    ];

    var parametriPerLettura = [];
    var parametriDataSource = {
        serverFiltering: false,
        pagesize: 10
    };

    var parametriKendoGrid = {
        excel: true,
        pdf: false,
        editable: false,
        groupable: false,
        //toolbarCommands: ["tmpl", ],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
        reorderable: true
    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: gridConferentiDataBound, /*funzioneDaChiamareDopoSelectAllRows: gridAssociazioniDopoCheckAllRows*/ };

    creaKendoGrid(
        idControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function gridConferentiRead(options) {
    // Di seguito spiego la ricerca legacy fatta sui soggetti:
    //   L'elenco varia in base a int_Configurazione_Moduli, che viene valorizzata da db al caricamento della pagina e vale F&F (codice 2) se c'è almeno un
    //   "Modulo_Generazione" su db con quel codice, altrimenti vale "Nessuna Generazione" (codice 0); inoltre varia in base all'estrazione selezionata:
    //   int_Configurazione_Moduli
    //     → 2 
    //         →  cod estrazione {184, 185} → si estraggono soggetti di tipologie {COD_CLIENTE, COD_FORNITORE, COD_CONFERENTE, COD_FORNITORE_ORTOFRUTTA} → per fare questo devo modificare il webservice oppure direttamente 'LeggiRapportoSpecificoxDocumenti' aggiungendo un nuovo tipo rapporto 
    //         →  altri cod estrazioni      → si estraggono soggetti di tipologie {COD_CONFERENTE, COD_FORNITORE_ORTOFRUTTA} -> posso usare la stessa ricerca usata per i produttori
    //     → 0 →  si estraggono solo conferenti di tipologia: COD_CLIENTE → dovrei fare una query restrittiva sempre su LeggiRapportoSpecificoxDocumenti per lo specifico rapporto contabile
    // 
    // Per la nuova ricerca prendiamo le seguenti decisioni:
    //   Ci allineiamo alla ricerca fatta in fase di inserimento dei documenti che andiamo ad estrarre, questa in base all'impostazione su db 
    //   "cIdSuperUserAccGerarchia" decide se mostrare quattro o due campi dei soggetti e se effettuare una gestione gerarchica degli stessi;
    //   i soggetti in questione sono "conferente", "primo cessionario", "secondo cessionario", "produttore" nel primo caso e
    //   "fornitore" e "provenienza" nel secondo (che corrispondono a "conferente" e "produttore")
    //   La gerarchia dei campi viene gestita nel primo caso, nell'ordine di importanza decrescente in cui ho indicato gli stessi; 
    //   ai fini delle estrazioni questa però risulta una limitazione per l'utente, pertanto la prendiamo in considerazione 
    //   solo al primo livello dei conferenti(/fornitori). Estraiamo quindi in questa pagina tutti i soggetti che presentano 
    //   contemporaneamente i ruoli di "cliente" e "fornitore" (li chiamerò "conferenti" d'ora in poi), in quanto sono la tipologia di soggetti
    //   legata anche in data entry ai lav_cod dei documenti. Per la griglia dei conferenti avremo soggetti che sono imprese gias, e decidiamo
    //   di impostare questo filtro a posteriori anche per le ddl dei cessionari e del produttore, con la differenza che questi non sono obbligatoriamente
    //   diretti "figli" dell'impresa corrente.
    //   Ad ora teniamo commentata la distinzione fra gli utenti con il modulo F&F (di codice 2) e gli altri, se necessario implementarla, aggiungere il
    //   codice all'else della condizione: parseInt($(cIdIntConfigurazioneModuli)) === 2

    var companiesList = [];
    //if (parseInt($(cIdIntConfigurazioneModuli)) === 2) {
    companiesList = RicercaConferentiPrimoLivello(
        objP_server,
        $(cIdPiva).val(),
        parseInt($(cIdSuperUserAccGerarchia).val()),
        ""
    );
    //}
    //else {
        // Ricerca soggetti senza modulo Fresh & Food
    //}
    options.success(companiesList);
}

function gridConferentiDataBound(e) {
    var gridId = e.sender.element[0].id;
    kendo_AggiustaDimensioneColonne("#" + gridId);

    var _wrapper = e.sender.wrapper,
        _header = _wrapper.find(".k-grid-header");

    var objEv = {
        data: {
            wrapper: _wrapper,
            header: _header
        }
    };

    resizeFixed(objEv);
    $(window).on("resize", { wrapper: _wrapper, header: _header }, resizeFixed);
    $(window).on("scroll", { wrapper: _wrapper, header: _header }, scrollFixed);
}

function gridConferentiCheckRow(e) {
    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = KendoGrid("grigliaConferenti"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)

    // Mostro i filtri dei cessionari e del produttore se ho un solo elemento selezionato ed i primi solo se cIdSuperUserAccGerarchia è valorizzato,
    // a monte ho un controllo di visibilità in base alla tipologia di estrazione selezionata
    var confSel = $.grep(KendoGrid("grigliaConferenti").dataSource.data(), function (d) { return d.Selected === true; })
    if (confSel.length === 1) {
        abilitaProduttore(true);

        if (parseInt($(cIdSuperUserAccGerarchia).val())) {
            abilitaPrimoCessionario(true);
            abilitaSecondoCessionario(true);
        }
    }
    else {
        abilitaProduttore(false);
        Set_KendoDDLValue("ddlProduttori", 0);

        if (parseInt($(cIdSuperUserAccGerarchia).val())) {
            abilitaPrimoCessionario(false);
            abilitaSecondoCessionario(false);

            Set_KendoDDLValue("ddlPrimiCessionari", 0);
            Set_KendoDDLValue("ddlSecondiCessionari", 0);
        }
    }
}

function getDsClientiFornitori() {
    // Dovendo in questa istanza cercare tutti quei soggetti il cui rapporto contabile è uno fra {COD_CONFERENTE, COD_FORNITORE_ORTOFRUTTA, COD_CLIENTEFORNITORE}
    // noto che questi corrispondono a rapporti che hanno contemporaneamente i flag su tabella "Cliente" e "Fornitore" valorizzati, quindi per semplicità riutilizzo
    // la "RicercaConferentiPrimoLivello" non impostando la gestione della gerarchia, in questo modo estraggo tutti quei soggetti che hanno un qualsiasi rapporto contabile
    // che rispetti i suddetti flag
    if (dataSourceClientiFornitori === null) {
        var arrConferenti = RicercaConferentiPrimoLivello(objP_server, $(cIdPiva).val(), 0);
        dataSourceClientiFornitori = arrConferenti.filter(function (dataItem) { return dataItem.IsImpresaGias === true; });
        AggiungiRigaVuota(dataSourceClientiFornitori, "Rag_Soc_Progressivo", "Cod_RisUm", "", 0);
    }
    return dataSourceClientiFornitori;
}

function ddlPrimiCessionariRead(options) {
    options.success(getDsClientiFornitori());
}

function ddlPrimiCessionariChange(e) {

}

function ddlSecondiCessionariRead(options) {
    options.success(getDsClientiFornitori());
}

function ddlSecondiCessionariChange(e) {

}

function ddlProduttoriRead(options) {
    if (parseInt($(cIdSuperUserAccGerarchia).val())) {
        // Se l'impostazione di gerarchia è attivata, allora questa ddl ha lo stesso datasource di quelle dei cessionari,
        options.success(getDsClientiFornitori());
    }
    else {
        // altrimenti visualizza ogni conferente
        var userFilter = options.data.filter !== undefined ? JSON.stringify(options.data.filter.filters) : ""; // User filter è in previsione di impostare la ricerca come server filtering
        dataSourceClientiFornitori = RicercaConferentiPrimoLivello(objP_server, $(cIdPiva).val(), 0, userFilter);
        AggiungiRigaVuota(dataSourceClientiFornitori, "Rag_Soc_Progressivo", "Cod_RisUm", "", 0);
        options.success(dataSourceClientiFornitori);
    }
}

function ddlProduttoriChange(e) {

}

function ddlSpecieRead(options) {
    var speciesList = RicercaSpecie($(cIdPiva).val(), false);
    options.success(speciesList);
}

function ddlSpecieChange(e) {
    KendoDDL("ddlVarieta").dataSource.read();

    if ($('#divgridProdotto').is(':visible')) {
        DeselezionaRigheGridProdotto();
        $("#divgridProdotto").hide();
    }
}

function ddlVarietaRead(options) {
    var vegCod = Get_KendoDDLValue("ddlSpecie");
    var varietyList = RicercaVarieta($(cIdPiva).val(), vegCod);
    options.success(varietyList);
}

function ddlVarietaChange(e) {

    if ($('#divgridProdotto').is(':visible')) {
        DeselezionaRigheGridProdotto();
        $("#divgridProdotto").hide();
    }

}

function ddlDocPrefissoRead(options) {
    var prefixList = LeggiDocPrefissiSuffissi($(cIdPiva).val(), -1);;
    //AggiungiRigaVuota(dataSourceDocPrefissi, "Doc_Numero_Sin", "Doc_Numero_Sin", "", "-999");
    options.success(prefixList);
}

function ddlDocSuffissoRead(options) {
    var suffixList = LeggiDocPrefissiSuffissi($(cIdPiva).val(), 1);
    //AggiungiRigaVuota(dataSourceDocSuffissi, "Doc_Numero_Des", "Doc_Numero_Des", "", "-999");
    options.success(suffixList);
}

function ddlStampantiRead(options) {
    var printersList = LeggiElencoStampanti($(cIdPiva).val());
    options.success(printersList);
}

function abilitaDate(abilitato) {
    $(".containerDataInizio").toggle(abilitato);
    $(".containerDataFine").toggle(abilitato);

    $(".containerDataInizio input").data("kendoDatePicker").enable(abilitato);
    $(".containerDataFine input").data("kendoDatePicker").enable(abilitato);
}

function abilitaProdotto(abilitato) {
    $(".containerProdotto").toggle(abilitato);
    $(".containerProdotto input").data("kendoDropDownList").enable(abilitato);
}

function abilitaDataGiacenza(abilitato) {
    $(".containerDataGiacenza").toggle(abilitato);
    $(".containerDataGiacenza input").data("kendoDatePicker").enable(abilitato);
}

function abilitaMagazzino(abilitato) {
    $(".containerMagazzino").toggle(abilitato);
    $(".containerMagazzino input").data("kendoDropDownList").enable(abilitato);
}

function abilitaRapportoContabile(abilitato) {
    $(".containerRapportoContabile").toggle(abilitato);
    $(".containerRapportoContabile input").data("kendoDropDownList").enable(abilitato);
}

function abilitaDettaglioStabilimento(abilitato) {
    $(".containerDettaglioStabilimento").toggle(abilitato);
    $(".containerDettaglioStabilimento input").data("kendoSwitch").enable(abilitato);
}

function abilitaTracciabilitaImpianti(abilitato) {
    $(".containerTracciabilitaImpianti").toggle(abilitato);
    $(".containerTracciabilitaImpianti input").data("kendoSwitch").enable(abilitato);
}

function abilitaCentroAziendale(abilitato) {
    $(".containerCentroAziendale").toggle(abilitato);
    $(".containerCentroAziendale input").data("kendoDropDownList").enable(abilitato);
}

function abilitaConferente(abilitato) {
    $(".containerConferente").toggle(abilitato);
}

function abilitaCessionariProduttore(abilitato) {
    $(".boxCessionariProduttore").toggle(abilitato);
}

function abilitaPrimoCessionario(abilitato) {
    $(".containerPrimoCessionario").toggle(abilitato);
    $(".containerPrimoCessionario input").data("kendoDropDownList").enable(abilitato);
}

function abilitaSecondoCessionario(abilitato) {
    $(".containerSecondoCessionario").toggle(abilitato);
    $(".containerSecondoCessionario input").data("kendoDropDownList").enable(abilitato);
}

function abilitaProduttore(abilitato) {
    $(".containerProduttore").toggle(abilitato);
    $(".containerProduttore input").data("kendoDropDownList").enable(abilitato);
}

function abilitaSpecie(abilitato) {
    $(".containerSpecie").toggle(abilitato);
    $(".containerSpecie input").data("kendoDropDownList").enable(abilitato);
}

function abilitaVarieta(abilitato) {
    $(".containerVarieta").toggle(abilitato);
    $(".containerVarieta input").data("kendoDropDownList").enable(abilitato);
}

function abilitaStampanti(abilitato) {
    $(".rowContainerStampanti").toggle(abilitato);
    
    //$(".containerStampanti").toggle(abilitato);
    $(".containerStampanti input").data("kendoDropDownList").enable(abilitato);

    //$(".containerNumeroCopieStampe").toggle(abilitato);
    $("#tbNumeroCopieStampe").data("kendoNumericTextBox").enable(abilitato);
}

function abilitaRangeBolle(abilitato) {
    $(".containerDocNumeri").toggle(abilitato);

    KendoDDL("ddlDocPrefisso").enable(abilitato);
    $("#tbDocPrimoNumero").data("kendoNumericTextBox").enable(abilitato);
    KendoDDL("ddlDocSuffisso").enable(abilitato);

    $("#tbDocUltimoNumero").data("kendoNumericTextBox").enable(abilitato);
}

function creakendoGridProdotti(divKendo) {

    var funzioniCRUD = {
        funzioneRead: kReadValorizzazioneProdotto_rows,
        checkBoxFunction: kSelezionatoProdotto
    };

    var idModel = "Prodotto_Cod";

    var campiKendoModel =  kReadValorizzazioneProdotto_mod();
    var colonneKendoGrid = kReadValorizzazioneProdotto_col();

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 10 };
    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        excel: true,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: true,
        btnEliminaTuttiFiltri: true,
        colonneCustomKendoGrid: []

    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: gridProdottiDataBound};

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(divKendo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}


function kSelezionatoProdotto(e) {
    var checked = this.checked;
    row = $(this).parents("tr");
    grid = $("#gridProdotto").data("kendoGrid");
    dataItem = grid.dataItem(row);


    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}


function kReadValorizzazioneProdotto_rows(options) {
    var data = $('#hdgridProdotto').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function gridProdottiDataBound(e) {
    var gridId = e.sender.element[0].id;
    kendo_AggiustaDimensioneColonne("#" + gridId);

    var _wrapper = e.sender.wrapper,
        _header = _wrapper.find(".k-grid-header");

    var objEv = {
        data: {
            wrapper: _wrapper,
            header: _header
        }
    };

    resizeFixed(objEv);
    $(window).on("resize", { wrapper: _wrapper, header: _header }, resizeFixed);
    $(window).on("scroll", { wrapper: _wrapper, header: _header }, scrollFixed);
}

function kReadValorizzazioneProdotto_col() {

    var colonneArray = [
        {
            "field": "NomeComune",
            "title": "Categoria Magazzino",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Cod_Articolo",
            "title": "Codice",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Prodotto_Des",
            "title": "Descrizione",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Veg_Des",
            "title": "Specie",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Cul_Des",
            "title": "Varietà",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "GRVA_COD_VEG_DES",
            "title": "Tipologia Varietale",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Reg_Des",
            "title": "Regolamento",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Codice_Esterno",
            "title": "Codice Esterno"
        },
        {
            "field": "Cat_Des",
            "title": "Categoria Commerciale",
            "filterable": {
                "multi": true,
                "search": true
            }
        }

    ];

    return colonneArray;
}

function kReadValorizzazioneProdotto_mod() {

    var data = $('#hdgridProdotto').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}


function OttieniRigheSelezionateGridProdotto() {

    let righe_selezionate_string = "";

    let gridProdotto = $("#gridProdotto").data("kendoGrid");

    if (gridProdotto === undefined || gridProdotto === null || gridProdotto === "" || $("#divgridProdotto").is(":visible")===false)
        return righe_selezionate_string;

    let gridProdotto_DataSource = gridProdotto.dataSource.data();

    let mat_cod_array = [];

    for (var x = 0; x < gridProdotto_DataSource.length; x++) {

        if (gridProdotto_DataSource[x].Selected === true) {

            let mat_cod = 0;

            if (gridProdotto_DataSource[x].Prodotto_Cod < 0)
                mat_cod = gridProdotto_DataSource[x].Prodotto_Cod * -1;

            mat_cod_array.push(mat_cod);

        }

    }

    if (mat_cod_array.length > 0) {
        //Se sono stati selezionati tutti i Prodotti passo il filtro vuoto
        if (mat_cod_array.length === gridProdotto_DataSource.length) {
            righe_selezionate_string = "";
        } else {
            righe_selezionate_string = mat_cod_array.join("|");
        }


    }

    return righe_selezionate_string;

}


function DeselezionaRigheGridProdotto() {

    let gridProdotto = $("#gridProdotto").data("kendoGrid");

    if (gridProdotto === undefined || gridProdotto === null || gridProdotto === "")
        return;

    let gridProdotto_DataSource = gridProdotto.dataSource.data();

    for (var x = 0; x < gridProdotto_DataSource.length; x++) {

        if (gridProdotto_DataSource[x].Selected === true) {

            gridProdotto_DataSource[x].Selected = false;

        }

    }
}

$("#btn_filtro_prodotti").on("click", function () {

    let estrazione = parseInt(Get_KendoDDLValue("ddlEstrazioni"))

    if ((elemCod === 210 || elemCod === 310) &&
        (estrazione !== enumEstrazioniConf.EC_Imballi.Value && estrazione !== enumEstrazioniConf.Saldo_Imballi.Value)) {

        $("#divgridProdotto").show();
        LeggiProdotti();
        creakendoGridProdotti("gridProdotto");
    }

});