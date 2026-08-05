<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="Scad_Lista.aspx.vb" Inherits="AgroAgenda_2010.Scad_Lista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        /*Mi serve per dare grafica uniforme ai controlli in sola lettura*/
        .disabled.dropdown-toggle {
            background-color: lightgray;
            border: 1px solid #428BCA;
            opacity: 1;
        }

        .k-upload-selected {
            display: none !important;
        }


        .file-icon {
            display: inline-block;
            float: left;
            padding: 10px;
            margin-top: 10px;
            font-size: 2.5em;
        }

        .file-heading {
            font-family: Arial, sans-serif;
            font-size: 1.1em;
            display: inline-block;
            float: left;
            width: 60%;
            margin: 6px 0 0 10px;
            /*height: 25px;*/
            -ms-text-overflow: ellipsis;
            -o-text-overflow: ellipsis;
            text-overflow: ellipsis;
            overflow: hidden;
            white-space: nowrap;
        }

        .file-name-heading {
            font-weight: bold;
            margin-top: 20px;
        }

        .file-size-heading {
            font-weight: normal;
            font-style: italic;
        }

        li.k-file div.file-wrapper {
            position: relative;
            height: 50px;
            width: 100%;
        }

        .fixed-header {
            top: 0;
            position: fixed;
            width: auto;
            z-index: 1;
        }

        
    /* Inizio stili per test menu filtri laterale */
    .xo-scadenziario-filtri-container {
        transition: transform 0.3s ease, opacity 0.3s ease;
        width: 55vw;
        height: calc(100vh - 50px);
        position: fixed;
        top: 50px;
        right: 0;
        background-color: white;
        z-index: 10000;
    }
     .xo-scadenziario-filtri-container.hidden-sidebar {
        opacity: 0;
        transform: translateX(100%);
     }

     .xo-ricerca-doc-contabili-filtri-action-container {
         margin-right: 20px;
         margin-left: 20px;
     }

     #xoRicercaDocToggleFiltri {
        transition: transform 0.3s ease, opacity 0.3s ease;
        top: 80px;
        position: fixed;
        z-index: 10000;
        right: 55vw;
     }
     #xoRicercaDocToggleFiltri.hidden-sidebar {
        top: 80px;
        position: fixed;
        z-index: 10000;
        right: 0;
     }
     #xoRicercaDocToggleFiltri span svg {
         width: 16px;
     }

     .btn.xonne-btn-filter {
        height: 40px !important;
        border-radius: 0 !important;
        border-top-left-radius: 16px !important;
        border-bottom-left-radius: 16px !important;
        border-color: white !important;
     }

     #xoRicercaDocumentiTabHeaderFiltri {
         display: none;
     }

     .xoRicercaDocumentiTabPanelFiltri {
        list-style: none;
        padding-left: 0;
     }
     .xo-scadenziario-filtri-container {
         overflow-y: scroll;
     }
     @media only screen and (max-width: 991px) {
        .xo-ricerca-doc-filtri-container {
            top: 100px;
            height: calc(100vh - 100px);
        }
     }
    </style>
    <script>
        function toggleMenu() {
            debugger;
            const sidebar = document.getElementById('id_filtriricerca');
            sidebar.classList.toggle('hidden-sidebar');
            const sidebarAction = document.getElementById('xoRicercaDocToggleFiltri');
            sidebarAction.classList.toggle('hidden-sidebar');
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!--VARIABILE PER I PERMESSI DI SCRITTURA-->
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <asp:HiddenField runat="server" ID="hf_AbilitazioneDocumentiAPP" />

    <asp:HiddenField ID="hf_UploadMultiploAllegatiAbilitato" runat="server" />

    <% If Master.Master_versione = "2022" Then %>
    <div id="xoRicercaDocToggleFiltri" class="btn xonne-btn-primary xonne-btn-filter hidden-sidebar" onclick="toggleMenu()">
        <span>
            <svg role="img" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="sliders" class="svg-inline--fa fa-sliders fa-lg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path fill="currentColor" d="M0 416c0-17.7 14.3-32 32-32l54.7 0c12.3-28.3 40.5-48 73.3-48s61 19.7 73.3 48L480 384c17.7 0 32 14.3 32 32s-14.3 32-32 32l-246.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 448c-17.7 0-32-14.3-32-32zm192 0c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zM384 256c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zm-32-80c32.8 0 61 19.7 73.3 48l54.7 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-54.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 288c-17.7 0-32-14.3-32-32s14.3-32 32-32l246.7 0c12.3-28.3 40.5-48 73.3-48zM192 64c-17.7 0-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32s-14.3-32-32-32zm73.3 0L480 64c17.7 0 32 14.3 32 32s-14.3 32-32 32l-214.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 128C14.3 128 0 113.7 0 96S14.3 64 32 64l86.7 0C131 35.7 159.2 16 192 16s61 19.7 73.3 48z"></path></svg>
        </span>
    </div>
            

    <!-- FILTRI Matteo qui iniziano gli elementi che implementano i filtri -->
    <div id="id_filtriricerca" class="col-lg-12 col-md-12 col-sm-12 xo-scadenziario-filtri-container hidden-sidebar">
    <% Else %>
    <div class="col-lg-12 col-md-12 col-sm-12" id="id_filtriricerca">
    <% End If %>
    

        <div id="pnlFiltriRicerca" class="panel-group" style="margin-top: 20px; opacity: 0;">
            <ul id="panelFiltriRicerca" <% If Master.Master_versione = "2022" Then %> class="xoRicercaDocumentiTabPanelFiltri" <% End If %>>
                <li class="k-state" id="panelBar_FiltriRicerca">
                    <span <% If Master.Master_versione = "2022" Then %> id="xoRicercaDocumentiTabHeaderFiltri" <% End If %> class="k-link k-state-selected k-selected">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltriRicerca %>" runat="server">FILTRI RICERCA</asp:Localize>
                    </span>

                    <!-- AZIENDA -->
                    <div class="row">

                        <div class="col-lg-8 col-md-6 col-sm-6" id="id_azienda" style="margin-top: 20px;">
                            <div class="input-group">
                                <label class="input-group-addon" id="CTRL_Azienda" for="ddlAzienda">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Azienda %>" runat="server">Azienda</asp:Localize>
                                </label>
                                <input type="text" id="ddlAzienda" name="ddlAzienda" class="form-control" />
                            </div>
                        </div>

                        <div class="col-lg-2 col-md-3 col-sm-3" style="margin-top: 20px;">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblAziendaCorrente" for="chkAziendaCorrente">
                                    <asp:Localize meta:resourcekey="AziendaCorrente" runat="server">Azienda Corrente</asp:Localize>
                                </label>
                                <input type="checkbox" name="chkAziendaCorrente" id="chkAziendaCorrente" class="kendoSwitch" />
                            </div>
                        </div>
                        
                        <div class="col-lg-2 col-md-3 col-sm-3" style="margin-top: 20px;">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblSoloAttive" for="ChkSoloAttive">
                                    <asp:Localize meta:resourcekey="SoloAttive" runat="server">Solo Aziende Attive</asp:Localize>
                                </label>
                                <input type="checkbox" name="ChkSoloAttive" id="ChkSoloAttive" class="kendoSwitch" />
                            </div>
                        </div>
                    </div>

                    <!-- CATEGORIA E TIPOLOGIA -->
                    <div class="row">

                        <div class="col-lg-5 col-md-5 col-sm-12" id="id_area">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" for="cmbArea">
                                            <asp:Localize meta:resourcekey="CategoriaDocumento" runat="server">Categoria</asp:Localize>
                                        </label>
                                        <input name="cmbArea" id="cmbArea" class="form-control">
                                    </div>
                                </div>
                            </div>
                        </div>


                        <div class="col-lg-6 col-md-5 col-sm-12" id="id_tipologia">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" for="cmbTipologia">
                                            <asp:Localize meta:resourcekey="TipologiaDocumento" runat="server">Tipologia</asp:Localize>
                                        </label>
                                        <select name="cmbTipologia" multiple="multiple" id="cmbTipologia" class="form-control"></select>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                    <!-- VALIDAZIONE, STORICO E SELEZIONE MULTIPLA -->
                    <div class="row">

                        <!-- Anna 29/04/22: aggiunti campi al filtro di ricerca -->
                        <div class="col-lg-3 col-md-6 col-sm-6" id="id_validazione">
                            <div class="input-group">
                                <label class="input-group-addon" for="cmbValidazione">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StatoValidazione %>" runat="server">Stato Validazione</asp:Localize>
                                </label>
                                <input name="cmbValidazione" id="cmbValidazione" class="form-control"></input>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-6 col-sm-6" id="id_ChkValidazione">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblValidazione" for="ChkValidazione">
                                    <asp:Localize meta:resourcekey="ValidazioneAttiva" runat="server">Validazione Attiva</asp:Localize>
                                </label>
                                <input type="checkbox" name="ChkValidazione" id="ChkValidazione" class="kendoSwitch" />
                            </div>
                        </div>

                        <!-- //Anna 02/05/22: modificato campo Storico in DDL -->
                        <div class="col-lg-3 col-md-7 col-sm-7">
                            <div class="input-group">
                                <label class="input-group-addon" for="ddlStorico">
                                    <asp:Localize Text="<%$ Resources: VisualizzaStorico %>" runat="server">Visualizza Storico</asp:Localize>
                                </label>
                                <input name="ddlStorico" id="ddlStorico" class="form-control"></input>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-5 col-sm-5">
                            <div class="input-group" id="GestioneStorico">
                                <label class="input-group-addon" id="lblGestioneStorico" for="ChkGestioneStorico">
                                    <asp:Localize Text="<%$ Resources: AbilitaSelezioneMultipla %>" runat="server">Abilita Selezione Multipla</asp:Localize>
                                </label>
                                <input type="checkbox" name="ChkGestioneStorico" id="ChkGestioneStorico" class="kendoSwitch" />
                            </div>
                        </div>

                    </div>

                    <!-- UPLOAD (Anna 29/04/22: aggiunti campi al filtro di ricerca)-->
                    <div class="row">

                        <div class="col-lg-4 col-md-4 col-sm-5">
                            <div class="form-horizontal">
                                <div class="input-group">
                                    <label class="input-group-addon" id="Lbl_inizio_upload" for="txt_Inizio_upload">
                                        <asp:Localize meta:resourcekey="UploadDAL" runat="server">Upload DAL</asp:Localize>
                                    </label>
                                    <input id="txt_Inizio_upload" name="txt_Inizio_upload" class="kendoCalendar" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-5">
                            <div class="form-horizontal">
                                <div class="input-group">
                                    <label class="input-group-addon" id="Lbl_fine_upload" for="txt_Fine_upload">
                                        <asp:Localize meta:resourcekey="UploadAL" runat="server">Upload AL</asp:Localize>
                                    </label>
                                    <input id="txt_Fine_upload" name="txt_Fine_upload" class="kendoCalendar" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-3 col-sm-5">
                            <div class="input-group">
                                <label class="input-group-addon" for="chkUtenteUpload">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SoloIMieiAllegati %>" runat="server">Solo i miei allegati</asp:Localize>
                                </label>
                                <input type="checkbox" name="chkUtenteUpload" id="chkUtenteUpload" class="kendoSwitch" />
                            </div>
                        </div>

                    </div>

                    <!-- SCADENZE -->
                    <div id="pnlScadenze" class="row" style="display: none;">

                        <!--PULSANTE PER L'AGGIORNAMENTO DELLE SCADENZE-->
                        <!--<div class="btn btn-default" id="BtnScad_UpdLista" onclick="eseguiRicercaScadenze();">
                            <span class="fa fa-refresh"> Aggiorna Lista Scadenze</span>
                        </div>-->

                        <div class="row">

                            <div class="col-lg-3 col-md-4 col-sm-6">
                                <div class="form-horizontal">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="Lbl_Validita_Inizio" for="txt_Validita_Inizio">
                                            <asp:Localize meta:resourcekey="InizioScadenza" runat="server">Inizio Scadenza</asp:Localize>
                                        </label>
                                        <input id="txt_Validita_Inizio" name="txt_Validita_Inizio" class="kendoCalendar" maxlength="10" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-4 col-sm-6">
                                <div class="form-horizontal">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="Lbl_Validita_Fine" for="txt_Validita_Fine">
                                            <asp:Localize meta:resourcekey="FineScadenza" runat="server">Fine Scadenza</asp:Localize>
                                        </label>
                                        <input id="txt_Validita_Fine" name="txt_Validita_Fine" class="kendoCalendar" maxlength="10" />
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                    <!-- BUTTONS -->
                    <div class="row" style="margin-top: 10px;">
                        <!-- Matteo duplicati i bottoni per stilizzare i i filtri nella barra laterale -->
                        <% If Master.Master_versione = "2022" Then %>
                        <div class="col-lg-4 col-md-4 col-sm-12" id="id_ricerca">
                            <div id="pnlFiltriRicerca2" class="panel-group">
                                <div class="btn btn-success btn-block xonne-btn-primary" id="btn_Ricerca" title="<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize>">
                                    <span class="fa fa-search  lampeggiante"></span>
                                    <span class="lampeggiante">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize>
                                    </span>
                                </div>
                            </div>
                        </div>

                        <!-- TOOL IMPORT -->
                        <div class="col-lg-4 col-md-4 col-sm-12" id="id_importascadenze">
                            <%--<div id="pnlImportaScadenze" class="row" style="margin-top:10px;">--%>
                            <!--PULSANTE PER L'IMPORTAZIONE DI NUOVE SCADENZE DA GIAS-->
                            <div class="btn btn-default btn-block xonne-btn-primary" id="BtnScad_ImportaScad" style="display: none" onclick="importaScadenzeDialog();" title="<asp:Localize meta:resourcekey="ImportaScadenze" runat="server">Importa Scadenze</asp:Localize>">
                                <span class="fa fa-refresh"></span>
                                <asp:Localize meta:resourcekey="ImportaScadenze" runat="server">Importa Scadenze</asp:Localize>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-12" id="id_caricadatiapp">
                            <!--PULSANTE PER L'IMPORTAZIONE DATI APP-->
                            <div class="btn btn-default btn-block btn-app-import xonne-btn-primary" id="BtnImportaApp" style="display: none" onclick="<%= If(SincroDatiApp, "CaricaDocumentiApp", "CaricaDatiApp") %>();" title="<asp:Localize meta:resourcekey="CaricaDatiApp" runat="server">Carica Dati App</asp:Localize>">
                                <span class="fa fa-refresh"></span>
                                <asp:Localize meta:resourcekey="CaricaDatiApp" runat="server">Carica Dati App</asp:Localize>
                            </div>
                        </div>
                        <% Else %>
                        <div class="col-lg-offset-6 col-lg-2 col-md-2 col-sm-2" id="id_ricerca">
                            <div id="pnlFiltriRicerca2" class="panel-group">
                                <div class="btn btn-success btn-block xonne-btn-primary" id="btn_Ricerca" title="<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize>">
                                    <span class="fa fa-search  lampeggiante xo-agronica-style"></span>
                                    <span class="lampeggiante">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize>
                                    </span>
                                </div>
                            </div>
                        </div>

                        <!-- TOOL IMPORT -->
                        <div class="col-lg-2 col-md-3 col-sm-3" id="id_importascadenze">
                            <%--<div id="pnlImportaScadenze" class="row" style="margin-top:10px;">--%>
                            <!--PULSANTE PER L'IMPORTAZIONE DI NUOVE SCADENZE DA GIAS-->
                            <div class="btn btn-default btn-block xonne-btn-primary" id="BtnScad_ImportaScad" style="display: none" onclick="importaScadenzeDialog();" title="<asp:Localize meta:resourcekey="ImportaScadenze" runat="server">Importa Scadenze</asp:Localize>">
                                <span class="fa fa-refresh"></span>
                                <asp:Localize meta:resourcekey="ImportaScadenze" runat="server">Importa Scadenze</asp:Localize>
                            </div>
                        </div>

                        <div class="col-lg-2 col-md-3 col-sm-3" id="id_caricadatiapp">
                            <!--PULSANTE PER L'IMPORTAZIONE DATI APP-->
                            <div class="btn btn-default btn-block btn-app-import xonne-btn-primary" id="BtnImportaApp" style="display: none" onclick="<%= If(SincroDatiApp, "CaricaDocumentiApp", "CaricaDatiApp") %>();" title="<asp:Localize meta:resourcekey="CaricaDatiApp" runat="server">Carica Dati App</asp:Localize>">
                                <span class="fa fa-refresh"></span>
                                <asp:Localize meta:resourcekey="CaricaDatiApp" runat="server">Carica Dati App</asp:Localize>
                            </div>
                        </div>
                        <% End If %>
                        

                    </div>
            </ul>
        </div>
    </div>


    <!--GRIGLIA CON LE SCADENZE-->
    <div class="col-lg-12 col-md-12 col-sm-12" id="id_griglia">
        <div class="row">
            <div style="overflow: auto; margin-top: 10px; margin-bottom: 125px;">
                <div id="tabella_scadenze"></div>
            </div>
        </div>
    </div>

    <div id="importaScadenzeDialog"></div>
    <div id="CaricaDatiApp"></div>

    <!-- Kendo Window -->
    <template id="tmplWindowCompressoDaGias">
        <div id="winCompressoDaGIAS">
            <div class="window-content">
                <div class="row">
                    <div class="btn btn-success btn_scarica_file" id="btn_scarica_file" hidden>
                        <span class="fa fa-file-o lampeggiante"></span><span class="lampeggiante">
                            <asp:Localize meta:resourcekey="ScaricaTuttiFile" runat="server">Scarica tutti i file</asp:Localize>
                        </span>
                    </div>
                    <div class="btn btn-success btn_scarica_ZIP" id="btn_scarica_ZIP">
                        <span class="fa fa-file-archive-o lampeggiante"></span><span class="lampeggiante">
                            <asp:Localize meta:resourcekey="ScaricaTuttiFile" runat="server">Scarica tutti i file</asp:Localize>
                            <!-- <asp:Localize meta:resourcekey="ScaricaZIP" runat="server">Scarica in formato ZIP</asp:Localize> -->
                        </span>
                    </div>
                </div>

                <br />

                <div id="kendoUpload" class="row">
                    <input id="files" name="content" type="file" />
                </div>
            </div>
        </div>
    </template>
    <div id="containerWindowCompressoDaGias">
    </div>

    <input type="hidden" id="hdKendo_Valorizzazione" />
    <input type="hidden" id="hdModalita" runat="server" />
    <input type="hidden" id="hdAllegato_Permesso" runat="server" />
    <input type="hidden" id="hdAllegato_Permesso_Cancellazione" runat="server" />
    <input type="hidden" id="hdAllegato_Validazione" runat="server" />
    <input type="hidden" id="hdAllegato_Validazione_Visibilita" runat="server" />
    <input type="hidden" id="QS_Type" runat="server" />
    <input type="hidden" id="hdRichiesta_Cod" runat="server" />
    <input type="hidden" id="hdArea_Provenienza" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdIdAgenda" runat="server" />
    <input type="hidden" id="hdRicetta" runat="server" />
    <input type="hidden" id="hdCod_Contatto" runat="server" />
    <input type="hidden" id="hdIndici" runat="server" />
        
    <input type="hidden" id="hdAllegato_Permesso_Richiesta_Modifica" runat="server" />
    <input type="hidden" id="hdAllegato_Permesso_Approvazione_Richiesta_Modifica" runat="server" />
    <input type="hidden" id="hdAllegato_Permesso_Rendcontazione_Modifica" runat="server" />
    <input type="hidden" id="hdAllegato_Permesso_Approvazione_Rendcontazione_Modifica" runat="server" />
    <input type="hidden" id="hdAllegato_Permesso_Storicizzazione" runat="server" />

    <input type="hidden" id="hdAnalisi_Testata_Cod" runat="server" />
    <input type="hidden" id="hdMac_Cod" runat="server" />
    <input type="hidden" id="hdTipologia_Provenienza" runat="server" />

    <input type="hidden" id="hdSito_Provenienza" runat="server" />
    <input type="hidden" id="hdxFiltroDocumenti" runat="server" />
    <input type="hidden" id="hdDataUpload" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Lista_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Lista.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Lista_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Lista_ws_client.js") %>"></script>

    <script type="text/javascript">
        var cModalita = "#<%=hdModalita.ClientID() %>";
        var cAllegato_Permesso = "#<%=hdAllegato_Permesso.ClientID() %>";
        var cAllegato_Permesso_Cancellazione = "#<%=hdAllegato_Permesso_Cancellazione.ClientID() %>";
        var cAllegato_Validazione = "#<%=hdAllegato_Validazione.ClientID() %>";
        var cAllegato_Validazione_Visibilita = "#<%=hdAllegato_Validazione_Visibilita.ClientID() %>";
        var cRichiesta_Cod = "#<%=hdRichiesta_Cod.ClientID() %>";
        var cArea_Provenienza = "#<%=hdArea_Provenienza.ClientID() %>";
        var cPaginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
        var cPiva = "#<%=hdPiva.ClientID() %>";
        var cIdAgenda = "#<%=hdIdAgenda.ClientID() %>";
        var cRicetta = "#<%=hdRicetta.ClientID() %>";
        var cCod_Contatto = "#<%=hdCod_Contatto.ClientID() %>";

        var cAllegato_Permesso_Richiesta_Modifica = "#<%=hdAllegato_Permesso_Richiesta_Modifica.ClientID() %>";
        var cAllegato_Permesso_Approvazione_Richiesta_Modifica = "#<%=hdAllegato_Permesso_Approvazione_Richiesta_Modifica.ClientID() %>";
        var cAllegato_Permesso_Rendcontazione_Modifica = "#<%=hdAllegato_Permesso_Rendcontazione_Modifica.ClientID() %>";
        var cAllegato_Permesso_Approvazione_Rendcontazione_Modifica = "#<%=hdAllegato_Permesso_Approvazione_Rendcontazione_Modifica.ClientID() %>";
        var cAllegato_Permesso_Storicizzazione = "#<%=hdAllegato_Permesso_Storicizzazione.ClientID() %>";

        var cAnalisi_Testata_Cod = "#<%=hdAnalisi_Testata_Cod.ClientID() %>";
        var cMac_Cod = "#<%=hdMac_Cod.ClientID() %>"
        var cTipologia_Provenienza = "#<%=hdTipologia_Provenienza.ClientID() %>";

        var cSito_Provenienza = "#<%=hdSito_Provenienza.ClientID() %>";
        var cxFiltroDocumenti = "#<%=hdxFiltroDocumenti.ClientID() %>";
        var cDataUpload = "#<%=hdDataUpload.ClientID() %>";
        var cIndici = "#<%=hdIndici.ClientID() %>";

    </script>


    <script id="templateBtnStoricoSI" type="text/x-kendo-template">
        <div class="btn btn-warning submit" id="btn_StoricoSI" style="margin-right: 3px;" onclick="confermaStoricizzazione(1)" title="<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Storicizza %>" runat="server">Storicizza</asp:Localize>">
           <span class="lampeggiante">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Storicizza %>" runat="server">Storicizza</asp:Localize>
           </span>
        </div>
    </script>

    <script id="templateBtnStoricoNO" type="text/x-kendo-template">
        <div class="btn btn-warning submit" id="btn_StoricoNO" style="margin-right: 3px;" onclick="confermaStoricizzazione(0)" title="<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AnnullaStoricizzazione %>" runat="server">Annulla Storicizzazione</asp:Localize>">
            <span class="lampeggiante">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AnnullaStoricizzazione %>" runat="server">Annulla Storicizzazione</asp:Localize>
            </span>
        </div>
    </script>

    <script id="templateBtnEliminazioneMassiva" type="text/x-kendo-template">
        <div class="btn btn-danger" id="btn_EliminazioneMassiva" style="margin-right: 3px;" onclick="confermaEliminazioneMassivaElemento()" title="<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CancellazioneMassiva %>" runat="server">Cancellazione Massiva</asp:Localize>">
            <span class="lampeggiante">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CancellazioneMassiva %>" runat="server">Cancellazione Massiva</asp:Localize>
            </span>
        </div>
    </script>

    <script id="fileTemplate" type="text/x-kendo-template">               
        <div class='file-wrapper'>
            <span class='file-icon fa #= CreaIconaDownloadDocumento(name.split('.')[1])#' onclick = 'Apri_Doc_Allegato("#=name#") '></span>
            <h4 class='file-heading file-name-heading'>#=name#</h4>
            <h4 class='file-heading file-size-heading'>
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dimensione %>" runat="server">Dimensione</asp:Localize>
                : #= grandezzaFile(size) # </h4>                      
        </div>

        <div>
            <strong class="">
                <button name='rimuoviElemento' id='#=files[0].uid#' type='button' class='k-upload-action'></button>
            </strong>
        </div>
    </script>

    <script id="templateBtnScaricaAllegatiSelezionati" type="text/x-kendo-template">
        <div class="btn btn-danger" id="btn_ScaricaAllegatiSelezionati" style="margin-right: 3px;" onclick="ScaricaAllegati(1)" title="<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScaricaAllegatiSelezionati %>" runat="server">Scarica Allegati Selezionati</asp:Localize>">
            <span class="fa fa-file-zip-o lampeggiante"> </span>
            <span class="lampeggiante">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScaricaAllegatiSelezionati %>" runat="server">Scarica Allegati Selezionati</asp:Localize>
            </span>
        </div>
    </script>

    <script id="templateBtnScaricaAllegatiAudit" type="text/x-kendo-template">
        <div class="btn btn-danger" id="btn_ScaricaAllegatiAudit" style="margin-right: 3px;" onclick="ScaricaAllegati(2)" title="<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScaricaTuttiAllegati %>" runat="server">Scarica Tutti Gli Allegati</asp:Localize>">
            <span class="fa fa-file-zip-o lampeggiante" ></span> 
            <span class="lampeggiante">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScaricaTuttiAllegati %>" runat="server">Scarica Tutti Gli Allegati</asp:Localize>
            </span>
        </div>
    </script>

    <script id="templateBtnAvanzamentoStato" type="text/x-kendo-template">
        <div class="btn btn-info" id="btn_AvanzamentoStato" style="margin-right: 3px;" onclick="AvanzamentoStato()" title="Avanzamento Stato" <%--<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AvanzamentoStato %>" runat="server">Avanzamento Stato</asp:Localize>">--%>
            <span class="fa fa-file-zip-o lampeggiante"> </span>
            <span class="lampeggiante">
                Avanzamento Stato <%--<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AvanzamentoStato %>" runat="server">Avanzamento Stato</asp:Localize>--%>
            </span>
        </div>
    </script>
    <script type="text/javascript" src="<%=PATH_GIASBASE %>agronica/Scripts/fileSaver.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
</asp:Content>
