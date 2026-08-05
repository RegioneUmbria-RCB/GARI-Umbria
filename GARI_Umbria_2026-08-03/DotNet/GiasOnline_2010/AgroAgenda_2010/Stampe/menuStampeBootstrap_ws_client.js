var indirizzohttp = "./menuStampeBootstrap.aspx";
var indirizzohttpWSGenerali = "./CdG_WS.aspx";

function caricaStampeAutorizzate() {
    ajaxAgronicaSync(indirizzohttp + "/caricaStampeAutorizzate",
        {},
        false,
        function (risposta) {
            if (risposta.RispostaOK) {
                stampeAutorizzate = JSON.parse(risposta.RispostaStringa)
            }
        }, function (risposta) { gestioneErrore(risposta.RispostaStringa) });
}

function caricaEnumStampe() {
    ajaxAgronicaSync(indirizzohttp + "/ottieniEnumStampe",
        {},
        false,
        function (risposta) {

            if (risposta.RispostaOK) {
                enumStampe = JSON.parse(risposta.RispostaStringa)
            }
        }, function (risposta) { gestioneErrore(risposta.RispostaStringa) });
}

function gestisciStampa(idStampa) {
    var prosegui = controllaDataValida();
    if (!prosegui) return;

    let anno = get_data("anno");
    var datiStampe = {
        "anno": anno == null ? "" : anno.split("-")[0],
        "switchGenerale1": getKendoSwitch("switchGenerale1"),
        "switchGenerale2": getKendoSwitch("switchGenerale2"),
        "dataInizio": get_data("txtDataInizio"),
        "dataFine": get_data("txtDataFine")
    }

    if (usaFiltroRicercaNG) {
        objRedirectStampe_FiltroRicercaNG = {
            "idStampa": idStampa,
            "datiStampe": datiStampe,
            "tipoMostra": -1,
        }
        var param = kendo.stringify({
            "piva": currentPiva,
            "id_stampa": idStampa,
            datiStampe: datiStampe
        });
        ajaxAgronica(indirizzohttp + "/Link_Pagina_FiltroRicercaNG",
            param,
            function (risposta) {
                let res = JSON.parse(risposta.RispostaStringa)

                if (res.RedirectFiltroRicercaNG) {
                    apriFinestraFiltroRicercaNG(res.RispostaStringa)
                }
                else
                    window.location = res.RispostaStringa;
            }, function (risposta) {
                let res = JSON.parse(risposta.RispostaStringa)
                kendo.alert(res.RispostaStringa)

                objRedirectStampe_FiltroRicercaNG = null
            });
    } else {

        WaitFrame.show();

        var param = kendo.stringify({
            reportSel: idStampa, datiStampe: datiStampe
        });
        ajaxAgronica(indirizzohttp + "/gestisciStampa",
            param,
            function (risposta) {
                window.location = risposta.RispostaStringa;
            }, function (risposta) {
                gestioneErrore(risposta.RispostaStringa)
                WaitFrame.hide();
            });
    }
}

function caricaStampePreferite() {
    ajaxAgronicaSync(indirizzohttp + "/caricaStampePreferite",
        {},
        false,
        function (risposta) {
            if (risposta.RispostaOK) {
                stampePreferite = JSON.parse(risposta.RispostaStringa);
            }
        }, function (risposta) {
            gestioneErrore(risposta.RispostaStringa)
        });
}

function aggiungiPreferiti(idStampe) {
    let param = { idStampe: idStampe };

    console.log(param)
    ajaxAgronicaSync(indirizzohttp + "/aggiungiPreferiti",
        JSON.stringify(param),
        false,
        function (risposta) {
            console.log(risposta);
            if (risposta.RispostaOK) {
                caricaStampePreferite()
                var ddRicercaKendo = $("#ddRicercaRapida").data("kendoDropDownList")
                ddRicercaKendo.trigger("change");
            }
        }, function (risposta) {
            gestioneErrore(risposta.RispostaStringa)
        });
}

function eliminaPreferiti(idStampe) {
    let param = { idStampe: idStampe };

    ajaxAgronicaSync(indirizzohttp + "/eliminaPreferiti",
        JSON.stringify(param),
        false,
        function (risposta) {
            console.log(risposta)
            if (risposta.RispostaOK) {
                caricaStampePreferite()
                var ddRicercaKendo = $("#ddRicercaRapida").data("kendoDropDownList")
                ddRicercaKendo.trigger("change");
            }
        }, function (risposta) {
            gestioneErrore(risposta.RispostaStringa)
        });
}

function gestioneErrore(risposta) {
    MessaggioErrore_Bootstrap('Errore: ${risposta} <br/>', "DIV_Messaggi");
}

function WS_Gestisci_Redirect_Stampe(chiavi) {
    var param = kendo.stringify({
        "tipoMostra": objRedirectStampe_FiltroRicercaNG.tipoMostra,
        "id_stampa": objRedirectStampe_FiltroRicercaNG.idStampa,
        "chiavi": chiavi,
        "datiStampe": objRedirectStampe_FiltroRicercaNG.datiStampe
    });

    ajaxAgronica(indirizzohttp + "/WS_Gestisci_Redirect_Stampe",
        param,
        function (risposta) {
            objRedirectStampe_FiltroRicercaNG = null
            //window.location = risposta.RispostaStringa;
            window.open(risposta.RispostaStringa)
        }, function (risposta) {
            objRedirectStampe_FiltroRicercaNG = null
            kendo.alert(risposta.Errore)
        }, null, true);
}