<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master"
    CodeBehind="Esportatore_Universale_2.aspx.vb" Inherits="AgronicaStampe_2010.Esportatore_Universale_2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script language="javascript" id="clientEventHandlersJS">
        function window_onbeforeunload() {
            var tbl = document.getElementById("caricando");
            tbl.style.posTop = String(document.body.scrollTop + 152);
            tbl.style.posLeft = String(document.body.scrollLeft + 16);

            var tbl = document.getElementById("Panel_Check");
            tbl.style.posTop = String(document.body.scrollTop - 1000);
            tbl.style.posLeft = String(document.body.scrollLeft - 1000);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron" style="margin-bottom:100px;">
    <div class="row">
            <div class="col-lg-12">
                <div class="col-lg-8">
                    <div class="hidden">
                        <div class="row">
                        <div class="btn-toolbar" role="toolbar" aria-label="...">
                            <asp:Panel ID="Panel_Btn_Imprese" runat="server" class="btn-group btn-group-lg">
                                <asp:Image ID="Img_Imprese" runat="server" ImageUrl="../../AB_Immagini/Icone24/x24 - 256 - Impresa.bmp"
                                    ToolTip="Gestione dei dati principali dell'appezzamento"></asp:Image>
                                <asp:Label ID="Lbl_BTN_Imprese" runat="server">Imprese</asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="Panel_Btn_Centri" runat="server" class="btn-group btn-group-lg">
                                <asp:Image ID="Img_Centri" runat="server" ImageUrl="../../AB_Immagini/Icone24/Centro.ico"
                                    ToolTip="Gestione dei dati catastali"></asp:Image>
                                <asp:Label ID="Lbl_BTN_Centri" runat="server">Centri</asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="Panel_Btn_Appezzamenti" runat="server" class="btn-group btn-group-lg">
                                <asp:Image ID="Img_Appezzamenti" runat="server" ImageUrl="../../AB_Immagini/Icone24/x24 - 256 - Appezzamento.bmp"
                                    ToolTip="Gestione dei dati accessori"></asp:Image>
                                <asp:Label ID="Lbl_BTN_Appezzamenti" runat="server">Appezz.</asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="Panel_Btn_Impianti" runat="server" class="btn-group btn-group-lg">
                                <asp:Image ID="Img_Impianti" runat="server" ImageUrl="../../AB_Immagini/Icone24/Impianto.ico"
                                    ToolTip="Gestione delle informazioni legate all'agricoltura biologica"></asp:Image>
                                <asp:Label ID="Lbl_BTN_Impianti" runat="server">Impianti</asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="Panel_Btn_Agenda" runat="server" class="btn-group btn-group-lg">
                                <asp:Image runat="server" ID="Img_Agenda" ImageUrl="../../AB_Immagini/Icone24/Agenda.ico"
                                    ToolTip="Salva le modifiche effettuate e torna alla pagina precedente"></asp:Image>
                                <asp:Label ID="Lbl_BTN_Agenda" runat="server">Agenda</asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="Panel_Btn_Rintraccio" runat="server" Visible="False" class="btn-group btn-group-lg">
                                <asp:Image ID="Img_Rintraccio" runat="server" ImageUrl="../../AB_Immagini/Icone24/Ope_Colturali_24.ico"
                                    ToolTip="Salva le modifiche effettuate e torna alla pagina precedente"></asp:Image>
                                <asp:Label ID="Lbl_BTN_Rintraccio" runat="server">XML Rintraccio</asp:Label>
                            </asp:Panel>
                        </div>
                    </div>
                        <br />
                        <br />
                    </div>
                    <div class="row">
                        <asp:Panel ID="Panel4" class="panel panel-default" runat="server" Visible="False">
                            <asp:Label ID="Label10" runat="server">EXPORT</asp:Label>
                            <asp:Button ID="Button2" runat="server" Text="Esporta!"></asp:Button>
                            <asp:Label ID="Label9" runat="server">Inserisci il nome da attribuire al file (o ai file) di output prodotto/i dall'esportazione:</asp:Label>
                            <asp:Label ID="Label8" runat="server">I file verranno salvati nella cartella AgroTemporanea o nella cartella selezionata in fase di installazione.</asp:Label>
                            <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="true">
                                <asp:ListItem Value="A">Excel (xls)</asp:ListItem>
                                <asp:ListItem Value="B">XML</asp:ListItem>
                                <asp:ListItem Value="C">XML Rintraccio</asp:ListItem>
                                <asp:ListItem Value="D">Access</asp:ListItem>
                                <asp:ListItem Value="E">Testo (campo fisso)</asp:ListItem>
                                <asp:ListItem Value="F">Testo (CSV)</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="Label7" runat="server">Per l'esportazione Excel e Rintraccio non è necessario digitare il nome del file.</asp:Label>
                            <asp:TextBox ID="Txt_FileOutput" runat="server"></asp:TextBox>
                        </asp:Panel>
                        <div >
                            <asp:Panel ID="Panel_Check" class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <asp:label ID="LblSeleziona" style="font-weight:bold;" runat="server" Visible="False">Seleziona i campi che vuoi estarre:</asp:label>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-6">
                                        
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:ImageButton ID="ImgBtnSelezionaTutto" class="hidden" runat="server" ImageUrl="../../AB_Immagini/Icone16/cS.ico" Visible="False"></asp:ImageButton>
                                        <asp:Label ID="LblSelezionaTutto" class="hidden"  runat="server" Visible="False">Seleziona tutto</asp:Label>
                                        <asp:ImageButton ID="ImgBtnDeselezionaTutto" class="hidden" runat="server" ImageUrl="../../AB_Immagini/Icone16/cN.ico" Visible="False"></asp:ImageButton>
                                        <asp:Label ID="LblDeselezionaTutto" class="hidden" runat="server" Visible="False">Deseleziona tutto</asp:Label>
                                        <button type="submit" id="BtnSelezionaTutto" runat="server" class="btn btn-primary btn-xs"><i class="fa fa-check-square-o fa-4"></i>Seleziona Tutto</button>
                                        <button type="submit" id="BtnDeselezionaTutto" runat="server" class="btn btn-primary btn-xs"><i class="fa fa-square-o fa-4"></i>Deseleziona Tutto</button>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-lg-4">
                                        <asp:CheckBoxList ID="ChkGruppo1" runat="server" Visible="False" ClientIDMode="Static">
                                            <asp:ListItem Value="0">Partita IVA</asp:ListItem>
                                            <asp:ListItem Value="1">Ragione Sociale</asp:ListItem>
                                            <asp:ListItem Value="2">CUAA</asp:ListItem>
                                            <asp:ListItem Value="3">Codice Socio</asp:ListItem>
                                            <asp:ListItem Value="4">Partita IVA Coop. Padre</asp:ListItem>
                                            <asp:ListItem Value="5">Cooperativa Padre</asp:ListItem>
                                            <asp:ListItem Value="6">Tecnico di Riferimento</asp:ListItem>
                                            <asp:ListItem Value="7">Centro Aziendale</asp:ListItem>
                                            <asp:ListItem Value="8">Indirizzo Centro Aziendale</asp:ListItem>
                                            <asp:ListItem Value="9">Cod ISTAT Comune e Provincia</asp:ListItem>
                                        </asp:CheckBoxList>
                                        <asp:CheckBoxList ID="ChkGruppo4" runat="server" Visible="false" ClientIDMode="Static">
                                            <asp:ListItem Value="32">CUAA</asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:CheckBoxList ID="ChkGruppo2" runat="server" Visible="False" ClientIDMode="Static">
                                            <asp:ListItem Value="10">Appezzamento</asp:ListItem>
                                            <asp:ListItem Value="11">Sup e Data Inizio/Fine Appezzamento</asp:ListItem>
                                            <asp:ListItem Value="12">Gruppo Vegetale</asp:ListItem>
                                            <asp:ListItem Value="13">Specie Vegetale</asp:ListItem>
                                            <asp:ListItem Value="14">Varietà</asp:ListItem>
                                            <asp:ListItem Value="15">Tipologia Varietale</asp:ListItem>
                                            <asp:ListItem Value="16">Finalità</asp:ListItem>
                                            <asp:ListItem Value="17">Destinazione d'uso</asp:ListItem>
                                            <asp:ListItem Value="18">Superficie Impianto</asp:ListItem>
                                            <asp:ListItem Value="19">Data Inizio/Fine Impianto</asp:ListItem>
                                        </asp:CheckBoxList>
                                        <asp:CheckBoxList ID="ChkGruppo5" runat="server" Visible="false" ClientIDMode="Static">
                                            <asp:ListItem Value="33">ZVN</asp:ListItem>
                                            <asp:ListItem Value="34">ACA</asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:CheckBoxList ID="ChkGruppo3" runat="server" Visible="False" ClientIDMode="Static">
                                            <asp:ListItem Value="20">Operazione e Data Semina/Trapianto</asp:ListItem>
                                            <asp:ListItem Value="21">Copertura</asp:ListItem>
                                            <asp:ListItem Value="22">Forma Allevamento e Dettaglio Specie Personalizzato</asp:ListItem>
                                            <asp:ListItem Value="23">Portinnesto e Impianto Irrigazione</asp:ListItem>
                                            <asp:ListItem Value="24">Sesto d'impianto</asp:ListItem>
                                            <asp:ListItem Value="25">N. Piante/Ha, N. Piante Tot e Resa Prevista</asp:ListItem>
                                            <asp:ListItem Value="26">Lotto e Data Inizio/Fine Distinta</asp:ListItem>
                                            <asp:ListItem Value="27">Regolamento e Capitolato Privato</asp:ListItem>
                                            <asp:ListItem Value="28">Organismo Referente e Magazzino di Conferimento</asp:ListItem>
                                            <asp:ListItem Value="29">Particelle, Sup Catastale e Sup di Intersezione</asp:ListItem>
                                            <asp:ListItem Value="30"><i style="color:gray">non utilizzato</i></asp:ListItem>
					                        <asp:ListItem Value="31"><i style="color:gray">non utilizzato</i><</asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                                <div class="row">
                                    <asp:Label ID="Lbl_Punti" runat="server" Visible="False">Seleziona il Punto del Sistema di Tracciabilità:</asp:Label>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:DropDownList ID="Cmb_Punti" runat="server" Visible="False" class="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <asp:Label ID="Lbl_Validita_Inizio" runat="server" Visible="False">Inserisci la validità inizio degli impianti:</asp:Label>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="Txt_ValiditaInizio" ClientIDMode="Static" class="form-control" runat="server" Visible="False" ReadOnly="True"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Panel ID="Panel_Esadecimale" class="panel panel-default"  runat="server">
                            <div class="panel-heading">
                                <h4>Smart Buid</h4>
                            </div>
                            <div class="panel-body">
                                <div class="col-lg-6" style="padding-right:25px;">
                                    <div class="row">
                                        <asp:Label ID="Lbl_Esadecimale" runat="server">Inserisci la Stringa SmartBuild per impostare i check:</asp:Label>
                                    </div>
                                    <div class="row">
                                        <asp:TextBox ID="Txt_Esadecimale" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <asp:Button ID="Btn_Imposta" runat="server" Text="Imposta Configurazione" class="btn btn-info btn-sm btn-block"></asp:Button>
                                    </div>
                                </div>
                                <div class="col-lg-6" style="padding-left:25px;">
                                    <div class="row">
                                        <asp:Label ID="Lbl_FileTxt" runat="server">Inserisci il nome del file txt (senza l'estensione) dove è salvato (o dove si andrà a salvare) la Stringa SmartBuild:</asp:Label>
                                    </div>
                                    <div class="row">
                                        <asp:TextBox ID="Txt_FileTxt" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <asp:Button ID="Btn_Salva" runat="server" Text="Salva configurazione" class="btn btn-primary btn-sm btn-block"></asp:Button>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <asp:Button ID="Btn_Apri" runat="server" Text="Apri configurazione" class="btn btn-warning btn-sm btn-block"></asp:Button>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="col-lg-4" style="padding-left:10px;">
                    <div class="row">
                        <asp:Panel ID="Panel_Esporta" class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h4>Export</h4>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <b>Tipo di Esportazione :</b>
                                </div>
                                <div class="row">
                                    <asp:RadioButtonList ID="RdBList_Esporta" runat="server" AutoPostBack="true" ClientIDMode="Static">
                                        <asp:ListItem Value="A">Excel (xls)</asp:ListItem>
                                        <asp:ListItem Value="C">XML Rintraccio</asp:ListItem>
                                        <asp:ListItem Value="B">XML</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <br />
                                <div class="row" ID="LblFiltroEsercizi" runat="server">
                                    <b>Filtro Esercizio degli Impianti :</b>
                                </div>
                                <div class="row">
                                    <asp:RadioButtonList ID="RdBList_Distinta" runat="server">
                                        <asp:ListItem Value="0">Visualizza tutti gli esercizi</asp:ListItem>
                                        <asp:ListItem Value="1" Selected="True">Filtra l'esercizio in base alla data seguente:</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:TextBox ID="Txt_Data" ClientIDMode="Static" runat="server" class="form-control"></asp:TextBox>
                                </div>
                                <br />
                                <div class="row">
                                    <asp:Label ID="lbl_ordinamento" class="Testo_08_Nero" runat="server">Ordinamento:</asp:Label>
                                    <asp:DropDownList id="Cmb_OrdinamentoImpianti" name="Cmb_OrdinamentoImpianti" runat="server" class="form-control"></asp:DropDownList>
                                </div>
                                <br />
                                <div class="row">
                                    <b>L'eventuale log degli errori verrà salvato sul server nel seguente percorso : </b>
                                </div>
                                <div class="row">
                                    <asp:TextBox ID="Txt_PathEsportazione" runat="server" class="form-control" ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <br />
                                <div class="row" id="Row_XML" runat="server" visible="false">
                                    <b>Il File XML verrà salvato nel seguente persorso:</b>
                                    <asp:TextBox ID="txt_XML" runat="server" class="form-control" ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <br />
                                <asp:Panel ID="PanelButtonEsporta" runat="server" Visible="false">
                                    <div class="row btn btn-success buttonClass gias-btn-primary" onclick="filtraMovimenti()">
                                        Filtra ed esporta dati
                                    </div>
                                </asp:Panel>

                                <asp:Button ID="Btn_Esporta" runat="server" Text="Esporta" class="btn btn-success btn-lg btn-block">
                                </asp:Button>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
</div>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Esportatore_Universale_2_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Esportatore_Universale_2.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Esportatore_Universale_2_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Esportatore_Universale_2_jQueryDocReady.js") %>"></script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
</asp:Content>
