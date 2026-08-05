var appezzamentoNuovoResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/Appezzamento_Nuovo.aspx.resx"
];

jQuery(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            appezzamentoNuovoResx.push(readResxFile(resxSinglePath, "Appezzamento_Nuovo_jQueryDocReady.js"));
        });
    }

    $('#div_terreno_nudo').hide();

    $('#Cmb_Finalita').attr("disabled", true);


    $('#ChkTerrenoNudo').change(function (e) {
        if ($('#ChkTerrenoNudo').is(':checked')) {
            $('#div_terreno_nudo').show();
            $('#div_coltura').hide();
        }
        else {
            $('#div_terreno_nudo').hide();
            $('#div_coltura').show();
        }
        nascondi_riepilogo_error();
    });


    // Carico la select Varieta' (on change Specie Vegetale) 
    $('#Cmb_Specie').change(function (e) {
        if ($('#Cmb_Specie').val() != "") {

            $('#Cmb_Finalita').attr("disabled", false);

            // Webservice per Finalità
            $.ajax({
                type: 'POST',
                url: 'Appezzamento_Nuovo.aspx/Carica_Select_Finalita',
                data: "{parametro:'" + $('#Cmb_Specie').val() + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#Cmb_Finalita').empty();
                    $('#Cmb_Finalita').html(r.d);
                    $('.selectpicker').selectpicker('refresh');
                }
            });


        }
        else {
            $('#Cmb_Finalita').empty();
            $('#Cmb_Finalita').attr("disabled", true);

        }

    });


    var fl2 = false
    var fl3 = true
    var fl4 = true
    var fl5 = false
    var fl6 = false


    nascondi_riepilogo_error();

    // Gestione Riepilogo Errori (Validazione)



    $("#TxtSuperficie").keyup(function () {
        if ($("#TxtSuperficie").val() != "") {
            $('.voce_2').hide();
            fl2 = true;
        }
        else {
            $('.voce_2').show();
            fl2 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#TxtSuperficie").change(function () {
        if ($("#TxtSuperficie").val() != "") {
            $('.voce_2').hide();
            fl2 = true;
        }
        else {
            $('.voce_2').show();
            fl2 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#TxtValiditaInizio").keyup(function () {
        if ($("#TxtValiditaInizio").val() != "") {
            $('.voce_3').hide();
            fl3 = true;
        }
        else {
            $('.voce_3').show();
            fl3 = false;
        }

        nascondi_riepilogo_error();
    });

    //$('#TxtValiditaInizio').datepicker()
    //    .on('changeDate', function (e) {
    //        $('.voce_3').hide();
    //        fl3 = true;

    //        nascondi_riepilogo_error();

    //    });

    $("#TxtValiditaFine").keyup(function () {
        if ($("#TxtValiditaFine").val() != "") {
            $('.voce_4').hide();
            fl4 = true;
        }
        else {
            $('.voce_4').show();
            fl4 = false;
        }

        nascondi_riepilogo_error();
    });

    //$('#TxtValiditaFine').datepicker()
    //    .on('changeDate', function (e) {
    //        $('.voce_4').hide();
    //        fl4 = true;

    //        nascondi_riepilogo_error();

    //    });

    $("#Cmb_Specie").change(function () {
        if ($("#Cmb_Specie").val() != "") {
            $('.voce_5').hide();
            fl5 = true;
        }
        else {
            $('.voce_5').show();
            fl5 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Cmb_Finalita").change(function () {
        if ($("#Cmb_Finalita").val() != "") {
            $('.voce_6').hide();
            fl6 = true;
        }
        else {
            $('.voce_6').show();
            fl6 = false;
        }

        nascondi_riepilogo_error();
    });


    function nascondi_riepilogo_error() {

        if (fl2)
            $('.voce_2').hide();

        if (fl3)
            $('.voce_3').hide();

        if (fl4)
            $('.voce_4').hide();

        if (fl5)
            $('.voce_5').hide();

        if (fl6 || $('#ChkTerrenoNudo').is(':checked'))
            $('.voce_6').hide();
        else
            $('.voce_6').show();

        if ((fl2) && (fl3) && (fl4) && (fl5) && (fl6 || $('#ChkTerrenoNudo').is(':checked')))
            $('#div_riepilogo_error').hide();
        else
            $('#div_riepilogo_error').show();
    }


    /////////////////////
    TxtValiditaInizio = $("#TxtValiditaInizio").kendoDatePicker({
        min: FinestraTemporaleInizio,
        max: FinestraTemporaleFine,
        //dateInput: true,
        format: "dd/MM/yyyy",
        change: function () {
            if (this.value() == null) {
                if (FinestraTemporaleInizio.getTime() == new Date(1900, 0, 1).getTime()) {
                    this.value(null);
                } else {
                    this.value(FinestraTemporaleInizio);
                }
            }
            if (!(this.value() >= FinestraTemporaleInizio || this.value() <= FinestraTemporaleFine)) {
                this.value(FinestraTemporaleInizio);
            }
            $('.voce_3').hide();
            fl3 = true;
            nascondi_riepilogo_error();
        }
    }).data("kendoDatePicker");
    
    TxtValiditaFine = $("#TxtValiditaFine").kendoDatePicker({
        min: FinestraTemporaleInizio,
        max: FinestraTemporaleFine,
        //dateInput: true,
        format: "dd/MM/yyyy",
        change: function () {
            if (this.value() == null) {
                if (FinestraTemporaleFine.getTime() == new Date(2100, 11, 31).getTime()) {
                    this.value(null);
                } else {
                    this.value(FinestraTemporaleFine);
                }
            }
            if (!(this.value() >= FinestraTemporaleInizio || this.value() <= FinestraTemporaleFine)) {
                this.value(FinestraTemporaleFine);
            }
            $('.voce_4').hide();
            fl4 = true;
            nascondi_riepilogo_error();

        }
    }).data("kendoDatePicker");




    if (!($('#ChkTerrenoNudo').is(':checked'))) {
        $('#Cmb_Specie').trigger("change");
    }


    var valInizio = new Date(new Date().getFullYear(), 0, 1);

    if (valInizio.getTime() >= FinestraTemporaleInizio.getTime() && valInizio.getTime() <= FinestraTemporaleFine.getTime()) {
        TxtValiditaInizio.value(valInizio);
    } else {
        TxtValiditaInizio.value(FinestraTemporaleInizio);
    }

    if (FinestraTemporaleFine.getTime() != new Date(2100, 11, 31).getTime()) {
        TxtValiditaFine.value(FinestraTemporaleFine);
    }

    var msgValidita = "";
    var msgValiditaInizio = ""
    var msgValiditaFine = ""
    if (FinestraTemporaleInizio.getTime() != new Date(1900, 00, 01).getTime()) {
        msgValiditaInizio = FinestraTemporaleInizio.toLocaleDateString()
    }

    if (FinestraTemporaleFine.getTime() != new Date(2100, 11, 31).getTime()) {
        msgValiditaFine = FinestraTemporaleFine.toLocaleDateString()
    }

    if (msgValiditaInizio !== "" || msgValiditaFine !== "") {
        msgValidita = "<div> Limite visibilità temporale:" + msgValiditaInizio + " - " + msgValiditaFine;
        $("#msgTxtValiditaInizio").html(msgValidita);
        $("#msgTxtValiditaFine").html(msgValidita);
    }

});