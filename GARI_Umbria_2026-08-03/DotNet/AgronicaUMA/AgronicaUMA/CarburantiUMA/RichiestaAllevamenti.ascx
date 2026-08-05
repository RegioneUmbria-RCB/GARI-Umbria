<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RichiestaAllevamenti.ascx.vb" Inherits="AgronicaUMA.RichiestaAllevamenti" %>
<div class="row">
    <div class="" style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
        <div id="tab_griglia_allevamenti"></div>
    </div>
</div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/CarburantiUMA/RichiestaAllevamenti.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/CarburantiUMA/RichiestaAllevamenti_ws_client.js")) %>"></script>