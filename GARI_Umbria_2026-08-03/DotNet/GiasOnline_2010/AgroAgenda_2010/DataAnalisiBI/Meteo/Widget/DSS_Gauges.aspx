<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="DSS_Gauges.aspx.vb" Inherits="AgroAgenda_2010.DSS_Gauges" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        
        form, body {
            background-color: #fff !important;
        }
        
        form {
            display: contents;
        }
        
        form > #kendoWindowiFrameGeneric {
            display: none;
        }
        
        body #wrap_master {
            display: flex;
        }
        
        .container {
            padding: 0px !important;
            display: flex;
        }

    </style>


    <link rel="stylesheet" href="DSS_Gauges.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="MainContainer" class="DSS_GaugesContainer">
    </div>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
        <script type="text/javascript">
            var url_meteo_ws = "../MeteoWS.aspx";

            var gestioneWaitFrame = true;
        </script>

    <%--<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo_resx.js") %>"></script>--%>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("WidgetCommon.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("StyleLoader.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Gauges_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DSS_Gauges.js") %>"></script>
    <%--<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo_Chart.js") %>"></script>--%>

</asp:Content>
