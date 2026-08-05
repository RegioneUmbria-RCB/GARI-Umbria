<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Lotto_AssegnazioneUC.ascx.vb" Inherits="AgroAgenda_2010.Lotto_AssegnazioneUC" %>
    
<style type="text/css">
    .errorClass {
        border-color:#D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }
</style>

<div class="panel-group anagArea" style="display: none;">

    <!-- Griglia parametri qualitativi -->
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
        <div id="tab_lotto_assegna" class="gias-table-plain"></div>
    </div>
</div>
 
<input type="hidden" id="hf_Tipo_Lotto" runat="server" />
    
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/template_comuni_dropdown.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/Lotti/UserControl/Lotto_AssegnazioneUC_globali.js")) %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/Lotti/UserControl/Lotto_AssegnazioneUC.js")) %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/Lotti/UserControl/Lotto_AssegnazioneUC_jQueryDocReady.js")) %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/Lotti/UserControl/Lotto_AssegnazioneUC_ws_client.js")) %>" ></script>

<script>
    var hfTipo_Lotto = "#<%=hf_Tipo_Lotto.ClientID %>";
</script>