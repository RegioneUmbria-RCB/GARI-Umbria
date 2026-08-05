<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Versione.aspx.vb" Inherits="AgronicaWebApiProfilatore.WebApiProfilatoreVersione.Versione" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" lang="en" >
<head id="Head1" runat="server">
    <title>Versione</title>
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<link href="Styles/AgronicaStyle.css" type="text/css" rel="stylesheet" />
</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<ASP:PANEL id="Pannello_Versione" style="Z-INDEX: 105; LEFT: 16px; OVERFLOW: auto; POSITION: absolute; TOP: 96px;"
				MS_POSITIONING="GridLayout" runat="server" Width="938px" Height="536px" BackColor="WhiteSmoke"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px" Visible="False" Design_Time_Lock="True">
				<table id="TabellaVersione" style="Z-INDEX: 106; LEFT: 16px; WIDTH: 874px; POSITION: absolute; TOP: 16px; HEIGHT: 34px"
					cellSpacing="5" cellPadding="3" width="874" border="0" aria-hidden="true" runat="server">
					<tr>
						<td></td>
					</tr>
				</table>
			</ASP:PANEL>
			<ASP:PANEL id="Pannello_Chiave" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 104; LEFT: 632px; OVERFLOW: auto; POSITION: absolute; TOP: 8px"
				MS_POSITIONING="GridLayout" runat="server" Width="320px" Height="72px" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<ASP:TEXTBOX id="Txt_ChiaveAccesso" style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 32px"
					tabIndex="11" runat="server" BackColor="PaleTurquoise" Height="15px" Width="232px" TextMode="Password"
					BorderStyle="None" MaxLength="25" CssClass="Testo_08_Blue"></ASP:TEXTBOX>
				<ASP:LABEL id="LABEL3" style="Z-INDEX: 122; LEFT: 16px; POSITION: absolute; TOP: 16px" runat="server"
					BackColor="#C0FFFF" Height="15px" Width="44px" CssClass="Testo_08_Nero_Bold">Password :</ASP:LABEL>
				<ASP:IMAGEBUTTON id="ImgBtn_Codice_Ins" style="Z-INDEX: 108; LEFT: 264px; POSITION: absolute; TOP: 16px"
					tabIndex="21" runat="server" Height="32px" Width="32px" ToolTip="Aggiungi il nuovo elemento"
					ImageUrl="App_Immagini/Varie/chiave.gif"></ASP:IMAGEBUTTON>
			</ASP:PANEL>
			<table id="TableTitolo" style="Z-INDEX: 101; LEFT: 16px; WIDTH: 608px; POSITION: absolute; TOP: 8px; HEIGHT: 41px"
				cellSpacing="1" cellPadding="1" width="608" border="0" aria-hidden="true" runat="server">
				<tr>
					<td style="WIDTH: 41px">
						<ASP:IMAGE id="ImgIcona" runat="server" 
                            ImageUrl="App_Immagini/icone32/stato.ico" Width="32px"
							Height="32px"></ASP:IMAGE></td>
					<td vAlign="middle" align="left" bgColor="#00bfff">&nbsp;
						<ASP:LABEL id="LblTitolo" runat="server" Width="306px" Height="7px" Font-Italic="True" BackColor="DeepSkyBlue"
							CssClass="Testo_12_Nero_Bold">Aggiornamenti & Implementazioni</ASP:LABEL></td>
				</tr>
			</table>
			<table id="TableTitolo2" style="Z-INDEX: 102; LEFT: 256px; WIDTH: 368px; POSITION: absolute; TOP: 56px; HEIGHT: 26px"
				cellSpacing="1" cellPadding="1" width="368" border="0" aria-hidden="true" runat="server">
				<tr>
					<td bgColor="#30dfef"></td>
				</tr>
			</table>
			<ASP:PANEL id="Panel1" style="Z-INDEX: 103; LEFT: 968px; POSITION: absolute; TOP: 8px" runat="server"
				Width="30px" Height="656px" Visible="False"></ASP:PANEL>
			<ASP:LABEL id="LABEL1" style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 56px" runat="server"
				Width="224px" Height="7px" Font-Italic="True" CssClass="Testo_12_Nero_Bold" ForeColor="Fuchsia"
				Font-Size="Large">Agronica WebApi Profilatore</ASP:LABEL>
		</form>
	</body>
</html>

