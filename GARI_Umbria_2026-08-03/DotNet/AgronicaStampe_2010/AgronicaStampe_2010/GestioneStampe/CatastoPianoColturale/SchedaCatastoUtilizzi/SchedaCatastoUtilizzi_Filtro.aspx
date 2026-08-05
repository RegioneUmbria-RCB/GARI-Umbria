<%@ Page Title="Scheda Catasto Utilizzi" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master"
    CodeBehind="SchedaCatastoUtilizzi_Filtro.aspx.vb" Inherits="AgronicaStampe_2010.SchedaCatastoUtilizzi_Filtro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%--    <link rel="stylesheet" type="text/css" href="BrogliaccioMovimentiTabella.css?<% =Application("GiasVersioneCorrente")%>" />--%>
    <script type="text/javascript">

        //DOCUMENT READY
        $(document).ready(function () {

            $('#Txt_Data').kendoDatePicker();
            $('#Txt_DataInizio').kendoDatePicker();
            $('#Txt_DataFine').kendoDatePicker();

            $('input[name=finestraTemporale]').change(RblChange);

            RblChange();

        });

        function RblChange() {

            var oggi = new Date();
            oggi.setHours(0, 0, 0, 0);

            var year = oggi.getFullYear();


            var selValue = $('input[name=finestraTemporale]:checked').val();
            switch (selValue) {
                case "0": //data
                    $('#DataDiv').show();
                    $('#TemporaleDiv').hide();
                    $('#Txt_Data').val(kendo.toString(oggi, "dd/MM/yyyy"));
                    $('#Txt_DataInizio').val('');
                    $('#Txt_DataFine').val('');

                    break;
                case "1": //intervallo
                    $('#TemporaleDiv').show();
                    $('#DataDiv').hide();
                    $('#Txt_DataInizio').val('01/01/' + year);
                    $('#Txt_DataFine').val('31/12/' + year);

                    break;
            }
        };


        function Stampa() {

            var dataDa = $('#Txt_DataInizio').val();
            var dataA = $('#Txt_DataFine').val();
            var data = $('#Txt_Data').val();

            var chiamaFunzione = false;
            switch ($('input[name=finestraTemporale]:checked').val()) {
                case "0":
                    //data
                    dataDa = "";
                    dataA = "";

                    dataValida = false;
                    if (data !== "") {
                        dataValida = isValidDate(data);
                    }

                    if (!dataValida) {
                        //MessaggioErrore_Bootstrap("Da data movimento non valida", "DIV_Messaggi");
                        alert('Impostare una data valida');
                        chiamaFunzione = false;
                    }
                    else {
                        chiamaFunzione = true;
                        dataDa = data;
                        dataA = data;
                    }

                    break;

                /////////////////////////////////////////////////////       

                case "1":
                    //intervallo


                    dataValida = false;
                    if (dataDa !== "") {
                        dataValida = isValidDate(dataDa);
                    }
                    if (!dataValida) {
                        //MessaggioErrore_Bootstrap("Da data movimento non valida", "DIV_Messaggi");
                        alert("Impostare la data inizio dell'intervallo");
                        chiamaFunzione = false;
                    }
                    else {

                        dataValida = false;
                        if (dataA !== "") {
                            dataValida = isValidDate(dataA);
                        }

                        if (!dataValida) {
                            //MessaggioErrore_Bootstrap("Da data movimento non valida", "DIV_Messaggi");
                            alert("Impostare la data fine dell'intervallo");
                            chiamaFunzione = false;
                        }
                        else {
                            chiamaFunzione = true;
                        }


                    }

                    //                      if (dataDa == "" || dataA == "") {
                    //                          alert('Impostare Data Inizio e Data Fine')
                    //                          chiamaFunzione = false;
                    //                      } else {
                    //                          chiamaFunzione = true;
                    //                      }




                    break;
            }
            if (chiamaFunzione) {

                //////////////////////////////////////////////////////////
                //      WS CLIENT
                //////////////////////////////////////////////////////////
                var indirizzohttp = "./SchedaCatastoUtilizzi_Filtro.aspx";

                var param = "{ piva: '" + $(cIdPiva).val() +
                                "', dataDa: '" + dataDa +
                                "', dataA: '" + dataA +
                                "' }";

                var risp = "";
                ajaxAgronicaSync(indirizzohttp + "/Stampa",
                param,
                false,
                function (risposta) {
                    window.location = risposta.RispostaStringa;
                }, null);


            }
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron" style="padding-left: 15px; padding-right: 15px; margin-bottom: 75px;">
        <div class="row w100">
            <div class="col-lg-4">
                <h4>
                    <span>Filtro Temporale</span></h4>
            </div>
            <div class="col-lg-8">
                <div class="row">
                    <div class="col-lg-8">
                        <form>
                        <input type="radio" name="finestraTemporale" value="0" />Data<br />
                        <input type="radio" name="finestraTemporale" value="1" checked="checked" />Intervallo
                        Temporale<br />
                        </form>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-8">
                        <div id="DataDiv">
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" for="data_inizio">
                                    Data</label>
                                <input type="text" id="Txt_Data" class="form-control" />
                            </div>
                        </div>
                        <div id="TemporaleDiv">
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" for="data_inizio">
                                    Data Inizio</label>
                                <input type="text" id="Txt_DataInizio" class="form-control" />
                            </div>
                            <div class="input-group">
                                <label class="input-group-addon control-label alert-info" for="data_inizio">
                                    Data Fine</label>
                                <input type="text" id="Txt_DataFine" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="row w100">
            <div class="col-lg-2">
                <div class="btn btn-success" onclick="Stampa();">
                    <i class="fa fa-search"></i>Stampa</div>
            </div>
        </div>
        <br />
    </div>
    <input type="hidden" id="hdPiva" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
    </script>
</asp:Content>
