<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="InterfacciaWeb_TEST.WebForm1" %>

<%@ Register Assembly="AgronicaControlliGIS" Namespace="AgronicaControlliGIS" TagPrefix="acg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyA2cJIYR1WcTQgp1VudoETCNvbfVIoG-ZQ&sensor=false&libraries=drawing,geometry"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    
    <script src="_Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="_Scripts/jquery-ui-1.8.18.custom.min.js"></script>
    <link href="_Styles/redmond/jquery-ui-1.8.18.custom.css" rel="stylesheet" type="text/css" />



    <style type="text/css">
        #contenitore_tool
        {
            width: 65px;
        }
        #sidebar
        {
            width: 150px;
            padding: 0.5em;
            border-color: #32CD32;
            border-style: dashed;
            border-width: 1px;
            z-index: 10000;
            background-color: rgba(255, 255, 255, 0.5);
        }
        #sitebar_indirizzo
        {
            width: 228px;
            height: 75px;
            padding: 0.5em;
            border-color: #32CD32;
            border-style: dashed;
            border-width: 1px;
            z-index: 10000;
            background-color: rgba(255, 255, 255, 0.5);
        }
        #map, html
        {
            padding: 0;
            margin: 0px;
            height: 100%;
        }
        body, Form
        {
            display: inline;
            font-family: Verdana, Helvetica, sans-serif;
            font-size: 10px;
            width: 100%;
            height: 100%;
            margin: 0px;
        }
        #color-palette
        {
            clear: both;
        }
        .color-button
        {
            width: 14px;
            height: 14px;
            font-size: 0;
            margin: 2px;
            float: left;
            cursor: pointer;
        }
        #delete-button
        {
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width:100%; height:100%">       
        <acg:V_M ID="Visualizzatore_Mappa" runat="server"></acg:V_M>

       
    </form>
</body>
</html>
