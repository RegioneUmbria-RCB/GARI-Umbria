<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master" CodeBehind="PUA_Letamazioni_Precedenti.aspx.vb" Inherits="PianoConcimazione_2017.PUA_Letamazioni_Precedenti" %>

<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .allineadestra {
            text-align: right !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <div class="panel panel-primary">
        <div class="panel-heading">
            <h4 class="panel-title">
                <b>LETAMAZIONI PRESENTI SU REGISTRO FERTILIZZAZIONI</b>
            </h4>
        </div>
        <div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-lg-12 nopadding">                      
                            <div id="kendo_LetamazioniPrecedentiQdC"></div>
                            <input type="hidden" id="HD_LetamazioniPrecedentiQdC" name="HD_LetamazioniPrecedentiQdC" runat="server" />               
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="panel panel-primary">
        <div class="panel-heading">
            <h4 class="panel-title">
                <b>DISPONIBILITA DI AZOTO DERIVANTE DA LETAMAZIONI PRECEDENTI (valore conteggiato nel PUA)</b>
            </h4>
        </div>
        <div>
            <div class="panel-body">

                <div class="row">
                    <div class="col-lg-12 nopadding">                      
                            <div id="kendo_LetamazioniPrecedenti"></div>
                            <input type="hidden" id="HD_LetamazioniPrecedenti" name="HD_LetamazioniPrecedenti" runat="server" />               
                    </div>
                </div>
                <div class="row" style="display:none">
                    <div class="col-lg-8 col-md-8 col-xs-12" >
                    </div>
                    <div class="col-lg-4 col-md-4 col-xs-12" style="float:right"    >
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group ">
                                    <span class="input-group-addon alert-info text-right btn_100">Disponibilità Azoto residua [Kg/ha]</span>
                                    <input type="text"  id="Txt_Azoto" readonly="readonly" />
                                </div>
                            </div>                        

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>




    <div class="panel panel-primary">
        <div class="panel-heading">
            <h4 class="panel-title">
                <b></b>
            </h4>
        </div>
        <div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-xs-12 nopadding">
                        <div class='btn btn-success btn_100' onclick='Salva();'><span class="fa fa-save"></span>SALVA</div>
                    </div>
                </div>
            </div>
        </div>
    </div>






    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdSaCod" runat="server" />
    <input type="hidden" id="hdAppezza" runat="server" />
    <input type="hidden" id="hdIdReg" runat="server" />
    <input type="hidden" id="hdProgettoCod" runat="server" />
    <input type="hidden" id="hdPuaCod" runat="server" />
    <input type="hidden" id="hdRegCod" runat="server" />
    <input type="hidden" id="hdIdAnagrafeVincoli" runat="server" />
       <input type="hidden" id="hdIdDataInizio" runat="server" />
       <input type="hidden" id="hdIdDataFine" runat="server" />

    <iframe id="iframe" style="display: none;"></iframe>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script src="PUA_Letamazioni_Precedenti.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="PUA_Letamazioni_Precedenti_jQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="PUA_Letamazioni_Precedenti_ws_client.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

    <script type="text/javascript">

        var id_HD_LetamazioniPrecedenti = "<%= HD_LetamazioniPrecedenti.ClientID%>";
        var id_HD_LetamazioniPrecedentiQdC = "<%= HD_LetamazioniPrecedentiQdC.ClientID%>";

        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdSaCod = "#<%=hdSaCod.ClientID() %>";
        var cIdAppezza = "#<%=hdAppezza.ClientID() %>";
        var cIdIdReg = "#<%=hdIdReg.ClientID() %>";
        var cIdProgettoCod = "#<%=hdProgettoCod.ClientID() %>";
        var cIdPuaCod = "#<%=hdPuaCod.ClientID() %>";
        var cIdRegCod = "#<%=hdRegCod.ClientID() %>";
        var cIdAnagrafeVincoli ="#<%=hdIdAnagrafeVincoli.ClientID() %>";
        var cIdDataInizio = "#<%=hdIdDataInizio.ClientID() %>";
        var cIdDataFine = "#<%=hdIdDataFine.ClientID() %>";


    </script>

</asp:Content>
