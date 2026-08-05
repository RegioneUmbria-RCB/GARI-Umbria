<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Bilancio_NPK.aspx.vb" Inherits="PianoConcimazione_2017.Bilancio_NPK" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head runat="server">
    <title></title>
    <!-- LinkxIframe -->
    <script type="text/javascript" src="<%=PATH_GIASBASE %>AA_Script/fancybox/jquery.mousewheel-3.0.4.pack.js?<% =Application("GiasVersioneCorrente")%>" ></script>
    <script type="text/javascript" src="<%=PATH_GIASBASE %>AA_Script/fancybox/jquery.fancybox-1.3.4.pack.js?<% =Application("GiasVersioneCorrente")%>" ></script>
    <link rel="stylesheet" type="text/css" href="<%=PATH_GIASBASE %>AA_Script/fancybox/jquery.fancybox-1.3.4.css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <a id="LinkxIframe" href="" style="display: none">LinkxIframe</a>   
        </div>
    </form>
</body>
</html>
