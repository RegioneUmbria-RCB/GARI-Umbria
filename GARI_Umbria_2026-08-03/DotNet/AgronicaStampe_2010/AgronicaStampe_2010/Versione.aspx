<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Versione.aspx.vb" Inherits="AgronicaStampe_2010.StampeVersione.Versione" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Versione</title>
    <LINK href="App_Scripts/CSS/AgronicaStyle.css" type="text/css" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
    <ASP:PANEL id="Pannello_Versione" style="Z-INDEX: 105; POSITION: absolute; OVERFLOW: auto; TOP: 96px; LEFT: 16px;"
				MS_POSITIONING="GridLayout" runat="server" Width="938px" Height="536px" BackColor="WhiteSmoke"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px" Visible="False" Design_Time_Lock="True">
				<table aria-hidden="true" style="Z-INDEX: 106; POSITION: absolute; WIDTH: 874px; HEIGHT: 34px; TOP: 16px; LEFT: 16px"
					id="TabellaVersione" border="0" cellSpacing="5" cellPadding="3" width="874" runat="server">
					<TR>
						<TD></TD>
					</TR>
				</TABLE>
			</ASP:PANEL>
			<ASP:PANEL id="Pannello_Chiave" style="Z-INDEX: 104; POSITION: absolute; SCROLLBAR-FACE-COLOR: #afeeee; OVERFLOW: auto; TOP: 8px; LEFT: 632px"
				MS_POSITIONING="GridLayout" runat="server" Width="320px" Height="72px" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<ASP:TEXTBOX style="Z-INDEX: 105; POSITION: absolute; TOP: 32px; LEFT: 16px" id="Txt_ChiaveAccesso"
					tabIndex="11" runat="server" BackColor="PaleTurquoise" Height="15px" Width="232px" CssClass="Testo_08_Blue"
					MaxLength="25" BorderStyle="None" TextMode="Password"></ASP:TEXTBOX>
				<ASP:LABEL style="Z-INDEX: 122; POSITION: absolute; TOP: 16px; LEFT: 16px" id="LABEL3" runat="server"
					BackColor="#C0FFFF" Height="15px" Width="44px" CssClass="Testo_08_Nero_Bold">Password :</ASP:LABEL>
				<ASP:IMAGEBUTTON style="Z-INDEX: 108; POSITION: absolute; TOP: 16px; LEFT: 264px" id="ImgBtn_Codice_Ins"
					tabIndex="21" runat="server" Height="32px" Width="32px" ImageUrl="AB_Immagini/Varie/chiave.gif"
					ToolTip="Aggiungi il nuovo elemento"></ASP:IMAGEBUTTON>
			</ASP:PANEL>
			<table aria-hidden="true" id="TableTitolo" style="Z-INDEX: 101; POSITION: absolute; WIDTH: 608px; HEIGHT: 41px; TOP: 8px; LEFT: 16px"
				cellSpacing="1" cellPadding="1" width="608" border="0" runat="server">
				<TR>
					<TD style="WIDTH: 41px">
						<ASP:IMAGE id="ImgIcona" runat="server" ImageUrl="AB_Immagini/icone32/stato.ico" Width="32px"
							Height="32px"></ASP:IMAGE></TD>
					<TD vAlign="middle" align="left" bgColor="#00bfff">&nbsp;
						<ASP:LABEL id="LblTitolo" runat="server" Width="306px" Height="7px" Font-Italic="True" BackColor="DeepSkyBlue"
							CssClass="Testo_12_Nero_Bold">Aggiornamenti & Implementazioni</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<table aria-hidden="true" id="TableTitolo2" style="Z-INDEX: 102; POSITION: absolute; WIDTH: 368px; HEIGHT: 26px; TOP: 56px; LEFT: 256px"
				cellSpacing="1" cellPadding="1" width="368" border="0" runat="server">
				<TR>
					<TD bgColor="#30dfef"></TD>
				</TR>
			</TABLE>
			<ASP:PANEL id="Panel1" style="Z-INDEX: 103; POSITION: absolute; TOP: 8px; LEFT: 968px" runat="server"
				Width="30px" Height="656px" Visible="False"></ASP:PANEL>
			<ASP:LABEL id="LABEL1" style="Z-INDEX: 107; POSITION: absolute; TOP: 56px; LEFT: 16px" runat="server"
				Width="224px" Height="7px" Font-Italic="True" CssClass="Testo_12_Nero_Bold" ForeColor="Fuchsia"
				Font-Size="Large">AgronicaStampe</ASP:LABEL>
    </form>
</body>
</html>
