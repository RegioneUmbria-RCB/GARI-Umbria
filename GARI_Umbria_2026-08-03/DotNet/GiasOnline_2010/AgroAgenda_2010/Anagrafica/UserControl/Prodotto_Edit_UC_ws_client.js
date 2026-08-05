var indirizzohttpPaginaProdotto = "./Prodotto_Edit.aspx";

function RicercaElencoCompletoProdottiPerAnagrafica(keys, objP_super_server, objP_server, objP_utenti, piva,
    Sa_Cod, Fabbricato_Cod, TipoDestinazione,
    Elem_Cod, soloInGiacenza, filtroPerDescrizione,
    Mode, Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, leggiUMformulati, Flag_QtaNoZero, xTipoPUARegolamento) {

    if (Elem_Cod === "")
        Elem_Cod = 0;

    var elencoProdottiCompleto = "[]";

    //Filtro per Regolamento
    var filtroReg = parseInt($('input[name=FiltroReg]:checked').val());

    //Filtro per Visibilità
    var filtroVisi = parseInt($('input[name=FiltroVisi]:checked').val());

    //Filtro per Valorizzati(Se presenti dei Costi o meno)
    var filtroValoriz = parseInt($('input[name=FiltroValoriz]:checked').val());

    var filtro_specie = "";

    var filtro_varieta = "";

    if (categorieProdotti_FiltrabiliXSpecieVarieta.includes(parseInt(Elem_Cod))) {

        if ($("#multiselSpecie").data("kendoMultiSelect").value().length > 0)
            filtro_specie = $("#multiselSpecie").data("kendoMultiSelect").value().join("|");

        if ($("#multiselVarieta").data("kendoMultiSelect").value().length > 0)
            filtro_varieta = $("#multiselVarieta").data("kendoMultiSelect").value().join("|");
    }


    var filtro_categoria_commerciale = "";


    if (categorieProdotti_FiltrabiliXCategoriaCommerciale.includes(parseInt(Elem_Cod))) {

        if ($("#multiselCategCommle").data("kendoMultiSelect").value().includes(0) === false)
            filtro_categoria_commerciale = $("#multiselCategCommle").data("kendoMultiSelect").value().join(",");

    }

    var XFiltroAggiuntivoMateriePrime = GeneraFiltroAggiuntivoXRicercaAnagraficaProdotti(filtro_categoria_commerciale, filtroReg, filtroVisi);

    //Non richiamo la RicercaElencoCompletoProdotti() in leggiTabelle_ws_client perchè altrimenti non mostra il waitframe di caricamento
    var param = kendo.stringify({
        objP_super_server: objP_super_server,
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva,
        xSa_Cod: parseInt(Sa_Cod),
        xFabbricato_Cod: parseInt(Fabbricato_Cod),
        xTipoDestinazione: parseInt(TipoDestinazione),
        Elem_Cod: parseInt(Elem_Cod),
        soloInGiacenza: soloInGiacenza,
        FiltroDescrizioneProdotto: filtroPerDescrizione,
        Mode: Mode,
        Cau_Mov: Cau_Mov,
        Data_Movimento_Str: Data_Movimento,
        xPUARegolamento: xPUARegolamento,
        xLottoAccettazione: xLottoAccettazione,
        leggiUMformulati: leggiUMformulati,
        metaschema: "",
        Flag_QtaNoZero: Flag_QtaNoZero,
        xTipoPUARegolamento: xTipoPUARegolamento,
        Elenco_Specie: filtro_specie,
        Elenco_Varieta: filtro_varieta,
        xFiltroAggiuntivoMateriePrime: XFiltroAggiuntivoMateriePrime,
        creaGriglia: true,
        filtroProdottiValorizzati: filtroValoriz,
        flagDiversificaDesFertilizzanti: false,
        FiltroCodiceProdotto: 0,
        FiltroCodiceTrappola: 0
    });

    WaitFrame.show();
    ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_ElencoCompletoProdotti + "/LeggiElencoCompletoProdotti",
        param,
        function (risposta) {
            var risp = risposta.RispostaStringa;
            var elencoProdottiLettiGrid = JSON.parse(risp);
            var elencoProdottiLetti = JSON.parse(risp).kendo_rows;

            var righe_griglia_kendoRicerca = [];


            if (Prodotto_Edit_UC_Elenco_Categorie_Magazzino === null)
                Prodotto_Edit_UC_Elenco_Categorie_Magazzino = CaricaCategorieMagazzinoXUtente();

            //Escludo le righe che hanno delle categorie che non possono essere viste dall'utente.
            for (var x = 0; x < elencoProdottiLetti.length; x++) {
                for (var y = 0; y < Prodotto_Edit_UC_Elenco_Categorie_Magazzino.length; y++) {

                    if (parseInt(elencoProdottiLetti[x].Elem_Cod) === parseInt(Prodotto_Edit_UC_Elenco_Categorie_Magazzino[y].Elem_Cod)) {
                        righe_griglia_kendoRicerca.push(elencoProdottiLetti[x]);
                    }
                }
            }

            elencoProdottiLettiGrid.kendo_rows = righe_griglia_kendoRicerca;

            elencoProdottiCompleto = JSON.stringify(elencoProdottiLettiGrid);

            if (elencoProdottiCompleto !== undefined && elencoProdottiCompleto !== null && elencoProdottiCompleto !== "[]") {

                $('#hdKendoProdotto_Valorizzazione').val(elencoProdottiCompleto);
                kendoProdotto_inizializza("divKendoProdotto", keys);


                let date = new Date();
                date.setTime(date.getTime() + (1 * 24 * 60 * 60 * 1000));

                $.cookie("Prodotto_Edit_UC_Descrizione", JSON.parse(filtroPerDescrizione)[0].value, { expires: date, path: '/' });
                $.cookie("Prodotto_Edit_UC_Categoria", Elem_Cod, { expires: date, path: '/' });

                if ($("#multiselSpecie").data("kendoMultiSelect") !== undefined &&
                    $("#multiselSpecie").data("kendoMultiSelect") !== null &&
                    $("#multiselSpecie").data("kendoMultiSelect") !== null)
                    $.cookie("Prodotto_Edit_UC_Specie", $("#multiselSpecie").data("kendoMultiSelect").value().join(","), { expires: date, path: '/' });

                if ($("#multiselVarieta").data("kendoMultiSelect") !== undefined &&
                    $("#multiselVarieta").data("kendoMultiSelect") !== null &&
                    $("#multiselVarieta").data("kendoMultiSelect") !== null)
                    $.cookie("Prodotto_Edit_UC_Varieta", $("#multiselVarieta").data("kendoMultiSelect").value().join(","), { expires: date, path: '/' });

                if ($("#multiselCategCommle").data("kendoMultiSelect") !== undefined &&
                    $("#multiselCategCommle").data("kendoMultiSelect") !== null &&
                    $("#multiselCategCommle").data("kendoMultiSelect") !== null)
                    $.cookie("Prodotto_Edit_UC_CategCommle", $("#multiselCategCommle").data("kendoMultiSelect").value().join(","), { expires: date, path: '/' });

                $.cookie("Prodotto_Edit_UC_Reg", filtroReg, { expires: date, path: '/' });

                $.cookie("Prodotto_Edit_UC_Visi", filtroVisi, { expires: date, path: '/' });

                $.cookie("Prodotto_Edit_UC_Valoriz", filtroValoriz, { expires: date, path: '/' });
            }

            WaitFrame.hide();

        }, function () {
            WaitFrame.hide();
        });

}

function Ricerca_Specie() {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Specie",
        "{ }",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

             //Aggiungo anche il filtro di specie vegetale “--- Specie non valorizzata” con veg_cod=0
            //che c'era nell'online vecchio, nell'ultima posizione
            let trovata = false;
            let index_specie_non_valorizzata = -1;

            for (var x = 0; x < risp.length; x++) {
                if (risp[x].veg_cod === 0) {
                    trovata = true;
                    index_specie_non_valorizzata = x;
                    break;
                }

            }

            let default_str_specie = "--- " + Traduzione(menuBSAnagraficaResx, "SpecieNonValorizzata", "Specie non valorizzata");

            if (trovata && index_specie_non_valorizzata!==-1) {

                risp[index_specie_non_valorizzata].veg_des = default_str_specie;

                risp.push(risp.splice(index_specie_non_valorizzata, 1)[0]);

            }else {
                let specie_non_valorizzata = {
                    "veg_cod": 0,
                    "veg_des": default_str_specie
                };

                risp.unshift(specie_non_valorizzata);
            }

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
            let risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}


function Leggi_Categorie_Commerciali() {

    var piva = $('#' + hidden_azienda_ClientID).val();
    var param = "{ piva: '" + piva + "'}";
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

function Carica_ddl_categ_prod(options) {
    if (Prodotto_Edit_UC_Elenco_Categorie_Magazzino === null)
        Prodotto_Edit_UC_Elenco_Categorie_Magazzino = CaricaCategorieMagazzinoXUtente();

    let tipo_operazione = parseInt($(Controls.xTipoOperazione).val());
    if (tipo_operazione === enum_tipoOperazione.Scrittura) {
        var categorieAmmesse = [];
        for (var x = 0; x < Prodotto_Edit_UC_Elenco_Categorie_Magazzino.length; x++) {
            //Escludo i Mangimi perchè nella vecchia gestione non li fa inserire come nuovi
            if ((Prodotto_Edit_UC_Elenco_Categorie_Magazzino[x].Tabella === "Materie_Prime" || Prodotto_Edit_UC_Elenco_Categorie_Magazzino[x].NomeComune === "" || Prodotto_Edit_UC_Elenco_Categorie_Magazzino[x].Tabella === "TipologieSementi") &&
                (Prodotto_Edit_UC_Elenco_Categorie_Magazzino[x].Elem_Cod !== categorieProdotti.MANGIMI)) {

                categorieAmmesse.push({ Elem_Cod: Prodotto_Edit_UC_Elenco_Categorie_Magazzino[x].Elem_Cod, NomeComune: Prodotto_Edit_UC_Elenco_Categorie_Magazzino[x].NomeComune });
            }
        }
        options.success(categorieAmmesse);
    }
    else {
        options.success(Prodotto_Edit_UC_Elenco_Categorie_Magazzino);
    }
}


function CaricaCategorieMagazzinoXUtente() {
    let categorieMag_filtrate = [];
    ajaxAgronicaSync("../Anagrafica/Prodotto_Edit.aspx/CaricaCategorieMagazzinoXUtente",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            //Escludo le categorie Coadiuvanti , Macchine e Attrezzature,
            //Rifiuti ,  Consistenza Zootecnica e Farmaci
            //Per ora escludo anche Confezioni Prodotti e  Alri Beni Ammortizzabili.
            for (var x = 0; x < risp.length; x++) {
                if (risp[x].Elem_Cod !== categorieProdotti.MACCHINE_ATTREZZATURE &&
                    risp[x].Elem_Cod !== categorieProdotti.COADIUVANTI &&
                    risp[x].Elem_Cod !== categorieProdotti.RIFIUTI &&
                    risp[x].Elem_Cod !== categorieProdotti.CONSISTENZA_ZOOTECNICA &&
                    risp[x].Elem_Cod !== categorieProdotti.FARMACI &&
                    risp[x].Elem_Cod !== categorieProdotti.CONFEZIONI_PRODOTTI &&
                    risp[x].Elem_Cod !== categorieProdotti.ALTRI_BENI_AMMORTIZZABILI) {
                    categorieMag_filtrate.push(risp[x]);
                }
            }

            if (risposta.ParametroDue) {
                if (risposta.ParametroDue_stringa === "" || risposta.ParametroDue_stringa === "0") {
                    Prodotto_Edit_UC_DefaultCategoriaProdotto = 0;
                }else {
                    Prodotto_Edit_UC_DefaultCategoriaProdotto=parseInt(risposta.ParametroDue_stringa);
                }
            }

        }, null);
    return categorieMag_filtrate;
}

function Leggi_ImpostazioniUtenteTabConfigurazione() {
    var Abilita_DDLCategBeniConf = false;
    var Mostra_ChkImballaggio = false;
    var Mostra_ChkContenitore = false;

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_ImpostazioneUtente",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            if (risp === true)
                Abilita_DDLCategBeniConf = true;

            let risp2 = JSON.parse(risposta.ParametroDue_stringa)[0];

            if (risp2.MostraImballaggi === "true")
                Mostra_ChkImballaggio = true;

            if (risp2.MostraContenitori === "true")
                Mostra_ChkContenitore = true;
        }, null);

    var Impostazioni_Utente = {
        Abilita_DDLCategBeniConf: Abilita_DDLCategBeniConf,
        Mostra_ChkImballaggio: Mostra_ChkImballaggio,
        Mostra_ChkContenitore: Mostra_ChkContenitore
    };

    return Impostazioni_Utente;
}

function Carica_ddl_prodotto_UC_TecnologieSementi() {
    elencoTecnologieSementi = [];

    var param = kendo.stringify({
        "objP_server": objP_server
    });

    ajaxAgronicaSync(pathCoreWS + "Metaschema/Tecnologie_Sementi.asmx/CaricaCombo_TecnologieSementi",
        param, false,
        function (risposta) {
            let str_risp = JSON.parse(risposta.RispostaStringa);
            elencoTecnologieSementi = str_risp;
        }, null);


    return elencoTecnologieSementi;
}


function Carica_ddl_prodotto_UC_specie_veg(SEM_COD) {
    var ElencoSpecie = [];
    if (SEM_COD === null || SEM_COD === "") {
        ElencoSpecie = RicercaSpecie($(cIdPiva).val(), true);
    }
    else {

        var param = kendo.stringify({
            "Sem_Cod": SEM_COD,
            "objP_server": objP_server,
            "objP_utenti": objP_utenti,
            "Veg_Cod": 0, "Flag_PrimaRiga": true, "Testo_PrimaRiga": "", "Cod_PrimaRiga": -1,
            "FinestraTemp_Inizio": "01/01/1900", "FinestraTemp_Fine": "31/12/2100", "FiltroAggiuntivo": "",
            "Ordinamento": "", "Flag_FiltroUtente": true, "Veg_Cod_daModificare": 0, "SemCod_Rif_VegCod_daModificare": 0
        });
        ajaxAgronicaSync(pathCoreWS + "Metaschema/TipologieSementi.asmx/CaricaCombo_SpecieVegetale_Semente_New",
            param, false,
            function (risposta) {
                let str_risp = JSON.parse(risposta.RispostaStringa);
                var Ar1 = str_risp.map(function (obj) { obj['Veg_Cod'] = obj['veg_cod']; obj['Veg_Des'] = obj['veg_des']; delete obj['veg_cod']; delete obj['veg_des']; return obj; });
                ElencoSpecie = Ar1;
            }, null);
    }
    return ElencoSpecie;
}

function Carica_ddl_razza_anim(gen_cod, spe_cod) {
    var ElencoRazze = [];
    var param = kendo.stringify(
        {
            Gen_Cod: gen_cod,
            Spe_Cod: spe_cod
        });
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlRazzaAnimale",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            ElencoRazze = risp;
        }, null);

    return ElencoRazze;
}

function Carica_ddl_prodotto_UC_varieta_Load(veg_cod, Cul_cod) {
    var elencovarieta = [];
    var param = kendo.stringify(
        {
            piva: $(Controls.xPiva).val(),
            Veg_cod: veg_cod,
            Cul_cod_da_modificare: Cul_cod
        });
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlVarietaColturale",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let objVuoto = {
                "Cul_Cod": 0,
                "Cul_Des": ""
            };
            risp.unshift(objVuoto);
            elencovarieta = risp;
        }, null);
    return elencovarieta;
}

function Carica_ddl_prodotto_UC_ditta_di_provenienza(options) {

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlDittadiProvenienza",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}


function Carica_ddl_sementi_materiale(options) {
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlTipologieSementi",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let objVuoto = {
                "SEM_COD": 0,
                "SEM_DES": ""
            };
            risp.unshift(objVuoto);
            options.success(risp);
        }, null);
}


function Carica_ddl_specie_anim(options) {
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlSpecieAnimali",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function Carica_ddl_indi_produt(gen_cod, spe_cod) {
    var ElencoIndirizzi = [];
    var param = kendo.stringify(
        {
            Gen_Cod: gen_cod,
            Spe_Cod: spe_cod
        });
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlIndirizzoProduttivo",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            ElencoIndirizzi = risp;
        }, null);

    return ElencoIndirizzi;
}

function LeggiProdotto(tipoOp) {
    let prodotto = null;
    let Operazione = "";
    let Duplica = "False";

    switch (tipoOp) {
        //Lettura
        case enum_tipoOperazione.Lettura:
            Operazione = "InfoProdotto";
            break;
        //Scrittura
        case enum_tipoOperazione.Scrittura:
            Operazione = "";
            break;
        //Modifica
        case enum_tipoOperazione.Modifica:
            Operazione = "EditProdotto";
            break;
        //Duplica(Copia un prodotto)
        case enum_tipoOperazione.Duplica:
            Operazione = "EditProdotto";
            Duplica = "True";
            break;
    }

    let param = null;
    if (Operazione === "EditProdotto") {
        let Proprietario = $(Controls.xProprietario).val();
        param = kendo.stringify({
            "piva": $(Controls.xPiva).val(), "elem_cod": $(Controls.xElem_Cod).val(),
            "mat_cod": $(Controls.xMat_Cod).val(), "duplica": Duplica, "proprietario": Proprietario, "sa_cod": parseInt($(xSa_Cod).val())
        });
    } else {
        param = kendo.stringify({ "piva": $(Controls.xPiva).val(), "elem_cod": $(Controls.xElem_Cod).val(), "mat_cod": $(Controls.xMat_Cod).val() });
    }

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/" + Operazione,
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            prodotto = risp[0];
            if (risposta.ParametroDue && Duplica === "True") {
                tabProdotto_Edit_UC_DaDuplicare = JSON.parse(risposta.ParametroDue_stringa);
            }               
        }, null);

    return prodotto;
}

function Carica_ddl_categ_ris() {
    let categorieRisorse = null;
    var param = kendo.stringify(
        {
            piva: $(Controls.xPiva).val(),
            Tipo_Classe: 0
        });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlCategoriaRisorsa",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let objVuoto = {
                "Linea_Classe_Cod": 0,
                "Linea_Classe_Des": ""
            };
            risp.unshift(objVuoto);
            categorieRisorse = risp;
        }, null);

    return categorieRisorse;
}

function Carica_ddl_linea_prod(linea_classe_cod) {
    var ElencoLineeProduzione = [];
    var param = kendo.stringify(
        {
            piva: $(Controls.xPiva).val(),
            Linea_Classe_Cod: linea_classe_cod
        });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlLineaProduzione",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let objVuoto = {
                "Linea_Cod": 0,
                "Linea_Des": ""
            };
            risp.unshift(objVuoto);
            ElencoLineeProduzione = risp;
        }, null);
    return ElencoLineeProduzione;
}

function Carica_ddl_prodotto_UC_final_prod(veg_cod) {
    var ElencoGruppiFinalita = [];
    var param = kendo.stringify(
        {
            grfi_cod: 0,
            veg_cod: veg_cod
        });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlFinalitaProduttiva",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let objVuoto = {
                "Grfi_Cod": 0,
                "Grfi_Des": ""
            };
            risp.unshift(objVuoto);
            ElencoGruppiFinalita = risp;
        }, null);
    return ElencoGruppiFinalita;
}

function Carica_ddl_prodotto_UC_unita_mis_def(elem_cod) {
    var ElencoUnitaMisuraDefault = [];
    var param = kendo.stringify(
        {
            elem_cod: elem_cod
        });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlUnitaMisuraDefault",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let objVuoto = {
                "udm_cod": 0,
                "udm_des": ""
            };
            risp.unshift(objVuoto);
            ElencoUnitaMisuraDefault = risp;
        }, null);
    return ElencoUnitaMisuraDefault;
}


function Leggi_Prodotti_Extra_PrivataXTab() {
    let prodotto_extra_privata = null;


    if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Scrittura) {
        prodotto_extra_privata = [];
    }
    else {
        //Se sto duplicando un prodotto aggiungo le righe lette dal prodtto duplicato come righe nuove del nuovo prodotto
        if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Duplica &&
            tabProdotto_Edit_UC_DaDuplicare !== undefined && tabProdotto_Edit_UC_DaDuplicare !== null && tabProdotto_Edit_UC_DaDuplicare.length > 0 &&
            tabProdotto_Edit_UC_DaDuplicare.find(o => o.tab === "a_tab_prodotto_UC_dati_contabilita")) {

            for (var x = 0; x < tabProdotto_Edit_UC_DaDuplicare.length; x++) {
                if (tabProdotto_Edit_UC_DaDuplicare[x].tab === "a_tab_prodotto_UC_dati_contabilita") {
                    prodotto_extra_privata = JSON.parse(tabProdotto_Edit_UC_DaDuplicare[x].dati)[0];
                    break;
                }
            }
        }
        else {
            let Piva = "";
            let proprietario = ($(Controls.xProprietario).val() === "True");
            let Elem_Cod = parseInt($(Controls.xElem_Cod).val());
            let materia_prima = IsMateriaPrima(Elem_Cod);
            //Carico Prodotti_Extra_Privata
            //Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietatio
            //Se flag proprietario = true passo parametri.Piva
            //Se flag proprietario = false passo objParametriAgenda.Piva
            if (proprietario === true)
                Piva = $(Controls.xPiva).val();
            else
                Piva = $(Controls.xPiva_Corrente).val();


            //Se il prodotto è una Materia_Prima allora leggo i valori come se fossi proprietario,
            //ma dopo rimangono in sola lettura.
            if (materia_prima === true)
                Piva = $(Controls.xPiva).val();


            let risp = Leggi_Prodotti_Extra_Privata(Piva, Elem_Cod, parseInt($(Controls.xMat_Cod).val()), parseInt($(Controls.xPro_Cod).val()),false);

            if (risp[0] !== undefined)
                prodotto_extra_privata = risp[0];
        }
    }

    return prodotto_extra_privata;
}


function Leggi_Prodotti_Extra_Privata(Piva, Elem_cod, Mat_cod, Pro_cod, leggiXGridRicerca) {

    let indirizzo = indirizzohttpPaginaProdotto;

    if (leggiXGridRicerca===true)
        indirizzo="../Anagrafica/Prodotto_Edit.aspx";

    let prodotto_extra_privata = null;

    var param = kendo.stringify({
        piva: Piva,
        elem_cod: Elem_cod,
        mat_cod: Mat_cod,
        pro_cod: Pro_cod
    });

    ajaxAgronicaSync(indirizzo + "/Leggi_Prodotti_Extra_Privata",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            prodotto_extra_privata = risp;
        }, null);

    return prodotto_extra_privata;
}


function CaricaProdottiXGrigliaStoricoPrezzi(options) {

    if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Scrittura) {
        options.success([]);
    }
    else {
        //Se sto duplicando un prodotto aggiungo le righe lette dal prodtto duplicato come righe nuove del nuovo prodotto
        if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Duplica &&
            tabProdotto_Edit_UC_DaDuplicare !== undefined && tabProdotto_Edit_UC_DaDuplicare !== null && tabProdotto_Edit_UC_DaDuplicare.length > 0 &&
            tabProdotto_Edit_UC_DaDuplicare.find(o => o.tab === "a_tab_prodotto_UC_storico_prezzi")) {

            for (var x = 0; x < tabProdotto_Edit_UC_DaDuplicare.length; x++) {
                if (tabProdotto_Edit_UC_DaDuplicare[x].tab === "a_tab_prodotto_UC_storico_prezzi") {
                    let ds = JSON.parse(tabProdotto_Edit_UC_DaDuplicare[x].dati);
                    options.success(ds);
                    break;
                }
            }

            let grid = $("#prodotto_UC_griglia_storico_prezzi").data("kendoGrid");
            let currentData = grid.dataSource.data();

            for (let i = 0; i < currentData.length; i++) {
                currentData[i].ID = 0;
                currentData[i].id = 0;
            }
        }
        else {
            let Piva = "";
            let proprietario = ($(Controls.xProprietario).val() === "True");
            //Carico Storico Prezzi
            //Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietatio
            //Se flag proprietario = true passo parametri.Piva
            //Se flag proprietario = false passo objParametriAgenda.Piva
            if (proprietario === true)
                Piva = $(Controls.xPiva).val();
            else
                Piva = $(Controls.xPiva_Corrente).val();

            let elencoPrezzi = LeggiGrigliaStoricoPrezzi(Piva, $(Controls.xElem_Cod).val(), $(Controls.xMat_Cod).val(), $(Controls.xPro_Cod).val());

            options.success(elencoPrezzi);
            //var param = kendo.stringify(
            //    {
            //        piva: Piva,
            //        elem_cod: $(Controls.xElem_Cod).val(),
            //        mat_cod: $(Controls.xMat_Cod).val(),
            //        pro_cod: $(Controls.xPro_Cod).val()
            //    });

            //ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/CaricaGrigliaStoricoPrezzi",
            //    param,
            //    false,
            //    function (risposta) {
            //        risp = JSON.parse(risposta.RispostaStringa);
            //        options.success(risp);
            //    }, null);
        }
    }  

}

function LeggiGrigliaStoricoPrezzi(Piva, Elem_Cod, Mat_Cod, Pro_Cod) {

    var ElencoStoricoPrezzi = [];

    var param = kendo.stringify(
        {
            piva: Piva,
            elem_cod: Elem_Cod,
            mat_cod: Mat_Cod,
            pro_cod: Pro_Cod
        });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/CaricaGrigliaStoricoPrezzi",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            ElencoStoricoPrezzi = risp;
        }, null);

    return ElencoStoricoPrezzi;
}

function Carica_ddl_griglia_unita_di_misura() {
    var Ar1 = [];

    //In realtà viene fatto filtro solo sull'elem_cod

    var param = kendo.stringify(
        {
            piva: $(Controls.xPiva).val(),
            elem_cod: $(Controls.xElem_Cod).val(),
            mat_cod: $(Controls.xMat_Cod).val(),
            sa_cod: $(xSa_Cod).val()
        });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_Unita_Di_Misura",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            Ar1 = risp;
        }, null);

    var elencounitadimisura = Ar1.map(function (obj) { obj['Udm_Cod'] = obj['udm_cod']; obj['Udm_Des'] = obj['udm_des']; delete obj['udm_cod']; delete obj['udm_des']; return obj; });
    return elencounitadimisura;
}

function Carica_ddl_prodotto_UC_tipo_varietale(Veg_Cod) {
    var ElencoTipo = [];
    var param = kendo.stringify(
        {
            Veg_Cod: Veg_Cod
        });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlGruppoVarietale",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let objVuoto = {
                "Grva_Cod": 0,
                "Grva_Des": ""
            };
            risp.unshift(objVuoto);
            ElencoTipo = risp;
        }, null);
    return ElencoTipo;
}

function CaricaProdottiXGrigliaCalibri() {
    var ElencoCalibriScelti = [];

    if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura) {
        //Se sto duplicando un prodotto aggiungo le righe lette dal prodtto duplicato come righe nuove del nuovo prodotto
        if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Duplica && 
            tabProdotto_Edit_UC_DaDuplicare !== undefined && tabProdotto_Edit_UC_DaDuplicare !== null && tabProdotto_Edit_UC_DaDuplicare.length > 0 &&
            tabProdotto_Edit_UC_DaDuplicare.find(o => o.tab === "a_tab_prodotto_UC_parametri_qualitativi")) {

            for (var x = 0; x < tabProdotto_Edit_UC_DaDuplicare.length; x++) {
                if (tabProdotto_Edit_UC_DaDuplicare[x].tab === "a_tab_prodotto_UC_parametri_qualitativi") {
                    ElencoCalibriScelti = JSON.parse(tabProdotto_Edit_UC_DaDuplicare[x].datiparamqualcalibri);
                    break;
                }
            }
        }
        else {
            let obj = {
                piva: $(Controls.xPiva).val(),
                mat_cod: $(Controls.xMat_Cod).val(),
                sa_cod: $(xSa_Cod).val()
            };

            //Se sono in scrittura passo al mat_cod -1 così non trova nessun calibro scelto
            if (enum_tipoOperazione.Scrittura === parseInt($(Controls.xTipoOperazione).val())) {
                obj.mat_cod = -1;
            }

            let param = kendo.stringify(obj);

            ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/CaricaGrigliaCalibri",
                param,
                false,
                function (risposta) {
                    let risp = JSON.parse(risposta.RispostaStringa);
                    ElencoCalibriScelti = risp;
                }, null);
        }
    }
    
    return ElencoCalibriScelti;
}

function CaricaProdottiXGrigliaTuttiCalibri(options) {
    var elencocalibri = [];

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_CalibriFrutti",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            elencocalibri = risp.map(function (obj) { obj['Tipo_Cod'] = obj['cal_cod']; obj['Cal_Des'] = obj['cal_des']; delete obj['cal_cod']; delete obj['cal_des']; return obj; });
        }, null);

    options.success(elencocalibri);
}

function CaricaProdottiXGrigliaIndici() {
    var ElencoIndiciScelti = [];

    if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura) {
        //Se sto duplicando un prodotto aggiungo le righe lette dal prodtto duplicato come righe nuove del nuovo prodotto
        if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Duplica && 
            tabProdotto_Edit_UC_DaDuplicare !== undefined && tabProdotto_Edit_UC_DaDuplicare !== null && tabProdotto_Edit_UC_DaDuplicare.length > 0 &&
            tabProdotto_Edit_UC_DaDuplicare.find(o => o.tab === "a_tab_prodotto_UC_parametri_qualitativi")) {

            for (var x = 0; x < tabProdotto_Edit_UC_DaDuplicare.length; x++) {
                if (tabProdotto_Edit_UC_DaDuplicare[x].tab === "a_tab_prodotto_UC_parametri_qualitativi") {
                    let ds = JSON.parse(tabProdotto_Edit_UC_DaDuplicare[x].datiparamqualindici);
                    ElencoIndiciScelti = ds.map(function (obj) { obj['IND_MAT_COD'] = obj['Tipo_Cod']; obj['IND_MAT_DES'] = obj['Ind_Mat_Des']; delete obj['Tipo_Cod']; delete obj['Ind_Mat_Des']; return obj; });
                    break;
                }
            }
        }
        else {
            var param = kendo.stringify(
                {
                    piva: $(Controls.xPiva).val(),
                    mat_cod: $(Controls.xMat_Cod).val(),
                    sa_cod: $(xSa_Cod).val()
                });

            ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/CaricaGrigliaIndici",
                param,
                false,
                function (risposta) {
                    let risp = JSON.parse(risposta.RispostaStringa);
                    ElencoIndiciScelti = risp.map(function (obj) { obj['IND_MAT_COD'] = obj['Tipo_Cod']; obj['IND_MAT_DES'] = obj['Ind_Mat_Des']; delete obj['Tipo_Cod']; delete obj['Ind_Mat_Des']; return obj; });
                }, null);
        }
    }

    return ElencoIndiciScelti;
}

function CaricaProdottiXGrigliaTuttiIndici(options) {
    let veg_cod = 0;

    if (KendoDDL("ddl_prodotto_UC_specie_veg") !== undefined && KendoDDL("ddl_prodotto_UC_specie_veg") !== null) {
        if (Get_KendoDDLValue("ddl_prodotto_UC_specie_veg") !== "" && Get_KendoDDLValue("ddl_prodotto_UC_specie_veg") !== null &&
            Get_KendoDDLValue("ddl_prodotto_UC_specie_veg") !== undefined ) {
            veg_cod = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_specie_veg"));
        }
    }

    var param = kendo.stringify(
        {
            Veg_Cod: veg_cod
        });
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_IndiciMaturita",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function Carica_ddl_prodotto_UC_Prodotto_base(regolamento, veg_cod, cul_cod) {
    var ElencoProdotti = [];

    var param = kendo.stringify(
        {
            piva: $(Controls.xPiva).val(),
            elem_cod: $(Controls.xElem_Cod).val(),
            veg_cod: veg_cod,
            cul_cod: cul_cod,
            regolamento: regolamento
        });
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlProdottoBase",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            ElencoProdotti = risp;
            ElencoProdotti.unshift({ "Mat_Cod": 0, "Mat_Des": "" });
        }, null);
    return ElencoProdotti;
}

function CaricaProdottiXGrigliaTraduzioni(options) {

    //Se sto duplicando un prodotto aggiungo le righe lette dal prodtto duplicato come righe nuove del nuovo prodotto
    if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Duplica && 
        tabProdotto_Edit_UC_DaDuplicare !== undefined && tabProdotto_Edit_UC_DaDuplicare !== null && tabProdotto_Edit_UC_DaDuplicare.length > 0 &&
        tabProdotto_Edit_UC_DaDuplicare.find(o => o.tab === "a_tab_prodotto_UC_traduzioni")) {

        for (var x = 0; x < tabProdotto_Edit_UC_DaDuplicare.length; x++) {
            if (tabProdotto_Edit_UC_DaDuplicare[x].tab === "a_tab_prodotto_UC_traduzioni") {
                let ds = JSON.parse(tabProdotto_Edit_UC_DaDuplicare[x].dati);
                options.success(ds);
                break;
            }
        }

        let grid = $("#prodotto_UC_griglia_traduzioni").data("kendoGrid");
        let currentData = grid.dataSource.data();

        for (let i = 0; i < currentData.length; i++) {
            currentData[i].Key_Traduzioni = "";
            currentData[i].id = "";
        }

    }
    else {

        var param = kendo.stringify(
            {
                piva: $(Controls.xPiva).val(),
                elem_cod: $(Controls.xElem_Cod).val(),
                mat_cod: $(Controls.xMat_Cod).val()
            });

        ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/CaricaGrigliaTraduzioni",
            param,
            false,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);
            }, null);
    }
}


function Carica_ddl_lingua_griglia_traduzioni() {
    var elencolingue = [];

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlLingua",
        "{}",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            elencolingue = risp;
        }, null);
    return elencolingue;
}

function Carica_prodotto_UC_ddlCodIva(options) {

    let Piva = "";
    let proprietario = ($(Controls.xProprietario).val() === "True");
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    //Carico IVA
    //Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietatio
    //Se flag proprietario = true passo parametri.Piva
    //Se flag proprietario = false passo objParametriAgenda.Piva
    if (proprietario === true)
        Piva = $(Controls.xPiva).val();
    else
        Piva = $(Controls.xPiva_Corrente).val();

    //Se il prodotto è una Materia_Prima allora leggo i valori come se fossi proprietario,
    //ma dopo rimangono in sola lettura.
    if (IsMateriaPrima(elem_cod) === true)
        Piva = $(Controls.xPiva).val();

    options.success(RicercaIVA_Aliquote(Piva));

}

function Carica_prodotto_UC_ddlCodIvaCompensazione(options) {
    var ElencoIvaComp = [];
    var risp = null;
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlIVACompensazione",
        "{}", false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            let objVuoto = {
                "Cod": 0,
                "Descrizione": ""
            };
            risp.unshift(objVuoto);
        },
        null);
    ElencoIvaComp[0] = { Cod: risp[0].Cod, Descrizione: risp[0].Descrizione };
    for (var i = 1; i < risp.length; i++) {
        ElencoIvaComp[i] = { Cod: risp[i].Cod, Descrizione: risp[i].Codice + " - " + risp[i].Descrizione + "-" + TraduzioneMultiResx(resxProdottoEditUC, "IvaOrdinariaBreve", "Iva Ord.") + risp[i].Aliquota_Ordinaria + " % - " + TraduzioneMultiResx(resxProdottoEditUC, "IvaCompensazioneBreve", "Iva Comp.") + risp[i].Aliquota_Compensativa + " %" };
    }
    options.success(ElencoIvaComp);

}

function Carica_prodotto_UC_ddlContoEconomicoAcquisto(options) {

    let Piva = "";
    let proprietario = ($(Controls.xProprietario).val() === "True");
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    //Carico ContoEconomicoAcquisto
    //Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietatio
    //Se flag proprietario = true passo parametri.Piva
    //Se flag proprietario = false passo objParametriAgenda.Piva
    if (proprietario === true)
        Piva = $(Controls.xPiva).val();
    else
        Piva = $(Controls.xPiva_Corrente).val();

    //Se il prodotto è una Materia_Prima allora leggo i valori come se fossi proprietario,
    //ma dopo rimangono in sola lettura.
    if (IsMateriaPrima(elem_cod) === true)
        Piva = $(Controls.xPiva).val();

    var param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: Piva,
        anno: new Date().getFullYear(),
        ricCod: 2,
        tipoDareAvere: "D",
        codConto: 0,
        imputabile: 1
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Contab/Conti.asmx/WS_Leggi_ContiEconomici",
        param, false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            var objVuoto = { "Cod_Conto": -1, "Descr_Conto": "" };
            risp.unshift(objVuoto);
            options.success(risp);
        },
        null);
  
}


function Carica_prodotto_UC_ddlContoEconomicoVendita(options) {
    let Piva = "";
    let proprietario = ($(Controls.xProprietario).val() === "True");
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    //Carico ContoEconomicoVendita
    //Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietatio
    //Se flag proprietario = true passo parametri.Piva
    //Se flag proprietario = false passo objParametriAgenda.Piva
    if (proprietario === true)
        Piva = $(Controls.xPiva).val();
    else
        Piva = $(Controls.xPiva_Corrente).val();

    //Se il prodotto è una Materia_Prima allora leggo i valori come se fossi proprietario,
    //ma dopo rimangono in sola lettura.
    if (IsMateriaPrima(elem_cod) === true)
        Piva = $(Controls.xPiva).val();

    options.success(RicercaContoEconomico(true, Piva, new Date().getFullYear(), "A", false));
}


function Carica_prodotto_UC_ddlContoPatrimonialeAcquisto(options) {

    let Piva = "";
    let proprietario = ($(Controls.xProprietario).val() === "True");
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    //Carico ContoPatrimonialeAcquisto
    //Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietatio
    //Se flag proprietario = true passo parametri.Piva
    //Se flag proprietario = false passo objParametriAgenda.Piva
    if (proprietario === true)
        Piva = $(Controls.xPiva).val();
    else
        Piva = $(Controls.xPiva_Corrente).val();

    //Se il prodotto è una Materia_Prima allora leggo i valori come se fossi proprietario,
    //ma dopo rimangono in sola lettura.
    if (IsMateriaPrima(elem_cod) === true)
        Piva = $(Controls.xPiva).val();

    var param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: Piva,
        anno: new Date().getFullYear(),
        ricCodPat: 2,
        tipoDareAvere: "A",
        codContoPat: 0,
        imputabile: 1
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Contab/Conti.asmx/WS_Leggi_ContiPatrimoniali",
        param, false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            var objVuoto = { "Cod_Conto_Pat": -1, "Descr_Conto_Pat": "" };
            risp.unshift(objVuoto);
            options.success(risp);
        },
        null);
}

function Carica_prodotto_UC_ddlContoPatrimonialeVendita(options) {

    let Piva = "";
    let proprietario = ($(Controls.xProprietario).val() === "True");
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    //Carico ContoPatrimonialeVendita
    //Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietatio
    //Se flag proprietario = true passo parametri.Piva
    //Se flag proprietario = false passo objParametriAgenda.Piva
    if (proprietario === true)
        Piva = $(Controls.xPiva).val();
    else
        Piva = $(Controls.xPiva_Corrente).val();

    //Se il prodotto è una Materia_Prima allora leggo i valori come se fossi proprietario,
    //ma dopo rimangono in sola lettura.
    if (IsMateriaPrima(elem_cod) === true)
        Piva = $(Controls.xPiva).val();

    options.success(RicercaContoPatrimoniale(true, Piva, new Date().getFullYear(), "D", false));

}

function Carica_prodotto_UC_ddlGruppoMerce(options) {

    let Piva = "";
    let proprietario = ($(Controls.xProprietario).val() === "True");
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    //Carico GruppoMerce
    //Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietatio
    //Se flag proprietario = true passo parametri.Piva
    //Se flag proprietario = false passo objParametriAgenda.Piva
    if (proprietario === true)
        Piva = $(Controls.xPiva).val();
    else
        Piva = $(Controls.xPiva_Corrente).val();

    //Se il prodotto è una Materia_Prima allora leggo i valori come se fossi proprietario,
    //ma dopo rimangono in sola lettura.
    if (IsMateriaPrima(elem_cod) === true)
        Piva = $(Controls.xPiva).val();

    var param = kendo.stringify({
        piva: Piva
    });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_Gruppi_Merce",
        param, false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            var objVuoto = { "Id_Gruppo_Merce": 0, "Descrizione_Concatenata": "" };
            risp.unshift(objVuoto);
            options.success(risp);
        },
        null);

}

function NuovoCodiceArticolo() {
    var Cod_Articolo = 0;
    var param = kendo.stringify({
        piva: $(Controls.xPiva).val()
    });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_LastCod_Articolo",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            if (risp[0].Codice !== undefined && risp[0].Codice !== null && isNaN(risp[0].Codice)===false)
                Cod_Articolo = parseInt(risp[0].Codice) + 1;
            else
                Cod_Articolo = 0;
        },
        null);
    return Cod_Articolo;
}

function Carica_ddl_udm_aspetto(options) {
    let ElencoUnitaMisuraBene = [];
    let elem_cod = parseInt($(Controls.xElem_Cod).val());

    //LE UNICHE UDM EXTRA VALIDE SONO KG E LITRI
    //(per i beni di confezionamento carico solo i kg mentre per gli altri le carico tutte e due)
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlUnitaMisuraBene",
        "{}", false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            for (var x = 0; x < risp.length; x++) {
                if (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) {
                    if (risp[x].UDM_COD === 2) {
                        ElencoUnitaMisuraBene.push({ UDM_COD: risp[x].UDM_COD, UDM_DES: risp[x].UDM_DES });
                    }
                }
                else {
                    if (risp[x].UDM_COD === 2 || risp[x].UDM_COD === 29) {
                        ElencoUnitaMisuraBene.push({ UDM_COD: risp[x].UDM_COD, UDM_DES: risp[x].UDM_DES });
                    }
                }
            }
        },
        null);
    options.success(ElencoUnitaMisuraBene);
}

function Carica_ddl_set(options) {
    let ElencoSet = [];
    ElencoSet.push({ peso_set_cod: 0, peso_set_des: TraduzioneMultiResx(resxProdottoEditUC, "PesoNominaleNonModificabile", "Peso Nominale Non Modificabile") });
    ElencoSet.push({ peso_set_cod: 1, peso_set_des: TraduzioneMultiResx(resxProdottoEditUC, "PesoNominaleModificabile", "Peso Nominale Modificabile") });
    ElencoSet.push({ peso_set_cod: 2, peso_set_des: TraduzioneMultiResx(resxProdottoEditUC, "PesoModificabileEConsentiSfuso", "Peso Modificabile e Consenti Sfuso") });

    options.success(ElencoSet);
}

function Carica_ddl_peso(options) {
    let ElencoPeso = [];
    let elem_cod = parseInt($(Controls.xElem_Cod).val());

    if (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) {
        ElencoPeso.push({ tipo_peso_cod: enum_TipoPeso.Peso_Netto, tipo_peso_des: TraduzioneMultiResx(resxProdottoEditUC, "Capacità", "Capacità:") });
    }
    else {
        ElencoPeso.push({ tipo_peso_cod: enum_TipoPeso.Peso_Netto, tipo_peso_des: TraduzioneMultiResx(resxProdottoEditUC, "PesoNettoNominaleKg", "Peso Netto Nominale Kg:") });
        ElencoPeso.push({ tipo_peso_cod: -1, tipo_peso_des: TraduzioneMultiResx(resxProdottoEditUC, "CapacitàNominaleLitri", "Capacità Nominale Litri:") });
    }

    options.success(ElencoPeso);
}

function Carica_ddl_Confezione_Base(options) {
    let ElencoConfezioneBase = [];

    ElencoConfezioneBase.push({ base_cod: enum_OmniTabelle.ot_NESSUNO, base_des: TraduzioneMultiResx(resxProdottoEditUC, "NessunAutomatismo", "Nessun Automatismo") });
    ElencoConfezioneBase.push({ base_cod: enum_OmniTabelle.ot_CONFEZIONI_FF, base_des: TraduzioneMultiResx(resxProdottoEditUC, "Confezione", "Confezione") });
    ElencoConfezioneBase.push({ base_cod: enum_OmniTabelle.ot_CONTENITORI_FF, base_des: TraduzioneMultiResx(resxProdottoEditUC, "Contenitore", "Contenitore") });
    ElencoConfezioneBase.push({ base_cod: enum_OmniTabelle.ot_IMBALLAGGI_FF, base_des: TraduzioneMultiResx(resxProdottoEditUC, "Imballaggio", "Imballaggio") });

    options.success(ElencoConfezioneBase);
}


function Carica_ddl_confezioni(options) {
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlConfezioni",
        "{}", false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            var objVuoto = { "Codice": "", "Descrizione": "" };
            risp.unshift(objVuoto);
            options.success(risp);
        },
        null);
}

function Leggi_Modulo_Anagrafe() {
    let rispo = [];
    let param = kendo.stringify({
        piva: $(Controls.xPiva).val()
    });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_Modulo_Anagrafe",
        param, false,
        function (risposta) {

            let risp = JSON.parse(risposta.RispostaStringa);

            if (risp !== undefined && risp !== null) {

                rispo = risp;

            }
        },
        null);
    return rispo;
}

function Carica_Tipo_Default(options) {

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlTipo_Default",
        "{}", false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            var objVuoto = { "Codice_Generazione": 0, "Descrizione": TraduzioneMultiResx(resxProdottoEditUC, "Altro", "Altro") };
            risp.unshift(objVuoto);
            options.success(risp);
        },
        null);
}


function Default_ParametriQualitativi_Dati_Tecnici(cal_cod) {
    let ElencoParametriQualitativi = null;
    let param = kendo.stringify({
        cal_cod: cal_cod
    });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Default_ParametriQualitativi_Dati_Tecnici",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            ElencoParametriQualitativi = risp;
        },
        null);

    return ElencoParametriQualitativi;
}

function ParamQualGestitiPerCheckBoxBeniConf(Piva,indirizzohttp) {
    let param = kendo.stringify({
        piva: Piva
    });

    ajaxAgronicaSync(indirizzohttp + "/ParametroQualGestitiXCheckBoxBeniConf",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            ParametriQualitativiGestitiPerCheckBoxBeniConf = risp;
        },
        null);

    return ParametriQualitativiGestitiPerCheckBoxBeniConf;
}

function RiempiMultiseSpecieDettagliBeniConf(options) {
    let elencoSpecie = RicercaSpecie($(cIdPiva).val());
    if (elencoSpecie !== null && elencoSpecie !== undefined) {
        let index = null;
        for (var x = 0; x < elencoSpecie.length; x++) {
            if (elencoSpecie[x].Veg_Des === "") {
                index = x;
                break;
            }
        }

        if (index !== null) {
            //Aggiungo nelle multiselect per il caso vuoto il messaggio "Tutte le specie Vegetali"
            elencoSpecie[index].Veg_Des = TraduzioneMultiResx(resxProdottoEditUC, "TutteLeSpecieVegetali", "Tutte le Specie Vegetali");
        }
        options.success(elencoSpecie);
    }
}

function RiempiMultiseVarietaDettagliBeniConf(options) {
    var multisel = KendoMultisel("multisel_prodotto_UC_varieta_dett_beni");
    var veg_cod = multisel.dataSource.options.transport.data.Veg_Cod;
    var elencoVarieta = RicercaVarieta($(cIdPiva).val(), veg_cod);

    if (elencoVarieta !== null && elencoVarieta !== undefined) {
        let index = null;
        for (var x = 0; x < elencoVarieta.length; x++) {
            if (elencoVarieta[x].Cul_Des === "") {
                index = x;
                break;
            }
        }
        if (index !== null) {
            //Aggiungo nelle multiselect per il caso vuoto il messaggio "Tutte le Varietà Colturali"
            elencoVarieta[index].Cul_Des = TraduzioneMultiResx(resxProdottoEditUC, "TutteLeVarietaColturali", "Tutte le Varietà Colturali");
        }
        options.success(elencoVarieta);
    }
}


function Carica_ddlProduzionePropria(options) {
    let ElencoProduzioni = [];

    ElencoProduzioni.push({ Produzione_Cod: enum_Produzione_Cod.Prodotto_Commercializzato, Produzione_Des: TraduzioneMultiResx(resxProdottoEditUC, "ProdottoCommercializzato", "Prodotto Commercializzato") });
    ElencoProduzioni.push({ Produzione_Cod: enum_Produzione_Cod.Produzione_Propria, Produzione_Des: TraduzioneMultiResx(resxProdottoEditUC, "ProduzionePropria", "Produzione Propria") });

    options.success(ElencoProduzioni);
}

function Carica_ddlPesoEgalizzato(options) {
    let ElencoPesi = [];

    ElencoPesi.push({ Peso_Cod: -1, Peso_Des: "" });
    ElencoPesi.push({ Peso_Cod: 1, Peso_Des: TraduzioneMultiResx(resxProdottoEditUC, "Si", "Sì") });
    ElencoPesi.push({ Peso_Cod: 0, Peso_Des: TraduzioneMultiResx(resxProdottoEditUC, "No", "No") });

    options.success(ElencoPesi);
}


function RicercaFiltriBeniConfezVeg(tabella_cod) {
    var filtro = null;


    //Se sto duplicando un prodotto aggiungo le righe lette dal prodtto duplicato come righe nuove del nuovo prodotto
    if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Duplica &&
        tabProdotto_Edit_UC_DaDuplicare !== undefined && tabProdotto_Edit_UC_DaDuplicare !== null && tabProdotto_Edit_UC_DaDuplicare.length > 0 &&
        tabProdotto_Edit_UC_DaDuplicare.find(o => o.tab === "a_tab_prodotto_UC_configurazione")) {

        for (var x = 0; x < tabProdotto_Edit_UC_DaDuplicare.length; x++) {
            if (tabProdotto_Edit_UC_DaDuplicare[x].tab === "a_tab_prodotto_UC_configurazione") {
                filtro = JSON.parse(tabProdotto_Edit_UC_DaDuplicare[x].dati);
                break;
            }
        }
    }
    else {
        var param = kendo.stringify({
            piva: $(Controls.xPiva).val(),
            mat_Cod: parseInt($(Controls.xMat_Cod).val()),
            tabella_Cod: tabella_cod
        });

        ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/RicercaFiltriBeniConfezVeg",
            param, false,
            function (risposta) {
                filtro = JSON.parse(risposta.RispostaStringa);
            },
            null);
    }

    return filtro;
}


function Imposta_Default_UDM_GridStoricoPrezzi() {
    var udm_cod_default = 0;
    var param = kendo.stringify({
        elem_cod: $(Controls.xElem_Cod).val(),
        Cod_BancheDati: $(Controls.xPro_Cod).val()
    });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto +"/Imposta_Default_UDM_GridStoricoPrezzi",
        param, false,
        function (risposta) {
            udm_cod_default = parseInt(risposta.RispostaStringa);
        },null);

    return udm_cod_default;
}


function Controlli_Cancella_Materia_Prima(Elem_cod,Mat_cod) {
    let prodotto = {
        CancellaOk: false,
        MsgErrore: ""
    };

    var param = kendo.stringify({
        elem_cod:parseInt(Elem_cod),
        mat_cod: parseInt(Mat_cod)
    });

    ajaxAgronicaSync("../Anagrafica/Prodotto_Edit.aspx/Controlli_Cancella_Materia_Prima",
        param,false,
        function (risposta) {
            prodotto.CancellaOk = risposta.RispostaOK;
        },
        function (risposta) {
            prodotto.CancellaOk = risposta.RispostaOK;
            prodotto.MsgErrore = risposta.RispostaStringa;
        });

    return prodotto;
}


function Cambia_Codice_ArticoloDaCodice_Esterno_InBaseAFlag() {

    let risp = false;

    let param = kendo.stringify({
        piva: $(Controls.xPiva).val()
    });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Cambia_Codice_Articolo_Codice_Esterno_InBaseAFlag",
        param, false,
        function (risposta) {
            if (risposta.RispostaStringa === "true" || risposta.RispostaStringa === true) {
                risp = true;
            }
        }, null);

    return risp;
}

function CaricaProdottiXGrigliaAlias(options) {

    if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Scrittura) {
        options.success([]);
    }
    else {
        //Se sto duplicando un prodotto aggiungo le righe lette dal prodtto duplicato come righe nuove del nuovo prodotto
        if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Duplica && 
            tabProdotto_Edit_UC_DaDuplicare !== undefined && tabProdotto_Edit_UC_DaDuplicare !== null && tabProdotto_Edit_UC_DaDuplicare.length > 0 &&
            tabProdotto_Edit_UC_DaDuplicare.find(o => o.tab === "a_tab_prodotto_UC_alias")) {

            for (var x = 0; x < tabProdotto_Edit_UC_DaDuplicare.length; x++) {
                if (tabProdotto_Edit_UC_DaDuplicare[x].tab === "a_tab_prodotto_UC_alias") {

                    let dati = JSON.parse(tabProdotto_Edit_UC_DaDuplicare[x].dati);

                    let ds = Imposta_ds_grid_Alias_XMultiselect(dati);

                    options.success(ds);
                    break;
                }
            }

            let grid = $("#prodotto_UC_griglia_alias").data("kendoGrid");
            let currentData = grid.dataSource.data();

            for (let i = 0; i < currentData.length; i++) {
                currentData[i].id = 0;
            }
        }
        else {

            var param = kendo.stringify({
                piva: $(Controls.xPiva).val(),
                mat_cod: $(Controls.xMat_Cod).val(),
                elem_cod: $(Controls.xElem_Cod).val(),
                sa_cod: $(xSa_Cod).val()
            });


            ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/CaricaGrigliaAlias",
                param, false,
                function (risposta) {

                    var risp = JSON.parse(risposta.RispostaStringa);

                    var ds = Imposta_ds_grid_Alias_XMultiselect(risp);

                    options.success(ds);

                }, null);
        }
    }  

}

function Carica_Contatti() {

    var ElencoContatti;

    var param = kendo.stringify({
        piva: $(Controls.xPiva).val()
    });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_Contatti_Clienti_Fornitori",
        param,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);

            ElencoContatti = risp;
        },
        null);

    return ElencoContatti;
}


function Carica_ddl_griglia_alias() {

    let elenco_alias = null;

    //Da ricaricare ogni volta in base al cambiamento del sa_cod impostato
    let Sa_Cod = 0;
    var pubblico = getKendoSwitch("cb_prodotto_UC_materia_prima");
    if (pubblico)
        Sa_Cod = -1;

    var param = kendo.stringify({
        piva: $(Controls.xPiva).val(),
        elem_cod: parseInt($(Controls.xElem_Cod).val()),
        sa_cod: Sa_Cod,
    });


    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Leggi_XddlAlias",
        param, false,
        function (risposta) {

            var risp = JSON.parse(risposta.RispostaStringa);

            let objVuoto = {
                "Mat_Cod_Alias": 0,
                "Mat_Des_Alias": ""
            };

            risp.unshift(objVuoto);

            elenco_alias = risp;

        }, null);

    return elenco_alias;

}

function Controlla_Se_Alias_Associato_a_Materia_Prima(){

    var param = kendo.stringify({
        piva: $(Controls.xPiva).val(),
        mat_cod_alias: $(Controls.xMat_Cod).val(),
    });


    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Controlla_Se_Alias_Associato_a_Materia_Prima",
        param, false,
        function (risposta) {

            Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima = (risposta.RispostaStringa == "True");

        }, null);

}

function ComponiMessaggioDiAvvisoCancellazioneProdotto(Piva, Elem_cod, Mat_cod, Sa_cod, isAlias) {

    var Messaggio = "";

    var param = kendo.stringify({
        piva: Piva,
        elem_cod: Elem_cod,
        mat_cod: Mat_cod,
        sa_cod: Sa_cod,
        isalias: isAlias
    });


    ajaxAgronicaSync("../Anagrafica/Prodotto_Edit.aspx/ComponiMessaggioDiAvvisoCancellazioneProdotto",
        param, false,
        function (risposta) {
            var risp = risposta.RispostaStringa;

            Messaggio = risp;

        }, null);

    return Messaggio;
}

function Controlla_Se_Prodotto_Movimentato(Elem_cod, Mat_cod, Pro_cod, leggiXGridRicerca, IsAlias) {

    let movi = false;

    Movimentazioni_Prodotto = Leggi_Dettagli_Prodotto(Elem_cod, Mat_cod, Pro_cod, leggiXGridRicerca, IsAlias);

    if (Movimentazioni_Prodotto !== null && Movimentazioni_Prodotto.length > 0) {
        movi = true;
    }

    return movi;

}

function Leggi_Dettagli_Prodotto(Elem_cod, Mat_cod, Pro_cod, leggiXGridRicerca, IsAlias) {

    let indirizzo = indirizzohttpPaginaProdotto;

    if (leggiXGridRicerca === true)
        indirizzo = "../Anagrafica/Prodotto_Edit.aspx";

    var prodotti = null;

    //Se sto cercando di eliminare un alias allora devo passare il mat_cod_alias,
    //per controllare prima se è movimentato
    var Mat_cod_Alias = 0;

    if (IsAlias === true) {
        Mat_cod_Alias = Mat_cod;
        Mat_cod = 0;
    }


    var param = kendo.stringify({
        elem_cod: Elem_cod,
        mat_cod: Mat_cod,
        pro_cod: Pro_cod,
        mat_cod_alias: Mat_cod_Alias
    });


    ajaxAgronicaSync(indirizzo + "/Controlla_Se_Prodotto_Movimentato",
        param, false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);

            prodotti = risp;

        }, null);

    return prodotti;
}

function submit_Effettivo() {

    let valoreRitorno =
        {
            Errori: false,
            Mat_Cod: 0,
            MessaggioErrore: ""
        };

    var tipoOperazioneDB = parseInt($(Controls.xTipoOperazione).val());

    var controlliPrimaDelSubmitOk = false;
    var erroriNelSubmit = "";
    var elem_cod = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_categ_prod"));
    var mat_cod = 0;

    if (tipoOperazioneDB !== enum_tipoOperazione.Duplica)
        mat_cod = $(Controls.xMat_Cod).val();

    var isMateriaPrima = IsMateriaPrima(elem_cod);
    var piva = $(Controls.xPiva).val();
    if (elem_cod === categorieProdotti.SEMENTI)
        isMateriaPrima = true;


    var grid_Prezzi = $("#prodotto_UC_griglia_storico_prezzi").data("kendoGrid");


    var salvaModel = new Object();
    salvaModel.Pro_Cod = parseInt($(Controls.xPro_Cod).val());
    salvaModel.IsMateriaPrima = isMateriaPrima;
    salvaModel.Mat_Cod = mat_cod;
    salvaModel.TipoOperazioneDB = tipoOperazioneDB;
    salvaModel.Piva = piva;
    salvaModel.Cal_Cod = 0;
    salvaModel.Proprietario = ($(Controls.xProprietario).val() === "True");
    salvaModel.CategoriaProdotto = elem_cod;
    salvaModel.Codice_Esterno = $("#txt_prodotto_UC_cod_est").val();

    salvaModel.Flag_Importato = 1;

    if (Prodotto_Edit_UC_Importato===false)
        salvaModel.Flag_Importato = 0;

    salvaModel.Componi_Cod_ArticoloDaCodice_Esterno = "True";

    if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === false)
        salvaModel.Componi_Cod_ArticoloDaCodice_Esterno = "False";

    salvaModel.IsModulo_FF = "False";

    if (Modulo_FF === true)
        salvaModel.IsModulo_FF = "True";

    salvaModel.ChkReferenza = 0;

    if (Prodotto_Materia_Prima_Letto !== null) {
        if (Prodotto_Materia_Prima_Letto.ChkReferenza !== undefined &&
            Prodotto_Materia_Prima_Letto.ChkReferenza !== null &&
            Prodotto_Materia_Prima_Letto.ChkReferenza !== "") {

            salvaModel.ChkReferenza = Prodotto_Materia_Prima_Letto.ChkReferenza;
        }
    }

    salvaModel.ChkAlias = 0;
    var is_alias = getKendoSwitch("cb_prodotto_UC_alias");
    if (is_alias)
        salvaModel.ChkAlias = 1;

    //Tab Configurazione
    var configurazioneModel = null;

    //Tab Traduzione
    var traduzioniModel = null;

    //Tab Parametri Qualitativi
    var parametriqualitativicalibriModel = null;
    var parametriqualitativindiciModel = null;

    //Tab Dati Tecnici
    var datiTecniciModel = null;

    //Opzioni Materia Prima
    var opzioniModel = null;

    //Parametri Qualitativi relativi alla referenza
    var ParametriQualitativiDatiTecniciModel = null;
    if (isMateriaPrima === true) {
        var grid_Traduzioni = $("#prodotto_UC_griglia_traduzioni").data("kendoGrid");
        var grid_Calibri = $("#prodotto_UC_griglia_calibri").data("kendoGrid");
        var grid_Alias = $("#prodotto_UC_griglia_alias").data("kendoGrid");

        //Se sono in modifica posso cambiare anche il codice articolo,
        //quindi valorizzo un altra variabile con il nuovo codice articolo.
        if (tipoOperazioneDB === enum_tipoOperazione.Modifica) {
            salvaModel.CodiceProdotto = Codice_Articolo_Originale;
            //Vuol dire che non è stato cambiato il codice articolo e quindi lo passo vuoto
            if (Codice_Articolo_Originale === $("#txt_prodotto_UC_cod_prod").val()) {
                salvaModel.NuovoCodiceProdotto = "";
            }
            else {
                salvaModel.NuovoCodiceProdotto = $("#txt_prodotto_UC_cod_prod").val();
            }

        } else {
            salvaModel.CodiceProdotto = $("#txt_prodotto_UC_cod_prod").val();
            salvaModel.NuovoCodiceProdotto = "";
        }

        salvaModel.Descrizione = $("#txt_prodotto_UC_Descrizione").val();
        salvaModel.Categoria_Risorsa = Get_KendoDDLValue("ddl_prodotto_UC_categ_ris");
        salvaModel.Linea_Cod = Get_KendoDDLValue("ddl_prodotto_UC_linea_prod");


        if (Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== undefined) {
            salvaModel.Cal_Cod = Prodotto_Materia_Prima_Letto.Cal_Cod;
        }

        //Se Prodotto_Edit_UC_Importato = true non salvo la tab Configurazione
        if (Prodotto_Edit_UC_Importato === false) {
            var Flag_EscludiMagazzino = $("#chk_prodotto_UC_escludi_da_magazzino").is(':checked');
            //var Flag_EscludiPreparazione = $("#chk_prodotto_UC_escludi_da_preparazioni").is(':checked');
            var Flag_Udm_Cod_Extra = $("#chk_prodotto_UC_Udm_Cod_Extra").is(':checked');
            var Flag_Extra = $("#chk_prodotto_UC_imposta_flag_extra").is(':checked');
            var Flag_Imballaggio = $("#chk_prodotto_UC_imballaggio").is(':checked');
            var Flag_Contenitore = $("#chk_prodotto_UC_contenitore").is(':checked');
            var Flag_Contenitore_Conf = $("#chk_prodotto_UC_Qta_Contenitore_Conf").is(':checked');
            var Flag_Variazione = $("#chk_prodotto_UC_imposta_flag_Variazione").is(':checked');
            var Flag_Confezione = $("#chk_prodotto_UC_confezione").is(':checked');
            var Flag_CompLotto = $("#chk_prodotto_UC_composizioneLotto").is(':checked');
            var Flag_UtilizzoCA = $("#chk_prodotto_UC_utilizzoCA").is(':checked');
        }


        //Se Prodotto_Edit_UC_Importato = true non salvo la tab Configurazione
        if (tabStripAperti.includes("a_tab_prodotto_UC_configurazione") && Prodotto_Edit_UC_Importato===false) {
            configurazioneModel = new Object();

            let Qta_Extra = 0;
            let Tara = 0;
            let Contenitore = 0;
            let Contenitore_Conf = 0;

            configurazioneModel.Flag_EscludiMagazzino = Flag_EscludiMagazzino ? 1 : 0;
            configurazioneModel.Flag_EscludiPreparazione = 0;
            //configurazioneModel.Flag_EscludiPreparazione = Flag_EscludiPreparazione ? 1 : 0;
            configurazioneModel.Flag_Variazione = Flag_Variazione ? 1 : 0;
            configurazioneModel.Flag_CompLotto = Flag_CompLotto ? 1 : 0;
            configurazioneModel.Flag_UtilizzoCA = Flag_UtilizzoCA ? 1 : 0;
            configurazioneModel.Flag_Udm_Cod_Extra = Flag_Udm_Cod_Extra ? 1 : 0;
            configurazioneModel.Flag_Extra = Flag_Extra ? 1 : 0;
            configurazioneModel.Flag_Imballaggio = Flag_Imballaggio ? 1 : 0;
            configurazioneModel.Confezione_Cod = Get_KendoDDLValue("ddl_prodotto_UC_confezioni");
            configurazioneModel.Udm_Cod_Extra = Get_KendoDDLValue("ddl_prodotto_UC_cod_extra");

            if (Get_KendoNumTBValue("txt_prodotto_UC_Qta_Extra") !== undefined &&
                Get_KendoNumTBValue("txt_prodotto_UC_Qta_Extra") !== null &&
                Get_KendoNumTBValue("txt_prodotto_UC_Qta_Extra") !== "")
                Qta_Extra = Get_KendoNumTBValue("txt_prodotto_UC_Qta_Extra");

            configurazioneModel.Qta_Extra = Qta_Extra;

            if (Get_KendoNumTBValue("txt_prodotto_UC_tara_nomi") !== undefined &&
                Get_KendoNumTBValue("txt_prodotto_UC_tara_nomi") !== null &&
                Get_KendoNumTBValue("txt_prodotto_UC_tara_nomi") !== "")
                Tara = Get_KendoNumTBValue("txt_prodotto_UC_tara_nomi");

            configurazioneModel.Tara = Tara;

            configurazioneModel.Flag_Contenitore_Conf = Flag_Contenitore_Conf ? 1 : 0;
            configurazioneModel.Flag_Contenitore = Flag_Contenitore ? 1 : 0;
            configurazioneModel.Flag_Confezione = Flag_Confezione ? 1 : 0;

            if (Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore") !== undefined &&
                Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore") !== null &&
                Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore") !== "")
                Contenitore = Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore");

            configurazioneModel.Contenitore = Contenitore;

            if (Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore_Conf") !== undefined &&
                Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore_Conf") !== null &&
                Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore_Conf") !== "")
                Contenitore_Conf = Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore_Conf");

            configurazioneModel.Contenitore_Conf = Contenitore_Conf;

            configurazioneModel.TipoPeso = Get_KendoDDLValue("ddl_prodotto_UC_peso");
            configurazioneModel.Peso_Set = Get_KendoDDLValue("ddl_prodotto_UC_set");
            if (elem_cod === categorieProdotti.TRASFORMATI_VEGETALI && Modulo_FF === true)
                configurazioneModel.Otabella_Cod_Base = Get_KendoDDLValue("ddl_prodotto_UC_Confezione_Base");
            //Filtri che vengono salvati su Otabelle_Parametri
            configurazioneModel.Filtro_Veg_Cod = "";
            configurazioneModel.Filtro_Cul_Cod = "";

            if (Modulo_FF === true && KendoMultisel("multisel_prodotto_UC_specie_dett_beni") !== undefined && KendoMultisel("multisel_prodotto_UC_specie_dett_beni") !== null &&
                (Flag_Imballaggio === true || Flag_Contenitore === true || Flag_Confezione === true)) {

                let multiSpecie = $("#multisel_prodotto_UC_specie_dett_beni").data("kendoMultiSelect");
                if (multiSpecie.value().length === 1 && multiSpecie.value()[0] !== "0" && multiSpecie.value()[0] !== "-1" && multiSpecie.value()[0] !== "") {
                    configurazioneModel.Filtro_Veg_Cod = "|" + multiSpecie.value()[0] + "|";
                }
                else {
                    if (multiSpecie.value().length === 1) {
                        if (multiSpecie.value()[0] === "-1") {
                            configurazioneModel.Filtro_Veg_Cod = "0";
                        }
                    }
                    else {
                        if (multiSpecie.value().includes("-1")) {
                            let filtroSpecie = "";
                            for (let x = 0; x < multiSpecie.value().length; x++) {
                                if (multiSpecie.value()[x] !== "-1")
                                    filtroSpecie += "|" + multiSpecie.value()[x];
                            }
                            configurazioneModel.Filtro_Veg_Cod = filtroSpecie+"|";
                        }
                        else {
                            if (Get_MultiselString("multisel_prodotto_UC_specie_dett_beni") !== undefined &&
                                Get_MultiselString("multisel_prodotto_UC_specie_dett_beni") !== null)
                                configurazioneModel.Filtro_Veg_Cod = "|" + Get_MultiselString("multisel_prodotto_UC_specie_dett_beni") + "|";
                            else
                                configurazioneModel.Filtro_Veg_Cod = "0";
                        }
                    }
                }

                if (KendoMultisel("multisel_prodotto_UC_varieta_dett_beni") !== undefined && KendoMultisel("multisel_prodotto_UC_varieta_dett_beni") !== null &&
                    (Flag_Imballaggio === true || Flag_Contenitore === true || Flag_Confezione === true)) {

                    let multiVarieta = $("#multisel_prodotto_UC_varieta_dett_beni").data("kendoMultiSelect");
                    configurazioneModel.Filtro_Cul_Cod = Get_MultiselString("multisel_prodotto_UC_varieta_dett_beni");
                    if (multiSpecie.value().length >= 1 && multiVarieta.value().length === 0) {
                        configurazioneModel.Filtro_Cul_Cod = "0";
                    }
                    if (multiVarieta.value().length === 1) {
                        if (multiVarieta.value()[0] === "0")
                            configurazioneModel.Filtro_Cul_Cod = "0";
                        else
                            configurazioneModel.Filtro_Cul_Cod = "|" + multiVarieta.value()[0] + "|";

                    }
                    else {
                        if (multiVarieta.value().includes("0")) {
                            let filtroVarieta = "";
                            for (let x = 0; x < multiVarieta.value().length; x++) {
                                if (multiVarieta.value()[x] !== "0")
                                    filtroVarieta += "|" + multiVarieta.value()[x];
                            }
                            configurazioneModel.Filtro_Cul_Cod = filtroVarieta + "|";
                        }
                        else {

                            if (Get_MultiselString("multisel_prodotto_UC_varieta_dett_beni") !== undefined &&
                                Get_MultiselString("multisel_prodotto_UC_varieta_dett_beni") !== null)
                                configurazioneModel.Filtro_Cul_Cod = "|" + Get_MultiselString("multisel_prodotto_UC_varieta_dett_beni") + "|";
                            else
                                configurazioneModel.Filtro_Cul_Cod = "0";
                        }
                    }
                }
            }

            //Salvo in una variabile il Parametro Qualitativo letto nel caso in cui sia stato gia scelto per cancellarlo da 
            //otabelle_parametri nel caso in cui Flag_Udm_cod_Extra sia 0.
            configurazioneModel.ParamQualOriginaleDettagliBeniConf = 0;
            if (Modulo_FF === true && parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Modifica) {
                if (Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== undefined &&
                    Prodotto_Materia_Prima_Letto !== "") {

                    if (Prodotto_Materia_Prima_Letto.ChkImballaggio > 0)
                        configurazioneModel.ParamQualOriginaleDettagliBeniConf = enum_OmniTabelle.ot_IMBALLAGGI_FF;

                    if (Prodotto_Materia_Prima_Letto.ChkContenitore > 0)
                        configurazioneModel.ParamQualOriginaleDettagliBeniConf = enum_OmniTabelle.ot_CONTENITORI_FF;

                }

                let Confezioni = RicercaValoriParametriQualitativi(5, $(cIdPiva).val(), false);
                if (Confezioni !== undefined && Confezioni !== null && Confezioni.length !== 0) {
                    configurazioneModel.ParamQualOriginaleDettagliBeniConf = enum_OmniTabelle.ot_CONFEZIONI_FF;
                }
            }
        }

        if (tabStripAperti.includes("a_tab_prodotto_UC_traduzioni")) {
            traduzioniModel = new Object();
            traduzioniModel.RigheInserite = righeInseriteGrid_Traduzioni;
            traduzioniModel.RigheModificate = righeModificateGrid_Traduzioni;
            traduzioniModel.RigheEliminate = righeEliminateGrid_Traduzioni;
            traduzioniModel.TutteLeRighe = righeTutteGrid_Traduzioni;
        }

        //Se Prodotto_Edit_UC_Importato = true non salvo la tab Parametri Qualitativi
        if (tabStripAperti.includes("a_tab_prodotto_UC_parametri_qualitativi") && Prodotto_Edit_UC_Importato === false) {
            parametriqualitativicalibriModel = new Object();
            parametriqualitativindiciModel = new Object();
            parametriqualitativicalibriModel.RigheScelte = righeSelezionateGrid_Calibri;
            parametriqualitativindiciModel.RigheScelte = righeSelezionateGrid_Indici;
        }


        datiTecniciModel = new Object();
        datiTecniciModel.Descrizione_Addizionale = $("#txt_prodotto_UC_descr_add").val();
        datiTecniciModel.Note = $("#txt_prodotto_UC_Note").val();
        datiTecniciModel.Regolamento = Get_KendoDDLValue("ddl_prodotto_UC_Regolamento");

        var tecnologiaSementi = null;
        var Germinabilita = null;
        if (elem_cod === categorieProdotti.SEMENTI) {
            datiTecniciModel.SementiEMaterialiVivaisti = Get_KendoDDLValue("ddl_prodotto_UC_sementi_materiale");

            let dd_TecnologieSementi = $('#ddl_prodotto_UC_TecnologieSementi').data("kendoDropDownList")
            if (dd_TecnologieSementi !== undefined) {
                tecnologiaSementi = dd_TecnologieSementi.value();
            }
            let txt_germinabilita = $('#txt_prodotto_UC_germinabilita').data("kendoNumericTextBox");
            if (txt_germinabilita !== undefined) {
                Germinabilita = txt_germinabilita.value() / 100;
            }
        }
        else {
            datiTecniciModel.SementiEMaterialiVivaisti = null;
        }

        datiTecniciModel.Cod_TecnologiaSementi = tecnologiaSementi;
        datiTecniciModel.Germinabilita = Germinabilita;


        datiTecniciModel.SpecieVegetali = Get_KendoDDLValue("ddl_prodotto_UC_specie_veg");
        datiTecniciModel.VarietaColturale = Get_KendoDDLValue("ddl_prodotto_UC_varieta");
        datiTecniciModel.TipologiaVarietale = Get_KendoDDLValue("ddl_prodotto_UC_tipo_varietale");
        datiTecniciModel.Sa_Cod = 0;
        var pubblico = getKendoSwitch("cb_prodotto_UC_materia_prima");
        if (pubblico)
            datiTecniciModel.Sa_Cod = -1;
        datiTecniciModel.DittaDiProvenienza = Get_KendoDDLValue("ddl_prodotto_UC_ditta_di_provenienza");

        if (KendoDDL("ddl_prodotto_UC_Prodottobase") !== undefined && KendoDDL("ddl_prodotto_UC_Prodottobase") !== null) {
            if (Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase") !== "")
                datiTecniciModel.Referenza = Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase");
            else
                datiTecniciModel.Referenza = 0;
        } else {
            datiTecniciModel.Referenza = 0;
        }

        datiTecniciModel.Finalita_Produttiva = Get_KendoDDLValue("ddl_prodotto_UC_final_prod");
        datiTecniciModel.UDM_Cod = Get_KendoDDLValue("ddl_prodotto_UC_unita_mis_def");

        var Flag_Biologico = $("#chk_prodotto_UC_agricoltura_biologica").is(':checked');
        var Flag_Convenzionale = $("#chk_prodotto_UC_agricoltura_convezionale").is(':checked');
        var Flag_Non_Agricolo = $("#chk_prodotto_UC_origine_non_agri").is(':checked');
        var Flag_Ausiliare_Fabbricazione = $("#chk_prodotto_UC_ausi_fabbr").is(':checked');

        if (elem_cod === categorieProdotti.SEMILAVORATI_PRODUZIONE_ANIMALE || elem_cod === categorieProdotti.TRASFORMATI_ANIMALI) {
            datiTecniciModel.SpecieAnimali = Get_KendoDDLValue("ddl_prodotto_UC_specie_anim");
            datiTecniciModel.IndirizzoAnimale = Get_KendoDDLValue("ddl_prodotto_UC_indi_produt");
            datiTecniciModel.RazzaAnimale = Get_KendoDDLValue("ddl_prodotto_UC_razza_anim");
        }
        else {
            datiTecniciModel.SpecieAnimali = null;
            datiTecniciModel.IndirizzoAnimale = null;
            datiTecniciModel.RazzaAnimale = null;
        }

        datiTecniciModel.Flag_Biologico = Flag_Biologico ? 1 : 0;
        datiTecniciModel.Flag_Convenzionale = Flag_Convenzionale ? 1 : 0;
        datiTecniciModel.Flag_Non_Agricolo = Flag_Non_Agricolo ? 1 : 0;
        datiTecniciModel.Flag_Ausiliare_Fabbricazione = Flag_Ausiliare_Fabbricazione ? 1 : 0;
        datiTecniciModel.PrioritaCdG = Get_KendoDDLValue("ddl_prodotto_UC_mat_prima_priorita_cdg");

        opzioniModel = new Object();
        opzioniModel.ConsentiCodiciDuplicati = false;
        opzioniModel.CodiceMaxLenght = 0;

        var optAmmettiCodiciDuplicati = Ottieni_Opzione_Materia_Prima(opzione_codice_duplicato);
        if (optAmmettiCodiciDuplicati != "" && optAmmettiCodiciDuplicati != undefined && optAmmettiCodiciDuplicati != null)
            opzioniModel.ConsentiCodiciDuplicati = optAmmettiCodiciDuplicati === "1" ? true : false;

        var optCodiceMaxLength = Ottieni_Opzione_Materia_Prima(opzione_codice_max_length);
        if (optCodiceMaxLength != "" && optCodiceMaxLength != undefined && optCodiceMaxLength != null)
            opzioniModel.CodiceMaxLenght = parseInt(optCodiceMaxLength);

        //Salvataggio Parametri Qualitativi - Dati Tecnici
        //Per cancellare o modificare il prodotto base di riferimento di un prodotto non deve essere movimentato
        //altrimenti posso solo modificare i parametri di quello selezionato all'inizio,

        ParametriQualitativiDatiTecniciModel = new Object();
        let paramQual = AggiungiValoriParametriQualitativi();
        var elimina = "False";
        let cal_cod = 0;
        let mat_cod_ref = 0;


        if (paramQual !== null && paramQual !== undefined) {

            if (Modulo_FF === true && parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.TRASFORMATI_VEGETALI && Is_OMNI === false) {
                if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura) {
                    if (Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== undefined) {
                        cal_cod = parseInt(Prodotto_Materia_Prima_Letto.Cal_Cod);
                    }
                    if (cal_cod !== 0) {

                        if (Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== undefined) {
                            mat_cod_ref = parseInt(Prodotto_Materia_Prima_Letto.Mat_Cod_Referenza);
                        }

                        if (mat_cod_ref !== parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase"))) {
                            if (Prodotto_Movimentato === true) {

                                let Msg_Errore = TraduzioneMultiResx(resxProdottoEditUC, "ProdottoBaseRiferimentoNonCambiabile", "Il Prodotto Base di Riferimento non può essere cambiato perchè il prodotto risulta già movimentato");

                                MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

                                valoreRitorno.Errori = true;
                                valoreRitorno.MessaggioErrore = Msg_Errore;
                                return valoreRitorno;

                            }
                        }
                    }
                }
            }          

            ParametriQualitativiDatiTecniciModel.Campi = kendoEscapeOggetto(paramQual);

        } else {
            //Se sono cambiate la specie e la varietà di un trasformato vegetale che aveva i parametri qualitativi e
            //ora sono state selezionate specie e varietà che non li hanno bisogna cancellare i parametri qualitativi
            //precedenti.

            if (Prodotto_Movimentato === false && Modulo_FF === true &&
                parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.TRASFORMATI_VEGETALI &&
                Prodotto_Edit_UC_Importato === false) {

                if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura &&
                    Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== undefined) {

                    cal_cod = parseInt(Prodotto_Materia_Prima_Letto.Cal_Cod);

                    let veg_cod_modificato = parseInt(datiTecniciModel.SpecieVegetali);
                    let cul_cod_modificato = parseInt(datiTecniciModel.VarietaColturale);

                    let veg_cod_letto = parseInt(Prodotto_Materia_Prima_Letto.Veg_Cod);
                    let cul_cod_letto = parseInt(Prodotto_Materia_Prima_Letto.Cul_Cod);

                    if (cal_cod !== 0 && veg_cod_modificato !== -1 && cul_cod_modificato !== 0) {

                        if (veg_cod_modificato !== veg_cod_letto &&
                            cul_cod_modificato !== cul_cod_letto) {

                            elimina = "True";
                        }
                    }

                }
            }
        }

        //Controllo che l'oggetto che contiene i datiTecnici non sia vuoto
        if (Object.keys(datiTecniciModel).length > 0 && datiTecniciModel.constructor === Object)
            datiTecniciModel.Elimina_Referenza_ParamQualDTec = elimina;
    }

    //Se Prodotto_Edit_UC_Importato = true non salvo la tab Storico Prezzi
    var prezziModel = null;
    if (tabStripAperti.includes("a_tab_prodotto_UC_storico_prezzi") && Prodotto_Edit_UC_Importato===false) {
        prezziModel = new Object();
        prezziModel.RigheInserite = righeInseriteGrid_Prezzi;
        prezziModel.RigheModificate = righeModificateGrid_Prezzi;
        prezziModel.RigheEliminate = righeEliminateGrid_Prezzi;
        prezziModel.TutteLeRighe = righeTutteGrid_Prezzi;
    }

    //Se è stata aperta la tab 'Dati Contabilità' carico anche i dati della tab 
    //'Dati Vendita Al Dettaglio' e viceversa così se erano stati salvati dei dati
    //in queste tab non riesco che vengano sovrascritti se non vengono aperti
    if (tabStripAperti.includes("a_tab_prodotto_UC_dati_contabilita")) {
        OnTabShow("a_tab_prodotto_UC_altri_dati");
    } else if (tabStripAperti.includes("a_tab_prodotto_UC_altri_dati")) {
        OnTabShow("a_tab_prodotto_UC_dati_contabilita");
    }

    var datiContabilitaModel = null;
    if (tabStripAperti.includes("a_tab_prodotto_UC_dati_contabilita")) {
        datiContabilitaModel = new Object();
        datiContabilitaModel.TipoOperazioneDB = xTipoOperazioneDatiCOntabiliAltriDati;
        datiContabilitaModel.Aliquota_Iva = Get_KendoDDLValue("prodotto_UC_ddlCodIva");
        datiContabilitaModel.Aliquota_Iva_Compensazione = Get_KendoDDLValue("prodotto_UC_ddlCodIvaCompensazione");
        datiContabilitaModel.Conto_Economico_Acquisto = Get_KendoDDLValue("prodotto_UC_ddlContoEconomicoAcquisto");
        datiContabilitaModel.Conto_Economico_Vendita = Get_KendoDDLValue("prodotto_UC_ddlContoEconomicoVendita");
        datiContabilitaModel.Conto_Patrimoniale_Acquisto = Get_KendoDDLValue("prodotto_UC_ddlContoPatrimonialeAcquisto");
        datiContabilitaModel.Conto_Patrimoniale_Vendita = Get_KendoDDLValue("prodotto_UC_ddlContoPatrimonialeVendita");
        datiContabilitaModel.Id_Gruppo_Merce = Get_KendoDDLValue("prodotto_UC_ddlGruppoMerce");
    }


    var altriDatiModel = null;
    if (tabStripAperti.includes("a_tab_prodotto_UC_altri_dati")) {
        altriDatiModel = new Object();
        altriDatiModel.TipoOperazioneDB = xTipoOperazioneDatiCOntabiliAltriDati;
        altriDatiModel.EAN = $("#txt_prodotto_UC_EAN").val();
        altriDatiModel.Barcode = $("#txt_prodotto_UC_Barcode").val();
        altriDatiModel.Peso_Netto = Get_KendoNumTBValue("txt_prodotto_UC_PesoNetto");
        altriDatiModel.Peso_Sgocciolato = Get_KendoNumTBValue("txt_prodotto_UC_PesoSgocciolato");
        altriDatiModel.Tara = Get_KendoNumTBValue("txt_prodotto_UC_Tara");
        altriDatiModel.Produzione_Propria = Get_KendoDDLValue("prodotto_UC_ddlProduzionePropria");
        altriDatiModel.Peso_Egalizzato = Get_KendoDDLValue("prodotto_UC_ddlPesoEgalizzato");
        altriDatiModel.Ingredienti = $("#txt_prodotto_UC_Ingredienti").val();

        altriDatiModel.Immagine_NomeFile = "";
        altriDatiModel.Immagine_Estensione = "";
        altriDatiModel.Immagine = "";

        if ($(xDatiImmagineCaricata).val() !== "") {

            let datiImmagine = JSON.parse($(xDatiImmagineCaricata).val());

            altriDatiModel.Immagine_NomeFile = datiImmagine.Nome;
            altriDatiModel.Immagine_Estensione = datiImmagine.Estensione;
            altriDatiModel.Immagine = btoa(datiImmagine.Immagine);
        }

    }

    var descrizioniAlternativeModel = null;
    if (tabStripAperti.includes("a_tab_prodotto_UC_alias")) {
        descrizioniAlternativeModel = new Object();
        descrizioniAlternativeModel.RigheInserite = righeInseriteGrid_Alias;
        descrizioniAlternativeModel.RigheModificate = righeModificateGrid_Alias;
        descrizioniAlternativeModel.RigheEliminate = righeEliminateGrid_Alias;
        descrizioniAlternativeModel.TutteLeRighe = righeTutteGrid_Alias;
    }

    salvaModel.DatiTecnici = datiTecniciModel;
    salvaModel.Configurazione = configurazioneModel;
    salvaModel.datiContabilita = datiContabilitaModel;
    salvaModel.StoricoPrezzi = prezziModel;
    salvaModel.Traduzioni = traduzioniModel;
    salvaModel.ParametriQualitativiCalibri = parametriqualitativicalibriModel;
    salvaModel.ParametriQualitativiIndici = parametriqualitativindiciModel;
    salvaModel.Opzioni = opzioniModel;
    salvaModel.ParametriQualitativiDatiTecnici = ParametriQualitativiDatiTecniciModel;
    salvaModel.AltriDati = altriDatiModel;
    salvaModel.DescrizioniAlternative = descrizioniAlternativeModel;

    var salvaModelEscaped = kendoEscapeOggetto(salvaModel);

    var importato = "True";

    if (Prodotto_Edit_UC_Importato === false)
        importato = "False";

    var param = "{model:'" + salvaModelEscaped + "' , importato:'" + importato+"'}";

    // Eseguo Check Preliminari
    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/ControlliPrimaDelSubmit",
        param, false,
        function (risposta) {
            controlliPrimaDelSubmitOk = true;
        },
        function (risposta) {
            erroriNelSubmit = risposta.RispostaStringa;
            MessaggioErrore_Bootstrap(erroriNelSubmit, "DIV_Messaggi");
            valoreRitorno.Errori = true;
            valoreRitorno.MessaggioErrore = erroriNelSubmit;
        });

    if (!controlliPrimaDelSubmitOk) {
        erroreSubmitGriglia(grid_Prezzi);
        erroreSubmitGriglia(grid_Traduzioni);
        erroreSubmitGriglia(grid_Alias);
    }

    if (controlliPrimaDelSubmitOk) {
        ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Submit",
            param, false,
            function (risposta) {
                valoreRitorno.Mat_Cod = risposta.RispostaStringa;
                RipristinaRigheInseriteModificateCancellateGriglia();
                kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "SalvataggioAvvenutoConSuccesso", "Salvataggio avvenuto con successo"));

            },
            function (risposta) {
                valoreRitorno.Errori = true;
                valoreRitorno.MessaggioErrore = risposta.RispostaStringa;
            });

        if (valoreRitorno.Errori) {
            MessaggioErrore_Bootstrap(valoreRitorno.MessaggioErrore, "DIV_Messaggi");
            // TODO Errore submit griglia
        }
    }

    return valoreRitorno;
}

function Link_Redirect_Prodotto(elem_cod) {

    let redirect_Url = "";

    let tipoOperazione = parseInt($(Controls.xTipoOperazione).val());

    //Da ricaricare ogni volta in base al cambiamento del sa_cod impostato
    let Sa_Cod = 0;
    var pubblico = getKendoSwitch("cb_prodotto_UC_materia_prima");
    if (pubblico)
        Sa_Cod = -1;

    let Duplica = false;

    if (tipoOperazione === enum_tipoOperazione.Duplica)
        Duplica = true;


    var param = kendo.stringify(
        {
            tipooperazione: tipoOperazione,
            piva: $(Controls.xPiva).val(),
            mat_cod: $(Controls.xMat_Cod).val(),
            elem_cod: elem_cod,
            prodotto_des: $(Controls.xProdotto_Des).val(),
            pro_cod: $(Controls.xPro_Cod).val(),
            duplica: Duplica,
            proprietario: $(Controls.xProprietario).val(),
            sa_cod: Sa_Cod,
            isalias: $(xIs_Alias).val(),
        });

    ajaxAgronicaSync(indirizzohttpPaginaProdotto + "/Redirect_Prodotto",
        param, false,
        function (risposta) {
            redirect_Url = risposta.RispostaStringa;
            if (winProdotto) redirect_Url += "&win=1";
            let urlSearch = new URLSearchParams(window.location.href);
            if (urlSearch.has("visibilita") && !redirect_Url.includes("visibilita")) {
                redirect_Url += "&visibilita=" + urlSearch.get("visibilita")
            }
        }, null);

    return redirect_Url;
}


function Cancella_Prodotto_Effettivo(rigaDaCancellare,materiaprima) {
    var salvaModel = new Object();
    var mat_cod_referenza = 0;
    var ParametroQual = 0;
    var Sa_Cod = 0;

    if (materiaprima === true) {
        salvaModel.CategoriaProdotto = rigaDaCancellare.Elem_Cod;
        salvaModel.CodiceProdotto = rigaDaCancellare.Cod_Articolo;
        salvaModel.IsMateriaPrima = true;
        salvaModel.Mat_Cod = rigaDaCancellare.Mat_Cod;
        salvaModel.TipoOperazioneDB = enum_tipoOperazione.Cancella;
        salvaModel.Piva = rigaDaCancellare.Piva;
        salvaModel.DatiTecnici = null
        salvaModel.DatiContabilita = null;
        salvaModel.StoricoPrezzi = null;
        salvaModel.Traduzioni = null;
        salvaModel.ParametriQualitativiCalibri = null;
        salvaModel.ParametriQualitativiIndici = null;
        salvaModel.Opzioni = null;
        salvaModel.ParametriQualitativiDatiTecnici = null;
        salvaModel.Cal_Cod = rigaDaCancellare.Cal_Cod;
        salvaModel.Proprietario = true;
        salvaModel.Pro_Cod = 0;
        salvaModel.ChkAlias = rigaDaCancellare.ChkAlias;

        Sa_Cod = rigaDaCancellare.Sa_Cod;

        //Passo anche se il bene di confezionamento è un IMBALLAGGIO,CONTENITORE o CONFEZIONE in modo da tale, 
        //da poterlo cancellare anche da Otabelle_Parametri nel caso sia stato inserito

        if (parseInt(rigaDaCancellare.Elem_Cod) === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI ||
            parseInt(rigaDaCancellare.Elem_Cod) === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI) {

            if (rigaDaCancellare.ChkImballaggio > 0)
                ParametroQual = enum_OmniTabelle.ot_IMBALLAGGI_FF;

            if (rigaDaCancellare.ChkContenitore > 0)
                ParametroQual = enum_OmniTabelle.ot_CONTENITORI_FF;

            let elenco = ParamQualGestitiPerCheckBoxBeniConf(rigaDaCancellare.Piva,"../Anagrafica/Prodotto_Edit.aspx");
            if (elenco !== null && elenco !== undefined) {
                let trovato = elenco.some(item => item.Tabella_ID === enum_OmniTabelle.ot_CONFEZIONI_FF);
                if (trovato !== undefined && trovato === true) {
                    let Confezioni = RicercaValoriParametriQualitativi(enum_OmniTabelle.ot_CONFEZIONI_FF, $(cIdPiva).val(), false);
                    if (Confezioni !== undefined && Confezioni !== null && Confezioni.length > 0) {
                        ParametroQual = enum_OmniTabelle.ot_CONFEZIONI_FF;
                    }
                }
            }
        }

        mat_cod_referenza = parseInt(rigaDaCancellare.Mat_Cod_Referenza);
    }
    else {
        salvaModel.Piva = rigaDaCancellare.Piva;
        salvaModel.CategoriaProdotto = rigaDaCancellare.Elem_Cod;
        salvaModel.Pro_Cod = rigaDaCancellare.Prodotto_Cod;
        salvaModel.IsMateriaPrima = false;
        salvaModel.TipoOperazioneDB = enum_tipoOperazione.Cancella;
    }

    var salvaModelEscaped = kendoEscapeOggetto(salvaModel);

    var Prodotto_Edit_UC_Keys = undefined;

    var param = "{model:'" + salvaModelEscaped + "', mat_cod_referenza:" + mat_cod_referenza + ", ParametroQual:" + parseInt(ParametroQual) + ", Sa_Cod:" + Sa_Cod+"}";

    WaitFrame.show();
    ajaxAgronica("../Anagrafica/Prodotto_Edit.aspx/Cancella_Prodotto",
        param,
        function (risposta) {

            if (materiaprima === true) {
                kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoCancellatoCorrettamente", "Prodotto cancellato correttamente"));
            }
            else {
                kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "StoricoPrezziEDatiContabilitaCancellatiCorrettamente", "'Storico Prezzi' e 'Dati Contabilità' cancellati correttamente"));
            }

            caricaGrigliaProdotto(Prodotto_Edit_UC_Keys);
        },
        function (risposta) {
            let erroreNellaDelete = risposta.RispostaStringa;
            MessaggioErrore_Bootstrap(erroreNellaDelete, "DIV_Messaggi");
        });
}


