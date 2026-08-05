<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Filtro_RiepilogoSuperficiMultiazienda.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_RiepilogoSuperficiMultiazienda" MasterPageFile="~/Master/StampeBootstrap.Master" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Filtro Riepilogo Utilizzo Superfici</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div id="containerFiltroStampe" class="container">
            <%--<div class="row">
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
            </div>--%>
        
        

            <div class="row">
                <div class="col-sm-12 col-md-10 col-lg-8">
                    <div class="input-group containerColonneStampa">
                        <label class="input-group-addon" id="lblColonneStampa" for="msColonneStampa">Seleziona i  campi che vuoi estrarre:</label>
                        <select type="text" name="msColonneStampa" id="msColonneStampa" class="form-control"></select>
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
                    <button type="submit" form="Form1" class="btn btn-success" id="btnStampa">STAMPA</button>
                </div>
            </div>

        </div>

    <!-- Hidden Controls Gestiti Lato Server -->
    <input type="hidden" id="hfPiva" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    
    
<%--    <script src="<%= ResolveClientUrl("~/GestioneStampe/RiepilogoSuperfici/MultiAzienda/Filtro_RiepilogoSuperficiMultiazienda_ws_client.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/GestioneStampe/RiepilogoSuperfici/MultiAzienda/Filtro_RiepilogoSuperficiMultiazienda.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/GestioneStampe/RiepilogoSuperfici/MultiAzienda/Filtro_RiepilogoSuperficiMultiazienda_jQueryDocReady.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>--%>


    <script type="text/javascript">
        // Variabili globali pagina che necessitano di valorizzazione tramite visual basic
        var cIdPiva = "#<%= hfPiva.ClientID %>";

        $(document).ready(function () {

            // Imposto la pagina che deve chiamare il form creato dalla Master
            $("#Form1").prop("action", "./RiepilogoSuperficiMultiazienda_XLS.aspx");

            $(".kendoCalendar").kendoDatePicker({
                footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
                max: new Date(2100, 11, 31),
                format: "dd/MM/yyyy"
            });

            var today = new Date();
            today.setHours(0, 0, 0);
            set_data("dpDataRif", today, null);


            creaKendoMultiselect(
                "msColonneStampa",
                {
                    read: function (options) {
                        options.success([
                            { id: "PartitaIva", nome: "Partita IVA" },
                            { id: "RagSoc", nome: "Ragione Sociale" },
                            { id: "CentroAziendale", nome: "Centro Aziendale" },
                            { id: "SpecieVegetale", nome: "Specie Vegetale" },
                            { id: "Varieta", nome: "Varietà" },
                            { id: "SupImpianto", nome: "Superficie Impianto" },
                            { id: "NumeroPiante", nome: "Numero Piante" },
                            { id: "Copertura", nome: "Copertura" },
                            { id: "Finalita", nome: "Finalità" },
                        ]);
                    }
                },
                "nome",
                "id"
            );

            Set_MultiselValue("msColonneStampa", "PartitaIva|RagSoc|SpecieVegetale|SupImpianto|NumeroPiante");

        });

        // Da utilizzare in caso in cui si cambi il pulsante da tipo 'submit' a tipo 'button', per eseguire codice specifico prima del submit del form
        //$("btnStampa").on("click", function (ev) {
        //    $("#Form1").submit();
        //});

    </script>

</asp:Content>