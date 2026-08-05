//#region CARICAMENTO DATI DOCUMENTO
function caricaScadDaIdElenco(id_elenco) {

    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "id_elenco": id_elenco });

        ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Entita.asmx/Leggi_con_documenti",
            parametri,
            false,
            async function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);

                if (risp !== undefined && risp !== "") {
                    var elem = risp[0];

                    $(txbID).val(id_elenco);

                    //Impostazione data riferimento per caricamento DDL Indici -- Non spostare
                    data_creazione = elem["Data_Creazione"];

                    $(cRichiesta_Cod).val(elem["Richiesta_Cod"]);
                    $(cId_Schema_Template).val(elem["Id_Schema_Template"]);
                    $(cAnalisi_Testata_Cod).val(elem["Analisi_Testata_Cod"]);
                    $(cIdAgenda).val(elem["ID_Agenda"]);
                    $(cRicetta_Operazione_Cod).val(elem["Ricetta_Operazione_cod"]);
                    cId_ImpresexParticelle = (elem["Id_ImpresexParticelle"]);

                    $("#ctl00_MainContent_stato_pratica").val(elem["Stato_Attuale"])

                    if ($(cRichiesta_Cod).val() !== "0" || $(cAnalisi_Testata_Cod).val() !== "0" || $(cIdAgenda).val() !== "0" || $(cRicetta_Operazione_Cod).val() !== "0") {
                        await ddlAzienda_Load(elem["piva"]);
                    } else {
                        await ddlAzienda_Load(undefined);
                    }

                    if ($(cIdAgenda).val() != 0 && $(cRicetta_Operazione_Cod).val() == 0) {
                        indexGrigliaDaMostrare_TipoEntitaSecondario = 0
                        let lavCod_visite = [5001, 5002, 5007]
                        for (let i = 0; i < lavCod_visite.length; i++) {
                            if (elem["Lav_Cod"] == lavCod_visite[i]) {
                                indexGrigliaDaMostrare_TipoEntitaSecondario = 2
                                break;
                            }
                        }
                    }

                    if ($(cRicetta_Operazione_Cod).val() != 0 && $(cIdAgenda).val() == 0) {
                        indexGrigliaDaMostrare_TipoEntitaSecondario = 1
                    }


                    await Set_KendoDDLValueVirtual('ddlAzienda', elem["piva"])
                    //$('#ddlAzienda').data("kendoDropDownList").value(elem["piva"]);
                    ddlAzienda_Change();
                    $('#ddlArea').data("kendoDropDownList").value(elem["id_area"]);
                    ddlArea_Change();
                    $('#ddlTipologia').data("kendoDropDownList").value(elem["ID_Tipologia"]);
                    ddlTipologia_Change();

                    //------ INDICI
                    //$('#ddlCentro').data("kendoDropDownList").value(elem["Sa_Cod"]);
                    //$('#ddlAppezzamento').data("kendoDropDownList").value(elem["Appezza"]);
                    $('#ddlMacchina').data("kendoDropDownList").value(elem["Mac_Cod"]);
                    $('#ddlContatto').data("kendoDropDownList").value(elem["Cod_Contatto"]);
                    $('#ddlAnalisi').data("kendoDropDownList").value(elem["Analisi_Testata_Cod"]);
                    //$('#ddlPianoConcimazione').data("kendoDropDownList").value(elem["PC_Testata_Cod"]);
                    //$('#ddlPua').data("kendoDropDownList").value(elem["PUA_Cod"]);

                    if (elem["Data"] !== "31/12/2100") {
                        $('#Txt_Data_Scadenza').val(elem["Data"]);
                    } else {
                        $('#Txt_Data_Scadenza').val("");
                    }

                    $('#Txt_Num_Documento' + usaUploadMultiplo).val(elem["Allegati_Documenti_Numero"]);

                    if (elem["Allegati_Documenti_Data"] !== "" && elem["Allegati_Documenti_Data"] !== "01/01/1900") {
                        $('#Txt_Data_Allegato').val(elem["Allegati_Documenti_Data"]);
                    } else {
                        $('#Txt_Data_Allegato').val("");
                    }

                    $('#txbDescrizione').val(elem["Testo"]);

                    $('#txbNote').val(elem["Note"]);


                    //--- ALLEGATI
                    if (kendo.parseDate(elem["Data_Upload"]) !== undefined) {
                        //formattedDate(new Date(), '/'));
                        $('#Txt_Data_Upload' + usaUploadMultiplo).val(formattedDate(elem["Data_Upload"], '/'));
                        //$('#Txt_Data_Upload').val(kendo.parseDate(elem["Data_Upload"]));
                    }

                    allegati_documenti_cod = parseInt(elem["Allegati_Documenti_Cod"]);
                    CompressoDaGIAS = Boolean(elem["CompressoDaGIAS"]);

                    //$('#Txt_Username' + usaUploadMultiplo).val(elem["Username_Upload"]);
                    $('input[name$="Txt_Username' + usaUploadMultiplo + '"]').val(elem["Username_Upload"]);

                    $('#Txt_Documento_Allegato' + usaUploadMultiplo).val(elem["File_Name"]);

                    spanDownloadAllegato = "";
                    $("#pnlApriAllegato").hide();
                    if ($("#Txt_Documento_Allegato" + usaUploadMultiplo).val() !== undefined && $("#Txt_Documento_Allegato" + usaUploadMultiplo).val() !== "") {
                        $("#pnlDatiAllegato" + usaUploadMultiplo).show();

                        if (usaUploadMultiplo == "") {
                            let icon = CreaIconaDownloadDocumento(elem["Allegati_Documenti_Estensione"]);
                            spanDownloadAllegato =
                                "<span class='fa fa-2x " + icon + " ' onclick = Leggi_Doc_Allegato(" + allegati_documenti_cod + "," + CompressoDaGIAS + ") ></span>";
                            $("#pnlApriAllegato").show();
                        }
                    }
                    //ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
                    //else if ($(cModalita).val() == "") {
                    //    //siamo in modalità scadenza, senza avere mai allegato un file. devo caricare il pannello degli allegati
                    //    $("#pnlDatiAllegato" + usaUploadMultiplo).show();
                    //    inizializza_KendoUpload([]);
                    //}

                    if (usaUploadMultiplo == "") {
                        $("#divDownloadAllegato").html(spanDownloadAllegato);
                        ImpostaEstensione();
                    }

                    if (usaUploadMultiplo !== "") {
                        CompressoDaGIAS = Boolean(elem["CompressoDaGIAS"]);

                        if (CompressoDaGIAS) {
                            decompressaFile(elem["File_Allegato_DB"]);
                        } else {
                            imposta_InitialFile_singolo(elem);
                        }

                        let pluto = $("#paperino").clone();
                        divKendoUpload_InitialFiles = pluto.slice();

                        carica_InitialFiles();
                    }


                    $('#cmbValidazione' + usaUploadMultiplo).data("kendoDropDownList").value(elem["Validazione_Flag"]);

                    if (parseInt(elem["ChkStorico"]) == 1) {
                        setKendoSwitch("ChkStorico" + usaUploadMultiplo, true);
                    }
                    else {
                        setKendoSwitch("ChkStorico" + usaUploadMultiplo, false);
                    }
                    resolve();

                    //Metto in sola lettura le ddl
                    //solaLetturaDdl();

                    ////Se la scadeza è una di quelle gestite da noi, blocco anche data e descrizione
                    //if (elem["ID_Tipologia"] < 0) {
                    //    $('#txbData').data("kendoDatePicker").enable(false);
                    //    $('#txbDescrizione').prop('readonly', 'readonly');
                    //}

                }
            }, null);

    });
}

function Leggi_Tipo_Entita_Chiavi() {
    var id_area = $('#ddlArea').data("kendoDropDownList").value();
    var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();

    if (jQuery.isNumeric(id_tipologia) === false) {
        return;
    }

    ////Mostro i pannelli solo se sono le nostre categorie 'speciali'
    //if (id_area !== "2" && id_area !== "7" && id_area !== "11" && id_tipologia >= 0) {
    //    return;
    //}

    let bOk = false;

    if (id_tipologia < 0) {
        bOk = true;
    }

    //Macchine
    if (id_area == "2") {
        bOk = true;
    }
    //UMA
    if (id_area == "7") {
        bOk = true;
    }
    //QDC
    if (id_area == "11") {
        bOk = true;
    }
    //Doc Contabiili
    if (id_area == "10") {
        bOk = true;
    }
    //Carico/Scarico di Magazzino
    if (id_area == "13") {
        bOk = true;
    }

    //Contratto Affitto Catasto
    if (id_area == "12" && id_tipologia == -26) {
        bOk = false;
    }

    //Mostro i pannelli solo se sono le nostre categorie 'speciali'
    if (bOk == false) {
        return;
    }

    var parametri = kendo.stringify({ "objP_server": objP_server, "id_tipologia": id_tipologia });
    var controller = "";
    tipoTipoEntitaCod = [];

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Tipo_Entita_Chiavi.asmx/Leggi_Tipo_Entita_Chiavi",
        parametri, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            nascondiPnlAccessori();

            for (var i in risp) {
                controller = "";
                switch (risp[i]["nome_chiave"].toLowerCase()) {
                    case "sa_cod":
                        //$("#pnlCentro").show();
                        break;
                    case "appezza":
                        //$("#pnlAppezzamento").show();
                        break;
                    case "cod_contatto":
                        $("#pnlContatto").show();
                        bcheckcontatto = true;
                        controller = "bcheckcontatto";
                        break;
                    case "mac_cod":
                        $("#pnlMacchina").show();
                        bcheckmacchina = true;
                        controller = "bcheckmacchina";
                        break;
                    case "analisi_testata_cod":
                        $("#pnlAnalisi").show();
                        bcheckanalisi = true;
                        controller = "bcheckanalisi";
                        break;
                    case "pc_testata_cod":
                        //$("#pnlPianoConcimazione").show();
                        break;
                    case "pua_cod":
                        //$("#pnlPua").show();
                        break;
                    case "id_agenda":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckagenda = true;
                        controller = "bcheckagenda";
                        break;
                    case "richiesta_cod":
                        $("#pnlUMA").show();
                        bcheckuma = true;
                        controller = "bcheckuma";
                        break;
                    case "ricetta_operazione_cod":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckricette = true;
                        controller = "bcheckricette";
                        break;
                    case "id_impresexparticelle":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckparticellecatastali = true;
                        controller = "bcheckparticellecatastali";
                        break;
                }

                tipoTipoEntitaCod.push([risp[i]["nome_chiave"].toLowerCase(), "Primario", controller, 0]);
            }
        }, null);


    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Tipo_Entita_Chiavi.asmx/Leggi_Tipo_Entita_Chiavi_Secondario",
        parametri, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            for (var i in risp) {
                controller = "";
                switch (risp[i]["nome_chiave"].toLowerCase()) {
                    case "sa_cod":
                        //$("#pnlCentro").show();
                        break;
                    case "appezza":
                        //$("#pnlAppezzamento").show();
                        break;
                    case "cod_contatto":
                        $("#pnlContatto").show();
                        bcheckcontatto = true;
                        controller = "bcheckcontatto";
                        break;
                    case "mac_cod":
                        $("#pnlMacchina").show();
                        bcheckmacchina = true;
                        controller = "bcheckmacchina";
                        break;
                    case "analisi_testata_cod":
                        $("#pnlAnalisi").show();
                        bcheckanalisi = true;
                        controller = "bcheckanalisi";
                        break;
                    case "pc_testata_cod":
                        //$("#pnlPianoConcimazione").show();
                        break;
                    case "pua_cod":
                        //$("#pnlPua").show();
                        break;
                    case "id_agenda":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckagenda = true;
                        controller = "bcheckagenda";
                        break;
                    case "richiesta_cod":
                        $("#pnlUMA").show();
                        bcheckuma = true;
                        controller = "bcheckuma";
                        break;
                    case "ricetta_operazione_cod":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckricette = true;
                        controller = "bcheckricette";
                        break;
                    case "id_impresexparticelle":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckparticellecatastali = true;
                        controller = "bcheckparticellecatastali";
                        break;
                }

                tipoTipoEntitaCod.push([risp[i]["nome_chiave"].toLowerCase(), "Secondario", controller, 0]);
            }
        }, null);
}

async function Leggi_Doc_Allegato(Allegati_Documenti_Cod, CompressoDaGIAS) {
    if (CompressoDaGIAS == true && UploadMultiploAllegatiAbilitato == false) {
        creaScaricaZip(Allegati_Documenti_Cod);
    } else {
        var param = kendo.stringify({
            'objP_server': objP_server,
            'objP_utenti': objP_utenti,
            'piva': '',
            'allegati_documenti_cod': Allegati_Documenti_Cod
        });

        ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_File_Allegato", param,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa);
                let fileDati = risp[0].File_Allegato_DB;
                let nomeFile = risp[0].Allegati_Documenti_NomeFile;
                let estensione = risp[0].Allegati_Documenti_Estensione;

                SaveAndOpenFileByteArray(nomeFile, fileDati, estensione);

            }, null);
    }
}

function leggiDataScadenza_daTipologia(ID_Tipologia) {
    var piva = $('#ddlAzienda').data("kendoDropDownList").value();
    var id_area = $('#ddlArea').val();
    var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();

    //var hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
    var soloPrivate = !(hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1");
    soloPrivate = false;

    var parametri = kendo.stringify({
        "objP_server": objP_server,
        "piva": piva, "soloPrivate": soloPrivate,
        "id_area": id_area, "controllaSeUtenteAutorizzato": true,
        "tipoPermessoDaControllare": tipoPermessoDaControllare,
        "listaIndici": "",
        "xMultiSelect": false,
        "id_tipologia": 0
    });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Tipologia.asmx/Leggi_Tipologie",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            if (risp !== undefined && risp !== "") {
                var elem;
                for (i in risp) {
                    elem = risp[i];
                    if (elem["id_tipologia"].toString() === id_tipologia) {
                        elem = risp[i];
                        FlagDataScadenzaObbligatoria = elem["FlagDataScadenzaObbligatoria"];
                        DataDefault = elem["DataDefault"];
                    }
                }
            }
        }, null);

    if (FlagDataScadenzaObbligatoria) {
        //Se il flag è true ma non è stata inserita una data scadenza, la rendo visibile E obbligatoria
        $("#Lbl_Data_Scadenza").addClass("lbl_required");
    }
}
// #endregion

//#region CONTROLLI E SALVATAGGIO
function SalvaDocumento(modalita, FlagEsci) {
    //if (validaAllegato(nuovo)) {
    CheckDatiNecessariInseriti();

    if (bDatiNecessariInseriti) {

        $('html,body').css('cursor', 'wait');

        //Estraggo i dati dalla maschera
        var id_area = $('#ddlArea').data("kendoDropDownList").value();
        var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();

        var id_elenco, id_alert_entita;
        if ($('#txbID').val() == undefined || $('#txbID').val() == "") {
            id_elenco = -1;
            id_alert_entita = -1;
        }
        else {
            id_elenco = $('#txbID').val();
            id_alert_entita = $('input[id*="hfId_Alert_Entita"]').val();
        }

        var piva = $('#ddlAzienda').data("kendoDropDownList").value();

        //var documento_scadenza = modalita;
        var tipo_doc = id_tipologia; //KendoDDL("ddlTipologia").value();

        var datascadenza = $("#Txt_Data_Scadenza").val();
        if (DataDefault === null || DataDefault === "" || datascadenza != "") {
            datascadenza = $("#Txt_Data_Scadenza").val();
        } else if (datascadenza === "" && !FlagDataScadenzaObbligatoria) {
            datascadenza = "";
        } else {
            DataDefault = DataDefault.substring(0, 5)
            let annoCorrente = new Date()
            annoCorrente = annoCorrente.getFullYear()
            datascadenza = DataDefault + "/" + annoCorrente
        }

        var descrizione = $('#txbDescrizione').val();
        var note = $('#txbNote').val();

        //-- ALLEGATI
        if (usaUploadMultiplo != "") {
            compressaFile();
        }

        var nome_file = $('#Txt_Documento_Allegato' + usaUploadMultiplo).val();
        var file_allegato = $('#File_Caricato' + usaUploadMultiplo).val();

        var num_doc = $("#Txt_Num_Documento" + usaUploadMultiplo).val();
        var data_upload = "";
        if (kendo.parseDate($("#Txt_Data_Upload" + usaUploadMultiplo).val()) !== undefined) {
            data_upload = $("#Txt_Data_Upload" + usaUploadMultiplo).val();
        }
        var username_upload = $(cUsername).val();
        var dataallegato = $("#Txt_Data_Allegato").val();

        var validazione_flag = $('#cmbValidazione' + usaUploadMultiplo).data("kendoDropDownList").value();

        var chkstorico = 0;
        if (getKendoSwitch("ChkStorico" + usaUploadMultiplo) == true) {
            chkstorico = 1;
        }

        var cod_contatto = $('#ddlContatto').data("kendoDropDownList").value();
        var sa_cod = 0 // $('#ddlCentro').data("kendoDropDownList").value();
        var appezza = 0// $('#ddlAppezzamento').data("kendoDropDownList").value();                
        var pc_testata_cod = 0 // $('#ddlPianoConcimazione').data("kendoDropDownList").value();
        var pua_cod = 0 // $('#ddlPua').data("kendoDropDownList").value();
        var mac_cod = $('#ddlMacchina').data("kendoDropDownList").value();
        var analisi_testata_cod = $('#ddlAnalisi').data("kendoDropDownList").value();
        var id_agenda = '0';
        var Ricetta_Operazione_Cod = '0';
        var Id_ImpresexParticelle = '0';

        if (id_agenda == "") id_agenda = $(cIdAgenda).val();


        if (id_area == 7) {
            var richiesta_cod = $('#ddlUma_Carburanti').data("kendoDropDownList").value();
        }

        var id_schema_template = $(cId_Schema_Template).val();

        // #region ID_AGENDA
        if (id_area == 11) {
            var griglia_Attivita = $("#griglia_Attivita").data("kendoGrid");
            var elementoSelezionato;
            if (griglia_Attivita != undefined) {
                elementoSelezionato = griglia_Attivita.dataItem(griglia_Attivita.select());
                id_agenda = (elementoSelezionato != undefined) ? (elementoSelezionato.Id_Agenda) : ('0');
            }

            var griglia_Visite = $("#griglia_Visite").data("kendoGrid");

            if (griglia_Visite != undefined && id_agenda == 0) {
                elementoSelezionato = griglia_Visite.dataItem(griglia_Visite.select());
                id_agenda = (elementoSelezionato != undefined) ? (elementoSelezionato.Id_Agenda) : ('0');
            }
        } else {
            var griglia_Riferimenti = $("#griglia_Riferimenti").data("kendoGrid");
            var operazioneSelezionata;
            if (griglia_Riferimenti != undefined) {
                operazioneSelezionata = griglia_Riferimenti.dataItem(griglia_Riferimenti.select());
                id_agenda = (operazioneSelezionata != undefined) ? (operazioneSelezionata.Id_Agenda) : ('0');
            }
        }
        // #endregion

        // #region RICETTA_OPERAZIONE_COD
        var griglia_RicetteODLBrogliaccio = $("#griglia_RicetteODLBrogliaccio").data("kendoGrid");
        var ricettaBrogliaccioSelezionata;

        if (griglia_RicetteODLBrogliaccio != undefined) {
            ricettaBrogliaccioSelezionata = griglia_RicetteODLBrogliaccio.dataItem(griglia_RicetteODLBrogliaccio.select());
            Ricetta_Operazione_Cod = (ricettaBrogliaccioSelezionata != undefined) ? (ricettaBrogliaccioSelezionata.Ricetta_Operazione_Cod) : ('0');
        }
        // #endregion

        // #region ID_IMPRESEXPARTICELLE
        var griglia_ParticelleCatastali = $("#griglia_ParticelleCatastali").data("kendoGrid");
        var particellaCatastaleSelezionata;

        if (griglia_ParticelleCatastali != undefined) {
            particellaCatastaleSelezionata = griglia_ParticelleCatastali.dataItem(griglia_ParticelleCatastali.select());
            Id_ImpresexParticelle = (particellaCatastaleSelezionata != undefined) ? (particellaCatastaleSelezionata.Id_ImpresexParticelle) : ('0');
        }
        // #endregion

        // #region MULTITIPOLOGIA
        var multiTipologia = "";
        if (KendoMultisel("cmbTipologia") !== undefined) {
            multiTipologia = KendoMultisel("cmbTipologia").value().join(",");

            //Se la descrizione non è stata modificata dall'utente, allora gli concateno tutte le multitipologie
            let testoTipologia = $("#ddlTipologia").data("kendoDropDownList").text();
            if (testoTipologia === descrizione) {
                let multiSel = $("#cmbTipologia").data("kendoMultiSelect");
                let tipologieSelezionate = multiSel.dataItems();
                let lista = [];

                $(tipologieSelezionate).each(function () {
                    lista.push(this.nome);
                });

                descrizione += ", " + lista.join(', ')
            }
        }
        // #endregion

        //chiamo il web service
        var strObjJSON = JSON.stringify({
            "id_elenco": id_elenco,
            "id_alert_entita": id_alert_entita,
            "allegati_documenti_cod": allegati_documenti_cod,
            "id_area": id_area,
            "id_tipologia": id_tipologia,
            "ballegato_modificato": bAllegato_Modificato,
            "piva": piva,
            "sa_cod": sa_cod,
            "appezza": appezza,
            "mac_cod": mac_cod,
            "cod_contatto": cod_contatto,
            "analisi_testata_cod": analisi_testata_cod,
            "id_agenda": id_agenda,
            "pc_testata_cod": pc_testata_cod,
            "pua_cod": pua_cod,
            "richiesta_cod": richiesta_cod,
            "id_schema_template": id_schema_template,
            "num_documento": num_doc,
            "data_documento": dataallegato,
            "testo": descrizione,
            "data": datascadenza,
            "nome_file": nome_file,
            "file_allegato": file_allegato,
            "validazione_flag": validazione_flag,
            "username_upload": username_upload,
            "data_upload": data_upload,
            "note": note,
            "chkstorico": chkstorico,
            "entitaxindici": AggiungiValoriIndici(),
            "multiTipologia": multiTipologia,
            "CompressoDaGIAS": CompressoDaGIAS,
            "Ricetta_Operazione_Cod": Ricetta_Operazione_Cod,
            "Id_ImpresexParticelle": Id_ImpresexParticelle
        });

        var parametri = JSON.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "strObjJSON": strObjJSON });

        ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert.asmx/Scrivi",
            parametri,
            function (risposta) {
                alert(risposta.RispostaStringa.split("|")[0]);

                //Controllo Gestione Dedicata
                if ($(cRichiesta_Cod).val() !== "0" ||
                    ($(cAnalisi_Testata_Cod).val() !== "0") || $(cCod_Contatto).val() !== "") {

                    //La pagina è stata aperta da un iframe quindi lo chiudo
                    //e verificare che arriviamo da NG o meno
                    if ($(cSito_Provenienza).val() == "300") {
                        window.parent.postMessage("chiudiWindowGiasNG", ottieniTargetOrigin(window));
                    } else {
                        window.parent.postMessage("chiudiFrameDocumentale", ottieniTargetOrigin(window));
                    }

                    ////La pagina è stata aperta da un iframe quindi lo chiudo
                    //if ($(cAnalisi_Testata_Cod).val() !== "0" && window.location !== window.parent.location) {
                    //   window.parent.postMessage("chiudiFrameDocumentale", '*');
                    //} else {
                    //    window.location = $(cPaginaRedirect).val();
                    //}
                } else {

                    if (FlagEsci) {

                        if (modalita == "hybrid") {
                            modalita = ""; //Verrà gestito il menù scadenze
                        }

                        var url = "./Scad_Lista.aspx?type=" + modalita + "&nuovo=" + FlagEsci;
                        //"../Scadenzario/Scad_lista.aspx?type=doc" + "&area_provenienza=" + "10" + "&p=" + dataItem.PIVA + "&id_agenda=" + dataItem.Id_Agenda;


                        if ($(cArea_Provenienza).val() != '0') {
                            url += "&area_provenienza=" + $(cArea_Provenienza).val() + "&p=" + $(cPiva).val();

                            if ($(cArea_Provenienza).val() == '11') {
                                if ($(cIdAgenda).val() != "0") {
                                    url += "&id_agenda=" + $(cIdAgenda).val();
                                }
                                if ($(cRicetta_Operazione_Cod).val() != "0") {
                                    url += "&Ricetta_Operazione_Cod=" + $(cRicetta_Operazione_Cod).val();
                                }
                            }

                            if ($(cArea_Provenienza).val() == '3') {
                                if ($(cAnalisi_Testata_Cod).val() != "0") {
                                    url += "&Analisi_Testata_Cod=" + $(cAnalisi_Testata_Cod).val();
                                }
                            }

                            if ($(cArea_Provenienza).val() == '2') {
                                if ($(cMac_Cod).val() != "0") {
                                    url += "&Mac_Cod=" + $(cMac_Cod).val();
                                }
                            }

                            if ($(cIdAgenda).val() != "0" && ($(cArea_Provenienza).val() == '10' || $(cArea_Provenienza).val() == '12')) {
                                url += "&id_agenda=" + $(cIdAgenda).val();
                            }
                        }

                        if ($(cSito_Provenienza).val() == '7') { //AUDIT
                            url += "&sito_provenienza=7&cod_Documenti="

                            if (Provienienza_Edit_Audit == true) {
                                window.parent.postMessage("chiusura iframe editaudit", ottieniTargetOrigin(window));




                            } else {
                                //Rispedisco alla ricerca documenti gli Allegati_Documenti_Cod + i cod dei documenti appena aggiunti
                                let newCod = risposta.RispostaStringa.split("|")[1]
                                if ($(cxFiltroDocumenti).val() !== "") {
                                    url += $(cxFiltroDocumenti).val() + (newCod != "" ? ',' + newCod : "")
                                } else {
                                    url += newCod
                                }
                            }
                        }

                        window.location = url;

                    } else {

                        // Reset Chiavi
                        $('#txbID').val("");
                        id_elenco = -1;
                        id_alert_entita = -1;

                        id_tipologia_old = 0;
                        bAllegato_Modificato = false;
                        CompressoDaGIAS = false;

                        //Reset Controlli
                        if (usaUploadMultiplo == "") {
                            RimuoviAllegato();
                        } else {
                            resetKendoUpload();
                        }

                        //$('#Txt_Num_Documento').val("");
                        //$('#Txt_Documento_Allegato').val("");
                        //$('#File_Caricato').val("");
                        //$("#pnlDatiAllegato").hide();
                        //$("#pnlApriAllegato").hide();

                        $('#id_indici_list div').html('');
                        $('#txbDescrizione').val("");
                        $('#Txt_Data_Scadenza').val("");
                        $('#Txt_Data_Allegato').val("");
                        $('#txbNote').val("");

                        sbloccaControlliDettagli();
                        //Set_KendoDDLValue("ddlAzienda", "");

                        //Inizializzo tutti i controlli figli
                        pulisciDdlCategorie();


                        ddlAzienda_Change();

                        //Nascondo tutti i controlli accessori
                        nascondiPnlAccessori();

                        AggiornaUsernameUpload();
                    }
                }
            }, null);

        $('html,body').css('cursor', 'default');
    }
}

function DatiNecessariSchema() {

    //Dati Necessari Schema
    var errore = "";

    var piva = $('#ddlAzienda').data("kendoDropDownList").value();
    var id_area = $('#ddlArea').data("kendoDropDownList").value();

    var richiesta_cod = parseInt($(cRichiesta_Cod).val());
    var id_schema_template = parseInt($(cId_Schema_Template).val());


    if (id_area == 7) {
        var richiesta_cod = $('#ddlUma_Carburanti').data("kendoDropDownList").value();
    }

    if (richiesta_cod !== 0 && id_schema_template !== 0) {

        if (usaUploadMultiplo == "") {
            var nome_file = $('#Txt_Documento_Allegato').val();
            var file_allegato = $('#File_Caricato').val();

            errore = checkDatiNecessariSchema(piva, richiesta_cod, id_schema_template, nome_file, file_allegato)

        } else {

            var esitoFirma = [];
            var esitoEstensione = [];
            for (let i = 0; i < listaDocumenti.length; i++) {

                var nome_file = listaDocumenti[i].nome;
                var file_allegato = listaDocumenti[i].file;

                if (file_allegato !== undefined && file_allegato !== null && file_allegato !== "") {

                    var risultato = checkDatiNecessariSchema(piva, richiesta_cod, id_schema_template, nome_file, file_allegato)

                    if (risultato !== "") {
                        if (risultato.includes("Il file deve essere firmato digitalmente")) {
                            esitoFirma.push(nome_file);
                        } else {
                            esitoEstensione.push(nome_file);
                        }
                    }
                }
            }

            if (esitoFirma.length > 0) {
                errore += "I seguenti file devono essere firmati digitalmente: <br>" + esitoFirma.join(", ") + "<br>"
            }
            if (esitoEstensione.length > 0) {
                errore += esitoEstensione.join(", <br>")
            }
        }
    }

    return errore;
}
// #endregion

function checkDatiNecessariSchema(piva, richiesta_cod, id_schema_template, nome_file, file_allegato) {

    var param = kendo.stringify({
        objP_server: objP_server, piva: piva, richiesta_cod: richiesta_cod, id_schema_template: id_schema_template, nome_file: nome_file, file_allegato: file_allegato
    });

    ajaxAgronicaSync("../GestioneAllegati/GestioneAllegati.aspx/ControllaDatiNecessariSchema",
        param, false,
        function (risposta) {
            /*var risp = JSON.parse(risposta.RispostaStringa);*/

            errore = risposta.RispostaStringa;

        }, function (risposta) {
            errore = risposta.Errore;
        });
    return errore;
}


//#region INDICI
function LeggiEntitaxIndici(id_alert_entita) {

    var elenco = "";
    var piva = $('#ddlAzienda').data("kendoDropDownList").value();

    var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva, id_alert_entita: id_alert_entita });

    ajaxAgronicaSync("../GestioneAllegati/GestioneAllegati.aspx/LeggiEntitaxIndici",
        param, false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            elenco = risp;
        }, null);

    return elenco;
}

function RicercaIndicixTipologia(id_area, id_tipologia) {

    var piva = $('#ddlAzienda').data("kendoDropDownList").value();

    if (piva !== "" && piva !== undefined) {


        var elenco = "";
        if (id_area == "" || id_area == undefined) {
            id_area = 0;
        }

        if (id_tipologia == "" || id_tipologia == undefined) {
            id_tipologia = 0;
        }

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            id_area: id_area,
            id_tipologia: id_tipologia,
            Data_Creazione: data_creazione
        });

        ajaxAgronicaSync("../GestioneAllegati/GestioneAllegati.aspx/RicercaIndicixTipologia",
            param, false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);

                elenco = risp;
            }, null);

        return elenco;

    }
}

function RicercaDDLIndice(id_indice, tipocampo, elenco_tipo, elenco_cod, elenco_cod_string) {

    var elencoindici = "";
    var piva = $('#ddlAzienda').data("kendoDropDownList").value();

    var param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva,
        id_indice: id_indice,
        tipocampo: tipocampo,
        elenco_tipo: elenco_tipo,
        elenco_cod: elenco_cod,
        elenco_cod_string: elenco_cod_string,
        data_riferimento: data_creazione
    });

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

function RiempiDdlMacchina(options) {

    var piva = $('#ddlAzienda').val();
    var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": piva, "mac_cod": parseInt($(cMac_Cod).val()), "id_tipologia": id_tipologia });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Macchine.asmx/Leggi_Macchine_Per_Documentale",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function RiempiDdlContatto(options) {

    var piva = $('#ddlAzienda').val();
    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": piva });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Contatti.asmx/Leggi_Contatti_Per_Piva",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function RiempiDdlUma_Carburanti(options) {

    var piva = $('#ddlAzienda').val();
    var parametri = kendo.stringify({ "piva": piva });

    ajaxAgronicaSync(indirizzohttp + "/LeggiRichiesteUMA",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function RiempiDdlAnalisi(options) {

    var piva = $('#ddlAzienda').val();
    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": piva });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Analisi_Testata.asmx/Leggi_Analisi_Testata_Per_Piva",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function RiempiDdlPianoConcimazione(options) {

    //var piva = $('#ddlAzienda').val();
    //var parametri = kendo.stringify({ "objP_server": objP_server, "piva": piva });

    //ajaxAgronicaSync(pathCoreWS + "Anagrafica/PianoConcimazione_Testata.asmx/Leggi_Piani_Concimazione_Per_Piva",
    //    parametri,
    //    false,
    //    function (risposta) {
    //        risp = JSON.parse(risposta.RispostaStringa);
    //        options.success(risp);
    //    }, null);
}

function RiempiDdlPua(options) {

    //var piva = $('#ddlAzienda').val();
    //var parametri = kendo.stringify({ "objP_server": objP_server, "piva": piva });

    //ajaxAgronicaSync(pathCoreWS + "Anagrafica/PianoConcimazione_Testata.asmx/Leggi_Pua_Per_Piva",
    //    parametri,
    //    false,
    //    function (risposta) {
    //        risp = JSON.parse(risposta.RispostaStringa);
    //        options.success(risp);
    //    }, null);
}

//  DDL OPERAZIONI (agenda) !!NON USATA!! --> SOSTITUITA DA GRIGLIA
function RiempiDdlAgenda(options, piva, idAgenda, idTipologia) {

    var parametri = "";
    var pathCaricaCmb = "";

    //Ho già la piva carico solo l'Azienda passata

    if (piva !== undefined && piva !== null && piva !== "") {
        parametri = kendo.stringify({ "Piva": piva, "idAgenda": idAgenda, "idTipologia": idTipologia, "objP_server": objP_server });
        pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaDDT";
    }

    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}
function RiempiDdlAgenda(options, piva, idArea, idAgenda, idTipologia) {
    var parametri = "";
    var pathCaricaCmb = "";

    if (idArea == 10) {
        if (piva !== undefined && piva !== null && piva !== "") {
            parametri = kendo.stringify({ "Piva": piva, "idAgenda": idAgenda, "idTipologia": idTipologia, "objP_server": objP_server });
            pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaDDT";
        }
    } else {
        parametri = kendo.stringify({ "Piva": piva, "idAgenda": idAgenda, "Sa_Cod": 0, "objP_server": objP_server });
        pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaGenerica";
    }

    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

//  DDL RICETTA (agenda) !!NON USATA!! --> SOSTITUITA DA GRIGLIA
function RiempiDdlRicetta(options, piva, idArea, RicettaOperazioneCod, idTipologia) {
    var parametri = "";
    var pathCaricaCmb = "";

    if (idArea == 10) {
        if (piva !== undefined && piva !== null && piva !== "") {
            parametri = kendo.stringify({ "Piva": piva, "idAgenda": idAgenda, "idTipologia": idTipologia, "objP_server": objP_server });
            pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaDDT";
        }
    } else {
        parametri = kendo.stringify({ "Piva": piva, "idAgenda": idAgenda, "Sa_Cod": 0, "objP_server": objP_server });
        pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaGenerica";
    }

    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}
// #endregion
function CaricaDatiDdlAzienda(piva) {

    return new Promise((resolve, reject) => {


        var parametri = "";
        var pathCaricaCmb = "";

        //Ho già la piva carico solo l'Azienda passata

        if (piva !== undefined && piva !== null && piva !== "") {
            parametri = kendo.stringify({ "piva": piva });
            pathCaricaCmb = indirizzohttp + "/LeggiRagione_Sociale";
        } else {
            parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "solo_aziende_attive": 0 });
            pathCaricaCmb = pathCoreWS + "Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtenteCodiceSocio";
        }

        ajaxAgronica(pathCaricaCmb,
            parametri,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp)
            }, null, null, false);
    })
}

//#region DETTAGLI
function RiempiDdlAzienda(options, piva) {

    var parametri = "";
    var pathCaricaCmb = "";

    //Ho già la piva carico solo l'Azienda passata

    if (piva !== undefined && piva !== null && piva !== "") {
        parametri = kendo.stringify({ "piva": piva });
        pathCaricaCmb = indirizzohttp + "/LeggiRagione_Sociale";
    } else {
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "solo_aziende_attive": 0 });
        pathCaricaCmb = pathCoreWS + "Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtenteCodiceSocio";
    }

    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null, null, false);
}

function RiempiDdlArea(options) {
    options.success(elencoaree);
}

function ricercaDdlArea() {
    var elenco = []
    var piva = $('#ddlAzienda').data("kendoDropDownList").value();

    //var hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
    var soloPrivate = !(hfId_Elenco_val != "" && hfId_Elenco_val != "-1");

    soloPrivate = false; //aggiunto al momento non so cosa fa

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": piva, "soloPrivate": soloPrivate, "controllaSeUtenteAutorizzato": true, "tipoPermessoDaControllare": tipoPermessoDaControllare, "Filtro": $(cFiltro).val() });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Area.asmx/Leggi_AreePerAzienda",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            elenco = risp;
        }, null);

    return elenco
}

function RiempiDdlTipologia(options) {

    var piva = $('#ddlAzienda').data("kendoDropDownList").value();

    var id_area = $('#ddlArea').val();

    if (jQuery.isNumeric(id_area) === false) {
        //options.success([]);
        //return;

        id_area = 0; //Tutte le Tipologie
    }

    //var hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
    var soloPrivate = !(hfId_Elenco_val != "" && hfId_Elenco_val != "-1");

    soloPrivate = false; //aggiunto al momento non so cosa fa

    var parametri = kendo.stringify({
        "objP_server": objP_server,
        "piva": piva,
        "soloPrivate": soloPrivate,
        "id_area": id_area,
        "controllaSeUtenteAutorizzato": true,
        "tipoPermessoDaControllare": tipoPermessoDaControllare,
        "listaIndici": "",
        "xMultiSelect": false,
        "id_tipologia": 0
    });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Tipologia.asmx/Leggi_Tipologie",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            /*Coprob....Elimino dall'elenco le voci sqnpi non più valide. Questo perchè non esiste validità all'interno delle tipologie*/
            if ($(cPivaSuperUser).val() == "00499531200") {

                let dataAttuale = new Date();
                let anno = dataAttuale.getFullYear();

                if ($('#txbID').val() == "" && parseInt(anno) > 2023) {
                    var elem;
                    for (i in risp) {
                        elem = risp[i];
                        if (elem["id_tipologia"].toString() === "15") {
                            risp.splice(i, 1);
                        }
                    }
                    for (i in risp) {
                        elem = risp[i];
                        if (elem["id_tipologia"].toString() === "16") {
                            risp.splice(i, 1);
                        }
                    }
                }
                if ($('#txbID').val() == "" && parseInt(anno) > 2024) {
                    for (i in risp) {
                        elem = risp[i];
                        if (elem["id_tipologia"].toString() === "58") {
                            risp.splice(i, 1);
                        }
                    }
                    for (i in risp) {
                        elem = risp[i];
                        if (elem["id_tipologia"].toString() === "24") {
                            risp.splice(i, 1);
                        }
                    }
                    for (i in risp) {
                        elem = risp[i];
                        if (elem["id_tipologia"].toString() === "52") {
                            risp.splice(i, 1);
                        }
                    }

                }

            }

            options.success(risp);
        }, null);
}
// #endregion

//#region KENDO UPLOAD
function getFileCompressoDaGIAS(Allegati_Documenti_Cod) {

    var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti, 'piva': '', 'allegati_documenti_cod': Allegati_Documenti_Cod });

    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_File_Allegato", param,
        async function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            if (risp !== undefined && risp !== "") {
                let fileDati = risp[0].File_Allegato_DB;
                let nomeFile = risp[0].Allegati_Documenti_NomeFile;
                let estensione = risp[0].Allegati_Documenti_Estensione;

                decompressaFile(fileDati);
                await carica_InitialFiles();

                console.log(listaDocumenti);
                zippaFiles();

            }
        }, null);
}
// #endregion

//#region ATTIVITA E VISITE
function ricercaAttivitaVisite(piva, idArea, idAgenda, idTipologia, dataDa, dataA) {
    var idArea = $('#ddlArea').val();
    let index = selezionaIndexGrigliaDaMostrare();

    var parametri = "";
    var pathCaricaCmb = "";
    var escludiIdAgenda = false;

    if (operazioneSelezionataModifica !== undefined && ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1"))) {
        escludiIdAgenda = true;
    }

    if (idArea == 11) {
        $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").show();
        //TO DO QDC TIPO AGENDA CON FILTRO QDC = TRUE
        if (index == 0) {
            parametri = kendo.stringify({
                "Piva": piva,
                "idAgenda": idAgenda,
                "Sa_Cod": 0,
                "objP_server": objP_server,
                "dataDa": dataDa,
                "dataA": dataA
            });
            pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaAttivita_toKendoGrid";

        } else if (index == 2) {
            parametri = kendo.stringify({
                "Piva": piva,
                "idAgenda": idAgenda,
                "Sa_Cod": 0,
                "objP_server": objP_server,
                "dataDa": dataDa,
                "dataA": dataA
            });
            pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaVisite_toKendoGrid";

        }
    } else {
        parametri = kendo.stringify({
            "Piva": piva,
            "idAgenda": idAgenda,
            "Sa_Cod": 0,
            "objP_server": objP_server,
            "dataDa": dataDa,
            "dataA": dataA
        });
        pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaGenerica_toKendoGrid";
    }

    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {

            if (idArea == 11) {
                if (index == 0) {
                    $('#hdGrigliaAttivita_Valorizzazione').val(risposta.RispostaStringa);
                    popolagriglia_Attivita_RicetteBrogliaccio_Visite("griglia_Attivita");
                    grigliaAttivita_caricata = true;

                } else if (index == 2) {
                    $('#hdGrigliaVisite_Valorizzazione').val(risposta.RispostaStringa);
                    popolagriglia_Attivita_RicetteBrogliaccio_Visite("griglia_Visite");
                    grigliaVisite_caricata = true;
                }
            }
        }, null);
}
// #endregion

//#region RICETTE / ODL / BROGLIACCIO
function ricercaRicetteODLBrogliaccio(idArea, Ricetta_Operazione_Cod, dataDa, dataA) {
    var idArea = $('#ddlArea').val();
    //let buttongroup = $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").kendoButtonGroup().data("kendoButtonGroup");
    //let index = buttongroup.current().index();

    var parametri = "";
    var pathCaricaCmb = "";

    if (idArea == 11) {
        $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").show();
    } else {
        $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").hide();
    }

    if (Ricetta_Operazione_Cod !== undefined && Ricetta_Operazione_Cod !== null && Ricetta_Operazione_Cod !== "") {
        parametri = kendo.stringify({
            "Ricetta_Operazione_Cod": Ricetta_Operazione_Cod,
            "objP_server": objP_server,
            "dataDa": dataDa,
            "dataA": dataA,
        });
        pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiRicetteBrogliaccio_toKendoGrid";
    }

    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {

            $('#hdGrigliaRicetteODLBrogliaccio_Valorizzazione').val(risposta.RispostaStringa);
            popolagriglia_Attivita_RicetteBrogliaccio_Visite("griglia_RicetteODLBrogliaccio", dataDa, dataA);
            grigliaRicetteBrogliaccio_caricata = true;

        }, null);
}
// #endregion

//#region RIFERIMENTI (DOC CONTABILI)
function ricercaRiferimenti(piva, idAgenda, idTipologia, dataDa, dataA) {

    var parametri = "";
    var pathCaricaCmb = "";

    var idArea = $('#ddlArea').val();

    var escludiIdAgenda = false;
    if (operazioneSelezionataModifica !== undefined && ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1"))) {
        escludiIdAgenda = true;
    }

    $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").hide();
    $("#Riferimenti").show();


    if (piva !== undefined && piva !== null && piva !== "") {
        parametri = kendo.stringify({
            "Piva": piva,
            "idAgenda": idAgenda,
            "idArea": idArea,
            "idTipologia": idTipologia,
            "objP_server": objP_server,
            "dataDa": dataDa,
            "dataA": dataA,
            "escludiIdAgenda": escludiIdAgenda
        });
        pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaDDT_toKendoGrid";
    }


    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {

            $('#hdGrigliaRiferimenti_Valorizzazione').val(risposta.RispostaStringa);
            popolagriglia_Riferimenti("griglia_Riferimenti");
            grigliaRiferimenti_caricata = true;

        }, null);

}
// #endregion

//#region PARTICELLE CATASTALI
function ricercaParticelleCatastali(piva) { //to do

    var parametri = "";
    var pathCaricaCmb = "";

    var escludiIdAgenda = false;
    if (operazioneSelezionataModifica !== undefined && ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1"))) {
        escludiIdAgenda = true;
    }

    $("#ParticelleCatastali").show();


    if (piva !== undefined && piva !== null && piva !== "") {
        parametri = kendo.stringify({
            "Piva": piva,
            "Id_ImpresexParticelle": cId_ImpresexParticelle,
            "objP_server": objP_server
        });
        pathCaricaCmb = pathCoreWS + "Anagrafica/Catasto.asmx/LeggiParticelleCatastali_toKendoGrid";
    }


    ajaxAgronicaSync(pathCaricaCmb,
        parametri,
        false,
        function (risposta) {

            $('#hdGrigliaParticelleCatastali_Valorizzazione').val(risposta.RispostaStringa);
            popolagriglia_ParticelleCatastali("griglia_ParticelleCatastali");
            grigliaParticelleCatastali_caricata = true;

        }, null);

}
// #endregion

// #region MULTISELECT TIPOLOGIA
//Anna 29/04/22: aggiunto multiselect tipologie al documento
function RiempiMultiSelectTipologia(options) {

    var piva = $('#ddlAzienda').data("kendoDropDownList").value();
    var id_area = $('#ddlArea').val();
    var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();

    //var hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
    var soloPrivate = !(hfId_Elenco_val != "" && hfId_Elenco_val != "-1");

    soloPrivate = false; //aggiunto al momento non so cosa fa

    var indici = "";
    for (var iindice = 0; iindice < elencoindici.length; iindice++) {
        indici += elencoindici[iindice].ID_Indice + ","
    }
    //Tolgo l'ultima virgola
    indici = indici.slice(0, indici.length - 1);

    var parametri = kendo.stringify({
        "objP_server": objP_server,
        "piva": piva, "soloPrivate": soloPrivate,
        "id_area": id_area,
        "controllaSeUtenteAutorizzato": true,
        "tipoPermessoDaControllare": tipoPermessoDaControllare,
        "listaIndici": indici,
        "xMultiSelect": true,
        "id_tipologia": id_tipologia
    });

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Tipologia.asmx/Leggi_Tipologie",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
            elementimultiCmbTipologia = risp.length;
        }, null);
}
// #endregion