<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Prodotto_Edit_UC.ascx.vb" Inherits="AgroAgenda_2010.Prodotto_Edit_UC" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc1" %>

    <style type="text/css">
        .errorClass {
            border-color:#D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;               
        }

        .buttonClass {
            margin: 0 0 10px 1px;
        }

        .jumbotron {
            margin-bottom: 0 !important;
        }

        .erroreCampiObbigatori {
            border:2px solid #D41E1A;             
        }

        /*Larghezza calendario come gli altri campi di input dei Parametri Qualitativi*/
        .kendoCalendar {
            width: 100%;
        }

        .table_prodotto_Edit_UC {
            border-collapse:collapse;
            border-spacing:0;
            border:0 none;
            border:0; 
            height:auto;
            width:auto;
            POSITION:absolute;
        }

        .table_prodotto_Edit_UC td {
            border:0 none;
            padding:1px;
            text-align:left;
        }

        .prodotto_Edit_UC_infoArea {
            background-color: #FFF;
            margin: 16px;
            padding: 20px 10px;
            border-radius: 5px;
        }
     
        .prodotto_Edit_UC_searchArea {
            margin: 5px;
            padding: 10px;
        }
        .prodotto_Edit_UC_searchArea .jumbotron {
            padding: 0 15px 0 15px;
        }
        .tab-pane .jumbotron {
            display: inline;
        }
        .tab-pane.DatiTecnici .jumbotron div, .prodotto_Edit_UC_searchArea .jumbotron {
            border-width: 0 !important;
            box-shadow: none;
        }
        .tab-pane .k-grid { box-shadow: none; }
        .border_no { 
            box-shadow: none;
            border-width: 0;
            padding-left: 0;
        }
        
    </style>

    <div class="panel-group prodotto_Edit_UC_searchArea" style="display: none;">
            <div class="jumbotron" style="padding-top: 30px;">
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
						    <div class="form-group">
								<div class="input-group">
									<label class="input-group-addon alert-info" for="id_ddl_prodotto_Edit_UC_Categorie">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaProdotti %>" runat="server"></asp:Localize>:
									</label>
									<input type="text" id='ddl_prodotto_Edit_UC_Categorie' class="form-control" />
								</div>
							</div>
						</div>
                    </div>
                   <div class="col-lg-5 col-md-5 col-sm-12 gias-pt-15px">
                       <div class="row-radio-group" id="FiltroRegGroup">
                           <label class="lbl_required" id="lbl_filtro_reg" for="FiltroRegGroup">
                               <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Regolamento %>" runat="server"></asp:Localize>:
                           </label>
                           <div class="radio-single">
                               <input id="FiltroReg_TUTTI" name="FiltroReg" value="0" class="k-radio" type="radio" />
                               <label class="k-radio-label" for="FiltroReg_TUTTI">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tutti %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                           <div class="radio-single">
                               <input id="FiltroReg_CONV" name="FiltroReg" value="1" class="k-radio" type="radio" />
                               <label class="k-radio-label" for="FiltroReg_CONV">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Convenzionale %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                           <div class="radio-single">
                               <input id="FiltroReg_BIO" name="FiltroReg" value="4" class="k-radio" type="radio" />
                               <label class="k-radio-label" for="FiltroReg_BIO">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Biologico %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                       </div>
                    
                       <div class="row-radio-group" id="FiltroVisiGroup">
                           <label class="lbl_required" id="lbl_filtro_visibilita" for="FiltroVisiGroup">
                               <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Visibilità %>" runat="server"></asp:Localize>:
                           </label>
                           <div class="radio-single">
                               <input id="FiltroVisi_TUTTI" name="FiltroVisi" value="-99" class="k-radio" type="radio" />
                               <label class="k-checkbox-label" for="FiltroVisi_TUTTI">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tutti %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                           <div class="radio-single">
                               <input id="FiltroVisi_Privato" name="FiltroVisi" value="0" class="k-radio" type="radio" />
                               <label class="k-checkbox-label" for="FiltroVisi_Privato">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Privato %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                           <div class="radio-single">
                               <input id="FiltroVisi_Pubblico" name="FiltroVisi" value="-1" class="k-radio" type="radio" />
                               <label class="k-checkbox-label" for="FiltroVisi_Pubblico">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Pubblico %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                       </div>

                       <div class="row-radio-group" id="FiltroValorizGroup">
                           <label class="lbl_required" id="lbl_filtro_valorizzati" for="FiltroValorizGroup">
                               <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Valorizzati %>" runat="server"></asp:Localize>:
                           </label>
                           <div class="radio-single">
                               <input id="FiltroValoriz_TUTTI" name="FiltroValoriz" value="-1" class="k-radio" type="radio" />
                               <label class="k-checkbox-label" for="FiltroValoriz_TUTTI">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tutti %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                           <div class="radio-single">
                               <input id="FiltroValoriz_Si" name="FiltroValoriz" value="1" class="k-radio" type="radio" />
                               <label class="k-checkbox-label" for="FiltroValoriz_Si">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Si %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                           <div class="radio-single">
                               <input id="FiltroValoriz_No" name="FiltroValoriz" value="0" class="k-radio" type="radio" />
                               <label class="k-checkbox-label" for="FiltroValoriz_No">
                                   <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, No %>" runat="server"></asp:Localize>
                               </label>
                           </div>
                       </div>
                    </div>
				</div>

                <div class="row">
                    <%--Ricerca per descrizione--%>
                    <div class="col-lg-6 col-md-6 col-sm-12" style="padding-top: 30px;">
                       <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_descrizione_prodotto">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server"></asp:Localize>/
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceArticoloAbbr %>" runat="server"></asp:Localize>:
                                    </span>
                                    <input type="text" id="txt_descrizione_prodotto" class="form-control " />      
                                    <label id="lbl_prodotto_Edit_UC_descrizione_3_caratteri" for="txt_descrizione_prodotto" class="input-group-addon ">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InserireTreCaratteriDellaDescrizione %>" runat="server"></asp:Localize>
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-5 col-md-5 col-sm-12" style="padding-top: 30px;">
						<div class="form-horizontal">
							<div class="form-group">
								<div id="ddl_Ricerca_XCategCommle" class="input-group">
                                    <label class="input-group-addon alert-info " for="multiselCategCommle">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaCommerciale %>" runat="server"></asp:Localize>:
                                    </label>
									<select name="multiselCategCommle" multiple="multiple" ID="multiselCategCommle" class="form-control"></select>
								</div>
							</div>
						</div>
                    </div>
				</div>

                <div class="row">
                        <div class="col-lg-6 col-md-6 col-sm-12" style="padding-top: 10px;">
							<div class="form-horizontal">
								<div class="form-group">
									<div id="ddl_Ricerca_XSpecie" class="input-group">
                                        <label class="input-group-addon alert-info " for="multiselSpecie">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Specie %>" runat="server"></asp:Localize>:
                                        </label>
                                        <select name="multiselSpecie" multiple="multiple" ID="multiselSpecie" class="form-control"></select>
									</div>
								</div>
							</div>
                        </div>
                        <div class="col-lg-5 col-md-5 col-sm-12" style="padding-top: 10px;">
							<div class="form-horizontal">
								<div class="form-group">
									<div id="ddl_Ricerca_XVarieta" class="input-group">
                                        <label class="input-group-addon alert-info " for="multiselVarieta">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server"></asp:Localize>:
                                        </label>
										<select name="multiselVarieta" multiple="multiple" ID="multiselVarieta" class="form-control"></select>
									</div>
								</div>
							</div>
                        </div>
                </div>

                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12 text-right">
                        <div class="btn btn-success xonne-btn-primary" id="prodotto_Edit_UC_Ricerca" style="margin-bottom: 20px;">
                            <i class="fa fa-search"></i>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server"></asp:Localize>
                        </div>
                    </div>
                </div>
            </div>
    </div>

    <div class="panel-group prodotto_Edit_UC_infoArea" style="display: none;">

            <!--Errore campi non completati in Nuovo-->
            <div class="row" id="div_riepilogo_error" style="margin-bottom: 25px;">
                <div class="col-lg-12 col-md-12">
                    <b>
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CompilareISeguentiCampi %>" runat="server"></asp:Localize>:</b>
                    <br />
                    <br />
                </div>
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <ul id="div_riepilogo_error_elenco">
                        <li class="voce_1" style="display: none;">
                            <asp:Localize meta:resourcekey="ObbligatorioCodiceProdotto" runat="server"></asp:Localize></li>
                        <li class="voce_2" style="display: none;">
                            <asp:Localize meta:resourcekey="ObbligatorioDescrizione" runat="server"></asp:Localize></li>
                        <li class="voce_3" style="display: none;">
                            <asp:Localize meta:resourcekey="ObbligatorioCodiceEsterno" runat="server"></asp:Localize></li>
                    </ul>
                </div>
            </div>

             <div class="panel-area prodotto_Edit_UC_informazioni">               
                <div class="row">
                    <!--Drop Down Categoria Prodotto-->
                    <div class="col-lg-8 col-md-8 col-sm-12" id="prodotto_Edit_UC_modifyArea">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group"> 
                                    <span class="input-group-addon alert-info" id="lbl_prodotto_UC_categ_prod">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaProdotto %>" runat="server"></asp:Localize>:</span>
                                    <input type="text" id='ddl_prodotto_UC_categ_prod' class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!--Switch Alias(parte GIAS LAN)-->
                    <div class="alias col-lg-4 col-md-4 col-sm-12">
                       <div class="input-group">
                            <label class="lbl_required" id="lbl_prodotto_UC_alias" for="cb_prodotto_UC_alias">
                                <asp:Localize Text="<%$ Resources: ProdottoPerDescrizioniAlternative %>" runat="server"></asp:Localize>:
                            </label>
                            <input type="checkbox" id="cb_prodotto_UC_alias" name="cb_prodotto_UC_alias" class="kendoSwitch"/>
                       </div>
                    </div>
                </div>

                <!--TextBox Codice Prodotto-->
                <div class="row">
                    <div class="col-lg-7 col-md-7 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_prodotto_UC_cod_prod">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceProdotto %>" runat="server"></asp:Localize>:
                                    </span>
                                    <input type="text" id="txt_prodotto_UC_cod_prod" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!--TextBox Codice Esterno-->
                    <div class="col-lg-4 col-md-4 col-sm-12">
                        <div id="codice_esterno" class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_prodotto_UC_cod_est">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceEsterno %>" runat="server"></asp:Localize>:
                            </span>
                            <input type="text" id="txt_prodotto_UC_cod_est" class="form-control" onChange="txt_prodotto_UC_cod_est_change()" disabled />
                        </div>
                    </div>
                </div>

                <!--TextBox Descrizione-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_prodotto_UC_Descrizione">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server"></asp:Localize> *:
                                    </span>
                                    <input type="text" id="txt_prodotto_UC_Descrizione" class="form-control" />
                                </div>
                            </div>
                       </div>
                    </div>
                    <!--Kendo Switch Componi Descrizione-->
                    <div id="switch_prodotto_UC_componi_descrizione" class="col-lg-4 col-md-4 col-sm-12" >
                        <label class="lbl_required" id="lbl_prodotto_UC_componi_descrizione" for="cb_prodotto_UC_componi_descrizione" >
                            <asp:Localize Text="<%$ Resources: ComponiDescrizioneInAutomatico %>" runat="server"></asp:Localize>:
                        </label>
                        <input type="checkbox" id="cb_prodotto_UC_componi_descrizione" name="cb_prodotto_UC_componi_descrizione" class="kendoSwitch"/>
                    </div>

                <!--Drop Down Categoria Commerciale(parte GIAS LAN)-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="categoria_risorsa input-group"> 
                                    <span class="input-group-addon alert-info" id="lbl_prodotto_UC_categ_ris">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaCommerciale %>" runat="server"></asp:Localize>:
                                    </span>
                                    <input type="text" id='ddl_prodotto_UC_categ_ris' class="prodotto_UC_categ_ris form-control txtUI required" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--Drop Down Linea Produzione(parte GIAS LAN)-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="linea_produzione input-group"> 
                                    <span class="input-group-addon alert-info" id="lbl_prodotto_UC_linea_prod">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, LineaProduzione %>" runat="server"></asp:Localize>:</span>
                                    <input type="text" id='ddl_prodotto_UC_linea_prod' class="prodotto_UC_linea_prod form-control txtUI required" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                </div>
           </div>
                <!-- Fine area con info generali-->
    </div>
    <div class="panel-group prodotto_Edit_UC_modifyArea" style="display: none;">               
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">

                    <ul class="nav nav-tabs" role="tablist" id="Prodotto_Edit_UC_tabs">
                        <li class="tabDatiTecnici active"><a href="#tab_prodotto_UC_dati_tecnici" data-toggle="tab" id="a_tab_prodotto_UC_dati_tecnici">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiGenerali %>" runat="server"></asp:Localize></a></li>
                        <li class="tabConfigurazione"><a href="#tab_prodotto_UC_configurazione" data-toggle="tab" id="a_tab_prodotto_UC_configurazione">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Configurazione %>" runat="server"></asp:Localize></a></li>
                        <li class="tabStoricoPrezzi"><a href="#tab_prodotto_UC_storico_prezzi" data-toggle="tab" id="a_tab_prodotto_UC_storico_prezzi">
                            <asp:Localize Text="<%$ Resources: StoricoPrezzi %>" runat="server"></asp:Localize></a></li>
                        <li class="tabParametriQualitativi"><a href="#tab_prodotto_UC_parametri_qualitativi" data-toggle="tab" id="a_tab_prodotto_UC_parametri_qualitativi">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriQualitativi %>" runat="server"></asp:Localize></a></li>
                        <li class="tabDatiContabili"><a href="#tab_prodotto_UC_dati_contabilita" data-toggle="tab" id="a_tab_prodotto_UC_dati_contabilita">
                            <asp:Localize Text="<%$ Resources: DatiContabilità %>" runat="server"></asp:Localize></a></li>
                        <li class="tabTraduzioni"><a href="#tab_prodotto_UC_traduzioni" data-toggle="tab" id="a_tab_prodotto_UC_traduzioni">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Traduzioni %>" runat="server"></asp:Localize></a></li>
                        <li class="tabAltriDati"><a href="#tab_prodotto_UC_altri_dati" data-toggle="tab" id="a_tab_prodotto_UC_altri_dati">
                            <asp:Localize Text="<%$ Resources: DatiVenditaDettaglio %>" runat="server"></asp:Localize></a></li>
                        <li class="tabAlias"><a href="#tab_prodotto_UC_alias" data-toggle="tab" id="a_tab_prodotto_UC_alias">
                            <asp:Localize Text="<%$ Resources: DescrizioniAlternative %>" runat="server"></asp:Localize></a></li>
                    </ul>
				
                    <div class="tab-content">
                        <div class="tab-pane DatiTecnici fade in active" id="tab_prodotto_UC_dati_tecnici" style="overflow: auto; margin-bottom: 70px;">

                      <div class="jumbotron">  

                       <!--Inizio Dati Colturali-->
                       <div class="dati_colturali col-lg-12 border_si border_no">
                        <div class="row">
                            <div id="titolo_dati_cul" class="col-md-12">
                                <h4 style="color: #052747; text-transform: uppercase;">
                                    <asp:Localize Text="<%$ Resources: DatiColturali %>" runat="server"></asp:Localize></h4>
                            </div>
                            <div id="titolo_dati_zoo" class="col-md-12" style="display:none">
                                <h4 style="color: #052747; text-transform: uppercase;">
                                    <asp:Localize Text="<%$ Resources: DatiZootecnici %>" runat="server"></asp:Localize></h4>
                            </div>
                        </div>

                        <div class="row">

                            <!--Drop Down Sementi e materiale vivaisti-->
                            <div id="sementi_materiale" class="col-lg-4 col-md-4 col-sm-12">
                                <div class="input-group ">
                                    <span class="input-group-addon alert-info " id="lbl_prodotto_UC_sementi_materiale">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SementeMaterialeVivaistico %>" runat="server"></asp:Localize>:
                                    </span>
                                    <input type="text" id="ddl_prodotto_UC_sementi_materiale" class="form-control " />
                                </div>
                            </div>

                           <!--Drop Down Specie Vegetale e Drop Down Specie Animale-->
                            <div class="col-lg-5 col-md-5 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="specie_vegetale input-group ">
                                            <span class="input-group-addon alert-info" id="lbl_prodotto_UC_specie_veg">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpecieVegetale %>" runat="server"></asp:Localize>:</span>
                                            <input type="text" id="ddl_prodotto_UC_specie_veg" class="form-control " />
                                        </div>
                                        <div class="specie_animale input-group " style="display:none">
                                            <span class="input-group-addon alert-info" id="lbl_prodotto_UC_specie_anim">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpecieAnimale %>" runat="server"></asp:Localize>:</span>
                                            <input type="text" id="ddl_prodotto_UC_specie_anim" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                            <!--Drop Down Varietà Colturale e Drop Down Indirizzo Produttivo Animale-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="varieta_colturale input-group">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_varieta">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, VarietàColturale %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="ddl_prodotto_UC_varieta" class="form-control " />
                                            </div>
                                            <div class="indirizzo_produttivo input-group " style="display:none">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_indi_produt">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, IndirizzoProduttivo %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="ddl_prodotto_UC_indi_produt" class="form-control " />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!--Drop Down Tipologia Varietale e Drop Down Razza Animale-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="tipologia_varietale input-group">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_tipo_varietale">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipologiaVarietale %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="ddl_prodotto_UC_tipo_varietale" class="form-control " />
                                            </div>
                                            <div class="razza_animale input-group " style="display:none">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_razza_anim">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Razza %>" runat="server"></asp:Localize>:
                                                </span>
                                                <input type="text" id="ddl_prodotto_UC_razza_anim" class="form-control " />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!--Drop Down Regolamento-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="regolamento input-group">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_Regolamento">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Regolamento %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="ddl_prodotto_UC_Regolamento" class="form-control " />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                              <!--Drop Down Prodotto Base di Riferimento-->
                            <div id="prodotto_base" class="row" style="display:none">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class=" input-group ">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_Prodottobase">
                                                    <asp:Localize Text="<%$ Resources: ProdottoBaseDiRiferimento %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="ddl_prodotto_UC_Prodottobase" class="form-control " />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!--Drop Down Finalità Produttiva (parte GIAS LAN)-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="ddl_prodotto_UC_final_prod input-group"> 
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_final_prod">
                                                    <asp:Localize Text="<%$ Resources: FinalitàProduttiva %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id='ddl_prodotto_UC_final_prod' class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div hidden class="tecnologieSementi input-group"> 
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_TecnologieSementi">
                                                    <asp:Localize Text="<%$ Resources: TecnologieSementi %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id='ddl_prodotto_UC_TecnologieSementi' class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                           <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div hidden class="germinabilita input-group"> 
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_germinabilita">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Germinabilita %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" name="txt_prodotto_UC_germinabilita" id='txt_prodotto_UC_germinabilita' class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                           </div>
                          <!--Fine Dati Colturali-->

                          <!--Referenze parametri Qualitativi-->
                          <div class="Referenze_Param_Qual col-lg-12 border_si" style="display:none">
                            <div class="row">
                                <div class="col-md-12">
                                    <h4 style="color: #052747; text-transform: uppercase;">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriQualitativi %>" runat="server"></asp:Localize></h4>
                                </div>
                            </div>
                            <div class="row" id="prodotto_UC_parametri_qualitativi_list" style="margin-top: 5px;">
                             <!-- N.B. Riempita dinamicamente  -->
                            </div>
                          </div>
                          <!--Fine Referenze-->


                          <!--Inzio Dettagli-->
                          <div class="dettagli col-lg-12 border_si" style="display:none">
                            <div class="row">
                                <div class="col-md-12">
                                    <h4 style="color: #052747; text-transform: uppercase;">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dettagli %>" runat="server"></asp:Localize></h4>
                                </div>
                            </div>
                             <div class="row">
                                 <div class="col-lg-6 col-md-6 col-sm-12" style="padding-left: 80px">
                                    <input type="checkbox" id="chk_prodotto_UC_agricoltura_biologica" class="dettagli k-checkbox" />
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_agricoltura_biologica">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AgricolturaBiologica %>" runat="server"></asp:Localize></label>
                                 </div>
                                 <div class="col-lg-6 col-md-6 col-sm-12" style="padding-top: 10px">
                                    <input type="checkbox" id="chk_prodotto_UC_origine_non_agri" class="dettagli k-checkbox"/>
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_origine_non_agri">
                                        <asp:Localize Text="<%$ Resources: OrigineNonAgricola %>" runat="server"></asp:Localize></label>
                                </div>
                           </div>
                             <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-12" style="padding-left: 80px">
                                    <input type="checkbox" id="chk_prodotto_UC_agricoltura_convezionale" class="dettagli k-checkbox"/>
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_agricoltura_convezionale" >
                                        <asp:Localize Text="<%$ Resources: AgricolturaConvenzionale %>" runat="server"></asp:Localize></label>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12" style="padding-top: 10px">
                                    <input type="checkbox" id="chk_prodotto_UC_ausi_fabbr" class="dettagli k-checkbox"/>
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_ausi_fabbr" >
                                        <asp:Localize Text="<%$ Resources: AusiliareDiFabbricazione %>" runat="server"></asp:Localize></label>
                                </div>  
                          </div>
                         </div>
                          <!--Fine Dettagli-->

                          <div class="informazioni_aggiuntive col-lg-12 border_si">
                            <br/>
                            <!--Drop Down Unità Misura (parte GIAS LAN)-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group"> 
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_unita_mis_def">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, UnitàdiMisura %>" runat="server"></asp:Localize> 
                                                    Default:</span>
                                                <input type="text" id='ddl_prodotto_UC_unita_mis_def' class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!--TextBox Descrizione Addizionale (parte GIAS LAN)-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_descr_add">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DescrizioneAddizionale %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="txt_prodotto_UC_descr_add" class="form-control " />
                                            </div>
                                        </div>
                                    </div>
                                 </div>
                            </div>

                              <!--Drop Down Ditta di Provenienza-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="ditta_di_provenienza input-group ">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_ditta_di_provenienza">
                                                    <asp:Localize Text="<%$ Resources: DittaDiProvenienza %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="ddl_prodotto_UC_ditta_di_provenienza" class="form-control " />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!--TextArea Note-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="lbl_prodotto_UC_Note" for="txt_prodotto_UC_Note">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server"></asp:Localize>:</label>
                                                    <textarea id="txt_prodotto_UC_Note" class="form-control k-content" rows="3"></textarea>
                                                </div>
                                            </div>
                                        </div>
                                </div>
                            </div>

                              <div class="row">
                                  <div class="col-md-8 col-sm-12">
                                      <div class="form-group">
                                          <div class="input-group">
                                              <label class="input-group-addon alert-info" id="lbl_prodotto_UC_mat_prima_priorita_cdg" for="ddl_prodotto_UC_mat_prima_priorita_cdg">
                                                  <asp:Localize Text="<%$ Resources: PrioritàProdottoPerCostiDiGestione %>" runat="server"></asp:Localize>:</label>
                                              <select class="form-group" name="ddl_prodotto_UC_mat_prima_priorita_cdg" id="ddl_prodotto_UC_mat_prima_priorita_cdg"></select>
                                          </div>
                                      </div>
                                  </div>
                              </div>

                          </div>

                          <!--Inizio Visibilità-->
                         <div class="col-lg-12 border_si">
                             <div class="row">
                                <div class="col-md-12">
                                    <h4 style="color: #052747; text-transform: uppercase;">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Visibilità %>" runat="server"></asp:Localize></h4>
                                </div>
                            </div>
                            <!--Kendo Switch per Materia Prima/Lavorato-->
                            <div class="col-lg-3 col-md-3 col-sm-12" >
                                <label class="lbl_required" id="lbl_prodotto_UC_materia_prima" for="lbl_prodotto_UC_materia_prima" >
                                    <asp:Localize Text="<%$ Resources: MateriaPrimaMovimentabileDaTutteLeImprese %>" runat="server"></asp:Localize>:</label>
                                <input type="checkbox" id="cb_prodotto_UC_materia_prima" name="cb_prodotto_UC_materia_prima" class="kendoSwitch"/>
                            </div>

                            <!--TextBox Impresa Referente-->
                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_impresa_ref">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImpresaReferente %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="txt_prodotto_UC_impresa_ref" class="form-control " disabled/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                          </div>
                           <!--Fine Visibilità-->
                       </div>                            
                     </div>

                        <div class="tab-pane Configurazione fade in" id="tab_prodotto_UC_configurazione" style="overflow: auto; margin-bottom: 70px;">
                            <div class="jumbotron">
                                <div class="row">
                                    <div class="col-lg-6 col-md-6 col-sm-12" style="padding-bottom: 0px">
                                        <!--CheckBox Prodotto Confezionato / Impostazione Pesi (fa parte del GIAS LAN)-->
                                        <div class="input-group ">
                                            <input type="checkbox" id="chk_prodotto_UC_Udm_Cod_Extra" class="k-checkbox" onclick="prodotto_Edit_UC_Udm_Cod_Extra()"/>
                                            <label class="k-checkbox-label" for="chk_prodotto_UC_Udm_Cod_Extra">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ProdottoConfezionato %>" runat="server"></asp:Localize></label>
                                        </div>
                                    </div>
                                </div>

                                <div class="configurazione_base col-lg-12 border_si">
                                    <br/>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <!--Dropdown tipo Default (fa parte del GIAS LAN)-->
                                            <div class="input-group ">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_tipo_default"> 
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipo %>" runat="server"></asp:Localize> Default:</span>
                                                <input type="text" id="ddl_prodotto_UC_tipo_default" class="form-control " />
                                            </div>
                                         </div>
                                      </div>
                                     <div class="row">
                                        <!--Dropdown Unita di Misura Aspetto Bene (fa parte del GIAS LAN)-->
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="input-group ">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_cod_extra"> <asp:Localize Text="<%$ Resources: UdmAspettoBene %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="ddl_prodotto_UC_cod_extra" class="form-control " />
                                            </div>
                                         </div>
                                        <div class="col-lg-2 col-md-2 col-sm-12">
                                            &nbsp;
                                         </div>
                                         <!--Dropdown e TextBox Peso (fa parte del GIAS LAN)-->
                                         <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="input-group ">
                                                <input type="text" id="ddl_prodotto_UC_peso" class="form-control "/>
                                                <span class="input-group-btn" style="width:0px;"></span>
                                                <input type="text" id="txt_prodotto_UC_Qta_Extra" class="form-control" />
                                            </div>                             
                                         </div>
                                      </div>
                                     <div class="row">
                                        <!--Dropdown Set (fa parte del GIAS LAN)-->
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="input-group ">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_set"> Set:</span>
                                                <input type="text" id="ddl_prodotto_UC_set" class="form-control " />
                                            </div>
                                         </div>
                                         <div class="col-lg-2 col-md-2 col-sm-12">
                                            &nbsp;
                                         </div>
                                         <!--TextBox tara Nominale (fa parte del GIAS LAN)-->
                                         <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="input-group ">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_tara"> <asp:Localize Text="<%$ Resources: TaraNominaleKg %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="txt_prodotto_UC_tara_nomi" class="form-control" />
                                            </div>
                                         </div>
                                      </div>
                                     <div class="row">
                                      <!--Dropdown Unita di Misura Base Gias FF (fa parte del GIAS LAN)-->
                                        <div class="Confezione_Base col-lg-6 col-md-6 col-sm-12">
                                            <div class="input-group ">
                                                <span class="input-group-addon alert-info" id="lbl_prodotto_UC_Confezione_Base"> <asp:Localize Text="<%$ Resources: UdmBaseGiasFF %>" runat="server"></asp:Localize>:</span>
                                                <input type="text" id="ddl_prodotto_UC_Confezione_Base" class="form-control " />
                                            </div>
                                         </div>
                                      </div>

                                     <div class="row">
                                      <!--CheckBox e TextBox Beni Contenuti (fa parte del GIAS LAN)-->
                                        <div class="Qta_Contenitore_Conf col-lg-12 col-md-12 col-sm-12">
                                            <div class="Qta_Contenitore_Conf input-group ">
                                                <input type="checkbox" id="chk_prodotto_UC_Qta_Contenitore_Conf" class="k-checkbox" onclick="prodotto_UC_Qta_Contenitore_Conf()"/>
                                                <label class="k-checkbox-label" for="chk_prodotto_UC_Qta_Contenitore_Conf">
                                                    <asp:Localize Text="<%$ Resources: NumeroBeniContenuti %>" runat="server"></asp:Localize>:</label>
                                                <input type="text" id="txt_prodotto_UC_Qta_Contenitore_Conf" class="form-control" />
                                           </div>
                                         </div>
                                     </div>

                                     <br/>
                                     <br/>

                                    <!--CheckBox Imposta Aspetto come unita di misura principale (fa parte del GIAS LAN)-->
                                     <div class="row">
                                         <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="input-group ">
                                                <input type="checkbox" id="chk_prodotto_UC_imposta_flag_extra" class="k-checkbox" />
                                                <label class="k-checkbox-label" for="chk_prodotto_UC_imposta_flag_extra">
                                                    <asp:Localize Text="<%$ Resources: ImpostaUDMAspettoBeneComePrincipale %>" runat="server"></asp:Localize></label>
                                            </div>
                                         </div>
                                      </div>
                               </div>
                            <br/><br/>
                            <!--CheckBox Imposta il Peso Variato come Principale (fa parte del GIAS LAN)-->
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-12" style="padding-top: 10px">
                                <div class="input-group ">
                                    <input type="checkbox" id="chk_prodotto_UC_imposta_flag_Variazione" class="k-checkbox" />
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_imposta_flag_Variazione">
                                        <asp:Localize Text="<%$ Resources: ImpostaPesoVariatoComePrincipale %>" runat="server"></asp:Localize></label>
                                </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-12" style="padding-top: 10px">
                                <div class="input-group " id ="checkbox_composizioneLotto">
                                    <input type="checkbox" id="chk_prodotto_UC_composizioneLotto" class="k-checkbox" onclick="Nascondi_Mostra_Filtro_Specie_Varieta()"/>
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_composizioneLotto">
                                        <asp:Localize Text="<%$ Resources: UtilizzaPerComposizioneLottoEntrata %>" runat="server"></asp:Localize></label>
                                </div>
                                </div>
                            </div>
                                
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-12" style="padding-top: 10px">
                                <div class="input-group " id ="checkbox_utilizzoCA">
                                    <input type="checkbox" id="chk_prodotto_UC_utilizzoCA" class="k-checkbox"/>
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_utilizzoCA">
                                        <asp:Localize Text="<%$ Resources: UtilizzabileInContrattiAffitto %>" runat="server"></asp:Localize></label>
                                </div>
                                </div>
                            </div>

                            <br/>
                          <!--Inzio Dettagli Bene Confezionamento-->
                          <div id="dettagli_beni_di_confezionamento" class="bene_conf col-lg-12 border_si">
                            <div class="row">
                                <div class="col-md-12">
                                    <h4 style="color: #052747; text-transform: uppercase;">
                                        <asp:Localize Text="<%$ Resources: DettagliBeneConfezionamento %>" runat="server"></asp:Localize></h4>
                                </div>
                            </div>
                            <div class="row">
                                 <!--CheckBox Imballagio , contenitore e confezione-->
                                 <div id="checkbox_imballaggio" class="col-lg-2 col-md-2 col-sm-12">
                                    <input type="checkbox" id="chk_prodotto_UC_imballaggio" class=" k-checkbox" onclick="Nascondi_Mostra_Filtro_Specie_Varieta()"/>
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_imballaggio">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Imballaggio %>" runat="server"></asp:Localize></label>
                                 </div>
                                 <div id="checkbox_contenitore" class="col-lg-2 col-md-2 col-sm-12">
                                    <input type="checkbox" id="chk_prodotto_UC_contenitore" class="k-checkbox" onclick="prodotto_Edit_UC_contenitore()" />
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_contenitore">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Contenitore %>" runat="server"></asp:Localize></label>
                                 </div>
                                 <div id="checkbox_confezione"class="col-lg-2 col-md-2 col-sm-12" style="display:none">
                                    <input type="checkbox" id="chk_prodotto_UC_confezione" class=" k-checkbox" onclick="Nascondi_Mostra_Filtro_Specie_Varieta()"/>
                                    <label class="k-checkbox-label" for="chk_prodotto_UC_confezione">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Confezione %>" runat="server"></asp:Localize></label>
                                 </div>
                           </div>
                            <br/>
                             <div class="row">
                                <!--TextBox Numero Beni Contenuti-->
                                <div class="Qta_Contenitore col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="prodotto_UC_lblQta_Contenitore" >
                                                    <asp:Localize Text="<%$ Resources: NumeroBeniContenuti %>" runat="server"></asp:Localize></span>
                                                <input type="number" id="txt_prodotto_UC_Qta_Contenitore" class="form-control " min="0"/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                             <div class="row">
                                <!--Dropdown categoria-->
                                <div id="categoria_dettagli_beni_conf" class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblconfezioni" for="ddl_prodotto_UC_confezioni">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Categoria %>" runat="server"></asp:Localize>:</label>
                                                    <input name="ddl_prodotto_UC_confezioni" id="ddl_prodotto_UC_confezioni" class="form-control" />
                                                </div>
                                            </div>                                      
                                    </div>
                                </div>
                            </div> 
                              <div id="filtri_dett_beni_conf_veg">
                                  <div class="row">
                                      <div class="col-md-12">
                                          <h6 style="color: #052747; text-transform: uppercase;">
                                              <asp:Localize Text="<%$ Resources: UtilizzabileCon %>" runat="server"></asp:Localize>:</h6>
                                      </div>
                                  </div>
                                  <div class="row">                                     
                                      <!--Multiselect specie per dettagli beni di confezionamento-->
                                      <div class="col-lg-6 col-md-6 col-sm-12">
                                          <div class="form-horizontal">
                                              <div class="form-group">
                                                  <div class="input-group">
                                                      <label class="input-group-addon alert-info" id="prodotto_UC_lblspecie_dett_beni">
                                                          <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpecieVegetale %>" runat="server"></asp:Localize>:</label>
                                                      <select name="multisel_prodotto_UC_specie_dett_beni" multiple="multiple" id="multisel_prodotto_UC_specie_dett_beni" class="form-control"></select>
                                                  </div>
                                              </div>
                                          </div>
                                      </div>
                                  </div>
                                  <div id="multisel_varieta_beni_conf_veg" class="row">
                                      <!--Multiselect varieta per dettagli beni di confezionamento-->
                                      <div class="col-lg-6 col-md-6 col-sm-12">
                                          <div class="form-horizontal">
                                              <div class="form-group">
                                                  <div class="input-group">
                                                      <label class="input-group-addon alert-info" id="prodotto_UC_lblvarieta_dett_beni" for="multisel_prodotto_UC_varieta_dett_beni">
                                                          <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server"></asp:Localize>:</label>
                                                      <select name="multisel_prodotto_UC_varieta_dett_beni" multiple="multiple" id="multisel_prodotto_UC_varieta_dett_beni" class="form-control"></select>
                                                  </div>
                                              </div>
                                          </div>
                                      </div>
                                  </div>
                              </div>
                         </div>
                          <!--Fine Dettagli Bene Confezionamento-->

                          <br/>
                          <br/>

                         <!--CheckBox escludi da gestione Preparazioni (fa parte del GIAS LAN)-->
                         <div class="col-lg-12 border_si border_no">
                          <div class="row">
                              <div class="col-lg-6 col-md-6 col-sm-12" style="display:none">
                                <div class="input-group ">
                                  <input type="checkbox" id="chk_prodotto_UC_escludi_da_preparazioni" class="k-checkbox" />
                                  <label class="k-checkbox-label" for="chk_prodotto_UC_escludi_da_preparazioni">
                                      <asp:Localize Text="<%$ Resources: EscludiDaGestionePreparazioni %>" runat="server"></asp:Localize></label>
                               </div>
                             </div>
                          <!--CheckBox escludi da movimentazioni magazzino (fa parte del GIAS LAN)-->
                             <div class="col-lg-6 col-md-6 col-sm-12 gias-p-x-30px gias-p-x-15-2023" >
                                <div class="input-group ">
                                  <input type="checkbox" id="chk_prodotto_UC_escludi_da_magazzino" class="k-checkbox" />
                                  <label class="k-checkbox-label" for="chk_prodotto_UC_escludi_da_magazzino">
                                      <asp:Localize Text="<%$ Resources: EscludiDaMovimentazioniMagazzino %>" runat="server"></asp:Localize></label>
                               </div>
                             </div>
                         </div>
                        </div>
                        </div>
                       </div>

                        <div class="tab-pane StoricoPrezzi fade in" id="tab_prodotto_UC_storico_prezzi" style="overflow: auto; margin-bottom: 70px;">

                            <div class="jumbotron">
                                 <div class="row">
                                    <!-- Griglia Storico Prezzi -->
                                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="prodotto_UC_griglia_storico_prezzi"></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="tab-pane ParametriQual fade in" id="tab_prodotto_UC_parametri_qualitativi" style="overflow: auto; margin-bottom: 70px;">

                           <div class="jumbotron">
                                <div class="row">
                                    <div class= "col-lg-6 col-md-6 col-sm-12">
                                    <!-- Griglia Calibri -->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="prodotto_UC_griglia_calibri"></div>
                                    </div>
                                </div>
                                <div class= "col-lg-6 col-md-6 col-sm-12">
                                    <!-- Griglia Indici -->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="prodotto_UC_griglia_indici"></div>
                                    </div>
                                </div>
                            </div>
                       </div>
                        </div>

                        <div class="tab-pane DatiContabili fade in" id="tab_prodotto_UC_dati_contabilita" style="overflow: auto; margin-bottom: 70px;">
                            <div class="jumbotron">
                                <div class="row">
                                    <!-- Dropdown IVA-->
                                    <div class="col-lg-6 col-md-6 col-sm-12 ">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblCodIva" for="prodotto_UC_ddlCodIva">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AliquotaIva %>" runat="server"></asp:Localize>:</label>
                                                    <input name="prodotto_UC_ddlCodIva" id="prodotto_UC_ddlCodIva" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                   <!-- Dropdown IVA in compensazione-->
                                   <div class="col-lg-6 col-md-6 col-sm-12 ">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon alert-info" id="prodotto_UC_lblCodIvaCompensazione" for="prodotto_UC_ddlCodIva">
                                                    <asp:Localize Text="<%$ Resources: AliquotaIvaInCompensazione %>" runat="server"></asp:Localize>:</label>
                                                <input name="prodotto_UC_ddlCodIvaCompensazione" id="prodotto_UC_ddlCodIvaCompensazione" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                  </div>
                                </div>
                                <div class="row">
                                    <!-- Dropdown Conto Economico Acquisto-->
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="lblContoEconomico" for="ddlContoEconomico">
                                                        <asp:Localize Text="<%$ Resources: ContoEconomicoAcquisto %>" runat="server"></asp:Localize>:</label>
                                                    <input name="prodotto_UC_ddlContoEconomicoAcquisto" id="prodotto_UC_ddlContoEconomicoAcquisto" class="form-control" />
                                                </div>
                                           </div>
                                       </div>
                                    </div>
                                  <!-- Dropdown Conto Economico Vendita-->
                                   <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon alert-info" id="prodotto_UC_lblContoEconomicoVendita" for="prodotto_UC_ddlContoEconomicoVendita">
                                                    <asp:Localize Text="<%$ Resources: ContoEconomicoVendita %>" runat="server"></asp:Localize>:</label>
                                                <input name="prodotto_UC_ddlContoEconomicoVendita" id="prodotto_UC_ddlContoEconomicoVendita" class="form-control" />
                                           </div>
                                        </div>
                                    </div>
                                   </div>
                            </div>
                            <div class="row">
                                <!-- Dropdown Conto Patrimoniale Acquisto-->
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon alert-info" id="prodotto_UC_lblContoPatrimonialeAcquisto" for="prodotto_UC_ddlContoPatrimonialeAcquisto">
                                                    <asp:Localize Text="<%$ Resources: ContoPatrimonialeAcquisto %>" runat="server"></asp:Localize>:</label>
                                                <input name="prodotto_UC_ddlContoPatrimonialeAcquisto" id="prodotto_UC_ddlContoPatrimonialeAcquisto" class="form-control" />
                                           </div>
                                        </div>
                                    </div>
                                </div>
                                <!-- Dropdown Conto Patrimoniale Vendita-->
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon alert-info" id="prodotto_UC_lblContoPatrimonialeVendita" for="prodotto_UC_ddlPatrimonialeVendita">
                                                    <asp:Localize Text="<%$ Resources: ContoPatrimonialeVendita %>" runat="server"></asp:Localize>:</label>
                                                <input name="prodotto_UC_ddlPatrimonialeVendita" id="prodotto_UC_ddlContoPatrimonialeVendita" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">

                                <!-- Dropdown Gruppo Merce-->
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon alert-info" id="prodotto_UC_lblGruppoMerce" for="prodotto_UC_ddlGruppoMerce">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GruppoMerce %>" runat="server"></asp:Localize>:</label>
                                                <input name="prodotto_UC_ddlGruppoMerce" id="prodotto_UC_ddlGruppoMerce" class="form-control" />
                                           </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                     </div>

                        <div class="tab-pane Traduzioni fade in" id="tab_prodotto_UC_traduzioni" style="overflow: auto; margin-bottom: 70px;">
                            <div class="jumbotron">
                                 <div class="row">
                                      <!-- Griglia Traduzioni -->
                                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="prodotto_UC_griglia_traduzioni"></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                       <div class="tab-pane Altri_Dati fade in" id="tab_prodotto_UC_altri_dati" style="overflow: auto; margin-bottom: 70px;">
                            <div class="jumbotron">
                                 <div class="row">
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblEAN" for="txt_prodotto_UC_EAN">EAN/GTIN:</label>
                                                    <input type="text" id="txt_prodotto_UC_EAN" name="EAN/GTIN" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblBarcode" for="txt_prodotto_UC_Barcode">
                                                        <asp:Localize Text="<%$ Resources: BarcodeInterno %>" runat="server"></asp:Localize>:</label>
                                                    <input type="text" id="txt_prodotto_UC_Barcode" name="Barcode Interno" class="form-control" />
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
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblPesoNetto" for="txt_prodotto_UC_PesoNetto">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PesoNetto %>" runat="server"></asp:Localize>:</label>
                                                    <input type="text" id="txt_prodotto_UC_PesoNetto" name="Peso Netto" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblPesoSgocciolato" for="txt_prodotto_UC_PesoSgocciolato">
                                                        <asp:Localize Text="<%$ Resources: PesoSgocciolato %>" runat="server"></asp:Localize>:</label>
                                                    <input type="text" id="txt_prodotto_UC_PesoSgocciolato" name="Peso Sgocciolato" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">                                                    
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblTara" for="txt_prodotto_UC_Tara">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tara %>" runat="server"></asp:Localize>:</label>
                                                    <input name="txt_prodotto_UC_Tara" id="txt_prodotto_UC_Tara" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                           </div>


                           <div class="row">
                                    <div id="produzione_propria" class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div  class="input-group">
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblProduzionePropria" for="prodotto_UC_ddlProduzionePropria">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ProduzionePropria %>" runat="server"></asp:Localize>:</label>
                                                    <input name="prodotto_UC_ddlProduzionePropria" id="prodotto_UC_ddlProduzionePropria" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">                                                    
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblPesoEgalizzato" for="prodotto_UC_ddlPesoEgalizzato">
                                                        <asp:Localize Text="<%$ Resources: PesoEgalizzato %>" runat="server"></asp:Localize>:</label>
                                                    <input name="prodotto_UC_ddlPesoEgalizzato" id="prodotto_UC_ddlPesoEgalizzato" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                           </div>

                           <div class="row">
                                  <div id="ingredienti" class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
				                            <div class="form-group">
					                            <div class="input-group">
                                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblIngredienti" for="txt_prodotto_UC_Ingredienti">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ingredienti %>" runat="server"></asp:Localize>:</label>
                                                    <textarea id="txt_prodotto_UC_Ingredienti" class="form-control k-content" style="resize: none;" rows="10"></textarea>
                                               </div>
                                            </div>
                                       </div>
                                </div>
                            </div>
                           <div class="row">
                               &nbsp;
                            <div class="col-lg-12 col-md-12 col-sm-12" id="id_prodotto_UC_rimuovi_immagine">
                                <div class="btn btn-success col-lg-12 col-md-12 col-sm-12" id="btn_prodotto_UC_rimuovi_immagine">
                                    <span class="fa fa-ban"></span><span class="lampeggiante">
                                        <asp:Localize Text="<%$ Resources: RimuoviImmagine %>" runat="server"></asp:Localize></span>
                                </div>
                            </div>
                           </div>
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <label class="input-group-addon alert-info" id="prodotto_UC_lblImage" for="prodotto_UC_imageEditor">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Immagine %>" runat="server"></asp:Localize>:</label>
                                    <div id="prodotto_UC_imageEditor"></div>
                                 </div>
                            </div>
                                
                        </div>
                    </div>


                       <div class="tab-pane Altri_Dati fade in" id="tab_prodotto_UC_alias" style="overflow: auto; margin-bottom: 70px;">
                           <div class="jumbotron">
                            <div class="row">
                                <!-- Griglia Storico Prezzi -->
                                <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                    <div id="prodotto_UC_griglia_alias"></div>
                                </div>
                            </div>
                           </div>
                       </div>

                 </div>
               </div>
            </div>
        </div>
    </div>  

    <div class="panel-group prodotto_Edit_UC_gridArea" style="display: none;">
        <!--Griglia-->
        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
            <input type="hidden" id="chiave_prodotti" />
            <input type="hidden" id="hdKendoProdotto_Valorizzazione" />
            <div id="divKendoProdotto"></div>
        </div>
    </div>
    <!-- fine container -->

<input type="hidden" id="hf_Piva" runat="server" />

<script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Anagrafica/UserControl/Prodotto_Edit_UC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Anagrafica/UserControl/Prodotto_Edit_UC_globali.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Anagrafica/UserControl/Prodotto_Edit_UC_ws_client.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Anagrafica/UserControl/Prodotto_Edit_UC_jQueryDocReady.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

<script>
    var cIdPiva = "#<%=hf_Piva.ClientID %>";
</script>
