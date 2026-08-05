

var indirizzohttp = "./SchedaRilieviWS.aspx";



function eseguiRicerca() {

    $('#WaitFrame').show();
    WaitFrame.show();

    if ($("#txt_DataDa").val() == "") {
        $("#txt_DataDa").val("01/01/2014");
        $("#txt_DataA").val("01/01/2015");
    }


    $.ajax({
        type: "POST",
        url: indirizzohttp + "/Ricerca",
        data: "{ DataDa: '" + $("#txt_DataDa").val() + "', DataA: '" + $("#txt_DataA").val() + "' }",
        dataType: "json",
        async: true,
        contentType: "application/json; charset=utf-8",
        error: function (xhr, textStatus, errorThrown) {
            $('#WaitFrame').hide();
            WaitFrame.hide();
            AgroMessaggio(divAgroErroreRosso, "Si è verificato un problema in Ajax", textStatus);
        },
        success: function (msg) {
            $('#WaitFrame').hide();
            WaitFrame.hide();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d.substring(0, 2) == "ok") {

                    var msg_d = msg.d.substring(2, msg.d.length - 1);

                    //$.logThis(msg_d);

                    var dd = jQuery.parseJSON(msg_d);

                    AgroWA_Table_sistemaDati(dd); //aggiusto i dati in base al tipo
                    $('#tabellaEsitoRicerca').html('');
                    waTablePrecedenti = $('#tabellaEsitoRicerca').WATable({
                        pageSize: 3,
                        pageSizes: [10, 15, 20, 30, 40, 50],
                        columnPicker: true,
                        filter: true,
                        preFill: true,
                        types: {
                            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                        }
                    }).data('WATable').setData(dd);
                    InitWaTable("#tabellaEsitoRicerca", waTablePrecedenti);

                }
                else
                    AgroMessaggio(divAgroErroreRosso, "Si è verificato un problema lato server (Errore 500)", msg.d);
            }

        }
    });





}