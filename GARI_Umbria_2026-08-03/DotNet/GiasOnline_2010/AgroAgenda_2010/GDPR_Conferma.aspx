<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="GDPR_Conferma.aspx.vb" Inherits="AgroAgenda_2010.GDPR_Conferma" %>


<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    

    <div class="row" style="margin-bottom: 80px;">

        <div id="cont" class="row jumbotron border_si xonne-default-text">

            <div class="row">

                <div class="col-lg-12">

                   
                    <asp:PlaceHolder ID="phInfo" runat="server" />
                    <asp:HiddenField ID="hdVersioneGDPR" runat="server" />

                </div>
            </div>
            <div class="row" style="margin-top: 10px">

                <div class="col-lg-12">

                    <asp:Button class="btn btn-success xonne-btn-primary" ID="conferma" runat="server" Text="Procedi" style="width: 200px" />

                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

   

</asp:Content>

