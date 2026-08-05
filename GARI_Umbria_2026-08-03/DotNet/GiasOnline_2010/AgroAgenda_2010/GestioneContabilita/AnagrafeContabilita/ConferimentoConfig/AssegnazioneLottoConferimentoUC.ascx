<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="AssegnazioneLottoConferimentoUC.ascx.vb" Inherits="AgroAgenda_2010.AssegnazioneLottoConferimentoUC" %>

    <div id="divParametriLotto_Conferimento_LottiUC" class="jumbotron gias-jumbotron-0 gias-mr-1 gias-ml-1 gias-border-white gias-pb-1" style="margin-top: 20px;">
        <div class="form-horizontal" style="margin-top: 20px;">
            <div class="form-group">

                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group">
                        <span class="input-group-addon lbl_required media-sized calc-size">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaProdotto %>" runat="server">
                                Categoria Prodotto
                            </asp:Localize>:
                        </span>
                        <input id="ddlTipologia_Conferimento_LottiUC" name="Tipologia Conferimento" class="form-control media-sized"/>
                    </div>
                </div>

                <div class="col-lg-6 col-md-6 col-sm-12">
                     <div class="input-group">
                        <span class="input-group-addon lbl_required media-sized calc-size">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Separatore %>" runat="server">
                                Separatore
                            </asp:Localize>:
                        </span>
                        <input id="ddlSeparatoreParametri_Conferimento_LottiUC" name="Separatore" class="form-control media-sized"/>
                     </div>
                </div>

                <div class="col-lg-6 col-md-6 col-sm-12">      
                    <label class="input-group-addon lbl_required" for="lstParametriLotto_Conferimento_LottiUC" 
                        id="lbllstParametriLotto_Conferimento_LottiUC">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ElencoParametri %>" runat="server">
                                Elenco Parametri
                            </asp:Localize>:
                        </label>
                    <select id="lstParametriLotto_Conferimento_LottiUC" style="width:100%"></select>
               </div>

               <div class="col-lg-6 col-md-6 col-sm-12">       
                    <label class="input-group-addon lbl_required" for="lstParametriLottoScelti_Conferimento_LottiUC" 
                        id="lbllstParametriLottoScelti_Conferimento_LottiUC">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriScelti %>" runat="server">
                                Parametri Scelti
                            </asp:Localize>:
                    </label>
                    <select id="lstParametriLottoScelti_Conferimento_LottiUC" style="width:100%"></select>
               </div>

               <div class="col-lg-6 col-md-6 col-sm-12" style="margin-top: 10px;">
                   <div id="Conferimento_LottiUC_Salva" class="btn btn-success gias-btn-primary xo-max-w-200 col-lg-12 col-md-12 col-sm-12" 
                       onclick="Salva_Configurazione_Lotto()">
                        <i class="fa fa-floppy-o"></i>Salva
                   </div>
               </div>

              <div class="col-lg-6 col-md-6 col-sm-12" style="margin-top: 10px;">
                   <div id="Conferimento_LottiUC_Annulla" class="btn btn-danger xo-max-w-200 col-lg-12 col-md-12 col-sm-12" 
                       onclick="Conferma_Annulla_Configurazione_Lotto()">
                        <i class="fa fa-undo"></i>Annulla Modifiche
                    </div>
               </div>

             </div>
         </div>
      </div>

<input type="hidden" id="hdData" runat="server" />

<!-- fine container -->
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/AssegnazioneLottoConferimentoUC_jQueryDocReady.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/AssegnazioneLottoConferimentoUC.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/AssegnazioneLottoConferimentoUC_ws_client.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoConfig/AssegnazioneLottoConferimentoUC_globali.js") %>" ></script>
<script type="text/javascript">
    var hfData = "#<%=hdData.ClientID() %>";
</script>