<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/UmaBootstrap.Master" CodeBehind="RiepilogoDatiUMA.aspx.vb" 
    Inherits="AgronicaUMA.RiepilogoDatiUMA" %>
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

    .infoLabel {
        font-weight: bold;
        font-size: 14px;
    }
    .info {
        font-size: 14px;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
   
    <div class="panel-body" style="padding-top: 30px;">        
        <div class="row">
            <div class="form-horizontal">
                <%--Anno--%>
                <div class="col-lg-3 col-md-3 col-sm-12 text-left">
                    <div class="input-group">
                        <label class="input-group-addon control-label alert-info" for="ddlAnno">
                            Anno
                        </label>
                        <input type="text" id="ddlAnno" name="ddlAnno" class="form-control" onchange="ddlAnno_Change();">
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-horizontal">
                <%--Azienda--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left">
                    <label class="infoLabel">
                        Azienda
                    </label>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                    <label class="info" id="azienda">
                        <%--Xxxxxxx Xxxxxx--%>
                    </label>
                </div>
                <%--Conto--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left">
                    <label class="infoLabel">
                        Conto
                    </label>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                    <label class="info" id="conto">
                        <%--Proprio / Terzi--%>
                    </label>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-horizontal">
                <%--CUAA--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left">
                    <label class="infoLabel">
                        CUAA
                    </label>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                    <label class="info" id="cuaa">
                        <%--XXXXX--%>
                    </label>
                </div>
                <%--Partita Iva--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left" style="width:12.5%">
                    <label class="infoLabel">
                        Partita Iva
                    </label>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12 text-left">
                    <label class="info" id="piva">
                        <%--nnnnnnnnnnn--%>
                    </label>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-horizontal">
                <%--Via--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left">
                    <label class="infoLabel">
                        Via
                    </label>
                </div>
                <div class="col-lg-11 col-md-11 col-sm-12 text-left">
                    <label class="info" id="address">
                        <%--Via Xxxx N 1nn--%>
                    </label>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-horizontal">
                <%--Città--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left">
                    <label class="infoLabel">
                        Città
                    </label>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                    <label class="info" id="citta">
                        <%--XXXXX--%>
                    </label>
                </div>
                <%--CAP--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left">
                    <label class="infoLabel">
                        CAP
                    </label>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12 text-left">
                    <label class="info" id="cap">
                        <%--nnnnn--%>
                    </label>
                </div>
                <%--Prov.--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left">
                    <label class="infoLabel">
                        Prov.
                    </label>
                </div>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left">
                    <label class="info" id="prov">
                        <%--XX--%>
                    </label>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-horizontal">
                <%--Telefono--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left" style="width:8.5%">
                    <label class="infoLabel">
                        Telefono
                    </label>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                    <label class="info" id="telefono">
                        <%--xxxxx--%>
                    </label>
                </div>
                <%--E-Mail--%>
                <div class="col-lg-1 col-md-1 col-sm-12 text-left" style="width:8.5%">
                    <label class="infoLabel">
                        E-Mail
                    </label>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12 text-left">
                    <label class="info" id="email">
                        <%--xxx--%>
                    </label>
                </div>
            </div>
        </div>
        <div style="height: 30px"></div>

        <%--Litri assegnati in richieste anno precedente--%>
        <div id="litriAssRichiestaAnnoPrecSection">
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri assegnati in richieste anno precedente
                        </label>
                    </div>
                    <div id="litriAssRichiestaAnnoPrec">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Richiedente
                        </label>
                    </div>
                    <div class="col-lg-7 col-md-7 col-sm-12 text-left">
                        <label class="info" id="litriAssRichiestaAnnoPrec_Richiedente">
                            <%--cdxxxxxx [Nome cognome]--%>
                        </label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Approvatore
                        </label>
                    </div>
                    <div class="col-lg-7 col-md-7 col-sm-12 text-left">
                        <label class="info" id="litriAssRichiestaAnnoPrec_Approvatore">
                            <%--cmxxxxxx [Nome Cognome]--%>
                        </label>
                    </div>
                </div>
            </div>
            <div style="height: 30px"></div>
        </div>

        <%--Litri assegnati in rendicontazione anno precedente--%>
        <div id="litriAssRendicontazioneAnnoPrecSection">
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri assegnati in rendicontazione anno prec.
                        </label>
                    </div>
                    <div id="litriAssRendicontazioneAnnoPrec">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Richiedente
                        </label>
                    </div>
                    <div class="col-lg-7 col-md-7 col-sm-12 text-left">
                        <label class="info" id="litriAssRendicontazioneAnnoPrec_Richiedente">
                            <%--cdxxxxxx [Nome cognome]--%>
                        </label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Approvatore
                        </label>
                    </div>
                    <div class="col-lg-7 col-md-7 col-sm-12 text-left">
                        <label class="info" id="litriAssRendicontazioneAnnoPrec_Approvatore">
                            <%--cmxxxxxx [Nome Cognome]--%>
                        </label>
                    </div>
                </div>
            </div>
            <div style="height: 30px"></div>
        </div>
        

        <%--Litri Rimanenza anno precedente--%>
        <div id="litriRimAnnoPrecSection">
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri Rimanenza anno precedente
                        </label>
                    </div>
                    <div id="litriRimAnnoPrec">
                    </div>
                </div>
            </div>
            <div style="height: 30px"></div>
        </div>
        
        <%--Litri Recupero accisa--%>
        <div id="litriRecuperoAccSection">
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri Recupero accisa dichiarati
                        </label>
                    </div>
                    <div id="litriRecuperoAccDichiarati">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri Recupero accisa confermati
                        </label>
                    </div>
                    <div id="litriRecuperoAccConfermati">
                    </div>
                </div>
            </div>
            <div style="height: 30px"></div>
        </div>
                
        <%--Litri anticipo anno in corso--%>
        <div id="litriAnticipoAnnoCorrSection">
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri anticipo anno in corso
                        </label>
                    </div>
                    <div id="litriAnticipoAnnoCorr">
                    </div>
                </div>
            </div>
            <div style="height: 30px"></div>
        </div>
        
        <%--Litri assegnati prima richiesta--%>
        <div id="litriAssPrimaRichiestaSection">
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri assegnati prima richiesta
                        </label>
                    </div>
                    <div id="litriAssPrimaRichiesta">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Richiedente
                        </label>
                    </div>
                    <div class="col-lg-7 col-md-7 col-sm-12 text-left">
                        <label class="info" id="litriAssPrimaRichiesta_Richiedente">
                            <%--cdxxxxxx [Nome cognome]--%>
                        </label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Approvatore
                        </label>
                    </div>
                    <div class="col-lg-7 col-md-7 col-sm-12 text-left">
                        <label class="info" id="litriAssPrimaRichiesta_Approvatore">
                            <%--cmxxxxxx [Nome Cognome]--%>
                        </label>
                    </div>
                </div>
            </div>        
            <div style="height: 30px"></div>
        </div>

        <div id="richiesteIntegrative">
        </div>
        
        <%--Litri acquistati alla data--%>
        <div id="litriAcquistatiAllaDataSection">
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri acquistati alla data
                        </label>
                    </div>
                    <div id="litriAcquistatiAllaData">
                    </div>
                </div>
            </div>
        </div>
        

        <%--Litri acquistabili alla data--%>
        <div id="litriAcquistabiliAllaDataSection">
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="infoLabel">
                            Litri acquistabili alla data
                        </label>
                    </div>
                    <div id="litriAcquistabiliAllaData">
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                        <label class="info">
                            (al netto delle rimanenze)
                        </label>
                    </div>                
                </div>
            </div>
        </div>
        
    </div>

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
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RiepilogoDatiUMA.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RiepilogoDatiUMA_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RiepilogoDatiUMA_ws_client.js") %>" ></script>

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

        var QS_Piva = "<%= QS_Piva.ToString %>";
        var QS_Anno = "<%= QS_Anno.ToString %>";
        var QS_Type = "<%= QS_Type.ToString %>";
        var QS_PagArrivo = "<%=QS_PagArrivo.ToString %>"
    </script>

</asp:Content>

