//--------------------------------------------------------------------------------
// GRIGLIA IMPUTAZIONE
//--------------------------------------------------------------------------------

function EmptyRead(options) { }
function EmptySubmit(options) { }

function CaricaGrigliaRifCatastali(options) {

    if (parseInt($(cIdAgenda).val()) !== 0) {

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CaricaGrigliaRifCatastali",
            kendo.stringify({
                piva: $(cIdPiva).val(),
                idAgenda: parseInt($(cIdAgenda).val())
            }),
            false,
            function(risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);
                ImpostaTipoOpTestataRifCatastali();
            },
            null);

    } else {

        ImpostaTipoOpTestataRifCatastali();

    }    
        
}

//--------------------------------------------------------------------------------
// FUNZIONI CONTROLLO E RESTORE
//--------------------------------------------------------------------------------

// Controllo campi obbligatori
function controllaRigheNonValidePerSubmitGrid(tipoOperazioneRiga, righe) {

    elencoErrori = new Object();

    elencoErrori.contesto = tipoOperazioneRiga;
    elencoErrori.messErr= "";
    elencoErrori.nrErr = 0;

    for (let x = 0; x < righe.length; x++) {

        var item = righe[x];

        // Controlli particella

        if (item.Id_Impr_Part === 0) {
            elencoErrori.messErr += componiMessaggioErrore(nomeColonnaParticella,"non impostata");
            elencoErrori.nrErr++;
        }

        if (item.Cod_Particella === "") {
            elencoErrori.messErr += componiMessaggioErrore(nomeColonnaCodiceParticella, "non impostato");
            elencoErrori.nrErr++;
        }

        if (tipoOperazioneRiga === enum_TipoOperazioneRiga.Inserimento) {

            let griglia = $(tabGrigliaRifCatastali).data("kendoGrid");
            let datiGriglia = griglia.dataSource.data();
            let totaleRighe = datiGriglia.length;

            for (let i = 0; i < totaleRighe; i++) {
                let rigaGriglia = datiGriglia[i];
                if (rigaGriglia.Id > 0 && rigaGriglia.dirty === false &&
                    rigaGriglia.PROV === item.PROV && rigaGriglia.COM === item.COM &&
                    rigaGriglia.SEZIONE === item.SEZIONE && rigaGriglia.FOGLIO === item.FOGLIO &&
                    rigaGriglia.NUMERO === item.NUMERO && rigaGriglia.SUBALTERNO === item.SUBALTERNO) {

                    elencoErrori.messErr += componiMessaggioErrore(nomeColonnaParticella, "già presente nel contratto corrente");
                    elencoErrori.nrErr++;

                }
            }

        }

        // Controlli superficie affittata

        if (item.Superficie === 0) {
            elencoErrori.messErr += componiMessaggioErrore(nomeColonnaSuperficieAffittata,"non impostata");
            elencoErrori.nrErr++;
        }

        if (item.Superficie < 0) {
            elencoErrori.messErr += componiMessaggioErrore(nomeColonnaSuperficieAffittata,"negativa");
            elencoErrori.nrErr++;
        }

        if (item.Superficie > item.Superficie_Catastale) {
            elencoErrori.messErr += componiMessaggioErrore(nomeColonnaSuperficieAffittata, "maggiore di " + nomeColonnaSuperficieCatastale);
            elencoErrori.nrErr++;
        }

        // Controlli data inizio/fine affitto

        let dataInizioValiditaContratto = kendo.parseDate($("#inDataInizVal").val());
        let dataFineValiditaContratto = kendo.parseDate($("#inDataFineVal").val());

        if (item.Validita_Inizio < dataInizioValiditaContratto || item.Validita_Inizio > dataFineValiditaContratto) {
            elencoErrori.messErr += componiMessaggioErrore(nomeColonnaDataInizioAffitto, "al di fuori del periodo contrattuale");
            elencoErrori.nrErr++;
        }

        if (item.Validita_Fine < dataInizioValiditaContratto || item.Validita_Fine > dataFineValiditaContratto) {
            elencoErrori.messErr += componiMessaggioErrore(nomeColonnaDataFineAffitto, "al di fuori del periodo contrattuale");
            elencoErrori.nrErr++;
        }

    }

    return elencoErrori;

}

function componiMessaggioErrore(nomeColonna,messaggio) {

    let messaggioErrore = "";

    if (elencoErrori.nrErr === 0) {
        messaggioErrore = "Dati non corretti in <b>" + elencoErrori.contesto + "</b> griglia riferimenti catastali:";
    }

    messaggioErrore += "</br> - <b>" + nomeColonna + "</b> " + messaggio;

    return messaggioErrore;

}

//--------------------------------------------------------------------------------
// RIEMPIMENTO ELENCHI SU RICHIESTA E LETTURE
//--------------------------------------------------------------------------------

function Elenco_Particelle_Riempi() {

    var risultato_lettura;
        
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Leggi_Particelle_Imprese",
        kendo.stringify(
            {
                piva: $(cIdPiva).val(),
                saCod: Qs_SaCod,
                dataInizioValidita: kendo.parseDate($("#inDataInizVal").val()),
                dataFineValidita: kendo.parseDate($("#inDataFineVal").val())
            }),
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);
       
    return risultato_lettura;

}

function LeggiLinkPaginaCatastoEdit() {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Link_Pagina_Catasto_Edit",
        kendo.stringify(
            {
                piva: $(cIdPiva).val(),
                titoloPossesso: 3,
                validitaInizio: get_data("inDataInizVal"),
                validitaFine: get_data("inDataFineVal")
            }),
        false,
        function (risposta) {
            risultato_lettura = risposta.RispostaStringa;
        }, null);

    return risultato_lettura;

}

//--------------------------------------------------------------------------------
// SUBMIT GENERALE
//--------------------------------------------------------------------------------

var replacer = function (key, value) {

    if (this[key] instanceof Date) {
        return this[key].toUTCString();
    }

    return value;
}

// Eventi di click pulsanti
function AggiornaEffettivoRifCatastali(forzaCancellaLegamiAppezzamentiCampi) {

    if (forzaCancellaLegamiAppezzamentiCampi === undefined) {
        forzaCancellaLegamiAppezzamentiCampi = enum_forzaCancellaLegami.Indefinita;
    }

    var allOk = false;

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        saCod: Qs_SaCod,
        idAgenda: parseInt($(cIdAgenda).val()),
        rigaInserita: kendoEscapeOggetto(rigaInserita),
        rigaModificata: kendoEscapeOggetto(rigaModificata),
        rigaCancellata: kendoEscapeOggetto(rigaCancellata),
        forzaCancellazioneLegamiAppezzamentiCampi: forzaCancellaLegamiAppezzamentiCampi
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/AggiornaContrattiImpreseParticelle",
        param, false,
        function (risposta) {

            ConfiguraGrigliaRifCatastali(idTabGrigliaRifCatastali);

            // Visto che ogni tanto sparisce, ma qui sono già dentro la tab, quindi se ho fatto una modifica
            // doveva per forza essere già visibile, allora forzo la sua visibilità
            $("#a_tabRifCatastali").show();

            allOk = true;

        },
        function (risposta) {
            if (Array.isArray(risposta.ErroriGias)) {
                let tipoErroreGias = risposta.ErroriGias[0].tipo;
                let messaggioErroreGias = risposta.ErroriGias[0].messaggio;
                if (tipoErroreGias === 3) {
                    RichiediConfermaCancellazioneLegami(messaggioErroreGias);
                } else {
                    let messaggioErrore = "";
                    if (tipoErroreGias === 999) {
                        messaggioErrore = risposta.Errore;
                    } else {
                        messaggioErrore = messaggioErroreGias;
                    }
                    MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi", 0);
                }
            }
        }
     );

    return allOk

}

function RichiediConfermaCancellazioneLegami(messaggioErrore) {

    let titolo = "Cancellazione Legami Particella Catastale";

    let messaggioConferma = ""

    messaggioConferma = "<b>ATTENZIONE!!!</b></br></br>"
    messaggioConferma += messaggioErrore + "</br></br>"
    messaggioConferma += "<b>Procedere alla cancellazione dei suddetti legami con la particella catastale?</b>"

    let kendoConfirm = $("<div></div>").kendoConfirm({
        title: titolo,
        messages: { okText: "Sì", cancel: "No" },
        content: messaggioConferma
    }).data("kendoConfirm");

    kendoConfirm.result.done(function () {
        AggiornaEffettivoRifCatastali(enum_forzaCancellaLegami.Si);
    });

    kendoConfirm.result.fail(function () {
        AggiornaEffettivoRifCatastali(enum_forzaCancellaLegami.No);
    });

    kendoConfirm.open();

}