<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Trattamenti_PostRaccolta.aspx.vb"
    MasterPageFile="../Master/Operazione.master" ValidateRequest="false" EnableEventValidation="false"
    Inherits="AgronicaDomandaIrrigua.Trattamenti_PostRaccolta" meta:resourcekey="PageResource1" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentOperazioniHeader" runat="server">
    <style type="text/css">
        .classComboFormulati .ui-autocomplete-input {
            min-width: 525px;
            width: 90%;
        }

        .rblSoglie {
            font-size: 10px;
        }
    </style>
    <script type="text/javascript">


        var Y;

        $(document).keypress(function (e) {
            if (e.which == 13) {
                $('#<%=btn_cerca.ClientID %>').click();
                return false;
            }
        });



        function Init() {

            $('#div_pannello_avversita_scroll').scroll(function () {
                Y = $('#div_pannello_avversita_scroll').scrollTop();
            });

        };


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
        function MettiColori() {

            var ComboDisciplinari = $('.ComboDisciplinari').hasClass('myCombo');


            if (ComboDisciplinari == true) {
                var colorato = false;
                if ($('.ComboDisciplinari').val() == 0) {
                    //metto il giallo solo dove c'è il dpi
                    $('.classeDPI').each(function () {
                        //if ($(this).html() != '&nbsp;') {
                        if ($(this).html() != 0) {
                            $(this).parent().addClass("sfondogiallo");
                            colorato = true;
                        } else {
                            $(this).parent().removeClass("sfondogiallo");
                        }
                    });
                    //regolamenti
                    $('.classeRegolamenti').each(function () {
                        //alert($(this).html());
                        //Reg. CE 834/07 (eEx. Reg. CE 2092/91)
                        //if ($(this).html().indexOf('834/07') >= 0) {
                        if ($(this).html() == 4) {
                            $(this).parent().addClass("sfondogiallo");

                        } else {
                            if (colorato = false) {
                                $(this).parent().removeClass("sfondogiallo");
                            }
                        }
                    });
                }
                else {
                    $('.classeDPI').parent().removeClass("sfondogiallo");


                    //se c'è il bio come regolamento ci deve essere bio anche nella combo disciplinare
                    //regolamenti

                    if ($('.ComboDisciplinari').val() != -2) {
                        $('.classeRegolamenti').each(function () {

                            //if ($(this).html().indexOf('834/07') >= 0) {
                            if ($(this).html() == 4) {
                                $(this).parent().addClass("sfondogiallo");

                            } else {

                            }
                        });

                    }
                }

            }
        }


        function CambiaFormulati() {
            $("#<%=BTN_ComboFormulati.ClientID %>").click();
        }

        //per la Gestione dell'Eliminazione
        function DoPostBack_Combo_Slave($_combo, valoreOpt) {
            //Disciplinari
            if ($_combo.attr("id").endsWith("ComboDisciplinari")) {
                $("#<%=BTN_ComboDisciplinari1.ClientID %>").click();
                $('#WaitFrame').show();
            }


            if ($_combo.attr("id").endsWith("ComboFiltriAggiuntiviAgenda")) {
                $("#<%=BTN_ComboFiltriAggiuntiviAgenda.ClientID %>").click();
                $('#WaitFrame').show();
            }

        }

        //per collegarsi al sito del profitosan
        function Info() {

            var classificazioni = $('.Cmb_FormulatoClassificazioni').val();

            stringa = $('.ComboFormulati').val();
            if (stringa == null) {
                return false;
            }
            if (stringa.toString().length > 0) {
                var splitted = stringa.split("£");
                pro_cod = splitted[0];

                //vecchia gestione TargetUrl='../../Popup_Informativi/Popup_Formulato.aspx?fr_cod=';
                TargetUrl = $('.IndirizzoProfitosan').val() + '/x_Tunnel/Tunnel_GiasOnLine.aspx?p=1&f=';
                Caratteristiche = "dialogWidth:520px; dialogHeight:890px; status:no; center:yes; edge:raised; help:no;"

                //vecchia gestione  a = window.showModalDialog(TargetUrl + pro_cod,"",Caratteristiche)	
                window.open(TargetUrl + pro_cod, 'profitosan', '');
            }
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

            if ($('.rigaImpianti').length > 0) {
                $('#<%=Txt_Acqua_Tot.ClientID %>').attr('disabled', 'disabled');
                $('#<%=Txt_Acqua_HA.ClientID %>').attr('disabled', 'disabled');
                return;
            }


            if (AcquaTotChecked()) {
                //abilito la txt acqua totale

                if ($('#<%=rblAcqua_Tot.ClientId %>').is(':disabled')) {
                    $('#<%=Txt_Acqua_Tot.ClientID %>').attr('disabled', 'disabled');
                    $('#<%=Txt_Acqua_HA.ClientID %>').attr('disabled', 'disabled');

                } else {
                    $('#<%=Txt_Acqua_Tot.ClientID %>').removeAttr('disabled');
                    $('#<%=Txt_Acqua_HA.ClientID %>').attr('disabled', 'disabled');
                }

            }
            else {
                if ($('#<%=rblAcqua_Tot.ClientId %>').is(':disabled')) {
                    $('#<%=Txt_Acqua_Tot.ClientID %>').attr('disabled', 'disabled');
                    $('#<%=Txt_Acqua_HA.ClientID %>').attr('disabled', 'disabled');

                } else {
                    //abilito la txt acqua HA
                    $('#<%=Txt_Acqua_HA.ClientID %>').removeAttr('disabled');
                    $('#<%=Txt_Acqua_Tot.ClientID %>').attr('disabled', 'disabled');
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

            //imposto
            if (dose_HA == false) {
                $('#<%=Txt_Dose_Ha.ClientID %>').attr('disabled', 'disabled');
            } else {
                $('#<%=Txt_Dose_Ha.ClientID %>').removeAttr('disabled');
            }

            if (dose_HL == false) {
                $('#<%=Txt_Dose_HL.ClientID %>').attr('disabled', 'disabled');
            } else {
                $('#<%=Txt_Dose_HL.ClientID %>').removeAttr('disabled');
            }

            if (doseTot_HA == false) {
                $('#<%=Txt_DoseTot_Ha.ClientID %>').attr('disabled', 'disabled');
            } else {
                $('#<%=Txt_DoseTot_Ha.ClientID %>').removeAttr('disabled');
            }

        }

        /////////////////////////////////////////////////////////
        ////////CALCOLO COSTI ACCESSORI AUTOMATICO///////////////

        function CalcolaCostiAccessori() {
            var flag = $(".CostiAperti").children().is(':checked');
            if (flag == true) {
                var sup = $('#<%=Txt_Suptrattata.ClientId %>').val().replace(',', '.');
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
            return $('#<%=Txt_Suptrattata.ClientId %>').val().replace(',', '.');
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
            return $('#<%=Txt_Acqua_Tot.ClientId %>').val().replace(',', '.');
        }
        function InserisciAcquaTot(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Acqua_Tot.ClientId %>').val(app);
        }

        function AcquaHA() {
            return $('#<%=Txt_Acqua_Ha.ClientId %>').val().replace(',', '.');
        }
        function InserisciAcquaHA(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Acqua_Ha.ClientId %>').val(app);
        }

        function DoseHA() {
            return $('#<%=Txt_Dose_Ha.ClientId %>').val().replace(',', '.');
        }
        function InserisciDoseHA(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Dose_Ha.ClientId %>').val(app);
        }

        function DoseHL() {
            return $('#<%=Txt_Dose_HL.ClientId %>').val().replace(',', '.');
        }
        function InserisciDoseHL(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Dose_HL.ClientId %>').val(app);
        }

        function TotHA() {
            return $('#<%=Txt_DoseTot_Ha.ClientId %>').val().replace(',', '.');
        }
        function InserisciTotHA(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_DoseTot_Ha.ClientId %>').val(app);
        }

        function AggiornaACQUA() {
            var _SupTrattata = SupTrattata();
            var valore = 0;
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
        }

        function AggiornaDOSI() {
            var _SupTrattata = SupTrattata();
            var _Acqua = AcquaTot();

            if (DoseHAChecked()) {
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
                            InserisciTotHA(_DoseTotaleHL);
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
            //Aggiorno l'acqua
            AggiornaACQUA();
            //Aggiorno le Dosi solo se ho la qta/ha selezionata
            AggiornaDOSI();
        }
        function AcquaHA_Keyup() {
            //Aggiorno l'acqua
            AggiornaACQUA();
            //Aggiorno le Dosi solo se ho la qta/ha selezionata
            AggiornaDOSI();
        }

        function AggiornaDopo_SupTrattata() {
            //Aggiorno l'acqua
            AggiornaACQUA();
            //Aggiorno le Dosi solo se ho la qta/ha selezionata
            AggiornaDOSI();

            //Agggiorno i costi accessori
            CalcolaCostiAccessori();
        }


        function PulisciGrigliaAv_GrAv() {
            $('.ChkSelezionaGruppoAvversita').children('input').each(function () {
                $(this).removeAttr('checked');
            });
            $('.ChkSelezionaAvversita').children('input').each(function () {
                $(this).removeAttr('checked');
            });


        }



        function DoPostBack_ControlliSiNo(key) {
            if (key == 'Salva') {
                $("#<%=HiddenVarie.ClientID %>").val("OK");
                $("#<%=ImgBtn_DoseInserisci.ClientID %>").click();
            }
        }



        function SelezionaAutoSoglia(oggetto) {

            var attr = $(oggetto).children().is(':checked');
            if (typeof attr !== 'undefined' && attr !== false) {
                $(oggetto).parent().parent().find('.stileSoglie').children().children().children().find("input").attr('checked', 'checked');
            }
        }


        /////////////////////////////////////////////////////
        /////////////////// LAVORATI ///////////////////////
        /////////////////////////////////////////////////////

        function SelezionaDeselezionaLavorati() {
            if ($('#chkSelezionaTuttiLavorati').is(':checked')) {


                //seleziono tutto
                $('.ChkSelezionaLavorato').each(function () {
                    $(this).children('input').attr('checked', 'checked');
                    ChkSelezionaLavorato_Click($(this).children('input'), true);
                });
                AggiornaDopo_SupTrattata();
            }
            else {


                //seleziono tutto
                $('.ChkSelezionaLavorato').each(function () {
                    $(this).children('input').removeAttr('checked');
                    ChkSelezionaLavorato_Click($(this).children('input'), true);
                });
                AggiornaDopo_SupTrattata();
            }
        }

        function ChkSelezionaLavorato_Click(Oggetto, tutti) {

            AggiornaValoreLavorato(Oggetto);
            RicalcolaQtaTotale();
            RicalcolaQtaCoinvolta();
            if (tutti == false) { AggiornaDopo_SupTrattata(); }

        }

        //Aggiornamento della Qta coinvolta del singolo 
        function AggiornaValoreLavorato(Oggetto) {
            if (Oggetto.is(':checked')) {
                Inserisci_ValoreQtaCoinvolta_Lavorato(Oggetto, ValoreQta_Lavorato(Oggetto));
                Oggetto.parent().parent().parent().find('.Qta_Giacenza').removeAttr('disabled');
            }
            else {
                Inserisci_ValoreQtaCoinvolta_Lavorato(Oggetto, '0.00');
                Oggetto.parent().parent().parent().find('.Qta_Giacenza').attr('disabled', 'disabled');
            }
        }


        //Dato un oggetto $ trova la superficie dell'impianto
        function ValoreQta_Lavorato(Oggetto) {
            return Oggetto.parent().parent().parent().children('.Giacenza').html().replace(',', '.');
        }

        function ValoreQtaCoinvolta_Lavorato(Oggetto) {
            return Oggetto.parent().parent().parent().find('.Qta_Giacenza').val().replace(',', '.');
        }

        function Inserisci_ValoreQtaCoinvolta_Lavorato(Oggetto, valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            Oggetto.parent().parent().parent().find('.Qta_Giacenza').val(app);
        }



        //Funzione per l'aggiornamento della qta Totale
        function RicalcolaQtaTotale() {
            var QtaTot = 0.00;
            //sel la checkbox è chekkata
            $('.ChkSelezionaLavorato').children('input:checked').each(function () {
                var app = ValoreQta_Lavorato($(this));
                QtaTot += parseFloat(app);
            });
            InserisciSupTotale(QtaTot);
        }
        //Aggiornamento della qta Coinvolta
        function RicalcolaQtaCoinvolta() {
            var QtaTot = 0.00;
            //sel la checkbox è chekkata
            $('.ChkSelezionaLavorato').children('input:checked').each(function () {
                var app = ValoreQtaCoinvolta_Lavorato($(this));
                QtaTot += parseFloat(app);
            });
            InserisciSupTrattata(QtaTot);
        }

        function Qta_Coinvolta_Keyup(Oggetto) {
            ControllaMax(Oggetto);
            RicalcolaQtaCoinvolta();
            AggiornaDopo_SupTrattata();
        }

        //Verifico che la superficie coinvolta non superi la superficie dell'impianto
        function ControllaMax(Oggetto) {
            var valoreSup_Imp = roundNumber(Oggetto.parent().parent().find('.Giacenza').html().replace(",", "."), 4);
            var valoreInserito = roundNumber(Oggetto.parent().parent().find('.Qta_Giacenza').val().replace(",", "."), 4);
            if (valoreSup_Imp < valoreInserito) {
                Oggetto.parent().parent().find('.Qta_Giacenza').val(valoreSup_Imp.toString().replace(".", ","));
                MessaggioErrore('La Qta trattata non può superare la giacenza!');
            }
        }

        function AbilitaDisabilita_QtaTrattata() {
            $('.ChkSelezionaLavorato').children('input').each(function () {
                if ($(this).is(':checked') == false) {
                    $(this).parent().parent().parent().find('.Qta_Giacenza').attr('disabled', 'disabled');
                }
            });
        }



        $(document).ready(function () {
            $("#InserisciDose").button();
            $("#InserisciDose").click(function () { $("#<%=ImgBtn_DoseInserisci.ClientID %>").click(); });
        });




    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlacexTRATTAMENTI" runat="server">
    <input id="IndirizzoProfitosan" type="hidden" class="IndirizzoProfitosan" runat="server" />
    <input type="hidden" id="HiddenVarie" runat="server" />
    <div class="box50" runat="server" id="pannelloDisciplinari" style="min-width: 350px">
        <div class="descrizione" style="width: 80px">
            <asp:Label ID="lblDPI" runat="server" meta:resourcekey="lblDPIResource1">Disciplinare</asp:Label>
        </div>
        <div class="valoriinput">
            <asp:UpdatePanel ID="UpdatePanelDisciplinare" runat="server">
                <contenttemplate>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanelDisciplinare"
                        DisplayAfter="50">
                        <progresstemplate>
                            <div class="LoadPanel">
                                <div class="loading-indicator-bars">
                                </div>
                            </div>
                        </progresstemplate>
                    </asp:UpdateProgress>
                    <cc1:combodisciplinari id="ComboDisciplinari1" runat="server" meta:resourcekey="ComboDisciplinari1Resource1" />
                </contenttemplate>
                <triggers>
                    <asp:AsyncPostBackTrigger ControlID="BTN_ComboSpecie" EventName="Click" />
                </triggers>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="box50" runat="server" id="pannelloFiltriRicerca" style="min-width: 350px">
        <div class="descrizione" style="width: 80px">
            <asp:Label ID="lblFiltriRicerca" runat="server" meta:metaresourcekey="lblFiltriRicercaResource1">Filtri Ricerca</asp:Label>
        </div>
        <div class="valoriinput">
            <asp:UpdatePanel ID="UpdatePanelFiltriAggiuntivi" runat="server">
                <contenttemplate>
                    <cc1:combofiltriaggiuntiviagenda id="ComboFiltriAggiuntiviAgenda1" runat="server"
                        meta:resourcekey="ComboFiltriAggiuntiviAgenda1Resource1" />
                </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentOperazioniContenuti" runat="server">
    <!-- Bottone del Disciplinare -->
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <contenttemplate>
            <asp:UpdateProgress ID="UpdateProgressUpdatePanel2" runat="server" AssociatedUpdatePanelID="UpdatePanel2"
                DisplayAfter="50">
                <progresstemplate>
                    <div class="LoadPanel">
                        <div class="loading-indicator-bars">
                        </div>
                    </div>
                </progresstemplate>
            </asp:UpdateProgress>
            <asp:Button ID="BTN_ComboDisciplinari1" runat="server" Text="Button" Style="display: none"
                meta:resourcekey="BTN_ComboDisciplinari1Resource1" />
            <asp:Button ID="BTN_ComboFiltriAggiuntiviAgenda" runat="server" Style="display: none"
                meta:resourcekey="BTN_ComboFiltriAggiuntiviAgendaResource1" />
        </contenttemplate>
    </asp:UpdatePanel>
    <table width="100%" aria-hidden="true">
        <tr id="Riga_Lavorati" runat="server">
            <td colspan="3">

                <div class="cento" style="overflow: auto; width: 100%; height: 100%; max-height: 250px; margin-top: 10px;">
                    <asp:UpdatePanel ID="UpdatePanelLavorati" runat="server">
                        <contenttemplate>
                            <asp:Button ID="AggiornaGrigliaMagazzino" runat="server" Style="display: none;" />
                            <asp:GridView ID="GridViewMagazzino" runat="server" AutoGenerateColumns="False" CellPadding="5"
                                CssClass="ui-widget-content" Caption="Prodotti Magazzino">
                                <columns>
                                    <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                        <headertemplate>

                                            <input type="checkbox" id="chkSelezionaTuttiLavorati" />


                                        </headertemplate>
                                        <itemtemplate>
                                            <asp:CheckBox ID="ChkSeleziona" runat="server" CssClass="ChkSelezionaLavorato" meta:resourcekey="ChkSelezionaResource1" />
                                        </itemtemplate>
                                        <headerstyle width="20px" />
                                        <itemstyle width="20px" />
                                        <footerstyle width="20px" />
                                        <controlstyle width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Elem_Cod" HeaderText="Elem_Cod">
                                        <itemstyle cssclass="displaynone" />
                                        <headerstyle cssclass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Mat_Cod" HeaderText="Mat_Cod">
                                        <itemstyle cssclass="displaynone" />
                                        <headerstyle cssclass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Cod_Progetto" HeaderText="Cod_Progetto">
                                        <itemstyle cssclass="displaynone" />
                                        <headerstyle cssclass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Cal_Cod" HeaderText="Cal_Cod">
                                        <itemstyle cssclass="displaynone" />
                                        <headerstyle cssclass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Udm_Cod" HeaderText="Udm_Cod">
                                        <itemstyle cssclass="displaynone" />
                                        <headerstyle cssclass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Piva" HeaderText="Piva">
                                        <itemstyle cssclass="displaynone" />
                                        <headerstyle cssclass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod">
                                        <itemstyle cssclass="displaynone" />
                                        <headerstyle cssclass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Id_Destinazione" HeaderText="Id_Destinazione">
                                        <itemstyle cssclass="displaynone" />
                                        <headerstyle cssclass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Fabbricato_des" HeaderText="Magazzino" SortExpression="Fabbricato_des"></asp:BoundField>
                                    <asp:BoundField DataField="Descrizione_Prodotto" HeaderText="Prodotto" SortExpression="Descrizione_Prodotto"></asp:BoundField>
                                    <asp:BoundField DataField="Cod_Articolo" HeaderText="Codice Articolo" SortExpression="Cod_Articolo"
                                        meta:resourcekey="BoundFieldResource3"></asp:BoundField>
                                    <asp:BoundField DataField="Lotto" HeaderText="Lotto di Accettazione" SortExpression="Lotto"
                                        meta:resourcekey="BoundFieldResource4"></asp:BoundField>
                                    <asp:BoundField DataField="Giacenza" HeaderText="Giacenza Rilevata [q]" SortExpression="Giacenza"
                                        meta:resourcekey="BoundFieldResource5">
                                        <itemstyle cssclass="Giacenza" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Qta [q]" SortExpression="Qta" meta:resourcekey="TemplateFieldResource2">
                                        <itemtemplate>
                                            <asp:TextBox ID="TxtQtaGiacenza" runat="server" CssClass="Qta_Giacenza txtUI" Text='0'
                                                ToolTip="Inserire la quantità da utilizzare, verrà ripatita sugli impianti in base alle superfici. Il dato che fa fede è sempre quello inserito sugli impianti."
                                                meta:resourcekey="TxtQtaGiacenzaResource1" />
                                        </itemtemplate>
                                        <controlstyle width="40px" />
                                        <headerstyle width="35px" />
                                    </asp:TemplateField>
                                </columns>
                                <headerstyle cssclass="ui-widget-header" />
                                <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                            </asp:GridView>
                        </contenttemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="clear">
                </div>
            </td>
        </tr>
        <tr id="Riga_Formulati" runat="server">
            <td id="Cella_Avversita" runat="server" valign="top" style="border-right: 1px solid #A6C9E2; width: 350px;">
                <table width="100%" aria-hidden="true">
                    <tr>
                        <td style="width: 280px">
                            <asp:Label ID="lblSupHaSelezionata" runat="server"><b>Qta [q]</b> Selezionata:</asp:Label>
                        </td>
                        <td style="width: 70px">
                            <input type="text" runat="server" style="width: 60px" class="txtUI SommaSuperficie"
                                id="Txt_SupSelezionata" disabled="disabled" readonly="readonly" value="0" />
                        </td>
                    </tr>
                    <tr style="background-color: #FFC0C0;">
                        <td>
                            <asp:Label ID="lblSupHaTrattata" runat="server"><b>Qta [q]</b> Trattata:</asp:Label>
                        </td>
                        <td>
                            <input type="text" runat="server" style="width: 60px" class="txtUI SommaSuperficieTrattata"
                                id="Txt_SupTrattata" readonly="readonly" value="0" />
                        </td>
                    </tr>
                </table>
                <div class="clear">
                </div>
                <div id="PannelloAvversita" runat="server">
                    <div style="width: 350px;">
                        <div style="float: left">
                            <label id="Lbl_Avversita" runat="server" class="descrizione" style="text-align: left; width: 200px;">
                            </label>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <contenttemplate>
                                <asp:ImageButton ID="LockModifica" runat="server" ImageUrl="~/AB_Immagini/Icone32/update.png"
                                    Visible="false" Style="margin-left: 0px; float: right; margin-right: 10px" />
                            </contenttemplate>
                        </asp:UpdatePanel>
                        <%--                        <div style="float: left;">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="RBL_Avversita" runat="server" RepeatDirection="Horizontal"
                                        Style="font-size: 10px; width: 350px;" CssClass="txtUI" AutoPostBack="True" meta:resourcekey="RBL_AvversitaResource1">
                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource1">Avversita&#39;/Infestanti</asp:ListItem>
                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Gruppi Avversita&#39;/Infestanti</asp:ListItem>
                                    </asp:RadioButtonList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>--%>
                    </div>
                    <div class="clear">
                    </div>
                    <div style="width: 350px;">
                        <div style="float: left;">

                            <asp:Label ID="Label31" runat="server" meta:resourcekey="lblTestoRicercaResource1" style="font-size: 10px; margin-left: 5px">Testo di Ricerca:</asp:Label>
                        </div>
                        <div style="float: left;">
                            <asp:TextBox ID="Txt_Avv" runat="server" CssClass="txtUI" Width="150px" style="margin-left: 5px"></asp:TextBox>
                        </div>
                        <div id="Div1" style="width: 32px" class="btn_per_load">
                            <img onclick="$('#<%=btn_cerca_avv.ClientID %>').click();" style="width: 32px; margin-left: 5px;" class="btn_per_load"
                                alt="" src="../AB_Immagini/icone32/lente.ico" />
                        </div>
                        <asp:Button ID="btn_cerca_avv" runat="server" Text="Cerca" Style="display: none" />
                    </div>
                    <div class="clear">
                    </div>
                    <asp:UpdatePanel ID="UpdatePanelAvversita" runat="server">
                        <contenttemplate>
                            <asp:UpdateProgress ID="UpdateProgressUpdatePanelAvversita" runat="server" AssociatedUpdatePanelID="UpdatePanelAvversita"
                                DisplayAfter="50">
                                <progresstemplate>
                                    <div class="LoadPanel">
                                        <div class="loading-indicator-bars">
                                        </div>
                                    </div>
                                </progresstemplate>
                            </asp:UpdateProgress>
                            <asp:Button ID="EventoAggiornamentoAvversita" runat="server" Style="display: none; width: 330px;"
                                CssClass="btn_per_load" meta:resourcekey="EventoAggiornamentoAvversitaResource1" />
                            <div class="cento" id="div_pannello_avversita_scroll" style="max-height: 200px; overflow: auto; width: 350px;">
                                <!-- DataGridGruppiAvversita -->
                                <%--                                <asp:GridView ID="GridViewGruppiAvversita" runat="server" AutoGenerateColumns="False"
                                    Width="330px" CellPadding="5" CssClass="ui-widget-content" meta:resourcekey="GridViewGruppiAvversitaResource1">
                                    <Columns>
                                        <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ChkSelezionaGruppoAvversita" runat="server" CssClass="ChkSelezionaGruppoAvversita"
                                                    meta:resourcekey="ChkSelezionaGruppoAvversitaResource1" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Av_Gru" HeaderText="Av_Gru" meta:resourcekey="BoundFieldResource1">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Av_Gru_Des" HeaderText="Gruppi Avversita'" HtmlEncode="False"
                                            meta:resourcekey="BoundFieldResource2"></asp:BoundField>
                                        <asp:BoundField DataField="Avversita_Infestanti" HeaderText="Avversita_Infestanti">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle CssClass="ui-widget-header" />
                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                </asp:GridView>--%>
                                <!-- DataGridAvversita -->
                                <asp:GridView ID="GridViewAvversita" runat="server" AutoGenerateColumns="False" Width="330px"
                                    CellPadding="5" CssClass="ui-widget-content" meta:resourcekey="GridViewAvversitaResource1">
                                    <columns>
                                        <asp:TemplateField meta:resourcekey="TemplateFieldResource2">
                                            <itemtemplate>
                                                <asp:CheckBox ID="ChkSelezionaAvversita" runat="server" CssClass="ChkSelezionaAvversita"
                                                    meta:resourcekey="ChkSelezionaAvversitaResource1" />
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Av_Cod" HeaderText="Av_Cod" meta:resourcekey="BoundFieldResource4">
                                            <itemstyle cssclass="displaynone" />
                                            <headerstyle cssclass="displaynone" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Av_Gru" HeaderText="Av_Gru" meta:resourcekey="BoundFieldResource1">
                                            <itemstyle cssclass="displaynone" />
                                            <headerstyle cssclass="displaynone" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Av_Des" HeaderText="Avversità / Infestanti" HtmlEncode="False" meta:resourcekey="BoundFieldResource5"></asp:BoundField>
                                        <asp:BoundField DataField="Avversita_Infestanti" HeaderText="Avversita_Infestanti">
                                            <itemstyle cssclass="displaynone" />
                                            <headerstyle cssclass="displaynone" />
                                        </asp:BoundField>
                                    </columns>
                                    <headerstyle cssclass="ui-widget-header" />
                                    <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                                </asp:GridView>
                            </div>
                        </contenttemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
            <td valign="top" style="border-right: 1px solid #A6C9E2; padding-left: 5px; min-width: 370px;">
                <asp:UpdatePanel ID="UpdatePanelFormulati" runat="server">
                    <contenttemplate>
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanelFormulati"
                            DisplayAfter="50">
                            <progresstemplate>
                                <div class="LoadPanel">
                                    <div class="loading-indicator-bars">
                                    </div>
                                </div>
                            </progresstemplate>
                        </asp:UpdateProgress>
                        <!-- formulati e testo ricerca -->
                        <table width="100%" aria-hidden="true">
                            <tr>
                                <td style="width: 245px; display: none">
                                    <asp:DropDownList ID="Cmb_FormulatoClassificazioni" runat="server" CssClass="txtUI Cmb_FormulatoClassificazioni myCombo"
                                        Style="min-width: 150px" meta:resourcekey="Cmb_FormulatoClassificazioniResource1">
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 10px; display: none"></td>
                                <td style="width: 100px">
                                    <b>
                                        <asp:Label ID="lblTestoRicerca" runat="server" meta:resourcekey="lblTestoRicercaResource1">Testo di Ricerca:</asp:Label></b>
                                </td>
                                <td>
                                    <asp:TextBox ID="Txt_Formulati" runat="server" CssClass="txtUI" Style="min-width: 50px; width: 95%"
                                        meta:resourcekey="Txt_FormulatiResource1">
                                    </asp:TextBox>
                                </td>
                                <td style="width: 40px">
                                    <div id="imgCerca" style="width: 32px" class="btn_per_load">
                                        <img onclick="$('#<%=btn_cerca.ClientID %>').click();" style="width: 32px" class="btn_per_load"
                                            alt="clicca per ricercare il prodotto" src="../AB_Immagini/icone32/lente.ico" />
                                    </div>
                                    <asp:Button ID="btn_cerca" runat="server" Text="Cerca" Style="display: none" />
                                </td>
                            </tr>
                        </table>
                        <!-- Prodotti -->
                        <table width="100%" aria-hidden="true">
                            <tr>
                                <td style="min-width: 570px">
                                    <cc1:comboformulati id="ComboFormulati" runat="server" class="classComboFormulati"
                                        meta:resourcekey="ComboFormulatiResource1" />
                                    <img id="ImgBtn_Info" src="../AB_Immagini/logo/logo_profitosan_small.jpg" style="cursor: pointer; vertical-align: middle"
                                        runat="server" onclick="Info();" alt="Profitosan" title="Profitosan" />

                                    <asp:Button ID="BTN_ComboFormulati" runat="server" Text="Button" Style="display: none"
                                        CssClass="btn_per_load" meta:resourcekey="BTN_ComboFormulatiResource1" />
                                    <br />
                                    <label id="Lbl_Num_Formulati" runat="server" style="color: red">
                                    </label>
                                    <br />
                                    <label id="Lbl_FormulatoInRevisione" runat="server" style="color: red; font-size: small"></label>

                                </td>

                            </tr>
                        </table>
                        <!-- Giacenze -->
                        <table width="100%" aria-hidden="true">
                            <tr>
                                <td style="background-color: #eee; width: 15%">
                                    <b>
                                        <asp:Label ID="lblGiacenzakgl" runat="server" meta:resourcekey="lblGiacenzakglResource1">Giacenza [kg] [l]:</asp:Label></b>
                                </td>
                                <td align="left" style="background-color: #eee; width: 30%; height: 50px;">
                                    <div style="height: 25px; float: left;">
                                        <asp:Label ID="Label1" runat="server" Style="width: 60px; margin-left: 5px;">Alla Data</asp:Label>
                                        <asp:Label ID="Lbl_Giacenza" runat="server" CssClass="txtUI" Style="min-width: 50px"
                                            meta:resourcekey="Lbl_GiacenzaResource1"></asp:Label>
                                    </div>
                                    <div style="height: 25px;">
                                        <asp:Label ID="Label2" runat="server" Style="width: 60px; margin-left: 5px;">Totale</asp:Label>
                                        <asp:Label ID="Lbl_GiacenzaTotale" runat="server" CssClass="txtUI" Style="min-width: 50px"
                                            meta:resourcekey="Lbl_GiacenzaResource1"></asp:Label>
                                    </div>
                                </td>
                                <td style="width: 2%"></td>
                                <td style="width: 15%">
                                    <asp:Label ID="lbl_qta_residua" runat="server" Visible="False" meta:resourcekey="lbl_qta_residuaResource1"><b>Dose/Ha Consentita [kg] [l]</b>:</asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="Lbl_Dose_Consigliata" runat="server" Style="min-width: 100px" Visible="False"
                                        meta:resourcekey="Lbl_Dose_ConsigliataResource1"></asp:Label>
                                </td>
                            </tr>
                            <!-- UDM e Dose Etichetta -->
                            <tr>
                                <td>
                                    <b>
                                        <asp:Label ID="lblUnitaMisura" runat="server" meta:resourcekey="lblUnitaMisuraResource1">Unita' di Misura:</asp:Label></b>
                                </td>
                                <td>
                                    <asp:DropDownList ID="Cmb_UdM" runat="server" AutoPostBack="True" CssClass="txtUI"
                                        Style="max-width: 100px" meta:resourcekey="Cmb_UdMResource1">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                                <td>
                                    <b>
                                        <asp:Label ID="lblDoseEtichetta" runat="server" meta:resourcekey="lblDoseEtichettaResource1">Dose Etichetta:</asp:Label></b>
                                </td>
                                <td>
                                    <asp:Label ID="Lbl_Dose_Etichetta" runat="server" ForeColor="Red" meta:resourcekey="Lbl_Dose_EtichettaResource1"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <!-- Acqua -->
                        <table width="100%" style="margin-top: 20px;" aria-hidden="true">
                            <tr style="background-color: #A6C9E2; text-align: center;">
                                <td>
                                    <b>
                                        <asp:Label ID="lblAcquaMiscelahl" runat="server" meta:resourcekey="lblAcquaMiscelahlResource1">Acqua nella Miscela [hl]</asp:Label></b>
                                </td>
                                <td style="min-width: 150px">
                                    <asp:RadioButton ID="rblAcqua_HA" CssClass="rblAcqua_HA" GroupName="rblAcqua" runat="server"
                                        Text="Quantità a Quintale" meta:resourcekey="rblAcqua_HAResource1" />
                                    <asp:TextBox ID="Txt_Acqua_Ha" runat="server" CssClass="txtUI AcquaHa" Style="margin-top: 5px; margin-left: 10px; width: 60px;"
                                        meta:resourcekey="Txt_Acqua_HaResource1">
                                    </asp:TextBox>
                                    <div style="width: 50px; float: right;">
                                        <asp:Label ID="lbl_acqua_provenienza" runat="server" Style="font-size: 10px" CssClass="lbl_acqua_provenienza"
                                            meta:resourcekey="lbl_acqua_provenienzaResource1"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <asp:RadioButton ID="rblAcqua_Tot" CssClass="rblAcqua_Tot" GroupName="rblAcqua" runat="server"
                                        Text="Quantità Totale" Checked="True" meta:resourcekey="rblAcqua_TotResource1" />
                                    <asp:TextBox ID="Txt_Acqua_Tot" runat="server" CssClass="txtUI AcquaTot" Style="margin-top: 5px; margin-left: 10px; width: 60px;"
                                        meta:resourcekey="Txt_Acqua_TotResource1">
                                    </asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        <!-- DOSE -->
                        <table style="width: 100%; padding: 0px; text-align: center; border: 1px solid #A6C9E2; margin-top: 20px;"
                            aria-hidden="true">
                            <tr>
                                <td></td>
                                <td style="border-right: 2px solid #A6C9E2;">
                                    <b>
                                        <asp:RadioButton ID="rbl_DoseHA" runat="server" GroupName="rblDose" Text="Dose/q"
                                            Checked="True" meta:resourcekey="rbl_DoseHAResource1" />
                                    </b>
                                </td>
                                <td>
                                    <b>
                                        <asp:RadioButton ID="rbl_DoseHL" runat="server" GroupName="rblDose" Text="Dose/Hl"
                                            meta:resourcekey="rbl_DoseHLResource1" />
                                    </b>
                                </td>
                            </tr>
                            <tr>
                                <td style="border-right: 2px solid #A6C9E2; text-align: right; padding-right: 5px;">
                                    <b>
                                        <asp:RadioButton ID="rbl_QtaDose" runat="server" GroupName="rbl_QtaTotHa" Text="Quantità Dose"
                                            Checked="True" meta:resourcekey="rbl_QtaDoseResource1" />
                                    </b>
                                </td>
                                <td style="border-right: 2px solid #A6C9E2;">
                                    <asp:TextBox ID="Txt_Dose_HA" runat="server" CssClass="txtUI qtaDose" Style="min-width: 100px"
                                        meta:resourcekey="Txt_Dose_HAResource1">
                                    </asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="Txt_Dose_HL" runat="server" CssClass="txtUI qtaDose" Style="min-width: 100px"
                                        meta:resourcekey="Txt_Dose_HLResource1">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr class="sfondoverde">
                                <td style="border-right: 2px solid #A6C9E2; text-align: right; padding-right: 5px;">
                                    <b>
                                        <asp:RadioButton ID="rbl_QtaTot" runat="server" GroupName="rbl_QtaTotHa" Text="Quantità Tal Quale"
                                            meta:resourcekey="rbl_QtaTotResource1" />
                                    </b>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="Txt_DoseTot_HA" runat="server" CssClass="txtUI qtaDoseTot" Style="min-width: 100px"
                                        meta:resourcekey="Txt_DoseTot_HAResource1">
                                    </asp:TextBox>
                                </td>
                                <%--<td>
                                    <asp:TextBox ID="Txt_DoseTot_HL" runat="server" CssClass="txtUI qtaDoseTot" Style="min-width: 100px"
                                        meta:resourcekey="Txt_DoseTot_HLResource1"></asp:TextBox>
                                </td>--%>
                            </tr>
                        </table>
                    </contenttemplate>
                    <triggers>
                        <asp:AsyncPostBackTrigger ControlID="ImgBtn_DoseInserisci" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="GridView_Dosi" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="BTN_ComboFormulati" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="EventoAggiornamentoAvversita" EventName="Click" />
                    </triggers>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="Script_Giacenza_Magazzino" runat="server">
                    <contenttemplate>
                    </contenttemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <div style="font-size: 10px; width: 100%" class="sfondoverde">
                    <div style="width: 300px">
                        <div style="float: left; margin-top: 3px; margin-bottom: 3px" id="InserisciDose">
                            <asp:Label ID="lblInserisciProdottoNellaMiscela" runat="server" meta:resourcekey="lblInserisciProdottoNellaMiscelaResource1">Inserisci il prodotto nella miscela</asp:Label>
                            <img src="../AB_Immagini/Icone16/FrecciaRossa_S.ico" alt="aggiungi dose" />
                        </div>
                        <asp:UpdatePanel ID="updateDoseInserisci" runat="server">
                            <contenttemplate>
                                <asp:ImageButton ID="ImgBtn_DoseInserisci" runat="server" ImageUrl="../AB_Immagini/icone32/frecciadn.ico"
                                    Style="width: 32px; display: none;" meta:resourcekey="ImgBtn_DoseInserisciResource1" />
                            </contenttemplate>
                            <triggers>
                                <asp:AsyncPostBackTrigger ControlID="ImgBtn_DoseInserisci" EventName="Click" />
                            </triggers>
                        </asp:UpdatePanel>
                        <div class="clear">
                        </div>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <table width="100%" aria-hidden="true">
                    <tr>
                        <td style="width: 100%">
                            <asp:UpdatePanel ID="UpdatePanelMiscela" runat="server">
                                <contenttemplate>
                                    <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="UpdatePanelMiscela"
                                        DisplayAfter="50">
                                        <progresstemplate>
                                            <div class="LoadPanel">
                                                <div class="loading-indicator-bars">
                                                </div>
                                            </div>
                                        </progresstemplate>
                                    </asp:UpdateProgress>
                                    <asp:GridView ID="GridView_Dosi" runat="server" AutoGenerateColumns="False" Width="100%"
                                        CellPadding="5" CssClass="ui-widget-content" Caption="Riepilogo della miscela di formulati da utilizzare nel trattamento"
                                        meta:resourcekey="GridView_DosiResource1">
                                        <columns>
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ce.ico' border='0'&gt; "
                                                HeaderText="Mod." CommandName="Modifica" meta:resourcekey="ButtonFieldResource1" />
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                                HeaderText="Canc." CommandName="Cancella" meta:resourcekey="ButtonFieldResource2" />
                                            <asp:BoundField DataField="Av_Cod" HeaderText="Av_Cod" meta:resourcekey="BoundFieldResource6">
                                                <itemstyle cssclass="displaynone" />
                                                <headerstyle cssclass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Av_Gru" HeaderText="Av_Gru" meta:resourcekey="BoundFieldResource7">
                                                <itemstyle cssclass="displaynone" />
                                                <headerstyle cssclass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Av_Des" HeaderText="Avversità / Infestanti" HtmlEncode="False"
                                                meta:resourcekey="BoundFieldResource8"></asp:BoundField>
                                            <asp:BoundField DataField="Fr_Cod" HeaderText="N. Reg. Min" meta:resourcekey="BoundFieldResource11"></asp:BoundField>
                                            <asp:BoundField DataField="Fr_Des" HeaderText="Formulato" meta:resourcekey="BoundFieldResource12"></asp:BoundField>
                                            <asp:BoundField DataField="Dose_Etichetta" HeaderText="Dose Etichetta" HtmlEncode="False"
                                                meta:resourcekey="BoundFieldResource13"></asp:BoundField>
                                            <asp:BoundField DataField="Dose_Etichetta_Max" HeaderText="Dose_Etichetta_Max" meta:resourcekey="BoundFieldResource14">
                                                <itemstyle cssclass="displaynone" />
                                                <headerstyle cssclass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Carenza" HeaderText="Tempo di Carenza" meta:resourcekey="BoundFieldResource15"></asp:BoundField>
                                            <asp:BoundField DataField="Udm_Cod" HeaderText="Udm_Cod" meta:resourcekey="BoundFieldResource17">
                                                <itemstyle cssclass="displaynone" />
                                                <headerstyle cssclass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Udm_Des" HeaderText="Unita' di Misura" meta:resourcekey="BoundFieldResource18"></asp:BoundField>
                                            <asp:BoundField DataField="Dose" HeaderText="Dose Reale" meta:resourcekey="BoundFieldResource19">
                                                <itemstyle cssclass="displaynone" />
                                                <headerstyle cssclass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Dose_Max" HeaderText="Dose Max" meta:resourcekey="BoundFieldResource20">
                                                <itemstyle cssclass="displaynone" />
                                                <headerstyle cssclass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Dose_HA" HeaderText="Dose [q]"></asp:BoundField>
                                            <asp:BoundField DataField="Dose_HL" HeaderText="Dose [HL]" meta:resourcekey="BoundFieldResource22"></asp:BoundField>
                                            <asp:BoundField DataField="Qta_Tot" HeaderText="Quantità Totale Distribuita" meta:resourcekey="BoundFieldResource23"></asp:BoundField>
                                            <asp:BoundField DataField="strPA_Cod" HeaderText="strPA_Cod">
                                                <itemstyle cssclass="displaynone" />
                                                <headerstyle cssclass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="strCLTOSS_COD" HeaderText="strCLTOSS_COD">
                                                <itemstyle cssclass="displaynone" />
                                                <headerstyle cssclass="displaynone" />
                                            </asp:BoundField>
                                        </columns>
                                        <headerstyle cssclass="ui-widget-header" />
                                        <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                                        <rowstyle cssclass="rigaImpianti" />
                                    </asp:GridView>
                                </contenttemplate>
                                <triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ImgBtn_DoseInserisci" EventName="Click" />
                                </triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
