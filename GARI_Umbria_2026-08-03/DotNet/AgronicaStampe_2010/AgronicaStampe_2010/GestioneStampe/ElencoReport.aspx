<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Stampe.Master" CodeBehind="ElencoReport.aspx.vb" Inherits="AgronicaStampe_2010.ElencoReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentStampeContenuti" runat="server">
    
    <style type="text/css">
        .hiddencol {
            display: none;
        }
    </style>

    <div style="max-height: 400px; overflow: auto;">
        <asp:GridView ID="dgrReport" runat="server" AutoGenerateColumns="False"
            CellPadding="5" CssClass="ui-widget-content">
            <Columns>              
                <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="false" ItemStyle-CssClass="hiddencol"  HeaderStyle-CssClass="hiddencol">
                </asp:BoundField>
                <asp:BoundField DataField="Documento_Cod" HeaderText="Documento_Cod" ItemStyle-CssClass="hiddencol"  HeaderStyle-CssClass="hiddencol">
                </asp:BoundField>
                <asp:BoundField DataField="SottoCartella" HeaderText="SottoCartella" HtmlEncode="false" NullDisplayText="" ItemStyle-CssClass="hiddencol"  HeaderStyle-CssClass="hiddencol">
                </asp:BoundField>
                <asp:BoundField DataField="NomeFile" HeaderText="Report" HtmlEncode="false">
                </asp:BoundField>
                <asp:TemplateField HeaderText="Scarica">  
                    <ItemTemplate>  
                        <asp:ImageButton ID="lnkDownload" runat="server" OnClick="lnkDownload_Click" ImageUrl="../AB_Immagini/Icone32/FrecciaDN.ico">
                        </asp:ImageButton>
                    </ItemTemplate>  
                </asp:TemplateField>
                <asp:BoundField DataField="Inizio" HeaderText="Validita' Inizio">
                </asp:BoundField> 
                <asp:BoundField DataField="Fine" HeaderText="Validita' Fine">
                </asp:BoundField>                
            </Columns>
            <HeaderStyle CssClass="ui-widget-header" />
            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
        </asp:GridView>
    </div>
    <div id="ElencoVuoto" runat="server" visible="false">Non sono stati archiviati report per gli impianti selezionati.        
    </div>
</asp:Content>
