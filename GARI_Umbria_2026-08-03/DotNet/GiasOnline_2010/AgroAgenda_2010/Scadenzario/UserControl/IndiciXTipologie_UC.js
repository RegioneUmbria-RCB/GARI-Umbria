var resxIndiciXTipologieUC = [];

function TraduciIndiciXTipologieUC(chiave, testoAlternativo) {
    if (resxIndiciXTipologieUC.length === 0) {
        resxIndiciXTipologieUC.push(readResxFile("Scadenzario/UserControl/App_LocalResources/IndiciXTipologie_UC.ascx.resx", "IndiciXTipologie_UC_ws_client.js"));
        resxIndiciXTipologieUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "IndiciXTipologie_UC_ws_client.js"));
    }
    return TraduzioneMultiResx(resxIndiciXTipologieUC, chiave, testoAlternativo);
}

function InXTip_UC_LsbTipologie_onchange() {
    //Estraggo i controlli
    var obj_lsbTipologie = $("#InXTip_UC_LsbTipologie");
    var obj_txtTipologia = $("#InXTip_UC_TxtNomeTipologia");

    //Estraggo nome e valore di ciò che è selezionato
    var nomeTipologia = $(obj_lsbTipologie).find('option:selected').text();
    valoreTipologia_IndiciXTipologie = $(obj_lsbTipologie).val();

    //Nascondo lo switch e la label di obbligo

    $("#Switch_Campo_Obbligatorio").hide();

    //Elimino gli elementi delle due listbox quando cambio area e disattivo lo switch obbligatorio
    let listBoxI = $("#Indici").data("kendoListBox");

    if (listBoxI !== undefined) {
        listBoxI.remove(listBoxI.items());
    }

    let listBoxIxT = $("#IndiciXTipo").data("kendoListBox");
    if (listBoxIxT !== undefined) {
        listBoxIxT.remove(listBoxIxT.items());
    }

    if ($("#cb_obbligatorio_liv").data("kendoSwitch") !== undefined) {
        $("#cb_obbligatorio_liv").data("kendoSwitch").enable(false);
        $("#cb_obbligatorio_liv").data("kendoSwitch").check(false);
    }


    if (valoreTipologia_IndiciXTipologie !== null) {
        //Se è stato selezionato qualcosa...
        $(obj_txtTipologia).val(nomeTipologia);

        if ((valoreTipologia_IndiciXTipologie !== undefined) || (valoreTipologia_IndiciXTipologie != "")) {
             //Richiamo caricamento delle ListBox
            ListBox_Indice_Load();
        }
    }
    else {
        //Se non è stato selezionato nulla...
        $(obj_txtTipologia).val("");
    }

}

////FUNZIONE PER CARICARE l'ELENCO DELLE AZIENDE
//function InXTip_UC_ddlAzienda_Load() {

//    var ds = new kendo.data.DataSource({
//        transport: { read: RiempiDdlAzienda }
//    });
//    $('#InXTip_UC_ddlAzienda').kendoDropDownList({
//        filter: "contains",
//        dataSource: ds,
//        dataTextField: "rag_soc",
//        dataValueField: "piva",
//        optionLabel: { "rag_soc": "SELEZIONA...", "piva": "" },
//        autoWidth: true,
//        change: InXTip_UC_ddlAzienda_change,
//        dataBound: InXTip_UC_ddlAzienda_OnDataBound
//    });
//}

//function InXTip_UC_ddlAzienda_change(e) {

//    //listaIndici = RiempiListBoxIndice();

//    //var listBox = $("#Indici").data("kendoListBox");

//    //if (listBox !== undefined) {
//    //    $("#Indici").data("kendoListBox").setDataSource(new kendo.data.DataSource({
//    //        data: listaIndici
//    //    }));

//    //    listBox.refresh();
//    //} else {

//    //    $("#Indici").kendoListBox({
//    //        dataSource: listaIndici,
//    //        //draggable: true,
//    //        connectWith: "IndiciXTipo",
//    //        //dropSources: ["IndiciXTipo"],
//    //        dataTextField: "TitoloIndice",
//    //        dataValueField: "ID_Indice",
//    //        toolbar: {
//    //            tools: ["transferTo", "transferFrom"]
//    //        },
//    //        add: onAdd,
//    //        //reorder: onReorder,
//    //        remove: onRemoveIndiciXTipo
//    //    });

//    //}
//    InXTip_UC_LsbAree_Load();
//}

//function InXTip_UC_ddlAzienda_OnDataBound(e) {
//    var ds = this.dataSource.data();
//    if (ds.length == 1) {
//        this.select(1); //seleziono l'elemento 
//        InXTip_UC_ddlAzienda_onchange(e); //forzo l'evento di onchange
//    }
//}
//ListBox Indice
function ListBox_Indice_Load() {
    listaIndici = RiempiListBoxIndice();

    var listBox = $("#Indici").data("kendoListBox");

    if (listBox !== undefined) {
        $("#Indici").data("kendoListBox").setDataSource(new kendo.data.DataSource({
            data: listaIndici
        }));

        listBox.refresh();
    } else {

        var tools = [];

        if ($("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True") {
            tools = ["transferTo", "transferFrom"];
        }

        $("#Indici").kendoListBox({
            dataSource: listaIndici,
            //draggable: true,
            connectWith: "IndiciXTipo",
            //dropSources: ["IndiciXTipo"],
            dataTextField: "TitoloIndice",
            dataValueField: "ID_Indice",
            toolbar: {
                tools: tools
            },
            add: onAdd,
            change: onChangeIndici,
            //reorder: onReorder,
            remove: onRemoveIndiciXTipo
        });

    }

    listaIndiciXTipo = LeggiIndicixTipologia(null);
    listBoxIndXTip = $("#IndiciXTipo").data("kendoListBox");

    if (listBoxIndXTip !== undefined) {
        //Caricamento ListBox IndiciXTipo
        $("#IndiciXTipo").data("kendoListBox").setDataSource(new kendo.data.DataSource({
            data: listaIndiciXTipo
        }));

        listBoxIndXTip.refresh();
    }
    else {
        $("#IndiciXTipo").kendoListBox({
            dataSource: listaIndiciXTipo,
            //draggable: true,
            connectWith: "Indici",
            dataTextField: "TitoloIndice",
            dataValueField: "ID_Indice",
            //dropSources: ["Indici"],
            toolbar: {
                tools: ["moveUp", "moveDown"]
            },
            add: onAdd,
            change: onChangeIndiciXTipo,
            reorder: onReorder, 
            remove: onRemoveIndiciXTipo
        });

        listBoxIndXTip = $("#IndiciXTipo").data("kendoListBox");
    }

    let listBoxIndici = $("#Indici").data("kendoListBox");
    if (listBoxIndici !== undefined) {
        $("#Indici").data("kendoListBox").setDataSource(new kendo.data.DataSource({
            data: listaIndici
        }));
    }

    //Cancella le righe della Listbox Indici che sono nella listbox Indici x tipologia
    if (listBoxIndXTip !== undefined) {
        for (var z = 0; z < listBoxIndXTip.dataSource._data.length; z++) {
            for (var i = 0; i < listBoxIndici.dataSource._data.length; i++) {
                var ElementoListBoxIndXTipo = listBoxIndici.dataSource._data[i];
                if ((listBoxIndXTip.dataSource._data[z].ID_Indice) === (listBoxIndici.dataSource._data[i].ID_Indice)) {
                    listBoxIndici.dataSource.remove(ElementoListBoxIndXTipo);
                }
            }
        }
        listBoxIndXTip.refresh();
        listBoxIndici.refresh();
     }

}

function onChangeIndici(e) {
    $("#Switch_Campo_Obbligatorio").hide();
}

function onAdd(e) {
    if (e.sender.options.connectWith != "IndiciXTipo") {
        Insert_IndiceXTipologia(e);
    } 
}

function onReorder(e) {
    if (RigaAggiornata_IndiciXTipologie != "") {
        e.dataItems[0].ChkObbligatorio_Tipologia = RigaAggiornata_IndiciXTipologie[0].ChkObbligatorio_Tipologia;
    }
    e.preventDefault();
    var dataSource = e.sender.dataSource;

    var dataItem = e.dataItems[0]
    var index = dataSource.indexOf(dataItem) + e.offset;
    dataSource.remove(dataItem);
    dataSource.insert(index, dataItem);
    e.sender.wrapper.find("[data-uid='" + dataItem.uid + "']").addClass(GIAS_K_STATE_SELECTED);
    var RigheAggiornate = [];
    for (var x = 0; x < dataSource._data.length; x++) {
        RigheAggiornate[x] = { ID_Indice: dataSource._data[x].ID_Indice, Ordinamento: x + 1, ChkObbligatorio_Tipologia: dataSource._data[x].ChkObbligatorio_Tipologia, ID_Area: valoreArea_IndiciXTipologie, ID_Tipologia: valoreTipologia_IndiciXTipologie }
    }
    Modifica_Ordine_Alert_IndicexTipologia( RigheAggiornate);
}

function onChangeIndiciXTipo(e) {
    e.preventDefault();
    var element = e.sender.select();
    var dataItem = e.sender.dataItem(element[0]);//Prendo la riga cliccata nella listbox
    if (dataItem != undefined) {
        RigaAggiornata_IndiciXTipologie = LeggiIndicixTipologia(dataItem.ID_Indice);

        $("#Switch_Campo_Obbligatorio").show();

        var switchObbligatorio = $("#cb_obbligatorio_liv").data("kendoSwitch");
        switchObbligatorio.enable(false);
        if (RigaAggiornata_IndiciXTipologie.length > 0) {
            //Imposto lo switch in base alla riga selezionata se è obbligatoria oppure no
            if (RigaAggiornata_IndiciXTipologie[0].ChkObbligatorio === 1) {
                switchObbligatorio.check(true);
                switchObbligatorio.enable(false);
            } else {
                switchObbligatorio.enable(true);
                if (RigaAggiornata_IndiciXTipologie[0].ChkObbligatorio_Tipologia !== 1) {
                    switchObbligatorio.check(false);
                } else {
                    switchObbligatorio.check(true);
                }
            }
        }
    }
}

function cb_obbligatorio_liv_change(e) {
    var switchObbligatorio = $("#cb_obbligatorio_liv").data("kendoSwitch");
    if (e.checked == true) {
        RigaAggiornata_IndiciXTipologie[0].ChkObbligatorio_Tipologia = 1;
        switchObbligatorio.enable(true);
    }
    else {
        RigaAggiornata_IndiciXTipologie[0].ChkObbligatorio_Tipologia = 0;
    }
    Modifica_Obbligo_Alert_IndicexTipologia(RigaAggiornata_IndiciXTipologie[0].ChkObbligatorio_Tipologia, RigaAggiornata_IndiciXTipologie[0].ID_Indice);
}

function onRemoveIndiciXTipo(e) {
    //Controllo che non venga invocato il cancella insieme all'insert
    if (e.sender.options.connectWith != "IndiciXTipo") {
        Delete_IndiceXTipologia(e);

        //Riordino la listbox Indice
        let lstIndice = $("#Indici").data("kendoListBox").dataSource._data;
        lstIndice.sort(Riordina("TitoloIndice"));
        $("#Indici").data("kendoListBox").setDataSource(new kendo.data.DataSource({
            data: lstIndice 
        }));
        $("#Indici").data("kendoListBox").refresh();

        $("#Switch_Campo_Obbligatorio").hide();
    }
}

//Funzione di riordinamento  
function Riordina(prop) {
    return function (a, b) {
        if (a[prop] > b[prop]) {
            return 1;
        } else if (a[prop] < b[prop]) {
            return -1;
        }
        return 0;
    }
} 


// Creazione Kendo Grid grid_tipologiexindice
function popolaGridTipologiexIndice(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: CaricoGrigliaIndicixTipologia,
        funzioneSubmit: { funzione: null, flagInsert: false, flagUpdate: false, flagDelete: false},
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true
    };

    var idModel = "Id_Indice";
    var campiKendoModel = {
        Id_Indice: { editable: false, type: "number" },
        TitoloIndice: { editable: false, type: "string" },
        NomeArea: { editable: false, type: "string"},
        NomeTipologia: { editable: false, type: "string"},
        Obbligatorio: { editable: false, type: "string"}
    };
    var colonneKendoGrid = [       
        //{
        //    field: "Ordinamento", title: "Ordine"
        //},
        {
            field: "NomeArea", title: TraduciIndiciXTipologieUC("Categoria", "Categoria"), filterable: { multi: true, search: true }
        },
        {
            field: "NomeTipologia", title: TraduciIndiciXTipologieUC("Tipologia", "Tipologia"), filterable: { multi: true, search: true }
        },
        {
            field: "TitoloIndice", title: TraduciIndiciXTipologieUC("Indice", "Indice"), filterable: { multi: true, search: true }
        },
        {
            field: "Obbligatorio", title: TraduciIndiciXTipologieUC("Obbligatorio", "Obbligatorio"), filterable: { multi: true, search: true }
        }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = { salvaRipristinaPersonalizzazioni: { url: pathCoreWS } };
    //var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBinding: onDataBindingVFattVarParamQual };
    var funzioniPrimaDopoEventi = {};
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

//Quando clicco sul bottone mostro la grid
$("#btn_mostra_grid_tipologiexindice").click(function () {
    //Richiamo la funzione per la creazione della Griglia per visualizzare tutte le tipologie associate a quell'indice
    $("#Switch_Campo_Obbligatorio").hide();
    popolaGridTipologiexIndice("grid_tipologiexindice");
})