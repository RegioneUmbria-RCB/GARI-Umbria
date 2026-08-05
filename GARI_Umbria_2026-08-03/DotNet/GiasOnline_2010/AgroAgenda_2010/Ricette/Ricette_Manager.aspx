<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master"
    CodeBehind="Ricette_Manager.aspx.vb" Inherits="AgroAgenda_2010.Ricette_Manager" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=txt_ValiditaInizio.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });
            $('#<%=txt_ValiditaFine.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });
            $.datepicker.regional['it'];

            $('#<%=Cmb_Specie.ClientID %>').combobox();
        });

        function stampa(ricettaCod, tipo) {
            var str = "Stampa/Ricetta_Stampa.aspx" + "?ricetta_cod=" + ricettaCod
                    + "\&ricetta_stampa_tipo=" + tipo +
                    "','Ricetta','height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes'";
            window.open(str);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <table style="width: 100%" aria-hidden="true">
        <tr valign="top">
            <td style="width: 100%">
                <table style="border-color: #0000C0; border-style: solid; border-width: 2px; padding: 5px; width: 100%" aria-hidden="true">
                    <tr>
                        <td colspan="4" style="text-decoration: underline; font-size: medium">
                            <b>Filtri di Ricerca</b>:
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:CheckBox ID="Chk_VisualizzaPubbliche" Text="Visualizza Ricette Pubbliche" Font-Bold="true"
                                runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Descrizione</b>:
                        </td>
                        <td>
                            <asp:TextBox ID="Txt_Filtro_Ricette" runat="server" CssClass="txtUI" ToolTip="Testo Ricerca"
                                Width="140px"></asp:TextBox>
                        </td>
                        <td>
                            <b>Intervallo Validità</b>:
                        </td>
                        <td>
                            <b>Da :</b><asp:TextBox ID="txt_ValiditaInizio" runat="server" CssClass="txtUI datepicker"
                                MaxLength="10" ToolTip="Data inizio" Width="77px"></asp:TextBox>
                            <b>A: </b>
                            <asp:TextBox ID="txt_ValiditaFine" runat="server" CssClass="txtUI datepicker" MaxLength="10"
                                ToolTip="Data massima" Width="77px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Specie Vegetale</b>:
                        </td>
                        <td colspan="2">
                            <asp:DropDownList ID="Cmb_Specie" runat="server" CssClass="txtUI">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Button ID="Btn_Cerca_Ricette" runat="server" Text="CERCA" Style="margin-left: 5px;
                                margin-right: 5px; color: red" CssClass="bottone" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="Btn_NuovaRicetta" runat="server" Text="NUOVA LINEA TECNICA" Style="margin-left: 5px;
                    margin-right: 5px; color: red" CssClass="bottone" />
                <asp:Button ID="Btn_NuovaRicettaImpianti" runat="server" Text="NUOVA RICETTA AZIENDALE"
                    Style="margin-left: 5px; margin-right: 5px; color: red" CssClass="bottone" />
                <asp:Button ID="Btn_PianificaAttivita" runat="server" Text="PIANIFICA ATTIVITA'"
                    Style="margin-left: 5px; margin-right: 5px; color: red" CssClass="bottone" />
            </td>
        </tr>
        <tr>
            <td valign="top" align="left">
                <asp:GridView ID="GridView_Ricette" runat="server" AutoGenerateColumns="False" CellPadding="5"
                    CellSpacing="0" CssClass="ui-widget-content" AllowPaging="false">
                    <Columns>
                        <asp:BoundField Visible="true" DataField="Ricetta_Cod" HeaderText="Codice" HtmlEncode="false">
                            <ItemStyle CssClass="displaynone" />
                            <HeaderStyle CssClass="displaynone" />
                        </asp:BoundField>
                        <asp:BoundField Visible="true" DataField="Ricetta_Numero" HeaderText="Codice" HtmlEncode="false">
                        </asp:BoundField>
                        <asp:BoundField Visible="true" DataField="Ricetta_Des" HeaderText="Ricetta" HtmlEncode="false">
                        </asp:BoundField>
                        <asp:BoundField Visible="true" DataField="Validita_Inizio" HeaderText="Data Inizio"
                            HtmlEncode="false" SortExpression="Data"></asp:BoundField>
                        <asp:BoundField Visible="true" DataField="Validita_Fine" HeaderText="Data Fine" HtmlEncode="false"
                            SortExpression="Data"></asp:BoundField>
                        <asp:BoundField Visible="true" DataField="Data_Ultima_Operazione" HeaderText="Data Ultima Operazione"
                            HtmlEncode="false" SortExpression="Data"></asp:BoundField>
                        <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ci.ico' border='0'&gt; "
                            HeaderText="Info" CommandName="Info" />
                        <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ce.ico' border='0'&gt; "
                            HeaderText="Mod." CommandName="Modifica" />
                        <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                            HeaderText="Canc." CommandName="Cancella" />
                        <%--<asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/Stampa16b.ico' border='0'&gt;"
                            HeaderText="Stampa Cert." CommandName="Stampa" />
                        <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/Stampa16b.ico' border='0'&gt;"
                            HeaderText="Stampa Az." CommandName="Stampa_RicAz" />--%>
                        <asp:BoundField DataField="stampa_cert" HeaderText="Stampa Cert."  HtmlEncode="false" />
                        <asp:BoundField DataField="stampa_ricaz" HeaderText="Stampa Az."  HtmlEncode="false"/>
                        <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/Copia16.ico' border='0'&gt;"
                            HeaderText="Copia" CommandName="Copia" />
                        <asp:BoundField Visible="true" DataField="agenda" HeaderText="Crea Operazioni" HtmlEncode="false">
                        </asp:BoundField>
                        <%--<asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/OperazioneAgenda16.ico' border='0'&gt;"
                            HeaderText="Crea Operazioni" CommandName="CreaOperazioni" />--%>
                        <asp:BoundField Visible="true" DataField="strId_Agenda" HeaderText="strId_Agenda"
                            HtmlEncode="false">
                            <ItemStyle CssClass="displaynone" />
                            <HeaderStyle CssClass="displaynone" />
                        </asp:BoundField>
                        <asp:BoundField Visible="true" DataField="tipo_ricetta" HeaderText="tipo_ricetta"
                            HtmlEncode="false">
                            <ItemStyle CssClass="displaynone" />
                            <HeaderStyle CssClass="displaynone" />
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle CssClass="ui-widget-header" />
                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
