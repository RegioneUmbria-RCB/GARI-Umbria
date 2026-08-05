<%@ Page Title="" Language="vb" AutoEventWireup="false" 
    MasterPageFile="~/Master/AgendaBootstrap_CtrlPnl.Master" 
    CodeBehind="PannelloDiControllo_NC_Analisi.aspx.vb" 
    Inherits="AgroAgenda_2010.PannelloDiControllo_NC_Analisi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_CtrlPnl.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_CtrlPnl" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_CtrlPnl" runat="server">

    <div class="panel-group" id="Pnl_PreFiltri">
        <div class="row">
            <div class="col-lg-12" style="margin-left: 20px;">
                <h3>Non Conformità - Analisi</h3>
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
                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" id="CTRL_Analisi">Analisi</label>
                                        <asp:DropDownList ID="ddlElencoPDCAnalisi" data-live-search="true" data-container="body" CssClass="selectpicker" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="panel-footer">              
                    <div class="btn btn-success" id="btn_ricerca">
                        <span class="fa fa-search"></span>  Ricerca
                    </div>
                    
                </div>
            </div>
        </div>
    </div>


    <%--SEZIONE DOVE SCRIVERE I MESSAGGI D'ERRORE--%>
    <div id="DIV_Messaggi" style="margin-top:10px;"></div>

    <div class="row" id="div_azioni_su_watable" style="display:none;">
        <div class="col-lg-12 text-right">
            <div class="btn btn-info export_excel" id="btn_esporta_excel" style="display:inline-block;">
                <i class="fa fa-file-excel-o"> Esporta su Excel</i>
            </div>
        </div>
    </div>

    <%--WATABLE CON LE NC DELLE ANALISI--%>
    <div style="overflow:auto; margin-top:10px; margin-bottom: 125px;">
        <div id="tabella_conformita">
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript_CtrlPnl" runat="server">
    <script type="text/javascript">

        window.onload = function () {
            //SELEZIONE DELLE TAB
//            $('#TabMenu a[id="TnonConformita"]').tab('show');
//            $('#TabMenu_NC a[id="TNC_Analisi"]').tab('show');

            //SPOSTO IL CONTENUTO NEL TAB GIUSTO
//            $('#NC_Analisi').html($('#Content_Da_Spostare').html());
            //            $('#Content_Da_Spostare').html('');


        };

        $(document).ready(function () {
        });

        $("#btn_ricerca").click(function () {
            eseguiRicerca();
        });

        //FUNZIONE CHE MOSTRA LA TABELLA
        function eseguiRicerca() {
            WaitFrame.show();
            var idPdcTestata = $('#' + '<%=ddlElencoPDCAnalisi.ClientID %>').val();

            $.ajax({
                async: true,
                type: 'POST',
                url: 'PannelloDiControllo_NC_Analisi.aspx/LeggiConformitaAnalisi',
                data: "{ID_PDC_Testata: '" + idPdcTestata + "' }",
                //data: "{DataDa: '" + DataDa + "', DataA: '" + DataA + "', Op: '" + Op + "', Azienda: '" + Azienda + "', Centro: '" + Centro + "', Tecnico: '" + Tecnico + "', Stato: '" + Stato + "', Oggetto: '" + Oggetto + "', PiuRecente: '" + PiuRecente + "' }",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (r) {
                    var dd = jQuery.parseJSON(r.d);
                    $('#tabella_conformita').html('');
                    waTable = $('#tabella_conformita').WATable({
                        columnPicker: true,
                        pageSize: 10,
                        pageSizes: [10, 20, 30, 40, 50, 'All'],
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
                    InitWaTable("#tabella_conformita", waTable);

                    $('#div_azioni_su_watable').show();

                    WaitFrame.hide();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    WaitFrame.hide();
                    MessaggioErrore_Bootstrap(xhr.status + "<br />" + thrownError, 'DIV_Messaggi');
                }
            });
        };

    </script>
</asp:Content>
