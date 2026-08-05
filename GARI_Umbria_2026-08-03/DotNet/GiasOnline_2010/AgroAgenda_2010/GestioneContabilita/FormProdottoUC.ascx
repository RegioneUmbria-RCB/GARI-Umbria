<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="FormProdottoUC.ascx.vb" Inherits="AgroAgenda_2010.FormProdottoUC" %>

<%@ Register TagPrefix="uc1" TagName="RaccolteXConferimentiUC" Src="./UserControl/RaccolteXConferimentiUC.ascx" %>

<asp:HiddenField runat="server" ID="hdKendo_RigaDoc" />
<asp:HiddenField runat="server" ID="hf_key_mov_dett" />
<asp:HiddenField runat="server" ID="hf_Cal_Cod" />
<asp:HiddenField runat="server" ID="hf_Cod_Progetto" />
<asp:HiddenField runat="server" ID="hf_Qta_Extra" />
<asp:HiddenField runat="server" ID="hf_Udm_Cod_Extra" />
<asp:HiddenField runat="server" ID="hf_riga_DataOraUltimaLettura" />
<asp:HiddenField runat="server" ID="hf_PendenzaIniziale" />
<asp:HiddenField runat="server" ID="hf_Categorie_Magazzino" />
<asp:HiddenField runat="server" ID="hf_Categoria_Magazzino_Dft" />
<asp:HiddenField runat="server" ID="hf_ArrayElemCodUdmCod" />
<asp:HiddenField runat="server" ID="hf_indirizzoProfitosan" />
<asp:HiddenField runat="server" ID="hf_LinkWsFitofarmaci" />
<asp:HiddenField runat="server" ID="hf_LinkEditProdotto" />
<asp:HiddenField runat="server" ID="hf_filtroMateriePrimeConferimento" />
<asp:HiddenField runat="server" ID="hdKendo_SceltaDaGiacenza_FormProdottoUC" />

<style>
    #id_row_raccolte_conferimenti {
        padding: 5px 0 15px 0;
    }

    #rowFunzioniPesiRiscontrati {
        padding-bottom: 15px;
    }
</style>

<div id="idFormMagazzino" class="panel-group">

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12 text-right">
            <%-- Utilizzato solo per il salvataggio di testata e prima riga di un nuovo documento --%>  
            <%--<div class="btn btn-success submit" id="btn_SalvaRigaDoc">
                <i class="fa fa-floppy-o"></i>Salva Riga
            </div>--%>

            <div class="btn btn-success" id="btn_SalvaENuovoRiga" style="width: 200px; display: none;" onclick="SalvaENuovaRigaDoc_click()">
                <span class="lampeggiante"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaENuovaRiga %>" runat="server"></asp:Localize></span>
            </div>

            <div class="btn btn-danger" id="btn_AnnullaModifiche" style="width: 200px;" onclick="AnnullaModificheRigaDoc_click()">
                <span class="lampeggiante"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AnnullaModificheRiga %>" runat="server"></asp:Localize></span>
            </div>

            <div class="btn btn-warning" id="btn_EsciRigaDoc" style="width: 150px;" onclick="EsciRigaDoc_click()">
                <span class="lampeggiante"><asp:Localize Text="<%$ Resources: EsciDallaRiga %>" runat="server"></asp:Localize></span>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-6 col-md-6 col-sm-12 col-lg-offset-3 col-lg-offset-3">
            <!--<h4>&nbsp;</h4>-->
            <div id="erroriMsgTabDettaglio" class="alert alert-danger" role="alert" style="display: none;">
                <ul class="errorMessages"></ul>
            </div>
        </div>
    </div>

    <ul id="panelbarFormProdottoUC" style="margin-top: 10px;">
        <%--<div class="jumbotron">--%>

        <li class="k-state" id="panelBar_RaccolteXConferimenti">
            <span class="k-link k-state-selected k-selected">RACCOLTE (DELL'ULTIMO MESE)</span> <!-- i18n: Gestire questo span nel js perché il periodo sarà dinamico con l'introduzione dell'impostazione -->
            <div class="row" id="id_row_raccolte_conferimenti">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <uc1:RaccolteXConferimentiUC id="RaccolteXConferimentiUC1" runat="server" />
                    <button type="button" class="btn btn-warning" name="raccolteXConferimenti_btnAssocia" id="raccolteXConferimenti_btnAssocia">
                        <i class="fa fa-cog"></i><asp:Localize Text="<%$ Resources: AssociaRaccolteAlConferimento %>" runat="server"></asp:Localize>
                    </button>
                </div>
            </div>
        </li>

        <li class="k-state" id="panelBar_OrdiniCliente">
            <span class="k-link k-state-selected k-selected">ORDINI CLIENTE</span> <!-- i18n Forse non serve tradurlo perché il valore viene sovrascritto dal js -->
            <div class="row" id="id_row_ordini_cliente" style="margin-top: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 5px;">
                        <div id="tab_ordini_cliente_formProdottoUC"></div>
                    </div>
                </div>
            </div>
        </li>

        <li class="k-state" id="panelBar_DDTCliente">
            <span class="k-link k-state-selected k-selected">DDT CLIENTE</span>
            <div class="row" id="id_row_ddt_cliente" style="margin-top: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 5px;">
                        <div id="tab_ddt_cliente_formProdottoUC"></div>
                    </div>
                </div>
            </div>
        </li>

        <li class="k-state-active k-active" id="panelBar_Magazzini">
            <span class="k-link k-state-selected k-selected text-uppercase"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Magazzini %>" runat="server"></asp:Localize></span>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-7 col-md-7 col-sm-12"> 
                    <div class="input-group" id="groupCausale_Riga">
                        <label class="input-group-addon" id="lblCausale_Riga" for="ddlCausale_Riga"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Causale %>" runat="server"></asp:Localize>:</label>
                        <input name="ddlCausale_Riga" id="ddlCausale_Riga" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-7 col-md-7 col-sm-12">
                    <div class="input-group" id="groupUbicProvenienza">
                        <label class="input-group-addon" id="lblUbicProvenienza" for="ddlUbicProvenienza"><asp:Localize Text="<%$ Resources: ProvenienzaScarico %>" runat="server"></asp:Localize>:</label>
                        <input name="ddlUbicProvenienza" id="ddlUbicProvenienza" class="form-control" required />
                    </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12">
                    <div class="input-group" id="groupGiacenzaProvenienza">
                        <label class="input-group-addon" id="lblGiacenzaProvenienza" for="txtGiacenzaProvenienza">
                            <asp:Localize Text="<%$ Resources: GiacenzaEsclusaQuestaRiga %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="txtGiacenzaProvenienza" id="txtGiacenzaProvenienza" class="form-control" readonly />
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-7 col-md-7 col-sm-12">
                    <div class="input-group" id="groupUbicDestinazione">
                        <label class="input-group-addon" id="lblUbicDestinazione" for="ddlUbicDestinazione">
                            <asp:Localize Text="<%$ Resources: DestinazioneCarico %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="ddlUbicDestinazione" id="ddlUbicDestinazione" class="form-control" required />
                    </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12">
                    <div class="input-group" id="groupGiacenzaDestinazione">
                        <label class="input-group-addon" id="lblGiacenzaDestinazione" for="txtGiacenzaProvenienza"><asp:Localize Text="<%$ Resources: GiacenzaEsclusaQuestaRiga %>" runat="server"></asp:Localize>:</label>
                        <input name="txtGiacenzaDestinazione" id="txtGiacenzaDestinazione" class="form-control" readonly />
                    </div>
                </div>
            </div>
        </li>

        <li class="k-state-active k-active" id="panelBar_Denuncia">
            <span class="k-link k-state-selected k-selected text-uppercase"><asp:Localize Text="<%$ Resources: DENUNCIA %>" runat="server"></asp:Localize></span>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblNrDenuncia" for="txtNrDenuncia"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroDenuncia %>" runat="server"></asp:Localize>:</label>
                        <input name="txtNrDenuncia" id="txtNrDenuncia" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblDataDenuncia" for="idDataDenuncia"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDenuncia %>" runat="server"></asp:Localize>:</label>
                        <input name="idDataDenuncia" id="idDataDenuncia" class="kendoDatePicker" style="width: 100%;" maxlength="10" />
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group">
                        &nbsp;
                    </div>
                </div>
            </div>
        </li>

        <li class="k-state-active k-active" id="panelBar_Prodotto">
            <span class="k-link k-state-selected k-selected text-uppercase"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prodotto %>" runat="server"></asp:Localize></span>
            <div class="row" style="margin-top: 10px;">
                <div class="col-lg-5 col-md-5 col-sm-12">
                    <div class="input-group" id="groupCategorieMagazzino">
                        <label class="input-group-addon" id="lblCategorieMagazzino" for="ddlCategorieMagazzino"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaProdotto %>" runat="server"></asp:Localize>:</label>
                        <input name="ddlCategorieMagazzino" id="ddlCategorieMagazzino" class="form-control" />
                    </div>
                </div>
                <% If DirectCast(Me.Page, AgroAgenda_2010.DocContabile).Master.Master_versione = "2022" %>
                <div class="col-lg-4 col-md-4 col-sm-12">
                <% Else %>
                <div class="col-lg-2 col-md-2 col-sm-12">
                <% End If %>
                    <div class="input-group" id="groupPUARegolamento">
                        <label class="input-group-addon" id="lblPUARegolamento" for="ddlPUARegolamento"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Regolamento %>" runat="server"></asp:Localize>:</label>
                        <input name="ddlPUARegolamento" id="ddlPUARegolamento" class="form-control" required />
                    </div>
                <% If DirectCast(Me.Page, AgroAgenda_2010.DocContabile).Master.Master_versione = "2022" %>
                </div>
                <% Else %>
                </div>
                <% End If %>

                <% If DirectCast(Me.Page, AgroAgenda_2010.DocContabile).Master.Master_versione = "2022" %>
                <div class="col-lg-3 col-md-3 col-sm-12">
                <% Else %>
                <div class="col-lg-2 col-md-2 col-sm-12">
                <% End If %>
                    <div class="input-group">
                        <button class="btn btn-info text-uppercase" id="btnNuovoProdotto" onclick="ApriEditProdotto()" type="button" style="padding-left: 15px; padding-right: 15px;">
                            <% If DirectCast(Me.Page, AgroAgenda_2010.DocContabile).Master.Master_versione = "2022" %><i class="fa fa-plus"></i><% End If %><asp:Localize Text="<%$ Resources: NuovoProdotto %>" runat="server"></asp:Localize>
                        </button>
                    </div>
                <% If DirectCast(Me.Page, AgroAgenda_2010.DocContabile).Master.Master_versione = "2022" %>
                </div>
                <% Else %>
                </div>
                <% End If %>
            </div>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-10 col-md-10 col-sm-12">
                    <div class="input-group" id="groupDdlProdotto">
                        <label class="input-group-addon" id="lblProdottoDes" for="ddlProdottoDes"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prodotto %>" runat="server"></asp:Localize>:</label>
                        <input name="ddlProdottoDes" id="ddlProdottoDes" class="form-control" required/>
                        <label id="lblInserireCaratteri" class="input-group-addon "><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InserireTreCaratteriDellaDescrizione %>" runat="server"></asp:Localize></label>
                    </div>
                </div>

                <div class="col-lg-2 col-md-2 col-sm-12">
                    <button class="btn btn-info" id="BtnInfo_Fito" onclick="InfoProfitosan()" type="button" 
                        title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, ConsultaProfitosan %>' runat='server'></asp:Localize>" 
                        style="padding-left: 15px; padding-right: 15px;">
                        <img src="../AB_Immagini/icone24/profitosan2.png" style="width: 20px; margin-right: 10px;"/>
                        Profitosan
                    </button>
                    <div id="BtnInfo_Concime" class="fa fa-3x fa-info-circle" style="color: darkred; cursor: pointer; display: inline;"
                         onclick="InfoFertilizzante();" role="button"
                        title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, VisualizzaDettagliProdotto %>' runat='server'></asp:Localize>"></div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;">
                <div class="col-sm-8">
                    <div class="input-group" id="groupDdlProdAlias">
                        <label class="input-group-addon" id="lblProdAlias" for="ddlProdAlias"><asp:Localize Text="<%$ Resources: EtichettaAlternativa %>" runat="server"></asp:Localize>:</label>
                        <select name="ddlProdAlias" id="ddlProdAlias" class="form-control"></select>
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="input-group" id="groupExtra_Str">
                        <label class="input-group-addon" id="lblExtra_Str" for="txtExtra_Str"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DescrizioneAddizionale %>" runat="server"></asp:Localize>:</label>
                        <input name="txtExtra_Str" id="txtExtra_Str" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="input-group" id="groupBeniStrumentali">
                        <label class="input-group-addon" id="lblBeniStrumentali" for="txtBeniStrumentali"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server"></asp:Localize>:</label>
                        <input name="txtBeniStrumentali" id="txtBeniStrumentali" class="form-control" />
                    </div>
                </div>
            </div>

            <!--  INIZIO CAMPI FERTILIZZANTI -->
            <div class="row" id="idDettagliFertilizzante" style="display: none;">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div id="bloccoN" class="input-group">
                        <label class="input-group-addon" id="lbl_N" for="idTxt_N">N:</label>
                        <input name="idTxt_N" id="idTxt_N" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div id="bloccoP2O5" class="input-group">
                        <label class="input-group-addon" id="lbl_P2O5" for="idTxt_P2O5">P2O5:</label>
                        <input name="idTxt_P2O5" id="idTxt_P2O5" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div id="bloccoK2O" class="input-group">
                        <label class="input-group-addon" id="lbl_K2O" for="idTxt_K2O">K2O:</label>
                        <input name="idTxt_K2O" id="idTxt_K2O" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div id="bloccoCu" class="input-group">
                        <label class="input-group-addon" id="lbl_Cu" for="idTxt_Cu">Cu:</label>
                        <input name="idTxt_Cu" id="idTxt_Cu" class="form-control" />
                    </div>
                </div>
            </div>
            <!--  FINE CAMPI FERTILIZZANTI -->

            <!--  INIZIO INFO SUL PRODOTTO -->
            <div class="row" style="margin-top: 5px;" id="idInfoSulProdotto">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblInfoSulProdotto" for="txtInfoSulProdotto"></label>
                    </div>
                </div>
            </div>
            <!-- FINE INFO SUL PRODOTTO -->

            <div class="row" style="margin-top: 5px;" id="idLottoImpianto">
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group" id="groupLottoImpianto">
                        <label class="input-group-addon" id="lblLottoImpianto" for="ddlLottoImpianto"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Lotto %>" runat="server"></asp:Localize>:</label>
                        <input name="ddlLottoImpianto" id="ddlLottoImpianto" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group" id="groupAggregaLottoImpianto">
                        <label class="input-group-addon" id="lblAggregaLottoImpianto" for="chkAggregaLottoImpianto">DA APPROFONDIRE Aggrega i lotti impianto:</label>
                        <input type="checkbox" name="chkAggregaLottoImpianto" id="chkAggregaLottoImpianto" class="kendoSwitch" />
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;" id="idTxtLottoAccettazione">
                <div class="col-lg-7 col-md-7 col-sm-12">
                    <div class="input-group" id="groupTxtLottoAccettazione">
                        <label class="input-group-addon" id="lblTxtLottoAccettazione" for="txtLottoAccettazione"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Lotto %>" runat="server"></asp:Localize>:</label>
                        <input name="txtLottoAccettazione" id="txtLottoAccettazione" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group" id="groupConfezionamentoLotto">
                        <label class="input-group-addon" id="lblConfezionamentoLotto" for="ddlConfezionamentoLotto">Confez.Lotto:</label>
                        <input name="ddlConfezionamentoLotto" id="ddlConfezionamentoLotto" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12" id="divColDataScadenza">
                    <div class="input-group" id="groupDataScadenza">
                        <label class="input-group-addon" id="lblDataScadenza" for="dpDataScadenza">Data Scadenza:</label>
                        <input name="dpDataScadenza" id="dpDataScadenza" />
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;" id="idDdlLottoAccettazione">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="input-group" id="groupDdlLottoAccettazione">
                        <label class="input-group-addon" id="lblDdlLottoAccettazione" for="ddlLottoAccettazione"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, LottoProdotto %>" runat="server"></asp:Localize>:</label>
                        <input name="ddlLottoAccettazione" id="ddlLottoAccettazione" class="form-control" />
                    </div>
                </div>
            </div>
                     
            <div class="row" id="id_row_button_scelta_da_giacenza" style="margin-bottom: 5px;" >
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <button class="btn btn-primary xi-btn-primary" id="btn_scelta_da_giacenza_formProdottoUC" onclick="btnCaricaGiacenzeFF_click()" type="button">
                        <i class="fa fa-cube"></i><asp:Localize Text="<%$ Resources: MostraGiacenze %>" runat="server"></asp:Localize>
                    </button>
                </div>
                <%--<div class="col-lg-10 col-md-10 col-sm-12">
                    &nbsp;
                </div>--%>
             </div>
            <div class="row" id="id_row_grid_scelta_da_giacenza">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <span id="titolo_grid_scelta_da_giacenza">
                        <i class="fa fa-info-circle"></i><asp:Localize Text="<%$ Resources: ScegliGiacenzaDaScaricare %>" runat="server">
                            Scegliere la giacenza specifica da cui scaricare il prodotto
                        </asp:Localize>:
                    </span>
                    <div style="overflow: auto; margin-top: 5px; margin-bottom: 5px;">
                        <div id="tab_grid_scelta_da_giacenza_formProdottoUC"></div>
                    </div>
                </div>
             </div>
            <div class="row" style="margin-top: 5px;" id="idParametroQualitativoCalibro">
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblCalibro" for="ddlCalibro">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametroQualitativo %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="ddlCalibro" id="ddlCalibro" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblParametroQualitativo" for="chkParametroQualitativo">Aggrega i Parametri Qualitativi:</label>
                        <input type="checkbox" name="chkParametroQualitativo" id="chkParametroQualitativo" class="kendoSwitch" />
                    </div>
                </div>
            </div>
            

            <div class="row" id="id_parametri_qualitativi_list" style="margin-top: 5px;">
                <!-- N.B. Riempita dinamicamente  -->
            </div>
          
        </li>
         
         <li class="k-state-active k-active" id="panelBar_ParametriGhG">
            <span class="k-link k-state-selected k-selected text-uppercase">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriGHG %>" runat="server"></asp:Localize>
            </span>

            <div class="row" id="id_parametri_indici_list_GHG" style="margin-top: 5px;">
                 <!-- N.B. Riempita dinamicamente  -->
            </div>
         </li>
         
       


        <li class="k-state-active k-active" id="panelBar_Quantita">
            <span class="k-link k-state-selected k-selected text-uppercase">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaQuantità %>" runat="server"></asp:Localize>
            </span>
            <%--<div class="row" id="id_row_quantita_desc" style="margin-top: 10px; margin-bottom: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <label class="input-group-addon" id="lblQtaDesc">QUANTITA</label>
                </div>
            </div>--%>
            <div class="row" style="margin-top: 10px;">
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group" id="groupUnitaMisura">
                        <label class="input-group-addon" id="lblUM" for="ddlUM">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaUDM %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="ddlUM" id="ddlUM" class="form-control" required />
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group" id="groupDoseEtichetta">
                        <label class="input-group-addon" id="lblDoseEtichetta" for="txtDoseEtichetta">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DoseEtichetta %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="txtDoseEtichetta" id="txtDoseEtichetta" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupQuantita">
                        <label class="input-group-addon" id="lblQuantita" for="idQuantita">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaQuantità %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idQuantita" id="idQuantita" class="form-control" required />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupQuantitaRiscontrata">
                        <label class="input-group-addon" id="lblQuantitaRiscontrata" for="idQuantitaRiscontrata">
                            <asp:Localize Text="<%$ Resources: QuantitaRiscontrata %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idQuantitaRiscontrata" id="idQuantitaRiscontrata" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    &nbsp;
                </div>
            </div>
            <div class="row" id="id_kg_FF" style="margin-top: 5px;">
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupKgLordi">
                        <label class="input-group-addon" id="lblKgLordi" for="idKgLordi">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PesoLordo %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idKgLordi" id="idKgLordi" class="form-control" required />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupKgNetti">
                        <label class="input-group-addon" id="lblKgNetti" for="idKgNetti">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PesoNetto %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idKgNetti" id="idKgNetti" class="form-control" required />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupTagliandoPesa" style="display:none">
                        <label class="input-group-addon campiObbligatori" id="lblTagliandoPesa" for="txtTagliandoPesa">
                            <asp:Localize Text="<%$ Resources: TagliandoPesaRCD %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="txtTagliandoPesa" id="txtTagliandoPesa" class="form-control">
                    </div>
                </div>
            </div>

            <div class="row" id="id_tara_row" style="/*margin-top: 5px;*/">
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupTara">
                        <label class="input-group-addon" id="lblTara" for="idTara">
                            <asp:Localize Text="<%$ Resources: TaraTotale %>" runat="server"></asp:Localize> KG:
                        </label>
                        <input name="idTara" id="idTara" class="form-control" required />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupTaraRiscontrata">
                        <label class="input-group-addon" id="lblTaraRiscontrata" for="idTaraRiscontrata">
                            <asp:Localize Text="<%$ Resources: TaraTotaleRiscontrata %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idTaraRiscontrata" id="idTaraRiscontrata" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    &nbsp;
                </div>
            </div>
            <div class="row" id="id_degrado_row" style="/*margin-top: 5px;*/">
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupDegradoPerc">
                        <label class="input-group-addon" id="lblDegradoPerc" for="idDegradoPerc">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PercentualeDegrado %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idDegradoPerc" id="idDegradoPerc" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupDegradoKg">
                        <label class="input-group-addon" id="lblDegradoCalc">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, KgDegrado %>" runat="server"></asp:Localize>:
                        </label>
                        <label class="input-group-addon" id="lblDegradoRisultatoCalc"></label>
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group gias-dotted-box-green gias-max-w-250" id="groupDegradoKgEffettivi">
                        <label class="input-group-addon gias-bg-white" style="background-color: orange; font-weight: bold" id="lblKgEffettiviCalc">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PesoAPagamentoKg %>" runat="server"></asp:Localize>:
                        </label>
                        <label class="input-group-addon gias-bg-white" style="background-color: orange; font-weight: bold" id="lblKgEffettiviRisultatoCalc"></label>
                    </div>
                </div>
            </div>

            <div class="row" id="id_kg_FF_riscontrati" style="/*margin-top: 5px;*/">
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupKgLordiRiscontrati">
                        <label class="input-group-addon" id="lblKgLordiRiscontrati" for="idKgLordiRiscontrati">
                            <asp:Localize Text="<%$ Resources: PesoLordoRiscontrato %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idKgLordiRiscontrati" id="idKgLordiRiscontrati" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="input-group" id="groupKgNettiRiscontrati">
                        <label class="input-group-addon" id="lblKgNettiRiscontrati" for="idKgNettiRiscontrati">
                            <asp:Localize Text="<%$ Resources: PesoNettoRiscontrato %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idKgNettiRiscontrati" id="idKgNettiRiscontrati" class="form-control" />
                    </div>
                </div>
            </div>

            <div class="row" id="rowFunzioniPesiRiscontrati" style="/*margin-top: 5px;*/">
                <div id="boxFunzioniPesiRiscontrati">
                    <div class="col-md-3 col-sm-12">
                        <button class="btn btn-info" id="btnImpostaValoriRiscontrati" type="button" title="" style="padding-left: 15px; padding-right: 15px;">
                        <asp:Localize Text="<%$ Resources: CopiaQuantitaNeiRiscontrati %>" runat="server"></asp:Localize>
                        </button>
                    </div>
                    <div class="col-md-3 col-sm-12">
                        <button class="btn btn-danger" id="btnResettaValoriRiscontrati" type="button" title="" style="padding-left: 15px; padding-right: 15px;">
                        <asp:Localize Text="<%$ Resources: EliminaQuantitaRiscontrate %>" runat="server"></asp:Localize>
                        </button>
                    </div>
                </div>
            </div>

            <div class="row" id="id_row_imballaggio" style="margin-top: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 5px;">
                        <div id="tab_imballaggi_formProdottoUC"></div>
                    </div>
                </div>

                <%--<div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblImballaggio" for="ddlImballaggio">Imballaggio:</label>
                        <input name="ddlImballaggio" id="ddlImballaggio" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblNrImballaggio" for="idNrImballaggio">Nr Imb.:</label>
                        <input name="idNrImballaggio" id="idNrImballaggio" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        &nbsp;
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblTaraImballaggio" for="idTaraImballaggio">Tara Imb.:</label>
                        <input name="idTaraImballaggio" id="idTaraImballaggio" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblNrRiscontratiImballaggio" for="idNrRiscontratiImballaggio">Imb. Riscontr.:</label>
                        <input name="idNrRiscontratiImballaggio" id="idNrRiscontratiImballaggio" class="form-control" />
                    </div>
                </div>--%>

                          <%--<div class="row" id="id_row_imballaggio" style="margin-top: 10px; margin-bottom: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <label class="input-group-addon" id="lblImballiDesc">IMBALLI</label>
                </div>
            </div>--%>


            <%--<div class="row"  style="margin-top: 5px;" id="id_row_contenitore">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblContenitore" for="ddlContenitore">Contenitore:</label>
                        <input name="ddlContenitore" id="ddlContenitore" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblNrContenitore" for="idNrContenitore">Nr Cont.:</label>
                        <input name="idNrContenitore" id="idNrContenitore" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblContPerImb" for="idContPerImb">Cont. per Imb.:</label>
                        <input name="idContPerImb" id="idContPerImb" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblTaraContenitore" for="idTaraContenitore">Tara Cont.:</label>
                        <input name="idTaraContenitore" id="idTaraContenitore" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblNrRiscontratiContenitore" for="idNrRiscontratiContenitore">Cont. Riscontr.:</label>
                        <input name="idNrRiscontratiContenitore" id="idNrRiscontratiContenitore" class="form-control" />
                    </div>
                </div>
            </div>--%>

            <%--<div class="row"  style="margin-top: 5px;" id="id_row_confezione">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblConfezione" for="ddlConfezione">Confezione:</label>
                        <input name="ddlConfezione" id="ddlConfezione" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblNrConfezione" for="idNrConfezione">Nr Confez.:</label>
                        <input name="idNrConfezione" id="idNrConfezione" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblConfPerCont" for="idConfPerCont">Confez. per Cont.:</label>
                        <input name="idConfPerCont" id="idConfPerCont" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblTaraConfezione" for="idTaraConfezione">Tara Confez.:</label>
                        <input name="idTaraConfezione" id="idTaraConfezione" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblNrRiscontratiConfezione" for="idNrRiscontratiConfezione">Confez. Riscontr.:</label>
                        <input name="idNrRiscontratiConfezione" id="idNrRiscontratiConfezione" class="form-control" />
                    </div>
                </div>
            </div>--%>
            </div>
        </li>
        <li class="k-state-active k-active" id="panelBar_DatiEconomici">
            <span class="k-link k-state-selected k-selected text-uppercase">
                <asp:Localize Text="<%$ Resources: DatiEconomici %>" runat="server"></asp:Localize>
            </span>

            <div class="row" id="id_prezzi_iva_importi" style="margin-top: 5px;">
                <div class="col-lg-3 col-md-3 col-sm-10" style="display: block;">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblPrezzo" for="idPrezzo">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prezzo %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idPrezzo" id="idPrezzo" class="form-control text-right" />
                    </div>
                </div>
                <div class="col-lg-1 col-md-1 col-sm-2">
                    <div class="btn btn-success xonne-btn-primary" id="btn_ricerca_prezzo_listino" style="display:none;" 
                        title="<asp:Localize Text='<%$ Resources: AggiornaDaListino %>' runat='server'></asp:Localize>">
                        <i class="fa fa-euro"></i>
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group" id="prezzoRiferitoAGroup">
                        <label class="input-group-addon" id="lblPrezzoRiferitoA" for="ddlPrezzoRiferitoA">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PrezzoRiferitoA %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="ddlPrezzoRiferitoA" id="ddlPrezzoRiferitoA" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12">
                    <div class="input-group" id="valoreRiferimentoGroup">
                        <label class="input-group-addon" id="lblValoreRiferimento" for="ddlValoreRiferimento">
                            <asp:Localize Text="<%$ Resources: EseguiCalcoliAPartireDa %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="ddlValoreRiferimento" id="ddlValoreRiferimento" class="form-control" />
                    </div>
                </div>

                <%--TODO 
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblListino" for="ddlListino">Listino:</label>
                        <input name="ddlListino" id="ddlListino" class="form-control" />
                    </div>
                </div>--%>

            </div>
            <div class="row">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group" id="imponibileGroup">
                        <label class="input-group-addon" id="lblImponibileTotale" for="idImponibileTotale">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Imponibile %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idImponibileTotale" id="idImponibileTotale" class="form-control text-right" />
                    </div>
                </div>
            </div>
            

            <div class="k-card-list">
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="k-card k-state-error" id="cardIva" style="margin-bottom: 10px;">
                        <div class="k-card-body" style="padding: 10px 5px 5px 5px !important;">
                            <h6 class="k-card-title text-uppercase">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Iva %>" runat="server"></asp:Localize></h6>
                            <!--<h6 class="k-card-subtitle">I.V.A.</h6>-->

                            <div class="row" style="margin-top: 5px;" id="id_iva_tipo">
                                <div class="col-lg-12 col-md-12 col-sm-12 padding-5">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lblCodIva" for="ddlCodIva">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AliquotaIva %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input name="ddlCodIva" id="ddlCodIva" class="form-control" />
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 5px;" id="id_iva_valore">
                                
                                <div class="col-lg-5 col-md-5 col-sm-12 padding-5">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lblIva" for="idIva" style="min-width: 50px;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Iva %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input name="idIva" id="idIva" class="form-control text-right" />
                                    </div>
                                </div>
                                <div class="col-lg-7 col-md-7 col-sm-12 padding-5">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lblForzaIva" for="chkForzaIva">
                                            <asp:Localize Text="<%$ Resources: InserimentoForzatoIva %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input type="checkbox" name="chkForzaIva" id="chkForzaIva" class="kendoSwitch" />
                                    </div>
                                </div>
                            </div>

                        </div>

                    </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="k-card k-state-warning" id="cardSconti" style="margin-bottom: 10px;">
                        <div class="k-card-body" style="padding: 10px 5px 5px 5px !important;">
                            <h6 class="k-card-title"><asp:Localize Text="<%$ Resources: Sconti %>" runat="server"></asp:Localize></h6>
                            <!--<h6 class="k-card-subtitle">Sconti</h6>-->

                            <div class="row" style="margin-top: 5px;" id="id_sconti_tipo">
                                <div class="col-lg-7 col-md-7 col-sm-12 padding-5">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lblScontoModalita" for="ddlScontoModalita" style="min-width: 50px;">
                                            <asp:Localize Text="<%$ Resources: ScontiModalita %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input name="ddlScontoModalita" id="ddlScontoModalita" class="form-control" />
                                    </div>
                                </div>

                                <div class="col-lg-3 col-md-3 col-sm-12 padding-5">
                                    <div class="input-group" style="display: none;">
                                        <label class="input-group-addon" id="lblScontoMagg" for="ddlScontoMagg">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScontoMaggiorazione %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input name="ddlScontoMagg" id="ddlScontoMagg" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-lg-5 col-md-5 col-sm-12">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lblScontoBase" for="idScontoBase">
                                            <asp:Localize Text="<%$ Resources: ScontoBase %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input name="idScontoBase" id="idScontoBase" class="form-control" />
                                    </div>
                                </div>

                            </div>
                            <div class="row" style="margin-top: 5px;" id="id_sconti_numerico">
                                <div class="col-lg-12 col-md-12 col-sm-12 padding-5">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lblScontoAddiz1" for="idScontoAddiz1">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScontiAddizionali %>" runat="server"></asp:Localize>:
                                        </label>
                                        <div class="row">
                                            <div class="col-lg-3 col-md-3 col-sm-12 padding-5">
                                                <input name="idScontoAddiz1" id="idScontoAddiz1" class="form-control" />
                                            </div>
                                            <div class="col-lg-1 col-md-1 col-sm-12 padding-5">
                                                <span class="fa fa-plus fa-2x"></span>
                                            </div>
                                            <div class="col-lg-3 col-md-3 col-sm-12 padding-5">
                                                <input name="idScontoAddiz2" id="idScontoAddiz2" class="form-control" />
                                            </div>
                                            <div class="col-lg-1 col-md-1 col-sm-12 padding-5">
                                                <span class="fa fa-plus fa-2x"></span>
                                            </div>
                                            <div class="col-lg-3 col-md-3 col-sm-12 padding-5">
                                                <input name="idScontoAddiz3" id="idScontoAddiz3" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div class="k-card-footer" style="padding: 10px 5px 5px 5px !important;">
                            <div class="col-lg-6 col-md-6 col-sm-12 padding-5">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lblScontoCalcolato" for="idScontoCalcolato">
                                        <asp:Localize Text="<%$ Resources: ScontoCalcolato %>" runat="server"></asp:Localize>:
                                    </label>
                                    <input name="idScontoCalcolato" id="idScontoCalcolato" class="form-control" />
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 padding-5">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lblScontoCalcolatoEuro" for="idScontoCalcolatoEuro">
                                        <asp:Localize Text="<%$ Resources: ScontoTotale %>" runat="server"></asp:Localize>:
                                    </label>
                                    <input name="idScontoCalcolatoEuro" id="idScontoCalcolatoEuro" class="form-control text-right" />
                                </div>
                            </div>

                        </div>
                    </div>
                    </div>
                </div>
            </div>
                
            <div class="row" style="margin-top: 5px;" id="id_prezzo_sconto_ricavati">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblPrezzoNetto" for="idPrezzo">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PrezzoNetto %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idPrezzoNetto" id="idPrezzoNetto" class="form-control text-right" readonly />
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    &nbsp;
                </div>
            </div>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group" id="imponibileNettoGroup">
                        <label class="input-group-addon" id="lblImponibileTotaleNetto" for="idImponibileTotaleNetto">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImponibileNetto %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idImponibileTotaleNetto" id="idImponibileTotaleNetto" class="form-control text-right" />
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;" id="id_importi">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblImportoUnitario" for="idImportoUnitario">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImportoUnitario %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idImportoUnitario" id="idImportoUnitario" class="form-control text-right" />
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblImportoTotale" for="idImportoTotale">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImportoTotale %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idImportoTotale" id="idImportoTotale" class="form-control text-right" />
                    </div>
                </div>
            </div>
        </li>
        <li class="k-state-active k-active" id="panelBar_Imputazioni">
            <span class="k-link k-state-selected k-selected text-uppercase">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Imputazioni %>" runat="server"></asp:Localize>
            </span>
            <%--<div class="row" id="id_row_imputazioni_desc" style="margin-top: 10px; margin-bottom: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <label class="input-group-addon" id="lblImputazioni">IMPUTAZIONI</label>
                </div>
            </div>--%>
            <div class="row" id="id_anno" style="margin-top: 5px;">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblAnno" for="ddlAnno">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Anno %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="ddlAnno" id="ddlAnno" class="form-control" />
                    </div>
                </div>
                <div class="col-lg-9 col-md-9 col-sm-12">
                    &nbsp;
                </div>
            </div>
            <div class="row" id="id_conto_econ" style="margin-top: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblContoEconomico" for="ddlContoEconomico">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ContoEconomico %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="ddlContoEconomico" id="ddlContoEconomico" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row" id="id_conto_pat" style="margin-top: 5px;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblContoPatrimoniale" for="ddlContoPatrimoniale">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ContoPatrimoniale %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="ddlContoPatrimoniale" id="ddlContoPatrimoniale" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row" id="id_provv" style="margin-top: 5px;">
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblProvvigioni" for="idProvvigioni">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provvigione %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="idProvvigioni" id="idProvvigioni" class="form-control text-right" />
                    </div>
                </div>
                <div class="col-lg-9 col-md-9 col-sm-12">
                    &nbsp;
                </div>
            </div>
        </li>
        <li class="k-state-active k-active" id="panelBar_Note">
            <span class="k-link k-state-selected k-selected text-uppercase">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NOTE %>" runat="server"></asp:Localize> / 
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Riferimenti %>" runat="server"></asp:Localize>
            </span>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-4 col-md-6 col-sm-12" id="colRigaRifNOrdine">
                    <div class="input-group" id="groupRigaRifNOrdine">
                        <label class="input-group-addon" id="lbl_rif_n_ordine" for="rigaRifNOrdine">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroOrdine %>" runat="server">Riferimento Ordine</asp:Localize>:
                            <!-- placeholder, viene sostituito in base all'effettivo tipo del documento in DocContabile_eventi.js -->
                        </label>
                        <input id="rigaRifNOrdine" name="rigaRifNOrdine" class="form-control" type="text"/>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-12" id="colRigaRifDataOrdine">
                    <div class="input-group" id="groupRigaRifDataOrdine">
                        <label class="input-group-addon" id="lbl_rif_data_ordine" for="rigaRifDataOrdine">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataOrdine %>" runat="server"></asp:Localize>:
                            <!-- placeholder, viene sostituito in base all'effettivo tipo del documento in DocContabile_eventi.js -->
                        </label>
                        <input id="rigaRifDataOrdine" name="rigaRifDataOrdine" class="kendoCalendar" type="date" style="width: 100%;" MaxLength="10"/>
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 5px;">
                <div class="col-lg-4 col-md-6 col-sm-12" id="colNotaDDTEsterna">
                    <div class="input-group" id="groupNotaDDTEsterna">
                        <label class="input-group-addon" id="lbl_nota_ddt_esterna" for="notaDDTEsterna">
                            <asp:Localize Text="<%$ Resources: RiferimentoDocEsterno %>" runat="server">Rif. Doc. Esterno</asp:Localize>:
                        </label>
                        <input id="notaDDTEsterna" name="notaDDTEsterna" class="form-control" type="text"/>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-12" id="colNotaRigaDDTEsterna">
                    <div class="input-group" id="groupNotaRigaDDTEsterna">
                        <label class="input-group-addon" id="lbl_nota_riga_ddt_esterna" for="notaRigaDDTEsterna">
                            <asp:Localize Text="<%$ Resources: RiferimentoRigaDocEsterno %>" runat="server">Rif. Riga Doc. Esterno</asp:Localize>:
                        </label>
                        <input id="notaRigaDDTEsterna" name="notaRigaDDTEsterna" class="form-control" type="text"/>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6 col-sm-12" id="colNotaDataDDTEsterna">
                    <div class="input-group" id="groupNotaDataDDTEsterna">
                        <label class="input-group-addon" id="lbl_nota_data_ddt_esterna" for="notaDataDDTEsterna">
                            <asp:Localize Text="<%$ Resources: DataDocEsterno %>" runat="server">Data Doc. Esterno</asp:Localize>:
                        </label>
                        <input id="notaDataDDTEsterna" name="notaDataDDTEsterna" class="kendoCalendar" type="date" style="width: 100%;" MaxLength="10"/>
                    </div>
                </div>
            </div>
            <div class="row" id="idNote">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon required" id="lblIdBoxNote" for="txtBoxNote">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server"></asp:Localize>:
                        </label>
                        <input name="txtBoxNote" id="txtBoxNote" class="form-control" />
                    </div>
                </div>
            </div>
        </li>

    </ul>

    <div class="row" style="margin-top: 5px;">
        <div class="col-lg-12 col-md-12 col-sm-12 text-right">
                <%-- Utilizzato solo per il salvataggio di testata e prima riga di un nuovo documento --%>         
                <%--<div class="btn btn-success submit" id="btn_SalvaRigaDoc2">
                    <i class="fa fa-floppy-o"></i>Salva Riga
                </div>--%>
               <div class="btn btn-success" id="btn_SalvaENuovoRiga2" style="width: 200px; display: none;" onclick="SalvaENuovaRigaDoc_click()">
                <span class="lampeggiante"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaENuovaRiga %>" runat="server"></asp:Localize></span>
                </div>

                <div class="btn btn-danger" id="btn_AnnullaModifiche2" style="width: 200px;" onclick="AnnullaModificheRigaDoc_click()">
                    <span class="lampeggiante"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AnnullaModificheRiga %>" runat="server"></asp:Localize></span>
                </div>
        
                <div class="btn btn-warning" id="btn_EsciRigaDoc2" style="width: 150px;" onclick="EsciRigaDoc_click()">
                    <span class="lampeggiante"><asp:Localize Text="<%$ Resources: EsciDallaRiga %>" runat="server"></asp:Localize></span>
                </div>
        </div>
    </div>

</div>
<!-- fine container -->
<div id="confermaCambioLottoAccettazioneUCDialog"></div>
<div id="confermaAnnullamentoFormProdottoUCDialog"></div>
<div id="infoFertilizzanteDialog"></div>
<div id="datiRiepilogativiPomodoroDialog"></div>
<div id="windowRiepilogoPrezziListini"></div>
<div id="formProdottoUC_dialogConfermaGenerica"></div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/FormProdottoUC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/FormProdottoUC_ImpostaVisibilita.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/FormProdottoUC_FreshAndFood.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/FormProdottoUC_CreaControlli.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/FormProdottoUC_ws_client.js")) %>"></script>

<script type="text/javascript">

    var hdKendo_RigaDoc = $("#<%=hdKendo_RigaDoc.ClientID() %>").val();
    var hf_key_mov_dett = $("#<%=hf_key_mov_dett.ClientID() %>").val();
    var hf_riga_DataOraUltimaLettura = $("#<%=hf_riga_DataOraUltimaLettura.ClientID() %>").val();
    var hf_Cal_Cod = parseInt($("#<%=hf_Cal_Cod.ClientID() %>").val());
    var hf_Qta_Extra = kendo.parseFloat($("#<%=hf_Qta_Extra.ClientID() %>").val());
    var hf_Udm_Cod_Extra = parseInt($("#<%=hf_Udm_Cod_Extra.ClientID() %>").val());

    var hf_PendenzaIniziale = $("#<%=hf_PendenzaIniziale.ClientID() %>").val();
    var hf_ArrayElemCodUdmCod = $("#<%=hf_ArrayElemCodUdmCod.ClientID() %>").val();

    var hf_Categorie_Magazzino = "<%= hf_Categorie_Magazzino.ClientID() %>";
    var hf_Categoria_Magazzino_Dft = "<%= hf_Categoria_Magazzino_Dft.ClientID() %>";
    var hf_filtroMateriePrimeConferimento = "<%= hf_filtroMateriePrimeConferimento.ClientID() %>";
    var hf_LinkEditProdotto = "<%= hf_LinkEditProdotto.ClientID() %>";
    var hf_indirizzoProfitosan = "<%= hf_indirizzoProfitosan.ClientID() %>";
    var hf_LinkWsFitofarmaci = $("#<%=hf_LinkWsFitofarmaci.ClientID() %>").val();

    var hdKendo_SceltaDaGiacenza_FormProdottoUC = $("#<%=hdKendo_SceltaDaGiacenza_FormProdottoUC.ClientID() %>").val();
    
</script>

<script id="aggiuntaLottoAccettazioneTemplate" type="text/x-kendo-template">
    <div>
        <asp:Localize Text="<%$ Resources: VuoiAggiungereIlLottoX %>" runat="server">Elemento non trovato. Vuoi aggiungere il lotto -</asp:Localize> '#: instance.filterInput.val() #' ?
    </div>
    <br />
    <button class="btn btn-warning" onclick="aggiungiNuovoLotto('#: instance.element[0].id #', '#: instance.filterInput.val() #')">
        <asp:Localize Text="<%$ Resources: AggiungiLotto %>" runat="server"></asp:Localize>
    </button>
</script> 

<script id="ddlCodIvaTemplate" type="text/x-kendo-template">#:Sigla_IVA## if (NaturaEsclusione_2 != "") { # [#:NaturaEsclusione_2#]# } #</script> 
