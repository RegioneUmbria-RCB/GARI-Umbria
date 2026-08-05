<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BloccoParticelle.aspx.vb"
    Inherits="AgronicaUMA.BloccoParticelle" MasterPageFile="~/Master/UmaBootstrap.Master" %>

<%--<%@ Register TagPrefix="uc" TagName="GestioneLavorazioniMenuUC" Src="../GestioneLavorazioni/LavorazioneMenuUC.ascx" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
    .errorClass {
        border-color: #D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
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
   
    <div id="panelArea" class="panel-group searchArea" style="opacity: 0;">

        <div class="panel-group preArea" style="display: none;">
            <div class="panel-body" style="padding-top: 30px;">

                <div class="row">

                    <div class="col-lg-8 col-md-8 col-sm-12 text-center">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon control-label alert-info" id="CTRL_Azienda" for="ddlAzienda">
                                        Impresa
                                    </label>
                                    <input type="text" id="ddlAzienda" name="ddlAzienda" class="form-control" aria-describedby="CTRL_Azienda" onchange="ddlAzienda_Change();">
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-4 col-md-4 col-sm-12" style="display: none;">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon control-label alert-info" id="CUAAlab" for="CUAA">
                                        CUAA
                                    </label>
                                    <input type="text" id="CUAA" name="CUAA" class="form-control" disabled="disabled">
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-4 col-md-4 col-sm-12 text-center">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon control-label alert-info" id="pivaLab" for="piva">
                                        P.IVA 
                                    </label>
                                    <input type="text" id="piva" name="piva" class="form-control" disabled="disabled">
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

                    <div class="col-lg-1 col-md-1 col-sm-4 text-center">
                        <div class="form-horizontal">
                                <div class="btn btn-success" id="btn_carica_griglia">
                                    <span class="fa fa-search lampeggiante"></span>
                                    <span class="lampeggiante">
                                        Cerca
                                    </span>
                               </div>
                        </div>
                    </div>
                    
                </div>

                <div class="row" style="display: none;">
                    
                    <div class="col-lg-4 col-md-4 col-sm-9">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon control-label alert-info" id="nDichiarazionelab" for="nDichiarazione">
                                        Numero Dichiarazione
                                    </label>
                                    <input type="text" id="nDichiarazione" name="nDichiarazione" class="form-control" disabled="disabled">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">

                <div class="jumbotron">
                    <div class="container" style="width: 100%; padding: 0">
                        <div class="container_blocco" style="padding: 0; /*margin-bottom: 70px*/">
                            
                                <div class="panel-group bloccoRichieste">
                                    
                                    <div class="row">

                                        <div id="dialogCanc">

                                        </div>
                                        <div id="dialogRinuncia">

                                        </div>
                                        <!--Griglia-->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="tab_griglia_bloccoParticelle"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- dialog modale squadre -->
    <div class="modal fade" id="iFrameSquadre" data-backdrop="static">
        <div class="modal-dialog" style="width: 98%; height: 98%">
            <div class="modal-content" style="height: 95%; border-radius: 0">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        <span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title">
                        <asp:Label ID="lblDescrSquadre" runat="server" meta:resourcekey="lblDescrSquadre" Text="Scelta Squadre"></asp:Label>
                    </h4>
                </div>
                <div class="modal-body" style="height: 85%; border-radius: 0">
                    <iframe src="" id="PaginaSquadre" style="width: 100%; height: 100%; display: block; border: 0"
                        frameborder="0"></iframe>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">
<%--                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Chiudi %>" runat="server">chiudi</asp:Localize>--%>
                        chiudi
                    </button>
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
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("bloccoParticelle.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("bloccoParticelle_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("bloccoParticelle_ws_client.js") %>" ></script>

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
    </script>

</asp:Content>
