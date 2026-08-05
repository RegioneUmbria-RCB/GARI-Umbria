<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ImputazioneImpiantiUC.ascx.vb" Inherits="AgroAgenda_2010.ImputazioneImpiantiUC" %>

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

    .dpiOn {
        box-shadow: 1px 1px rgba(0,0,0,.075) inset;
        background-color: Orange;
        pointer-events: none;
    }
</style>

<div id="panelAreaImputazioneImpianti" class="panel-group searchArea" style="opacity: 1;">

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12 text-right">
             
            <div class="btn btn-danger" id="btn_AnnullaModifiche3" style="width: 200px;" onclick="AnnullaModificheRigaDoc_click()">
                <span class="lampeggiante"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AnnullaModificheRiga %>" runat="server"></asp:Localize></span>
            </div>

            <div class="btn btn-warning" id="btn_EsciRigaDoc3" style="width: 150px;" onclick="EsciRigaDoc_click()">
                <span class="lampeggiante"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Esci %>" runat="server"></asp:Localize></span>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">

            <div class="container_tabImputazioneImpianti" style="padding: 0px; /*margin-bottom: 70px*/">
                <%--<div id="tabstrip_tabImputazioneImpianti">
                        <ul>
                            <li  class="k-state-active k-active">Imputazione Impianti</li>                                
                        </ul>--%>

                <div class="panel-group Impianti">

                    <div class="row">
                        <div class="col-lg-8 col-md-10 col-sm-12">
                            <div class="input-group" id="boxChkImpiantiIndefiniti">
                                <!-- i18n Non più visualizzata -->
                                <label class="input-group-addon" id="lbImpiantiIndefiniti" for="ChkImpiantiIndefiniti">Impianti Colturali Indefiniti</label>
                                <input type="checkbox" name="ChkImpiantiIndefiniti" id="ChkImpiantiIndefiniti" class="kendoSwitch" />
                            </div>
                            <span id="warningImpiantiIndefiniti">
                                <label class="myLabelBold">
                                    <i class="fa fa-info-circle"></i>
                                    <asp:Localize Text="<%$ Resources: NessunImpiantoNonVerrannoCreateRaccolte %>" runat="server"></asp:Localize>
                                </label>
                            </span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-8 col-md-8 col-sm-12" id="id_note_raccolta">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_note_raccolta" for="txt_operazione">
                                            <asp:Localize Text="<%$ Resources: NoteRaccolta %>" runat="server"></asp:Localize>:
                                        </span>
                                        <asp:TextBox ID="txt_note_raccolta" runat="server" CssClass="form-control">
                                        </asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- richiesti da Conferimento Pomodoro -->
                    <div class="row" id="row_impianti_pomodoro" style="display:none;">                        
                        <div class="col-lg-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" id="lblCodVarietaPomodoro" for="ddlCodVarietaPomodoro">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceVarieta %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input id="ddlCodVarietaPomodoro" name="ddlCodVarietaPomodoro" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" id="lblDescAppezzamenti" for="txtDescAppezzamenti">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Appezzamenti %>" runat="server"></asp:Localize>:
                                        </label>
                                        <input id="txtDescAppezzamenti" name="txtDescAppezzamenti" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!--Filtri-->
                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-sm-12" id="id_filtroimpianti">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" for="cmbFiltroImpianti">
                                            <asp:Localize Text="<%$ Resources: FiltroImpianti %>" runat="server"></asp:Localize>:
                                        </label>
                                        <select name="FiltroImpianti" multiple="multiple" id="cmbFiltroImpianti" class="form-control"></select>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-6 col-md-6 col-sm-12" id="id_ripartizione">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" for="cmbRipartizione">
                                            <asp:Localize Text="<%$ Resources: Ripartizione %>" runat="server"></asp:Localize>:
                                        </label>
                                        <select name="Ripartizione" multiple="multiple" id="cmbRipartizione" class="form-control"></select>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12" id="id_riga_impianti">
                            <!--Griglia-->
                            <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                <div id="tab_griglia_impianti"></div>
                            </div>
                        </div>
                    </div>

                </div>

                <%--</div>--%>
            </div>

        </div>

    </div>


    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12 text-right">
            <div class="btn btn-danger" id="btn_AnnullaModifiche4" style="width: 200px;" onclick="AnnullaModificheRigaDoc_click()">
                <span class="lampeggiante"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AnnullaModificheRiga %>" runat="server"></asp:Localize></span>
            </div> 

            <div class="btn btn-warning" id="btn_EsciRigaDoc4" style="width: 150px;" onclick="EsciRigaDoc_click()">
                <span class="lampeggiante"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Esci %>" runat="server"></asp:Localize></span>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/ImputazioneImpiantiUC_globali.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/ImputazioneImpiantiUC_jQueryDocReady.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/ImputazioneImpiantiUC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneContabilita/UserControl/ImputazioneImpiantiUC_ws_client.js")) %>"></script>
