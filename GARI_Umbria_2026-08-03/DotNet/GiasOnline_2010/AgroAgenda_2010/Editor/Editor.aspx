<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Editor.aspx.vb"
    Inherits="AgroAgenda_2010.Editor" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
    #exportPdf {
        margin: 0 0 10px 1px;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
	<!-- Filtri -->
	<div id="searchArea" class="panel-group searchArea" >
		<div class="panel-body" style="padding-top:5px;">
            <div class="row">
                <div Class="col-lg-12 col-md-12 col-sm-12">
                    <div Class="btn btn-success" id="btn_salva">
                            <span Class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
                        </div>
                    </div>  
                </div>    

                <div id = "editorArea" Class="panel-group editorArea">
		            <div style = "overflow: auto; margin-top: 10px; margin-bottom: 70px;" >
                        <textarea id="editorID" rows="10" cols="30" style="width:100%;height:400px">
                        </textarea>
                    </div>
	            </div>
   
	        </div>
    </div>

    <input type="hidden" id="hf_UtenteAbilitatoLettura" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura" runat="server" />
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdTesto" runat="server" />
    <input type="hidden" id="hdSa_Cod" runat="server" />
    <input type="hidden" id="hdCod_RisUm" runat="server" />
    <input type="hidden" id="hdElem_Cod" runat="server" />
    <input type="hidden" id="hdPro_Cod" runat="server" />
    <input type="hidden" id="hdMat_Cod" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cTesto = "#<%=hdTesto.ClientID() %>";

        var cSa_Cod = "#<%=hdSa_Cod.ClientID() %>";
        var cCod_RisUm = "#<%=hdCod_RisUm.ClientID() %>";
        var cElem_Cod = "#<%=hdElem_Cod.ClientID() %>";
        var cPro_Cod = "#<%=hdPro_Cod.ClientID() %>";
        var cMat_Cod = "#<%=hdMat_Cod.ClientID() %>";
 
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("editor_globali.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("editor.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("editor_ws_client.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("editor_jQueryDocReady.js") %>" ></script>

</asp:Content>