<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/UmaBootstrap.Master" CodeBehind="ConfigurazioneUMA.aspx.vb" Inherits="AgronicaUMA.ConfigurazioneUMA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorCell {
            background-color: #dc143c57;
        }
        .k-grid .k-column-title {
            white-space: normal;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="panelArea" class="panel-group searchArea" style="opacity: 0;">
        <div class="jumbotron">
            <div class="container" style="width: 100%; padding: 0">
                <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="form-horizontal">
                            <div class="input-group">
                               <label class="input-group-addon" id="Lbl_inizio_upload" for="InizioValidita">
                                    Inizio Validità
                                </label>
                                <input id="InizioValidita" name="InizioValidita" class="kendoCalendar" maxlength="10" />
                            </div>
                        </div>
                    </div>

                   <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="form-horizontal">
                            <div class="input-group">
                               <label class="input-group-addon" id="Lbl_Fine_upload" for="FineValidita">
                                    Fine Validità
                                </label>
                                <input id="FineValidita" name="FineValidita" class="kendoCalendar" maxlength="10" />
                            </div>
                        </div>
                    </div>
                    <div class="btn btn-success" id="btn_carica_elenco">
                        <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Cerca
                        </span>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="container_elenco k-content" style="padding: 0;">
                            <div id="tabstrip_elenco" style="display: none">
                                <ul style="text-transform: uppercase;" data-tabs="tabs">
                                    <li class="k-state-active k-active">
                                        <asp:Localize Text="Macrousi - Lavorazioni" runat="server">Macrousi - Lavorazioni</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="Lavorazioni Alternative" runat="server">Lavorazioni Alternative</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="Setup" runat="server">Setup</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="Configurazione Allevamenti" runat="server">Configurazione Allevamenti</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="Date Rendicontazioni" runat="server">Date Rendicontazioni</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="UF Colture" runat="server">UF Colture</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="Elenco Macrousi" runat="server">Elenco Macrousi UMA</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="Elenco Lavorazioni" runat="server">Elenco Lavorazioni UMA</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="Elenco Allevamenti" runat="server">Elenco Allevamenti UMA</asp:Localize>
                                    </li>
                                    <li>
                                        <asp:Localize Text="Associazioni Macrousi" runat="server">Associazioni Macrousi</asp:Localize>
                                    </li>
                                </ul>

                                <div id="tabLavUMA">
                                    <div class="row">
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="grdConfigurazioniUMA"></div>
                                        </div>
                                    </div>
                                </div>

                                <div id="tabLavAlt">
                                    <div class="">
                                        <div class="row">
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdLavorazioniAlternative"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                                <div id="tabSetup">
                                    <div class="">
                                        <div class="row">
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdSetup"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="tabConfigurazioneAllevamenti">
                                    <div class="">
                                        <div class="row">
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdUMAConfigurazioneAllevamenti"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="tabConfigurazioneDateRendicontazioni">
                                    <div class="">
                                        <div class="row">
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdConfigurazioneDateRendicontazioni"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="tabConfigurazioneUF">
                                    <div class="">
                                        <div class="row">
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdConfigurazioneUF"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="tabElencoMacrousiUMA">
                                    <div class="">
                                        <div class="row">
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdElencoMacrousiUMA"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="tabElencoLavorazioniUMA">
                                    <div class="">
                                        <div class="row">
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdElencoLavorazioniUMA"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="tabElencoAllevamentiUMA">
                                    <div class="">
                                        <div class="row">
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdElencoAllevamentiUMA"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                                <div id="tabAssociazioniMacrousiUMA">
                                    <div class="">
                                        <div class="row" style="margin-left: 5px; margin-top: 10px">

                                           <%-- <div class="col-lg-3 col-md-3 col-sm-12 text-center">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <label class="input-group-addon control-label alert-info" id="lblMacrousoUMA" for="dllMacrousoUMA">
                                                                Macrouso UMA
                                                            </label>
                                                            <input type="text" id="dllMacrousoUMA" name="dllMacrousoUMA" class="form-control">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>--%>

                                            <div class="btn btn-success" id="btnModificaMassiva" style="margin-left: 10px">
                                                <span class="lampeggiante">Modifiche Massive
                                                </span>
                                            </div>
                                        </div>

                                        <div class="row">                                            
                                            
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="grdAssociazioniMacrousiUMA"></div>
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
    <!-- hidden fields -->
    <input type="hidden" id="hfKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hfPaginaRedirect" runat="server" />
    <input type="hidden" id="hfPaginaRedirect_Codificata" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoLettura" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura" runat="server" />



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("configurazioniUMA_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("configurazioneUMA_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("configurazioneUMA.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("configurazioneUMA_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>

    <script type="text/javascript">

    var objP_server = '<%=objparametri_server_string %>';
    
    </script>

</asp:Content>
