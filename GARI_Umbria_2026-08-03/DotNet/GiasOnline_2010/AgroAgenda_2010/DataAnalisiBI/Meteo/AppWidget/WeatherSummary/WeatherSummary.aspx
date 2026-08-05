<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WeatherSummary.aspx.vb" Inherits="AgroAgenda_2010.WeatherSummary" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>

    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />

    <link rel="stylesheet" href="../../../../Styles/font-awesome-4.7.0/css/font-awesome.min.css" media="screen" />
    <link rel="stylesheet" href="../../../../Styles/bootstrap.min.css" media="screen" />
    <link rel="stylesheet" href="<%=PATH_GIASBASE %>agronica/Styles/bootstrap_AGRONICA.css?<% =Application("GiasVersioneCorrente")%>" media="screen" />
    <link rel="stylesheet" href="./WeatherSummary.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />

    <link href="../../styles/site.css" rel="stylesheet" type="text/css" />    
    <asp:PlaceHolder ID="kendoPlaceHeader" runat="server"></asp:PlaceHolder>
    <cc2:jquery id="jquery" runat="server" />

    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <cc2:bootstrap id="bootstrap" runat="server" />
        <cc2:Agrokendo id="agroKendo" runat="server" />
        <cc2:agronicabase id="AgronicaBase" runat="server" />
        <asp:PlaceHolder ID="versioneMasterPlaceHolder" runat="server"></asp:PlaceHolder>
        <cc2:AgroVersioneMaster id="agroVersioneMaster" runat="server" />

        <div id="widget"></div>

        <script type="text/javascript">
            var piva = "<%= piva %>";
            var weatherSummaryAllowed = <%=weatherSummaryAllowed %>;
            var localizationPageUrl = "<%= ResolveClientUrl("~/Localization.aspx") %>";
            const GiasVersioneMaster = '<%=Master_versione %>';
        </script>

        <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../../Meteo_Resx.js") %>"></script>
        <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("doc-ready.js") %>"></script>
        <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../../Widget/RiepilogoMeteo.js") %>"></script>
        <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../../Widget/WidgetCommon.js") %>"></script>
        <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../../Widget/StyleLoader.js") %>"></script>
        <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../../Meteo_Chart.js") %>"></script>
    </form>
</body>
</html>
