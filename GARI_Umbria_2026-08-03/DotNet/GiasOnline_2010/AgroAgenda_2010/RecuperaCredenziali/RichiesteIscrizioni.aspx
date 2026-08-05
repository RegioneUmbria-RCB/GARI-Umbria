<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RichiesteIscrizioni.aspx.vb" Inherits="AgroAgenda_2010.RichiesteIscrizioni" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title><asp:Localize Text="<%$ Resources: NuovaRichiestaIscrizione %>" runat="server"></asp:Localize></title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />

    <link rel="stylesheet" href="<%= ResolveUrl("~/Styles/font-awesome-4.7.0/css/font-awesome.min.css?" & Application("GiasVersioneCorrente")) %>" media="screen" />
    
    <!--bootstrap css-->
    <asp:PlaceHolder ID="bootstrapPlaceHeader" runat="server"></asp:PlaceHolder>
    
    <link rel="stylesheet" href="<%=PATH_GIASBASE %>agronica/Styles/bootstrap_AGRONICA.css?<% =Application("GiasVersioneCorrente")%>" media="screen" />

    <asp:PlaceHolder ID="siteCssPlaceHolder" runat="server"></asp:PlaceHolder>
    <cc2:jquery ID="jquery" runat="server" />

    <asp:PlaceHolder ID="kendoPlaceHeader" runat="server"></asp:PlaceHolder>

    <link rel="stylesheet" href="<%= ResolveUrl("RecuperaCredenziali.css?" & Application("GiasVersioneCorrente").ToString) %>" media="screen" />
    
    <!-- Versione Grafica Pagina Login -->
    <% If Login_Versione = "2022" Then %>
        <link rel="stylesheet" href="<%=PATH_GIASBASE %>agronica/Styles/styleXonneLogin.css?<% =Application("GiasVersioneCorrente")%>" media="screen" />
    <% End If %>

    <style type="text/css">
        .nascosto {
            display: none;
        }

        #containerRichiesteIscrizioni > div:last-child {
            margin-bottom: 80px;
        }

        #epDatiRichiedente .row, #epDatiAutorizzazione .row {
            margin-bottom: 10px;
            margin-top: 10px;
        }

        #boxInfoPagina, #boxIndici {
            margin-bottom: 20px;
        }

        #titoloPagina {
            display: inline;
        }

        #infoPagina {
            display: inline;
            font-weight: bold;
            margin: 0 15px;
        }

        #boxCaricaDocumento {
            margin-top: 15px;
        }

        #boxTipoFirma {
            margin-top: 10px;
        }

        #btnScaricaTemplIscr {
            font-weight: bold;
        }

        #boxIndici > .row {
            margin: 12px 0 0 0;
        }

        #txtCodiceFiscale {
            text-transform: uppercase;
        }

        /* Pop-up creato da kendo per la multiselect */
        #msCuaa-list {
            display: none !important;
        }

        #fileCaricaDocumento {
            display: none;
        }

        #titleAllegato {
            margin-bottom: 5px;
        }

        #boxProcediRichiesta {
            padding: 1.5em 1.25em;
        }
    </style>
</head>
<body>
    <div id="wrap_master">
        <cc2:AgroMasterPage ID="AgroMasterPage" runat="server" />

        <form id="Form1" runat="server">
            <div id="containerRichiesteIscrizioni" class="container nascosto">
                <div>
                    <asp:PlaceHolder ID="headerPlaceHolder" runat="server"></asp:PlaceHolder>
                </div>
                <div id="boxInfoPagina">
                    <h4 id="titoloPagina"><asp:Localize Text="<%$ Resources: RichiestaIscrizione %>" runat="server"></asp:Localize></h4>
                    <span id="infoPagina">
                        <asp:Localize Text="<%$ Resources: InfoPagina %>" runat="server"></asp:Localize>
                    </span>
                    <button type="button" id="btnScaricaTemplIscr" name="btnScaricaTemplIscr" class="btnAsLink">
                        <i class="fa fa-download"></i><asp:Localize Text="<%$ Resources: DocumentoDaScaricare %>" runat="server"></asp:Localize>
                    </button>
                    <div id="boxCaricaDocumento" class="row">
                        <div class="col-sm-10">
                            <h5 id="titleAllegato"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Allegato %>" runat="server"></asp:Localize> *</h5>
                            <div>
                                <span class="input-group-btn">
                                    <button type="button" class="btn" id="btnCaricaDocumento"><i class="fa fa-folder-open"></i></button>
                                </span>
                                <input type="text" class="form-control" id="txtCaricaDocumento" name="txtCaricaDocumento" readonly />
                            </div>
                        </div>
                        <input type="file" id="fileCaricaDocumento" name="fileCaricaDocumento" />
                    </div>
                    <div id="boxTipoFirma" class="row">
                        <div class="col-sm-5">
                            <div>
                                <label class="input-group-addon myLabelBold" id="lblTipoFirma" for="ddlTipoFirma">
                                    <asp:Localize Text="<%$ Resources: FirmaApplicataDocumento %>" runat="server"></asp:Localize> *
                                </label>
                                <select class="form-control" id="ddlTipoFirma" name="ddlTipoFirma" ></select>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="boxIndici"></div>
                <div id="boxNote" class="row">
                    <div class="col-sm-10">
                        <div>
                            <label class="input-group-addon" id="lblNote" for="txtAreaNote">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server"></asp:Localize>
                            </label>
                            <textarea class="form-control" id="txtAreaNote" name="txtAreaNote"></textarea>
                        </div>
                    </div>
                </div>
                <div id="boxProcediRichiesta">
                    <div class="row">
                        <div class="col-sm-3">
                            <button type="button" class="btn btn-success" id="btnProcediRichiesta" name="btnProcediRichiesta">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Procedi %>" runat="server"></asp:Localize>
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Hidden field -->
            <input type="hidden" runat="server" id="hfValSuperServer" name="hfValSuperServer" />
            <input type="hidden" runat="server" id="hfDescSuperServer" name="hfDescSuperServer" />
            <input type="hidden" runat="server" id="hfPivaSuperServer" name="hfPivaSuperServer" />
            <input type="hidden" runat="server" id="hfObjPSuperServer" name="hfObjPSuperServer" />
            <input type="hidden" runat="server" id="hfObjPServer" name="hfObjPServer" />
            <input type="hidden" runat="server" id="hfObjPUtenti" name="hfObjPUtenti" />
            <input type="hidden" runat="server" id="hfPathCoreWS" name="hfPathCoreWS" />
            <input type="hidden" runat="server" id="hfTemplateIscr" name="hfTemplateIscr" />
            <input type="hidden" runat="server" id="hfTipologiaIscr" name="hfTipologiaIscr" />

            <cc2:bootstrap ID="bootstrap" runat="server" />
            <cc2:AgronicaBase ID="AgronicaBase" runat="server" />
            <cc2:bootstrap_WaitFrame ID="bW" runat="server" />
            <cc2:jAgroHelper ID="jAgroHelper1" runat="server" />
            <cc2:jquery_cookie ID="jCookie" runat="server" />
            
            <script>
                var localizationPageUrl = "<%= ResolveUrl("~/Localization.aspx") %>";
            </script>

            <script src="<%= ResolveUrl("~/Scripts/jquery.linq.min.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>
            <script src="<%= ResolveUrl("~/Scripts/jquery.tmpl.min.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>
            <script src="<%= ResolveUrl("~/Scripts/jquery.validate.min.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>
            <script src="<%= ResolveUrl("~/RecuperaCredenziali/RichiesteIscrizioni_jQueryDocReady.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>
            <script src="<%= ResolveUrl("~/RecuperaCredenziali/RichiesteIscrizioni.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>
            <script src="<%= ResolveUrl("~/ScriptsGestionali/funzioni_comuni.js?" & Application("GiasVersioneCorrente")) %>" type="text/javascript"></script>

            <cc2:Agrokendo id="agroKendo" runat="server" />

        </form>
    </div>
    <asp:PlaceHolder ID="footerPlaceHolder" runat="server"></asp:PlaceHolder>
</body>
</html>
