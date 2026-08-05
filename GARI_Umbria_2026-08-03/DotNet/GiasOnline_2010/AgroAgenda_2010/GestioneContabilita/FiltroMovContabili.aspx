<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master"
    CodeBehind="FiltroMovContabili.aspx.vb" Inherits="AgroAgenda_2010.FiltroMovContabili" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <asp:UpdatePanel ID="upContent" runat="server">
        <ContentTemplate>
            <div id="contenitore" class="padDxSx3px">
                <input id="Txt_DaInviare" size="1" name="Txt_DaInviare" runat="server" style="display: none" />
                <!--<input type="button" id="testMe" value="aggiorna" onclick="aspnetForm.submit();" />-->
                <div id="toolbarOperazioni" class="boxColore padDxSx3px Box100">
                    <asp:ImageButton runat="server" ID="ID_Trova" ImageUrl="../AB_Immagini/icone32/Trova2.ico"
                        ToolTip="Cerca i movimenti in base al filtro impostato"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Azzera" ImageUrl="../AB_Immagini/icone32/Gomma32.ico"
                        ToolTip="Azzera il filtro impostato"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Seleziona" ImageUrl="../AB_Immagini/icone32/ValidazioneSI.ico"
                        ToolTip="Seleziona tutti i movimenti"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Deseleziona" ImageUrl="../AB_Immagini/icone32/ValidazioneNO.ico"
                        ToolTip="Deseleziona tutti i movimenti"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Info" CssClass="Nascosto" ImageUrl="../AB_Immagini/icone32/Informazioni.bmp"
                        ToolTip="Visualizza il movimento selezionato"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Riferimenti" CssClass="Nascosto" ImageUrl="../AB_Immagini/icone32/Storico02.ico"
                        ToolTip="Visualizza le operazioni di agenda e/o i movimenti associati al movimento selezionato">
                    </asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Stampa" ImageUrl="../AB_Immagini/icone32/stampa.ico"
                        ToolTip="Stampa il movimento" CssClass="Nascosto"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Allega" ImageUrl="../AB_Immagini/icone32/Clip32.ico"
                        ToolTip="Aggancia i documenti selezionati"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Blocca" CssClass="Nascosto" ImageUrl="../AB_Immagini/icone32/ControlliBlocca.ico"
                        ToolTip="Blocco Movimenti"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Sblocca" CssClass="Nascosto" ImageUrl="../AB_Immagini/icone32/ControlliSblocca.ico"
                        ToolTip="Sblocco Movimenti"></asp:ImageButton>
                    <asp:ImageButton runat="server" ID="ID_Impresa" ImageUrl="../AB_Immagini/icone32/TrovaImprese.ico"
                        ToolTip="Seleziona un'altra impresa" CssClass="Nascosto"></asp:ImageButton>
                </div>
                <br />

                <asp:Label ID="Label7" runat="server" Visible="False"></asp:Label>
                <asp:Panel ID="Pannello_Generale" runat="server" CssClass="Box100">
                    <asp:Panel ID="Pannello_Filtri" runat="server" CssClass="boxColore padDxSx3px">
                        <div id="RicercaSX" class="boxColore padDxSx3px" style="width: 20%; float: left; margin-right:1%">
                            <asp:Label ID="Label1" runat="server" CssClass="titoloBox Etichetta75pxleft">Periodo di Competenza:</asp:Label>
                            <div class="clear">
                            </div>
                            <asp:Label ID="LABEL3" CssClass="Etichetta75pxleft" runat="server">Dal:</asp:Label>
                            <asp:TextBox ID="Txt_DataInizio" runat="server" CssClass="txtUI_120 datepicker floatSX"></asp:TextBox>
                            <div class="clear">
                            </div>
                            <asp:Label ID="LABEL4" CssClass="Etichetta75pxleft" runat="server">Al:</asp:Label>
                            <asp:TextBox ID="Txt_DataFine" runat="server" Style="float: left" CssClass="txtUI_120 datepicker floatSX"></asp:TextBox>
                            <div class="clear">
                            </div>
                            <asp:Label ID="LABEL2" runat="server" CssClass="Etichetta75pxleft">Scadenza:</asp:Label>
                            <asp:TextBox ID="Txt_Scadenza" runat="server" Style="float: left" CssClass="txtUI_120 datepicker floatSX"></asp:TextBox>
                            <div class="clear">
                            </div>
                            <asp:Label ID="Label22" runat="server" CssClass="Etichetta75pxleft">Visualizza Movimenti:</asp:Label>
                            <div class="clear">
                            </div>
                            <asp:RadioButtonList ID="Rbl_BloccoSblocco" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="-999" Selected="True">Tutti</asp:ListItem>
                                <asp:ListItem Value="1">Bloccati</asp:ListItem>
                                <asp:ListItem Value="0">Sbloccati</asp:ListItem>
                            </asp:RadioButtonList>
                            <div class="clear">
                            </div>
                        </div>

                        <div id="RicercaCentro" style="width: 38%; float: left; margin-right:1%" class="boxColore">
                            <div>
                                <asp:Label ID="Label9" runat="server" CssClass="titoloBox Etichetta75pxleft">Filtri movimenti:</asp:Label>
                                <div class="clear">
                                </div>
                                <asp:Label ID="Label14" runat="server" CssClass="Etichetta75pxleft">Tipo Movimento:</asp:Label>
                                <asp:DropDownList ID="Cmb_TipoMovimento" runat="server" CssClass="floatSX" AutoPostBack="True">
                                </asp:DropDownList>
                                <asp:Label ID="Label5" runat="server" CssClass="Etichetta75pxleft">Causale:</asp:Label>
                                <asp:DropDownList ID="Cmb_Causale" runat="server" CssClass="floatSX">
                                </asp:DropDownList>
                                <asp:Label ID="Label12" runat="server" CssClass="Etichetta75pxleft">Rapporto Contabile:</asp:Label>
                                <asp:DropDownList ID="Cmb_RappContabili" runat="server" CssClass="floatSX" AutoPostBack="True">
                                </asp:DropDownList>
                                <asp:Label ID="Label8" runat="server" CssClass="Etichetta75pxleft">Contatto:</asp:Label>
                                <asp:DropDownList ID="Cmb_Contatti" runat="server" CssClass="floatSX">
                                </asp:DropDownList>
                                <div class="clear">
                                </div>
                            </div>
                            <div id="numerazione">
                                <div style="width: 25%; float: left; margin-top: 13px;">
                                    <asp:Label ID="Label24" runat="server">Numero Documento:</asp:Label>
                                </div>
                                <div style="width: 25%; float: left;">
                                    <div>
                                        <asp:Label ID="Label27" runat="server">prefisso</asp:Label></div>
                                    <div>
                                        <asp:TextBox ID="Txt_DocNumero_Sin" runat="server" CssClass="txtUI txt_75px"></asp:TextBox></div>
                                </div>
                                <div style="width: 25%; float: left;">
                                    <div>
                                        <asp:Label ID="Label29" runat="server">numero</asp:Label></div>
                                    <div>
                                        <asp:TextBox ID="Txt_DocNumero" runat="server" CssClass="txtUI txt_75px"></asp:TextBox></div>
                                </div>
                                <div style="width: 25%; float: left;">
                                    <div>
                                        <asp:Label ID="Label30" runat="server">suffisso</asp:Label></div>
                                    <div>
                                        <asp:TextBox ID="Txt_DocNumero_Des" runat="server" CssClass="txtUI txt_75px"></asp:TextBox></div>
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                            <div id="opzioniNumerazione">
                                <asp:Label ID="Label21" runat="server" CssClass="Etichetta75pxleft">Tipo Ordinamento:</asp:Label>
                                <asp:DropDownList ID="Cmb_Ordinamento" runat="server" CssClass="floatSX">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div style="width: 38%; float: left; margin:0px;" class="padDxSx3px boxColore">
                            <asp:Panel ID="Pannello_Conti" runat="server">
                                <asp:Label ID="Label18" runat="server" CssClass="titoloBox Etichetta75pxleft">Filtri contab.:</asp:Label>
                                <div class="clear">
                                </div>
                                <div>
                                    <div class="floatSX50">
                                        <asp:Label ID="Label6" CssClass="Etichetta75pxleft" runat="server">Anno Contabile:</asp:Label>
                                        <asp:DropDownList ID="Cmb_AnnoContabile" runat="server" AutoPostBack="True">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="floatDX50">
                                        <asp:ImageButton ID="ImgBtn_Conti" runat="server" ImageUrl="../AB_Immagini/Icone32/Albero.ico">
                                        </asp:ImageButton>
                                        <asp:CheckBox ID="Chk_Conti" runat="server" Enabled="False" Text="Filtro avanzato">
                                        </asp:CheckBox>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                                <div>
                                    <asp:Label ID="Label17" runat="server" CssClass="Etichetta75pxleft">Riclassificazione:</asp:Label>
                                    <asp:DropDownList ID="Cmb_Riclassificazione" CssClass="floatSX" runat="server" AutoPostBack="True">
                                    </asp:DropDownList>
                                    <asp:Label ID="Label13" runat="server" CssClass="Etichetta75pxleft">Conto:</asp:Label>
                                    <asp:DropDownList ID="Cmb_Conti" runat="server" CssClass="floatSX">
                                    </asp:DropDownList>
                                    <asp:CheckBox ID="Chk_Fratelli" runat="server" CssClass="floatSX" Text="Anche i fratelli">
                                    </asp:CheckBox>
                                    <asp:CheckBox ID="Chk_Figli" runat="server" CssClass="floatSX" Text="Anche i figli">
                                    </asp:CheckBox>
                                    <input id="Txt_FiltroConti" class="Nascosto" name="Txt_FiltroConti" runat="server" />
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="clear">
                    </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>

                    <asp:Panel ID="Pannello_Movimenti" runat="server">
                        
                    <asp:Panel ID="Pannello_Risultati_MovEco" runat="server">
                        <asp:Label ID="Label10" runat="server" CssClass="testo_08_nero_bold">Num. Dettagli Movimenti :</asp:Label>
                        <asp:TextBox ID="Txt_NumMovimenti" runat="server" ReadOnly="True"></asp:TextBox>
                        <asp:Label ID="Label11" runat="server" CssClass="testo_08_nero_bold">Totale Imponibile :</asp:Label>
                        <asp:TextBox ID="Txt_Imponibile" runat="server" ReadOnly="True"></asp:TextBox>
                        <asp:Label ID="Label15" runat="server" CssClass="testo_08_nero_bold">Totale Imposta :</asp:Label>
                        <asp:Label ID="Label16" runat="server" CssClass="testo_08_nero_bold">Totale Importo:</asp:Label>
                        <asp:TextBox ID="Txt_Imposta" runat="server" ReadOnly="True"></asp:TextBox>
                        <asp:TextBox ID="Txt_Importo" runat="server" ReadOnly="True"></asp:TextBox>
                    </asp:Panel>
                    <asp:Panel ID="Pannello_Risultati_BolleDDT" runat="server">
                        <asp:Label ID="Label26" runat="server" CssClass="testo_08_nero_bold">Num. Dettagli Movimenti:</asp:Label>
                        <asp:TextBox ID="Txt_NumMovimenti2" runat="server" ReadOnly="True"></asp:TextBox>
                        <asp:Label ID="Label25" runat="server" CssClass="testo_08_nero_bold">Totale Imponibile:</asp:Label>
                        <asp:TextBox ID="Txt_Imponibile_2" runat="server" ReadOnly="True"></asp:TextBox>
                        <asp:Label ID="Label23" runat="server" CssClass="testo_08_nero_bold">Totale Quantità:</asp:Label>
                        <asp:TextBox ID="Txt_Quantita" runat="server" ReadOnly="True"></asp:TextBox>
                    </asp:Panel>

                    <br />

                    <asp:DataGrid ID="DataGrid_Movimenti" runat="server" ShowFooter="True" CellPadding="1"
                            CellSpacing="3" AutoGenerateColumns="False" HorizontalAlign="Center" CssClass="ui-widget-content">
                            <FooterStyle></FooterStyle>
                            <AlternatingItemStyle Font-Size="8pt" Font-Names="Verdana" HorizontalAlign="Left"
                                ForeColor="Black" VerticalAlign="Middle"></AlternatingItemStyle>
                            <ItemStyle Font-Size="8pt" Font-Names="Verdana" HorizontalAlign="Left" ForeColor="Black"
                                VerticalAlign="Middle"></ItemStyle>
                            <HeaderStyle CssClass="ui-widget-header" />
                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateColumn>
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle Font-Bold="True" HorizontalAlign="Center" ForeColor="Blue" VerticalAlign="Middle">
                                    </ItemStyle>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ChkSelezionaOperazione" AutoPostBack="false" runat="server"></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="Data" HeaderText="Data Movimento">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Doc_Numero_Sin" HeaderText="Doc_Numero_Sin">
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Doc_Numero" HeaderText="Doc_Numero">
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Doc_Numero_Des" HeaderText="Doc_Numero_Des">
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Doc_Numero_Completo" HeaderText="Num. Documento">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Lav_Cod" HeaderText="Lav_Cod"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Causale" HeaderText="Causale">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Scadenza" HeaderText="Scadenza">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Saldato" HeaderText="Saldato">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Cod_RisUm" HeaderText="Cod_RisUm"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Cod_Contatto" HeaderText="Piva / Cod.Fisc Contatto">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Contatto" HeaderText="Contatto">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Anno" HeaderText="Anno"></asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Ric_Cod" HeaderText="Ric_Cod"></asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Cod_Conto" HeaderText="Cod_Conto"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Conto" HeaderText="Conto">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Qta" HeaderText="Quantit&#224;">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Prezzo_Unitario" HeaderText="Prezzo Unitario">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Prezzo_Unitario_Netto" HeaderText="Prezzo Unitario Netto">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Sconto" HeaderText="Sconto">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Imponibile" HeaderText="Imponibile (&amp;euro;)">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Cod_Iva" HeaderText="Cod_Iva">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Aliquota" HeaderText="Aliquota">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="IVA" HeaderText="IVA (&amp;euro;)">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Importo" HeaderText="Importo (&amp;euro;)">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Blocco_Flag" HeaderText="Bloccato">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Blocco_Data" HeaderText="Data Blocco&lt;br/&gt;/Sblocco">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Blocco_Username" HeaderText="Username Blocco">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Id_Agenda" HeaderText="Id_Agenda">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="XML_MovDettagli" HeaderText="XML_MovDettagli">
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Stato_Export" HeaderText="Stato Esportazione">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </asp:Panel>

                </asp:Panel>
                 
                
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
