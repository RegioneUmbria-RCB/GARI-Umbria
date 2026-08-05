var Cmb_Sezionali;
var Cmb_AspettoBeni;
var Cmb_CausaleContabilita;
var Cmb_CausaleTrasporto;
var Cmb_Contatto1;
var Cmb_Contatto2;
var Cmb_Coop1;
var Cmb_Coop2;
var Cmb_Agente;
var Cmb_CapoArea;
var Cmb_GestioneVettore;
var Cmb_Dipendente;
var Cmb_Vettore;
var Cmb_TipoIndVettore;
var Cmb_AccModalitaTrasporto;
var Cmb_AccUnitaTrasporto;
var Cmb_MezzoTrasporto;
var Cmb_Operatore;
var Cmb_TrasportoUdm;
var Cmb_ModPagamento;

var filtroPersona = [{ field: "Rag_Soc_Completa" }, { field: "Cod_Contatto" }, { field: "Attivita_Des" }, { field: "Progressivo" }];

function get_Cmb_Sezionale(idControllo) {
    return new Promise(function(resolve, reject) {

        try {
            creaKendoDropDownList(idControllo,
                { read: LeggiSezionali },
                "Sezionale_Des", "Sezionale_Cod",
                "contains", null, null, false
            );

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }

    });
}

function get_Cmb_ModPagamento(idControllo) {
    return new Promise(function(resolve, reject) {

        try {
            creaKendoDropDownList(idControllo,
                { read: LeggiModPagamento },
                "Cau_Pagamento_Sigla", "Cau_Pagamento",
                "contains", null, null, false
            );

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }

    });
}

function get_Cmb_AspettoBeni(idControllo) {
    return new Promise(function(resolve, reject) {
        try {

            creaKendoDropDownList(idControllo,
                { read: LeggiAspettoBeni },
                "AspettoBene_Des", "AspettoBene_Cod",
                "contains", null, null, false
            );

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }

    });
}

function get_Cmb_CausaleTrasporto(idControllo) {

    return new Promise(function (resolve, reject) {
        try {
            creaKendoDropDownList(
                idControllo,
                { read: LeggiCausaliTrasporto },
                "Causale_Trasporto_Des", "Causale_Trasporto_Cod",
                "contains", null,
                $("#noDataTemplateCausaleTrasporto").html(),
                false
            );

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });
}



function get_Cmb_TipoDocumento(idControllo) {

    return new Promise(function (resolve, reject) {
        try {
            creaKendoDropDownList(
                idControllo,
                { read: LeggiTipoDocumento },
                "xDescrizione", "Codice",
                "contains", null,
                $("#noDataTemplateTipoDocumento").html(),
                false
            );

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {

                ddl.bind("change", function (ev) {
                    // devo rendere obbligatori i campi nota fattura per alcuni valori specifici
                    let dataItemSel = ev.sender.dataItem();

                    if (dataItemSel !== undefined) {
                        let valSel = dataItemSel.Codice;
                        let sdiFatturaObbl = false;
                        /* I valori provengono dalla tabella TipologieDocumento, non abbiamo un TipoEnumerativo associato:
                        TD05 Nota di Debito
                        TD16 Integrazione fattura reverse charge interno
                        TD17 Integrazione/autofattura per acquisto servizi dall'estero
                        TD18 Integrazione per acquisto di beni intracomunitari */
                        let aePerSDIObbl = ["5", "16", "17", "18"];
                        if (aePerSDIObbl.includes(valSel)) {
                            sdiFatturaObbl = true;
                        }
                        ImpostaVisibilitaNotaFattura(true, sdiFatturaObbl);
                        VerificaDatiMinimiTestata();
                    }

                });

                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });
}


function get_Cmb_Contatto1(idControllo) {

    return new Promise(function (resolve, reject) {
        try {

            let idTipoInd1 = "inTipoIndirizzoCC1";

            let idCedCes2 = ddlContatto2Nome;
            let idTipoInd2 = "inTipoIndirizzoCC2";

            let idVettore = "inVettore";
            let idAgenti = "inAgente";
            let idCapoArea = "inCapoArea";

            let ruoloAccettazione = "";
            if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
                ruoloAccettazione = "Conferente";
            }

            let _checkRaccolte = false;
            if (raccolteXConferimenti_AbilitazioneGenerale(false) && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value) {
                _checkRaccolte = true;
            }

            // Per il contatto1 in lettura o in modifica evito di rileggere tutti i contatti
            let _cod_risum_contatto1 = 0;
            if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value ||
                cIdTipoOp === enum_TipoOperazioneDB.Modifica.value) {
                let docData = JSON.parse($('input[name$="hdKendo_TestataDoc"]').val());
                _cod_risum_contatto1 = docData.CodRisUm;
            }
            
            //creaKendoDropDownList(IDControllo, t, textfield, valuefield, filterType, filteringArray, noDataTemplate, autoBind, template, valueTemplate, autoWidth)
            creaKendoDropDownList(idControllo,
                { read: LeggiCessionariCedentiAsync, data: { idControllo: idControllo, ruoloAccettazione: ruoloAccettazione, checkRaccolte: _checkRaccolte, cod_risum_contatto: _cod_risum_contatto1 } },
                "Rag_Soc_Completa",
                "Cod_RisUm",
                "contains",
                filtroPersona,
                null,
                false,
                null,
                null,
                false,
                null,
                null,
                true
            ).bind("change",
                {
                    idTipoIndirizzo: idTipoInd1,
                    cc: "CC1",
                    ruoloAccettazione: ruoloAccettazione,
                    idCedCesDestDiv: idCedCes2,
                    idTipoIndDestDiv: idTipoInd2,
                    idVettore: idVettore,
                    idAgente: idAgenti,
                    idCapoArea: idCapoArea
                },
                inCessionarioCedente_change);

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });

}

function get_Cmb_Contatto2(idControllo) {

    return new Promise(function (resolve, reject) {
        try {

            let idTipoInd2 = "inTipoIndirizzoCC2";

            let ruoloAccettazione = "";
            if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
                ruoloAccettazione = "Produttore";
            }

            let _checkRaccolte = false;
            if (raccolteXConferimenti_AbilitazioneGenerale(false) && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value) {
                _checkRaccolte = true;
            }

            // Per il contatto2 in lettura evito di rileggere tutti i contatti
            let _cod_risum_contatto2 = 0;
            if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
                let docData = JSON.parse($('input[name$="hdKendo_TestataDoc"]').val());
                _cod_risum_contatto2 = docData.CodDestinazione;
            }

            creaKendoDropDownList(idControllo,
                { read: ruoloAccettazione === "" ? LeggiCessionariCedentiAsync : LeggiConferentiGerarchiaSync, data: { idControllo: idControllo, ruoloAccettazione: ruoloAccettazione, checkRaccolte: _checkRaccolte, cod_risum_contatto: _cod_risum_contatto2 } },
                "Rag_Soc_Completa",
                "Cod_RisUm",
                "contains", filtroPersona,
                null, false, null, null, null, null, null, true
            ).bind("change",
                { idTipoIndirizzo: idTipoInd2, cc: "CC2", ruoloAccettazione: ruoloAccettazione },
                inCessionarioCedente_change);

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });

}

function create_Cmb_Contatto1Sync(idControllo) {

    let idTipoInd1 = "inTipoIndirizzoCC1";

    let idCedCes2 = ddlContatto2Nome;
    let idTipoInd2 = "inTipoIndirizzoCC2";

    let idVettore = "inVettore";
    let idAgenti = "inAgente";
    let idCapoArea = "inCapoArea";

    let ruoloAccettazione = "";
    if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
        ruoloAccettazione = "Conferente";
    }

    let _checkRaccolte = false;
    if (raccolteXConferimenti_AbilitazioneGenerale(false) && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value) {
        _checkRaccolte = true;
    }

    let noDataTemplate = kendo.format(TraduzioneMultiResx(resxObj, "InserireNCaratteriNessunRisultatoPerDato",
        ""), LunghezzaMinimaFiltroContatto.toString());

    creaKendoDropDownListServerFilteringComplete(idControllo,
        { read: LeggiCessionariCedentiSync, data: { idControllo: idControllo, ruoloAccettazione: ruoloAccettazione, checkRaccolte: _checkRaccolte, cod_risum_contatto: 0 } },
        "Rag_Soc_Completa",
        "Cod_RisUm",
        "contains", 
        noDataTemplate, false, null, null,
        false,
        null, null, true, null, LunghezzaMinimaFiltroContatto
    ).bind("change",
        {
            idTipoIndirizzo: idTipoInd1,
            cc: "CC1",
            ruoloAccettazione: ruoloAccettazione,
            idCedCesDestDiv: idCedCes2,
            idTipoIndDestDiv: idTipoInd2,
            idVettore: idVettore,
            idAgente: idAgenti,
            idCapoArea: idCapoArea
        },
        inCessionarioCedente_change);

    let ddl = KendoDDL(idControllo);
    if (ddl === undefined || ddl === null) {
        throw new Error("Non è stato possibile creare " + idControllo);
    }
    ddl.bind("filtering", ddlContatto_filtering);
}

function create_Cmb_Contatto2Sync(idControllo) {

    let idTipoInd2 = "inTipoIndirizzoCC2";

    let ruoloAccettazione = "";
    if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
        ruoloAccettazione = "Produttore";
    }

    let _checkRaccolte = false;
    if (raccolteXConferimenti_AbilitazioneGenerale(false) && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value) {
        _checkRaccolte = true;
    }

    // Per il contatto2 se sono in modifica evito di rileggere tutti i contatti
    // N.B. se arrivo qui sono in modalità serverFilternig
    let _cod_risum_contatto2 = 0;
    if (cIdTipoOp === enum_TipoOperazioneDB.Modifica.value) {
        let docData = JSON.parse($('input[name$="hdKendo_TestataDoc"]').val());
        _cod_risum_contatto2 = docData.CodDestinazione;
    }

    let noDataTemplate = kendo.format(TraduzioneMultiResx(resxObj, "InserireNCaratteriNessunRisultatoPerDato",
        ""), LunghezzaMinimaFiltroContatto.toString());

    creaKendoDropDownListServerFilteringComplete(idControllo,
        { read: ruoloAccettazione === "" ? LeggiCessionariCedentiSync : LeggiConferentiGerarchiaSync, data: { idControllo: idControllo, ruoloAccettazione: ruoloAccettazione, checkRaccolte: _checkRaccolte, cod_risum_contatto: _cod_risum_contatto2 } },
        "Rag_Soc_Completa",
        "Cod_RisUm",
        "contains", 
        noDataTemplate, false, null, null,
        false,
        null, null, true, null, LunghezzaMinimaFiltroContatto
    ).bind("change",
        { idTipoIndirizzo: idTipoInd2, cc: "CC2", ruoloAccettazione: ruoloAccettazione },
        inCessionarioCedente_change);

    let ddl = KendoDDL(idControllo);
    if (ddl === undefined || ddl === null) {
        throw new Error("Non è stato possibile creare " + idControllo);
    }
    ddl.bind("filtering", ddlContatto_filtering);

}

function ddlContatto_filtering(e) {

    var filter = e.filter;

    if (filter === undefined ||
        (filter !== undefined && (!filter.value || filter.value.length < LunghezzaMinimaFiltroContatto))) {

        //prevent filtering if the filter does not value
        e.preventDefault();

    }
     
}

function get_Cmb_ContattoCoop1(idControllo) {

    return new Promise(function (resolve, reject) {
        try {

            let ruoloAccettazione = "";
            if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
                ruoloAccettazione = "Coop1";
            }

            creaKendoDropDownList(idControllo,
                { read: LeggiConferentiGerarchiaSync, data: { idControllo: idControllo, ruoloAccettazione: ruoloAccettazione } },
                "Rag_Soc_Completa",
                "Cod_RisUm",
                "contains", filtroPersona,
                null, false
            ).bind("change",
                { cc: "CC3", ruoloAccettazione: ruoloAccettazione },
                inCessionarioCedente_change);

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });

}

function get_Cmb_ContattoCoop2(idControllo) {

    return new Promise(function (resolve, reject) {
        try {

            let ruoloAccettazione = "";
            if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
                ruoloAccettazione = "Coop2";
            }

            creaKendoDropDownList(idControllo,
                { read: LeggiConferentiGerarchiaSync, data: { idControllo: idControllo, ruoloAccettazione: ruoloAccettazione } },
                "Rag_Soc_Completa",
                "Cod_RisUm",
                "contains", filtroPersona,
                null, false
            ).bind("change",
                { cc: "CC4", ruoloAccettazione: ruoloAccettazione },
                inCessionarioCedente_change);

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });

}

function get_Cmb_Agente(idControllo) {

    return new Promise(function (resolve, reject) {
        try {

            creaKendoDropDownList(idControllo,
                { read: LeggiAgentiCapoAreaTerzisti, data: { idControllo: idControllo, tipoRapporto: enum_TipoRapporto.Agenti } },
                "Rag_Soc_Completa", "Cod_RisUm",
                null, null, null, false);

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });

}

function get_Cmb_CapoArea(idControllo) {

    return new Promise(function (resolve, reject) {
        try {

            creaKendoDropDownList(idControllo,
                { read: LeggiAgentiCapoAreaTerzisti, data: { idControllo: idControllo, tipoRapporto: enum_TipoRapporto.CapoArea } },
                "Rag_Soc_Completa", "Cod_RisUm",
                null, null, null, false);

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });

}

function get_Cmb_TrasportoUdm(idControllo) {

    return new Promise(function (resolve, reject) {
        try {

            creaKendoDropDownList(idControllo,
                { read: LeggiUnitaMisuraTrasporto, data: { idControllo: idControllo } },
                "UDM_DES", "UDM_COD",
                null, null, null, false);

            let ddl = KendoDDL(idControllo);
            if (ddl === undefined || ddl === null) {
                throw new Error("Non è stato possibile creare " + idControllo);
            } else {
                resolve(ddl);
            }

        } catch (err) {
            let msgError = "ERRORE in creazione " + idControllo + ": " + err;
            console.error(err);
            reject(msgError);
        }
    });

}


function creaBtnFiltraContattiXRaccolte(idBtn, idCmbContatti) {
    let btnJQuery = $("#" + idBtn);

    btnJQuery.show();

    btnJQuery.kendoTooltip({ content: TraduzioneMultiResx(resxObj, "MostraContattiRaccolte", "Mostra solo i contatti con raccolte assegnabili") });

    btnJQuery.addClass("btnFiltroNonAttivo");
    btnJQuery.data("filtroAttivo", false);

    btnJQuery.on("click", { idCmb: idCmbContatti }, function (ev) {

        if ($(ev.currentTarget).data("filtroAttivo") === false) {
            // Applico il filtro
            KendoDDL(ev.data.idCmb).dataSource.filter({
                logic: "or",
                filters: [
                    { field: "Cod_RisUm", operator: "eq", value: 0 },
                    { field: "LavCod_Raccolta", operator: "eq", value: 125 }
                ]
            });
            KendoDDL(ev.data.idCmb).select(0);
            $(ev.currentTarget).data("filtroAttivo", true);
            $(ev.currentTarget).removeClass("btnFiltroNonAttivo");

            btnJQuery.data("kendoTooltip").destroy();
            btnJQuery.kendoTooltip({ content: TraduzioneMultiResx(resxObj, "MostraTuttiContatti", "Mostra tutti i contatti") });
        }
        else {
            // Tolgo il filtro
            KendoDDL(ev.data.idCmb).dataSource.filter({});
            $(ev.currentTarget).data("filtroAttivo", false);
            $(ev.currentTarget).addClass("btnFiltroNonAttivo");

            btnJQuery.data("kendoTooltip").destroy();
            btnJQuery.kendoTooltip({ content: TraduzioneMultiResx(resxObj, "MostraContattiRaccolte", "Mostra solo i contatti con raccolte assegnabili") });
        }

    });
    
}

function creaBtnLockContattiXCompliantISCC(idBtn, idCmbContatti) {
    let btnJQuery = $("#" + idBtn);

    btnJQuery.show();

    if (btnJQuery.data("kendoTooltip") == undefined) {

        btnJQuery.kendoTooltip({ content: TraduzioneMultiResx(resxObj, "SbloccaInserimentoPerISCC", "Sblocca Inserimento per ISCC") });

        btnJQuery.addClass("btnFiltroNonAttivo");
        btnJQuery.data("filtroAttivo", false);

        btnJQuery.on("click", { idCmb: idCmbContatti }, function (ev) {

            if ($(ev.currentTarget).data("filtroAttivo") === false) {

                let inputISCC = "";
                let boxISCC = "";

                if (ev.data.idCmb == ddlContatto1Nome) {
                    inputISCC = "reqCompliantISCC_Contatto1";
                    boxISCC = "boxCompliantISCC_Contatto1";
                }
                else if (ev.data.idCmb == ddlContatto2Nome) {
                    inputISCC = "reqCompliantISCC_Contatto2";
                    boxISCC = "boxCompliantISCC_Contatto2";
                }

                //$("#" + inputISCC).attr("required", false);
                $("#" + inputISCC).removeClass("ISCCNonConforme");
                $("#" + boxISCC).removeClass("k-invalid");

                VerificaDatiMinimiTestata();

                $(ev.currentTarget).data("filtroAttivo", true);
                $(ev.currentTarget).removeClass("btnFiltroNonAttivo");
                $(ev.currentTarget).attr("disabled", true);
            }

        });
    }
    else {
        btnJQuery.attr("disabled", false);
        btnJQuery.addClass("btnFiltroNonAttivo");
        btnJQuery.data("filtroAttivo", false);
    }

}

/** Restituisce zero se la gestione è disabilitata, uno altrimenti */
function imputazioneImpianti_AbilitazioneGenerale() {
    let imputImpiantiAbilitata = 0;

    if (lavCodAccettazione === true) {

        if (imputazioneImpianti_gestioneAbilitata === null) {
            // Leggo l'impostazione per la prima volta
            imputazioneImpianti_gestioneAbilitata = imputazioneImpianti_GetImpostazione($(cIdPiva).val(), Qs_SaCod);

            if (imputazioneImpianti_gestioneAbilitata === "") {
                imputazioneImpianti_gestioneAbilitata = 1;
            }
            else {
                imputazioneImpianti_gestioneAbilitata = parseInt(imputazioneImpianti_gestioneAbilitata);

                if (isNaN(imputazioneImpianti_gestioneAbilitata)) {
                    imputazioneImpianti_gestioneAbilitata = 1;
                }
            }
        }

        imputImpiantiAbilitata = imputazioneImpianti_gestioneAbilitata;
    }

    return imputImpiantiAbilitata;
}

/** Restituisce zero se la gestione è disabilitata, uno altrimenti */
function raccolteXConferimenti_AbilitazioneGenerale(gestioneWaitFrame) {

    if (gestioneWaitFrame == undefined || gestioneWaitFrame == null) {
        gestioneWaitFrame = true;
    }

    let raccolteConfAbilitate = 0;

    if (lavCodAccettazione === true && lavCodAccettazionePomodoro === false) {

        if (raccolteXConferimenti_gestioneAbilitata === null) {
            // Leggo l'impostazione per la prima volta
            raccolteXConferimenti_gestioneAbilitata = raccolteXConferimenti_GetImpostazione($(cIdPiva).val(), Qs_SaCod, gestioneWaitFrame);

            if (raccolteXConferimenti_gestioneAbilitata === "") {
                raccolteXConferimenti_gestioneAbilitata = 1;
            }
            else {
                raccolteXConferimenti_gestioneAbilitata = parseInt(raccolteXConferimenti_gestioneAbilitata);

                if (isNaN(raccolteXConferimenti_gestioneAbilitata)) {
                    raccolteXConferimenti_gestioneAbilitata = 1;
                }
            }
        }

        raccolteConfAbilitate = raccolteXConferimenti_gestioneAbilitata;
    }

    return raccolteConfAbilitate;
}