<%@ Page Title="Fatturazione Elettronica" Language="vb" AutoEventWireup="false" CodeBehind="FattElettronica.aspx.vb"
    Inherits="AgroAgenda_2010.FattElettronica" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }

        .flex-row {
            flex-direction: row;
        }

        #tipo_doc_group .k-content, #radio_group .k-content {
            box-shadow: none;
            border: none;
        }

        #log_fatturazione td:first-child {
            white-space: nowrap;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <div class="panel-group searchArea" style="display: none;">
        <div class="panel-body" style="padding-top: 30px;">
            <div class="jumbotron">
                <div class="row">
                    <div class="col-lg-6 col-sm-6 col-sm-12 ">
                        <div class="input-group">
                            <label class="input-group-addon lbl_required" id="lblAzienda" for="ddlAziende">Azienda:</label>
                            <input type="text" id="ddlAziende" style="width: 100%;" />
                        </div>
                    </div>
                    <div class="col-lg-6 col-sm-6 col-xs-12" id="tipo_doc_group">
                        <div class="input-group flex-row">
                            <span class="input-group-addon lbl_required" id="lbl_TipoDoc" for="groupTipoDoc">Tipo Doc:</span>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="radio" id="rbFatture" name="groupTipoDoc" value="F" class="k-radio" />
                                <label class="k-radio-label" for="rbFatture">Fatture</label>
                            </div>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="radio" id="rbNoteCredito" name="groupTipoDoc" value="N" class="k-radio" />
                                <label class="k-radio-label" for="rbNoteCredito">Note di Accredito</label>
                            </div>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="radio" id="rbTipoTutto" name="groupTipoDoc" value="T" class="k-radio" checked="checked" />
                                <label class="k-radio-label" for="rbTipoTutto">Tutto</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-3 col-sm-6 col-xs-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon lbl_required" id="lbl_DataDal" for="txt_DataDal">Data Doc dal:</label>
                                    <input id="txt_DataDal" name="txt_DataDal" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-3 col-sm-6 col-xs-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon lbl_required" id="lbl_DataAl" for="txt_DataAl">Data Doc al:</label>
                                    <input id="txt_DataAl" name="txt_DataAl" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-5 col-sm-6 col-xs-12" id="radio_group">
                        <div class="input-group flex-row">
                            <span class="input-group-addon lbl_required" id="lbl_Esito" for="groupFiltroLav">Esito:</span>
                            <div id="divAperte" class="form-control k-content" style="width: auto;">
                                <input type="radio" id="rbPositivo" name="groupFiltroLav" value="P" class="k-radio" />
                                <label class="k-radio-label" for="rbPositivo">Solo positivo</label>
                            </div>
                            <div id="divChiuse" class="form-control k-content" style="width: auto;">
                                <input type="radio" id="rbNegativo" name="groupFiltroLav" value="N" class="k-radio" />
                                <label class="k-radio-label" for="rbNegativo">Solo negativo</label>
                            </div>
                            <div id="divTutte" class="form-control k-content" style="width: auto;">
                                <input type="radio" id="rbLavTutto" name="groupFiltroLav" value="T" class="k-radio" checked="checked" />
                                <label class="k-radio-label" for="rbLavTutto">Tutto</label>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-1 col-sm-6 col-xs-12">
                        <div class="btn btn-success xonne-btn-primary" id="btn_ricerca" style="margin-bottom: 10px;">
                            <i class="fa fa-search"></i>Ricerca
                        </div>
                    </div>
                </div>
                <%If hf_UtenteAbilitatoScrittura.Value Then %>
                <div id="servizi_fatturazione"></div>
                <% End If %>
            </div>
        </div>
    </div>
    <iframe id="iframe" style="display: none;"></iframe>

    <div class="panel-group elencoFattElettronica" style="display: none;">
        <!--Griglia-->
        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
            <div id="tab_fatt_elettronica"></div>
        </div>
    </div>

    <!-- fine container -->

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdIdFatturaElettr" runat="server" value="" />
    <input type="hidden" id="hdKendo_FattureElettroniche" runat="server" />
    <div id="details" style="padding: 16px;"></div>
    <script type="text/x-kendo-template" id="template">
    <div id="details-container">
        <h4>N. #= NumeroDocumento # del #= kendo.toString(DataDocumento,"dd/MM/yyyy") #</h4>
        <table class="table">
        <tbody>
            <tr>
                <td style="width:200px;">Cliente:</td>
                <td class="bold">#= Cliente #</td>
            </tr>
            <tr>
                <td width="20%">Blocco:</td>
                <td width="80%" class="bold">#= templateBlocco(BloccoFlag) #</td>
            </tr>
            <tr>
                <td>Nome File XML:</td>
                <td class="bold">#= NomeFileXML #</td>
            </tr>
            <tr>
                <td>Nome File ZIP:</td>
                <td class="bold">#= NomeFileZIP #</td>
            </tr>
            <tr>
                <td>Stato Invio:</td>
                <td class="bold">#= templateStatoInvio(StatoInvio) #</td>
            </tr>
            <tr>
                <td>Data Ora invio:</td>
                <td class="bold">#= templateData(DataOraInvio) #</td>
            </tr>
            <tr>
                <td>Stato Esito:</td>
                <td class="bold #= StatoEsito=="300"?"red":"" #">#= templateStatoEsito(StatoEsito) #</td>
            </tr>
            <tr>
                <td>Note:</td>
                <td class="bold">#= Note #</td>
            </tr>
        </tbody>
        </table>
        <div id="log_fatturazione"></div>
    </div>
    </script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("FattElettronica_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("FattElettronica.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("FattElettronica_jQueryDocReady.js") %>"></script>

</asp:Content>
