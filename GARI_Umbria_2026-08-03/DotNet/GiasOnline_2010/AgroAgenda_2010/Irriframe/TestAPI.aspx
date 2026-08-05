<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="TestAPI.aspx.vb" Inherits="AgroAgenda_2010.TestAPI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- UTENTE -->
    <div class="row">
        <div class="col-lg-12"><h4>UTENTE (USER)</h4></div>
        <div class="col-lg-4">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_UserName">Email</label>
                        <input type="text" id="Txt_UserName" class="form-control" value="demo@irriframe.it" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Password">Password</label>
                        <input type="text" id="Txt_Password" class="form-control" value="password" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Id_User">ID User</label>
                        <input type="text" id="Txt_Id_User" class="form-control" value="3" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="btn btn-success" onclick="RegistraUtente();">
                <span class="fa fa-user"></span> Registra Utente
            </div>
        </div>
    </div>

    <!-- AZIENDA -->
    <div class="row">
        <div class="col-lg-12"><h4>AZIENDA (FARM)</h4></div>
        <div class="col-lg-4">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_RagSoc">Ragione Sociale</label>
                        <input type="text" id="Txt_RagSoc" class="form-control" value="Azienda agricola Rossi" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-3">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Piva">Piva</label>
                        <input type="text" id="Txt_Piva" class="form-control" value="00000000001"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-3">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Piva">Cuaa</label>
                        <input type="text" id="Txt_Cuaa" class="form-control" value="00000000001"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Id_Farm">ID Farm</label>
                        <input type="text" id="Txt_Id_Farm" class="form-control"  value="4530" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-12">
            <div class="btn btn-success" onclick="RegistraImpresa();">
                <span class="fa fa-industry"></span> Registra Impresa
            </div>
        </div>
    </div>

    <!-- APPEZZAMENTO -->
    <div class="row">
        <div class="col-lg-12"><h4>APPEZZAMENTO (PLOT)</h4></div>
        <div class="col-lg-4">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Desc_Plot">Descrizione</label>
                        <input type="text" id="Txt_Desc_Plot" class="form-control" value="plot 31663 25" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Latitudine">Latitudine</label>
                        <input type="text" id="Txt_Latitudine" class="form-control" value="44.5240108" />
                    </div>
                </div>
            </div>
        </div>        
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Longitudine">Longitudine</label>
                        <input type="text" id="Txt_Longitudine" class="form-control" value="11.3499689" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Superficie">Superficie [m<sup>2</sup>]</label>
                        <input type="text" id="Txt_Superficie" class="form-control" value="10000"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Pendenza">Pendenza</label>
                        <input type="text" id="Txt_Pendenza" class="form-control" value="2"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Altezza">Altezza oriz.</label>
                        <input type="text" id="Txt_Altezza" class="form-control" value="1,4"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Pietra">% Pietra</label>
                        <input type="text" id="Txt_Pietra" class="form-control" value="0"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Sabbia">% Sabbia</label>
                        <input type="text" id="Txt_Sabbia" class="form-control" value="22"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Argilla">% Argilla</label>
                        <input type="text" id="Txt_Argilla" class="form-control" value="55"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Id_Farm_Plot">ID Farm</label>
                        <input type="text" id="Txt_Id_Farm_Plot" class="form-control"  value="4530" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Id_Plot">ID Plot</label>
                        <input type="text" id="Txt_Id_Plot" class="form-control" value="31663" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-12">
            <div class="btn btn-success" onclick="RegistraAppezzamento();">
                <span class="fa fa-map"></span> Registra Appezzamento
            </div>
        </div>
    </div>

    <!-- IMPIANTO / ESERCIZIO -->
    <div class="row">
        <div class="col-lg-12"><h4>IMPIANTO / ESERCIZIO (CROP)</h4></div>
        <div class="col-lg-4">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Desc_Crop">Descrizione</label>
                        <input type="text" id="Txt_Desc_Crop" class="form-control" value="test" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Coltura">Coltura</label>
                        <input type="text" id="Txt_Coltura" class="form-control" value="8" />
                    </div>
                </div>
            </div>
        </div>        
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Ciclo">Ciclo</label>
                        <input type="text" id="Txt_Ciclo" class="form-control" value="1261" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Data_Inizio">Data inizio</label>
                        <input type="text" id="Txt_Data_Inizio" class="form-control" value="25/04/2021"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Data_Raccolta">Data raccolta</label>
                        <input type="text" id="Txt_Data_Raccolta" class="form-control" value="29/04/2021"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Su_Fila">Su Fila</label>
                        <input type="text" id="Txt_Su_Fila" class="form-control" value="2"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Tra_Fila">Tra Fila</label>
                        <input type="text" id="Txt_Tra_Fila" class="form-control" value="3"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Conduzione">Cond. InterFila</label>
                        <input type="text" id="Txt_Conduzione" class="form-control" value="I"  />
                    </div>
                </div>
            </div>
        </div>
        
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Vigore">Classe Vigore</label>
                        <input type="text" id="Txt_Vigore" class="form-control" value="2"  />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Id_Plot_Crop">ID Plot</label>
                        <input type="text" id="Txt_Id_Plot_Crop" class="form-control" value="31663" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Id_Crop">ID Crop</label>
                        <input type="text" id="Txt_Id_Crop" class="form-control" value="138358" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-12">
            <div class="btn btn-success" onclick="RegistraImpianto();">
                <span class="fa fa-leaf"></span> Registra Impianto
            </div>
        </div>
    </div>

    <!-- IRRIGAZIONE -->
    <%--<div class="row">
        <div class="col-lg-12"><h4>IRRIGAZIONE</h4></div>
        <div class="col-lg-4">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_Piva_Agenda">Piva</label>
                        <input type="text" id="Txt_Piva_Agenda" class="form-control" value="<%=objParametriAgenda.Piva%>" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="input-group">
                        <label class="input-group-addon alert-info" for="Txt_ID_Agenda">ID Agenda</label>
                        <input type="text" id="Txt_ID_Agenda" class="form-control" value="1" />
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="btn btn-success" onclick="RegistraIrrigazione();">
                <span class="fa fa-tint"></span> Registra Irrigazione
            </div>
        </div>
    </div>--%>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("TestAPI.js") %>"></script>
</asp:Content>
