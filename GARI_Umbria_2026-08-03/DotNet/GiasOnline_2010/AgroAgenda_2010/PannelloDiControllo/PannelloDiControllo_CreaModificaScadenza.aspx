<%@ Page Title="" Language="vb" AutoEventWireup="false" 
    MasterPageFile="~/Master/AgendaBootstrap.master" 
    CodeBehind="PannelloDiControllo_CreaModificaScadenza.aspx.vb" 
    Inherits="AgroAgenda_2010.PannelloDiControllo_CreaModificaScadenza" %>

<%@ Register TagPrefix="al" TagName="AllegatoDettaglio" Src="~/PannelloDiControllo/Allegato_Dettaglio.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--SEZIONE CON I CAMPI NASCOSTI-->
    <input type="hidden" id="hfID_Categoria" runat="server" />
    <input type="hidden" id="hfID_Elem" runat="server" />

    <div id="pnlAreaComune">
        <div class="row">
            <div class="col-lg-12" style="margin-left: 20px;">
                <h3>Modifica Scadenza</h3>
            </div>
        </div>
        <div>
            <div class="row">
                <div class="col-md-6">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_ID" for="txbID">ID</label>
                        <input type="text" id="txbID" class="form-control" aria-describedby="CTRL_ID" readonly="readonly" />
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Gravita" for="ddlGravita">
                            <span class="fa fa-exclamation-circle"> Gravità</span>
                        </label>
                        <select id="ddlGravita" class="form-control selectpicker" aria-describedby="CTRL_Gravita">
                        </select>
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top:10px;">
                <div class="col-md-6">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Area" for="ddlArea">Area</label>
                        <select id="ddlArea" class="form-control selectpicker" aria-describedby="CTRL_Area"
                            onchange="ddlArea_Change(this,null);">
                        </select>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Tipologia" for="ddlTipologia">Tipologia</label>
                        <select id="ddlTipologia" data-live-search="true" class="form-control selectpicker"
                            aria-describedby="CTRL_Tipologia">
                        </select>
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 10px;">
                <div class="col-md-6">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Azienda" for="ddlAzienda">Azienda</label>
                        <select id="ddlAzienda" data-live-search="true" class="form-control selectpicker"
                            aria-describedby="CTRL_Azienda">
                        </select>
                    </div>
                </div>
            </div>

            <div class="row" style="margin-top:10px;">
                <div class="col-md-6">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Data" for="txbData">
                            <span class="fa fa-calendar"> Data Scadenza</span>
                        </label>
                        <input type="text" id="txbData" class="form-control datepicker" aria-describedby="CTRL_Data" min="1900-01-01" max="2100-12-31" required/>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Utente" for="ddlUtente">
                            <span class="fa fa-user"> Utente Creazione</span>
                        </label>
                        <select id="ddlUtente" data-live-search="true" aria-describedby="CTRL_Utente" class="form-control selectpicker"  readonly="readonly" required></select>
                    </div>
                </div>
            </div>

            <div class="row" style="margin-top:10px;">
                <div class="col-md-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Descrizione" for="txbDescrizione">Descrizione</label>
                        <input type="text" id="txbDescrizione" class="form-control" aria-describedby="CTRL_Descrizione" />                
                    </div>
                </div>
            </div>

            <div class="row" style="margin-top:10px;">
                <div class="col-md-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="CTRL_Note" for="txbNote">Note</label>
                        <textarea id="txbNote" class="form-control" aria-describedby="CTRL_Note" rows="2" cols="20"></textarea>
                    </div>
                </div>
            </div>

            <div class="row" style="margin-top:10px;">
                <div class="col-md-12">
                    <div class="input-group dropdown">
                        <div class="input-group-btn" id="CTRL_Allegati">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                                <span class="fa fa-paperclip"></span> Allegati<span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu" role="menu">
                                <li><a href="#Modal_AllDet" data-toggle="modal" pnlCtrl-alldetIDAll="-1" pnlCtrl-alldetIDLista="-1" pnlCtrl-alldetIDDet="<%=hfID_Elem.Value %>" class="apri-AllegatoDettaglio">Crea nuovo</a></li>
                                <li><a href="#Modal_AllDet" data-toggle="modal" pnlCtrl-alldetIDAll="-1" pnlCtrl-alldetIDLista="-1" pnlCtrl-alldetIDDet="<%=hfID_Elem.Value %>" class="apri-AllegatoDettaglio">Modifica</a></li>
                                <li><a href="#">Elimina</a></li>
                            </ul>
                        </div>
                        <select id="ddlAllegati" aria-describedby="CTRL_Allegati" class="form-control selectpicker" onchange="PnlDet_ddlAllegati_Change(this, 'hfID_ListaAllegati')"></select>
                    </div>
                </div>
            </div>

        </div>
    </div>
    <asp:Panel ID="PnlDettagli" runat="server"></asp:Panel>

    <!--SEZIONE CON I PULSANTI DI SALVATAGGIO-->
    <div style="margin-top: 10px;">
        <button class="btn btn-success" id="btnSalva">
            <span class="fa fa-floppy-o"> Salva</span>
        </button>
    </div>

    <!--FINESTRA MODALE PER LA VISUALIZZAZIONE/MODIFICA/CREAZIONE DEGLI ALLEGATI-->
    <al:AllegatoDettaglio id="allDet" runat="server"></al:AllegatoDettaglio>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PannelloDiControllo_CreaModificaScadenza_Globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PannelloDiControllo_CreaModificaScadenza_jQueryDocReady.js") %>"></script>
    <script type="text/javascript">

        //ASSEGNO LE VARIABILI GLOBALI
        hfID_Elem_ClientID = '<%=hfID_Elem.ClientID() %>';
        hfID_Categoria_ClientID = '<%=hfID_Categoria.ClientID() %>';

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
        function caricaDatiScadenza(ID_Elem) {

            ajaxAgronicaSync("PannelloDiControllo_ScriptService_Scadenze.asmx/LeggiScadenza",
                        "{ID_Elem: '" + ID_Elem + "'}", false,
                        function (risposta) {

                            var elem = risposta.RispostaStringa;
                            
                            $('#txbID').val(elem.ID);

                            $('#ddlGravita').val(elem.Gravita);
                            $('#ddlGravita').selectpicker('refresh')

                            $('#' + hfID_Categoria_ClientID).val(elem.ID_Categoria);

                            if (elem.Piva != 0 && elem.Piva != null) {
                                $('#ddlAzienda').val(elem.Piva);
                                $('#ddlAzienda').selectpicker('refresh');
                            }

                            $('#ddlStato').val(elem.ID_Stato);
                            $('#ddlStato').selectpicker('refresh');

                            //Data
                            var dataDaMs = new Date(parseInt((elem.DataScadenza).substr(6)));
                            var yyyy = dataDaMs.getFullYear().toString();
                            var mm = (dataDaMs.getMonth() + 1).toString(); // getMonth() is zero-based
                            var dd = dataDaMs.getDate().toString();
                            var dataYYYYMMDD = yyyy + '-' + (mm[1] ? mm : "0" + mm[0]) + '-' + (dd[1] ? dd : "0" + dd[0]); // padding
                            var dataDDMMYYYY = (dd[1] ? dd : "0" + dd[0]) + '/' + (mm[1] ? mm : "0" + mm[0]) + '/' + yyyy; // padding
                            $('#txbData').val(dataDDMMYYYY);

                            $('#ddlUtente').val(elem.Utente);
                            $('#ddlUtente').selectpicker('refresh');
                            $('#txbDescrizione').val(elem.Descrizione);
                            $('#txbNote').val(elem.Note);

                            $('#hfID_ListaAllegati').val(elem.ID_ListaAllegati);

                            if (elem.ID_ListaAllegati != "") //se esistono degli allegati
                            {
                                ajaxAgronicaSync("PannelloDiControllo_ScriptService_Allegati.asmx/LeggiAllegatiDaLista",
                                    "{ID_Lista: '" + elem.ID_ListaAllegati + "'}", false,
                                    function (risposta) {
                                        var lista = risposta.RispostaStringa;
                                        popolaCombo('#ddlAllegati', lista, null);
                                    }, null);
                            }

                        }, null);


        }

        //FUNZIONE PER CARICARE I DATI DELLE CATEGORIE DELLE NC PRENDENDO L'ID DELLE CAT
        function CategorieScadenza_Load() {

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

            var rval;
            $(elemDaFiltrare).each(function () {
                rval = rval + '"' + EstraiNomeCtrlDaID($(this).attr("id")) + '": "' + $(this).val() + '", ';
            });
            rval = "{ " + rval.substring(0, rval.length - 2) + " }";

            ajaxAgronica("PannelloDiControllo_ScriptService_Scadenze.asmx/SalvaScadenza",
                "{controlli: '" + rval + "'}",
                function (risposta) {
                    var msg = risposta.RispostaStringa;

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