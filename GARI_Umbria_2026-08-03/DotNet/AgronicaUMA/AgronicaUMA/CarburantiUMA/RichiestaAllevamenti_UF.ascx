<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RichiestaAllevamenti_UF.ascx.vb" Inherits="AgronicaUMA.RichiestaAllevamenti_UF" %>
<div class="row">
    <h3>Dati necessari al calcolo dei capi allevabili</h3>
    <div class="panel panel-default padding-top" id="panel-info-calcolato">
        <div class="panel-heading">
            <h5><b>Unità foraggere prodotte</b></h5>
        </div>
        <div class="panel-body" style="margin-top: 15px;">

            <div class="col-lg-6 col-md-6 col-sm-12" id="id-panel-fascicolo-UF">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon control-label alert-info" id="CTRL_Fascicolo_UF" for="ddlFascicolo_UF">
                                Fascicolo
                            </label>
                            <input type="text" id="ddlFascicolo_UF" name="ddlFascicolo_UF" class="form-control" aria-describedby="CTRL_Fascicolo_UF" <%--onchange="ddlFascicolo_UF_Change();"--%>>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-2 col-md-4 col-sm-12">
                <div class="btn btn-success" id="btn_aggiungi_colture">
                    <span class="fa fa-plus lampeggiante"></span>
                    <span class="lampeggiante">Aggiungi colture</span>
                </div>
                <div class="btn btn-danger" id="btn_elimina_colture">
                    <span class="fa fa-trash lampeggiante"></span>
                    <span class="lampeggiante">Elimina tutto</span>
                </div>
            </div>
        
            <div class="col-lg-12 col-md-12 col-sm-12" style="overflow: auto; /*margin-top: 10px; margin-bottom: 70px;*/">
                <div id="tab_griglia_UFcolture"></div>
            </div>
        </div>
        <div class="panel-heading" style="display: none;">
            <h5><b>Indicazioni aggiuntive</b></h5>
        </div>
        <div class="col-lg-6 col-md-8 col-sm-12" id="montagnaSwitchBox" style="margin-top: 15px; display: none;">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon control-label alert-info" id="montagnaSwitch" for="montagnaSwitchCheck">
                            Intera superficie in zona montana con scarsa produttività
                        </label>
                        <input type="checkbox" id="montagnaSwitchCheck" name="montagnaSwitchCheck" class="kendoSwitch">
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/CarburantiUMA/RichiestaAllevamenti_UF.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/CarburantiUMA/RichiestaAllevamenti_UF_ws_client.js")) %>"></script>