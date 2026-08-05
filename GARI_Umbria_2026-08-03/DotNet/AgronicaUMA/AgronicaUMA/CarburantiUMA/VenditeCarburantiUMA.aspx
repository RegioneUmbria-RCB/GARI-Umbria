<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/UmaBootstrap.Master" CodeBehind="VenditeCarburantiUMA.aspx.vb" Inherits="AgronicaUMA.VenditeCarburantiUMA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="panelArea" class="panel-group searchArea"<%-- style="opacity: 0;"--%>>
        <div class="panel-group preArea"<%-- style="display: none;"--%>>
            <div class="panel-body" style="padding-top: 30px;">

                <div class="row">

                    <div class="col-lg-8 col-md-8 col-sm-12 text-center">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon control-label alert-info" id="CTRL_Azienda" for="ddlAzienda">
                                        Impresa
                                    </label>
                                    <input type="text" id="ddlAzienda" name="ddlAzienda" class="form-control" aria-describedby="CTRL_Azienda">
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-2 col-md-2 col-sm-12 text-center">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon control-label alert-info" id="annolab" for="anno">
                                        Anno
                                    </label>
                                    <input type="text" id="anno" name="anno" class="form-control">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="btn btn-success" id="btn_carica_elenco">
                        <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Cerca
                        </span>
                    </div>
                    
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="jumbotron">
                <div class="row" style="margin-bottom: 10px">
                    <div class="warningInfo" style="color: red; font-weight:bold; display: none">
                    </div>
                </div>                
                <div class="container">
                    <div id="grdVenditeUMACarburanti"></div>
                </div>
            </div>
        </div>
    </div>

    <div style="display: none">
        <div id="dettagliLtAcquistabili">
            <div class="row">
                <div class="col-lg-8 col-md-10">
                    <div class="input-group">
                        <span class="input-group-addon" for="contoProprioTxtBox">Conto Proprio</span>
                        <input type="text" id="contoProprioTxtBox" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-8 col-md-10">
                    <div class="input-group">
                        <span class="input-group-addon" for="contoTerziTxtBox">Conto Terzi</span>
                        <input type="text" id="contoTerziTxtBox" class="form-control" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <input type="hidden" id="hf_UtenteAbilitatoLettura" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura" runat="server" />
    <input type="hidden" runat="server" id="hfPathCoreWS" name="hfPathCoreWS" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("VenditeCarburantiUMA.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("VenditeCarburantiUMA_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("VenditeCarburantiUMA_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>

    <script type="text/javascript">
        objP_server = '<%=objparametri_server_string %>';
        pathCoreWS = '<%=hfPathCoreWS.Value %>';

    </script>
</asp:Content>
