<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="MenuStampeBootstrap.aspx.vb" Inherits="AgroAgenda_2010.MenuStampeBootstrap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="menuStampeBootstrap.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField ID="hd_usaFiltroRicercaNG" runat="server" />
    <asp:HiddenField ID="hd_CurrentPiva" runat="server" />

    <div class="jumbotron gias-mt-1 gias-m-1 gias-ml-1 gias-border-white gias-stampe-bootstrap">
        <div id="win_gestionePreferiti" style="display: none">
            <div class="window-content">
                <div class="row">
                    <div id="win_ricercaStampe" class="col-lg-12 col-md-12 col-sm-12 gias-2024">
                        <span class="gias-2024 icon fa fa-search"></span>
                        <input type="text" id="win_txtRicercaStampe" name="win_txtRicercaStampe" autocomplete="off" />
                    </div>
                </div>

                <div class="row">
                    <div id="win_divStampe" class="col-lg-7 col-md-7 col-sm-12 gias-2024">
                        Stampe<br />
                        <div id="win_listStampe"></div>
                    </div>

                    <div id="win_divPreferiti" class="col-lg-4 col-md-4 col-sm-12 gias-2024">
                        Preferiti<br />
                        <div id="win_listPreferiti"></div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row gias-mb-1">
            <div class="col-lg-4 col-md-4 col-sm-12 gias-preferiti-switch-container">

                <label for="switchDDL" class="left">
                    Tutti
                </label>
                <input id="switchDDL" name="switchDDL" />
                <label for="switchDDL" class="right">
                    Preferiti
                </label>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-4 col-md-4 col-sm-12">
                <div class="form-group">
                    <div class="input-group">
                        <label id="lblddRicercaRapida" class="input-group-addon" for="ddRicercaRapida">
                            Ricerca Rapida:         
                        </label>
                        <input name="ddRicercaRapida" id="ddRicercaRapida" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="col-lg-4 col-md-4 col-sm-1">
                <div class="btn btn-success buttonClass gias-btn-primary" id="btn_eseguiStampa" style="margin-left: 10px;">
                    <span class="fa fa-play"></span>Avvia Stampa
                </div>
                <div class="btn btn-success buttonClass gias-btn-primary" id="btn_gestionePreferiti" style="margin-left: 10px;">
                    <span class="fa fa-star"></span>Gestisci Preferiti
                </div>
            </div>
        </div>
        <div id="line">
            <hr style="background-color: #002850; height: 1px;" />
        </div>

        <div id="stampeParametersContainer" class="gias-stampa-parameters">
            <div class="gias-params-title">
                Parametri di stampa
            </div>
            <div class="row gias-mt-1">

                <div class="col-lg-4 col-md-6 col-sm-12">
                    <div class="input-group" id="annoGroup" style="display: none">
                        <label class="input-group-addon control-label" id="lbl_UC__anno" for="anno">
                            Anno:
                        </label>
                        <input type="text" id="anno" name="anno" autocomplete="off" />
                    </div>
                </div>

                <div class="col-lg-4 col-md-6 col-sm-12">
                    <div class="input-group" id="switchGeneraleGroup" style="display: none">
                        <label class="input-group-addon control-label" id="label_switch_generale" for="switchGenerale1">
                            Test:
                        </label>
                        <input id="switchGenerale1" name="switchGenerale1" autocomplete="off" />
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-12">
                    <div class="input-group" id="switchGeneraleGroup2" style="display: none">
                        <label class="input-group-addon control-label" id="label_switch_generale2" for="switchGenerale2">
                            Test2:
                        </label>
                        <input id="switchGenerale2" name="switchGenerale2" autocomplete="off" />
                    </div>
                </div>
                <%--            <div class="col-lg-6 col-md-6 col-sm-12">
                    <b>Tutte le Stampe:</b>
                    <div class="MenuStampe">
                        <ul id="menu">
                        </ul>
                    </div>
                </div>--%>


                <%--            <div class="col-lg-6 col-md-6 col-sm-12" style="margin-block: 10px;">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="PreferitiSalvati" style="float: right;">
                                <ul id="PreferitiSalvatiMenu">
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>--%>
            </div>
            <div class="row" id="dataGroup">
                <div class="col-lg-4 col-md-6 col-sm-12" id="dataInizioGroup">

                    <div class="input-group">
                        <label class="input-group-addon control-label alert-info" id="lblDataInizio" for="txtDataInizio">
                            Data inizio:
                        </label>
                        <input type="text" id="txtDataInizio" name="txtDataInizio" class="form-control" aria-describedby="lblDataInizio" />
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-12" id="dataFineGroup">

                    <div class="input-group">
                        <label class="input-group-addon control-label alert-info" id="lblDataFine" for="txtDataFine">
                            Data Fine:
                        </label>
                        <input type="text" id="txtDataFine" name="txtDataFine" class="form-control" aria-describedby="lblDataFine" />
                    </div>
                </div>


            </div>
        </div>
    </div>



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <%--    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
        var cIsBudget = "#<%=hdIsBudget.ClientID() %>";
        var cIdBudget = "<%= 0 %>";
    </script>--%>

    <%--<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>--%>
    <%--<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>--%>

    <script type="text/javascript">
        var usaFiltroRicercaNG = $('#<%=hd_usaFiltroRicercaNG.ClientID %>').val() == 'True' ? true : false;
        var currentPiva = $('#<%=hd_CurrentPiva.ClientID %>').val();
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("menuStampeBootstrap_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("menuStampeBootstrap.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("menuStampeBootstrap_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("menuStampeBootstrap_jQueryDocReady.js") %>"></script>


</asp:Content>
