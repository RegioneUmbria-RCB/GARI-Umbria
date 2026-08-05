<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="AgronicaStampe_2010._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="App_Styles/AgronicaStyle.css" />
    <style type="text/css">
        .style1
        {
            width: 150px;
            height: 43px;
        }
        .style2
        {
            color: #003366;
            font-size: xx-large;
        }
    </style>
</head>
<body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
        <table aria-hidden="true" id="TableMain" height="100%" cellSpacing="5" cellPadding="5" border="0">
            <tr>
                <td class="Testo_08_Nero" bgColor="Blue" colSpan="3" height="20">&nbsp;</td>
            </tr>
            <tr>
                <td class="Testo_08_Nero" bgColor="#0066FF" colSpan="3" height="20"></td>
            </tr>
            <tr>
                <td class="Testo_08_Nero" bgColor="#3399FF" colSpan="3" height="20"></td>
            </tr>
            <tr>
                <td height="14"></td>
                <td height="14"></td>
                <td height="14"></td>
            </tr>
            <tr>
                <td class="Testo_12_Blue_Bold" align="center" width="575">&nbsp;
                </td>
                <td class="Testo_07_Blue_Bold" align="center" width="446">
                    <p><em></em></p>
                </td>
                <td align="center" width="200">&nbsp;</td>
            </tr>
            <tr>
                <td height="16">&nbsp;&nbsp;</td>
                <td align="center" height="16"></td>
                <td height="16">&nbsp;&nbsp;<img alt="" class="style1" 
                        src="AB_Immagini/Logo/logo_150.png" /></td>
            </tr>
            <tr>
                <td class="Testo_07_Blue_Bold" align="center" colSpan="3">
                    <p class="style2"><em>AGRONICA</em></p>
                    <p class="style2"><em>STAMPE 2010</em></p>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="Testo_08_Blue_Bold" align="center"></td>
                <td></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="Testo_08_Blue_Bold" align="center"></td>
                <td></td>
            </tr>
            <tr>
                <td class="Testo_08_Nero" align="center" bgColor="#33CC33" colSpan="3" 
                    height="20"></td>
            </tr>
            <tr>
                <td class="Testo_08_Nero" align="center" bgColor="#009933" colSpan="3" 
                    height="20"></td>
            </tr>
            <tr>
                <td class="Testo_08_Nero" align="center" bgColor="#003300" colSpan="3" height="20">
                    <p style="color: white;">
                        Ultimo Aggiornamento&nbsp;:&nbsp;&nbsp;&nbsp;<asp:Label ID="lblUA" runat="server"></asp:Label>
                    </p>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
