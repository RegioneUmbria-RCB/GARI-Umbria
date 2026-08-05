var indirizzohttp = "./InvestimentoCatasto.aspx";


/* 
 * Investimento Catasto
 * 
 * */


/**
 * 
 */
function CaricaInformazioniInvestimento() {

}


function onDataBindingRighe() {

}

function letturaDatiInvestimentoCatastoKendoGridKendo() {

    var d1 = $("#Txt_Data_DA").data("kendoDatePicker").value();
    var d2 = $("#Txt_Data_A").data("kendoDatePicker").value();

    if (d1 == undefined) { d1 = new Date(1900, 0, 1); }
    if (d2 == undefined) { d2 = new Date(2100, 11, 31); }

    var mostraRipartoAppezzamenti = $("#chk_RipartoAppezzamenti").is(":checked");
    var mostraAppezzaSenzaRiparto = $("#chk_AppezzaSenzaRiparto").is(":checked");
    var mostraParticelleSenzaRiparto = $("#chk_ParticelleSenzaRiparto").is(":checked");

    //var selezioneSuiCampi = $("#chk_Switch").is(":checked");
    var selezioneSuiCampi = $("#switch-app-campi").is(":checked");
    var tuttoIlCatastoInArchivio = $("#chk_tuttoIlCatastoInArchivio").is(":checked");

    var sintesiCUAAEstremiCatastali = $("#chk_sintesiCUAAEstremiCatastali").is(":checked");

    var o = {
        piva: piva,
        txtDataIniDist: d1,
        txtDataFinDist: d2,
        mostraRipartoAppezzamenti: mostraRipartoAppezzamenti,
        mostraAppezzaSenzaRiparto: mostraAppezzaSenzaRiparto,
        mostraParticelleSenzaRiparto: mostraParticelleSenzaRiparto,

        selezioneSuiCampi: selezioneSuiCampi,

        sintesiCUAAEstremiCatastali: sintesiCUAAEstremiCatastali,
        tuttoIlCatastoInArchivio: tuttoIlCatastoInArchivio
    }

    ajaxAgronica("InvestimentoCatasto.aspx/InvestimentoCatastoElabora", JSON.stringify(o),
        function (risposta) {
            $("#hdInvestimentoCatastoKendoGrid").val(risposta.RispostaStringa);
            InvestimentoCatastoKendoGridKendo("divInvestimentoCatastoKendoGrid");
            Personalizza_Griglia_Investimento();
        }, null);

}

function kReadInvestimentoCatastoKendoGrid_rows(options) {

    var data = $('#hdInvestimentoCatastoKendoGrid').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadInvestimentoCatastoKendoGrid_col() {

    var data = $('#hdInvestimentoCatastoKendoGrid').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function applicaFooter(parametriDataSource) {
    //var selezioneSuiCampi = $("#chk_Switch").is(":checked");
    var selezioneSuiCampi = $("#switch-app-campi").is(":checked");
    var sintesiCUAAEstremiCatastali = $("#chk_sintesiCUAAEstremiCatastali").is(":checked");
    var tuttoIlCatastoInArchivio = $("#chk_tuttoIlCatastoInArchivio").is(":checked");

    if (!selezioneSuiCampi) {
        if (!sintesiCUAAEstremiCatastali) {
            parametriDataSource.aggregate.push({ field: "Area", aggregate: "sum" });
            parametriDataSource.aggregate.push({ field: "SemTrap_Superficie", aggregate: "sum" });
        }
        parametriDataSource.aggregate.push({ field: "sup_app", aggregate: "sum" });
        parametriDataSource.aggregate.push({ field: "ParticellaSuperficieHa", aggregate: "sum" });
    } else {
        if (!tuttoIlCatastoInArchivio) {
            parametriDataSource.aggregate.push({ field: "SemTrap_Superficie", aggregate: "sum" });
            parametriDataSource.aggregate.push({ field: "ParticellaSuperficieHa", aggregate: "sum" });
        } else {
            parametriDataSource = {}
        }
    }
}

function kReadInvestimentoCatastoKendoGrid_mod() {

    var data = $('#hdInvestimentoCatastoKendoGrid').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}


function InvestimentoCatastoKendoGridonDataBindingRighe() {

}


function appezzamento_riparto_catasto_coloraRidimensiona(e) {
    coloraRigheImpostaPulsanti("#divInvestimentoCatastoKendoGrid", e);
    kendoGridFlatResizeColonne();
}

function AppezzaDaChiave(chiave) {
    if (chiave != undefined) {
        var v1 = chiave.split("_");
        return v1[0] + "_" + v1[1] + "_" + v1[2];
    }
}

function coloraRigheImpostaPulsanti(grid_elem, eventArgs) {

    var grid = $(grid_elem).data('kendoGrid');
    var items = eventArgs.sender.items();

    var colori = ["#FFF", "#CCFFCC"];
    var chiavePrecedente = "";
    var coloreBool = false;

    for (var i = 0; i < items.length; i++) {

        var dataItem = grid.dataItem(items[i]);

        var cc = AppezzaDaChiave(dataItem.chiave);

        if (!(cc === chiavePrecedente || i === 0)) {
            coloreBool = !coloreBool
        }
        chiavePrecedente = cc;

        //imposta il colore
        $(items[i]).css("background-color", colori[Number(coloreBool)]);

    }
}



function kendoGridFlatResizeColonne() {

    var grid = $("#divInvestimentoCatastoKendoGrid").data("kendoGrid");

    for (i = 0; i < grid.columns.length; i++) {

        if (grid.columns[i].width === undefined) {

            grid.autoFitColumn(i);

        }

    }
}


function InvestimentoCatastoKendoGridKendo(divInvestimentoCatastoKendoGrid) {

    var funzioniCRUD = {
        funzioneRead: kReadInvestimentoCatastoKendoGrid_rows
    };
    var idModel = "kendoKey";
    var campiKendoModel = kReadInvestimentoCatastoKendoGrid_mod();
    var colonneKendoGrid = kReadInvestimentoCatastoKendoGrid_col();
    var parametriPerLettura = [];
    var parametriDataSource = {
        pagesize: 50,
        aggregate: []
    };

    applicaFooter(parametriDataSource);


    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        groupable: false,
        scrollable: true,
        sortable: true,
        reorderable: true,
        resizable: true,
        columnMenu: true,
        filterable: { multi: true, search: true },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        pdf: false,
        excel: true,
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDataBinding: InvestimentoCatastoKendoGridonDataBindingRighe,
        funzioneDaChiamareDopoDataBound: function (e) {
            var gridId = e.sender.element[0].id;
            appezzamento_riparto_catasto_coloraRidimensiona(e);
        }
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["kendoKey"];

    creaKendoGrid(divInvestimentoCatastoKendoGrid, // rappresenta l'ID del div a cui si associa la griglia
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

// ####################### VISTE #######################

//Salvataggio/caricamento viste
function RiempiListaViste(options) {
    options.success(Lista_Viste_Investimento());
}

//Chiamata alla selezione di una vista salvata dalla lista
function CambiaListaViste_Investimento(e) {
    var nome_vista = KendoDDL("selListaViste").value();
    if (nome_vista && nome_vista !== "") {
        Leggi_Vista_Investimento(nome_vista, false);
    }
}

//Salva la configurazione attuale
function Salva_Vista_Investimento(nomeVista) {

    var parametriGriglia = getParametriGriglia_Investimento(location.pathname, "divInvestimentoCatastoKendoGrid");

    var mostraRipartoAppezzamenti = $("#chk_RipartoAppezzamenti").is(":checked");
    var mostraAppezzaSenzaRiparto = $("#chk_AppezzaSenzaRiparto").is(":checked");
    var mostraParticelleSenzaRiparto = $("#chk_ParticelleSenzaRiparto").is(":checked");
    var sintesiCUAAEstremiCatastali = $("#chk_sintesiCUAAEstremiCatastali").is(":checked");
    var tuttoIlCatastoInArchivio = $("#chk_tuttoIlCatastoInArchivio").is(":checked");
    //var selezioneSuiCampi = $("#chk_Switch").is(":checked");
    var selezioneSuiCampi = $("#switch-app-campi").is(":checked");

    var parametriVista = {
        nome: nomeVista,
        piva: piva,
        mostraRipartoAppezzamenti: mostraRipartoAppezzamenti,
        mostraAppezzaSenzaRiparto: mostraAppezzaSenzaRiparto,
        mostraParticelleSenzaRiparto: mostraParticelleSenzaRiparto,
        sintesiCUAAEstremiCatastali: sintesiCUAAEstremiCatastali,
        tuttoIlCatastoInArchivio: tuttoIlCatastoInArchivio,
        selezioneSuiCampi: selezioneSuiCampi,
        personalizzazioni: parametriGriglia
    };

    var parametri = kendo.stringify({ "nome": nomeVista, "vista": kendo.stringify(parametriVista) });
    ajaxAgronica("InvestimentoCatasto.aspx/SalvaVista", parametri,
        function (risposta) {
            var risp = risposta.RispostaStringa;
            //aggiungo la vista appena salvata all'elenco di quelli disponibili
            var listaViste = KendoDDL("selListaViste");
            var dataSource = listaViste.dataSource;
            var trovaVista = dataSource.data().filter(function (dataItem) { return dataItem.cod === nomeVista; });
            if (trovaVista.length === 0) dataSource.add({ cod: nomeVista, desc: nomeVista });
            listaViste.select(function (dataItem) { return dataItem.cod === nomeVista; });
            kendo.alert("Vista salvata correttamente.");
        },
        function (risposta) {
            console.log("Errore nel salvataggio impostazioni vista Investimento progetti: " + risposta.Errore);
        }
    );
}

//Recupero i parametri della KendoGrid per poterla salvare
function getParametriGriglia_Investimento(pagina, nomeDiv) {
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

//Recupero la lista delle viste salvate disponibili
function Lista_Viste_Investimento() {
    var risultato_lettura;
    ajaxAgronicaSync("InvestimentoCatasto.aspx/ListaViste",
        kendo.stringify({ "categoriaVista": "a" }),
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            var objVuoto = { "cod": "", "desc": "" };
            risp.unshift(objVuoto);
            risultato_lettura = risp;
        }, null
    );
    return risultato_lettura;
}

/*Chiamato alla selezione di una vista dall'elenco per modificare 
tutti i parametri della pagina in modo da poterlo lanciare come è stato salvato*/
function Leggi_Vista_Investimento(nomeVista, esegui) {
    var parametri = kendo.stringify({ "nome": nomeVista });
    ajaxAgronica("InvestimentoCatasto.aspx/LeggiVista", parametri,
        function (risposta) {
            if (risposta.RispostaOK) {
                if (risposta.RispostaStringa !== "") {
                    var params = JSON.parse(risposta.RispostaStringa);
                    Personalizza_Vista_Investimento(params);
                    if (esegui) letturaDatiInvestimentoCatastoKendoGridKendo();
                } else kendo.alert("Nessuna personalizzazione vista");
            } else console.log("Errore nella lettura impostazioni viste Investimento progetti: " + risposta.Errore);
        }, null
    );
}

var personalizzazioni = null;

//Modfica i parametri della pagina per farli coincidere con quelli della vista selezionata
function Personalizza_Vista_Investimento(parametri) {

    //$("#chk_Switch").prop('checked', parametri.selezioneSuiCampi);
    $("#switch-app-campi").prop('checked', parametri.selezioneSuiCampi);
    $("#chk_RipartoAppezzamenti").prop('checked', parametri.mostraRipartoAppezzamenti);
    $("#chk_AppezzaSenzaRiparto").prop('checked', parametri.mostraAppezzaSenzaRiparto);
    $("#chk_ParticelleSenzaRiparto").prop('checked', parametri.mostraParticelleSenzaRiparto);
    $("#chk_sintesiCUAAEstremiCatastali").prop('checked', parametri.sintesiCUAAEstremiCatastali);
    $("#chk_tuttoIlCatastoInArchivio").prop('checked', parametri.tuttoIlCatastoInArchivio);

    if (parametri.personalizzazioni !== undefined)
        personalizzazioni = parametri.personalizzazioni;

}

//elimina la vista attualmente selezionata
function Cancella_Vista_Investimento(nomeVista) {
    var parametri = kendo.stringify({ "nome": nomeVista });
    ajaxAgronica("InvestimentoCatasto.aspx/CancellaVista", parametri,
        function (risposta) {
            var risp = risposta.RispostaStringa;
            KendoDDL("selListaViste").dataSource.read();
            // KendoDDL("selListaViste").value(-1);
            KendoDDL("selListaViste").refresh();
            kendo.alert("Vista eliminata correttamente.");
        },
        function (risposta) {
            console.log("Errore nella cancellazione vista investimento progetti: " + risposta.Errore);
        }
    );
}

// ripristina personalizzazioni griglia report vendite
function Personalizza_Griglia_Investimento() {
    var grid = $('#divInvestimentoCatastoKendoGrid').data('kendoGrid');
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

function FiltraImpiantiConFiltroRicercaNG() {
    var cat = $('#estrazionePerCampi').val()
    var param = kendo.stringify({
        "piva": currentPiva,
        "cat": cat
    });

    ajaxAgronica(indirizzohttp + "/Link_Pagina_FiltroRicercaNG",
        param,
        function (risposta) {
            apriFinestraFiltroRicercaNG(risposta.RispostaStringa)
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        });
}

function apriFinestraFiltroRicercaNG(url) {
    window.addEventListener('message', chiudiFinestraFiltroRicercaNG);

    $(document.body).append('<div id="filtro_ricerca_ng"></div>');

    var entity = $('#estrazionePerCampi').val() == "campo" ? 'Campi' : "Impianti"

    $('#filtro_ricerca_ng').kendoWindow({
        title: "Filtra {0}".format(entity),
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#filtro_ricerca_ng').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function chiudiFinestraFiltroRicercaNG(event) {
    let kWin = $('#filtro_ricerca_ng').data("kendoWindow");
    let urlKWin = kWin.options.content.url;

    if (verificaOriginSecondaria(window, urlKWin, event) &&
        (event != null && event.data != null) && (event.data.messaggio != null) &&
        event.data.messaggio.includes("chiudiWindowGiasNG")) {

        CreaEntitaDaChiavi(event.data.inData.chiavi, event.data.inData.tipoEntita)

        kWin.close();
    }
}

function CreaEntitaDaChiavi(chiavi, tipoEntita) {

    var param = kendo.stringify({
        "chiavi": chiavi,
        "tipoEntita": tipoEntita
    });

    ajaxAgronica(indirizzohttp + "/CreaEntitaDaChiavi",
        param,
        function (risposta) {
            location.reload() //forziamo il reload della pagina per poter leggere gli impianti 
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        }, null, true);
}