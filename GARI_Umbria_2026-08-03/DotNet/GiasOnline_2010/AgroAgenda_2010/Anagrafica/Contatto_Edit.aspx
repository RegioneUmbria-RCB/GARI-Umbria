<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Contatto_Edit.aspx.vb" Inherits="AgroAgenda_2010.Contatto_Edit" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row">


        <div class="col-lg-12 col-md-12">
            <div class="row">
                <div class="col-lg-10 col-md-10">
                </div>
                <div class="col-lg-2 col-md-2 text-right">
                    <%--<div class="btn btn-success" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">--%>

                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>

                    <div class="btn btn-success xi-btn-primary" title="Salva" onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
                        <i class="fa fa-floppy-o"></i>Salva
                    </div>
                    <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                    <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown"
                        aria-haspopup="true" aria-expanded="false" style="margin-top: -20px !important;">
                        <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu" style="right: 0; left: auto !important;">
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(0); ValidaxSubmit();">Salva
                            ed Esci</a></li>
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(1); ValidaxSubmit();">Salva
                            e Continua</a></li>
                    </ul>
                    <% End If%>
                    <asp:HiddenField ID="tipo_salva" runat="server" ClientIDMode="Static"  />
                    <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static"
                        Style="display: none" />
                    <asp:ImageButton ID="ImgBtn_SalvaDistinta" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static"
                        Style="display: none" />

                    <% End If%>
                </div>
            </div>
        </div>

        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <ul id="Ul1" class="nav nav-tabs" data-tabs="tabs">
                <li class="active tab_dati_contatto"><a href="#tab_dati_contatto" data-toggle="tab">Dati Personali</a></li>
                <li class="tab_indirizzi"><a href="#tab_indirizzi" data-toggle="tab">Indirizzi</a></li>
                <li class="tab_costi"><a href="#tab_costi" data-toggle="tab">Costi</a></li>
            </ul>

            <div id="my-tab-content" class="tab-content">
                <!-- TAB 1 -->
                <div class="tab-pane active" id="tab_dati_contatto">
                    <div class="jumbotron">

                        <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>

                        <div class="row" id="div_riepilogo_error" style="margin-bottom: 25px;">
                            <div class="col-lg-12 col-md-12">
                                <b>I seguenti campi devono essere compilati:</b>
                                <br />
                                <br />
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <ul id="div_riepilogo_error_elenco">
                                    <li class="voce_1">Il campo <b>Codice fiscale</b> è da compilare</li>
                                    <li class="voce_2">Il campo <b>Cognome</b> è da compilare</li>
                                    <li class="voce_3">Il campo <b>Nome</b> è da compilare</li>
                                    <li class="voce_4">Il campo <b>Partita Iva</b> è da compilare</li>
                                    <li class="voce_5">Il campo <b>Ragione Sociale</b> è da compilare</li>
                                </ul>
                            </div>
                        </div>

                        <% End If%>

                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <%--<label class="col-lg-6 col-md-6 col-sm-6">
                                                <input type="radio" id="p1" name="list_persona" checked="checked" value="0" /> Persona Fisica
                                            </label> 
                                            <label class="col-lg-6 col-md-6 col-sm-6">
                                                <input type="radio" id="p2" name="list_persona" value="1" /> Persona Giuridica
                                            </label> --%>
                                        <asp:RadioButtonList ID="RBL_TipoUtente" AutoPostBack="false" runat="server" CssClass="table_tipoutente" ClientIDMode="Static"
                                            RepeatDirection="Horizontal">
                                            <asp:ListItem Value="0" Selected="True"><b>Persona Fisica</b></asp:ListItem>
                                            <asp:ListItem Value="1"><b>Persona Giuridica</b></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_CentriAziendali" for="Cmb_CentriAziendali">Visibilità</span>
                                                    <asp:DropDownList ID="Cmb_CentriAziendali" runat="server" CssClass="form-control" ClientIDMode="Static"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_CentriAziendali">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div class="row" style="padding-right: 15px;">
                                    <div class="col-lg-12 border_si">

                                        <div id="datiAzienda" style="display: none;">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <h5 class="agronica-card-title card-title">Dati Anagrafici Azienda</h5>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-11 col-md-11 col-sm-11">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">

                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_Piva" for="Txt_Piva">Partita IVA *</span>
                                                                <asp:TextBox ID="Txt_Piva" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>

                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-1 col-md-1 col-sm-1">
                                                    <asp:ImageButton ID="ImageButton1" ImageUrl="../AB_Immagini/Icone32/update.png" OnClientClick="generate_piva_click(); return false;" runat="server" ToolTip="Genera automaticamente una partita iva" Height="24px" Width="24px" ClientIDMode="Static"/>
                                                    <asp:Button ID="Apri_ModificaPiva" ToolTip="Modifica Partita IVA" runat="server" ClientIDMode="Static"
                                                        Visible="false" Text="Modifica" CssClass="bottone" Style="background-color: #E6F4FF;" />

                                                </div>
                                            </div>

                                            <div class="row" id="rowModificaPiva" runat="server">
                                                <!--<div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="btn btn-success" id="btn_ModificaPIVA" onclick="inserisciPiva();">
                                                            Modifica Partita Iva
                                                        </div>
                                                    </div>
                                                </div>-->
                                            </div>

                                            <div class="row">
                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_Rag_Soc" for="Txt_Rag_Soc">Ragione Sociale *</span>
                                                                <asp:TextBox ID="Txt_Rag_Soc" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>



                                        <div id="datiPersona">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <h5 class="agronica-card-title card-title">Dati Anagrafici Persona</h5>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-11 col-md-11 col-sm-11">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">

                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" for="Txt_CF">Codice Fiscale *</span>
                                                                <asp:TextBox ID="Txt_CF" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>

                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-1 col-md-1 col-sm-1">
                                                    <asp:ImageButton ID="ImgBtn_CF" ImageUrl="../AB_Immagini/Icone32/update.png" OnClientClick="generate_CodFisc_click(); return false;"
                                                        runat="server" ToolTip="Genera automaticamente un codice fiscale" Height="24px"
                                                        Width="24px" ClientIDMode="Static" />
                                                </div>


                                            </div>

                                            <div class="row" id="rowModificaCF" runat="server">
                                                <!--<div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="btn btn-success" id="btn_ModificaCF" onclick="inserisciCF();">
                                                            Modifica Codice Fiscale
                                                        </div>
                                                    </div>
                                                </div>-->
                                            </div>

                                            <div class="row">
                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="TxtCognome" for="Txt_Cognome">Cognome *</span>
                                                                <asp:TextBox ID="Txt_Cognome" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="TxtNome" for="Txt_Nome">Nome *</span>
                                                                <asp:TextBox ID="Txt_Nome" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="TxtDataNascita" for="Txt_DataNascita"><i class="fa fa-calendar"></i>Data di Nascita</span>
                                                                <asp:TextBox ID="Txt_DataNascita" runat="server" CssClass="form-control txtUI datepicker" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6 col-md-12 col-sm-6">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="ddlSesso" for="ddl_Sesso">Sesso</span>
                                                                <asp:DropDownList ID="ddl_Sesso" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                                    <asp:ListItem Selected="True" Value="M">M</asp:ListItem>
                                                                    <asp:ListItem Value="F">F</asp:ListItem>
                                                                </asp:DropDownList>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>



                                <div class="row" style="padding-right: 15px;">
                                    <div class="col-lg-12 border_si">


                                        <div class="row">
                                            <div class="col-md-12 text-center">
                                                <h4 style="color: #052747; text-transform: uppercase;">Recapiti</h4>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="TxtTelefono" for="Txt_Telefono">Telefono</span>
                                                            <asp:TextBox ID="Txt_Telefono" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="TxtCellulare" for="Txt_Cellulare">Cellulare</span>
                                                            <asp:TextBox ID="Txt_Cellulare" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="TxtFax" for="Txt_Fax">Fax</span>
                                                            <asp:TextBox ID="Txt_Fax" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="TxtMail" for="Txt_Mail">E-mail</span>
                                                            <asp:TextBox ID="Txt_Mail" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>



                                    </div>
                                </div>


                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="row">
                                    <div id="tabRapporti"></div>

                                </div>

                            </div>
                        </div>




                        <div id="datiPatentino" class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">Patentino</h4>
                                    </div>

                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Patentino" for="Txt_Patentino">N° Patentino</span>
                                                    <asp:TextBox ID="Txt_Patentino" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Patentino_Ente" for="Txt_Patentino_Ente">Ente Rilascio</span>
                                                    <asp:TextBox ID="Txt_Patentino_Ente" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Patentino_Rilascio" for="Txt_Patentino_Rilascio"><i class="fa fa-calendar"></i>Data Rilascio</span>
                                                    <asp:TextBox ID="Txt_Patentino_Rilascio" MaxLength="10" runat="server" CssClass="txtUI datepicker form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Patentino_Scadenza" for="Txt_Patentino_Scadenza"><i class="fa fa-calendar"></i>Data Scadenza</span>
                                                    <asp:TextBox ID="Txt_Patentino_Scadenza" MaxLength="10" runat="server" CssClass="txtUI datepicker form-control" ClientIDMode="Static"></asp:TextBox>
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
                                        <h5 class="agronica-card-title card-title">Altri Codici</h5>
                                    </div>


                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Progressivo" for="Txt_Progressivo">Cod./Matricola</span>
                                                    <asp:TextBox ID="Txt_Progressivo" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Attivita" for="Txt_Attivita">Attività</span>
                                                    <asp:TextBox ID="Txt_Attivita" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Badge" for="Txt_Badge">Badge</span>
                                                    <asp:TextBox ID="Txt_Badge" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>


                </div>

                <!-- TAB 2 -->
                <div class="tab-pane" id="tab_indirizzi">
                    <div class="jumbotron">

                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_tipo_Indirizzo" for="ddl_tipo_Indirizzo">Residenza</span>
                                            <asp:DropDownList ID="ddl_tipo_Indirizzo" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                <asp:ListItem Value="3">Residenza</asp:ListItem>
                                                <asp:ListItem Value="5">Luogo di Nascita</asp:ListItem>
                                                <asp:ListItem Value="2">Domicilio</asp:ListItem>
                                                <asp:ListItem Value="4">Residenza Estiva</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_via" for="txt_via">Indirizzo *</span>
                                            <asp:TextBox ID="txt_via" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="div_prov_com">
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_provincia" for="ddl_provincia">Provincia *</span>
                                                <asp:DropDownList ID="ddl_provincia" runat="server" CssClass="form-control selectpicker" ClientIDMode="Static">
                                                </asp:DropDownList>
                                                <asp:TextBox ID="Txt_ProCodIstat" runat="server" CssClass="form-control " Style="display: none" ClientIDMode="Static"> </asp:TextBox>
                                                <asp:TextBox ID="Txt_ProvinciaSigla" runat="server" CssClass="form-control " Style="display: none" ClientIDMode="Static"> </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_comune" for="ddl_comune">Comune *</span>
                                                <asp:DropDownList ID="ddl_comune" runat="server" CssClass="form-control selectpicker" ClientIDMode="Static">
                                                </asp:DropDownList>
                                                <asp:TextBox ID="Txt_ComCodIstat" runat="server" CssClass="form-control " Style="display: none" ClientIDMode="Static"> </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_cap" for="txt_cap">CAP *</span>
                                                <asp:TextBox ID="txt_cap" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_frazione" for="txt_frazione">Frazione</span>
                                            <asp:TextBox ID="txt_frazione" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_stato" for="txt_stato">Stato</span>
                                            <%--     <asp:TextBox ID="txt_stato" runat="server" CssClass="txtUI form-control"></asp:TextBox> --%>
                                            <asp:DropDownList name="cmb_Stato" ID="cmb_Stato" runat="server" CssClass="form-control selectpicker"
                                                AutoPostBack="False" data-live-search="true" aria-describedby="lbl_stato" ClientIDMode="Static">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Note_Indirizzo" for="Txt_Note_Indirizzo">Note</span>
                                            <asp:TextBox ID="Txt_Note_Indirizzo" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                            <div class="col-lg-12 text-right">
                                <div class="btn btn-info" id="btn_aggiungi_indirizzo">
                                    <%--<div class="btn btn-info" onclick="$('#<=ImgBtn_Aggiungi_Codice.ClientID %>').click();">--%>
                                    <i class="fa fa-plus"></i>Nuovo
                                </div>
                                <div class="btn btn-success" id="btn_modifica_indirizzo" style="display: none;">
                                    <%--<div class="btn btn-info" onclick="$('#<=ImgBtn_Aggiungi_Codice.ClientID %>').click();">--%>
                                    <i class="fa fa-pencil"></i>Modifica
                                </div>
                                <asp:ImageButton ID="ImgBtn_Salva_Indirizzi" runat="server" Style="display: none" ClientIDMode="Static"/>
                                <asp:ImageButton ID="ImgBtn_Aggiungi_Indirizzo" runat="server" Style="display: none" ClientIDMode="Static"/>
                            </div>
                            <%End If %>
                        </div>

                        <div class="row">
                            <div id="tabIndirizzi">
                            </div>
                        </div>


                    </div>
                </div>

                <!-- TAB 3 -->
                <div class="tab-pane" id="tab_costi">
                    <div class="jumbotron">

                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_UdmCod_Prezzo" for="ddl_UdmCod_Prezzo">Misura *</span>
                                            <asp:DropDownList ID="ddl_UdmCod_Prezzo" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                <asp:ListItem Value="2">Ora</asp:ListItem>
                                                <asp:ListItem Value="1">Ettaro</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Prezzo" for="Txt_Prezzo">Prezzo *</span>
                                            <asp:TextBox ID="Txt_Prezzo" runat="server" CssClass="txtUI form-control" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Inizio_Prezzo" for="Txt_Inizio_Prezzo"><i class="fa fa-calendar"></i>Validità Inizio *</span>
                                            <asp:TextBox ID="Txt_Inizio_Prezzo" runat="server" CssClass="txtUI form-control datepicker" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Fine_Prezzo" for="Txt_Fine_Prezzo"><i class="fa fa-calendar"></i>Validità Fine *</span>
                                            <asp:TextBox ID="Txt_Fine_Prezzo" runat="server" CssClass="txtUI form-control datepicker" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>


                            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                            <div class="col-lg-12 text-right">
                                <div class="btn btn-info" id="btn_aggiungi_costo">
                                    <%--<div class="btn btn-info" onclick="$('#<=ImgBtn_Aggiungi_Codice.ClientID %>').click();">--%>
                                    <i class="fa fa-plus"></i>Aggiungi
                                </div>
                                <asp:ImageButton ID="ImgBtn_Aggiungi_Costo" runat="server" Style="display: none" ClientIDMode="Static" />
                            </div>
                            <%End If %>
                        </div>

                        <div class="row">
                            <div id="tabCosti">
                            </div>
                        </div>


                    </div>
                </div>


            </div>
            <!-- fine TAB content -->

        </div>
    </div>

    <div id="window_cambia_CF"></div>

    <div id="window_cambia_PIVA"></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contatto_Edit.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contatto_Edit_jQueryDocReady.js") %>" ></script>

    <script type="text/javascript">
        var jsIndirizzi = <%=jsIndirizzi%>;
        var jsCosti = <%=jsCosti%>;
        var jsRapporti = <%=jsRapporti%>;
    </script>
    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
