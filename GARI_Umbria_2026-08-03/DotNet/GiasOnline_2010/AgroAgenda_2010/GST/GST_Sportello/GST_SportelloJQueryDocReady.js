

$(document).ready(function () {

    $("#kendoWindowiFrameGeneric").css("display", "none");
    let m_btm = ($(".AgronicaFooter").innerHeight() ?? 0) + 10;
    $("#grid-container").css("margin-bottom", m_btm + "px");


    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)//,  Larghezza calendario come il campo di input...
    });

    $("#inSpecie").kendoMultiSelect({
        dataTextField: "DesSpecie",
        dataValueField: "IdSpecie",
        dataSource: [],
        autoClose: true,
        placeholder: "Selezionare le specie da includere.."
    });

    creaGrigliaPassaggiSportello();

    $("#EditSportello").kendoDialog({
        width: "75%",
        title: "",
        modal: true,
        visible: false,
        closable: false,
        actions: [
            {
                text: "OK",
                action: function (e) {
                    return salvaSportello();
                }
            },
            {
                text: "Annulla"
            }
        ],
        open:
            function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                $("body").addClass("ob-no-scroll");
            },
        close: function (e) {
            $("body").removeClass("ob-no-scroll");
        }
    });

    $("#EditSportello").removeClass("hidden");


    let funzioniCRUD = {
        funzioneRead: CaricaGrigliaSportelli
    };

    let campiKendoModel = {
        id_sportello: { editable: false, type: "number" },
        sportello: { editable: false, type: "string" },
        specie: { editable: false, type: "string" },
        validitaInizio: { editable: false, type: "date" },
        validitaFine: { editable: false, type: "date" }
    };

    let colonneKendoGrid = [
        {
            command: [
                stdKendoGridComandoModifica(function (e) {
                    let itd = this.dataItem($(e.currentTarget).closest("tr"));
                    editSportello(itd.id_sportello);
                }, "fa fa-pencil fa-lg"),
                stdKendoGridComandoCancella(function (e) {
                    let itd = this.dataItem($(e.currentTarget).closest("tr"));
                    confdlg("Eliminazione sportello", "Eliminare definitivamente lo sportello <b>" + itd.sportello + "</b>?", eliminaSportello, itd, true);
                }, "fa fa-trash fa-lg")
            ],
            title: "Operazioni",
            width: "160px"
        },
        { field: "sportello", title: "Sportello" },
        {
            field: "specie",
            title: "Specie",
            attributes: {
                "class": "ellipsis"
            }
        },
        {
            title: "Validità", headerAttributes: { style: "text-align:center;" },
            columns: [
                { field: "validitaInizio", title: "Dal", format: "{0:dd/MM/yyyy}", width: 100 },
                { field: "validitaFine", title: "Al", format: "{0:dd/MM/yyyy}", width: 100 }
            ]
        }
    ];

    let parametriKendoGrid = {
        excel: false,
        pdf: false,
        pageable: false,
        groupable: false,
        reorderable: true,
        filterable: false,
        sortable: false,
        selectable: "row",
        columnMenu: false,
        btnEliminaTuttiFiltri: false,
        toolbarCommands: ["template_AddSportello"]
    };

    let funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            let gridId = e.sender.element[0].id;
            let grid = $("#" + gridId).data("kendoGrid");
            grid.autoFitColumn(1);
        }
    };

    creaKendoGrid("GridSportelli", // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        "Id_Sportello", //idModel, // chiave riga
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        null, //parametriPerLettura, // parametri da passare alla lettura
        {}, //parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        false, //mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        [] //colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    $(window).resize(ridimensiona);

    ridimensiona();
});

function ridimensiona() {

    let win_h = $(window).height();
    let ftr_h = ($(".AgronicaFooter").outerHeight() ?? 0) + 10;
    let top_h = $("#grid-container").position().top; 

    let h = win_h - top_h - ftr_h;
    h = Math.max(500, h);
    $("#grid-container").innerHeight(h);

    $("#GridSportelli").data("kendoGrid").resize();
}


function creaGrigliaPassaggiSportello() {

    let funzioniCRUD = {
        funzioneRead: function (options) {

            let passaggi = [];
            let s_pass = $(cFasiSportello).val();
            if (s_pass !== "") {
                passaggi = JSON.parse(s_pass);
            }
            options.success(passaggi);

        },
        funzioneSubmit: {
            funzione: submitPassaggio,
            flagInsert: true,
            flagUpdate: true,
            flagDelete: true
        },
        UtenteAbilitatoInserimentoModifica: true,
        UtenteAbilitatoCancellazione: true
    };

    let campiKendoModel = {
        IdPassaggio: { editable: false, nullable: false },
        Passaggio: { defaultValue: { IdPassaggio: 0, DesPassaggio: "Seleziona..." } },
        DataInizio: { type: "date", nullable: false },
        DataFine: { type: "date", nullable: false }
    };

    let colonneKendoGrid = [
        {
            command: [
                {
                    iconClass: {
                        edit: "fa fa-pencil fa-lg",
                        update: "fa fa-check fa-lg",
                        cancel: "fa fa-close fa-lg"
                    },
                    className: "blockModifica",
                    name: "edit",
                    text: {
                        edit: "&nbsp",
                        update: "&nbsp",
                        cancel: "&nbsp"
                    }
                },
                stdKendoGridComandoCancella(function (e) {
                    let itd = this.dataItem($(e.currentTarget).closest("tr"));
                    let that = this;
                    confdlg("Eliminazione fase sportello",
                        "Eliminare definitivamente la fase <b> " + itd.Passaggio.DesPassaggio + "</b> per lo sportello?",
                        function () {
                            that.dataSource.remove(itd);
                        });
                }, "fa fa-trash fa-lg")
            ],
            title: "Operazioni",
            width: "160px"
        },
        { field: "Passaggio", title: "Fase", editor: passaggiDropDownEditor, template: "#=Passaggio.DesPassaggio#" },
        { field: "DataInizio", title: "Data inizio", format: "{0:dd/MM/yyyy}" },
        { field: "DataFine", title: "Data fine", format: "{0:dd/MM/yyyy}" },
        {
            field: "Passaggio",
            title: "Visibilità impianti",
            template: "#=(Passaggio.VisImpianti ? 'Tutti' : 'Solo interni') #",
            width: "11em",
            headerAttributes: { "class": "gridColumnCenter" },
            attributes: { "class": "gridColumnCenter change_ddl" },
            editable: function (e) { return false; }
        },
        {
            field: "Passaggio",
            title: "Operazioni permesse",
            template: "#=(Passaggio.OpPermesse ? 'Si' : 'No') #",
            width: "13em",
            headerAttributes: { "class": "gridColumnCenter" },
            attributes: { "class": "gridColumnCenter change_ddl" },
            editable: function (e) { return false; }
        },
        {
            field: "Passaggio",
            title: "Comunica operazioni",
            template: "#=(Passaggio.LogOperazioni ? 'Si' : 'No') #",
            width: "13em",
            headerAttributes: { "class": "gridColumnCenter" },
            attributes: { "class": "gridColumnCenter change_ddl" },
            editable: function (e) { return false; }
        },
        {
            field: "Passaggio",
            title: "Notifiche attive",
            template: "#=(Passaggio.Notifiche ? 'Si' : 'No') #",
            width: "10em",
            headerAttributes: { "class": "gridColumnCenter" },
            attributes: { "class": "gridColumnCenter change_ddl" },
            editable: function (e) { return false; }
        }
    ];

    let parametriKendoGrid = {
        editable: {
            mode: "inline"
        },
        //colonneCustomKendoGrid: colCustKendoGrid,
        excel: false,
        pdf: false,
        pageable: false,
        groupable: false,
        reorderable: false,
        filterable: false,
        sortable: false,
        columnMenu: false,
        btnEliminaTuttiFiltri: false
    };

    let funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: function (e) {
            let gridId = e.sender.element[0].id;
            $("#" + gridId + " .k-grid-update").removeClass("k-primary");
            $("#" + gridId + " .k-grid-update").addClass("gridCommandUpdate");
            $("#" + gridId + " .k-grid-cancel").addClass("gridCommandCancel");
        }
    };

    creaKendoGrid("GridFasiSportello", // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        "IdPassaggio", //idModel, // chiave riga
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        null, //parametriPerLettura_, // parametri da passare alla lettura
        {}, //parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        false, //mostraRigheCancellate_, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        null //colonneDisabilitateSoloInModifica_ // colonne non modificabili in modifica["colA", "colB", ...]
    );

    $("#GridFasiSportello .k-grid-content").css("height", "200px");

}


function passaggiDropDownEditor(container, options) {

    let s_pass = $(cFasiDDL).val();
    let pass = [];
    if (s_pass !== "") {

        pass = JSON.parse(s_pass);

    } else {

        pass = leggiPassaggi();

        if (pass.length > 0) {

            $(cFasiDDL).val(JSON.stringify(pass));

        }
    }

    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: false,
            dataTextField: "DesPassaggio",
            dataValueField: "IdPassaggio",
            dataSource: {
                data: pass
            },
            change: function (e) {
                let model = options.model;
                let grid = $("#GridFasiSportello").data("kendoGrid");
                let row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
                let rowChildren = $(row).children(".change_ddl");
                $.each(rowChildren, function (i, child) {

                    let col = grid.columns[child.cellIndex];
                    let template = col.template;
                    let kendoTemplate = kendo.template(template);
                    rowChildren.eq(i).html(kendoTemplate(model));
                });
            }
        });
}

