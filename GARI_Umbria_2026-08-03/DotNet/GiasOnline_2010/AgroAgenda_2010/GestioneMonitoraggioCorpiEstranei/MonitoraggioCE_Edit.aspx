<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master" CodeBehind="MonitoraggioCE_Edit.aspx.vb" Inherits="AgroAgenda_2010.MonitoraggioCE_Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.ui.timepicker.js?" & Application("GiasVersioneCorrente").ToString) %>"> </script>
    <link href="../Styles/jquery.ui.timepicker.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <table style="width:100%; " aria-hidden="true">
        <tr>
            <td width="25%"></td>
            <td width="25%"></td>
            <td width="25%"></td>
            <td width="25%"></td>
        </tr>
        <tr>
            <td colspan="3">
                <table class="boxColore" width="100%" aria-hidden="true">
                    <tr>
                        <td colspan="2"><b>Caricamento dati Bolla di Accettazione</b></td>
                    </tr>
                    <tr>
                        <td colspan="2" style="width:100%; ">
                            <div style="float: left;" >
                                <asp:Label ID="Label1" runat="server" Width="130px"  >Stabilimento: </asp:Label>
                            </div>
                            <div>
                                <asp:dropdownlist id="Cmb_Magazzino" runat="server" Height="23px" Width="400px" CssClass="myCombo"></asp:dropdownlist>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label2" runat="server" Width="130px"  >N° Bolla di Accettazione: </asp:Label>
                            </div>
                            <div style="float: left; min-width:350px;" >
                                <asp:dropdownlist id="Cmb_Prefisso" runat="server" Width="64px" CssClass="myCombo"></asp:dropdownlist>
                                <asp:TextBox ID="txt_Numero_Bolla" runat="server"  Width="216px"  CssClass="txtUI" style="margin-left:10px;"></asp:TextBox>                                                              
                            </div>
                        </td>
                        <td style=" min-width:320px">
                            <div style="float: left;" >
                                <asp:Label ID="Label3" runat="server" Width="50"   >Anno: </asp:Label>
                            </div>
                            <div>
                                <asp:TextBox ID="Txt_Bolla_Anno" runat="server"  Width="75"  CssClass="txtUI" style="margin-left:10px;"></asp:TextBox>
                                <asp:Button ID="Btn_CaricaDatiBolla" runat="server" Text="Carica Dati Bolla" CssClass="btn_per_load" style="margin-left:10px" />
                            </div>                            
                        </td>
                    </tr>
                    <tr>
                        <td>
                             <div style="float: left;" >
                                <asp:Label ID="Label4" runat="server" Width="130px"  >N° DDT di Conferimento: </asp:Label>
                            </div>
                            <div style="float: left;  min-width:350px;" >
                                <asp:TextBox ID="Txt_DocNumeroSin_DDT" runat="server"  Width="50px"  CssClass="txtUI" ></asp:TextBox>
                                <asp:TextBox ID="Txt_DocNumero_DDT" runat="server"  Width="100px"  CssClass="txtUI" style="margin-left:10px;"></asp:TextBox>
                                <asp:TextBox ID="Txt_DocNumeroDes_DDT" runat="server"  Width="50"  CssClass="txtUI" style="margin-left:10px;"></asp:TextBox>                              
                            </div>
                        </td>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label5" runat="server"  Width="50"   >Data DDT: </asp:Label>
                            </div>
                            <div>
                                <asp:TextBox ID="Txt_Data_DDT" runat="server"  Width="75"  CssClass="txtUI" style="margin-left:10px;"></asp:TextBox>
                                <script type="text/javascript">
                                      $(function () {
                                          $("#<%= Txt_Data_DDT.ClientID %>").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
                                      });                       
                                </script>
                                <asp:Button ID="Btn_CaricaProduttori" runat="server" Text="Carica Produttori" CssClass="btn_per_load" style="margin-left:10px"  />
                                
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div style="float: left;" >
                                <asp:Label ID="Label18" runat="server" Width="130px">Produttori</asp:Label>
                            </div>
                            <div>
                                <asp:dropdownlist id="Cmb_Produttori" runat="server" Height="23px" Width="700px" CssClass="myCombo"></asp:dropdownlist>
                            </div>    
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <div style="float: left; width:100%; margin-left:15px;" >
                    <ASP:IMAGEBUTTON  style="float: left;" id="ImgBtnSalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" Height="32px"
                        Width="32px"  ToolTip="Premere il pulsante per salvare l'operazione"></ASP:IMAGEBUTTON>
                    <asp:radiobuttonlist  id="RBL_Salva" runat="server" CssClass="txtUI" BorderStyle="None" >
                        <asp:ListItem Value="0">Salva ed Esci</asp:ListItem>
                        <asp:ListItem Value="1" Selected="True">Salva e Continua</asp:ListItem>
                    </asp:radiobuttonlist>
                </div>
                <div style="clear: both" />

                <%-- Abbiamo deciso di nascondere il pulsante per creare i CE da dentro l'edit, 
                    perché cmq poi non vengono ricaricate le ddl, quindi di fatto per vedere le modifiche devo uscire dall'edit e rientrare  --%>
                <div style="float: left; width:100%; margin-left:15px; display: none" >
                    <ASP:IMAGEBUTTON id="ImgBtn_GestioneCE" runat="server" ImageUrl="~/AB_Immagini/Icone32/Insetto32_03.ico" Height="32px"
                        Width="32px" ></ASP:IMAGEBUTTON>
                    <asp:label id="Label19" runat="server"  Width="128px" CssClass="txtUI" BorderStyle="None" style=" margin:10px;"  >Inserisci / Modifica Corpi Estranei</asp:label>
                </div>
                
            </td>
        </tr>  
        <tr>    
            <td colspan="2">
                <table class="boxColore" aria-hidden="true">
                    <tr>
                        <td><b>Pesa</b></td>
                    </tr>
                    <tr>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label6" runat="server" Width="150px"  >Specie - Varieta' </asp:Label>
                            </div>
                            <div >
                                <asp:TextBox ID="Txt_Varieta" runat="server"  Width="450px"  CssClass="txtUI" ReadOnly="true" ></asp:TextBox>                           
                            </div>
                        </td>
                       
                    </tr>
                    <tr>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label7" runat="server" Width="150px"  >Produttore </asp:Label>
                            </div>
                            <div >
                                <asp:TextBox ID="Txt_Produttore" runat="server"  Width="450px"  CssClass="txtUI" ReadOnly="true" ></asp:TextBox>                           
                            </div>
                        </td>                        
                    </tr>
                    <tr>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label8" runat="server" Width="150px"  >Conferente </asp:Label>
                            </div>
                            <div >
                                <asp:TextBox ID="Txt_Conferente" runat="server"  Width="450px"  CssClass="txtUI" ReadOnly="true"></asp:TextBox>                           
                            </div>
                        </td>                      
                    </tr>
                    <tr>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label9" runat="server" Width="75px"  >Data Bolla </asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_DataBolla" runat="server"  Width="75px"  CssClass="txtUI" ReadOnly="true" ></asp:TextBox>                           
                            </div>
                            <div  style="float: left;" >
                                <asp:Label ID="Label10" runat="server" Width="75px" style="padding-left:10px"  >Kg </asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_Kg" runat="server"  Width="75px"  CssClass="txtUI" ReadOnly="true" ></asp:TextBox>                           
                            </div>
                            <div  style="float: left;" >
                                <asp:Label ID="Label11" runat="server" Width="75px" style="padding-left:10px"  >Data Arrivo </asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_DataArrivo" runat="server"  Width="75px"  CssClass="txtUI"  ></asp:TextBox>   
                                <script type="text/javascript">
                                      $(function () {
                                          $("#<%= Txt_DataArrivo.ClientID %>").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
                                      });                       
                                </script>                        
                            </div>
                            <div  style="float: left;" >
                                <asp:Label ID="Label12" runat="server" Width="75px" style="padding-left:10px"  >Carico N° </asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_Carico_N" runat="server"  Width="75px"  CssClass="txtUI"  ></asp:TextBox>                           
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <td colspan="2" style=" vertical-align:top; ">
                <table id="Pannello_Pesticidi" runat="server" class="boxColore" width="100%" aria-hidden="true">
                    <tr>
                        <td><b>Pesticidi</b></td>
                    </tr>
                    <tr>
                         <td>
                            <asp:RadioButtonList id=Rbl_Pesticidi  runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1">SI COOP</asp:ListItem>
                                <asp:ListItem Value="2" Selected="True">NO COOP</asp:ListItem>
                                <asp:ListItem Value="3">In Attesa di Analisi</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
                <table class="boxColore"  width="100%" style="margin-top:15px" aria-hidden="true">
                    <tr>
                        <td><b>Note</b></td>
                    </tr>
                    <tr>
                        <td >
                            <asp:TextBox ID="Txt_Note" TextMode="MultiLine" runat="server"  Width="500px"  CssClass="txtUI" ></asp:TextBox>      
                        </td>
                    </tr>
                </table>
                <div id="Pannello_Piazzale" style=" display:none; ">
                    <div style="float: left;" >
                        Punteggio <asp:TextBox ID="Txt_Punteggio" runat="server"  Width="75px"  CssClass="txtUI" ></asp:TextBox>                           
                    </div>
                    <div style="float: left;" >
                        Classifica <asp:TextBox ID="Txt_Classifica" runat="server"  Width="75px"  CssClass="txtUI" ></asp:TextBox>                           
                    </div>
                   
                </div>
            </td>           
        </tr>     
        <tr>
            <td colspan="4">
                <table class="boxColore" width="100%" aria-hidden="true">
                    <tr>
                        <td width="50%"></td>
                        <td width="50%"></td>
                    </tr>
                    <tr>
                        <td colspan="2"><b>Produzione</b></td>
                    </tr>
                    <tr>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label13" runat="server" Width="200px"  >Data Inizio Cottura</asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_Data_Inizio_Cottura" runat="server"  Width="75"  CssClass="txtUI" ></asp:TextBox>         
                                <script type="text/javascript">
                                      $(function () {
                                          $("#<%= Txt_Data_Inizio_Cottura.ClientID %>").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
                                      });                       
                                </script>                  
                            </div>
                            <div style="float: left;" >
                                <asp:Label ID="Label14" runat="server" Width="50px" style="margin-left:10px"  >Ora</asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_Ora_Inizio_Cottura" runat="server"  Width="75"  CssClass="txtUI" ></asp:TextBox>    
                                <script type="text/javascript">
                                    $(function () {
                                        $("#<%= Txt_Ora_Inizio_Cottura.ClientID %>").timepicker();
                                    });                       
                                </script>                       
                            </div>
                        </td>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label16" runat="server" Width="200px" >Confezione Marchio (1)</asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_Confezione_Marchio_1" runat="server"  Width="400"  CssClass="txtUI" ></asp:TextBox>                           
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                             <div style="float: left;" >
                                <asp:Label ID="Label15" runat="server" Width="200px" >Livello Qualitativo</asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_Livello_Qualitativo" runat="server"  Width="400"  CssClass="txtUI" ></asp:TextBox>                           
                            </div>
                        </td>
                        <td>
                            <div style="float: left;" >
                                <asp:Label ID="Label17" runat="server" Width="200px" >Confezione Marchio (2)</asp:Label>
                            </div>
                            <div style="float: left;" >
                                <asp:TextBox ID="Txt_Confezione_Marchio_2" runat="server"  Width="400"  CssClass="txtUI" ></asp:TextBox>                           
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="center"><b>LINEA 1</b></td>
                        <td align="center"><b>LINEA 2</b></td>
                    </tr>
                    <tr>
                        <td  align="center">
                            <table class="boxColore" aria-hidden="true">
                                <tr>
                                    <td>
                                        <div style="float: left;">Aereoseparatori&nbsp;</div>
                                        <asp:TextBox ID="Txt_N_AE_1" runat="server"  Width="50"  CssClass="txtUI" style="margin-left:10px" ></asp:TextBox>
                                        <asp:dropdownlist id="Cmb_TipoCE_AE_1" runat="server" Width="250px" style="margin-left:10px"  CssClass="myCombo"></asp:dropdownlist>
                                    </td>
                                </tr>
                                <tr>
                                    <td >
                                        <div style="float: left;"><ASP:LISTBOX id="List_AE_1"  runat="server" Height="72px" Width="342px" ></ASP:LISTBOX></div>
                                        <div>
                                            <ASP:IMAGEBUTTON id="ImgBtn_Ins_AE_1"  runat="server" ImageUrl="~/AB_Immagini/Icone32/Nuovo.ico" Height="32px" Width="32px" ToolTip="Aggiungi il nuovo elemento" style="margin-left:25px "></ASP:IMAGEBUTTON>
                                            <br />
                                            <ASP:IMAGEBUTTON id="ImgBtn_Del_AE_1"  runat="server" ImageUrl="~/AB_Immagini/Icone32/cestino.ico" Height="32px" Width="32px" ToolTip="Cancella l'elemento selezionato" style="margin-left:25px"></ASP:IMAGEBUTTON>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td  align="center">
                            <table class="boxColore" aria-hidden="true">
                                <tr>
                                    <td>
                                        <div style="float: left;">Aereoseparatori&nbsp;</div>
                                        <asp:TextBox ID="Txt_N_AE_2" runat="server"  Width="50"  CssClass="txtUI" style="margin-left:10px" ></asp:TextBox>
                                        <asp:dropdownlist id="Cmb_TipoCE_AE_2" runat="server" Width="250px" style="margin-left:10px"  CssClass="myCombo"></asp:dropdownlist>
                                    </td>
                                </tr>
                                <tr>
                                    <td >
                                        <div style="float: left;"><ASP:LISTBOX id=List_AE_2  runat="server" Height="72px" Width="342px" ></ASP:LISTBOX></div>
                                        <div>
                                            <ASP:IMAGEBUTTON id="ImgBtn_Ins_AE_2"  runat="server" ImageUrl="~/AB_Immagini/Icone32/Nuovo.ico" Height="32px" Width="32px" ToolTip="Aggiungi il nuovo elemento" style="margin-left:25px "></ASP:IMAGEBUTTON>
                                            <br />
                                            <ASP:IMAGEBUTTON id="ImgBtn_Del_AE_2"  runat="server" ImageUrl="~/AB_Immagini/Icone32/cestino.ico" Height="32px" Width="32px" ToolTip="Cancella l'elemento selezionato" style="margin-left:25px"></ASP:IMAGEBUTTON>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                     <tr>
                        <td  align="center">
                            <table class="boxColore" aria-hidden="true">
                                <tr>
                                    <td>
                                        <div style="float: left;">Cernitrice Ottica</div>
                                        <asp:TextBox ID="Txt_N_CO_1" runat="server"  Width="50"  CssClass="txtUI" style="margin-left:10px" ></asp:TextBox>
                                        <asp:dropdownlist id="Cmb_TipoCE_CO_1" runat="server" Width="250px" style="margin-left:10px"  CssClass="myCombo"></asp:dropdownlist>
                                    </td>
                                </tr>
                                <tr>
                                    <td >
                                        <div style="float: left;"><ASP:LISTBOX id="Lista_CO_1"  runat="server" Height="72px" Width="342px" ></ASP:LISTBOX></div>
                                        <div>
                                            <ASP:IMAGEBUTTON id="ImgBtn_Ins_CO_1"  runat="server" ImageUrl="~/AB_Immagini/Icone32/Nuovo.ico" Height="32px" Width="32px" ToolTip="Aggiungi il nuovo elemento" style="margin-left:25px "></ASP:IMAGEBUTTON>
                                            <br />
                                            <ASP:IMAGEBUTTON id="ImgBtn_Del_CO_1"  runat="server" ImageUrl="~/AB_Immagini/Icone32/cestino.ico" Height="32px" Width="32px" ToolTip="Cancella l'elemento selezionato" style="margin-left:25px"></ASP:IMAGEBUTTON>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td  align="center">
                            <table class="boxColore" aria-hidden="true">
                                <tr>
                                    <td>
                                        <div style="float: left;">Cernitrice Ottica</div>
                                        <asp:TextBox ID="Txt_N_CO_2" runat="server"  Width="50"  CssClass="txtUI" style="margin-left:10px" ></asp:TextBox>
                                        <asp:dropdownlist id="Cmb_TipoCE_CO_2" runat="server" Width="250px" style="margin-left:10px"  CssClass="myCombo"></asp:dropdownlist>
                                    </td>
                                </tr>
                                <tr>
                                    <td >
                                        <div style="float: left;"><ASP:LISTBOX id="Lista_CO_2"  runat="server" Height="72px" Width="342px" ></ASP:LISTBOX></div>
                                        <div>
                                            <ASP:IMAGEBUTTON id="ImgBtn_Ins_CO_2"  runat="server" ImageUrl="~/AB_Immagini/Icone32/Nuovo.ico" Height="32px" Width="32px" ToolTip="Aggiungi il nuovo elemento" style="margin-left:25px "></ASP:IMAGEBUTTON>
                                            <br />
                                            <ASP:IMAGEBUTTON id="ImgBtn_Del_CO_2"  runat="server" ImageUrl="~/AB_Immagini/Icone32/cestino.ico" Height="32px" Width="32px" ToolTip="Cancella l'elemento selezionato" style="margin-left:25px"></ASP:IMAGEBUTTON>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                     <tr>
                        <td  align="center">
                            <table class="boxColore" aria-hidden="true">
                                <tr>
                                    <td>
                                        <div style="float: left;">Cernita Manuale</div>
                                        <asp:TextBox ID="Txt_N_CM_1" runat="server"  Width="50"  CssClass="txtUI" style="margin-left:10px" ></asp:TextBox>
                                        <asp:dropdownlist id="Cmb_TipoCE_CM_1" runat="server" Width="250px" style="margin-left:10px"  CssClass="myCombo"></asp:dropdownlist>
                                    </td>
                                </tr>
                                <tr>
                                    <td >
                                        <div style="float: left;"><ASP:LISTBOX id="Lista_CM_1"  runat="server" Height="72px" Width="342px" ></ASP:LISTBOX></div>
                                        <div>
                                            <ASP:IMAGEBUTTON id="ImgBtn_Ins_CM_1"  runat="server" ImageUrl="~/AB_Immagini/Icone32/Nuovo.ico" Height="32px" Width="32px" ToolTip="Aggiungi il nuovo elemento" style="margin-left:25px "></ASP:IMAGEBUTTON>
                                            <br />
                                            <ASP:IMAGEBUTTON id="ImgBtn_Del_CM_1"  runat="server" ImageUrl="~/AB_Immagini/Icone32/cestino.ico" Height="32px" Width="32px" ToolTip="Cancella l'elemento selezionato" style="margin-left:25px"></ASP:IMAGEBUTTON>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td  align="center">
                            <table class="boxColore" aria-hidden="true">
                                <tr>
                                    <td>
                                        <div style="float: left;">Cernita Manuale</div>
                                        <asp:TextBox ID="Txt_N_CM_2" runat="server"  Width="50"  CssClass="txtUI" style="margin-left:10px" ></asp:TextBox>
                                        <asp:dropdownlist id="Cmb_TipoCE_CM_2" runat="server" Width="250px" style="margin-left:10px"  CssClass="myCombo"></asp:dropdownlist>
                                    </td>
                                </tr>
                                <tr>
                                    <td >
                                        <div style="float: left;"><ASP:LISTBOX id="Lista_CM_2"  runat="server" Height="72px" Width="342px" ></ASP:LISTBOX></div>
                                        <div>
                                            <ASP:IMAGEBUTTON id="ImgBtn_Ins_CM_2"  runat="server" ImageUrl="~/AB_Immagini/Icone32/Nuovo.ico" Height="32px" Width="32px" ToolTip="Aggiungi il nuovo elemento" style="margin-left:25px "></ASP:IMAGEBUTTON>
                                            <br />
                                            <ASP:IMAGEBUTTON id="ImgBtn_Del_CM_2"  runat="server" ImageUrl="~/AB_Immagini/Icone32/cestino.ico" Height="32px" Width="32px" ToolTip="Cancella l'elemento selezionato" style="margin-left:25px"></ASP:IMAGEBUTTON>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
           
        </tr> 
    </table>
    <div>
        <INPUT id="Txt_InsertCE" type="hidden" size="1" name="Txt_InsertCE" runat="server" />
    </div>
</asp:Content>
