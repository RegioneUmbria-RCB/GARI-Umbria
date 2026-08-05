var waTablePrecedenti;

//funzione che mostra la tabella
function eseguiRicerca() {

    var selezAnno= $('#' + anno).val();
    var selezPiva= $('#' + azienda).val();
    var selezCulCod= $('#' + varieta).val();
    var selezCodTecnico= $('#' + tecnico).val();

    var lTipoExport = TipoExport();
    var listaParametri = "{ TipoExport: '" + lTipoExport + "', anno: '" + selezAnno + "', piva: '" + selezPiva + "', cul_Cod: '" + selezCulCod + "', cod_Tecnico: '" + selezCodTecnico + "' }"

    //var url = "TabaccoStatistiche.aspx";
    var url = "../PannelloDiControllo/PannelloDiControllo_ScriptService.asmx";

    ajaxAgronica("TabaccoStatistiche.aspx/LeggiEsitoRicerca", listaParametri,
                function (risposta) {
                    var dd = jQuery.parseJSON(risposta.RispostaStringa);
                    AgroWA_Table_sistemaDati(dd); //aggiusto i dati in base al tipo
                    $('#tabellaEsitoRicerca').html('');
                    waTablePrecedenti = $('#tabellaEsitoRicerca').WATable({
                        pageSize: 10,
                        pageSizes: [5, 10, 20, 30, 40, 50],
                        columnPicker: true,
                        filter: true,
                        preFill: true,
                        sortEmptyLast: false,
                        tableCreated: function (data) {
                            $("#tabellaEsitoRicerca .export_report").remove();
                            $("#tabellaEsitoRicerca").prepend('<div id="exportRep" class="btn btn-info export_report" style="display: none; float: right; margin-left: 10px; margin-top: 10px;"><i class="fa fa-file-pdf-o"></i> Esporta Report</div>');

                            $("#exportRep").click(function () {
                                EsportaSuReport();
                            });

                            //se ho selezionato un report mostro il pulsante di esportazione altrimenti no
                            if (TipoExport() > 4) 
                                $(".export_report").show();
                            else
                                $(".export_report").hide();

                        },
                        pageChanged: function (data) {

                        },
                        types: {
                            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                        }
                    }).data('WATable').setData(dd);
                    //Inizializzo il WATABLE
                    InitWaTableExport("#tabellaEsitoRicerca", waTablePrecedenti, risposta.opzioniWatable.nomeVarDtInSession, risposta.opzioniWatable.PrefissoNomeFileExport, url, objP_server);
                }, null);

}

function EsportaSuReport() {
    var lTipoExport = TipoExport();

    ajaxAgronica("TabaccoStatistiche.aspx/EsportaReport",
                "{ TipoExport: '" + lTipoExport + "' }",
                function (risposta) {
                    window.open(risposta.RispostaStringa);
                }, null);
}