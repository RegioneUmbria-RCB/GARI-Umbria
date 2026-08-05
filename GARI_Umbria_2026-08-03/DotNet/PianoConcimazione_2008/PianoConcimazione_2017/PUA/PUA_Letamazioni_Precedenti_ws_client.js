
function kendo_LetamazioniPrecedenti_Leggi() {

    var regolamento_cod = parseInt($(cIdRegCod).val());
    var pua_cod = parseInt($(cIdPuaCod).val());
    var piva = $(cIdPiva).val();
    var sa_cod = parseInt($(cIdSaCod).val());
    var appezza = parseInt($(cIdAppezza).val());
    var id_reg = parseInt($(cIdIdReg).val());
    var progetto_cod = parseInt($(cIdProgettoCod).val());
    var id = parseInt($(cIdAnagrafeVincoli).val());

    ajaxAgronicaSync("PUA_Letamazioni_Precedenti.aspx/LeggiLetamazioniPrecedenti", JSON.stringify({ regolamento_cod: regolamento_cod, pua_cod: pua_cod, id: id,piva: piva, sa_cod: sa_cod, appezza: appezza, id_reg: id_reg, progetto_cod: progetto_cod }), false,
        function (risposta) {
            if (risposta.RispostaOK) {
                $("#" + id_HD_LetamazioniPrecedenti).val(risposta.RispostaStringa);
            }
            else {
                alert(risposta.Errore);
            }
        }, null);

}
function kendo_LetamazioniPrecedentiQdC_Leggi() {

    var regolamento_cod = parseInt($(cIdRegCod).val());
    var pua_cod = parseInt($(cIdPuaCod).val());
    var piva = $(cIdPiva).val();
    var sa_cod = parseInt($(cIdSaCod).val());
    var appezza = parseInt($(cIdAppezza).val());
    var id_reg = parseInt($(cIdIdReg).val());
    var progetto_cod = parseInt($(cIdProgettoCod).val());
    var id = parseInt($(cIdAnagrafeVincoli).val());

    var data_inizio = $(cIdDataInizio).val();
    var data_fine = $(cIdDataFine).val();
   
    ajaxAgronicaSync("PUA_Letamazioni_Precedenti.aspx/LeggiLetamazioniPrecedentiQdC", JSON.stringify({ regolamento_cod: regolamento_cod, pua_cod: pua_cod, id: id, piva: piva, sa_cod: sa_cod, appezza: appezza, id_reg: id_reg, progetto_cod: progetto_cod,data_inizio:data_inizio,data_fine:data_fine }), false,
        function (risposta) {
            if (risposta.RispostaOK) {
                $("#" + id_HD_LetamazioniPrecedentiQdC).val(risposta.RispostaStringa);
            }
            else {
                alert(risposta.Errore);
            }
        }, null);

}



