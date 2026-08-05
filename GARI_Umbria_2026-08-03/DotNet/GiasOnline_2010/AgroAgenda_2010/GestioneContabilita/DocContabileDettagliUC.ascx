<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DocContabileDettagliUC.ascx.vb" Inherits="AgroAgenda_2010.DocContabileDettagliUC" %>
<%@ Register TagPrefix="uc" TagName="Giacenze_MagazzinoUC" Src="../GestioneMagazzini/Giacenze_MagazzinoUC.ascx" %>
<%@ Register TagPrefix="uc2" TagName="FormProdottoUC" Src="./FormProdottoUC.ascx" %>
<%@ Register TagPrefix="uc3" TagName="ImputazioneImpiantiUC" Src="./UserControl/ImputazioneImpiantiUC.ascx" %>

<div id="panelAreaDocContabileDettagli" class="panel-group searchArea" style="opacity: 0;">
    <%--
        -------------> TODO  Server ora movimento??
        
        <div class="row" style="margin-top: 10px;" id="idDataOraCausale">
        <div class="col-lg-3 col-md-3 col-sm-12">
			<div class="input-group">
				<label class="input-group-addon control-label alert-info" id="lblDataMovimento" for="idDataMovimento">Data Movimento:</label>
                <input name="idDataMovimento" id="idDataMovimento" class="kendoDatePicker" style="width: 100%;" MaxLength="10" />
			</div>
		</div>
        <div class="col-lg-3 col-md-3 col-sm-12">
			<div class="input-group">
				<label class="input-group-addon control-label alert-info" id="lblOra" for="idOra">Ora Movimento:</label>
				<input name="idOra" id="idOra" style="width: 100%;"  />
			</div>
		</div>
    </div>--%>

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="jumbotron" style="padding-bottom: 0;">
                 <div class="container" style="width: 100%;">                              
                    <div class="container_dettagli" style="padding: 0px; /*margin-bottom: 70px*/">
                        <div id="tabstrip_dettagli">
                            <ul>
                                <li class="k-state-active k-active"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Riepilogo %>" runat="server"></asp:Localize></li>
                                <li><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dettaglio %>" runat="server"></asp:Localize></li>
                                <li><asp:Localize Text="<%$ Resources: ScaricoDaGiacenza %>" runat="server"></asp:Localize></li>
                                <li><asp:Localize Text="<%$ Resources: SceltaImpianti %>" runat="server"></asp:Localize></li>
                            </ul>
                            <!-- TAB  Riepilogo -->
                            <div class="panel-group tabRiepilogo" style="display:none;" id="a_tabRiepilogo">
                               <div class="row" id="idRigheMovimenti">
                                    <div class="col-lg-12 col-md-12 col-sm-12"> 
                                        <div style="overflow: auto; margin-top: 15px;">
                                            <div id="tab_elenco_movimenti" class="gias-table-plain"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>  

                            <!-- TAB  Dettaglio Movimenti -->
                            <div class="panel-group tabDettaglioMovimenti" style="display:none;" id="a_tabDettaglioMovimenti">
                                <div class="row" id="idFormProdotto">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <uc2:FormProdottoUC id="FormProdottoUC1" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <!-- TAB  Scarico Da Giacenza -->
                            <div class="panel-group tabScaricoDaGiacenza" style="display:none;" id="a_tabScaricoDaGiacenza">
                                <div class="row" id="idScaricoDaGiacenza">
                                      <div class="col-lg-12 col-md-12 col-sm-12">
                                       <uc:Giacenze_MagazzinoUC id="Giacenze_MagazzinoUC" runat="server" />
                                    </div>
                                </div>  
                            </div>

                            <!-- TAB  Imputazione Impianti -->
                            <div class="panel-group tabImputazioneImpianti" style="display:none;" id="a_tabImputazioneImpianti">
                                <div class="row" id="idImputazioneImpianti">
                                      <div class="col-lg-12 col-md-12 col-sm-12">                             
                                         <uc3:ImputazioneImpiantiUC id="ImputazioneImpiantiUC1" runat="server" />
                                     </div>
                                </div>  
                            </div>

                            <%--             
                            <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                                <ul class="nav nav-tabs" role="tablist" id="tabs">
                                    <li><a href="#tabRiepilogo" data-toggle="tab" id="a_tabRiepilogo">Riepilogo</a></li> 
                                    <li class="active"><a href="#tabDettaglioMovimenti" data-toggle="tab" id="a_tabDettaglioMovimenti">DETTAGLIO</a></li>
                                    <li><a href="#tabScaricoDaGiacenza" data-toggle="tab" id="a_tabScaricoDaGiacenza">SCARICO DA GIACENZA</a></li>                        
                                </ul>
                             </div>
                            --%>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

         <!-- Dialog Descrizione OP -->
        <div class="modal fade" id="CD_UC_window_aggiunta_automatica1" data-backdrop="static" data-keyboard="false">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <h4 class="modal-title" id="lbl_new_item">
                            <strong><label id="titelModal" style=" font-size: 16px;"></label></strong>
                        </h4>
                    </div>

                    <div class="modal-body">
                        <div class="form-group" style="padding: 15px 0;">
                             <input name="listaCdcWbs" id="listaCdcWbs" class="from-control" style="width: 100%;"/>
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-success" id="btn_descrizione" onclick="contabDettagliUC_associaRigaCdgToProgetti();">
                            <i class="fa fa-link"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Associa %>" runat="server"></asp:Localize>
                        </button>
                    </div>
                </div>
            </div>
        </div>
</div>
<!-- fine container -->
  
<div id="confermaEliminazioneRigaDialog"></div>




  <!-- INIZIO Finestra Ricerca -->
    <div class="container-fluid"> <!--style="overflow: auto; margin-top: 10px; margin-bottom: 70px;"-->
		<div id="tab_ricercaArea" class="panel-group tab_ricercaArea" style="display:none;">
		    <!--EDIT-->
            <div class="container">
                <div class="row">
				    <div class="col-xs-12"> <!-- class="col-lg-8 col-md-8 col-sm-12"-->
					    <div style="overflow: auto; margin-top: 5px;">
                            <div id="tab_ricerca"></div>
                        </div>
				    </div>
			    </div>
            </div>
		</div>
	</div>

  <!-- FINE Finestra Ricerca -->

 
    <input type="hidden" id="hdKendo_Prodotti" runat="server" />

    <input type="hidden" id="hf_Qs_Key" runat="server" />
    <input type="hidden" id="hf_Qs_ElemCod" runat="server" />
    <input type="hidden" id="hf_Qs_OraSelezionata" runat="server" />
    <input type="hidden" id="hf_Qs_PagRitorno" runat="server" />
    <input type="hidden" id="hf_Qs_Mode" runat="server" />
    <input type="hidden" id="hf_Qs_Tipo" runat="server" />
    <input type="hidden" id="hf_Qs_CodContatto" runat="server" />
    <input type="hidden" id="hf_ChkAccompagnatoria" runat="server" />
    <input type="hidden" id="hf_xFabbricato_Cod" runat="server" />
    <input type="hidden" id="hf_contattoAziendaGias" runat="server" />
    <input type="hidden" id="hdKendo_Imballi_formProdottoUC" runat="server" />
    <input type="hidden" id="hdKendo_Ordini_formProdottoUC" runat="server" />
    <input type="hidden" id="hdKendo_DDT_formProdottoUC" runat="server" />
  


   
    <script type="text/javascript">
         
        var Qs_Key = $("#<%=hf_Qs_Key.ClientID() %>").val(); 
        var Qs_ElemCod = $("#<%=hf_Qs_ElemCod.ClientID() %>").val(); 
        var Qs_OraSelezionata = $("#<%=hf_Qs_OraSelezionata.ClientID() %>").val();
        var Qs_PagRitorno = $("#<%=hf_Qs_PagRitorno.ClientID() %>").val();
        var Qs_Mode = $("#<%=hf_Qs_Mode.ClientID() %>").val();
        var Qs_Tipo = $("#<%=hf_Qs_Tipo.ClientID() %>").val();
        var Qs_CodContatto = $("#<%=hf_Qs_CodContatto.ClientID() %>").val(); 
        var xFabbricato_Cod = $("#<%=hf_xFabbricato_Cod.ClientID() %>").val();
        var chkAccompagnatoria = $("#<%=hf_ChkAccompagnatoria.ClientID() %>").val();
        var contattoAziendaGias = $("#<%=hf_contattoAziendaGias.ClientID() %>").val();
        var hdKendo_Imballi_formProdottoUC = $("#<%=hdKendo_Imballi_formProdottoUC.ClientID() %>").val();
        var hdKendo_Ordini_formProdottoUC = $("#<%=hdKendo_Ordini_formProdottoUC.ClientID() %>").val();
        var hdKendo_DDT_formProdottoUC = $("#<%=hdKendo_DDT_formProdottoUC.ClientID() %>").val();
 
    </script>
     
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/docContabileDettagliUC_righe_movimenti_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/docContabileDettagliUC_righe_movimenti.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/docContabileDettagliUC.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/docContabileDettagliUC_jQueryDocReady.js")) %>"></script>

    <%-- Questi file non adrebbero usati, se proprio servono prima di reincluderli testare bene che non ci siano errori dovuti alle modifiche sulle lavorazioni --%>
<%--    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneMagazzini/giacenze_magazzino_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneMagazzini/giacenze_magazzino.js")) %>"></script>--%>

    <style>

        #tab_elenco_movimenti .k-grid-toolbar {
            overflow: unset;
        }

        #tab_elenco_movimenti .menuSceltaColonne {
            margin-left: 15px;
        }

        #tab_elenco_movimenti .menuSceltaColonne .k-menu-link {
            padding: 8px;
            font-size: 12px;
            font-family: 'Lato', sans-serif;
        }

        #tab_elenco_movimenti .menuSceltaColonne .k-menu-group {
            padding: 7px;
        }

        #tab_elenco_movimenti .menuSceltaColonne .k-menu-group .k-content {
            background-color: transparent;
            padding: 5px;
        }

    </style>

    <script id="templateChkSceltaColonne" type="text/x-kendo-template">
        <ul class="menuSceltaColonne">
            <li>
                <asp:Localize Text="<%$ Resources: MostraNascondiGruppiColonne %>" runat="server">Mostra/Nascondi Gruppi Colonne</asp:Localize>
                <ul>
                    <li>
                    <div id="groupChk_Mostra_PesoRiscontrato">
                        <input type="checkbox" id="chk_Mostra_PesoRiscontrato" name="chk_Mostra_PesoRiscontrato" data-type="boolean" class="k-checkbox" onclick="SceltaColonne(this)">
                        <label id="lbl_Mostra_PesoRiscontrato" style="margin-right:15px;" class="k-checkbox-label" for="chk_Mostra_PesoRiscontrato">
                            <asp:Localize Text="<%$ Resources: PesiRiscontrati %>" runat="server"></asp:Localize>
                        </label>
                    </div>
                    </li>

                    <li>
                    <div id="groupChk_Mostra_Imputazioni">
                        <input type="checkbox" id="chk_Mostra_Imputazioni" name="chk_Mostra_Imputazioni" data-type="boolean" class="k-checkbox" onclick="SceltaColonne(this)">
                        <label id="lbl_Mostra_Imputazioni" style="margin-right:15px;" class="k-checkbox-label" for="chk_Mostra_Imputazioni">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Imputazioni %>" runat="server"></asp:Localize>
                        </label>
                    </div>
                    </li>

                    <li>
                    <div id="groupChk_Mostra_Economico">
                        <input type="checkbox" id="chk_Mostra_Economico" name="chk_Mostra_Economico" checked data-type="boolean" class="k-checkbox" onclick="SceltaColonne(this)">
                        <label id="lbl_Mostra_Economico" style="margin-right:15px;" class="k-checkbox-label" for="chk_Mostra_Economico">
                            <asp:Localize Text="<%$ Resources: Economico %>" runat="server"></asp:Localize>
                        </label>
                    </div>
                    </li>

                    <li>
                    <div id="groupChk_Mostra_Quantita">
                        <input type="checkbox" id="chk_Mostra_Quantita" name="chk_Mostra_Quantita" checked data-type="boolean" class="k-checkbox" onclick="SceltaColonne(this)">
                        <label id="lbl_Mostra_Quantita" style="margin-right:15px;" class="k-checkbox-label" for="chk_Mostra_Quantita">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaQuantità %>" runat="server"></asp:Localize>
                        </label>
                    </div>
                    </li>

                    <li>
                    <div id="groupChk_Mostra_Prodotto">
                        <input type="checkbox" id="chk_Mostra_Prodotto" name="chk_Mostra_Prodotto" checked data-type="boolean" class="k-checkbox" onclick="SceltaColonne(this)">
                        <label id="lbl_Mostra_Prodotto" style="margin-right:15px;" class="k-checkbox-label" for="chk_Mostra_Prodotto">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prodotto %>" runat="server"></asp:Localize>
                        </label>
                    </div>
                    </li>
                </ul>
            </li>
        </ul>
    </script>

    <script id="templatePulsanteNuovaRiga" type="text/x-kendo-template">
        <div class="btn btn-success xi-btn-primary" id="btn_PulsanteNuovaRiga" style="margin-right: 3px;" onclick="DocContabileNuovaRiga()"
             title='<asp:Localize Text="<%$ Resources: NuovaRiga %>" runat="server"></asp:Localize>'>
            <i class="fa fa-plus"></i>
        </div>
    </script> 

    <script id="contabDettagliUC_tmplAssociaCDG" type="text/x-kendo-template">
        <button type="button" class="btn btn-success" name="contabDettagliUC_btnAssociaRigaCdgProgetti" id="contabDettagliUC_btnAssociaRigaCdgProgetti" onclick="contabDettagliUC_associaRigaCdgProgetti()">
            <i class="fa fa-link"></i><span  id="spnAssociaRigaCdgProgetti"><asp:Localize Text="<%$ Resources: AssociaCdCWBS %>" runat="server"></asp:Localize></span>
        </button>
    </script>