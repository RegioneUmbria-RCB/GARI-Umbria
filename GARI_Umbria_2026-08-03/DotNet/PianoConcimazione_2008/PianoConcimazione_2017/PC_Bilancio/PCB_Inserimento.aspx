<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master"
    CodeBehind="PCB_Inserimento.aspx.vb" Inherits="PianoConcimazione_2017.PCB_Inserimento" %>



<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Piano Concimazione a Bilancio</title>
    <style>
        .k-list-item-text {
            width: 100%
        }
    </style>
    <script type="text/javascript">

        var Qs_Piva;
        var Qs_Operazione;
        var Qs_Tipo;
        var usaAnalisiModelloNG;

        function apriFormDialog(url) {
            $("#PaginaGeneric").attr("src", url);
            $("#iFrameGeneric").modal('toggle');
        }


        function ChiusuraModale() {
            $('#iFrameGeneric').modal("hide");
        }

        function iFrame_Chiusura() {
            $("#PaginaGeneric").attr("src", "");
            //ricarico le analisi
            $("#Btn_hidden_CaricaAnalisi").click();
        }

        function SelezionaDeselezionaTutti() {

            if ($('#chkSelezionaTuttiImpianti').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaImpianto').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaImpianto').each(function () {
                    $(this).children('input').prop('checked', false);
                });
            }
        }

        function AggiornaDescrizione() {
            var Desc = $('#<%=ddlRegolamento.ClientID %> option:selected').text();
            $('#<%=Txt_Descrizione.ClientId %>').val(Desc);
            var anno = Desc.match((/\d/g));
            anno = anno.join("");
            var data_inizio = '01/01/' + anno;
            var data_fine = '31/12/' + anno;
            $('#<%=Txt_ValiditaInizio.ClientId %>').val(data_inizio);
            $('#<%=Txt_ValiditaFine.ClientId %>').val(data_fine);
            $('#<%=Txt_Anno.ClientId %>').val(anno);
        }


        function disableEnterKey(e) {
            var key;
            if (window.event)
                key = window.event.keyCode; //IE
            else
                key = e.which; //firefox 

            return (key != 13);
        }

        function IsNumeric(val) {
            if (isNaN(parseFloat(val))) {
                return false;
            }
            return true;
        }

        /****************************************************************************************************************/
        function Valore_FattoreVariazione(Oggetto) {
            return Oggetto.parent().parent().parent().children('.Variazione').html().replace(',', '.');
        }

        function Valore_FattoreCodice(Oggetto) {
            return Oggetto.parent().parent().parent().children('.Codice').html().replace(',', '.');
        }

        /********** AZOTO ***********************************************************************************************/



        function Chk_SelDecrementi_N_Click(Oggetto, tutti) {
            RicalcolaDecrementoTotaleN();
            CalcolaDoseN();
        }
        function Chk_SelIncrementi_N_Click(Oggetto, tutti) {
            RicalcolaIncrementoTotaleN();
            CalcolaDoseN();
        }

        //Funzione per l'aggiornamento della riduzione Totale
        function RicalcolaDecrementoTotaleN() {
            var Tot = 0.00;
            //sel la checkbox è chekkata
            $('.Chk_SelDecrementi_N').children('input:checked').each(function () {
                var app = Valore_FattoreVariazione($(this));
                Tot += parseFloat(app);
            });
            //InserisciIncTotaleN(SupTot);
            $('#<%=Txt_TotDecrementi.ClientId %>').val(Tot);
        }

        //Funzione per l'aggiornamento della riduzione Totale
        function RicalcolaIncrementoTotaleN() {
            var Tot = 0.00;
            //sel la checkbox è chekkata
            $('.Chk_SelIncrementi_N').children('input:checked').each(function () {
                var app = Valore_FattoreVariazione($(this));
                Tot += parseFloat(app);

                var cod = Valore_FattoreCodice($(this));
                if (cod == 46) {
                    var max = $('#<%=Txt_MaxIncrementi.ClientId %>').val();
                    max += parseFloat(app);
                    $('#<%=Txt_MaxIncrementi.ClientId %>').val(max);
                }
            });
            //InserisciIncTotaleN(SupTot);
            $('#<%=Txt_TotIncrementi.ClientId %>').val(Tot);

        }

        function InserisciIncTotaleN(valore) {
            var app = valore;
            $('#<%=Txt_TotIncrementi.ClientId %>').val(app);
        }

        function CalcolaDoseN() {

            var TotIncrementi = $('#<%=Txt_TotIncrementi.ClientId %>').val();
            if (!(IsNumeric(TotIncrementi))) {
                TotIncrementi = 0;
            }

            var MaxIncrementi = $('#<%=Txt_MaxIncrementi.ClientId %>').val();
            if (!(IsNumeric(MaxIncrementi))) {
                MaxIncrementi = 0;
            }

            var TotDecrementi = $('#<%=Txt_TotDecrementi.ClientId %>').val();
            if (!(IsNumeric(TotDecrementi))) {
                TotDecrementi = 0;
            }

            var DoseStd = $('#<%=Txt_DoseStandard.ClientId %>').val();
            //var DoseMas = $('#<%=Txt_MAS.ClientId %>').val();
            var DoseMas = parseFloat($('#<%=ddlMasS.ClientID %> option:selected').val());

            var DoseRic = 0;
            DoseRic = parseFloat(DoseStd) + parseFloat(TotIncrementi) - parseFloat(TotDecrementi);

            if (parseFloat(DoseRic) < 0) {
                DoseRic = 0;
            } else if (DoseRic > (parseFloat(DoseStd) + parseFloat(MaxIncrementi))) {
                DoseRic = parseFloat(DoseStd) + parseFloat(MaxIncrementi);
            }
            if (parseFloat(DoseMas) > 0) {
                if (parseFloat(DoseRic) > parseFloat(DoseMas)) {
                    DoseRic = parseFloat(DoseMas)
                }
            }

            //aggiorno il calcolato solo se non sono in 1 e 2 anno allevamento
            var fase = parseFloat($('#<%=ddlFaseCiclo.ClientID %> option:selected').val());
            switch (fase) {
                case 1: case 2: case 3:
                    break;
                default:
                    $('#<%=Txt_DoseRicalcolata.ClientId %>').val(DoseRic);
                    $('#<%=N_Ammesso.ClientId %>').val(DoseRic);
            }

        }


        /********** FOSFORO ***********************************************************************************************/

        function ddlDoseP_Click() {
            AggiornaDoseStandardP();
            RicalcolaIncrementoTotaleP();
            RicalcolaDecrementoTotaleP();
            CalcolaDoseP();
        }

        function Chk_SelDecrementi_P_Click(Oggetto, tutti) {
            RicalcolaDecrementoTotaleP();
            CalcolaDoseP();
        }
        function Chk_SelIncrementi_P_Click(Oggetto, tutti) {
            RicalcolaIncrementoTotaleP();
            CalcolaDoseP();
        }

        //aggiorno dose standard
        function AggiornaDoseStandardP() {

            //aggiorno la dose standard solo se non sono in 1 e 2 anno allevamento
            var fase = parseFloat($('#<%=ddlFaseCiclo.ClientID %> option:selected').val());
            switch (fase) {
                case 1: case 2: case 3:
                    break;
                default:
                    var Dose = $('#<%=ddlDoseP.ClientID %> option:selected').val().split("|")[1];
                    $('#<%=Txt_DoseStandardP.ClientId %>').val(Dose);
            }
        }


        //Funzione per l'aggiornamento della riduzione Totale
        function RicalcolaDecrementoTotaleP() {
            var Tot = 0.00;
            //sel la checkbox è chekkata
            $('.Chk_SelDecrementi_P').children('input:checked').each(function () {
                var app = Valore_FattoreVariazione($(this));
                Tot += parseFloat(app);
            });
            //InserisciIncTotaleN(SupTot);
            $('#<%=Txt_TotDecrementiP.ClientId %>').val(Tot);
        }

        //Funzione per l'aggiornamento della riduzione Totale
        function RicalcolaIncrementoTotaleP() {
            var Tot = 0.00;
            //sel la checkbox è chekkata
            $('.Chk_SelIncrementi_P').children('input:checked').each(function () {
                var app = Valore_FattoreVariazione($(this));
                Tot += parseFloat(app);
            });
            //InserisciIncTotaleN(SupTot);
            $('#<%=Txt_TotIncrementiP.ClientId %>').val(Tot);

        }

        function CalcolaDoseP() {
            var TotIncrementi = $('#<%=Txt_TotIncrementiP.ClientId %>').val();
            if (!(IsNumeric(TotIncrementi))) {
                TotIncrementi = 0;
            }

            var TotDecrementi = $('#<%=Txt_TotDecrementiP.ClientId %>').val();
            if (!(IsNumeric(TotDecrementi))) {
                TotDecrementi = 0;
            }

            var DoseStd = $('#<%=Txt_DoseStandardP.ClientId %>').val();
            var DoseRic = 0;
            DoseRic = parseFloat(DoseStd) + parseFloat(TotIncrementi) - parseFloat(TotDecrementi);

            //aggiorno il calcolato solo se non sono in 1 e 2 anno allevamento
            var fase = parseFloat($('#<%=ddlFaseCiclo.ClientID %> option:selected').val());
            switch (fase) {
                case 1: case 2: case 3:
                    break;
                default:
                    $('#<%=Txt_DoseRicalcolataP.ClientId %>').val(DoseRic);
                    $('#<%=P_Ammesso.ClientId %>').val(DoseRic);
            }



        }


        /********** POTASSIO ***********************************************************************************************/

        function ddlDoseK_Click() {
            AggiornaDoseStandardK();
            RicalcolaIncrementoTotaleK();
            RicalcolaDecrementoTotaleK();
            CalcolaDoseK();
        }

        function Chk_SelDecrementi_K_Click(Oggetto, tutti) {
            RicalcolaDecrementoTotaleK();
            CalcolaDoseK();
        }
        function Chk_SelIncrementi_K_Click(Oggetto, tutti) {
            RicalcolaIncrementoTotaleK();
            CalcolaDoseK();
        }

        //aggiorno dose standard
        function AggiornaDoseStandardK() {

            //aggiorno la dose standard solo se non sono in 1 e 2 anno allevamento
            var fase = parseFloat($('#<%=ddlFaseCiclo.ClientID %> option:selected').val());
            switch (fase) {
                case 1: case 2: case 3:
                    break;
                default:
                    var Dose = $('#<%=ddlDoseK.ClientID %> option:selected').val().split("|")[1];
                    $('#<%=Txt_DoseStandardK.ClientId %>').val(Dose);
            }

        }

        //Funzione per l'aggiornamento della riduzione Totale
        function RicalcolaDecrementoTotaleK() {
            var Tot = 0.00;
            //sel la checkbox è chekkata
            $('.Chk_SelDecrementi_K').children('input:checked').each(function () {
                var app = Valore_FattoreVariazione($(this));
                Tot += parseFloat(app);
            });
            //InserisciIncTotaleN(SupTot);
            $('#<%=Txt_TotDecrementiK.ClientId %>').val(Tot);
        }

        //Funzione per l'aggiornamento della riduzione Totale
        function RicalcolaIncrementoTotaleK() {
            var Tot = 0.00;
            //sel la checkbox è chekkata
            $('.Chk_SelIncrementi_K').children('input:checked').each(function () {
                var app = Valore_FattoreVariazione($(this));
                Tot += parseFloat(app);
            });
            //InserisciIncTotaleN(SupTot);
            $('#<%=Txt_TotIncrementiK.ClientId %>').val(Tot);

        }

        function CalcolaDoseK() {
            var TotIncrementi = $('#<%=Txt_TotIncrementiK.ClientId %>').val();
            if (!(IsNumeric(TotIncrementi))) {
                TotIncrementi = 0;
            }

            var TotDecrementi = $('#<%=Txt_TotDecrementiK.ClientId %>').val();
            if (!(IsNumeric(TotDecrementi))) {
                TotDecrementi = 0;
            }

            var DoseStd = $('#<%=Txt_DoseStandardK.ClientId %>').val();
            var DoseRic = 0;
            DoseRic = parseFloat(DoseStd) + parseFloat(TotIncrementi) - parseFloat(TotDecrementi);

            //aggiorno il calcolato solo se non sono in 1 e 2 anno allevamento
            var fase = parseFloat($('#<%=ddlFaseCiclo.ClientID %> option:selected').val());
            switch (fase) {
                case 1: case 2: case 3:
                    break;
                default:
                    $('#<%=Txt_DoseRicalcolataK.ClientId %>').val(DoseRic);
                    $('#<%=K_Ammesso.ClientId %>').val(DoseRic);
            }

        }


        function ddlP_Click() {
            var forma = parseFloat($('#<%=DDL_P2O5.ClientID %> option:selected').val());
            var txtP = parseFloat($('#<%=Txt_P.ClientId %>').val());
            switch (forma) {
                case 1:
                    $('#<%=Txt_P.ClientId %>').val(txtP * 0.436);
                    break;
                default:
                    $('#<%=Txt_P.ClientId %>').val(txtP * 2.291);
                    break;
            }
        }

        function ddlK_Click() {
            var forma = parseFloat($('#<%=DDL_K2O.ClientID %> option:selected').val());
            var txtK = parseFloat($('#<%=Txt_K.ClientId %>').val());
            switch (forma) {
                case 1:
                    $('#<%=Txt_K.ClientId %>').val(txtK * 0.83);
                    break;
                default:
                    $('#<%=Txt_K.ClientId %>').val(txtK * 1.205);
                    break;
            }
        }

        $(document).ready(function () {
            docReady();
        });


        var cmbTipoSorgente;
        var cmbOrigineDati;

        async function docReady() {

            Qs_Piva = $('#<%=hd_Piva.ClientId %>').val();
            Qs_Operazione = $('#<%=hd_Operazione.ClientId %>').val();
            Qs_Tipo = $('#<%=hd_Tipo.ClientId %>').val();
            usaAnalisiModelloNG = $('#<%=hd_usaAnalisiModelloNG.ClientID %>').val() == 'True' ? true : false;

            $('.DatePicker').kendoDatePicker();

            $('#<%=ddlRegolamento.ClientID %>').change(function () {
                AggiornaDescrizione();
            });

            $('#chkSelezionaTuttiImpianti').click(function () {
                SelezionaDeselezionaTutti();
            });

            $('#<%=ddlDoseP.ClientID %>').change(function () {
                ddlDoseP_Click();
            });

            $('#<%=ddlDoseK.ClientID %>').change(function () {
                ddlDoseK_Click();
            });

            $('.Chk_SelDecrementi_N').click(function () {
                Chk_SelDecrementi_N_Click($(this).find('input'), false);
            });

            $('.Chk_SelIncrementi_N').click(function () {
                Chk_SelIncrementi_N_Click($(this).find('input'), false);
            });

            $('.Chk_SelDecrementi_P').click(function () {
                Chk_SelDecrementi_P_Click($(this).find('input'), false);
            });

            $('.Chk_SelIncrementi_P').click(function () {
                Chk_SelIncrementi_P_Click($(this).find('input'), false);
            });

            $('.Chk_SelDecrementi_K').click(function () {
                Chk_SelDecrementi_K_Click($(this).find('input'), false);
            });

            $('.Chk_SelIncrementi_K').click(function () {
                Chk_SelIncrementi_K_Click($(this).find('input'), false);
            });

            $('#<%=DDL_P2O5.ClientID %>').change(function () {
                ddlP_Click();
            });

            $('#<%=DDL_K2O.ClientID %>').change(function () {
                ddlK_Click();
            });

            $('#<%=ddlMasS.ClientID %>').change(function () {
                CalcolaDoseN();
            });

            $('#<%=ddlMasB.ClientID %>').change(function () {
                $('#<%=Btn_Bilancio.ClientID %>').click();
            });

            $('#iFrameGeneric').on('hidden.bs.modal', function () {
                iFrame_Chiusura();

            });

            let resp = await SportelloPCB();

            if (!resp.Sportello_Aperto) {
                $("#MainContent_Div_BTNSalva").hide();
                if (Qs_Operazione == "1") {
                    kendo.alert("Non è possibile creare il Piano di Concimazione. Sportello chiuso.");
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



            $('#<%=Meteo_ChkAgenda.ClientId %>').val(0);
<%--            $('#<%=Meteo_TipoSorgente.ClientId %>').val(0);
            $('#<%=Meteo_Sorgente.ClientId %>').val(0);--%>

            cmbTipoSorgente = $("#cmbTipoSorgente").kendoDropDownList({
                autoBind: true,
                autoWidth: true,
                filter: null,
                dataTextField: "sorgente_des",
                dataValueField: "sorgente_cod",
                dataSource: RicaricaTipoSorgente(),
                optionLabel: "Seleziona...",
                change: function (e) {

                    $('#<%=Meteo_TipoSorgente.ClientId %>').val(this.value());
                    $('#<%=Meteo_TipoSorgente_Real.ClientId %>').val(this.value());

                    //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)
                    RicaricaOrigineDati();
                }
            }).data("kendoDropDownList");

            let versioneKendo = kendo.version.split('.')[0];
            var template = "";
            if (versioneKendo <= 2021) {
                template += "<div class='k-state-default'>";
                template += "   <span style='float:left;'>";
                template += "       #: data.nome_stazione #";
                template += "   </span>";
                template += "   <span style='float:right;'>";
                template += "       #: data.distanza #";
                template += "   </span>";
                template += "</div>";
            } else {

                let tmplt_r2 = "<span style='flex-grow:1;'>" +
                    "               #: data.nome_stazione #" +
                    "           </span>" +
                    "           <span>" +
                    "               #: data.distanza #" +
                    "           </span>";
                template = "<div style='display:inline-block; width:100%;'>" +
                    "           <div style='display:flex; font-size:12px; padding-bottom:3px;'>" +
                    tmplt_r2 +
                    "           </div>" +
                    "       </div>"
                //"           <div style='display:flex; font-size:11px; padding-bottom:3px;'>" + tmplt_r2 +
                //"           </div>";
            }
            cmbOrigineDati = $("#cmbOrigineDati").kendoDropDownList({
                autoBind: true,
                autoWidth: true,
                dataTextField: "nome_stazione",
                dataValueField: "id_stazione",
                filter: "contains",
                optionLabel: "Seleziona...",
                template: template,
                //valueTemplate: "#: data.sorgente #",
                change: function (e) {

                    $('#<%=Meteo_TipoSorgente_Real.ClientId %>').val(e.sender.dataItem().tipo_sorgente);
                    $('#<%=Meteo_Sorgente.ClientId %>').val(this.value());
                }
            }).data("kendoDropDownList");

            //$.each($(".calc-width"), function () {
            //    $(this).children(".input-group-addon").css("width", $(this).children(".input-group-addon").outerWidth());
            //    $(this).css({ "table-layout": "fixed", "width": "100%" });
            //});

            //setto il default 
            if ($('#<%=Meteo_TipoSorgente.ClientId %>').val() !== '0') {
                cmbTipoSorgente.value($('#<%=Meteo_TipoSorgente.ClientId %>').val());
                cmbTipoSorgente.trigger("change");
            }
            if ($('#<%=Meteo_Sorgente.ClientId %>').val() !== '0') {
                cmbOrigineDati.value($('#<%=Meteo_Sorgente.ClientId %>').val());
            }

<%--                    if (datiPassaggio.TipoSorgente !== undefined) {
                        cmbTipoSorgente.value($('#<%=Meteo_TipoSorgente.ClientId %>').val());
                        cmbTipoSorgente.trigger("change");
                    }

                    if (datiPassaggio.Sorgente !== undefined) {
                        cmbOrigineDati.value(datiPassaggio.Sorgente)
                    }--%>


            let viewModel = kendo.observable({
                checkboxChecked: false,
                clickHandler: function (e) {
                    if (e.data.checkboxChecked) {
                        $('#<%=Meteo_ChkAgenda.ClientId %>').val(1);
                        cmbTipoSorgente.enable(false);
                        cmbOrigineDati.enable(false);
                    } else {
                        $('#<%=Meteo_ChkAgenda.ClientId %>').val(0);
                        cmbTipoSorgente.enable(true);
                        cmbOrigineDati.enable(true);
                    }
                }
            });
            kendo.bind($("#opRegGiasChk"), viewModel);

            if (usaAnalisiModelloNG) {
                window.addEventListener('message', event => {
                    if (verificaOriginSecondaria(window, window.origin, event) &&
                        (typeof event.data != null) && (event.data.messaggio != null) &&
                        event.data.messaggio.includes("chiudiWindowGiasNG")) {
                        ChiusuraModale();
                    }
                });
            }

        }

        function RicaricaTipoSorgente() {
            let sorgenti = [];

            ajaxAgronicaSync("../MeteoWS.aspx/RicaricaSorgenteDati",
                JSON.stringify({}),
                false,
                function (risposta) {
                    sorgenti = JSON.parse(risposta.RispostaStringa);
                },
                function (risposta) {
                }
            );
            return sorgenti;
        }

        function RicaricaOrigineDati() {

            let SaCod = $('#<%=Cmb_Centro.ClientID %> option:selected').val();

            let tipoSorgente = cmbTipoSorgente.value();

            if (tipoSorgente == "") {
                cmbOrigineDati.setDataSource([]);
                cmbOrigineDati.refresh();
                return;
            }

            let param = "{ PIVA: '" + Qs_Piva + "', SaCod: " + SaCod + ", TipoSorgenteDati: " + tipoSorgente + " }";

            ajaxAgronicaSync("../MeteoWS.aspx/RicaricaOrigineDati",
                param,
                true,
                function (risposta) {
                    OrigineDati = JSON.parse(risposta.RispostaStringa);
                    cmbOrigineDati.setDataSource(OrigineDati);
                    cmbOrigineDati.refresh();
                    cmbOrigineDati.select(-1);
                },
                null
            );
        }


        function SportelloPCB() {
            return new Promise((resolve, reject) => {
                var parametri = kendo.stringify({
                    Piva: Qs_Piva
                });

                ajaxAgronica("PCB_Inserimento.aspx/SportelloPCB",
                    parametri,
                    function (risposta) {
                        resolve(JSON.parse(risposta.RispostaStringa));
                    }, null, null, false);
            });
        }

        function CaricaPiogge() {
            let tipoSorgente = $('#<%=Meteo_TipoSorgente_Real.ClientId %>').val();
            if (tipoSorgente == "" && $('#<%=Meteo_ChkAgenda.ClientId %>').val() == '1')
                tipoSorgente = "0" //Il WS si aspetta un intero, con stringa vuota si schianta...

            let param = {
                PIVA: Qs_Piva,
                SaCod: $('#<%=Cmb_Centro.ClientID %> option:selected').val(),
                Anno: $('#<%=Txt_Anno.ClientID %>').val(),
                leggiDaAgenda: $('#<%=Meteo_ChkAgenda.ClientId %>').val(),
                tipoSorgente: tipoSorgente,
                sorgente: $('#<%=Meteo_Sorgente.ClientId %>').val(),
                regolamento: $('#<%=ddlRegolamento.ClientID %> option:selected').val()
            };

            ajaxAgronicaSync("PCB_Inserimento.aspx/Carica_Piogge_WM",
                JSON.stringify(param),
                true,
                function (risposta) {
                    let values = JSON.parse(risposta.RispostaStringa);

                    $('#<%=Txt_Pioggia.ClientId %>').val(values.Pioggia);
                    $('#<%=Txt_Pioggia_Febbraio.ClientId %>').val(values.Pioggia_Febbraio);
                },
                null
            );

            return false;
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hd_Piva" runat="server" />
    <asp:HiddenField ID="hd_Operazione" runat="server" />
    <asp:HiddenField ID="hd_Tipo" runat="server" />
    <asp:HiddenField ID="hd_usaAnalisiModelloNG" runat="server" />
    <div class="container">
        <!-- TESTATA -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-testata gias-mt-1">
            <div class="panel-heading gias-section-title">
                <h4 class="panel-title"></h4>
            </div>
            <div>
                <div class="panel-body">

                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
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
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Descrizione</span>
                                        <asp:TextBox ID="Txt_Descrizione" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-3 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Inizio</span>
                                        <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="form-control DatePicker"
                                            MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-3 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Fine</span>
                                        <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="form-control DatePicker"
                                            MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-2 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Anno</span>
                                        <asp:TextBox ID="Txt_Anno" runat="server" CssClass="form-control "></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-8 col-md-6 col-xs-12  ">
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
                                    <% If Master.Master_versione <> "Agronica" Then %>
                                    <span class="input-group-addon alert-info gias-check-container-no-border">Dichiarazione di Non Utilizzo Fertilizzanti</span>
                                    <% End If %>
                                    <div class="input-group gias-asp-check switch">
                                        <% If Master.Master_versione = "Agronica" Then %>
                                        <span class="input-group-addon alert-info gias-check-container-no-border">Dichiarazione di Non Utilizzo Fertilizzanti</span>
                                        <span class="form-control" style="display: inline-block; width: 30px;">
                                            <% End If %>
                                            <asp:CheckBox ID="Chk_NonUtilizzo_Fertilizzanti" runat="server" ClientIDMode="Static"></asp:CheckBox>
                                            <% If Master.Master_versione <> "Agronica" Then %>
                                            <label for="Chk_NonUtilizzo_Fertilizzanti" class="switch-label">Switch</label>
                                            <% Else %>
                                        </span>
                                        <% End If %>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                </div>
            </div>
        </div>
        <!-- COLTURA -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-coltura">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b>DATI COLTURA E UBICAZIONE</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Centro Aziendale</span>
                                                <asp:DropDownList ID="Cmb_Centro" runat="server" CssClass="form-control selectpicker required stato_group"
                                                    data-live-search="true" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <div class="input-group">
                                        <asp:RadioButtonList ID="RBL_Specie" runat="server" Style="float: left;" Font-Size="10px"
                                            CellPadding="0" CellSpacing="0" RepeatDirection="Horizontal" AutoPostBack="true">
                                            <asp:ListItem Value="0" Selected="True">Visualizza le Specie Vegetali previste da Piano Concimazione</asp:ListItem>
                                            <asp:ListItem Value="1">Visualizza le Specie Vegetali presenti nel Piano Colturale</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Specie Vegetale</span>
                                                <asp:DropDownList ID="Cmb_Specie" runat="server" CssClass="form-control selectpicker required stato_group"
                                                    data-live-search="true" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Finalità</span>
                                                <asp:DropDownList ID="ddlFinalitaRer" runat="server" CssClass="form-control selectpicker required stato_group"
                                                    data-live-search="true" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Fase / Ciclo</span>
                                                <asp:DropDownList ID="ddlFaseCiclo" runat="server" CssClass="form-control selectpicker required"
                                                    data-live-search="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">RESA DICHIARATA [t/Ha]</span>
                                                <asp:TextBox ID="Txt_Resa" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-group">
                                        <span class="col-form-label">RESA RIFERIMENTO [T/HA]</span>
                                        <label id="Lbl_ResaRiferimento" runat="server" style="font-weight: bold; margin-left: 10PX"></label>
                                    </div>
                                </div>
                                <%--                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-group">
                                        <span class="col-form-label">FATTORE CORRETTIVO N [KG/T]</span>
                                        <label id="Lbl_FattoreCorrettivoResa" runat="server" style="font-weight: bold; margin-left: 10PX"></label>
                                    </div>
                                </div>--%>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Anticipazioni [Anni]</span>
                                                <asp:TextBox ID="Txt_AnticipazioniAnni" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Fissazione N [%] (Colture Leguminose)</span>
                                                <asp:TextBox ID="Txt_NFissazione" runat="server" CssClass="form-control">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <div class="col-md-12 nopadding">
                            <div class="row">
                            </div>
                        </div>
                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <% If Master.Master_versione <> "Agronica" Then %>
                                            <span class="input-group-addon alert-info gias-check-container-no-border">Coltura Protetta</span>
                                            <% End If %>
                                            <div class="input-group gias-asp-check switch">
                                                <% If Master.Master_versione = "Agronica" Then %>
                                                <span class="input-group-addon alert-info gias-check-container-no-border">Coltura Protetta</span>
                                                <span class="form-control" style="display: inline-block; width: 30px;">
                                                    <% End If %>
                                                    <asp:CheckBox ID="Chk_Serra" runat="server" ClientIDMode="Static"></asp:CheckBox>
                                                    <% If Master.Master_versione <> "Agronica" Then %>
                                                    <label for="Chk_Serra" class="switch-label">Switch</label>
                                                    <% Else %>
                                                </span>
                                                <% End If %>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <% If Master.Master_versione <> "Agronica" Then %>
                                            <span class="input-group-addon alert-info gias-check-container-no-border">ZVN</span>
                                            <% End If %>
                                            <div class="input-group gias-asp-check switch">
                                                <% If Master.Master_versione = "Agronica" Then %>
                                                <span class="input-group-addon alert-info gias-check-container-no-border">ZVN</span>
                                                <span class="form-control" style="display: inline-block; width: 30px;">
                                                    <% End If %>
                                                    <asp:CheckBox ID="Chk_ZVN" runat="server" ClientIDMode="Static"></asp:CheckBox>
                                                    <% If Master.Master_versione <> "Agronica" Then %>
                                                    <label for="Chk_ZVN" class="switch-label">Switch</label>
                                                    <% Else %>
                                                </span>
                                                <% End If %>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Area Omogenea</span>
                                                <asp:TextBox ID="Txt_Area" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Ubicazione</span>
                                                <asp:DropDownList ID="ddlUbicazione" runat="server" CssClass="form-control selectpicker required stato_group"
                                                    data-live-search="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- SUOLO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-suolo">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b>CARATTERISTICHE SUOLO</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">

                    <div class="row">
                        <div class="col-lg-9 col-md-9 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Analisi</span>
                                        <asp:DropDownList ID="ddlAnalisi" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>


                        <div class="col-lg-3 col-md-3 col-xs-12" style="display: flex; align-items: center">
                            <asp:Button ID="BtnAnalisi" runat="server" Style="width: 100%" class="btn btn-info" Text="ANALISI" />
                            <asp:Button ID="Btn_hidden_CaricaAnalisi" ClientIDMode="Static" runat="server" CssClass="hidden" />
                            <asp:Button ID="BtnVisualizza" runat="server" Style="width: 100%" class="btn btn-info" />
                            <div style="width: 5px; height: auto; display: inline-block"></div>
                            <asp:Button ID="BtnRicerca" runat="server" Style="width: 100%" class="btn btn-info" Text="<%$ Resources: PianoConcimazione_2017, GestioneAnalisi %>" />
                        </div>


                    </div>
                    <div class="row">

                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">Sabbia [%]</span>
                                            <asp:TextBox ID="Txt_Sabbia" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">Argilla [%]</span>
                                            <asp:TextBox ID="Txt_Argilla" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">Limo [%]</span>
                                            <asp:TextBox ID="Txt_Limo" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">pH</span>
                                            <asp:TextBox ID="Txt_PH" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">Calc.Tot. [%]</span>
                                            <asp:TextBox ID="Txt_CalcTot" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">Calc.Att. [%]</span>
                                            <asp:TextBox ID="Txt_CalcAtt" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">S.O. [%]</span>
                                            <asp:TextBox ID="Txt_SO" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">C/N</span>
                                            <asp:TextBox ID="Txt_CN" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">N [g/kg]</span>
                                            <asp:TextBox ID="Txt_N" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:DropDownList ID="DDL_P2O5" runat="server" Style="border: 0px; background-color: #d9edf7">
                                                    <asp:ListItem Value="0">P2O5 [ppm]</asp:ListItem>
                                                    <asp:ListItem Value="1">P [ppm]</asp:ListItem>
                                                </asp:DropDownList>
                                            </span>

                                            <asp:TextBox ID="Txt_P" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:DropDownList ID="DDL_K2O" runat="server" Style="border: 0px; background-color: #d9edf7">
                                                    <asp:ListItem Value="0">K2O [ppm]</asp:ListItem>
                                                    <asp:ListItem Value="1">K [ppm]</asp:ListItem>
                                                </asp:DropDownList>
                                            </span>
                                            <asp:TextBox ID="Txt_K" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">MgO [ppm]</span>
                                            <asp:TextBox ID="Txt_Mg" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">C.S.C. [meq/100 g]</span>
                                            <asp:TextBox ID="Txt_CSC" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">Disponibilità Ossigeno</span>
                                            <asp:DropDownList ID="ddlDispOssigeno" runat="server" CssClass="form-control selectpicker required stato_group"
                                                data-live-search="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12" id="Div_SalvaAnalisi" runat="server">
                                <asp:Button ID="Btn_SalvaAnalisi" runat="server" Style="width: 100%; text-align: center; font-size: small;" class="btn btn-warning" ToolTip="SALVA COME NUOVA ANALISI" Text="SALVA COME NUOVA ANALISI (*)" />
                                <br />
                                <label style="font-size: xx-small">(*) Se è stato selezionato un Centro Aziendale l'analisi verrà associata a tale Centro, altrimenti verrà associata all'intera Azienda</label><br />
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">Descrizione Analisi</span>
                                            <asp:TextBox ID="Txt_AnalisiDes" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- PRATICHE AGRONIMICHE -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-pratiche-agronomiche">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b>PRATICHE AGRONOMICHE</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-8 col-md-8 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Precessione</span>
                                        <asp:DropDownList ID="ddlPrecessione" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <% If Master.Master_versione <> "Agronica" Then %>
                                    <span class="input-group-addon alert-info gias-check-container-no-border">Semina su Sodo</span>
                                    <% End If %>
                                    <div class="input-group gias-asp-check switch">
                                        <% If Master.Master_versione = "Agronica" Then %>
                                        <span class="input-group-addon alert-info gias-check-container-no-border">Semina su Sodo</span>
                                        <span class="form-control" style="display: inline-block; width: 30px;">
                                            <% End If %>
                                            <asp:CheckBox ID="Chk_SeminaSodo" runat="server" ClientIDMode="Static"></asp:CheckBox>
                                            <% If Master.Master_versione <> "Agronica" Then %>
                                            <label for="Chk_SeminaSodo" class="switch-label">Switch</label>
                                            <% Else %>
                                        </span>
                                        <% End If %>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <% If Master.Master_versione <> "Agronica" Then %>
                                    <span class="input-group-addon alert-info gias-check-container-no-border">Irriguo</span>
                                    <% End If %>
                                    <div class="input-group gias-asp-check switch">
                                        <% If Master.Master_versione = "Agronica" Then %>
                                        <span class="input-group-addon alert-info gias-check-container-no-border">Irriguo</span>
                                        <span class="form-control" style="display: inline-block; width: 30px;">
                                            <% End If %>
                                            <asp:CheckBox ID="Chk_RegimeIrriguo" runat="server" ClientIDMode="Static"></asp:CheckBox>
                                            <% If Master.Master_versione <> "Agronica" Then %>
                                            <label for="Chk_RegimeIrriguo" class="switch-label">Switch</label>
                                            <% Else %>
                                        </span>
                                        <% End If %>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-5 col-md-5 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Tipo Fertilizzante</span>
                                        <asp:DropDownList ID="DdlTipoFertilizzante" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-5 col-md-5 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Frequenza</span>
                                        <asp:DropDownList ID="DdlFrequenza" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Qta N [kg/ha]</span>
                                        <asp:TextBox ID="Txt_QtaN_FerPrec" runat="server" CssClass="form-control" MaxLength="10">
                                        </asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- METEO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-meteo">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b>METEO</b>
                </h4>
            </div>
            <div>
                <input type="hidden" id="Meteo_ChkAgenda" runat="server" />
                <input type="hidden" id="Meteo_TipoSorgente" runat="server" />
                <input type="hidden" id="Meteo_TipoSorgente_Real" runat="server" />
                <input type="hidden" id="Meteo_Sorgente" runat="server" />
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group calc-width">
                                <label class="input-group-addon alert-info" id="lbl_Sorgente" for="TxtSorgente">
                                    <asp:Localize meta:resourcekey="SorgenteDati" runat="server">Categoria Sorgente Dati</asp:Localize></label>
                                <input type="text" name="cmbTipoSorgente" id="cmbTipoSorgente" value="" style="width: -webkit-fill-available;" />
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group calc-width">
                                <label class="input-group-addon alert-info" id="lbl_Origine" for="TxtTipologiaDti">
                                    <asp:Localize meta:resourcekey="OrigineDati" runat="server">Origine Dati Meteo</asp:Localize>
                                </label>
                                <input type="text" name="cmbOrigineDati" id="cmbOrigineDati" value="" style="width: -webkit-fill-available;" />
                                <%--<input type="text" class="form-control" name="cmbOrigineDati" id="cmbOrigineDati" value="" style="width: -webkit-fill-available;" />--%>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <input type="checkbox" id="opRegGiasChk" class="k-checkbox" data-bind="checked: checkboxChecked, events: { change: clickHandler }">
                            <label class="k-checkbox-label" for="opRegGiasChk">Leggi i dati delle operazioni registrate</label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label_data_fine_piogge" runat="server" CssClass="input-group-addon alert-info">Precipitazioni dal 01/10 al 31/01</asp:Label>
                                <asp:TextBox ID="Txt_Pioggia" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label4" runat="server" CssClass="input-group-addon alert-info">Precipitazioni Febbraio</asp:Label>
                                <asp:TextBox ID="Txt_Pioggia_Febbraio" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <button id="Btn_Meteo" class="btn btn-info" style="width: 100%;" onclick="return CaricaPiogge()">Carica Piogge</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- BOTTONE VISUALIZZA BILANCIO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-bottone-bilancio" id="Div_BtnVisualizza" runat="server" style="background-color: none">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b></b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="btn btn-info btn_100" onclick="$('#<%=Btn_Bilancio.ClientID %>').click();">
                                CALCOLA BILANCIO
                            </div>
                            <asp:Button ID="Btn_Bilancio" runat="server" Style="display: none" />

                            <input type="hidden" id="Testata_Cod" runat="server" />

                            <input type="hidden" id="N_Ammesso" runat="server" />
                            <input type="hidden" id="K_Ammesso" runat="server" />
                            <input type="hidden" id="P_Ammesso" runat="server" />

                            <!--  <input type="hidden" id="N_MAS" runat="server" /> -->
                            <input type="hidden" id="Fattore_Correttivo_N_Resa" runat="server" />

                            <input type="hidden" id="ResaBassa" runat="server" />
                            <input type="hidden" id="ResaAlta" runat="server" />
                            <input type="hidden" id="ResaBassaDes" runat="server" />
                            <input type="hidden" id="ResaAltaDes" runat="server" />

                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- BILANCIO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-bilancio" id="Div_Bilancio" runat="server">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b>BILANCIO</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <asp:GridView ID="grdNecessita" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 100%">
                            <Columns>
                                <asp:BoundField DataField="Indice" HeaderText="" Visible="false" />
                                <asp:BoundField DataField="Descrizione" HeaderText="Necessita'" />
                                <asp:BoundField DataField="N" HeaderText="N [Kg/Ha]" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="P" HeaderText="P2O5 [Kg/Ha]" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="K" HeaderText="K2O [Kg/Ha]" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                            </Columns>
                            <HeaderStyle CssClass="ui-widget-header" />
                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                    <div class="row">
                        <asp:GridView ID="GrdDisponibilita" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 100%">
                            <Columns>
                                <asp:BoundField DataField="Indice" HeaderText="" Visible="false" />
                                <asp:BoundField DataField="Descrizione" HeaderText="Disponibilita'" />
                                <asp:BoundField DataField="N" HeaderText="N [Kg/Ha]" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="P" HeaderText="P2O5 [Kg/Ha]" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="K" HeaderText="K2O [Kg/Ha]" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                            </Columns>
                            <HeaderStyle CssClass="ui-widget-header" />
                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>

                    <div class="row" style="margin-top: 10px">
                        <div>
                            <asp:DropDownList ID="ddlMasB" runat="server" CssClass="myCombo" Style="margin-bottom: 5px;" ForeColor="Red"></asp:DropDownList>
                            <asp:Label ID="LblAttenzione" runat="server" CssClass="txtUI" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- SCHEDE -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-schede" id="Div_Schede" runat="server">
            <div class="panel-heading">
                <h4 class="panel-title"></h4>
            </div>
            <div>
                <div class="panel-body">

                    <!-- AZOTO -->
                    <div class="row">


                        <div class="panel panel-primary">
                            <div class="panel-heading" align="center">
                                <h4 class="panel-title">
                                    <b>AZOTO</b>
                                </h4>
                            </div>
                            <div>
                                <div class="panel-body">
                                    <div id="tab_Scheda_N" runat="server" class="row">
                                        <div class="row">
                                            <div class="col-lg-4 col-md-4 col-xs-12  ">
                                                <div style="float: left; padding: 10px;">
                                                    <asp:Button ID="RicaricaGrid" runat="server" Style="display: none;" />
                                                    <asp:GridView ID="grdDecrementi" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                        AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="Chk_SelDecrementi_N" CssClass="Chk_SelDecrementi_N" ToolTip="Seleziona"
                                                                        runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Fattore_Des" HeaderText="Decrementi" />
                                                            <asp:BoundField DataField="Valore" HeaderText="Valore" ItemStyle-HorizontalAlign="center">
                                                                <ItemStyle CssClass="Variazione" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                        <HeaderStyle CssClass="ui-widget-header" />
                                                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                                    </asp:GridView>

                                                </div>
                                            </div>

                                            <div class="col-lg-4 col-md-4 col-xs-12  ">
                                                <div style="float: left; padding: 10px;">

                                                    <asp:GridView ID="grdIncrementi" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                        AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="Chk_SelIncrementi_N" ToolTip="Seleziona" runat="server" CssClass="Chk_SelIncrementi_N" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Fattore_Cod">
                                                                <ItemStyle CssClass="Codice displaynone" />
                                                                <HeaderStyle CssClass="displaynone" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Fattore_Des" HeaderText="Incrementi" />
                                                            <asp:BoundField DataField="Valore" HeaderText="Valore" ItemStyle-HorizontalAlign="center">
                                                                <ItemStyle CssClass="Variazione" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                        <HeaderStyle CssClass="ui-widget-header" />
                                                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                                    </asp:GridView>



                                                </div>

                                            </div>

                                            <div class="col-lg-4 col-md-4 col-xs-12  ">
                                                <div style="min-height: 150px; float: left; min-width: 250px; padding: 10px;">
                                                    <div>
                                                        <asp:Label ID="TextBox1" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            Text="Dose standard :" Width="135px" Height="20px"></asp:Label>
                                                        <asp:TextBox ID="Txt_DoseStandard" runat="server" CssClass="txtui"
                                                            MaxLength="250" Style="margin-left: 5px; pointer-events: none" Width="60px"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="LabelMAS" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            Text="Limite MAS :" Width="135px" Height="20px"></asp:Label><br />
                                                        <!--      <asp:TextBox ID="Txt_MAS" runat="server" CssClass="txtui" MaxLength="250" Style="margin-left: 5px; pointer-events: none"
                                                        ToolTip="La dose non può superare il MAS" Width="60px"></asp:TextBox> -->
                                                        <asp:DropDownList ID="ddlMasS" runat="server" CssClass="myCombo" Width="215px" Style="margin-bottom: 5px;"></asp:DropDownList>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label5" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            Text="Max Incrementi :" Width="135px" Height="20px"></asp:Label>
                                                        <asp:TextBox ID="Txt_MaxIncrementi" runat="server" CssClass="txtui" MaxLength="250"
                                                            Style="margin-left: 5px; pointer-events: none" ToolTip="Massimo valore consentito per gli incrementi"
                                                            Width="60px"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label1" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            Text="Totale incrementi :" Width="135px" Height="20px"></asp:Label>
                                                        <asp:TextBox ID="Txt_TotIncrementi" runat="server" CssClass="txtui" MaxLength="250"
                                                            Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                        </asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label2" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            Text="Totale decrementi :" Width="135px" Height="20px"></asp:Label>
                                                        <asp:TextBox ID="Txt_TotDecrementi" runat="server" CssClass="txtui" MaxLength="250"
                                                            Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                        </asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label3" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            Text="Dose ricalcolata :" Width="135px" Height="20px"></asp:Label>
                                                        <asp:TextBox ID="Txt_DoseRicalcolata" runat="server" CssClass="txtui" Font-Bold="True" MaxLength="250"
                                                            Style="margin-left: 5px; pointer-events: none" ToolTip="Dose di N consentita" Width="60px">
                                                        </asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="LabelMAS_Nota" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            Text="" Height="20px"></asp:Label>

                                                    </div>
                                                </div>

                                            </div>

                                        </div>

                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- FOSFORO -->
                    <div class="row">

                        <div class="panel panel-primary">
                            <div class="panel-heading" align="center">
                                <h4 class="panel-title">
                                    <b>FOSFORO</b>
                                </h4>
                            </div>
                            <div>
                                <div class="panel-body">

                                    <div id="tab_Scheda_P" runat="server" class="row">

                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="float: left; padding: 10px">

                                                <asp:GridView ID="grdDecrementiP" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                    AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="Chk_SelDecrementi_P" CssClass="Chk_SelDecrementi_P" ToolTip="Seleziona"
                                                                    runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Fattore_Des" HeaderText="Decrementi" />
                                                        <asp:BoundField DataField="Valore" HeaderText="Valore" ItemStyle-HorizontalAlign="center">
                                                            <ItemStyle CssClass="Variazione" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <HeaderStyle CssClass="ui-widget-header" />
                                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                                </asp:GridView>


                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="float: left; padding: 10px;">

                                                <asp:GridView ID="grdIncrementiP" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                    AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="Chk_SelIncrementi_P" ToolTip="Seleziona" runat="server" CssClass="Chk_SelIncrementi_P" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Fattore_Cod">
                                                            <ItemStyle CssClass="Codice displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Fattore_Des" HeaderText="Incrementi" />
                                                        <asp:BoundField DataField="Valore" HeaderText="Valore" ItemStyle-HorizontalAlign="center">
                                                            <ItemStyle CssClass="Variazione" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <HeaderStyle CssClass="ui-widget-header" />
                                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                                </asp:GridView>


                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="min-height: 150px; float: left; min-width: 250px; padding: 10px;">
                                                <div>
                                                    <asp:Label ID="Label7" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Selezionare la dose" Width="135px" Height="20px"></asp:Label>
                                                    <br />
                                                    <asp:DropDownList ID="ddlDoseP" runat="server" CssClass="myCombo" Width="215px"
                                                        Style="margin-bottom: 5px;">
                                                    </asp:DropDownList>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label6" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Dose standard :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_DoseStandardP" runat="server" CssClass="txtui"
                                                        MaxLength="250" Style="margin-left: 5px; pointer-events: none" Width="60px"></asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label8" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Totale incrementi :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_TotIncrementiP" runat="server" CssClass="txtui" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label9" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Totale decrementi :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_TotDecrementiP" runat="server" CssClass="txtui" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label10" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Dose ricalcolata :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_DoseRicalcolataP" runat="server" CssClass="txtui" Font-Bold="True" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" ToolTip="Dose di P consentita" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                            </div>

                                        </div>
                                    </div>



                                </div>
                            </div>
                        </div>

                    </div>

                    <!-- POTASSIO -->
                    <div class="row">
                        <div class="panel panel-primary">
                            <div class="panel-heading" align="center">
                                <h4 class="panel-title">
                                    <b>POTASSIO</b>
                                </h4>
                            </div>
                            <div>
                                <div class="panel-body">

                                    <div id="tab_Scheda_K" runat="server" class="row">
                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="float: left; padding: 10px;">
                                                <asp:GridView ID="grdDecrementiK" runat="server" AllowPaging="false"
                                                    AutoGenerateColumns="False" CellPadding="10" CellSpacing="10">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="Chk_SelDecrementi_K" CssClass="Chk_SelDecrementi_K" ToolTip="Seleziona"
                                                                    runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Fattore_Des" HeaderText="Decrementi" />
                                                        <asp:BoundField DataField="Valore" HeaderText="Valore" ItemStyle-HorizontalAlign="center">
                                                            <ItemStyle CssClass="Variazione" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <HeaderStyle CssClass="ui-widget-header" />
                                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />

                                                </asp:GridView>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="float: left; padding: 10px;">

                                                <asp:GridView ID="grdIncrementiK" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                    AutoGenerateColumns="False" CellPadding="10" CellSpacing="10" EnableModelValidation="True" Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="Chk_SelIncrementi_K" ToolTip="Seleziona" runat="server" CssClass="Chk_SelIncrementi_K" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Fattore_Cod">
                                                            <ItemStyle CssClass="Codice displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Fattore_Des" HeaderText="Incrementi" />
                                                        <asp:BoundField DataField="Valore" HeaderText="Valore" ItemStyle-HorizontalAlign="center">
                                                            <ItemStyle CssClass="Variazione" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <HeaderStyle CssClass="ui-widget-header" />
                                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                                </asp:GridView>



                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="min-height: 150px; float: left; min-width: 250px; padding: 10px; width: 100%">
                                                <div>
                                                    <asp:Label ID="Label11" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Selezionare la dose" Width="135px" Height="20px"></asp:Label>
                                                    <br />
                                                    <asp:DropDownList ID="ddlDoseK" runat="server" CssClass="myCombo" Width="215px"
                                                        Style="margin-bottom: 5px;">
                                                    </asp:DropDownList>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label12" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Dose standard :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_DoseStandardK" runat="server" CssClass="txtui"
                                                        MaxLength="250" Style="margin-left: 5px; pointer-events: none" Width="60px"></asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label13" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Totale incrementi :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_TotIncrementiK" runat="server" CssClass="txtui" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label14" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Totale decrementi :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_TotDecrementiK" runat="server" CssClass="txtui" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label15" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        Text="Dose ricalcolata :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_DoseRicalcolataK" runat="server" CssClass="txtui" Font-Bold="True" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" ToolTip="Dose di K consentita" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                            </div>

                                        </div>
                                    </div>


                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- BOTTONE SALVA PIANO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-salva-piano" id="Div_BTNSalva" runat="server">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b></b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="btn btn-success btn_100" onclick="$('#<%=Btn_Salva.ClientID %>').click();">
                                SALVA PIANO CONCIMAZIONE
                            </div>
                            <asp:Button ID="Btn_Salva" runat="server" Style="display: none" />



                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- IMPIANTI -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-impianti" id="Div_Appezzamenti" runat="server" style="margin-bottom: 75px;">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b>APPEZZAMENTI</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row" id="Div_NPK_Calcolati" runat="server">
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label17" runat="server" CssClass="input-group-addon alert-info">N [Kg/Ha]</asp:Label>
                                <asp:TextBox ID="Txt_N_Da_Applicare" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label18" runat="server" CssClass="input-group-addon alert-info">P2O5 [Kg/Ha]</asp:Label>
                                <asp:TextBox ID="Txt_P_Da_Applicare" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label19" runat="server" CssClass="input-group-addon alert-info">K2O [Kg/Ha]</asp:Label>
                                <asp:TextBox ID="Txt_K_Da_Applicare" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row" id="Div_BtnApplica" runat="server" style="margin-bottom: 10px">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="btn btn-success btn_100" onclick="$('#<%=Btn_Applica.ClientID %>').click();">
                                ASSOCIA N - P2O5 - K2O AGLI APPEZZAMENTI SELEZIONATI
                            </div>
                            <asp:Button ID="Btn_Applica" runat="server" Style="display: none" />
                        </div>
                    </div>
                    <div class="row" style="overflow: scroll;">
                        <asp:GridView ID="GridView_Impianti" runat="server" AutoGenerateColumns="False" CellPadding="5"
                            CssClass="ui-widget-content">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <input type="checkbox" id="chkSelezionaTuttiImpianti" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ChkSelezionaImpianto" runat="server" CssClass="ChkSelezionaImpianto" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    <FooterStyle Width="20px" />
                                    <ControlStyle Width="20px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Campo_Cod" HeaderText="Campo_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Appezza" HeaderText="Appezza" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Id_Reg" HeaderText="Id_Reg" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="progetto_cod" HeaderText="progetto_cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Veg_Cod" HeaderText="Veg_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Cul_Cod" HeaderText="Cul_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Grfi_Cod" HeaderText="Grfi_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="rag_soc" HeaderText="rag_soc" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" SortExpression="Sa_Nome"></asp:BoundField>
                                <asp:BoundField DataField="Campo_Nome" HeaderText="Campo" SortExpression="Campo_Nome"></asp:BoundField>

                                <asp:BoundField DataField="App_Nome" HeaderText="App." SortExpression="App_Nome"></asp:BoundField>
                                <asp:BoundField DataField="Catasto" HeaderText="Catasto" SortExpression="Catasto" HtmlEncode="false"></asp:BoundField>

                                <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" SortExpression="Descrizione"></asp:BoundField>
                                <asp:BoundField DataField="Sup_Imp" HeaderText="Sup.[ha]" SortExpression="Sup_Imp">
                                    <ItemStyle CssClass="Sup_Imp" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Validita_Inizio" HeaderText="Data Inizio Impianto" SortExpression="Validita_Inizio"></asp:BoundField>
                                <asp:BoundField DataField="Validita_Fine" HeaderText="Data Fine Impianto" SortExpression="Validita_Fine"></asp:BoundField>
                                <asp:BoundField DataField="QtaMaxN" HeaderText="Qta N Max [Kg/Ha]" SortExpression="QtaMaxN"></asp:BoundField>
                                <asp:BoundField DataField="QtaMaxP2O5" HeaderText="Qta P Max [Kg/Ha]" SortExpression="QtaMaxP2O5"></asp:BoundField>
                                <asp:BoundField DataField="QtaMaxK2O" HeaderText="Qta K Max [Kg/Ha]" SortExpression="QtaMaxK2O"></asp:BoundField>

                            </Columns>
                            <HeaderStyle CssClass="ui-widget-header" />
                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>

        <asp:UpdatePanel ID="UpdatePanelPerScript" runat="server">
            <ContentTemplate>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
</asp:Content>
