<%@ Page Title="" Language="vb" AutoEventWireup="false" Codebehind="ExcelAutomatico_XLS.aspx.vb" Inherits="AgronicaStampe_2010.ExcelAutomatico_XLS"  %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
	<HEAD>
		<title>Excel Automatico</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<%-- eliminato il runat=server per evitare l'inserimento del view state e di una riga vuota nell'Excel --%>
		<form id="Form1" method="post" runat="server">
			<TABLE id="TableDati" style="Z-INDEX: 101; POSITION: absolute; TOP: 8px; LEFT: 8px" cellSpacing="1" 
				cellPadding="1" runat="server">
			</TABLE>
		</form>
	</body>
</HTML>
