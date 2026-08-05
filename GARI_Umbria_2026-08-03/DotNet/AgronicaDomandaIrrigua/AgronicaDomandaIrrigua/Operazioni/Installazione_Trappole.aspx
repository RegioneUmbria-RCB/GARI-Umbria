<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Operazione.master" ValidateRequest="false"
    CodeBehind="Installazione_Trappole.aspx.vb" Inherits="AgronicaDomandaIrrigua.Installazione_Trappole" meta:resourcekey="PageResource1" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentOperazioniHeader" runat="server">
    <style type="text/css">
        .classComboFertilizzanti .ui-autocomplete-input
        {
            min-width: 525px;
            width: 90%;
        }
    </style>
    <script type="text/javascript">

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
        }

        function QtaTOTChecked() {
        }

        function Abilita_Disabilita_DOSI() {
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
        }
        function InserisciAcquaTot(valore) {
        }

        function AcquaHA() {
        }
        function InserisciAcquaHA(valore) {
        }

        function DoseHA() {
        }
        function InserisciDoseHA(valore) {
        }

        function DoseHL() {
        }
        function InserisciDoseHL(valore) {
        }

        function TotHA() {
        }
        function InserisciTotHA(valore) {
        }

        function TotHL() {
        }
        function InserisciTotHL(valore) {
        }


        function AggiornaACQUA() {
        }


        function AggiornaDOSI() {
        }

        function AcquaTot_Keyup() {
        }
        function AcquaHA_Keyup() {
        }

        function AggiornaDopo_SupTrattata() {
            //Aggiorno i costi accessori
            CalcolaCostiAccessori();
        }


        function PulisciGrigliaAv_GrAv() {
        }




        //        function CHKAssociaCambia (Oggetto) {
        //            //controllo che non ho inserito alcuna dose 

        //            if (Oggetto.is(':checked')) {

        //                Oggetto.parent().parent().parent().find('.CHKAssocia').children('input:checked').each(function () {
        //                                        $(this).removeAttr('checked');
        //                });

        //                Oggetto.attr('checked', 'checked');
        //                }
        //        }


        $(document).ready(function () {
            //          $("#InserisciDose").button();
            //            $("#InserisciDose").click(function () { $("#<%=ImgBtn_DoseInserisci.ClientID %>").click(); });
        });

        //        $(document).ready(function () {
        //            $("#ModificaDose").button();
        //            
        //        });

        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlacexTRATTAMENTI" runat="server">
    <input type="hidden" id="HiddenVarie" runat="server" />
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
                <asp:Label ID="lblSupHaSelezionata" runat="server" 
                    meta:resourcekey="lblSupHaSelezionataResource1"><b>Sup. [Ha]</b> Selezionata:</asp:Label>
            </td>
            <td style="background-color: #cccccc;">
                <input type="text" runat="server" style="width: 60px" class="txtUI SommaSuperficie"
                    id="Txt_SupSelezionata"  disabled="disabled" readonly="readonly" value="0" />
            </td>
            <td style="width: 50px">
            </td>
        </tr>
        <tr style="background-color: #FFC0C0; display: none;">
            <td style="display: none;">
                <asp:Label ID="lblSupHaTrattata" runat="server" 
                    meta:resourcekey="lblSupHaTrattataResource1"><b>Sup. [Ha]</b> Trattata:</asp:Label>
            </td>
            <td style="display: none;">
                <input type="text" runat="server" style="width: 60px" class="txtUI SommaSuperficieTrattata"
                    id="Txt_SupTrattata" value="0" />
            </td>
            <td style="width: 50px">
            </td>
        </tr>
    </table>
    <!-- DATI Trappole -->
    <asp:UpdatePanel ID="UpdatePanelFertilizzanti" runat="server">
        <ContentTemplate>
            <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="UpdatePanelFertilizzanti"
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
                    <td>
                        <b><asp:Label ID="lblDispenser" runat="server">Dispenser:</asp:Label></b>
                    </td>
                    <td>
                        <asp:DropDownList ID="cmb_Trappola" runat="server" CssClass="txtUI" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                    <td style="width: 50px">
                    </td>
                    <td>
                        <b><asp:Label ID="lblDittaFornitrice" runat="server">Ditta Fornitrice:</asp:Label></b>
                    </td>
                    <td>
                        <asp:DropDownList ID="cmb_Ditte" runat="server" CssClass="txtUI" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b><asp:Label ID="lblAvversita" runat="server">Avversità:</asp:Label></b>
                    </td>
                    <td>
                        <asp:DropDownList ID="cmb_Avversita" runat="server" CssClass="txtUI" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                    <td>
                        <b><asp:Label ID="lblCodiceAvversita" runat="server">Codice Avversità:</asp:Label></b>
                    </td>
                    <td>
                        <asp:Label ID="Txt_CodAvversita" runat="server" CssClass="txtUI" Style="min-width: 100px"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b><asp:Label ID="lblNumeroTotale" runat="server">Numero Totale:</asp:Label></b>
                    </td>
                    <td>
                        <asp:TextBox ID="Txt_NumeroTrappole" runat="server" CssClass="txtUI"></asp:TextBox>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b><asp:Label ID="lblGiacenzeInneschi" runat="server">Giacenze [Inneschi]</asp:Label></b>
                    </td>
                    <td>
                        <asp:Label ID="Txt_GiacenzaInneschi" runat="server" CssClass="txtUI" Style="min-width: 100px"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td>
                        <b><asp:Label ID="lblGiacenzeTrappole" runat="server">Giacenze [Trappole]:</asp:Label></b>
                    </td>
                    <td>
                        <asp:Label ID="Txt_Giacenza" runat="server" CssClass="txtUI" Style="min-width: 100px"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b><asp:Label ID="lblDurataFeromonegg" runat="server">Durata Feromone [gg.]:</asp:Label></b>
                    </td>
                    <td>
                        <asp:Label ID="Txt_GiorniFeromone" runat="server" CssClass="txtUI" Style="min-width: 100px"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td>
                        <b><asp:Label ID="lblScadenza" runat="server">Scadenza:</asp:Label></b>
                    </td>
                    <td>
                        <asp:Label ID="Txt_DataScadenz" runat="server" CssClass="txtUI" Style="min-width: 100px"></asp:Label>
                    </td>
                </tr>
            </table>
        
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ImgBtn_DoseInserisci" EventName="Click" />
        </Triggers>
    
    <table width="100%" aria-hidden="true">
        <tr>
            <td colspan="3" align="center">
                <div style="font-size: 10px; width: 100%" class="sfondoverde" id="sfondoverde" runat="server">
                    <%--  <div style="width: 100px">

                         <div style="float: left; margin-top: 3px; margin-bottom: 3px" id="InserisciDose">
                            &nbsp;<asp:Label ID="Lbl_Inserisci" runat="server" Text="Inserisci"></asp:Label>
                            <img src="../AB_Immagini/Icone16/FrecciaRossa_S.ico" alt="aggiungi dose" />
                            <asp:ImageButton ID="ImageButton_Sblocca" runat="server" Height="23px" 
                                ImageUrl="~/AB_Immagini/Icone32/FrecciaUP.ico" Width="32px" />
                        </div>
                        <asp:UpdatePanel ID="updatepanelInserisci" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:ImageButton ID="ImgBtn_DoseInserisci" runat="server" ImageUrl="../AB_Immagini/icone32/frecciadn.ico"
                                    ToolTip="" Style="width: 32px; display: none;" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="clear">
                        </div>
                    </div>--%>
                    <div style="width: 100px; height: 32px;">
                        <asp:ImageButton ID="ImgBtn_DoseInserisci" runat="server" ImageUrl="~/AB_Immagini/icone32/frecciadn.ico"
                            ToolTip="" Style="width: 32px; display: none;" />
                        <%--              <asp:UpdatePanel ID="updatepanelTrappoleMostra" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:ImageButton ID="ImgBtn_TrappoleMostra" runat="server" ImageUrl="~/AB_Immagini/Icone16/FrecciaRossa_S.ico"
                                    ToolTip="" Style="width: 32px; display: none;" />
                            </ContentTemplate>
                        </asp:UpdatePanel>--%>
                        <div class="clear">
                            <br />
                            <asp:ImageButton ID="ImageButton_MostraTrappole" runat="server" Height="23px" ImageUrl="~/AB_Immagini/Icone32/FrecciaDN.ico"
                                Width="32px" />
                            <asp:ImageButton ID="ImageButton_Sblocca" runat="server" Height="23px" ImageUrl="~/AB_Immagini/Icone32/FrecciaUP.ico"
                                Width="32px" />
                            <br />
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Gridview Impianti Selezionati -->
    <asp:UpdatePanel ID="UpdatePanelGridImpianti" runat="server">
        <ContentTemplate>
            <div style="overflow: auto; width: 100%">
                <asp:PlaceHolder ID="PlaceTabella" runat="server"></asp:PlaceHolder>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
