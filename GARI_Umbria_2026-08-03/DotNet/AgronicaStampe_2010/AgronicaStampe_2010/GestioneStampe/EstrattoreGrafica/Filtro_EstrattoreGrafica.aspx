<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Filtro_EstrattoreGrafica.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_EstrattoreGrafica" aspcompat="true" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
	<HEAD>
		<title>Filtro_EstrattoreGrafica</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
         <link rel="stylesheet" type="text/css" href="../../App_Styles/Site.css" />
    <link rel="stylesheet" type="text/css" href="../../App_Styles/jquery-ui-1.10.0.custom.min.css" />
    <script type="text/javascript" src="../../App_Scripts/jquery-1.9.0.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript" src="../../App_Scripts/jquery-ui-1.10.0.custom.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript" src="../../App_Scripts/jquery.ui.datepicker-it.js?<% =Application("GiasVersioneCorrente")%>"></script>
	<link href="../../App_Styles/AgronicaStyle.css" type="text/css" rel="stylesheet">
	<script type="text/javascript">
		    $(document).ready(function () {
		        $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
		    });
    </script>
    <style>
        #ui-datepicker-div
        {
            z-index: 10000;
        }
    </style>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table aria-hidden="true" id="Table2" style="Z-INDEX: 104; LEFT: 128px; WIDTH: 780px; POSITION: absolute; TOP: 16px; HEIGHT: 41px"
				cellSpacing="1" cellPadding="1" width="780" border="0">
				<TR>
					<TD style="WIDTH: 35px">
						<ASP:IMAGE id="IMAGE1" runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"></ASP:IMAGE></TD>
					<TD vAlign="middle" align="left" bgColor="#00bfff">&nbsp;&nbsp;
						<ASP:LABEL id="LABEL1" runat="server" Width="406px" Height="7px" BackColor="DeepSkyBlue" CssClass="Testo_12_Nero_Bold"> Filtro Estrazione Dati Grafici</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<ASP:IMAGE id="ImageLogo" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				Width="96px" Height="73px" ImageUrl="../../AB_Immagini/logo/Logo_GiasOnline_Mini.jpg"></ASP:IMAGE>
			<asp:imagebutton id="ImgBtnAnnulla" style="Z-INDEX: 102; LEFT: 928px; POSITION: absolute; TOP: 20px"
				runat="server" ImageUrl="../../AB_Immagini/icone32/Esci.bmp"></asp:imagebutton>
			<table aria-hidden="true" id="Table1" style="Z-INDEX: 100; LEFT: 128px; WIDTH: 852px; POSITION: absolute; TOP: 64px; HEIGHT: 26px"
				cellSpacing="1" cellPadding="1" width="852" border="0">
				<TR>
					<TD class="Testo_08_Nero" bgColor="#30dfef">
						<P>&nbsp;</P>
					</TD>
				</TR>
			</TABLE>
			<ASP:LABEL id="LABEL16" style="Z-INDEX: 110; LEFT: 40px; POSITION: absolute; TOP: 304px" runat="server"
				Width="328px" Height="16px" BackColor="#C0FFC0" CssClass="Testo_08_Nero_Bold" ForeColor="Green">Selezionare i Layers che si desiderano stampare :</ASP:LABEL>
			<ASP:PANEL id="Pannello_Layers" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 111; LEFT: 40px; OVERFLOW: auto; POSITION: absolute; TOP: 320px"
				MS_POSITIONING="GridLayout" runat="server" Width="496px" Height="320px" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<asp:CheckBoxList id="CBL_Layers" style="Z-INDEX: 100; LEFT: 16px; POSITION: absolute; TOP: 48px"
					runat="server" Width="456px" BackColor="#C0FFFF" CssClass="Testo_08_Blue" AutoPostBack="True"></asp:CheckBoxList>
				<asp:imagebutton id="ImgBtnSelezionaTutte" style="Z-INDEX: 103; LEFT: 112px; POSITION: absolute; TOP: 8px"
					runat="server" ImageUrl="../../AB_Immagini/icone32/validazionesi.ico" BackColor="#C0FFFF"></asp:imagebutton>
				<asp:imagebutton id="ImgBtnEliminaSelezione" style="Z-INDEX: 104; LEFT: 272px; POSITION: absolute; TOP: 8px"
					runat="server" ImageUrl="../../AB_Immagini/icone32/validazioneno.ico" BackColor="#C0FFFF"></asp:imagebutton>
				<asp:label id="Label15" style="Z-INDEX: 101; LEFT: 152px; POSITION: absolute; TOP: 8px" runat="server"
					Width="96px" BackColor="#C0FFFF" CssClass="testo_08_blue_bold">Seleziona tutti i Layers</asp:label>
				<asp:label id="Label14" style="Z-INDEX: 105; LEFT: 312px; POSITION: absolute; TOP: 8px" runat="server"
					Width="112px" BackColor="#C0FFFF" CssClass="testo_08_blue_bold">Deseleziona tutti i Layers</asp:label>
			</ASP:PANEL>
			<ASP:PANEL id="Pannello_Data" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 112; LEFT: 600px; OVERFLOW: auto; POSITION: absolute; TOP: 320px"
				MS_POSITIONING="GridLayout" runat="server" Width="305px" Height="186" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<asp:RadioButtonList id="rblStampa" style="Z-INDEX: 105; LEFT: 32px; POSITION: absolute; TOP: 16px" runat="server"
					Width="237px" BackColor="#C0FFFF" CssClass="Testo_08_Nero_Bold" AutoPostBack="True" RepeatDirection="Horizontal">
					<asp:ListItem Value="0">Giorno</asp:ListItem>
					<asp:ListItem Value="1" Selected="True">Intervallo di tempo</asp:ListItem>
				</asp:RadioButtonList>
				<ASP:PANEL id="Pannello_Giorno" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 112; LEFT: 16px; OVERFLOW: auto; POSITION: absolute; TOP: 48px"
					MS_POSITIONING="GridLayout" runat="server" Width="256px" Height="108px" BackColor="#C0FFFF"
					Visible="False">&nbsp;&nbsp; <INPUT class="Testo_08_Nero_Bold" id="BtnDataStampa" style="Z-INDEX: 109; LEFT: 200px; POSITION: absolute; TOP: 32px; HEIGHT: 17px"
						type="button" value="..." name="BtnDataStampa" Width="25px" Height="17px"> 
<ASP:TEXTBOX id="TxtStampa" style="Z-INDEX: 108; LEFT: 40px; POSITION: absolute; TOP: 24px" runat="server"
						Width="120px" Height="24px"  CssClass="txtui datepicker" 
						MaxLength="10" ></ASP:TEXTBOX></ASP:PANEL>
				<ASP:PANEL id="Pannello_Intervallo" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 111; LEFT: 16px; OVERFLOW: auto; POSITION: absolute; TOP: 48px"
					MS_POSITIONING="GridLayout" runat="server" Width="256px" Height="102px" BackColor="#C0FFFF"
					Visible="False">&nbsp;&nbsp; 
<asp:Label id="LblDA" style="Z-INDEX: 108; LEFT: 24px; POSITION: absolute; TOP: 32px" runat="server"
						Width="32px" Height="16px" BackColor="#C0FFFF" CssClass="Testo_08_Nero_Bold"
						ToolTip="Data iniziale dell'intervallo in cui verranno stampati solo gli impianti attivi in quell'intervallo">DA :</asp:Label>
<ASP:TEXTBOX id="TxtValiditaInizio" style="Z-INDEX: 108; LEFT: 64px; POSITION: absolute; TOP: 24px"
						runat="server" Width="120px" Height="24px" 
						CssClass="txtui datepicker"
						MaxLength="10" ></ASP:TEXTBOX><INPUT class="Testo_08_Nero_Bold" id="BtnDataInizio" style="Z-INDEX: 108; LEFT: 216px; POSITION: absolute; TOP: 32px; HEIGHT: 17px"
						type="button" value="..." name="BtnDataInizio" Width="25px" Height="17px"> 
<asp:Label id="LblA" style="Z-INDEX: 108; LEFT: 24px; POSITION: absolute; TOP: 72px" runat="server"
						Width="32px" Height="16px" BackColor="#C0FFFF" CssClass="Testo_08_Nero_Bold"
						ToolTip="Data finale dell'intervallo in cui verranno stampati solo gli impianti attivi in quell'intervallo">A :</asp:Label>
<ASP:TEXTBOX id="TxtValiditaFine" style="Z-INDEX: 107; LEFT: 64px; POSITION: absolute; TOP: 64px"
						runat="server" Width="120px" Height="24px" 
						CssClass="txtui datepicker"
						MaxLength="10" ></ASP:TEXTBOX><INPUT class="Testo_08_Nero_Bold" id="BtnDataFine" style="Z-INDEX: 106; LEFT: 216px; POSITION: absolute; TOP: 72px; HEIGHT: 17px"
						type="button" value="..." name="BtnDataFine" Width="25px" Height="17px"></ASP:PANEL>
			</ASP:PANEL>
			<ASP:LABEL id="LABEL2" style="Z-INDEX: 113; LEFT: 600px; POSITION: absolute; TOP: 304px" runat="server"
				Width="232px" Height="15px" BackColor="#C0FFC0" CssClass="Testo_08_Rosso_Bold" ForeColor="Green">&nbsp;Date di riferimento per la stampa :</ASP:LABEL>
			<ASP:PANEL id="Pannello_Stampa" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 114; LEFT: 600px; OVERFLOW: auto; POSITION: absolute; TOP: 536px"
				MS_POSITIONING="GridLayout" runat="server" Width="302px" Height="102px" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<ASP:IMAGEBUTTON id="ImgBtn_Stampa" style="Z-INDEX: 119; LEFT: 40px; POSITION: absolute; TOP: 32px"
					runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico" BackColor="#C0FFFF"></ASP:IMAGEBUTTON>
				<ASP:LABEL id="LABEL12" style="Z-INDEX: 119; LEFT: 96px; POSITION: absolute; TOP: 40px" runat="server"
					Width="166px" Height="15px" BackColor="#C0FFFF" CssClass="Testo_08_Rosso_Bold">Stampa i dati selezionati</ASP:LABEL>
			</ASP:PANEL>
			<ASP:LABEL id="LABEL3" style="Z-INDEX: 115; LEFT: 40px; POSITION: absolute; TOP: 104px" runat="server"
				Width="80px" Height="15px" BackColor="#C0FFC0" CssClass="Testo_08_Rosso_Bold" ForeColor="Green">&nbsp;Riepilogo :</ASP:LABEL>
			<ASP:PANEL id="Note" style="SCROLLBAR-FACE-COLOR: #afeeee; Z-INDEX: 116; LEFT: 40px; OVERFLOW: auto; POSITION: absolute; TOP: 120px"
				MS_POSITIONING="GridLayout" runat="server" Width="889" Height="160px" BackColor="#C0FFFF"
				BORDERSTYLE="Solid" BORDERCOLOR="#0000C0" BORDERWIDTH="2px">
				<ASP:LABEL id="LABEL13" style="Z-INDEX: 100; LEFT: 18px; POSITION: absolute; TOP: 16px" runat="server"
					Width="404px" Height="16px" BackColor="#C0FFFF" CssClass="Testo_08_Nero">E&#39; stata richiesta la stampa per l&#39;impresa :</ASP:LABEL>
				<asp:Label id="Lbl_Impresa" style="Z-INDEX: 101; LEFT: 368px; POSITION: absolute; TOP: 40px"
					runat="server" Width="470px" Height="20px" BackColor="#C0FFFF" CssClass="Testo_08_Rosso_Bold"></asp:Label>
				<asp:Label id="Label4" style="Z-INDEX: 102; LEFT: 15px; POSITION: absolute; TOP: 96px" runat="server"
					Width="352px" Height="17px" BackColor="#C0FFFF" CssClass="Testo_08_Nero">Si tenga presente che il periodo di attivita' di tale impresa va </asp:Label>
				<asp:Label id="Lbl_ValiditaInizio" style="Z-INDEX: 103; LEFT: 448px; POSITION: absolute; TOP: 96px"
					runat="server" Width="86px" Height="20px" BackColor="#C0FFFF" CssClass="Testo_08_Blue_Bold"></asp:Label>
				<asp:Label id="Lbl_ValiditaFine" style="Z-INDEX: 104; LEFT: 608px; POSITION: absolute; TOP: 96px"
					runat="server" Width="102px" Height="20px" BackColor="#C0FFFF" CssClass="Testo_08_Blue_Bold"></asp:Label>
				<asp:Label id="Label7" style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 128px" runat="server"
					Width="506px" Height="16px" BackColor="#C0FFFF" CssClass="Testo_08_Nero">Impostando una data esterna all'intervallo, si potrebbe ottenere una stampa nulla ...</asp:Label>
				<asp:Label id="Label8" style="Z-INDEX: 106; LEFT: 408px; POSITION: absolute; TOP: 96px" runat="server"
					Width="32px" Height="20px" BackColor="#C0FFFF" CssClass="Testo_08_Nero_Bold">Dal :</asp:Label>
				<asp:Label id="Label9" style="Z-INDEX: 107; LEFT: 584px; POSITION: absolute; TOP: 96px" runat="server"
					Width="24px" Height="20px" BackColor="#C0FFFF" CssClass="Testo_08_Nero_Bold">Al :</asp:Label>
				<asp:Label id="Lbl_Piva" style="Z-INDEX: 108; LEFT: 120px; POSITION: absolute; TOP: 40px" runat="server"
					Width="110px" Height="20px" BackColor="#C0FFFF" CssClass="Testo_08_Blue_Bold"></asp:Label>
				<asp:Label id="Label5" style="Z-INDEX: 109; LEFT: 264px; POSITION: absolute; TOP: 40px" runat="server"
					Width="88px" Height="16px" BackColor="#C0FFFF" CssClass="Testo_08_Nero_Bold">Rag.Sociale :</asp:Label>
				<asp:Label id="Label6" style="Z-INDEX: 110; LEFT: 24px; POSITION: absolute; TOP: 40px" runat="server"
					Width="80px" Height="16px" BackColor="#C0FFFF" CssClass="Testo_08_Nero_Bold">Partita IVA :</asp:Label>
				<asp:Label id="Label10" style="Z-INDEX: 113; LEFT: 24px; POSITION: absolute; TOP: 64px" runat="server"
					Width="128px" Height="16px" BackColor="#C0FFFF" CssClass="Testo_08_Nero_Bold">Centro Aziendale :</asp:Label>
				<asp:Label id="Lbl_Centro" style="Z-INDEX: 114; LEFT: 152px; POSITION: absolute; TOP: 64px"
					runat="server" Width="682px" Height="20px" BackColor="#C0FFFF" CssClass="Testo_08_Rosso_Bold"></asp:Label>
			</ASP:PANEL>
		</form>
	</body>
</HTML>
