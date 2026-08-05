<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap_Visite.Master" 
        CodeBehind="Visite_ListaDettagli.aspx.vb" Inherits="AgroAgenda_2010.Visite_ListaDettagli" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_Visite.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_Visite" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_Visite" runat="server">

    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoCreazioneVisita" />

    <!--PULSANTE PER LA CREAZIONE DI UNA NUOVA VISITA-->
    <div class="btn btn-warning" id="BtnVisita_Add" style="display:none" onclick="nuovaVisita();">
        <span class="fa fa-file-o"> Crea Visita</span>
    </div>

    <!--WATABLE CON LE VISITE-->
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 125px;">
        <div id="tabella_visiteDettagli"></div>
    </div>

    <input type="hidden" id="hdKendo_Valorizzazione"/>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript_Visite" runat="server">

 <script type="text/javascript">

     $(document).ready(function () {

         <% If Master.pivaAziendaSelezionataClientSide <> "" Then %>
         //Se l'utente è abilitato aggiungo il pulsante per l'inserimento di nuove Scadenze
         if ($("input[name$='hf_UtenteAbilitatoCreazioneVisita']").val() == "True") {
             $("#BtnVisita_Add").show();
         }
         <% End If %>

         //Mostra tutte le Visite
         eseguiRicercaVisiteDettagli();

         //INFO DELL'ELEMENTO
         $("body").on("click", ".info_elem", function () {
             var tr_elem = $(this).closest("tr");
             var datiGriglia = $('#tabella_visiteDettagli').data('kendoGrid');
             var piva = datiGriglia.dataItem(tr_elem).Piva;
             var id_agenda = datiGriglia.dataItem(tr_elem).Id_Agenda;
             var lav_cod = datiGriglia.dataItem(tr_elem).Lav_Cod;
             var type = 0; // 0 lettura - 2 modifica

             var param = kendo.stringify({ 'type': type, 'id_agenda': id_agenda, 'piva': piva, 'lav_cod': lav_cod });

             ajaxAgronica("Visite_Lista.aspx/infomodifica_operazione_singola", param,
                 function (risposta) {
                     window.location = risposta.RispostaStringa;
                 }, null);
         }); 

         //MODIFICA DELL'ELEMENTO
         $("body").on("click", ".edit_elem", function () {
             var tr_elem = $(this).closest("tr");
             var datiGriglia = $('#tabella_visiteDettagli').data('kendoGrid');
             var piva = datiGriglia.dataItem(tr_elem).Piva;
             var id_agenda = datiGriglia.dataItem(tr_elem).Id_Agenda;
             var lav_cod = datiGriglia.dataItem(tr_elem).Lav_Cod;
             var type = 2; // 0 lettura - 2 modifica

             var param = kendo.stringify({ 'type': type, 'id_agenda': id_agenda, 'piva': piva, 'lav_cod': lav_cod });

             ajaxAgronica("Visite_Lista.aspx/infomodifica_operazione_singola", param,
                 function (risposta) {
                     window.location = risposta.RispostaStringa;
                 }, null);
         }); 

         //CANCELLAZIONE DELL'ELEMENTO
         $("body").on("click", ".del_elem", function () {
             if (confirm("L'Intera visita verrà cancellata definitivamente. Vuoi procedere?")) {
                 var tr_elem = $(this).closest("tr");
                 var datiGriglia = $('#tabella_scadenze').data('kendoGrid');
                 var id_elenco = datiGriglia.dataItem(tr_elem).ID_Elenco;
                 var param = kendo.stringify({ 'objP_server': objP_server, 'id_elenco': id_elenco });
                 ajaxAgronica(pathCoreWS + "AgronicaCoreVisite/Visite.asmx/CancellaXXXXXXXXXXXXXX", param,
                     function (risposta) {
                         MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

                         //Ricarico le NC
                         eseguiRicercaVisiteDettagli();
                     }, null);

             }

         });

     });

     //FUNZIONE PER CREARE UNA NUOVA VISITA
     function nuovaVisita() {

         ajaxAgronica("Visite_Lista.aspx/nuova_operazione", null,
             function (risposta) {
                 window.location = risposta.RispostaStringa;
             }, null);
     }

     //FUNZIONE CHE MOSTRA LA TABELLA
     function eseguiRicercaVisiteDettagli() {
         var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti });

         ajaxAgronica(pathCoreWS + "AgronicaCoreVisite/Visite.asmx/Leggi_ListaVisiteDettagli_ToKendoGrid", param,
             function (risposta) {

                 $('#hdKendo_Valorizzazione').val(risposta.RispostaStringa);
                 popolaGrigliaVisite("tabella_visiteDettagli");

             }, null);
     }

     function apriFormDialog(url) {
         $("#PaginaGeneric").attr("src", url);
         $("#iFrameGeneric").modal('toggle');
     }

     function chiudidialog() {
         $('#dialog').modal('hide');
     }


     //-----------------------------------------------------------------------------------------------------------------------------------
     //KENDO
     //-----------------------------------------------------------------------------------------------------------------------------------

     function popolaGrigliaVisite(IDControllo) {

         var funzioniCRUD = { funzioneRead: kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
         var idModel = "Id_Agenda";
         var campiKendoModel = kReadValorizzazione_mod();
         var colonneKendoGrid = kReadValorizzazione_col();
         var parametriPerLettura = null;
         var parametriDataSource = {
             sort: [{ field: "Data_Movimento", dir: "desc" }, { field: "Ora_Movimento", dir: "desc" }]
         };
         var parametriKendoGrid = {
             columnMenu: true,
             pdf: false,
             groupable: false,
             salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
             pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
             //sortable: {mode: "multiple", allowUnsort: true, showIndexes: true},
             filterable: { mode: "menu" },
             scrollable: false
         };
         var funzioniPrimaDopoEventi = {};
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

         //$('#' + IDControllo + ' th input:text').css("min-width", "100px");
         //$('#' + IDControllo).data("kendoGrid").refresh();

     }

     function kReadValorizzazione_rows(options) {

         var data = $('#hdKendo_Valorizzazione').val();
         jSonParsed_Kendo = JSON.parse(data);

         options.success(jSonParsed_Kendo.kendo_rows);
     }

     function kReadValorizzazione_col() {

         var data = $('#hdKendo_Valorizzazione').val();
         jSonParsed_Kendo = JSON.parse(data);

         return jSonParsed_Kendo.kendo_columns;
     }

     function kReadValorizzazione_mod() {

         var data = $('#hdKendo_Valorizzazione').val();
         jSonParsed_Kendo = JSON.parse(data);

         return jSonParsed_Kendo.kendo_model;
     }

 </script>

</asp:Content>
