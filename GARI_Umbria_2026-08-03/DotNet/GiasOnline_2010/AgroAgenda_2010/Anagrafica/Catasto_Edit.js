// disabilitazione del tasto BACK 
document.onkeypress = function (event) {
    if (typeof window.event != 'undefined') { // ie
        event = window.event;
        event.target = event.srcElement; // make ie confirm to standards !!
    }
    var kc = event.keyCode;
    var tt = event.target.type;

    if ((kc != 8) || (tt == 'text') || (tt == 'password') || (tt == 'textarea'))
        return true;
    alert('Disabilitato il tasto BACK della tastiera');
    return false;
}

if (typeof window.event != 'undefined') // ie
    document.onkeydown = document.onkeypress; // Trap bksp in ie. !! Note: does not trap enter, but onkeypress does !!


function ValidaxSubmit() {

    var flag = controlla_form();

    if (flag) {

        var cod_prov = $('#Cmb_Provincia option:selected').text();
        cod_prov = cod_prov.substring(4, cod_prov.indexOf(')'));
        var cod_cod = $('#Cmb_Comune').val();
        cod_cod = cod_cod.substring(3, cod_cod.length - 2);

        // Salvo in session i codici di Provincia e Comune
        $.ajax({
            type: 'POST',
            url: 'Catasto_Edit.aspx/Salva_ProvCom_Estero',
            data: "{cod_prov: '" + cod_prov + "', cod_com: '" + cod_cod + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: false,
            success: function (r) {
            }
        });

        // Salvo in session la griglia dei metodi di produzione associati alla particella catastale
        var kGrid = KendoGrid("grigliaMetodiProduzione");
        if (kGrid !== undefined && kGrid !== null) {
            var dtGrid = kGrid.dataSource.data();

            //var _righeInserite = dtGrid.filter((el) => { return el.dirty == true && el.Chiave == "" && (!el.deleted) });
            //var _righeModificate = dtGrid.filter((el) => { return el.dirty == true && el.Chiave != "" && (!el.deleted) });
            //var _righeCancellate = dtGrid.filter((el) => { return el.Chiave != "" && el.deleted == true });
            var _righeToccate = dtGrid.filter((el) => { return el.dirty == true || el.deleted == true });
            var _righeNonCancellate = dtGrid.filter((el) => { return el.deleted === undefined || el.deleted === null || el.deleted == false });

            var grigliaMod = false;
            if (_righeToccate.length > 0) {
                grigliaMod = true;
            }

            var parametri = {
                righeGrigliaNonEliminate: JSON.stringify(_righeNonCancellate),
                modificheEffettuate: grigliaMod
            };

            ajaxAgronicaSync(
                "Catasto_Edit.aspx/SalvaGrigliaMetodiProduzione",
                JSON.stringify(parametri),
                false,
                function (rispServer) {
                    console.log(rispServer);
                },
                function (rispServer) {
                    console.log(rispServer);
                }
            );
        }
        

        $('#ImgBtn_SalvaTutto').click();

    }
}

function controlla_campo_obbligatorio(nome_campo) {
    if ($('#' + nome_campo).val() == "") {
        $('#' + nome_campo).parent().append('<label id="' + nome_campo + '-error" class="custom_val error" for="' + nome_campo + '">' + TraduzioneMultiResx(catastoEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#' + nome_campo).parent().children(".required").css('border', '1px solid #D41E1A');
        return true;
    }
    return false;
}

function controlla_campo_numerico(nome_campo) {
    if ($('#' + nome_campo).val().match(/[a-z]/i)) {
        $('#' + nome_campo).parent().append('<label id="' + nome_campo + '-error" class="custom_val error" for="' + nome_campo + '">' + TraduzioneMultiResx(catastoEditResx, 'IlCampoPuòcontenereSoloNumeri', 'Il campo può contenere solo numeri') + '</label>');
        $('#' + nome_campo).parent().children(".required").css('border', '1px solid #D41E1A');
        return true;
    }
    return false;
}

function controlla_form() {

    var flag = true;
    var n_inv = 0;
    var arrErroriGriglie = [];

    v.resetForm();

    // Azzero tutte le label custom_val
    $(".custom_val").each(function (i, obj) {
        $(this).css('border', '1px solid #ccc');
        $(this).parent().children().css('border-color', '#ccc');
        $(this).remove();
    });

    $("#div_errori_salvataggio").hide();
    $("#elenco_errori_salvataggio").html("");

    // Validazione Campi

    if (controlla_campo_obbligatorio("Cmb_Provincia")) { flag = false; n_inv++; }
    if (controlla_campo_obbligatorio("Cmb_Comune")) { flag = false; n_inv++; }

    if (controlla_campo_obbligatorio("Txt_Numero")) { flag = false; n_inv++; }
    if (controlla_campo_obbligatorio("Txt_Foglio")) { flag = false; n_inv++; }
    if (controlla_campo_numerico("Txt_Numero")) { flag = false; n_inv++; }
    if (controlla_campo_numerico("Txt_Foglio")) { flag = false; n_inv++; }

    if (controlla_campo_obbligatorio("TxtSup_Ettari")) { flag = false; n_inv++; }
    if (controlla_campo_obbligatorio("TxtSup_Are")) { flag = false; n_inv++; }
    if (controlla_campo_obbligatorio("TxtSup_Centiare")) { flag = false; n_inv++; }
    if (controlla_campo_numerico("TxtSup_Ettari")) { flag = false; n_inv++; }
    if (controlla_campo_numerico("TxtSup_Are")) { flag = false; n_inv++; }
    if (controlla_campo_numerico("TxtSup_Centiare")) { flag = false; n_inv++; }

    // Controllo se è stato inserito almeno 1 possesso
    if ($('#tabPossessi .watable >tbody >tr').length == 0) {
        arrErroriGriglie.push(TraduzioneMultiResx(catastoEditResx, 'DeveEssereInseritoAlmenoUnPossesso', 'Deve essere inserito almeno un Possesso'));
        flag = false;
        n_inv++;
    }


    // Controllo se le date sono corrette
    var TxtValiditaInizio = $('#TxtValiditaInizio').val().split("/");
    var TxtValiditaFine = $('#TxtValiditaFine').val().split("/");

    ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
    fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

    if (ini > fin) {

        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">' + TraduzioneMultiResx(catastoEditResx, 'DataInizioNonPuòEssereMaggioreDiDataFine', 'La data di inizio non può essere maggiore di quella di fine') + '</label>');
        $('#TxtValiditaInizio').closest("input").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;

    }


    // Controlli di validità sulla griglia metodi produzione
    var kGrid = KendoGrid("grigliaMetodiProduzione");
    if (kGrid !== undefined && kGrid !== null) {
        var dtGrid = kGrid.dataSource.data();
        var _righeToccate = dtGrid.filter((el) => { return el.dirty == true || el.deleted == true });
        var _righeNonCancellate = dtGrid.filter((el) => { return el.deleted === undefined || el.deleted === null || el.deleted == false });

        if (_righeToccate.length > 0) {
            for (let riga of _righeNonCancellate) {
                if (riga.Validita_Inizio >= riga.Validita_Fine) {
                    flag = false;
                    n_inv++;
                    arrErroriGriglie.push(TraduzioneMultiResx(catastoEditResx,
                        "InserimentoMetodoProduzioneDataFinePrecedenteDataInizio", "Si sta cercando di inserire un metodo di produzione con data di fine antecedente a data di inizio"));
                    break;
                }
                else {
                    let righeSovrapposte = _righeNonCancellate.filter(function (el) {
                        return el.Validita_Fine >= riga.Validita_Inizio && el.Validita_Inizio <= riga.Validita_Fine
                    });

                    if (righeSovrapposte.length > 1) {
                        flag = false;
                        n_inv++;
                        arrErroriGriglie.push(TraduzioneMultiResx(catastoEditResx, "InserimentoMetodiProduzioneStessoPeriodo", "Si sta cercando di inserire più metodi di produzione nello stesso periodo"));
                        break;
                    }
                }

                
            }
        }

    }

    ///////////////////////////////


    if (v.valid() && flag) {

        //$('.nav-tabs li.active a .error-tab').remove( ".error-tab" );
        return true
    }
    else {
        if (arrErroriGriglie.length > 0) {
            for (let err of arrErroriGriglie) {
                $("#elenco_errori_salvataggio").append(
                    "<li>" + err + "</li>"
                );
            }

            $("#div_errori_salvataggio").show();
        }
        //                var err_message = "";
        //                err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red'>" + n_inv + "</div>";
        //                $('.nav-tabs li.active a').append(err_message);

        return false;
    }

}




//*** Funzione che applica alla WaTable la Footable
function applicaFooTable(tabella, elem_hide) {
    //$('#' + tabella + ' table thead tr').find('th:nth-child(3)').attr('data-hide', 'phone, tablet');
    $('#' + tabella + ' table').removeClass('phone');
    $('#' + tabella + ' table').removeClass('breakpoint');
    $('#' + tabella + ' table').removeClass('footable-loaded');
    $('#' + tabella + ' table').removeClass('footable');

    $('#' + tabella + ' table thead tr').find('th:nth-child(1)').attr('data-toggle', 'true');

    jQuery.each(elem_hide, function (i, val) {
        $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'phone, tablet');
    });


    setTimeout(function () {
        $('#' + tabella + ' table').footable({
            breakpoints: {
                phone: 480,
                tablet: 700
            }
        });
    }, 100);


}


var watablePossessi;
var watableMacrousi;
var watableZone;
var watableClassamenti;


function AggiornaTabPossessi(d) {

    AgroWA_Table_sistemaDati(d);

    var elem_hide = new Array(4, 5);

    $('#tabPossessi').html('');
    watablePossessi = $("#tabPossessi").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        checkboxes: false,
        tableCreated: function (data) { applicaFooTable('tabPossessi', elem_hide); },
        pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabPossessi", watablePossessi);
}

function AggiornaTabMacrousi(d) {

    AgroWA_Table_sistemaDati(d);

    var elem_hide = new Array(4, 5);

    $('#tabMacrousi').html('');
    watableMacrousi = $("#tabMacrousi").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        checkboxes: false,
        tableCreated: function (data) { applicaFooTable('tabMacrousi', elem_hide); },
        pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabMacrousi", watableMacrousi);
}


function AggiornaTabZone(d) {

    AgroWA_Table_sistemaDati(d);

    $('#tabZone').html('');
    watableZone = $("#tabZone").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        checkboxes: false,
        tableCreated: function (data) {
            //                                    coloraMovimenti();
        }
        , pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabZone", watableZone);
}


function AggiornaTabClassamenti(d) {

    AgroWA_Table_sistemaDati(d);


    $('#tabClassamenti').html('');
    watableClassamenti = $("#tabClassamenti").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        checkboxes: false,
        tableCreated: function (data) {
        },
        pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabClassamenti", watableClassamenti);
}


function EliminaClassamento(obj) {
    var Id_Cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Catasto_Edit.aspx/EliminaClassamento',
        data: "{Id_Cod:'" + Id_Cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true)
                AggiornaTabClassamenti(JSON.parse(r.d.RispostaStringa));
            else {
                alert(r.d.Errore);
            }
        }
    });
}


function EliminaMacrouso(obj) {
    var Id_Cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Catasto_Edit.aspx/EliminaMacrouso',
        data: "{Id_Cod:'" + Id_Cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true)
                AggiornaTabMacrousi(JSON.parse(r.d.RispostaStringa));
            else {
                alert(r.d.Errore);
            }
        }
    });
}


function EliminaPossesso(obj) {
    var Id_Cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Catasto_Edit.aspx/EliminaPossesso',
        data: "{Id_Cod:'" + Id_Cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true)
                AggiornaTabPossessi(JSON.parse(r.d.RispostaStringa));
            else {
                alert(r.d.Errore);
            }
        }
    });
}


function EliminaZona(obj) {
    var Id_Cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Catasto_Edit.aspx/EliminaZona',
        data: "{Id_Cod:'" + Id_Cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true)
                AggiornaTabZone(JSON.parse(r.d.RispostaStringa));
            else {
                alert(r.d.Errore);
            }
        }
    });
}

function changePartKey() {
    prov = $("#Cmb_Provincia option:selected").text().substring(4, 7);
    com = $("#Cmb_Comune").val().substring(3, 6);
    sez = $("#Txt_Sezione").val();
    foglio = $("#Txt_Foglio").val();
    numero = $("#Txt_Numero").val();
    subalterno = $("#Txt_Subalterno").val();

    if (foglio != "" && numero != "") {
        controllo_Catasto_Insert(prov, com, sez, foglio, numero, subalterno);
    }
}

function controllo_Catasto_Insert(prov, com, sez, foglio, numero, sub) {

    var parametri = { prov: prov, com: com, sez: sez, foglio: foglio, numero: numero, subalterno: subalterno }

    ajaxAgronica("./Catasto_Edit.aspx/controllo_Catasto_Insert",
        JSON.stringify(parametri),
        function(risposta) {
            if (risposta.RispostaStringa == "True") {
                kendo.alert(TraduzioneMultiResx(catastoEditResx,
                    "LaParticellaEsisteGiàPerLImpresa",
                    "La particella esiste già per l'impresa."));
                //abilitaSezioni(false);
                $(".btn-salva button").prop("disabled", true);
            } else {
                //abilitaSezioni(true);
                $(".btn-salva button").prop("disabled", false);
            }
        },
        null);
}

function abilitaSezioni(attDis) {
    if (attDis) {
        $("#divPossessi").show();
        $("#divSuperficie").show();
        $("#content").show();
    } else {
        $("#divPossessi").hide();
        $("#divSuperficie").hide();
        $("#content").hide();
    }
}

// Disabilito i caratteri speciali nell'input 
$('#TxtSup_Ettari').bind('keypress', function (event) {
    var regex = new RegExp("^[a-zA-Z0-9]+$");
    var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
    if (!regex.test(key)) {
        event.preventDefault();
        return false;
    }
});

function AggiornaSupCondotta() {
    try {
        var ettari = parseInt($('#TxtSup_Ettari').val());
        var are = parseInt($('#TxtSup_Are').val());
        var centiare = parseInt($('#TxtSup_Centiare').val());

        //var superficie2 = ettari + (are * 0.01) + (centiare * 0.0001)
        var superficie = (ettari + (are * 0.01) + (centiare * 0.0001)).toPrecision(ettari.toString().length + 4)

        if (!isNaN(superficie)) $('#Txt_SupCondotta').val(superficie);
    } catch(e){

    }
}

function popolaGrigliaMetodiProduzione(IDControllo) {
    var objFunzioneSubmit = {};

    var abilitaModifica = jsOperazione === enum_TipoOperazioneDB.Lettura.value ? false : true;
    // If necessario perché alla funzione creaKendoGrid non posso passare il parametro objFunzioneSubmit con la proprietà "funzione" valorizzata e le altre proprietà impostate a false
    if (abilitaModifica === true) {
        // Imposto una funzione fittizia per avere il salvataggio in funzione esterna ed i flag per avere 
        // i pulsanti di insert / cancellazione e la possibilità di modificare le righe
        objFunzioneSubmit = {
            funzione: () => { },
            flagInsert: true,
            flagUpdate: true,
            flagDelete: true
        };
    }

    var funzioniCRUD = {
        funzioneRead: gridMetodiProduzioneRead,
        UtenteAbilitatoInserimentoModifica: abilitaModifica,
        UtenteAbilitatoCancellazione: abilitaModifica,
        funzioneSubmit: objFunzioneSubmit,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };

    var idModel = "Chiave";
    var campiKendoModel = {
        Chiave: { editable: false, type: "number" },
        MetodoProduzione_Cod: { editable: false, type: "number" },
        MetodoProduzione_Des: { editable: true, type: "string" },
        Validita_Inizio: { editable: true, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: true, type: "date", defaultValue: new Date("2100/12/31") },
    };

    var styleOut = "vertical-align: middle; text-align: center;";
    var colonneKendoGrid = [
        { field: "MetodoProduzione_Des", title: TraduzioneMultiResx(catastoEditResx, "MetodoDiProduzione", "Metodo di Produzione"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, hidden: false, editor: ddlMetodiProdEditor },
        { field: "Validita_Inizio", title: TraduzioneMultiResx(catastoEditResx, "DataInizio", "Data Inizio"), headerAttributes: { style: styleOut }, hidden: false, format: "{0:dd/MM/yyyy}", editor: dpMetodiProdEditor },
        { field: "Validita_Fine", title: TraduzioneMultiResx(catastoEditResx, "DataFine", "Data Fine"), headerAttributes: { style: styleOut }, hidden: false, format: "{0:dd/MM/yyyy}", editor: dpMetodiProdEditor },
    ];

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        columnMenu: true,
        pageable: { pageSizes: [10] },
        pdf: false,
        groupable: false,
        // columnMenu: false
        // colonneCustomKendoGrid: colCustKendoGrid
    };
    var funzioniPrimaDopoEventi = {
        /* funzioneDaChiamareDopoSave: HideTabDettagli, 
         * funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti,  
         * funzioneDaChiamareDopoDelete: HideTabDettagli 
         * funzioneDaChiamareDopoDataBound: ucUmaAllevamenti_gridDataBound*/
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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
}

function gridMetodiProduzioneRead(options) {
    var ds = [];

    if (jsOperazione != enum_TipoOperazioneDB.Scrittura.value) {
        var partFoglio = 0;
        if ($("#Txt_Foglio").val() !== "") {
            partFoglio = parseInt($("#Txt_Foglio").val());
        }
        var partNumero = 0;
        if ($("#Txt_Numero").val() !== "") {
            partNumero = parseInt($("#Txt_Numero").val());
        }

        var parametri = {
            prov: $('#Cmb_Provincia option:selected').text().substring(4, 7),
            com: $('#Cmb_Comune').val().substring(3, 6),
            sez: $("#Txt_Sezione").val(),
            foglio: partFoglio,
            numero: partNumero,
            subalterno: $("#Txt_Subalterno").val()
        };

        ajaxAgronicaSync(
            "Catasto_Edit.aspx/CaricaGrigliaMetodiProduzione",
            JSON.stringify(parametri),
            false,
            function (rispServer) {
                ds = JSON.parse(rispServer.RispostaStringa);
            },
            function (rispServer) {
                kendo.alert(rispServer.RispostaStringa);
            }
        );
    }

    options.success(ds);
}

function ddlMetodiProdEditor(container, options) {
    var arrMP = [
        { MetodoProduzione_Cod: 1, MetodoProduzione_Des: TraduzioneMultiResx(catastoEditResx, 'Convenzionale', 'Convenzionale')},
        { MetodoProduzione_Cod: 2, MetodoProduzione_Des: TraduzioneMultiResx(catastoEditResx, 'InConversione', 'In Conversione')},
        { MetodoProduzione_Cod: 3, MetodoProduzione_Des: TraduzioneMultiResx(catastoEditResx, 'Biologico', 'Biologico')}
    ];
    creaDropDownEditor(container, "MetodoProduzione_Des", "MetodoProduzione_Cod", arrMP, ddlMetodiProdChange);
}

function ddlMetodiProdChange(ev) {
    var dataItem = ev.sender.dataItem();
    var gridID = ev.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.dirty = true;

    model.MetodoProduzione_Cod = dataItem.MetodoProduzione_Cod;
    model.dirtyFields.MetodoProduzione_Cod = true;

    model.MetodoProduzione_Des = dataItem.MetodoProduzione_Des;
    model.dirtyFields.MetodoProduzione_Des = true;

    kendoFastRedrawRow(grid, row);
}

function dpMetodiProdEditor(container, options) {

    $('<input required name="' + options.field + '" data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '"/>')
        .appendTo(container)
        .kendoDatePicker({
            change: function (ev) {
                if (this.value() === null) {
                    // In caso di errori
                    this.value(new Date(1900, 1, 1, 0, 0));
                }
            },
            min: new Date(1900, 1, 1, 0, 0),
            max: new Date(2100, 12, 31, 0, 0)
        });

    // tooltipElement NOTE: data-for attribute should match editor's name attribute
    $('<span class="k-invalid-msg" data-for="' + options.field + '"></span>')
        .appendTo(container);
}