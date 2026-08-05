<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="EstrazioneCatastoAffitti.aspx.vb" Inherits="AgronicaStampe_2010.EstrazioneCatastoAffitti" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
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
        margin-bottom: 0 !important;
    }
    </style>

    <style type="text/css">
        .fixed-header {
            top:0;
            position:fixed;
            width:auto;
            z-index: 1;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />

    <div class="panel-group searchArea">
        <div class="panel-body" style="padding-top:5px;">

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div id="tabstrip_Filtri">

                        <div class="tab-pane fade in" id="tabGenerale" style="overflow: auto">
                            <div class="jumbotron">

                                <div class="row" id="rowCentroAziendale">
                                    <div class="col-lg-9 col-sm-9">
										<div class="form-horizontal">
						                    <div class="form-group">
											    <div class="input-group">
												    <label class="input-group-addon lbl_required" id="lbl_centro_aziendale" for="id_multiselCentroAziendale">Centro Aziendale:</label>
													<select name="multiselCentroAziendale" multiple="multiple" ID="id_multiselCentroAziendale" class="form-control" data-placeholder="Tutti"></select>
											    </div>
										    </div>
									    </div>
									</div>
							    </div>

                                <div class="row">
                                    <div class="form-group col-lg-3 col-sm-5">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_DataDal" for="txt_DataDal">
                                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Dal %>" runat="server"></asp:Localize>
                                            </span>
                                            <input id="txt_DataDal" name="txt_DataDal" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                        </div>
                                    </div>

                                    <div class="form-group col-lg-3 col-sm-5">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_DataAl" for="txt_DataAl">
                                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Al %>" runat="server"></asp:Localize>:
                                            </span>
                                            <input id="txt_DataAl" name="txt_DataAl" class="kendoCalendar" style="width: 100%;" maxlength="10" />
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
                    <div class="btn btn-success" id="btn_ricerca">
                        <span class="fa fa-gear lampeggiante"></span><span class="lampeggiante">
                            <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Ricerca %>" runat="server">Ricerca</asp:Localize></span>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Griglia kendo -->
    <div id="containerNew" class="form-group panel-body">
        <div id="gridCatAff"></div>
    </div>
    <br />
    <br />
    <br />
    <br />

    <!-- Hidden Controls Gestiti Lato Server -->
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdUtenteAbilitato" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        // Variabili globali pagina che necessitano di valorizzazione tramite visual basic
        var cIdPiva = "#<%= hdPiva.ClientID %>";
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script src="<%= ResolveClientUrl("~/GestioneStampe/CatastoAffitti/EstrazioneCatastoAffitti/EstrazioneCatastoAffitti_ws_client.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/GestioneStampe/CatastoAffitti/EstrazioneCatastoAffitti/EstrazioneCatastoAffitti.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/GestioneStampe/CatastoAffitti/EstrazioneCatastoAffitti/EstrazioneCatastoAffitti_jQueryDocReady.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>

</asp:Content>

