function CaricaGrigliaRicette(deferred) {

    //Leggo da db solo se non ho mai letto
    if ($("#divKendoRicette").html() === '') {
        var data_inizio = $(data_inizio_ClientID).val();
        var data_fine = $(data_fine_ClientID).val();
        var sa_cod = $('#ddlCentri').val();
        var veg_cod = $('#ddlSpecie').val();

        var TipoOperazione = $('#ddlTipoOperazione').data("kendoMultiSelect").value();
        var impianti = $('#ddlImpianti').data("kendoMultiSelect").value();

        KendoRicette_leggi(undefined, data_inizio, data_fine, sa_cod, veg_cod, deferred, TipoOperazione, impianti);
    }

}

function kendoRicette_onDataBoundedRighe(e) {

    coloraRigheRicette(e);
    nascondiPulsantiRicette(e);

    //Se in mobile mostro solo la colonna unica
    mostraColonnaUnicaSeInMobile(e, 3);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe(e, 3);

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

function KendoRicette_inizializza(divKendoRicette) {

    var funzioniCRUD = {
        funzioneRead: kRicetteReadValorizzazione_rows//,
        //checkBoxFunction: KendoRicette_checked
    };

    var idModel = "Ricetta_Cod";

    var campiKendoModel = kRicetteReadValorizzazione_mod();
    var colonneKendoGrid = kRicetteReadValorizzazione_col();

    var jsLblInfo = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblInfo", "Info");
    var jsLblModifica = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblModifica", "Modifica");
    var jsLblCancella = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblCancella", "Cancella");
    var jsLblCopia = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblCopia", "Copia");
    var jsLblRicetta = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblRicetta", "Ricetta");
    var jsLblBrogliaccio = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "anchor_brogliaccio.Text", "Brogliaccio");

    var pulsanteCopiaRicette = "<div class='btn btn-warning btnCopia' style='display:block;width:70px;border:0px;margin-bottom:5px;' onclick=copiaRicetta(this.closest('tr'),this.closest('.k-grid'))>" + jsLblCopia + "</div>";
    if (Request_QueryString("gis") === "true") {
        pulsanteCopiaRicette = "";
    }



    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };

    var templateCommand = "<div class='btn btn-info btnStampaCert' style='display:block;width:110px;border:0px;' onclick=stampaCertRicetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-print'></i>" + jsLblRicetta + "</div>" +
        //"<div class='btn btn-info btnStampaAz' style='display:block;width:70px;border:0px;' onclick=stampaAzRicetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-print'></i>Aziend.</div>" +
        "<div class='btn btn-info btnStampaPianoLavori' style='display:block;width:70px;border:0px;' onclick=stampaPianoLavoriRicetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-print'></i>OdL</div>"
    //"<div class='btn btn-warning btnCreaOp' style='display:block;width:70px;border:0px;margin-bottom:0px;' onclick=creaOpRicetta(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-plus'></i>Operaz.</div>" 
    if (permessoRicetta == true) {
        templateCommand += "<div class='btn btn-warning btnApplicaRicettaDaFareAgenda' style='display:block;width:70px;border:0px;' onclick=ApriRicettaInPaginaQdC(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-right'></i>QdC</div>"
    }

    if (permessoBrogliaccio == true) {
        templateCommand += "<div class='btn btn-warning btnApplicaRicettaDaFareBrogliaccio' style='display:block;width:110px;border:0px;' onclick=ApriRicettaDaFareInPaginaFatto(this.closest('tr'),this.closest('.k-grid'))><i class='fa fa-arrow-right'></i>" + jsLblBrogliaccio + "</div>"
    }

    if ($("input[name$='hf_UtenteAbilitatoGestioneVisualizaAllegato']").val() == "True") {
        templateCommand += "<div> <span class='fa fa-file-text-o fa-2x info_elem pull-left' title='Gestione Documenti' onclick=ApriKendoWindowRicercaDocumenti(this.closest('tr'),this.closest('.k-grid'))></span>";
    }

    if ($("input[name$='hf_UtenteAbilitatoGestioneNuovoAllegato']").val() == "True") {
        templateCommand += "<span class='fa fa-paperclip fa-2x info_elem pull-right' title='Aggiungi nuovo allegato'  onclick=ApriKendoWindowAggiungiNuovoAllegato(this.closest('tr'),this.closest('.k-grid')) ></span></div>";
    }

    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        toolbarCommands: ["templateLegendaMenuAgendaRicette"],
        excel: true,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: { mode: "row" },
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoRicetta(this.closest('tr'),this.closest('.k-grid'))>" + jsLblInfo + "</div>" +
                        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaRicetta(this.closest('tr'),this.closest('.k-grid'))>" + jsLblModifica + "</div>" +
                        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaRicetta(this.closest('tr'),this.closest('.k-grid'))>" + jsLblCancella + "</div>" +
                        pulsanteCopiaRicette
                }, title: "Azioni", width: "97px"
            },
            {
                command: {
                    template: templateCommand
                }, title: "Azioni 2", width: "140px"
            }
        ]

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendoRicette_onDataBoundedRighe
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoRicette = creaKendoGrid(divKendoRicette, // rappresenta l'ID del div a cui si associa la griglia
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

    //var griglia = $('#' + divKendoRicette).data("kendoGrid");
    //griglia.dataSource.group({ field: 'Ricetta_Cod' });
    //griglia.autoFitColumn("Validita_Inizio");
    //griglia.autoFitColumn("Validita_Fine");

}

function kRicetteReadValorizzazione_rows(options) {

    var data = $('#hdKendoRicette_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kRicetteReadValorizzazione_col() {

    var data = $('#hdKendoRicette_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function kRicetteReadValorizzazione_mod() {

    var data = $('#hdKendoRicette_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function KendoRicette_checked(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#divKendoRicette").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    dataItem.dirty = true;

    rowKendoGridSelected(row, checked)
}

function coloraRigheRicette(eventArgs) {

    var grid = eventArgs.sender;
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        //if (dataItem.WAnagraficaStati_Colore !== null) {
        //    this.style.backgroundColor = dataItem.WAnagraficaStati_Colore;
        if (dataItem.in_uso === "1") {
            this.className += " kendoRiga_AgendaRicettaDaFareInBrogliaccio";
        } else if (dataItem.blocco_flag === "1") {
            this.className += " kendoRiga_AgendaRicettaBloccata";
        } else if (dataItem.piva === "") {
            this.className += " kendoRiga_AgendaRicettaPubblica";
        }
    });

}

function nascondiPulsantiRicette(eventArgs) {

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
            $(this.querySelector(".btnApplicaRicettaDaFareBrogliaccio")).hide();
            $(this.querySelector(".btnApplicaRicettaDaFareAgenda")).hide();
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
            //$(this.querySelector("td:nth-child(2)")).html("");
            $(this.querySelector(".btnStampaCert")).hide();
            $(this.querySelector(".btnStampaPianoLavori")).hide();
        }


    });

}

function infoRicetta(tr_elem, grid_elem) {

    var permessoLettura = check_permessoOperazione(enumRicette, enumLettura);

    if (permessoLettura == true) {

        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);

        //var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "piva": datiRiga.piva, "data_inizio": data_inizio, "data_fine": data_fine, "tipo_ricetta": datiRiga.Tipo_Ricetta });
        var parametri = kendo.stringify({ "type": 0, "data_operazione": datiRiga.Ricetta_Operazione_Data, "ricetta_cod": datiRiga.Ricetta_Cod, "ricetta_operazione_cod": datiRiga.Ricetta_Operazione_Cod, "lav_cod": datiRiga.lav_cod, "veg_cod": datiRiga.veg_cod_op, "tipo_ricetta": datiRiga.Tipo_Ricetta });

        ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/infomodifica_operazione_ricetta",
            parametri,
            function (risposta) {
                var url = risposta.RispostaStringa;
                window.location.href = url;
            }, null);
        { }
    }
}

function modificaRicetta(tr_elem, grid_elem) {

    var permessoModifica = check_permessoOperazione(enumRicette, enumModifica);

    if (permessoModifica == true) {
        var datiGriglia = $(grid_elem).data('kendoGrid');
        var datiRiga = datiGriglia.dataItem(tr_elem);

        //var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "piva": datiRiga.piva, "data_inizio": data_inizio, "data_fine": data_fine, "tipo_ricetta": datiRiga.Tipo_Ricetta, "in_uso": datiRiga.in_uso });
        var parametri = kendo.stringify({
            "type": 2,
            "data_operazione": datiRiga.Ricetta_Operazione_Data,
            "ricetta_cod": datiRiga.Ricetta_Cod,
            "ricetta_operazione_cod": datiRiga.Ricetta_Operazione_Cod,
            "lav_cod": datiRiga.lav_cod,
            "veg_cod": datiRiga.veg_cod_op,
            "tipo_ricetta": datiRiga.Tipo_Ricetta
        });

        //ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/infomodifica_operazione_ricetta",
        //    parametri,
        //    function (risposta) {
        //        var url = risposta.RispostaStringa;
        //        window.location.href = url;
        //    }, null);

        $.ajax({
            type: 'POST',
            url: "MenuBS_Agenda_Nuovo.aspx/infomodifica_operazione_ricetta",
            data: parametri,
            contentType: 'application/json; charset=utf-8',
            cache: false, dataType: 'json', async: true,
            success: function (r) {
                if (r.d.RispostaOK)
                    window.location = r.d.RispostaStringa;
                else
                    MessaggioErrore(r.d.Errore);
            }
        });
    }
}

function eliminaRicetta(tr_elem, grid_elem) {

    var permessoCancellazione = check_permessoOperazione(enumRicette, enumCancellazione);

    if (permessoCancellazione == true) {
        kendo.confirm("Sei sicuro di voler eliminare l'operazione di ricetta selezionata?").then(function () {

            var datiGriglia = $(grid_elem).data('kendoGrid');
            var datiRiga = datiGriglia.dataItem(tr_elem);

            var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "ricetta_operazione_cod": datiRiga.Ricetta_Operazione_Cod, "in_uso": datiRiga.in_uso, "isBrogliaccio": false });

            ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_Cancella",
                parametri,
                function (risposta) {
                    if (risposta.RispostaOK === true) {
                        if (risposta.Errore !== "") {
                            MessaggioErrore(risposta.Errore); //Errore specifico per mancanza permessi cancellazione
                        } else {
                            ScritturaOK('Operazione di ricetta cancellata correttamente');
                            btnAggiorna_click(); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab
                        }
                    } else {
                        MessaggioErrore('Errore durante la cancellazione dell\'operazione di ricetta');
                    }

                }, null);

        }, null);
    }
}

function copiaRicetta(tr_elem, grid_elem) {
    var permessoCopia = check_permessoOperazione(enumRicette, -1); //-1 = Copia Elemento

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
                    ScritturaOK('Ricetta copiata correttamente');
                    btnAggiorna_click(); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab
                } else {
                    MessaggioErrore('Errore durante la copia della Ricetta');
                }
            }, null);
    }
}

function creaOpRicetta(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var data_inizio = datiRiga.Validita_Inizio === null ? '' : datiRiga.Validita_Inizio;
    var data_fine = datiRiga.Validita_Fine === null ? '' : datiRiga.Validita_Fine;

    var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "piva": datiRiga.piva, "data_inizio": data_inizio, "data_fine": data_fine, "tipo_ricetta": datiRiga.Tipo_Ricetta });

    ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_CreaOperazioni",
        parametri,
        function (risposta) {
            var url = risposta.RispostaStringa;
            window.location.href = url;
        }, null);

}

function stampaCertRicetta(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "ricetta_stampa_tipo": 1 });

    ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_Stampa",
        parametri,
        function (risposta) {
            var url = risposta.RispostaStringa;
            window.open(url, 'Ricetta', 'height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes');
        }, null);

}

function stampaAzRicetta(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "ricetta_stampa_tipo": 2 });

    ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_Stampa",
        parametri,
        function (risposta) {
            var url = risposta.RispostaStringa;
            window.open(url, 'Ricetta', 'height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes');
        }, null);

}

function stampaPianoLavoriRicetta(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "ricetta_stampa_tipo": 3 });

    ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/Ricette_Stampa",
        parametri,
        function (risposta) {
            var url = risposta.RispostaStringa;
            window.open(url, 'Ricetta', 'height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes');
        }, null);

}

function popolaDdlRicette(IDControllo) {

    $(IDControllo).kendoDropDownList({
        dataSource: [{ ric_cod: 5, ric_des: "Ricetta Aziendale" },
        { ric_cod: 0, ric_des: "Linea Tecnica" },
        { ric_cod: 8, ric_des: "Pianifica Attività" }],
        dataTextField: "ric_des",
        dataValueField: "ric_cod",
        optionLabel: { "ric_des": "SELEZIONA...", "ric_cod": "" },
        autoBind: false,
        autoWidth: ($(window).width() >= 768) ? true : false
    });
}

function ApriRicettaDaFareInPaginaFatto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var parametri = kendo.stringify({ "ricetta_cod": datiRiga.Ricetta_Cod, "Ricetta_Operazione_Cod": datiRiga.Ricetta_Operazione_Cod, "lav_cod": datiRiga.lav_cod, "veg_cod_op": datiRiga.veg_cod_op, "Ricetta_Operazione_Data": datiRiga.Ricetta_Operazione_Data });

    ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/ApriRicettaDaFareInPaginaFatto",
        parametri,
        function (risposta) {
            var url = risposta.RispostaStringa;
            window.location.href = url;
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        });
}


function ApriRicettaInPaginaQdC(tr_elem, grid_elem) {

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


function btnNuovaOperazioneRicetta_click() {
    var op = $("#ddlOperazioniRicette").data("kendoDropDownList");
    var lavcod = op.value();

    if (lavcod === "") {
        alert('Selezionare un\'operazione di Ricetta.');
    } else {
        redirectOperazioneRicettaPreferita(lavcod);
    }
}