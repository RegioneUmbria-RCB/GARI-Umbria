<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Servizi_Lista_BS.aspx.vb" Inherits="AgroAgenda_2010.Servizi_Lista_BS" ClientIDMode="Static" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
/*        .btn-center {
            width: 100%;
            margin-bottom: 5px;
            margin-top: 5px;
        }

        .container {
            margin-left: 0px !important;
            margin-right: 0px !important;
            width: 100% !important
        }
*/
        /*@media (min-width:800px){
            .container{
                
            }
        }*/
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="" id="tab_dati_generali">
        <div class="">

            <div class="row">

                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_imprese">Impresa</span>
                                <input type="text" id="Cmb_Imprese" name="Data" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-12 border_si">
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12 ">
                        <!--text-center-->
                        <h4>Filtro Data</h4>
                    </div>

                    <div class="row">

                        <div class="col-lg-1 col-md-1 col-sm-2 text-center">
                            <input id="data-switch" aria-label="Data Switch" />
                        </div>

                        <div class="col-lg-3 col-md-3 col-sm-6 text-center">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_validita_inizio" for="TxtValiditaInizio"><i class="fa fa-calendar"></i>Valide alla data</span>
                                        <input type="text" id="TxtValiditaPratica" class="form-control kendoDate " />
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <div id="FiltroAvanzatoPratiche" class="row" style="display:none">
                    <div class="col-lg-12 col-md-12 col-sm-12 ">
                        <h4>Filtro Aziende per Pratiche</h4>
                    </div>

                    <div class="row">

                        <div class="col-lg-1 col-md-1 col-sm-2 text-center">
                            <input id="pratiche-switch" aria-label="Pratiche Switch" />
                        </div>

                        <div class="col-lg-9 col-md-9 col-sm-10 ">
                            <div id="kendoFiltroPratiche"></div>
                        </div>

                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12 ">
                        <h4>Filtro Servizi</h4>
                    </div>

                    <div class="row">

                        <div class="col-lg-1 col-md-1 col-sm-2 text-center">
                            <input id="servizi-switch" aria-label="Pratiche Switch" />
                        </div>

                        <div class="col-lg-9 col-md-9 col-sm-10 ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_servizi"></i>Servizi</span>
                                        <input type="text" id="ddlServizi" class="kendoDropDownList" />
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </div>

            <div class="row">

                <div class="col-lg-12 col-md-12 col-sm-12 text-center">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div id="btn_CercaPratiche" class="btn btn-info">
                                <i class="fa fa-search" aria-hidden="true"></i>
                                <span id="lbl_CercaPratiche">Cerca Pratiche</span>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <div class="">
                <div class="">
                    <div id="kendoPratiche"></div>
                </div>
            </div>

        </div>
    </div>
    <div style="display: none">
        <div id="kendoDialogNuovo">
            <div class="jumbotron">
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_imprese_new">Impresa</span>
                                    <input type="text" id="Cmb_Imprese_New" name="Data" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">Servizio</span>
                                    <input type="text" id="Cmb_Servizio" name="Data" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">Data Inizio Competenza</span>
                                    <input type="text" id="Cmb_Data_Inizio" name="Data" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">Data Fine Competenza</span>
                                    <input type="text" id="Cmb_Data_Fine" name="Data" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">Data Apertura Pratica</span>
                                    <input type="text" id="Cmb_Data_Apertura" name="Data" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">Numero</span>
                                    <input type="text" id="Txt_Numero" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="kendoDialogPassaggioStato">
            <div class="jumbotron">
                <input type="hidden" id="HDPassaggioStato_Pratica_Cod" />
                <input type="hidden" id="HDPassaggioStato_PassaggioDiStato" />
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <h4 id="lbl_Servizio"></h4>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="input-group">
                                <span class="input-group-addon alert-info">Procedura:</span>
                                <input type="text" id="CmbProcedura" name="Data" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <h5 id="lbl_StatoAttuale"></h5>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">Stato di Destinazione:</span>
                                    <input type="text" id="Cmb_Stato_Destinazione" name="Data" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <div class="row" style="display:none;">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">Data Riferimento</span>
                                    <input type="text" id="Txt_Data_Riferimento" name="Data" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">Note</span>
                                    <textarea id="Txt_Note" class="form-control"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <div id="divDSS">
                    </br>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <h4>DSS</h4>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Pacchetti modelli DSS acquistati</span>
                                        <input type="text" id="DSS_cmb_pacchettiAcquistati" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Data Scadenza</span>
                                        <input type="text" id="Txt_Data_Scadenza" name="Data" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 text-center">
                            <div id="btn_AggiungiDSS" class="btn btn-info">
                                <i class="fa fa-arrow-down" aria-hidden="true"></i>
                                <span id="lbl_AggiungiDSS">Aggiungi pacchetto selezionato</span>
                                <i class="fa fa-arrow-down" aria-hidden="true"></i>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 text-center">
                            <div id="kendoDSS"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div id="kendoDialogCambiaServizio">
            <div class="jumbotron">
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="input-group">
                                <span class="input-group-addon alert-info">Servizio:</span>
                                <input type="text" id="CmbCambiaServizio" name="Data" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>
    <input type="hidden" id="HD_Username" runat="server" />
    <input type="hidden" id="HD_piva" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script src="Servizi_Lista_BS.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="Servizi_Lista_BS_jQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="Servizi_Lista_BS_ws_client.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

    <script id="templatekendoPratiche" type="text/x-kendo-template">
        <div class="btn btn-success" id="AttivaServizi" onClick="AttivaServizi();" style="display: none">
            <span class="lampeggiante">Attiva Nuovi Servizi</span>
        </div>
        <div class="btn btn-success" id="PassaggioStato" onClick="Passaggio_di_Stato();" style="display: none">
            <span class="lampeggiante">Passaggio di Stato</span>
        </div>
        <div class="btn btn-success" id="BloccaP" onClick="BloccaPratiche();" style="display: none">
            <span class="lampeggiante"><i class="fa fa-lock" aria-hidden="true"></i></span>
        </div>
        <div class="btn btn-success" id="SbloccaP" onClick="SbloccaPratiche();" style="display: none">
            <span class="lampeggiante"><i class="fa fa-unlock" aria-hidden="true"></i></span>
        </div>
        <div class="btn btn-success" id="UndoP" onClick="UndoPratiche();" style="display: none">
            <span class="lampeggiante" style="display:flex;align-items:center;justify-content:center"><i class="fa fa-undo fa-2x" aria-hidden="true"></i> Annulla Passaggio di Stato</span>
        </div>
        <div class="btn btn-success" id="EsportazioneZespri" onClick="EsportazioneZespri();" style="display: none">
            <span class="lampeggiante">Esportazione QDC Zespri</span>
        </div>
        <div class="btn btn-success" id="EsportazioneRegione" onClick="EsportazioneRegione();" style="display: none">
            <span class="lampeggiante">Esportazione Regione</span>
        </div>
        <div class="btn btn-success" id="CambiaServizio" onClick="CambiaServizio();" style="display: none">
            <span class="lampeggiante">Cambia Servizio</span>
        </div>
    </script>

    <script type="text/javascript">
        var id_HD_Username = "<%= HD_Username.ClientID%>";
        var id_HD_Piva = "<%= HD_piva.ClientID%>";
    </script>

</asp:Content>
