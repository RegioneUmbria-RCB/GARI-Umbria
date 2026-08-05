function CaricaGrigliaBrogliaccio(deferred) {

    //Leggo da db solo se non ho mai letto
    if ($("#divKendoBrogliaccio").html() === '') {
        var data_inizio = $(data_inizio_ClientID).val();
        var data_fine = $(data_fine_ClientID).val();
        var sa_cod = $('#ddlCentri').val();
        var veg_cod = $('#ddlSpecie').val();

        KendoBrogliaccio_leggi(undefined, data_inizio, data_fine, sa_cod, veg_cod, deferred);
    }

}

function kendoBrogliaccio_onDataBoundedRighe(e) {

    coloraRigheBrogliaccio(e);
    nascondiPulsantiBrogliaccio(e);

    //Se in mobile mostro solo la colonna unica
    mostraColonnaUnicaSeInMobile(e, 3);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe(e, 2);

    var wrapper = e.sender.wrapper,
        header = wrapper.find(".k-grid-header");

    //function resizeFixed() {
    //    var paddingRight = parseInt(header.css("padding-right"));
    //    header.css("width", wrapper.width() - paddingRight);
    //}

    //function scrollFixed() {
    //    var offset = $(this).scrollTop(),
    //        tableOffsetTop = wrapper.offset().top,
    //        tableOffsetBottom = tableOffsetTop + wrapper.height() - header.height();
    //    if (offset < tableOffsetTop || offset > tableOffsetBottom) {
    //        header.removeClass("fixed-header");
    //    } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !header.hasClass("fixed")) {
    //        header.addClass("fixed-header");
    //    }
    //}

    //resizeFixed();
    //$(window).resize(resizeFixed);
    //$(window).scroll(scrollFixed);

}

function KendoBrogliaccio_inizializza(divKendoBrogliaccio) {

    var funzioniCRUD = {
        funzioneRead: kBrogliaccioReadValorizzazione_rows//,
        //checkBoxFunction: KendoBrogliaccio_checked
    };

    var idModel = "Ricetta_Operazione_Cod";

    var campiKendoModel = kBrogliaccioReadValorizzazione_mod();
    var colonneKendoGrid = kBrogliaccioReadValorizzazione_col();

    var pulsanteCopiaBrogliaccio = "<div class='btn btn-warning btnCopia' style='display:block;width:70px;border:0px;margin-bottom:5px;' onclick=copiaBrogliaccio(this.closest('tr'),this.closest('.k-grid'))>Copia</div>";
    if (Request_QueryString("gis") === "true") {
        pulsanteCopiaBrogliaccio = "";
    }

    var pulsanteAllegati = "";
    if ($("input[name$='hf_UtenteAbilitatoGestioneVisualizaAllegato']").val() == "True") {
        pulsanteAllegati += "<div> <span class='fa fa-file-text-o fa-2x info_elem pull-left' title='Gestione Documenti' onclick=ApriKendoWindowRicercaDocumenti(this.closest('tr'),this.closest('.k-grid'))></span>";
    }
    if ($("input[name$='hf_UtenteAbilitatoGestioneNuovoAllegato']").val() == "True") {
        pulsanteAllegati += "<span class='fa fa-paperclip fa-2x info_elem pull-right' title='Aggiungi nuovo allegato'  onclick=ApriKendoWindowAggiungiNuovoAllegato(this.closest('tr'),this.closest('.k-grid')) ></span></div>";
    }

    var templateCommand = ""
    if (permessoQdC == true) {
        templateCommand = "<div class='btn btn-warning btnSalvaInQdC' style='display:block;width:70px;border:0px;margin-bottom:0px;' onclick=ApriBrogliaccioInPaginaQdC(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-right'></i><i class='fa fa-book'></i></div>"
    }
    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        toolbarCommands: ["templateLegendaMenuAgendaBrogliaccio"],
        excel: true,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: { mode: "row" },
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoBrogliaccio(this.closest('tr'),this.closest('.k-grid'))>Info</div>" +
                        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaBrogliaccio(this.closest('tr'),this.closest('.k-grid'))>Modifica</div>" +
                        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaBrogliaccio(this.closest('tr'),this.closest('.k-grid'))>Cancella</div>" +
                        pulsanteCopiaBrogliaccio + pulsanteAllegati
                }, title: "Azioni", width: "97px"
            },
            {
                command: {
                    template: templateCommand
                    //"<div class='btn btn-info btnStampaCert' style='display:block;width:70px;border:0px;' onclick=stampaCertRicetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-print'></i>Certif.</div>" +
                    //"<div class='btn btn-info btnStampaAz' style='display:block;width:70px;border:0px;' onclick=stampaAzRicetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-print'></i>Aziend.</div>" +
                    //"<div class='btn btn-info btnStampaPianoLavori' style='display:block;width:70px;border:0px;' onclick=stampaPianoLavoriRicetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-print'></i>Piano</div>" +
                    //"<div class='btn btn-warning btnCreaOp' style='display:block;width:70px;border:0px;margin-bottom:0px;' onclick=creaOpRicetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-plus'></i>Operaz.</div>" +
                }, title: "Azioni 2", width: "97px"
            }//,
            //{
            //    command: {
            //        template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoRicettaOperazione(this.closest('tr'),this.closest('.k-grid'))>Info</div>" +
            //            "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaRicettaOperazione(this.closest('tr'),this.closest('.k-grid'))>Modifica</div>" +
            //            "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaRicettaOperazione(this.closest('tr'),this.closest('.k-grid'))>Cancella</div>"
            //    }, title: "Azioni Operazioni", width: "97px"
            //}
        ]

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendoBrogliaccio_onDataBoundedRighe
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoBrogliaccio = creaKendoGrid(divKendoBrogliaccio, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    //var griglia = $('#' + divKendoBrogliaccio).data("kendoGrid");
    //griglia.dataSource.group({ field: 'Ricetta_Cod' });
    //griglia.autoFitColumn("Validita_Inizio");
    //griglia.autoFitColumn("Validita_Fine");

}

function kBrogliaccioReadValorizzazione_rows(options) {

    var data = $('#hdKendoBrogliaccio_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kBrogliaccioReadValorizzazione_col() {

    var data = $('#hdKendoBrogliaccio_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function kBrogliaccioReadValorizzazione_mod() {

    var data = $('#hdKendoBrogliaccio_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function KendoBrogliaccio_checked(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#divKendoBrogliaccio").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    dataItem.dirty = true;

    rowKendoGridSelected(row, checked)
}

function coloraRigheBrogliaccio(eventArgs) {

    var grid = eventArgs.sender;
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        //if (dataItem.WAnagraficaStati_Colore !== null) {
        //    this.style.backgroundColor = dataItem.WAnagraficaStati_Colore;
        //} else
        if (dataItem.in_uso === "1") {
            this.className += " kendoRiga_AgendaRicettaInQdC";
        } else if (dataItem.blocco_flag === "1") {
            this.className += " kendoRiga_AgendaRicettaBloccata";
        } else if (dataItem.piva === "") {
            this.className += " kendoRiga_AgendaRicettaPubblica";
        }

    });

}

function nascondiPulsantiBrogliaccio(eventArgs) {

    var grid = eventArgs.sender;
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        if (dataItem.PermessoModifica === "False") {
            $(this.querySelector(".btnModifica")).hide();
            $(this.querySelector(".btnCancella")).hide();
            $(this.querySelector(".btnCreaOp")).hide();
        }

        if (dataItem.in_uso === "1") {
            $(this.querySelector(".btnModifica")).hide();
            $(this.querySelector(".btnCancella")).hide();
            $(this.querySelector(".btnSalvaInQdC")).hide();
        }

        //TUTTI TRANNE "LINEA TECNICA"
        if (dataItem.Tipo_Ricetta !== 0) {
            $(this.querySelector(".btnCreaOp")).hide();
        }

        //PUA
        if (dataItem.Tipo_Ricetta === 2) {
            //$(this.querySelectorAll(".btn")).hide();
            $(this.querySelector("td:nth-child(1)")).html("<div class='btn btn-success' style='display:block;width:70px;border:0px;word-wrap:break-word;white-space:normal;' onclick='Gestione_Operazione_Menu(37)'>Vai al PUA Zootecnico</div>");
            $(this.querySelector("td:nth-child(2)")).html("");
        }

        //Piano Distribuzione Concimi
        if (dataItem.Tipo_Ricetta === 6) {
            //$(this.querySelectorAll(".btn")).hide();
            $(this.querySelector("td:nth-child(1)")).html("<div class='btn btn-success' style='display:block;width:70px;border:0px;word-wrap:break-word;white-space:normal;' onclick='Gestione_Operazione_Menu(24)'>Vai al Piano Concimazione</div>");
            $(this.querySelector("td:nth-child(2)")).html("");
        }


    });

}

function infoBrogliaccio(tr_elem, grid_elem) {
    var permessoLettura = check_permessoOperazione(enumBrogliaccio, enumLettura);

    if (permessoLettura == true) {
        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);

        //var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "piva": datiRiga.piva, "data_inizio": data_inizio, "data_fine": data_fine, "tipo_ricetta": datiRiga.Tipo_Ricetta });
        var parametri = kendo.stringify({ "type": 0, "data_operazione": datiRiga.Ricetta_Operazione_Data, "ricetta_cod": datiRiga.Ricetta_Cod, "ricetta_operazione_cod": datiRiga.Ricetta_Operazione_Cod, "lav_cod": datiRiga.lav_cod, "veg_cod": datiRiga.veg_cod_op, "tipo_ricetta": datiRiga.Tipo_Ricetta });

        ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/infomodifica_operazione_brogliaccio",
            parametri,
            function (risposta) {
                var url = risposta.RispostaStringa;
                window.location.href = url;
            }, null);
    }
}

function modificaBrogliaccio(tr_elem, grid_elem) {

    var permessoModifica = check_permessoOperazione(enumBrogliaccio, enumModifica);

    if (permessoModifica == true) {
        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);

        //var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "piva": datiRiga.piva, "data_inizio": data_inizio, "data_fine": data_fine, "tipo_ricetta": datiRiga.Tipo_Ricetta, "in_uso": datiRiga.in_uso });
        var parametri = kendo.stringify({ "type": 2, "data_operazione": datiRiga.Ricetta_Operazione_Data, "ricetta_cod": datiRiga.Ricetta_Cod, "ricetta_operazione_cod": datiRiga.Ricetta_Operazione_Cod, "lav_cod": datiRiga.lav_cod, "veg_cod": datiRiga.veg_cod_op, "tipo_ricetta": datiRiga.Tipo_Ricetta });

        $.ajax({
            type: 'POST',
            url: "MenuBS_Agenda_Nuovo.aspx/infomodifica_operazione_brogliaccio",
            data: parametri,
            contentType: 'application/json; charset=utf-8',
            cache: false, dataType: 'json', async: true,
            success: function (risposta) {
                if (risposta.d.RispostaOK) {
                    var url = risposta.d.RispostaStringa;
                    window.location.href = url;
                } else {
                    MessaggioErrore(risposta.d.Errore);
                }

            }
        });

        //ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/infomodifica_operazione_brogliaccio",
        //    parametri,
        //    function (risposta) {
        //        if (risposta.RispostaOK) {
        //            var url = risposta.RispostaStringa;
        //            window.location.href = url;
        //        } else {
        //            MessaggioErrore(risposta.Errore);
        //        }              

        //    }, null);
    }
}

function eliminaBrogliaccio(tr_elem, grid_elem) {

    var permessoCancellazione = check_permessoOperazione(enumBrogliaccio, enumCancellazione);

    if (permessoCancellazione == true) {
        kendo.confirm("Sei sicuro di voler eliminare l'operazione di brogliaccio selezionato?").then(function () {
            var datiGriglia = $(grid_elem).data('kendoGrid');
            var datiRiga = datiGriglia.dataItem(tr_elem);

            var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "ricetta_operazione_cod": datiRiga.Ricetta_Operazione_Cod, "in_uso": datiRiga.in_uso, "APP_Ricetta_Operazione_ID": datiRiga.APP_Ricetta_Operazione_ID });

            ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_VerificaSeCostiCollegatiECancella",
                parametri,
                function (risposta) {

                    if (risposta.RispostaOK === true) {
                        if (risposta.RispostaStringa !== "") {

                            var dialog = $("<div></div>").kendoDialog({
                                title: "Cancellazione operazione di ricetta",
                                closable: false,
                                modal: true,
                                content: "<p>All'operazione selezionata sono collegate ore/costi. Come vuoi procedere?<p>",
                                actions: [
                                    {
                                        text: 'Cancella tutto', action: function (e) {
                                            ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_CancellaRicettaCancellaCosti", parametri,
                                                function (risposta) {

                                                    if (risposta.RispostaOK === true) {
                                                        ScritturaOK('Operazioni di brogliaccio e ore/costi cancellate correttamente');
                                                        btnAggiorna_click(); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab
                                                    } else {
                                                        MessaggioErrore('Errore durante la cancellazione');
                                                    }

                                                }, null);
                                        }
                                    },
                                    {
                                        text: 'Cancella intervento e conserva ore/costi', action: function (e) {
                                            ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_CancellaRicettaConvertiCosti", parametri,
                                                function (risposta) {

                                                    if (risposta.RispostaOK === true) {
                                                        ScritturaOK('Operazione di brogliaccio cancellata correttamente');
                                                        btnAggiorna_click(); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab
                                                    } else {
                                                        MessaggioErrore('Errore durante la cancellazione');
                                                    }

                                                }, null);
                                        }
                                    },
                                    { text: 'Annulla', primary: true }
                                ]
                            });

                            $(dialog).parent().find("button").css('white-space', 'normal');
                            dialog.data("kendoDialog").open();

                        } else {
                            if (risposta.Errore !== "") {
                                MessaggioErrore(risposta.Errore); //Errore specifico per mancanza permessi cancellazione
                            } else {
                                ScritturaOK('Operazione di brogliaccio cancellata correttamente');
                                btnAggiorna_click(); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab
                            }
                        }
                    } else {
                        MessaggioErrore('Errore durante la cancellazione dell\'operazione di brogliaccio');
                    }

                }, null);

        }, null);
    }
}

function copiaBrogliaccio(tr_elem, grid_elem) {
    var permessoCopia = check_permessoOperazione(enumBrogliaccio, -1); //-1 = Copia Elemento

    if (permessoCopia == true) {
        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);

        var data_inizio = datiRiga.Validita_Inizio === null ? '' : datiRiga.Validita_Inizio;
        var data_fine = datiRiga.Validita_Fine === null ? '' : datiRiga.Validita_Fine;

        var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "piva": datiRiga.piva, "data_inizio": data_inizio, "data_fine": data_fine, "tipo_ricetta": datiRiga.Tipo_Ricetta });

        ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_Copia",
            parametri,
            function (risposta) {
                if (risposta.RispostaOK === true) {
                    ScritturaOK('Brogliaccio copiato correttamente');
                    btnAggiorna_click(); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab
                } else {
                    MessaggioErrore('Errore durante la copia del Brogliaccio');
                }
            }, null);
    }
}

function ApriBrogliaccioInPaginaQdC(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "Ricetta_Operazione_Cod": datiRiga.Ricetta_Operazione_Cod, "lav_cod": datiRiga.lav_cod, "veg_cod_op": datiRiga.veg_cod_op, "Ricetta_Operazione_Data": datiRiga.Ricetta_Operazione_Data });

    ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/ApriRicettaFattoInPaginaQdC",
        parametri,
        function (risposta) {
            var url = risposta.RispostaStringa;
            window.location.href = url;
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        });

}

function btnNuovaOperazioneBrogliaccio_click() {
    var op = $("#ddlOperazioniBrogliaccio").data("kendoDropDownList");
    var lavcod = op.value();

    if (lavcod === "") {
        alert('Selezionare un\'operazione di Brogliaccio.');
    } else {
        redirectOperazioneBrogliaccioPreferita(lavcod);
    }
}
