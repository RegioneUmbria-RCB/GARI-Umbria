<%@ Page Title="" Language="vb" AutoEventWireup="false" 
    MasterPageFile="~/Master/AgendaBootstrap_CtrlPnl.Master" 
    CodeBehind="PannelloDiControllo_Scadenze.aspx.vb" 
    Inherits="AgroAgenda_2010.PannelloDiControllo_Scadenze" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_CtrlPnl.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_CtrlPnl" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_CtrlPnl" runat="server">
    <div class="panel-group" id="Pnl_PreFiltri">
        <div class="row">
            <div class="col-lg-12" style="margin-left: 20px;">
                <h3>Scadenze</h3>
            </div>
        </div>
        <div class="panel panel-primary">
            <div id="Pnl_PreFiltri_Testata" class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#Pnl_PreFiltri" href="#Pnl_PreFiltri_Contenuto">
                        <i class="fa fa-plus-circle"></i> Filtri di ricerca</a>
                </h4>
            </div>
            <div id="Pnl_PreFiltri_Contenuto" class="panel-collapse collapse">
                <div class="panel-body">
                </div>
            </div>
        </div>
    </div>

    <%--SEZIONE CON I PULSANTI DI RICERCA ED ESPORTAZIONE--%>
    <div style="margin-top: 10px;">
        <button type="button" class="btn btn-success" id="btn_ricerca" onclick=" eseguiRicerca();">
            <span class="fa fa-search"></span> Ricerca
        </button>
        <button type="button" class="btn btn-default" id="btn_aggiorna" onclick="AggiornaListaScadenze();">
            <span class="fa fa-refresh"></span> Aggiorna lista Scadenze
        </button>
    </div>

    <%--SEZIONE DOVE SCRIVERE I MESSAGGI D'ERRORE--%>
    <div id="DIV_Messaggi" style="margin-top:10px;"></div>

    <%--WATABLE CON LE NC DELLE NC OPTA--%>
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 125px;">
        <div id="tabella_scadenze"></div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript_CtrlPnl" runat="server">
    <script type="text/javascript">
        var url = "http://localhost/AgronicaCoreWS/Esportazioni/EsportazioniExcel.asmx";

        $(document).ready(function () {

            //MODIFICA DELL'ELEMENTO
            $("body").on("click", ".edit_elem", function () {

                var chiave = $(this).attr('chiave');
                ajaxAgronica("PannelloDiControllo_ScriptService_Scadenze.asmx/Edit_Scadenza",
                        "{chiave: '" + chiave + "' }", 
                        function (risposta) {
                            window.open('PannelloDiControllo_CreaModificaScadenza.aspx', '_blank', 'resizable=yes');
                        }, null);
            });

            //CANCELLAZIONE DELL'ELEMENTO
            $("body").on("click", ".del_elem", function () {

                var chiave = $(this).attr('chiave');
                ajaxAgronica("PannelloDiControllo_ScriptService_Scadenze.asmx/Del_Scadenza",
                        "{chiave: '" + chiave + "' }",
                        function (risposta) {
                            //MESSAGGIOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
                        }, null);
            });

        });        


        //AGGIORNA L'ELENCO DELLE SCADENZE
        function AggiornaListaScadenze() {

            ajaxAgronica("PannelloDiControllo_ScriptService_Scadenze.asmx/AggiornaListaScadenze", null,
                        function (risposta) {
                            if (risposta.RispostaOK)
                                eseguiRicerca();
                        }, null);
        }

        //FUNZIONE CHE MOSTRA LA TABELLA
        function eseguiRicerca() {

            ajaxAgronica("PannelloDiControllo_ScriptService_Scadenze.asmx/LeggiScadenze", null,
                function (risposta) {
                    var dd = jQuery.parseJSON(risposta.RispostaStringa);
                    AgroWA_Table_sistemaDati(dd); //aggiusto i dati in base al tipo
                    $('#tabella_scadenze').html('');
                    waTablePrecedenti = $('#tabella_scadenze').WATable({
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
                    InitWaTableExport("#tabella_scadenze", waTablePrecedenti, risposta.opzioniWatable.nomeVarDtInSession, risposta.opzioniWatable.PrefissoNomeFileExport, url, objP_server);

                }, null);
            }


    </script>


</asp:Content>
