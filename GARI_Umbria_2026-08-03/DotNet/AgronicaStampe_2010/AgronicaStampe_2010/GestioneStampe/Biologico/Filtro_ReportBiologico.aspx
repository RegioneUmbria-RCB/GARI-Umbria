<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="Filtro_ReportBiologico.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_ReportBiologico" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color:#D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }
        .buttonClass {
            margin: 0 0 10px 1px;
        }
        .jumbotron {
            margin-bottom: 0px !important;
        }

        @media only screen and (min-width: 997px) {
            .fixed-header {
                top:50px;
                position:fixed;
                width:auto;
                z-index: 1;
            }
        }

        @media only screen and (max-width: 996px) {
            .fixed-header {
                top:100px;
                position:fixed;
                width:auto;
                z-index: 1;
            }
        }
    
        .boxFiltriRicerca {
            margin-top: 25px;
        }

        #searchArea .row {
            margin: 5px 0;
        }

        #searchArea .boxFiltriConfigurazioneStampa {
            margin: 5px 0 15px 0;
        }

        .row.rigaTipoEstrazione {
            margin-bottom: 30px;
        }

        .rigaTipoEstrazione .titoloTipoEstrazione {
            margin-top: 4px;
        }

        .titoloDate {
            margin-bottom: 15px;
        }

        .boxFiltriAvvio .form-control {
            width: 100%;
        }

        #btn_ricerca {
            margin-left: 15px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

     <div id="searchArea" class="panel-group searchArea" style="display:none;">
        <div class="panel-body">
            <div class="boxFiltriAvvio">
                <div class="row">
                    <div class="col-xs-4 col-sm-2">
                        <h5 class="titoloTipoEstrazione"><asp:Localize meta:resourcekey="EstrarreDatiCome" runat="server">Estrarre i dati come</asp:Localize></h5>
                    </div>
                    <div class="col-xs-8 col-sm-3">
                        <div class="btn btn-success buttonClass" id="TipoOutput" >
                            <span><asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Report %>" runat="server">Report</asp:Localize></span>
                            <span><asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Tabella %>" runat="server">Tabella</asp:Localize></span>
                        </div>
                    </div>
                    <div class="col-xs-4 col-sm-2">
                        <label class="lbl_required" id="lblEstrazione" for="ddlEstrazioni">
                            <asp:Localize meta:resourcekey="SelezionaEstrazione" runat="server">Seleziona Estrazione</asp:Localize>
                        </label>
                    </div>
                    <div class="col-xs-5 col-sm-4">
                        <input type="text" name="ddlEstrazioni" id="ddlEstrazioni" class="form-control" />
                    </div>
                </div>
                <div class="btn btn-success buttonClass" id="btn_ricerca">
                    <%--<span class="fa fa-search"></span>--%><asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Esegui %>" runat="server">Esegui</asp:Localize>
                </div>
            </div>
            <div class="boxFiltriRicerca" style="display:none;">
                
                <div class="row boxFiltriStruttureAziendali">
                    <div class="col-sm-12 col-md-8 col-lg-7">
                        <div class="input-group boxImpresa">
                            <label class="input-group-addon lbl_required" id="lblImpresa" for="ddlImprese">
                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Impresa %>" runat="server">Impresa</asp:Localize>
                            </label>
                            <input type="text" name="ddlImprese" id="ddlImprese" class="form-control" />
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-8 col-lg-7">
                        <div class="input-group boxCentroAziendale">
                            <label class="input-group-addon lbl_required" id="lblCentroAziendale" for="ddlCentriAziendali">
                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, CentroAziendale %>" runat="server">Centro Aziendale</asp:Localize>
                            </label>
                            <input type="text" name="ddlCentriAziendali" id="ddlCentriAziendali" class="form-control" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <p class="boxCentroAziendale"><i>Centro Aziendale dal quale prelevare il Codice Operatore Bio, l'Organismo di Controllo e l'Indirizzo per l'intestazione</i></p>
                    </div>
                    <div class="col-sm-12 col-md-8 col-lg-7">
                        <div class="input-group boxMagazzino">
                            <label class="input-group-addon lbl_required" id="lblMagazzini" for="ddlMagazzini">
                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Magazzino %>" runat="server">Magazzino</asp:Localize>
                            </label>
                            <input type="text" name="ddlMagazzini" id="ddlMagazzini" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriDate">
                    <%--<div class="col-sm-12"><h5 class="titoloDate">Intervallo Temporale</h5></div>--%>
                    <div class="col-sm-5 col-md-4">
                        <div class="input-group boxDataInizio">
                            <label class="input-group-addon lbl_required" id="lblDataInizio" for="dpDataInizio">
                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, DataInizio %>" runat="server">Data Inizio</asp:Localize>
                            </label>
                            <input type="text" name="dpDataInizio" id="dpDataInizio" class="form-control kendoCalendar" />
                        </div>
                    </div>
                    <div class="col-sm-5 col-md-4">
                        <div class="input-group boxDataFine">
                            <label class="input-group-addon lbl_required" id="lblDataFine" for="dpDataFine">
                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, DataFine %>" runat="server">Data Fine</asp:Localize>
                            </label>
                            <input type="text" name="dpDataFine" id="dpDataFine" class="form-control kendoCalendar" />
                        </div>
                    </div>
                </div>     
                <div class="row boxFiltriCategorie">
                    <div class="col-lg-10">
                        <!-- Si intendono le categorie prodotto -->
                        <div class="input-group boxCategorieMagazzino">
                            <label class="input-group-addon lbl_required" id="lblCategorieMagazzino" for="msCategorieMagazzino">
                                <asp:Localize meta:resourcekey="FiltroCategorieMagazzino" runat="server">Filtro Categorie Magazzino</asp:Localize>
                            </label>
                            <select name="msCategorieMagazzino" id="msCategorieMagazzino" class="form-control"></select>
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriTipoAppezzamento">
                    <div class="col-sm-6 col-md-5 col-lg-4">
                        <div class="input-group boxTipoAppezzamento">
                            <label class="input-group-addon lbl_required" id="lblTipoAppezzamento" for="msTipoAppezzamento">
                                <asp:Localize meta:resourcekey="FiltroTipoAppezzamento" runat="server">Tipo Appezzamento</asp:Localize>
                            </label>
                            <select name="msTipoAppezzamento" id="msTipoAppezzamento" class="form-control"></select>
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriClasseProdotto">
                    <div class="col-sm-8 col-md-6">
                        <div class="input-group boxClasseProdotto">
                            <label class="input-group-addon" id="lblClasseProdotto" for="ddlClassiProdotto">Filtro Classe Prodotto</label>
                            <input type="text" name="ddlClassiProdotto" id="ddlClassiProdotto" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriModalitaStampa">
                    <div class="col-sm-6 col-md-5 col-lg-4">
                        <div class="input-group boxArrotondamento">
                            <label class="input-group-addon" id="lblArrotondamento" for="ddlArrotondamenti">
                                <asp:Localize meta:resourcekey="TipoArrotondamento" runat="server">Tipo di arrotondamento</asp:Localize>
                            </label>
                            <input type="text" name="ddlArrotondamenti" id="ddlArrotondamenti" class="form-control" />
                        </div>
                    </div>
                    <div class="col-sm-8 col-md-6 col-lg-5">
                        <!-- la colonna lg-5 non è sufficiente per mantenere questo campo, ma essendo l'ultimo della sua colonna non nasconde altri campi
                        a destra quindi la uso per evitare che diventi di una lunghezza eccessiva con schermi maggiori di 1400px -->
                        <div class="input-group boxStampaLotto">
                            <label class="input-group-addon" id="lblStampaLotto" for="ddlStampeLotto">
                                <asp:Localize meta:resourcekey="ModalitaStampa" runat="server">Modalità di stampa del lotto</asp:Localize>
                            </label>
                            <input type="text" name="ddlStampeLotto" id="ddlStampeLotto" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriConfigurazioneStampa">
                    <div class="col-xs-6 col-md-4">
                        <div class="boxCodiceArticolo">
                            <label class="lbl_required" id="lblCodiceArticolo" for="kSwitchCodiceArticolo">
                                <asp:Localize meta:resourcekey="StampaCodiceArticolo" runat="server">Stampa del codice articolo</asp:Localize>
                            </label>
                            <input type="checkbox" name="kSwitchCodiceArticolo" id="kSwitchCodiceArticolo" class="form-control kendoSwitch" />
                        </div>
                    </div>
                    <div class="col-xs-6 col-md-4">
                        <div class="boxConsistenzaVasca">
                            <label class="lbl_required" id="lblConsistenzaVasca" for="kSwitchConsistenzaVasca">
                                <asp:Localize meta:resourcekey="LeggiConsistenzeDiVasca" runat="server">Leggi Consistenze di Vasca</asp:Localize>
                            </label>
                            <input type="checkbox" name="kSwitchConsistenzaVasca" id="kSwitchConsistenzaVasca" class="form-control kendoSwitch" />
                        </div>
                    </div>
                </div>    
                <div class="row boxFiltriDataFirma">
                    <div class="col-xs-6 col-md-4">
                        <div class="boxMostraDataStampa">
                            <label class="lbl_required" id="lblMostraDataStampa" for="kSwitchMostraDataStampa">
                                <asp:Localize meta:resourcekey="MostraDataStampa" runat="server">Mostra Data Stampa</asp:Localize>
                            </label>
                            <input type="checkbox" name="kSwitchMostraDataStampa" id="kSwitchMostraDataStampa" class="form-control kendoSwitch" />
                        </div>
                    </div>
                    <div class="col-xs-6 col-md-4">
                        <div class="boxMostraFirmaODC">
                            <label class="lbl_required" id="lblMostraFirmaODC" for="kSwitchMostraFirmaODC">
                                <asp:Localize meta:resourcekey="MostraFirmaODC" runat="server">Mostra Firma ODC </asp:Localize>
                            </label>
                            <input type="checkbox" name="kSwitchMostraFirmaODC" id="kSwitchMostraFirmaODC" class="form-control kendoSwitch" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriProdottiContatti">
                    <div class="col-md-8">
                        <div class="input-group boxProdotto">
                            <label class="input-group-addon" id="lblProdotto" for="ddlProdotti">
                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Prodotto %>" runat="server">Prodotto</asp:Localize>
                            </label>
                            <input type="text" name="ddlProdotti" id="ddlProdotti" class="form-control" />
                        </div>
                    </div>
                    <div class="col-md-8">
                        <div class="input-group boxContatto">
                            <label class="input-group-addon" id="lblContatto" for="ddlContatti">
                                <asp:Localize meta:resourcekey="FiltroContatti" runat="server">Contatti (P.Iva o Ragione Sociale / CF o Nome)</asp:Localize>
                            </label>
                            <input type="text" name="ddlContatti" id="ddlContatti" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriRegioni boxRegione">
                    <div class="col-xs-6 col-sm-4 col-lg-3">
                        <label class="lbl_required" id="lblRegione" for="kSwitchRegione">
                            <asp:Localize meta:resourcekey="RegioneRiferimento" runat="server">Regione di riferimento</asp:Localize>
                        </label>
                        <input type="checkbox" name="kSwitchRegione" id="kSwitchRegione" class="form-control kendoSwitch" />
                        

                        <%--<div class="input-group boxRegione">
                            <label class="input-group-addon" id="lblRegione" for="ddlRegioni">
                                <asp:Localize meta:resourcekey="RegioneRiferimento" runat="server">Regione di riferimento</asp:Localize>
                            </label>
                            <input type="text" name="ddlRegioni" id="ddlRegioni" class="form-control" />
                        </div>--%>
                    </div>
                    <div class="col-xs-5 col-sm-3 col-lg-2">
                        <div class="input-group"><input type="text" name="ddlRegioni" id="ddlRegioni" class="form-control" /></div>
                    </div>
                    <div class="col-xs-7 col-sm-4 boxLogoRegione">
                        <label class="lbl_required" id="lblLogoRegione" for="kSwitchLogoRegione">
                            <asp:Localize meta:resourcekey="StampaLogoRegione" runat="server">Stampa il logo della regione</asp:Localize>
                        </label>
                        <input type="checkbox" name="kSwitchRegione" id="kSwitchLogoRegione" class="form-control kendoSwitch" />
                    </div>
                </div>
                <div class="row BoxFiltriSemilavorati">
                    <div class="col-sm-10 col-md-9 col-lg-8">
                        <div class="input-group boxSemilavoratoTrasformato">
                            <label class="input-group-addon lbl_required" id="lblSemilavoratoTrasformato" for="ddlSemilavoratiTrasformati">
                                <asp:Localize meta:resourcekey="SemilavoratoTrasformato" runat="server">Semilavorato o Trasformato</asp:Localize>
                            </label>
                            <input type="text" name="ddlSemilavoratiTrasformati" id="ddlSemilavoratiTrasformati" class="form-control" />
                        </div>
                    </div>
                    <div class="col-sm-10 col-md-9 col-lg-8">
                        <div class="input-group boxPreparazioni">
                            <label class="input-group-addon lbl_required" id="lblPreparazione" for="ddlPreparazioni">
                                <asp:Localize meta:resourcekey="LineaProduzionePreparazione" runat="server">Linea di Produzione o Preparazione</asp:Localize>
                            </label>
                            <input type="text" name="ddlPreparazioni" id="ddlPreparazioni" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriStampaSezioni">
                    <div class="col-xs-6 col-md-4">
                        <div class="boxSezioneA">
                            <label class="lbl_required" id="lblSezioneA" for="kSwitchSezioneA">
                                <asp:Localize meta:resourcekey="StampaSezione" runat="server">Stampa Sezione</asp:Localize> A
                            </label>
                            <input type="checkbox" name="kSwitchSezioneA" id="kSwitchSezioneA" class="form-control kendoSwitch" />
                        </div>
                    </div>
                    <div class="col-xs-6 col-md-4">
                        <div class="boxSezioneB">
                            <label class="lbl_required" id="lblSezioneB" for="kSwitchSezioneB">
                                <asp:Localize meta:resourcekey="StampaSezione" runat="server">Stampa Sezione</asp:Localize> B
                            </label>
                            <input type="checkbox" name="kSwitchSezioneB" id="kSwitchSezioneB" class="form-control kendoSwitch" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriTipoMovimento">
                    <div class="col-sm-6 col-md-5 col-lg-4">
                        <div class="input-group boxTipoMovimento">
                            <label class="input-group-addon" id="lblTipoMovimento" for="ddlTipiMovimenti">
                                <asp:Localize meta:resourcekey="TipiMovimenti" runat="server">Tipi Movimenti</asp:Localize>
                            </label>
                            <input type="text" name="ddlTipiMovimenti" id="ddlTipiMovimenti" class="form-control" />
                        </div>
                    </div>
                    <div class="col-sm-8 col-md-6 col-lg-5">
                        <div class="input-group boxOrigineDatiCauScarichi">
                            <label class="input-group-addon" id="lblOrigineDatiCauScarichi" for="ddlOrigineDatiCauScarichi">
                                <asp:Localize meta:resourcekey="EstraiDatiDa" runat="server">Estrai dati da</asp:Localize>:
                            </label>
                            <input type="text" name="ddlOrigineDatiCauScarichi" id="ddlOrigineDatiCauScarichi" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriGiacenzeMagazzino">
                    <div class="col-sm-6 col-md-5 col-lg-4">
                        <div class="boxGiacenzeMagazzino">
                            <label class="lbl_required" id="lblGiacenzeMagazzino" for="kSwitchGiacenzeMagazzino">
                                <asp:Localize meta:resourcekey="VisualizzaGiacenzeMagazzino" runat="server">Visualizza giacenze di magazzino</asp:Localize>
                            </label>
                            <input type="text" name="kSwitchGiacenzeMagazzino" id="kSwitchGiacenzeMagazzino" class="form-control kendoSwitch"/>
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriMultiSelProdotti">
                    <div class="col-md-10">
                        <div class="input-group boxMultiSelProdotti">
                            <label class="input-group-addon lbl_required" id="lblMultiSelProdotti" for="msProdotti">
                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Prodotti %>" runat="server">Prodotti</asp:Localize>
                            </label>
                            <select name="msProdotti" id="msProdotti" class="form-control"></select>
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriLivelloDettaglio">
                    <div class="col-sm-7 col-md-6 col-lg-4">
                        <div class="input-group boxLivelloDettaglio">
                            <label class="input-group-addon lbl_required" id="lblLivelloDettaglio" for="ddlLivelliDettaglio">
                                <asp:Localize meta:resourcekey="LivelloDettaglio" runat="server">Livello Dettaglio</asp:Localize>
                            </label>
                            <input type="text" name="ddlLivelliDettaglio" id="ddlLivelliDettaglio" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="row boxFiltriFornitore">
                    <div class="col-sm-6 col-md-8 col-lg-10">
                        <div class="input-group boxFornitore">
                            <label class="input-group-addon lbl_required" id="lblFornitore" for="msFornitore">
                                <asp:Localize Text="<%$ Resources: AgronicaStampe_2010, Fornitore %>" runat="server">Fornitore</asp:Localize>
                            </label>
                            <select name="msFornitore" id="msFornitore" class="form-control"></select>
                        </div>
                    </div>
                </div>
                <div class="row boxSegnalaBancheDati">
                    <div class="col-sm-5">
                        <p class="boxAssistenza">
                            <!-- L'attributo href della anchor viene valorizzato con un protocollo 'mailto' da javascript -->
                            <a id="anchorAssistenza" href="#">
                                <img id="imgAssistenza" src="../../AB_Immagini/icone32/Posta32.ico"
                                     alt="<asp:Localize meta:resourcekey='InviaSegnalazioneBancheDati' runat='server'></asp:Localize>">
                            </a>
                            <i><asp:Localize meta:resourcekey="SegnalaBancheDati" runat="server">Segnala fitofarmaci/fertilizzanti biologici ma visualizzati come convenzionali nella Scheda Materie Prime Bio</asp:Localize></i>
                        </p>
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Estrazione come Griglia -->
	    <div id="gridArea" class="panel-group gridArea" style="display: none;">
		    <div class="panel-body" style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                <div id="gridEstrazioneBio"></div>
            </div>
	    </div>
       
    </div>
    <!-- fine container -->
  
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdMailTo" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdMailTo = "#<%=hdMailTo.ClientID() %>";
        var cFlagMostraFirmaODC = <%=Mostra_Firma_ODC.ToString.ToLower %>;
        var cFLagDefaultMostraData = <%=Default_Data_Odierna.ToString.ToLower %>;
        var cReportSelezionato = <%=Report_Selezionato.ToString %>;
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Filtro_ReportBiologico_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Filtro_ReportBiologico_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Filtro_ReportBiologico.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Filtro_ReportBiologico_jQueryDocReady.js") %>"></script>
</asp:Content>
