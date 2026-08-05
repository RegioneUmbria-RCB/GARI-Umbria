<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="AttivazioneModuliUC.ascx.vb" Inherits="AgroAgenda_2010.AttivazioneModuli" %>

<div id="div_Conferimento_AttivazioneModuli_LottiUC" class="jumbotron gias-jumbotron-0 gias-mr-1 gias-ml-1 gias-border-white gias-pb-1" style="margin-top: 20px;">
    <div class="form-horizontal" style="margin-top: 20px;">
        <div class="form-group">

            <div class="col-lg-6 col-md-6 col-sm-12">
                <div class="input-group">
                    <span class="input-group-addon lbl_required media-sized calc-size">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneFreshAndFood %>" runat="server">
                            Gestione Fresh&Food
                        </asp:Localize>:
                    </span>
                    <input type="checkbox" id="chkTrasformazioniVegetali" aria-label="chkTrasformazioniVegetali" />
                </div>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-12">
                 <div class="input-group">
                    <span class="input-group-addon lbl_required media-sized calc-size">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneParametriQualitativiTrasformatiAnimali %>" runat="server">
                            Gestione Parametri qualitativi Trasformati animali
                        </asp:Localize>:
                    </span>
                     <input type="checkbox" id="chkTrasformazioniAnimali" aria-label="chkTrasformazioniAnimali" />
                 </div>
            </div>
            
            <div class="col-lg-6 col-md-6 col-sm-12" style="margin-top: 10px;">
                   <div id="Conferimento_AttivazioneModuli_LottiUC_Salva" class="btn btn-success gias-btn-primary xo-max-w-200 col-lg-12 col-md-12 col-sm-12" 
                       onclick="Salva_AttivazioneModuli()">
                        <i class="fa fa-floppy-o"></i>Salva
                   </div>
               </div>

              <div class="col-lg-6 col-md-6 col-sm-12" style="margin-top: 10px;">
                   <div id="Conferimento_AttivazioneModuli_LottiUC_Annulla" class="btn btn-danger xo-max-w-200 col-lg-12 col-md-12 col-sm-12" 
                       onclick="Conferma_Annulla_AttivazioneModuli()">
                        <i class="fa fa-undo"></i>Annulla Modifiche
                    </div>
               </div>

         </div>
     </div>
  </div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/AttivazioneModuliUC.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/AttivazioneModuliUC_jQueryDocReady.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/AttivazioneModuliUC_ws_client.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/AttivazioneModuliUC_globali.js") %>" ></script>