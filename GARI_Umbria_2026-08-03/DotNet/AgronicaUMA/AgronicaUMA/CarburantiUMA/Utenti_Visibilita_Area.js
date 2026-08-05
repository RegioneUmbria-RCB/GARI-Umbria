let destroyed = [];
let elencoAree = {};

function ConfiguraGrigliaVisibilita(IDControllo) {

    var omettiAnnulla = false;
    var funzioneSubmitDaUsare = null;

    funzioneSubmitDaUsare = { funzione: InsertVisibilita, flagInsert: true, flagUpdate: true, flagDelete: false };

    var funzioniCRUD = {
        funzioneRead: Visibilita_Area_Read,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: true,
        UtenteAbilitatoCancellazione: true,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: omettiAnnulla
    };
    var idModel = "id";
    var campiKendoModel = null;
    var colonneCustomKendoGrid = new Array();

    campiKendoModel = {
        Gruppo_Cod: { editable: false, type: "number", validation: { required: true } },
        Gruppo: { editable: true, type: "string", validation: { required: true } },
        UserName: { editable: true, type: "string", validation: { required: true } },
        Area_Cod: { editable: false, type: "number", validation: { required: true } },
        Area: { editable: true, type: "string", validation: { required: true } },
        Piva_Azienda: { editable: false, type: "string", validation: { required: false } },
        CUAA: { editable: true, type: "string", validation: { required: false } },
        rag_soc: { editable: false, type: "string", validation: { required: true } },
    };

    var styleElen = /*"background-color: #C4C4EF; */"text-align: center; vertical-align: top";

    colonneCustomKendoGrid.push({
        command: {
            template: "<div class='btn-group-vertical'>" +
                "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaRigaVisibilita(this.closest('tr'),this.closest('.k-grid'))>Cancella</div>" +
                "</div>"
        }, title: "Azioni", width: "97px", headerAttributes: { style: styleElen }
    });

    var colonneKendoGrid = [
        { field: "Gruppo", title: "Gruppo", width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen }, editor: Gruppo_DropDownEditor },
        { field: "UserName", title: "UserName", width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen }, editor: UserNameTextEditor },
        { field: "Area", title: "Area", width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen }, editor: Area_DropDownEditor },
        { field: "CUAA", title: "CUAA", width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen }, editor: PivaTextEditor },
        { field: "rag_soc", title: "Rag. Soc.", width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } }
    ];

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        //columnMenu: false,
        pdf: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: elencoOnDataBound/*, funzioneDaChiamareDopoDelete: HideTabDettagli*/ };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ['Gruppo','UserName','Piva_Azienda'];

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

    var grid = $("#" + IDControllo).data("kendoGrid");

    //grid.unbind('cellClose');
    //grid.unbind('remove');

    grid.bind("cellClose", grid_cellCloseUtenti_Visibilita_Area);

}

function grid_cellCloseUtenti_Visibilita_Area(e) {
    if (e.model.dirtyFields != undefined && e.model.dirty) {

        var fieldName = e.container.find("input").attr("name");

        var grid = $("#griglia_visibilita_area").data("kendoGrid");
        var model = e.model;
        var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
        
        if (e.model.CUAA != "") {

            var indexColumnRagSoc = grid.wrapper.find(".k-grid-header [data-field=" + "rag_soc" + "]").index();

            var daCUAA = TrovaAziendaDaCUAA(e.model.CUAA);
            e.model.rag_soc = daCUAA.Item1
            e.model.Piva_Azienda = daCUAA.Item2
            row.children()[indexColumnRagSoc].innerText = daCUAA.Item1

            if (e.model.rag_soc == "") {
                row.children()[indexColumnRagSoc].classList.add("errorCell")
                $(row.children()[indexColumnRagSoc]).kendoTooltip({
                    content: "La PIVA non corrisponde ad un'azienda",
                    position: "top"
                });
            }
            else {
                if (row.children()[indexColumnRagSoc].classList.contains("errorCell")) {
                    row.children()[indexColumnRagSoc].classList.remove("errorCell");
                }
            }

        } else if (e.model.UserName != "") {

                var indexColumnUsername = grid.wrapper.find(".k-grid-header [data-field=" + "UserName" + "]").index();

                var check = CheckUtente(e.model.UserName);
                
                if (!check) {
                    row.children()[indexColumnUsername].classList.add("errorCell");
                    $(row.children()[indexColumnUsername]).kendoTooltip({
                        content: "L'utente non esiste",
                        position: "top"
                    });
                }
                else {
                    if (row.children()[indexColumnUsername].classList.contains("errorCell")) {
                        row.children()[indexColumnUsername].classList.remove("errorCell");;
                    }
            }
            
        }
        else {
            var indexColumnUsername = grid.wrapper.find(".k-grid-header [data-field=" + "UserName" + "]").index();

            if (row.children()[indexColumnUsername].classList.contains("errorCell")) {
                row.children()[indexColumnUsername].classList.remove("errorCell");
            }
        }

        if (fieldName != "" && e.model.Piva_Azienda != "") {
            var indexColumnCUAA = grid.wrapper.find(".k-grid-header [data-field=" + "CUAA" + "]").index();

            var check = CheckVisibilita(e.model.Gruppo_Cod, e.model.UserName, e.model.Area_Cod, e.model.Piva_Azienda);
            
            if (check) {
                row.children()[indexColumnCUAA].classList.add("errorCell");
                $(row.children()[indexColumnCUAA]).kendoTooltip({
                    content: "Visibilita' gia' presente per questa azienda",
                    position: "top"
                });
            }
            else {
                if (row.children()[indexColumnCUAA].classList.contains("errorCell")) {
                    row.children()[indexColumnCUAA].classList.remove("errorCell");;
                }
            }
        }
    }
}

function ddlArea_Load() {

    $('#ddlArea').kendoDropDownList({
        filter: "contains",
        dataSource: {
            transport: {
                read: RiempiDdlArea
            }
        },
        dataTextField: "area_Desc",
        dataValueField: "area_Cod",
        mapValueTo: "dataItem",
        disabeld: true,
        //dataBound: ddlArea_OnDataBound,
    });

    $('#ddlArea').data("kendoDropDownList").enable(false);

}

/*function ddlArea_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlAzienda.onchange(); //forzo l'evento di onchange
    }
}*/

function eliminaRigaVisibilita(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    kendo.confirm("Sei sicuro di voler eliminare l'elemento selezionato?")
        .done(() => {
            datiGriglia.dataSource.remove(datiRiga);
            destroyed.push(datiRiga);
            //datiGriglia.dataSource.sync();
            datiGriglia.refresh();
        })
        .fail(() => { return; });

}

function InsertVisibilita(e) {
}

function AggiornaVisibilita() {

    if ($("#griglia_visibilita_area").find(".errorCell").length != 0) {
        kendo.alert("Verificare i dati segnalati prima di salvare");
        return false;
    }

    WaitFrame.show();

    var data = $("#griglia_visibilita_area").data("kendoGrid").dataSource.data();
    let created = data.filter((el) => { return el.dirty == true});
    let updated = data.filter((el) => { return false});
    
    let modificheFatte = false;
    if (created.length > 0) {
        let check = true;
        created.forEach(x => { if ((x.UserName != "" && x.Gruppo_Cod != 0) || (x.UserName == "" && x.Gruppo_Cod == 0) || (x.rag_soc == "" && x.Piva_Azienda != "") || x.Area == "") check = false })
        if (check)
            modificheFatte = true;
        else {
            kendo.alert("Controlla i dati prima di procedere");
            WaitFrame.hide();
            return false;
        }
    }
    if (updated.length > 0) {
        modificheFatte = true;
    }
    if (destroyed.length > 0) {
        destroyed.forEach(x => { if (x.Gruppo_Cod == null) x.Gruppo_Cod = 0 })
        modificheFatte = true;
    }
    if (modificheFatte) {
        ws_InserisciVisibilita(created, updated, destroyed);
    }

    ConfiguraGrigliaVisibilita("griglia_visibilita_area")

    WaitFrame.hide();
    destroyed = [];

}

function Gruppo_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (row.UserName != "" || (row.Gruppo_Cod != 0 && row.CUAA != ""))
        return;

    PopolaElencoGruppo().then(
        elencoGruppi => {
            elencoGruppi.unshift({
                Gruppi_Utente_cod: 0,
                Gruppi_Utente_des: ''
            });

            var index = elencoGruppi.indexOf(elencoGruppi.find(x => x.Gruppi_Utente_cod === row.Gruppo_Cod))

            creaDropDownEditor(container, "Gruppi_Utente_des", "Gruppi_Utente_cod", elencoGruppi, changeGruppo).select(index);
        }
    );
}

function changeGruppo(e) {
    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    /*parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);*/

    model.Gruppo = dataItem.Gruppi_Utente_des;
    model.Gruppo_Cod = dataItem.Gruppi_Utente_cod;
    model.dirty = true;
}

function UserNameTextEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (row.Gruppo_Cod != 0 || (row.UserName != "" && row.CUAA != ""))
        return;

    $('<input data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoTextBox();
}

function Area_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (elencoAree.length == 0) {
        elencoAree.push({
            Area_Cod: 0,
            Area: ''
        });
    }
    if (row.dirty == false)
        return;
     
    creaDropDownEditor(container, "area_Desc", "area_Cod", elencoAree, changeArea);
        
}

function changeArea(e) {
    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    /*parentRow = $("#" + gridID).parents(".k-detail-row").prev();
    parentGrid = $("#tab_griglia_dettagliImpianti").data("kendoGrid");
    var parentRowItem = parentGrid.dataItem(parentRow);*/

    model.Area = dataItem.area_Desc;
    model.Area_Cod = dataItem.area_Cod;
    model.dirty = true;
}

function PivaTextEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (row.dirty == false)
        return;

    $('<input data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoTextBox();
}