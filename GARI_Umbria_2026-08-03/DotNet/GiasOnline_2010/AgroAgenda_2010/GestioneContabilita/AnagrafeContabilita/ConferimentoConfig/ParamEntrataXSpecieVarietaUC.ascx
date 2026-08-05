<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ParamEntrataXSpecieVarietaUC.ascx.vb" Inherits="AgroAgenda_2010.ParamEntrataXSpecieVarietaUC" %>

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
            </div>
        </div>
    </div>
    <div class="panel-group anagArea" style="display: none;">
        <!-- Griglia parametri specie varieta -->
        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
            <div id="tab_parametri_specie_varieta"></div>
        </div>
    </div>

    <!-- fine container -->
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/ParamEntrataXSpecieVarietaUC_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/ParamEntrataXSpecieVarietaUC_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/ParamEntrataXSpecieVarietaUC_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/ParamEntrataXSpecieVarietaUC.js") %>"></script>