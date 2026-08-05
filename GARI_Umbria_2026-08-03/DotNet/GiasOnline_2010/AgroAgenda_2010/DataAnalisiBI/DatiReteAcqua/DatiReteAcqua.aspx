
<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="DatiReteAcqua.aspx.vb" Inherits="AgroAgenda_2010.DatiReteAcqua" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="<%= srv_gm %>" type="text/javascript"></script>

    <style type="text/css">
        .responsive-container-centered {
            display: flex;
            justify-content: center;
            padding-top: 15px;
        }

        /* Extra Small*/
        @media (max-width: 767px) {
            .responsive-content {
                min-width: 100%;
            }
        }

        /* Small*/
        @media (min-width: 768px) and (max-width: 991px) {
            .responsive-content {
                min-width: 75%;
            }
        }

        /* Medium*/
        @media (min-width: 992px) and (max-width: 1199px) {
            .responsive-content {
                min-width: 50%;
            }
        }

        /* Large*/
        @media (min-width: 1200px) {
            .responsive-content {
                width: 50%;
                margin-left : auto;
                margin-right : auto;
            }
        }

        .form-label {
            color: #9e9e9e;
            font-weight: bold;
        }

        @media (max-width: 767px) {

            .form-label:not(:first-child) {
                margin-top: 20px;
            }

            .form-label {
                margin-bottom: 5px;
            }

            .form-separator {
                margin: 20px 0px;
            }
        }

        @media (min-width: 768px) {

            .form-layout {
                display: table-cell;
                /*vertical-align: middle;*/
                text-align: center;
                width: 50%;
/*                display: grid;
                grid-template-columns: minmax(0, auto) 1fr;
                grid-gap: 20px 10px;
                align-items: center;*/
            }

            .form-label {
                overflow: hidden;
                text-overflow: ellipsis;
                white-space: nowrap;
                text-align: start;
                margin-bottom: 5px;
                margin-top: 10px;
            }

            .form-separator {
                grid-column: 1 / span 2;
                border-bottom: 1px solid #ddd;
            }

            .transparent {
                opacity: 0 !important;
            }

            .form-separator {
                margin-top: 20px;
            }

            .form-group {

            }

        }

        .transparent {
            opacity: 0 !important;
        }

        .hidden {
            display: none !important;
        }

        .water-label{
            display: table-cell;
            text-align: start !important;
            font-family: 'Lato', sans-serif;
            color: #575757;
            font-weight: 400;
            /* text-transform: capitalize; */
            background: none;
            border: none;
            padding-left: 0;
            font-size: 14px;
            min-width: 100px;
            /*text-align: center !important;*/
            width: 1%;
            white-space: nowrap;
            vertical-align: middle;
        }
    </style>
</asp:Content>    

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="id_MainContainer" class="container" style="padding: 0px;">
        <div id="tabstrip">
            <ul>
                <li id="parametri" class="k-state-active k-active">
                    <asp:Localize Text="<%$ Resources: DatiReteAcqua, Parametri %>" runat="server">Parametri ricerca</asp:Localize>
                </li>
                <li id="dati_letture">
                    <asp:Localize Text="<% Resources: DatiReteAcqua, DatiLetture %>" runat="server">Dati letture sensori</asp:Localize>
                </li>
<%--                <li id="Dati_Pressione">
                    <asp:Localize Text="<% Resources: DatiReteAcqua, Pressione %>" runat="server">Dati Pressione</asp:Localize>
                </li>--%>
                <li id="Dati_PrelieviOsservatiEffettivi">
                    <asp:Localize Text="<% Resources: DatiReteAcqua, RisultatoCalcolo %>" runat="server">Prelievi Effettivi - Attesi</asp:Localize>
                </li>
            </ul>

            <div id="tab_search">
                <div class="responsive-container-centered">
                    <div class="responsive-content">
                        <div id="contatori" class="form-layout">
                            <div class="row">
                                <div class="form-group">
                                    <div class="row">
                                        <div class="form-label">
                                            <span>
                                                <asp:Localize Text="<%$ Resources: DatiReteAcqua, GeolocalizzazioneGruppo %>" runat="server">Geolocalizzazione gruppo di consegna</asp:Localize>
                                            </span>
                                        </div>
                                        <div>
                                            <div id="geo-pos-edit"></div>
                                        </div>
                                    </div>
                                    <div class="form-label">
                                        <span>
                                            <asp:Localize Text="<%$ Resources:  DatiReteAcqua, GruppiConsegna %>" runat="server">Gruppi di Consegna</asp:Localize>
                                        </span>
                                    </div>
                                    <div>
                                        <input type="text" name="ddlElencoGruppiConsegna" id="ddlElencoGruppiConsegna" value="" style="width: -webkit-fill-available;" />
                                    </div>                                
                                    <div class="form-separator"></div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group">
                                    <div class="form-label">
                                        <span>
                                            <asp:Localize Text="<%$ Resources:  DatiReteAcqua, PeriodoRicerca %>" runat="server">Periodo ricerca</asp:Localize>
                                        </span>
                                    </div>
                                    <div style="display: grid; grid-template-columns: 1fr 1fr; grid-column-gap: 10px;">
                                        <div>
                                            <input type="text" id="txt_DataDa" class="kendoCalendar" maxlength="10" style="width:100%" autocomplete="off" />
                                        </div>
                                
                                        <div>
                                            <input type="text" id="txt_DataA" class="kendoCalendar" maxlength="10" style="width:100%" autocomplete="off" />
                                        </div>                                    
                                    </div>
                                    <div class="btn btn-success" id="btn_ricerca" style="width: -webkit-fill-available;margin-top: 25px;">
                                        <span class="fa fa-cogs"></span>
                                        <span>
                                            <asp:Localize Text="<%$ Resources:  DatiReteAcqua, VisualizzaLetture %>" runat="server">Visualizza dati letture contatori</asp:Localize>
                                        </span>
                                    </div>
                                    <div class="form-separator"></div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group">
                                    <div class="form-horizontal">
                                        <div class="row" >
                                            <div class="col-lg-3 col-md-3 col-sm-12" >
                                                <div class="input-group" id="groupAnnoElaborazione">
                                                    <label class="input-group-addon " for="ddlYear" id="lblYear"><asp:Localize Text="<%$ Resources:  DatiReteAcqua, AnnoElaborazione %>" runat="server">Anno di Elaborazione</asp:Localize></label>
                                                    <input name="ddlYear" id="ddlYear"  class="form-control" style="width:-webkit-fill-available" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="input-group" id="groupDatiStorici">
                                                <label id="lbl_DatiStorici" for="chkDatiStorici"><asp:Localize Text="<%$ Resources:  DatiReteAcqua, DatiStorici %>" runat="server">Visualizza Dati Storici</asp:Localize></label>
                                                <input type="checkbox" id="chkDatiStorici" name="chkDatiStorici" class="kendoSwitch"/>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="input-group" id="groupLatLngContatore">
                                                <label id="lbl_UseLatLngContatore" for="chkUseLatLngContatore"><asp:Localize Text="<%$ Resources:  DatiReteAcqua, UseLatLngContatore %>" runat="server">Utilizza coordinate gruppo di consegna per trovare la stazione meteo pubblica</asp:Localize></label>
                                                <input type="checkbox" id="chkUseLatLngContatore" name="chkUseLatLngContatore" class="kendoSwitch"/>
                                            </div>
                                        </div>
                                        <div class="row" id="geo-pos-meteo">
                                            <div class="form-label">
                                                <span>
                                                    <asp:Localize Text="<%$ Resources: DatiReteAcqua,Geolocalizzazione %>" runat="server">Geolocalizzazione stazione meteo di riferimento</asp:Localize>
                                                </span>
                                            </div>
                                            <div>
                                                <div id="geo-pos-edit-meteo"></div>
                                            </div>
                                        </div>
                                        <div class="form-label">
                                            <span>
                                                <asp:Localize Text="<%$ Resources: DatiReteAcqua,CategoriaStazioni %>" runat="server">Categoria stazioni</asp:Localize>
                                            </span>
                                        </div>
                                        <div>
                                            <input type="text" name="ddlTipoSorgente" id="ddlTipoSorgente" value="" style="width: -webkit-fill-available;" />
                                        </div>

                                        <div class="form-label">
                                            <span>
                                                <asp:Localize Text="<%$ Resources: DatiReteAcqua,StazioneMeteo %>" runat="server">Stazione meteo</asp:Localize>
                                            </span>
                                        </div>
                                        <div>
                                            <input type="text" name="ddlOrigineDati" id="ddlOrigineDati" value="" style="width: -webkit-fill-available;" />
                                        </div>
                                        <div style="margin-top: 25px;">
                                            <div class="btn btn-success" id="btn_calcola" style="width: -webkit-fill-available;">
                                                <span class="fa fa-cogs"></span>
                                                <span>
                                                    <asp:Localize Text="<%$ Resources: DatiReteAcqua,EseguiCalcolo %>" runat="server">Esegui modello confronto prelievi</asp:Localize>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--TAB DATI-->
            <div id="tab-dati-contatori">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="form-label" style="text-align:left; font-size:medium">
                        <span>
                            <asp:Localize Text="<%$ Resources: DatiReteAcqua,DatiGeneraliContatore %>" runat="server"><b>Dati generali contatore</b></asp:Localize>
                        </span>
                    </div>

                    <div id="deviceImage" style="margin-top: 20px;">

                    </div>
                    <div id="divDatiAnagrafe" style="margin-top: 20px;">

                    </div>
                </div>
                <div class="col-lg-8 col-md-8 col-sm-12">
                    <div id="detailChart">
                    </div>
                    <div id="divKendoContatoriOut"  style="height: 490px;">
                    </div>
                </div>
                
            </div>

            <div id="tab-dati-confronto">
                <div id="chart">
                </div>
                <div id="chart-vol">
                </div>
                <div id="divKendoCalcoloOut">
                </div>
            </div>

        </div>
    </div>
    <input type="hidden" id="hdKendoColums" runat="server" />
    <input type="hidden" id="hdKendoRows" runat="server" />
    <input type="hidden" id="hdKendoModel" runat="server" />

    <input type="hidden" id="hdKendoColumsLast" runat="server" />
    <input type="hidden" id="hdKendoRowsLast" runat="server" />
    <input type="hidden" id="hdKendoModelLast" runat="server" />

    <input type="hidden" id="hdKendoColumsCalcolo" runat="server" />
    <input type="hidden" id="hdKendoRowsCalcolo" runat="server" />
    <input type="hidden" id="hdKendoModelCalcolo" runat="server" />

    <input type="hidden" id="hdKendoChartCalcolo" runat="server" />

    <input type="hidden" id="hdPiva" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcqua.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcqua_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcqua_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcqua_Charts.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../Meteo/Plugins/GeoPosEdit.js") %>"></script>

    <script type="text/javascript">
        var hdKendoColumsClientID = "<%= hdKendoColums.ClientID %>";
        var hdKendoRowsClientID = "<%= hdKendoRows.ClientID %>";
        var hdKendoModelClientID = "<%= hdKendoModel.ClientID %>";
        var hdKendoColumsLastClientID = "<%= hdKendoColumsLast.ClientID %>";
        var hdKendoRowsLastClientID = "<%= hdKendoRowsLast.ClientID %>";
        var hdKendoModelLastClientID = "<%= hdKendoModelLast.ClientID %>";
        var hdKendoColumsCalcoloClientID = "<%= hdKendoColumsCalcolo.ClientID %>";
        var hdKendoRowsCalcoloClientID = "<%= hdKendoRowsCalcolo.ClientID %>";
        var hdKendoModelCalcoloClientID = "<%= hdKendoModelCalcolo.ClientID %>";
        var hdKendoChartCalcoloClientID = "<%= hdKendoChartCalcolo.ClientID %>";
        var hdPivaClientID = "<%= hdPiva.ClientID %>";
        var UtenteAbilitatoLettura = true;
        var UtenteAbilitatoScrittura = false;
    </script>
</asp:Content>

