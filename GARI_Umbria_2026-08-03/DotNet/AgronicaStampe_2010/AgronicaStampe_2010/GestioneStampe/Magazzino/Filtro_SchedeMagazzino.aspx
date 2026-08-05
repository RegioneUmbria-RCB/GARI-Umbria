<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Filtro_SchedeMagazzino.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_SchedeMagazzino" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
	<HEAD>
		<title>Filtro_SchedeMagazzino</title>
		<meta name="vs_showGrid" content="True">
		<meta name="vs_snapToGrid" content="True">
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK rel="stylesheet" type="text/css" href="../../App_Styles/AgronicaStyle.css">
		<LINK rel="stylesheet" type="text/css" href="../../App_Styles/jquery-ui-1.10.0.custom.min.css">
		<script type="text/javascript" src="../../App_Scripts/jquery-1.9.0.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<script type="text/javascript" src="../../App_Scripts/jquery-ui-1.10.0.custom.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<script type="text/javascript" src="../../App_Scripts/jquery.ui.datepicker-it.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<style>#ui-datepicker-div { Z-INDEX: 10000 }
		</style>
		<script type="text/javascript"> 				
 				$(document).ready(function(){				
					$(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
				});
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table aria-hidden="true" style="HEIGHT: 41px; WIDTH: 771px; POSITION: absolute; LEFT: 128px; Z-INDEX: 100; TOP: 16px"
				id="Table3" border="0" cellSpacing="1" cellPadding="1" width="771">
				<TR>
					<TD style="WIDTH: 41px"><ASP:IMAGE id="IMAGE2" runat="server" Height="32px" Width="32px" ImageUrl="../../AB_Immagini/icone32/stampa.ico"></ASP:IMAGE></TD>
					<TD bgColor="#5c9ccc" vAlign="middle" align="left">&nbsp;
						<ASP:LABEL id="LABEL3" runat="server" Height="7px" Width="465px" CssClass="Testo_12_Nero_Bold"
							BackColor="#5C9CCC">Filtro Schede di Magazzino</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<ASP:IMAGEBUTTON id="ImgBtn_StampaExcel" style="POSITION: absolute; LEFT: 872px; Z-INDEX: 109; TOP: 168px"
				runat="server" ImageUrl="../../AB_Immagini/icone32/XLS_01.ico" Width="32px" Height="32px"></ASP:IMAGEBUTTON>

                				<asp:checkbox id="CB_BloccaOperazioni" style="POSITION: absolute; TOP: 220px; left:855; width: 119px;"
					runat="server"  CssClass="Testo_07_Blue" Text="Blocca Operazioni" Visible="false"></asp:checkbox>

			<asp:label style="POSITION: absolute; LEFT: 920px; Z-INDEX: 106; TOP: 152px" id="Label48" runat="server"
				Width="56px" CssClass="testo_08_blue_bold">Stampa</asp:label><ASP:IMAGEBUTTON style="POSITION: absolute; LEFT: 872px; Z-INDEX: 105; TOP: 128px" id="ImgBtn_Stampa"
				runat="server" Height="32px" Width="32px" ImageUrl="../../AB_Immagini/icone32/stampa.ico"></ASP:IMAGEBUTTON><ASP:PANEL style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 16px; Z-INDEX: 107; TOP: 104px"
				id="Pannello_Filtro" MS_POSITIONING="GridLayout" runat="server" Height="540px" Width="832px" BackColor="#E6F4FF" BORDERCOLOR="#0000C0" BORDERWIDTH="1px" BorderStyle="Solid">
				<ASP:LABEL id="LABEL9" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 101; TOP: 280px" runat="server"
					Width="72px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Impresa :</ASP:LABEL>
				<ASP:DROPDOWNLIST id="Cmb_Impresa" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 108; TOP: 280px"
					runat="server" Width="496px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
				<ASP:DROPDOWNLIST id="Cmb_CentroAziendale" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 103; TOP: 312px"
					runat="server" Width="496px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
				<ASP:DROPDOWNLIST id="Cmb_Magazzino" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 104; TOP: 344px"
					runat="server" Width="496px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
				<ASP:LABEL id="LABEL1" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 100; TOP: 312px" runat="server"
					Width="128px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Centro Aziendale :</ASP:LABEL>
				<ASP:LABEL id="LABEL2" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 102; TOP: 344px" runat="server"
					Width="72px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Magazzino :</ASP:LABEL>
				<ASP:PANEL id="Pannello_Giorno" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 106; TOP: 152px"
					MS_POSITIONING="GridLayout" runat="server" Width="464px" Height="78px" BackColor="#E6F4FF"
					Visible="False">&nbsp;&nbsp; 
<ASP:TEXTBOX id="TxtStampa" style="POSITION: absolute; LEFT: 56px; Z-INDEX: 108; TOP: 32px" runat="server"
						Width="120px" Height="20px" BackColor="#FFFFFF" CssClass="Testo_10_Blue_Bold datepicker"
						BorderStyle="None" MaxLength="10" ToolTip="Data in cui verranno stampate le giacenze"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL7" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 111; TOP: 8px" runat="server"
						Width="154px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Data Stampa :</ASP:LABEL></ASP:PANEL>
				<ASP:RADIOBUTTONLIST id="Rbl_SchedaMagazzino" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 109; TOP: 24px"
					runat="server" Width="260px" Height="48px" CssClass="Testo_08_Blue" AutoPostBack="True" RepeatLayout="Flow"
					BACKCOLOR="#E6F4FF">
					<asp:ListItem Value="9" Selected="True">Scheda Movimenti di Magazzino</asp:ListItem>
					<asp:ListItem Value="10">Scheda Giacenze di Magazzino</asp:ListItem>
					<asp:ListItem Value="11">Scheda Fertilizzanti in Magazzino</asp:ListItem>
					<asp:ListItem Value="12">Scheda Prodotti Fitosanitari in Magazzino</asp:ListItem>
				</ASP:RADIOBUTTONLIST>
				<ASP:LABEL id="LABEL8" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 107; TOP: 8px" runat="server"
					Width="144px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Stampa selezionata :</ASP:LABEL>
				<ASP:PANEL id="Pannello_IntervalloMovimento" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 105; TOP: 152px"
					MS_POSITIONING="GridLayout" runat="server" Width="464px" Height="81" BackColor="#E6F4FF"
					Visible="False">&nbsp;&nbsp;&nbsp; 
<ASP:LABEL id="LABEL12" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 101; TOP: 8px" runat="server"
						Width="192px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Intervallo temporale:</ASP:LABEL>
<ASP:LABEL id="LABEL11" style="POSITION: absolute; LEFT: 24px; Z-INDEX: 102; TOP: 40px" runat="server"
						Width="24px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero">Da :</ASP:LABEL>
<ASP:LABEL id="LABEL10" style="POSITION: absolute; LEFT: 224px; Z-INDEX: 103; TOP: 40px" runat="server"
						Width="24px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero">A :</ASP:LABEL>
<ASP:TEXTBOX id="TxtDataDa" style="POSITION: absolute; LEFT: 56px; Z-INDEX: 2000; TOP: 35px"
						runat="server" Width="120px" Height="20px" BackColor="#FFFFFF" CssClass="Testo_10_Blue_Bold datepicker"
						BorderStyle="None" MaxLength="10" ToolTip="Data inizio"></ASP:TEXTBOX>
<ASP:TEXTBOX id="TxtDataA" style="POSITION: absolute; LEFT: 248px; Z-INDEX: 2001; TOP: 35px"
						runat="server" Width="120px" Height="20px" BackColor="#FFFFFF" CssClass="Testo_10_Blue_Bold datepicker"
						BorderStyle="None" MaxLength="10" ToolTip="Data fine"></ASP:TEXTBOX></ASP:PANEL>
				<ASP:LABEL id="LABEL13" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 110; TOP: 248px" runat="server"
					Width="120px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Ricerca Impresa :</ASP:LABEL>
				<ASP:TEXTBOX id="Txt_Impresa" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 111; TOP: 248px"
					runat="server" Width="256px" Height="16px" BackColor="#FFFFFF" CssClass="Testo_08_Blue" BorderStyle="None"
					ToolTip=""></ASP:TEXTBOX>
				<ASP:IMAGEBUTTON id="ImgBtn_CercaImpresa" style="POSITION: absolute; LEFT: 424px; Z-INDEX: 112; TOP: 240px"
					runat="server" ImageUrl="../../AB_Immagini/icone32/lente.ico" Width="32px" Height="32px" BackColor="#E6F4FF"></ASP:IMAGEBUTTON>
				<ASP:LABEL id="lbl_categorie" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 113; TOP: 376px"
					runat="server" Width="136px" Height="17px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Categoria Prodotto :</ASP:LABEL>
				<ASP:DROPDOWNLIST id="cmb_CatProdotto" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 114; TOP: 376px"
					runat="server" Width="496px" Height="23px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
				<ASP:LABEL id="LABEL16" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 115; TOP: 8px" runat="server"
					Width="128px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Seleziona il tipo di arrotondamento:</ASP:LABEL>
				<ASP:RADIOBUTTONLIST id="Rbl_Arrotondamento" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 116; TOP: 40px"
					runat="server" Width="176px" Height="48px" CssClass="Testo_08_Blue" BACKCOLOR="#E6F4FF" RepeatColumns="2">
					<asp:ListItem Value="0">Nessuno</asp:ListItem>
					<asp:ListItem Value="1">Unit&#224;</asp:ListItem>
					<asp:ListItem Value="2">1 Decimale</asp:ListItem>
					<asp:ListItem Value="3">2 Decimali</asp:ListItem>
					<asp:ListItem Value="4" Selected="True">3 Decimali</asp:ListItem>
					<asp:ListItem Value="5">4 Decimali</asp:ListItem>
				</ASP:RADIOBUTTONLIST>
				<ASP:LABEL id="Lbl_NumImprese" style="POSITION: absolute; LEFT: 736px; Z-INDEX: 117; TOP: 280px"
					runat="server" Width="72px" Height="8px" BackColor="#E6F4FF" CssClass="Testo_08_rosso_Bold">0</ASP:LABEL>
				<ASP:LABEL id="LABEL17" style="POSITION: absolute; LEFT: 664px; Z-INDEX: 118; TOP: 280px" runat="server"
					Width="64px" Height="8px" BackColor="#E6F4FF" CssClass="Testo_08_nero_bold">Trovate:</ASP:LABEL>
				<ASP:LABEL id="Lbl_Ordinamento" style="POSITION: absolute; LEFT: 899px; Z-INDEX: 119; TOP: 120px"
					runat="server" Width="112px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold"
					Visible="False">Seleziona il tipo di ordinamento:</ASP:LABEL>
				<ASP:RADIOBUTTONLIST id="Rbl_Ordinamento" style="POSITION: absolute; LEFT: 496px; Z-INDEX: 121; TOP: 176px"
					runat="server" Width="112px" Height="48px" CssClass="Testo_08_Blue" Visible="False" RepeatLayout="Flow"
					BACKCOLOR="#E6F4FF">
					<asp:ListItem Value="0" Selected="True">Data</asp:ListItem>
					<asp:ListItem Value="1">Prodotto</asp:ListItem>
				</ASP:RADIOBUTTONLIST>
				<ASP:PANEL id="Pannello_Intervallo" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 122; TOP: 152px"
					MS_POSITIONING="GridLayout" runat="server" Width="464px" Height="81" BackColor="#E6F4FF"
					Visible="False">&nbsp;&nbsp;&nbsp; 
<ASP:LABEL id="LABEL6" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 101; TOP: 8px" runat="server"
						Width="154px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Annata Agraria :</ASP:LABEL>
<ASP:LABEL id="LABEL4" style="POSITION: absolute; LEFT: 24px; Z-INDEX: 102; TOP: 40px" runat="server"
						Width="24px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero">Da :</ASP:LABEL>
<ASP:LABEL id="LABEL5" style="POSITION: absolute; LEFT: 200px; Z-INDEX: 103; TOP: 40px" runat="server"
						Width="24px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero">A :</ASP:LABEL>
<ASP:TEXTBOX id="TxtDataInizio" style="POSITION: absolute; LEFT: 56px; Z-INDEX: 104; TOP: 35px"
						runat="server" Width="120px" Height="20px" BackColor="#FFFFFF" CssClass="Testo_10_Blue_Bold datepicker"
						BorderStyle="None" MaxLength="10" ToolTip="Data inizio"></ASP:TEXTBOX>
<ASP:TEXTBOX id="TxtDataFine" style="POSITION: absolute; LEFT: 224px; Z-INDEX: 105; TOP: 35px"
						runat="server" Width="120px" Height="20px" BackColor="#FFFFFF" CssClass="Testo_10_Blue_Bold datepicker"
						BorderStyle="None" MaxLength="10" ToolTip="Data fine"></ASP:TEXTBOX>
<ASP:IMAGEBUTTON id="ImgBtn_AnnataPrecedente" style="POSITION: absolute; LEFT: 368px; Z-INDEX: 100; TOP: 24px"
						runat="server" ImageUrl="../../AB_Immagini/icone32/frecciasx.ico" Width="32px"
						Height="32px" BackColor="#E6F4FF" ToolTip="Annata Precedente"></ASP:IMAGEBUTTON>
<ASP:IMAGEBUTTON id="ImgBtn_AnnataSuccessiva" style="POSITION: absolute; LEFT: 416px; Z-INDEX: 106; TOP: 24px"
						runat="server" ImageUrl="../../AB_Immagini/icone32/frecciadx.ico" Width="32px"
						Height="32px" BackColor="#E6F4FF" ToolTip="Annata Successiva"></ASP:IMAGEBUTTON></ASP:PANEL>
				<ASP:LABEL id="LABEL19" style="POSITION: absolute; LEFT: 664px; Z-INDEX: 123; TOP: 344px" runat="server"
					Width="64px" Height="8px" BackColor="#E6F4FF" CssClass="Testo_08_nero_bold">Trovati:</ASP:LABEL>
				<ASP:LABEL id="LABEL20" style="POSITION: absolute; LEFT: 664px; Z-INDEX: 124; TOP: 312px" runat="server"
					Width="64px" Height="8px" BackColor="#E6F4FF" CssClass="Testo_08_nero_bold">Trovati:</ASP:LABEL>
				<ASP:LABEL id="Lbl_NumCentri" style="POSITION: absolute; LEFT: 736px; Z-INDEX: 125; TOP: 312px"
					runat="server" Width="72px" Height="8px" BackColor="#E6F4FF" CssClass="Testo_08_rosso_Bold">0</ASP:LABEL>
				<ASP:LABEL id="Lbl_NumMagazzini" style="POSITION: absolute; LEFT: 736px; Z-INDEX: 126; TOP: 344px"
					runat="server" Width="72px" Height="8px" BackColor="#E6F4FF" CssClass="Testo_08_rosso_Bold">0</ASP:LABEL>
				<ASP:PANEL id="Pannello_Prodotti" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 16px; Z-INDEX: 127; TOP: 400px"
					MS_POSITIONING="GridLayout" runat="server" Width="800px" Height="108px" BackColor="#E6F4FF"
					Visible="False">&nbsp;&nbsp;&nbsp; 
<ASP:LABEL id="LABEL14" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 100; TOP: 16px" runat="server"
						Width="120px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Ricerca Prodotto :</ASP:LABEL>
<ASP:TEXTBOX id="Txt_ProdottoCerca" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 101; TOP: 16px"
						runat="server" Width="128px" Height="16px" BackColor="#FFFFFF" CssClass="Testo_08_Blue"
						BorderStyle="None" ToolTip=""></ASP:TEXTBOX>
<ASP:IMAGEBUTTON id="ImgBtn_ProdottiCerca" style="POSITION: absolute; LEFT: 272px; Z-INDEX: 102; TOP: 8px"
						runat="server" ImageUrl="../../AB_Immagini/icone32/lente.ico" Width="32px"
						Height="32px" BackColor="#E6F4FF"></ASP:IMAGEBUTTON>
<ASP:LABEL id="lbl_prodotti" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 103; TOP: 48px"
						runat="server" Width="104px" Height="17px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Prodotti :</ASP:LABEL>
<ASP:DROPDOWNLIST id="cmb_Prodotti" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 105; TOP: 48px"
						runat="server" Width="496px" Height="27px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
<ASP:LABEL id="lbl_trovati" style="POSITION: absolute; LEFT: 648px; Z-INDEX: 106; TOP: 48px"
						runat="server" Width="64px" Height="8px" BackColor="#E6F4FF" CssClass="Testo_08_nero_bold">Trovati:</ASP:LABEL>
<ASP:LABEL id="Lbl_NumProdotti" style="POSITION: absolute; LEFT: 720px; Z-INDEX: 107; TOP: 48px"
						runat="server" Width="72px" Height="8px" BackColor="#E6F4FF" CssClass="Testo_08_rosso_Bold">0</ASP:LABEL>
<ASP:LABEL id="lbl_lotto" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 108; TOP: 90px" runat="server"
						Width="56px" Height="17px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Lotto:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_Lotto" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 132; TOP: 90px"
						runat="server" Width="96px" Height="16px" BackColor="#FFFFFF" CssClass="Testo_08_Blue"
						BorderStyle="None" ToolTip=""></ASP:TEXTBOX></ASP:PANEL>
				<ASP:PANEL id="Pannello_CodiciArticolo" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 488px; Z-INDEX: 128; TOP: 8px"
					MS_POSITIONING="GridLayout" runat="server" Width="336px" Height="128px" BackColor="#E6F4FF"
					Visible="False">&nbsp;&nbsp;&nbsp; 
<ASP:LABEL id="LABEL15" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 101; TOP: 0px" runat="server"
						Width="280px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold">Seleziona le categoria di cui si vuole stampare il codice articolo:</ASP:LABEL>
<asp:CheckBoxList id="ChkList_Categorie" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 102; TOP: 32px"
						runat="server" Width="320px" Height="6px" BackColor="#E6F4FF" CssClass="Testo_08_Blue"></asp:CheckBoxList></ASP:PANEL>
				<ASP:DROPDOWNLIST tabIndex="7" id="Cmb_Regioni" style="POSITION: absolute; LEFT: 664px; Z-INDEX: 129; TOP: 240px"
					runat="server" Width="152px" Height="18px" CssClass="Testo_08_Nero" AutoPostBack="True"></ASP:DROPDOWNLIST>
				<asp:checkbox id="Chk_LogoRegione" style="POSITION: absolute; LEFT: 488px; Z-INDEX: 130; TOP: 239px"
					runat="server" Width="173px" BackColor="#E6F4FF" CssClass="Testo_07_Blue" Text="Stampa il logo della regione"></asp:checkbox>
				<asp:checkbox id="Chk_Composizione" style="POSITION: absolute; LEFT: 352px; Z-INDEX: 131; TOP: 408px"
					runat="server" Width="296px" Height="32px" BackColor="#E6F4FF" CssClass="Testo_08_Blue" Visible="False"
					Text="Stampa la composizione dei prodotti fitosanitari"></asp:checkbox>
				<ASP:LABEL id="Lbl_StampaLotto" style="POSITION: absolute; LEFT: 624px; Z-INDEX: 132; TOP: 144px"
					runat="server" Width="192px" Height="15px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold"
					Visible="False">Seleziona la modalità di stampa del lotto:</ASP:LABEL>
				<ASP:RADIOBUTTONLIST id="Rbl_StampaLotto" style="POSITION: absolute; LEFT: 624px; Z-INDEX: 133; TOP: 176px"
					runat="server" Width="184px" Height="48px" CssClass="Testo_08_Blue" Visible="False" RepeatLayout="Flow"
					BACKCOLOR="#E6F4FF">
					<asp:ListItem Value="0">Stampa sempre il lotto</asp:ListItem>
					<asp:ListItem Value="1" Selected="True">Stampa in base alla configurazione del prodotto</asp:ListItem>
				</ASP:RADIOBUTTONLIST>
			</ASP:PANEL><ASP:IMAGE style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 8px" id="ImageLogo" runat="server"
				Height="73px" Width="96px" ImageUrl="../../AB_Immagini/logo/Logo_GiasOnline_Mini.jpg"></ASP:IMAGE>
			<table aria-hidden="true" style="HEIGHT: 26px; POSITION: absolute; LEFT: 128px; Z-INDEX: 103; TOP: 64px" id="Table4"
				border="0" cellSpacing="1" cellPadding="1" width="849">
				<TR>
					<TD bgColor="#5c9ccc"></TD>
				</TR>
			</TABLE>
			<ASP:IMAGEBUTTON style="POSITION: absolute; LEFT: 920px; Z-INDEX: 102; TOP: 24px" id="ImgBtnEsci"
				runat="server" Height="32px" Width="32px" ImageUrl="../../AB_Immagini/icone32/esci.bmp"></ASP:IMAGEBUTTON><asp:panel style="POSITION: absolute; LEFT: 1008px; Z-INDEX: 108; TOP: 808px" id="Panel1" runat="server"
				Height="72px" Width="88px" Visible="False">Panel</asp:panel></form>
	</body>
</HTML>
