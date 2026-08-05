<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master" CodeBehind="PUA_Dichiarazione_Effluenti.aspx.vb" Inherits="PianoConcimazione_2017.PUA_Dichiarazione_Effluenti" %>

<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">


    <title>PUA</title>

    <style type="text/css">
        .btn2icon {
            border: 0px;
            background-color: inherit;
            min-width: 0px !important;
            padding: 0px;
        }

        .allineadestra {
            text-align: right !important;
        }
    </style>

    <script>

        var id_HD_Effluenti = "<%= HD_Effluenti.ClientID%>";

        var id_HD_Chimici = "<%= HD_Chimici.ClientID%>";

        var Qs_PUA_Cod;
        var Qs_Regolamento_Cod;
        var Qs_Piva;
        var Qs_Sa_Cod;
        var Qs_Operazione;

        function popolaGrigliaEffluenti(IDControllo) {

            $("#" + IDControllo).html("");

            var funzioniCRUD = {
                funzioneRead: Eff_kReadValorizzazione_rows,
                funzioneInsert: Eff_kWriteValorizzazione_rows,
                funzioneUpdate: Eff_kWriteValorizzazione_rows,
                funzioneDelete: Eff_kWriteValorizzazione_rows,
                gestisciSalvataggioFinaleAParte: true
            }

            var idModel = "id";
            var campiKendoModel = Eff_kReadValorizzazione_mod();
            var colonneKendoGrid = Eff_kReadValorizzazione_col();
            var parametriPerLettura = null;
            var parametriDataSource = {
                aggregate: [{ field: "azoto_qta", aggregate: "sum" }]
            };

            var parametriKendoGrid = {
                impostaColonneKendoGridDaCookie: false,
                columnMenu: true,
                sortable: true,
                pdf: false,
                excel: false,
                groupable: false,
                pageable: false,
                filterable: true,
                btnEliminaTuttiFiltri: false,
                salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
                toolbarCommands: ["templateBtn_DisponibiltaConferimentiEsterni"],
                editable: {
                    mode: "popup",
                    window: {
                        title: "Modifica Dati Effluenti"
                    }
                },
                cancel: function () {
                    $("#" + IDControllo).data('kendoGrid').refresh();

                    let Eff_Cod = Request_QueryString("Eff_Cod");
                    if (Eff_Cod !== null) {
                    }

                },
                colonneCustomKendoGrid: [
                    {
                        command: [{
                            name: "edit",
                            text: {
                                edit: "",
                                update: "Conferma Dati",
                                cancel: "Annulla"
                            },
                            iconClass: "fa fa-pencil-square-o fa-2x",
                            className: "btn2icon"
                        },
                        {
                            name: "destroy",
                            text: "",
                            className: "k-custom-delete btn2icon",
                            iconClass: "fa fa-trash-o fa-2x"
                        }
                        ],
                        title: "Operazioni",
                        width: "90px"
                    }
                ]
            };


            colonneKendoGrid.push(
                {
                    command: [
                        {
                            name: "PeriodoDivieto", text: "Modifica Periodo Divieto", className: "PeriodoDivieto",
                            click: function (e) {
                                var datiRiga = $(e.currentTarget).closest("div.k-grid").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
                                apriDivieti(datiRiga);
                            }
                        }
                    ],
                    title: "", width: "130px"
                }
            );


            var funzioniPrimaDopoEventi = {
                funzioneDaChiamareDopoEdit: Eff_onEdit,
                funzioneDaChiamareDopoDataBound: Eff_dopoDataBound,
                funzioneDaChiamarePrimaDelSave: Eff_primaDelSave,
                funzioneDaChiamarePrimaDiEdit: function (e) {
                    if (e.model.id == 0) {
                        var grid = $('#kendo_Effluenti').data("kendoGrid");
                        var currentData = grid.dataSource.data();
                        var minid =0;

                        for (var i = 0; i < currentData.length; i++) {
                            if (currentData[i].id < minid) {
                                minid = currentData[i].id;
                            }
                        }
                        var newid = minid - 1;
                        e.model.id = newid;
                    }
                }
            };
            var mostraRigheCancellate = false;
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

        function aggiornaControlliMascheraPopup(dataItem) {

            //Rendo non modificabile l'unità di misura
            $("input[name='udm_sim']").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);

            $("input[name='strPeriodoDivieto']").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);

            

            if (dataItem.flag_matrice_prevalente === 0) {

                //Nascondo e resetto % Zootecnico
                $("div[data-container-for='perc_zootecnico']").hide();
                $("label[for='perc_zootecnico']").parent().hide();
                if (dataItem.flag_tipo_allevamento === 1) {
                    $("input[name='perc_zootecnico']").data("kendoNumericTextBox").value(100);
                } else {
                    $("input[name='perc_zootecnico']").data("kendoNumericTextBox").value(0);
                }
                $("input[name='perc_zootecnico']").data("kendoNumericTextBox").trigger("change");

            }
            else {

                //Mostro % Zootecnico
                $("div[data-container-for='perc_zootecnico']").show();
                $("label[for='perc_zootecnico']").parent().show();

            }
        }

        function Eff_onEdit(e) {

            aggiornaControlliMascheraPopup(e.model);

            var titolo = $("input[name='azoto_titoli']").data("kendoNumericTextBox");
            var N = $("input[name='azoto_qta']").data("kendoNumericTextBox");
            var carico = $("input[name='carico']").data("kendoNumericTextBox");

            titolo.bind("change", function (e) {
                var model = $("#kendo_Effluenti").data("kendoGrid").editable.options.model
                var udm_cod = model.udm_cod;
                var res = roundNumber(carico.value() * this.value(), 2);
                if (udm_cod == 19) {
                    res = res * 10;
                }
                N.value(res);
                model.set("azoto_qta", res);
            });

            N.bind("change", function (e) {
                var model = $("#kendo_Effluenti").data("kendoGrid").editable.options.model
                var udm_cod = model.udm_cod;
                var res = roundNumber(this.value() / carico.value(), 3);
                if (udm_cod == 19) {
                    res = res / 10;
                }
                titolo.value(res);
                model.set("azoto_titoli", res);
            });

            carico.bind("change", function (e) {
                var model = $("#kendo_Effluenti").data("kendoGrid").editable.options.model
                var udm_cod = model.udm_cod;
                var res = roundNumber(this.value() * titolo.value(), 2);
                if (udm_cod == 19) {
                    res = res * 10;
                }
                N.value(res);
                model.set("azoto_qta", res);
            });

        }

        function Eff_dopoDataBound(e) {
            Calcola();
        }

        function Eff_primaDelSave(e) {

            if (e.model.carico === null || e.model.carico <= 0) {
                e.preventDefault();
                kendo.alert("Inserire la quantità caricata!");
                return false;
            }

            if (e.model.azoto_qta === null || e.model.azoto_qta <= 0) {
                e.preventDefault();
                kendo.alert("Inserire la quantità di azoto!");
                return false;
            }
            else {

                //Controllo che non esista un altro elemento in griglia con stessa provenienza, effluente, titolo
                let att_eff_cod = e.model.eff_cod;
                let att_uid = e.model.uid;
                let att_azoto_titoli = e.model.azoto_titoli;
                let att_flag_provenienzaesterna = e.model.flag_provenienzaesterna;

                let data = e.sender.dataSource.data();

                for (let i = 0; i < data.length; i++) {
                    if (data[i].eff_cod === att_eff_cod && data[i].azoto_titoli === att_azoto_titoli && data[i].flag_provenienzaesterna === att_flag_provenienzaesterna && data[i].uid !== att_uid) {
                        e.preventDefault();
                        kendo.alert("Errore: l'effluente selezionato è già stato impostato");
                        break;
                    }
                }
            }
        }

        function Eff_kReadValorizzazione_rows(options) {

            var data = $('#' + id_HD_Effluenti).val();
            jSonParsed_Kendo = JSON.parse(data);

            options.success(jSonParsed_Kendo.kendo_rows);

        }

        function Eff_kWriteValorizzazione_rows(e) {
            if (e.data.models[0].perc_zootecnico < 0 || e.data.models[0].perc_zootecnico > 100) {
                kendo.alert("% Zootecnico deve essere compreso tra 0 e 100");
                return false;
            }

            e.success(e.data.models);

        }


        function Eff_kReadValorizzazione_mod() {

            var data = $('#' + id_HD_Effluenti).val();
            jSonParsed_Kendo = JSON.parse(data);


            return jSonParsed_Kendo.kendo_model;
        }

        function Eff_kReadValorizzazione_col() {

            var data = $('#' + id_HD_Effluenti).val();
            jSonParsed_Kendo = JSON.parse(data);

            kendo_Colonne_estendi(jSonParsed_Kendo, "flag_provenienzaesterna", "Provenienza Esterna", 0, "flag_provenienzaesterna_des", flag_provenienzaesterna_Template, null, null, flag_provenienzaesterna_filterable);
            kendo_Colonne_estendi(jSonParsed_Kendo, "tipo_eff_cod", "Tipo Effluente", 1, "tipo_eff_des", tipo_eff_Template, null, null, tipo_eff_filterable);
            kendo_Colonne_estendi(jSonParsed_Kendo, "eff_cod", "Effluente", 3, "eff_des", eff_Template, null, null, eff_filterable);

            return jSonParsed_Kendo.kendo_columns;
        }






        function apriDivieti(datiRiga) {

            var strrigheKendoGrid = '';
            if (datiRiga.jSonDivieti !== '') {
                strrigheKendoGrid = JSON.parse(datiRiga.jSonDivieti); 
            };

            var eff_cod = datiRiga.id;

            popolaGrigliaDivieti(strrigheKendoGrid, 'kendo_Divieti', eff_cod);

            $('#dialogDivieti').kendoDialog({
                title: "Periodo Divieto",
                modal: true,
                resizable: true,
                closable:false,
                width: "50%",
                height: "50%",
                actions: [
                    {
                        text: 'Conferma', primary: true,
                        action: function (e) {

                            var grid = $('#kendo_Effluenti').data("kendoGrid");
                            var currentData = grid.dataSource.data();

                            var gridD = $('#kendo_Divieti').data("kendoGrid");
                            var currentDataD = gridD.dataSource.data();

                            for (var i = 0; i < currentData.length; i++) {
                                if (currentData[i].id == eff_cod) {
                                    currentData[i].jSonDivieti = JSON.stringify(currentDataD);
                                    var strDivieti = '';
                                    
                                    for (var j = 0; j < currentDataD.length; j++) {
                                        var Inizio_Divieto = '...';
                                        var Fine_Divieto = '...';
                                        if (currentDataD[j].data_divieto_da !== null) {                                   
                                            if (currentDataD[j].data_divieto_da.toDateString() !== AGRODATAINIZIO) {
                                                var data_inizio = formattedReverseDate(currentDataD[j].data_divieto_da);
                                                Inizio_Divieto = data_inizio.substring(6, 10) + "/" + data_inizio.substring(4, 6) + "/" + data_inizio.substring(0, 4);
                                            }
                                        }
                                        if (currentDataD[j].data_divieto_a !== null) {                                 
                                            if (currentDataD[j].data_divieto_a.toDateString() !== AGRODATAFINE) {
                                                var data_fine = formattedReverseDate(currentDataD[j].data_divieto_a);
                                                Fine_Divieto = data_fine.substring(6, 10) + "/" + data_fine.substring(4, 6) + "/" + data_fine.substring(0, 4);
                                            }
                                        }
                                        strDivieti += Inizio_Divieto + '-' + Fine_Divieto + '\n';
                                    } 
                                    currentData[i].strPeriodoDivieto = strDivieti;
                                    kendo_imposta_valore(currentData[i], "strPeriodoDivieto", strDivieti);
                                   
                                }
                            }
                            grid.refresh();
                        }

                    },
                    { text: 'Annulla' }
                ],
                close: function () {
                }
            }).data("kendoDialog").open();
          
        }

 

        var AGRODATAINIZIO = new Date(1900, 0, 1, 0, 0, 0, 0).toDateString();
        var AGRODATAFINE = new Date(2100, 11, 31, 0, 0, 0, 0).toDateString();

        function Div_kWriteValorizzazione_rows(e) {

            var row = $(this).parents("tr");
            var grid = $('#kendo_Effluenti').data("kendoGrid");
            var dataItem = grid.dataItem(row);

            if (e.data.models[0].data_divieto_da !== null && e.data.models[0].data_divieto_a !== null) {

                if (e.data.models[0].data_divieto_da > e.data.models[0].data_divieto_a) {
                    kendo.alert("La data di inizio divieto non può essere successiva a quella di fine");
                    return false;
                }
            }

            e.success(e.data.models);

        }



         function popolaGrigliaDivieti(strrigheKendo, IDControllo, eff_cod) {

             var funzioniCRUD = {
                funzioneRead: function (options) {
                    options.success(strrigheKendo);
                },
                funzioneInsert: Div_kWriteValorizzazione_rows,
                funzioneUpdate: Div_kWriteValorizzazione_rows,
                funzioneDelete: Div_kWriteValorizzazione_rows,
                gestisciSalvataggioFinaleAParte: true
            };
            var idModel = "id";
            var campiKendoModel = {
                id_pua_effluente: { editable: true, type: "number" },
                id: { editable: true, type: "number" },
                data_divieto_da: { editable: true, type: "date" },
                data_divieto_a: { editable: true, type: "date" }
            };
            var colonneKendoGrid = [
                { field: "data_divieto_da", title: "Data Inizio Divieto", format: "{0:dd/MM/yyyy}" },
                { field: "data_divieto_a", title: "Data Fine Divieto", format: "{0:dd/MM/yyyy}" }
            ];
            var parametriPerLettura = null;
            var parametriDataSource = {};
            var parametriKendoGrid = {
                impostaColonneKendoGridDaCookie: false,
                columnMenu: false,
                sortable: false,
                pdf: false,
                excel: false,
                groupable: false,
                pageable: false,
                filterable: false,
                btnEliminaTuttiFiltri: false,
                editable: {
                    mode: "popup",
                    window: {
                        title: "Modifica Periodo Divieto"
                    }
                },
                cancel: function () {
                },
                colonneCustomKendoGrid: [
                    {
                        command: [{
                            name: "edit",
                            text: {
                                edit: "",
                                update: "Conferma Dati",
                                cancel: "Annulla"
                            },
                            iconClass: "fa fa-pencil-square-o fa-2x",
                            className: "btn2icon"
                        },
                        {
                            name: "destroy",
                            text: "",
                            className: "k-custom-delete btn2icon",
                            iconClass: "fa fa-trash-o fa-2x"
                        }
                        ],
                        title: "Operazioni",
                        width: "90px"
                    }
                ]
            };
            var funzioniPrimaDopoEventi = {
                funzioneDaChiamarePrimaDiEdit: function (e) {
                    if (e.model.id_pua_effluente == 0) {
                        e.model.id_pua_effluente = eff_cod;
                    }
                }
                //funzioneDaChiamareDopoDataBound: Div_dopoDataBound,
                //funzioneDaChiamarePrimaDelSave: Div_primaDelSave
            };
            var mostraRigheCancellate = false;
            var colonneDisabilitateSoloInModifica = null;

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



        var jSonParsed_Kendo_Effluenti;
        var jSonParsed_Kendo_PianiDistribuzione;
        var window_Distribuzione;
        var current_PC;


        function kendo_Effluenti_Leggi() {

            var regolamento_cod = parseFloat($('#<%=ddlRegolamento.ClientID %> option:selected').val());

            ajaxAgronicaSync("PUA_Dichiarazione_Effluenti.aspx/LeggiEffluenti", JSON.stringify({ pua_cod: Qs_PUA_Cod, regolamento_cod: regolamento_cod }), false,
                function (risposta) {
                    if (risposta.RispostaOK) {
                        $("#" + id_HD_Effluenti).val(risposta.RispostaStringa);
                    }
                    else {
                        alert(risposta.Errore);
                    }
                }, null);
        }

        function flag_provenienzaesterna_Template(container, options) {

            var testo = "";
            if (options.model[options.field] === 1)
                testo = ' checked="checked" ';
            $('<input type="checkbox" class="k-checkbox" id="ckb_flag_provenienzaesterna"' + testo + '/><label class="k-checkbox-label" for="ckb_flag_provenienzaesterna"></label>').appendTo(container);

            $('#ckb_flag_provenienzaesterna').change(function () {
                var model = $("#kendo_Effluenti").data("kendoGrid").editable.options.model;
                if ($(this).is(":checked")) {
                    model.set("flag_provenienzaesterna", 1);
                    model.set("flag_provenienzaesterna_des", "Sì");
                }
                else {
                    model.set("flag_provenienzaesterna", 0);
                    model.set("flag_provenienzaesterna_des", "No");
                }
            });
        }

        elencoProvenienze = [
            { "flag_provenienzaesterna": 0, "flag_provenienzaesterna_des": "No" },
            { "flag_provenienzaesterna": 1, "flag_provenienzaesterna_des": "Sì" }
        ];

        flag_provenienzaesterna_filterable = {
            extra: false,
            operators: {
                number: {
                    eq: "Uguale a"
                }
            },
            ui: function (element) {
                element.kendoDropDownList({
                    autoWidth: true,
                    filter: "contains",
                    dataSource: elencoProvenienze,
                    dataTextField: "flag_provenienzaesterna_des",
                    dataValueField: "flag_provenienzaesterna",
                    optionLabel: " "
                });
            }
        };


        var eff_di_un_tipo_eff;
        var all_di_un_tipo_eff;
        var eff_di_tutti_tipo_eff;

        function tipo_eff_Template(container, options) {

            $('<input id ="cmbtipo_eff" data-text-field="tipo_eff_des" data-value-field="tipo_eff_cod" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    dataTextField: "tipo_eff_des",
                    dataValueField: "tipo_eff_cod",
                    open: kendoDropDownAdjustWidth,
                    dataBound: kendoDropDownAdjustWidth,
                    autoWidth: true,
                    autoBind: true,
                    dataSource: { transport: { read: RiempiDdlTipoEff } },
                    change: function (e) {

                        //leggo la chiave ed aggiorno le diverse chiavi del model...        
                        var dataItem = e.sender.dataItem();

                        // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato            
                        model = $("#kendo_Effluenti").data("kendoGrid").editable.options.model

                        model.set("tipo_eff_cod", dataItem.tipo_eff_cod);
                        model.set("tipo_eff_des", dataItem.tipo_eff_des);

                        RiempiDdlEff(dataItem.tipo_eff_cod);

                        var ddl = $("#cmbeff").data("kendoDropDownList");
                        ddl.dataSource.data(eff_di_un_tipo_eff);
                        if (eff_di_un_tipo_eff.length > 0) {
                            ddl.select(0);
                            ddl.trigger("change");
                        }

                        aggiornaControlliMascheraPopup(dataItem);
                    }

                });
        }


       tipo_eff_filterable = {
            extra: false,
            operators: {
                number: {
                    eq: "Uguale a"
                }
            },
            ui: function (element) {
                element.kendoDropDownList({
                    autoWidth: true,
                    filter: "contains",
                    dataSource: { transport: { read: RiempiDdlTipoEff } },
                    dataTextField: "tipo_eff_des",
                    dataValueField: "tipo_eff_cod",
                    optionLabel: " "
                });
            }
        };

        function eff_Template(container, options) {

            RiempiDdlEff(options.model.tipo_eff_cod);

            $('<input id ="cmbeff" data-text-field="eff_des" data-value-field="eff_cod" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    dataTextField: "eff_des",
                    dataValueField: "eff_cod",
                    autoWidth: true,
                    autoBind: true,
                    filter: "startswith",
                    dataSource: eff_di_un_tipo_eff,
                    open: kendoDropDownAdjustWidth,
                    dataBound: kendoDropDownAdjustWidth,
                    change: function (e) {

                        var dataItem = e.sender.dataItem();

                        // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato            
                        model = $("#kendo_Effluenti").data("kendoGrid").editable.options.model

                        model.set("eff_cod", dataItem.eff_cod);
                        model.set("eff_des", dataItem.eff_des);

                        model.set("udm_cod", dataItem.udm_cod);
                        model.set("udm_sim", dataItem.udm_sim);

                        model.set("azoto_titoli", dataItem.n);
                        $("input[name='azoto_titoli']").data("kendoNumericTextBox").trigger("change");
                    }

                });

        }

        eff_filterable = {
            extra: false,
            operators: {
                number: {
                    eq: "Uguale a"
                }
            },
            ui: function (element) {
                element.kendoDropDownList({
                    autoWidth: true,
                    filter: "contains",
                    dataSource: eff_di_tutti_tipo_eff,
                    dataTextField: "eff_des",
                    dataValueField: "eff_cod",
                    optionLabel: " "
                });
            }
        };

        function RiempiDdlTipoEff(options) {

            var regolamento_cod = parseFloat($('#<%=ddlRegolamento.ClientID %> option:selected').val());

            var params = {
                regolamento_cod: regolamento_cod
            }
            ajaxAgronica("PUA_Dichiarazione_Effluenti.aspx/LeggiTipoEff",
                JSON.stringify(params),
                function (risposta) {
                    var p = JSON.parse(risposta.RispostaStringa);
                    options.success(p);
                }, null);



        }

        function RiempiDdlEff(tipo_eff_cod) {

            var regolamento_cod = parseFloat($('#<%=ddlRegolamento.ClientID %> option:selected').val());

            var params = {
                regolamento_cod: regolamento_cod,
                tipo_eff_cod: tipo_eff_cod
            }
            ajaxAgronicaSync("PUA_Dichiarazione_Effluenti.aspx/LeggiEff",
                JSON.stringify(params), false,
                function (risposta) {
                    eff_di_un_tipo_eff = JSON.parse(risposta.RispostaStringa);
                }, null);

        }

        function RiempiDdlTuttiEff(tipo_eff_cod) {

            var regolamento_cod = parseFloat($('#<%=ddlRegolamento.ClientID %> option:selected').val());

            var params = {
                regolamento_cod: regolamento_cod,
                tipo_eff_cod: tipo_eff_cod
            }
            ajaxAgronicaSync("PUA_Dichiarazione_Effluenti.aspx/LeggiEff",
                JSON.stringify(params), false,
                function (risposta) {
                    eff_di_tutti_tipo_eff = JSON.parse(risposta.RispostaStringa);
                }, null);

        }

        // GRIGLIA CHIMICI

        function popolaGrigliaChimici(IDControllo) {

            var funzioniCRUD = { funzioneRead: Chim_kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
            var idModel = "pro_cod";
            var campiKendoModel = Chim_kReadValorizzazione_mod();
            var colonneKendoGrid = Chim_kReadValorizzazione_col();
            var parametriPerLettura = null;
            var parametriDataSource = {
                aggregate: [{ field: "n_tot", aggregate: "sum" }]
            };
            var parametriKendoGrid = {
                impostaColonneKendoGridDaCookie: false,
                columnMenu: true,
                sortable: true,
                pdf: false,
                excel: false,
                groupable: false,
                pageable: false,
                filterable: true,
                btnEliminaTuttiFiltri: false,
                salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
                editable: false
            };
            var funzioniPrimaDopoEventi = {};
            var mostraRigheCancellate = false;
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

        function Chim_kReadValorizzazione_rows(options) {

            var data = $('#' + id_HD_Chimici).val();
            jSonParsed_Kendo = JSON.parse(data);
            options.success(jSonParsed_Kendo.kendo_rows);

        }

        function Chim_kReadValorizzazione_col() {

            var data = $('#' + id_HD_Chimici).val();
            jSonParsed_Kendo = JSON.parse(data);
            return jSonParsed_Kendo.kendo_columns;
        }

        function Chim_kReadValorizzazione_mod() {

            var data = $('#' + id_HD_Chimici).val();
            jSonParsed_Kendo = JSON.parse(data);
            return jSonParsed_Kendo.kendo_model;

        }

        function VisualizzaChimici() {
            kendo_Chimici_Leggi();
            popolaGrigliaChimici('kendo_Chimici');
            $('#kendo_Chimici .k-footer-template').addClass("allineadestra");
            $('#DivChimici').show();
            Calcola();
        }


        function kendo_Chimici_Leggi() {

            var data = $('#<%=Txt_ValiditaInizio.ClientID %>').val();

            ajaxAgronicaSync("PUA_Dichiarazione_Effluenti.aspx/LeggiChimici", JSON.stringify({ piva: Qs_Piva, data: data }), false,
                function (risposta) {
                    if (risposta.RispostaOK) {
                        $("#" + id_HD_Chimici).val(risposta.RispostaStringa);
                    }
                    else {
                        alert(risposta.Errore);
                    }
                }, null);
        }


        // CALCOLO SUP NECESSARIE

        function Calcola() {

            var grid = $("#kendo_Effluenti").data("kendoGrid");
            var data = grid.dataSource.data();

            var gridC = $("#kendo_Chimici").data("kendoGrid");
            if (gridC !== undefined) {
                var dataC = gridC.dataSource.data();
            }

            var TotN = 0;
            var Ha_ZVN = 0;
            var Ha_Ord = 0;

            for (var i = 0; i < data.length; i++) {
                TotN += data[i].azoto_qta;
            }

            if (gridC !== undefined) {
                for (var i = 0; i < dataC.length; i++) {
                    TotN += dataC[i].n_tot;
                }
            }

            if (TotN > 0) {
                Ha_ZVN = TotN / 170;
                Ha_Ord = TotN / 340;
            }

            $('#<%=Txt_Azoto.ClientID %>').val(kendo.format('{0:n2}', roundNumber(TotN, 4)));
            $('#<%=Txt_ZVN.ClientID %>').val(kendo.format('{0:n4}', roundNumber(Ha_ZVN, 4)));
            $('#<%=Txt_Ordinaria.ClientID %>').val(kendo.format('{0:n4}', roundNumber(Ha_Ord, 4)));
        }




        function Salva(tipoUscita) {

            var grid = $("#kendo_Effluenti").data("kendoGrid");
            var data = grid.dataSource.data();

            var regolamento_cod = parseFloat($('#<%=ddlRegolamento.ClientID %> option:selected').val());
            //var regolamento_cod = parseInt($("#ddlRegolamenti").data("kendoDropDownList").value());
            var sa_cod = parseFloat($('#<%=ddlCentriAziendali.ClientID %> option:selected').val());
            var tipo = parseFloat($('#<%=ddlMetodo.ClientID %> option:selected').val());
            var flag_nonutilizzo_fertilizzanti = $('#<%=Chk_NonUtilizzo_Fertilizzanti.ClientID %>').is(':checked');

            var params = {
                piva: Qs_Piva,
                sa_cod: sa_cod,
                pua_cod: parseInt(Qs_PUA_Cod),
                regolamento_cod: regolamento_cod,
                pua_tipo: tipo,
                data_da: $('#<%=Txt_ValiditaInizio.ClientID %>').val(),
                data_a: $('#<%=Txt_ValiditaFine.ClientID %>').val(),
                anno: parseInt($('#<%=Txt_Anno.ClientID %>').val()),
                note: $('#<%=Txt_Note.ClientID %>').val(),
                flag_nonutilizzo_fertilizzanti: flag_nonutilizzo_fertilizzanti,
                dati: JSON.stringify(data),
            }

            ajaxAgronica("PUA_Dichiarazione_Effluenti.aspx/Salva",
                JSON.stringify(params),
                function (risposta) {
                    kendo.alert(risposta.RispostaStringa);
                    let puacod = risposta.ParametroDue_stringa;

                    if (tipoUscita === 1)//Esci
                        esci();
                    else if (tipoUscita === 2)//Piano Distrib
                        apriPianoDistr(puacod);

                }, null);
        }

        $(document).ready(function () {

            docReady();

        });

        async function docReady() {

            Qs_PUA_Cod = $('#<%=hd_PUA_Cod.ClientId %>').val();
            Qs_Regolamento_Cod = $('#<%=hd_Regolamento_Cod.ClientId %>').val();
            Qs_Piva = $('#<%=hd_Piva.ClientId %>').val();
            Qs_Sa_Cod = $('#<%=hd_Sa_Cod.ClientId %>').val();
            Qs_Operazione = $('#<%=hd_Operazione.ClientId %>').val();

            let resp = await SportelloPUA();

            if (!resp.Sportello_Aperto) {
                $("#MainContent_Div_BTNSalva").hide();
                if (Qs_Operazione == "1") {
                    kendo.alert("Non è possibile creare il PUA. Sportello chiuso.")
                }
            }

            //$('.DatePicker').kendoDatePicker();
            $('#<%=Txt_ValiditaInizio.ClientID %>').kendoDatePicker({
                min: resp.Validita_Inizio,
                max: resp.Validita_Fine
            });

            $('#<%=Txt_ValiditaInizio.ClientID %>').kendoDateInput({
                min: resp.Validita_Inizio,
                max: resp.Validita_Fine
            });

            $('#<%=Txt_ValiditaFine.ClientID %>').kendoDatePicker({
                min: resp.Validita_Inizio,
                max: resp.Validita_Fine
            });

            $('#<%=Txt_ValiditaFine.ClientID %>').kendoDateInput({
                min: resp.Validita_Inizio,
                max: resp.Validita_Fine
            });

            //ddlRegolamenti_Load(false, 0);
            //ddlCentri_Load(false);            

            kendo_Effluenti_Leggi();
            popolaGrigliaEffluenti('kendo_Effluenti');
            $('#kendo_Effluenti .k-footer-template').addClass("allineadestra");

            RiempiDdlTuttiEff(0);

        }

        function SportelloPUA() {
            return new Promise((resolve, reject) => {
                var parametri = kendo.stringify({
                    Piva: Qs_Piva
                });

                ajaxAgronica("PUA_Dichiarazione_Effluenti.aspx/SportelloPUA",
                    parametri,
                    function (risposta) {
                        resolve(JSON.parse(risposta.RispostaStringa));
                    }, null, null, false);
            });
        }

        function apriPianoDistr(puacod) {

            var regolamento_cod = parseFloat($('#<%=ddlRegolamento.ClientID %> option:selected').val());
            //var regolamento_cod = parseInt($("#ddlRegolamenti").data("kendoDropDownList").value());

            var params = {
                objP_server: objP_server,
                objP_utenti: objP_utenti,
                pua_cod: parseInt(puacod),
                regolamento_cod: regolamento_cod,
                piva: Qs_Piva
            }

            ajaxAgronica(pathCoreWS + "Contab/PUA_Testata.asmx/VaiAlPianoDistribuzione",
                JSON.stringify(params),
                function (risposta) {
                    WaitFrame.show();
                    window.location = risposta.RispostaStringa;
                }, null);

        }

        function esci() {
            Azione_Indietro();
        }

        function ApriNuovoDdtRicevuto() {

            var regolamento_cod = parseFloat($('#<%=ddlRegolamento.ClientID %> option:selected').val());
            //var regolamento_cod = parseInt($("#ddlRegolamenti").data("kendoDropDownList").value());

            var params = {
                piva: Qs_Piva,
                regolamento_cod: regolamento_cod,
                data: $('#<%=Txt_ValiditaInizio.ClientID %>').val()
            }

            ajaxAgronica("PUA_Dichiarazione_Effluenti.aspx/ApriNuovoDdtRicevuto",
                JSON.stringify(params),
                function (risposta) {
                    eval(risposta.RispostaStringa);
                }, null);
        }

        function BtnDisponibiltaConferimentiEsterni_Click() {

            var regolamento_cod = parseFloat($('#<%=ddlRegolamento.ClientID %> option:selected').val());

            var params = {
                piva: Qs_Piva,
                regolamento_cod: regolamento_cod,
                data_da: $('#<%=Txt_ValiditaInizio.ClientID %>').val(),
                data_a: $('#<%=Txt_ValiditaFine.ClientID %>').val()
            }

            ajaxAgronica("PUA_Dichiarazione_Effluenti.aspx/VerificaDisponibiltaConferimentiEsterni",
                JSON.stringify(params),
                function (risposta) {

                    let grid = $("#kendo_Effluenti").data("kendoGrid");
                    let ds = grid.dataSource;
                    let dati = ds.data();

                    let datiDaAggMod = JSON.parse(risposta.RispostaStringa);
                    let strMsg = "Per i seguenti effluenti già presenti in griglia sono stati trovati dei DDT/Fatture. Si vogliono sostituire i precedenti valori con quelli nuovi?<br><br>";
                    let righeDaMod = [];

                    for (i = 0; i < datiDaAggMod.length; i++) {

                        let effGiaPresente = false;
                        //aggiungo sempre nuove righe perchè la chiave non è piu eff_cod
                        //for (j = 0; j < dati.length; j++) {
                        //    if (dati[j].eff_cod === datiDaAggMod[i].eff_cod) {
                        //        effGiaPresente = true;
                        //        if ((dati[j].carico === datiDaAggMod[i].carico && (dati[j].azoto_titoli === datiDaAggMod[i].azoto_titoli || datiDaAggMod[i].azoto_titoli === 0)) === false) {
                        //            righeDaMod.push(datiDaAggMod[i]);
                        //            if (datiDaAggMod[i].azoto_titoli !== 0) {
                        //                strMsg += "<b>" + dati[j].eff_des + ":</b> -> <b>Carico:</b> " + datiDaAggMod[i].carico + " " + dati[j].udm_sim + " (prec. " + dati[j].carico + " " + dati[j].udm_sim + ") <b>Titolo:</b> " + datiDaAggMod[i].azoto_titoli + " " + dati[j].udm_sim + " (prec. " + dati[j].azoto_titoli + " " + dati[j].udm_sim + ")<br>";
                        //            } else {
                        //                strMsg += "<b>" + dati[j].eff_des + ":</b> -> <b>Carico:</b> " + datiDaAggMod[i].carico + " " + dati[j].udm_sim + " (prec. " + dati[j].carico + " " + dati[j].udm_sim + ")<br>";
                        //            }
                        //        }
                        //        break;
                        //    }
                        //}

                        if (effGiaPresente === false) {

                            if (datiDaAggMod[i].id == 0) {
                                var gridTmp = $('#kendo_Effluenti').data("kendoGrid");
                                var currentData = gridTmp.dataSource.data();
                                var minid = 0;
                                for (var j = 0; j < currentData.length; j++) {
                                    if (currentData[j].id < minid) {
                                        minid = currentData[j].id;
                                    }
                                }
                                var newid = minid - 1;
                                datiDaAggMod[i].id = newid;
                            }

                            ds.add(datiDaAggMod[i]);
                        }

                    }

                    if (righeDaMod.length > 0) {
                        kendo.confirm(strMsg).then(function () {
                            for (i = 0; i < righeDaMod.length; i++) {
                                for (j = 0; j < dati.length; j++) {
                                    if (dati[j].eff_cod === righeDaMod[i].eff_cod) {
                                        dati[j].carico = righeDaMod[i].carico;
                                        if (righeDaMod[i].azoto_titoli !== 0) {
                                            dati[j].azoto_titoli = righeDaMod[i].azoto_titoli;
                                        }
                                        if (dati[j].udm_cod == 19) {
                                            dati[j].azoto_qta = dati[j].carico * 10 * dati[j].azoto_titoli;
                                        }
                                        else {
                                            dati[j].azoto_qta = dati[j].carico * dati[j].azoto_titoli;
                                        }

                                    }
                                }
                            }

                            dati.trigger("change");

                        }, function () { });
                    }




                }, null);
        }


    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField id="hd_PUA_Cod" runat="server" />
    <asp:HiddenField id="hd_Regolamento_Cod" runat="server" />
    <asp:HiddenField id="hd_Piva" runat="server" />
    <asp:HiddenField id="hd_Sa_Cod" runat="server" />
    <asp:HiddenField id="hd_Operazione" runat="server" />
    <div class="container" style="margin-bottom: 50px;">

        <!-- TESTATA -->
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h4 class="panel-title"><b></b></h4>
            </div>
            <div>
                <div class="panel-body">

                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Regolamento</span>
                                        <asp:DropDownList ID="ddlRegolamento" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Centro Aziendale</span>
                                        <asp:DropDownList ID="ddlCentriAziendali" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-xs-12 col-md-6 col-lg-3">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Metodo</span>
                                        <asp:DropDownList ID="ddlMetodo" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-md-6 col-lg-3 ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Inizio</span>
                                        <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="form-control DatePicker" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-md-6 col-lg-3">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Fine</span>
                                        <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="form-control DatePicker" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-12 col-md-6 col-lg-3">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Anno</span>
                                        <asp:TextBox ID="Txt_Anno" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-6 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Note</span>
                                        <asp:TextBox ID="Txt_Note" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Dichiarazione di Non Utilizzo Fertilizzanti</span>
                                        <asp:CheckBox ID="Chk_NonUtilizzo_Fertilizzanti" runat="server" CssClass="form-control"></asp:CheckBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-6 col-xs-12 ">
                            <div class='btn btn-info btn_100' onclick='ApriNuovoDdtRicevuto();'>Nuovo DDT Ricevuto</div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <!-- CONSISTENZE -->
        <div class="panel panel-primary displaynone">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>DATI CONSISTENZE</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 nopadding">
                            <div class="row">
                                <div id="kendo_Consistenze"></div>
                                <input type="hidden" id="HD_Consistenze" name="HD_Consistenze" runat="server" />

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- EFFLUENTI -->
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>EFFLUENTI</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 nopadding">
                            <div class="row">
                                <div id="kendo_Effluenti"></div>
                                <input type="hidden" id="HD_Effluenti" name="HD_Effluenti" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- CHIMICI -->
        <div class="panel panel-primary" id="DivChimici" style="display: none">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>FERTILIZZANTI CHIMICI</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 nopadding">
                            <div class="row">
                                <div id="kendo_Chimici"></div>
                                <input type="hidden" id="HD_Chimici" name="HD_Chimici" runat="server" />

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- TERRENO NECESSARIO -->
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h4 class="panel-title"><b>TERRENO NECESSARIO</b></h4>
            </div>
            <div>
                <div class="panel-body">

                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Azoto totale [kg]</span>
                                        <asp:TextBox ID="Txt_Azoto" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zona ZVN [ha]</span>
                                        <asp:TextBox ID="Txt_ZVN" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zona Ordinaria [ha]</span>
                                        <asp:TextBox ID="Txt_Ordinaria" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>



        <!-- BOTTONE SALVA -->
        <div class="panel panel-primary" id="Div_BTNSalva" runat="server" style="margin-bottom: 50px;">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b></b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-8 col-md-8 col-xs-12  " id="BtnSalva" runat="server">
                            <div class='btn btn-success btn_100' onclick='Salva(1);'><span class="fa fa-save"></span>SALVA ed Esci</div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-xs-12  " id="BtnSalvaEVaiAlPiano" runat="server">
                            <div class='btn btn-success btn_100' onclick='Salva(2);'><span class="fa fa-save"></span>SALVA e vai al Piano Distribuzione</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>



        <div id="dialogDivieti" >
            <div class="row">
                <div class="col-lg-12">
                    <div id="kendo_Divieti"></div>
                </div>
            </div>
            <br />
        </div>




        <input type="hidden" id="hdPiva" runat="server" />
        <input type="hidden" id="hdSaCod" runat="server" />
        <input type="hidden" id="hdPuaCod" runat="server" />
        <input type="hidden" id="hdPuaTipo" runat="server" />
        <input type="hidden" id="hdRegCod" runat="server" />
        <input type="hidden" id="hdRicettaCod" runat="server" />
        <input type="hidden" id="hdModalita" runat="server" />
        <input type="hidden" id="hdDataInizio" runat="server" />
        <input type="hidden" id="hdDataFine" runat="server" />
        <input type="hidden" id="hdBloccoFlag" runat="server" />

    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script id="templateBtn_DisponibiltaConferimentiEsterni" type="text/x-kendo-template">
        <div class="btn btn-warning" id="BtnDisponibiltaConferimentiEsterni" onclick="BtnDisponibiltaConferimentiEsterni_Click();">
            <span class="lampeggiante">Disponibiltà Conferimenti Esterni</span>
        </div>
                <div class="btn btn-info" id="BtnVisualizzaChimici" onclick="VisualizzaChimici();">
            <span class="lampeggiante">Visualizza fertilizzanti chimici disponibili</span>
        </div>
    </script>

</asp:Content>


