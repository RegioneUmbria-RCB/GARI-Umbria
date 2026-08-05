<%@ Page Title="Doc Contabile" Language="vb" AutoEventWireup="false" CodeBehind="DocContabile.aspx.vb"
    Inherits="AgroAgenda_2010.DocContabile" MasterPageFile="~/Master/AgendaBootstrap.Master" ValidateRequest="false" %>
<%@ Import Namespace="AgronicaCoreDataProvider" %>
<%@ Import Namespace="AgronicaControlli_2010" %>
<%@ Register TagPrefix="uc1" TagName="DocContabileDettagliUC" Src="./DocContabileDettagliUC.ascx" %>
<%@ Register TagPrefix="uc2" TagName="BeniconfezionamentoUC" Src="../GestioneMagazzini/BeniconfezionamentoUC.ascx" %>
<%@ Register TagPrefix="uc2" TagName="CastellettoUC" Src="../GestioneContabilita/Fatturazione/CastellettoUC.ascx" %>
<%@ Register TagPrefix="uc3" TagName="RiepilogoPesiUC" Src="./UserControl/RiepilogoPesiUC.ascx"  %>
<%@ Register TagPrefix="uc4" TagName="RifCatastaliUC" Src="../GestioneContabilita/ContrattiAffitto/RifCatastaliUC.ascx" %> <%-- i18n --%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

<style type="text/css">

    /*hide validation message*/
    .k-tooltip-validation {
        visibility: hidden !important;
    }

    .k-form-error.k-invalid-msg {
        display: block;
    }

    .k-widget.form-control {
        white-space:initial;
    }

    /*add red border*/
    /*.k-widget > span.k-invalid,
    input.k-invalid
    {
        border: 1px solid red !important;
    }*/

    .errorClass {
        border: 2px solid #D41E1A !important;
    }
     
    
    .errorMessages span {
        font-weight: bold;
    }

    #numDocTxtRow > div:first-child, #numDocDDLRow > div:first-child, #rowNumDocSin2 > div:first-child {
        padding-left:0;
    }

    .blockModifica, .blockCancella, .blockDuplica {
        /* display: block; con questo non è possibile ridimensionare la colonna dei pulsanti */
        margin-top: 10px !important;
        margin-bottom: 10px !important;
    }
    
    /*.k-grid  .k-grid-header  .k-header  .k-link {
        height: auto;
    }
  
    .k-grid  .k-grid-header  .k-header {
        white-space: normal;
    }

    .k-grid-header .k-header {
        overflow: visible;
        white-space: normal;
    }*/

    /*.breakWordDocCont {
        word-break: break-all !important;
        word-wrap: break-word !important;
        vertical-align: top;
    }

    .k-grid-header .k-header {
        overflow: visible !important;
        white-space: normal !important;
    }*/

    .fixed-header {
        top:0;
        position:fixed;
        width:auto;
        z-index: 1;
    }


    .k-panelbar>.k-panelbar-header>.k-link:hover {
        background-color: #052747;
    }

    .k-panelbar>.k-panelbar-header>.k-link.k-state-selected:hover,
    .k-panelbar>.k-panelbar-header>.k-link.k-selected:hover {
         background-color: #052747;
    }
    
    .k-panelbar>.k-item>.k-link.k-state-selected, 
    .k-panelbar>.k-item>.k-link.k-selected, 
    .k-panelbar>.k-panelbar-header>.k-link {
        background-color: #052747;
    }

    .k-panelbar .k-header {
        /*background-color: #428bca;*/
        background-color: #052747;
    }
    
    .k-panelbar>.k-item>.k-link {
        color: white !important;
        /*text-decoration: underline !important;*/
    }
   

    #tabDettagliDoc .k-grid tbody .k-button {
        -moz-min-width: 20px;
        -ms-min-width: 20px;
        -o-min-width: 20px;
        -webkit-min-width: 20px;
        min-width: 20px;
        width: 45px;
    }
    
    #btn_PulsanteNuovaRiga
    {
        float: left;
    }

    /* per utilizzo SOLO con kendoWindow, altrimenti non serve*/
     /* reset everything to the default box model */

      *, :before, :after
      {
        -webkit-box-sizing: content-box;
        -moz-box-sizing: content-box;
        box-sizing: content-box;
      }

      /* set a border-box model only to elements that need it */

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


    .ob-no-scroll { 
        overflow: hidden;
    }

    .bd-callout {
        padding: 1rem;
        margin-top: 1rem;
        margin-bottom: 1rem;
        border: 1px solid #e0e0e0;
        border-left-width: 0.5rem;
        border-right-width: 0.5rem;
        border-radius: 0.75rem;
    }

    .bd-callout h4, .bd-callout h5 {
        margin-top: 0;
        margin-bottom: .25rem;
    }

    .bd-callout p:last-child {
        margin-bottom: 0;
    }

    .bd-callout code {
        border-radius: .25rem;
    }

    .bd-callout + .bd-callout {
        margin-top: -.25rem;
    }

    .bd-callout-info {
        border-left-color: #5bc0de;
        border-right-color: #5bc0de;
    }

    .bd-callout-info h4, .bd-callout-info h5 {
        color: #5bc0de;
    }

    .bd-callout-warning {
        border-left-color: #f0ad4e;
        border-right-color: #f0ad4e;
    }

    .bd-callout-warning h4, .bd-callout-warning h5 {
        color: #f0ad4e;
    }

    .bd-callout-danger {
        border-left-color: #d9534f;
        border-right-color: #d9534f;
    }

    .bd-callout-danger h4, .bd-callout-danger h5 {
        color: #d9534f;
    }

    .bd-callout-primary {
        border-left-color: #052747;
        border-right-color: #052747;
    }

    .bd-callout-primary h4, .bd-callout-primary h5 {
        color: #052747;
    }


    .fieldlist {
        margin: 0 0 -1em;
        padding: 0;
    }

    .fieldlist li {
        list-style: none;
        padding-bottom: 1em;
    }

    .k-panelbar .k-content, .k-panelbar .k-panel {
        border-bottom: none !important;
    }

    .k-panelbar .first-row {
        margin-top: 10px;
    }

    .tab-pane .first-row {
        margin-top: 10px;
    }

    .k-card.k-state-pers1 {
        border-color: #052747;
        color: #214665;
        background-color: #fcfcfc
    }

    .k-card.k-state-pers2 {
        border-color: #af4c0a;
        color: #5b0201;
        background-color: #ffffff;
    }

    .k-card.k-state-pers3 {
        border-color: #6e2515;
        color: #803220;
        background-color: #fbf5f4 /*#fef5f3*/;
    }

    .btnFiltroNonAttivo {
        background-color: lightgrey;
        border-color: lightgrey;
    }
    
    .btnFiltroNonAttivo[disabled] {
        background-color: lightgrey;
        border-color: lightgrey;
    }

    .k-content-frame {
        border: 0;
        width: 100%;
        height: 100%;
    }

</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestionePrezziLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestionePrezziScrittura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestioneNuovoAllegato" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestioneVisualizaAllegato" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestioneGHG" />
    <asp:HiddenField runat="server" ID="hf_utenteAbilitatoISCCNonConforme" />
    <asp:HiddenField runat="server" ID="hf_utenteAbilitatoProdottiScrittura" />
    <asp:HiddenField runat="server" ID="hf_utenteAbilitatoContattiScrittura" />


	<div class="container-fluid"> <!--style="overflow: auto; margin-top: 10px; margin-bottom: 70px;"-->
	    <!--<div id="cover" ></div>
        <div id="loader" class="loader"></div>
        -->

		<div id="pippo" class="panel-group"> <!--style="display:none;">-->
		    <!--EDIT-->
            <div id="containerDocContabile" class="container">
            
                <div id="intestazione" class="tab-pane kendoValidatorTestata" style="display: block; overflow: auto; margin-bottom: 20px;">

                    <div class="row first-row">
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            
                            <div class="k-card-list">
                                <div class="k-card k-state-pers1" id="cardDocPrincipale" style="margin-bottom: 10px;">
                                    <div class="k-card-body">
                                        <!--<h5 class="k-card-title">Documento Principale</h5>-->
                                        <!--<h6 class="k-card-subtitle">Card Subtitle</h6>-->
                                        
                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="input-group" id="inNumDocGroup">
                                                    <label class="input-group-addon" id="lbl_des_num_doc" for="inNumDoc">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroDocumento %>" runat="server">Numero Documento</asp:Localize>:
                                                    </label>
                                                    <div class="row" id="numDocTxtRow">
                                                        <div class="col-lg-3 col-md-3 col-sm-12">
                                                            <input name="inNumDocSin" id="inNumDocSin" class="form-control k-content text-uppercase" 
                                                                title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Prefisso %>' runat='server'></asp:Localize>" 
                                                                onchange="inNumDocSin_change()"/>
                                                        </div>
                                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                                            <div class="input-group">
                                                                <input name="inNumDoc" id="inNumDoc" class="form-control k-content" required/>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3 col-md-3 col-sm-12">
                                                            <input name="inNumDocDes" id="inNumDocDes" class="form-control k-content text-uppercase" 
                                                                title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Suffisso %>' runat='server'></asp:Localize>" 
                                                                onchange="inNumDocDes_change()"/>
                                                        </div>
                                                        <div class="col-lg-2 col-md-2 col-sm-12">
                                                            <input name="inNumDocLock" id="inNumDocLock" type="checkbox" class="kendoSwitch"/>
                                                        </div>
                                                    </div>
                                                    <div class="row" id="numDocDDLRow">
                                                        <div class="col-lg-8 col-md-8 col-sm-12">
                                                            <input name="inNumDocDDL" id="inNumDocDDL" class="form-control k-content"/>
                                                        </div>
                                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                                            <input name="inNumDocShow" id="inNumDocShow" class="form-control k-content" 
                                                                title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, VisualizzaNumDocCompleto %>' runat='server'></asp:Localize>" 
                                                                disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-4 col-md-4 col-sm-12">
                                                <div class="input-group" id="groupDataEmissione">
                                                    <label class="input-group-addon" id="lbl_data_emissione" for="inDataEmissione">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataEmissione %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input id="inDataEmissione" name="inDataEmissione" class="kendoCalendarMM" type="date" style="width: 100%;" maxlength="10" required/>
                                                </div>
                                            </div>
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div id="groupDataLock" class="xi-form-action">
                                                    <input name="inDataLock" id="inDataLock" type="checkbox" class="kendoSwitch"/>
                                                    <div class="btn btn-success xi-btn-primary submit" id="btnSalvaDataDoc" onclick="btnSalvaDataDoc_click()" style="display: none;">
                                                        <i class="fa fa-floppy-o"></i>
                                                        <asp:Localize Text="<%$ Resources: SalvaData %>" runat="server">Salva Data</asp:Localize>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-7 col-md-7 col-sm-12">
                                                <div class="input-group" id="groupNumProtocollo">
                                                    <label class="input-group-addon" id="lbl_num_protocollo" for="inNumProtocollo">
                                                        <asp:Localize Text="<%$ Resources: NumProtocollo %>" runat="server"></asp:Localize>
                                                    </label>
                                                    <input name="inNumProtocollo" id="inNumProtocollo" class="form-control" disabled="disabled"/>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-7 col-md-7 col-sm-12">
                                                <div class="input-group" id="groupDataRegistrazione">
                                                    <label class="input-group-addon" id="lbl_data_registrazione" for="inDataRegistrazione">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataRegistrazioneAbbr  %>" runat="server">Data registr.</asp:Localize>:
                                                    </label>
                                                    <input id="inDataRegistrazione" name="inDataRegistrazione" class="kendoCalendar" type="date" style="width: 100%;" maxlength="10" required/>
                                                </div>
                                            </div>

                                            <div class="col-lg-5 col-md-5 col-sm-12">
                                                <div class="input-group" id="groupNumRegistrazione">
                                                    <label class="input-group-addon" id="lbl_num_registrazione" for="inNumRegistrazione">
                                                        <asp:Localize Text="<%$ Resources: NumRegistrazione %>" runat="server"></asp:Localize>:</label>
                                                    <input name="inNumRegistrazione" id="inNumRegistrazione" class="form-control k-input" disabled="disabled" required/>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                                <div class="k-card k-state-pers3" id="cardDocSecondario" style="margin-bottom: 10px; display: none;">
                                    <div class="k-card-body">
                                        <%--<h5 class="k-card-title">Documento Secondario</h5>--%>
                                        
                                        <div class="row" id="rowNumDocSecondario" style="display: none;">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="input-group">
                                                    <label class="input-group-addon" id="lbl_des_num_doc2" for="inNumDoc2">
                                                        <asp:Localize Text="<%$ Resources: NumeroDDT %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <div class="row" id="rowNumDocSin2">
                                                        <div class="col-lg-3 col-md-3 col-sm-12">
                                                            <input name="inNumDocSin2" id="inNumDocSin2" class="form-control k-content text-uppercase" 
                                                                title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Prefisso %>' runat='server'></asp:Localize>"/>
                                                        </div>
                                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                                            <div class="input-group">
                                                                <input name="inNumDoc2" id="inNumDoc2" class="form-control k-content"/>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3 col-md-3 col-sm-12">
                                                            <input name="inNumDocDes2" id="inNumDocDes2" class="form-control k-content text-uppercase"
                                                                title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Suffisso %>' runat='server'></asp:Localize>"/>
                                                        </div>
                                                        <%--<div class="col-lg-2 col-md-2 col-sm-12">
                                                            <button class="btn btn-warning" id="btnRefreshNumDoc2" onclick="btnRefreshNumDoc_click()" type="button" title="Calcolo numero del documento in base a prefisso, suffisso e data">
                                                                <i class="fa fa-bolt"></i>
                                                            </button>
                                                        </div>--%>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row" id="rowDataSecondaria" style="display: none;">
                                            <div class="col-lg-9 col-md-9 col-sm-12">
                                                <div class="input-group" id="groupDataEmissione2">
                                                    <label class="input-group-addon" id="lbl_data_emissione2" for="inDataEmissione2">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataEmissione %>" runat="server"></asp:Localize> 2:
                                                    </label>
                                                    <input id="inDataEmissione2" name="inDataEmissione2" class="kendoCalendar" type="date" style="width: 100%;" maxlength="10" required/>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            
                            <div class="k-card-list">
                                <div class="k-card k-state-pers1" id="cardContrattiAffitto" style="margin-bottom: 10px; display: none;">
                                    <div class="k-card-body">

                                        <div class="row">
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="input-group">
                                                    <label class="input-group-addon" id="lbl_data_iniz_val" for="inDataInizVal">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ValiditàDal %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input id="inDataInizVal" name="inDataInizVal" class="kendoCalendar" type="date" style="width: 100%;" maxlength="10"/>
                                                </div>
                                            </div>
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="input-group">
                                                    <label class="input-group-addon" id="lbl_data_fine_val" for="inDataFineVal">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ValiditàAl %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input id="inDataFineVal" name="inDataFineVal" class="kendoCalendar" type="date" style="width: 100%;" maxlength="10"/>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="input-group" id="groupCausaleAffitto">
                                                    <label class="input-group-addon" id="lbl_causale_affitto" for="inCausaleTrasportoAffitto">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Causale %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input name="inCausaleTrasportoAffitto" id="inCausaleTrasportoAffitto" class="form-control" />
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>

                            <div class="k-card-list">
                                <div class="k-card k-state-pers1" id="cardInfoTrasporto" style="margin-bottom: 10px;">
                                    <div class="k-card-body">

                                        <div class="row">
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="input-group"  id="checkboxacc">
                                                    <label class="input-group-addon" id="lblAccompagnatoria" for="chkAccompagnatoria">
                                                        <asp:Localize Text="<%$ Resources: FatturaAccompagnatoria %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input type="checkbox" name="chkAccompagnatoria" id="chkAccompagnatoria" class="kendoSwitch" />
                                                </div>
                                            </div>
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="input-group" id="boxChkProvvisorio">
                                                    <label class="input-group-addon" id="lblProvvisorio" for="chkProvvisorio">
                                                        <asp:Localize Text="<%$ Resources: DocumentoProvvisorio %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input type="checkbox" name="chkProvvisorio" id="chkProvvisorio" class="kendoSwitch" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="input-group" id="groupAspettoBeni">
                                                    <label class="input-group-addon" id="lbl_aspetto_beni" for="inAspettoBeni">
                                                        <asp:Localize Text="<%$ Resources: AspettoBeni %>" runat="server"></asp:Localize>:</label>
                                                    <input name="inAspettoBeni" id="inAspettoBeni" class="form-control"/>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="input-group" id="groupCausaleTrasporto">
                                                    <label class="input-group-addon" id="lbl_causale_trasporto" for="inCausaleTrasporto">
                                                        <asp:Localize Text="<%$ Resources: CausaleDocumento %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input name="inCausaleTrasporto" id="inCausaleTrasporto" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                        
                                        <div class="row">
                                            <div class="col-lg-5 col-md-5 col-sm-12">
                                                <div class="input-group" id="groupNumeroColli">
                                                    <label class="input-group-addon" id="lbl_numero_colli" for="inNumeroColli">
                                                        <asp:Localize Text="<%$ Resources: NumColli %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input name="inNumeroColli" id="inNumeroColli" class="form-control k-content"/>
                                                </div>
                                            </div>
                                            <div class="col-lg-7 col-md-7 col-sm-12" id="groupPesoTotale">
                                                <div class="col-lg-6 col-md-6">
                                                    <input name="inTipoPeso" id="inTipoPeso" class="form-control" disabled="disabled"/>
                                                </div>
                                                <div class="col-lg-6 col-md-6">
                                                    <input name="inPesoTotale" id="inPesoTotale" class="form-control k-content" disabled="disabled"/>
                                                </div>
                                            </div>
                                        </div>
                                        
                                        <div class="row">
                                            <div class="col-lg-8 col-md-8 col-sm-12">
                                                <div class="input-group" id="groupDataSpedizione">
                                                    <label class="input-group-addon" id="lbl_data_spedizione" for="inDataSpedizione">
                                                        <asp:Localize Text="<%$ Resources: DataSpedizione  %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input id="inDataSpedizione" name="inDataSpedizione" class="kendoCalendarTime" style="width: 100%;" MaxLength="16" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="input-group" id="groupNaturaBeni">
                                                    <label class="input-group-addon" id="lbl_natura_beni" for="inNaturaBeni">
                                                        <asp:Localize Text="<%$ Resources: NaturaDeiBeni %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input id="inNaturaBeni" name="inNaturaBeni" class="form-control k-content" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="input-group" id="groupCausaleContabilita" style="display: none;">
                                                    <label class="input-group-addon" id="lbl_causale_contabilita" for="inCausaleContabilita">
                                                        <asp:Localize Text="<%$ Resources: CausaleContabilita %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input id="inCausaleContabilita" name="inCausaleContabilita" class="form-control k-content" />
                                                </div>
                                            </div>
                                        </div>


                                         <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12">
                                                <div class="input-group" id="groupinTipoDocumento">
                                                    <label class="input-group-addon" id="lbl_tipologia" for="inTipoDocumento">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipoDocumento %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <input name="inTipoDocumento" id="inTipoDocumento" class="form-control" />
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                            
                            <div class="row" id="rowStatoOrdine" style="display: none;">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <div id="statoEvasioneOrdine" class="alert" role="alert" style="text-align: center; padding: 5px; height: 2.5em;">
                                        <h4 style="margin-bottom: 5px;">
                                            <strong><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StatoEvasione %>" runat="server"></asp:Localize>:</strong>
                                            <span id="spanStatoEvasioneOrdine"></span>
                                            <button class="btn <% If (Master.Master_versione <> "2022") Then %>btn-danger <% End If %> xonne-btn-primary" style="float: right;" id="btn_forzaEvasioneDocumento"
                                                    onclick="btnForzaEvasioneDocumento_click()" type="button" title="Forza Evasione Documento">
                                                <i class="fa fa-truck fa-lg"></i><asp:Localize Text="<%$ Resources: Evadi %>" runat="server"></asp:Localize>
                                            </button>
                                        </h4>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                    
                    <div class="row first-row">
                        <div class="<%If (Master.Master_versione = "2022") Then %>col-lg-4 col-md-4 col-sm-12<% Else %>col-lg-5 col-md-5 col-sm-12<% End If %>">
                            <div class="input-group" id="groupContatto1">
                                <label class="input-group-addon" id="lbl_cedente_cessionario_1" for="inCedenteCessionario1">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Cliente %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="inCedenteCessionario1" id="inCedenteCessionario1" class="form-control k-input" required/>
                                <div id="boxCompliantISCC" style="display:none;">
                                    <label for="reqCompliantISCC" style="display:none;">
                                        <asp:Localize Text="<%$ Resources: VerificaISCC %>" runat="server"></asp:Localize>
                                    </label>
                                    <input type="hidden" id="reqCompliantISCC" name="reqCompliantISCC" value="1" />
                                </div>

                                <div id="boxCompliantISCC_Contatto1" style="display:none;">
                                    <label for="reqCompliantISCC_Contatto1" style="display:none;">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Fornitore %>" runat="server"></asp:Localize>:
                                        <asp:Localize Text="<%$ Resources: VerificaISCC %>" runat="server"></asp:Localize>
                                    </label>
                                    <input type="hidden" id="reqCompliantISCC_Contatto1" name="reqCompliantISCC_Contatto1" value="1" />
                                </div>
                                <div id="boxCompliantISCC_Contatto2" style="display:none;">
                                    <label for="reqCompliantISCC_Contatto2" style="display:none;">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provenienza %>" runat="server"></asp:Localize>:
                                        <asp:Localize Text="<%$ Resources: VerificaISCC %>" runat="server"></asp:Localize>
                                    </label>
                                    <input type="hidden" id="reqCompliantISCC_Contatto2" name="reqCompliantISCC_Contatto2" value="1" />
                                </div>
                            </div>
                        </div>
                        <div class="<%If (Master.Master_versione = "2022") Then %>col-lg-2 col-md-2 col-sm-12<% Else %>col-lg-1 col-md-1 col-sm-12<%End If %>" style="display:block">
                            <div class="xi-form-action">
                                <button class="btn <% If (Master.Master_versione <> "2022") Then %>btn-danger <% End If %> xonne-btn-primary" id="btn_nuovo_cedente_cessionario_1" onclick="btnNuovoContatto_click(enum_tipologia_Contatto.cedente)" type="button" 
                                    title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, CreazioneNuovoContatto %>' runat='server'></asp:Localize>">
                                    <i class="fa fa-plus"></i>
                                </button>
                                 <button class="btn btn-warning xonne-btn-primary" id="btn_modifica_cedente_cessionario_1" onclick="btnModificaContatto_click(enum_tipologia_Contatto.cedente)" type="button" 
                                     title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, ModificaContatto %>' runat='server'></asp:Localize>">
                                    <i class="fa fa-edit"></i>
                                </button>
                                 <button type="button" class="btn btn-success gias-btn-square-filter" id="btn_cedente_cessionario_1_raccolte" style="display:none;">
                                    <i class="fa fa-filter"></i>
                                </button>
                                 <button type="button" class="btn btn-success gias-btn-square-filter" id="btn_cedente_cessionario_1_compliant_iscc" style="display:none;">
                                    <i class="fa fa-lock"></i>
                                </button>
                              </div>
                        </div>
                        
                        <div class="col-lg-3 col-md-3 col-sm-12">
                            <div class="input-group" id="groupPiva1">
                                <label class="input-group-addon" id="lbl_piva_cf_ced_ces_1" for="inPivaCfCC1">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PartitaIvaAbbr %>" runat="server"></asp:Localize> /
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceFiscaleSigla %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="inPivaCfCC1" id="inPivaCfCC1" class="form-control" disabled="disabled"/>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-3 col-sm-12">
                            <div class="input-group" id="groupProgressivo1">
                                <label class="input-group-addon" id="lbl_progressivo_ced_ces_1" for="inProgressivoCC1">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="inProgressivoCC1" id="inProgressivoCC1" class="form-control" disabled="disabled"/>
                            </div>
                        </div>

                        <div class="col-lg-5 col-md-5 col-sm-12">
                            <div class="input-group" id="groupAttivita1" style="display: none">
                                <label id="lbl_attivita_ced_ces_1" class="input-group-addon" for="inAttivitaCC1">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Attività %>" runat="server"></asp:Localize>:
                                </label>
                                <input id="inAttivitaCC1" class="form-control" disabled="disabled" name="inAttivitaCC1" />
                            </div>
                        </div>
                    </div>

                    <div class="row first-row">
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="input-group" id="groupAltriLocatori" style="display: none">
                                <label class="input-group-addon" id="lbl_altri_locatori" for="inAltriLocatori">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AltriLocatori %>" runat="server"></asp:Localize>:
                                </label>
                                <textarea name="inAltriLocatori" id="inAltriLocatori" class="form-control" rows="2" style="resize:none"></textarea>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="input-group" id="groupRifOrdini" style="display: none">
                                <label class="input-group-addon" id="lbl_rif_ordini" for="inRifOrdini">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RiferimentoOrdini %>" runat="server"></asp:Localize>:
                                </label>
                                <textarea name="inRifOrdini" id="inRifOrdini" class="form-control" rows="2" style="resize:none"></textarea>
                            </div>
                        </div>
                    </div>


                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-sm-12 col-lg-offset-3 col-lg-offset-3">
                            <!--<h4>&nbsp;</h4>-->
                            <div id="erroriMsgIntestazione" class="alert alert-danger" role="alert" style="display: none;">
                                <ul class="errorMessages"></ul>
                            </div>
                        </div>
                    </div>

                <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <h4>&nbsp;</h4>
                        </div>
                    </div>

                </div>
                                    
                <div class="row">
                    <div class="col-lg-4 col-md-4 col-sm-12">
                        <span id="msgBlocco"></span>
                    </div>
                    <div class="col-lg-8 col-md-8 col-sm-12 text-right">
                        <%-- Utilizzato solo per il salvataggio di testata e prima riga di un nuovo documento (non più usato)
                        <div class="btn btn-success submit" id="btn_SalvaRigaDoc" onclick="SalvaRigaDoc_click()"  style="display: none;">
                            <i class="fa fa-floppy-o"></i>Salva
                        </div>
                        --%>
                        <%-- Utilizzato solo per il salvataggio di testata e righe di un documento esistente --%>
                        <div class="btn btn-info submit text-uppercase" id="btnNuovoDocmmento" onclick="ApriKendoWindowAggiungiNuovoAllegato()" style="display: none;">
			                <i class="fa fa-paperclip fa-1x info_elem"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NuovoAllegato %>" runat="server"></asp:Localize>
		                </div>
                        <div class="btn btn-success submit text-uppercase" id="btnDocmmenti" onclick="btnDocmmenti_click()" style="display: none;">
			                <i class="fa fa-file-text-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneAllegati %>" runat="server"></asp:Localize>
		                </div>
                        <div class="btn btn-success submit xi-btn-primary" id="btnSalvaTestataPiuRiga" onclick="btnSalvaDoc_click(false,false,false)" style="display: none;">
                            <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server"></asp:Localize>
                        </div>
                        <div class="btn btn-success submit xi-btn-primary" id="btnSalvaEsciDoc" onclick="btnSalvaDoc_click(true,false,false)" style="display: none;">
                            <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server"></asp:Localize>
                        </div>
                         <div class="btn btn-success submit xi-btn-primary" id="btnSalvaENuovoDoc" onclick="btnSalvaDoc_click(false,false,true)" style="display: none;">
                            <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaENuovoDoc %>" runat="server"></asp:Localize>
                        </div>
                        <div class="btn btn-warning submit" id="btnSalvaStampa" onclick="btnSalvaDoc_click(false,true,false)" style="display: none;">
                            <i class="fa fa-print"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaStampa %>" runat="server"></asp:Localize>
                        </div>
                        <div class="btn btn-warning submit" id="btnStampaDoc" onclick="StampaDoc()"  style="display: none;"> 
			                <i class="fa fa-print"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StampaDocumentoAbbr %>" runat="server"></asp:Localize>
		                </div>
                        <div class="btn btn-danger" id="btnEsciDoc" onclick="Azione_Indietro_DocContabile()">
                            <i class="fa fa-reply"></i><span id="btnEsciDocTxt"><asp:Localize Text="<%$ Resources: EsciSenzaSalvare %>" runat="server"></asp:Localize></span>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                            <ul class="nav nav-tabs" role="tablist" id="tabs">
                                <li><a href="#tabTestataDoc" data-toggle="tab" id="a_tabTestataDoc">
                                    <asp:Localize Text="<%$ Resources: Testata %>" runat="server"></asp:Localize>
                                    </a></li>
                                <li><a href="#tabRiepilogoPesi" data-toggle="tab" id="a_tabRiepilogoPesi">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RiepilogoPesi %>" runat="server"></asp:Localize>
                                    </a></li>
                                <li><a href="#tabDettagliDoc" data-toggle="tab" id="a_tabDettagliDoc">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Righe %>" runat="server"></asp:Localize>
                                    </a></li>
                                <li><a href="#tabLayoutDoc" data-toggle="tab" id="a_tabLayoutDoc">Layout</a></li>
                                <li><a href="#tabBeniConfezionamento" data-toggle="tab" id="a_tabBeniConfezionamento">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Imballi %>" runat="server"></asp:Localize>
                                    </a></li>
                                <li><a href="#tabCastelletto" data-toggle="tab" id="a_tabCastelletto">
                                    <asp:Localize Text="<%$ Resources: CastellettoIva %>" runat="server"></asp:Localize>
                                    </a></li>
                                <!-- tab Dati Contratti Affitto -->
                                <li><a href="#tabRifCatastali" data-toggle="tab" id="a_tabRifCatastali" style="display: none">
                                    <asp:Localize Text="<%$ Resources: RifCatastali %>" runat="server"></asp:Localize>
                                    </a></li>
                            </ul>

                            <div class="tab-content">
                                <!-- tab Dati Intestazione -->
                                <div class="tab-pane kendoValidatorTestata fade in" id="tabTestataDoc" style="overflow: auto; margin-bottom: 50px;">
                                
                                    <div class="row first-row">
                                        <div class="col-lg-5 col-md-5 col-sm-12">
                                            <div class="input-group" id="groupCentroAziendale">
                                                <label class="input-group-addon" id="lbl_centro_aziendale" for="inCentroAziendale">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server"></asp:Localize>:
                                                </label>
                                                <input name="inCentroAziendale" id="inCentroAziendale" class="form-control" disabled/>
                                            </div>
                                        </div>
                                    </div>

                                    <div>
                                        <div class="col-lg-5 col-md-5 col-sm-12">
                                            <div class="input-group">
                                                <label class="input-group-addon" id="lbl_operatore" for="inOperatore" 
                                                    title="<asp:Localize Text='<%$ Resources: AutoreUltimaModifica %>' runat='server'></asp:Localize>">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Operatore %>" runat="server"></asp:Localize>:
                                                </label>
                                                <input name="inOperatore" id="inOperatore" class="form-control" 
                                                    title="<asp:Localize Text='<%$ Resources: AutoreUltimaModifica %>' runat='server'></asp:Localize>"/>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="input-group" id="groupTipologiaDocumento">
                                                <label class="input-group-addon" id="lbl_tipo_documento" for="inTipologiaDocumento">
                                                    <asp:Localize Text="<%$ Resources: TipologiaDocumento %>" runat="server"></asp:Localize>:
                                                </label>
                                                <input name="inTipologiaDocumento" id="inTipologiaDocumento" class="form-control"/>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12 col-lg-offset-3 col-lg-offset-3">
                                            <!--<h4>&nbsp;</h4>-->
                                            <div id="erroriMsgTabTestata" class="alert alert-danger" role="alert" style="display: none;">
                                                <ul class="errorMessages"></ul>
                                            </div>
                                        </div>
                                    </div>
                                    
                                    <div id="idFormPanelTestata" class="panel-group">
                                        <ul id="panelbarTestata" style="margin-top: 10px;">
                                            
                                            <!-- Informazioni Economiche -->
                                            <li class="k-state-active k-active" id="panelBarInfoEconomiche">
                                                <span id="panelBarTitleInfoEconomiche" class="k-link k-state-selected k-selected text-uppercase">
                                                    <asp:Localize Text="<%$ Resources: InformazioniEconomiche %>" runat="server"></asp:Localize>
                                                </span>

                                                <div class="row first-row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12" id="groupSezionale">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_sezionale" for="inSezionale">
                                                                <asp:Localize Text="<%$ Resources: Sezionale %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inSezionale" id="inSezionale" class="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12" id="groupModPagamento">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_mod_pagamento" for="inModPagamento">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ModalitàPagamento %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inModPagamento" id="inModPagamento" class="form-control" />
                                                        </div>
                                                    </div>
                                                </div>

                                            </li>

                                            <!-- Scadenza Ordine -->
                                            <li class="k-state-active k-active" id="panelBarScadenzaOrdine">
                                                <span id="panelBarTitleScadenzaOrdine" class="k-link k-state-selected k-selected text-uppercase">
                                                    <asp:Localize Text="<%$ Resources: ScadenzaOrdine %>" runat="server"></asp:Localize>
                                                </span>

                                                <div class="row first-row">
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupScadenzaUnica">
                                                            <label class="input-group-addon" id="lbl_scadenza_unica" for="inScadenzaUnica">
                                                                <asp:Localize Text="<%$ Resources: ScadenzaUnica %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inScadenzaUnica" id="inScadenzaUnica" type="checkbox" class="kendoSwitch"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                                        <div class="input-group" id="groupDataEvasionePrevista">
                                                            <label class="input-group-addon" id="lbl_data_evasione_prevista" for="inDataEvasionePrevista">
                                                                <asp:Localize Text="<%$ Resources: DataEvasionePrevista %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input id="inDataEvasionePrevista" name="inDataEvasionePrevista" class="kendoCalendar" type="date" style="width: 100%;" MaxLength="10"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2 col-md-2 col-sm-12">
                                                        <div class="input-group" id="groupEvasioneTassativa">
                                                            <label class="input-group-addon" id="lbl_evasione_tassativa" for="inEvasioneTassativa">
                                                                <asp:Localize Text="<%$ Resources: Tassativa %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inEvasioneTassativa" id="inEvasioneTassativa" type="checkbox" class="kendoSwitch"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                                        <div class="input-group" id="groupDataSpedizionePrevista">
                                                            <label class="input-group-addon" id="lbl_data_spedizione_prevista" for="inDataSpedizionePrevista">
                                                                <asp:Localize Text="<%$ Resources: DataSpedizionePrevista %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input id="inDataSpedizionePrevista" name="inDataSpedizionePrevista" class="kendoCalendarTime" style="width: 100%;" MaxLength="16" />
                                                        </div>
                                                    </div>

                                                </div>
                                            </li>

                                            <!-- Numerazione Ordine -->
                                            <li class="k-state-active k-active" id="panelBarNumerazioneOrdine">
                                                <span id="panelBarTitleNumerazioneOrdine" class="k-link k-state-selected k-selected text-uppercase">
                                                    <asp:Localize Text="<%$ Resources: NumerazioneOrdine %>" runat="server"></asp:Localize>
                                                </span>

                                                <div class="row first-row">
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupNOrdineCliente">
                                                            <label class="input-group-addon" id="lbl_n_ordine_cliente" for="inNOrdineCliente">
                                                                <asp:Localize Text="<%$ Resources: RifOrdineCliente %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input id="inNOrdineCliente" name="inNOrdineCliente" class="form-control" type="text"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupDataOrdineCliente">
                                                            <label class="input-group-addon" id="lbl_data_ordine_cliente" for="inDataOrdineCliente">
                                                                <asp:Localize Text="<%$ Resources: DataRif %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input id="inDataOrdineCliente" name="inDataOrdineCliente" class="kendoCalendar" type="date" style="width: 100%;" MaxLength="10"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupNOrdineConsorzio">
                                                            <label class="input-group-addon" id="lbl_n_ordine_consorzio" for="inNOrdineConsorzio">
                                                                <asp:Localize Text="<%$ Resources: NumeroOrdineConsorzio %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input id="inNOrdineConsorzio" name="inNOrdineConsorzio" class="form-control" type="text"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupAnnoOrdineConsorzio">
                                                            <label class="input-group-addon" id="lbl_anno_ordine_consorzio" for="inAnnoOrdineConsorzio">
                                                                <asp:Localize Text="<%$ Resources: AnnoOrdineConsorzio %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input id="inAnnoOrdineConsorzio" name="inAnnoOrdineConsorzio" class="" type="date" style="width: 100%;" maxlength="4"/>
                                                        </div>
                                                    </div>

                                                </div>
                                            </li>

                                            <!-- Cedente / Cessionario -->
                                            <li class="k-state-active k-active" id="panelBarIntestatario">
                                                <span id="panelBarTitleIntestatario" class="k-link k-state-selected k-selected text-uppercase">
                                                    <asp:Localize Text="<%$ Resources: IntestatarioDocumento %>" runat="server"></asp:Localize>
                                                </span>
                                                <div>
                                                    <div class="row first-row">
                                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_tipo_indirizzo_ced_ces_1" for="inTipoIndirizzoCC1">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipoIndirizzo %>" runat="server"></asp:Localize>:
                                                                </label>
                                                                <input name="inTipoIndirizzoCC1" id="inTipoIndirizzoCC1" class="form-control" required/>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_indirizzo_ced_ces_1" for="inIndirizzoCC1">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Indirizzo %>" runat="server"></asp:Localize>:
                                                                </label>
                                                                <input name="inIndirizzoCC1" id="inIndirizzoCC1" class="form-control" disabled="disabled"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_localita_ced_ces_1" for="inLocalitaCC1">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Localita %>" runat="server"></asp:Localize>:
                                                                </label>
                                                                <input name="inLocalitaCC1" id="inLocalitaCC1" class="form-control" disabled="disabled"/>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_comune_ced_ces_1" for="inComuneCC1">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Comune %>" runat="server"></asp:Localize>:
                                                                </label>
                                                                <input name="inComuneCC1" id="inComuneCC1" class="form-control" disabled="disabled"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_provincia_ced_ces_1" for="inProvinciaCC1">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provincia %>" runat="server"></asp:Localize>:
                                                                </label>
                                                                <input name="inProvinciaCC1" id="inProvinciaCC1" class="form-control" disabled="disabled"/>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-2 col-md-2 col-sm-12">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_cap_ced_ces_1" for="inCapCC1">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CAP %>" runat="server"></asp:Localize>:
                                                                </label>
                                                                <input name="inCapCC1" id="inCapCC1" class="form-control" disabled="disabled"/>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_stato_ced_ces_1" for="inStatoCC1">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Stato %>" runat="server"></asp:Localize>:
                                                                </label>
                                                                <input name="inStatoCC1" id="inStatoCC1" class="form-control" disabled="disabled"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </li>
                                        
                                            <!-- Cooperativa -->
                                            <li class="k-state-active k-active" id="panelBarCooperativa">
                                                <span class="k-link k-state-selected k-selected">
                                                    1° <asp:Localize Text="<%$ Resources: Cedente %>" runat="server"></asp:Localize>
                                                </span>
                                                
                                                <div class="row first-row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_cedente_cessionario_acc_3" for="inCedenteCessionarioAcc3">
                                                                1° <asp:Localize Text="<%$ Resources: Cedente %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inCedenteCessionarioAcc3" id="inCedenteCessionarioAcc3" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_piva_cf_ced_ces_3" for="inPivaCfCC3">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PartitaIvaAbbr %>" runat="server"></asp:Localize> /
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceFiscaleSigla %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inPivaCfCC3" id="inPivaCfCC3" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_progressivo_ced_ces_3" for="inProgressivoCC3">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inProgressivoCC3" id="inProgressivoCC3" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </li>
                                        
                                            <!-- 2° Cooperativa -->
                                            <li class="k-state-active k-active" id="panelBarCooperativa2">
                                                <span class="k-link k-state-selected k-selected">
                                                    2° <asp:Localize Text="<%$ Resources: Cedente %>" runat="server"></asp:Localize>
                                                </span>
                                                
                                                <div class="row first-row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_cedente_cessionario_acc_4" for="inCedenteCessionarioAcc4">
                                                                2° <asp:Localize Text="<%$ Resources: Cedente %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inCedenteCessionarioAcc4" id="inCedenteCessionarioAcc4" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_piva_cf_ced_ces_4" for="inPivaCfCC4">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PartitaIvaAbbr %>" runat="server"></asp:Localize> /
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceFiscaleSigla %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inPivaCfCC4" id="inPivaCfCC4" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_progressivo_ced_ces_4" for="inProgressivoCC4">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inProgressivoCC4" id="inProgressivoCC4" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </li>
                                            
                                            <!-- Cedente / Cessionario Diverso -->
                                            <li class="k-state-active k-active" id="panelBarDestinatario">
                                                <span id="panelBarTitleDestinatario" class="k-link k-state-selected k-selected">
                                                    <asp:Localize Text="<%$ Resources: DestinazioneMerce %>" runat="server"></asp:Localize>
                                                </span>

                                                <div class="row first-row">
                                                    <div class="col-lg-5 col-md-5 col-sm-12">
                                                        <div class="input-group" id="groupContatto2">
                                                            <label class="input-group-addon" id="lbl_cedente_cessionario_2" for="inCedenteCessionario2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Destinatario %>" runat="server"></asp:Localize>
                                                            </label>
                                                            <input name="inCedenteCessionario2" id="inCedenteCessionario2" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-1 col-md-1 col-sm-12">
                                                        <button type="button" class="btn gias-btn-square-filter btn-success" id="btn_cedente_cessionario_2_raccolte" style="display:none;">
                                                            <i class="fa fa-filter"></i>
                                                        </button>
                                                        <button type="button" class="btn gias-btn-square-filter btn-success" id="btn_cedente_cessionario_2_compliant_iscc" style="display:none;">
                                                            <i class="fa fa-lock"></i>
                                                        </button>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupPiva2">
                                                            <label class="input-group-addon" id="lbl_piva_cf_ced_ces_2" for="inPivaCfCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PartitaIvaAbbr %>" runat="server"></asp:Localize> /
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceFiscaleSigla %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inPivaCfCC2" id="inPivaCfCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupProgressivo2">
                                                            <label class="input-group-addon" id="lbl_progressivo_ced_ces_2" for="inProgressivoCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inProgressivoCC2" id="inProgressivoCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                    <div id="Attivita2" class="col-lg-5 col-md-5 col-sm-12">
                                                        <div class="input-group" id="groupAttivita2" style="display:none">
                                                            <label class="input-group-addon" id="lbl_attivita_ced_ces_2" for="inAttivitaCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Attività %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inAttivitaCC2" id="inAttivitaCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                
<%--                                                <div class="row" style="padding-left: 15px; padding-right: 15px;">
                                                    <div class="col-lg-6 col-md-6 col-sm-12 bd-callout bd-callout-primary">
                                                        <h5 id="lbl_indirizzo_destinazione">Indirizzo Destinazione merce</h5>
                                                    </div>
                                                </div>--%>

                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_tipo_indirizzo_ced_ces_2" for="inTipoIndirizzoCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipoIndirizzo %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inTipoIndirizzoCC2" id="inTipoIndirizzoCC2" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_indirizzo_ced_ces_2" for="inIndirizzoCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Indirizzo %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inIndirizzoCC2" id="inIndirizzoCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_localita_ced_ces_2" for="inLocalitaCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Localita %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inLocalitaCC2" id="inLocalitaCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_comune_ced_ces_2" for="inComuneCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Comune %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inComuneCC2" id="inComuneCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_provincia_ced_ces_2" for="inProvinciaCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provincia %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inProvinciaCC2" id="inProvinciaCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2 col-md-2 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_cap_ced_ces_2" for="inCapCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CAP %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inCapCC2" id="inCapCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_stato_ced_ces_2" for="inStatoCC2">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Stato %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inStatoCC2" id="inStatoCC2" class="form-control" disabled="disabled"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </li>
                                            
                                            <!-- Trasporto -->
                                            <li class="k-state-active k-active" id="panelBarDatiTrasporto">
                                                <span class="k-link k-state-selected k-selected text-uppercase">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Trasporto %>" runat="server"></asp:Localize>
                                                </span>
                                                
                                                <div class="row first-row">
                                                    <% If Master.Master_versione = "2022" Then %>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_trasporto_cura" for="inTrasportoCuraGroup">
                                                                <asp:Localize Text="<%$ Resources: TrasportoACuraDel %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <div class="input-group" id="inTrasportoCuraGroup">
                                                                <input id="inTrasportoCuraCedente" name="inTrasportoCura" value="0" class="k-radio" type="radio" checked="checked"/>
                                                                <label class="k-radio-label" for="inTrasportoCuraCedente" style="margin-right: 10px;">
                                                                    <asp:Localize Text="<%$ Resources: Cedente %>" runat="server"></asp:Localize>
                                                                </label>

                                                                <input id="inTrasportoCuraCessionario" name="inTrasportoCura" value="1" class="k-radio" type="radio"/>
                                                                <label class="k-radio-label" for="inTrasportoCuraCessionario" style="margin-right: 10px;">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Cessionario %>" runat="server"></asp:Localize>
                                                                </label>

                                                                <input id="inTrasportoCuraVettore" name="inTrasportoCura" value="2" class="k-radio" type="radio"/>
                                                                <label class="k-radio-label" for="inTrasportoCuraVettore" style="margin-right: 10px;">
                                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Vettore %>" runat="server"></asp:Localize>
                                                                </label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <% Else %>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <label class="input-group-addon" id="lbl_trasporto_cura" for="inTrasportoCuraGroup">
                                                            <asp:Localize Text="<%$ Resources: TrasportoACuraDel %>" runat="server"></asp:Localize>:
                                                        </label>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                    
                                                        <div class="input-group" id="inTrasportoCuraGroup">
                                                            <input id="inTrasportoCuraCedente" name="inTrasportoCura" value="0" class="k-radio" type="radio" checked="checked"/>
                                                            <label class="k-radio-label" for="inTrasportoCuraCedente" style="margin-right: 10px;">
                                                                <asp:Localize Text="<%$ Resources: Cedente %>" runat="server"></asp:Localize>
                                                            </label>

                                                            <input id="inTrasportoCuraCessionario" name="inTrasportoCura" value="1" class="k-radio" type="radio"/>
                                                            <label class="k-radio-label" for="inTrasportoCuraCessionario" style="margin-right: 10px;">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Cessionario %>" runat="server"></asp:Localize>
                                                            </label>

                                                            <input id="inTrasportoCuraVettore" name="inTrasportoCura" value="2" class="k-radio" type="radio"/>
                                                            <label class="k-radio-label" for="inTrasportoCuraVettore" style="margin-right: 10px;">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Vettore %>" runat="server"></asp:Localize>
                                                            </label>
                                                        </div>
                                                    </div>
                                                    <% End If %>
                                                    <div class="col-lg-6 col-md-5 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_gestione_vettore" for="inGestioneVettore">
                                                                <asp:Localize Text="<%$ Resources: Clausola %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inGestioneVettore" id="inGestioneVettore" class="form-control" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row xo-2022-contabile-d-flex">
                                                    <% If Master.Master_versione = "2022" Then %>
                                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                                    <% Else %>
                                                    <div class="col-lg-5 col-md-5 col-sm-12">
                                                    <% End If %>
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_vettore" for="inVettore">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Vettore %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inVettore" id="inVettore" class="form-control"/>
                                                        </div>
                                                    <% If Master.Master_versione = "2022" Then %>
                                                    </div>
                                                    <% Else %>
                                                    </div>
                                                    <% End If %>

                                                    <% If Master.Master_versione = "2022" Then %>
                                                    <div class="col-lg-2 col-md-2 col-sm-12 xo-2022-input-group" style="display:block">
                                                    <% Else %>
                                                    <div class="col-md-2 col-sm-12" style="display:block">
                                                    <% End If %>
                                                        <button class="btn <% If (Master.Master_versione <> "2022") Then %>btn-danger <% End If %> xonne-btn-primary" id="btn_nuovo_vettore" onclick="btnNuovoContatto_click(enum_tipologia_Contatto.vettore)" type="button"
                                                            title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, CreazioneNuovoContatto %>' runat='server'></asp:Localize>">
                                                            <i class="fa fa-plus"></i>
                                                        </button>
                                                         <button class="btn btn-warning xonne-btn-primary" id="btn_modifica_vettore" onclick="btnModificaContatto_click(enum_tipologia_Contatto.vettore)"
                                                             type="button" title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, ModificaContatto %>' runat='server'></asp:Localize>">
                                                            <i class="fa fa-edit"></i>
                                                        </button>
                                                    <% If Master.Master_versione = "2022" Then %>
                                                    </div>
                                                    <% Else %>
                                                    </div>
                                                    <% End If %>

                                                    <div class="col-md-5 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_dipendente" for="inDipendenti">
                                                                <asp:Localize Text="<%$ Resources: Dipendente %>" runat="server"></asp:Localize>
                                                            </label>
                                                            <input name="inDipendenti" id="inDipendenti" class="form-control" />
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_tipo_indirizzo_vettore" for="inTipoIndirizzoVettore">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipoIndirizzo %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inTipoIndirizzoVettore" id="inTipoIndirizzoVettore" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <div id="inDettaglioIndirizzoVettore" style="border: coral solid 4px; width: auto; height: auto;"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                                
                                                <div class="row" style="padding-left: 15px; padding-right: 15px;">
                                                    <div class="col-lg-12 col-md-12 col-sm-12 bd-callout bd-callout-danger">
                                                        <h5><asp:Localize Text="<%$ Resources: TrasportoAccise %>" runat="server">Info Trasporto x Accise</asp:Localize></h5>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_acc_modalita_trasporto" for="inAccModalitaTrasporto">
                                                                <asp:Localize Text="<%$ Resources: ModalitaTrasporto %>" runat="server">Modalità Trasporto</asp:Localize>:
                                                            </label>
                                                            <input name="inAccModalitaTrasporto" id="inAccModalitaTrasporto" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_acc_unita_trasporto" for="inAccUnitaTrasporto">
                                                                <asp:Localize Text="<%$ Resources: UnitaTrasporto %>" runat="server">Unità di Trasporto</asp:Localize>:
                                                            </label>
                                                            <input name="inAccUnitaTrasporto" id="inAccUnitaTrasporto" class="form-control"/>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row" style="padding-left: 15px; padding-right: 15px;">
                                                    <div class="col-lg-12 col-md-12 col-sm-12 bd-callout bd-callout-danger">
                                                        <h5><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MezzoTrasporto %>" runat="server">Mezzo Trasporto</asp:Localize></h5>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_mezzo_trasporto" for="inMezzoTrasporto">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Targa %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inMezzoTrasporto" id="inMezzoTrasporto" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-5 col-md-5 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_descrizione_mezzo" for="inDescrizioneMezzo">
                                                                <asp:Localize Text="<%$ Resources: DescrizioneMezzo %>" runat="server">Descrizione Mezzo</asp:Localize>:
                                                            </label>
                                                            <input name="inDescrizioneMezzo" id="inDescrizioneMezzo" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_peso_tara_trasporto" for="inPesoTaraTrasporto">
                                                                <asp:Localize Text="<%$ Resources: PesoTaraKg %>" runat="server">Peso Tara Kg</asp:Localize>:
                                                            </label>
                                                            <input name="inPesoTaraTrasporto" id="inPesoTaraTrasporto" class="form-control"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_n_immatricolazione_rimorchio" for="inNImmatricolazioneRimorchio">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroImmatricolazioneRimorchio %>" runat="server">
                                                                    N° Immatricolazione Rimorchio
                                                                </asp:Localize>:
                                                            </label>
                                                            <input name="inNImmatricolazioneRimorchio" id="inNImmatricolazioneRimorchio" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_n_autorizzazione_trasporto" for="inNAutorizzazioneTrasporto">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroAutorizzazioneTrasporto %>" runat="server">
                                                                    N° Autorizzazione Trasporto
                                                                </asp:Localize>:
                                                            </label>
                                                            <input name="inNAutorizzazioneTrasporto" id="inNAutorizzazioneTrasporto" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_data_autorizzazione_trasporto" for="inDataAutorizzazioneTrasporto">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataRilascioAutorizzazione %>" runat="server">
                                                                    Data Rilascio Autorizzazione
                                                                </asp:Localize>:
                                                            </label>
                                                            <input name="inDataAutorizzazioneTrasporto" id="inDataAutorizzazioneTrasporto" class="kendoCalendar" style="width: 100%;" MaxLength="10"/>
                                                        </div>
                                                    </div>
                                                </div>
                                                            
                                                <div class="row" style="padding-left: 15px; padding-right: 15px;">
                                                    <div class="col-lg-12 col-md-12 col-sm-12 bd-callout bd-callout-danger">
                                                        <h5><asp:Localize Text="<%$ Resources: DistanzaTrasporto %>" runat="server">Distanza Trasporto</asp:Localize></h5>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-3 col-md-4 col-sm-6">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lblUnitaMisuraTrasporto" for="ddlUnitaMisuraTrasporto">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaUdM %>" runat="server">
                                                                    Unità di misura
                                                                </asp:Localize>:
                                                            </label>
                                                            <input name="ddlUnitaMisuraTrasporto" id="ddlUnitaMisuraTrasporto" class="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 col-md-4 col-sm-6">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lblDistanzaTrasporto" for="ntbDistanzaTrasporto">
                                                                <asp:Localize Text="<%$ Resources: DistanzaTrasporto %>" runat="server">Distanza Trasporto</asp:Localize>:
                                                            </label>
                                                            <input name="ntbDistanzaTrasporto" id="ntbDistanzaTrasporto" class="form-control" />
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div id="groupDistanzaLock" class="xi-form-action">
                                                            <input name="inDistanzaLock" id="inDistanzaLock" type="checkbox" class="kendoSwitch"/>
                                                            <div class="btn btn-success xi-btn-primary submit" id="btnSalvaDistanzaDoc" onclick="btnSalvaDistanzaDoc_click()" style="display: none;">
                                                                <i class="fa fa-floppy-o"></i>
                                                                <asp:Localize Text="<%$ Resources: SalvaDistanza %>" runat="server">Salva Distanza</asp:Localize>
                                                            </div>
                                                        </div>
                                                    </div>


                                                </div>
                                            </li>
                                            
                                            <!-- Provvigioni Agente / Capo Area -->
                                            <li class="k-state-active k-active" id="panelBarProvvigioni">
                                                <span class="k-link k-state-selected k-selected text-uppercase">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provvigioni %>" runat="server"></asp:Localize>
                                                </span>

                                                <div id="rowAgenti" class="row first-row" >
                                                    <div class="col-lg-5 col-md-5 col-sm-12">
                                                        <div class="input-group" id="groupAgente">
                                                            <label class="input-group-addon" id="lbl_agente" for="inAgente">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Agente %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inAgente" id="inAgente" class="form-control"/>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-1 col-md-1 col-sm-12" style="display:block">
                                                        <div id="groupNuovoModificaAgente">
                                                            <button class="btn btn-danger xonne-btn-primary" id="btn_nuovo_agente" onclick="btnNuovoContatto_click(enum_tipologia_Contatto.agente)" type="button" 
                                                                title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, CreazioneNuovoContatto %>' runat='server'></asp:Localize>">
                                                                <i class="fa fa-plus"></i>
                                                            </button>
                                                             <button class="btn btn-warning " id="btn_modifica_agente" onclick="btnModificaContatto_click(enum_tipologia_Contatto.agente)" 
                                                                 type="button" title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, ModificaContatto %>' runat='server'></asp:Localize>">
                                                                <i class="fa fa-edit"></i>
                                                            </button>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupAgenteProvvigione">
                                                            <label class="input-group-addon" id="lbl_agente_provvigione" for="inAgenteProvvigione">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provvigione %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input name="inAgenteProvvigione" id="inAgenteProvvigione" class="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupAgenteProvvigionePerc">
                                                            <label class="input-group-addon" id="lbl_agente_provvigione_perc" for="inAgenteProvvigionePerc">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ProvvigionePercentuale %>" runat="server"></asp:Localize>
                                                            </label>
                                                            <input name="inAgenteProvvigionePerc" id="inAgenteProvvigionePerc" class="form-control" />
                                                        </div>
                                                    </div>
                                                </div>
                                                
                                                <div id="rowCapoArea" class="row first-row">
                                                    <div class="col-lg-5 col-md-5 col-sm-12">
                                                        <div class="input-group" id="groupCapoArea">
                                                            <label class="input-group-addon" id="lbl_capoarea" for="inCapoArea">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CapoArea %>" runat="server"></asp:Localize>
                                                            </label>
                                                            <input name="inCapoArea" id="inCapoArea" class="form-control"/>
                                                        </div>
                                                    </div>
                                                     <div class="col-lg-1 col-md-1 col-sm-12" style="display:block">
                                                        <div id="groupNuovoModificaCapoArea">
                                                            <button class="btn btn-danger" id="btn_nuovo_capoArea" onclick="btnNuovoContatto_click(enum_tipologia_Contatto.capoArea)" type="button" 
                                                                title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, CreazioneNuovoContatto %>' runat='server'></asp:Localize>">
                                                                <i class="fa fa-plus"></i>
                                                            </button>
                                                             <button class="btn btn-warning" id="btn_modifica_capoArea" onclick="btnModificaContatto_click(enum_tipologia_Contatto.capoArea)" type="button" 
                                                                 title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, ModificaContatto %>' runat='server'></asp:Localize>">
                                                                <i class="fa fa-edit"></i>
                                                            </button>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupCapoAreaProvvigione">
                                                            <label class="input-group-addon" id="lbl_capoarea_provvigione" for="inCapoAreaProvvigione">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ProvvigionePercentuale %>" runat="server"></asp:Localize>
                                                            </label>
                                                            <input name="inCapoAreaProvvigione" id="inCapoAreaProvvigione" class="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                                        <div class="input-group" id="groupCapoAreaProvvigionePerc">
                                                            <label class="input-group-addon" id="lbl_capoarea_provvigione_perc" for="inCapoAreaProvvigionePerc">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ProvvigionePercentuale %>" runat="server"></asp:Localize>
                                                            </label>
                                                            <input name="inCapoAreaProvvigionePerc" id="inCapoAreaProvvigionePerc" class="form-control" />
                                                        </div>
                                                    </div>
                                                </div>

                                            </li>

                                            <!-- Note -->
                                            <li class="k-state-active k-active" id="panelBarNoteDocumento">
                                                <span class="k-link k-state-selected k-selected text-uppercase">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server"></asp:Localize>
                                                </span>

                                                <div class="row first-row">
                                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_note_testata" for="inNoteTestata">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <textarea name="inNoteTestata" id="inNoteTestata" class="form-control" rows="2"></textarea>
                                                        </div>
                                                    </div>
                                                </div>
                                            </li>

                                            <!-- Allegati -->
                                            <li class="k-state-active k-active" id="panelBarAllegatiDocumento">
                                                <span class="k-link k-state-selected k-selected text-uppercase">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Riferimenti %>" runat="server"></asp:Localize>
                                                </span>

                                                <div class="row first-row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_doc_allegati" for="inDocAllegati">
                                                                <asp:Localize Text="<%$ Resources: DocumentiAssociati %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <textarea name="inDocAllegati" id="inDocAllegati" class="form-control" rows="3" disabled="disabled"></textarea>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="input-group">
                                                            <label class="input-group-addon" id="lbl_lav_associate" for="inLavAssociate">
                                                                <asp:Localize Text="<%$ Resources: LavorazioniAssociate %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <textarea name="inLavAssociate" id="inLavAssociate" class="form-control" rows="3" disabled="disabled"></textarea>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div id="boxRifNotaCredito">
                                                    <div class="row">
                                                        <div class="col-md-3">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_n_nota_fattura" for="inNotaFattura">
                                                                    <asp:Localize Text="<%$ Resources: NumeroFatturaOriginariaNotaCredito %>" runat="server"></asp:Localize>
                                                                </label>
                                                                <input type="text" id="inNotaFattura" name="inNotaFattura" class="form-control" />
                                                            </div>
                                                        </div>
                                                        <div class="col-md-2">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_data_nota_fattura" for="inDataNotaFattura">
                                                                    <asp:Localize Text="<%$ Resources: DataFatturaOriginariaNotaCredito %>" runat="server"></asp:Localize>
                                                                </label>
                                                                <input type="date" id="inDataNotaFattura" name="inDataNotaFattura" class="kendoCalendar" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lblDDTResoSDI" for="inDDTResoSDI">
                                                                    <asp:Localize Text="<%$ Resources: NumDDTResoSDI %>" runat="server"></asp:Localize>
                                                                </label>
                                                                <input type="text" id="inDDTResoSDI" name="inDDTResoSDI" class="form-control" />
                                                            </div>
                                                        </div>
                                                        <div class="col-md-2">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lblDataDDTResoSDI" for="inDataDDTResoSDI">
                                                                    <asp:Localize Text="<%$ Resources: DataDDTResoSDI  %>" runat="server"></asp:Localize>
                                                                </label>
                                                                <input type="date" id="inDataDDTResoSDI" name="inDataDDTResoSDI" class="kendoCalendar" />
                                                            </div>
                                                        </div>
                                                        <div class="col-md-2">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lblRigaDDTResoSDI" for="inRigaDDTResoSDI">
                                                                    <asp:Localize Text="<%$ Resources: RigaDDTResoSDI %>" runat="server"></asp:Localize>
                                                                </label>
                                                                <input type="text" id="inRigaDDTResoSDI" name="inRigaDDTResoSDI" class="form-control" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                            </li>

                                        </ul>
                                    </div>

                                </div>
                                

                                <!-- tab Riepilogo pesi -->
                                <div class="tab-pane fade in" id="tabRiepilogoPesi" style="overflow: auto; margin-bottom: 50px;">
                                    <uc3:RiepilogoPesiUC id="RiepilogoPesiUC1" runat="server" />
                                </div>

                            
                                <!-- tab Dati Dettagli -->
                                <div class="tab-pane fade in" id="tabDettagliDoc" style="overflow: auto; margin-bottom: 50px;">
                                    <uc1:DocContabileDettagliUC id="DocContabileDettagliUC1" runat="server" />
                                </div>
                            

                                <!-- tab Layout -->
                                <div class="tab-pane fade in" id="tabLayoutDoc" style="overflow: auto; margin-bottom: 50px;">
                                    
                                    <!-- Formati Stampa -->
                                    <div class="row first-row" id="rowFormatiStampa">
                                        <div class="col-lg-8 col-md-8 col-sm-12">
                                            <div class="input-group" id="groupFormatiStampa">
                                                <label class="input-group-addon" id="lbl_formati_stampa" for="groupFormatiStampa">
                                                    <asp:Localize Text="<%$ Resources: FormatiStampa %>" runat="server">Formati di stampa</asp:Localize>:
                                                </label>
                                                <div id="divFormatiStampaGroup" style="margin-left: 1em;">
                                                    <div id="grpChkLayoutStandard">
                                                        <input id="inChkLayoutStandard" name="inFormatiStampa" value="0" class="k-radio" type="radio" checked="checked"/>
                                                        <label class="k-radio-label" for="inChkLayoutStandard">Standard</label>
                                                    </div>
                                                    <div id="grpChkLayoutPeso">
                                                        <input id="inChkLayoutPeso" name="inFormatiStampa" value="1" class="k-radio" type="radio"/>
                                                        <label class="k-radio-label" for="inChkLayoutPeso">
                                                            <asp:Localize Text="<%$ Resources: PesiRealiPrezzoUdm %>" runat="server">Tutti i Pesi Reali + Prezzo per Udm</asp:Localize>
                                                        </label>
                                                    </div>
                                                    <div id="grpChkLayoutRiscontratoTotale">
                                                        <input id="inChkLayoutRiscontratoTotale" name="inFormatiStampa" value="2" class="k-radio" type="radio"/>
                                                        <label class="k-radio-label" for="inChkLayoutRiscontratoTotale">
                                                            <asp:Localize Text="<%$ Resources: PseiRiscontratiPrezzoUdm %>" runat="server">Tutti i Pesi Riscontrati (se disponibili) + Prezzo per Udm</asp:Localize>
                                                        </label>
                                                    </div>
                                                    <div id="grpChkLayoutRiscontrato">
                                                        <input id="inChkLayoutRiscontrato" name="inFormatiStampa" value="3" class="k-radio" type="radio"/>
                                                        <label class="k-radio-label" for="inChkLayoutRiscontrato">
                                                            <asp:Localize Text="<%$ Resources: PesiRealiFatturaRiscontrato %>" runat="server">Tutti i Pesi Reali + Peso Fatturazione / Riscontrato</asp:Localize>
                                                        </label>
                                                    </div>
                                                    <div id="grpChkLayoutPrezzo">
                                                        <input id="inChkLayoutPrezzo" name="inFormatiStampa" value="4" class="k-radio" type="radio"/>
                                                        <label class="k-radio-label" for="inChkLayoutPrezzo">
                                                            <asp:Localize Text="<%$ Resources: DettagliEconomici %>" runat="server">Dettagli Economici</asp:Localize>
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                 <!-- tab Beni Confezionamento -->
                                <div class="tab-pane fade in" id="tabBeniConfezionamento" style="overflow: auto; margin-bottom: 50px;">
                                    <uc2:BeniConfezionamentoUC id="BeniConfezionamentoUC1" runat="server" />
                                </div>

                                <!-- tab Castelletto -->
                                <div class="tab-pane fade in" id="tabCastelletto" style="overflow: auto; margin-bottom: 50px;">
                                    <uc2:CastellettoUC id="CastellettoUC1" runat="server" />
                                </div>
                                
                                <!-- tab Riferimenti Catastali -->
                                <div class="tab-pane fade in" id="tabRifCatastali" style="overflow: auto; margin-bottom: 50px;">
                                    <uc4:RifCatastaliUC id="RifCatastaliUC1" runat="server" />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="row"  style="margin-bottom: 30px;">
	                <div class="col-lg-12 col-md-12 col-sm-12 text-right">
                        <%-- Utilizzato solo per il salvataggio di testata e prima riga di un nuovo documento (non più usato)
		                <div class="btn btn-success submit" id="btn_SalvaRigaDoc2" onclick="SalvaRigaDoc_click()"  style="display: none;">
			                <i class="fa fa-floppy-o"></i>Salva
		                </div>
                        --%>
                        <%-- Utilizzato solo per il salvataggio di testata e righe di un documento esistente --%>
                        <div class="btn btn-info submit" id="btnNuovoDocmmento2" onclick="ApriKendoWindowAggiungiNuovoAllegato()" style="display: none;">
			                <i class="fa fa-paperclip fa-1x info_elem"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NuovoAllegato %>" runat="server"></asp:Localize>
		                </div>
                        <div class="btn btn-success submit text-uppercase" id="btnDocmmenti2"  onclick="btnDocmmenti_click()" style="display: none;">
			                <i class="fa fa-file-text-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneAllegati %>" runat="server"></asp:Localize>
		                </div>
		                <div class="btn btn-success submit xi-btn-primary" id="btnSalvaTestataPiuRiga2" onclick="btnSalvaDoc_click(false,false,false)" style="display: none;">
			                <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server"></asp:Localize>
		                </div>
		                <div class="btn btn-success submit xi-btn-primary" id="btnSalvaEsciDoc2" onclick="btnSalvaDoc_click(true,false,false)" style="display: none;">
			                <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server"></asp:Localize>
		                </div>
                       <div class="btn btn-success submit xi-btn-primary" id="btnSalvaENuovoDoc2" onclick="btnSalvaDoc_click(false,false,true)" style="display: none;">
                            <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaENuovoDoc %>" runat="server"></asp:Localize>
                        </div>
		                <div class="btn btn-warning submit" id="btnSalvaStampa2" onclick="btnSalvaDoc_click(false,true,false)" style="display: none;"> 
			                <i class="fa fa-print"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaStampa %>" runat="server"></asp:Localize>
		                </div>
                        <div class="btn btn-warning submit" id="btnStampaDoc2" onclick="StampaDoc()" style="display: none;"> 
			                <i class="fa fa-print"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StampaDocumentoAbbr %>" runat="server"></asp:Localize>
		                </div>
		                <div class="btn btn-danger" id="btnEsciDoc2" onclick="Azione_Indietro_DocContabile()">
			                <i class="fa fa-reply"></i><span id="btnEsciDocTxt2"><asp:Localize Text="<%$ Resources: EsciSenzaSalvare %>" runat="server"></asp:Localize></span>
		                </div>
	                </div>
                </div>
            </div>

        </div>
    </div>
    <!-- fine container -->

    <div id="selectCentro" style="display:none;">
        <div class="window-content" style="margin-top: 20px;margin-bottom: 20px;">
            <div class="row">
			    <div class="col-xs-12">
                    <div class="form-group">
                        <div class="input-group">
                            <label for="ddlCentro" class="input-group-addon lbl_required" id="lblCentro">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server"></asp:Localize>:
                            </label>
                            <select id="ddlCentro" class="form-control"></select>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="window-footer">
            <div class="row">
                <div class="col-xs-12">
                    <div class="pull-right">
                        <button type="button" class="k-primary k-button" id="btn_selectCentro">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Conferma %>" runat="server"></asp:Localize>
                        </button>&nbsp;
                        <button type="button" class="k-button" id="btn_annullaCentro">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Annulla %>" runat="server"></asp:Localize>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <!-- dialogs varie -->
    <div id="nuovoContattoWindow" class="panel-group" style="display:none;">
        <input type="text" name="inTipoGestioneContatto" id="inTipoGestioneContatto" style="display:none">
        <iframe class="k-content-frame" name="target_iframe" src="about:blank"></iframe>
    </div>
    <div id="confermaEliminazioneDialog"></div>
    <div id="confermaAnnullamentoDialog"></div>
    
    <!-- Hidden Controls -->
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdStato_Cod" runat="server" />
    <input type="hidden" id="hdOperazione" runat="server" />
    <input type="hidden" id="hdLavCod" runat="server" />
    <input type="hidden" id="hdIdAgenda" runat="server" />
    <input type="hidden" id="hdDataDocumento" runat="server" />
    <input type="hidden" id="hdTipoOp" runat="server" />
    <input type="hidden" id="hdKendo_TestataDoc" runat="server" />
    <input type="hidden" id="hdKendo_RigheDoc" runat="server" />
    <input type="hidden" id="hdOpzioniContab" runat="server" />
    <input type="hidden" id="hdTipoAccettazione" runat="server" />
    
    <input type="hidden" id="hf_Qs_PagRitorno" runat="server" />
    <input type="hidden" id="hf_Qs_SaCod" runat="server" />
    <input type="hidden" id="hf_Qs_CaricoScarico" runat="server" />
    <input type="hidden" id="hf_Qs_DataSelezionata" runat="server" />
    <input type="hidden" id="hf_Qs_RagSocContatto" runat="server" />
    <input type="hidden" id="hf_Qs_ModalitaDoc" runat="server" />
    <input type="hidden" id="hf_qsPuaRegolamento" runat="server" />
    <input type="hidden" id="hf_Modulo_Anagrafe_Log" runat="server" />

    <!-- Rappresentano l'utente Gias corrente -->
    <input type="hidden" id="hf_OperatoreCodFisc" runat="server" />
    <input type="hidden" id="hf_OperatoreNominativo" runat="server" />

    <!-- Rappresentano l'utente Gias che ha fatto l'ultima modifica al documento (in insert sono vuote) -->
    <input type="hidden" id="hf_OperatoreModificaCodFisc" runat="server" />
    <input type="hidden" id="hf_OperatoreModificaNominativo" runat="server" />
    
    <input type="hidden" id="hf_UrlPostDelete" runat="server" />
    <input type="hidden" id="hf_UrlNuovoDocumento" runat="server" />

    <input type="hidden" id="hf_ChkAccompagnatoria" runat="server" />

    <input type="hidden" id="hf_ModalitaProtettaDoc" runat="server" />
    <input type="hidden" id="hf_ModalitaProtettaRiga" runat="server" />
    <input type="hidden" id="hf_MesAccessoNonConsentito" runat="server" />
    <input type="hidden" id="hf_Qs_ServizioCod" runat="server" />

    <input type="hidden" id="hdId_Mov_Testata" runat="server" />
    <input type="hidden" id="hdId_Mov_BC_Principale" runat="server" />
    <input type="hidden" id="hdCau_Mov_BC_Principale" runat="server" />
    <input type="hidden" id="hdModalita" runat="server" />
    <input type="hidden" id="hdDes_Lib" runat="server" />
    <input type="hidden" id="hdStatoOrdine" runat="server" />
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />
    <input type="hidden" id="hdData" runat="server" />
    <input type="hidden" id="hdCod_Risum_Scarico" runat="server" />

    <input type="hidden" id="hdPiva_Scarico" runat="server" />
    <input type="hidden" id="hdId_Agenda_Scarico" runat="server" />
    <input type="hidden" id="hdId_Mov_Testata_Scarico" runat="server" />
    <input type="hidden" id="hdId_Mov_Scarico" runat="server" /> 
    <input type="hidden" id="hdLav_Cod_Scarico" runat="server" /> 
    
    <input type="hidden" id="hf_RicercaType" runat="server" /> 
    <input type="hidden" id="hf_RicercaDoc" runat="server" />

    <input type="hidden" id="hf_IdSessionNuovoDoc" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/template_comuni_dropdown.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/imballi_pesi_FF.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/leggi_tabelle_FF_ws_client.js")) %>"></script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DocContabile_globali.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DocContabile.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DocContabile_controlli.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DocContabile_errori.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DocContabile_eventi.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DocContabile_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DocContabile_ws_client.js") %>" ></script>

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cStato_Cod = "#<%=hdStato_Cod.ClientID() %>";
        var cOp = "#<%=hdOperazione.ClientID() %>";
        var cIdLavCod = parseInt($("#<%=hdLavCod.ClientID() %>").val());
        var cIdAgenda = "#<%=hdIdAgenda.ClientID() %>";
        var cIdDataDocumento = "#<%=hdDataDocumento.ClientID() %>";
        var cIdTipoOp = parseInt($("#<%=hdTipoOp.ClientID() %>").val()); 
        var cIdOpzioniContab = "#<%=hdOpzioniContab.ClientID() %>";
        var cTipoAccettazione = "#<%=hdTipoAccettazione.ClientID() %>";

        var cPagRitorno = $("#<%=hf_Qs_PagRitorno.ClientID() %>").val();
        var Qs_SaCod = $("#<%=hf_Qs_SaCod.ClientID() %>").val();
        var Qs_CaricoScarico = $("#<%=hf_Qs_CaricoScarico.ClientID() %>").val().toString();
        var Qs_DataSelezionata = $("#<%=hf_Qs_DataSelezionata.ClientID() %>").val();
        var Qs_RagSocContatto = $("#<%=hf_Qs_RagSocContatto.ClientID() %>").val();
        var Qs_PuaRegolamento = $("#<%=hf_qsPuaRegolamento.ClientID() %>").val();

        var Qs_ModalitaDoc = $("#<%=hf_Qs_ModalitaDoc.ClientID() %>").val();
        var modulo_anagrafe_log = $("#<%=hf_Modulo_Anagrafe_Log.ClientID() %>").val().split("|");
        var operatoreCodFisc = $("#<%=hf_OperatoreCodFisc.ClientID() %>").val();
        var operatoreNominativo = $("#<%=hf_OperatoreNominativo.ClientID() %>").val();
        var operatoreModificaCodFisc = $("#<%=hf_OperatoreModificaCodFisc.ClientID() %>").val();
        var operatoreModificaNominativo = $("#<%=hf_OperatoreModificaNominativo.ClientID() %>").val();

        var hf_UrlPostDelete = "#<%=hf_UrlPostDelete.ClientID() %>";  
        var hf_UrlNuovoDocumento = "#<%=hf_UrlNuovoDocumento.ClientID() %>";
        var hf_IdSessionNuovoDoc = "#<%=hf_IdSessionNuovoDoc.ClientID() %>";
        var hf_Qs_SaCod = "#<%=hf_Qs_SaCod.ClientID() %>";
        var hf_ChkAccompagnatoria = "#<%=hf_ChkAccompagnatoria.ClientID() %>";
       
        var hf_ModalitaProtettaDoc = "#<%=hf_ModalitaProtettaDoc.ClientID() %>";
        var hf_ModalitaProtettaRiga = "#<%=hf_ModalitaProtettaRiga.ClientID() %>";
        var cIdMesAccessoNonConsentito = "#<%=hf_MesAccessoNonConsentito.ClientID() %>";

        var cId_Mov_Testata = "#<%=hdId_Mov_Testata.ClientID() %>";
        var cId_Mov_BC_Principale = "#<%=hdId_Mov_BC_Principale.ClientID() %>";
        var cCau_Mov_BC_Principale = "#<%=hdCau_Mov_BC_Principale.ClientID() %>";
        var cModalita = "#<%=hdModalita.ClientID() %>";       
        
        
        var cDes_Lib = "#<%=hdDes_Lib.ClientID() %>";
        var statoEvasioneDoc = JSON.parse($("#<%=hdStatoOrdine.ClientID() %>").val());
        var cData = "#<%=hdData.ClientID() %>";      
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";

       
        var cPiva_Scarico = "#<%=hdPiva_Scarico.ClientID() %>";
        var cId_Agenda_Scarico = "#<%=hdId_Agenda_Scarico.ClientID() %>";
        var cId_Mov_Testata_Scarico = "#<%=hdId_Mov_Testata_Scarico.ClientID() %>";
        var cId_Mov_Scarico = "#<%=hdId_Mov_Scarico.ClientID() %>";
        var cLav_Cod_Scarico = "#<%=hdLav_Cod_Scarico.ClientID() %>";
        var cCod_Risum_Scarico = "#<%=hdCod_Risum_Scarico.ClientID() %>";

        var cIdRicercaType = "#<%=hf_RicercaType.ClientID() %>";
        var cIdRicercaDoc = "#<%=hf_RicercaDoc.ClientID() %>";

    </script>
    
    <script id="noDataTemplateCausaleTrasporto" type="text/x-kendo-template">
        <div>
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,NessunElementoTrovato %>" runat="server">Nessun elemento trovato</asp:Localize>. <br/>
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,VuoiAggiungereLElementoX %>" runat="server">Vuoi aggiungere l'elemento -</asp:Localize> '#: instance.filterInput.val() #' ?
        </div>
        <br />
        <button class="btn btn-warning" onclick="addNewCausaleTrasporto('#: instance.element[0].id #', '#: instance.filterInput.val() #')">
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,AggiungiElemento %>" runat="server">Aggiungi elemento</asp:Localize>
        </button>
    </script>

    <script id="noDataTemplateMezzoTrasporto" type="text/x-kendo-template">
        <div>
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,NessunElementoTrovato %>" runat="server">Nessun elemento trovato</asp:Localize>. <br/>
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,VuoiAggiungereLElementoX %>" runat="server">Vuoi aggiungere l'elemento -</asp:Localize> '#: instance.filterInput.val() #' ?
        </div>
        <br />
        <button class="btn btn-warning" onclick="addNewMezzoTrasporto('#: instance.element[0].id #', '#: instance.filterInput.val() #')">
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,AggiungiElemento %>" runat="server">Aggiungi elemento</asp:Localize>
        </button>
    </script>
    
    <script id="templateNumDocDDL" type="text/x-kendo-template">
      <span>
        #: Sigla # - <strong>#: PrefissoSuffisso_Des #</strong> <br>(Prefisso = <em>"#: Doc_Numero_Sin #"</em> <br>&nbsp;Suffisso = <em>"#: Doc_Numero_Des #"</em>)
      </span>
    </script>

    <script type="text/javascript">

        async function Contatto_gestisciValore(valore)
        {
            await nuovoCedenteCessionario_Salvato(valore);
        }
    </script>

</asp:Content>
