<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Operazione.master"
    ValidateRequest="false" CodeBehind="RilievoPiogge.aspx.vb" Inherits="AgroAgenda_2010.RilievoPiogge"
    meta:resourcekey="PageResource1" %>


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
        }

//        window.addEventListener("message", function (ev) {
//            if (ev.data.message === "lanciaWSmappe") {
//                $("#<%=BottoneNascostoWS.ClientID %>").click()
//            }
//        });
        function clickButtonWS() {

            $("#<%=BottoneNascostoWS.ClientID %>").click()

        }

        function SelezionaDeselezionaTuttiCentri() {
            if ($('#chkSelezionaTuttiCentri').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaCentro').each(function () {
                    $(this).children('input').attr('checked', 'checked');
                    //                    ChkSelezionaImpianto_Click($(this).children('input'));
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaCentro').each(function () {
                    $(this).children('input').removeAttr('checked');
                    //                    ChkSelezionaImpianto_Click($(this).children('input'));
                });
            }
        }


        function SelezionaDeselezionaTuttiRilievi() {
            if ($('#chkSelezionaTuttiRilievi').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaRilievo').each(function () {
                    $(this).children('input').attr('checked', 'checked');
                    //                    ChkSelezionaImpianto_Click($(this).children('input'));
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaRilievo').each(function () {
                    $(this).children('input').removeAttr('checked');
                    //                    ChkSelezionaImpianto_Click($(this).children('input'));
                });
            }
            CalcolaRilieviSelezionati();
        }

        function ChkSelezionaRilievo_Click() {
            CalcolaRilieviSelezionati();
        }

        function CalcolaRilieviSelezionati() {
            var sup = 0.00;
            var num = 0.00;
            $('#<%= GridView_Piogge.ClientID %>').find('.ChkSelezionaRilievo').each(function () {
                if ($(this).children('input').is(':checked')) {
                    num = num + 1;
                    $(this).parent().parent().find('.Txt_mm_pioggia').each(function () {
                        sup = sup + parseFloat($(this).val().replace(',', '.'));
                    });
                }
            });
            var valore = roundNumber(sup, 4) + "";
            valore = valore.replace(".", ",");
            $('#<%= lblDataA3.ClientID %>').text(valore);
        }

        function UDMDoseCambiata() {

        }

        function CopiaValoriColonnaTipoIrrig(pippo) {
        }

        function CopiaValoriColonna(pippo) {

        }

        function getUDM() {

        }


        function DoseCambiata(pippo) {

        }

        function AggiornaDose(pippo) {

        }

        function OreCambiata(pippo) {

        }

        function PortataCambiata(pippo) {

        }

        function QtaTotCambiata(pippo) {

        }


        function DoPostBack_ControlliSiNo(key) {
        }

        function Info() {

        }

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
        /////////////////////////////////////////////////////////


        ///////////////////////////////////
        ////////RICAVA INSERISCI///////////
        ///////////////////////////////////

        function SupTrattata() {
            
        }

        function InserisciSupTrattata(valore) {
          
        }

        function SupTotale() {
            
        }

        function InserisciSupTotale(valore) {
           
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

        $(document).ready(function () {

        });

        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlacexTRATTAMENTI" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentOperazioniContenuti" runat="server">
    <asp:UpdatePanel ID="UpdatePanelFiltroCentri" runat="server">
        <ContentTemplate>
            <table id="TableRBL_Coordinate" style="width: 40%; padding: 0px; text-align: left;
                border: 1px solid #A6C9E2; margin-top: 20px;" aria-hidden="true">
                <tr>
                    <td class="sfondoverde" style="width: 120px">
                        <b>
                            <asp:Label ID="lblRilevaPiogge" runat="server" meta:resourcekey="lblRilevaPioggeResource1">Rileva Piogge:</asp:Label></b>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td style="width: 75%">
                        <asp:RadioButtonList ID="RBL_TipoRilievo" runat="server" Style="float: left;" Font-Size="10px"
                            CellPadding="0" CellSpacing="0" AutoPostBack="True" RepeatDirection="Horizontal"
                            meta:resourcekey="RBL_TipoRilievoResource1">
                            <asp:ListItem Text="Da Dati Meteo Gias (solo RER)" Value="0" meta:resourcekey="TipoRilievoListItemResource0" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Manualmente" Value="1" meta:resourcekey="TipoRilievoListItemResource1"></asp:ListItem>
                            <asp:ListItem Text="Da Stazioni" Value="2" meta:resourcekey="TipoRilievoListItemResource2"></asp:ListItem>
                            <asp:ListItem Text="Dati Agronica" Value="3" meta:resourcekey="TipoRilievoListItemResource3"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
            <table style="width: 90%; margin-top: 20px;" aria-hidden="true">
                <tr>
                    <td align="center">
                        <b>
                            <asp:Label ID="LabelInserisciCentri" runat="server" Text=" Inserisci i Centri da Rilevare "
                                meta:resourcekey="LabelInserisciCentriResource1"></asp:Label></b>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="left">
                        <div style="font-size: 10px; width: 100%" class="sfondoverde" id="sfondoverdeCentri"
                            runat="server" align="center">
                            <div style="width: 100px; height: 40px;">
                                <div class="clear">
                                    <br />
                                    <asp:ImageButton ID="IMGB_InserisciCentri" runat="server" Height="23px" ImageUrl="~/AB_Immagini/Icone32/FrecciaDN.ico"
                                        Width="32px" meta:resourcekey="IMGB_InserisciCentriResource1" />
                                    <asp:ImageButton ID="IMGB_RimuoviCentri" runat="server" Height="23px" ImageUrl="~/AB_Immagini/Icone32/FrecciaUP.ico"
                                        Width="32px" meta:resourcekey="IMGB_RimuoviCentriResource1" />
                                    <br />
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel ID="updatepanelGridViewCentri" runat="server">
                <ContentTemplate>
                    <asp:Button ID="BottoneNascostoWS" runat="server" Text="BottoneNascostoPerRecupeoCoordinateWS"
                        Style="display: none" meta:resourcekey="BottoneNascostoWSResource1" />
                    <asp:GridView ID="GridViewCentriMeteo" runat="server" AutoGenerateColumns="False"
                        Width="900px" CellPadding="5" CssClass="ui-widget-content" Caption="Seleziona i Centri Aziendali"
                        meta:resourcekey="GridViewCentriMeteoResource1">
                        <Columns>
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                <HeaderTemplate>
                                    <input type="checkbox" id="chkSelezionaTuttiCentri" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="ChkSelezionaCentro" Checked="True" runat="server" CssClass="ChkSelezionaCentro"
                                        meta:resourcekey="ChkSelezionaCentroResource1" />
                                </ItemTemplate>
                                <HeaderStyle Width="20px" />
                                <ItemStyle Width="20px" />
                                <FooterStyle Width="20px" />
                                <ControlStyle Width="20px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" SortExpression="Rag_Soc"
                                Visible="False" meta:resourcekey="BoundFieldResource1"></asp:BoundField>
                            <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" SortExpression="Sa_Nome"
                                meta:resourcekey="BoundFieldResource2"></asp:BoundField>
                            <asp:TemplateField HeaderText="Coordinata X" SortExpression="CoordX" meta:resourcekey="TemplateFieldResource2">
                                <ItemTemplate>
                                    <asp:TextBox ID="Txt_CoordX" runat="server" CssClass="Txt_CoordX" Text='0' meta:resourcekey="Txt_CoordXResource1" />
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="90px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Coordinata Y" SortExpression="CoordY" meta:resourcekey="TemplateFieldResource3">
                                <ItemTemplate>
                                    <asp:TextBox ID="Txt_CoordY" runat="server" CssClass="Txt_CoordY" Text='0' meta:resourcekey="Txt_CoordYResource1" />
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="90px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Numero Quadrante" SortExpression="NumeroQuadrante"
                                meta:resourcekey="TemplateFieldResource4">
                                <ItemTemplate>
                                    <asp:TextBox ID="Txt_NumeroQuadrante" runat="server" CssClass="Txt_NumeroQuadrante"
                                        Text='0' meta:resourcekey="Txt_NumeroQuadranteResource1" />
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="90px" />
                            </asp:TemplateField>
                            <asp:ButtonField HeaderText="Ricerca Per Indirizzo" Text="&lt;img src='../AB_Immagini/icone32/Lente.ico' border='0'&gt;"
                                CommandName="RicercaPerIndirizzo" meta:resourcekey="ButtonFieldResource1">
                                <ControlStyle Width="60px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:ButtonField>
                            <asp:ButtonField HeaderText="Salva Coordinate" Text="&lt;img src='../AB_Immagini/icone32/Dischetto.ico' border='0'&gt;"
                                CommandName="SalvaCoordinate" meta:resourcekey="ButtonFieldResource2">
                                <ControlStyle Width="60px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:ButtonField>
                            <asp:TemplateField HeaderText="Ricerca per:" SortExpression="ModifTipoIrrig" meta:resourcekey="TemplateFieldResource5">
                                <ItemTemplate>
                                    <asp:RadioButtonList ID="RBL_Coordinate" runat="server" Style="float: left;" Font-Size="10px"
                                        CellPadding="0" CellSpacing="0" meta:resourcekey="RBL_CoordinateResource1">
                                        <asp:ListItem Selected="True" Text="Coordinate UTM" Value="0" meta:resourcekey="ListItemResource3"></asp:ListItem>
                                        <asp:ListItem Text="Quadrante" Value="1" meta:resourcekey="ListItemResource4"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </ItemTemplate>
                                <ControlStyle Width="110px" />
                                <HeaderStyle Width="120px" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="ui-widget-header" />
                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                    </asp:GridView>
                    <asp:GridView ID="GridViewCentriManuale" runat="server" AutoGenerateColumns="False"
                        Width="50%" CellPadding="5" CssClass="ui-widget-content" Caption="Seleziona i Centri Aziendali e il numero di rilievi per centro"
                        meta:resourcekey="GridViewCentriManualeResource1">
                        <Columns>
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource6">
                                <HeaderTemplate>
                                    <input type="checkbox" id="chkSelezionaTuttiCentri" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="ChkSelezionaCentro" runat="server" Checked="True" CssClass="ChkSelezionaCentro"
                                        meta:resourcekey="ChkSelezionaCentroResource2" />
                                </ItemTemplate>
                                <HeaderStyle Width="20px" />
                                <ItemStyle Width="20px" />
                                <FooterStyle Width="20px" />
                                <ControlStyle Width="20px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" SortExpression="Rag_Soc"
                                Visible="False" meta:resourcekey="BoundFieldResource3"></asp:BoundField>
                            <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" SortExpression="Sa_Nome"
                                meta:resourcekey="BoundFieldResource4"></asp:BoundField>
                            <asp:TemplateField HeaderText="Numero di rilievi da effettuare" SortExpression="NumRilievi"
                                meta:resourcekey="TemplateFieldResource7">
                                <ItemTemplate>
                                    <asp:TextBox ID="Txt_NumRilievi" runat="server" CssClass="Txt_NumRilievi" Text='1'
                                        meta:resourcekey="Txt_NumRilieviResource1" />
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="ui-widget-header" />
                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                    </asp:GridView>
                    <asp:GridView ID="GridViewCentriStazioni" runat="server" AutoGenerateColumns="False"
                        Width="900px" CellPadding="5" CssClass="ui-widget-content" Caption="Seleziona i Centri Aziendali"
                        meta:resourcekey="GridViewCentriMeteoResource1">
                        <Columns>
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                <HeaderTemplate>
                                    <input type="checkbox" id="chkSelezionaTuttiCentri" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="ChkSelezionaCentro" Checked="True" runat="server" CssClass="ChkSelezionaCentro"
                                        meta:resourcekey="ChkSelezionaCentroResource1" />
                                </ItemTemplate>
                                <HeaderStyle Width="20px" />
                                <ItemStyle Width="20px" />
                                <FooterStyle Width="20px" />
                                <ControlStyle Width="20px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" SortExpression="Rag_Soc"
                                Visible="False" meta:resourcekey="BoundFieldResource1"></asp:BoundField>
                            <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" SortExpression="Sa_Nome"
                                meta:resourcekey="BoundFieldResource2"></asp:BoundField>
                            <asp:TemplateField HeaderText="Longitudine" SortExpression="Longitudine" >
                                <ItemTemplate>
                                    <asp:TextBox ID="Txt_Longitudine" runat="server" CssClass="Txt_Longitudine" Text='0' />
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="90px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Latitudine" SortExpression="Latitudine" >
                                <ItemTemplate>
                                    <asp:TextBox ID="Txt_Latitudine" runat="server" CssClass="Txt_Latitudine" Text='0' />
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                                <HeaderStyle Width="90px" />
                            </asp:TemplateField>
                            <asp:ButtonField HeaderText="Ricerca Per Indirizzo" Text="&lt;img src='../AB_Immagini/icone32/Lente.ico' border='0'&gt;"
                                CommandName="RicercaPerIndirizzoLatLong">
                                <ControlStyle Width="60px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:ButtonField>
                            <asp:ButtonField HeaderText="Salva Coordinate" Text="&lt;img src='../AB_Immagini/icone32/Dischetto.ico' border='0'&gt;"
                                CommandName="SalvaCoordinateLatLong">
                                <ControlStyle Width="60px" />
                                <HeaderStyle Width="80px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:ButtonField>
                            <asp:TemplateField HeaderText="Stazione Meteo" SortExpression="StazioneMeteo">
                                <ItemTemplate>
                                    <asp:DropDownList ID="CMB_Stazione" runat="server" CssClass="CMB_Stazione" />
                                </ItemTemplate>
                                <ControlStyle Width="500px" />
                                <HeaderStyle Width="510px" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="ui-widget-header" />
                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
            <table id="TabellaFiltriMeteo" runat="server" style="width: 90%; padding: 0px; text-align: left;
                border: 1px solid #A6C9E2; margin-top: 20px;" aria-hidden="true">
                <tr runat="server">
                    <td style="width: 120px" class="sfondoverde" runat="server">
                        <b>
                            <asp:Label ID="LabelTabellaFiltriMeteo" runat="server" Text="Filtra dati Meteo" /></b>:
                    </td>
                    <td style="width: 5%" runat="server">
                    </td>
                    <td style="width: 25%" runat="server">
                    </td>
                    <td style="width: 5%" runat="server">
                    </td>
                    <td style="width: 20%" runat="server">
                    </td>
                    <td style="width: 25%" runat="server">
                    </td>
                    <td runat="server">
                    </td>
                </tr>
                <tr runat="server">
                    <td runat="server">
                    </td>
                    <td style="text-align: right" runat="server">
                        <b>
                            <asp:Label ID="lblDataDa" runat="server">Data Da:</asp:Label></b>
                    </td>
                    <td runat="server">
                        <asp:TextBox ID="Text_BoxDataDa" runat="server" CssClass="txtUI datepicker">gg/mm/aaa</asp:TextBox>
                    </td>
                    <td style="width: 50px" runat="server">
                    </td>
                    <td style="text-align: right" runat="server">
                        <b>
                            <asp:Label ID="lblRaggruppamentoprecipitazioni" runat="server">Raggruppamento precipitazioni:</asp:Label></b>
                    </td>
                    <td runat="server">
                        <asp:DropDownList ID="DropDownOre" runat="server" CssClass="txtUI" AutoPostBack="True"
                            Visible="False">
                        </asp:DropDownList>
                        <b>Giornaliero</b>
                    </td>
                    <td runat="server">
                    </td>
                </tr>
                <tr runat="server">
                    <td runat="server">
                    </td>
                    <td style="text-align: right" runat="server">
                        <b>
                            <asp:Label ID="lblDataA" runat="server">Data A:</asp:Label></b>
                    </td>
                    <td runat="server">
                        <asp:TextBox ID="Text_BoxDataA" runat="server" CssClass="txtUI datepicker">gg/mm/aaa</asp:TextBox>
                    </td>
                    <td runat="server">
                    </td>
                    <td style="text-align: right" runat="server">
                        <b>
                            <asp:Label ID="lblSogliaMinimaPrecipitazioniSignificative" runat="server">Soglia Minima Precipitazioni Significative (>=):</asp:Label></b>
                    </td>
                    <td runat="server">
                        <asp:TextBox ID="TextBoxSoglia" runat="server" CssClass="txtUI" Text="0,1"></asp:TextBox><b><asp:Label
                            ID="lblmm" runat="server">mm</asp:Label></b>
                    </td>
                    <td runat="server">
                    </td>
                </tr>
                <tr runat="server">
                    <td runat="server">
                    </td>
                    <td runat="server">
                    </td>
                    <td runat="server">
                    </td>
                    <td runat="server">
                    </td>
                    <td runat="server">
                    </td>
                    <td runat="server">
                    </td>
                    <td runat="server">
                    </td>
                </tr>
            </table>
            <table style="width: 90%; margin-top: 20px;" aria-hidden="true">
                <tr>
                    <td align="center">
                        <b>
                            <asp:Label ID="LabelCaricaPioggiaDaMeteo" runat="server" Text=" Carica i mm di pioggia dal Servizio Web dei dati Meteo "
                                meta:resourcekey="LabelCaricaPioggiaDaMeteoResource1"></asp:Label></b> <b>
                                    <asp:Label ID="LabelCaricaPioggiaManuale" runat="server" Text=" Inserisci i mm di pioggia rilevati"
                                        meta:resourcekey="LabelCaricaPioggiaManualeResource1"></asp:Label></b>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="left">
                        <div style="font-size: 10px; width: 100%" class="sfondoverde" id="sfondoverde" runat="server"
                            align="center">
                            <div style="width: 100px; height: 40px;">
                                <div class="clear">
                                    <br />
                                    <asp:ImageButton ID="ImageButton_MostraRilievi" runat="server" Height="23px" ImageUrl="~/AB_Immagini/Icone32/FrecciaDN.ico"
                                        Width="32px" meta:resourcekey="ImageButton_MostraRilieviResource1" />
                                    <asp:ImageButton ID="ImageButton_Sblocca" runat="server" Height="23px" ImageUrl="~/AB_Immagini/Icone32/FrecciaUP.ico"
                                        Width="32px" meta:resourcekey="ImageButton_SbloccaResource1" />
                                    <br />
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="updatepanelGridRilievi" runat="server" style="width: 100%; overflow: auto">
        <ContentTemplate>
            <table width="100%" id="NoteTabella" runat="server" aria-hidden="true">
                <tr runat="server">
                    <td colspan="3" align="left" style="width: 50%;" runat="server">
                        <div style="font-size: 12px; width: 100%;">
                            <asp:Image ID="ImageInfo1" runat="server" ImageUrl="~/AB_Immagini/Icone16/Esclamativo16.ico"
                                Style="width: 14px" />
                            <asp:Label ID="LabelInfo1" runat="server" Text="Label"></asp:Label>
                            <br />
                            <asp:Image ID="ImageInfo2" runat="server" ImageUrl="~/AB_Immagini/Icone16/Esclamativo16.ico"
                                Style="width: 14px" />
                            <asp:Label ID="LabelInfo2" runat="server" Text="Label"></asp:Label>
                        </div>
                        <br />
                    </td>
                    <td colspan="3" align="left" runat="server">
                    </td>
                </tr>
            </table>
            <asp:GridView ID="GridView_Piogge" runat="server" AutoGenerateColumns="False" Width="90%"
                CellPadding="5" CssClass="ui-widget-content" Caption="Rilievo Piogge Centri Aziendali "
                meta:resourcekey="GridView_PioggeResource1">
                <Columns>
                    <asp:TemplateField meta:resourcekey="TemplateFieldResource8">
                        <HeaderTemplate>
                            <input type="checkbox" id="chkSelezionaTuttiRilievi" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="ChkSelezionaRilievo" Checked="True" runat="server" CssClass="ChkSelezionaRilievo"
                                meta:resourcekey="ChkSelezionaRilievoResource1" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" />
                        <ItemStyle Width="20px" />
                        <FooterStyle Width="20px" />
                        <ControlStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" SortExpression="Rag_Soc"
                        Visible="False" meta:resourcekey="BoundFieldResource5"></asp:BoundField>
                    <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" SortExpression="Sa_Nome"
                        meta:resourcekey="BoundFieldResource6"></asp:BoundField>
                    <asp:TemplateField HeaderText="Data Rilievo" SortExpression="Data" meta:resourcekey="TemplateFieldResource9">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_Data" runat="server" CssClass="Txt_Data" meta:resourcekey="Txt_DataResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="80px" />
                        <HeaderStyle Width="90px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ora Rilievo" SortExpression="Ora" Visible="False"
                        meta:resourcekey="TemplateFieldResource10">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_Ora" runat="server" CssClass="Txt_Ora" Text='23' meta:resourcekey="Txt_OraResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="60px" />
                        <HeaderStyle Width="70px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Minuti Rilievo" SortExpression="Minuti" Visible="False"
                        meta:resourcekey="TemplateFieldResource11">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_Minuti" runat="server" CssClass="Txt_Minuti" Text='0' meta:resourcekey="Txt_MinutiResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="60px" />
                        <HeaderStyle Width="70px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Pioggia [mm]" SortExpression="mm_pioggia" meta:resourcekey="TemplateFieldResource12">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_mm_pioggia" runat="server" CssClass="Txt_mm_pioggia" ToolTip="Millimetri di pioggia rilevati"
                                Text='0' meta:resourcekey="Txt_mm_pioggiaResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="60px" />
                        <HeaderStyle Width="70px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="TotalePeriodoCentro" HeaderText="Cumulo per Centro [mm]" SortExpression="TotalePeriodoCentro">
                        <ControlStyle Width="60px" />
                        <HeaderStyle Width="70px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Note" SortExpression="Note" meta:resourcekey="TemplateFieldResource13">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_Note" runat="server" CssClass="Txt_Note" meta:resourcekey="Txt_NoteResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="280px" />
                        <HeaderStyle Width="290px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="T. Minima [°C]" SortExpression="temp_minima" meta:resourcekey="TemplateFieldResource14">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_temp_minima" runat="server" CssClass="Txt_temp_minima" Text='0'
                                meta:resourcekey="Txt_temp_minimaResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="65px" />
                        <HeaderStyle Width="75px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="T. Massima [°C]" SortExpression="temp_massima" meta:resourcekey="TemplateFieldResource15">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_temp_massima" runat="server" CssClass="Txt_temp_massima" Text='0'
                                meta:resourcekey="Txt_temp_massimaResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="65px" />
                        <HeaderStyle Width="75px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Umidità [%]" SortExpression="umidita" meta:resourcekey="TemplateFieldResource16">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_umidita" runat="server" CssClass="Txt_umidita" Text='0' meta:resourcekey="Txt_umiditaResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="65px" />
                        <HeaderStyle Width="75px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Irraggiamento [W/m²]" SortExpression="irraggiamento" meta:resourcekey="TemplateFieldResource17">
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_irraggiamento" runat="server" CssClass="Txt_irraggiamento" Text='0' meta:resourcekey="Txt_irraggiamentoResource1" />
                        </ItemTemplate>
                        <ControlStyle Width="115px" />
                        <HeaderStyle Width="75px" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="ui-widget-header" />
                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
            </asp:GridView>
            <br />
            <table id="tab1" style="width: 100%; margin-top: 20px; margin-bottom: 20px;" aria-hidden="true">
                <tr>
                    <td align="right" style="width: 25%;">
                        <b>
                            <asp:Label ID="lblDataA2" runat="server">Totale Rielievi Selezionati:</asp:Label>
                        </b>
                    </td>
                    <td align="left" style="width: 25%;">
                        <b>
                            <asp:Label ID="lblDataA3" class="totaleselezione" runat="server">0</asp:Label>
                        </b>
                    </td>
                    <td align="left" style="width: 25%;">
                    </td>
                    <td align="left" style="width: 25%;">
                    </td>
                </tr>
            </table>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="updatepanelRilieviSalvati" runat="server" style="width: 100%;
        overflow: auto">
        <ContentTemplate>
            <table id="TableRilieviSalvati" runat="server" style="width: 100%; margin-top: 20px;
                margin-bottom: 20px;" aria-hidden="true">
                <tr runat="server">
                    <td colspan="3" align="left" runat="server">
                        <div style="font-size: 10px; width: 90%" class="sfondoverde" id="Div1" runat="server"
                            align="center">
                            <div style="width: 100px; height: 40px;">
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
            <asp:GridView ID="GridViewRilieviSalvati" runat="server" AutoGenerateColumns="False"
                Width="90%" CellPadding="5" CssClass="ui-widget-content" Caption="Rilievi Nel Mese Corrente e In Quelli Attigui "
                meta:resourcekey="GridViewRilieviSalvatiResource1">
                <Columns>
                    <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" SortExpression="Rag_Soc"
                        Visible="False" meta:resourcekey="BoundFieldResource7"></asp:BoundField>
                    <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" SortExpression="Sa_Nome"
                        meta:resourcekey="BoundFieldResource8"></asp:BoundField>
                    <asp:BoundField DataField="Data" HeaderText="Data Rilievo" SortExpression="Data"
                        meta:resourcekey="BoundFieldResource9">
                        <ControlStyle Width="80px" />
                        <HeaderStyle Width="90px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Ora" HeaderText="Ora Rilievo" SortExpression="Ora" Visible="False"
                        meta:resourcekey="BoundFieldResource10">
                        <ControlStyle Width="60px" />
                        <HeaderStyle Width="70px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="mm_pioggia" HeaderText="Pioggia [mm]" SortExpression="mm_pioggia"
                        meta:resourcekey="BoundFieldResource11">
                        <ControlStyle Width="60px" />
                        <HeaderStyle Width="70px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TotalePeriodoCentro" HeaderText="Totale Centro [mm]" SortExpression="TotalePeriodoCentro">
                        <ControlStyle Width="60px" />
                        <HeaderStyle Width="70px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Note" HeaderText="Note" SortExpression="Note" meta:resourcekey="BoundFieldResource12">
                        <ControlStyle Width="280px" />
                        <HeaderStyle Width="290px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="temp_minima" HeaderText="T. Minima [°C]" SortExpression="temp_minima"
                        meta:resourcekey="BoundFieldResource13">
                        <ControlStyle Width="65px" />
                        <HeaderStyle Width="75px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="temp_massima" HeaderText="T. Massima [°C]" SortExpression="temp_massima"
                        meta:resourcekey="BoundFieldResource14">
                        <ControlStyle Width="65px" />
                        <HeaderStyle Width="75px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="umidita" HeaderText="Umidità [%]" SortExpression="umidita"
                        meta:resourcekey="BoundFieldResource15">
                        <ControlStyle Width="65px" />
                        <HeaderStyle Width="75px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="irraggiamento" HeaderText="Irraggiamento [W/m²]" SortExpression="irraggiamento"
                        meta:resourcekey="BoundFieldResource16">
                        <ControlStyle Width="65px" />
                        <HeaderStyle Width="75px" />
                    </asp:BoundField>
                </Columns>
                <HeaderStyle CssClass="ui-widget-header" />
                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
