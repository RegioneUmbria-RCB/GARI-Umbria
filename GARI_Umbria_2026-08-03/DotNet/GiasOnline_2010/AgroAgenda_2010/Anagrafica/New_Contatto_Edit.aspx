<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="New_Contatto_Edit.aspx.vb" Inherits="AgroAgenda_2010.New_Contatto_Edit" %>

<%@ Register TagPrefix="uc1" TagName="ListinixContattiUC" Src="~/GestioneContabilita/Listini/ListiniPrezzixContattiUC.ascx" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        .gear-auto-generate {
            height: 24px;
            width: 24px;
            background: url(../AB_Immagini/Icone32/update.png) no-repeat center;
            background-size: cover;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row">

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12 text-right">
                <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura = True) And (XTipoOperazione <> enum_TipoOperazioneDB.Lettura) Then%>

                <div class="btn btn-success xi-btn-primary" title='<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Salva %>" runat="server">Salva</asp:Localize>' onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
                    <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Salva %>" runat="server">Salva</asp:Localize>
                </div>
                <asp:HiddenField ID="tipo_salva" runat="server" />
                <asp:ImageButton ID="ImgBtn_AnnullaTutto" ImageUrl="~/AB_Immagini/icone32/esci.bmp"
                        runat="server" Style="float: right; display:none" />
                    
                <% End If%>
            </div>
        </div>

        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <ul id="Ul1" class="nav nav-tabs" data-tabs="tabs">
                <li class="active tab_dati_contatto"><a href="#tab_dati_contatto" data-toggle="tab"><asp:Localize meta:resourcekey="DatiPersonali" runat="server">Dati Personali</asp:Localize></a></li>
                <li class="tab_rubrica"><a href="#tab_rubrica" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Rubrica %>" runat="server">Rubrica</asp:Localize>
                </a></li>
                <li class="tab_indirizzi"><a href="#tab_indirizzi" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Indirizzi %>" runat="server">Indirizzi</asp:Localize>
                </a></li>
                <li class="tab_rapporti_contabili"><a href="#tab_rapporti_contabili" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,RapportiContabili %>" runat="server">Rapporti Contabili</asp:Localize>
                </a></li>
                <li class="tab_efatt"><a href="#tab_efatt" data-toggle="tab"><asp:Localize meta:resourcekey="FatturaElettronica" runat="server">Fattura Elettronica</asp:Localize></a></li>
                <li class="tab_altri_dati"><a href="#tab_altri_dati" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AltriDati %>" runat="server">Altri Dati</asp:Localize>
                </a></li>
               <%-- <%If (GestisciContabilita = True) Then%>
                    <li class="tab_dettagli_contabili"><a href="#tab_dettagli_contabili" data-toggle="tab">Dettagli Contabili</a></li>
                <% End If%>--%>
                <li class="tab_dettagli_contabili"><a href="#tab_dettagli_contabili" style="display:none" data-toggle="tab"><asp:Localize meta:resourcekey="DettagliContabili" runat="server">Dettagli Contabili</asp:Localize></a></li>
                <li class="tab_costi_new"><a href="#tab_costi_new" data-toggle="tab"><asp:Localize meta:resourcekey="Costi" runat="server">Costi</asp:Localize></a></li>
                <li class="tab_memo"><a href="#tab_memo" data-toggle="tab"><asp:Localize meta:resourcekey="Memo" runat="server">Memo</asp:Localize></a></li>
            </ul>

            <div id="my-tab-content" class="tab-content">
                <!-- TAB 1 -->
                <div class="tab-pane active" id="tab_dati_contatto">
                    <div class="jumbotron">

                        <%If (XTipoOperazione = enum_TipoOperazioneDB.Scrittura) Then%>

                        <div class="row" id="div_riepilogo_error" style="margin-bottom: 25px;">
                            <div class="col-lg-12 col-md-12">
                                <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CompilareISeguentiCampi %>" runat="server">I seguenti campi devono essere compilati:</asp:Localize></b>
                                <br />
                                <br />
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <ul id="div_riepilogo_error_elenco">
                                    <li class="voce_1" style="display:none"><asp:Localize meta:resourcekey="ObbligatorioCodiceFiscale" runat="server">Il campo <b>Codice fiscale</b> è da compilare</asp:Localize></li>
                                    <li class="voce_2" style="display:none"><asp:Localize meta:resourcekey="ObbligatorioCognome" runat="server">Il campo <b>Cognome</b> è da compilare</asp:Localize></li>
                                    <li class="voce_3" style="display:none"><asp:Localize meta:resourcekey="ObbligatorioNome" runat="server">Il campo <b>Nome</b> è da compilare</asp:Localize></li>
                                    <li class="voce_4" style="display:none"><asp:Localize meta:resourcekey="ObbligatorioPartitaIva" runat="server">Il campo <b>Partita Iva</b> è da compilare</asp:Localize></li>
                                    <li class="voce_5" style="display:none"><asp:Localize meta:resourcekey="ObbligatorioRagioneSociale" runat="server">Il campo <b>Ragione Sociale</b> è da compilare</asp:Localize></li>
                                    <li class="voce_6" style="display:none"><asp:Localize meta:resourcekey="ObbligatorioRapportoContabile" runat="server">Assegnare al contatto almeno un rapporto contabile</asp:Localize></li>
                                    <li class="voce_7" style="display:none"><asp:Localize meta:resourcekey="ObbligatorioVAT" runat="server">Il campo <b>VAT</b> è da compilare</asp:Localize></li>
                                    <li class="voce_8" style="display:none"><asp:Localize meta:resourcekey="ObbligatorioIntestazione" runat="server">Il campo <b>Intestazione</b> è da compilare</asp:Localize></li>
                                </ul>
                            </div>
                        </div>

                        <% End If%>

                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="row">
                                    <div class="col-lg-12 border_si border_si__no_shadow">
                                    <div class="row">
                                    <div class="col-lg-9 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_ddl_itaeste">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Tipo %>" runat="server">Tipo</asp:Localize>
                                                    </span>
                                                    <input type="text" name="ddl_ItaEste" id="ddl_ItaEste" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                            <div class="form-group form-group--switch">
                                                <label id="lbl_cb_fittizio" for="cb_fittizio"><asp:Localize meta:resourcekey="Fittizio" runat="server">Fittizio</asp:Localize></label>     
                                                <input type="checkbox" id="cb_fittizio" name="cb_fittizio" class="kendoSwitch" />
                                            </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12">
                                        <%--<label class="col-lg-6 col-md-6 col-sm-6">
                                                <input type="radio" id="p1" name="list_persona" checked="checked" value="0" /> Persona Fisica
                                            </label> 
                                            <label class="col-lg-6 col-md-6 col-sm-6">
                                                <input type="radio" id="p2" name="list_persona" value="1" /> Persona Giuridica
                                            </label> --%>
                                        <asp:RadioButtonList ID="RBL_TipoUtente" AutoPostBack="false" runat="server" CssClass="table_tipoutente"
                                            RepeatDirection="Horizontal">
                                            <asp:ListItem Value="0" Selected="True" Text="<%$ Resources: AgronicaAgenda_2010, PersonaFisica %>"><b></b></asp:ListItem>
                                            <asp:ListItem Value="1" Text="<%$ Resources: AgronicaAgenda_2010, PersonaGiuridica %>"><b></b></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>

                                    <div class="col-lg-12 container--visibilita">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_CentriAziendali" for="Cmb_CentriAziendali">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Visibilità %>" runat="server">Visibilità</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_CentriAziendali" runat="server" CssClass="form-control"  ClientIDMode="Static"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_CentriAziendali">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                </div>
                                </div>

                                <div class="row">
                                    <div class="col-lg-12 border_si border_si__no_shadow">

                                        <div id="datiAzienda" style="display: none;">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <h5 class="agronica-card-title card-title"><asp:Localize meta:resourcekey="DatiAnagraficiAzienda" runat="server">Dati Anagrafici Azienda</asp:Localize></h5>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-11 col-md-11 col-sm-11">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">

                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_Piva" for="Txt_Piva">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,PartitaIVA %>" runat="server">Partita IVA</asp:Localize> *
                                                                </span>
                                                                <asp:TextBox ID="Txt_Piva" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>

                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-1 col-md-1 col-sm-1">
                                                    <button ID="ImageButton1" onclick="generate_piva_click(); return false;"
                                                            runat="server" title="<%$ Resources: AgronicaAgenda_2010,GeneraAutomaticamentePartitaIva %>" 
                                                            class="btn gear-auto-generate">
                                                    </button>
                                                    <asp:Button ID="Apri_ModificaPiva" ToolTip="<%$ Resources: AgronicaAgenda_2010,ModificaPartitaIVA %>" runat="server"
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
                                                                <span class="input-group-addon alert-info" id="lbl_Rag_Soc" for="Txt_Rag_Soc">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,RagioneSociale %>" runat="server">Ragione Sociale</asp:Localize> *
                                                                </span>
                                                                <asp:TextBox ID="Txt_Rag_Soc" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>

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
                                                                <span class="input-group-addon alert-info" id="lbl_Nome_Breve_Azienda" for="Txt_Nome_Breve_Azienda">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,NomeBreve %>" runat="server">Nome Breve</asp:Localize>
                                                                </span>
                                                                <asp:TextBox ID="Txt_Nome_Breve_Azienda" runat="server" CssClass="form-control txtUI" ClientIDMode="Static"></asp:TextBox>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="row_cf_estero">

                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_cf_estero" for="Txt_CF_Estero">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CodiceFiscale %>" runat="server">Codice Fiscale</asp:Localize>
                                                                </span>
                                                                <asp:TextBox ID="Txt_CF_Estero" runat="server" CssClass="form-control txtUI" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                               <%-- <div class="col-lg-3 col-md-12 col-sm-12 text-center">
                                                    <div class="btn btn-success buttonClass" id="btn_copia_piva" style="margin-left:10px;">
                                                        <span class="fa"></span>Partita Iva
                                                    </div>
				                                </div>--%>

                                            </div>

                                        </div>



                                        <div id="datiPersona">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <h5 class="agronica-card-title card-title"><asp:Localize meta:resourcekey="DatiAnagraficaPersona" runat="server">Dati Anagrafici Persona</asp:Localize></h5>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-11 col-md-11 col-sm-11">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">

                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_cf" for="Txt_CF">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CodiceFiscale %>" runat="server">Codice Fiscale</asp:Localize> *
                                                                </span>
                                                                <asp:TextBox ID="Txt_CF" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>

                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-1 col-md-1 col-sm-1">
                                                    <button ID="ImgBtn_CF" onclick="generate_CodFisc_click(); return false;"
                                                            runat="server" title="<%$ Resources: AgronicaAgenda_2010,GeneraAutomaticamenteCodiceFiscale %>" 
                                                            class="btn gear-auto-generate">
                                                    </button>
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
                                                                <span class="input-group-addon alert-info" id="TxtCognome" for="Txt_Cognome">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Cognome %>" runat="server">Cognome</asp:Localize> *
                                                                </span>
                                                                <asp:TextBox ID="Txt_Cognome" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="TxtNome" for="Txt_Nome">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Nome %>" runat="server">Nome</asp:Localize> *
                                                                </span>
                                                                <asp:TextBox ID="Txt_Nome" runat="server" CssClass="form-control txtUI required" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_Nome_Breve_Persona" for="Txt_Nome_Breve_Persona">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,NomeBreve %>" runat="server">Nome Breve</asp:Localize>
                                                                </span>
                                                                <asp:TextBox ID="Txt_Nome_Breve_Persona" runat="server" CssClass="form-control txtUI" ClientIDMode="Static"></asp:TextBox>

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
                                                                <span class="input-group-addon alert-info" id="TxtDataNascita" for="Txt_DataNascita">
                                                                    <i class="fa fa-calendar"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,DataNascita %>" runat="server">Data di Nascita</asp:Localize>
                                                                </span>
                                                                <asp:TextBox ID="Txt_DataNascita" runat="server" CssClass="form-control txtUI datepicker"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6 col-md-12 col-sm-6">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="ddlSesso" for="ddl_Sesso">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Sesso %>" runat="server">Sesso</asp:Localize>
                                                                </span>
                                                                <asp:DropDownList ID="ddl_Sesso" runat="server" CssClass="form-control">
                                                                    <asp:ListItem Selected="True" Value="M" Text="<%$ Resources: AgronicaAgenda_2010,MaschioSigla %>"></asp:ListItem>
                                                                    <asp:ListItem Value="F" Text="<%$ Resources: AgronicaAgenda_2010,FemminaSigla %>"></asp:ListItem>
                                                                </asp:DropDownList>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-6 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_Convenevoli"><asp:Localize meta:resourcekey="Convenevoli" runat="server">Convenevoli</asp:Localize></span>
                                                            <input type="text" id="ddlConvenevoli" class="form-control " />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>



                                <div class="row">
                                    <div class="col-lg-12 border_si border_si__no_shadow">


                                        <div class="row">
                                            <div class="col-md-12">
                                                <h5 class="agronica-card-title card-title">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,AltriCodici %>" runat="server">Altri Codici</asp:Localize>
                                                </h5>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_Badge" for="Txt_Badge"><asp:Localize meta:resourcekey="Badge" runat="server">Badge</asp:Localize></span>
                                                            <asp:TextBox ID="Txt_Badge" runat="server" CssClass="txtUI form-control"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-3 col-md-12 col-sm-12">
                                                <div class="form-group">
                                                    <label id="lbl_cb_eudr" for="cb_eudr"><asp:Localize meta:resourcekey="EUDR" runat="server">EUDR</asp:Localize></label>     
                                                    <input type="checkbox" id="cb_eudr" name="cb_eudr" class="kendoSwitch" />
                                                </div>
                                            </div>
                                        </div>
                                        
                                    </div>
                                </div>


                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <%--<div class="row">
                                    <div id="tabRapporti"></div>

                                </div>--%>

                                <div class="row" style="padding-right: 15px;">
	                                <div class="col-lg-12 border_si border_si__no_shadow">

		                                <div id="datiRiepilogoRapContabili">
			                                <div class="row">
				                                <div class="col-md-12">
					                                <h5 class="agronica-card-title card-title">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,RapportiContabili %>" runat="server">Rapporti Contabili</asp:Localize>
                                                    </h5>
				                                </div>
			                                </div>
		   
                                            <div id="rapportoPrincipale">
			                                    <div class="row">
				                                    <div class="col-lg-12 col-md-12 col-sm-12">
					                                    <div class="form-horizontal">
						                                    <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_Rapporto_Principale2">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,RapportoContabile %>" runat="server">Rapporto Contabile</asp:Localize>
                                                                    </span>
                                                                    <input type="text" name="ddl_Rapporto_Principale" id="ddl_Rapporto_Principale" class="form-control " />
                                                                </div>
						                                    </div>
					                                    </div>
				                                    </div>
			                                    </div>
                                           
                                            </div>

                                            <div id="listaRapportiSelezionati" style="display: none;">
                                                     <div class="row">
				                                        <div class="col-lg-12 col-md-12 col-sm-12">
					                                        <div class="form-horizontal">
						                                        <div class="form-group">
							                                
                                                                     <div class="col-lg-12 col-md-12 col-sm-12" id="idRapportiSelezionati">
                                                                        <select style="width: 100%" id="rapportiSelezionati"></select>
                                                                    </div>

						                                        </div>
					                                        </div>
				                                        </div>
			                                        </div>
                                                </div>

                                            <div class="row" style="display:none" id="rowinforapporticontabili">
				                                <div class="col-md-12 text-center">
					                                <h6 style="font-weight:bold; color:#D9543F; "><asp:Localize meta:resourcekey="SezioneRapportiContabiliAggiuntaModifica" runat="server">
                                                        Per aggiungere altri rapporti contabili o modificare quelli presenti accedere alla sezione 'Rapporti Contabili'
                                                    </asp:Localize></h6>
				                                </div>
			                                </div>
                                           

		                                </div>

	                                </div>
                                </div>

                                <div class="row" style="padding-right: 15px;">
	                                <div class="col-lg-12 border_si border_si__no_shadow">
                                        <div id="gestioneDocumenti" style="display: block;">
                                            <div class="row">
				                                <div class="col-lg-12 col-md-12 col-sm-12">
					                                <div class="form-horizontal">
						                                <div class="form-group">
							                                
                                                                <div class="row">
				                                                <div class="col-md-12">
					                                                <h5 class="agronica-card-title card-title"><asp:Localize meta:resourcekey="Documenti" runat="server">Documenti</asp:Localize></h5>
                                                                    <div class="btn btn-success buttonClass" title='<asp:Localize meta:resourcekey="PatentinoEAltriDocumenti" runat="server">Patentino e altri documenti</asp:Localize>' id="btn_apri_gestione_documenti">
                                                                        <span class="fa fa-folder-open"></span><asp:Localize meta:resourcekey="PatentinoEAltriDocumenti" runat="server">Patentino e altri documenti</asp:Localize>
                                                                    </div>
                                                                    <!-- ANNA 03/24 -- WIP ALLINEAMENTO PASSANDO DA DOCUMENTALE. INTERROTTO, RIPRENDERE IN FUTURO! -->
                                                                    <%--<div class="btn btn-success buttonClass" id="btn_apri_ricerca_documenti">
                                                                        <span class="fa fa-info">Visualizza Patentini</span>
                                                                    </div>
                                                                    <div class="btn btn-success buttonClass" id="btn_apri_edit_documenti">
                                                                        <span class="fa fa-paperclip">Inserisci Patentino</span>
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

                            </div>
                        </div>

                    </div>

                </div>

                <!-- TAB 2 -->
                <div class="tab-pane" id="tab_indirizzi">
                    <div class="table--alone" id="grdIndirizzi"></div>
                </div>

                <!-- TAB 3 -->
                <div class="tab-pane" id="tab_rapporti_contabili">
                    <!-- Griglia parametri qualitativi -->
                    <div class="table--alone" id="griglia_rapporti_contabili"></div>
                </div>

                 <!-- TAB 3 -->
                <div class="tab-pane" id="tab_rubrica">
                    <!-- Griglia parametri qualitativi -->
                    <div class="table--alone" id="griglia_rubrica"></div>
                </div>
                
                 <!-- TAB 3 -->
                <div class="tab-pane" id="tab_indirizzi_new">
                    <!-- Griglia parametri qualitativi -->
                    <div class="table--alone" id="griglia_indirizzi"></div>
                </div>

                 <!-- TAB 4 -->
                <div class="tab-pane" id="tab_efatt">
                    <div class="jumbotron">

                        <div id="datiEFatt" class="row">
                            <div class="col-lg-12 border_si border_si__no_shadow">
                                <div class="row">

                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Tipo_Contatto" for="Ddl_Tipo_Contatto">
                                                        <asp:Localize meta:resourcekey="TipologiaContatto" runat="server">Tipologia Contatto</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="Ddl_Tipo_Contatto" runat="server" CssClass="form-control">
                                                        <asp:ListItem Selected="True" Value="-1" Text=""> </asp:ListItem>
                                                        <asp:ListItem Value="1" meta:resourcekey="ContattoPrivato" Text="Contatto Privato"></asp:ListItem>
                                                        <asp:ListItem Value="2" meta:resourcekey="PubblicaAmministrazione" Text="Pubblica Amministrazione"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Rappresentante_Fiscale">
                                                        <asp:Localize meta:resourcekey="RappresentanteFiscale" runat="server">Rappresentante Fiscale</asp:Localize>
                                                    </span>
                                                    <input type="text" id="Ddl_Rappresentante_Fiscale" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    </div>

                                 <div class="row">
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Pec" for="Txt_Pec"><asp:Localize meta:resourcekey="Pec" runat="server">Pec</asp:Localize></span>
                                                    <asp:TextBox ID="Txt_Pec" MaxLength="50" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Codice_SDI" for="Txt_Codice_SDI"><asp:Localize meta:resourcekey="CodiceSDI" runat="server">Codice SDI</asp:Localize></span>
                                                    <asp:TextBox ID="Txt_Codice_SDI" MaxLength="10" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                 <div class="row">

                                     <div class="row">
                                        <div class="col-md-12">
                                            <h5 class="agronica-card-title card-title">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,DichiarazioneIntento %>" runat="server">Dichiarazione di Intento</asp:Localize>
                                            </h5>
                                        </div>
                                     </div>

                                    <div class="col-lg-6 col-md-6 col-sm-6">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_dich_intenti_protocollo" for="txt_dich_intenti_protocollo">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,RiferimentoTesto %>" runat="server">N° di Protocollo</asp:Localize>
                                            </label>
                                            <asp:TextBox ID="txt_dich_intenti_protocollo" MaxLength="50" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_dich_intenti_data" for="txt_dich_intenti_data">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,RiferimentoData %>" runat="server">Data Protocollo</asp:Localize>
                                            </label>
                                            <input ID="txt_dich_intenti_data" name="txt_dich_intenti_data" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                                        </div>
                                    </div>


                                </div>
                            </div>
                        </div>
                        
                    </div>
                </div>


                 <!-- TAB 3 -->
                <div class="tab-pane" id="tab_costi_new">
                    <!-- Griglia parametri qualitativi -->
                    <div class="table--alone" id="griglia_costi"></div>
                </div>

                 <!-- TAB 3 -->
                <div class="tab-pane" id="tab_altri_dati">
                    <div class="jumbotron">
                         <div id="altriDati" class="row">
                             <div class="col-lg-12 border_si border_si__no_shadow">

                                 <div class="row">
                                    <div class="col-md-12">
                                        <h5 class="agronica-card-title card-title"><asp:Localize meta:resourcekey="CodiciDAA" runat="server">Codici DAA</asp:Localize></h5>
                                    </div>
                                </div>

                                  <div class="row">

                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_altri_cod_accisa" for="Txt_Cod_Accisa"><asp:Localize meta:resourcekey="CodiceAccisa" runat="server">Codice Accisa</asp:Localize></span>
                                                        <asp:TextBox ID="Txt_Cod_Accisa" MaxLength="10" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-12" id="colCod_UA">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_altri_cod_ua" for="Txt_Cod_UA"><asp:Localize meta:resourcekey="CodiceUA" runat="server">Codice UA</asp:Localize></span>
                                                        <asp:TextBox ID="Txt_Cod_UA" MaxLength="10" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-12" id="colConto_Gar">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_altri_cod_conto_gar" for="Txt_Cod_Conto_Gar"><asp:Localize meta:resourcekey="CodiceContoGaranzia" runat="server">Codice Conto Garanzia</asp:Localize></span>
                                                        <asp:TextBox ID="Txt_Cod_Conto_Gar" MaxLength="10" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                </div>


                                 <div class="row">

                                        <div class="col-lg-8 col-md-8 col-sm-12" id="col_Orig_Dest">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_altri_origine_spedizione"><asp:Localize meta:resourcekey="OrigineDellaSpedizion" runat="server">Origine della Spedizione</asp:Localize></span>
                                                        <input type="text" id="ddl_Origine_Spedizione" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        
                                 </div>

                                 <div class="row" id="rowDepFiscale">


                                     <div class="col-lg-8 col-md-8 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_altri_rif_dep_fisc" for="Txt_Rif_Dep_Fisc"><asp:Localize meta:resourcekey="RiferimentoDepositoFiscale" runat="server">Riferimento Deposito Fiscale</asp:Localize></span>
                                                        <asp:TextBox ID="Txt_Rif_Dep_Fisc" MaxLength="10" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                 </div>

                                  <div class="row">

                                      <div class="col-lg-8 col-md-8 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_altri_cod_uff_dogan"><asp:Localize meta:resourcekey="CodiceUfficioDoganale" runat="server">Codice Ufficio Doganale</asp:Localize></span>
                                                        <input type="text" id="ddl_Cod_Uff_Dogan" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                  </div>

                                  <div class="row">

                                       <div class="col-lg-8 col-md-8 col-sm-12" id="col_Tipo_Destinazione">
                                            <div class="form-horizontal">
                                               <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_altri_cod_tipo_dest"><asp:Localize meta:resourcekey="TipologiaDestinazione" runat="server">Tipologia Destinazione</asp:Localize></span>
                                                        <input type="text" id="ddl_Cod_Tipo_Destinazione" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                  </div>

                            </div>

                             
                             <div class="col-lg-12 border_si border_si__no_shadow">

                                 <div class="row">
                                    <div class="col-md-12">
                                        <h5 class="agronica-card-title card-title">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Note %>" runat="server">Note</asp:Localize>
                                        </h5>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_altri_Note" for="Txt_Altri_Dati_Note">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Note %>" runat="server">Note</asp:Localize>:
                                            </label>
                                            <textarea id="Txt_Altri_Dati_Note" class="form-control k-content" rows="3"></textarea>
                                        </div>
                                    </div>
                                </div>

                                 <div class="row">
                                        <div class="col-lg-12 col-sm-12">
											<div class="form-horizontal">
						                        <div class="form-group">
													<div class="input-group">
                                                        <label class="input-group-addon alert-info lbl_required" id="lbl_altri_note_operazioni" for="multiselNoteOperazioni">
                                                            <asp:Localize meta:resourcekey="Documenti" runat="server">Documenti</asp:Localize>:
                                                        </label>
                                                        <!-- i18n attributo data nella select -->
                                                        <select name="multiselNoteOperazioni" multiple="multiple" ID="id_multiselNoteOperazioni" class="form-control" data-placeholder="Tutti"></select>
													</div>
												</div>
											</div>
										</div>

                                 </div>


                             </div>

                             
                             <div class="col-lg-12 border_si border_si__no_shadow">

                                     <div class="row">
                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <label class="input-group-addon alert-info" id="lbl_altri_Note2" for="Txt_Altri_Dati_Note2"><asp:Localize meta:resourcekey="NoteSupplementari" runat="server">Note Supplementari</asp:Localize>:</label>
                                                <textarea id="Txt_Altri_Dati_Note2" class="form-control k-content" rows="3"></textarea>
                                            </div>
                                        </div>
                                    </div>

                                 <div class="row">

                                     <div class="row">
                                        <div class="col-lg-12 col-sm-12">
											<div class="form-horizontal">
						                        <div class="form-group">
													<div class="input-group">
                                                        <label class="input-group-addon alert-info lbl_required" id="lbl_altri_note_operazioni2" for="multiselNoteOperazioni2">
                                                            <asp:Localize meta:resourcekey="Documenti" runat="server">Documenti</asp:Localize>:
                                                        </label>
                                                        <!-- i18n attributo data nella select -->
                                                        <select name="multiselNoteOperazioni2" multiple="multiple" ID="id_multiselNoteOperazioni2" class="form-control" data-placeholder="Tutti"></select>
													</div>
												</div>
											</div>
										</div>

                                 </div>

                                 </div>

                             </div>
                             
                             <div class="col-lg-12 border_si border_si__no_shadow" id="divCaloPeso" >
                                <div class = "row">
                                    <div class = "row">
                                        <div class="col-lg-6 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon alert-info" id="lblCaloPeso" for="Txt_Calo_Peso">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CaloPeso %>" runat="server"></asp:Localize>
                                                        </label>
                                                        <input name="Txt_Calo_Peso" id="Txt_Calo_Peso" class="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div> 
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="lblCoeffCaloPeso" for="Txt_Coeff_Calo_Peso">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CoeffCaloPeso %>" runat="server"></asp:Localize>
                                                    </label>
                                                    <input name="Txt_Coeff_Calo_Peso" id="Txt_Coeff_Calo_Peso" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                  

                                <%-- <div class="col-lg-6 col-md-12 col-sm-12">
                                     <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_caloPeso" for="Txt_Calo_Peso"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CaloPeso %>" runat="server"></asp:Localize></span>
                                                <asp:TextBox ID="Txt_Calo_Peso" MaxLength="10" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                 </div>--%>

                             </div>
                             
                         </div>

                    </div>
                </div>


                
                <div class="tab-pane" id="tab_dettagli_contabili">
                    <div class="jumbotron">
                         <div id="dettagliContabili" class="row">

                            <div class="col-lg-12 border_si border_si__no_shadow">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h5 class="agronica-card-title card-title"><asp:Localize meta:resourcekey="CoordinateBancarie" runat="server">Coordinate Bancarie</asp:Localize></h5>
                                    </div>
                                </div>
                                <!-- Griglia liquidita (coordinate bancarie) -->
                                <div class="table--alone" id="griglia_liquidita"></div>

                            </div>

                             <div hidden class="col-lg-12 border_si border_si__no_shadow">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h5 class="agronica-card-title card-title"><asp:Localize meta:resourcekey="ContiDiretti" runat="server">Conti Diretti</asp:Localize></h5>
                                    </div>
                                </div>
                                 <div class="jumbotron">
                                    <div class="row">
                                        <!-- Griglia liquidita (coordinate bancarie) -->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="griglia_conti"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                             <div class="row" id="tabConti">
                                 <div class="col-sm-12 border_si border_si__no_shadow">
                                     <!-- LEFT -->
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <div class="row">

                                        <div class="col-lg-6 col-md-12 col-sm-12">
										    <div class="form-horizontal">
						                        <div class="form-group">
                                                    <div class="input-group">
						                                <label class="input-group-addon alert-info" id="lblSconto" for="idScontoCliente"><asp:Localize meta:resourcekey="ScontoCliente" runat="server">Sconto Cliente</asp:Localize></label>
						                                <input name="idScontoCliente" id="idScontoCliente" class="form-control" />
					                                </div>
											    </div>
										    </div>
									    </div>

                                    </div>

                                    <div class="row">

                                        <div class="col-lg-6 col-md-12 col-sm-12">
										        <div class="form-horizontal">
						                            <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id=""><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScontiAddizionali %>" runat="server"></asp:Localize></span>
						                                    <input name="idScontoAdd1" id="idScontoAdd1" class="form-control" />
					                                    </div>
											        </div>
										        </div>
									        </div>

                                          <div class="col-lg-2 col-md-12 col-sm-12">
										        <div class="form-horizontal">
						                            <div class="form-group input-inline__without-label">
                                                        <div class="input-group">
						                                    <input name="idScontoAdd2" id="idScontoAdd2" class="form-control" />
					                                    </div>
											        </div>
										        </div>
									        </div>

                                          <div class="col-lg-2 col-md-2 col-sm-12">
										    <div class="form-horizontal">
						                        <div class="form-group input-inline__without-label">
                                                    <div class="input-group">
						                                <input name="idScontoAdd3" id="idScontoAdd3" class="form-control" />
					                                </div>
											    </div>
										    </div>
									    </div>

                                         <div class="col-lg-2 col-md-2 col-sm-12">
										    <div class="form-horizontal">
						                        <div class="form-group input-inline__without-label">
                                                    <div class="input-group">
						                                <input name="idScontoAddTot" id="idScontoAddTot" class="form-control" />
					                                </div>
											    </div>
										    </div>
									    </div>


                                    </div>

                                    <div class="row">

                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
					                            <div class="input-group calc-width">
                                                    <label class="input-group-addon alert-info" id="lblddlivadefault" for="ddl_iva_default"><asp:Localize meta:resourcekey="IvaDefault" runat="server">Iva Default</asp:Localize></label>
                                                    <input type="text" id="ddl_iva_default" class="form-control " />
					                            </div>
                                            </div>
				                        </div>

                                    </div>

                                    <div class="row">

                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
					                            <div class="input-group calc-width">
                                                    <label class="input-group-addon alert-info" id="lblddlcontoecodefault" for="ddl_contoEco_default" ><asp:Localize meta:resourcekey="ContoEcoDefault" runat="server">Conto Economico Default</asp:Localize></label>
                                                    <input type="text" id="ddl_contoEco_default" class="form-control " style="width: -webkit-fill-available;" onchange="ddlContoEconChange()" />
					                            </div>
                                            </div>
				                        </div>

                                    </div>

                                    <div class="row">

                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
					                            <div class="input-group calc-width">
                                                    <label class="input-group-addon alert-info" id="lblddlcontopatdefault" for="ddl_contoPat_default"><asp:Localize meta:resourcekey="ContoPatDefault" runat="server">Conto Patrimoniale Default</asp:Localize></label>
                                                    <input type="text" id="ddl_contoPat_default" class="form-control " onchange="ddlContoPatChange()"/>
					                            </div>
                                            </div>
				                        </div>

                                    </div>

                                    <div class="row">

                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                               <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_agente">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Agente %>" runat="server">Agente</asp:Localize>
                                                        </span>
                                                        <input type="text" id="ddl_agente" class="form-control " />
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
                                                        <span class="input-group-addon alert-info" id="lbl_capo_area">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CapoArea %>" runat="server">Capo Area</asp:Localize>
                                                        </span>
                                                        <input type="text" id="ddl_capo_area" class="form-control " />
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
                                                        <span class="input-group-addon alert-info" id="lbl_vettore">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Vettore %>" runat="server">Vettore</asp:Localize>
                                                        </span>
                                                        <input type="text" id="ddl_vettore" class="form-control " />
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
                                                        <span class="input-group-addon alert-info" id="lbl_indirizzo_fatturazione"><asp:Localize meta:resourcekey="IndirizzoFatturazione" runat="server">Indirizzo Fatturazione</asp:Localize></span>
                                                        <input type="text" id="ddl_indirizzo_fatturazione" class="form-control " />
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
                                                        <span class="input-group-addon alert-info" id="lbl_fatturazione_automatica"><asp:Localize meta:resourcekey="FatturazioneAutomatica" runat="server">Fatturazione Automatica</asp:Localize></span>
                                                        <input type="text" id="ddl_fatturazione_automatica" class="form-control " />
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
                                                        <span class="input-group-addon alert-info" id="lbl_documento_fatturazione"><asp:Localize meta:resourcekey="DocumentoFatturazione" runat="server">Documento Fatturazione</asp:Localize></span>
                                                        <input type="text" id="ddl_documento_fatturazione" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                 <!-- RIGHT -->
                                <div class="col-lg-6 col-md-12 col-sm-12" style="display:block">
                                    <div class="row">

                                         <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                               <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_modalita_pagamento">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ModalitàPagamento %>" runat="server"></asp:Localize>
                                                        </span>
                                                        <input type="text" id="ddl_modalita_pagamento" class="form-control " />
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
                                                        <span class="input-group-addon alert-info" id="lbl_iban_default"><asp:Localize meta:resourcekey="IBANDefault" runat="server">IBAN Default</asp:Localize></span>
                                                        <input type="text" id="ddl_iban_default" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="row">

                                        <div class="col-lg-6 col-md-12 col-sm-12">
										    <div class="form-horizontal">
						                        <div class="form-group">
                                                    <div class="input-group">
						                                <label class="input-group-addon alert-info" id="lblProvvigioneAgente" for="idProvvigioneAgente"><asp:Localize meta:resourcekey="ProvvigioneAgente" runat="server">Provvigione Agente</asp:Localize></label>
						                                <input name="idProvvigioneAgente" id="idProvvigioneAgente" class="form-control" />
					                                </div>
											    </div>
										    </div>
									    </div>

                                    </div>

                                    <div class="row">

                                        <div class="col-lg-6 col-md-12 col-sm-12">
										    <div class="form-horizontal">
						                        <div class="form-group">
                                                    <div class="input-group">
						                                <label class="input-group-addon alert-info" id="lblProvvigioneCapoArea" for="idProvvigioneACapoArea"><asp:Localize meta:resourcekey="ProvvigioneCapoArea" runat="server">Provvigione Capo Area</asp:Localize></label>
						                                <input name="idProvvigioneACapoArea" id="idProvvigioneACapoArea" class="form-control" />
					                                </div>
											    </div>
										    </div>
									    </div>

                                    </div>

                                    <div class="row">
                                        <div class="col-lg-8 col-md-9 col-sm-12">
                                            <div class="form-horizontal">
                                               <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_listino_prezzi_acq"><asp:Localize meta:resourcekey="ListinoPrezziAcquisto" runat="server">Listino Prezzi Acquisto</asp:Localize></span>
                                                        <input type="text" id="ddl_listino_prezzi_acq" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-5 col-sm-12 text-center">
		                                    <div class="btn btn-success buttonClass button--inline__without-label" id="btn_associa_listini_acq">
			                                    <span class="fa fa-folder-open"></span><asp:Localize meta:resourcekey="AssociaContattiListini_Acq" runat="server">Associa Listini Acquisto</asp:Localize>
		                                    </div>
	                                    </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-8 col-md-9 col-sm-12">
                                            <div class="form-horizontal">
                                               <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_listino_prezzi_ven"><asp:Localize meta:resourcekey="ListinoPrezziVendita" runat="server">Listino Prezzi Vendita</asp:Localize></span>
                                                        <input type="text" id="ddl_listino_prezzi_ven" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-5 col-sm-12 text-center">
		                                    <div class="btn btn-success buttonClass button--inline__without-label" id="btn_associa_listini_ven">
			                                    <span class="fa fa-folder-open"></span><asp:Localize meta:resourcekey="AssociaContattiListini_Ven" runat="server">Associa Listini Vendita</asp:Localize>
		                                    </div>
	                                    </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                               <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_gestione_vettore"><asp:Localize meta:resourcekey="GestioneVettore" runat="server">Gestione Vettore</asp:Localize></span>
                                                        <input type="text" id="ddl_gestione_vettore" class="form-control " />
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
                                                        <span class="input-group-addon alert-info" id="lbl_referenteConferimento">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, referenteConferimento %>" runat="server">Referente Conferimento</asp:Localize>
                                                        </span>
                                                        <input type="text" id="ddl_referenteConferimento" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                                 </div>
                             </div>

                             <div class="col-lg-12 border_si border_si__no_shadow">

                                    <div class="row">
                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                               <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_test">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DestinazioneDiversa %>" runat="server"></asp:Localize>
                                                        </span>
                                                        <input type="text" name="ddl_destinazione_diversa" id="ddl_test" class="form-control " />
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
                                                        <span class="input-group-addon alert-info" id="lbl_ind_destinazione_diversa"><asp:Localize meta:resourcekey="IndirizzoDestinazioneDiversa" runat="server">Indirizzo Destinazione Diversa</asp:Localize></span>
                                                        <input type="text" id="ddl_ind_destinazione_diversa" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane" id="tab_memo">
                    <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_Memo" for="Txt_Memo">
                                                <asp:Localize meta:resourcekey="Memo" runat="server">Memo</asp:Localize>:
                                            </label>
                                            <textarea id="Txt_Memo" class="form-control k-content" rows="20"></textarea>
                                        </div>
                                    </div>
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

     <!-- dialogs varie -->
    <div id="nuovoTipoIndirizzoWindow" class="panel-group" style="display:block;">
        <div id="indirizziTipo" class="row">

            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="row">
                    <div class="col-lg-11 col-md-11 text-right">
                        <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura = True) And (XTipoOperazione <> enum_TipoOperazioneDB.Lettura) Then%>

                           <%-- <div class="btn btn-success" onclick="aggiornaindirizzitipo(false);" style="margin-bottom: 20px; margin-top: 10px;">
                                <i class="fa fa-floppy-o"></i><asp:localize text="<%$ resources: agronicaagenda_2010,salvaecontinua %>" runat="server">salva e continua</asp:localize>
                            </div>--%>

                            <div class="btn btn-success xi-btn-primary" onclick="AggiornaIndirizziTipo(true);" style="margin-bottom: 20px; margin-top: 10px;">
                                <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                            </div>

                        <%End If %>
                    </div>
                </div>
            </div>

            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="row">
                        <div class="jumbotron">
                            <div class="row">
                            <!-- Griglia tipi indirizzo -->
                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                <div id="griglia_indirizzi_tipo"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="row">
                    <div class="col-lg-11 col-md-11 text-right">
                        <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura = True) And (XTipoOperazione <> enum_TipoOperazioneDB.Lettura) Then%>

                           <%-- <div class="btn btn-success" onclick="AggiornaIndirizziTipo(false);" style="margin-bottom: 20px; margin-top: 10px;">
                                <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                            </div>--%>

                            <div class="btn btn-success xi-btn-primary" onclick="AggiornaIndirizziTipo(true);" style="margin-bottom: 20px; margin-top: 10px;">
                                <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                            </div>

                        <%End If %>
                    </div>
                </div>
            </div>

        </div>
    </div>
    
     <div id="ucAssociazioneListini" style="display:none">
        <uc1:ListinixContattiUC ClientIDMode="Static"  id="ListinixContatti" runat="server"></uc1:ListinixContattiUC>
     </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoLettura" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura" runat="server" />
    <input type="hidden" id="hf_UtenteAbiliato_Modifica_Associazione_Listini" runat="server" />
    <input type="hidden" id="hf_TipoOperazioneContatto" runat="server" />
    <input type="hidden" id="hf_AperturaDaPoup" runat="server" />
    <input type="hidden" id="hf_ApriDatiPatentino" runat="server" />
    <input type="hidden" id="hf_Cod_Rapporto" runat="server" />
    <input type="hidden" id="hf_Convenevoli" runat="server" />
    <input type="hidden" id="hf_RappresentanteFiscale" runat="server" />
    <input type="hidden" id="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" runat="server" />
    <input type="hidden" id="hf_note" runat="server" />
    <input type="hidden" id="hf_note2" runat="server" />
    <input type="hidden" id="hf_noteOperazioni" runat="server" />
    <input type="hidden" id="hf_noteOpererazioni2" runat="server" />
    <input type="hidden" id="hf_caloPeso" runat="server" />
    <input type="hidden" id="hf_coeffCaloPeso" runat="server" />
    <input type="hidden" id="hf_tipo_destinazione" runat="server" />
    <input type="hidden" id="hf_ufficioDogane" runat="server" />
    <input type="hidden" id="hf_origineDestinazione" runat="server" />
    <input type="hidden" id="hf_Piva_Azienda" runat="server" />
    <input type="hidden" id="hf_dettCont_sconto_cliente" runat="server" />
    <input type="hidden" id="hf_dettCont_sconto_add1" runat="server" />
    <input type="hidden" id="hf_dettCont_sconto_add2" runat="server" />
    <input type="hidden" id="hf_dettCont_sconto_add3" runat="server" />
    <input type="hidden" id="hf_dettCont_provvigione_agente" runat="server" />
    <input type="hidden" id="hf_dettCont_provvigione_capoarea" runat="server" />

    <input type="hidden" id="hf_dettCont_agente_cod" runat="server" />
    <input type="hidden" id="hf_dettCont_referenteConferimento_cod" runat="server" />
    <input type="hidden" id="hf_dettCont_capoarea_cod" runat="server" />
    <input type="hidden" id="hf_dettCont_vettore_cod" runat="server" />

    <input type="hidden" id="hf_dettCont_gestionevettore_cod" runat="server" />
    <input type="hidden" id="hf_dettCont_iva_default" runat="server" />
    <input type="hidden" id="hf_dettCont_conto_economico_default" runat="server" />
    <input type="hidden" id="hf_dettCont_conto_patrimoniale_default" runat="server" />
    <input type="hidden" id="hf_dettCont_iban_default" runat="server" />
    <input type="hidden" id="hf_dettCont_mod_pag_default" runat="server" />
    <input type="hidden" id="hf_dettCont_fatturazione_automatica" runat="server" />
    <input type="hidden" id="hf_dettCont_documento_fatturazione" runat="server" />
    <input type="hidden" id="hf_dettCont_destinazione_diversa" runat="server" />
    <input type="hidden" id="fh_dettCont_tipo_indirizzo_default_destinazione_diversa" runat="server" />
    <input type="hidden" id="hf_dettCont_listini_prezzi_acq" runat="server" />
    <input type="hidden" id="hf_dettCont_listini_prezzi_ven" runat="server" />
    <input type="hidden" id="hf_dettCont_tipo_ind_default" runat="server" />
    <input type="hidden" id="hf_OpzioniContatti" runat="server" />

    <input type="hidden" id="hf_Memo" runat="server" />

    <input type="hidden" id="hf_xCodContatto" runat="server" />
    <input type="hidden" id="hf_fittizio" runat="server" />
    <input type="hidden" id="hf_IDCF" runat="server" />
    <input type="hidden" id="hf_ImpresaGias" runat="server" />
    <input type="hidden" id="hf_apertodaGiasNG" runat="server" />
    <input type="hidden" id="hf_impedisci_eliminazione_contatti" runat="server" />
    <input type="hidden" id="hf_EUDR" runat="server" />

    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("New_Contatto_Edit_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("New_Contatto_Edit.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("New_Contatto_Edit_ws_client.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("New_Contatto_Edit_globali.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>

    <script type="text/javascript">

        <%--var ImgBtn_SalvaTutto_ClienId = "<%=ImgBtn_SalvaTutto.ClientID %>"--%>
        var ImgBtn_CF_ClientId = "#<%=ImgBtn_CF.ClientID %>";
        var ImgBtn_PIVA_ClientId = "#<%=ImageButton1.ClientID %>";

        var jsIndirizzi = <%=jsIndirizzi%>;
        var jsIndirizziNew = <%=jsIndirizziNew%>;
        var jsRubrica = <%=jsRubrica%>;
        var jsCostiNew = <%=jsCostiNew%>;
        var jsRapporti = <%=jsRapporti%>;
        var jsLiquidita = <%=jsLiquidita%>;
        var jsConti = <%=jsConti%>;

        var Controls = {
            xCodContatto: "#<%=hf_xCodContatto.ClientID %>",
            Visibilita: "#<%=Cmb_CentriAziendali.ClientID %>",
            IDCF: "#<%=hf_IDCF.ClientID %>",
            ImpresaGias: "#<%=hf_ImpresaGias.ClientID %>",
            Fittizio: "#<%=hf_fittizio.ClientID %>",
            PIVA: "#<%=Txt_Piva.ClientID %>",
            Nome: "#<%=Txt_Nome.ClientID %>",
            Cognome: "#<%=Txt_Cognome.ClientID %>",
            Nome_Breve_Persona: "#<%=Txt_Nome_Breve_Persona.ClientID %>",
            DataNascita: "#<%=Txt_DataNascita.ClientID %>",
            RagioneSociale: "#<%=Txt_Rag_Soc.ClientID %>",
            Nome_Breve_Azienda: "#<%=Txt_Nome_Breve_Azienda.ClientID %>",
            CF: "#<%=Txt_CF.ClientID %>",
            CF_Estero: "#<%=Txt_CF_Estero.ClientID %>",
            TipoUtente: "#<%=RBL_TipoUtente.ClientID %>",
            Sesso: "#<%=ddl_Sesso.ClientID %>",
            Badge: "#<%=Txt_Badge.ClientID %>",
            EUDR: "#<%=hf_EUDR.ClientID %>",
            TipologiaContatto: "#<%=Ddl_Tipo_Contatto.ClientID %>",
            Pec: "#<%=Txt_Pec.ClientID %>",
            CodiceSDI: "#<%=Txt_Codice_SDI.ClientID %>",
            ImgBtn_CF: "#<%=ImgBtn_CF.ClientID %>",
            ImgBtn_AnnullaTutto: "#<%=ImgBtn_AnnullaTutto.ClientID %>",
            TipoSalva: "#<%=tipo_salva.ClientID() %>",
            PaginaRedirect: "#<%=hdPaginaRedirect.ClientID() %>",
            TipoOperazioneContatto: "#<%=hf_TipoOperazioneContatto.ClientID() %>",
            ConvenevoleSalvato: "#<%=hf_Convenevoli.ClientID() %>",
            RappFiscaleSalvato: "#<%=hf_RappresentanteFiscale.ClientID() %>",
            DichiarazioneIntentoProtocollo: "#<%=txt_dich_intenti_protocollo.ClientID %>",
            DichiarazioneIntentoDataProtocollo: "#<%=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.ClientID() %>",
            AperturaDaPopup: "#<%=hf_AperturaDaPoup.ClientID() %>",
            PopupCodRapporto: "#<%=hf_Cod_Rapporto.ClientID() %>",
            AperturaDatiPatentino: "#<%=hf_ApriDatiPatentino.ClientID() %>",
            Note: "#<%=hf_note.ClientID() %>",
            Note2: "#<%=hf_note2.ClientID() %>",
            NoteOperazioni: "#<%=hf_noteOperazioni.ClientID() %>",
            NoteOperazioni2: "#<%=hf_noteOpererazioni2.ClientID() %>",
            CaloPeso: "#<%=hf_caloPeso.ClientID() %>",
            CoeffCaloPeso: "#<%=hf_coeffCaloPeso.ClientID() %>",
            CodiceAccisa: "#<%=Txt_Cod_Accisa.ClientID() %>",
            CodiceUA: "#<%=Txt_Cod_UA.ClientID() %>",
            CodContoGaranzia: "#<%=Txt_Cod_Conto_Gar.ClientID() %>",
            TipoDestinazione: "#<%=hf_tipo_destinazione.ClientID() %>",
            OrigineDestinazione: "#<%=hf_origineDestinazione.ClientID() %>",
            UfficioDogane: "#<%=hf_ufficioDogane.ClientID() %>",
            PivaAzienda: "#<%=hf_Piva_Azienda.ClientID() %>",
            RifDepositoFiscale: "#<%=Txt_Rif_Dep_Fisc.ClientID() %>",
            DettCont_Sconto_Cliente: "#<%=hf_dettCont_sconto_cliente.ClientID() %>",
            DettCont_Sconto_Add1: "#<%=hf_dettCont_sconto_add1.ClientID() %>",
            DettCont_Sconto_Add2: "#<%=hf_dettCont_sconto_add2.ClientID() %>",
            DettCont_Sconto_Add3: "#<%=hf_dettCont_sconto_add3.ClientID() %>",
            DettCont_Provvigione_Capo_area: "#<%=hf_dettCont_provvigione_capoarea.ClientID() %>",
            DettCont_Provvigione_Agente: "#<%=hf_dettCont_provvigione_agente.ClientID() %>",
            DettCont_Agente_Cod: "#<%=hf_dettCont_agente_cod.ClientID() %>",
            DettCont_referenteConferimento_cod: "#<%=hf_dettCont_referenteConferimento_cod.ClientID() %>",   
            DettCont_CapoArea_Cod: "#<%=hf_dettCont_capoarea_cod.ClientID() %>",
            DettCont_Vettore_Cod: "#<%=hf_dettCont_vettore_cod.ClientID() %>",
            DettCont_GestioneVettore_Cod: "#<%=hf_dettCont_gestionevettore_cod.ClientID() %>",
            DettCont_Iva_Default: "#<%=hf_dettCont_iva_default.ClientID() %>",
            DettCont_Conto_Economico_Default: "#<%=hf_dettCont_conto_economico_default.ClientID() %>",
            DettCont_Conto_Patrimoniale_Default: "#<%=hf_dettCont_conto_patrimoniale_default.ClientID() %>",
            DettCont_Iban_Default: "#<%=hf_dettCont_iban_default.ClientID() %>",
            DettCont_Mod_Pag_Default: "#<%=hf_dettCont_mod_pag_default.ClientID() %>",
            DettCont_Fatturazione_Automatica: "#<%=hf_dettCont_fatturazione_automatica.ClientID() %>",
            DettCont_Documento_Fatturazione: "#<%=hf_dettCont_documento_fatturazione.ClientID() %>",
            DettCont_Destinazione_Diversa: "#<%=hf_dettCont_destinazione_diversa.ClientID() %>",
            DettCont_Tipo_Indirizzo_Default_Destinazione_Diversa: "#<%=fh_dettCont_tipo_indirizzo_default_destinazione_diversa.ClientID() %>",
            DettCont_Listino_Prezzi_Acq: "#<%=hf_dettCont_listini_prezzi_acq.ClientID() %>",
            DettCont_Listino_Prezzi_Ven: "#<%=hf_dettCont_listini_prezzi_ven.ClientID() %>",
          
            
            DettCont_Tipo_Indirizzo_Default: "#<%=hf_dettCont_tipo_ind_default.ClientID() %>",
            OpzioniContatti: "#<%=hf_OpzioniContatti.ClientID() %>",
            UtenteAbiliato_Modifica_Associazione_Listini: "#<%=hf_UtenteAbiliato_Modifica_Associazione_Listini.ClientID() %>",
            Memo: "#<%=hf_Memo.ClientID() %>"
        }

        var impedisci_eliminazione_contatti = "#<%=hf_impedisci_eliminazione_contatti.ClientID %>";

    </script>
        <script type="text/javascript" language="javascript">

        function gestisciValore(valore) {
            try {
                window.parent.Contatto_gestisciValore(valore);
            }
            catch (e) {
            }
        }

        function gestisciValore_esci() {
            try {
                window.parent.Contatto_gestisciValore_esci();
            }
            catch (e) {
            }
        }

    </script>

    <script id="templateKendoGestioneTipiIndirizzo" type="text/x-kendo-template">
        <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura = True) And (XTipoOperazione <> enum_TipoOperazioneDB.Lettura) Then%>

        # if (GiasVersioneMaster === "2022") { #
            <div class="vertical-separator"></div>
            <div class="btn btn-success k-grid--button" data-title='<asp:Localize Text="<%$ Resources:GestioneTipiIndirizzo %>" runat="server">Gestione Tipi Indirizzo</asp:Localize>' onclick="GestioneTipiIndirizzo();" style="margin-left: 4px;">
                <i class="icon-options-settings"></i>
            </div>
        # } else { #
            <div class="btn btn-success" onclick="GestioneTipiIndirizzo();" style="margin-left: 4px;">
                <i class="fa fa-home"></i><asp:Localize Text="<%$ Resources:GestioneTipiIndirizzo %>" runat="server">Gestione Tipi Indirizzo</asp:Localize>
            </div>
        # } #
                    
	    <% End If%>
    </script>

    <script id="noDataTemplateConvenevoli" type="text/x-kendo-template">
        <div>
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,NessunElementoTrovato %>" runat="server">Nessun elemento trovato</asp:Localize>. <br/>
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,VuoiAggiungereLElementoX %>" runat="server">Vuoi aggiungere l'elemento -</asp:Localize> '#: instance.filterInput.val() #' ?
        </div>
        <br />
        <button class="btn btn-warning" onclick="aggiungiNuovoConvenevole('#: instance.element[0].id #', '#: instance.filterInput.val() #')">
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,AggiungiElemento %>" runat="server">Aggiungi elemento</asp:Localize>
        </button>
    </script>

    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>

     <asp:UpdatePanel ID="Script_Panel" runat="server" UpdateMode="Always">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
