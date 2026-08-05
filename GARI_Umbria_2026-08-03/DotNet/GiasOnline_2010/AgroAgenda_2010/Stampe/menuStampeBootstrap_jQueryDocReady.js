$(document).ready(function () {

    $.logThis("DocReady: INIZIO");
    // al caricamento della pagina nascondo il box dei parametri di stampa
    // viene mostrato su condizione della stampa richiesta
    $("#stampeParametersContainer").hide();

    //caricamento dati
    caricaStampeAutorizzate();
    promiseStampePreferite = caricaStampePreferite();
    //fine caricamento dati


    // vari controlli che non richiedono dati
    $("#btn_eseguiStampa").click(function () {
        let idStampa = Get_KendoDDLValue("ddRicercaRapida")
        gestisciStampa(idStampa)
    });


    $("#anno").kendoDatePicker({
        start: "decade",
        depth: "decade",
        format: "yyyy",
        value: new Date()
    });
    $("#anno").on('keypress', function (e) {
        var char = String.fromCharCode(e.which);
        if (!(/[0-9]/.test(char))) {
            e.preventDefault();
        }
    });

    $("#btn_aggiungiAPreferiti").click(function () {
        let idStampa = Get_KendoDDLValue("ddRicercaRapida")
        aggiungiPreferiti(idStampa)
    });

    $("#btn_EliminaDaPreferiti").click(function () {
        let idStampa = Get_KendoDDLValue("ddRicercaRapida")
        eliminaPreferiti(idStampa)
    });



    onChangeKendoSwitch = (x) => {
            var dropdownlist = $("#ddRicercaRapida").data("kendoDropDownList");
            if (getKendoSwitch("switchDDL"))
                dropdownlist.dataSource.transport = { read: leggiPreferiti };
            else
                dropdownlist.dataSource.transport= { read: caricaDDLRicercaRapida };
            // Read the new data
            dropdownlist.dataSource.read();
            dropdownlist.select(0);
    }


    $("#txtDataInizio").kendoDatePicker({
        value: new Date(new Date().getFullYear(), 0, 1)
    });
    $("#txtDataFine").kendoDatePicker({
        value: new Date(new Date().getFullYear(), 11, 31)
    });


    creaKendoSwitch("switchDDL", "Preferiti", "Tutti", false, onChangeKendoSwitch)
    creaKendoDropDownList("ddRicercaRapida", { read: caricaDDLRicercaRapida }, "text", "value", undefined, undefined, undefined, undefined, undefined,undefined, undefined, "group");
    creaKendoSwitch("switchGenerale1", "No", "Si", false)
    creaKendoSwitch("switchGenerale2", "No", "Si", false)

    //$("#txtDataInizio").kendoDatePicker();
    //$("#txtDataFine").kendoDatePicker();

    caricaEnumStampe();


    var ddRicercaKendo = $("#ddRicercaRapida").data("kendoDropDownList")
    ddRicercaKendo.bind("change", function (e) {
        //si occupa di cambiare dinamicamente dal bottone aggiungi a preferit a quello elimina preferiti
        //in base a se la stampe e' gia nel elenco preferiti o no
        idStampa = Get_KendoDDLValue("ddRicercaRapida");
        intIdStampa = parseInt(idStampa)
        try {
            $("#annoGroup").hide();
            $("#switchGeneraleGroup").hide();
            $("#switchGeneraleGroup2").hide();
            $("#dataGroup").hide();
            switch (intIdStampa) {
                case enumStampe.Atto_Notorio:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Atto_Notorio * - 1:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Adesione_Etico_Ambientale:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Tenuta_Scheda_Campagna:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Codice_Condotta:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Adesione_DPI:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Impegnativa_Eurep:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Impegnativa_QC:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break 
                case enumStampe.Impegnativa_Confusione_Sessuale:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Allegato_CatastoeValorizzazioni:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Mandato_Trasmissione_Telematica_Dati:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Impegnativa_Orticole_Gest_Annuale:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Impegnativa_Orticole_Gest_Breve:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Impegnativa_Fagiolino_Mercato_Fresco:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Impegnativa_Orticole_Industria:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Impegnativa_Pomodoro_Industria:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                break
                case enumStampe.Adesione_ModuloGrasp:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.Adesione_ProtocolloGlobalGAP:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.Adesione_NurtureModule:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.Adesione_Despar:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.Adesione_Conad:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.Accordo_Responsabilita_di_Filiera:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.Dichiarazione_di_Responsabilita:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.Fitoregolatori_Kiwi:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.Adesione_StandardLeaf:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.SchedaAziendale:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.ImpegnoProduzioneSociDivisoxCentri:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.QuestionarioValutazioneAzienda_Aggiornamento:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup").show();
                break
                case enumStampe.ImpegnativaColtivazioneConferimento:
                    $("#stampeParametersContainer").show();
                    $("#annoGroup").show();
                    $('#label_switch_generale').text('Stampa vuota:');
                    $("#switchGeneraleGroup").show();
                    $('#label_switch_generale2').text('Stampa fronte-retro:');
                    $("#switchGeneraleGroup2").show();
                break
                case enumStampe.ObiettivoDiProduzioneAsipo:
                    $("#stampeParametersContainer").show();
                    $("#dataGroup").show()
                break
                default:
                    $("#stampeParametersContainer").hide();
                    $("#annoGroup").hide();
                    $("#switchGeneraleGroup").hide();
                    $("#switchGeneraleGroup2").hide();
                    $("#dataGroup").hide();

            }
        } catch (x) {
            console.log(x);
            $("#stampeParametersContainer").hide();
            $("#annoGroup").hide();
            $("#switchGeneraleGroup").hide();
            $("#switchGeneraleGroup2").hide();
            $("#dataGroup").hide();
        }


        let flag = true;
        stampePreferite.forEach(element => {
            if (element.value == idStampa) {
                $('#sezioneAggiungiPreferiti').hide();
                $('#sezioneEliminaPreferiti').show();
                flag = false;
                return;
            }
        });
        if (flag) {
            $('#sezioneEliminaPreferiti').hide();
            $('#sezioneAggiungiPreferiti').show();
        }
    });

    ddRicercaKendo.trigger("change");
    $("#btn_gestionePreferiti").click(function () {
        win_gestionePreferiti = $("#win_gestionePreferiti").kendoWindow({
            title: "Gestione Preferiti",
            width: "90%",
            height: "90%",
            closable: true,
            modal: true,
            visible: false,
            resizable: true,
            open: function () {
                onLoad = true;
                this.center();
                imposta_txtRicercaPreferiti();
                popolaElencoPreferiti();
                popolaElencoStampe();
            },
            close: async function (e) {
                onLoad = false;

                if (win_txtRicercaStampe) {
                    win_txtRicercaStampe.value("");
                    win_txtRicercaStampe.destroy();
                }
                if (win_listStampe) {
                    win_listStampe.dataSource.data([]);
                    win_listStampe.destroy();
                    $("#win_listStampe").empty();
                }
                if (win_listPreferiti) {
                    win_listPreferiti.dataSource.data([]);
                    win_listPreferiti.destroy();
                    $("#win_listPreferiti").empty();
                }

                onChangeKendoSwitch();
            }
        }).data("kendoWindow");
        win_gestionePreferiti.open();
    });

    var dropdownHeight = $(window).height() * 0.7;
    ddRicercaKendo.setOptions({ height: dropdownHeight });

    $.logThis("DocReady: FINE");
})