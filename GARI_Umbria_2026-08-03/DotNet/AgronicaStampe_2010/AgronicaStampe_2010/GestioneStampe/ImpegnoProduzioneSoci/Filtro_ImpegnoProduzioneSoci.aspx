<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="Filtro_ImpegnoProduzioneSoci.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_ImpegnoProduzioneSoci" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .ddl-kendo {
            white-space: normal;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="panel-body">
        <div class="row">
            <div class="col-sm-12 col-md-9 col-lg-7">
                <div class="input-group boxSocio">
                    <label class="input-group-addon lbl_required" id="lblSocio" for="ddlSocio">Socio</label>
                    <input type="text" name="ddlSocio" id="ddlSocio" class="form-control ddl-kendo" />
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-6 col-md-5 col-lg-4">
                <div class="input-group boxImpianti">
                    <label class="input-group-addon" id="lblImpianti" for="ddlImpianti">Stampa gli Impianti</label>
                    <input type="text" name="ddlImpianti" id="ddlImpianti" class="form-control ddl-kendo" />
                </div>
            </div>
            <div class="col-sm-6">
                <p id="infoImpiantiAttiviAnno"><i class="fa fa-info-circle"></i>Impianti con Data Inizio <= 31/12/<i>Anno Validità Inizio</i> e Data Fine >= <i>Validità Inizio</i></p>
                <p id="infoImpiantiNatiAnno"><i class="fa fa-info-circle"></i>Impianti con Data Inizio >= <i>Validità Inizio</i> e Data Inizio <= 31/12/<i>Anno Validità Inizio</i></p>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-6 col-md-5 col-lg-4">
                <div class="input-group boxTipoArchivio">
                    <label class="input-group-addon" id="lblTipoArchivio" for="ddlTipoArchivio">Stampa da</label>
                    <input type="text" name="ddlTipoArchivio" id="ddlTipoArchivio" class="form-control ddl-kendo" />
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-6 col-md-5 col-lg-4">
                <div class="input-group boxValiditaInizio">
                    <label class="input-group-addon lbl_required" id="lblValiditaInizio" for="dpValiditaInizio">Validità inizio</label>
                    <input type="text" name="dpValiditaInizio" id="dpValiditaInizio" class="form-control kendoCalendar" />
                </div>
            </div>
            <div class="col-sm-6">
                <p id="infoInizioValidita"><i class="fa fa-info-circle"></i>Periodo di stampa compreso fra questa data e fine anno</p>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-2">
                <div class="btn btn-success buttonClass" id="btnStampa">Stampa</div>
            </div>
        </div>
        
    </div>

    <input type="hidden" id="hfPiva" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript">
        var cIdPiva = "#<%=hfPiva.ClientID() %>";
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Filtro_ImpegnoProduzioneSoci_jQueryDocReady.js") %>"></script>

</asp:Content>
