<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="BrogliaccioMovimentiTabella.aspx.vb" Inherits="AgronicaStampe_2010.BrogliaccioMovimentiTabella" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="BrogliaccioMovimentiTabella.css?<% =Application("GiasVersioneCorrente")%>" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron" style="padding-left: 15px; padding-right: 15px; margin-bottom: 75px;">
        <div class="row w100" >
            <div class="col-lg-4">
                <h4><span >Filtro Temporale</span></h4>
            </div>
            <div class="col-lg-8">
                <div class="row" >
                    <div class="col-lg-8">
                        <form>
					        <input type="radio" name="finestraTemporale" Value="0" checked="checked"/>Mese<br />
					        <input type="radio" name="finestraTemporale" Value="1"/>Intervallo Temporale<br />
                        </form>
                    </div>
                </div>
                <br />
                <div class="row" >
                    <div class="col-lg-8">
                        <div id="MeseDiv" >
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" for="data_inizio">Mese</label>
                                <input type="text" id="Txt_Mese" Class="form-control" />
                            </div>
                        </div>
                        <div id="TemporaleDiv">
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" for="data_inizio">Data Inizio</label>
                                <input type="text" id="Txt_DataInizio" Class="form-control" />
                            </div>
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" for="data_inizio">Data Fine</label>
                                <input type="text" id="Txt_DataFine" Class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="row w100" >
            <div class="col-lg-2">
                <div class="btn btn-success" onclick="AggiornaKendo();">
                <i class="fa fa-search"></i> Cerca</div>
            </div>
        </div>
        <br />
        <div class="row w100" >
            <div class="col-lg-12">
                <div id="kTable"></div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <!--<script src="http://kendo.cdn.telerik.com/2018.2.516/js/jszip.min.js"></script>-->
    <script src="BrogliaccioMovimentiTabella.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="BrogliaccioMovimentiTabella_JQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="BrogliaccioMovimentiTabella_ws_client.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
</asp:Content>
