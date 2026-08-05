function btnScaricaTemplIscrClick(ev) {
    var param = kendo.stringify({
        objP_server: $("[name='hfObjPServer']").val(),
        objP_utenti: $("[name='hfObjPUtenti']").val(),
        piva: $("[name='hfPivaSuperServer']").val(),
        allegati_documenti_cod: ev.data.Allegati_Documenti_Cod
    });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_File_Allegato", param,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            let fileDati = risp[0].File_Allegato_DB;
            let nomeFile = risp[0].Allegati_Documenti_NomeFile;

            SaveAndOpenFileByteArray(nomeFile, fileDati);

        }, null);
}

function fileCaricaDocumentoChange(ev) {
    var file = ev.target.files[0];
    if (file) {
        $("#txtCaricaDocumento").val(file.name);

        var reader = new FileReader();
        reader.onload = function (readerEvt) {
            var binaryString = readerEvt.target.result;
            allegatoInBase64 = btoa(binaryString);
        };
        reader.readAsBinaryString(file);
    }
}

function ddlTipoFirmaRead(options) {
    var arrQual = [
        { Value: 0, Des: "" },
        { Value: 1, Des: TraduzioneMultiResx(resxObj, "FirmaAutografa", "Firma Autografa") },
        { Value: 2, Des: TraduzioneMultiResx(resxObj, "FirmaDigitale", "Firma Digitale") },
    ];
    options.success(arrQual);
}

// Creazione dinamica indici
function RicercaIndicixTipologia(id_area, id_tipologia) {

    var piva = $("[name='hfPivaSuperServer']").val();

    if (piva !== "" && piva !== undefined) {


        var elenco = "";
        if (id_area == "" || id_area == undefined) {
            id_area = 0;
        }

        if (id_tipologia == "" || id_tipologia == undefined) {
            id_tipologia = 0;
        }

        var param = kendo.stringify({ objP_server: $("[name='hfObjPServer']").val(), objP_utenti: $("[name='hfObjPUtenti']").val(), piva: piva, id_area: id_area, id_tipologia: id_tipologia });

        ajaxAgronicaSync("../GestioneAllegati/GestioneAllegati.aspx/RicercaIndicixTipologia",
            param, false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);

                elenco = risp;
            }, null);

        return elenco;

    }
}

function RicercaDDLIndice(id_indice, tipocampo, elenco_tipo, elenco_cod) {

    var elencoindici = "";
    var piva = $("[name='hfPivaSuperServer']").val();
    var data_riferimento = "";

    var param = kendo.stringify({ objP_server: $("[name='hfObjPServer']").val(), objP_utenti: $("[name='hfObjPUtenti']").val(), piva: piva, id_indice: id_indice, tipocampo: tipocampo, elenco_tipo: elenco_tipo, elenco_cod: elenco_cod, data_riferimento: data_riferimento});

    ajaxAgronicaSync("../GestioneAllegati/GestioneAllegati.aspx/RicercaDDLIndice",
        param, false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);

            var objVuoto = { "Valore_Cod": 0, "Valore_Des": "" };
            risp.unshift(objVuoto);

            elencoindici = risp;
        }, null);

    return elencoindici;
}

function CreaIndici(id_area, id_tipologia) {

    let container = document.getElementById("boxIndici");
    let contaRighe = 0;
    let nrRighe = 0;
    let colonnePerRiga = 3;
    let colBootstrap = 12 / colonnePerRiga;
    let newRowDiv = null;
    var piva = $("[name='hfPivaSuperServer']").val();
    var id_elenco = "";

    elencoindici = RicercaIndicixTipologia(id_area, id_tipologia);

    for (var iindice = 0; iindice < elencoindici.length; iindice++) {
        if (elencoindici[iindice].ID_Indice !== 0) {
            if (elencoindici[iindice].TipoCampo == 0 ||
                elencoindici[iindice].TipoCampo == 1 ||
                elencoindici[iindice].TipoCampo == 2) {

                if (contaRighe == colonnePerRiga) {
                    contaRighe = 0;
                }

                if (contaRighe == 0) {
                    if (newRowDiv !== null) {
                        container.appendChild(newRowDiv);
                    }
                    nrRighe++;
                    newRowDiv = creaNewRowDiv("id" + nrRighe);
                }


                var newColumnDiv = creaNewColumnBS(colBootstrap, colBootstrap, colBootstrap);
                var newDivInputGroup = creaDIV("input-group");
                var newKey = elencoindici[iindice].TipoCampo + "_" + elencoindici[iindice].ID_Indice;

                //Impostazione Titolo Indice
                var titoloindice = elencoindici[iindice].TitoloIndice;
                if (parseInt(elencoindici[iindice].ChkObbligatorio) == 1 || parseInt(elencoindici[iindice].ChkObbligatorio_Tipologia) == 1) {
                    titoloindice = titoloindice + "*";
                }


                let tipo_param = "";
                if (parseInt(elencoindici[iindice].TipoCampo) == 0) {
                    //libera imputazione                        


                    switch (elencoindici[iindice].TipoDato) {

                        case "string":

                            tipo_param = "txt";
                            newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, "input-group-addon", tipo_param + newKey));
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control"));

                            break;

                        case "date":

                            tipo_param = "txt";
                            newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, "input-group-addon", tipo_param + newKey));
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "kendoCalendar"));

                            break;

                        case "numeric":

                            tipo_param = "txt";
                            newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, "input-group-addon", tipo_param + newKey));
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control"));

                            break;

                        case "boolean":

                            tipo_param = "chk";
                            newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, "input-group-addon", tipo_param + newKey));
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "kendoSwitch"));

                            break;

                    }



                }
                else {
                    tipo_param = "ddl";

                    newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, "input-group-addon", tipo_param + newKey));
                    newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control", null, false, ""));

                }


                newColumnDiv.appendChild(newDivInputGroup);
                newRowDiv.appendChild(newColumnDiv);

                contaRighe++;

                //}

            }
        }

        // ultimo giro
        if (iindice == elencoindici.length - 1 && newRowDiv !== null) {
            container.appendChild(newRowDiv);
        }

    }


    //Creazione Controlli Kendo
    for (var iindice2 = 0; iindice2 < elencoindici.length; iindice2++) {
        if (elencoindici[iindice2].ID_Indice !== 0) {

            var chiave = elencoindici[iindice2].TipoCampo + "_" + elencoindici[iindice2].ID_Indice;

            switch (parseInt(elencoindici[iindice2].TipoCampo)) {

                case 0:

                    switch (elencoindici[iindice2].TipoDato) {

                        case "string":
                            break;

                        case "date":

                            var controllo = "#txt" + chiave + "";
                            $(controllo).kendoDatePicker({
                                footer: "#: kendo.toString(data, 'd')#",
                                max: new Date(2100, 11, 31)
                            });

                            break;

                        case "numeric":

                            var controllo = "#txt" + chiave + "";
                            $(controllo).kendoNumericTextBox();
                            break;

                        case "boolean":

                            var controllo = "chk" + chiave + "";
                            creaKendoSwitch(controllo);
                            break;

                    }

                    break;

                default:

                    elencodettagli = RicercaDDLIndice(elencoindici[iindice2].ID_Indice, elencoindici[iindice2].TipoCampo, elencoindici[iindice2].Elenco_Tipo, elencoindici[iindice2].Elenco_Cod);
                    creaKendoDropDownList("ddl" + chiave, { read: RiempiDDLDettagli }, "Valore_Des", "Valore_Cod");
                    //$("ddl" + chiave).autoWidth = true;

                    if (parseInt(elencoindici[iindice2].Elenco_Tipo) == 1 && piva !== undefined && id_elenco == "") {

                        //in caso di impresa gias --> default = impresa
                        Set_KendoDDLValue("ddl" + chiave, piva);

                    }

                    break;

            }
        }
    }


    $(container).show();
}

function RiempiDDLDettagli(options) {
    options.success(elencodettagli);
}

//// Leggere i valori dai controlli per salvarli su DB

function AggiungiValoriIndici() {

    let arrParams = [];

    if (elencoindici !== undefined && elencoindici !== null && elencoindici.length !== 0) {


        for (var iindice = 0; iindice < elencoindici.length; iindice++) {
            if (elencoindici[iindice].ID_Indice !== 0) {
                if (elencoindici[iindice].TipoCampo == 0 ||
                    elencoindici[iindice].TipoCampo == 1 ||
                    elencoindici[iindice].TipoCampo == 2) {

                    var chiave3 = elencoindici[iindice].TipoCampo + "_" + elencoindici[iindice].ID_Indice;

                    let vID_Indice = parseInt(elencoindici[iindice].ID_Indice);
                    let vID_Indice_Det = 0;
                    let vElenco_Val = 0;
                    let vValore_Des = "";
                    let vTipoCampo = elencoindici[iindice].TipoCampo;


                    switch (parseInt(elencoindici[iindice].TipoCampo)) {

                        case 0: //Valore Libero

                            switch (elencoindici[iindice].TipoDato) {

                                case "boolean":

                                    vValore_Des = getKendoSwitch("chk" + chiave3)
                                    break;

                                default:

                                    vValore_Des = $('input[name$="txt' + chiave3 + '"]').val();
                                    break;

                            }

                            break;

                        case 1: //Dettaglio

                            vID_Indice_Det = Get_KendoDDLValue("ddl" + chiave3);
                            break;

                        case 2: //Elenco

                            vElenco_Val = Get_KendoDDLValue("ddl" + chiave3);
                            break;
                    }



                    //verifico se è già presente nell'array
                    let elems = arrayLookup(arrParams, "ID_Indice", chiave3);

                    //l'oggetto è di fatto ritornato per riferimento, quindi le modifiche fatte qui non sono su una copia, ma sull'elemento stesso
                    if (elems !== undefined && elems !== null) {

                        elems.ID_Indice = vID_Indice;
                        elems.ID_Indice_Det = vID_Indice_Det;
                        elems.Elenco_Val = vElenco_Val;
                        elems.Valore_Des = vValore_Des;
                        elems.TipoCampo = vTipoCampo;

                    } else {
                        let res = {
                            ID_Indice: vID_Indice,
                            ID_Indice_Det: vID_Indice_Det,
                            Elenco_Val: vElenco_Val,
                            Valore_Des: vValore_Des,
                            TipoCampo: vTipoCampo

                        };
                        arrParams.push(res);
                    }

                }
            }
        }

    }

    return arrParams;

}



function CheckDatiNecessariInseriti() {

    var bDatiNecessariInseriti = true;

    // Controllo sia stato caricato il documento
    if (allegatoInBase64 === null) {
        $("<div></div>").kendoAlert({
            title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
            content: TraduzioneMultiResx(resxObj, "OoccorreCaricareDocumento", "Occorre caricare il documento"),
        }).data("kendoAlert").open();

        return false;
    }

    // Controllo sia stata specificata la tipologia di firma utilizzata
    var tipoFirma = parseInt(Get_KendoDDLValue("ddlTipoFirma"));
    if (tipoFirma === 0) {
        $("<div></div>").kendoAlert({
            title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
            content: TraduzioneMultiResx(resxObj, "IndicareTipologiaFirmaDocumento", "Indicare la tipologia di firma utilizzata per segnare il documento"),
        }).data("kendoAlert").open();

        return false;
    }

    // Controllo gli indici obbligatori
    if (elencoindici !== undefined && elencoindici !== null && elencoindici.length !== 0) {

        for (var iindice = 0; iindice < elencoindici.length; iindice++) {
            if (elencoindici[iindice].ID_Indice !== 0 && (parseInt(elencoindici[iindice].ChkObbligatorio) == 1) || parseInt(elencoindici[iindice].ChkObbligatorio_Tipologia) == 1) {
                if (elencoindici[iindice].TipoCampo == 0 ||
                    elencoindici[iindice].TipoCampo == 1 ||
                    elencoindici[iindice].TipoCampo == 2) {

                    var chiave3 = elencoindici[iindice].TipoCampo + "_" + elencoindici[iindice].ID_Indice;

                    var valore_cod = 0;
                    var valore_des = "";

                    switch (parseInt(elencoindici[iindice].TipoCampo)) {

                        case 0: //Valore Libero

                            switch (elencoindici[iindice].TipoDato) {

                                case "date":

                                    valore_des = kendo.parseDate($("#txt" + chiave3).val());
                                    if (valore_des == null) {
                                        valore_des = "";
                                    }
                                    break;

                                case "boolean":

                                    valore_des = "ok"; //sempre valorizzato
                                    break;

                                default:

                                    valore_des = $('input[name$="txt' + chiave3 + '"]').val();
                                    break;

                            }

                            break;

                        default:

                            valore_cod = Get_KendoDDLValue("ddl" + chiave3);
                            break;
                    }


                    if ((valore_cod == 0 || valore_cod == "") && valore_des == "") {

                        $("<div></div>").kendoAlert({
                            title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
                            content: kendo.format(
                                TraduzioneMultiResx(
                                    scadCreaModItemResx,
                                    "ValoreObbligatorioXNonImpostato",
                                    "Valore obbligatorio {0} non impostato correttamente"
                                ),
                                elencoindici[iindice].TitoloIndice
                            ),
                        }).data("kendoAlert").open();

                        bDatiNecessariInseriti = false;
                        break;

                    }


                }

            }
        }

    }
    return bDatiNecessariInseriti;

}




function btnProcediRichiestaClick(ev) {
    
    if (CheckDatiNecessariInseriti() === true) {

        // Effettuo Controlli sul documento
        var mesErroriAllegato = "";

        var mexAntivirus = checkVirus($("#txtCaricaDocumento").val(), allegatoInBase64, $("[name='hfObjPSuperServer']").val());
        if (mexAntivirus !== "") {
            //$("#txtCaricaDocumento").val("");
            $("#fileCaricaDocumento").val("");
            allegatoInBase64 = null;

            mesErroriAllegato = mexAntivirus;
        }
        else {
            var tipoFirma = parseInt(Get_KendoDDLValue("ddlTipoFirma"));
            if (tipoFirma === 2) {
                ajaxAgronicaSync("./RichiesteIscrizioni.aspx/VerificaFirmaDigitale",
                    JSON.stringify({ fileBase64: allegatoInBase64 }),
                    false,
                    function (risp) {
                        // Callback success - Non reimposto il messaggio di errore
                    },
                    function (risp) {
                        // Callback error
                        mesErroriAllegato = TraduzioneMultiResx(resxObj, "FirmaDigitaleNonValida", "Firma digitale assente o non valida. Caricare un documento di estensione 'p7m'");
                    }
                );
            }
        }

        if (mesErroriAllegato === "") {
            // Superati i controlli proseguo con l'inserimento del documento
            var idAreaCategoria = 8;
            var idTipologia = -13;
            var idElenco = -1;
            var idAlertEntita = -1;
            var allegatiDocumentiCod = 0;
            var bAllegatoModificato = true;
            var numDoc = "";
            var dataScadenza = new Date();
            dataScadenza.setDate(dataScadenza.getDate() + 1);
            var strDataScadenza = DataOra_DataIta(dataScadenza);
            var nomeFile = $("#txtCaricaDocumento").val();

            var jsonTipologia = $("#hfTipologiaIscr").val();
            var arrTipologia = JSON.parse(jsonTipologia);
            var objTipologia = arrTipologia[0];

            var mesTipoFirma = "";
            if (tipoFirma === 1) {
                mesTipoFirma = "\r\n" + TraduzioneMultiResx(resxObj, "DocumentoSenzaFirmaDigitale", "Il documento non contiene firma digitale");
            }
            else {
                if (tipoFirma === 2) {
                    mesTipoFirma = "\r\n" + TraduzioneMultiResx(resxObj, "DocumentoConFirmaDigitale", "Il documento contiene firma digitale");
                }
            }
            var descrizione = objTipologia.Nome_Tipologia + mesTipoFirma;

            var note = $("#txtAreaNote").data("kendoTextArea").value();
            var validazioneFlag = 0; // 0 = Documento da validare; 1 = Doc. valido; -1 = Doc. non valido
            var dataUpload = new Date();
            dataUpload.setHours(0, 0, 0);
            var usernameUpload = "";

            var params = {
                objP_server: $("[name='hfObjPServer']").val(),
                objP_utenti: $("[name='hfObjPUtenti']").val(),
                strObjJSON: JSON.stringify({
                    id_elenco: idElenco,
                    id_alert_entita: idAlertEntita,
                    allegati_documenti_cod: allegatiDocumentiCod,
                    id_area: idAreaCategoria,
                    id_tipologia: idTipologia,
                    ballegato_modificato: bAllegatoModificato,
                    piva: $("[name='hfPivaSuperServer']").val(),
                    sa_cod: 0,
                    appezza: 0,
                    mac_cod: "",
                    cod_contatto: "",
                    analisi_testata_cod: 0,
                    pc_testata_cod: 0,
                    pua_cod: 0,
                    num_documento: numDoc,
                    testo: descrizione,
                    data: strDataScadenza,
                    nome_file: nomeFile,
                    file_allegato: allegatoInBase64,
                    validazione_flag: validazioneFlag,
                    username_upload: usernameUpload,
                    data_upload: dataUpload,
                    note: note,
                    entitaxindici: AggiungiValoriIndici()
                })
            };

            ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert.asmx/Scrivi",
                JSON.stringify(params),
                false,
                function (risposta) {
                    console.log(risposta);
                    // Callback success
                    $("<div></div>").kendoAlert({
                        title: TraduzioneMultiResx(resxObj, "RichiestaCorrettamenteInviata", "La Richiesta è stata correttamente inviata"),
                        content: TraduzioneMultiResx(resxObj, "TornaHomepage", "Premere 'ok' per tornare alla homepage"),
                        close: function () {
                            var linkHomepage = "";
                            ajaxAgronicaSync("./RichiesteIscrizioni.aspx/LinkHomepage", JSON.stringify({ valSuperServer: $("#hfValSuperServer").val() }), false, function (risp) {
                                linkHomepage = risp.RispostaStringa;
                            });
                            window.location.href = linkHomepage;
                        }
                    }).data("kendoAlert").open();
                },
                null,
                null,
                true
            );
        }
        else {
            $("<div></div>").kendoAlert({
                title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
                content: mesErroriAllegato,
            }).data("kendoAlert").open();
        }

    }

}