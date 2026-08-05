

var indirizzohttp = "./MenuBS_WS.aspx";

function SetDataItemEntity(Entity, dataItems) {
    dataItems.push({
        id: Entity.ID,
        TipoDocumento: "xxx",
        Operazione: Entity.Name,
        Lotto: Entity.Lotto,
        DataDocumento: Entity.DataDocumento,
        NumeroDocumento: Entity.NumeroDocumento,
        IntestatarioDocumento: Entity.IntestatarioDocumento,
        StartEntity: Entity.StartChainEntity,
        X: Entity.X,
        Y: Entity.Y
    });
}

function SetDataItemEntityLink(EntityLink, dataLink) {
    dataLink.push({ from: EntityLink.StartLink, to: EntityLink.EndLink, label: EntityLink.Qty + " (" + EntityLink.UM+")" });
}

function visualTemplate(options) {
    var dataviz = kendo.dataviz;
    var dataItem = options.dataItem;
    var g = new dataviz.diagram.Group();

    //g.position(new kendo.dataviz.diagram.Point(dataItem.X, dataItem.Y));

    g.drawingElement.options.tooltip = {
        shared: true,
        position: "bottom",
        content: function (e) {
            let target = e.target; // the element for which the tooltip is shown

            let content = "<div style='padding: 15px; background-color: #f0f9ff; border-color: #f0f9ff; color: #50607f;'>";

            if (dataItem.IntestatarioDocumento != '') {
                content += "<div style='justify-self: end;'>Intestatario Documento: " + dataItem.IntestatarioDocumento + "</div>";
            }
            if (dataItem.NumeroDocumento != '') {
                content += "<div style='justify-self: end;'>Numero Documento: " + dataItem.NumeroDocumento + "</div>";
            }
            if (dataItem.DataDocumento != '') {
                content += "<div style='justify-self: end;'>Data Documento: " + dataItem.DataDocumento + "</div>";
            }

            content += "<div style='justify-self: end;'>Lotto: " + dataItem.Lotto + "</div>";
            content += "</div>";

            return content;
        }
    };

    if (dataItem.StartEntity == true) {
        g.append(new dataviz.diagram.Rectangle({
            width: 210,
            height: 60,
            fill: {
                color: "Orange"
            }
        }));
    } else {
        g.append(new dataviz.diagram.Rectangle({
            width: 250,
            height: 60,
            fill: {
                color: "Orange"
            }
        }));
    }

    g.append(new dataviz.diagram.TextBlock({
        text: dataItem.Operazione,
        x: 15,
        y: 10,
        fill: "#000"
    }));
    g.append(new dataviz.diagram.TextBlock({
        text: dataItem.Lotto,
        x: 15,
        y: 30,
        fill: "#000"
    }));

    return g;
}

//chiamata per POC
function exportPOC() {
    var TrackCode = $("#txt_TrackCode").val().toUpperCase();
    var TipoLotto = 4;
    
    if (TrackCode == "") {
        kendo.alert("Nessun dato indicato nella casella Ricerca.");
        return;
    }

    // Ricerca di un eventuale riga di entrata o uscita scelta
    var CalCodSelected = 0;
    var IdMovDetSelected = 0;
    var righeSelezionate = 0;

    var grid = $("#tab_testata_righe_movimenti").data("kendoGrid");

    if (grid !== undefined) {
        var currentData = grid.dataSource.data();

        for (x = 0; x < currentData.length; x++) {
            item = currentData[x];
            if (item.Selected) {
                if (righeSelezionate == 0)
                    CalCodSelected += item.Cal_Cod;
                IdMovDetSelected += item.Id_Mov_Det;
                righeSelezionate++;
            }
        }
    }

    var trovatoErrore = false;
    if (righeSelezionate == 0) {
        if ($('input[name$="ddlCertif"]').data("kendoDropDownList").value() != "0") {
            trovatoErrore = true;
            MessaggioErrore_Bootstrap("E' possibile fare il filtro per certificazione solo scegliendo almeno una riga documento", "DIV_Messaggi");
        }

    }
    else if (righeSelezionate > 1) {
        trovatoErrore = true;
        MessaggioErrore_Bootstrap("E' possibile selezionare solo una riga di documento", "DIV_Messaggi");

    }

    var ForzaNuovaEsecuzioneAlgoritmoRintraccia = $("#ForzaNuovaEsecuzioneAlgoritmoRintraccia").is(':checked');
    var VistaAlbero = false;
    var FromOutToIn = $("#cbFromOutToIn").is(':checked');
    var cCertificazione = 0;
    if ($('input[name$="ddlCertif"]').data("kendoDropDownList").value() != "0")
        cCertificazione = parseInt($('input[name$="ddlCertif"]').data("kendoDropDownList").value());
    
    var parametri = kendo.stringify({
        TrackCode: TrackCode,
        TipoLotto: TipoLotto,
        ForzaNuovaEsecuzioneAlgoritmoRintraccia: ForzaNuovaEsecuzioneAlgoritmoRintraccia,
        FromOutToIn: FromOutToIn,
        cCertificazione: cCertificazione,
        CalCodSelected: CalCodSelected,
        IdMovDetSelected: IdMovDetSelected,
        Modalita: Exp_Tracciabilita_Modalita,
        DestinazionePath: Exp_Tracciabilita_DestinazionePath,
        LinkWSEsterno: Exp_Tracciabilita_LinkWSEsterno,
        WSEsternoParametri: Exp_Tracciabilita_WSEsternoParametri
    });


    if (!trovatoErrore) {
        ajaxAgronica(indirizzohttp + "/exportPOC", parametri,
            function (risposta) {
                var msg_d = risposta.RispostaStringa;

                if (msg_d == "Zero") {

                    $.logThis("Nessun risultato.");
                    $('#tabOpAgenda').html('Nessun Risultato.');
                } else {
                    kendo.alert(msg_d);
                }
            }, null);
    }
}



//per chiamata Standard Ajax
function track() {

    var TrackCode = $("#txt_TrackCode").val().toUpperCase();
    var TipoLotto = $(iTipoLotto).val();
    var TipoVisualizzazione = $("#Cmb_TipoVisualizzazione").val();

    if (TrackCode == "") {
        kendo.alert("Nessun dato indicato nella casella Ricerca.");
        return;
    }

    // Ricerca di un eventuale riga di entrata o uscita scelta
    var CalCodSelected = 0;
    var IdMovDetSelected = 0;
    var righeSelezionate = 0;

    var grid = $("#tab_testata_righe_movimenti").data("kendoGrid");

    if (grid !== undefined) {
        var currentData = grid.dataSource.data();

        for (x = 0; x < currentData.length; x++) {
            item = currentData[x];
            if (item.Selected) {
                if (righeSelezionate == 0)
                    CalCodSelected += item.Cal_Cod;
                IdMovDetSelected += item.Id_Mov_Det;
                righeSelezionate++;
            }
        }
    }

    var trovatoErrore = false;
    if (righeSelezionate == 0) {
        if ($('input[name$="ddlCertif"]').data("kendoDropDownList").value() != "0") {
            trovatoErrore = true;
            MessaggioErrore_Bootstrap("E' possibile fare il filtro per certificazione solo scegliendo almeno una riga documento", "DIV_Messaggi");
        }

    }
    else if (righeSelezionate > 1) {
        trovatoErrore = true;
        MessaggioErrore_Bootstrap("E' possibile selezionare solo una riga di documento", "DIV_Messaggi");

    }

    var ForzaNuovaEsecuzioneAlgoritmoRintraccia = $("#ForzaNuovaEsecuzioneAlgoritmoRintraccia").is(':checked');
    var VistaAlbero = false;
    var FromOutToIn = $("#cbFromOutToIn").is(':checked');
    var cCertificazione = 0;
    if ($('input[name$="ddlCertif"]').data("kendoDropDownList").value() != "0")
        cCertificazione = parseInt($('input[name$="ddlCertif"]').data("kendoDropDownList").value());

    if (!trovatoErrore) {

        switch (TipoVisualizzazione) {
            case "1":
                //Visualizzazione Tabellare
                ajaxAgronica(indirizzohttp + "/track", "{ TrackCode: '" + TrackCode + "', TipoLotto: '" + TipoLotto + "', ForzaNuovaEsecuzioneAlgoritmoRintraccia: " + ForzaNuovaEsecuzioneAlgoritmoRintraccia + ", FromOutToIn: " + FromOutToIn + ", cCertificazione: " + cCertificazione + ", CalCodSelected: " + CalCodSelected + ", IdMovDetSelected: " + IdMovDetSelected + "  }",
                    function (risposta) {
                        var msg_d = risposta.RispostaStringa;

                        if (msg_d == "Zero") {

                            $.logThis("Nessun risultato.");
                            $('#tabOpAgenda').html('Nessun Risultato.');
                        }
                        else {
                            $('#tab_lotti').html('');
                            $("#tabOpAgenda").html('');
                            $("#diaOpAgenda").html('');
                            $("#tabOpAgenda").show();
                            $("#diaOpAgenda").hide();

                            $("#pannello-RicLotti").collapse('hide');
                            $("#Alto2").show();
                            $("#pannello-RicRisultati").collapse('show');

                            $("#IntestazioneRintracciabilita").html(risposta.Intestazione);

                            if (risposta.ImmagineProdotto != "" && risposta.ImmagineProdotto != null && risposta.ImmagineProdotto != undefined) {
                                $("#ImmagineRintracciabilita").html("<img style='height: 150px;' src='" + risposta.ImmagineProdotto + "' >");
                            }


                            $('input[name$="hdKendo_Tracking"]').val(risposta.RispostaStringa);

                            if (VistaAlbero) {
                                // Versione TreeView
                                var funzioniCRUD = { funzioneRead: kReadTrack_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
                                var idModel = "FF_Track_Cal_Cod";
                                var idModelParent = "FF_Track_Cal_Cod_Padre";
                                var campiKendoModel = kReadTrack_mod();
                                var colonneKendoTreeList = kReadTrack_col();
                                var parametriPerLettura = [VistaAlbero, TrackCode];
                                var parametriDataSource = {
                                };
                                var parametriKendoTreeList = {
                                    //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
                                    columnMenu: true
                                };
                                var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundTrack, funzioneDaChiamarePrimaDiExcelExport: excelExportTrack };

                                creaKendoTreeList("tabOpAgenda", // rappresenta l'ID del div a cui si associa la griglia
                                    funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
                                    idModel, // chiave riga 
                                    idModelParent, // padre tree list   
                                    campiKendoModel, // campi modello
                                    colonneKendoTreeList, // colonne da mostrare
                                    parametriPerLettura, // parametri da passare alla lettura
                                    parametriDataSource, // parametri data source { chiave - valore}
                                    parametriKendoTreeList,   // parametri griglia [{ chiave - valore}]
                                    funzioniPrimaDopoEventi
                                );
                                // Fine Versione TreeView
                            }
                            else {
                                // Versione KendoGrid
                                var funzioniCRUD = { funzioneRead: kReadTrack_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
                                var idModel = "id_mov_det";
                                var campiKendoModel = kReadTrack_mod();
                                var colonneKendoGrid = kReadTrack_col();
                                var parametriPerLettura = null;
                                var parametriDataSource = {
                                };
                                var parametriKendoGrid = {
                                    salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
                                    columnMenu: true,
                                    groupable: false
                                };
                                var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundTrack, funzioneDaChiamarePrimaDiExcelExport: excelExportTrack };
                                var mostraRigheCancellate = false;
                                var colonneDisabilitateSoloInModifica = null;


                                var grid = creaKendoGrid("tabOpAgenda",
                                    funzioniCRUD,
                                    idModel,
                                    campiKendoModel,
                                    colonneKendoGrid,
                                    parametriPerLettura,
                                    parametriDataSource,
                                    parametriKendoGrid,
                                    funzioniPrimaDopoEventi,
                                    mostraRigheCancellate,
                                    colonneDisabilitateSoloInModifica
                                );
                                // Fine Versione KendoGrid
                            }


                            // Nascondo la tabella righe movimenti e rimetto a 0 il flag Certificazioni
                            if ($("#tab_testata_righe_movimenti").data("kendoGrid") != undefined && $("#tab_testata_righe_movimenti").data("kendoGrid") != null) {
                                $("#tab_testata_righe_movimenti").data("kendoGrid").destroy();
                                $("#tab_testata_righe_movimenti").empty();
                                $('input[name$="ddlCertif"]').data("kendoDropDownList").value(0);
                            }

                            //codice success
                            //AgroWA_Table_sistemaDati(dd); //aggiusto i dati in base al tipo
                            //watableOpAgenda = $("#tabOpAgenda").WATable({
                            //    pageSize: 100,
                            //    pageSizes: [5, 10, 20, 40],
                            //    filter: true,
                            //    preFill: false,
                            //    checkboxes: true,
                            //    types: {
                            //        string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                            //        date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                            //        number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                            //        bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                            //    },
                            //    tableCreated: function (data) {
                            //        $.logThis("tabella in tabOpAgenda creata!");
                            //        impostaTabella("#tabOpAgenda");
                            //        nascondiColonne_tabOpAgenda();

                            //    }
                            //}).data('WATable').setData(dd);

                            //InitWaTable("#tabOpAgenda", watableOpAgenda);
                            //NomeColonnaDataTable("#tabOpAgenda", watableOpAgenda);
                            //nascondiColonne_tabOpAgenda();
                            //generaElencoLavorazioni();
                        }
                    }, null);
                break;
            case "2":
                //Visualizzazione Grafo
                ajaxAgronica(indirizzohttp + "/trackDiagram", "{ TrackCode: '" + TrackCode + "', TipoLotto: '" + TipoLotto + "', ForzaNuovaEsecuzioneAlgoritmoRintraccia: " + ForzaNuovaEsecuzioneAlgoritmoRintraccia + ", FromOutToIn: " + FromOutToIn + ", cCertificazione: " + cCertificazione + ", CalCodSelected: " + CalCodSelected + ", IdMovDetSelected: " + IdMovDetSelected + "  }",
                    function (risposta) {
                        var msg_d = risposta.RispostaStringa;
                        if (msg_d == "Zero") {

                            $.logThis("Nessun risultato.");
                            $('#tabOpAgenda').html('Nessun Risultato.');
                        }
                        else {
                            $('#tab_lotti').html('');
                            $("#tabOpAgenda").html('');
                            $("#diaOpAgenda").html('');
                            $("#tabOpAgenda").hide();
                            $("#diaOpAgenda").show();

                            $("#pannello-RicLotti").collapse('hide');
                            $("#Alto2").show();
                            $("#pannello-RicRisultati").collapse('show');

                            $("#IntestazioneRintracciabilita").html(risposta.Intestazione);

                            if (risposta.ImmagineProdotto != "") {
                                $("#ImmagineRintracciabilita").html("<img style='height: 150px;' src='" + risposta.ImmagineProdotto + "' >");
                            }

                            var dd = jQuery.parseJSON(msg_d);
                            var dataItems = [];
                            var dataLinks = [];
                            dd.EntityList.forEach(Entity => SetDataItemEntity(Entity, dataItems));
                            dd.EntityLinkList.forEach(EntityLink => SetDataItemEntityLink(EntityLink, dataLinks));

                            $("#diaOpAgenda").kendoDiagram({
                                shapeDefaults: {
                                    editable: {
                                        tools: false
                                    },
                                    visual: visualTemplate
                                },
                                dataSource: {
                                    data: dataItems,
                                    schema: {
                                        model: {
                                            id: "id",
                                            fields: {
                                                id: { from: "id", type: "number" },
                                                TipoDocumento: { type: "string" },
                                                NumeroDocumento: { type: "string" },
                                                DataDocumento: { type: "string" },
                                                Lotto: { type: "string" }
                                            }
                                        }
                                    }
                                },
                                connectionsDataSource: dataLinks,
                                layout: {
                                    alignContent: "center",
                                    alignItems: "center",
                                    type: "tree",
                                    subtype: "down"
                                },
                                connectionDefaults: {
                                    stroke: {
                                        color: "#979797",
                                        width: 1
                                    },
                                    type: "polyline",
                                    startCap: "FilledCircle",
                                    endCap: "ArrowEnd",
                                    content: {
                                        template: "#= label#"
                                    }
                                },
                                editable: {
                                    tools: false
                                },
                                pannable: false,
                                zoomRate: 0,
                                dataBound: function () {
                                    var bbox = this.boundingBox();
                                    this.wrapper.width(bbox.width + bbox.x + 50);
                                    this.wrapper.height(bbox.height + bbox.y + 50);
                                    this.resize();
                                },
                                change: function (e) {
                                    if (e.added.length === 1) {
                                        var shape = e.added[0];

                                        // shape.model will be renamed to shape.dataItem
                                        var dataItem = shape.dataItem || shape.model;

                                        shape.position({ x: dataItem.X, y: dataItem.Y });
                                    }
                                }
                            });
                        }
                    }, null);
                break;
            default:
                Kendo.alert("Opzione di visualizzazione non mappata");
                break;
        }
    }

}

function kReadTrack_rows(options, parametriPerLettura) {
    VistaAlbero = false;
    TrackCode = "";
    if (parametriPerLettura != null && parametriPerLettura.length == 2) {
        VistaAlbero = parametriPerLettura[0];
        TrackCode = parametriPerLettura[1];
    }

    var data = $('input[name$="hdKendo_Tracking"]').val();
    jSonParsed_Kendo = JSON.parse(data);

    if (VistaAlbero) {
        //console.log(data);
        jSonParsed_Kendo.kendo_rows.unshift({
            Data: "",
            Dettagli: "",
            FF_Track_Cal_Cod: "0",
            FF_Track_Cal_Cod_Padre: "",
            FF_Track_Lotto: TrackCode,
            FF_Track_Lotto_Padre: "",
            FF_Track_Qta_Extra_Totale: 0,
            Lav_cod: "",
            Lav_des: "",
            Operazione_DES: "",
            Piva: "",
            Rag_Soc: "",
            Veg_cod: "",
            blocco_flag: "",
            gru_des: "",
            sa_cod: ""
        })
    }

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadTrack_col() {

    var data = $('input[name$="hdKendo_Tracking"]').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadTrack_mod() {

    var data = $('input[name$="hdKendo_Tracking"]').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}

function onDataBoundTrack(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    }

    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        if (dataItem.get("FF_Righe_Aggiunte") != null) {
            if (dataItem.get("FF_Righe_Aggiunte") == "W") {
                row.addClass("rowKendoWarning");
            }
            if (dataItem.get("FF_Righe_Aggiunte") == "C") {
                row.addClass("rowKendoCritical");
            }
        }
    }
}

function excelExportTrack(e) {
    var titolo = $("#IntestazioneRintracciabilita").html();
    var sheet = e.workbook.sheets[0];
    e.workbook.fileName = "Export tracciabilità " + titolo + ".xlsx";
    var nrColonnaRigheAggiunte = sheet.columns.length - 1;
    for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
        var row = sheet.rows[rowIndex];
        if (row.cells[nrColonnaRigheAggiunte].value == "W" || row.cells[nrColonnaRigheAggiunte].value == "C") {
            var colore = "#fda";
            if (row.cells[nrColonnaRigheAggiunte].value == "C")
                colore = "#fdd";

            for (var colIndex = 0; colIndex < (sheet.columns.length); colIndex++) {
                row.cells[colIndex].background = colore;

            }
        }
        for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
            //replace <br> with crlf solo per le caselle di testo, altrimenti va in crash
            if (Object.prototype.toString.call(row.cells[cellIndex].value) === '[object String]') {
                row.cells[cellIndex].value = (row.cells[cellIndex].value).replaceAll('<br>', '\n').replaceAll('<br/>', '\n');
                //testo a capo
                row.cells[cellIndex].wrap = true;
                //altezza della riga in base al numero
                var hRow = row.cells[cellIndex].value.split('\n').length;
                row.height = hRow > 1 ? hRow * 20 : 20
            }

        }
    }
}

function RicercaRigheDocumenti() {

    var cCertificazione = 0;
    if ($('input[name$="ddlCertif"]').data("kendoDropDownList").value() != "0")
        cCertificazione = parseInt($('input[name$="ddlCertif"]').data("kendoDropDownList").value());

    var docNr = 0;
    if ($('input[name$="TxtDocNumero"]').val() != null && $('input[name$="TxtDocNumero"]').val() != "")
        docNr = parseInt($('input[name$="TxtDocNumero"]').val().replace(".", ""));

    var param = "{ piva: ' ',  lotto: '" + $("#txt_TrackCode").val().toUpperCase() +
        "',  docNumeroSin: '" + $('input[name$="TxtDocNumeroSin"]').val() +
        "',  docNumero: " + docNr +
        ",  docNumeroDes: '" + $('input[name$="TxtDocNumeroDes"]').val() +
        "',  nrRiga: '" + $('input[name$="TxtNrRiga"]').val() +
        "',  fromOutToIn: " + $("#cbFromOutToIn").is(':checked') +
        ",  certificazione: " + cCertificazione +
        "  }";

    var risp = "";
    ajaxAgronica(indirizzohttp + "/CaricaGrigliaRigheDocumenti",
        param,
        function (risposta) {
            $('input[name$="hdKendo_RigheMovimento"]').val(risposta.RispostaStringa);
            popolaGrigliaMovimenti("tab_testata_righe_movimenti");

            $("#tabOpAgenda").data("kendoGrid").destroy();
            $("#tabOpAgenda").empty();

        }, null);
}

function RicercaCertificazione(options) {
    var valoriParamQual = RicercaValoriParametriQualitativi(12, $(cIdPiva).val());
    options.success(valoriParamQual);
}