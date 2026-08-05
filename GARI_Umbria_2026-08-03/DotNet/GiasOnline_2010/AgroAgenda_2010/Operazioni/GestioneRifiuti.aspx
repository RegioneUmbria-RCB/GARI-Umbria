<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master" CodeBehind="GestioneRifiuti.aspx.vb" Inherits="AgroAgenda_2010.GestioneRifiuti" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">

          

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <asp:UpdatePanel ID="update_si_no" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="UdmMovimenti_SI_NO" runat="server" Value="0" />
           </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upDati" runat="server">
        <ContentTemplate>
            <div style="overflow: auto; width: 96%; min-width: 920px; margin-right: 2%; margin-left: 2%;">
                <div>
                    <div class="clear" style="display: none">
                    </div>
                    <asp:Panel ID="Pannello_smaltitore" runat="server" Width="100%">
                        <div class="boxColore">
                            <div>
                                <asp:Label ID="lbl_smaltitore" runat="server" BackColor="#C0FFC0" CssClass="titoloBox">Soggetto che ha effettuato il Servizio di Raccolta</asp:Label>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="min-width: 950px; width: 100%;">
                                <div style="float: left; width: 100%">
                                    <div style="float: left">
                                        <asp:Label ID="lbl_Contatto" runat="server"  Width="150px">Smaltitore</asp:Label>
                                    </div>
                                    <div style="float: left; margin-left: 10px;">
                                        <asp:TextBox ID="Txt_Smaltitore" runat="server"  
                                    BackColor="#FFFFFF" CssClass="txtUI"></asp:TextBox>
                                        <asp:DropDownList ID="Cmb_Smaltitore" runat="server" CssClass="txtUI" 
                                            Visible="false">
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="clear">
                                </div>
                            </div>
                          
                           <div style="min-width: 950px; width: 100%;">
                                <div style="float: left; width: 50%">
                                    <div style="float: left">
                                <asp:Label ID="lbl_Convenzione" runat="server" Visible="True" Width="150px">Estremi Convenzione</asp:Label>
                                    </div>
                                    <div style="float: left; margin-left: 10px;">
                                                                 <asp:TextBox ID="Txt_Convenzione" runat="server"  Width="250px"
                                    BackColor="#FFFFFF" CssClass="txtUI"></asp:TextBox>
                                    </div>
                                </div>
                                <div style="float: left; width: 50%">
                                    <div style="float: left">
                                <asp:Label ID="lbl_DocConvenzione" runat="server"  Width="150px">Estremi Documento</asp:Label>
                                    </div>
                                    <div style="float: left; margin-left: 10px;">
                                    <asp:TextBox ID="Txt_DocConvenzione" runat="server"  Width="250px" BackColor="#FFFFFF"
                                    CssClass="txtUI"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                                   <div style="min-width: 950px; width: 100%; display:none" >
                                <div style="float: left; width: 100%">
                                    <div style="float: left">
                                        <asp:Label  ID="Label1" runat="server"  Width="150px">Data Smaltimento</asp:Label>


                                    </div>
                                    <div style="float: left; margin-left: 10px;">
                                          <asp:TextBox ID="Txt_Data" runat="server"  Width="250px" BackColor="#FFFFFF"
                                    CssClass="txtUI datepicker"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="clear">
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>


                </div>
                <div class="clear">
                </div>
                <div>

                   <div style="min-width: 950px; width: 100%;"  id="div_comandi" runat="server">
                                <div style="float: left; width: 100%">
                                    <div style="float: left">
                                 <asp:Button ID="Btn_Aggiungi" ToolTip="Aggiungi" runat="server"
                                    Text="AGGIUNGI RIFIUTO" Style="margin-right: 5px; color: red" CssClass="bottone btn_per_load "
                                     />
                                    </div>
                                    <div style="float: left; margin-left: 100px;">
                                                              <asp:ImageButton ID="ImgBtnSalvaTutto" runat="server" 
                            ImageUrl="../AB_Immagini/Icone32/dischetto.ico" Width="32px" /> 
                                    </div>
                                </div>
                             
                                <div class="clear">
                                </div>
                            </div>



                    
                   <div class="clear">
                            </div>
                    <asp:Panel ID="Pannello_Prodotti" runat="server"  Width="100%">
                        <div class="boxColore">
                            <asp:Label ID="Lbl_PannelloCarichi" runat="server" BackColor="#C0FFC0" Visible="False"
                                CssClass="titoloBox">Elenco Rifiuti
            Selezionati</asp:Label>
                            <div class="clear">
                            </div>

                                                            <asp:GridView ID="GridViewRifiuti" runat="server" AutoGenerateColumns="False" CellPadding="5"
                                    CssClass="ui-widget-content" 
                                    >
                                    <Columns>
                                         <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                        CommandName="Elimina" />

                                        <asp:BoundField DataField="Contatore" HeaderText="Contatore">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>  

                                        <asp:TemplateField HeaderText="Data Carico" >
                                            <ItemTemplate>
                                                <asp:TextBox ID="TxtDataCarico" runat="server" CssClass="datepicker required" />
                                            </ItemTemplate>
                                            <ControlStyle Width="100px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Data Scarico" >
                                            <ItemTemplate>
                                                <asp:TextBox ID="TxtDataScarico" runat="server" CssClass="datepicker required"  />
                                            </ItemTemplate>
                                            <ControlStyle Width="100px" />
                                        </asp:TemplateField>
                                   
                                                                 
                                        <asp:TemplateField HeaderText="(Codice CER) Rifiuto" >
                                            <ItemTemplate>
                                                <asp:DropDownList ID="CmbRifiuti" runat="server"  />
                                                <asp:TextBox ID="TxtRifiuti" runat="server" Width="100%"  />
                                            </ItemTemplate>
                                                                                   
                                        </asp:TemplateField>                                                                 
                                                                                                                  
                                        <asp:TemplateField HeaderText="Unita di Misura" SortExpression="UnitaMisura" meta:resourcekey="TemplateFieldResource13">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="CmbUdm" runat="server" CssClass="CmbUdm" meta:resourcekey="CmbUdmResource1" />
                                            </ItemTemplate>
                                            <ControlStyle Width="80px" />
                                            <HeaderStyle Width="90px" />
                                        </asp:TemplateField>
           
                                        <asp:TemplateField HeaderText="Qta" >
                                            <ItemTemplate>
                                                <asp:TextBox ID="TxtQta" runat="server"  />
                                            </ItemTemplate>
                                            <ControlStyle Width="40px" />
                                            <HeaderStyle Width="35px" />
                                        </asp:TemplateField>
                                                  
                                        <asp:BoundField DataField="Codice" HeaderText="Codice">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>
                                         <asp:BoundField DataField="DataCarico" HeaderText="DataCarico">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DataScarico" HeaderText="DataScarico">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>     
                                      
                                        <asp:BoundField DataField="Udm_Cod" HeaderText="Udm_Cod">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>     
                                        <asp:BoundField DataField="Qta" HeaderText="Qta">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>     
      
                                    </Columns>
                                    <HeaderStyle CssClass="ui-widget-header" />
                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                </asp:GridView>



                        </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>
                    <asp:Panel ID="Pannello_Cancellazione" runat="server" Visible="False">
                        <div class="boxColore">
                            <asp:Label ID="LblCancellazione" runat="server" Width="298px" CssClass="Testo_12_Rosso_Bold">Premere
            l'icona CESTINO per confermare la cancellazione</asp:Label>
                            <asp:ImageButton ID="ImgBtnCancella" runat="server" Width="32px" ImageUrl="../AB_Immagini/Icone32/Cancella.bmp">
                            </asp:ImageButton>
                        </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>
                    <div>
                        <input id="SI_NO" size="1" type="hidden" name="SI_NO" runat="server" />
                     
                        <asp:TextBox ID="Txt_Giacenza_Destinazione_Tot_old" runat="server" ReadOnly="True"
                            CssClass="txtUI" BackColor="#C0FFC0" Width="41px" Visible="False"></asp:TextBox>
                        <asp:TextBox ID="Txt_Giacenza_Provenienza_Tot_old" runat="server" ReadOnly="True"
                            CssClass="txtUI" BackColor="#C0FFC0" Width="41px" Visible="False"></asp:TextBox>
                        <input id="Modifica_Data" size="7" type="hidden" name="Modifica_Data" runat="server" />
                        <input id="IndirizzoProfitosan" size="7" type="hidden" name="IndirizzoProfitosan"
                            runat="server" />
                        <asp:TextBox ID="Txt_DaInviare" runat="server" Style="display: none"></asp:TextBox>
                    </div>
                    <div class="clear">
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
