function pulisci_form_da_validazione() {

    // Azzero tutte le label custom_val
    $(".custom_val").each(function (i, obj) {
        $(this).parent().find('input').css('border', '1px solid #ccc');
        //$(this).parent().children().css('border-color', '#ccc');
        $(this).remove();
    });

    // Resetto la validazione del tab Dati Personali
    $('.nav-tabs li.active a').find('.error-tab').remove();

}


function controlla_form() {

    var flag = true;
    var n_inv = 0;

    var tipo = $('#RBL_TipoUtente').find('input:checked').val();

    v.resetForm();

    pulisci_form_da_validazione();



    if (tipo == 0) {
        if ($('#Txt_CF').val() == "") {
            $('#Txt_CF').parent().append('<label id="Txt_CF-error" class="custom_val error" for="Txt_CF">Il campo deve essere compilato</label>');
            $('#Txt_CF').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($('#Txt_Cognome').val() == "") {
            $('#Txt_Cognome').parent().append('<label id="Txt_Cognome-error" class="custom_val error" for="Txt_Cognome">Il campo deve essere compilato</label>');
            $('#Txt_Cognome').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($('#Txt_Nome').val() == "") {
            $('#Txt_Nome').parent().append('<label id="Txt_Nome-error" class="custom_val error" for="Txt_Nome">Il campo deve essere compilato</label>');
            $('#Txt_Nome').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        $("#Txt_Piva-error").hide();
        $("#Txt_Rag_Soc-error").hide();
        $(".voce_4").hide();
        $(".voce_5").hide();
        $(".voce_1").show();
        $(".voce_2").show();
        $(".voce_3").show();
    }

    if (tipo == 1) {
        if ($('#Txt_Piva').val() == "") {
            $('#Txt_Piva').parent().append('<label id="Txt_Piva-error" class="custom_val error" for="Txt_Piva">Il campo deve essere compilato</label>');
            $('#Txt_Piva').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($('#Txt_Rag_Soc').val() == "") {
            $('#Txt_Rag_Soc').parent().append('<label id="Txt_Rag_Soc-error" class="custom_val error" for="Txt_Rag_Soc">Il campo deve essere compilato</label>');
            $('#Txt_Rag_Soc').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        $("#Txt_CF-error").hide();
        $("#Txt_Cognome-error").hide();
        $("#Txt_Nome-error").hide();
        $(".voce_1").hide();
        $(".voce_2").hide();
        $(".voce_3").hide();
        $(".voce_4").show();
        $(".voce_5").show();

    }

    if (v.valid() && flag) {
        return true
    }
    else {
        var err_message = "";
        err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
        $('.nav-tabs li.active a').append(err_message);

        return false;
    }


}


function ValidaxSubmit() {

    var flag = controlla_form();

    if (flag) {

        // Salvo in session la tabella Rapporti
        var rap_arr = "";

        for (var i = 0; i < watableRapporti.getData('checked').rows.length; i++) {
            rap_arr += watableRapporti.getData('checked').rows[i]['Cod_Rapporto'] + "|";

        }

        if (rap_arr == "") {
            kendo.alert("Selezionare almeno un rapporto contabile");
            return false;
        }

        $.ajax({
            type: 'POST',
            url: 'Contatto_Edit.aspx/Salva_Rapporti_In_Session',
            data: "{dati: '" + rap_arr + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: false,
            success: function (r) {
            }
        });

        $.ajax({
            type: 'POST',
            url: 'Contatto_Edit.aspx/Set_Comune',
            data: "{comune:'" + $('#ddl_comune').val() + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
            }

        });


        $('#ImgBtn_SalvaTutto').click();

    }

}


function valida_indirizzo() {

    var ok = true

    pulisci_form_da_validazione();


    if ($('#txt_via').val() == "") {
        $('#txt_via').parent().append('<label id="txt_via-error" class="custom_val error" for="txt_via">Il campo deve essere compilato</label>');
        $('#txt_via').parent().children(".required").css('border', '1px solid #D41E1A');
        ok = false;
    }

    if ($('#ddl_provincia').val() == "-1") {
        $('#ddl_provincia').parent().append('<label id="ddl_provincia-error" class="custom_val error" for="ddl_provincia">Il campo deve essere selezionato</label>');
        $('#ddl_provincia').parent().children(".required").css('border', '1px solid #D41E1A');
        ok = false;
    }

    if ($('#ddl_comune').val() == "") {
        $('#ddl_comune').parent().append('<label id="ddl_comune-error" class="custom_val error" for="ddl_comune">Il campo deve essere selezionato</label>');
        $('#ddl_comune').parent().children(".required").css('border', '1px solid #D41E1A');
        ok = false;
    }

    if ($('#txt_cap').val() == "") {
        $('#txt_cap').parent().append('<label id="txt_cap-error" class="custom_val error" for="txt_cap">Il campo deve essere compilato</label>');
        $('#txt_cap').parent().children(".required").css('border', '1px solid #D41E1A');
        ok = false;
    }

    return ok

}


function valida_costo() {

    var ok = true

    pulisci_form_da_validazione();


    if ($('#Txt_Prezzo').val() == "") {
        $('#Txt_Prezzo').parent().append('<label id="Txt_Prezzo-error" class="custom_val error" for="Txt_Prezzo">Il campo deve essere compilato</label>');
        $('#Txt_Prezzo').parent().children(".required").css('border', '1px solid #D41E1A');
        ok = false;
    }

    if ($('#Txt_Inizio_Prezzo').val() == "") {
        $('#Txt_Inizio_Prezzo').parent().append('<label id="Txt_Inizio_Prezzo-error" class="custom_val error" for="Txt_Inizio_Prezzo">Il campo deve essere compilato</label>');
        $('#Txt_Inizio_Prezzo').parent().children(".required").css('border', '1px solid #D41E1A');
        ok = false;
    }

    if ($('#Txt_Fine_Prezzo').val() == "") {
        $('#Txt_Fine_Prezzo').parent().append('<label id="Txt_Fine_Prezzo-error" class="custom_val error" for="Txt_Fine_Prezzo">Il campo deve essere compilato</label>');
        $('#Txt_Fine_Prezzo').parent().children(".required").css('border', '1px solid #D41E1A');
        ok = false;
    }

    return ok

}



//*** Funzione che applica alla WaTable la Footable
function applicaFooTable(tabella, elem_hide, elem_hide_all) {
    //$('#' + tabella + ' table thead tr').find('th:nth-child(3)').attr('data-hide', 'phone, tablet');
    $('#' + tabella + ' table').removeClass('phone');
    $('#' + tabella + ' table').removeClass('breakpoint');
    $('#' + tabella + ' table').removeClass('footable-loaded');
    $('#' + tabella + ' table').removeClass('footable');


    $('#' + tabella + ' table thead tr').find('th:nth-child(1)').attr('data-toggle', 'true');
    //Calcolo dove mettere il bottone x espandere riga
    //            if ($('#' + tabella + ' table').width() <= 700) {
    //                if ($('#' + tabella + ' table').width() <= 480)
    //                    $('#' + tabella + ' table thead tr').find('th:nth-child(2)').attr('data-toggle', 'true');
    //                else
    //                    $('#' + tabella + ' table thead tr').find('th:nth-child(1)').attr('data-toggle', 'true');
    //            }

    jQuery.each(elem_hide, function (i, val) {
        $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'phone, tablet');
    });

    jQuery.each(elem_hide_all, function (i, val) {
        $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'all');
    });


    $('#' + tabella + ' table').footable({
        breakpoints: {
            phone: 480,
            tablet: 700
        }
    });


}

function AggiornaTabIndirizzi(d) {

    var elem_hide = new Array(4, 5, 6);
    var elem_hide_all = new Array(7, 8, 9, 10);
    AgroWA_Table_sistemaDati(d);

    $('#tabIndirizzi').html('');
    watableIndirizzi = $("#tabIndirizzi").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        checkboxes: false,
        tableCreated: function (data) {
            applicaFooTable('tabIndirizzi', elem_hide, elem_hide_all);

        }
        , pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabIndirizzi", watableIndirizzi);
}

function AggiornaTabCosti(d) {

    var elem_hide = new Array(4, 5, 6);
    var elem_hide_all = new Array(7, 8, 9, 10);
    AgroWA_Table_sistemaDati(d);

    //            for(var i =0; i < d.rows.length -1; i++){
    //                $.each(d.rows[i], function( index, value ) {
    //                      if ((index == "Data Inizio") || (index == "Data Fine")){
    //                        value = value.split(' ');
    //                        value = value[0];
    //                        }

    //                });
    //            }


    $('#tabCosti').html('');
    watableCosti = $("#tabCosti").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: true,
        preFill: false,
        tableCreated: function (data) {
            applicaFooTable('tabCosti', elem_hide, elem_hide_all);
        },
        checkboxes: false,
        pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabCosti", watableCosti);
}


function AggiornaTabRapporti(d) {

    var elem_hide = [];
    var elem_hide_all = new Array(4, 5, 6, 7);
    AgroWA_Table_sistemaDati(d);

    $('#tabRapporti').html('');
    watableRapporti = $("#tabRapporti").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        tableCreated: function (data) {
            //applicaFooTable('tabRapporti', elem_hide, elem_hide_all); 
            $('#tabRapporti .watable thead tr.sort th:nth-child(1)').css('width', '15px');
            $('#tabRapporti .watable tbody tr td:nth-child(1)').css('text-align', 'center');


            if ($('#Cmb_CentriAziendali').is(":disabled")) {
                $('#tabRapporti .watable .unique').attr("disabled", true);
                $('#tabRapporti .watable .checkToggle').attr("disabled", true);
            }

        },
        checkboxes: true,
        pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabRapporti", watableRapporti);
}


function generate_CodFisc_click() {
    $.ajax({
        type: "POST",
        url: "Contatto_Edit.aspx/GetRandomCodFisc",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#Txt_CF').val(msg.d);
            $("#Txt_CF").trigger("keyup");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function generate_piva_click() {

    $.ajax({
        type: "POST",
        url: "Contatto_Edit.aspx/GetRandomPiva",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#Txt_Piva').val(msg.d);
            $("#Txt_Piva").trigger("keyup");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


function ModificaIndirizzo(obj) {
    var chiave = $(obj).attr('chiave');
    var i;

    //$('#btn_aggiungi_indirizzo').hide();
    $('#btn_modifica_indirizzo').show();

    for (i = 0; i < watableIndirizzi.getData().rows.length; i++) {
        var q = watableIndirizzi.getData().rows[i];
        if (q.Tool == chiave) {
            $('#ddl_comune').attr('disabled', false);
            $("#txt_via").val(q.Indirizzo);
            $("#txt_frazione").val(q.Frazione);
            $("#txt_cap").val(q.Cap);
            //$("#<=txt_stato.ClientID %>").val(q.Stato);

            var stato = q.Stato;
            if (stato == "&nbsp;" || stato == "") {
                stato = "SELEZIONA";
            }
            else {
                stato = stato.toLowerCase();
                stato = stato.charAt(0).toUpperCase() + stato.slice(1);
            }


            $("#cmb_stato option").each(function () {
                if ($(this).text() == stato) {
                    $(this).attr('selected', true)
                    $(this).parent().selectpicker('refresh');
                }
            });

            $("#cmb_stato").val(q.Stato);
            $("#Txt_Note_Indirizzo").val(q.Note);
            $("#Txt_ProCodIstat").val(q.Provincia_cod)
            $("#Txt_ProvinciaSigla").val(q.Prov)
            $("#Txt_ComCodIstat").val(q.Comune_cod)


            var tipologia = q.Tipologia;
            $("#ddl_tipo_Indirizzo option").each(function () {
                if (tipologia == $(this).text()) {
                    $(this).prop('selected', true);
                    $(this).parent().selectpicker('refresh');
                }

                i = i + 1;
            });

            var prov = q.Prov;
            $("#ddl_provincia option").each(function () {
                if (prov == $(this).val()) {
                    $(this).prop('selected', true);
                    $(this).parent().selectpicker('refresh');
                }

                i = i + 1;
            });


            $.ajax({
                type: 'POST',
                url: 'Contatto_Edit.aspx/Carica_Comuni',
                data: "{provincia:'" + prov + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {

                    $('#ddl_comune').empty();
                    $('#ddl_comune').parent().find('button').removeClass('disabled');
                    $('#ddl_comune').append(r.d[0]);

                    $('#ddl_comune').selectpicker('refresh');

                    var comune = q.Comune.toLowerCase();
                    $("#ddl_comune option").each(function () {
                        if (comune == $(this).text()) {
                            $(this).attr('selected', 'selected');
                        }

                        i = i + 1;
                    });

                    $('#ddl_comune').selectpicker('refresh');

                }
            });




        }
    }

    //                 EliminaCodice(obj);                

}


function aggiungi_costo() {

    var parametri = ""

    parametri += $('#ddl_UdmCod_Prezzo').val() + "|";
    parametri += $('#ddl_UdmCod_Prezzo option:selected').text() + "|";

    parametri += $('#Txt_Inizio_Prezzo').val() + "|";
    parametri += $('#Txt_Fine_Prezzo').val() + "|";
    parametri += $('#Txt_Prezzo').val();



    $.ajax({
        type: 'POST',
        url: 'Contatto_Edit.aspx/Aggiungi_Costo',
        data: "{parametri:'" + parametri + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            // Aggiorna la watable indirizzi
            AggiornaTabCosti(JSON.parse(r.d));

        }
    });



}


var kendoDialog_Cambia_PIVA;
var kendoDialog_Cambia_CF;

function AnnullaCambioCF() {
    kendoDialog_Cambia_CF.close();
}

function AnnullaCambiaPIVA() {
    kendoDialog_Cambia_PIVA.close();
}

function cambiaCF() {

}

function cambiaPIVA() {

}

function inserisciCF() {

    if (kendoDialog_Cambia_CF == undefined) {

        kendoDialog_Cambia_CF = $("#window_cambia_CF").kendoDialog({
            width: "400px",
            title: "Cambia Codice Fiscale",
            closable: true,
            modal: true,
            content: "<row> <div class='col-lg-12 col-md-12 col-xs-12'> <div class='col-lg-6 col-md-6 col-xs-6'> <p>Codice Fiscale:</p> </div> <div class='col-lg-6 col-md-6 col-xs-6'> <input type='text' class='form-control' id='AnnoPratica'> </div> </div> </row>",
            actions: [
                { text: 'Annulla', action: AnnullaCambioCF },
                { text: 'Conferma', primary: true, action: cambiaCF }
            ]
        }).data("kendoDialog");
    } else {

        kendoDialog_Cambia_CF.open();

    }


}

function inserisciPiva() {

    if (kendoDialog_Cambia_PIVA == undefined) {

        kendoDialog_Cambia_PIVA = $("#window_cambia_PIVA").kendoDialog({
            width: "400px",
            title: "Cambia P.IVA",
            closable: true,
            modal: true,
            content: "<row> <div class='col-lg-12 col-md-12 col-xs-12'> <div class='col-lg-6 col-md-6 col-xs-6'> <p>P.Iva:</p> </div> <div class='col-lg-6 col-md-6 col-xs-6'> <input type='text' class='form-control' id='AnnoPratica'> </div> </div> </row>",
            actions: [
                { text: 'Annulla', action: AnnullaCambiaPIVA },
                { text: 'Conferma', primary: true, action: cambiaPIVA }
            ]
        }).data("kendoDialog");

    } else {

        kendoDialog_Cambia_PIVA.open();

    }
}

function EliminaCosto(obj) {
    var id = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Contatto_Edit.aspx/Elimina_Costo',
        data: "{id:'" + id + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            AggiornaTabCosti(JSON.parse(r.d));
        }
    });

}