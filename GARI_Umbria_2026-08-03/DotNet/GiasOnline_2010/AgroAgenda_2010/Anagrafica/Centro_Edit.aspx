<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="Centro_Edit.aspx.vb" Inherits="AgroAgenda_2010.Centro_Edit" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        #TxtVerificaPiva {
            font-size: 16px;
        }
    </style>

    <script src="<%= srv_gm %>" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row">

        <div class="col-lg-12 col-md-12">
            <div class="row">
                <div class="col-lg-10 col-md-10" style="padding: 10px 15px; line-height: 1.6;">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Riferimenti %>" runat="server">Riferimenti</asp:Localize>
                    <b>
                        <asp:Label ID="LblRiferimenti" runat="server" ClientIDMode="Static">
                        </asp:Label></b>



                </div>
                <div class="col-lg-2 col-md-2 text-right">
                    <%--<div class="btn btn-success" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">--%>

                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>

                    <div class="btn btn-success" onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
                        <i class="fa fa-floppy-o"></i>
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                    </div>
                    <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                    <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown"
                        aria-haspopup="true" aria-expanded="false" style="margin-top: -20px !important;">
                        <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu" style="right: 0; left: auto !important;">
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(0); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                        </a></li>
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(1); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                        </a></li>
                    </ul>
                    <% End If%>
                    <asp:HiddenField ID="tipo_salva" runat="server" ClientIDMode="Static" />
                    <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static"
                        Style="display: none" />

                    <% End If%>
                </div>
            </div>
        </div>
        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <ul id="Ul1" class="nav nav-tabs" data-tabs="tabs">
                <li class="active" class="tab_dati_centro"><a href="#tab_dati_centro" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiCentro %>" runat="server">Dati Centro</asp:Localize>
                </a></li>
                <!--<li><a href="#tab_gerarchia" data-toggle="tab">Gerarchia</a></li>-->
                <li class="tab_dati_accessori"><a href="#tab_dati_accessori" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiAccessori %>" runat="server">Dati Accessori</asp:Localize>
                </a></li>
                <li class="tab_biologico"><a href="#tab_biologico" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Biologico %>" runat="server">Biologico</asp:Localize>
                </a></li>
            </ul>
            <div id="my-tab-content" class="tab-content">
                <div class="tab-pane active" id="tab_dati_centro">
                    <div class="jumbotron">

                        <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>

                        <div class="row" id="div_riepilogo_error">
                            <div class="col-lg-12 col-md-12">
                                <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CompilareISeguentiCampi %>" runat="server">I seguenti campi devono essere compilati:</asp:Localize></b>
                                <br />
                                <br />
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <ul id="div_riepilogo_error_elenco">
                                    <li class="voce_1"><asp:Localize meta:resourcekey="ObbligatorioDenominazione" runat="server">Il campo <b>Denominazione</b> è da compilare</asp:Localize></li>
                                    <li class="voce_2"><asp:Localize meta:resourcekey="ObbligatorioTipologia" runat="server">Il campo <b>Tipologia</b> è da selezionare</asp:Localize></li>
                                    <li class="voce_3"><asp:Localize meta:resourcekey="ObbligatorioVia" runat="server">Il campo <b>Via</b> è da compilare</asp:Localize></li>
                                    <li class="voce_4"><asp:Localize meta:resourcekey="ObbligatorioCAP" runat="server">Il campo <b>CAP</b> è da compilare</asp:Localize></li>
                                    <li class="voce_5"><asp:Localize meta:resourcekey="ObbligatorioProvincia" runat="server">Il campo <b>Provincia</b> è da selezionare</asp:Localize></li>
                                    <li class="voce_6"><asp:Localize meta:resourcekey="ObbligatorioComune" runat="server">Il campo <b>Comune</b> è da selezionare</asp:Localize></li>
                                </ul>
                            </div>
                        </div>

                        <% End If%>                        


                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Denominazione" for="TxtDenominazione">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Denominazione %>" runat="server">Denominazione</asp:Localize>
                                                *
                                            </span>
                                            <asp:TextBox ID="TxtDenominazione" runat="server" ClientIDMode="Static" CssClass="form-control required" aria-describedby="lbl_Denominazione"> </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Tipologia" for="Cmb_Tipologia">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipologia %>" runat="server">Tipologia</asp:Localize>
                                                *
                                            </span>
                                            <asp:DropDownList ID="Cmb_Tipologia" runat="server" ClientIDMode="Static" CssClass="form-control required">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_TitoloPossesso" for="Cmb_TitoloPossesso"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TitoloDiPossesso %>" runat="server">Titolo di Possesso</asp:Localize></span>
                                            <asp:DropDownList ID="Cmb_TitoloPossesso" runat="server" ClientIDMode="Static" CssClass="form-control ">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>                        

                        <div class="row" style="display: none">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Piva" for="TxtPiva">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PartitaIVA %>" runat="server">Partita IVA</asp:Localize>
                                                *
                                            </span>
                                            <asp:TextBox ID="TxtPiva" runat="server" ClientIDMode="Static" CssClass="form-control" aria-describedby="lbl_Piva"
                                                > </asp:TextBox>
                                            <p id="TxtVerificaPiva">
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_SaCod" for="TxtSaCod">SA Cod *</span> <%-- i18n Struttura Aziendale --%>
                                            <asp:TextBox ID="TxtSaCod" runat="server" ClientIDMode="Static" CssClass="form-control" aria-describedby="lbl_SaCod"
                                                MaxLength="8"> </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <%--<div class="row">
                            <div class="radio-inline ">
                                <label>
                                    <img src="../AB_Immagini/icone24/Cooperativa24a.ico" />
                                    <asp:RadioButton ID="Opt_TipoConsorzio" runat="server" Text="Consorzio" GroupName="TipoImpresa">
                                    </asp:RadioButton>
                                </label>
                            </div>
                            <div class="radio-inline  ">
                                <label>
                                    <img src="../AB_Immagini/icone24/Cooperativa24b.ico" />
                                    <asp:RadioButton ID="Opt_TipoOP" runat="server" Text="OP" GroupName="TipoImpresa">
                                    </asp:RadioButton>
                                </label>
                            </div>
                            <div class="radio-inline  ">
                                <label>
                                    <img src="../AB_Immagini/icone24/Cooperativa24c.ico" />
                                    <asp:RadioButton ID="Opt_TipoCooperativa" runat="server" Text="Cooperativa" GroupName="TipoImpresa">
                                    </asp:RadioButton>
                                </label>
                            </div>
                            <div class="radio-inline  ">
                                <label>
                                    <img src="../AB_Immagini/icone24/x02_Impresa.png" />
                                    <asp:RadioButton ID="Opt_TipoImpresa" runat="server" Text="Impresa" GroupName="TipoImpresa"
                                        Checked="true"></asp:RadioButton>
                                </label>
                            </div>
                        </div>--%>
                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Indirizzo %>" runat="server">Indirizzo</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6 col-md-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_via" for="Txt_Via">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Via %>" runat="server">Via</asp:Localize>
                                                        *
                                                    </span>
                                                    <asp:TextBox ID="Txt_Via" runat="server" ClientIDMode="Static" CssClass="form-control required" aria-describedby="lbl_via"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_frazione" for="Txt_Frazione">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Frazione %>" runat="server">Frazione</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_Frazione" runat="server" ClientIDMode="Static" CssClass="form-control  " aria-describedby="lbl_frazione"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="div_prov_com">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_Prov" for="Cmb_Provincia">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provincia %>" runat="server">Provincia</asp:Localize>
                                                            *
                                                        </span>
                                                        <asp:DropDownList ID="Cmb_Provincia" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker required"
                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Prov">
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="Txt_ProCodIstat" runat="server" ClientIDMode="Static" CssClass="form-control " Style="display: none"> </asp:TextBox>
                                                        <asp:TextBox ID="Txt_ProvinciaSigla" runat="server" ClientIDMode="Static" CssClass="form-control " Style="display: none"> </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_Com" for="Cmb_Comune">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Comune %>" runat="server">Comune</asp:Localize>
                                                            *
                                                        </span>
                                                        <asp:DropDownList ID="Cmb_Comune" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker required"
                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Com">
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="Txt_ComCodIstat" runat="server" ClientIDMode="Static" CssClass="form-control " Style="display: none"> </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_cap" for="Txt_CAP">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CAP %>" runat="server">CAP</asp:Localize>
                                                        *
                                                    </span>
                                                    <asp:TextBox ID="Txt_CAP" runat="server" ClientIDMode="Static" CssClass="form-control  " aria-describedby="lbl_cap">

                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_stato" for="Txt_Stato">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Stato %>" runat="server">Stato</asp:Localize>
                                                    </span>
                                                    <%-- <asp:TextBox ID="Txt_Stato" runat="server" CssClass="form-control  " aria-describedby="lbl_stato"> </asp:TextBox>--%>
                                                    <asp:DropDownList name="cmb_Stato" ID="cmb_Stato" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_stato">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_note_ind" for="Txt_Note">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server">Note</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_Note" runat="server" ClientIDMode="Static" CssClass="form-control  " aria-describedby="lbl_note_ind"> </asp:TextBox>
                                                    <asp:TextBox ID="Txt_CodIndirizzo" runat="server" ClientIDMode="Static" CssClass="displaynone"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Geolocalizzazione %>" runat="server">Geolocalizzazione</asp:Localize>
                                        </h4>
                                    </div>

                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_lat" for="Txt_Latitude">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, LatitudineAbbr %>" runat="server">Lat.</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_Latitude" runat="server" ClientIDMode="Static" CssClass="form-control" aria-describedby="lbl_lat"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_lng" for="Txt_Longitude">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, LongitudineAbbr %>" runat="server">Long.</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_Longitude" runat="server" ClientIDMode="Static" CssClass="form-control" aria-describedby="lbl_lng"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-1 col-md-1 hidden-sm">
                                    </div>
                                    <div class="col-lg-3 col-md-3 col-sm-12 text-right">
                                        <div class="btn btn-default" onclick="ricercaIndirizzo()" id="btn_geolocalizza" runat="server" clientidmode="Static" style="margin-top: 0 !important">
                                            <%--<div class="btn btn-default" onclick="$('#<=ImgBtn_Geolacalizzazione.ClientID %>').click();">--%>
                                            <i class="fa fa-globe"></i>
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Geolocalizza %>" runat="server">Geolocalizza</asp:Localize>
                                        </div>
                                        <%--                                   <asp:ImageButton ID="ImgBtn_Geolacalizzazione" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                            Style="display: none" />--%>
                                    </div>


                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Rubrica %>" runat="server">Rubrica</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Rubrica_Numero" for="TxtRubrica_Numero">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Valore %>" runat="server">Valore</asp:Localize>
                                                        *
                                                    </span>
                                                    <asp:TextBox ID="TxtRubrica_Numero" runat="server" ClientIDMode="Static" CssClass="form-control" aria-describedby="lbl_Rubrica_Numero"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group" id="tipo_rubrica">
                                                    <span class="input-group-addon alert-info" id="lbl_Rubrica_Descrizione" for="Cmb_Rubrica_Descrizione">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipologia %>" runat="server">Tipologia</asp:Localize>
                                                        *
                                                    </span>
                                                    <%--<asp:TextBox ID="TxtRubrica_Descrizione" runat="server" CssClass="form-control" aria-describedby="lbl_Rubrica_Descrizione"> </asp:TextBox>--%>
                                                    <asp:DropDownList ID="Cmb_Rubrica_Descrizione" runat="server" ClientIDMode="Static" CssClass="form-control" AutoPostBack="false">
                                                        <asp:ListItem Value=""></asp:ListItem>
                                                        <asp:ListItem Value="Telefono" Text="<%$ Resources: AgronicaAgenda_2010, Telefono %>"></asp:ListItem>
                                                        <asp:ListItem Value="Cellulare" Text="<%$ Resources: AgronicaAgenda_2010, Cellulare %>"></asp:ListItem>
                                                        <asp:ListItem Value="Fax" Text="<%$ Resources: AgronicaAgenda_2010, Fax %>"></asp:ListItem>
                                                        <asp:ListItem Value="Email" Text="<%$ Resources: AgronicaAgenda_2010, Email %>"></asp:ListItem>
                                                        <asp:ListItem Value="Social" Text="<%$ Resources: AgronicaAgenda_2010, Social %>"></asp:ListItem>
                                                        <asp:ListItem Value="Web" Text="<%$ Resources: AgronicaAgenda_2010, Web %>"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                        <div class="form-horizontal text-right">
                                            <%If (OperazioneSuRubrica = "") Then%>
                                            <div class="btn btn-info btn_aggiungi_rubrica">
                                                <%--<div class="btn btn-info btn_aggiungi_rubrica" onclick="$('#<=ImgBtn_Rubrica_Ins.ClientID %>').click();">--%>
                                                <i class="fa fa-plus"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>

                                                <asp:ImageButton ID="ImgBtn_Rubrica_Ins" runat="server" ClientIDMode="Static" Style="display: none" />
                                            </div>
                                            <%End If%>
                                            <%If (OperazioneSuRubrica = "Modifica") Then%>
                                            <div class="btn btn-default btn_nuovo_rubrica" onclick="$('#<%=ImgBtn_Rubrica_Nuovo.ClientID %>').click();">
                                                <i class="fa fa-file"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nuovo %>" runat="server">Nuovo</asp:Localize>

                                                <asp:ImageButton ID="ImgBtn_Rubrica_Nuovo" runat="server" ClientIDMode="Static" Style="display: none" />
                                            </div>
                                            <div class="btn btn-success btn_modifica_rubrica" onclick="$('#<%=ImgBtn_Rubrica_Mod.ClientID %>').click();">
                                                <i class="fa fa-file-text"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Modifica %>" runat="server">Modifica</asp:Localize>

                                                <asp:ImageButton ID="ImgBtn_Rubrica_Mod" runat="server" ClientIDMode="Static" Style="display: none" />
                                            </div>
                                            <%End If%>
                                        </div>
                                        <%End If%>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="table-responsive" id="tabGerarchia">
                                            <div id="tabRubrica">
                                                <%--<asp:GridView ID="GridView_Rubrica" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered custom-table"
                                                UseAccessibleHeader="true">
                                                <Columns>
                                                    <asp:ButtonField Text="<img src='../AB_Immagini/icone16/ce.ico' border='0' class='img-modifica-rubrica'> "
                                                        HeaderText="Mod." CommandName="Modifica" meta:resourcekey="ButtonFieldResource1" />
                                                    <asp:ButtonField Text="<img src='../AB_Immagini/icone16/gomma16.ico' border='0' class='img-cancella-rubrica'>"
                                                        HeaderText="Canc." CommandName="Cancella" meta:resourcekey="ButtonFieldResource2" />

                                                    <asp:BoundField DataField="cod_rubrica" HeaderText="ID">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="descr" HeaderText="Descrizione">
                                                    </asp:BoundField>
                                                     <asp:BoundField DataField="Numero" HeaderText="Valore">
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>--%>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                        <div class="jumbotron" id="div-additional" style="display: none;">
                            <div class="row">
                                <div class="col-md-8">
                                    <div class="input-group">
                                        <span class="input-group-addon" id="spn_chk_centro">
                                            <asp:CheckBox ID="Chk_Centro" runat="server" ClientIDMode="Static" aria-label="..." />
                                        </span>
                                        <label aria-describedby="spn_chk_centro" id="lbl_chk_centro" class="form-control alert-info"
                                            for="<%=Chk_Centro.ClientID %>">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CreaCentroAziendaleAssociatoAdImpresa %>" runat="server">
                                                Crea un Centro Aziendale associato all'Impresa
                                            </asp:Localize>
                                        </label>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <small><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,InFaseDiCreazioneImpresaCreaCentroAziendale %>" runat="server">
                                        In fase di creazione di una nuova impresa, crea automaticamente anche un Centro Aziendale.
                                    </asp:Localize></small>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-8" style="margin-bottom: 10px">
                                    <div class="input-group">
                                        <span class="input-group-addon" id="spn_chk_magazzino">
                                            <asp:CheckBox ID="Chk_Magazzino" runat="server" ClientIDMode="Static" aria-label="..." />
                                        </span>
                                        <label aria-describedby="spn_chk_magazzino" id="lbl_chk_magazzino" class="form-control alert-info"
                                            for="<%=Chk_Magazzino.ClientID %>">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CreaMagazzinoAssociatoACentroAziendale %>" runat="server">
                                                Crea un Magazzino associato al Centro Aziendale
                                            </asp:Localize>
                                        </label>
                                    </div>
                                </div>
                                <div class="col-md-4" style="margin-bottom: 10px">
                                    <small><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,InFaseDiCreazioneCentroAziendaleCreaMagazzino %>" runat="server">
                                        In fase di creazione di un nuovo Centro Aziendale, crea automaticamente anche un Magazzino.
                                    </asp:Localize></small>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="tab_gerarchia">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CooperativeImpreseReferenti %>" runat="server">Cooperative / Imprese Referenti</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-9 col-md-9 col-sm-9">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_txtPivaPadre" for="txtPivaPadre">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TestoRicerca %>" runat="server">Testo Ricerca</asp:Localize>
                                                    </span>
                                                    <input type="text" class="form-control" id="txtPivaPadre" aria-describedby="lbl_txtPivaPadre" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-3 col-md-3 col-sm-3">
                                        <div class="btn btn-info btn_100" onclick="$('#<%=ImgBtn_Cerca_RagSoc.ClientID %>').click();">
                                            <i class="fa fa-search"></i>
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Cerca %>" runat="server">Cerca</asp:Localize>
                                        </div>
                                        <asp:ImageButton ID="ImgBtn_Cerca_RagSoc" runat="server" ClientIDMode="Static" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                            Style="display: none" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-9 col-md-9 col-sm-9">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon control-label alert-info" id="lbl_padre" for="Cmb_Imprese">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Referente %>" runat="server">Referente</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_Imprese" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker"
                                                        data-live-search="true" aria-describedby="lbl_padre">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                    <div class="col-lg-3 col-md-3 col-sm-3">
                                        <div class="btn btn-info btn_100" onclick="$('#<%=ImgBtn_Aggiungi_Padre.ClientID %>').click();">
                                            <i class="fa fa-plus"></i>
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                        </div>
                                        <asp:ImageButton ID="ImgBtn_Aggiungi_Padre" runat="server" ClientIDMode="Static" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                            Style="display: none" />
                                    </div>
                                    <%End If%>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="table-responsive">
                                            <asp:GridView ID="GridView_Padri" runat="server" ClientIDMode="Static" AutoGenerateColumns="False" CssClass="table table-bordered"
                                                UseAccessibleHeader="true">
                                                <Columns>
                                                    <asp:ButtonField ButtonType="Link" HeaderText="<%$ Resources: AgronicaAgenda_2010, CancellaAbbr %>" Text="<i class='fa fa-times'></i>"
                                                        CommandName="Elimina">
                                                        <HeaderStyle Font-Names="Verdana" />
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:ButtonField>
                                                    <asp:BoundField DataField="Piva" HeaderText="<%$ Resources: AgronicaAgenda_2010, PartitaIva %>"></asp:BoundField>
                                                    <asp:BoundField DataField="Rag_Soc" HeaderText="<%$ Resources: AgronicaAgenda_2010, RagioneSociale %>">
                                                        <ItemStyle HorizontalAlign="left"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Validita_Inizio" HeaderText="<%$ Resources: AgronicaAgenda_2010, Dal %>">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Validita_Fine" HeaderText="<%$ Resources: AgronicaAgenda_2010, Al %>">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="tab_dati_accessori">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-md-12 text-center">
                                <h4 style="color: #052747; text-transform: uppercase;"><asp:Localize meta:resourcekey="PeriodoAttivitàCentroAziendale" runat="server">Periodo Attività Centro Aziendale</asp:Localize></h4>
                            </div>
                            <div class="col-lg-4">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_validita_inizio" for="TxtValiditaInizio">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dal %>" runat="server">Dal</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="TxtValiditaInizio" runat="server" ClientIDMode="Static" CssClass="form-control datepicker"
                                        MaxLength="10"> </asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_validita_fine" for="TxtValiditaFine">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Al %>" runat="server">Al</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="TxtValiditaFine" runat="server" ClientIDMode="Static" CssClass="form-control datepicker"
                                        MaxLength="10"> </asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <ul class="no-bullet">
                                    <li><i class="fa fa-info-circle"></i><small>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, EstremiConsideratiGiorniValidi %>" runat="server">Gli estremi sono considerati entrambi giorni validi.</asp:Localize>
                                    </small></li>
                                    <li><i class="fa fa-info-circle"></i><small>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CaselleVuoteIndicanoValiditàNonLimitata %>" runat="server">Le caselle possono essere lasciate vuote per indicare validità non limitata.</asp:Localize>
                                    </small></li>
                                </ul>
                            </div>

                            <div class="col-lg-12">
                                <div>
                                    <i class="fa fa-exclamation-triangle"></i>
                                    <small>
                                        <b><asp:Localize meta:resourcekey="AZIENDAValidità" runat="server">AZIENDA validità:</asp:Localize></b>
                                        <asp:Label ID="lbl_azienda_data_inizio" runat="server" ClientIDMode="Static" CssClass="txtUI" BorderStyle="None"></asp:Label>
                                        - 
                                        <asp:Label ID="lbl_azienda_data_fine" runat="server" ClientIDMode="Static" CssClass="txtUI" BorderStyle="None"></asp:Label>
                                    </small>
                                </div>
                            </div>

                        </div>

                        <div class="row" style="margin-top: 20px;">
                            <div class="col-lg-8 col-md-8 col-sm-12" id="CentroEsterno_Div" runat="server">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_CentroEsterno" for="Cmb_CentroEsterno">
                                                <asp:Localize 
                                                    Text="<%$ Resources: CentroEsterno %>" 
                                                    runat="server">Centro Esterno
                                                </asp:Localize>
                                            </span>
                                            <asp:DropDownList 
                                                ID="Cmb_CentroEsterno" 
                                                runat="server" 
                                                ClientIDMode="Static" 
                                                CssClass="form-control ">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiciAnagrafici %>" runat="server">Codici Anagrafici</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group" id="tipo_codici">
                                                            <span class="input-group-addon alert-info" id="lbl_Codice" for="CmbCodice">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server">Codice</asp:Localize>:
                                                            </span>
                                                            <asp:DropDownList ID="CmbCodice" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker"
                                                                data-live-search="true" aria-describedby="lbl_Codice">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_valorecod" for="TxtCodiceValore">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Valore %>" runat="server">Valore</asp:Localize>:
                                                            </span>
                                                            <asp:TextBox ID="TxtCodiceValore" runat="server" ClientIDMode="Static" CssClass="form-control" aria-describedby="lbl_valorecod">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_validita_inizio_codice" for="TxtValiditaInizioCodice">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dal %>" runat="server">Dal</asp:Localize>
                                                            </span>
                                                            <asp:TextBox ID="TxtValiditaInizioCodice" runat="server" ClientIDMode="Static" CssClass="form-control datepicker"
                                                                MaxLength="10">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_validita_fine_codice" for="TxtValiditaFineCodice">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Al %>" runat="server">Al</asp:Localize>
                                                            </span>
                                                            <asp:TextBox ID="TxtValiditaFineCodice" runat="server" ClientIDMode="Static" CssClass="form-control datepicker"
                                                                MaxLength="10">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                            <div class="col-lg-12 text-right">
                                                <div class="btn btn-info" id="btn_aggiungi_codice">
                                                    <%--<div class="btn btn-info" onclick="$('#<=ImgBtn_Aggiungi_Codice.ClientID %>').click();">--%>
                                                    <i class="fa fa-plus"></i>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                                </div>
                                                <asp:ImageButton ID="ImgBtn_Aggiungi_Codice" runat="server" ClientIDMode="Static" Style="display: none" />
                                            </div>
                                            <%End If %>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="table-responsive">
                                            <div id="tabCodici">
                                                <%--<asp:GridView ID="GridView_Codici" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered custom-table"
                                                UseAccessibleHeader="true">
                                                <Columns>
                                                    <asp:ButtonField ButtonType="Link" HeaderText="Canc." Text="<i class='fa fa-times'></i>"
                                                        CommandName="Elimina">
                                                        <HeaderStyle Font-Names="Verdana" />
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:ButtonField>
                                                    <asp:BoundField DataField="Contatore" HeaderText="Contatore">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Id_Cod" HeaderText="Codice">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Descrizione" HeaderText="Codice"></asp:BoundField>
                                                    <asp:BoundField DataField="Val_Cod" HeaderText="Valore">
                                                        <ItemStyle HorizontalAlign="left"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Validita_Inizio" HeaderText="Dal">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Validita_Fine" HeaderText="Al">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>--%>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

                <div class="tab-pane" id="tab_biologico">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="row">
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="row">
                                            <div class="col-lg-8 col-md-8 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_CodiceImpresa" for="TxtCodiceImpresa">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceOperatoreAbbr %>" runat="server">Cod. Operatore</asp:Localize>
                                                            </span>
                                                            <asp:TextBox ID="TxtCodiceImpresa" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                                MaxLength="50">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-4 col-md-4 col-sm-12" style="margin-top: 5px;">
                                                <asp:CheckBox ID="chkLast" runat="server" ClientIDMode="Static" aria-label="..." Style="float: left; padding-right: 15px;" />
                                                <label aria-describedby="spn_chk_contatto_pubblico" id="lbl_Last"
                                                    for="<%=chkLast.ClientID %>">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Automatico %>" runat="server">Automatico</asp:Localize>
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-6 col-sm-12" style="float: right;">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_TipoAttivita" for="CmbTipoAttivita">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipoAttività %>" runat="server">Tipo attività</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="CmbTipoAttivita" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                        data-live-search="true" aria-describedby="lbl_TipoAttivita">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_OrganismoControllo" for="CmbOrganismoControllo">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OrganismoDiControllo %>" runat="server">Organismo di Controllo (ODC):</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="CmbOrganismoControllo" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                        data-live-search="true" aria-describedby="lbl_OrganismoControllo">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OrientamentoTecnicoEconomico %>" runat="server">Orientamento Tecnico Economico (O.T.E.)</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12">
                                        <%--<div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon" id="lbl_OTE" for="CmbOTE">O.T.E.</span>
                                                    <asp:DropDownList ID="CmbOTE" runat="server" CssClass="form-control"
                                                        data-live-search="true" aria-describedby="lbl_OTE">
                                                    </asp:DropDownList>--%>
                                        <select id="cblOTE" class="selectpicker" multiple data-hide-disabled="true" data-live-search="true" runat="server" clientidmode="Static">
                                        </select>
                                        <%--    </div>
                                            </div>
                                        </div>--%>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="table-responsive">
                                            <asp:GridView ID="GridView_OTE" runat="server" ClientIDMode="Static" AutoGenerateColumns="False" CssClass="table table-bordered custom-table"
                                                UseAccessibleHeader="true">
                                                <Columns>
                                                    <asp:ButtonField ButtonType="Link" HeaderText="<%$ Resources: AgronicaAgenda_2010, CancellaAbbr %>" Text="<i class='fa fa-times'></i>"
                                                        CommandName="Elimina">
                                                        <HeaderStyle Font-Names="Verdana" />
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:ButtonField>
                                                    <asp:BoundField DataField="Contatore" HeaderText="<%$ Resources: AgronicaAgenda_2010, Contatore %>">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Id_Cod" HeaderText="Codice">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Descrizione" HeaderText="<%$ Resources: AgronicaAgenda_2010, Codice %>"></asp:BoundField>
                                                    <asp:BoundField DataField="Val_Cod" HeaderText="<%$ Resources: AgronicaAgenda_2010, Valore %>">
                                                        <ItemStyle HorizontalAlign="left"></ItemStyle>
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superfici %>" runat="server">Superfici</asp:Localize>
                                            [Ha]
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4 col-md-4  col-sm-4 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Sup_Totale" for="TxtSup_Totale">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Totale %>" runat="server">Totale</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="TxtSup_Totale" runat="server" ClientIDMode="Static" CssClass="form-control">
                                                    </asp:TextBox>
                                                </div>
                                                <span class="lbl_more_info"><asp:Localize meta:resourcekey="SommaParticelleCatastali" runat="server">(Somma delle particelle catastali)</asp:Localize></span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4  col-sm-4 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Sup_Tare" for="TxtSup_Tare">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tare %>" runat="server">Tare</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="TxtSup_Tare" runat="server" ClientIDMode="Static" CssClass="form-control">
                                                    </asp:TextBox>
                                                </div>
                                                <span class="lbl_more_info"><asp:Localize meta:resourcekey="DifferenzaFraSuperficieTotaleESau" runat="server">(Differenza fra Sup.Totale e SAU)</asp:Localize></span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4  col-sm-4 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Sup_SAU" for="TxtSup_SAU">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SAUTotale %>" runat="server">SAU Totale</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="TxtSup_SAU" runat="server" ClientIDMode="Static" CssClass="form-control">
                                                    </asp:TextBox>
                                                </div>
                                                <span class="lbl_more_info"><asp:Localize meta:resourcekey="SommaDegliAppezzamenti" runat="server">(Somma degli appezzamenti)</asp:Localize></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4 col-md-4  col-sm-4 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Sup_SAU_Convenzionale" for="txtSup_SAU_Convenzionale">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SAUConvenzionaleAbbr %>" runat="server">SAU Convenz.</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="txtSup_SAU_Convenzionale" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                        MaxLength="8">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4  col-sm-4 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Sup_SAU_Conversione" for="txtSup_SAU_Conversione">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SAUConversioneAbbr %>" runat="server">SAU Convers.</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="txtSup_SAU_Conversione" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                        MaxLength="8">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4  col-sm-4 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Sup_SAU_Biologico" for="txtSup_SAU_Biologico">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SAUBiologico %>" runat="server">SAU Biologico</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="txtSup_SAU_Biologico" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                        MaxLength="8">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4 col-md-4  col-sm-4 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Sup_Bosco" for="TxtSup_Bosco">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Bosco %>" runat="server">Bosco</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="TxtSup_Bosco" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                        MaxLength="8">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4  col-sm-4 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Sup_Prati" for="TxtSup_Prati">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prati %>" runat="server">Prati</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="TxtSup_Prati" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                        MaxLength="8">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>


            </div>
        </div>
    </div>
    <!-- VARIE -->
    <!-- contiene l'indice del tab selezionato, viene controllato lato server CalledFromClient_SetIndexTab -->
    <input type="hidden" id="clickedTabUI" runat="server" clientidmode="Static" />
    <!-- salva la chiamata alla funzione per il postback -->
    <input type="hidden" id="fooName_PostedBack" runat="server" clientidmode="Static" />
    <!-- parametri le funzione di postback -->
    <input type="hidden" id="fooName_Param_PostedBack" runat="server" clientidmode="Static" />
    <br />
    <input id="InizioCentro" name="InizioCentro" type="hidden" runat="server" clientidmode="Static" />
    <input id="FineCentro" name="FineCentro" type="hidden" runat="server" clientidmode="Static" />

    <input type="hidden" id="ATPrevalenteText" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript">
        var initRubrica = JSON.stringify(<%=jsRubrica%>);
        var initCodici = JSON.stringify(<%=jsCodici%>);
    </script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Centro_Edit.js") %>" ></script>
    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
