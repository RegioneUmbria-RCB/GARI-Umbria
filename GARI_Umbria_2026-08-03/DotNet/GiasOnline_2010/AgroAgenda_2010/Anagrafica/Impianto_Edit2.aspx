<%@ Page Title="Impianto Anagrafica" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Impianto_Edit2.aspx.vb" Inherits="AgroAgenda_2010.Impianto_Edit2" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveClientUrl("Impianto_Edit2.css?" & Application("GiasVersioneCorrente").ToString) %>" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="display: none">
        <div id="windowOperazione">
            <div class="row">
                <div class="col-lg-12 col-md-12">
                    <div class="input-group">
                        <span class="input-group-addon" id="lbl_operazione" for="Cmb_Operazioni">Operazione</span>
                        <input type="text" id="Cmb_Operazioni" class="form-control" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-5 col-md-5"></div>
                    <div class="col-lg-2 col-md-2">
                        <div class="btn btn-success" id="btnSalvaEOpSel" onclick="ValidaxSubmit('2');">
                            <i class="fa fa-floppy-o"></i>Salva
                        </div>
                    </div>

                    <div class="col-lg-5 col-md-5"></div>
                </div>

            </div>
        </div>
    </div>
    <div data-toggle="validator" role="form" class="row">

        <div class="col-lg-12 col-md-12">
            <div class="row">
                <div class="col-lg-8 col-md-8" style="padding: 10px 15px; line-height: 1.6;">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Centro %>" runat="server">Centro</asp:Localize>: <b id="LblCentro"></b>
                    <br />
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Campo %>" runat="server">Campo</asp:Localize>: <b id="LblCampo"></b>
                    <br />
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Appezzamento %>" runat="server">Appezzamento</asp:Localize>: <b id="LblAppezza"></b>
                    <br />
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SuperficieAppezzamentoAbbr %>" runat="server">Sup. Appezzamento</asp:Localize>
                    [Ha]: <b id="LblSubAppezza"></b>
                </div>

                <div class="col-lg-2 col-md-2 text-right">
                    <div class="btn btn-success xi-btn-primary" id="btnSalva" onclick="ValidaxSubmit();">
                        <i class="fa fa-floppy-o"></i>
                        <asp:Localize meta:resourcekey="SalvaImpianto" runat="server">Salva</asp:Localize>
                    </div>
                </div>

                <div class="col-lg-2 col-md-2 text-right">
                    <div class="btn btn-success" id="btnSalva_e_Op" onclick="InserisciOperazione();">
                        <i class="fa fa-floppy-o"></i>
                        <asp:Localize meta:resourcekey="SalvaImpiantoEOp" runat="server">Salva e Inserisci Operazione</asp:Localize>
                    </div>
                </div>

                <div id="btnSalvaScrivi" class="pull-right" style="display: none;">
                    <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown"
                        aria-haspopup="true" aria-expanded="false">
                        <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu">
                        <li><a href="#" onclick="$('#tipo_salva').val(0); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                        </a></li>
                        <li><a href="#" onclick="$('#tipo_salva').val(1); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaENuovo %>" runat="server">Salva e Nuovo</asp:Localize>
                        </a></li>
                        <li><a href="#" onclick="$('#tipo_salva').val(2); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                        </a></li>
                    </ul>
                </div>
                <asp:HiddenField ID="tipo_salva" runat="server" />
                <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                    Style="display: none" />
                <asp:ImageButton ID="ImgBtn_SalvaDistinta" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                    Style="display: none" />
            </div>
        </div>
        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">

            <ul id="Ul1" class="" data-tabs="tabs">
                <li class="active tab_dati_impianto">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiImpianto %>" runat="server">Dati Impianto</asp:Localize></li>
                <li class="tab_dati_accessori">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiAccessori %>" runat="server">Dati Accessori</asp:Localize></li>
                <!--<li class="tab_dist_prod">Esercizi</li>-->
                <!--<li class="tab_gis_sps">GIS/GPS</li>   
                <li class="tab_analisi_costi">Analisi Costi</li>   
                <li class="tab_investimento">Investimento</li>  -->
            </ul>

            <!-- TAB 1 -->
            <div class="" id="tab_dati_impianto">
                <div class="jumbotron">
                    <div class="row">
                        <div class="col-lg-12">
                            <input type="checkbox" id="ChkTerrenoNudo" class="k-checkbox" />
                            <label aria-describedby="ChkTerrenoNudo" id="lbl_TerrenoNudo" class="k-checkbox-label" for="ChkTerrenoNudo">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TerrenoNudoNessunaColtura %>" runat="server">Terreno Nudo (Nessuna Coltura)</asp:Localize>
                            </label>
                            <input type="hidden" id="InizioAppezzamento" runat="server" name="InizioAppezzamento" />
                            <input type="hidden" id="FineAppezzamento" runat="server" name="FineAppezzamento" />
                        </div>
                    </div>

                    <div class="row" id="chk_no">
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_Specie" for="Cmb_Specie">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpecieVegetale %>" runat="server">Specie Vegetale</asp:Localize>
                                            *
                                        </span>
                                        <input type="text" id="Cmb_Specie" class="kendoDropDownList" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_Finalita" for="Cmb_Finalita">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Finalità %>" runat="server">Finalità</asp:Localize>
                                            *
                                        </span>
                                        <input type="text" id="Cmb_Finalita" class="kendoDropDownList" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_Cultivar" for="Cmb_Cultivar">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server">Varietà</asp:Localize>
                                            *
                                        </span>
                                        <input type="text" id="Cmb_Cultivar" class="kendoDropDownList" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_TipologiaVarietale" for="Cmb_TipologiaVarietale">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipologiaVarietale %>" runat="server">Tipologia Varietale</asp:Localize>
                                            </span>
                                            <input type="text" id="Cmb_TipologiaVarietale" class="kendoDropDownList" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row" id="chk_yes">
                        <div class="col-lg-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_CodiciTerreno" for="Cmb_CodiciTerreno">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DestinazioneDUso %>" runat="server">Destinazione d'uso</asp:Localize>
                                            *
                                        </span>
                                        <input type="text" id="Cmb_CodiciTerreno" class="kendoDropDownList" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12" id="div_coltura_annuale">
                            <input type="checkbox" id="ChkColturaAnnuale" class="k-checkbox" />
                            <label aria-describedby="ChkColturaAnnuale" id="lblColturaAnnuale" class="k-checkbox-label"
                                for="ChkColturaAnnuale">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ColturaAnnuale %>" runat="server">Coltura Annuale</asp:Localize>
                            </label>
                        </div>

                        <div class="col-lg-12">
                            <div>
                                <i class="fa fa-exclamation-triangle"></i>
                                <small>
                                    <b>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CENTROValidità %>" runat="server">CENTRO validità:</asp:Localize></b>
                                    <i id="lbl_centro_data_inizio"></i>
                                    - 
                                        <i id="lbl_centro_data_fine"></i>
                                </small>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div>
                                <i class="fa fa-exclamation-triangle"></i>
                                <small>
                                    <b>
                                        <asp:Localize meta:resourcekey="APPEZZAMENTOValidità" runat="server">APPEZZAMENTO validità:</asp:Localize></b>
                                    <i id="lbl_appezza_data_inizio"></i>
                                    - 
                                        <i id="lbl_appezza_data_fine"></i>
                                </small>
                            </div>
                        </div>

                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_validita_inizio" for="TxtValiditaInizio">
                                            <i class="fa fa-calendar"></i>
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,DataInizioImpianto %>" runat="server">Data Inizio Impianto</asp:Localize>
                                            *
                                        </span>
                                        <input type="text" id="TxtValiditaInizio" class="form-control kendoDate required" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_validita_fine" for="TxtValiditaFine">
                                            <i class="fa fa-calendar"></i>
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,DataFineImpianto %>" runat="server">Data Fine Impianto</asp:Localize>
                                        </span>
                                        <input type="text" id="TxtValiditaFine" class="form-control kendoDate required" />
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
                                        <span class="input-group-addon alert-info" id="lbl_Superficie" for="TxtSuperficie">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,SuperficieNettaColtivataAbbr %>" runat="server">Sup. netta coltivata</asp:Localize>
                                            [Ha]
                                        </span>
                                        <input type="text" id="TxtSuperficie" class="form-control kendoDoubleNumber" />
                                    </div>
                                </div>
                            </div>
                            <span id="msgCfrGisSuperficieSenzaCatasto" class="clsCfrGisSuperficie"></span>
                        </div>

                         <!-- --- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - --->
                        <div class="col-lg-4 col-md-4 col-sm-12"> 
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_Data_Inizio_Innesto" for="Txt_DataInizioInnesto">
                                            <i class="fa fa-calendar"></i><label>
                                            <asp:Localize meta:resourcekey="DataInizioInnesto" runat="server">Data innesto varietà</asp:Localize></label>
                                        </span>
                                        <input type="text" id="Txt_DataInizioInnesto" class="form-control kendoDate required" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-12"> 
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_Data_Inizio_Produzione" for="Txt_DataInizioProduzione">
                                            <i class="fa fa-calendar"></i><label>
                                            <asp:Localize meta:resourcekey="DataInizioProduzione" runat="server">Data Inizio Produzione</asp:Localize></label>
                                        </span>
                                        <input type="text" id="Txt_DataInizioProduzione" class="form-control kendoDate required" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!--- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - - --->
                    </div>
                </div>

                <div class="jumbotron">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 text-center">
                            <h4 style="color: #052747; text-transform: uppercase;">
                                <asp:Localize meta:resourcekey="EserciziImpianti" runat="server">Esercizi Impianti</asp:Localize></h4>
                        </div>
                    </div>

                    <div class="row" style="min-height: 30px; margin-bottom: 10px;">
                        <div class="col-lg-8 col-md-8 text-right">
                        </div>

                        <div class="col-lg-2 col-md-2 text-center">
                            <div class="btn btn-success" id="btn_salva_distinta" onclick="ValidaxDistinta();">
                                <i class="fa fa-floppy-o"></i>
                                <asp:Localize meta:resourcekey="SalvaEsercizio" runat="server">Salva Esercizio</asp:Localize>
                            </div>
                        </div>

                        <div class="col-lg-2 col-md-2 text-center">
                            <div class="btn btn-success" id="btn_nuova_distinta">
                                <i class="fa fa-plus"></i>
                                <asp:Localize meta:resourcekey="NuovoEsercizio" runat="server">Nuovo Esercizio</asp:Localize>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12">
                            <div class="table-responsive">
                                <input type="hidden" id="kendoDistinte" />
                                <div id="tabImpianti">
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="scheda_distinta" style="margin-top: 20px;">
                        <div class="row">
                            <div class="col-md-12">
                                <h5 style="color: #052747;">
                                    <asp:Localize meta:resourcekey="DateDiEsercizio" runat="server">Date di Esercizio</asp:Localize></h5>
                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_ValiditaInizio_Distinta" for="Txt_ValiditaInizio_Distinta"><i class="fa fa-calendar"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Inizio %>" runat="server">Inizio</asp:Localize>
                                                *</span>
                                            <input type="text" id="Txt_ValiditaInizio_Distinta" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_ValiditaFine_Distinta" for="Txt_ValiditaFine_Distinta"><i class="fa fa-calendar"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Fine %>" runat="server">Fine</asp:Localize>
                                                *</span>
                                            <input type="text" id="Txt_ValiditaFine_Distinta" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-10 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Lotto" for="Txt_Lotto">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Lotto %>" runat="server">Lotto</asp:Localize>
                                            </span>
                                            <input type="text" id="Txt_Lotto" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-2 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Codice" for="TxtCodice">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server">Codice</asp:Localize>
                                                </span>
                                            <input type="text" id="TxtCodice" class="form-control " disabled="true" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--<div class="col-lg-6 col-md-6 col-sm-12" style="display:none;">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Sup_Prog" for="Txt_Sup_Prog">Sup. esercizio [Ha]</span>
                                            <input type="text" id="Txt_Sup_Prog" class="form-control kendoDoubleNumber" />
                                        </div>
                                    </div>
                                </div>
                            </div>--%>
                        </div>

                        <div class="row">
                            <% if AlgoritmoCodifica <> "" Then %><div class="col-lg-11 col-md-11 col-sm-11">
                            <% Else %><div class="col-lg-12 col-md-12 col-sm-12">
                            <% End If %>
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Descrizione" for="Txt_Descrizione">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server">Descrizione</asp:Localize>
                                            </span>
                                            <input type="text" id="Txt_Descrizione" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            <%if AlgoritmoCodifica <> "" Then %></div>
                            <% Else %></div>
                            <%End If %>

                            <% if AlgoritmoCodifica <> "" Then %>
                            <div class="col-lg-1 col-md-1 col-sm-1">
                                <div class="btn btn-success Tooltip" id="btnGeneraDescrizione" onclick="GeneraDescrizione();" title="Genera Descrizione">
                                    <i class="fa fa-list-ol"></i>
                                </div>
                            </div>
                            <% End If %>
                        </div>

                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_PianteHa2" for="TxtPianteHa2">
                                                <asp:Localize meta:resourcekey="PianteHa" runat="server">Piante/Ha</asp:Localize></label>
                                            <input type="text" id="TxtPianteHa2" class="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_PianteImpianto2" for="TxtPianteImpianto2">
                                                <asp:Localize meta:resourcekey="NPianteImpianto" runat="server">Piante/Impianto</asp:Localize></label>
                                            <input type="text" id="TxtPianteImpianto2" class="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Anna 16/05/22: aggiunto button per  [rif. chiamata 18136]
                                nascosti campi Piante/HA e Piante/Impianto da DENSITA' IMPIANTO 
                                (nell'esportazione degli impianti viene riportato quello della distinta e alcune aziende non lo compilavano) -->
                            <div class="col-lg-4 col-md-4  col-sm-12 text-left">
                                <div class="btn btn-success" id="btn_calcola_pianteImpianto" onclick="calcola_pianteImpianto();" title="<asp:Localize meta:resourcekey="toolTip_CalcolaPiante" runat="server">Calcola il Numero di Piante in base al sesto d'impianto impostato</asp:Localize>">
                                    <i class="fa fa-calculator"></i>
                                    <asp:Localize meta:resourcekey="Calcola" runat="server">Calcola</asp:Localize>
                                </div>
                            </div>
                        </div>

                         <!-- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -->
                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_PianteHa_Femmina" for="Txt_PianteHa_Femmina">
                                                <asp:Localize meta:resourcekey="PianteHa_Femmina" runat="server">Piante/Ha Femmine</asp:Localize></label>
                                            <input type="text" id="Txt_PianteHa_Femmina" class="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_PianteImpianto_Femmina" for="Txt_PianteImpianto_Femmina">
                                                <asp:Localize meta:resourcekey="NPianteImpianto_Femmina" runat="server">Piante/Impianto femmine</asp:Localize></label>
                                            <input type="text" id="Txt_PianteImpianto_Femmina" class="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_PianteHa_Maschio" for="Txt_PianteHa_Maschio">
                                                <asp:Localize meta:resourcekey="PianteHa_Maschio" runat="server">Piante/Ha Maschi</asp:Localize></label>
                                            <input type="text" id="Txt_PianteHa_Maschio" class="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon alert-info" id="lbl_PianteImpianto_Maschio" for="Txt_PianteImpianto_Maschio">
                                                <asp:Localize meta:resourcekey="NPianteImpianto_Maschio" runat="server">Piante/Impianto Maschi</asp:Localize></label>
                                            <input type="text" id="Txt_PianteImpianto_Maschio" class="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                         <!-- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -->

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h5 style="color: #052747;">
                                            <asp:Localize meta:resourcekey="DatiInPrevisione" runat="server">Dati in Previsione</asp:Localize></h5>
                                    </div>

                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_semina_prevista" for="Txt_semina_prevista"><i class="fa fa-calendar"></i>
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,DataSemina %>" runat="server">Data Semina/Trapianto</asp:Localize>
                                                    </span>
                                                    <input type="text" id="Txt_semina_prevista" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_raccolta_prevista" for="Txt_raccolta_prevista"><i class="fa fa-calendar"></i>
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,DataRaccolta %>" runat="server">Data Raccolta</asp:Localize>
                                                        </span>
                                                        <input type="text" id="Txt_raccolta_prevista" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_fioritura_prevista" for="Txt_fioritura_prevista"><i class="fa fa-calendar"></i>
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,DataFioritura %>" runat="server">Data Fioritura</asp:Localize>
                                                        </span>
                                                        <input type="text" id="Txt_fioritura_prevista" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_ResaPrevista" for="Txt_ResaPrevista">
                                                            <asp:Localize meta:resourcekey="ResaKgPerHa" runat="server">Resa [Kg/Ha]</asp:Localize>
                                                        </span>
                                                        <input type="text" id="Txt_ResaPrevista" class="form-control " />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Regolamento" for="Cmb_Regolamento">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Regolamento %>" runat="server">Regolamento</asp:Localize>
                                            </span>
                                            <input type="text" id="Cmb_Regolamento" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Disciplinare" for="Cmb_Disciplinare">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Disciplinare %>" runat="server">Disciplinare</asp:Localize>
                                            </span>
                                            <input type="text" id="Cmb_Disciplinare" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row" id="rowIAF">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="Lbl_IAF" for="Cmb_IAF">
                                                <asp:Localize meta:resourcekey="ImpegniAggiuntiviFacoltativi" runat="server">Impegni Aggiuntivi Facoltativi (IAF)</asp:Localize></span>
                                            <input type="text" id="Cmb_IAF" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- Campi opzionali 3 in Tab Distinte Produzioni -->
                        <div class="row">
                            <div class="col-lg-12">

                                <div class="panel-group" id="accordion6">
                                    <div class="panel panel-primary">
                                        <div class="panel-heading">
                                            <h4 class="panel-title">
                                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion6" href="#pannello-6"><i class="fa fa-plus-circle"></i>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OpzioniAggiuntive %>" runat="server">Opzioni Aggiuntive</asp:Localize>
                                                </a>
                                                <div class="tag_opt">
                                                    <div>
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AltreInfo %>" runat="server">Altre Info</asp:Localize>
                                                    </div>
                                                </div>
                                                <div style="clear: both;"></div>
                                            </h4>
                                        </div>

                                        <div id="pannello-6" class="panel-collapse ">
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_CapitolatoPrivato" for="Cmb_CapitolatoPrivato">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CapitolatoPrivato %>" runat="server">Capitolato Privato</asp:Localize></span>
                                                                    <input type="text" id="Cmb_CapitolatoPrivato" class="form-control " />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_OrganismoReferente" for="Cmb_OrganismoReferente">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OrganismoReferente %>" runat="server">Organismo Referente</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_OrganismoReferente" class="form-control " />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!--  - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - - -->
                                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_LicenzaColtivazione" for="Cmb_LicenzaColtivazione">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, LicenzaColtivazione %>" runat="server">Licenza Coltivazione</asp:Localize></span>
                                                                    <input type="text" id="Cmb_LicenzaColtivazione" class="form-control " />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <!-- - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - - -->


                                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_Riferimento_Trasferimento_Dati" for="Cmb_Riferimento_Trasferimento_Dati">Riferimento Trasferimento Dati
                                                                    </span>
                                                                    <input type="text" id="Cmb_Riferimento_Trasferimento_Dati" class="form-control " />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_MagazzinoConferimento" for="Cmb_MagazzinoConferimento">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MagazzinoConferimento %>" runat="server">Magazzino Conferimento</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_MagazzinoConferimento" class="form-control " />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_PianoSemina" for="Cmb_PianoSemina">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PianoSemina %>" runat="server">Piano Semina</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_PianoSemina" class="form-control " />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2 col-md-6 col-sm-6">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_distinta_chiusa" for="chk_distinta_chiusa">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, EsercizioChiuso %>" runat="server">Esercizio Chiuso</asp:Localize>
                                                                    </span>
                                                                    <input id="chk_distinta_chiusa" name="chk_distinta_chiusa" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4 col-md-6 col-sm-6">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_secondo_raccolto" for="chk_secondo_raccolto">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SecondoRaccolto %>" runat="server">Secondo Raccolto</asp:Localize>

                                                                    </span>
                                                                    <input id="chk_secondo_raccolto" name="chk_secondo_raccolto" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <% If ReplicaGIAS <> "" Then %>
                                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                                        <div class="form-horizontal" id="div_distinta_replica">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_distinta_replica" for="chk_distinta_replica">
                                                                        <asp:Localize meta:resourcekey="CreaImpiantoEsercizioSuAltraAzienda" runat="server">Crea Impianto/Esercizio su altra azienda</asp:Localize>
                                                                    </span>
                                                                    <input id="chk_distinta_replica" name="chk_distinta_replica" />
                                                                    <label class="input-group-addon" style="width: 100%">
                                                                        <asp:Localize meta:resourcekey="RichiedeSalvataggioEsercizioImpianto" runat="server">(Richiede il salvataggio dell'esercizio e dell'impianto)</asp:Localize></label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <% End If %>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize meta:resourcekey="ApportiMassimiDiMacroelementi" runat="server">Apporti Massimi di Macroelementi</asp:Localize></h4>
                                    </div>

                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_RegolamentoConc" for="Cmb_RegolamentoConc">
                                                        <asp:Localize meta:resourcekey="RegolamentoFertilizzazioni" runat="server">Reg. Fertilizzazioni</asp:Localize></span>
                                                    <input type="text" id="Cmb_RegolamentoConc" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_FinalitaConc" for="Cmb_FinalitaConc">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Tipologia %>" runat="server">Tipologia</asp:Localize>
                                                    </span>
                                                    <input type="text" id="Cmb_FinalitaConc" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Stato" for="Cmb_Stato">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,StatoImpianto %>" runat="server">Stato Impianto</asp:Localize>
                                                    </span>
                                                    <input type="text" id="Cmb_Stato" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-3 col-md-3 col-sm-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_N" for="TxtN">N [kg/ha]</span>
                                                    <input type="text" id="TxtN" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-3 col-md-3 col-sm-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_P2O5" for="TxtP2O5">P2O5 [kg/ha]</span>
                                                    <input type="text" id="TxtP2O5" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-3 col-md-3 col-sm-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_K2O" for="TxtK2O">K2O [kg/ha]</span>
                                                    <input type="text" id="TxtK2O" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-3 col-md-3 col-sm-6">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_MgO" for="TxtMgO">MgO [kg/ha]</span>
                                                    <input type="text" id="TxtMgO" class="form-control " />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <hr/>

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CodiciAnagrafici %>" runat="server">Codici Anagrafici</asp:Localize>
                                        </h4>
                                    </div>
                                </div>

                                <div class="row">
                                    <input type="hidden" id="kendoCodiciDistinte" />
                                    <div class="col-lg-12">
                                        <div id="tabCodici">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Particelle %>" runat="server">Particelle</asp:Localize>
                                        </h4>
                                    </div>
                                </div>

                                <div class="row">
                                    <input type="hidden" id="kendoParticelleDistinte" />
                                    <div class="col-lg-12">
                                        <div id="tabParticelle">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- TAB 2 -->
            <div class="" id="tab_dati_accessori">
                <div class="jumbotron">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="lbl_Codice_Impianto" for="Txt_CodiceImpianto">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CodiceImpianto %>" runat="server">Codice Impianto</asp:Localize>
                                        </span>
                                        <input type="text" id="Txt_CodiceImpianto" class="form-control " />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                        <!-- Campi opzionali 1 in Tab Dati Accessori -->
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="panel-group" id="accordion1">
                                    <div class="panel panel-primary">
                                        <div class="panel-heading">
                                            <h4 class="panel-title">
                                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion1" href="#pannello-1"><i class="fa fa-plus-circle"></i>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,OpzioniAggiuntive %>" runat="server">Opzioni Aggiuntive</asp:Localize></a>
                                                <div class="tag_opt">
                                                    <div>
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Impianto %>" runat="server">Impianto</asp:Localize>
                                                    </div>
                                                    <div>
                                                        <asp:Localize meta:resourcekey="DensitàImpianto" runat="server">Densità impianto</asp:Localize>
                                                    </div>
                                                </div>
                                                <div style="clear: both;"></div>
                                            </h4>
                                        </div>

                                        <div id="pannello-1" class="panel-collapse ">
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <input type="checkbox" id="ChkConsociazione" class="k-checkbox" />
                                                        <label aria-describedby="ChkConsociazione" id="lbl_Consociazione" class="k-checkbox-label" for="ChkConsociazione">
                                                            <asp:Localize meta:resourcekey="ImpiantoConsociato" runat="server">Impianto Consociato</asp:Localize>
                                                        </label>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <input type="checkbox" id="ChkCoverCrops" class="k-checkbox" />
                                                        <label aria-describedby="ChkCoverCrops" id="lbl_CoverCrops" class="k-checkbox-label" for="ChkCoverCrops">
                                                            <asp:Localize meta:resourcekey="ImpiantoCoverCropsDiCopertura" runat="server">Impianto Cover Crops (di copertura)</asp:Localize>
                                                        </label>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <input type="checkbox" id="ChkMonitorato" class="k-checkbox" />
                                                        <label aria-describedby="ChkMonitorato" id="lbl_Monitorato" class="k-checkbox-label" for="ChkMonitorato">
                                                            <asp:Localize meta:resourcekey="ImpiantoMonitorato" runat="server">Impianto Monitorato</asp:Localize>
                                                        </label>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_ImpIrrigazione" for="Cmb_ImpIrrigazione">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImpiantoIrrigazione %>" runat="server">Impianto Irrigazione</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_ImpIrrigazione" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_FormaAllevamento" for="Cmb_FormaAllevamento">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,FormaAllevamento %>" runat="server">Forma Allevamento</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_FormaAllevamento" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_Portinnesto" for="Cmb_Portinnesto">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Portinnesto %>" runat="server">Portinnesto</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_Portinnesto" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_Data_Inizio_Portinnesto" for="Txt_Data_Inizio_Portinnesto">
                                                                        <i class="fa fa-calendar"></i>
                                                                        <!--<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Dal %>" runat="server">Messa a dimora Portinnesto</asp:Localize>-->
                                                                        Messa a dimora Portinnesto
                                                                    </span>
                                                                    <input type="text" id="Txt_Data_Inizio_Portinnesto" class="form-control kendoDate required" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_SeminaTrapianto" for="Cmb_SeminaTrapianto">
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,SeminaTrapianto %>" runat="server">Semina / Trapianto</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_SeminaTrapianto" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_ProvenienzaSeme" for="Cmb_ProvenienzaSeme">
                                                                        <asp:Localize meta:resourcekey="ProvenienzaSeme" runat="server">Provenienza Seme</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_ProvenienzaSeme" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_ConduzioneSu" for="Cmb_ConduzioneSu">
                                                                        <asp:Localize meta:resourcekey="ConduzioneSuFila" runat="server">Conduzione Su Fila</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_ConduzioneSu" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_ConduzioneTra" for="Cmb_ConduzioneTra">
                                                                        <asp:Localize meta:resourcekey="ConduzioneTraFila" runat="server">Conduzione Tra Fila</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_ConduzioneTra" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_DettaglioVarietaPersonalizzato" for="Cmb_DettaglioVarietaPersonalizzato">
                                                                        <asp:Localize meta:resourcekey="DettaglioSpeciePersonalizzato" runat="server">Dett. Specie Personalizzato</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_DettaglioVarietaPersonalizzato" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                     <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_CodiceZona" for="Cmb_CodiceZona">
                                                                        <asp:Localize meta:resourcekey="CodiceZona" runat="server">Codice Zona</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="Cmb_CodiceZona" class="kendoDropDownList" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <div class="row">
                                                            <div class="col-md-12 text-center">
                                                                <h4 style="color: #052747; text-transform: uppercase;">
                                                                    <asp:Localize meta:resourcekey="DensitàImpianto" runat="server">densità impianto</asp:Localize></h4>
                                                            </div>

                                                            <div class="col-md-12">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                        <h5 style="color: #052747;">
                                                                            <asp:Localize meta:resourcekey="SestoDiImpianto" runat="server">Sesto di Impianto</asp:Localize></h5>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_Superficie2" for="TxtSuperficie2">
                                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,SuperficieNettaColtivataAbbr %>" runat="server">Sup. netta coltivata</asp:Localize>
                                                                                        [Ha] *
                                                                                    </span>
                                                                                    <input type="text" id="TxtSuperficie2" class="form-control kendoDoubleNumber" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_DistanzaSuFila_M" for="Txt_DistanzaSuFila_M">
                                                                                        <asp:Localize meta:resourcekey="DistanzaSuFila" runat="server">Distanza su fila</asp:Localize>
                                                                                        [m] *
                                                                                    </span>
                                                                                    <input type="text" id="Txt_DistanzaSuFila_M" class="form-control kendoDoubleNumber" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_DistanzaTraFila_M" for="Txt_DistanzaTraFila_M">
                                                                                        <asp:Localize meta:resourcekey="DistanzaTraFila" runat="server">Distanza tra fila</asp:Localize>
                                                                                        [m] *
                                                                                    </span>
                                                                                    <input type="text" id="Txt_DistanzaTraFila_M" class="form-control kendoDoubleNumber" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-12">
                                                                        <input type="checkbox" id="ChkFilaBinata" class="k-checkbox" />
                                                                        <label aria-describedby="ChkFilaBinata" id="lbl_FilaBinata" for="ChkFilaBinata" class="k-checkbox-label">
                                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FilaBinata %>" runat="server">Fila Binata</asp:Localize>
                                                                        </label>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_Interbina" for="Txt_Interbina">
                                                                                        <asp:Localize meta:resourcekey="Interbina" runat="server">Interbina</asp:Localize>
                                                                                        [m] *
                                                                                    </span>
                                                                                    <input type="text" id="Txt_Interbina" class="form-control kendoDoubleNumber" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_Germinabilita" for="Txt_Germinabilita">
                                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Germinabilita %>" runat="server"></asp:Localize> *
                                                                                    </span>
                                                                                    <input type="text" id="Txt_Germinabilita" class="form-control kendoDoubleNumber" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <!-- - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -  -->
                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                             <div class="form-group">                                                                                 
                                                                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                                                    <input type="checkbox" id="ChkMaschiSesto" class="k-checkbox" />
                                                                                    <label aria-describedby="ChkMaschiSesto" id="lbl_MaschiSesto" class="k-checkbox-label" for="ChkMaschiSesto">
                                                                                        <asp:Localize meta:resourcekey="MaschiSesto" runat="server">Maschi in Sesto</asp:Localize>
                                                                                    </label>
                                                                                </div>

                                                                                <%-- <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_MaschiSesto" for="ChkMaschiSesto"><label> 
                                                                                        <asp:Localize meta:resourcekey="MaschiSesto" runat="server">Maschi in Sesto</asp:Localize></label>                               
                                                                                    </span>
                                                                                    <input id="ChkMaschiSesto" name="ChkMaschiSesto" class="k-checkbox" />
                                                                                 </div>--%>

                                                                            </div>
                                                                       </div>
                                                                   </div>
                                                                   <!-- - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - - -->

                                                                </div>
                                                            </div>

                                                            <div class="col-md-12" style="display: none;">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                        <h5 style="color: #052747;">
                                                                            <asp:Localize meta:resourcekey="CalcoloPiante" runat="server">Calcolo Piante</asp:Localize></h5>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_PianteHa" for="TxtPianteHa">
                                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,PianteHa %>" runat="server">Piante/Ha</asp:Localize>
                                                                                    </span>
                                                                                    <input type="text" id="TxtPianteHa" class="form-control kendoDoubleNumber" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_PianteImpianto" for="TxtPianteImpianto">
                                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,NPianteImpianto %>" runat="server">Piante/Impianto</asp:Localize>
                                                                                    </span>
                                                                                    <input type="text" id="TxtPianteImpianto" class="form-control kendoDoubleNumber" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Copertura" for="Cmb_Copertura">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Copertura %>" runat="server">Copertura</asp:Localize>
                                            </span>
                                            <input type="text" id="Cmb_Copertura" class="kendoDropDownList" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <!-- Campi opzionali 2 in Tab Dati Accessori -->
                            <div class="col-lg-12">
                                <div class="panel-group" id="accordion2">
                                    <div class="panel panel-primary">
                                        <div class="panel-heading">
                                            <h4 class="panel-title">
                                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#pannello-2">
                                                    <i class="fa fa-plus-circle"></i>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,OpzioniAggiuntive %>" runat="server">Opzioni Aggiuntive</asp:Localize>
                                                </a>
                                                <div class="tag_opt">
                                                    <div>
                                                        <asp:Localize meta:resourcekey="PeriodoCopertura" runat="server">Periodo copertura</asp:Localize>
                                                    </div>
                                                </div>
                                                <div style="clear: both;"></div>
                                            </h4>
                                        </div>

                                        <div id="pannello-2" class="panel-collapse ">
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_CopDataInizio" for="TxtCopDataInizio">
                                                                        <i class="fa fa-calendar"></i>
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Dal %>" runat="server">Dal</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="TxtCopDataInizio" class="form-control kendoDate required" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_CopDataFine" for="TxtCopDataFine">
                                                                        <i class="fa fa-calendar"></i>
                                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Al %>" runat="server">Al</asp:Localize>
                                                                    </span>
                                                                    <input type="text" id="TxtCopDataFine" class="form-control kendoDate required" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Resa1" for="Txt_Resa1">
                                                <asp:Localize meta:resourcekey="ResaPrevistaKgTotali" runat="server">Resa Storica Prevista [Kg tot.]</asp:Localize>
                                            </span>
                                            <input type="text" id="Txt_Resa1" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Resa2" for="Txt_Resa2">
                                                <asp:Localize meta:resourcekey="ResaCorrettaKgTotali" runat="server">Resa Storica Corretta [Kg tot.]</asp:Localize>
                                            </span>
                                            <input type="text" id="Txt_Resa2" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_UnitaVitata" for="TXT_UnitaVitata">
                                                <asp:Localize meta:resourcekey="UnitàVitata" runat="server">Unità Vitata</asp:Localize>
                                            </span>
                                            <input type="text" id="TXT_UnitaVitata" class="form-control " />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <!-- Campi opzionali 3 in Tab Dati Accessori -->
                            <div class="col-lg-12">
                                <div class="panel-group" id="accordion3">
                                    <div class="panel panel-primary">
                                        <div class="panel-heading">
                                            <h4 class="panel-title">
                                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion3" href="#pannello-3">
                                                    <i class="fa fa-plus-circle"></i>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,OpzioniAggiuntive %>" runat="server">Opzioni Aggiuntive</asp:Localize>
                                                </a>
                                                <div class="tag_opt">
                                                    <div>
                                                        <asp:Localize meta:resourcekey="VarietàELinee" runat="server">Varietà e linee</asp:Localize>
                                                    </div>
                                                </div>
                                                <div style="clear: both;"></div>
                                            </h4>
                                        </div>

                                        <div id="pannello-3" class="panel-collapse ">
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <input type="checkbox" id="ChkVarietaIbrida" class="k-checkbox" />
                                                        <label aria-describedby="ChkVarietaIbrida" id="lbl_VarietaIbrida" class="k-checkbox-label" for="ChkVarietaIbrida">
                                                            <asp:Localize meta:resourcekey="VarietàIbrida" runat="server">Varietà Ibrida</asp:Localize>
                                                        </label>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-md-12 text-center">
                                                        <h4 style="color: #052747; text-transform: uppercase;">
                                                            <asp:Localize meta:resourcekey="LineaMaschio" runat="server">Linea maschio</asp:Localize></h4>
                                                    </div>

                                                    <div class="col-md-12">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <h5 style="color: #052747;">
                                                                    <asp:Localize meta:resourcekey="Genetica" runat="server">Genetica</asp:Localize></h5>
                                                            </div>

                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon alert-info" id="lbl_CodBMBDBT_M" for="Txt_CodBMBDBT_M">Cod. BM-BD-BT
                                                                            </span>
                                                                            <!-- i18n -->
                                                                            <input type="text" id="Txt_CodBMBDBT_M" class="form-control " />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon alert-info" id="lbl_Genetica_M" for="Txt_Genetica_M">
                                                                                <asp:Localize meta:resourcekey="Genetica" runat="server">Genetica</asp:Localize>
                                                                            </span>
                                                                            <input type="text" id="Txt_Genetica_M" class="form-control " />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon alert-info" id="lbl_OffType_M" for="Txt_OffType_M">Off-Type
                                                                            </span>
                                                                            <!-- i18n -->
                                                                            <input type="text" id="Txt_OffType_M" class="form-control " />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12" id="linea-femmina">
                                                        <hr />
                                                        <div class="row">
                                                            <div class="col-md-12 text-center">
                                                                <h4 style="color: #052747; text-transform: uppercase;">
                                                                    <asp:Localize meta:resourcekey="LineaFemmina" runat="server">Linea femmina</asp:Localize></h4>
                                                            </div>

                                                            <div class="col-md-12">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                        <h5 style="color: #052747;">
                                                                            <asp:Localize meta:resourcekey="Genetica" runat="server">Genetica</asp:Localize></h5>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_CodBMBDBT_F" for="Txt_CodBMBDBT_F">Cod. BM-BD-BT
                                                                                    </span>
                                                                                    <!-- i18n -->
                                                                                    <input type="text" id="Txt_CodBMBDBT_F" class="form-control " />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_Genetica_F" for="Txt_Genetica_F">
                                                                                        <asp:Localize meta:resourcekey="Genetica" runat="server">Genetica</asp:Localize>
                                                                                    </span>
                                                                                    <input type="text" id="Txt_Genetica_F" class="form-control " />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_OffType_F" for="Txt_OffType_F">Off-Type
                                                                                    </span>
                                                                                    <input type="text" id="Txt_OffType_F" class="form-control " />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="col-md-12">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                        <h5 style="color: #052747;">
                                                                            <asp:Localize meta:resourcekey="SestoDiImpianto" runat="server">Sesto di Impianto</asp:Localize></h5>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_DistanzaSuFila_F" for="Txt_DistanzaSuFila_F">
                                                                                        <asp:Localize meta:resourcekey="DistanzaSuFila" runat="server">Distanza su fila</asp:Localize>
                                                                                        [m] 
                                                                                    </span>
                                                                                    <input type="text" id="Txt_DistanzaSuFila_F" class="form-control " />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_DistanzaTraFila_F" for="Txt_DistanzaTraFila_F">
                                                                                        <asp:Localize meta:resourcekey="DistanzaTraFila" runat="server">Distanza tra fila</asp:Localize>
                                                                                        [m]
                                                                                    </span>
                                                                                    <input type="text" id="Txt_DistanzaTraFila_F" class="form-control " />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row" id="pannelloTuberi">
                            <div class="col-md-12 text-center">
                                <h4 style="color: #052747; text-transform: uppercase;">
                                    <asp:Localize meta:resourcekey="PannelloTuberi" runat="server">Pannello Tuberi</asp:Localize></h4>
                            </div>

                            <div class="col-lg-12">
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_PartiTuberi" for="Txt_Resa2">
                                                    <asp:Localize meta:resourcekey="Parti" runat="server">Parti</asp:Localize>
                                                </span>
                                                <input type="text" id="Txt_PartiTuberi" class="form-control " />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Lbl_TagliatoTuberi" for="TXT_UnitaVitata">
                                                    <asp:Localize meta:resourcekey="Tubero" runat="server">Tubero</asp:Localize>
                                                </span>
                                                <input type="text" id="Cmb_TagliatoTuberi" class="form-control " />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
            </div>
        </div>
    </div>

    <!-- Dialog Descrizione OP -->
    <div class="modal fade" id="modalDescrizioneOP" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <h4 class="modal-title" id="lbl_new_item">
                        <asp:Localize meta:resourcekey="SelezionaDescrizione" runat="server">Seleziona Descrizione</asp:Localize>
                    </h4>
                </div>

                <div class="modal-body">
                    <div class="form-group" style="padding: 15px 0;">
                        <label for="recipient-name" class="control-label">
                            <asp:Localize meta:resourcekey="SelezionaProdottoComeDescrizione" runat="server">Seleziona un prodotto da usare come descrizione</asp:Localize>
                        </label>
                        <select id="descrizioneProdotto" style="width: 100%;">
                        </select>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-success" id="btn_descrizione" onclick="CambiaDescrizione();">
                        <i class="fa fa-search"></i>
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Seleziona %>" runat="server">Seleziona</asp:Localize>
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- VARIE -->
    <!-- contiene l'indice del tab selezionato, viene controllato lato server CalledFromClient_SetIndexTab -->
    <asp:ImageButton ID="ImgBtn_Cancella_Distinta" runat="server" Style="display: none" />
    <input type="hidden" id="clickedTabUI" runat="server" />
    <!-- salva la chiamata alla funzione per il postback -->
    <input type="hidden" id="fooName_PostedBack" runat="server" />
    <!-- parametri le funzione di postback -->
    <input type="hidden" id="fooName_Param_PostedBack" runat="server" />
    <br />
    <input id="InizioCentro" name="InizioCentro" type="hidden" runat="server" />
    <input id="FineCentro" name="FineCentro" type="hidden" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Impianto_Edit2_kendo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Impianto_Edit2.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Impianto_Edit2_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Impianto_Edit2_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>

    <script type="text/javascript">
        var permessi = <%= permessiStr %>;
        var algoritmoCodifica = "<%= AlgoritmoCodifica %>";
        var replicaGIAS = "<%= ReplicaGIAS %>";
        var obj_Impianto = <%= objImpianto.ToString  %>;
        var obj_Distinta_Selezionata;
        var obj_Permessi_IAF = <%= objPermessi_IAF.ToString %>;
        var obj_Codici = <%= cmb_Codici.ToString %>;
        var obj_CodiciParticelle = <%= cmb_CodiciParticelle.ToString %>;
        var obj_Particelle = <%= cmb_Particelle.ToString %>;
        var operazione_DB = <%= operazione.ToString  %>;
        var objPermessi_IAF = <%= objPermessi_IAF.ToString %>;
        var defaultFinalita = "<%= DefaultFinalita %>";
        var defaultRegolamento = "<%= DefaultRegolamento %>";
    </script>

    <script id="popupCodici_Template" type="text/x-kendo-template">
        <div class="k-edit-label">
            <label for="Codice" class="campiObbligatori"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Codice %>" runat="server">Codice</asp:Localize></label>
        </div>
        <div data-container-for="Codice" class="k-edit-field">
            <div required="required" data-bind="value:id_cod" id="Cmb_Codici"></div>
        </div>

        <div class="k-edit-label">
            <label for="valore"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Valore %>" runat="server">Valore</asp:Localize></label>
        </div>
        <div data-container-for="valore" class="k-edit-field">
            <input required="required" type="text" data-type="text" class="k-input k-textbox" name="valore" data-bind="value:val_cod" />
        </div>
    </script>

    <script id="popupParticelle_Template" type="text/x-kendo-template">
        <div class="k-edit-label">
            <label for="Catasto" class="campiObbligatori"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Catasto %>" runat="server">Catasto</asp:Localize></label>
        </div>
        <div data-container-for="Catasto" class="k-edit-field">
            <div required="required" data-bind="value:valoreProv" id="Cmb_Particelle"></div>
        </div>

        <div class="k-edit-label">
            <label for="Codice" class="campiObbligatori"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Codice %>" runat="server">Codice</asp:Localize></label>
        </div>
        <div data-container-for="Codice" class="k-edit-field">
            <div required="required" data-bind="value:id_cod" id="Cmb_CodiciParticelle"></div>
        </div>

        <div class="k-edit-label">
            <label for="valore"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Valore %>" runat="server">Valore</asp:Localize></label>
        </div>
        <div data-container-for="valore" class="k-edit-field">
            <input required="required" type="text" data-type="text" class="k-input k-textbox" name="valore" data-bind="value:val_cod" />
        </div>
    </script>

</asp:Content>
