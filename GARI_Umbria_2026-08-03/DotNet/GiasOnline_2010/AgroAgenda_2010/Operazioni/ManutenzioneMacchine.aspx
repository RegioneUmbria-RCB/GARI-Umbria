<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="../Master/AgendaBootstrap.Master" 
CodeBehind="ManutenzioneMacchine.aspx.vb" Inherits="AgroAgenda_2010.ManutenzioneMacchine" %>

     <%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="row">
    <div class="col-lg-12 text-right">
        <div class="btn btn-success" onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
            <i class="fa fa-floppy-o"></i>Salva</div>

        <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
            Style="display: none" />
      
    </div>

</div>

<div class="jumbotron" style="margin-bottom: 100px;">
    <div class="row">
        <div class="col-lg-6 col-md-12 col-sm-12">
            <div class="row nopadding">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <asp:Label ID="LblLavorazione" runat="server" meta:resourcekey="lblSupHaSelezionataResource1"
                                    CssClass="input-group-addon alert-info"><i class="fa fa-calendar"></i> Macchine del</asp:Label>
                                <asp:TextBox runat="server" CssClass="form-control datepicker" id="Txt_DataIntervento"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <asp:Label ID="lbl_Certificato" runat="server" meta:resourcekey="lblSupHaSelezionataResource1"
                                    CssClass="input-group-addon alert-info">Certificato N°</asp:Label>
                                <asp:TextBox runat="server" CssClass="form-control" id="Txt_Certificato"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <asp:Label cssClass="input-group-addon alert-info" id="Lbl_CentroRevisione" for="Cmb_CentroRevisione" runat="server">Organismo di controllo</asp:Label>
                                <asp:DropDownList ID="Cmb_CentroRevisione" runat="server" CssClass="form-control selectpicker"
                                    data-live-search="true" aria-describedby="Lbl_CentroRevisione">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <asp:Label ID="lbl_scadenza" runat="server" meta:resourcekey="lblSupHaSelezionataResource1"
                                    CssClass="input-group-addon alert-info"><i class="fa fa-calendar"></i> Scade il</asp:Label>
                                <asp:TextBox runat="server" CssClass="form-control datepicker" ID="Txt_DataScadenza"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-6 col-md-12 col-sm-12">
            <div class="row nopadding">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <asp:Label ID="lbl_Note" runat="server" meta:resourcekey="lblSupHaSelezionataResource1"
                                    CssClass="input-group-addon alert-info">Note Intervento</asp:Label>
                                <asp:TextBox  TextMode="MultiLine" rows="6" runat="server" CssClass="form-control" id="Txt_Note"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                    <div class="btn btn-info" id="btn_Nuovo_Utente" onclick="nuovoContatto();">
                        <i class="fa fa-plus"></i>Nuovo Contatto
                    </div>
                </div>
                <!--<div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                    <div class="btn btn-info" id="btn_nuovo_allegato" onclick="nuovoAllegato();">
                        <i class="fa fa-plus"></i>Aggiungi Allegato
                    </div>
                </div>-->
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12 border_si">
            <div class="col-md-12 text-center">
                <h5 style="color: #052747; text-transform: uppercase;">Macchine e attrezzature</h5>
            </div>
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div id="tabMacchine" style="overflow: auto;">
                </div>
            </div>
        </div>
    </div>
</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <div id="cont_script">
    </div>
        <script type="text/javascript">

            var watableMacchine;


             function AggiornaTabMacchine(d) {

                AgroWA_Table_sistemaDati(d);

                $('#tabMacchine').html('');
                watableMacchine = $("#tabMacchine").WATable({
                    pageSize: 50,
                    pageSizes: [50],
                    filter: true,
                    preFill: false,
                    checkboxes: true,
                    tableCreated: function (data) {
                        //                                    coloraMovimenti();
                    }
                    , pageChanged: function (data) {
                        //                                    coloraMovimenti();
                    }
                , types: {
                    string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                    date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                    number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                    bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                }
                }).data('WATable').setData(d);

            InitWaTable("#tabMacchine", watableMacchine);
        }


 

            function ValidaxSubmit() {

                var flag = controlla_form();

                if (flag) {

                    if (!ControlloData($('#<%=Txt_DataIntervento.clientID %>').val())) {
                        alert("Inserire una data intervento valida!");
                        return
                    }

                    if (!ControlloData($('#<%=Txt_DataScadenza.clientID %>').val())) {
                        alert("Inserire una data scadenza valida!");
                        return
                    }

                    if ($('#<%=txt_certificato.clientID %>').val() === ""){
                        alert("Inserire un numero di certificato!");
                        return
                    }
                    else {
                        if (isNaN($('#<%=txt_certificato.clientID %>').val())) {
                            alert("Inserire un valore numerico nel certificato!");
                            return
                        }
                        
                    }

                    var app_arr="";

                    for (var i=0; i<watableMacchine.getData('checked').rows.length; i++){
                        app_arr += watableMacchine.getData('checked').rows[i]['chiave'] +"|";
                   
                        //app_arr.push(watableGestCat.getData('checked').rows[i]['chiave']);
                    }

                    // Controllo se è stata selezionata almeno una riga
                    if(app_arr != ""){

                        app_arr.slice(0, -1);

                        var aggiorna_date = confirm("Si vogliono aggiornare anche le date di ultima " + $('#<%= Master.Lbl_Titolo.ClientID %>').text().toLowerCase() + " in anagrafica?")

                         // salvo le righe checked (cambiate lato client) per il Salva Tutto
                         $.ajax({
                             type: 'POST',
                             url: 'ManutenzioneMacchine.aspx/Salva_Macchine_X_Salva_Tutto',
                             data: "{dati: '" + app_arr +"', aggiorna_date:'" + aggiorna_date + "'}",
                             contentType: 'application/json; charset=utf-8',
                             cache: false,
                             dataType: 'json', async: false,
                             success: function (r) {
                                 $('#<%=ImgBtn_SalvaTutto.ClientID %>').click();
                             }
                         });



                    }
                    else
                        alert('Selezionare almeno un Macchinario');

                }
            }


            function controlla_form() {

                var flag = true;
                var n_inv = 0;

                v.resetForm();

                // Azzero tutte le label custom_val
                $(".custom_val").each(function (i, obj) {
                    $(this).css('border', '1px solid #ccc');
                    $(this).parent().children().css('border-color', '#ccc');
                    $(this).remove();
                });

                // Validazione Campi


                //////////////////////////

                if (v.valid() && flag) {

                    //$('.nav-tabs li.active a .error-tab').remove( ".error-tab" );
                    return true
                }
                else {
                    //                var err_message = "";
                    //                err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red'>" + n_inv + "</div>";
                    //                $('.nav-tabs li.active a').append(err_message);

                    return false;
                }
            }


            function carica_macchine(){
            // carico le macchine già checkate o meno in base al tipooperazione e Session("UtenteAbilitato_Lettura")/Session("UtenteAbilitato_Modifica")
                $.ajax({
                    type: 'POST',
                    url: 'ManutenzioneMacchine.aspx/Carica_Macchine',
                    data: "{dati: ''}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: false,
                    success: function (r) {
                        if (r.d.RispostaOK) {
                            var dd = JSON.parse(r.d.RispostaStringa);
                            AggiornaTabMacchine(dd);
                        }
                        else {
                            alert(r.d.Errore);
                        }

                    }
                });
            }


            function nuovoContatto() {
                $.ajax({
                    type: 'POST',
                    url: 'ManutenzioneMacchine.aspx/Carica_NuovoContatto',
                    data: "{dati: ''}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: false,
                    success: function (r) {
                        if (r.d.RispostaOK) {
                            $('#cont_script').append(r.d.ParametroDue_stringa);
                        }
                        else {
                            alert(r.d.Errore);
                        }

                    }
                });
            }


            function nuovoAllegato() {
                alert("Operazione in manutenzione");
//                COMMENTATO PERCHe' da finire
//                var dataScadenza = $('#<%=Txt_DataScadenza.clientID %>').val();
//                if (!ControlloData(dataScadenza)) {
//                    alert("Inserire una data scadenza valida!");
//                    return
//                }
//                $.ajax({
//                    type: 'POST',
//                    url: 'ManutenzioneMacchine.aspx/ApriScadenziario',
//                    data: "{dataFine: '" + dataScadenza + "'}",
//                    contentType: 'application/json; charset=utf-8',
//                    cache: false,
//                    dataType: 'json', async: false,
//                    success: function (r) {
//                        if (r.d.RispostaOK) {
//                            $('#cont_script').append(r.d.ParametroDue_stringa);
//                        }
//                        else {
//                            alert(r.d.Errore);
//                        }

//                    }
//                });
            }

            function ControlloData(data) {
       
                // Struttura l'espressione regolare
                var espressione = /^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$/;

                // Effettua il test sulla stringa e 
                //   ritorna il risultato con un alert
                if (!espressione.test(data)) {
                    return false;
                } else {
                    return true;
                }
            }


            jQuery(document).ready(function () {
                carica_macchine();
            });

           

        </script>
</asp:Content>
