//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    //Nascondo i vari bottoni nell'header dove c'è il titolo della pagina
    try {
        var a = $(".titolo_sezione").html();
        if (a != undefined) {
            if (a.indexOf("</span>") != 0) {
                var g = a.substring(a.indexOf("</span>") + 7, a.length);
                $(".titolo_sezione").html(g);
            }
        }
    }
    catch (err) {

    }


    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    kendo.ui.NumericTextBox.fn.options.min = 0;

    CaricaControlli();

    $.logThis("DocReady: FINE");


});


//Se premo invio evito che parta la submit
$(document).keypress(function (e) {
    if (e.which == '13') {
        e.preventDefault();
        return false;
    }
});
