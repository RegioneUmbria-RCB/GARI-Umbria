
/* RegCaricoScaricoPassaporti_ws_client */

var indirizzohttp = "./RegCaricoScaricoPassaporti.aspx";

function LeggiAnnataAgraria() {

    ajaxAgronica(indirizzohttp + "/LeggiAnnataAgraria", "{ }",
        function (risposta) {
            var xRisp = JSON.parse(risposta.RispostaStringa);

            $("#Txt_Data_DA").val(xRisp.Txt_Data_DA);
            $("#Txt_Data_A").val(xRisp.Txt_Data_A);


        }, null);
}


function RicercaRigheRegistroPassaporti(options) {
    var risp = JSON.parse($("#hdKendoRegistroPassaporti").val());
    options.success(risp.kendo_rows);
}

function RecuperaClickImpostaModalita() {
    $("#messaggi").html(TraduzioneMultiResx(resxObj, "ModalitaDatiCancellati", "Stai Lavorando in modalità: Recupera Dati Cancellati o Incongruenti"));
    $("#btn_recupera").addClass("pulsanteDisabilitato");

}

function AggiornaClickImpostaModalita() {
    $("#messaggi").html("");
    $("#btn_recupera").removeClass("pulsanteDisabilitato");
}

/**
 * chiamata server
 * @param {boolean} RiportoDocumenti Attiva il riporto delle righe dai documenti contabili/lavorazioni GIAS
 * @param {boolean} RecuperaRigheRegistro Recupera le righe di registro cancellate o incongruenti
 * @param {boolean} CambiaModalita Cambia la modalità fra Aggiorna/Recupera
 */
function RicercaRigheRegistroPassaportiLettura(RiportoDocumenti, RecuperaRigheRegistro, CambiaModalita) {

    if (RecuperaRigheRegistro) {
        UltimoPulsantePremuto = enum_ultimoPulsantePremuto.Recupera;
        if (CambiaModalita) {
            RecuperaClickImpostaModalita();
        }
    } else {
        UltimoPulsantePremuto = enum_ultimoPulsantePremuto.Aggiorna;
        if (CambiaModalita) {
            AggiornaClickImpostaModalita();
        }
    }
     
    var param = "{ piva: '" + piva + "', DataDa: '" + $("#Txt_Data_DA").val() + "', DataA: '" + $("#Txt_Data_A").val() + "', RiportoDocumenti: " + RiportoDocumenti.toString() + ", RecuperaRigheRegistro: " + RecuperaRigheRegistro.toString() +  "}";

    ajaxAgronica(indirizzohttp + "/RegistroPassaportiLettura",
        param,
        function (risposta) {

            $("#hdKendoRegistroPassaporti").val(risposta.RispostaStringa);

            if (risposta.ParametroDue_stringa !== "") {
                //i18n
                kendo.alert("Si è verificato un errore in fase di riporto delle operazioni GIAS:" + risposta.ParametroDue_stringa);
            }
            kendoGridFlatCaricoScaricoPassaporti("kendoRegistroPassaporti");

        }, null);
}


function cliPassaportoVivaiAggiornaDatiSrv_test(testD) {
    var msgCampo = "";
    //i18n Amoroso: Secondo me non necessario tradurre in quanto controlli di validità precedenti evitano questi messaggi di errore
    if (testD.Codice_RUOP === "") {
        msgCampo = msgCampo + "Codice RUOP, ";
    }
    if (testD.TipoZona === "") {
        msgCampo = msgCampo + "Tipo Zona, ";
    }
    if (testD.PaeseDiOrigine === "") {
        msgCampo = msgCampo + "Paese di Origine, ";
    }
    if (testD.Causale === "") {
        msgCampo = msgCampo + "Causale, ";
    }
    if (testD.CaricoScarico === "") {
        msgCampo = msgCampo + "Carico/Scarico, ";
    }
    if (testD.Destinazione === "") {
        msgCampo = msgCampo + "Destinazione, ";
    }
    if (testD.Veg_Des_Lat === "") {
        msgCampo = msgCampo + "Specie/Cultivar, ";
    }
    if (testD.Lotto_1 === "") {
        msgCampo = msgCampo + "Lotto_1, ";
    }
    return msgCampo;
}
function cliPassaportoVivaiAggiornaDatiSrv() {

    //DEBUG_GABRIELE
    // effettuate modifiche dopo l'implementazione della funzioneCRUD -> funzioneSubmit

    var grid = $("#kendoRegistroPassaporti").data("kendoGrid");
    var currentData = grid.dataSource.data();


    //get the new and the updated records
    var currentData = grid.dataSource.data();
    var foundErr = false;
    var messaggeErr = "";
    // controllo che non ci siano due righe uguali
    for (x = 0; x < currentData.length; x++) {
        var testD = currentData[x];
        if (testD.isNew() || testD.dirty) {

            var msgCampo = cliPassaportoVivaiAggiornaDatiSrv_test(testD);
            if (msgCampo !== "") {
                //i18n Amoroso: Secondo me non necessario tradurre in quanto controlli di validità precedenti evitano questi messaggi di errore
                messaggeErr += "Riga: " + x.toString() +  ": Completare i dati obbligatori per procedere: " + msgCampo;
                foundErr = true;
            }

        }
    }

    if (foundErr) {
        kendo.alert(messaggeErr);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    for (var i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            currentData[i].Vivaista_PIVA = piva;
            //currentData[i].GIAS_Stato = enum_wAnagraficaStati_PP.Confermato.value;
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        var righeInserite = JSON.stringify(newRecords).replace(/'/g, "\\'");;
        var righeModificate = JSON.stringify(updatedRecords).replace(/'/g, "\\'");;
        var righeCancellate = JSON.stringify(deletedRecords).replace(/'/g, "\\'");;
        var allOk = false;


        var bRecuperaRigheRegistro = false;
        if (UltimoPulsantePremuto.value === enum_ultimoPulsantePremuto.Recupera.value) {
            bRecuperaRigheRegistro = true;
        }

        var param = "{ righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "', RecuperaRigheRegistro: " + bRecuperaRigheRegistro + " }";

        ajaxAgronicaSync(indirizzohttp + "/PassaportoVivaiAggiornaDatiSrv", param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];

                var pj = JSON.parse($("#hdKendoRegistroPassaporti").val());
                pj.kendo_rows = currentData;
                $("#hdKendoRegistroPassaporti").val(JSON.stringify(pj));

                //grid.dataSource.read();
                //grid.refresh();
                RicercaRigheRegistroPassaportiLettura(false, false, !bRecuperaRigheRegistro);
                kendo.alert(TraduzioneMultiResx(resxObj, "SalvataggioEffettuatoCorrettamente", "Salvataggio effettuato correttamente"));
            }, null);

        if (!allOk) {
            if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
                var dsSort = [];
                //dsSort.push({ field: "Tabella_Des", dir: "asc" });
                //dsSort.push({ field: "val_des", dir: "asc" });

                // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
                // che non si vedono più
                ripristinaRigheCancellateKendoGrid(grid, dsSort);
            }
        }
    }
}

function confermaVoceRegistro(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var bRecuperaRigheRegistro = false;
    if (UltimoPulsantePremuto.value === enum_ultimoPulsantePremuto.Recupera.value) {
        bRecuperaRigheRegistro = true;
    }
    var param = " { piva: '" + piva + "', VivaiPassaporti_Operazione_cod: " + datiRiga.ws_VivaiPassaporti_Operazione_cod + ", RecuperaRigheRegistro: " + bRecuperaRigheRegistro + " }";

    ajaxAgronica(indirizzohttp + "/ConfermaVoceRegistro",
        param,
        function (risposta) {
            
            switch (datiRiga.Azione) { 
                case "PEN":
                    RicercaRigheRegistroPassaportiLettura(false, false, !bRecuperaRigheRegistro);
                    break;
                case "ASS":
                    RicercaRigheRegistroPassaportiLettura(false, false, !bRecuperaRigheRegistro);
                    break;
                case "MOD":
                    RicercaRigheRegistroPassaportiLettura(false, false, !bRecuperaRigheRegistro);
                    break;
                default:
                    $(tr_elem).removeClass("kendoRiga_pp_" + datiRiga.statoColore);
                    $($(tr_elem).children("td")[2]).html("Confirmed");
                    datiRiga.GIAS_Stato = enum_wAnagraficaStati_PP.Confermato.value;
                    datiRiga.WAnagraficaStati_DES = enum_wAnagraficaStati_PP.Confermato.name;
                    ppAttivaDisattivaPulsanti(enum_wAnagraficaStati_PP.Confermato.value, tr_elem);
                    break;
            }
        }, null);
}


function stampaPassaporto(tr_elem, grid_elem) {


    var locwsVivaiPassaportiOperazioneCod;
    if (tr_elem) {

        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);
        locwsVivaiPassaportiOperazioneCod = datiRiga.ws_VivaiPassaporti_Operazione_cod;
    } else {
        locwsVivaiPassaportiOperazioneCod = -1;
    }


    var param = " { piva: '" + piva + "', VivaiPassaporti_Operazione_cod: " + locwsVivaiPassaportiOperazioneCod + " }" ;

    ajaxAgronica(indirizzohttp + "/StampaPassaporto",
        param,
        function (risposta) {
            //console.log(risposta);
            //alert(risposta);


            //utility.log(indirizzohttp);

            StampaPassaportoApriStampe(risposta.RispostaStringa);

        }, null);
}

function StampaPassaportoApriStampe(rispostaStringa) {

    let w_w = window.outerWidth;
    let w_h = window.outerHeight;

    w_w = w_w * 0.95;
    w_h = w_h * 0.75;


    KendoWindowGenericApri(rispostaStringa, TraduzioneMultiResx(resxObj, "StampaDelPassaporto", "Stampa del passaporto"), undefined, parseInt(w_w), parseInt(w_h), 10, 10);

}