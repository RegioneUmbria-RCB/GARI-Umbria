
/**
* Restituisce l'oggetto Kendo DatePicker
*
* @param {string} IDControllo Id/name del controllo
* @returns {Object<string,any>} oggetto Kendo DatePicker
*/
function KendoDate(IDControllo) {
    var input = $("#" + IDControllo);
    if (input.length <= 0) {
        input = $('[name$="' + IDControllo + '"]');
    }
    return input.data("kendoDatePicker");
}

function set_data(IDControllo, v, def) {
    var dtPicker = $('input[name$="' + IDControllo + '"]').data("kendoDatePicker");
    if (v === null)
        dtPicker.value(def);
    else
        dtPicker.value(v);
}

function get_data(IDControllo) {
    var dtPicker = $('input[name$="' + IDControllo + '"]').data("kendoDatePicker");
    var dt = dtPicker.value();
    if (dt !== null)
        return formattedReverseDate(dt, "-");
    return dt;
}

/**
* Restituisce l'oggetto Kendo DateTimePicker
*
* @param {string} IDControllo Id/name del controllo
* @returns {Object<string,any>} oggetto Kendo DateTimePicker
*/
function KendoDateTime(IDControllo) {
    var input = $("#" + IDControllo);
    if (input.length <= 0) {
        input = $('[name$="' + IDControllo + '"]');
    }
    return input.data("kendoDateTimePicker");
}

function set_dataTime(IDControllo, v, def) {
    var dtPicker = $('input[name$="' + IDControllo + '"]').data("kendoDateTimePicker");
    if (v === null)
        dtPicker.value(def);
    else
        dtPicker.value(v);
}

function formattedReverseDate(date, sep) {

    if (sep == null)
        sep = "";

    var d = new Date(date || Date.now()),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join(sep);

}

function creaKendoDatePickerRange(ctrl_inizio, ctrl_fine) {
    //http://demos.telerik.com/kendo-ui/datepicker/rangeselection

    function startChange() {
        var startDate = start.value(),
            endDate = end.value();

        if (startDate) {
            startDate = new Date(startDate);
            startDate.setDate(startDate.getDate());
            end.min(startDate);
        } else if (endDate) {
            start.max(new Date(endDate));
            end.min(new Date(1900, 0, 1));//Aggiunto io
        } else {
            endDate = new Date();
            start.max(endDate);
            end.min(endDate);
        }
    }

    function endChange() {
        var endDate = end.value(),
            startDate = start.value();

        if (endDate) {
            endDate = new Date(endDate);
            endDate.setDate(endDate.getDate());
            start.max(endDate);
        } else if (startDate) {
            end.min(new Date(startDate));
            start.max(new Date(2100, 11, 31));//Aggiunto io
        } else {
            endDate = new Date();
            start.max(endDate);
            end.min(endDate);
        }
    }

    var start = $(ctrl_inizio).kendoDatePicker({
        dateInput: true,
        change: startChange
    }).data("kendoDatePicker");

    var end = $(ctrl_fine).kendoDatePicker({
        dateInput: true,
        change: endChange
    }).data("kendoDatePicker");

    start.max(end.value());
    end.min(start.value());
}