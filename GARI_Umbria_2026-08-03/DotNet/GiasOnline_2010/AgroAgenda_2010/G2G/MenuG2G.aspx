<%@ Page Title="Menu G2G" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="MenuG2G.aspx.vb" Inherits="AgroAgenda_2010.MenuG2G" EnableEventValidation="false" ValidateRequest="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="panCfgContainer" runat="server">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
            <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

            <div class="panel-group searchArea">
                <div class="panel-body" style="padding-top: 30px;">
                    <div class="row">
                        <div class="col-lg-6 col-sm-6 col-sm-12 ">

                            <asp:Button ID="btn_salva" runat="server" Text="Salva" style="display:none" />
                            <div class="clear">
                            </div>

                        <%--    <asp:Button ID="btn_salva2" runat="server" Text="Salva" />
                            <div class="clear">
                            </div>--%>

                            <asp:Button ID="btn_nuovo" runat="server" Text="Nuovo"  style="display:none" />
                            <div class="clear">
                            </div>

                            <div class="btn btn-success" id="btn_salvaJs">
                                <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
                            </div>

                            <div class="btn btn-warning" id="btn_nuovoJs">
                                <span class="fa fa-file-o"></span><span class="lampeggiante">Nuovo</span>
                            </div>
                            <div class="input-group">
                                <span class="input-group-addon lbl_required" id="lblConfigurazione" for="ddlConfigurazioni">Configurazione:</span>
                                <asp:DropDownList type="text" ID="ddlConfigurazioni" Style="width: 100%;" runat="server" AutoPostBack="true" />
                                <asp:TextBox runat="server" ID="txtCfg" />
                            </div>
                        </div>
                        <div class="col-lg-6 col-sm-6 col-sm-12 ">
                            <div class="btn btn-success" id="btn_esporta" style="margin-bottom: 10px;">
                                <i class="fa fa-copy"></i>G2G
                            </div>
                            <div class="btn btn-warning" id="btn_esporta_xml" style="margin-bottom: 10px;">
                                <i class="fa fa-copy"></i>G2G Custom
                            </div>

                        </div>
                    </div>
                    

                </div>
            </div>



             <div class="row">
                 <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                         <%-- ul = UNORDERED LIST--%>
                          <ul class="nav nav-tabs" role="tablist" id="tabs">
                           <li class="active"><a href="#tabGenerali" data-toggle="tab"" id="a_tabGenerali">Generali</a></li>                    
                           <li><a href="#tabDati" data-toggle="tab" id="a_tabDati">Configurazione GIAS</a></li>
                           <li><a href="#tabTrasferimento" data-toggle="tab" id="a_tabTrasferimento">Impostazioni di Trasferimento Impresa</a></li>
                           <li><a href="#tabGlobali" data-toggle="tab" id="a_tabGlobali">Impostazioni di Trasferimento Globali</a></li>
                           <li><a href="#tabXML" data-toggle="tab" id="a_tabXML">XML</a></li>
                           <li><a href="#tabLOG" data-toggle="tab" id="a_tabLOG">LOG</a></li>
                        </ul>
                        

                                              
                           <div class="tab-content">
                             <!-- tab Generali -->     
                             <div class="tab-pane fade active in" id="tabGenerali" style="overflow: auto; margin-bottom: 70px;">                                                              
                                 <div class="jumbotron" style="padding-bottom: 0;">
                                         <div class="container" style="width: 100%;">                              
                                            <div class="container_dettagli" style="padding: 0px; /*margin-bottom: 70px*/">

                                                <asp:Panel ID="generali" runat="server" CssClass="boxColore">

                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_1">URL del Web Service:</span>
                                                        <asp:TextBox ID="Txb_WebService" runat="server" CssClass="floatSX txtUI_170" />
                            
                                                        <asp:Label CssClass="etichetta_L" ID="LblErr" runat="server"
                                                            Text="" Enabled="False" ForeColor="Red"></asp:Label>
                                                        <asp:Button ID="btn_DDConnessioni" runat="server" Text="Leggi" />
                                                        <div class="clear">
                                                        </div>
                                                       
                                                    </div>

                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_2">Connessione SERVER ORIGINE</span>
                                                        <asp:DropDownList ID="ddConnessione_SERVER_ORIGINE" CssClass="floatSX txtUI_170"
                                                            runat="server" AutoPostBack="true"/>
                                                    </div>

                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_3">Connessione SERVER DESTINAZIONE</span>
                                                        <asp:DropDownList ID="ddConnessione_SERVER_DESTINAZIONE" runat="server" CssClass="floatSX txtUI_170" />
                                                    </div>

                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_4">Connessione UTENTI ORIGINE</span>
                                                        <asp:DropDownList ID="ddConnessione_UTENTI_ORIGINE" runat="server" CssClass="floatSX txtUI_170" />
                                                        <asp:Button ID="btnRefreshOrigine" runat="server" Text="Leggi" />
                                                        <div class="clear">
                                                        </div>
                                                    </div>

                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_5">Connessione UTENTI DESTINAZIONE</span>
                                                        <asp:DropDownList ID="ddConnessione_UTENTI_DESTINAZIONE" runat="server" CssClass="floatSX txtUI_170" />
                                                        <asp:Button ID="btnRefreshDestinazione" runat="server" Text="Leggi" />
                                                        <div class="clear">
                                                        </div>
                                                    </div>

                                                    </asp:Panel>
                                                </div>
                                          </div>
                                  </div>
                            </div>
                            


                             <!-- tab Dati -->                          
                            <div class="tab-pane fade in " id="tabDati" style="overflow: auto; margin-bottom: 70px;">
                                <div class="row">                           
                                     <div class="jumbotron" style="padding-bottom: 0;">
                                         <div class="container" style="width: 100%;">                              
                                            <div class="container_testata" style="padding: 0px; /*margin-bottom: 70px*/">

                                                <asp:Panel ID="Panel1" runat="server" CssClass="boxColore">


                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_6">Progressivo Gias Origine</span>
                                        <asp:TextBox ID="txtProgressivoGIAS_ORIGINE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_7">Progressivo Gias Destinazione</span>
                                        <asp:TextBox ID="txtProgressivoGIAS_DESTINAZIONE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_8">PivaSuperUser ORIGINE</span>
                                        <asp:TextBox ID="txtPivaSuperUser_ORIGINE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_9">PivaSuperUser DESTINAZIONE</span>
                                        <asp:TextBox ID="txtPivaSuperUser_DESTINAZIONE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_10">UsernameSuperUser Origine</span>
                                        <asp:TextBox ID="txtUsernameSuperUser_Origine" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_11">UsernameSuperUser Destinazione</span>
                                        <asp:TextBox ID="txtUsernameSuperUser_Destinazione" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_12">Import CodFiscale ORIGINE</span>
                                        <asp:TextBox ID="txtImport_CodFiscale_ORIGINE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_13">Import CodFiscale DESTINAZIONE</span>
                                        <asp:TextBox ID="txtImport_CodFiscale_DESTINAZIONE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_14">Import Username ORIGINE</span>
                                        <asp:TextBox ID="txtImport_Username_ORIGINE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_15">Import Username DESTINAZIONE</span>
                                        <asp:TextBox ID="txtImport_Username_DESTINAZIONE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>
                                    
                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_16">Codice Anagrafe che identifica l'impresa in origine (id_cod)</span>
                                        <asp:TextBox ID="txtImport_CodiceImpresa_ORIGINE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_17">Codice Anagrafe che identifica l'impresa in destinazione (id_cod)</span>
                                        <asp:TextBox ID="txtImport_CodiceImpresa_DESTINAZIONE" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>
                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_18">Impostazioni Trasformazione Dati</span>
                                        <asp:TextBox ID="txtImport_ImpostazioniTrasfromazioni" runat="server" CssClass="floatSX txtUI_170" />
                                    </div>

                                </asp:Panel>
                                <div>
                                    <asp:Panel ID="panPulsantiera" runat="server">
                                        <asp:Button ID="Passaverifiche" Text="Passa a verifiche" runat="server" />
                                        <asp:Button ID="SelezionaImpianti" Text="Seleziona Impianti da Filtro" runat="server" />
                                        <hr />
                                    </asp:Panel>
                                    <div>
                                        <asp:Label ID="lblEsito" ForeColor="Red" runat="server"></asp:Label>
                                    </div>
                                </div>



                                <asp:Panel ID="pulsantieraVerifiche" runat="server">
                                    <asp:Button ID="btnAvviaVerificheQualita" runat="server" Text="Avvia verifica quantitativa" />
                                </asp:Panel>
                                <div class="clear">
                                </div>
                                <asp:GridView ID="GridView_VerificheQta" runat="server" AutoGenerateColumns="false"
                                    Width="100%" CellPadding="5" CssClass="ui-widget-content" AllowSorting="false"
                                    Caption="Selezionare le tabelle oggetto di verifica: <img src='../../AA_Immagini/Anagrafica.ico' alt='Aggiungi Colonne' id='btn_Impostazioni_Colonne' style='margin-left:10px' />">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:HyperLink ID="txtClass" runat="server" Width="120px" NavigateUrl='<%# Eval("Nome_Tabella", "G2G_Verifiche_DettaglioTabella.aspx?t={0}")%>' Text='<%# Eval("Nome_Tabella") %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>

                                        </asp:TemplateField>

                                        <asp:BoundField DataField="n_record_da_query_origine" HeaderText="n_record_da_query_origine"></asp:BoundField>
                                        <asp:BoundField DataField="n_record_da_query_destinazione" HeaderText="n_record_da_query_destinazione"></asp:BoundField>
                                        <asp:BoundField DataField="Differenza_Rilevata" HeaderText="Differenza"></asp:BoundField>

                                    </Columns>
                                    <HeaderStyle CssClass="ui-widget-header" />
                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                </asp:GridView>


                                <div class="clear">
                                </div>
                                <div>
                                    <asp:Label ID="titoloGlobali" runat="server" Text="Impostazioni globali" />
                                </div>
                                <asp:CheckBox ID="ckCac_Codifica" runat="server" Text="Codifiche e decodifica (CAC)" />
                                <asp:CheckBox ID="ckCac_Note" runat="server" Text="Note" />
                                <asp:CheckBox ID="ckCac_PDC" runat="server" Text="Piani di campionamento" />

                                <div class="clear">
                                </div>
                                <div class="BoxColore">
                                    <asp:PlaceHolder ID="tabImprese" runat="server"></asp:PlaceHolder>
                                    <input id="hidTabella" runat="server" type="hidden" />
                                    <asp:Table ID="tblImprese" runat="server">
                                    </asp:Table>
                                </div>
                              </div>


                            </div>
                         </div>
                       </div>
                    </div>









                            <!-- tab Trasferimento -->                          
                            <div class="tab-pane fade in " id="tabTrasferimento" style="overflow: auto; margin-bottom: 70px;">
                                <div class="row">                           
                                     <div class="jumbotron" style="padding-bottom: 0;">
                                         <div class="container" style="width: 100%;">                              
                                            <div class="container_testata" style="padding: 0px; /*margin-bottom: 70px*/">

                                                <asp:Panel ID="Panel3" runat="server" CssClass="boxColore">
                                                    <div class="clear">
                                                    </div>
                                                    <asp:CheckBox ID="FlagImporta_Audit" runat="server" Text="Audit"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label5" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_Audit" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label6" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_Audit" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                    
                                                    <asp:Label ID="Label41" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_Audit" runat="server"  Width="400px"></asp:TextBox>                                    
                                                    <div class="clear">
                                                    </div>                                                      
                                                    <asp:CheckBox ID="FlagImporta_Audit_Interviste" runat="server" Text="Audit Interviste"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label1" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_Audit_Interviste" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label2" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_Audit_Interviste" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label42" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_Audit_Interviste" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>
                                                    <asp:CheckBox ID="Flagimporta_catasto" runat="server" Text="Catasto"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label3" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_catasto" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label4" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_catasto" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label43" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_catasto" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>                                                     
                                                    <asp:CheckBox ID="Flagimporta_gis" runat="server" Text="Gis"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label7" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_gis" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label8" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_gis" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label44" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_gis" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div> 
                                                    <asp:CheckBox ID="Flagimporta_pianocolturale" runat="server" Text="Piano Colturale"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label9" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_pianocolturale" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label10" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_pianocolturale" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label45" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_pianocolturale" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div> 
                                                    <asp:CheckBox ID="Flagimporta_materieprime" runat="server" Text="Materie Prime"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label11" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_materieprime" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label12" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_materieprime" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label46" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_materieprime" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_agenda" runat="server" Text="Agenda"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label13" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_agenda" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label14" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_agenda" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label47" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_agenda" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>
                                                    <asp:CheckBox ID="flagimporta_pap" runat="server" Text="Pap"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label15" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_pap" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label16" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_pap" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label48" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_pap" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_papz" runat="server" Text="PapZ"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label17" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_papz" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label18" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_papz" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label49" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_papz" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_notificabio" runat="server" Text="Notifica Bio"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label19" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_notificabio" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label20" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_notificabio" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label50" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_notificabio" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="Flagimporta_profilazione" runat="server" Text="Profilazione"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label21" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_profilazione" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label22" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_profilazione" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label51" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_profilazione" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_planning" runat="server" Text="Planning"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label23" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_planning" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label24" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_planning" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label52" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_planning" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_distinta" runat="server" Text="Distinte"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label25" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_distinta" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label26" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_distinta" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label53" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_distinta" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_ricette" runat="server" Text="Ricette"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label27" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_ricette" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label28" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_ricette" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label54" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_ricette" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_pua" runat="server" Text="Pua"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label29" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_pua" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label30" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_pua" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label55" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_pua" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_piano_concimazione" runat="server" Text="Piano Concimazione"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label31" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_piano_concimazione" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label32" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_piano_concimazione" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label56" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_piano_concimazione" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_pratiche" runat="server" Text="Pratiche"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label33" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_pratiche" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label34" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_pratiche" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label57" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_pratiche" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="flagimporta_pratiche_pull" runat="server" Text="pratiche (da destinazione)"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label33p" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_pratiche_pull" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label34p" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_pratiche_pull" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label57p" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_pratiche_pull" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div> 
                                                    

                                                    <asp:CheckBox ID="Flagimporta_LineeProduttive" runat="server" Text="Linee Produttive"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label35" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_LineeProduttive" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label36" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_LineeProduttive" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label58" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_LineeProduttive" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>  
                                                    <asp:CheckBox ID="Flagimporta_piani_di_campionamento" runat="server" Text="Piani di Campionamento"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label37" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_piani_di_campionamento" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label38" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_piani_di_campionamento" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label59" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_piani_di_campionamento" runat="server"  Width="400px"></asp:TextBox> 
                                                    <div class="clear">
                                                    </div>    
                                                    <asp:CheckBox ID="Flagimporta_analisi" runat="server" Text="Analisi"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label39" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_analisi" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label40" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_analisi" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label60" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_analisi" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>    
                                                    <asp:CheckBox ID="Flagimporta_allegati" runat="server" Text="Allegati"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label61" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_allegati" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label62" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_allegati" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label63" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_allegati" runat="server"  Width="400px"></asp:TextBox>  



                                                  <%--  Controlli nascosti da preservare--%>
                                                    <asp:TextBox ID="filtrone" runat="server"  visible ="False"></asp:TextBox>  
                                                    <asp:TextBox ID="filtronerisultato" runat="server"  visible ="False"></asp:TextBox>  
                                                    <asp:TextBox ID="filtronerisultato_azienda" runat="server"  visible ="False"></asp:TextBox>  


                                                </asp:Panel>
                               
                                            </div>
                                        </div>
                                   </div>
                               </div>
                            </div>





                               

                            <!-- tab Globali -->                          
                            <div class="tab-pane fade in " id="tabGlobali" style="overflow: auto; margin-bottom: 70px;">
                                <div class="row">                           
                                     <div class="jumbotron" style="padding-bottom: 0;">
                                         <div class="container" style="width: 100%;">                              
                                            <div class="container_testata" style="padding: 0px; /*margin-bottom: 70px*/">

                                                <asp:Panel ID="Panel4" runat="server" CssClass="boxColore">
                                                    <div class="clear">
                                                    </div>
                                                    <asp:CheckBox ID="FlagAllinea_Recodes" runat="server" Text="Allinea Recodes"  Width="250px" />                                                                                                                                                                                     
                                                    <div class="clear">
                                                    </div>                                                      

                                                    <asp:CheckBox ID="Flagimporta_Note" runat="server" Text="Note"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label67" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_Note" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label68" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_Note" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label69" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_Note" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>

                                                    <asp:CheckBox ID="Flagimporta_Profilazione_Globale" runat="server" Text="Profilazione"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label70" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_Profilazione_Globale" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label71" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_Profilazione_Globale" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label72" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_Profilazione_Globale" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>                  
                                                    
                                                    <asp:CheckBox ID="Flagimporta_Cac_Codifica" runat="server" Text="Cac Codifica"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label73" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_Cac_Codifica" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label74" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_Cac_Codifica" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label75" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_Cac_Codifica" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div> 

                                                    <asp:CheckBox ID="Flagimporta_PianiCampionamento" runat="server" Text="Piani Campionamento"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label76" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_PianiCampionamento" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label77" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_PianiCampionamento" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label78" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_PianiCampionamento" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div> 


                                                    <asp:CheckBox ID="Flagimporta_Contatti" runat="server" Text="Contatti"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label79" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_Contatti" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label80" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_Contatti" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label81" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_Contatti" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>  

                                                    <asp:CheckBox ID="Flagimporta_Macchine" runat="server" Text="Macchine"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label82" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_Macchine" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label83" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_Macchine" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label84" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_Macchine" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>

                                                    <asp:CheckBox ID="Flagimporta_MateriePrimePubbliche" runat="server" Text="Materie Prime"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label82m" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_MateriePrimePubbliche" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label83m" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_MateriePrimePubbliche" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label84m" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_MateriePrimePubbliche" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>

                                                    <asp:CheckBox ID="Flagimporta_Campionature" runat="server" Text="Campionature"  Width="250px" />                                                                                                
                                                    <asp:Label ID="Label82c" runat="server" Text="Validita Inizio" />                                                       
                                                    <asp:TextBox ID="ValiditaInizio_Campionature" runat="server" CssClass="txtUI datepicker"  Width="88px"></asp:TextBox>
                                                    <asp:Label ID="Label83c" runat="server" Text="Validita Fine" />                                                       
                                                    <asp:TextBox ID="ValiditaFine_Campionature" runat="server" CssClass="txtUI datepicker" Width="88px"></asp:TextBox>                                                        
                                                    <asp:Label ID="Label84c" runat="server" Text="Configurazione" />                                                       
                                                    <asp:TextBox ID="Configurazione_Campionature" runat="server"  Width="400px"></asp:TextBox>  
                                                    <div class="clear">
                                                    </div>

                                                </asp:Panel>
                               
                                            </div>
                                        </div>
                                   </div>
                               </div>
                            </div>







                             <!-- tab XML -->                          
                            <div class="tab-pane fade in " id="tabXML" style="overflow: auto; margin-bottom: 70px;">
                                <div class="row">                           
                                     <div class="jumbotron" style="padding-bottom: 0;">
                                         <div class="container" style="width: 100%;">                              
                                            <div class="container_testata" style="padding: 0px; /*margin-bottom: 70px*/">

                                                <asp:Panel ID="Panel2" runat="server" CssClass="boxColore">

                                                    <asp:Button ID="btn_XML" runat="server" Text="Rendi Modificabile Xml"/>
                                                    <div class="clear">
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-lg-12">
                                                           <%--<asp:TextBox ID="txtConfigurazione" style="width: 100%; height: 460px; font-family: 'Courier New', sans-serif" /--%>
                                                               <asp:TextBox ID="txtConfigurazione" runat="server" ValidateRequestMode="Disabled"  CssClass="floatSX txtUI_170" style="width: 100%; height: 460px; font-family: 'Courier New', sans-serif"
                                                            Enabled="False" TextMode="MultiLine" />

                                                        </div>
                                                    </div>

                                                </asp:Panel>
                                                </div>
                                             </div>
                                         </div>
                                    </div>

                        </div>
                     
                            <!-- tab LOG -->                          
                            <div class="tab-pane fade in " id="tabLOG" style="overflow: auto; margin-bottom: 70px;">
                                <div class="row" style="padding-top: 10px;">
                                    <div class="col-lg-6">
                                        <textarea id="txtLog" style="width:100%;height:460px;font-family:'Courier New', sans-serif"></textarea>
                                    </div>
                                    <div class="col-lg-6">
                                        <textarea id="txtLogErrori" style="width:100%;height:460px;font-family:'Courier New', sans-serif"></textarea>
                                    </div> 
                                </div>
                                <div class="row">
                                    <div class="col-lg-2 col-sm-6 col-xs-12">
                                        <div class="btn btn-success" id="btn_elabora_log" onclick="elaboraLog();" style="margin-bottom:10px;">
                                            <i class="fa fa-copy"></i>Elabora LOG
                                        </div>
                                    </div>
                                </div>
                            </div>

                      </div>
                   </div>
                </div>
           </div>



            <!-- fine container -->
            <input type="hidden" id="hdPiva" runat="server" />
            <div id="details"></div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">
        var objP_server = '<%= objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var ddlConfigurazioni_ClientID = "<%= ddlConfigurazioni.ClientID %>";
        var txtConfigurazione_ClientID = "<%= txtConfigurazione.ClientID %>";
        var btn_nuovo_ClientID = "<%= btn_nuovo.ClientID %>";
        var btn_salva_ClientID = "<%= btn_salva.ClientID %>";

        function BeginRequestHandler() {
            WaitFrame.show();
        }

        function EndRequestHandler() {

            WaitFrame.hide();

            $("#btn_salvaJs").click(function () {
                $("#" + btn_salva_ClientID).click();
            });

            $("#btn_nuovoJs").click(function () {
                $("#" + btn_nuovo_ClientID).click();
            });

            //eventi di click pulsanti
            $("#btn_esporta").click(function () {
                var id_cfg = $("#" + ddlConfigurazioni_ClientID).val(); //$("#ddlConfigurazioni").data("kendoDropDownList").value();
                EsportaDatiG2G(id_cfg, "");
            });

            $("#btn_esporta_xml").click(function () {
                var xml_cfg = $("#" + txtConfigurazione_ClientID).val(); // $("#txtConfigurazione").val();
                EsportaDatiG2G(0, xml_cfg);
            });
        }
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuG2G_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuG2G.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuG2G_jQueryDocReady.js") %>"></script>

</asp:Content>
