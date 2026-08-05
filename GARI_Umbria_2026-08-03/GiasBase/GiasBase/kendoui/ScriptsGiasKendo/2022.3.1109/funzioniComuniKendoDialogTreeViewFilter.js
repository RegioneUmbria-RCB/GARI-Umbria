

/* funzioniComuniKendoDialogTreeViewFilter.js */

/**
 * Crea un dialog con all'interno un treeview'
 * @param {any} IDControllo
 * @param {any} funzioniCRUD
 * @param {any} idModel
 * @param {any} campiKendoModel
 * @param {any} titoloFinestra
 * @param {any} parametriPerLettura
 * @param {any} parametriDataSource
 * @param {any} parametriKendoTreeView
 * @param {any} funzioniPrimaDopoEventi
 */
function creaKendoDialogTreeViewFilter(
    // PARAMETRI OBBLIGATORI
    IDControllo,
    funzioniCRUD,
    idModel,
    campiKendoModel,
    titoloFinestra,
    // PARAMETRI FACOLTATIVO
    parametriPerLettura, // parametri da passare alla lettura
    parametriDataSource, // parametri data source { chiave - valore}
    parametriKendoTreeView,   // parametri kendo TreeView [{ chiave - valore}]
    funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi { chiave - valore}:
    //{   
    //    funzioneDaChiamarePrimaDelDataBinding: yyyyy, // funzione da chiamare all'inizio del databinding
    //    funzioneDaChiamareDopoDataBinding: yyyyy, // funzione da chiamare alla fine del databinding
    //    funzioneDaChiamarePrimaDelDataBound: yyyyy, // funzione da chiamare all'inizio del databound
    //    funzioneDaChiamareDopoDataBound: yyyyy, // funzione da chiamare alla fine del databound
    //    funzioneDaChiamarePrimaDelSave: yyyyy, // funzione da chiamare all'inizio del save
    //    funzioneDaChiamareDopoSave: yyyyy // funzione da chiamare alla fine del save
    //    funzioneDaChiamareDopoDelete: yyyyy // funzione da chiamare dopo la cancellazione di una riga 
    //    funzioneDaChiamareCheck: yyyyy // funzione da chiamare su evento check
    //}        
) {

    if (IDControllo == null) {
        alert("Non mi hai passato l'ID del DIV che contiene il treeview");
        return;
    }


    $("#" + IDControllo + "_multiselect").kendoMultiSelect({
        dataTextField: "text",
        dataValueField: "id"
    });


    var dialog = $("#" + IDControllo + "_dialog");
    var multiSelect = $("#" + IDControllo + "_multiselect").data("kendoMultiSelect");

    $("#" + IDControllo + "openWindow").kendoButton();

    multiSelect.readonly(true);

    $("#" + IDControllo + "_openWindow").click(function () {
        dialog.data("kendoDialog").open();        
    });

    dialog.kendoDialog({
        width: "400px",
        title: titoloFinestra,
        visible: false,
        actions: [
            {
                text: 'Cancel',
                primary: false,
                action: agroDialogTreeViewFilter_onCancelClick
            },
            {
                text: 'Ok',
                primary: true,
                action: agroDialogTreeViewFilter_onOkClick
            }
        ],
        close: agroDialogTreeViewFilter_onClose
    }).data("kendoDialog");

    var myTreeView = creaKendoTreeView(IDControllo + "_treeview", funzioniCRUD, idModel, campiKendoModel, parametriPerLettura, parametriDataSource, parametriKendoTreeView, funzioniPrimaDopoEventi);

    $("#" + IDControllo + "_filterText").keyup(function (e) {
        var filterText = $(this).val();

        if (filterText !== "") {
            $("." + IDControllo + "_selectAll").css("visibility", "hidden");

            $("#" + IDControllo + "_treeview .k-group .k-group .k-in").closest("li").hide();
            $("#" + IDControllo + "_treeview .k-group").closest("li").hide();
            $("#" + IDControllo + "_treeview .k-in:contains(" + filterText + ")").each(function () {
                $(this).parents("ul, li").each(function () {
                    var treeView = $("#" + IDControllo + "_treeview").data("kendoTreeView");
                    treeView.expand($(this).parents("li"));
                    $(this).show();
                });
            });
            $("#" + IDControllo + "_treeview .k-group .k-in:contains(" + filterText + ")").each(function () {
                $(this).parents("ul, li").each(function () {
                    $(this).show();
                });
            });
        }
        else {
            $("#" + IDControllo + "_treeview .k-group").find("li").show();
            var nodes = $("#" + IDControllo + "_treeview > .k-group > li");

            $.each(nodes, function (i, val) {
                if (nodes[i].getAttribute("data-expanded") == null) {
                    $(nodes[i]).find("li").hide();
                }
            });

            $("." + IDControllo + "_selectAll").css("visibility", "visible");
        }
    });

    function agroDialogTreeViewFilter_onOkClick(e) {

        var IDControllo = e.sender.element[0].id;
        IDControllo = agroDialogTreeViewFilter_getID(IDControllo);

        var checkedNodes = [];
        var treeView = $("#" + IDControllo +"_treeview").data("kendoTreeView");

        getCheckedNodes(treeView.dataSource.view(), checkedNodes);
        agroDialogTreeViewFilter_populateMultiSelect(IDControllo, checkedNodes);

        if (funzioniPrimaDopoEventi.agroDialogTreeViewFilter_onAfterOkClick !== undefined) {
            funzioniPrimaDopoEventi.agroDialogTreeViewFilter_onAfterOkClick();
        }

        e.sender.close();
    }

    return myTreeView;
}


function agroDialogTreeViewFilter_onCancelClick(e) {
    e.sender.close();
}

function agroDialogTreeViewFilter_getID(idCompleto) {

    return idCompleto.split("_")[0];

}


function agroDialogTreeViewFilter_onClose() {
    $("#openWindow").fadeIn();
}

function agroDialogTreeViewFilter_populateMultiSelect(IDControllo, checkedNodes) {

    var multiSelect = $("#" + IDControllo + "_multiselect").data("kendoMultiSelect");
    multiSelect.dataSource.data([]);

    var multiData = multiSelect.dataSource.data();
    if (checkedNodes.length > 0) {
        var array = multiSelect.value().slice();
        for (var i = 0; i < checkedNodes.length; i++) {
            multiData.push({ text: checkedNodes[i].text, id: checkedNodes[i].id });
            array.push(checkedNodes[i].id.toString());
        }

        multiSelect.dataSource.data(multiData);
        multiSelect.dataSource.filter({});
        multiSelect.value(array);
    }
}

function checkUncheckAllNodes(nodes, checked) {
    //*** GABRIELE 16/07/2018 ***
    //nella configurazione del treeview è impostato checkboxes -> checkChildren: true
    //è sufficiente impostare e check/uncheck tutti i nodi a livello 0 e l'impostazione si propaga in automatico a tutti i loro figli
    let nl = nodes.length;
    if (nl == 0)
        return;
    for (let i = 0; i < nl; i++) {
        nodes[i].set("checked", checked);
    }
/*
    for (var i = 0; i < nodes.length; i++) {
        nodes[i].set("checked", checked);

        if (nodes[i].hasChildren) {
            checkUncheckAllNodes(nodes[i].children.view(), checked);
        }
    }
*/
}

function chbAllOnChange() {
    var checkedNodes = [];
    var treeView = $("#treeview").data("kendoTreeView");
    var isAllChecked = $('#chbAll').prop("checked");

    checkUncheckAllNodes(treeView.dataSource.view(), isAllChecked)

    if (isAllChecked) {
        setMessage($('#treeview input[type="checkbox"]').length);
    }
    else {
        setMessage(0);
    }
}

function getCheckedNodes(nodes, checkedNodes) {
    var node;

    for (var i = 0; i < nodes.length; i++) {
        node = nodes[i];

        if (node.checked) {
            checkedNodes.push({ text: node.text, id: node.id });
        }

        if (node.hasChildren) {
            getCheckedNodes(node.children.view(), checkedNodes);
        }
    }
}



function onCheck() {
    var checkedNodes = [];
    var treeView = $("#treeview").data("kendoTreeView");

    getCheckedNodes(treeView.dataSource.view(), checkedNodes);
    setMessage(checkedNodes.length);
}

function onExpand(e) {
    if ($("#filterText").val() == "") {
        $(e.node).find("li").show();
    }
}

function setMessage(checkedNodes) {
    var message;

    if (checkedNodes > 0) {
        message = checkedNodes + " categories selected";
    }
    else {
        message = "0 categories selected";
    }

    $("#result").html(message);

}


