<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" 
    CodeBehind="DSS_Fertirrigazione.aspx.vb" Inherits="AgroAgenda_2010.DSS_Fertirrigazione" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <link href="DSS_Fertirrigazione.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet"/>
    
    <script src="<%= srv_gm %>" type="text/javascript"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="id_MainContainer" class="container" style="position: absolute; left: 0; top: 0; width: 100%; height: 100%; padding: 0px; overflow-y: auto; background-color: white;">
        <div id="DSS_Irrigazione_Main" style="width:100%; padding:10px;">
            <div id="DSS_Irrigazione_Anbi" style="margin-bottom: 15px; opacity: 0;">
                <div id="__anbi__" class="k-block k-shadow anbi-collapsible">
                    <div class="anbi-content">
                        <img style="align-content:center" src="images/logoifanbi.png" />
                        <br />
                        <div class="anbi-text">
                            <div class="anbi-short"></div>
                            <div class="anbi-full"></div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="filtroSpecie" class="transparent" style="margin-bottom: 15px;">
                <input type="text" id="cmbSpecieVegetale" value="" style="width: -webkit-fill-available;" />
            </div>

            <div id="DSS_Irrigazione_Indicatori">

            </div>
            <div class="modal" id="modal">
                <div id="obfuscator" class="obfuscator" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript">
        var url_meteows = "../Meteo/MeteoWS.aspx";
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo/Widget/StyleLoader.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Fertirrigazione_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Fertirrigazione.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Fertirrigazione_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo/Meteo_resx.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo/Meteo_Chart.js") %>"></script>

</asp:Content>
