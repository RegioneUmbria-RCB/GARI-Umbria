<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Scad_CategorieTipologie_UC.ascx.vb" Inherits="AgroAgenda_2010.Scad_CategorieTipologie_UC" %>
<%@ Import Namespace="AgroAgenda_2010" %>


<!--CATEGORIE-->
    <div id="pnlCategorie" class="panel-group" style="padding-top: 3px">
        <div class="panel panel-primary">
            <div class="panel-heading" style="padding: 25px 7px 15px 7px;">
                <h3 class="panel-title" style="font-size: 24px; padding-left: 15px;">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Categorie %>" runat="server">CATEGORIE</asp:Localize>
                </h3>
            </div>
            <div class="panel-body">

             <%--<div class="row" style="margin-bottom:20px;">
                    <div class="col-lg-12 col-md-12 col-xs-12">
                        <div class="input-group">
                            <label class="input-group-addon control-label alert-info" id="CTRL_Azienda" for="ddlAzienda">Azienda</label>
                            <input type="text" id="ddlAzienda" name="ddlAzienda" class="form-control" aria-describedby="CTRL_Azienda" onchange="LsbAree_Load();"/> 
                        </div>
                        <span style="color:red">NB: Le categorie create per un'azienda saranno visibili anche per quelle gerarchicamente figlie (se presenti).</span>
                    </div>
                </div>--%>
                

                <div class="row">
                    <div class="col-md-6">
                        <div>Categoria</div>
                        <select id="LsbAree" size="6" style="width:300px;" onchange="LsbAree_onchange();"></select>
                    </div>
                    <div class="col-md-6" style="margin-top: 25px;">
                        <div class="row">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                            <label class="input-group-addon" id="LblNomeArea" for="TxtNomeArea">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NomeCategoria %>" runat="server">Nome Categoria</asp:Localize>
                                            </label>
                                            <input type="text" id="TxtNomeArea" class="form-control" aria-describedby="LblNomeArea" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="btn btn-warning btn-danger" id="BtnAree_Del" style="display:none; margin-bottom: 2px" onclick="conferma_cancellaArea();">
                                <span class="fa fa-trash"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaCancella %>" runat="server">Cancella</asp:Localize>
                            </div>
                            <div class="btn btn-success xi-btn-primary" id="BtnAree_Edit" style="display:none; margin-bottom: 2px" onclick="modificaArea();">
                                <span class="fa fa-save"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Modifica %>" runat="server">Modifica</asp:Localize>
                            </div>
                            <div class="btn btn-success xi-btn-primary add" id="BtnAree_Add" style="display:none; margin-bottom: 2px" onclick="aggiungiArea();">
                                <span class="fa fa-save"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AggiungiComeNuovo %>" runat="server">Aggiungi come nuovo</asp:Localize>
                            </div>
                            
                            <!-- Anna 23/07/21: Aggiunta button per spostare documenti su DB -->
                            <div class="btn btn-success" id="spostaCategoria"  style="background-color:red" onclick="spostaCategoria();">
                                <span class="fa fa-save"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpostaAllegatiSuDB %>" runat="server">Sposta allegati su database (solo SuperUser)</asp:Localize>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row" style="margin-top: 10px;">
                    <div class="col-md-6">
                        <div><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipologia %>" runat="server">Tipologia</asp:Localize></div>
                        <select id="LsbTipologie" size="6" style="width:300px;" onchange="LsbTipologie_onchange();"></select>
                    </div>

                    <div class="col-md-6" style="margin-top: 25px;">
                        <div class="row">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                            <label class="input-group-addon" id="LblNomeTipologia" for="TxtNomeTipologia">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NomeTipologia %>" runat="server">Nome Tipologia</asp:Localize>
                                            </label>
                                            <input type="text" id="TxtNomeTipologia" class="form-control" aria-describedby="LblNomeTipologia" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%--<!-- Anna 14/04/22: aggiunti FlagDataScadenzaObbligatoria e DataDefault -->
                        <div class="row">
                            <div class="input-group">
                                <label class="input-group-addon" id="lblScadenzaObbligatoria" for="FlagDataScadenzaObbligatoria">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScadenzaObbligatoria %>" runat="server">Scadenza Obbligatoria</asp:Localize>
                                </label>
                                <input type="checkbox" name="FlagDataScadenzaObbligatoria" id="FlagDataScadenzaObbligatoria" class="" />
                                                                
                                <label class="input-group-addon " id="lblDataDefault" for="DataDefault">
                                     <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDefault %>" runat="server">Data Default (gg/mm)</asp:Localize>:
                                </label>
                                <input id="DataDefault" name="DataDefault" class="" style="width: 50%;" />
                             </div>
                        </div>--%>

                        <div class="row">
                            <div class="btn btn-warning btn-danger" id="BtnTipologie_Del" style="display:none; margin-bottom: 2px" onclick="conferma_cancellaTipologia();">
                                <span class="fa fa-trash"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaCancella %>" runat="server">Cancella</asp:Localize>
                            </div>
                            <div class="btn btn-success xi-btn-primary" id="BtnTipologie_Edit" style="display:none; margin-bottom: 2px" onclick="modificaTipologia();">
                                <span class="fa fa-save"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Modifica %>" runat="server">Modifica</asp:Localize>
                            </div>
                            <div class="btn btn-success xi-btn-primary add" id="BtnTipologie_Add" style="display:none; margin-bottom: 2px" onclick="aggiungiTipologia();">
                                <span class="fa fa-save"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AggiungiComeNuovo %>" runat="server">Aggiungi come nuovo</asp:Localize>
                            </div>

                            <!-- Anna 23/07/21: Aggiunta button per spostare documenti su DB -->
                            <div class="btn btn-success" id="spostaTipologia" style="background-color:red" onclick="spostaTipologia();">
                                <span class="fa fa-save"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpostaAllegatiSuDB %>" runat="server">Sposta allegati su database (solo SuperUser)</asp:Localize>
                            </div>
                        </div>

                        <!-- Anna 14/04/22: aggiunti FlagDataScadenzaObbligatoria e DataDefault -->
                        <div class="row" style="margin-top:10px;">
                            <div class="input-group" id ="accessori_tipologia">
                                <label class="input-group-addon" id="lblScadenzaObbligatoria" for="FlagDataScadenzaObbligatoria">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ScadenzaObbligatoria %>" runat="server">Scadenza Obbligatoria</asp:Localize>
                                </label>
                                <input type="checkbox" name="FlagDataScadenzaObbligatoria" id="FlagDataScadenzaObbligatoria" class="kendoSwitch" />
                                                                
                                <label class="input-group-addon " id="lblDataDefault" for="DataDefault">
                                     <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDefault %>" runat="server">Data Default (gg/mm)</asp:Localize>:
                                </label>
                                <input id="DataDefault" name="DataDefault" class="" style="width: 50%;" onchange="AccessoriTipologia_change()" />                               

                             </div>
                        </div>

                        <div class="row" >
                            <div class="input-group" id="Tipologie_utilizzabili_da_app">
                                    <label class="input-group-addon" id="lbl_utilizzabile_da_app" for="switch_utilizzabile_da_app" >Utilizzabile da APP:</label>
                                    <input type="checkbox" id="switch_utilizzabile_da_app" name="switch_utilizzabile_da_app" class="kendoSwitch"/>                             
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
 
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/Scad_CategorieTipologie_UC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/Scad_CategorieTipologie_UC_ws_client.js")) %>"></script>

<script>
    $(document).ready(function () {

        var d = new Date();
        var year = d.getFullYear();
        var openFlag = true;

        // create DatePicker from input HTML element
        $("#DataDefault").kendoDatePicker({
            // defines the start view
            start: "year",
            // defines when the calendar should return date
            depth: "month",

            // display month and year in the input
            format: "dd/MM",
            yearRange: year,
            // specifies that DateInput is used for masking the input element
            dateInput: true,

            //i mesi partono da index 0
            min: new Date(year, 0, 1), 
            max: new Date(year, 11, 31),
            //Cosa mostra alla base del calendario
            footer: "#: kendo.toString(data, 'dd MMMM') #",
            
            value: new Date(),

            //Questa funzione rimuove la vista dell'anno dal calendario
            open: function (e) {
                var dp = e.sender;
                var calendar = dp.dateView.calendar;

                if (openFlag) {
                    calendar.setOptions({
                        animation: false
                    });
                    openFlag = false;
                    calendar.navigateUp();
                }


                if (calendar.view().name === "year") {
                    calendar.element.find(".k-header").addClass("k-hidden");
                };

                calendar.bind("navigate", function (e) {
                    var cal = e.sender;
                    var view = cal.view();

                    if (view.name === "year") {
                        cal.element.find(".k-header").addClass("k-hidden");
                    } else {
                        var navFast = $(".k-nav-fast");

                        var dsa = cal.element.find(".k-header").removeClass("k-hidden");
                        navFast[0].innerText = navFast[0].innerText.slice(0, -5);
                    }

                });
            },
            close: function (e) {
                var calendar = e.sender.dateView.calendar;

                calendar.unbind("navigate");
                calendar.element.find(".k-header").removeClass("k-hidden");
            }
        }).data("kendoDatePicker");

        //$("#DataDefault").data("kendoDatePicker").enable(false);
        // DataDefault nascosta di default, se FlagDataScadenzaObbligatoria attivo > lo mostro
        //nascondiDataDefault(FlagDataScadenzaObbligatoria);

        //$("#FlagDataScadenzaObbligatoria").change(function () {
        //    if (getKendoSwitch("FlagDataScadenzaObbligatoria")) {
        //        $("#DataDefault").data("kendoDatePicker").enable(true);
        //    } else {
        //        $("#DataDefault").data("kendoDatePicker").enable(false);
        //    }
        //});
                
        //function nascondiDataDefault(FlagDataScadenzaObbligatoria) {
        //    var FlagDataScadenzaObbligatoria = getKendoSwitch("FlagDataScadenzaObbligatoria");
        //    var controllo = $("#DataDefault").data("kendoDatePicker");
        //    if (FlagDataScadenzaObbligatoria) {
        //        controllo.show();
        //    } else {
        //        controllo.hide();
        //    }
        //}


    });
</script>
