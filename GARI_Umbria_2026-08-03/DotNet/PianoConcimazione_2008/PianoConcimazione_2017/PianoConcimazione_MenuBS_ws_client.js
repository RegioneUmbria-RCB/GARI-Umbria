// #region PIANO DISTRIBUZIONE
function ws_BloccaSbloccaPUA(arrPUA, blocca) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            "arrPUA": arrPUA,
            "blocca": blocca
        });

        ajaxAgronica("PianoConcimazione_MenuBS.aspx/BloccaSbloccaPUA",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}

function infoPD(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    ajaxAgronica("PianoConcimazione_MenuBS.aspx/InfoPD", JSON.stringify({ Ricetta_Cod: dataItem.Ricetta_Cod, Piva: dataItem.Piva }),
        function (risposta) {
            if (risposta.RispostaOK) {
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function modificaPD(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    ajaxAgronica("PianoConcimazione_MenuBS.aspx/ModificaPD", JSON.stringify({ Ricetta_Cod: dataItem.Ricetta_Cod, Piva: dataItem.Piva }),
        function (risposta) {
            if (risposta.RispostaOK) {
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function eliminaPD(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    ajaxAgronica("PianoConcimazione_MenuBS.aspx/EliminaPD", JSON.stringify({ Ricetta_Cod: dataItem.Ricetta_Cod, Piva: dataItem.Piva }),
        function (risposta) {
            if (risposta.RispostaOK) {
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function stampaPD(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    ajaxAgronica("PianoConcimazione_MenuBS.aspx/StampaPD", JSON.stringify({ Ricetta_Cod: dataItem.Ricetta_Cod, Piva: dataItem.Piva }),
        function (risposta) {
            if (risposta.RispostaOK) {
                window.open(risposta.RispostaStringa);
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function creaPianoDistr(dataItem) {
    //            e.preventDefault();

    //            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    var param = JSON.stringify({
        PC_Testata_Cod: dataItem.PC_Testata_Cod,
        PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA,
        PC_Tipo: dataItem.PC_Tipo,
        Regolamento_Tipo: dataItem.Regolamento_Tipo,
        Regolamento_Cod: dataItem.Regolamento_Cod,
        Data_Da: dataItem.Validita_Inizio,
        Data_A: dataItem.Validita_Fine,
        PC_Testata_Des: dataItem.PC_Testata_Des,
        blocco_flag: dataItem.blocco_flag
    });


    ajaxAgronica("PianoConcimazione_MenuBS.aspx/CreaPianoDistribuzione", param,
        function (risposta) {
            if (risposta.RispostaOK) {
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function apriElencoPianoDistr(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $(start_date_dist).val($(Txt_ValiditaInizio).val());
    $(end_date_dist).val($(Txt_ValiditaFine).val());

    var param = JSON.stringify({
        PC_Testata_Cod: dataItem.PC_Testata_Cod,
        PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA,
        PC_Tipo: dataItem.PC_Tipo,
        Regolamento_Tipo: dataItem.Regolamento_Tipo,
        Regolamento_Cod: dataItem.Regolamento_Cod,
        blocco_flag: dataItem.blocco_flag,
        data_inizio: '',
        data_fine: ''
    });


    ajaxAgronicaSync("PianoConcimazione_MenuBS.aspx/LeggiPianiDistribuzione", param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                console.log(risposta.ParametroDue_stringa);
                if (risposta.ParametroDue_stringa === "0") {
                    if (dataItem.Regolamento_Tipo === PUA) { //PUA
                        creaPianoDistr(dataItem);
                    } else {
                        kendo.confirm(TraduzioneMultiResx(MenuBSResx, "NonEsisteNessunPianoDistribuzioneAssociatoCrearneUno", "Non esiste nessun piano distribuzione associato. Vuoi crearne uno?"))
                            .done(function () {
                                creaPianoDistr(dataItem);
                            })
                    }
                } else {

                    if (dataItem.Regolamento_Tipo === PUA) {  //PUA
                        window.location = risposta.RispostaStringa;
                    } else {
                        jSonParsed_Kendo_PianiDistribuzione = JSON.parse(risposta.RispostaStringa);
                        //Creo la Window e la griglia
                        //TODO
                        //kendo.alert("Crea window e Griglia");
                        window_Distribuzione.data("kendoWindow").open();
                        window_Distribuzione.data("kendoWindow").title(TraduzioneMultiResx(MenuBSResx, "ElencoPianiDistribuzioneDi", "Elenco piani distribuzione di") + ": " + dataItem.PC_Testata_Des);

                        GrigliaKendoDistribuzione('kendo_PianiDistribuzione');
                    }
                }
            } else {
                kendo.alert(risposta.Errore);
            }
        }, null);
    current_PC = dataItem;
}

function AggiornaDist() {

    if (current_PC != null) {

        let param = JSON.stringify({
            PC_Testata_Cod: current_PC.PC_Testata_Cod,
            PC_Dettagli_PIVA: current_PC.PC_Dettagli_PIVA,
            data_inizio: $(start_date_dist).val(),
            data_fine: $(end_date_dist).val()
        })

        ajaxAgronicaSync("PianoConcimazione_MenuBS.aspx/LeggiPianiDistribuzione", param, false,
            function (risposta) {
                if (risposta.RispostaOK) {
                    jSonParsed_Kendo_PianiDistribuzione = JSON.parse(risposta.RispostaStringa);
                    GrigliaKendoDistribuzione('kendo_PianiDistribuzione');
                } else {
                    kendo.alert(risposta.Errore);
                }
            }, null);
    } else {
        kendo.alert(TraduzioneMultiResx(MenuBSResx, "ErroreChiudereElencoRiprovare.", "Errore. Chiudere l'elenco e riprovare."));
    }
}

// #endregion

// #region PIANO CONCIMAZIONE / PIANO NUTRIZIONALE
function ws_BloccaSbloccaPC(arrPC, blocca) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            "arrPC": arrPC,
            "blocca": blocca
        });

        ajaxAgronica("PianoConcimazione_MenuBS.aspx/BloccaSbloccaPC",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, false, false);
    });
}

function eliminaPC(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    //let msg = dataItem.Regolamento_Tipo === PUA ? "Sei sicuro di voler eliminare il PUA selezionato?" : "Sei sicuro di voler eliminare il Piano Concimazione selezionato?";
    let msg;
    let Regolamento_Tipo = dataItem.Regolamento_Tipo

    if (Regolamento_Tipo === PUA) { // 1 = Piano concimazione; 2 = PUA; 4 = Piano Nutrizionale Coprob (PRIVATO); 5 = Piano Nutrizionale IBF
        msg = TraduzioneMultiResx(MenuBSResx, "SicuroVolerEliminarePUA", "Sei sicuro di voler eliminare il PUA selezionato?");
    } else if (Regolamento_Tipo === PianoNutrizionale || Regolamento_Tipo === PianoNutrizionale_IBF) {
        msg = TraduzioneMultiResx(MenuBSResx, "SicuroVolerEliminarePianoNutrizionale", "Sei sicuro di voler eliminare il Piano Nutrizionale selezionato?");
    } else {
        msg = TraduzioneMultiResx(MenuBSResx, "SicuroVolerEliminarePianoConcimazione", "Sei sicuro di voler eliminare il Piano Concimazione selezionato?");
    }


    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    kendo.confirm(msg)
        .done(function () {
            ajaxAgronica("PianoConcimazione_MenuBS.aspx/EliminaPC", JSON.stringify({ PC_Testata_Cod: dataItem.PC_Testata_Cod, PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA, Regolamento_Cod: dataItem.Regolamento_Cod, Regolamento_Tipo: Regolamento_Tipo }),
                function (risposta) {
                    if (risposta.RispostaOK) {
                       alert(risposta.RispostaStringa);

                        //MARCOG -> Da sistemare
                        location.reload();


                        //var grid = $("#kendo_PianiConcimazione").data("kendoGrid");
                        //var n = grid.removeRow("dataItem");
                        var dataSource = $("#kendo_PianiConcimazione").data("kendoGrid").dataSource;
                        dataSource.remove(dataItem);
                        dataSource.sync();
                        //grid.refresh();
                    } else {
                        console.log(risposta);
                        alert(risposta.Errore);
                    }
                }, null);
        })
}

function infoPC(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    let param = JSON.stringify({
        PC_Testata_Cod: dataItem.PC_Testata_Cod,
        PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA,
        PC_Tipo: dataItem.PC_Tipo,
        Regolamento_Tipo: dataItem.Regolamento_Tipo, // 1 = Piano Concimazione; 4 = Piano Nutrizionale Coprob (Barbabietola da Zucchero); 5 = Piano Nutrizionale IBF (generale)
        Regolamento_Cod: dataItem.Regolamento_Cod
    })

    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    ajaxAgronica("PianoConcimazione_MenuBS.aspx/InfoPC", param,
        function (risposta) {
            if (risposta.RispostaOK) {
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

//Nuovo PianoConcimazione
function Nuovo(tipo_1_bilancio_3_schede_2_doseStd, singola_azienda, singola_coltura, regolamento_tipo_1_PianoConcimazione_2_PUA) {

    var sceltaImpiantiConFiltroRicercaNG = regolamento_tipo_1_PianoConcimazione_2_PUA == 1 && !singola_azienda && !singola_coltura && usaFiltroRicercaNG;
    if (sceltaImpiantiConFiltroRicercaNG) {
        FiltraImpiantiConFiltroRicercaNG();
    } else {
        ajaxAgronica("PianoConcimazione_MenuBS.aspx/NuovoPC", JSON.stringify({ Tipo: tipo_1_bilancio_3_schede_2_doseStd, SingolaAzienda: singola_azienda, SingolaColtura: singola_coltura, Regolamento_Tipo: regolamento_tipo_1_PianoConcimazione_2_PUA }),
            function (risposta) {
                if (risposta.RispostaOK) {
                    window.location = risposta.RispostaStringa;
                }
                else {
                    console.log(risposta);
                    kendo.alert(risposta.Errore);
                }
            }, null);
    }
}

function defConsistenze(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    ajaxAgronica("PianoConcimazione_MenuBS.aspx/ModificaPC", JSON.stringify({ PC_Testata_Cod: dataItem.PC_Testata_Cod, PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA, PC_Tipo: dataItem.PC_Tipo, Regolamento_Tipo: dataItem.Regolamento_Tipo, Regolamento_Cod: dataItem.Regolamento_Cod, blocco_flag: dataItem.blocco_flag }),
        function (risposta) {
            if (risposta.RispostaOK) {
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function modificaPC(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let param = JSON.stringify({
        PC_Testata_Cod: dataItem.PC_Testata_Cod,
        PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA,
        PC_Tipo: dataItem.PC_Tipo,
        Regolamento_Tipo: dataItem.Regolamento_Tipo, // 1 = Piano Concimazione; 4 = Piano Nutrizionale Coprob (Barbabietola da Zucchero); 5 = Piano Nutrizionale IBF (generale)
        Regolamento_Cod: dataItem.Regolamento_Cod,
        blocco_flag: dataItem.blocco_flag
    })

    ajaxAgronica("PianoConcimazione_MenuBS.aspx/ModificaPC", param,
        function (risposta) {
            if (risposta.RispostaOK) {
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function stampaPC_daDataItem(dataItem) {

    let param = JSON.stringify({
        PC_Testata_Cod: dataItem.PC_Testata_Cod,
        PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA,
        PC_Tipo: dataItem.PC_Tipo,
        Regolamento_Tipo: dataItem.Regolamento_Tipo, // 1 = Piano Concimazione; 4 = Piano Nutrizionale Coprob (Barbabietola da Zucchero); 5 = Piano Nutrizionale IBF (generale)
        Regolamento_Cod: dataItem.Regolamento_Cod
    })

    ajaxAgronica("PianoConcimazione_MenuBS.aspx/StampaPC", param,
        function (risposta) {
            if (risposta.RispostaOK) {
                window.open(risposta.RispostaStringa);
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function apriAllegatoPC(tr_elem, grid_elem, Regolamento_Tipo) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    if (dataItem.Allegati_Documenti_Cod != 0) {
        ajaxAgronica("PianoConcimazione_MenuBS.aspx/apriAllegatoPC",
            JSON.stringify({ PC_Testata_Cod: dataItem.PC_Testata_Cod, PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA, Allegati_Documenti_Cod: dataItem.Allegati_Documenti_Cod }),
            function (risposta) {
                if (risposta.RispostaOK) {
                    window.open(risposta.RispostaStringa);
                }
                else {
                    console.log(risposta);
                    kendo.alert(risposta.Errore);
                }
            }, null);

    } else {

        var msg = ""
        if (Regolamento_Tipo === PUA) { // 1 = Piano concimazione; 2 = PUA; 4 = Piano Nutrizionale Coprob (PRIVATO); 5 = Piano Nutrizionale IBF
            msg = (TraduzioneMultiResx(MenuBSResx, "NessunAllegatoAssociatoPUAProcedereStampa", "Nessun allegato associato al PUA. Vuoi procedere con la stampa?"))
        } else if (Regolamento_Tipo === PianoNutrizionale || Regolamento_Tipo === PianoNutrizionale_IBF) {
            msg = (TraduzioneMultiResx(MenuBSResx, "NessunAllegatoAssociatoPianoNutrizionaleProcedereStampa", "Nessun allegato associato al Piano Nutrizionale. Vuoi procedere con la stampa?"))
        } else {
            msg = (TraduzioneMultiResx(MenuBSResx, "NessunAllegatoAssociatoPianoConcimazioneProcedereStampa", "Nessun allegato associato al Piano di Concimazione. Vuoi procedere con la stampa?"))
        }

        kendo.confirm(msg)
            .done(function () {
                stampaPC_daDataItem(dataItem);
            })
    }
}

// #endregion


//importa comunicazione
function ImportaComunicazione() {
    ajaxAgronica("PianoConcimazione_MenuBS.aspx/ImportaComunicazione", JSON.stringify({}),
        function (risposta) {
            if (risposta.RispostaOK) {
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function stampaRegistroFertilizzazioni_daDataItem(dataItem) {
    ajaxAgronica("PianoConcimazione_MenuBS.aspx/StampaRegistroFertilizzazioni", JSON.stringify({ PC_Testata_Cod: dataItem.PC_Testata_Cod, PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA, PC_Tipo: dataItem.PC_Tipo, Regolamento_Tipo: dataItem.Regolamento_Tipo, Regolamento_Cod: dataItem.Regolamento_Cod }),
        function (risposta) {
            if (risposta.RispostaOK) {
                window.open(risposta.RispostaStringa);
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, null);
}

function creaPratiche(strAnni, servizio_cod) {
    return new Promise((resolve, reject) => {
        ajaxAgronica("PianoConcimazione_MenuBS.aspx/CreaPratiche",
            JSON.stringify({ strAnni: strAnni, servizio_cod: servizio_cod }),
            function (risposta) {
                if (risposta.RispostaOK == true) {
                    kendo.alert(TraduzioneMultiResx(MenuBSResx, "PraticaCreataCorrettamente", "Pratica creata correttamente"));
                }
                resolve(risposta);
            }, null);
    });
}

function verificaIndiciBilancio(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    $(start_date_dist).val($(Txt_ValiditaInizio).val());
    $(end_date_dist).val($(Txt_ValiditaFine).val());

    var param = JSON.stringify({
        PC_Testata_Cod: dataItem.PC_Testata_Cod,
        PC_Dettagli_PIVA: dataItem.PC_Dettagli_PIVA,
        PC_Tipo: dataItem.PC_Tipo,
        Regolamento_Tipo: dataItem.Regolamento_Tipo,
        Regolamento_Cod: dataItem.Regolamento_Cod,
        blocco_flag: dataItem.blocco_flag,
        data_inizio: '',
        data_fine: ''
    });

    // Valido solo nel caso di PUA
    ajaxAgronicaSync("PianoConcimazione_MenuBS.aspx/ApriVerificaIndiciBilancio", param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                console.log(risposta.ParametroDue_stringa);
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                kendo.alert(risposta.Errore);
            }
        }, function (risposta) {
            kendo.alert(risposta.Errore);
        });
    current_PC = dataItem;
}


