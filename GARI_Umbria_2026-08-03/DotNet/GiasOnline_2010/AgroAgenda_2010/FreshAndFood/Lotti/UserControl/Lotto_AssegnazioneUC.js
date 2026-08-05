
function popolaLotto_Assegna(IDControllo) {

    var Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: RicercaLottoAssegna,
        funzioneSubmit: { funzione: SubmitLottoAssegna, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "key_lotto_assegna";

    var Obbligatorio = true;

    if (Tipo_Lotto === "L")
        Obbligatorio = false;

    var campiKendoModel = {
        key_lotto_assegna: { editable: false, type: "string" },
        Cod_RisUm: { editable: true, type: "number", validation: { required: Obbligatorio } },
        Rag_Soc: { editable: true, type: "string", validation: { required: Obbligatorio } },
        Veg_Cod: { editable: true, type: "string", validation: { required: Obbligatorio } },
        Veg_Des: { editable: true, type: "string", validation: { required: Obbligatorio } },
        Cul_Cod: { editable: true, type: "number", validation: { required: Obbligatorio } },
        Cul_Des: { editable: true, type: "string", validation: { required: Obbligatorio } },
        qualita_cod: { editable: true, type: "number", validation: { required: false } },
        qualita_des: { editable: true, type: "string", validation: { required: false } },
        certif_cod: { editable: true, type: "number", validation: { required: false } },
        certif_des: { editable: true, type: "string", validation: { required: false } },
        lotto: { editable: true, type: "string", validation: { required: Obbligatorio } },
        validita_inizio: { editable: true, type: "date", validation: { required: Obbligatorio } },
        validita_fine: { editable: true, type: "date", validation: { required: Obbligatorio } },
        Preparazione_Cod: { editable: true, type: "number", validation: { required: false }, defaultValue: 0 },
        Preparazione_Des: { editable: true, type: "string", validation: { required: false }, defaultValue: "" },
        Reg_Cod: { editable: true, type: "number", validation: { required: false }, defaultValue: 0 },
        Reg_Des: { editable: true, type: "string", validation: { required: false }, defaultValue: "" },
        Tipo_Lotto: { editable: false, type: "string", defaultValue: Tipo_Lotto },
        Algoritmo_Config: { editable: true,  validation: { required: false }, defaultValue: "0" },
        Algoritmo_Config_Des: { editable: true, validation: { required: false }, defaultValue: "" },
        Algoritmo_Config_String: { editable: true, type: "string", validation: { required: false }, defaultValue: "" },
        Algoritmo_Config_Des_String: { editable: true, type: "string", validation: { required: false }, defaultValue: "" },
        Separatore_Config: { editable: true, type: "number", validation: { required: false }, defaultValue: 0 },
        Separatore_Config_Des: { editable: true, type: "string", validation: { required: false }, defaultValue: "" }
    };

    var colonneKendoGrid = [];

    if (Tipo_Lotto === "E") {

        colonneKendoGrid.push(
            {
                field: "Rag_Soc", title: "Fornitore", editor: codrisum_DropDownEditor, daDuplicare: true
            },
            {
                field: "Veg_Des", title: "Specie", defaultValue: "-1", editor: specie_DropDownEditor, daDuplicare: true
            },
            {
                field: "Cul_Des", title: "Varietà", editor: varieta_DropDownEditor, daDuplicare: true
            },
            {
                field: "qualita_des", title: "Qualità", editor: qualita_DropDownEditor, daDuplicare: true
            },
            {
                field: "certif_des", title: "Certificazione", editor: certif_DropDownEditor, daDuplicare: true
            },
            { field: "lotto", title: "Lotto da assegnare", daDuplicare: true },
            { field: "validita_inizio", title: "Data inizio", format: "{0:dd/MM/yyyy}", daDuplicare: true },
            { field: "validita_fine", title: "Data fine", format: "{0:dd/MM/yyyy}", daDuplicare: true }
        );

    } else if (Tipo_Lotto === "L") {

        colonneKendoGrid.push(
            {
                field: "Preparazione_Des", title: "Tipologia Lavorazione", editor: lavorazione_DropDownEditor, daDuplicare: true, filterable: { multi: true, search: true }
            },
            {
                field: "Veg_Des", title: "Specie", editor: specie_DropDownEditor, daDuplicare: true, filterable: { multi: true, search: true }
            },
            {
                field: "Cul_Des", title: "Varietà", editor: varieta_DropDownEditor, daDuplicare: true, filterable: { multi: true, search: true }
            },
            {
                field: "Reg_Des", title: "Regolamento", editor: regolamento_DropDownEditor, daDuplicare: true, filterable: { multi: true, search: true }
            },
            {
                field: "Algoritmo_Config_Des_String", title: "Parametri Scelti", editor: algoritmo_config_MultiSelectEditor, daDuplicare: true, filterable: { multi: true, search: true }
            },
            {
                field: "Separatore_Config_Des", title: "Separatore", editor: separatore_DropDownEditor, daDuplicare: true, filterable: { multi: true, search: true }
            }
        );

        //Imposto i Default del model quando la pagina è aperta dalla "Configurazioni Lavorazioni"
        campiKendoModel.Cod_RisUm.defaultValue = 0;

        campiKendoModel.Rag_Soc = 0;

        campiKendoModel.Veg_Cod.defaultValue = "-1";

        campiKendoModel.Veg_Des.defaultValue = "";

        campiKendoModel.Cul_Cod.defaultValue = 0;

        campiKendoModel.Cul_Des.defaultValue = "";

        campiKendoModel.qualita_cod.defaultValue = 0;

        campiKendoModel.qualita_des.defaultValue = "";

        campiKendoModel.certif_cod.defaultValue = 0;

        campiKendoModel.certif_des.defaultValue = "";

        campiKendoModel.lotto.defaultValue = "";

        campiKendoModel.validita_inizio.defaultValue = new Date("1900/01/01");

        campiKendoModel.validita_fine.defaultValue = new Date("2100/12/31");

    }


    var parametriPerLettura = null;
    var parametriDataSource = {};

    var colCustKendoGrid = [
        {
            command: [
                {
                    iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: "", update: "Conf.", cancel: "Ann." }
                },
                {
                    iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "destroy", text: ""
                }
            ],
            title: "Operazioni"//, locked: true
        }
    ];

    // Se l'utente non è abilitato in modifica non mostro il pulsante di duplicazione
    // Le colonne modifica / cancellazione e inserimento nuova riga sono già gestite nel GiasBase
    if (UteAbilitatoInsMod) {
        colCustKendoGrid[0].command.push(
            {
                iconClass: "fa fa-files-o fa-xs", className: "blockDuplica", name: "duplica", text: "", click: duplicaRigaKendoGrid
            });
    }

    var parametriKendoGrid = {
        editable: {
            mode: "inline"
        },
        colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
        //,
        //filterable: {
        //    mode: "row"
        //},
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditLottoAssegna };

    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];
    //var colonneDisabilitateSoloInModifica = ["Cod_RisUm", "Rag_Soc", "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des", "qualita_cod", "qualita_des", "certif_cod", "certif_des", "validita_inizio", "validita_fine"];
    //var colonneDisabilitateSoloInModifica = ["Cod_RisUm", "Rag_Soc", "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des", "qualita_cod", "qualita_des", "certif_cod", "certif_des"];

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

function codrisum_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Rag_Soc", "Cod_RisUm", elencoFornitori, changeFornitore);
}

function changeFornitore(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cod_RisUm = dataItem.Cod_RisUm;
    model.Rag_Soc = dataItem.Rag_Soc;

}

function specie_DropDownEditor(container, options) {

    let Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    creaDropDownEditor(container, "Veg_Des", "Veg_Cod", elencoSpecie, changeSpecie);

    if (Tipo_Lotto === "L")
        $('input[name$="Veg_Cod"]').data("kendoDropDownList").enable(false);

}

function changeSpecie(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Veg_Cod = dataItem.Veg_Cod;
    model.Veg_Des = dataItem.Veg_Des;

    model.Cul_Cod = 0;
    model.Cul_Des = "";

    var varieta = RicercaVarieta(cIdPiva, model.Veg_Cod);

    var varietaDDL = $('input[name$="Cul_Cod"]').data("kendoDropDownList");
    varietaDDL.setDataSource(varieta);


}

function varieta_DropDownEditor(container, options) {

    let Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    var Veg_Cod = options.model.Veg_Cod;
    if (rigaDaCopiareKendoGrid !== undefined && rigaDaCopiareKendoGrid !== null)
        Veg_Cod = rigaDaCopiareKendoGrid.Veg_Cod;

    if (Veg_Cod !== undefined) {
        if (Veg_Cod === "")
            Veg_Cod = "-1";
        var varieta = RicercaVarieta(cIdPiva, Veg_Cod);
        creaDropDownEditor(container, "Cul_Des", "Cul_Cod", varieta, changeVarieta);
    }

    if (Tipo_Lotto === "L")
        $('input[name$="Cul_Cod"]').data("kendoDropDownList").enable(false);
}

function changeVarieta(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Cul_Cod = dataItem.Cul_Cod;
    model.Cul_Des = dataItem.Cul_Des;

}


function qualita_DropDownEditor(container, options) {

    creaDropDownEditor(container, "qualita_des", "qualita_cod", elencoQualitaAssegnaLotto, changeQualita);
}

function changeQualita(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.qualita_cod = dataItem.qualita_cod;
    model.qualita_des = dataItem.qualita_des;

}

function certif_DropDownEditor(container, options) {

    creaDropDownEditor(container, "certif_des", "certif_cod", elencoCertificAssegnaLotto, changeCertif);
}

function changeCertif(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.certif_cod = dataItem.certif_cod;
    model.certif_des = dataItem.certif_des;

}

function lavorazione_DropDownEditor(container, options) {

    let Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    creaDropDownEditor(container, "Preparazione_Des", "Preparazione_Cod", elencoTipologieLavorazioniAssegnaLotto, changeLavorazione);

    if (Tipo_Lotto === "L")
        $('input[name$="Preparazione_Cod"]').data("kendoDropDownList").enable(false);
}

function changeLavorazione(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Preparazione_Cod = dataItem.Preparazione_Cod;
    model.Preparazione_Des = dataItem.Preparazione_Des;

}

function regolamento_DropDownEditor(container, options) {

    let Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    creaDropDownEditor(container, "Reg_Des", "Reg_Cod" , elencoRegolamentiAssegnaLotto, changeRegolamento);

    if (Tipo_Lotto === "L")
        $('input[name$="Reg_Cod"]').data("kendoDropDownList").enable(false);
}

function changeRegolamento(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Reg_Cod = dataItem.Reg_Cod;
    model.Reg_Des = dataItem.Reg_Des;

}

function algoritmo_config_MultiSelectEditor(container) {

    let Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    $('<input name="Algoritmo_Config"/>')
        .appendTo(container)
        .kendoMultiSelect({
            autoBind: true,
            dataTextField: "Algoritmo_Config_Des",
            dataValueField: "Algoritmo_Config",
            dataSource: elencoParametriAssegnaLotto,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: changeAlgoritmo_Config
        }).data("kendoMultiSelect");

    var multi = $('input[name$="Algoritmo_Config"]').data("kendoMultiSelect");
    multi.list.width("auto");

    if (Tipo_Lotto === "L")
        multi.enable(false);
}

function changeAlgoritmo_Config(e) {
    var Algoritmo_Config_Scelti = $('input[name$="Algoritmo_Config"]').data("kendoMultiSelect").dataItems();

    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    model.Algoritmo_Config = [];
    model.Algoritmo_Config_Des = [];
    model.Algoritmo_Config_String = "";
    model.Algoritmo_Config_Des_String = "";

    if (Algoritmo_Config_Scelti.length > 0) {

        for (var x = 0; x < Algoritmo_Config_Scelti.length; x++) {
            model.Algoritmo_Config.push(Algoritmo_Config_Scelti[x].Algoritmo_Config);
            model.Algoritmo_Config_Des.push(Algoritmo_Config_Scelti[x].Algoritmo_Config_Des);
        }

        if (model.Algoritmo_Config.length>0)
            model.Algoritmo_Config_String = model.Algoritmo_Config.join("|");

        if (model.Algoritmo_Config_Des.length > 0)
            model.Algoritmo_Config_Des_String = model.Algoritmo_Config_Des.join(",");

    }
    else {
        model.Algoritmo_Config = [elencoParametriAssegnaLotto[0].Algoritmo_Config.toString()];
        model.Algoritmo_Config_Des = [elencoParametriAssegnaLotto[0].Algoritmo_Config_Des.toString()];
        model.Algoritmo_Config_String = elencoParametriAssegnaLotto[0].Algoritmo_Config.toString();
        model.Algoritmo_Config_Des_String = elencoParametriAssegnaLotto[0].Algoritmo_Config_Des.toString();
    }
}

function separatore_DropDownEditor(container, options) {

    let Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    creaDropDownEditor(container, "Separatore_Config_Des", "Separatore_Config" , elencoSeparatoriAssegnaLotto, changeSeparatore);

    if (Tipo_Lotto === "L")
        $('input[name$="Separatore_Config"]').data("kendoDropDownList").enable(false);

}

function changeSeparatore(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Separatore_Config = dataItem.Separatore_Config;
    model.Separatore_Config_Des = dataItem.Separatore_Config_Des;

}

function duplicaRigaKendoGrid(e) {

    var grid = $("#tab_lotto_assegna").data("kendoGrid");
    var row = $(e.target).closest("tr");

    var hasChanges = grid.dataSource.hasChanges();

    if (!hasChanges) {

        e.preventDefault();

        rigaDaCopiareKendoGrid = grid.dataItem(row);
        rigaDuplicataKendoGrid = true;
        grid.addRow();

    }
    else {
        alert("Sono presenti righe non salvate: procedere prima con il salvataggio");
    }
}

function onEditLottoAssegna(e) {

    var Tipo_Lotto = $(hfTipo_Lotto).val().toUpperCase();

    if (Tipo_Lotto === "E") {
        // Gestione della duplicazione di una riga
        //      Vengono copiati solo i valori delle colonne marcate come "Da duplicare""
        //      Questa funzione viaggia in coppia con la funzione Duplica
        if (rigaDuplicataKendoGrid && rigaDaCopiareKendoGrid != null && e.model.isNew() && !e.model.dirty) {

            var gridId = e.sender.element[0].id;
            var grid = $("#" + gridId).data("kendoGrid");

            rigaDaCopiareKendoGrid.forEach(function (valore, campo) {
                //console.log('[' + campo + '] ' + valore);
                var duplicazioneEffettuata = false;
                for (let r = 0; r < grid.columns.length; r++) {
                    let col = grid.columns[r];
                    if (col.daDuplicare && col.field !== undefined && col.field === campo) {
                        if (e.model.get(campo) !== valore) {
                            if (campo === "validita_inizio" || campo === "validita_fine")
                                e.container.find("input[name=" + campo + "]").val(formattedDate(valore, "/")).change();
                            else
                                e.container.find("input[name=" + campo + "]").val(valore).change();
                            e.model.set(campo, valore);
                        }
                        duplicazioneEffettuata = true;
                    }
                }

                if (!duplicazioneEffettuata) {
                    // Se non ho trovato il campo fra le colonne della griglia cerco se è una DropDownList (i campi 
                    // codice in questo caso non fanno parte delle colonne quindi non li trova)
                    if (e.container.find("input[name=" + campo + "]").data("kendoDropDownList") !== undefined) {
                        var keyCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.dataValueField;
                        var textCampo = null;

                        // nomeCampoEffettivo è un campo valorizzato solo per i parametri qualitativi
                        if (e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.nomeCampoEffettivo !== undefined)
                            textCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.nomeCampoEffettivo;
                        else
                            textCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.dataTextField;

                        if (textCampo != null) {
                            for (let r = 0; r < grid.columns.length; r++) {
                                let col = grid.columns[r];
                                if (col.daDuplicare && col.field !== undefined && col.field === textCampo) {
                                    if (e.model.get(campo) !== valore) {
                                        e.container.find("input[name=" + keyCampo + "]").val(valore).change();
                                        e.model.set(campo, valore);
                                    }
                                    duplicazioneEffettuata = true;
                                }
                            }
                        }
                    }
                }

                if (!duplicazioneEffettuata) {
                    //Gestione dei campi che non voglio duplicare per evitare che vengano impostati con i default
                    for (var f in grid.dataSource.options.schema.model.fields) {
                        if (grid.dataSource.options.schema.model.fields.hasOwnProperty(f) &&
                            f === campo &&
                            grid.dataSource.options.schema.model.fields[f].defaultValue !== undefined) {
                            //console.log(f + " -> " + grid.dataSource.options.schema.model.fields[f]);
                            if (grid.dataSource.options.schema.model.fields[f].type == "string") {
                                e.container.find("input[name=" + f + "]").val("").change();
                                e.model.set(f, "");
                            }
                            if (grid.dataSource.options.schema.model.fields[f].type == "date") {
                                e.container.find("input[name=" + f + "]").val("").change();
                                e.model.set(f, "");
                            }
                            if (grid.dataSource.options.schema.model.fields[f].type == "number") {
                                e.container.find("input[name=" + f + "]").val(0).change();
                                e.model.set(f, 0);
                            }
                            if (grid.dataSource.options.schema.model.fields[f].type == "boolean") {
                                e.container.find("input[name=" + f + "]").val(false).change();
                                e.model.set(f, false);
                            }
                        }
                    }
                }
            });

            rigaDuplicataKendoGrid = false;
            rigaDaCopiareKendoGrid = null;

        }
    }
    else if (Tipo_Lotto === "L") {

        //Apro la kendo window verso lo UC Lavorazione_Assegna_Lotti
        let dati_riga = "";
        let titolo_window = "";

        if (!e.model.isNew()) {

            dati_riga = e.model.Preparazione_Cod + "_" + e.model.Veg_Cod + "_" + e.model.Cul_Cod + "_" + e.model.Reg_Cod + "_" + e.model.Algoritmo_Config.join("|") + "_" + e.model.Separatore_Config;

            titolo_window = "Configurazione Lavorazioni - Modifica Riga : " + e.model.Preparazione_Des + " " + e.model.Veg_Des + " " + e.model.Cul_Des + " " + e.model.Reg_Des;
        }
        else {

            if (rigaDuplicataKendoGrid && rigaDaCopiareKendoGrid != null && e.model.isNew() && !e.model.dirty) {

                dati_riga = rigaDaCopiareKendoGrid.Preparazione_Cod + "_" + rigaDaCopiareKendoGrid.Veg_Cod + "_" + rigaDaCopiareKendoGrid.Cul_Cod + "_" + rigaDaCopiareKendoGrid.Reg_Cod + "_" + rigaDaCopiareKendoGrid.Algoritmo_Config.join("|") + "_" + rigaDaCopiareKendoGrid.Separatore_Config;

                titolo_window = "Configurazione Lavorazioni - Copia Riga : " + rigaDaCopiareKendoGrid.Preparazione_Des + " " + rigaDaCopiareKendoGrid.Veg_Des + " " + rigaDaCopiareKendoGrid.Cul_Des + " " + rigaDaCopiareKendoGrid.Reg_Des;

                rigaDuplicataKendoGrid = false;
                rigaDaCopiareKendoGrid = null;

            } else {

                titolo_window = "Configurazione Lavorazioni - Nuova Riga ";

            }

        }

        //Apro la pagina di Configurazione Lotto per le Lavorazioni in una kendowindow
        var url = "./Lavorazione_Assegna_Lotti.aspx?p=" + cIdPiva + "&d=" + dati_riga;
        apriKendoWindowLavorazione_Assegna_Lotti(url, titolo_window, e);

    }

}

function apriKendoWindowLavorazione_Assegna_Lotti(url, title,riga) {
    $(document.body).append('<div id="parametriLavorazioneWindow"></div>');
    $('#parametriLavorazioneWindow').kendoWindow({
        title: title,
        modal: true,
        resizable: false,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Close"],
        close: function () {

            $("#parametriLavorazioneWindow").data("kendoWindow").destroy();

            if (rigaDaSalvareDaKendoWindowAssegnaLotto !== undefined &&
                rigaDaSalvareDaKendoWindowAssegnaLotto !== null &&
                rigaDaSalvareDaKendoWindowAssegnaLotto !== "") {

                if (rigaDaSalvareDaKendoWindowAssegnaLotto.Preparazione_Cod !== undefined &&
                    rigaDaSalvareDaKendoWindowAssegnaLotto.Preparazione_Cod !== null) {

                    riga.model.set("Preparazione_Cod", rigaDaSalvareDaKendoWindowAssegnaLotto.Preparazione_Cod);

                    $("input[name='Preparazione_Cod']").data("kendoDropDownList").value(rigaDaSalvareDaKendoWindowAssegnaLotto.Preparazione_Cod);

                    $("input[name='Preparazione_Cod']").data("kendoDropDownList").trigger("change");
                }

                if (rigaDaSalvareDaKendoWindowAssegnaLotto.Veg_Cod !== undefined &&
                    rigaDaSalvareDaKendoWindowAssegnaLotto.Veg_Cod !== null) {

                    riga.model.set("Veg_Cod", rigaDaSalvareDaKendoWindowAssegnaLotto.Veg_Cod);

                    $("input[name='Veg_Cod']").data("kendoDropDownList").value(rigaDaSalvareDaKendoWindowAssegnaLotto.Veg_Cod);

                    $("input[name='Veg_Cod']").data("kendoDropDownList").trigger("change");
                }


                if (rigaDaSalvareDaKendoWindowAssegnaLotto.Cul_Cod !== undefined &&
                    rigaDaSalvareDaKendoWindowAssegnaLotto.Cul_Cod !== null) {

                    riga.model.set("Cul_Cod", rigaDaSalvareDaKendoWindowAssegnaLotto.Cul_Cod);

                    $("input[name='Cul_Cod']").data("kendoDropDownList").value(rigaDaSalvareDaKendoWindowAssegnaLotto.Cul_Cod);

                    $("input[name='Cul_Cod']").data("kendoDropDownList").trigger("change");
                }


                if (rigaDaSalvareDaKendoWindowAssegnaLotto.Reg_Cod !== undefined &&
                    rigaDaSalvareDaKendoWindowAssegnaLotto.Reg_Cod !== null) {

                    riga.model.set("Reg_Cod", rigaDaSalvareDaKendoWindowAssegnaLotto.Reg_Cod);

                    $("input[name='Reg_Cod']").data("kendoDropDownList").value(rigaDaSalvareDaKendoWindowAssegnaLotto.Reg_Cod);

                    $("input[name='Reg_Cod']").data("kendoDropDownList").trigger("change");
                }

                if (rigaDaSalvareDaKendoWindowAssegnaLotto.Algoritmo_Config !== undefined &&
                    rigaDaSalvareDaKendoWindowAssegnaLotto.Algoritmo_Config !== null) {

                    riga.model.set("Algoritmo_Config", rigaDaSalvareDaKendoWindowAssegnaLotto.Algoritmo_Config);

                    $("input[name='Algoritmo_Config']").data("kendoMultiSelect").value(rigaDaSalvareDaKendoWindowAssegnaLotto.Algoritmo_Config);

                    $("input[name='Algoritmo_Config']").data("kendoMultiSelect").trigger("change");
                }

                if (rigaDaSalvareDaKendoWindowAssegnaLotto.Separatore_Config !== undefined &&
                    rigaDaSalvareDaKendoWindowAssegnaLotto.Algoritmo_Config !== null) {

                    riga.model.set("Separatore_Config", rigaDaSalvareDaKendoWindowAssegnaLotto.Separatore_Config);

                    $("input[name='Separatore_Config']").data("kendoDropDownList").value(rigaDaSalvareDaKendoWindowAssegnaLotto.Separatore_Config);

                    $("input[name='Separatore_Config']").data("kendoDropDownList").trigger("change");
                }

                rigaDaSalvareDaKendoWindowAssegnaLotto = null;

                $("#tab_lotto_assegna").data("kendoGrid").saveChanges();
            }
            else {
                //Elimino e ricreo la Grid

                let grid_tab_lotto_assegna = $("#tab_lotto_assegna").data("kendoGrid");

                if (grid_tab_lotto_assegna !== undefined && grid_tab_lotto_assegna !== null && grid_tab_lotto_assegna !== "") 
                    grid_tab_lotto_assegna.destroy();

                popolaLotto_Assegna("tab_lotto_assegna");
            }

        }
    }).data('kendoWindow').center().maximize();
}

function Salva_Da_KendoWindow_Lotto_AssegnazioneUC(salvaModel) {

    if (salvaModel === undefined || salvaModel === null || salvaModel === "")
        return;

    rigaDaSalvareDaKendoWindowAssegnaLotto = salvaModel;

    $("#parametriLavorazioneWindow").data("kendoWindow").close();

}
