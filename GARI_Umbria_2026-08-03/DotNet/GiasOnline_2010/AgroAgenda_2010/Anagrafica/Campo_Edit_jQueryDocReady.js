var ValiditaInizio_Old;
var ValiditaFine_Old;
var gridCodici;
var campoEditResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/Campo_Edit.aspx.resx"
];

jQuery(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            campoEditResx.push(readResxFile(resxSinglePath, "Campo_Edit_jQueryDocReady.js"));
        });
    }

    $('#aspnetForm').change(function () {
        controlla_form();
    });

    $('.datepicker').on('changeDate', function (ev) {
        $(this).datepicker('hide');
        controlla_form();
    });

    var initGestCat;
    var initParticelle;
    var initCodici;

    if (jsGestCat !== "") {
        initGestCat = jsGestCat;
    } else {
        initGestCat = "";
    }

    if (initGestCat != "") {
        AggiornaTabGestCat(JSON.parse(initGestCat));
    }

    if (jsParticelle !== "") {
        initParticelle = jsParticelle;
    } else {
        initParticelle = "";
    }

    if (initParticelle != "") {
        AggiornaTabParticelle(JSON.parse(initParticelle));
    }

    inizializzaKendoCodiciCampi("tabCodici");

    if ($('#chk_serra').is(':checked'))
        $('#sup_serra').show();
    else
        $('#sup_serra').hide();


    // visibilità orientamento colturale
    if ($('#Cmb_OrientamentoColturale').val() == 1) {

        if ($('#Cmb_SpecieVegetale').val() == "") {
            $('#Cmb_SpecieVegetale').attr('disabled', false);

            $.ajax({
                type: 'POST',
                url: 'Campo_Edit.aspx/Carica_Specie_Vegetali',
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#Cmb_SpecieVegetale').empty();
                    $('#Cmb_SpecieVegetale').append(r.d);

                    $('.selectpicker').selectpicker('refresh');
                }
            });
        }
    }
    else {
        $('#Cmb_SpecieVegetale').attr('disabled', true);
        $('#Cmb_SpecieVegetale').parent().find('button').attr('disabled', true);
    }


    // Quando cambia Ordinamento Colt., popolo Specie Veg.
    $('#Cmb_OrientamentoColturale').change(function (e) {

        //$('#<=Cmb_Comune.ClientID %>').parent().children().attr("disabled",false);

        //provincia = $('#<=Cmb_Provincia.ClientID %>').val();

        if ($('#Cmb_OrientamentoColturale').val() == 1) {
            //$('.riga_referente').show();
            $('#Cmb_SpecieVegetale').attr('disabled', false);
            $('#Cmb_SpecieVegetale').parent().find('button').attr('disabled', false);

            $.ajax({
                type: 'POST',
                url: 'Campo_Edit.aspx/Carica_Specie_Vegetali',
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#Cmb_SpecieVegetale').empty();
                    $('#Cmb_SpecieVegetale').append(r.d);

                    $('.selectpicker').selectpicker('refresh');
                }
            });
        }
        else {
            $('#Cmb_SpecieVegetale').empty();
            $('#Cmb_SpecieVegetale').attr('disabled', true);
            $('#Cmb_SpecieVegetale').parent().find('button').attr('disabled', true);
        }

    });


    //// Cambiano le date ---> Filtra la watable degli Appezzamenti
    //$('.date_validita').datepicker()
    //    .on('changeDate', function (e) {
    //        filtra_watable_app();
    //    });

    //            $('.date_validita').on('focusout', function (e) {
    //                    filtra_watable_app();
    //            });



    $('#chk_serra').click(function (e) {
        if ($('#chk_serra').is(':checked')) {
            $('#sup_serra').show();
            Imposta_Serra("1");
        } else {
            $('#sup_serra').hide();
            Imposta_Serra("0");
        }
    });


    // Controllo quale tipo di gestione campo è settato inizialmente
    if ($('#Opt_Senza_Catasto').is(':checked')) {
        $('.tab_catasto').hide();
        $('.div_SupCatasto').hide();
    }


    if ($('#Opt_Con_Catasto').is(':checked')) {
        $('.tab_catasto').show();
        $('.div_SupCatasto').show();
    }

    // Quando cambia la gestione del campo
    $('#Opt_Senza_Catasto').click(function (e) {
        $('.div_SupCatasto').hide();
        $('.tab_catasto').hide();


        $.ajax({
            type: 'POST',
            url: 'Campo_Edit.aspx/Aggiorna_Appezzamenti',
            data: "{tipo:'senza_catasto', data_inizio:'" + $('#TxtValiditaInizio').val() + "', data_fine:'" + $('#TxtValiditaFine').val() + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                AggiornaTabGestCat(JSON.parse(r.d));
            }
        });



    });

    $('#Opt_Con_Catasto').click(function (e) {
        $('.div_SupCatasto').show();
        $('.tab_catasto').show();

        $.ajax({
            type: 'POST',
            url: 'Campo_Edit.aspx/Aggiorna_Appezzamenti',
            data: "{tipo:'con_catasto', data_inizio:'" + $('#TxtValiditaInizio').val() + "', data_fine:'" + $('#TxtValiditaFine').val() + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                AggiornaTabGestCat(JSON.parse(r.d));
            }
        });

    });




    //////////////////////////////////


    //*** Gestione Watable Particelle ***//



    // Gestione click checkbox di riga
    //$('#tabParticelle .watable input[type="checkbox"]').click(function (e) {
    $(document).on("click", '#tabParticelle .watable input[type="checkbox"]', function () {
        //alert();
        var flag;
        var final;

        if ($(this).is(':checked')) {

            var val = $(this).parent().parent().find('.pos_sup_disp2').text();

            if (parseFloat(val.replace(/,/g, ".")) >= 0)
                $(this).parent().parent().find('.add_particella_val').val(val);
            else
                $(this).parent().parent().find('.add_particella_val').val(0);

            $(this).parent().parent().find('.pos_sup_disp').text(0);

            if (parseFloat(val.replace(/,/g, ".")) >= 0)
                $(this).parent().parent().find('.pos_sup_int').text(val);
            else
                $(this).parent().parent().find('.pos_sup_int').text(0);

            $(this).parent().parent().find('.wacol_checked').text(1);

            dati_particella = "";
            dati_particella += $(this).parent().parent().find('.pos_chiave').text() + "|";
            dati_particella += $(this).parent().parent().find('.pos_sup_disp').text() + "|";
            dati_particella += $(this).parent().parent().find('.add_particella_val').val();

            flag = "aggiungi";
        }
        else {

            dati_particella = "";
            dati_particella += $(this).parent().parent().find('.pos_chiave').text() + "|";
            dati_particella += $(this).parent().parent().find('.pos_sup_disp2').text() + "|";
            dati_particella += $(this).parent().parent().find('.add_particella_val').val();

            var val = $(this).parent().parent().find('.pos_sup_disp2').text();
            $(this).parent().parent().find('.add_particella_val').val(0);
            $(this).parent().parent().find('.pos_sup_disp').text(val);
            $(this).parent().parent().find('.pos_sup_int').text(0);
            $(this).parent().parent().find('.wacol_checked').text(0);


            $(this).parent().parent().find('.unique').attr("checked", true);

            flag = "elimina";
        }


        if (flag == "aggiungi")
            aggiungi_particella_ws(dati_particella, 1);

        if (flag == "elimina")
            elimina_particella_ws(dati_particella, 1);

    });


    // Gestione click checkbox di riga
    //$('#tabParticelle .watable input.add_particella_val').focusout(function (e) {
    $(document).on("focusout", '#tabParticelle .watable input.add_particella_val', function () {
        //alert();
        var chk = $(this).parent().parent().find('input[type="checkbox"]');
        var current = parseFloat($(this).val().replace(/,/g, "."));
        var final;
        var flag;

        if (current != 0) {
            if (chk.is(':checked')) {
                //var base = parseFloat($(this).parent().parent().find('.pos_sup_int').text().replace(/,/g , "."));
                var base = parseFloat($(this).parent().parent().find('.pos_sup_disp2').text().replace(/,/g, "."));
                final = base - current;
                $(this).parent().parent().find('.pos_sup_disp').text(final.toString().replace(/\./g, ','));
                $(this).parent().parent().find('.pos_sup_int').text($(this).val());
                flag = "aggiungi";
            }
            else {
                var base = parseFloat($(this).parent().parent().find('.pos_sup_disp2').text().replace(/,/g, "."));
                final = base - current;
                $(this).parent().parent().find('.pos_sup_disp').text(final.toString().replace(/\./g, ','));
                $(this).parent().parent().find('.pos_sup_int').text($(this).val());
                flag = "elimina";
            }

            $(this).parent().parent().find('input[type="checkbox"]').attr('checked', true);
            $(this).parent().parent().find('.wacol_checked').text(1);


            var dati = "";
            dati += $(this).parent().parent().find('.pos_chiave').text() + "|";
            dati += $(this).parent().parent().find('.pos_sup_disp').text() + "|";
            dati += $(this).parent().parent().find('.add_particella_val').val();


            // Inserisco il WARNING
            if (final < 0)
                alert(TraduzioneMultiResx(campoEditResx, "AttenzioneValoreSelezionatoMaggioreSuperficieDisponibileTotale", "ATTENZIONE: il valore selezionato è maggiore della superficie disponibile totale"));

            if (flag == "aggiungi")
                aggiungi_particella_ws(dati, 1);

            if (flag == "elimina")
                elimina_particella_ws(dati, 1);

        }
    });

    $("#TxtValiditaInizio").kendoDatePicker();
    $("#TxtValiditaFine").kendoDatePicker({
        max: new Date(2100, 11, 31)
    });

    //////////////////////////////////

    ValiditaInizio_Old = $("#TxtValiditaInizio").val();
    ValiditaFine_Old = $("#TxtValiditaFine").val();

}); //end ready