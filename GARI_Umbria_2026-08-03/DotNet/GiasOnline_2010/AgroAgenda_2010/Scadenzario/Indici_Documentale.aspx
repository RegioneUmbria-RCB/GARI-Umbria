<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Indici_Documentale.aspx.vb"
    Inherits="AgroAgenda_2010.Indici_Documentale" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
    .errorClass {
        border-color:#D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
   
    <div id="panelArea" class="panel-group searchArea">
      
       <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">

            <div class="jumbotron" style="padding-bottom: 0;">
                <div class="container" style="width: 100%;">
                    <div class="container_tabIndice" style="padding: 0; /*margin-bottom: 70px*/">
                       
                        <div class="panel-group Indice">

                           <div class="row">

                              <%--  <div class="col-lg-3 col-md-3 col-sm-12" id="id_area">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="cmbArea">Categoria:</label>
                                                <select name="cmbArea" id="cmbArea" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                                </div>


                                <div class="col-lg-3 col-md-3 col-sm-12" id="id_tipologia">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="cmbTipologia">Tipologia:</label>
                                                <select name="cmbTipologia" multiple="multiple" id="cmbTipologia" class="form-control">                                                
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                </div>--%>

                                
                               

                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_titolo_indice" for="txt_titolo_indice">
                                                <asp:Localize meta:resourcekey="TitoloIndice" runat="server">Titolo Indice</asp:Localize>:
                                            </span>
                                            <asp:TextBox ID="txt_titolo_indice" runat="server" CssClass="form-control" >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>


                                
                            <div class="col-lg-6 col-md-6 col-sm-12">
                               <%-- <div class="btn btn-success" id="saveChanges">
                                    <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
                                </div>--%>
                                 <div class="btn btn-success xonne-btn-primary" id="btn_salva_esci">
                                    <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">
                                         <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                                     </span>
                                </div>
                            </div>                          

                       </div>

                           <div class="row">

                                <div class="col-lg-6 col-md-6 col-sm-12">
					                <div class="input-group">
						                <label class="input-group-addon" id="lblObbligatorio" for="ChkObbligatorio">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Obbligatorio %>" runat="server">Obbligatorio</asp:Localize>
                                        </label>
						                <input type = "checkbox" name="ChkObbligatorio" id="ChkObbligatorio" Class="kendoSwitch" />
					                </div>
				                </div>

                                <div class="col-lg-3 col-md-3 col-sm-12">
					                <div class="input-group">
						                <label class="input-group-addon" id="lblRiservato" for="ChkRiservato">
                                            <asp:Localize meta:resourcekey="Riservato" runat="server">Riservato</asp:Localize>
                                        </label>
						                <input type = "checkbox" name="ChkRiservato" id="ChkRiservato" Class="kendoSwitch" />
					                </div>
				       
                                </div>
				            

                               <div class="col-lg-3 col-md-3 col-sm-12">
					                <div class="input-group">
						                <label class="input-group-addon" id="lblSpeciale" for="ChkSpeciale">
                                            <asp:Localize meta:resourcekey="Speciale" runat="server">Speciale</asp:Localize>
                                        </label>
						                <input type = "checkbox" name="ChkSpeciale" id="ChkSpeciale" Class="kendoSwitch" />
					                </div>
				       
                                </div>
                                
                          </div>

                            <div class="row">                                                                        
                              
                                <div class="col-lg-3 col-md-3 col-sm-12">
			                        <div class="form-horizontal">
                                       <div class="input-group">
                                            <label class="input-group-addon lbl_required" id="Lbl_Validita_Inizio" for="txt_Validita_Inizio">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ValiditàInizio %>" runat="server">Validità Inizio</asp:Localize>:
                                            </label>
                                            <input ID="txt_Validita_Inizio" name="txt_Validita_Inizio" class="kendoCalendar" style="width: 75%;" MaxLength="10" />                                
                                        </div>
                                    </div>				       
                                </div>



                                <div class="col-lg-3 col-md-3 col-sm-12">
			                        <div class="form-horizontal">
                                       <div class="input-group">
                                            <label class="input-group-addon lbl_required" id="lbl_Validita_Fine" for="txt_Validita_Fine">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ValiditàFine %>" runat="server">Validità Fine</asp:Localize>:
                                            </label>
                                            <input ID="txt_Validita_Fine" name="txt_Validita_Fine" class="kendoCalendar" style="width: 75%;" MaxLength="10" />                                
                                        </div>
                                    </div>				       
                                </div>

                             </div>

                             
                                
                            


                             <div class="col-lg-3 col-md-3 col-sm-12" id="id_tipo">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="cmbTipo">
                                                    <asp:Localize meta:resourcekey="TipoCampo" runat="server">Tipo Campo</asp:Localize>:
                                                </label>
                                                <select name="cmbTipo" id="cmbTipo" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                            </div>


                            <div class="col-lg-3 col-md-3 col-sm-12" id="id_elenco">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="cmbElenco">
                                                    <asp:Localize meta:resourcekey="Elenco" runat="server">Elenco</asp:Localize>:
                                                </label>
                                                <select name="cmbElenco" id="cmbElenco" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12" id="id_elenco_valore">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="cmbElenco_Valore">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Valore %>" runat="server">Valore</asp:Localize>:
                                                </label>
                                                <select name="cmbElenco_Valore" id="cmbElenco_Valore" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                            </div>


                            <div class="col-lg-3 col-md-3 col-sm-12" id="id_libero">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="cmbLibero">
                                                    <asp:Localize meta:resourcekey="TipoDato" runat="server">Tipo Dato</asp:Localize>:
                                                </label>
                                                <select name="cmbLibero" id="cmbLibero" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                            </div>



                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12" id="id_riga_dettagli">
                                    <!--Griglia-->
                                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="tab_griglia_dettagli"></div>
                                    </div>
                                </div>

                            </div>
                        </div>

                    </div>


                </div>

            </div>

        </div>
     
     </div> 

  </div>          
   

    <input type="hidden" id="hdId_Area" runat="server" />
    <input type="hidden" id="hdId_Indice" runat="server" />
    <input type="hidden" id="hdRiservato" runat="server" />
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />


    

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Indici_Documentale_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Indici_Documentale.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Indici_Documentale_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Indici_Documentale_ws_client.js") %>"></script>

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";
        var cId_Area = "#<%=hdId_Area.ClientID() %>";
        var cId_Indice = "#<%=hdId_Indice.ClientID() %>";
        var cRiservato = "#<%=hdRiservato.ClientID() %>";
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        var cPaginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";

    </script>

</asp:Content>