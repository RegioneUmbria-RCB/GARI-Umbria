<%@ Page Title="IrrigazioneBS" Language="vb" AutoEventWireup="false" CodeBehind="IrrigazioneBS.aspx.vb"
    Inherits="AgroAgenda_2010.IrrigazioneBS" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }

/*        .DoseAcquaIrrigazione {
            background-color: Pink;
        }*/

        .DoseAcquaIrrigazione {
            background-color: #ff99aa;
        }

/*        .OrePortataIrrigazione {
            background-color: #C6E99C;
        }*/

        .OrePortataIrrigazione {
            background-color: #a0da58;
        }

/*        .QtaTotaleAcquaIrrigazione {
            background-color: #8AB7DA;
        }*/

        .QtaTotaleAcquaIrrigazione {
            background-color: #64a0ce;
        }

         .TitoloNoteIrrigazione {
            font-size: 11pt;
            font-weight: 700;
            line-height: 30px;
            text-transform: uppercase;
         }
         #master_contenitore_principale {
                padding-left: 15px;
                padding-right: 15px;
            }

        .btn-DSS {
            font-size: 11px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron xo-irrigazione-bs" id="divTestataRicetta_Irrigazione" style="padding-left: 15px; padding-right: 15px; display: none">
        <div class="row">
            <div class="col-lg-6">
                <h4>Dati Ricetta</h4>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Txt_Ricetta_Descrizione_Irrigazione">Descrizione</span>
                            <input type="text" id="Txt_Ricetta_Descrizione_Irrigazione" class="form-control" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Txt_Ricetta_Numero_Irrigazione">Numero</span>
                            <input type="text" id="Txt_Ricetta_Numero_Irrigazione" class="form-control" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon alert-info" id="lbl_Data_Inizio_ricetta_Irrigazione">Da</span>
                    <input id="Data_Inizio_ricetta_Irrigazione" class="form-control" />
                </div>
            </div>
            <div class="col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon alert-info" id="lbl_Data_Fine_ricetta_Irrigazione">A</span>
                    <input id="Data_Fine_ricetta_Irrigazione" class="form-control" />
                </div>
            </div>
            <div class="col-lg-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group"> 
                            <span class="input-group-addon alert-info" id="lbl_Txt_Ricetta_Nota_Irrigazione">Nota</span>
                            <input type="text" id="Txt_Ricetta_Nota_Irrigazione" class="form-control" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="jumbotron xo-irrigazione-bs xo-padding-right-30px xo-padding-bottom-0">

        <div class="row" style="padding-left: 15px; padding-right: 15px;">

            <!-- FILTRI -->
            <div id="divFiltri" class="col-md-12">

                <div class="row">
                    <div class="col-md-6">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <!--Date Picker Data Irrigazione-->
                                    <label class="input-group-addon alert-info" for="Data_Irrigazione">Data</label>
                                    <input id="Data_Irrigazione" title="Data Irrigazione" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-6 col-md-6 col-sm-12 " style="padding-right: 0">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <!--DropDown Specie Irrigazione-->
                                    <label class="input-group-addon alert-info" for="ddl_Specie_Irrigazione">Specie</label>
                                    <input id="ddl_Specie_Irrigazione" style="max-width: 560px !important" title="Specie" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <!--DropDown Centro Aziendale Irrigazione-->
                                    <label class="input-group-addon alert-info" for="ddl_Centro_Aziendale_Irrigazione">Centro Aziendale</label>
                                    <input id="ddl_Centro_Aziendale_Irrigazione" title="Centro Aziendale Irrigazione" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-6 col-md-6 col-sm-12 " style="padding-right: 0">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <!--DropDown Tipo Irrigazione-->
                                    <label class="input-group-addon alert-info" for="ddl_Tipo_Irrigazione">Seleziona per Tipo di Irrigazione</label>
                                    <input id="ddl_Tipo_Irrigazione" title="Tipo di Irrigazione" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="jumbotron xo-irrigazione-bs xo-padding-top-0">
        <div class="row xo-row-flex-bottom">
            <div class="col-md-6 col-xs-12 xo-padding-right-22-5px">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <!--DropDown Unità di Misura Irrigazione-->
                            <label class="input-group-addon alert-info" for="ddl_Udm_Irrigazione">Unità di Misura Della Dose:</label>
                            <input id="ddl_Udm_Irrigazione" title="Unità di Misura Della Dose" class="form-control" />
                        </div>
                    </div>
                </div>
            </div>
            <div class=" col-md-6 col-xs-12">
                <div class="form-horizonta">
                    <div class="form-group">
                        <div class="input-group">
                            <!--CheckBox Verifica Compatibilità Microirrigazione-->
                            <label class='k-checkbox-label' for="chkMicroirr">Verifica Compatibilità Microirrigazione:</label>
                            <input id="chkMicroirr" type='checkbox' class='k-checkbox'>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="jumbotron xo-irrigazione-bs">
        <div class="row">
            <%--template kendo grid--%>
            <script id="templateBtnFiltraColonneGrid_Impianti_Irrigazione" type="text/x-kendo-template">
                <div  id="BtnFiltraColonneGrid_Impianti_Irrigazione" class="btn btn-success" onclick="filtraColonne_Impianti_Irrigazione();">
                    <span class="lampeggiante">Seleziona Colonne</span>
                </div>
            </script>

            <script id="templateBtnImpostaValoriGrid_Impianti_Irrigazione" type="text/x-kendo-template">
                <div  id="BtnImpostaValoriGrid_Impianti_Irrigazione" class="btn btn-success" onclick="impostaValori_Impianti_Irrigazione();">
                    <span class="lampeggiante">Imposta Valori in tutte le righe</span>
                </div>
            </script>
            <%--fine template kendo grid--%>

            <%If (Master.Master_versione <> "2022") Then %>
            <!--Note Grid Impianti Irrigazione-->
            <div id="Note_grid_Impianti_Irrigazione">
                <div class="row">
                    <i class="fa fa-exclamation-triangle" aria-hidden="true"></i><strong>Inserire a scelta : 'Dose' oppure 'Ore + Portata'.</strong>
                </div>
                <div class="row" style="padding-bottom: 10px">
                    <i class="fa fa-exclamation-triangle" aria-hidden="true"></i><strong>La quantità totale di acqua viene calcolata automaticamente.</strong>
                </div>
            </div>
            <% End If %>

            <div style="overflow: auto;">
                <div id="Mess_grid_Impianti_Irrigazione" style='text-align: center;'></div>
                 <%If (Master.Master_versione = "2022")Then %>
                    <!--Note Grid Impianti Irrigazione-->
                    <div id="Note_grid_Impianti_Irrigazione" class="xo-alert xo-alert-warning">
                        <i class="fa fa-exclamation-triangle" aria-hidden="true"></i>
                        <div class="row">
                            <p>Inserire a scelta : 'Dose' oppure 'Ore + Portata'.</p>
                            <p>La quantità totale di acqua viene calcolata automaticamente.</p>
                        </div>
                    </div>
                 <% End If %>
                
                <!--Grid Impianti Irrigazione-->
                <div id="grid_Impianti_Irrigazione"></div>
            </div>
        </div>

        <!--Dialog per la gestione delle colonne da visualizzare -->
        <div class="modal fade" id="dialogImpostazioniColonneGrid_Impianti_Irrigazione" tabindex="-1" role="dialog"
            aria-labelledby="dialogImpostazioniColonneGrid_Impianti_IrrigazioneLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="dialogImpostazioniColonneGrid_Impianti_IrrigazioneLabel">Visualizzazione colonne</h4>
                    </div>
                    <div class="modal-body">
                        <div id="elenco_colonneGrid_Impianti_Irrigazione">
                        </div>
                    </div>
                    <!--- Bottoni Salvattaggio e Annulla -->
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" id="btn_Annulla_Colonne_Grid_Impianti_Irrigazione" data-dismiss="modal">
                            Annulla</button>
                        <button type="button" class="btn btn-success" id="btn_Salva_Colonne_Grid_Impianti_Irrigazione" onclick="$('#dialogImpostazioniColonneGrid_Impianti_Irrigazione').modal('hide');SalvaImpostazioniColonneGrid_Impianti_Irrigazione();">
                            Aggiungi
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <!--Fine Dialog per la gestione delle colonne da visualizzare -->

        <!--Dialog per l'inserimento dei valori in tutte le relative righe -->
        <div class="modal fade" id="dialogImpostaValoriGrid_Impianti_Irrigazione" tabindex="-1" role="dialog"
            aria-labelledby="dialogImpostaValoriGrid_Impianti_IrrigazioneLabel" aria-hidden="true">
            <div class="modal-dialog" id="dialogImpostaValoriGrid_Impianti_Irrigazione_dialog" role="document">
                <div class="modal-content" id="dialogImpostaValoriGrid_Impianti_Irrigazione_content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="dialogImpostaValoriGrid_Impianti_IrrigazioneLabel">Impostazione Valori nelle righe</h4>
                    </div>
                    <div class="modal-body">
                        <div id="controlliGrid_Impianti_Irrigazione">
                        </div>
                    </div>
                    <!--- Bottoni Salvattaggio e Annulla -->
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" id="btn_Annulla_ImpostaValori_Grid_Impianti_Irrigazione" data-dismiss="modal">
                            Annulla</button>
                        <button type="button" class="btn xonne-btn-primary btn-success" id="btn_Salva_ImpostaValori_Grid_Impianti_Irrigazione" onclick="CopiaValoriGrid_Impianti_Irrigazione();">
                            Copia Valori
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <!--Fine Dialog per l'inserimento dei valori in tutte le relative colonne -->
    </div>

    <div class="jumbotron xo-irrigazione-bs">
        <div class="row">
            <div class="col-md-6 col-xs-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span id="lblSupHaSelezionata" class="input-group-addon alert-info"><b>Sup. [ha]</b> Selezionata:</span>
                            <input type="text" class="form-control SommaSuperficie" id="Txt_SupSelezionataIrrigazione" readonly="readonly" disabled="disabled" value="0" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


        <!--Parte di Rilievi Pioggie-->
    <div class="jumbotron xo-padding-y-15px xo-irrigazione-bs">
        <div style="padding: 0px; text-align: left; margin-top: 10px;">
            <div style="width: 100%;" class="xo-padding-y-15px xo-width-auto">
                <h4><b><strong>Tabella informativa sui Rilievi Piogge</strong></b></h4>
            </div>
            <div style="width: 100%" class="xo-margin-y-15px xo-alert xo-alert-info xo-width-auto">
                <%If (Master.Master_versione = "2022") %>
                    <i class="fa fa-info-circle" aria-hidden="true"></i>
                    <div>
                        <p>Di seguito è possibile cercare i rilievi piogge effettuati sui centri aziendali interessati nella operazione di irrigazione. </p>
                        <p>Le informazioni sui rilievi piogge possono essere utili per meglio gestire le irrigazioni. </p>
                        <p>Selezionare la data in cui filtrare i dati sulle piogge. </p>
                        <p>La colonna 'Totale Nel Periodo' indica i millimetri totali rilevati nel centro nel periodo selezionato. </p>
                    </div>
                    
                <% Else %>
                    <strong>Di seguito è possibile cercare i rilievi piogge effettuati sui centri aziendali interessati nella operazione di irrigazione. </strong>
                    <br>
                    <strong>Le informazioni sui rilievi piogge possono essere utili per meglio gestire le irrigazioni. </strong>
                    <br>
                    <strong>Selezionare la data in cui filtrare i dati sulle piogge. </strong>
                    <br>
                    <strong>La colonna 'Totale Nel Periodo' indica i millimetri totali rilevati nel centro nel periodo selezionato. </strong>
                <% End If %>
            </div>
            <div style="padding-top: 20px;" class="xo-padding-top-0">
                <div class="row xo-row-flex-bottom">
                    <!--Date Picker Data Da Grid Rilievi Pioggie-->     
                    <div class="col-lg-5 col-md-5 col-sm-12">   
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">                                    
                                    <label class="input-group-addon alert-info" for="Data_Da_Irrigazione">Data Da</label>
                                    <input id="Data_Da_Irrigazione" title="Data Da" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!--Date Picker Data A Grid Rilievi Pioggie-->
                    <div class="col-lg-5 col-md-5 col-sm-12">   
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon alert-info" for="Data_A_Irrigazione">Data A</label>
                                    <input id="Data_A_Irrigazione" title="Data A" class="form-control" />
                                 </div>
                            </div>
                        </div>
                     </div>

                    <!--Bottone Cerca Pioggie-->
                    <div class="col-lg-2 col-md-2 col-sm-12"  style="padding-left: 20px;">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <div class="btn btn-success xonne-btn-primary btn_100" onclick="CreaKendoGrid_RilieviPioggie_Irrigazione();">
                                        <i class="fa fa-search"></i>Cerca
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                   </div>  
                </div>  
           </div>
           <div class="row" style="overflow: auto;">
               <div id="Mess_grid_RilieviPioggie_Irrigazione" style='text-align: center;'></div>
                <!--Grid Rilievi Pioggie Irrigazione-->
                <div id="grid_RilieviPioggie_Irrigazione"></div>
            </div>
        </div>


    <div class="jumbotron xo-irrigazione-bs">
        <div class="row">
            <div class="col-md-12 TitoloNoteIrrigazione" id="kendo_Note_Irrigazione" onclick="mostraNascondiIrrigazione('divNote')">
                <a class="fa fa-plus-circle" id="a_Titolo_Note_Irrigazione"></a>NOTE
            </div>
        </div>
        <div class="row">
                <div id="divNote" class="col-md-12">
                    <div class="form-horizontal">
                        <ul class="nav nav-tabs" role="tablist" id="tabs">
                            <li id="li_tabGiust" class="active"><a href="#tabGiust" data-toggle="tab" id="a_tabGiust" style="display: none;">Giustificazioni</a></li>
                            <li id="li_tabNote"><a href="#tabNote" data-toggle="tab" id="a_tabNote">Note</a></li>
                            <li><a href="#tabMeteo" data-toggle="tab" id="a_tabMeteo" style="display: none;">Meteo</a></li>
                            <li><a href="#tabVentoIntensita" data-toggle="tab" id="a_tabVentoIntensita" style="display: none;">Vento Intensita</a></li>
                            <li><a href="#tabVentoDirezione" data-toggle="tab" id="a_tabVentoDirezione" style="display: none;">Vento Direzione</a></li>
                            <li><a href="#tabTemperatura" data-toggle="tab" id="a_tabTemperatura" style="display: none;">Temperatura</a></li>
                            <li><a href="#tabOrario" data-toggle="tab" id="a_tabOrario" style="display: none;">Orario</a></li>
                            <li><a href="#tabMotivazioni" data-toggle="tab" id="a_tabMotivazioni" style="display: none;">Motivazione</a></li>
                        </ul>

                        <div class="tab-content">
                            <div class="tab-pane fade in active" id="tabGiust" style="max-height: 180px; overflow: auto;">
                                <div id="checkbox_a_tabGiust" style="display: none;"></div>
                            </div>
                            <div class="tab-pane fade in" id="tabNote" style="max-height: 180px; overflow: auto;">
                                <textarea id="Txt_Note" class="form-control k-content" style="height: 100%;" rows="3"></textarea>
                            </div>
                            <div class="tab-pane fade in" id="tabMeteo" style="max-height: 180px; overflow: auto;">
                                <div id="checkbox_a_tabMeteo" style="display: none;"></div>
                            </div>
                            <div class="tab-pane fade in" id="tabVentoIntensita" style="max-height: 180px; overflow: auto;">
                                <div id="checkbox_a_tabVentoIntensita" style="display: none;"></div>
                            </div>
                            <div class="tab-pane fade in" id="tabVentoDirezione" style="max-height: 180px; overflow: auto;">
                                <div id="checkbox_a_tabVentoDirezione" style="display: none;"></div>
                            </div>
                            <div class="tab-pane fade in" id="tabTemperatura" style="max-height: 180px; overflow: auto;">
                                <div id="checkbox_a_tabTemperatura" style="display: none;"></div>
                            </div>
                            <div class="tab-pane fade in" id="tabOrario" style="max-height: 180px; overflow: auto;">
                                <div id="checkbox_a_tabOrario" style="display: none;"></div>
                            </div>
                            <div class="tab-pane fade in" id="tabMotivazioni" style="max-height: 180px; overflow: auto;">
                                <div id="checkbox_a_tabMotivazioni" style="display: none;"></div>
                            </div>
                        </div>

                    </div>
            </div>
        </div>
    </div>

    <div class="row xo-irrigazione-bs" style="margin-bottom: 20px;">
       
        <div class="col-lg-6 col-md-6 col-xs-6" style="padding-top: 10px; padding-left: 5px; padding-right: 5px; display: none" id="divSalvaRicetta">
            <div class="btn btn-success btn_100" onclick="BottoneSalvaRicetta(1);">
                <i class="fa fa-floppy-o"></i>Salva ed Esci
            </div>
        </div>

        <div class="col-md-12 col-xs-12 col-lg-8" id="divSalva" style="padding-top: 10px; padding-left: 5px; padding-right: 5px;">
            <div class="btn btn-success xonne-btn-primary btn_100" onclick="Salva_Irrigazione(1);">
                <i class="fa fa-floppy-o"></i>Salva ed Esci
            </div>
        </div>

        <div class="col-lg-2 col-md-6 col-xs-12" style="padding-top: 10px; padding-left: 5px; padding-right: 5px;" id="divSalvaNuovo">
            <div class="btn btn-success btn_100 xonne-btn-primary" onclick="Salva_Irrigazione(2);">
                <i class="fa fa-floppy-o"></i>Salva e Nuovo
            </div>
        </div>

        <div class="col-lg-2 col-md-6 col-xs-12" style="padding-top: 10px; padding-left: 5px; padding-right: 5px;" id="divSalvaDuplica">
            <div class="btn btn-success btn_100 xonne-btn-primary" onclick="Salva_Irrigazione(3);">
                <i class="fa fa-floppy-o"></i>Salva e Duplica
            </div>
        </div>
        
        <div class="col-lg-6 col-md-6 col-xs-12" style="padding-top: 10px; padding-left: 5px; padding-right: 5px; display: none" id="divRicetteSalvaEVai">
            <div>
                <ul id="menuRicette_Irrigazione">
                    <li class="btn btn-success btn_100">
                        <i class="fa fa-floppy-o"></i>
                        Salva e:
                        <ul id="ulMenuRicette_Irrigazione">
                        </ul>
                    </li>
                </ul>
            </div>
        </div>

        <%--                <div class="col-lg-2 col-md-6 col-xs-12" style="padding-top: 10px; padding-left: 5px; padding-right: 5px;" id="divSalvaVaiCosti">
            <div class="btn btn-success btn_100" onclick="Salva_Irrigazione(4);">
                <i class="fa fa-floppy-o">Salva e Vai ai Costi</i>
            </div>
        </div>--%>
    </div>


    <input type="hidden" id="hf_KendoGrid_Impianti_Irrigazione" runat="server" />
    <input type="hidden" id="hf_ElencoColonneKendoGrid_Impianti_Irrigazione" runat="server" />
    <input type="hidden" id="hf_TipoOperazione" runat="server" />
    <input type="hidden" id="hf_Piva" runat="server" />
    <input type="hidden" id="hf_ParamAgenda" runat="server" />
    <input type="hidden" id="hf_KendoGrid_RilieviPioggie_Irrigazione" runat="server" />
    <input type="hidden" id="hf_Ricetta_Cod" runat="server" />
    <input type="hidden" id="hf_Operazione_Ricetta" runat="server" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("IrrigazioneBS_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("IrrigazioneBS.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("IrrigazioneBS_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("IrrigazioneBS_ws_client.js") %>"></script>

    <!--Aggiunto riferimento allo script GIS.js per utilizzare la funzione registrazioneAgenda()-->
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Gis/Gis.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Gis/Gis_Scripts/scriptMaps_DI_PARTENZA_BS.js")) %>"></script>
    <!---------->

    <!--Per rendere la dialog bootstrap resizable e draggable-->
    <link rel="stylesheet" href="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Styles/jquery-ui-1.11.1.min.css")) %>" />
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scripts/jquery-ui-1.10.3.custom.min.js")) %>"></script>
    <!---------->

    <script>
        var cIdPiva = "#<%=hf_Piva.ClientID %>";
        var TipoOperazione = "#<%=hf_TipoOperazione.ClientID %>";
        var DatiLetti = "#<%=hf_ParamAgenda.ClientID %>";
        var KendoGridmpianti_Irrigazione = "#<%=hf_KendoGrid_Impianti_Irrigazione.ClientID %>";
        var ElencoColonneKendoGrid_Impianti_Irrigazione = "#<%=hf_ElencoColonneKendoGrid_Impianti_Irrigazione.ClientID %>";
        var KendoGridRilieviPioggie_Irrigazione = "#<%=hf_KendoGrid_RilieviPioggie_Irrigazione.ClientID %>";
        var Ricetta_Cod = "#<%=hf_Ricetta_Cod.ClientID %>";
        var Operazione_Ricetta = "#<%=hf_Operazione_Ricetta.ClientID%>";
        var objParametriAgenda_VisualizzaSoloBottoneSalvaEsci = <%=objParametriAgenda_VisualizzaSoloBottoneSalvaEsci %>;
    </script>


    <script type="text/kendo-x-tmpl" id="tmp_listbox_turni">
        <span>TURNO:</span><span style='padding-left: 2px; float: right; font-weight: bold;'>#:kendo.toString(new Date(Data_Turno), "g")#</span><br>
        <span>DURATA:</span><span style='padding-left: 2px; float: right; font-weight: bold;'>#:Qta_Acqua# #:UDM_DES#</span><br><br>
    </script>

</asp:Content>


