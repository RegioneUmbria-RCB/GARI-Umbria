<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Filtro_Stampe_Cespiti.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_Stampe_Cespiti"  MasterPageFile="~/Master/StampeBootstrap.Master" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <title>Filtro Stampe Cespiti</title>
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


            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    
					<asp:RadioButtonList id="Rbl_TipoStampa" runat="server"  CssClass="txtUI" AutoPostBack="true">
                    
					    <asp:ListItem Value="E" Selected="True">Civilistico e Fiscale</asp:ListItem>
					    <asp:ListItem Value="C">Civilistico</asp:ListItem>
                        <asp:ListItem Value="F">Fiscale</asp:ListItem>
                        
				        </asp:RadioButtonList>
                
                </div>
            </div>
            
            <div class="row" style="height:15px"></div>
 
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    				
                    <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon lbl_required" id="lbl_anno" for="txt_anno">Anno</span>
                                            
                                    <asp:TextBox ID="txt_anno" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                                                          
                                </div>
                            </div>
                    </div>
               
                </div>
            </div>


            <div class="row" style="height:15px"></div>
                                  
            <div style="clear: both;"></div>      
            <div class="row" style="height:25px"></div>
                       
               
       </div> 


</asp:Content>