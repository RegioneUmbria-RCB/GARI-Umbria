
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////     GRIGLIE   //////// ///////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function ConfiguraGrigliaCarico(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === StatoNonLettura();
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === StatoNonLettura();

    var funzioniCRUD = {
        funzioneRead: CaricaGrigliaCarico,
        funzioneSubmit: { funzione: SubmitGrid_Carico, flagInsert: StatoNonLetturaBoolean(), flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc 
    };
    
    var idModel = "Id_Mov_Det";
    
    var campiKendoModel = {
        Id_Mov_Det: { editable: false, type: "number" },
        Tipo_BC: { editable: true, type: "number", validation: { required: true } },
        Tipo_Des_BC: { editable: true, type: "string", validation: { required: true } },
        Elem_Cod: { editable: false, type: "number" },
        Mat_Cod: { editable: false, type: "number" },
        Mat_Des: { editable: true, type: "string", validation: { required: true } },
        Tara: { editable: true, type: "number", validation: { required: true } },
        Lotto: { editable: true, type: "string", validation: { required: true } },
        Qta_Extra: { editable: false, type: "number", validation: { required: true } },
        Giacenza: { editable: true, type: "number" },
        Qta: { editable: true, type: "number", validation: { required: true } }
    };

    var colonneKendoGrid = [
        { field: "Tipo_Des_BC", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Tipo", "Tipo"), editor: Tipo_Carico_DropDownEditor },
  //      { field: "Cod_Articolo", title: "Cod. Articolo", filterable: { multi: true, search: true }, editor: Codice_Articolo_Carico_DropDownEditor },
        { field: "Mat_Des", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Descrizione", "Descrizione"), filterable: { multi: true, search: true }, editor: Bene_Confezionamento_Carico_DropDownEditor },
        { field: "Tara", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Tara", "Tara"), format: "{0:n3}", editor: editKendoNumericTextBoxForGridInline },
        //{ field: "Qta_Extra", title: "Tara", format: "{0:n4}" },
        { field: "Giacenza", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Giacenza", "Giacenza"), format: "{0:n0}" } //, editor: ChiudiCellaCarico }
    ];

    if ($(cCau_Mov_BC_Principale).val() === CAU_SCARICO) {
        colonneKendoGrid.push({ field: "Qta", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Scarico", "Scarico"), format: "{0:n0}", editor: editKendoNumericTextBoxForGridInline });
    } else {
        colonneKendoGrid.push({ field: "Qta", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Carico", "Carico"), format: "{0:n0}", editor: editKendoNumericTextBoxForGridInline });
    }

    var colCustKendoGrid = "";

    if (StatoNonLettura() === "True") {

        colCustKendoGrid = [
            {
                command: [
                    {
                        iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: " ", update: " ", cancel: " " }
                    },
                    {
                        iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "destroy", text: ""
                    }
                ],
                title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Operazioni", "Operazioni")
            }
        ];
    }
    else {
        colCustKendoGrid = [
            {
                command: [],
                title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Operazioni", "Operazioni")
            }
        ];
    }


    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        editable: {
            mode: "inline"
        },
        colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        groupable: false
        //,
        //filterable: {
        //    mode: "row"
        //},
    }; 


    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: DataBoundGrigliaCarichi, funzioneDaChiamareDopoEdit: EditGrigliaCarichi, funzioneDaChiamareDopoSave: SaveGrigliaCarichi };


    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = ["Giacenza"];

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

function DataBoundGrigliaCarichi(e) {
}

function SaveGrigliaCarichi(e) {
    
}


function ConfiguraGrigliaCaricoEredita(IDControllo) {

    var omettiAnnulla = true;

    var funzioneSubmitDaUsare = null;

    funzioneSubmitDaUsare = { funzione: null, flagInsert: false, flagUpdate: false, flagDelete: false };

    var funzioniCRUD = {
        funzioneRead: CaricaGrigliaCaricoEredita,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "False",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "False",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: omettiAnnulla
    };

    var idModel = "Id_Mov_Det";
    var campiKendoModel = null;

    campiKendoModel = {
        Id_Mov_Det: { editable: false, type: "number" },
        Tipo_BC: { editable: false, type: "number" },
        Tipo_Des_BC: { editable: false, type: "string", validation: { required: true } },
        Elem_Cod: { editable: false, type: "number" },
        Mat_Cod: { editable: false, type: "number" },
        Mat_Des: { editable: false, type: "string", validation: { required: true } },
        Tara: { editable: true, type: "number", validation: { required: true } },
        Lotto: { editable: false, type: "string", validation: { required: true } },
        Qta_Extra: { editable: false, type: "number", validation: { required: true } },
        Giacenza: { editable: false, type: "number", validation: { required: true } },
        Qta: { editable: false, type: "number", validation: { required: true } }
    };

    var colonneKendoGrid = [
        { field: "Tipo_Des_BC", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Tipo", "Tipo"), editor: Tipo_Carico_DropDownEditor, daDuplicare: true },
        //{ field: "Cod_Articolo", title: "Cod. Articolo", filterable: { multi: true, search: true }, editor: Codice_Articolo_Carico_DropDownEditor },
        { field: "Mat_Des", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Descrizione", "Descrizione"), filterable: { multi: true, search: true }, editor: Bene_Confezionamento_Carico_DropDownEditor, daDuplicare: true },
        { field: "Tara", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Tara", "Tara"), format: "{0:n3}" },
        //{ field: "Qta_Extra", title: "Tara", format: "{0:n4}" },
        { field: "Giacenza", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Giacenza", "Giacenza"), format: "{0:n0}" }
    ];

    if ($(cCau_Mov_BC_Principale).val() === CAU_SCARICO) {
        colonneKendoGrid.push({ field: "Qta", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Scarico", "Scarico"), format: "{0:n0}" });
    } else {
        colonneKendoGrid.push({ field: "Qta", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Carico", "Carico"), format: "{0:n0}" });
    }

    var parametriPerLettura = []; //[AggiornaImpianti];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        groupable: false
        //columnMenu: false,
        // colonneCustomKendoGrid: colCustKendoGrid
    }; // { salvaRipristinaPersonalizzazioni: { url: pathCoreWS }};



    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundGrigliaEredita };
    // var funzioniPrimaDopoEventi = {};

    var mostraRigheCancellate = false;
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


function ConfiguraGrigliaScarico(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === StatoNonLettura();
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === StatoNonLettura();

    var funzioniCRUD = {
        funzioneRead: CaricaGrigliaScarico,
        funzioneSubmit: { funzione: SubmitGrid_Scarico, flagInsert: StatoNonLetturaBoolean(), flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };

    var idModel = "Id_Mov_Det";
    var campiKendoModel = null;

    campiKendoModel = {
        Id_Mov_Det: { editable: false, type: "number" },
        Tipo_BC: { editable: true, type: "number" },
        Tipo_Des_BC: { editable: true, type: "string", validation: { required: true } },
        Elem_Cod: { editable: false, type: "number" },
        Mat_Cod: { editable: true, type: "number" },
        Mat_Cod_Scarico: { editable: true, type: "number" },        
        Mat_Des_Scarico: { editable: true, type: "string", validation: { required: true } },
        Tara: { editable: true, type: "number", validation: { required: true } },
        Lotto: { editable: true, type: "string", validation: { required: true } },
        Qta_Extra: { editable: false, type: "number", validation: { required: true } },
        Giacenza: {  editable: true, type: "number", validation: { required: true } },
        Qta: { editable: true, type: "number", validation: { required: true } }
    };

    var colonneKendoGrid = [
        { field: "Tipo_Des_BC", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Tipo", "Tipo"), editor: Tipo_Scarico_DropDownEditor },
        //{ field: "Cod_Articolo", title: "Cod. Articolo", filterable: { multi: true, search: true }, editor: Codice_Articolo_Scarico_DropDownEditor },
        { field: "Mat_Des_Scarico", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Descrizione", "Descrizione"), filterable: { multi: true, search: true }, editor: Bene_Confezionamento_Scarico_DropDownEditor },
        { field: "Tara", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Tara", "Tara"), format: "{0:n3}", editor: editKendoNumericTextBoxForGridInline },
        //{ field: "Qta_Extra", title: "Tara", format: "{0:n4}" },
        { field: "Giacenza", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Giacenza", "Giacenza"), format: "{0:n0}" },
        { field: "Qta", title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Scarico", "Scarico"), format: "{0:n0}", editor: editKendoNumericTextBoxForGridInline }
    ];

    var colCustKendoGrid = "";

    if (StatoNonLettura() === "True") {
        colCustKendoGrid = [
            {
                command: [
                    {
                        iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: " ", update: " ", cancel: " " }
                    },
                    {
                        iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "destroy", text: ""
                    }
                ],
                title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Operazioni", "Operazioni")
            }
        ];
    }
    else {
        colCustKendoGrid = [
            {
                command: [],
                title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Operazioni", "Operazioni")
            }
        ];
    }


    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        editable: {
            mode: "inline"
        },
        colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        groupable: false
        //,
        //filterable: {
        //    mode: "row"
        //},
    }; 
    

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: DataBoundGrigliaScarichi, funzioneDaChiamareDopoEdit: EditGrigliaScarichi, funzioneDaChiamareDopoSave: SaveGrigliaScarichi };
    

    var mostraRigheCancellate = false;
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
 
function DataBoundGrigliaScarichi(e) {
}

function EditGrigliaScarichi(e) {

    if (e.model.isNew()) {
        if (e.container.find("input[name=Tipo_BC]").data("kendoDropDownList") !== undefined) {
            if (e.container.find("input[name=Tipo_BC]").val() === "0") {
                e.container.find("input[name=Tipo_BC]").val("3").change();
                e.model.set("Tipo_BC", "3");
            }
        }
    }

    e.container.find("input[name='Giacenza']").each(function () { $(this).attr("disabled", "disabled") });       

}

function SaveGrigliaScarichi(e) {
    
}


function StatoNonLettura() {

    if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
        return "False";
    }
    else {
        return "True";
    }

}

function StatoNonLetturaBoolean() {

    if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
        return false;
    }
    else {
        return true;
    }

}


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////     CARICO   ///////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        function Tipo_Carico_DropDownEditor(container, options) {

            creaDropDownEditor(container, "Tipo_Des_BC", "Tipo_BC", Elenco_Tipo_Beni_Confezionamento, changeTipo_Carico); 
        }

        function changeTipo_Carico(e) {

            var dataItem = e.sender.dataItem();
            var grid = $("#tab_griglia_carico").data("kendoGrid");
            var model = grid.dataItem(this.element.closest("tr"));
            var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

            model.Tipo_BC = dataItem.Tipo_BC;
            model.Tipo_Des_BC = dataItem.Tipo_Des_BC;
            model.Elem_Cod = dataItem.Elem_Cod;
            model.Mat_Cod = 0;
            model.Mat_Des = "";
            model.Cod_Articolo = "";
            model.Tara = 0;
            model.Giacenza = 0;
            model.Qta_Extra = 0;

            var tipo = 0;

            if (model.Tipo_BC !== undefined) {
                tipo = model.Tipo_BC;
            }

            Elenco_Beni_Confezionamento = Elenco_Beni_Confezionamento_Riempi(false, tipo);

            var BeniConfezionamento_DDL = $('input[name$="Mat_Cod"]').data("kendoDropDownList");
            BeniConfezionamento_DDL.setDataSource(Elenco_Beni_Confezionamento);



           // kendoFastRedrawRow(grid, row);
        }

        function Bene_Confezionamento_Carico_DropDownEditor(container, options) {

            var tipo = 0;

            if (options.model.Tipo_BC !== undefined) {
                tipo = options.model.Tipo_BC;
            }

            Elenco_Beni_Confezionamento = Elenco_Beni_Confezionamento_Riempi(false, tipo);

            creaDropDownEditor(container, "Mat_Des", "Mat_Cod", Elenco_Beni_Confezionamento, changeBene_Confezionamento_Carico);

        }

        function changeBene_Confezionamento_Carico(e) {

            var dataItem = e.sender.dataItem();
            var grid = $("#tab_griglia_carico").data("kendoGrid");
            var model = grid.dataItem(this.element.closest("tr"));
            var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

            model.Elem_Cod = dataItem.Elem_Cod;
            model.Mat_Cod = dataItem.Mat_Cod;
            model.Mat_Des = dataItem.Mat_Des;
            model.Tipo_BC = dataItem.Tipo_BC;
            model.Tipo_Des_BC = dataItem.Tipo_Des_BC;
            model.Cod_Articolo = dataItem.Cod_Articolo;    
            model.set("Tara", dataItem.Tara);    
            model.set("Giacenza", ControllaGiacenza(Sa_Cod_Carico, Fabbricato_Cod_Carico, model.Mat_Cod));
            model.Qta_Extra = dataItem.Qta_Extra;
            

        }

        function kEventoSelezionaRigaCarico(e) {
            var checked = this.checked,
                row = $(this).parents("tr"),
                grid = $("#tab_griglia_carico").data("kendoGrid"),
                dataItem = grid.dataItem(row);

            dataItem.Selected = checked;
         
        }






        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////     SCARICO   //////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        function Tipo_Scarico_DropDownEditor(container, options) {

            creaDropDownEditor(container, "Tipo_Des_BC", "Tipo_BC", Elenco_Tipo_Beni_Confezionamento, changeTipo_Scarico);

        }

        function changeTipo_Scarico(e) {

            var dataItem = e.sender.dataItem();
            var grid = $("#tab_griglia_scarico").data("kendoGrid");
            var model = grid.dataItem(this.element.closest("tr"));
            var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

            model.Tipo_BC = dataItem.Tipo_BC;
            model.Tipo_Des_BC = dataItem.Tipo_Des_BC;
            model.Elem_Cod = dataItem.Elem_Cod;
            model.Mat_Cod_Scarico = 0;
            model.Mat_Des_Scarico = "";
            model.Mat_Cod = 0;
            model.Cod_Articolo = "";
            model.Tara = 0;
            model.Giacenza = 0;
            model.Qta_Extra = 0;


            var tipo = 0;

            if (model.Tipo_BC !== undefined) {
                tipo = model.Tipo_BC;
            }

            Elenco_Beni_Confezionamento = Elenco_Beni_Confezionamento_Riempi(false, tipo);
            
            var BeniConfezionamento_DDL = $('input[name$="Mat_Cod_Scarico"]').data("kendoDropDownList");
            BeniConfezionamento_DDL.setDataSource(Elenco_Beni_Confezionamento);

            
        }

        function Bene_Confezionamento_Scarico_DropDownEditor(container, options) {

            var tipo = 0;

            if (options.model.Tipo_BC !== undefined) {
                tipo = options.model.Tipo_BC;
            }

            Elenco_Beni_Confezionamento = Elenco_Beni_Confezionamento_Riempi(false, tipo);

            creaDropDownEditor(container, "Mat_Des_Scarico", "Mat_Cod_Scarico", Elenco_Beni_Confezionamento, changeBene_Confezionamento_Scarico);

        }

        function changeBene_Confezionamento_Scarico(e) {

            var dataItem = e.sender.dataItem();
            var grid = $("#tab_griglia_scarico").data("kendoGrid");
            var model = grid.dataItem(this.element.closest("tr"));
            var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

            model.Elem_Cod = dataItem.Elem_Cod;
            model.Mat_Cod_Scarico = dataItem.Mat_Cod_Scarico;
            model.Mat_Des_Scarico = dataItem.Mat_Des_Scarico;
            model.Mat_Cod = dataItem.Mat_Cod_Scarico;
            model.Tipo_BC = dataItem.Tipo_BC;
            model.Tipo_Des_BC = dataItem.Tipo_Des_BC;
            model.Cod_Articolo = dataItem.Cod_Articolo;
            model.set("Tara", dataItem.Tara);    

            cmbProvenienza_change();
            let w_giac = ControllaGiacenza(Sa_Cod_Scarico, Fabbricato_Cod_Scarico, model.Mat_Cod_Scarico);
            model.set("Giacenza", w_giac);            
            model.Qta_Extra = dataItem.Qta_Extra;
            
        }

        function kEventoSelezionaRigaScarico(e) {
            var checked = this.checked,
                row = $(this).parents("tr"),
                grid = $("#tab_griglia_scarico").data("kendoGrid"),
                dataItem = grid.dataItem(row);

            dataItem.Selected = checked;

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////   FUNZIONI COMUNI   ////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        function ControllaGiacenza(sa_cod, fabbricato_cod, mat_cod) {

            var Giacenza = Leggi_Giacenza(sa_cod, fabbricato_cod, mat_cod);

            return Giacenza;

        }




function RiempiCmbDestinazione(options) {
    options.success(elencoMagazziniOmni);
}

function cmbDestinazione_change(e) {

    var ddl = KendoDDL("cmbDestinazione");

    if (ddl.dataItem() !== undefined) {
        Sa_Cod_Carico = ddl.dataItem().SA_COD;
        Fabbricato_Cod_Carico = ddl.dataItem().Fabbricato_Cod;
    } else {
        Sa_Cod_Carico = 0;
        Fabbricato_Cod_Carico = 0;
    }

}



function RiempiCmbProvenienza(options) {
    options.success(elencoMagazziniOmni);
}

function cmbProvenienza_change(e) {

    var ddl = KendoDDL("cmbProvenienza");

    if (ddl.dataItem() !== undefined) {
        Sa_Cod_Scarico = ddl.dataItem().SA_COD;
        Fabbricato_Cod_Scarico = ddl.dataItem().Fabbricato_Cod;
    } else {
        Sa_Cod_Scarico = 0;
        Fabbricato_Cod_Scarico = 0;
    }

}





function kendo_Operazioni_onDataBoundGrigliaEredita(e) {
    coloraRigheOperazioni("#tab_griglia_carico_eredita", e);
}

function coloraRigheOperazioni(grid_elem, eventArgs) {

    var grid = $(grid_elem).data('kendoGrid');
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);
        this.className += " kendoRiga_Arancione";
    });

}


function Imposta_Numero_Colli() {
    Numero_Colli_Carico = Imposta_Numero_Colli_Griglia("#tab_griglia_carico") + Imposta_Numero_Colli_Griglia("#tab_griglia_carico_eredita");
    Numero_Colli_Scarico = Imposta_Numero_Colli_Griglia("#tab_griglia_scarico");
}

function Imposta_Numero_Colli_Griglia(griglia) {

    var totale = 0;
    var grid = $(griglia).data("kendoGrid");
    if (grid !== undefined) {
        var currentData = grid.dataSource.data();
        if (currentData.length > 0) {

            for (var i = 0; i < currentData.length; i++) {
                if (currentData[i].deleted !== true && currentData[i].Qta !== 0) {
                    totale = totale + currentData[i].Qta;

                }
            }

        }
    }

    return totale;
}






////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////// SUBMIT SINGOLE GRIGLIE ///////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function SubmitGrid_Carico(options) {

    var grid = $("#tab_griglia_carico").data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheValidePerSubmitGrid(options.data.created) +
        controllaRigheValidePerSubmitGrid(options.data.updated);

    if (nrErr > 0) {
        if (nrErr === 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxBeniConfezionamentoUC, "EsisteUnaRigaNonCompletaGrigliaIngressoBeni",
                "Esiste una riga con dati non completi nella griglia ingresso beni di confezionamento."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(resxBeniConfezionamentoUC, "EsistonoNRigheNonCompleteGrigliaIngressoBeni",
                "Esistono {0} righe con dati non completi nella griglia ingresso beni di confezionamento."), nrErr), "DIV_Messaggi");

        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    for (let i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

    }

    for (let j = 0; j < grid.dataSource._destroyed.length; j++) {
        deletedRecords.push(grid.dataSource._destroyed[j].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        righeInseriteGrid_Scarico = "";
        righeModificateGrid_Scarico = "";
        righeCancellateGrid_Scarico = "";

        righeInseriteGrid_Carico = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Carico = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_Carico = kendoEscapeOggetto(deletedRecords);

        if (Sa_Cod_Carico !== 0 && Fabbricato_Cod_Carico !== 0) {
            AggiornaEffettivo(false);
        } else {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxBeniConfezionamentoUC, "NecessarioMappareUnMagazzinoDestinazioneImballi",
                "Non è stato impostato un magazzino di destinazione per gli imballi. Se l'elenco è vuoto è necessario mappare un magazzino per gli imballi all'interno della configurazione del modulo."),
                "DIV_Messaggi");
        }

    }
}

function SubmitGrid_Scarico(options) {

    var grid = $("#tab_griglia_scarico").data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheValidePerSubmitGrid(options.data.created) +
        controllaRigheValidePerSubmitGrid(options.data.updated);

    if (nrErr > 0) {
        if (nrErr === 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxBeniConfezionamentoUC, "EsisteUnaRigaNonCompletaGrigliaBeniResi",
                "Esiste una riga con dati non completi nella griglia beni di confezionamento resi."), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(TraduzioneMultiResx(resxBeniConfezionamentoUC, "EsisteNRigheNonCompleteGrigliaBeniResi",
                "Esistono {0} righe con dati non completi nella griglia beni di confezionamento resi."), nrErr), "DIV_Messaggi");

        // erroreSubmitFattoriVariazioneParametriQualitativi(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    for (let i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

    }

    for (let j = 0; j < grid.dataSource._destroyed.length; j++) {
        deletedRecords.push(grid.dataSource._destroyed[j].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        righeInseriteGrid_Carico = "";
        righeModificateGrid_Carico = "";
        righeCancellateGrid_Carico = "";

        righeInseriteGrid_Scarico = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Scarico = kendoEscapeOggetto(updatedRecords);
        righeCancellateGrid_Scarico = kendoEscapeOggetto(deletedRecords);

        if (Sa_Cod_Scarico !== 0 && Fabbricato_Cod_Scarico !== 0) {
            AggiornaEffettivo(false);
        } else {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxBeniConfezionamentoUC, "NecessarioMappareUnMagazzinoProvenienzaImballi",
                "Non è stato impostato un magazzino di provenienza per gli imballi. Se l'elenco è vuoto è necessario mappare un magazzino per gli imballi all'interno della configurazione del modulo."),
                "DIV_Messaggi");
        }
        
    }
}


function BloccaMagazzinoCarico() {
    if (NumeroRighe("#tab_griglia_carico") === 0 && NumeroRighe("#tab_griglia_carico_eredita") === 0) {
        KendoDDL("cmbDestinazione").enable(true);
    }
    else {
        KendoDDL("cmbDestinazione").enable(false);
    }
}

function BloccaMagazzinoScarico(grid) {

    if (NumeroRighe(grid) === 0 && StatoNonLettura() === "True") {
        KendoDDL("cmbProvenienza").enable(true);
        //KendoNumTB("inNumDocScarico").enable(true);
        //Set_KendoNumTBValue("inNumDocScarico", 0);  //se avevo una riga e poi la cancello (quindi ho eliminato il doc), devo ri-azzerare il numero
        //$("#inNumDocSinScarico").attr("readonly", false);
        //$("#inNumDocDesScarico").attr("readonly", false);
        //$("#btnRefreshNumDocScarico").show();
    }
    else {
        KendoDDL("cmbProvenienza").enable(false);
        //KendoNumTB("inNumDocScarico").enable(false);
        //$("#inNumDocSinScarico").attr("disabled", true);
        //$("#inNumDocDesScarico").attr("disabled", true);
        //KendoDDL("inNumDocDDLScarico").enable(false);
        //$("#btnRefreshNumDocScarico").hide();
    }

}


function NumeroRighe(Id_Grid) {

    var n = 0;
    var grid = $(Id_Grid).data("kendoGrid");
    if (grid !== undefined && grid !== null) {
        var currentData = grid.dataSource.data();
        n = currentData.length;        
    }

    return n;
}


function btnRefreshNumDocScarico_click() {
    RefreshNumDocScarico();
}

function RefreshNumDocScarico() {
    let data = kendo.parseDate($("#inDataEmissione").val());

    if (data !== undefined && data !== null) {
        let docNumeroSin = $("#inNumDocSinScarico").val().toUpperCase();
        let docNumeroDes = $("#inNumDocDesScarico").val().toUpperCase();
        let anno = data.getFullYear();

        let lastNumDoc = 0;
        lastNumDoc = LastNumDocumento($(cIdPiva).val(), $(cLav_Cod_Scarico).val(), anno, docNumeroSin, docNumeroDes, 0);
        let newNumDoc = lastNumDoc + 1;
        Set_KendoNumTBValue("inNumDocScarico", newNumDoc);
    }
}

function EditGrigliaCarichi(e) {

    if (e.model.isNew()) {
        if (e.container.find("input[name=Tipo_BC]").data("kendoDropDownList") !== undefined) {
            if (e.container.find("input[name=Tipo_BC]").val() === "0") {
                e.container.find("input[name=Tipo_BC]").val("3").change();
                e.model.set("Tipo_BC", "3");
            }
        }
    }

    e.container.find("input[name='Giacenza']").each(function () { $(this).attr("disabled", "disabled") });       
}

function ImpostaTipoOpTestataScarico() {
    if (StatoNonLettura() === "False") {
        tipoOpTestataScarico = enum_TipoOperazioneDB.Lettura.value;

        //Blocco tutto
        RiBloccoNumeroDoc(true, "inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocLockScarico");

        //non posso sbloccare, neanche volendo
        $("#inNumDocLockScarico").data("kendoSwitch").enable(false);

    } else if (NumeroRighe("#tab_griglia_scarico") === 0) {

        tipoOpTestataScarico = enum_TipoOperazioneDB.Scrittura.value;

        RiBloccoNumeroDoc(true, "inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocLockScarico");

        //do la possibilità di sbloccare
        $("#inNumDocLockScarico").data("kendoSwitch").enable(true);

    } else {
        tipoOpTestataScarico = enum_TipoOperazioneDB.Modifica.value;

        //Blocco tutto
        RiBloccoNumeroDoc(true, "inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocLockScarico");

        //non posso sbloccare, neanche volendo
        $("#inNumDocLockScarico").data("kendoSwitch").enable(false);
    }

}

function inNumDocScarico_change(e) {

    //Qui potremmo anche dire che una volta assegnato non si cambia più?!?

    //TODO: da rivedere!!!!

    if (tipoOpTestataScarico === enum_TipoOperazioneDB.Modifica.value && e.sender.value() === 0) {
        //se sono in modifica il numero 0 non è consentito!
        alert("Il numero documento deve essere maggiore di 0");
    }

    MostraNumeroDocCompleto("inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocShowScarico");

}

function inNumDocSinScarico_change(e) {
    MostraNumeroDocCompleto("inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocShowScarico");
}

function inNumDocDesScarico_change(e) {
    MostraNumeroDocCompleto("inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocShowScarico");
}

function inNumDocDDLScarico_change(e) {

    let dataItem = KendoDDL("inNumDocDDLScarico").dataItem();
    if (dataItem !== undefined && dataItem !== null) {

        //metto prefisso e suffisso in sin e des
        $("#inNumDocSinScarico").val(dataItem.Doc_Numero_Sin);
        $("#inNumDocDesScarico").val(dataItem.Doc_Numero_Des);

        //ricostruisco il numero completo
        MostraNumeroDocCompleto("inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocShowScarico");

    }
}

function inNumDocLockScarico_change(e) {
    let state = e.checked;

    if (state === true) {

        //Ho appena bloccato ==> rendo non editabili i numeri
        RiBloccoNumeroDoc(false, "inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocLockScarico");

    } else {
        //Ho appena sbloccato ==> rendo editabili i numeri

        let kendoConfirm = $("<div></div>").kendoConfirm({
            title: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Attenzione", "Attenzione"),
            messages: { okText: TraduzioneMultiResx(resxBeniConfezionamentoUC, "Si", "Sì"), cancel: TraduzioneMultiResx(resxBeniConfezionamentoUC, "No", "No") },
            content: TraduzioneMultiResx(resxBeniConfezionamentoUC, "ConfermaModificaNumeroDoc", "Permettere modifica manuale del numero di documento. Proseguire?")
        }).data("kendoConfirm");

        kendoConfirm.result.done(function () {
            DecidoDiSbloccareNumero("inNumDocDDLScarico", "inNumDocScarico", "inNumDocSinScarico", "inNumDocDesScarico", "inNumDocShowScarico");
            //Disabilito il lock, in modo che non si possa ri-bloccare finché non si salva
            $("#inNumDocLockScarico").data("kendoSwitch").enable(false);
        });

        kendoConfirm.result.fail(function () {
            setKendoSwitch("inNumDocLockScarico", true);
        });

        kendoConfirm.open();

    }
}
