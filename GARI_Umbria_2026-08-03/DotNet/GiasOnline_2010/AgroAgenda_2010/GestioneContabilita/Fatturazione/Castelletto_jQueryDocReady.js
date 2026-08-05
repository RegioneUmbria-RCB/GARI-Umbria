
//DOCUMENT READY
$(document).ready(function () {

    $.logThis("Castelletto_jQueryDocReady: INIZIO");
    
    impostaCastellettoUC(true, true, true);

    (function ($, kendo) {
        $.extend(true, kendo.ui.validator, {
            rules: { // custom rules
                timevalidation: function (input, params) {

                    if ($(input).data("kendoTimePicker") !== undefined &&
                        $(input).data("kendoTimePicker").value() === null)
                        return false;

                    return true;
                }
            },
            messages: {
                timevalidation: function (input) {
                    return "Ora non valida";
                }
            }
        });
    })(jQuery, kendo);



    //inizializzazione della pagina la prima volta che viene caricata

    $(".preArea").show();
    

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    
    //fine controlli

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

  

    //eventi di click pulsanti
   

 

    function onActivate(e) {
        //kendoConsole.log("Activated: " + $(e.item).find("> .k-link").text());
        var selectedIndex = $(e.item).index();
        //kendoConsole.log("selectedIndex: " + selectedIndex);

        
    }

    $.logThis("Castelletto_jQueryDocReady: FINE");

});