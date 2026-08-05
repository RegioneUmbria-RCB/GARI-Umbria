async function Leggi_Doc_Allegato(Allegati_Documenti_Cod, CompressoDaGIAS) {

    if (CompressoDaGIAS == true) {
        windowCompressoDaGIAS(Allegati_Documenti_Cod);
    } else {
        var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti, 'piva': '', 'allegati_documenti_cod': Allegati_Documenti_Cod });

        ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_File_Allegato", param,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa);
                let fileDati = risp[0].File_Allegato_DB;
                let nomeFile = risp[0].Allegati_Documenti_NomeFile;
                let estensione = risp[0].Allegati_Documenti_Estensione;

                SaveAndOpenFileByteArray(nomeFile, fileDati, estensione);

                //MessaggioTuttoOK_Bootstrap("Esportazione effettuata correttamente", "DIV_Messaggi");

            }, null);
    }
}

function Elenco_Aree_Riempi(flagVuoto) {

    var risultato_lettura = [];
    var piva = $('#ddlAzienda').data("kendoDropDownList").value();

    ajaxAgronicaSync("./Indici_Documentale.aspx/Leggi_Aree",
        "{ piva: '" + piva + "'}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            if (flagVuoto === true) {
                objVuoto = {
                    "ID_Area": 0,
                    "Nome": ""
                };
                risp.unshift(objVuoto);
            };

            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Elenco_Tipologie_Riempi(id_area) {

    var risultato_lettura;
    var piva = $('#ddlAzienda').data("kendoDropDownList").value();

    ajaxAgronicaSync("./Indici_Documentale.aspx/Leggi_Tipologie",
        "{ piva: '" + piva + "', " +
        "  id_area: " + id_area + "}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            risultato_lettura = risp;
        }, null);


    return risultato_lettura;
}

//FUNZIONE CHE MOSTRA LA TABELLA
function eseguiRicercaScadenze(onLoad_GrigliaVuota) {

    var filtro_area = "";


    if (KendoDDL("cmbArea").dataItem() !== undefined) {
        if (KendoDDL("cmbArea").dataItem().ID_Area !== 0) {
            filtro_area = KendoDDL("cmbArea").dataItem().ID_Area;
        }
    }

    var filtro_tipologia = KendoMultisel("cmbTipologia").value().join(",");

    var piva = $('#ddlAzienda').data("kendoDropDownList").value();
    if (getKendoSwitch("chkAziendaCorrente"))
        piva = currentPiva

    //Per caricare la griglia vuota usiamo una piva fasulla, onLoad_GrigliaVuota == true solo se non ci sono filtri salvati 
    if (onLoad_GrigliaVuota == true) {
        piva = '-1'
    }

    var chkdocumento = 0;
    if ($(cModalita).val() == "doc") {
        chkdocumento = 1;
    }

    var chksoloattive = 0;
    if (getKendoSwitch("ChkSoloAttive")) {
        chksoloattive = 1;
    }


    //Anna 02/05/22: modificato campo Storico in DDL
    var storico = $('#ddlStorico').data("kendoDropDownList").value();
    if ($(cSito_Provenienza).val() == '7') //AUDIT
        storico = -1 //VOGLIO TUTTI I DOCUMENTI

    //var chkstorico = 0;
    //if (getKendoSwitch("ChkStorico")) {
    //    chkstorico = 1;
    //}


    var validita_inizio = $('input[name$="txt_Validita_Inizio"]').val();
    var validita_fine = $('input[name$="txt_Validita_Fine"]').val();

    //Anna 29/04/22: aggiunti campi al filtro di ricerca
    var Stato_Validazione = $('#cmbValidazione').data("kendoDropDownList").value();
    var Inizio_Upload = $('input[name$="txt_Inizio_upload"]').val();
    var Fine_Upload = $('input[name$="txt_Fine_upload"]').val();

    var chkUtenteUpload = 0;
    if (getKendoSwitch("chkUtenteUpload")) {
        chkUtenteUpload = 1;
    }

    var param = kendo.stringify({
        'objP_server': objP_server,
        'objP_utenti': objP_utenti,
        'piva': piva,
        'filtro_area': filtro_area,
        'filtro_tipologia': filtro_tipologia,
        'chkdocumento': chkdocumento,
        'chksoloattive': chksoloattive,
        'chkstorico': storico, //chkstorico,
        'richiesta_cod': $(cRichiesta_Cod).val(),
        'analisi_testata_cod': $(cAnalisi_Testata_Cod).val(),
        'mac_cod': $(cMac_Cod).val(),
        'Validita_Inizio': validita_inizio,
        'Validita_Fine': validita_fine,
        'id_agenda': $(cIdAgenda).val(),
        'Stato_Validazione': Stato_Validazione,
        'Utente_Upload': chkUtenteUpload,
        'Inizio_Upload': Inizio_Upload,
        'Fine_Upload': Fine_Upload,
        'Ricetta_Operazione_Cod': $(cRicetta).val(),
        'xFiltroDocumenti': $(cxFiltroDocumenti).val(),
        'Cod_Contatto': $(cCod_Contatto).val(),
        'Indici': $(cIndici).val()
    });


    ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_Elenco_ToKendoGrid_New", param,
        function (risposta) {

            $('#hdKendo_Valorizzazione').val(risposta.RispostaStringa);
            popolaGrigliaScadenze("tabella_scadenze", getKendoSwitch("ChkGestioneStorico"));

            //Anna Salvataggio filtri/personalizzazioni griglia
            Applica_Personalizzazioni_Griglie();

        }, null);

    //Anna 03/05/22: salvo i parametri della ricerca [TO-DO il 04/05]
    //SalvaParametri();
}

//IMPORTA SCADENZE
function importaScadenze() {

    var listaCheck = $("#importaScadenzeDialog input:checked");
    //var listaID = listaCheck.map(function (a) { return $(a).foo; });

    if (listaCheck.length > 0) {

        var ID_Tipologia_daImportare = "";
        $.each(listaCheck, function (i, row) {
            ID_Tipologia_daImportare += row.value + "|";
        });

        ID_Tipologia_daImportare = ID_Tipologia_daImportare.substring(0, ID_Tipologia_daImportare.length - 1);

        var param = kendo.stringify({ 'objP_server': objP_server, 'ID_Tipologia_daImportare': ID_Tipologia_daImportare });

        ajaxAgronica(pathCoreWS + "AgronicaCoreScadenziario/Alert_Elenco.asmx/Importa_Scadenze", param,
            function (risposta) {

                alert(risposta.RispostaStringa);
                eseguiRicercaScadenze();

            }, null);
    }
}

// #region FILTRI/PERSONALIZZAZIONI GRIGLIA

function Leggi_Filtri() {

    return new Promise((resolve, reject) => {

        var param = kendo.stringify(
            {
                versione: versioneJsonFiltri
            });

        ajaxAgronicaSync(indirizzohttp + '/LeggiFiltri', param, false,
            async function (risposta) {
                if (risposta.RispostaOK) {
                    if (risposta.RispostaStringa !== "") {
                        var parametri = JSON.parse(risposta.RispostaStringa);
                        await Personalizza_Report(parametri);

                        let proseguiConRicercaScadenze = proseguiConRicerca();
                        if (proseguiConRicercaScadenze == true) {
                            eseguiRicercaScadenze()
                        } else {
                            eseguiRicercaScadenze(true)
                        }


                    } else {
                        //Non sono mai stati salvati dei filtri, carico la griglia vuota
                        eseguiRicercaScadenze(true)
                    }
                } else {
                    console.log("Errore nella lettura impostazioni griglia: " + risposta.Errore);
                }
                resolve()
            }, null);

    })
}

    function setPersonalizzazioniGrigliaKendo(grid, options) {

        try {

            var dataSource = grid.dataSource;
            var savedColumns = options.columns;

            //NUMERO DI RIGHE PER PAGINA
            if (options.pageSize) {
                dataSource.pageSize(options.pageSize);
            } else {
                //Significa che è stato impostato su 'All', non siamo in grado di reimpostarlo direttamente, quindi elmenti per pagina = numero righe totali
                let nrRighe = dataSource._data.length
                dataSource.pageSize(nrRighe);
            }

            if (options.page)
                dataSource.page(options.page);

            //RIORDINAMENTO COLONNE
            var indOrd = 0;
            for (i = 0; i < savedColumns.length; i++) {
                let col;

                //Cerco la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
                if (savedColumns[i].field && savedColumns[i].field != null) {
                    col = grid.columns.find(function (v, index) { return grid.columns[index].field == savedColumns[i].field; });
                } else if (savedColumns[i].title) {
                    col = grid.columns.find(function (v, index) { return grid.columns[index].title == savedColumns[i].title; });
                }

                //Se ho trovato la colonna...
                if (col) {
                    //Sposto la colonna in testa
                    if (savedColumns[i].hidden != true)
                        grid.reorderColumn(indOrd, col);
                    indOrd++;
                }

            }

            //MOSTRO O NASCONDO COLONNE
            for (i = savedColumns.length - 1; i >= 0; i--) {
                let col;

                //Cerco la colonna salvata tra le colonne reali in base al field (campo in tabella) o al titolo della colonna
                if (savedColumns[i].field && savedColumns[i].field != null) {
                    col = grid.columns.find(function (v, index) { return grid.columns[index].field == savedColumns[i].field; });
                } else if (savedColumns[i].title) {
                    col = grid.columns.find(function (v, index) { return grid.columns[index].title == savedColumns[i].title; });
                }

                //Se ho trovato la colonna...
                if (col) {
                    if (savedColumns[i].hidden == true) {
                        grid.hideColumn(col);
                    } else { //else if (savedColumns[i].hidden == false) 
                        grid.showColumn(col);
                    }
                }
            }

            //ORDINAMENTO (ASC/DESC) DELLE COLONNE
            if (options.sort) {

                var ordinam = options.sort;
                for (i = ordinam.length - 1; i >= 0; i--) {
                    let col = grid.columns.find(function (v, index) { return grid.columns[index].field == ordinam[i].field; });

                    if (col == undefined) {
                        ordinam.splice(i, 1);
                    }
                }

                if (ordinam.length > 0)
                    dataSource.sort(options.sort);
            }

            //FILTRI PER COLONNE
            if (options.filter) {

                var filtri = options.filter.filters;
                for (i = filtri.length - 1; i >= 0; i--) {
                    let col = grid.columns.find(function (v, index) { return grid.columns[index].field == filtri[i].field; });

                    if (col == undefined) {
                        filtri.splice(i, 1);
                    }

                    // Sistemazione della data che era stata memorizzata a db in formato GMT
                    if (col.field !== undefined && grid.dataSource.options.schema.model.fields[col.field].type === "date")
                        filtri[i].value = kendo.parseDate(filtri[i].value);
                }

                if (filtri.length > 0)
                    dataSource.filter(options.filter);
            }

            //RAGGRUPPAMENTO IN TOOLBAR
            if (options.group) {

                var raggrup = options.group;
                for (i = raggrup.length - 1; i >= 0; i--) {
                    let col = grid.columns.find(function (v, index) { return grid.columns[index].field == raggrup[i].field; });

                    if (col == undefined) {
                        raggrup.splice(i, 1);
                    }
                }

                if (raggrup.length > 0)
                    dataSource.group(options.group);
            }

        } catch (err) {
            console.log(err);
        }
    }

    function Salva_Filtri() {

        var idControlloTestata = "tabella_scadenze"
        var griglia = [];

        var parametriGrigliaTestata = getParametriGrigliaTestata(location.pathname, idControlloTestata);

        griglia.push(CreaNuovoOggettoGriglia(idControlloTestata, parametriGrigliaTestata));

        var parametriReport = kendo.stringify({
            "_versione": versioneJsonFiltri,

            "_azienda": KendoDDL("ddlAzienda").value(),
            "_chkSoloAziendeAttive": KendoSwitch("ChkSoloAttive").value(),
            "_chkAziendaCorrente": KendoSwitch("chkAziendaCorrente").value(),

            "_area": KendoDDL("cmbArea").value(),
            "_tipologie": KendoMultisel("cmbTipologia").value().join("|"),

            "_validazione": KendoDDL("cmbValidazione").value(),
            "_chkValidazioneAttiva": KendoSwitch("ChkValidazione").value(),

            "_storico": KendoDDL("ddlStorico").value(),
            "_chkGestioneStorico": KendoSwitch("ChkGestioneStorico").value(),

            "_chkSoloMieiAllegati": KendoSwitch("chkUtenteUpload").value(),

            "_uploadDal": $('input[name$="txt_Inizio_upload"]').val()+ ($('input[name$="txt_Inizio_upload"]').val().trim().length == 10 ? " 00:00" : ""),
            "_uploadAl": $('input[name$="txt_Fine_upload"]').val() + ($('input[name$="txt_Fine_upload"]').val().trim().length == 10 ? " 23:59" : ""),

            "_inizioScadenza": $('input[name$="txt_Validita_Inizio"]').val(),
            "_fineScadenza": $('input[name$="txt_Validita_Fine"]').val(),

            "_griglia": griglia
        });

        var parametri = kendo.stringify({ "parametri": parametriReport });
        ajaxAgronica(indirizzohttp + '/SalvaFiltri', parametri,
            function (risposta) {
                if (risposta.RispostaOK) {
                    var risp = risposta.RispostaStringa;
                }
                else console.log("Errore nel salvataggio dei filtri per la ricerca documenti: " + risposta.Errore);
            }, null);

}

// #endregion