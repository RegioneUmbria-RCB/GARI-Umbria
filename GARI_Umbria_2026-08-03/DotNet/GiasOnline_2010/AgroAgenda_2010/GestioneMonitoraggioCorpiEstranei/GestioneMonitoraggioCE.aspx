<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master" CodeBehind="GestioneMonitoraggioCE.aspx.vb" Inherits="AgroAgenda_2010.GestioneMonitoraggioCE" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="cHead" ContentPlaceHolderID="ContentAgendaHead" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#ID_Cancella').on('click', function () {
                if (confirm("Sei sicuro di voler eliminare la riga selezionata?") == true) {
                    $('#<%=btn_delete_riga.ClientID %>').click();
                }
            });
        });

        function SelezionaDeselezionaTutti() {
            if ($('#chkSelezionaTutteOperazioni').is(':checked')) {
                //seleziono tutto
                alert('a');
                $('.ChkSelezionaOperazione').each(function () {
                    alert('b');
                    $(this).children('input').attr('checked', 'checked');
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaOperazione').each(function () {
                    $(this).children('input').removeAttr('checked');
                });
            }
        }
    </script>
</asp:Content>
<asp:Content ID="cBody"  ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <table class="boxColore" style="width:100%; " aria-hidden="true">
        <tr>
            <td colspan="2" style=" padding-bottom:20px; padding-top:10px;">
                <div>
                    <asp:ImageButton ImageUrl="~/AB_Immagini/Icone32/Lente.ico" ID="ID_Trova" runat="server" ToolTip="Cerca i monitoraggi in base al filtro impostato" CssClass="btn_per_load" /> 
                    <asp:ImageButton ImageUrl="~/AB_Immagini/icone32/Gomma32.ico" ID="ID_Azzera" runat="server" ToolTip="Azzera il filtro impostato" CssClass="btn_per_load" />   
                    <asp:ImageButton ImageUrl="~/AB_Immagini/Icone32/Informazioni.png" id="ID_Informazioni" runat="server" ToolTip="Visualizza il modulo di carico selezionato" CssClass="btn_per_load" />               
                    <asp:ImageButton ImageUrl="~/AB_Immagini/icone32/Nuovo.ico" id="ID_Nuovo" ToolTip="Inserisci un nuovo modulo di carico"  runat="server" CssClass="btn_per_load" />
                    <asp:ImageButton ImageUrl="~/AB_Immagini/icone32/Modifica.ico" id="ID_Modifica" ToolTip="Modifica il modulo di carico selezionato" runat="server"  CssClass="btn_per_load" />
                    <img id ="ID_Cancella" src="../AB_Immagini/Icone32/Cestino.ico" alt="Elimina il modulo di carico selezionato" class="elimina_riga btn_per_load"  />                  
                    <asp:ImageButton ImageUrl="~/AB_Immagini/Icone32/Stampa.ico" ID="ID_Stampa" runat="server" ToolTip="Esportazione excel dei moduli di carico" CssClass="btn_per_load"  />
                    <asp:ImageButton ImageUrl="~/AB_Immagini/Icone32/Stampa.ico" ID="ID_StampaAggregata" runat="server" ToolTip="Esportazione excel dei moduli di carico (aggregata)" CssClass="btn_per_load" />  
                    <asp:ImageButton ImageUrl="~/AB_Immagini/icone32/Insetto32_03.ico" ID="ID_Insetto" runat="server" ToolTip="Aggiunta/Modifica dei Corpi Estranei" CssClass="btn_per_load" />                
                </div>
                <div style="display: none;">
                    <asp:Button ID="btn_delete_riga" runat="server" Text="delete_riga" />
                    <asp:HiddenField ID="hidden_chiaveRiga" runat="server" />
                </div>

            </td>
        </tr>
        <tr >
            <td style="max-width:350px">            
                <div style="float: left;" >
                    <div style="float: left;" >
                        <asp:Label ID="lbl_data" runat="server" Width="150px" >Data di arrivo</asp:Label>    
                        <asp:Label ID="lbl_dataInizio" runat="server" Width="39px" >Dal: </asp:Label>                        
                    </div>
                    <div style="float: left;" >
                        <asp:TextBox ID="Txt_DataInizio" runat="server" class="txtUI" Width="75px"></asp:TextBox>
                        <script type="text/javascript">
                             $(function () {
                                 $("#<%= Txt_DataInizio.ClientID %>").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
                             });                       
                        </script>
                    </div>
                    <div style="float: left;" >
                        <asp:Label ID="lbl_dataFine" runat="server" Width="39px" style=" margin-left:10px" >Al: </asp:Label>                       
                    </div>
                    <div style="float: left;" >
                        <asp:TextBox ID="Txt_DataFine" runat="server" CssClass="txtUI" Width="75px"></asp:TextBox>
                        <script type="text/javascript">
                              $(function () {
                                  $("#<%= Txt_DataFine.ClientID %>").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
                              });                       
                        </script>
                    </div>
                           
                </div>

            </td>
            <td style="float: left; " >
                <div style="float: left;">
                    <asp:Label ID="Label2" runat="server" Width="130px"  >Specie: </asp:Label> 
                </div>    
                <div >               
                    <asp:dropdownlist id="Cmb_Specie" runat="server" Height="23px" Width="272px" CssClass="myCombo change_per_load" AutoPostBack="true"></asp:dropdownlist>
                </div>
            </td>
        </tr>
        <tr style="margin-top:10px;">   
            <td>                    
                <div style="float: left;" >
                    <asp:Label ID="Label1" runat="server" Width="130px"  >Stabilimento: </asp:Label>
                </div>
                <div>
                    <asp:dropdownlist id="Cmb_Magazzino" runat="server" Height="23px" Width="272px" CssClass="myCombo"></asp:dropdownlist>
                </div>
            </td>
            <td>
                <div style="float: left;">
                    <asp:Label ID="Label4" runat="server" Width="130px"  >Regolamento: </asp:Label> 
                </div> 
                <div>
                    <asp:RadioButtonList ID="Rbl_Biologico" runat="server" AutoPostBack="True" 
                        RepeatDirection="Horizontal" class="txtUI" Width="400px" BorderStyle="None" >
                        <asp:ListItem Selected="True" Value="-1">Nessun filtro</asp:ListItem>
                        <asp:ListItem Value="4">Biologico</asp:ListItem>
                        <asp:ListItem Value="0">Non Biologico</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </td>
        </tr>

        <tr>        
            <td style="margin-top:10px;">           
                <div style="float: left;" >
                    <asp:Label ID="Label3" runat="server" Width="130px"  >Ragione Sociale: </asp:Label>
                </div>
                <div style="float: left;">
                    <asp:TextBox ID="Txt_FiltroRagSoc_Produttore" runat="server"  Width="239px" 
                        BackColor="#FFFFFF" CssClass="txtUI"></asp:TextBox>
                </div>
                <div style=" margin-left: 10px;">
                    <asp:ImageButton ID="Btn_Carica_Produttore" runat="server" ImageUrl="../AB_Immagini/icone32/lente.ico"
                        CssClass="floatSX btn_per_load" ToolTip="Premere il pulsante per caricare i formulati nella lista"
                        Width="24px" Height="24px" margin-left="10px" />
                    <asp:Label ID="Txt_NumProdotti" runat="server" >&nbsp;</asp:Label>
                </div>
                
            </td>
            <td>
                <div style="float: left;" >
                    <asp:Label ID="Label5" runat="server" Width="130px"  >Prodotto: </asp:Label>
                </div>
                <div>
                    <asp:dropdownlist id="Cmb_Prodotto" runat="server" Height="23px" Width="272px" CssClass="myCombo"></asp:dropdownlist>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div style="float: left;" >
                    <asp:Label ID="Label7" runat="server" Width="130px"  ></asp:Label>
                </div>
                <div>
                    <asp:dropdownlist id="Cmb_Produttore" runat="server" Height="23px" Width="272px" CssClass="myCombo change_per_load" AutoPostBack="true"></asp:dropdownlist>
                </div>
            </td>
            <td>
                <div style="float: left;" >
                    <asp:Label ID="Label6" runat="server" Width="130px"  >Appezzamento: </asp:Label>
                </div>
                <div>
                    <asp:dropdownlist id="Cmb_Appezzamento" runat="server" Height="23px" Width="272px" CssClass="myCombo"></asp:dropdownlist>
                </div>
            </td>
        </tr>
        <tr>   
            <td>
                <div style="float: left;" >
                    <asp:Label ID="Label8" runat="server" Width="130px"  >Ritrovamento: </asp:Label>
                </div>
                <div>
                    <asp:dropdownlist id="Cmb_TipologiaRitrovamento" runat="server" Height="23px" Width="272px" CssClass="myCombo"></asp:dropdownlist>
                </div>    
            </td>
            <td>
                <div style="float: left;">
                    <asp:Label ID="Label9" runat="server" Width="130px"  >Pericolosita': </asp:Label> 
                </div> 
                <div>
                    <asp:RadioButtonList ID="Rbl_Pericolosita" runat="server" BorderStyle="None"
                        RepeatDirection="Horizontal" class="myCheckList" Width="400px" >
                        <asp:ListItem Value="0" Selected="True">Tutte</asp:ListItem>
						<asp:ListItem Value="1">Alta</asp:ListItem>
						<asp:ListItem Value="2">Media/Bassa</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:label id="Lbl_NumModuliCarico" runat="server" Height="14px" Width="344px" BorderStyle="None" CssClass="txtUI">Numero Moduli di Carico trovati:</asp:label>

            </td>
        </tr>

        <tr>
            <td colspan="2">
                <asp:GridView ID="DataGrid_CE" runat="server" AutoGenerateColumns="False" Width="100%"
                    CellPadding="5" CssClass="ui-widget-content" AllowSorting="True" >
                    <Columns>
                       <asp:TemplateField >
                            <HeaderTemplate>
                                <input type="checkbox" id="chkSelezionaTutteOperazioni" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="ChkSelezionaOperazione" runat="server" CssClass="ChkSelezionaOperazione"/>
                            </ItemTemplate>
                            <HeaderStyle Width="20px" />
                            <ItemStyle Width="20px" />
                            <FooterStyle Width="20px" />
                            <ControlStyle Width="20px" />
                        </asp:TemplateField>
						<asp:BoundField Visible="False" DataField="Piva" HeaderText="Piva"></asp:BoundField>
						<asp:BoundField Visible="False" DataField="Sa_Cod" HeaderText="Sa_Cod"></asp:BoundField>
						<asp:BoundField Visible="False" DataField="Id_Agenda" HeaderText="Id_Agenda"></asp:BoundField>
						<asp:BoundField Visible="False" DataField="Id_Mov" HeaderText="Id_Mov"></asp:BoundField>
						<asp:BoundField Visible="False" DataField="Extra_Int" HeaderText="Anno">
							<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						</asp:BoundField>
						<asp:BoundField DataField="Data_Bolla" HeaderText="Data Bolla"></asp:BoundField>
						<asp:BoundField DataField="Numero_Bolla" HeaderText="Numero Bolla">
							<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						</asp:BoundField>
						<asp:BoundField DataField="Data_Movimento" HeaderText="Data Arrivo">
							<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
						</asp:BoundField>
						<asp:BoundField DataField="Colli" HeaderText="Num. Carico"></asp:BoundField>
						<asp:BoundField DataField="Mat_Des" HeaderText="Specie - Variet&#224;">
							<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
						</asp:BoundField>
						<asp:BoundField DataField="Rag_Soc_Produttore" 
                            HeaderText="Ragione Sociale&lt;br/&gt;Produttore" HtmlEncode="False">
							<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
						</asp:BoundField>
						<asp:BoundField DataField="Rag_Soc_Conferente" 
                            HeaderText="Ragione Sociale&lt;br/&gt;Conferente" HtmlEncode="False"></asp:BoundField>
						<asp:BoundField DataField="QTA" HeaderText="Kg.">
							<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
						</asp:BoundField>
						<asp:BoundField DataField="Mov_Desc" HeaderText="Note"></asp:BoundField>
						<asp:BoundField DataField="Stabilimento" HeaderText="Stabilimento"></asp:BoundField>                        
                    </Columns>
                    <HeaderStyle CssClass="ui-widget-header" />
                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                </asp:GridView>
            </td>
        </tr>
    </table>
    <INPUT style="Z-INDEX: 108; LEFT: 24px; WIDTH: 41px; POSITION: absolute; TOP: 584px; HEIGHT: 6px"
				id="SI_NO" size="1" type="hidden" name="SI_NO" runat="server">
    <input id="Input_Popup" size="9" type="hidden" name="Input_Popup" runat="server" />
</asp:Content>
