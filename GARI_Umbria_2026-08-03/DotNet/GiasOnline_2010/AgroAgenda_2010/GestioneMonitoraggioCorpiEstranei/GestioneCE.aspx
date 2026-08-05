<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master" CodeBehind="GestioneCE.aspx.vb" Inherits="AgroAgenda_2010.GestioneCE" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    
    <base target="_self" />
    <script type="text/javascript">

      $(function () {

        $('#ID_Cancella').on('click', function () {
        
             if (confirm("Sei sicuro di voler eliminare il corpo estraneo?") == true) {

                $('#<%=btn_delete_riga.ClientID %>').click();
            }

        });
    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
<table class="boxColore" style="width:100%; " aria-hidden="true">
    <tr >
        <td style=" width:480px;">
            <div class="cento" style="overflow: auto; width: 100%; height: 100%; max-height: 400px;
                margin-top: 10px;">
             
                        <asp:GridView ID="DataGrid_CE" runat="server" AutoGenerateColumns="False"
                            CellPadding="5" Style="width: 100%; margin-top: 5px; background-image: none; "  >
                            <Columns>
                                <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ChkSelezionaCE" runat="server" class="check_drg_ce" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Desc_CorpoEstraneo" HeaderText="Descrizione ">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="LimiteMax_Aeroseparatori" HeaderText="Limite Max Aeroseparatori ">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="LimiteMax_CernitriciOttiche" HeaderText="Limite Max CernitriciOttiche ">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="LimiteMax_CernitaManuale" HeaderText="Limite Max CernitaManuale ">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundField>
                            </Columns>
                            <HeaderStyle CssClass="ui-widget-header" />
                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                        </asp:GridView>
            </div>  
        </td>
        <td style=" vertical-align:top; padding-left:15px;">
            <div>
                <asp:ImageButton ImageUrl="~/AB_Immagini/Icone32/Lente.ico" ID="ID_Trova" runat="server" ToolTip="Carica l'elenco dei monitoraggi" />
                <asp:ImageButton ImageUrl="~/AB_Immagini/icone32/Nuovo.ico" id="ID_Nuovo" ToolTip="Inserisci un nuovo modulo di carico"  runat="server" />
                <asp:ImageButton ImageUrl="~/AB_Immagini/icone32/Modifica.ico" id="ID_Modifica" ToolTip="Modifica il modulo di carico selezionato" runat="server" />
                <img id ="ID_Cancella" src="../AB_Immagini/Icone32/Cestino.ico" alt="Elimina il modulo di carico selezionato" class="elimina_riga" />
                <asp:ImageButton ImageUrl="~/AB_Immagini/Icone32/Esci03.ico" id="ID_Annulla" ToolTip="Aggiunta/Modifica dei Corpi Estranei"  runat="server" />
                <asp:ImageButton ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" id="ID_Salva" ToolTip="Aggiunta/Modifica dei Corpi Estranei"  runat="server" />
            </div>
            <div style="display: none;">
                <asp:Button ID="btn_delete_riga" runat="server" Text="delete_riga" />
                <asp:HiddenField ID="hidden_chiaveRiga" runat="server" />
            </div>
        
            <asp:Panel  runat="server" ID="Pannello_Inserimento"  Visible="false" >

                <table id="Pannello_CE_Insert" style=" vertical-align:top; " runat="server" aria-hidden="true">
                    <tr>
                        <td id="lbl_inserisci_modifica" runat="server" style="padding-left:15px;">Nome corpo estraneo</td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">
                            <ASP:TEXTBOX id="Txt_DescCE" runat="server" Width="200px" class="txtUI" ></ASP:TEXTBOX>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">Limite Max Aeroseparatori</td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">
                            <ASP:TEXTBOX id="Txt_LimiteMaxAero" runat="server" Width="200px" class="txtUI" ></ASP:TEXTBOX>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">Limite Max Cernitrici Ottiche</td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">
                            <ASP:TEXTBOX id="Txt_LimiteMaxCO" runat="server" Width="200px" class="txtUI" ></ASP:TEXTBOX>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">Limite Max Cernita Manuale</td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">
                            <asp:TextBox id="Txt_LimiteMaxCM" runat="server" Width="200px" class="txtUI" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">
                            <asp:TextBox ID="Txt_Cod_CorpoEstraneo" runat="server" Width="200" class="txtUI" ></asp:TextBox>     
                            <input style="Z-INDEX: 107; POSITION: absolute; TOP: 16px; LEFT: 744px" id="SI_NO" size="1"
                                type="hidden" name="SI_NO" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>

</asp:Content>
