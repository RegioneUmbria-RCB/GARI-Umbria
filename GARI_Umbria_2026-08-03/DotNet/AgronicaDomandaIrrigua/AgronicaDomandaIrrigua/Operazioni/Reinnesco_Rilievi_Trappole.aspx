<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Operazione.master" CodeBehind="Reinnesco_Rilievi_Trappole.aspx.vb" ValidateRequest="false"
Inherits="AgronicaDomandaIrrigua.Reinnesco_Rilievi_Trappole" meta:resourcekey="PageResource1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentOperazioniHeader" runat="server">
    <style type="text/css">
        .classComboFertilizzanti .ui-autocomplete-input
        {
            min-width: 525px;
            width: 90%;
        }
    </style>
    <script type="text/javascript">



        function ChkSelezionaTrappola_Click(Oggetto) {
         
            //controllo che non ho inserito alcuna dose 
            //            if ($('.rigaImpianti').length == 0) {


            //            }
        }




        //per la Gestione dell'Eliminazione 
        function DoPostBack_Combo_Slave($_combo, valoreOpt) {
        }

       


        //per collegarsi al sito del profitosan
        function Info() {
        }

        function DoPostBack_ControlliSiNo(key) {
 
            if (key == 'Giacenze') {
 
                $("#<%=Giacenza_SI_NO.ClientID %>").val("OK");
                $("#<%=Master_Operazione.Property_ImgBtn_Salva.ClientID %>").click();
            }
        }

        function SelezionaDeselezionaTutteTrappole() {
            if ($('#chkSelezionaTutteLeTrappole').is(':checked')) {

                //seleziono tutto
                $('.ChkSelezionaTrappola').each(function () {
                    $(this).children('input').attr('checked', 'checked');
                    ChkSelezionaImpianto_Click($(this).children('input'));
                });
            }
            else {

                //seleziono tutto
                $('.ChkSelezionaTrappola').each(function () {
                    $(this).children('input').removeAttr('checked');
                    ChkSelezionaImpianto_Click($(this).children('input'));
                });
            }
        }

        function ChkSelezionaImpianto_Click(Oggetto) {
            //disabilito oi abilito text
  
        }

        ///////////////////////////////////////////
        /////////////ABILITA DISABIITA/////////////
        ///////////////////////////////////////////


        function AcquaTotChecked() {
        }
        function Abilita_Disabilita_ACQUA() {
        }
        function DoseHAChecked() {
        }

        function QtaTOTChecked() {
        }

        function Abilita_Disabilita_DOSI() {
        }

        
        /////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        ///////////////////////////////////
        ////////RICAVA INSERISCI///////////
        ///////////////////////////////////

        function SupTrattata() {
            
        }
        function InserisciSupTrattata(valore) {
           
        }

        function SupTotale() {
            
        }
        function InserisciSupTotale(valore) {
           
        }

        function AcquaTot() {
        }
        function InserisciAcquaTot(valore) {
        }

        function AcquaHA() {
        }
        function InserisciAcquaHA(valore) {
        }

        function DoseHA() {
        }
        function InserisciDoseHA(valore) {
        }

        function DoseHL() {
        }
        function InserisciDoseHL(valore) {
        }

        function TotHA() {
        }
        function InserisciTotHA(valore) {
        }

        function TotHL() {
        }
        function InserisciTotHL(valore) {
        }


        function AggiornaACQUA() {
        }


        function AggiornaDOSI() {
        }

        function AcquaTot_Keyup() {
        }
        function AcquaHA_Keyup() {
        }

        function AggiornaDopo_SupTrattata() {
            //Aggiorno i costi accessori
            CalcolaCostiAccessori();
        }


        function PulisciGrigliaAv_GrAv() {
        }

      
        $(document).ready(function () {
            //            $("#InserisciDose").button();
            //            $("#InserisciDose").click(function () { $("#<%=ImgBtn_DoseInserisci.ClientID %>").click(); });
        });

        
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlacexTRATTAMENTI" runat="server">
    <input type="hidden" id="HiddenVarie" runat="server" />
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentOperazioniContenuti" runat="server">
 

    <!-- hidden per si no  --> 
    <asp:UpdatePanel ID="updateGiacenze_si_no" runat="server"><ContentTemplate>
   <asp:HiddenField ID="Giacenza_SI_NO" runat="server" Value="0" />
   </ContentTemplate></asp:UpdatePanel>

                <div style="font-size: 10px; width: 100%; height: 58px;" align="center" class="sfondoverde" 
                    id="sfondoverde" runat="server">
                   

                        <asp:ImageButton ID="ImgBtn_DoseInserisci" runat="server" 
                            ImageUrl="~/AB_Immagini/icone32/frecciadn.ico" 
                            Style="width: 46px; display: none;" 
                            meta:resourcekey="ImgBtn_DoseInserisciResource1" />


                        <asp:UpdatePanel ID="updatepanelTrappoleMostra" runat="server">
                            <ContentTemplate>
                                <asp:ImageButton ID="ImgBtn_TrappoleMostra" runat="server" 
                                    ImageUrl="~/AB_Immagini/Icone16/FrecciaRossa_S.ico" 
                                    Style="width: 32px; display: none;" 
                                    meta:resourcekey="ImgBtn_TrappoleMostraResource1" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                            

                            <asp:ImageButton ID="ImageButton_Sblocca" runat="server" Height="23px" 
                                ImageUrl="~/AB_Immagini/Icone32/FrecciaUP.ico" Width="32px" 
                            meta:resourcekey="ImageButton_SbloccaResource1" />

                            <br />

                            <asp:Label ID="LabelMostraSlblocca" runat="server" 
                            Text="Mostra Le Trappole" meta:resourcekey="LabelMostraSlbloccaResource1" ></asp:Label>

                            <br />

                            <asp:ImageButton ID="ImageButton_MostraTrappole" runat="server" Height="23px" 
                                ImageUrl="~/AB_Immagini/Icone32/FrecciaDN.ico" Width="32px" 
                            meta:resourcekey="ImageButton_MostraTrappoleResource1" />
                            
                            <br />
                        
                    
                </div>


 
 <table width="100%" aria-hidden="true">
        <tr>
            <td colspan="3" align="left">
                <div style="font-size: 12px; width: 100%; " 
                    id="DivNoteTabella" runat="server" visible="False">
 
                <br />
                <asp:Label ID="Label5" runat="server" Text="Label" 
                        meta:resourcekey="Label5Resource1" ></asp:Label>
                <br />
                <asp:Label ID="Label1" runat="server" Text="Label" 
                        meta:resourcekey="Label1Resource1" ></asp:Label>
                <asp:Label ID="Label2" runat="server" Text="Label" 
                        meta:resourcekey="Label2Resource1" ></asp:Label> 
                <br />
                <asp:Label ID="Label3" runat="server" Text="Label" 
                        meta:resourcekey="Label3Resource1" ></asp:Label>
                <br />
                <asp:Label ID="Label4" runat="server" Text="Label" 
                        meta:resourcekey="Label4Resource1" ></asp:Label>
                                <br />
                <asp:Label ID="Label6" runat="server" Text="Label" 
                        meta:resourcekey="Label6Resource1" ></asp:Label>
                </div>
                <br />
            </td>
        </tr>
    </table>
   

   <asp:UpdatePanel ID="updatepanelGrid" runat ="server" ><ContentTemplate>

    <asp:GridView ID="GridView_TrappoleInstallate" runat="server" 
        AutoGenerateColumns="False" Width="100%"
                        CellPadding="5" CssClass="ui-widget-content" 
                        
                        
        Caption="Selezionare le Trappole da Reinnescare " 
           meta:resourcekey="GridView_TrappoleInstallateResource1">
        <Columns>
         
            <asp:BoundField DataField="Sa_nome" HeaderText="Centro Aziendale" 
                                SortExpression="Sa_nome" 
                meta:resourcekey="BoundFieldResource1"></asp:BoundField>
            <asp:BoundField DataField="Veg_Des" HeaderText="Specie" 
                                SortExpression="Veg_Des" 
                meta:resourcekey="BoundFieldResource2"></asp:BoundField>
            <asp:BoundField DataField="Cul_Des" HeaderText="Varietà" 
                                SortExpression="Cul_Des" 
                meta:resourcekey="BoundFieldResource3"></asp:BoundField>
            <asp:BoundField DataField="App_nome" HeaderText="Appezz. di Installazione" 
                                SortExpression="App_nome" 
                meta:resourcekey="BoundFieldResource4"></asp:BoundField>

             <asp:BoundField DataField="Trap_Num" HeaderText="Codice Trappola" 
                                SortExpression="Trap_Num" 
                meta:resourcekey="BoundFieldResource5"></asp:BoundField>
            <asp:BoundField DataField="Freatimetro" HeaderText="Codice Personale" 
                                SortExpression="Freatimetro" 
                meta:resourcekey="BoundFieldResource6"></asp:BoundField>
            <asp:BoundField DataField="Trap_Des" HeaderText="Descrizione Trappola" 
                                SortExpression="Trap_Des" 
                meta:resourcekey="BoundFieldResource7"></asp:BoundField>
            <asp:BoundField DataField="Sigla_Av" HeaderText="Avversita" 
                                SortExpression="Sigla_Av" 
                meta:resourcekey="BoundFieldResource8"></asp:BoundField>
            <asp:BoundField DataField="Uso_Desc" HeaderText="Uso Trappola" 
                                SortExpression="Uso_Desc" 
                meta:resourcekey="BoundFieldResource9"></asp:BoundField>
            <asp:BoundField DataField="Validita_Inizio" HeaderText="Data Installazione" 
                                SortExpression="Validita_Inizio" 
                meta:resourcekey="BoundFieldResource10"></asp:BoundField>      
            <asp:BoundField DataField="Dose" HeaderText="Inneschi alla Installazione" 
                                SortExpression="Dose" 
                meta:resourcekey="BoundFieldResource11"></asp:BoundField>
                                  
            <asp:BoundField DataField="Data_Reinnesco" HeaderText="Data Ultimo Innesco" 
                                SortExpression="Data_Reinnesco" 
                meta:resourcekey="BoundFieldResource12"></asp:BoundField>
            <asp:BoundField DataField="Reinnesco_Dose" HeaderText="Inneschi al Reinnesco" 
                                SortExpression="Reinnesco_Dose" 
                meta:resourcekey="BoundFieldResource13"></asp:BoundField>

            <asp:BoundField DataField="Scadenza" HeaderText="Scadenza" 
                                SortExpression="Scadenza" 
                meta:resourcekey="BoundFieldResource14"></asp:BoundField>

            <asp:BoundField DataField="Inneschi_Attivi" HeaderText="Inneschi Attivi alla data Operazione" 
                                SortExpression="Inneschi_Attivi" 
                meta:resourcekey="BoundFieldResource15"></asp:BoundField>

           <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                <HeaderTemplate>
                    <input type="checkbox" id="chkSelezionaTutteLeTrappole" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="ChkSelezionaTrappola" runat="server" 
                                        CssClass="ChkSelezionaTrappola" 
                        meta:resourcekey="ChkSelezionaTrappolaResource1" />
                </ItemTemplate>
                <HeaderStyle Width="20px" />
                <ItemStyle Width="20px" />
                <FooterStyle Width="20px" />
                <ControlStyle Width="20px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Reinneschi" SortExpression="Reinneschi" 
                meta:resourcekey="TemplateFieldResource2">
                <ItemTemplate>
                   <asp:TextBox ID="Txt_Reinnesco" runat="server" CssClass="Txt_Reinnesco" Style="width: 60px"
                                        Text="0" meta:resourcekey="Txt_ReinnescoResource1" />
                </ItemTemplate>
            </asp:TemplateField>

             <asp:BoundField DataField="Giacenza" HeaderText="Giacenza Inneschi" 
                 SortExpression="Giacenza" meta:resourcekey="BoundFieldResource16"></asp:BoundField>



            <asp:BoundField DataField="Qta2" HeaderText="Qta2" SortExpression="Qta2" Visible="false" />



        </Columns>
        <HeaderStyle CssClass="ui-widget-header" />
        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
    </asp:GridView>

       <asp:GridView ID="GridView_Reinneschi" runat="server" 
        AutoGenerateColumns="False" Width="100%"
                        CellPadding="5" CssClass="ui-widget-content" 
                        
                        
        Caption="Reinneschi effettuati nell'operazione colturale " 
           meta:resourcekey="GridView_ReinneschiResource1">
        <Columns>
         
            <asp:BoundField DataField="Sa_nome" HeaderText="Centro Aziendale" 
                                SortExpression="Sa_nome" 
                meta:resourcekey="BoundFieldResource17"></asp:BoundField>
            <asp:BoundField DataField="Veg_Des" HeaderText="Specie" 
                                SortExpression="Veg_Des" 
                meta:resourcekey="BoundFieldResource18"></asp:BoundField>
            <asp:BoundField DataField="Cul_Des" HeaderText="Varietà" 
                                SortExpression="Cul_Des" 
                meta:resourcekey="BoundFieldResource19"></asp:BoundField>
            <asp:BoundField DataField="App_nome" HeaderText="Appezz. di Installazione" 
                                SortExpression="App_nome" 
                meta:resourcekey="BoundFieldResource20"></asp:BoundField>

             <asp:BoundField DataField="Trap_Num" HeaderText="Codice Trappola" 
                                SortExpression="Trap_Num" 
                meta:resourcekey="BoundFieldResource21"></asp:BoundField>
            <asp:BoundField DataField="Freatimetro" HeaderText="Codice Personale" 
                                SortExpression="Freatimetro" 
                meta:resourcekey="BoundFieldResource22"></asp:BoundField>
            <asp:BoundField DataField="Trap_Des" HeaderText="Descrizione Trappola" 
                                SortExpression="Trap_Des" 
                meta:resourcekey="BoundFieldResource23"></asp:BoundField>
            <asp:BoundField DataField="Sigla_Av" HeaderText="Avversita" 
                                SortExpression="Sigla_Av" 
                meta:resourcekey="BoundFieldResource24"></asp:BoundField>
            <asp:BoundField DataField="Uso_Desc" HeaderText="Uso Trappola" 
                                SortExpression="Uso_Desc" 
                meta:resourcekey="BoundFieldResource25"></asp:BoundField>
            <asp:BoundField DataField="Validita_Inizio" HeaderText="Data Installazione" 
                                SortExpression="Validita_Inizio" 
                meta:resourcekey="BoundFieldResource26"></asp:BoundField>      
                                  
            <asp:BoundField DataField="Data_Reinnesco" HeaderText="Data Reinnesco" 
                                SortExpression="Data_Reinnesco" 
                meta:resourcekey="BoundFieldResource27"></asp:BoundField>

            <asp:BoundField DataField="Scadenza" HeaderText="Scadenza" 
                                SortExpression="Scadenza" 
                meta:resourcekey="BoundFieldResource28"></asp:BoundField>

            <asp:TemplateField HeaderText="Reinneschi" SortExpression="Reinneschi" 
                meta:resourcekey="TemplateFieldResource3">
                <ItemTemplate>
                   <asp:TextBox ID="Txt_Reinnesco" runat="server" CssClass="Txt_Reinnesco" Style="width: 60px"
                                        Text='<%# Eval("Reinneschi") %>' 
                        meta:resourcekey="Txt_ReinnescoResource2"/>
                </ItemTemplate>
            </asp:TemplateField>

             <asp:BoundField DataField="Giacenza" HeaderText="Giacenza Inneschi" 
                 SortExpression="Giacenza" meta:resourcekey="BoundFieldResource29"></asp:BoundField>



            <asp:BoundField DataField="Qta2" HeaderText="Qta2" SortExpression="Qta2" Visible="false" />



        </Columns>
        <HeaderStyle CssClass="ui-widget-header" />
        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
    </asp:GridView>
    </ContentTemplate></asp:UpdatePanel>

</asp:Content>

