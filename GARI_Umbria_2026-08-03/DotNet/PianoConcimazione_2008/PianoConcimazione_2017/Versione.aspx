<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Versione.aspx.vb" Inherits="PianoConcimazione_2017.PianoConcimazioneVersione.Versione" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML lang="en">
	<HEAD>
		<title>Versione</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="/giasbase/AA_Script/CSS/AgronicaStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<ASP:PANEL id="Pannello_Versione" style="Z-INDEX: 105; LEFT: 16px; OVERFLOW: auto; POSITION: absolute; TOP: 96px;"
				MS_POSITIONING="GridLayout" runat="server" Width="938px" Height="536px" BackColor="WhiteSmoke"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px" Visible="False" Design_Time_Lock="True">
				<TABLE aria-hidden="true" id="TabellaVersione" style="Z-INDEX: 106; LEFT: 16px; WIDTH: 874px; POSITION: absolute; TOP: 16px; HEIGHT: 34px"
					cellSpacing="5" cellPadding="3" width="874" border="0" runat="server">
					<TR>
						<TD></TD>
					</TR>
				</TABLE>
			</ASP:PANEL>
			<ASP:PANEL id="Pannello_Chiave" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 104; LEFT: 632px; OVERFLOW: auto; POSITION: absolute; TOP: 8px"
				MS_POSITIONING="GridLayout" runat="server" Width="320px" Height="72px" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<ASP:TEXTBOX id="Txt_ChiaveAccesso" style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 32px"
					tabIndex="11" runat="server" BackColor="PaleTurquoise" Height="15px" Width="232px" TextMode="Password"
					BorderStyle="None" MaxLength="20" CssClass="Testo_08_Blue"></ASP:TEXTBOX>
				<ASP:LABEL id="LABEL3" style="Z-INDEX: 122; LEFT: 16px; POSITION: absolute; TOP: 16px" runat="server"
					BackColor="#C0FFFF" Height="15px" Width="44px" CssClass="Testo_08_Nero_Bold">Password :</ASP:LABEL>
				<ASP:IMAGEBUTTON id="ImgBtn_Codice_Ins" style="Z-INDEX: 108; LEFT: 264px; POSITION: absolute; TOP: 16px"
					tabIndex="21" runat="server" Height="32px" Width="32px" ToolTip="Aggiungi il nuovo elemento"
					ImageUrl="/giasbase/AB_Immagini/Varie/chiave.gif"></ASP:IMAGEBUTTON>
			</ASP:PANEL>
			<TABLE aria-hidden="true" id="TableTitolo" style="Z-INDEX: 101; LEFT: 16px; WIDTH: 608px; POSITION: absolute; TOP: 8px; HEIGHT: 41px"
				cellSpacing="1" cellPadding="1" width="608" border="0" runat="server">
				<TR>
					<TD style="WIDTH: 41px">
						<ASP:IMAGE id="ImgIcona" runat="server" ImageUrl="/giasbase/AB_Immagini/icone32/stato.ico" Width="32px"
							Height="32px"></ASP:IMAGE></TD>
					<TD vAlign="middle" align="left" bgColor="#00bfff">&nbsp;
						<ASP:LABEL id="LblTitolo" runat="server" Width="306px" Height="7px" Font-Italic="True" BackColor="DeepSkyBlue"
							CssClass="Testo_12_Nero_Bold">Aggiornamenti & Implementazioni</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<TABLE aria-hidden="true" id="TableTitolo2" style="Z-INDEX: 102; LEFT: 256px; WIDTH: 368px; POSITION: absolute; TOP: 56px; HEIGHT: 26px"
				cellSpacing="1" cellPadding="1" width="368" border="0" runat="server">
				<TR>
					<TD bgColor="#30dfef"></TD>
				</TR>
			</TABLE>
			<ASP:PANEL id="Panel1" style="Z-INDEX: 103; LEFT: 968px; POSITION: absolute; TOP: 8px" runat="server"
				Width="30px" Height="656px" Visible="False"></ASP:PANEL>
			<ASP:LABEL id="LABEL1" style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 56px" runat="server"
				Width="224px" Height="7px" Font-Italic="True" CssClass="Testo_12_Nero_Bold" ForeColor="Red"
				Font-Size="Large">PianoConcimazione</ASP:LABEL>
		</form>
	</body>
</HTML>
