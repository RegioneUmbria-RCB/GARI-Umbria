
//DOCUMENT READY
jQuery(function () {




    //    /// inizio eventi per gestione cambio check su tab_azienda
    //    $("#tab_lotti").on("change", ".seleziona_lotto", function () {
    //        if ($(this).is(':checked') == true)
    //        //aggiungo
    //            ModificaArray(seleziona_piva, true, $(this).val());
    //        else
    //            ModificaArray(seleziona_piva, false, $(this).val());
    //    });
    //    $('#tab_lotti').on('aggiornamentofiltro', function () {
    //        $('.seleziona_lotto').each(function () {
    //            var index = seleziona_piva.indexOf($(this).val());
    //            if (index > -1) {
    //                $(this).attr("checked", "checked");
    //            }
    //        });
    //    });
    //    /// FINE



    if (!primaRicerca) {
        Cerca(
            $("#" + txtID_Agenda_ClientID).val(),
            $("#" + cmbOModuli_Referenze_Config_Testata_ClientID).val(),
            $("#" + hLav_Cod_ClientID).val()
         );
        primaRicerca = true;
    }

    //soluzione 1. non viene impostato il n. bins.. quindi cambio soluzione..
    //    $(".watable .checkToggle").click();
    //    $(".watable .checkToggle").hide();
    //    
    //    $(".watable .checkToggle").on('click', function () {        

    //        $(".unique").each(function () {

    //            this.click();

    //        });
    //    });

    
    //soluzione 2.
    $(".watable .checkToggle").hide();
    $(".unique").each(function () {

        this.click();
    
    });


    if ($("#" + cmbOModuli_Referenze_Config_Testata_ClientID).length == 1) {
        $("#divCFGTabelle").hide();
    }

    //modal
    $("#divAnteprimaStampa").modal({
        keyboard: true,
        show: false
    });

});