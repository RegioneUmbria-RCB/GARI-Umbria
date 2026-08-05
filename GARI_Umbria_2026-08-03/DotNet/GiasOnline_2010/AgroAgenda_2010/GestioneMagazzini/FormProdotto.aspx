<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="../Master/Agenda.master"
    CodeBehind="FormProdotto.aspx.vb" Inherits="AgroAgenda_2010.FormProdotto" meta:resourcekey="PageResource1" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="cHead" ContentPlaceHolderID="ContentAgendaHead" runat="server">

    <script type="text/javascript" src="../Scripts/jquery-ui.combobox.js?<% =Application("GiasVersioneCorrente")%>"></script>

    <%--     <link rel="stylesheet" href="../Styles/bootstrap.min.css" media="screen" />
    <link rel="stylesheet" href="../Styles/bootstrap-select.min.css" media="screen" />--%>

    <%--    <cc1:bootstrap ID="bootstrap" runat="server" />
    <cc1:bootstrap_select ID="bS" runat="server" />--%>


    <%--    <script type="text/javascript">

        function DoPostBack_ControlliSiNo(key) {
            if (key == 'UdmMovimenti') {
                $("#<%=UdmMovimenti_SI_NO.ClientID %>").val("OK");
                $("#<%=ImgBtn_Inserisci_nel_DataGrid.ClientID %>").click();
            }
            if (key == 'CaricoInnesco') {
                $("#<%=Btn_CaricaInneschi.ClientID %>").click();

            }
            if (key == 'SommaProdotto') {
                $("#<%=Btn_SommaProdotto.ClientID %>").click();
            }
        }

        function FunctSalvaEsci() {
            window.returnValue = $('#<%=Txt_DaInviare.ClientID %>').value;
            window.close();
        }
        -
            function RisorseFun() {
                a = window.showModalDialog("../GestioneRisorse/GestioneRisorse_Edit.aspx?" + document.all("TxtProdotto").value, "", "dialogWidth:1000px;dialogHeight:700px;status:no; center:yes;edge:raised; help:no;")
            }
        function AnagrafeAnimale() {
            a = window.showModalDialog("../GestioneStalle/ZooAnimale_Edit.aspx?" + document.all("Txt_AnagrafeAnimale").value, "", "dialogWidth:1000px;dialogHeight:700px;status:no; center:yes;edge:raised; help:no;")
            document.all("Aggiunto_Animale").value = "1"
            document.getElementById("Form1").submit()
        }
    </script>--%>
    <style type="text/css">
        .valgn {
            vertical-align: top;
        }

        .titoloBox {
            background: #042649;
        }

        .ui-widget-header {
            background: #042649;
            border-color: #042649;
        }

        .ui-autocomplete-input {
            width: 400px;
        }

        .Testo_10_White_Bold {
            color: #FFFFFF;
            font-weight: bold;
            font-style: normal;
            text-decoration: none;
        }

        .Testo_08_Blue {
            margin-bottom: 0px;
        }

        .testo_08_nero {
        }

        .Testo_08_Nero_Bold {
            margin-bottom: 0px;
        }

        .Testo_08_Blue_Bold {
        }

        #Risorse {
            height: 29px;
            width: 32px;
        }

        .testo_08_verde {
        }

        .boxColore {
            padding: 10px;
        }

        #pnl_fornitore input {
            vertical-align: top;
        }
    </style>
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <asp:UpdatePanel ID="update_si_no" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="UdmMovimenti_SI_NO" runat="server" Value="0" />

        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upDati" runat="server">
        <ContentTemplate>
            <!-- Usati per memorizzare informazioni non usate in interfaccia, ma che non devono essere cancellate in modifica -->
            <asp:HiddenField runat="server" ID="hf_RifEsterno" />
            <asp:HiddenField runat="server" ID="hf_RifEsterno_2" />

            <!---<input id="btnAggiornaDebug" type="button" onclick="aspnetForm.submit()" />-->
            <div style="overflow: auto; width: 96%; min-width: 920px; margin-right: 2%; margin-left: 2%;">
                <div>
                    <div class="clear" style="display: none">
                    </div>
                    <asp:Panel ID="Pannello_DocLight" runat="server" Width="100%" Visible="false">
                        <div id="Pnl_DocLight" class="boxColore">
                            <div>
                                <asp:Label ID="LABEL10" runat="server" CssClass="titoloBox" Text="<%$ Resources: AgronicaAgenda_2010,DatiDocumento %>">Dati Documento</asp:Label>
                            </div>
                            <div style="float: left; margin-left: 10px;">
                                <asp:Label ID="Label31" Width="150px" runat="server" ForeColor="Black" meta:resourcekey="Label31" Text="Numero Documento *:"></asp:Label>
                                <asp:TextBox ID="Txt_NumeroDoc" runat="server" CssClass="txtUI"></asp:TextBox>
                                <asp:Label ID="Label34" Width="100px" runat="server" meta:resourcekey="Label34" Text="(solo numeri)">(solo numeri)</asp:Label>
                            </div>
                            <div style="float: left; margin-left: 10px;" id="pnl_fornitore">
                                <asp:Label ID="Label25" runat="server" ForeColor="Black" CssClass="valgn" meta:resourcekey="Label25" Text="Fornitore *:"></asp:Label>
                                <asp:DropDownList ID="Cmb_Fornitore" runat="server" CssClass="myCombo"
                                    AutoPostBack="false">
                                </asp:DropDownList>
                                &nbsp;&nbsp; &nbsp; &nbsp; 
                                <%--<asp:ImageButton ID="ImgBtn_NuovoFornitore" runat="server" ImageUrl="../AB_Immagini/icone24/btnnuovofornitore.png"
                                         ToolTip="Crea una nuovo fornitore" />   --%>
                                <%--<img  id="ImgBtn_NuovoFornitore"  src="../AB_Immagini/icone24/btnnuovofornitore.png"
                                         alt="Crea un nuovo fornitore" onclick="NuovoContatto()" />  --%>
                                <asp:ImageButton ID="ImgBtn_NuovoFornitore" runat="server" ImageUrl="../AB_Immagini/icone24/btnnuovofornitore.png"
                                                 meta:resourcekey="ImgBtn_NuovoFornitore" ToolTip="Inserisci un nuovo fornitore"></asp:ImageButton>
                                <%--<asp:TextBox ID="TxtContatto" runat="server" Style="display: none"></asp:TextBox>--%>
                                <input id="InsFornitore" size="9" type="hidden" name="InsFornitore" runat="server" />
                                <asp:ImageButton ID="ImgBtn_CaricaFornitori" runat="server" ImageUrl="../AB_Immagini/icone32/Lente.ico" Height="1px" Width="1px"
                                    visible="true"></asp:ImageButton>
                            </div>
                            <div class="clear" style="height: 0px">
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>
                    <asp:Panel ID="Pannello_Movimento" runat="server" Width="100%">
                        <div class="boxColore">
                            <div>
                                <asp:Label ID="LABEL1" runat="server" CssClass="titoloBox" Text="<%$ Resources: AgronicaAgenda_2010,Movimento %>">Movimento</asp:Label>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="width: 100%;">
                                <div style="float: left; min-width: 140px">
                                    <asp:Label ID="lbl_data" runat="server" Width="50px" ForeColor="Black" meta:resourcekey="lbl_data" Text="Data *:"></asp:Label>
                                    <script type="text/javascript">
                                        $(function () {
                                            $("#<%= Txt_DataMovimento.ClientID %>").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
                                        });

                                        $("#<%=Txt_DataMovimento.ClientID %>").change(function () {
                                            $("#<%=BTN_ChangeData.ClientID %>").click();
                                        });

                                    </script>
                                    <asp:Button ID="BTN_ChangeData" runat="server" Text="Button" Style="display: none"
                                        meta:resourcekey="BTN_ChangeDataResource1" />
                                    <asp:TextBox ID="Txt_DataMovimento" runat="server" CssClass="txtUI" Width="80px"></asp:TextBox>
                                </div>
                                <div style="float: left; width: 90px; min-width: 70px; margin-left: 10px;">
                                    <asp:Label ID="lbl_ora" runat="server" meta:resourcekey="lbl_ora" Text="Ora:"></asp:Label>
                                    <asp:TextBox ID="Txt_Ora" runat="server" CssClass="txtHour txtUI" Width="43px"></asp:TextBox>
                                </div>
                                <div style="float: left; width: 488px; min-width: 420px; margin-left: 10px;">
                                    <asp:Label ID="lbl_causale" runat="server" meta:resourcekey="lbl_causale" Text="Causale:"></asp:Label>
                                    <asp:DropDownList ID="cmb_Causale" runat="server" Width="413px" CssClass="txtUI"
                                        AutoPostBack="True">
                                    </asp:DropDownList>
                                    <asp:TextBox ID="Txt_PendenzaIniziale" runat="server" Visible="False" Width="10px"></asp:TextBox>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <asp:Label ID="LblNote" Width="90px" runat="server" meta:resourcekey="LblNote" Text="Rif.Doc./Note:"></asp:Label>
                                    <asp:TextBox ID="TextBoxNote" runat="server" CssClass="txtUI"></asp:TextBox>
                                </div>
                                <div class="clear" style="height: 0px">
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>
                    <asp:Panel ID="Pannello_Magazzini" runat="server" Width="100%">
                        <div class="boxColore">
                            <div>
                                <asp:Label ID="lbl_magazzini" runat="server" CssClass="titoloBox" meta:resourcekey="lbl_magazzini" Text="Magazzini Movimentati"></asp:Label>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="min-width: 950px; width: 100%;">
                                <div style="float: left; width: 54%">
                                    <div style="float: left">
                                        <asp:Label ID="lbl_MagProvenienza" runat="server" ForeColor="Black" meta:resourcekey="lbl_MagProvenienza" Text="Magazzino di Provenienza*:"></asp:Label>
                                    </div>
                                    <div style="float: left; margin-left: 10px;">
                                        <asp:DropDownList ID="Cmb_Provenienza" runat="server" Width="426px" CssClass="txtUI"
                                            AutoPostBack="True">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div style="float: left; width: 24%; margin-left: 10px;">
                                    <div style="float: left;">
                                        <asp:Label ID="lbl_giacenza1_mag" runat="server" CssClass="Testo_08_verde" Text="<%$ Resources: AgronicaAgenda_2010,Giacenza %>">Giacenza</asp:Label>
                                    </div>
                                    <div style="float: left; margin-left: 10px;">
                                        <asp:TextBox ID="Txt_Giacenza_Provenienza" runat="server" Width="210px"
                                            CssClass="txtUI" ReadOnly="True"></asp:TextBox>
                                    </div>
                                </div>
                                <div style="float: left; width: 102px; margin-left: 10px;">
                                    <asp:CheckBox ID="Chk_MovimentoMag" runat="server" Width="102px" CssClass="testo_08_nero"
                                                  meta:resourcekey="Chk_MovimentoMag" Text="Mov Magazz" Enabled="False"></asp:CheckBox>
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                            <div style="min-width: 950px; width: 100%;">
                                <div style="float: left; width: 54%">
                                    <div style="float: left;">
                                        <asp:Label ID="lbl_MagDestinazione" runat="server" ForeColor="Black" meta:resourcekey="lbl_MagDestinazione" Text="Magazzino di Destinazione *:"></asp:Label>
                                    </div>
                                    <div style="float: left; margin-left: 10px;">
                                        <asp:DropDownList ID="Cmb_Destinazione" runat="server" Width="426px" CssClass="txtUI"
                                            AutoPostBack="True">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div style="float: left; width: 24%; min-width: 150px; margin-left: 10px;">
                                    <div style="float: left;">
                                        <asp:Label ID="lbl_giacenza2_mag" runat="server" CssClass="Testo_08_verde" Text="<%$ Resources: AgronicaAgenda_2010,Giacenza %>">Giacenza</asp:Label>
                                    </div>
                                    <div style="float: left; margin-left: 10px;">
                                        <asp:TextBox ID="Txt_Giacenza_Destinazione" runat="server" Width="210px"
                                            CssClass="txtUI" ReadOnly="True"></asp:TextBox>
                                    </div>
                                </div>
                                <div style="float: left; width: 102px; margin-left: 10px;">
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                            <asp:Panel ID="denuncia" runat="server" Visible="false" Width="100%">
                                <asp:Label ID="lbl_NumeroDenuncia" runat="server" Visible="True" Width="184px">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroDenuncia %>" runat="server"></asp:Localize>
                                </asp:Label>
                                <asp:TextBox ID="Txt_NumeroDenuncia" runat="server" Visible="True" Width="216px"
                                    BackColor="#FFFFFF" CssClass="txtUI"></asp:TextBox>
                                <asp:Label ID="lbl_DataDenuncia" runat="server" Visible="True" Width="120px">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDenuncia %>" runat="server"></asp:Localize>
                                </asp:Label>
                                <script type="text/javascript">
                                    $(function () {
                                        $("#<%= Txt_DataDenuncia.ClientID %>").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
                                    });

                                    $("#<%=Txt_DataDenuncia.ClientID %>").change(function () {
                                        $("#<%=BTN_ChangeDataDenuncia.ClientID %>").click();
                                    });

                                </script>
                                <asp:Button ID="BTN_ChangeDataDenuncia" runat="server" Text="Button" Style="display: none"
                                    meta:resourcekey="BTN_ChangeDataResource1" />
                                <asp:TextBox ID="Txt_DataDenuncia" runat="server" Visible="True" Width="86px" BackColor="#FFFFFF"
                                    CssClass="txtUI"></asp:TextBox>
                            </asp:Panel>
                            <div class="clear" style="height: 0px">
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>
                    <asp:Panel ID="Pannello_Prodotto" runat="server" Width="100%">
                        <div class="boxColore">
                            <div>
                                <asp:Label ID="lbl_prodotto" runat="server" CssClass="titoloBox" meta:resourcekey="lbl_prodotto" Text="Prodotto Movimentato"></asp:Label>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="width: 100%;">
                                <div style="float: left; width: 18%;">
                                    <asp:Label ID="lbl_categoria" runat="server" meta:resourcekey="lbl_categoria" Text="Categoria Prodotto *:"></asp:Label>
                                </div>
                                <div style="float: left; width: 52%;">
                                    <asp:DropDownList ID="cmb_Categoria" runat="server" Width="100%" CssClass="txtUI"
                                        AutoPostBack="True">
                                    </asp:DropDownList>

                                    <asp:Button ID="Btn_CaricaInneschi" runat="server" Text="Button" Style="display: none" />
                                    <asp:HiddenField ID="Trap_Cod" Value="0" runat="server" />
                                </div>
                                <%-- i18n Bottone con immagine contenente il testo "Nuovo Articolo" --%>
                                <div style="float: left; margin-left: 10px;">
                                    <%--         <asp:Label ID="Lbl_Risorse" runat="server" CssClass="Testo_08_Blue_Bold" Width="105px"
                                        Visible="false">Nuovo Prodotto Aziendale</asp:Label>
                                    <img id="Risorse" src="../AB_Immagini/icone32/prodotti.ico" style="width: 24px; height: 24px; cursor: pointer"
                                        runat="server" visible="false" onclick="RisorseFun()" />--%>
                                    <asp:ImageButton ID="ImgBtn_NuovoProdotto" runat="server" ImageUrl="../AB_Immagini/icone24/btnnuovoarticolo4.png"
                                        CssClass="floatSX" Visible="false" meta:resourcekey="ImgBtn_NuovoProdotto" ToolTip="Crea una nuova anagrafica prodotto" />
                                    <%--<div id="Risorse" class="btn btn-info full_w" onclick="RisorseFun();">                                    
                                    <span class="hidden-none margin-r">
                                        <span id="RisorseLbl">Crea nuovo prodotto</span>
                                    </span>
                                </div>--%>
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="float: left; width: 100%;">
                                <div style="float: left;">
                                    <div style="float: left; width: 80px;">
                                        <asp:Label ID="lbl_CercaProdotto" runat="server" Text="<%$ Resources: AgronicaAgenda_2010,Nome %>">Nome</asp:Label>
                                    </div>
                                    <div style="float: left;">
                                        <asp:TextBox ID="Txt_CercaProdotto" runat="server" CssClass="txtUI" Width="150px"></asp:TextBox>
                                    </div>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <div style="float: left; width: 83px;">
                                        <asp:Label ID="Lbl_CercaCodArticolo" runat="server" Visible="False" Text="<%$ Resources: AgronicaAgenda_2010,CodiceArticoloAbbr %>">Cod. Articolo</asp:Label>
                                    </div>
                                    <div style="float: left;">
                                        <asp:TextBox ID="Txt_CercaCodArticolo" Width="150px" runat="server" BackColor="#FFFFFF"
                                            CssClass="txtUI" Visible="False"></asp:TextBox>
                                    </div>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <div style="float: left; width: 80px;">
                                        <asp:Label ID="lbl_CercaLotto" runat="server" Visible="False" Text="<%$ Resources: AgronicaAgenda_2010,Lotto %>">Lotto</asp:Label>
                                    </div>
                                    <div style="float: left;">
                                        <asp:TextBox ID="Txt_CercaLotto" Width="150px" runat="server" BackColor="#FFFFFF"
                                            CssClass="txtUI" Visible="False"></asp:TextBox>
                                    </div>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <div style="float: left; width: 90px;">
                                        <asp:Label ID="Lbl_PUARegolamenti" runat="server" Visible="False" Text="<%$ Resources: AgronicaAgenda_2010,Regolamento %>">Regolamento</asp:Label>
                                    </div>
                                    <div style="float: left;">
                                        <asp:DropDownList ID="cmb_PUARegolamenti" runat="server" Width="100%" CssClass="txtUI"
                                            AutoPostBack="false">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div style="float: left; width: 18%; margin-left: 10px;">
                                    <%-- i18n Bottone contenente un'immagine con testo "Ricerca" --%>
                                    <asp:Label ID="Lbl_Cerca" runat="server" CssClass="floatSX" Width="50px"></asp:Label>
                                    <asp:ImageButton ID="ImgBtn_CercaProdotti" runat="server" ImageUrl="../AB_Immagini/icone24/btnricerca.png"
                                        CssClass="floatSX" meta:resourcekey="ImgBtn_CercaProdotti" ToolTip="Carica gli articoli"
                                        margin-left="10px" />
                                    <asp:Label ID="Txt_NumProdotti" runat="server" CssClass="Testo_08_Rosso_Bold">&nbsp;</asp:Label>
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="width: 100%;">
                                <div style="float: left; width: 18%;">
                                    <asp:Label ID="lbl_descrizione" runat="server" Width="160px" meta:resourcekey="lbl_descrizione" Text="Descrizione Prodotto *:"></asp:Label>
                                </div>
                                <div style="float: left; width: 52%;">
                                    <asp:DropDownList ID="cmb_Prodotti" runat="server" AutoPostBack="True" CssClass="txtUI"
                                        Width="100%">
                                    </asp:DropDownList>
                                    <asp:TextBox ID="Txt_BeniStrumentali" runat="server" BackColor="#FFFFFF" CssClass="txtUI"
                                        Visible="False" Width="100%"></asp:TextBox>
                                    <div>
                                        <asp:Label ID="info_sul_prodotto" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <asp:TextBox ID="TxtProdotto" runat="server" Style="display: none"></asp:TextBox>
                                    <%--                                    <asp:ImageButton runat="server" ID="BtnInfo_fito" ImageUrl="../AB_Immagini/icone24/profitosan.png" alt="Consulta il Profitosan"
                                        Height="24px" Width="28px" Visible="false"></asp:ImageButton>--%>
                                    <asp:ImageButton runat="server" ID="BtnInfo_fito" ImageUrl="../AB_Immagini/icone24/btnprofitosan4.png" Visible="false" Text="<%$ Resources: AgronicaAgenda_2010, ConsultaProfitosan %>"></asp:ImageButton>
                                    <asp:ImageButton runat="server" ID="BtnInfo_Concime" ImageUrl="../AB_Immagini/icone32/informazioni.png" Height="24px" Width="28px" Visible="false" Text="<%$ Resources: AgronicaAgenda_2010, VisualizzaDettagliProdotto %>"></asp:ImageButton>
                                    <cc1:PopUpInformativi_Fertilizzante ID="PopUpInformativi_Fertilizzante" runat="server"
                                        meta:resourcekey="PopUpInformativi_FertilizzanteResource1" />
                                    <asp:Panel ID="dialogPupUpInformativiFertilizzante" runat="server" meta:resourcekey="dialogPupUpInformativiFertilizzanteResource1">
                                    </asp:Panel>
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            
                            <div id="DettagliFertilizzante" style="width: 100%" >
                                <div style="float: left; width: 18%;">&nbsp;</div>
                                <div style="float: left; width: 52%;">
                                    <%-- i18n: Simboli chimici non tradurre --%>
                                    <div id="bloccoN" style="float: left;">
                                        <div style="float: left; width: 45px;">
                                            <asp:Label ID="lbl_N" runat="server" Visible="True">N:</asp:Label>
                                        </div>
                                        <div style="float: left;">
                                            <asp:TextBox ID="Txt_N" runat="server" CssClass="txtUI" style="width: 60px"></asp:TextBox>
                                        </div>
                                    </div>                                    

                                    <div id="bloccoP2O5" style="float: left; margin-left: 10%;">
                                        <div style="float: left; width: 45px;">
                                            <asp:Label ID="lbl_P2O5" runat="server" Visible="True">P2O5:</asp:Label>
                                        </div>
                                        <div style="float: left;">
                                            <asp:TextBox ID="Txt_P2O5" runat="server" CssClass="txtUI" style="width: 60px"></asp:TextBox>
                                        </div>
                                    </div>
                                
                                    <div id="bloccoK2O" style="float: left; margin-left: 10%;">
                                        <div style="float: left; width: 45px;">
                                            <asp:Label ID="lbl_K2O" runat="server" Visible="True">K2O:</asp:Label>
                                        </div>
                                        <div style="float: left;">
                                            <asp:TextBox ID="Txt_K2O" runat="server" CssClass="txtUI" style="width: 60px"></asp:TextBox>
                                        </div>
                                    </div>
                                    
                                    <div id="bloccoCu" style="float: left; margin-left: 10%;">
                                        <div style="float: left; width: 45px;">
                                            <asp:Label ID="lbl_Cu" runat="server" Visible="True">Cu:</asp:Label>
                                        </div>
                                        <div style="float: left;">
                                            <asp:TextBox ID="Txt_Cu" runat="server" CssClass="txtUI" style="width: 60px"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div class="clear"></div>

                            <div style="width: 100%;">
                                <div style="float: left; width: 18%;">
                                    <asp:Label ID="lbl_LottoInterno" runat="server" Width="168px" meta:resourcekey="lbl_LottoInterno" Text="Lotto Impianto *:"></asp:Label>
                                </div>
                                <div style="float: left; width: 52%;">
                                    <asp:DropDownList ID="cmb_Lotto" runat="server" AutoPostBack="True" CssClass="txtUI"
                                        Enabled="False" Width="100%">
                                    </asp:DropDownList>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <asp:CheckBox ID="Chk_LottoImpianto" runat="server" AutoPostBack="True" CssClass="testo_08_nero"
                                        Enabled="False" meta:resourcekey="Chk_LottoImpianto" Text="Aggrega i lotti impianto" Width="220px" />
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="width: 100%;">
                                <div style="float: left; width: 18%;">
                                    <asp:Label ID="Lbl_Ordine" runat="server" Width="104px" meta:resourcekey="Lbl_Ordine" Text="Ordine:"></asp:Label>
                                </div>
                                <div style="float: left; width: 52%;">
                                    <asp:DropDownList ID="Cmb_Ordini" runat="server" AutoPostBack="True" CssClass="txtUI"
                                        Enabled="False" Width="100%">
                                    </asp:DropDownList>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <asp:CheckBox ID="Chk_Contatto" runat="server" AutoPostBack="True" CssClass="testo_08_nero"
                                        Enabled="False" meta:resourcekey="Chk_Contatto" Text="Carica ordini associati al contatto" Width="220px" />
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="width: 100%;">
                                <div style="float: left; width: 18%;">
                                    <asp:Label ID="lbl_LottoAccettazione" runat="server" Width="136px" meta:resourcekey="lbl_LottoAccettazione" Text="Lotto Prodotto:"></asp:Label>
                                </div>
                                <div style="float: left; width: 52%;">
                                    <asp:DropDownList ID="cmb_LottoAccettazione" runat="server" AutoPostBack="True" CssClass="txtUI"
                                        Enabled="False" Width="100%">
                                    </asp:DropDownList>
                                    <asp:TextBox ID="Txt_LottoAccettazione" runat="server" BackColor="#FFFFFF" CssClass="txtUI"
                                        Visible="False" Width="100%"></asp:TextBox>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="width: 100%;">
                                <div style="float: left; width: 18%;">
                                    <asp:Label ID="lbl_calibri" runat="server" Width="152px" Text="<%$ Resources: AgronicaAgenda_2010, ParametroQualitativo %>"></asp:Label> *:
                                </div>
                                <div style="float: left; width: 52%;">
                                    <asp:DropDownList ID="cmb_Calibro" runat="server" AutoPostBack="True" CssClass="txtUI"
                                        Enabled="False" Width="100%">
                                    </asp:DropDownList>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <asp:CheckBox ID="Chk_ParametroQualitativo" runat="server" AutoPostBack="True" CssClass="testo_08_nero"
                                        Enabled="False" meta:resourcekey="Chk_ParametroQualitativo" Text="Aggrega i Parametri Qualitativi" Width="220px" />
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="width: 100%;">
                                <div style="float: left; width: 18%;">
                                    <asp:Label ID="lbl_udm" runat="server" Width="119px" meta:resourcekey="lbl_udm" Text="Unità di Misura *:"></asp:Label>
                                </div>
                                <div style="float: left; width: 52%;">
                                    <asp:DropDownList ID="cmb_Udm" runat="server" AutoPostBack="True" CssClass="txtUI"
                                        Enabled="False" Width="100%">
                                    </asp:DropDownList>
                                </div>
                                <div style="float: left; margin-left: 10px;">
                                    <asp:Label ID="Lbl_DoseEtichetta" runat="server" Visible="False" Text="<%$ Resources: AgronicaAgenda_2010, DoseEtichetta %>"></asp:Label>
                                    <asp:TextBox ID="Txt_DoseEtichetta" runat="server" CssClass="txtUI"
                                        ReadOnly="True" Visible="False" Width="108px"></asp:TextBox>
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                            <div class="clear" style="height: 0px">
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>
                    <asp:Panel ID="Pannello_Dettaglio" runat="server" Width="100%">
                        <div class="boxColore">
                            <div>
                                <asp:Label ID="lbl_dettaglio" runat="server" CssClass="titoloBox" meta:resourcekey="lbl_dettaglio" Text="Dettagli del Prodotto"></asp:Label>
                            </div>
                            <div class="clear">
                            </div>
                            <div style="width: 100%;">
                                <div style="float: left; width: 30%;">
                                    <asp:Label ID="lbl_qta" runat="server" Width="72px" ForeColor="Black" meta:resourcekey="lbl_qta" Text="Quantità *:"></asp:Label>
                                    <asp:TextBox ID="Txt_Qta_Extra" runat="server" CssClass="txtUI"
                                        ReadOnly="True" Visible="False" Width="96px"></asp:TextBox>
                                    <asp:TextBox ID="Txt_Quantita" runat="server" BackColor="#FFFFFF" CssClass="txtUI"
                                        Width="100px"></asp:TextBox>
                                </div>
                                <div style="float: left; width: 28%; margin-left: 10px;">
                                    <asp:RadioButton ID="Opt_PrezzoUnitario" runat="server" Checked="True" GroupName="Euro"
                                        Text=" " Width="24px" />
                                    <asp:Label ID="lbl_prezzo" runat="server" Width="120px" meta:resourcekey="lbl_prezzo" Text="Prezzo Unitario:"></asp:Label>
                                    <asp:TextBox ID="Txt_PrezzoUnitario" runat="server" BackColor="#FFFFFF" CssClass="txtUI"
                                        Width="100px"></asp:TextBox>
                                    <asp:Label ID="lbl_euro" runat="server" Width="6px">&euro;</asp:Label>
                                </div>
                                <div style="float: left; width: 28%; margin-left: 10px; display: none">
                                    <asp:Label ID="lbl_PrezzoNetto" runat="server" Width="126px" meta:resourcekey="lbl_PrezzoNetto" Text="Prezzo Unit Netto"></asp:Label>
                                    <asp:TextBox ID="Txt_PrezzoUnitario_Netto" runat="server" CssClass="txtUI"
                                        ReadOnly="True" Width="100px"></asp:TextBox>
                                    <asp:Label ID="Label4" runat="server" Width="6px">&euro;</asp:Label>
                                </div>
                                <div style="float: left;">
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                            <asp:Panel ID="Pannello_Economico" runat="server" Width="100%">
                                <div class="boxColore">
                                    <div style="width: 100%;">
                                        <asp:Label ID="Label15" runat="server" CssClass="titoloBox" Width="144px" meta:resourcekey="Label15" Text="Dettagli Economici"></asp:Label>
                                    </div>
                                    <div class="clear">
                                    </div>
                                    <div style="float: left; width: 30%;">
                                        <div style="float: left; width: 40%;">
                                            <asp:RadioButton ID="Opt_Sconto" runat="server" GroupName="Variazione" Text=" " Width="24px" />
                                            <asp:Label ID="lbl_sconto" runat="server" meta:resourcekey="lbl_sconto" Text="Sconto:"></asp:Label>
                                        </div>
                                        <div style="float: left; width: 20%;">
                                            <asp:TextBox ID="Txt_ScontoPercentuale" runat="server" BackColor="#FFFFFF" CssClass="txtUI"
                                                Width="30px"></asp:TextBox>
                                            <asp:Label ID="Label24" runat="server" Width="20px">%</asp:Label>
                                        </div>
                                        <div style="float: left; width: 30%;">
                                            <asp:TextBox ID="Txt_Sconto_Calcolato" runat="server" CssClass="txtUI"
                                                ReadOnly="True" Width="70px"></asp:TextBox>
                                            <asp:Label ID="Label30" runat="server">&euro;</asp:Label>
                                        </div>
                                    </div>
                                    <div style="float: left; width: 30%; margin-left: 10px;">
                                        <div style="float: left; width: 50%;">
                                            <asp:RadioButton ID="Opt_Imponibile" runat="server" GroupName="Euro" Text=" " Width="24px" />
                                            <asp:Label ID="LABEL6" runat="server" Width="104px" meta:resourcekey="LABEL6" Text="Imponibile:"></asp:Label>
                                        </div>
                                        <div style="float: left; width: 40%;">
                                            <asp:TextBox ID="Txt_Imponibile" runat="server" BackColor="#FFFFFF" CssClass="txtUI"
                                                Width="100px"></asp:TextBox>
                                            <asp:Label ID="Label11" runat="server">&euro;</asp:Label>
                                        </div>
                                    </div>
                                    <div style="float: left; width: 30%; margin-left: 10px;">
                                        <asp:Label ID="LABEL29" runat="server" Width="118px" meta:resourcekey="LABEL29" Text="Imponibile Netto:"></asp:Label>
                                        <asp:TextBox ID="Txt_Imponibile_Netto" runat="server" CssClass="txtUI"
                                            ReadOnly="True" Width="100px"></asp:TextBox>
                                        <asp:Label ID="Label28" runat="server">&euro;</asp:Label>
                                    </div>
                                    <div class="clear" style="height: 10px;">
                                    </div>
                                    <div style="float: left; width: 30%;">
                                        <div style="float: left; width: 20%;">
                                            <asp:Label ID="LABEL12" runat="server" Width="32px" meta:resourcekey="LABEL12" Text="IVA applicata"></asp:Label>
                                        </div>
                                        <div style="float: left; width: 40%;">
                                            <asp:DropDownList ID="Cmb_IVA" runat="server" CssClass="txtUI" Width="113px">
                                            </asp:DropDownList>
                                        </div>
                                        <div style="float: left; width: 30%;">
                                            <asp:TextBox ID="Txt_IVA" runat="server" CssClass="txtUI" ReadOnly="True"
                                                Width="70px"></asp:TextBox>
                                            <asp:Label ID="Label7" runat="server">&euro;</asp:Label>
                                        </div>
                                    </div>
                                    <div style="float: left; width: 30%; margin-left: 10px;">
                                        <div style="float: left; width: 50%;">
                                            <asp:CheckBox ID="Chk_IVAmanuale" runat="server" AutoPostBack="True" CssClass="testo_08_nero"
                                                          meta:resourcekey="Chk_IVAmanuale" Text="Arrotonda l'IVA manualmente" />
                                        </div>
                                        <div style="float: left; width: 40%;">
                                            <asp:TextBox ID="Txt_IVAmanuale" runat="server" CssClass="txtUI"
                                                Enabled="False" Width="100px"></asp:TextBox>
                                            <asp:Label ID="Label26" runat="server" Width="16px">&euro;</asp:Label>
                                        </div>
                                    </div>
                                    <div style="float: left; width: 30%; display: none">
                                        <div style="float: left; width: 40%;">
                                            <asp:RadioButton ID="Opt_Maggiorazione" runat="server" GroupName="Variazione" Text=" "
                                                Width="24px" />
                                            <asp:Label ID="lbl_magg" runat="server" meta:resourcekey="lbl_magg" Text="Maggiorazione:"></asp:Label>
                                        </div>
                                        <div style="float: left; width: 20%;">
                                            <asp:TextBox ID="Txt_MaggiorazionePercentuale" runat="server" BackColor="#FFFFFF"
                                                CssClass="txtUI" Width="30px"></asp:TextBox>
                                            <asp:Label ID="Label17" runat="server" Width="20px">%</asp:Label>
                                        </div>
                                        <div style="float: left; width: 30%;">
                                            <asp:TextBox ID="Txt_Maggiorazione_Calcolato" runat="server"
                                                CssClass="txtUI" ReadOnly="True" Width="70px">
                                            </asp:TextBox>
                                            <asp:Label ID="Label9" runat="server">&euro;</asp:Label>
                                        </div>
                                    </div>
                                    <div style="float: left; width: 30%; margin-left: 10px;">
                                        <div style="float: left; width: 50%;">
                                            <asp:RadioButton ID="Opt_Importo" runat="server" GroupName="Euro" Text=" " Width="24px" />
                                            <asp:Label ID="LABEL5" runat="server" Width="111px" meta:resourcekey="LABEL5" Text="Importo Complessivo:"></asp:Label>
                                        </div>
                                        <div style="float: left; width: 40%;">
                                            <asp:TextBox ID="Txt_Importo" runat="server" BackColor="#FFFFFF" CssClass="txtUI"
                                                Width="100px"></asp:TextBox>
                                            <asp:Label ID="Label27" runat="server">&euro;</asp:Label>
                                        </div>
                                    </div>
                                    <div class="clear" style="height: 10px;">
                                    </div>
                                    <div style="float: left; width: 15%; margin-left: 10px;">
                                        <asp:Button ID="Btn_CalcolaImporti" runat="server" CssClass="testo_08_nero" Text="Calcola Importi"
                                            Width="120px" />
                                    </div>
                                    <div style="width: 65%; float: left; margin-left: 10px;">
                                        <asp:Label ID="Label23" runat="server" CssClass="testo_08_verde" meta:resourcekey="Label23">
                                            Dopo aver inserito lo Sconto o la Maggiorazione, l&#39;IVA e dopo aver selezionato una delle 3 modalità: 
                                            Prezzo Unitario / Imponibile / Importo con il relativo valore, premere &#39;Calcola Importi&#39;
                                        </asp:Label>
                                    </div>
                                    <div class="clear" style="height: 10px;">
                                    </div>
                                    <div style="width: 100%; display: none">
                                        <asp:Label ID="Label16" runat="server" CssClass="titoloBox" Width="208px" meta:resourcekey="Label16" Text="Imputazione Piano dei Conti"></asp:Label>
                                    </div>
                                    <div class="clear" style="display: none">
                                    </div>
                                    <div style="float: left; width: 20%; display: none">
                                        <asp:Label ID="LABEL18" runat="server" Width="112px" meta:resourcekey="LABEL18" Text="Anno contabile:"></asp:Label>
                                        <asp:DropDownList ID="Cmb_AnnoContabile" runat="server" AutoPostBack="True" CssClass="txtUI"
                                            Width="88px">
                                        </asp:DropDownList>
                                    </div>
                                    <div style="float: left; width: 75%; margin-left: 10px; display: none">
                                        <asp:CheckBox ID="Chk_Conti" runat="server" AutoPostBack="True" CssClass="testo_08_nero"
                                                      meta:resourcekey="Chk_Conti" Text="Visualizza solo i conti direttamente imputabili a " Width="317px" />
                                        <asp:DropDownList ID="Cmb_ContiContatto" runat="server" CssClass="txtUI" Visible="False"
                                            Width="435px">
                                        </asp:DropDownList>
                                        <asp:Label ID="LABEL19" runat="server" Width="146px" meta:resourcekey="LABEL19" Text="Conto Da Imputare:"></asp:Label>
                                        <asp:DropDownList ID="Cmb_Conti" runat="server" CssClass="txtUI" Width="458px">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </asp:Panel>
                    <div class="clear">
                    </div>
                    <div>
                        <div style="float: left;">
                            <asp:Panel ID="PANEL2" runat="server" Visible="False">
                                <asp:Label ID="Lbl_SalvaEsci" runat="server" Width="106px" CssClass="Testo_08_rosso_Bold" meta:resourcekey="Lbl_SalvaEsci" Text="Esci e torna al documento"></asp:Label>
                                <img id="SalvaEsci" onclick="FunctSalvaEsci()" name="SalvaEsci" alt="SALVA" src="../AB_Immagini/Icone32/btnsalva.png"
                                    width="40" runat="server">
                                <asp:ImageButton ID="IMAGEBUTTON2" runat="server" Width="32px" ImageUrl="../AB_Immagini/Icone32/ValidazioneSI.ico"></asp:ImageButton>
                                <asp:Label ID="LABEL33" runat="server" Width="58px" CssClass="Testo_08_rosso_Bold" Text="<%$ Resources: AgronicaAgenda_2010,Applica %>">Applica</asp:Label>
                            </asp:Panel>
                        </div>
                        <%-- i18n I due seguenti div sono visualizzati? --%>
                        <div style="float: left;">
                            <asp:Panel ID="PANEL1" runat="server" Visible="False">
                                <asp:Label ID="LABEL32" runat="server" Width="222px" CssClass="Testo_08_rosso_Bold">Inserisci i dettagli nel Documento</asp:Label>
                                <asp:ImageButton ID="IMAGEBUTTON1" runat="server" Width="32px" ImageUrl="../AB_Immagini/Icone32/ValidazioneSI.ico"></asp:ImageButton>
                            </asp:Panel>
                        </div>
                        <div style="float: left;">
                            <asp:Panel ID="Pannello_InsDocumento" runat="server" Visible="false">
                                <asp:Label ID="lbl_InsDocumento" runat="server" Width="228px" CssClass="Testo_08_rosso_Bold">Inserisci i dettagli nel Documento</asp:Label>
                                <asp:ImageButton ID="ImgBtnInsDocumento" runat="server" Width="32px" ImageUrl="../AB_Immagini/Icone32/ValidazioneSI.ico"></asp:ImageButton>
                            </asp:Panel>
                        </div>
                        <asp:UpdatePanel ID="update_salva" runat="server">
                            <ContentTemplate>
                                <div style="float: left;">
                                    <asp:Panel ID="Pannello_Salvataggio" runat="server" Visible="False">
                                        <%-- i18n I pulsanti seguenti sono immagini con le scritte "salva ed esci" e "salva e nuovo" --%>
                                        <%--<asp:Label ID="Lbl_SalvaMagazzino" runat="server" Width="222px" CssClass="Testo_08_rosso_Bold">Salva il carico</asp:Label>--%>
                                        <asp:ImageButton ID="ImgBtnSalvaTutto" runat="server" ImageUrl="../AB_Immagini/Icone24/btn_salva_esci.png" ToolTip="SALVA"></asp:ImageButton>
                                        &nbsp;&nbsp;&nbsp;
                                        <asp:ImageButton ID="ImgBtnSalvaNuovo" runat="server" ImageUrl="../AB_Immagini/Icone24/btn_salva_nuovo.png" ToolTip="SALVA"></asp:ImageButton>

                                    </asp:Panel>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="float: left;">
                            <asp:Panel ID="Pannello_InsGriglia" runat="server" Visible="False">
                                &nbsp;
                            <asp:ImageButton ID="ImgBtn_Inserisci_nel_DataGrid" runat="server" Width="32px" BackColor="White"
                                ImageUrl="../AB_Immagini/icone32/frecciadn.ico" meta:resourcekey="ImgBtn_Inserisci_nel_DataGrid" ToolTip="Premere il pulsante per inserire il carico nel documento di trasporto"></asp:ImageButton>
                                <asp:Label ID="Lbl_Inserisci_Carico" runat="server" Width="216px" BackColor="White"
                                    CssClass="Testo_08_Rosso_Bold" meta:resourcekey="Lbl_Inserisci_Carico" Text="Inserisci nel riepilogo"></asp:Label>
                            </asp:Panel>
                        </div>
                        <div class="clear">
                        </div>
                        <asp:Panel ID="Pannello_Prodotti" runat="server" Visible="False" Width="100%">
                            <div class="boxColore">
                                <asp:Label ID="Lbl_PannelloCarichi" runat="server" Visible="False"
                                    CssClass="titoloBox" meta:resourcekey="Lbl_PannelloCarichi" Text="Elenco Prodotti Selezionati"></asp:Label>
                                <div class="clear">
                                </div>
                                <asp:DataGrid ID="DataGrid_Prodotti" runat="server" Visible="False" Width="100%"
                                    CellPadding="5" CssClass="ui-widget-content" AutoGenerateColumns="False">
                                    <ItemStyle CssClass="Testo_08_Nero"></ItemStyle>
                                    <HeaderStyle Font-Bold="True" HorizontalAlign="Center" BackColor="#042649" CssClass="Testo_10_White_Bold"
                                        VerticalAlign="Middle" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False"></HeaderStyle>
                                    <Columns>
                                        <asp:BoundColumn Visible="False" DataField="Elem_Cod" HeaderText="Elem_Cod"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Pro_Cod" HeaderText="Pro_Cod"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Mat_Cod" HeaderText="Mat_Cod"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Cal_Cod" HeaderText="Cal_Cod"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Cod_Progetto" HeaderText="Cod_Progetto"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Lotto" HeaderText="Lotto"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Codice" HeaderText="Codice">
                                            <HeaderStyle Width="30px"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Piva_Destinazione" HeaderText="Piva_Destinazione"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="SaCod_Destinazione" HeaderText="SaCod_Destinazione"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Id_Destinazione" HeaderText="Id_Destinazione"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Destinazione" HeaderText="Destinazione"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Piva_Provenienza" HeaderText="Piva_Provenienza"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="SaCod_Provenienza" HeaderText="SaCod_Provenienza"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Id_Provenienza" HeaderText="Id_Provenienza"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Provenienza" HeaderText="Provenienza"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="Descrizione" HeaderText="<%$ Resources: AgronicaAgenda_2010,Descrizione %>">
                                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Udm_Cod" HeaderText="Udm_Cod"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="Udm_Des" HeaderText="<%$ Resources: AgronicaAgenda_2010,RisorsaUDM %>">
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Qta" HeaderText="<%$ Resources: AgronicaAgenda_2010,RisorsaQuantità %>">
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Prezzo_Unitario" HeaderText="<%$ Resources: AgronicaAgenda_2010,PrezzoUnitario %>">
                                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Chiave" HeaderText="Chiave"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Fase_Cod" HeaderText="Fase_Cod"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Prodotto" HeaderText="Prodotto"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Udm_Cod_Extra" HeaderText="Udm_Cod_Extra"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Qta_Extra" HeaderText="Qta_Extra"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Prezzo_Unitario_Netto" HeaderText="Prezzo Unitario Netto">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Cod_Variazione" HeaderText="Cod_Variazione">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Variazione_Perc" HeaderText="<%$ Resources: AgronicaAgenda_2010,ScontoMaggiorazionePercentuale %>">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Variazione" HeaderText="<%$ Resources: AgronicaAgenda_2010,ScontoMaggiorazione %>">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Cod_Iva" HeaderText="Cod_Iva">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Aliquota" HeaderText="<%$ Resources: AgronicaAgenda_2010,AliquotaIva %>">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Iva" HeaderText="<%$ Resources: AgronicaAgenda_2010,IVA %>">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Imponibile" HeaderText="<%$ Resources: AgronicaAgenda_2010,Imponibile %>">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Imponibile_Netto" HeaderText="<%$ Resources: AgronicaAgenda_2010,ImponibileNetto %>">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Prezzo_Effettivo" HeaderText="Prezzo_Effettivo"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="Anno" HeaderText="<%$ Resources: AgronicaAgenda_2010,Anno %>">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Ric_Cod" HeaderText="Ric_Cod"></asp:BoundColumn>
                                        <asp:BoundColumn Visible="False" DataField="Cod_Conto" HeaderText="Cod_Conto"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="Conto" HeaderText="<%$ Resources: AgronicaAgenda_2010,Conto %>">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:ButtonColumn Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                            HeaderText="<%$ Resources: AgronicaAgenda_2010,Elimina %>" CommandName="Cancella">
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:ButtonColumn>
                                    </Columns>
                                </asp:DataGrid>
                            </div>
                        </asp:Panel>
                        <div class="clear">
                        </div>
                        <asp:Panel ID="Pannello_Cancellazione" runat="server" Visible="False">
                            <div class="boxColore">
                                <asp:Label ID="LblCancellazione" runat="server" Width="298px" CssClass="Testo_12_Rosso_Bold" 
                                           meta:resourcekey="LblCancellazione" Text="Premere l'icona CESTINO per confermare la cancellazione"></asp:Label>
                                <asp:ImageButton ID="ImgBtnCancella" runat="server" Width="32px" ImageUrl="../AB_Immagini/Icone32/Cancella.bmp"></asp:ImageButton>
                            </div>
                        </asp:Panel>
                        <div class="clear">
                        </div>
                        <div>
                            <input id="SI_NO" size="1" type="hidden" name="SI_NO" runat="server" />
                            <asp:Label ID="lbl_giacenza_centro_old" runat="server" CssClass="Testo_08_verde"
                                Width="106px" Visible="False">Giacenza Centro</asp:Label>
                            <asp:TextBox ID="Txt_Giacenza_Destinazione_Tot_old" runat="server" ReadOnly="True"
                                CssClass="txtUI" Width="41px" Visible="False"></asp:TextBox>
                            <asp:TextBox ID="Txt_Giacenza_Provenienza_Tot_old" runat="server" ReadOnly="True"
                                CssClass="txtUI" Width="41px" Visible="False"></asp:TextBox>
                            <input id="Modifica_Data" size="7" type="hidden" name="Modifica_Data" runat="server" />
                            <input id="IndirizzoProfitosan" size="7" type="hidden" name="IndirizzoProfitosan"
                                runat="server" />
                            <asp:Panel ID="Pannello_Generale" runat="server" Width="100%">
                                <div class="boxColore">
                                    <asp:Panel ID="Pannello_Animale" runat="server" Width="100%">
                                        <asp:DropDownList ID="Cmb_IndirizzoProduttivo" runat="server" Width="378px" CssClass="txtUI"
                                            AutoPostBack="True" Enabled="False">
                                        </asp:DropDownList>
                                        <asp:Label ID="LABEL14" runat="server" Width="144px" meta:resourcekey="LABEL14" Text="Indirizzo Produttivo :"></asp:Label>
                                        <asp:Label ID="LABEL13" runat="server" Width="112px" meta:resourcekey="LABEL13" Text="Specie Animale :"></asp:Label>
                                        <a href="JavaScript:AnagrafeAnimale();">
                                            <img id="AnagrafeAnimale" border="0" name="Risorse" src="../AB_Immagini/Icone32/consistenze32.ico"
                                                width="32" runat="server"></a>
                                        <asp:DropDownList ID="Cmb_Animali" runat="server" Width="378px" CssClass="txtUI"
                                            AutoPostBack="True" Enabled="False">
                                        </asp:DropDownList>
                                        <asp:Label ID="Lbl_AnagrafeAnimale" runat="server" Width="172px" CssClass="Testo_08_Blue_Bold" meta:resourcekey="Lbl_AnagrafeAnimale" Text="Nuova Anagrafe Animale"></asp:Label>
                                        <asp:TextBox ID="Txt_AnagrafeAnimale" runat="server" Width="1px"></asp:TextBox>
                                        <input id="Aggiunto_Animale" name="Aggiunto_Animale" size="7" type="hidden" runat="server" />
                                        <asp:Label ID="LABEL21" runat="server" Width="100px" meta:resourcekey="LABEL21" Text="Matricola:"></asp:Label>
                                        <asp:Label ID="LABEL22" runat="server" Width="116px" meta:resourcekey="LABEL22" Text="Razza:"></asp:Label>
                                        <asp:DropDownList ID="Cmb_Matricola" runat="server" Visible="False" Width="378px"
                                            CssClass="txtUI" AutoPostBack="True" Enabled="False">
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="Cmb_Razza" runat="server" Visible="False" Width="378px" CssClass="txtUI"
                                            AutoPostBack="True" Enabled="False">
                                        </asp:DropDownList>
                                        <%-- i18n Vedi colonna Prezzo Unitario --%>
                                        <asp:Panel ID="Pannello_ZooAnimali" runat="server" Width="100%">
                                            <asp:DataGrid ID="DataGrid_ZooAnimali" runat="server" Width="840px" AutoGenerateColumns="False"
                                                CellSpacing="3" CellPadding="1">
                                                <ItemStyle Font-Size="XX-Small" CssClass="Testo_08_Nero"></ItemStyle>
                                                <HeaderStyle Font-Size="XX-Small" Font-Bold="True" HorizontalAlign="Center" CssClass="Testo_10_Nero_Bold"
                                                    VerticalAlign="Middle" BackColor="#87B6D9"></HeaderStyle>
                                                <Columns>
                                                    <asp:TemplateColumn>
                                                        <HeaderStyle Width="40px"></HeaderStyle>
                                                        <ItemStyle Font-Bold="True" HorizontalAlign="Center" ForeColor="Blue"></ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChkSeleziona" runat="server"></asp:CheckBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Cod_Progetto" HeaderText="Cod_Progetto"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Gen_Cod" HeaderText="Gen_Cod"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Spe_Cod" HeaderText="Spe_Cod"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Ipro_Cod" HeaderText="Ipro_Cod"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Raz_Cod" HeaderText="Raz_Cod"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Cat_Cod" HeaderText="Cat_Cod"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Spe_Des" HeaderText="Specie Animale"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Ipro_Des" HeaderText="Indirizzo Produttivo"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Raz_Des" HeaderText="<%$ Resources: AgronicaAgenda_2010,Razza %>"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Cat_Des" HeaderText="Categoria"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Matricola" HeaderText="<%$ Resources: AgronicaAgenda_2010,Matricola %>"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Nome" HeaderText="<%$ Resources: AgronicaAgenda_2010,Nome %>"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Collare" HeaderText="<%$ Resources: AgronicaAgenda_2010,Collare %>"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Nome_Aia" HeaderText="Nome_Aia"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Matricola_Aia" HeaderText="Matricola_Aia"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Dat_Nascita" HeaderText="Dat_Nascita"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Prov_Nascita" HeaderText="Prov_Nascita"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Stato_Nascita" HeaderText="Stato_Nascita"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="AUA_AZI_NASCITA" HeaderText="AUA_AZI_NASCITA"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="AUSL_AZI_NASCITA" HeaderText="AUSL_AZI_NASCITA"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Sesso" HeaderText="<%$ Resources: AgronicaAgenda_2010,Sesso %>"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Mat_Padre" HeaderText="Mat_Padre"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Mat_Madre" HeaderText="Mat_Madre"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Peso" HeaderText="<%$ Resources: AgronicaAgenda_2010,Peso %>"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Data_Pesa" HeaderText="Data_Pesa"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Metodo_Produzione" HeaderText="Metodo_Produzione"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Metodo_Produzione_Des" HeaderText="<%$ Resources: AgronicaAgenda_2010,MetodoDiProduzione %>"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Conversione_Inizio" HeaderText="Conversione_Inizio"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Conversione_Fine" HeaderText="Conversione_Fine"></asp:BoundColumn>
                                                    <asp:TemplateColumn HeaderText="<%$ Resources: AgronicaAgenda_2010,QuantitàAbbr %>">
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="TxtQta" runat="server" Width="80px" BackColor="white" CssClass="txtUI"
                                                                ReadOnly="false"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Prezzo Unitario [&amp;#8364;]">
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="TxtPrezzo" runat="server" Width="80px" BackColor="white" CssClass="txtUI"
                                                                ReadOnly="false"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Qta_Reale" HeaderText="Qta_Reale"></asp:BoundColumn>
                                                    <asp:BoundColumn Visible="False" DataField="Prezzo_Reale" HeaderText="Prezzo_Reale"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </asp:Panel>
                                        <div class="clear">
                                        </div>
                                        <asp:Label ID="Label20" runat="server" Visible="False" CssClass="titoloBox">Elenco Consistenze Selezionati</asp:Label>
                                    </asp:Panel>
                                    <div class="clear">
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Label ID="Label2" runat="server" Width="80px" Visible="False"> </asp:Label>
                            <asp:Label ID="Label3" runat="server" Width="86px" Visible="False"></asp:Label>
                            <asp:TextBox ID="Txt_DaInviare" runat="server" Style="display: none"></asp:TextBox>
                            <asp:Button ID="Btn_SommaProdotto" runat="server" Text="Button" Style="display: none" />
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                </div>
                <!--------------------- DIALOG --->
                <div id="dialog" style="overflow: hidden">
                    <div id="xiframe" style="height: 100%">
                    </div>
                </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">

        function DoPostBack_ControlliSiNo(key) {
            if (key == 'UdmMovimenti') {
                $("#<%=UdmMovimenti_SI_NO.ClientID %>").val("OK");
                $("#<%=ImgBtn_Inserisci_nel_DataGrid.ClientID %>").click();
            }
            if (key == 'CaricoInnesco') {
                $("#<%=Btn_CaricaInneschi.ClientID %>").click();

            }
            if (key == 'SommaProdotto') {
                $("#<%=Btn_SommaProdotto.ClientID %>").click();
            }
        }

        function FunctSalvaEsci() {
            window.returnValue = $('#<%=Txt_DaInviare.ClientID %>').value;
            window.close();
        }
        function RisorseFun() {
            //window.showModalDialog("../GestioneRisorse/GestioneRisorse_Edit.aspx?" + $("#ctl00_ContentAgendaContenuti_TxtProdotto").val(), "", "dialogWidth:1000px;dialogHeight:700px;status:no; center:yes;edge:raised; help:no;")
            window.open("../../Giasonline/GestioneRisorse/GestioneRisorse_Edit.aspx?" + $("#ctl00_ContentAgendaContenuti_TxtProdotto").val())
        }
        //function NuovoContatto() {
        //     window.open("../GestioneContatti/Contatto.aspx?" + $("#ctl00_ContentAgendaContenuti_TxtContatto").val())
        //}
        function AnagrafeAnimale() {
            a = window.showModalDialog("../GestioneStalle/ZooAnimale_Edit.aspx?" + document.all("Txt_AnagrafeAnimale").value, "", "dialogWidth:1000px;dialogHeight:700px;status:no; center:yes;edge:raised; help:no;")
            document.all("Aggiunto_Animale").value = "1"
            document.getElementById("Form1").submit()
        }

        function Contatto_gestisciValore_esci() {
            $("#dialog").dialog("close");
        }

        function Contatto_gestisciValore(valore) {

            $("#dialog").dialog("close");
            $.logThis("impostato il valore su InsFornitore");
            <%--$("#<%=Txt_Cerca_Mittente2.ClientID() %>").val(valore);--%>
            $("#<%=InsFornitore.ClientID() %>").val(valore);

            clickButtonContatti();
        }

        function clickButtonContatti() {

            $.logThis("Chiamata a ImgBtn_CaricaFornitori");
            $("#<%=ImgBtn_CaricaFornitori.ClientID %>").click();

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

        <%--function clickButtonContatti() {

                                            $.logThis("Chiamata a Img_btn_CercaMittente2");
                                            $("#<%=Img_btn_CercaMittente2.ClientID %>").click();

                                        }--%>

    </script>

<script>
    $(document).ready(function () {
        $('.myCombo').combobox3();

        ImpostaLinguaDefaultDatePicker();
    });

    // i18n Codice non necessario?
    function ImpostaLinguaDefaultDatePicker() {
        //Google translate breaks datepicker
        $('.ui-datepicker').addClass('notranslate');

        // detect browser language but not a decent one 
        var userLang = navigator.language || navigator.userLanguage; 

        if (userLang === "pt-BR") {
            $.datepicker.setDefaults($.datepicker.regional['pt-BR']);
        } else {
            $.datepicker.setDefaults($.datepicker.regional['it']);
        }
    }

</script>

</asp:Content>

