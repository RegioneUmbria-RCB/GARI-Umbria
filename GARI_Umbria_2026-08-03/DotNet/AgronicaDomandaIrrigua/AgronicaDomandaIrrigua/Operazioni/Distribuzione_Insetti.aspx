<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Operazione.master"
    ValidateRequest="false" CodeBehind="Distribuzione_Insetti.aspx.vb" Inherits="AgronicaDomandaIrrigua.Distribuzione_Insetti"
    meta:resourcekey="PageResource1" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentOperazioniHeader" runat="server">
    <style type="text/css">
        .classComboFertilizzanti .ui-autocomplete-input {
            min-width: 525px;
            width: 90%;
        }
    </style>
    <script type="text/javascript">

        function BloccaSbloccaChkImpollinatore() {

            if ($('.rigaImpianti').length > 0) {

                $("#<%=ChkImpollinatore.ClientID %>").attr('disabled', 'disabled');
            }
            else {

                $("#<%=ChkImpollinatore.ClientID %>").removeAttr('disabled');

            }
        }

        function Impollinatore() {
            $('#<%=ChkImpollinatore.ClientID %>').click(function () {
                if ($('#<%=ChkImpollinatore.ClientID %>').is(':checked')) {
                    $('.avversita').hide();
                } else {
                    $('.avversita').show();
                }
            });
        }


        function Verifica_Tasto_Premuto() {
            switch (window.event.keyCode) {
                case 13:
                    //invio 
                    if ($("*:focus").attr('id').toString() == "<%=Txt_Insetti.ClientID %>") {
                        $('#<%=ImgBtn_Cerca.ClientID %>').click();
                    } else { return false; }
                    break;
            }
        }

        //per la Gestione dell'Eliminazione 
        function DoPostBack_Combo_Slave($_combo, valoreOpt) {
            //postBack Fertilizzanti
            if ($_combo.attr("id").endsWith("ComboFertilizzanti")) {

            }
        }

        function DoPostBack_ControlliSiNo(key) {
            if (key == 'Giacenze') {
                $("#<%=Giacenza_SI_NO.ClientID %>").val("OK");
                $("#<%=Master_Operazione.Property_ImgBtn_Salva.ClientID %>").click();
            }
        }


        //per collegarsi al sito del profitosan
        function Info() {

        }


        ///////////////////////////////////////////
        /////////////ABILITA DISABIITA/////////////
        ///////////////////////////////////////////


        function AcquaTotChecked() {
        }
        function Abilita_Disabilita_ACQUA() {
        }


        function DoseHAChecked() {
            return true;
        }

        function QtaTOTChecked() {
            return $('#<%=rbl_QtaTot.ClientId %>').is(':checked');
        }

        function Abilita_Disabilita_DOSI() {
            var dose_HA, doseTot_HA;
            dose_HA = false;
            doseTot_HA = false;

            if (DoseHAChecked()) {
                //DOSE HA
                if (QtaTOTChecked()) { doseTot_HA = true; }
                else { dose_HA = true; }
            }

            //imposto
            if (dose_HA == false) {
                $('#<%=Txt_Dose_Ha.ClientID %>').attr('disabled', 'disabled');
            } else {
                $('#<%=Txt_Dose_Ha.ClientID %>').removeAttr('disabled');
            }


            if (doseTot_HA == false) {
                $('#<%=Txt_DoseTot_Ha.ClientID %>').attr('disabled', 'disabled');
            } else {
                $('#<%=Txt_DoseTot_Ha.ClientID %>').removeAttr('disabled');
            }

        }




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
        }
        function InserisciAcquaTot(valore) {
        }

        function AcquaHA() {
        }
        function InserisciAcquaHA(valore) {
        }

        function DoseHA() {
            return $('#<%=Txt_Dose_Ha.ClientId %>').val().replace(',', '.');
        }
        function InserisciDoseHA(valore) {
            $('#<%=Txt_DoseTot_Ha.ClientId %>').val($('#<%=Txt_DoseTot_Ha.ClientId %>').val().replace(',', ''));
            $('#<%=Txt_DoseTot_Ha.ClientId %>').val($('#<%=Txt_DoseTot_Ha.ClientId %>').val().replace('.', ''));
            var app = roundNumber(valore, 4) + "";
            //var app = roundNumber(valore, 0) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Dose_Ha.ClientId %>').val(app);
        }

        function DoseHL() {
        }
        function InserisciDoseHL(valore) {
        }

        function TotHA() {
            return $('#<%=Txt_DoseTot_Ha.ClientId %>').val().replace(',', '.');
        }
        function InserisciTotHA(valore) {
            $('#<%=Txt_Dose_Ha.ClientId %>').val($('#<%=Txt_Dose_Ha.ClientId %>').val().replace(',', ''));
            $('#<%=Txt_Dose_Ha.ClientId %>').val($('#<%=Txt_Dose_Ha.ClientId %>').val().replace('.', ''));
            var app = roundNumber(valore, 4) + "";
            //var app = roundNumber(valore, 0) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_DoseTot_Ha.ClientId %>').val(app);
        }

        function TotHL() {
        }
        function InserisciTotHL(valore) {
        }


        function AggiornaACQUA() {
        }


        function AggiornaDOSI() {
            var _SupTrattata = SupTrattata();
            var _SupTotale = SupTotale();
            InserisciSupTotale(_SupTotale)
            InserisciSupTrattata(_SupTrattata)
            if (DoseHAChecked()) {
                //LAVORO A HA
                if (QtaTOTChecked()) {
                    //LAVORO A TAL QUALE
                    if (_SupTrattata > 0) {
                        var _DoseTotaleHA = TotHA();
                        var _DoseHA = _DoseTotaleHA / _SupTrattata;
                        InserisciDoseHA(_DoseHA);
                    }
                }
                else {
                    //LAVORO A DOSE
                    if (_SupTrattata > 0) {
                        var _DoseHA = DoseHA();
                        var _DoseTotaleHA = _SupTrattata * _DoseHA;
                        InserisciTotHA(_DoseTotaleHA);

                    }
                }
            }
        }


        function AcquaTot_Keyup() {
        }
        function AcquaHA_Keyup() {
        }

        function AggiornaDopo_SupTrattata() {
            //Aggiorno le Dosi solo se ho la qta/ha selezionata
            AggiornaDOSI();
            //Agggiorno i costi accessori
            CalcolaCostiAccessori();
        }


        function PulisciGrigliaAv_GrAv() {
        }


        $(document).ready(function () {
            $("#InserisciDose").button();
            $("#InserisciDose").click(function () { $("#<%=ImgBtn_DoseInserisci.ClientID %>").click(); });
            Impollinatore();
        });


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

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlacexTRATTAMENTI" runat="server">
    <input type="hidden" id="HiddenVarie" runat="server" />
    <div class="box50" runat="server" id="pannelloFiltriRicerca" style="min-width: 350px">
        <div class="descrizione" style="width: 80px">
            <asp:Label ID="lblFiltriRicerca" runat="server" meta:metaresourcekey="lblFiltriRicercaResource1">Filtri Ricerca</asp:Label>
        </div>
        <div class="valoriinput">
            <asp:UpdatePanel ID="UpdatePanelFiltriAggiuntivi" runat="server">
                <ContentTemplate>
                    <cc1:ComboFiltriAggiuntiviAgenda ID="ComboFiltriAggiuntiviAgenda1" runat="server"
                        meta:resourcekey="ComboFiltriAggiuntiviAgenda1Resource1" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentOperazioniContenuti" runat="server">
    <!-- hidden per si no  -->
    <asp:UpdatePanel ID="updateGiacenze_si_no" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="Giacenza_SI_NO" runat="server" Value="0" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Dati Superficie-->
    <table width="50%" aria-hidden="true">
        <tr>
            <td style="background-color: #cccccc;">
                <asp:Label ID="lblSupHaSelezionata" runat="server" meta:resourcekey="lblSupHaSelezionataResource1"><b>Sup. [Ha]</b> Selezionata:</asp:Label>
            </td>
            <td style="background-color: #cccccc;">
                <input type="text" runat="server" style="width: 60px" class="txtUI SommaSuperficie"
                    id="Txt_SupSelezionata" disabled="disabled" readonly="readonly" value="0" />
            </td>
            <td style="width: 50px"></td>
        </tr>

        <tr>
            <td style="background-color: #FFC0C0;">
                <asp:Label ID="lblSupHaTrattata" runat="server" meta:resourcekey="lblSupHaTrattataResource1"><b>Sup. [Ha]</b> Trattata:</asp:Label>
            </td>
            <td style="background-color: #FFC0C0;">
                <input type="text" runat="server" style="width: 60px" class="txtUI SommaSuperficieTrattata"
                    id="Txt_SupTrattata" disabled="disabled" readonly="readonly" value="0" />
            </td>
            <td style="width: 50px"></td>
        </tr>
    </table>
    <table width="100%" aria-hidden="true">
        <tr id="Riga_Formulati" runat="server">
            <td id="Cella_Avversita" runat="server" valign="top" style="border-right: 1px solid #A6C9E2; width: 350px;"
                class="avversita">
                <div id="PannelloAvversita" runat="server">
                    <%--                    <div style="width: 350px;">
                        <div style="float: left">
                            <label id="Lbl_Avversita" runat="server" class="descrizione" style="font-size: 10px;
                                width: 65px;">
                            </label>
                        </div>
                        <div style="float: left;">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="RBL_Avversita" runat="server" RepeatDirection="Horizontal"
                                        Style="font-size: 10px; width: 210px;" CssClass="txtUI" AutoPostBack="True" meta:resourcekey="RBL_AvversitaResource1">
                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource1">Avversita&#39;</asp:ListItem>
                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Gruppi Avversita&#39;</asp:ListItem>
                                    </asp:RadioButtonList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="clear">
                    </div>--%>
                    <div style="width: 350px;">
                        <div style="float: left;">

                            <asp:Label ID="Label3" runat="server" meta:resourcekey="lblTestoRicercaResource1" Style="font-size: 10px; margin-left: 5px">Testo di Ricerca:</asp:Label>
                        </div>
                        <div style="float: left;">
                            <asp:TextBox ID="Txt_Avv" runat="server" CssClass="txtUI" Width="150px" Style="margin-left: 5px"></asp:TextBox>
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
                        <ContentTemplate>
                            <asp:UpdateProgress ID="UpdateProgressUpdatePanelAvversita" runat="server" AssociatedUpdatePanelID="UpdatePanelAvversita"
                                DisplayAfter="50">
                                <ProgressTemplate>
                                    <div class="LoadPanel">
                                        <div class="loading-indicator-bars">
                                        </div>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <asp:Button ID="EventoAggiornamentoAvversita" runat="server" Style="display: none; width: 330px;"
                                CssClass="btn_per_load" meta:resourcekey="EventoAggiornamentoAvversitaResource1" />
                            <div class="cento" style="max-height: 200px; overflow: auto; width: 350px;">
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
                                       
                                    </Columns>
                                    <HeaderStyle CssClass="ui-widget-header" />
                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                </asp:GridView>
                                --%>
                                <!-- DataGridGruppiAvversita -->
                                <asp:GridView ID="GridViewAvversita" runat="server" AutoGenerateColumns="False" Width="330px"
                                    CellPadding="5" CssClass="ui-widget-content" meta:resourcekey="GridViewAvversitaResource1">
                                    <Columns>
                                        <asp:TemplateField meta:resourcekey="TemplateFieldResource2">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ChkSelezionaAvversita" runat="server" CssClass="ChkSelezionaAvversita"
                                                    meta:resourcekey="ChkSelezionaAvversitaResource1" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Av_Cod" HeaderText="Av_Cod" meta:resourcekey="BoundFieldResource4">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Av_Gru" HeaderText="Av_Gru" meta:resourcekey="BoundFieldResource1">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Av_Des" HeaderText="Avversità / Gruppi Avversità" HtmlEncode="False"
                                            meta:resourcekey="BoundFieldResource5"></asp:BoundField>
                                    </Columns>
                                    <HeaderStyle CssClass="ui-widget-header" />
                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
            <td valign="top" style="border-right: 1px solid #A6C9E2; padding-left: 5px; min-width: 370px;">
                <div>
                    <asp:CheckBox ID="ChkImpollinatore" Checked="false" Text="Distribuzione per Impollinazione (non utilizza l'Avversità)"
                        runat="server" />
                </div>
                <asp:UpdatePanel ID="UpdatePanelFormulati" runat="server">
                    <ContentTemplate>
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanelFormulati"
                            DisplayAfter="50">
                            <ProgressTemplate>
                                <div class="LoadPanel">
                                    <div class="loading-indicator-bars">
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <!-- formulati e testo ricerca -->
                        <table width="100%" aria-hidden="true">
                            <tr>
                                <td style="width: 100px">
                                    <b>
                                        <asp:Label ID="lblTestoRicerca" runat="server" meta:resourcekey="lblTestoRicercaResource1">Testo di Ricerca:</asp:Label></b>
                                </td>
                                <td>
                                    <asp:TextBox ID="Txt_Insetti" runat="server" CssClass="txtUI" Style="min-width: 50px; width: 95%"></asp:TextBox>
                                </td>
                                <td style="width: 40px">
                                    <asp:ImageButton ID="ImgBtn_Cerca" runat="server" ImageUrl="../AB_Immagini/icone32/lente.ico"
                                        ToolTip="Premere il pulsante per caricare gli insetti utili nella lista" Style="width: 32px"
                                        CssClass="btn_per_load" />
                                </td>
                            </tr>
                        </table>
                        <!-- Prodotti -->
                        <table width="100%" aria-hidden="true">
                            <tr>
                                <td style="min-width: 570px">
                                    <asp:DropDownList ID="Cmb_Insetti" runat="server" CssClass="txtUI" AutoPostBack="true">
                                    </asp:DropDownList>
                                    <br />
                                    <label id="Lbl_Num_Insetti" runat="server" style="color: red">
                                    </label>
                                </td>
                            </tr>
                        </table>
                        <!-- Giacenze -->
                        <table width="100%" aria-hidden="true">
                            <tr>
                                <td style="background-color: #eee; width: 15%">
                                    <b>
                                        <asp:Label ID="lblGiacenzan" runat="server">Giacenza [n]:</asp:Label></b>
                                </td>
                                <td align="left" style="background-color: #eee; width: 20%">
                                    <asp:Label ID="Lbl_Giacenza" runat="server" CssClass="txtUI" Style="min-width: 100px"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <!-- DOSE -->
                        <table style="width: 100%; padding: 0px; text-align: center; border: 1px solid #A6C9E2; margin-top: 20px;" aria-hidden="true">
                            <tr>
                                <td style="border-right: 2px solid #A6C9E2; text-align: right; padding-right: 5px;">
                                    <b>
                                        <asp:RadioButton ID="rbl_QtaDose" runat="server" GroupName="rbl_QtaTotHa" Text="Dose Ha"
                                            Checked="True" meta:resourcekey="rbl_QtaDoseResource1" /></b>
                                </td>
                                <td style="border-right: 2px solid #A6C9E2;">
                                    <asp:TextBox ID="Txt_Dose_HA" runat="server" CssClass="txtUI qtaDose" Style="min-width: 100px"
                                        meta:resourcekey="Txt_Dose_HAResource1"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="sfondoverde">
                                <td style="border-right: 2px solid #A6C9E2; text-align: right; padding-right: 5px;">
                                    <b>
                                        <asp:RadioButton ID="rbl_QtaTot" runat="server" GroupName="rbl_QtaTotHa" Text="Quantità Tal Quale"
                                            meta:resourcekey="rbl_QtaTotResource1" /></b>
                                </td>
                                <td style="border-right: 2px solid #A6C9E2;">
                                    <asp:TextBox ID="Txt_DoseTot_HA" runat="server" CssClass="txtUI qtaDoseTot" Style="min-width: 100px"
                                        meta:resourcekey="Txt_DoseTot_HAResource1"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ImgBtn_DoseInserisci" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="GridView_Dosi" EventName="RowCommand" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="Script_Giacenza_Magazzino" runat="server">
                    <ContentTemplate>
                    </ContentTemplate>
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
                            <ContentTemplate>
                                <asp:ImageButton ID="ImgBtn_DoseInserisci" runat="server" ImageUrl="../AB_Immagini/icone32/frecciadn.ico"
                                    Style="width: 32px; display: none;" meta:resourcekey="ImgBtn_DoseInserisciResource1" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ImgBtn_DoseInserisci" EventName="Click" />
                            </Triggers>
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
                                <ContentTemplate>
                                    <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="UpdatePanelMiscela"
                                        DisplayAfter="50">
                                        <ProgressTemplate>
                                            <div class="LoadPanel">
                                                <div class="loading-indicator-bars">
                                                </div>
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                    <asp:GridView ID="GridView_Dosi" runat="server" AutoGenerateColumns="False" Width="100%"
                                        CellPadding="5" CssClass="ui-widget-content" Caption="Riepilogo insetti da distribuire nel trattamento"
                                        meta:resourcekey="GridView_DosiResource1">
                                        <Columns>
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ce.ico' border='0'&gt; "
                                                HeaderText="Mod." CommandName="Modifica" meta:resourcekey="ButtonFieldResource1" />
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                                HeaderText="Canc." CommandName="Cancella" meta:resourcekey="ButtonFieldResource2" />
                                            <asp:BoundField DataField="Ins_Cod" HeaderText="Ins_Cod">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Ins_Des" HeaderText="Insetto/Acaro"></asp:BoundField>
                                            <asp:BoundField DataField="Dose_HA" HeaderText="Numero [HA]"></asp:BoundField>
                                            <asp:BoundField DataField="Qta_Tot" HeaderText="Quantità Totale Distribuita"></asp:BoundField>
                                        </Columns>
                                        <HeaderStyle CssClass="ui-widget-header" />
                                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                        <RowStyle CssClass="rigaImpianti" />
                                    </asp:GridView>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ImgBtn_DoseInserisci" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
