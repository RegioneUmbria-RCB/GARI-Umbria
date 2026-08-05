<%@ Page Title="Trasferimento" Language="vb" AutoEventWireup="false" CodeBehind="Trasferimento.aspx.vb"
    Inherits="AgroAgenda_2010.Trasferimento" MasterPageFile="~/Master/AgendaBootstrap.Master" ValidateRequest="false" %>

<%@ Register TagPrefix="uc" TagName="Giacenze_MagazzinoUC" Src="../GestioneMagazzini/Giacenze_MagazzinoUC.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
     
    .errorClass {
        border-color:#D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }

    .text-uppercase {
        text-transform: uppercase;
    }

    .blockModifica, .blockCancella, .blockDuplica {
        /* display: block; con questo non è possibile ridimensionare la colonna dei pulsanti */
        margin-top: 10px !important;
        margin-bottom: 10px !important;
    }

    .fixed-header {
        top:0;
        position:fixed;
        width:auto;
        z-index: 1;
    }

    #tab_giacenze .k-grid tbody .k-button,
    #tab_elenco_scarichi .k-grid tbody .k-button,
    #tab_elenco_carichi .k-grid tbody .k-button {
        -moz-min-width: 20px;
        -ms-min-width: 20px;
        -o-min-width: 20px;
        -webkit-min-width: 20px;
        min-width: 20px;
        width: 20px;
    }

.form-control, /* if this class is applied to a Kendo UI widget, its layout may change */
.container,
.container-fluid,
.row,
.col-xs-1, .col-sm-1, .col-md-1, .col-lg-1,
.col-xs-2, .col-sm-2, .col-md-2, .col-lg-2,
.col-xs-3, .col-sm-3, .col-md-3, .col-lg-3,
.col-xs-4, .col-sm-4, .col-md-4, .col-lg-4,
.col-xs-5, .col-sm-5, .col-md-5, .col-lg-5,
.col-xs-6, .col-sm-6, .col-md-6, .col-lg-6,
.col-xs-7, .col-sm-7, .col-md-7, .col-lg-7,
.col-xs-8, .col-sm-8, .col-md-8, .col-lg-8,
.col-xs-9, .col-sm-9, .col-md-9, .col-lg-9,
.col-xs-10, .col-sm-10, .col-md-10, .col-lg-10,
.col-xs-11, .col-sm-11, .col-md-11, .col-lg-11,
.col-xs-12, .col-sm-12, .col-md-12, .col-lg-12
{
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
}
             
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    
    <div class="row">
        <div class="col-sm-12">
            <div class="col-sm-12 border_si">
                <div class="col-lg-3 col-md-4 col-sm-12">
                    <div class="input-group" id="groupDataEmissione">
                        <label class="input-group-addon lbl_required" id="lbl_data_emissione" for="inDataEmissione">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataEmissione %>" runat="server">Data emissione</asp:Localize>:
                        </label>
                        <input id="inDataEmissione" name="inDataEmissione" class="kendoCalendar" type="date" style="width: 100%;" maxlength="10" required/>
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group">
        <%--                <span class="input-group-addon lbl_required"><asp:Localize meta:resourcekey="Cella" runat="server">Cella:</asp:Localize></span>
                        <input id="idCella_TLav" name="idCella_TLav" class="form-control"/>--%>

                        <label class="input-group-addon" id="lbl_cmb_destinazione" for="cmbDestinazioneDef">
                            <asp:Localize meta:resourcekey="DestinazioneDefault" runat="server">Destinazione Default</asp:Localize>:
                        </label>
                        <input id="cmbDestinazioneDef" name="cmbDestinazioneDef" class="form-control" required/>
                    </div>
                </div>
            </div>
        </div>
    </div>

    
    <div class="row">
        <div class="col-sm-12">
            <div class="col-sm-12 border_si">
                <div class="col-lg-3 col-md-4 col-sm-12">
                    <div class="input-group" id="groupTrasportoUDM">
       
                        <label class="input-group-addon" id="lblUnitaMisuraTrasporto" for="ddlUnitaMisuraTrasporto">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaUdM %>" runat="server">
                                Unità di misura
                            </asp:Localize>:
                        </label>
                        <input name="ddlUnitaMisuraTrasporto" id="ddlUnitaMisuraTrasporto" class="form-control" />
                    </div>
                </div>
                <div class="col-md-3 col-md-4 col-sm-6">
                    <div class="input-group" id="groupTrasportoQTY">
                        <label class="input-group-addon" id="lblDistanzaTrasporto" for="ntbDistanzaTrasporto">
                            <asp:Localize Text="<%$ Resources: DistanzaTrasporto %>" runat="server">Distanza Trasporto</asp:Localize>:
                        </label>
                        <input name="ntbDistanzaTrasporto" id="ntbDistanzaTrasporto" class="form-control" />
                    </div>
                </div>
             </div>
         </div>    
    </div>

    <div id="panelArea" class="panel-group searchArea" style="opacity: 0;">
                     
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                        <ul class="nav nav-tabs text-uppercase" role="tablist" id="tabs">
                            <li class="active"><a href="#tabScaricoSuLavorazione" data-toggle="tab" id="a_tabScaricoSuLavorazione">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Uscite %>" runat="server">Uscite</asp:Localize>
                            </a></li>
                            <li><a href="#tabCaricoDaLavorazione" data-toggle="tab" id="a_tabCaricoDaLavorazione">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ingressi %>" runat="server">Ingressi</asp:Localize>
                            </a></li>
<%--                            <li><a href="#tabUsciteDaDDT" data-toggle="tab" id="a_tabUsciteDaDDT">
                                <asp:Localize meta:resourcekey="UsciteDaDDT" runat="server">Uscite da DDT</asp:Localize>
                            </a></li>--%>
                        </ul>
                        <div class="tab-content">
 
                            <!-- tab Scarico su Lavorazione -->
                            <div class="tab-pane fade active in" id="tabScaricoSuLavorazione" style="overflow: auto; margin-bottom: 70px;">
                               <div class="row">
                                   <div class="col-lg-12 col-md-12 col-sm-12" style="border:2px;">
                                       <uc:Giacenze_MagazzinoUC id="Giacenze_MagazzinoUC" runat="server" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <%--<div id="areaRicercaScarichi" class="panel-group ScarichiArea">--%>
                                     <%--       <div class="panel-body" style="padding-top:5px;">   --%>   
                                                <!-- Griglia scaricati su lavorazione -->
                                                <div style="overflow: auto; margin-top: 5px;">
                                                    <div id="tab_elenco_scarichi"></div>
                                                </div>
                                          <%--  </div>--%>
                                        <%--</div>--%>
                                    </div>
                                </div>
                            </div>
                            
                            <!-- tab Carico su Lavorazione -->
                            <div class="tab-pane fade in" id="tabCaricoDaLavorazione" style="overflow: auto; margin-bottom: 70px;">
                                   <div class="row">
                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                           <%-- <div id="areaRicercaCarichi" class="panel-group CarichiArea">--%>
                                               <%-- <div class="panel-body" style="padding-top:5px;">   --%>   
                                                    <!-- Griglia caricati su lavorazione -->
                                                    <div style="overflow: auto; margin-top: 5px;">
                                                        <div id="tab_elenco_carichi"></div>
                                                    </div>
                                                <%--</div>--%>
                                           <%-- </div>--%>
                                        </div>
                                    </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        
    </div>

    <!-- fine container -->

    <input type="hidden" id="hdPivaSuperuser" runat="server" />
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdId_Agenda" runat="server" />
    <input type="hidden" id="hdId_Mov_Scarico" runat="server" />
    <input type="hidden" id="hdId_Mov_Carico" runat="server" />
    <input type="hidden" id="hdData_Op" runat="server" />
    <input type="hidden" id="hdDistanza_Trasporto_UDM" runat="server" />
    <input type="hidden" id="hdDistanza_Trasporto" runat="server" />
    <input type="hidden" id="hdKendo_TestataLavorazione" runat="server" />
    <input type="hidden" id="hdKendo_Scarichi" runat="server" />
    <input type="hidden" id="hdKendo_Carichi" runat="server" />
    <input type="hidden" id="hdKendo_Prodotti" runat="server" />
    <input type="hidden" id="hf_Modulo_Anagrafe_Log" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoGestioneGHG" runat="server" />


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPivaSuperuser = "#<%=hdPivaSuperuser.ClientID() %>";
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdAgenda = "#<%=hdId_Agenda.ClientID() %>";
        var cIdMovScarico = "#<%=hdId_Mov_Scarico.ClientID() %>";
        var cIdMovCarico = "#<%=hdId_Mov_Carico.ClientID() %>";
        var cIdDataOp = "#<%=hdData_Op.ClientID() %>";
        var cDistanza_Trasporto_UDM = "#<%=hdDistanza_Trasporto_UDM.ClientID() %>";
        var cDistanza_Trasporto = "#<%=hdDistanza_Trasporto.ClientID() %>";
        var modulo_anagrafe_log = $("#<%=hf_Modulo_Anagrafe_Log.ClientID() %>").val().split("|");
        var cUtenteAbilitatoGestioneGHG = "#<%=hf_UtenteAbilitatoGestioneGHG.ClientID() %>";
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/template_comuni_dropdown.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/imballi_pesi_FF.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneMagazzini/giacenze_magazzino_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneLavorazioni/carichi_scarichi_ws_client.js")) %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneMagazzini/giacenze_magazzino.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneLavorazioni/carichi_scarichi.js")) %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Trasferimento.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Trasferimento_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Trasferimento_ws_client.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneLavorazioni/lavorazioni_ws_client.js")) %>" ></script>
   
</asp:Content>