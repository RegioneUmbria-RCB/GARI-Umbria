function creaKendoMultiselect(IDControllo, t, textfield, valuefield, filterType, functionSelect, functionDeselect, functionChange, isVirtual, autobind) {

    var ds = new kendo.data.DataSource({ transport: t });

    if (filterType === null ||
        filterType === undefined) {
        filterType = "contains";
    }

    if (autobind == undefined || autobind == null) {
        autobind = true
    }

    let parametri = {
        autoClose: false,
        dataSource: ds,
        dataTextField: textfield,
        dataValueField: valuefield,
        filter: filterType,
        select: functionSelect,
        deselect: functionDeselect,
        change: functionChange,
        autoBind: autobind
    }

    if (isVirtual !== undefined && isVirtual !== null && isVirtual) {
        parametri.virtual = {
            itemHeight: 26,
            valueMapper: function (options, dataValueField = valuefield) {

                var val = options.value;
                if (val != "" && val != "-1") {
                    var item = this.dataSource._pristineData.find(el => el[dataValueField] == val);
                    options.success(this.dataSource._pristineData.indexOf(item));
                } else {
                    options.success();
                }
            }
        };
        parametri.mapValueTo = "dataItem";

        parametri.dataBound = function (e) {
            var ds = this.dataSource.data();
            if (ds.length == 1) {
                this.select(1);
            }
        };
    }

    $('select[name$="' + IDControllo + '"]').kendoMultiSelect(parametri);
}

function creaKendoMultiselectEditor(container, _dataTextField, _dataValueField, _dataSource, functionChange) {

    $('<input required name="' + _dataValueField + '"/>')
        .appendTo(container)
        .kendoMultiSelect({
            autoBind: true,
            dataTextField: _dataTextField,
            dataValueField: _dataValueField,
            dataSource: _dataSource,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: functionChange
        }).data("kendoMultiSelect");

    var ddl = $('input[name$="' + _dataValueField + '"]').data("kendoMultiSelect");
    ddl.list.width("auto");

    return ddl;

}

function creaKendoMultiselectServerFiltering(IDControllo, t, minLength, textfield, valuefield, filterType, functionSelect, functionDeselect, functionChange) {

    var ds = new kendo.data.DataSource({ serverFiltering: true, transport: t });

    if (filterType === null ||
        filterType === undefined) {
        filterType = "contains";
    }

    $('select[name$="' + IDControllo + '"]').kendoMultiSelect({
        autoClose: false,
        autoBind: false,
        dataSource: ds,
        dataTextField: textfield,
        dataValueField: valuefield,
        filter: filterType,
        minLength: minLength,
        select: functionSelect,
        deselect: functionDeselect,
        change: functionChange
    });
}

/**
 * Crea una kendo drop down filtrabile via server
 * @param {string} NameControllo Valore Attributo "NAME": il controllo deve essere un input con un campo name. il valore di quest'ultimo deve essere passato a questo parametro
 * @param {string} _dataTextField
 * @param {string} _dataValueField
 * @param {function} functionRead
 * @param {function} functionChange
 * @param {number} minLength
 * @param {string} defaultValue
 * @param {string} defaultText
 * @param {function} functionClose
 * @param {boolean} autoWidth
 */
function creaKendoDropDownListServerFiltering(NameControllo, _dataTextField, _dataValueField, functionRead, functionChange, minLength, defaultValue, defaultText, functionClose, autoWidth, noDataTemplate) {

    if (autoWidth === undefined || autoWidth === null) {
        autoWidth = true;
    }

    if (noDataTemplate === undefined || noDataTemplate === null || noDataTemplate === "") {
        noDataTemplate = "Digitare almeno " + minLength.toString() + " caratteri.<br/><br/> Se non vengono mostrati risultati non esistono dati contententi questi caratteri";
    }

    $('input[name$="' + NameControllo + '"]').kendoDropDownList({
        autoBind: false,
        autoWidth: autoWidth,
        value: defaultValue,
        text: defaultText,
        dataTextField: _dataTextField,
        dataValueField: _dataValueField,
        filter: "contains",
        minLength: minLength,
        //open: function (e) {
        //    if (e.sender.dataSource.data().length > 0)
        //        e.sender.dataSource.data([]);
        //},
        change: functionChange,
        close: functionClose,
        noDataTemplate: noDataTemplate,
        dataSource: {
            serverFiltering: true,
            transport: {
                read: functionRead
            }
        }

    });

    var ddl = $('input[name$="' + NameControllo + '"]').data("kendoDropDownList");
    //ddl.list.width("auto");

    return ddl;
}

/**
 * Crea una kendo drop down filtrabile su server
 * @param {string} IDControllo Valore Attributo "NAME": il controllo deve essere un input con un campo name. il valore di quest'ultimo deve essere passato a questo parametro
 * @param {any} t
 * @param {string} textfield
 * @param {string} valuefield
 * @param {any} filterType
 * @param {any} noDataTemplate
 * @param {any} autoBind
 * @param {any} template
 * @param {any} valueTemplate
 * @param {boolean} autoWidth
 * @param {string} groupField
 * @param {function} groupTemplateFunction
 * @param {boolean} isVirtual
 * @param {any} optionLabel
 * @param {number} minLength
 */
function creaKendoDropDownListServerFilteringComplete(IDControllo, t, textfield, valuefield, filterType, noDataTemplate, autoBind, template, valueTemplate, autoWidth, groupField, groupTemplateFunction, isVirtual, optionLabel, minLength) {
    //filteringArray: var filtroPersona =  [{ field: "Rag_Soc" }, { field: "Cod_Contatto", operator: "contains" }];
    //se non è presente la proprietà 'operator', viene messo obj.operator = "contains"

    var dsParam = { transport: t };
    if (groupField !== undefined && groupField !== null) {
        dsParam.group = { field: groupField }
    }


    var ds = new kendo.data.DataSource(dsParam);
    ds.options.serverFiltering = true;

    if (filterType === null || filterType === undefined) {
        filterType = "contains";
    }

    if (autoBind === undefined || autoBind === null) {
        autoBind = true;
    }

    if (autoWidth === undefined || autoWidth === null) {
        autoWidth = true;
    }

    var dropDown = {
        autoWidth: autoWidth,
        autoBind: autoBind,
        dataSource: ds,
        dataTextField: textfield,
        dataValueField: valuefield,
        template: template,
        valueTemplate: valueTemplate,
        filter: filterType,
        noDataTemplate: noDataTemplate,
        minLength: minLength
    };

    if ((groupField !== undefined && groupField !== null) && (groupTemplateFunction !== undefined && groupTemplateFunction !== null)) {
        dropDown.groupTemplate = groupTemplateFunction
    }

    if (isVirtual !== undefined && isVirtual !== null && isVirtual) {
        dropDown.virtual = {
            itemHeight: 26,
            valueMapper: function (options, dataValueField = valuefield) {

                var val = options.value;
                if (val != "" && val != "-1") {
                    var item = this.dataSource._pristineData.find(el => el[dataValueField] == val);
                    options.success(this.dataSource._pristineData.indexOf(item));
                } else {
                    options.success();
                }
            }
        };
        dropDown.mapValueTo = "dataItem";

        dropDown.dataBound = function (e) {
            var ds = this.dataSource.data();
            if (ds.length == 1) {
                this.select(1);
            }
        };
    }

    if (optionLabel !== undefined && optionLabel !== null) {
        dropDown.optionLabel = optionLabel
    }

    return $('#' + IDControllo).kendoDropDownList(dropDown);
}

/**
 * Crea una kendo drop down filtrabile su client
 * @param {string} IDControllo Valore Attributo "NAME": il controllo deve essere un input con un campo name. il valore di quest'ultimo deve essere passato a questo parametro
 * @param {any} t
 * @param {string} textfield
 * @param {string} valuefield
 * @param {any} filterType
 * @param {any} filteringArray
 * @param {any} noDataTemplate
 * @param {any} autoBind
 * @param {any} template
 * @param {any} valueTemplate
 * @param {boolean} autoWidth
 * @param {string} groupField
 * @param {function} groupTemplateFunction
 * @param {boolean} isVirtual
 */
function creaKendoDropDownList(IDControllo, t, textfield, valuefield, filterType, filteringArray, noDataTemplate, autoBind, template, valueTemplate, autoWidth, groupField, groupTemplateFunction, isVirtual, optionLabel) {
    //filteringArray: var filtroPersona =  [{ field: "Rag_Soc" }, { field: "Cod_Contatto", operator: "contains" }];
    //se non è presente la proprietà 'operator', viene messo obj.operator = "contains"

    var dsParam = { transport: t };

    if (groupField !== undefined && groupField !== null) {
        dsParam.group = { field: groupField }
    }


    var ds = new kendo.data.DataSource(dsParam);

    if (filterType === null || filterType === undefined) {
        filterType = "contains";
    }

    if (autoBind === undefined || autoBind === null) {
        autoBind = true;
    }

    if (autoWidth === undefined || autoWidth === null) {
        autoWidth = true;
    }


    var dropDown = {
        autoWidth: autoWidth,
        autoBind: autoBind,
        dataSource: ds,
        dataTextField: textfield,
        dataValueField: valuefield,
        template: template,
        valueTemplate: valueTemplate,
        filter: filterType,
        noDataTemplate: noDataTemplate
    };

    if ((groupField !== undefined && groupField !== null) && (groupTemplateFunction !== undefined && groupTemplateFunction !== null)) {
        dropDown.groupTemplate = groupTemplateFunction
    }

    if (isVirtual !== undefined && isVirtual !== null && isVirtual) {
        dropDown.virtual = {
            itemHeight: 26,
            valueMapper: function (options, dataValueField = valuefield) {

                var val = options.value;
                if (val != "" && val != "-1") {
                    var item = this.dataSource._pristineData.find(el => el[dataValueField] == val);
                    options.success(this.dataSource._pristineData.indexOf(item));
                } else {
                    options.success();
                }
            }
        };
        dropDown.mapValueTo = "dataItem";

        dropDown.dataBound = function (e) {
            var ds = this.dataSource.data();
            if (ds.length == 1) {
                this.select(1);
            }
        };
    }

    if (optionLabel !== undefined && optionLabel !== null) {
        dropDown.optionLabel = optionLabel
    }


    //per fare filtro su più campi (anche nascosti)
    if (filteringArray !== null && filteringArray !== undefined && filteringArray.length > 0) {

        dropDown.filtering = function (ev) {

            var filterValue = ev.filter !== undefined ? ev.filter.value : "";
            ev.preventDefault();

            //aggiungo proprietà extra all'array che mi è arrivato
            filteringArray.forEach(function (obj) {
                if (!obj.hasOwnProperty('operator')) {
                    obj.operator = filterType;
                }
                obj.value = filterValue;
            });

            this.dataSource.filter({
                logic: "or",
                filters: filteringArray
            });
        };
    }

    return $('#' + IDControllo).kendoDropDownList(dropDown);
}

function creaKendoDropDownListWithData(IDControllo, d) {

    var ds = new kendo.data.DataSource({ data: d });
    return $('input[name$="' + IDControllo + '"]').kendoDropDownList({
        dataSource: ds,
        dataTextField: "text",
        dataValueField: "value"
    });
}


function editKendoNumericTextBox(container, options) {

    var decNr = 0;
    if (options.format !== undefined) {
        if (options.format === "{0:n1}")
            decNr = 1;
        if (options.format === "{0:n2}")
            decNr = 2;
        if (options.format === "{0:n3}")
            decNr = 3;
        if (options.format === "{0:n4}")
            decNr = 4;
        if (options.format === "{0:n5}")
            decNr = 5;
        if (options.format === "{0:n6}")
            decNr = 6;
    }

    $('<input data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
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

// ------ DropDownList --------------------------------------
/**
* Restituisce l'oggetto Kendo DropDownList
*
* @param {string} IDControllo Id/name del controllo
* @returns {Object<string,any>} oggetto Kendo DropDownList
*/
function KendoDDL(IDControllo) {
    var input = $("#" + IDControllo);
    if (input.length <= 0) {
        input = $('[name$="' + IDControllo + '"]');
    }
    return input.data("kendoDropDownList");
}

/**
* Restituisce il valore della Kendo DropDownList. Se non è stato selezionato nulla restituisce il valDefault passato (se definito)
*
* @param {string} IDControllo Id/name del controllo
* @param {any} valDefault Valore di default da usare nel caso non sia stato selezionato nulla
* @returns {any} valore della Kendo DropDownList
*/
function Get_KendoDDLValue(IDControllo, valDefault) {
    var ddl = KendoDDL(IDControllo);

    if (ddl.selectedIndex === -1 && valDefault !== undefined) {
        return valDefault;
    } else {
        const value = ddl.value();
        const optLabel = ddl.options.optionLabel;
        if (value === "" && optLabel != null && optLabel != "" && ddl.dataSource.view().length === 0) {
            return optLabel[ddl.options.dataValueField];
        }
        return value;
    }
}

/**
* Imposta valore della Kendo DropDownList (con valore default)
*
* @param {string} IDControllo Id/name del controllo
* @param {any} valore valore da impostare
* @param {any} defaultValue valore di default (impostato se valore è null)
*/
function Set_KendoDDLValue(IDControllo, valore, defaultValue) {
    var ddl = KendoDDL(IDControllo);
    if (ddl !== undefined && ddl !== null) {
        if (valore !== undefined && valore !== null)
            ddl.value(valore);
        else
            ddl.value(defaultValue);
    }
}

async function Set_KendoDDLValueVirtual(IDControllo, valore, defaultValue, timeout, interval) {
    var ddl = KendoDDL(IDControllo);
    if (ddl !== undefined && ddl !== null) {
        if (valore !== undefined && valore !== null) {
            ddl.value(valore);
            if (timeout === undefined || timeout === null) {
                timeout = 20000;
            }
            if (interval === undefined || interval === null) {
                interval = 50;
            }

            return new Promise((resolve) => {
                const startTime = Date.now();
                const intervalId = setInterval(() => {
                    let value = Get_KendoDDLValue(IDControllo);
                    if (value == valore) {
                        clearInterval(intervalId);
                        resolve(true);
                    } else if (Date.now() - startTime >= timeout) {
                        clearInterval(intervalId);
                        resolve(false);
                    }
                }, interval);
            });
        }
        else {
            ddl.value(defaultValue);
        }
    }
}

/**
* Imposta valore della Kendo DropDownList dalla descrizione (con valore default)
*
* @param {string} IDControllo Id/name del controllo
* @param {string} sigla sigla del valore da impostare
* @param {any} defaultValue valore di default (impostato se valore è null)
*/
function Set_KendoDDLValue_From_Sigla(IDControllo, sigla, defaultValue) {
    var ddl = KendoDDL(IDControllo);
    if (ddl !== undefined && ddl !== null) {
        const data = ddl.dataSource.data();
        let valore = defaultValue;
        for (let i = 0, e = data.length; i < e; i++) {
            const dataRow = data[i];
            if (dataRow.val_sigla === sigla) {
                valore = dataRow.val_cod;
                break;
            }
        }
        ddl.value(valore);
    }
}

/**
* Imposta valore della Kendo DropDownList
*
* @param {string} IDControllo Id/name del controllo
* @param {any} v valore da impostare
*/
function Set_KendoDDLValueNoDef(IDControllo, v) {
    var ddl = KendoDDL(IDControllo);
    if (ddl !== undefined && ddl !== null)
        ddl.value(v);
}

function ClearDDL(IDControllo) {
    var ddl = KendoDDL(IDControllo);
    if (ddl !== undefined && ddl !== null) {
        if (ddl.dataSource !== undefined) {
            ddl.dataSource.data([{}]);
            ddl.text(""); // clears visible text
            ddl.value(""); // clears invisible value
        }
    }
}

function Set_KendoDDLIndex(IDControllo, valore, indice) {
    var ddl = KendoDDL(IDControllo);
    if (valore === null)
        ddl.select(indice);
    else {
        ddl.value(valore);
        if (ddl.select() < 0)
            ddl.select(indice);
    }
}

// ----------------------------------------------------------

// ------ MultiColumnComboBox -------------------------------

/**
* Restituisce l'oggetto Kendo MultiColumnComboBox
*
* @param {string} IDControllo Id/name del controllo
* @returns {Object<string,any>} oggetto Kendo MultiColumnComboBox
*/

function KendoMultiColumnComboBox(IDControllo) {
    var input = $('#' + IDControllo);
    if (input.length <= 0) {
        input = $('[name$="' + IDControllo + '"]');
    }
    return input.data("kendoMultiColumnComboBox");
}

// ----------------------------------------------------------

// ------ MultiSelect ---------------------------------------
function KendoMultisel(IDControllo) {
    var input = $('#' + IDControllo);
    if (input.length <= 0) {
        input = $('[name$="' + IDControllo + '"]');
    }
    return input.data("kendoMultiSelect");
}

function Get_MultiselString(IDControllo) {
    if (KendoMultisel(IDControllo) === undefined)
        return null;
    var str = KendoMultisel(IDControllo).value().join('|');
    if (str === "")
        return null;
    return str;
}

function Set_MultiselValue(IDControllo, valore) {
    if (KendoMultisel(IDControllo) !== undefined) {
        if (valore === undefined || valore === null)
            KendoMultisel(IDControllo).value([]);
        else
            KendoMultisel(IDControllo).value(valore.split('|'));
    }
}
// ----------------------------------------------------------

// ------ NumericTextBox ------------------------------------
function KendoNumTB(IDControllo) {
    var input = $('#' + IDControllo);
    if (input.length <= 0) {
        input = $('[name$="' + IDControllo + '"]');
    }
    return input.data("kendoNumericTextBox");
}

function Get_KendoNumTBValue(IDControllo, ZeroIfNull) {
    if (ZeroIfNull === undefined || ZeroIfNull === null)
        ZeroIfNull = false;

    if (ZeroIfNull && KendoNumTB(IDControllo).value() === null)
        return 0;

    return KendoNumTB(IDControllo).value();
}

function Set_KendoNumTBValue(IDControllo, valore) {
    if (valore === undefined)
        valore = KendoNumTB(IDControllo).min();
    KendoNumTB(IDControllo).value(valore);
}

function KendoNumericValue(IDControllo, defaultValue) {
    var numTB = KendoNumTB(IDControllo);
    var val = numTB.value();
    if (val === null) {
        if (defaultValue === null ||
            defaultValue === undefined) {
            return numTB.min();
        }
        else {
            return defaultValue;
        }
    }

    return val;
}

//lasciata per il momento per retro-compatibilità, non usare (eliminabile se agenda > 21/03/2018)
function KendoNumericBox(IDControllo) {
    return $('#' + IDControllo).data("kendoNumericTextBox");
}
// ----------------------------------------------------------

function creaDropDownEditor(container, _dataTextField, _dataValueField, _dataSource, functionChange, required) {

    if (required == undefined || required == null) {
        required = true;
    }
    let req = "";
    if (required) {
        req = "required";
    }

    $('<input ' + req + ' name="' + _dataValueField + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: _dataTextField,
            dataValueField: _dataValueField,
            dataSource: _dataSource,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: functionChange
        });

    var ddl = $('input[name$="' + _dataValueField + '"]').data("kendoDropDownList");
    ddl.list.width("auto");

    return ddl;
}

function creaDropDownEditorId(container, _dataTextField, _dataValueField, _dataSource, functionChange, IDControllo, bindToValue) {

    var id = '<input required name="' + IDControllo + '" id="' + IDControllo + '" ';
    var autoBind = false;

    if (bindToValue !== undefined && bindToValue !== "") {
        id = id + ' data-bind="value:' + bindToValue + '"';
    } else {
        autoBind = true;
    }

    id = id + ' />';

    $(id).appendTo(container)
        .kendoDropDownList({
            autoBind: autoBind,
            dataTextField: _dataTextField,
            dataValueField: _dataValueField,
            dataSource: _dataSource,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: functionChange
        });

    var ddl = $('#' + IDControllo).data("kendoDropDownList");
    ddl.list.width("auto");

    return ddl;
}

function creaDropDownEditorServerFiltering(container, _dataTextField, _dataValueField, functionRead, functionChange, minLength, defaultValue, defaultText, functionClose) {

    $('<input required name="' + _dataValueField + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: false,
            autoWidth: true,
            value: defaultValue,
            text: defaultText,
            dataTextField: _dataTextField,
            dataValueField: _dataValueField,
            filter: "contains",
            minLength: minLength,
            open: function (e) {
                //var listContainer = e.sender.list.closest(".k-list-container");
                //listContainer.width(listContainer.width() + kendo.support.scrollbar());
                if (e.sender.dataSource.data().length > 0)
                    e.sender.dataSource.data([]);
            },
            change: functionChange,
            close: functionClose,
            noDataTemplate: "Digitare almeno " + minLength.toString() + " caratteri.<br/><br/> Se non vengono mostrati risultati non esistono dati contententi questi caratteri",
            dataSource: {
                serverFiltering: true,
                transport: {
                    read: functionRead
                }
            }

        });

    var ddl = $('input[name$="' + _dataValueField + '"]').data("kendoDropDownList");
    //ddl.list.width("auto");

    return ddl;
}



function creaMultiColumnComboBoxEditor(container, _dataTextField, _dataValueField, _dataSource, functionChange, columns, filterFields, height, popupOriginYX, minLength) {

    $('<input required name="' + _dataValueField + '"/>')
        .appendTo(container)
        .kendoMultiColumnComboBox({
            autoBind: true,
            dataTextField: _dataTextField,
            dataValueField: _dataValueField,
            dataSource: _dataSource,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: functionChange,
            columns: columns,
            filterFields: filterFields,
            height: height,
            popup: {
                origin: popupOriginYX
            },
            minLength: minLength
        });

    var mccb = $('input[name$="' + _dataValueField + '"]').data("kendoMultiColumnComboBox");
    mccb.list.width("auto");

    return mccb;
}

// ------ Switch --------------------------------------
/**
* Restituisce l'oggetto Kendo Switch
*
* @param {string} IDControllo Id/name del controllo
* @returns {Object<string,any>} oggetto Kendo Switch
*/
function KendoSwitch(IDControllo) {
    var input = $("#" + IDControllo);
    if (input.length <= 0) {
        input = $('input[name$="' + IDControllo + '"]');
    }
    return input.data("kendoSwitch");
}

function creaKendoSwitch(name, onLabel, offLabel, checked, change, width, size) {
    if (name === undefined || name === "") name = ".kendoSwitch"; else name = 'input[name$="' + name + '"]';
    if (onLabel === undefined || onLabel === "") onLabel = "SI";
    if (offLabel === undefined || offLabel === "") offLabel = "NO";
    if (width === undefined || width === "") width = "60";
    if (size === undefined || size === "") size = "12px";
    var kendo = $(name).kendoSwitch({ messages: { checked: onLabel, unchecked: offLabel }, checked: checked, change: change, width: width });
    if (name === undefined || name === "") $(".k-switch").css({ 'font-size': size }); else $(name).parent().css({ 'font-size': size });
    if (checked !== undefined && checked) kendo.data("kendoSwitch").check(true);
    return kendo.data("kendoSwitch");
}

function getKendoSwitch(name) {
    return KendoSwitch(name).check();
}

function setKendoSwitch(name, value) {
    return KendoSwitch(name).check(value);
}

function setKendoSwitchVisible(name, visible) {
    if (visible)
        return KendoSwitch(name).wrapper.show();
    else
        return KendoSwitch(name).wrapper.hide();
}

// ------ Slider --------------------------------------
/**
* Restituisce l'oggetto Kendo Slider
*
* @param {string} IDControllo Id/name del controllo
* @returns {Object<string,any>} oggetto Kendo Slider
*/
function KendoSlider(IDControllo) {
    var input = $("#" + IDControllo);
    if (input.length <= 0) {
        input = $('input[name$="' + IDControllo + '"]');
    }
    return input.data("kendoSlider");
}

/**
 * Crea l'oggetto Kendo Slider
 *
 * @param {string} name name del controllo
 * @param {number} min The minimum value (default: 0)
 * @param {number} max The maximum value (default: 10)
 * @param {number} smallStep The small step value (default: 1) [determines the amount of Slider value change when the end user clicks on the increase or decrease buttons of the Slider]
 * @param {number} largeStep The delta with which the value will change when the user presses the Page Up or Page Down key. (default: = smallStep) [largeStep will also set a large tick for every large step]
 * @param {string} orientation The orientation of a Slider ["horizontal"|"vertical"] (default: "horizontal")
 * @param {boolean} showButtons Can be used to show or hide the increase and decrease buttons (default: true)
 * @param {string} tickPlacement Denotes the location of the tick marks ["topLeft"|"bottomRight"|"both"|"none"] (default: "both")
 * @param {string} increaseButtonTitle The title of the increase button (default: "Aumenta")
 * @param {string} decreaseButtonTitle The title of the decrease button (default: "Diminuisci")
 * @param {string} dragHandleTitle The title of the drag handle (default: "Trascina")
 * @param {string} width dimensione dello slider (default: "200px")
 * @param {number} startValue il valore dello slider da impostare in fase di creazione
 * @param {function()} change funzione da chiamare sull'evento change
 * @returns {Object<string,any>} oggetto Kendo Slider
 *
 */
function creaKendoSlider(name, min, max, smallStep, largeStep, orientation, showButtons, tickPlacement, increaseButtonTitle, decreaseButtonTitle, dragHandleTitle, width, startValue, change) {
    if (name === undefined || name === "") name = ".kendoSlider"; else name = 'input[name$="' + name + '"]';

    if (min === undefined || min === null || min == "") min = 0;
    if (max === undefined || max === null || max == "") max = 10;

    if (smallStep === undefined || smallStep === null || smallStep == "") smallStep = 1;
    //Se non passato, large step = small step
    if (largeStep === undefined || largeStep === null || largeStep == "") largeStep = smallStep;

    if (orientation === undefined || orientation === null || orientation === "") orientation = "horizontal";
    if (showButtons === undefined || showButtons === null) showButtons = true;

    if (tickPlacement === undefined || tickPlacement === null || tickPlacement === "") tickPlacement = "both";

    if (increaseButtonTitle === undefined || increaseButtonTitle === null) increaseButtonTitle = "Aumenta";
    if (decreaseButtonTitle === undefined || decreaseButtonTitle === null) decreaseButtonTitle = "Diminuisci";
    if (dragHandleTitle === undefined || dragHandleTitle === null) dragHandleTitle = "Trascina";

    if (width === undefined || width === null || width === "") width = "200px";

    var kendo = $(name).kendoSlider({
        min: min,
        max: max,
        smallStep: smallStep,
        largeStep: largeStep,
        orientation: orientation,
        showButtons: showButtons,
        tickPlacement: tickPlacement,
        increaseButtonTitle: increaseButtonTitle,
        decreaseButtonTitle: decreaseButtonTitle,
        dragHandleTitle: dragHandleTitle,
        change: change
    });

    kendo.data("kendoSlider").wrapper.css("width", width);
    kendo.data("kendoSlider").resize();

    if (startValue !== undefined && startValue !== null && startValue != "" && !isNaN(startValue))
        kendo.data("kendoSlider").value(startValue);

    return kendo.data("kendoSlider");
}

/**
 * Restituisce il valore attuale del Kendo Slider
 *
 * @param {string} name name del controllo
 * @returns {number} valore attuale dello slider
 */
function getKendoSlider(name) {
    return KendoSlider(name).value();
}

/**
 * Imposta un valore sul Kendo Slider
 *
 * @param {string} name name del controllo
 * @param {number} value valore da impostare
 */
function setKendoSlider(name, value) {
    return KendoSlider(name).value(value);
}

/**
 * Rende visibile o invisibile il Kendo Slider
 *
 * @param {string} name name del controllo
 * @param {boolean} visible imposta la visibilità
 */
function setKendoSliderVisible(name, visible) {
    if (visible)
        return KendoSlider(name).wrapper.show();
    else
        return KendoSlider(name).wrapper.hide();
}

(function ($) {
    var CoordMaskedTextBox = kendo.ui.MaskedTextBox.extend({
        options: {
            name: "CoordMaskedTextBox",
            coord: "",
            formato: ""
        },
        init: function (element, options) {

            options.mask = "~";
            options.rules = {
                "~": /[+-]/
            };

            if (typeof options.formato !== "string") {
                options.formato = "DEC"; //"GMS"
            }

            // The base call to the widget initialization.
            kendo.ui.MaskedTextBox.fn.init.call(this, element, options);

            this._gestisciFormato(this.options.formato);

            $(element).keypress(function (e) {
                let tb = $(this).data("kendoCoordMaskedTextBox");
                let raw = tb.raw();
                if (raw === "" || this.selectionStart === 0) {

                    if ('0' <= e.key && e.key <= '9') {

                        tb.value("+" + e.key);

                        e.preventDefault();
                    }
                }
            });

        },
        _gestisciFormato: function (formato) {

            let mask = "~00°00'00\"";
            let placeholder = "±GG°MM'SS\"";
            if (formato === "DEC") {
                mask = "~00.0000000°";
                placeholder = "±GG.GGGGGGG°";
            }

            this.element.attr("placeholder", this.options.coord + " " + placeholder);
            this.setOptions({ mask: mask, formato: formato });
        },
        _gestisciValue: function (decVal, formato) {

            if (isNaN(decVal)) {
                return "";
            }

            let val = ""
            if (formato === "DEC") {

                val = kendo.toString(decVal, "00.0000000000");

            } else {

                let max_deg = 90;
                if (this.options.coord === "Lng") {
                    max_deg = 180;
                }

                let sign = decVal < 0 ? -1 : 1;
                let absVal = Math.abs(Math.round(decVal * 1000000));
                if (absVal <= (max_deg * 1000000)) {

                    let dec = absVal % 1000000 / 1000000;
                    let deg = Math.floor(absVal / 1000000) * sign;
                    let min = Math.floor(dec * 60);
                    let sec = (dec - min / 60) * 3600;

                    val = kendo.toString(deg, "00") + "" + kendo.toString(min, "00") + "" + kendo.toString(sec, "00");
                }
            }

            if (val.length > 0 && val.charAt(0) !== "-") {
                val = "+" + val;
            }

            return val;
        },
        switchFormato: function () {

            let formato = "GMS";
            if (formato === this.options.formato) {
                formato = "DEC";
            }

            let val = this._gestisciValue(this.decimalValue(), formato);

            this._gestisciFormato(formato);

            this.value(val);
        },
        decimalValue: function () {

            let decVal = Number.NaN;

            if (this.options.formato === "DEC") {

                let val = this.value();
                if (val.length > 0) {
                    decVal = parseFloat(val.replace("°", "").replace(this.options.promptChar, "").replace(",", "."));
                }

            } else {

                let raw = this.raw();
                if (raw.length === 7) { //+GGMMSS

                    let deg = parseFloat(raw.slice(0, 3));
                    let min = parseFloat(raw.slice(3, 5));
                    let sec = parseFloat(raw.slice(5, 7));

                    if (!isNaN(deg) && !isNaN(min) && !isNaN(sec)) {

                        let sign = deg < 0 ? -1 : 1;
                        let abs = Math.abs(deg);
                        decVal = sign * (abs + (min / 60.0) + (sec / 3600));
                    }
                }
            }

            return decVal;
        },
        setDecimalValue: function (decVal) {

            if (typeof (decVal) == 'string' && !isNaN(decVal)) {
                decVal = kendo.parseFloat(decVal);
            }

            //let val = this._gestisciValue(decVal, this.options.formato);

            //this._gestisciFormato(this.options.formato);

            //this.value(val);

            this.value(this._gestisciValue(decVal, this.options.formato));
        }
    });
    kendo.ui.plugin(CoordMaskedTextBox);
})(jQuery);