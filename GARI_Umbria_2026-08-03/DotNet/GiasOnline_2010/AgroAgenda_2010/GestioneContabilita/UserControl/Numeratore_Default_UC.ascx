<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Numeratore_Default_UC.ascx.vb" Inherits="AgroAgenda_2010.Numeratore_Default_UC" %>

 <div id="idFormNumeratoreTipo" class="panel-group">

    <!-- Griglia parametri qualitativi --> 
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
        <div id="tab_numeratore_defaults"></div>
    </div>

</div> 

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/numeratore_Default_UC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/numeratore_Default_UC_globali.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/numeratore_Default_UC_ws_client.js")) %>"></script>
