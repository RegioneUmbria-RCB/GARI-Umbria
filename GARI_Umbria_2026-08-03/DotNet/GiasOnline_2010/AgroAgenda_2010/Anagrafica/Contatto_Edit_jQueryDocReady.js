var objParametri_Agenda;

jQuery(document).ready(function () {

    //carico eventualmente la tabella di Rubrica
    var initInidirzzi;
    var initCosti;
    var initRapporti;

    if (jsIndirizzi != "") {
        initInidirzzi = jsIndirizzi;
    } else {
        initInidirzzi = "";
    }

    if (initInidirzzi != "") {
        AggiornaTabIndirizzi(initInidirzzi);
    }


    if (jsCosti != "") {
        initCosti = jsCosti;
    } else {
        initCosti = "";
    }

    if (initCosti != "") {
        AggiornaTabCosti(initCosti);
    }


    if (jsRapporti != "") {
        initRapporti = jsRapporti;
    } else {
        initRapporti = "";
    }

    if (initRapporti != "") {
        AggiornaTabRapporti(initRapporti);
    }


    objParametri_Agenda = JSON.parse(objP_agenda);

    if (objParametri_Agenda.Tipo_Operazione === "1") {
        $("#RBL_TipoUtente").find('input').prop('disabled', false);
    } else {
        $("#RBL_TipoUtente").find('input').prop('disabled', true);
    }
    
    // Controllo se vengono selezionati/deselezionati Rapporti Contabili
    // Inserisco controllo se per il rapporto contabile selezionato esistono movimenti contabili (WS)
    // nel caso non tolgo il check

    // Gestione click check della tabella possessi 
    $(document).on("change", 'input[type=checkbox].unique', function () {

        if (!$(this).is(':checked')) {
            var codice;
            var ris = true;
            if ($('#datiPersona').is(':visible')) {
                codice = $('#Txt_CF').val();
            }

            if ($('#datiAzienda').is(':visible')) {
                codice = $('#Txt_Piva').val();
            }

            $.ajax({
                type: 'POST',
                url: 'Contatto_Edit.aspx/Controlla_MovimentiContabiliWS',
                data: "{Cod_Contatto:'" + codice + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {
                    if (r.d.RispostaOK == false) {
                        ris = false;
                        alert(r.d.Errore);
                    }
                }
            });

            if (!ris) {
                $(this).attr('checked', true);
                this.setAttribute("checked", "checked");
                this.checked = true;
            }

        }

    });


    $('#ddl_comune').parent().children().attr("disabled", true);

    // Gestione della selezione della nazione
    if ($('#cmb_Stato').val() == "IT") {
        $("#div_prov_com").show();
        $("#lbl_frazione").text("Frazione");
    }
    else {
        $("#div_prov_com").hide();
        $("#lbl_frazione").text("Città");
    }

    $('#cmb_Stato').change(function (e) {
        if ($('#cmb_Stato').val() == "IT") {
            $("#div_prov_com").show();
            $("#lbl_frazione").text("Frazione");

        }
        else {
            $("#div_prov_com").hide();
            $("#lbl_frazione").text("Città");
            $("#txt_cap").text("00000");
            $("#ddl_comune").val("");
            $("#Txt_ProCodIstat").val("000");
            $("#Txt_ComCodIstat").val("000");
            $("#ddl_provincia").val("00");

        }
    });


    // Setto la tabella Rapporti la prima volta
    var tipo = $('#RBL_TipoUtente').find('input:checked').val();
    $.ajax({
        type: 'POST',
        url: 'Contatto_Edit.aspx/Aggiorna_RapportiWS',
        data: "{tipo_persona:" + tipo + "}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            AggiornaTabRapporti(JSON.parse(r.d));

        }
    });


    // Quando cambia Provincia...
    $('#ddl_provincia').change(function (e) {

        $('#ddl_comune').parent().children().attr("disabled", false);

        provincia = $('#ddl_provincia').val();

        if ($('#ddl_provincia').val() != "") {
            //$('.riga_referente').show();

            // ...mostro Comuni relativi
            $.ajax({
                type: 'POST',
                url: 'Contatto_Edit.aspx/Carica_Comuni',
                data: "{provincia:'" + provincia + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#ddl_comune').empty();
                    $('#ddl_comune').append(r.d[0]);
                    //                            $('#<=Txt_ProCodIstat.ClientID %>').val(r.d[1]);
                    //                            $('#<=Txt_ProvinciaSigla.ClientID %>').val(provincia);

                    $('.selectpicker').selectpicker('refresh');
                }
            });


            // ...salvo codice provincia
            $.ajax({
                type: 'POST',
                url: 'Contatto_Edit.aspx/Cambia_provincia_salva_codice',
                data: "{targa:'" + provincia + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {

                    $('#Txt_ProCodIstat').val(r.d);
                    $('#Txt_ProvinciaSigla').val(provincia);

                }
            });



        }
        else {
            $('#ddl_comune').empty();
            $('#ddl_comune').parent().children().attr("disabled", true);
            $('.selectpicker').selectpicker('refresh');
        }
    });


    // Quando cambia Comune...
    $('#ddl_comune').change(function (e) {

        provincia = $('#ddl_provincia').val();
        nome_comune = $('#ddl_comune option:selected').text();

        // ...salvo codice provincia
        $.ajax({
            type: 'POST',
            url: 'Contatto_Edit.aspx/Cambia_comune_salva_codice',
            data: "{provincia:'" + provincia + "', nome_comune:'" + nome_comune + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                var values = r.d.split('|');

                $('#Txt_ComCodIstat').val(values[1]);


            }
        });
    });



    $('#RBL_TipoUtente').change(function (e) {
        var tipo = $('#RBL_TipoUtente').find('input:checked').val();

        pulisci_form_da_validazione();

        if (tipo == 1) {
            $('#datiAzienda').show();
            $('#datiPersona').hide();
            $('#datiPatentino').hide();
        }
        else {
            $('#datiAzienda').hide();
            $('#datiPersona').show();
            $('#datiPatentino').show();
        }

        // Aggiorno la tabella Rapporti
        $.ajax({
            type: 'POST',
            url: 'Contatto_Edit.aspx/Aggiorna_RapportiWS',
            data: "{tipo_persona:" + tipo + "}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                AggiornaTabRapporti(JSON.parse(r.d));

            }
        });


        // Aggiorno la select Visibilità
        $.ajax({
            type: 'POST',
            url: 'Contatto_Edit.aspx/Aggiorna_Visibilita',
            data: "{tipo_persona:" + tipo + "}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {

                $('#Cmb_CentriAziendali').empty();
                $('#Cmb_CentriAziendali').append(r.d);
                $('.selectpicker').selectpicker('refresh');

            }
        });


        // CaricaTipologiaIndirizzo e aggiorna select relativa
        $.ajax({
            type: 'POST',
            url: 'Contatto_Edit.aspx/CaricaTipologiaIndirizzo',
            data: "{tipo_persona:" + tipo + "}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                $('#ddl_tipo_Indirizzo').empty();
                $('#ddl_tipo_Indirizzo').append(r.d);
                $('.selectpicker').selectpicker('refresh');

            }
        });

        controlla_form();

    });


    // Click su modifica Indirizzo (salvataggio)
    $('#btn_modifica_indirizzo').click(function () {

        var parametri = ""

        parametri += $('#ddl_tipo_Indirizzo').val() + "|";
        parametri += $('#ddl_tipo_Indirizzo option:selected').text() + "|";
        parametri += $('#txt_via').val() + "|";
        parametri += $('#ddl_provincia').val() + "|";
        parametri += $('#ddl_provincia option:selected').text() + "|";
        parametri += $('#Txt_ProCodIstat').val() + "|";
        parametri += $('#Txt_ComCodIstat').val() + "|";
        parametri += $('#ddl_comune option:selected').text() + "|";
        parametri += $('#txt_frazione').val() + "|";
        parametri += $('#txt_cap').val() + "|";
        //parametri += $('#<=txt_stato.ClientID %>').val() + "|";
        parametri += $('#cmb_Stato option:selected').text() + "|";
        parametri += $('#Txt_Note_Indirizzo').val();


        $.ajax({
            type: 'POST',
            url: 'Contatto_Edit.aspx/Modifica_Indirizzo',
            data: "{parametri:'" + parametri + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                // Aggiorna la watable indirizzi
                AggiornaTabIndirizzi(JSON.parse(r.d));

            }
        });

    });


    // Click su aggiungi Indirizzo (salvataggio)
    $('#btn_aggiungi_indirizzo').click(function () {

        var ok = valida_indirizzo();

        // Se è validato
        if (ok) {

            var parametri = ""

            parametri += $('#ddl_tipo_Indirizzo').val() + "|";
            parametri += $('#ddl_tipo_Indirizzo option:selected').text() + "|";
            parametri += $('#txt_via').val() + "|";
            parametri += $('#ddl_provincia').val() + "|";
            parametri += $('#ddl_provincia option:selected').text() + "|";
            parametri += $('#Txt_ProvinciaSigla').val() + "|";
            parametri += $('#ddl_comune').val() + "|";
            parametri += $('#ddl_comune option:selected').text() + "|";
            parametri += $('#txt_frazione').val() + "|";
            parametri += $('#txt_cap').val() + "|";
            //parametri += $('#<=txt_stato.ClientID %>').val() + "|";
            parametri += $('#cmb_stato option:selected').text() + "|";
            parametri += $('#Txt_Note_Indirizzo').val();


            $.ajax({
                type: 'POST',
                url: 'Contatto_Edit.aspx/Aggiungi_Indirizzo',
                data: "{parametri:'" + parametri + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {

                    if (r.d.RispostaOK == true)
                        // Aggiorna la watable indirizzi
                        AggiornaTabIndirizzi(JSON.parse(r.d.RispostaStringa));
                    else {
                        alert(r.d.Errore);
                    }

                }
            });


        }

    });


    // Click su aggiungi Costo (salvataggio)
    $('#btn_aggiungi_costo').click(function () {

        var ok = valida_costo();

        if (ok) {
            // Controllo sulle date di inserimento costi
            var parametri2 = ""

            parametri2 += $('#ddl_UdmCod_Prezzo option:selected').text() + "|";
            parametri2 += $('#Txt_Inizio_Prezzo').val() + "|";
            parametri2 += $('#Txt_Fine_Prezzo').val();

            $.ajax({
                type: 'POST',
                url: 'Contatto_Edit.aspx/Controlla_Costo',
                data: "{parametri:'" + parametri2 + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        //AggiornaTabGerarchia(JSON.parse(r.d.RispostaStringa));
                        aggiungi_costo();
                    else {
                        alert(r.d.Errore);
                    }

                }
            });

        }


    });



    //            // Click su elimina Costo
    //            $('#tabCosti .watable .del_elem').click(function () {

    //                $.ajax({
    //                        type: 'POST',
    //                        url: 'Contatto_Edit.aspx/Elimina_Costo',
    //                        data: "{id:'" + $(this).attr('chiave') + "'}",
    //                        contentType: 'application/json; charset=utf-8',
    //                        cache: false,
    //                        dataType: 'json', async: true,
    //                        success: function (r) {
    //                             AggiornaTabCosti(JSON.parse(r.d));
    //                        }
    //                    });
    //            });






    // Footable al click del tab tab_indirizzi 
    //            $('.tab_indirizzi').click(function () {
    //                var elem_hide = new Array(4,5,6);
    //                var elem_hide_all = new Array(7, 8, 9, 10);

    //                applicaFooTable('tabIndirizzi', elem_hide, elem_hide_all); 
    //            });




    ///////////////////////////////////////

    var fl1 = false
    var fl2 = false
    var fl3 = false
    var fl4 = false
    var fl5 = false

    nascondi_riepilogo_error();

    // Gestione Riepilogo Errori (Validazione)
    $("#Txt_CF").keyup(function () {
        if ($("#Txt_CF").val() != "") {
            $('.voce_1').hide();
            fl1 = true;
        }
        else {
            $('.voce_1').show();
            fl1 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#ImgBtn_CF").click(function () {
        $('.voce_1').hide();
        fl1 = true;

        nascondi_riepilogo_error();
    });



    $("#Txt_Cognome").keyup(function () {
        if ($("#Txt_Cognome").val() != "") {
            $('.voce_2').hide();
            fl2 = true;
        }
        else {
            $('.voce_2').show();
            fl2 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Txt_Nome").keyup(function () {
        if ($("#Txt_Nome").val() != "") {
            $('.voce_3').hide();
            fl3 = true;
        }
        else {
            $('.voce_3').show();
            fl3 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Txt_Piva").keyup(function () {
        if ($("#Txt_Piva").val() != "") {
            $('.voce_4').hide();
            fl4 = true;
        }
        else {
            $('.voce_4').show();
            fl4 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Txt_Rag_Soc").keyup(function () {
        if ($("#Txt_Rag_Soc").val() != "") {
            $('.voce_5').hide();
            fl5 = true;
        }
        else {
            $('.voce_5').show();
            fl5 = false;
        }

        nascondi_riepilogo_error();
    });



    function nascondi_riepilogo_error() {
        if (fl1)
            $('.voce_1').hide();

        if (fl2)
            $('.voce_2').hide();

        if (fl3)
            $('.voce_3').hide();

        if (fl4)
            $('.voce_4').hide();

        if (fl5)
            $('.voce_5').hide();

        if ((fl1) && (fl2) && (fl3) || (fl4) && (fl5))
            $('#div_riepilogo_error').hide();
        else
            $('#div_riepilogo_error').show();
    }


    /////////////////////



    $('#RBL_TipoUtente').trigger("change");

}); // End Ready