<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/OperazioneBootstrap.master" CodeBehind="RilieviBS.aspx.vb" Inherits="AgronicaDomandaIrrigua.RilieviBS"  ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Master/OperazioneBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentOperazioniHeader" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RilieviBS.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RilieviBSjQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RilieviBSKendo.js") %>"></script>

    <style type="text/css">
        #panelBarVisite
        .form-control, /* if this class is applied to a Kendo UI widget, its layout may change */
        .container, .container-fluid, .row,
        .col-xs-1, .col-sm-1, .col-md-1, .col-lg-1,
        .col-xs-2, .col-sm-2, .col-md-2, .col-lg-2,
        .col-xs-3, .col-sm-3, .col-md-3, .col-lg-3,
        .col-xs-4, .col-sm-4, .col-md-4, .col-lg-4,
        .col-xs-5, .col-sm-5, .col-md-5, .col-lg-5,
        .col-xs-6, .col-sm-6, .col-md-6, .col-lg-6,
        .col-xs-7, .col-sm-7, .col-md-7, .col-lg-7,
        .col-xs-8, .col-sm-8, .col-md-8, .col-lg-8,
        .col-xs-9, .col-sm-9, .col-md-9, .col-lg-9,
        .col-xs-10, .col-sm-10, .col-md-10, .col-lg-10,
        .col-xs-11, .col-sm-11, .col-md-11, .col-lg-11,
        .col-xs-12, .col-sm-12, .col-md-12, .col-lg-12 {
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }

        /*Per far sì che le dropdown kendo non sbordino*/
        .k-widget.k-dropdown {
            width: 100%;
            display: table !important;
            table-layout: fixed;
        }

        .text-uppercase {
            text-transform: uppercase;
        }
    </style>
    <div class="btn btn-info " id="btnPassaAiDati" style="display:block;width:100%;border:0px;margin-bottom:10px;" style="display:none" onclick="ScorriGiu();">Passa ai dati</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentOperazioniContenuti" runat="server">

    <input type="hidden" id="hdRilievi" name="hdRilievi"  runat="server" />
    <input type="hidden" id="hdImpianti" name="hdImpianti" runat="server" />
    <input type="hidden" id="hdLav_Cod" runat="server" />
    <input type="hidden" id="hdTipoOperazione" runat="server" />
    <input type="hidden" id="hdSelezioneCategorieVisite" runat="server" />

    <input type="hidden" id="hdPreset_RilievoIndiciMaturita" />
    <input type="hidden" id="hdPreset_RilievoIndiciReseRaccolta" />
    <input type="hidden" id="hdPreset_RilievoAvversitaInCampo" />
    <input type="hidden" id="hdPreset_RilievoFasiFenologiche" />
    <input type="hidden" id="hdPreset_RilievoErbeInfestanti" />
    <input type="hidden" id="hdPreset_RilievoDanniRaccolta" />
    <input type="hidden" id="hdPreset_Visita" />

    <div class="jumbotron xo-rilievi-bs" style="padding-left: 15px; padding-right: 15px;">
        <div class="form-horizontal" style="padding-left: 15px; padding-right: 15px;">
            <div class="form-group">

                <ul id="panelBarVisite">
                    <li class="k-state-active k-active" id="panelBarDatiGenerali" runat="server" style="display:none">
		                <span class="k-link text-uppercase"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiGenerali %>" runat="server">Dati Generali</asp:Localize></span>
                        <div style="padding-top:20px; padding-bottom:20px;">
                            <!-- NOTA GENERICA DELLA VISITA -->
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info " id="lblNote" for="txtNote">
                                            <asp:Localize meta:resourcekey="lblNote" runat="server">Nota Generale della Visita:</asp:Localize>
                                        </label>
                                        <asp:textbox class="form-control" id="txtNote" aria-describedby="txtNote" type="text" runat="server" />
                                    </div>
                                </div>
                            </div>

                            <!-- CATEGORIE OPERAZIONI DA INCLUDERE NELLE VISITE -->
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12" style="display:none">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info " id="lblCategoriaLiv1" for="comboCategoriaLiv1">
                                            <asp:Localize Text="<%$ Resources:lblCategoriaLiv1 %>" runat="server">Categoria Liv1:</asp:Localize>
                                        </label>
                                        <select multiple="multiple" class="form-control" id="comboCategoriaLiv1" aria-describedby="comboCategoriaLiv1"></select>
                                    </div>
                                </div>

                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12"><!--class="col-lg-6 col-md-6 col-sm-6 col-xs-12"-->
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info " id="lblCategoriaLiv2" for="comboCategoriaLiv2">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Attività %>" runat="server">Attività</asp:Localize>:
                                        </label>
                                        <select multiple="multiple" class="form-control" id="comboCategoriaLiv2" aria-describedby="comboCategoriaLiv2"></select>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

	                <li class="k-state" id="panelBarAvversitaInCampo" runat="server" style="display:none"><!-- agroAccordion-->
		                <span class="k-link text-uppercase"><asp:Localize Text="<%$ Resources:RilieviAvversità %>" runat="server">Rilievi Avversità</asp:Localize></span> <!--agroAccordionTitolo-->
                        <div style="padding-top:20px; padding-bottom:20px;">
                            <!-- CONTROLLI RILIEVI AVVERSITA IN CAMPO -->
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="RilievoAvversitaInCampo">
                                        <label class="input-group-addon control-label alert-info " id="lblRilievoAvversitaInCampo" for="comboRilievoAvversitaInCampo">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Avversità %>" runat="server">Avversità</asp:Localize>:
                                        </label>
                                        <input class="form-control" id="comboRilievoAvversitaInCampo" aria-describedby="comboRilievoAvversitaInCampo" type="text" />
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="QuantitàAvversitaInCampo">
                                        <label class="input-group-addon control-label alert-info " id="lblQtaRilevataAvversitaInCampo" for="comboQtaRilevataAvversitaInCampo">
                                            <asp:Localize Text="<%$ Resources:QuantitàRilevata %>" runat="server">Quantità Rilevata</asp:Localize>: (*)
                                        </label>
                                        <input class="form-control" id="comboQtaRilevataAvversitaInCampo" aria-describedby="QtaRilevataAvversitaInCampo" type="text" style="display:none" />
                                        <input class="form-control" id="txtQtaRilevataAvversitaInCampo" aria-describedby="QtaRilevataAvversitaInCampo" type="text" />
                                    </div>
                                </div>
                            </div>

                            <div class="row" id="labelSoglie" runat="server" style="display:none">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-left">
                                    <span class="fa fa-warning"></span>
                                  <label class=" alert-info" style=" background-color:yellow">
                                      <asp:Localize Text="<%$ Resources:InGialloSogliaDisciplinare %>" runat="server"></asp:Localize>
                                  </label>
                                </div>
                            </div>

                            <!-- PULSANTE AGGIUNGI RILIEVO AVVERSITA' -->
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-center">
                                    <div class="btn btn-info" id="AggiungiRilievoAvversitaInCampo" style="min-width:250px;" onclick="AggiungiRilievoAvversitaInCampo();">
                                        <i class="fa fa-arrow-down"></i><span>
                                            <asp:Localize Text="<%$ Resources:AggiungiRilievoAvversità %>" runat="server">Aggiungi Rilievo Avversità</asp:Localize>
                                        </span><i class="fa fa-arrow-down"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

	                <li class="k-state" id="panelBarIndiciMaturita" runat="server" style="display:none"><!-- agroAccordion-->
		                <span class="k-link text-uppercase"><asp:localize Text="<%$ Resources:RilieviIndiciMaturità %>" runat="server">Rilievi Indici Maturità</asp:localize></span> <!--agroAccordionTitolo-->
                        <div style="padding-top:20px; padding-bottom:20px;">
                            <!-- CONTROLLI INDICI MATURITA -->
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="RilievoIndiciMaturita">
                                        <label class="input-group-addon control-label alert-info " id="lblRilievoIndiciMaturita" for="comboRilievoIndiciMaturita">
                                            <asp:localize Text="<%$ Resources:IndiceMaturità %>" runat="server">Indice Maturità</asp:localize>:
                                        </label>
                                        <input class="form-control" id="comboRilievoIndiciMaturita" aria-describedby="comboRilievoIndiciMaturita" type="text" />
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="QuantitàIndiciMaturita">
                                        <label class="input-group-addon control-label alert-info " id="lblQtaRilevataIndiciMaturita" for="comboQtaRilevataIndiciMaturita">
                                            <asp:Localize Text="<%$ Resources:QuantitàRilevata %>" runat="server">Quantità Rilevata</asp:Localize>: (*)
                                        </label>
                                        <input class="form-control" id="comboQtaRilevataIndiciMaturita" aria-describedby="QtaRilevataIndiciMaturita" type="text" style="display:none" />
                                        <input class="form-control" id="txtQtaRilevataIndiciMaturita" aria-describedby="QtaRilevataIndiciMaturita" type="text" />
                                    </div>
                                </div>
                            </div>

                            <!-- PULSANTE AGGIUNGI RILIEVO INDICI MATURITA' -->
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-center">
                                    <div class="btn btn-info" id="AggiungiRilievoIndiciMaturita" style="min-width:250px;" onclick="AggiungiRilievoIndiciMaturita();">
                                        <i class="fa fa-arrow-down"></i><span>
                                            <asp:Localize Text="<%$ Resources:AggiungiIndiceMaturità %>" runat="server">Aggiungi Indice Maturità</asp:Localize>
                                        </span><i class="fa fa-arrow-down"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

                    <li class="k-state" id="panelBarDanniRaccolta" runat="server" style="display:none"><!-- agroAccordion-->
		                <span class="k-link text-uppercase"><asp:Localize Text="<%$ Resources:RilieviDanniAllaRaccolta %>" runat="server">Rilievi Danni Alla Raccolta</asp:Localize></span> <!--agroAccordionTitolo-->
                        <div style="padding-top:20px; padding-bottom:20px;">
                            <!-- CONTROLLI DANNI ALLA RACCOLTA -->
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="RilievoDanniRaccolta">
                                        <label class="input-group-addon control-label alert-info " id="lblRilievoDanniRaccolta" for="comboRilievoDanniRaccolta">
                                            <asp:Localize Text="<%$ Resources:DanniAllaRaccolta %>" runat="server">Danni Alla Raccolta</asp:Localize>:
                                        </label>
                                        <input class="form-control" id="comboRilievoDanniRaccolta" aria-describedby="comboRilievoDanniRaccolta" type="text" />
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="QuantitàDanniRaccolta">
                                        <label class="input-group-addon control-label alert-info " id="lblQtaRilevataDanniRaccolta" for="comboQtaRilevataDanniRaccolta">
                                            <asp:Localize Text="<%$ Resources:QuantitàRilevata %>" runat="server">Quantità Rilevata</asp:Localize>: (*)
                                        </label>
                                        <input class="form-control" id="comboQtaRilevataDanniRaccolta" aria-describedby="QtaRilevataDanniRaccolta" type="text" style="display:none" />
                                        <input class="form-control" id="txtQtaRilevataDanniRaccolta" aria-describedby="QtaRilevataDanniRaccolta" type="text" />
                                    </div>
                                </div>
                            </div>

                            <!-- PULSANTE AGGIUNGI RILIEVO DANNI ALLA RACCOLTA -->
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-center">
                                    <div class="btn btn-info" id="AggiungiRilievoDanniRaccolta" style="min-width:250px;" onclick="AggiungiRilievoDanniRaccolta();">
                                        <i class="fa fa-arrow-down"></i><span>
                                            <asp:Localize Text="<%$ Resources:AggiungiDanniAllaRaccolta %>" runat="server">Aggiungi Danni Alla Raccolta</asp:Localize>
                                        </span><i class="fa fa-arrow-down"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

                    <li class="k-state" id="panelBarIndiciReseRaccolta" runat="server" style="display:none"><!-- agroAccordion-->
		                <span class="k-link text-uppercase"><asp:Localize Text="<%$ Resources:RilieviIndiciReseRaccolta %>" runat="server">Rilievi Indici Rese/Raccolta</asp:Localize></span> <!--agroAccordionTitolo-->
                        <div style="padding-top:20px; padding-bottom:20px;">
                            <!-- CONTROLLI INDICI RESE/RACCOLTA -->
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="RilievoIndiciReseRaccolta">
                                        <label class="input-group-addon control-label alert-info " id="lblRilievoIndiciReseRaccolta" for="comboRilievoIndiciReseRaccolta">
                                            <asp:Localize Text="<%$ Resources:IndiceReseRaccolta %>" runat="server">Indice Rese/Raccolta</asp:Localize>:
                                        </label>
                                        <input class="form-control" id="comboRilievoIndiciReseRaccolta" aria-describedby="comboRilievoIndiciReseRaccolta" type="text" />
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="QuantitàIndiciIndiciReseRaccolta">
                                        <label class="input-group-addon control-label alert-info " id="lblQtaRilevataIndiciReseRaccolta" for="comboQtaRilevataIndiciReseRaccolta">
                                            <asp:Localize Text="<%$ Resources:QuantitàRilevata %>" runat="server">Quantità Rilevata</asp:Localize>: (*)
                                        </label>
                                        <input class="form-control" id="comboQtaRilevataIndiciReseRaccolta" aria-describedby="QtaRilevataIndiciReseRaccolta" type="text" style="display:none" />
                                        <input class="form-control" id="txtQtaRilevataIndiciReseRaccolta" aria-describedby="QtaRilevataIndiciReseRaccolta" type="text" />
                                    </div>
                                </div>
                            </div>

                            <!-- PULSANTE AGGIUNGI RILIEVO RESE/RACCOLTA -->
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-center">
                                    <div class="btn btn-info" id="AggiungiRilievoIndiciReseRaccoltaa" style="min-width:250px;" onclick="AggiungiRilievoIndiciReseRaccolta();">
                                        <i class="fa fa-arrow-down"></i><span>
                                            <asp:Localize Text="<%$ Resources:AggiungiIndiciReseRaccolta %>" runat="server">Aggiungi Indici Rese/Raccolta</asp:Localize>
                                        </span><i class="fa fa-arrow-down"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

                    <li class="k-state" id="panelBarFasiFenologiche" runat="server" style="display:none"><!-- agroAccordion-->
		                <span class="k-link text-uppercase"><asp:Localize Text="<%$ Resources:RilieviFasiFenologicheScalaBBCH %>" runat="server">Rilievi Fasi Fenologiche - Scala BBCH</asp:Localize></span> <!--agroAccordionTitolo-->
                        <div style="padding-top:20px; padding-bottom:20px;">
                            <!-- CONTROLLI RILIEVI FASI FENOLOGICHE -->
                         
                            <div class="row">
                           
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="RilievoFasiFenologiche">
                                        <label class="input-group-addon control-label alert-info " id="lblRilievoFasiFenologiche" for="comboRilievoFasiFenologiche">
                                            <asp:Localize Text="<%$ Resources:FaseFenologica %>" runat="server">Fase Fenologica</asp:Localize>:
                                        </label>
                                        <input class="form-control" id="comboRilievoFasiFenologiche" aria-describedby="comboRilievoFasiFenologiche" type="text" />
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="DataFasiFenologiche">
                                        <label class="input-group-addon control-label alert-info " id="lblDataRilevataFasiFenologiche" for="comboDataRilevataFasiFenologiche">
                                            <asp:Localize Text="<%$ Resources:AgronicaAgenda_2010, Data %>" runat="server">Data</asp:Localize>: (*)
                                        </label>
                                        <input class="form-control" id="comboDataRilevataFasiFenologiche" aria-describedby="DataRilevataFasiFenologiche" type="text" style="display:none" />
                                        <input class="form-control kendoDatePicker" id="txtDataRilevataFasiFenologiche" aria-describedby="DataRilevataFasiFenologiche" type="text" />
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-left">
                                    <span class="fa fa-warning"></span>
                                  <label class=" alert-info" style=" background-color:pink">
                                      <asp:Localize Text="<%$ Resources:InRosaFaseFenologicaAssociataFioritura %>" runat="server">In rosa è indicata la Fase Fenologica associata alla Fioritura, visualizzata nel QdC</asp:Localize>
                                  </label>
                                </div>
                            </div>

                            <!-- PULSANTE AGGIUNGI RILIEVO FASE FENOLOGICA -->
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-center">
                                    <div class="btn btn-info" id="AggiungiRilievoFasiFenologiche" style="min-width:250px;" onclick="AggiungiRilievoFasiFenologiche();">
                                        <i class="fa fa-arrow-down"></i><span>
                                            <asp:Localize Text="<%$ Resources:AggiungiRilievoFaseFenologica %>" runat="server">Aggiungi Rilievo Fase Fenologica</asp:Localize></span><i class="fa fa-arrow-down"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

                    <li class="k-state" id="panelBarErbeInfestanti" runat="server" style="display:none"><!-- agroAccordion-->
		                <span class="k-link text-uppercase"><asp:Localize Text="<%$ Resources:RilieviErbeInfestanti %>" runat="server">Rilievi Erbe Infestanti</asp:Localize></span> <!--agroAccordionTitolo-->
                        <div style="padding-top:20px; padding-bottom:20px;">
                            <!-- CONTROLLI RILIEVI ERBE INFESTANTI -->
                         
                            <div class="row">
                           
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="RilievoErbeInfestanti">
                                        <label class="input-group-addon control-label alert-info " id="lblRilievoErbeInfestanti" for="comboRilievoRilievoErbeInfestanti">
                                            <asp:Localize Text="<%$ Resources:ErbeInfestanti %>" runat="server">Erbe Infestanti</asp:Localize>:
                                        </label>
                                        <input class="form-control" id="comboRilievoErbeInfestanti" aria-describedby="comboRilievoErbeInfestanti" type="text" />
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group" id="QuantitàErbeInfestanti">
                                        <label class="input-group-addon control-label alert-info " id="lblQtaRilevataErbeInfestanti" for="comboQtaRilevataErbeInfestanti">
                                            <asp:Localize Text="<%$ Resources:QuantitàRilevata %>" runat="server">Quantità Rilevata</asp:Localize>: (*)
                                        </label>
                                        <input class="form-control" id="comboQtaRilevataErbeInfestanti" aria-describedby="QtaRilevataErbeInfestanti" type="text" style="display:none" />
                                        <input class="form-control" id="txtQtaRilevataErbeInfestanti" aria-describedby="QtaRilevataErbeInfestanti" type="text" />
                                    </div>
                                </div>
                            </div>

                            <!-- PULSANTE AGGIUNGI RILIEVO Erbe Infestanti -->
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-center">
                                    <div class="btn btn-info" id="AggiungiRilievoErbeInfestanti" style="min-width:250px;" onclick="AggiungiRilievoErbeInfestanti();">
                                        <i class="fa fa-arrow-down"></i><span>
                                            <asp:Localize Text="<%$ Resources:AggiungiRilievoErbeInfestanti %>" runat="server">Aggiungi Rilievo Erbe Infestanti</asp:Localize>
                                        </span><i class="fa fa-arrow-down"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

                    <li class="k-state" id="panelBarVisitePersonalizzate" runat="server" style="display:none"><!-- agroAccordion-->
		                <span class="k-link text-uppercase"><asp:Localize Text="<%$ Resources:VisitePersonalizzate %>" runat="server">Visite Personalizzate</asp:Localize></span> <!--agroAccordionTitolo-->
                        <div style="padding-top:20px; padding-bottom:20px;">
                            <!-- CONTROLLI VISITE PERSONALIZZATE -->
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info " id="lblVisitePers" for="comboVisitePers">
                                            <asp:Localize Text="<%$ Resources:TipoVisita %>" runat="server">Tipo Visita</asp:Localize>:
                                        </label>
                                        <input class="form-control" id="comboVisitePers" aria-describedby="lblVisitePers" type="text" />
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info " id="lblVisitaPers_Descr" for="txtVisitaPers_Descr">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Descrizione %>" runat="server">Descrizione</asp:Localize>: (*)
                                        </label>
                                        <input class="form-control" id="txtVisitaPers_Descr" aria-describedby="lblVisitaPers_Descr" type="text" />
                                    </div>
                                </div>
                            </div>

                            <!-- PULSANTE AGGIUNGI VISITA PERSONALIZZATA -->
                            <div class="row" style="margin-bottom:20px;">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 text-center">
                                    <div class="btn btn-info" id="AggiungiVisitaPers" style="min-width:250px;" onclick="AggiungiVisitaPers();">
                                        <i class="fa fa-arrow-down"></i><span>
                                            <asp:Localize Text="<%$ Resources:AggiungiVisita %>" runat="server">Aggiungi Visita</asp:Localize>
                                        </span><i class="fa fa-arrow-down"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

                </ul>

            </div>
        </div>

        <!--GRIGLIA RILIEVI-->
        <div class="row" style="margin-top:20px;margin-bottom:10px;">
            <div class="col-lg-12">
                <div id="divRilievi" name="divRilievi"></div>
            </div>
        </div>

    </div>



    <script type="text/javascript">

        var indirizzohttp = "RilieviBS.aspx";

        var hdRilievi_clientID = "#<%= hdRilievi.Clientid%>";
        var hdLav_Cod_clientID = "#<%= hdLav_Cod.Clientid%>";
        var hdTipoOperazione_clientID = "#<%= hdTipoOperazione.Clientid%>";
        var hdSelezioneCategorieVisite_clientID = "#<%= hdSelezioneCategorieVisite.Clientid%>";

        var panelBarAvversitaInCampo_clientID = "#<%= panelBarAvversitaInCampo.ClientID%>";
        var labelSoglie_clientID = "#<%= labelSoglie.ClientID%>";
        var panelBarIndiciMaturita_clientID = "#<%= panelBarIndiciMaturita.ClientID%>";
        var panelBarVisitePersonalizzate_clientID = "#<%= panelBarVisitePersonalizzate.ClientID%>";
        var panelBarFasiFenologiche_clientID ="#<%= panelBarFasiFenologiche.ClientID%>";
        var panelBarErbeInfestanti_clientID = "#<%= panelBarErbeInfestanti.ClientID%>";
        var panelBarDanniRaccolta_clientID ="#<%= panelBarDanniRaccolta.ClientID%>";
        var panelBarIndiciReseRaccolta_clientID = "#<%= panelBarIndiciReseRaccolta.ClientID%>";

        var comboSpecie_clientID = "#<%= Master.Property_ComboSpecie.ddl_Specie.ClientID%>";
        var comboCentroAziendale_clientID = "#<%= Master.Property_ComboCentroAziendale.ddl_CentroAziendale.ClientID%>";
        var comboDisciplinari_clientID = "#<%= Master.Property_ComboDisciplinari.ddl_Disciplinari.ClientID%>";

        var LAVCOD_VISITA = "<%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_VISITA%>";
        var LAVCOD_RILIEVOAVVERSITAINCAMPO = "<%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO%>";
        var LAVCOD_RILIEVOINDICIMATURITA = "<%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA%>";
        var LAVCOD_ALTRELAVORAZIONI = "<%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_ALTRE_OPERAZIONI%>";
        var LAVCOD_FASIFENOLOGICHE = "<%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE%>";
        var LAVCOD_RILIEVOERBEINFESTANTI = "<%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_ERBE_INFESTANTI%>";
        var LAVCOD_RILIEVODANNIALLARACCOLTA = "<%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DANNI_RACCOLTA%>";
        var LAVCOD_RILIEVOINDICIRESERACCOLTA = "<%=AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA%>";

        var IMPOSTAZIONE_PERSON_RILIEVO_FASI_FENOLOGICHE = "<%=AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE%>";
        var IMPOSTAZIONE_PERSON_RILIEVO_INDICI_MATURITA = "<%=AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA%>";
        var IMPOSTAZIONE_PERSON_RILIEVO_DANNI_RACCOLTA = "<%=AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA%>";
        var IMPOSTAZIONE_PERSON_RILIEVO_AVVERSITA = "<%=AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA%>";
        var IMPOSTAZIONE_PERSON_RILIEVO_ERBE_INFESTANTI = "<%=AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_ERBE_INFESTANTI%>";
        var IMPOSTAZIONE_PERSON_RILIEVO_INDICI_RESE_RACCOLTA = "<%=AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA%>";

        function ValidaxSubmit() {

            //rimuovo quanto disabilitato per poter ottenere i dati lato server:
            //http://stackoverflow.com/questions/7357256/disabled-form-inputs-do-not-appear-in-the-request

            var grid = $("#divRilievi").data("kendoGrid");
            if (grid === undefined) {
                MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareAlmenoUnRilievo", "Specificare almeno un rilievo"), "DIV_Messaggi");
                return;
            }

            var data = grid.dataSource.data();
            if (data === undefined || data.length === 0){
                MessaggioErrore(Traduzione(rilieviBSResxLocal, "SpecificareAlmenoUnRilievo", "Specificare almeno un rilievo"), "DIV_Messaggi");
                return;
            }

            $(hdRilievi_clientID).val(JSON.stringify(data));

            //click per salvataggio
            $("#<%=Master.Property_ImgBtn_Salva.ClientID %>").click();

        }

	$(document).ready(function () {
		if ("<%=pivaSuperUser %>"==="00127310357"){
        //if (Request_QueryString("IncludiVisite") === "true") {
            $('#btnPassaAiDati').show();
        } else {
            $('#btnPassaAiDati').hide();
        }
	});

    function ScorriGiu(){
        window.location="#divRilievi";
    }
    </script>

</asp:Content>

