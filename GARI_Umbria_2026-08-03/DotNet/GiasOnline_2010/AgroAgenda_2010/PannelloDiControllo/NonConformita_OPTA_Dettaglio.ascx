<%@ Control Language="vb" AutoEventWireup="false" className="CTRL_NC_OPTA_Dettaglio" 
        CodeBehind="NonConformita_OPTA_Dettaglio.ascx.vb" 
        Inherits="AgroAgenda_2010.NonConformita_OPTA_Dettaglio" %>

<div class="panel panel-default" style="background-color: #F1F3F7 !important; border:0 !important;">
    <h4 class="panel-title">
        <a role="button" data-toggle="collapse" data-parent="#accordion<%=hfID_Pannello.Value %>" href="#collapse<%=hfID_Pannello.Value %>" aria-expanded="true" aria-controls="collapse<%=hfID_Pannello.Value %>">
            <span style="float:left;"><i class="fa fa-plus-circle"></i></span>
            <div class="panel-heading" id="PnlTitolo" runat="server" style="float:left; background-color: #F1F3F7 !important; height: 25px;"></div>
            <div style="float:none;"></div>
        </a>
    </h4>
</div>

<div id="collapse<%=hfID_Pannello.Value %>" class="panel-collapse pannelloDettaglio" role="tabpanel" aria-labelledby="heading<%=hfID_Pannello.Value %>">
    <div class="panel-body">
        <!--Campi nascosti con gli ID che non devono essere visti ma mi devo ricordare-->
        <asp:HiddenField ID="hfID_Pannello" runat="server" />
        <asp:HiddenField ID="hfID_Elem" runat="server" />
        <input type="hidden" id="<%=hfID_Pannello.Value %>hfID_ListaAllegati" />
 
        <div class="row">
            <div class="col-lg-4 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon" id="<%=hfID_Pannello.Value %>CTRL_Data" for="<%=hfID_Pannello.Value %>txbData">
                                <span class="fa fa-calendar"></span> Data
                            </label>
                            <input type="text" id="<%=hfID_Pannello.Value %>txbData" class="form-control datepicker" aria-describedby="<%=hfID_Pannello.Value %>CTRL_Data" min="1900-01-01" max="2100-12-31" required/>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-4 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon" id="<%=hfID_Pannello.Value %>CTRL_Utente" for="<%=hfID_Pannello.Value %>ddlUtente">
                                <span class="fa fa-user"></span> Utente
                            </label>
                            <select id="<%=hfID_Pannello.Value %>ddlUtente" data-live-search="true" aria-describedby="<%=hfID_Pannello.Value %>CTRL_Utente" class="form-control selectpicker" required></select>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-4 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon" id="<%=hfID_Pannello.Value %>CTRL_Stato" for="<%=hfID_Pannello.Value %>ddlStato">Stato</label>
                            <select id="<%=hfID_Pannello.Value %>ddlStato" aria-describedby="<%=hfID_Pannello.Value %>CTRL_Stato" class="form-control selectpicker" required></select>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon" id="<%=hfID_Pannello.Value %>CTRL_Descrizione" for="<%=hfID_Pannello.Value %>txbDescrizione">Descrizione</label>
                            <input type="text" id="<%=hfID_Pannello.Value %>txbDescrizione" class="form-control" aria-describedby="<%=hfID_Pannello.Value %>CTRL_Descrizione" />              
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon" id="<%=hfID_Pannello.Value %>CTRL_Note" for="<%=hfID_Pannello.Value %>txbNote">Note</label>
                            <textarea id="<%=hfID_Pannello.Value %>txbNote" class="form-control" aria-describedby="<%=hfID_Pannello.Value %>CTRL_Note" rows="3" cols="20"></textarea>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group dropdown">
                            <div class="input-group-btn" id="<%=hfID_Pannello.Value %>CTRL_Allegati">
                                <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                                    <span class="fa fa-paperclip"></span> Allegati<span class="caret"></span>
                                </button>
                                <ul class="dropdown-menu" role="menu">
                                    <li><a href="#Modal_AllDet" data-toggle="modal" pnlCtrl-alldetIDAll="-1" pnlCtrl-alldetIDLista="-1" pnlCtrl-alldetIDDet="<%=hfID_Elem.Value %>" class="apri-AllegatoDettaglio">Crea nuovo</a></li>
                                    <li class="disabled" title="Selezionare prima un allegato"><a href="#Modal_AllDet" data-toggle="modal" pnlCtrl-alldetIDAll="-1" pnlCtrl-alldetIDLista="-1" pnlCtrl-alldetIDDet="<%=hfID_Elem.Value %>" class="apri-AllegatoDettaglio">Modifica</a></li>
                                    <li class="disabled" title="Selezionare prima un allegato"><a href="#">Elimina</a></li>
                                </ul>
                            </div>
                            <select id="<%=hfID_Pannello.Value %>ddlAllegati" aria-describedby="<%=hfID_Pannello.Value %>CTRL_Allegati" class="form-control selectpicker" onchange="PnlDet_ddlAllegati_Change(this, '<%=hfID_Pannello.Value %>hfID_ListaAllegati')"></select>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>


<script src="NonConformita_OPTA_Dettaglio_JQuery.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
<script type="text/javascript">

    //ALL'AVVIO
    $(document).ready(function () {
        ddlStato_Load('<%=hfID_Pannello.Value %>ddlStato');
        ddlUtente_Load('<%=hfID_Pannello.Value %>ddlUtente');

        PnlDet_ddlAllegati_Change("<%=hfID_Pannello.Value %>ddlAllegati", "<%=hfID_Pannello.Value %>hfID_ListaAllegati");
        if(<%=hfID_Elem.Value %> == -1){
             var tagLI_CreaNuovo = $("#<%=hfID_Pannello.Value %>ddlAllegati").parent().find('a:contains("nuovo")').parent();
             tagLI_CreaNuovo.addClass("disabled");
             tagLI_CreaNuovo.attr('title', "");
        }
    });

</script>
