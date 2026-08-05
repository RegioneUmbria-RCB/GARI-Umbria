<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DSSWidget.aspx.vb" Inherits="AgroAgenda_2010.DSSWidget" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>

    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />

    <title></title>
    <link rel="stylesheet" href="../../Styles/font-awesome-4.7.0/css/font-awesome.min.css" media="screen" />
    <link rel="stylesheet" href="../../Styles/bootstrap.min.css" media="screen" />
    <%--<link rel="stylesheet" href="<%=PATH_GIASBASE %>agronica/Styles/bootstrap_AGRONICA.css?<% =Application("GiasVersioneCorrente")%>" media="screen" />--%>
    <link rel="stylesheet" href="Widget/DSS_Gauges.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />

    <link href="../../styles/site.css" rel="stylesheet" type="text/css" />    
    <asp:PlaceHolder ID="kendoPlaceHeader" runat="server"></asp:PlaceHolder>
    <cc2:jquery id="jquery" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        
        <%--necessario per aggiungere controlli agronica--%>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        
        <div id="widget">
        </div>
        

        <cc2:bootstrap id="bootstrap" runat="server" />
        <cc2:Agrokendo id="agroKendo" runat="server" />

        <cc2:agronicabase id="AgronicaBase" runat="server" />

        
        <script type="text/javascript">

            var url_meteo_ws = "./MeteoWS.aspx";
            var cIdPiva = "<%= cIdPiva %>";
            var cModelloPrevisionaleAutorizzato = <%=cModelloPrevisionaleAutorizzato %>;
            var cEnableRedirectToDSSDifesa = <%=cEnableRedirectToDSSDifesa %>;
            var cMessaggioAggiuntivo = "<%= MessaggioAggiuntivo%>";

            var localizationPageUrl = "<%= ResolveClientUrl("~/Localization.aspx") %>";

            var gestioneWaitFrame = false;
        </script>

        <script src="Meteo_resx.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

        <script src="Widget/WidgetCommon.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
        <script src="Widget/StyleLoader.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
        <script src="Widget/DSS_Summary.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
        <script src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Widget/DSS_Gauges.js") %>" type="text/javascript"></script>


        <script type="text/javascript"> 

            function ImpostaAltezzaIframePadreSeEsiste() { 
                setTimeout(function () {
                    // first parameter is the message to be passed
                    // second paramter is the domain of the parent 
                    // in this case "*" has been used for demo sake (denoting no preference)
                    // in production always pass the target domain for which the message is intended 
                    try {
                        var id_widget = "DSSAgronica1";
                        var altezza = $("#widget").height();
                        var messaggio = id_widget+'-'+altezza;

                        window.top.postMessage( messaggio  , "*");
                    } catch (e) {

                    }

                }, 500);
            }

            function gauge_click(params) {
                var parametri = kendo.stringify({
                    params: JSON.stringify(params)
                });

                ajaxAgronica("./DSSWidget.aspx/SalvaParametriAndGetDSSDifesaUrl",
                    parametri,
                    function (risposta) {
                        window.location = risposta.RispostaStringa;
                    }, null,
                    null, gestioneWaitFrame);
            }

           

            $(document).ready(function () {

                if (!Array.isArray(datiMeteoResx)) {
                    datiMeteoResx = [];
                }
                datiMeteoResx.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "DSSWidget.aspx"));

                $( window ).resize(function() {
                    ImpostaAltezzaIframePadreSeEsiste();
                });

                if (cMessaggioAggiuntivo !== "") {
                    $("#widget").html(cMessaggioAggiuntivo);
                } else {

                    if (cModelloPrevisionaleAutorizzato) {

                        $("#widget").DSS_Summary({
                            url_meteoWS: "./MeteoWS.aspx",
                            piva: cIdPiva,
                            summary: false,
                            view_grid: false,
                            msg_summary: TraduzioneMultiResx(datiMeteoResx, "RiepilogoIndicatoriEmergenzeDSS", "Riepilogo indicatori emergenze DSS"),
                            window_title: TraduzioneMultiResx(datiMeteoResx, "IndicatoriEmergenzeDSS", "Indicatori emergenze DSS"),
                            msg_wait: TraduzioneMultiResx(datiMeteoResx, "IndicatoriEmergenzeDSSInAttesaDiElaborazione", "Indicatori emergenze DSS - In attesa di elaborazione..."),
                            msg_na: TraduzioneMultiResx(datiMeteoResx, "IndicatoriEmergenzeDSSNonDisponibili", "Indicatori emergenze DSS non disponibili"),
                            msg_err: TraduzioneMultiResx(datiMeteoResx, "IndicatoriEmergenzeDSSErroreInElaborazione", "Indicatori emergenze DSS - Errore in elaborazione!"),
                            fun_callback: function (status) {
                                if (status === 1) {
                                    ImpostaAltezzaIframePadreSeEsiste();
                                }                                
                            },
                            gauge_click_callback: cEnableRedirectToDSSDifesa ? gauge_click : null
                        });

                    } else {

                        $("#widget").html("Non si dispone dei permessi necessari."); //i18n: Traduzione non necessaria?
                    }
                }
            });

        </script>
    </form>
</body>
</html>
