<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Filtro_RiepilogoUtilizzoSuperfici.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_RiepilogoUtilizzoSuperfici" MasterPageFile="~/Master/StampeBootstrap.Master" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Filtro Riepilogo Utilizzo Superfici</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="containerFiltroStampe" class="container">
        <div class="row">
            <div class="col-sm-12 col-md-8 col-lg-6">
                <div class="input-group containerImpresa">
                    <label class="input-group-addon lbl_required" id="lblImpresa" for="ddlImprese">
                        <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Impresa %>" runat="server">Impresa</asp:Localize>
                    </label>
                    <select type="text" name="ddlImprese" id="ddlImprese" class="form-control"></select>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-12 col-md-8 col-lg-6">
                <div class="input-group containerCentroAziendale">
                    <label class="input-group-addon" id="lblCentroAziendale" for="ddlCentriAziendali">Centro Aziendale</label>
                    <select type="text" name="ddlCentriAziendali" id="ddlCentriAziendali" class="form-control"></select>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6 col-md-4 col-lg-3">
                <div class="input-group containerDataRif">
                    <label class="input-group-addon" id="lblDataRif" for="dpDataRif">Data Riferimento</label>
                    <input type="text" name="dpDataRif" id="dpDataRif" class="form-control kendoCalendar" />
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-2">
                <button type="button" class="btn btn-success" id="btnStampa">STAMPA</button>
            </div>
        </div>
    </div>


    <!-- Hidden Controls Gestiti Lato Server -->
    <input type="hidden" id="hfPiva" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    
    
    <script src="<%= ResolveClientUrl("~/GestioneStampe/RiepilogoSuperfici/MonoAzienda/Filtro_RiepilogoUtilizzoSuperfici_ws_client.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/GestioneStampe/RiepilogoSuperfici/MonoAzienda/Filtro_RiepilogoUtilizzoSuperfici.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/GestioneStampe/RiepilogoSuperfici/MonoAzienda/Filtro_RiepilogoUtilizzoSuperfici_jQueryDocReady.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>


    <script type="text/javascript">
        // Variabili globali pagina che necessitano di valorizzazione tramite visual basic
        var cIdPiva = "#<%= hfPiva.ClientID %>";
    </script>

</asp:Content>