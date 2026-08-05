<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Custom500.aspx.vb" Inherits="AgronicaUMA.Custom500" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">

        <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
        <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
                <title></title>
                <link rel="stylesheet" href="./Styles/bootstrap.min.css" media="screen" />

                <cc2:jquery id="jquery" runat="server" />

            </head>
    <body>

        <!-- Questa pagina non può rientrare nelle logiche della pagina master. -->
        <div id="DIV_Messaggi"></div>

        <form id="Form1" runat="server" class="form-horizontal">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

            <asp:Panel ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
                <h1>
                    <asp:Label ID="errore" runat="server"></asp:Label>
                </h1>
                <asp:PlaceHolder ID="script" runat="server"></asp:PlaceHolder>
                <br />
                <br />
                <asp:Panel ID="pannelloerrore" runat="server" Visible="false">
                    <asp:PlaceHolder ID="phDettaglioErrore" runat="server"></asp:PlaceHolder>
                </asp:Panel>
            </asp:Panel>

            <cc2:bootstrap id="bootstrap" runat="server" />
            <cc2:jquery_cookie ID="jCookie" runat="server" />
            <cc2:agronicabase id="AgronicaBase" runat="server" />

            <asp:Panel ContentPlaceHolderID="ContentScript" runat="server" ID="Content3">

                <script type="text/javascript">
                    function posizionaMessggi(jQuerySelector) {
                        try {

                            $(jQuerySelector).css("z-index", "100000");
                            $(jQuerySelector).css("position", "absolute");
                            $(jQuerySelector).css("top", window.pageYOffset.toString() + "px");
                            $(jQuerySelector).css("width", $(window).width().toString() + "px");

                        } catch (e) {
                            console.log("posizionaMessggi non riuscita...");
                        }

                    }

                    function MessaggioErrore_Bootstrap(str, id_div) {
                        var stringa_html = '<div class="alert alert-danger" role="alert">' + str +
                            '<button type="button" class="close chiudi_alert" data-dismiss="alert"><span aria-hidden="true">&times;</span> <span class="sr-only">Close</span></button></div>';
                        $('#' + id_div).html(stringa_html);
                        posizionaMessggi('#' + id_div);
                    }

                    function goURL() {
                        MessaggioErrore_Bootstrap("Sessione scaduta. <a href='javascript:SessioneScadutaGestione()'>Fare click QUI se non si viene indirizzati automaticamente</a> in <span id='AgronicaBaseTimeOutSessione'></span>", "DIV_Messaggi");
                        AgronicaBaseImpostaRedirectStart("DIV_Messaggi");
                    }

                </script>

                <asp:PlaceHolder ID="phChiamataGoURL" runat="server"></asp:PlaceHolder>

            </asp:Panel>
        </form>

    </body>
</html>
