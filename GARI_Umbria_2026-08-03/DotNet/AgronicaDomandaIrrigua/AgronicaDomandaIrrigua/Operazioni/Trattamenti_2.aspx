<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Trattamenti_2.aspx.vb"
    MasterPageFile="~/Master/OperazioneBootstrap.master" ValidateRequest="false"
    EnableEventValidation="false" Inherits="AgronicaDomandaIrrigua.Trattamenti_2" meta:resourcekey="PageResource1" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<%@ MasterType VirtualPath="~/Master/OperazioneBootstrap.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentOperazioniHeader" runat="server">
    <style type="text/css">
        .full_w {
            width: 100%;
        }

        .sfondogiallo:hover {
            background: #F6A828 !important;
        }

        .radiusBorder {
            border-radius: 15px;
        }
    </style>
    <script type="text/javascript">

        //utilizzate per i controlli
        var id_HD_Buffer_Min = "<%= HD_Buffer_Min.ClientID%>";
        var id_HD_Buffer_Max = "<%= HD_Buffer_Max.ClientID%>";
        var id_HD_Buffer_Min_Tmp = "<%= HD_Buffer_Min_Tmp.ClientID%>";
        var id_HD_Buffer_Max_Tmp = "<%= HD_Buffer_Max_Tmp.ClientID%>";
        var id_HD_Perc_Abb_Min = "<%= HD_Perc_Abb_Min.ClientID%>";

        function ValidaxSubmit() {

            //rimuovo quanto disabilitato per poter ottenere i dati lato server:
            //http://stackoverflow.com/questions/7357256/disabled-form-inputs-do-not-appear-in-the-request

            //click per salvataggio
            $("#<%=Master.Property_ImgBtn_Salva.ClientID %>").click();

        }


        $(document).keypress(function (e) {
            if (e.which == 13) {
                $('#<%=btn_cerca.ClientID %>').click();
                return false;
            }
        });

        function Verifica_Tasto_Premuto() {
            switch (window.event.keyCode) {
                case 13:
                    //invio 
                    if ($("*:focus").attr('id').toString() == "<%=Txt_Formulati.ClientID %>") {
                        $('#<%=btn_cerca.ClientID %>').click();
                    } else { return false; }
                    break;
            }
        }

        //funzione per aggiungere il colore sulle righe che hanno il DPI quando il filtro dei disciplinari non presenta alcun tipo di disciplinare selezionato
        //toDelete, deprecata per utilizzo griglia kendo.
        function MettiColori() {

            return;

            if ($('.ComboDisciplinari').val() == 0) {
                //metto il giallo solo dove c'è il dpi
                $('.classeDPI').each(function () {
                    //if ($(this).html() != '&nbsp;') {
                    if ($(this).html() != 0) {
                        //alert('DPI ' + $(this).html());
                        $(this).parent().addClass("sfondogiallo");
                        $(this).parent().find('input[type="checkbox"]').prop('disabled', true);
                        colorato = true;
                    } else {
                        $(this).parent().removeClass("sfondogiallo");
                        $(this).parent().find('input[type="checkbox"]').removeProp('disabled');
                    }
                });
                //regolamenti
                $('.classeRegolamenti').each(function () {

                    //alert($(this).html());
                    //Reg. CE 834/07 (eEx. Reg. CE 2092/91)
                    //if ($(this).html().indexOf('834/07') >= 0) {
                    if ($(this).html() == 4) {
                        //alert('BIO ' + $(this).html());
                        $(this).parent().addClass("sfondogiallo");
                        $(this).parent().find('input[type="checkbox"]').prop('disabled', true);
                    } else {
                        if (colorato = false) {
                            $(this).parent().removeClass("sfondogiallo");
                            $(this).parent().find('input[type="checkbox"]').removeProp('disabled');
                        }
                    }
                });
            }
            else {
                $(document).removeClass("sfondogiallo");
                console.log('MettiColori - false');
                //$('.classeDPI').parent().removeClass("sfondogiallo");


                //se c'è il bio come regolamento ci deve essere bio anche nella combo disciplinare
                //regolamenti

                if ($('.ComboDisciplinari').val() != -2) {
                    $('.classeRegolamenti').each(function () {


                        //if ($(this).html().indexOf('834/07') >= 0) {
                        if ($(this).html() == 4) {
                            alert(ComboDisciplinari);
                            //alert('BIO ' + $(this).html());
                            $(this).parent().parent().addClass("sfondogiallo");

                        } else {

                        }
                    });
                }
            }
        }


        function CambiaFormulati() {
            $("#<%=BTN_ComboFormulati.ClientID %>").click();
        }

        function CambiaFertilizzanti() {
            $("#<%=BTN_ComboFertilizzanti.ClientID %>").click();
        }

        function CambiaSementi() {
            $("#<%=BTN_ComboSementi.ClientID %>").click();
        }

        //sul cambio della soglia apro la combo delle dosi se ce n'è più di una.
        $(document).on('change', '#<%=ddl_soglieAvversita.ClientID%>', function () { 
            if ($('#<%=ComboEtichetta.ddl_Dosi.clientID%> option').length > 1)
                $('#<%=ComboEtichetta.ddl_Dosi.clientID%>').parent().find('button').click();
        });

        //etichetta
        $(document).on('change', '.ComboEtichetta', function () {
            $("#<%=Btn_ComboEtichetta.ClientID %>").click();
            WaitFrame.show();
        });







        function ImpostaAvversitaProdotto() {

            var appoggio_P = $('#cella_prodotto');
            var appoggio_A = $("#<%=riga_avversita.ClientID %>");

            //controllo il valore
            if ($(".ComboFiltriAggiuntiviAgenda").parent().children('select').val() == 1) {
                //prodotti avversita
                $('#contenitore_1').append(appoggio_P);
                $('#contenitore_1').append(appoggio_A);
            }
            else {
                //avversità prodotto
                $('#contenitore_1').append(appoggio_A);
                $('#contenitore_1').append(appoggio_P);
            }

        }


        $(document).on('change', '.ComboEpocheDPI', function () {
            $("#<%=Btn_ComboEpocheDPI.ClientID %>").click();
            WaitFrame.show();
        });


        function MovimentiMagazzino() {

            let visualizzazione_mode = 1; //0=ricerca (default) 1=dettaglio
            let tab_richiesto = 1; //0=giacenze (default) 1=movimenti

            let piva = $("#<%=hdPiva.ClientID %>").val();

            let sa_cod = parseInt($('.ComboCentroAziendale').val());
            let fabbricato_cod = $('.ComboMagazzini').val();

            let lav_cod = parseInt($('.ComboOperazioni').val());

            let elem_cod = 0;
            let pro_cod = 0;
            let mat_cod = 0;
            let stringa = '';

            switch (lav_cod) {
                case 71: case 2: case 160: case 68:
                    elem_cod = <%=AgronicaCoreDataProvider.CostantiPersonalizzate.SEMENTI %>;
                    stringa = $('#<%=ComboSementi.ClientId %>').children().val();
                    if (stringa == null) {
                        return false;
                    }
                    if (stringa.toString().length > 0) {
                        mat_cod = stringa.split('§')[0];
                    }
                    break;
                case 14: case 156: case 124: case 123: case 26: case 106:
                    elem_cod = <%=AgronicaCoreDataProvider.CostantiPersonalizzate.FERTILIZZANTI %>;
                    stringa = $('#<%=ComboFertilizzanti.ClientId %>').children().val();
                    if (stringa == null) {
                        return false;
                    }
                    if (stringa.toString().length > 0) {
                        pro_cod = stringa.split('/')[0];
                    }
                    break;
                case 74: case 18: case 155: case 13:
                    elem_cod = <%=AgronicaCoreDataProvider.CostantiPersonalizzate.FORMULATI %>;
                    stringa = $('select.ComboFormulati').val();
                    if (stringa == null) {
                        return false;
                    }
                    if (stringa.toString().length > 0) {
                        pro_cod = stringa.split("£")[0];
                    }
                    break;
            }

            let data;
            if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
                data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
            } else {
                data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
            }


            let param = JSON.stringify({
                visualizzazione_mode: visualizzazione_mode,
                tab_richiesto: tab_richiesto,
                piva: piva,
                sa_cod: sa_cod,
                fabbricato_cod: fabbricato_cod,
                lav_cod: lav_cod,
                elem_cod: elem_cod,
                pro_cod: pro_cod,
                mat_cod: mat_cod,
                data: data
            });

            $.ajax({
                type: "POST",
                url: "trattamenti_2.aspx/apriGestioneMagazzini",
                data: param,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    apriGestioneMagazzini(msg.d);
                }
            });

        }



        function apriGestioneMagazzini(url) {
            $(document.body).append('<div id="GestioneMagazziniWindow"></div>');
            $('#GestioneMagazziniWindow').kendoWindow({
                title: "Dettaglio Movimenti",
                modal: true,
                resizable: true,
                iframe: true,
                width: "80%",
                height: "80%",
                content: url,
                close: function () {
                    $('#GestioneMagazziniWindow').kendoWindow('destroy');
                }
            }).data("kendoWindow").center();
        }

        function MovimentiContabili() {

            let visualizzazione_mode = 1; //0=ricerca (default) 1=dettaglio
            let tab_richiesto = 1; //0=giacenze (default) 1=movimenti

            let piva = $("#<%=hdPiva.ClientID %>").val();

            let sa_cod = parseInt($('.ComboCentroAziendale').val());
            let fabbricato_cod = $('.ComboMagazzini').val();

            let lav_cod = parseInt($('.ComboOperazioni').val());

            let elem_cod = 0;
            let pro_cod = 0;
            let mat_cod = 0;
            let stringa = '';

            switch (lav_cod) {
                case 71: case 2: case 160: case 68:
                    elem_cod = <%=AgronicaCoreDataProvider.CostantiPersonalizzate.SEMENTI %>;
                    //stringa = $('#<%=ComboSementi.ClientId %>').children().val();
                    //if (stringa == null) {
                        //return false;
                    //}
                    //if (stringa.toString().length > 0) {
                        //mat_cod = stringa.split('§')[0];
                    //}
                    break;
                case 14: case 156: case 124: case 123: case 26: case 106:
                    elem_cod = <%=AgronicaCoreDataProvider.CostantiPersonalizzate.FERTILIZZANTI %>;
                    //stringa = $('#<%=ComboFertilizzanti.ClientId %>').children().val();
                    //if (stringa == null) {
                    //    return false;
                    //}
                    //if (stringa.toString().length > 0) {
                    //    pro_cod = stringa.split('/')[0];
                    //}
                    break;
                case 74: case 18: case 155: case 13:
                    elem_cod = <%=AgronicaCoreDataProvider.CostantiPersonalizzate.FORMULATI %>;
                    //stringa = $('select.ComboFormulati').val();
                    //if (stringa == null) {
                    //    return false;
                    //}
                    //if (stringa.toString().length > 0) {
                    //    pro_cod = stringa.split("£")[0];
                    //}
                    break;
            }

            let data;
            if ($(id_txt_DataOperazione).data("kendoDateTimePicker") !== undefined) {
                data = $(id_txt_DataOperazione).data("kendoDateTimePicker").value();
            } else {
                data = $(id_txt_DataOperazione).data("kendoDatePicker").value();
            }


            let param = JSON.stringify({
                visualizzazione_mode: visualizzazione_mode,
                tab_richiesto: tab_richiesto,
                piva: piva,
                sa_cod: sa_cod,
                fabbricato_cod: fabbricato_cod,
                lav_cod: lav_cod,
                elem_cod: elem_cod,
                pro_cod: pro_cod,
                mat_cod: mat_cod,
                data: data
            });

            $.ajax({
                type: "POST",
                url: "trattamenti_2.aspx/apriGestioneMovimentiContabili",
                data: param,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    apriGestioneMovimentiContabili(msg.d);
                }
            });

        }

        function apriGestioneMovimentiContabili(url) {
            $(document.body).append('<div id="GestioneMovimentiContabiliWindow"></div>');
            $('#GestioneMovimentiContabiliWindow').kendoWindow({
                title: "Dettaglio Movimenti Contabili",
                modal: true,
                resizable: true,
                iframe: true,
                width: "95%",
                height: "95%",
                content: url,
                close: function () {
                    $('#GestioneMovimentiContabiliWindow').kendoWindow('destroy');
                }
            }).data("kendoWindow").center();
        }










        function Info() {

            //var classificazioni = $('.Cmb_FormulatoClassificazioni').val();

            stringa = $('select.ComboFormulati').val();
            if (stringa == null) {
                return false;
            }
            if (stringa.toString().length > 0) {
                var splitted = stringa.split("£");
                pro_cod = splitted[0];

                //vecchia gestione TargetUrl='../../Popup_Informativi/Popup_Formulato.aspx?fr_cod=';
                //                TargetUrl = $('.IndirizzoProfitosan').val() + '/x_Tunnel/Tunnel_GiasOnLine.aspx?p=1&f=';
                TargetUrl = $('.IndirizzoProfitosan').val();
                Caratteristiche = "dialogWidth:520px; dialogHeight:890px; status:no; center:yes; edge:raised; help:no;"

                //vecchia gestione  a = window.showModalDialog(TargetUrl + pro_cod,"",Caratteristiche)	
                window.open(TargetUrl + pro_cod, 'profitosan', '');
            }
        }


        function ApriRilievo(lav_cod) {


            var param = '';
            var impiantiSelezionati = $(id_hdKendo_Impianti_Selezione).val();

            if (lav_cod === 0) {

                var soglia = $('#<%=ddl_soglieAvversita.ClientID%>').val();

                if (soglia == null) {
                    return false;
                }

                if (soglia.toString().length > 0) {

                    var splitted = soglia.split("_");

                    if (splitted.length = 5) {
                        lav_cod = splitted[4];
                    }
                    else {
                        lav_cod = splitted[3];
                    }


                    param = kendo.stringify({ "lav_cod": lav_cod, "dpi_cod": $(".ComboDisciplinari").val(), "av_cod": $("#<%= ddl_avversita.ClientID %>").val(), "udm_cod": $("#<%= ddl_soglieAvversita.ClientID %>").val(), "impiantiSelezionati": impiantiSelezionati });


                }

            }

            else {

                param = kendo.stringify({ "lav_cod": lav_cod, "dpi_cod": 0, "av_cod": 0, "udm_cod": 0, "impiantiSelezionati": impiantiSelezionati });

            }

            if (param != '') {

                ajaxAgronica("Trattamenti_2.aspx/ApriRilievo",
                    param,
                    function (risposta) {
                        var risp = risposta.RispostaStringa;

                        //apertura in finestra modale..
                        $("#PaginaGeneric").attr("src", risp);
                        $("#iFrameGeneric h4 span").html("Rilievo");
                        $("#iFrameGeneric").modal('toggle');
                        $('#iFrameGeneric').on('hidden.bs.modal', function () {
                            RilievoRientroProssimoStep();
                            if (lav_cod === 79) {
                                $('#<%=Master.Property_AggiornaGrigliaImpianti.ClientID%>').click();
                            }
                        });

                    }, null);

            }




        }


        function RilievoRientro() {
            $("#PaginaGeneric").attr("src", "");
            $("#iFrameGeneric").modal('hide');
        }

        //Grilli: per modifica Attività
        //function ChiudiModale() {
        //    $("#PaginaGeneric").attr("src", "");            
        //    $("#iFrameGeneric").modal('hide');            
        //}

        function RilievoRientroProssimoStep() {
            ajaxAgronica("Trattamenti_2.aspx/ApriRilievoRientra",
                "{ }",
                function (risposta) {

                }, null);
        }


        ///////////////////////////////////////////
        /////////////ABILITA DISABIITA/////////////
        ///////////////////////////////////////////
        function elementExists(id) {
            var el = document.getElementById(id);

            if (el != null) {
                return true;
            }

            return false;
        }

        function AcquaTotChecked() {
            return $('#<%=rblAcqua_Tot.ClientId %>').is(':checked');
        }


        function Abilita_Disabilita_ACQUA() {

            if (AcquaTotChecked()) {
                if ($('#<%=rblAcqua_Tot.ClientId %>').is(':disabled')) {
                    Abilita_Disabilita_Text(".AcquaTot", false);
                    Abilita_Disabilita_Text(".AcquaHa", false);

                } else {
                    Abilita_Disabilita_Text(".AcquaTot", true);
                    Abilita_Disabilita_Text(".AcquaHa", false);
                }

            }
            else {
                if ($('#<%=rblAcqua_Tot.ClientId %>').is(':disabled')) {
                    Abilita_Disabilita_Text(".AcquaTot", false);
                    Abilita_Disabilita_Text(".AcquaHa", false);
                } else {
                    Abilita_Disabilita_Text(".AcquaTot", false);
                    Abilita_Disabilita_Text(".AcquaHa", true);
                }
            }
        }


        function DoseHAChecked() {
            return $('#<%=rbl_DoseHA.ClientId %>').is(':checked');
        }

        function QtaTOTChecked() {
            return $('#<%=rbl_QtaTot.ClientId %>').is(':checked');
        }

        function Abilita_Disabilita_DOSI() {

            var dose_HA, dose_HL, doseTot_HA, doseTot_HL;
            dose_HA = false;
            dose_HL = false;
            doseTot_HA = false;
            doseTot_HL = false;

            if (DoseHAChecked()) {
                //DOSE HA
                if (QtaTOTChecked()) { doseTot_HA = true; }
                else { dose_HA = true; }
            }
            else {
                //DOSE HL
                if (QtaTOTChecked()) { doseTot_HA = true; }
                else { dose_HL = true; }
            }

            if (dose_HA == false) {
                Abilita_Disabilita_Text(".qtaDoseHa", false);
            } else {
                Abilita_Disabilita_Text(".qtaDoseHa", true);
            }

            if (dose_HL == false) {
                Abilita_Disabilita_Text(".qtaDoseHl", false);
            } else {
                Abilita_Disabilita_Text(".qtaDoseHl", true);
            }

            if (doseTot_HA == false) {
                Abilita_Disabilita_Text(".qtaDoseTot", false);
            } else {
                Abilita_Disabilita_Text(".qtaDoseTot", true);
            }

        }

        /////////////////////////////////////////////////////////
        ////////CALCOLO COSTI ACCESSORI AUTOMATICO///////////////

        function CalcolaCostiAccessori() {
            var flag = $(".CostiAperti").children().is(':checked');
            if (flag == true) {
                var sup = $('#<%=Txt_SupTrattata.ClientId %>').val().replace(',', '.');
                if (sup > 0) {
                    aggiornaCostiSuServer(sup);
                    $('.GridViewCostiAccessoriVisibili').find('.UdmCosti').each(function () {
                        var udm = $(this).val();
                        if (udm == 1) {
                            InserisciQtaCosti($(this), sup)
                            var CostoUnitario = ValoreCostoUnitario($(this));
                            if (CostoUnitario != 0) {
                                var tot = CostoUnitario * sup
                                InserisciCosto($(this), tot);
                            }
                        }
                        else if (udm == 2) {

                            var minuti = Number($(this).parent().parent().children('.ore').html());
                            minuti = minuti * 60;
                            minuti = minuti + Number($(this).parent().parent().children('.minuti').html());
                            minuti = minuti * sup;

                            //Grilli: ho aggiunto l'IF perché altrimenti ripulisce sempre tutto anche se non è stato impostato
                            if (minuti > 0) {
                                var ore = Math.floor(minuti / 60);
                                var resto = minuti - (ore * 60);
                                // InserisciQtaCosti
                                var OreDecimal = ore.toString() + "," + (Math.floor(resto * 100 / 60)).toString()
                                $(this).parent().parent().find('.QtaCosti').val(OreDecimal);

                                var CostoUnitario = ValoreCostoUnitario($(this));
                                if (CostoUnitario != 0) {
                                    var tot = CostoUnitario * Number(OreDecimal.replace(",", "."));
                                    InserisciCosto($(this), tot);
                                }
                            }
                        };
                    });
                };
            };
        }

        function InserisciQtaCosti(Oggetto, valore) {
            var sup = roundNumber(valore, 4) + "";
            sup = sup.replace(".", ",");
            $(Oggetto).parent().parent().find('.QtaCosti').val(sup);
        }

        function InserisciCosto(Oggetto, valore) {
            var tot = roundNumber(valore, 4) + "";
            tot = tot.replace(".", ",");
            $(Oggetto).parent().parent().find('.Costo').html(tot);
        }

        function ValoreCostoUnitario(Oggetto) {
            return $(Oggetto).parent().parent().children('.CostoUnitario').html().replace(',', '.');
        }

        function ValoreUDMCostoUnitario(Oggetto) {
            return $(Oggetto).parent().parent().children('.UdmCosti').val();
        }

        function aggiornaCostiSuServer(sup) {
            return;
            var Attesa;
            $.ajax({
                type: "POST",
                url: "trattamenti_2.aspx/Update_Sup_Costi",
                data: "{ sup: '" + sup + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
        /////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        ///////////////////////////////////
        ////////RICAVA INSERISCI///////////
        ///////////////////////////////////

        function SupTrattata() {
            return $('#<%=Txt_SupTrattata.ClientId %>').val().replace(',', '.');
        }
        function InserisciSupTrattata(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Suptrattata.ClientId %>').val(app);
        }

        function SupTotale() {
            return $('#<%=Txt_SupSelezionata.ClientId %>').val().replace(',', '.');
        }
        function InserisciSupTotale(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_SupSelezionata.ClientId %>').val(app);
        }

        function AcquaTot() {

            $('#<%=Hidden_Dose_Acqua_Tot_Reale.ClientId %>').val($('#<%=Txt_Acqua_Tot.ClientId %>').val().replace(',', '.'));
            return $('#<%=Hidden_Dose_Acqua_Tot_Reale.ClientId %>').val().replace(',', '.');
        }
        function InserisciAcquaTot(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Acqua_Tot.ClientId %>').val(app);
            $('#<%=Hidden_Dose_Acqua_Tot_Reale.ClientId %>').val(valore.toString().replace(".", ","));
        }

        function AcquaHA() {
            //return $('#<%=Txt_Acqua_Ha.ClientId %>').val().replace(',', '.');
            $('#<%=Hidden_Dose_Acqua_Ha_Reale.ClientId %>').val($('#<%=Txt_Acqua_Ha.ClientId %>').val().replace(',', '.'));
            return $('#<%=Hidden_Dose_Acqua_Ha_Reale.ClientId %>').val().replace(',', '.');
        }
        function InserisciAcquaHA(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Acqua_Ha.ClientId %>').val(app);
            $('#<%=Hidden_Dose_Acqua_Ha_Reale.ClientId %>').val(valore.toString().replace(".", ","));
        }

        function DoseHA() {
            //return $('#<%=Txt_Dose_Ha.ClientId %>').val().replace(',', '.');
            $('#<%=Hidden_Dose_Ha_Reale.ClientId %>').val($('#<%=Txt_Dose_HA.ClientId %>').val().replace(',', '.'));
            return $('#<%=Hidden_Dose_Ha_Reale.ClientId %>').val().replace(',', '.');
        }
        function InserisciDoseHA(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Dose_Ha.ClientId %>').val(app);
            $('#<%=Hidden_Dose_Ha_Reale.ClientId %>').val(valore.toString().replace(".", ","));
        }

        function DoseHL() {
            //return $('#<%=Txt_Dose_HL.ClientId %>').val().replace(',', '.');
            $('#<%=Hidden_Dose_Hl_Reale.ClientId %>').val($('#<%=Txt_Dose_HL.ClientId %>').val().replace(',', '.'));
            return $('#<%=Hidden_Dose_Hl_Reale.ClientId %>').val().replace(',', '.');
        }
        function InserisciDoseHL(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Dose_HL.ClientId %>').val(app);
            $('#<%=Hidden_Dose_Hl_Reale.ClientId %>').val(valore.toString().replace(".", ","));
        }

        function TotHA() {
            //return $('#<%=Txt_DoseTot_Ha.ClientId %>').val().replace(',', '.');
            $('#<%=Hidden_Dose_Tot_Reale.ClientId %>').val($('#<%=Txt_DoseTot_HA.ClientId %>').val().replace(',', '.'));
            return $('#<%=Hidden_Dose_Tot_Reale.ClientId %>').val().replace(',', '.');
        }

        function InserisciTotHA(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_DoseTot_Ha.ClientId %>').val(app);
            $('#<%=Hidden_Dose_Tot_Reale.ClientId %>').val(valore.toString().replace(".", ","));
        }



        function SupCalcolata() {
            return $('#<%=Txt_SupCalcolata.ClientId %>').val().replace(',', '.');
        }
        function InserisciSupCalcolata(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_SupCalcolata.ClientId %>').val(app);
        }

        function ConfiguraOperazione() {
            var lavcod = $('.ComboOperazioni').val();
            //acqua
            switch (parseInt(lavcod)) {
                case 14:
                case 156:
                case 124:
                case <%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_SEMINA %>:
                case <%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_SOD_SEDDING %>:
                case <%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_SOVESCIO %>:
                case <%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_TRAPIANTO %>:
                    $('.Acqua').hide();
                    break;
                default:
                    $('.Acqua').show();
                    break;
            }
            //tratt polverulento
            switch (parseInt(lavcod)) {
                case 74:
                    $('.Div_NoAcqua').show();
                    break;
                default:
                    $('.Div_NoAcqua').hide();
                    break;
            }

            //avv
            switch (parseInt(lavcod)) {
                case 74: case 18: case 155: case 13:
                    $('.Avversita').show();
                    break;
                default:
                    $('.Avversita').hide();
                    break;
            }
            //dose etichetta
            switch (parseInt(lavcod)) {
                case 14: case 156: case 124: case 123: case 26: case 106:
                    $('.DoseEtichetta').hide();
                    break;
                default:
                    $('.DoseEtichetta').show();
                    break;
            }

        }




        function AggiornaACQUA() {
            var _SupTrattata = SupTrattata();
            var valore = 0;
            //if (AcquaTotChecked() && AcquaHA() == 0) {
            if (AcquaTotChecked()) {
                //ACQUA TOTALE
                var _AcquaTot = AcquaTot();
                if (_AcquaTot != 0 && _AcquaTot != null) {
                    if (_SupTrattata != 0 && _SupTrattata != null) {
                        valore = _AcquaTot / _SupTrattata;
                    }
                }
                InserisciAcquaHA(valore);
            }
            else {
                //HA
                var _AcquaHA = AcquaHA();
                if (_AcquaHA != 0 && _AcquaHA != null) {
                    if (_SupTrattata != 0 && _SupTrattata != null) {
                        valore = _SupTrattata * _AcquaHA;
                    }
                }
                InserisciAcquaTot(valore);
            }

            AggiornaDOSI();

            Btn_AggiornaDosiGriglia_click();
        }


        function GridVew_Dosi_ClientMod_toString(sGridVew_Dosi_ClientMod) {
            $("#<%= GridVew_Dosi_ClientMod.ClientID %>").val(JSON.stringify(sGridVew_Dosi_ClientMod));
        }

        function Btn_AggiornaDosiGriglia_click() {

            var sGridVew_Dosi_ClientMod = [];

            $("#<%=GridView_Dosi.ClientID %> .rigaImpianti").each(function () {

                var frCod = $(this).find(".Fr_Cod").text();

                var xHa_Hl = $(this).find(".Ha_Hl").text();
                var Acqua_tot = parseFloat($("#<%= Hidden_Dose_Acqua_Tot_Reale.ClientID %>").val().replace(",", "."));
                var Sup_tot = parseFloat($("#<%= Txt_SupTrattata.ClientID %>").val().replace(",", "."));
                var Sup_calc = $(this).find(".Sup_Calcolata").text();
                var lotto = $(this).find(".Lotto").text();

                if (xHa_Hl != "") {

                    var Ha_Hl = parseInt(xHa_Hl);

                    if ($.isNumeric(Sup_calc.replace(",", ".")) && parseFloat(Sup_calc.replace(",", ".")) > 0)
                        Sup_tot = parseFloat(Sup_calc.replace(",", "."));

                    Btn_AggiornaDoseRiga_click(sGridVew_Dosi_ClientMod, frCod, $(this), Ha_Hl, Acqua_tot, Sup_tot, lotto);


                }

            });

            GridVew_Dosi_ClientMod_toString(sGridVew_Dosi_ClientMod);
        }


        function Btn_AggiornaDoseRiga_click(sGridVew_Dosi_ClientMod, frCod, Riga, Ha_Hl, Acqua_tot, Sup_tot, lotto) {

            var dose_ha;
            var dose_hl;
            var dose_ha_reale;
            var dose_hl_reale;
            var dose_tot;
            var dose_tot_reale;

            switch (Ha_Hl) {

                case 0:

                    dose_hl = parseFloat(Riga.find(".Dose_HL").text().replace(",", "."));
                    dose_hl_reale = parseFloat(Riga.find(".Dose_HL_Reale").text().replace(",", "."));

                    var dose_tot_hl = (parseFloat(Riga.find(".Dose_HL_Reale").text().replace(",", ".")) * 10000 * Acqua_tot) / 10000;
                    dose_tot = dose_tot_hl.toFixed(4).replace(".", ",");
                    Riga.find(".Qta_Tot").text(dose_tot);
                    dose_tot_reale = dose_tot_hl.toString().replace(".", ",");
                    Riga.find(".Qta_Tot_Reale").text(dose_tot_reale);

                    dose_ha = (dose_tot_hl / Sup_tot).toFixed(4).replace(".", ",");
                    Riga.find(".Dose_HA").text(dose_ha);
                    dose_ha_reale = (dose_tot_hl / Sup_tot).toString().replace(".", ",");
                    Riga.find(".Dose_HA_Reale").text(dose_ha_reale);

                    sGridVew_Dosi_ClientMod.push({
                        fr_cod: frCod,
                        riga: i,
                        dose_tot: dose_tot,
                        dose_ha: dose_ha,
                        dose_hl: dose_hl,
                        dose_ha_reale: dose_ha_reale,
                        dose_hl_reale: dose_hl_reale,
                        dose_tot_reale: dose_tot_reale,
                        lotto: lotto
                    });
                    break;

                case 1:

                    dose_ha = parseFloat(Riga.find(".Dose_HA").text().replace(",", "."));
                    dose_ha_reale = parseFloat(Riga.find(".Dose_HA_Reale").text().replace(",", "."));

                    var dose_tot_ha = (parseFloat(Riga.find(".Dose_HA_Reale").text().replace(",", ".")) * 10000 * Sup_tot) / 10000;
                    dose_tot = dose_tot_ha.toFixed(4).replace(".", ",");
                    Riga.find(".Qta_Tot").text(dose_tot);
                    dose_tot_reale = dose_tot_ha.toString().replace(".", ",");
                    Riga.find(".Qta_Tot_Reale").text(dose_tot_reale);

                    if (Acqua_tot == 0) {
                        dose_hl = 0
                        dose_hl_reale = 0;
                    }
                    else {
                        dose_hl = (dose_tot_ha / Acqua_tot).toFixed(4).replace(".", ",");
                        dose_hl_reale = (dose_tot_ha / Acqua_tot).toString().replace(".", ",");
                    }

                    Riga.find(".Dose_HL").text(dose_hl);
                    Riga.find(".Dose_HL_Reale").text(dose_hl_reale);

                    sGridVew_Dosi_ClientMod.push({
                        fr_cod: frCod,
                        riga: i,
                        dose_tot: dose_tot,
                        dose_ha: dose_ha,
                        dose_hl: dose_hl,
                        dose_ha_reale: dose_ha_reale,
                        dose_hl_reale: dose_hl_reale,
                        dose_tot_reale: dose_tot_reale,
                        lotto: lotto
                    });
                    break;

                case -1:

                    var Dose_Etichetta_Value = Riga.find(".Dose_Etichetta_Value").text().trim();
                    var ArrayDosiValue = Dose_Etichetta_Value.split("<br>");
                    var ArrayDose = [];
                    var perHaHl = 0;
                    var Udm_Cod_Max_scomposta = 0;
                    var Udm_Cod = 0;
                    var valoreQta_Tot;

                    var lavcod = $('.ComboOperazioni').val();

                    switch (parseInt(lavcod)) {

                        case 14: case 156: case 124: case 123: case 26: case 106:

                            dose_ha = parseFloat(Riga.find(".Dose_HA").text().replace(",", "."));
                            dose_ha_reale = parseFloat(Riga.find(".Dose_HA_Reale").text().replace(",", "."));

                            var dose_tot_ha = (parseFloat(Riga.find(".Dose_HA_Reale").text().replace(",", ".")) * 10000 * Sup_tot) / 10000;
                            dose_tot = dose_tot_ha.toFixed(4).replace(".", ",");
                            Riga.find(".Qta_Tot").text(dose_tot);
                            dose_tot_reale = dose_tot_ha.toString().replace(".", ",");
                            Riga.find(".Qta_Tot_Reale").text(dose_tot_reale);

                            if (Acqua_tot == 0) {
                                dose_hl = 0
                                dose_hl_reale = 0;
                            }
                            else {
                                dose_hl = (dose_tot_ha / Acqua_tot).toFixed(4).replace(".", ",");
                                dose_hl_reale = (dose_tot_ha / Acqua_tot).toString().replace(".", ",");
                            }

                            Riga.find(".Dose_HL").text(dose_hl);
                            Riga.find(".Dose_HL_Reale").text(dose_hl_reale);

                            sGridVew_Dosi_ClientMod.push({
                                fr_cod: frCod,
                                riga: i,
                                dose_tot: dose_tot,
                                dose_ha: dose_ha,
                                dose_hl: dose_hl,
                                dose_ha_reale: dose_ha_reale,
                                dose_hl_reale: dose_hl_reale,
                                dose_tot_reale: dose_tot_reale,
                                lotto: lotto
                            });

                            break;

                        default:

                            if (Dose_Etichetta_Value != '') {

                                if (ArrayDosiValue.length > 0) {

                                    //for (var i = 0; i < ArrayDosiValue.length; i++) {
                                    for (var i = 0; i < 1; i++) {

                                        ArrayDose = ArrayDosiValue[i].split("$");

                                        if (ArrayDose !== undefined) {
                                            if (ArrayDose.length > 0) {
                                                Udm_Cod = ArrayDose[3];
                                                var udmScomposta = ScomponiUdm(parseInt(Udm_Cod));

                                                switch (udmScomposta.perHa_hl) {
                                                    case 2121:

                                                        dose_hl = parseFloat(Riga.find(".Dose_HL").text().replace(",", "."));
                                                        dose_hl_reale = parseFloat(Riga.find(".Dose_HL_Reale").text().replace(",", "."));

                                                        var dose_tot_hl = (parseFloat(Riga.find(".Dose_HL").text().replace(",", ".")) * 10000 * Acqua_tot) / 10000;
                                                        dose_tot = dose_tot_hl.toFixed(4).replace(".", ",");
                                                        Riga.find(".Qta_Tot").text(dose_tot);
                                                        dose_tot_reale = dose_tot_hl.toString().replace(".", ",");
                                                        Riga.find(".Qta_Tot_Reale").text(dose_tot_reale);

                                                        dose_ha = (dose_tot_hl / Sup_tot).toFixed(4).replace(".", ",");
                                                        Riga.find(".Dose_HA").text(dose_ha);
                                                        dose_ha_reale = (dose_tot_hl / Sup_tot).toString().replace(".", ",");
                                                        Riga.find(".Dose_HA_Reale").text(dose_ha_reale);

                                                        sGridVew_Dosi_ClientMod.push({
                                                            fr_cod: frCod,
                                                            riga: i,
                                                            dose_tot: dose_tot,
                                                            dose_ha: dose_ha,
                                                            dose_hl: dose_hl,
                                                            dose_ha_reale: dose_ha_reale,
                                                            dose_hl_reale: dose_hl_reale,
                                                            dose_tot_reale: dose_tot_reale,
                                                            lotto: lotto
                                                        });
                                                        break;

                                                    case 2123:

                                                        dose_ha = parseFloat(Riga.find(".Dose_HA").text().replace(",", "."));
                                                        dose_ha_reale = parseFloat(Riga.find(".Dose_HA_Reale").text().replace(",", "."));

                                                        var dose_tot_ha = (parseFloat(Riga.find(".Dose_HA").text().replace(",", ".")) * 10000 * Sup_tot) / 10000;
                                                        dose_tot = dose_tot_ha.toFixed(4).replace(".", ",");
                                                        Riga.find(".Qta_Tot").text(dose_tot);
                                                        dose_tot_reale = dose_tot_ha.toString().replace(".", ",");
                                                        Riga.find(".Qta_Tot_Reale").text(dose_tot_ha);

                                                        if (Acqua_tot == 0) {
                                                            dose_hl = 0
                                                            dose_hl_reale = 0;
                                                        }
                                                        else {
                                                            dose_hl = (dose_tot_ha / Acqua_tot).toFixed(4).replace(".", ",");
                                                            dose_hl_reale = (dose_tot_ha / Acqua_tot).toString().replace(".", ",");
                                                        }

                                                        Riga.find(".Dose_HL").text(dose_hl);
                                                        Riga.find(".Dose_HL_Reale").text(dose_hl_reale);

                                                        sGridVew_Dosi_ClientMod.push({
                                                            fr_cod: frCod,
                                                            riga: i,
                                                            dose_tot: dose_tot,
                                                            dose_ha: dose_ha,
                                                            dose_hl: dose_hl,
                                                            dose_ha_reale: dose_ha_reale,
                                                            dose_hl_reale: dose_hl_reale,
                                                            dose_tot_reale: dose_tot_reale,
                                                            lotto: lotto
                                                        });
                                                        break;
                                                }

                                            }
                                        }

                                    }
                                }
                            }

                            break;
                    }



            }

        }

        function AggiornaDOSI() {

            var _SupTrattata = SupTrattata();
            var _Acqua = AcquaTot();


            if (DoseHAChecked()) {

                var lavcod = $('.ComboOperazioni').val();
                var isSemina = false;


                switch (parseInt(lavcod)) {
                    case 71: case 2: case 160: case 68:
                        isSemina = true;
                        break;
                }

                var frazionaSemina = false;
                $('.TipoSemina').children().children().each(function () {
                    if ($(this).children().children('input').is(":checked") && $(this).children().children('input').val() === "50") {
                        frazionaSemina = true;
                    }
                });

                if (isSemina && frazionaSemina) {

                    var _DoseTotaleHA = TotHA();
                    var _DoseHA = DoseHA();
                    var _SupCalcolata = SupCalcolata();

                    if (QtaTOTChecked()) {
                        if (parseFloat(_SupCalcolata) != 0 && parseFloat(_DoseTotaleHA) != 0) {
                            _DoseHA = _DoseTotaleHA / _SupCalcolata;
                            InserisciDoseHA(_DoseHA);
                        }
                    }
                    else {
                        if (parseFloat(_SupCalcolata) != 0 && parseFloat(_DoseHA) != 0) {
                            _DoseTotaleHA = _SupCalcolata * _DoseHA;
                            InserisciTotHA(_DoseTotaleHA);
                        }
                    }

                    InserisciDoseHL(0);

                }


                else {

                    //LAVORO A HA
                    if (QtaTOTChecked()) {
                        //LAVORO A TAL QUALE
                        if (_SupTrattata > 0) {
                            var _DoseTotaleHA = TotHA();
                            var _DoseHA = _DoseTotaleHA / _SupTrattata;
                            InserisciDoseHA(_DoseHA);
                            //VERIFICO L'acqua
                            if (_Acqua > 0) {
                                var _DoseHL = _DoseTotaleHA / _Acqua;
                                InserisciDoseHL(_DoseHL);
                            }
                            if (_Acqua == 0) {
                                InserisciDoseHL(0);
                            }
                        }
                    }
                    else {
                        //LAVORO A DOSE
                        if (_SupTrattata > 0) {
                            var _DoseHA = DoseHA();
                            var _DoseTotaleHA = _SupTrattata * _DoseHA;

                            InserisciTotHA(_DoseTotaleHA);
                            //VERIFICO L'acqua
                            if (_Acqua > 0) {
                                var _DoseHL = _DoseTotaleHA / _Acqua;
                                InserisciDoseHL(_DoseHL);
                            }
                            if (_Acqua == 0) {
                                InserisciDoseHL(0);
                            }
                        }
                    }

                }




            }
            else {
                //LAVORO A HL 
                if (QtaTOTChecked()) {
                    //LAVORO A TAL QUALE
                    if (_Acqua > 0) {
                        var _DoseTotaleHL = TotHA();
                        var _DoseHL = _DoseTotaleHL / _Acqua;
                        InserisciDoseHL(_DoseHL);
                        //VERIFICO L'acqua
                        if (_SupTrattata > 0) {
                            var _DoseHA = _DoseTotaleHL / _SupTrattata;
                            InserisciDoseHA(_DoseHA);
                        }
                    }
                    if (_Acqua == 0) {
                        InserisciDoseHL(0);
                    }
                }
                else {
                    //LAVORO A DOSE
                    if (_Acqua > 0) {
                        var _DoseHL = DoseHL();
                        var _DoseTotaleHL = _Acqua * _DoseHL;
                        //VERIFICO L'acqua
                        if (_SupTrattata > 0) {
                            InserisciTotHA(_DoseTotaleHL);
                            var _DoseHA = _DoseTotaleHL / _SupTrattata;
                            InserisciDoseHA(_DoseHA);
                        }
                    }
                    if (_Acqua == 0) {
                        InserisciDoseHA(0);
                    }
                }
            }
        }

        function AcquaTot_Keyup() {
            AggiornaACQUA();
        }
        function AcquaHA_Keyup() {
            AggiornaACQUA();
        }

        function AggiornaDopo_SupTrattata() {
            //Aggiorno l'acqua
            AggiornaACQUA();

            Btn_AggiornaDosiGriglia_click();

        }


        function DoPostBack_ControlliSiNo(key) {
            if (key == 'Salva') {
                $("#<%=HiddenVarie.ClientID %>").val("OK");
                $("#<%=Master_Operazione.Property_ImgBtn_Salva.ClientID %>").click();

            }
            if (key == 'Prosegui') {
                $("#<%=HiddenVarie.ClientID %>").val("OK");
                $("#<%=ImgBtn_DoseInserisci.ClientID %>").click();
            }
        }

        $(document).on('click', '#InserisciDose', function () { $("#<%=ImgBtn_DoseInserisci.ClientID %>").click(); });

        $(document).ready(function () {

            ConfiguraOperazione();

        });


        //solo x distr ammendanti con direttiva nitrati
        function AggiornaEfficienza() {

            var lavcod = $('.ComboOperazioni').val();

            switch (parseInt(lavcod)) {

                case 124:

                    var Regolamento = $('.ComboDisciplinari').val();
                    var Array = Regolamento.split('/');
                    if (Array.length > 1) {
                        var TipoReg = Regolamento.split('/')[1];
                        if (TipoReg == 2) {

                            var valore = $('#<%=ComboFertilizzanti.ClientId %>').children().val();
                            var FerCod = valore.split('/')[0];
                            var Epoca = $('#<%=ComboEpoche.ClientId %>').children('select').val();
                            var TipoAllevamento = $('#<%=Cmb_Allevamenti.ClientId %>').val();
                            var RegolamentoCod = Regolamento.split('/')[0];
                            var Dose = 1;
                            var EffCod = valore.split('$')[6];
                            var Udm = $('#<%=Cmb_UdM.ClientId %>').val();
                            var qta = $('#<%=Txt_Dose_HA.ClientId %>').val().replace(',', '.');
                            var N = $('#<%=Txt_N.ClientId %>').val().replace(',', '.');
                            var qta_trasformata = 0;
                            var qta_N = 0;
                            var ClassiTessiture = [];
                            var VegCod = $("#<%= Master.Property_ComboSpecie.ddl_Specie.ClientID%>").val();

                            let impiantiSelezionati = $(id_hdKendo_Impianti_Selezione).val();
                            let impSelez = JSON.parse(impiantiSelezionati);
                            impSelez.forEach(function (imp) {
                                if (imp.ListaClassiTessitura !== undefined) {
                                    imp.ListaClassiTessitura.forEach(function (ct) {
                                        if (!ClassiTessiture.includes(ct)) {
                                            ClassiTessiture.push(ct);
                                        }
                                    });
                                }
                            });


                            switch (Udm) {
                                case "2": //kg
                                    qta_trasformata = qta
                                    break;
                                case "29": //l
                                    qta_trasformata = qta
                                    break;
                                case "4": //q
                                    qta_trasformata = qta * 100
                                    break;
                                case "304": //t
                                    qta_trasformata = qta * 1000
                                    break;
                                case "19": //mc 
                                    qta_trasformata = qta * 1000
                                    break;
                            }
                            qta_N = qta_trasformata * N / 100;
                            if (qta_N > 125) {
                                Dose = 2;
                            }

                            if (TipoAllevamento === undefined) {
                                TipoAllevamento = 0;
                            }

                            let veg_cod = VegCod
                            if (VegCod.split("/").length > 0 && !isNaN(parseInt(VegCod.split("/")[0]))) {
                                veg_cod = (VegCod.split("/")[0])
                            }

                            let param = kendo.stringify({ "EffCod": EffCod, "Epoca": Epoca, "TipoAllevamento": TipoAllevamento, "Dose": Dose, "RegolamentoCod": RegolamentoCod, "ClassiTessiture": ClassiTessiture, "VegCod": veg_cod });


                            //loading
                            $('#WaitFrame').show();

                            $.ajax({
                                type: "POST",
                                url: "Trattamenti_2.aspx/AggiornaEfficienza",
                                data: param,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {
                                    $('#WaitFrame').hide();
                                    $('#<%=Txt_Efficienza.ClientId %>').val(msg.d);

                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    $('#WaitFrame').hide();
                                    MessaggioErrore(xhr.status + "<br />" + thrownError);
                                }
                            });

                        }
                    }



                    break;
            }


        }

        $(document).on('change', '.ComboEpoche', function () {
            $("#<%=Btn_ComboEpoche.ClientID %>").click();
            WaitFrame.show();
        });

        $(document).on('keyup', '.Txt_N', function () {
            N_Keyup();
        });






        $(document).on('click', '#CheckDettaglioDoseConsentita', function () { $("#<%=ImgBtn_DettaglioDoseConsentita.ClientID %>").click(); });


        //Aggiorno N Utile 
        function N_Keyup() {
            AggiornaN();
        }

        function AggiornaN() {
            var _N = TitoloN();
            var _Efficienza = Efficienza();
            var valore = 0;

            if (_N != 0 && _N != null) {
                if (_Efficienza != 0 && _Efficienza != null) {
                    valore = _N * _Efficienza;
                }
            }
            InserisciNUtile(valore);

        }

        function TitoloN() {
            return $('#<%=Txt_N.ClientId %>').val().replace(',', '.');
        }
        function Efficienza() {
            return $('#<%=Txt_Efficienza.ClientId %>').val().replace(',', '.');
        }
        function InserisciNUtile(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_N_Utile.ClientId %>').val(app);
        }









    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlacexTRATTAMENTI" runat="server">

    <input id="IndirizzoProfitosan" type="hidden" class="IndirizzoProfitosan" runat="server" />
    <input type="hidden" id="HiddenVarie" runat="server" />
    <input type="hidden" id="hf_app_ricetta_operazione_id" runat="server" />
    <input type="hidden" id="hdPiva" runat="server" />
    <div class="col-md-12 nopadding" runat="server" id="pannelloFiltriRicerca" style="display: none">
        <asp:UpdatePanel ID="UpdatePanelFiltriAggiuntivi" runat="server">
            <ContentTemplate>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <asp:Label ID="lblFiltriRicerca" runat="server" meta:metaresourcekey="lblFiltriRicercaResource1"
                                CssClass="input-group-addon alert-info">Filtri Ricerca</asp:Label>
                            <cc1:ComboFiltriAggiuntiviAgenda ID="ComboFiltriAggiuntiviAgenda1" runat="server"
                                Bootstrap="true" meta:resourcekey="ComboFiltriAggiuntiviAgenda1Resource1" CssClass="ComboFiltriAggiuntiviAgenda" />
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>


    <div class="col-md-12 nopadding" runat="server" id="pannelloAttivita" style="display: none">
        <asp:UpdatePanel ID="UpdatePanelAttivita" runat="server">
            <ContentTemplate>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <asp:Label ID="lblAttivita" runat="server" CssClass="input-group-addon alert-info">Attivita</asp:Label>
                            <asp:DropDownList ID="ComboAttivita" runat="server" CssClass="selectpicker" data-live-search="true" AutoPostBack="false"></asp:DropDownList>
                            <asp:Button ID="BTN_Attivita" runat="server" Text="Button" Style="display: none" />
                        </div>
                    </div>

                    <%--Grilli: per modifica Attività
                    <div class="form-group" style="display:flex;">
                        <asp:panel ID="pnlBtnAttivita" runat="server" CssClass="input-group">
                            <asp:Label ID="lblAttivita" runat="server" CssClass="input-group-addon alert-info">Attività</asp:Label>
                            <asp:DropDownList ID="ComboAttivita" runat="server" CssClass="selectpicker" AutoPostBack="false"></asp:DropDownList>  
                            <asp:Button ID="BTN_Attivita" runat="server" Text="Button" Style="display: none" />
                            <asp:Button ID="BTN_RicaricaComboAttivita" runat="server" Text="Button" Style="display: none" />
                        </asp:panel>
                        <asp:panel id="pnlCreaNuovaAttivita" class="col-lg-4 col-md-4 col-xs-12" runat="server" Visible="false" style="padding-right:0px">
                            <div class="btn btn-success" style="width: -webkit-fill-available;" id="btnCreaNuovaAttivita" onclick="btnCreaNuovaAttivita_click()" >Crea Nuova Attività</div>
                        </asp:panel>
                    </div>--%>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="col-md-12 nopadding" runat="server" id="pannelloDescrizioneAttivita" style="display: none">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <asp:Label ID="Label1" runat="server" CssClass="input-group-addon alert-info">Descrizione</asp:Label>
                            <input type="text" runat="server" style="min-width: 250px;" class="form-control" id="Txt_AttivitaDes" />
                        </div>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="BTN_Attivita" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>


</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentOperazioniContenuti" runat="server">
    <div class="col-md-12">
        <asp:UpdatePanel ID="upSeg" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <div id="dSegnalazioni" style="padding: 10px">
                    <b>
                        <asp:Label ID="lErroriSegnalazioni" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </b>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="BTN_ChangeData" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:Button ID="BTN_ComboFiltriAggiuntiviAgenda" runat="server" Style="display: none"
                meta:resourcekey="BTN_ComboFiltriAggiuntiviAgendaResource1" />
        </ContentTemplate>
    </asp:UpdatePanel>


    <div class="jumbotron" style="padding-left: 15px; padding-right: 15px;">
        <div class="col-md-12">

            <asp:UpdatePanel ID="UpdateSuperfici" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-6 col-xs-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <asp:Label ID="lblSupHaSelezionata" runat="server" meta:resourcekey="lblSupHaSelezionataResource1"
                                            CssClass="input-group-addon alert-info">Sup. [ha] Selezionata</asp:Label>
                                        <input type="text" runat="server" class="form-control SommaSuperficie" id="Txt_SupSelezionata"
                                            readonly="readonly" disabled="disabled" value="0" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class=" col-md-6 col-xs-12">
                            <div class="form-horizonta">
                                <div class="form-group">
                                    <div class="input-group">
                                        <asp:Label ID="lblSupHaTrattata" runat="server" meta:resourcekey="lblSupHaTrattataResource1"
                                            CssClass="input-group-addon alert-info">Sup. [ha] Trattata</asp:Label>
                                        <input type="text" runat="server" class="form-control SommaSuperficieTrattata" id="Txt_SupTrattata"
                                            value="0" />
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="row">

            <asp:UpdatePanel ID="UpdatePanel_Prodotti" runat="server">
                <ContentTemplate>

                    <!-- Semina: opzioni di salvataggio -->
                    <asp:Panel runat="server" ID="pnlSeminaOpzioniSalvataggio" class="col-md-12" Visible="false">

                        <div class="row ">
                            <div class="col-md-12">
                                <h4><i class="fa fa-cogs"></i>Opzioni di Salvataggio</h4>
                            </div>
                        </div>

                        <div class="row" style="margin-bottom: 20px; padding-left: 15px; padding-right: 15px;">
                            <div class="col-md-12">
                                <asp:RadioButtonList ID="RBL_Tipo_Semina" runat="server" CssClass="form-control TipoSemina" AutoPostBack="true">
                                    <asp:ListItem Text="Registra Solo Semina/Trapianto" Value="30"></asp:ListItem>
                                    <asp:ListItem Text="Registra Semina/Trapianto ed Aggiorna l'anagrafica dell'Appezzamento/Impianto (Specie, Varietà e Convenzionale/BIO)" Value="40"></asp:ListItem>
                                    <asp:ListItem Text="Fraziona gli Appezzamenti in base ai Lotti delle Materie Prime e Registra le Semine/Trapianti sui nuovi Appezzamenti/Impianti" Value="50"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>

                    </asp:Panel>

                    <!-- epoca -->

                    <div class="col-md-12" runat="server" id="riga_epoca_trattamento">

                        <div class="row">
                            <div class="col-md-12 col-xs-12">
                                <div class="form-horizonta">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label id="Lbl_Epoca" runat="server" class="input-group-addon alert-info">
                                            </label>
                                            <cc1:ComboEpocheDPI ID="ComboEpocheDPI1" runat="server" meta:resourcekey="ComboEpocheDPI1Resource1" CssClass="ComboEpocheDPI"
                                                Bootstrap="true" />
                                            <asp:Button ID="Btn_ComboEpocheDPI" runat="server" Style="display: none" meta:resourcekey="Btn_ComboEpocheDPIResource1" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>



                    <div id="contenitore_1">
                    </div>

                    <!-- Prodotto -->

                    <div class="col-md-12" id="cella_prodotto">

                        <div class="row">
                            <div class="col-md-12">
                                <h4><i class="fa fa-search"></i>
                                    <asp:Localize meta:resourcekey="RicercaProdotto" runat="server">Ricerca Prodotto da utilizzare</asp:Localize></h4>
                            </div>
                        </div>

                        <!-- formulati e testo ricerca -->
                        <div class="row">
                            <div class="col-md-8 col-xs-12">
                                <asp:TextBox ID="Txt_Formulati" runat="server" CssClass="form-control" meta:resourcekey="Txt_FormulatiResource1" placeholder="inserisci un testo per velocizzare la ricerca in banca dati (per nome/per codice registrazione/ se vuoto ricerca tutti ammessi)"></asp:TextBox>
                            </div>
                            <div class="col-md-2 col-xs-6">

                                <div class="btn btn-info btn_per_load" id="imgCerca_Prodotti" onclick="cercaProdotti();" style="width: 100%">
                                    <i class="fa fa-search"></i>
                                    <span>
                                        <asp:Localize meta:resourcekey="Cerca2" runat="server">Cerca</asp:Localize></span>
                                </div>
                                <asp:Button ID="btn_cerca" runat="server" Text="Cerca" Style="display: none" />
                                <span id="spn_chk_VisualizzaGiacenzeZero" style="float: left; margin-right: 5px; margin-top: 5px;">
                                    <asp:CheckBox ID="CB_VisualizzaGiacenzeZero" runat="server" aria-label="..." />
                                </span>
                                <label aria-describedby="spn_chk_VisualizzaGiacenzeZero" id="lbl_chk_VisualizzaGiacenzeZero"
                                    class="form-control" for="<%=CB_VisualizzaGiacenzeZero.ClientID %>" style="border: 0 !important; -webkit-box-shadow: none !important; -moz-box-shadow: none !important; box-shadow: none !important;">
                                    <asp:Localize meta:resourcekey="lbl_chk_VisualizzaGiacenzeZeroResource1" runat="server">Visualizza anche le Giacenze 0</asp:Localize>
                                </label>
                            </div>
                            <div class="col-md-2 col-xs-6 text-right" >
                                <div class="btn btn-info full_w" id="btn_cerca_movimenti" onclick="MovimentiContabili();"
                                    runat="server">
                                    <i class="fa fa-paperclip"></i>
                                    <span class="hidden-none margin-r">
                                        <asp:Localize meta:resourcekey="CercaAllega" runat="server">Cerca</asp:Localize>
                                    </span>
                                </div>
                                <asp:Button ID="Btn_MovContabili" runat="server" Style="display: none;"  />
                                <input type="hidden" runat="server" id="hfRigheMovContabili"  />
                            </div>
                        </div>

                        <!-- Prodotti -->

                        <div class="row">
                            <div class="col-md-12 col-xs-12">
                                <h4 style="margin-bottom: -15px;">
                                    <asp:Localize meta:resourcekey="RisultatiRicercaProdotto" runat="server">Risultati Ricerca Prodotto</asp:Localize>
                                    <label id="Lbl_Num_Formulati" runat="server" class="danger"></label>
                                    <label id="lbl_DettaglioFertilizzanti" runat="server" class="danger" meta:resourcekey="lblDettaglioFertilizzanti"></label>
                                </h4>
                                <br />

                            </div>
                        </div>
                        <div class="row Formulati" id="riga_formulati" runat="server">
                            <div class="col-md-8 col-xs-12">
                                <cc1:ComboFormulati ID="ComboFormulati" runat="server" class="classComboFormulati"
                                    Bootstrap="true" meta:resourcekey="ComboFormulatiResource1" />
                                <asp:Button ID="BTN_ComboFormulati" runat="server" Text="Button" Style="display: none"
                                    CssClass="btn_per_load" meta:resourcekey="BTN_ComboFormulatiResource1" /><br />
                            </div>
                            <div class="col-md-2 col-xs-12">
                                <div class="btn btn-info full_w" id="ImgBtn_Info" onclick="Info();"
                                    runat="server">
                                    <img src="../AB_Immagini/icone24/profitosan.png" style="height: 20px;" alt="Vedi su Profitosan" />
                                    <span class="hidden-none margin-r">
                                        <asp:Label ID="lblBancheDati" runat="server" meta:resourcekey="lblBancheDatiResource1"></asp:Label>
                                    </span>
                                </div>
                            </div>
                            <div class="col-md-2 col-xs-12">
                                <div class="btn btn-info full_w" id="DivDettagliMagazzinoFormulati" onclick="MovimentiMagazzino();"
                                    runat="server">
                                    <span class="hidden-none margin-r">
                                        <asp:Localize meta:resourcekey="MovimentiMagazzino" runat="server">Movimenti Magazzino</asp:Localize>
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div class="row Fertilizzanti" id="riga_fertilizzanti" runat="server">
                            <div class="col-md-10 col-xs-12">
                                <cc1:ComboFertilizzanti ID="ComboFertilizzanti" runat="server" Bootstrap="true"
                                    class="classComboFertilizzanti" meta:resourcekey="ComboFertilizzantiResource1" />
                                <asp:Button ID="BTN_ComboFertilizzanti" runat="server" Text="Button"
                                    Style="display: none" meta:resourcekey="BTN_ComboFertilizzantiResource1" />
                            </div>
                            <div class="col-md-2 col-xs-12">
                                <div class="btn btn-info full_w" id="DivDettagliMagazzinoFertilizzanti" onclick="MovimentiMagazzino();"
                                    runat="server">
                                    <span class="hidden-none margin-r">
                                        <asp:Localize meta:resourcekey="MovimentiMagazzino" runat="server">Movimenti Magazzino</asp:Localize>
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div class="row Sementi" id="riga_sementi" runat="server">
                            <div class="col-md-10 col-xs-12">
                                <cc1:ComboSementi ID="ComboSementi" runat="server" Bootstrap="true"
                                    class="classComboSementi" meta:resourcekey="ComboSementiResource1" />
                                <asp:Button ID="BTN_ComboSementi" runat="server" Text="Button"
                                    Style="display: none" meta:resourcekey="BTN_ComboSementiResource1" />
                            </div>
                            <div class="col-md-2 col-xs-12">
                                <div class="btn btn-info full_w" id="DivDettagliMagazzinoSementi" onclick="MovimentiMagazzino();"
                                    runat="server">
                                    <span class="hidden-none margin-r">
                                        <asp:Localize meta:resourcekey="MovimentiMagazzino" runat="server">Movimenti Magazzino</asp:Localize>
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div class="row" id="Riga_NoteProdotto" runat="server" visible="false">
                            <div class="col-md-12 col-xs-12">
                                <label id="Lbl_NoteProdotto" runat="server" style="color: red; font-size: small"></label>
                            </div>
                        </div>


                    </div>


                    <!-- ANIMALI -->
                    <div class="col-md-12" id="riga_animali" runat="server" style="padding-top: 15PX;">
                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-group">
                                    <div class="input-group">
                                        <%--      <asp:Label ID="Label1" runat="server" CssClass="input-group-addon alert-info"></asp:Label>--%>
                                        <asp:DropDownList ID="Cmb_Allevamenti" runat="server" CssClass="selectpicker"
                                            AutoPostBack="True">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- EPOCA -->
                    <div class="col-md-12" id="riga_epoca_fertilizzazione" runat="server" style="padding-top: 15PX;">
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="lblEpoca" runat="server" meta:resourcekey="lblEpocaResource1" CssClass="input-group-addon alert-info">Epoca</asp:Label>
                                            <cc1:ComboEpocheFertilizzazione ID="ComboEpoche" runat="server" bootstrap="true" CssClass="ComboEpoche" />
                                            <asp:Button ID="Btn_ComboEpoche" runat="server" Style="display: none" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">

                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="lblEfficienza" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lblEfficienzaResource1">Efficienza</asp:Label>
                                            <asp:TextBox ID="Txt_Efficienza" runat="server" CssClass="form-control" Style="min-width: 50px; pointer-events: none; background-color: lightgray"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="lblNUtile" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lblNUtileResource1">N Utile</asp:Label>
                                            <asp:TextBox ID="Txt_N_Utile" runat="server" CssClass="form-control" Style="min-width: 50px; pointer-events: none; background-color: lightgray"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- APPORTI -->
                    <div class="col-md-12" id="riga_apporti" runat="server" style="padding-top: 15PX;">

                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <label runat="server" id="lbl_n_calcolato" style="display: none">(*) media pesata dei valori inseriti in fase di carico</label>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <label runat="server" id="lbl_p_calcolato" style="display: none">(*) media pesata dei valori inseriti in fase di carico</label>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <label runat="server" id="lbl_k_calcolato" style="display: none">(*) media pesata dei valori inseriti in fase di carico</label>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <label runat="server" id="lbl_cu_calcolato" style="display: none">(*) media pesata dei valori inseriti in fase di carico</label>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="Label3" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lbl_NResource1">N</asp:Label>
                                            <asp:TextBox ID="Txt_N" runat="server" CssClass="form-control Txt_N " Style="min-width: 50px; pointer-events: none; background-color: lightgray"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="Label4" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lbl_P2O5Resource1">P2O5</asp:Label>
                                            <asp:TextBox ID="Txt_P2O5" runat="server" CssClass="form-control" Style="min-width: 50px; pointer-events: none; background-color: lightgray"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="Label5" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lblK2OResource1">K2O</asp:Label>
                                            <asp:TextBox ID="Txt_K2O" runat="server" CssClass="form-control" Style="min-width: 50px; pointer-events: none; background-color: lightgray"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="Label6" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lblCuResource1">Cu</asp:Label>
                                            <asp:TextBox ID="Txt_Cu" runat="server" CssClass="form-control" Style="min-width: 50px; pointer-events: none; background-color: lightgray"></asp:TextBox>
                                            <asp:TextBox ID="Txt_MgO" runat="server" CssClass="form-control" Style="min-width: 50px; pointer-events: none; background-color: lightgray; display: none"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                    <!-- Avversità -->

                    <div class="col-md-12 Avversita" id="riga_avversita" runat="server">

                        <asp:Button ID="EventoAggiornamentoAvversita" runat="server" Style="display: none;"
                            CssClass="btn_per_load" meta:resourcekey="EventoAggiornamentoAvversitaResource1" />
                        <div class="row">
                            <div class="col-md-12">

                                <h4><i class="fa fa-bug"></i>
                                    <asp:Label ID="lbl_avversita" runat="server" meta:resourcekey="lbl_avversitaResource1">Avversità / Gruppi Avversità</asp:Label>
                                </h4>

                                <asp:DropDownList ID="ddl_avversita" runat="server" AutoPostBack="True" CssClass="selectpicker" HTMLEncode="true" data-live-search="true" />
                            </div>
                        </div>

                        <div class="row" id="Riga_Soglia" runat="server" visible="false">

                            <div class="row">
                                <div class="col-md-12">
                                    <h4><i class="fa fa-bug"></i>
                                        <asp:Localize meta:resourcekey="SoglieGiustificazioni" runat="server">Soglie/Giustificazioni</asp:Localize></h4>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-10 col-md-10 col-xs-12">
                                    <asp:DropDownList ID="ddl_soglieAvversita" runat="server" CssClass="selectpicker" HTMLEncode="true" AutoPostBack="true" />
                                    <label id="Lbl_Verifica_Soglia" runat="server" style="color: red; font-size: small" visible="false"></label>
                                </div>
                                <asp:Panel ID="pnlBtnRilievo" class="col-lg-2 col-md-2 col-xs-12" runat="server" Visible="false">
                                    <div class="btn btn-info btn_per_load" id="Rilievo" onclick="ApriRilievo(0);" style="width: 100%">
                                        <i class="fa fa-bug"></i>
                                        <span>
                                            <asp:Localize meta:resourcekey="Rileva" runat="server">Rileva</asp:Localize></span>
                                    </div>
                                    <asp:Button ID="BtnRileva" runat="server" Style="display: none" />
                                </asp:Panel>
                            </div>

                        </div>


                    </div>

                    <!-- DOSAGGIO -->

                    <div class="col-md-12" id="Div1">

                        <div class="row ">
                            <div class="col-md-12">
                                <h4><i class="fa fa-ticket"></i>
                                    <asp:Localize meta:resourcekey="DoseProdotto" runat="server">Dose Prodotto</asp:Localize></h4>
                            </div>
                        </div>

                        <!-- DOSE -->
                        <!-- UDM e Dose Etichetta -->
                        <div class="row DoseEtichetta" id="riga_dose_etichetta" runat="server">
                            <div class="col-md-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="lblDoseEtichetta" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lblDoseEtichettaResource1">Dose Etichetta</asp:Label>
                                            <cc1:ComboEtichetta ID="ComboEtichetta" runat="server" Bootstrap="true" />
                                            <asp:Button ID="Btn_ComboEtichetta" runat="server" Text="Button" Style="display: none" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>


                        <!-- Giacenze -->
                        <div class="row" id="riga_giacenze" runat="server" visible="false">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <asp:Label ID="lblGiacenzakgl" runat="server" meta:resourcekey="lblGiacenzakglResource1"><b>Giacenza Magazzino selezionato [kg] [l]</b></asp:Label>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <asp:Localize meta:resourcekey="AllaData" runat="server">Alla Data:</asp:Localize>
                                    <asp:Label ID="Lbl_Giacenza" runat="server" meta:resourcekey="Lbl_GiacenzaResource1"></asp:Label>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <asp:Localize meta:resourcekey="GiacenzaTotale" runat="server">Totale:</asp:Localize>
                                    <asp:Label ID="Lbl_GiacenzaTotale" runat="server" meta:resourcekey="Lbl_GiacenzaResource1"></asp:Label>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12" id="cella_disponibilita" runat="server" style="display: none">
                                    <asp:Localize meta:resourcekey="Lbl_DisponibilitaAttuale_Name" runat="server">Disponibilità odierna prevista da stoccaggio [mc] [t]:</asp:Localize>
                                    <asp:Label ID="Lbl_DisponibilitaAttuale" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="lbl_qta_residua" runat="server" Visible="False" meta:resourcekey="lbl_qta_residuaResource1"
                                                CssClass="input-group-addon">Dose/Ha Consentita da disciplinare [kg] [l]   </asp:Label>
                                            <asp:Label ID="Lbl_Dose_Consigliata" runat="server" Visible="False" meta:resourcekey="Lbl_Dose_ConsigliataResource1" Style="margin-left: 10px; vertical-align: -webkit-baseline-middle;"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-2 col-md-2 col-sm-12" id="Div_DettaglioDoseConsentita" runat="server" visible="false">
                                <div class="btn btn-info btn_per_load" id="CheckDettaglioDoseConsentita">
                                    <span class="hidden-none margin-r">
                                        <asp:Label ID="Label9" runat="server"> Dettaglio Dose Consentita Disciplinare</asp:Label>
                                    </span>
                                </div>
                                <asp:ImageButton ID="ImgBtn_DettaglioDoseConsentita" runat="server" ImageUrl="../AB_Immagini/icone32/frecciadn.ico" Style="display: none;" />
                            </div>


                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:Label ID="lbl_qta_residua_ricetta" runat="server" Visible="False" meta:resourcekey="lbl_qta_residuaResource1"
                                                CssClass="input-group-addon">Qta/Ha ancora distribuibile sugli appezzamenti selezionati [mc] [t]</asp:Label>
                                            <asp:Label ID="Lbl_Dose_Consigliata_Ricetta" runat="server" Visible="False" Style="margin-left: 10px; vertical-align: -webkit-baseline-middle;"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                             <div class="col-lg-9 col-md-9 col-sm-12" runat="server" style="display: block; padding-bottom:20px">
                                <asp:Label ID="Lbl_Dose_Consigliata2" runat="server" Visible="true" meta:resourcekey="Lbl_Dose_ConsigliataResource2" Style="margin-left: 10px; vertical-align: -webkit-baseline-middle;"></asp:Label>
                            </div>
                        </div>


                        <style>
                            .rblA {
                                text-align: center;
                            }
                        </style>


                        <div class="row">

                            <div class="col-md-6">

                                <!-- DOSE -->

                                <div class="row">
                                    <div class="col-md-12" style="padding: 0">

                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">

                                                    <asp:Label ID="lblUnitaMisura" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lblUnitaMisuraResource1">Unita' di Misura</asp:Label>
                                                    <span>
                                                        <asp:DropDownList ID="Cmb_UdM" runat="server" AutoPostBack="True" CssClass="selectpicker UdmCod" data-container="body"
                                                            meta:resourcekey="Cmb_UdMResource1" onchange="AggiornaDOSI_con_unita(); memorizza_unita_base();">
                                                        </asp:DropDownList>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>


                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-xs-4">
                                    </div>
                                    <div class="col-xs-4">
                                        <asp:RadioButton ID="rbl_DoseHA" runat="server" GroupName="rblDose" Text="Dose/Ha"
                                            CssClass="radio" Checked="True" meta:resourcekey="rbl_DoseHAResource1" />
                                    </div>
                                    <div class="col-xs-4 Acqua" id="RblHLAcqua" runat="server">
                                        <asp:RadioButton ID="rbl_DoseHL" runat="server" GroupName="rblDose" Text="Dose/Hl"
                                            CssClass="radio" meta:resourcekey="rbl_DoseHLResource1" />
                                    </div>
                                </div>
                                <div class="row" style="padding: 5px 0;">
                                    <div class="col-xs-4">
                                        <asp:RadioButton ID="rbl_QtaDose" runat="server" GroupName="rbl_QtaTotHa" Text="Quantità  Dose"
                                            CssClass="radio" Checked="True" meta:resourcekey="rbl_QtaDoseResource1" />
                                    </div>
                                    <div class="col-xs-4">
                                        <asp:TextBox ID="Txt_Dose_HA" runat="server" CssClass="form-control qtaDoseHa" Style="min-width: 50px"
                                            meta:resourcekey="Txt_Dose_HAResource1"></asp:TextBox>
                                        <input type="hidden" id="Hidden_Dose_Ha_Reale" runat="server" />
                                    </div>
                                    <div class="col-xs-4" id="TxtHlAcqua" runat="server">
                                        <asp:TextBox ID="Txt_Dose_HL" runat="server" CssClass="form-control qtaDoseHl Acqua" Style="min-width: 50px"
                                            meta:resourcekey="Txt_Dose_HLResource1"></asp:TextBox>
                                        <input type="hidden" id="Hidden_Dose_Hl_Reale" runat="server" />
                                    </div>
                                </div>
                                <div class="row" style="padding: 5px 0;">
                                    <div class="col-xs-4">
                                        <asp:RadioButton ID="rbl_QtaTot" runat="server" GroupName="rbl_QtaTotHa" Text="Quantità Tal Quale"
                                            CssClass="radio" meta:resourcekey="rbl_QtaTotResource1" />
                                    </div>
                                    <div class="col-xs-8">
                                        <asp:TextBox ID="Txt_DoseTot_HA" runat="server" CssClass="form-control qtaDoseTot"
                                            Style="min-width: 100px" meta:resourcekey="Txt_DoseTot_HAResource1"></asp:TextBox>
                                        <input type="hidden" id="Hidden_Dose_Tot_Reale" runat="server" />
                                    </div>
                                </div>

                                <div class="row" id="DivSupCalcolata" runat="server" style="padding: 5px 0;">
                                    <div class="col-xs-4" style="margin-top: 7px;">
                                        <asp:Label ID="Label8" runat="server">Sup. [ha] Calcolata</asp:Label>
                                    </div>
                                    <div class="col-xs-8">
                                        <input type="text" runat="server" class="form-control SupCalcolata" id="Txt_SupCalcolata"
                                            value="0" />
                                    </div>
                                </div>

                                <input type="hidden" id="hf_Piva_Rif" runat="server" />
                                <input type="hidden" id="hf_Sa_Cod_Rif" runat="server" />
                                <input type="hidden" id="hf_ID_Agenda_Rif" runat="server" />
                                <input type="hidden" id="hf_ID_Mov_Rif" runat="server" />
                                <input type="hidden" id="hf_ID_Mov_Det_Rif" runat="server" />
                                <input type="hidden" id="hf_Lav_Cod_Rif" runat="server" />
                                <input type="hidden" id="hf_Cau_Mov_Rif" runat="server" />
                                <input type="hidden" id="hf_Des_Rif" runat="server" />
                                <input type="hidden" id="hf_Qta_Rif" runat="server" />


                            </div>

                            <!-- Acqua -->

                            <div class="col-md-6 bg_lightblue Acqua radiusBorder float-left" id="DivAcqua" runat="server">
                                <div class="row ">
                                    <div class="col-md-12" style="text-align: center;">


                                        <div class="row">
                                            <div class="col-md-12">
                                                <h4><i class="fa fa-tint"></i>
                                                    <asp:Localize meta:resourcekey="AcquaInMiscelaBotteDiluzione" runat="server">Acqua in Miscela botte/diluizione [hl]</asp:Localize></h4>
                                            </div>

                                        </div>
                                        <div class="col-md-6 ">
                                            <div class="row">
                                                <div class="col-xs-12">
                                                    <asp:RadioButton ID="rblAcqua_HA" CssClass="rblAcqua_HA  " GroupName="rblAcqua" runat="server"
                                                        Text="Quantità  a Ettaro [hl]" meta:resourcekey="rblAcqua_HAResource1" />
                                                </div>
                                                <div class="col-xs-12">
                                                    <asp:TextBox ID="Txt_Acqua_Ha" runat="server" CssClass="form-control AcquaHa" meta:resourcekey="Txt_Acqua_HaResource1"></asp:TextBox>
                                                    <input type="hidden" id="Hidden_Dose_Acqua_Ha_Reale" runat="server" />
                                                </div>
                                                <div class="col-xs-12">
                                                    <asp:Label ID="lbl_acqua_provenienza" runat="server" Style="font-size: 10px" CssClass="lbl_acqua_provenienza"
                                                        meta:resourcekey="lbl_acqua_provenienzaResource1"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6 ">
                                            <div class="row">
                                                <div class="col-xs-12">
                                                    <asp:RadioButton ID="rblAcqua_Tot" CssClass="rblAcqua_Tot  " GroupName="rblAcqua"
                                                        runat="server" Text="Quantità  Totale [hl]" Checked="True" meta:resourcekey="rblAcqua_TotResource1" />
                                                </div>
                                                <div class="col-xs-12">
                                                    <asp:TextBox ID="Txt_Acqua_Tot" runat="server" CssClass="form-control AcquaTot" meta:resourcekey="Txt_Acqua_TotResource1"></asp:TextBox>
                                                    <input type="hidden" id="Hidden_Dose_Acqua_Tot_Reale" runat="server" />
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                    <div class="col-md-12 " id="Div_NoAcqua" runat="server">
                                        <div>
                                            <asp:CheckBox ID="Check_NoAcqua" Checked="false" runat="server" />
                                            <asp:Localize meta:resourcekey="TrattamentoPolverulento" runat="server">Trattamento Polverulento</asp:Localize>
                                            <br />
                                            <asp:Localize meta:resourcekey="NonUtilizzaremiscelaProdottiDiluireAcqua" runat="server">(N.B. Non utilizzare in miscela con prodotti da diluire in acqua)</asp:Localize>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 ">
                                            <asp:HiddenField ID="hdn_TipoInputAcqua" Value="" runat="server" ClientIDMode="Static" />
                                            <asp:Button ID="MacchinaInserita" runat="server" Style="display: none;" />
                                        </div>
                                    </div>

                                </div>
                                <div class="row" style="height: 55px;">
                                </div>
                            </div>

                        </div>

                    </div>

                    <!-- BOTTONE INSERISCI DOSE -->
                    <div class="col-md-12 nopadding">
                        <hr />
                    </div>
                    <div class="col-md-12 text-center">
                        <div id="InserisciDose" class="btn btn-info btn_per_load">
                            <i class="fa fa-arrow-down"></i>
                            <asp:Label ID="lblInserisciProdottoNellaMiscela" runat="server" meta:resourcekey="lblInserisciProdottoNellaMiscelaResource1">Inserisci
    il prodotto nella miscela</asp:Label>
                            <i class="fa fa-arrow-down"></i>

                        </div>

                        <asp:ImageButton ID="ImgBtn_DoseInserisci" runat="server" ImageUrl="../AB_Immagini/icone32/frecciadn.ico"
                            Style="display: none;" meta:resourcekey="ImgBtn_DoseInserisciResource1" />

                    </div>

                    <!-- GRIGLIA DOSI -->

                    <asp:HiddenField ID="GridVew_Dosi_ClientMod" runat="server" />
                    <asp:HiddenField ID="Lotto_Modificato" runat="server" />
                    <asp:HiddenField ID="Fr_Cod_Modificato" runat="server" />
                    <asp:HiddenField ID="Piva_Modificato" runat="server" />
                    <asp:HiddenField ID="Sa_Cod_Modificato" runat="server" />
                    <asp:HiddenField ID="Fabbricato_Cod_Modificato" runat="server" />
                    <asp:HiddenField ID="Piva_Rif_Modificato" runat="server" />
                    <asp:HiddenField ID="Sa_Cod_Rif_Modificato" runat="server" />
                    <asp:HiddenField ID="Id_Agenda_Rif_Modificato" runat="server" />
                    <asp:HiddenField ID="Id_Mov_Rif_Modificato" runat="server" />
                    <asp:HiddenField ID="Id_Mov_Det_Rif_Modificato" runat="server" />
                    <input type="hidden" id="HD_Buffer_Min" runat="server" />
                    <input type="hidden" id="HD_Buffer_Max" runat="server" />
                    <input type="hidden" id="HD_Buffer_Min_Tmp" runat="server" />
                    <input type="hidden" id="HD_Buffer_Max_Tmp" runat="server" />
                    <input type="hidden" id="HD_Perc_Abb_Min" runat="server" />
                    <input type="hidden" id="HD_PrincipiAttiviPercAbb" runat="server" />
                    
                    <div class="">
                        <asp:Button ID="Btn_AggiornaDosiGriglia" runat="server" Style="display: none" />
                        <div style="overflow-x: auto; width: 100%">
                            <asp:GridView ID="GridView_Dosi" runat="server" AutoGenerateColumns="False" CssClass="tablet footable"
                                meta:resourcekey="GridView_DosiResource1">
                                <Columns>
                                    <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ce.ico' border='0'&gt;"
                                        HeaderText="Mod." CommandName="Modifica" meta:resourcekey="ButtonFieldResource1" />
                                    <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                        HeaderText="Canc." CommandName="Cancella" meta:resourcekey="ButtonFieldResource2" />
                                    <asp:BoundField DataField="Av_Cod" HeaderText="Av_Cod" meta:resourcekey="BoundFieldResource6">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Av_Gru" HeaderText="Av_Gru" meta:resourcekey="BoundFieldResource7">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Av_Des" HeaderText="Avversità / Gruppi Avversità" HtmlEncode="False"
                                        meta:resourcekey="BoundFieldResource8"></asp:BoundField>
                                    <asp:BoundField DataField="Soglia_Value" HeaderText="Soglia_Value" HtmlEncode="False"
                                        meta:resourcekey="BoundFieldResource9">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Soglia_Des" HeaderText="Richiesta Soglia / Giustif." HtmlEncode="False"
                                        meta:resourcekey="BoundFieldResource10"></asp:BoundField>

                                    <asp:BoundField DataField="Fr_Cod" HeaderText="N. Reg. Min" meta:resourcekey="BoundFieldResource11">
                                        <ItemStyle CssClass="Fr_Cod" />

                                    </asp:BoundField>
                                    <asp:BoundField DataField="Fr_Des" HeaderText="Formulato" meta:resourcekey="BoundFieldResource12"></asp:BoundField>


                                    <asp:BoundField DataField="Efficienza" HeaderText="Efficienza"
                                        meta:resourcekey="BoundFieldResourceEfficienza"></asp:BoundField>
                                    <asp:BoundField DataField="N" HeaderText="N"
                                        meta:resourcekey="BoundFieldResourceN"></asp:BoundField>
                                    <asp:BoundField DataField="N_Utile" HeaderText="N Utile"
                                        meta:resourcekey="BoundFieldResourceN_Utile"></asp:BoundField>
                                    <asp:BoundField DataField="P2O5" HeaderText="P2O5"
                                        meta:resourcekey="BoundFieldResourceP"></asp:BoundField>
                                    <asp:BoundField DataField="K2O" HeaderText="K2O"
                                        meta:resourcekey="BoundFieldResourceK"></asp:BoundField>
                                    <asp:BoundField DataField="MgO" HeaderText="Mg"
                                        meta:resourcekey="BoundFieldResourceM">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />

                                    </asp:BoundField>
                                    <asp:BoundField DataField="Cu" HeaderText="Cu"
                                        meta:resourcekey="BoundFieldResourceCu"></asp:BoundField>

                                    <asp:BoundField DataField="BufferMin" HeaderText="Buffer Min">
                                        <ItemStyle CssClass="displaynone BufferMin" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BufferMax" HeaderText="Buffer Max">
                                        <ItemStyle CssClass="displaynone BufferMax" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="strBuffer" HeaderText="Buffer Zone [m]"></asp:BoundField>

                                    <asp:BoundField DataField="Dose_Etichetta" HeaderText="Dose Etichetta" HtmlEncode="False"
                                        meta:resourcekey="BoundFieldResource13"></asp:BoundField>
                                    <asp:BoundField DataField="Dose_Etichetta_Max" HeaderText="Dose_Etichetta_Max" meta:resourcekey="BoundFieldResource14">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Carenza" HeaderText="Tempo di Carenza" meta:resourcekey="BoundFieldResource15"></asp:BoundField>
                                    <asp:BoundField DataField="Prima_Raccolta" HeaderText="Data prima raccolta utile"
                                        meta:resourcekey="BoundFieldResource16"></asp:BoundField>

                                    <%--Per Semina--%>
                                    <asp:BoundField DataField="Cod_Articolo" HeaderText="Codice Articolo"></asp:BoundField>
                                    <asp:BoundField DataField="Lotto" HeaderText="Lotto">
                                        <ItemStyle CssClass="Lotto" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Udm_Cod" HeaderText="Udm_Cod" meta:resourcekey="BoundFieldResource17">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Udm_Des" HeaderText="Unita' di Misura" meta:resourcekey="BoundFieldResource18"></asp:BoundField>
                                    <asp:BoundField DataField="Dose" HeaderText="Dose Reale" meta:resourcekey="BoundFieldResource19">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dose_Max" HeaderText="Dose Max" meta:resourcekey="BoundFieldResource20">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dose_HA" HeaderText="Dose [HA]" meta:resourcekey="BoundFieldResource21">
                                        <ItemStyle CssClass="Dose_HA" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dose_HL" HeaderText="Dose [HL]" meta:resourcekey="BoundFieldResource22">
                                        <ItemStyle CssClass="Dose_HL" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Qta_Tot" HeaderText="Quantità Totale Distribuita"
                                        meta:resourcekey="BoundFieldResource23">
                                        <ItemStyle CssClass="Qta_Tot" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="strPA_Cod" HeaderText="strPA_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="strCLTOSS_COD" HeaderText="strCLTOSS_COD">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dose_Etichetta_Value" HeaderText="Dose_Etichetta_Value">
                                        <ItemStyle CssClass="displaynone Dose_Etichetta_Value" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:ButtonField Text="<img src='../AB_Immagini/icone16/ci.ico' border='0' />" HeaderText="Dose Consigliata"
                                        CommandName="Select" meta:resourcekey="ButtonFieldResource3"></asp:ButtonField>

                                    <asp:BoundField DataField="Dose_HA_Reale" HeaderText="Dose_HA_Reale">
                                        <ItemStyle CssClass="displaynone Dose_HA_Reale" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dose_Hl_Reale" HeaderText="Dose_Hl_Reale">
                                        <ItemStyle CssClass="displaynone Dose_HL_Reale" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dose_Tot_Reale" HeaderText="Dose_Tot_Reale">
                                        <ItemStyle CssClass="displaynone Qta_Tot_Reale" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Ha_Hl" HeaderText="Ha_Hl">
                                        <ItemStyle CssClass="displaynone Ha_Hl" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dose_QtaTot" HeaderText="Dose_QtaTot">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="strPA_COD_Pesi" HeaderText="strPA_COD_Pesi">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Sup_Calcolata" HeaderText="Sup.Calcolata [Ha]">
                                        <ItemStyle CssClass="displaynone Sup_Calcolata" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Fabbricato_Des" HeaderText="Magazzino" HtmlEncode="False"
                                        meta:resourcekey="BoundFieldResource24"></asp:BoundField>
                                    <asp:BoundField DataField="Des_Rif" HeaderText="Riferimento" HtmlEncode="False"
                                        meta:resourcekey="BoundFieldResource24"></asp:BoundField>

                                    <asp:BoundField DataField="Piva" HeaderText="Piva">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Fabbricato_Cod" HeaderText="Fabbricato_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Piva_Rif" HeaderText="Piva_Rif">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Sa_Cod_Rif" HeaderText="Sa_Cod_Rif">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Id_Agenda_Rif" HeaderText="Id_Agenda_Rif">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Id_Mov_Rif" HeaderText="Id_Mov_Rif">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Id_Mov_Det_Rif" HeaderText="Id_Mov_Det_Rif">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="PrincipiAttiviPercAbb" HeaderText="Perc Abb">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>

                                </Columns>
                                <HeaderStyle CssClass="ui-widget-header" />
                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                <RowStyle CssClass="rigaImpianti" />
                            </asp:GridView>
                        </div>

                    </div>


                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="BTN_Magazzini" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>


            <asp:UpdatePanel ID="UpdatePanelRaccolta" runat="server" style="display:none" >
                <ContentTemplate>
                    <!-- Prodotto -->
                    <div class="col-md-12" id="cella_raccolta">





                        <!-- Prodotti -->

                        <div class="row">
                            <div class="col-md-12 col-xs-12">
                                <h4 style="margin-bottom: -15px;">
                                    <asp:Localize runat="server">Prodotto</asp:Localize>
                                </h4>
                                <br />

                            </div>
                        </div>


                        <div class="row Prodotti" id="Div4" runat="server">
                            <div class="col-md-12 col-xs-12">

                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <asp:DropDownList ID="cmb_Prodotto" runat="server" AutoPostBack="True" CssClass="selectpicker" data-container="body">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 col-xs-12">


                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">

                                            <asp:Label ID="Label2" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="lblUnitaMisuraResource1">Unita' di Misura</asp:Label>
                                            <span>
                                                <asp:DropDownList ID="Cmb_UdM_Raccolta" runat="server" AutoPostBack="True" CssClass="selectpicker UdmCodRaccolta" data-container="body"
                                                    meta:resourcekey="Cmb_UdMResource1">
                                                </asp:DropDownList>
                                            </span>
                                        </div>
                                    </div>
                                </div>



                            </div>
                            <div class="col-md-6 col-xs-12">

                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">

                                            <asp:Label ID="Label7" runat="server" CssClass="input-group-addon alert-info">Quantita' Totale</asp:Label>
                                            <span>
                                                <asp:TextBox ID="Txt_QtaTotRaccolta" runat="server" CssClass="form-control"
                                                    Style="min-width: 100px"></asp:TextBox>
                                            </span>
                                        </div>
                                    </div>
                                </div>



                            </div>
                        </div>



                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>




        <script>


            $(document).on('keydown', '#<%=Txt_Formulati.ClientID %>', function () {
                Verifica_Tasto_Premuto();
            });

            //$(window).load(function () {
            $(window).on('load', function () {
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

                    InizializzazioneControlliPagina();


            });


            function InizializzazioneControlliPagina() {

                //ImpostaAvversitaProdotto();

                Abilita_Disabilita_ACQUA();
                Abilita_Disabilita_Resto();
                Abilita_Disabilita_DOSI();

                AggiornaACQUA();

                Mostra_Nascondi_PanelSemina();

                //Grilli 14/08/2018 Se entro nella pagina ed è una ricetta eseguo all'avvio il verifica conformità
                if (<%= (hf_app_ricetta_operazione_id.Value <> "").ToString().ToLower() %> && <%=Master.Property_Div_ImgBtn_CheckDPI.Visible.ToString().ToLower() %>) {

                    var kendoConfirm = $("<div></div>").kendoConfirm({
                        title: "Controlla...",
                        messages: { okText: "Sì", cancel: "No" },
                        content: "Vuoi verificare la conformità dell'intervento?"
                    }).data("kendoConfirm");

                    kendoConfirm.result.done(function () { $('#<%=Master.Property_Div_ImgBtn_CheckDPI.ClientID %>').click(); });
                    kendoConfirm.open();

                }

            }

            function Mostra_Nascondi_PanelSemina() {
                console.log("Mostra_Nascondi_PanelSemina");
                var lavcod = $('.ComboOperazioni').val();
                var isSemina = false;

                switch (parseInt(lavcod)) {
                    case 71: case 2: case 160: case 68:
                        isSemina = true;
                        break;
                }

                if (isSemina) {
                    let magazzinoSelezionato = $(".ComboMagazzini").val();
                    if (magazzinoSelezionato == "0") {
                        $("#<%=pnlSeminaOpzioniSalvataggio.ClientID%>").hide();
                    } else {
                        $("#<%=pnlSeminaOpzioniSalvataggio.ClientID%>").show();
                    }
                }

            }

            <%--Grilli: per modifica Attività
            function btnCreaNuovaAttivita_click() {

                ajaxAgronica("Trattamenti_2.aspx/ApriPaginaAttivita",
                   "",
                    function (risposta) {
                        var url = risposta.RispostaStringa;

                        //apertura in finestra modale..
                        $("#PaginaGeneric").attr("src", url);
                        $("#iFrameGeneric h4 span").html("Gestione Attività");
                        $("#iFrameGeneric").modal('toggle');                        
                        $('#iFrameGeneric').on('hidden.bs.modal', function () {

                           $('#<%=BTN_RicaricaComboAttivita.ClientID %>').click();

                        });

                    }, null);

            }--%>

            function cercaProdotti() {
                MacchinaInserita(false);
                $('#<%=btn_cerca.ClientID %>').click();
            }

            function MacchinaInserita(callServer) {

                if (callServer == undefined) {
                    callServer = true;
                }

                kDirtyAll("#kendo_CostiAccessori");
                var grid = $("#kendo_CostiAccessori").data("kendoGrid");
                var data = grid.dataSource.data();
                var finalData = new Array();

                for (var i = 0; i < data.length; i++) {

                    var dataModel = data[i];
                    if (dataModel.Risorsa_Cod != "" && dataModel.deleted != true) {
                        finalData.push(dataModel);
                    }

                }
                $("#" + id_hdKendo_CostiAccessori_Selezione).val(JSON.stringify(finalData));

                $("#kendo_CostiAccessori").addClass("dpiOff");

                $("#costi_Manodopera").addClass("dpiOff");

                $("#costi_Macchine").addClass("dpiOff");

                $("#kendo_impianti").addClass("dpiOff");

                if (callServer) {
                    $('#<%=MacchinaInserita.ClientID %>').click();
                }
            }

            function getRighe(righeKendoGrid) {
                $('#GestioneMovimentiContabiliWindow').data("kendoWindow").close();
                $("#<%=hfRigheMovContabili.ClientID %>").val(JSON.stringify(righeKendoGrid));
                $('#<%=Btn_MovContabili.ClientID %>').click();
            }

        </script>

    </div>
</asp:Content>
