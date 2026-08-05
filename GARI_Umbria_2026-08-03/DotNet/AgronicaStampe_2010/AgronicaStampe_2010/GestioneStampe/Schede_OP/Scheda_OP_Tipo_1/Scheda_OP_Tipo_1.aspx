<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Scheda_OP_Tipo_1.aspx.vb" Inherits="AgronicaStampe_2010.Scheda_OP_Tipo_1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="../../../App_Styles/AgronicaStyle.css" rel="stylesheet" type="text/css" />
    <style type='text/css'>
    ol,li {
    margin: 0;
    padding: 0;
}

ol {
    counter-reset: foo;
    display: table;
}

li 
{
    
    list-style: none;
    counter-increment: foo;
    display: table-row;
    
}

li::before {
    content: counter(foo) ".";
    display: table-cell;
    text-align: right;
    padding-right: .3em;
}
</style>



</head>
<body>
    <form id="form1" runat="server">
     
     <%--<style type='text/css'>li { text-decoration:underline;} </style>--%>
     <ul>
     <li>aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa</li>
     </ul>
    </form>
</body>
</html>
