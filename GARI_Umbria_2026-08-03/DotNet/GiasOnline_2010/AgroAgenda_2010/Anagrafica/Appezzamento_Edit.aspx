<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="Appezzamento_Edit.aspx.vb" Inherits="AgroAgenda_2010.Appezzamento_Edit" ClientIDMode="static" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- non togliere clientIDmode della pagina  Page primaRiga  -->
    <style type="text/css">
        #kendo_Particelle .k-grid-header-wrap > table, /* header table */
        #kendo_Particelle .k-grid-content table, /* data table, no virtual scrolling */
        #kendo_Particelle .k-virtual-scrollable-wrap table /* data table, with virtual scrolling */ {
            min-width: 1200px;
        }
        .clsCfrGisSuperficie {
            color:red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row">

        <div class="col-lg-12 col-md-12">
            <div class="row">
                <div class="col-lg-10 col-md-10" style="padding: 10px 15px; line-height: 1.6;">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Centro %>" runat="server">Centro</asp:Localize>: <b>
                        <asp:Label ID="LblCentro" runat="server"> </asp:Label></b>
                    <br />
                    <div id="div_lblCampo">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Campo %>" runat="server">Campo</asp:Localize>: <b>
                            <asp:Label ID="LblCampo" ClientIDMode="static" runat="server" Text="0"></asp:Label></b>
                    </div>
                    <div id="div_Superficie_Con">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superficie %>" runat="server">Superficie</asp:Localize> [Ha]: <b>
                            <asp:Label ID="LblSuperficie_Con_Catasto" runat="server" Text="0" EnableViewState="true"> </asp:Label></b>
                        <br /><span id="msgCfrGisSuperficieConCatasto" class="clsCfrGisSuperficie"></span>
                    </div>
                    <asp:TextBox ID="TxtSuperficie" runat="server" CssClass="txtUI " Width="100px" Style="display: none" />
                </div>
                <div class="col-lg-2 col-md-2 text-right">
                    <%--<div class="btn btn-success" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">--%>

                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Appezzamento).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>

                    <div class="btn btn-success" onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
                        <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                    </div>
                    <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                    <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown"
                        aria-haspopup="true" aria-expanded="false">
                        <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu">
                        <li><a href="#" onclick="$('#tipo_salva').val(0); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                        </a></li>
                        <li><a href="#" onclick="$('#tipo_salva').val(1); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                        </a></li>
                    </ul>
                    <% End If%>
                    <asp:HiddenField ClientIDMode="Static" ID="tipo_salva" runat="server" />
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                        Style="display: none" />

                    <% End If%>
                </div>

            </div>
        </div>
        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <ul id="Ul1" class="nav nav-tabs" data-tabs="tabs">
                <li class="active"><a href="#tab_dati_impianto" data-toggle="tab"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiAppezzamento %>" runat="server">Dati Appezzamento</asp:Localize></a></li>
                <li class="tab_dati_catastali"><a href="#tab_dati_catastali" data-toggle="tab"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiCatastali %>" runat="server">Dati Catastali</asp:Localize></a></li>
                <li class="tab_biologico"><a href="#tab_biologico" data-toggle="tab"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AgricolturaBiologica %>" runat="server">Agricoltura Biologica</asp:Localize></a></li>
                <li class="tab_indirizzi"><a href="#tab_indirizzi" data-toggle="tab"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Indirizzi %>" runat="server">Indirizzi</asp:Localize></a></li>
            </ul>
            <div id="my-tab-content" class="tab-content">
                <!-- TAB 1 --- DATI PRINCIPALI -->
                <div class="tab-pane active" id="tab_dati_impianto">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <h5 style="color: #052747;"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneCatastale %>" runat="server">Gestione Catastale</asp:Localize></h5>
                                            </div>
                                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                                <div class="radio-inline">
                                                    <label>
                                                        <asp:RadioButton ID="RBL_Catasto_Con" runat="server" meta:resourcekey="RBL_Catasto_Con" Text="Appezzamento con gestione catastale" GroupName="GestioneCatastale" class="GestioneCatastale"></asp:RadioButton>
                                                    </label>
                                                    &nbsp;&nbsp;&nbsp;
                                                    <label>
                                                        <asp:RadioButton ID="RBL_Catasto_Senza" runat="server" meta:resourcekey="RBL_Catasto_Senza" Text="Appezzamento senza gestione catastale" GroupName="GestioneCatastale" class="GestioneCatastale"></asp:RadioButton>
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12" id="div_superficie">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superficie %>" runat="server">Superficie</asp:Localize> [Ha]
                                                            </span>
                                                            <asp:TextBox ID="TxtSuperficie_SenzaCatasto" runat="server" CssClass="form-control"
                                                                MaxLength="10">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <span id="msgCfrGisSuperficieSenzaCatasto" class="clsCfrGisSuperficie"></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h5 style="color: #052747;"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, UtilizzoDelTerreno %>" runat="server">Utilizzo del Terreno</asp:Localize></h5>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="radio-inline">
                                            <label>
                                                <asp:RadioButton ID="Opt_Convenzionale" runat="server" Text="<%$ Resources: AgronicaAgenda_2010, Convenzionale %>" GroupName="UtilizzoTerreno" class="UtilizzoTerreno"></asp:RadioButton>
                                            </label>
                                        </div>
                                        <div class="radio-inline">
                                            <label>
                                                <asp:RadioButton ID="Opt_InConversione" runat="server" Text="<%$ Resources: AgronicaAgenda_2010, InConversione %>" GroupName="UtilizzoTerreno" class="UtilizzoTerreno"></asp:RadioButton>
                                            </label>
                                        </div>
                                        <div class="radio-inline">
                                            <label>
                                                <asp:RadioButton ID="Opt_Biologico" runat="server" Text="<%$ Resources: AgronicaAgenda_2010, Biologico %>" GroupName="UtilizzoTerreno" class="UtilizzoTerreno"></asp:RadioButton>
                                            </label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" clientidmode="Static" id="lbl_AppNome" for="TxtAppNome">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Denominazione %>" runat="server">Denominazione</asp:Localize>
                                                <a href="javascript:void(0);" data-toggle="tooltip" data-placement="top" title="<asp:Localize meta:resourcekey='SeVuotoCreoNomeAutomaticamente' runat='server'></asp:Localize>">
                                                    <i class="fa fa-info-circle"></i>
                                                </a>
                                            </span>
                                            <asp:TextBox ID="TxtAppNome" runat="server" CssClass="form-control" MaxLength="500" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-horizontal" id="div_campo_ass" runat="server">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_campo_ass" for="Cmb_campo_ass">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Campo %>" runat="server">Campo</asp:Localize> *
                                            </span>
                                            <asp:DropDownList ID="Cmb_campo_ass" runat="server" CssClass="form-control" AutoPostBack="false">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <input id="hidden_tipoCampo" type="hidden" runat="server" style="display: none" />
                                    <!-- 1=squadro 2=aggregatore -->
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12">
                                <div>
                                    <i class="fa fa-exclamation-triangle"></i>
                                    <small>
                                        <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CENTROValidità %>" runat="server">CENTRO validità:</asp:Localize></b>
                                        <asp:Label ID="lbl_centro_data_inizio" runat="server" CssClass="txtUI" BorderStyle="None"></asp:Label>
                                        - 
                                        <asp:Label ID="lbl_centro_data_fine" runat="server" CssClass="txtUI" BorderStyle="None"></asp:Label>
                                    </small>
                                    <input id="InizioCentro" name="InizioCentro" type="hidden" runat="server" style="display: none" />
                                    <input id="FineCentro" name="FineCentro" type="hidden" runat="server" style="display: none" />
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_validita_inizio" for="TxtValiditaInizio"><i class="fa fa-calendar"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataInizio %>" runat="server">Data Inizio</asp:Localize> *
                                            </span>
                                            <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="form-control datepicker"
                                                MaxLength="10">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_validita_fine" for="TxtValiditaFine"><i class="fa fa-calendar"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataFine %>" runat="server">Data Fine</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtValiditaFine" runat="server" CssClass="form-control datepicker"
                                                MaxLength="10">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>



                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Rif_Alfanum_App" for="Txt_Rif_Alfanum_App">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RiferimentoAppezzamentoAbbr %>" runat="server">Rif. Appezzamento</asp:Localize>
                                                <a href="javascript:void(0);" data-toggle="tooltip" data-placement="top" title="<asp:Localize meta:resourcekey='RiferimentoAppezzamentoPerSchedaDiCampagna' runat='server'></asp:Localize>">
                                                    <i class="fa fa-info-circle"></i>
                                                </a>
                                            </span>
                                            <asp:TextBox ID="Txt_Rif_Alfanum_App" runat="server" CssClass="form-control" MaxLength="15">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Isola" for="Txt_Isola">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Isola %>" runat="server">Isola</asp:Localize>
                                                <a href="javascript:void(0);" data-toggle="tooltip" data-placement="top" title="<asp:Localize meta:resourcekey='IsolaAppezzamento' runat='server'></asp:Localize>">
                                                    <i class="fa fa-info-circle"></i>
                                                </a>
                                            </span>
                                            <asp:TextBox ID="Txt_Isola" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <hr />

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize meta:resourcekey="RotazioniColturali" runat="server">Rotazioni Colturali</asp:Localize>
                                        </h4>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" for="ddl_Coltura_1">
                                                        <asp:Localize meta:resourcekey="ColturaPrecedentePrimoAnno" runat="server">Coltura Precedente 1° anno</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="ddl_Coltura_1" runat="server" CssClass="form-control" AutoPostBack="false">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" for="ddl_Coltura_2">
                                                        <asp:Localize meta:resourcekey="ColturaPrecedenteSecondoAnno" runat="server">Coltura Precedente 2° anno</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="ddl_Coltura_2" runat="server" CssClass="form-control" AutoPostBack="false">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" for="ddl_Coltura_3">
                                                        <asp:Localize meta:resourcekey="ColturaPrecedenteTerzoAnno" runat="server">Coltura Precedente 3° anno</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="ddl_Coltura_3" runat="server" CssClass="form-control" AutoPostBack="false">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" for="ddl_Coltura_4">
                                                        <asp:Localize meta:resourcekey="ColturaPrecedenteQuartoAnno" runat="server">Coltura Precedente 4° anno</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="ddl_Coltura_4" runat="server" CssClass="form-control" AutoPostBack="false">
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
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize meta:resourcekey="PosizioneAppezzamento" runat="server">Posizione appezzamento</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Pendenza" for="TxtPendenza">
                                                        <asp:Localize meta:resourcekey="Pendenza" runat="server">Pendenza</asp:Localize> [%]
                                                    </span>
                                                    <asp:TextBox ID="TxtPendenza" runat="server" CssClass="form-control"
                                                        MaxLength="10">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Esposizione" for="Cmb_Esposizione">
                                                        <asp:Localize meta:resourcekey="Esposizione" runat="server">Esposizione</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_Esposizione" runat="server" CssClass="form-control" AutoPostBack="false">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Ubicazione" for="Cmb_Ubicazione">
                                                        <asp:Localize meta:resourcekey="Ubicazione" runat="server">Ubicazione</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_Ubicazione" runat="server" CssClass="form-control" AutoPostBack="false">
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
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize meta:resourcekey="CoordinateBaricentro" runat="server">Coordinate baricentro</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group alert-info">
                                                    <span class="input-group-addon alert-info" id="lbl_CoordinataX" for="TxtCoordinataX">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,LatitudineAbbr %>" runat="server">Lat.</asp:Localize> (X)
                                                    </span>
                                                    <asp:TextBox ID="TxtCoordinataX" runat="server" CssClass="form-control"
                                                        MaxLength="10">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_CoordinataY" for="TxtCoordinataY">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,LongitudineAbbr %>" runat="server">Lng.</asp:Localize> (Y)
                                                    </span>
                                                    <asp:TextBox ID="TxtCoordinataY" runat="server" CssClass="form-control"
                                                        MaxLength="10">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_CoordinataZ" for="TxtCoordinataZ">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Altitudine %>" runat="server">Altitudine</asp:Localize> [m]
                                                    </span>
                                                    <asp:TextBox ID="TxtCoordinataZ" runat="server" CssClass="form-control"
                                                        MaxLength="10">
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
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize meta:resourcekey="RispettoBufferZone" runat="server">Rispetto Buffer Zone</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <h4><asp:Localize meta:resourcekey="LunghezzaConfineCon" runat="server">Lunghezza confine con:</asp:Localize></h4>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group alert-info">
                                                    <span class="input-group-addon alert-info" for="txtDistBZ_CorpiIdrici">
                                                        <asp:Localize meta:resourcekey="CorpiIdriciSuperficiali" runat="server">Corpi idrici superficiali</asp:Localize> [m]
                                                    </span>
                                                    <asp:TextBox ID="txtDistBZ_CorpiIdrici" runat="server" CssClass="form-control"
                                                        MaxLength="10">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" for="txtDistBZ_AreeResPub">
                                                        <asp:Localize meta:resourcekey="AreeResidenzialiPubbliche" runat="server">Aree residenziali/pubbliche</asp:Localize> [m]
                                                    </span>
                                                    <asp:TextBox ID="txtDistBZ_AreeResPub" runat="server" CssClass="form-control"
                                                        MaxLength="10">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" for="txtDistBZ_Allevamenti">
                                                        <asp:Localize meta:resourcekey="Allevamenti" runat="server">Allevamenti</asp:Localize> [m]
                                                    </span>
                                                    <asp:TextBox ID="txtDistBZ_Allevamenti" runat="server" CssClass="form-control"
                                                        MaxLength="10">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" for="txtDistBZ_VegNatNonColt">
                                                        <asp:Localize meta:resourcekey="VegetazioneNaturaleNonColtivata" runat="server">Vegetazione naturale/non coltivata</asp:Localize> [m]
                                                    </span>
                                                    <asp:TextBox ID="txtDistBZ_VegNatNonColt" runat="server" CssClass="form-control"
                                                        MaxLength="10">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" for="txtSupBZ_Riduzione">
                                                        <asp:Localize meta:resourcekey="OffsetDaUltimaPiantaCapezzagna" runat="server">Offset da ultima pianta (Capezzagna)</asp:Localize> [m]
                                                    </span>
                                                    <asp:TextBox ID="txtSupBZ_Riduzione" runat="server" CssClass="form-control"
                                                        MaxLength="10">
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
                                                            <asp:DropDownList ID="CmbCodice" runat="server" CssClass="form-control selectpicker"
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
                                                            <span class="input-group-addon alert-info" id="lbl_valorecod" for="TxtCodiceValore2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Valore %>" runat="server">Valore</asp:Localize>:
                                                            </span>
                                                            <asp:TextBox ID="TxtCodiceValore2" runat="server" CssClass="form-control" aria-describedby="lbl_valorecod">
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
                                                            <asp:TextBox ID="TxtValiditaInizioCodice" runat="server" CssClass="form-control datepicker"
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
                                                            <asp:TextBox ID="TxtValiditaFineCodice" runat="server" CssClass="form-control datepicker"
                                                                MaxLength="10">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Appezzamento).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                            <div class="col-lg-12 text-right">
                                                <div class="btn btn-info" id="btn_Aggiungi_Codice" runat="server">
                                                    <i class="fa fa-plus"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
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
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <!-- TAB 2  ---- CATASTO -->
                <div class="tab-pane" id="tab_dati_catastali">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="row">
                                    <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                        <asp:CheckBox ID="Chk_Macrousi" runat="server" name="chk_datiCatastali[]" AutoPostBack="false"
                                            Text="<%$ Resources: AgronicaAgenda_2010,VisualizzaIMacrousi %>" />
                                        <br />
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                        <asp:CheckBox ID="Chk_Utilizzi" runat="server" name="chk_datiCatastali[]" AutoPostBack="false"
                                            Text="<%$ Resources: AgronicaAgenda_2010,VisualizzaGliUtilizzi %>" />
                                        <br />
                                        <asp:DropDownList ID="Cmb_Utilizzo1" runat="server" Style="display: none" CssClass="form-control cmb_datiCatastali" AutoPostBack="false">
                                        </asp:DropDownList>
                                        <asp:CheckBox ID="Chk_Varieta" runat="server" name="chk_datiCatastali[]" AutoPostBack="false"
                                                      meta:resourcekey="Chk_Varieta" Text="Visualizza le Varietà" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div id="kendo_Particelle"></div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane" id="tab_indirizzi">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-12">
                                <h5><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Indirizzi %>" runat="server">Indirizzi</asp:Localize></h5>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12">
                                <div id="kendoIndirizzi"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- TAB 3 -->
                <div class="tab-pane" id="tab_biologico">
                    <div class="jumbotron">

                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_NumeroAppBio" for="Txt_NumeroAppBio">
                                                <asp:Localize meta:resourcekey="NumeroAppezzamentoBiologico" runat="server">N° Appezzamento Biologico</asp:Localize> *
                                                <a href="javascript:void(0);" data-toggle="tooltip" data-placement="bottom" title="<asp:Localize meta:resourcekey='RiferimentoRiportatoInSezioniPapENotifica' runat='server'></asp:Localize>">
                                                    <i class="fa fa-info-circle"></i>
                                                </a>
                                            </span>
                                            <asp:TextBox ID="Txt_NumeroAppBio" runat="server" CssClass="form-control"
                                                aria-describedby="lbl_NumeroAppBio">
                                            </asp:TextBox>
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
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, UtilizzoDelTerreno %>" runat="server">Utilizzo del Terreno</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-5">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group" id="div_Cmb_Utilizzo">
                                                            <span class="input-group-addon alert-info" id="lbl_Utilizzo" for="Cmb_Utilizzo">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server">Codice</asp:Localize>
                                                            </span>
                                                            <asp:DropDownList ID="Cmb_Utilizzo" runat="server" CssClass="form-control selectpicker"
                                                                data-live-search="true" aria-describedby="lbl_Utilizzo">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>


                                        <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Appezzamento).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                        <div class="col-lg-12 text-right">
                                            <div class="btn btn-info" id="btn_aggiungi_utilizzo" runat="server">
                                                <i class="fa fa-plus"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                            </div>
                                            <%--<div class="btn btn-info" onclick="$('#<%=ImgBtn_Aggiungi_Codice.ClientID %>').click();">
                                                    <i class="fa fa-plus"></i>Aggiungi
                                                </div>
                                                <asp:ImageButton ID="ImgBtn_Aggiungi_Codice" runat="server" Style="display: none" />--%>
                                        </div>
                                        <%End If%>
                                    </div>
                                    <div class="col-lg-7">
                                        <div id="tabUtilizzo">
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>


                        <div class="row">
                            <div class="col-md-12">
                                <h5 style="color: #052747;"><asp:Localize meta:resourcekey="ConformitàPerAgricolturaBiologica" runat="server">Conformità per l'Agricoltura Biologica</asp:Localize></h5>
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_ConfiniRischio" for="Txt_ConfiniRischio">
                                                <asp:Localize meta:resourcekey="ConfiniARischio" runat="server">Confini a Rischio</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_ConfiniRischio" runat="server" CssClass="form-control"
                                                aria-describedby="lbl_ConfiniRischio">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_DataFineImpiego" for="Txt_DataFineImpiego"><i class="fa fa-calendar"></i>
                                                <asp:Localize meta:resourcekey="FineImpiegoProdottiNonConformi" runat="server">Fine Impiego Prod. non Conformi </asp:Localize>
                                                <a href="javascript:void(0);" data-toggle="tooltip" data-placement="top" title="<asp:Localize meta:resourcekey='DataCessazioneImpiegoFitosanitariNonConformi' runat='server'></asp:Localize>">
                                                    <i class="fa fa-info-circle"></i>
                                                </a>
                                            </span>
                                            <asp:TextBox ID="Txt_DataFineImpiego" runat="server" CssClass="form-control datepicker"
                                                MaxLength="10">
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

    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>


    <!-- i18n Non usato? -->
    <div style="display: none">
        <input id="appezza" type="hidden" runat="server" value="0" />
        <input id="hidden_campo_cod" type="hidden" runat="server" value="0" />
        <input id="hidden_pushedParticelleDaSalvare" type="hidden" runat="server" value="" />
        <input id="hidden_tipoParticelleDaSalvare" type="hidden" runat="server" value="" />
        <asp:Label ID="LblRiferimenti" ClientIDMode="static" runat="server" Width="100%" CssClass="txtUI" BorderStyle="None" Style="display: none" />
        <asp:ImageButton ID="ImgBtn_SalvaTutto" ClientIDMode="static" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" Style="display: none" />
        <asp:CheckBox ID="Chk_SuperficieCatastale" runat="server" Checked="true" AutoPostBack="true" Text="La superficie dell'appezzamento deve corrispondere con la superficie catastale impostata" CssClass="txtUI" BorderStyle="None" />
        <input type="text" id="Input_SupTot" runat="server" class="txtUI" value="0,0000" readonly="readonly" />
        <asp:Label ID="Lbl_NoteCatastoAppezzamento" ClientIDMode="Static" runat="server" Text="L'Appezzamento appartiene ad un Campo per il quale NON e' stata definita una superficie catastale. Le particelle disponibili sono quelle del Centro Aziendale !!!"></asp:Label>

        <div id="tabsCnt" runat="server" width="100%">

            <div runat="server" style="width: 100%;">
                <table style="font-size: 12px; padding-left: 10px; text-align: left;" width="100%" aria-hidden="true">

                    <tr>
                        <td colspan="3">
                            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="clear"></div>
                                    <asp:GridView ID="DataGridParticelle" runat="server" AutoGenerateColumns="False"
                                        CellPadding="1" CellSpacing="2" CssClass="ui-widget-content " Style="font-size: 12px; margin-top: 10px;">
                                        <Columns>
                                            <asp:BoundField DataField="CodiceIstat_Provincia" HeaderText="CodiceIstat_Provincia">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="CodiceIstat_Comune" HeaderText="CodiceIstat_Comune">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Part_Cod" HeaderText="Part_Cod">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Provincia" HeaderText="Prov"></asp:BoundField>
                                            <asp:BoundField DataField="Comune" HeaderText="Com"></asp:BoundField>
                                            <asp:BoundField DataField="Sezione" HeaderText="Sez">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Foglio" HeaderText="Fgl">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Numero" HeaderText="Num">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Subalterno" HeaderText="Sub">
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SuperficieLorda"
                                                HeaderText="Sup. Catastale&lt;br&gt;Lorda&lt;br&gt;[ha]" HtmlEncode="False">
                                                <HeaderStyle HorizontalAlign="Center" Width="75px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Superficie" HeaderText="Sup. Condotta [ha]">
                                                <HeaderStyle HorizontalAlign="Center" Width="75px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SuperficieCondottaDisponibile"
                                                HeaderText="Sup. Condotta&lt;br&gt;Disponibile&lt;br&gt;[ha]"
                                                HtmlEncode="False">
                                                <HeaderStyle Width="80px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Right" ForeColor="Blue" CssClass="SuperficieDisponibile"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Macrouso_Des" HeaderText="Macrouso"></asp:BoundField>
                                            <asp:BoundField DataField="Sup_Macrouso" HeaderText="Sup. Macrouso [ha]">
                                                <HeaderStyle HorizontalAlign="Center" Width="75px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SuperficieMacrousoDisponibile"
                                                HeaderText="Sup. Macrouso&lt;br&gt;Disponibile&lt;br&gt;[ha]"
                                                HtmlEncode="False">
                                                <HeaderStyle Width="80px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Right" ForeColor="Blue" CssClass="SupMacrouso"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Veg_Des_agea" HeaderText="Utilizzo"></asp:BoundField>
                                            <asp:BoundField DataField="Cul_Des_agea" HeaderText="Varieta'">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Sup_Utilizzo" HeaderText="Sup. Utilizzo [ha]">
                                                <HeaderStyle HorizontalAlign="Center" Width="75px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SuperficieUtilizzoDisponibile"
                                                HeaderText="Sup. Utilizzo&lt;br&gt;Disponibile&lt;br&gt;[ha]"
                                                HtmlEncode="False">
                                                <HeaderStyle Width="80px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Right" ForeColor="Blue" CssClass="SupUtilizzo"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Intersezione [ha]">
                                                <HeaderStyle Width="90px"></HeaderStyle>
                                                <ItemStyle Font-Bold="True" HorizontalAlign="Center" ForeColor="Blue"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="ChkSelezionaParticella" runat="server" CssClass="ChkSelezionaParticella"></asp:CheckBox>
                                                    <asp:TextBox ID="TxtIntersezione" CssClass="SupIntersezione Testo_08_Blue" Style="text-align: right"
                                                        runat="server" Height="17px" Width="60px" BackColor="#FFFFC0" BorderStyle="None" MaxLength="10"
                                                        ReadOnly="false" ToolTip="Superficie di intersezione fra la Particella e l'Elemento selezionato"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Progressivo" HeaderText="Progressivo">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SuperficieImpiegata"
                                                HeaderText="Sup.&lt;br&gt;Impiegata" HtmlEncode="False">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Macrouso_Cod" HeaderText="Macrouso_Cod">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Veg_Cod_agea" HeaderText="Veg_Cod_agea">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Cul_Cod_agea" HeaderText="Cul_Cod_agea">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle CssClass="ui-widget-header" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>

                </table>
            </div>
            <div class="clear"></div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Appezzamento_Edit.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Appezzamento_Edit_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Appezzamento_Edit_kendoEvents.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Appezzamento_Edit_jQueryDocReady.js") %>"></script>

    <script type="text/javascript">

        var txtSuperficieJQuerySelector = "#<%=TxtSuperficie_SenzaCatasto.ClientID %>";

        var UtenteAbilitatoScrittura =
            <% If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Appezzamento).Scrittura = True) Then %>
            true;
            <% else %>
            false;
            <% end if %>


        var client_operazione = <% if (Operazione = enum_TipoOperazioneDB.Modifica) Then %> true <% Else %> false <% End if %>;

        /** roba vecchia */
        //        function DoPostBack_ControlliSiNo(key) {

        //            $('#< %=Btn_SalvaTutto.ClientID % >').click();

        //        }




        function caricaCodici() {
    <% if (jsCodici <> "") then %>
               initCodici = <%=jsCodici %>;
    <% else%>
               initCodici = "";
    <% end if %>
            if (initCodici != "")
                AggiornaTabCodici(initCodici);
        }

        function caricaUtilizzi() {
    <% if (jsUtilizzo <> "") Then %>
               initUtilizzo = <%=jsUtilizzo %>;
    <% else%>
               initUtilizzo = "";
    <% end if %>
            if (initUtilizzo != "")
                AggiornaTabUtilizzo(initUtilizzo);
        }

        var initIndirizzi = <%= jsIndirizzi %>;

    </script>

</asp:Content>



