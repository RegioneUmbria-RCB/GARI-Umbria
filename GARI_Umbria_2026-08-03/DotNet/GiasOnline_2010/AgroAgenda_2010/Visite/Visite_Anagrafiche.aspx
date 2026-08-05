<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap_Visite.master" 
    CodeBehind="Visite_Anagrafiche.aspx.vb" Inherits="AgroAgenda_2010.Visite_Anagrafiche" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_Visite.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_Visite" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_Visite" runat="server">

    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <!--VISITE PERSONALIZZATE-->
    <div id="pnlVisitePers" class="panel-group" style="padding-top: 3px">
        <div class="panel panel-primary">
            <div class="panel-heading" style="padding: 25px 7px 15px 7px;">
                <h3 class="panel-title" style="font-size: 24px; padding-left: 15px;">Visite Personalizzate</h3>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-6">
                        <select id="LsbVisitePers" size="4" style="width:300px;" onchange="ImpostaCtrlVisitePers();"></select>
                    </div>
                    <div class="col-md-6">
                        <div class="row">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="LblNomeVisitePers" for="TxtNomeVisitePers">Nome</label>
                                        <input type="text" id="TxtNomeVisitePers" class="form-control" aria-describedby="LblNomeVisitePers" nome_elem="" id_elem="" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon control-label alert-info" id="LblSiglaVisitePers" for="TxtSiglaVisitePers">Sigla</label>
                                        <input type="text" id="TxtSiglaVisitePers" class="form-control" aria-describedby="LblSiglaVisitePers" sigla_elem="" sigla="" />
                                    </div>
                                </div>
                            </div>
                        </div>



                        <div class="row">
                            <div class="btn btn-warning" id="BtnVisitePers_Del" style="display:none;" onclick="cancellaVisitePers();">
                                <span class="fa fa-trash"> Cancella</span>
                            </div>
                            <div class="btn btn-success" id="BtnVisitePers_Edit" style="display:none;" onclick="modificaVisitePers();">
                                <span class="fa fa-save"> Modifica</span>
                            </div>
                            <div class="btn btn-success add" id="BtnVisitePers_Add" style="display:none;" onclick="aggiungiVisitePers();">
                                <span class="fa fa-save"> Aggiungi come nuovo</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript_Visite" runat="server">
        <script type="text/javascript">

            var UtenteAbilitatoScrittura;
            var lavCodVisite = <%= AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_VISITA %>;

            //OPERAZIONI ALL'AVVIO
            $(document).ready(function () {
                //Controllo se ha i permessi di lettura e scrittura sulla pagina
                UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"

                //Se l'utente è abilitato alla scrittura aggiungo i pulsanti per l'inserimento di nuovi elementi
                if (UtenteAbilitatoScrittura == true) {
                    $(".add").show();
                }

                //Leggo l'elnco delle visite personalizzate
                leggiVisitePers();

            });

            function leggiVisitePers() {
                var param = kendo.stringify({ "objP_server": objP_server, "Id_Attivita": 0, "Lav_Cod": lavCodVisite });

                ajaxAgronicaSync(pathCoreWS + "Contab/AttivitaXOperazioni.asmx/Leggi", param, false,
                    function (risposta) {
                        var lista = risposta.RispostaStringa;
                        popolaListBox('LsbVisitePers', lista, null);
                    }, null);
            }

            //Per aggiornare il contenuto di una Listbox con una lista di listitem
            function popolaListBox(IDControllo, lista, idSelez) {
                //pulisco la combo
                $("#" + IDControllo).find('option').remove();

                //genero il codice HTML
                var strHtml = "";
                lista = JSON.parse(lista);
                for (var i = 0; i < lista.length; i++) {
                    strHtml += "<option value='" + lista[i]["ID_Attivita"] + "'>" + lista[i]["Descrizione"] + "         Sigla:" + lista[i]["Sigla"] + "</option>";
                }

                //aggiungo l'HTML al controllo
                $("#" + IDControllo).html(strHtml);
                //seleziono l'eventuale elemento
                if (idSelez != null) {
                    $("#" + IDControllo).val(idSelez);
                }
            }

            //Per impostare la textBox in base a cio che è stato selezionato nella select
            function ImpostaCtrlVisitePers() {

                //Estraggo i controlli dal pannello
                var obj_lsb = $("#pnlVisitePers select")[0];
                var obj_txt = $("#pnlVisitePers input")[0];
                var obj_lsb1 = $("#pnlVisitePers select")[1];
                var obj_txt1 = $("#pnlVisitePers input")[1];
                var obj_btnEdit = $("#pnlVisitePers [id$='Edit']")[0];
                var obj_btnDel = $("#pnlVisitePers [id$='Del']")[0];
               
                var valore = $(obj_lsb).val();
               
                if (valore != null) {

                    //Se è stato selezionato qualcosa...

                    //Estraggo nome e valore di ciò che è selezionato
                    var nomeesteso = $(obj_lsb).find('option:selected').text().split("Sigla:");

                    var nome = nomeesteso[0].trim();
                    var sigla = nomeesteso[1].trim();

                    $(obj_txt).val(nome);
                    $(obj_txt).attr("nome_elem", nome);
                    $(obj_txt).attr("id_elem", valore);
                    $(obj_txt1).val(sigla);
                    $(obj_txt1).attr("sigla_elem", sigla);
                    $(obj_txt1).attr("sigla", sigla);

                    if (UtenteAbilitatoScrittura) {
                        $(obj_btnEdit).show();
                        $(obj_btnDel).show();
                    }
                }
                else {
                    //Se non è stato selezionato nulla...
                    $(obj_txt).val("");
                    $(obj_txt).attr("nome_elem", "");
                    $(obj_txt).attr("id_elem", "");

                    $(obj_txt1).val("");
                    $(obj_txt1).attr("sigla_elem", "");
                    $(obj_txt1).attr("sigla", "");


                    $(obj_btnEdit).hide();
                    $(obj_btnDel).hide();
                }

            }

            //PER MODIFICARE UNA VISITA PERSONALIZZATA
            function modificaVisitePers() {

                //Estraggo i controlli dal pannello
                var obj_lsb = $("#pnlVisitePers select")[0];
                var obj_txt = $("#pnlVisitePers input")[0];
                var obj_txt1 = $("#pnlVisitePers input")[1];

                //Estraggo nome e id nuovi
                var Descrizione = $(obj_txt).val();
                var Sigla = $(obj_txt1).val();
                var ID_Attivita = $(obj_txt).attr("id_elem");

                //Salvo le modifiche
                var param = JSON.stringify({ 'objP_server': objP_server, 'ID_Attivita': ID_Attivita, 'Descrizione': Descrizione, 'Sigla': Sigla });
                ajaxAgronicaSync(pathCoreWS + 'Contab/Attivita.asmx/ModificaDescrizioneAttivita', param, false,
                    null, null);

                //Leggo i nuovi dati
                leggiVisitePers();

                //Aggiorno i controlli
                ImpostaCtrlVisitePers();
            }

            //PER AGGIUNGERE UNA VISITA PERSONALIZZATA
            function aggiungiVisitePers() {

                //Estraggo i controlli dal pannello
                var obj_lsb = $("#pnlVisitePers select")[0];
                var obj_txt = $("#pnlVisitePers input")[0];
                var obj_txt1 = $("#pnlVisitePers input")[1];

                //Estraggo nome nuovo
                var Descrizione = $(obj_txt).val();
                var Sigla = $(obj_txt1).val();
                var Lav_Cod = lavCodVisite;
                var Tariffa_Cod = 0;
                var ID_Attivita = 0;

                //Aggiungo l'elemento
                var param = JSON.stringify({ 'objP_server': objP_server, 'Lav_Cod': Lav_Cod, 'ID_Attivita': ID_Attivita, 'Tariffa_Cod': Tariffa_Cod, 'Descrizione': Descrizione, 'Sigla': Sigla });
                ajaxAgronicaSync(pathCoreWS + 'Contab/Attivita.asmx/Aggiungi', param, false,
                    null, null);


                //Leggo i nuovi dati
                leggiVisitePers();

                //Aggiorno i controlli
                ImpostaCtrlVisitePers();
            }

            //PER CANCELLARE UNA VISITA PERSONALIZZATA
            function cancellaVisitePers() {

                //Estraggo i controlli dal pannello
                var obj_lsb = $("#pnlVisitePers select")[0];
                var obj_txt = $("#pnlVisitePers input")[0];
                var obj_txt1 = $("#pnlVisitePers input")[1];

                //Estraggo nome e id nuovi
                var Lav_Cod = lavCodVisite;
                var ID_Attivita = $(obj_txt).attr("id_elem");

                //Salvo le modifiche
                var param = JSON.stringify({ 'objP_server': objP_server, 'ID_Attivita': ID_Attivita, 'Lav_Cod': Lav_Cod });
                ajaxAgronicaSync(pathCoreWS + 'Contab/Attivita.asmx/Cancella', param, false,
                    null, null);

                //Leggo i nuovi dati
                leggiVisitePers();

                //Aggiorno i controlli
                ImpostaCtrlVisitePers();

            }


        </script>

</asp:Content>
