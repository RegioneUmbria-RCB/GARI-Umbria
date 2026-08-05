<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="../Master/Agenda.master"
    CodeBehind="DocumentoContabileGenerico.aspx.vb" Inherits="AgroAgenda_2010.DocumentoContabileGenerico"
    meta:resourcekey="PageResource1" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="cHead" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <script language="javascript" type="text/javascript">

        function Contatto_gestisciValore_esci() {
            $("#dialog").dialog("close");
        }

        function Contatto_gestisciValore(valore) {
            $("#dialog").dialog("close");
            $.logThis("impostato il valore su Txt_Cerca_Mittente2");
            $("#<%=Txt_Cerca_Mittente2.ClientID() %>").val(valore);

            clickButtonContatti();
        }

        function clickButtonContatti() {

            $.logThis("Chiamata a Img_btn_CercaMittente2");
            $("#<%=Img_btn_CercaMittente2.ClientID %>").click();

        }

        $(function () {

            $.logThis("inizializzazione pagina");

            $("#tabs").tabs();
            $("#tabTestata").on("click", function () { $("#<%=imgBtn_Testata.ClientID() %>").click(); });
            $("#tabDettagli").on("click", function () { $("#<%=imgBtn_Dettagli.ClientID() %>").click(); });

            $("#tabContatti").tabs();

        });

        function settavalore(valore, dove) {

            if (dove == "prodotto")
                $("#<%=InsProdotto.ClientID %>").val(valore);
            else
                $("#<%=Input_AllegaDoc.ClientID %>").val(valore);

            $("#dialog").dialog("close");

            document.getElementById("aspnetForm").submit();

        }

        function apriFormDialog(url, w, h) {

            $.logThis("apriFormDialog(w: " + w + ", h: " + h + ")");

            if (w != 0) {
                $('#dialog').dialog({
                    autoOpen: false,
                    height: h,
                    width: w,
                    maxHeight: $(window).height() - 30,
                    maxWidth: $(window).width() - 60,
                    modal: true
                });
            }

            $("#dialog").dialog("open");

            $("#xiframe").html("<iframe frameBorder='0' src='" + url + "' style='width:100%; height:" + ($("#dialog").height()) + "px'><iframe>");
        }


        function chiudidialog() {
            $("#dialog").dialog("close");
        }

        function CopiaValoreInTxt_DataRegistrazione() {
            //$('#<%= Txt_DataRegistrazione.ClientID %>').val($('#<%= Txt_DataEmissione.ClientID %>').val())
        }

    </script>
    <style>
        body
        {
            overflow-x: hidden;
        }
                .titoloBox{
            background: #042649;
        }
                .ui-widget-header{
            background: #042649;
            border-color: #042649;
        }
       .ui-autocomplete-input {
            width: 400px;
        }
    </style>
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <div id="contenitoreBody" style="padding-left: 3px; padding-right: 3px">
        <div id="toolSalva" class="boxColore" style="width: 20%; float: left; padding: 10px;
            display: none">
         
            <asp:UpdatePanel runat="server" ID="upButtons">
                <ContentTemplate>
                    <asp:ImageButton ID="ImgBtn_Testata" runat="server" Style="display: none" />
                    <asp:ImageButton ID="ImgBtn_Dettagli" runat="server" Style="display: none" />
                    <asp:Button ID="BTN_ChangeData" runat="server" Text="Button" Style="display: none"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="clear" style="display: none">
        </div>
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1" id="tabTestata">Testata</a></li>
                <li><a href="#tabs-2" id="tabDettagli">Dettagli</a></li>
                <li style="margin-left: 200px">
                    <%--<asp:Button ID="Btn_Salva" runat="server" Text="SALVA ed ESCI" Style="color: red" CssClass="bottone " />--%>
                    <asp:ImageButton ID="ImgBtn_Salva" runat="server" Text="SALVA ed ESCI" ImageUrl="../AB_Immagini/Icone24/btn_salva_esci.png" />
                </li>
                    
                <li style="margin-left: 20px">
                    <%--<asp:Button ID="Btn_Salva_Nuovo" runat="server" Text="SALVA e NUOVO" Style="color: red"
                        CssClass="bottone " />--%>
                    <asp:ImageButton ID="ImgBtn_Salva_Nuovo" runat="server" Text="SALVA ed ESCI" ImageUrl="../AB_Immagini/Icone24/btn_salva_nuovo.png" />
                </li>
            </ul>
            <div id="tabs-1" class="cento" style="padding: 10px">
                <asp:UpdatePanel ID="upTabs_1" runat="server" UpdateMode="Always">
                    <ContentTemplate>
         
                        <div class="cento boxColore" style="padding: 10px">
                            <div class="Box100">
                                <div id="DivFattura" style="width: 100%; float: left;">
                                    <div style="width: 20%; float: left;">
                                        <asp:RadioButtonList ID="Rbl_TipoFattura" runat="server" CssClass="boxColore" AutoPostBack="true"
                                            RepeatDirection="Horizontal">
                                            <asp:ListItem Value="0">Fattura Differita</asp:ListItem>
                                            <asp:ListItem Value="1">Fattura Immediata</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <div style="width: 80%; float: left;">
                                        <div id="numerazione" style="width: 65%; float: left;">
                                            <div style=" float: left; margin-top: 13px;">
                                                <asp:Label ID="Label1" runat="server" CssClass="red">Rif. Documento:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label>
                                            </div>
                                            <div style=" float: left;">
                                                <div>
                                                    <asp:Label ID="Label28" runat="server">es. DDT\Fat\&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label></div>
                                                <div>
                                                    <asp:TextBox ID="Txt_NumFattura_Sin" runat="server" CssClass="txtUI txt_75px"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;</div>
                                            </div>
                                            <div style=" float: left;">
                                                <div>
                                                    <asp:Label ID="Label11" runat="server" CssClass="red">es. 115</asp:Label></div>
                                                <div>
                                                    <asp:TextBox ID="Txt_NumFattura" runat="server" CssClass="txtUI txt_75px"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;</div>
                                            </div>
                                            <div style="width: 25%; float: left;">
                                                <div>
                                                    <asp:Label ID="Label13" runat="server">es. \2019</asp:Label></div>
                                                <div>
                                                    <asp:TextBox ID="Txt_NumFattura_Des" runat="server" CssClass="txtUI txt_75px"></asp:TextBox></div>
                                            </div>
                                   
                                        </div>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                                <!------------------------------>
                                <!------------------------------>
                                <div class="Box100">
                                    <div id="DivDate" style="width: 100%; float: left;">
                                        <div style="width: 32%; float: left; margin-top: 7px;">
                                            <asp:Label ID="Label3" runat="server" CssClass="red">Data Emissione:&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label>
                                             <asp:TextBox ID="Txt_DataEmissione" runat="server" CssClass="txtUI datepicker Txt_DataEmissione"
                                                Width="100px"></asp:TextBox>
                                        </div>
                                        <%--<div style="width: 16%; float: left;">
                                           
                                        </div>--%>
                                        <div style="width: 16%; float: left; margin-top: 7px;">
                                            <asp:Label ID="lbl_DataScadenza" runat="server">Data Scadenza:</asp:Label>
                                        </div>
                                        <div style="width: 16%; float: left;">
                                            <asp:TextBox ID="Txt_DataScadenza" runat="server" CssClass="txtUI datepicker" Width="100px"></asp:TextBox>
                                        </div>
                                        <div style="width: 16%; float: left; margin-top: 7px;">
                                            <asp:Label ID="Label34" runat="server">Data Registrazione:</asp:Label>
                                        </div>
                                        <div style="width: 16%; float: left;">
                                            <asp:TextBox ID="Txt_DataRegistrazione" runat="server" CssClass="txtUI datepicker Txt_DataRegistrazione"
                                                Width="100px"></asp:TextBox>
                                        </div>
                                        <div class="clear">
                                        </div>
                                    </div>
                                    <div class="clear" style="height: 0px">
                                    </div>
                                </div>
                                <div class="clear" style="height: 0px">
                                </div>
                                <div id="contenutoSx" class="">
                                    <!------------------------------>
                                    <div class="Box100">
                                    </div>
                                    <div id="progressivi" style="width: 65%; float: left; margin-top: 13px; display: none">
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
                                    <asp:Panel ID="panDDT" Style="width: 100%; float: left; margin-top: 13px;" runat="server">
                                        <div>
                                            <asp:Label ID="lNcolli" Style="min-width: 110px; float: left; padding-left: 5px"
                                                runat="server">Num. Colli :</asp:Label>
                                            <asp:TextBox ID="Txt_NumColli" Style="float: left" runat="server" CssClass="txtUI txt_75px"></asp:TextBox>
                                            <asp:Label ID="lblCausale" Style="min-width: 110px; float: left; padding-left: 5px"
                                                runat="server">Causale del trasporto</asp:Label>
                                            <asp:DropDownList ID="Cmb_Causale" Style="float: left" runat="server">
                                            </asp:DropDownList>
                                            <asp:Label ID="LblNaturabeni" Style="min-width: 110px; float: left; padding-left: 5px"
                                                runat="server">Natura dei beni</asp:Label>
                                            <asp:TextBox ID="Txt_NaturaBeni" Style="float: left" runat="server" CssClass="txtUI_170"></asp:TextBox>
                                            <asp:Label ID="lblAspetti" Style="min-width: 110px; float: left; padding-left: 5px"
                                                runat="server">Aspetto dei beni</asp:Label>
                                            <asp:DropDownList ID="Cmb_Aspetto" Style="float: left" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="clear" style="height: 0px">
                                        </div>
                                        <div class="Nascosto">
                                            <asp:Label ID="lblDataRitiro" Style="min-width: 110px; float: left; padding-left: 5px"
                                                runat="server">Data Ritiro:</asp:Label>
                                            <asp:TextBox ID="TXT_DataConsegna" runat="server" Style="float: left" CssClass="txtUI datepicker"></asp:TextBox>
                                            <asp:Label ID="lblOraRitiro" Style="min-width: 30px; float: left; padding-left: 5px"
                                                runat="server">Ora:</asp:Label>
                                            <asp:TextBox ID="txt_Ora" runat="server" CssClass="txtHour" Style="float: left"></asp:TextBox>
                                        </div>
                                        <div class="clear" style="height: 0px">
                                        </div>
                                    </asp:Panel>
                                    <div class="clear">
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                                <!------------------------------>
                                <div class="Box100">
                                    <div id="DivDes" style="width: 100%; float: left;">
                                        <div style="width: 10%; float: left;">
                                            <asp:Label ID="Label27" runat="server" Height="8px" Width="88px">Descrizione:</asp:Label>
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
                                            <asp:Label ID="Label8" runat="server">Note:</asp:Label>
                                        </div>
                                        <div style="width: 90%; float: left;">
                                            <asp:TextBox ID="Txt_Note" runat="server" CssClass="txtUI" Width="100%"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="clear" style="height: 0px">
                                </div>
                                <!------------------------------->
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div id="tabContatti" runat="server">
                    <ul>
                        <li><a href="#tabs-c1" id="tabC1">
                            <label id="Lbl_tabC1" runat="server">
                                Contatto</label></a></li>
                        <li><a href="#tabs-c2" id="tabC2">
                            <label id="Lbl_tabC2" runat="server">
                                Contatto Diverso</label></a></li>
                      
                        <li><a href="#tabs-c3" id="tabC3">
                            <label id="Lbl_tabC3" runat="server">
                                Trasporto</label></a></li>
                        <li style=" display:none"><a href="#tabs-c4" id="tabC4" runat="server">
                            <label id="Lbl_tabC4" runat="server">
                                Mezzo di Trasporto</label></a></li>
                            
                    </ul>
                    <div id="tabs-c1" class="cento" style="padding: 10px">
                        <asp:UpdatePanel ID="UpdatePanel_Contatto" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <div style="display: none">
                                    <asp:Button ID="BottoneNascostoContatti" runat="server" Text="BottoneNascostoContatti" />
                                </div>
                                <div id="RicercaContatto">
                                    <asp:CheckBox ID="Chk_Contatto_GIAS" runat="server" Text="Contatto GIAS" Visible="False">
                                    </asp:CheckBox><br />
                                    <div id="RicercaContatto_ragSoc">
                                        <asp:Label ID="Lbl_Cerca_Mittente" runat="server" Style="float: left; min-width: 200px">Cerca per ragione sociale:</asp:Label>
                                        <asp:TextBox ID="Txt_Cerca_Mittente" CssClass="txtUI" runat="server" Style="float: left;"></asp:TextBox>
                                        <asp:ImageButton ID="Img_btn_CercaMittente" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                                            Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                                            CssClass="btn_per_load" Style="float: left; padding-left: 10px"></asp:ImageButton>
                                                                            <asp:ImageButton ID="ImageButtonNuovoContatto" runat="server" ImageUrl="../AB_Immagini/icone32/Nuovo.ico"
                                            Height="32px" Width="32px" ToolTip="Premere il pulsante per Inserire Un nuovo Contatto."
                                            CssClass="btn_per_load" Style="float: left; padding-left: 10px"></asp:ImageButton>
                                    </div>
                                    <div class="clear">
                                    </div>
                                    <div id="RicercaContatto_PIVA">
                                        <asp:Label ID="Lbl_Cerca_Mittente2" runat="server" Style="float: left; min-width: 200px">Cerca per partita iva:</asp:Label>
                                        <asp:TextBox ID="Txt_Cerca_Mittente2" CssClass="txtUI" runat="server" MaxLength="11"
                                            Style="float: left;"></asp:TextBox>
                                        <asp:ImageButton ID="Img_btn_CercaMittente2" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                                            Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                                            CssClass="btn_per_load" Style="float: left; padding-left: 10px"></asp:ImageButton>
                                    </div>
                                    <div class="clear">
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                                <div id="datiContatto">
                                    <div>
                                        <asp:Label ID="Lbl_Contatto" Style="float: left" runat="server" CssClass="red">Destinatario:</asp:Label>
                                        <asp:DropDownList ID="Cmb_Contatti" Style="float: left" runat="server" Width="60%"
                                            AutoPostBack="True" CssClass="change_per_load">
                                        </asp:DropDownList>
                                        <div style="float: left; padding-left: 10px; padding-top: -10px">
                                            <div style="text-align: center" style="padding-top: -6px">
                                                <asp:ImageButton ID="imgBtn_Nuovo_Contatto" runat="server" ImageUrl="../AB_Immagini/icone32/Nuovo.ico"
                                                    Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                                                    CssClass="Nascosto"></asp:ImageButton>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="clear">
                                    </div>
                                    <div>
                                        <div>
                                            <div style="float: left; width: 50%">
                                                <asp:Label ID="Label9" runat="server" Style="min-width: 100px; float: left">Partita IVA: </asp:Label>
                                                <asp:TextBox ID="Txt_CodContatto_Contatto" CssClass="txtUI_120" Style="float: left"
                                                    runat="server" ReadOnly="True"></asp:TextBox>
                                            </div>
                                            <div style="float: right; width: 50%">
                                                <asp:Label ID="Label30" runat="server" Style="min-width: 100px;">Codice Fiscale: </asp:Label>
                                                <asp:TextBox ID="Txt_CodiceFiscale_Contatto" CssClass="txtUI_170" runat="server"
                                                    ReadOnly="True"></asp:TextBox>
                                                <asp:TextBox ID="Txt_CodIndirizzo_Contatto" runat="server" ReadOnly="True" Visible="False"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="clear">
                                        </div>
                                    </div>
                                    <div style="padding-top: 5px">
                                        <div>
                                            <div style="float: left; width: 50%">
                                                <asp:Label ID="Label10" runat="server" Style="min-width: 90px; float: left">Progressivo:</asp:Label>
                                                <asp:TextBox ID="Txt_Progressivo_Contatto" CssClass="txtUI_120" Style="float: left"
                                                    runat="server" ReadOnly="True"></asp:TextBox>
                                            </div>
                                            <div style="float: right; width: 50%">
                                                <asp:Label ID="Label12" runat="server" Style="min-width: 90px;">Attività:</asp:Label>
                                                <asp:TextBox ID="Txt_Attivita_Contatto" runat="server" CssClass="txtUI_120" ReadOnly="True"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="clear">
                                        </div>
                                    </div>
                                    <div style="padding-top: 5px">
                                        <asp:Label ID="Label19" runat="server" Height="16px" Width="104px">Tipo Indirizzo:</asp:Label>
                                        <asp:DropDownList ID="Cmb_TipoIndirizzo_Contatto" runat="server" Enabled="False"
                                            AutoPostBack="True" CssClass="change_per_load">
                                        </asp:DropDownList>
                                        <asp:ListBox ID="List_Indirizzo_Contatto" runat="server" Height="58px" Width="100%">
                                        </asp:ListBox>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="tabs-c2" class="cento" style="padding: 10px">
                        <asp:UpdatePanel ID="UpdatePanel_ContattoDiverso" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <div style="padding: 5px">
                                    <div id="RicercaDiverso">
                                        <asp:Label ID="Lbl_Cerca_Diverso" runat="server" Style="float: left; min-width: 200px">Cerca per ragione sociale:</asp:Label>
                                        <asp:TextBox ID="Txt_Cerca_Diverso" CssClass="txtUI" runat="server" Style="float: left;"></asp:TextBox>
                                        <asp:ImageButton ID="Img_btn_CercaDiverso" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                                            Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                                            CssClass="btn_per_load" Style="float: left; padding-left: 10px"></asp:ImageButton><br />
                                    <!--    <div class="clear">
                                        </div>
                                        <asp:Label ID="Lbl_Cerca_Diverso2" runat="server" Style="float: left; min-width: 200px">Cerca per partita iva:</asp:Label>
                                        <asp:TextBox ID="Txt_Cerca_Diverso2" runat="server" CssClass="txtUI" MaxLength="11"
                                            Style="float: left"></asp:TextBox>
                                        <asp:ImageButton ID="Img_btn_CercaDiverso2" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                                            Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                                            CssClass="btn_per_load" Style="float: left; padding-left: 10px"></asp:ImageButton>
                                   -->
                                    </div>
                                    <div class="clear">
                                    </div>
                                    <asp:Label ID="Lbl_DestinazioneDiversa" runat="server">Destinatario:</asp:Label>
                                    <asp:DropDownList ID="Cmb_DestinatarioDiverso" runat="server" wsidth="70%" AutoPostBack="True"
                                        CssClass="change_per_load">
                                    </asp:DropDownList>
                                    <br />
                                    <div style="padding-top: 15px">
                                        <div>
                                            <div style="float: left; width: 50%">
                                                <asp:TextBox ID="Txt_CodIndirizzo_DestDiverso" runat="server" ReadOnly="True" Visible="False"></asp:TextBox>
                                                <asp:Label ID="Label41" runat="server" Style="min-width: 96px; float: left">Partita IVA:</asp:Label>
                                                <asp:TextBox ID="Txt_CodContatto_DestDiverso" Style="float: left" CssClass="txtUI_120"
                                                    runat="server" ReadOnly="True"></asp:TextBox>
                                            </div>
                                            <div style="float: right; width: 50%">
                                                <asp:Label ID="Label43" runat="server" Style="min-width: 96px; float: left">Codice Fiscale:</asp:Label>
                                                <asp:TextBox ID="Txt_CodiceFiscale_DestDiverso" Style="float: left" runat="server"
                                                    CssClass="txtUI_170" ReadOnly="True"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="clear">
                                        </div>
                                    </div>
                                    <div style="padding-top: 5px">
                                        <div>
                                            <div style="float: left; width: 50%">
                                                <asp:Label ID="Label45" runat="server" Style="min-width: 96px; float: left">Progressivo:</asp:Label>
                                                <asp:TextBox ID="Txt_Progressivo_DestDiverso" Style="float: left" runat="server"
                                                    CssClass="txtUI_120" ReadOnly="True"></asp:TextBox>
                                            </div>
                                            <div style="float: right; width: 50%">
                                                <asp:Label ID="Label44" runat="server" Style="min-width: 96px; float: left">Attività:</asp:Label>
                                                <asp:TextBox ID="Txt_Attivita_DestDiverso" runat="server" Style="float: left" CssClass="txtUI_120"
                                                    ReadOnly="True"></asp:TextBox>
                                                <asp:CheckBox ID="Chk_Destinatario_GIAS" runat="server" Text="Destinatario GIAS"
                                                    Visible="False"></asp:CheckBox>
                                            </div>
                                        </div>
                                        <div class="clear">
                                        </div>
                                    </div>
                                    <div style="padding-top: 5px">
                                        <asp:Label ID="Label21" runat="server">Tipo Indirizzo:</asp:Label>
                                        <asp:DropDownList ID="Cmb_TipoIndirizzo_DestDiverso" runat="server" Enabled="False"
                                            AutoPostBack="True" CssClass="change_per_load">
                                        </asp:DropDownList>
                                        <asp:ListBox ID="List_Indirizzo_DestDiverso" runat="server" Height="58px" Width="100%">
                                        </asp:ListBox>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="tabs-c3" class="cento" style="padding: 10px">
                        <asp:UpdatePanel ID="UpdatePanel_Trasporto" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:Panel runat="server" ID="Pannello_Vettore" class="box100 boxColore" Style="padding: 5px">
                                    <div>
                                        <div class="boxColore" style="float: left; width: 33%">
                                            <asp:Label ID="Label7" runat="server">Trasporto a cura del:</asp:Label>
                                            <asp:RadioButtonList ID="Rbl_Trasporto" runat="server" AutoPostBack="True" CssClass="change_per_load">
                                                <asp:ListItem Value="Cedente">Mittente (Cedente)</asp:ListItem>
                                                <asp:ListItem Value="Cessionario">Destinatario (Cessionario)</asp:ListItem>
                                                <asp:ListItem Value="Vettore">Vettore</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <div style="float: right; width: 66%">
                                            <asp:Label ID="Label29" runat="server">Cerca:</asp:Label>
                                            <asp:TextBox ID="txt_CercaVettore" runat="server" Style="float: left" CssClass="txtUI"></asp:TextBox>
                                            <asp:ImageButton ID="ImgBtn_CercaVettore" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico"
                                                Height="32px" Width="32px" ToolTip="Premere il pulsante per eseguire la ricerca."
                                                CssClass="btn_per_load" Style="float: left; padding-left: 10px"></asp:ImageButton>
                                            <div class="clear">
                                            </div>
                                            <asp:Label ID="Label16" runat="server">Vettore:</asp:Label>
                                            <asp:DropDownList ID="Cmb_Vettore" runat="server" Width="100%" Enabled="False" AutoPostBack="True"
                                                CssClass="change_per_load">
                                            </asp:DropDownList>
                                            <asp:CheckBox ID="Chk_Vettore_GIAS" runat="server" Height="16px" Width="152px" Text="Vettore GIAS"
                                                Visible="False"></asp:CheckBox>
                                            <asp:Label ID="Label42" runat="server" Style="min-width: 100px; float: left">Partita IVA:</asp:Label>
                                            <asp:TextBox ID="Txt_CodContatto_Vettore" runat="server" Style="float: left" Height="16px"
                                                CssClass="txtUI_120" ReadOnly="True"></asp:TextBox>
                                            <div class="clear">
                                            </div>
                                            <asp:Label ID="Label46" runat="server" Style="min-width: 100px; float: left">Cod Fiscale:</asp:Label>
                                            <asp:TextBox ID="Txt_CodiceFiscale_Vettore" CssClass="txtUI_170" Style="float: left"
                                                runat="server" ReadOnly="True"></asp:TextBox>
                                            <div class="clear">
                                            </div>
                                        </div>
                                        <div class="clear">
                                        </div>
                                        <div>
                                            <asp:Label ID="Label17" runat="server" Height="16px" Width="104px">Tipo Indirizzo:</asp:Label>
                                            <asp:DropDownList ID="Cmb_TipoIndirizzo_Vettore" runat="server" Height="19px" Width="160px"
                                                Enabled="False" AutoPostBack="True" CssClass="change_per_load">
                                            </asp:DropDownList>
                                            <asp:TextBox ID="Txt_CodIndirizzo_Vettore" runat="server" Height="17px" ReadOnly="True"
                                                Visible="False"></asp:TextBox>
                                            <asp:ListBox ID="List_Indirizzo_Vettore" runat="server" Height="58px" Width="100%">
                                            </asp:ListBox>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <div class="clear">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="tabs-c4" class="cento" style="padding: 10px; display:none">
                        <asp:UpdatePanel ID="UpdatePanel_MezzoTrasporto" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:Panel ID="Pannello_Mezzo" runat="server" CssClass="box100 boxColore" Style="padding: 5px;">
                                    <div>
                                        <asp:Label ID="Label18" runat="server" Style="min-width: 240px; float: left">Targa:</asp:Label>
                                        <asp:TextBox ID="Txt_Targa" runat="server" CssClass="txtUI_120" Style="float: left"></asp:TextBox>
                                        <asp:CheckBox ID="Chk_ParcoMacchine" runat="server" Style="float: left" Text="Gestisci Parco Macchine"
                                            AutoPostBack="True" CssClass="change_per_load Nascosto"></asp:CheckBox>
                                        <asp:DropDownList ID="Cmb_Targa" runat="server" Style="float: left" Visible="False"
                                            AutoPostBack="True" CssClass="change_per_load">
                                        </asp:DropDownList>
                                        <div class="clear">
                                        </div>
                                        <asp:Label ID="Label25" runat="server" Style="min-width: 240px; float: left">Peso Tara Kg:</asp:Label>
                                        <asp:TextBox ID="Txt_PesoTara" runat="server" Style="float: left" CssClass="txtUI_120"></asp:TextBox>
                                        <div class="clear">
                                        </div>
                                        <asp:Label ID="Label20" runat="server" Style="min-width: 240px; float: left">Num. Immatricolazione:</asp:Label>
                                        <asp:TextBox ID="Txt_Immatr" runat="server" Style="float: left" CssClass="txtUI_120"></asp:TextBox>
                                        <div class="clear">
                                        </div>
                                        <asp:Label ID="Label22" runat="server" Style="min-width: 240px; float: left">Num. Immatricolazione Rimorchio:</asp:Label>
                                        <asp:TextBox ID="Txt_ImmatrRimorchio" runat="server" Style="float: left" CssClass="txtUI_120"></asp:TextBox>
                                        <div class="clear">
                                        </div>
                                        <asp:Label ID="Label23" runat="server" Style="min-width: 240px; float: left">Num. Autorizzazione Trasporto:</asp:Label>
                                        <asp:TextBox ID="Txt_Autorizz" runat="server" Style="float: left" CssClass="txtUI_120"></asp:TextBox>
                                        <div class="clear">
                                        </div>
                                        <asp:Label ID="Label24" runat="server" Style="min-width: 240px; float: left">Data Rilascio Autorizzazzione:</asp:Label>
                                        <asp:TextBox ID="Txt_DataAutorizz" runat="server" Style="float: left" CssClass="txtUI_120 datepicker"></asp:TextBox>
                                        <div class="clear">
                                        </div>
                                    </div>
                                </asp:Panel>
                                <div class="clear">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div id="tabs-2" class="cento" style="padding: 10px">
                <asp:UpdatePanel ID="upTabs_2" runat="server">
                    <ContentTemplate>
                        <div id="intestazioneDett" style="padding: 10px; width: 100%">
                            <asp:Panel ID="PulsantieraDettagli" runat="server" CssClass="boxColore" Style="width: 25%;
                                float: left; padding: 10px">
                                <asp:ImageButton runat="server" ImageUrl="../AB_Immagini/Icone32/Nuovo.ico"
                                    ID="ID_Prodotto" ToolTip="Carica un Prodotto nella Documento" />
                             <%--   <asp:ImageButton runat="server" ImageUrl="../AB_Immagini/Icone32/Ope_Zootecniche.ico"
                                    ID="ID_Zootecnico" ToolTip="Carica una consistenza zootecnica nell documento" />--%>
                                <asp:ImageButton runat="server" ImageUrl="../AB_Immagini/Icone32/Clip32.ico" ID="ID_Allegati"
                                    ToolTip="Allega D.D.T." />
                                <asp:ImageButton runat="server" ImageUrl="../AB_Immagini/Icone32/Cancella.bmp" ID="ID_Cancella"
                                    ToolTip="Rimuovi tutti i dettagli inseriti in tabella" />
                                <%--<asp:ImageButton runat="server" ImageUrl="../AB_Immagini/Icone32/Stampa.ico" ID="ID_Stampa"
                                    ToolTip="Stampa il documento" />--%>
                            </asp:Panel>
                            <asp:Panel ID="Pannello_Peso" runat="server" CssClass="boxColore" Style=" float:right;
                                width: 65%">
                                <asp:Label ID="Lbl_Peso" runat="server">Peso Netto Kg:</asp:Label>
                                <asp:TextBox ID="Txt_PesoNetto" runat="server" CssClass="txtUI_120">0</asp:TextBox>
                                <asp:Button ID="Btn_RicavaPesoNetto" runat="server" Text="Calcola"></asp:Button>
                                <asp:Label ID="Lbl_CalcoloPeso" runat="server">Ricava il Peso Netto totale da tutti i dettagli inseriti nella tabella</asp:Label>
                            </asp:Panel>
                            <div class="clear">
                            </div>
                        </div>
                        <asp:Label ID="Lbl_Prodotti" runat="server" CssClass="titoloBox">&nbsp;Elenco Prodotti:</asp:Label>
                        <asp:TextBox ID="TxtQueryStringProdotto" runat="server" Height="1px" Width="1px"
                            Style="display: none"></asp:TextBox>
                        <input id="InsProdotto" size="9" type="hidden" name="InsProdotto" runat="server" />
                        <input id="ValiditaInizioSportello" size="9" type="hidden" name="ValiditaInizioSportello" runat="server" />
                        <input id="ValiditaFineSportello" size="9" type="hidden" name="ValiditaFineSportello" runat="server" />
                        <asp:Panel ID="Pannello_Prodotti" runat="server" Height="242px" Style="overflow: scroll">
                            <asp:DataGrid ID="DataGrid_Prodotti" runat="server" Width="800px" AutoGenerateColumns="False"
                                CellPadding="5" CssClass="ui-widget-content">
                                <AlternatingItemStyle></AlternatingItemStyle>
                                <ItemStyle Height="25px"></ItemStyle>
                                <HeaderStyle CssClass="ui-widget-header" />
                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
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
                                    <asp:ButtonColumn Text="&lt;img src='../AB_Immagini/icone16/cE.ico' border='0'&gt;"
                                        HeaderText="Mod" CommandName="Modifica">
                                        <HeaderStyle HorizontalAlign="Center" Width="30px" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    </asp:ButtonColumn>
                                    <asp:ButtonColumn Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                        HeaderText="Canc" CommandName="Cancella">
                                        <HeaderStyle HorizontalAlign="Center" Width="30px" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    </asp:ButtonColumn>
                                </Columns>
                            </asp:DataGrid>
                        </asp:Panel>
                        <div class="clear">
                        </div>
                        <div id="DetpannelliGiu">
                            <asp:Panel ID="Pannello_IVA" runat="server" CssClass="boxColore" Style="width: 44%;
                                float: left">
                                <asp:Label ID="lblriva" runat="server" CssClass="titoloBox">&nbsp;Riepilogo IVA:</asp:Label>
                                <asp:DataGrid ID="DataGrid_IVA" runat="server" AutoGenerateColumns="False" CellPadding="5"
                                    CssClass="ui-widget-content">
                                    <AlternatingItemStyle></AlternatingItemStyle>
                                    <HeaderStyle CssClass="ui-widget-header" />
                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
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
                            <asp:Panel ID="Pannello_Riepilogo" runat="server" Style="width: 50%; float: right;
                                padding: 5px" CssClass="boxColore">
                                <div>
                                    <asp:Label ID="Label31" runat="server" CssClass="titoloBox">Riepilogo del Documento:</asp:Label>
                                </div>
                                <div>
                                    <asp:Label ID="Txt_TipoSconto" runat="server">Modalità di applicazione della variazione del prezzo:</asp:Label>
                                    <asp:DropDownList ID="Cmb_TipoSconto" runat="server">
                                    </asp:DropDownList>
                                </div>
                                <div class="clear">
                                </div>
                                <div>
                                    <asp:Label ID="Label32" runat="server" Style="min-width: 214px; float: left">Totale Imponibile Lordo:</asp:Label>
                                    <asp:TextBox ID="Txt_ImponibileLordo" runat="server" CssClass="txtUI_120" Style="float: left"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:Label ID="Label26" runat="server" Style="float: left">&euro;</asp:Label>
                                </div>
                                <div class="clear">
                                </div>
                                <div>
                                    <asp:Label ID="Label33" runat="server" Style="min-width: 214px; float: left">Totale Variazioni:</asp:Label>
                                    <asp:TextBox ID="Txt_Variazioni" runat="server" CssClass="txtUI_120" Style="float: left"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:Label ID="Label36" runat="server" Style="float: left">&euro;</asp:Label>
                                </div>
                                <div class="clear">
                                </div>
                                <div>
                                    <asp:Label ID="Label37" runat="server" Style="min-width: 214px; float: left">Totale Imponibile Netto:</asp:Label>
                                    <asp:TextBox ID="Txt_ImponibileNetto" runat="server" CssClass="txtUI_120" Style="float: left"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:Label ID="Label47" runat="server" Style="float: left">&euro;</asp:Label>
                                </div>
                                <div class="clear">
                                </div>
                                <div>
                                    <asp:Label ID="Label38" runat="server" Style="min-width: 214px; float: left">Totale IVA:</asp:Label>
                                    <asp:TextBox ID="Txt_IVA" runat="server" CssClass="txtUI_120" Style="float: left"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:Label ID="Label49" runat="server" Style="float: left">&euro;</asp:Label>
                                </div>
                                <div class="clear">
                                </div>
                                <hr size="1" style="width: 100%" />
                                <div>
                                    <asp:Label ID="Label39" runat="server" Style="min-width: 214px; float: left">Totale Importo Documento:</asp:Label>
                                    <asp:TextBox ID="Txt_Importo" runat="server" CssClass="txtUI_120" Style="float: left"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:Label ID="Label48" runat="server" Style="float: left">&euro;</asp:Label>
                                </div>
                            </asp:Panel>
                            <input id="Input_AllegaDoc" size="9" type="hidden" name="Input_AllegaDoc" runat="server" />
                        </div>
                        <div class="clear">
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <asp:TextBox ID="Txt_DatiPagamenti" runat="server" Visible="False" TextMode="MultiLine"></asp:TextBox>
        </div>
    </div>
    <!--------------------- DIALOG --->
    <div id="dialog" style="overflow: hidden">
        <div id="xiframe" style="height: 100%">
        </div>
    </div>
</asp:Content>
