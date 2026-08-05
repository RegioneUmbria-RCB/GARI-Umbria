<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="MenuBS_2017.aspx.vb" Inherits="AgroAgenda_2010.MenuBS_2017" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <%--<link href="MenuBS_2017.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" rel="stylesheet" />--%>

    <asp:PlaceHolder ID="Meteo_headerPlaceHeader" runat="server"></asp:PlaceHolder>

    <%--<style>
        .row-centered {
            text-align:center;
        }
        .col-centered {
            display:inline-block;
            float:none;
            text-align:left;
            margin-right:-4px;
        }
    </style>--%>

    <style>
        .elemento-nascosto {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- spazio superiore sotto all'header-->
    <!-- qui ci vanno il widget per il meteo, uno spazio per gli alert ed una sezione per le news -->
    <div id="contenitore_principale" class="container">

        <%--<div id="widget" style="margin-bottom:10px;">

            <div class="row row-centered">

                <!-- widget del meteo -->
                <div class="col-xs-4 col-md-4 col-lg-2 col-centered" id="widget_meteo">
                    <cc1:AgroMeteo ID="meteo" runat="server" modalita="dashboard"/>
                    <div id="error-msg" class="widgetemergenza" style="display:none;">
                        <div style="position: relative; overflow: hidden; height: 100%; font-size: larger; font-weight: bolder;">
                            <div class="METEO-marquee-text" style="position: absolute; white-space: nowrap; top: 50%; opacity: 1;"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PrevisioniMeteoNonDisponibili %>" runat="server">Previsioni meteo non disponibili</asp:Localize></div>
                        </div>
                    </div>
                </div>

                <!-- widget delle emergenze -->
                <div class="col-xs-4 col-md-4 col-lg-2 col-centered" id="widget_meteo_2">
                    <div class='widgetemergenza' id="widget-meteo-2"></div>
                </div>

                <div class="col-xs-4 col-md-4 col-lg-2 col-centered" id="widget_emergenze">
                    <div class='widgetemergenza' id="widget-gauges"></div>
                </div>

                <!-- widget allarmi -->
                <div class="col-xs-12 col-md-12 col-lg-6 col-centered" id="widget_allarmi">
                </div>

            </div>           
        </div>--%>

        <!-- devo avere 3 colonne: una stretta per il menu, una media per le categoria ed una larga per i preferiti-->
        <div id="dashboard">

            <div class="row">
                
                <%--<div class="col-xs-12 col-md-12 col-lg-2" id="dashboard_menu" style="margin-bottom:10px;">
                </div>--%>

                <div class="col-xs-12 col-md-12 col-lg-2" style="padding-left:0px;padding-right:0px;">

                    <div class="row">

                        <!-- widget del meteo -->
                        <div class="col-xs-4 col-md-4 col-lg-12" style="margin-top:5px;margin-bottom:5px;" id="widget_meteo">
                            <cc1:AgroMeteo ID="meteo" runat="server" modalita="dashboard" altezza="100px" />
                            <div id="error-msg" class="widgetemergenza" style="display:none;">
                                <div style="position: relative; overflow: hidden; height: 100%; font-size: larger; font-weight: bolder;">
                                    <div class="METEO-marquee-text" style="position: absolute; white-space: nowrap; top: 50%; opacity: 1;"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PrevisioniMeteoNonDisponibili %>" runat="server">Previsioni meteo non disponibili</asp:Localize></div>
                                </div>
                            </div>
                        </div>

                        <!-- widget delle emergenze -->
                        <div class="col-xs-4 col-md-4 col-lg-12" style="margin-top:5px;margin-bottom:5px;" id="widget_meteo_2">
                            <div class='widgetemergenza' style="height:100px;" id="widget-meteo-2"></div>
                        </div>
                        <% if DSSDifesa_Autorizzato Then %>
                        <div class="col-xs-4 col-md-4 col-lg-12" style="margin-top:5px;margin-bottom:5px;" id="widget_emergenze">
                            <div class='widgetemergenza' style="height:100px;" id="widget-gauges"></div>
                        </div>
                        <% End If %>


                        <div class="col-xs-4 col-md-4 col-lg-12" style="margin-top:5px;margin-bottom:5px;" id="widget_suolo">
                            <div class="widgetemergenza" style="height:100px;" id="widget-suolo"></div>
                        </div>

                        <div class="col-xs-12" style="margin-top:5px;margin-bottom:5px;" id="dashboard_menu">
                        </div>

                    </div>
                    
                </div>

                <div class="col-xs-6 col-sm-6 col-lg-4" style="padding-left:0px;padding-right:0px;">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-lg-6" id="macrocategoria_prima_colonna"></div>
                        <div class="col-xs-12 col-sm-6 col-lg-6" id="macrocategoria_seconda_colonna"></div>         
                    </div>
                </div>

                <div class="col-xs-6 col-sm-6 col-lg-6">
                    <input autocomplete="off" onkeydown="if (event.keyCode == 13) return false;" onkeyup="CercaSezioni(this.value, 'false');" type="text" placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, CercaVoceDiMenu %>' runat='server'></asp:Localize>" class="ricercavocemenu" id="preferiti_ricerca_voce" style="margin-bottom: 5px;">
                    <div class='btn btn-default buttonpreferiti' id='gestione_preferiti_bottone_dashboard' onclick='GestionePreferiti();'><span class='testo_pulsante_preferiti'><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestionePreferiti %>" runat="server">Gestione Preferiti</asp:Localize><i class="fa fa-star-o" aria-hidden="true"></i></span></div>
                    <div class='btn btn-default buttonpreferiti' id='preferiti_bottone_dashboard' onclick='LeggiPreferitiDash();' style='display:none;'><span class='testo_pulsante_preferiti'><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, VaiAiPreferiti %>" runat="server">Vai ai Preferiti</asp:Localize><i class="fa fa-star-o" aria-hidden="true"></i></span></div>
                    <div id="contenitore_sezioni"> </div>
                    <%-- <div class='btn btn-default buttonpreferiti' id='menu_precedente_bottone_dashboard' onclick='GoToMenuPrecedente();' style="margin-top: 5px;"><span class='testo_pulsante_preferiti'>Menu Precedente <i class="fa fa-undo" aria-hidden="true"></i></span></div> --%>
                </div>
            </div>
        </div>        
    </div>

    <!-- gestione preferiti -->
    <div id="gestione_preferiti" style="display:none;">
        <div id="ricerca_preferiti">
            <input autocomplete="off" onkeydown="if (event.keyCode == 13) return false;" onkeyup="CercaPreferiti(this.value);" type="text" placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, CercaVoceDiMenu %>' runat='server'></asp:Localize>" class="ricercavocemenu" id="ricerca_voce_preferiti" style="margin-bottom: 5px;">
        </div>
        <div id="contenitore_preferiti">        
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript">
        var flagFiltroAziende =<%= IIf(FiltroAziende, "true", "false") %>;
        var lenFiltroAziende = <%= LenFiltroAziende %>;
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("MenuBS_2017.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("MenuBS_2017_jQueryDocReady.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("~/DataAnalisiBI/Meteo/Meteo_resx.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("~/DataAnalisiBI/Meteo/Widget/WidgetCommon.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("~/DataAnalisiBI/Meteo/Widget/StyleLoader.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("~/DataAnalisiBI/Meteo/Widget/RiepilogoMeteo.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("~/DataAnalisiBI/Meteo/Widget/DSS_Summary.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("~/DataAnalisiBI/Meteo/Widget/MonitorSuolo.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveUrl("~/DataAnalisiBI/Meteo/Meteo_Chart.js")) %>"></script>
    
    <%= RedirectPagina %>

</asp:Content>