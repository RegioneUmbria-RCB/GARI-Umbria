var objParametri_Agenda;
var catastoEditResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/Catasto_Edit.aspx.resx"
];

$(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            catastoEditResx.push(readResxFile(resxSinglePath, "Catasto_Edit_jQueryDocReady.js"));
        });
    }

    objParametri_Agenda = JSON.parse(objP_agenda);

    //tooltip
    $('a[data-toggle="tooltip"]').tooltip({
        animated: 'fade',
        placement: 'top',
        container: 'body'
    });


    $("#div_errori_salvataggio").hide();

    $('.tab_classamento').click(function () {
        var elem_hide = new Array(4, 5, 6, 7, 8, 9);
        applicaFooTable('tabClassamenti', elem_hide);
    });

    //$("#<%=Cmb_Provincia.ClientID%>").change(function(){ changePartKey(); });

    $("#Cmb_Comune").change(function () { changePartKey(); });

    $("#Txt_Sezione").change(function () { changePartKey(); });

    $("#Txt_Foglio").change(function () { changePartKey(); });

    $("#Txt_Numero").change(function () { changePartKey(); });

    $("#Txt_Subalterno").change(function () { changePartKey(); });

    var initPossessi;
    var initMacrousi;
    var initZone;
    var initClassamenti;


    if (jsPossessi != "") {
        initPossessi = jsPossessi;
    } else {
        initPossessi = "";
    }
    if (initPossessi != "") {
        AggiornaTabPossessi(initPossessi);
    }

    //carico eventualmente la tabella di Macrousi
    if (jsMacrousi != "") {
        initMacrousi = jsMacrousi;
    } else {
        initMacrousi = "";
    }
    if (initMacrousi != "") {
        AggiornaTabMacrousi(initMacrousi);
    }

    //carico eventualmente la tabella di Zone
    if (jsZone != "") {
        initZone = jsZone;
    } else {
        initZone = "";
    }
    if (initZone != "") {
        AggiornaTabZone(initZone);
    }

    //carico eventualmente la tabella di Classamenti
    if (jsClassamenti != "") {
        initClassamenti = jsClassamenti;
    } else {
        initClassamenti = "";
    }
    if (initClassamenti != "") {
        AggiornaTabClassamenti(initClassamenti);
    }


    if ($("#chk_estero").is(':checked')) {
        $('#Cmb_Provincia').attr("disabled", true);
        $('#Cmb_Comune').attr("disabled", true);
    }


    // Colorazione riga selezionata in watable
    $("body").on("click", ".watable tbody tr td:not('.footable-row-detail-cell')", function () {

        $(".watable tbody tr.success").each(function (i) {
            $(this).removeClass("success");
        });

        // soottolineo la riga selezionata...
        $(this).parent().addClass('success');
    });

    // Gestione click del bottone Aggiungi in tab Possessi
    $('#btn_aggiungi_possesso').click(function (e) {

        var codice = $('#Cmb_TitoloPossesso').val();
        var possesso;
        var superficie = $('#Txt_SupCondotta').val();
        var cod_particella = $('#Txt_CodParticella').val();
        var DataInizio = "";
        var DataFine = "";

        // Controllo se è stato immesso del testo nel input del Cerca
        if (codice != "" && superficie != "") {
            //codice = codice.slice(0, -1);

            DataInizio = $('#TxtValiditaInizio').val();
            DataFine = $('#TxtValiditaFine').val();

            $("#Cmb_TitoloPossesso option").each(function () {
                if (this.value == codice) {
                    //alert(this.value);
                    possesso = this.text;

                    if (possesso.indexOf("'") > -1) {
                        possesso = possesso.replace(/\'/g, ' ');
                    }
                }
            });

            $.ajax({
                type: 'POST',
                url: 'Catasto_Edit.aspx/Aggiungi_Possesso',
                data: "{ codice:'" + codice + "', possesso:'" + possesso + "', superficie:'" + superficie + "', cod_particella:'" + cod_particella + "', DataInizio:'" + DataInizio + "', DataFine:'" + DataFine + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        AggiornaTabPossessi(JSON.parse(r.d.RispostaStringa));
                    else {
                        alert(r.d.Errore);
                    }

                }
            });
        }
    });


    // Gestione click del bottone Aggiungi in tab Macrousi
    $('#btn_aggiungi_macrousi').click(function (e) {

        var codice = $('#Cmb_Macrousi').val();
        var macrouso;
        var superficie = $('#Txt_SupMacrouso').val();
        var DataInizio = "";
        var DataFine = "";

        // Controllo se è stato immesso del testo nel input del Cerca
        if (codice != "" && superficie != "") {
            //codice = codice.slice(0, -1);

            DataInizio = $('#TxtValiditaInizioMacrouso').val();
            DataFine = $('#TxtValiditaFineMacrouso').val();

            $("#Cmb_Macrousi option").each(function () {
                if (this.value == codice) {
                    //alert(this.value);
                    macrouso = this.text;

                    if (macrouso.indexOf("'") > -1) {
                        macrouso = macrouso.replace(/\'/g, ' ');
                    }
                }
            });

            $.ajax({
                type: 'POST',
                url: 'Catasto_Edit.aspx/Aggiungi_Macrouso',
                data: "{ codice:'" + codice + "', macrouso:'" + macrouso + "', superficie:'" + superficie + "', DataInizio:'" + DataInizio + "', DataFine:'" + DataFine + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        AggiornaTabMacrousi(JSON.parse(r.d.RispostaStringa));
                    else {
                        alert(r.d.Errore);
                    }

                }
            });
        }
    });


    // Gestione click del bottone Aggiungi in tab Zone
    $('#btn_aggiungi_zonizzazione').click(function (e) {

        var codice = $('#Cmb_Zone').val();
        var zona;
        var superficie = $('#Txt_SupZona').val();
        var DataInizio = "";
        var DataFine = "";

        // Controllo se è stato immesso del testo nel input del Cerca
        if (codice != "" && superficie != "") {

            DataInizio = $('#TxtValiditaInizioZona').val();
            DataFine = $('#TxtValiditaFineZona').val();

            $("#Cmb_Zone option").each(function () {
                if (this.value == codice) {
                    //alert(this.value);
                    zona = this.text;

                    if (zona.indexOf("'") > -1) {
                        zona = zona.replace(/\'/g, ' ');
                    }
                }
            });

            $.ajax({
                type: 'POST',
                url: 'Catasto_Edit.aspx/Aggiungi_Zona',
                data: "{ codice:'" + codice + "', zona:'" + zona + "', superficie:'" + superficie + "', DataInizio:'" + DataInizio + "', DataFine:'" + DataFine + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        AggiornaTabZone(JSON.parse(r.d.RispostaStringa));
                    else {
                        alert(r.d.Errore);
                    }

                }
            });
        }
    });


    // Gestione click del bottone Aggiungi in tab Classamento
    $('#btn_aggiungi_classamento').click(function (e) {

        var porzione = $('#TxtPorzione').val();
        var classeCatasto = $('#TxtClasseCatasto').val();

        var codice = $('#Cmb_QualitaCatasto').val();
        var qualita;
        var superficie = $('#TxtSupClass').val();
        var RedditoDom = $('#TxtRedditoDom').val();
        var RedditoAgr = $('#TxtRedditoAgr').val();

        // Controllo se è stato immesso del testo nel input del Cerca
        if (codice != "") {

            $("#Cmb_QualitaCatasto option").each(function () {
                if (this.value == codice) {
                    //alert(this.value);
                    qualita = this.text;

                    if (qualita.indexOf("'") > -1) {
                        qualita = qualita.replace(/\'/g, ' ');
                    }
                }
            });

            $.ajax({
                type: 'POST',
                url: 'Catasto_Edit.aspx/Aggiungi_Classamento',
                data: "{ codice:'" + codice + "', qualita:'" + qualita + "', superficie:'" + superficie + "', porzione:'" + porzione + "', classeCatasto:'" + classeCatasto + "', RedditoDom:'" + RedditoDom + "', RedditoAgr:'" + RedditoAgr + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        AggiornaTabClassamenti(JSON.parse(r.d.RispostaStringa));
                    else {
                        alert(r.d.Errore);
                    }

                }
            });
        }
    });


    // Quando cambia Provincia, mostro Comuni relativi
    $('#Cmb_Provincia').change(function (e) {


        provincia = $('#Cmb_Provincia').val();

        if ($('#Cmb_Provincia').val() != "") {
            $('#Cmb_Comune').parent().children().attr("disabled", false);

            $.ajax({
                type: 'POST',
                url: 'Catasto_Edit.aspx/Carica_Comuni',
                data: "{provincia:'" + provincia + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#Cmb_Comune').empty();
                    $('#Cmb_Comune').append(r.d[0]);
                    //                            $('#<=Txt_ProCodIstat.ClientID %>').val(r.d[1]);
                    //                            $('#<=Txt_ProvinciaSigla.ClientID %>').val(provincia);                       
                    $('.selectpicker').selectpicker('refresh');
                }
            });
        }
        else {
            // Disabilito i comuni se vuoto
            $('#Cmb_Comune').empty();
            $('#Cmb_Comune').parent().children('.input-group-btn').attr("disabled", true);
            $('.selectpicker').selectpicker('refresh');
        }
    });


    // Quando cambia Comune
    $('#Cmb_Comune').change(function (e) {

        comune = $('#Cmb_Comune').val();
        //$("#<=ddl_comune.ClientID %> select").val(comune);
        $("#Cmb_Comune > option").each(function () {
            if (this.value == comune) {
                //$(this).attr('selected', 'selected');
                $.ajax({
                    type: 'POST',
                    url: 'Catasto_Edit.aspx/Set_Comune',
                    data: "{comune:'" + comune + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                    }
                });
            }
        });

        //$('#<=Txt_ComCodIstat.ClientID %>').val(comune.substring(3, 6));

    });



    var fl1 = true
    var fl2 = true
    var fl3 = false
    var fl4 = false

    nascondi_riepilogo_error();

    $("#Cmb_Provincia").change(function () {
        if ($("#Cmb_Provincia").val() != "") {
            $('.voce_1').hide();
            fl1 = true;
        }
        else {
            $('.voce_1').show();
            fl1 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Cmb_Comune").change(function () {
        if ($("#Cmb_Comune").val() != "") {
            $('.voce_2').hide();
            fl2 = true;
        }
        else {
            $('.voce_2').show();
            fl2 = false;
        }

        nascondi_riepilogo_error();
    });


    // Gestione Riepilogo Errori (Validazione)
    $("#Txt_Foglio").keyup(function () {
        if ($("#Txt_Foglio").val() != "") {
            $('.voce_3').hide();
            fl3 = true;
        }
        else {
            $('.voce_3').show();
            fl3 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Txt_Numero").change(function () {
        if ($("#Txt_Numero").val() != "") {
            $('.voce_4').hide();
            fl4 = true;
        }
        else {
            $('.voce_4').show();
            fl4 = false;
        }

        nascondi_riepilogo_error();
    });

    $('#TxtSup_Ettari').change(function () {
        AggiornaSupCondotta();
    });
    $('#TxtSup_Are').change(function () {
        AggiornaSupCondotta();
    });
    $('#TxtSup_Centiare').change(function () {
        AggiornaSupCondotta();
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

        if ((fl1) && (fl2) && (fl3) && (fl4))
            $('#div_riepilogo_error').hide();
        else
            $('#div_riepilogo_error').show();

    }


    /////////////////////
    if (objParametri_Agenda.Tipo_Operazione === "1") {
        // Gestione check particella ESTERA
        $("#chk_estero").click(function () {
            // Modifico i valori di Prov e Com + disabilito
            if ($("#chk_estero").is(':checked')) {
                $('#Cmb_Provincia').val('00');
                $('#Cmb_Provincia').selectpicker('refresh');
                $('#Cmb_Provincia').attr('disabled', true);
                $('#TxtCodProvincia').text('000');

                $.ajax({
                    type: 'POST',
                    url: 'Catasto_Edit.aspx/Carica_Comuni',
                    data: "{provincia:'00'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: false,
                    success: function (r) {
                        //alert(r.d);
                        $('#Cmb_Comune').empty();
                        $('#Cmb_Comune').append(r.d[0]);
                        //                            $('#<=Txt_ProCodIstat.ClientID %>').val(r.d[1]);
                        //                            $('#<=Txt_ProvinciaSigla.ClientID %>').val(provincia);                       
                        $('.selectpicker').selectpicker('refresh');
                    }
                });

                $('#Cmb_Comune').val('00000000');
                $('#Cmb_Comune').selectpicker('refresh');
                $('#Cmb_Comune').attr("disabled", true);
                $('#TxtCodComune').text('000');

            }
            else {
                //$('#<=Cmb_Provincia.ClientID %>').parent().find('button').removeClass('disabled');
                $('#Cmb_Provincia').attr('disabled', false);
                $('#Cmb_Provincia').selectpicker('refresh');
                $('#Cmb_Comune').attr("disabled", false);
                $('#Cmb_Comune').selectpicker('refresh');
            }

        });
    }
    AggiornaSupCondotta();

    popolaGrigliaMetodiProduzione("grigliaMetodiProduzione");
});