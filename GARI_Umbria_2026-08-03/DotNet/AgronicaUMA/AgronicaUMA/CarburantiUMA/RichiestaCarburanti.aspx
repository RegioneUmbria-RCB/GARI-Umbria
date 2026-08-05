<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RichiestaCarburanti.aspx.vb"
    Inherits="AgronicaUMA.RichiestaCarburanti" MasterPageFile="~/Master/UmaBootstrap.Master" %>

<%@ Register TagPrefix="UCMacchine" TagName="UMA_Macchine" Src="./UMA_Macchine.ascx" %>
<%@ Register TagPrefix="UCTerzisti" TagName="UMA_Terzisti" Src="./UMA_Terzisti.ascx" %>
<%@ Register TagPrefix="UCAllevamenti" TagName="UMA_Allevamenti" Src="./RichiestaAllevamenti.ascx" %>
<%@ Register TagPrefix="UCAllevamentiUF" TagName="UMA_Allevamenti_UF" Src="./RichiestaAllevamenti_UF.ascx" %>
<%@ Register TagPrefix="UCDocumenti" TagName="UMA_Documenti" Src="./RichiestaDocumenti.ascx" %>
<%@ Register TagPrefix="UCTerzistiParz" TagName="UMA_TerzistiParziale" Src="./UMA_TerzistiParziale.ascx" %>



<%--<%@ Register TagPrefix="uc" TagName="GestioneLavorazioniMenuUC" Src="../GestioneLavorazioni/LavorazioneMenuUC.ascx" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }

        .dpiOn {
            box-shadow: 1px 1px rgba(0,0,0,.075) inset;
            background-color: Orange;
            pointer-events: none;
        }

        .warningCell {
            background-color: #FF7F50;
        }

        .errorCell {
            background-color: #DC143C;
        }

        .DeepStyleHeader {
            background-color: deepskyblue;
        }

        .verbale-istruttoria .row {
            margin-bottom: 10px;
        }

        .panel-heading-richiesta {
            background-color: #c3e3e3;
        }

        .k-widget.k-window.k-dialog.k-confirm {
            width: 50%;
            word-wrap: break-word;
        }

        .k-grid .k-column-title {
            white-space: normal;

        }

        .k-grid-header th.k-header {
            vertical-align: top; 
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <span id="alertPassaggio"></span>

    <div id="divDati" style="display: none">
        <div id="panelArea" class="panel-group searchArea" style="opacity: 0;">

            <div class="panel-group preArea" style="display: none;">
                <div class="panel-body" style="padding-top: 30px;">
                    <div class="row">

                        <div class="col-lg-8 col-md-8 col-sm-12">
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

                        <div class="col-lg-5 col-md-5 col-sm-12" style="display: none;">
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

                        <div class="col-lg-3 col-md-6 col-sm-6 text-center">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="cittaLab" for="citta">
                                            Città
                                        </label>
                                        <input type="text" id="citta" name="citta" class="form-control" disabled="disabled">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!--<div class="col-lg-5 col-md-5 col-sm-12 text-center">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="indirizzoLab" for="indirizzo">
                                            Indirizzo
                                        </label>
                                        <input type="text" id="indirizzo" name="indirizzo" class="form-control" disabled="disabled">
                                    </div>
                                </div>
                            </div>
                        </div>-->

                        <div class="col-lg-2 col-md-6 col-sm-6 text-center">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="annolab" for="anno">
                                            Anno
                                        </label>
                                        <input type="text" id="anno" name="anno" class="form-control" disabled="disabled" onchange="anno_Change();">
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="col-lg-2 col-md-6 col-sm-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="nDichiarazionelab" for="nDichiarazione">
                                            N. Dichiarazione
                                        </label>
                                        <input type="text" id="nDichiarazione" name="nDichiarazione" class="form-control" disabled="disabled">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-6 col-sm-6">

                            <div>
                                <h5>
                                    <p id="tipo-richiesta-par"></p>
                                </h5>
                            </div>

                            <div>
                                <h5>
                                    <p id="stato-richiesta-par"></p>
                                </h5>
                            </div>

                        </div>

                    </div>

                    <div class="row">
                        <div class="col-lg-7 col-md-12 col-sm-12" id="noRichiestaBox" style="display: none">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="noRichiestaAnnoProx" for="noRichiestaAnnoProxCheck">
                                            Azienda cessata: il prossimo anno non verranno inserite nuove richieste 
                                        </label>
                                        <input type="checkbox" id="noRichiestaAnnoProxCheck" name="noRichiestaAnnoProxCheck" class="kendoSwitch">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="errorInfoFineRendicontazione col-lg-5 col-md-12 col-sm-12" style="color: red; display: none">
                        </div>
                    </div>

                    <div class="row">

                        <div class="col-lg-4 col-md-4 col-sm-9">
                            <div class="btn btn-success" id="btn_crea_richiesta">
                                <span class="fa fa-plus lampeggiante"></span><span class="lampeggiante">Crea Nuova Richiesta
                                </span>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-9" style="display: none;">
                            <p>Stato:</p>
                            <p id="stato_pratica"></p>
                            <input type="hidden" id="stato_pratica_cod" />
                        </div>
                    </div>

                    <div>
                        <div id="row_Fascicolo" class="row" style="display: none">
                            <div class="col-lg-6">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon control-label alert-info" id="lbl_fascicolo" for="fascicolo">
                                                Fascicolo
                                            </label>
                                            <input type="text" id="fascicolo" name="fascicolo" class="form-control">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row" style="margin-top: 10px">
                            <div class="col-lg-7">
                                <div class="btn btn-success" id="btn_carica_richieste">
                                    <span class="fa fa-plus lampeggiante"></span><span class="lampeggiante" id="ricButt">Inserisci richiesta
                                    </span>
                                </div>

                                <div class="btn btn-success" id="btn_salva" style="display: none">
                                    <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
                                </div>
                                <div class="btn btn-warning" id="btn_stampa_rich_rendicon" style="display: none">
                                    <span class="fa fa-print lampeggiante"></span><span class="lampeggiante">Stampa</span>
                                </div>
                                <div class="btn btn-warning" id="btn_opzioni_stampa_istruttoria" style="display: none">
                                    <span class="fa fa-print lampeggiante"></span><span class="lampeggiante">Stampa Verbale</span>
                                </div>
                                <div class="btn btn-warning" id="btn_PassaggioDiStato" style="display: none;">
                                    Avanzamento Pratica
                                </div>
                                <div class="btn btn-warning" id="btn_DettaglioAppezzamenti" style="display: none;">
                                    Dettaglio Appezzamenti
                                </div>
                                <%--<div class="btn btn-warning" id="btn_SintesiRichieste" style="display: none;">
                                    Sintesi Richieste e Rendicontazioni
                                </div>--%>
                                <div class="btn btn-warning" id="btn_SintesiUMA" style="display: none;">
                                    Sintesi UMA
                                </div>
                            </div>
                            <div class="errorInfo" style="color: red; display: none">
                            </div>
                            <div class="btn btn-success" id="btn_ConsultaLavorazioniTerzisti" style="display: none;">
                                Consulta Lavorazioni Terzisti / Coop
                            </div>
                            <div class="btn btn-success" id="btn_apri_frame_analisi_terreno" style="display: none;">
                                Analisi Terreni
                            </div>

                        </div>
                    </div>

                    <div class="panel panel-default padding-top" id="panel-rimanenze-anno-prec" style="margin-top: 20px;">
                        <div class="panel-heading">
                            <h5 id="lbl_rimanenze_anno_prec">Rimanenze anno precedente</h5>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblRimanenza_Gasolio_prec" for="TxtRimanenza_Gasolio_prec">
                                                    Gasolio
                                                </label>
                                                <input type="text" id="TxtRimanenza_Gasolio_prec" name="TxtRimanenza_Gasolio_prec" class="form-control" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblRimanenza_Benzina_prec" for="TxtRimanenza_Benzina_prec">
                                                    Benzina
                                                </label>
                                                <input type="text" id="TxtRimanenza_Benzina_prec" name="TxtRimanenza_Benzina_prec" class="form-control" disabled>
                                            </div>
                                            <script src="configurazioneUMA_ws_client.js"></script>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblRimanenza_Gasolio_Serra_prec" for="TxtRimanenza_Gasolio_Serra_prec">
                                                    Gasolio Serra
                                                </label>
                                                <input type="text" id="TxtRimanenza_Gasolio_Serra_prec" name="TxtRimanenza_Gasolio_Serra_prec" class="form-control" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>

                    <div class="panel panel-default padding-top" id="panel-rimanenze" style="margin-top: 20px; display: none;">
                        <div class="panel-heading">
                            <h5 id="lbl_rimanenze"></h5>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblRimanenza_Gasolio" for="TxtRimanenza_Gasolio">
                                                    Gasolio
                                                </label>
                                                <input type="text" id="TxtRimanenza_Gasolio" name="TxtRimanenza_Gasolio" class="form-control">
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblRimanenza_Benzina" for="TxtRimanenza_Benzina">
                                                    Benzina
                                                </label>
                                                <input type="text" id="TxtRimanenza_Benzina" name="TxtRimanenza_Benzina" class="form-control">
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblRimanenza_Gasolio_Serra" for="TxtRimanenza_Gasolio_Serra">
                                                    Gasolio Serra
                                                </label>
                                                <input type="text" id="TxtRimanenza_Gasolio_Serra" name="TxtRimanenza_Gasolio" class="form-control">
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>

                    <div class="panel panel-default padding-top" id="panel-carburantiTerzisti" style="margin-top: 20px; display: none;">
                        <div class="panel-heading-richiesta" style="font-weight: 800;" id="lbl_Richiesta">
                            <%--<p ></p>--%>
                        </div>

                        <div class="row" id="row_MsgErroreModifica" style="display: none;">
                            <%--<div class="col-lg-3 col-md-4 col-sm-12">--%>
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <div class="msgErrorAnticipoModifica" style="color: red">
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--</div>--%>
                        </div>

                        <div class="panel-body" style="background-color: #c3e3e3">

                            <div class="row" id="row_percentualeTerzisti" style="margin-top: 10px; display: none;">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group col-lg-4 col-md-12 col-sm-12">
                                            <label class="input-group-addon control-label alert-info" id="lblPercentualeAnticipoTerzisti">
                                                Percentuale Anticipo
                                            </label>
                                            <input type="number" step="0.01" id="txtVarPercentualeAnticipoTerzisti" name="VarPercentualeAnticipo" class="form-control" onchange="controlloPercAnticipoModifica()" disabled>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="gasolioTerzistiLab" for="gasolioTerzisti">
                                                    Gasolio
                                                </label>
                                                <input type="text" id="gasolioTerzisti" name="gasolioTerzisti" class="form-control" onchange="controlloAnticipoGasolioModifica()" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-6" style="">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="benzinaTerzistiLab" for="benzinaTerzisti">
                                                    Benzina
                                                </label>
                                                <input type="text" id="benzinaTerzisti" name="benzinaTerzisti" class="form-control" onchange="controlloAnticipoBenzinaModifica()" disabled>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="gasolioSerraTerzistiLab" for="gasolioSerraTerzisti">
                                                    Gasolio Serra
                                                </label>
                                                <input type="text" id="gasolioSerraTerzisti" name="gasolioSerraTerzisti" class="form-control" onchange="controlloAnticipoGasolioSerraModifica()" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>

                    <div class="panel panel-default padding-top" id="panel-carburantiTerzisti-Approvato" style="margin-top: 20px; display: none;">
                        <div class="panel-heading-richiesta">
                            <h5><b>Richiesta iniziale approvata</b></h5>
                        </div>

                        <div class="panel-body" style="background-color: #c3e3e3">

                            <div class="row">

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="gasolioTerzistiApproLab" for="gasolioTerzistiAppro">
                                                    Gasolio
                                                </label>
                                                <input type="text" id="gasolioTerzistiAppro" name="gasolioTerzistiAppro" class="form-control" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-6" style="">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="benzinaTerzistiApproLab" for="benzinaTerzistiAppro">
                                                    Benzina
                                                </label>
                                                <input type="text" id="benzinaTerzistiAppro" name="benzinaTerzistiAppro" class="form-control" disabled>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="gasolioSerraTerzistiApproLab" for="gasolioSerraTerzistiAppro">
                                                    Gasolio Serra
                                                </label>
                                                <input type="text" id="gasolioSerraTerzistiAppro" name="gasolioSerraTerzistiAppro" class="form-control" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>

                    <div class="panel panel-default padding-top" id="panel-anticipazioni-colturali" style="margin-top: 20px;">
                        <div class="panel-heading">
                            <h5 id="lbl_anticipazioni_colturali">Anticipazioni colturali da terzisti anno precedente</h5>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-3 col-md-3 col-sm-3">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblAnticipazioni_Gasolio" for="TxtAnticipazioni_Gasolio">
                                                    Gasolio
                                                </label>
                                                <input type="text" id="TxtAnticipazioni_Gasolio" name="TxtAnticipazioni_Gasolio" class="form-control" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3 col-md-3 col-sm-3">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblAnticipazioni_Benzina" for="TxtAnticipazioni_Benzina">
                                                    Benzina
                                                </label>
                                                <input type="text" id="TxtAnticipazioni_Benzina" name="TxtAnticipazioni_Benzina" class="form-control" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3 col-md-3 col-sm-3">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="lblAnticipazioni_Gasolio_Serra" for="TxtAnticipazioni_Gasolio_Serra">
                                                    Gasolio Serra
                                                </label>
                                                <input type="text" id="TxtAnticipazioni_Gasolio_Serra" name="TxtAnticipazioni_Gasolio_Serra" class="form-control" disabled>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3 col-md-3 col-sm-3">
                                    <div class="btn btn-success" id="btn_anticipazioniTerzisti" >
                                        <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Dettaglio anticipazioni colturali</span>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>

                    <div class="panel panel-default padding-top" id="panel-permesso-acqua" style="margin-top: 20px;">
                        <div class="panel-heading-permessoAcqua" style="background-color: #c3e3e3">
                            <h5 id="title-permesso-acqua"><b>Permesso attingimento acqua</b></h5>
                        </div>

                        <div class="panel-body" style="background-color: #c3e3e3">

                            <div class="row">

                                <div class="col-lg-4 col-md-4 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="permessoAcquaLab" for="permessoAcqua">
                                                    Totale permesso acqua (mc)
                                                </label>
                                                <input type="text" id="permessoAcqua" name="permessoAcqua" class="form-control" >
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-8 col-md-8 col-sm-6" style="">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon control-label alert-info" id="notePermessoAcquaLab" for="notePermessoAcqua">
                                                    Note permesso acqua
                                                </label>
                                                <input type="text" id="notePermessoAcqua" name="notePermessoAcqua" class="form-control" >
                                            </div>
                                        </div>

                                    </div>
                                </div>
                        
                            </div>

                        </div>
                    </div>


                    <div class="panel panel-default padding-top" id="panel-info-calcolato" style="margin-top: 20px; display: none">
                        <div class="panel-heading">
                            <h5><b>Attenzione</b></h5>
                        </div>
                        <div class="panel-body">
                            <p id="info-calcolato-p"></p>
                        </div>
                    </div>

                </div>
                <!--<div class="row">
                    <div class="col-lg-4 col-md-4 col-sm-9">
                        <div class="btn btn-success" id="btn_salva">
                            <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
                        </div>
                    </div>
                </div>-->

            </div>
        </div>


        <div class="row" id="mainContainer">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">

                    <div class="jumbotron">
                        <div class="row align-items-center">
                            <div class="col-lg-4 col-md-3 col-sm-12">
                            </div>
                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="btn btn-success mt-3 mb-3 text-center btn_100" id="btn_AssegnaAutomaticamenteCarburante" onclick="AssegnaAutomaticamenteCarburante();" style="display: none; margin-bottom: 20px;">
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-3 col-sm-12">
                            </div>
                        </div>
                        <div class="row" id="legenda">
                            <div class="col-lg-12" style="padding-bottom: 15px">
                                <h5>I litri carburante calcolati e richiesti sono già stati decurtati a norma di legge</h5>
                            </div>
                        </div>
                        <div class="row" id="warning_appezzamenti" style="display: none" >
                            <div class="col-lg-12" style="padding-bottom: 15px">
                                <div class="row" id="warning_appezzamenti_message" style="padding-bottom: 5px" >
                                </div>
                                <button type="button" class="btn btn-warning" id="btnCorreggiSuperfici" style="display: none" onclick="CorreggiSuperficiAppezzamenti_Click()">Allinea Tutti</button>
                            </div>
                        </div>
                        <div class="container" style="width: 100%; padding: 0">
                            <div class="container_dettagli" style="padding: 0; /*margin-bottom: 70px*/">
                                <div id="tabstrip_dettagli">
                                    <ul style="text-transform: uppercase;">
                                        <li class="Ric k-state-active k-active" id="tab1">Richiesta per Gruppo Colturale / Lavorazione
                                        </li>
                                        <li class="Terz" id="tab2">Richiesta per Gruppo Colturale / Lavorazione
                                        </li>
                                        <li class="TerzParz" id="tab6">Inserimento Lavorazioni
                                        </li>
                                        <li class="Doc" id="tab3">Documenti
                                        </li>
                                        <li class="Mac" id="tab4">Macchine
                                        </li>
                                        <li class="All" id="tab5">Allevamenti
                                        </li>
                                        <li class="Rim" id="tab7">Gestione rimanenze 
                                        </li>
                                        <!--<li class="Riep">
                                            <asp:Localize Text="Riepilogo" runat="server">Riepilogo</asp:Localize>
                                        </li>-->
                                        <li class="Verb">Note Chiusura Verbale Istruttoria
                                        </li>
                                    </ul>

                                    <%--TAB richieste--%>
                                    <div class="panel-group dettagliImpianti" style="display: none;">
                                        <div class="row">
                                            <div id="legendaNote" style="display: none;">
                                                <div class="col-lg-12" style="padding-bottom: 15px">
                                                    <h5>Legenda</h5>
                                                </div>
                                                <div class="col-lg-1" style="width: 10px; padding-right: 0px;">
                                                    <div style="width: 10px; height: 10px; background-color: #e3c668">
                                                    </div>
                                                </div>
                                                <div class="col-lg-10">
                                                    <h5 id="legendaPerc">Note di compilazione obbligatorie</h5>
                                                </div>
                                                </div>
                                        </div>

                                        <div class="row">
                                            <!--Griglia-->
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="tab_griglia_dettagliImpianti"></div>
                                            </div>


                                        </div>
                                    </div>

                                    <div class="panel-group terzisti" style="display: none;">
                                        <div class="row">
                                            <UCTerzisti:UMA_Terzisti ID="Div_UMA_Terzisti1" runat="server" />
                                        </div>
                                    </div>

                                    <div class="panel-group terzistilavparziali" style="display: none;">
                                        <div class="row">
                                            <UCTerzistiParz:UMA_TerzistiParziale ID="Div_UMA_TerzistiParziale" runat="server" />

                                        </div>
                                    </div>

                                    <div class="panel-group documenti" style="display: none;">

                                        <div class="row">

                                            <div class="btn btn-success" id="btn_carica_allegati" style="display: none">
                                                <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Elenco Documenti
                                                </span>
                                            </div>

                                            <div class="btn btn-success" id="btn_nuovo_allegato" style="display: none">
                                                <span class="fa fa-file-o lampeggiante"></span><span class="lampeggiante">Nuovo Documento</span>
                                            </div>

                                            <UCDocumenti:UMA_Documenti ID="Div_UMA_Documenti" runat="server" />

                                        </div>

                                        <div class="row">

                                            <!--Griglia-->
                                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                <div id="tab_griglia_documenti"></div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="panel-group macchine">
                                        <div class="row">

                                            <div class="btn btn-success" id="btn_aggiungi_macchina" style="display: none;">
                                                <span class="lampeggiante"></span><span class="lampeggiante">Gestione Macchine
                                                </span>
                                            </div>

                                            <UCMacchine:UMA_Macchine ID="Div_UMA_Macchine" runat="server" />
                                        </div>
                                    </div>

                                    <div class="panel-group allevamenti" style="display: none;">
                                        <div class="row">
                                            <UCAllevamenti:UMA_Allevamenti ID="Div_UMA_Allevamenti" runat="server" />
                                        </div>
                                        <div class="row">
                                            <UCAllevamentiUF:UMA_Allevamenti_UF ID="UMA_Allevamenti_UF" runat="server" />
                                        </div>
                                    </div>


                                    <div class="panel-group Gestione-rimanenze">
                                        <div class="row">
                                            <div class="" id="panel-rimanenaze-anno-precbot" style="margin-top: 10px; text-align: center">
                                                <div class="">
                                                    <div class="btn btn-success" id="btn_ValorizzaConferma" style="display: none">
                                                        </span><span class="lampeggiante">CONFERMA CARBURANTE TRASFERITO / RESTITUITO / RIASSEGNABILE / RECUPERO ACCISE</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="panel panel-default padding-top" id="panel-rimanenaze-anno-prec" style="margin-top: 20px;">
                                                <div class="panel-heading">
                                                    <h5 id="lbl_rimanenzea_anno_prec">Carburante non utilizzato dichiarato</h5>
                                                </div>
                                                <div class="panel-body">

                                                    <div class="row">

                                                        <div class="col-lg-4 col-md-4 col-sm-9">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblNonUtilizzatoGasolio" for="TxtNonUtilizzatoGasolio">
                                                                            Gasolio
                                                                        </label>
                                                                        <input type="text" id="TxtNonUtilizzatoGasolio" name="TxtNonUtilizzatoGasolio" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4 col-md-4 col-sm-9">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblNonUtilizzatoBenzina" for="TxtNonUtilizzatoBenzina">
                                                                            Benzina
                                                                        </label>
                                                                        <input type="text" id="TxtNonUtilizzatoBenzina" name="TxtNonUtilizzatoBenzina" class="form-control">
                                                                    </div>
                                                                    <script src="configurazioneUMA_ws_client.js"></script>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3 col-md-3 col-sm-9">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblNonUtilizzatoGasolio_Serra" for="TxtNonUtilizzatoGasolio_Serra">
                                                                            Gasolio Serra
                                                                        </label>
                                                                        <input type="text" id="TxtNonUtilizzatoGasolio_Serra" name="TxtNonUtilizzatoGasolio_Serra" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-lg-11 col-md-11 col-sm-11">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblCausali del non utilizzo" for="TxtRimanenza_Gasolio_Serra_prec">
                                                                            Causali del non utilizzo
                                                                        </label>
                                                                        <%--<input type="text" id="DdlCausalidelnonutilizzo" name="DdlCausalidelnonutilizzo" class="kendoDropDownList" >--%>
                                                                        <%--<input type="text" id="ddlServizi" class="kendoDropDownList" />--%>
                                                                        <select name="DdlCausalidelnonutilizzo" multiple="multiple" id="DdlCausalidelnonutilizzo" class="form-control"></select>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="panel panel-default padding-top" id="panel-Trasferimenti" style="margin-top: 20px;">
                                                <div class="panel-heading">
                                                    <h5 id="lbl_trasferimenti">Di cui trasferimenti</h5>
                                                    <h5 id="lblsubTitolotraferimenti"></h5>
                                                  
                                                </div>
                                                <div class="panel-body">

                                                    <div class="row">
                                                        <div id="grdTrasferimenti"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="panel panel-default padding-top" id="panel-restituzioni" style="margin-top: 20px;">
                                                <div class="panel-heading">
                                                    <h5 id="lbl_restituzioni">Di cui restituzioni al distributore</h5>
                                                </div>
                                                <div class="panel-body">

                                                    <div class="row">
                                                        <div id="grdRestituzioni"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="panel panel-default padding-top" id="panel-rimanenaze-riassegnabili-anno-prec" name="panel-rimanenaze-riassegnabili-anno-prec" style="margin-top: 20px;">
                                                <div class="panel-heading">
                                                    <h5 id="lbl_rimanenzea_riassegnabili_anno_prec">Di cui rimanenze riassegnabili</h5>
                                                </div>
                                                <div class="panel-body">

                                                    <div class="row">

                                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblriassegnabiliGasolio" for="TxtriassegnabiliGasolio">
                                                                            Gasolio
                                                                        </label>
                                                                        <input type="text" id="TxtriassegnabiliGasolio" name="TxtriassegnabiliGasolio" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblriassegnabiliBenzina" for="TxtriassegnabiliBenzina">
                                                                            Benzina
                                                                        </label>
                                                                        <input type="text" id="TxtriassegnabiliBenzina" name="TxtriassegnabiliBenzina" class="form-control">
                                                                    </div>
                                                                    <script src="configurazioneUMA_ws_client.js"></script>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblriassegnabiliGasolio_Serra" for="TxtriassegnabiliGasolio_Serra">
                                                                            Gasolio Serra
                                                                        </label>
                                                                        <input type="text" id="TxtriassegnabiliGasolio_Serra" name="TxtriassegnabiliGasolio_Serra" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblriassegnabiliGasolioConferma" for="TxtriassegnabiliGasolioConferma">
                                                                            Conferma Gasolio
                                                                        </label>
                                                                        <input type="text" id="TxtriassegnabiliGasolioConferma" name="TxtriassegnabiliGasolioConferma" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblriassegnabiliBenzinaConferma" for="TxtriassegnabiliBenzinaConferma">
                                                                            Conferma Benzina
                                                                        </label>
                                                                        <input type="text" id="TxtriassegnabiliBenzinaConferma" name="TxtriassegnabiliBenzinaConferma" class="form-control">
                                                                    </div>
                                                                    <script src="configurazioneUMA_ws_client.js"></script>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblriassegnabiliGasolio_SerraConferma" for="TxtriassegnabiliGasolio_SerraConferma">
                                                                            Conferma Gasolio Serra
                                                                        </label>
                                                                        <input type="text" id="TxtriassegnabiliGasolio_SerraConferma" name="TxtriassegnabiliGasolio_SerraConferma" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="panel panel-default padding-top" id="panel-recuperoaccise" style="margin-top: 20px;">
                                                <div class="panel-heading">
                                                    <h5 id="lbl_recuperoaccise">Di cui rimanenze a recupero accise</h5>
                                                </div>
                                                <div class="panel-body">

                                                    <div class="row">

                                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblrecuperoacciseGasolio" for="TxtrecuperoacciseGasolio">
                                                                            Gasolio
                                                                        </label>
                                                                        <input type="text" id="TxtrecuperoacciseGasolio" name="TxtrecuperoacciseGasolio" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblrecuperoacciseBenzina" for="TxtrecuperoacciseBenzina">
                                                                            Benzina
                                                                        </label>
                                                                        <input type="text" id="TxtrecuperoacciseBenzina" name="TxtrecuperoacciseBenzina" class="form-control">
                                                                    </div>
                                                                    <script src="configurazioneUMA_ws_client.js"></script>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblrecuperoacciseGasolio_Serra" for="TxtrecuperoacciseGasolio_Serra">
                                                                            Gasolio Serra
                                                                        </label>
                                                                        <input type="text" id="TxtrecuperoacciseGasolio_Serra" name="TxtrecuperoacciseGasolio_Serra" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblrecuperoacciseGasolioConferma" for="TxtrecuperoacciseGasolioConferma">
                                                                            Conferma Gasolio
                                                                        </label>
                                                                        <input type="text" id="TxtrecuperoacciseGasolioConferma" name="TxtrecuperoacciseGasolioConferma" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4 col-md-4 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblrecuperoacciseBenzinaConferma" for="TxtrecuperoacciseBenzinaConferma">
                                                                            Conferma Benzina
                                                                        </label>
                                                                        <input type="text" id="TxtrecuperoacciseBenzinaConferma" name="TxtrecuperoacciseBenzinaConferma" class="form-control">
                                                                    </div>
                                                                    <script src="configurazioneUMA_ws_client.js"></script>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <label class="input-group-addon control-label alert-info" id="lblrecuperoacciseGasolio_SerraConferma" for="TxtrecuperoacciseGasolio_SerraConferma">
                                                                            Conferma Gasolio Serra
                                                                        </label>
                                                                        <input type="text" id="TxtrecuperoacciseGasolio_SerraConferma" name="TxtrecuperoacciseGasolio_SerraConferma" class="form-control">
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel-group verbale-istruttoria" style="display: none;">
                                        <div class="row">
                                            <div class="col-lg-4 col-md-4 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <label class="myLabelBold" id="lblIstrModDati" for="ksIstrModDati">
                                                            L'istruttoria ha comportato la modifica dei dati dichiarati nella domanda con una riduzione degli stessi
                                                        </label>
                                                        <input type="text" id="ksIstrModDati" name="ksIstrModDati" aria-describedby="lblIstrModDati">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row boxIstrSegnMacchine">
                                            <div class="col-lg-5 col-md-5 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <label class="myLabelBold" id="lblIstrSegnMacchine" for="ksIstrSegnMacchine">
                                                            L'istruttoria ha comportato le segnalazioni di osservazioni rispetto al parco macchine dichiarato in domanda
                                                        </label>
                                                        <input type="text" id="ksIstrSegnMacchine" name="ksIstrSegnMacchine" aria-describedby="lblIstrSegnMacchine">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-3 col-md-4 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <label class="input-group-addon control-label alert-info" id="lblIstrEsito" for="ddlIstrEsito">
                                                                Esito
                                                            </label>
                                                            <input type="text" id="ddlIstrEsito" name="ddlIstrEsito" class="form-control" aria-describedby="lblIstrEsito">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 col-md-8 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <label class="input-group-addon control-label alert-info" id="lblIstrNote" for="txtAreaIstrNote">
                                                                Note Istruttoria
                                                            </label>
                                                            <textarea id="txtAreaIstrNote" name="txtAreaIstrNote" class="form-control" aria-describedby="lblIstrNote"></textarea>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-sm-2">
                                                <div class="btn btn-warning" id="btn_stampa_istruttoria">
                                                    <span class="fa fa-print lampeggiante"></span><span class="lampeggiante">Stampa Verbale</span>
                                                </div>
                                            </div>
                                        </div>
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
                        Chiudi
                    </button>
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
    <input type="hidden" id="hdTestUMA_Abilitato" runat="server" />
    <input type="hidden" id="hdTestUMA_Mese" runat="server" />
    <input type="hidden" id="hdUtenteAbilitatoMacchine" runat="server" />
    <input type="hidden" id="hdUtenteAbilitatoAnalisiTerreno" runat="server" />
    <input type="hidden" id="hdSuperUser" runat="server" />
    <input type="hidden" id="hcoordinatoreAfor" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">
        var username_master = "<%= Master.agroMasterPage_UtenteUsername %>";
        var prePiva = "<%=hdPiva.Value() %>";
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
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
        var cTestUMA_Abilitato = "#<%=hdTestUMA_Abilitato.ClientID() %>";
        var cTestUMA_Mese = "#<%=hdTestUMA_Mese.ClientID() %>";
        var cAbilitato_Macchine = "#<%=hdUtenteAbilitatoMacchine.ClientID() %>";
        var cAbilitato_AnalisiTerreno = "#<%=hdUtenteAbilitatoAnalisiTerreno.ClientID() %>";
        var hSuperUserUsername = "<%=hdSuperUser.Value() %>"
        var hCoordinatoreAfor = "<%=hcoordinatoreAfor.Value() %>"

        var QS_Piva = "<%= QS_Piva.ToString %>";
        var QS_Richiesta = <%=QS_Richiesta.ToString %>;
        var QS_Type = <%=QS_Type.ToString %>;
        var QS_Avanzamento = <%=QS_Avanzamento.ToString %>;
        var QS_Anticipo = <%=QS_Anticipo.ToString %>;
        var QS_TipoAzienda = <%=QS_TipoAzienda.ToString %>;
        var QS_TipoOp = <%=QS_TipoOp.ToString %>;
        var QS_PivaCod = <%=hdPiva_Codificata.ClientID()  %>;
        var QS_Programmazione_Cod_Fascicolo = "<%=QS_Programmazione_Cod_Fascicolo.ToString %>";
        var QS_UsaAnalisiTerrenoNG = <%=QS_UsaAnalisiTerrenoNG.ToString %>;
    </script>

    <script id="templateToolbarDettagliImpianti" type="text/x-kendo-template">
        
    </script>

    <script id="headerFascicoli" type="text/x-kendo-template">
        <div class="row k-widget k-header">
            <div class="col-lg-4 col-md-6 col-xs-6">
                <span class="validazione" >N. Validazione</span>
            </div>
            <div class="col-lg-1 col-md-6 col-xs-6">
                <span class="data">Data</span>
            </div>
            <div class="col-lg-2 col-md-6 col-xs-6">
                <span class="ente">Ente</span>
            </div>
            <div class="col-lg-4 col-md-6 col-xs-6">
                <span class="ente">Pianificazione</span>
            </div>
            <div class="col-lg-1 col-md-6 col-xs-6">
                <span class="ente">Op.</span>
            </div>
        </div>
    </script>
    <script id="valueFascicoli" type="text/x-kendo-template">
        <span class="selected-value ">#: (data.n_validazione) ? data.n_validazione : '' #</span>
        <span>#: (data.data_validazione) ? data.data_validazione : '' #</span>
        <span>#: (data.ente_validatore) ? data.ente_validatore : '' #</span>
        <span>#: (data.strProgrammazioniDes) ? data.strProgrammazioniDes: '' #</span>
    </script>
    <script id="templateFascicoli" type="text/x-kendo-template">
        <div class="row">
            <div class="col-lg-4 col-md-6 col-xs-6">
                <span class="validazione" >#:data.n_validazione#</span>
            </div>
            <div class="col-lg-1 col-md-6 col-xs-6">
                <span class="data">#:data.data_validazione#</span>
            </div>
            <div class="col-lg-2 col-md-6 col-xs-6">
                <span class="ente">#:data.ente_validatore#</span>
            </div>
        </div>
    </script>

    <div id="raccoglitore_Anticipo" style="display: none;">
        <%--onchange="ImpostaPercentualeAnticipo()"--%>
        <div id="Imposta_PercAnticipo">
            <div class="row" id="row_MsgErrore" style="display: none;">
                <%--<div class="col-lg-3 col-md-4 col-sm-12">--%>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="myLabelBold" id="lblMsgAliquotaSup" style="color: red">
                                <%--L' aliquota non può essere superiore all' Aliquota di default--%>
                            </label>
                        </div>
                    </div>
                </div>
                <%--</div>--%>
            </div>

            <div class="row" id="row_percentuale" style="margin-top: 10px">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group col-lg-4 col-md-12 col-sm-12">
                            <label class="input-group-addon control-label alert-info" id="lblPercentualeAnticipo">
                                Percentuale Anticipo
                            </label>
                            <input type="number" step="0.01" id="txtVarPercentualeAnticipo" name="VarPercentualeAnticipo" class="form-control" onchange="controlloPercAnticipoInserita()">
                        </div>
                    </div>
                </div>
            </div>

            <div class="panel panel-default padding-top" id="panel_Anticipo" style="margin-top: 20px; display: none;">
                <div class="panel-heading-richiesta" style="font-weight: 800;" id="lbl_RichiestaAnticipo">
                </div>

                <div class="panel-body" style="background-color: #c3e3e3">
                    <div class="row">
                        <div class="col-lg-3 col-md-4 col-sm-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="gasolioAnticipoLab" for="gasolioAnticipo">
                                            Gasolio
                                        </label>
                                        <input type="number" step="1" id="gasolioAnticipo" name="gasolioAnticipo" class="form-control" onchange="controlloAnticipoGasolioInserito()" disabled>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-4 col-sm-6" style="">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="benzinaAnticipoLab" for="benzinaAnticipo">
                                            Benzina
                                        </label>
                                        <input type="number" step="1" id="benzinaAnticipo" name="benzinaAnticipo" class="form-control" onchange="controlloAnticipoBenzinaInserito()" disabled>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-4 col-sm-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="gasolioSerraAnticipoLab" for="gasolioSerraAnticipo">
                                            Gasolio Serra
                                        </label>
                                        <input type="number" step="1" id="gasolioSerraAnticipo" name="gasolioSerraAnticipo" class="form-control" onchange="controlloAnticipoGasolioSerraInserito()" disabled>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


            <div class="row" style="margin-top: 10px">
                <div class="btn btn-success" id="btn_proseguiAnticipo">
                    <span class="fa fa-plus lampeggiante"></span>
                    <span class="lampeggiante" id="ricButt2" onclick="ImpostaPercentualeAnticipo()">Prosegui</span>
                </div>
            </div>
        </div>
    </div>

    <%--<input type="text" id="hdLavorazioniTerzistiDett" runat="server" />--%>
    <div id="griglia_dettagliTerzisti">
        <div class="row" style="margin-top: 10px">
            <div class="col-lg-12">
                <input type="hidden" id="LavorazioniTerzistiDett" />
                <div id="dettagliolav">
                </div>
            </div>
        </div>
    </div>

    <div id="griglia_dettagliAnticipazioni">
    <div class="row" style="margin-top: 10px">
        <div class="col-lg-12">
            <input type="hidden" id="AnticipazioniDett" />
            <div id="dettaglioAnt">
            </div>
        </div>
    </div>
</div>

    <div id="griglia_dettagliAppezzamenti">
        <div id="row_Fascicolo1" class="row">
            <div class="col-lg-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon control-label alert-info" id="lbl_fascicoloric">Fascicolo</label>
                            <input id="fascicoloric" type="text" name="fascicoloric" class="form-control">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row" style="margin-top: 10px">
            <div class="col-lg-7">
                <div class="btn btn-success" id="btn_cerca" onclick="MostraGrigliaDettaglioAppezzamenti()">
                    <span class="fa fa-search lampeggiante"></span>
                    <span class="lampeggiante" id="ricButt1">Cerca</span>
                </div>
            </div>
        </div>
        <div class="row" style="margin-top: 10px">
            <div class="col-lg-12">
                <input type="hidden" id="grigliadettaglioval" />
                <div id="dettaglio">
                </div>
            </div>
        </div>
    </div>


    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("richiestaCarburanti_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("richiestaCarburanti.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("richiestaCarburanti_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("richiestaCarburanti_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UMA_Macchine.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UMA_Macchine_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UMA_Terzisti.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UMA_Terzisti_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UMA_TerzistiParziale.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UMA_TerzistiParziale_ws_client.js") %>"></script>


</asp:Content>


