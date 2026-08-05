<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="DatiReteAcquaStoricoXSpecie.aspx.vb" Inherits="AgroAgenda_2010.DatiReteAcquaStoricoXSpecie" %>

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
        }
        .hidden {
            display: none !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="id_MainContainer" class="container responsive-tab-strip" style="padding: 0px;">
        <%--<div class="col-lg-4 col-md-4 col-sm-12">--%>
        <div id="tab_main">
            <div class="form-horizontal">
                <div class="col-lg-8 col-md-8 col-sm-12 col-xs-12">
                    <div class="form-group">
                        <div class="row">
                            <div >
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Specie" for="ddlSpecie">
                                                <asp:Localize Text="<%$ Resources: DatiReteAcqua, SpecieVegetale %>" runat="server">Specie Vegetale</asp:Localize>
                                            </span>
                                            <input type="text" name="ddlSpecie" id="ddlSpecie" value="" style="width: -webkit-fill-available;" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div >
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Anno" for="ddlYear">
                                                <asp:Localize Text="<%$ Resources: DatiReteAcqua, SelAnno %>" runat="server">Anno</asp:Localize>
                                            </span>
                                            <input type="text" name="ddlYear" id="ddlYear" value="" style="width: -webkit-fill-available;" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div >
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">            
                                            <span class="input-group-addon alert-info" id="lbl_GruppoConsegna" for="dllGruppoConsegna">
                                                <asp:Localize Text="<%$ Resources: DatiReteAcqua, GruppiConsegna %>" runat="server">Gruppo Consegna</asp:Localize>
                                            </span>
                                            <input type="text" name="dllGruppoConsegna" id="dllGruppoConsegna" value="" style="width: -webkit-fill-available;" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">

                                            <div class="btn btn-success submit" id="btnAddData" onclick="AggiungiSpecie()" style="background-color: orange;">
                                                <i class="fa fa-plus"></i><asp:Localize Text="<%$ Resources: DatiReteAcqua, AddSpecie %>" runat="server">Aggiungi Specie</asp:Localize>
                                            </div>
                                            <div class="btn btn-success submit" id="btnSaveData" onclick="SalvaTutto()">
                                                <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: DatiReteAcqua, btSalva %>" runat="server">Salva</asp:Localize>
                                            </div>
                                            <div class="btn btn-default " id="btnCancel" onclick="AnnullaTutto()" style="background-color: yellow;">
                                                <i class="fa fa-close"></i><asp:Localize Text="<%$ Resources: DatiReteAcqua, btAnnulla %>" runat="server">Annulla</asp:Localize>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-group searchArea" id="searchArea" style="padding-top:10px;">
                         <!-- griglia risultati -->
                        <asp:Panel runat="Server" ID="mainTab" style="overflow: auto; width: 100%">  <%--margin-top: 10px; margin-bottom: 70px--%>
                            <div id="divKendoGridData" ></div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="permessiNonDisponibili" Visible ="false"><p><asp:Localize Text="<%$ Resources: DatiReteAcqua, NoPermessi %>" runat="server">Permessi non disponibili</asp:Localize></p></asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div id="confermaUscitaDialog"></div>

    <input type="hidden" id="hdKendoModel" runat="server" />
    <input type="hidden" id="hdKendoColumns" runat="server" />
    <input type="hidden" id="hdKendoData" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcquaStoricoXSpecie.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcquaStoricoXSpecie_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DatiReteAcquaStoricoXSpecie_ws_client.js") %>"></script>

    <script type="text/javascript">
        var hdKendoColumsClientID = "<%= hdKendoColumns.ClientID %>";
        var hdKendoRowsClientID = "<%= hdKendoData.ClientID %>";
        var hdKendoModelClientID = "<%= hdKendoModel.ClientID %>";
    </script>
</asp:Content>