
<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Meteo.aspx.vb" Inherits="AgroAgenda_2010.Meteo" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <link rel="stylesheet" href="Meteo_CommonStyles.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />
    <link rel="stylesheet" href="Meteo.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />

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
        #elab-periodo .k-textbox, #DivFiltri .toggle-label { 
            font-size: 14px; 
        }
        #DivFiltri .toggle-label { line-height: 2em !important; }
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
                       
                        <div class="form-layout">

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
                                    <asp:Localize Text="<%$ Resources: PeriodoMeteo %>" runat="server">Periodo</asp:Localize>
                                </span>
                            </div>
                            <div style="display: grid; grid-template-columns: 1fr auto 1fr; grid-column-gap: 10px; align-items: center;">
                                <div>
                                    <input type="text" id="txt_DataDa" class="kendoCalendar" maxlength="10" style="width:100%" autocomplete="off" />
                                </div>
                                <div>
                                    <span class="k-icon k-i-information info-forecast"></span>
                                </div>
                                <div>
                                    <input type="text" id="txt_DataA" class="kendoCalendar" maxlength="10" style="width:100%" autocomplete="off" />
                                </div>                                    
                            </div>
                          
                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: FrequenzaDati %>" runat="server">Frequenza dati</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <label class="toggle-switch">
                                    <input id="switch-freq-dati" type="checkbox" />
                                    <div class="toggle-back">
                                        <div class="toggle"></div>
                                        <div class="toggle-label on">
                                            <asp:Localize Text="<%$ Resources: Oraria %>" runat="server">Oraria</asp:Localize>
                                        </div>
                                        <div class="toggle-label off">
                                            <asp:Localize Text="<%$ Resources: Giornaliera %>" runat="server">Giornaliera</asp:Localize>
                                        </div>
                                    </div>
                                </label>
                            </div>

                            <div class="form-label" id="lbl_serie_storiche">
                                <span>
                                    <asp:Localize Text="<%$ Resources: ConfrontaConAnniPreced %>" runat="server">Confronto anni</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <select id="serie_storiche"></select>
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: SogliaSommaTermica %>" runat="server">Soglia somma termica</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <input type="text" name="txt_Soglia_Germinazione" id="txt_Soglia_Germinazione" style="width: 100%;" autocomplete="off" />
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: SogliaFabbisognoFreddo %>" runat="server">Soglia fabbisogno freddo</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <input type="text" name="txt_Soglia_FabbisognoFreddo", id="txt_Soglia_FabbisognoFreddo" style="width: 100%;" autocomplete="off" />
                            </div>
                        </div>
                        
                        <div style="margin-top: 25px;">
                            <div class="btn btn-success xonne-btn-primary" id="btn_ricerca_meteo" style="width: -webkit-fill-available;">
                                <span class="fa fa-cogs"></span>
                                <span>
                                    <asp:Localize Text="<%$ Resources: DatiMeteo %>" runat="server">Dati meteo</asp:Localize>
                                </span>
                            </div>
                        </div>

                    </div>
                </div>
            
                </div>
            <% End If %>
        <!-- FINE SIDEBAR SEARCH -->

        <div id="tabstrip" class="meteo-transparent gias-mt-20 dss-analisi-dati-meteo-container">
            <ul>
                <li id="Ricerca_Intestazione" class="k-state-active k-active gias-display-none">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize>
                </li>
                <li id="Dati_Intestazione">
                    <asp:Localize Text="<% Resources: DatiMeteo %>" runat="server">Dati meteo</asp:Localize>
                </li>
            </ul>

            
            <!--TAB RICERCA-->
            <div id="tab-ricerca"> 
                <% If Master.Master_versione <> "2022" Then %>
                <div class="responsive-container-centered">

                    <div class="responsive-content">
                       
                        <div class="form-layout">

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
                                    <asp:Localize Text="<%$ Resources: PeriodoMeteo %>" runat="server">Periodo</asp:Localize>
                                </span>
                            </div>
                            <div style="display: grid; grid-template-columns: 1fr auto 1fr; grid-column-gap: 10px; align-items: center;">
                                <div>
                                    <input type="text" id="txt_DataDa" class="kendoCalendar" maxlength="10" style="width:100%" autocomplete="off" />
                                </div>
                                <div>
                                    <span class="k-icon k-i-information info-forecast"></span>
                                </div>
                                <div>
                                    <input type="text" id="txt_DataA" class="kendoCalendar" maxlength="10" style="width:100%" autocomplete="off" />
                                </div>                                    
                            </div>
                          
                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: FrequenzaDati %>" runat="server">Frequenza dati</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <label class="toggle-switch">
                                    <input id="switch-freq-dati" type="checkbox" />
                                    <div class="toggle-back">
                                        <div class="toggle"></div>
                                        <div class="toggle-label on">
                                            <asp:Localize Text="<%$ Resources: Oraria %>" runat="server">Oraria</asp:Localize>
                                        </div>
                                        <div class="toggle-label off">
                                            <asp:Localize Text="<%$ Resources: Giornaliera %>" runat="server">Giornaliera</asp:Localize>
                                        </div>
                                    </div>
                                </label>
                            </div>

                            <div class="form-label" id="lbl_serie_storiche">
                                <span>
                                    <asp:Localize Text="<%$ Resources: ConfrontaConAnniPreced %>" runat="server">Confronto anni</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <select id="serie_storiche"></select>
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: SogliaSommaTermica %>" runat="server">Soglia somma termica</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <input type="text" name="txt_Soglia_Germinazione" id="txt_Soglia_Germinazione" style="width: 100%;" autocomplete="off" />
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: SogliaFabbisognoFreddo %>" runat="server">Soglia fabbisogno freddo</asp:Localize>
                                </span>
                            </div>
                            <div>
                                <input type="text" name="txt_Soglia_FabbisognoFreddo", id="txt_Soglia_FabbisognoFreddo" style="width: 100%;" autocomplete="off" />
                            </div>
                        </div>
                        
                        <div style="margin-top: 25px;">
                            <div class="btn btn-success" id="btn_ricerca_meteo" style="width: -webkit-fill-available;">
                                <span class="fa fa-cogs"></span>
                                <span>
                                    <asp:Localize Text="<%$ Resources: DatiMeteo %>" runat="server">Dati meteo</asp:Localize>
                                </span>
                            </div>
                        </div>

                    </div>
                </div>
                <% End If %>
            </div>
             

            <!--TAB DATI-->
            <div id="tab-dati">
                <div id="divKendoOut">
                </div>
            </div>

        </div>

    </div>

    <input type="hidden" id="hdPiva" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">

        var url_meteo_ws = "./MeteoWS.aspx";

        var cIdPiva = "#<%=hdPiva.ClientID() %>";

    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_resx.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_CommonUI.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_Grid.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Meteo_Chart.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Plugins/GeoPosEdit.js") %>"></script>

</asp:Content>
