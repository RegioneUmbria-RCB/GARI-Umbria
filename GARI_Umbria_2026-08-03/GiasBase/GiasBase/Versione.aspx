<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Versione.aspx.vb" Inherits="GiasBase.GiasBaseVersione.Versione" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <title></title>
</head>


<body>
    <form id="FORM1" runat="server">
        <!--Bottone per invocare il postback dal client-->
        <asp:Button ID="BTN_PostBack" runat="server" Style="display: none;" />
        <asp:HiddenField ID="ID_Elimina" runat="server" Value="none" />
        <!--per invocare direttamente degli script-->
        <asp:Literal ID="controllo_script" runat="server"></asp:Literal>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div class="page">
            <!--TOP-->
            <div style="text-align: center; height: 80px;">
                <table style="width: 100%;" aria-hidden="true">
                    <tr>
                        <td style="width: 100px">
                            <asp:Image ID="Logo" Height="73px" Width="96px" ImageUrl="~/agronica/AB_Immagini/logo/Logo_GiasOnline_Mini.jpg"
                                runat="server" />
                        </td>
                        <td>
                            <div style="width: 100%; height: 32px;" class="ui-widget-header">
                                <div style="float: left; margin-left: 10px; font-size: 15px; margin-top: 5px;">
                                    Gias BASE
                                </div>
                                <div style="float: right; height: 32px; margin-top: 2px; width: 32px; margin-right: 17px">
                                </div>
                            </div>
                            <div style="text-align: left; width: 100%; margin-top: 15px; height: 15px;" class="ui-widget-header">
                                <div style="float: left">
                                    Aggiornamenti ed implementazioni
                                <div style="float: right">
                                </div>
                                </div>
                            </div>
                        </td>
                        <td style="width: 50px"></td>
                    </tr>
                </table>
                <asp:Panel ID="Pannello_Versione" runat="server">
                    <table id="TabellaVersione" runat="server" aria-hidden="true">
                    </table>
                </asp:Panel>
            </div>
            <div class="clear">
            </div>
            <div class="clear">
            </div>
        </div>
    </form>
</body>

</html>
