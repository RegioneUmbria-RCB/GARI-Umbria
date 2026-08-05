
/* ClientJs/precision.js */

var precision = {
    GETvAppIdRateDaStringa: function (AppIdRate) {
        return AppIdRate.split("|");
    }
    ,
//#region Rateo
pfRateo: function (isBootstrap) {
    var ChiaveAlbero = $("#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica").val().toString(); //.replace(/\\/g, '\\\\'); ;
    var noSel = "Nessuna ricetta selezionata.";

    if (ChiaveAlbero == "") {
        alert(noSel);
        return false;
    }

    var sChiaveAlbero = ChiaveAlbero.split(separatoreChiaveAlbero)[0];
    var sRicettaOperazioneCod = ChiaveAlbero.split(separatoreChiaveAlbero)[28];

    if (!(sChiaveAlbero == "a60" || sChiaveAlbero == "60")) {
        alert(noSel);
        return false;
    }

    if (sRicettaOperazioneCod == 0) {
        alert(noSel);
        return false;
    }


    interfaccia.loading(true);

    let lDataRiferimentoSentinel;
    if ($("#wmsToolsNavBar").is(":visible")) {
        lDataRiferimentoSentinel = $("#txtDialogRateo_dataSentinel").val();
    } else {
        lDataRiferimentoSentinel = "01/01/1900";
    }

    let DescrizioneDelPiano = $("#txtDescrizioneDelPiano").val();

    ajaxAgronica(indirizzohttp + "/pfRateoSrv", 
        "{ ChiaveAlbero: '" + ChiaveAlbero + "', DescrizioneDelPiano:'" + DescrizioneDelPiano + "', cellsize: '" + $("#txtCellSize").val() + "', DataRiferimentoPerLetturaDatiSentinel: '" + lDataRiferimentoSentinel + "'  }",
        function (risposta) {

            interfaccia.loading(false);
            if (isBootstrap) {
                ChiudiKendoDialog("#dialogRateo")
            } else {
                $("#dialogRateo").dialog("close");
            }

            alert(risposta.RispostaStringa);

        }, null);
    

}
,
//#end region Rateo

//#region gestione planning da impianti
GeneraDescrizionePlanning: function () {
    //selezione multipla --> ok passo a generazione planning
    var listaImpiantiDaChiaveAlbero = $("#ChiaveAlberoMultiSelezione").val();
    if (listaImpiantiDaChiaveAlbero != "") {
        listaImpiantiDaChiaveAlbero = listaImpiantiDaChiaveAlbero.substring(0, listaImpiantiDaChiaveAlbero.length - 2);
        this.GeneraDescrizionePlanning_final(listaImpiantiDaChiaveAlbero);
        return true;
    }

    var Entita_Cod = "";
    Entita_Cod = $('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString();  //.replace(/\\/g, '\\\\'); //selectedShape.chiavealbero.toString();
    $("#hidCultivarRicetteNodoPartenza").val($('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString());

    if (Entita_Cod != "") {
        this.GeneraDescrizionePlanning_final(Entita_Cod);
        return true;
    }

    alert("nessun elemento selezionato.");
}
,

GeneraDescrizionePlanning_final: function (Entita_Cod) {

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/GeneraDescrizionePlanning",
        data: "{ ChiaveAlbero: '" + Entita_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $("#txtNomePlanning").val(msg.d);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });
}
,
clickGeneraPlanningT: function () {
    clickGeneraPlanning(true);
}
,clickGeneraPlanning:function (isTest) {
    //selezione multipla --> ok passo a generazione planning
    var listaImpiantiDaChiaveAlbero = $("#ChiaveAlberoMultiSelezione").val();
    if (listaImpiantiDaChiaveAlbero != "") {
        listaImpiantiDaChiaveAlbero = listaImpiantiDaChiaveAlbero.substring(0, listaImpiantiDaChiaveAlbero.length - 2);
        if (isTest)
            this.clickGeneraPlanning_Test(listaImpiantiDaChiaveAlbero);
        else
            this.clickGeneraPlanning_Final(listaImpiantiDaChiaveAlbero);
        return true;
    }

    var Entita_Cod = "";
    Entita_Cod = $('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString();  //.replace(/\\/g, '\\\\'); //selectedShape.chiavealbero.toString();
    $("#hidCultivarRicetteNodoPartenza").val($('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString());

    if (Entita_Cod != "") {
        if (isTest)
            this.clickGeneraPlanning_Test(Entita_Cod);
        else
            this.clickGeneraPlanning_Final(Entita_Cod);
        return true;
    }

    alert("nessun elemento selezionato.");
}
,
clickGeneraPlanning_Test: function (Entita_Cod) {

    var AggregaGrafica = "0"
    AggregaGrafica = $("#txtAggregaEntitaGrafichePerPianificazioneScartoM").val();

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/clickVerificaCartografiaAggregatoDaChiaveAlbero",
        data: "{ ChiaveAlbero: '" + Entita_Cod + "', AggregaGrafica: '" + AggregaGrafica + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                alert(msg.d);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });
}
,
clickGeneraPlanning_Final:function (Entita_Cod) {

    var AggregaGrafica = "-1"
    if ($("#ckAggregaEntitaGrafichePerPianificazione").is(":checked")) {
        AggregaGrafica = $("#txtAggregaEntitaGrafichePerPianificazioneScartoM").val();
    }

    var Programmazione_Des = $("#txtNomePlanning").val();

    interfaccia.loading(true);

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/clickGeneraPlanning",
        data: "{ Programmazione_Des: '" + Programmazione_Des + "', ChiaveAlbero: '" + Entita_Cod + "', AggregaGrafica: '" + AggregaGrafica + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                if (stringhe.startsWith(msg.d, 'errore')) {
                    alert(msg.d);
                } else {
                    utility.log("redir to: " + msg.d);
                    alert(msg.d);
                    $("#dialogGeneraPlanning").dialog("close");
                    if ($('#AggiornaFiltro').length > 0) {
                        $('#AggiornaFiltro').click();
                    }
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });
}
//#end gestione planning
//#region selezione per appezzamenti con impianti di specie omogenea
,
clickselectPerSpecie:function () {

    var Entita_Cod = "";
    Entita_Cod = $('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString();  //.replace(/\\/g, '\\\\'); //selectedShape.chiavealbero.toString();
    $("#hidCultivarRicetteNodoPartenza").val($('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString());

    if (Entita_Cod == "") {
        Entita_Cod = $("#hidCultivarRicetteNodoPartenza").val().toString();
        $("#hidCultivarRicetteNodoPartenza").val("");
    }

    var lista_cul_cod = interfaccia.collassaDatiTabella("#tblCultivarXRicette", "#hidCultivarRicette");
    if (lista_cul_cod == "-1" || lista_cul_cod == -1)
        lista_cul_cod = "";

    //se sono tutte valorizzate allora posso procedere
    if (Entita_Cod == "") {
        utility.log("no imp = " + Entita_Cod);
        alert("nessun impianto selezionato.");
        return false;
    }

    this.ElencoImpiantiOmogenei(Entita_Cod, lista_cul_cod, "impianti");
}
,
clickselectPerSpecie_final: function (Entita_Cod) {

    //selezione multipla
    var elementiAlbero = Entita_Cod.split("|");
    utility.log("lll: " + elementiAlbero.length);

    clearSelectionBoth();

    for (var i = 0; i < elementiAlbero.length; i++)
        selezioneDaIdAlbero(elementiAlbero[i], true);
}
,
ElencoImpiantiOmogenei:function (Entita_Cod, lista_cul_cod, dialogDaMostrare, isBootstrap) {

    var splitted = Entita_Cod.split(separatoreChiaveAlbero);
    if (splitted[5] != "0") {
        if (isBootstrap) {
            this.clickRicetta_FinalBs(Entita_Cod);
        } else {
            this.clickRicetta_Final(Entita_Cod);
        }
        
    }
    else {

        interfaccia.loading(true);

        $.ajax({
            type: "POST",
            url: indirizzohttp + "/PopolaDatiSpecie",
            data: "{ ChiaveAlbero: '" + Entita_Cod + "', lista_cul_cod: '" + lista_cul_cod + "', ckRibaltaGrafica: '" + $(ckRibaltaGrafica).is(":checked") + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                interfaccia.loading(false);
                if (msg.d == 'SessioneScaduta') {
                    $('#dialogSessioneScaduta').dialog("open");
                }
                else {

                    if (stringhe.startsWith(msg.d, 'errore')) {
                        alert(msg.d);
                    } else {
                        if (stringhe.startsWith(msg.d, '<')) {
                            $("#tblCultivarRicette").html(msg.d);

                            if (dialogDaMostrare == "ricette") {
                                $("#dialogCultivarRicette").dialog({ buttons: {
                                    "Procedi": function () {
                                        this.clickRicetta()
                                        $(this).dialog("close");
                                    },
                                    "Annulla": function () {
                                        $(this).dialog("close");
                                    }
                                }
                                });

                            }
                            else {

                                $("#dialogCultivarRicette").dialog({ buttons: {
                                    "Seleziona": function () {
                                        this.clickselectPerSpecie()
                                        $(this).dialog("close");
                                    },
                                    "Annulla": function () {
                                        $(this).dialog("close");
                                    }
                                }
                                });

                            }

                            $("#dialogCultivarRicette").dialog("open");
                        }
                        else {
                            lista_cul_cod = "";
                            $("#hidCultivarRicette").val("");
                            $("#tblCultivarXRicette").html("");

                            utility.log("dialogDaMostrare:" + dialogDaMostrare);
                            if (dialogDaMostrare == "ricette") {
                                this.clickRicetta_Final(msg.d);
                            }
                            else {
                                this.clickselectPerSpecie_final(msg.d);
                            }


                        }
                    }
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                interfaccia.loading(false);
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
}
//#end region selezione per appezzamenti con impianti di specie omogenea
//#region gesione ricette
,
clickRicetta: function (isBootstrap) {

    //selezione multipla --> ok passo alla ricetta
    var listaImpiantiDaChiaveAlbero = $("#ChiaveAlberoMultiSelezione").val();
    if (listaImpiantiDaChiaveAlbero != "") {
        utility.log("ChiaveAlberoMultiSelezione: " + listaImpiantiDaChiaveAlbero);
        listaImpiantiDaChiaveAlbero = listaImpiantiDaChiaveAlbero.substring(0, listaImpiantiDaChiaveAlbero.length - 2);

        if (isBootstrap) {
            this.clickRicetta_FinalBs(listaImpiantiDaChiaveAlbero);
        } else {
            this.clickRicetta_Final(listaImpiantiDaChiaveAlbero);
        }
        

        return true;
    }

    //selezione di un planning --> ok passso alla ricetta
    var Entita_Cod = "";
    Entita_Cod = $('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString();  //.replace(/\\/g, '\\\\'); //selectedShape.chiavealbero.toString();
    $("#hidCultivarRicetteNodoPartenza").val($('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString());

    if ( stringhe.startsWith(Entita_Cod, chiaveAlberoPlanning)) {

        if (isBootstrap) {
            this.clickRicetta_FinalBs(Entita_Cod);
        } else {
            this.clickRicetta_Final(Entita_Cod);
        }
        

        return true;
    }

    var lista_cul_cod = interfaccia.collassaDatiTabella("#tblCultivarXRicette", "#hidCultivarRicette");
    if (lista_cul_cod == "-1" || lista_cul_cod == -1)
        lista_cul_cod = "";


    if (Entita_Cod == "") {
        Entita_Cod = $("#hidCultivarRicetteNodoPartenza").val().toString();
        $("#hidCultivarRicetteNodoPartenza").val("");
    }

    //se sono tutte valorizzate allora posso procedere
    if (Entita_Cod == "") {
        utility.log("no imp = " + Entita_Cod + " - lista cul cod " + lista_cul_cod);
        alert("nessun impianto selezionato.");
        return false;
    }

    utility.log("cc = " + lista_cul_cod);

    this.ElencoImpiantiOmogenei(Entita_Cod, lista_cul_cod, "ricette", isBootstrap);

}
,
    clickRicetta_FinalBs: function (Entita_Cod) {

        //se sono tutte valorizzate allora posso procedere
        if (Entita_Cod == "") {
            alert("nessun impianto selezionato.");
            return false;
        }


        ajaxAgronica(indirizzohttp + "/clickRicettaBs",
            "{ ChiaveAlbero: '" + Entita_Cod + "'}",
            function (risposta) {
                ModalBootstrapApri(risposta.RispostaStringa, "Nuova Linea Guida");
            }, null);
        
}
,
clickRicetta_Final: function (Entita_Cod) {
    //var Entita_Cod = $('#HiddenSelezioneAlberoAnagraficaAlberoAnagrafica').val().toString();  //.replace(/\\/g, '\\\\'); //selectedShape.chiavealbero.toString();
    //utility.log(Entita_Cod);

    //se sono tutte valorizzate allora posso procedere
    if (Entita_Cod == "") {
        alert("nessun impianto selezionato.");
        return false;
    }

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/clickRicetta",
        data: "{ ChiaveAlbero: '" + Entita_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            interfaccia.loading(false);
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                if (stringhe.startsWith(msg.d, 'errore')) {
                    alert(msg.d);
                } else {
                    $("#frameRicette").attr("src", msg.d);
                    utility.log("redir to: " + msg.d);
                    $("#pop_up_Ricette").dialog("open");
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            interfaccia.loading(false);
            alert(xhr.status);
            alert(thrownError);
        }
    });

}
//#end region gestione ricette
,

GETvaloreAppIdRateDaStringa: function (AppIdRate, chiave, nome) {
    var vAppIdRate = AppIdRate.split("|");

    for (var k = 0; k < vAppIdRate.length; k++) {

        var chiaveValore = vAppIdRate[k].split("§ ");
        if (stringhe.contains(chiaveValore[0], nome + " " + chiave))
            return chiaveValore[1];

    }

    for (var k = 0; k < vAppIdRate.length; k++) {

        var chiaveValore = vAppIdRate[k].split("§ ");
        if (stringhe.contains(chiaveValore[0], chiave))
            return chiaveValore[1];

    }
    return undefined;
}
,
GETvAppIdRateChiaviValori: function (AppIdRate) {
    return AppIdRate.split("§ ");
}
,
SettaMaxMin: function (AppIdRate, elemento , nome) {
    var vAppIdRate = this.GETvAppIdRateDaStringa(AppIdRate);
    for (var i = 0; i < elemento.length; i++) {
        for (var k = 0; k < vAppIdRate.length; k++) {

            var cvAppIdRate = this.GETvAppIdRateChiaviValori(vAppIdRate[k]);

            if (stringhe.contains(nome + " " + elemento[i].nome, cvAppIdRate[0])) {

                if ($.isNumeric(cvAppIdRate[1])) {

                    let val = parseFloat(cvAppIdRate[1]);

                    if (val < elemento[i].v_min) {
                        elemento[i].v_min = val;
                        //utility.log(elemento[i].v_min);
                    }

                    if (val > elemento[i].v_max) {
                        elemento[i].v_max = val;
                    }
                }


            }


        }

    }
}
}



