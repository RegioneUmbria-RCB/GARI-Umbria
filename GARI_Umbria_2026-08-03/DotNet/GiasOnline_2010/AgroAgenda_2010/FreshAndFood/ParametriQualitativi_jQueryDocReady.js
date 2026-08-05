//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    $("#grid_Area_ParametriQualitativi").hide();

    CreaTreeView_ParametriQualitativi("treeview_ParametriQualitativi");

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    $.logThis("DocReady: FINE");


});