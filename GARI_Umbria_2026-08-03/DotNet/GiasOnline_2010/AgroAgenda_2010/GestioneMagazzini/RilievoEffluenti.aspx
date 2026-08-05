<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="RilievoEffluenti.aspx.vb" Inherits="AgroAgenda_2010.RilievoEffluenti" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script>


        $(document).ready(function () {

            kendo_Giacenze_Leggi();

        });

        var jSonParsed_Kendo;


        function popolaGrigliaEffluenti(IDControllo) {

            var funzioniCRUD = { funzioneRead: Eff_kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
            var idModel = "fer_cod";
            var campiKendoModel = Eff_kReadValorizzazione_mod();
            var colonneKendoGrid = Eff_kReadValorizzazione_col();
            var parametriPerLettura = null;
            var parametriDataSource = {};
            var parametriKendoGrid = {
                impostaColonneKendoGridDaCookie: false,
                columnMenu: false,
                sortable: false,
                pdf: false,
                excel: false,
                groupable: false,
                pageable: false,// { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
                btnEliminaTuttiFiltri: false,
                editable: false,
                filterable: false// { mode: "row" }//,filterable: true
            };
            var funzioniPrimaDopoEventi = {
                funzioneDaChiamareDopoDataBound: Eff_onDataBoundRighe,
                funzioneDaChiamarePrimaDelDetailInit: detailInit
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

        function Eff_kReadValorizzazione_rows(options) {
            var data = $('#' + id_HD_Effluenti).val();
            jSonParsed_Kendo = JSON.parse(data);
            options.success(jSonParsed_Kendo.kendo_rows);
        }

        function Eff_kReadValorizzazione_col() {
            var data = $('#' + id_HD_Effluenti).val();
            jSonParsed_Kendo = JSON.parse(data);
            return jSonParsed_Kendo.kendo_columns;
        }

        function Eff_kReadValorizzazione_mod() {
            var data = $('#' + id_HD_Effluenti).val();
            jSonParsed_Kendo = JSON.parse(data);
            return jSonParsed_Kendo.kendo_model;
        }

        function Eff_onDataBoundRighe(e) {

            //coloraRighe("#Kendo_Effluenti", e);

            //impostaPulsantiRighe("#Kendo_Effluenti", e);

            //if (ImpostazioniPaginaGlobali.Visualizza_Filtri === false) {
            e.sender.expandRow(e.sender.tbody.find("tr.k-master-row").first());
            //}

        }

        function detailInit(e) {

            var fer_cod_selezionato = e.data.fer_cod;
            var id_div = "GrigliaDettagli" + fer_cod_selezionato.toString();
            $("<div id='" + id_div + "' />").appendTo(e.detailCell);
            popolaGrigliaGiacenze(id_div, e.data.fer_cod);

        }


        function popolaGrigliaGiacenze(IDControllo, fer_cod) {

            var funzioniCRUD = { funzioneRead: Giacenze_kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
            var idModel = "id";
            var campiKendoModel = Giacenze_kReadValorizzazione_mod();
            var colonneKendoGrid = Giacenze_kReadValorizzazione_col();
            var parametriPerLettura = null;
            var parametriDataSource = { filter: [{ field: "fer_cod", operator: "eq", value: fer_cod }] };
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
                editable: false
            };
            var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: Dett_onDataBoundRighe };
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

        function Giacenze_kReadValorizzazione_rows(options) {

            var data = $('#' + id_HD_Giacenze).val();
            jSonParsed_Kendo = JSON.parse(data);
            options.success(jSonParsed_Kendo.kendo_rows);

        }

        function Giacenze_kReadValorizzazione_col() {

            var data = $('#' + id_HD_Giacenze).val();
            jSonParsed_Kendo = JSON.parse(data);
            return jSonParsed_Kendo.kendo_columns;
        }

        function Giacenze_kReadValorizzazione_mod() {

            var data = $('#' + id_HD_Giacenze).val();
            jSonParsed_Kendo = JSON.parse(data);
            return jSonParsed_Kendo.kendo_model;

        }

        function Dett_onDataBoundRighe(e) {

            //var gridId = e.sender.element[0].id;
            //coloraRighe("#" + gridId, e);

        }


        function kendo_Giacenze_Leggi() {

            var piva = $('#<%=hdPiva.ClientID %>').val();
            var sacod = $('#<%=hdSaCod.ClientID %>').val();
            var fabbricatocod = $('#<%=hdFabbricatoCod.ClientID %>').val();
            var data = $('#<%=hdData.ClientID %>').val();
            var puacod = $('#<%=hdPuaCod.ClientID %>').val();
            var regolamentocod = $('#<%=hdRegCod.ClientID %>').val();

            ajaxAgronicaSync("RilievoEffluenti.aspx/LeggiGiacenzeEffluenti", JSON.stringify({ piva: piva, sacod: sacod, fabbricatocod: fabbricatocod, data: data, puacod: puacod, regolamentocod: regolamentocod }), false,
                function (risposta) {
                    if (risposta.RispostaOK) {
                        $("#" + id_HD_Effluenti).val(risposta.RispostaStringa.KendoGrid);
                        $("#" + id_HD_Giacenze).val(risposta.RispostaStringa.KendoGridFiglia);
                        popolaGrigliaEffluenti("Kendo_Effluenti");
                    }
                    else {
                        alert(risposta.Errore);
                    }
                }, null);
        }


        function Salva() {

            var grid = $("#Kendo_Effluenti").data("kendoGrid");
            var data = grid.dataSource.data();

            var piva = $('#<%=hdPiva.ClientID %>').val();
            var sacod = $('#<%=hdSaCod.ClientID %>').val();
            var fabbricatocod = $('#<%=hdFabbricatoCod.ClientID %>').val();

            var strKendoEffluenti = '';
            var strKendoGiacenze = '';

            strKendoEffluenti = kendo.stringify(JSON.parse($("#" + id_HD_Effluenti).val()).kendo_rows);
            strKendoGiacenze = kendo.stringify(JSON.parse($("#" + id_HD_Giacenze).val()).kendo_rows);


            var params = {
                piva: piva,
                sacod: sacod,
                fabbricatocod: fabbricatocod,
                strKendoEffluenti: strKendoEffluenti,
                strKendoGiacenze: strKendoGiacenze,
                dati: JSON.stringify(data),
            }


            let tuttook = false;
            ajaxAgronicaSync("RilievoEffluenti.aspx/Salva",
                JSON.stringify(params), false,
                function (risposta) {
                    //let NTot = risposta.ParametroDue_stringa;
                    //parent.N_fertilizzazioniPrecedenti = kendo.parseFloat(NTot);
                    kendo.alert("Salvataggio avvenuto con successo")
                    tuttook = true;
                }, function (risposta) {
                    alert(risposta.Errore);
                });

            if (tuttook === true) {
                setTimeout(function () {
                    if ($("input[name$='hdApertodaGiasNG']").val() === "True") {
                        window.parent.postMessage("chiudiWindowGiasNG", '*');
                    } else {
                        window.close();
                        //parent.chiudiLetamazioni();
                    }
                }, 3000)
            }
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="panel panel-primary" id="DivChimici">
        <div class="panel-heading">
            <h4 class="panel-title">
                <b>GIACENZE EFFLUENTI</b>
            </h4>
        </div>
        <div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-lg-12 nopadding">
                        <div class="row">
                            <div id="Kendo_Effluenti"></div>
                            <input type="hidden" id="HD_Giacenze" name="HD_Giacenze" runat="server" />
                            <input type="hidden" id="HD_Effluenti" name="HD_Effluenti" runat="server" />
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
                    <div class="col-lg-12 col-md-12 col-xs-12  ">
                        <div class='btn btn-success btn_100' onclick='Salva();'><span class="fa fa-save"></span>SALVA CARICHI MENSILI STIMATI</div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdSaCod" runat="server" />
    <input type="hidden" id="hdFabbricatoCod" runat="server" />
    <input type="hidden" id="hdPuaCod" runat="server" />
    <input type="hidden" id="hdRegCod" runat="server" />
    <input type="hidden" id="hdData" runat="server" />
    <input type="hidden" id="hdApertodaGiasNG" runat="server" />

    <iframe id="iframe" style="display: none;"></iframe>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">

        var id_HD_Giacenze = "<%= HD_Giacenze.ClientID%>";
        var id_HD_Effluenti = "<%= HD_Effluenti.ClientID%>";


    </script>

</asp:Content>
