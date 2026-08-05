<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Filtro_SchedaTracciabilita.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_SchedaTracciabilita" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
	<HEAD>
		<title>Filtro Scheda Tracciabilita</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../../App_Scripts/CSS/AgronicaStyle.css" type="text/css" rel="stylesheet">
		<SCRIPT language="vbscript">
			sub BtnDataInizio_OnClick()
				a = window.showModalDialog("../../AA_Script/Controlli/AgroCalendario/AgroCalendario.aspx?dsel=" & document.all("TxtValiditaInizio").value,"","dialogWidth:280px;dialogHeight:350px;status:no; center:yes;edge:raised; help:no;")
				'assegno il valore di ritorno della finestra modale
				if a<>"" then
					if a="-1" then
						document.all("TxtValiditaInizio").value = ""
					else
						document.all("TxtValiditaInizio").value = a
					end if					
				end if
			end sub
			sub BtnDataFine_OnClick()
				a = window.showModalDialog("../../AA_Script/Controlli/AgroCalendario/AgroCalendario.aspx?dsel=" & document.all("TxtValiditaFine").value,"","dialogWidth:280px;dialogHeight:350px;status:no; center:yes;edge:raised; help:no;")
				'assegno il valore di ritorno della finestra modale
				if a<>"" then
					if a="-1" then
						document.all("TxtValiditaFine").value = ""
					else
						document.all("TxtValiditaFine").value = a
					end if					
				end if
			end sub
		</SCRIPT>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<ASP:IMAGE id="ImageLogo" style="Z-INDEX: 100; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				ImageUrl="../../AB_Immagini/logo/Logo_GiasOnline_Mini.jpg" Height="73px" Width="96px"></ASP:IMAGE>
			<ASP:LABEL id="LABEL3" style="Z-INDEX: 109; LEFT: 128px; POSITION: absolute; TOP: 112px" runat="server"
				Width="168px" Height="15px" BackColor="#C0FFC0" CssClass="Testo_08_Rosso_Bold" ForeColor="Green">&nbsp;Opzioni per la stampa :</ASP:LABEL>
			<ASP:PANEL id="Pannello_Filtri" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 108; LEFT: 128px; OVERFLOW: auto; POSITION: absolute; TOP: 128px"
				MS_POSITIONING="GridLayout" runat="server" Width="302px" Height="256px" BORDERWIDTH="2px"
				BORDERCOLOR="#0000C0" BORDERSTYLE="Solid" BackColor="#C0FFFF">
				<asp:checkbox id="Chk_VisualizzaTipologieVarietali" style="Z-INDEX: 100; LEFT: 24px; POSITION: absolute; TOP: 16px"
					runat="server" Width="192px" CssClass="Testo_08_Blue" BackColor="#C0FFFF" Text="Visualizza Tipologie Varietali"></asp:checkbox>
				<ASP:LABEL id="LABEL16" style="Z-INDEX: 101; LEFT: 16px; POSITION: absolute; TOP: 56px" runat="server"
					Width="242px" Height="15px" CssClass="Testo_08_Nero_Bold" BackColor="#C0FFFF">Seleziona il tipo di arrotondamento :</ASP:LABEL>
				<ASP:RADIOBUTTONLIST id="Rbl_Arrotondamento" style="Z-INDEX: 103; LEFT: 24px; POSITION: absolute; TOP: 112px"
					runat="server" Width="170px" Height="48px" CssClass="Testo_08_Blue" RepeatLayout="Flow" BACKCOLOR="#C0FFFF">
					<asp:ListItem Value="-1">Nessuno</asp:ListItem>
					<asp:ListItem Value="0">Unit&#224;</asp:ListItem>
					<asp:ListItem Value="1">1 Decimale</asp:ListItem>
					<asp:ListItem Value="2">2 Decimali</asp:ListItem>
					<asp:ListItem Value="3" Selected="True">3 Decimali</asp:ListItem>
					<asp:ListItem Value="4">4 Decimali</asp:ListItem>
				</ASP:RADIOBUTTONLIST>
				<ASP:LABEL id="LABEL4" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 72px" runat="server"
					Width="242px" Height="15px" ForeColor="Green" CssClass="Testo_08_Nero" BackColor="#C0FFFF">(per la quantità di prodotto utilizzata nelle operazioni colturali)</ASP:LABEL>
			</ASP:PANEL>
			<ASP:PANEL id="Pannello_Stampa" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 107; LEFT: 528px; OVERFLOW: auto; POSITION: absolute; TOP: 320px"
				MS_POSITIONING="GridLayout" runat="server" Height="64px" Width="302px" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<ASP:IMAGEBUTTON id="ImgBtn_Stampa" style="Z-INDEX: 101; LEFT: 40px; POSITION: absolute; TOP: 16px"
					runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico" BackColor="#C0FFFF"></ASP:IMAGEBUTTON>
				<ASP:LABEL id="LABEL12" style="Z-INDEX: 102; LEFT: 96px; POSITION: absolute; TOP: 24px" runat="server"
					Width="166px" Height="15px" CssClass="Testo_08_Rosso_Bold" BackColor="#C0FFFF">Stampa i dati selezionati</ASP:LABEL>
			</ASP:PANEL>
			<ASP:PANEL id="Pannello_Data" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 106; LEFT: 528px; OVERFLOW: auto; POSITION: absolute; TOP: 128px"
				MS_POSITIONING="GridLayout" runat="server" Height="128px" Width="305px" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<ASP:PANEL id="Pannello_Intervallo" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 111; LEFT: 24px; OVERFLOW: auto; POSITION: absolute; TOP: 16px"
					MS_POSITIONING="GridLayout" runat="server" Width="256px" Height="88px" BackColor="#C0FFFF">&nbsp;&nbsp; 
<asp:Label id="LblDA" style="Z-INDEX: 108; LEFT: 24px; POSITION: absolute; TOP: 16px" runat="server"
						Width="32px" Height="16px" CssClass="Testo_08_Nero_Bold" BackColor="#C0FFFF"
						ToolTip="Data iniziale dell'intervallo in cui verranno stampati solo gli impianti attivi in quell'intervallo">DA :</asp:Label>
<ASP:TEXTBOX id="TxtValiditaInizio" style="Z-INDEX: 108; LEFT: 64px; POSITION: absolute; TOP: 8px"
						runat="server" Width="120px" Height="24px" CssClass="Testo_12_Blue_Bold"
						BackColor="PaleTurquoise" ToolTip="Data iniziale dell'intervallo in cui verranno stampati solo gli impianti attivi in quell'intervallo"
						BorderStyle="None" MaxLength="10"></ASP:TEXTBOX><INPUT class="Testo_08_Nero_Bold" id="BtnDataInizio" style="display:none; Z-INDEX: 108; LEFT: 216px; POSITION: absolute; TOP: 16px; HEIGHT: 17px"
						type="button" value="..." name="BtnDataInizio" Width="25px" Height="17px"> 
<asp:Label id="LblA" style="Z-INDEX: 108; LEFT: 24px; POSITION: absolute; TOP: 56px" runat="server"
						Width="32px" Height="16px" CssClass="Testo_08_Nero_Bold" BackColor="#C0FFFF"
						ToolTip="Data finale dell'intervallo in cui verranno stampati solo gli impianti attivi in quell'intervallo">A :</asp:Label>
<ASP:TEXTBOX id="TxtValiditaFine" style="Z-INDEX: 107; LEFT: 64px; POSITION: absolute; TOP: 48px"
						runat="server" Width="120px" Height="24px" CssClass="Testo_12_Blue_Bold"
						BackColor="PaleTurquoise" ToolTip="Data finale dell'intervallo in cui verranno stampati solo gli impianti attivi in quell'intervallo"
						BorderStyle="None" MaxLength="10"></ASP:TEXTBOX><INPUT class="Testo_08_Nero_Bold" id="BtnDataFine" style="display:none; Z-INDEX: 106; LEFT: 216px; POSITION: absolute; TOP: 56px; HEIGHT: 17px"
						type="button" value="..." name="BtnDataFine" Width="25px" Height="17px"></ASP:PANEL>
			</ASP:PANEL>
			<ASP:LABEL id="LABEL2" style="Z-INDEX: 105; LEFT: 528px; POSITION: absolute; TOP: 112px" runat="server"
				Height="15px" Width="232px" CssClass="Testo_08_Rosso_Bold" BackColor="#C0FFC0" ForeColor="Green">&nbsp;Date di riferimento per la stampa :</ASP:LABEL>
			<table aria-hidden="true" id="Table2" style="Z-INDEX: 101; LEFT: 128px; WIDTH: 780px; POSITION: absolute; TOP: 16px; HEIGHT: 41px"
				cellSpacing="1" cellPadding="1" width="780" border="0">
				<TR>
					<TD style="WIDTH: 35px"><ASP:IMAGE id="IMAGE1" runat="server" ImageUrl="../../AB_Immagini/Icone32/stampa.ico" Height="32px"
							Width="32px"></ASP:IMAGE></TD>
					<TD vAlign="middle" align="left" bgColor="#00bfff">&nbsp;&nbsp;
						<ASP:LABEL id="LABEL1" runat="server" Height="7px" Width="406px" CssClass="Testo_12_Nero_Bold"
							BackColor="DeepSkyBlue">Filtro sulla Scheda di Tracciabilita'</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<table aria-hidden="true" id="Table1" style="Z-INDEX: 102; LEFT: 128px; WIDTH: 852px; POSITION: absolute; TOP: 64px; HEIGHT: 26px"
				cellSpacing="1" cellPadding="1" width="852" border="0">
				<TR>
					<TD class="Testo_08_Nero" bgColor="#30dfef">
						<P>&nbsp;</P>
					</TD>
				</TR>
			</TABLE>
			<asp:imagebutton id="ImgBtnAnnulla" style="Z-INDEX: 103; LEFT: 928px; POSITION: absolute; TOP: 24px"
				runat="server" ImageUrl="../../AB_Immagini/icone32/Esci.bmp"></asp:imagebutton></form>
	</body>
</HTML>
