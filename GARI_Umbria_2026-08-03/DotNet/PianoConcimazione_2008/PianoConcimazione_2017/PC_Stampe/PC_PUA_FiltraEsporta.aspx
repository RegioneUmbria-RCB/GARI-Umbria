<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master" CodeBehind="PC_PUA_FiltraEsporta.aspx.vb" Inherits="PianoConcimazione_2017.PC_PUA_FiltraEsporta" %>


<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <title>Filtra ed esporta PUA</title>

    <style type="text/css">
        .form-control {
            font-size: 12px;
        }
    </style>

    <script>

        var id_HD_Pua = "<%= HD_Pua.ClientID%>";

        $(document).ready(function () {

            //MultiSelect Imprese
            $("#<%= ddl_Gerarchia_Impresa.clientID%>").kendoMultiSelect({
                autoClose: false
            }).data("kendoMultiSelect");

            //MultiSelect SpecieVegetali
            $("#<%= ddl_SpecieVegetali.clientID%>").kendoMultiSelect({
                autoClose: false
            }).data("kendoMultiSelect");

            $("#ctl00_MainContent_data_inizio").kendoDatePicker({
                // defines the start view
                start: "decade",
                // defines when the calendar should return date
                depth: "decade",
                // display month and year in the input
                format: "01/01/yyyy"
            });

            $("#ctl00_MainContent_data_fine").kendoDatePicker({
                // defines the start view
                start: "decade",
                // defines when the calendar should return date
                depth: "decade",
                // display month and year in the input
                format: "31/12/yyyy"
            });

        });



        //function kendo_ProgrammazioneTestata_LeggiAjax() {

            //ajaxAgronica("PC_PUA_FiltraEsporta.aspx/LeggiPUA_Testata", JSON.stringify({ imprese: tipoImpresa, dataInizio: dataInizio, dataFine: dataFine, filtroUltima: checked }),
            //    function (risposta) {
            //        if (risposta.RispostaOK) {
            //            callback(risposta);
            //        }
            //        else {
            //            alert(risposta.Errore);
            //        }
            //    }, null, null, false);

            // kendo_Pua_Leggi();

            //ajaxAgronica("PC_PUA_FiltraEsporta.aspx/LeggiPUA_Testata", JSON.stringify({ imprese: tipoImpresa, dataInizio: dataInizio, dataFine: dataFine, filtroUltima: checked }),             
            //        function (risposta) {
            //            //window.open(risposta.RispostaStringa);

            //            //La funzione mi ritorna un array di byte che faccio scaricare al volo
            //            var datiEsportati = risposta.RispostaStringa;
            //            var nomeFile = risposta.opzioniWatable.PrefissoNomeFileExport;
            //            datiEsportati = new Uint8Array(datiEsportati);
            //            var contentType = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,';
            //            var blob = new Blob([datiEsportati], { 'type': contentType });
            //            var data = new Date(Date.now());
            //            //var nomeFile = prefissoNomeFile + data.getFullYear() + '-' + (parseInt(data.getMonth()) + 1) + '-' + data.getDate() + '_' + data.getHours() + '-' + data.getMinutes() + '-' + data.getSeconds() + '-' + data.getMilliseconds() + '.xlsx';

            //            //in base al tipo di browser faccio scaricare
            //            var isIE = /*@cc_on!@*/false || !!document.documentMode;
            //            if (isIE) { //se Ã¨ IE
            //                navigator.msSaveOrOpenBlob(blob, nomeFile);
            //            }
            //            else { //Se sono gli altri...Chrome...
            //                var aLink = document.createElement('a');
            //                aLink.setAttribute("id", nomeFile);
            //                var evt = document.createEvent("HTMLEvents");
            //                evt.initEvent("click", true, false);
            //                aLink.href = window.URL.createObjectURL(blob);
            //                aLink.download = nomeFile;
            //                //aLink.dispatchEvent(evt);
            //                aLink.click();
            //            }
            //        }, null);

        //}

        function kendo_Pua_Leggi() {

            var tipoImpresa;
            var dataInizio;
            var dataFine;

            var imprese = $("#<%= ddl_Gerarchia_Impresa.clientID%>").val();
            tipoImpresa = ""
            if (imprese == undefined) {
                tipoimpresa = ""
            } else {
                var i = 0;
                for (i = 0; i < imprese.length; i++) {
                    tipoImpresa += imprese[i] + "|"
                }
            }
            dataInizio = $('#MainContent_data_inizio').val();
            if ((dataInizio == undefined) || (dataInizio == '')) {
                dataInizio = "01/01/1900"
            }
            dataFine = $('#MainContent_data_fine').val();
            if ((dataFine == undefined) || (dataFine == '')) {
                dataFine = "31/12/2100"
            }

            var checked = $('#chekUltimo').is(':checked');

            //ajaxAgronicaSync("PC_PUA_FiltraEsporta.aspx/LeggiPUA_Testata", JSON.stringify({ imprese: tipoImpresa, dataInizio: dataInizio, dataFine: dataFine, filtroUltima: checked }), false,
            //    function (risposta) {
            //        if (risposta.RispostaOK) {
            //            $("#" + id_HD_Pua).val(risposta.RispostaStringa);
            //        }
            //        else {
            //            alert(risposta.Errore);
            //        }
            //    }, null);

            var param = kendo.stringify({
                imprese: tipoImpresa,
                dataInizio: dataInizio,
                dataFine: dataFine,
                filtroUltima: checked
            });

            ajaxAgronica("PC_PUA_FiltraEsporta.aspx/LeggiPUA_Testata",
            //ajaxAgronica("PC_PUA_FiltraEsporta.aspx/LeggiPUA_Testata_OLD",               
                param,
                function (risposta) {
                    if (risposta.RispostaOK) {
                        $("#" + id_HD_Pua).val(risposta.RispostaStringa);
                        popolaGrigliaPua('kendo_Pua');
                        $('#DivPua').show();
                    }
                    else {
                        alert(risposta.Errore);
                    }
                },
                function (risposta) {
                    MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
                });

        }


        function popolaGrigliaPua(IDControllo) {

            var funzioniCRUD = { funzioneRead: ReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
            var idModel = "id";
            var campiKendoModel = ReadValorizzazione_mod();
            var colonneKendoGrid = ReadValorizzazione_col();
            var parametriPerLettura = null;
            var parametriDataSource = {
                //aggregate: [{ field: "n_tot", aggregate: "sum" }]
            };
            var parametriKendoGrid = {
                impostaColonneKendoGridDaCookie: false,
                columnMenu: true,
                sortable: true,
                pdf: false,
                excel: true,
                groupable: false,
                pageable: {
                    pageSizes: [5, 10, 20, 50, 100, "all"],
                    buttonCount: 3
                },
                filterable: { mode: "row" },
                btnEliminaTuttiFiltri: false,
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

        function ReadValorizzazione_rows(options) {

            var data = $('#' + id_HD_Pua).val();
            jSonParsed_Kendo = JSON.parse(data);
            options.success(jSonParsed_Kendo.kendo_rows);

        }

        function ReadValorizzazione_col() {

            var data = $('#' + id_HD_Pua).val();
            jSonParsed_Kendo = JSON.parse(data);
            return jSonParsed_Kendo.kendo_columns;
        }

        function ReadValorizzazione_mod() {

            var data = $('#' + id_HD_Pua).val();
            jSonParsed_Kendo = JSON.parse(data);
            return jSonParsed_Kendo.kendo_model;

        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container" style="margin-bottom: 50px;">

        <!-- filtri -->
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h4 class="panel-title"><b>FILTRI</b></h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Gerarchia Imprese</span>
                                        <asp:DropDownList ID="ddl_Gerarchia_Impresa" runat="server"
                                            CssClass="form-control" meta:resourcekey="ddl_centro_aziendaleResource1" multiple="multiple">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-xs-12 ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Inizio</span>
                                        <input type="text" class="form-control data_filtro" id="data_inizio" maxlength="10" runat="server" />

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-xs-12 ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Fine</span>
                                        <input type="text" class="form-control" id="data_fine" maxlength="10" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row w100" style="display: none">
                        <div class="col-lg-4">
                            <h4><span>Filtro Specie Vegetali</span></h4>
                        </div>

                        <div class="col-lg-4">
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" for="data_inizio">
                                    Tipo
                                </label>
                                <asp:DropDownList ID="ddl_SpecieVegetali" runat="server"
                                    CssClass="form-control" meta:resourcekey="ddl_centro_aziendaleResource1" multiple="multiple">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row w100" style="display: none">
                        <div class="col-lg-4">
                            <h4><span>Opzioni Aggiuntive</span></h4>
                        </div>

                        <div class="col-lg-4">
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" for="data_inizio">
                                    Visualizza Ultimo PUA
                                </label>
                                <input type="checkbox" class="form-control data_filtro" id="chekUltimo" clientidmode="Static" runat="server">
                            </div>

                        </div>
                    </div>


                </div>
            </div>
        </div>




        <div class="panel panel-primary" id="Div_BTNEsporta" runat="server" style="margin-bottom: 50px;">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b></b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="btn btn-success btn_100" onclick="kendo_Pua_Leggi();">
                                <i class="fa fa-search"></i>Esporta Dati
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>



        <div class="panel panel-primary" id="DivPua" style="display: none">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>Elenco Piani</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 nopadding">
                            <div class="row">
                                <div id="kendo_Pua"></div>
                                <input type="hidden" id="HD_Pua" name="HD_Pua" runat="server" />

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>








</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
</asp:Content>
