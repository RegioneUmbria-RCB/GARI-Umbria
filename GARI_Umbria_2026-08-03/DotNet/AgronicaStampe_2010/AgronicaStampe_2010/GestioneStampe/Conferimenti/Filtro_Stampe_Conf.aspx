<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Filtro_Stampe_Conf.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_Stampe_Conf" MasterPageFile="~/Master/StampeBootstrap.Master" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <title>Filtro Stampe Conferimenti</title>
    <style>
        /* Questo selettore per uniformare la classe alla versione 3.3 di Bootstrap (la più vecchia documentata)
           rispetto alla nostra 3.0 attualmente utilizzata (2020/07/10)
        */
        /*.form-inline .form-group {
            display: inline-table;
        }

        .form-inline .input-group {
            display: inline-table;
        }*/

        .display-flex {
            display: flex;
        }

        .fixed-header {
            top:0;
            position:fixed;
            width:auto;
            z-index: 1;
        }

        #ui-datepicker-div {
            Z-INDEX: 10000
        }

        #containerNew {
            /*padding: 45px;*/
            padding-top: 30px;
        }

        .boxDate, .boxStampeMassive, .boxCentriMagazzini, .boxSoggetti, .boxProdotti {
            display: none;
        }

        .boxStampeMassive, .boxCentriMagazzini, .boxSoggetti, .boxProdotti {
            margin-top: 15px;
        }

        .titleProdotto, .titleConferente {
            margin-bottom: 5px;
        }

        /*.containerFiltriDateInizioFine, .containerEstrazioniEsegui {
            justify-content: space-between;
        }*/

        /*.containerDataInizio, .containerDataFine {
            flex-grow: 1;
        }*/

        /*.containerDataInizio {
            margin-right: 20px;
        }

        .containerDataFine {
            margin-left: 20px;
        }*/

        .containerEstrazioniEsegui {
            justify-content: space-evenly;
            flex-wrap: wrap;
            margin: 0 5px 20px 5px;
        }

        .containerElencoEstrazioni {
            flex-grow: 2;
        }

        #btnEseguiEstrazione {
            flex-grow: 1;
            height: max-content !important;
            margin-left: 20px;
            max-width: 350px;
            min-width: 180px;
        }


        .boxSoggetti .containerRapportoContabile {
            margin-bottom: 20px;
        }

        #titoloConferenti {
            margin-bottom: 10px;
        }

        #grigliaConferenti {
            margin-bottom: 20px;
        }

        .rowContainerStampanti {
            margin-top: 10px;
        }

        /*.containerDocNumeri .k-textbox, .containerDocNumeri .k-numerictextbox {
            width: auto;
        }*/

        .containerDocPrimoLabel, .containerDocUltimoLabel, .containerDocPrefissoLabel, .containerDocSuffissoLabel {
            display:inline-block;
            float: left;
            padding-top: 5px;
        }

        .containerDocNumeriLabel {
            margin-bottom: 5px;
        }

        .containerDocNumeriLabel, .containerDocPrimoLabel, .containerDocPrefissoLabel {
            padding-left: 15px;
        }

        .containerDocUltimoLabel, .containerDocSuffissoLabel {
            padding-left: 30px;
        }


        /*#btnEseguiEstrazione {
            width: 100%;
        }*/

        #containerFiltroStampe > * {
            display: none;
        }

        #containerFiltroStampe > #containerNew {
            display: block;
        }

        #confirmStampaMassiva .form-control {
            width: 80%;
        }

        #confirmStampaMassiva label {
            margin-top: 10px;
        }

        #confirmStampaMassiva label:first-child {
            margin-top: 0px;
        }

        #confirmStampaMassiva #tAreaConfermaFiltri {
            height: 85px !important;
            resize: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        // Variabili globali pagina che necessitano di valorizzazione tramite visual basic
        var cIdPiva = "#<%= hdPiva.ClientID %>";
        var cIdSuperUserAccGerarchia = "#<%= hdSuperUserAccGerarchia.ClientID %>";
        var cIdIntConfigurazioneModuli = "#<%= hdIntConfigurazioneModuli.ClientID %>";
        var cIdAbilitaExportConf = "#<%= hdAbilitaExportConf.ClientID %>";
        var hf_filtroMateriePrimeConferimento = "#<%= hf_filtroMateriePrimeConferimento.ClientID %>";
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script src="<%= ResolveClientUrl("~/GestioneStampe/Conferimenti/Filtro_Stampe_Conf_ws_client.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/GestioneStampe/Conferimenti/Filtro_Stampe_Conf.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/GestioneStampe/Conferimenti/Filtro_Stampe_Conf_jQueryDocReady.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="containerFiltroStampe" class="container">

        <div class="row">
            <div class="col-lg-10"></div>

            <div class="col-lg-2" style="float: right; margin-right: 10px; text-align: right;">
                <asp:ImageButton ID="ImgBtn_Stampa" runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico"></asp:ImageButton>
            </div>
        </div>


        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Label ID="LABEL1" runat="server"><h5>Selezionare il Report che si desidera stampare:</h5></asp:Label>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:RadioButtonList ID="Rbl_Report" runat="server" CssClass="txtUI" AutoPostBack="true">
                    <asp:ListItem Value="220" Selected="True">Riepilogo Conferimenti per Articolo</asp:ListItem>
                    <asp:ListItem Value="184">Estratto Conto Beni Confezionamento</asp:ListItem>
                    <asp:ListItem Value="185">Saldo Imballi</asp:ListItem>
                    <asp:ListItem Value="217">Esportazione Excel Bolle Conferimento</asp:ListItem>
                    <asp:ListItem Value="219">Esportazione Excel Trasportatori</asp:ListItem>
                    <asp:ListItem Value="186">Riepilogo Conferimenti</asp:ListItem>                  
                </asp:RadioButtonList>
                  <!--<asp:ListItem Value="187">Export Tracciabilità Conferimenti</asp:ListItem>-->
            </div>
        </div>
        <div class="row" style="height: 15px"></div>

        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon lbl_required" id="lbl_validita_inizio" for="Txt_ValiditaInizio" runat="server"><i class="fa fa-calendar"></i>Data Inizio </span>

                            <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="form-control datepicker2 required" MaxLength="10"></asp:TextBox>

                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon lbl_required" id="lbl_validita_fine" for="Txt_ValiditaFine" runat="server"><i class="fa fa-calendar"></i>Data Fine</span>

                            <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="form-control datepicker2 required" MaxLength="10"></asp:TextBox>

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

                            <span runat="server" class="input-group-addon lbl_required" id="lbl_DataGiacenza" for="Txt_DataGiacenza"><i class="fa fa-calendar"></i>Data Giacenza </span>

                            <asp:TextBox ID="Txt_DataGiacenza" runat="server" CssClass="form-control datepicker2 required" MaxLength="10"></asp:TextBox>

                        </div>
                    </div>
                </div>
            </div>

        </div>


        <div class="row" id="IdRigaMagazzino">

            <div class="col-lg-12 col-md-12 col-sm-12">

                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span runat="server" class="input-group-addon" id="lbl_Magazzino" for="Cmb_Magazzino">Magazzino
                            </span>
                            <asp:DropDownList ID="Cmb_Magazzino" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>

                </div>
            </div>

        </div>


        <div style="clear: both;"></div>
        <div class="row" style="height: 25px"></div>

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Label ID="lbl_prodotto" runat="server"><h5>Prodotto</h5></asp:Label>
            </div>
        </div>


        <div class="row">
            <div class="col-lg-5 col-md-5 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon" id="lbl_filtro_prodotti_cod" runat="server" for="Txt_Filtro_CodArticolo">Codice</span>

                            <asp:TextBox ID="Txt_Filtro_CodArticolo" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>

                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-5 col-md-5 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon" id="lbl_filtro_prodotti_des" runat="server" for="Txt_Filtro_MatDes">Descrizione</span>

                            <asp:TextBox ID="Txt_Filtro_MatDes" runat="server" CssClass="form-control"></asp:TextBox>

                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-2 col-md-2 col-sm-12">
                <asp:Button ID="Btn_Carica_Prodotti" runat="server" class="btn btn-default dropdown-toggle" Text="Carica"></asp:Button>
            </div>

        </div>



        <div class="row">

            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:DropDownList ID="Cmb_Prodotti" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
            </div>

        </div>

        <div class="row" style="height: 25px"></div>
        <div style="clear: both;"></div>


        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Label ID="lbl_Soggetti" runat="server"><h5>Conferente</h5></asp:Label>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-5 col-md-5 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon" id="lbl_Piva_ConfCli" runat="server" for="Txt_FiltroPiva_Conf">P.IVA</span>

                            <asp:TextBox ID="Txt_FiltroPiva_Conf" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>

                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-5 col-md-5 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon" id="lbl_RagSoc_ConfCli" runat="server" for="Txt_FiltroRagSoc_Conf">Ragione Sociale</span>

                            <asp:TextBox ID="Txt_FiltroRagSoc_Conf" runat="server" CssClass="form-control"></asp:TextBox>

                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-2 col-md-2 col-sm-12">
                <asp:Button ID="Btn_Carica_Conferente" runat="server" class="btn btn-default dropdown-toggle" Text="Carica"></asp:Button>
            </div>

        </div>


        <div class="row">
            <div class="col-lg-12">

                <asp:DropDownList ID="cmb_conferente" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>

            </div>

        </div>

        <!--13/06/2017 -->
        <div class="row" style="height: 25px"></div>
        <div style="clear: both;"></div>


        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Label ID="Lbl_RapCon" runat="server"><h5>Rapporto contabile</h5></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:RadioButtonList ID="Rbl_RapCon" runat="server" CssClass="txtUI" AutoPostBack="false">
                    <asp:ListItem Value="0" Selected="True">Tutti</asp:ListItem>
                    <asp:ListItem Value="-18">Conferente</asp:ListItem>
                    <asp:ListItem Value="-24">Fornitore Ortofrutta</asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>


        <div class="row">
            <div class="col-lg-3 col-md-3 col-sm-12">
                <asp:Label ID="Lbl_DettaglioStabilimento" runat="server"><h5>Dettaglio Destinazione</h5></asp:Label>
            </div>
            <%-- </div>
            <div class="row">--%>
            <div class="col-lg-9 col-md-9 col-sm-12">
                <asp:CheckBox ID="Chk_DettStabilimento" runat="server" AutoPostBack="false"></asp:CheckBox>
            </div>
        </div>

        <!-- INIZIO GESTIONE NUOVA -->

        <div id="separatoreDaCancellare" class="row" style="margin-bottom: 125px;"></div>

        <div id="containerNew" class="form-group panel-body">

            <div class="display-flex containerEstrazioniEsegui">
                <div class="input-group containerElencoEstrazioni">
                    <label class="input-group-addon" id="lblEstrazioni" for="ddlEstrazioni">Selezionare il Report che si desidera stampare:</label>
                    <input type="text" name="ddlEstrazioni" id="ddlEstrazioni" class="form-control" />
                </div>

                <button type="button" class="btn btn-success" id="btnEseguiEstrazione">ESEGUI</button>
            </div>
            <%--<div class="boxDate">
                <div class="display-flex containerFiltriDateInizioFine">
                    <div class="input-group containerDataInizio">
                        <label class="input-group-addon" id="lblDataInizio" for="dpDataInizio">Data Inizio</label>
                        <input type="text" name="dpDataInizio" id="dpDataInizio" class="form-control kendoCalendar" />
                    </div>

                    <div class="input-group containerDataFine">
                        <label class="input-group-addon" id="lblDataFine" for="dpDataFine">Data Fine</label>
                        <input type="text" name="dpDataFine" id="dpDataFine" class="form-control kendoCalendar" />
                    </div>
                </div>

                <div class="input-group containerDataGiacenza">
                    <label class="input-group-addon" id="lblDataGiacenza" for="dpDataGiacenza">Data Giacenza</label>
                    <input type="text" name="dpDataGiacenza" id="dpDataGiacenza" class="form-control kendoCalendar" />
                </div>
            </div>--%>

            <div class="boxDate">
                <div class="row containerFiltriDateInizioFine">
                    <div class="col-sm-4">
                        <div class="input-group containerDataInizio">
                            <label class="input-group-addon" id="lblDataInizio" for="dpDataInizio">Data Inizio</label>
                            <input type="text" name="dpDataInizio" id="dpDataInizio" class="form-control kendoCalendar" />
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group containerDataFine">
                            <label class="input-group-addon" id="lblDataFine" for="dpDataFine">Data Fine</label>
                            <input type="text" name="dpDataFine" id="dpDataFine" class="form-control kendoCalendar" />
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="input-group containerDataGiacenza">
                            <label class="input-group-addon" id="lblDataGiacenza" for="dpDataGiacenza">Data Giacenza</label>
                            <input type="text" name="dpDataGiacenza" id="dpDataGiacenza" class="form-control kendoCalendar" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="boxStampeMassive">
                <div class="row containerDocNumeri">
                    <div class="containerDocNumeriLabel">
                        <label class="myLabelBold" id="lblDocNumeri">Bolle da stampare (comprese nell'intervallo):</label>
                    </div>
                    <div class="col-sm-4 col-sm-3">
                        <div class="input-group">
                            <label class="input-group-addon" for="ddlDocPrefisso">Prefisso</label>
                            <input type="text" name="ddlDocPrefisso" id="ddlDocPrefisso" class="form-control" />
                        </div>
                    </div>
                    <div class="col-xs-5 col-sm-3">
                        <label class="myLabelBold">Da numero</label>
                        <input type="text" min="1" step="1" name="tbDocPrimoNumero" id="tbDocPrimoNumero" class="form-control" />
                    </div>
                    <div class="col-xs-5 col-sm-3">
                        <label class="myLabelBold">A numero</label>
                        <input type="number" min="1" step="1" name="tbDocUltimoNumero" id="tbDocUltimoNumero" class="form-control" />
                    </div>
                    <div class="col-sm-4 col-sm-3">
                        <div class="input-group">
                            <label class="input-group-addon" for="ddlDocSuffisso">Suffisso</label>
                            <input type="text" name="ddlDocSuffisso" id="ddlDocSuffisso" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row rowContainerStampanti">
                    <div class="col-sm-6 col-md-5 col-lg-4">
                        <div class="input-group containerStampanti">
                            <label class="input-group-addon" id="lblStampanti" for="ddlStampanti">Stampante</label>
                            <input type="text" name="ddlStampanti" id="ddlStampanti" class="form-control" />
                        </div>
                    </div>
                    <div class="col-sm-4 col-lg-3">
                        <div class="containerNumeroCopieStampe">
                            <label class="myLabelBold" id="lblNumeroCopie" for="tbNumeroCopieStampe">Numero copie</label>
                            <input type="number" min="1" step="1" name="tbNumeroCopieStampe" id="tbNumeroCopieStampe" class="form-control" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="boxCentriMagazzini">
                <div class="row">
                    <div class="col-md-6">
                        <div class="input-group containerCentroAziendale">
                            <label class="input-group-addon" id="lblCentroAziendale" for="ddlCentriAziendali">Centro Aziendale</label>
                            <input type="text" name="ddlCentriAziendali" id="ddlCentriAziendali" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="input-group containerMagazzino">
                            <label class="input-group-addon" id="lblMagazzini" for="ddlMagazzini">Magazzino</label>
                            <input type="text" name="ddlMagazzini" id="ddlMagazzini" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="input-group containerDettaglioStabilimento">
                            <label class="input-group-addon" id="lblDettaglioStabilimento" for="kSwitchDettaglioStabilimento">Dettaglio Destinazione</label>
                            <input type="checkbox" name="kSwitchDettaglioStabilimento" id="kSwitchDettaglioStabilimento" class="kendoSwitch" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="input-group containerTracciabilitaImpianti">
                            <label class="input-group-addon" id="lblTracciabilitaImpianti" for="kSwitchTracciabilitaImpianti">Visualizza Tracciabilità Impianti</label>
                            <input type="checkbox" name="kSwitchTracciabilitaImpianti" id="kSwitchTracciabilitaImpianti" class="kendoSwitch" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="boxSoggetti">
                <div class="row">
                    <div class="col-md-6">
                        <div class="input-group containerRapportoContabile">
                            <h5 class="titoloRapportoContabile">Rapporto Contabile</h5>
                            <input type="text" name="ddlRapportiContabili" id="ddlRapportiContabili" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <div class="containerConferente">
                            <h5 id="titoloConferenti">Conferenti</h5>
                            <div id="grigliaConferenti"></div>
                        </div>
                    </div>
                </div>

                <div class="boxCessionariProduttore">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="containerPrimoCessionario">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lblPrimoCessionario" for="ddlPrimiCessionari">1° Cessionario (P.Iva / Ragione Sociale)</label>
                                    <input type="text" name="ddlPrimiCessionari" id="ddlPrimiCessionari" class="form-control" />
                                    <%--<label class="input-group-addon">(Inserire 3 caratteri)</label>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="containerSecondoCessionario">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lblSecondoCessionario" for="ddlSecondiCessionari">2° Cessionario (P.Iva / Ragione Sociale)</label>
                                    <input type="text" name="ddlSecondiCessionari" id="ddlSecondiCessionari" class="form-control" />
                                    <%--<label class="input-group-addon">(Inserire 3 caratteri)</label>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="containerProduttore">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lblProduttore" for="ddlProduttori">Produttore (P.Iva / Ragione Sociale)</label>
                                    <input type="text" name="ddlProduttori" id="ddlProduttori" class="form-control" />
                                    <%--<label class="input-group-addon">(Inserire 3 caratteri)</label>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
            </div>

            <div class="boxProdotti">
                <div class="row">
                    <div class="col-md-6">
                        <div class="containerSpecie">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblSpecie" for="ddlSpecie">Specie Vegetale</label>
                                <input type="text" name="ddlSpecie" id="ddlSpecie" class="form-control" />
                            </div>
                        </div>

                    </div>
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="containerVarieta">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblVarieta" for="ddlVarieta">Varietà Vegetale</label>
                                <input type="text" name="ddlVarieta" id="ddlVarieta" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div id="divddlProdotto" class="col-md-6" style="overflow: auto; margin-bottom: 50px;">
                        <div class="containerProdotto">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblProdotto" for="ddlProdotti">Prodotto</label>
                                <input type="text" name="ddlProdotti" id="ddlProdotti" class="form-control" />
                                <%--<label class="input-group-addon">(Inserire 3 caratteri della descrizione)</label>--%>
                            </div>
                        </div>
                    </div>
                </div>

               <div class="row">
                    <div id="btn_filtro_prodotti" class="btn btn-success col-lg-6 col-md-6 col-sm-12"  style="overflow: auto; margin-bottom: 50px;">
                        <i class="fa fa-search"></i>Filtro Prodotti
                    </div>

                    <div id="divgridProdotto" class="col-lg-12 col-md-12 col-sm-12">
                        <div style="overflow: auto; margin-bottom: 70px;">
                            <input type="hidden" id="hdgridProdotto" />
                            <div id="gridProdotto"></div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <script type="text/template" id="tmplConfirmStampaMassiva">
            <div id="confirmStampaMassiva">
                <label class="myLabelBold">Stampante selezionata:</label>
                <input type="text" readonly name="inputConfermaStampante" id="inputConfermaStampante" class="form-control" />

                <label class="myLabelBold">Numero copie impostate:</label>
                <input type="text" readonly name="inputConfermaNumeroCopie" id="inputConfermaNumeroCopie" class="form-control" />

                <label class="myLabelBold">Filtri selezionati per la stampa:</label>
                <textarea readonly name="tAreaConfermaFiltri" id="tAreaConfermaFiltri" class="form-control"></textarea>
            </div>
        </script>
        
        <div class="row" style="margin-bottom: 125px;"></div>


        <!-- Hidden Controls Gestiti Lato Server -->
        <input type="hidden" id="hdPiva" runat="server" />
        <input type="hidden" id="hdSuperUserAccGerarchia" runat="server" />
        <input type="hidden" id="hdIntConfigurazioneModuli" runat="server" />
        <input type="hidden" id="hdAbilitaExportConf" runat="server" />
        <input type="hidden" id="hf_filtroMateriePrimeConferimento" runat="server" /> 

    </div>
</asp:Content>
