async function leggiDatiPratica(Pratica_Cod) {
    return new Promise((resolve, reject) => {

        var parametri = {
            Pratica_Cod: Pratica_Cod
        };

        ajaxAgronica("Servizi_Lista_BS.aspx/leggiDatiPratica",
            kendo.stringify(parametri),
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                if (arr.length == 1) {
                    resolve(arr[0]);
                }
                resolve({});
            }, null, false, false);
    });
}

async function leggiPassaggioDiStato(PassaggioDiStato_Cod) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            PassaggioDiStato_Cod: PassaggioDiStato_Cod
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/leggiPassaggioDiStato",
            parametri,
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                if (arr.length == 1) {
                    resolve(arr[0]);
                }
                resolve({});
            }, null, false, false);
    });
}

async function ws_caricaProcedura(Pratica_Cod) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Pratica_Cod: Pratica_Cod
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/CaricaProcedura",
            parametri,
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                resolve(arr);
            }, null, false, false);
    });
}

async function Carica_cmb_Procedura(ddlProcedure) {
    return new Promise((resolve, reject) => {
        if ($("#CmbProcedura").data("kendoDropDownList") !== undefined && CmbProcedura != undefined) {
            CmbProcedura.destroy();
            $("#CmbProcedura").html("");
        }
        let onLoad = true;
        $("#CmbProcedura").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "WorkFlow_des",
            dataValueField: "WorkFlow_Cod",
            dataSource: ddlProcedure,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                resolve(this);
            },
            optionLabel: TraduzioneMultiResx(resxObj, "Seleziona", 'SELEZIONA')
        }).data("kendoDropDownList");

    });
}

async function ws_caricaStatiDestinazione(Stato_Cod, Servizio_Cod, Pratica_Cod) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Stato_Cod: Stato_Cod,
            Servizio_Cod: Servizio_Cod,
            elencoPratiche: [Pratica_Cod]
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/StatiDestinazione",
            parametri,
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                resolve(arr);
            }, null, false, false);
    });
}

async function ws_caricaStatiDestinazioneUMA(Pratica_Cod, Stato_Cod, Servizio_Cod) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Pratica_Cod: Pratica_Cod,
            Stato_Cod: Stato_Cod,
            Servizio_Cod: Servizio_Cod
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/StatiDestinazioneUMA",
            parametri,
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                resolve(arr);
            }, null, false, false);
    });
}

async function ws_caricaPacchettiDSS() {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({});

        ajaxAgronica("Servizi_Lista_BS.aspx/caricaPacchettiDSS",
            parametri,
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                resolve(arr);
            }, null, false, false);
    });
}

async function ws_CaricaDSS_PassaggioStato(PassaggioDiStato_Cod) {
    return new Promise((resolve, reject) => {
        if (PassaggioDiStato_Cod === 0) {
            resolve(new Array());
        } else {
            var ActualDate = new Date();
            var stringData = ActualDate.toLocaleDateString();
            var param = {
                PassaggioDiStato_Cod: PassaggioDiStato_Cod
            };

            ajaxAgronica("Servizi_Lista_BS.aspx/CaricaDSS_PassaggioStato", JSON.stringify(param),
                function (risposta) {
                    resolve(JSON.parse(risposta.RispostaStringa));
                }, null, false, false);
        }
    });
}

async function ws_PassaggioStato(Pratiche_Str, Stato_Cod, Data_Riferimento, Note, WorkFlow_Cod, PassaggioDiStato_cod, controlli) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Pratiche_Str: JSON.stringify(Pratiche_Str),
            Stato_Cod: Stato_Cod,
            Data_Riferimento_str: Data_Riferimento.toLocaleDateString(),
            Note: Note,
            WorkFlow_Cod: WorkFlow_Cod,
            PassaggioDiStato_cod: PassaggioDiStato_cod,
            objLista_DSS_Selezionati: JSON.stringify(objLista_DSS_Selezionati),
            controlli: controlli
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/PassaggioStato",
            parametri,
            function (risposta) {
                resolve(risposta);
            }, null, false, false);
    });
}

function LeggiPermessoUtenteP(username, id_attivita, id_operazione) {

    var ActualDate = new Date();
    var stringData = ActualDate.toLocaleDateString();
    var param = {
        UserName: username,
        Id_Servizio: 5,
        Id_Attivita: id_attivita,
        Id_Operazione: id_operazione,
        DataOraControllo: ActualDate,
        xFiltroAggiuntivo: '',
        objP_utenti: objP_utenti
    };
    return new Promise((resolve, reject) => {
        ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/Controlla_Permessi_UtenteR", JSON.stringify(param),
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa.toLowerCase()));
            }, null, false, false);
    });
}