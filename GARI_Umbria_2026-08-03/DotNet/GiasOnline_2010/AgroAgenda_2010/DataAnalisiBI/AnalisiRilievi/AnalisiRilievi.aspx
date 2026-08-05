<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="AnalisiRilievi.aspx.vb" Inherits="AgroAgenda_2010.AnalisiRilievi" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="../Meteo/Meteo_CommonStyles.css?<% =Application("GiasVersioneCorrente")%>" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField ID="hd_usaFiltroRicercaNG" runat="server" />
    <asp:HiddenField ID="hd_CurrentPiva" runat="server" />

    <div id="id_MainContainer" class="container" style="padding: 0px;">

        <div id="tabstrip" class="meteo-transparent">
            <ul>
                <li id="Ricerca_Intestazione" class="k-state-active k-active">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize>
                </li>
                <li id="Dati_Intestazione">
                    <asp:Localize Text="<%$ Resources: DatiAnalisiRilievi %>" runat="server"></asp:Localize>
                </li>
            </ul>

            <!--TAB RICERCA-->
            <div id="tab-ricerca"> 
                
                <div class="responsive-container-centered">
                    <div class="responsive-content">

                        <div style="margin-bottom:25px;">
                            <div id="filterContainer" class="k-block" style="width: -webkit-fill-available;padding: 4px;display: flex;">
                                <div>
                                    <a id="FiltraImpianti" class="k-icon k-i-filter" style="cursor: pointer;font-size: 24px;"></a>
                                </div>
                                <div style="width: -webkit-fill-available;position: relative;">
                                    <span style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);white-space: nowrap;overflow: hidden;text-overflow: ellipsis;">
                                        <asp:Label ID="lblFiltroImpostato" Text="" style="font-weight: bold; color: #0000A0;" runat="server"></asp:Label>
                                    </span>
                                </div>
                                <div>
                                    <a id="FiltraImpiantiRipulisci" class="k-icon k-i-filter-clear" style="cursor: pointer;font-size: 24px;"></a>
                                </div>
                            </div>
                        </div>

                        <div class="form-layout">
                            
                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: PeriodoAnalisi %>" runat="server"></asp:Localize>
                                </span>
                            </div>
                            <div style="display:flex;">
                                <div style="padding-right:5px; width:100%">
                                    <input type="text" id="txt_DataDa" class="kendoCalendar" maxlength="10" style="width:100%" autocomplete="off" />
                                </div>
                                <div style="padding-left:5px; width:100%;">
                                    <input type="text" id="txt_DataA" class="kendoCalendar" maxlength="10" style="width:100%" autocomplete="off" />
                                </div>
                            </div>

                            <div class="form-label">
                                <span>
                                    <asp:Localize Text="<%$ Resources: ParametroAnalisi %>" runat="server"></asp:Localize>
                                </span>
                            </div>
                            <div>
                                <input type="text" name="cmbParametroAnalisi" id="cmbParametroAnalisi" value="" style="width: -webkit-fill-available;" />
                            </div>
                        </div>

                        <%If Analisi_Rilievi_Autorizzato Then %>

                        <div style="margin-top: 25px;">
                            <div class="btn btn-success" id="btn_analizza" style="width: -webkit-fill-available;">
                                <span class="fa fa-cogs"></span>
                                <span>
                                    <asp:Localize Text="<%$ Resources:AnalizzaRilievi %>" runat="server"></asp:Localize>
                                </span>
                            </div>
                        </div>
                        
                        <%End If %>
                   
                    </div>
                </div>

            </div>

            <!--TAB DATI-->
            <div id="tab-dati">
                <div id="divKendoOut">
                </div>
            </div>

        </div>

    </div>
    
    <asp:Button runat="server" ID="Btn_FiltraImpianti" Style="display: none;" />
    <asp:Button runat="server" ID="Btn_BackToGIS" Style="display: none;" />

    <asp:HiddenField runat="server" ID="id_TestataTemp" />
    <asp:HiddenField runat="server" ID="id_FiltroGriglia" />

    <input type="hidden" id="hdPiva" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">

        var url_meteo_ws = "../Meteo/MeteoWS.aspx";

        var filtroneImpostato = <%= FiltroneImpostato.ToString.ToLower %>;
        var usaFiltroRicercaNG = $('#<%=hd_usaFiltroRicercaNG.ClientID %>').val() == 'True' ? true : false;
        var currentPiva = $('#<%=hd_CurrentPiva.ClientID %>').val();

        function resetFiltro() {
            filtroneImpostato = false;
            $("#<%= lblFiltroImpostato.ClientID%>").text("<%= DescrizioneFiltroSemplice %>");
        }

        var id_Btn_FiltraImpianti = "<%= Btn_FiltraImpianti.ClientID%>";
        var idTestataTemp = "<%= id_TestataTemp.ClientID%>";
        var idFiltroGriglia = "<%= id_FiltroGriglia.ClientID%>";

        var cIdPiva = "#<%=hdPiva.ClientID() %>";

    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo/Meteo_resx.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("AnalisiRilievi_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("AnalisiRilievi_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("AnalisiRilievi.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo/Meteo_CommonUI.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo/Meteo_Grid.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo/Meteo_Chart.js") %>"></script>

</asp:Content>
