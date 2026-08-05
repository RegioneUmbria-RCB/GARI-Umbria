<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Contratti_Pomodoro.aspx.vb"
    Inherits="AgroAgenda_2010.Contratti_Pomodoro" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

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
      
<div class="container" style="width: 100%;">                              
                                                
                <div class="panel-group ContrattiPomodoro">

                       <%-- PRIMA RIGA--%>

                        <div class="row"> 

                            <div class="col-lg-2 col-md-2 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_anno" for="txt_anno">
                                                <asp:Localize meta:resourcekey="Anno" runat="server">Anno</asp:Localize>:
                                            </span>
                                            <asp:TextBox ID="txt_anno" runat="server" CssClass="form-control" MaxLength="4" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_nome" for="txt_nome">
                                                <asp:Localize meta:resourcekey="Nome" runat="server">Titolo</asp:Localize>:
                                            </span>
                                            <asp:TextBox ID="txt_nome" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>



                             <div class="col-lg-4 col-md-4 col-sm-12" id="id_causale">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="cmbCausale">
                                                    <asp:Localize meta:resourcekey="cmbCausale" runat="server">Causale Contratto</asp:Localize>:
                                                </label>
                                                <select name="cmbCausale" id="cmbCausale" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                            </div>


                              <div class="col-lg-2 col-md-2 col-sm-12">                                                                             
                                    <div class="btn btn-success" id="btn_salva">
                                        <span class="fa fa-save lampeggiante"></span><span class="lampeggiante" style="width: 50%;">Salva</span>                                                                     
                                    </div>
                            </div>

                       </div>

                    <%-- SECONDA RIGA --%>

                    <div class="row">
                        <%-- Cmb Centri --%>
                        <div class="col-lg-5 col-md-5 col-sm-12" id="id_centri">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" for="cmbCentri">
                                            <asp:Localize meta:resourcekey="cmbCentri" runat="server">Centro</asp:Localize>:
                                        </label>
                                        <select name="cmbCentri" id="cmbCentri" class="form-control"></select>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%-- Cmb Fabbricati --%>
                        <div class="col-lg-5 col-md-5 col-sm-12" id="id_fabbricati">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" for="cmbFabbricati">
                                            <asp:Localize meta:resourcekey="cmbFabbricati" runat="server">Fabbricato</asp:Localize>:
                                        </label>
                                        <select name="cmbFabbricati" id="cmbFabbricati" class="form-control"></select>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                       <%-- TERZA RIGA--%>

                        <div class="row"> 

                            <div class="col-lg-2 col-md-2 col-sm-12">
                                <div class="input-group">
                                    <label class="input-group-addon lbl_required" id="lbl_TxtNumero" for="TxtNumero">Numero                                                                               
                                    </label>
                                    <input id="TxtNumero" class="form-control" />
                                </div>
                            </div>

                             <div class="col-lg-6 col-md-6 col-sm-12" id="id_conferente">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="cmbConferente">
                                                    <asp:Localize meta:resourcekey="cmbConferente" runat="server">Conferente</asp:Localize>:
                                                </label>
                                                <select name="cmbConferente" id="cmbConferente" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                            </div>
                   
                            
                                <div class="col-lg-4 col-md-4 col-sm-12">
			                        <div class="form-horizontal">
                                       <div class="input-group">
                                            <label class="input-group-addon lbl_required" id="lbl_Data" for="txt_Data">Data Stipulazione</label>
                                            <input ID="txt_Data" name="txt_Data" class="kendoCalendar" style="width: 50%;" MaxLength="10" />                                
                                        </div>
                                    </div>				       
                                </div>

                      </div>


                       


                          
                     <%-- GRIGLIA FASI--%>
                     <div class="row">                                                       
                                                                    
                        <div class="panel-group Agenda">
                            <div class="row">                                                               
                                <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">                                                                      
                                    <!--Griglia Contratti Pomodoro-->
                                    <div id="tab_griglia_fasi"></div>                                                                      
                                </div>
                            </div>

                        </div>
                    
                     </div>


                     <%-- GRIGLIA CLAUSOLE--%>
                     <div class="row">                                                       
                                                                    
                        <div class="panel-group Agenda">
                            <div class="row">                                                               
                                <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">                                                                                                         
                                    <div id="tab_griglia_clausole"></div>                                                                      
                                </div>
                            </div>

                        </div>
                    
                     </div>
                                                       
                       
                    
                    <%-- SPECIFICHE--%>
                    <div class="row">                      
                        <div class="col-lg-6 col-md-6 col-sm-12" id="id_listino">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" for="cmbListino">
                                            <asp:Localize meta:resourcekey="cmbCausale" runat="server">Listino Prezzi</asp:Localize>:
                                        </label>
                                        <select name="cmbListino" id="cmbListino" class="form-control"></select>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <%--Premio 1--%>
                     <div class="row"> 

                                <div class="col-lg-2 col-md-2 col-sm-12">
					                <div class="input-group">
						                <label class="input-group-addon" id="lblPremio" for="ChkPremio">
                                            <asp:Localize meta:resourcekey="Premio" runat="server">Premio gg progressivo</asp:Localize>
                                        </label>
						                <input type = "checkbox" name="ChkPremio" id="ChkPremio" Class="kendoSwitch" />
					                </div>
				       
                                </div>

                                
                               <div class="col-lg-2 col-md-2 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_premio" for="txt_premio">di €/t
                                               <%-- <asp:Localize meta:resourcekey="Premio" runat="server"></asp:Localize>:--%>
                                            </span>
                                            <asp:TextBox ID="txt_premio" runat="server" CssClass="form-control" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                               </div>
                               <div class="col-lg-2 col-md-2 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_tetto" for="txt_tetto">fino al massimo di €
                                               <%-- <asp:Localize meta:resourcekey="Premio" runat="server"></asp:Localize>:--%>
                                            </span>
                                            <asp:TextBox ID="txt_tetto" runat="server" CssClass="form-control" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                              </div>

                                <div class="col-lg-3 col-md-3 col-sm-12">
			                        <div class="form-horizontal">
                                       <div class="input-group">
                                            <label class="input-group-addon lbl_required" id="lbl_DataPremio" for="txt_DataPremio">dal                                               
                                            </label>
                                            <input ID="txt_DataPremio" name="txt_DataPremio" class="kendoCalendar" MaxLength="10" />                                
                                        </div>
                                    </div>				       
                                </div>


                              <div class="col-lg-3 col-md-3 col-sm-12">
			                        <div class="form-horizontal">
                                       <div class="input-group">
                                            <label class="input-group-addon lbl_required" id="lbl_DataPremioFine" for="txt_DataPremioFine">al                                              
                                            </label>
                                            <input ID="txt_DataPremioFine" name="txt_DataPremioFine" class="kendoCalendar" MaxLength="10" />                                
                                        </div>
                                    </div>				       
                                </div>

                             
  
                     </div>  
                    

                    <%--Premio 2--%>
                     <div class="row"> 

                                <div class="col-lg-2 col-md-2 col-sm-12">
					                <div class="input-group">
						                <label class="input-group-addon" id="lblPremio2" for="ChkPremio2">
                                            <asp:Localize meta:resourcekey="Premio" runat="server">Premio gg progressivo</asp:Localize>
                                        </label>
						                <input type = "checkbox" name="ChkPremio2" id="ChkPremio2" Class="kendoSwitch" />
					                </div>
				       
                                </div>

                                
                               <div class="col-lg-2 col-md-2 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_premio2" for="txt_premio2">di €/t
                                               <%-- <asp:Localize meta:resourcekey="Premio" runat="server"></asp:Localize>:--%>
                                            </span>
                                            <asp:TextBox ID="txt_premio2" runat="server" CssClass="form-control" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                               </div>
                               <div class="col-lg-2 col-md-2 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_tetto2" for="txt_tetto2" style="display:none">
                                            </span>
                                            <asp:TextBox ID="txt_tetto2" runat="server" CssClass="form-control" visible="false"  >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                              </div>

                                <div class="col-lg-3 col-md-3 col-sm-12">
			                        <div class="form-horizontal">
                                       <div class="input-group">
                                            <label class="input-group-addon lbl_required" id="lbl_DataPremio2" for="txt_DataPremio2">dal                                               
                                            </label>
                                            <input ID="txt_DataPremio2" name="txt_DataPremio2" class="kendoCalendar" MaxLength="10" />                                
                                        </div>
                                    </div>				       
                                </div>


                              <div class="col-lg-3 col-md-3 col-sm-12">
			                        <div class="form-horizontal">
                                       <div class="input-group">
                                            <label class="input-group-addon lbl_required" id="lbl_DataPremioFine2" for="txt_DataPremioFine2">al                                              
                                            </label>
                                            <input ID="txt_DataPremioFine2" name="txt_DataPremioFine2" class="kendoCalendar" MaxLength="10" />                                
                                        </div>
                                    </div>				       
                              </div>                             
  
                     </div>  


                      <div class="row"> 

                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_DMA" for="txt_DMA">Limite Massimo Difetti Maggiori %</span>
                                            <asp:TextBox ID="txt_DMA" runat="server" CssClass="form-control" MaxLength="4" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_Franchigia_DMA" for="txt_Franchigia_DMA">Franchigia Difetti Maggiori %</span>
                                            <asp:TextBox ID="txt_Franchigia_DMA" runat="server" CssClass="form-control" MaxLength="4" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                      
                      </div>


                     <div class="row"> 

                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_DMI" for="txt_DMI">Limite Massimo Difetti Minori %</span>
                                            <asp:TextBox ID="txt_DMI" runat="server" CssClass="form-control" MaxLength="4" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_Coefficiente" for="txt_Coefficiente">Coefficiente Dequalificazione %</span>
                                            <asp:TextBox ID="txt_Coefficiente" runat="server" CssClass="form-control" MaxLength="4" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label alert-info" id="lbl_Limite" for="txt_Limite">Limite Abbattimento Prezzo %</span>
                                            <asp:TextBox ID="txt_Limite" runat="server" CssClass="form-control" MaxLength="4" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                      
                      </div>

              </div>

         </div>

    </div>

 

          
    
   
    <input type="hidden" id="hdContratto_Cod" runat="server" />
    <input type="hidden" id="hdCau_Contratto" runat="server" />
    <input type="hidden" id="hdVeg_Cod" runat="server" />
    
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contratti_Pomodoro_globali.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contratti_Pomodoro.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contratti_Pomodoro_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contratti_Pomodoro_ws_client.js") %>" ></script>
    
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";

        var cContratto_Cod = "#<%=hdContratto_Cod.ClientID() %>";
        var cCau_Contratto = "#<%=hdCau_Contratto.ClientID() %>";
        var cVeg_Cod = "#<%=hdVeg_Cod.ClientID() %>";
    
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        var cPaginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";

    </script>

    <%--<div class="btn btn-success" id="saveChanges">
   <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
</div>--%>

</asp:Content>
