function LeggiInizializzaDomanda(iddomanda, piva, anno) {

    let id_domanda = 0;
    if (iddomanda !== "") {
        id_domanda = iddomanda;
    }
    var parametri = kendo.stringify({ "id": id_domanda, "piva": piva, "anno": anno });

    ajaxAgronicaSync(indirizzohttp + "/LeggiDomanda",
        parametri,
        false,
        function (risposta) {
            $(cIdDatiDomanda).val(risposta.RispostaStringa);
            CaricaDatiVideata();
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function GetUrlBack() {

    var parametri = kendo.stringify({ "codPagina": 2, "data": $(cIdPaginaRedirectCodificata).val() });

    ajaxAgronicaSync(indirizzohttp + "/GetUrlIndietro",
        parametri,
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            window.location.href = risp;
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function SalvaEsci() {
    if (SalvaTutto() === true) {
        window.history.go(-1);
    }
}

function SalvaTutto() {
    let c = 0;
    let check = CheckSalvaOK($("#divKendoOut").data("kendoGrid").dataSource.data());
    if (check === false) {
        kendo.alert("Selezionare almeno una particella prima di salvare. ");
        return false;
    }
    ScriviAggiornaDomandaIrrigua();
    return true;
}

function EsciSenzaSalvare() {
    if ($(cIdEnableMod).val() === "False") {
        if ($(cIdPaginaRedirectCodificata).val() !== "") {
            GetUrlBack();
        } else {
            window.history.go(-1);
        }
    } else {
        if (CurrentMod === true) {
            $("#confermaUscitaDialog").kendoDialog({
                width: "400px",
                title: "",
                closable: false,
                modal: true,
                visible: false,
                content: "Uscire senza salvare?",
                actions: [
                    {
                        text: "Conferma", action: function (e) {
                            if ($(cIdPaginaRedirectCodificata).val() !== "") {
                                GetUrlBack();
                            } else {
                                window.history.go(-1);
                            }
                        }
                    },
                    { text: "Annulla", primary: true }
                ]
            });
            $("#confermaUscitaDialog").data("kendoDialog").open();
        } else {
            if ($(cIdPaginaRedirectCodificata).val() !== "") {
                GetUrlBack();
            } else {
                window.history.go(-1);
            }
        }
    }
}

function ScriviAggiornaDomandaIrrigua() {
    var domanda = JSON.parse($(cIdDatiDomanda).val());
    delete domanda.__type;

    var newDettaglio = $("#divKendoOut").data("kendoGrid").dataSource.data();
    for (var i = 0; i < newDettaglio.length; i++) {
        domanda.dettaglio[i].Selezionato = newDettaglio[i].Selezionato;
    }
    var parametri = kendo.stringify({ "data": kendo.stringify(domanda) });

    ajaxAgronicaSync(indirizzohttp + "/ScriviAggiornaDomanda",
        parametri,
        false,
        function (risposta) {
            $(cIdDatiDomanda).val(risposta.RispostaStringa);
            CaricaDatiVideata();
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function CheckSalvaOK(data) {
    let c = 0;
    for (var i = 0; i < data.length; i++) {
        if (data[i].Selezionato === true) {
            c++;
        }
    }
    if (c > 0) {
        return true;
    } else {
        return false;
    }
}