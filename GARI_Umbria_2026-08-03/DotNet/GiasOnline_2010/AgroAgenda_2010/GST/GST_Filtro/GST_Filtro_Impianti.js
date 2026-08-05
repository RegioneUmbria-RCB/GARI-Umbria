

function _url(s) {

    let url = location.pathname;
    //splitto su / e prendo la parte finale
    let url_split = url.split("/")
    url = url_split[url_split.length - 1];

    return url + "/" + s;
}


function RiempiGrigliaRisultato(ut_amm) {

    $('#WaitFrame').hide();

    let kendoGrid = $("#GrigliaRisultato").getKendoGrid();

    if (!kendoGrid) {

        kendoGrid = creaGrigliaRisultato("GrigliaRisultato", ut_amm);

        let cont_elem = $("#GrigliaRisultato_Container");
        let cont_h = cont_elem.innerHeight() - parseInt(cont_elem[0].style.paddingTop) - 4;
        let grid_elem = $("#GrigliaRisultato");
        let grid_tb_h = Math.ceil(grid_elem.find(".k-grid-toolbar").outerHeight(true));
        let grid_hdr_h = Math.ceil(grid_elem.find(".k-grid-header").outerHeight(true));
        let grid_cont_h = Math.floor(cont_h - (grid_tb_h + grid_hdr_h));

        grid_elem.find(".k-grid-content").css("height", grid_cont_h + "px");

    }

    kendoGrid.dataSource.read();
}


function caricaGrigliaRisultato(options) {

    let jsondata = [];

    $.ajax({
        type: "POST",
        url: _url("LeggiGrigliaImpianti"),
        async: false,
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            jsondata = JSON.parse(msg.d);
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

    options.success(jsondata);
}


function creaGrigliaRisultato(elemId, ut_amm) {

    var funzioniCRUD = {
        funzioneRead: function (options) { options.success([]); }
    };
    var idModel = "UNID_Impianto";
    var campiKendoModel = {
        UNID_Impianto: { type: "number" },
        Referente: { type: "string" },
        Indirizzo: { type: "string" },
        Comune: { type: "string" },
        Prov: { type: "string" },
        Regione: { type: "string" },
        IndApp: { type: "string" },
        Specie: { type: "string" },
        Tipologia: { type: "string" },
        Superficie: { type: "number" },
        Lat: { type: "number" },
        Lng: { type: "number" }
    };
    var colonneKendoGrid = [
        {
            field: "Referente",
            filterable: { multi: true },
            menu: false
        },
        {
            field: "Indirizzo",
            filterable: false,
            menu: false
        },
        {
            field: "Comune",
            filterable: { multi: true },
            menu: false
        },
        {
            title: "Prov.",
            field: "Prov",
            filterable: { multi: true },
            menu: false
        },
        {
            title: "Regione",
            field: "Regione",
            filterable: { multi: true },
            menu: false
        },
        {
            title: "Indirizzo appezzamento",
            field: "IndApp",
            filterable: false,
            menu: false
        },
        {
            title: "Specie",
            field: "Specie",
            filterable: { multi: true },
            menu: false
        },
        {
            title: "Tipologia",
            field: "Tipologia",
            filterable: { multi: true },
            menu: false
        },
        {
            title: "Superficie",
            field: "Superficie",
            filterable: false,
            menu: false,
            headerAttributes: {
                style: "text-align: right;"
            },
            attributes: {
                style: "text-align: right;"
            },
            format: "{0:0.000}"
        },
        {
            title: "Baricentro",
            headerAttributes: {
                style: "text-align: center;"
            },
            attributes: {
                style: "text-align: center;"
            },
            columns: [
                {
                    title: "Lat",
                    field: "Lat",
                    filterable: false,
                    menu: false,
                    headerAttributes: {
                        style: "text-align: right;"
                    },
                    attributes: {
                        style: "text-align: right;"
                    },
                    format: "{0:0.0000000000000}"
                },
                {
                    title: "Lng",
                    field: "Lng",
                    filterable: false,
                    menu: false,
                    headerAttributes: {
                        style: "text-align: right;"
                    },
                    attributes: {
                        style: "text-align: right;"
                    },
                    format: "{0:0.0000000000000}"
                }
            ]
        },
        {
            title: "ID",
            field: "UNID_Impianto",
            filterable: false,
            menu: false
        }
    ];
    var parametriPerLettura = [];
    var parametriDataSource = {
        sort: {
            field: "Referente",
            dir: "asc"
        },
        aggregate: [{ field: "Superficie", aggregate: "sum" }]
    };

    var parametriKendoGrid = {
        groupable: false,
        scrollable: true,
        editable: false,
        resizable: true,
        //columnMenu: { columns: false },
        pdf: false,
        excel: true,
        pageable: false,
        //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
    };

    var funzioniPrimaDopoEventi = { };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(elemId, // rappresenta l'ID del div a cui si associa la griglia
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

    let grid = $("#" + elemId).getKendoGrid();

    let options = grid.getOptions();
    options.dataSource.transport.read = caricaGrigliaRisultato;
    options.autoBind = false;
    options.dataBound = _aggiornaRiepilogo;
    grid.setOptions(options);

    grid.thead.find("[data-field=Indirizzo]>.k-header-column-menu").remove();
    grid.thead.find("[data-field=IndApp]>.k-header-column-menu").remove();
    grid.thead.find("[data-field=Superficie]>.k-header-column-menu").remove();
    grid.thead.find("[data-field=Lat]>.k-header-column-menu").remove();
    grid.thead.find("[data-field=Lng]>.k-header-column-menu").remove();
    grid.thead.find("[data-field=UNID_Impianto]>.k-header-column-menu").remove();

    grid.thead.find(".k-header-column-menu .k-icon.k-i-more-vertical").removeClass("k-i-more-vertical").addClass("k-i-filter")

    $("#" + elemId).find(".k-grid-footer").remove();

    grid.bind("columnMenuInit", _gridColumnMenuInit);


    let gridTB = $("#" + elemId).find(".k-grid-toolbar");

    let divRiepilogo = document.createElement("div");
    divRiepilogo.id = elemId + "-riepilogo";
    divRiepilogo.style.paddingLeft = "25px";
    divRiepilogo.style.flexGrow = "1";
    divRiepilogo.style.justifyContent = "end";
    divRiepilogo.style.fontSize = "13px";
    divRiepilogo.style.fontWeight = "bold";
    divRiepilogo.style.alignSelf = "stretch";
    gridTB.append(divRiepilogo);

    let divRiepNumero = document.createElement("div");
    divRiepNumero.style.padding = "0px 10px";
    divRiepNumero.style.border = "1px solid #ccc";
    divRiepNumero.style.borderRadius = "4px";
    divRiepNumero.style.backgroundColor = "#fefefe";
    divRiepNumero.style.display = "flex";
    divRiepNumero.style.alignItems = "center";
    divRiepilogo.appendChild(divRiepNumero);

    let spanNumeroLbl = document.createElement("span");
    spanNumeroLbl.style.paddingRight = "5px";
    spanNumeroLbl.style.color = "#aeaeae";
    let spanNumeroTxt = document.createElement("span");
    spanNumeroTxt.className = "riepilogo-numero";
    divRiepNumero.appendChild(spanNumeroLbl);
    divRiepNumero.appendChild(spanNumeroTxt);

    spanNumeroLbl.textContent = "Totale impianti";
    spanNumeroTxt.textContent = "...";

    if (ut_amm) {

        divRiepNumero.style.marginRight = "2px";

        let divRiepSuperficie = document.createElement("div");
        divRiepSuperficie.style.padding = "0px 10px";
        divRiepSuperficie.style.border = "1px solid #ccc";
        divRiepSuperficie.style.borderRadius = "4px";
        divRiepSuperficie.style.backgroundColor = "#fefefe";
        divRiepSuperficie.style.display = "flex";
        divRiepSuperficie.style.alignItems = "center";
        divRiepSuperficie.style.marginLeft = "2px";
        divRiepilogo.appendChild(divRiepSuperficie);

        let spanSuperficieLbl = document.createElement("span");
        spanSuperficieLbl.style.paddingRight = "5px";
        spanSuperficieLbl.style.color = "#aeaeae";
        let spanSuperficieTxt = document.createElement("span");
        spanSuperficieTxt.className = "riepilogo-superficie";
        divRiepSuperficie.appendChild(spanSuperficieLbl);
        divRiepSuperficie.appendChild(spanSuperficieTxt);

        spanSuperficieLbl.textContent = "Totale superficie";
        spanSuperficieTxt.textContent = "...";
    }

    return grid;
}


function _aggiornaRiepilogo(e) {

    let toolbar = e.sender.element.find(".k-grid-toolbar");

    if (toolbar.length === 0) {
        return;
    }

    let spanNum = toolbar.find(".riepilogo-numero");
    if (spanNum.length > 0) {

        spanNum[0].textContent = kendo.toString(e.sender.dataSource.view().length, "0");
    }

    let spanSup = toolbar.find(".riepilogo-superficie");
    if (spanSup.length > 0) {

        let sum = e.sender.dataSource.aggregates().Superficie.sum;
       
        spanSup[0].textContent = kendo.toString(sum, "0.000") + " ha";
    }
}


function _gridColumnMenuInit(e) {

    let items = e.container.find(".k-menu").children(".k-menu-item");

    items.each(function (i, el) {

        if ($(el).find(".k-filterable").length === 0) {

            el.remove();
        } else {

            if ($(el).hasClass("k-last")) {

                $(el).addClass("k-first");
            }
        }
    });

    items = e.container.find(".k-menu-separator").remove();

    if (e.field === "Referente"
        || e.field === "Comune"
        || e.field === "Prov"
        || e.field === "Regione"
        || e.field === "Specie"
        || e.field === "Tipologia") {

        let filterMultiCheck = e.container.find(".k-filterable").data("kendoFilterMultiCheck");

        filterMultiCheck.container.empty();
        filterMultiCheck.checkSource.sort({ field: e.field, dir: "asc" });
        filterMultiCheck.checkSource.data(filterMultiCheck.checkSource.view().toJSON());
        filterMultiCheck.createCheckBoxes();
    }
}


function clickInterferenze() {

    let filtered = [];

    $.each($("#GrigliaRisultato").getKendoGrid().dataSource.view(), function (i, elem) {
        filtered.push({
            UNID_Impianto: elem.UNID_Impianto
        });
    });


    $.ajax({
        type: "POST",
        url: _url("ScriviGrigliaImpianti"),
        async: false,
        data: JSON.stringify({ jsondata: JSON.stringify(filtered) }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function SalvaPreventivo() {
    WaitFrame.show();
    SalvaEstrazioneWS("Preventivo");
    WaitFrame.hide();
}

function SalvaConsuntivo() {
    WaitFrame.show();
    SalvaEstrazioneWS("Consuntivo");
    WaitFrame.hide();
}