<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="Catasto_Edit.aspx.vb" Inherits="AgroAgenda_2010.Catasto_Edit" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>



<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .RBL_Salva tbody tr td {
            padding-left: 20px;
            padding-right: 20px;
        }

        #div_errori_salvataggio {
            background-color: #d82f2b;
            color: #fff;
            -webkit-border-radius: 6px;
            -moz-border-radius: 6px;
            border-radius: 6px;
            padding: 20px 0;
            margin-bottom: 15px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="form_particella" class="row" data-toggle="validator" role="form">
        <div class="col-lg-12 col-md-12">
            <div class="row">
                <div class="col-lg-10 col-md-10" style="padding: 10px 35px; line-height: 1.6;">
               <%--     Riferimenti:
                    <b><asp:Label ID="LblRiferimenti" runat="server">
                        </asp:Label></b>
                <br />
                    Impresa:
                    <b><asp:Label ID="Lbl_Impresa" runat="server">
                        </asp:Label></b>
                <br />--%> 
                    Centro:
                    <b><asp:Label ID="Lbl_CentroAziendale" runat="server" ClientIDMode="Static" >
                        </asp:Label></b>
                </div>

                 <!--- SALVATAGGIO VISIBLE DESKTOP -->
                <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                <div class="col-lg-2 col-md-2 visible-lg">
                   <%-- <div class="bs-callout-success bs-callout-success-all alert alert-success">--%>
                   <div>
                        <asp:Panel ID="Box_Salva" runat="server" meta:resourcekey="Box_SalvaResource1" CssClass="text-right" style="padding-right: 20px; margin-bottom: 20px;">
                            <%--<div class="col-md-4">
                                 <div class="btn btn-success btn_per_load" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();" style="width: 100%">
                                <div class="btn btn-success btn_per_load2" style="width: 100%">
                                    <span class="fa fa-floppy-o"></span><span>Salva</span>
                                </div>
                            </div>--%>
                        
                            <%--<asp:ImageButton ID="ImgBtn_Salva" runat="server" ImageUrl="../AB_Immagini/icone32/dischetto.ico"
                                Style="display: none;" meta:resourcekey="ImgBtn_SalvaResource1" />--%>
                            <asp:ImageButton ID="ImgBtn_SalvaTutto" ClientIDMode="Static" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" Style="display: none" />


                            <div class="btn-group btn-salva">
                                <button type="button" class="btn btn-success" onclick="ValidaxSubmit();"><i class="fa fa-floppy-o"></i>
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                                </button>

                                <%If (Operazione <> AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Modifica) Then %>

                                <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <span class="caret"></span>
                                <span class="sr-only">Toggle Dropdown</span>
                                </button>
                                    
                                <ul class="dropdown-menu">
                                    <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(0); ValidaxSubmit();" >
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                                    </a></li>
                                    <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(1); ValidaxSubmit();">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                                    </a></li>
                                </ul>
                                <asp:HiddenField ID="tipo_salva" runat="server" />
                                <% End If  %>
                            </div>
                            
                        </asp:Panel>
                    </div>
                </div>
                <%End If %>

            </div>
            <div class="jumbotron">

                <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                         
                         <div class="row" id="div_riepilogo_error">
                            <div class="col-lg-12 col-md-12">
                                <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CompilareISeguentiCampi %>" runat="server">I seguenti campi devono essere compilati:</asp:Localize></b>
                                <br /><br />
                            </div>
                             <div class="col-lg-12 col-md-12 col-sm-12">
                                <ul id="div_riepilogo_error_elenco">
                                    <li class="voce_1"><asp:Localize meta:resourcekey="ObbligatorioProvincia" runat="server">Il campo <b>Provincia</b> è da selezionare</asp:Localize></li>
                                    <li class="voce_2"><asp:Localize meta:resourcekey="ObbligatorioComune" runat="server">Il campo <b>Comune</b> è da selezionare</asp:Localize></li>
                                    <li class="voce_3"><asp:Localize meta:resourcekey="ObbligatorioFoglio" runat="server">Il campo <b>Foglio</b> è da compilare</asp:Localize></li>
                                    <li class="voce_4"><asp:Localize meta:resourcekey="ObbligatorioNumero" runat="server">Il campo <b>Numero</b> è da compilare</asp:Localize></li>
                                </ul>
                            </div>
                         </div>

                <% End If%>

                <div class="row" id="div_errori_salvataggio" style="display:none;">
                    <div class="col-sm-12">
                        <b><asp:Localize Text="<%$ Resources: CorreggereGliErroriPerSalvare %>" runat="server">Correggere i seguenti errori per poter salvare</asp:Localize>:</b>
                        <br/><br/>
                    </div>
                    <div class="col-sm-12">
                        <ul id="elenco_errori_salvataggio"></ul>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12 col-md-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <asp:CheckBox ClientIDMode="Static" ID="chk_estero" runat="server" AutoPostBack="false" Text="<%$ Resources: ParticellaCatastaleESTERA %>" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-6 col-md-6">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <%--<label class="col-lg-3 control-label">
                                    Provincia:</label>
                                <div class="col-lg-9">
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <asp:DropDownList ID="Cmb_Provincia" runat="server" CssClass="form-control selectpicker "
                                                AutoPostBack="True" data-live-search="true" required>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-xs-2 hidden">
                                            <asp:Label ID="TxtCodProvincia" runat="server" Style="font-weight: bold;">
                                            </asp:Label>
                                        </div>
                                    </div>
                                </div>--%>

                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="basic-addon1">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provincia %>" runat="server">Provincia</asp:Localize> *
                                    </span>
                                    <asp:DropDownList ID="Cmb_Provincia" runat="server" CssClass="form-control selectpicker " ClientIDMode="Static"
                                        AutoPostBack="False" data-live-search="true">
                                    </asp:DropDownList>
                                </div>
                                 <div class="col-xs-2 hidden">
                                    <asp:Label ID="TxtCodProvincia" runat="server" Style="font-weight: bold;" ClientIDMode="Static">
                                    </asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <%--<label class="col-lg-3 control-label">
                                    Comune:</label>
                                <div class="col-lg-9">
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <asp:DropDownList ID="Cmb_Comune" runat="server" CssClass="form-control selectpicker "
                                                AutoPostBack="True" data-live-search="true" required>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-xs-2 hidden">
                                            <asp:Label ID="TxtCodComune" runat="server" Style="font-weight: bold;">
                                            </asp:Label>
                                        </div>
                                    </div>
                                </div>--%>
                                 <div class="input-group">
                                    <span class="input-group-addon alert-info" id="Span1">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Comune %>" runat="server">Comune</asp:Localize> *
                                    </span>
                                    <asp:DropDownList ID="Cmb_Comune" runat="server" CssClass="form-control selectpicker " ClientIDMode="Static"
                                        AutoPostBack="False" data-live-search="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-xs-2 hidden">
                                    <asp:Label ID="TxtCodComune" runat="server" Style="font-weight: bold;" ClientIDMode="Static">
                                    </asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <%--<label class="col-lg-3 control-label">
                                    Sezione:</label>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="Txt_Sezione" runat="server" CssClass="form-control ">
                                    </asp:TextBox>
                                </div>--%>
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="Span2">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Sezione %>" runat="server">Sezione</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="Txt_Sezione" runat="server" CssClass="form-control " maxlength="2" ClientIDMode="Static">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6">
                        <div class="form-horizontal">
                            <div class="form-group">
                               <%-- <label class="col-lg-3 control-label">
                                    Foglio:</label>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="Txt_Foglio" runat="server" CssClass="form-control "
                                        required>
                                    </asp:TextBox>
                                </div>--%>
                                <div class="input-group required">
                                    <span class="input-group-addon alert-info" id="Span3">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Foglio %>" runat="server">Foglio</asp:Localize> *
                                    </span>
                                    <asp:TextBox ID="Txt_Foglio" runat="server" CssClass="form-control" ClientIDMode="Static">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6">
                        <div class="form-horizontal">
                            <div class="form-group">
                               <%-- <label class="col-lg-3 control-label">
                                    Numero:</label>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="Txt_Numero" runat="server" CssClass="form-control "
                                        required>
                                    </asp:TextBox>
                                </div>--%>
                                <div class="input-group required">
                                    <span class="input-group-addon alert-info" id="Span4">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Numero %>" runat="server">Numero</asp:Localize> *
                                    </span>
                                    <asp:TextBox ID="Txt_Numero" runat="server" CssClass="form-control " ClientIDMode="Static">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6">
                        <div class="form-horizontal">
                            <div class="form-group">
                               <%-- <label class="col-lg-3 control-label">
                                    Subalterno:</label>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="Txt_Subalterno" runat="server" CssClass="form-control "
                                        required>
                                    </asp:TextBox>
                                </div>--%>
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="Span5">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Subalterno %>" runat="server">Subalterno</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="Txt_Subalterno" runat="server" CssClass="form-control " maxlength="3" ClientIDMode="Static">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-12 col-md-12 border_si" id="divSuperficie">
                        <h4 class="text-center" style="color:#052747; text-transform: uppercase;">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superficie %>" runat="server">Superficie</asp:Localize></h4>
                        <div class="row">
                            <div class="col-lg-4 col-md-4">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <%--<label class="col-lg-4 control-label">
                                            Ettari:</label>
                                        <div class="col-lg-8">
                                            <asp:TextBox ID="TxtSup_Ettari" runat="server" CssClass="form-control "
                                                required Text="0"> 
                                            </asp:TextBox>
                                            <i style="font-size: 10px">Valore Numerico Intero</i>
                                        </div>--%>
                                        <div class="input-group required">
                                            <span class="input-group-addon alert-info" id="Span6">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ettari %>" runat="server">Ettari</asp:Localize> *
                                                <a href="javascript:void(0);" data-toggle="tooltip" data-placement="top" title="<asp:Localize meta:resourcekey='ValoreNumericoIntero' runat='server'></asp:Localize>">
                                                    <i class="fa fa-info-circle"></i>
                                                </a>
                                            </span>
                                            <asp:TextBox ID="TxtSup_Ettari" runat="server" CssClass="form-control input-obbligatori" Text="0" ClientIDMode="Static">  
                                            </asp:TextBox>
                                        </div>
       
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                       <%-- <label class="col-lg-4 control-label">
                                            Are:</label>
                                        <div class="col-lg-8">
                                            <asp:TextBox ID="TxtSup_Are" runat="server" CssClass="form-control "
                                                required Text="0"> 
                                            </asp:TextBox>
                                            <i style="font-size: 10px">Valore Numerico Intero tra 0 e 99</i>
                                        </div>--%>
                                        <div class="input-group required">
                                            <span class="input-group-addon alert-info" id="Span7">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Are %>" runat="server">Are</asp:Localize> * 
                                                <a href="javascript:void(0);" data-toggle="tooltip" data-placement="top" title="<asp:Localize meta:resourcekey='ValoreNumericoInteroRange' runat='server'></asp:Localize>">
                                                    <i class="fa fa-info-circle"></i>
                                                </a>
                                            </span>
                                            <asp:TextBox ID="TxtSup_Are" runat="server" CssClass="form-control " Text="0" MaxLength="2" ClientIDMode="Static"> 
                                            </asp:TextBox>
                                        </div>
  
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <%--<label class="col-lg-4 control-label">
                                            Centiare:</label>
                                        <div class="col-lg-8">
                                            <asp:TextBox ID="TxtSup_Centiare" runat="server" CssClass="form-control "
                                                required Text="0"> 
                                            </asp:TextBox>
                                            <i style="font-size: 10px">Valore Numerico Intero tra 0 e 99</i>
                                        </div>--%>
                                        <div class="input-group required">
                                            <span class="input-group-addon alert-info" id="Span8">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Centiare %>" runat="server">Centiare</asp:Localize> * 
                                                <a href="javascript:void(0);" data-toggle="tooltip" data-placement="top" title="<asp:Localize meta:resourcekey='ValoreNumericoInteroRange' runat='server'></asp:Localize>">
                                                    <i class="fa fa-info-circle"></i>
                                                </a>
                                            </span>
                                            <asp:TextBox ID="TxtSup_Centiare" runat="server" CssClass="form-control " Text="0" MaxLength="2" ClientIDMode="Static"> 
                                            </asp:TextBox>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-12 col-md-12 border_si" id="divPossessi">
                        <h4 class="text-center" style="color:#052747; text-transform: uppercase;"><asp:Localize meta:resourcekey="Possessi" runat="server">Possessi</asp:Localize></h4>
          
                        <div class="row">
                            <div class="col-lg-12">
                                <div><i class="fa fa-exclamation-triangle"></i>
                                    <small>
                                        <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CENTROAZIENDALEValidità %>" runat="server">CENTRO AZIENDALE validità:</asp:Localize></b>
                                        <asp:Label ID="lbl_centro_az_data_inizio" runat="server" ClientIDMode="Static"  CssClass="txtUI" BorderStyle="None"></asp:Label> 
                                        - 
                                        <asp:Label ID="lbl_centro_az_data_fine" runat="server" ClientIDMode="Static" CssClass="txtUI" BorderStyle="None"></asp:Label>
                                    </small>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                 <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span9">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,TitoloDiPossesso %>" runat="server">Titolo di Possesso</asp:Localize> *
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_TitoloPossesso" runat="server" CssClass="form-control" ClientIDMode="Static"
                                                            AutoPostBack="false">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span12">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SuperficieCondottaAbbr %>" runat="server">Sup. Condotta</asp:Localize> (ha) *
                                                    </span>
                                                    <asp:TextBox ID="Txt_SupCondotta" runat="server" CssClass="form-control" MaxLength="10" ClientIDMode="Static">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span17">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceParticella %>" runat="server">Codice Particella</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_CodParticella" runat="server" CssClass="form-control" MaxLength="10" ClientIDMode="Static">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="row">
                                                    <label class="col-xs-12 control-label" style="text-align: left;">
                                                        <asp:Localize meta:resourcekey="PeriodoPossesso" runat="server">Periodo Possesso</asp:Localize>:
                                                    </label>
                                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="Span10" style="min-width: 50px;">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dal %>" runat="server">Dal</asp:Localize>
                                                            </span>
                                                            <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="form-control datepicker" ClientIDMode="Static"
                                                                MaxLength="10">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="Span11" style="min-width: 50px;">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Al %>" runat="server">Al</asp:Localize>
                                                            </span>
                                                            <asp:TextBox ID="TxtValiditaFine" runat="server" CssClass="form-control datepicker" ClientIDMode="Static"
                                                                MaxLength="10">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12 div-aggiungi">
                                        <%--<div class="btn btn-info" onclick="$('#<%=ImgBtn_Inserisci.ClientID %>').click();">--%>
                                        <div class="btn btn-info" id="btn_aggiungi_possesso" runat="server" ClientIDMode="Static">
                                            <i class="fa fa-plus"></i>
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                        </div>
                                       <%-- <asp:ImageButton ID="ImgBtn_Inserisci" runat="server" Style="display: none;" ImageUrl="~/AB_Immagini/Icone24/Freccia24RossaDX.ico" />--%>
                                    </div>
                                    
                                </div>
                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="row">
                                    <div class="col-lg-12 col-md-12">
                                        <div id="tabPossessi"></div>
                                        <%--<div class="table-responsive">
                                            <asp:GridView ID="GridView_Possessi" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                                UseAccessibleHeader="true">
                                                <Columns>
                                                    <asp:ButtonField ButtonType="Link" HeaderText="Canc." Text="<i class='fa fa-times'></i>"
                                                        CommandName="Elimina">
                                                        <HeaderStyle Font-Names="Verdana" />
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:ButtonField>
                                                    <asp:BoundField DataField="TitoloPossesso_Des" HeaderText="Titolo di Possesso"></asp:BoundField>
                                                    <asp:BoundField DataField="Validita_Inizio" HeaderText="Dal">
                                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Validita_Fine" HeaderText="Al">
                                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Sup_Condotta" HeaderText="Sup. Condotta" HtmlEncode="False">
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>--%>
                                    </div>
                                </div>
                            </div>

                        </div>
                        

                    </div>

                    <div class="col-lg-12 col-md-12 border_si" id="divMetodoProduzione">
                        <h4 class="text-center" style="color:#052747; text-transform: uppercase;">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MetodoDiProduzione %>" runat="server">Metodo Di Produzione</asp:Localize>
                        </h4>

                        <div id="grigliaMetodiProduzione"></div>
                    </div>

                </div>
            </div>
        </div>
        <%--<div class="col-lg-2 text-right">
            <div class="col-lg-12">
                <div class="btn btn-success btn_100" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">
                    <i class="fa fa-floppy-o"></i>Salva
                </div>
                <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                    Style="display: none" />
            </div>
            <div class="col-lg-12">
                <asp:RadioButtonList ID="RBL_Salva" runat="server" Style="float: right">
                    <asp:ListItem Value="0" Selected="True">Salva ed Esci</asp:ListItem>
                    <asp:ListItem Value="1">Salva e Continua</asp:ListItem>
                </asp:RadioButtonList>
                <div style="clear: both">
                </div>
            </div>
        </div>--%>
    </div>
    <!-- VARIE -->
    <!-- contiene l'indice del tab selezionato, viene controllato lato server CalledFromClient_SetIndexTab -->
    <input type="hidden" id="clickedTabUI" runat="server" />
    <!-- salva la chiamata alla funzione per il postback -->
    <input type="hidden" id="fooName_PostedBack" runat="server" />
    <!-- parametri le funzione di postback -->
    <input type="hidden" id="fooName_Param_PostedBack" runat="server" />
    <br />
    <!-- FILTRI TAB -->
    <div id="content">
        <ul id="tabs" class="nav nav-tabs" data-tabs="tabs">
            <%--<li class="active"><a href="#tab_possessi" data-toggle="tab">Possessi</a></li>--%>
            <li class="active"><a href="#tab_macrousi" class="tab_macrousi" data-toggle="tab">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Macrousi %>" runat="server">Macrousi</asp:Localize>
            </a></li>
            <li><a href="#tab_zonizzazione" class="tab_zonizzazione" data-toggle="tab">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Zonizzazione %>" runat="server">Zonizzazione</asp:Localize>
            </a></li>
            <li><a href="#tab_classamento" class="tab_classamento" data-toggle="tab">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Classamento %>" runat="server">Classamento</asp:Localize>
            </a></li>
        </ul>
        <div id="my-tab-content" class="tab-content">
            <%--<div class="tab-pane active" id="tab_possessi">
                <div class="jumbotron">
                    <div class="row" style="margin-top: 10px;">
                        <div class="col-lg-6 col-md-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                     <div class="input-group">
                                        <span class="input-group-addon" id="Span9">Titolo Possesso</span>
                                        <asp:DropDownList ID="Cmb_TitoloPossesso" runat="server" CssClass="form-control"
                                                AutoPostBack="false">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="row">
                                        <label class="col-lg-4 col-md-4 col-sm-12 control-label" style="text-align: left;">
                                            Periodo Possesso:</label>            
                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                            <div class="input-group">
                                                <span class="input-group-addon" id="Span10" style="min-width: 50px;">dal</span>
                                                <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="form-control datepicker"
                                                    MaxLength="10">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                            <div class="input-group">
                                                <span class="input-group-addon" id="Span11" style="min-width: 50px;">al</span>
                                                <asp:TextBox ID="TxtValiditaFine" runat="server" CssClass="form-control datepicker"
                                                    MaxLength="10">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon" id="Span12">Sup. Condotta (ha)</span>
                                        <asp:TextBox ID="Txt_SupCondotta" runat="server" CssClass="form-control" MaxLength="10">
                                        </asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 text-right">
                            <div class="btn btn-info" onclick="$('#<%=ImgBtn_Inserisci.ClientID %>').click();">
                                <i class="fa fa-plus"></i>Aggiungi
                            </div>
                            <asp:ImageButton ID="ImgBtn_Inserisci" runat="server" Style="display: none;" ImageUrl="~/AB_Immagini/Icone24/Freccia24RossaDX.ico" />
                        </div>
                    </div>
                </div>
                <div class="row" style="margin-top: 10px;">
                    <div class="col-lg-12 col-md-12">
                        <div class="table-responsive">
                            <asp:GridView ID="GridView_Possessi" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                UseAccessibleHeader="true">
                                <Columns>
                                    <asp:ButtonField ButtonType="Link" HeaderText="Canc." Text="<i class='fa fa-times'></i>"
                                        CommandName="Elimina">
                                        <HeaderStyle Font-Names="Verdana" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:ButtonField>
                                    <asp:BoundField DataField="TitoloPossesso_Des" HeaderText="Titolo di Possesso"></asp:BoundField>
                                    <asp:BoundField DataField="Validita_Inizio" HeaderText="Dal">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Validita_Fine" HeaderText="Al">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sup_Condotta" HeaderText="Sup. Condotta" HtmlEncode="False">
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>--%>
            <div class="tab-pane active" id="tab_macrousi">
                <div class="jumbotron">
                    <div class="row" style="margin-top: 10px;">
                        <div class="col-lg-6 col-md-6">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group" style="word-wrap: break-word;">
                                                <span class="input-group-addon alert-info" id="Span13">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Macrouso %>" runat="server">Macrouso</asp:Localize> *
                                                </span>
                                                <asp:DropDownList ID="Cmb_Macrousi" runat="server" CssClass="form-control input-small" AutoPostBack="false" ClientIDMode="Static">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <%--<label class="col-lg-2 control-label">
                                                Superficie (ha):</label>
                                            <div class="col-lg-10">
                                                <asp:TextBox ID="Txt_SupMacrouso" runat="server" CssClass="form-control" MaxLength="10">
                                                </asp:TextBox>
                                            </div>--%>
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span16">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superficie %>" runat="server">Superficie</asp:Localize> (ha) *
                                                </span>
                                                <asp:TextBox ID="Txt_SupMacrouso" runat="server" CssClass="form-control" MaxLength="10" ClientIDMode="Static">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-xs-12 control-label" style="text-align: left;">
                                                    <asp:Localize meta:resourcekey="PeriodoMacrouso" runat="server">Periodo Macrouso</asp:Localize>:</label>
                                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="Span14" style="min-width: 50px;">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dal %>" runat="server">Dal</asp:Localize>
                                                        </span>
                                                        <asp:TextBox ID="TxtValiditaInizioMacrouso" runat="server" CssClass="form-control datepicker" ClientIDMode="Static"
                                                            MaxLength="10">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="Span15" style="min-width: 50px;">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Al %>" runat="server">Al</asp:Localize>
                                                        </span>
                                                        <asp:TextBox ID="TxtValiditaFineMacrouso" runat="server" CssClass="form-control datepicker" ClientIDMode="Static"
                                                            MaxLength="10">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-lg-12 div-aggiungi">
                                    <%--<div class="btn btn-info" onclick="$('#<%=ImgBtn_InserisciMacrouso.ClientID %>').click();">--%>
                                    <div class="btn btn-info" id="btn_aggiungi_macrousi" runat="server" ClientIDMode="Static">
                                        <i class="fa fa-plus"></i>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                    </div>
                    <%--                <asp:ImageButton ID="ImgBtn_InserisciMacrouso" runat="server" Style="display: none;"
                                        ImageUrl="~/AB_Immagini/Icone24/Freccia24RossaDX.ico" />--%>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6">
                            <div class="row">
                                <div class="col-lg-12 col-md-12">
                                    <div id="tabMacrousi"></div>
                                    <%--<div class="table-responsive">
                                        <asp:GridView ID="GridView_Macrousi" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                                            <Columns>
                                                <asp:ButtonField ButtonType="Link" HeaderText="Canc." Text="<i class='fa fa-times'></i>"
                                                    CommandName="Elimina">
                                                    <HeaderStyle Font-Names="Verdana" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:ButtonField>
                                                <asp:BoundField DataField="Descrizione" HeaderText="Macrouso"></asp:BoundField>
                                                <asp:BoundField DataField="Validita_Inizio" HeaderText="Dal">
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Validita_Fine" HeaderText="Al">
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Superficie" HeaderText="Sup.&lt;br&gt;" HtmlEncode="False">
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
            </div>
            <div class="tab-pane" id="tab_zonizzazione">
                <div class="jumbotron">
                    <div class="row" style="margin-top: 10px;">
                        <div class="col-lg-6 col-md-6">
                            <div class="row"">
                                <div class="col-lg-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <%--<label class="col-lg-2 control-label">
                                                Zona:</label>
                                            <div class="col-lg-10">
                                                <asp:DropDownList ID="Cmb_Zone" runat="server" CssClass="form-control" AutoPostBack="false">
                                                </asp:DropDownList>
                                            </div>--%>
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_Zone">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Zona %>" runat="server">Zona</asp:Localize> *
                                                </span>
                                                <asp:DropDownList ID="Cmb_Zone" runat="server" CssClass="form-control" AutoPostBack="false" ClientIDMode="Static">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <%--<label class="col-lg-2 control-label">
                                                Superficie (ha):</label>
                                            <div class="col-lg-10">
                                                <asp:TextBox ID="Txt_SupZona" runat="server" CssClass="form-control" MaxLength="10">
                                                </asp:TextBox>
                                            </div>--%>
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_SupZona">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superficie %>" runat="server">Superficie</asp:Localize> (ha) *
                                                </span>
                                                <asp:TextBox ID="Txt_SupZona" runat="server" CssClass="form-control" MaxLength="10" ClientIDMode="Static">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-xs-12 control-label" style="text-align: left;">
                                                    <asp:Localize meta:resourcekey="PeriodoZona" runat="server">Periodo Zona</asp:Localize>:</label>
                                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="Span14" style="min-width: 50px;">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dal %>" runat="server">Dal</asp:Localize>
                                                        </span>
                                                        <asp:TextBox ID="TxtValiditaInizioZona" runat="server" CssClass="form-control datepicker" ClientIDMode="Static"
                                                            MaxLength="10">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="Span15" style="min-width: 50px;">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Al %>" runat="server">Al</asp:Localize>
                                                        </span>
                                                        <asp:TextBox ID="TxtValiditaFineZona" runat="server" CssClass="form-control datepicker" ClientIDMode="Static"
                                                            MaxLength="10">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12 col-md-12 text-right">
                                    <%--<div class="btn btn-info" onclick="$('#<%=ImgBtn_InserisciZona.ClientID %>').click();">--%>
                                    <div class="btn btn-info" id="btn_aggiungi_zonizzazione" runat="server" ClientIDMode="Static">
                                        <i class="fa fa-plus"></i>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                    </div>
                           <%--         <asp:ImageButton ID="ImgBtn_InserisciZona" runat="server" Style="display: none;"
                                        ImageUrl="~/AB_Immagini/Icone24/Freccia24RossaDX.ico" />--%>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-6 col-md-6">
                            <div class="row">
                                <div class="col-lg-12 col-md-12">
                                    <div id="tabZone"></div>
                                    <%--<div class="table-responsive">
                                        <asp:GridView ID="GridView_Zone" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                                            <Columns>
                                                <asp:ButtonField ButtonType="Link" HeaderText="Canc." Text="<i class='fa fa-times'></i>"
                                                    CommandName="Elimina">
                                                    <HeaderStyle Font-Names="Verdana" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:ButtonField>
                                                <asp:BoundField DataField="Descrizione" HeaderText="Macrouso"></asp:BoundField>
                                                <asp:BoundField DataField="Area" HeaderText="Sup.&lt;br&gt;" HtmlEncode="False">
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                </div>
                
            </div>
            <div class="tab-pane" id="tab_classamento">
                <div class="jumbotron">
                    <div class="row" style="margin-top: 10px;">
                        <div class="col-lg-6 col-md-6">
                            <div class="row">
                                <div class="col-lg-12 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span19">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Porzione %>" runat="server">Porzione</asp:Localize>
                                                </span>
                                                <asp:TextBox ID="TxtPorzione" runat="server" CssClass="form-control" MaxLength="10" ClientIDMode="Static">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span20">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superficie %>" runat="server">Superficie</asp:Localize>
                                                </span>
                                                <asp:TextBox ID="TxtSupClass" runat="server" CssClass="form-control" MaxLength="10" ClientIDMode="Static">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon  alert-info" id="Span21">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Qualità %>" runat="server">Qualità</asp:Localize> *
                                                </span>
                                                <asp:DropDownList ID="Cmb_QualitaCatasto" runat="server" CssClass="form-control" ClientIDMode="Static"
                                                    AutoPostBack="false">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span22">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Classe %>" runat="server">Classe</asp:Localize>
                                                </span>
                                                <asp:TextBox ID="TxtClasseCatasto" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span23">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,RedditoDominicale %>" runat="server">Reddito dominicale</asp:Localize> [&euro;]
                                                </span>
                                                <asp:TextBox ID="TxtRedditoDom" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span24">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,RedditoAgrario %>" runat="server">Reddito agrario</asp:Localize> [&euro;]
                                                </span>
                                                <asp:TextBox ID="TxtRedditoAgr" runat="server" CssClass="form-control" ClientIDMode="Static">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-12 text-right">
                                    <div class="btn btn-info" id="btn_aggiungi_classamento" runat="server" ClientIDMode="Static">
                                        <i class="fa fa-plus"></i>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                    </div>
                       <%--             <asp:ImageButton ID="ImgBtn_InserisciClasse" runat="server" Style="display: none;"
                                        ImageUrl="~/AB_Immagini/Icone24/Freccia24RossaDX.ico" />--%>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-6 col-md-6">
                            <div class="row">
                                <div class="col-lg-12 col-md-12">
                                    <div id="tabClassamenti"></div>
                                    <%--<div class="table-responsive">
                                        <asp:GridView ID="GridView_Classamento" runat="server" AutoGenerateColumns="False"
                                            CssClass="table table-bordered" UseAccessibleHeader="true">
                                            <Columns>
                                                <asp:ButtonField ButtonType="Link" HeaderText="Canc." Text="<i class='fa fa-times'></i>"
                                                    CommandName="Elimina">
                                                    <HeaderStyle Font-Names="Verdana" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:ButtonField>
                                                <asp:BoundField DataField="Porzione" HeaderText="Porzione">
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Qualita_Des" HeaderText="Qualita'"></asp:BoundField>
                                                <asp:BoundField DataField="Classe" HeaderText="Classe">
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Sup_Classe" HeaderText="Sup."></asp:BoundField>
                                                <asp:BoundField DataField="Reddito_Dominicale" HeaderText="Reddito<br/>Dominicale"
                                                    HtmlEncode="False"></asp:BoundField>
                                                <asp:BoundField DataField="Reddito_Agrario" HeaderText="Reddito<br/>Agrario" HtmlEncode="False">
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
            </div>
        </div>
    </div>
    <%--
    <div class="col-lg-12 col-md-12 text-center" style="margin-top: 15px; padding: 0 25%;">
        <div class="row">
            <div class="col-lg-6">
                <div class="btn btn-success btn_100" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">
                    <i class="fa fa-floppy-o"></i>Salva
                </div>           
            </div>
        </div>
        <div class="row">
            <div class="col-lg-6 col-md-6">
                <asp:RadioButtonList ID="RBL_Salva" runat="server" Style="float: right">
                    <asp:ListItem Value="0" Selected="True">Salva ed Esci</asp:ListItem>
                    <asp:ListItem Value="1">Salva e Continua</asp:ListItem>
                </asp:RadioButtonList>
                <div style="clear: both">
                </div>
            </div>
        </div>
    </div>
    --%>
     <!--- SALVATAGGIO VISIBLE MOBILE -->
     <br />
    <div class="col-md-6 col-sm-8 col-xs-12 hidden-lg">
        <%--<div class="bs-callout-success bs-callout-success-all alert alert-success">--%>
            <asp:Panel ID="Panel1" runat="server" meta:resourcekey="Box_SalvaResource1" CssClass="row">
                <div class="col-md-4 col-sm-12 text-center">
                    <%--<div class="btn btn-success btn_per_load" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();" style="width: 100%">
                        <span class="fa fa-floppy-o"></span><span>Salva</span>
                    </div>--%>
                    <div class="btn-group btn-salva">
                        <button type="button" class="btn btn-success" onclick="ValidaxSubmit();"><i class="fa fa-floppy-o"></i>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                        </button>

                        <%If (Operazione <> AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Modifica) Then %>

                        <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <span class="caret"></span>
                        <span class="sr-only">Toggle Dropdown</span>
                        </button>
                                    
                        <ul class="dropdown-menu">
                            <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(0); ValidaxSubmit();" >
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                            </a></li>
                            <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(1); ValidaxSubmit();">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                            </a></li>
                        </ul>
                        <asp:HiddenField ID="HiddenField1" runat="server" />
                        <% End If  %>
                    </div>
                </div>
                <div class="col-md-8 col-sm-8 salva_radio">
                    <%--<asp:ImageButton ID="ImgBtn_Salva" runat="server" ImageUrl="../AB_Immagini/icone32/dischetto.ico"
                        Style="display: none;" meta:resourcekey="ImgBtn_SalvaResource1" />
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" Style="display: none" />
                    <style>
                        .RBL_Salva tbody tr td
                        {
                            padding-left: 20px;
                            padding-right: 20px;
                        }
                    </style>
                    --%>
                    <%--<asp:RadioButtonList ID="RadioButtonList1" runat="server" CssClass="RBL_Salva radio" meta:resourcekey="RBL_SalvaResource1"
                        RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True" Text="Salva ed Esci" Value="0" meta:resourcekey="ListItemResource1"></asp:ListItem>
                        <asp:ListItem Text="Salva e Nuovo" Value="1" meta:resourcekey="ListItemResource2"></asp:ListItem>
                        <asp:ListItem Text="Salva e Duplica" Value="2" meta:resourcekey="ListItemResource3"></asp:ListItem>
                    </asp:RadioButtonList>--%>
                  

                </div>
            </asp:Panel>
        <%--</div>--%>
    </div>
    <input id="InizioCentro" name="InizioCentro" type="hidden" runat="server">
    <input id="FineCentro" name="FineCentro" type="hidden" runat="server">

    <br /><br /><br /><br /><br /><br /><br />


</asp:Content>
<asp:Content ID="cont" ContentPlaceHolderID="ContentScript" runat="server">
    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Catasto_Edit.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Catasto_Edit_jQueryDocReady.js") %>" ></script>

    <script type="text/javascript">
        var jsOperazione = <%=Operazione%>;

        var jsPossessi = <%=jsPossessi%>;
        var jsMacrousi = <%=jsMacrousi%>;
        var jsZone = <%=jsZone%>;
        var jsClassamenti = <%=jsClassamenti%>;
    </script>
    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
