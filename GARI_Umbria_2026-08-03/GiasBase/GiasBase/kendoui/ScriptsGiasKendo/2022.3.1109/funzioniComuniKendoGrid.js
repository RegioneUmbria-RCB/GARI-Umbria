var indirizzohttp_GridServerExport = "AgronicaCoreUtility/KendoGridServerExport.asmx";

/**
* Restituisce l'oggetto Kendo Grid
*
* @param {string} IDControllo Id/name del controllo
* @returns {Object<string,any>} oggetto Kendo Grid
*/
function KendoGrid(IDControllo) {
    var input = $("#" + IDControllo);
    if (input.length <= 0) {
        input = $('[name$="' + IDControllo + '"]');
    }
    return input.data("kendoGrid");
}

/**
* Creazione di una griglia Kendo
*
* @param {string} IDControllo Rappresenta il selector JQuery del div a cui si associa la griglia
* @param {object} funzioniCRUD Funzioni js da chiamare per read, insert, update, delete { chiave - vaAlore}: { funzioneRead: xxxx, funzioneInsert: yyyy, funzioneUpdate: zzzz, funzioneDelete: kkkk }
* @param {string} idModel chiave di riga 
* @param {object} campiKendoModel definizione del modello kendo
* @param {object} colonneKendoGrid definizione delle colonne
*/
function creaKendoGrid(
    // PARAMETRI OBBLIGATORI
    IDControllo,
    funzioniCRUD,
    idModel,
    campiKendoModel,
    colonneKendoGrid,
    // PARAMETRI FACOLTATIVO
    parametriPerLettura, // parametri da passare alla lettura
    parametriDataSource, // parametri data source { chiave - valore}
    parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
    funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi { chiave - valore}:
    //{   
    //    funzioneDaChiamarePrimaDelDataBinding: yyyyy, // funzione da chiamare all'inizio del databinding
    //    funzioneDaChiamareDopoDataBinding: yyyyy, // funzione da chiamare alla fine del databinding
    //    funzioneDaChiamarePrimaDelDataBound: yyyyy, // funzione da chiamare all'inizio del databound
    //    funzioneDaChiamareDopoDataBound: yyyyy, // funzione da chiamare alla fine del databound
    //    funzioneDaChiamarePrimaDelSave: yyyyy, // funzione da chiamare all'inizio del save
    //    funzioneDaChiamareDopoSave: yyyyy // funzione da chiamare alla fine del save
    //    funzioneDaChiamareDopoDelete: yyyyy // funzione da chiamare dopo la cancellazione di una riga 
    //    funzioneDaChiamareDopoAnnulla: yyyyy  // funzione da chiamare quando si annulla una modifica sulla riga
    //}
    mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
    colonneDisabilitateSoloInModifica, // colonne non modificabili in modifica["colA", "colB", ...]
    grigliaEndlessScrolling // se true chiama specifica funzione creaKendoGridEndless
) {

    if (IDControllo == null) {
        alert("Non mi hai passato l'ID del DIV che contiene la griglia");
        return;
    }

    var grigliaEndless = false;
    if (grigliaEndlessScrolling != null && grigliaEndlessScrolling != 'undefined') {
        grigliaEndless = grigliaEndlessScrolling
    }
    if (grigliaEndless) {
        creaKendoGridEndlessScrolling(IDControllo,
            funzioniCRUD,
            idModel,
            campiKendoModel,
            colonneKendoGrid,
            parametriPerLettura,
            parametriDataSource,
            parametriKendoGrid,
            funzioniPrimaDopoEventi,
            mostraRigheCancellate,
            colonneDisabilitateSoloInModifica
        )
        return;
    }

    if (funzioniCRUD == null || funzioniCRUD.funzioneRead == null) {
        alert("Non mi hai passato la funzione da chiamare in lettura");
        return;
    }

    if (funzioniCRUD != null) {
        errFound = false;
        for (var k in funzioniCRUD) {
            if (k != "funzioneRead" &&
                k != "funzioneInsert" &&
                k != "funzioneUpdate" &&
                k != "funzioneDelete" &&
                k != "funzioneSubmit" &&
                k != "checkBoxFunction" &&
                k != "omettiPulsantiSalva" &&
                k != "omettiPulsantiAnnulla" &&
                k != "UtenteAbilitatoInserimentoModifica" &&
                k != "UtenteAbilitatoCancellazione" &&
                k != "gestisciSalvataggioFinaleAParte" &&
                k != "menuColonneAdattivo") {
                errFound = true;
                alert("Fra le funzioni CRUD mi hai passato la chiave " + k + " che non è gestita");
            }
        }
        if (!errFound) {
            // la funzioneSubmit evita/impedisce che vengano chiamate la funzioneInsert, la funzioneDelete e la funzioneUpdate
            // se ho valorizzato la funzioneSubmit verifico che non abbia valorizzato nessuna delle altre (solo per chiarezza)
            if ((funzioniCRUD.funzioneSubmit !== undefined && funzioniCRUD.funzioneSubmit !== null && funzioniCRUD.funzioneSubmit.funzione != null)  //ho valorizzato la funzioneSubmit...
                && (funzioniCRUD.funzioneInsert != null ||
                    funzioniCRUD.funzioneUpdate != null ||
                    funzioniCRUD.funzioneDelete != null)) {

                errFound = true;
                alert("Fra le funzioni CRUD mi hai passato la chiave funzioneSubmit ed una tra funzioneInsert, funzioneUpdate e funzioneDelete");

            }
        }
        if (errFound)
            return;

        // Se il parametro non viene passato si assume che l'utente abbia i permessi
        funzioniCRUD.UtenteAbilitatoInserimentoModifica = (typeof funzioniCRUD.UtenteAbilitatoInserimentoModifica === 'undefined') ? true : funzioniCRUD.UtenteAbilitatoInserimentoModifica;
        funzioniCRUD.UtenteAbilitatoCancellazione = (typeof funzioniCRUD.UtenteAbilitatoCancellazione === 'undefined') ? true : funzioniCRUD.UtenteAbilitatoCancellazione;

        if (funzioniCRUD.funzioneSubmit === undefined) {
            funzioniCRUD.funzioneSubmit = { funzione: null };
        }

        if (funzioniCRUD.funzioneSubmit.funzione != null) {
            funzioniCRUD.funzioneSubmit.flagUpdate = (typeof funzioniCRUD.funzioneSubmit.flagUpdate === 'undefined') ? true : funzioniCRUD.funzioneSubmit.flagUpdate;
            funzioniCRUD.funzioneSubmit.flagInsert = (typeof funzioniCRUD.funzioneSubmit.flagInsert === 'undefined') ? true : funzioniCRUD.funzioneSubmit.flagInsert;
            funzioniCRUD.funzioneSubmit.flagDelete = (typeof funzioniCRUD.funzioneSubmit.flagDelete === 'undefined') ? true : funzioniCRUD.funzioneSubmit.flagDelete;

            if (funzioniCRUD.funzioneSubmit.flagUpdate == false &&
                funzioniCRUD.funzioneSubmit.flagInsert == false &&
                funzioniCRUD.funzioneSubmit.flagDelete == false) {

                alert("Hai passato la funzioneSubmit nelle funzioni CRUD con tutti i flag falsi");
                return;

            }
        }
        else {
            funzioniCRUD.funzioneSubmit.flagUpdate = false;
            funzioniCRUD.funzioneSubmit.flagInsert = false;
            funzioniCRUD.funzioneSubmit.flagDelete = false;
        }

    }

    // Se true non vengono mostrati i pulsanti Salva e Annulla anche se c'è una funzione di Submit o di Insert/Update/Delete
    // Utilizzato quando si vuole fare il salvataggio con un pulsante esterno
    if (funzioniCRUD.omettiPulsantiSalva === undefined) {
        funzioniCRUD.omettiPulsantiSalva = false;
    }

    if (funzioniCRUD.omettiPulsantiAnnulla === undefined) {
        funzioniCRUD.omettiPulsantiAnnulla = false;
    }

    if (idModel == null) {
        alert("Non mi hai passato la chiave della riga della griglia");
        return;
    }

    if (campiKendoModel == null) {
        alert("Non mi hai passato i campi del modello della griglia");
        return;
    }

    if (colonneKendoGrid == null) {
        alert("Non mi hai passato le colonne della griglia");
        return;
    }

    funzioniPrimaDopoEventi = (typeof funzioniPrimaDopoEventi === 'undefined') ? {} : funzioniPrimaDopoEventi;
    errFound = false;
    for (var k in funzioniPrimaDopoEventi) {
        if (k != "funzioneDaChiamarePrimaDelDataBinding" &&
            k != "funzioneDaChiamareDopoDataBinding" &&
            k != "funzioneDaChiamarePrimaDelDataBound" &&
            k != "funzioneDaChiamareDopoDataBound" &&
            k != "funzioneDaChiamarePrimaDelDetailInit" &&
            k != "funzioneDaChiamareDopoDetailInit" &&
            k != "funzioneDaChiamarePrimaDelChange" &&
            k != "funzioneDaChiamareDopoChange" &&
            k != "funzioneDaChiamarePrimaDelSave" &&
            k != "funzioneDaChiamareDopoSave" &&
            k != "funzioneDaChiamarePrimaDiSelectAllRows" &&
            k != "funzioneDaChiamareDopoSelectAllRows" &&
            k != "funzioneDaChiamarePrimaDiEdit" &&
            k != "funzioneDaChiamareDopoEdit" &&
            k != "funzioneDaChiamarePrimaDiSaveChangesKendoGrid" &&
            k != "funzioneDaChiamareDopoDelete" &&
            k != "funzioneDaChiamarePrimaDiExcelExport" &&
            k != "funzioneDaChiamareDopoAnnulla") {
            errFound = true;
            alert("Fra le funzioni da chiamare prima o dopo agli eventi mi hai passato la chiave " + k + " che non è gestita");
        }
    }
    if (errFound)
        return;

    //costruisco il template sull'elemento quando mi è stato passato come id (costruendo la griglia lato server)
    // (necessario perché passando da json non è possibile fare lo store di funzioni, ma solo di stringhe)
    colonneKendoGrid.forEach(function (x) {
        if ((x.templateIdControllo !== undefined && x.templateIdControllo !== null && x.templateIdControllo !== "") &&
            (x.template === undefined || x.template === null || x.template === "")) {
            x.template = kendo.template($("#" + x.templateIdControllo).html());
            delete x.templateIdControllo;
        }
    });



    // Per le funzioni di accesso al database è necessario fare delle funzioni anonime 
    // altrimenti le chiama subito nel momento in cui crea la griglia
    var readFunction = null;
    if (funzioniCRUD.funzioneRead != null) {
        if (parametriPerLettura != null && parametriPerLettura.length > 0) {
            readFunction = function (options) {
                funzioniCRUD.funzioneRead(options, parametriPerLettura);

                //var risp = funzioniCRUD.funzioneRead(parametriPerLettura.toString());
                //options.success(risp);
            };
        }
        else {
            readFunction = function (options) {
                funzioniCRUD.funzioneRead(options);

            };
        }
    }

    if (funzioniCRUD.UtenteAbilitatoInserimentoModifica) {
        var submitFunction = null;
        if (funzioniCRUD.funzioneSubmit.funzione != null &&
            (funzioniCRUD.funzioneSubmit.flagInsert || funzioniCRUD.funzioneSubmit.flagUpdate)) {
            submitFunction = (function (options) {
                if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                    options.error();
                }
                funzioniCRUD.funzioneSubmit.funzione(options, parametriPerLettura, parametriDataSource.parametriInsert, parametriDataSource.parametriUpdate);
            });
        }
        else {
            var insertFunction = null;
            if (funzioniCRUD.funzioneInsert != null) {
                insertFunction = (function (options) {
                    if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                        options.error();
                    }
                    funzioniCRUD.funzioneInsert(options, parametriDataSource.parametriInsert);
                });
            }

            var updateFunction = null;
            if (funzioniCRUD.funzioneUpdate != null) {
                updateFunction = (function (options) {
                    if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                        options.error();
                    }
                    funzioniCRUD.funzioneUpdate(options, parametriDataSource.parametriUpdate);
                });
            }
        }
    }

    //qui:
    if (funzioniCRUD.checkBoxFunction != null) {

        $.extend(campiKendoModel, {
            Selected: { type: "boolean", editable: false }
        });
    }

    if (funzioniCRUD.menuColonneAdattivo === undefined) {
        funzioniCRUD.menuColonneAdattivo = true;
    }

    parametriDataSource = (typeof parametriDataSource === 'undefined' || parametriDataSource == null) ? {} : parametriDataSource;
    parametriKendoGrid = (typeof parametriKendoGrid === 'undefined' || parametriKendoGrid == null) ? {} : parametriKendoGrid;
    parametriKendoGrid.groupable = (typeof parametriKendoGrid.groupable === 'undefined') ? true : parametriKendoGrid.groupable;
    parametriKendoGrid.scrollable = (typeof parametriKendoGrid.scrollable === 'undefined') ? true : parametriKendoGrid.scrollable;
    parametriKendoGrid.sortable = (typeof parametriKendoGrid.sortable === 'undefined') ? true : parametriKendoGrid.sortable;
    parametriKendoGrid.resizable = (typeof parametriKendoGrid.resizable === 'undefined') ? true : parametriKendoGrid.resizable;
    parametriKendoGrid.filterable = (typeof parametriKendoGrid.filterable === 'undefined') ? true : parametriKendoGrid.filterable;
    parametriKendoGrid.editable = (typeof parametriKendoGrid.editable === 'undefined') ? true : parametriKendoGrid.editable;
    parametriKendoGrid.pageable = (typeof parametriKendoGrid.pageable === 'undefined') ? true : parametriKendoGrid.pageable;
    parametriKendoGrid.excel = (typeof parametriKendoGrid.excel === 'undefined') ? true : parametriKendoGrid.excel;
    parametriKendoGrid.pdf = (typeof parametriKendoGrid.pdf === 'undefined') ? true : parametriKendoGrid.pdf;
    parametriKendoGrid.search = (typeof parametriKendoGrid.search === 'undefined') ? false : parametriKendoGrid.search;
    parametriKendoGrid.pdfTemplate = (typeof parametriKendoGrid.pdfTemplate === 'undefined') ? { allPages: true } : parametriKendoGrid.pdfTemplate;
    parametriKendoGrid.columnMenu = (typeof parametriKendoGrid.columnMenu === 'undefined') ? true : parametriKendoGrid.columnMenu;
    parametriKendoGrid.toolbarCommands = (typeof parametriKendoGrid.toolbarCommands === 'undefined') ? null : parametriKendoGrid.toolbarCommands;
    parametriKendoGrid.height = (typeof parametriKendoGrid.height === 'undefined') ? null : parametriKendoGrid.height;
    parametriKendoGrid.colonneCustomKendoGrid = (typeof parametriKendoGrid.colonneCustomKendoGrid === 'undefined') ? null : parametriKendoGrid.colonneCustomKendoGrid;
    parametriKendoGrid.reorderable = (typeof parametriKendoGrid.reorderable === 'undefined') ? null : parametriKendoGrid.reorderable;
    parametriKendoGrid.rowTemplate = (typeof parametriKendoGrid.rowTemplate === 'undefined') ? null : parametriKendoGrid.rowTemplate;
    parametriKendoGrid.checkSelezioneRiga = (typeof parametriKendoGrid.checkSelezioneRiga === 'undefined') ? null : parametriKendoGrid.checkSelezioneRiga;
    parametriKendoGrid.salvaRipristinaPersonalizzazioni = (typeof parametriKendoGrid.salvaRipristinaPersonalizzazioni === 'undefined') ? false : parametriKendoGrid.salvaRipristinaPersonalizzazioni;
    parametriKendoGrid.selectable = (typeof parametriKendoGrid.selectable === 'undefined') ? false : parametriKendoGrid.selectable;
    parametriKendoGrid.btnEliminaTuttiFiltri = (typeof parametriKendoGrid.btnEliminaTuttiFiltri === 'undefined') ? true : parametriKendoGrid.btnEliminaTuttiFiltri;
    parametriKendoGrid.lockCancella = (typeof parametriKendoGrid.lockCancella === 'undefined') ? false : parametriKendoGrid.lockCancella;

    var editPopupMode = false;
    var editInlineMode = false;
    editPopupMode = (parametriKendoGrid.editable !== false &&
        (parametriKendoGrid.editable === "popup" ||
            (parametriKendoGrid.editable.mode !== undefined && parametriKendoGrid.editable.mode === "popup")));
    editInlineMode = (parametriKendoGrid.editable !== false &&
        (parametriKendoGrid.editable === "inline" ||
            (parametriKendoGrid.editable.mode !== undefined && parametriKendoGrid.editable.mode === "inline")));

    parametriDataSource.pagesize = (typeof parametriDataSource.pagesize === 'undefined') ? 10 : parametriDataSource.pagesize;
    // Se non c'è paginazione il pagesize viene impostato altissimo (se si imposta a zero non funziona bene il filtro)
    if (!parametriKendoGrid.pageable)
        parametriDataSource.pagesize = 99999999;
    parametriDataSource.aggregate = (typeof parametriDataSource.aggregate === 'undefined') ? null : parametriDataSource.aggregate;
    parametriDataSource.sort = (typeof parametriDataSource.sort === 'undefined') ? null : parametriDataSource.sort;

    // Aggiunta colonne Custom
    if (parametriKendoGrid.colonneCustomKendoGrid != null) {
        //vado all'indietro perchè faccio l'unshift
        for (i = parametriKendoGrid.colonneCustomKendoGrid.length - 1; i >= 0; i--) {
            // Se l'utente non ha permessi di inserimento/modifica e cancellazione elimino il pulsante
            for (var x = parametriKendoGrid.colonneCustomKendoGrid[i].command.length - 1; x >= 0; x--) {
                if (!funzioniCRUD.UtenteAbilitatoInserimentoModifica &&
                    parametriKendoGrid.colonneCustomKendoGrid[i].command[x].name == "edit")
                    parametriKendoGrid.colonneCustomKendoGrid[i].command.splice(x, 1);
                else
                    if (!funzioniCRUD.UtenteAbilitatoCancellazione &&
                        parametriKendoGrid.colonneCustomKendoGrid[i].command[x].name == "destroy")
                        parametriKendoGrid.colonneCustomKendoGrid[i].command.splice(x, 1);
            }

            if ((parametriKendoGrid.colonneCustomKendoGrid[i].command.length !== undefined && parametriKendoGrid.colonneCustomKendoGrid[i].command.length > 0) ||
                parametriKendoGrid.colonneCustomKendoGrid[i].command !== undefined && parametriKendoGrid.colonneCustomKendoGrid[i].command.length === undefined)
                colonneKendoGrid.unshift(parametriKendoGrid.colonneCustomKendoGrid[i]);
        }

    }

    var checkBoxFunction = null;
    var dirtyAllRows = true;
    if (funzioniCRUD.checkBoxFunction != null) {

        var width = 50;
        var sortable = false;
        var filterable = true;
        var title = 'Seleziona';
        var locked = false;
        var field = "Selected";

        if (parametriKendoGrid.checkSelezioneRiga) {
            width = (typeof parametriKendoGrid.checkSelezioneRiga.width === 'undefined') ? width : parametriKendoGrid.checkSelezioneRiga.width;
            sortable = (typeof parametriKendoGrid.checkSelezioneRiga.sortable === 'undefined') ? sortable : parametriKendoGrid.checkSelezioneRiga.sortable;
            filterable = (typeof parametriKendoGrid.checkSelezioneRiga.filterable === 'undefined') ? filterable : parametriKendoGrid.checkSelezioneRiga.filterable;
            title = (typeof parametriKendoGrid.checkSelezioneRiga.title === 'undefined') ? title : parametriKendoGrid.checkSelezioneRiga.title;
            locked = (typeof parametriKendoGrid.checkSelezioneRiga.locked === 'undefined') ? locked : parametriKendoGrid.checkSelezioneRiga.locked;
            field = (typeof parametriKendoGrid.checkSelezioneRiga.field === 'undefined') ? field : parametriKendoGrid.checkSelezioneRiga.field;
            dirtyAllRows = (typeof parametriKendoGrid.checkSelezioneRiga.dirtyAllRows === 'undefined') ? dirtyAllRows : parametriKendoGrid.checkSelezioneRiga.dirtyAllRows;
        }

        var colSelected = {
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
                //return "<input type=\"checkbox\" id=\"" + dataItem.id + "\" #= Selected ? \'checked=\"checked\"\' : \"\" class=\"k-checkbox\"><label style='margin-right:0px;' class=\"k-checkbox-label\" for=\"" + dataItem.id + "\"></label>"

                var m = "<input type=\"checkbox\" id=\"" + IDControllo + "_" + dataItem.id + "\" #= Selected ? checked=\"checked\" : \"\" # class=\"k-checkbox checkbox-selectionRow\"><label style=\"margin-right:0px;\" class=\"k-checkbox-label\" for=\"" + IDControllo + "_" + dataItem.id + "\"></label>";
                //console.debug(m);
                return m;
            }
        };
        if (parametriKendoGrid.headerAttributes !== undefined)
            colSelected.headerAttributes = parametriKendoGrid.headerAttributes;
        colonneKendoGrid.unshift(colSelected);
    }

    if (funzioniCRUD.UtenteAbilitatoCancellazione) {

        var gestisciCancellazione = false;

        if (funzioniCRUD.funzioneSubmit.funzione != null && funzioniCRUD.funzioneSubmit.flagDelete) {
            if (submitFunction == null) {
                submitFunction = (function (options) {
                    if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                        options.error();
                    }
                    funzioniCRUD.funzioneSubmit.funzione(options);
                });
            }

            gestisciCancellazione = true;
        }
        else {
            var deleteFunction = null;
            if (funzioniCRUD.funzioneDelete != null) {
                deleteFunction = (function (options) {
                    if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                        options.error();
                    }
                    funzioniCRUD.funzioneDelete(options);
                });

                gestisciCancellazione = true;
            }
        }

        if (gestisciCancellazione) {
            // Caso in cui si vogliano mostrare le righe cancellate (testo rosso e barrato tramite css)
            if (mostraRigheCancellate) {

                let colAnnulla = {
                    title: "Cancella",
                    width: 100,
                    locked: parametriKendoGrid.lockCancella,
                    command: [{
                        iconClass: "fa fa-trash-o",
                        className: "btn-Cancella",
                        name: "Cancella",
                        text: "&nbsp",
                        click: function (e) {
                            var grid = $("#" + IDControllo).getKendoGrid();
                            var row = $(e.target).closest("tr");
                            dataItem = grid.dataItem(row);
                            dataItem.mostraRigheCancellate = true;
                            if (dataItem.deleted != null && dataItem.deleted) {
                                dataItem.deleted = false;
                                row.removeClass("deletedKendoRow");
                            }
                            else {
                                dataItem.deleted = true;
                                row.addClass("deletedKendoRow");
                            }

                            if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDelete != null) {
                                funzioniPrimaDopoEventi.funzioneDaChiamareDopoDelete(e);
                            }

                            e.preventDefault(e);

                        }
                    }]
                };
                if (parametriKendoGrid.headerAttributes !== undefined)
                    colAnnulla.headerAttributes = parametriKendoGrid.headerAttributes;

                colonneKendoGrid.unshift(colAnnulla);

            }
            else {

                if (!editPopupMode && !editInlineMode) {
                    //Aggiungo la colonna con il pulsante di cancellazione all'inizio della griglia
                    let colAnnulla = { command: [{ name: "destroy", template: "<span class='fa fa-trash-o k-grid-delete'></span>" }], title: "Cancella" };
                    if (parametriKendoGrid.headerAttributes !== undefined)
                        colAnnulla.headerAttributes = parametriKendoGrid.headerAttributes;

                    colonneKendoGrid.unshift(colAnnulla);
                }

            }

        }
    }

    // Sembra non più necessario dalla versione 2017
    if (parametriKendoGrid.filterable) {
        if (parametriKendoGrid.filterable == true) {
            parametriKendoGrid.filterable = {
                messages: {
                    clear: "Pulisci",
                    filter: "Applica",
                    isFalse: "No",
                    isTrue: "Sì",
                    checkAll: "Seleziona tutto",
                    selectedItemsFormat: "{0} elementi selezionati"
                }
            };
        }
        else {
            parametriKendoGrid.filterable['messages'] =
            {
                clear: "Pulisci",
                filter: "Applica",
                isFalse: "No",
                isTrue: "Sì",
                checkAll: "Seleziona tutto",
                selectedItemsFormat: "{0} elementi selezionati"
            };
        }
    }


    if (parametriKendoGrid.pageable) {
        parametriKendoGrid.pageable.input = true;
        parametriKendoGrid.pageable.numeric = true;
    }

    if (parametriKendoGrid.columnMenu) {
        parametriKendoGrid.columnMenu = {
            filterable: true,
            sortable: false,
            columns: true
        };
    }

    if (parametriKendoGrid.sortable) {
        parametriKendoGrid.sortable = {
            mode: "multiple"
        };
    }

    // toolbar
    var toolbar = [];

    if (parametriKendoGrid.excel)
        toolbar.push({ name: "excel", text: "" });

    if (parametriKendoGrid.pdf) {
        toolbar.push({ name: "pdf", text: "" });
    }


    //Se impostato aggiungo il pulsante per salvare le personalizzazioni della griglia
    if (parametriKendoGrid.salvaRipristinaPersonalizzazioni != false)
        toolbar.push({
            name: 'salvaPersGrigliaKendo', template: kendo.template('<div class="grid-customization">' +
                '<div class="k-button btnSalvaPersonalizzazioniGriglia" onclick="salvaPersonalizzazioniGrigliaKendo(&quot;' + parametriKendoGrid.salvaRipristinaPersonalizzazioni.url + '&quot;, &quot;' + location.pathname + '&quot;, &quot;' + IDControllo + '&quot;, true)" ' +
                '>' +
                '<span class="k-icon k-i-save" style="font-size: 17px;"></span ></div>' +
                '<div class="k-button btnCancellaPersonalizzazioniGrigliaKendo ' + GIAS_K_STATE_DISABLED + ' cancellaPersonalizzazioniGrigliaKendo" onclick="cancellaPersonalizzazioniGrigliaKendo(&quot;' + parametriKendoGrid.salvaRipristinaPersonalizzazioni.url + '&quot;, &quot;' + location.pathname + '&quot;, &quot;' + IDControllo + '&quot;, true)" ' +
                '>' +
                '<span class="k-icon k-i-delete" style="margin:0px; font-size:17px;"></span></div>' +
                '</div >')
        });

    if (parametriKendoGrid.btnEliminaTuttiFiltri != false) {

        if ((typeof GiasVersioneMaster != 'undefined') && (GiasVersioneMaster === "2022")) {
            toolbar.push({ name: 'btnEliminaTuttiFiltri', template: kendo.template('<div class="k-button xonne-btn-filters" id="btnEliminaTuttiFiltri" onclick="btnEliminaTuttiFiltri(&quot;' + IDControllo + '&quot;)"><span class="k-icon k-i-filter-clear "></span></div>') });
        }
        else {
            toolbar.push({ name: 'btnEliminaTuttiFiltri', template: kendo.template('<div class="k-button btnEliminaTuttiFiltri" id="btnEliminaTuttiFiltri" onclick="btnEliminaTuttiFiltri(&quot;' + IDControllo + '&quot;)"><span class="k-icon k-i-filter-clear "></span>&nbsp;Pulisci Filtri</div>') });
        }
    }
    if (parametriKendoGrid.toolbarCommands != null) {
        for (i = 0; i < parametriKendoGrid.toolbarCommands.length; i++) {
            var myID = "#" + parametriKendoGrid.toolbarCommands[i];
            toolbar.push({ template: kendo.template($(myID).html()) });
        }
    }

    // pulsanti impostati solo se vengono passate le relative funzioni
    if (!editPopupMode && !editInlineMode && (
        ((funzioniCRUD.funzioneInsert != null || funzioniCRUD.funzioneUpdate != null || funzioniCRUD.funzioneSubmit.funzione != null) && funzioniCRUD.UtenteAbilitatoInserimentoModifica) ||
        ((funzioniCRUD.funzioneDelete != null || funzioniCRUD.funzioneSubmit.funzione != null) && funzioniCRUD.UtenteAbilitatoCancellazione)
    )
    ) {
        if (funzioniCRUD.omettiPulsantiSalva === false) {
            toolbar.push("save");

        }
        if (funzioniCRUD.omettiPulsantiAnnulla === false) {
            toolbar.push("cancel");
        }
    }
    if ((funzioniCRUD.funzioneInsert != null || (funzioniCRUD.funzioneSubmit.funzione != null && funzioniCRUD.funzioneSubmit.flagInsert))
        && funzioniCRUD.UtenteAbilitatoInserimentoModifica) {
        toolbar.push("create");
    }


    if (parametriKendoGrid.search) {
        toolbar.push("search");
    }




    // Cambio il testo di campo obbligatorio nek caso non si sia specificato
    msgRequired(campiKendoModel);

    //Se impostato ripristino le personalizzazioni della griglia
    var jsonDaRipristinare = null;
    var pagina = location.pathname;

    if (parametriKendoGrid.salvaRipristinaPersonalizzazioni != false) {
        jsonDaRipristinare = ripristinaPersonalizzazioniGrigliaKendo(parametriKendoGrid.salvaRipristinaPersonalizzazioni.url, location.pathname, IDControllo);

        if (jsonDaRipristinare !== null && jsonDaRipristinare !== "")
            parametriDataSource = setPersonalizzazioniGrigliaKendoDati(pagina, IDControllo, jsonDaRipristinare, parametriKendoGrid, parametriDataSource, campiKendoModel, colonneKendoGrid);


    }

    var dataSourceKendoGrid = new kendo.data.DataSource({
        transport: {
            read: readFunction,
            create: insertFunction,
            update: updateFunction,
            destroy: deleteFunction,
            submit: submitFunction,
            parameterMap: function (options, operation) {
                if (operation !== "read" && options.models) {
                    return { models: kendo.stringify(options.models) };
                }
            }
        },
        batch: true,
        pageSize: parametriDataSource.pagesize,
        schema: {
            model: {
                id: idModel,
                fields: campiKendoModel
            }
        },
        aggregate: parametriDataSource.aggregate,
        group: parametriDataSource.group,
        filter: parametriDataSource.filter,
        sort: parametriDataSource.sort
    });

    var originalMouseLeave = kendo.ui.Menu.fn._mouseleave;
    var mouseLeave = function (e) {
        var that = this;
        clearTimeout(this._timeoutHandle);
        this._timeoutHandle = setTimeout(function () {
            originalMouseLeave.call(that, e);
        }, 1000);
    };

    kendo.ui.Menu.fn._mouseleave = mouseLeave; // function() {};

    var originalMouseEnter = kendo.ui.Menu.fn._mouseenter;
    var mouseEnter = function (e) {
        clearTimeout(this._timeoutHandle);
        originalMouseEnter.call(this, e);
    };

    var grid = $("#" + IDControllo).data("kendoGrid");
    if (grid != null) {
        grid.destroy();
        $("#" + IDControllo).empty();
    }
    kendo.ui.Menu.fn._mouseenter = mouseEnter;

    var kendo_grid = $("#" + IDControllo).kendoGrid(
        {
            dataSource: dataSourceKendoGrid,
            groupable: parametriKendoGrid.groupable,
            scrollable: parametriKendoGrid.scrollable,
            sortable: parametriKendoGrid.sortable,
            resizable: parametriKendoGrid.resizable,
            reorderable: parametriKendoGrid.reorderable,
            selectable: parametriKendoGrid.selectable,
            rowTemplate: parametriKendoGrid.rowTemplate,
            filterable: parametriKendoGrid.filterable,
            pageable: parametriKendoGrid.pageable,
            height: parametriKendoGrid.height,
            toolbar: toolbar,
            dataBinding: onDataBindingKendoGrid,
            dataBound: onDataBoundKendoGrid,
            detailInit: funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDetailInit === undefined &&
                funzioniPrimaDopoEventi.funzioneDaChiamareDopoDetailInit === undefined ? null : onDetailInitKendoGrid,
            edit: onEditKendoGrid,
            cancel: funzioniPrimaDopoEventi.funzioneDaChiamareDopoAnnulla,
            save: onSaveKendoGrid,
            change: onChangeKendoGrid,
            saveChanges: onSaveChangesKendoGrid,
            filter: onFilterKendoGrid,
            columns: colonneKendoGrid,
            columnMenu: parametriKendoGrid.columnMenu,
            //columnHide: onColumnHideKendoGrid,
            //columnShow: onColumnShowKendoGrid,
            editable: parametriKendoGrid.editable,
            navigatable: true,
            //mobile: true,
            filterMenuInit: onFilterMenuInit,
            columnMenuInit: onColumnMenuInit,
            columnMenuOpen: funzioniCRUD.menuColonneAdattivo === false ? null : onColumnMenuOpen,
            noRecords: {
                template: "Non sono presenti dati"
            },
            excel: {
                allPages: true,
                filterable: true
            },
            excelExport: onExcelExportKendoGrid,
            pdf: parametriKendoGrid.pdfTemplate,
            page: onPage
        });
    // TODO Stefano aggiungere per gestione tab (vedi in fondo) .find("table").on("keydown", onGridKeydown);

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-cancel-changes",
        position: "top",
        content: "Annulla modifiche"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-add",
        position: "top",
        content: "Aggiungi riga"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-excel",
        position: "top",
        content: "Esporta in Excel"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-Info",
        position: "top",
        content: "Informazioni"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-Modifica",
        position: "top",
        content: "Modifica"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-pdf",
        position: "top",
        content: "Esporta in PDF"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".btnSalvaPersonalizzazioniGriglia",
        position: "top",
        content: "Salva personalizzazione"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".btnCancellaPersonalizzazioniGrigliaKendo",
        position: "top",
        content: "Cancella personalizzazione"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-Cancella",
        position: "top",
        content: "Elimina"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".elimina-selezionati",
        position: "top",
        content: "Elimina selezionati"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".importa-selezionati",
        position: "top",
        content: "Importa selezionati"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".controlla-selezionati",
        position: "top",
        content: "Controlla selezionati"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".xonne-btn-filters",
        position: "top",
        content: "Pulisci filtri"
    }).data("kendoTooltip");
 
    let filterExcludeButton = ":not(.k-grid-cancel-changes):not(.k-grid-add):not(.k-grid-excel):not(.k-grid-Info):not(.k-grid-Modifica):not(.k-grid-pdf):not(.btnSalvaPersonalizzazioniGriglia):not(.btnCancellaPersonalizzazioniGrigliaKendo):not(.k-grid-Cancella)";

    //Tooltip su bottoni che hanno un data-title
    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid--button[data-title]" + filterExcludeButton,
        position: "top",
        content: (e) => e.target[0].dataset.title
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-button[data-title]" + filterExcludeButton,
        position: "top",
        content: (e) => e.target[0].dataset.title
    }).data("kendoTooltip");

    //Tooltip su bottoni che hanno un title (escludo quelli che hanno anche un data-title, perché sono ricompresi nei casi sopra)
    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid--button[title]" + filterExcludeButton,
        position: "top",
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-button[title]:not([data-title])" + filterExcludeButton,
        position: "top",
    }).data("kendoTooltip");



    if (jsonDaRipristinare !== null && jsonDaRipristinare !== "") {
        setPersonalizzazioniGrigliaKendoColonne(pagina, IDControllo, jsonDaRipristinare);

        //Se esiste una personalizzazione attivo il pulsante con il cestino (sempre che esista anche il pulsante)
        if ($("#" + IDControllo + " .cancellaPersonalizzazioniGrigliaKendo").length > 0)
            $("#" + IDControllo + " .cancellaPersonalizzazioniGrigliaKendo")[0].classList.remove(GIAS_K_STATE_DISABLED);
    }


    var idEventoSelect = "#" + IDControllo + "-header-chb";

    //evento "seleziona tutte le righe"
    $(idEventoSelect).change(function (ev) {

        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSelectAllRows != null) {
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSelectAllRows(ev);
        }

        var checked = ev.target.checked;

        var grid = $(this).closest("[data-role=grid]").data("kendoGrid");
        var dataSource = grid.dataSource;
        var filters = dataSource.filter();
        var allData = dataSource.data();
        var query = new kendo.data.Query(allData);
        var filteredData = query.filter(filters).data;

        // La scelta fatta in testata viene estesa ad ogni riga di ogni pagina
        // Di conseguenza 
        $.each(filteredData, function (idx, dataItem) {
            //verifico se la riga è disabilitata
            var chk = $("#" + grid.element[0].id + "_" + dataItem.id);

            if ($(chk).closest('tr').css('pointer-events') != 'none') {

                dataItem.Selected = checked;
                if (dirtyAllRows)
                    dataItem.dirty = dirtyAllRows;
                var row = grid.tbody.find("tr[data-uid='" + dataItem.uid + "']");

                if (checked) {
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

    function onPage(e) {
        //console.log(e.page);
        //$(".k-checkbox").not("#header-chb").each(function (idx, item) {
        //    row = $(this).parents("tr");
        //    if (item.checked) {
        //        //-select the row
        //        if (!row.hasClass(GIAS_K_STATE_SELECTED))
        //            row.addClass(GIAS_K_STATE_SELECTED);
        //    } else {
        //        //-remove selection
        //        if (row.hasClass(GIAS_K_STATE_SELECTED))
        //            row.removeClass(GIAS_K_STATE_SELECTED);
        //    }
        //});
    }

    function onFilterKendoGrid(e) {

        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");

        //if (e.filter == null) {
            //console.log("filter has been cleared");
        //} else {
            //console.log(e.filter.logic);
            //console.log(e.filter.filters[0].field);
            //console.log(e.filter.filters[0].operator);
            //console.log(e.filter.filters[0].value);
        //}
    }


    function onSaveChangesKendoGrid(e) {

        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSaveChangesKendoGrid != null) {
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSaveChangesKendoGrid(e);
        }

        //Gestione delle righe cancellate quando si continuano a mostrare sulla griglia fino al Salva
        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        var data = grid.dataSource.data();
        for (var i = data.length - 1; i >= 0; i--) {
            if (data[i].mostraRigheCancellate && data[i].deleted) {
                grid.dataSource.remove(data[i]);
            }
        }

    }

    function onEditKendoGrid(e) {


        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiEdit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiEdit(e);

        var fieldName;
        var inputs = e.container.find("input");
        for (var i = 0; i < inputs.length; i++) {
            if ($(inputs[i]).attr("name")) {
                fieldName = $(inputs[i]).attr("name");
                break;
            }
        }
        if (fieldName === undefined && this._lastCellIndex !== undefined && this.columns[this._lastCellIndex] !== undefined)
            fieldName = this.columns[this._lastCellIndex].field;
        if (fieldName === undefined && e.container.find("input")[0] !== undefined)
            fieldName = $(e.container.find("input")[0]).attr('data-value-field');

        // Se la riga è marcata come cancellata non permetto più la modifica
        if (e.model.deleted != null && e.model.deleted) {
            this.closeCell(); // prevent editing
        }

        // Gestione di colonne editabili per permettere l'inserimento ma che non devono essere modificabili in modifica
        if (colonneDisabilitateSoloInModifica != null && colonneDisabilitateSoloInModifica.length > 0) {
            if (e.model.isNew() == false) {
                for (i = 0; i < colonneDisabilitateSoloInModifica.length; i++) {

                    if (fieldName !== undefined && fieldName == colonneDisabilitateSoloInModifica[i]) {
                        this.closeCell(); // prevent editing
                        break;
                    }
                }
            }
        }

        if (editPopupMode) {
            $('*[required="required"]').each(function (index) {
                $('label[for="' + this.name + '"]').addClass("campiObbligatori");
            });
        }

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoEdit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoEdit(e);

    }

    function onDataBindingKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBinding != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBinding(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBinding != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBinding(e);
    }

    function onDetailInitKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDetailInit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDetailInit(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDetailInit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoDetailInit(e);
    }

    function onDataBoundKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBound != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBound(e);


        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        var data = grid.dataSource.data();

        // Attach dell'evento in tutti i checkbox di selezione eccetto quello di testata
        //$("#" + gridId + " .k-checkbox").not(".header-chb").click(funzioniCRUD.checkBoxFunction);
        if (funzioniCRUD.checkBoxFunction != null) {
            //sono costretta a fare prima l'off, perché se avevo già fatto il data bound
            // (per esempio sto ricreando la griglia facendo dataSource.read()),
            // finirei per fare l'attach più volte, e quindi anche la funzione passata verrebbe chiamata molteplici volte.
            $("#" + gridId).off("click", ".k-checkbox.checkbox-selectionRow", funzioniCRUD.checkBoxFunction);
            $("#" + gridId).on("click", ".k-checkbox.checkbox-selectionRow", funzioniCRUD.checkBoxFunction);
        }

        // Riapplico lo stile a tutte le righe cancellate
        var rows = e.sender.tbody.children();
        for (var i = 0; i < rows.length; i++) {
            var row = $(rows[i]);
            var dataItem = e.sender.dataItem(row);

            // TODO Stefano
            // Riapplicare lo stile a tutte le celle modificate
            // Sospeso perchè ad oggi non si riescono a trovare le sole celle editate
            ////////if (dataItem.dirty)
            ////////{
            ////////    var cell = row.children()[0];
            ////////    if (!cell.hasClass("k-dirty-cell")) {
            ////////        cell.addClass("k-dirty-cell");
            ////////        cell.prepend("<span class='k-dirty'></span>");
            ////////    }
            ////////}
            if (dataItem == undefined) {
                continue;
            }

            if (dataItem.mostraRigheCancellate) {
                if (dataItem.deleted != null && dataItem.deleted) {
                    if (!row.hasClass("deletedKendoRow"))
                        row.addClass("deletedKendoRow");
                }
                else {
                    if (row.hasClass("deletedKendoRow"))
                        row.removeClass("deletedKendoRow");
                }
            }

            let s = dataItem.id;
            if (dataItem.id === null || dataItem.id === undefined) {
                if (i === 0)
                    console.warn("onDataBoundKendoGrid: La griglia " + gridId + " non ha una chiave definita.\n --> dataItem: ", dataItem);
            } else if (typeof dataItem.id !== 'number') {

                // Provato con tutti i chr speciali, questi sono quelli che danno fastidio
                let specialChars = ["/", "^", "~", "`", "&", "=", "'", ",", ";", "{", "}", "|"];
                for (let i = 0; i < specialChars.length; i++) {
                    if (s.includes(specialChars[i]))
                        s = s.replaceAll(specialChars[i], "\\" + specialChars[i]);
                }

            }
            var chk = $("#" + gridId + "_" + s);
            if (dataItem.Selected !== undefined) {
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
        if (funzioniCRUD.UtenteAbilitatoCancellazione) {
            if (funzioniCRUD.funzioneDelete != null) {
                e.sender.tbody.find(".k-button.fa").each(function (idx, element) {
                    $(element).removeClass("fa fa-trash-o").find("span").addClass("fa fa-trash-o del_elem");
                    var innerContent = $(element).html().replace("Cancella", "");
                    $(element).html(innerContent);
                });
            }
        }

        gestisciPermessiModifica(grid, gridId);

        //se non specificato equivale a true
        if (parametriKendoGrid.impostaColonneKendoGridDaCookie === undefined ||
            parametriKendoGrid.impostaColonneKendoGridDaCookie === null ||
            parametriKendoGrid.impostaColonneKendoGridDaCookie == true) {

            impostaColonneKendoGridDaCookie(grid, gridId);
        }


        ////' VAnni: 3/4/2017: imposto i dati per il grafico..
        //if (parametriKendoGrid.chartCfg !== undefined ||
        //    parametriKendoGrid.chartCfg !== null) {

        //    //divchart, group, category, format, type) {
        //    initChart(
        //        convertData(e.sender.dataSource, collapsed),
        //        parametriKendoGrid.chartCfg.divchart, //"#divKendoChartCMaturazione",
        //        parametriKendoGrid.chartCfg.group, //"column",
        //        parametriKendoGrid.chartCfg.category, //"row",
        //        parametriKendoGrid.chartCfg.format, //"{0}",
        //        parametriKendoGrid.chartCfg.type, //"line"
        //        parametriKendoGrid.chartCfg.sort //"line"
        //    );
        //}

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBound != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBound(e);
    }

    function onExcelExportKendoGrid(e) {

        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiExcelExport != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiExcelExport(e);

        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        var dataSource = grid.dataSource;
        var data = new kendo.data.Query(dataSource.data()).filter(dataSource.filter()).data;

        // Controllo che i campi non inizino con un carattere che potrebbero provocare un formula injection
        var charsVietati = ["=", "+", "-", "@"];

        // Utilizzo e.workbook e non data per considerare solo le colonne visibili
        let trovateRigheFormulaInjection = false;

        if (e.workbook !== undefined && e.workbook.sheets !== undefined) {
            for (let i = 0; i < e.workbook.sheets[0].rows.length; i++) {
                e.workbook.sheets[0].rows[i].cells.forEach(function (valore, campo) {
                    if (!$.isNumeric(valore.value)) {
                        for (let c = 0; c < charsVietati.length; c++) {
                            if (String(valore.value).trimLeft().startsWith(charsVietati[c])) {
                                let indexOfFirst = String(valore.value).indexOf(charsVietati[c]);
                                if (!$.isNumeric(String(valore.value).slice(indexOfFirst + 1).trimLeft().charAt(0))) {
                                    valore.value = "'" + valore.value;
                                    trovateRigheFormulaInjection = true;
                                    break;
                                }
                            }
                        }
                    }
                });
            }
        }

        if (trovateRigheFormulaInjection) {
            let mess = "Sono presenti celle che iniziano con i caratteri = + - @ seguiti da valori non numerici." + "<br/>" +
                "Per motivi di sicurezza in queste celle è stato inserito come prefisso un apice (\')." + "<br/><br/>" +
                "There are cells starting with the characters = + - @ followed by non-numeric values." + "<br/>" +
                "For security reasons a quote(\') has been entered in these cells as a prefix.";
            $("<div></div>").kendoAlert({
                title: "",
                content: mess,
                messages: {
                    okText: "OK"
                }
            }).data("kendoAlert").open();
        }

        if (data.length > 10000) {
            e.preventDefault();
            ExportExcelServer(e);
        }



    }

    function onChangeKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelChange != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelChange(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoChange != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoChange(e);
    }

    function onSaveKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelSave != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelSave(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoSave != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoSave(e);
    }

    function gestisciPermessiModifica(grid, gridId) {
        if (!funzioniCRUD.UtenteAbilitatoInserimentoModifica && !funzioniCRUD.UtenteAbilitatoCancellazione) {
            // QUESTO SE VOGLIO NASCONDERE TUTTA LA TOOLBAR
            //$("#" + gridId + " .k-grid-cancel-changes").parent().hide();

            $("#" + gridId + " .k-grid-cancel-changes").hide();
            $("#" + gridId + " .k-grid-save-changes").hide();
            //$("#" + gridId + " .k-add").parent().hide();
            //$("#" + gridId + " .k-update").parent().hide();
            //$("#" + gridId + " .k-cancel").parent().hide();
            //OR
            //$("#" + gridId + " .k-grid-cancel-changes").remove();
            //$("#" + gridId + " .k-grid-save-changes").remove();
            //$("#" + gridId + " .k-add").parent().remove();
            //$("#" + gridId + " .k-update").parent().remove();
            //$("#" + gridId + " .k-cancel").parent().remove();
        }
    }


    return kendo_grid;

}  // Fine creaKendoGrid ... non spostare prima degli eventi perchè ci sono variabili testate

function creaKendoGridEndlessScrolling(
    // PARAMETRI OBBLIGATORI
    IDControllo,
    funzioniCRUD,
    idModel,
    campiKendoModel,
    colonneKendoGrid,
    // PARAMETRI FACOLTATIVO
    parametriPerLettura, // parametri da passare alla lettura
    parametriDataSource, // parametri data source { chiave - valore}
    parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
    funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi { chiave - valore}:
    //{   
    //    funzioneDaChiamarePrimaDelDataBinding: yyyyy, // funzione da chiamare all'inizio del databinding
    //    funzioneDaChiamareDopoDataBinding: yyyyy, // funzione da chiamare alla fine del databinding
    //    funzioneDaChiamarePrimaDelDataBound: yyyyy, // funzione da chiamare all'inizio del databound
    //    funzioneDaChiamareDopoDataBound: yyyyy, // funzione da chiamare alla fine del databound
    //    funzioneDaChiamarePrimaDelSave: yyyyy, // funzione da chiamare all'inizio del save
    //    funzioneDaChiamareDopoSave: yyyyy // funzione da chiamare alla fine del save
    //    funzioneDaChiamareDopoDelete: yyyyy // funzione da chiamare dopo la cancellazione di una riga 
    //    funzioneDaChiamareDopoAnnulla: yyyyy  // funzione da chiamare quando si annulla una modifica sulla riga
    //}
    mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
    colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
) {

    if (IDControllo == null) {
        alert("Non mi hai passato l'ID del DIV che contiene la griglia");
        return;
    }

    if (funzioniCRUD != null) {
        errFound = false;
        for (var k in funzioniCRUD) {
            if (k != "funzioneRead" &&
                k != "funzioneInsert" &&
                k != "funzioneUpdate" &&
                k != "funzioneDelete" &&
                k != "funzioneSubmit" &&
                k != "checkBoxFunction" &&
                k != "omettiPulsantiSalva" &&
                k != "omettiPulsantiAnnulla" &&
                k != "UtenteAbilitatoInserimentoModifica" &&
                k != "UtenteAbilitatoCancellazione" &&
                k != "gestisciSalvataggioFinaleAParte" &&
                k != "menuColonneAdattivo") {
                errFound = true;
                alert("Fra le funzioni CRUD mi hai passato la chiave " + k + " che non è gestita");
            }
        }
        if (!errFound) {
            // la funzioneSubmit evita/impedisce che vengano chiamate la funzioneInsert, la funzioneDelete e la funzioneUpdate
            // se ho valorizzato la funzioneSubmit verifico che non abbia valorizzato nessuna delle altre (solo per chiarezza)
            if ((funzioniCRUD.funzioneSubmit !== undefined && funzioniCRUD.funzioneSubmit !== null && funzioniCRUD.funzioneSubmit.funzione != null)  //ho valorizzato la funzioneSubmit...
                && (funzioniCRUD.funzioneInsert != null ||
                    funzioniCRUD.funzioneUpdate != null ||
                    funzioniCRUD.funzioneDelete != null)) {

                errFound = true;
                alert("Fra le funzioni CRUD mi hai passato la chiave funzioneSubmit ed una tra funzioneInsert, funzioneUpdate e funzioneDelete");

            }
        }
        if (errFound)
            return;

        // Se il parametro non viene passato si assume che l'utente abbia i permessi
        funzioniCRUD.UtenteAbilitatoInserimentoModifica = (typeof funzioniCRUD.UtenteAbilitatoInserimentoModifica === 'undefined') ? true : funzioniCRUD.UtenteAbilitatoInserimentoModifica;
        funzioniCRUD.UtenteAbilitatoCancellazione = (typeof funzioniCRUD.UtenteAbilitatoCancellazione === 'undefined') ? true : funzioniCRUD.UtenteAbilitatoCancellazione;

        if (funzioniCRUD.funzioneSubmit === undefined) {
            funzioniCRUD.funzioneSubmit = { funzione: null };
        }

        if (funzioniCRUD.funzioneSubmit.funzione != null) {
            funzioniCRUD.funzioneSubmit.flagUpdate = (typeof funzioniCRUD.funzioneSubmit.flagUpdate === 'undefined') ? true : funzioniCRUD.funzioneSubmit.flagUpdate;
            funzioniCRUD.funzioneSubmit.flagInsert = (typeof funzioniCRUD.funzioneSubmit.flagInsert === 'undefined') ? true : funzioniCRUD.funzioneSubmit.flagInsert;
            funzioniCRUD.funzioneSubmit.flagDelete = (typeof funzioniCRUD.funzioneSubmit.flagDelete === 'undefined') ? true : funzioniCRUD.funzioneSubmit.flagDelete;

            if (funzioniCRUD.funzioneSubmit.flagUpdate == false &&
                funzioniCRUD.funzioneSubmit.flagInsert == false &&
                funzioniCRUD.funzioneSubmit.flagDelete == false) {

                alert("Hai passato la funzioneSubmit nelle funzioni CRUD con tutti i flag falsi");
                return;

            }
        }
        else {
            funzioniCRUD.funzioneSubmit.flagUpdate = false;
            funzioniCRUD.funzioneSubmit.flagInsert = false;
            funzioniCRUD.funzioneSubmit.flagDelete = false;
        }

    }

    // Se true non vengono mostrati i pulsanti Salva e Annulla anche se c'è una funzione di Submit o di Insert/Update/Delete
    // Utilizzato quando si vuole fare il salvataggio con un pulsante esterno
    if (funzioniCRUD.omettiPulsantiSalva === undefined) {
        funzioniCRUD.omettiPulsantiSalva = false;
    }

    if (funzioniCRUD.omettiPulsantiAnnulla === undefined) {
        funzioniCRUD.omettiPulsantiAnnulla = false;
    }

    if (idModel == null) {
        alert("Non mi hai passato la chiave della riga della griglia");
        return;
    }

    if (campiKendoModel == null) {
        alert("Non mi hai passato i campi del modello della griglia");
        return;
    }

    if (colonneKendoGrid == null) {
        alert("Non mi hai passato le colonne della griglia");
        return;
    }

    funzioniPrimaDopoEventi = (typeof funzioniPrimaDopoEventi === 'undefined') ? {} : funzioniPrimaDopoEventi;
    errFound = false;
    for (var k in funzioniPrimaDopoEventi) {
        if (k != "funzioneDaChiamarePrimaDelDataBinding" &&
            k != "funzioneDaChiamareDopoDataBinding" &&
            k != "funzioneDaChiamarePrimaDelDataBound" &&
            k != "funzioneDaChiamareDopoDataBound" &&
            k != "funzioneDaChiamarePrimaDelDetailInit" &&
            k != "funzioneDaChiamareDopoDetailInit" &&
            k != "funzioneDaChiamarePrimaDelChange" &&
            k != "funzioneDaChiamareDopoChange" &&
            k != "funzioneDaChiamarePrimaDelSave" &&
            k != "funzioneDaChiamareDopoSave" &&
            k != "funzioneDaChiamarePrimaDiSelectAllRows" &&
            k != "funzioneDaChiamareDopoSelectAllRows" &&
            k != "funzioneDaChiamarePrimaDiEdit" &&
            k != "funzioneDaChiamareDopoEdit" &&
            k != "funzioneDaChiamarePrimaDiSaveChangesKendoGrid" &&
            k != "funzioneDaChiamareDopoDelete" &&
            k != "funzioneDaChiamarePrimaDiExcelExport" &&
            k != "funzioneDaChiamareDopoAnnulla") {
            errFound = true;
            alert("Fra le funzioni da chiamare prima o dopo agli eventi mi hai passato la chiave " + k + " che non è gestita");
        }
    }
    if (errFound)
        return;

    //costruisco il template sull'elemento quando mi è stato passato come id (costruendo la griglia lato server)
    // (necessario perché passando da json non è possibile fare lo store di funzioni, ma solo di stringhe)
    colonneKendoGrid.forEach(function (x) {
        if ((x.templateIdControllo !== undefined && x.templateIdControllo !== null && x.templateIdControllo !== "") &&
            (x.template === undefined || x.template === null || x.template === "")) {
            x.template = kendo.template($("#" + x.templateIdControllo).html());
            delete x.templateIdControllo;
        }
    });

    if (funzioniCRUD.UtenteAbilitatoInserimentoModifica) {
        var submitFunction = null;
        if (funzioniCRUD.funzioneSubmit.funzione != null &&
            (funzioniCRUD.funzioneSubmit.flagInsert || funzioniCRUD.funzioneSubmit.flagUpdate)) {
            submitFunction = (function (options) {
                if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                    options.error();
                }
                funzioniCRUD.funzioneSubmit.funzione(options, parametriPerLettura, parametriDataSource.parametriInsert, parametriDataSource.parametriUpdate);
            });
        }
        else {
            var insertFunction = null;
            if (funzioniCRUD.funzioneInsert != null) {
                insertFunction = (function (options) {
                    if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                        options.error();
                    }
                    funzioniCRUD.funzioneInsert(options, parametriDataSource.parametriInsert);
                });
            }

            var updateFunction = null;
            if (funzioniCRUD.funzioneUpdate != null) {
                updateFunction = (function (options) {
                    if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                        options.error();
                    }
                    funzioniCRUD.funzioneUpdate(options, parametriDataSource.parametriUpdate);
                });
            }
        }
    }

    //qui:
    if (funzioniCRUD.checkBoxFunction != null) {

        $.extend(campiKendoModel, {
            Selected: { type: "boolean", editable: false }
        });
    }

    if (funzioniCRUD.menuColonneAdattivo === undefined) {
        funzioniCRUD.menuColonneAdattivo = true;
    }

    parametriDataSource = (typeof parametriDataSource === 'undefined' || parametriDataSource == null) ? {} : parametriDataSource;
    parametriKendoGrid = (typeof parametriKendoGrid === 'undefined' || parametriKendoGrid == null) ? {} : parametriKendoGrid;
    parametriKendoGrid.groupable = (typeof parametriKendoGrid.groupable === 'undefined') ? true : parametriKendoGrid.groupable;
    parametriKendoGrid.sortable = (typeof parametriKendoGrid.sortable === 'undefined') ? true : parametriKendoGrid.sortable;
    parametriKendoGrid.resizable = (typeof parametriKendoGrid.resizable === 'undefined') ? true : parametriKendoGrid.resizable;
    parametriKendoGrid.filterable = (typeof parametriKendoGrid.filterable === 'undefined') ? true : parametriKendoGrid.filterable;
    parametriKendoGrid.editable = (typeof parametriKendoGrid.editable === 'undefined') ? true : parametriKendoGrid.editable;
    parametriKendoGrid.excel = (typeof parametriKendoGrid.excel === 'undefined') ? true : parametriKendoGrid.excel;
    parametriKendoGrid.pdf = (typeof parametriKendoGrid.pdf === 'undefined') ? true : parametriKendoGrid.pdf;
    parametriKendoGrid.search = (typeof parametriKendoGrid.search === 'undefined') ? false : parametriKendoGrid.search;
    parametriKendoGrid.pdfTemplate = (typeof parametriKendoGrid.pdfTemplate === 'undefined') ? { allPages: true } : parametriKendoGrid.pdfTemplate;
    parametriKendoGrid.columnMenu = (typeof parametriKendoGrid.columnMenu === 'undefined') ? true : parametriKendoGrid.columnMenu;
    parametriKendoGrid.toolbarCommands = (typeof parametriKendoGrid.toolbarCommands === 'undefined') ? null : parametriKendoGrid.toolbarCommands;
    parametriKendoGrid.height = (typeof parametriKendoGrid.height === 'undefined') ? null : parametriKendoGrid.height;
    parametriKendoGrid.colonneCustomKendoGrid = (typeof parametriKendoGrid.colonneCustomKendoGrid === 'undefined') ? null : parametriKendoGrid.colonneCustomKendoGrid;
    parametriKendoGrid.reorderable = (typeof parametriKendoGrid.reorderable === 'undefined') ? null : parametriKendoGrid.reorderable;
    parametriKendoGrid.rowTemplate = (typeof parametriKendoGrid.rowTemplate === 'undefined') ? null : parametriKendoGrid.rowTemplate;
    parametriKendoGrid.checkSelezioneRiga = (typeof parametriKendoGrid.checkSelezioneRiga === 'undefined') ? null : parametriKendoGrid.checkSelezioneRiga;
    parametriKendoGrid.salvaRipristinaPersonalizzazioni = (typeof parametriKendoGrid.salvaRipristinaPersonalizzazioni === 'undefined') ? false : parametriKendoGrid.salvaRipristinaPersonalizzazioni;
    parametriKendoGrid.selectable = (typeof parametriKendoGrid.selectable === 'undefined') ? false : parametriKendoGrid.selectable;
    parametriKendoGrid.btnEliminaTuttiFiltri = (typeof parametriKendoGrid.btnEliminaTuttiFiltri === 'undefined') ? true : parametriKendoGrid.btnEliminaTuttiFiltri;
    parametriKendoGrid.lockCancella = (typeof parametriKendoGrid.lockCancella === 'undefined') ? false : parametriKendoGrid.lockCancella;

    var editPopupMode = false;
    var editInlineMode = false;
    editPopupMode = (parametriKendoGrid.editable !== false &&
        (parametriKendoGrid.editable === "popup" ||
            (parametriKendoGrid.editable.mode !== undefined && parametriKendoGrid.editable.mode === "popup")));
    editInlineMode = (parametriKendoGrid.editable !== false &&
        (parametriKendoGrid.editable === "inline" ||
            (parametriKendoGrid.editable.mode !== undefined && parametriKendoGrid.editable.mode === "inline")));

    parametriDataSource.pagesize = (typeof parametriDataSource.pagesize === 'undefined') ? 10 : parametriDataSource.pagesize;

    // Pageable Tolto
    // Se non c'è paginazione il pagesize viene impostato altissimo (se si imposta a zero non funziona bene il filtro)
    //if (!parametriKendoGrid.pageable)
    //    parametriDataSource.pagesize = 99999999;

    parametriDataSource.aggregate = (typeof parametriDataSource.aggregate === 'undefined') ? null : parametriDataSource.aggregate;
    parametriDataSource.sort = (typeof parametriDataSource.sort === 'undefined') ? null : parametriDataSource.sort;

    // Aggiunta colonne Custom
    if (parametriKendoGrid.colonneCustomKendoGrid != null) {
        //vado all'indietro perchè faccio l'unshift
        for (i = parametriKendoGrid.colonneCustomKendoGrid.length - 1; i >= 0; i--) {
            // Se l'utente non ha permessi di inserimento/modifica e cancellazione elimino il pulsante
            for (var x = parametriKendoGrid.colonneCustomKendoGrid[i].command.length - 1; x >= 0; x--) {
                if (!funzioniCRUD.UtenteAbilitatoInserimentoModifica &&
                    parametriKendoGrid.colonneCustomKendoGrid[i].command[x].name == "edit")
                    parametriKendoGrid.colonneCustomKendoGrid[i].command.splice(x, 1);
                else
                    if (!funzioniCRUD.UtenteAbilitatoCancellazione &&
                        parametriKendoGrid.colonneCustomKendoGrid[i].command[x].name == "destroy")
                        parametriKendoGrid.colonneCustomKendoGrid[i].command.splice(x, 1);
            }

            if ((parametriKendoGrid.colonneCustomKendoGrid[i].command.length !== undefined && parametriKendoGrid.colonneCustomKendoGrid[i].command.length > 0) ||
                parametriKendoGrid.colonneCustomKendoGrid[i].command !== undefined && parametriKendoGrid.colonneCustomKendoGrid[i].command.length === undefined)
                colonneKendoGrid.unshift(parametriKendoGrid.colonneCustomKendoGrid[i]);
        }

    }

    var checkBoxFunction = null;
    var dirtyAllRows = true;
    if (funzioniCRUD.checkBoxFunction != null) {

        var width = 50;
        var sortable = false;
        var filterable = true;
        var title = 'Seleziona';
        var locked = false;
        var field = "Selected";

        if (parametriKendoGrid.checkSelezioneRiga) {
            width = (typeof parametriKendoGrid.checkSelezioneRiga.width === 'undefined') ? width : parametriKendoGrid.checkSelezioneRiga.width;
            sortable = (typeof parametriKendoGrid.checkSelezioneRiga.sortable === 'undefined') ? sortable : parametriKendoGrid.checkSelezioneRiga.sortable;
            filterable = (typeof parametriKendoGrid.checkSelezioneRiga.filterable === 'undefined') ? filterable : parametriKendoGrid.checkSelezioneRiga.filterable;
            title = (typeof parametriKendoGrid.checkSelezioneRiga.title === 'undefined') ? title : parametriKendoGrid.checkSelezioneRiga.title;
            locked = (typeof parametriKendoGrid.checkSelezioneRiga.locked === 'undefined') ? locked : parametriKendoGrid.checkSelezioneRiga.locked;
            field = (typeof parametriKendoGrid.checkSelezioneRiga.field === 'undefined') ? field : parametriKendoGrid.checkSelezioneRiga.field;
            dirtyAllRows = (typeof parametriKendoGrid.checkSelezioneRiga.dirtyAllRows === 'undefined') ? dirtyAllRows : parametriKendoGrid.checkSelezioneRiga.dirtyAllRows;
        }

        var colSelected = {
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
                //return "<input type=\"checkbox\" id=\"" + dataItem.id + "\" #= Selected ? \'checked=\"checked\"\' : \"\" class=\"k-checkbox\"><label style='margin-right:0px;' class=\"k-checkbox-label\" for=\"" + dataItem.id + "\"></label>"

                var m = "<input type=\"checkbox\" id=\"" + IDControllo + "_" + dataItem.id + "\" #= Selected ? checked=\"checked\" : \"\" # class=\"k-checkbox checkbox-selectionRow\"><label style=\"margin-right:0px;\" class=\"k-checkbox-label\" for=\"" + IDControllo + "_" + dataItem.id + "\"></label>";
                //console.debug(m);
                return m;
            }
        };
        if (parametriKendoGrid.headerAttributes !== undefined)
            colSelected.headerAttributes = parametriKendoGrid.headerAttributes;
        colonneKendoGrid.unshift(colSelected);
    }

    if (funzioniCRUD.UtenteAbilitatoCancellazione) {

        var gestisciCancellazione = false;

        if (funzioniCRUD.funzioneSubmit.funzione != null && funzioniCRUD.funzioneSubmit.flagDelete) {
            if (submitFunction == null) {
                submitFunction = (function (options) {
                    if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                        options.error();
                    }
                    funzioniCRUD.funzioneSubmit.funzione(options);
                });
            }

            gestisciCancellazione = true;
        }
        else {
            var deleteFunction = null;
            if (funzioniCRUD.funzioneDelete != null) {
                deleteFunction = (function (options) {
                    if (!(funzioniCRUD.gestisciSalvataggioFinaleAParte === true)) {
                        options.error();
                    }
                    funzioniCRUD.funzioneDelete(options);
                });

                gestisciCancellazione = true;
            }
        }

        if (gestisciCancellazione) {
            // Caso in cui si vogliano mostrare le righe cancellate (testo rosso e barrato tramite css)
            if (mostraRigheCancellate) {

                let colAnnulla = {
                    title: "Cancella",
                    width: 100,
                    locked: parametriKendoGrid.lockCancella,
                    command: [{
                        iconClass: "fa fa-trash-o",
                        className: "btn-Cancella",
                        name: "Cancella",
                        text: "&nbsp",
                        click: function (e) {
                            var grid = $("#" + IDControllo).getKendoGrid();
                            var row = $(e.target).closest("tr");
                            dataItem = grid.dataItem(row);
                            dataItem.mostraRigheCancellate = true;
                            if (dataItem.deleted != null && dataItem.deleted) {
                                dataItem.deleted = false;
                                row.removeClass("deletedKendoRow");
                            }
                            else {
                                dataItem.deleted = true;
                                row.addClass("deletedKendoRow");
                            }

                            if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDelete != null) {
                                funzioniPrimaDopoEventi.funzioneDaChiamareDopoDelete(e);
                            }

                            e.preventDefault(e);

                        }
                    }]
                };
                if (parametriKendoGrid.headerAttributes !== undefined)
                    colAnnulla.headerAttributes = parametriKendoGrid.headerAttributes;

                colonneKendoGrid.unshift(colAnnulla);

            }
            else {

                if (!editPopupMode && !editInlineMode) {
                    //Aggiungo la colonna con il pulsante di cancellazione all'inizio della griglia
                    let colAnnulla = { command: [{ name: "destroy", template: "<span class='fa fa-trash-o k-grid-delete'></span>" }], title: "Cancella" };
                    if (parametriKendoGrid.headerAttributes !== undefined)
                        colAnnulla.headerAttributes = parametriKendoGrid.headerAttributes;

                    colonneKendoGrid.unshift(colAnnulla);
                }

            }

        }
    }

    // Sembra non più necessario dalla versione 2017
    if (parametriKendoGrid.filterable) {
        if (parametriKendoGrid.filterable == true) {
            parametriKendoGrid.filterable = {
                messages: {
                    clear: "Pulisci",
                    filter: "Applica",
                    isFalse: "No",
                    isTrue: "Sì",
                    checkAll: "Seleziona tutto",
                    selectedItemsFormat: "{0} elementi selezionati"
                }
            };
        }
        else {
            parametriKendoGrid.filterable['messages'] =
            {
                clear: "Pulisci",
                filter: "Applica",
                isFalse: "No",
                isTrue: "Sì",
                checkAll: "Seleziona tutto",
                selectedItemsFormat: "{0} elementi selezionati"
            };
        }
    }

    if (parametriKendoGrid.columnMenu) {
        parametriKendoGrid.columnMenu = {
            filterable: true,
            sortable: false,
            columns: true
        };
    }

    if (parametriKendoGrid.sortable) {
        parametriKendoGrid.sortable = {
            mode: "multiple"
        };
    }

    // toolbar
    var toolbar = [];

    if (parametriKendoGrid.excel)
        toolbar.push({ name: "excel", text: "" });

    if (parametriKendoGrid.pdf) {
        toolbar.push({ name: "pdf", text: "" });
    }

    //Se impostato aggiungo il pulsante per salvare le personalizzazioni della griglia
    if (parametriKendoGrid.salvaRipristinaPersonalizzazioni != false)
        toolbar.push({
            name: 'salvaPersGrigliaKendo', template: kendo.template('<div class="grid-customization">' +
                '<div class="k-button btnSalvaPersonalizzazioniGriglia" onclick="salvaPersonalizzazioniGrigliaKendo(&quot;' + parametriKendoGrid.salvaRipristinaPersonalizzazioni.url + '&quot;, &quot;' + location.pathname + '&quot;, &quot;' + IDControllo + '&quot;, true)" ' +
                '>' +
                '<span class="k-icon k-i-save" style="font-size: 17px;"></span ></div>' +
                '<div class="k-button btnCancellaPersonalizzazioniGrigliaKendo ' + GIAS_K_STATE_DISABLED + ' cancellaPersonalizzazioniGrigliaKendo" onclick="cancellaPersonalizzazioniGrigliaKendo(&quot;' + parametriKendoGrid.salvaRipristinaPersonalizzazioni.url + '&quot;, &quot;' + location.pathname + '&quot;, &quot;' + IDControllo + '&quot;, true)" ' +
                '>' +
                '<span class="k-icon k-i-delete" style="margin:0px; font-size:17px;"></span></div>' +
                '</div >')
        });

    if (parametriKendoGrid.btnEliminaTuttiFiltri != false) {

        if ((typeof GiasVersioneMaster != 'undefined') && (GiasVersioneMaster === "2022")) {
            toolbar.push({ name: 'btnEliminaTuttiFiltri', template: kendo.template('<div class="k-button xonne-btn-filters" id="btnEliminaTuttiFiltri" onclick="btnEliminaTuttiFiltri(&quot;' + IDControllo + '&quot;)"><span class="k-icon k-i-filter-clear "></span></div>') });
        }
        else {
            toolbar.push({ name: 'btnEliminaTuttiFiltri', template: kendo.template('<div class="k-button btnEliminaTuttiFiltri" id="btnEliminaTuttiFiltri" onclick="btnEliminaTuttiFiltri(&quot;' + IDControllo + '&quot;)"><span class="k-icon k-i-filter-clear "></span>&nbsp;Pulisci Filtri</div>') });
        }
    }
    if (parametriKendoGrid.toolbarCommands != null) {
        for (i = 0; i < parametriKendoGrid.toolbarCommands.length; i++) {
            var myID = "#" + parametriKendoGrid.toolbarCommands[i];
            toolbar.push({ template: kendo.template($(myID).html()) });
        }
    }

    // pulsanti impostati solo se vengono passate le relative funzioni
    if (!editPopupMode && !editInlineMode && (
        ((funzioniCRUD.funzioneInsert != null || funzioniCRUD.funzioneUpdate != null || funzioniCRUD.funzioneSubmit.funzione != null) && funzioniCRUD.UtenteAbilitatoInserimentoModifica) ||
        ((funzioniCRUD.funzioneDelete != null || funzioniCRUD.funzioneSubmit.funzione != null) && funzioniCRUD.UtenteAbilitatoCancellazione)
    )
    ) {
        if (funzioniCRUD.omettiPulsantiSalva === false) {
            toolbar.push("save");

        }
        if (funzioniCRUD.omettiPulsantiAnnulla === false) {
            toolbar.push("cancel");
        }
    }
    if ((funzioniCRUD.funzioneInsert != null || (funzioniCRUD.funzioneSubmit.funzione != null && funzioniCRUD.funzioneSubmit.flagInsert))
        && funzioniCRUD.UtenteAbilitatoInserimentoModifica) {
        toolbar.push("create");
    }

    if (parametriKendoGrid.search) {
        toolbar.push("search");
    }

    // Cambio il testo di campo obbligatorio nek caso non si sia specificato
    msgRequired(campiKendoModel);

    //Se impostato ripristino le personalizzazioni della griglia
    var jsonDaRipristinare = null;
    var pagina = location.pathname;

    if (parametriKendoGrid.salvaRipristinaPersonalizzazioni != false) {
        jsonDaRipristinare = ripristinaPersonalizzazioniGrigliaKendo(parametriKendoGrid.salvaRipristinaPersonalizzazioni.url, location.pathname, IDControllo);

        if (jsonDaRipristinare !== null && jsonDaRipristinare !== "")
            parametriDataSource = setPersonalizzazioniGrigliaKendoDati(pagina, IDControllo, jsonDaRipristinare, parametriKendoGrid, parametriDataSource, campiKendoModel, colonneKendoGrid);


    }

    var dataSourceKendoGrid = new kendo.data.DataSource({
        data: parametriDataSource.data,
        pageSize: parametriDataSource.pagesize,
        schema: {
            model: {
                id: idModel,
                fields: campiKendoModel
            }
        },
        aggregate: parametriDataSource.aggregate,
        group: parametriDataSource.group,
        filter: parametriDataSource.filter,
        sort: parametriDataSource.sort
    });

    var originalMouseLeave = kendo.ui.Menu.fn._mouseleave;
    var mouseLeave = function (e) {
        var that = this;
        clearTimeout(this._timeoutHandle);
        this._timeoutHandle = setTimeout(function () {
            originalMouseLeave.call(that, e);
        }, 1000);
    };

    kendo.ui.Menu.fn._mouseleave = mouseLeave; // function() {};

    var originalMouseEnter = kendo.ui.Menu.fn._mouseenter;
    var mouseEnter = function (e) {
        clearTimeout(this._timeoutHandle);
        originalMouseEnter.call(this, e);
    };

    var grid = $("#" + IDControllo).data("kendoGrid");
    if (grid != null) {
        grid.destroy();
        $("#" + IDControllo).empty();
    }
    kendo.ui.Menu.fn._mouseenter = mouseEnter;

    var kendo_grid = $("#" + IDControllo).kendoGrid(
        {
            dataSource: dataSourceKendoGrid,
            groupable: parametriKendoGrid.groupable,
            scrollable: { endless: true },
            sortable: parametriKendoGrid.sortable,
            resizable: parametriKendoGrid.resizable,
            reorderable: parametriKendoGrid.reorderable,
            selectable: parametriKendoGrid.selectable,
            rowTemplate: parametriKendoGrid.rowTemplate,
            filterable: parametriKendoGrid.filterable,
            pageable: {
                numeric: false,
                previousNext: false,
            },
            height: parametriKendoGrid.height,
            toolbar: toolbar,
            dataBinding: onDataBindingKendoGrid,
            dataBound: onDataBoundKendoGrid,
            detailInit: funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDetailInit === undefined &&
                funzioniPrimaDopoEventi.funzioneDaChiamareDopoDetailInit === undefined ? null : onDetailInitKendoGrid,
            edit: onEditKendoGrid,
            cancel: funzioniPrimaDopoEventi.funzioneDaChiamareDopoAnnulla,
            save: onSaveKendoGrid,
            change: onChangeKendoGrid,
            saveChanges: onSaveChangesKendoGrid,
            filter: onFilterKendoGrid,
            columns: colonneKendoGrid,
            columnMenu: parametriKendoGrid.columnMenu,
            //columnHide: onColumnHideKendoGrid,
            //columnShow: onColumnShowKendoGrid,
            editable: parametriKendoGrid.editable,
            navigatable: true,
            //mobile: true,
            filterMenuInit: onFilterMenuInit,
            columnMenuInit: onColumnMenuInit,
            columnMenuOpen: funzioniCRUD.menuColonneAdattivo === false ? null : onColumnMenuOpen,
            noRecords: {
                template: "Non sono presenti dati"
            },
            excel: {
                allPages: true,
                filterable: true
            },
            excelExport: onExcelExportKendoGrid,
            pdf: parametriKendoGrid.pdfTemplate,
            page: onPage
        });
    // TODO Stefano aggiungere per gestione tab (vedi in fondo) .find("table").on("keydown", onGridKeydown);

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-cancel-changes",
        position: "top",
        content: "Annulla modifiche"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-add",
        position: "top",
        content: "Aggiungi riga"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-excel",
        position: "top",
        content: "Esporta in Excel"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-Info",
        position: "top",
        content: "Informazioni"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-Modifica",
        position: "top",
        content: "Modifica"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-pdf",
        position: "top",
        content: "Esporta in PDF"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".btnSalvaPersonalizzazioniGriglia",
        position: "top",
        content: "Salva personalizzazione"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".btnCancellaPersonalizzazioniGrigliaKendo",
        position: "top",
        content: "Cancella personalizzazione"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid-Cancella",
        position: "top",
        content: "Elimina"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".elimina-selezionati",
        position: "top",
        content: "Elimina selezionati"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".importa-selezionati",
        position: "top",
        content: "Importa selezionati"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".controlla-selezionati",
        position: "top",
        content: "Controlla selezionati"
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".xonne-btn-filters",
        position: "top",
        content: "Pulisci filtri"
    }).data("kendoTooltip");

    let filterExcludeButton = ":not(.k-grid-cancel-changes):not(.k-grid-add):not(.k-grid-excel):not(.k-grid-Info):not(.k-grid-Modifica):not(.k-grid-pdf):not(.btnSalvaPersonalizzazioniGriglia):not(.btnCancellaPersonalizzazioniGrigliaKendo):not(.k-grid-Cancella)";

    //Tooltip su bottoni che hanno un data-title
    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid--button[data-title]" + filterExcludeButton,
        position: "top",
        content: (e) => e.target[0].dataset.title
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-button[data-title]" + filterExcludeButton,
        position: "top",
        content: (e) => e.target[0].dataset.title
    }).data("kendoTooltip");

    //Tooltip su bottoni che hanno un title (escludo quelli che hanno anche un data-title, perché sono ricompresi nei casi sopra)
    $("#" + IDControllo).kendoTooltip({
        filter: ".k-grid--button[title]" + filterExcludeButton,
        position: "top",
    }).data("kendoTooltip");

    $("#" + IDControllo).kendoTooltip({
        filter: ".k-button[title]:not([data-title])" + filterExcludeButton,
        position: "top",
    }).data("kendoTooltip");

    if (jsonDaRipristinare !== null && jsonDaRipristinare !== "") {
        setPersonalizzazioniGrigliaKendoColonne(pagina, IDControllo, jsonDaRipristinare);

        //Se esiste una personalizzazione attivo il pulsante con il cestino (sempre che esista anche il pulsante)
        if ($("#" + IDControllo + " .cancellaPersonalizzazioniGrigliaKendo").length > 0)
            $("#" + IDControllo + " .cancellaPersonalizzazioniGrigliaKendo")[0].classList.remove(GIAS_K_STATE_DISABLED);
    }


    var idEventoSelect = "#" + IDControllo + "-header-chb";

    //evento "seleziona tutte le righe"
    $(idEventoSelect).change(function (ev) {

        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSelectAllRows != null) {
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSelectAllRows(ev);
        }

        var checked = ev.target.checked;

        var grid = $(this).closest("[data-role=grid]").data("kendoGrid");
        var dataSource = grid.dataSource;
        var filters = dataSource.filter();
        var allData = dataSource.data();
        var query = new kendo.data.Query(allData);
        var filteredData = query.filter(filters).data;

        // La scelta fatta in testata viene estesa ad ogni riga di ogni pagina
        // Di conseguenza 
        $.each(filteredData, function (idx, dataItem) {
            //verifico se la riga è disabilitata
            var chk = $("#" + grid.element[0].id + "_" + dataItem.id);

            if ($(chk).closest('tr').css('pointer-events') != 'none') {

                dataItem.Selected = checked;
                if (dirtyAllRows)
                    dataItem.dirty = dirtyAllRows;
                var row = grid.tbody.find("tr[data-uid='" + dataItem.uid + "']");

                if (checked) {
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

    function onPage(e) {
    }

    function onFilterKendoGrid(e) {

        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");

        //if (e.filter == null) {
            //console.log("filter has been cleared");
        //} else {
            //console.log(e.filter.logic);
            //console.log(e.filter.filters[0].field);
            //console.log(e.filter.filters[0].operator);
            //console.log(e.filter.filters[0].value);
        //}
    }


    function onSaveChangesKendoGrid(e) {

        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSaveChangesKendoGrid != null) {
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiSaveChangesKendoGrid(e);
        }

        //Gestione delle righe cancellate quando si continuano a mostrare sulla griglia fino al Salva
        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        var data = grid.dataSource.data();
        for (var i = data.length - 1; i >= 0; i--) {
            if (data[i].mostraRigheCancellate && data[i].deleted) {
                grid.dataSource.remove(data[i]);
            }
        }

    }

    function onEditKendoGrid(e) {

        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiEdit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiEdit(e);

        var fieldName;
        var inputs = e.container.find("input");
        for (var i = 0; i < inputs.length; i++) {
            if ($(inputs[i]).attr("name")) {
                fieldName = $(inputs[i]).attr("name");
                break;
            }
        }
        if (fieldName === undefined && this._lastCellIndex !== undefined && this.columns[this._lastCellIndex] !== undefined)
            fieldName = this.columns[this._lastCellIndex].field;
        if (fieldName === undefined && e.container.find("input")[0] !== undefined)
            fieldName = $(e.container.find("input")[0]).attr('data-value-field');

        // Se la riga è marcata come cancellata non permetto più la modifica
        if (e.model.deleted != null && e.model.deleted) {
            this.closeCell(); // prevent editing
        }

        // Gestione di colonne editabili per permettere l'inserimento ma che non devono essere modificabili in modifica
        if (colonneDisabilitateSoloInModifica != null && colonneDisabilitateSoloInModifica.length > 0) {
            if (e.model.isNew() == false) {
                for (i = 0; i < colonneDisabilitateSoloInModifica.length; i++) {

                    if (fieldName !== undefined && fieldName == colonneDisabilitateSoloInModifica[i]) {
                        this.closeCell(); // prevent editing
                        break;
                    }
                }
            }
        }

        if (editPopupMode) {
            $('*[required="required"]').each(function (index) {
                $('label[for="' + this.name + '"]').addClass("campiObbligatori");
            });
        }

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoEdit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoEdit(e);

    }

    function onDataBindingKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBinding != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBinding(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBinding != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBinding(e);
    }

    function onDetailInitKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDetailInit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDetailInit(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDetailInit != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoDetailInit(e);
    }

    function onDataBoundKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBound != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelDataBound(e);


        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        var data = grid.dataSource.data();

        // Attach dell'evento in tutti i checkbox di selezione eccetto quello di testata
        //$("#" + gridId + " .k-checkbox").not(".header-chb").click(funzioniCRUD.checkBoxFunction);
        if (funzioniCRUD.checkBoxFunction != null) {
            //sono costretta a fare prima l'off, perché se avevo già fatto il data bound
            // (per esempio sto ricreando la griglia facendo dataSource.read()),
            // finirei per fare l'attach più volte, e quindi anche la funzione passata verrebbe chiamata molteplici volte.
            $("#" + gridId).off("click", ".k-checkbox.checkbox-selectionRow", funzioniCRUD.checkBoxFunction);
            $("#" + gridId).on("click", ".k-checkbox.checkbox-selectionRow", funzioniCRUD.checkBoxFunction);
        }

        // Riapplico lo stile a tutte le righe cancellate
        var rows = e.sender.tbody.children();
        for (var i = 0; i < rows.length; i++) {
            var row = $(rows[i]);
            var dataItem = e.sender.dataItem(row);

            // TODO Stefano
            // Riapplicare lo stile a tutte le celle modificate
            // Sospeso perchè ad oggi non si riescono a trovare le sole celle editate
            ////////if (dataItem.dirty)
            ////////{
            ////////    var cell = row.children()[0];
            ////////    if (!cell.hasClass("k-dirty-cell")) {
            ////////        cell.addClass("k-dirty-cell");
            ////////        cell.prepend("<span class='k-dirty'></span>");
            ////////    }
            ////////}
            if (dataItem.mostraRigheCancellate) {
                if (dataItem.deleted != null && dataItem.deleted) {
                    if (!row.hasClass("deletedKendoRow"))
                        row.addClass("deletedKendoRow");
                }
                else {
                    if (row.hasClass("deletedKendoRow"))
                        row.removeClass("deletedKendoRow");
                }
            }

            let s = dataItem.id;
            if (dataItem.id === null || dataItem.id === undefined) {
                if (i === 0)
                    console.warn("onDataBoundKendoGrid: La griglia " + gridId + " non ha una chiave definita.\n --> dataItem: ", dataItem);
            } else if (typeof dataItem.id !== 'number') {

                // Provato con tutti i chr speciali, questi sono quelli che danno fastidio
                let specialChars = ["/", "^", "~", "`", "&", "=", "'", ",", ";", "{", "}", "|"];
                for (let i = 0; i < specialChars.length; i++) {
                    if (s.includes(specialChars[i]))
                        s = s.replaceAll(specialChars[i], "\\" + specialChars[i]);
                }

            }
            var chk = $("#" + gridId + "_" + s);
            if (dataItem.Selected !== undefined) {
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
        if (funzioniCRUD.UtenteAbilitatoCancellazione) {
            if (funzioniCRUD.funzioneDelete != null) {
                e.sender.tbody.find(".k-button.fa").each(function (idx, element) {
                    $(element).removeClass("fa fa-trash-o").find("span").addClass("fa fa-trash-o del_elem");
                    var innerContent = $(element).html().replace("Cancella", "");
                    $(element).html(innerContent);
                });
            }
        }

        gestisciPermessiModifica(grid, gridId);

        //se non specificato equivale a true
        if (parametriKendoGrid.impostaColonneKendoGridDaCookie === undefined ||
            parametriKendoGrid.impostaColonneKendoGridDaCookie === null ||
            parametriKendoGrid.impostaColonneKendoGridDaCookie == true) {

            impostaColonneKendoGridDaCookie(grid, gridId);
        }


        ////' VAnni: 3/4/2017: imposto i dati per il grafico..
        //if (parametriKendoGrid.chartCfg !== undefined ||
        //    parametriKendoGrid.chartCfg !== null) {

        //    //divchart, group, category, format, type) {
        //    initChart(
        //        convertData(e.sender.dataSource, collapsed),
        //        parametriKendoGrid.chartCfg.divchart, //"#divKendoChartCMaturazione",
        //        parametriKendoGrid.chartCfg.group, //"column",
        //        parametriKendoGrid.chartCfg.category, //"row",
        //        parametriKendoGrid.chartCfg.format, //"{0}",
        //        parametriKendoGrid.chartCfg.type, //"line"
        //        parametriKendoGrid.chartCfg.sort //"line"
        //    );
        //}

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBound != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoDataBound(e);
    }

    function onExcelExportKendoGrid(e) {

        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiExcelExport != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDiExcelExport(e);

        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        var dataSource = grid.dataSource;
        var data = new kendo.data.Query(dataSource.data()).filter(dataSource.filter()).data;

        // Controllo che i campi non inizino con un carattere che potrebbero provocare un formula injection
        var charsVietati = ["=", "+", "-", "@"];

        // Utilizzo e.workbook e non data per considerare solo le colonne visibili
        let trovateRigheFormulaInjection = false;

        if (e.workbook !== undefined && e.workbook.sheets !== undefined) {
            for (let i = 0; i < e.workbook.sheets[0].rows.length; i++) {
                e.workbook.sheets[0].rows[i].cells.forEach(function (valore, campo) {
                    if (!$.isNumeric(valore.value)) {
                        for (let c = 0; c < charsVietati.length; c++) {
                            if (String(valore.value).trimLeft().startsWith(charsVietati[c])) {
                                let indexOfFirst = String(valore.value).indexOf(charsVietati[c]);
                                if (!$.isNumeric(String(valore.value).slice(indexOfFirst + 1).trimLeft().charAt(0))) {
                                    valore.value = "'" + valore.value;
                                    trovateRigheFormulaInjection = true;
                                    break;
                                }
                            }
                        }
                    }
                });
            }
        }

        if (trovateRigheFormulaInjection) {
            let mess = "Sono presenti celle che iniziano con i caratteri = + - @ seguiti da valori non numerici." + "<br/>" +
                "Per motivi di sicurezza in queste celle è stato inserito come prefisso un apice (\')." + "<br/><br/>" +
                "There are cells starting with the characters = + - @ followed by non-numeric values." + "<br/>" +
                "For security reasons a quote(\') has been entered in these cells as a prefix.";
            $("<div></div>").kendoAlert({
                title: "",
                content: mess,
                messages: {
                    okText: "OK"
                }
            }).data("kendoAlert").open();
        }

        if (data.length > 10000) {
            e.preventDefault();
            ExportExcelServer(e);
        }



    }

    function onChangeKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelChange != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelChange(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoChange != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoChange(e);
    }

    function onSaveKendoGrid(e) {
        if (funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelSave != null)
            funzioniPrimaDopoEventi.funzioneDaChiamarePrimaDelSave(e);

        if (funzioniPrimaDopoEventi.funzioneDaChiamareDopoSave != null)
            funzioniPrimaDopoEventi.funzioneDaChiamareDopoSave(e);
    }

    function gestisciPermessiModifica(grid, gridId) {
        if (!funzioniCRUD.UtenteAbilitatoInserimentoModifica && !funzioniCRUD.UtenteAbilitatoCancellazione) {
            // QUESTO SE VOGLIO NASCONDERE TUTTA LA TOOLBAR
            //$("#" + gridId + " .k-grid-cancel-changes").parent().hide();

            $("#" + gridId + " .k-grid-cancel-changes").hide();
            $("#" + gridId + " .k-grid-save-changes").hide();
            //$("#" + gridId + " .k-add").parent().hide();
            //$("#" + gridId + " .k-update").parent().hide();
            //$("#" + gridId + " .k-cancel").parent().hide();
            //OR
            //$("#" + gridId + " .k-grid-cancel-changes").remove();
            //$("#" + gridId + " .k-grid-save-changes").remove();
            //$("#" + gridId + " .k-add").parent().remove();
            //$("#" + gridId + " .k-update").parent().remove();
            //$("#" + gridId + " .k-cancel").parent().remove();
        }
    }


    return kendo_grid;

}  // Fine creaKendoGridEndlessScrolling ... non spostare prima degli eventi perchè ci sono variabili testate


function onColumnHideKendoGrid(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");

    var date = new Date();
    var m = 60;
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));

    var columnsHidden = $.cookie(gridId + "Grid");
    if (columnsHidden == null)
        columnsHidden = "";
    if (columnsHidden.indexOf(e.column.field + ";") == -1) {
        columnsHidden += e.column.field + ";";
        $.cookie(gridId + "Grid", columnsHidden, { expires: date });
    }
}

function onColumnShowKendoGrid(e) {
    var gridId = e.sender.element[0].id;

    var date = new Date();
    var m = 60;
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));

    var columnsHidden = $.cookie(gridId + "Grid");
    if (columnsHidden == null)
        columnsHidden = "";
    if (columnsHidden.indexOf(e.column.field + ";") !== -1) {
        columnsHidden = columnsHidden.replace(e.column.field + ";", "");
        $.cookie(gridId + "Grid", columnsHidden, { expires: date });
    }
}

function onFilterMenuInit(e) {

    // In caso di aggiunta della colonna Selected solo lato client (quindi senza che il modello sia costruito
    // da dati letti da DB, questa contiene undefined (il defaultValue sul model sembra non funzionare)
    // Di conseguenza il filtro per avere le sole righe non selezionate non funzionerebbe
    // Questo evento viene lanciato solo la prima volta che si clicca sul campo di filtro e quindi si può
    // impostare false dove è undefined
    if (e.field == "Selected") {
        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        var data = grid.dataSource.data();
        for (var i = data.length - 1; i >= 0; i--) {
            if (data[i].Selected == undefined) {
                data[i].Selected = false;
            }
        }
    }

    //// Sort del filtro
    //var filterMultiCheck = this.thead.find("[data-field=" + e.field + "]").data("kendoFilterMultiCheck")
    //filterMultiCheck.container.empty();
    //filterMultiCheck.checkSource.sort({ field: e.field, dir: "asc" });
    //// uncomment the following line to handle any grouping from the original dataSource:
    //// filterMultiCheck.checkSource.group(null);
    //filterMultiCheck.checkSource.data(filterMultiCheck.checkSource.view().toJSON());
    //filterMultiCheck.createCheckBoxes();

}

function onColumnMenuOpen(e) {
    try {

        var columnsCount = e.sender.columns.length;
        var ulColumns = Math.ceil(columnsCount / 25);

        var menu = $(e.container.children()[0]).data("kendoMenu").element[0];
        if (menu !== null && menu !== undefined) {
            var span = $(menu).find("span").filter(function () { return ($(this).text().indexOf('Colonne') > -1); })[0];
            if (span !== null && span !== undefined) {
                var ul = $(span).next();
                ul.css("columns", '' + ulColumns + '');
                ul.css("column-rule-style", "solid");
                ul.css("column-rule-color", "lightgray");
            }
        }



    } catch (e) {

    }
}

function onColumnMenuInit(e) {
    try {

        var filterMenu = this.thead.find("[data-field='" + e.field + "']").data("kendoColumnMenu").filterMenu;
        var checkSource = filterMenu.checkSource;

        // Salva il filtro corrente per il checkSource se presente
        if (checkSource) {
            var currentFilter = checkSource.filter();

            // Rimuove temporaneamente il filtro per mostrare tutti i valori
            checkSource.filter({}); // reset

            // Ordina
            checkSource.sort({ field: e.field, dir: "asc" });

            var view = checkSource.view();
            checkSource.data(view.toJSON());

            // Rigenera le checkbox con tutti i valori
            filterMenu.container.empty();
            filterMenu.createCheckBoxes();

            // Ripristina il filtro dati originale se presente
            if (currentFilter) {
                checkSource.filter(currentFilter);
            }

            // Reimposta i checkbox selezionati dai filtri attivi
            setTimeout(function () {
                var selectedValues = getFilterValuesByField(filterMenu.dataSource.filter(), e.field);

                $(".k-multicheck-wrap input[type='checkbox']:not(.k-checkbox-all)").each(function () {
                    const labelText = $(this).closest("label").text().trim();
                    if (selectedValues.includes(labelText)) {
                        $(this).prop("checked", true);
                    }
                });

                const $allVisible = $(".k-multicheck-wrap .k-item:visible input[type='checkbox']:not(.k-checkbox-all)");
                const allVisibleChecked = $allVisible.length > 0 && $allVisible.length === $allVisible.filter(":checked").length;
                filterMenu.checkBoxAll.prop("checked", allVisibleChecked);
            }, 50);
        }

        // Gestione search box
        if (filterMenu.searchTextBox) {
           
            filterMenu.searchTextBox.keyup(function (ev) {
                if ($(ev.target).val()) {
                    setTimeout(function () {
                        if ($('.k-item:visible').length > 0) {
                            filterMenu.checkBoxAll.closest('li').show();
                            filterMenu.checkBoxAll.unbind().change(function (event) {
                                event.preventDefault();
                                event.stopImmediatePropagation();
                                var checked = $(this).is(':checked');
                                let ul = $(this).closest("ul");
                                ul.find('.k-item:visible input:not(".k-check-all")').each(function (idx, item) {
                                    if ($(item).is(':checked') !== checked) {
                                        $(item).click();
                                    }
                                });

                                $(this).prop('checked', checked ? 'checked' : '');
                            });
                        }
                    }, 100);
                }
            });
        }

    } catch (ex) {
        console.error("Errore in onColumnMenuInit:", ex);
    }
}

// Estrae i valori filtrati per un campo, anche da filtri annidati
function getFilterValuesByField(filterObj, fieldName) {
    let values = [];

    if (!filterObj) return values;

    if (Array.isArray(filterObj.filters)) {
        filterObj.filters.forEach(f => {
            if (f.filters) {
                values = values.concat(getFilterValuesByField(f, fieldName));
            } else if (f.field === fieldName && f.operator === "eq") {
                values.push(f.value);
            }
        });
    }

    return values;
}

function impostaColonneKendoGridDaCookie(grid, gridId) {
    var columnsHidden = $.cookie(gridId + "Grid");
    if (columnsHidden != null) {
        var colNames = columnsHidden.split(";");
        for (var n = 0; n < colNames.length; n++) {
            for (var i = 0; i < grid.columns.length; i++) {
                if (grid.columns[i] != null && grid.columns[i].field == colNames[n]) {
                    grid.hideColumn(grid.columns[i].field);
                }
            }
        }
    }
}


function kGetElementiSelezionati(jquery_Selector) {

    var rval = new Array();

    var grid = $(jquery_Selector).closest("[data-role=grid]").data("kendoGrid");
    if (grid == undefined) {
        return rval;
    }

    var dataSource = grid.dataSource;
    var allData = dataSource.data();


    $.each(allData, function (idx, dataItem) {
        if (dataItem.Selected)
            rval.push(dataItem);
    });

    return rval;

}

// In caso di click su Annulla della griglia riappare la ricerca e non si eseguono operazioni
////$("#" + IDControllo).on("mousedown", ".k-grid-cancel-changes", function (e) {
////    var grid = $("#tab_testata_griglia_calibri").data("kendoGrid");
////    if (grid != null) {
////        var hasChanges = grid.dataSource.hasChanges();

////        if (hasChanges) {
////            if (confirm("Sono state effettuate modifiche sui dati, confermi di voler uscire senza salvare?")) {
////                // Do nothing!
////            } else {
////                // Do nothing!
////            }
////        }
////    }
////});

// Da stackoverflow.com/questions/13613098/refresh-a-single-kendo-grid-row
// Aggiorna un solo Item di una giglia dopo una modifica 
// In questo modo non c'è bisogno di fare il refresh e non si perde il posizionamento
function kendoFastRedrawRow(grid, row) {
    var dataItem = grid.dataItem(row);

    //Le nuove versioni di Kendo assegnano il ruolo "gridcell" anche alla colonna iniziale con la freccettina per la griglia gerarchica
    var rowChildren = $(row).children('td[role="gridcell"]').not('.k-hierarchy-cell');
    var x = -1;
    var primaColonna = 0;
    if (grid.options.editable !== undefined &&
        grid.options.editable.mode !== undefined &&
        grid.options.editable.mode == "inline") {
        primaColonna = 1;
    }
    for (var i = primaColonna; i < grid.columns.length; i++) {

        var column = grid.columns[i];

        if (column.columns === undefined) {
            x = kendoFastRedrawRowSingleColumn(grid, row, column, rowChildren, dataItem, x);
        } else {
            for (var i1 = 0; i1 < column.columns.length; i1++) {
                x = kendoFastRedrawRowSingleColumn(grid, row, column.columns[i1], rowChildren, dataItem, x);
            }
        }
    }
}

function kendoFastRedrawRowSingleColumn(grid, row, column, rowChildren, dataItem, x) {

    x++;  // Probabilmente non serve

    var template = column.template;

    var cell = rowChildren.eq(x);

    if (column.field !== undefined && column.field != "Selected") {

        if (template !== undefined) {
            var kendoTemplate = kendo.template(template);

            // Render using template
            cell.html(kendoTemplate(dataItem));
        } else {

            var fieldValue = dataItem[column.field];

            if (fieldValue !== null) {
                var format = column.format;
                var values = column.values;

                if (values !== undefined && values != null) {
                    // use the text value mappings (for enums)
                    for (var j = 0; j < values.length; j++) {
                        var value = values[j];
                        if (value.value == fieldValue) {
                            cell.html(value.text);
                            break;
                        }
                    }
                } else if (format !== undefined) {
                    // use the format
                    cell.html(kendo.format(format, fieldValue));
                } else {
                    // Just dump the plain old value
                    cell.html(fieldValue);
                }
            }
        }
    }

    return x;
}


// Da jsbin.com/pifevi/1/edit?html,output
// TODO Stefano non funziona bene perché se ci sono celle editabili ma da cui si fa la closeCell se campi chiave
// Inoltre occorre mettere class='editable-cell' nelle celle editabili (fare ciclo all'entrata)
function onGridKeydown(e) {
    if (e.keyCode === kendo.keys.TAB) {
        var grid = $(this).closest("[data-role=grid]").data("kendoGrid");
        var current = grid.current();
        if (!current.hasClass("editable-cell")) {
            var nextCell;
            if (e.shiftKey) {
                nextCell = current.prevAll(".editable-cell");
                if (!nextCell[0]) {
                    //search the next row
                    var prevRow = current.parent().prev();
                    nextCell = prevRow.children(".editable-cell:last");
                }
            } else {
                nextCell = current.nextAll(".editable-cell");
                if (!nextCell[0]) {
                    //search the next row
                    var nextRow = current.parent().next();
                    nextCell = nextRow.children(".editable-cell:first");
                }
            }
            grid.current(nextCell);
            grid.editCell(nextCell[0]);
        }

    }
}

// Funzione da chiamare per poter mostrare le righe cancellate che non si vedono più
// in caso di errori durante l'aggiornamento
// Le righe vengono aggiunte in fondo alla griglia e quindi deve essere rifatto il sort
// secondo il criterio attuale; se l'utente non ha fatto sort viene applicato quello passato in dsSort
function ripristinaRigheCancellateKendoGrid(grid, dsSort) {

    for (i = 0; i < grid.dataSource._destroyed.length; i++) {
        if (grid.dataSource._destroyed[i].mostraRigheCancellate)
            grid.dataSource._destroyed[i].deleted = true;
        grid.dataSource.add(grid.dataSource._destroyed[i]);
    }

    grid.dataSource._destroyed = [];
    if (typeof (grid.dataSource.sort()) == "undefined" &&
        dsSort != null &&
        dsSort.length > 0) {
        grid.dataSource.sort(dsSort);
    }
    else
        grid.dataSource.sort();

}


function numberEditor1decimals(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            format: "{0:n1}",
            decimals: 1,
            selectOnFocus: true
            //,step    : 0.5  eventualmente va dato col . anche se siamo in lingua italiana
        });
}

function numberEditor2decimals(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            format: "{0:n2}",
            decimals: 2,
            selectOnFocus: true
            //,step    : 0.5  eventualmente va dato col . anche se siamo in lingua italiana
        });
}

function numberEditor3decimals(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            format: "{0:n3}",
            decimals: 3,
            selectOnFocus: true
            //,step    : 0.5  eventualmente va dato col . anche se siamo in lingua italiana
        });
}

function numberEditor4decimals(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            format: "{0:n4}",
            decimals: 4,
            selectOnFocus: true
            //,step    : 0.5  eventualmente va dato col . anche se siamo in lingua italiana
        });
}

function numberEditor5decimals(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            format: "{0:n5}",
            decimals: 5,
            selectOnFocus: true
            //,step    : 0.5 eventualmente va dato col . anche se siamo in lingua italiana
        });
}

function numberEditor6decimals(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            format: "{0:n6}",
            decimals: 6,
            selectOnFocus: true
            //,step    : 0.5 eventualmente va dato col . anche se siamo in lingua italiana
        });
}

function timeEditor(container, options) {
    $('<input data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '"/>')
        .appendTo(container)
        .kendoTimePicker({
            change: function () {
                var prev = this.options.previous;
                var value = this.value();
                if (value === null) {
                    // In caso di errori
                    this.value(new Date(1900, 1, 1, 0, 0));
                }
            },
            min: new Date(1900, 1, 1, 0, 0)
            //max: new Date(2100, 12, 31, 0, 0)
        });
}

function dateTimeEditor(container, options) {
    $('<input data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '"/>')
        .appendTo(container)
        .kendoDateTimePicker({
            change: function () {
                var prev = this.options.previous;
                var value = this.value();
                if (value === null) {
                    // In caso di errori
                    this.value(new Date(1900, 0, 1, 0, 0));
                }
            },
            min: new Date(1900, 0, 1, 0, 0),
            max: new Date(2100, 11, 31, 23, 59)
        }
        );
}

/**
* Estende il modello dati della griglia
*
* @param {kendo_model} modello oggetto che contiene l'oggetto kendo_model da estendere
* @param {string} field nome del campo
*/
function kendo_Modello_estendi(modello, field) {

    $.extend(modello.kendo_model, field);


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
function kendo_Colonne_estendi(kendo_model, field, title, posizione, template_field, editor_template, css, command, filterable, daDuplicare, gruppoColonne) {

    var tf = "";
    if (template_field != "")
        tf = "#=" + template_field + "#";

    var attributes = undefined;

    if (css !== undefined && css !== null) {
        attributes = { "class": css };
    }

    if (daDuplicare === undefined)
        daDuplicare = false;

    if (gruppoColonne === undefined)
        gruppoColonne = "";

    var nuovaColonna = undefined;
    if (editor_template !== undefined && editor_template !== null) {
        nuovaColonna = {
            field: field,
            title: title,
            editor: editor_template,
            template: tf,
            attributes: attributes,
            command: command,
            filterable: filterable,
            daDuplicare: daDuplicare,
            gruppoColonne: gruppoColonne
        };
    } else {
        nuovaColonna = {
            field: field,
            title: title,
            command: command,
            filterable: filterable,
            daDuplicare: daDuplicare,
            gruppoColonne: gruppoColonne
        };
    }

    kendo_model.kendo_columns.splice(posizione, 0, nuovaColonna);

}

/**
* Estende le colonne della griglia per le combo
*    N.B.  RISPETTO ALLA kendo_Colonne_estendi UTILIZZA LA DESCRIZIONE NEL CAMPO FIELD E QUESTO PERMETTE DI
*          ORDINARE LA GRIGLIA PER DESCRIZIONE E DI ESPORTARE QUESTA
*
* @param {kendo_model} kendo_model modello kendo
* @param {string} field nome del campo
* @param {string} title titolo della colonna
* @param {number} posizione posizione a base zero
* @param {string} template nome del campo template da utilizzare
* @param {string} editor_template template per le combo
* @param {Object} command comando da associare alla colonna
*/
function kendo_Colonne_estendi_DDL(kendo_model, field, title, posizione, template_field, editor_template, css, command, filterable, daDuplicare, gruppoColonne) {

    var tf = "";
    if (template_field != "")
        tf = "#=" + template_field + "#";

    var attributes = undefined;

    if (css !== undefined && css !== null) {
        attributes = { "class": css };
    }

    if (daDuplicare === undefined)
        daDuplicare = false;

    if (gruppoColonne === undefined)
        gruppoColonne = "";

    var nuovaColonna = undefined;
    if (editor_template !== undefined && editor_template !== null) {
        nuovaColonna = {
            field: template_field,
            title: title,
            editor: editor_template,
            template: tf,
            attributes: attributes,
            command: command,
            filterable: filterable,
            daDuplicare: daDuplicare,
            gruppoColonne: gruppoColonne
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
function kendo_imposta_valore(dataItem, NomeCampo, valore, formattazione) {

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
function kendo_indiceColonna_DatoNomeCampo(jQuerySelector, NomeCampo) {
    return $(jQuerySelector).find("th[data-field='" + NomeCampo + "']").index();
}


/**
* Ottiene il valore appena digitato sulla griglia
*
* @param {kendogrid} grid griglia kendo $("#jquerySelector").data("kendoGrid")
*/
function kendo_valore_digitato(grid) {
    return $(grid.current()[0]).find(".k-input").val();
}

/**
* Ottiene il nome del campo nel model associato all'ultimo valore digitato/selezionato
*
* @param {object} e Evento
*/
function kendo_fieldName_digitato(e) {
    return $(e.sender._editContainer).find("[data-bind]").attr("data-bind").replace("value:", "");
}


/**
* Aggiusta automaticamente la larghezza delle colonne per la griglia recuperata dal selettore JQuery 
*
* @param {string} jQuerySelector Selettore JQuery
*/
function kendo_AggiustaDimensioneColonne(jQuerySelector) {
    var grid = $(jQuerySelector).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].widthfisso === undefined && grid.columns[i].columns == undefined) {
            grid.autoFitColumn(i);
        } else if (grid.columns[i].columns != undefined && grid.columns[i].columns.length > 0) {
            for (var ii = 0; ii < grid.columns[i].columns.length; ii++) {
                if (grid.columns[i].columns[ii].widthfisso === undefined) {
                    grid.autoFitColumn(grid.columns[i].columns[ii]);
                }
            }
        }
    }
}

function btnEliminaTuttiFiltri(nomeDiv) {
    var grid = $('#' + nomeDiv).data('kendoGrid');

    if (grid !== undefined) {
        //$("form.k-filter-menu button[type='reset']").trigger("click");
        grid.dataSource.filter({});
    }
}

function salvaPersonalizzazioniGrigliaKendo(urlbase, pagina, nomeDiv, mostraMess) {

    if (mostraMess === undefined)
        mostraMess = true;

    //var pagina = location.pathname; es: "/AgronicaAgenda_2010/Menu/MenuBS_Agenda_Nuovo.aspx"
    //var nomeDiv = 'divKendoOperazioni'; es: "divKendoOperazioni"

    var jsonDaSalvare = getPersonalizzazioniGrigliaKendo(pagina, nomeDiv);

    //localStorage[pagina + '|' + nomeDiv] = jsonDaSalvare;

    if (objP_utenti && objP_utenti != "") {

        var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "personalizzazione": jsonDaSalvare });

        ajaxAgronica(urlbase + PAGINA_CORE_UTENTI_IMPOSTAZIONI_R + '/Set_PersonalizzazioniGrigliaKendo', parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    var x = risposta.RispostaStringa;

                    //Se esiste una personalizzazione attivo il pulsante con il cestino (sempre che esista anche il pulsante)
                    if ($("#" + nomeDiv + " .cancellaPersonalizzazioniGrigliaKendo").length > 0)
                        $("#" + nomeDiv + " .cancellaPersonalizzazioniGrigliaKendo")[0].classList.remove(GIAS_K_STATE_DISABLED);

                    if (mostraMess)
                        $("<div></div>").kendoAlert({ title: "Tutto Bene", content: "Personalizzazioni griglia salvate correttamente." }).data("kendoAlert").open();
                }
                else
                    console.log("Err in salvataggio personalizzazioni kendo: pagina-> " + pagina + " - nomeDiv-> " + nomeDiv + " - errore-> " + risposta.Errore);
            }, null);
    }
}

function ripristinaPersonalizzazioniGrigliaKendo(urlbase, pagina, nomeDiv) {

    //var jsonDaRipristinare = localStorage[pagina + '|' + nomeDiv];
    var jsonDaRipristinare = null;
    if (objP_utenti && objP_utenti != "") {

        var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "pagina": pagina, "nomeDiv": nomeDiv });

        ajaxAgronicaSync(urlbase + PAGINA_CORE_UTENTI_IMPOSTAZIONI_R + '/Get_PersonalizzazioniGrigliaKendo', parametri, false,
            function (risposta) {
                if (risposta.RispostaOK) {
                    jsonDaRipristinare = risposta.RispostaStringa;

                }
                else
                    console.log("Err in lettura personalizzazioni kendo: pagina-> " + pagina + " - nomeDiv-> " + nomeDiv + " - errore-> " + risposta.Errore);
            }, null, null, false);
    }

    return jsonDaRipristinare;

}

function cancellaPersonalizzazioniGrigliaKendo(urlbase, pagina, nomeDiv, mostraMess) {

    if (mostraMess === undefined)
        mostraMess = true;

    if (objP_utenti && objP_utenti != "") {

        var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "pagina": pagina, "nomeDiv": nomeDiv });

        ajaxAgronicaSync(urlbase + PAGINA_CORE_UTENTI_IMPOSTAZIONI_R + '/Del_PersonalizzazioniGrigliaKendo', parametri, false,
            function (risposta) {
                if (risposta.RispostaOK) {

                    //Ricarico la griglia
                    var grid = $("#" + nomeDiv).data("kendoGrid");
                    setTimeout(function () {
                        grid.refresh();
                    });

                    //disattivo il pulsante con il cestino (sempre che esista anche il pulsante)
                    if ($("#" + nomeDiv + " .cancellaPersonalizzazioniGrigliaKendo").length > 0)
                        $("#" + nomeDiv + " .cancellaPersonalizzazioniGrigliaKendo")[0].classList.add(GIAS_K_STATE_DISABLED);

                    if (mostraMess)
                        $("<div></div>").kendoAlert({ title: "Tutto Bene", content: "Personalizzazioni griglia cancellate correttamente.<br>Rifare la ricerca o ricaricare la pagina per visualizzare la configurazione originale." }).data("kendoAlert").open();
                }
                else
                    console.log("Err in cancellazione personalizzazioni kendo: pagina-> " + pagina + " - nomeDiv-> " + nomeDiv + " - errore-> " + risposta.Errore);
            }, null, null, false);
    }

}

function getPersonalizzazioniGrigliaKendo(pagina, nomeDiv) {

    //var pagina = location.pathname; es: "/AgronicaAgenda_2010/Menu/MenuBS_Agenda_Nuovo.aspx"
    //var nomeDiv = 'divKendoOperazioni'; es: "divKendoOperazioni"
    var jsonDaSalvare = null;

    var grid = $('#' + nomeDiv).data('kendoGrid');

    if (grid !== undefined) {
        var dataSource = grid.dataSource;

        //var options = grid.getOptions();

        var columns = grid.columns;
        var pageSize = dataSource.pageSize();
        var sort = dataSource.sort();
        var filter = dataSource.filter();
        var group = dataSource.group();

        var options = { pagina: pagina, nomeDiv: nomeDiv, columns: columns, pageSize: pageSize, sort: sort, filter: filter, group: group };
        var options2 = JSON.parse(kendo.stringify(options));
        jsonDaSalvare = kendo.stringify(fixParametriGrigliaKendo(options2));
    }

    return jsonDaSalvare;
}

// fix per evitare script injection
function fixParametriGrigliaKendo(options) {
    for (i = 0; i < options.columns.length; i++) {
        options.columns[i].command = null;
        options.columns[i].template = null;
        options.columns[i].headerTemplate = null;
        options.columns[i].headerAttributes = null;
        options.columns[i].footerTemplate = null;
        options.columns[i].groupHeaderColumnTemplate = null;
        if (options.columns[i].columns != undefined && options.columns[i].columns != null) {
            for (j = 0; j < options.columns[i].columns.length; j++) {
                options.columns[i].columns[j].command = null;
                options.columns[i].columns[j].template = null;
                options.columns[i].columns[j].headerTemplate = null;
                options.columns[i].columns[j].headerAttributes = null;
                options.columns[i].columns[j].footerTemplate = null;
                options.columns[i].columns[j].groupHeaderColumnTemplate = null;
            }
        }
    }
    return options;
}

function setPersonalizzazioniGrigliaKendoDati(pagina, nomeDiv, jsonDaRipristinare, parametriKendoGrid, parametriDataSource, campiKendoModel, colonneKendoGrid) {

    try {

        var options = JSON.parse(jsonDaRipristinare);

        var grid = $('#' + nomeDiv).data('kendoGrid');
        //grid.setOptions(options);

        //NUMERO DI RIGHE PER PAGINA
        if (options.pageSize && parametriKendoGrid.pageable)
            parametriDataSource.pagesize = options.pageSize;

        //ORDINAMENTO (ASC/DESC) DELLE COLONNE
        if (options.sort) {

            var ordinam = options.sort;
            for (i = ordinam.length - 1; i >= 0; i--) {
                let col = colonneKendoGrid.find(function (v, index) { return colonneKendoGrid[index].field == ordinam[i].field; });

                if (col == undefined) {
                    ordinam.splice(i, 1);
                }
            }

            if (ordinam.length > 0)
                parametriDataSource.sort = options.sort;
        }

        //FILTRI PER COLONNE
        if (options.filter) {

            var filtri = options.filter.filters;
            for (i = filtri.length - 1; i >= 0; i--) {
                let col = colonneKendoGrid.find(function (v, index) { return colonneKendoGrid[index].field == filtri[i].field; });

                if (col == undefined) {
                    filtri.splice(i, 1);
                }

                // Sistemazione della data che era stata memorizzata a db in formato GMT
                if (col.field != undefined && campiKendoModel[col.field].type === "date")
                    filtri[i].value = kendo.parseDate(filtri[i].value);
            }

            if (filtri.length > 0)
                parametriDataSource.filter = options.filter;
        }

        //RAGGRUPPAMENTO IN TOOLBAR
        //DA TESTARE
        //            if (options.group) {

        //                var raggrup = options.group;
        //                for (i = raggrup.length - 1; i >= 0; i--) {
        //                    var col = colonneKendoGrid.find(function (v, index) { return colonneKendoGrid[index].field == raggrup[i].field; });

        //                    if (col == undefined) {
        //                        raggrup.splice(i, 1);
        //                    }
        //                }

        //                if (raggrup.length > 0)
        //                    parametriDataSource.group = options.group;
        //            }
    }
    catch (err) {
        console.log(err);
    }

    return parametriDataSource;

}

function setPersonalizzazioniGrigliaKendoColonne(pagina, nomeDiv, jsonDaRipristinare) {

    try {

        var options = JSON.parse(jsonDaRipristinare);

        var grid = $('#' + nomeDiv).data('kendoGrid');
        var dataSource = grid.dataSource;
        //grid.setOptions(options);

        var savedColumns = options.columns;

        //RIORDINAMENTO COLONNE
        var indOrd = 0;
        for (i = 0; i < savedColumns.length; i++) {
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
        for (i = savedColumns.length - 1; i >= 0; i--) {
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

        applyPersonalizzazioneColumnWidth(nomeDiv, savedColumns);
    }
    catch (err) {
        console.log(err);
    }
}

/**
 * Apply a preselected width and a minimum width to the columns inside the view (personalizzazione).
 * @param {any} gridId
 * @param {any} savedColumns
 */
function applyPersonalizzazioneColumnWidth(gridId, savedColumns) {
    var grid = $("#" + gridId).getKendoGrid();

    let minWidth = calculateTotalSavedColumnsWidth(savedColumns, grid);

    for (let i = savedColumns.length - 1; i >= 0; i--) {
        let savedWidth = savedColumns[i].width;
        if (grid.columns[i] == null)
            continue;

        let columnToSet = getCorrespondingColumn(grid.columns, savedColumns[i].field, i);
        if (columnToSet == null) {
            continue;
        }

        if (savedWidth != null)
            columnToSet.width = savedWidth;
        else {
            columnToSet.width = minWidth;
        }
    }
    grid.setOptions({
        columns: grid.columns,
        scrollable: true
    });
}

/**
 * Check whether the column we are about to change width to (from inside the view),
 * has a corresponding pair inside the grid personalization.
 */
function getCorrespondingColumn(gridColumns, field, index) {
    if (field == null)
        return null;

    if (gridColumns[index].field == field)
        return gridColumns[index];

    return gridColumns.find(col => col.field === field);
}


/**
 * Calculates the minimum width of the columns which do not have a set
 * width property - inside the view (personalizzazione).
 * @param {any} savedColumns
 * @param {any} tableWidth
 */
function calculateTotalSavedColumnsWidth(savedColumns, grid) {
    let tableWidth = grid.tbody.closest("table").width();

    let savedWidth = 0;
    let numColumnsWithNoWidthSetting = 0;

    for (let i = 0; i < savedColumns.length; ++i) {
        if (!savedColumns[i].hidden) {
            if (savedColumns[i].width != null)
                savedWidth += parseInt(savedColumns[i].width, 10);
            else
                numColumnsWithNoWidthSetting++;
        }
    }

    let minWidth = 120;
    if (numColumnsWithNoWidthSetting > 0) {
        let remainingWidth = tableWidth - (savedWidth + numColumnsWithNoWidthSetting * minWidth);
        if (remainingWidth > 0)
            minWidth = (tableWidth - savedWidth) / numColumnsWithNoWidthSetting;
    }

    return minWidth;
}



/**
* Escape di oggetti che rappresentano righe del model in griglie kendo
*
* @param {object} oggetto Rappresenta l'oggetto di cui fare il parse
*/
function kendoEscapeOggetto(oggetto) {

    //fare riferimento a questo articolo per info aggiuntive su escape del double quotes
    //https://stackoverflow.com/questions/4542556/error-when-passing-quotes-to-webservice-by-ajax
    return JSON.stringify(oggetto).replace(/'/g, "\\'").replace(/\\"/g, "\\\\\\\"");
}

function msgRequired(val) {
    mioObj = val;
    $.each(val, function (index, value) {
        if (value !== null && typeof value === 'object')
            msgRequired(value);
        else {
            if (index == 'required' && value.toString() == 'true') {
                mioObj[index] = { message: "Campo obbligatorio" };
            }
        }
    });
}

function stdKendoGridComandoModifica(clickEvent, icon_class) {
    if (icon_class == undefined || icon_class == null)
        icon_class = "fa fa-pencil";
    var cmd = {
        iconClass: icon_class,
        className: "Modifica",
        name: "Modifica",
        text: "&nbsp",
        click: clickEvent
    };

    return cmd;
}

function stdKendoGridComandoCancella(clickEvent, icon_class) {
    if (icon_class == undefined || icon_class == null)
        icon_class = "fa fa-trash-o";
    var cmd = {
        iconClass: icon_class,
        className: "Cancella",
        name: "Cancella",
        text: "&nbsp",
        click: clickEvent
    };

    return cmd;
}

// editKendoNumericTextBoxForGridInline e setValidation vengono utilizzate per applicare l'obbligatorietà dei campi
// nelle griglie inline
function editKendoNumericTextBoxForGridInline(container, options) {

    var decNr = 0;
    if (options.format !== undefined) {
        if (options.format == "{0:n1}")
            decNr = 1;
        if (options.format == "{0:n2}")
            decNr = 2;
        if (options.format == "{0:n3}")
            decNr = 3;
        if (options.format == "{0:n4}")
            decNr = 4;
        if (options.format == "{0:n5}")
            decNr = 5;
        if (options.format == "{0:n6}")
            decNr = 6;
    }

    $('<input data-type="number"' + setValidation(container, options) + ' data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: decNr,
            format: options.format,
            spinners: false,
            selectOnFocus: true
        }).off("keydown");

    // Inizio - per selezionare il contenuto quando si clicca sul campo o ci si arriva con il tab
    // Necessario perchè con i NumericTextBox questo non funziona
    // *** SOSTITUITO DALLA VERSIONE KENDO  2020.3.1118 dal selectOnFocus: true sopra
    //var myInput = container.find('input[name="' + options.field + '"]');

    //myInput.bind("focus", function () {
    //    var input = $(this);
    //    clearTimeout(input.data("selectTimeId")); //stop started time out if any

    //    var selectTimeId = setTimeout(function () {
    //        input.select();
    //    });

    //    input.data("selectTimeId", selectTimeId);
    //}).blur(function (e) {
    //    clearTimeout($(this).data("selectTimeId")); //stop started timeout
    //});
    // Fine - per selezionare il contenuto quando si clicca sul campo o ci si arriva con il tab

    $('<span class="k-invalid-msg" data-for="' + options.field + '"></span>').appendTo(container);
}

function setValidation(container, options) {

    // N.B. Questa funzione è stata testata solo con edit inline della Kendo Grid
    //      Non funziona sicuramente con edit popup mode ed è ancora da provare con 
    var retValidator = "";

    if ($(container).closest("[data-role=grid]").data("kendoGrid") !== undefined &&
        $(container).closest("[data-role=grid]").data("kendoGrid").dataSource.options.schema.model.fields[options.field].validation !== undefined) {

        var validation = $(container).closest("[data-role=grid]").data("kendoGrid").dataSource.options.schema.model.fields[options.field].validation;

        if (validation !== undefined) {
            if (validation.required !== undefined && validation.required !== null) {

                retValidator += " required ";

                if (validation.required.message !== undefined && validation.required.message !== null)
                    retValidator += " data-required-msg=\"" + validation.required.message + "\"";
            }

            if (validation.min !== undefined && validation.min !== null)
                retValidator += " min=\"" + validation.min + "\"";

            if (validation.max !== undefined && validation.max !== null)
                retValidator += " max=\"" + validation.max + "\"";

        }
    }

    return retValidator;
}


async function ExportExcelServer(e) {

    var risposta_byte = null;
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    var dataSource = grid.dataSource;
    var fileName = "Export.XLSX";

    var options = {
        format: "xlsx",
        fileName: fileName
    };
    var model = grid.columns;
    // fix per evitare script injection
    for (i = 0; i < model.length; i++) {
        model[i].command = null;
        //model[i].template = null;
        model[i].headerTemplate = null;
        model[i].headerAttributes = null;
        model[i].footerTemplate = null;
        model[i].groupHeaderColumnTemplate = null;
    }

    var data = new kendo.data.Query(dataSource.data()).filter(dataSource.filter()).data

    if (dataSource.sort() != undefined) {
        data = new kendo.data.Query(data)
            .sort(dataSource.sort()).data
    } 


    var tutteLeRighe = [];
    var urlBaseCoreWS = "";

    try {

        urlBaseCoreWS = GetUrlBaseCoreWS();

        for (let i = 0; i < data.length; i++) {
            for (var key in data[i]) {
                if (data[i][key] instanceof Date) { // per ignorare quelle ereditate
                    if (typeof AGRODATAINIZIO !== 'undefined' && AGRODATAINIZIO != undefined && AGRODATAINIZIO != null && data[i][key].getTime() === AGRODATAINIZIO.getTime()) {
                        data[i][key] = "";
                    } else if (typeof AGRODATAFINE !== 'undefined' && AGRODATAFINE != undefined && AGRODATAFINE != null && data[i][key].getTime() === AGRODATAFINE.getTime()) {
                        data[i][key] = "";
                    } else {
                        data[i][key] = kendo.toString(data[i][key], "dd/MM/yyyy")
                    }
                }
            }

            tutteLeRighe.push(data[i].toJSON());
        }

        var righeChunked = ChunkArray(tutteLeRighe, 1500);
        
        for (let i = 0; i < righeChunked.length; i++) {
            await storeDataChunk(urlBaseCoreWS, fileName, i, righeChunked[i])
        }

        exportToExcel(urlBaseCoreWS, options, model).then(risposta_byte => {
            if (risposta_byte != null && risposta_byte != undefined) {
                var blob = new Blob([ToArrayBuffer(risposta_byte)], { type: 'application/octet-stream' });
                var link = document.createElement("a");
                link.href = window.URL.createObjectURL(blob);
                link.download = fileName;
                link.click();
            }
        })
       
    }
    catch (err) {
        MessaggioErrore_Bootstrap(err, "DIV_Messaggi");
        console.error("Errore interno: " + err);
    }

}

function storeDataChunk(urlBaseCoreWS, fileName, i, righeChunked) {

    let param = kendo.stringify(
        {
            fileName: fileName,
            chunk: i + 1,
            data: kendoEscapeOggetto(righeChunked)
        });

    return new Promise((resolve, reject) => {
        ajaxAgronica(
            urlBaseCoreWS + indirizzohttp_GridServerExport + "/StoreDataChunk",
            param,
            (risposta) => resolve(risposta.RispostaStringa),
            function (risposta) {
                var errore = risposta.RispostaStringa;
                MessaggioErrore_Bootstrap(errore, "DIV_Messaggi");
                console.error("Errore interno: " + errore);
                reject(errore);
            },
            null,
            false
        );
    });
}

function exportToExcel(urlBaseCoreWS, options, model) {
    var param = kendo.stringify(
        {
            options: kendoEscapeOggetto(options),
            model: kendoEscapeOggetto(model)
        });
    return new Promise((resolve, reject) => {
        ajaxAgronica(urlBaseCoreWS + indirizzohttp_GridServerExport + "/ExportToExcel",
            param,
            function (risposta) {
                risposta_byte = risposta.RispostaStringa;
                resolve(risposta_byte);
            }, function (risposta) {
                var errore = risposta.RispostaStringa;
                MessaggioErrore_Bootstrap(errore, "DIV_Messaggi");
                console.error("Errore interno: " + errore);
                reject(errore);
            });
    });

}

function ToArrayBuffer(array) {
    return new Uint8Array(array);
}

function ChunkArray(array, size) {
    var results = [];
    while (array.length) {
        results.push(array.splice(0, size));
    }
    return results;
}

function GetUrlBaseCoreWS() {

    var url = window.location.protocol + "//" + window.location.hostname;

    if (typeof pathCoreWS !== "undefined") {
        if (pathCoreWS !== undefined && pathCoreWS.includes(url)) {
            url = "";
        }
    } else {
        throw 'Path core WS non trovato';
    }

    return url + pathCoreWS;
}

/**
 * Aggiunge la classe di selezione alla riga tr se la riga è stata selezionata o viceversa
 * @param {object} row elemento html rappresentante la riga (tr)
 * @param {boolean} checked indica se la riga è stata selezionata o no
 */
function rowKendoGridSelected(row, checked) {
    if (checked) {
        if (!row.hasClass(GIAS_K_STATE_SELECTED))
            //-select the row
            row.addClass(GIAS_K_STATE_SELECTED);
    } else {
        if (row.hasClass(GIAS_K_STATE_SELECTED))
            //-remove selection
            row.removeClass(GIAS_K_STATE_SELECTED);
    }
}
