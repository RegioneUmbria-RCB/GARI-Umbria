<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ModalitaPagamentoUC.ascx.vb" Inherits="AgroAgenda_2010.ModalitaPagamentoUC" %>

<div class="panel-group anagArea" style="display: none;">   
    <!-- Griglia reparti piani -->
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">           
        <div class="col-lg-12 col-md-12 col-sm-12"> 
            <div id="tab_modalita_pagamento" class="gias-table-plain"></div>
        </div>
    </div>
</div>

<input type="hidden" id="hdData" runat="server" />

<!-- fine container -->
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ModalitaPagamento/ModalitaPagamentoUC_globali.js") %>"></script> 
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ModalitaPagamento/ModalitaPagamentoUC_jQueryDocReady.js") %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ModalitaPagamento/ModalitaPagamentoUC.js") %>"></script> 
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ModalitaPagamento/ModalitaPagamentoUC_ws_client.js") %>"></script> 
<script type="text/javascript">
    var hfData = "#<%=hdData.ClientID() %>";
</script>