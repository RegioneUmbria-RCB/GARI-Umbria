function LeggiUltimoValoreAnnoPrecedente(piva,anno) {
    var parametri = kendo.stringify({ "piva": piva, "anno": anno, tipolettura: 1 });

    ajaxAgronicaSync(indirizzohttp + "/LeggiSingolaLetturaContatore",
        parametri,
        false,
        function (risposta) {
            $(cIdDatiUltimaLetturaAnnoPrecedente).val(risposta.RispostaStringa);
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function LeggiPrimoValoreAnnoSuccessivo(piva,anno) {
    var parametri = kendo.stringify({ "piva": piva, "anno": anno, tipolettura: 2 });

    ajaxAgronicaSync(indirizzohttp + "/LeggiSingolaLetturaContatore",
        parametri,
        false,
        function (risposta) {
            $(cIdDatiPrimaLetturaAnnoSuccessivo).val(risposta.RispostaStringa);
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}


function LeggiLettureContatori(piva, id_contatore, anno) {
    var parametri = kendo.stringify({ "piva": piva, "id_contatore": id_contatore, "anno": anno });

    ajaxAgronicaSync(indirizzohttp + "/LeggiLettureContatore",
        parametri,
        false,
        function (risposta) {
            $(cIdDatiLetture).val(risposta.RispostaStringa);
            CaricaDatiVideata();
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function EliminaLettureContatori(id_lettura,piva, id_contatore, startdate,enddate) {
    var parametri = kendo.stringify({ "id_lettura": id_lettura, "piva": piva, "id_contatore": id_contatore, "startdate": startdate, "enddate": enddate});

    ajaxAgronicaSync(indirizzohttp + "/EliminaLetturaContatore",
        parametri,
        false,
        function (risposta) {
            $(cIdDatiLetture).val(JSON.stringify(risposta.RispostaStringa));
            CaricaDatiVideata();
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function ScriviAggiornaLettureContatori() {
    var letture = JSON.parse($(cIdDatiLetture).val());
    delete letture.__type;

    var newDettaglio = $("#divKendoGridLetture").data("kendoGrid").dataSource.data();
    for (var i = 0; i < newDettaglio.length; i++) {
        if (newDettaglio[i].id === -1) {
            letture.elencoLetture.push({ "id": newDettaglio[i].id, "id_contatore": newDettaglio[i].id_contatore, "datalettura": newDettaglio[i].datalettura, "valore": newDettaglio[i].valore });
        }
        if (newDettaglio[i].dirty === true) {
            for (var j = 0; j < letture.elencoLetture.length; j++) {
                if (newDettaglio[i].id === letture.elencoLetture[j].id) {
                    letture.elencoLetture[j].datalettura = newDettaglio[i].datalettura;
                    letture.elencoLetture[j].valore = newDettaglio[i].valore;
                    break;
                }
            }
        }
    }
    for (var j = 0; j < letture.elencoLetture.length; j++) {
        var check = newDettaglio.find(item => item.id === letture.elencoLetture[j].id);
        if (!check) {
            let obj = letture.elencoLetture[j];
            letture.elencoLetture.unshift(obj);
        }
    }

    letture.anno = $("#txtAnno").data("kendoNumericTextBox").value();

    var parametri = kendo.stringify({ "data": kendo.stringify(letture) });

    ajaxAgronicaSync(indirizzohttp + "/ScriviLettureContatori",
        parametri,
        false,
        function (risposta) {
            $(cIdDatiLetture).val(JSON.stringify(risposta.RispostaStringa));
            CaricaDatiVideata();
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}

function LeggiAnagraficaContatore(piva,datarif) {
    var parametri = kendo.stringify({ "piva": piva, "datarif": datarif });

    ajaxAgronicaSync(indirizzohttp + "/LeggiContatoriAziendali",
        parametri,
        false,
        function (risposta) {
            $(cIdIdContatore).val(risposta.RispostaStringa);
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        }
    );
}