
function kendoProdotto_inizializza(divKendo, keys) {

    let Piva = $('#' + hidden_azienda_ClientID).val();
    let categorie = Get_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie");
    let Elem_Cod = parseInt(categorie);
    
    var funzioniCRUD = { funzioneRead: kReadValorizzazioneProdotto_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneProdotto_mod();
    var colonneKendoGrid = kReadValorizzazioneProdotto_col();

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
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
        selectable: "row",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: true,
        btnEliminaTuttiFiltri: false,
        colonneCustomKendoGrid: [
            {
                command: {
                    //Per i prodotti di banche dati nascondo il duplica
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoProdotto(this.closest('tr'),this.closest('.k-grid'))>" + TraduzioneMultiResx(resxProdottoEditUC, "Info", "Info") + "</div>" +
                        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaProdotto(this.closest('tr'),this.closest('.k-grid'))>" + TraduzioneMultiResx(resxProdottoEditUC, "Modifica", "Modifica") + "</div>" +
                        "<div class='btn btn-info btnDuplica' style='display:block;width:70px;border:0px;' onclick=duplicaProdotto(this.closest('tr'),this.closest('.k-grid'))>" + TraduzioneMultiResx(resxProdottoEditUC, "Duplica", "Duplica") + "</div>" +
                        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaProdotto(this.closest('tr'),this.closest('.k-grid'))>" + TraduzioneMultiResx(resxProdottoEditUC, "Cancella", "Cancella") + "</div>"
                }, title: TraduzioneMultiResx(resxProdottoEditUC, "Azioni", "Azioni"), width: "97px"
            }
        ]

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            //Se in mobile mostro solo la colonna unica
            //mostraColonnaUnicaSeInMobile(e, 1);
            autoFitSeMobile(e);

            //Accorcio l'altezza delle righe
            riduciAltezzaRighe(e, 1);

            if (keys != undefined) {
                var arrKeys = keys.split(",");
                var grid = $("#" + divKendo).data("kendoGrid");
                var data = grid.dataSource.data();
                for (var i = 0; i < arrKeys.length; i++) {
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].chiave == arrKeys[i]) {
                            var rowUid = data[j].uid;
                            var row = grid.table.find("[data-uid=" + rowUid + "]");
                            grid.select(row);
                        }
                    }
                }
            }
            nascondiBottoniProdotto();
        }
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoOperazioni = creaKendoGrid(divKendo, // rappresenta l'ID del div a cui si associa la griglia
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

function nascondiBottoniProdotto() {
    var grid = $("#divKendoProdotto").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        if (!permesso_prodotti_read) {
            $(this).find(".btnInfo").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnDuplica").each(function (item) {
                $(this).hide();
            });
        }

        if (!permesso_prodotti_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnDuplica").each(function (item) {
                $(this).hide();
            });
        }

        //Per i prodotti di banche dati nascondo il duplica
        let elem_cod = parseInt(model.Elem_Cod);
        let materia_prima = IsMateriaPrima(elem_cod);
        let is_alias = false;

        if (parseInt(model.ChkAlias) === 1)
            is_alias = true;

        if (materia_prima === false) {

            $(this).find(".btnDuplica").each(function (item) {
                $(this).hide();
            });

        } else if (materia_prima === true) {
            //Tolgo il bottone elimina per evitare di eliminare prodotti di cui non sono proprietario
            let piva = $('#' + hidden_azienda_ClientID).val();
            let piva_prodotto = model.Piva;
            let proprietario = false;

            if (piva === piva_prodotto)
                proprietario = true;

            if (proprietario === false) {
                $(this).find(".btnCancella").each(function (item) {
                    $(this).hide();
                });

                //Tolgo anche il bottone duplica se non sono proprietario
                $(this).find(".btnDuplica").each(function (item) {
                    $(this).hide();
                });

                //Se il Prodotto è un Alias Pubblico ma io non sono il Proprietario tolgo il pulsante di modifica
                //perché per ora tutti i campi inseribili(Categoria Magazzino,Codice Prodotto,Codice Esterno, Descrizione,
                //Visibilita e Traduzioni) sono modificabili solamente dall'Azienda proprietaria
                if (is_alias) {
                    $(this).find(".btnModifica").each(function (item) {
                        $(this).hide();
                    });
                }
            }
        }

    });
}

function kReadValorizzazioneProdotto_rows(options) {

    let data = $('#hdKendoProdotto_Valorizzazione').val();
    let jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneProdotto_col() {

    let data = $('#hdKendoProdotto_Valorizzazione').val();
    let jSonParsed_Kendo = JSON.parse(data);

    let columns = jSonParsed_Kendo.kendo_columns;

    //Rinomino la colonna da Categoria Magazzino a Categoria Prodotto
    for (let c of columns) {

        if (c.field === "NomeComune") {
            c.title = TraduzioneMultiResx(resxProdottoEditUC, "CategoriaProdotto", "Categoria Prodotto");
            break;
        }

    }

    return columns;
}

function kReadValorizzazioneProdotto_mod() {

    let data = $('#hdKendoProdotto_Valorizzazione').val();
    let jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaProdotto(keys) {

    var categorie = Get_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie");
    var Elem_Cod = parseInt(categorie);

    var filtroPerDescrizione = "";

    if ($("#txt_descrizione_prodotto").val() != "" && $("#txt_descrizione_prodotto").val() != undefined && $("#txt_descrizione_prodotto").val().length >= 3) {
        filtroPerDescrizione = $("#txt_descrizione_prodotto").val();
    }

    var descrizioneFilterArray = [];
    var descrizione =
    {
        value: $("#txt_descrizione_prodotto").val(),
        field: "Prodotto_Des",
        operator: "contains",
        ignoreCase: true
    };
    descrizioneFilterArray.push(descrizione);
    filtroPerDescrizione = kendo.stringify(descrizioneFilterArray);

    var soloInGiacenza = false;
    var Sa_Cod = 0;
    var Fabbricato_Cod = 0;
    var TipoDestinazione = 0;
    var Cau_Mov = "7300";
    var xPUARegolamento = 0;
    var xLottoAccettazione = "";
    var Data_Movimento = kendo.parseDate(storageGetItem("MenuBS_Anagrafica.filterData"))
    if (Data_Movimento === null)
        Data_Movimento = formattedDate(new Date(), "/");
    var Flag_QtaNoZero = false;
    var xTipoPUARegolamento = 0;
    var piva = $('#' + hidden_azienda_ClientID).val();


    var ValoreFiltroDescrizioneProdotto = [];

    if (filtroPerDescrizione !== "[]")
        ValoreFiltroDescrizioneProdotto = JSON.parse(filtroPerDescrizione)[0].value;

    if (FiltroMinCaratteriPerRicerca() &&
        (ValoreFiltroDescrizioneProdotto === "" || ValoreFiltroDescrizioneProdotto.length < 3)) {
        kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "InserireAlmenoTreCaratteriProdotto", "Inserire almeno tre caratteri della Descrizione/Cod. Articolo del Prodotto."));
    }
    else {
        RicercaElencoCompletoProdottiPerAnagrafica(keys, objP_super_server, objP_server, objP_utenti, piva,
            Sa_Cod, Fabbricato_Cod, TipoDestinazione,
            Elem_Cod, soloInGiacenza, filtroPerDescrizione, "", Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, false, Flag_QtaNoZero, xTipoPUARegolamento);
    }

}

function GeneraFiltroAggiuntivoXRicercaAnagraficaProdotti(filtro_categoria_commerciale, filtroReg, filtroVisi) {

    let FiltroAggiuntivo = "";

    if ((filtro_categoria_commerciale !== "" && filtro_categoria_commerciale !== "0") &&
        filtroReg !== 0 && filtroVisi !== -99) {

        FiltroAggiuntivo += " ( Materie_Prime.Cat_Cod IN (" + filtro_categoria_commerciale + ") AND Materie_Prime.Regolamento = " + filtroReg + " AND Materie_Prime.Sa_Cod = " + filtroVisi + ")"

        return FiltroAggiuntivo;
    }
    else {

        if (filtro_categoria_commerciale === "" || filtro_categoria_commerciale === "0") {

            if (filtroReg !== 0 && filtroVisi !== -99) {

                FiltroAggiuntivo += " ( Materie_Prime.Regolamento = " + filtroReg + " AND Materie_Prime.Sa_Cod = " + filtroVisi + ")";

            }
            else {

                if (filtroReg !== 0) {
                    FiltroAggiuntivo += " Materie_Prime.Regolamento = " + filtroReg;
                }

                if (filtroVisi !== -99) {
                    FiltroAggiuntivo += " Materie_Prime.Sa_Cod = " + filtroVisi;
                }
            }

            return FiltroAggiuntivo;
        }

        if (filtroReg === 0) {

            if ((filtro_categoria_commerciale !== "" && filtro_categoria_commerciale !== "0") && filtroVisi !== -99) {

                FiltroAggiuntivo += " ( Materie_Prime.Cat_Cod IN (" + filtro_categoria_commerciale + ") AND Materie_Prime.Sa_Cod = " + filtroVisi + ")";

            }
            else {

                if (filtro_categoria_commerciale !== "" && filtro_categoria_commerciale !== "0") {
                    FiltroAggiuntivo += " Materie_Prime.Cat_Cod IN (" + filtro_categoria_commerciale + ")";
                }

                if (filtroVisi !== -99) {
                    FiltroAggiuntivo += " Materie_Prime.Sa_Cod = " + filtroVisi;
                }

            }

            return FiltroAggiuntivo;
        }

        if (filtroVisi === -99) {

            if ((filtro_categoria_commerciale !== "" && filtro_categoria_commerciale !== "0") && filtroReg !== 0) {

                FiltroAggiuntivo += " ( Materie_Prime.Cat_Cod IN (" + filtro_categoria_commerciale + ") AND Materie_Prime.Regolamento = " + filtroReg + ")";

            }
            else {

                if (filtro_categoria_commerciale !== "" && filtro_categoria_commerciale !== "0") {
                    FiltroAggiuntivo += " Materie_Prime.Cat_Cod IN (" + filtro_categoria_commerciale + ")";
                }

                if (filtroReg !== 0) {
                    FiltroAggiuntivo += " Materie_Prime.Regolamento = " + filtroReg;
                }

            }

            return FiltroAggiuntivo;
        }

    }

    return FiltroAggiuntivo;
}

async function modificaProdotto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var pro_cod = 0;
    var mat_cod = 0;

    if (datiRiga.Prodotto_Cod < 0)
        mat_cod = datiRiga.Prodotto_Cod * -1;
    else
        pro_cod = datiRiga.Prodotto_Cod;

    var elem_cod = datiRiga.Elem_Cod;
    var piva = datiRiga.Piva;
    var prodotto_des = datiRiga.Prodotto_Des;
    var sa_cod = datiRiga.sa_cod;

    var isalias = false;

    if (datiRiga.ChkAlias > 0)
        isalias = true;

    var permessoModifica = await PermessoModificaProdotto(piva, sa_cod);

    $(".prodotto_Edit_UC_gridArea").hide();
    $(".prodotto_Edit_UC_searchArea").hide();
    $(".prodotto_Edit_UC_modifyArea").show();

    //Sono in modifica quindi passo duplica come false
    //Aggiunto il parametro MostraTuttiTab in base al sa_cod,
    //se il prodotto è pubblico o privato.
    var param = kendo.stringify(
        {
            piva: piva,
            mat_cod: mat_cod,
            elem_cod: elem_cod,
            prodotto_des: prodotto_des,
            pro_cod: pro_cod,
            duplica: false,
            proprietario: permessoModifica,
            sa_cod: sa_cod,
            isalias: isalias
        });

    //if (permessoModifica) {
    WaitFrame.show();
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditProdotto',
        data: param,
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            let parametroVisibilita = riportaParametroVisibilita();
            if (r.d.indexOf("?") >= 0) {
                parametroVisibilita = parametroVisibilita.replace("?", "&")
            }
            window.location = r.d + parametroVisibilita;
        }
    });
    //}
}


async function infoProdotto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var pro_cod = 0;
    var mat_cod = 0;

    if (datiRiga.Prodotto_Cod < 0)
        mat_cod = datiRiga.Prodotto_Cod * -1;
    else
        pro_cod = datiRiga.Prodotto_Cod;

    var elem_cod = datiRiga.Elem_Cod;
    var piva = datiRiga.Piva;
    var prodotto_des = datiRiga.Prodotto_Des;
    var sa_cod = datiRiga.sa_cod;

    var isalias = false;

    if (datiRiga.ChkAlias > 0)
        isalias = true;

    $(".prodotto_Edit_UC_gridArea").hide();
    $(".prodotto_Edit_UC_searchArea").hide();
    $(".prodotto_Edit_UC_modifyArea").show();

    var param = kendo.stringify(
        {
            piva: piva,
            mat_cod: mat_cod,
            elem_cod: elem_cod,
            prodotto_des: prodotto_des,
            pro_cod: pro_cod,
            sa_cod: sa_cod,
            isalias: isalias
        });

    WaitFrame.show();
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoProdotto',
        data: param,
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            let parametroVisibilita = riportaParametroVisibilita();
            if (r.d.indexOf("?") >= 0) {
                parametroVisibilita = parametroVisibilita.replace("?", "&")
            }
            window.location = r.d + parametroVisibilita;
        }
    });
}


async function eliminaProdotto(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var pro_cod = 0;
    var mat_cod = 0;

    if (datiRiga.Prodotto_Cod < 0)
        mat_cod = datiRiga.Prodotto_Cod * -1;
    else
        pro_cod = datiRiga.Prodotto_Cod;

    var elem_cod = datiRiga.Elem_Cod;

    var sa_cod = datiRiga.sa_cod;
    var piva = datiRiga.Piva;

    var mat_prima = IsMateriaPrima(parseInt(elem_cod));
    var permessoModifica = false;

    if (mat_prima === true)
        permessoModifica = await PermessoModificaProdotto(piva, sa_cod);
    else
        permessoModifica = true;

    var isAlias = false;

    if (datiRiga.ChkAlias > 0)
        isAlias = true;

    //Permesso modifica
    if (permessoModifica) {

        //Controlla se il Prodotto è movimentato  
        //Aggiungere anche altri controlli del LAN


        var mostra_dialog_cancella = false;
        var msgTextDialog = "";

        if (mat_prima === true) {

            //############# 1. CONTROLLO MOVIMENTAZIONI #############################################################################
            let movimentato = Controlla_Se_Prodotto_Movimentato(elem_cod, mat_cod, pro_cod, true, isAlias);

            if (movimentato === true) {
                kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoMovimentatoNonCancellabile", "Il Prodotto è già movimentato non è possibile cancellarlo"));
            } else {

                let prod = Controlli_Cancella_Materia_Prima(elem_cod, mat_cod);

                if (prod.CancellaOk === false) {
                    kendo.alert(prod.MsgErrore);
                }
                else {
                    var prodotto = null;
                    //Leggo per avere a disposizione tutti i dati di quella materia prima
                    let paramObj = {
                        piva: piva,
                        elem_cod: parseInt(elem_cod),
                        mat_cod: parseInt(mat_cod)
                    };

                    let param = kendo.stringify(paramObj);

                    ajaxAgronicaSync("../Anagrafica/Prodotto_Edit.aspx/InfoProdotto",
                        param, false,
                        function (risposta) {
                            let risp = JSON.parse(risposta.RispostaStringa);
                            prodotto = risp[0];
                        }, null);

                    if (prodotto !== null && prodotto !== undefined && prodotto !== "") {

                        //Se il prodotto è importato non lo cancello
                        if (parseInt(prodotto.Flag_Importato) === 1) {
                            kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoImportatoNonCancellabile", "Il Prodotto è stato importato non è possibile cancellarlo"));
                            mostra_dialog_cancella = false;
                        }
                        else {
                            mostra_dialog_cancella = true;
                            let mess = ComponiMessaggioDiAvvisoCancellazioneProdotto(piva, elem_cod, mat_cod, sa_cod, isAlias);

                            msgTextDialog = TraduzioneMultiResx(resxProdottoEditUC, "ConfermaCancellazioneProdotto", "Si desidera davvero eliminare il prodotto aziendale dall'archivio anagrafico?");

                            if (mess !== "") {
                                msgTextDialog += "<br/><strong>" + TraduzioneMultiResx(resxProdottoEditUC, "ConfermaCancellazioneRiferimentiProdotto", "Verrano eliminati anche:") + "<br/>" + mess + "</strong>";
                            }
                        }
                    }
                    else {
                        kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "ErroreCancellazioneProdotto", "Errore non è possibile completare la cancellazione del prodotto"));
                        mostra_dialog_cancella = false;
                    }
                }
            }
        } else {
            mostra_dialog_cancella = true;
            msgTextDialog = TraduzioneMultiResx(resxProdottoEditUC, "ConfermaCancellazioneStoricoDati", "Si desidera davvero eliminare lo 'Storico Prezzi' , i 'Dati Contabilità' e i 'Dati Vendita Dettaglio' associati al prodotto aziendale?");
        }


        if (mostra_dialog_cancella) {
            let container = document.getElementById("divKendoProdotto");
            let id_dialog = creaNewRowDiv("id_dialog_cancella_prodotto");
            container.appendChild(id_dialog);

            $("#id_dialog_cancella_prodotto").kendoDialog({
                title: TraduzioneMultiResx(resxProdottoEditUC, "ConfermaCancellazione", "Conferma Cancellazione"),
                closable: true,
                modal: {
                    preventScroll: true
                },
                content: msgTextDialog,
                actions: [{
                    text: TraduzioneMultiResx(resxProdottoEditUC, "No", "No"),
                    primary: true
                },
                {
                    text: TraduzioneMultiResx(resxProdottoEditUC, "Si", "Sì"),
                    action: function (e) {

                        if (mat_prima === true) {
                            Cancella_Prodotto_Effettivo(prodotto, true);
                        } else {

                            if (datiRiga.Piva === "")
                                datiRiga.Piva = $('#' + hidden_azienda_ClientID).val();

                            Cancella_Prodotto_Effettivo(datiRiga, false);
                        }

                    }
                }]
            });
        }
    }
    else {
        kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "NonSiDisponePermessiCancellareProdotto", "Non si dispone dei permessi per cancellare il prodotto."));
    }


}

async function duplicaProdotto(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var pro_cod = 0;
    var mat_cod = 0;

    if (datiRiga.Prodotto_Cod < 0)
        mat_cod = datiRiga.Prodotto_Cod * -1;
    else
        pro_cod = datiRiga.Prodotto_Cod;

    var elem_cod = datiRiga.Elem_Cod;
    var piva = datiRiga.Piva;
    var prodotto_des = datiRiga.Prodotto_Des;
    var sa_cod = datiRiga.sa_cod;

    var isalias = false;

    if (datiRiga.ChkAlias > 0)
        isalias = true;

    var permessoModifica = await PermessoModificaProdotto(piva, sa_cod);

    $(".prodotto_Edit_UC_gridArea").hide();
    $(".prodotto_Edit_UC_searchArea").hide();
    $(".prodotto_Edit_UC_modifyArea").show();

    //Sono in Duplica quindi passo duplica come true,
    //Aggiunto il parametro MostraTuttiTab in base al sa_cod,
    //se il prodotto è pubblico o privato.
    var param = kendo.stringify(
        {
            piva: piva,
            mat_cod: mat_cod,
            elem_cod: elem_cod,
            prodotto_des: prodotto_des,
            pro_cod: pro_cod,
            duplica: true,
            proprietario: permessoModifica,
            sa_cod: sa_cod,
            isalias: isalias
        });

    //if (permessoModifica) {
    WaitFrame.show();
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditProdotto',
        data: param,
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = r.d + riportaParametroVisibilita();
        }
    });
    //}
}


function PermessoModificaProdotto(piva, sa_cod) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            piva: piva,
            sa_cod: sa_cod
        });

        ajaxAgronica("./MenuBs_Anagrafica.aspx/PermessoModificaProdotto",
            parametri,
            function (risposta) {
                if (risposta.RispostaConferma == false) {
                    //kendo.alert(risposta.RispostaStringa);
                }
                resolve(risposta.RispostaConferma);
            }, null, null, false);
    });
}

function RiempiSpecie(options) {
    options.success(Prodotto_Edit_UC_Elenco_Specie);
}

function RiempiCategorie(options) {
    let categ_magxRicerca = Prodotto_Edit_UC_Elenco_Categorie_Magazzino;
    let objVuoto = { "Elem_Cod": 0, "NomeComune": "" };
    categ_magxRicerca.unshift(objVuoto);

    options.success(categ_magxRicerca);
}

function RiempiVarieta(options) {
    var elencoVuoto = [{ Cul_Cod: -1, Cul_Des: "" }];
    if (Prodotto_Edit_UC_Elenco_Varieta !== "") {
        options.success(Prodotto_Edit_UC_Elenco_Varieta);
    } else {
        options.success(elencoVuoto);
    }
}

function RiempiCategorieCommerciali(options) {
    options.success(Prodotto_Edit_UC_Elenco_Categorie_Commerciali);
}


function CategoriaMagChange(e) {
    let elem_cod = parseInt(Get_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie"));

    if (categorieProdotti_FiltrabiliXSpecieVarieta.includes(elem_cod)) {

        $("#ddl_Ricerca_XSpecie").show();
        $("#ddl_Ricerca_XVarieta").show();

        $("#multiselSpecie").data("kendoMultiSelect").enable(true);
        $("#multiselVarieta").data("kendoMultiSelect").enable(true);
    }
    else {
        $("#ddl_Ricerca_XSpecie").hide();
        $("#ddl_Ricerca_XVarieta").hide();

        $("#multiselSpecie").data("kendoMultiSelect").enable(false);
        $("#multiselVarieta").data("kendoMultiSelect").enable(false);
        $("#multiselSpecie").data("kendoMultiSelect").value("");
        $("#multiselVarieta").data("kendoMultiSelect").value("");
    }


    if (categorieProdotti_FiltrabiliXCategoriaCommerciale.includes(elem_cod) &&
        $("#multiselCategCommle").data("kendoMultiSelect").dataSource.data().length>0) {

        $("#ddl_Ricerca_XCategCommle").show();

        $("#multiselCategCommle").data("kendoMultiSelect").enable(true);
    }
    else {
        $("#ddl_Ricerca_XCategCommle").hide();

        $("#multiselCategCommle").data("kendoMultiSelect").enable(false);
        $("#multiselCategCommle").data("kendoMultiSelect").value("");
    }

    $("#lbl_prodotto_Edit_UC_descrizione_3_caratteri").toggle(FiltroMinCaratteriPerRicerca());

}


function SpecieChange(e) {

    var filtro_specie = $("#multiselSpecie").data("kendoMultiSelect").value().join(",");
    
    if (filtro_specie !== "") {
        Prodotto_Edit_UC_Elenco_Varieta = Leggi_Varieta(filtro_specie);
    }
    else {
        Prodotto_Edit_UC_Elenco_Varieta = "";
        $("#multiselVarieta").data("kendoMultiSelect").value("");
    }

    $("#lbl_prodotto_Edit_UC_descrizione_3_caratteri").toggle(FiltroMinCaratteriPerRicerca());

    //Se viene deselezionata una Specie deseleziono anche le eventuali Varietà scelte di quella Specie
    let filtro_specieArray = $("#multiselSpecie").data("kendoMultiSelect").dataItems();

    let filtro_varietaArray = $("#multiselVarieta").data("kendoMultiSelect").dataItems();

    if (filtro_specieArray.length > 0 &&
        filtro_varietaArray.length > 0) {

        let varieta_scelte = [];

        for (var x = 0; x < filtro_specieArray.length; x++) {
            for (var y = 0; y < filtro_varietaArray.length; y++) {
                if (filtro_varietaArray[y].Cul_Des.includes(filtro_specieArray[x].veg_des)) {
                    varieta_scelte.push(filtro_varietaArray[y].Cul_Cod);
                }
            }
        }

        $("#multiselVarieta").data("kendoMultiSelect").value(varieta_scelte);
    }

    $("#multiselVarieta").data("kendoMultiSelect").dataSource.read();
    $("#multiselVarieta").data("kendoMultiSelect").refresh();
}

function Ricerca_Prodotti(keys) {
    $(".prodotto_Edit_UC_gridArea").show();
    caricaGrigliaProdotto(keys);
}

//Drop Down Categoria Prodotto Change:
//Abilitata solo nella fase di Scrittura di un Nuovo Prodotto
function Prodotto_Edit_UC_ddl_prodotto_UC_categ_prod_change(e) {

    if ($("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod") !== undefined &&
        $("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod_des") !== undefined) {

        var msgText = TraduzioneMultiResx(resxProdottoEditUC, "SiDesideraCambiareCategoriaProdotto_", "Si desidera davvero cambiare la Categoria Prodotto? Verranno persi i valori inseriti per la Categoria: ") +
             $("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod_des");

        let container = document.getElementById("prodotto_Edit_UC_modifyArea");
        let id_dialog = creaNewRowDiv("id_dialog_change_categ_prod");
        container.appendChild(id_dialog);

        $("#id_dialog_change_categ_prod").kendoDialog({
            title: TraduzioneMultiResx(resxProdottoEditUC, "ConfermaCambioCategoriaProdotto", "Conferma Cambio Categoria Prodotto"),
            closable: false,
            modal: {
                preventScroll: true
            },
            content: msgText,
            actions: [{
                text: TraduzioneMultiResx(resxProdottoEditUC, "No", "No"),
                primary: true,
                action: function (e) {
                    Set_KendoDDLValue("ddl_prodotto_UC_categ_prod", parseInt($("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod")));

                    $("#id_dialog_change_categ_prod").remove();
                }
            },
            {
                text: TraduzioneMultiResx(resxProdottoEditUC, "Si", "Sì"),
                action: function (e) {

                    $("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod", Get_KendoDDLValue("ddl_prodotto_UC_categ_prod"));
                    $("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod_des", KendoDDL("ddl_prodotto_UC_categ_prod").text());

                    $("#id_dialog_change_categ_prod").remove();

                    //Ricarico la pagina ogni volta che si cambia Categoria Prodotto
                    var elem_cod = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_categ_prod"));
                    var redirect_Url = Link_Redirect_Prodotto(elem_cod);

                    //Utilizzo replace perchè così anche se nella creazione di un 
                    //nuovo prodotto ho cambiato molte volte categoria se faccio torna indietro
                    //del browser mi torna subito alla pagina di ricerca
                    window.location.replace(redirect_Url);
                },
            }]
        });

    }
    else {
        $("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod", Get_KendoDDLValue("ddl_prodotto_UC_categ_prod"));
        $("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod_des", KendoDDL("ddl_prodotto_UC_categ_prod").text());

        //Ricarico la pagina ogni volta che si cambia Categoria Prodotto
        var elem_cod = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_categ_prod"));
        var redirect_Url = Link_Redirect_Prodotto(elem_cod);

        //Utilizzo replace perchè così anche se nella creazione di un 
        //nuovo prodotto ho cambiato molte volte categoria se faccio torna indietro
        //del browser mi torna subito alla pagina di ricerca
        window.location.replace(redirect_Url);
    }
}

//Drop Down Sementi e materiali vivaisti
function Prodotto_Edit_UC_ddl_sementi_materiale_Load() {
    let ds = new kendo.data.DataSource({ transport: { read: Carica_ddl_sementi_materiale } });

    if ($('#ddl_prodotto_UC_sementi_materiale').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_sementi_materiale').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_sementi_materiale').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "SEM_DES",
            dataValueField: "SEM_COD",
            change: Prodotto_Edit_UC_ddl_sementi_materiale_change
        }).data("kendoDropDownList");
    }
    else {
        $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").setDataSource(ds);
    }

}

function Prodotto_Edit_UC_ddl_sementi_materiale_change(e) {

    let ddl_sementi_mat_text = $('#ddl_prodotto_UC_sementi_materiale').data("kendoDropDownList").text();
    let ddl_sementi_mat_value = $('#ddl_prodotto_UC_sementi_materiale').data("kendoDropDownList").value();
    if (ddl_sementi_mat_value == "1") {
        $('.tecnologieSementi').removeAttr("hidden");
        $('.germinabilita').removeAttr("hidden");
        Prodotto_Edit_UC_ddl_prodotto_UC_TecnologieSementi();
        germinabilita = $('#txt_prodotto_UC_germinabilita').data("kendoNumericTextBox");
        if (germinabilita == undefined) {
            $("#txt_prodotto_UC_germinabilita").kendoNumericTextBox({ format: "0.##\\%", min: 1, max: 100, decimals: 2, value: 100 });
        }

    } else {
        $('.tecnologieSementi').attr("hidden", "hidden");
        $('.germinabilita').attr("hidden", "hidden");
    }

    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {
        $("#txt_prodotto_UC_Descrizione").val(ddl_sementi_mat_text);

        let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

        if (ddl_sementi_mat_text !== "" && dd_reg !== undefined) {
            if (parseInt(dd_reg.value()) === 4) {
                $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
            }
            else {
                let desc = $("#txt_prodotto_UC_Descrizione").val();
                if (desc.includes(" - BIO")) {
                    let nuova_desc = desc.replace(" - BIO", "");
                    $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                }
            }
        }

    }


    if (ddl_sementi_mat_text !== "") {
        //abilito la combo da caricare
        $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(true);
        let Sem_Cod = Get_KendoDDLValue("ddl_prodotto_UC_sementi_materiale");
        Prodotto_Edit_UC_ddl_prodotto_UC_specie_veg_Load(Sem_Cod);
    }
    else {
        Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", -1);
        $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(false);
    }

    //disabilito la combo che deve essere caricata dopo
    $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
    $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);

    //ripulisco le combo
    Set_KendoDDLValue("ddl_prodotto_UC_varieta", 0);
    Set_KendoDDLValue("ddl_prodotto_UC_tipo_varietale", 0);

    nascondi_riepilogo_error();
}

function Prodotto_Edit_UC_ddl_prodotto_UC_TecnologieSementi(e) {
    elencoTecnologieSementi = Carica_ddl_prodotto_UC_TecnologieSementi();
    let ds = new kendo.data.DataSource({ data: elencoTecnologieSementi });

    if ($('#ddl_prodotto_UC_TecnologieSementi').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_TecnologieSementi').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_TecnologieSementi').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Descrizione",
            dataValueField: "Cod_TecnologiaSementi",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_TecnologieSementi_change
        }).data("kendoDropDownList");
    }
    else {
        $("#ddl_prodotto_UC_TecnologieSementi").data("kendoDropDownList").setDataSource(ds);
    }
}

function Prodotto_Edit_UC_ddl_prodotto_UC_TecnologieSementi_change() {
    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === false)
        return;

    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    let dd_TecnologieSementi = $('#ddl_prodotto_UC_TecnologieSementi').data("kendoDropDownList")
    if (dd_TecnologieSementi !== undefined) {
        let desc = $("#txt_prodotto_UC_Descrizione").val() ;
        elencoTecnologieSementi.forEach((elemTecnologia) => {
            desc = desc.replace(` - ${elemTecnologia.Descrizione}`, "");
        })

        if (elem_cod === categorieProdotti.SEMENTI && $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").value() == "1" && dd_TecnologieSementi.value() != 0) {
            desc += " - " + dd_TecnologieSementi.text();
        }
       
        $("#txt_prodotto_UC_Descrizione").val(desc);

    }
}
//Drop Down Specie Vegetali 
function Prodotto_Edit_UC_ddl_prodotto_UC_specie_veg_Load(SEM_COD) {
    let ElencoSpecie = Carica_ddl_prodotto_UC_specie_veg(SEM_COD);
    let ds = new kendo.data.DataSource({ data: ElencoSpecie });

    if ($('#ddl_prodotto_UC_specie_veg').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_specie_veg').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_specie_veg').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Veg_Des",
            dataValueField: "Veg_Cod",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_specie_veg_change
        }).data("kendoDropDownList");
    }
    else {
        $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").setDataSource(ds);
    }
}


function Prodotto_Edit_UC_ddl_prodotto_UC_specie_veg_change(e) {
    //Cambio la textbox della descrizione in base alla specie scelta
    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true)
        $("#txt_prodotto_UC_Descrizione").val("");

    let ddl_spec_veg_text = $('#ddl_prodotto_UC_specie_veg').data("kendoDropDownList").text();
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    let ddl_sementi_text = "";

    if (elem_cod === categorieProdotti.SEMENTI && $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList") !== undefined)
        ddl_sementi_text = $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text();

    if (ddl_spec_veg_text !== "") {
        if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {
            if (elem_cod === categorieProdotti.SEMENTI && !($("#ddl_prodotto_UC_sementi_materiale").prop('disabled'))) {
                $("#txt_prodotto_UC_Descrizione").val(ddl_sementi_text + " - " + ddl_spec_veg_text);
            }
            else {
                $("#txt_prodotto_UC_Descrizione").val(ddl_spec_veg_text);
            }
            
            let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

            if (dd_reg !== undefined) {
                if (parseInt(dd_reg.value()) === 4) {
                    $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
                }
                else {
                    let desc = $("#txt_prodotto_UC_Descrizione").val();
                    if (desc.includes(" - BIO")) {
                        let nuova_desc = desc.replace(" - BIO", "");
                        $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                    }
                }

            }
            let dd_TecnologieSementi = $('#ddl_prodotto_UC_TecnologieSementi').data("kendoDropDownList")
            if (dd_TecnologieSementi !== undefined) {
                if (elem_cod === categorieProdotti.SEMENTI && $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").value() == "1" && dd_TecnologieSementi.value() != 0) {
                    let nuova_desc = $("#txt_prodotto_UC_Descrizione").val() + " - " + dd_TecnologieSementi.text();
                    $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                }

            }
        }

        if (elem_cod === categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE)
            PopolaGrigliaIndici("prodotto_UC_griglia_indici");

        let cul_cod = Get_KendoDDLValue("ddl_prodotto_UC_varieta");
        if (cul_cod === "")
            cul_cod = 0;
        let veg_cod = Get_KendoDDLValue("ddl_prodotto_UC_specie_veg");
        Prodotto_Edit_UC_ddl_prodotto_UC_varieta_Load(veg_cod, cul_cod);

        Prodotto_Edit_UC_ddl_prodotto_UC_tipo_varietale_Load(veg_cod);

        //Drop Down Finalita Produttiva Load(fa parte del GIAS LAN)
        Prodotto_Edit_UC_ddl_prodotto_UC_final_prod_Load(veg_cod);

        if (Prodotto_Movimentato === false && elem_cod === categorieProdotti.TRASFORMATI_VEGETALI
            && Modulo_FF === true && Prodotto_Edit_UC_Importato === false && Is_OMNI === false) {

            //Prendo i valori caricati dalle DDL come filtro per prodotto base
            let regolamento = Get_KendoDDLValue("ddl_prodotto_UC_Regolamento");
            Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_Load(regolamento, veg_cod, cul_cod);

            //Ora i Parametri Qualitativi sono legati alla Specie Vegetale e alla Varietà non più al Prodotto Base di Riferimento.
            if (Get_KendoDDLValue("ddl_prodotto_UC_varieta") === "" || parseInt(Get_KendoDDLValue("ddl_prodotto_UC_varieta")) === 0 ) {
                $(".Referenze_Param_Qual").hide();
            }
            else {
                if (Is_OMNI === false)
                    creaParametriQualitativi_FF_Zoo(parseInt(veg_cod), cul_cod, 0);
            }

        }
        $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(true);
        $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(true);
    }
    else {
        if (elem_cod === categorieProdotti.SEMENTI && $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {
            $("#txt_prodotto_UC_Descrizione").val(ddl_sementi_text);

            let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

            if (dd_reg !== undefined) {
                if (parseInt(dd_reg.value()) === 4) {
                    $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
                }
                else {
                    let desc = $("#txt_prodotto_UC_Descrizione").val();
                    if (desc.includes(" - BIO")) {
                        let nuova_desc = desc.replace(" - BIO", "");
                        $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                    }
                }
            }
        }
        //Per azzerare le dropdown se non si è scelta nessuna specie vegetale
        var dataSourceVarieta = new kendo.data.DataSource({ data: [{ "Cul_Cod": 0, "Cul_Des": "" }] });
        var dataSourceTipoVar = new kendo.data.DataSource({ data: [{ "Grva_Cod": 0, "Grva_Des": "" }] });
        $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").setDataSource(dataSourceTipoVar);
        $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").setDataSource(dataSourceVarieta);

        if (Modulo_FF === true && elem_cod === categorieProdotti.TRASFORMATI_VEGETALI && Prodotto_Edit_UC_Importato === false &&
            Is_OMNI === false) {
            var dataSourceProdBase = new kendo.data.DataSource({ data: [{ "Mat_Cod": 0, "Mat_Des": "" }] });
            $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").setDataSource(dataSourceProdBase);
            $(".Referenze_Param_Qual").hide();
        }

    }

    nascondi_riepilogo_error();

}

//Drop Down Varietà colturale
function Prodotto_Edit_UC_ddl_prodotto_UC_varieta_Load(Veg_cod, Cul_cod) {
    let ElencoVarieta = Carica_ddl_prodotto_UC_varieta_Load(Veg_cod, Cul_cod);
    let ds = new kendo.data.DataSource({ data: ElencoVarieta });

    if ($('#ddl_prodotto_UC_varieta').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_varieta').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_varieta').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Cul_Des",
            dataValueField: "Cul_Cod",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_varieta_change
        }).data("kendoDropDownList");
    }
    else {
        $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").setDataSource(ds);
    }
}

function Prodotto_Edit_UC_ddl_prodotto_UC_varieta_change(e) {
    //Cambio la textbox della descrizione in base alla varietà scelta
    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true)
        $("#txt_prodotto_UC_Descrizione").val("");

    let ddl_varieta_text = $('#ddl_prodotto_UC_varieta').data("kendoDropDownList").text();
    let ddl_specie_veg_text = $('#ddl_prodotto_UC_specie_veg').data("kendoDropDownList").text();
    let ddl_tipo_var_text = $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").text();

    if (ddl_varieta_text !== "") {

        if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {

            if (ddl_tipo_var_text !== "") {

                if (parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.SEMENTI) {
                    if ($("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text() !== "") {
                        $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + ddl_varieta_text + " - " + ddl_tipo_var_text + " - " + $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text());
                    }
                }
                else {
                    $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + ddl_varieta_text + " - " + ddl_tipo_var_text);
                }
            }
            else {
                if (parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.SEMENTI) {
                    if ($("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text() !== "") {
                        $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + ddl_varieta_text + " - " + $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text());
                    }
                }
                else {
                    $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + ddl_varieta_text);
                }
            }

            let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

            if (dd_reg !== undefined) {
                if (parseInt(dd_reg.value()) === 4) {
                    $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
                }
                else {
                    let desc = $("#txt_prodotto_UC_Descrizione").val();
                    if (desc.includes(" - BIO")) {
                        let nuova_desc = desc.replace(" - BIO", "");
                        $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                    }
                }
            }
        }

    }
    else {
        if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {
            if (parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.SEMENTI) {
                if ($("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text() !== "") {
                    $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text());
                }
            }
            else {
                $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text);
            }

            let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

            if (dd_reg !== undefined) {
                if (parseInt(dd_reg.value()) === 4) {
                    $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
                }
                else {
                    let desc = $("#txt_prodotto_UC_Descrizione").val();
                    if (desc.includes(" - BIO")) {
                        let nuova_desc = desc.replace(" - BIO", "");
                        $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                    }
                }
            }
        }

    }

    if (Prodotto_Movimentato === false && parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.TRASFORMATI_VEGETALI
        && Modulo_FF === true && Prodotto_Edit_UC_Importato === false && Is_OMNI === false) {

        let veg_cod = "-1";
        if (Get_KendoDDLValue("ddl_prodotto_UC_specie_veg") !== "") {
            veg_cod = Get_KendoDDLValue("ddl_prodotto_UC_specie_veg");
        }

        let cul_cod = "0";
        if (Get_KendoDDLValue("ddl_prodotto_UC_varieta") !== "") {
            cul_cod = Get_KendoDDLValue("ddl_prodotto_UC_varieta");
        }

        let regolamento = Get_KendoDDLValue("ddl_prodotto_UC_Regolamento");
        Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_Load(regolamento, veg_cod, cul_cod);


        if (ddl_varieta_text !== "" && Is_OMNI === false) {
            //Ora i Parametri Qualitativi sono legati alla Specie Vegetale e alla Varietà non più al Prodotto Base di Riferimento. 
            creaParametriQualitativi_FF_Zoo(parseInt(veg_cod), parseInt(cul_cod), 0);
        }
        else {
            $(".Referenze_Param_Qual").hide();
        }

    }

    nascondi_riepilogo_error();
}

//Drop Down Tipologia Varietale 
function Prodotto_Edit_UC_ddl_prodotto_UC_tipo_varietale_Load(Veg_Cod) {
    let ElencoTipo = Carica_ddl_prodotto_UC_tipo_varietale(Veg_Cod);
    let ds = new kendo.data.DataSource({ data: ElencoTipo });

    if ($('#ddl_prodotto_UC_tipo_varietale').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_tipo_varietale').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_tipo_varietale').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Grva_Des",
            dataValueField: "Grva_Cod",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_tipo_varietale_change
        }).data("kendoDropDownList");
    } else {
        $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").setDataSource(ds);
    }

}

function Prodotto_Edit_UC_ddl_prodotto_UC_tipo_varietale_change(e) {

    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {
        $("#txt_prodotto_UC_Descrizione").val("");

        let ddl_specie_veg_text = $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").text();
        let ddl_varieta_text = $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").text();
        let ddl_tipo_var = $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").text();

        if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_tipo_varietale")) !== 0) {
            //Cambio la textbox della descrizione in base alla tipologia varietale scelta
            if (parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.SEMENTI && $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text() !== "")
                $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + ddl_varieta_text + " - " + ddl_tipo_var + " - " + $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text());
            else
                $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + ddl_varieta_text + " - " + ddl_tipo_var);

        }
        else {
            if (parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.SEMENTI && $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text() !== "")
                $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + ddl_varieta_text + " - " + $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text());
            else
                $("#txt_prodotto_UC_Descrizione").val(ddl_specie_veg_text + " - " + ddl_varieta_text);

        }

        let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

        if (dd_reg !== undefined) {
            if (parseInt(dd_reg.value()) === 4) {
                $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
            }
            else {
                let desc = $("#txt_prodotto_UC_Descrizione").val();
                if (desc.includes(" - BIO")) {
                    let nuova_desc = desc.replace(" - BIO", "");
                    $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                }
            }
        }
    }

    nascondi_riepilogo_error();
}

//Drop Down Ditta di provenienza
function Prodotto_Edit_UC_ddl_prodotto_UC_ditta_di_provenienza_Load() {
    let ds = new kendo.data.DataSource({ transport: { read: Carica_ddl_prodotto_UC_ditta_di_provenienza } });

    if ($('#ddl_prodotto_UC_ditta_di_provenienza').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_ditta_di_provenienza').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_ditta_di_provenienza').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Ditta_Des",
            dataValueField: "Ditta_Cod",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_ditta_di_provenienza_change,
            optionLabel: {
                Ditta_Des: "",
                Ditta_Cod: ""
            }
        }).data("kendoDropDownList");
    }
}

function Prodotto_Edit_UC_ddl_prodotto_UC_ditta_di_provenienza_change(e) {

}

//Drop Down Regolamento
function Prodotto_Edit_UC_ddl_prodotto_UC_Regolamento_Load() {
    let Regolamenti = [{ Reg_DES: TraduzioneMultiResx(resxProdottoEditUC, "RegNessuno", "Convenzionale (Reg. Nessuno)"), Reg_COD: 1 },
        { Reg_DES: TraduzioneMultiResx(resxProdottoEditUC, "RegCE834/07", "Biologico (Reg.CE 834/07 (Ex.Reg.CE 2092/91))"), Reg_COD: 4 }
    ];
    let ds = new kendo.data.DataSource({ data: Regolamenti });

    if ($('#ddl_prodotto_UC_Regolamento').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_Regolamento').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_Regolamento').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Reg_DES",
            dataValueField: "Reg_COD",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_Regolamento_change
        }).data("kendoDropDownList");
    }

}

function Prodotto_Edit_UC_ddl_prodotto_UC_Regolamento_change(e) {
    if (Prodotto_Movimentato === false && parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.TRASFORMATI_VEGETALI
        && Modulo_FF === true && Prodotto_Edit_UC_Importato === false && Is_OMNI === false) {
        //Prendo i valori caricati dalle DDL come filtro per prodotto base
        let veg_cod = Get_KendoDDLValue("ddl_prodotto_UC_specie_veg");
        let cul_cod = Get_KendoDDLValue("ddl_prodotto_UC_varieta");
        let regolamento = Get_KendoDDLValue("ddl_prodotto_UC_Regolamento");
        Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_Load(regolamento, veg_cod, cul_cod);
    }

    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true &&
        (KendoDDL("ddl_prodotto_UC_specie_veg") !== undefined ||
            KendoDDL("ddl_prodotto_UC_sementi_materiale") !== undefined ||
            KendoDDL("ddl_prodotto_UC_specie_anim") !== undefined)) {

        let regolamento = Get_KendoDDLValue("ddl_prodotto_UC_Regolamento");
        let ddl_sem_text = "";
        let ddl_spec_veg_text = "";
        let ddl_specie_anim_text = "";

        if (KendoDDL("ddl_prodotto_UC_sementi_materiale") !== undefined)
            ddl_sem_text = $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").text();

        if (KendoDDL("ddl_prodotto_UC_specie_veg") !== undefined)
            ddl_spec_veg_text = $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").text();

        if (KendoDDL("ddl_prodotto_UC_specie_anim") !== undefined)
            ddl_specie_anim_text = $("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text();

        if (ddl_sem_text !== "" || ddl_spec_veg_text !== "" || ddl_specie_anim_text !== "") {
            if (parseInt(regolamento) === 4) {
                $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
            }
            else {
                let desc = $("#txt_prodotto_UC_Descrizione").val();
                if (desc.includes(" - BIO")) {
                    let nuova_desc = desc.replace(" - BIO", "");
                    $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                }
            }
        }
    }
}

//Drop Down Prodotto base di riferimento
function Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_Load(regolamento, veg_cod, cul_cod) {
    let ElencoProdotti = Carica_ddl_prodotto_UC_Prodotto_base(regolamento, veg_cod, cul_cod);
    let ds = new kendo.data.DataSource({ data: ElencoProdotti });

    if ($('#ddl_prodotto_UC_Prodottobase').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_Prodottobase').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_Prodottobase').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Mat_Des",
            dataValueField: "Mat_Cod",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_change,
        }).data("kendoDropDownList");
    } else {
        $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").setDataSource(ds);
    }
}

function Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_change(e) {
    //Mostro i Parametri Qualitativi solo al change del Prodotto base di riferimento , se il modulo anagrafe è fresh food e se abbiamo un trasformato vegetale
    if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase")) !== 0) {

        //Se è stato scelto un Prodotto base di riferimento, il prodotto deve essere avere
        //la stessa visibilità del Prodotto base di riferimento.
        let sa_cod_ddl_prodotto_UC_Prodottobase = parseInt($("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").dataItem().Sa_Cod);

        if (sa_cod_ddl_prodotto_UC_Prodottobase === 0) {
            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").check(false);
        }
        else if(sa_cod_ddl_prodotto_UC_Prodottobase === -1) {
            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").check(true);
        }

        $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(false);
        $("#cb_prodotto_UC_materia_prima").attr("last_selected_sa_cod", getKendoSwitch("cb_prodotto_UC_materia_prima"));

        if (Modulo_FF === true && parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.TRASFORMATI_VEGETALI) {

            let ds = $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").dataSource.data();
            let prodottoscelto = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase"));
            let pos = ds.map(function (e) { return e.Mat_Cod; }).indexOf(prodottoscelto);
            let veg_cod = 0;
            let cul_cod = 0;
            let cal_cod = 0;
            if ((pos !== -1) && (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura) && (Prodotto_Materia_Prima_Letto !== null)
                && (Prodotto_Materia_Prima_Letto !== undefined)) {

                if (ds[pos].Veg_Cod !== undefined && ds[pos].Cul_Cod !== undefined && Prodotto_Materia_Prima_Letto.Cal_Cod !== undefined) {
                    veg_cod = ds[pos].Veg_Cod;
                    cul_cod = ds[pos].Cul_Cod;
                    cal_cod = Prodotto_Materia_Prima_Letto.Cal_Cod;
                }
            }
            else if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Scrittura) {
                if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_specie_veg")) > 0 && parseInt(Get_KendoDDLValue("ddl_prodotto_UC_varieta")) !== 0) {
                    veg_cod = Get_KendoDDLValue("ddl_prodotto_UC_specie_veg");
                    cul_cod = Get_KendoDDLValue("ddl_prodotto_UC_varieta");
                }
            }


            if (pos !== -1) {
                if (ds[pos].Veg_Cod !== undefined && ds[pos].Cul_Cod !== undefined && ds[pos].Regolamento !== undefined) {
                    Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", parseInt(ds[pos].Veg_Cod));
                    Prodotto_Edit_UC_ddl_prodotto_UC_varieta_Load(parseInt(ds[pos].Veg_Cod), parseInt(ds[pos].Cul_Cod));
                    Set_KendoDDLValue("ddl_prodotto_UC_varieta", parseInt(ds[pos].Cul_Cod));
                    Set_KendoDDLValue("ddl_prodotto_UC_Regolamento", parseInt(ds[pos].Regolamento));
                    Prodotto_Edit_UC_ddl_prodotto_UC_tipo_varietale_Load(parseInt(ds[pos].Veg_Cod));
                }
            }
        }
    }
    else if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase")) === 0) {
        //Riabilito il check della visibilità.
        $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(true);
    }
}

//Drop Down Specie Animale change
function Prodotto_Edit_UC_ddl_prodotto_UC_specie_anim_change(e) {
    let valore_selezionato = e.target.value;
    let res = valore_selezionato.split("|");
    let GEN_COD = parseInt(res[0]);
    let SPE_COD = parseInt(res[1]);

    if (GEN_COD !== undefined && GEN_COD !== null && SPE_COD !== undefined && SPE_COD !== null)
        Set_KendoDDLValue("ddl_prodotto_UC_specie_anim", GEN_COD + "|" + SPE_COD);

    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {

        $("#txt_prodotto_UC_Descrizione").val("");

        if ($("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text() !== "") {

            $("#txt_prodotto_UC_Descrizione").val($("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text());

            if (KendoDDL("ddl_prodotto_UC_razza_anim") !== undefined && KendoDDL("ddl_prodotto_UC_razza_anim") !== null && KendoDDL("ddl_prodotto_UC_razza_anim") !== "" && KendoDDL("ddl_prodotto_UC_razza_anim").text() !== "" &&
                KendoDDL("ddl_prodotto_UC_indi_produt") !== undefined && KendoDDL("ddl_prodotto_UC_indi_produt") !== null && KendoDDL("ddl_prodotto_UC_indi_produt") !== "" && KendoDDL("ddl_prodotto_UC_indi_produt").text() !== "") {

                $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione")[0].value + " - " + $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").text() + " - " + $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").text());
            }
            else {
                if (KendoDDL("ddl_prodotto_UC_razza_anim") !== undefined && KendoDDL("ddl_prodotto_UC_razza_anim") !== null && KendoDDL("ddl_prodotto_UC_razza_anim") !== "" && KendoDDL("ddl_prodotto_UC_razza_anim").text() !== "") {
                    $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione")[0].value + " - " + $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").text());
                }

                if (KendoDDL("ddl_prodotto_UC_indi_produt") !== undefined && KendoDDL("ddl_prodotto_UC_indi_produt") !== null && KendoDDL("ddl_prodotto_UC_indi_produt") !== "" && KendoDDL("ddl_prodotto_UC_indi_produt").text() !== "") {
                    $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione")[0].value + " - " + $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").text());
                }
            }


            let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

            if (dd_reg !== undefined) {
                if (parseInt(dd_reg.value()) === 4) {
                    $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
                }
                else {
                    let desc = $("#txt_prodotto_UC_Descrizione").val();
                    if (desc.includes(" - BIO")) {
                        let nuova_desc = desc.replace(" - BIO", "");
                        $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                    }
                }
            }
        }


    }


    if ($("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text() !== "") {
        Prodotto_Edit_UC_ddl_prodotto_UC_indi_produt_Load(GEN_COD, SPE_COD);
        Prodotto_Edit_UC_ddl_prodotto_UC_razza_anim_Load(GEN_COD, SPE_COD);
        $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").enable(true);
        $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").enable(true);
    }
    else {
        //Pulisco le Dropdown perchè in questo caso ho solo un codice e non tutti e due
        $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").setDataSource({});
        $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").setDataSource({});
    }
    nascondi_riepilogo_error();

}

//Drop Down Indirizzo Produttivo Animale
function Prodotto_Edit_UC_ddl_prodotto_UC_indi_produt_Load(gen_cod, spe_cod) {
    let ElencoIndirizzi = Carica_ddl_indi_produt(gen_cod, spe_cod);
    let ds = new kendo.data.DataSource({ data: ElencoIndirizzi });

    if ($('#ddl_prodotto_UC_indi_produt').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_indi_produt').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_indi_produt').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "IPRO_DES",
            dataValueField: "IPRO_COD",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_indi_produt_change
        }).data("kendoDropDownList");
    }
    else {
        $('#ddl_prodotto_UC_indi_produt').data("kendoDropDownList").setDataSource(ds);
    }

}

//Drop Down Indirizzo Produttivo Animale change
function Prodotto_Edit_UC_ddl_prodotto_UC_indi_produt_change(e) {
    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {
        $("#txt_prodotto_UC_Descrizione").val("");
        if ($("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").text() !== "") {
            $("#txt_prodotto_UC_Descrizione").val($("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text() + " - " + $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").text());

            if (KendoDDL("ddl_prodotto_UC_razza_anim") !== undefined && KendoDDL("ddl_prodotto_UC_razza_anim") !== null && KendoDDL("ddl_prodotto_UC_razza_anim") !== "" && KendoDDL("ddl_prodotto_UC_razza_anim").text() !== "") {
                $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione")[0].value + " - " + $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").text());
            }
        }
        else {
            $("#txt_prodotto_UC_Descrizione").val($("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text());

            if (KendoDDL("ddl_prodotto_UC_razza_anim") !== undefined && KendoDDL("ddl_prodotto_UC_razza_anim") !== null && KendoDDL("ddl_prodotto_UC_razza_anim") !== "" && KendoDDL("ddl_prodotto_UC_razza_anim").text() !== "") {
                $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione")[0].value + " - " + $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").text());
            }
        }

        let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

        if (dd_reg !== undefined) {
            if (parseInt(dd_reg.value()) === 4) {
                $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
            }
            else {
                let desc = $("#txt_prodotto_UC_Descrizione").val();
                if (desc.includes(" - BIO")) {
                    let nuova_desc = desc.replace(" - BIO", "");
                    $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                }
            }
        }
    }

    nascondi_riepilogo_error();
}

//Drop Down Razza Animale
function Prodotto_Edit_UC_ddl_prodotto_UC_razza_anim_Load(gen_cod, spe_cod) {
    let ElencoRazze = Carica_ddl_razza_anim(gen_cod, spe_cod);
    let ds = new kendo.data.DataSource({ data: ElencoRazze });

    if ($('#ddl_prodotto_UC_razza_anim').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_razza_anim').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_razza_anim').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "RAZ_DES",
            dataValueField: "RAZ_COD",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_razza_anim_change
        }).data("kendoDropDownList");
    }
    else {
        $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").setDataSource(ds);
    }
}

//Drop Down Razza Animale change
function Prodotto_Edit_UC_ddl_prodotto_UC_razza_anim_change(e) {

    if ($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check() === true) {
        $("#txt_prodotto_UC_Descrizione").val("");

        if (KendoDDL("ddl_prodotto_UC_razza_anim").text() !== "") {
            $("#txt_prodotto_UC_Descrizione").val($("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text() + " - " + $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").text() + " - " + $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").text());
        }else {
            if (KendoDDL("ddl_prodotto_UC_indi_produt") !== undefined && KendoDDL("ddl_prodotto_UC_indi_produt") !== null && KendoDDL("ddl_prodotto_UC_indi_produt") !== "" && KendoDDL("ddl_prodotto_UC_indi_produt").text() !== "") {
                $("#txt_prodotto_UC_Descrizione").val($("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text() + " - " + $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").text());
            }
            else {
                $("#txt_prodotto_UC_Descrizione").val($("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").text());
            }
        }

        let dd_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");

        if (dd_reg !== undefined) {
            if (parseInt(dd_reg.value()) === 4) {
                $("#txt_prodotto_UC_Descrizione").val($("#txt_prodotto_UC_Descrizione").val() + " - BIO");
            }
            else {
                let desc = $("#txt_prodotto_UC_Descrizione").val();
                if (desc.includes(" - BIO")) {
                    let nuova_desc = desc.replace(" - BIO", "");
                    $("#txt_prodotto_UC_Descrizione").val(nuova_desc);
                }
            }
        }
    }

    nascondi_riepilogo_error();
}

//Drop Down Categoria Commerciale change(fa parte del GIAS LAN)
function Prodotto_Edit_UC_ddl_prodotto_UC_categ_ris_change(e) {

    if (parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.TRASFORMATI_VEGETALI &&
        Modulo_FF === true && Prodotto_Movimentato === false && Is_OMNI === false) {

        let last_linea_prod = parseInt($("#ddl_prodotto_UC_linea_prod").attr("last_Prodotto_Edit_Linea_Cod"));

        if (last_linea_prod !== undefined &&
            last_linea_prod !== null &&
            last_linea_prod !== 0) {

            Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", 0);

            //Resetto la Varieta
            var dataSourcevarieta = new kendo.data.DataSource({ data: [{ "Cul_Cod": 0, "Cul_Des": "" }] });
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").setDataSource(dataSourcevarieta);
            Set_KendoDDLValue("ddl_prodotto_UC_varieta", 0);

            //Resetto la Tipologia Varietale
            var dataSourceTipologiaVar = new kendo.data.DataSource({ data: [{ "Grva_Cod": 0, "Grva_Des": "" }] });
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").setDataSource(dataSourceTipologiaVar);
            Set_KendoDDLValue("ddl_prodotto_UC_tipo_varietale", 0);


            Set_KendoDDLValue("ddl_prodotto_UC_Regolamento", 1);

            //Resetto il Prodotto_Base
            var dataSourceProdBase = new kendo.data.DataSource({ data: [{ "Mat_Cod": 0, "Mat_Des": "" }] });
            $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").setDataSource(dataSourceProdBase);
            Set_KendoDDLValue("ddl_prodotto_UC_Prodottobase", 0);

            //Resetto la Finalità Produttiva (che dipende dal veg_cod) 
            var dataSourceFinalita = new kendo.data.DataSource({ data: [{ "Grfi_Cod": 0, "Grfi_Des": "" }] });
            $("#ddl_prodotto_UC_final_prod").data("kendoDropDownList").setDataSource(dataSourceFinalita);
            Set_KendoDDLValue("ddl_prodotto_UC_final_prod", 0);

            $(".Referenze_Param_Qual").hide();
        }

        if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase")) !== 0) {
            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(false);
        }
        else if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase")) === 0) {
            //Riabilito lo switch visibilità nel caso fosse stato disabilitato con la selezione del prodotto base di riferimento
            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(true);
        }

    }

    //Ricarico le Linee di Produzione 
    var linea_classe_cod = parseInt(e.sender.dataItem().Linea_Classe_Cod);
    Prodotto_Edit_UC_ddl_prodotto_UC_linea_prod_Load(linea_classe_cod);
    $("#ddl_prodotto_UC_linea_prod").attr("last_Prodotto_Edit_Linea_Cod", 0);
    Set_KendoDDLValue("ddl_prodotto_UC_linea_prod", 0);
}

//Drop Down LineaProduzione Load(fa parte del GIAS LAN)
function Prodotto_Edit_UC_ddl_prodotto_UC_linea_prod_Load(linea_classe_cod) {
    let ElencoLineeProduzione = Carica_ddl_linea_prod(linea_classe_cod);
    let ds = new kendo.data.DataSource({ data: ElencoLineeProduzione });

    if ($('#ddl_prodotto_UC_linea_prod').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_linea_prod').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_linea_prod').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Linea_Des",
            dataValueField: "Linea_Cod",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_linea_prod_change
        }).data("kendoDropDownList");
    }
    else {
        $('#ddl_prodotto_UC_linea_prod').data("kendoDropDownList").setDataSource(ds);
    }
}

//Drop Down LineaProduzione change(fa parte del GIAS LAN)
function Prodotto_Edit_UC_ddl_prodotto_UC_linea_prod_change(e) {

    //Se sono in modifica di un Trasformato Vegetale disabilito le ddl Specie,Varieta e Regolamento;
    //lascio abilitato solo linea e prodotto base di riferimento
    if (parseInt($(Controls.xElem_Cod).val()) === categorieProdotti.TRASFORMATI_VEGETALI &&
        Modulo_FF === true && Prodotto_Movimentato === false && Is_OMNI === false) {
        let ds = $("#ddl_prodotto_UC_linea_prod").data("kendoDropDownList").dataSource.data();
        let lineascelta = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_linea_prod"));
        $("#ddl_prodotto_UC_linea_prod").attr("last_Prodotto_Edit_Linea_Cod", lineascelta);
        var pos = ds.map(function (e) { return e.Linea_Cod; }).indexOf(lineascelta);


        if (pos !== -1 && (ds[pos].Veg_cod !== undefined && parseInt(ds[pos].Veg_cod) !== 0) &&
            (ds[pos].Cul_Cod !== undefined && parseInt(ds[pos].Cul_Cod) !== 0) &&
            (ds[pos].Reg_Cod !== undefined && parseInt(ds[pos].Reg_Cod) !== 0)) {

            Set_KendoDDLValue("ddl_prodotto_UC_Prodottobase", 0);
            Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", parseInt(ds[pos].Veg_cod));
            Prodotto_Edit_UC_ddl_prodotto_UC_varieta_Load(parseInt(ds[pos].Veg_cod), parseInt(ds[pos].Cul_Cod));
            Set_KendoDDLValue("ddl_prodotto_UC_varieta", parseInt(ds[pos].Cul_Cod));
            Set_KendoDDLValue("ddl_prodotto_UC_Regolamento", parseInt(ds[pos].Reg_Cod));
            Prodotto_Edit_UC_ddl_prodotto_UC_tipo_varietale_Load(parseInt(ds[pos].Veg_cod));
            //Ricarico la Finalità Produttiva con il veg_cod scelto
            Prodotto_Edit_UC_ddl_prodotto_UC_final_prod_Load(parseInt(ds[pos].Veg_cod));
            Set_KendoDDLValue("ddl_prodotto_UC_final_prod", 0);
            if (Prodotto_Movimentato === false && Prodotto_Edit_UC_Importato === false && Is_OMNI === false) {
                Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_Load(parseInt(ds[pos].Reg_Cod), parseInt(ds[pos].Veg_cod), parseInt(ds[pos].Cul_Cod));
                $(".Referenze_Param_Qual").hide();

                //Ora i Parametri Qualitativi sono legati alla Specie Vegetale e alla Varietà non più al Prodotto Base di Riferimento.
                creaParametriQualitativi_FF_Zoo(parseInt(ds[pos].Veg_cod), parseInt(ds[pos].Cul_Cod), 0);
            }

            if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Scrittura) {

                $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(true);

                $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(true);

                $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(true);
            }
        }
        else {
            Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", 0);

            //Resetto la Varieta
            var dataSourcevarieta = new kendo.data.DataSource({ data: [{ "Cul_Cod": 0, "Cul_Des": "" }] });
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").setDataSource(dataSourcevarieta);
            Set_KendoDDLValue("ddl_prodotto_UC_varieta", 0);

            //Resetto la Tipologia Varietale
            var dataSourceTipologiaVar = new kendo.data.DataSource({ data: [{ "Grva_Cod": 0, "Grva_Des": "" }] });
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").setDataSource(dataSourceTipologiaVar);
            Set_KendoDDLValue("ddl_prodotto_UC_tipo_varietale", 0);


            Set_KendoDDLValue("ddl_prodotto_UC_Regolamento", 1);

            //Resetto il Prodotto_Base
            var dataSourceProdBase = new kendo.data.DataSource({ data: [{ "Mat_Cod": 0, "Mat_Des": "" }] });
            $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").setDataSource(dataSourceProdBase);
            Set_KendoDDLValue("ddl_prodotto_UC_Prodottobase", 0);

            //Resetto la Finalità Produttiva (che dipende dal veg_cod) 
            var dataSourceFinalita = new kendo.data.DataSource({ data: [{ "Grfi_Cod": 0, "Grfi_Des": "" }] });
            $("#ddl_prodotto_UC_final_prod").data("kendoDropDownList").setDataSource(dataSourceFinalita);
            Set_KendoDDLValue("ddl_prodotto_UC_final_prod", 0);

            $(".Referenze_Param_Qual").hide();
        }


        if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase")) !== 0) {
            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(false);
        }
        else if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase")) === 0) {
            //Riabilito lo switch visibilità nel caso fosse stato disabilitato con la selezione del prodotto base di riferimento
            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(true);
        }

    }

}

function Prodotto_Edit_UC_alias_change(e) {

    if (e.checked === true) {
        $(xIs_Alias).val("True");
    }else if (e.checked === false) {
        $(xIs_Alias).val("False");
    }

    //Ricarico la pagina ogni volta che si seleziona o si deseleziona l'Alias
    var elem_cod = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_categ_prod"));
    var redirect_Url = Link_Redirect_Prodotto(elem_cod);

    //Utilizzo replace perchè così anche se nella creazione o modifica di un 
    //prodotto ho cambiato molte volte alias se faccio torna indietro
    //del browser mi torna subito alla pagina di ricerca
    window.location.replace(redirect_Url);
}

function Prodotto_Edit_UC_ddl_prodotto_UC_categ_ris_Load() {
    let categorieRisorse = Carica_ddl_categ_ris();
    let ds = new kendo.data.DataSource({ data: categorieRisorse });
    let ddl = $('#ddl_prodotto_UC_categ_ris').data("kendoDropDownList");

    if ($('#ddl_prodotto_UC_categ_ris').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_categ_ris').data("kendoDropDownList") === null) {

        ddl = $('#ddl_prodotto_UC_categ_ris').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Linea_Classe_Des",
            dataValueField: "Linea_Classe_Cod",
        }).data("kendoDropDownList");
    }

    return ddl;
}


//Drop Down Finalita Produttiva Load(fa parte del GIAS LAN)
function Prodotto_Edit_UC_ddl_prodotto_UC_final_prod_Load(veg_cod) {
    let ds = null;
    if (veg_cod === 0) {
        let vuoto = [{ "Grfi_Cod": 0, "Grfi_Des": "" }];
        ds = new kendo.data.DataSource({ data: vuoto });
    }
    else {
        let ElencoGruppoFinalita = Carica_ddl_prodotto_UC_final_prod(veg_cod);
        ds = new kendo.data.DataSource({ data: ElencoGruppoFinalita });
    }

    if ($('#ddl_prodotto_UC_final_prod').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_final_prod').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_final_prod').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "Grfi_Des",
            dataValueField: "Grfi_Cod",
        }).data("kendoDropDownList");
    }
    else {
        $("#ddl_prodotto_UC_final_prod").data("kendoDropDownList").setDataSource(ds);
    }
}

//Drop Down Unita di misura Default Load(fa parte del GIAS LAN)
function Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(elem_cod) {
    let ElencoUnitaMisuraDefault = Carica_ddl_prodotto_UC_unita_mis_def(elem_cod);
    let ds = new kendo.data.DataSource({ data: ElencoUnitaMisuraDefault });

    if ($('#ddl_prodotto_UC_unita_mis_def').data("kendoDropDownList") === undefined ||
        $('#ddl_prodotto_UC_unita_mis_def').data("kendoDropDownList") === null) {

        $('#ddl_prodotto_UC_unita_mis_def').kendoDropDownList({
            filter: "contains",
            dataSource: ds,
            autoBind: true,
            dataTextField: "udm_des",
            dataValueField: "udm_cod",
            change: Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_change,
        }).data("kendoDropDownList");
    } else {
        $("#ddl_prodotto_UC_unita_mis_def").data("kendoDropDownList").setDataSource(ds);
    }

}

function Ddl_prodotto_UC_mat_prima_priorita_cdg_Read(options) {
    options.success([
        { Val: 0, Des: TraduzioneMultiResx(resxProdottoEditUC, "ProdottoPrincipale", "Prodotto principale") },
        { Val: 1, Des: TraduzioneMultiResx(resxProdottoEditUC, "ProdottoSecondario", "Prodotto secondario") }
    ]);
}

function Prodotto_Edit_UC_materia_prima_change(e) {

    //All'onchange da pubblico a privato fai il controllo se c'è un alias pubblico(o viceversa se si sta modificando un alias) e dici che non è possibile evidenziando dove hai trovato il problema.
    //Stessa cosa se si passa da privato a pubblico

    let TipoOperazione = parseInt($(Controls.xTipoOperazione).val());

    let is_alias = getKendoSwitch("cb_prodotto_UC_alias");

    let elem_cod = parseInt($(Controls.xElem_Cod).val());

    let isMateriaPrima = IsMateriaPrima(elem_cod);

    if (isMateriaPrima === true &&
        $("#cb_prodotto_UC_materia_prima").attr("last_selected_sa_cod") !== undefined &&
        $("#cb_prodotto_UC_materia_prima").attr("last_selected_sa_cod") !== null &&
        $("#cb_prodotto_UC_materia_prima").attr("last_selected_sa_cod") !== "") {

        WaitFrame.show();

        let last_selected_sa_cod = ($("#cb_prodotto_UC_materia_prima").attr("last_selected_sa_cod") == "true");

        if (is_alias === false) {

            OnTabShow("a_tab_prodotto_UC_alias");

            let grid_Alias = $("#prodotto_UC_griglia_alias").data("kendoGrid");

            if (grid_Alias !== undefined && grid_Alias !== null && grid_Alias !== "") {

                let ds_grid_Alias = grid_Alias.dataSource.data();

                if (ds_grid_Alias.length > 0) {

                    e.checked=last_selected_sa_cod;

                    kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "ImpossibileCambiareVisibilitàProdottoPerAlias", "Non è possibile cambiare la Visibilità perché ci sono delle Descrizioni Alternative associate al Prodotto."));

                    e.preventDefault();
                }

            }

        } else if (is_alias === true &&
                    TipoOperazione !== enum_tipoOperazione.Scrittura &&
                    TipoOperazione !== enum_tipoOperazione.Duplica) {

            if (Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima === undefined ||
                Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima === null ||
                Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima === "") {

                Controlla_Se_Alias_Associato_a_Materia_Prima();
            }


            if (Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima === true) {

                e.checked = last_selected_sa_cod;

                kendo.alert(TraduzioneMultiResx(resxProdottoEditUC, "ImpossibileCambiareVisibilitàAlias", "Non è possibile cambiare la Visibilità perché il Prodotto per Descrizioni Alternative è già stato associato a dei Prodotti."));

                e.preventDefault();
            }

        }

        WaitFrame.hide();

    }

    $("#cb_prodotto_UC_materia_prima").attr("last_selected_sa_cod", e.checked);

}

//Disabilito tutti i controlli quando viene cliccato il bottone INFO
function Disabilita_Controlli_infoProdotto_Edit() {
    $("#switch_prodotto_UC_componi_descrizione").hide();

    $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(false);
    $("#cb_prodotto_UC_alias").data("kendoSwitch").enable(false);

    let ddl_sementi = $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList");
    let elem_cod = parseInt($(Controls.xElem_Cod).val());

    if (elem_cod === categorieProdotti.SEMENTI && ddl_sementi !== undefined) {
        ddl_sementi.enable(false);
    }

    let ddl_specie_anim = $("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList");
    let ddl_indi_prod = $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList");
    let ddl_razza = $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList");

    if ((elem_cod === categorieProdotti.SEMILAVORATI_PRODUZIONE_ANIMALE || elem_cod === categorieProdotti.TRASFORMATI_ANIMALI) &&
        (ddl_specie_anim !== undefined || ddl_indi_prod !== undefined || ddl_razza !== undefined)) {

        ddl_specie_anim.enable(false);
        ddl_indi_prod.enable(false);
        ddl_razza.enable(false);
    }

    let ddl_specie_veg = $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList");
    let ddl_varieta = $("#ddl_prodotto_UC_varieta").data("kendoDropDownList");
    let ddl_tipo_var = $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList");
    let ddl_ditta_prov = $("#ddl_prodotto_UC_ditta_di_provenienza").data("kendoDropDownList");
    let ddl_reg = $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList");
    let ddl_tecnologieSementi = $("#ddl_prodotto_UC_TecnologieSementi").data("kendoDropDownList");

    if (ddl_specie_veg !== undefined)
        ddl_specie_veg.enable(false);

    if (ddl_varieta !== undefined)
        ddl_varieta.enable(false);

    if (ddl_tipo_var !== undefined)
        ddl_tipo_var.enable(false);

    if (ddl_ditta_prov !== undefined)
        ddl_ditta_prov.enable(false);

    if (ddl_ditta_prov !== undefined)
        ddl_ditta_prov.enable(false);

    if (ddl_reg !== undefined)
        ddl_reg.enable(false);
    if (ddl_tecnologieSementi !== undefined)
        ddl_tecnologieSementi.enable(false);


    $('#txt_prodotto_UC_Note').attr("disabled", "disabled");
    $('#txt_prodotto_UC_Descrizione').attr("disabled", "disabled");
    $('#txt_prodotto_UC_cod_prod').attr("disabled", "disabled");

    if ($('#txt_prodotto_UC_germinabilita').data("kendoNumericTextBox") !== null &&
        $('#txt_prodotto_UC_germinabilita').data("kendoNumericTextBox") !== undefined) {

        $('#txt_prodotto_UC_germinabilita').data("kendoNumericTextBox").readonly();
        $('#txt_prodotto_UC_germinabilita').attr("disabled", "disabled");

    }


    let ddl_prod_base = $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList");

    if (elem_cod === categorieProdotti.TRASFORMATI_VEGETALI && Modulo_FF === true && ddl_prod_base !== undefined && Is_OMNI === false) {
        $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").enable(false);
    }

    let ddl_final_prod = $("#ddl_prodotto_UC_final_prod").data("kendoDropDownList");
    let ddl_unita_mis_def = $("#ddl_prodotto_UC_unita_mis_def").data("kendoDropDownList");

    if (ddl_final_prod !== undefined)
        ddl_final_prod.enable(false);

    if (ddl_unita_mis_def !== undefined)
        ddl_unita_mis_def.enable(false);
    $("#txt_prodotto_UC_descr_add").attr("disabled", "disabled");
    //$('.dettagli').attr('disabled', true).removeAttr('checked');

    let ddl_categ_ris = $("#ddl_prodotto_UC_categ_ris").data("kendoDropDownList");
    let ddl_linea_prod = $("#ddl_prodotto_UC_linea_prod").data("kendoDropDownList");

    if (ddl_categ_ris !== undefined)
        ddl_categ_ris.enable(false);

    if (ddl_linea_prod !== undefined)
        ddl_linea_prod.enable(false);

    $('#txt_prodotto_UC_cod_est').attr("disabled", "disabled");

    let ddlMatPrimaPrioritaCdG = KendoDDL("ddl_prodotto_UC_mat_prima_priorita_cdg");
    if (ddlMatPrimaPrioritaCdG !== undefined) {
        ddlMatPrimaPrioritaCdG.enable(false);
    }

    if (paramQual_FF_filtrospevar !== undefined && paramQual_FF_filtrospevar !== null && paramQual_FF_filtrospevar !== "" &&
        valoriparamQual_FF_filtrospevar !== undefined && valoriparamQual_FF_filtrospevar !== null) {


        for (var x = 0; x < paramQual_FF_filtrospevar.length; x++) {


                switch (paramQual_FF_filtrospevar[x].Tipo) {
                    case 3:
                        // Tipo 3 sono i parametri a libera imputazione numerici
                        tipo_param = "txt";
                        let id_kendonumTxt = $("#" + tipo_param + paramQual_FF_filtrospevar[x].Tabella_Cod_Des).data("kendoNumericTextBox");
                        if (id_kendonumTxt !== undefined &&
                            id_kendonumTxt !== null &&
                            id_kendonumTxt !== "") {
                            id_kendonumTxt.enable(false);
                        }
                        break;

                    case 4:
                        // Tipo 4 sono i parametri a libera imputazione stringa
                        tipo_param = "txtStr";
                        $("#" + tipo_param + paramQual_FF_filtrospevar[x].Tabella_Cod_Des + "").attr("disabled", "disabled");
                        break;

                    case 5:
                        // Tipo 5 sono i parametri a libera imputazione data
                        tipo_param = "date";
                        let id_kendodatePicker = $("#" + tipo_param + paramQual_FF_filtrospevar[x].Tabella_Cod_Des).data("kendoDatePicker");
                        if (id_kendodatePicker !== undefined &&
                            id_kendodatePicker !== null &&
                            id_kendodatePicker !== "") {
                            id_kendodatePicker.enable(false);
                        }
                        break;

                    default:
                        let risp = controllaValoriDDLParametriQualitativi(valoriparamQual_FF_filtrospevar, parseInt(paramQual_FF_filtrospevar[x].Tabella_ID))

                        if (risp === true) {
                            // DDL
                            tipo_param = "ddl";
                            let id_kendodropdown = $("#" + tipo_param + paramQual_FF_filtrospevar[x].Tabella_Cod_Des).data("kendoDropDownList");
                            if (id_kendodropdown !== undefined &&
                                id_kendodropdown !== null &&
                                id_kendodropdown !== "") {
                                id_kendodropdown.enable(false);

                                //Oltre alle Dropdown dei contenitori e confezione disabilito anche le loro
                                //rispettive textbox
                                if (parseInt(paramQual_FF_filtrospevar[x].Tabella_ID) === 5 || parseInt(paramQual_FF_filtrospevar[x].Tabella_ID) === 8) {
                                    tipo_param = "txt";
                                    let id_kendonumTxt = $("#" + tipo_param + paramQual_FF_filtrospevar[x].Tabella_Cod_Des).data("kendoNumericTextBox");
                                    if (id_kendonumTxt !== undefined &&
                                        id_kendonumTxt !== null &&
                                        id_kendonumTxt !== "") {
                                        id_kendonumTxt.enable(false);
                                    }
                                }
                            }
                            break;
                        }

                }


            

        }
     }
}

function Disabilita_Controlli_NuovoProdotto(Elem_Cod_CategScelta) {
    Set_KendoDDLValue("ddl_prodotto_UC_varieta", 0);
    Set_KendoDDLValue("ddl_prodotto_UC_tipo_varietale", 0);
    Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", -1);

    //Mostro la tab parametri qualitativi solo per i prodotti che sono semilavorati vegetali
    if (Elem_Cod_CategScelta !== categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE) {
        $("#a_tab_prodotto_UC_parametri_qualitativi").hide();
        $(".tab-pane.fade.in.active").removeClass("active");
        $(".nav-tabs li.tabParametriQualitativi.active").removeClass("active");
        $(".nav-tabs li.tabDatiTecnici").addClass("active");
        $(".tab-pane.DatiTecnici").addClass("active fade in");
    }
    else {
        $("#a_tab_prodotto_UC_parametri_qualitativi").show();
    }

    Configuratore_Maschera_MateriaPrima(Elem_Cod_CategScelta);

    //Disabilito le Dropdown di dati generali
    switch (Elem_Cod_CategScelta) {
        case categorieProdotti.ALTRE_RISORSE:
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(true);
            Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            break;
        case categorieProdotti.MATERIE_PRIME_VEGETALI:
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(true);
            Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            break;
        case categorieProdotti.CARBURANTI:
        case categorieProdotti.MATERIE_PRIME_ANIMALI:
            if (categorieProdotti.CARBURANTI === Elem_Cod_CategScelta) {
                Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            }
            break;
        case categorieProdotti.SEMILAVORATI_PRODUZIONE_ANIMALE:
        case categorieProdotti.TRASFORMATI_ANIMALI:
            creaKendoDropDownList("ddl_prodotto_UC_specie_anim", { read: Carica_ddl_specie_anim }, "SPE_DES", "SPE_COD").bind("change", Prodotto_Edit_UC_ddl_prodotto_UC_specie_anim_change);
            Prodotto_Edit_UC_ddl_prodotto_UC_indi_produt_Load(0, 0);
            Prodotto_Edit_UC_ddl_prodotto_UC_razza_anim_Load(0, 0);
            $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").enable(false);
            break;
        case categorieProdotti.SEMENTI:
            Prodotto_Edit_UC_ddl_sementi_materiale_Load();
            Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", -1);
            $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);
            Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            break;
        case categorieProdotti.CONFEZIONI_PRODOTTI:
        case categorieProdotti.ALTRI_BENI_AMMORTIZZABILI:
            Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            break;
        case categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI:
        case categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE:
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(true);
            Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            break;
        case categorieProdotti.RICAMBI:
            Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            break;
        case categorieProdotti.SERVIZI_PROFESSIONALI:
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(true);
            Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            break;
        case categorieProdotti.TRASFORMATI_VEGETALI:
            Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(true);
            break;
    }
}

function CaricaProdottoBancheDati(tipooperazione) {
    var elem_cod = parseInt($(Controls.xElem_Cod).val());
    var pro_cod = $(Controls.xPro_Cod).val();
    var prodotto_des = $(Controls.xProdotto_Des).val();

    Set_KendoDDLValue("ddl_prodotto_UC_categ_prod", elem_cod);
    $("#txt_prodotto_UC_cod_prod").val(pro_cod);
    $("#txt_prodotto_UC_Descrizione").val(prodotto_des);
    $("#ddl_prodotto_UC_categ_prod").data("kendoDropDownList").enable(false);

    $("#switch_prodotto_UC_componi_descrizione").hide();
    $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(false);

    $(".alias").hide();
    $("#cb_prodotto_UC_alias").data("kendoSwitch").enable(false);
}

function CaricaProdotto(tipooperazione) {

    var elem_cod = null;

    var is_alias = ($(xIs_Alias).val() === "True");
    if (tipooperazione == enum_tipoOperazione.Scrittura) {
        elem_cod = parseInt($(Controls.xElem_Cod).val());
        Set_KendoDDLValue("ddl_prodotto_UC_categ_prod", elem_cod);
        $("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod", Get_KendoDDLValue("ddl_prodotto_UC_categ_prod"));
        $("#ddl_prodotto_UC_categ_prod").attr("last_selected_categ_prod_des", KendoDDL("ddl_prodotto_UC_categ_prod").text());
        $("#txt_prodotto_UC_cod_prod").val("");
        //Imposto come Regolamento Nessuno in Nuovo
        Set_KendoDDLValue("ddl_prodotto_UC_Regolamento", 1);
        var ragione_sociale = $(Controls.xRagioneSociale).val();
        $("#txt_prodotto_UC_impresa_ref").val(ragione_sociale);

        Disabilita_Controlli_NuovoProdotto(elem_cod);

        if (is_alias === false) {
            //Carico  i Parametri Qualitativi anche per i Trasformati Animali
            if (elem_cod === categorieProdotti.TRASFORMATI_ANIMALI) {
                if (Modulo_Zoo === true && Is_OMNI === false) {
                    creaParametriQualitativi_FF_Zoo(0, 0, 0);
                }
            }
        }

    }


    if (tipooperazione != enum_tipoOperazione.Scrittura) {
        var prodotto = LeggiProdotto(tipooperazione);
        Prodotto_Materia_Prima_Letto = prodotto;

        elem_cod = prodotto.Elem_Cod;
        Codice_Articolo_Originale = prodotto.Cod_Articolo;


        //Mostro la tab parametri qualitativi solo per i prodotti che sono semilavorati vegetali
        if (prodotto.Elem_Cod !== categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE) {
            $("#a_tab_prodotto_UC_parametri_qualitativi").hide();
        }


        if (prodotto.Sa_Cod === -1) {
            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").check(true);
        }
        else {
            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").check(false);
        }

        //Flag prodotto OMNI ChkReferenza = 1
        if (prodotto.ChkReferenza === 1) {
            Is_OMNI = true;
        }

        //Se il prodotto è gia movimentato disabilito la dropdown di
        //prodotto base di riferimento,così non si può cambiare referenza ma comunque
        //possono essere modificati i parametri qualitativi

        let movi = true;
        if (tipooperazione === enum_tipoOperazione.Duplica) {
            movi = false;
        }
        else {
            movi = Controlla_Se_Prodotto_Movimentato(parseInt($(Controls.xElem_Cod).val()), parseInt($(Controls.xMat_Cod).val()), parseInt($(Controls.xPro_Cod).val()), true, is_alias);
        }
        if (elem_cod === categorieProdotti.TRASFORMATI_VEGETALI) {
            if (Modulo_FF === true && Is_OMNI === false) {
                Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_Load(prodotto.Regolamento, prodotto.Veg_Cod, prodotto.Cul_Cod);
                if (movi === true) {
                    Prodotto_Movimentato = true;
                    $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").enable(false);
                }
                else {
                    Prodotto_Movimentato = false;
                    $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").enable(true);
                }

                if (prodotto.Mat_Cod_Referenza !== "" && prodotto.Mat_Cod_Referenza !== undefined && prodotto.Mat_Cod_Referenza !== null && prodotto.Mat_Cod_Referenza !== 0) {
                    //Imposto la ddl Prodottobase con la sua referenza
                    Set_KendoDDLValue("ddl_prodotto_UC_Prodottobase", prodotto.Mat_Cod_Referenza);

                    //Se è stato scelto un Prodotto base di riferimento, il prodotto deve essere avere
                    //la stessa visibilità del Prodotto base di riferimento.
                    let prodBaseSel = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase"));

                    if (isNaN(prodBaseSel) === false && prodBaseSel !== 0) {
                        let sa_cod_ddl_prodotto_UC_Prodottobase = parseInt($("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").dataItem().Sa_Cod);

                        if (sa_cod_ddl_prodotto_UC_Prodottobase === 0) {
                            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").check(false);
                        }
                        else if (sa_cod_ddl_prodotto_UC_Prodottobase === -1) {
                            $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").check(true);
                        }

                        $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(false);
                    }

                }

                //Ora i Parametri Qualitativi sono legati alla Specie Vegetale e alla Varietà non più al Prodotto Base di Riferimento. 
                //Solamente se il prodotto letto non è un Alias allora carico i Parametri Qualitativi
                if (prodotto.ChkAlias === 0 && Is_OMNI === false)
                    creaParametriQualitativi_FF_Zoo(parseInt(prodotto.Veg_Cod), parseInt(prodotto.Cul_Cod), parseInt(prodotto.Cal_Cod));
            }
            else {
                $("#prodotto_base").hide();
            }
        }

        //Se il Prodotto è un Alias ed è stato  movimentato oppure associato ad un Prodotto allora
        //rendo non editabile lo switch
        if (is_alias === true) {

            if (tipooperazione !== enum_tipoOperazione.Scrittura &&
                tipooperazione !== enum_tipoOperazione.Duplica) {

                if (Prodotto_Movimentato === true) {

                    $("#cb_prodotto_UC_alias").data("kendoSwitch").enable(false);

                } else if (Prodotto_Movimentato === false) {

                    if (Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima === undefined ||
                        Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima === null ||
                        Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima === "") {

                        Controlla_Se_Alias_Associato_a_Materia_Prima();

                    }

                    if (Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima === true)
                        $("#cb_prodotto_UC_alias").data("kendoSwitch").enable(false);

                }
            }

        } else {
            //Se il Prodotto è una Materia Prima disabilito lo switch per renderlo un alias
            if (is_alias === false && prodotto.ChkAlias===0)
                $("#cb_prodotto_UC_alias").data("kendoSwitch").enable(false);
        }
        

        //Carico  i Parametri Qualitativi anche per i Trasformati Animali
        if (elem_cod === categorieProdotti.TRASFORMATI_ANIMALI && prodotto.ChkAlias === 0) {
            if (Modulo_Zoo === true && Is_OMNI === false) {
                creaParametriQualitativi_FF_Zoo(0, 0, parseInt(prodotto.Cal_Cod) );
            }
        }

        //Se sono in Duplica di un prodotto di OMNI ChkReferenza=1 dato che non posso creare un prodotto OMNI
        //allora imposto il ChkReferenza a 0,stessa cosa se sto cercando di duplicare un prodotto importato
        if (tipooperazione === enum_tipoOperazione.Duplica) {
            prodotto.ChkReferenza = 0;
            Prodotto_Materia_Prima_Letto.ChkReferenza = 0;
            prodotto.Flag_Importato = 0;
            Prodotto_Materia_Prima_Letto.Flag_Importato = 0;
        }


        //Lo mostro solamente il codice esterno non lo faccio modificare
        $("#txt_prodotto_UC_cod_est").val(prodotto.Codice_Esterno);

        $("#txt_prodotto_UC_Note").val(prodotto.Note);
        $("#ddl_prodotto_UC_categ_prod").data("kendoDropDownList").enable(false);

        $("#txt_prodotto_UC_cod_prod").val(prodotto.Cod_Articolo);

        $("#txt_prodotto_UC_Descrizione").val(prodotto.Mat_Des);
        $("#txt_prodotto_UC_impresa_ref").val(prodotto.Referente);


        //Se il regolamento non è né 1 né 4 allora imposto il primo regolamento
        if (prodotto.Regolamento !== 1 && prodotto.Regolamento !== 4)
            prodotto.Regolamento = 1;

        if (tipooperazione != enum_tipoOperazione.Scrittura) {
            Prodotto_Edit_UC_ddl_prodotto_UC_linea_prod_Load(prodotto.CAT_COD);
            Prodotto_Edit_UC_ddl_prodotto_UC_final_prod_Load(prodotto.Veg_Cod);
        }

        Set_KendoDDLValue("ddl_prodotto_UC_Regolamento", prodotto.Regolamento);
        Set_KendoDDLValue("ddl_prodotto_UC_categ_prod", prodotto.Elem_Cod);
        Set_KendoDDLValue("ddl_prodotto_UC_ditta_di_provenienza", prodotto.Ditta_Cod);
        Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", prodotto.Veg_Cod);
        Set_KendoDDLValue("ddl_prodotto_UC_categ_ris", prodotto.CAT_COD);
        Set_KendoDDLValue("ddl_prodotto_UC_linea_prod", prodotto.Linea_Cod);
        Set_KendoDDLValue("ddl_prodotto_UC_final_prod", prodotto.Grfi_Cod);
        Set_KendoDDLValue("ddl_prodotto_UC_unita_mis_def", prodotto.Udm_Cod);

        $("#ddl_prodotto_UC_linea_prod").attr("last_Prodotto_Edit_Linea_Cod", prodotto.Linea_Cod);

        //CheckBox Dettagli(fa parte del GIAS LAN)
        //if (prodotto.Flag_Biologico > 0)
        //    $("#chk_prodotto_UC_agricoltura_biologica").prop("checked", true);
        //if (prodotto.Flag_Convenzionale > 0)
        //    $("#chk_prodotto_UC_agricoltura_convezionale").prop("checked", true);
        //if (prodotto.Flag_NonAgricolo > 0)
        //    $("#chk_prodotto_UC_origine_non_agri").prop("checked", true);
        //if (prodotto.Flag_AusiliareFabbricazione > 0)
        //    $("#chk_prodotto_UC_ausi_fabbr").prop("checked", true);

        //TextBox Descrizione Addizionale(fa parte del GIAS LAN)
        $("#txt_prodotto_UC_descr_add").val(prodotto.Extra_Str);

        Prodotto_Edit_UC_ddl_prodotto_UC_varieta_Load(prodotto.Veg_Cod, prodotto.Cul_Cod);
        Set_KendoDDLValue('ddl_prodotto_UC_varieta', prodotto.Cul_Cod);

        Prodotto_Edit_UC_ddl_prodotto_UC_tipo_varietale_Load(prodotto.Veg_Cod);
        Set_KendoDDLValue('ddl_prodotto_UC_tipo_varietale', prodotto.GRVA_COD_VEG);

        var isMateriaPrima = IsMateriaPrima(elem_cod);

        if (prodotto.Elem_Cod === categorieProdotti.SEMENTI) {
            Set_KendoDDLValue("ddl_prodotto_UC_sementi_materiale", prodotto.Sem_Cod);

            //let ddl_sementi_mat_value = $('#ddl_prodotto_UC_sementi_materiale').data("kendoDropDownList").value();
            if (prodotto.Sem_Cod == "1") {
                $('.tecnologieSementi').removeAttr("hidden");
                $('.germinabilita').removeAttr("hidden");
                Prodotto_Edit_UC_ddl_prodotto_UC_TecnologieSementi();
                $("#txt_prodotto_UC_germinabilita").kendoNumericTextBox({ format: "0.##\\%", min: 1, max: 100, decimals: 2, value: prodotto.Germinabilita * 100 });
                Set_KendoDDLValue('ddl_prodotto_UC_TecnologieSementi', prodotto.Cod_TecnologiaSementi);
            } else {
                $('.tecnologieSementi').attr("hidden", "hidden");
                $('.germinabilita').attr("hidden", "hidden");
            }
        }

        let valMatPrimaPrioritaCdG = 0;
        if (prodotto.Priorita !== undefined && prodotto.Priorita !== null) {
            valMatPrimaPrioritaCdG = prodotto.Priorita;
        }
        Set_KendoDDLValue("ddl_prodotto_UC_mat_prima_priorita_cdg", valMatPrimaPrioritaCdG);

        if (isMateriaPrima && prodotto.ChkAlias===0) {

            // Configurazione della maschera in base a prodotto.Elem_Cod
            Configuratore_Maschera_MateriaPrima(prodotto.Elem_Cod);

            switch (prodotto.Elem_Cod) {
                // Lavorati aziendali

                case categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE:
                    break;
                case categorieProdotti.TRASFORMATI_VEGETALI:
                    Set_KendoDDLValue("ddl_prodotto_UC_sementi_materiale", 0);
                    Set_KendoDDLValue("ddl_prodotto_UC_specie_veg", prodotto.Veg_Cod);
                    $("#ddl_prodotto_UC_ditta_di_provenienza").data("kendoDropDownList").enable(true);
                    $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(true);
                    break;

                case categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI:
                    $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(true);
                    $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(true);
                    break;

                case categorieProdotti.SEMILAVORATI_PRODUZIONE_ANIMALE:

                case categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI:

                case categorieProdotti.TRASFORMATI_ANIMALI:
                    var SPE_COD = prodotto.SPE_COD;
                    var GEN_COD = prodotto.GEN_COD;
                    var IPRO_COD = prodotto.IPRO_COD;
                    var RAZ_COD = prodotto.RAZ_COD;
                    creaKendoDropDownList("ddl_prodotto_UC_specie_anim", { read: Carica_ddl_specie_anim }, "SPE_DES", "SPE_COD").bind("change", Prodotto_Edit_UC_ddl_prodotto_UC_specie_anim_change);
                    Prodotto_Edit_UC_ddl_prodotto_UC_indi_produt_Load(GEN_COD, SPE_COD);
                    Prodotto_Edit_UC_ddl_prodotto_UC_razza_anim_Load(GEN_COD, SPE_COD);
                    Set_KendoDDLValue("ddl_prodotto_UC_specie_anim", GEN_COD + "|" + SPE_COD);

                    //Caso "Tutti gli Inidrizzi Produttivi"
                    if (parseInt(IPRO_COD) === -1) {
                        Set_KendoDDLValue("ddl_prodotto_UC_indi_produt", IPRO_COD.toString());
                    }
                    else {
                        Set_KendoDDLValue("ddl_prodotto_UC_indi_produt", GEN_COD + "|" + SPE_COD + "|" + IPRO_COD);
                    }


                    //Caso "Tutte le Razze"
                    if (parseInt(RAZ_COD) === -1) {
                        Set_KendoDDLValue("ddl_prodotto_UC_razza_anim", RAZ_COD.toString());
                    }
                    else {
                        Set_KendoDDLValue("ddl_prodotto_UC_razza_anim", GEN_COD + "|" + SPE_COD + "|" + RAZ_COD);
                    }


                    break;
                case categorieProdotti.MATERIE_PRIME_VEGETALI:
                case categorieProdotti.MATERIE_PRIME_ANIMALI:
                    break;
                //ALTRE MATERIE//
                //altre risorse
                case categorieProdotti.ALTRE_RISORSE:
                    break;
                case categorieProdotti.ALTRI_BENI_AMMORTIZZABILI:
                    break;
            }
        } else if (isMateriaPrima && prodotto.ChkAlias === 1) {
            Disabilita_Controlli_NuovoProdotto(elem_cod);
        }

        //Disabilito lo switch della visibilità se il prodotto è stato movimentato da un'altra azienda che non è la proprietaria
        if (movi === true) {
            if (Movimentazioni_Prodotto !== null && Movimentazioni_Prodotto.findIndex(m => m.PIVA !== prodotto.Piva) > -1) {
                $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").enable(false);
            }
        }


    }



    //Per abilitare la Dropdown Prodotto Base di Riferimento bisogna che il prodotto sia :
    // TRASFORMATO VEGETALE e il Modulo generazione deve essere 2(Fresh and Food)
    if ((elem_cod === categorieProdotti.TRASFORMATI_VEGETALI) && (Modulo_FF === true) && (Is_OMNI === false)) {
        if (prodotto !== undefined) {
            if (prodotto.ChkReferenza !== 1 && Prodotto_Movimentato === false)
                $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").enable(true);
            else
                $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").enable(false);
        }
    }


    if (tipooperazione === enum_tipoOperazione.Modifica) {

        //Se sono in modifica di un PRODOTTO MOVIMENTATO per esempio un Trasformato vegetale o animale,
        //disabilito 
        if (Prodotto_Movimentato === true) {
            if (prodotto.Elem_Cod === categorieProdotti.TRASFORMATI_VEGETALI) {
                $("#ddl_prodotto_UC_categ_ris").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_linea_prod").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList").enable(false);
            }

            if (prodotto.Elem_Cod === categorieProdotti.TRASFORMATI_ANIMALI) {
                $("#ddl_prodotto_UC_specie_anim").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_indi_produt").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_razza_anim").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList").enable(false);
            }
        } else if (Prodotto_Movimentato === false) {
             //Se sono in modifica di un Trasformato Vegetale NON MOVIMENTATO disabilito le ddl Specie,Varieta e Regolamento;
            //lascio abilitato solo linea e prodotto base di riferimento
            if (prodotto.Elem_Cod === categorieProdotti.TRASFORMATI_VEGETALI && Modulo_FF === true && (prodotto.Mat_Cod_Referenza !== 0 && prodotto.Mat_Cod_Referenza !== "")) {
                $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
                $("#ddl_prodotto_UC_Regolamento").data("kendoDropDownList").enable(false);

            }
        }
    }

    //Se sono in duplica carico le tab dei dati che devono essere duoplicati senza che l'utente apra le tab una alla volta
    if (tabProdotto_Edit_UC_DaDuplicare !== undefined && tabProdotto_Edit_UC_DaDuplicare !== null && tabProdotto_Edit_UC_DaDuplicare.length > 0 &&
        tipooperazione === enum_tipoOperazione.Duplica) {

        for (var x = 0; x < tabProdotto_Edit_UC_DaDuplicare.length; x++) {
            OnTabShow(tabProdotto_Edit_UC_DaDuplicare[x].tab);
        }
    }


    //Controllo se un prodotto è importato
    let flag_importato = 0;

    if (tipooperazione !== enum_tipoOperazione.Scrittura && tipooperazione !== enum_tipoOperazione.Duplica)
        flag_importato = prodotto.Flag_Importato;

    Prodotto_Importato(flag_importato);

    if (tipooperazione === enum_tipoOperazione.Scrittura || tipooperazione === enum_tipoOperazione.Duplica) {
        //Calcolo il nuovo Codice_Articolo se sono in Nuovo o Duplica e se Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno=false,
        //altrimenti lo imposto vuoto
        if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === false) {
            $("#txt_prodotto_UC_cod_prod").val(NuovoCodiceArticolo());
        }
        else if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === true) {
            $("#txt_prodotto_UC_cod_prod").val("");
        }
    }

    if ($("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList") !== undefined &&
        $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList") !== null &&
        Prodotto_Edit_UC_Importato === true && Is_OMNI === false)
        $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").enable(false);

    if (tipooperazione != enum_tipoOperazione.Scrittura) {
        //Prodotti OMNI(chkReferenza = 1)
        //è permesso solo l’inserimento dei costi, la traduzione estera, l’inserimento dei dati contabili 
        if (prodotto.ChkReferenza === 1) {
            Disabilita_Controlli_infoProdotto_Edit();
        }

        if (tipooperazione == enum_tipoOperazione.Lettura) {
            Prodotto_Edit_UC_ddl_prodotto_UC_varieta_Load(prodotto.Veg_Cod, prodotto.Cul_Cod);
            Set_KendoDDLValue('ddl_prodotto_UC_varieta', prodotto.Cul_Cod);
            Disabilita_Controlli_infoProdotto_Edit();
        }
        
    }

    //Switch Alias(fa parte del GIAS LAN)
    $("#cb_prodotto_UC_alias").data("kendoSwitch").check(is_alias);

    $("#cb_prodotto_UC_materia_prima").attr("last_selected_sa_cod", getKendoSwitch("cb_prodotto_UC_materia_prima"));

    //Disabilito i Parametri Qualitativi delle confezioni se l'unita di misura di default è il numero
    Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_change(null);

}




// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA COSTI
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaStoricoPrezzi(IDControllo, Disabilita_Griglia) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var colonna_editabile = false;

    if (Disabilita_Griglia !== undefined && Disabilita_Griglia !== null && Disabilita_Griglia !== null && Disabilita_Griglia === true) {
        UteAbilitatoInsMod = false;
        UteAbilitatoCanc = false;
    }

    if (UteAbilitatoInsMod === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    //Carico l'unita di misura preimpostata quando creo una nuova riga 
    if (ElencoUnitadimisura == null || ElencoUnitadimisura == undefined || tipoOperazione === enum_tipoOperazione.Scrittura)
        ElencoUnitadimisura = Carica_ddl_griglia_unita_di_misura();

    var udm_cod = 0;
    var udm_des = "";

    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    if (IsMateriaPrima(elem_cod)) {
        if (KendoDDL("ddl_prodotto_UC_unita_mis_def") !== null && KendoDDL("ddl_prodotto_UC_unita_mis_def") !== undefined)
            udm_cod = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_unita_mis_def"));
    }
    else {
        udm_cod = Imposta_Default_UDM_GridStoricoPrezzi();
    }

    //Se non ha default la categoria scelta ma ha solo due unità di misura(riga vuota e un altra),
    //imposto come default la seconda unità di misura.
    if (udm_cod === 0 && ElencoUnitadimisura.length === 2) {
        for (var x = 0; x < ElencoUnitadimisura.length; x++) {
            if (ElencoUnitadimisura[x].Udm_Cod !== 0) {
                udm_cod = ElencoUnitadimisura[x].Udm_Cod;
                break;
            }
        }
    }

    //Ottengo la descrizione dell'unità di misura di default
    if (udm_cod !== 0) {
        for (var x = 0; x < ElencoUnitadimisura.length; x++) {
            if (parseInt(ElencoUnitadimisura[x].Udm_Cod) === udm_cod) {
                udm_des = ElencoUnitadimisura[x].Udm_Des;
                break;
            }
        }
    }


    var funzioniCRUD = {
        funzioneRead: CaricaProdottiXGrigliaStoricoPrezzi,
        funzioneSubmit: { funzione: SubmitProdottiXGrigliaStoricoPrezzi, flagInsert: true, flagUpdate: true, flagDelete: UteAbilitatoCanc },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true
        //omettiPulsantiAnnulla: false
    };
    var idModel = "ID";
    var campiKendoModel = {
        ID: { editable: false, type: "number", defaultValue: 0 },
        Udm_Cod: { editable: colonna_editabile, type: "number", defaultValue: udm_cod },
        Pro_Cod: { editable: false, type: "number", defaultValue: 0 },
        Udm_Des: { editable: colonna_editabile, type: "string", defaultValue: udm_des },
        Prezzo_Unitario: { editable: colonna_editabile, type: "number", validation: { min: 0 } },
        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },
        Username_Creazione: { editable: false, type: "string" },
        Data_Creazione: { editable: false, type: "date" }
    };
    var colonneKendoGrid = [
        {
            field: "Validita_Inizio", title: TraduzioneMultiResx(resxProdottoEditUC, "ValiditàInizio", "Validità Inizio"), format: "{0:dd/MM/yyyy}"
        },
        {
            field: "Validita_Fine", title: TraduzioneMultiResx(resxProdottoEditUC, "ValiditàFine", "Validità Fine"), format: "{0:dd/MM/yyyy}"
        },
        {
            field: "Udm_Des", title: TraduzioneMultiResx(resxProdottoEditUC, "UnitàDiMisura", "Unità di Misura"), editor: unita_misura_DropDownEditor
        },
        {
            field: "Prezzo_Unitario", title: TraduzioneMultiResx(resxProdottoEditUC, "Prezzo", "Prezzo"), editor: numberEditor2decimals, format: "{0:n2}"
        }
    ];

    //var parametriKendoGrid = {};
    var parametriKendoGrid = {
        //editable: {
        //    mode: "inline"
        //},
        //colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditCosti, funzioneDaChiamareDopoDataBound: StoricoPrezziKendoGridDataBound };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];


    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function StoricoPrezziKendoGridDataBound(e) {
}

function onEditCosti(e) {
    var tipoOperazione = parseInt($(Controls.xTipoOperazione).val());
    if (tipoOperazione === enum_tipoOperazione.Lettura)
        e.sender.closeCell();

}

function unita_misura_DropDownEditor(container) {
    var tipoOperazione = parseInt($(Controls.xTipoOperazione).val());
    if (ElencoUnitadimisura == null || ElencoUnitadimisura == undefined || tipoOperazione === enum_tipoOperazione.Scrittura)
        ElencoUnitadimisura = Carica_ddl_griglia_unita_di_misura();

    ddl_udm = creaDropDownEditor(container, "Udm_Des", "Udm_Cod", ElencoUnitadimisura, changeUnitaMisura);

}

function changeUnitaMisura(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#prodotto_UC_griglia_storico_prezzi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Udm_Cod = dataItem.Udm_Cod;
    model.Udm_Des = dataItem.Udm_Des;
    //model.dirty = true;
}


function SubmitProdottiXGrigliaStoricoPrezzi(options) {

    var grid = $("#prodotto_UC_griglia_storico_prezzi").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitProdottiXGrigliaStoricoPrezzi(options.data.created) +
        controllaRigheCompletePerSubmitProdottiXGrigliaStoricoPrezzi(options.data.updated);

    if (nrErr > 0) {
        if (nrErr === 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxProdottoEditUC, "EsisteRigaIncompletaNellaGrigliaStoricoPrezzi", "Esiste una riga con dati non completi nella griglia Storico Prezzi."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(resxProdottoEditUC, "EsistonoNRigheIncompleteNellaGrigliaStoricoPrezzi", "Esistono {0} righe con dati non completi nella griglia Storico Prezzi."), nrErr), "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

    errMess = controllaRigheValidePerSubmitProdottiXGrigliaStoricoPrezzi(options.data.created, "");
    errMess = controllaRigheValidePerSubmitProdottiXGrigliaStoricoPrezzi(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Prezzi = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Prezzi = kendoEscapeOggetto(updatedRecords);
        righeEliminateGrid_Prezzi = kendoEscapeOggetto(deletedRecords);
    }

    righeTutteGrid_Prezzi = kendoEscapeOggetto(righeNonCancellate);
}


function controllaRigheCompletePerSubmitProdottiXGrigliaStoricoPrezzi(righe) {

    var nrErr = 0;
    for (let x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return nrErr;
}

function controllaRigheValidePerSubmitProdottiXGrigliaStoricoPrezzi(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return errMess;
}



function OnTabShow(tabId) {

    if (tabStripAperti.includes(tabId))
        return;

    let gridPrezzi = null;
    let gridTraduzioni = null;
    let gridAlias = null;
    let TipoOperazione = parseInt($(Controls.xTipoOperazione).val());

    tabStripAperti.push(tabId);

    // Verifica se prodotto aperto è una materia_prima
    var elem_cod = parseInt($(Controls.xElem_Cod).val());

    var isMateriaPrima = IsMateriaPrima(elem_cod);

    var proprietario = ($(Controls.xProprietario).val() == "True");

    if ((Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata === "") &&
        (tabId === "a_tab_prodotto_UC_dati_contabilita" || tabId === "a_tab_prodotto_UC_altri_dati")) 
        Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata = Leggi_Prodotti_Extra_PrivataXTab();
    

    switch (tabId) {
        case "a_tab_prodotto_UC_dati_tecnici":
            // Se non è proprietario ma è Materia Prima: Disabilito le Dropdown
            //Se Prodotto_Edit_UC_Importato=true tutte le tab in sole visualizzazione tranne "Dati Contabilità" e "Traduzioni"
            if ((isMateriaPrima === true && proprietario === false && TipoOperazione !== enum_tipoOperazione.Duplica) ||
                (Prodotto_Edit_UC_Importato === true)) {
                Disabilita_Controlli_infoProdotto_Edit();
            }
            break;
        case "a_tab_prodotto_UC_configurazione":
            $("#txt_prodotto_UC_Qta_Extra").kendoNumericTextBox({ format: "{0:n3}", decimals: 3, min: 0 , value: 0 });
            $("#txt_prodotto_UC_tara_nomi").kendoNumericTextBox({ format: "{0:n3}", decimals: 3, min: 0, value: 0 });
            $("#txt_prodotto_UC_Qta_Contenitore").kendoNumericTextBox({ format: "{0:n0}", decimals: 0, min: 0, value: 0 });
            $("#txt_prodotto_UC_Qta_Contenitore_Conf").kendoNumericTextBox({ format: "{0:n0}", decimals: 0, min: 0, value: 0 });
            //Controllo se è da mostrare il checkbox confezione nei dettagli bene di confezionamento
            if (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI) {
                let elenco = ParamQualGestitiPerCheckBoxBeniConf($(Controls.xPiva).val(), indirizzohttpPaginaProdotto);
                if (elenco !== null && elenco !== undefined) {
                    let trovato = elenco.some(item => item.Tabella_ID === enum_OmniTabelle.ot_CONFEZIONI_FF.toString());
                    if (trovato !== undefined && trovato === true)
                        Mostra_chkconfezione = true;
                }
            }    

            CaricaMascheraConfigurazione();

            // Se non è proprietario ma è Materia Prima: Disabilito le Dropdown
            //INFO: Disabilito i controlli della maschera
            //Se Prodotto_Edit_UC_Importato=true tutte le tab in sole visualizzazione tranne "Dati Contabilità" e "Traduzioni"
            if ((isMateriaPrima === true && proprietario === false && (TipoOperazione !== enum_tipoOperazione.Duplica || TipoOperazione === enum_tipoOperazione.Lettura)) ||
                (Prodotto_Edit_UC_Importato === true)) {
                DisabilitaTabConfigurazione();
            }

            break;
        case "a_tab_prodotto_UC_storico_prezzi":
            //INFO: Nascondo  i bottoni Aggiungi e Annulla delle griglie
            //Se Prodotto_Edit_UC_Importato=true tutte le tab in sole visualizzazione tranne "Dati Contabilità" e "Traduzioni"
            if (TipoOperazione === enum_tipoOperazione.Lettura || Prodotto_Edit_UC_Importato === true) {
                PopolaGrigliaStoricoPrezzi("prodotto_UC_griglia_storico_prezzi", true);
            } else {
                PopolaGrigliaStoricoPrezzi("prodotto_UC_griglia_storico_prezzi", null);
            }

            gridPrezzi = $("#prodotto_UC_griglia_storico_prezzi").data("kendoGrid");
            gridPrezzi.bind("cellClose", grid_cellClose);
            break;
        case "a_tab_prodotto_UC_parametri_qualitativi":
            PopolaGrigliaCalibri("prodotto_UC_griglia_calibri");
            PopolaGrigliaIndici("prodotto_UC_griglia_indici");
            break;
        case "a_tab_prodotto_UC_dati_contabilita":
            //Se Prodotto_Edit_UC_Importato=true tutte le tab in sole visualizzazione tranne "Dati Contabilità" e "Traduzioni"
            creaKendoDropDownList("prodotto_UC_ddlCodIva", { read: Carica_prodotto_UC_ddlCodIva }, "Sigla_IVA", "Cod_IVA").bind("change", Prodotto_Edit_UC_prodotto_UC_ddlCodIva_change);
            creaKendoDropDownList("prodotto_UC_ddlCodIvaCompensazione", { read: Carica_prodotto_UC_ddlCodIvaCompensazione }, "Descrizione", "Cod").bind("change", Prodotto_Edit_UC_prodotto_UC_ddlCodIvaCompensazione_change);
            creaKendoDropDownList("prodotto_UC_ddlContoEconomicoAcquisto", { read: Carica_prodotto_UC_ddlContoEconomicoAcquisto }, "Descr_Conto", "Cod_Conto").bind("change", Prodotto_Edit_UC_prodotto_UC_ddlContoEconomicoAcquisto_change);
            creaKendoDropDownList("prodotto_UC_ddlContoEconomicoVendita", { read: Carica_prodotto_UC_ddlContoEconomicoVendita }, "Descr_Conto", "Cod_Conto").bind("change", Prodotto_Edit_UC_prodotto_UC_ddlContoEconomicoVendita_change);
            creaKendoDropDownList("prodotto_UC_ddlContoPatrimonialeAcquisto", { read: Carica_prodotto_UC_ddlContoPatrimonialeAcquisto }, "Descr_Conto_Pat", "Cod_Conto_Pat").bind("change", Prodotto_Edit_UC_prodotto_UC_ddlContoPatrimonialeAcquisto_change);
            creaKendoDropDownList("prodotto_UC_ddlContoPatrimonialeVendita", { read: Carica_prodotto_UC_ddlContoPatrimonialeVendita }, "Descr_Conto_Pat", "Cod_Conto_Pat").bind("change", Prodotto_Edit_UC_prodotto_UC_ddlContoPatrimonialeVendita_change);
            creaKendoDropDownList("prodotto_UC_ddlGruppoMerce", { read: Carica_prodotto_UC_ddlGruppoMerce }, "Descrizione_Concatenata", "Id_Gruppo_Merce").bind("change", Prodotto_Edit_UC_prodotto_UC_ddlGruppoMerce_change);

            //Carico i dati legati al prodotto dalla tabella Prodotti_Extra_Privata
            //var prodotto_extra_privata = Leggi_Prodotti_Extra_PrivataXTab();
            //if (prodotto_extra_privata != undefined && prodotto_extra_privata != null && prodotto_extra_privata != "") {
            if (Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata != undefined && Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata != null && Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata != "") {
                xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Modifica;

                if (TipoOperazione === enum_tipoOperazione.Duplica)
                    xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Scrittura;

                Set_KendoDDLValue("prodotto_UC_ddlCodIva", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Cod_Iva);
                Set_KendoDDLValue("prodotto_UC_ddlCodIvaCompensazione", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Cod_Iva_Compensazione);
                Set_KendoDDLValue("prodotto_UC_ddlContoEconomicoAcquisto", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Cod_Conto_Economico_Acquisto);
                Set_KendoDDLValue("prodotto_UC_ddlContoEconomicoVendita", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Cod_Conto_Economico_Vendita);
                Set_KendoDDLValue("prodotto_UC_ddlContoPatrimonialeAcquisto", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Cod_Conto_Patrimoniale_Acquisto);
                Set_KendoDDLValue("prodotto_UC_ddlContoPatrimonialeVendita", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Cod_Conto_Patrimoniale_Vendita);
                Set_KendoDDLValue("prodotto_UC_ddlGruppoMerce", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Id_Gruppo_Merce);
            }
            else {
                if (TipoOperazione === enum_tipoOperazione.Scrittura)
                    xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Scrittura;
                else if (TipoOperazione === enum_tipoOperazione.Duplica)
                    xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Duplica;
            }


            //INFO: Disabilito le Dropdown se sono in Lettura oppure se il Prodotto è
            //una Materia_Prima ma non sono Proprietario (per i Prodotti di Banche Dati invece è sempre possibile inserire dei Dati Contabili)
            if (TipoOperazione === enum_tipoOperazione.Lettura || (isMateriaPrima === true && proprietario===false)) {
                $("#prodotto_UC_ddlCodIva").data("kendoDropDownList").enable(false);
                $("#prodotto_UC_ddlCodIvaCompensazione").data("kendoDropDownList").enable(false);
                $("#prodotto_UC_ddlContoEconomicoAcquisto").data("kendoDropDownList").enable(false);
                $("#prodotto_UC_ddlContoEconomicoVendita").data("kendoDropDownList").enable(false);
                $("#prodotto_UC_ddlContoPatrimonialeAcquisto").data("kendoDropDownList").enable(false);
                $("#prodotto_UC_ddlContoPatrimonialeVendita").data("kendoDropDownList").enable(false);
                $("#prodotto_UC_ddlGruppoMerce").data("kendoDropDownList").enable(false);
            }
            break;
        case "a_tab_prodotto_UC_traduzioni":
            //Se Prodotto_Edit_UC_Importato=true tutte le tab in sole visualizzazione tranne "Dati Contabilità" e "Traduzioni"
            //INFO e se non è proprietario ma è Materia Prima: Nascondo  i bottoni Aggiungi e Annulla della griglia
            if ((TipoOperazione === enum_tipoOperazione.Lettura) || (isMateriaPrima === true && proprietario === false)) {
                PopolaGrigliaTraduzioni("prodotto_UC_griglia_traduzioni",true);
            } else {
                PopolaGrigliaTraduzioni("prodotto_UC_griglia_traduzioni", null);
            }
            break;
        case "a_tab_prodotto_UC_altri_dati":
            creaKendoDropDownList("prodotto_UC_ddlProduzionePropria", { read: Carica_ddlProduzionePropria }, "Produzione_Des", "Produzione_Cod");
            creaKendoDropDownList("prodotto_UC_ddlPesoEgalizzato", { read: Carica_ddlPesoEgalizzato }, "Peso_Des", "Peso_Cod");
            $("#txt_prodotto_UC_PesoNetto").kendoNumericTextBox({ format: "{0:n6}", decimals: 6, min: 0, value:0 });
            $("#txt_prodotto_UC_PesoSgocciolato").kendoNumericTextBox({ format: "{0:n6}", decimals: 6, min: 0, value: 0 });
            $("#txt_prodotto_UC_Tara").kendoNumericTextBox({ format: "{0:n6}", decimals: 6, min: 0, value: 0 });

            Prodotto_Edit_UC_CreaImageEditor();

            //Mostro Ingredienti, Produzione Propria solo per Trasformati Vegetali e Trasformati Animali
            if (elem_cod === categorieProdotti.TRASFORMATI_VEGETALI || elem_cod === categorieProdotti.TRASFORMATI_ANIMALI) {
                $("#ingredienti").show();

                $("#produzione_propria").show();
            }else {
                $("#ingredienti").hide();

                $("#produzione_propria").hide();
            }

            //Come Default metto Prodotto_Commercializzato
            Set_KendoDDLValue("prodotto_UC_ddlProduzionePropria", enum_Produzione_Cod.Prodotto_Commercializzato);

            //Come Default metto vuoto -1
            Set_KendoDDLValue("prodotto_UC_ddlPesoEgalizzato", -1);

            //Carico i dati legati al prodotto dalla tabella Prodotti_Extra_Privata
            //var prodotto_extra_privata = Leggi_Prodotti_Extra_PrivataXTab();
            //if (prodotto_extra_privata != undefined && prodotto_extra_privata != null && prodotto_extra_privata != "") {
            if (Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata != undefined && Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata != null && Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata != "") {
                xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Modifica;

                if (TipoOperazione === enum_tipoOperazione.Duplica)
                    xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Scrittura;

                $("#txt_prodotto_UC_EAN").val(Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.EAN);
                $("#txt_prodotto_UC_Barcode").val(Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Barcode);
                Set_KendoNumTBValue("txt_prodotto_UC_PesoNetto", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Peso_Netto);
                Set_KendoNumTBValue("txt_prodotto_UC_PesoSgocciolato", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Peso_Sgocciolato);
                Set_KendoNumTBValue("txt_prodotto_UC_Tara", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Tara);
                Set_KendoDDLValue("prodotto_UC_ddlProduzionePropria", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Produzione_Propria);
                Set_KendoDDLValue("prodotto_UC_ddlPesoEgalizzato", Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Peso_Egalizzato);
                $("#txt_prodotto_UC_Ingredienti").val(Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Ingredienti);

                $("#id_prodotto_UC_rimuovi_immagine").show();

                //Carico l'Immagine nel kendo editor
                if (Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine !== null && Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine !== "" &&
                    Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine_Estensione !== null && Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine_Estensione !== "" &&
                    Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine_NomeFile !== null && Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine_NomeFile !== "") {

                    let byteCharacters_Immagine = atob(Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine);

                    var imageEditor = $("#prodotto_UC_imageEditor").data("kendoImageEditor");

                    imageEditor.drawImage(byteCharacters_Immagine).done(function (image) {

                        let datiImmagine = new Object();

                        datiImmagine.Nome = Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine_NomeFile;
                        datiImmagine.Estensione = Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata.Immagine_Estensione;
                        datiImmagine.Immagine = image.src;

                        imageEditor.options.saveAs.fileName = datiImmagine.Nome;

                        $("#prodotto_UC_lblImage").text("Immagine '" + datiImmagine.Nome + "' :");

                        $(xDatiImmagineCaricata).val(kendo.stringify(datiImmagine));

                        imageEditor.drawCanvas(image);
                    });

                }

            }
            else {
                if (TipoOperazione === enum_tipoOperazione.Scrittura)
                    xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Scrittura;
                else if (TipoOperazione === enum_tipoOperazione.Duplica)
                    xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Duplica;
            }


            //INFO: Disabilito le Dropdown se sono in Lettura oppure se il Prodotto è
            //una Materia_Prima ma non sono Proprietario (per i Prodotti di Banche Dati invece è sempre possibile inserire dei Dati Contabili)
            if (TipoOperazione === enum_tipoOperazione.Lettura || (isMateriaPrima === true && proprietario === false)) {

                $('#txt_prodotto_UC_EAN').attr("disabled", "disabled");
                $('#txt_prodotto_UC_Barcode').attr("disabled", "disabled");
                $('#txt_prodotto_UC_PesoNetto').data("kendoNumericTextBox").enable(false);
                $('#txt_prodotto_UC_PesoSgocciolato').data("kendoNumericTextBox").enable(false);
                $('#txt_prodotto_UC_Tara').data("kendoNumericTextBox").enable(false);
                $("#prodotto_UC_ddlProduzionePropria").data("kendoDropDownList").enable(false);
                $("#prodotto_UC_ddlPesoEgalizzato").data("kendoDropDownList").enable(false);               
                $('#txt_prodotto_UC_Ingredienti').attr("disabled", "disabled");
                $("#id_prodotto_UC_rimuovi_immagine").hide();
            }
            break;

        case "a_tab_prodotto_UC_alias":
            //INFO , se non è proprietario ma è Materia Prima o se è Importato: Nascondo  i bottoni Aggiungi e Annulla della griglia
            if ((TipoOperazione === enum_tipoOperazione.Lettura) ||
                (isMateriaPrima === true && proprietario === false) ||
                Prodotto_Edit_UC_Importato === true) {

                PopolaGrigliaAlias("prodotto_UC_griglia_alias", true);
            } else {
                PopolaGrigliaAlias("prodotto_UC_griglia_alias", null);
            }
            break;
    }
}


function onSelect(ev) {

    let estensione = ev.files[0].extension;

    let datiImmagine = new Object();

    datiImmagine.Nome = ev.files[0].name;
    datiImmagine.Estensione = estensione;
    datiImmagine.Immagine = "";

    $("#prodotto_UC_lblImage").text("Immagine '" + datiImmagine.Nome + "' :");

    $(xDatiImmagineCaricata).val(kendo.stringify(datiImmagine));

}

function handleUploadEvents(imageEditor) {
    OpenExecuted = true;
    imageEditor._upload.bind("select", onSelect);
}

function grid_cellClose(e) {
    if (e.type == "save") {
        input = e.container.find("input[name='Validita_Fine']").data("kendoDatePicker");
        if (input != undefined) {
            if (input.value() == "" || input.value() == undefined || input.value() == null)
                e.model.Validita_Fine = new Date("2100/12/31")
        }

        input = e.container.find("input[name='Validita_Inizio']").data("kendoDatePicker");
        if (input != undefined) {
            if (input.value() == "" || input.value() == undefined || input.value() == null)
                e.model.Validita_Inizio = new Date("1900/01/01")
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA CALIBRI
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaCalibri(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: CaricaProdottiXGrigliaTuttiCalibri,
        funzioneSubmit: { funzione: SubmitProdottiXGrigliaCalibri, flagInsert: false, flagDelete: false },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true,
        checkBoxFunction: kSelezionatoCalibro,

    };
    var idModel = "Tipo_Cod";
    var campiKendoModel = {
        Tipo_Cod: { editable: false, type: "number" },
        Cal_Des: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        {
            field: "Cal_Des", title: TraduzioneMultiResx(resxProdottoEditUC, "Calibri", "Calibri"), filterable: { multi: true, search: true }
        }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        //salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
        salvaRipristinaPersonalizzazioni: false,
        columnMenu: false,
        reorderable: false,
        excel: false,
        pdf: false,
        groupable: false,
        btnEliminaTuttiFiltri: false
    };

    var parametriPerLettura = null;
    var parametriDataSource = { pagesize: 50 };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDataBound: function (e) {

            var TipoOperazione = parseInt($(Controls.xTipoOperazione).val());
            var elem_cod = parseInt($(Controls.xElem_Cod).val());
            var isMateriaPrima = IsMateriaPrima(elem_cod);
            var proprietario = ($(Controls.xProprietario).val() == "True");

            var ElencoCalibriScelti = CaricaProdottiXGrigliaCalibri();
            var grid = $("#prodotto_UC_griglia_calibri").data("kendoGrid");
            var ds = grid.dataSource.data();
            var mat_cod = parseInt($(Controls.xMat_Cod).val());
            for (var x = 0; x < ds.length; x++) {
                for (var i = 0; i < ElencoCalibriScelti.length; i++) {
                    if (ds[x].Tipo_Cod === ElencoCalibriScelti[i].Tipo_Cod && mat_cod === ElencoCalibriScelti[i].Mat_Cod) {
                        ds[x].Selected = true;
                    }
                }
            }

            //INFO e se non è proprietario ma è Materia Prima: Nascondo  i bottoni Aggiungi e Annulla delle griglie
            //Se Prodotto_Edit_UC_Importato=true tutte le tab in sole visualizzazione tranne "Dati Contabilità" e "Traduzioni"
            if (TipoOperazione === enum_tipoOperazione.Lettura || (isMateriaPrima === true && proprietario === false) || Prodotto_Edit_UC_Importato === true) {
                $("#prodotto_UC_griglia_calibri-header-chb").attr('disabled', true);
                var allRows = grid.items();
                $.each(allRows, function (index, value) {
                    $(value).find(".k-checkbox").attr('disabled', true);
                })
            }
        }
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function kSelezionatoCalibro(e) {
    var checked = this.checked;
    row = $(this).parents("tr");
    grid = $("#prodotto_UC_griglia_calibri").data("kendoGrid");
    dataItem = grid.dataItem(row);
    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}

function SubmitProdottiXGrigliaCalibri(e) {
    var RigheScelte = [];
    var grid = $("#prodotto_UC_griglia_calibri").data("kendoGrid");
    var currentData = grid.dataSource.data();
    for (var i = 0; i < currentData.length; i++) {
        RigheScelte.push(currentData[i].toJSON());
    }
    if (RigheScelte.length > 0) {

        for (var x = 0; x < RigheScelte.length; x++) {
            if (RigheScelte[x].Selected === undefined ||
                RigheScelte[x].Selected === null ||
                RigheScelte[x].Selected === "")
                RigheScelte[x].Selected = false;
        }
        righeSelezionateGrid_Calibri = kendoEscapeOggetto(RigheScelte);
    }
}

// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA INDICI
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaIndici(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";


    var funzioniCRUD = {
        funzioneRead: CaricaProdottiXGrigliaTuttiIndici,
        funzioneSubmit: { funzione: SubmitProdottiXGrigliaIndici, flagInsert: false, flagDelete: false },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true,
        checkBoxFunction: kSelezionatoIndice
    };
    var idModel = "IND_MAT_COD";
    var campiKendoModel = {
        IND_MAT_DES: { editable: false, type: "string" },
        IND_MAT_COD: { editable: false, type: "number" },
        Selected: { editable: false, type: "boolean" },
    };
    var colonneKendoGrid = [
        {
            field: "IND_MAT_DES", title: TraduzioneMultiResx(resxProdottoEditUC, "IndiciDiMaturita", "Indici di Maturità"), filterable: { multi: true, search: true }
        }
    ];

    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: false,
        columnMenu: false,
        reorderable: false,
        excel: false,
        pdf: false,
        groupable: false,
        btnEliminaTuttiFiltri: false
    };

    var parametriPerLettura = null;
    var parametriDataSource = { pagesize: 50 };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDataBound: function (e) {

            var TipoOperazione = parseInt($(Controls.xTipoOperazione).val());
            var elem_cod = parseInt($(Controls.xElem_Cod).val());
            var isMateriaPrima = IsMateriaPrima(elem_cod);
            var proprietario = ($(Controls.xProprietario).val() == "True");


            var ElencoIndiciScelti = CaricaProdottiXGrigliaIndici();
            var grid = $("#prodotto_UC_griglia_indici").data("kendoGrid");
            var ds = grid.dataSource.data();
            var mat_cod = parseInt($(Controls.xMat_Cod).val());
            for (var x = 0; x < ds.length; x++) {
                for (var i = 0; i < ElencoIndiciScelti.length; i++) {
                    if (ds[x].IND_MAT_COD === ElencoIndiciScelti[i].IND_MAT_COD && mat_cod === parseInt(ElencoIndiciScelti[i].Mat_Cod)) {
                        ds[x].Selected = true;
                    }
                }
            }

            //INFO e se non è proprietario ma è Materia Prima: Nascondo  i bottoni Aggiungi e Annulla delle griglie
            //Se Prodotto_Edit_UC_Importato=true tutte le tab in sole visualizzazione tranne "Dati Contabilità" e "Traduzioni"
            if (TipoOperazione === enum_tipoOperazione.Lettura || (isMateriaPrima === true && proprietario === false) || Prodotto_Edit_UC_Importato === true) {
                $("#prodotto_UC_griglia_indici-header-chb").attr('disabled', true);
                var allRows = grid.items();
                $.each(allRows, function (index, value) {
                    $(value).find(".k-checkbox").attr('disabled', true);
                })
            }
        }
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];


    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function kSelezionatoIndice(e) {
    var checked = this.checked;
    row = $(this).parents("tr");
    grid = $("#prodotto_UC_griglia_indici").data("kendoGrid");
    dataItem = grid.dataItem(row);


    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}


function SubmitProdottiXGrigliaIndici(e) {
    var grid = $("#prodotto_UC_griglia_indici").data("kendoGrid");
    var currentData = grid.dataSource.data();
    var RigheScelte = [];
    for (var i = 0; i < currentData.length; i++) {
        RigheScelte.push(currentData[i].toJSON());
    }

    if (RigheScelte.length > 0) {
        for (var x = 0; x < RigheScelte.length; x++) {
            if (RigheScelte[x].Selected === undefined ||
                RigheScelte[x].Selected === null ||
                RigheScelte[x].Selected === "")
                RigheScelte[x].Selected = false;
        }
        righeSelezionateGrid_Indici = kendoEscapeOggetto(RigheScelte);
    }
}

function IsMateriaPrima(elem_cod) {

    if (Prodotto_Edit_UC_Elenco_Categorie_Magazzino === null) {
        Prodotto_Edit_UC_Elenco_Categorie_Magazzino = CaricaCategorieMagazzinoXUtente();
    }

    var categoria = Prodotto_Edit_UC_Elenco_Categorie_Magazzino.find(obj => {
        return obj.Elem_Cod === elem_cod;
    });

    if (categoria != null && categoria != undefined) {
        return categoria.Tabella === "Materie_Prime" || categoria.Tabella === "TipologieSementi";
    }
    else
        return false;

}

// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA TRADUZIONI
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaTraduzioni(IDControllo, Disabilita_Griglia) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var colonna_editabile = false;

    if (Disabilita_Griglia !== undefined && Disabilita_Griglia !== null && Disabilita_Griglia!==null && Disabilita_Griglia === true) {
        UteAbilitatoInsMod = false;
        UteAbilitatoCanc = false;
    }

    if (UteAbilitatoInsMod === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var funzioniCRUD = {
        funzioneRead: CaricaProdottiXGrigliaTraduzioni,
        funzioneSubmit: { funzione: SubmitProdottiXTraduzioni, flagInsert: true, flagUpdate: true, flagDelete: UteAbilitatoCanc },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true
    };
    var idModel = "Key_Traduzioni";
    var campiKendoModel = {
        Key_Traduzioni: { editable: false, type: "string", defaultValue: "" },
        Elem_Cod: { editable: false, type: "number" },
        Lingua_Cod: { editable: colonna_editabile, type: "number" },
        Nome: { editable: colonna_editabile, type: "string" },
        Lingua_Cod_Letta: { editable: false, type: "number", defaultValue: 0},
        Mat_Cod: { editable: colonna_editabile, type: "number" },
        Mat_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        DATA_AGG: { editable: false, type: "date" }
    };
    var colonneKendoGrid = [
        {
            field: "Nome", title: TraduzioneMultiResx(resxProdottoEditUC, "Lingua", "Lingua"), editor: Lingua_DropDownEditor
        },
        {
            field: "Mat_Des", title: TraduzioneMultiResx(resxProdottoEditUC, "Traduzione", "Traduzione")
        },
        {
            field: "DATA_AGG", title: TraduzioneMultiResx(resxProdottoEditUC, "DataAggiunta", "Data Aggiunta"), format: "{0:dd/MM/yyyy}"
        }
    ];

    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditLingua };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];


    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function onEditLingua(e) {
    var tipoOperazione = parseInt($(Controls.xTipoOperazione).val());

    if (tipoOperazione === enum_tipoOperazione.Lettura)
        e.sender.closeCell();

}

function Lingua_DropDownEditor(container) {
    creaDropDownEditor(container, "Nome", "Lingua_Cod", Carica_ddl_lingua_griglia_traduzioni(), changeLingua);
    var grid = $("#prodotto_UC_griglia_traduzioni").data("kendoGrid");
    var model = grid.dataItem(($(container).closest("tr")));

    if (model.isNew() === false) {
        model.Lingua_Cod_Letta = model.Lingua_Cod;
    }

}

function changeLingua(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#prodotto_UC_griglia_traduzioni").data("kendoGrid");

    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Lingua_Cod = dataItem.Lingua_Cod;
    model.Nome = dataItem.Nome;

}

function SubmitProdottiXTraduzioni(options) {

    var grid = $("#prodotto_UC_griglia_traduzioni").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitProdottiXTraduzioni(options.data.created) +
        controllaRigheCompletePerSubmitProdottiXTraduzioni(options.data.updated);

    if (nrErr > 0) {
        if (nrErr == 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxProdottoEditUC, "EsisteRigaIncompletaNellaGrigliaTraduzioni", "Esiste una riga con dati non completi nella griglia Traduzioni."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(resxProdottoEditUC, "EsistonoNRigheIncompleteNellaGrigliaTraduzioni", "Esistono {0} righe con dati non completi nella griglia Traduzioni."), nrErr), "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        erroriSubmitGriglie = true;
        return;
    }

    errMess = controllaRigheValidePerSubmitProdottiXTraduzioni(options.data.created, "");
    errMess = controllaRigheValidePerSubmitProdottiXTraduzioni(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi")

        erroreSubmitGriglia(grid);
        erroriSubmitGriglie = true;
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }


    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Traduzioni = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Traduzioni = kendoEscapeOggetto(updatedRecords);
        righeEliminateGrid_Traduzioni = kendoEscapeOggetto(deletedRecords);
    }

    righeTutteGrid_Traduzioni = kendoEscapeOggetto(righeNonCancellate);
}


function controllaRigheCompletePerSubmitProdottiXTraduzioni(righe) {

    var nrErr = 0;
    for (let x = 0; x < righe.length; x++) {
        let item = righe[x];
        if (item.Nome == "" || item.Lingua_Cod == 0 || item.Lingua_Cod == undefined)
            nrErr++;
    }

    return nrErr;
}

function controllaRigheValidePerSubmitProdottiXTraduzioni(righe, precMess) {

    var errMess = precMess;

    for (let x = 0; x < righe.length; x++) {
        var item = righe[x];
    }

    return errMess;
}


// --------------------------------------------------------------------------------------------------------------------------
// TAB DATI CONTABILITA'
// --------------------------------------------------------------------------------------------------------------------------

//Drop Down IVA change
function Prodotto_Edit_UC_prodotto_UC_ddlCodIva_change(e) {

}

//Drop Down IVA Compensazione change
function Prodotto_Edit_UC_prodotto_UC_ddlCodIvaCompensazione_change(e) {

}

//Drop Down Conto Economico Acquisto change
function Prodotto_Edit_UC_prodotto_UC_ddlContoEconomicoAcquisto_change(e) {

}

//Drop Down Conto Economico Vendita change
function Prodotto_Edit_UC_prodotto_UC_ddlContoEconomicoVendita_change(e) {

}

//Drop Down Conto Patrimoniale Acquisto change
function Prodotto_Edit_UC_prodotto_UC_ddlContoPatrimonialeAcquisto_change(e) {

}

//Drop Down Conto Patrimoniale Vendita change
function Prodotto_Edit_UC_prodotto_UC_ddlContoPatrimonialeVendita_change(e) {

}

//Drop Down Gruppo Merce change
function Prodotto_Edit_UC_prodotto_UC_ddlGruppoMerce_change(e) {

}


function Controlla_Campi_Obbligatori() {
    var CodiceProdotto = $("#txt_prodotto_UC_cod_prod").val();
    var Descrizione = $("#txt_prodotto_UC_Descrizione").val();
    var CodiceEsterno = $("#txt_prodotto_UC_cod_est").val();

    if ((Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === false) &&
        (CodiceProdotto === null || CodiceProdotto === "" || CodiceProdotto == undefined))
        return false;

    if ((Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === true) &&
        (CodiceEsterno === null || CodiceEsterno === "" || CodiceEsterno == undefined))
        return false;

    if (Descrizione === null || Descrizione === "" || Descrizione == undefined)
        return false;

    if (nascondi_riepilogo_error()) {
        return false;
    }

    return true;

}


function Valida_Salvataggio(nuovo, esci) {

    erroriSubmitGriglie = false;

    if (!Controlla_Campi_Obbligatori())
        return;


    if (tabStripAperti.includes("a_tab_prodotto_UC_altri_dati")) {

        if ($(xDatiImmagineCaricata).val() !== "") {

            let datiImmagine = JSON.parse($(xDatiImmagineCaricata).val());

            //Controllo che l'immagine caricata non sia un virus
            let rilevatoVirus = "";

            if (datiImmagine.Estensione !== "" && datiImmagine.Immagine !== "") {

                let Immagine_convertita = btoa(datiImmagine.Immagine);

                let Estensione_Immagine_Minuscolo = datiImmagine.Estensione.toLowerCase();

                rilevatoVirus = checkVirus(Estensione_Immagine_Minuscolo, Immagine_convertita, objP_super_server);
            }


            if (rilevatoVirus !== "") {

                MessaggioErrore_Bootstrap(rilevatoVirus, "DIV_Messaggi");

                return;

            } else if (rilevatoVirus === "") {

                //Controllo che l'estensione delle immagini caricate sia supportata
                if (TipidiImmagineCaricabili.includes(datiImmagine.Estensione.toLowerCase()) === false) {
                    let Msg_Errore = TraduzioneMultiResx(resxProdottoEditUC, "EstensioneImmagineNonSupportata", "Estensione dell'Immagine caricata non supportata.") + "<br> " +
                        TraduzioneMultiResx(resxProdottoEditUC, "EstensioniSupportate", "Le estensioni supportate sono:") + " <strong>" + TipidiImmagineCaricabili.join(",") + "</strong>.";

                    MessaggioErrore_Bootstrap(Msg_Errore, "DIV_Messaggi");

                    return;
                }
            }
        }
    }

    submit_Dati_Gliglie();
    if (erroriSubmitGriglie)
        return;

    let valoreRitorno = submit_Effettivo();

    if (!valoreRitorno.Errori) {
        let tipoOperazione = parseInt($(Controls.xTipoOperazione).val());

        if (esci) {
            var paginaRedirect = $(Controls.xPaginaRedirect).val();
            if (winProdotto) {

                if ((window.parent.$("#windowNuovoProdotto").data("kendoWindow") === undefined ||
                    window.parent.$("#windowNuovoProdotto").data("kendoWindow") === null) &&
                    (window.parent.$("#dialog") !== undefined &&
                     window.parent.$("#dialog") !== null)) {

                    window.parent.$("#dialog").dialog("close")
                } else {
                    window.parent.$("#windowNuovoProdotto").data("kendoWindow").close();
                }

            }
            else if (paginaRedirect !== undefined && paginaRedirect !== "") {
                let parametroVisibilita = riportaParametroVisibilita();
                if (paginaRedirect.indexOf("?") >= 0) {
                    parametroVisibilita = parametroVisibilita.replace("?", "&")
                }
                window.location.href = paginaRedirect + riportaParametroVisibilita();
            }
        }

        if (nuovo) {
            var elem_cod = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_categ_prod"));
            var redirect_Url = Link_Redirect_Prodotto(elem_cod);
            window.location = redirect_Url;
        }


        if ((tipoOperazione === enum_tipoOperazione.Scrittura || tipoOperazione === enum_tipoOperazione.Duplica) && esci === false && nuovo === false) {

            // Chiesto salvataggio senza uscita
            // Aggiorno xMatCod con nuovo mat_cod se ero in inserimento di un nuovo prodotto.
            $(Controls.xMat_Cod).val(valoreRitorno.Mat_Cod);

            $(Controls.xTipoOperazione).val(enum_tipoOperazione.Modifica);
            xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione;

            //Faccio il redirect alla pagina della modifica del prodotto appena creato
            var param = kendo.stringify(
                {
                    piva: $(Controls.xPiva_Corrente).val(),
                    mat_cod: $(Controls.xMat_Cod).val(),
                    elem_cod: $(Controls.xElem_Cod).val(),
                    prodotto_des: $("#txt_prodotto_UC_Descrizione").val(),
                    pro_cod: 0,
                    duplica: false,
                    proprietario: true,
                    sa_cod: $(xSa_Cod).val()
                });

            $.ajax({
                type: 'POST',
                url: '../MenuAnagrafica/MenuBs_Anagrafica.aspx/EditProdotto',
                data: param,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    window.location = r.d + (winProdotto ? "&win=1" : "");
                }
            });
        }

        //Se sono in Modifica e clicco su Salva ricarico le griglie modificate
        if (tipoOperazione === enum_tipoOperazione.Modifica && esci === false && nuovo === false) {

            if (tabStripAperti.includes("a_tab_prodotto_UC_parametri_qualitativi")) {
                righeSelezionateGrid_Calibri = "";
                righeSelezionateGrid_Indici = "";
                $("#prodotto_UC_griglia_calibri").data('kendoGrid').dataSource.read();
                $("#prodotto_UC_griglia_calibri").data("kendoGrid").refresh();
                $("#prodotto_UC_griglia_indici").data('kendoGrid').dataSource.read();
                $("#prodotto_UC_griglia_indici").data("kendoGrid").refresh();
            }

            if (tabStripAperti.includes("a_tab_prodotto_UC_storico_prezzi")) {
                righeInseriteGrid_Prezzi = "";
                righeModificateGrid_Prezzi = "";
                righeEliminateGrid_Prezzi = "";
                righeTutteGrid_Prezzi = "";

                $("#prodotto_UC_griglia_storico_prezzi").data('kendoGrid').dataSource.read();
                $("#prodotto_UC_griglia_storico_prezzi").data("kendoGrid").refresh();
            }

            if (tabStripAperti.includes("a_tab_prodotto_UC_traduzioni")) {
                righeInseriteGrid_Traduzioni = "";
                righeModificateGrid_Traduzioni = "";
                righeEliminateGrid_Traduzioni = "";
                righeTutteGrid_Traduzioni = "";

                $("#prodotto_UC_griglia_traduzioni").data('kendoGrid').dataSource.read();
                $("#prodotto_UC_griglia_traduzioni").data("kendoGrid").refresh();
            }
        }
    }

}

function riportaParametroVisibilita() {
    const urlParams = new URLSearchParams(window.location.search);
    const myParam = urlParams.get('visibilita');
    let returnString = ""
    if (myParam != undefined) {
        returnString = "?visibilita=" + myParam;
    }
    return returnString;
}

function prodotto_Edit_UC_ValidaxSubmit(nuovo, esci) {

    let elem_cod = parseInt($(Controls.xElem_Cod).val());

    let mostraDialogConferma = false;
    let content = [];
    let listaAzioniConferma = [];

    let tipoOperazione = parseInt($(Controls.xTipoOperazione).val());
    let impGruppoMerceControllo = $(cIdGruppoMerceControllo).val();

    if (tipoOperazione === enum_tipoOperazione.Scrittura && impGruppoMerceControllo == true) {
        let kddlGruppoMerce = KendoDDL("prodotto_UC_ddlGruppoMerce");
        // L'elemento kendo potrebbe essere undefined perché l'utente non ha cliccato sulla tab "dati contabilita"
        if (kddlGruppoMerce === undefined || (kddlGruppoMerce !== undefined && kddlGruppoMerce.dataSource.data().length > 1 && parseInt(kddlGruppoMerce.value()) === 0)) {
            mostraDialogConferma = true;
            content.push(TraduzioneMultiResx(resxProdottoEditUC, "NonInseritoGruppoMerce", "Non è stato inserito il gruppo merce"));
            //listaAzioniConferma.push(function (e) {});
        }
    }

    switch (elem_cod) {

        case categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI:
        case categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI:

            if (tabStripAperti.includes("a_tab_prodotto_UC_configurazione") &&
                (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) &&
                Prodotto_Edit_UC_Importato === false) {

                let Flag_Imballaggio = $("#chk_prodotto_UC_imballaggio").is(':checked');
                let Flag_Contenitore = $("#chk_prodotto_UC_contenitore").is(':checked');
                let Flag_Confezione = $("#chk_prodotto_UC_confezione").is(':checked');
                let Flag_CompLotto = $("chk_prodotto_UC_composizioneLotto").is(':checked');
                let Flag_Udm_Cod_Extra = $("#chk_prodotto_UC_Udm_Cod_Extra").is(':checked');
                let Flag_UtilizzoCA = $("#chk_prodotto_UC_utilizzoCA").is(':checked');
                let Confezione_Cod = Get_KendoDDLValue("ddl_prodotto_UC_confezioni");
                let Qta_Contenitore = Get_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore");
                let filtroSpecie = Get_MultiselString("multisel_prodotto_UC_specie_dett_beni");
                let filtroVarieta = Get_MultiselString("multisel_prodotto_UC_varieta_dett_beni");

                if ((Mostra_chkconfezione === true || Impostazione_utenteTabConfigurazione.Mostra_ChkContenitore === true ||
                    Impostazione_utenteTabConfigurazione.Mostra_ChkImballaggio === true) &&
                    (Flag_Udm_Cod_Extra === false && (Flag_Imballaggio === true || Flag_Contenitore === true || Flag_Confezione === true ||
                        (Confezione_Cod !== "" && Confezione_Cod !== 0) ||
                        (Qta_Contenitore !== "" && Qta_Contenitore !== null && Qta_Contenitore !== 0) ||
                        (filtroSpecie !== undefined && filtroSpecie !== null && filtroSpecie !== "") ||
                        (filtroVarieta !== undefined && filtroVarieta !== null && filtroVarieta !== "")))) {

                    mostraDialogConferma = true;
                    content.push(TraduzioneMultiResx(resxProdottoEditUC, "ImpostazionePesiNecessariaPerDettagliConfezionamento", "Se non viene selezionato 'Impostazione Pesi', perderai i dati di 'Dettagli Beni Di Confezionamento'"));
                    listaAzioniConferma.push(function (e) {
                        //Azzero la parte dei dettagli beni di confezionamento
                        //let imbagestito = ParametriQualitativiGestitiPerCheckBoxBeniConf.some(item => parseInt(item.Tabella_ID) === enum_OmniTabelle.ot_IMBALLAGGI_FF);
                        //let contgestito = ParametriQualitativiGestitiPerCheckBoxBeniConf.some(item => parseInt(item.Tabella_ID) === enum_OmniTabelle.ot_CONTENITORI_FF);

                        let imbagestito = true;
                        let contgestito = true;

                        if (Flag_Imballaggio && imbagestito === true && (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI)) {
                            $("#chk_prodotto_UC_imballaggio").prop("checked", false);
                            $("#chk_prodotto_UC_imballaggio").attr('disabled', false);
                        }
                        if (Flag_Contenitore && contgestito === true && (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI)) {
                            $("#chk_prodotto_UC_contenitore").prop("checked", false);
                            $("#chk_prodotto_UC_contenitore").attr('disabled', false);
                            Set_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore", 0);
                        }
                        if (Flag_Confezione && Mostra_chkconfezione === true) {
                            $("#chk_prodotto_UC_confezione").prop("checked", false);
                            $("#chk_prodotto_UC_contenitore").attr('disabled', false);
                        }
                        if (Flag_CompLotto && imbagestito === true && elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) {
                            $("#chk_prodotto_UC_composizioneLotto").prop("checked", false);
                            $("#chk_prodotto_UC_composizioneLotto").attr('disabled', false);
                        }

                        Set_MultiselValue("multisel_prodotto_UC_specie_dett_beni", "");
                        Set_MultiselValue("multisel_prodotto_UC_varieta_dett_beni", "");
                        Set_KendoDDLValue("ddl_prodotto_UC_confezioni", "");

                        Nascondi_Mostra_Filtro_Specie_Varieta();
                    });
                }
            }
            break;

        case categorieProdotti.TRASFORMATI_VEGETALI:

            //non deve essere più obbligatorio il collegamento a un prodotto OMNI;
            //se però il F & F è attivo ed esistono prodotti OMNI della stessa specie / varietà 
            //chiedere conferma prima di salvare senza prodotto 

            let ddl_prodotto_base = $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList");

            if (elem_cod === categorieProdotti.TRASFORMATI_VEGETALI && Modulo_FF === true && Is_OMNI === false &&
                (ddl_prodotto_base !== undefined && parseInt(Get_KendoDDLValue("ddl_prodotto_UC_Prodottobase")) === 0 &&
                    ddl_prodotto_base.dataSource.data().length > 1)) {

                mostraDialogConferma = true;
                content.push(TraduzioneMultiResx(resxProdottoEditUC, "NonInseritoProdottoBase", "Non è stato scelto il Prodotto Base di Riferimento per il Trasformato Vegetale"));
                //listaAzioniConferma.push(function (e) {});
            }
            break;
    }


    if (!mostraDialogConferma) {
        Valida_Salvataggio(nuovo, esci);
    }
    else {
        let container = document.getElementById("prodotto_Edit_UC_modifyArea");
        let id_dialog = creaNewRowDiv("dialog_conferma_salva");
        container.appendChild(id_dialog);
        
        let strContent = "";
        if (content.length === 1) {
            strContent = TraduzioneMultiResx(resxProdottoEditUC, "RilevatoSingoloMessaggio", "È stato rilevato il seguente messaggio");
        }
        else {
            strContent = TraduzioneMultiResx(resxProdottoEditUC, "RilevatiMultipliMessaggi", "Sono stati rilevati i seguenti messaggi");
        }

        strContent += "<br/><br/>- " + content.join("<br/>-") + "<br/><br/>" + TraduzioneMultiResx(resxProdottoEditUC, "ProcedereComunqueSalvataggio", "Procedere comunque al salvataggio?");

        $("#" + id_dialog.id).kendoDialog({
            title: TraduzioneMultiResx(resxProdottoEditUC, "ConfermaSalvataggio", "Conferma Salvataggio"),
            closable: false,
            modal: {
                preventScroll: true
            },
            content: strContent,
            actions: [{
                text: TraduzioneMultiResx(resxProdottoEditUC, "Annulla", "Annulla"),
                primary: true,
            },
            {
                text: TraduzioneMultiResx(resxProdottoEditUC, "Salva", "Salva"),
                action: function (e) {
                    for (let funzioneConferma of listaAzioniConferma) {
                        funzioneConferma(e);
                    }
                    
                    Valida_Salvataggio(nuovo, esci);
                },
            }]
        });
    }

}

//Se si clicca sul pulsante di annulla si ritorna nella pagine del menu di ricerca
function prodotto_Edit_UC_Annulla() {
    var paginaRedirect = $(Controls.xPaginaRedirect).val();
    if (winProdotto) {

        if ((window.parent.$("#windowNuovoProdotto").data("kendoWindow") === undefined ||
            window.parent.$("#windowNuovoProdotto").data("kendoWindow") === null) &&
            (window.parent.$("#dialog") !== undefined &&
            window.parent.$("#dialog") !== null)) {

            window.parent.$("#dialog").dialog("close")
        } else {
            window.parent.$("#windowNuovoProdotto").data("kendoWindow").close();
        }
    }
    else if (paginaRedirect !== undefined && paginaRedirect !== "") {
        let parametroVisibilita = riportaParametroVisibilita();
        if (paginaRedirect.indexOf("?") >= 0) {
            parametroVisibilita = parametroVisibilita.replace("?", "&")
        }
        window.location.href = paginaRedirect + riportaParametroVisibilita();
    }
}

function submit_Dati_Gliglie() {


    righeInseriteGrid_Prezzi = "";
    righeModificateGrid_Prezzi = "";
    righeEliminateGrid_Prezzi = "";
    righeTutteGrid_Prezzi = "";

    righeInseriteGrid_Traduzioni = "";
    righeModificateGrid_Traduzioni = "";
    righeEliminateGrid_Traduzioni = "";
    righeTutteGrid_Traduzioni = "";

    var griglia_prezzi = $("#prodotto_UC_griglia_storico_prezzi").data("kendoGrid");
    if (griglia_prezzi != null && griglia_prezzi != undefined)
        griglia_prezzi.saveChanges();

    var griglia_traduzioni = $("#prodotto_UC_griglia_traduzioni").data("kendoGrid");
    if (griglia_traduzioni != null && griglia_traduzioni != undefined)
        griglia_traduzioni.saveChanges();

    //Parametri Qualitativi
    var griglia_calibri = $("#prodotto_UC_griglia_calibri").data("kendoGrid");
    if (griglia_calibri != null && griglia_calibri != undefined)
        griglia_calibri.saveChanges();


    var griglia_indici = $("#prodotto_UC_griglia_indici").data("kendoGrid");
    if (griglia_indici != null && griglia_indici != undefined)
        griglia_indici.saveChanges();


    var griglia_alias = $("#prodotto_UC_griglia_alias").data("kendoGrid");
    if (griglia_alias != null && griglia_alias != undefined)
        griglia_alias.saveChanges();
}

function erroreSubmitGriglia(grid) {

    if (grid == null || grid == undefined)
        return;

    var dsSort = [];
    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}

function Ottieni_Opzione_Materia_Prima(chiave) {
    return GetPropertyFromJson($(Controls.xOpzioni).val(), chiave);
}

function Configuratore_Maschera_MateriaPrima(Elem_Cod_CategScelta) {

    $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").check(false);

    $("#cb_prodotto_UC_alias").data("kendoSwitch").check(false);

    switch (Elem_Cod_CategScelta) {
        case categorieProdotti.ALTRE_RISORSE:
            $("#switch_prodotto_UC_componi_descrizione").hide();
            $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(false);
            $(".categoria_risorsa").show();
            $(".linea_produzione").show();
            $(".dati_colturali").show();
            $("#sementi_materiale").hide();
            $(".specie_vegetale").hide();
            $(".varieta_colturale").hide();
            $(".tipologia_varietale").hide();
            $(".specie_animale").hide();
            $(".indirizzo_produttivo").hide();
            $(".razza_animale").hide();
            //$(".dettagli").hide();
            $(".ddl_prodotto_UC_final_prod").hide();
            $(".regolamento").show();
            $("#prodotto_base").hide();
            $(".ditta_di_provenienza").hide();
            //Tab Configurazione
            $(".bene_conf").hide();
            $(".Qta_Contenitore_Conf").show();
            $("#filtri_dett_beni_conf_veg").hide();
            $("#titolo_dati_cul").hide();
            $("#titolo_dati_zoo").hide();
            $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            break;
        case categorieProdotti.MATERIE_PRIME_VEGETALI:
            $("#switch_prodotto_UC_componi_descrizione").hide();
            $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(false);
            $(".categoria_risorsa").show();
            $(".linea_produzione").show();
            $("#titolo_dati_cul").hide();
            $("#titolo_dati_zoo").hide();
            $(".dati_colturali").show();
            //$(".dettagli").hide();
            $("#sementi_materiale").hide();
            $(".specie_vegetale").hide();
            $(".varieta_colturale").hide();
            $(".tipologia_varietale").hide();
            $(".ditta_di_provenienza").hide();
            $("#prodotto_base").hide();
            $(".specie_animale").hide();
            $(".indirizzo_produttivo").hide();
            $(".razza_animale").hide();
            $(".regolamento").show();
            $(".ddl_prodotto_UC_final_prod").hide();
            //Tab Configurazione
            $(".bene_conf").hide();
            $(".Qta_Contenitore_Conf").show();
            $("#filtri_dett_beni_conf_veg").hide();
            $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            break;
        case categorieProdotti.CARBURANTI:
        case categorieProdotti.MATERIE_PRIME_ANIMALI:
            $("#switch_prodotto_UC_componi_descrizione").hide();
            $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(false);
            $("#titolo_dati_cul").hide();
            $("#titolo_dati_zoo").hide();
            $(".dati_colturali").hide();
            //$(".dettagli").hide();
            $(".ditta_di_provenienza").hide();
            if (categorieProdotti.MATERIE_PRIME_ANIMALI === Elem_Cod_CategScelta) {
                $(".categoria_risorsa").hide();
                $(".linea_produzione").hide();
            }
            else {
                Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(Elem_Cod_CategScelta);
                $(".categoria_risorsa").show();
                $(".linea_produzione").show();
            }
            //Tab Configurazione
            $(".bene_conf").hide();
            $(".Qta_Contenitore_Conf").show();
            $("#prodotto_base").hide();
            $("#filtri_dett_beni_conf_veg").hide();
            $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            break;
        case categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI:
        case categorieProdotti.SEMILAVORATI_PRODUZIONE_ANIMALE:
        case categorieProdotti.TRASFORMATI_ANIMALI:
        case categorieProdotti.MANGIMI:


            if (Elem_Cod_CategScelta === categorieProdotti.SEMILAVORATI_PRODUZIONE_ANIMALE ||
                Elem_Cod_CategScelta === categorieProdotti.TRASFORMATI_ANIMALI) {

                //Aggiungo gli asterisci alle label dei campi obbligatori
                //############################################
                $("#lbl_prodotto_UC_specie_anim").html("");
                $("#lbl_prodotto_UC_specie_anim").html(TraduzioneMultiResx(resxProdottoEditUC, "SpecieAnimale", "Specie Animale") + " *:");

                if (Elem_Cod_CategScelta === categorieProdotti.SEMILAVORATI_PRODUZIONE_ANIMALE) {
                    $("#lbl_prodotto_UC_indi_produt").html("");
                    $("#lbl_prodotto_UC_indi_produt").html(TraduzioneMultiResx(resxProdottoEditUC, "IndirizzoProduttivo", "Indirizzo Produttivo") + " *:");
                    $("#lbl_prodotto_UC_razza_anim").html("");
                    $("#lbl_prodotto_UC_razza_anim").html(TraduzioneMultiResx(resxProdottoEditUC, "Razza", "Razza") + " *:");
                }

                //############################################
            }


            if (Elem_Cod_CategScelta === categorieProdotti.TRASFORMATI_ANIMALI) {
                $(".categoria_risorsa").show();
            }
            else {
                $(".categoria_risorsa").hide();
            }
            $(".linea_produzione").hide();
            $(".dati_colturali").show();
            $("#sementi_materiale").hide();
            $(".specie_vegetale").hide();
            $(".varieta_colturale").hide();
            $(".tipologia_varietale").hide();
            $(".ditta_di_provenienza").hide();
            $("#prodotto_base").hide();
            $(".specie_animale").show();
            $(".indirizzo_produttivo").show();
            $(".razza_animale").show();
            $(".regolamento").show();
            $(".ddl_prodotto_UC_final_prod").hide();
            //if (categorieProdotti.TRASFORMATI_ANIMALI === Elem_Cod_CategScelta) {
            //    $(".dettagli").show();
            //}
            //else {
            //    $(".dettagli").hide();
            //}

            $("#titolo_dati_cul").hide();

            //Tab Configurazione
            if (categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI === Elem_Cod_CategScelta ||
                categorieProdotti.MANGIMI === Elem_Cod_CategScelta) {

                if (categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI === Elem_Cod_CategScelta) {
                    $(".bene_conf").show();
                    $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ImpostazionePesi", "Impostazione Pesi"));
                    $(".Qta_Contenitore_Conf").hide();
                }
                else if (categorieProdotti.MANGIMI === Elem_Cod_CategScelta) {
                    $(".bene_conf").hide();
                    $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
                    $(".Qta_Contenitore_Conf").show();
                }
                $(".dati_colturali").hide();
                $(".specie_animale").hide();
                $(".indirizzo_produttivo").hide();
                $(".razza_animale").hide();
                $(".regolamento").hide();
                $("#titolo_dati_zoo").hide();
                $("#switch_prodotto_UC_componi_descrizione").hide();
                $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(false);
            }
            else {
                $(".bene_conf").hide();
                $(".Qta_Contenitore_Conf").show();
                $("#titolo_dati_zoo").show();
                $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
                $("#switch_prodotto_UC_componi_descrizione").show();
                $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(true);
            }
            $("#filtri_dett_beni_conf_veg").hide();
            break;
        case categorieProdotti.SEMENTI:
            //Aggiungo gli asterisci alle label dei campi obbligatori
            //############################################
            $("#lbl_prodotto_UC_sementi_materiale").html("");
            $("#lbl_prodotto_UC_sementi_materiale").html(TraduzioneMultiResx(resxProdottoEditUC, "SementeMaterialeVivaistico", "Sementi e materiale vivaistico") + " *:");

            $("#lbl_prodotto_UC_specie_veg").html("");
            $("#lbl_prodotto_UC_specie_veg").html(TraduzioneMultiResx(resxProdottoEditUC, "SpecieVegetale", "Specie Vegetale") + " *:");

            $("#lbl_prodotto_UC_varieta").html("");
            $("#lbl_prodotto_UC_varieta").html(TraduzioneMultiResx(resxProdottoEditUC, "VarietàColturale", "Varietà Colturale") + " *:");
            //############################################

            $("#switch_prodotto_UC_componi_descrizione").show();
            $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(true);
            $(".categoria_risorsa").show();
            $(".linea_produzione").show();
            $(".specie_animale").hide();
            $(".indirizzo_produttivo").hide();
            $(".razza_animale").hide();
            $(".dati_colturali").show();
            $("#sementi_materiale").show();
            $(".specie_vegetale").show();
            $(".varieta_colturale").show();
            $(".tipologia_varietale").show();
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);
            //$(".dettagli").show();
            $(".regolamento").show();
            $(".ddl_prodotto_UC_final_prod").show();
            $("#prodotto_base").hide();
            $(".ditta_di_provenienza").show();
            //Tab Configurazione
            $(".bene_conf").hide();
            $(".Qta_Contenitore_Conf").show();
            $("#filtri_dett_beni_conf_veg").hide();
            $("#titolo_dati_zoo").hide();
            $("#titolo_dati_cul").show();
            $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            break;
        case categorieProdotti.CONFEZIONI_PRODOTTI:
        case categorieProdotti.ALTRI_BENI_AMMORTIZZABILI:
            if (categorieProdotti.ALTRI_BENI_AMMORTIZZABILI === Elem_Cod_CategScelta) {
                $(".categoria_risorsa").show();
                $(".linea_produzione").show();
                $("#titolo_dati_cul").hide();
                $(".dati_colturali").hide();
            }
            else {
                $(".categoria_risorsa").hide();
                $(".linea_produzione").hide();
                $("#titolo_dati_cul").hide();
                $(".dati_colturali").hide();
            }
            $("#switch_prodotto_UC_componi_descrizione").hide();
            $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(false);
            //$(".dettagli").show();
            $("#titolo_dati_zoo").hide();
            $("#sementi_materiale").hide();
            $(".ditta_di_provenienza").hide();
            //Tab Configurazione
            $(".bene_conf").hide();
            $(".Qta_Contenitore_Conf").show();
            $("#prodotto_base").hide();
            $("#filtri_dett_beni_conf_veg").hide();
            $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            break;
        case categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI:
        case categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE:

            if (Elem_Cod_CategScelta===categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE) {
                //Aggiungo gli asterisci alle label dei campi obbligatori
                //############################################
                $("#lbl_prodotto_UC_specie_veg").html("");
                $("#lbl_prodotto_UC_specie_veg").html(TraduzioneMultiResx(resxProdottoEditUC, "SpecieVegetale", "Specie Vegetale") + " *:");

                $("#lbl_prodotto_UC_varieta").html("");
                $("#lbl_prodotto_UC_varieta").html(TraduzioneMultiResx(resxProdottoEditUC, "VarietàColturale", "Varietà Colturale") + " *:");
                //############################################
            }


            $(".categoria_risorsa").show();
            $(".linea_produzione").show();
            $("#titolo_dati_cul").show();
            $("#titolo_dati_zoo").hide();
            $(".dati_colturali").show();
            $("#sementi_materiale").hide();
            //$(".dettagli").hide();
            $(".regolamento").show();
            $(".ddl_prodotto_UC_final_prod").show();
            $(".specie_vegetale").show();
            $(".varieta_colturale").show();
            $(".tipologia_varietale").show();
            $(".specie_animale").hide();
            $(".indirizzo_produttivo").hide();
            $(".razza_animale").hide();
            $("#prodotto_base").hide();
            $("#ddl_prodotto_UC_tipo_varietale").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_varieta").data("kendoDropDownList").enable(false);
            $("#ddl_prodotto_UC_specie_veg").data("kendoDropDownList").enable(true);
            $(".ditta_di_provenienza").show();
            //Tab Configurazione
            if (categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI === Elem_Cod_CategScelta) {
                $(".bene_conf").show();
                $(".Qta_Contenitore_Conf").hide();
                $("#filtri_dett_beni_conf_veg").show();
                $(".dati_colturali").hide();
                $("#switch_prodotto_UC_componi_descrizione").hide();
                $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(false);
                $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ImpostazionePesi", "Impostazione Pesi"));
            }
            else {
                $(".bene_conf").hide();
                $(".Qta_Contenitore_Conf").show();
                $("#switch_prodotto_UC_componi_descrizione").show();
                $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(true);
                $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            }
            break;
        case categorieProdotti.RICAMBI:
            $("#switch_prodotto_UC_componi_descrizione").hide();
            $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(false);
            $(".categoria_risorsa").show();
            $(".linea_produzione").show();
            $("#titolo_dati_cul").show();
            $("#titolo_dati_zoo").hide();
            $(".dati_colturali").hide();
            $(".ditta_di_provenienza").show();
            //$(".dettagli").hide();
            //Tab Configurazione
            $(".bene_conf").hide();
            $(".Qta_Contenitore_Conf").show();
            $("#prodotto_base").hide();
            $("#filtri_dett_beni_conf_veg").hide();
            $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            break;
        case categorieProdotti.SERVIZI_PROFESSIONALI:
            $("#switch_prodotto_UC_componi_descrizione").show();
            $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(true);
            $(".categoria_risorsa").show();
            $(".linea_produzione").show();
            $("#titolo_dati_cul").show();
            $("#titolo_dati_zoo").hide();
            $(".dati_colturali").show();
            $("#sementi_materiale").hide();
            //$(".dettagli").hide();
            $(".regolamento").hide();
            $(".ddl_prodotto_UC_final_prod").hide();
            $(".specie_vegetale").show();
            $(".varieta_colturale").show();
            $(".tipologia_varietale").show();
            $(".specie_animale").hide();
            $(".indirizzo_produttivo").hide();
            $(".razza_animale").hide();
            $("#prodotto_base").hide();
            $(".ditta_di_provenienza").hide();
            //Tab Configurazione
            $(".bene_conf").hide();
            $(".Qta_Contenitore_Conf").show();
            $("#filtri_dett_beni_conf_veg").hide();
            $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            break;
        case categorieProdotti.TRASFORMATI_VEGETALI:

            //Aggiungo gli asterisci alle label dei campi obbligatori
            //############################################
            $("#lbl_prodotto_UC_specie_veg").html("");
            $("#lbl_prodotto_UC_specie_veg").html(TraduzioneMultiResx(resxProdottoEditUC, "SpecieVegetale", "Specie Vegetale") + " *:");

            $("#lbl_prodotto_UC_varieta").html("");
            $("#lbl_prodotto_UC_varieta").html(TraduzioneMultiResx(resxProdottoEditUC, "VarietàColturale", "Varietà Colturale") + " *:");
            //############################################

            $("#switch_prodotto_UC_componi_descrizione").show();
            $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").enable(true);
            $(".categoria_risorsa").show();
            $(".linea_produzione").show();
            $("#titolo_dati_cul").show();
            $("#titolo_dati_zoo").hide();
            $(".dati_colturali").show();
            $("#sementi_materiale").hide();
            $(".specie_vegetale").show();
            $(".varieta_colturale").show();
            $(".tipologia_varietale").show();
            $(".ditta_di_provenienza").show();
            if (Modulo_FF === true && Is_OMNI === false) {
                $("#prodotto_base").show();
            }
            else {
                $("#prodotto_base").hide();
            }
            $(".specie_animale").hide();
            $(".indirizzo_produttivo").hide();
            $(".razza_animale").hide();
            $(".regolamento").show();
            //$(".dettagli").show();
            $(".ddl_prodotto_UC_final_prod").show();
            //Tab Configurazione
            $(".bene_conf").hide();
            $(".Qta_Contenitore_Conf").show();
            $("#filtri_dett_beni_conf_veg").hide();
            $('label[for="chk_prodotto_UC_Udm_Cod_Extra"]').html(TraduzioneMultiResx(resxProdottoEditUC, "ProdottoConfezionato", "Prodotto Confezionato"));
            break;
        default:
            $("#multisel_prodotto_UC_varieta_dett_beni").hide();
    }
}

//Carica l'interfaccia della tab Configurazione
function CaricaMascheraConfigurazione() {
    //Leggo le impostazioni utente per poter abilitare la DDl Categoria e mostrae i checkBox Imaballaggi e Confezioni
    Impostazione_utenteTabConfigurazione = Leggi_ImpostazioniUtenteTabConfigurazione();
    var elem_cod = parseInt($(Controls.xElem_Cod).val());
    //var imbagestito = ParametriQualitativiGestitiPerCheckBoxBeniConf.some(item => parseInt(item.Tabella_ID) === enum_OmniTabelle.ot_IMBALLAGGI_FF);
    //var contgestito = ParametriQualitativiGestitiPerCheckBoxBeniConf.some(item => parseInt(item.Tabella_ID) === enum_OmniTabelle.ot_CONTENITORI_FF);

    //Nel caso in cui sono in Un Bene di confezionamento lo mostro sempre imballaggio, contenitore e composizione del lotto
    var imbagestito = true;
    var contgestito = true;
    var complottoGestito = true;
    var utilizzoCA = true;

    //per il momento gestisco (successivamente) questi checkbox senza configurazioni utente, mostrandole 
    //solo se si tratta di: 
    //BENI_CONFEZIONAMENTO_VEGETALI
    $("#checkbox_composizioneLotto").hide();
    //ALTRE_RISORSE
    $("#checkbox_utilizzoCA").hide();


    if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Scrittura)
        $(".Qta_Contenitore").hide();


    //Mostro le checkbox confezione,imballaggio,contenitore solo se sono in un bene di confezionamento ed è
    //anche gestito, e se è gestito ed è anche presente nella tabella Otabelle_Parametri lo checko altrimenti
    //rimane deckeckato.
    if ((elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI)) {

        //Controllo prima se è gestita la confezione se non è gestita la nascondo altrimenti lo mostro
        if (Mostra_chkconfezione === true) {
            $("#checkbox_confezione").show();
            if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura) {
                let Confezioni = RicercaValoriParametriQualitativi(enum_OmniTabelle.ot_CONFEZIONI_FF, $(cIdPiva).val(), false);

                if (Confezioni !== undefined && Confezioni !== null && Confezioni.length > 0 &&
                    $(Controls.xMat_Cod).val() !== undefined && $(Controls.xMat_Cod).val() !== null &&
                    $(Controls.xMat_Cod).val() !== "" && $(Controls.xMat_Cod).val() !== "0" && $(Controls.xMat_Cod).val() !== 0) {

                    let trovato = Confezioni.some(item => item.mat_cod === parseInt($(Controls.xMat_Cod).val()));

                    if (trovato !== undefined && trovato === true) {
                        $("#chk_prodotto_UC_confezione").prop("checked", true);
                    } else {
                        $("#chk_prodotto_UC_confezione").prop("checked", false);
                    }

                } else {
                    $("#chk_prodotto_UC_confezione").prop("checked", false);
                }

            }
        }
        else {
            $("#checkbox_confezione").hide();
            $("#chk_prodotto_UC_confezione").prop("checked", false);
        }

        //Nel caso in cui sono in Un Bene di confezionamento lo mostro sempre
        //Controllo prima se è gestito l'Imballaggio se non è gestito lo nascondo altrimenti lo mostro
        if (imbagestito !== undefined && imbagestito === true) {

            if (Impostazione_utenteTabConfigurazione !== null && Impostazione_utenteTabConfigurazione.Mostra_ChkImballaggio === true) {
                $("#checkbox_imballaggio").show();
            }
            else {
                $("#checkbox_imballaggio").hide();
            }

            if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura && Prodotto_Materia_Prima_Letto !== undefined &&
                Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== "") {

                if (Prodotto_Materia_Prima_Letto.ChkImballaggio > 0)
                    $("#chk_prodotto_UC_imballaggio").prop("checked", true);
                else
                    $("#chk_prodotto_UC_imballaggio").prop("checked", false);
            }

        }
        else {
            $("#checkbox_imballaggio").hide();
        }

        //Nel caso in cui sono in Un Bene di confezionamento lo mostro sempre
        //Controllo prima se è gestito il contenitore se non è gestito lo nascondo altrimenti lo mostro
        if (contgestito !== undefined && contgestito === true) {

            if (Impostazione_utenteTabConfigurazione !== null && Impostazione_utenteTabConfigurazione.Mostra_ChkContenitore === true) {
                $("#checkbox_contenitore").show();
            }
            else {
                $("#checkbox_contenitore").hide();
            }

            if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura && Prodotto_Materia_Prima_Letto !== undefined &&
                Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== "") {

                if (Prodotto_Materia_Prima_Letto.ChkContenitore > 0) {
                    $("#chk_prodotto_UC_contenitore").prop("checked", true);
                    $(".Qta_Contenitore").show();
                }
                else {
                    $("#chk_prodotto_UC_contenitore").prop("checked", false);
                    $(".Qta_Contenitore").hide();
                }
            }
        }
        else {
            $("#checkbox_contenitore").hide();
            $(".Qta_Contenitore").hide();
        }

        //Nel caso in cui sono in un Bene di confezionamento lo mostro sempre
        //Controllo prima se è gestita la composizione lotto (se non è gestita lo nascondo, altrimenti lo mostro)
        if (complottoGestito !== undefined && complottoGestito === true && elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) {

            if (Impostazione_utenteTabConfigurazione !== null) {
                $("#checkbox_composizioneLotto").show();
            }
            else {
                $("#checkbox_composizioneLotto").hide();
            }

            if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura && Prodotto_Materia_Prima_Letto !== undefined &&
                Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== "") {

                if (Prodotto_Materia_Prima_Letto.Extra_Int > 0)
                    $("#chk_prodotto_UC_composizioneLotto").prop("checked", true);
                else
                    $("#chk_prodotto_UC_composizioneLotto").prop("checked", false);
            }

        }
        else {
            $("#checkbox_composizioneLotto").hide();
        }


        //Disabilito i checkbox Imballaggio,Contenitore,Confezione se sono in modifica ed è stato scelto uno di loro.
        if (parseInt($(Controls.xTipoOperazione).val()) === enum_tipoOperazione.Modifica && Prodotto_Materia_Prima_Letto !== undefined &&
            Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== "") {

            if ($("#chk_prodotto_UC_imballaggio").prop("checked"))
                $("#chk_prodotto_UC_imballaggio").attr('disabled', true);

            if ($("#chk_prodotto_UC_contenitore").prop("checked"))
                $("#chk_prodotto_UC_contenitore").attr('disabled', true);

            if ($("#chk_prodotto_UC_confezione").prop("checked"))
                $("#chk_prodotto_UC_confezione").attr('disabled', true);

        }
    }

    if (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) {

        //Creo le Multiselect specie e varieta solo se c'è il modulo il fresh and food e sono abilitati o gli imballaggi o 
        //i contenitori o le confezioni
        if (Modulo_FF === true &&
            ((Impostazione_utenteTabConfigurazione !== null && (Impostazione_utenteTabConfigurazione.Mostra_ChkImballaggio === true || Impostazione_utenteTabConfigurazione.Mostra_ChkContenitore)) ||
                Mostra_chkconfezione === true)) {

            let tabella_cod = Abilita_Disabilita_checkbox_dettagli_beni_conf();
            creaKendoMultiselect("multisel_prodotto_UC_specie_dett_beni", { read: RiempiMultiseSpecieDettagliBeniConf }, "Veg_Des", "Veg_Cod");
            creaKendoMultiselect("multisel_prodotto_UC_varieta_dett_beni", { read: RiempiMultiseVarietaDettagliBeniConf, data: { Veg_Cod: -1 } }, "Cul_Des", "Cul_Cod");
            Nascondi_Mostra_Filtro_Specie_Varieta();
            $("#multisel_prodotto_UC_specie_dett_beni").data("kendoMultiSelect").bind("change", MultiSpecieDettagliBeniConf_change);

            //Settare le multiselect una volta che riusciro a leggere i dati da otabelleparametri,se la specie letta è solo una 
            //allora setto e mostro anche la multiselect della varieta alrimenti rimane nascosta
            if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura) {
                if (tabella_cod !== 0) {
                    var prodotto = RicercaFiltriBeniConfezVeg(tabella_cod);

                    //Se non ci sono filtri specie e varieta per quel imballaggio,conf o contenitore
                    if (prodotto !== null && prodotto !== undefined && prodotto.length === 0) {
                        $("#multisel_varieta_beni_conf_veg").hide();
                    }

                    if (prodotto[0] !== null && prodotto[0] !== undefined && prodotto[0] !== "") {
                        if (prodotto[0].OFiltro_Veg_Cod !== "") {
                            let veg_cod = prodotto[0].OFiltro_Veg_Cod.split("|");

                            if (veg_cod.length === 1)
                                $("#multisel_varieta_beni_conf_veg").show();
                            else
                                $("#multisel_varieta_beni_conf_veg").hide();

                            //Se prendo un filtro con un solo veg_cod ma che è comunque separato da | e che quindi devo togliere
                            if (veg_cod.length === 3 && veg_cod[0] === "" && veg_cod[2] === "" && veg_cod[1] !== "") {
                                if (veg_cod[1] === "|null|" || veg_cod[1] === "null")
                                    Set_MultiselValue("multisel_prodotto_UC_specie_dett_beni", "-1");
                                else
                                    Set_MultiselValue("multisel_prodotto_UC_specie_dett_beni", veg_cod[1]);
                            } else {

                                if (prodotto[0].OFiltro_Veg_Cod === "0")
                                    Set_MultiselValue("multisel_prodotto_UC_specie_dett_beni", "-1");
                                else
                                    Set_MultiselValue("multisel_prodotto_UC_specie_dett_beni", prodotto[0].OFiltro_Veg_Cod);
                            }
                        }
                        if (prodotto[0].OFiltro_Cul_Cod !== "") {
                            $("#multisel_varieta_beni_conf_veg").show();
                            let multiSpecie = KendoMultisel("multisel_prodotto_UC_specie_dett_beni");
                            let multiselVarieta = KendoMultisel("multisel_prodotto_UC_varieta_dett_beni");

                            multiselVarieta.dataSource.options.transport.data.Veg_Cod = parseInt(multiSpecie.value()[0]);
                            multiselVarieta.dataSource.read();
                            let cul_Cod = prodotto[0].OFiltro_Cul_Cod.split("|");
                            //Se prendo un filtro con un solo cul_Cod ma che è comunque separato da | e che quindi devo togliere
                            if (cul_Cod.length === 3 && cul_Cod[0] === "" && cul_Cod[2] === "" && cul_Cod[1] !== "") {
                                if (cul_Cod[1] === "|null|" || cul_Cod[1] === "null")
                                    Set_MultiselValue("multisel_prodotto_UC_varieta_dett_beni", "0");
                                else
                                    Set_MultiselValue("multisel_prodotto_UC_varieta_dett_beni", cul_Cod[1]);
                            } else {
                                Set_MultiselValue("multisel_prodotto_UC_varieta_dett_beni", prodotto[0].OFiltro_Cul_Cod);
                            }

                        }
                        else {
                            $("#multisel_varieta_beni_conf_veg").hide();
                        }
                    }
                }
            }
        }
        else {
            $("#filtri_dett_beni_conf_veg").hide();
            $("#multisel_varieta_beni_conf_veg").hide();
        }
    }

    if (elem_cod === categorieProdotti.ALTRE_RISORSE) {
        if (utilizzoCA !== undefined && utilizzoCA === true ) {

            if (Impostazione_utenteTabConfigurazione !== null) {
                $("#checkbox_utilizzoCA").show();
            }
            else {
                $("#checkbox_utilizzoCA").hide();
            }

            if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Scrittura && Prodotto_Materia_Prima_Letto !== undefined &&
                Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== "") {

                if (Prodotto_Materia_Prima_Letto.Extra_Int > 0)
                    $("#chk_prodotto_UC_utilizzoCA").prop("checked", true);
                else
                    $("#chk_prodotto_UC_utilizzoCA").prop("checked", false);
            }

        }
        else {
            $("#checkbox_utilizzoCA").hide();
        }
    }

    creaKendoDropDownList("ddl_prodotto_UC_cod_extra", { read: Carica_ddl_udm_aspetto }, "UDM_DES", "UDM_COD").bind("change", ddl_udm_aspetto_change);

    creaKendoDropDownList("ddl_prodotto_UC_peso", { read: Carica_ddl_peso }, "tipo_peso_des", "tipo_peso_cod").bind("change", null);
    $("#ddl_prodotto_UC_peso").data("kendoDropDownList").enable(false);

    creaKendoDropDownList("ddl_prodotto_UC_set", { read: Carica_ddl_set }, "peso_set_des", "peso_set_cod").bind("change", null);
    if (Modulo_FF === true && elem_cod === categorieProdotti.TRASFORMATI_VEGETALI) {
        $(".Confezione_Base").show();
        creaKendoDropDownList("ddl_prodotto_UC_Confezione_Base", { read: Carica_ddl_Confezione_Base }, "base_des", "base_cod").bind("change", null);
    }
    else {
        $(".Confezione_Base").hide();
    }
    creaKendoDropDownList("ddl_prodotto_UC_confezioni", { read: Carica_ddl_confezioni }, "Descrizione", "Codice").bind("change", null);

    //Disabilito la dropdown Categoria nei Beni di Confezionamento Vegetale se l'utente non ha il permesso sulle ACCISE
    //(vedi Gias LAN).

    if (Impostazione_utenteTabConfigurazione !== null && Impostazione_utenteTabConfigurazione.Abilita_DDLCategBeniConf === true)
        $("#ddl_prodotto_UC_confezioni").data("kendoDropDownList").enable(true);
    else
        $("#ddl_prodotto_UC_confezioni").data("kendoDropDownList").enable(false);

    creaKendoDropDownList("ddl_prodotto_UC_tipo_default", { read: Carica_Tipo_Default }, "Descrizione", "Codice_Generazione").bind("change", null);

    $("#ddl_prodotto_UC_tipo_default").data("kendoDropDownList").enable(false);

    if (Prodotto_Materia_Prima_Letto !== undefined && Prodotto_Materia_Prima_Letto !== null && Prodotto_Materia_Prima_Letto !== "") {

        //#####Controlli della maschera che non dipendono da Udm_Cod_extra####
        Set_KendoDDLValue("ddl_prodotto_UC_tipo_default", Prodotto_Materia_Prima_Letto.Tipo_Default);

        Set_KendoDDLValue("ddl_prodotto_UC_Confezione_Base", Prodotto_Materia_Prima_Letto.OTabella_Cod_Base);

        if (Prodotto_Materia_Prima_Letto.Flag_Variazione > 0)
            $("#chk_prodotto_UC_imposta_flag_Variazione").prop("checked", true);
        else
            $("#chk_prodotto_UC_imposta_flag_Variazione").prop("checked", false);

        if (Prodotto_Materia_Prima_Letto.ChkEscludi_Magazzino > 0)
            $("#chk_prodotto_UC_escludi_da_magazzino").prop("checked", true);
        else
            $("#chk_prodotto_UC_escludi_da_magazzino").prop("checked", false);
        //####################################################################


        if (Prodotto_Materia_Prima_Letto.Udm_Cod_Extra === 0) {
            $(".configurazione_base").hide();
            $("#chk_prodotto_UC_imposta_flag_extra").prop("checked", false);
            Set_KendoDDLValue("ddl_prodotto_UC_cod_extra", 0);
            Set_KendoDDLValue("ddl_prodotto_UC_confezioni", "");
            Set_KendoNumTBValue("txt_prodotto_UC_Qta_Extra", 0);
            Set_KendoNumTBValue("txt_prodotto_UC_tara_nomi", 0);
            $("#chk_prodotto_UC_Qta_Contenitore_Conf").prop("checked", false);
            Set_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore_Conf", 0);
            Set_KendoDDLValue("ddl_prodotto_UC_set", 0);
        }
        else {
            $(".configurazione_base").show();
            $("#chk_prodotto_UC_Udm_Cod_Extra").prop("checked", true);

            if (Prodotto_Materia_Prima_Letto.Flag_Extra > 0)
                $("#chk_prodotto_UC_imposta_flag_extra").prop("checked", true);
            else
                $("#chk_prodotto_UC_imposta_flag_extra").prop("checked", false);

            Set_KendoDDLValue("ddl_prodotto_UC_cod_extra", Prodotto_Materia_Prima_Letto.Udm_Cod_Extra);

            //DETTAGLI BENI CONFEZIONAMENTO
            if (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) {
                //Riempire textbox quantita per i beni confezionati vedi GIAS LAN
                Set_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore", Prodotto_Materia_Prima_Letto.Qta_Contenitore);
                if (Prodotto_Materia_Prima_Letto.Confezione_Cod !== undefined && Prodotto_Materia_Prima_Letto.Confezione_Cod !== null) {
                    Set_KendoDDLValue("ddl_prodotto_UC_confezioni", Prodotto_Materia_Prima_Letto.Confezione_Cod);
                }
                else {
                    Set_KendoDDLValue("ddl_prodotto_UC_confezioni", "");
                }

            }

            Set_KendoDDLValue("ddl_prodotto_UC_peso", Prodotto_Materia_Prima_Letto.Tipo_Peso);

            Set_KendoNumTBValue("txt_prodotto_UC_Qta_Extra", Prodotto_Materia_Prima_Letto.Qta_Extra);

            Set_KendoNumTBValue("txt_prodotto_UC_tara_nomi", Prodotto_Materia_Prima_Letto.Tara);

            if (elem_cod !== categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI && elem_cod !== categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) {
                if (Prodotto_Materia_Prima_Letto.Qta_Contenitore > 0) {
                    $("#chk_prodotto_UC_Qta_Contenitore_Conf").prop("checked", true);
                }
                else {
                    $("#chk_prodotto_UC_Qta_Contenitore_Conf").prop("checked", false);
                    $("#txt_prodotto_UC_Qta_Contenitore_Conf").data("kendoNumericTextBox").enable(false);
                }
                Set_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore_Conf", Prodotto_Materia_Prima_Letto.Qta_Contenitore);
            }

            Set_KendoDDLValue("ddl_prodotto_UC_set", Prodotto_Materia_Prima_Letto.Peso_Set);

            //if (Prodotto_Materia_Prima_Letto.ChkEscludi_Preparazione > 0)
            //    $("#chk_prodotto_UC_escludi_da_preparazioni").prop("checked", true);
            //else
            //    $("#chk_prodotto_UC_escludi_da_preparazioni").prop("checked", false);

        }
    }
    else {
        $(".configurazione_base").hide();
        Set_KendoDDLValue("ddl_prodotto_UC_cod_extra", 2);
        Set_KendoDDLValue("ddl_prodotto_UC_set", 0);
        Set_KendoDDLValue("ddl_prodotto_UC_peso", 1);
        Set_KendoDDLValue("ddl_prodotto_UC_Confezione_Base", 0);
        Set_KendoDDLValue("ddl_prodotto_UC_confezioni", 0);
        Set_KendoDDLValue("ddl_prodotto_UC_tipo_default", 0);
        Set_KendoNumTBValue("txt_prodotto_UC_Qta_Extra", 0);
        Set_KendoNumTBValue("txt_prodotto_UC_tara_nomi", 0);
        Set_KendoNumTBValue("txt_prodotto_UC_Qta_Contenitore_Conf", 0);
        $('#txt_prodotto_UC_Qta_Contenitore_Conf').data("kendoNumericTextBox").enable(false);

        $("#chk_prodotto_UC_Udm_Cod_Extra").prop("checked", false);
        $("#chk_prodotto_UC_Qta_Contenitore_Conf").prop("checked", false);
        $("#chk_prodotto_UC_imposta_flag_extra").prop("checked", false);
        $("#chk_prodotto_UC_imposta_flag_Variazione").prop("checked", false);
        $("#chk_prodotto_UC_composizioneLotto").prop("checked", false);
        $("#chk_prodotto_UC_imballaggio").prop("checked", false);
        $("#chk_prodotto_UC_contenitore").prop("checked", false);
        $("#chk_prodotto_UC_escludi_da_magazzino").prop("checked", false);
        $("#multisel_varieta_beni_conf_veg").hide();
        $("#filtri_dett_beni_conf_veg").hide();
    }

    //Se è checkato Numero beni Contenuti abilito anche la textbox associata,altrimenti no 
    if ($("#chk_prodotto_UC_Qta_Contenitore_Conf").is(':checked') === true) {
        $('#txt_prodotto_UC_Qta_Contenitore_Conf').data("kendoNumericTextBox").enable(true);
    }
    else {
        $('#txt_prodotto_UC_Qta_Contenitore_Conf').data("kendoNumericTextBox").enable(false);
    }

    //Se l'utente non può gestire né gli imballaggi né le confezione e neanche le confezioni nascondo i 
    //Dettagli Beni di confezionamento.
    if ((elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI) &&
        (Mostra_chkconfezione === true || Impostazione_utenteTabConfigurazione.Mostra_ChkContenitore === true ||
            Impostazione_utenteTabConfigurazione.Mostra_ChkImballaggio === true)) {

        $("#dettagli_beni_di_confezionamento").show();
    }
    else {
        $("#dettagli_beni_di_confezionamento").hide();
    }
}


function ddl_udm_aspetto_change(e) {
    //In base se vengono scelti i chilogrammi o i litri,cambio anche la ddl_prodotto_UC_peso
    if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_cod_extra")) === 29) {
        Set_KendoDDLValue("ddl_prodotto_UC_peso", -1);
    }
    if (parseInt(Get_KendoDDLValue("ddl_prodotto_UC_cod_extra")) === 2) {
        Set_KendoDDLValue("ddl_prodotto_UC_peso", enum_TipoPeso.Peso_Netto);
    }
}

//ChechBox Prodotto Confezionato/Impostazione Pesi
function prodotto_Edit_UC_Udm_Cod_Extra() {
    if ($("#chk_prodotto_UC_Udm_Cod_Extra").prop("checked")) {
        $(".configurazione_base").show();
        Set_KendoDDLValue("ddl_prodotto_UC_cod_extra", 2);
    }
    else {
        $(".configurazione_base").hide();
        Set_KendoDDLValue("ddl_prodotto_UC_cod_extra", 0);
    }

}

function MultiSpecieDettagliBeniConf_change(e) {
    var multiSpecie = $("#multisel_prodotto_UC_specie_dett_beni").data("kendoMultiSelect");
    let veg_cod = parseInt(Get_MultiselString("multisel_prodotto_UC_specie_dett_beni"));
    if (multiSpecie.value().length === 1) {
        if (veg_cod === -1 || veg_cod === 0) {
            $("#multisel_varieta_beni_conf_veg").hide();
            Set_MultiselValue("multisel_prodotto_UC_varieta_dett_beni", "0");
        } else {
            $("#multisel_varieta_beni_conf_veg").show();

            var multiselVarieta = KendoMultisel("multisel_prodotto_UC_varieta_dett_beni");

            multiselVarieta.dataSource.options.transport.data.Veg_Cod = multiSpecie.value()[0];
            multiselVarieta.dataSource.read();
        }

    }
    else {
        //Se ho due elementi selezionati nella multiselect della Specie ma uno di questi due è "Tutte le Specie Vegetali",
        //carico lo stesso la multiselect delle varietà con l'altra Specie.
        if (multiSpecie.value().length === 2 && multiSpecie.value().includes("-1")) {
            $("#multisel_varieta_beni_conf_veg").show();

            var multiselVarieta = KendoMultisel("multisel_prodotto_UC_varieta_dett_beni");

            for (var x = 0; x < multiSpecie.value().length; x++) {
                if (multiSpecie.value()[x] !== "-1") {
                    multiselVarieta.dataSource.options.transport.data.Veg_Cod = multiSpecie.value()[x];
                    multiselVarieta.dataSource.read();
                    break;
                }

            }

        }
        else {
            $("#multisel_varieta_beni_conf_veg").hide();
            Set_MultiselValue("multisel_prodotto_UC_varieta_dett_beni", "0");
        }

    }
}

//ChechBox Contenitore
function prodotto_Edit_UC_contenitore() {
    if ($("#chk_prodotto_UC_contenitore").prop("checked")) {
        $(".Qta_Contenitore").show();
    }
    else {
        $(".Qta_Contenitore").hide();
    }

    Nascondi_Mostra_Filtro_Specie_Varieta();
    Abilita_Disabilita_checkbox_dettagli_beni_conf();
}

//ChechBox Contenitore_Conf
function prodotto_UC_Qta_Contenitore_Conf() {
    if ($("#chk_prodotto_UC_Qta_Contenitore_Conf").prop("checked")) {
        $('#txt_prodotto_UC_Qta_Contenitore_Conf').data("kendoNumericTextBox").enable(true);
    }
    else {
        $('#txt_prodotto_UC_Qta_Contenitore_Conf').data("kendoNumericTextBox").enable(false);
    }
}

//Nasconde o Mostra i filtri per specie  e varieta in base se il bene di confezionamento è un imballaggio,
//contenitore o confezione 
function Nascondi_Mostra_Filtro_Specie_Varieta() {

    let categoriaProdotto = parseInt($(Controls.xElem_Cod).val());

    if (categoriaProdotto !== categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI) {
        if (Modulo_FF === true && ((Impostazione_utenteTabConfigurazione !== null &&
            (Impostazione_utenteTabConfigurazione.Mostra_ChkImballaggio === true ||
                Impostazione_utenteTabConfigurazione.Mostra_ChkContenitore === true)) ||
            Mostra_chkconfezione === true)) {

            if ($("#chk_prodotto_UC_imballaggio").prop("checked") || $("#chk_prodotto_UC_contenitore").prop("checked") ||
                $("#chk_prodotto_UC_confezione").prop("checked")) {

                $("#filtri_dett_beni_conf_veg").show();

                $("#multisel_varieta_beni_conf_veg").show();
            }
            else {
                Set_MultiselValue("multisel_prodotto_UC_specie_dett_beni", "-1");

                let multiselVarieta = KendoMultisel("multisel_prodotto_UC_varieta_dett_beni");

                multiselVarieta.dataSource.options.transport.data.Veg_Cod = -1;

                multiselVarieta.dataSource.read();

                $("#filtri_dett_beni_conf_veg").hide();

                $("#multisel_varieta_beni_conf_veg").hide();
            }
        }
        else {

            $("#filtri_dett_beni_conf_veg").hide();

            $("#multisel_varieta_beni_conf_veg").hide();
        }
    }
    else {
        $("#filtri_dett_beni_conf_veg").hide();

        $("#multisel_varieta_beni_conf_veg").hide();
    }

    Set_MultiselValue("multisel_prodotto_UC_specie_dett_beni", "-1");

    let multiselVarieta = KendoMultisel("multisel_prodotto_UC_varieta_dett_beni");

    multiselVarieta.dataSource.options.transport.data.Veg_Cod = -1;

    multiselVarieta.dataSource.read();

    Set_MultiselValue("multisel_prodotto_UC_varieta_dett_beni", "0");

    Abilita_Disabilita_checkbox_dettagli_beni_conf();
}


function Abilita_Disabilita_checkbox_dettagli_beni_conf() {
    let chechbox_scelta = 0;
    if ($("#chk_prodotto_UC_imballaggio").prop("checked")) {
        $("#chk_prodotto_UC_contenitore").attr('disabled', true);
        $("#chk_prodotto_UC_confezione").attr('disabled', true);
        $("#chk_prodotto_UC_composizioneLotto").attr('disabled', true);

        chechbox_scelta = enum_OmniTabelle.ot_IMBALLAGGI_FF;
    }
    if ($("#chk_prodotto_UC_contenitore").prop("checked")) {
        $("#chk_prodotto_UC_imballaggio").attr('disabled', true);
        $("#chk_prodotto_UC_confezione").attr('disabled', true);
        $("#chk_prodotto_UC_composizioneLotto").attr('disabled', true);
        chechbox_scelta = enum_OmniTabelle.ot_CONTENITORI_FF;
    }
    if ($("#chk_prodotto_UC_confezione").prop("checked")) {
        $("#chk_prodotto_UC_imballaggio").attr('disabled', true);
        $("#chk_prodotto_UC_contenitore").attr('disabled', true);
        chechbox_scelta = enum_OmniTabelle.ot_CONFEZIONI_FF;
    }
    if ($("#chk_prodotto_UC_composizioneLotto").prop("checked")) {
        $("#chk_prodotto_UC_imballaggio").attr('disabled', true);
        $("#chk_prodotto_UC_contenitore").attr('disabled', true);
    }


    if ($("#chk_prodotto_UC_imballaggio").prop("checked") !== true &&
        $("#chk_prodotto_UC_contenitore").prop("checked") !== true &&
        $("#chk_prodotto_UC_confezione").prop("checked") !== true &&
        $("#chk_prodotto_UC_composizioneLotto").prop("checked") !== true) {
        $("#chk_prodotto_UC_imballaggio").attr('disabled', false);
        $("#chk_prodotto_UC_contenitore").attr('disabled', false);
        $("#chk_prodotto_UC_confezione").attr('disabled', false);
        $("#chk_prodotto_UC_composizioneLotto").attr('disabled', false);
    }

    return chechbox_scelta;
}

//Disabilito i controlli della tab Configurazione quando sono su INFO
function DisabilitaTabConfigurazione() {
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    $("#chk_prodotto_UC_Udm_Cod_Extra").attr('disabled', true);
    $("#chk_prodotto_UC_Qta_Contenitore_Conf").attr('disabled', true);
    $("#chk_prodotto_UC_imposta_flag_extra").attr('disabled', true);
    $("#chk_prodotto_UC_imposta_flag_Variazione").attr('disabled', true);
    //$("#chk_prodotto_UC_escludi_da_preparazioni").attr('disabled', true);
    $("#chk_prodotto_UC_escludi_da_magazzino").attr('disabled', true);
    //$('.dettagli').attr('disabled', true).removeAttr('checked');

    let ddl_cod_extra = $("#ddl_prodotto_UC_cod_extra").data("kendoDropDownList");
    let ddl_set = $("#ddl_prodotto_UC_set").data("kendoDropDownList");
    if (ddl_cod_extra !== undefined)
        $("#ddl_prodotto_UC_cod_extra").data("kendoDropDownList").enable(false);

    if (ddl_set !== undefined)
        ddl_set.enable(false);

    let ddl_conf_base = $("#ddl_prodotto_UC_Confezione_Base").data("kendoDropDownList");
    if (Modulo_FF === true && elem_cod === categorieProdotti.TRASFORMATI_VEGETALI && ddl_conf_base !== undefined) {
        ddl_conf_base.enable(false);
    }

    let ddl_peso = $("#ddl_prodotto_UC_peso").data("kendoDropDownList");
    let ddl_confezioni = $("#ddl_prodotto_UC_confezioni").data("kendoDropDownList");

    if (ddl_peso !== undefined)
        ddl_peso.enable(false);

    if (ddl_confezioni !== undefined)
        ddl_confezioni.enable(false);

    $('#txt_prodotto_UC_Qta_Extra').data("kendoNumericTextBox").enable(false);
    $('#txt_prodotto_UC_tara_nomi').data("kendoNumericTextBox").enable(false);
    $('#txt_prodotto_UC_Qta_Contenitore').data("kendoNumericTextBox").enable(false);
    $('#txt_prodotto_UC_Qta_Contenitore_Conf').data("kendoNumericTextBox").enable(false);

    //let mostraChkCont = ParametriQualitativiGestitiPerCheckBoxBeniConf.some(item => parseInt(item.Tabella_ID) === enum_OmniTabelle.ot_CONTENITORI_FF);
    let mostraChkCont = true;
    if (mostraChkCont === true && (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI))
        $("#chk_prodotto_UC_contenitore").attr('disabled', true);

    //let mostraChkImba = ParametriQualitativiGestitiPerCheckBoxBeniConf.some(item => parseInt(item.Tabella_ID) === enum_OmniTabelle.ot_IMBALLAGGI_FF);
    let mostraChkImba = true;
    if (mostraChkImba === true && (elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_ANIMALI || elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI))
        $("#chk_prodotto_UC_imballaggio").attr('disabled', true);

    //disabilita la checkbox solo per beni di confezionamento
    let mostraChkCompLotto = true;
    if (mostraChkCompLotto === true && elem_cod === categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI)
        $("#chk_prodotto_UC_composizioneLotto").attr('disabled', true);

    if (Mostra_chkconfezione === true)
        $("#chk_prodotto_UC_confezione").attr('disabled', true);

    let multi_var = $("#multisel_prodotto_UC_varieta_dett_beni").data("kendoMultiSelect");
    let multi_spec = $("#multisel_prodotto_UC_specie_dett_beni").data("kendoMultiSelect");

    if ((mostraChkCont === true || mostraChkImba === true || Mostra_chkconfezione === true) && multi_var !== undefined)
        multi_var.enable(false);

    if ((mostraChkCont === true || mostraChkImba === true || Mostra_chkconfezione === true) && multi_spec !== undefined)
        multi_spec.enable(false);
}

// Creazione dinamica controlli
function creaParametriQualitativi_FF_Zoo(veg_cod, cul_cod, cal_cod) {
    // Pulizia del DIV 
    $('#prodotto_UC_parametri_qualitativi_list div').html('');

    WaitFrame.show();

    let container = document.getElementById("prodotto_UC_parametri_qualitativi_list");
    let contaRighe = 0;
    let nrRighe = 0;
    let newRowDiv = null;
    let foundParamQual = false;
    let Modulo_Anagrafe = 0;
    let Elem_Cod = parseInt($(Controls.xElem_Cod).val());

    if (Elem_Cod === categorieProdotti.TRASFORMATI_VEGETALI && Modulo_FF === true)
        Modulo_Anagrafe = enum_Omni_Modulo_Generazione.FreshFood;

    if (Elem_Cod === categorieProdotti.TRASFORMATI_ANIMALI && Modulo_Zoo === true)
        Modulo_Anagrafe = enum_Omni_Modulo_Generazione.Zoo;

    paramQual_FF_filtrospevar = RicercaParametriQualitativiFiltroSpecieVarieta(true, $(cIdPiva).val(), Modulo_Anagrafe, veg_cod, cul_cod);

    for (var ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {
        if (paramQual_FF_filtrospevar[ipar].Tabella_ID !== 0) {
            // Creo il controllo solo per i parametri a libera imputazione oppure se c'è almeno un  
            // valore da caricare nella DDL(se length === 1 significa che c'è solo il record vuoto)

            valoreparamQual_FF_filtrospevar = RicercaValoriParametriQualitativiFiltratiSpecieVarieta(paramQual_FF_filtrospevar[ipar].Tabella_ID, $(cIdPiva).val(), true, veg_cod, cul_cod);

            if (paramQual_FF_filtrospevar[ipar].Tipo === 3 ||
                paramQual_FF_filtrospevar[ipar].Tipo === 4 ||
                paramQual_FF_filtrospevar[ipar].Tipo === 5 ||
                valoreparamQual_FF_filtrospevar.length > 1) {
                // 3 colonne per riga
                if (contaRighe == 3) {
                    contaRighe = 0;
                }

                if (contaRighe == 0) {
                    if (newRowDiv !== null) {
                        container.appendChild(newRowDiv);
                    }
                    nrRighe++;
                    newRowDiv = creaNewRowDiv("id" + nrRighe.toString);
                }

                // Tipo 3 sono i parametri a libera imputazione numerici
                // Tipo 4 sono i parametri a libera imputazione stringa
                // Tipo 5 sono i parametri a libera imputazione data
                let tipo_param = "";
                switch (paramQual_FF_filtrospevar[ipar].Tipo) {
                    case 3:
                        foundParamQual = true;
                        tipo_param = "txt";
                        break;

                    case 4:
                        foundParamQual = true;
                        tipo_param = "txtStr";
                        break;

                    case 5:
                        foundParamQual = true;
                        tipo_param = "date";
                        break;

                    default:
                        tipo_param = "ddl";
                        break;
                }

                //Creo anche le textbox oltre alle dropdown di Contenitori e Confezioni
                if (parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID) === 5 || parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID) === 8) {

                    //Creo una nuova riga con la ddl e la sua textbox dei Contenitori.
                    //e poi creo un altra riga con la ddl e la sua textbox dei Confezioni.
                    if (newRowDiv !== null) {
                        container.appendChild(newRowDiv);
                    }
                    nrRighe = 3;
                    newRowDiv = creaNewRowDiv("id" + nrRighe.toString);

                    tipo_param = "ddl";
                    var newColumnDiv = creaNewColumnBS(6, 6, 12);
                    var newDivInputGroup = creaDIV("input-group");
                    //i18N Creazione Label dinamica  da tradurre
                    newDivInputGroup.appendChild(creaLabel("lbl" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, paramQual_FF_filtrospevar[ipar].Tabella_Des, "input-group-addon alert-info", tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des));
                    newDivInputGroup.appendChild(creaInputGenerico(tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, "form-control"));

                    newColumnDiv.appendChild(newDivInputGroup);
                    newRowDiv.appendChild(newColumnDiv);

                    tipo_param = "txt";
                    var newColumnDiv = creaNewColumnBS(6, 6, 12);
                    var newDivInputGroup = creaDIV("input-group");
                    var descr_label = ""
                    if (parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID) === 5) {
                        descr_label = TraduzioneMultiResx(resxProdottoEditUC, "NumeroDiConfezioniPerContenitore", "Numero di confezioni per contenitore");
                    } else if (parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID) === 8) {
                        descr_label = TraduzioneMultiResx(resxProdottoEditUC, "NumeroDiContenitoriPerImballo", "Numero di contenitori per imballo");
                    }

                    newDivInputGroup.appendChild(creaLabel("lbl_txt" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, descr_label, "input-group-addon alert-info", tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des));
                    newDivInputGroup.appendChild(creaInputGenerico(tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, "form-control"));

                }
                else {
                    var newColumnDiv = creaNewColumnBS(4, 4, 12);
                    var newDivInputGroup = creaDIV("input-group");
                    //i18N Creazione Label dinamica  da tradurre
                    newDivInputGroup.appendChild(creaLabel("lbl" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, paramQual_FF_filtrospevar[ipar].Tabella_Des, "input-group-addon alert-info", tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des));
                    switch (paramQual_FF_filtrospevar[ipar].Tipo) {
                        case 3:
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, "form-control"));
                            break;

                        case 4:
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, "form-control"));
                            break;

                        case 5:
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, "kendoCalendar"));
                            break;

                        default:
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, "form-control"));
                            break;
                    }

                }
                newColumnDiv.appendChild(newDivInputGroup);
                newRowDiv.appendChild(newColumnDiv);
                contaRighe++;
            }

            if (valoreparamQual_FF_filtrospevar.length > 1)
                valoriparamQual_FF_filtrospevar.push({ Tabella_ID: parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID), Dati: valoreparamQual_FF_filtrospevar });
        }

        // primo giro
        if (ipar === 0) {
            container.innerHTML = "";
        }

        // ultimo giro
        if (ipar == paramQual_FF_filtrospevar.length - 1 && newRowDiv !== null) {
            container.appendChild(newRowDiv);
        }

    }

    for (var ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {
        if (paramQual_FF_filtrospevar[ipar].Tabella_ID !== 0) {

            let tipo_param = "";
            switch (paramQual_FF_filtrospevar[ipar].Tipo) {
                case 3:
                    // Tipo 3 sono i parametri a libera imputazione numerici
                    tipo_param = "txt";
                    $("#" + tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des).kendoNumericTextBox({ format: "n0", decimals: 0, min: 0 });
                    break;

                case 4:
                    // Tipo 4 sono i parametri a libera imputazione stringa
                    tipo_param = "txtStr";
                    // NULLA DA FARE
                    break;

                case 5:
                    // Tipo 5 sono i parametri a libera imputazione data
                    tipo_param = "date";
                    // NULLA DA FARE, la classe l'ho già impostata sopra
                    break;

                default:
                    // DDL

                    if (controllaValoriDDLParametriQualitativi(valoriparamQual_FF_filtrospevar, parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID))) {
                        tipo_param = "ddl";

                        //let ParamQualFiltratiXSpecieVarieta = RicercaValoriParametriQualitativiFiltratiSpecieVarieta(paramQual_FF_filtrospevar[ipar].Tabella_ID, $(cIdPiva).val(), true, veg_cod, cul_cod);

                        //let ParamQualFiltratiXSpecieVarieta = "";
                        for (var x = 0; x < valoriparamQual_FF_filtrospevar.length; x++) {
                            if (parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID) === parseInt(valoriparamQual_FF_filtrospevar[x].Tabella_ID)) {
                                ParamQualFiltratiXSpecieVarieta = valoriparamQual_FF_filtrospevar[x].Dati;
                                break;
                            }
                        }

                        creaKendoDropDownList(tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, { read: LeggiValoriParametriQualitativiFiltratiSpecieVarieta, data: { elencoValori: ParamQualFiltratiXSpecieVarieta } }, "val_des", "val_cod").bind("change", ddlParamQual_change);
                        let dropdownlist = $("#" + tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des).data("kendoDropDownList");
                        dropdownlist.refresh();
                        if (!foundParamQual && KendoDDL(tipo_param + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des).dataSource.data().length > 1) {
                            foundParamQual = true;
                        }
                    }

                    break;
            }
        }
    }


    if (foundParamQual) {
        $("#prodotto_UC_parametri_qualitativi_list").show();
        $(".kendoCalendar").kendoDatePicker({
            footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
            max: new Date(2100, 11, 31)
        });
        $(".Referenze_Param_Qual").show();

        // INIZIO Questa per gestire il nr contenitori per imballo e nr confezioni per contenitore
        for (var x = 0; x < paramQual_FF_filtrospevar.length; x++) {
            if (parseInt(paramQual_FF_filtrospevar[x].Tabella_ID) === 5 || parseInt(paramQual_FF_filtrospevar[x].Tabella_ID) === 8) {
                //Rende possibilie l'inserimento di solo i valori numerici (non negativi)
                $("#txt" + paramQual_FF_filtrospevar[x].Tabella_Cod_Des).kendoNumericTextBox({ format: "n0", decimals: 0, min: 0 });
            }
        }
        // FINE Questa per gestire il nr contenitori per imballo e nr confezioni per contenitore

        let Default_ParametriQualitativi = Default_ParametriQualitativi_Dati_Tecnici(cal_cod);
        let valore = 0;

        //Imposto i valori dei controlli creati qui sopra
        for (let ipar = 0; ipar < paramQual_FF_filtrospevar.length; ipar++) {
            if (Default_ParametriQualitativi.length > 0 && Default_ParametriQualitativi !== null && Default_ParametriQualitativi !== undefined) {
                if (paramQual_FF_filtrospevar[ipar].Tabella_ID !== 0) {

                    for (let i = 0; i < Default_ParametriQualitativi.length; i++) {
                        valore = Default_ParametriQualitativi[i].Tipo_Cod;
                        if (Default_ParametriQualitativi[i].Tipo === "o" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des &&
                            paramQual_FF_filtrospevar[ipar].Tipo !== 3 &&
                            controllaValoriDDLParametriQualitativi(valoriparamQual_FF_filtrospevar, parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID))) {

                            Set_KendoDDLValue("ddl" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, valore);
                            //Imposto anche le textbox di confezione e contenitore
                            if (Default_ParametriQualitativi[i].Tipo === "oconfezione" || Default_ParametriQualitativi[i].Tipo === "ocontenitore") {
                                Set_KendoNumTBValue("txt" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, Default_ParametriQualitativi[i].Val_Cod);
                            }
                        }
                        if (Default_ParametriQualitativi[i].Tipo === "o" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des &&
                            paramQual_FF_filtrospevar[ipar].Tipo === 3) {
                            Set_KendoNumTBValue("txt" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, Default_ParametriQualitativi[i].Val_Cod);
                        }
                        if (Default_ParametriQualitativi[i].Tipo === "o" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des &&
                            paramQual_FF_filtrospevar[ipar].Tipo === 4) {
                            let nomeCampoStr = "txtStr" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des;
                            $('input[name$=' + nomeCampoStr + ']').val(Default_ParametriQualitativi[i].Val_Cod);
                        }
                        if (Default_ParametriQualitativi[i].Tipo === "o" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des &&
                            paramQual_FF_filtrospevar[ipar].Tipo === 5) {
                            set_data("date" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, Default_ParametriQualitativi[i].Val_Cod, null);
                        }

                    }
                }
            }
            else {
                if (paramQual_FF_filtrospevar[ipar].Tipo !== 3 &&
                    controllaValoriDDLParametriQualitativi(valoriparamQual_FF_filtrospevar, parseInt(paramQual_FF_filtrospevar[ipar].Tabella_ID))) {
                    valore = 0;
                    Set_KendoDDLValue("ddl" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, valore);
                    //Imposto anche le textbox di confezione e contenitore
                    if (paramQual_FF_filtrospevar[ipar].Tabella_ID === "5" || paramQual_FF_filtrospevar[ipar].Tabella_ID === "8") {
                        Set_KendoNumTBValue("txt" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, valore);
                    }
                }
                if (paramQual_FF_filtrospevar[ipar].Tipo === 3) {
                    valore = 0;
                    Set_KendoNumTBValue("txt" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, valore);
                }
                if (paramQual_FF_filtrospevar[ipar].Tipo === 4) {
                    valore = "";
                    let nomeCampoStr = "txtStr" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des;
                    $('input[name$=' + nomeCampoStr + ']').val(valore);
                }
                if (paramQual_FF_filtrospevar[ipar].Tipo === 5) {
                    valore = "";
                    set_data("date" + paramQual_FF_filtrospevar[ipar].Tabella_Cod_Des, valore, null);
                }
            }
        }
    }
    else {
        $("#prodotto_UC_parametri_qualitativi_list").hide();
        $(".Referenze_Param_Qual").hide();
    }

    WaitFrame.hide();
}

// Leggere i valori dai controlli per salvarli su DB
function AggiungiValoriParametriQualitativi() {

    if (paramQual_FF_filtrospevar !== undefined && paramQual_FF_filtrospevar !== null && paramQual_FF_filtrospevar.length !== 0) {

        let arrParams = [];

        for (let iPar = 0; iPar < paramQual_FF_filtrospevar.length; iPar++) {
            if (paramQual_FF_filtrospevar[iPar].Tabella_ID !== 0) {

                let oTipo = "o" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des;

                let tipoCod = 0;
                let valCod = 0;


                // Tipo 3 sono i parametri a libera imputazione numerici
                // Tipo 4 sono i parametri a libera imputazione stringa
                // Tipo 5 sono i parametri a libera imputazione data
                switch (paramQual_FF_filtrospevar[iPar].Tipo) {
                    case 3:
                        valCod = Get_KendoNumTBValue("txt" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des);
                        break;

                    case 4:
                        let nomeCampoStr = "txtStr" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des;
                        valCod = $('input[name$=' + nomeCampoStr + ']').val();
                        break;

                    case 5:
                        valCod = get_data("date" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des);
                        break;

                    default:
                        if (KendoDDL("ddl" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des) !== undefined)
                            tipoCod = Get_KendoDDLValue("ddl" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des);
                        break;
                }



                if (parseInt(paramQual_FF_filtrospevar[iPar].Tabella_ID) === 5 || parseInt(paramQual_FF_filtrospevar[iPar].Tabella_ID) === 8) {
                    //Se esiste sia la dropdown che la textbox dei contenitori e confezioni allora salvo il valore della textbox
                    if (KendoDDL("ddl" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des) !== undefined &&
                        $("#txt" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des).data("kendoNumericTextBox") !== undefined) {
                        valCod = Get_KendoNumTBValue("txt" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des);
                    }

                }


                //verifico se è già presente nell'array, sennò creo con oTipo + tipo_cod
                let elems = arrayLookup(arrParams, "Tabella_Nome", oTipo);

                //l'oggetto è di fatto ritornato per riferimento, quindi le modifiche fatte qui non sono su una copia, ma sull'elemento stesso
                let wValCod = null;
                switch (paramQual_FF_filtrospevar[iPar].Tipo) {
                    case 1:
                        if (valCod === undefined || valCod === null || valCod === "")
                            wValCod = 0;
                        else
                            wValCod = kendo.parseFloat(valCod);
                        break;
                    case 3:
                        //Se il kendo numeric textbox è vuoto assume come valore null
                        if (valCod === null)
                            wValCod = 0;
                        else if (kendo.parseFloat(valCod) !== "")
                            wValCod = valCod;
                        break;
                    case 4:
                        wValCod = valCod.toString();
                        break;
                    case 5:
                        //Se il kendo calendar è vuoto assume come valore null
                        if (valCod === null)
                            wValCod = "";
                        else if (kendo.parseDate(valCod) !== "")
                            wValCod = valCod;
                        break;
                }

                if (wValCod !== undefined && wValCod !== null) {
                    if (elems !== undefined && elems !== null) {
                        elems.Tipo_Cod = parseInt(tipoCod);
                        elems.Val_Cod = wValCod;
                    } else {
                        let progressivo = 0;

                        if (Prodotto_Materia_Prima_Letto !== undefined && Prodotto_Materia_Prima_Letto !== null) {
                            progressivo = Prodotto_Materia_Prima_Letto.Cal_Cod;
                        }

                        let res = {
                            Progressivo: progressivo,
                            Tipo_Campion: oTipo,
                            Tipo_ParamQual: paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des,
                            Tipo_Cod: parseInt(tipoCod),
                            Val_Cod: wValCod,
                            ChkTara_Campionatura: 0,
                            Tara_Campionatura: 0,
                            Valore_Des: paramQual_FF_filtrospevar[iPar].Tabella_ID
                        };
                        arrParams.push(res);
                    }
                }

            }
        }

        if (arrParams.length > 0) {
            return arrParams;
        }

    }
    return;
}


function controllaValoriDDLParametriQualitativi(valoriparamQual_FF_filtrospevar,Tabella_ID) {

    var risp = false;

    if (valoriparamQual_FF_filtrospevar === undefined || valoriparamQual_FF_filtrospevar === null || valoriparamQual_FF_filtrospevar === "" ||
        Tabella_ID === undefined || Tabella_ID === null || Tabella_ID === "")
        return risp;


    for (var y = 0; y < valoriparamQual_FF_filtrospevar.length;y++) {
        if (parseInt(valoriparamQual_FF_filtrospevar[y].Tabella_ID) === Tabella_ID) {
            if (valoriparamQual_FF_filtrospevar[y].Dati.length > 1) {
                risp = true;
                return risp;
            }
            else {
                risp = false;
                return risp;
            }
        }
    }


    return risp;
}


function ddlParamQual_change(e) {

}

function Prodotto_Edit_UC_Genera_Descrizione_change(e) {
    let switch_descrizione = $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch");
    if (switch_descrizione.check() === true) {
        if ($("#txt_prodotto_UC_Descrizione").val() !== "" && $("#txt_prodotto_UC_Descrizione").val() !== " ") {
            let container = document.getElementById("switch_prodotto_UC_componi_descrizione");
            let id_dialog = creaNewRowDiv("id_dialog_genera_descr");
            container.appendChild(id_dialog);

            $("#id_dialog_genera_descr").kendoDialog({
                title: TraduzioneMultiResx(resxProdottoEditUC, "ConfermaGenerazioneDescrizione", "Conferma Generazione Descrizione"),
                closable: false,
                modal: {
                    preventScroll: true
                },
                content: TraduzioneMultiResx(resxProdottoEditUC, "SiDesideraDavveroComporreDescrizione", "Si desidera davvero comporre in automatico la Descrizione del prodotto in base ai dati compilati?</br>( Verrà sovrascritta la Descrizione inserita )"),
                actions: [{
                    text: TraduzioneMultiResx(resxProdottoEditUC, "No", "No"),
                    action: function (e) {
                        switch_descrizione.check(false);
                    },
                    primary: true,
                },
                {
                    text: TraduzioneMultiResx(resxProdottoEditUC, "Si", "Sì"),
                    action: function (e) {
                        Prodotto_Edit_UC_Genera_Descrizione_Dati_Scelti();
                    }
                }]
            });
        }
        else {
            Prodotto_Edit_UC_Genera_Descrizione_Dati_Scelti();
        }
    }
}


function Prodotto_Edit_UC_Genera_Descrizione_Dati_Scelti() {
    let elem_cod = parseInt($(Controls.xElem_Cod).val());
    let descr = "";
    let ddl_reg = $('#ddl_prodotto_UC_Regolamento').data("kendoDropDownList");


    switch (elem_cod) {
        case categorieProdotti.SEMILAVORATI_PRODUZIONE_ANIMALE:
        case categorieProdotti.TRASFORMATI_ANIMALI:
            let ddl_specie_anim_text = $('#ddl_prodotto_UC_specie_anim').data("kendoDropDownList").text();
            let ddl_indi_prod_text = $('#ddl_prodotto_UC_indi_produt').data("kendoDropDownList").text();
            let ddl_razza_anim_text = $('#ddl_prodotto_UC_razza_anim').data("kendoDropDownList").text();

            if (ddl_specie_anim_text !== "") {
                descr += ddl_specie_anim_text;
            }

            if (ddl_indi_prod_text !== "") {
                descr += " - " + ddl_indi_prod_text;
            }


            if (ddl_razza_anim_text !== "") {
                descr += " - " + ddl_razza_anim_text;
            }

            break;
        default:
            let ddl_specie_veg_text = $('#ddl_prodotto_UC_specie_veg').data("kendoDropDownList").text();
            let ddl_tipo_var_text = $('#ddl_prodotto_UC_tipo_varietale').data("kendoDropDownList").text();
            let ddl_var_text = $('#ddl_prodotto_UC_varieta').data("kendoDropDownList").text();


            if (ddl_specie_veg_text !== "") {
                descr += ddl_specie_veg_text;
            }

            if (ddl_var_text !== "") {
                descr += " - " + ddl_var_text;
            }


            if (ddl_tipo_var_text !== "") {
                descr += " - " + ddl_tipo_var_text;
            }


            if (elem_cod === categorieProdotti.SEMENTI) {
                let ddl_semente_text = $('#ddl_prodotto_UC_sementi_materiale').data("kendoDropDownList").text();
                if (ddl_semente_text !== "") {
                    if (ddl_specie_veg_text !== "") {
                        descr += " - " + ddl_semente_text;
                    }
                    else {
                        descr += ddl_semente_text;
                    }
                }
            }

            break;
    }

    if (descr !== "" && ddl_reg.text() !== "" && parseInt(ddl_reg.value()) === 4) {
        descr += " - BIO";
    }

    let dd_TecnologieSementi = $('#ddl_prodotto_UC_TecnologieSementi').data("kendoDropDownList")
    if (dd_TecnologieSementi !== undefined) {
        if (elem_cod === categorieProdotti.SEMENTI && $("#ddl_prodotto_UC_sementi_materiale").data("kendoDropDownList").value() == "1" && dd_TecnologieSementi.value() != 0) {
            descr += " - " + dd_TecnologieSementi.text();
        }

    }

    $("#txt_prodotto_UC_Descrizione").val(descr);
    nascondi_riepilogo_error();
}


function Prodotto_Importato(Flag_Importato) {
    //Aggiungo l'asterisco di campo obbligatorio a codice prodotto.
    $("#lbl_prodotto_UC_cod_prod").html("");
    $("#lbl_prodotto_UC_cod_prod").html(TraduzioneMultiResx(resxProdottoEditUC, "CodiceProdotto", "Codice Prodotto") + " *:");

    let TipoOp = parseInt($(Controls.xTipoOperazione).val());

    let elem_cod = parseInt($(Controls.xElem_Cod).val());

    if (Flag_Importato !== undefined && Flag_Importato !== null && Flag_Importato !== "") {

        //Se Flag_Importato = 1 metto in sola lettura tutte le tab tranne "Dati Contabilità" , "Traduzioni" e "Dati vendita dettaglio"
        if (Flag_Importato === 1) {
            Prodotto_Edit_UC_Importato = true;
            Disabilita_Controlli_infoProdotto_Edit();
        }
        //Se Flag_Importato = 0 e se è attivo il flag "Univocità Codice Esterno - Codice Interno",
        //abilito textbox codice esterno e disabilito codice articolo che viene generato
        else {

            //Abilito il codice esterno solo per i Trasformati Vegetali
            if (elem_cod === categorieProdotti.TRASFORMATI_VEGETALI) {
                let risp = Cambia_Codice_ArticoloDaCodice_Esterno_InBaseAFlag();

                if (risp === true) {

                    Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno = true;

                    if (TipoOp !== enum_tipoOperazione.Lettura) {
                        $('#txt_prodotto_UC_cod_prod').attr("disabled", "disabled");
                        $('#txt_prodotto_UC_cod_est').removeAttr('disabled');
                    }

                    //Aggiungo l'asterisco di campo obbligatorio a 
                    //codice esterno, ma prima lo tolgo dal codice prodotto.
                    $("#lbl_prodotto_UC_cod_prod").html("");
                    $("#lbl_prodotto_UC_cod_prod").html(TraduzioneMultiResx(resxProdottoEditUC, "CodiceProdotto", "Codice Prodotto") + " *:");

                    $("#lbl_prodotto_UC_cod_est").html("");
                    $("#lbl_prodotto_UC_cod_est").html(TraduzioneMultiResx(resxProdottoEditUC, "CodiceEsterno", "Codice Esterno") + " *:");
                }
            }

        }
    }

}

function txt_prodotto_UC_cod_est_change() {
    //Quando sono nella creazione di un nuovo Prodotto non aggiorno subito il codice_articolo ma lo farò al salvataggio
    //Se Flag_Importato = 0 e se è attivo il flag "Univocità Codice Esterno - Codice Interno",
    //allora quando cambio il Codice_Esterno genero il codice_articolo con  Codice_Esterno_Mat_Cod
    let tipoOperazione = parseInt($(Controls.xTipoOperazione).val());

    if (tipoOperazione !== enum_tipoOperazione.Scrittura && tipoOperazione !== enum_tipoOperazione.Duplica) {
        if ($("#txt_prodotto_UC_cod_est").val() === undefined ||
            $("#txt_prodotto_UC_cod_est").val() === null)
            return;

        if ($("#txt_prodotto_UC_cod_est").val() === "") {
            $("#txt_prodotto_UC_cod_prod").val("");
        }
        else {
            if ($(Controls.xMat_Cod).val() !== undefined && $(Controls.xMat_Cod).val() !== null && $(Controls.xMat_Cod).val() !== "")
                $("#txt_prodotto_UC_cod_prod").val($("#txt_prodotto_UC_cod_est").val() + "_" + $(Controls.xMat_Cod).val());
        }
    }
} 


function Prodotto_Edit_UC_CreaImageEditor() {

    let TipoOperazione = parseInt($(Controls.xTipoOperazione).val());

    let elem_cod = parseInt($(Controls.xElem_Cod).val());

    let isMateriaPrima = IsMateriaPrima(elem_cod);

    let proprietario = ($(Controls.xProprietario).val() == "True");
    
    let items_toolbar_prodotto_UC_imageEditor = ["open","save"];

    //Se non sono in Lettura e sono Proprietario attivo il pulsante anche per l'upload dell'immagine
    if (TipoOperazione === enum_tipoOperazione.Lettura || (isMateriaPrima === true && proprietario === false))
        items_toolbar_prodotto_UC_imageEditor.splice(0, 1);

    $("#prodotto_UC_imageEditor").kendoImageEditor({
        toolbar: {
            items: items_toolbar_prodotto_UC_imageEditor
        },
        imageLoaded: function (e) {
            let immagine = e.image;

            if ($(xDatiImmagineCaricata).val() !== "") {

                let datiImmagine = JSON.parse($(xDatiImmagineCaricata).val());

                datiImmagine.Immagine = immagine.src;

                $("#prodotto_UC_imageEditor").data("kendoImageEditor").options.saveAs.fileName = datiImmagine.Nome;

                $("#prodotto_UC_lblImage").text(TraduzioneMultiResx(resxProdottoEditUC, "Immagine", "Immagine") + " '" + datiImmagine.Nome + "' :");

                $(xDatiImmagineCaricata).val(kendo.stringify(datiImmagine));
            }
        },
        execute: function (e) {
            if (e.command === "OpenImageEditorCommand" && !OpenExecuted) {
                setTimeout(function () {
                    handleUploadEvents(e.sender);
                });
            }
        },
        error: function (e) {
            Prodotto_Edit_UC_EliminaeRicreaImageEditor();
        }
    });
}

//Rimuovo l'Immagine caricata
$("#btn_prodotto_UC_rimuovi_immagine").on("click", function () {

    $("#prodotto_UC_lblImage").text(TraduzioneMultiResx(resxProdottoEditUC, "Immagine", "Immagine") + ":");

    $(xDatiImmagineCaricata).val("");

    Prodotto_Edit_UC_EliminaeRicreaImageEditor();
});


function Prodotto_Edit_UC_EliminaeRicreaImageEditor() {
    //Svuoto il div dell'immagine e l'hiddenfield
    $("#prodotto_UC_imageEditor").empty();

    //Ricreo l'image editor
    Prodotto_Edit_UC_CreaImageEditor();

    OpenExecuted = false;
}

// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA ALIAS
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaAlias(IDControllo, Disabilita_Griglia) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var colonna_editabile = false;

    if (Disabilita_Griglia !== undefined && Disabilita_Griglia !== null && Disabilita_Griglia !== null && Disabilita_Griglia === true) {
        UteAbilitatoInsMod = false;
        UteAbilitatoCanc = false;
    }

    if (UteAbilitatoInsMod === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var funzioniCRUD = {
        funzioneRead: CaricaProdottiXGrigliaAlias,
        funzioneSubmit: { funzione: SubmitProdottiXAlias, flagInsert: true, flagUpdate: true, flagDelete: UteAbilitatoCanc },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true
    };

    var idModel = "Mat_Cod_Alias";
    var campiKendoModel = {
        Mat_Cod_Alias: { editable: colonna_editabile, type: "number", validation: { required: true }, defaultValue: 0 },
        Mat_Des_Alias: { editable: colonna_editabile, type: "string", validation: { required: true },defaultValue: "" },
        Mat_Cod_Alias_Old: { editable: false, type: "number", defaultValue: 0 },
        Mat_Des_Alias_Old: { editable: false, type: "string", defaultValue: "" },
        Codice_Lingua: { editable: false, type: "string", defaultValue: "" },
        Filtro_Contatti: { editable: colonna_editabile, validation: { required: false }, defaultValue: "" },
        Filtro_Contatti_Des: { editable: colonna_editabile, validation: { required: false }, defaultValue: "" },
        Filtro_Contatti_String: { editable: colonna_editabile, type: "string", validation: { required: false }, defaultValue: "" },
        Filtro_Contatti_Des_String: { editable: colonna_editabile, type: "string", validation: { required: false }, defaultValue: "" }
    };
    var colonneKendoGrid = [
        {
            field: "Mat_Des_Alias", title: TraduzioneMultiResx(resxProdottoEditUC, "Prodotto", "Prodotto"), editor: Alias_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Filtro_Contatti_Des_String", title: TraduzioneMultiResx(resxProdottoEditUC, "Contatti", "Contatti"), editor: Contatti_MultiSelectEditor, filterable: { multi: true, search: true }
        }
    ];

    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditAlias };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];


    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function onEditAlias(e) {
    var tipoOperazione = parseInt($(Controls.xTipoOperazione).val());

    if (tipoOperazione === enum_tipoOperazione.Lettura)
        e.sender.closeCell();

}


function Alias_DropDownEditor(container,options) {

    let elenco_alias = Carica_ddl_griglia_alias();

    creaDropDownEditor(container, "Mat_Des_Alias", "Mat_Cod_Alias", elenco_alias, changeAlias);

    //Mat_Cod_Alias_Old e Mat_Des_Old sono valorizzati sono in modifica perché mi servono per fare l'update
    //della riga in Materie_Prime_Alias

    if (options.model.isNew() === false) {
        options.model.Mat_Cod_Alias_Old = options.model.Mat_Cod_Alias;
        options.model.Mat_Des_Alias_Old = options.model.Mat_Des_Alias;
    }

}

function changeAlias(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#prodotto_UC_griglia_alias").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Mat_Cod_Alias = dataItem.Mat_Cod_Alias;
    model.Mat_Des_Alias = dataItem.Mat_Des_Alias;
}

function Contatti_MultiSelectEditor(container) {

    if (ElencoContatti === undefined || ElencoContatti === null)
        ElencoContatti = Carica_Contatti();

    $('<input name="Filtro_Contatti"/>')
        .appendTo(container)
        .kendoMultiSelect({
            autoBind: true,
            dataTextField: "Rag_Soc",
            dataValueField: "Cod_RisUm",
            dataSource: ElencoContatti,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: changeContatti
        }).data("kendoMultiSelect");

    var multi = $('input[name$="Filtro_Contatti"]').data("kendoMultiSelect");
    multi.list.width("auto");

}

function changeContatti(e) {

    if (ElencoContatti === undefined || ElencoContatti === null)
        ElencoContatti = Carica_Contatti();

    var Filtro_Contatti_Scelti = $('input[name$="Filtro_Contatti"]').data("kendoMultiSelect").dataItems();

    var grid = $("#prodotto_UC_griglia_alias").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    model.Filtro_Contatti = [];
    model.Filtro_Contatti_Des = [];
    model.Filtro_Contatti_String = "";
    model.Filtro_Contatti_Des_String = "";

    if (Filtro_Contatti_Scelti.length > 0) {

        for (var x = 0; x < Filtro_Contatti_Scelti.length; x++) {
            model.Filtro_Contatti.push(Filtro_Contatti_Scelti[x].Cod_RisUm);
            model.Filtro_Contatti_Des.push(Filtro_Contatti_Scelti[x].Rag_Soc);
        }

        if (model.Filtro_Contatti.length > 0)
            model.Filtro_Contatti_String = model.Filtro_Contatti.join("|");

        if (model.Filtro_Contatti_Des.length > 0)
            model.Filtro_Contatti_Des_String = model.Filtro_Contatti_Des.join(",");

    }
    else {
        model.Filtro_Contatti = [""];
        model.Filtro_Contatti_Des = [""];
        model.Filtro_Contatti_String = "";
        model.Filtro_Contatti_Des_String = "";
    }
}

function SubmitProdottiXAlias(options) {

    var grid = $("#prodotto_UC_griglia_alias").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitProdottiXAlias(options.data.created) +
        controllaRigheCompletePerSubmitProdottiXAlias(options.data.updated);

    if (nrErr > 0) {
        if (nrErr === 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxProdottoEditUC, "EsisteRigaIncompletaNellaGrigliaAlias", "Esiste una riga con dati non completi nella griglia delle Descrizioni Alternative."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(resxProdottoEditUC, "EsistonoNRigheIncompleteNellaGrigliaAlias", "Esistono {0} righe con dati non completi nella griglia delle Descrizioni Alternative."), nrErr), "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        erroriSubmitGriglie = true;
        return;
    }

    //errMess = controllaRigheValidePerSubmitProdottiXAlias(options.data.created, "");
    //errMess = controllaRigheValidePerSubmitProdottiXAlias(options.data.updated, errMess);

    //if (errMess != "") {
    //    MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi")

    //    erroreSubmitGriglia(grid);
    //    erroriSubmitGriglie = true;
    //    return;
    //}

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();

    for (let i = 0; i < currentData.length; i++) {

        var orginalFiltro_Contatti_String = currentData[i].Filtro_Contatti_String;

        if (currentData[i].orginalFiltro_Contatti_String === "0")
            currentData[i].orginalFiltro_Contatti_String = "";

        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

        currentData[i].Filtro_Contatti_String = orginalFiltro_Contatti_String;
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }


    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Alias = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Alias = kendoEscapeOggetto(updatedRecords);
        righeEliminateGrid_Alias = kendoEscapeOggetto(deletedRecords);
    }

    righeTutteGrid_Alias = kendoEscapeOggetto(righeNonCancellate);
}

function controllaRigheCompletePerSubmitProdottiXAlias(righe) {

    var nrErr = 0;
    for (let x = 0; x < righe.length; x++) {
        let item = righe[x];
        if (item.Mat_Cod_Alias == 0 || item.Mat_Des_Alias == "")
            nrErr++;
    }

    return nrErr;
}

function Imposta_ds_grid_Alias_XMultiselect(ds) {

    if (ds === undefined || ds === null || ds === "")
        return [];

    var Filtro_Contatti_Letti = [];
    for (var x = 0; x < ds.length; x++) {

        ds[x].Mat_Cod_Alias_Old = ds[x].Mat_Cod_Alias;
        ds[x].Mat_Des_Alias_Old = ds[x].Mat_Des_Alias;

        //Creo l'Array per la gestione dei Filtro_Contatti nella Multiselect della griglia
        Filtro_Contatti_Letti = ds[x].Filtro_Contatti_String.split("|");
        ds[x].Filtro_Contatti = [];
        for (var y = 0; y < Filtro_Contatti_Letti.length; y++) {
            if (Filtro_Contatti_Letti[y] !== "") {
                ds[x].Filtro_Contatti.push(Filtro_Contatti_Letti[y].toString());
            }
        }

        //Creo l'Array per la gestione dei Filtro_Contatti_Des nella Multiselect della griglia
        ds[x].Filtro_Contatti_Des = [];
        ds[x].Filtro_Contatti_Des_String = "";

        if (ElencoContatti === undefined || ElencoContatti === null)
            ElencoContatti = Carica_Contatti();

        for (var y = 0; y < Filtro_Contatti_Letti.length; y++) {
            for (var b = 0; b < ElencoContatti.length; b++) {
                if (parseInt(Filtro_Contatti_Letti[y]) === ElencoContatti[b].Cod_RisUm) {
                    ds[x].Filtro_Contatti_Des.push(ElencoContatti[b].Rag_Soc);
                }
            }
        }

        if (ds[x].Filtro_Contatti_Des.length > 0) {

            ds[x].Filtro_Contatti_Des_String = ds[x].Filtro_Contatti_Des.join(",");

        } else if (ds[x].Filtro_Contatti_Des.length === 0 && ds[x].Filtro_Contatti.length === 0) {
            //Imposto i valori vuoti
            ds[x].Filtro_Contatti.push("");
            ds[x].Filtro_Contatti_Des.push("");

            ds[x].Filtro_Contatti_String += "";
            ds[x].Filtro_Contatti_Des_String += "";
        }

    }

    return ds;

}

function Configuratore_Maschera_MateriaPrima_Alias() {

    //Se la Materia Prima è un Alias nascondo tutte le tab tranne la tab delle Traduzioni, 
    //nascondo la Categoria Commerciale , la Linea di Produzione  e
    //nascondo tutti i Dati Generali tranne la Visibilità. 

    $("#a_tab_prodotto_UC_configurazione").hide();
    $("#a_tab_prodotto_UC_storico_prezzi").hide();
    $("#a_tab_prodotto_UC_parametri_qualitativi").hide();
    $("#a_tab_prodotto_UC_dati_contabilita").hide();
    $("#a_tab_prodotto_UC_altri_dati").hide();
    $("#a_tab_prodotto_UC_alias").hide();

    $("#switch_prodotto_UC_componi_descrizione").hide();
    $(".categoria_risorsa").hide();
    $(".linea_produzione").hide();
    $(".dati_colturali").hide();
    $(".informazioni_aggiuntive").hide();


}

function Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_change(e) {
    //Se in anagrafica l'U.M. di default è numero non si possano scegliere le confezioni impostando stringa vuota la ddl
    //e mettere 0 per confezioni per contenitore

    if ($("#ddl_prodotto_UC_unita_mis_def").data("kendoDropDownList") !== undefined && $("#ddl_prodotto_UC_unita_mis_def").data("kendoDropDownList") !== null) {

        let unita_misura_def_value = parseInt(Get_KendoDDLValue("ddl_prodotto_UC_unita_mis_def"));

        if ($("#ddlconfezione") !== undefined && $("#ddlconfezione") !== null &&
            $("#ddlconfezione").data("kendoDropDownList") !== undefined && $("#ddlconfezione").data("kendoDropDownList") !== null) {

            if (unita_misura_def_value === 38) {
                $("#ddlconfezione").data("kendoDropDownList").enable(false);
                Set_KendoDDLValue("ddlconfezione", 0);
            } else {
                $("#ddlconfezione").data("kendoDropDownList").enable(true);
            }

        }

        if ($("#txtconfezione") !== undefined && $("#txtconfezione") !== null &&
            $("#txtconfezione").data("kendoNumericTextBox") !== undefined && $("#txtconfezione").data("kendoNumericTextBox") !== null) {

            if (unita_misura_def_value === 38) {
                $("#txtconfezione").data("kendoNumericTextBox").enable(false);
                Set_KendoNumTBValue("txtconfezione", 0);
            } else {
                $("#txtconfezione").data("kendoNumericTextBox").enable(true);
            }

        }

    }

}

function RipristinaRigheInseriteModificateCancellateGriglia() {

    //Pulisco le righe di tutte le griglie

    let grid_traduzioni = $("#prodotto_UC_griglia_traduzioni").data("kendoGrid");

    if (tabStripAperti.includes("a_tab_prodotto_UC_traduzioni") &&
        grid_traduzioni !== undefined && grid_traduzioni !== null) {

        righeInseriteGrid_Traduzioni = "";
        righeModificateGrid_Traduzioni = "";
        righeEliminateGrid_Traduzioni = "";
        righeTutteGrid_Traduzioni = "";

        grid_traduzioni.dataSource.read();

        grid_traduzioni.refresh();
    }

    let grid_calibri = $("#prodotto_UC_griglia_calibri").data("kendoGrid");

    if (tabStripAperti.includes("a_tab_prodotto_UC_parametri_qualitativi") && Prodotto_Edit_UC_Importato === false &&
        grid_calibri !== undefined && grid_calibri !== null) {

        righeSelezionateGrid_Calibri = "";

        grid_calibri.dataSource.read();

        grid_calibri.refresh();
    }

    let grid_indici = $("#prodotto_UC_griglia_indici").data("kendoGrid");

    if (tabStripAperti.includes("a_tab_prodotto_UC_parametri_qualitativi") && Prodotto_Edit_UC_Importato === false &&
        grid_indici !== undefined && grid_indici !== null) {

        righeSelezionateGrid_Indici = "";

        grid_indici.dataSource.read();

        grid_indici.refresh();
    }

    let grid_prezzi = $("#prodotto_UC_griglia_storico_prezzi").data("kendoGrid");

    if (tabStripAperti.includes("a_tab_prodotto_UC_storico_prezzi") && Prodotto_Edit_UC_Importato === false &&
        grid_prezzi !== undefined && grid_prezzi !== null) {

        righeInseriteGrid_Prezzi = "";
        righeModificateGrid_Prezzi = "";
        righeEliminateGrid_Prezzi = "";
        righeTutteGrid_Prezzi = "";

        grid_prezzi.dataSource.read();

        grid_prezzi.refresh();
    }

    let grid_alias = $("#prodotto_UC_griglia_alias").data("kendoGrid");
    if (tabStripAperti.includes("a_tab_prodotto_UC_alias") && grid_alias !== undefined && grid_alias !== null) {

        righeInseriteGrid_Alias = "";
        righeModificateGrid_Alias = "";
        righeEliminateGrid_Alias = "";
        righeTutteGrid_Alias = "";

        grid_alias.dataSource.read();

        grid_alias.refresh();
    }

}

/**
 * Restituisce true se l'utente deve obbligatoriamente inserire almeno 3 caratteri per ricercare il prodotto.
 * L'obbligo è disabilitato in questi casi:
 * 1) se l'utente sceglie una categoria di prodotto di materie_prime oppure
 * 2) se l'utente sceglie una specie od una varietà (per quest'ultima eccetto il valore vuoto di default)
 * */
function FiltroMinCaratteriPerRicerca() {

    // Aggiungere un controllo per disabilitare l'obbligo di 3 caratteri se l'utente sceglie una categoria di prodotto di materie_prime
    let obbligoCaratteri = true;

    var categorie = Get_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie");
    var Elem_Cod = parseInt(categorie);

    if (IsMateriaPrima(Elem_Cod)) {
        obbligoCaratteri = false;
    }
    else {
        var multiselSpecie_value = $("#multiselSpecie").data("kendoMultiSelect").value();
        //var multiselVarieta_value = $("#multiselVarieta").data("kendoMultiSelect").value();

        if (categorieProdotti_FiltrabiliXSpecieVarieta.includes(Elem_Cod) && multiselSpecie_value.length > 0) {
            obbligoCaratteri = false;
        }
    }

    return obbligoCaratteri;

}