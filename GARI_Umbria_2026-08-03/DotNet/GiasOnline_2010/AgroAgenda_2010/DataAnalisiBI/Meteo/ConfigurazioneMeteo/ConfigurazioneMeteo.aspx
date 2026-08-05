
<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" 
    CodeBehind="ConfigurazioneMeteo.aspx.vb" Inherits="AgroAgenda_2010.ConfigurazioneMeteo" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <link href="ConfigurazioneMeteo.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet" type="text/css" />

    <script src="<%= srv_gm %>" type="text/javascript"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="mainContainer" class="container" style="padding: 0px;">

        <div id="tabstrip" style="opacity: 0;">

            <ul>
                <li id="tabApplicazione" class="k-state-active k-active">...
                </li>
                <% if Anagrafiche_Stazioni_Autorizzato Then %>
                <li id="tabAnagrafica">...
                </li>
                <% end if %>
                <% if Alias_Pubbliche_Autorizzato Then %>
                <li id="tabAlias">...
                </li>
                <% end if %>
            </ul>

            <!-- TAB ASSOCIAZIONE STAZIONI/UTILIZZI -->
            <div>

                <div id="applicArea" style="display: grid; grid-template-columns: 1fr 1fr; grid-column-gap: 25px;">

                    <div style="display: grid; grid-template-rows: auto 1fr; grid-row-gap: 10px;">
                        <div>
                            <div style="padding-bottom: 3px; display: flex;">
                                <div style="flex-grow: 1;">
                                    <input type="text" id="ddlTipoSorgente" style="width: 100%;" />
                                </div>
                                <div id="btnStazioniGIS" class="k-button fa-btn"><span class="fa fa-globe"></span></div>
                            </div>
                            <div id="applicStazioniFilter" style="padding-top: 3px;"></div>
                            <div id="applicStazioniLocator" style="padding-top: 3px;"></div>
                        </div>
                        
                        <div id="elencoStaz" style="height: 100%; overflow-y: auto;"></div>
                    </div>

                    <div id="applicAreaTarget">

                        <div id="applicMenu"></div>
                        <div id="applicDSSDifesa"></div>
                        <div id="applicDSSIrriga"></div>
                        <div id="applicMonitorHP"></div>
                        <div id="applicMonitorSuolo"></div>

                    </div>

                </div>

            </div>

            <% if Anagrafiche_Stazioni_Autorizzato Then %>

            <!-- TAB GESTIONE ANAGRAFICA STAZIONI-->
            <div>

                <div id="anagArea" style="display: grid; grid-template-columns: 1fr 1fr; grid-column-gap: 25px;">
                    <div style="display: grid; grid-template-rows: auto 1fr; grid-row-gap: 10px;">
                        <div id="anagStazioniFilter"></div>
                        <div id="stazSrc" style="height: 100%; overflow-y: auto;"></div>
                    </div>
                    <div id="anagEditArea" style="display: grid; grid-template-rows: auto 1fr; grid-row-gap: 10px; position: relative;">
                        <div>
                            <asp:HiddenField runat="server" ID="Id_StazioneEdit" />
                            <div>
                                <input type="text" class="k-textbox" id="input-nome-stazione" style="width: 100%;" />
                            </div>
                            <div>
                                <div id="input-pos-stazione" style="padding-top: 5px;"></div>
                            </div>
                            
                            <div style="display: grid; grid-template-columns: 1fr 1fr; grid-column-gap: 10px; padding-top: 5px; font-size: 14px; font-weight: bold;">
                                <div id="btnEditConferma" class="k-button hdr-btn" style="color: rgb(0, 128, 0);">
                                    <span class="fa fa-check fa-lg"></span>
                                    <span id="dstEditMode">&nbsp;</span>
                                </div>
                                <div id="btnEditAnnulla" class="k-button hdr-btn" style="color: #d45b1a;">
                                    <span class="fa fa-ban fa-lg"></span>
                                    <span id="lblEditAnnulla">&nbsp;</span>
                                </div>
                            </div>
                            <div id="container-notifica-edit" class='notifica-no-icon' style="margin-top: 5px;"></div>
                            <div id="notifica-edit"></div>
                        </div>
                        <div id="sensDst" style="height: 100%; overflow-y: auto;"></div>
                    </div>
                </div>

            </div>

            <% end If %>

            
            <% if Alias_Pubbliche_Autorizzato Then %>

            <!-- TAB GESTIONE STAZIONI PUBBLICHE PREFERITE -->
            <div>
                <div id="aliasArea"></div>
            </div>
            
            <% end If %>

        </div>

    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">

        var IdStazioneEdit = "<%= Id_StazioneEdit.ClientID%>";

        var url_meteows = "../MeteoWS.aspx";
        var url_page = location.pathname; 

    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo_resx.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ConfigurazioneMeteo_Applicazioni.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ConfigurazioneMeteo_Anagrafica.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ConfigurazioneMeteo_Alias.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ConfigurazioneMeteo_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ConfigurazioneMeteo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Plugins/GeoPosEdit.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Widget/StyleLoader.js") %>"></script>

</asp:Content>
