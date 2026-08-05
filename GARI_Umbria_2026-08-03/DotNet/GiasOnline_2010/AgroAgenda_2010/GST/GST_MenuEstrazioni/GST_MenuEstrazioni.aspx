<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="GST_MenuEstrazioni.aspx.vb" Inherits="AgroAgenda_2010.GST_MenuEstrazioni" %>


<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="GST_MenuEstrazioni.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="GST_MenuEstrazioni_Body" style="opacity:0;">

        <%--parte alta--%>
        
        <div id="sportello_selection" class="row" style="padding-bottom:10px;">
            <div class="col-lg-5">
                <div style="display: grid; gap: 10px;">
                        <div class="input-group">
                            <label class="input-group-addon control-label " id="lblSelezioneSportello" for="ddlSelezioneSportello">Sportello:</label>
                            <input id="ddlSelezioneSportello" name="ddlSelezioneSportello" style="width:-webkit-fill-available;" />
                        </div>
                        
                </div>
            </div>
            <div class="col-lg-5">
                <div style="display: grid; gap: 10px;">
                        <div class="input-group">
                            <label class="input-group-addon control-label " id="lblSelezioneRegione" for="ddlSelezioneRegione">Regione:</label>
                            <input id="ddlSelezioneRegione" name="ddlSelezioneRegione" style="width:-webkit-fill-available;" />
                        </div>
                </div>
            </div>
            <div class="col-lg-2">
                <div style="display: grid; gap: 10px;">
                    <div style="display:flex;">
                        <div id="btnMostraEstrazioni" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Mostra Estrazioni</div>
                    </div>
                </div>
            </div>
        </div>
        <div id="top-side" class="row" style="padding-bottom:10px;display:none">
            <div class="col-lg-3">
                <div style="display: grid; gap: 10px;">
                    <div style="display:flex;">
                        <div id="btnPreventivoColtivazione" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Preventivo di Coltivazione</div>
                    </div>
                </div>
            </div>
            <div class="col-lg-3">
                <div style="display: grid; gap: 10px;">
                    <div style="display:flex;">
                        <div id="btnInterferenze" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Interferenze</div>
                    </div>
                </div>
            </div>
            <div class="col-lg-3">
                <div style="display: grid; gap: 10px;">
                    <div style="display:flex;">
                        <div id="btnVariazioni" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Variazioni di Coltivazione</div>
                    </div>
                </div>
            </div>
            <div class="col-lg-3">
                <div style="display: grid; gap: 10px;">
                    <div style="display:flex;">
                        <div id="btnConsuntivo" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Consuntivo di Coltivazione</div>
                    </div>
                </div>
            </div>
        </div>
        <!--
        <div id="top-side" class="row" style="padding-bottom:10px;">
            <div style="display: grid; gap: 10px;">
                <div style="display:flex;">
                    <div id="btnPreventivoColtivazione" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Preventivo di Coltivazione</div>
                    <div id="btnInterferenze" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Interferenze</div>
                    <div id="btnVariazioniColtivazione" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Variazioni di Coltivazione</div>
                    <div id="btnConsuntivoColtivazione" class="k-button" style="width:-webkit-fill-available; margin-right:5px;"><span class="fa fa-table fa-2x"></span>Consuntivo di Coltivazione</div>
                </div>
            </div>
        </div>
            -->

            <%--Griglie Estrazione--%>

        <div id="grid-container" class="row" style="overflow-y:hidden;">

            <div class ="col-lg-12">
                <div id="estrazioneKendoGrid" style="display:none;">

                </div>
            </div>

        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <%--inclusioni--%>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_MenuEstrazioniJQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_MenuEstrazioni.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_MenuEstrazioni_kendoEvents.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_MenuEstrazioni_ws_client.js") %>"></script>

</asp:Content>
