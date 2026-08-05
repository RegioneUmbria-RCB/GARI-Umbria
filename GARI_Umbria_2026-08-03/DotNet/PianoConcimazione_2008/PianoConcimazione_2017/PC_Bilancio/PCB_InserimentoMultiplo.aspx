<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master"
    CodeBehind="PCB_InserimentoMultiplo.aspx.vb" Inherits="PianoConcimazione_2017.PCB_InserimentoMultiplo" %>

<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Piano Concimazione a Bilancio</title>
    <script type="text/javascript">

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

        function SelezionaDeselezionaTuttiZVN() {
            if ($('#chkSelezionaTuttiZVN').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaZVN').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaZVN').each(function () {
                    $(this).children('input').removeProp('checked');
                });
            }
        }

        function SelezionaDeselezionaTuttiSerra() {
            if ($('#chkSelezionaTuttiSerra').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaSerra').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                $('.ChkSelezionaSerra').each(function () {
                    $(this).children('input').removeProp('checked');
                });
            }
        }

        function SelezionaDeselezionaSeminaSodo() {
            if ($('#chkSelezionaTuttiSeminaSodo').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaSeminaSodo').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                $('.ChkSelezionaSeminaSodo').each(function () {
                    $(this).children('input').removeProp('checked');
                });
            }
        }
        function SelezionaDeselezionaTuttiIrriguo() {
            if ($('#chkSelezionaTuttiIrriguo').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaIrriguo').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                $('.ChkSelezionaIrriguo').each(function () {
                    $(this).children('input').removeProp('checked');
                });
            }
        }

        function SelezionaDeselezionaTuttiPiani() {
            if ($('#chkSelezionaTuttiPiani').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaPiano').each(function () {
                    $(this).children('input').prop('checked', 'checked');
                });
            }
            else {
                //seleziono tutto
                $('.ChkSelezionaPiano').each(function () {
                    $(this).children('input').removeProp('checked');
                });
            }
        }

        function CopiaResa() {
            var resa = $('#txtResaIntestazione').val();
            $('.txtCopiaResa').val(resa);
        }
        function CopiaAnticipazioni() {
            var anni = $('#txtAnticipazioniIntestazione').val();
            $('.txtCopiaAnticipazioni').val(anni);
        }
        function CopiaFissazione() {
            var fiss = $('#txtFissazioneIntestazione').val();
            $('.txtCopiatxtFissazione').val(fiss);
        }
        function CopiaArea() {
            var area = $('#txtAreaOmogeneaIntestazione').val();
            $('.txtCopiaArea').val(area);
        }
        function CopiaPioggia() {
            var pioggia = $('#txtPrecipitazioniIntestazione').val();
            $('.txtCopiaPrecipitazioni').val(pioggia);
        }
        function CopiaPioggiaFeb() {
            var pioggia = $('#txtPrecipitazioniIntestazioneFeb').val();
            $('.txtCopiaPrecipitazioniFeb').val(pioggia);
        }
        function CopiaNFert() {
            var n_fert = $('#txtQtaNFertIntestazione').val();
            $('.txtCopiaQtaNFert').val(n_fert);
        }

        function AggiornaPiano() {
            $('#<%=BtnAggiornaBilancio.ClientID %>').click();

        }

        function AggiornaBilancio() {
            $('#<%=BtnCalcolaBilancio.ClientID %>').click();

        }

        /////////////////////
        // SCHEDE
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
            var DoseMas = $('#<%=Txt_MAS.ClientId %>').val();
            var DoseRic = 0;
            DoseRic = parseFloat(DoseStd) + parseFloat(TotIncrementi) - parseFloat(TotDecrementi);

            if (parseFloat(DoseRic) < 0) {
                DoseRic = 0;
            } else if (DoseRic > (parseFloat(DoseStd) + parseFloat(MaxIncrementi))) {
                DoseRic = parseFloat(DoseStd) + parseFloat(MaxIncrementi);
            }
            if (parseFloat(DoseMas) != 0) {
                if (parseFloat(DoseRic) > parseFloat(DoseMas)) {
                    DoseRic = parseFloat(DoseMas)
                }
            }

            //aggiorno il calcolato solo se non sono in 1 e 2 anno allevamento
            var fase = 4;
            fase = parseFloat($('#<%=Hidden_Fase_Cod.ClientId %>').val());
            switch (fase) {
                case 1: case 2: case 3:
                    break;
                default:
                    $('#<%=Txt_DoseRicalcolata.ClientId %>').val(DoseRic);
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
            var fase = 4;
            fase = parseFloat($('#<%=Hidden_Fase_Cod.ClientId %>').val());
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
            var fase = 4;
            fase = parseFloat($('#<%=Hidden_Fase_Cod.ClientId %>').val());
            switch (fase) {
                case 1: case 2: case 3:
                    break;
                default:
                    $('#<%=Txt_DoseRicalcolataP.ClientId %>').val(DoseRic);
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
            var fase = 4;
            fase = parseFloat($('#<%=Hidden_Fase_Cod.ClientId %>').val());
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
            var fase = 4;
            fase = parseFloat($('#<%=Hidden_Fase_Cod.ClientId %>').val());
            switch (fase) {
                case 1: case 2: case 3:
                    break;
                default:
                    $('#<%=Txt_DoseRicalcolataK.ClientId %>').val(DoseRic);
            }

        }




        /////////////////////

        $(document).ready(function () {
            docReady();
        });

        async function docReady() {
            $('#chkSelezionaTuttiImpianti').click(function () {
                SelezionaDeselezionaTutti();
            });
            $('#chkSelezionaTuttiSerra').click(function () {
                SelezionaDeselezionaTuttiSerra();
            });
            $('#chkSelezionaTuttiZVN').click(function () {
                SelezionaDeselezionaTuttiZVN();
            });
            $('#chkSelezionaTuttiSeminaSodo').click(function () {
                SelezionaDeselezionaSeminaSodo();
            });
            $('#chkSelezionaTuttiIrriguo').click(function () {
                SelezionaDeselezionaTuttiIrriguo();
            });

            $('#chkSelezionaTuttiPiani').click(function () {
                SelezionaDeselezionaTuttiPiani();
            });


            $(".ddlFasiHeader").change(function () {
                $(".DdlSelezionaFase").val($(this).val());
            });
            $(".ddlUbicazioneHeader").change(function () {
                $(".DdlSelezionaUbicazione").val($(this).val());
            });
            $(".ddlDispOssHeader").change(function () {
                $(".DdlSelezionaDispOss").val($(this).val());
            });
            $(".ddlPrecessioneHeader").change(function () {
                $(".DdlSelezionaPrecessione").val($(this).val());
            });
            $(".ddlFertilizzanteHeader").change(function () {
                $(".DdlSelezionaFertilizzante").val($(this).val());
            });
            $(".ddlFrequenzaHeader").change(function () {
                $(".DdlSelezionaFrequenza").val($(this).val());
            });


            $('#txtResaIntestazione').keyup(function () {
                CopiaResa();
            });
            $('#txtAnticipazioniIntestazione').keyup(function () {
                CopiaAnticipazioni();
            });
            $('#txtFissazioneIntestazione').keyup(function () {
                CopiaFissazione();
            });
            $('#txtAreaOmogeneaIntestazione').keyup(function () {
                CopiaArea();
            });
            $('#txtPrecipitazioniIntestazione').keyup(function () {
                CopiaPioggia();
            });
            $('#txtPrecipitazioniIntestazioneFeb').keyup(function () {
                CopiaPioggiaFeb();
            });
            $('#txtQtaNFertIntestazione').keyup(function () {
                CopiaNFert();
            });


            $('.DatePicker').kendoDatePicker();


            $('#<%=ddlRegolamento.ClientID %>').change(function () {
                AggiornaDescrizione();
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

            let resp = await SportelloPCB();

            if (!resp.Sportello_Aperto) {
                $("#<%=Btn_Salva.ClientID %>").hide();
                kendo.alert("Non è possibile creare il Piano di Concimazione. Sportello chiuso.")
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

        }

        function SportelloPCB() {
            return new Promise((resolve, reject) => {
                var parametri = kendo.stringify({});

                ajaxAgronica("PCB_InserimentoMultiplo.aspx/SportelloPCB_Multiplo",
                    parametri,
                    function (risposta) {
                        resolve(JSON.parse(risposta.RispostaStringa));
                    }, null, null, false);
            });
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">



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
                        <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="form-control "
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
                        <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="form-control "
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


    <div class="row" id="Div_Aggregazione" runat="server">
        <div class="col-lg-12 col-md-12 col-xs-12">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <span class="input-group-addon alert-info">Opzione Aggregazione Appezzamenti</span>
                        <asp:DropDownList ID="CmbAggregazione" runat="server" CssClass="form-control selectpicker required stato_group"
                            data-live-search="true" AutoPostBack="true">
                            <asp:ListItem Value="0">Crea un Piano Concimazione per Azienda, Centro, Specie, Finalità</asp:ListItem>
                            <asp:ListItem Value="1">Crea un Piano Concimazione per Azienda, Centro, Specie, Finalità, Vulnerabilità</asp:ListItem>
                            <asp:ListItem Value="2">Crea un Piano Concimazione per Azienda, Centro, Specie, Finalità, Vulnerabilità, Analisi</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row" style="overflow: scroll; margin-bottom: 10px;">
        <div class="col-lg-12 col-md-12 col-xs-12">
            <asp:GridView ID="grdSpecie" runat="server" CssClass="ui-widget-content" AutoGenerateColumns="False"
                CellPadding="5" CellSpacing="5" Style="overflow: scroll; max-width: 700px">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="a" runat="server">SEL.</asp:Label>
                            <input type="checkbox" id="chkSelezionaTuttiImpianti" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="ChkSeleziona" runat="server" CssClass="ChkSelezionaImpianto" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" CssClass=" text-center" />
                        <ItemStyle Width="20px" HorizontalAlign="Center" />
                        <FooterStyle Width="20px" />
                        <ControlStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Centro" HeaderText="IMPRESA<br>CENTRO" HtmlEncode="False" />
                    <asp:BoundField DataField="Impianti" HeaderText="APPEZZAMENTI" HtmlEncode="False" />

                    <asp:BoundField DataField="strCatasto" HeaderText="CATASTO" SortExpression="Catasto" HtmlEncode="false"></asp:BoundField>

                    <asp:BoundField DataField="Piva">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Sa_Cod">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Veg_Cod">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Veg_Des" HeaderText="SPECIE" HtmlEncode="False" />
                    <asp:TemplateField HeaderText="FINALITA'">
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlFinalita" runat="server" AutoPostBack="false" CssClass="txtUI"
                                Width="200px">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblFaseHeader" runat="server">FASE/CICLO</asp:Label><br>
                            <asp:DropDownList ID="ddlFasiHeader" runat="server" CssClass="txtUI ddlFasiHeader"
                                Width="200px" BackColor="White" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlFasi" runat="server" Width="200px"
                                CssClass="txtUI DdlSelezionaFase">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Resa">
                        <HeaderTemplate>
                            <asp:Label ID="LblResa" runat="server">RESA (t/ha)</asp:Label>
                            <input type="text" style="width: 50px" id="txtResaIntestazione" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="txtResa" runat="server" CssClass="txtCopiaResa txtUI"
                                Width="50px">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblSerra" runat="server">COLTURA PROTETTA</asp:Label>
                            <input type="checkbox" id="chkSelezionaTuttiSerra" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="ChkSerra" runat="server" CssClass="ChkSelezionaSerra" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" CssClass=" text-center" />
                        <ItemStyle Width="20px" HorizontalAlign="Center" />
                        <FooterStyle Width="20px" />
                        <ControlStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Anticipazioni">
                        <HeaderTemplate>
                            <asp:Label ID="LblAnticipazioni" runat="server">ANTICIPAZIONI [Anni]</asp:Label>
                            <input type="text" style="width: 50px" id="txtAnticipazioniIntestazione" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="txtAnticipazioni" runat="server" AutoPostBack="false" CssClass="txtCopiaAnticipazioni txtUI"
                                Width="50px">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="fissazione">
                        <HeaderTemplate>
                            <asp:Label ID="LblFissazione" runat="server">FISSAZIONE [%]</asp:Label>
                            <input type="text" style="width: 50px" id="txtFissazioneIntestazione" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_NFissazione" runat="server" CssClass="txtCopiatxtFissazione txtUI"
                                Width="50px">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblUbicazioneHeader" runat="server">UBICAZIONE</asp:Label><br>
                            <asp:DropDownList ID="ddlUbicazioneHeader" runat="server" CssClass="txtUI ddlUbicazioneHeader"
                                Width="200px" BackColor="White" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlUbicazione" runat="server" Width="200px" CssClass="txtUI DdlSelezionaUbicazione">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblZVN" runat="server">ZVN</asp:Label>
                            <input type="checkbox" id="chkSelezionaTuttiZVN" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="ChkZVN" runat="server" CssClass="ChkSelezionaZVN" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" CssClass=" text-center" />
                        <ItemStyle Width="20px" HorizontalAlign="Center" />
                        <FooterStyle Width="20px" />
                        <ControlStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Area Omogenea">
                        <HeaderTemplate>
                            <asp:Label ID="LblAreaOmogenea" runat="server">AREA OMOGENEA</asp:Label>
                            <input type="text" style="width: 50px" id="txtAreaOmogeneaIntestazione" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="txtAreaOmogenea" runat="server" CssClass="txtCopiaArea txtUI" Width="50px">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Analisi Terreno">
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlAnalisi" runat="server" CssClass="txtUI" Width="200px">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblDispOssHeader" runat="server">DISP. OSSIGENO</asp:Label><br>
                            <asp:DropDownList ID="ddlDispOssHeader" runat="server" CssClass="txtUI ddlDispOssHeader"
                                Width="200px" BackColor="White" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlDispOss" runat="server" Width="200px" CssClass="txtUI DdlSelezionaDispOss">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Precipitazioni Inv">
                        <HeaderTemplate>
                            <asp:Label ID="LblPrecipitazioni" runat="server">PRECIPITAZIONI<br />dal 01/10 al 31/01 [mm]</asp:Label>
                            <input type="text" id="txtPrecipitazioniIntestazione" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_Pioggia" runat="server" CssClass="txtCopiaPrecipitazioni txtUI">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Precipitazioni Feb">
                        <HeaderTemplate>
                            <asp:Label ID="LblPrecipitazioniFeb" runat="server">PRECIPITAZIONI<br />Febbraio [mm]</asp:Label>
                            <input type="text" id="txtPrecipitazioniIntestazioneFeb" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="Txt_Pioggia_Febbraio" runat="server" CssClass="txtCopiaPrecipitazioniFeb txtUI">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblPrecessioneHeader" runat="server">PRECESSIONE</asp:Label><br>
                            <asp:DropDownList ID="ddlPrecessioneHeader" runat="server" CssClass="txtUI ddlPrecessioneHeader"
                                Width="200px" BackColor="White" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlPrecessione" runat="server" Width="200px" CssClass="txtUI DdlSelezionaPrecessione">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblSeminaSodo" runat="server">SEMINA SU SODO</asp:Label>
                            <input type="checkbox" id="chkSelezionaTuttiSeminaSodo" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="ChkSeminaSodo" runat="server" CssClass="ChkSelezionaSeminaSodo" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" CssClass=" text-center" />
                        <ItemStyle Width="20px" HorizontalAlign="Center" />
                        <FooterStyle Width="20px" />
                        <ControlStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblIrriguo" runat="server">IRRIGUO</asp:Label>
                            <input type="checkbox" id="chkSelezionaTuttiIrriguo" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="ChkIrriguo" runat="server" CssClass="ChkSelezionaIrriguo" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" CssClass=" text-center" />
                        <ItemStyle Width="20px" HorizontalAlign="Center" />
                        <FooterStyle Width="20px" />
                        <ControlStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblFertilizzanteHeader" runat="server">FERTILIZZANTE</asp:Label><br>
                            <asp:DropDownList ID="ddlFertilizzanteHeader" runat="server" CssClass="txtUI ddlFertilizzanteHeader"
                                Width="200px" BackColor="White" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlFertilizzante" runat="server" Width="200px" CssClass="txtUI DdlSelezionaFertilizzante">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LblFrequenzaHeader" runat="server">FREQUENZA</asp:Label><br>
                            <asp:DropDownList ID="ddlFrequenzaHeader" runat="server" CssClass="txtUI ddlFrequenzaHeader"
                                BackColor="White" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlFrequenza" runat="server" CssClass="txtUI DdlSelezionaFrequenza">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Qta N [kg/ha]">
                        <HeaderTemplate>
                            <asp:Label ID="LblQtaNFert" runat="server">QTA N [kg/ha]</asp:Label>
                            <input type="text" style="width: 50px;" id="txtQtaNFertIntestazione" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="txtQtaNFert" runat="server" CssClass="txtCopiaQtaNFert txtUI" Width="50px">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle BackColor="#428bca" />
                <PagerStyle HorizontalAlign="Center" />
            </asp:GridView>
        </div>
    </div>

    <div class="row" style="margin-bottom: 10px">
        <div class="col-lg-12 col-md-12 col-xs-12">
            <asp:Button ID="Btn_Bilancio" runat="server" class="btn btn-info btn_100" Text="CALCOLA BILANCIO" />
            <asp:Button ID="Btn_Schede" runat="server" class="btn btn-info btn_100" Text="CALCOLA BILANCIO" />
        </div>


    </div>

    <div class="row" style="margin-bottom: 10px">
        <div class="col-lg-12 col-md-12 col-xs-12">


            <asp:GridView ID="GridViewBilanci" runat="server" CssClass="ui-widget-content" AutoGenerateColumns="False"
                CellPadding="5" CellSpacing="5">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="a" runat="server">Sel.</asp:Label>
                            <input type="checkbox" id="chkSelezionaTuttiPiani" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="ChkSelezionaPiano" runat="server" CssClass="ChkSelezionaPiano" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" HorizontalAlign="Center" />
                        <ItemStyle Width="20px" HorizontalAlign="Center" />
                        <FooterStyle Width="20px" />
                        <ControlStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Centro" HeaderText="Impresa" HtmlEncode="False" />
                    <asp:BoundField DataField="Impianti" HeaderText="Impianti" HtmlEncode="False" />
                    <asp:BoundField DataField="Piva">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Sa_Cod">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Veg_Cod">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Veg_Des" HeaderText="Specie" HtmlEncode="False" />
                    <asp:BoundField DataField="Grfi_Cod">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Grfi_Des" HeaderText="Finalità" HtmlEncode="False" />                                                         

                    <asp:BoundField DataField="N_Ammesso" HeaderText="N Ammesso [kg/ha]" HtmlEncode="False" />
                    <asp:BoundField DataField="P_Ammesso" HeaderText="P Ammesso [kg/ha]" HtmlEncode="False" />
                    <asp:BoundField DataField="K_Ammesso" HeaderText="K Ammesso [kg/ha]" HtmlEncode="False" />

                    <asp:ButtonField ButtonType="Link" HeaderText="Dettaglio" Text="Dettaglio"
                        CommandName="Dettaglio">
                        <HeaderStyle Font-Names="Verdana" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </asp:ButtonField>

                     <asp:TemplateField HeaderText="N">
                        <HeaderTemplate>
                            <asp:Label ID="Lbl_N_Ammesso" runat="server">N da assegnare agli appezzamenti [kg/ha]</asp:Label>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="txtN_Ammesso" runat="server" CssClass="txtUI"
                                Width="50px">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="P">
                        <HeaderTemplate>
                            <asp:Label ID="Lbl_P_Ammesso" runat="server">P_Ammesso da assegnare agli appezzamenti [kg/ha]</asp:Label>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="txtP_Ammesso" runat="server" CssClass="txtUI"
                                Width="50px">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="K">
                        <HeaderTemplate>
                            <asp:Label ID="Lbl_K_Ammesso" runat="server">K Ammesso da assegnare agli appezzamenti [kg/ha]</asp:Label>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:TextBox ID="txtK_Ammesso" runat="server" CssClass="txtUI"
                                Width="50px">
                            </asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>


                </Columns>
                <HeaderStyle CssClass="ui-widget-header" />
                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
            </asp:GridView>

        </div>
    </div>

    <div class="row" style="margin-bottom: 80px">
        <div class="col-lg-12 col-md-12 col-xs-12  ">
            <asp:Button ID="Btn_Salva" runat="server" class="btn btn-success btn_100" Text="SALVA PIANI CONCIMAZIONE E ASSOCIALI AGLI IMPIANTI" />
        </div>
    </div>


    <!--Dialog dettaglio-->
    <div id="dialogDettaglioBilancio" class="modal fade">
        <div class="modal-dialog" style="width: min-content;">
            <div class="modal-content" style="min-width: 1000px;">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        <span aria-hidden="true">&times;</span><span class="sr-only">Chiudi</span></button>
                    <h4 class="modal-title">Dettagli Bilancio</h4>
                </div>
                <div class="modal-body">

                    <p>

                        <div class="row">

                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h4 class="panel-title">
                                        <b>CARATTERISTICHE SUOLO</b>
                                    </h4>
                                </div>
                                <div>
                                    <div class="panel-body">


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
                                                                <span class="input-group-addon alert-info">PH</span>
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
                                                                <span class="input-group-addon alert-info">P2O5 [ppm]</span>
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
                                                                <span class="input-group-addon alert-info">K2O [ppm]</span>
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
                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info">C.S.C. [meq/100 g]</span>
                                                                <asp:TextBox ID="Txt_CSC" runat="server" CssClass="form-control">
                                                                </asp:TextBox>

                                                                <asp:Button ID="BtnAggiornaBilancio" runat="server" Style="display: none;" />
                                                                <asp:Button ID="BtnCalcolaBilancio" runat="server" Style="display: none;" />


                                                                <input type="hidden" runat="server" id="IndiceBilancio" />

                                                                <input type="hidden" id="Hidden_Veg_Cod" runat="server" />
                                                                <input type="hidden" id="Hidden_Grfi_Cod" runat="server" />
                                                                <input type="hidden" id="Hidden_Fase_Cod" runat="server" />
                                                                <input type="hidden" id="Hidden_Resa" runat="server" />
                                                                <input type="hidden" id="Hidden_AnticipazioniAnni" runat="server" />
                                                                <input type="hidden" id="Hidden_Copertura" runat="server" />
                                                                <input type="hidden" id="Hidden_UbicazioneCod" runat="server" />
                                                                <input type="hidden" id="Hidden_PercNFissazione" runat="server" />
                                                                <input type="hidden" id="Hidden_DispOssigeno" runat="server" />
                                                                <input type="hidden" id="Hidden_PiovositaFeb" runat="server" />
                                                                <input type="hidden" id="Hidden_Piovosita" runat="server" />
                                                                <input type="hidden" id="Hidden_Precessione" runat="server" />
                                                                <input type="hidden" id="Hidden_TipoFertilizzante" runat="server" />
                                                                <input type="hidden" id="Hidden_IdFrequenza" runat="server" />
                                                                <input type="hidden" id="Hidden_QtaN_FerPrec" runat="server" />

                                                                <input type="hidden" id="Hidden_Fattori" runat="server" />

                                                                <%--                                                                <input type="hidden" id="N_Ammesso" runat="server" />
                                                                <input type="hidden" id="K_Ammesso" runat="server" />
                                                                <input type="hidden" id="P_Ammesso" runat="server" />--%>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="panel panel-primary" id="Div_Bilancio" runat="server">
                                <div class="panel-heading">
                                    <h4 class="panel-title">
                                        <b>BILANCIO</b>
                                    </h4>
                                </div>
                                <div>
                                    <div class="panel-body">
                                        <div class="row">
                                            <asp:GridView ID="grdNecessita" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True">
                                                <Columns>
                                                    <asp:BoundField DataField="Indice" HeaderText="" Visible="false" />
                                                    <asp:BoundField DataField="Descrizione" HeaderText="Necessita'" />
                                                    <asp:BoundField DataField="N" HeaderText="N" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                                    <asp:BoundField DataField="P" HeaderText="P2O5" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                                    <asp:BoundField DataField="K" HeaderText="K2O" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                                </Columns>
                                                <HeaderStyle CssClass="ui-widget-header" />
                                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                            </asp:GridView>
                                        </div>
                                        <div class="row">
                                            <asp:GridView ID="GrdDisponibilita" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True">
                                                <Columns>
                                                    <asp:BoundField DataField="Indice" HeaderText="" Visible="false" />
                                                    <asp:BoundField DataField="Descrizione" HeaderText="Disponibilita'" />
                                                    <asp:BoundField DataField="N" HeaderText="N" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                                    <asp:BoundField DataField="P" HeaderText="P2O5" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                                    <asp:BoundField DataField="K" HeaderText="K2O" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                                </Columns>
                                                <HeaderStyle CssClass="ui-widget-header" />
                                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                            </asp:GridView>
                                        </div>

                                        <div class="row" style="margin-top: 10px">
                                            <div>
                                                <asp:Label ID="LblAttenzione" runat="server" CssClass="txtUI" ForeColor="Red"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="panel panel-primary" id="Div_Schede" runat="server">
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
                                                                            <asp:Label ID="Label16" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                                                Text="Limite MAS :" Width="135px" Height="20px"></asp:Label>
                                                                            <asp:TextBox ID="Txt_MAS" runat="server" CssClass="txtui" MaxLength="250" Style="margin-left: 5px; pointer-events: none"
                                                                                ToolTip="La dose non può superare il MAS" Width="60px"></asp:TextBox>
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

                        </div>


                    </p>



                    <div class="modal-footer">

                        <button type="button" class="btn btn-default" data-dismiss="modal">
                            ANNULLA</button>
                        <button type="button" class="btn btn-primary" onclick="AggiornaBilancio(); ">
                            RICALCOLA BILANCIO</button>
                        <button type="button" class="btn btn-primary" onclick="AggiornaPiano();">
                            AGGIORNA BILANCIO</button>

                    </div>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>






</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
</asp:Content>
