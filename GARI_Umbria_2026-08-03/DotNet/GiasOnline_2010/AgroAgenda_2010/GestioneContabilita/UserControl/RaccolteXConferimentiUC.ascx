<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RaccolteXConferimentiUC.ascx.vb" Inherits="AgroAgenda_2010.RaccolteXConferimentiUC" %>

<style>
    #raccolteConfUC_container {
        display: none;
        /*padding: 5px 5px 15px 5px;*/
        margin-bottom: 15px;
    }

    #raccolteConfUC_boxGrigliaRaccolte .k-grid-content {
        max-height: 400px;
    }

</style>


<div id="raccolteConfUC_container">
    <div id="raccolteConfUC_boxGrigliaRaccolte"></div>
</div>

<script id="raccolteConfUC_tmplNuovaRaccolta" type="text/x-kendo-template">
    <button type="button" class="btn btn-success" name="raccolteConfUC_btnNuovaRaccolta" id="raccolteConfUC_btnNuovaRaccolta" onclick="raccolteConfUC_nuovaRaccolta()">
        <!--<i class="fa fa-plus"></i>-->Registra Raccolta Mancante
    </button>
</script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/RaccolteXConferimentiUC_globali.js")) %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/RaccolteXConferimentiUC_ws_client.js")) %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/RaccolteXConferimentiUC.js")) %>" ></script>