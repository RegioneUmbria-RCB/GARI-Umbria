<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FiltroStampeDAA.aspx.vb"
    Inherits="AgronicaStampe_2010.FiltroStampeDAA" MasterPageFile="~/Master/StampeBootstrap.Master" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
     
        #exportExcel
        {
            margin: 0 0 10px 1px;
        }

        #exportPdf
        {
            margin: 0 0 10px 1px;
        }

        
        #mostraConfiguratore
        {
            margin: 0 0 10px 1px;
        }

        #mostraPivot
        {
            margin: 0 0 10px 1px;
        }

        
        #mostraTutto
        {
            margin: 0 0 10px 1px;
        }


</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_Data_Da" />
    <asp:HiddenField runat="server" ID="hf_Data_A" />
    <asp:HiddenField runat="server" ID="hf_Piva" />

	<!-- Filtri -->
	<div id="searchArea" class="panel-group searchArea" style="display: none;">
		<div class="panel-body" style="padding-top:5px;">
            <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-12">
						<div class="input-group">
							<span class="input-group-addon" id="lbl_DaData">Da data:</span>
							<input ID="DaData" name="DaData" runat="server" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
						</div>
					</div>
					<div class="col-lg-3 col-md-3 col-sm-12">
						<div class="input-group">
							<span class="input-group-addon" id="lbl_AData">A data:</span>
							<input ID="AData" name="AData" runat="server" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
						</div>
					</div>
            </div>
			<div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                        <ul class="nav nav-tabs" role="tablist" id="tabs">
                            <li class="active"><a href="#tabReport" data-toggle="tab" id="a_tabReport">Selezione Report</a></li>
                            <li> <a href="#tabRiepilogo" data-toggle="tab" id="a_tabRiepilogo">Anteprima Dati</a></li>
                        </ul>

                        <div class="tab-content">
                             <!-- tab Report -->
                            <div class="tab-pane fade active in" id="tabReport" style="overflow: auto; margin-bottom: 70px;">
                                <div class="jumbotron">


                                 <div class="row">
                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="input-group">
						                    <span class="input-group-addon lbl_required" id="lbl_DataRif">Data stampa:</span>
						                    <input ID="DataRif" name="DataRif" runat="server" class="kendoCalendar" required style="width: 100%;" MaxLength="10" />
					                    </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_numProt">Numero protocollo:</span>
                                            <input name="numProt" id="numProt" class="form-control k-content" required />
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_uffDog">Ufficio Dogane:</span>
                                            <input name="uffDog" id="uffDog" class="form-control k-content" required />
                                        </div>
                                    </div>
                                </div>

                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="input-group">
                                                <span class="input-group-addon lbl_required" id="lbl_LivelloRottura">Tipo report:</span>
											    <input id="idTipoReport" name="idTipoReport" class="form-control"/>
                                            </div>
                                        </div>    
                                        <div class="col-lg-3 col-md-3 col-sm-12">
                                            <div class="btn btn-success" id="btn_report">
                                                <span class="fa fa-print fa-3"></span><span class="">Report</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                              <!-- tab riepilogo -->
                            <div class="tab-pane fade in" id="tabRiepilogo" style="overflow: auto; margin-bottom: 70px;">
                                <div class="jumbotron">
                                    <div class="row">
                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                        <div class="btn btn-success" id="btn_aggiorna_riepilogo">
                                            <span class="fa fa-print fa-3"></span><span class="">Ricerca Garanzie Circolazione</span>
                                        </div>
                                    </div>
                                </div>
                                    <div class="panel-group modifyArea" style="display:none;">
                                        <!--Griglia-->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px; ">
                                            <div id="tab_riepilogo_garanzie" style="display: block;"></div>
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

    <input type="hidden" id="hdPaginaRedirect" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var piva = "#<%=hf_Piva.ClientID() %>";
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
        var data_Da = "#<%=hf_Data_Da.ClientID() %>";
        var data_A = "#<%=hf_Data_A.ClientID() %>";
    </script>

    <script src="filtroStampeDAA_globali.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="filtroStampeDAA.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="filtroStampeDAA_ws_client.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="filtroStampeDAA_jQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    
</asp:Content>