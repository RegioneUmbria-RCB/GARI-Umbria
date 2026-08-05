<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/UMABootstrap.Master" 
    CodeBehind="ReportControllo.aspx.vb" Inherits="AgronicaUMA.ReportControllo" %>

<%--<%@ Register TagPrefix="uc" TagName="GestioneLavorazioniMenuUC" Src="../GestioneLavorazioni/LavorazioneMenuUC.ascx" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }
        .k-grid .k-column-title {
            white-space: normal;
        }
        /*.k-icon, k-i-more-vertical, .k-grid-header .k-link:link, .k-grid-header .k-link:visited, 
    .k-grid-header .k-nav-current.k-state-hover .k-link, .k-grouping-header .k-link {
    color: black;
    font-weight: bold;
    }*/

        .dpiOn {
            box-shadow: 1px 1px rgba(0,0,0,.075) inset;
            background-color: Orange;
            pointer-events: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <%--<div id="panelArea" class="panel-group searchArea" style="opacity: 0;">

        <div class="panel-group preArea" >
            <div class="panel-body" style="padding-top: 30px;">

                <h4>
                    Report di Controllo UMA
                </h4>

            </div>
        </div>
    </div>--%>

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">

                <div class="jumbotron">
                    <div class="container" style="width: 100%; padding: 0">
                        <div class="container_report" style="padding: 0; /*margin-bottom: 70px*/">
                            <div id="tabstrip_report" >
                                <ul style="text-transform: uppercase;">
                                    <li class="k-state-active k-active elas" id="tab1">
                                        <asp:Localize Text="Report ELAS" runat="server">Report ELAS</asp:Localize>
                                    </li>
                                    <li class="elencoIna" id="tab2">
                                        <asp:Localize Text="Elenco Inadempienti al 30/6" runat="server">Elenco Inadempienti al 30/6</asp:Localize>
                                    </li>
                                    <li class="elencoTrasf" id="tab3">
                                        <asp:Localize Text="Elenco Trasferimenti" runat="server">Elenco Trasferimenti</asp:Localize>
                                    </li>
                                    <li class="segnalazioneAccise" id="tab4">
                                        <asp:Localize Text="Segnalazione e recupero accise" runat="server">Segnalazione recupero accise</asp:Localize>
                                    </li>
                                </ul>

                                <%--TAB elas--%>

                                <div class="panel-group reportELAS" >

                                    <div class="row">

                                        <div class="col-lg-2 col-md-2 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="annolab" for="anno">
                                                            Anno
                                                        </label>
                                                        <input type="text" id="anno" name="anno" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-3 col-md-4 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="bimestrelab" for="bimestre">
                                                            Bimestre
                                                        </label>
                                                        <input type="text" id="bimestre" name="bimestre" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="btn btn-success" id="btn_carica_report">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Cerca
                                            </span>
                                        </div>

                                        <div class="btn btn-success" id="btn_esporta_carica" style="margin-left: 10px">
                                            <span class="fa fa-table lampeggiante"></span><span class="lampeggiante">Esporta e carica
                                            </span>
                                        </div>

                                        <div class="btn btn-success" id="btn_elenco_report_elas" style="margin-left: 10px">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Elenco report ELAS
                                            </span>
                                        </div>

                                    </div>
                                    
                                        <div id="dialogCanc">
                                        </div>
                                        <div id="dialogRinuncia">
                                        </div>
                                        <!--Griglia-->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="tab_griglia_reportELAS"></div>
                                        </div>
                                    </div>
                                <div class="panel-group ElencoInadempienti" style="display: none" >

                                    <div class="row">

                                        <div class="col-lg-2 col-md-2 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="annolabIna" for="annoIna">
                                                            Anno
                                                        </label>
                                                        <input type="text" id="annoIna" name="annoIna" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                           <div class="col-lg-3 col-md-3 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="provLab" for="prov">
                                                            Provincia
                                                        </label>
                                                        <input type="text" id="prov" name="prov" class="form-control" onchange="ddlProv_Change();">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-3 col-md-3 col-sm-12 text-center city">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="cittaLab" for="citta">
                                                            Città
                                                        </label>
                                                        <input type="text" id="citta" name="citta" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="row">

                                        <div class="col-lg-5 col-md-5 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="statoPraticaLab" for="statoPratica">
                                                            Stato Pratica
                                                        </label>
                                                        <input type="text" id="statoPratica" name="statoPratica" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-3 col-md-3 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="contoLab" for="conto">
                                                            conto
                                                        </label>
                                                        <input type="text" id="conto" name="conto" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        </div>

                                    <div class="row">

                                        <div class="btn btn-success" id="btn_carica_Elenco">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Cerca
                                            </span>
                                        </div>

                                        <div class="btn btn-success" id="btn_esporta_Elenco" style="margin-left: 10px">
                                            <span class="fa fa-table lampeggiante"></span><span class="lampeggiante">Esporta e carica
                                            </span>
                                        </div>

                                        <div class="btn btn-success" id="btn_elenco_report_inadempienti" style="margin-left: 10px">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Elenco report inadempienti
                                            </span>
                                        </div>

                                    </div>
    
                                        <!--Griglia-->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="tab_griglia_ElencoInadempienti"></div>
                                        </div>
                                    </div>
                                <div class="panel-group ElencoTrasferimenti" style="display: none" >

                                    <div class="row">
                                        <div class="col-lg-12" style="padding-bottom: 15px">
                                            <h5>Elenco trasferimenti ricevuti senza pratiche nel periodo di competenza</h5>                                        
                                        </div>
                                    </div>

                                    <div class="row">

                                        <div class="col-lg-2 col-md-2 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="annolabtrasf" for="annoTrasf">
                                                            Anno
                                                        </label>
                                                        <input type="text" id="annoTrasf" name="annoTrasf" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="btn btn-success" id="btn_carica_elenco_trasf">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Cerca
                                            </span>
                                        </div>

                                    </div>

                                    <!--Griglia-->
                                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="tab_griglia_elencoTrasf"></div>
                                    </div>
                                </div>
                                <div class="panel-group SegnaAccise" style="display: none" >

                                    <div class="row">

                                        <div class="col-lg-2 col-md-2 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="annolabAcc" for="annoAcc">
                                                            Anno
                                                        </label>
                                                        <input type="text" id="annoAcc" name="annoAcc" class="form-control" onchange="checkTermineUltimo()">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                            <div class="col-lg-3 col-md-3 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="provlabAcc" for="provAcc">
                                                            Provincia
                                                        </label>
                                                        <input type="text" id="provAcc" name="provAcc" class="form-control" onchange="ddlProvAcc_Change();">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-3 col-md-3 col-sm-12 text-center city">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="cittaLabAcc" for="cittaAcc">
                                                            Città
                                                        </label>
                                                        <input type="text" id="cittaAcc" name="cittaAcc" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="row">

                                        <div class="col-lg-3 col-md-3 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="tipoPraticaLabAcc" for="tipoPraticaAcc">
                                                            Tipo Pratica
                                                        </label>
                                                        <input type="text" id="tipoPraticaAcc" name="tipoPraticaAcc" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-3 col-md-3 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="contoLabAcc" for="contoAcc">
                                                            conto
                                                        </label>
                                                        <input type="text" id="contoAcc" name="contoAcc" class="form-control">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>   
                                        
                                        <div class="col-lg-3 col-md-3 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="giaSegnLabAcc" for="giaSegnAcc">
                                                            Pratiche gia' segnalate
                                                        </label>
                                                        <input type="text" id="giaSegnAcc" name="giaSegnAcc" class="form-control" onchange="mostraFiltriSegnalati();">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="row sagnalatiFiltri" style="display: none">                                        

                                        <div class="col-lg-3 col-md-3 col-sm-12 text-center">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="segnDaLabAcc" for="segnDaAcc">
                                                            Pratiche segnalate dal:
                                                        </label>
                                                        <input type="text" id="segnDaAcc" name="segnDaAcc" class="kendoCalendar">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="divAlertAnno col-lg-5 col-md-12 col-sm-12" style="color: red; display: none; margin-bottom: 10px">
                                            E' ancora possibile l'inserimento di rendicontazioni per l'anno selezionato
                                        </div>
                                    </div>

                                    <div class="row" style="margin-left: 15px">

                                        <div class="btn btn-success" id="btn_carica_Accise">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Cerca
                                            </span>
                                        </div>

                                        <div class="btn btn-success" id="btn_esporta_Accise" style="margin-left: 10px">
                                            <span class="fa fa-table lampeggiante"></span><span class="lampeggiante">Esporta e carica
                                            </span>
                                        </div>

                                        <div class="btn btn-success" id="btn_elenco_report_accise" style="margin-left: 10px">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Elenco report segnalazione recupero accise
                                            </span>
                                        </div>
                                    </div>

                                    <div class="row" style="margin-left: 5px; margin-top: 10px">

                                        <div class="dataSegn col-lg-3 col-md-3 col-sm-12 text-center" style="display: none">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="dataSegnLabAcc" for="dataSegnAcc">
                                                            Data Segnalazione
                                                        </label>
                                                        <input type="text" id="dataSegnAcc" name="dataSegnAcc" class="kendoCalendar">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="btn btn-warning" id="btn_segnalazione" style="margin-left: 10px">
                                            <span class="lampeggiante">Aggiorna come segnalati
                                            </span>
                                        </div>

                                        <div class="btn btn-danger" id="btn_annulla_segnalazione" style="margin-left: 10px">
                                            <span class="lampeggiante">Aggiorna come non segnalati
                                            </span>
                                        </div>

                                    </div>
    
                                    <!--Griglia-->
                                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="tab_griglia_SegnalazioniAccise"></div>
                                    </div>
                                </div>
                               
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    

    <!--dialogs varie-->
    <div id="confermaEliminazioneDialog"></div>

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdId_Agenda" runat="server" />
    <input type="hidden" id="hdId_Mov" runat="server" />
    <input type="hidden" id="hdId_Mov_Det" runat="server" />
    <input type="hidden" id="hdId_Agenda_CDG" runat="server" />
    <input type="hidden" id="hdModalita" runat="server" />
    <input type="hidden" id="hdAutomatico" runat="server" />
    <input type="hidden" id="hdId_CDG" runat="server" />
    <input type="hidden" id="hdLav_Cod" runat="server" />
    <input type="hidden" id="hdVeg_Cod" runat="server" />
    <input type="hidden" id="hdDes_Lib" runat="server" />
    <input type="hidden" id="hdMov_Desc" runat="server" />
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />
    <input type="hidden" id="hdiGiacenze" runat="server" />
    <input type="hidden" id="hdData" runat="server" />
    <input type="hidden" id="hdDataQdC" runat="server" />
    <input type="hidden" id="hdCosti_Ricavi" runat="server" />
    <input type="hidden" id="hdTabRisorseDescr" runat="server" />
    <input type="hidden" id="hdTabCentriDescr" runat="server" />
    <input type="hidden" id="hdId_Attivita" runat="server" />
    <input type="hidden" id="hdAttivita_Des" runat="server" />
    <input type="hidden" id="hdAttivita_Eredita_Enabled" runat="server" />
    <input type="hidden" id="hd_Poliennale" runat="server" />
    <input type="hidden" id="hdSplit" runat="server" />
    <input type="hidden" id="hdTabEreditaDescr" runat="server" />
    <input type="hidden" id="hdPivaSuperUser" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
        
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportControllo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportControllo_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportControllo_ws_client.js") %>"></script>

    <script type="text/javascript">

        var username_master = "<%= Master.agroMasterPage_UtenteUsername %>";
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "<%=hdPiva.Value() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";
        var cId_Agenda = "#<%=hdId_Agenda.ClientID() %>";
        var cId_Mov = "#<%=hdId_Mov.ClientID() %>";
        var cId_Mov_Det = "#<%=hdId_Mov_Det.ClientID() %>";
        var cId_Agenda_CDG = "#<%=hdId_Agenda_CDG.ClientID() %>";
        var cModalita = "#<%=hdModalita.ClientID() %>";
        var cAutomatico = "#<%=hdAutomatico.ClientID() %>";
        var cId_CDG = "#<%=hdId_CDG.ClientID() %>";
        var cLav_Cod = "#<%=hdLav_Cod.ClientID() %>";
        var cVeg_Cod = "#<%=hdVeg_Cod.ClientID() %>";
        var cData = "#<%=hdData.ClientID() %>";
        var cPoliennale = "#<%=hd_Poliennale.ClientID() %>";
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
        var cSplit = "#<%=hdSplit.ClientID() %>";
        var QS_Avanzamento = <%=QS_Avanzamento.ToString %>;
        var QS_Piva = "<%= QS_Piva.ToString %>";
        var cIdPivaSuperUser = "<%=hdPivaSuperUser.Value() %>";
    </script>

</asp:Content>
