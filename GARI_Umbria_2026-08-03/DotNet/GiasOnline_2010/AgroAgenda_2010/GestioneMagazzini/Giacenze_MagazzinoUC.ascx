<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Giacenze_MagazzinoUC.ascx.vb" Inherits="AgroAgenda_2010.Giacenze_MagazzinoUC" %>

     <div id="areaGiacenzeMagazzino" class="panel-group searchArea">
        <div class="panel-body" style="padding-top:10px;">   
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                        <div ID="tabstrip_GiacenzeMagazzino" Class="kendoTabStrip_GiacenzeMagazzino">
                            <ul>
								<li><asp:Localize meta:resourcekey="FiltroProdottoELotto" runat="server">Filtro Prodotto e lotto</asp:Localize></li>
								<li><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltroPerData %>" runat="server">Filtro per data</asp:Localize></li>
                                <li><asp:Localize meta:resourcekey="FiltroPerCentro" runat="server">Filtro per centro</asp:Localize></li>
								<li><asp:Localize meta:resourcekey="FiltroPerCaratteristiche" runat="server">Filtro per caratteristiche</asp:Localize></li>
                                <li><asp:Localize meta:resourcekey="FiltroPerImballi" runat="server">Filtro per imballi</asp:Localize></li>
							</ul>

                            <!-- tab Filtro Colturale -->
                            <div id="tabFiltroProdottoLotto" style="overflow: auto">
                                   <div class="row">
                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Lotto" for="txt_Lotto">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Lotto %>" runat="server">Lotto</asp:Localize>:
                                                        </span>
                                                        <asp:TextBox ID="txt_Lotto" runat="server" CssClass="form-control"/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                       <div class="col-lg-4 col-md-4 col-sm-12">
											<div class="form-horizontal">
						                        <div class="form-group">
													<div class="input-group">
														<label class="input-group-addon lbl_required" for="ddlSpecie" >
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Specie %>" runat="server">Specie</asp:Localize>:
                                                        </label>
														<asp:TextBox ID="ddlSpecie" runat="server" CssClass="form-control"></asp:TextBox>
				                                    </div>
						                        </div>
						                    </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-12">
											<div class="form-horizontal">
												<div class="form-group">
													<div class="input-group">
														<label class="input-group-addon lbl_required" for="multiselVarieta">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server">Varietà</asp:Localize>:
                                                        </label>
														<select name="multiselVarieta" multiple="multiple" ID="id_multiselVarieta" Class="form-control"></select>
													</div>
												</div>
											</div>
                                        </div>
                                    </div>
                            
                                <!-- tab Filtro Prodotto -->
                                <div id="tabFiltroProdotto" style="overflow: auto">
                                        <div class="row">
                                            <div class="col-lg-8 col-md-8 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon lbl_required" id="lbl_Prodotto">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prodotto %>" runat="server">Prodotto</asp:Localize>:
                                                            </span>
														    <input id="idIProdotto" class="form-control"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-4 col-md-4 col-sm-12">
                                                <div class="input-group">
                                                    <label class="input-group-addon" id="lblAggregaLottoImpianto" for="chkAggregaLottoImpianto">
                                                        <asp:Localize meta:resourcekey="SoloGiacenzePositive" runat="server">Solo giacenze positive:</asp:Localize>
                                                    </label>
                                                    <input type="checkbox" name="chkGiacenzePositive" id="chkGiacenzePositive" class="kendoSwitch" />
                                                </div>
                                            </div>
                                    </div>
                                </div>
                         </div>

                            <!-- tab Filtro Per data -->
                            <div id="tabFiltroPerData" style="overflow: auto">
                                <div class="row">
                                        <div class="col-lg-5 col-md-5 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_DataRif_GiacenzeMagazzino" for="txt_DataRif_GiacenzeMagazzino">
                                                            <asp:Localize meta:resourcekey="DataRiferimentoGiacenza" runat="server">Data riferimento giacenza:</asp:Localize>
                                                        </span>
                                                        <input ID="txt_DataRif_GiacenzeMagazzino" name="txt_DataRif_GiacenzeMagazzino" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-7 col-md-7 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                     &nbsp;
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>                       
                            </div>

                            <!-- tab Filtro per centro -->
                            <div id="tabFiltroPerCentro" style="overflow: auto">
                                <div class="row" id="rowCentroAziendale">
                                        <div class="col-lg-9 col-md-9 col-sm-12">
											<div class="form-horizontal">
						                        <div class="form-group">
													<div class="input-group">
														<label class="input-group-addon lbl_required" id="lbl_centro_aziendale" for="idCentro">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server">Centro Aziendale</asp:Localize>:
                                                        </label>
														<select name="idCentro" ID="idCentro" class="form-control"></select>
													</div>
												</div>
											</div>
										</div>
                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                     &nbsp;
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
							    </div>
                            </div>

                            <!-- tab Filtro Per caratteristiche -->
                            <div id="tabFiltroCaratteristiche" style="overflow: auto">
                                    <div class="row">
                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Qualita">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Qualità %>" runat="server">Qualità</asp:Localize>:
                                                        </span>
														<input id="idQualita" name="idQualita" class="form-control"/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Calibro">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Calibro %>" runat="server">Calibro</asp:Localize>:
                                                        </span>
                                                        <input id="idCalibro" name="idCalibro" class="form-control"/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
										<div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Certificazione">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Certificazione %>" runat="server">Certificazione</asp:Localize>:
                                                        </span>
														<input id="idCertificazione" name="idCertificazione" class="form-control"/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <%--<div class="col-lg-3 col-md-3 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Rugginosita">Rugginosità:</span>
														<input id="idRugginosita" name="idRugginosita" class="form-control"/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>--%>
                                    </div>
                            </div>
                            
                            <!-- tab Filtro Imballo -->
                            <div id="tabFiltroImballi" style="overflow: auto">
                                    <div class="row">
                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Imballo">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Imballo %>" runat="server">Imballo</asp:Localize>:
                                                        </span>
														<input id="idImballaggio" name="idImballaggio" class="form-control"/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Contenitore">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Contenitore %>" runat="server">Contenitore</asp:Localize>:
                                                        </span>
                                                       <input id="idContenitore" name="idContenitore" class="form-control"/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
										<div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Confezione">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Confezione %>" runat="server">Confezione</asp:Localize>:
                                                        </span>
														<input id="idConfezione" name="idConfezione" class="form-control"/>
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
           <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="btn btn-success xonne-btn-primary" id="btn_ricerca_GiacenzeMagazzino">
                        <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize>
                        </span>
                    </div>
                </div>
            </div>     
         </div>
         <!-- Griglia giacenze -->
            <div style="overflow: auto; margin-top: 5px; margin-bottom: 5px;">
                <div id="tab_giacenze"></div>
            </div>
    </div> 
      
    <input type="hidden" id="hdKendo_Giacenze" runat="server" />
    <!-- fine container -->
