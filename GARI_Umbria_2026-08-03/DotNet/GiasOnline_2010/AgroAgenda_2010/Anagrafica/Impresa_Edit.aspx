<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="Impresa_Edit.aspx.vb" Inherits="AgroAgenda_2010.Impresa_Edit" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        #TxtVerificaPiva {
            font-size: 16px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row">
        <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impresa).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
        <div class="col-lg-12 text-right">
            <%--<div class="btn btn-success" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">--%>

            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>

            <div class="btn btn-success xonne-btn-primary" onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
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
            <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ClientIDMode="Static" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                Style="display: none" />
            <asp:ImageButton ID="ImgBtn_SalvaDistinta" runat="server" ClientIDMode="Static" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                Style="display: none" />

            <% End If%>
        </div>
        <%End If%>
        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <ul id="Ul1" class="nav nav-tabs" data-tabs="tabs">
                <li class="active"><a href="#tab_dati_azienda" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiAzienda %>" runat="server">Dati Azienda</asp:Localize>
                </a></li>
                <li><a href="#tab_gerarchia" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Gerarchia %>" runat="server">Gerarchia</asp:Localize>
                </a></li>
                <li><a href="#tab_dati_accessori" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiAccessori %>" runat="server">Dati Accessori</asp:Localize>
                </a></li>
            </ul>
            <div id="my-tab-content" class="tab-content">
                <div class="tab-pane active" id="tab_dati_azienda">
                    <div class="jumbotron">

                        <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>

                        <div class="row" id="div_riepilogo_error" style="margin-bottom: 25px;">
                            <div class="col-lg-12 col-md-12">
                                <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CompilareISeguentiCampi %>" runat="server">I seguenti campi devono essere compilati:</asp:Localize></b>
                                <br />
                                <br />
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <ul id="div_riepilogo_error_elenco">
                                    <li class="voce_1"><asp:Localize meta:resourcekey="ObbligatorioRagioneSociale" runat="server">Il campo <b>Ragione Sociale</b> è da compilare</asp:Localize></li>
                                    <li class="voce_2"><asp:Localize meta:resourcekey="ObbligatorioFormaGiuridica" runat="server">Il campo <b>Forma giuridica</b> è da selezionare</asp:Localize></li>
                                    <li class="voce_3"><asp:Localize meta:resourcekey="ObbligatorioPartitaIva" runat="server">Il campo <b>Partita IVA</b> è da compilare</asp:Localize></li>
                                    <li class="voce_4"><asp:Localize meta:resourcekey="ObbligatorioCFCUAA" runat="server">Il campo <b>CF / CUAA</b> è da compilare</asp:Localize></li>
                                    <li class="voce_5"><asp:Localize meta:resourcekey="ObbligatorioVia" runat="server">Il campo <b>Via</b> è da compilare</asp:Localize></li>
                                    <!--<li class="voce_6">Il campo <b>Provincia</b> è da selezionare</li>-->
                                    <li class="voce_7"><asp:Localize meta:resourcekey="ObbligatorioComune" runat="server">Il campo <b>Comune</b> è da selezionare</asp:Localize></li>
                                    <li class="voce_8"><asp:Localize meta:resourcekey="ObbligatorioCAP" runat="server">Il campo <b>CAP</b> è da compilare</asp:Localize></li>
                                </ul>
                            </div>
                        </div>

                        <% End If%>

                        <div class="row" style="margin-bottom: 10px">
                            <div class="col-lg-8 col-md-8 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required alert-info" id="lbl_RagSoc" for="TxtRagioneSociale">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RagioneSociale %>" runat="server">Ragione Sociale</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtRagioneSociale" runat="server" CssClass="form-control required" ClientIDMode="Static" aria-describedby="lbl_RagSoc">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <%--<span class="input-group-addon lbl_required" id="Span1" for="TxtRagioneSociale">Tipologia:</span>
                                       <asp:DropDownList ID="Cmb_Tipologia" runat="server" CssClass="form-control"
                                                        AutoPostBack="false">
                                                   <asp:ListItem Value="1">Consorzio</asp:ListItem>
                                                   <asp:ListItem Value="2">OP</asp:ListItem>
                                                   <asp:ListItem Value="3">Cooperativa</asp:ListItem>
                                                   <asp:ListItem Value="4">Impresa</asp:ListItem>
                                                </asp:DropDownList>
                                        </div>--%>
                                            <span class="input-group-addon alert-info" id="lbl_FormaGiuridica" for="Cmb_FormaGiuridica">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FormaGiuridica %>" runat="server">Forma giuridica</asp:Localize>
                                            </span>
                                            <asp:DropDownList ID="Cmb_FormaGiuridica" runat="server" CssClass="form-control required" ClientIDMode="Static">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-5 col-md-5 col-sm-11">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required alert-info" id="lbl_Piva" for="TxtPiva">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PartitaIVA %>" runat="server">Partita IVA</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtPiva"
                                                runat="server"
                                                CssClass="form-control required"
                                                aria-describedby="lbl_Piva"
                                                ClientIDMode="Static"
                                                MaxLength="25">
                                            </asp:TextBox>
                                            <p id="TxtVerificaPiva">
                                            </p>
                                        </div>
                                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TxtPiva"
                                                ForeColor="Red" SetFocusOnError="true" Display="Dynamic"
                                                ErrorMessage=" Non sono consentiti caratteri speciali" ID="rfvname"
                                                ValidationExpression="^[\sa-zA-Z0-9]*$">

                                            </asp:RegularExpressionValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-1 col-md-1 col-sm-1">
                                <div id="BtnPiva_Fittizia" class="btn btn-success mt-25" data-toggle="tooltip" title="<asp:Localize Text='<%$ Resources:GeneraPartitaIvaFittizia %>' runat='server'></asp:Localize>">
                                    <i class="fa fa-cog xonne-cog" aria-hidden="true"></i>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required alert-info" id="lbl_CF" for="TxtCodiceFiscale">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceFiscaleSigla %>" runat="server">CF</asp:Localize>
                                                / 
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceUnicoAziendaAgricolaSigla %>" runat="server">CUAA</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtCodiceFiscale" runat="server" CssClass="form-control required" ClientIDMode="Static"
                                                aria-describedby="lbl_CF" MaxLength="16">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--<div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_CUAA" for="txtCuaaCod">CUAA</span>
                                            <asp:TextBox ID="txtCuaaCod" runat="server" CssClass="form-control required"
                                                aria-describedby="lbl_CUAA" MaxLength="16">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>
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
                                                    <span class="input-group-addon lbl_required alert-info" id="lbl_via" for="Txt_Via">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Via %>" runat="server">Via</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_Via" runat="server" CssClass="form-control required" aria-describedby="lbl_via" ClientIDMode="Static">
                                                    </asp:TextBox>
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
                                                    <asp:TextBox ID="Txt_Frazione" runat="server" CssClass="form-control  " aria-describedby="lbl_frazione" ClientIDMode="Static">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="div_prov_com">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required alert-info" id="lbl_Prov" for="dll_Provincia">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provincia %>" runat="server">Provincia</asp:Localize>
                                                        </span>
                                                        <asp:DropDownList ID="dll_Provincia" runat="server" CssClass="form-control selectpicker required" ClientIDMode="Static"
                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Prov">
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="Txt_ProCodIstat" runat="server" CssClass="form-control " Style="display: none" ClientIDMode="Static">
                                                        </asp:TextBox>
                                                        <asp:TextBox ID="Txt_ProvinciaSigla" runat="server" CssClass="form-control " Style="display: none" ClientIDMode="Static">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required alert-info" id="lbl_Com" for="ddl_comune">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Comune %>" runat="server">Comune</asp:Localize>
                                                        </span>
                                                        <asp:DropDownList ID="ddl_comune" runat="server" CssClass="form-control selectpicker required" ClientIDMode="Static"
                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Com">
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="Txt_ComCodIstat" runat="server" CssClass="form-control " Style="display: none" ClientIDMode="Static">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon lbl_required alert-info" id="lbl_cap" for="Txt_CAP">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CAP %>" runat="server">CAP</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_CAP" name="Txt_CAP" runat="server" CssClass="form-control required" aria-describedby="lbl_cap" ClientIDMode="Static">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_stato" for="cmb_Stato">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Stato %>" runat="server">Stato</asp:Localize>
                                                    </span>
                                                    <%--<asp:TextBox ID="Txt_Stato" runat="server" CssClass="form-control  " aria-describedby="lbl_stato">
                                                    </asp:TextBox>--%>
                                                    <asp:DropDownList name="cmb_Stato" ID="cmb_Stato" runat="server" CssClass="form-control selectpicker" ClientIDMode="Static"
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
                                                    <asp:TextBox ID="Txt_Note" runat="server" CssClass="form-control  " aria-describedby="lbl_note_ind" ClientIDMode="Static">
                                                    </asp:TextBox>
                                                    <asp:TextBox ID="Txt_CodIndirizzo" runat="server" CssClass="displaynone" ClientIDMode="Static">
                                                    </asp:TextBox>
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
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Contatti %>" runat="server">Contatti</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <ul class="no-bullet">
                                            <li><i class="fa fa-info-circle"></i><small>
                                                <asp:Localize meta:resourcekey="InserireImpresaContatti" runat="server">Questa sezione permette di inserire l'impresa nei Contatti. La modifica del rapporto contabile è gestita nella sezione Contatti.</asp:Localize>
                                            </small></li>
                                            <li><i class="fa fa-info-circle"></i><small>
                                                    <asp:Localize meta:resourcekey="SelezionareRapportoContabilePerImpresa" runat="server"><b>E' obbligatorio</b> selezionare almeno un Rapporto Contabile da attribuire all'impresa.</asp:Localize>
                                            </small>
                                                <br />
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12">
                                        <label aria-describedby="spn_chk_contatto_pubblico" id="lbl_contatto_pubblico" for="<%=ChkVisibilita.ClientID %>">
                                            <asp:CheckBox ID="ChkVisibilita" runat="server" aria-label="..." Style="float: left; padding-right: 15px;" ClientIDMode="Static" />
                                            <asp:Localize meta:resourcekey="RendereImpresaContattoVisibileDaTutte" runat="server">Rendi l'impresa un Contatto visibile a tutte le imprese</asp:Localize>
                                        </label>
                                        <asp:Button ID="Btn_VerificaProgressivo" CssClass="btn btn-default" runat="server" ClientIDMode="Static"
                                            Text="Verifica Progressivo" Style="margin-left: 15px;"></asp:Button>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12" style="margin-bottom: 50px">
                                        <div class="table-responsive ">
                                            <asp:GridView ID="DataGrid_Contatti" runat="server" ClientIDMode="Static" AutoGenerateColumns="False" CssClass="table custom-table borderless"
                                                UseAccessibleHeader="true">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChkRapporto" runat="server" ClientIDMode="Static" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Piva" HeaderText="Piva">
                                                        <ItemStyle CssClass="non_mostrare" />
                                                        <HeaderStyle CssClass="non_mostrare" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod">
                                                        <ItemStyle CssClass="non_mostrare" />
                                                        <HeaderStyle CssClass="non_mostrare" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Cod_Rapporto" HeaderText="Cod_Rapporto">
                                                        <ItemStyle CssClass="non_mostrare" />
                                                        <HeaderStyle CssClass="non_mostrare" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Rapporto_Des" HeaderText="<%$ Resources: AgronicaAgenda_2010, RapportoContabile %>"></asp:BoundField>
                                                    <asp:TemplateField HeaderText="<%$ Resources: AgronicaAgenda_2010, Progressivo %>">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="TxtProgressivo" runat="server" ClientIDMode="Static" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="<%$ Resources: AgronicaAgenda_2010, Attività %>">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="TxtAttivita" runat="server" ClientIDMode="Static" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Codice">
                                                        <ItemStyle CssClass="non_mostrare" />
                                                        <HeaderStyle CssClass="non_mostrare" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="TxtCodRisUm" runat="server" ClientIDMode="Static" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="jumbotron" id="div-additional" style="display: none;">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="input-group">
                                    <label aria-describedby="spn_chk_centro" id="lbl_chk_centro" class="form-control"
                                        for="<%=Chk_Centro.ClientID %>">
                                        <asp:CheckBox ID="Chk_Centro" runat="server" ClientIDMode="Static" aria-label="..." Style="float: left; padding-right: 15px;" Checked="true" Enabled="false" />
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CreaCentroAziendaleAssociatoAdImpresa %>" runat="server">Crea un Centro Aziendale associato all'Impresa</asp:Localize>
                                    </label>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <i class="fa fa-info-circle"></i><small>
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InFaseDiCreazioneImpresaCreaCentroAziendale %>" runat="server">In fase di creazione di una nuova impresa, crea automaticamente anche un Centro Aziendale.</asp:Localize>
                                </small>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6" style="margin-bottom: 10px">
                                <div class="input-group">
                                    <label aria-describedby="spn_chk_magazzino" id="lbl_chk_magazzino" class="form-control"
                                        for="<%=Chk_Magazzino.ClientID %>">
                                        <asp:CheckBox ID="Chk_Magazzino" runat="server" ClientIDMode="Static" aria-label="..." Style="float: left; padding-right: 15px;" Checked="true" />
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CreaMagazzinoAssociatoACentroAziendale %>" runat="server">Crea un Magazzino associato al Centro Aziendale</asp:Localize>
                                    </label>
                                </div>
                            </div>
                            <div class="col-md-6" style="margin-bottom: 10px">
                                <i class="fa fa-info-circle"></i><small>
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InFaseDiCreazioneCentroAziendaleCreaMagazzino %>" runat="server">
                                        In fase di creazione di un nuovo Centro Aziendale, crea automaticamente anche un Magazzino.
                                    </asp:Localize>
                                </small>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="tab_gerarchia">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-12 text-center">
                                <h4 style="color: #052747; text-transform: uppercase;">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipoIcona %>" runat="server">Tipo Icona</asp:Localize>
                                </h4>
                            </div>
                            <div class="col-lg-12 text-center">
                                <div class="radio-inline">
                                    <label>
                                        <img src="../AB_Immagini/icone24/Cooperativa24a.ico" />
                                        <asp:RadioButton ID="Opt_TipoConsorzio" runat="server" ClientIDMode="Static" Text="<%$ Resources: AgronicaAgenda_2010, Consorzio %>" GroupName="TipoImpresa"></asp:RadioButton>
                                    </label>
                                </div>
                                <div class="radio-inline">
                                    <label>
                                        <img src="../AB_Immagini/icone24/Cooperativa24b.ico" />
                                        <asp:RadioButton ID="Opt_TipoOP" runat="server" ClientIDMode="Static" Text="OP" GroupName="TipoImpresa"></asp:RadioButton> <%-- i18n Per cosa sta O.P.? --%>
                                    </label>
                                </div>
                                <div class="radio-inline">
                                    <label>
                                        <img src="../AB_Immagini/icone24/Cooperativa24c.ico" />
                                        <asp:RadioButton ID="Opt_TipoCooperativa" runat="server" ClientIDMode="Static" Text="<%$ Resources: AgronicaAgenda_2010, Cooperativa %>" GroupName="TipoImpresa"></asp:RadioButton>
                                    </label>
                                </div>
                                <div class="radio-inline">
                                    <label>
                                        <img src="../AB_Immagini/icone24/x02_Impresa.png" />
                                        <asp:RadioButton ID="Opt_TipoImpresa" runat="server" ClientIDMode="Static" Text="<%$ Resources: AgronicaAgenda_2010, Impresa %>" GroupName="TipoImpresa"
                                            Checked="true"></asp:RadioButton>
                                    </label>
                                </div>
                            </div>
                        </div>
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
                                                    <%--<input type="text" class="form-control" id="txtPivaPadre" aria-describedby="lbl_txtPivaPadre" />--%>
                                                    <asp:TextBox ID="txtPivaPadre" runat="server" onkeydown="return (event.keyCode!=13);" ClientIDMode="Static" CssClass="form-control" aria-describedby="lbl_txtPivaPadre"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-3 col-md-3 col-sm-3">
                                        <div class="btn btn-default btn_100" id="btn_gerarchia_search">
                                            <i class="fa fa-search"></i>
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Cerca %>" runat="server">Cerca</asp:Localize>
                                        </div>
                                        <%--<div class="btn btn-default btn_100" id="btn_gerarchia_search" onclick="$('#<%=ImgBtn_Cerca_RagSoc.ClientID %>').click();">
                                            <i class="fa fa-search"></i>Cerca
                                        </div>
                                            <asp:ImageButton ID="ImgBtn_Cerca_RagSoc" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                            Style="display: none" />--%>
                                    </div>
                                </div>
                                <div class="row riga_referente" style="display: none;">
                                    <div class="col-lg-9 col-md-9 col-sm-9">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon control-label alert-info" id="lbl_padre" for="Cmb_Imprese">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AziendaReferente %>" runat="server">Azienda referente</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_Imprese" runat="server" ClientIDMode="Static" CssClass="form-control" data-live-search="true"
                                                        aria-describedby="lbl_padre">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impresa).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                    <div class="col-lg-3 col-md-3 col-sm-3">
                                        <div class="btn btn-info btn_100" id="btn_gerarchia_add">
                                            <i class="fa fa-plus"></i>
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                        </div>
                                        <%--<div class="btn btn-info btn_100" onclick="$('#<%=ImgBtn_Aggiungi_Padre.ClientID %>').click();">
                                            <i class="fa fa-plus"></i>Aggiungi
                                        </div>
                                        <asp:ImageButton ID="ImgBtn_Aggiungi_Padre" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                            Style="display: none" />--%>
                                    </div>
                                    <%End If%>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="table-responsive">
                                            <div id="tabGerarchia">
                                            </div>
                                            <%--<asp:GridView ID="GridView_Padri" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                                UseAccessibleHeader="true">
                                                <Columns>
                                                    <asp:ButtonField ButtonType="Link" HeaderText="Canc." Text="<i class='fa fa-times'></i>"
                                                        CommandName="Elimina">
                                                        <HeaderStyle Font-Names="Verdana" />
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:ButtonField>
                                                    <asp:BoundField DataField="Piva" HeaderText="Partita Iva"></asp:BoundField>
                                                    <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale">
                                                        <ItemStyle HorizontalAlign="left"></ItemStyle>
                                                    </asp:BoundField>
                                                    
                                                </Columns>
                                            </asp:GridView>
                                            <asp:Button ID="refresh_Grid_Padri" runat="server" style="display:none;" />
                                            --%>
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
                            <div class="col-lg-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_tecnico" for="CmbTecnico">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TecnicoReferente %>" runat="server">Tecnico Referente</asp:Localize>:
                                            </span>
                                            <asp:DropDownList ID="CmbTecnico" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker"
                                                data-live-search="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_odc" for="CmbTecnico">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OrganismoDiControllo %>" runat="server">Organismo di Controllo (ODC):</asp:Localize>
                                            </span>
                                            <asp:DropDownList ID="CmbOdc" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker"
                                                data-live-search="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 text-center">
                                <h4 style="color: #052747; text-transform: uppercase;">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PeriodoAttivitàImpresa %>" runat="server">Periodo Attività Impresa</asp:Localize>
                                </h4>
                            </div>
                            <div class="col-lg-4">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_validita_inizio" for="TxtValiditaInizio">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dal %>" runat="server">Dal</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="TxtValiditaInizio" runat="server" ClientIDMode="Static" CssClass="form-control datepicker"
                                        MaxLength="10">
                                    </asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_validita_fine" for="TxtValiditaFine">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Al %>" runat="server">Al</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="TxtValiditaFine" runat="server" ClientIDMode="Static" CssClass="form-control datepicker"
                                        MaxLength="10">
                                    </asp:TextBox>
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
                                    <div class="col-lg-5">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group" id="div_CmbCodice">
                                                            <span class="input-group-addon alert-info" id="lbl_Codice" for="CmbCodice">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server">Codice</asp:Localize>:
                                                            </span>
                                                            <asp:DropDownList ID="CmbCodice" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker"
                                                                data-live-search="true" aria-describedby="lbl_Codice">
                                                            </asp:DropDownList>
                                                            <%--<asp:DropDownList ID="CmbCodice" runat="server" CssClass="form-control"
                                                                data-live-search="true" aria-describedby="lbl_Codice">
                                                            </asp:DropDownList>--%>
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
                                            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impresa).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                            <div class="col-lg-12 text-right">
                                                <div class="btn btn-info" id="btn_Aggiungi_Codice">
                                                    <i class="fa fa-plus"></i>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                                </div>
                                                <%--<div class="btn btn-info" onclick="$('#<%=ImgBtn_Aggiungi_Codice.ClientID %>').click();">
                                                    <i class="fa fa-plus"></i>Aggiungi
                                                </div>
                                                <asp:ImageButton ID="ImgBtn_Aggiungi_Codice" runat="server" Style="display: none" />--%>
                                            </div>
                                            <%End If%>
                                        </div>
                                    </div>
                                    <div class="col-lg-7">
                                        <div id="tabCodici">
                                            <%--<div class="table-responsive">
                                            <asp:GridView ID="GridView_Codici" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered custom-table"
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
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Validita_Fine" HeaderText="Al">
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                            <asp:Button ID="refresh_Grid_Codici" runat="server" Text="Button" Style="display: none;" />
                                        </div>--%>
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
        <div class="row" data-toggle="validator" role="form">
            <div class="col-lg-12">
                <hr />
            </div>
        </div>
</asp:Content>
<asp:Content ID="cont" ContentPlaceHolderID="ContentScript" runat="server">

    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Impresa_Edit.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Impresa_Edit_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Impresa_Edit_ws_client.js") %>" ></script>

    <script type="text/javascript">
        var jsPadre = <%=jsPadre%>;
        var jsCodici = <%=jsCodici%>;
    </script>
    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
