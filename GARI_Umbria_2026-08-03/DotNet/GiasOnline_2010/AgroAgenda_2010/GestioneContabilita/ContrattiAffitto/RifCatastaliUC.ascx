<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RifCatastaliUC.ascx.vb" Inherits="AgroAgenda_2010.RifCatastaliUC" %>

<style type="text/css">
    .errorClass {
        border-color: #D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }

    .blockModifica, .blockCancella, .blockDuplica {
        /* display: block; con questo non è possibile ridimensionare la colonna dei pulsanti */
        margin-top: 10px !important;
        margin-bottom: 10px !important;
    }
</style>

<div id="panelAreaRifCatastali" class="panel-group searchArea" style="opacity: 1;">
    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="jumbotron" style="padding-bottom: 0;">
                <div class="container" style="width: 100%;">
                    <div class="container_tabRifCatastali" style="padding: 0; /*margin-bottom: 70px*/">
                        <!-- TAB Riferimenti Catastali -->
                        <div class="panel-group Scarico" id="a_tabRifCata_Scarico">
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12" id="id_riga_scarico">
                                    <!--Griglia-->
                                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="tab_griglia_rif_catastali"></div>
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

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/ContrattiAffitto/RifCatastaliUC_globali.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/ContrattiAffitto/RifCatastaliUC_jQueryDocReady.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/ContrattiAffitto/RifCatastaliUC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/ContrattiAffitto/RifCatastaliUC_ws_client.js")) %>"></script>

<script id="template_Kendo_Btn_CreaParticella" type="text/x-kendo-template">
    <div class="btn btn-success" style="margin-right: 3px;" onclick="RichiamaCreaParticella()">
        <i class="fa fa-plus"></i>Crea Particella
    </div>
</script>  
