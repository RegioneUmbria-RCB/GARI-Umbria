<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="Statistiche_Accesso.aspx.vb" Inherits="AgronicaStampe_2010.Statistiche_Accesso" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron gias-mt-1 gias-m-1 gias-ml-1 gias-border-white ">
        <div class="row">
            <div class="col-lg-3 col-md-6 col-sm-12">
                <div class="form-group">
                    <div class="form-group">
                        <label id="lblddRicercaRapida" class="input-group-addon" for="ddRicercaRapida">
                            Filtra in base all'azienda:         
                            <i class="fa fa-info-circle" id="tooltipiAziende" aria-hidden="true" ></i>
                        </label>
                        <input name="ddRicercaRapida" id="ddRicercaRapida" class="form-control" />
                    </div>
                </div>
            </div>

            <div class="col-lg-3 offset-lg-1 col-md-6 col-sm-12">
                <div>
                    <label class="me-2">Filtra per:</label>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="rbl_data" id="dataCompetenza" value="1" checked>
                        <label class="form-check-label" for="dataCompetenza">Data Competenza (anno)</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="rbl_data" id="dataRegistrazione" value="2">
                        <label class="form-check-label" for="dataRegistrazione">Data Registrazione</label>
                    </div>
                </div>
            </div>
            <div class="col-lg-2 col-md-6 col-sm-12">
                <div class="form-group">
                    <div class="form-group">
                        <label class="input-group-addon" for="DataValiditaInizio">
                            Validità inizio:
                        </label>
                        <input name="DataValiditaInizio" id="DataValiditaInizio" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="col-lg-2 col-md-6 col-sm-12">
                <div class="form-group">
                    <div class="form-group">
                        <label class="input-group-addon" for="DataValiditaFine">
                            Validità fine:
                        </label>
                        <input name="DataValiditaFine" id="DataValiditaFine" class="form-control" />
                    </div>
                </div>
            </div>
        </div>
        <div id="line">
            <hr style="background-color: #002850; height: 1px;" />
        </div>
        <div class="row" id="divBottoni">

            <div class="col-lg-2">
                <div class="btn btn-success mt-30 xonne-btn-primary  py-2" id="btn_elaboraStatistica">
                    Elabora statistica
                </div>
            </div>
            <div class="col-lg-3">
                <div class="btn btn-success mt-30 xonne-btn-primary  py-2" id="btn_elencoSinteticoMovimenti">
                    <span class="fa fa fa-file-excel-o"></span>Elenco sintetico movimenti per azienda
                </div>
            </div>

            <div class="col-lg-4">
                <div class="btn btn-success mt-30 xonne-btn-primary  py-2" id="btn_elencoSinteticoDettagliOperazioni">
                    <span class="fa fa fa-file-excel-o"></span>Elenco sintetico con dettagli operazioni colturali
                </div>
            </div>

            <div class="col-lg-2">
                <div class="btn btn-success mt-30 xonne-btn-primary py-2" id="btn_reportDettagli">
                    <span class="fa fa fa-file-excel-o"></span>Report dettagli servizi per azienda
                </div>
            </div>

        </div>
        <div class="row" id="divRowCharts">
            <div class="col-lg-12">
                <div id="chartTransazioni" style="background: center"></div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <div id="chartAziendeMovimentante" style="background: center"></div>
            </div>
        </div>
    </div>


    <iframe id="iframe" style="display: none;"></iframe>
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <%--<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>--%>
    <%--<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>--%>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Statistiche_Accesso.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Statistiche_Accesso_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Statistiche_Accesso_jQueryDocReady.js") %>"></script>
</asp:Content>
