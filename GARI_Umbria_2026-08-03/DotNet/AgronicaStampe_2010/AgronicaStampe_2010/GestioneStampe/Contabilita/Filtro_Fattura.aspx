<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Filtro_Fattura.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_Fattura" MasterPageFile="~/Master/StampeBootstrap.Master" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <title>Filtro Stampa Fattura/DDT</title>
    <style>#ui-datepicker-div { Z-INDEX: 10000 }
		</style>		
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
        });
		</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
       <div class="container" >
            
            <div class="row">
                <div class="col-lg-10"></div>
                
                <div class="col-lg-2" style="float: right; margin-right: 10px; text-align: right;">
                    <asp:ImageButton id="ImgBtn_Stampa" runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico" ></asp:ImageButton>
				</div>
            </div>


            <div class="row" id ="Riga_TipoView" runat="server">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    
                    <asp:Label ID="Label1" runat="server" Text="Visualizzazione prodotti:"></asp:Label>
					<asp:RadioButtonList id="Rbl_TipoView" runat="server"  CssClass="txtUI" AutoPostBack="true">
                    
					    <asp:ListItem Value="N" Selected="True">Standard</asp:ListItem>
					    <asp:ListItem Value="P">Ragguppa per prodotto (Semilavorati o Trasformati Vegetali)</asp:ListItem>
                                             
				        </asp:RadioButtonList>
                
                </div>
            </div>
           
            
            <div class="row" style="height:15px"></div>
 
            
            <div class="row" id ="Riga_TipoOutput" runat="server">
                <div class="col-lg-12 col-md-12 col-sm-12">
                     
                    <asp:Label ID="Label2" runat="server" Text="Layout:"></asp:Label>                           
					<asp:RadioButtonList id="Rbl_TipoOutput" runat="server"  CssClass="txtUI" AutoPostBack="true">
                    
					    <asp:ListItem Value="P" Selected="True">PDF</asp:ListItem>
					    <asp:ListItem Value="C">Carta intestata</asp:ListItem>
                                             
				        </asp:RadioButtonList>
                
                </div>
            </div>


            <div class="row" style="height:15px"></div>
                                  
            <div style="clear: both;"></div>      
            <div class="row" style="height:25px"></div>
                       
               
       </div> 


</asp:Content>
