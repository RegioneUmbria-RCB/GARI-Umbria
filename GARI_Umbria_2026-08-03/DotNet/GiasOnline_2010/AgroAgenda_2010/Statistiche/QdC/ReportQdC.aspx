<%@ Page Title="Report QdC" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="ReportQdC.aspx.vb" Inherits="AgroAgenda_2010.ReportQdC" %>

<asp:Content id="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
    .fixed-header {
        top:0;
        position:fixed;
        width:auto;
        z-index: 1;
    }

    .errorClass {
        border-color:#D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }
    .buttonClass {
        margin: 0 0 10px 1px;
    }
    .jumbotron {
        margin-bottom: 0px !important;
    }
    #SceltaLivelli .k-listbox {
         height: 100%;
    }

    .k-dropdownlist .k-input-inner {
        white-space: normal;
    }

    .mostraGraficoReport {
        margin-left: 15px;
    }
    
    /* Inizio stili per test menu filtri laterale */
    #xoReportVenditeFiltriContainer {
        transition: transform 0.3s ease, opacity 0.3s ease;
        width: 70vw;
        height: calc(100vh - 50px);
        position: fixed;
        top: 50px;
        right: 0;
        background-color: white;
        z-index: 10000;
        overflow-y: scroll;
    }
     #xoReportVenditeFiltriContainer.hidden-sidebar {
        opacity: 0;
        transform: translateX(100%);
     }

     .xo-ricerca-doc-contabili-filtri-action-container {
         margin-right: 20px;
         margin-left: 20px;
     }

     #xoReportVenditeToggleFiltri {
        transition: transform 0.3s ease, opacity 0.3s ease;
        top: 120px;
        position: fixed;
        z-index: 10000;
        right: 70vw;
     }
     #xoReportVenditeToggleFiltri.hidden-sidebar {
        top: 120px;
        position: fixed;
        z-index: 10000;
        right: 0;
     }
     #xoReportVenditeToggleFiltri span svg {
         width: 16px;
     }

     .btn.xonne-btn-filter {
        height: 40px !important;
        border-radius: 0 !important;
        border-top-left-radius: 16px !important;
        border-bottom-left-radius: 16px !important;
        border-color: white !important;
     }

     @media only screen and (max-width: 991px) {
        #xoReportVenditeFiltriContainer {
            top: 100px;
            height: calc(100vh - 100px);
        }
     }
     /* Fine stili per menu laterale */
    </style>
    <script>
        function toggleMenu() {
            //debugger;
            const sidebar = document.getElementById('xoReportVenditeFiltriContainer');
            sidebar.classList.toggle('hidden-sidebar');
            const sidebarAction = document.getElementById('xoReportVenditeToggleFiltri');
            sidebarAction.classList.toggle('hidden-sidebar');
        }
    </script>
</asp:Content>

<asp:Content id="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" id="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" id="hf_UtenteAbilitatoScrittura" />
    <%--<uc:LavorazioneMenuUC id="LavorazioneMenuUC1" runat="server" />--%>

     <div id="searchArea" class="panel-group searchArea xo-report-vendite-container" style="display:none;">
        <% If Master.Master_versione = "2022" Then %>
        <div id="xoReportVenditeToggleFiltri" class="btn xonne-btn-primary xonne-btn-filter" onclick="toggleMenu()">
            <span>
                <svg role="img" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="sliders" class="svg-inline--fa fa-sliders fa-lg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path fill="currentColor" d="M0 416c0-17.7 14.3-32 32-32l54.7 0c12.3-28.3 40.5-48 73.3-48s61 19.7 73.3 48L480 384c17.7 0 32 14.3 32 32s-14.3 32-32 32l-246.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 448c-17.7 0-32-14.3-32-32zm192 0c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zM384 256c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zm-32-80c32.8 0 61 19.7 73.3 48l54.7 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-54.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 288c-17.7 0-32-14.3-32-32s14.3-32 32-32l246.7 0c12.3-28.3 40.5-48 73.3-48zM192 64c-17.7 0-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32s-14.3-32-32-32zm73.3 0L480 64c17.7 0 32 14.3 32 32s-14.3 32-32 32l-214.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 128C14.3 128 0 113.7 0 96S14.3 64 32 64l86.7 0C131 35.7 159.2 16 192 16s61 19.7 73.3 48z"></path></svg>
            </span>
        </div>
            

        <!-- FILTRI Matteo qui iniziano gli elementi che implementano i filtri -->
        <div id="xoReportVenditeFiltriContainer" class="panel-body xo-report-vendite-filtri-container" style="padding-top:5px;">
        <% Else %>
        <div class="panel-body xo-report-vendite-filtri-container" style="padding-top:5px;">
        <% End If %>

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;" >
                        <ul class="nav nav-tabs" role="tablist" id="tabs">
                            <li class="active"><a href="#tabFiltroGenerale" data-toggle="tab" id="a_tabFiltroGenerale">
                                <asp:Localize meta:resourcekey="Generale" runat="server">Generale</asp:Localize>
                            </a></li>
                            <li><a href="#tabFiltroOperazioni" data-toggle="tab" id="a_tabFiltroOperazioni">
                                <asp:Localize meta:resourcekey="FiltroOp" runat="server">Filtro Operazioni</asp:Localize> 
                            </a></li>
                            <li><a href="#tabFiltroImpianti" data-toggle="tab" id="a_tabFiltroImpianti" style="display: none">
                                <asp:Localize meta:resourcekey="FiltroImp" runat="server">Filtro Impianti</asp:Localize>
                            </a></li>
						    <li><a href="#tabFiltroAzienda" data-toggle="tab" id="a_tabFiltroAzienda">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltroAzienda %>" runat="server">Filtro Azienda</asp:Localize>
						    </a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane fade active in" id="tabFiltroGenerale" style="overflow: auto">
                                <div class="jumbotron">
                                    <div class="row">
                                        <div class="col-lg-6 col-sm-6">
				                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" id="lbl_tipo_analisi" for="selTipoAnalisi">
                                                            <asp:Localize meta:resourcekey="TipoAnalisi" runat="server">Tipo analisi</asp:Localize></label>
                                                        <input type="text" id="id_selTipoAnalisi" class="kendoDropDownList" />
                                                    </div>
				                                </div>
                                            </div>     
                                        </div>
                                        <div class="col-lg-6 col-sm-6">
									        <div class="form-horizontal">
                                                <div class="form-group">
												    <div class="input-group">
													    <label class="input-group-addon lbl_required" id="lbl_estrazione" for="selEstrazione">
                                                            <asp:Localize meta:resourcekey="TipoEstra" runat="server">Tipo estrazione</asp:Localize></label>
													    <input type="text" id="id_selEstrazione" class="kendoDropDownList" />
												    </div>
											    </div>
										    </div>
									    </div>
                                    </div>
                                    <div class="row">
                                        <div id="classSwitchImpianti" class="col-lg-3 col-sm-6">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_EstraiImpianti" for="SwitchEstraiImpianti">
                                                            <asp:Localize meta:resourcekey="EstraiImp" runat="server">Estrai Impianti</asp:Localize></span>
                                                        <input id="SwitchEstraiImpianti" name="SwitchEstraiImpianti" runat="server" CssClass="form-control" MaxLength="255">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="classSwitchProdotti" class="col-lg-3 col-sm-6">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_EstraiProdotti" for="SwitchEstraiProdotti">
                                                            <asp:Localize meta:resourcekey="EstraiProd" runat="server">Estrai Prodotti</asp:Localize></span>
                                                        <input id="SwitchEstraiProdotti" name="SwitchEstraiProdotti" runat="server" CssClass="form-control" MaxLength="255">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" hidden>
                                        <div class="form-group col-sm-8">
                                            <div class="input-group" id="groupDdlTipoOutput">
                                                <label class="input-group-addon" id="lblTipoOutput" for="ddlTipoOutput">
                                                    <asp:Localize meta:resourcekey="GeneraReportFormat" runat="server">Genera report in formato</asp:Localize></label>
                                                <select id="TipoOutput" name="TipoOutput" class="form-control">
                                                    <%--<option value="0">Tabella</option>--%>
                                                    <option value="1">Pivot</option>
                                                    <%--<option value="2">PDF</option>--%>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                </div> 
                            </div>
                            <div class="tab-pane fade in" id="tabFiltroOperazioni" style="overflow: auto">
                                <div class="jumbotron">   
                                    <div class="row">
                                        <div class="col-lg-6 col-sm-6">
											<div class="form-horizontal">
						                        <div class="form-group">
                                                    <div class="input-group">
						                                <label class="input-group-addon lbl_required" id="lbl_tipo_operazione" for="selTipoOperazione">
                                                            <asp:Localize meta:resourcekey="TipoOp" runat="server">Tipo Operazione</asp:Localize></label>
						                                <select name="selTipoOperazione" multiple="multiple" id="id_selTipoOperazione" class="form-control" data-placeholder="Tutti"></select>
                                                    </div>
											    </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-sm-6">
											<div class="form-horizontal">
						                        <div class="form-group">
													<div class="input-group">
														<label class="input-group-addon lbl_required" id="lbl_operazione" for="multiselOperazione">
                                                            <asp:Localize meta:resourcekey="Operazione" runat="server">Operazione</asp:Localize></label>
														<select name="multiselOperazione" multiple="multiple" id="id_multiselOperazione" class="form-control" data-placeholder="Tutte"></select>
													</div>
												</div>
											</div>
										</div>
									</div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="ddlFiltroDateOperazione">
                                                            <asp:Localize meta:resourcekey="FiltroDateOperazione" runat="server">Filtro date operazione</asp:Localize></label>
                                                        <select name="ddlFiltroDateOperazione" id="ddlFiltroDateOperazione" class="form-control"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="DateImpostateOperazioni">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_DataRifDal" for="Txt_DataOpDal">
                                                            <asp:Localize meta:resourcekey="DataDaOp" runat="server">Da data operazione</asp:Localize></span>
                                                        <input id="Txt_DataOpDal" name="Txt_DataOpDal" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_DataRifAl" for="Txt_DataOpAl">
                                                            <asp:Localize meta:resourcekey="DataAOp" runat="server">A data operazione</asp:Localize></span>
                                                        <input id="Txt_DataOpAl" name="Txt_DataOpAl" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="PeriodoImpostatoOperazioni" style="display: none">
                                        <div class="col-lg-4 col-md-4 col-sm-9">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="periodoLabOperazioni" for="periodoGiorniOperazioni">
                                                            <asp:Localize meta:resourcekey="periodoGiorni" runat="server">Periodo giorni</asp:Localize>
                                                        </label>
                                                        <input type="text" id="periodoGiorniOperazioni" name="periodoGiorniOperazioni" class="form-control" disabled="disabled">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <!-- tab Filtro Azienda -->
                            <div class="tab-pane fade in" id="tabFiltroAzienda" style="overflow: auto">
                                <div class="jumbotron">
                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
						                        <div class="form-group">
													<div class="input-group">
														<label class="input-group-addon lbl_required" for="multiselReferente">
                                                            <asp:Localize meta:resourcekey="Referente" runat="server">Referente</asp:Localize></label>
														<select name="multiselReferente" multiple="multiple" id="id_multiselReferente" class="form-control"></select>
													</div>
												</div>
											</div>
                                        </div>
									</div>
                                    <div class="row">
                                        <div class="col-lg-10 col-md-10 col-sm-12">
				                            <div class="form-horizontal">
                                                <div class="form-group">
												    <div class="input-group">
													    <label class="input-group-addon lbl_required" for="multiselAzienda">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Azienda %>" runat="server" >Azienda</asp:Localize></label>
													    <select name="multiselAzienda" multiple="multiple" id="id_multiselAzienda" class="form-control"></select>
                                                        <label class="input-group-addon "> (Inserire 3 caratteri della descrizione)</label>
												    </div>
								                </div>
				                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-4 col-md-4 col-sm-12">
											<div class="form-horizontal">
												<div class="form-group">
													<div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="multiselCentroAzienda">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server" >Centro Aziendale</asp:Localize></label>
														<select name="multiselCentroAzienda" multiple="multiple" id="id_multiselCentroAzienda" class="form-control"></select>
													</div>
												</div>
											</div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
				                            <div class="form-horizontal">
                                                <div class="form-group">
									                <div class="input-group">
										                <label class="input-group-addon lbl_required" for="multiselNazione">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nazione %>" runat="server" >Nazione</asp:Localize></label>
										                <select name="multiselNazione" multiple="multiple" id="id_msNazione" class="form-control" data-placeholder="Tutte"></select>
									                </div>
								                </div>
				                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
									                    <label class="input-group-addon lbl_required" for="multiselContea">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Regione %>" runat="server" >Regione</asp:Localize></label>
									                    <select name="multiselContea" multiple="multiple" id="id_msContea" class="form-control" data-placeholder="Tutte"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
				                                        <label class="input-group-addon lbl_required" for="multiselSottocontea">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ProvinciaAbbr %>" runat="server" >Provincia</asp:Localize></label>
				                                        <select name="multiselSottocontea" multiple="multiple" id="id_msSottocontea" class="form-control" data-placeholder="Tutte"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="multiselDistretto">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Comune %>" runat="server" >Comune</asp:Localize></label>
                                                        <select name="multiselDistretto" multiple="multiple" id="id_msDistretto" class="form-control" data-placeholder="Tutti"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- tab Filtro Impianti -->
							<div class="tab-pane fade in" id="tabFiltroImpianti" style="overflow: auto">
                                <div class="jumbotron">
                                    <div class="row">
                                        <div class="col-lg-12 col-md-12 col-sm-12">
											<div class="form-horizontal">
						                        <div class="form-group">
													<div class="input-group">
														<label class="input-group-addon lbl_required" for="multiselSpecie">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Specie %>" runat="server" >Specie</asp:Localize></label>
														<select name="multiselSpecie" multiple="multiple" id="id_msSpecie" class="form-control" data-placeholder="Tutte"></select>
													</div>
												</div>
											</div>
										</div>
									</div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="ddlFiltroDateImpianto">
                                                            <asp:Localize meta:resourcekey="FiltroDateImpianto" runat="server">Filtro date impianto</asp:Localize></label>
                                                        <select name="ddlFiltroDateImpianto" id="ddlFiltroDateImpianto" class="form-control"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="DateImpostate">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_DataRifImpDal" for="Txt_DataImpDal">
                                                            <asp:Localize meta:resourcekey="DataDaImp" runat="server">Da data impianto</asp:Localize></span>
                                                        <input id="Txt_DataImpDal" name="Txt_DataImpDal" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_DataRifImpAl" for="Txt_DataImpAl">
                                                            <asp:Localize meta:resourcekey="DataAImp" runat="server">A data impianto</asp:Localize></span>
                                                        <input id="Txt_DataImpAl" name="Txt_DataImpAl" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="PeriodoImpostato" style="display: none">
                                        <div class="col-lg-4 col-md-4 col-sm-9">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon control-label alert-info" id="periodoLab" for="periodoGiorni">
                                                            <asp:Localize meta:resourcekey="periodoGiorni" runat="server">Periodo giorni</asp:Localize>
                                                        </label>
                                                        <input type="text" id="periodoGiorni" name="periodoGiorni" class="form-control" disabled="disabled">
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
            <div class="row xo-report-vendite-filtri-action-container" style="margin-top: 5px;">
                <div class="col-lg-12 col-xs-12">
                    <% If hdPreferiti.Value Then %>
                    <div class="form-horizontal">
                    	<div class="form-group col-sm-8 col-xs-12">
                            <div class="input-group">
						        <label class="input-group-addon lbl_required" id="lbl_lista_report" for="selListaReport">Report salvati:</label>
						        <select name="selListaReport" id="id_selListaReport" class="form-control" style="white-space: normal;" placeholder="Seleziona un report..."></select>
                            </div>
						</div>
                        <div class="col-sm-4 col-xs-12" style="margin-top: 33px">
                            <div class="btn btn-warning buttonClass xonne-btn-primary" id="btn_salva" style="margin-left:10px;">
                                <span class="fa fa-floppy-o"></span>Salva
                            </div>
                            <div class="btn btn-danger buttonClass" id="btn_elimina" style="margin-left:10px;">
                                <span class="fa fa-trash"></span>Elimina
                            </div>
                        </div>
					</div>
                    <% End If %>  
                </div>
                <div class="col-lg-5 col-xs-12">
                    <div class="row">
                        <div class="col-sm-4">
                            <%--<div class="btn btn-success buttonClass" id="btn_ricerca" style="margin-left:10px;">
                                <span class="fa fa-search"></span>Ricerca
                            </div>--%>
                            <div class="btn btn-success buttonClass xonne-btn-primary" id="btn_esegui">
                                <span class="fa fa-search xo-agronica-style"></span>
                                <asp:Localize meta:resourcekey="Esegui" runat="server">Esegui</asp:Localize>
                            </div>
                        </div>
                    </div>
                </div>
            </div>     
        </div>
        
<%--        <!-- Griglia Report Vendite -->
	    <div id="gridArea" class="panel-group gridArea" style="display: none; margin-bottom: 70px">
		    <div class="panel-body" id="boxGraficoReport" style="display: none; overflow: auto; margin: 10px 0;">
                <div class="row" id="rowConfigGraficoReport">
                    <div class="col-lg-10 col-xs-10" id="configGraficoReport" style="display: none;">
                        <div class="row">
							<div class="form-horizontal">
						        <div class="form-group col-lg-4 col-xs-6">
                                    <div class="input-group">
									    <label class="input-group-addon lbl_required" id="lbl_CampiGrafico" for="selCampiGrafico">Campo:</label>
									    <select name="selCampiGrafico" id="id_selCampiGrafico" class="form-control" data-placeholder="Tutti"></select>
								    </div>
                                </div>
                                <div class="form-group col-lg-4 col-xs-6">
								    <div class="input-group">
									    <label class="input-group-addon lbl_required" id="lbl_ValoriGrafico" for="selValoriGrafico">Valore:</label>
									    <select name="selValoriGrafico" id="id_selValoriGrafico" class="form-control" data-placeholder="Tutti"></select>
								    </div>
                                </div>
                                <div class="col-lg-4 col-xs-12">
                                    <span id="exportGraficoImg" class="k-button k-button-icontext hidden-on-narrow buttonClass"><span class="k-icon k-i-image"></span>Esporta su Immagine</span>
                                    <span id="exportGraficoPdf" class="k-button k-button-icontext hidden-on-narrow buttonClass"><span class="k-icon k-i-pdf"></span>Esporta su PDF</span>
                                </div>
							</div>
						</div>
                    </div>
                </div>
                <div id="graficoReportQdC" style="width:100%; height:500px; margin-bottom: 30px; display: none;"></div>
                
            </div>
            <div class="panel-body" style="margin: 10px 0;">
                <div id="tab_testata_griglia_report_qdc"></div>
            </div>
	    </div>--%>

        <!-- Pivot Report Vendite -->
        <div id="pivotGridArea" class="panel-group pivotGridArea" style="display: none;">
            <div class="panel-body" style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                <div class="row">
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <div class="btn btn-success buttonClass xonne-btn-primary" id="mostraTutto">
                            <span class="fa fa-undo"></span>
                            <asp:Localize meta:resourcekey="Tutto" runat="server">Tutto</asp:Localize>
                        </div>
                        <div class="btn btn-success buttonClass xonne-btn-primary" id="mostraPivot">
                            <span class="fa fa-table"></span>
                            <asp:Localize meta:resourcekey="Dati" runat="server">Dati</asp:Localize>
                        </div>                    
                        <div class="btn btn-success buttonClass xonne-btn-primary" id="mostraConfiguratore">
                            <span class="fa fa-cogs"></span>
                            <asp:Localize meta:resourcekey="Configuratore" runat="server">Configuratore</asp:Localize>
                        </div>
                        <%--<div class="btn btn-danger buttonClass xonne-btn-primary" id="mostraGrafico">
                            <span class="fa fa-bar-chart"></span>Grafico
                        </div>--%>
                    </div>
                    <div class="col-lg-8 col-md-6 col-sm-12" id="grafico_tab_config" style="display: none;">
                        <div class="row">
                            <div class="form-horizontal">
						        <div class="form-group col-lg-6 col-xs-12">
								    <div class="input-group">
									    <label class="input-group-addon lbl_required" id="lbl_tipo_grafico" for="selTipoGrafico">Tipo grafico:</label>
									    <select name="selTipoGrafico" id="id_selTipoGrafico" class="form-control" data-placeholder="Tutti"></select>
								    </div>
							    </div>
						        <div class="col-lg-6 col-xs-12">
                                    <span id="exportGraficoPivotImg" class="k-button k-button-icontext hidden-on-narrow buttonClass"><span class="k-icon k-i-image"></span>Esporta su Immagine</span>
                                    <span id="exportGraficoPivotPdf" class="k-button k-button-icontext hidden-on-narrow buttonClass"><span class="k-icon k-i-pdf"></span>Esporta su PDF</span>
                                </div>
                            </div>
                        </div>                        
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <div id="configuratore_tab_riepilogo" style="width:100%;"></div>
                    </div>
                    <div class="col-lg-8 col-md-6 col-sm-12">                        
                        <div id="grafico_tab_riepilogo" style="width:100%; height:500px; display: none;"></div>
                    </div>
                </div>
                <div class="row" style=" margin-top: 10px;">
                    <div class="col-lg-6 col-xs-12">
                        <span id="exportExcel" class="k-button k-button-icontext hidden-on-narrow buttonClass"><span class="k-icon k-i-excel"></span>Esporta su Excel</span>
                        <span id="exportPdf" class="k-button k-button-icontext hidden-on-narrow buttonClass"><span class="k-icon k-i-pdf"></span>Esporta su PDF</span>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div id="pivot_tab_riepilogo" class="hidden-on-narrow"></div>
                    </div>
                </div>
            </div>
        </div>
    
        <!-- Pivot Report PDF -->
         
    </div>
    <!-- fine container -->
  
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdLingua" runat="server" />
    <input type="hidden" id="hdPreferiti" runat="server" />
    <input type="hidden" id="hdType" runat="server" />

    <script id="rowTemplate" type="text/x-kendo-template">
    # if ((member.name.indexOf("Data_Creazione_Azienda") === 0 && member.name !== "Data_Creazione_Azienda") || (member.name.indexOf("Data_Creazione_Imp") === 0 && member.name !== "Data_Creazione_Imp")) { #
        #: kendo.toString(kendo.parseDate(member.caption), "d") #
    # } else { #
        #: member.caption #
    # } #
    </script>

    <script id="columnTemplate" type="text/x-kendo-template">
    # if ((member.name.indexOf("Data_Creazione_Azienda") === 0 && member.name !== "Data_Creazione_Azienda") || (member.name.indexOf("Data_Creazione_Imp") === 0 && member.name !== "Data_Creazione_Imp")) { #
        #: kendo.toString(kendo.parseDate(member.caption), "d") #
    # } else { #
        #: member.caption #
    # } #
    </script>
    
    <script id="tmplBtnGraficoGriglia" type="text/x-kendo-template">
    <div class="btn btn-danger mostraGraficoReport">
        <span class="fa fa-bar-chart"></span>Grafico
    </div>
    </script>

</asp:Content>

<asp:Content id="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdLingua = "#<%=hdLingua.ClientID() %>";
        var cIdPreferiti = "#<%=hdPreferiti.ClientID() %>";
        var cIdType = "#<%=hdType.ClientID() %>";
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportQdC_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportQdC_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportQdC.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportQdC_jQueryDocReady.js") %>"></script>

</asp:Content>
