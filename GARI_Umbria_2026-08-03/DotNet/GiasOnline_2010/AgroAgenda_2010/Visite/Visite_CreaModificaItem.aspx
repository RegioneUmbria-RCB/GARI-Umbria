<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Visite_CreaModificaItem.aspx.vb" MasterPageFile="~/Master/AgendaBootstrap_Visite.master" 
    Inherits="AgroAgenda_2010.Visite_CreaModificaItem" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_Visite.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_Visite" runat="server">
<style type="text/css">
    /*Mi serve per dare grafica uniforme ai controlli in sola lettura*/
    .disabled.dropdown-toggle
    {
        background-color: lightgray;
        border: 1px solid #428BCA;
        opacity: 1;
    }

    .k-dropdown-wrap {
        border-color: #428BCA !important;
        border-bottom-left-radius:0px;
        border-top-left-radius:0;
    }
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_Visite" runat="server">

    <!--SEZIONE DOVE SCRIVERE I MESSAGGI D'ERRORE-->
    <div id="DIV_Messaggi"></div>

    <!--VARIABILE IN CUI VIENE SALVATO IL JSON ED IL SUO ID-->
    <asp:HiddenField ID="hfId_Agenda" runat="server"/>

    <!--VARIABILE PER I PERMESSI DI SCRITTURA-->
    <asp:HiddenField ID="hf_UtenteAbilitatoScrittura" runat="server"/>

    <!--SEZIONE CON I CONTROLLI DELLA VISITA-->
    <div ID="PnlDettagli" class="panel" style="margin-bottom: 100px;">

        <div class="row" style="margin-top:10px;">
            <!-- DATA E ORA -->
            <div class="col-lg-6 col-md-6 col-xs-12">
                <div class="input-group">
                    <label class="input-group-addon control-label alert-info" id="CTRL_DataOra" for="txbDataOra">Data e Ora Visita</label>
                    <input type="text" id="txbDataOra" class="form-control" aria-describedby="CTRL_DataOra"/>
                </div>
            </div>
            <!-- UTENTE -->
            <div class="col-lg-6 col-md-6 col-xs-12">
                <div class="input-group">
                    <label class="input-group-addon control-label alert-info" id="CTRL_Utente" for="ddlUtente">Utente</label>
                    <input type="text" id="ddlUtente" class="form-control" aria-describedby="CTRL_Utente"/>
                </div>
            </div>
        </div>

        <div class="row">
            <!-- CATEGORIA 1  -->
            <div class="col-lg-6 col-md-6 col-xs-12">
                <div class="input-group">
                    <label class="input-group-addon control-label alert-info" id="CTRL_Categorie1" for="ddlCategorie1">Categoria 1</label>
                    <input type="text" id="ddlCategorie1" class="form-control ddlkendo" aria-describedby="CTRL_Categorie1"/>
                </div>
            </div>
            <!-- CATEGORIA 2 -->
            <div class="col-lg-6 col-md-6 col-xs-12">
                <div class="input-group">
                    <label class="input-group-addon control-label alert-info" id="CTRL_Categorie2" for="ddlCategorie2">Categoria 2</label>
                    <input type="text" id="ddlCategorie2" class="form-control" aria-describedby="CTRL_Categorie2"/>
                </div>
            </div>
        </div>

        <div class="row">
            <!-- AZIENDA -->
            <div class="col-lg-6 col-md-6 col-xs-12">
                <div class="input-group">
                    <label class="input-group-addon control-label alert-info" id="CTRL_Azienda" for="ddlAzienda">Azienda</label>
                    <input type="text" id="ddlAzienda" class="form-control" aria-describedby="CTRL_Azienda"/> 
                </div>
            </div>
            <!-- CENTRO AZIENDALE -->
            <div class="col-lg-6 col-md-6 col-xs-12">
                <div class="input-group">
                    <label class="input-group-addon control-label alert-info" id="CTRL_Centro" for="ddlCentro">Centro Aziendale</label>
                    <input type="text" id="ddlCentro" class="form-control" aria-describedby="CTRL_Centro"/> 
                </div>
            </div>
        </div>

        <!-- IMPIANTO -->
        <div class="col-lg-12 col-md-12 col-xs-12">
            <div class="input-group">
                <label class="input-group-addon control-label alert-info" id="CTRL_Impianto" for="ddlImpianto">Impianto</label>
                <input type="text" id="ddlImpianto" class="form-control" aria-describedby="CTRL_Impianto"/> 
            </div>
        </div>

        <!-- PUNTO GPS -->
        <div class="row" style="margin-top:10px;">
            <div class="col-lg-12 col-md-12 col-xs-12">
                <div class="input-group">
                    <label class="input-group-addon control-label alert-info" id="CTRL_PuntoGPS" for="txbPuntoGPS">Punto GPS</label>
                    <input type="text" id="txbPuntoGPS" class="form-control" aria-describedby="CTRL_PuntoGPS" />            
                </div>
            </div>
        </div>

        <!-- NOTE -->
        <div class="row" style="margin-top:10px;">
            <div class="col-lg-12 col-md-12 col-xs-12">
                <div class="input-group">
                    <label class="input-group-addon control-label alert-info" id="CTRL_Note" for="txbNote">Note</label>
                    <textarea id="txbNote" class="form-control" aria-describedby="CTRL_Note" rows="2"></textarea>              
                </div>
            </div>
        </div>

        <!--SEZIONE CON I PULSANTI DI SALVATAGGIO-->
        <div class="row" style="background-color:Orange; padding:10px; border-radius:4px;">
            <div class="col-lg-12">
                <button type="button" class="btn btn-success xi-btn-primary pull-right" id="btnSalva" style="margin:0px 5px; display:none;" onclick="salvaVisita();">
                    <span class="fa fa-floppy-o"></span> Salva ed Esci
                </button>
            </div>
        </div>

    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript_Visite" runat="server">

    <script type="text/javascript">

        //INDICO CHE PRIMA DELL'UNLOAD DEVE MOSTRARE UN WAIT FRAME
        window.onbeforeunload = function () { WaitFrame.show(); };

        //DOCUMENT READY
        $(document).ready(function () {
            //Carico la combo delle Categorie1
            ddlCategorie1_Load();

            //CArico la combo con le Aziende
            ddlAzienda_Load();

            $("#txbDataOra").kendoDateTimePicker({ value: new Date(), dateInput: true });

            //Inizializzo il controllo della tipologia (senza dati)
            //$('#ddlTipologia"]').kendoDropDownList({ dataSource: [] });

            //Inizializzo tutti i controlli figli
            //pulisciDdlCategorie();
            //pulisciDdlAccessorie();

            //Se è una scadenza già esistente, la carico
            //var hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
            //if (hfId_Elenco_val != "" && hfId_Elenco_val != "-1") {
            //    caricaScadDaIdElenco(hfId_Elenco_val);
            //}

            //Se l'utente è abilitato aggiungo il pulsante per il salvataggio delle Scadenze
            //if ($("input[id*='hf_UtenteAbilitatoScrittura']").val() == "True") {
            //    $("#btnSalva").show();
            //}

        });

        //DDL CATEGORIE1
        function ddlCategorie1_Load() {

            $('#ddlCategorie1').kendoDropDownList({
                filter: "contains",
                dataSource: {transport: { read: RiempiDdlCategorie1 }},
                dataTextField: "nome",
                dataValueField: "id_categoria1",
                optionLabel: { "nome": "SELEZIONA...", "id_categoria1": "" },
                dataBound: ddlCategorie1_OnDataBound,
                change: ddlCategorie2_Load
            });

        }

        function RiempiDdlCategorie1(options) {

            var parametri = kendo.stringify({ "objP_server": objP_server});

            ajaxAgronica(pathCoreWS + "AgronicaCoreVisite/Visite_Categoria1.asmx/Leggi",
                parametri,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    options.success(risp);
                }, null);
        }

        function ddlCategorie1_OnDataBound(e) {
            var ds = this.dataSource.data();
            if (ds.length == 1) {
                this.select(1); //seleziono l'elemento 
                $('#ddlCategorie1').trigger("change"); //forzo l'evento di onchange
            }
        }

        //DDL CATEGORIE2
        function ddlCategorie2_Load() {

            $('#ddlCategorie2').kendoDropDownList({
                filter: "contains",
                dataSource: { transport: { read: RiempiDdlCategorie2 }},
                dataTextField: "nome",
                dataValueField: "id_categoria2",
                optionLabel: { "nome": "SELEZIONA...", "id_categoria2": "" },
                dataBound: ddlCategoria2_OnDataBound
            });

        }

        function RiempiDdlCategorie2(options) {

            var id_categoria1 = $('#ddlCategorie1').val();
            var parametri = kendo.stringify({ "objP_server": objP_server, "ID_Categoria1": id_categoria1 });

            ajaxAgronicaSync(pathCoreWS + "AgronicaCoreVisite/Visite_Categoria2.asmx/Leggi",
                parametri,
                false,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    options.success(risp);
                }, null);
        }

        function ddlCategoria2_OnDataBound(e) {
            var ds = this.dataSource.data();
            if (ds.length == 1) {
                this.select(1); //seleziono l'elemento 
                this.trigger("change"); //forzo l'evento di onchange
            }
        }

        //DDL AZIENDE
        function ddlAzienda_Load() {

            $('#ddlAzienda').kendoDropDownList({
                filter: "contains",
                dataSource: {transport: { read: RiempiDdlAzienda }},
                dataTextField: "Rag_Soc",
                dataValueField: "Piva",
                optionLabel: { "Rag_Soc": "SELEZIONA...", "Piva": "" },
                dataBound: ddlAzienda_OnDataBound,
                change: ddlCentro_Load
            });

        }

        function RiempiDdlAzienda(options) {

            var testoRicerca = "";
            var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "testoRicerca": testoRicerca });

            ajaxAgronica(pathCoreWS + "Anagrafica/Imprese.asmx/CaricaAzienda_GIS",//LeggiImpreseConFiltroUtente",
                parametri,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    options.success(risp);
                }, null);
        }

        function ddlAzienda_OnDataBound(e) {
            var ds = this.dataSource.data();
            if (ds.length == 1) {
                this.select(1); //seleziono l'elemento 
                this.trigger("change"); //forzo l'evento di onchange
            }
        }

        //DDL CENTRI
        function ddlCentro_Load() {

            $('#ddlCentro').kendoDropDownList({
                filter: "contains",
                dataSource: { transport: { read: RiempiDdlCentro } },
                dataTextField: "Sa_Nome",
                dataValueField: "Piva_Sa_Cod",
                optionLabel: { "Sa_Nome": "SELEZIONA...", "Piva_Sa_Cod": "" },
                dataBound: ddlCentro_OnDataBound,
                change: ddlImpianto_Load
            });

        }

        function RiempiDdlCentro(options) {

            var piva = $('#ddlAzienda').val();
            if (piva === undefined || piva === "") {
                return;
            }

            var parametri = kendo.stringify({ "objP_server": objP_server, "Piva": piva });

            ajaxAgronica(pathCoreWS + "Anagrafica/CentroAziendale.asmx/CaricaCentroAziendale_GIS",
                parametri,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    options.success(risp);
                }, null);
        }

        function ddlCentro_OnDataBound(e) {
            var ds = this.dataSource.data();
            if (ds.length == 1) {
                this.select(1); //seleziono l'elemento 
                this.trigger("change"); //forzo l'evento di onchange
            }
        }

        //DDL IMPIANTI
        function ddlImpianto_Load() {

            $('#ddlImpianto').kendoDropDownList({
                filter: "contains",
                dataSource: { transport: { read: RiempiDdlImpianto } },
                dataTextField: "Reg_Descr",
                dataValueField: "Id_Reg",
                optionLabel: { "Reg_Descr": "SELEZIONA...", "Id_Reg": "" },
                dataBound: ddlImpianto_OnDataBound
            });

        }

        function RiempiDdlImpianto(options) {

            var centro_selezionato  = $('#ddlCentro').val();
            if (centro_selezionato === undefined || centro_selezionato  === "") {
                return;
            }

            var Piva = centro_selezionato.split("|")[0];
            var Sa_Cod = centro_selezionato.split("|")[1];
            var Veg_Cod = "0";
            var isSementi = "false";

            var parametri = kendo.stringify({ "objP_server": objP_server, "Piva": Piva, "Sa_Cod": Sa_Cod, "Veg_Cod": Veg_Cod, "isSementi": isSementi });

            ajaxAgronica(pathCoreWS + "Anagrafica/Reg_Impianto.asmx/CaricaImpiantiEsistenti_GIS",
                parametri,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    options.success(risp);
                }, null);
        }

        function ddlImpianto_OnDataBound(e) {
            var ds = this.dataSource.data();
            if (ds.length == 1) {
                this.select(1); //seleziono l'elemento 
                $('#ddlImpianto').trigger("change"); //forzo l'evento di onchange
            }
        }




        function pulisciDdl() {
            var listaDdl = ["ddlArea", "ddlTipologia"];

            for (var i in listaDdl) {
                $('#' + listaDdl[i]).kendoDropDownList({ dataSource: [] });
            }
        }

        function solaLetturaDdl() {
            var listaDdl = ["ddlAzienda", "ddlArea", "ddlTipologia", "ddlCentro", "ddlAppezzamento", "ddlMacchina", "ddlContatto", "ddlAnalisi", "ddlPianoConcimazione", "ddlPua"];

            for (var i in listaDdl) {
                $('#' + listaDdl[i]).data("kendoDropDownList").enable(false);
            }
        }

        function caricaScadDaIdElenco(id_elenco) {

            var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "id_elenco": id_elenco });

            ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert_Entita.asmx/Leggi_con_documenti",
                parametri,
                false,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);

                    if (risp !== undefined && risp !== "") {
                        var elem = risp[0];

                        $(txbID).val(id_elenco);
                        $('#ddlAzienda').data("kendoDropDownList").value(elem["piva"]);
                        ddlAzienda_Change();
                        $('#ddlArea').data("kendoDropDownList").value(elem["id_area"]);
                        ddlArea_Change();
                        $('#ddlTipologia').data("kendoDropDownList").value(elem["ID_Tipologia"]);
                        ddlTipologia_Change();
                        $('#ddlCentro').data("kendoDropDownList").value(elem["Sa_Cod"]);
                        $('#ddlAppezzamento').data("kendoDropDownList").value(elem["Appezza"]);
                        $('#ddlMacchina').data("kendoDropDownList").value(elem["Mac_Cod"]);
                        $('#ddlContatto').data("kendoDropDownList").value(elem["Cod_Contatto"]);
                        $('#ddlAnalisi').data("kendoDropDownList").value(elem["Analisi_Testata_Cod"]);
                        $('#ddlPianoConcimazione').data("kendoDropDownList").value(elem["PC_Testata_Cod"]);
                        $('#ddlPua').data("kendoDropDownList").value(elem["PUA_Cod"]);

                        $('#txbData').val(elem["Data"]);
                        $('#txbDescrizione').val(elem["Testo"]);
                        $('#txbNote').val(elem["Note"]);

                    }


                }, null);

            //Metto in sola lettura alcuni controlli
            solaLetturaDdl();

        }

        function salvaScadenza() {

            //Estraggo i dati dalla maschera
            var id_area = $('#ddlArea').data("kendoDropDownList").value();
            var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();

            var id_elenco, id_alert_entita;
            if ($('#txbID').val() == undefined || $('#txbID').val() == "") {
                id_elenco = -1;
                id_alert_entita = -1;
            }
            else {
                id_elenco = $('#txbID').val();
                id_alert_entita = $('input[id*="hfId_Alert_Entita"]').val();
            }

            var piva = $('#ddlAzienda').data("kendoDropDownList").value();
            var sa_cod = $('#ddlCentro').data("kendoDropDownList").value();
            var appezza = $('#ddlAppezzamento').data("kendoDropDownList").value();
            var cod_contatto = $('#ddlContatto').data("kendoDropDownList").value();
            var analisi_testata_cod = $('#ddlAnalisi').data("kendoDropDownList").value();
            var pc_testata_cod = $('#ddlPianoConcimazione').data("kendoDropDownList").value();
            var pua_cod = $('#ddlPua').data("kendoDropDownList").value();
            var mac_cod = $('#ddlMacchina').data("kendoDropDownList").value();

            var data = $('#txbData').val();
            var testo = $('#txbDescrizione').val();
            var note = $('#txbNote').val();

            //per ora ignoro l'allegato

            //chiamo il web service
            var strObjJSON = JSON.stringify({ "id_elenco": id_elenco, "id_alert_entita": id_alert_entita, "id_area": id_area, "id_tipologia": id_tipologia, "piva": piva, "sa_cod": sa_cod, "appezza": appezza, "mac_cod": mac_cod, "cod_contatto": cod_contatto, "analisi_testata_cod": analisi_testata_cod, "pc_testata_cod": pc_testata_cod, "pua_cod": pua_cod, "data": data, "testo": testo, "note": note });
            var parametri = JSON.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "strObjJSON": strObjJSON });

            ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Alert.asmx/Scrivi",
                parametri,
                false,
                function (risposta) {
                    alert(risposta.RispostaStringa);

                    //Chiudo la finestra e ricarico le scadenze
                    parent.iFrameGeneric_Chiudi();
                    parent.eseguiRicercaScadenze();

                }, null);

        }


    </script>

</asp:Content>
