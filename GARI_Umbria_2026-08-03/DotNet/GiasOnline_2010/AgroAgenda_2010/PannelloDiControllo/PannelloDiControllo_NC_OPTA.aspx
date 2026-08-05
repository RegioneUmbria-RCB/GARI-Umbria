<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap_CtrlPnl.Master"
    CodeBehind="PannelloDiControllo_NC_OPTA.aspx.vb" Inherits="AgroAgenda_2010.PannelloDiControllo_NC_OPTA" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_CtrlPnl.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_CtrlPnl" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_CtrlPnl" runat="server">
    
    <div class="panel-group" id="Pnl_PreFiltri">
        <div class="row">
            <div class="col-lg-12" style="margin-left: 20px;">
                <h3>Non Conformità - Audit tti</h3>
            </div>
        </div>
        <div class="panel panel-primary">
            <div id="Pnl_PreFiltri_Testata" class="panel-heading" style="height: 30px; margin-left: 15px;">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#Pnl_PreFiltri" href="#Pnl_PreFiltri_Contenuto">
                        <i class="fa fa-plus-circle"></i> Filtri di ricerca</a>
                </h4>
            </div>
            <div id="Pnl_PreFiltri_Contenuto" class="panel-collapse collapse in">
                <div class="panel-body">
                    <%--CONTROLLI CON I FILTRI - RIGA 1--%>
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label id="CTRL_Recente">
                                            <input id="CkbRecente" type="checkbox" aria-label="..." style="float: left; margin: 2px 5px 0 0;">
                                            Visualizza ultimo stato della NC
                                        </label>
                                    
                                </div>
                           </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="CTRL_DataInizio"><span class="fa fa-calendar"></span>Data
                                            inizio </label>
                                        <%--type="datetime-local"--%>
                                        <input id="TxbDataInzio" type="date" aria-describedby="CTRL_DataInizio" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="CTRL_DataFine"><span class="fa fa-calendar"></span>Data
                                            fine</label>
                                        <input id="TxbDataFine" type="date" aria-describedby="CTRL_DataFine" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%--CONTROLLI CON I FILTRI - RIGA 2--%>
                    <div class="row">
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="CTRL_OP">OP</label>
                                        <asp:DropDownList ID="ddlListaOP" data-live-search="true" aria-describedby="CTRL_OP"
                                            data-container="body" CssClass="form-control selectpicker" runat="server">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="CTRL_Azienda">Azienda</label>
                                        <asp:DropDownList ID="ddlListaAziende" data-live-search="true"
                                            aria-describedby="CTRL_Azeinda" data-container="body" CssClass="form-control selectpicker"
                                            runat="server">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="CTRL_Centro">Centro</label>
                                        <asp:DropDownList ID="ddlListaCentro" data-live-search="true"
                                            aria-describedby="CTRL_Centro" data-container="body" CssClass="form-control selectpicker"
                                            runat="server">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%--CONTROLLI CON I FILTRI - RIGA 3--%>
                    <div class="row">
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="CTRL_Tec">Tecnico Rilevatore</label>
                                        <asp:DropDownList ID="ddlListaTec" data-live-search="true" aria-describedby="CTRL_Tec"
                                            data-container="body" CssClass="form-control selectpicker" runat="server">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="input-group">
                                <label class="input-group-addon" id="CTRL_Stato">Stato Rilievo</label>
                                <asp:DropDownList ID="ddlListaStato" data-live-search="true"
                                    aria-describedby="CTRL_Stato" data-container="body" CssClass="form-control selectpicker"
                                    runat="server">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="input-group">
                                <label class="input-group-addon" id="CTRL_OggRil">Oggetto Rilevato</label>
                                <asp:DropDownList ID="ddlListaOggRil" data-live-search="true"
                                    aria-describedby="CTRL_OggRil" data-container="body" CssClass="form-control selectpicker"
                                    runat="server">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="panel-footer">              
                    
                    <button type="button" class="btn btn-success" id="btn_ricerca" onclick=" eseguiRicerca();">
                        <span class="fa fa-search"></span> Ricerca
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="row" id="div_azioni_su_watable">
        <div class="col-lg-12 text-right">
            <button type="button" class="btn btn-default" id="btn_aggiorna" onclick="AggiornaListaNC_OPTA();">
                <span class="fa fa-refresh"></span> Aggiorna
            </button>  
        </div>
    </div>

    <%--WATABLE CON LE NC DELLE NC OPTA--%>
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 125px;">
        <div id="tabella_conformita">
        
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript_CtrlPnl" runat="server">
    <script type="text/javascript">
        var url = "http://localhost/AgronicaCoreWS/Esportazioni/EsportazioniExcel.asmx";

        $(document).ready(function () {

            //MODIFICA DELL'ELEMENTO
            $("body").on("click", ".edit_elem", function () {
                var chiave = $(this).attr('chiave');
                var jsonNC = [{ "ID_Dettaglio": 0, "ID_NC": 1, "ID_Categoria": 2, "Piva": "ciao", "Utente": "pivas", "Data": "2012-04-23T18:25:43.511Z", "ID_Stato": 0, "ID_ListaAllegati": 3, "Descrizione": "des", "TabDettaglio_Nome": "fff", "TabDettaglio_Chiave": "ggg", "Note": "not", "ID_Gravita": 6 }, { "ID_Dettaglio": 100, "ID_NC": 10, "ID_Categoria": 20, "Piva": "ciao", "Utente": "pivas", "Data": "2012-04-23T18:25:43.511Z", "ID_Stato": 0, "ID_ListaAllegati": 3, "Descrizione": "des", "TabDettaglio_Nome": "fff", "TabDettaglio_Chiave": "ggg", "Note": "not", "ID_Gravita": 6}];
                var parametri = { 'PnlCtrl_ID_NC': chiave, 'NC': jsonNC };
                //var parametri = { 'PnlCtrl_ID_NC': chiave };
                apriPopup('PannelloDiControllo_CreaModificaItem.aspx', 'post', 'CreaModificaItem', 'resizable=yes', 'parametri', parametri);
//                var chiave = $(this).attr('chiave');
//                ajaxAgronica("PannelloDiControllo_ScriptService_NC.asmx/Edit_NC",
//                        "{chiave: '" + chiave + "' }",
//                        function (risposta) {
//                            window.open('PannelloDiControllo_CreaModificaItem.aspx', '_blank', 'resizable=yes');
//                        }, null);
            });

            //CANCELLAZIONE DELL'ELEMENTO
            $("body").on("click", ".del_elem", function () {
                var chiave = $(this).attr('chiave');
                ajaxAgronica("PannelloDiControllo_ScriptService_NC.asmx/Del_NC",
                        "{chiave: '" + chiave + "' }",
                        function (risposta) {
                            //MESSAGGIOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
                        }, null);
            });

        });        


        //FUNZIONE CHE MOSTRA LA TABELLA
        function eseguiRicerca() {
            var DataDa = $("#TxbDataInizio").val();
            var DataA = $("#TxbDataFine").val();
            var Op = $("#<%= ddlListaOP.ClientID %>").val();
            var Azienda = $("#<%= ddlListaAziende.ClientID %>").val();
            var Centro = $("#<%= ddlListaCentro.ClientID %>").val();
            var Tecnico = $("#<%= ddlListaTec.ClientID %>").val();
            var Stato = $("#<%= ddlListaStato.ClientID %>").val();
            var Oggetto = $("#<%= ddlListaOggRil.ClientID %>").val();
            var PiuRecente = $("#CkbRecente").prop("checked");

            var param = "{DataDa: '" + DataDa + "', DataA: '" + DataA + "', Op: '" + Op + "', Azienda: '" + Azienda + "', Centro: '" + Centro + "', Tecnico: '" + Tecnico + "', Stato: '" + Stato + "', Oggetto: '" + Oggetto + "', PiuRecente: '" + PiuRecente + "' }"

            ajaxAgronica("PannelloDiControllo_ScriptService_NC_OPTA.asmx/LeggiConformitaOpta", null,
                function (risposta) {
                    var dd = jQuery.parseJSON(risposta.RispostaStringa);
                    AgroWA_Table_sistemaDati(dd); //aggiusto i dati in base al tipo
                    $('#tabella_conformita').html('');
                    waTablePrecedenti = $('#tabella_conformita').WATable({
                        pageSize: 10,
                        pageSizes: [10, 20, 30, 40, 50, 'All'],
                        columnPicker: true,
                        filter: true,
                        preFill: true,
                        checkboxes: true,
                        types: {
                            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                        }
                    }).data('WATable').setData(dd);
                    InitWaTableExport("#tabella_conformita", waTablePrecedenti, risposta.opzioniWatable.nomeVarDtInSession, risposta.opzioniWatable.PrefissoNomeFileExport, url, objP_server);

                }, null);
            }

          


        //AGGIORNA L'ELENCO DELLE NC
        function AggiornaListaNC_OPTA() {
            ajaxAgronica("PannelloDiControllo_ScriptService_NC_OPTA.asmx/AggiornaListaNC_OPTA", null,
                        function (risposta) {
                            window.open(r.RispostaStringa);
                        }, null);
        }


        //FUNZIONE PER APRIRE UN POPUP E PASSARE DEI DATI IN POST
        function apriPopup(frmAction, //url the form has to be sended to
            frmMethod, //post||get
            winName, //name used for window and form-target
            winOpts, //options for window.open
            jsonName, //the name of the json inside _POST
            json//the object to send
            ) {
                        //open the window
                        var win = window.open('about:blank', winName, winOpts);
                        win.focus();
                        //create form & input and append it to the body
                        var f = document.createElement('form');
                        f.setAttribute('action', frmAction);
                        f.style.display = 'none';
                        f.setAttribute('target', winName);
                        f.setAttribute('method', frmMethod);
                        var e = document.createElement('input');
                        e.setAttribute('name', jsonName);
                        e.setAttribute('value', JSON.stringify(json));
                        f.appendChild(e);
                        document.body.appendChild(f);
                        //send the form 
                        f.submit();
                        //remove the form after a moment
                        setTimeout(function () { f.parentNode.removeChild(f); }, 1000);
                        return false;
         }




    </script>
</asp:Content>
