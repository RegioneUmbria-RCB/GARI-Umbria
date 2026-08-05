<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="PrenotazionePiante_Riepilogo.aspx.vb" Inherits="AgroAgenda_2010.PrenotazionePiante_Riepilogo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="">

        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <div id ="tabstrip">
                <ul>
                <li class="k-state-active k-active">
                    <asp:Localize meta:resourcekey="RiepilogoProgetti" runat="server">Riepilogo Progetti Sintetici</asp:Localize></li>
                <li>
                    <asp:Localize meta:resourcekey="RiepilogoProgettiRichieste" runat="server">Riepilogo Progetti per Richieste</asp:Localize></li>
            </ul>
            <div class="" id="tab_dati_generaliSintetico">
                <div class="jumbotron">

                    <div class="row">

                        <div class="col-lg-12 col-md-12 col-sm-12 text-center">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div id="btn_AggiornaReportSintetico" class="btn btn-info">
                                        <i class="fa fa-search" aria-hidden="true"></i>
                                        <span id="lbl_CercaGiacenzeSintetico">Aggiorna Report</span>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                    <div class="row">

                        <div id="kendoRiepilogoSintetico"></div>

                    </div>

                </div>
            </div>
            <div class="" id="tab_dati_generali">
                <div class="jumbotron">

                    <div class="row" style="display: none">
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_data">Data</span>
                                        <input type="text" id="Txt_Data" name="Data" class="form-control" required validationmessage="{0} è obbligatoria" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">

                        <div class="col-lg-12 col-md-12 col-sm-12 text-center">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div id="btn_AggiornaReport" class="btn btn-info">
                                        <i class="fa fa-search" aria-hidden="true"></i>
                                        <span id="lbl_CercaGiacenze">Aggiorna Report</span>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                    <div class="row">

                        <div id="kendoRiepilogo"></div>

                    </div>

                </div>
            </div>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PrenotazionePiante_Riepilogo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PrenotazionePiante_Riepilogo_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PrenotazionePiante_Riepilogo_ws_client.js") %>"></script>
</asp:Content>
