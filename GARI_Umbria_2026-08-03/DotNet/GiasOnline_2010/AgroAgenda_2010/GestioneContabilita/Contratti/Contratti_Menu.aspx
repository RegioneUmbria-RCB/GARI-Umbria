<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Contratti_Menu.aspx.vb"
    Inherits="AgroAgenda_2010.Contratti_Menu" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
    
    .errorClass {
            border-color:#D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;               
    }
    
    .dpiOn {
        box-shadow: 1px 1px rgba(0,0,0,.075) inset;
        background-color: Orange;
        pointer-events: none;
    }

</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
   
    <div id="panelArea" class="panel-group searchArea" style="opacity: 0;">
      
          <div class="row">
   
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                        <%-- ul = UNORDERED LIST--%>
                        <ul class="nav nav-tabs" role="tablist" id="tabs">
                           
                            <li class="active"><a href="#tabContrattiPomodoro" data-toggle="tab"" id="a_tabContrattiPomodoro">CONTRATTI POMODORO</a></li>
                            <li><a href="#tabClausole" data-toggle="tab"" id="a_tabClausole">CLAUSOLE</a></li>  

                            </ul>
                                <div class="tab-content">
                                     <!-- tabContrattiPomodoro -->
                                    <div class="tab-pane fade active in" id="tabContrattiPomodoro" style="overflow: auto; margin-bottom: 70px;">                                                              
                                         <div class="jumbotron" style="padding-bottom: 0;">
                                                 <div class="container" style="width: 100%;">                              
                                                    <div class="container_Padre" style="padding: 0px; /*margin-bottom: 70px*/">                                              
                                                        <div class="panel-group ContrattiPomodoro">
                                                             <div class="row"> 

                                                                   <div class="col-lg-6 col-md-6 col-sm-12" ID="id_riga_conferente">                                                                 											                        
													                        <div class="input-group">
														                        <label class="input-group-addon lbl_required" for="multiselConferente">Conferente:</label>
														                        <select name="multiselConferente" multiple="multiple" ID="id_multiselConferente" Class="form-control"></select>
													                        </div>												                      
                                                                </div>

                                                                 <div class="col-lg-6 col-md-6 col-sm-12" ID="id_riga_prodotto">                                                                 											                        
													                        <div class="input-group">
														                        <label class="input-group-addon lbl_required" for="multiselProdotto">Prodotto:</label>
														                        <select name="multiselProdotto" multiple="multiple" ID="id_multiselProdotto" Class="form-control"></select>
													                        </div>												                      
                                                                </div>
                                                             
                                                             </div>

                                                             <div class="row"> 
                                                                 
                                                                  <div class="col-lg-3 col-md-3 col-sm-12" ID="id_riga1">                                                                           
                                                                         <div class="input-group">
                                                                            <label class="input-group-addon lbl_required" id="lbl_TxtNumero" for="TxtNumero">Numero                                                                               
                                                                            </label>
                                                                            <input id="TxtNumero" class="form-control" />
                                                                        </div>
                                                                 </div>
                                                            

                                                                 <div class="col-lg-3 col-md-3 col-sm-12" ID="id_riga_anno">                                                                 											                        
													                        <div class="input-group">
														                        <label class="input-group-addon lbl_required" for="multiselAnno">Anno:</label>
														                        <select name="multiselAnno" multiple="multiple" ID="id_multiselAnno" Class="form-control"></select>
													                        </div>												                      
                                                                </div>

                                                                

                                                                <div class="col-lg-3 col-md-3 col-sm-12">  
                                                                  <div class="btn btn-success xonne-btn-primary" id="btn_Ricerca">
                                                                    <span class="fa fa-search lampeggiante"></span><span class="lampeggiante" style="width: 100%;">Trova</span>                                                                     
                                                                  </div>
                                                                </div>
 

                                                              
                                                                <div class="col-lg-3 col-md-3 col-sm-12">                                                                             
                                                                       <div class="btn btn-success xonne-btn-primary" id="btn_nuovo_contratto">
                                                                         <span class="fa fa-file lampeggiante"></span><span class="lampeggiante" style="width: 100%;">Nuovo Contratto Pomodoro</span>                                                                     
                                                                     </div>
                                                                </div>


<%--                                                                  <div class="col-lg-3 col-md-3 col-sm-12">                                                                             
                                                                       <div class="btn btn-success" id="btn_nuovo_contratto_generico">
                                                                         <span class="fa fa-file lampeggiante"></span><span class="lampeggiante" style="width: 50%;">Nuovo Contratto</span>                                                                     
                                                                     </div>
                                                                </div>--%>
                                                            </div>


                                                      
                                                      
                                                            <!--Griglia-->                                                    
                                                            <div class="panel-group Agenda">
                                                                <div class="row">                                                               
                                                                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">                                                                      
                                                                        <!--Griglia Contratti Pomodoro-->
                                                                        <div id="tab_griglia_contratti_pomodoro"></div>                                                                      
                                                                    </div>
                                                                </div>

                                                            </div>
                                                       
                                                   
                                                        </div>  

                                                     </div>

                                                </div>
                                                          
                                        </div>

                                     </div>

                                     <!-- tabClausole -->
                                     <div class="tab-pane fade in" id="tabClausole" style="overflow: auto; margin-bottom: 70px;">                                                              
                                         <div class="jumbotron" style="padding-bottom: 0;">
                                                 <div class="container" style="width: 100%;">                              
                                                      <div class="panel-group Clausole">
                                                          <div class="row">                                                               
                                                               <div class="col-lg-3 col-md-3 col-sm-12"> 
                                                                 <div class="btn btn-success xonne-btn-primary" id="SalvaClausole">
                                                                    <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva Clausole</span>
                                                                </div> 
                                                              </div> 
                                                          </div>                                                                                                                                       
                                                      </div>
                                                          
                                                      <div class="row"> 
                                                      

                                                            <!--Griglia-->
                                                            <%--<div class="panel-group padreImpianti" style="display: none;">--%>
                                                              <div class="row"> 
                                                                <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                                                    <div id="tab_griglia_clausole"></div>
                                                                </div>
                                                              </div>
                                            
                                                              <%-- </div>--%>



                                                         </div>
                                                            
                                                   </div>


                                                  </div>
                               
                                           </div>
                                     
                                     </div>

                               </div>    
                       </div>
                              
                </div>
                
    </div>

 
    <!--dialogs varie-->    
	<div id="confermaEliminazioneDialog"></div>
	
   
    <input type="hidden" id="hdCau_Contratto" runat="server" />
    
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contratti_Menu_globali.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contratti_Menu.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contratti_Menu_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contratti_Menu_ws_client.js") %>" ></script>

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";

        var cCau_Contratto = "#<%=hdCau_Contratto.ClientID() %>";
    
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        var cPaginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";

    </script>

</asp:Content>
