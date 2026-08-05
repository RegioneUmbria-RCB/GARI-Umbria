<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Scad_CreaModificaItem.aspx.vb" MasterPageFile="~/Master/AgendaBootstrap.Master"
    Inherits="AgroAgenda_2010.Scad_CreaModificaItem" %>

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
        /*
        span.k-button.k-state-active {
            background: #ebebeb; //#3276B1;
            color: black;
        }*/

        .ricerca {
            color: #fff;
            border-color: #eea236;
            background: #eea236;
            margin: 0px 5px 0px 5px;
            border-radius: 4px;
        }

            .ricerca:hover {
                color: #fff;
                border-color: #ec971f;
                background: #ec971f;
            }

        .k-state-active.ricerca, .k-active.ricerca {
            color: #fff;
            border-color: #eea236;
            background: #eea236;
            box-shadow: none;
        }

            .k-state-active.ricerca:hover, .k-active.ricerca:hover {
                color: #fff;
                border-color: #ec971f;
                background: #ec971f;
            }

        .k-button.ricerca {
            border-radius: 4px;
        }
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!--SEZIONE DOVE SCRIVERE I MESSAGGI D'ERRORE-->
    <div id="DIV_Messaggi"></div>

    <!--VARIABILE IN CUI VIENE SALVATO IL JSON ED IL SUO ID-->
    <asp:HiddenField ID="hfId_Alert_Entita" runat="server" />
    <asp:HiddenField ID="hfId_Elenco" runat="server" />

    <!--VARIABILE PER I PERMESSI DI SCRITTURA-->
    <asp:HiddenField ID="hf_UtenteAbilitatoScrittura" runat="server" />
    <asp:HiddenField ID="hf_UploadMultiploAllegatiAbilitato" runat="server" />

    <!--SEZIONE CON I CONTROLLI DELLA SCADENZA-->
    <div id="PnlDettagli" class="panel" style="margin-top: 10px; margin-bottom: 50px; padding-top: 15px;">
        <%If (Master.Master_versione = "2022")%>
        <div class="row xo-mb-2 xo-mr-1 xo-p-x-15px" style="padding-left: 5px;">
            <button type="button" class="btn btn-success xi-btn-primary btn-float-r" id="btnSalva" style="margin-left: 8px" onclick="Salva(true);">
                <span class="fa fa-floppy-o"></span>
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva e Chiudi</asp:Localize>
            </button>
            <button type="button" class="btn btn-success xi-btn-primary btn-float-r" id="btnNuovo" onclick="Salva(false);">
                <span class="fa fa-floppy-o"></span>
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaENuovo %>" runat="server">Salva e Nuovo</asp:Localize>
            </button>
        </div>
        <% End If %>

        <div class="row first-row">
            <div class="col-lg-9 col-md-8 col-sm-12">
                <div class="k-card-list">
                    <div class="k-card k-state-pers1" id="cardDocPrincipale" style="margin-bottom: 10px;">
                        <%If (Master.Master_versione <> "2022")%>
                        <div class="k-card-body">
                            <% End If %>

                            <div class="row">

                                <!-- ID SCADENZA -->
                                <div class="col-lg-3 col-md-3 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" id="CTRL_ID" for="txbID">
                                            <asp:Localize meta:resourcekey="DocumentoID" runat="server">ID</asp:Localize></label>
                                        <input type="text" id="txbID" class="form-control" aria-describedby="CTRL_ID" readonly="readonly" />
                                    </div>
                                </div>

                                <!-- AZIENDA -->
                                <div class="col-lg-9 col-md-9 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" id="CTRL_Azienda" for="ddlAzienda">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Azienda %>" runat="server">Azienda</asp:Localize>
                                        </label>
                                        <input type="text" id="ddlAzienda" name="ddlAzienda" class="form-control" aria-describedby="CTRL_Azienda" onchange="ddlAzienda_Change();" />
                                    </div>
                                </div>

                            </div>
                            <%If (Master.Master_versione <> "2022")%>
                        </div>
                        <% End If %>
                    </div>
                </div>
            </div>
            <%If (Master.Master_versione <> "2022")%>
            <div class="col-lg-3 col-md-2 col-xs-12" style="padding-left: 5px;">
                <button type="button" class="btn btn-success xi-btn-primary btn-float-r" id="btnSalva" style="margin-left: 8px" onclick="Salva(true);">
                    <span class="fa fa-floppy-o"></span>
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva e Chiudi</asp:Localize>
                </button>
                <button type="button" class="btn btn-success xi-btn-primary btn-float-r" id="btnNuovo" onclick="Salva(false);">
                    <span class="fa fa-floppy-o"></span>
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaENuovo %>" runat="server">Salva e Nuovo</asp:Localize>
                </button>
            </div>
            <% End If %>
        </div>

        <div class="row second-row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="k-card-list">
                    <div class="k-card k-state-pers1" id="cardInfoTrasporto" style="margin-bottom: 10px;">
                        <%If (Master.Master_versione <> "2022")%>
                        <div class="k-card-body">
                            <% End If %>

                            <div class="row">
                                <!-- AREA/CATEGORIA  -->
                                <div class="col-lg-6 col-md-6 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" id="CTRL_Area" for="ddlArea">
                                            <asp:Localize meta:resourcekey="CategoriaDocumento" runat="server">Categoria</asp:Localize>
                                        </label>
                                        <input type="text" id="ddlArea" name="ddlArea" class="form-control ddlkendo" aria-describedby="CTRL_Area" onchange="ddlArea_Change();" />
                                    </div>
                                </div>
                                <!-- TIPOLOGIA -->
                                <div class="col-lg-6 col-md-6 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" id="CTRL_Tipologia" for="ddlTipologia">
                                            <asp:Localize meta:resourcekey="TipologiaDocumento" runat="server">Tipologia</asp:Localize>
                                        </label>
                                        <input type="text" id="ddlTipologia" name="ddlTipologia" class="form-control" aria-describedby="CTRL_Tipologia" onchange="ddlTipologia_Change();" />
                                    </div>
                                </div>
                            </div>

                            <!-- MULTI TIPOLOGIA  Anna 29/04/22: aggiunto multiselect tipologie al documento -->
                            <div class="row" id="divMultiTipologia">
                                <div class="col-lg-8 col-md-6 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="lbl_cmbTipologia" for="cmbTipologia">
                                            <asp:Localize meta:resourcekey="AltreTipologieDocumento" runat="server">Altre tipologie a cui associare il documento</asp:Localize>
                                        </label>
                                        <input type="text" id="cmbTipologia" name="cmbTipologia" class="form-control" aria-describedby="lbl_cmbTipologia" />
                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-6 col-xs-12">
                                    <b>NB:
                                        <asp:Localize meta:resourcekey="SalvataggioMultiTipologiaDocumento" runat="server">Al salvataggio verranno creati tanti documenti quante sono le tipologie aggiuntive scelte</asp:Localize></b>
                                </div>

                            </div>
                            <%If (Master.Master_versione <> "2022")%>
                        </div>
                        <% End If %>
                    </div>
                </div>
            </div>

            <!-- CENTRO AZIENDALE -->
            <div id="pnlCentro" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Centro" for="ddlCentro">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server">Centro Aziendale</asp:Localize>
                        </label>
                        <input type="text" id="ddlCentro" name="ddlCentro" class="form-control" aria-describedby="CTRL_Centro" />
                    </div>
                </div>
            </div>

            <!-- APPEZZAMENTO -->
            <div id="pnlAppezzamento" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Appezzamento" for="ddlAppezzamento">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Appezzamento %>" runat="server">Appezzamento</asp:Localize>
                        </label>
                        <input type="text" id="ddlAppezzamento" name="ddlAppezzamento" class="form-control" aria-describedby="CTRL_Appezzamento" />
                    </div>
                </div>
            </div>

            <!-- MACCHINA -->
            <div id="pnlMacchina" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon lbl_required" id="CTRL_Macchina" for="ddlMacchina">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MacchinaAttrezzatura %>" runat="server">Macchina / Attrezzatura</asp:Localize>
                        </label>
                        <input type="text" id="ddlMacchina" name="ddlMacchina" class="form-control" aria-describedby="CTRL_Macchina" />
                    </div>
                </div>
            </div>

            <!-- CONTATTO -->
            <div id="pnlContatto" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon lbl_required" id="CTRL_Contatto" for="ddlContatto">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Contatto %>" runat="server">Contatto</asp:Localize>
                        </label>
                        <input type="text" id="ddlContatto" name="ddlContatto" class="form-control" aria-describedby="CTRL_Contatto" />
                    </div>
                </div>
            </div>

            <!-- RIFERIMENTI -->
            <div id="pnlRiferimentiAgenda_Ricette_Catasto" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="k-card-list">
                        <div class="k-card k-state-pers1" style="margin-bottom: 10px;">
                            <%If (Master.Master_versione <> "2022")%>
                            <div class="k-card-body">
                                <% End If %>
                                <div class="row">
                                    <div id="filtriGriglia">
                                        <div id="filtroDate" style="display: none">
                                            <div class="col-lg-3 col-md-3 col-xs-3">
                                                <div class="input-group">
                                                    <label class="input-group-addon" id="lbl_txt_data_da" for="txt_data_da">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDa %>" runat="server">Data da</asp:Localize>
                                                    </label>
                                                    <input onchange='onChange_Data()' type="text" id="txt_data_da" name="txt_data_da" class="form-control kendoCalendar" aria-describedby="txt_data_da" maxlength="10" />
                                                </div>
                                            </div>

                                            <div class="col-lg-3 col-md-3 col-xs-3">
                                                <div class="input-group">
                                                    <label class="input-group-addon" id="lbl_txt_data_a" for="txt_data_a">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataA %>" runat="server">Data a</asp:Localize>
                                                    </label>
                                                    <input onchange='onChange_Data()' type="text" id="txt_data_a" name="txt_data_a" class="form-control kendoCalendar" aria-describedby="txt_data_a" maxlength="10" />
                                                </div>
                                            </div>
                                        </div>

                                        <div id="cercaRiferimenti" style="display: none">
                                            <div class="col-lg-2 col-md-2 col-xs-12" style="padding-left: 5px;">
                                                <button type="button" class="btn btn-warning" id="btnCerca" onclick="button_cercaRiferimenti()">
                                                    <span class="fa fa-search"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaRiferimenti %>" runat="server">Cerca Riferimenti</asp:Localize>
                                                </button>
                                            </div>
                                        </div>

                                        <div id="cercaParticelleCatastali" style="display: none">
                                            <div class="col-lg-2 col-md-2 col-xs-12" style="padding-left: 5px;">
                                                <button type="button" class="btn btn-warning" id="btnCercaParticelleCatastali" onclick="button_cercaParticelleCatastali()">
                                                    <span class="fa fa-search"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaParticelleCatastali %>" runat="server">Cerca Particelle Catastali</asp:Localize>
                                                </button>
                                            </div>
                                        </div>

                                        <% If Master.Master_versione = "2022" Then %>
                                        <div class="col-lg-4 col-md-4 col-xs-4">
                                            <div id="TipoOutput_Attivita_RicetteODLBrogliaccio_Visite" class="mt-30">
                                                <span class="ricerca btn xonne-btn-primary"><span class="fa fa-search xonne-search"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaAttivita %>" runat="server">Cerca Attività</asp:Localize>
                                                </span>
                                                <span class="ricerca btn xonne-btn-primary"><span class="xonne-search"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaRicetteODLBrogliacci %>" runat="server">Cerca Ricette/ODL e Brogliacci</asp:Localize>
                                                </span>
                                                <span class="ricerca btn xonne-btn-primary"><span class="xonne-search"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaVisite %>" runat="server">Cerca Visite</asp:Localize>
                                                </span>
                                            </div>
                                        </div>
                                        <% Else %>
                                        <div class="col-lg-4 col-md-4 col-xs-4">
                                            <div class="btn btn-warning" id="TipoOutput_Attivita_RicetteODLBrogliaccio_Visite">
                                                <span class="ricerca"><span class="fa fa-search"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaAttivita %>" runat="server">Cerca Attività</asp:Localize>
                                                </span>
                                                <span class="ricerca"><span class="fa fa-search"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaRicetteODLBrogliacci %>" runat="server">Cerca Ricette/ODL e Brogliacci</asp:Localize>
                                                </span>
                                                <span class="ricerca"><span class="fa fa-search"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaVisite %>" runat="server">Cerca Visite</asp:Localize>
                                                </span>
                                            </div>
                                        </div>
                                        <% End If %>
                                    </div>
                                </div>

                                <div class="col-lg-12 col-md-12 col-sm-12" id="griglie">

                                    <div class="row" id="Riferimenti" style="display: none">
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 10px;">
                                            <label class="input-group-addon lbl_required" id="lbl_griglia_Riferimenti" for="griglia_Riferimenti">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Riferimenti %>" runat="server">Riferimenti</asp:Localize>
                                            </label>
                                            <div id="griglia_Riferimenti"></div>
                                        </div>
                                    </div>

                                    <div class="row" id="Attivita" style="display: none">
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 10px;">
                                            <label class="input-group-addon lbl_required" id="lbl_griglia_Attivita" for="griglia_Attivita">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Attività %>" runat="server">Attività</asp:Localize>
                                            </label>
                                            <div id="griglia_Attivita"></div>
                                        </div>
                                    </div>

                                    <div class="row" id="RicetteODLBrogliaccio" style="display: none">
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 10px;">
                                            <label class="input-group-addon lbl_required" id="lbl_griglia_RicetteODLBrogliaccio" for="griglia_RicetteODLBrogliaccio">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RicetteODLBrogliacci %>" runat="server">Ricette/ODL e Brogliacci</asp:Localize>
                                            </label>
                                            <div id="griglia_RicetteODLBrogliaccio"></div>
                                        </div>
                                    </div>

                                    <div class="row" id="Visite" style="display: none">
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 10px;">
                                            <label class="input-group-addon lbl_required" id="lbl_griglia_Visite" for="griglia_Visite">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Visite %>" runat="server">Visite</asp:Localize>
                                            </label>
                                            <div id="griglia_Visite"></div>
                                        </div>
                                    </div>

                                    <div class="row" id="ParticelleCatastali" style="display: none">
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 10px;">
                                            <label class="input-group-addon lbl_required" id="lbl_griglia_ParticelleCatastali" for="griglia_ParticelleCatastali">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParticelleCatastali %>" runat="server">Particelle Catastali</asp:Localize>
                                            </label>
                                            <div id="griglia_ParticelleCatastali"></div>
                                        </div>
                                    </div>
                                </div>

                                <%--                                <div class="col-lg-12 col-md-12 col-xs-12">
                                    <div class="input-group">
                                        <label class="input-group-addon lbl_required" id="CTRL_Agenda" for="ddlAgendaPIPPO">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Riferimenti %>" runat="server">Riferimenti</asp:Localize>
                                        </label>
                                        <input type="text" id="ddlAgendaPIPPO" name="ddlAgendaPIPPO" class="form-control ddlkendo" aria-describedby="CTRL_Agenda" onchange="ddlAgendaPIPPO_Change();" />
                                    </div>
                                </div>--%>
                                <%If (Master.Master_versione <> "2022")%>
                            </div>
                            <% End If %>
                        </div>
                    </div>
                </div>
            </div>

            <!-- RICETTA -->
            <div id="pnlRicetta" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon lbl_required" id="CTRL_Ricetta" for="ddlRicetta_Operazione_Cod">
                            Ricetta Operazione
                        </label>
                        <input type="text" id="ddlRicetta" name="ddlRicetta" class="form-control ddlkendo" aria-describedby="CTRL_Ricetta" />
                    </div>
                </div>
            </div>

            <!-- UMA CARBURANTI -->
            <div id="pnlUMA" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_UMA" for="ddlUma_Carburanti">
                            <%--<asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, "Richiesta" %>" runat="server">--%>Pratica
                       <%-- </asp:Localize>--%>
                        </label>
                        <input type="text" id="ddlUma_Carburanti" name="ddlUma_Carburanti" class="form-control" aria-describedby="CTRL_UMA" />
                    </div>
                </div>
            </div>

            <!-- ANALISI -->
            <div id="pnlAnalisi" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Analisi" for="ddlAnalisi">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Analisi %>" runat="server">Analisi</asp:Localize>
                        </label>
                        <input type="text" id="ddlAnalisi" name="ddlAnalisi" class="form-control" aria-describedby="CTRL_Analisi" />
                    </div>
                </div>
            </div>

            <!-- PIANO CONCIMAZIONE -->
            <div id="pnlPianoConcimazione" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_PianoConcimazione" for="ddlPianoConcimazione">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PianoConcimazione %>" runat="server">Piano Concimazione</asp:Localize>
                        </label>
                        <input type="text" id="ddlPianoConcimazione" name="ddlPianoConcimazione" class="form-control" aria-describedby="CTRL_PianoConcimazione" />
                    </div>
                </div>
            </div>

            <!-- PUA -->
            <div id="pnlPua" class="row" style="margin-top: 10px; display: none;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Pua" for="ddlPua">PUA</label>
                        <!-- i18n -->
                        <input type="text" id="ddlPua" name="ddlPua" class="form-control" aria-describedby="CTRL_Pua" />
                    </div>
                </div>
            </div>


            <!-- LISTA INDICI -->
            <div class="row" id="id_indici_list" style="margin-top: 5px;">
                <!-- N.B. Riempita dinamicamente  -->
            </div>


            <%--      <div id="datiDocumento" class="row">

            <div class="col-lg-12 border_si">
                <div class="row">
                    <div class="col-md-12 text-center">
                        <h4 style="color: #052747; text-transform: uppercase;"><%=hdTitle.Value%></h4>
                        <input type="hidden" id="Txt_ID_Elenco" value="0" />
                    </div>
                </div>--%>


            <div class="row xo-p-x-15px" style="margin-top: 10px;">
                <!-- DESCRIZIONE -->
                <div class="col-lg-9 col-md-9 col-xs-9">
                    <div class="input-group">
                        <label class="input-group-addon lbl_required" id="CTRL_Descrizione" for="txbDescrizione">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server">Descrizione</asp:Localize>
                        </label>
                        <textarea id="txbDescrizione" name="txbDescrizione" class="form-control" aria-describedby="CTRL_Descrizione" rows="2"></textarea>
                    </div>
                </div>

                <!-- SCADENZA -->
                <div class="col-lg-3 col-md-3 col-sm-3" id="divDataScadenza">
                    <div class="form-horizontal">
                        <div class="input-group">
                            <label class="input-group-addon" id="Lbl_Data_Scadenza" for="Txt_Data_Scadenza"><%=hdTitle_Scadenza.Value%></label>
                            <input id="Txt_Data_Scadenza" name="Txt_Data_Scadenza" class="kendoCalendar" maxlength="10" />
                        </div>
                    </div>
                </div>
            </div>


            <!-- NOTE -->
            <div class="row xo-p-x-15px" style="margin-top: 10px;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Note" for="txbNote">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server">Note</asp:Localize>
                        </label>
                        <textarea id="txbNote" name="txbNote" class="form-control" aria-describedby="CTRL_Note" rows="3"></textarea>
                    </div>
                </div>
            </div>

        </div>
        <%If (Master.Master_versione <> "2022")%>
    </div>
    <% End If %>

    <!-- ALLEGATO KENDOUPLOAD (multi allegato) / Upload (singolo allegato) -->
    <div id="pnlAllegato_kendoUpload" class="row xo-p-x-15px" style="display: none">
        <div class="dropZoneElement <%If (Master.Master_versione = "2022")%> col-lg-12 col-md-12 col-xs-12 <% End If %>" id="paperino">
            <label class="input-group-addon lbl_required" for="files" id="lbl_Documento_Allegato_kendoUpload"><%=hdTitle_Allegato.Value%><span class="fa fa-folder-open fa-3"></span></label>
            <input id="files" name="content" type="file" class="kendoUpload" />

            <input type="hidden" id="Txt_Documento_Allegato_kendoUpload" />
            <input type="hidden" id="File_Allegato_kendoUpload" />
            <input type="hidden" id="File_Caricato_kendoUpload" />

        </div>

        <div id="pnlDatiAllegato_kendoUpload" class="row">
            <div class="row" style="margin-top: 10px;">
                <br />
                <!-- NUMERO -->
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon" id="lbl_Documento_kendoUpload" for="Txt_Num_Documento_kendoUpload">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NrAllegato %>" runat="server">Nr. Allegato</asp:Localize></span>
                                <input type="text" id="Txt_Num_Documento_kendoUpload" class="form-control " />
                            </div>
                        </div>
                    </div>
                </div>

                 <!-- DATA ALLEGATO -->
                 <div class="col-lg-3 col-md-3 col-sm-3" id="divDataAllegato">
                     <div class="form-horizontal">
                         <div class="input-group">
                             <label class="input-group-addon" id="Lbl_Data_Allegato" for="Txt_Data_Scadenza">
                                 <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataAllegato %>" runat="server">Data Allegato</asp:Localize>
                             </label>
                             <input id="Txt_Data_Allegato" name="Txt_Data_Allegato" class="kendoCalendar" maxlength="10" />
                         </div>
                     </div>
                 </div>


                <!-- UTENTE VALIDAZIONE -->
                <div class="col-lg-3 col-md-3 col-sm-12" id="id_validazione_kendoUpload">
                    <div class="form-horizontal">
                        <div class="input-group">
                            <div class="input-group">
                                <label class="input-group-addon" for="cmbValidazione_kendoUpload">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Validazione %>" runat="server">Validazione</asp:Localize>:
                                </label>
                                <select name="cmbValidazione_kendoUpload" id="cmbValidazione_kendoUpload" class="form-control"></select>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-3 col-md-3 col-sm-12" id="stato_des">
                    <div class="input-group">
                        <span class="input-group-addon lbl_required" id="lblstato_pratica" for="stato_pratica">
                            Stato:
                        </span>
                        <asp:TextBox ID="stato_pratica" runat="server" CssClass="form-control" ReadOnly="true">
                        </asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row" style="margin-top: 10px;">

                <!-- UTENTE UPLOAD -->
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <span class="input-group-addon lbl_required" id="lblUsername_kendoUpload" for="Txt_Username_kendoUpload">
                            <asp:Localize meta:resourcekey="UtenteUpload" runat="server">Utente Upload</asp:Localize>
                        </span>

                        <asp:TextBox ID="Txt_Username_kendoUpload" runat="server" CssClass="form-control" ReadOnly="true">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- DATA UPLOAD -->
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group" readonly="true">
                                <%--<asp:Localize meta:resourcekey="DataUpload" runat="server">Data Upload</asp:Localize>--%>
                                <label class="input-group-addon lbl_required" id="lblData_Upload_kendoUpload" for="Txt_Data_Upload_kendoUpload">
                                    <asp:Localize meta:resourcekey="DataUpload" runat="server">Data Upload</asp:Localize>
                                </label>
                                <input id="Txt_Data_Upload_kendoUpload" name="Txt_Data_Upload_kendoUpload" class="kendoCalendar" readonly="readonly" maxlength="10" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="lblStorico_kendoUpload" for="ChkStorico_kendoUpload">
                            <asp:Localize meta:resourcekey="Storico" runat="server">Storicizzato</asp:Localize>
                        </label>
                        <input type="checkbox" name="ChkStorico_kendoUpload" id="ChkStorico_kendoUpload" class="kendoSwitch" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="pnlAllegato" class="row" style="display: none">

        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <span class="input-group-addon lbl_required" id="lbl_Documento_Allegato"><%=hdTitle_Allegato.Value%></span>
                        <span class="input-group-btn">
                            <button id="Btn_Sfoglia_Allegato" class="btn btn-default" type="button" onclick="openFileDialogFinto();" style="font-size: 18px">
                                <span class="fa fa-folder-open fa-3"></span>
                            </button>
                        </span>
                        <input type="text" id="Txt_Documento_Allegato" class="form-control" readonly="readonly" style="font-size: 18px" />
                        <input type="file" id="File_Allegato" onchange="scriviPercorsoFileSuTxt();" style="display: none;" />
                        <input type="hidden" id="File_Caricato" />
                    </div>

                </div>
            </div>


            <div id="pnlDatiAllegato" class="row">
                <div class="row" style="margin-top: 10px;">

                    <!-- NUMERO -->
                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon" id="lbl_Documento" for="Txt_Num_Documento">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NrAllegato %>" runat="server">Nr. Allegato</asp:Localize></span>
                                    <input type="text" id="Txt_Num_Documento" class="form-control " />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- UTENTE VALIDAZIONE -->
                    <div class="col-lg-3 col-md-3 col-sm-12" id="id_validazione">
                        <div class="form-horizontal">
                            <div class="input-group">
                                <div class="input-group">
                                    <label class="input-group-addon" for="cmbValidazione">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Validazione %>" runat="server">Validazione</asp:Localize>:
                                    </label>
                                    <select name="cmbValidazione" id="cmbValidazione" class="form-control"></select>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="row" style="margin-top: 10px;">

                    <!-- UTENTE UPLOAD -->
                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="input-group">
                            <span class="input-group-addon lbl_required" id="lblUsername" for="Txt_Username">
                                <asp:Localize meta:resourcekey="UtenteUpload" runat="server">Utente Upload</asp:Localize>
                            </span>

                            <asp:TextBox ID="Txt_Username" runat="server" CssClass="form-control" ReadOnly="true">
                            </asp:TextBox>
                        </div>
                    </div>

                    <!-- DATA UPLOAD -->
                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group" readonly="true">
                                    <%--<asp:Localize meta:resourcekey="DataUpload" runat="server">Data Upload</asp:Localize>--%>
                                    <label class="input-group-addon lbl_required" id="lblData_Upload" for="Txt_Data_Upload">
                                        <asp:Localize meta:resourcekey="DataUpload" runat="server">Data Upload</asp:Localize>
                                    </label>
                                    <input id="Txt_Data_Upload" name="Txt_Data_Upload" class="kendoCalendar" readonly="readonly" maxlength="10" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="input-group">
                            <label class="input-group-addon" id="lblStorico" for="ChkStorico">
                                <asp:Localize meta:resourcekey="Storico" runat="server">Storicizzato</asp:Localize>
                            </label>
                            <input type="checkbox" name="ChkStorico" id="ChkStorico" class="kendoSwitch" />
                        </div>
                    </div>


                    <!-- RIMUOVI -->
                    <div class="col-lg-3 col-md-3 col-sm-12 text-right" id="id_rimuovi">
                        <div class="btn btn-success" id="btn_Rimuovi">
                            <span class="fa fa-ban"></span><span class="lampeggiante">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Rimuovi %>" runat="server">Rimuovi</asp:Localize></span>
                        </div>
                    </div>
                </div>
            </div>



            <div id="pnlApriAllegato" class="row" style="margin-top: 10px; margin-bottom: 10px;">

                <!-- Allegato -->
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="form-horizontal">
                        <span class="input-group-addon" id="lblApriallegato">
                            <asp:Localize meta:resourcekey="ApriAllegato" runat="server">Apri allegato:</asp:Localize></span>
                    </div>
                </div>
                <div class="col-lg-10 col-md-10 col-sm-12">
                    <div class="form-horizontal">
                        <div id="divDownloadAllegato"></div>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <%If (Master.Master_versione = "2022")%>
    </div>
    <% End If %>


    <%-- <iframe id="iframe" style="display:none;"></iframe>--%>
    <input type="hidden" id="hdGrigliaRiferimenti_Valorizzazione" />
    <input type="hidden" id="hdGrigliaRicetteODLBrogliaccio_Valorizzazione" />
    <input type="hidden" id="hdGrigliaAttivita_Valorizzazione" />
    <input type="hidden" id="hdGrigliaVisite_Valorizzazione" />
    <input type="hidden" id="hdGrigliaParticelleCatastali_Valorizzazione" />

    <input type="hidden" id="hdUsername" runat="server" />
    <input type="hidden" id="hdPivaSuperUser" runat="server" />
    <input type="hidden" id="hdModalita" runat="server" />
    <input type="hidden" id="hdAllegato_Permesso" runat="server" />
    <input type="hidden" id="hdTipoOperazione" runat="server" />
    <input type="hidden" id="hdAllegato_Validazione" runat="server" />
    <input type="hidden" id="hdAllegato_Validazione_Visibilita" runat="server" />
    <input type="hidden" id="hdTitle" runat="server" />
    <input type="hidden" id="hdTitle_Scadenza" runat="server" />
    <input type="hidden" id="hdTitle_Allegato" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />

    <input type="hidden" id="hdRichiesta_Cod" runat="server" />
    <input type="hidden" id="hdId_Schema_Template" runat="server" />
    <input type="hidden" id="hdId_Tipologia" runat="server" />
    <input type="hidden" id="hdIdAgenda" runat="server" />
    <input type="hidden" id="hdRicetta_Operazione_Cod" runat="server" />
    <input type="hidden" id="hdAnalisi_Testata_Cod" runat="server" />
    <input type="hidden" id="hdCod_Contatto" runat="server" />
    <input type="hidden" id="hdMac_Cod" runat="server" />   

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdArea_Provenienza" runat="server" />
    <input type="hidden" id="hdData_Scadenza_Analisi" runat="server" />
    <input type="hidden" id="hdAllegato_Permesso_Storicizzazione" runat="server" />
    <input type="hidden" id="hdWorkFlow_Abilitato" runat="server" />
    <input type="hidden" id="hdSito_Provenienza" runat="server" />
    <input type="hidden" id="hdxFiltroDocumenti" runat="server" />
    <input type="hidden" id="hdIndici" runat="server" />
    <input type="hidden" id="hdFiltro" runat="server" />

</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_CreaModificaItem_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_CreaModificaItem.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_CreaModificaItem_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_CreaModificaItem_ws_client.js") %>"></script>

    <script type="text/javascript">

        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";

        var cUsername = "#<%=hdUsername.ClientID() %>";
        var cPivaSuperUser = "#<%=hdPivaSuperUser.ClientID() %>";
        var cModalita = "#<%=hdModalita.ClientID() %>";
        var cTipoOperazione = "#<%=hdTipoOperazione.ClientID() %>";
        var cPaginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
        var cAllegato_Permesso = "#<%=hdAllegato_Permesso.ClientID() %>";
        var cAllegato_Validazione = "#<%=hdAllegato_Validazione.ClientID() %>";
        var cAllegato_Validazione_Visibilita = "#<%=hdAllegato_Validazione_Visibilita.ClientID() %>";
        var cId_Tipologia = "#<%=hdId_Tipologia.ClientID() %>";
        var cPiva = "#<%=hdPiva.ClientID() %>";
        var cArea_Provenienza = "#<%=hdArea_Provenienza.ClientID() %>";

        var cRichiesta_Cod = "#<%=hdRichiesta_Cod.ClientID() %>";
        var cAnalisi_Testata_Cod = "#<%=hdAnalisi_Testata_Cod.ClientID() %>";
        var cIdAgenda = "#<%=hdIdAgenda.ClientID() %>";
        var cId_Schema_Template = "#<%=hdId_Schema_Template.ClientID() %>";
        var cRicetta_Operazione_Cod = "#<%=hdRicetta_Operazione_Cod.ClientID() %>";
        var cCod_Contatto = "#<%=hdCod_Contatto.ClientID() %>";
        var cMac_Cod = "#<%=hdMac_Cod.ClientID() %>";       

        var cData_Scadenza_Analisi = "#<%=hdData_Scadenza_Analisi.ClientID() %>";
        var cAllegato_Permesso_Storicizzazione = "#<%=hdAllegato_Permesso_Storicizzazione.ClientID() %>";
        var workFlow_Abilitato = "#<%=hdWorkFlow_Abilitato.ClientID() %>";
        var cSito_Provenienza = "#<%=hdSito_Provenienza.ClientID() %>";
        var cxFiltroDocumenti = "#<%=hdxFiltroDocumenti.ClientID() %>";
        var cIndici = "#<%=hdIndici.ClientID() %>";
        var cFiltro = "#<%=hdFiltro.ClientID() %>";

    </script>


    <script id="fileTemplate" type="text/x-kendo-template">               
        <div class='file-wrapper'>
            <span class='file-icon fa #= CreaIconaDownloadDocumento(name.split('.')[1])#' onclick = 'Apri_Doc_Allegato("#=name#") '></span>
            <h4 class='file-heading file-name-heading'>#=name#</h4>
            <h4 class='file-heading file-size-heading'><asp:Localize meta:resourcekey="Dimensione" runat="server">Dimensione</asp:Localize>: #= grandezzaFile(size) # </h4>                      
        </div>

        <div>
            <strong class="">
                <button name='rimuoviElemento' id='#=files[0].uid#' type='button' class='k-upload-action'></button>
            </strong>
        </div>
    </script>

    <script type="text/javascript" src="<%=PATH_GIASBASE %>agronica/Scripts/fileSaver.min.js?<% =Application("GiasVersioneCorrente")%>"></script>

</asp:Content>
