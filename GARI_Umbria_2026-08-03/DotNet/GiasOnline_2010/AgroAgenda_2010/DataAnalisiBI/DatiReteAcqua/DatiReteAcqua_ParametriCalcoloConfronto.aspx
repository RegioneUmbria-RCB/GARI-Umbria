<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="DatiReteAcqua_ParametriCalcoloConfronto.aspx.vb" Inherits="AgroAgenda_2010.DatiReteAcqua_ParametriCalcoloConfronto" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .responsive-container-centered {
            display: flex;
            justify-content: center;
        }
        /* Extra Small*/
        @media (max-width: 767px) {
            .responsive-content {
                min-width: 100%;
            }
            .responsive-tab-strip{
                width : 100%;
                margin-left : auto;
                margin-right : auto;
            }
        }

        /* Small*/
        @media (min-width: 768px) and (max-width: 991px) {
            .responsive-content {
                min-width: 100%;
            }
            .responsive-tab-strip{
                width : 85%;
                margin-left : auto;
                margin-right : auto;
            }
        }

        /* Medium*/
        @media (min-width: 992px) and (max-width: 1199px) {
            .responsive-content {
                min-width: 75%;
            }
            .responsive-tab-strip{
                width : 70%;
                margin-left : auto;
                margin-right : auto;
            }
        }

        /* Large*/
        @media (min-width: 1200px) {
            .responsive-content {
                min-width: 50%;
            }
            .responsive-tab-strip{
                width : 85%;
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
                display: grid;
                grid-template-columns: minmax(0, auto) 1fr;
                grid-gap: 20px 10px;
                align-items: center;
            }

            .form-label {
                overflow: hidden;
                text-overflow: ellipsis;
                white-space: nowrap;
                text-align: end;
            }

            .form-separator {
                grid-column: 1 / span 2;
                border-bottom: 1px solid #ddd;
            }

            .transparent {
                opacity: 0 !important;
            }

            .hidden {
                display: none !important;
            }

        }
     </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="id_MainContainer" class="container responsive-tab-strip" style="padding: 0px;">

        <div class="form-layout">
<%--            <div class="input-group">
                <span class="input-group-addon alert-info" id="lbl_Year" for="ddlYear">
                    <asp:Localize Text="Valido nell'anno" runat="server">Valido nell'anno</asp:Localize>
                </span>
                <input type="text" name="ddlYear" id="ddlYear" value="" style="width: -webkit-fill-available;" />
            </div>--%>
            <div >
                <div>
                    <div class="btn btn-success submit" id="btnSaveAll" onclick="SalvaTutto()">
                        <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: DatiReteAcqua, btSalva %>" runat="server">Salva</asp:Localize>
                    </div>
                    <div class="btn btn-default " id="btnCancel" onclick="AnnullaTutto()" style="background-color: yellow;">
                        <i class="fa fa-close"></i><asp:Localize Text="<%$ Resources: DatiReteAcqua ,btAnnulla %>" runat="server">Annulla</asp:Localize>
                    </div>
                </div>
            </div>
        </div>
        <div id="tabstrip" style="margin-top: 5px">
            <ul>
                <li id="ParametriGenerali" class="k-state-active k-active">
                    <asp:Localize Text="<%$ Resources: DatiReteAcqua, ParametriGenerali %>" runat="server">Parametri Generali</asp:Localize>
                </li>
                <li id="Coefficienti_Coltura">
                    <asp:Localize Text="<% Resources: DatiReteAcqua, Coefficienti_Coltura %>" runat="server">Coefficienti per Coltura</asp:Localize>
                </li>
            </ul>

            <div id="tab_parametrigenerali">
                <div class="form-layout">
                    <div>
<%--                        <div class="btn btn-success submit" id="btnSaveGeneralParameters" onclick="SalvaParametriGenerali()">
                            <i class="fa fa-floppy-o"></i>Salva Parametri
                        </div>--%>
                        <div class="btn btn-default " id="btnInitGeneralParameters" onclick="InizializzaParametriGenerali()" style="background-color: greenyellow;">
                            <i class="fa fa-certificate"></i><asp:Localize Text="<%$ Resources: DatiReteAcqua, btInizializza %>" runat="server">Inizializza</asp:Localize>
                        </div>
                    </div>
                </div>
                <div class="responsive-container-centered">
                    <div class="responsive-content">
                        <div id="divKendoGridDataPars" style="width: 50%;" >
                            </div>
                    </div>
                </div>
            </div>
            <div id="tab_coeff_specie">
                <div>
                    <div class="form-group">
                        <%--<div class="k-card" style="margin: 8px" id ="generale">--%>
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Specie" for="Cmb_Specie">
                                <asp:Localize Text="<%$ Resources: DatiReteAcqua, SpecieVegetale %>" runat="server">Specie Vegetale</asp:Localize>
                            </span>
                            <input type="text" name="ddlSpecie" id="ddlSpecie" value="" style="width: 50%; " />
                        </div>
                        <div class="input-group">
                            <div class="btn btn-success submit " id="btnAddData" onclick="AggiungiCoefficientiXSpecie()" style="background-color: orange;">
                                <i class="fa fa-plus"></i>Aggiungi Coefficienti per Specie
                            </div>
<%--                            <div class="btn btn-success submit " id="btnSaveData" onclick="SalvaCoefficientiXSpecie()" >
                                <i class="fa fa-floppy-o"></i>Salva
                            </div>--%>
                        </div>
                    </div>
                    <div class="panel-group searchArea" id="searchArea" >
                            <!-- griglia risultati -->
                        <asp:Panel runat="Server" ID="mainTab" style="overflow: auto; width: 50%">  <%--margin-top: 10px; margin-bottom: 70px--%>
                            <div id="divCoeffSpecie" ></div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="permessiNonDisponibili" Visible ="false"><p>Permessi non disponibili</p></asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <input type="hidden" id="hdKendoParsColumns" runat="server" />
    <input type="hidden" id="hdKendoParsRows" runat="server" />
    <input type="hidden" id="hdKendoParsModel" runat="server" />

    <input type="hidden" id="hdKendoCoeffXSpecieColumns" runat="server" />
    <input type="hidden" id="hdKendoCoeffXSpecieRows" runat="server" />
    <input type="hidden" id="hdKendoCoeffXSpecieModel" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcqua_ParametriCalcoloConfronto.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcqua_ParametriCalcoloConfronto_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcqua_ParametriCalcoloConfronto_ws_client.js") %>"></script>
    <script type="text/javascript">
        var hdKendoParsColumnsClientID = "<%= hdKendoParsColumns.ClientID %>";
        var hdKendoParsRowsClientID = "<%= hdKendoParsRows.ClientID %>";
        var hdKendoParsModelClientID = "<%= hdKendoParsModel.ClientID %>";
        var hdKendoCoeffXSpecieColumnsClientID = "<%= hdKendoCoeffXSpecieColumns.ClientID %>";
        var hdKendoCoeffXSpecieRowsClientID = "<%= hdKendoCoeffXSpecieRows.ClientID %>";
        var hdKendoCoeffXSpecieModelClientID = "<%= hdKendoCoeffXSpecieModel.ClientID %>";
    </script>

</asp:Content>