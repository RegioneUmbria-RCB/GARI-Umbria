


/**
 * Aggiusta automaticamente la larghezza della drop down
 * @param {} e 
 * @returns {} 
 */
function kendoDropDownAdjustWidth(e, fixed) {


    var listContainer = e.sender.list.closest(".k-list-container");

    var listContainerWidth = listContainer.width();
    var containerWidth = $(window).width();

    if (containerWidth !== null) {
        if (listContainerWidth > containerWidth) {
            listContainerWidth = containerWidth - 13 - kendo.support.scrollbar();
        }
    }

    var w = 0;
    if (fixed !== undefined) {
        w = fixed;
    } else {
        w = listContainerWidth + kendo.support.scrollbar();
    }

    listContainer.width(w);


}

/**
 * Aggiunge un elemento alla dropdown
 * @param {} widgetId 
 * @param {} value 
 * @param {} text 
 * @returns {} 
 */
function kendoDropDown_addNew(widgetId, value, text, dataValueField, dataTextField) {
    var widget = $(widgetId).getKendoDropDownList();
    var dataSource = widget.dataSource;

    var s = '{ "' + dataValueField + '": "' + JsonEscape(value) + '", "' + dataTextField + '": "' + JsonEscape(text) + '" }';

    dataSource.add(JSON.parse(s));

    dataSource.one("sync", function () {
        widget.select(dataSource.view().length - 1);
    });

    dataSource.sync();

}

/**
 * Rimuove un elemento in base al suo valore
 * @param {string} widgetId selettore jquery della kendo dropdown
 * @param {string} value valore da rimuovere
 * @param {string} dataValueField campo per la ricerca
 * @returns {} 
 */
function kendoDropDown_removeByValue(widgetId, value, dataValueField) {

    var widget = $(widgetId).getKendoDropDownList();
    var dataSource = widget.dataSource;

    var oldData = dataSource.data();

    for (var i = 0; i < oldData.length; i++) {
        if (oldData[i][dataValueField] == value)
            widget.dataSource.remove(oldData[i]); //remove item
    }

    dataSource.sync();


}