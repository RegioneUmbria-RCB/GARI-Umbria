<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CastellettoUC.ascx.vb" Inherits="AgroAgenda_2010.CastellettoUC" %>

<style type="text/css">
    .errorClass {
        border-color: #D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }

    .blockModifica, .blockCancella, .blockDuplica {
        /* display: block; con questo non è possibile ridimensionare la colonna dei pulsanti */
        margin-top: 10px !important;
        margin-bottom: 10px !important;
    }
</style>

<div id="panelAreaCastelletto" class="panel-group searchArea" >
    <div class="row">

        <div class="col-lg-8 col-md-8 col-sm-12">
            <div class="jumbotron" style="padding-bottom: 0;">
                <div class="container" style="width: 100;">
                    
                        <div class="row" id="id_riga_castelletto">
                            
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <!--Griglia-->
                                <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                    <div id="tab_griglia_castelletto"></div>
                                </div>
                            </div>

                        </div>

               </div>
            </div>
          </div>


          <!--Riepilogo-->
          <div class="col-lg-4 col-md-4 col-sm-12">
            <div class="jumbotron" style="padding-bottom: 0;">
                <div class="container" style="width: 100;">     
                    
                       <div class="row" style="margin-top: 5px;">
                       </div>

                       <div class="row" style="margin-top: 5px;">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="input-group" id="imponibileLordoGroup">
                                <label class="input-group-addon" id="lblImponibileTotaleLordo" for="idImponibileTotaleLordo">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TotaleImponibileLordo %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="idImponibileTotaleLordo" id="idImponibileTotaleLordo" class="form-control text-right nbRiepilogoImporto" />
                            </div>
                        </div>
                       </div>

                       <div class="row" style="margin-top: 5px;">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="input-group" id="variazioniGroup">
                                <label class="input-group-addon" id="lblVariazione" for="idVariazione">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TotaleVariazioni %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="idVariazione" id="idVariazione" class="form-control text-right nbRiepilogoImporto" />
                            </div>
                        </div>
                       </div>

                      <div class="row" style="margin-top: 5px;">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="input-group" id="imponibileNettoGroup">
                                <label class="input-group-addon" id="lblImponibileNetto" for="idImponibileNetto">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImponibileNetto %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="idImponibileNetto" id="idImponibileNetto" class="form-control text-right nbRiepilogoImporto" />
                            </div>
                        </div>
                       </div>

                       <div class="row" style="margin-top: 5px;">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="input-group" id="impostaGroup">
                                <label class="input-group-addon" id="lblImposta" for="idImposta">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TotaleImposta %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="idImposta" id="idImposta" class="form-control text-right nbRiepilogoImporto" />
                            </div>
                         </div>
                       </div>

                       <div class="row" style="margin-top: 5px;">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="input-group" id="totaleGroup">
                                <label class="input-group-addon" id="lblTotale" for="idTotale">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TotaleDocumento %>" runat="server"></asp:Localize>:
                                </label>
                                <input name="idTotale" id="idTotale" class="form-control text-right nbRiepilogoImporto" />
                            </div>
                         </div>
                       </div>


               </div>
     
             </div>
          
          </div>
   </div>
</div>
  


<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/Fatturazione/CastellettoUC_jQueryDocReady.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/Fatturazione/CastellettoUC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/Fatturazione/CastellettoUC_ws_client.js")) %>"></script>
