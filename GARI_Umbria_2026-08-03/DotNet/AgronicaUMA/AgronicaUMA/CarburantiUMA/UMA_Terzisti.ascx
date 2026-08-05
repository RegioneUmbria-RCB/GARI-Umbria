<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="UMA_Terzisti.ascx.vb" Inherits="AgronicaUMA.UMA_Terzisti" %>
<div class="row">

    <div class="btn btn-success" id="btn_nuova_richiesta_terzista" style="display: none">
        <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Cerca Richieste 
        </span>
    </div>

</div>

<div class="row">

    <div class="row">
        <div id="legendaNote" style="display: none;">
            <div class="col-lg-12" style="padding-bottom: 15px">
                <h5>Legenda</h5>
            </div>
            <div class="col-lg-1" style="width: 10px; padding-right: 0px;">
                <div style="width: 10px; height: 10px; background-color: #e3c668">
                </div>
            </div>
            <div class="col-lg-10">
                <h5 id="legendaPerc">Note di compilazione obbligatorie</h5>
            </div>
        </div>
    </div>

    <!--Griglia-->
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
        <div id="tab_griglia_terzisti"></div>
    </div>

</div>
