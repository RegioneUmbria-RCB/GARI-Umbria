

function getDataToPrint(tiporeport) {

    var rval="";
    var data = waTable.getData(true, false);
    

    for (var i = 0; i < data.rows.length; i++) {

        $.logThis(data.rows[i].Sel);
        $.logThis(data.rows[i].Numero_di_Etichette_Pedana);
        if (tiporeport == 1) {
            rval = rval + data.rows[i].Sel + "," + data.rows[i].Numero_di_Etichette_Pedana + '|';
        }
        else {
            rval = rval + data.rows[i].Sel + "," + data.rows[i].Numero_di_Etichette_Imballi + '|';
        }
    }

    $.logThis(rval);
    $("#" + hDataToPrint_clientID).val(rval);
    $("#" + hTipoReport_clientID).val(tiporeport);
    
}


//per chiamata Standard Ajax

function Cerca(id_agenda, OModuli_Referenze_Config_Testata, lav_cod) {

    ajaxAgronicaSync("./AnteprimaEtichette.aspx/Filtra", JSON.stringify({ id_agenda: id_agenda, OModuli_Referenze_Config_Testata: OModuli_Referenze_Config_Testata, lav_cod: lav_cod }), false,
        function (risposta) {

            var dd = jQuery.parseJSON(risposta.RispostaStringa);
            $('#tab_lotti').html('');

            waTable = $('#tab_lotti').WATable({
                pageSize: 100,
                pageSizes: [],
                filter: false,
                preFill: true,
                sorting: false,
                checkboxes: true,
                tableCreated: function (data) {

                    if (isFirstCreated) {

                        impostaTabella('tab_lotti');
                        $(".selectpicker").selectpicker("refresh");

                        if (!isDebug) {
                            NascondiColonne();
                        }
                    }
                    else {
                        $(".selectpicker").selectpicker("refresh");
                    }


                }
            }).data('WATable').setData(dd);

            //clickAll('#tab_lotti');
        }, null);


        
        if (lav_cod == "5001")
            RipristinaConfigurazioneDaDettagli_Cod();
        else
            RiportaNumeroDiCopie();



}



 function NascondiColonne() {
    nascondiColonnaTabella("#tab_lotti", "8");
    nascondiColonnaTabella("#tab_lotti", "9");
    nascondiColonnaTabella("#tab_lotti", "10");
    nascondiColonnaTabella("#tab_lotti", "11");
}