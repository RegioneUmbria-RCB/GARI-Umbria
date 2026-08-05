<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="GST_Menu.aspx.vb" Inherits="AgroAgenda_2010.GST_Menu" %>


<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="GST_Menu.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="GST_Menu_Body" style="opacity:0;">

<%--                
            <div>
                <div style="display:flex; justify-content:space-between; justify-content:flex-end;">
                    <div id="btnVerificaInterferenze" class="k-button" style="flex-grow: 1;"><span class="fa fa-cogs fa-2x"></span>Verifica interferenze</div>
                    <div id="btnDistanzeMinime" class="k-button" style="flex-grow: 1;"><span class="fa fa-info-circle fa-2x"></span>Distanze minime</div>
                    <div id="btnCodificaSpecie" class="k-button" style="flex-grow: 1;"><span class="fa fa-info-circle fa-2x"></span>Codifica specie vegetali</div>
                </div>
            </div>
--%>

        
        <%--parte alta--%>
        <div id="top-side" class="row" style="padding-bottom:10px;">

                <%--parte sinistra--%>
                <div class="col-lg-5">
                    <!-- Nuovi impianti -->
                    <div style="display: grid; gap: 10px;">
                        <div>
                            <div style="display:flex;">
                                <div id="btnNuovoImpianto" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-pencil-square-o fa-2x"></span>Gestione impianti sportello</div>
                                <div id="btnMappaturaLibera" class="k-button" style="width:-webkit-fill-available; margin-left:5px;"><span class="fa fa-globe fa-2x"></span>Mappatura libera</div>
                            </div>
                        </div>
                        <div>
                            <%--<div style="display:grid; grid-gap:10px; grid-template-columns:auto max-content; align-items:center;">--%>
                                <div class="input-group">
                                    <label class="input-group-addon control-label " id="lblSportello2" for="ddl_sportello">Sportello:</label>
                                    <input id="ddl_sportello" style="width:-webkit-fill-available;"/>
                                </div>
<%--                                <div style="margin-bottom:10px;">
                                    <input type="checkbox" class="k-checkbox" id="cbSportelliPrecedenti">
                                    <label class="k-checkbox-label" for="cbSportelliPrecedenti" style="margin:0px;">Mostra sportelli precedenti</label>
                                </div>--%>
                            <%--</div>--%>

                            <div class="k-block" style="cursor: pointer; padding:3px 5px 3px 0px;" id="id_info_preventivo_consuntivo">
                                <div style="display: grid; grid-template-columns: auto auto 1fr; grid-gap: 5px; grid-template-rows: 1.5em; align-items: center;">
                                    <div>
                                        <span class="fa fa-question-circle fa-lg"></span>
                                    </div>
                                    <div>
                                        <label id="lbl_preventivo_consuntivo" style="margin-bottom: 0px; cursor:inherit;"></label>
                                    </div>
                                    <div id="id_info_sportello" style="overflow:hidden; white-space:nowrap; text-overflow:ellipsis; color:#888;">
                                    </div>
                                </div>                                
                            </div>
                        </div>
                    </div>
                </div>

                <%--parte centrale--%>
                <div class="col-lg-3">

                    <!-- Interferenze -->
                    <div style="display: grid; gap: 10px;">
                        <div id="btnVerificaInterferenze" class="k-button"><span class="fa fa-cogs fa-2x"></span>Verifica interferenze</div>

                        <div id="btnSalvaVariazioni" class="k-button"><span class="fa fa-table fa-2x"></span>Consolida variazioni</div>
                    </div>
<%--                    
Ricerca delle possibili interferenze in un sottoinsieme di impianti selezionati dall'utente.
--%>
                </div>

                <%--parte centrale 2--%>
                <div class="col-lg-2">

                    <!-- Informazioni -->
                    <div style="display: grid; gap: 10px;">
                        <div id="btnDistanzeMinime" class="k-button"><span class="fa fa-info-circle fa-2x"></span>Distanze minime</div>
                        <%--
                            Pagina informativa sulle distanze minime di legge da rispettare nell'ambito del Progetto Sementi.
                            --%>

                        <div id="btnCodificaSpecie" class="k-button"><span class="fa fa-info-circle fa-2x"></span>Codifica specie vegetali</div>
                        <%--  
                            Pagina informativa sulla relazione tra le specie vegetali contemplate dal Progetto Sementi e le specie vegetali GIAS.
                            --%>
                    </div>
                </div>

                <%--parte destra--%>
                <div class="col-lg-2">
                    
                    <div style="display: grid; gap: 10px;">
                        <div id="btnGestioneSportello" runat="server"><span class="fa fa-calendar-o fa-2x"></span>Gestione sportello</div>
                    </div>
                </div>
            </div>

            <%--messaggistica--%>

            <div id="grid-container" style="overflow-y:hidden;">

                <div style="display: grid; grid-template-columns: 1fr 1fr; height:100%; max-height:100%;">

                    <%--parte sinistra--%>
                    <div class="k-block" style="margin-right:5px; padding-top:5px;">
                        <div class="k-header" style="margin-bottom:0px; width:100%; text-align:center;font-size:large;">
                            Comunicazioni e notizie personali
                        </div>
                        <div id="placeInterferenzeScroll" style="overflow-y:auto;">
                            <div id="placeInterferenze">
                            </div>
                        </div>
                    </div>

                    <%--parte destra--%>
                    <div class="k-block" style="margin-left:5px; padding-top:5px;">
                        <div class="k-header" style="margin-bottom:0px; width:100%; text-align:center;font-size:large;">
                            Comunicazioni dirette a tutti
                            <div style="position:absolute; top:0; left:100%; transform:translateX(-100%); padding-right:5px; padding-top:2px;">
                                <div id="btnStampaElenco" class="k-button" style="padding:3px 15px;" runat="server">
                                    <span class="fa fa-print"></span>
                                </div>
                            </div>
                        </div>
                        <div id="placeNotificheScroll" style="overflow-y:auto;">
                            <div id ="placeNotifiche" style="margin-left:10px; margin-right:10px; padding-top: 10px; padding-bottom: 10px;">
                            </div>
                        </div>
                    </div>

                </div>

            </div>

    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <%--variabili globali pagina--%>
    <script type="text/javascript">
        var btnStampaElenco_ClientID = "<%= btnStampaElenco.ClientID %>";
        var btnGestioneSportello_ClientID = "<%= btnGestioneSportello.ClientID %>";
    </script>

    <%--inclusioni--%>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_MenuJQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Menu.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Menu_ws_client.js") %>"></script>

</asp:Content>
