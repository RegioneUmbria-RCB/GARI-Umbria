<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Fabbricato_Edit.aspx.vb" Inherits="AgroAgenda_2010.Fabbricato_Edit" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .checkbox .btn, .checkbox-inline .btn {
            padding-left: 2em;
            min-width: 8em;
        }

        .checkbox label, .checkbox-inline label {
            text-align: left;
            padding-left: 0.5em;
        }

        .checkbox input[type="checkbox"] {
            float: none;
        }

        #div_stalla_bdn {
            display: none;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row gias-zootecnia-anagrafiche-fabbricato-container">

        <div class="col-lg-12 col-md-12 gias-zootecnia-anagrafiche-fabbricato-header">
            <div class="row">
                <div class="col-lg-10 col-md-10" style="padding: 10px 15px; line-height: 1.6;">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Riferimenti %>" runat="server">Riferimenti</asp:Localize>:
                    <b><asp:Label ID="LblRiferimenti" runat="server" Visible="false"></asp:Label></b>
                    <br />
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Centro %>" runat="server">Centro</asp:Localize>:
                    <b><asp:Label ID="LblCentro" runat="server"></asp:Label></b>
                </div>
                <div class="col-lg-2 col-md-2 text-right">
                    <%--<div class="btn btn-success" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">--%>
                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>

                        <div class="btn btn-success xi-btn-primary btn-save" title="Salva" onclick="ValidaxSubmit();" style="margin-top: 20px; margin-bottom: 20px;">
                            <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Salva %>" runat="server">Salva</asp:Localize>
                        </div>
                        <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                            <button type="button" class="btn btn-success dropdown-toggle btn-operations" data-toggle="dropdown"
                                aria-haspopup="true" aria-expanded="false">
                                <i class="fa fa-chevron-down"></i><span class="sr-only">Toggle Dropdown</span>
                            </button>
                            <ul class="dropdown-menu" style="right: 0; left: auto !important; margin-bottom: 20px;">
                                <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(0); ValidaxSubmit();">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                                </a></li>
                                <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(1); ValidaxSubmit();">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                                </a></li>
                            </ul>
                        <% End If%>
                        <asp:HiddenField ID="tipo_salva" runat="server" />
                        <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" Style="display: none" />
                    <% End If%>
                </div>
            </div>
        </div>
        <div class="col-lg-12 gias-zootecnia-anagrafiche-fabbricato-content" id="tabs" style="margin-bottom: 100px;">
            <ul id="Ul1" class="nav nav-tabs" data-tabs="tabs">
                <li class="active"><a href="#tab_dati_fabbricato" data-toggle="tab">
                    <asp:Localize meta:resourcekey="DatiFabbricato" runat="server">Dati Fabbricato</asp:Localize>
                </a></li>
                <!--<li><a href="#tab_gerarchia" data-toggle="tab">Gerarchia</a></li>-->
                <li><a href="#tab_dati_accessori" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,DatiAccessori %>" runat="server">Dati Accessori</asp:Localize>
                </a></li>
            </ul>
            <div id="my-tab-content" class="tab-content">
                <div class="tab-pane active gias-content-padding-x gias-content-padding-y" id="tab_dati_fabbricato">
                    <div class="jumbotron">

                        <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>

                        <div class="row" id="div_riepilogo_error" style="margin-bottom: 25px;">
                            <div class="col-lg-12 col-md-12">
                                <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CompilareISeguentiCampi %>" runat="server">I seguenti campi devono essere compilati:</asp:Localize></b>
                                <br />
                                <br />
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <ul id="div_riepilogo_error_elenco">
                                    <li class="voce_1"><asp:Localize meta:resourcekey="ObbligatorioDenominazione" runat="server">Il campo <b>Denominazione</b> è da compilare</asp:Localize></li>
                                    <li class="voce_2"><asp:Localize meta:resourcekey="ObbligatorioVia" runat="server">Il campo <b>Via</b> è da compilare</asp:Localize></li>
                                    <li class="voce_3"><asp:Localize meta:resourcekey="ObbligatorioProvincia" runat="server">Il campo <b>Provincia</b> è da selezionare</asp:Localize></li>
                                    <li class="voce_4"><asp:Localize meta:resourcekey="ObbligatorioComune" runat="server">Il campo <b>Comune</b> è da selezionare</asp:Localize></li>
                                    <li class="voce_5"><asp:Localize meta:resourcekey="ObbligatorioCAP" runat="server">Il campo <b>CAP</b> è da compilare</asp:Localize></li>  
                                </ul>
                            </div>
                        </div>

                        <% End If%>

                        <div class="row gias-zootecnia-anagrafiche-fabbricato-content-row">
                            <div class="col-sm-12">
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_Denominazione" for="TxtDenominazione">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Denominazione %>" runat="server">Denominazione</asp:Localize> *</span>
                                                <asp:TextBox ID="TxtDenominazione" runat="server" CssClass="form-control required gias-default-font-size gias-input" aria-describedby="lbl_Denominazione"> </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal gias-zootecnia-anagrafiche-fabbricato-content-row-last-item">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_TipoFabbricato" for="Cmb_TipoFabbricato">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Tipologia %>" runat="server">Tipologia</asp:Localize> *
                                                </span>
                                                <asp:DropDownList ID="Cmb_TipoFabbricato" runat="server" CssClass="form-control required gias-default-font-size gias-input" AutoPostBack="false">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row gias-zootecnia-anagrafiche-fabbricato-content-row gias-zootecnia-anagrafiche-fabbricato-content-row-checkbox">
                            <div class="col-sm-12">
                                <div class="col-sm-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="checkbox" id="chkMagFarm">
                                                <label>
                                                    <asp:CheckBox ID="chk_MagazzinoFarmaci" runat="server" meta:resourcekey="chk_MagazzinoFarmaci" Text="Magazzino Farmaci" />
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row"  id="row_InfoAggiuntive" style="display: none">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_ProprietarioCapi" for="TxtProprietarioCapi">
                                                <%--<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ProprietarioCapi %>" runat="server">P.Iva/Cod.Fisc. Proprietario Capi</asp:Localize>--%>P.Iva/Cod.Fisc. Proprietario Capi *</span>
                                            <asp:TextBox ID="TxtProprietarioCapi" runat="server" CssClass="form-control required" aria-describedby="lbl_ProprietarioCapi"> </asp:TextBox>

                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_CodiceBDN" for="TxtCodiceBDN">Codice Azienda BDN *</span>
                                            <asp:TextBox ID="TxtCodiceBDN" runat="server" CssClass="form-control required" aria-describedby="lbl_CodiceBDN"> </asp:TextBox>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row gias-zootecnia-anagrafiche-fabbricato-content-row">
                            <div class="col-lg-12">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <h5 class="agronica-card-title card-title">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Indirizzo %>" runat="server">Indirizzo</asp:Localize>
                                        </h5>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6 col-md-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_via" for="Txt_Via">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Via %>" runat="server">Via</asp:Localize> *
                                                    </span>
                                                    <asp:TextBox ID="Txt_Via" runat="server" CssClass="form-control required gias-input" aria-describedby="lbl_via"> </asp:TextBox>
                                                    <asp:TextBox ID="Txt_CodIndirizzo_Centro" runat="server" CssClass="form-control" Style="display: none;"> </asp:TextBox>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6">
                                        <div class="form-horizontal gias-zootecnia-anagrafiche-fabbricato-content-row-last-item">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_frazione" for="Txt_Frazione">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Frazione %>" runat="server">Frazione</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_Frazione" runat="server" CssClass="form-control gias-default-font-size gias-input" aria-describedby="lbl_frazione"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div id="div_prov_com">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_Prov" for="dll_Provincia">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Provincia %>" runat="server">Provincia</asp:Localize> *
                                                        </span>
                                                        <asp:DropDownList ID="Cmb_Provincia" runat="server" CssClass="form-control selectpicker required"
                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Prov">
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="Txt_ProCodIstat" runat="server" CssClass="form-control " Style="display: none"> </asp:TextBox>
                                                        <asp:TextBox ID="Txt_ProvinciaSigla" runat="server" CssClass="form-control " Style="display: none"> </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal gias-zootecnia-anagrafiche-fabbricato-content-row-last-item">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_Com" for="ddl_comune">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Comune %>" runat="server">Comune</asp:Localize> *
                                                        </span>
                                                        <asp:DropDownList ID="Cmb_Comune" runat="server" CssClass="form-control selectpicker required"
                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Com">
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="Txt_ComCodIstat" runat="server" CssClass="form-control " Style="display: none"> </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group ">
                                                        <span class="input-group-addon alert-info" id="lbl_cap" for="Txt_CAP">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CAP %>" runat="server">CAP</asp:Localize> *
                                                        </span>
                                                        <asp:TextBox ID="Txt_CAP" runat="server" CssClass="form-control required gias-default-font-size gias-input" aria-describedby="lbl_cap"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal gias-zootecnia-anagrafiche-fabbricato-content-row-last-item">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_stato" for="Txt_Stato">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Stato %>" runat="server">Stato</asp:Localize>
                                                    </span>
                                                    <%--<asp:TextBox ID="Txt_Stato" runat="server" CssClass="form-control  " aria-describedby="lbl_stato"> </asp:TextBox>--%>
                                                    <asp:DropDownList name="cmb_Stato" ID="cmb_Stato" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_stato">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal gias-zootecnia-anagrafiche-fabbricato-content-row-last-item">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_note_ind" for="Txt_Note">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Note %>" runat="server">Note</asp:Localize>
                                                    </span>
                                                    <asp:TextBox ID="Txt_Note" runat="server" CssClass="form-control gias-default-font-size gias-input" aria-describedby="lbl_note_ind"> </asp:TextBox>
                                                    <asp:TextBox ID="Txt_CodIndirizzo" runat="server" CssClass="displaynone"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>


                        <div class="row gias-zootecnia-anagrafiche-fabbricato-content-row">
                            <div class="col-lg-12">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h5 class="agronica-card-title card-title"><asp:Localize meta:resourcekey="IdoneitàFabbricato" runat="server">Idoneità del Fabbricato</asp:Localize></h5>
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
                                        <select id="cbl_idoneita" class="selectpicker" multiple data-hide-disabled="true" data-live-search="true" runat="server">
                                            <%--<option value="1">Caratteristiche di Costruzione</option>
                                                        <option value="2">Separazione Ambienti</option>
                                                        <option value="3">Separazione Prodotti</option>
                                                        <option value="4">Condizioni Igienico-Sanitarie</option>
                                                        <option value="5">Autorizzazione Sanitaria</option>
                                                        <option value="6">HACCP</option>
                                                        <option value="7">Planimetria</option>
                                                        <option value="8">Layout</option>
                                                        <option value="9">Diagrammi di Flusso</option>
                                                        <option value="10">CDX-M004</option>
                                                        <option value="11">Superfici Minime Coperte</option>
                                                        <option value="12">Superfici Minime Scoperte</option>--%>
                                        </select>
                                        <%--    </div>
                                            </div>
                                        </div>--%>
                                    </div>

                                </div>
                            </div>
                        </div>


                        <div class="row gias-zootecnia-anagrafiche-fabbricato-content-row" style="margin-top: 20px;">
                            <div class="col-sm-12">
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_TitoloPossesso" for="Cmb_TitoloPossesso">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,TitoloDiPossesso %>" runat="server">Titolo di Possesso</asp:Localize>
                                                </span>
                                                <asp:DropDownList ID="Cmb_TitoloPossesso" runat="server" CssClass="form-control gias-default-font-size gias-input" AutoPostBack="false">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_Particella" for="Cmb_Particella">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,ParticellaCatastale %>" runat="server">Particella Catastale</asp:Localize>
                                                </span>
                                                <asp:DropDownList ID="Cmb_Particella" runat="server" CssClass="form-control gias-default-font-size gias-input" AutoPostBack="false">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal gias-zootecnia-anagrafiche-fabbricato-content-row-last-item">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_Regolamenti" for="Cmb_Regolamenti">
                                                    <asp:Localize meta:resourcekey="NormaDiRiferimento" runat="server">Norma di Riferimento</asp:Localize>
                                                </span>
                                                <asp:DropDownList ID="Cmb_Regolamenti" runat="server" CssClass="form-control gias-default-font-size gias-input" AutoPostBack="false">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row gias-zootecnia-anagrafiche-fabbricato-content-row" id="div_dettagli_stalla">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Animali" for="Cmb_Animali">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpecieAnimale %>" runat="server">Specie Animale</asp:Localize>
                                            </span>
                                            <asp:DropDownList ID="Cmb_Animali" runat="server" CssClass="form-control" AutoPostBack="false">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_IndirizzoProduttivo" for="Cmb_IndirizzoProduttivo">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,IndirizzoProduttivo %>" runat="server">Indirizzo Produttivo</asp:Localize></span>
                                            <asp:DropDownList ID="Cmb_IndirizzoProduttivo" runat="server" CssClass="form-control" AutoPostBack="false">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_TipoRicovero" for="Cmb_TipoRicovero">
                                                <asp:Localize meta:resourcekey="TipoRicovero" runat="server">Tipo Ricovero</asp:Localize></span>
                                            <asp:DropDownList ID="Cmb_TipoRicovero" runat="server" CssClass="form-control" AutoPostBack="false">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_SottotipoRicovero" for="Cmb_SottotipoRicovero">
                                                <asp:Localize meta:resourcekey="SottotipoRicovero" runat="server">Sottotipo Ricovero</asp:Localize></span>
                                            <asp:DropDownList ID="Cmb_SottotipoRicovero" runat="server" CssClass="form-control" AutoPostBack="false">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_AllevamentoCodice_BDN" for="TxtAllevamentoCodice_BDN">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceASL %>" runat="server">Codice Azienda BDN</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtCodiceStallaBDN" runat="server" CssClass="form-control " aria-describedby="lbl_AllevamentoCodice_BDN"> </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12" style="display: none;">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_AllevamentoIdFiscale_BDN" for="TxtAllevamentoId_BDN">
                                                <asp:Localize meta:resourcekey="CodiceFiscaleAllevamentoBDN" runat="server">Codice Fiscale Allevamento BDN</asp:Localize> </span>
                                            <asp:TextBox ID="TxtIdFiscaleStallaBDN" runat="server" CssClass="form-control " aria-describedby="lbl_AllevamentoIdFiscale_BDN"> </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row gias-zootecnia-anagrafiche-fabbricato-content-row" id="div_stalla_bdn">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize meta:resourcekey="ConfigurazioneStallaBDN" runat="server">Caratteristiche stalla</asp:Localize></h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <input type="hidden" id="kendoGridBDN" runat="server" />
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div id="divKendoConfigurazioni"></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row" id="div_caratt_stalla">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize meta:resourcekey="CaratteristicheStalla" runat="server">Caratteristiche stalla</asp:Localize></h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group" id="div_CmbStalla">
                                                            <span class="input-group-addon alert-info" id="lbl_Caratteristiche" for="Cmb_Caratteristiche">
                                                                <asp:Localize meta:resourcekey="Attributo" runat="server">Attributo</asp:Localize></span>
                                                            <asp:DropDownList ID="Cmb_Caratteristiche" runat="server" CssClass="form-control" AutoPostBack="false">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_Caratteristica_Valore" for="Txt_Caratteristica_Valore">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Valore %>" runat="server">Valore</asp:Localize></span>
                                                            <asp:TextBox ID="Txt_Caratteristica_Valore" runat="server" CssClass="form-control" aria-describedby="lbl_Caratteristica_Valore"> </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                            <div class="col-lg-12 text-right">
                                                <div class="btn btn-info" id="btn_Aggiungi_Stalla">
                                                    <%--<div class="btn btn-info" onclick="$('#<=ImgBtn_Stalla_Ins.ClientID %>').click();">--%>
                                                    <i class="fa fa-plus"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                                </div>
                                                <%--<asp:ImageButton ID="ImgBtn_Stalla_Ins" runat="server" Style="display: none" />--%>
                                            </div>
                                            <%End If %>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="table-responsive">
                                            <div id="tabStalle">
                                            </div>
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
                                    <span class="input-group-addon alert-info" id="spn_chk_centro">
                                        <asp:CheckBox ID="Chk_Centro" runat="server" aria-label="..." />
                                    </span>
                                    <label aria-describedby="spn_chk_centro" id="lbl_chk_centro" class="form-control"
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
                                    <span class="input-group-addon alert-info" id="spn_chk_magazzino">
                                        <asp:CheckBox ID="Chk_Magazzino" runat="server" aria-label="..." />
                                    </span>
                                    <label aria-describedby="spn_chk_magazzino" id="lbl_chk_magazzino" class="form-control"
                                        for="<%=Chk_Magazzino.ClientID %>">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CreaMagazzinoAssociatoACentroAziendale %>" runat="server">
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
                <div class="tab-pane" id="tab_gerarchia">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CooperativeImpreseReferenti %>" runat="server">Cooperative / Imprese Referenti</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-9 col-md-9 col-sm-9">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_txtPivaPadre" for="txtPivaPadre">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,TestoRicerca %>" runat="server">Testo Ricerca</asp:Localize>:
                                                    </span>
                                                    <input type="text" class="form-control" id="txtPivaPadre" aria-describedby="lbl_txtPivaPadre" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-3 col-md-3 col-sm-3">
                                        <div class="btn btn-info btn_100" onclick="$('#<%=ImgBtn_Cerca_RagSoc.ClientID %>').click();">
                                            <i class="fa fa-search"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Cerca %>" runat="server">Cerca</asp:Localize>
                                        </div>
                                        <asp:ImageButton ID="ImgBtn_Cerca_RagSoc" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                            Style="display: none" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-9 col-md-9 col-sm-9">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon control-label alert-info" id="lbl_padre" for="Cmb_Imprese">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Referente %>" runat="server">Referente</asp:Localize>:
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_Imprese" runat="server" CssClass="form-control selectpicker"
                                                        data-live-search="true" aria-describedby="lbl_padre">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                    <div class="col-lg-3 col-md-3 col-sm-3">
                                        <div class="btn btn-info btn_100" onclick="$('#<%=ImgBtn_Aggiungi_Padre.ClientID %>').click();">
                                            <i class="fa fa-plus"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                        </div>
                                        <asp:ImageButton ID="ImgBtn_Aggiungi_Padre" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                            Style="display: none" />
                                    </div>
                                    <%End If%>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="table-responsive">
                                            <asp:GridView ID="GridView_Padri" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                                UseAccessibleHeader="true">
                                                <Columns>
                                                    <asp:ButtonField ButtonType="Link" HeaderText="<%$ Resources: AgronicaAgenda_2010,CancellaAbbr %>" Text="<i class='fa fa-times'></i>"
                                                        CommandName="Elimina">
                                                        <HeaderStyle Font-Names="Verdana" />
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:ButtonField>
                                                    <asp:BoundField DataField="Piva" HeaderText="<%$ Resources: AgronicaAgenda_2010,PartitaIVA %>"></asp:BoundField>
                                                    <asp:BoundField DataField="Rag_Soc" HeaderText="<%$ Resources: AgronicaAgenda_2010,RagioneSociale %>">
                                                        <ItemStyle HorizontalAlign="left"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Validita_Inizio" HeaderText="<%$ Resources: AgronicaAgenda_2010,Dal %>">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Validita_Fine" HeaderText="<%$ Resources: AgronicaAgenda_2010,Al %>">
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
                <div class="tab-pane gias-zootecnia-anagrafiche-fabbricato-dati-accessori gias-content-padding-x gias-content-padding-y" id="tab_dati_accessori">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-sm-12 border_si gias-zootecnia-anagrafiche-fabbricato-dati-accessori-periodo-attivita">
                                <div class="col-md-12 gias-content-no-padding-x">
                                    <h5 class="agronica-card-title card-title">
                                        <asp:Localize meta:resourcekey="PeriodoAttività" runat="server">Periodo Attività</asp:Localize></h5>
                                </div>
                                <div class="col-lg-4 gias-content-no-padding-x gias-content-padding-bottom">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_validita_inizio" for="TxtValiditaInizio">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Dal %>" runat="server">Dal</asp:Localize>
                                        </span>
                                        <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="form-control datepicker gias-input"
                                            MaxLength="10"> </asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_validita_fine" for="TxtValiditaFine">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Al %>" runat="server">Al</asp:Localize>
                                        </span>
                                        <asp:TextBox ID="TxtValiditaFine" runat="server" CssClass="form-control datepicker gias-input"
                                            MaxLength="10"> </asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-12 gias-content-no-padding-x gias-content-padding-bottom gias-zootecnia-anagrafiche-fabbricato-dati-accessori-message-container">
                                    <ul class="no-bullet container-info">
                                        <li>
                                            <i class="fa fa-info-circle"></i>
                                            <small><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,EstremiConsideratiGiorniValidi %>" runat="server">
                                                    Gli estremi sono considerati entrambi giorni validi.
                                            </asp:Localize></small>
                                        </li>
                                        <li>
                                            <i class="fa fa-info-circle"></i>
                                            <small><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CaselleVuoteIndicanoValiditàNonLimitata %>" runat="server">
                                                Le caselle possono essere lasciate vuote per indicare validità non limitata.
                                            </asp:Localize></small>
                                        </li>
                                    </ul>
                                </div>

                                <div class="col-lg-12 gias-content-no-padding-x gias-zootecnia-anagrafiche-fabbricato-dati-accessori-message-container">
                                    <div class="container-alert__validity">
                                        <i class="fa fa-exclamation-triangle color-danger"></i>
                                        <small>
                                            <b class="color-danger"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CENTROAZIENDALEValidità %>" runat="server">
                                                CENTRO AZIENDALE validità:
                                            </asp:Localize></b>
                                            <asp:Label ID="lbl_centro_data_inizio" runat="server" CssClass="txtUI" BorderStyle="None"></asp:Label>
                                            - 
                                            <asp:Label ID="lbl_centro_data_fine" runat="server" CssClass="txtUI" BorderStyle="None"></asp:Label>
                                        </small>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-lg-6 gias-content-no-padding-x">
                                        <div class="input-group gias-toolbar-margin-y">
                                            <span class="input-group-addon alert-info" id="lbl_Sup_Convenzionale" for="TxtSup_Convenzionale">
                                                <asp:Localize meta:resourcekey="VolumeConvenzionaleMc" runat="server">Volume Convenzionale [mc]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtSup_Convenzionale" runat="server" CssClass="form-control gias-input"
                                                MaxLength="6"> </asp:TextBox>
                                            <asp:TextBox ID="TxtSup_Conversione" runat="server" CssClass="form-control"
                                                MaxLength="6" Style="display: none"> </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Sup_Biologico" for="TxtSup_Biologico">
                                                <asp:Localize meta:resourcekey="VolumeBiologicoMc" runat="server">Volume Biologico [mc]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtSup_Biologico" runat="server" CssClass="form-control gias-input"
                                                MaxLength="6"> </asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-6 gias-content-no-padding-x">
                                        <div class="form-group">
                                            <div class="checkbox">
                                                <label>
                                                    <asp:CheckBox ID="chk_VisibileApp" runat="server" meta:resourcekey="chk_VisibileApp" Text="Visibile da App" />
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="checkbox">
                                                <label>
                                                    <asp:CheckBox ID="chk_DefaultOrtofrutta" runat="server" Text="Default Magazzino Imballaggi Ortofrutta" />
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        

                        <div class="row">
                            <div class="col-lg-12 border_si gias-zootecnia-anagrafiche-fabbricato-dati-accessori-codici-anagrafici">
                                <div class="row">
                                    <div class="col-md-12 gias-content-no-padding-x">
                                        <h5 class="agronica-card-title card-title">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CodiciAnagrafici %>" runat="server">Codici Anagrafici</asp:Localize>
                                        </h5>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6 gias-content-no-padding-x">
                                        <div class="row">
                                            <div class="col-lg-12 gias-content-no-padding-x">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group" id="div_CmbCodice">
                                                            <span class="input-group-addon alert-info" id="lbl_Codice" for="CmbCodice">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Codice %>" runat="server">Codice</asp:Localize>:
                                                            </span>
                                                            <asp:DropDownList ID="CmbCodice" runat="server" CssClass="form-control selectpicker"
                                                                data-live-search="true" aria-describedby="lbl_Codice">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 gias-content-no-padding-x">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_valorecod" for="TxtCodiceValore">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Valore %>" runat="server">Valore</asp:Localize>:
                                                            </span>
                                                            <asp:TextBox ID="TxtCodiceValore" runat="server" CssClass="form-control gias-input" aria-describedby="lbl_valorecod">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 gias-content-no-padding-x">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_validita_inizio_codice" for="TxtValiditaInizioCodice">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Dal %>" runat="server">Dal</asp:Localize>
                                                            </span>
                                                            <asp:TextBox ID="TxtValiditaInizioCodice" runat="server" CssClass="form-control datepicker gias-input">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-6 gias-content-no-padding-right">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_validita_fine_codice" for="TxtValiditaFineCodice">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Al %>" runat="server">Al</asp:Localize>
                                                            </span>
                                                            <asp:TextBox ID="TxtValiditaFineCodice" runat="server" CssClass="form-control datepicker gias-input">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                            <div class="col-lg-12 text-right gias-content-no-padding-x">
                                                <div class="btn btn-info btn--add-code" id="btn_Aggiungi_Codice">
                                                    <%--<div class="btn btn-info" onclick="$('#<=ImgBtn_Aggiungi_Codice.ClientID %>').click();">--%>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Aggiungi %>" runat="server">Aggiungi</asp:Localize><i class="fa fa-circle-plus"></i>
                                                </div>
                                                <%--<asp:ImageButton ID="ImgBtn_Aggiungi_Codice" runat="server" Style="display: none" />--%>
                                            </div>
                                            <%End If %>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 border_si gias-zootecnia-anagrafiche-fabbricato-dati-accessori-codici-anagrafici-tabella gias-table-plain">
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
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Validita_Fine" HeaderText="Al">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
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
        </div>
    </div>

    <!-- VARIE -->
    <!-- contiene l'indice del tab selezionato, viene controllato lato server CalledFromClient_SetIndexTab -->
    <input type="hidden" id="clickedTabUI" runat="server" />
    <!-- salva la chiamata alla funzione per il postback -->
    <input type="hidden" id="fooName_PostedBack" runat="server" />
    <!-- parametri le funzione di postback -->
    <input type="hidden" id="fooName_Param_PostedBack" runat="server" />
    <br />
    <input id="InizioCentro" name="InizioCentro" type="hidden" runat="server" />
    <input id="FineCentro" name="FineCentro" type="hidden" runat="server" />
    <div class="row" data-toggle="validator" role="form">
    </div>
</asp:Content>
<asp:Content ID="cont" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        //Costanti
        const MAGAZZINO_FARMACI_COD = 124;
        const idCheckBoxFarmacci = "#<%= chk_MagazzinoFarmaci.ClientID %>";
    </script>
    <script type="text/javascript">

        //'  Vanni, 28/05/2014 15:04:00: gestione del log in console...
        //        jQuery.logThis = function (text) {
        //            if ((window['console'] != undefined)) {
        //                console.log(text);
        //            }
        //        }

        // disabilitazione del tasto BACK 
        document.onkeypress = function (event) {
            if (typeof window.event != 'undefined') { // ie
                event = window.event;
                event.target = event.srcElement; // make ie confirm to standards !!
            }
            var kc = event.keyCode;
            var tt = event.target.type;

            if ((kc != 8) || (tt == 'text') || (tt == 'password') || (tt == 'textarea'))
                return true;
            alert('Disabilitato il tasto BACK della tastiera');
            return false;
        }

        if (typeof window.event != 'undefined') // ie
            document.onkeydown = document.onkeypress; // Trap bksp in ie. !! Note: does not trap enter, but onkeypress does !!


        function ValidaxSubmit() {

            var flag = controlla_form();

            if (flag) {
                // Passo i valori selezionati di OTE in session (per il salvataggio)
                var values = "";

                $("#<%=cbl_idoneita.ClientID%> option").each(function () {
                    if ($(this).is(':selected'))
                        values = values + $(this).val() + "-";
                    else
                        values = values + "-";
                });
                values = values.substring(0, values.length - 1);

                $.ajax({
                    type: 'POST',
                    url: 'Fabbricato_Edit.aspx/Salva_cbl_idoneita',
                    data: "{valori:'" + values + "'}",
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json', async: true,
                    success: function (r) {
                        //                               var codici = r.d;
                        //                                //alert(r.d);
                        //                               codici = codici.substring(1, codici.length);
                        //                               var vals=codici.split('-');
                    }
                });


                $.ajax({
                    type: 'POST',
                    url: 'Fabbricato_Edit.aspx/Set_Comune',
                    data: "{comune:'" + $('#<%=Cmb_Comune.ClientID %>').val() + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                    }

                });


                if ($('#<%=Cmb_TipoRicovero.ClientID%>').val() != "") {
                    // riempio la select Indirizzo Produttivo
                    $.ajax({
                        type: 'POST',
                        url: 'fabbricato_edit.aspx/Salva_Cmb_TipoRicovero',
                        data: "{valore:'" + $('#<%=Cmb_TipoRicovero.ClientID%>').val() + "'}",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json', async: false,
                        success: function (r) {
                        }
                    });
                }




                $('#<%=ImgBtn_SalvaTutto.ClientID %>').click();

            }


        }

        function controlla_form() {

            var flag = true;
            var n_inv = 0;


            v.resetForm();

            // Azzero tutte le label custom_val
            $(".custom_val").each(function (i, obj) {
                $(this).css('border', '1px solid #ccc');
                $(this).parent().children().css('border-color', '#ccc');
                $(this).remove();
            });


            if ($('#<%=TxtDenominazione.ClientID %>').val() == "") {
                $('#<%=TxtDenominazione.ClientID %>').parent().append('<label id="<%=TxtDenominazione.ClientID %>-error" class="custom_val error" for="<%=TxtDenominazione.ClientID %>">'
                    + TraduzioneMultiResx(fabbricatoEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
                $('#<%=TxtDenominazione.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }

            if ($('#<%=Cmb_TipoFabbricato.ClientID %>').val() == "") {
                $('#<%=Cmb_TipoFabbricato.ClientID %>').parent().append('<label id="<%=Cmb_TipoFabbricato.ClientID %>-error" class="custom_val error" for="<%=Cmb_TipoFabbricato.ClientID %>">'
                       + TraduzioneMultiResx(fabbricatoEditResx, 'SelezionareUnCampo', 'Selezionare un campo') + '</label > ');
                   $('#<%=Cmb_TipoFabbricato.ClientID %>').parent().children(".required").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }

            var cap = $('#<%=Txt_CAP.ClientID %>').val();
            if (cap.length < 5) {
                $('#<%=Txt_CAP.ClientID %>').parent().append('<label id="<%=Txt_CAP.ClientID %>-error" class="custom_val error" for="<%=Txt_CAP.ClientID %>">'
                       + TraduzioneMultiResx(fabbricatoEditResx, 'ControlloLunghezzaCampoCAP', 'Il campo deve essere di 5 caratteri') + '</label>');
                   $('#<%=Txt_CAP.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }

            if ($('#<%=Cmb_Provincia.ClientID %>').val() == "") {
                $('#<%=Cmb_Provincia.ClientID %>').parent().append('<label id="<%=Cmb_Provincia.ClientID %>-error" class="custom_val error" for="<%=Cmb_Provincia.ClientID %>">'
                       + TraduzioneMultiResx(fabbricatoEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
                   $('#<%=Cmb_Provincia.ClientID %>').parent().children(".required").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }


            if ($('#<%=Cmb_Comune.ClientID %>').val() == "") {
                $('#<%=Cmb_Comune.ClientID %>').parent().append('<label id="<%=Cmb_Comune.ClientID %>-error" class="custom_val error" for="<%=Cmb_Comune.ClientID %>">'
                       + TraduzioneMultiResx(fabbricatoEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
                   $('#<%=Cmb_Comune.ClientID %>').parent().children(".required").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }

            if ($(idCheckBoxFarmacci).is(':checked') && $('#<%=TxtProprietarioCapi.ClientID %>').val() == "") {
                $('#<%=TxtProprietarioCapi.ClientID %>').parent().append('<label id="<%=TxtProprietarioCapi.ClientID %>-error" class="custom_val error" for="<%=TxtProprietarioCapi.ClientID %>">'
                    + TraduzioneMultiResx(fabbricatoEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
                $('#<%=TxtProprietarioCapi.ClientID %>').parent().children(".required").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }
            if ($(idCheckBoxFarmacci).is(':checked') && $('#<%=TxtCodiceBDN.ClientID %>').val() == "") {
                $('#<%=TxtCodiceBDN.ClientID %>').parent().append('<label id="<%=TxtCodiceBDN.ClientID %>-error" class="custom_val error" for="<%=TxtCodiceBDN.ClientID %>">'
                    + TraduzioneMultiResx(fabbricatoEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
                $('#<%=TxtCodiceBDN.ClientID %>').parent().children(".required").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }

<%--            let tipologiaFabbricato = $('#<%=Cmb_TipoFabbricato.ClientID%>').val();
            if (tipologiaFabbricato == MAGAZZINO_FARMACI_COD && $('#<%=TxtProprietarioCapi.ClientID %>').val() == "") {
                $('#<%=TxtProprietarioCapi.ClientID %>').parent().append('<label id="<%=TxtProprietarioCapi.ClientID %>-error" class="custom_val error" for="<%=TxtProprietarioCapi.ClientID %>">'
                    + TraduzioneMultiResx(fabbricatoEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
                $('#<%=TxtProprietarioCapi.ClientID %>').parent().children(".required").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }--%>


            <%--if($('#<%=TxtSup_Convenzionale.ClientID %>').val() =="")
            {   
                $('#<%=TxtSup_Convenzionale.ClientID %>').parent().append('<label id="<%=TxtSup_Convenzionale.ClientID %>-error" class="custom_val error" for="<%=TxtSup_Convenzionale.ClientID %>">Il campo non può essere vuoto</label>');
                $('#<%=TxtSup_Convenzionale.ClientID %>').parent().children( ".required" ).css('border','1px solid #D41E1A');
                flag = false;
                n_inv++;
            }
            else{

                // Controllo se il cap contiene valori non numerici
                if ($('#<%=TxtSup_Convenzionale.ClientID %>').val().match(/[a-z]/i)) {
                    $('#<%=TxtSup_Convenzionale.ClientID %>').parent().append('<label id="<%=TxtSup_Convenzionale.ClientID %>-error" class="custom_val error" for="<%=TxtSup_Convenzionale.ClientID %>">Il campo può contenere solo numeri</label>');
                    $('#<%=TxtSup_Convenzionale.ClientID %>').closest( "input" ).css('border','1px solid #D41E1A');
                    flag = false;
                    n_inv++;
                }
                else
                {
                    if ($('#<%=TxtSup_Convenzionale.ClientID %>').val() < 0) {
                    $('#<%=TxtSup_Convenzionale.ClientID %>').parent().append('<label id="<%=TxtSup_Convenzionale.ClientID %>-error" class="custom_val error" for="<%=TxtSup_Convenzionale.ClientID %>">Il campo deve essere maggiore di 0</label>');
                    $('#<%=TxtSup_Convenzionale.ClientID %>').closest( "input" ).css('border','1px solid #D41E1A');
                    flag = false;
                    n_inv++;
                }
                }

            }


            if($('#<%=TxtSup_Biologico.ClientID %>').val() =="")
            {   
                $('#<%=TxtSup_Biologico.ClientID %>').parent().append('<label id="<%=TxtSup_Biologico.ClientID %>-error" class="custom_val error" for="<%=TxtSup_Biologico.ClientID %>">Il campo non può essere vuoto</label>');
                $('#<%=TxtSup_Biologico.ClientID %>').parent().children( ".required" ).css('border','1px solid #D41E1A');
                flag = false;
                n_inv++;
            }
            else{

                // Controllo se il cap contiene valori non numerici
                if ($('#<%=TxtSup_Biologico.ClientID %>').val().match(/[a-z]/i)) {
                    $('#<%=TxtSup_Biologico.ClientID %>').parent().append('<label id="<%=TxtSup_Biologico.ClientID %>-error" class="custom_val error" for="<%=TxtSup_Biologico.ClientID %>">Il campo può contenere solo numeri</label>');
                    $('#<%=TxtSup_Biologico.ClientID %>').closest( "input" ).css('border','1px solid #D41E1A');
                    flag = false;
                    n_inv++;
                }
                else
                {
                    if ($('#<%=TxtSup_Biologico.ClientID %>').val() < 0) {
                    $('#<%=TxtSup_Biologico.ClientID %>').parent().append('<label id="<%=TxtSup_Biologico.ClientID %>-error" class="custom_val error" for="<%=TxtSup_Biologico.ClientID %>">Il campo deve essere maggiore di 0</label>');
                    $('#<%=TxtSup_Biologico.ClientID %>').closest( "input" ).css('border','1px solid #D41E1A');
                    flag = false;
                    n_inv++;
                }
                }

            }--%>



            // Controllo se le date sono corrette
            var TxtValiditaInizio = $('#<%=TxtValiditaInizio.ClientID %>').val().split("/");
            var TxtValiditaFine = $('#<%=TxtValiditaFine.ClientID %>').val().split("/");
            var TxtCentroInizio = $('#<%=lbl_centro_data_inizio.ClientID %>').text().split("/");
            var TxtCentroFine = $('#<%=lbl_centro_data_fine.ClientID %>').text().split("/");

            ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
            fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);
            c_ini = new Date(TxtCentroInizio[2], TxtCentroInizio[1] - 1, TxtCentroInizio[0]);
            c_fin = new Date(TxtCentroFine[2], TxtCentroFine[1] - 1, TxtCentroFine[0]);

            if (ini > fin) {

                $('#<%=TxtValiditaInizio.ClientID %>').parent().append('<label id="<%=TxtValiditaInizio.ClientID %>-error" class="custom_val error" for="<%=TxtValiditaInizio.ClientID %>">'
                       + TraduzioneMultiResx(fabbricatoEditResx, 'DataInizioNonPuòEssereMaggioreDiDataFine', 'La data di inizio non può essere maggiore di quella di fine') + '</label>');
                   $('#<%=TxtValiditaInizio.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;

            }

            if ((c_ini > ini) || (fin > c_fin)) {
                $('#<%=TxtValiditaInizio.ClientID %>').parent().append('<label id="<%=TxtValiditaInizio.ClientID %>-error" class="custom_val error" for="<%=TxtValiditaInizio.ClientID %>">'
                       + TraduzioneMultiResx(fabbricatoEditResx, 'DateImmesseDevonoEssereCompreseNellaValiditàCentroAziendale', 'Le date immesse devono essere comprese nella validità del Centro Aziendale') + '</label>');
                   $('#<%=TxtValiditaInizio.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                flag = false;
                n_inv++;
            }


            ///////////////////////////////


            if (v.valid() && flag) {

                $('.nav-tabs li.active a .error-tab').remove(".error-tab");
                return true
            }
            else {
                var err_message = "";
                err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
                $('.nav-tabs li.active a').append(err_message);

                return false;
            }

        }



        function AggiornaTabCodici(d) {
            AgroWA_Table_sistemaDati(d);

            $('#tabCodici').html('');
            watableCodici = $("#tabCodici").WATable({
                pageSize: 50,
                pageSizes: [50],
                filter: false,
                preFill: false,
                checkboxes: false,
                tableCreated: function (data) {
                    //                                    coloraMovimenti();
                }
                , pageChanged: function (data) {
                    //                                    coloraMovimenti();
                }
                , types: {
                    string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                    date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                    number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                    bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                }
            }).data('WATable').setData(d);

            InitWaTable("#tabCodici", watableCodici);
        }


        function AggiornaTabStalle(d) {
            AgroWA_Table_sistemaDati(d);

            $('#tabStalle').html('');
            watableStalle = $("#tabStalle").WATable({
                pageSize: 50,
                pageSizes: [50],
                filter: false,
                preFill: false,
                checkboxes: false,
                tableCreated: function (data) {
                    //                                    coloraMovimenti();
                }
                , pageChanged: function (data) {
                    //                                    coloraMovimenti();
                }
                , types: {
                    string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                    date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                    number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                    bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                }
            }).data('WATable').setData(d);

            InitWaTable("#tabStalle", watableStalle);
        }

        function EliminaCodice(obj) {
            var Id_Cod = $(obj).attr('chiave');

            $.ajax({
                type: 'POST',
                url: 'Impresa_Edit.aspx/EliminaCodice',
                data: "{Id_Cod:'" + Id_Cod + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        AggiornaTabCodici(JSON.parse(r.d.RispostaStringa));
                    else {
                        alert(r.d.Errore);
                    }
                }
            });
        }


        function ModificaCodice(obj) {

            // Pulisco le strutture
            $('#<%=CmbCodice.ClientID %>').val('');
                $('.selectpicker').selectpicker('refresh');
                $('#<%=TxtCodiceValore.ClientID %>').val('');
                $('#<%=TxtValiditaInizioCodice.ClientID %>').val('');
                $('#<%=TxtValiditaFineCodice.ClientID %>').val('');


                var Id_Cod = $(obj).attr('chiave');

                var i;

                for (i = 0; i < watableCodici.getData().rows.length; i++) {
                    var q = watableCodici.getData().rows[i];
                    if (q.Tool == Id_Cod) {
                        var codice = q.Codice;

                        $("#<%=CmbCodice.ClientID %> option").each(function () {

                            if (Id_Cod == $(this).val()) {
                                $(this).prop('selected', true);
                                $('.selectpicker').selectpicker('refresh');
                            }

                            i = i + 1;

                        });


                        var valore = q.Valore;
                        $('#<%=TxtCodiceValore.ClientID %>').val(valore);

                        var Dal = q.Dal;
                        if (Dal == "...")
                            Dal = "";
                        $('#<%=TxtValiditaInizioCodice.ClientID %>').val(Dal);

                        var Al = q.Al;
                        if (Al == "...")
                            Al = "";
                        $('#<%=TxtValiditaFineCodice.ClientID %>').val(Al);

                    break;
                }
            }

            //                 EliminaCodice(obj);                

        }


        var fl1 = false;
        var fl2 = false
        var fl3 = false
        var fl4 = false
        var fl5 = false

        function checkOnFinish() {
            if ($("#<%=TxtDenominazione.ClientID %>").val() !== "") {
                fl1 = true;
            }
            if ($("#<%=Txt_Via.ClientID %>").val() !== "") {
                fl2 = true;
            }
            if ($("#<%=Cmb_Provincia.ClientID %>").val() !== "") {
                fl3 = true;
            }
            if ($("#<%=Cmb_Comune.ClientID %>").val() !== "") {
                fl4 = true;
            }
            if ($("#<%=Txt_CAP.ClientID %>").val() !== "") {
                fl5 = true;
            }
        }
		
		var fabbricatoEditResx = [];
        var resxArrPath = [
            "App_GlobalResources/AgronicaAgenda_2010.resx",
            "Anagrafica/App_LocalResources/Fabbricato_Edit.aspx.resx"
        ];

        jQuery(document).ready(function () {

            if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
                // Carico i files resx per le traduzioni
                resxArrPath.forEach(function (resxSinglePath) {
                    fabbricatoEditResx.push(readResxFile(resxSinglePath, "Fabbricato_Edit.aspx"));
                });
            }

            var call = 0;
            $('#aspnetForm').change(function () {
                controlla_form();
            });

            // Disabilito le select al caricamento della pagina
            $('#<%=Cmb_IndirizzoProduttivo.ClientID%>').attr('disabled', 'disabled');
            $('#<%=Cmb_TipoRicovero.ClientID%>').attr('disabled', 'disabled');
            $('#<%=Cmb_SottotipoRicovero.ClientID%>').attr('disabled', 'disabled');
            $('#<%=Cmb_Caratteristiche.ClientID%>').attr('disabled', 'disabled');


            // Carico le idoneità
            $.ajax({
                type: 'POST',
                url: 'Fabbricato_Edit.aspx/Ripristina_cbl_idoneita',
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#<%=cbl_idoneita.ClientID %>').empty();
                    $('#<%=cbl_idoneita.ClientID %>').append(r.d);
                    $('.selectpicker').selectpicker('refresh');
                }
            });



            // Colorazione riga selezionata in watable
            $("body").on("click", ".watable tbody tr td:not('.footable-row-detail-cell')", function () {

                $(".watable tbody tr.success").each(function (i) {
                    $(this).removeClass("success");
                });

                // soottolineo la riga selezionata...
                $(this).parent().addClass('success');
            });




            // Gestione della selezione della nazione
            if ($("#<%=cmb_Stato.ClientID%> :selected").attr('Gestione_Gerarchia_Geografica') == "1") {

                $("#div_prov_com").show();
                $("#lbl_frazione").text(TraduzioneMultiResx(fabbricatoEditResx, "Frazione", "Frazione"));
            }
            else {
                $("#div_prov_com").hide();
                $("#lbl_frazione").text(TraduzioneMultiResx(fabbricatoEditResx, "Città", "Città"));
            }



            //carico eventualmente la tabella di gerarchia codici
            var initCodici;

             <% if (jsCodici <> "") Then %>
            initCodici = <%=jsCodici %>;
            <% else%>
            initCodici = "";
            <% end if %>
            if (initCodici != "") {
                AggiornaTabCodici(initCodici);
            }

            //carico eventualmente la tabella di stalle
            var initStalle;

             <% if (jsStalle <> "") Then %>
            initStalle = <%=jsStalle %>;
            <% else%>
            initStalle = "";
            <% end if %>
            if (initStalle != "") {
                AggiornaTabStalle(initStalle);
            }

            //carico eventualmente la tabella di stalle
            var initConfigurazioniBDN;

             <% if (jsStalleConfigurazioniBDN <> "") Then %>
            initConfigurazioniBDN = <%=jsStalleConfigurazioniBDN %>;
            <% else%>
            initConfigurazioniBDN = [];
            <% end if %>
            if (initConfigurazioniBDN !== "") {
                AggiornaTabConfigurazioneBDN(initConfigurazioniBDN);
            }

            //Disabilito il cambio di tab se la validazione è fallita
            //            $('.nav-tabs > li > a').click(function (e) {
            //                if ($('.error-tab').length)
            //                    return false;
            //                else
            //                    return true;
            //            });

            // Controllo se il valore della Select Tipologia Fabbricato = STALLA
            var tipo_fab = $('#<%=Cmb_TipoFabbricato.ClientID%>').val();
            if ((tipo_fab == 70) || (tipo_fab >= 170 && tipo_fab <= 179)) {
                //alert($('#<%=Cmb_TipoFabbricato.ClientID%>').val());
                $('#div_dettagli_stalla').show();
                $('#div_caratt_stalla').show();
                $('#div_stalla_bdn').show();
                $('#<%=Cmb_Caratteristiche.ClientID%>').removeAttr('disabled');

            }


            function mostraInfoMagazzinoFarmacci() {
                if ($(idCheckBoxFarmacci).is(':checked')) {
                    $('#row_InfoAggiuntive').show();
                } else {
                    $('#row_InfoAggiuntive').hide();
                }
            }
            $(idCheckBoxFarmacci).click(mostraInfoMagazzinoFarmacci);
            mostraInfoMagazzinoFarmacci();

            //if (tipo_fab == MAGAZZINO_FARMACI_COD) {
            //    $('#row_InfoAggiuntive').show();
            //} else {
            //    $('#row_InfoAggiuntive').hide();
            //}


            // riempio la select Idoneità
            $.ajax({
                type: 'POST',
                url: 'fabbricato_edit.aspx/Riempi_Cmb_Idoneita',
                data: "",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json', async: true,
                success: function (r) {
                    $('#cbl_idoneita').empty();
                    $('#cbl_idoneita').append(r.d);
                    $('.selectpicker').selectpicker('refresh');
                }
            });




            $('#<%=cmb_Stato.ClientID%>').change(function (e) {

                if ($("#<%=cmb_Stato.ClientID%> :selected").attr('Gestione_Gerarchia_Geografica') == "1") {

                    stato = $('#<%=cmb_Stato.ClientID%>').val();

                    if ($('#<%=cmb_Stato.ClientID%>').val() != "") {
                        $('#<%=Cmb_Provincia.ClientID %>').parent().children().attr("disabled", false);

                        $.ajax({
                            type: 'POST',
                            url: 'Fabbricato_Edit.aspx/Carica_Province',
                            data: "{stato:'" + stato + "'}",
                            contentType: 'application/json; charset=utf-8',
                            cache: false,
                            dataType: 'json', async: true,
                            success: function (r) {
                                //alert(r.d);
                                $('#<%=Cmb_Provincia.ClientID %>').empty();
                                $('#<%=Cmb_Provincia.ClientID %>').append(r.d[0]);
                                $('#<%=Txt_ProCodIstat.ClientID %>').val(r.d[1]);

                                $('#<%=Cmb_Comune.ClientID%>').empty();


                                $('.selectpicker').selectpicker('refresh');
                            }
                        });
                    }
                    else {
                        // Disabilito i comuni se vuoto
                        $('#<%=Cmb_Provincia.ClientID %>').empty();
                        $('#<%=Cmb_Provincia.ClientID %>').parent().children().attr("disabled", true);
                        $('.selectpicker').selectpicker('refresh');
                    }
                }                
                
                if ($("#<%=cmb_Stato.ClientID%> :selected").attr('Gestione_Gerarchia_Geografica') == "1") {
                    $("#div_prov_com").show();
                    $("#lbl_frazione").text(TraduzioneMultiResx(fabbricatoEditResx, "Frazione", "Frazione"));
                }
                else {
                    $("#div_prov_com").hide();
                    $("#lbl_frazione").text(TraduzioneMultiResx(fabbricatoEditResx, "Città", "Città"));
                }
           
            });


            // ... onChange della stessa select (in fase NEW)
            $('#<%=Cmb_TipoFabbricato.ClientID%>').change(function () {
                let tipologiaFabbricato = $('#<%=Cmb_TipoFabbricato.ClientID%>').val();
                $('#row_InfoAggiuntive').hide();

                if ((tipologiaFabbricato == 70) || (tipologiaFabbricato >= 170 && tipologiaFabbricato <= 179)) {
                    //alert($('#<%=Cmb_TipoFabbricato.ClientID%>').val());
                    $('#div_dettagli_stalla').show();
                    $('#div_caratt_stalla').show();
                    $('#div_stalla_bdn').show();

                }
                else if (tipologiaFabbricato == MAGAZZINO_FARMACI_COD) {
                    $('#row_InfoAggiuntive').show();
                }
                else {
                    $('#div_dettagli_stalla').hide();
                    $('#div_caratt_stalla').hide();
                    $('#div_stalla_bdn').hide();
                    
                }

                if (tipologiaFabbricato == 20) {
                    $('#chkMagFarm').show();
                } else {
                    $('#chkMagFarm').hide();
                }
            });


            // ... onChange della select Cmb_Animali
            $('#<%=Cmb_Animali.ClientID%>').change(function () {

                // prelevo i dati da Cmb_Animali
                var Cmb_Animali = $('#<%=Cmb_Animali.ClientID%>').val();
                var values = Cmb_Animali.split('|');

                if ($('#<%=Cmb_Animali.ClientID%>').val() != "") {
                    // riempio la select Indirizzo Produttivo
                    $.ajax({
                        type: 'POST',
                        url: 'fabbricato_edit.aspx/Riempi_Cmb_IndirizzoProduttivo',
                        data: "{Gen_Cod:'" + values[0] + "', Spe_Cod:'" + values[1] + "'}",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json', async: true,
                        success: function (r) {
                            $('#<%=Cmb_IndirizzoProduttivo.ClientID %>').empty();
                            $('#<%=Cmb_IndirizzoProduttivo.ClientID %>').append(r.d);
                            $('#<%=Cmb_IndirizzoProduttivo.ClientID %>').removeAttr('disabled');
                        }
                    });
                }
                else {
                    $('#<%=Cmb_IndirizzoProduttivo.ClientID %>').empty();
                    $('#<%=Cmb_IndirizzoProduttivo.ClientID%>').attr('disabled', 'disabled');
                    $('#<%=Cmb_TipoRicovero.ClientID %>').empty();
                    $('#<%=Cmb_TipoRicovero.ClientID%>').attr('disabled', 'disabled');
                    $('#<%=Cmb_SottotipoRicovero.ClientID %>').empty();
                    $('#<%=Cmb_SottotipoRicovero.ClientID%>').attr('disabled', 'disabled');
                    $('#<%=Cmb_Caratteristiche.ClientID %>').empty();
                    $('#<%=Cmb_Caratteristiche.ClientID%>').attr('disabled', 'disabled');
                }

            });


            // ... onChange della select Cmb_IndirizzoProduttivo
            $('#<%=Cmb_IndirizzoProduttivo.ClientID%>').change(function () {

                // prelevo i dati da Cmb_IndirizzoProduttivo
                var Cmb_IndirizzoProduttivo = $('#<%=Cmb_IndirizzoProduttivo.ClientID%>').val();
                var values = Cmb_IndirizzoProduttivo.split('|');

                if ($('#<%=Cmb_IndirizzoProduttivo.ClientID%>').val() != "") {

                    // riempio la select Tipo Ricovero
                    $.ajax({
                        type: 'POST',
                        url: 'fabbricato_edit.aspx/Riempi_Cmb_TipoRicovero',
                        data: "{Gen_Cod:'" + values[0] + "', Spe_Cod:'" + values[1] + "', IPro_Cod:'" + values[2] + "'}",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json', async: true,
                        success: function (r) {
                            $('#<%=Cmb_TipoRicovero.ClientID %>').empty();
                            $('#<%=Cmb_TipoRicovero.ClientID %>').append(r.d);
                            $('#<%=Cmb_TipoRicovero.ClientID %>').removeAttr('disabled');
                        }
                    });

                }
                else {
                    $('#<%=Cmb_TipoRicovero.ClientID %>').empty();
                    $('#<%=Cmb_TipoRicovero.ClientID%>').attr('disabled', 'disabled');
                    $('#<%=Cmb_SottotipoRicovero.ClientID %>').empty();
                    $('#<%=Cmb_SottotipoRicovero.ClientID%>').attr('disabled', 'disabled');
                    $('#<%=Cmb_Caratteristiche.ClientID %>').empty();
                    $('#<%=Cmb_Caratteristiche.ClientID%>').attr('disabled', 'disabled');
                }

            });


            // ... onChange della select Cmb_TipoRicovero
            $('#<%=Cmb_TipoRicovero.ClientID%>').change(function () {

                // prelevo i dati da Cmb_IndirizzoProduttivo
                var Cmb_TipoRicovero = $('#<%=Cmb_TipoRicovero.ClientID%>').val();
                var values = Cmb_TipoRicovero.split('|');

                if ($('#<%=Cmb_TipoRicovero.ClientID%>').val() != "") {
                    // riempio la select Tipo Ricovero
                    $.ajax({
                        type: 'POST',
                        url: 'fabbricato_edit.aspx/Riempi_Cmb_Sottotipi_Stalla',
                        data: "{Gen_Cod:'" + values[0] + "', Spe_Cod:'" + values[1] + "', IPro_Cod:'" + values[2] + "', Cod_Fabb:'" + values[3] + "'}",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json', async: true,
                        success: function (r) {
                            $('#<%=Cmb_SottotipoRicovero.ClientID %>').empty();
                            $('#<%=Cmb_SottotipoRicovero.ClientID %>').append(r.d);
                            $('#<%=Cmb_SottotipoRicovero.ClientID %>').removeAttr('disabled');
                        }
                    });

                }
                else {
                    $('#<%=Cmb_SottotipoRicovero.ClientID %>').empty();
                    $('#<%=Cmb_SottotipoRicovero.ClientID%>').attr('disabled', 'disabled');
                    $('#<%=Cmb_Caratteristiche.ClientID %>').empty();
                    $('#<%=Cmb_Caratteristiche.ClientID%>').attr('disabled', 'disabled');
                }

            });


            // ... onChange della select Cmb_SottotipoRicovero
            $('#<%=Cmb_SottotipoRicovero.ClientID%>').change(function () {

                // prelevo i dati da Cmb_IndirizzoProduttivo
                var Cmb_SottotipoRicovero = $('#<%=Cmb_SottotipoRicovero.ClientID%>').val();
                //var values = Cmb_TipoRicovero.split('|');

                if ($('#<%=Cmb_SottotipoRicovero.ClientID%>').val() != "") {
                    // riempio la select Tipo Ricovero
                    $.ajax({
                        type: 'POST',
                        url: 'fabbricato_edit.aspx/Riempi_Cmb_Caratteristiche',
                        data: "{Cod_Fabb:'" + Cmb_SottotipoRicovero + "'}",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json', async: true,
                        success: function (r) {
                            $('#<%=Cmb_Caratteristiche.ClientID %>').empty();
                            $('#<%=Cmb_Caratteristiche.ClientID %>').append(r.d);
                            $('#<%=Cmb_Caratteristiche.ClientID %>').removeAttr('disabled');
                        }
                    });

                }
                else {
                    $('#<%=Cmb_Caratteristiche.ClientID %>').empty();
                    $('#<%=Cmb_Caratteristiche.ClientID%>').attr('disabled', 'disabled');
                }

            });




            // Gestione click del bottone Aggiungi in tab Codici
            $('#btn_Aggiungi_Codice').click(function (e) {

                var codice = $('#div_CmbCodice button').text();
                var valore = $('#<%=TxtCodiceValore.ClientID%>').val();
                var DataInizio = "";
                var DataFine = "";

                // Controllo se è stato immesso del testo nel input del Cerca
                if ((codice != "") && (valore != "")) {
                    codice = codice.slice(0, -1);

                    DataInizio = $('#<%=TxtValiditaInizioCodice.ClientID%>').val();
                    DataFine = $('#<%=TxtValiditaFineCodice.ClientID%>').val();

                    $("#<%=CmbCodice.ClientID%> option").each(function () {
                        if (this.text == codice) {
                            //alert(this.value);
                            codice_id = this.value;
                        }
                    });

                    $.ajax({
                        type: 'POST',
                        url: 'fabbricato_edit.aspx/Aggiungi_Codice',
                        data: "{codice:'" + codice + "', valore:'" + valore + "', codice_id:'" + codice_id + "', DataInizio:'" + DataInizio + "', DataFine:'" + DataFine + "'}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            if (r.d.RispostaOK == true)
                                AggiornaTabCodici(JSON.parse(r.d.RispostaStringa));
                            else {
                                alert(r.d.Errore);
                            }

                        }
                    });

                }
                else {
                    $('#<%=Cmb_Imprese.ClientID%>').focus();
                }


            });


            // Gestione click del bottone Aggiungi in tab Stalle
            $('#btn_Aggiungi_Stalla').click(function (e) {

                var codice = $('#<%=Cmb_Caratteristiche.ClientID%>').val();
                var valore = $('#<%=Txt_Caratteristica_Valore.ClientID%>').val();
                //var DataInizio = "";
                //var DataFine = "";

                // Controllo se è stato immesso del testo nel input del Cerca
                if ((codice != "") && (valore != "")) {

                    $.ajax({
                        type: 'POST',
                        url: 'fabbricato_edit.aspx/Aggiungi_Stalla',
                        data: "{codice:'" + codice + "', valore:'" + valore + "'}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            if (r.d.RispostaOK == true)
                                AggiornaTabStalle(JSON.parse(r.d.RispostaStringa));
                            else {
                                alert(r.d.Errore);
                            }

                        }
                    });

                }
                else {
                    $('#<%=Cmb_Imprese.ClientID%>').focus();
                }


            });

            // Quando cambia Provincia, mostro Comuni relativi
            $('#<%=Cmb_Provincia.ClientID %>').change(function (e) {


                provincia = $('#<%=Cmb_Provincia.ClientID %>').val();

                if ($('#<%=Cmb_Provincia.ClientID %>').val() != "") {
                    // $('#<=Cmb_Comune.ClientID %>').parent().children().attr("disabled",false);

                    $.ajax({
                        type: 'POST',
                        url: 'Fabbricato_Edit.aspx/Carica_Comuni',
                        data: "{provincia:'" + provincia + "'}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            //alert(r.d);
                            $('#<%=Cmb_Comune.ClientID %>').empty();
                            $('#<%=Cmb_Comune.ClientID %>').append(r.d[0]);
                            $('#<%=Txt_ProCodIstat.ClientID %>').val(r.d[1]);
                            $('#<%=Txt_ProvinciaSigla.ClientID %>').val(provincia);

                            $('.selectpicker').selectpicker('refresh');
                        }
                    });
                }
                else {
                    // Disabilito i comuni se vuoto
                    $('#<%=Cmb_Comune.ClientID %>').empty();
                    $('#<%=Cmb_Comune.ClientID %>').parent().children().attr("disabled", true);
                    $('.selectpicker').selectpicker('refresh');
                }
            });

            checkOnFinish();
            nascondi_riepilogo_error();

            $(".datepicker").attr("autocomplete", "off");

            // Gestione Riepilogo Errori (Validazione)
            $("#<%=TxtDenominazione.ClientID %>").keyup(function () {
                if ($("#<%=TxtDenominazione.ClientID %>").val() != "") {
                    $('.voce_1').hide();
                    fl1 = true;
                }
                else {
                    $('.voce_1').show();
                    fl1 = false;
                }

                nascondi_riepilogo_error();
            });

            $("#<%=Txt_Via.ClientID %>").keyup(function () {
                if ($("#<%=Txt_Via.ClientID %>").val() != "") {
                    $('.voce_2').hide();
                    fl2 = true;
                }
                else {
                    $('.voce_2').show();
                    fl2 = false;
                }

                nascondi_riepilogo_error();
            });



            $("#<%=Cmb_Provincia.ClientID %>").change(function () {
                if ($("#<%=Cmb_Provincia.ClientID %>").val() != "") {
                    $('.voce_3').hide();
                    fl3 = true;
                }
                else {
                    $('.voce_3').show();
                    fl3 = false;
                }

                nascondi_riepilogo_error();
            });

            $("#<%=Cmb_Comune.ClientID %>").change(function () {
                if ($("#<%=Cmb_Comune.ClientID %>").val() != "") {
                    $('.voce_4').hide();
                    fl4 = true;
                }
                else {
                    $('.voce_4').show();
                    fl4 = false;
                }

                nascondi_riepilogo_error();
            });

            $("#<%=Txt_CAP.ClientID %>").keyup(function () {
                if ($("#<%=Txt_CAP.ClientID %>").val() != "") {
                    $('.voce_5').hide();
                    fl5 = true;
                }
                else {
                    $('.voce_5').show();
                    fl5 = false;
                }

                nascondi_riepilogo_error();
            });


            function nascondi_riepilogo_error() {
                if (fl1)
                    $('.voce_1').hide();

                if (fl2)
                    $('.voce_2').hide();

                if (fl3)
                    $('.voce_3').hide();

                if (fl4)
                    $('.voce_4').hide();

                if (fl5)
                    $('.voce_5').hide();


                if ((fl1) && (fl2) && (fl3) && (fl4) && (fl5))
                    $('#div_riepilogo_error').hide();
                else
                    $('#div_riepilogo_error').show();
            }


            /////////////////////

        });

        var griglia;

        function AggiornaTabConfigurazioneBDN(d) {
            divKendoConfigurazioni = "divKendoConfigurazioni";
            let objAgenda = JSON.parse(objP_agenda);
            let modifica = true;
            let funzioneSubmit = { funzione: kConfigurazioneBDN_Submit, flagInsert: true, flagUpdate: true, flagDelete: true };

            if (objAgenda.Tipo_Operazione == "0") {
                funzioneSubmit = undefined;
                modifica = false;
            }

            var funzioniCRUD = {
                funzioneRead: (options) => { return options.success(d); },
                funzioneSubmit: funzioneSubmit
            };

            var idModel = "ID";

            var campiKendoModel = kConfigurazioneBDNModel(modifica);
            var colonneKendoGrid = kConfigurazioneBDNColumns();

            var parametriPerLettura = [];
            var parametriDataSource = { pagesize: 50 };
            var parametriKendoGrid = {
                columnMenu: true,
                impostaColonneKendoGridDaCookie: false,
                //toolbarCommands: ["templateLegendaMenuAgendaOperazioniTutte"],
                excel: false,
                pdf: false,
                sortable: true,
                groupable: false,
                reorderable: true,
                salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
                scrollable: false,
                //pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
                //filterable: { mode: "row" },
                checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
                colonneCustomKendoGrid: [
                    //{
                    //    command: {
                    //        template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoElemento(this.closest('tr'),this.closest('.k-grid'))>Info</div>" +
                    //            "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaElemento(this.closest('tr'),this.closest('.k-grid'))>Modifica</div>" +
                    //            "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaElemento(this.closest('tr'),this.closest('.k-grid'))>Cancella</div>"
                    //    }, title: "Azioni", width: "97px"
                    //}
                ]

            };
            var funzioniPrimaDopoEventi = {
                funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundedRighe,
                funzioneDaChiamareDopoEdit: Con_onEdit
            };
            var mostraRigheCancellate = true;
            var colonneDisabilitateSoloInModifica = [];

            

            var KendoMacchine = creaKendoGrid(divKendoConfigurazioni, // rappresenta l'ID del div a cui si associa la griglia
                funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
                idModel, // chiave riga 
                campiKendoModel, // campi modello
                colonneKendoGrid, // colonne da mostrare
                parametriPerLettura, // parametri da passare alla lettura
                parametriDataSource, // parametri data source { chiave - valore}
                parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
                funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
                mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
                colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
            );

            griglia = $('#' + divKendoConfigurazioni).data("kendoGrid");
        }

        function kendo_Operazioni_onDataBoundedRighe(e) {
            WaitFrame.hide();
        }

        function kConfigurazioneBDN_Submit(e) {
            let righe = griglia.dataSource.data();
            let parametri = { str: JSON.stringify(righe) };
            ajaxAgronica(
                "fabbricato_edit.aspx/Aggiungi_Configurazione_BDN",
                JSON.stringify(parametri),
                function (risposta) {
                    e.success();
                }, null)
        }

        function Con_onEdit(e) {
            if (e.model.dirty == false && e.model.id == 0) {
                e.model.Validita_Fine = new Date(2100, 11, 31);
                e.model.Validita_Inizio = new Date(1900, 0, 1);
                //griglia.refresh();
            }
        }

        function kConfigurazioneBDNModel(modifica){
            var kendo_model = {
                "ID": { "editable": false, "type": "number" },
                "Piva": { "editable": false, "type": "string" },
                "sa_cod": { "editable": false, "type": "string" },
                "STA_NUM": { "editable": false, "type": "string" },
                "CF_Detentore": { "editable": modifica, "type": "string", validation: { required: true } },
                "CF_Proprietario": { "editable": modifica, "type": "string", validation: { required: true } },
                "RagSoc_Detentore": { "editable": false, "type": "string" },
                "RagSoc_Proprietario": { "editable": false, "type": "string" },
                "Validita_Inizio": { "editable": modifica, "type": "date", defaultValue: new Date(1900, 0, 1) },
                "Validita_Fine": { "editable": modifica, "type": "date", defaultValue: new Date(2100, 11, 31) }
            };
            return kendo_model;
        }

        function kConfigurazioneBDNColumns(){
            return [
                {
                    "field": "CF_Detentore",
                    "title": TraduzioneMultiResx(fabbricatoEditResx, "CodFiscDetentore", "C.F. Detentore"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    }
                },
                //{
                //    "field": "RagSoc_Proprietario",
                //    "title": "Proprietario",
                //    "filterable": {
                //        "multi": true,
                //        "search": true
                //    }
                //},
                {
                    "field": "CF_Proprietario",
                    "title": TraduzioneMultiResx(fabbricatoEditResx, "CodFiscProprietario", "C.F. Proprietario"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    }
                },
                //{
                //    "field": "RagSoc_Proprietario",
                //    "title": "Proprietario",
                //    "filterable": {
                //        "multi": true,
                //        "search": true
                //    }
                //},
                {
                    "field": "Validita_Inizio",
                    "title": TraduzioneMultiResx(fabbricatoEditResx, "ValiditàInizio", "Validita Inizio"),
                    filterable: {
                        ui: "datepicker"
                    },
                    template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #'
                },
                {
                    "field": "Validita_Fine",
                    "title": TraduzioneMultiResx(fabbricatoEditResx, "ValiditàFine", "Validita Fine"),
                    filterable: {
                        ui: "datepicker"
                    },
                    template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #'
                },
                //{
                //    "field": "Data_Modifica",
                //    "title": "Data Modifica",
                //    filterable: {
                //        ui: "datepicker"
                //    },
                //    "format": "{0:dd/MM/yyyy}"
                //},
                //{
                //    "field": "Utente_Modifica",
                //    "title": "Utente Modifica",
                //    "filterable": {
                //        "multi": true,
                //        "search": true
                //    }
                //},
                //{
                //    "field": "Data_Creazione",
                //    "title": "Data Creazione",
                //    filterable: {
                //        ui: "datepicker"
                //    },
                //    "format": "{0:dd/MM/yyyy}"
                //},
                //{
                //    "field": "Utente_Creazione",
                //    "title": "Utente Creazione",
                //    "filterable": {
                //        "multi": true,
                //        "search": true
                //    }
                //},
            ];
        }

        function kConfigurazioneBDNrows(){
            return initConfigurazioniBDN;
        }

    </script>
    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
