<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Numeratore_PrefissoSuffisso_UC.ascx.vb" Inherits="AgroAgenda_2010.Numeratore_PrefissoSuffisso_UC" %>

 <div id="idFormNumeratorePrefissoSuffisso" class="panel-group">

    <!-- Griglia parametri qualitativi --> 
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
        <div id="tab_numeratore_prefisso_suffisso"></div>
    </div>

</div> 

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/Numeratore_PrefissoSuffisso_UC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/Numeratore_PrefissoSuffisso_UC_globali.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/Numeratore_PrefissoSuffisso_UC_ws_client.js")) %>"></script>
