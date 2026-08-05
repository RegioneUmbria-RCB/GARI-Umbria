/**
*  Creazione 2017-04-20
*  solo WS
*/



function TestSuperficieCfrGis() {

    let s1 = $(txtSuperficieJQuerySelector).val();
    let msgOutSel = "";

    if (s1 === "") {
        s1 = $("#LblSuperficie_Con_Catasto").html();
        msgOutSel = "#msgCfrGisSuperficieConCatasto";
    } else {
        msgOutSel = "#msgCfrGisSuperficieSenzaCatasto";
    }

    if (s1 === "" || s1 === "0") {
        return;
    }


    var rifArr = $("#LblRiferimenti").html().split(" ");

    var oToTestGisEntita = {
        Piva: rifArr[0],
        Sa_Cod: rifArr[1],
        Appezza: rifArr[3],
        Id_Imp: 0,
        TipoEntita_Cod: 1
    }

    $(msgOutSel).html("");

    ajaxAgronica("../GIS/GIS.aspx/TestSuperficieCfrGis", JSON.stringify({ ToTestGisEntita: oToTestGisEntita, SuperficieDaTestare: s1.replace(",", ".") }),
        function (risposta) {
            if (risposta.RispostaStringa !== "") {
                $(msgOutSel).html(risposta.RispostaStringa);
            }
        }, null);

}


function leggiParticelle() {
    WaitFrame.show();
    var Campo = $('#hidden_campo_cod').val();
    var paramPart = {
        Campo_Cod: Number(Campo)
        , Appezza: Number($('#appezza').val())
        , flag_Macrousi: ($("#Chk_Macrousi").is(':checked'))
        , flag_Utilizzi: ($("#Chk_Utilizzi").is(':checked'))
        , flag_Varieta: ($("#Chk_Varieta").is(':checked'))
        , DataValiditaInizio: $('#TxtValiditaInizio').val()
        , DataValiditaFine: $('#TxtValiditaFine').val()
        , flag_Modifica: client_operazione
    };

    if (paramPart.DataValiditaFine === "") {
        paramPart.DataValiditaFine = '31/12/2100'
    }
    if (paramPart.DataValiditaInizio === "") {
        paramPart.DataValiditaInizio = '01/01/1900'
    }

    ajaxAgronicaSync("Appezzamento_Edit.aspx/CaricaKendo_Particelle", "{ paramPart: '" + JSON.stringify(paramPart) + "' }", true,
        function (risposta) {
            if (risposta.RispostaOK) {
                jSonParsed_Kendo_Particelle = JSON.parse(risposta.RispostaStringa);
                GrigliaKendoParticelle("kendo_Particelle");
                kendoRefresh("#kendo_Particelle");
                WaitFrame.hide();
            }
            else {
                WaitFrame.hide();
                alert(risposta.Errore);
            }
        }, null);

    //kendoRefresh("#kendo_Particelle");
}



function EliminaUtilizzo(obj) {
    var Id_Cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Appezzamento_Edit.aspx/EliminaUtilizzo',
        data: "{Id_Cod:'" + Id_Cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true)
                AggiornaTabUtilizzo(JSON.parse(r.d.RispostaStringa));
            else {
                alert(r.d.Errore);
            }
        }
    });
}

function EliminaCodice(obj) {
    var Id_Cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Appezzamento_Edit.aspx/EliminaCodice',
        data: "{Id_Cod:'" + Id_Cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true)
                AggiornaTabCodici(JSON.parse(r.d.RispostaStringa));
            else {
                alert(r.d.Errore);
            }
        }
    });
}

function AggiornaIndirizzi() {
    //WaitFrame.show();
    let kendoStr = JSON.stringify($("#kendoIndirizzi").data("kendoGrid").dataSource.data());
    var parametri = kendo.stringify({ "kendoGrid": kendoStr });

    ajaxAgronica("Appezzamento_Edit.aspx/AggiornaIndirizzi",
        parametri,
        function (risposta) {
            //$("#kendoIndirizzi").data("kendoGrid").destroy();
            //$("#kendoIndirizzi").html("");
            caricaIndirizzi(JSON.parse(risposta.RispostaStringa));
            WaitFrame.hide();
        }, null, null, false);
}

function WS_controllaMovimenti_CdG_xModificaValidita() {

    var rifArr = $("#LblRiferimenti").html().split(" ");
    let piva, sa_cod, appezza

    piva = rifArr[0]
    sa_cod = rifArr[1]
    appezza = rifArr[3]

    var risp = null
    var parametri = kendo.stringify({
        "piva": piva,
        "sa_cod": sa_cod,
        "appezza": appezza,
        "validita_inizio": $('#TxtValiditaInizio').val(),
        "validita_fine": $('#TxtValiditaFine').val()
    });

    ajaxAgronicaSync("Appezzamento_Edit.aspx/controllaMovimenti_CdG_xModificaValidita",
        parametri, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        }, null);

    return risp
}
