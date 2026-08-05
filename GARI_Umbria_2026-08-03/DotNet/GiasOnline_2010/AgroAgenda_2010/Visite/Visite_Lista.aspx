<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap_Visite.Master" 
        CodeBehind="Visite_Lista.aspx.vb" Inherits="AgroAgenda_2010.Visite_Lista" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_Visite.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_Visite" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_Visite" runat="server">

    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoCreazioneVisita" />
    <asp:HiddenField runat="server" ID="hf_AbilitazioneVisiteAPP" />

    <!--PULSANTE PER LA CREAZIONE DI UNA NUOVA VISITA-->
    <div class="btn btn-warning" id="BtnVisita_Add" style="display:none" onclick="nuovaVisita();">
        <span class="fa fa-file-o"></span>Crea Visita
    </div>

    <!--PULSANTE PER L'IMPORTAZIONE VISITE APP-->
    <div class="btn btn-warning" id="BtnImportaVisiteAPP" style="display:none" onclick="<%= If(SincroDatiApp, "CaricaVisiteAPP", "ImportaVisiteAPP") %>();">
        <span class="fa fa-download"></span>Carica dati APP
    </div>

    <!--WATABLE CON LE VISITE-->
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 125px;">
        <div id="tabella_visite"></div>
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

         // se visite app attive e utente abilitato mostro pulsante importazione visite app
         if ($("input[name$='hf_UtenteAbilitatoCreazioneVisita']").val() == "True" && $("input[name$='hf_AbilitazioneVisiteAPP']").val() == "1") {
             $("#BtnImportaVisiteAPP").show();
         }

         //Mostra tutte le Visite
         eseguiRicercaVisite();

         //INFO DELL'ELEMENTO
         $("body").on("click", ".info_elem", function () {
             var tr_elem = $(this).closest("tr");
             var datiGriglia = $('#tabella_visite').data('kendoGrid');
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
             var datiGriglia = $('#tabella_visite').data('kendoGrid');
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
             //if (confirm('La Visita verrà cancellata definitivamente. Vuoi procedere?')) {
                 var tr_elem = $(this).closest("tr");
                 var datiGriglia = $('#tabella_visite').data('kendoGrid');
                 //var id_elenco = datiGriglia.dataItem(tr_elem).ID_Elenco;
                 //var param = kendo.stringify({ 'objP_server': objP_server, 'id_elenco': id_elenco });
                 //ajaxAgronica(pathCoreWS + "AgronicaCoreVisite/Visite.asmx/CancellaXXXXXXXXXXXXXX", param,
                 //    function (risposta) {
                 //        MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");

                 //        //Ricarico le NC
                 //        eseguiRicercaVisite();
                 //    }, null);

                 eliminaElemento(tr_elem, '#tabella_visite');

             //}

         });




         //ALLEGA NUOVO DOCUMENTO
         $("body").on("click", ".add_doc", function () {
             var tr_elem = $(this).closest("tr");
             var grid_elem = $(this).closest(".k-grid");

             var dataItem = GetIdAgendaInElem(tr_elem, grid_elem);

             var ID_Alert_Entita = -1;
             var ID_Elenco = -1;
             var Modalita = "doc";

             var Id_Area = 11;
             var Tipologia;
             var Piva = (typeof dataItem.Piva === 'undefined') ? dataItem.piva : dataItem.Piva;
             var Id_Agenda = (typeof dataItem.Id_Agenda === 'undefined') ? 0 : dataItem.Id_Agenda;
             var Ricetta_Operazione_Cod = (typeof dataItem.Ricetta_Operazione_Cod === 'undefined') ? 0 : dataItem.Ricetta_Operazione_Cod;

             if (Tipologia == undefined || Tipologia == null || Tipologia == "") {
                 switch (parseInt(dataItem.Lav_Cod)) {
                     case 2004: //Ordine Acquisto           
                         Tipologia = -18;
                         break;
                     case 1025: //DDT Ricevuto              
                         Tipologia = -19;
                         break;
                     case 1054:
                     case 1076:
                     case 1078: //Conferimento    
                         Tipologia = -20;
                         break;
                     case 1031: //DDT Emesso                
                         Tipologia = -21;
                         break;
                     case 2002: //Ordine Vendita            
                         Tipologia = -22;
                         break;
                     case 1000: //Ordine Vendita
                         Tipologia = -23;
                         break;
                     case 1001: //Fattura emessa            
                         Tipologia = -24;
                         break;
                 }

                 Tipologia = (typeof Tipologia === 'undefined') ? 0 : Tipologia;

                 if (Tipologia != undefined && Tipologia != null && Tipologia != "") {
                     Id_Area = 10;
                 }
             }



             var param = kendo.stringify({
                 'Piva': Piva, 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'Id_Area': Id_Area, 'Tipologia': Tipologia, 'area_provenienza': Id_Area, 'Id_Agenda': Id_Agenda, 'Ricetta_Operazione_Cod': Ricetta_Operazione_Cod
             });
             //var param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 1, 'ID_Alert_Entita': ID_Alert_Entita });
             var url = "../Scadenzario/Scad_CreaModificaItem.aspx?scadstr=" + param + "&type=" + Modalita + "&p=" + dataItem.Piva;

             $(document.body).append('<div id="nuovo_documentale"></div>');
             $('#nuovo_documentale').kendoWindow({
                 title: "Nuovo Documento",
                 modal: true,
                 resizable: true,
                 iframe: true,
                 width: "80%",
                 height: "80%",
                 content: url,
                 actions: ["Maximize", "Close"],
                 close: function () {
                     $('#nuovo_documentale').kendoWindow('destroy');
                 }
             }).data('kendoWindow').center().maximize();
         });



         //GESTIONE DOCUMENTI
         $("body").on("click", ".visible_doc", function () {
             var tr_elem = $(this).closest("tr");
             var grid_elem = $(this).closest(".k-grid");

             var dataItem = GetIdAgendaInElem(tr_elem, grid_elem);

             var Id_Area = 11;
             var Ricetta_Operazione_Cod = (typeof dataItem.Ricetta_Operazione_Cod === 'undefined') ? 0 : dataItem.Ricetta_Operazione_Cod;
             var Id_Agenda = (typeof dataItem.Id_Agenda === 'undefined') ? 0 : dataItem.Id_Agenda;
             var Piva = (typeof dataItem.Piva === 'undefined') ? dataItem.piva : dataItem.Piva;

             switch (parseInt(dataItem.Lav_cod)) {
                 case 2004: //Ordine Acquisto           
                 case 1025: //DDT Ricevuto              
                 case 1054:
                 case 1076:
                 case 1078: //Conferimento    
                 case 1031: //DDT Emesso                
                 case 2002: //Ordine Vendita            
                 case 1000: //Ordine Vendita
                 case 1001: //Fattura emessa            
                     Id_Area = 10;
                     break;
             }

             var url = "../Scadenzario/Scad_lista.aspx?type=doc" + "&area_provenienza=" + Id_Area + "&p=" + Piva + "&id_agenda=" + Id_Agenda + "&Ricetta_Operazione_Cod=" + Ricetta_Operazione_Cod;

             $(document.body).append('<div id="ricerca_documentale"></div>');
             $('#ricerca_documentale').kendoWindow({
                 title: "Ricerca Documenti",
                 modal: true,
                 resizable: true,
                 iframe: true,
                 width: "80%",
                 height: "80%",
                 content: url,
                 actions: ["Maximize", "Close"],
                 close: function () {
                     $('#ricerca_documentale').kendoWindow('destroy');
                 }
             }).data('kendoWindow').center().maximize();
         });

     });

     function GetIdAgendaInElem(tr_elem, grid_elem) {
         var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
         return dataItem;
     }

     //FUNZIONE PER CREARE UNA NUOVA VISITA
     function nuovaVisita() {

         ajaxAgronica("Visite_Lista.aspx/nuova_operazione", null,
             function (risposta) {
                 window.location = risposta.RispostaStringa;
             }, null);
     }

     function eliminaElemento(tr_elem, grid_elem) {

         var datiGriglia = $(grid_elem).data('kendoGrid');
         var datiRiga = datiGriglia.dataItem(tr_elem);
         var chiavi_composite_checked = kendo.toString(datiRiga.Data_Movimento, "dd/MM/yyyy hh:mm:ss") + "_" + datiRiga.Id_Agenda + "_" + datiRiga.Lav_Cod + "_" + "0" + "_" + "0" + "_" + "0" + "_" + "0" + "_" + datiRiga.Piva;

         //Mostro l'alert e nel caso cancello...
         ConfermaControlliSiNo('Sei sicuro di voler eliminare la visita selezionata?', 'del_elem|' + chiavi_composite_checked);

     }

     //FUNZIONE PER LA CANCELLAZIONE DELL'OPERAZIONE IN 2 PASSAGGI
     function DoPostBack_ControlliSiNo(str) {

         if (str.startsWith('del_elem')) {

             //SalvaParametriDiv('#frmInput', false);

             ajaxAgronica("../Menu/MenuBS_Agenda_Nuovo.aspx/elimina_operazione_multipla", JSON.stringify({ strChiaviComposite: str, proseguiInCasoDiAlert: false }),
                 function (risposta) {

                     if (risposta.RispostaOK) {
                         ScritturaOK(risposta.RispostaStringa);
                         eseguiRicercaVisite();// Aggiorno la tabella
                     }
                     else {
                         //non cancellabile
                         MessaggioErrore(risposta.Errore);
                     }

                 }, function (risposta) {
                     if (risposta.RispostaConferma == true) {
                         ConfermaControlliSiNo(risposta.Errore, 'conferma_' + str);
                     } else {
                         MessaggioErrore(risposta.Errore);
                     }

                 });
         }

         else if (str.startsWith('conferma_del_elem')) {

             //SalvaParametriDiv('#frmInput', false);

             ajaxAgronica("./Menu/MenuBS_Agenda_Nuovo.aspx/elimina_operazione_multipla", JSON.stringify({ strChiaviComposite: str, proseguiInCasoDiAlert: true }),
                 function (risposta) {

                     if (risposta.RispostaOK) {
                         ScritturaOK(risposta.RispostaStringa);
                         eseguiRicercaVisite();// Aggiorno la tabella
                     }
                     else {
                         //non cancellabile
                         MessaggioErrore(risposta.Errore);
                     }

                 }, function (risposta) {
                     MessaggioErrore(risposta.Errore);
                 });
         }

     }

     //FUNZIONE CHE MOSTRA LA TABELLA
     function eseguiRicercaVisite() {
         var param = kendo.stringify({ 'objP_server': objP_server, 'objP_utenti': objP_utenti });

         ajaxAgronica(pathCoreWS + "AgronicaCoreVisite/Visite.asmx/Leggi_ListaVisite_ToKendoGrid", param,
             function (risposta) {

                 $('#hdKendo_Valorizzazione').val(risposta.RispostaStringa);
                 popolaGrigliaVisite("tabella_visite");

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
             scrollable: false,
             salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
             pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
             //sortable: {mode: "multiple", allowUnsort: true, showIndexes: true},
             filterable: { mode: "menu" }
         };
         var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe };
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

         //$("#" + IDControllo).data("kendoGrid").dataSource.sort([{ field: "Data_Movimento", dir: "desc" }, { field: "Ora_Movimento", dir: "desc" }]);
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

     function onDataBoundRighe(e) {

         var gridId = e.sender.element[0].id;
         var grid = $("#" + gridId).data("kendoGrid");
         for (var i = 0; i < grid.columns.length; i++) {
             grid.autoFitColumn(i);
         }
     }

     function ImportaVisiteAPP() {

        $("#BtnImportaVisiteAPP").addClass("disabled");
        // console.log("ImportaVisiteAPP");

        var parametri = kendo.stringify({ "Piva": "", "objP_server": objP_server, "objP_utenti": objP_utenti });

        ajaxAgronica(pathCoreWS + "AgronicaCoreVisite/Visite.asmx/ImportaVisiteAPP",
            parametri,
            function (risposta) {
                $("#BtnImportaVisiteAPP").removeClass("disabled");
                if (risposta.RispostaOK) {
                    if (risposta.RispostaStringa !== "") {
                        kendo.alert(risposta.RispostaStringa);
                        //Ricarico dati griglia
                        eseguiRicercaVisite();
                    } else {
                        kendo.alert("Non ci sono dati da importare");
                    }
                } else {
                    kendo.alert("Si è verificato un problema durante l'importazione dei dati<br><br>" + risposta.Errore);
                }
            }, null);

     }

     function CaricaVisiteAPP() {

         $("#BtnImportaVisiteAPP").addClass("disabled");

         ajaxAgronica(pathCoreWS + "GiasApp/SincroDatiApp.asmx/CaricaDatiApp",

             kendo.stringify({ "tipo": "30", "piva": "", "objP_super_server": objP_super_server, "objP_server": objP_server, "objP_utenti": objP_utenti }),

             function (risposta) {

                 $("#BtnImportaVisiteAPP").removeClass("disabled");

                 if (risposta.RispostaOK) {
                     if (risposta.RispostaStringa !== "") {
                         kendo.alert(risposta.RispostaStringa);
                         //Ricarico dati griglia
                         eseguiRicercaVisite();
                     } else {
                         kendo.alert("Non ci sono dati da importare");
                     }
                 } else {
                     kendo.alert("Si è verificato un problema durante l'importazione dei dati<br><br>" + risposta.Errore);
                 }

             }, null);

     }

 </script>

</asp:Content>
