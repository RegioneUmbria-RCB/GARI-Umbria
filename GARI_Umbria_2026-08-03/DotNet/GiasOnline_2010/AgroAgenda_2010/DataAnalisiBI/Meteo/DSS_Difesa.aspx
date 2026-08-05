<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="DSS_Difesa.aspx.vb" Inherits="AgroAgenda_2010.DSSDifesa" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <link rel="stylesheet" href="Meteo_CommonStyles.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />
    <link rel="stylesheet" href="DSS_Difesa.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />
    <link rel="stylesheet" href="Widget/DSS_Gauges.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />

    <script src="<%= srv_gm %>" type="text/javascript"></script>
    <script>
        function toggleMenu() {
            const sidebar = document.getElementById('DivFiltri');
            sidebar.classList.toggle('hidden-sidebar');
            const sidebarAction = document.getElementById('xoRicercaDocToggleFiltri');
            sidebarAction.classList.toggle('hidden-sidebar');
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        /** Inizio stili per test menu filtri laterale */
        #DivFiltri {
            margin-top: 0 !important;
        }
        .btn.gias-btn-toggle-filter-sidebar {
            right: 75vw;
        }
        .gias-right-sidebar-filters {
            width: 75vw;
        }
        #DivFiltri .form-layout .form-label { 
            font-size: 14px; 
            font-family: 'Lato', sans-serif;
            color: #575757;
            font-weight: 400;
            text-transform: capitalize;
            background: none; 

        }
        #elab-periodo .k-textbox { 
            font-size: 14px; 
        }
        #elab-periodo .k-datepicker.k-input { height: 31px; }
    </style>

    <div id="id_MainContainer" class="container" style="padding: 0px;">

        <!-- SIDEBAR SEARCH -->
        <% If Master.Master_versione = "2022" Then %>
            
            <!-- FILTRI Matteo qui iniziano gli elementi che implementano i filtri -->
            <div id="xoRicercaDocToggleFiltri" class="btn gias-btn-primary gias-btn-toggle-filter-sidebar" onclick="toggleMenu()">
                <span>
                    <svg role="img" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="sliders" class="svg-inline--fa fa-sliders fa-lg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path fill="currentColor" d="M0 416c0-17.7 14.3-32 32-32l54.7 0c12.3-28.3 40.5-48 73.3-48s61 19.7 73.3 48L480 384c17.7 0 32 14.3 32 32s-14.3 32-32 32l-246.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 448c-17.7 0-32-14.3-32-32zm192 0c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zM384 256c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zm-32-80c32.8 0 61 19.7 73.3 48l54.7 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-54.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 288c-17.7 0-32-14.3-32-32s14.3-32 32-32l246.7 0c12.3-28.3 40.5-48 73.3-48zM192 64c-17.7 0-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32s-14.3-32-32-32zm73.3 0L480 64c17.7 0 32 14.3 32 32s-14.3 32-32 32l-214.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 128C14.3 128 0 113.7 0 96S14.3 64 32 64l86.7 0C131 35.7 159.2 16 192 16s61 19.7 73.3 48z"></path></svg>
                </span>
            </div>
            <div class="row gias-right-sidebar-filters " id="DivFiltri">
 
               
            <div class="responsive-container-centered">
                <div class="responsive-content">

                    <div id="main-grid" class="form-layout">

                        <div class="form-label">
                            <span>
                                <asp:Localize Text="<%$ Resources: Geolocalizzazione %>" runat="server">Geolocalizzazione</asp:Localize>
                            </span>
                        </div>
                        <div>
                            <div id="geo-pos-edit"></div>
                        </div>

                        <div class="form-label">
                            <span>
                                <asp:Localize Text="<%$ Resources: CategoriaStazioneMeteo %>" runat="server">Categoria stazioni</asp:Localize>
                            </span>
                        </div>
                        <div>
                            <input type="text" name="cmbTipoSorgente" id="cmbTipoSorgente" value="" style="width: -webkit-fill-available;" />
                        </div>

                        <div class="form-label">
                            <span>
                                <asp:Localize Text="<%$ Resources: StazioneMeteo %>" runat="server">Stazione meteo</asp:Localize>
                            </span>
                        </div>
                        <div>
                            <input type="text" name="cmbOrigineDati" id="cmbOrigineDati" value="" style="width: -webkit-fill-available;" />
                        </div>


                        <div class="form-separator"></div>

                        <div class="form-label">
                            <span>
                                <asp:Localize Text="<%$ Resources: PeriodoModello %>" runat="server">Periodo elaborazione</asp:Localize>
                            </span>
                        </div>
                        <div style="display: flex;">
                            <div id="elab-periodo" style="flex-grow: 1;"></div>
                            <%If Autorizzato_Impostazione_Modelli Then %>
                            <div id="config-modelli" style="margin-left: 10px;"></div>
                            <%End If %>
                        </div>

                        <div class="form-label">
                            <span>
                                <asp:Localize Text="<%$ Resources: SpecieVegetale %>" runat="server">Specie vegetale</asp:Localize>
                            </span>
                        </div>
                        <div>
                            <select id="cmbSpecieVegetale" style="width: -webkit-fill-available;"></select>
                        </div>

                        <div class="form-label">
                            <span>
                                <asp:Localize Text="<%$ Resources: ModelloAvversita %>" runat="server">Modello avversità</asp:Localize>
                            </span>
                        </div>
                        <div>
                            <select id="cmbAvModAlg" style="width: -webkit-fill-available;"></select>
                        </div>
                    </div>

                    <div style="margin-top: 25px;">
                        <div class="btn btn-success xonne-btn-primary" id="btn_Modelli_Previsionali" style="width: -webkit-fill-available;">
                            <span class="fa fa-cogs"></span>
                            <span>
                                <asp:Localize Text="<%$ Resources: ElaboraModelloPrevisionale %>" runat="server">Elabora modello previsionale</asp:Localize>
                            </span>
                        </div>
                    </div>
                       
                </div>
            </div>

                </div>
            <% End If %>
        <!-- FINE SIDEBAR SEARCH -->
        
        <div id="tabstrip" class="meteo-transparent gias-mt-20">
            <ul>
                <li id="Indicatori" class="k-state-active k-active">
                    <asp:Localize meta:resourcekey="Indicatori" runat="server">Indicatori</asp:Localize>
                </li>
                <% If Master.Master_versione <> "2022" Then %>
                <li id="Ricerca_Intestazione">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize>
                </li>
                <% End If %>
                <li id="Dati_Intestazione">
                    <asp:Localize Text="<% Resources: EsitoModelloPrevisionale %>" runat="server">Esito modello previsionale</asp:Localize>
                </li>
            </ul>

            <!--TAB INDICATORI-->
            <div id="tab-indicatori">
                <div id="ID_Gauges" style="width: 100%; height: 100%;">
                </div>
            </div>
            
            <% If Master.Master_versione <> "2022" Then %>
            <!--TAB RICERCA-->
            <div id="tab-ricerca"> 
                <div class="responsive-container-centered">
                    <div class="responsive-content">

                        <div id="main-grid" class="form-layout">

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: Geolocalizzazione %>" runat="server">Geolocalizzazione</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <div id="geo-pos-edit"></div>
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: CategoriaStazioneMeteo %>" runat="server">Categoria stazioni</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <input type="text" name="cmbTipoSorgente" id="cmbTipoSorgente" value="" style="width: -webkit-fill-available;" />
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: StazioneMeteo %>" runat="server">Stazione meteo</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <input type="text" name="cmbOrigineDati" id="cmbOrigineDati" value="" style="width: -webkit-fill-available;" />
                            </div>


                            <div class="form-separator"></div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: PeriodoModello %>" runat="server">Periodo elaborazione</asp:Localize>
                                </span>
                            </div>
                            <div style="display: flex;">
                                <div id="elab-periodo" style="flex-grow: 1;"></div>
                                <%If Autorizzato_Impostazione_Modelli Then %>
                                <div id="config-modelli" style="margin-left: 10px;"></div>
                                <%End If %>
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: SpecieVegetale %>" runat="server">Specie vegetale</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <select id="cmbSpecieVegetale" style="width: -webkit-fill-available;"></select>
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: ModelloAvversita %>" runat="server">Modello avversità</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <select id="cmbAvModAlg" style="width: -webkit-fill-available;"></select>
                            </div>
                        </div>

                        <div style="margin-top: 25px;">
                            <div class="btn btn-success xonne-btn-primary" id="btn_Modelli_Previsionali" style="width: -webkit-fill-available;">
                                <span class="fa fa-cogs"></span>
                                <span>
                                    <asp:Localize Text="<%$ Resources: ElaboraModelloPrevisionale %>" runat="server">Elabora modello previsionale</asp:Localize>
                                </span>
                            </div>
                        </div>
                       
                    </div>
                </div>
            </div>
             <% End If %>
            <!--TAB DATI-->
            <div id="tab-dati">
                <div id="divKendoOut">
                </div>
            </div>

        </div>

    </div>

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdExternalLoad" runat="server" />
    <input type="hidden" id="hdParametri" runat="server" />
    <input type="hidden" id="hdRagSoc" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">

        var url_meteo_ws = "./MeteoWS.aspx";
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdExternalLoad = "#<%=hdExternalLoad.ClientID() %>";
        var cIdParametri = "#<%=hdParametri.ClientID() %>";
        var AutorizzatoImpostazioneModelli = <%= Autorizzato_Impostazione_Modelli.ToString.ToLower %>;
        var Rag_Soc = "#<%=hdRagSoc.ClientID() %>";
        var vegCod = <%= vegCod%>;

        var NascondiFiltri = "<%= NascondiFiltri%>";
        if (NascondiFiltri == 1) {
            const sidebar = document.getElementById('DivFiltri');
            sidebar.classList.add('hidden-sidebar');
            const sidebarAction = document.getElementById('xoRicercaDocToggleFiltri');
            sidebarAction.classList.add('hidden-sidebar');
            sidebarAction.style.display = 'none';
        }

        var gestioneWaitFrame = true;
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_resx.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Difesa_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Difesa.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Difesa_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Difesa_ConfigurazioneModelli.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_CommonUI.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_Grid.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_Chart.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Widget/WidgetCommon.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Widget/StyleLoader.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Widget/DSS_Summary.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Widget/DSS_Gauges.js") %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Plugins/GeoPosEdit.js") %>"></script>

</asp:Content>
