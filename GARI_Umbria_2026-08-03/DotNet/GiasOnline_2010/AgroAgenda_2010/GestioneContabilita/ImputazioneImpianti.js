
//DOCUMENT READY
$(document).ready(function () {

    $.logThis("BeniConfezionamento_jQueryDocReady: INIZIO");
    

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

    if (parseInt($(cIdAgenda).val()) !== 0) {
        $('input[name$="txt_dataop"]').val(formattedDate($(cData).val(), '/'));
    }
    else {
       $('input[name$="txt_dataop"]').val(formattedDate(new Date(), '/'));
    }

    
    //fine controlli

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

  

    //eventi di click pulsanti
   


    $("#btn_salva").click(
        function () {
            AggiornaDati(false);
        });


 

    function onActivate(e) {
        //kendoConsole.log("Activated: " + $(e.item).find("> .k-link").text());
        var selectedIndex = $(e.item).index();
        //kendoConsole.log("selectedIndex: " + selectedIndex);

        
    }



    $.logThis("BeniConfezionamento_jQueryDocReady: FINE");

});