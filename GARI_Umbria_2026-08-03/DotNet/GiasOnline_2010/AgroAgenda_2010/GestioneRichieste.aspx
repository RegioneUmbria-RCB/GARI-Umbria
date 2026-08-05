<%@ Page Language="vb" AutoEventWireup="false" Codebehind="GestioneRichieste.aspx.vb" 
Inherits="AgroAgenda_2010.GestioneRichieste" validateRequest="false" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>


<!DOCTYPE HTML>
<HTML lang="en">
	<HEAD>
		<title>GestioneRichieste</title>
	</HEAD>
	<body >
		<form id="Form1" method="post" runat="server">
			<asp:textbox id="TxtRisultato" 
				runat="server"  ></asp:textbox>&nbsp;


            <cc2:agronicabase id="AgronicaBase" runat="server" />

		</form>
	</body>
</HTML>
