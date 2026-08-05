<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap_CtrlPnl.Master"
    CodeBehind="PannelloDiControllo_CreaModificaItem.aspx.vb" Inherits="AgroAgenda_2010.PannelloDiControllo_CreaModificaItem" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_CtrlPnl.Master" %>
<%@ Register TagPrefix="al" TagName="AllegatoDettaglio" Src="~/PannelloDiControllo/Allegato_Dettaglio.ascx" %>
<%@ Register TagPrefix="ncDet" TagName="NCDettaglio" Src="~/PannelloDiControllo/NonConformita_OPTA_Dettaglio.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_CtrlPnl" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_CtrlPnl" runat="server">
    <!--SEZIONE CON I CAMPI NASCOSTI-->
    <input type="hidden" id="hfID_NC" runat="server" />
    <input type="hidden" id="hfID_Categoria" runat="server" />
    <input type="hidden" id="hfObjJsonNonConformita" runat="server" />
    
    <div class="container">
        <!--SEZIONE DOVE SCRIVERE I MESSAGGI D'ERRORE-->
        <div id="DIV_Messaggi" style="margin-top:10px;"></div>

        <!--SEZIONE CON I PULSANTI DI SALVATAGGIO-->
        <div class="row">
            <div class="col-lg-12">
                <button type="button" class="btn btn-success pull-right xi-btn-primary" id="btnSalva" style="margin:10px 5px">
                    <span class="fa fa-floppy-o"></span> Salva
                </button>
                <button type="button" class="btn btn-default pull-right" id="btnAggiungi" runat="server" style="margin:10px 5px">
                    <span class="fa fa-plus-circle"></span> Aggiungi
                </button>
            </div>
        </div>

        <div id="pnlAreaComune">
            <!--SEZIONE CON LA PARTE DI NC VALIDA PER TUTTO IL WORKFLOW-->
            <div class="row">
                <div class="col-lg-12">
                    <h4 id="pnlAreaComune_Titolo">AREA COMUNE</h4>
                </div>
            </div>
            <div id="pnlAreaComune_body">
                <div class="row">
                    <div class="col-lg-4 col-md-4 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lbl_txbID_NC" for="txbID_NC">
                                        ID NC</label>
                                    <input type="text" id="txbID_NC" class="form-control" aria-describedby="CTRL_ID"
                                        readonly="readonly" />
                                    </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lbl_ddlAzienda" for="ddlAzienda">
                                        Azienda</label>
                                    <select id="ddlAzienda" data-live-search="true" class="form-control selectpicker"
                                        aria-describedby="CTRL_Azienda" onchange="ddlAzienda_Change(this);">
                                    </select>
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
                                    <label class="input-group-addon" id="lbl_ddlGravita" for="ddlGravita">
                                        <span class="fa fa-exclamation-circle"></span> Gravità
                                    </label>
                                    <select id="ddlGravita" class="form-control selectpicker" aria-describedby="CTRL_Gravita">
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lbl_ddlArea" for="ddlArea">
                                        Area</label>
                                    <select id="ddlArea" class="form-control selectpicker" aria-describedby="CTRL_Area"
                                        onchange="ddlArea_Change(this,null);">
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon" id="lbl_ddlTipologia" for="ddlTipologia">
                                        Tipologia</label>
                                    <select id="ddlTipologia" data-live-search="true" class="form-control selectpicker"
                                        aria-describedby="CTRL_Tipologia">
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <div class="panel-group acc_main" id="accordion" role="tablist" aria-multiselectable="true" style="margin-bottom:125px;">   
                <asp:Panel ID="PnlDettagli" runat="server"></asp:Panel>
        </div>

    </div> <!-- end container -->

    <!--FINESTRA MODALE PER LA VISUALIZZAZIONE/MODIFICA/CREAZIONE DEGLI ALLEGATI-->
    <al:AllegatoDettaglio id="allDet" runat="server"></al:AllegatoDettaglio>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript_CtrlPnl" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PannelloDiControllo_CreaModificaItem_Globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PannelloDiControllo_CreaModificaItem_jQueryDocReady.js") %>"></script>
    <script type="text/javascript">

        //ASSEGNO LE VARIABILI GLOBALI
        hfID_NC_ClientID = '<%=hfID_NC.ClientID() %>'
        hfID_Categoria_ClientID = '<%=hfID_Categoria.ClientID() %>'

        //SE PRESENTI IMPOSTO I DATI PASSATI PER LA NON CONFORMITA' --> DA SPOSTARE PERCHE COSI LO FA OGNI VOLTA
        if ($('#<%=hfObjJsonNonConformita.ClientID()%>').val() != null) {
            var lista = $('#<%=hfObjJsonNonConformita.ClientID()%>').val();
            lista = JSON.parse(lista);
            impostaCampiNCdaOggetto(lista);
        }

        // FUNZIONE PER CARICARE IL TITOLO DELL'AREA COMUNE
        function pnlAreaComune_Titolo_Load() {
            ajaxAgronicaSync("PannelloDiControllo_ScriptService_NC.asmx/LeggiTitoloAreaComune",
                        "{ID_Categoria: '" + $('#' + hfID_Categoria_ClientID).val() + "' ,ID_NC: '" + $('#' + hfID_NC_ClientID).val() + "'}", false,
                        function (risposta) {
                            $('#pnlAreaComune_Titolo').text(risposta.RispostaStringa);
                        }, null);
        }

        // FUNZIONE PER CARICARE LE GRAVITA'
        function ddlGravita_Load(ddlId) {
            ajaxAgronicaSync("PannelloDiControllo_ScriptService_Gravita.asmx/LeggiGravita", null, false,
                        function (risposta) {
                            var lista = risposta.RispostaStringa;
                            popolaCombo('#ddlGravita', lista, null);
                        }, null);
        }

        // FUNZIONE PER CARICARE LE AREE
        function ddlArea_Load() {
            ajaxAgronicaSync("PannelloDiControllo_ScriptService_Categorie.asmx/LeggiAree", null, false,
                        function (risposta) {
                            var lista = risposta.RispostaStringa;
                            popolaCombo('#ddlArea', lista, null);
                        }, null);
        }

        // FUNZIONE PER CARICARE LE AZIENDE
        function ddlAzienda_Load() {
            ajaxAgronicaSync("PannelloDiControllo_ScriptService.asmx/LeggiAziende", null, false,
                        function (risposta) {
                            var lista = risposta.RispostaStringa;
                            popolaCombo('#ddlAzienda', lista, null);
                        }, null);
        }

        // SE VIENE SELEZIONATA UN'AREA
        function ddlArea_Change(ddlId, idCatSelez) {
            ajaxAgronicaSync("PannelloDiControllo_ScriptService_Categorie.asmx/LeggiTipologiePerArea",
                        "{area: '" + $('#' + ddlId.id + ' option:selected').text() + "'}", false,
                        function (risposta) {
                            var lista = risposta.RispostaStringa;
                            popolaCombo('#ddlTipologia', lista, idCatSelez);
                        }, null);
        }

        // FUNZIONE PER CARICARE I DATI
        function caricaDatiNC(ID_NC) {

            ajaxAgronicaSync("PannelloDiControllo_ScriptService_NC.asmx/LeggiNC",
                "{ID_NC: '" + ID_NC + "'}", false,
                function (risposta) {
                    var lista = risposta.RispostaStringa;
                    impostaCampiNCdaOggetto(lista);
                }, null);
        }

        function impostaCampiNCdaOggetto(lista) {
            //carico prima l'area comune
            $('#txbID_NC').val(lista[0].ID_NC);

            $('#ddlGravita').val(lista[0].Gravita);
            $('#ddlGravita').selectpicker('refresh')

            $('#' + hfID_Categoria_ClientID).val(lista[0].ID_Categoria);

            if (lista[0].Piva != 0 && lista[0].Piva != null) {
                $('#ddlAzienda').val(lista[0].Piva);
                $('#ddlAzienda').selectpicker('refresh');
            }

            for (var i = 0; i < lista.length; i++) {
                $('#PnlDet' + (i + 1) + '_ddlStato').val(lista[i].ID_Stato);
                $('#PnlDet' + (i + 1) + '_ddlStato').selectpicker('refresh');

                //Data
                var dataDaMs = new Date(parseInt((lista[i].Data).substr(6)));
                var yyyy = dataDaMs.getFullYear().toString();
                var mm = (dataDaMs.getMonth() + 1).toString(); // getMonth() is zero-based
                var dd = dataDaMs.getDate().toString();
                var dataYYYYMMDD = yyyy + '-' + (mm[1] ? mm : "0" + mm[0]) + '-' + (dd[1] ? dd : "0" + dd[0]); // padding
                var dataDDMMYYYY = (dd[1] ? dd : "0" + dd[0]) + '/' + (mm[1] ? mm : "0" + mm[0]) + '/' + yyyy; // padding
                $('#PnlDet' + (i + 1) + '_txbData').val(dataDDMMYYYY);

                $('#PnlDet' + (i + 1) + '_ddlUtente').val(lista[i].Utente);
                $('#PnlDet' + (i + 1) + '_ddlUtente').selectpicker('refresh');
                $('#PnlDet' + (i + 1) + '_txbDescrizione').val(lista[i].Descrizione);
                $('#PnlDet' + (i + 1) + '_txbNote').val(lista[i].Note);

                //$('#PnlDet' + i + '_hfID_Elem').val(lista[i].ID); è un controllo server quindi così non ci accedo
                $('#PnlDet' + (i + 1) + '_hfID_ListaAllegati').val(lista[i].ID_ListaAllegati);

                if (lista[i].ID_ListaAllegati != "") //se esistono degli allegati
                {
                    ajaxAgronicaSync("PannelloDiControllo_ScriptService_Allegati.asmx/LeggiAllegatiDaLista",
                        "{ID_Lista: '" + lista[i].ID_ListaAllegati + "'}", false,
                        function (risposta) {
                            var lista = risposta.RispostaStringa;
                            popolaCombo('#PnlDet' + (i + 1) + '_ddlAllegati', lista, null);
                        }, null);
                }
            }

        }

        //FUNZIONE PER CARICARE I DATI DELLE CATEGORIE DELLE NC PRENDENDO L'ID DELLE CAT
        function CategorieNC_Load() {

            //leggo la categoria da selezionare
            var idCat = $('#' + hfID_Categoria_ClientID).val();

            //carico le aree e seleziono quella giusta
            ajaxAgronicaSync("PannelloDiControllo_ScriptService_Categorie.asmx/LeggiAree_ConSelezione",
                "{ID_Categoria: '" + idCat + "'}", false,
                function (risposta) {
                    var lista = risposta.RispostaStringa;
                    popolaCombo('#ddlArea', lista, idCat);
                }, null);

            //carico le tipologie e seleziono quella giusta
            ddlArea_Change(ddlArea, idCat);
        }

        function btnSalva_Click() {
            var elemDaFiltrare = "#pnlAreaComune input[id], #pnlAreaComune select[id], #pnlAreaComune textarea[id], "
            elemDaFiltrare += "#<%=PnlDettagli.ClientID %> input[id], #<%=PnlDettagli.ClientID %> select[id], #<%=PnlDettagli.ClientID %> textarea[id]"

            var rval="";
            $(elemDaFiltrare).each(function () {
                rval = rval + '"' + EstraiNomeCtrlDaID($(this).attr("id")) + '": "' + $(this).val() + '", ';
            });
            rval = "{ " + rval.substring(0, rval.length - 2) + " }";

            ajaxAgronica("PannelloDiControllo_ScriptService_NC.asmx/SalvaNC",
                "{controlli: '" + rval + "', nDettagli: " + $("#<%=PnlDettagli.ClientID %> .pannelloDettaglio").length + "}",
                function (risposta) {
                    MessaggioTuttoOK_Bootstrap("Non Conformità salvata correttamente", "Div_Messaggi");
                }, null);
        }

        //Per ottenere l'ID di un controllo
        function EstraiNomeCtrlDaID(IDControllo) {
            var ctrl = IDControllo.substring(IDControllo.indexOf("PnlDet"));
            return ctrl;
        }

        //Per aggiornare il contenuto di una Combo con una lista di listitem
        function popolaCombo(IDControllo, lista, idSelez) {
            //pulisco la combo
            $(IDControllo).find('option').remove();

            //genero il codice HTML
            var strHtml = "";
            for (var i = 0; i < lista.length; i++) {
                strHtml += "<option value='" + lista[i]["Value"] + "'>" + lista[i]["Text"] + "</option>";
            }

            //aggiungo l'HTML al controllo
            $(IDControllo).html(strHtml);
            //seleziono l'eventuale elemento
            if (idSelez != null) {
                $(IDControllo).val(idSelez);
            }
            //aggiorno il plugin
            $(IDControllo).selectpicker('refresh');
        }

    </script>
</asp:Content>
