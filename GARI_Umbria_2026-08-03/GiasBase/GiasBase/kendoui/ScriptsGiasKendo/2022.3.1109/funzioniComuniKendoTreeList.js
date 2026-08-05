
/**
* Creazione di una TreeList Kendo
*
* @param {string} IDControllo Rappresenta il selector JQuery del div a cui si associa la griglia
* @param {object} funzioniCRUD Funzioni js da chiamare per read, insert, update, delete { chiave - valore}: { funzioneRead: xxxx, funzioneInsert: yyyy, funzioneUpdate: zzzz, funzioneDelete: kkkk }
* @param {string} IDModel chiave di riga
* @param {string} idModelParent riga padre
* @param {object} campiKendoModel definizione del modello kendo
* @param {object} colonneKendoTreeList definizione delle colonne
*/
function creaKendoTreeList(
    // PARAMETRI OBBLIGATORI
    IDControllo,
    funzioniCRUD,
    idModel,
    idModelParent,
    campiKendoModel,
    colonneKendoTreeList,
    // PARAMETRI FACOLTATIVO
    parametriPerLettura, // parametri da passare alla lettura
    parametriDataSource, // parametri data source { chiave - valore}
    parametriKendoTreeList,   // parametri griglia [{ chiave - valore}]
    funzioniPrimaDopoEventi // funzioni da chiamare all'inizio e alla fine dei vari eventi { chiave - valore}:
                            //{   
                            //    funzioneDaChiamarePrimaDelDataBinding: yyyyy, // funzione da chiamare all'inizio del databinding
                            //    funzioneDaChiamareDopoDataBinding: yyyyy, // funzione da chiamare alla fine del databinding
                            //    funzioneDaChiamarePrimaDelDataBound: yyyyy, // funzione da chiamare all'inizio del databound
                            //    funzioneDaChiamareDopoDataBound: yyyyy, // funzione da chiamare alla fine del databound
                            //    funzioneDaChiamarePrimaDelSave: yyyyy, // funzione da chiamare all'inizio del save
                            //    funzioneDaChiamareDopoSave: yyyyy // funzione da chiamare alla fine del save
                            //}
    ) {

    if (IDControllo == null)
    {
        alert("Non mi hai passato l'ID del DIV che contiene il TreeList");
        return;
    }
        
    if (funzioniCRUD == null || funzioniCRUD.funzioneRead == null)
    {
        alert("Non mi hai passato la funzione da chiamare in lettura");
        return;
    }
     
    if (funzioniCRUD != null)
    {
        errFound = false;
        for (var k in funzioniCRUD) {
            if (k != "funzioneRead" &&
                k != "funzioneInsert" &&
                k != "funzioneUpdate" &&
                k != "funzioneDelete" &&
                k != "checkBoxFunction" &&
                k != "UtenteAbilitatoInserimentoModifica" &&
                k != "UtenteAbilitatoCancellazione")
            {
                errFound = true;
                alert("Fra le funzioni CRUD mi hai passato la chiave " + k + " che non è gestita");
            }
        }
        if (errFound)
            return;

        // Se il parametro non viene passato si assume che l'utente abbia i permessi
        funzioniCRUD.UtenteAbilitatoInserimentoModifica = (typeof funzioniCRUD.UtenteAbilitatoInserimentoModifica === 'undefined') ? true : funzioniCRUD.UtenteAbilitatoInserimentoModifica;
        funzioniCRUD.UtenteAbilitatoCancellazione = (typeof funzioniCRUD.UtenteAbilitatoCancellazione === 'undefined') ? true : funzioniCRUD.UtenteAbilitatoCancellazione;

    }
    
 
    if (idModel == null) {
        alert("Non mi hai passato la chiave della riga del TreeList");
        return;
    }

    if (idModelParent == null) {
        alert("Non mi hai passato il parent della riga del TreeList");
        return;
    }

    if (campiKendoModel == null) {
        alert("Non mi hai passato i campi del modello del TreeList");
        return;
    }

    if (colonneKendoTreeList == null) {
        alert("Non mi hai passato le colonne del TreeList");
        return;
    }

    funzioniPrimaDopoEventi = (typeof funzioniPrimaDopoEventi === 'undefined') ? {} : funzioniPrimaDopoEventi;
    errFound = false;
    for (var k in funzioniPrimaDopoEventi) {
        if (k != "funzioneDaChiamarePrimaDelDataBinding" &&
            k != "funzioneDaChiamareDopoDataBinding" &&
            k != "funzioneDaChiamarePrimaDelDataBound" &&
            k != "funzioneDaChiamareDopoDataBound" &&
            k != "funzioneDaChiamarePrimaDelSave" &&
            k != "funzioneDaChiamarePrimaDiSelectAllRows" &&
            k != "funzioneDaChiamareDopoSelectAllRows" &&
            k != "funzioneDaChiamareDopoEdit" &&
            k != "funzioneDaChiamareDopoSave") {
            errFound = true;
            alert("Fra le funzioni da chiamare prima o dopo agli eventi mi hai passato la chiave " + k + " che non è gestita");
        }
    }
    if (errFound)
        return;

    // Per le funzioni di accesso al database è necessario fare delle funzioni anonime 
    // altrimenti le chiama subito nel momento in cui crea il TreeList
    var readFunction = null;
    if (funzioniCRUD.funzioneRead != null) {
        if (parametriPerLettura != null && parametriPerLettura.length > 0)
        {
            readFunction = function (options) {
                funzioniCRUD.funzioneRead(options, parametriPerLettura);

                //var risp = funzioniCRUD.funzioneRead(parametriPerLettura.toString());
                //options.success(risp);
            };
        }  
        else
        {
            readFunction = function (options) {
                funzioniCRUD.funzioneRead(options);
                 
            };
        }
    }

    if (funzioniCRUD.UtenteAbilitatoInserimentoModifica) {
        var insertFunction = null;
        if (funzioniCRUD.funzioneInsert != null) {
            insertFunction = (function (options) {
                funzioniCRUD.funzioneInsert(options);
            });
        }
   
        var updateFunction = null;
        if (funzioniCRUD.funzioneUpdate != null) {
            updateFunction = (function (options) {
                funzioniCRUD.funzioneUpdate(options);
            });
        }
    }
    
    //qui:
    if (funzioniCRUD.checkBoxFunction != null) {

        $.extend(campiKendoModel, {
            Selected: { type: "boolean", editable: false }
        });
    }

    parametriDataSource = (typeof parametriDataSource === 'undefined' || parametriDataSource == null) ? {} : parametriDataSource;
    parametriKendoTreeList = (typeof parametriKendoTreeList === 'undefined' || parametriKendoTreeList == null) ? {} : parametriKendoTreeList;
    parametriKendoTreeList.scrollable = (typeof parametriKendoTreeList.scrollable === 'undefined') ? true : parametriKendoTreeList.scrollable;
    parametriKendoTreeList.sortable = (typeof parametriKendoTreeList.sortable === 'undefined') ? true : parametriKendoTreeList.sortable;
    parametriKendoTreeList.filterable = (typeof parametriKendoTreeList.filterable === 'undefined') ? true : parametriKendoTreeList.filterable;
    parametriKendoTreeList.excel = (typeof parametriKendoTreeList.excel === 'undefined') ? true : parametriKendoTreeList.excel;
    parametriKendoTreeList.pdf = (typeof parametriKendoTreeList.pdf === 'undefined') ? true : parametriKendoTreeList.pdf;
    parametriKendoTreeList.columnMenu = (typeof parametriKendoTreeList.columnMenu === 'undefined') ? true : parametriKendoTreeList.columnMenu;
    parametriKendoTreeList.toolbarCommands = (typeof parametriKendoTreeList.toolbarCommands === 'undefined') ? null : parametriKendoTreeList.toolbarCommands;
    parametriKendoTreeList.height = (typeof parametriKendoTreeList.height === 'undefined') ? null : parametriKendoTreeList.height;
    parametriKendoTreeList.colonneCustomKendoTreeList = (typeof parametriKendoTreeList.colonneCustomKendoTreeList === 'undefined') ? null : parametriKendoTreeList.colonneCustomKendoTreeList;
    parametriKendoTreeList.checkSelezioneRiga = (typeof parametriKendoTreeList.checkSelezioneRiga === 'undefined') ? null : parametriKendoTreeList.checkSelezioneRiga;
    parametriKendoTreeList.salvaRipristinaPersonalizzazioni = (typeof parametriKendoTreeList.salvaRipristinaPersonalizzazioni === 'undefined') ? false : parametriKendoTreeList.salvaRipristinaPersonalizzazioni;
    parametriKendoTreeList.resizable = (typeof parametriKendoTreeList.resizable === 'undefined') ? true : parametriKendoTreeList.resizable;
    parametriKendoTreeList.reorderable = (typeof parametriKendoTreeList.reorderable === 'undefined') ? true : parametriKendoTreeList.reorderable;
    parametriDataSource.aggregate = (typeof parametriDataSource.aggregate === 'undefined') ? null : parametriDataSource.aggregate;

    // Aggiunta colonne Custom
    if (parametriKendoTreeList.colonneCustomKendoTreeList != null) {
        //vado all'indietro perchè faccio l'unshift
        for (i = parametriKendoTreeList.colonneCustomKendoTreeList.length -1; i >= 0; i--)
            colonneKendoTreeList.unshift(parametriKendoTreeList.colonneCustomKendoTreeList[i]);
    }

    var checkBoxFunction = null;
    if (funzioniCRUD.checkBoxFunction != null) {

        var width = 50;
        var sortable = false;
        var filterable = true;
        var title = 'Seleziona';
        var locked = false;
        var field = "Selected";

        if (parametriKendoTreeList.checkSelezioneRiga){
            width=(typeof parametriKendoTreeList.checkSelezioneRiga.width === 'undefined') ? width : parametriKendoTreeList.checkSelezioneRiga.width;
            sortable=(typeof parametriKendoTreeList.checkSelezioneRiga.sortable === 'undefined') ? sortable : parametriKendoTreeList.checkSelezioneRiga.sortable;
            filterable=(typeof parametriKendoTreeList.checkSelezioneRiga.filterable === 'undefined') ? filterable : parametriKendoTreeList.checkSelezioneRiga.filterable;
            title=(typeof parametriKendoTreeList.checkSelezioneRiga.title === 'undefined') ? title : parametriKendoTreeList.checkSelezioneRiga.title;
            locked=(typeof parametriKendoTreeList.checkSelezioneRiga.locked === 'undefined') ? locked : parametriKendoTreeList.checkSelezioneRiga.locked;
            field=(typeof parametriKendoTreeList.checkSelezioneRiga.field === 'undefined') ? field : parametriKendoTreeList.checkSelezioneRiga.field;
        }

        colonneKendoTreeList.unshift({
            title: title,
            field: field,
            sortable: sortable,
            width: width,
            filterable: filterable,
            locked: locked,
            headerTemplate: '<input type="checkbox" id="' + IDControllo + '-header-chb" class="k-checkbox header-chb"><label style="margin-right:0px;" class="k-checkbox-label" for="' + IDControllo + '-header-chb"></label>',
            template: function (dataItem) {
                //console.log(idModel);
                //console.log(dataItem.kendoKey);
                // NON FUNZIONA IN IE: return `<input type="checkbox"   id="${dataItem[idModel]}" #= Selected ? \'checked="checked"\'   : "" class="k-checkbox"><label class="k-checkbox-label" for="${dataItem[idModel]}"></label>`
                return "<input type=\"checkbox\" id=\"" + dataItem.id + "\" #= Selected ? \'checked=\"checked\"\' : \"\" class=\"k-checkbox\"><label style='margin-right:0px;' class=\"k-checkbox-label\" for=\"" + dataItem.id + "\"></label>"

            }
        });
    }

    if (funzioniCRUD.UtenteAbilitatoCancellazione) {
        var deleteFunction = null;
        if (funzioniCRUD.funzioneDelete != null) {
            deleteFunction = (function (options) {
                funzioniCRUD.funzioneDelete(options);
            });
        }
    }

    // Sembra non più necessario dalla versione 2017
    if (parametriKendoTreeList.filterable)
    {
        if (parametriKendoTreeList.filterable == true)
        {
            parametriKendoTreeList.filterable = {
                messages: {
                    clear: "Pulisci",
                    filter: "Applica",
                    isFalse: "No",
                    isTrue: "Sì",
                    checkAll: "Seleziona tutto",
                    selectedItemsFormat: "{0} elementi selezionati"
                }
            }
        }
        else
        {
            parametriKendoTreeList.filterable['messages'] =  
             {
                clear: "Pulisci",
                filter: "Applica",
                isFalse: "No",
                isTrue: "Sì",
                checkAll: "Seleziona tutto",
                selectedItemsFormat: "{0} elementi selezionati"
             }
        }
    }
 
    if (parametriKendoTreeList.columnMenu)
    {
        parametriKendoTreeList.columnMenu = {
            filterable: true,
            sortable: false,
            columns: true
        }
    }

    if (parametriKendoTreeList.sortable)
    {
        parametriKendoTreeList.sortable = {
            mode: "multiple"
        }
    }

    // toolbar
    var toolbar = [];

    //Se impostato aggiungo il pulsante per salvare le personalizzazioni della griglia
    if (parametriKendoTreeList.salvaRipristinaPersonalizzazioni != false)
        toolbar.push({ name: 'salvaPersGrigliaKendo', template: kendo.template('<div class="k-button" onclick="salvaPersonalizzazioniKendoTreeList(&quot;' + parametriKendoTreeList.salvaRipristinaPersonalizzazioni.url + '&quot;, &quot;' + location.pathname + '&quot;, &quot;' + IDControllo + '&quot;)">Salva Personalizzazioni Griglia</div>') });

    // pulsanti impostati solo se vengono passate le relative funzioni
    if (((funzioniCRUD.funzioneInsert != null || funzioniCRUD.funzioneUpdate != null) && funzioniCRUD.UtenteAbilitatoInserimentoModifica) ||
        (funzioniCRUD.funzioneDelete != null && funzioniCRUD.UtenteAbilitatoCancellazione))
    {
        toolbar.push("save");
        toolbar.push("cancel");
    }
    if (funzioniCRUD.funzioneInsert != null && funzioniCRUD.UtenteAbilitatoInserimentoModifica)
    {
        toolbar.push("create");
    }

    if (parametriKendoTreeList.excel)
        toolbar.push("excel");

    if (parametriKendoTreeList.pdf)
        toolbar.push("pdf");

    if (parametriKendoTreeList.toolbarCommands != null)
    {
        for (i = 0; i<parametriKendoTreeList.toolbarCommands.length; i++)
        {
            var myID = "#" + parametriKendoTreeList.toolbarCommands[i];
            toolbar.push({ template: kendo.template($(myID).html()) });
        }
    }     
    
    var dataSourceKendoTreeList = new kendo.data.TreeListDataSource({
        transport: {
            read: readFunction,
            create: insertFunction,
            update: updateFunction,
            destroy: deleteFunction,
            parameterMap: function (options, operation) {
                if (operation !== "read" && options.models) {
                    return { models: kendo.stringify(options.models) };
                }
            }
        },
        schema: {
            model: {
                id: idModel,
                parentId: idModelParent,
                fields: campiKendoModel
            }
        },
        aggregate: parametriDataSource.aggregate 
    });

    var originalMouseLeave = kendo.ui.Menu.fn._mouseleave;
    var mouseLeave = function (e) {
        var that = this;
        clearTimeout(this._timeoutHandle);
        this._timeoutHandle = setTimeout(function () {
            originalMouseLeave.call(that, e);
        }, 1000);
    }

    kendo.ui.Menu.fn._mouseleave = mouseLeave; // function() {};

    var originalMouseEnter = kendo.ui.Menu.fn._mouseenter;
    var mouseEnter = function (e) {
        clearTimeout(this._timeoutHandle);
        originalMouseEnter.call(this, e);
    }
       
    var treeList = $("#" + IDControllo).data("kendoTreeList");
    if (treeList != null) {
        treeList.destroy();
        $("#" + IDControllo).empty();
    }
    kendo.ui.Menu.fn._mouseenter = mouseEnter;
    var kendo_treelist = $("#" + IDControllo).kendoTreeList(
      {
          dataSource: dataSourceKendoTreeList,
          scrollable: parametriKendoTreeList.scrollable,
          sortable: parametriKendoTreeList.sortable,
          filterable: parametriKendoTreeList.filterable,
          resizable: parametriKendoTreeList.resizable ,
          reorderable: parametriKendoTreeList.reorderable,
          height: parametriKendoTreeList.height,
          toolbar: toolbar,
          dataBinding: onDataBindingKendoTreeList,
          dataBound: onDataBoundKendoTreeList,
          edit: onEditKendoTreeList,
          save: onSaveKendoTreeList,
          columns: colonneKendoTreeList,
          columnMenu: parametriKendoTreeList.columnMenu,
          //columnHide: onColumnHideKendoTreeList,
          //columnShow: onColumnShowKendoTreeList,
          editable: true,
          ////mobile: true,
          filterMenuInit: onFilterMenuInitTreeList,
          //noRecords: {
          //    template: "Non sono presenti dati"
          //},
          excel: {
              filterable: true
          }
      });

    //Se impostato ripristino le personalizzazioni della griglia
    if (parametriKendoTreeList.salvaRipristinaPersonalizzazioni != false)
        ripristinaPersonalizzazioniKendoTreeList(parametriKendoTreeList.salvaRipristinaPersonalizzazioni.url, location.pathname, IDControllo);

    var idEventoSelect = "#" + IDControllo + "-header-chb";

    //evento "seleziona tutte le righe"
    $(idEventoSelect).change(function (ev) {

        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSelectAllRows != null) {
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSelectAllRows(ev);
        }

        var checked = ev.target.checked;

        var treeList = $(this).closest("[data-role=treeList]").data("kendoTreeList");
        var dataSource = treeList.dataSource;
        var filters = dataSource.filter();
        var allData = dataSource.data();
        var query = new kendo.data.Query(allData);
        var filteredData = query.filter(filters).data;

        // La scelta fatta in testata viene estesa ad ogni riga di ogni pagina
        // Di conseguenza 
        $.each(filteredData, function (idx, dataItem) {
            //verifico se la riga è disabilitata
            var chk = $("#" + dataItem.id);

            if ($(chk).closest('tr').css('pointer-events')!= 'none') {

                dataItem.Selected = checked;
                dataItem.dirty = true;
                var row = treeList.tbody.find("tr[data-uid='" + dataItem.uid + "']");
           
                if (checked ) {
                    if (!($(chk).closest('tr').is('.' + GIAS_K_STATE_SELECTED))) {
                        $(chk).click();
                    }
                    row.addClass(GIAS_K_STATE_SELECTED);
                } else {
                    if ($(chk).closest('tr').is('.' + GIAS_K_STATE_SELECTED)) {
                        $(chk).click();
                    }
                    row.removeClass(GIAS_K_STATE_SELECTED);
                }
            }
        });

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoSelectAllRows != null) {
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoSelectAllRows(ev);
        }

    });

    function onEditKendoTreeList(e) {
    
        var fieldName = e.container.find("input").attr("name");

        // Se la riga è marcata come cancellata non permetto più la modifica
        if (e.model.deleted != null && e.model.deleted)
        {
            this.closeCell(); // prevent editing
        }

        // Gestione di colonne editabili per permettere l'inserimento ma che non devono essere modificabili in modifica

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoEdit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoEdit(e);


    }

    function onDataBindingKendoTreeList(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBinding != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBinding(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBinding != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBinding(e);
    }

    function onDataBoundKendoTreeList(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBound != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBound(e);


        var treeListId = e.sender.element[0].id;
        var treeList = $("#" + treeListId).data("kendoTreeList");
        var data = treeList.dataSource.data();

        // Attach dell'evento in tutti i checkbox di selezione eccetto quello di testata
        $("#" + treeListId + " .k-checkbox").not(".header-chb").click(funzioniCRUD.checkBoxFunction);

        // Riapplico lo stile a tutte le righe cancellate
        var rows = e.sender.tbody.children();
        for (var i = 0; i < rows.length; i++) {
            var row = $(rows[i]);
            var dataItem = e.sender.dataItem(row);

            var chk = $("#" + dataItem.id)
            if (dataItem.Selected !== undefined)
            {
                if (dataItem.Selected) {
                    chk.prop('checked', true);
                    //-select the row
                    if (!row.hasClass(GIAS_K_STATE_SELECTED))
                        row.addClass(GIAS_K_STATE_SELECTED);
                } else {
                    chk.prop('checked', false);
                    //-remove selection
                    if (row.hasClass(GIAS_K_STATE_SELECTED))
                        row.removeClass(GIAS_K_STATE_SELECTED);
                }
            }
            else {
                dataItem.Selected = false;
                chk.prop('checked', false);
                //-remove selection
                if (row.hasClass(GIAS_K_STATE_SELECTED))
                    row.removeClass(GIAS_K_STATE_SELECTED);
            }
        }

        // Necessario per Delete Custom
        if (funzioniCRUD.UtenteAbilitatoCancellazione)
        {
            if (funzioniCRUD.funzioneDelete != null) {
                e.sender.tbody.find(".k-button.fa").each(function (idx, element) {
                    $(element).removeClass("fa fa-trash-o").find("span").addClass("fa fa-trash-o del_elem");
                    var innerContent = $(element).html().replace("Cancella", "");
                    $(element).html(innerContent);
                });
            }
        }
        
        gestisciPermessiModificaTreeList(treeList, treeListId);

        //se non specificato equivale a true
        if (parametriKendoTreeList.impostaColonneKendoTreeListDaCookie === undefined || 
            parametriKendoTreeList.impostaColonneKendoTreeListDaCookie === null ||
            parametriKendoTreeList.impostaColonneKendoTreeListDaCookie == true) {

            impostaColonneKendoTreeListDaCookie(treeList, treeListId);
        }

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBound != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBound(e);
    }

    function onSaveKendoTreeList(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelSave != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelSave(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoSave != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoSave(e);
    }

    function gestisciPermessiModificaTreeList(treeList, treeListId) {
        if (!funzioniCRUD.UtenteAbilitatoInserimentoModifica && !funzioniCRUD.UtenteAbilitatoCancellazione) {
            // QUESTO SE VOGLIO NASCONDERE TUTTA LA TOOLBAR
            //$("#" + treeListId + " .k-treeList-cancel-changes").parent().hide();

            $("#" + treeListId + " .k-treeList-cancel-changes").hide();
            $("#" + treeListId + " .k-treeList-save-changes").hide();
            //$("#" + treeListId + " .k-add").parent().hide();
            //$("#" + treeListId + " .k-update").parent().hide();
            //$("#" + treeListId + " .k-cancel").parent().hide();
            //OR
            //$("#" + treeListId + " .k-treeList-cancel-changes").remove();
            //$("#" + treeListId + " .k-treeList-save-changes").remove();
            //$("#" + treeListId + " .k-add").parent().remove();
            //$("#" + treeListId + " .k-update").parent().remove();
            //$("#" + treeListId + " .k-cancel").parent().remove();
        }
    }


    return kendo_treelist;

}  // Fine creaKendoTreeList ... non spostare prima degli eventi perchè ci sono variabili testate


function onColumnHideKendoTreeList(e) {
    var treeListId = e.sender.element[0].id;
    var treeList = $("#" + treeListId).data("kendoTreeList");

    var date = new Date();
    var m = 60;
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));

    var columnsHidden = $.cookie(treeListId + "TreeList");
    if (columnsHidden == null)
        columnsHidden = "";
    if (columnsHidden.indexOf(e.column.field + ";") == -1) {
        columnsHidden += e.column.field + ";";
        $.cookie(treeListId + "TreeList", columnsHidden, { expires: date });
    }
}

function onColumnShowKendoTreeList(e) {
    var treeListId = e.sender.element[0].id;

    var date = new Date();
    var m = 60;
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));

    var columnsHidden = $.cookie(treeListId + "TreeList");
    if (columnsHidden == null)
        columnsHidden = "";
    if (columnsHidden.indexOf(e.column.field + ";") !== -1) {
        columnsHidden = columnsHidden.replace(e.column.field + ";", "");
        $.cookie(treeListId + "TreeList", columnsHidden, { expires: date });
    }
}

function onFilterMenuInitTreeList(e) {

    // In caso di aggiunta della colonna Selected solo lato client (quindi senza che il modello sia costruito
    // da dati letti da DB, questa contiene undefined (il defaultValue sul model sembra non funzionare)
    // Di conseguenza il filtro per avere le sole righe non selezionate non funzionerebbe
    // Questo evento viene lanciato solo la prima volta che si clicca sul campo di filtro e quindi si può
    // impostare false dove è undefined
    if (e.field == "Selected") {
        var treeListId = e.sender.element[0].id;
        var treeList = $("#" + treeListId).data("kendoTreeList");
        var data = treeList.dataSource.data();
        for (var i = data.length - 1; i >= 0; i--) {
            if (data[i].Selected == undefined) {
                data[i].Selected = false;
            }
        }
    }
}


function impostaColonneKendoTreeListDaCookie(treeList, treeListId) {
    var columnsHidden = $.cookie(treeListId + "TreeList");
    if (columnsHidden != null) {
        var colNames = columnsHidden.split(";");
        for (var n = 0; n < colNames.length; n++) {
            for (var i = 0; i < treeList.columns.length; i++) {
                if (treeList.columns[i] != null && treeList.columns[i].field == colNames[n]) {
                    treeList.hideColumn(treeList.columns[i].field);
                }
            }
        }
    }
}


function kGetElementiSelezionatiTreeList(jquery_Selector) {

    var rval = new Array();

    var treeList = $(jquery_Selector).closest("[data-role=treeList]").data("kendoTreeList");
    if (treeList == undefined) {
        return rval;
    }

    var dataSource = treeList.dataSource;
    var allData = dataSource.data();
    

    $.each(allData, function (idx, dataItem) {
        if (dataItem.Selected)
            rval.push(dataItem);
    });
    
    return rval;

}


/**
* Estende le colonne della griglia, aggiungendo se necessario il template per le combo
*
* @param {kendo_model} kendo_model modello kendo
* @param {string} field nome del campo
* @param {string} title titolo della colonna
* @param {number} posizione posizione a base zero
* @param {string} template nome del campo template da utilizzare
* @param {string} editor_template template per le combo
* @param {Object} command comando da associare alla colonna
*/
function kendo_Colonne_estendiTreeList(kendo_model, field, title, posizione, template_field, editor_template, css, command) {

    var tf = "";
    if (template_field != "")
        tf = "#=" + template_field + "#";

    var attributes = undefined;

    if (css !== undefined) {
        attributes = { "class": css };
    }

    var nuovaColonna = undefined;
    if (editor_template !== undefined && editor_template !== null) {
        nuovaColonna = {
            field: field,
            title: title,
            editor: editor_template,
            template: tf,
            attributes: attributes,
            command: command
        };
    } else {
        nuovaColonna = {
            field: field,
            title: title,
            command: command
        };
    }

    kendo_model.kendo_columns.splice(posizione, 0, nuovaColonna);

}
/**
* Imposta il valore di una cella nella tabella della griglia Kendo (e nel model)
*
* @param {dataItem} dataItem Data Item Kendo
* @param {string} nome del campo del model
* @param {object} valore valore da impostare
* @param {string} formattazione (in formato kendo) da impostare sulla griglia (es.: per i valori numerici)
*/
function kendo_imposta_valoreTreeList(dataItem, NomeCampo, valore, formattazione) {

    var roundX = 0;
    if (formattazione !== undefined && formattazione !== null) {

        switch (formattazione[0]) {
            case "n":
                roundX = parseInt(formattazione.replace("n", ""));
                valore = roundNumber(valore, roundX);
            default:

        }

    }

    dataItem.set(NomeCampo, valore);
    dataItem.dirty = true;
}

/**
* Dato il nome del campo del modello restituisce l'indice della colonna nella tabella della griglia kendo
*
* @param {string} jQuerySelector Selettore
* @param {string} NomeCampo Nome del campo
*/
function kendo_indiceColonna_DatoNomeCampoTreeList(jQuerySelector, NomeCampo) {
    return $(jQuerySelector).find("th[data-field='" + NomeCampo + "']").index();
}

/**
* Aggiusta automaticamente la larghezza delle colonne per il tree list recuperato dal selettore JQuery 
*
* @param {string} jQuerySelector Selettore JQuery
*/
function kendo_AggiustaDimensioneColonneTreeList(jQuerySelector) {
    var kendoTreeList = $(jQuerySelector).data("kendoTreeList");
    for (var i = 0; i < kendoTreeList.columns.length; i++) {
        if (kendoTreeList.columns[i].widthfisso === undefined)
            kendoTreeList.autoFitColumn(i);
    }

}

function salvaPersonalizzazioniKendoTreeList(urlbase, pagina, nomeDiv) {

    //var pagina = location.pathname; es: "/AgronicaAgenda_2010/Menu/MenuBS_Agenda_Nuovo.aspx"
    //var nomeDiv = 'divKendoOperazioni'; es: "divKendoOperazioni"

    var jsonDaSalvare = getPersonalizzazioniKendoTreeList(pagina, nomeDiv);

    //localStorage[pagina + '|' + nomeDiv] = jsonDaSalvare;

    if (objP_utenti && objP_utenti != "") {

        var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "personalizzazione": jsonDaSalvare });

        ajaxAgronica(urlbase + PAGINA_CORE_UTENTI_IMPOSTAZIONI_R + '/Set_PersonalizzazioniGrigliaKendo', parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    var x = risposta.RispostaStringa;
                    alert("Personalizzazioni griglia salvate correttamente.");
                }
                else
                    console.log("Err in salvataggio personalizzazioni kendo: pagina-> " + pagina + " - nomeDiv-> " + nomeDiv + " - errore-> " + risposta.Errore)
            }, null);
    }
}

function ripristinaPersonalizzazioniKendoTreeList(urlbase, pagina, nomeDiv) {

    //var jsonDaRipristinare = localStorage[pagina + '|' + nomeDiv];

    if (objP_utenti && objP_utenti != "") {

        var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "pagina": pagina, "nomeDiv": nomeDiv });

        ajaxAgronica(urlbase + PAGINA_CORE_UTENTI_IMPOSTAZIONI_R + '/Get_PersonalizzazioniGrigliaKendo', parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    var jsonDaRipristinare = risposta.RispostaStringa;
                    setPersonalizzazioniKendoTreeList(pagina, nomeDiv, jsonDaRipristinare);
                }
                else
                    console.log("Err in lettura personalizzazioni kendo: pagina-> " + pagina + " - nomeDiv-> " + nomeDiv + " - errore-> " + risposta.Errore)
            }, null);
    }



}

function getPersonalizzazioniKendoTreeList(pagina, nomeDiv) {

    //var pagina = location.pathname; es: "/AgronicaAgenda_2010/Menu/MenuBS_Agenda_Nuovo.aspx"
    //var nomeDiv = 'divKendoOperazioni'; es: "divKendoOperazioni"
    var treeList = $('#' + nomeDiv).data('kendoTreeList');
    var dataSource = treeList.dataSource;

    //var options = treeList.getOptions();

    var columns = treeList.columns;
    var sort = dataSource.sort();
    var filter = dataSource.filter();
    var group = dataSource.group();

    var options = { pagina: pagina, nomeDiv: nomeDiv, columns: columns, sort: sort, filter: filter, group: group };
    var jsonDaSalvare = kendo.stringify(options);

    return jsonDaSalvare
}

function setPersonalizzazioniKendoTreeList(pagina, nomeDiv, jsonDaRipristinare) {

    if (jsonDaRipristinare && jsonDaRipristinare != "") {
        try {

            var options = JSON.parse(jsonDaRipristinare);

            var treeList = $('#' + nomeDiv).data('kendoTreeList');
            var dataSource = treeList.dataSource;
            //treeList.setOptions(options);

            var savedColumns = options.columns;

            //RIORDINAMENTO COLONNE
            for (i = savedColumns.length - 1; i >= 0; i--) {
                var col;

                //Cerco la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
                if (savedColumns[i].field && savedColumns[i].field != null) {
                    col = treeList.columns.find(function (v, index) { return treeList.columns[index].field == savedColumns[i].field; });
                } else if (savedColumns[i].title) {
                    col = treeList.columns.find(function (v, index) { return treeList.columns[index].title == savedColumns[i].title; });
                }

                //Se ho trovato la colonna...
                if (col) {
                    //Sposto la colonna in testa
                    treeList.reorderColumn(0, col);
                }

            }

            //MOSTRO O NASCONDO COLONNE
            for (i = savedColumns.length - 1; i >= 0; i--) {
                var col;

                //Cerco la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
                if (savedColumns[i].field && savedColumns[i].field != null) {
                    col = treeList.columns.find(function (v, index) { return treeList.columns[index].field == savedColumns[i].field; });
                } else if (savedColumns[i].title) {
                    col = treeList.columns.find(function (v, index) { return treeList.columns[index].title == savedColumns[i].title; });
                }

                //Se ho trovato la colonna...
                if (col) {
                    if (savedColumns[i].hidden == true) {
                        treeList.hideColumn(col);
                    } else { //else if (savedColumns[i].hidden == false) 
                        treeList.showColumn(col);
                    }
                }
            }

            //ORDINAMENTO (ASC/DESC) DELLE COLONNE
            if (options.sort) {

                var ordinam = options.sort;
                for (i = ordinam.length - 1; i >= 0; i--) {
                    var col = treeList.columns.find(function (v, index) { return treeList.columns[index].field == ordinam[i].field; });

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
                for (i = filtri.length - 1; i >= 0; i--) {
                    var col = treeList.columns.find(function (v, index) { return treeList.columns[index].field == filtri[i].field; });

                    if (col == undefined) {
                        filtri.splice(i, 1);
                    }
                }

                if (filtri.length > 0)
                    dataSource.filter(options.filter);
            }

            //RAGGRUPPAMENTO IN TOOLBAR
            //DA TESTARE
            //            if (options.group) {

            //                var raggrup = options.group;
            //                for (i = raggrup.length - 1; i >= 0; i--) {
            //                    var col = treeList.columns.find(function (v, index) { return treeList.columns[index].field == raggrup[i].field; });

            //                    if (col == undefined) {
            //                        raggrup.splice(i, 1);
            //                    }
            //                }

            //                if (raggrup.length > 0)
            //                    dataSource.group(options.group);
            //            }
        }
        catch (err) {
            console.log(err);
        }
    }

}