<%@ Page Title="Esporta_GiasToSap" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="Esporta_GiasToSap.aspx.vb" Inherits="AgronicaStampe_2010.Esporta_GiasToSap" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Esporta_GiasToSap.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	
	<div class="boxNoSlide">
		<h3 style="float: left">Esportazione da GIAS a SAP</h3>
		<!-- btnStampa -->
		<div class="btn btn-info add" id="BtnStampa" style="float: right; margin-top: 25px; ">
			<i class="fa fa-print"></i>
			<span>
				Stampa
			</span>
		</div>
		<h3 style="clear: both"></h3>
		<div class="jumbotron">
			<div id="tabstrip">
				<div class="row">
					<div class="col-lg-3 col-md-12 col-xs-12">
						<!-- Cooperative -->
						<div class="input-group">
							<label class="input-group-addon control-label alert-info" id="lblCooperative" for="CBLCooperativa">
								Cooperative:
							</label>
							<input type="text" id="CBLCooperativa" class="form-control" aria-describedby="lblCooperative" />
						</div>
					</div>
					<div class="col-lg-3 col-md-12 col-xs-12">
						<!-- Piani Semina -->
						<div class="input-group">
							<label class="input-group-addon control-label alert-info" id="lblPianiSemina" for="CBLPianoSemina">
								Piani Semina:
							</label>
							<input type="text" id="CBLPianoSemina" class="form-control" aria-describedby="lblPianiSemina" />
						</div>
					</div>
				</div>
				<div class="row">
                        <div class="col-lg-6 col-md-6 col-xs-12">
                            <!-- Specie Vegetali-->
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" id="lblSpecieVegetali" for="ddlSpecieVegetali">
                                    Specie Vegetale
                                </label>
                                <input type="text" id="ddlSpecieVegetali" class="form-control" aria-describedby="lblSpecieVegetali" />
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-xs-12">
                            <!-- Varieta -->
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" id="lblVarieta" for="ddlVarieta">
                                    Varieta
                                </label>
                                <input type="text" id="ddlVarieta" class="form-control" aria-describedby="lblVarieta" />
                            </div>
                        </div>
                    </div>
				<div class="row">
					<div class="col-lg-1 col-md-6 col-xs-12">
						<label>
						Tipo report:
						</label>
						<!-- Formato -->
						<ul class="fieldlist">
							<li>
								<input type="radio" value="0" name="formato" class="k-radio" id="excel" checked="checked"/>
								<label class="k-radio-label" for="excel">
									Excel
								</label>
							</li>
							<li>
								<input type="radio" value="1" name="formato" class="k-radio" id="www"/>
								<label class="k-radio-label" for="www">
									www
								</label>
							</li>
						</ul>
					</div>
					<div class="col-lg-1 col-md-6 col-xs-12">
						<label>
						Tipo data:
						</label>
						<!-- Anno/Intervallo-->
						<ul class="fieldlist" id="annoOintervallo">
							<li>
								<input type="radio" value="0" name="annointervallo" class="k-radio" id="annoO" checked="checked"/>
								<label class="k-radio-label" for="annoO">
									Anno
								</label>
							</li>
							<li>
								<input type="radio" value="1" name="annointervallo" class="k-radio" id="intervalloO"/>
								<label class="k-radio-label" for="intervalloO">
									Intervallo  
								</label>
							</li>
						</ul>
					</div>
					<div class="col-lg-4 col-md-6 col-xs-12">
                       <!-- Data inizio-->
                       <div id="inizio" class="input-group">
                           <label class="input-group-addon control-label alert-info" id="lblInizioIntervallo" for="txtInizioIntervallo">
							   Data inizio
                           </label>
                           <input type="text" value="gg/MM/yyyy" id="txtInizioIntervallo" class="form-control" aria-describedby="lblInizioIntervallo" />
                       </div>
                    </div>
					<div class="col-lg-4 col-md-6 col-xs-12">
                       <!-- Data fine-->
                       <div id="fine" class="input-group">
                           <label class="input-group-addon control-label alert-info" id="lblFineIntervallo" for="txtFineIntervallo">
							   Data fine
                           </label>
                           <input type="text" value="gg/MM/yyyy" id="txtFineIntervallo" class="form-control" aria-describedby="lblFineIntervallo" />
                       </div>
                    </div>
					<!-- btnOggi -->
					<div class="btn btn-success add" id="btnOggi" onclick="ImpostaOggi();">
						<span>Oggi</span>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
	<script type="text/javascript">
		$(document).ready(function () {

			$(document).on("change", "input[type='radio']", function () {
				radioButtonCambio("#" + this.id);
			});

			popolaDropDownPianiSemina();
			popolaDropDownCooperative();
            popolaDropDownSpecieVegetali();
			popolaDropDownVarieta();

			$("#txtInizioIntervallo").kendoDatePicker({format: 'dd/MM/yyyy'});
			$("#txtFineIntervallo").kendoDatePicker({ format: 'dd/MM/yyyy' });
			ImpostaOggi();

			var anno = document.querySelector('input[name="annointervallo"]:checked').id;
			radioButtonCambio("#" + anno);

			$("#txtInizioIntervallo").attr("readonly", false);
            $("#txtFineIntervallo").attr("readonly", false);
		});

        $("#BtnStampa").click(function () {
			prendiValori();
		});
    </script>
</asp:Content>