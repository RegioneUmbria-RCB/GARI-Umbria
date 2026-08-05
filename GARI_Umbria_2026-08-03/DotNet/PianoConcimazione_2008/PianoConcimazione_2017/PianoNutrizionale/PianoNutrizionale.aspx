<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master"
    CodeBehind="PianoNutrizionale.aspx.vb" Inherits="PianoConcimazione_2017.PianoNutrizionale" %>

<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Piano Nutrizionale</title>
    
    <style type="text/css">
    
        .pad {
            padding: 5px;
        }

    </style>
    
    <script type="text/javascript">

        var Qs_Piva;
        var Qs_Operazione;
        var Qs_Tipo;
        var QS_AnalisiNG;

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
                    $(this).children('input').removeProp('checked');
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
            return true
        }


        /****************************************************************************************************************/

        function Valore_FattoreVariazione(Oggetto) {
            return Oggetto.parent().parent().parent().children('.Variazione').html().replace(',', '.');
        }

        function Valore_FattoreCodice(Oggetto) {
            return Oggetto.parent().parent().parent().children('.Codice').html().replace(',', '.');
        }


        /****************************************************************/
        /****************************************************************/
        /****************************************************************/


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
            QS_AnalisiNG = $('#<%=hd_usaAnalisiModelloNG.ClientID %>').val();
            
            $('.DatePicker').kendoDatePicker();

            <%--$('#<%=ddlRegolamento.ClientID %>').change(function () {
                AggiornaDescrizione();
            });--%>

            $('#chkSelezionaTuttiImpianti').click(function () {
                SelezionaDeselezionaTutti();
            });



            $('#<%=DDL_P2O5.ClientID %>').change(function () {
                ddlP_Click();
            });

            $('#<%=DDL_K2O.ClientID %>').change(function () {
                ddlK_Click();
            });



            $('#iFrameGeneric').on('hidden.bs.modal', function () {
                iFrame_Chiusura();
            });

            //NON SERVE PER ORA
            //let resp = await SportelloPCB();

            //if (!resp.Sportello_Aperto) {
            //    $("#MainContent_Div_BTNSalva").hide();
            //    if (Qs_Operazione == "1") {
            //        kendo.alert("Non è possibile creare il Piano Nutrizionale. Sportello chiuso.")
            //    }
            //}

          <%--  $('#<%=Txt_ValiditaInizio.ClientID %>').kendoDatePicker({
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
            });--%>


            /****************** METEO ******************/
            $('#<%=Meteo_ChkAgenda.ClientId %>').val(0);

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

            cmbOrigineDati = $("#cmbOrigineDati").kendoDropDownList({
                autoBind: true,
                autoWidth: true,
                dataTextField: "nome_stazione",
                dataValueField: "id_stazione",
                filter: "contains",
                optionLabel: "Seleziona...",
                template: "<div class='k-state-default'><span style='float:left;'>#: data.nome_stazione #</span><span style='float:right;'>#: data.distanza #</span></div>",
                //valueTemplate: "#: data.sorgente #",
                change: function (e) {

                    $('#<%=Meteo_TipoSorgente_Real.ClientId %>').val(e.sender.dataItem().tipo_sorgente);
                    $('#<%=Meteo_Sorgente.ClientId %>').val(this.value());
                }
            }).data("kendoDropDownList");

            $.each($(".calc-width"), function () {
                $(this).children(".input-group-addon").css("width", $(this).children(".input-group-addon").outerWidth());
                $(this).css({ "table-layout": "fixed", "width": "100%" });
            });

            //setto il default 
            if ($('#<%=Meteo_TipoSorgente.ClientId %>').val() !== '0') {
                cmbTipoSorgente.value($('#<%=Meteo_TipoSorgente.ClientId %>').val());
                cmbTipoSorgente.trigger("change");
            }
            if ($('#<%=Meteo_Sorgente.ClientId %>').val() !== '0') {
                cmbOrigineDati.value($('#<%=Meteo_Sorgente.ClientId %>').val());
            }

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

            if (QS_AnalisiNG == "True") {
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


        // NON SERVE PER ORA
        //function SportelloPCB() {
        //    return new Promise((resolve, reject) => {
        //        var parametri = kendo.stringify({
        //            Piva: Qs_Piva
        //        });

        //        ajaxAgronica("PianoNutrizionale.aspx/SportelloPCB",
        //            parametri,
        //            function (risposta) {
        //                resolve(JSON.parse(risposta.RispostaStringa));
        //            }, null, null, false);
        //    });
        //}


        function CaricaPiogge() {
            let param = {
                PIVA: Qs_Piva,
                SaCod: $('#<%=Cmb_Centro.ClientID %> option:selected').val(),
                Anno: $('#<%=Txt_Anno.ClientID %>').val(),
                leggiDaAgenda: $('#<%=Meteo_ChkAgenda.ClientId %>').val(),
                tipoSorgente: $('#<%=Meteo_TipoSorgente_Real.ClientId %>').val(),
                sorgente: $('#<%=Meteo_Sorgente.ClientId %>').val(),
                regolamento: $('#<%=ddlRegolamento.ClientID %> option:selected').val()
            };

            ajaxAgronicaSync("PianoNutrizionale.aspx/Carica_Piogge_WM",
                JSON.stringify(param),
                true,
                function (risposta) {
                    let values = JSON.parse(risposta.RispostaStringa);

                    $('#<%=Txt_Pioggia_Autunno.ClientId %>').val(values.Pioggia);
                    $('#<%=Txt_Pioggia_Primavera.ClientId %>').val(values.Pioggia_Primavera);
                },
                null
            );
            return false;
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField id="hd_Piva" runat="server" />
    <asp:HiddenField id="hd_Operazione" runat="server" />
    <asp:HiddenField id="hd_Tipo" runat="server" />
    <asp:HiddenField ID="hd_usaAnalisiModelloNG" runat="server" />
    <div class="container">

        <!-- TESTATA -->
        <div class="panel panel-primary">

            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>DATI TESTATA PIANO</b>
                </h4>
            </div>

            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Piano Nutrizionale</span>
                                        <asp:DropDownList ID="ddlRegolamento" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-xs-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Descrizione</span>
                                        <asp:TextBox ID="Txt_Descrizione" runat="server" CssClass="form-control">
                                        </asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-3 col-xs-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Inizio</span>
                                        <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="form-control DatePicker" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-3 col-xs-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Fine</span>
                                        <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="form-control DatePicker" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-2 col-md-2 col-xs-12">
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
                        <div class="col-lg-10 col-md-12 col-xs-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Note</span>
                                        <asp:TextBox ID="Txt_Note" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-2 col-md-12 col-sm-12" id="Div_Biologico" runat="server">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">BIOLOGICO</span>
                                        <asp:CheckBox ID="BiologicoChk" runat="server" CssClass="form-control" Width="30PX"></asp:CheckBox>
                                    </div>
                                </div>
                            </div>
                        </div>                                                
                    </div>
                </div>
            </div>
        </div>

        <!-- COLTURA -->
        <div class="panel panel-primary">

            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>DATI UBICAZIONE E CARATTERISTICHE SUOLO </b>
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
                                <div class="col-lg-4 col-md-12 col-sm-12">
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

                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <div class="input-group">
                                        <asp:RadioButtonList ID="RBL_Specie" runat="server" Style="float: left;" Font-Size="10px"
                                            CellPadding="0" CellSpacing="0" RepeatDirection="Horizontal" AutoPostBack="true">
                                            <asp:ListItem Value="0" Selected="True">Visualizza le Specie Vegetali del Piano Nutrizionale</asp:ListItem>
                                            <asp:ListItem Value="1">Visualizza le Specie Vegetali del Piano Colturale</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>

                                <div class="col-lg-2 col-md-12 col-sm-12" id="Div_Resa" runat="server">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">Resa</span>
                                                <asp:TextBox ID="Txt_Resa" runat="server" CssClass="form-control">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%--<div class="col-md-12 nopadding">
                            <div class="row">                                
                            </div>
                        </div>--%>

                        <div class="col-lg-9 col-md-9 col-xs-12" id="Div_Analisi" runat="server">
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

                        <div class="col-lg-3 col-md-3 col-xs-12" id="Div_ButtonAnalisi" runat="server" style="display:flex;align-items:center">
                            <asp:Button ID="Btn_Analisi" runat="server" Style="width: 100%" class="btn btn-info" Text="ANALISI"/>
                            <asp:Button ID="Btn_hidden_CaricaAnalisi" ClientIDMode="Static" runat="server" CssClass="hidden"/>
                            <asp:Button ID="BtnVisualizza" runat="server" Style="width: 100%" class="btn btn-info" />
                            <div style="width:5px;height:auto;display:inline-block"></div>
                            <asp:Button ID="BtnRicerca" runat="server" Style="width: 100%" class="btn btn-info" Text="<%$ Resources: PianoConcimazione_2017, GestioneAnalisi %>" />
                        </div>
                    </div>

                    <div class="row" id="Div_ParametriAnalisi" runat="server">
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
                        </div>

                        <div class="row" id="Div_ParametriAnalisi2" runat="server">
                            <div class="col-lg-3 col-md-2 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">NTOT [g/kg]</span>
                                            <asp:TextBox ID="Txt_NTOT" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-2 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">NORG [g/kg] </span>
                                            <asp:TextBox ID="Txt_NORG" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-2 col-sm-12" id="Div_Ubicazione" runat="server">
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

                            <div class="col-lg-0 col-md-0 col-sm-0" style =" display:none">
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

                            <div class="col-lg-0 col-md-0 col-sm-0" style =" display:none">
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

                           <%-- <div class="col-lg-3 col-md-2 col-sm-12">
                            </div>--%>

                            <div class="col-lg-3 col-md-2 col-sm-12 " id="Div_SalvaAnalisi" runat="server">
                                <asp:Button ID="Btn_SalvaAnalisi" runat="server" Style="width: 100%; text-align: center; font-size: small;" class="btn btn-warning" ToolTip="SALVA COME NUOVA ANALISI" Text="SALVA COME NUOVA ANALISI (*)"/>
                                <br/>
                                <label style="font-size: xx-small">(*) Se è stato selezionato un Centro Aziendale l'analisi verrà associata a tale Centro, altrimenti verrà associata all'intera Azienda</label><br/>
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

                       <%--<div class="row">
                                <div class="col-lg-3 col-md-4 col-sm-12 col-lg-offset-9 col-md-offset-8" >                            
                                </div>
                            </div>--%>
                    </div>
                </div>
            </div>
        </div>

        <!-- PRATICHE AGRONIMICHE -->
        <div class="panel panel-primary">
            <div class="panel-heading">

                <h4 class="panel-title">
                    <b>PRATICHE AGRONOMICHE</b>
                </h4>
            </div>

            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-6 col-md-12 col-sm-12">
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

                        <div class="col-lg-6 col-md-12 col-sm-12" id="Div_Precessione" runat="server">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Precessione Anno Precedente</span>
                                        <asp:DropDownList ID="ddlPrecessioneAnnoPrecedente" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row" id="Div_Precessione2" runat="server">
                        <div class="col-lg-6 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Precessione 2 Anni Prima</span>
                                        <asp:DropDownList ID="ddlPrecessioneDueAnniPrima" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-6 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Precessione 3 Anni Prima</span>
                                        <asp:DropDownList ID="ddlPrecessioneTreAnniPrima" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Fertilizzante Già Apportato</span>
                                        <asp:DropDownList ID="DdlTipoFertilizzante" runat="server" CssClass="form-control selectpicker required stato_group"
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

        <!-- METEO -->
        <div class="panel panel-primary">

            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>DATI METEO</b>
                </h4>
            </div>

            <div>
                <input type="hidden" id="Meteo_ChkAgenda" runat="server"/>
                <input type="hidden" id="Meteo_TipoSorgente" runat="server"/>
                <input type="hidden" id="Meteo_TipoSorgente_Real" runat="server"/>
                <input type="hidden" id="Meteo_Sorgente" runat="server"/>

                <div class="panel-body">
                    <div class="row" id="Div_Piogge" runat="server">
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group calc-width">
                                <label class="input-group-addon alert-info" id="lbl_Sorgente" for="TxtSorgente">
                                    <asp:Localize meta:resourcekey="SorgenteDati" runat="server">Categoria Sorgente Dati</asp:Localize></label>
                                <input type="text" name="cmbTipoSorgente" id="cmbTipoSorgente" value="" style="width: -webkit-fill-available;"/>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group calc-width">
                                <label class="input-group-addon alert-info" id="lbl_Origine" for="TxtTipologiaDti">
                                    <asp:Localize meta:resourcekey="OrigineDati" runat="server">Origine Dati Meteo</asp:Localize></label>
                                <input type="text" class="form-control" name="cmbOrigineDati" id="cmbOrigineDati" value="" style="width: -webkit-fill-available;"/>
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
                                <asp:Label ID="label_data_fine_piogge" runat="server" CssClass="input-group-addon alert-info">Precipitazioni Autunnali (Ott/Gen)</asp:Label>
                                <asp:TextBox ID="Txt_Pioggia_Autunno" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-12" id="Div_Pioggia_Primavera" runat="server">
                            <div class="input-group">
                                <asp:Label ID="label1" runat="server" CssClass="input-group-addon alert-info">Precipitazioni Primaverili (Mar/Apr)</asp:Label>
                                <asp:TextBox ID="Txt_Pioggia_Primavera" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>                        
                        
                        <div class="col-lg-4 col-md-4 col-sm-12" id="Div_Empty" runat="server">
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <button id="Btn_Meteo" class="btn btn-info" style="width: 100%;" onclick="return CaricaPiogge()">Carica Piogge</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- BOTTONE VISUALIZZA BILANCIO -->
        <div class="panel panel-primary" id="Div_BtnVisualizza" runat="server" style="background-color: none">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b></b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="btn btn-info btn_100" onclick="$('#<%=Btn_Bilancio.ClientID %>').click();">
                                CALCOLA VALORE &nbsp; N
                            </div>
                            <asp:Button ID="Btn_Bilancio" runat="server" Style="display: none"/>

                            <input type="hidden" id="Testata_Cod" runat="server"/>

                            <div id="Div_ValoriAmmessi" runat="server">
                                <div class="row" runat="server">
                                    <br/>
                                    <div class="col-lg-3 col-md-4 col-sm-12">
                                        <div class="input-group">
                                            <asp:Label ID="label2" runat="server" CssClass="input-group-addon alert-info">CONSIGLIO N [KG/HA]</asp:Label>
                                            <asp:TextBox ID="N_Ammesso" runat="server" CssClass="form-control" ReadOnly>
                                            </asp:TextBox>                                            
                                        </div>
                                    </div>
<%--                                 <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="input-group">
                                            <asp:Label ID="label3" runat="server" CssClass="input-group-addon alert-info">FRAZIONAMENTO</asp:Label>
                                            <asp:TextBox ID="N_Frazionamento" runat="server" CssClass="form-control" ReadOnly >
                                            </asp:TextBox>                                           
                                        </div>
                                    </div>--%>
                                    
                                    <div class="col-lg-6 col-md-4 col-sm-12" id="Div_N_Integrazione" runat="server">
                                        <div class="input-group">
                                            <asp:Label ID="label4" runat="server" CssClass="input-group-addon alert-info">INTEGRAZIONE CONSENTITA A SEGUITO DELLE PIOGGE PRIMAVERILI [Kg/Ha]</asp:Label>
                                            <asp:TextBox ID="N_Integrazione" runat="server" CssClass="form-control" ReadOnly >
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="row" runat="server">
                                    <div class="col-lg-12 col-md-12 col-sm-12" id="Div1" runat="server">
                                       <asp:Label ID="label3" runat="server" CssClass="input-group-addon alert-info">Note Generali</asp:Label>
                                       <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" ReadOnly TextMode="MultiLine" Rows="7">Per quantitativi superiori a 70 kg/Ha si consiglia di distribuire circa i 2/3 in pre-semina / pre-emergenza e la parte restante in copertura alla sarchiatura entro le 6-8 foglie. 
Quantitativi inferiori (da 40 a 70 kg/ha) possono essere anticipati integralmente alle prime fasi di sviluppo. Quantitativi molto bassi (inferiori a 40 kg /ha) possono essere riservati alla fase di sarchiatura.
Per quantitativi pari a zero va sempre tenuta in considerazione la possibilità di effettuare apporti con prodotti fogliari per massimizzare l’efficienza fotosintetica.
La informiamo inoltre che verrà monitorata le quantità di piogge primaverili con effetto dilavante per suggerire, all’occorrenza, eventuali integrazioni dell’elemento in sarchiatura.
Per la tipologia di fertilizzante da distribuire e per eventuali ulteriori delucidazioni consultare il proprio tecnico nonché leggere i bollettini inerenti all’argomento. I consigli sono orientativi in funzione della qualità del campionamento.
N.B.: Attenersi alle norme regionali e comunali relative alla distribuzione dei fertilizzanti azotati.</asp:TextBox>                                        
                                    </div>
                                 </div>

                                 <%--<div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="input-group">
                                            <asp:Label ID="label3" runat="server" CssClass="input-group-addon alert-info">P2O5 [Kg/Ha]</asp:Label>
                                            <asp:TextBox ID="K_Ammesso" runat="server" CssClass="form-control" ReadOnly>
                                            </asp:TextBox>
                                       </div>
                                 </div>
                                 
                                 <div class="col-lg-4 col-md-4 col-sm-12">
                                        <div class="input-group">
                                            <asp:Label ID="label4" runat="server" CssClass="input-group-addon alert-info">K2O [Kg/Ha]</asp:Label>
                                            <asp:TextBox ID="P_Ammesso" runat="server" CssClass="form-control" ReadOnly>
                                            </asp:TextBox>
                                        </div>
                                 </div>--%>
                            </div>

                           <div class="row" runat="server">
                                    <asp:GridView ID="grdConsiglioNPK" runat="server" CssClass="ui-widget-content" AllowPaging="false" 
                                        AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 33%"  >
                                        <Columns>
                                            <asp:BoundField DataField="Indice" HeaderText="" Visible="false"/>
                                            <asp:BoundField DataField="Consiglio_N"  HeaderText="Consiglio N" HtmlEncode="False" ItemStyle-Width="70px" ItemStyle-CssClass="pad"  ItemStyle-HorizontalAlign="left"  />
                                            <%--<asp:BoundField DataField="Consiglio_P" HeaderText="Consiglio P" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="left"/>
                                        <asp:BoundField DataField="Consiglio_K" HeaderText="Consiglio K" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="left"/>--%>
                                        </Columns>
                                        <HeaderStyle CssClass="ui-widget-header"/>
                                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center"/>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- BOTTONE SALVA PIANO -->
        <div class="panel panel-primary" id="Div_BTNSalva" runat="server">

            <div class="panel-heading">
                <h4 class="panel-title">
                    <b></b>
                </h4>
            </div>

            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12">
                            <div class="btn btn-success btn_100" onclick="$('#<%=Btn_Salva.ClientID %>').click();">
                                SALVA PIANO NUTRIZIONALE
                            </div>
                            <asp:Button ID="Btn_Salva" runat="server" Style="display: none"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- IMPIANTI -->
        <div class="panel panel-primary" id="Div_Appezzamenti" runat="server" style="margin-bottom: 75px;">

            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>APPEZZAMENTI</b>
                </h4>
            </div>

            <div>
                <div class="panel-body">
                    <%--<div class="row" id="Div_NPK_Calcolati" runat="server">
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
                    </div>--%>

                    <div class="row" id="Div_BtnApplica" runat="server" style="margin-bottom: 10px">
                        <div class="col-lg-12 col-md-12 col-xs-12">
                            <div class="btn btn-success btn_100" onclick="$('#<%=Btn_Applica.ClientID %>').click();">
                                ASSOCIA N AGLI APPEZZAMENTI SELEZIONATI
                            </div>
                            <asp:Button ID="Btn_Applica" runat="server" Style="display: none"/>
                        </div>
                    </div>

                    <div class="row" style="overflow: scroll;">
                        <asp:GridView ID="GridView_Impianti" runat="server" AutoGenerateColumns="False" CellPadding="5"
                            CssClass="ui-widget-content">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <input type="checkbox" id="chkSelezionaTuttiImpianti"/>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ChkSelezionaImpianto" runat="server" CssClass="ChkSelezionaImpianto"/>
                                    </ItemTemplate>
                                    <HeaderStyle Width="20px"/>
                                    <ItemStyle Width="20px"/>
                                    <FooterStyle Width="20px"/>
                                    <ControlStyle Width="20px"/>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="false" >
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="Campo_Cod" HeaderText="Campo_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="Appezza" HeaderText="Appezza" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="Id_Reg" HeaderText="Id_Reg" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="progetto_cod" HeaderText="progetto_cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>

                                <asp:BoundField DataField="Veg_Cod" HeaderText="Veg_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="Cul_Cod" HeaderText="Cul_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="Grfi_Cod" HeaderText="Grfi_Cod" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="rag_soc" HeaderText="rag_soc" HtmlEncode="false">
                                    <ItemStyle CssClass="displaynone"/>
                                    <HeaderStyle CssClass="displaynone"/>
                                </asp:BoundField>

                                <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" SortExpression="Sa_Nome" ItemStyle-Width="250px"></asp:BoundField>
                                <asp:BoundField DataField="Campo_Nome" HeaderText="Campo" SortExpression="Campo_Nome" ItemStyle-Width="150px"></asp:BoundField>

                                <asp:BoundField DataField="App_Nome" HeaderText="App." SortExpression="App_Nome" ItemStyle-Width="150px"></asp:BoundField>
                                <asp:BoundField DataField="Catasto" HeaderText="Catasto" SortExpression="Catasto" HtmlEncode="false" ItemStyle-Width="250px"></asp:BoundField>

                                <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" SortExpression="Descrizione" ItemStyle-Width="250px"></asp:BoundField>
                                <asp:BoundField DataField="Sup_Imp" HeaderText="Sup.[ha]" SortExpression="Sup_Imp" ItemStyle-Width="100px">
                                    <ItemStyle CssClass="Sup_Imp"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="Validita_Inizio" HeaderText="Data Inizio Impianto" SortExpression="Validita_Inizio" ItemStyle-Width="200px"></asp:BoundField>
                                <asp:BoundField DataField="Validita_Fine" HeaderText="Data Fine Impianto" SortExpression="Validita_Fine" ItemStyle-Width="200px"></asp:BoundField>

                                <asp:BoundField DataField="QtaMaxN" HeaderText="Qta N Consigliata [Kg/Ha]" SortExpression="QtaMaxN" ItemStyle-Width="250px"></asp:BoundField>
                                <%--<asp:BoundField DataField="QtaMaxP2O5" HeaderText="Qta P Max [Kg/Ha]" SortExpression="QtaMaxP2O5"></asp:BoundField>
                                <asp:BoundField DataField="QtaMaxK2O" HeaderText="Qta K Max [Kg/Ha]" SortExpression="QtaMaxK2O"></asp:BoundField>--%>

                            </Columns>
                            <HeaderStyle CssClass="ui-widget-header"/>
                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center"/>
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
