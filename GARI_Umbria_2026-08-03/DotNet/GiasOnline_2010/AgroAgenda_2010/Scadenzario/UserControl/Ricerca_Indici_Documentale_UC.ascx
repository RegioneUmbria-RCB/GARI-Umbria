<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Ricerca_Indici_Documentale_UC.ascx.vb" Inherits="AgroAgenda_2010.Ricerca_Indici_Documentale_UC" %>

    <input type="hidden" id="hdRiservato" runat="server" />

<%--    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdRiservato" runat="server" />--%>

    <div id="panelArea" class="panel-group searchArea">
      
         <div class="row">
        
            <div class="col-lg-offset-9 col-lg-3 col-md-3 col-sm-12" ID="id_nuovo">
                       <br />                          
                <div class="btn btn-success xi-btn-primary btn-block" id="btn_Nuovo">
                    
                <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">
                        <asp:Localize meta:resourcekey="NuovoIndiceDocumentale" runat="server">Nuovo Indice Documentale</asp:Localize>
                    </span>
                </div>

            </div>
        
         </div>

         <div class="row">
   
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                    
                    <div class="row">
                                                                    
                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                            <!--Griglia Alert-->
                            <div id="tab_griglia_Indici"></div>
                                                
                        </div>

                    </div>
                                                          
             
                 </div>
                                    
               </div>

         </div>

    </div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/Ricerca_Indici_Documentale_UC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/Ricerca_Indici_Documentale_UC_jQueryDocReady.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/Ricerca_Indici_Documentale_UC_ws_client.js")) %>"></script>

    <script type="text/javascript">
        var cRiservato = "#<%=hdRiservato.ClientID() %>";
<%--        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";--%>

    </script>

