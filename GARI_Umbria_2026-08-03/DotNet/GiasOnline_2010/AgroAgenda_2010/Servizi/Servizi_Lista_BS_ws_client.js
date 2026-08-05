function ws_carica_pratiche(piva) {
    return new Promise((resolve, reject) => {
        var filtroImprese = "";
        if (piva === undefined) {
            filtroImprese = Cmb_Imprese.value();

        } else {
            filtroImprese = piva;
        }

        var dataFiltro = new Date(1900, 0, 1, 0, 0, 0, 0);
        if (dataSwitch.value()) {
            dataFiltro = TxtValiditaPratica.value();
        }

        let filtroServizi = new Array();
        let filtroServiziStati = "";

        if (praticheSwitch.value()) {
            filtroServiziStati = JSON.stringify(kendoFiltroPratiche.dataSource.data());
        }

        if (serviziSwitch.value()) {
            filtroServizi = ddlServizi.value();
        }

        var parametri = kendo.stringify({
            FiltroImprese_str: filtroImprese,
            dataFiltro: dataFiltro,
            filtroServiziStati: filtroServiziStati,
            filtroServizi: filtroServizi
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/Carica_Pratiche_Light",
            parametri,
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);
                resolve(resp);
            }, null, false, false);

    });
}

function ws_Carica_Cmb_Imprese(options) {
    var parametri = kendo.stringify({});

    ajaxAgronica("Servizi_Lista_BS.aspx/Carica_Cmb_Imprese",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "Piva": "-1", "Rag_Soc": "TUTTE" };
            resp.unshift(objVuoto);
            options.success(resp);
        }, null, false, false);
}

function ws_Carica_Cmb_Imprese_New(options) {
    var parametri = kendo.stringify({});

    ajaxAgronica("Servizi_Lista_BS.aspx/Carica_Cmb_Imprese",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            options.success(resp);
        }, null, null, false);
}

async function Permesso_Elenco_Pratiche(callback) {
    return new Promise((resolve, reject) => {
        ajaxAgronica("Servizi_Lista_BS.aspx/Permesso_Elenco_Pratiche", " { } ", function (risposta) {
            resolve(risposta);
        }, null, false, false);
    });
}

async function ws_aggiornaPratica(str_pratiche) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            str_pratiche: JSON.stringify(str_pratiche)
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/aggiornaPratiche",
            parametri,
            function (risposta) {
                resolve();
            }, null, false, false);
    });
}

async function ws_eliminaPratica(str_pratiche) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            str_pratiche: JSON.stringify(str_pratiche)
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/eliminaPratiche",
            parametri,
            function (risposta) {
                resolve(risposta);
            }, null, false, false);
    });
}

async function ws_attivaServizio() {
    return new Promise((resolve, reject) => {
        let data_inizio = Data_Inizio_New.value();
        if (data_inizio === "" || data_inizio === null || data_inizio === undefined) {
            data_inizio = "01/01/1900";
        }
        let data_fine = Data_Fine_New.value();
        if (data_fine === "" || data_fine === null || data_fine === undefined) {
            data_fine = "31/12/2100";
        }
        let Numero = $("#Txt_Numero").val();
        var parametri = kendo.stringify({
            piva: CmbImprese_New.value(),
            Servizio_Cod: CmbServizi_New.value(),
            Data_Inizio_str: Data_Inizio_New.value(),
            Data_Fine_str: Data_Fine_New.value(),
            Data_Inizio_Pratica_str: Cmb_Data_Apertura.value().toLocaleDateString(),
            Numero: Numero
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/attivaServizio",
            parametri,
            function (risposta) {
                resolve(risposta);
            }, null, false, false);
    });
}


async function ws_caricaProcedura(Pratica_Cod) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Pratica_Cod: Pratica_Cod
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/caricaProcedura",
            parametri,
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                resolve(arr);
            }, null, false, false);
    });
}

async function ws_caricaStatiDestinazione(Stato_Cod, Servizio_Cod) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Stato_Cod: Stato_Cod,
            Servizio_Cod: Servizio_Cod
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/StatiDestinazione",
            parametri,
            function (risposta) {
                let arr = JSON.parse(risposta.RispostaStringa);
                resolve(arr);
            }, null, false, false);
    });
}

async function ws_PassaggioStato(Pratiche_Str, Stato_Cod, Data_Riferimento, Note, WorkFlow_Cod, PassaggioDiStato_cod, objLista_DSS_Selezionati, controlli) {
    return new Promise((resolve, reject) => {

        if (controlli == undefined) {
            controlli = false;
        }

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

async function ws_aggiornaStato(Pratica_Cod, Stato_Cod, Data_Riferimento, note, WorkFlow_Cod, PassaggioDiStato_Cod, Servizio_Cod, Stato_Origine) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Pratica_Cod: Pratica_Cod,
            Stato_Cod: Stato_Cod,
            Data_Riferimento_str: Data_Riferimento.toLocaleDateString(),
            Note: note,
            WorkFlow_Cod: WorkFlow_Cod,
            PassaggioDiStato_Cod: PassaggioDiStato_Cod,
            Servizio_Cod: Servizio_Cod,
            Stato_Origine: Stato_Origine
        });

        ajaxAgronica("Servizi_Lista_BS.aspx/aggiornaStato",
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

function LeggiImpostazione_Superuser(impostazione_cod) {

    var param = {
        objP_Utenti: objP_utenti,
        impostazione_cod: impostazione_cod,
        Username_1Utente_o_2SuperUser: 2
    };
    return new Promise((resolve, reject) => {
        ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_ImpostazioniByUsername_1Utente_o_2SuperUser", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa.toLowerCase());
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

async function ws_EsportazioneZespri() {
    return new Promise((resolve, reject) => {
        var param = {};

        ajaxAgronica("Servizi_Lista_BS.aspx/EsportazioneZespri", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}

async function ws_EsportazioneRegione() {
    return new Promise((resolve, reject) => {
        var param = {};

        ajaxAgronica("Servizi_Lista_BS.aspx/EsportazioneRegione", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}

async function ws_BloccaPratiche(blocca, praticaList) {
    return new Promise((resolve, reject) => {
        var param = {
            "blocca": blocca,
            "pratiche": praticaList
        };

        ajaxAgronica("Servizi_Lista_BS.aspx/BloccaPratiche", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}

async function Carica_PassaggiDiStato(pratica_cod) {
    return new Promise((resolve, reject) => {
        var param = {
            "Pratica_Cod": pratica_cod
        };

        ajaxAgronica("Servizi_Lista_BS.aspx/Carica_PassaggiDiStato", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}

async function trovaStatiServizio(Servizio_Cod) {
    return new Promise((resolve, reject) => {
        var param = {
            "Servizio_Cod": Servizio_Cod
        };

        ajaxAgronica("Servizi_Lista_BS.aspx/trovaStatiServizio", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}


async function ws_CambiaServizio(Pratica_Cod, Servizio_Cod) {
    return new Promise((resolve, reject) => {
        var param = {
            "Pratica_Cod": Pratica_Cod,
            "Servizio_Cod": Servizio_Cod
        };

        ajaxAgronica("Servizi_Lista_BS.aspx/CambiaServizio", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}

async function ws_isSuperUSer() {
    return new Promise((resolve, reject) => {
        var param = {};

        ajaxAgronica("Servizi_Lista_BS.aspx/isSuperUser", JSON.stringify(param),
            function (risposta) {
                if (risposta.RispostaStringa == "true") {
                    resolve(true);
                } else {
                    resolve(false);
                }
            }, null, false, false);
    });
}

async function ws_UndoPratiche(praticaList) {
    return new Promise((resolve, reject) => {
        var param = {
            "str_pratiche": JSON.stringify(praticaList)
        };

        ajaxAgronica("Servizi_Lista_BS.aspx/UndoPratiche", JSON.stringify(param),
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}