function filtraMovimenti() {
    var datiAggiuntiviFiltroRicerca = [
        {
            Campo: "ZVN",
            Checked: $("#ChkGruppo5 input[value='33']").is(":checked")
        },
        {
            Campo: "ACA",
            Checked: $("#ChkGruppo5 input[value='34']").is(":checked")
        }
    ];
    var param = kendo.stringify({
        "datiAggiuntiviFiltroRicerca": JSON.stringify(datiAggiuntiviFiltroRicerca)
    });

    ajaxAgronica(indirizzohttp + "/Link_Pagina_FiltroRicercaNG",
        param,
        function (risposta) {
            let res = JSON.parse(risposta.RispostaStringa);
            apriFinestraFiltroRicercaNG(res.RispostaStringa);
        }, function (risposta) {
            kendo.alert(JSON.parse(risposta.Errore))
        });
}

function apriFinestraFiltroRicercaNG(url) {
    window.addEventListener('message', chiudiFinestraFiltroRicercaNG);

    $(document.body).append('<div id="filtro_ricerca_ng"></div>');

    var entity = 'Movimenti'

    $('#filtro_ricerca_ng').kendoWindow({
        title: "Filtra {0}".format(entity),
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#filtro_ricerca_ng').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function chiudiFinestraFiltroRicercaNG(event) {
    let kWin = $('#filtro_ricerca_ng').data("kendoWindow");
    let urlKWin = kWin.options.content.url;

    if (verificaOriginSecondaria(window, urlKWin, event) &&
        (event != null && event.data != null) && (event.data.messaggio != null) &&
        event.data.messaggio.includes("chiudiWindowGiasNG")) {

        EsportaExcel(event.data.inData.chiavi, event.data.inData.tipoEntita)

        kWin.close();
    }
}

function EsportaExcel(chiavi, tipoEntita) {

    var controlValues = {
        "Chiavi": chiavi,
        "ChkValue0": $("#ChkGruppo1 input[value='0']").is(":checked"),
        "ChkValue1": $("#ChkGruppo1 input[value='1']").is(":checked"),
        "ChkValue2": $("#ChkGruppo1 input[value='2']").is(":checked"),
        "ChkValue3": $("#ChkGruppo1 input[value='3']").is(":checked"),
        "ChkValue4": $("#ChkGruppo1 input[value='4']").is(":checked"),
        "ChkValue5": $("#ChkGruppo1 input[value='5']").is(":checked"),
        "ChkValue6": $("#ChkGruppo1 input[value='6']").is(":checked"),
        "ChkValue7": $("#ChkGruppo1 input[value='7']").is(":checked"),
        "ChkValue8": $("#ChkGruppo1 input[value='8']").is(":checked"),
        "ChkValue9": $("#ChkGruppo1 input[value='9']").is(":checked"),
        "ChkValue10": $("#ChkGruppo2 input[value='10']").is(":checked"),
        "ChkValue11": $("#ChkGruppo2 input[value='11']").is(":checked"),
        "ChkValue12": $("#ChkGruppo2 input[value='12']").is(":checked"),
        "ChkValue13": $("#ChkGruppo2 input[value='13']").is(":checked"),
        "ChkValue14": $("#ChkGruppo2 input[value='14']").is(":checked"),
        "ChkValue15": $("#ChkGruppo2 input[value='15']").is(":checked"),
        "ChkValue16": $("#ChkGruppo2 input[value='16']").is(":checked"),
        "ChkValue17": $("#ChkGruppo2 input[value='17']").is(":checked"),
        "ChkValue18": $("#ChkGruppo2 input[value='18']").is(":checked"),
        "ChkValue19": $("#ChkGruppo2 input[value='19']").is(":checked"),
        "ChkValue20": $("#ChkGruppo3 input[value='20']").is(":checked"),
        "ChkValue21": $("#ChkGruppo3 input[value='21']").is(":checked"),
        "ChkValue22": $("#ChkGruppo3 input[value='22']").is(":checked"),
        "ChkValue23": $("#ChkGruppo3 input[value='23']").is(":checked"),
        "ChkValue24": $("#ChkGruppo3 input[value='24']").is(":checked"),
        "ChkValue25": $("#ChkGruppo3 input[value='25']").is(":checked"),
        "ChkValue26": $("#ChkGruppo3 input[value='26']").is(":checked"),
        "ChkValue27": $("#ChkGruppo3 input[value='27']").is(":checked"),
        "ChkValue28": $("#ChkGruppo3 input[value='28']").is(":checked"),
        "ChkValue29": $("#ChkGruppo3 input[value='29']").is(":checked"),
        "ChkValue30": $("#ChkGruppo3 input[value='30']").is(":checked"),
        "ChkValue31": $("#ChkGruppo3 input[value='31']").is(":checked"),
        "ChkValue32": $("#ChkGruppo4 input[value='32']").is(":checked"),
        "ChkValue33": $("#ChkGruppo5 input[value='33']").is(":checked"),
        "ChkValue34": $("#ChkGruppo5 input[value='34']").is(":checked"),
        "RdBList_Esporta_SelectedValue": $("input[id*='RdBList_Esporta']:checked").val()
    };

    var parametri = JSON.stringify({controlValues: controlValues})

    return new Promise((resolve, reject) => {
        ajaxAgronica(indirizzohttp + "/EsportazioneAgenda_Excel",
            parametri,
            function (risposta) {
                if (risposta.RispostaStringa != '') {
                    let risp = JSON.parse(risposta.RispostaStringa);
                    let fileDati = risp.File;
                    let nomeFile = risp.NomeFile;
                    let estensione = risp.Estensione;

                    SaveAndOpenFileByteArray(nomeFile, fileDati, estensione);
                }
            }, function (risposta) {
                reject();
                gestioneErrore(risposta.RispostaStringa)
            });
    })

    //ajaxAgronica(indirizzohttp + "/SalvaChiaviEsportazioneAgenda",
    //    data,
    //    function (risposta) {
    //        location.reload() //forziamo il reload della pagina per poter esportare l'excel
    //    }, function (risposta) {
    //        kendo.alert(risposta.Errore)
    //    });
}