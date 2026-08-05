<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="../Master/Agenda.master"
    CodeBehind="DocumentoContabileGenerico_APP.aspx.vb" Inherits="AgroAgenda_2010.DocumentoContabileGenerico_APP"
    meta:resourcekey="PageResource1" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="cHead" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <script language="javascript" type="text/javascript">
        $(function () {
            $("#tabs").tabs();
        });
    </script>
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <div id="tabs">
        <ul>
            <li><a href="#tabs-1">Testata</a></li>
            <li><a href="#tabs-2">Dettagli</a></li>
            <li><a href="#tabs-3">Allegati</a></li>
            <li><a href="#tabs-4">Pagamenti</a></li>
        </ul>
        <div id="tabs-1" class="cento" style="padding: 10px">
            <div class="cento boxColore" style="padding: 10px">
                <div id="contenutoSx" class="Box80_SX">
                    <!------------------------------>
                    <div class="Box100">
                        <div id="numerazione" style="width: 35%; float: left;">
                            <div style="width: 25%; float: left; margin-top: 13px;">
                                <asp:Label ID="Label1" runat="server">Numero Fattura :</asp:Label>
                            </div>
                            <div style="width: 25%; float: left;">
                                <div>
                                    <asp:Label ID="Label28" runat="server">prefisso</asp:Label></div>
                                <div>
                                    <asp:TextBox ID="Txt_NumFattura_Sin" runat="server" CssClass="txtUI txt_75px"></asp:TextBox></div>
                            </div>
                            <div style="width: 25%; float: left;">
                                <div>
                                    <asp:Label ID="Label11" runat="server">numero</asp:Label></div>
                                <div>
                                    <asp:TextBox ID="Txt_NumFattura" runat="server" CssClass="txtUI txt_75px"></asp:TextBox></div>
                            </div>
                            <div style="width: 25%; float: left;">
                                <div>
                                    <asp:Label ID="Label13" runat="server">suffisso</asp:Label></div>
                                <div>
                                    <asp:TextBox ID="Txt_NumFattura_Des" runat="server" CssClass="txtUI txt_75px"></asp:TextBox></div>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div id="progressivi" style="width: 65%; float: left; margin-top: 13px;">
                            <div style="width: 20%; float: left">
                                <asp:Label ID="Label5" runat="server">Progressivo Protocollo: </asp:Label></div>
                            <div style="width: 20%; float: left">
                                <asp:TextBox ID="Txt_ProgrProtocollo" runat="server" Enabled="False" ReadOnly="True"
                                    CssClass="txtUI txt_75px"></asp:TextBox></div>
                            <div style="width: 20%; float: left">
                                <asp:Label ID="Label2" runat="server">Progressivo Registrazione: </asp:Label></div>
                            <div style="width: 20%; float: left">
                                <asp:TextBox ID="Txt_ProgrRegistrazione" runat="server" Enabled="False" ReadOnly="True"
                                    CssClass="txtUI txt_75px"></asp:TextBox></div>
                            <div style="width: 20%; float: left">
                                <asp:Label ID="Label4" runat="server">(generato automaticamente)</asp:Label>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                    <!------------------------------>
                    <!------------------------------>
                    <div class="Box100">
                        <div id="DivDate" style="width: 100%; float: left;">
                            <div style="width: 16%; float: left; margin-top: 13px;">
                                <asp:Label ID="Label3" runat="server">Data Emissione :</asp:Label>
                            </div>
                            <div style="width: 16%; float: left;">
                                <asp:TextBox ID="Txt_DataEmissione" runat="server" CssClass="txtUI datepicker change_per_load hasDatepicker"></asp:TextBox>
                            </div>
                            <div style="width: 16%; float: left; margin-top: 13px;">
                                <asp:Label ID="Label40" runat="server">Data Scadenza :</asp:Label>
                            </div>
                            <div style="width: 16%; float: left;">
                                <asp:TextBox ID="Txt_DataScadenza" runat="server" CssClass="txtUI datepicker change_per_load hasDatepicker"></asp:TextBox>
                            </div>
                            <div style="width: 16%; float: left;">
                                <asp:Label ID="Label34" runat="server">Data Registrazione :</asp:Label>
                            </div>
                            <div style="width: 16%; float: left;">
                                <asp:TextBox ID="Txt_DataRegistrazione" runat="server" CssClass="txtUI datepicker change_per_load hasDatepicker"></asp:TextBox>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                    <!------------------------------>
                    <div class="Box100">
                        <div id="DivDes" style="width: 100%; float: left;">
                            <div style="width: 10%; float: left;">
                                <asp:Label ID="Label27" runat="server" Height="8px" Width="88px">Descrizione :</asp:Label>
                            </div>
                            <div style="width: 90%; float: left;">
                                <asp:TextBox ID="Txt_Descrizione" runat="server" CssClass="txtUI" Width="100%"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                    <!------------------------------->
                    <!------------------------------>
                    <div class="Box100">
                        <div id="DivNote" style="width: 100%; float: left;">
                            <div style="width: 10%; float: left;">
                                <asp:Label ID="Label8" runat="server">Note :</asp:Label>
                            </div>
                            <div style="width: 90%; float: left;">
                                <asp:TextBox ID="Txt_Note" runat="server" CssClass="txtUI" Width="100%"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                    <!------------------------------->
                </div>
                <div id="ContenutoDX" class="Box20_DX">
                    <asp:RadioButtonList ID="Rbl_TipoFattura" runat="server" Enabled="False">
                        <asp:ListItem Value="0">Fattura Differita</asp:ListItem>
                        <asp:ListItem Value="1">Fattura Immediata</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <div class="clear">
                </div>
            </div>
            <div class="clear ">
            </div>
            <div class="cento boxColore" style="padding: 10px">
                <div id="Pannello_MittenteDestinatario" class="box50 boxColore" style="padding: 5px">
                    <div>
                        <asp:Label ID="Lbl_DettagliContatto" runat="server">Dettagli Contatto :</asp:Label>
                    </div>
                    <div id="RicercaContatto">
                        <asp:CheckBox ID="Chk_Contatto_GIAS" runat="server" Text="Contatto GIAS" Visible="False">
                        </asp:CheckBox><br />
                        <div id="RicercaContatto_ragSoc">
                            <asp:Label ID="Lbl_Cerca_Mittente" runat="server" Style="float: left; min-width: 200px">Cerca per ragione sociale:</asp:Label>
                            <asp:TextBox ID="Txt_Cerca_Mittente" CssClass="txtUI" runat="server" Style="float: left;"></asp:TextBox>
                            <asp:ImageButton ID="Img_btn_CercaMittente" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                                Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                                Style="float: left; padding-left: 10px"></asp:ImageButton>
                        </div>
                        <div class="clear">
                        </div>
                        <div id="RicercaContatto_PIVA">
                            <asp:Label ID="Lbl_Cerca_Mittente2" runat="server" Style="float: left; min-width: 200px">Cerca per partita iva :</asp:Label>
                            <asp:TextBox ID="Txt_Cerca_Mittente2" CssClass="txtUI" runat="server" MaxLength="11"
                                Style="float: left;"></asp:TextBox>
                            <asp:ImageButton ID="Img_btn_CercaMittente2" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                                Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                                Style="float: left; padding-left: 10px"></asp:ImageButton>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                    <div id="datiContatto">
                        <div>
                            <asp:Label ID="Lbl_Contatto" runat="server">Destinatario :</asp:Label>
                            <asp:DropDownList ID="Cmb_Contatti" runat="server" Width="60%" AutoPostBack="True">
                            </asp:DropDownList>
                            <div style="float: right; padding-right: 10px">
                                <div>
                                    <asp:Label ID="Label29" runat="server">Nuovo Contatto</asp:Label>
                                </div>
                                <div style="text-align: center">
                                    <asp:ImageButton ID="imgBtn_Nuovo_Contatto" runat="server" ImageUrl="../AB_Immagini/icone32/Nuovo.ico"
                                        Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca.">
                                    </asp:ImageButton>
                                </div>
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <div>
                            <asp:Label ID="Label9" runat="server" Style="min-width: 90px; float: left">Partita IVA :</asp:Label>
                            <asp:TextBox ID="Txt_CodContatto_Contatto" CssClass="txtUI_120" runat="server" ReadOnly="True"></asp:TextBox>
                            <asp:Label ID="Label30" runat="server" Style="min-width: 90px;">Codice Fiscale :</asp:Label>
                            <asp:TextBox ID="Txt_CodiceFiscale_Contatto" CssClass="txtUI_120" runat="server"
                                ReadOnly="True"></asp:TextBox>
                            <asp:TextBox ID="Txt_CodIndirizzo_Contatto" runat="server" ReadOnly="True" Visible="False"></asp:TextBox>
                        </div>
                        <div style="padding-top: 5px">
                            <asp:Label ID="Label10" runat="server" Style="min-width: 90px; float: left">Progressivo :</asp:Label>
                            <asp:TextBox ID="Txt_Progressivo_Contatto" CssClass="txtUI_120" runat="server" ReadOnly="True"></asp:TextBox>
                            <asp:Label ID="Label12" runat="server" Style="min-width: 90px;">Attività :</asp:Label>
                            <asp:TextBox ID="Txt_Attivita_Contatto" runat="server" CssClass="txtUI_120" ReadOnly="True"></asp:TextBox>
                        </div>
                        <div style="padding-top: 5px">
                            <asp:Label ID="Label19" runat="server" Height="16px" Width="104px">Tipo Indirizzo :</asp:Label>
                            <asp:DropDownList ID="Cmb_TipoIndirizzo_Contatto" runat="server" Enabled="False"
                                AutoPostBack="True">
                            </asp:DropDownList>
                            <asp:ListBox ID="List_Indirizzo_Contatto" runat="server" Height="58px" Width="100%">
                            </asp:ListBox>
                        </div>
                    </div>
                </div>
            </div>
            <div id="Pannello_DestinatarioDiverso" class="box50 boxColore" style="float: right;
                padding: 5px">
                <div style="padding: 5px">
                    <div>
                        <asp:CheckBox ID="Chk_DestinatarioDiverso" runat="server" Text="Destinatario Diverso"
                            AutoPostBack="True"></asp:CheckBox>
                    </div>
                    <br />
                    <div id="RicercaDiverso">
                        <asp:Label ID="Lbl_Cerca_Diverso" runat="server" Style="float: left; min-width: 200px">Cerca per ragione sociale :</asp:Label>
                        <asp:TextBox ID="Txt_Cerca_Diverso" CssClass="txtUI" runat="server" Style="float: left;"></asp:TextBox>
                        <asp:ImageButton ID="Img_btn_CercaDiverso" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                            Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                            Style="float: left; padding-left: 10px"></asp:ImageButton><br />
                        <asp:Label ID="Lbl_Cerca_Diverso2" runat="server" Style="float: left; min-width: 200px">Cerca per partita iva :</asp:Label>
                        <asp:TextBox ID="Txt_Cerca_Diverso2" runat="server" CssClass="txtUI" MaxLength="11"
                            Style="float: left"></asp:TextBox>
                        <asp:ImageButton ID="Img_btn_CercaDiverso2" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                            Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                            Style="float: left; padding-left: 10px"></asp:ImageButton>
                    </div>
                    <div class="clear">
                    </div>
                    <asp:Label ID="Label6" runat="server">Destinatario :</asp:Label>
                    <asp:DropDownList ID="Cmb_DestinatarioDiverso" runat="server" Height="19px" Width="70%"
                        Enabled="False" AutoPostBack="True">
                    </asp:DropDownList>
                    <br />
                    <div style="padding-top: 5px">
                        <asp:TextBox ID="Txt_CodIndirizzo_DestDiverso" runat="server" ReadOnly="True" Visible="False"></asp:TextBox>
                        <asp:Label ID="Label41" runat="server">Partita IVA :</asp:Label>
                        <asp:TextBox ID="Txt_CodContatto_DestDiverso" CssClass="txtUI_120" runat="server"
                            ReadOnly="True"></asp:TextBox>
                        <asp:Label ID="Label43" runat="server" Height="16px">Codice Fiscale :</asp:Label>
                        <asp:TextBox ID="Txt_CodiceFiscale_DestDiverso" runat="server" CssClass="txtUI_120"
                            ReadOnly="True"></asp:TextBox>
                        <br />
                        <asp:Label ID="Label45" runat="server">Progressivo :</asp:Label>
                        <asp:TextBox ID="Txt_Progressivo_DestDiverso" runat="server" CssClass="txtUI_120"
                            ReadOnly="True"></asp:TextBox>
                        <asp:Label ID="Label44" runat="server" Height="16px" Width="58px">Attività :</asp:Label>
                        <asp:TextBox ID="Txt_Attivita_DestDiverso" runat="server" CssClass="txtUI_120" ReadOnly="True"></asp:TextBox>
                        <asp:CheckBox ID="Chk_Destinatario_GIAS" runat="server" Text="Destinatario GIAS"
                            Visible="False"></asp:CheckBox>
                        <asp:Label ID="Label21" runat="server">Tipo Indirizzo :</asp:Label>
                        <asp:DropDownList ID="Cmb_TipoIndirizzo_DestDiverso" runat="server" Enabled="False"
                            AutoPostBack="True">
                        </asp:DropDownList>
                        <asp:ListBox ID="List_Indirizzo_DestDiverso" runat="server" Height="58px" Width="100%">
                        </asp:ListBox>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
            <div id="Pannello_Vettore" class="box50 boxColore" style="padding: 5px">
                <asp:Label ID="Label15" runat="server">Trasporto :</asp:Label>
                <div>
                    <div class="boxColore" style="float: left; width: 33%">
                        <asp:Label ID="Label7" runat="server">Trasporto a cura del :</asp:Label>
                        <asp:RadioButtonList ID="Rbl_Trasporto" runat="server" AutoPostBack="True">
                            <asp:ListItem Value="Cedente">Mittente (Cedente)</asp:ListItem>
                            <asp:ListItem Value="Cessionario">Destinatario (Cessionario)</asp:ListItem>
                            <asp:ListItem Value="Vettore">Vettore</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <div style="float: right; width: 66%">
                        <asp:Label ID="Label16" runat="server">Vettore :</asp:Label>
                        <asp:DropDownList ID="Cmb_Vettore" runat="server" Width="100%" Enabled="False" AutoPostBack="True">
                        </asp:DropDownList>
                        <asp:CheckBox ID="Chk_Vettore_GIAS" runat="server" Height="16px" Width="152px" Text="Vettore GIAS"
                            Visible="False"></asp:CheckBox>
                        <asp:Label ID="Label42" runat="server" Style="min-width: 100px">Partita IVA :</asp:Label>
                        <asp:TextBox ID="Txt_CodContatto_Vettore" runat="server" Height="16px" CssClass="txtUI_120"
                            ReadOnly="True"></asp:TextBox><br />
                        <asp:Label ID="Label46" runat="server" Style="min-width: 100px">Cod Fiscale :</asp:Label>
                        <asp:TextBox ID="Txt_CodiceFiscale_Vettore" CssClass="txtUI_120" runat="server" ReadOnly="True"></asp:TextBox>
                    </div>
                    <div class="clear">
                    </div>
                    <div>
                        <asp:Label ID="Label17" runat="server" Height="16px" Width="104px">Tipo Indirizzo :</asp:Label>
                        <asp:DropDownList ID="Cmb_TipoIndirizzo_Vettore" runat="server" Height="19px" Width="160px"
                            Enabled="False" AutoPostBack="True">
                        </asp:DropDownList>
                        <asp:TextBox ID="Txt_CodIndirizzo_Vettore" runat="server" Height="17px" ReadOnly="True"
                            Visible="False"></asp:TextBox>
                        <asp:ListBox ID="List_Indirizzo_Vettore" runat="server" Height="58px" Width="100%">
                        </asp:ListBox>
                    </div>
                </div>
            </div>
            <div id="Pannello_Mezzo" class="box50 boxColore" style="float: right; padding: 5px">
                <asp:Label ID="Label14" runat="server" Height="16px" Width="192px">Dettagli Mezzo di Trasporto :</asp:Label>
                <div>
                    <asp:CheckBox ID="Chk_ParcoMacchine" runat="server" Text="Gestisci Parco Macchine"
                        AutoPostBack="True"></asp:CheckBox>
                    <asp:Label ID="Label18" runat="server" Height="16px" Width="56px">Targa :</asp:Label>
                    <asp:TextBox ID="Txt_Targa" runat="server" CssClass="txtUI_120"></asp:TextBox>
                    <asp:DropDownList ID="Cmb_Targa" runat="server" Height="19px" Width="136px" Visible="False"
                        AutoPostBack="True">
                    </asp:DropDownList>
                    <asp:Label ID="Label25" runat="server" Height="16px" Width="96px">Peso Tara Kg :</asp:Label>
                    <asp:TextBox ID="Txt_PesoTara" runat="server" CssClass="txtUI_120"></asp:TextBox><br />
                    <asp:Label ID="Label20" runat="server" Height="16px" Width="168px">Num. Immatricolazione :</asp:Label>
                    <asp:TextBox ID="Txt_Immatr" runat="server" CssClass="txtUI_120"></asp:TextBox><br />
                    <asp:Label ID="Label22" runat="server" Height="16px" Width="238px">Num. Immatricolazione Rimorchio :</asp:Label>
                    <asp:TextBox ID="Txt_ImmatrRimorchio" runat="server" CssClass="txtUI_120"></asp:TextBox><br />
                    <asp:Label ID="Label23" runat="server" Height="16px" Width="214px">Num. Autorizzazione Trasporto :</asp:Label>
                    <asp:TextBox ID="Txt_Autorizz" runat="server" CssClass="txtUI_120"></asp:TextBox><br />
                    <asp:Label ID="Label24" runat="server" Height="16px" Width="214px">Data Rilascio Autorizzazzione :</asp:Label>
                    <asp:TextBox ID="Txt_DataAutorizz" runat="server" CssClass="txtUI_120"></asp:TextBox><br />
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div id="tabs-2" class="cento" style="padding: 10px">

            <div id="intestazioneDett" style="padding: 10px; width: 100%">
                <asp:panel ID="PulsantieraDettagli" runat="server" CssClass="boxColore" style="width:30%; float:left; padding:10px">
                    <asp:imageBUTTON runat="server" ImageUrl="../AB_Immagini/Icone32/MenuProdotti01.ico" ID="ID_Prodotto" ToolTip="Carica un Prodotto nella fattura" />
                    <asp:imageBUTTON runat="server" ImageUrl="../AB_Immagini/Icone32/Ope_Zootecniche.ico" ID="ID_Zootecnico" ToolTip="Carica una consistenza zootecnica nella fattura" />
                    <asp:imageBUTTON runat="server" ImageUrl="../AB_Immagini/Icone32/Clip32.ico" ID="ID_Allegati" ToolTip="Allega D.D.T." />
                    <asp:imageBUTTON runat="server" ImageUrl="../AB_Immagini/Icone32/Cancella.bmp" ID="ID_Cancella" ToolTip="Rimuovi tutti i dettagli inseriti in tabella" />
                    <asp:imageBUTTON runat="server" ImageUrl="../AB_Immagini/Icone32/Stampa.ico" ID="ID_Stampa" ToolTip="Stampa la fattura" />
                </asp:panel>
                <asp:Panel ID="Pannello_Peso" runat="server" CssClass="boxColore" style="float:right; width:68%">
                    <asp:Label ID="Lbl_Peso" runat="server">Peso Netto Kg :</asp:Label>
                    <asp:TextBox ID="Txt_PesoNetto" runat="server" CssClass="txtUI_120">0</asp:TextBox>
                    <asp:Button ID="Btn_RicavaPesoNetto" runat="server" Text="Calcola">
                    </asp:Button>
                    <asp:Label ID="Lbl_CalcoloPeso" runat="server">Ricava il Peso Netto totale da tutti i dettagli inseriti nella tabella</asp:Label>
                </asp:Panel>
                <div class="clear" />
            </div>
            


            <asp:Label ID="Lbl_Prodotti" runat="server">&nbsp;Elenco Prodotti :</asp:Label>
            <asp:TextBox ID="TxtQueryStringProdotto" runat="server" Height="1px" Width="1px"></asp:TextBox>
            <input id="InsProdotto" size="9" type="hidden" name="InsProdotto" runat="server" />
            <asp:Panel ID="Pannello_Prodotti" runat="server" Height="242px" style="overflow:scroll">
                <asp:DataGrid ID="DataGrid_Prodotti" runat="server" Width="800px" AutoGenerateColumns="False"
                    GridLines="None" CellSpacing="3" CellPadding="1">
                    <AlternatingItemStyle></AlternatingItemStyle>
                    <ItemStyle Height="25px"></ItemStyle>
                    <HeaderStyle Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                    <Columns>
                        <asp:BoundColumn Visible="False" DataField="Chiave" HeaderText="Chiave">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Codice" HeaderText="Cod.">
                            <HeaderStyle HorizontalAlign="Center" Width="30px" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Pendente" HeaderText="Pendente"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Allegato" HeaderText="Allegato">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Tipo" HeaderText="Tipo"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Elem_Cod" HeaderText="Elem_Cod"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Pro_Cod" HeaderText="Pro_Cod"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Mat_Cod" HeaderText="Mat_Cod"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Cod_Progetto" HeaderText="Cod_Progetto">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Fase_Cod" HeaderText="Fase_Cod"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Lotto" HeaderText="Lotto"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Cal_Cod" HeaderText="Cal_Cod"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Udm_Cod" HeaderText="Udm_Cod"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Udm_Cod_Extra" HeaderText="Udm_Cod_Extra">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Qta_Extra" HeaderText="Qta_Extra"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Prezzo_Effettivo" HeaderText="Prezzo_Effettivo">
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Udm_Des" HeaderText="Unit&#224; Misura">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Qta" HeaderText="Quantit&#224;">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Prezzo_Unitario" HeaderText="Prezzo Unitario">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Prezzo_Unitario_Netto" HeaderText="Prezzo Unitario Netto">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Cod_Variazione" HeaderText="Cod_Variazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Variazione_Perc" HeaderText="Sconto / Magg %">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Variazione" HeaderText="Sconto / Magg">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Imponibile" HeaderText="Imponibile">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Imponibile_Netto" HeaderText="Imponibile Netto">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Cod_Iva" HeaderText="Cod_Iva"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Aliquota" HeaderText="Aliquota">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="IVA" HeaderText="IVA">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Importo" HeaderText="Importo">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Anno" HeaderText="Anno">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Ric_Cod" HeaderText="Ric_Cod"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Cod_Conto" HeaderText="Cod_Conto"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Conto" HeaderText="Conto">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Piva_Destinazione" HeaderText="Piva_Destinazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Sa_Cod_Destinazione" HeaderText="Sa_Cod_Destinazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Id_Agenda_Destinazione" HeaderText="Id_Agenda_Destinazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Id_Mov_Destinazione" HeaderText="Id_Mov_Destinazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Id_Mov_Det_Destinazione" HeaderText="Id_Mov_Det_Destinazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Id_Destinazione" HeaderText="Id_Destinazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Destinazione" HeaderText="Destinazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Piva_Provenienza" HeaderText="Piva_Provenienza">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Sa_Cod_Provenienza" HeaderText="Sa_Cod_Provenienza">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Id_Agenda_Provenienza" HeaderText="Id_Agenda_Provenienza">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Id_Mov_Provenienza" HeaderText="Id_Mov_Provenienza">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Id_Mov_Det_Provenienza" HeaderText="Id_Mov_Det_Provenienza">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Id_Provenienza" HeaderText="Id_Provenienza">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Provenienza" HeaderText="Provenienza">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Xml_Destinazione" HeaderText="Xml_Destinazione">
                        </asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="Xml_Provenienza" HeaderText="Xml_Provenienza">
                        </asp:BoundColumn>
                        <asp:ButtonColumn Text="&lt;img src='../../AB_Immagini/icone16/cE.ico' border='0'&gt;"
                            HeaderText="Mod" CommandName="Modifica">
                            <HeaderStyle HorizontalAlign="Center" Width="30px" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:ButtonColumn>
                        <asp:ButtonColumn Text="&lt;img src='../../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                            HeaderText="Canc" CommandName="Cancella">
                            <HeaderStyle HorizontalAlign="Center" Width="30px" VerticalAlign="Middle"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:ButtonColumn>
                    </Columns>
                </asp:DataGrid>
            </asp:Panel>
            <div class="clear" />
            <div id="DetpannelliGiu">
                <asp:Panel ID="Pannello_IVA" runat="server" Height="218px" Width="50%" style="width:50%;float:left">
                    <asp:Label ID="lblriva" runat="server" Height="8px" Width="104px">&nbsp;Riepilogo IVA :</asp:Label>
                    <asp:DataGrid ID="DataGrid_IVA" runat="server" AutoGenerateColumns="False" GridLines="None"
                        CellSpacing="3" CellPadding="1">
                        <AlternatingItemStyle></AlternatingItemStyle>
                        <ItemStyle Height="25px"></ItemStyle>
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                        <Columns>
                            <asp:BoundColumn Visible="False" DataField="Cod_Iva" HeaderText="Cod_Iva">
                                <HeaderStyle HorizontalAlign="Center" Width="30px" VerticalAlign="Middle"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Aliquota" HeaderText="Aliquota">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Imponibile" HeaderText="Imponibile">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Iva" HeaderText="Iva">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Importo" HeaderText="Importo">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                            </asp:BoundColumn>
                        </Columns>
                    </asp:DataGrid>
                </asp:Panel>
                <asp:Panel ID="Pannello_Riepilogo" runat="server" Height="218px" style="width:50%; float:right">
                    <asp:Label ID="Txt_TipoSconto" runat="server" Height="16px" Width="192px">Modalità di applicazione della variazione del prezzo :</asp:Label>
                    <asp:DropDownList ID="Cmb_TipoSconto" runat="server" Width="230px">
                    </asp:DropDownList>
                    <asp:Label ID="Label32" runat="server" Height="16px" Width="214px">Totale Imponibile Lordo :</asp:Label>
                    <asp:Label ID="Label33" runat="server" Height="16px" Width="214px">Totale Variazioni :</asp:Label>
                    <asp:Label ID="Label37" runat="server" Height="16px" Width="214px">Totale Imponibile Netto :</asp:Label>
                    <asp:Label ID="Label38" runat="server" Height="16px" Width="214px">Totale IVA :</asp:Label>
                    <asp:Label ID="Label39" runat="server" Height="16px" Width="214px">Totale Importo Fattura :</asp:Label>
                    <asp:TextBox ID="Txt_ImponibileLordo" runat="server" Height="16px" Width="176px"
                        ReadOnly="True"></asp:TextBox>
                    <asp:TextBox ID="Txt_Variazioni" runat="server" Height="16px" Width="176px" ReadOnly="True"></asp:TextBox>
                    <asp:TextBox ID="Txt_ImponibileNetto" runat="server" Height="16px" Width="176px"
                        ReadOnly="True"></asp:TextBox>
                    <asp:TextBox ID="Txt_IVA" runat="server" Height="16px" Width="176px" ReadOnly="True"></asp:TextBox>
                    <asp:TextBox ID="Txt_Importo" runat="server" Height="16px" Width="176px" ReadOnly="True"></asp:TextBox>
                    <hr size="1" width="100%" />
                    <asp:Label ID="Label26" runat="server" Height="10px" Width="12px">&euro;</asp:Label>
                    <asp:Label ID="Label36" runat="server" Height="10px" Width="12px">&euro;</asp:Label>
                    <asp:Label ID="Label47" runat="server" Height="10px" Width="12px">&euro;</asp:Label>
                    <asp:Label ID="Label48" runat="server" Height="10px" Width="12px">&euro;</asp:Label>
                    <asp:Label ID="Label49" runat="server" Height="10px" Width="12px">&euro;</asp:Label>
                </asp:Panel>
                <asp:Label ID="Label31" runat="server" Height="8px" Width="184px">&nbsp;Riepilogo del Documento :</asp:Label>
                
                <input id="Input_AllegaDoc" size="9" type="hidden" name="Input_AllegaDoc" runat="server" />
            </div>
            <div class="clear" />
        </div>
        <div id="tabs-3" class="cento" style="padding: 10px">
            terzo tab
        </div>
        <div id="tabs-4" class="cento" style="padding: 10px">
            quarto tab
        </div>
    </div>
</asp:Content>
