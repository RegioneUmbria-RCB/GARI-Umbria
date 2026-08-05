<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Stampe.Master" CodeBehind="Selezione_Quadro_P.aspx.vb" Inherits="AgronicaStampe_2010.Selezione_Quadro_P" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentStampeContenuti" runat="server">

<table aria-hidden="true">
    <tr>
        <td>           
            <table aria-hidden="true" class="boxColore">
                <tr>
                    <td style=" height:35px;">
                        E' stata richiesta la stampa del Quadro P per l'impresa:
                    </td>            
                </tr>   
                <tr>
                    <td id="Row_Piva" runat="server" style=" height:35px;">
                        Piva:
                    </td>            
                </tr> 
                <tr>
                    <td id="Row_RagSoc" runat="server" style=" height:35px;">
                        Ragione sociale:
                    </td>            
                </tr> 
                <tr>
                    <td id="Row_CentroAziendale" runat="server" style=" height:35px;">
                        In particolare per il Centro Aziendale:
                    </td>            
                </tr>
                <tr>
                    <td id="Row_NoteCentro" style=" height:35px;" runat="server">
                        Si tenga presente che il periodo di attivita' di tale centro aziendale e' impostato come:
                    </td>            
                </tr>
                <tr>
                    <td id="Row_InizioAttivita" runat="server" style=" height:35px;">
                        Inizio Attività:
                    </td>            
                </tr>
                <tr>
                    <td  id="Row_FineAttivita" runat="server" style=" height:35px;">
                        Fine Attività:
                    </td>            
                </tr>     
                <tr>
                    <td style=" height:35px;">
                        Impostando una data esterna all'intervallo, si potrebbe ottenere una stampa nulla ...
                    </td>            
                </tr>
            </table>
        </td>
        <td valign="top">
            <table aria-hidden="true" class="boxColore" style="height:100%;">
                <tr  valign="middle">
                    <td valign="middle">
                        <b style="margin-left: 5px;">Data di riferimento</b>:
                        
                    </td>
                    <td >
                        <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="txtui datepicker" MaxLength="10"
                            Style="margin-left: 5px;" ToolTip="Data di riferimento" Width="77px">
                        </asp:TextBox>
                    </td>
                </tr>   
                <tr><td colspan="2"> 
                            <asp:checkbox id="Chk_Centro" 
						            runat="server"  Text="Visualizza il Centro Aziendale"></asp:checkbox>
                    </td>
                </tr>     
                <tr><td colspan="2"> 
                            <asp:checkbox id="Chk_SoloOccupate" 
						            runat="server"  Text="Solo le particelle occupate"></asp:checkbox>
                    </td>
                </tr>        
                <tr valign="middle">                        
                    <td  style="padding-left: 5px;">
                        Stampa i dati selezionati
                    </td>
                    <td align="center" >
                        <ASP:IMAGEBUTTON id="ImgBtn_Stampa" 
						    runat="server"  ImageUrl="../../AB_Immagini/Icone32/stampa.ico"></ASP:IMAGEBUTTON>					        
                    </td>
                </tr>                 
            </table>
        </td>
    </tr>
</table>


</asp:Content>
