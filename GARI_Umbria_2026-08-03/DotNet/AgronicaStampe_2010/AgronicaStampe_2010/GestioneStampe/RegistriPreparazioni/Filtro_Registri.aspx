<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Filtro_Registri.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_Registri"  %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
	<HEAD>
		<title>Filtro_Registri</title>
		<meta name="vs_snapToGrid" content="True">
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK rel="stylesheet" type="text/css" href="../../App_Scripts/CSS/AgronicaStyle.css">
		<LINK rel="stylesheet" type="text/css" href="../../App_Styles/jquery-ui-1.10.0.custom.min.css" />
		<script type="text/javascript" src="../../App_Scripts/jquery-1.9.0.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<script type="text/javascript" src="../../App_Scripts/jquery-ui-1.10.0.custom.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<script type="text/javascript" src="../../App_Scripts/jquery.ui.datepicker-it.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<script type="text/javascript"> 				
 				$(document).ready(function(){				
					$(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
				});
		</script>
		<style>#ui-datepicker-div { Z-INDEX: 10000 }
		</style>
		<style>#Rbl_Report TBODY TR TD { WIDTH: 50% }
		</style>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server" style="POSITION: absolute">
			<ASP:PANEL style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 107; TOP: 216px"
				id="Pannello_RegCantina" MS_POSITIONING="GridLayout" runat="server" BORDERSTYLE="None"
				BORDERWIDTH="2px" Height="742px" Width="1074" BackColor="White">
				<ASP:PANEL id="Pannello_ConsEnologiche" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 0px; Z-INDEX: 100; TOP: 344px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="960px" Height="104px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BORDERCOLOR="#0000C0">
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_CentroAziendale" style="POSITION: absolute; LEFT: 88px; Z-INDEX: 100; TOP: 8px"
						runat="server" Width="344px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
					<ASP:LABEL id="LABEL7" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 8px" runat="server"
						BackColor="#E6F4FF" Width="72px" Height="15px" CssClass="Testo_08_Nero_Bold">Centro Az.:</ASP:LABEL>
					<ASP:IMAGEBUTTON id="ImgBtn_StampaConsistenze" style="POSITION: absolute; LEFT: 816px; Z-INDEX: 102; TOP: 64px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"
						ToolTip="Stampa le Consistenze Enologiche"></ASP:IMAGEBUTTON>
					<asp:Label id="Label8" style="POSITION: absolute; LEFT: 856px; Z-INDEX: 103; TOP: 56px" runat="server"
						BackColor="#E6F4FF" Width="80px" Height="18px" CssClass="Testo_08_Rosso_Bold">Stampa le Consistenze Enologiche</asp:Label>
					<ASP:LABEL id="LABEL1" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 104; TOP: 40px" runat="server"
						BackColor="#E6F4FF" Width="64px" Height="15px" CssClass="Testo_08_Nero_Bold">Piano:</ASP:LABEL>
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Piano" style="POSITION: absolute; LEFT: 64px; Z-INDEX: 105; TOP: 40px"
						runat="server" Width="120px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
					<ASP:LABEL id="LABEL10" style="POSITION: absolute; LEFT: 576px; Z-INDEX: 106; TOP: 73px" runat="server"
						BackColor="#E6F4FF" Width="96px" Height="15px" CssClass="Testo_08_Nero_Bold">Ordinamento:</ASP:LABEL>
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Ordinamento" style="POSITION: absolute; LEFT: 672px; Z-INDEX: 107; TOP: 71px"
						runat="server" Width="128px" Height="18px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
					<ASP:TEXTBOX id="Txt_DataConsistenze" style="POSITION: absolute; LEFT: 872px; Z-INDEX: 108; TOP: 32px"
						runat="server" BackColor="#FFFFFF" Width="80px" Height="16px" CssClass="Testo_08_Blue datepicker"
						ToolTip="" MaxLength="10" BorderStyle="None"></ASP:TEXTBOX>
					<ASP:LABEL id="LABEL35" style="POSITION: absolute; LEFT: 808px; Z-INDEX: 109; TOP: 24px" runat="server"
						BackColor="#E6F4FF" Width="56px" Height="15px" CssClass="Testo_08_Nero_Bold">Data di stampa:</ASP:LABEL>
					<asp:CheckBox id="Chk_ConsEnoZero" style="POSITION: absolute; LEFT: 616px; Z-INDEX: 111; TOP: 0px"
						runat="server" BackColor="#E6F4FF" Width="168px" CssClass="Testo_08_Nero" Text="Stampa anche le  vasche con consistenze =0"></asp:CheckBox>
					<ASP:LABEL id="LABEL36" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 112; TOP: 72px" runat="server"
						BackColor="#E6F4FF" Width="112px" Height="15px" CssClass="Testo_08_Nero_Bold">Vasca:</ASP:LABEL>
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Vasca" style="POSITION: absolute; LEFT: 64px; Z-INDEX: 113; TOP: 72px"
						runat="server" Width="120px" Height="18px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
					<ASP:LABEL id="LABEL45" style="POSITION: absolute; LEFT: 192px; Z-INDEX: 114; TOP: 40px" runat="server"
						BackColor="#E6F4FF" Width="112px" Height="15px" CssClass="Testo_08_Nero_Bold">Linea produttiva:</ASP:LABEL>
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_LineeConsistenze" style="POSITION: absolute; LEFT: 320px; Z-INDEX: 115; TOP: 40px"
						runat="server" Width="475px" Height="18px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
					<asp:CheckBox id="Chk_VascheNoMov" style="POSITION: absolute; LEFT: 448px; Z-INDEX: 116; TOP: 0px"
						runat="server" BackColor="#E6F4FF" Width="168px" CssClass="Testo_08_Nero" Text="Stampa anche le  vasche non movimentate"></asp:CheckBox>
					<ASP:LABEL id="LABEL53" style="POSITION: absolute; LEFT: 192px; Z-INDEX: 117; TOP: 64px" runat="server"
						BackColor="#E6F4FF" Width="96px" Height="28px" CssClass="Testo_08_Nero_Bold">Tipologia semilavorati:</ASP:LABEL>
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Categoria" style="POSITION: absolute; LEFT: 496px; Z-INDEX: 118; TOP: 72px"
						runat="server" Width="70px" Height="18px" CssClass="Testo_08_Blue">
						<asp:ListItem Value="0">No filtro</asp:ListItem>
						<asp:ListItem Value="19">Docg</asp:ListItem>
						<asp:ListItem Value="20">Dop</asp:ListItem>
						<asp:ListItem Value="21">Igp</asp:ListItem>
						<asp:ListItem Value="22">Tavola</asp:ListItem>
						<asp:ListItem Value="163">Varietale</asp:ListItem>
					</ASP:DROPDOWNLIST>
					<ASP:LABEL id="LABEL54" style="POSITION: absolute; LEFT: 416px; Z-INDEX: 119; TOP: 72px" runat="server"
						BackColor="#E6F4FF" Width="80px" Height="20px" CssClass="Testo_08_Nero_Bold">Categoria:</ASP:LABEL>
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Semilavorati" style="POSITION: absolute; LEFT: 288px; Z-INDEX: 120; TOP: 72px"
						runat="server" Width="118px" Height="18px" CssClass="Testo_08_Blue">
						<asp:ListItem Value="0">No filtro</asp:ListItem>
						<asp:ListItem Value="55">Vino</asp:ListItem>
						<asp:ListItem Value="54">Vino Atto a Divenire</asp:ListItem>
						<asp:ListItem Value="53">VNAF</asp:ListItem>
						<asp:ListItem Value="52">MostoPF</asp:ListItem>
						<asp:ListItem Value="51">Mosto</asp:ListItem>
						<asp:ListItem Value="433">Vino Arricchito</asp:ListItem>
						<asp:ListItem Value="434">VNAF Arricchito</asp:ListItem>
						<asp:ListItem Value="231">Vino in Frizzantatura</asp:ListItem>
						<asp:ListItem Value="247">Vino in Spumantizzazione</asp:ListItem>
						<asp:ListItem Value="319">Vino Atto in Frizzantatura </asp:ListItem>
						<asp:ListItem Value="248">Vino Atto in Spumantizzazione </asp:ListItem>
					</ASP:DROPDOWNLIST>
					<asp:CheckBox id="Chk_StampaRiepilogo" style="POSITION: absolute; LEFT: 808px; Z-INDEX: 121; TOP: 0px"
						runat="server" BackColor="#E6F4FF" Width="136px" CssClass="Testo_08_Nero" Text="Stampa il riepilogo"></asp:CheckBox>
				</ASP:PANEL>
				<ASP:LABEL id="LABEL24" style="POSITION: absolute; LEFT: 584px; Z-INDEX: 101; TOP: 456px" runat="server"
					BackColor="#C0FFC0" Width="40px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green">&nbsp;DAA:</ASP:LABEL>
				<ASP:PANEL id="Pannello_DAA" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 584px; Z-INDEX: 102; TOP: 472px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="376px" Height="166px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BORDERCOLOR="#0000C0">
					<ASP:LABEL id="LABEL20" style="POSITION: absolute; LEFT: 7px; Z-INDEX: 100; TOP: 7px" runat="server"
						BackColor="#E6F4FF" Width="360px" Height="15px" CssClass="Testo_08_Nero">Documento amministrativo d'accompagnamento per i prodotti soggetti ad accisa che circola non in regime sospensivo.</ASP:LABEL>
					<ASP:LABEL id="LABEL21" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 42px" runat="server"
						BackColor="#E6F4FF" Width="127px" Height="15px" CssClass="Testo_08_Nero_Bold">Esemplare:</ASP:LABEL>
					<asp:RadioButtonList id="Rbl_DAA_Esemplare" style="POSITION: absolute; LEFT: 6px; Z-INDEX: 102; TOP: 58px"
						runat="server" BackColor="#E6F4FF" Width="192px" Height="36px" CssClass="Testo_08_Nero">
						<asp:ListItem Value="1">per lo speditore</asp:ListItem>
						<asp:ListItem Value="2">per il destinatario</asp:ListItem>
						<asp:ListItem Value="3">da rinviare allo speditore</asp:ListItem>
						<asp:ListItem Value="4">per il paese di destinazione</asp:ListItem>
					</asp:RadioButtonList>
					<ASP:LABEL id="LABEL22" style="POSITION: absolute; LEFT: 200px; Z-INDEX: 103; TOP: 42px" runat="server"
						BackColor="#E6F4FF" Width="136px" Height="15px" CssClass="Testo_08_Nero_Bold">Selezionare la pagina da stampare:</ASP:LABEL>
					<asp:RadioButtonList id="Rbl_DAA_Pagina" style="POSITION: absolute; LEFT: 200px; Z-INDEX: 105; TOP: 80px"
						runat="server" BackColor="#E6F4FF" Width="104px" Height="3px" CssClass="Testo_08_Nero" RepeatDirection="Horizontal">
						<asp:ListItem Value="1">Avanti</asp:ListItem>
						<asp:ListItem Value="2">Retro</asp:ListItem>
					</asp:RadioButtonList>
					<ASP:IMAGEBUTTON id="ImgBtn_StampaDAA" style="POSITION: absolute; LEFT: 200px; Z-INDEX: 106; TOP: 120px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"
						ToolTip="Stampa il DAA"></ASP:IMAGEBUTTON>
					<asp:Label id="Label23" style="POSITION: absolute; LEFT: 240px; Z-INDEX: 107; TOP: 128px" runat="server"
						BackColor="#E6F4FF" Width="104px" Height="14px" CssClass="Testo_08_Rosso_Bold">Stampa il DAA</asp:Label>
					<ASP:IMAGEBUTTON id="ImgBtn_TelematizzazioneAccise" style="POSITION: absolute; LEFT: 144px; Z-INDEX: 108; TOP: 40px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/Doc10.ico"
						ToolTip="Telematizzazione Accise"></ASP:IMAGEBUTTON>
				</ASP:PANEL>
				<ASP:LABEL id="LABEL2" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 103; TOP: 0px" runat="server"
					BackColor="#C0FFC0" Width="192px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green"> &nbsp;Stampa Registri di Cantina:</ASP:LABEL>
				<ASP:PANEL id="Pannello_Temporale" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 0px; Z-INDEX: 104; TOP: 16px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="960px" Height="145px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BORDERCOLOR="#0000C0">&nbsp; 
<ASP:IMAGEBUTTON id="ImgBtn_Stampa" style="POSITION: absolute; LEFT: 904px; Z-INDEX: 100; TOP: 16px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"
						ToolTip="Stampa il Registro di Cantina"></ASP:IMAGEBUTTON>
<asp:CheckBox id="Chk_StampaIntestazione" style="POSITION: absolute; LEFT: 288px; Z-INDEX: 101; TOP: 8px"
						runat="server" BackColor="#E6F4FF" Width="138px" CssClass="Testo_08_Nero"
						Text="Stampa Intestazione"></asp:CheckBox>
<asp:CheckBox id="Chk_StampaRiporti" style="POSITION: absolute; LEFT: 456px; Z-INDEX: 102; TOP: 8px"
						runat="server" BackColor="#E6F4FF" Width="106px" Height="19px" CssClass="Testo_08_Nero"
						Text="Stampa Riporti" Visible="False"></asp:CheckBox>
<ASP:PANEL id="Pannello_Date" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 103; TOP: 48px"
						MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="216px"
						Height="55px" BORDERWIDTH="2px" BORDERSTYLE="None">
						<ASP:LABEL id="LABEL4" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 104; TOP: 9px" runat="server"
							BackColor="#E6F4FF" Width="88px" Height="15px" CssClass="Testo_08_Nero_Bold">Data Inizio :</ASP:LABEL>
						<ASP:LABEL id="LABEL5" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 105; TOP: 33px" runat="server"
							BackColor="#E6F4FF" Width="88px" Height="15px" CssClass="Testo_08_Nero_Bold">Data Fine :</ASP:LABEL>
						<ASP:TEXTBOX id="Txt_DataInizio" style="POSITION: absolute; LEFT: 96px; Z-INDEX: 106000; TOP: 6px"
							runat="server" BackColor="#FFFFFF" Width="104px" Height="18px" CssClass="Testo_08_Blue datepicker"
							MaxLength="10" BorderStyle="None"></ASP:TEXTBOX>
						<ASP:TEXTBOX id="Txt_DataFine" style="POSITION: absolute; LEFT: 96px; Z-INDEX: 1080000; TOP: 30px"
							runat="server" BackColor="#FFFFFF" Width="104px" Height="18px" CssClass="Testo_08_Blue datepicker"
							MaxLength="10" BorderStyle="None"></ASP:TEXTBOX>
					</ASP:PANEL>
<ASP:PANEL id="Pannello_AnnoMese" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 104; TOP: 48px"
						MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="258px"
						Height="64px" BORDERWIDTH="2px" BORDERSTYLE="None">&nbsp; 
<ASP:LABEL id="LABEL29" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 10px" runat="server"
							BackColor="#E6F4FF" Width="120px" Height="15px" CssClass="Testo_08_Nero_Bold">Seleziona l'anno:</ASP:LABEL>
<ASP:LABEL id="LABEL28" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 34px" runat="server"
							BackColor="#E6F4FF" Width="120px" Height="15px" CssClass="Testo_08_Nero_Bold">Seleziona il mese:</ASP:LABEL>
<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Anno" style="POSITION: absolute; LEFT: 128px; Z-INDEX: 102; TOP: 8px"
							runat="server" Width="117px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="False"></ASP:DROPDOWNLIST>
<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Mese" style="POSITION: absolute; LEFT: 128px; Z-INDEX: 103; TOP: 32px"
							runat="server" Width="117px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="False">
							<asp:ListItem Value="1">Gennaio</asp:ListItem>
							<asp:ListItem Value="2">Febbraio</asp:ListItem>
							<asp:ListItem Value="3">Marzo</asp:ListItem>
							<asp:ListItem Value="4">Aprile</asp:ListItem>
							<asp:ListItem Value="5">Maggio</asp:ListItem>
							<asp:ListItem Value="6">Giugno</asp:ListItem>
							<asp:ListItem Value="7">Luglio</asp:ListItem>
							<asp:ListItem Value="8">Agosto</asp:ListItem>
							<asp:ListItem Value="9">Settembre</asp:ListItem>
							<asp:ListItem Value="10">Ottobre</asp:ListItem>
							<asp:ListItem Value="11">Novembre</asp:ListItem>
							<asp:ListItem Value="12">Dicembre</asp:ListItem>
						</ASP:DROPDOWNLIST></ASP:PANEL>
<asp:RadioButtonList id="Rbl_MeseIntervallo" style="POSITION: absolute; LEFT: 5px; Z-INDEX: 105; TOP: 8px"
						runat="server" BackColor="#E6F4FF" Width="209px" Height="24px" CssClass="testo_08_nero"
						AutoPostBack="True" RepeatDirection="Horizontal">
						<asp:ListItem Value="0" Selected="True">Mese</asp:ListItem>
						<asp:ListItem Value="1">Intervallo Temporale</asp:ListItem>
					</asp:RadioButtonList>
<ASP:IMAGEBUTTON id="btnSalvaCacheRegistro" style="POSITION: absolute; LEFT: 904px; Z-INDEX: 106; TOP: 56px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/salva.ico"
						ToolTip="Stampa il Registro di Cantina"></ASP:IMAGEBUTTON>
<ASP:IMAGEBUTTON id="btnApriCache" style="POSITION: absolute; LEFT: 904px; Z-INDEX: 107; TOP: 96px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/doc5.ico"
						ToolTip="Stampa il Registro di Cantina"></ASP:IMAGEBUTTON>
<ASP:PANEL id="Pannello_Partita" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 108; TOP: 48px"
						MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="260px"
						Height="95px" BORDERWIDTH="2px" BORDERSTYLE="None" Visible="False">&nbsp; 
<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_LineaProduzione" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 100; TOP: 26px"
							runat="server" Width="252px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
<ASP:LABEL id="LABEL42" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 102; TOP: 6px" runat="server"
							BackColor="#E6F4FF" Width="121px" Height="15px" CssClass="Testo_08_Nero">Selezionare il vino</ASP:LABEL>
<ASP:LABEL id="LABEL43" style="POSITION: absolute; LEFT: 2px; Z-INDEX: 103; TOP: 48px" runat="server"
							BackColor="#E6F4FF" Width="131px" Height="15px" CssClass="Testo_08_Nero">Selezionare la partita</ASP:LABEL>
<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Partita" style="POSITION: absolute; LEFT: 1px; Z-INDEX: 104; TOP: 67px"
							runat="server" Width="251px" Height="18px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST></ASP:PANEL>
<ASP:PANEL id="Pannello_ContoTerzi" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 592px; Z-INDEX: 109; TOP: 8px"
						MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="298"
						Height="128px" BORDERWIDTH="2px" BORDERSTYLE="None" Visible="False">
						<ASP:LABEL id="LABEL37" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 100; TOP: 0px" runat="server"
							BackColor="#E6F4FF" Width="144px" Height="15px" CssClass="Testo_08_Nero">Conto Lavorazione:</ASP:LABEL>
						<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_ContattiContoTerzi" style="POSITION: absolute; LEFT: 26px; Z-INDEX: 101; TOP: 104px"
							runat="server" Width="262px" Height="18px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
						<asp:RadioButtonList id="Rbl_ContoTerzi" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 105; TOP: 16px"
							runat="server" BackColor="#E6F4FF" Width="296px" Height="3px" CssClass="Testo_08_Nero" AutoPostBack="True">
							<asp:ListItem Value="1">Stampa il registro complessivo</asp:ListItem>
							<asp:ListItem Value="2">Stampa un registro unico (voci di riepilogo separate per c/lav)</asp:ListItem>
							<asp:ListItem Value="3" Selected="True">Stampa un registro separato per ogni c/lav</asp:ListItem>
						</asp:RadioButtonList>
					</ASP:PANEL>
<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Centri_Registri" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 110; TOP: 32px"
						runat="server" Width="104px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"
						Visible="False"></ASP:DROPDOWNLIST>
<asp:Label id="Label40" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 111; TOP: 32px" runat="server"
						BackColor="#E6F4FF" Width="112px" Height="13px" CssClass="Testo_08_Nero"
						Visible="False">Centro aziendale:</asp:Label>
<asp:CheckBoxList id="CBL_Categoria" style="POSITION: absolute; LEFT: 352px; Z-INDEX: 113; TOP: 32px"
						runat="server" BackColor="#E6F4FF" Width="224px" Height="80px" CssClass="Testo_08_Nero"
						Visible="False">
						<asp:ListItem Value="19" Selected="True">Docg</asp:ListItem>
						<asp:ListItem Value="20" Selected="True">Dop</asp:ListItem>
						<asp:ListItem Value="21" Selected="True">Igp</asp:ListItem>
						<asp:ListItem Value="22" Selected="True">Tavola</asp:ListItem>
					</asp:CheckBoxList>
<asp:Label id="Lbl_Categoria" style="POSITION: absolute; LEFT: 288px; Z-INDEX: 114; TOP: 40px"
						runat="server" BackColor="#E6F4FF" Width="64px" Height="13px" CssClass="Testo_08_Nero"
						Visible="False">Categoria:</asp:Label></ASP:PANEL>
				<ASP:LABEL id="LABEL16" style="POSITION: absolute; LEFT: 488px; Z-INDEX: 105; TOP: 168px" runat="server"
					BackColor="#C0FFC0" Width="112px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green"> &nbsp;Registro vuoto:</ASP:LABEL>
				<ASP:PANEL id="Pannello_RegistroVuoto" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 488px; Z-INDEX: 106; TOP: 184px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="472px" Height="136px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BORDERCOLOR="#0000C0">
					<ASP:IMAGEBUTTON id="ImgBtn_StampaRegistroVuoto" style="POSITION: absolute; LEFT: 328px; Z-INDEX: 100; TOP: 80px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"
						ToolTip="Stampa il registro vuoto da vidimare"></ASP:IMAGEBUTTON>
					<ASP:LABEL id="LABEL11" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 40px" runat="server"
						BackColor="#E6F4FF" Width="240px" Height="15px" CssClass="Testo_08_Nero">Specificare la numerazione delle pagine</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_NumPagine" style="POSITION: absolute; LEFT: 40px; Z-INDEX: 102; TOP: 104px"
						runat="server" BackColor="#FFFFFF" Width="45px" Height="18px" CssClass="Testo_08_Blue" MaxLength="10"
						BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX id="Txt_NumPagineDa" style="POSITION: absolute; LEFT: 24px; Z-INDEX: 103; TOP: 56px"
						runat="server" BackColor="#FFFFFF" Width="45px" Height="18px" CssClass="Testo_08_Blue" MaxLength="10"
						BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX id="Txt_NumPagineA" style="POSITION: absolute; LEFT: 104px; Z-INDEX: 104; TOP: 56px"
						runat="server" BackColor="#FFFFFF" Width="45px" Height="18px" CssClass="Testo_08_Blue" MaxLength="10"
						BorderStyle="None"></ASP:TEXTBOX>
					<ASP:LABEL id="LABEL12" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 105; TOP: 56px" runat="server"
						BackColor="#E6F4FF" Width="14px" Height="15px" CssClass="Testo_08_Nero">da</ASP:LABEL>
					<ASP:LABEL id="LABEL13" style="POSITION: absolute; LEFT: 88px; Z-INDEX: 106; TOP: 56px" runat="server"
						BackColor="#E6F4FF" Width="14px" Height="15px" CssClass="Testo_08_Nero">a</ASP:LABEL>
					<ASP:LABEL id="LABEL14" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 107; TOP: 104px" runat="server"
						BackColor="#E6F4FF" Width="14px" Height="15px" CssClass="Testo_08_Nero">TOT</ASP:LABEL>
					<ASP:LABEL id="LABEL18" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 108; TOP: 88px" runat="server"
						BackColor="#E6F4FF" Width="280px" Height="15px" CssClass="Testo_08_Nero">... e il numero totale delle pagine (facoltativo):</ASP:LABEL>
					<asp:CheckBox id="Chk_IntestazioneRptVuoto" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 109; TOP: 8px"
						runat="server" BackColor="#E6F4FF" Width="138px" CssClass="Testo_08_Nero" Text="Stampa Intestazione"></asp:CheckBox>
					<ASP:LABEL id="LABEL15" style="POSITION: absolute; LEFT: 288px; Z-INDEX: 110; TOP: 16px" runat="server"
						BackColor="#E6F4FF" Width="177px" Height="27px" CssClass="Testo_08_Nero">Indicare il progressivo e la sigla del registro (facoltativo):</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_ProgrSigla_Registro" style="POSITION: absolute; LEFT: 296px; Z-INDEX: 111; TOP: 48px"
						runat="server" BackColor="#FFFFFF" Width="104px" Height="18px" CssClass="Testo_08_Blue" BorderStyle="None"></ASP:TEXTBOX>
				</ASP:PANEL>
				<ASP:LABEL id="LABEL17" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 107; TOP: 168px" runat="server"
					BackColor="#C0FFC0" Width="136px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green"> &nbsp;Copertina Registro:</ASP:LABEL>
				<ASP:PANEL id="Pannello_Copertina" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 0px; Z-INDEX: 108; TOP: 184px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="480px" Height="136px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BORDERCOLOR="#0000C0">
					<ASP:IMAGEBUTTON id="ImgBtn_StampaFrontespizio" style="POSITION: absolute; LEFT: 424px; Z-INDEX: 100; TOP: 88px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"
						ToolTip="Stampa la copertina del registro"></ASP:IMAGEBUTTON>
					<ASP:LABEL id="LABEL19" style="POSITION: absolute; LEFT: 232px; Z-INDEX: 101; TOP: 40px" runat="server"
						BackColor="#E6F4FF" Width="144px" Height="27px" CssClass="Testo_08_Nero">Indicare il progressivo e la sigla del registro (facoltativo):</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_Frontespizio" style="POSITION: absolute; LEFT: 392px; Z-INDEX: 102; TOP: 48px"
						runat="server" BackColor="#FFFFFF" Width="72px" Height="18px" CssClass="Testo_08_Blue" BorderStyle="None"></ASP:TEXTBOX>
					<asp:CheckBox id="Chk_Pagina1" style="POSITION: absolute; LEFT: 3px; Z-INDEX: 104; TOP: 8px" runat="server"
						BackColor="#E6F4FF" Width="232px" CssClass="Testo_08_Nero" Text="Visualizzare il numero di pagina (1)"></asp:CheckBox>
					<ASP:LABEL id="LABEL30" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 105; TOP: 40px" runat="server"
						BackColor="#E6F4FF" Width="128px" Height="15px" CssClass="Testo_08_Nero">Specificare il numero totale delle pagine:</ASP:LABEL>
					<ASP:LABEL id="LABEL31" style="POSITION: absolute; LEFT: 144px; Z-INDEX: 106; TOP: 48px" runat="server"
						BackColor="#E6F4FF" Width="14px" Height="15px" CssClass="Testo_08_Nero">TOT</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_Copertina_TotPagine" style="POSITION: absolute; LEFT: 176px; Z-INDEX: 107; TOP: 48px"
						runat="server" BackColor="#FFFFFF" Width="45px" Height="18px" CssClass="Testo_08_Blue" MaxLength="10"
						BorderStyle="None"></ASP:TEXTBOX>
					<asp:Label id="Label41" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 108; TOP: 80px" runat="server"
						BackColor="#E6F4FF" Width="174px" Height="13px" CssClass="Testo_08_Nero">Indirizzo sede cantina: </asp:Label>
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Indirizzo" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 109; TOP: 96px"
						runat="server" Width="400px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
					<asp:CheckBox id="Chk_SaNome" style="POSITION: absolute; LEFT: 240px; Z-INDEX: 110; TOP: 8px"
						runat="server" BackColor="#E6F4FF" Width="208px" CssClass="Testo_08_Nero" Text="Stampa nome Centro Aziendale"></asp:CheckBox>
				</ASP:PANEL>
				<ASP:LABEL id="LABEL6" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 109; TOP: 328px" runat="server"
					BackColor="#C0FFC0" Width="224px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green"> &nbsp;Stampa Consistenze Enologiche:</ASP:LABEL>
				<ASP:PANEL id="Pannello_Debug" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 0px; Z-INDEX: 113; TOP: 472px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="580px" Height="166px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BORDERCOLOR="#0000C0">
					<asp:Label id="Label411" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 102; TOP: 136px" runat="server"
						BackColor="#E6F4FF" Width="72px" Height="13px" CssClass="Testo_08_Nero">Mag./vasca:</asp:Label>
					<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_MagazzinoVasca" style="POSITION: absolute; LEFT: 96px; Z-INDEX: 101; TOP: 136px"
						runat="server" Width="416px" Height="18px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
					<ASP:IMAGEBUTTON id="ImgBtn_VerificaRegistri" style="POSITION: absolute; LEFT: 520px; Z-INDEX: 100; TOP: 128px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"></ASP:IMAGEBUTTON>
					<ASP:PANEL id="Pannello_VoceRiepilogo" style="POSITION: absolute; LEFT: 96px; Z-INDEX: 103; TOP: 16px"
						MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="472px" Height="115"
						BORDERWIDTH="2px" BORDERSTYLE="None" Visible="False">
						<asp:Label id="Label38" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 101; TOP: 0px" runat="server"
							BackColor="#E6F4FF" Width="174px" Height="13px" CssClass="Testo_08_Nero">Seleziona voce di riepilogo:</asp:Label>
						<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_VociRiepilogo" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 102; TOP: 16px"
							runat="server" Width="464px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
						<asp:Label id="Label399" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 103; TOP: 40px" runat="server"
							BackColor="#E6F4FF" Width="174px" Height="13px" CssClass="Testo_08_Nero">Seleziona un prodotto:</asp:Label>
						<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_MateriePrime" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 104; TOP: 56px"
							runat="server" Width="464px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
						<asp:Label id="Label364" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 105; TOP: 80px" runat="server"
							BackColor="#E6F4FF" Width="174px" Height="13px" CssClass="Testo_08_Nero">Seleziona una linea:</asp:Label>
						<ASP:DROPDOWNLIST tabIndex="16" id="cmb_LineeByMat_Cod" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 106; TOP: 96px"
							runat="server" Width="464px" Height="18px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
					</ASP:PANEL>
					<asp:RadioButtonList id="Rbl_Verifica" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 105; TOP: 8px"
						runat="server" BackColor="#E6F4FF" Width="88px" Height="3px" CssClass="Testo_08_Nero" AutoPostBack="True">
						<asp:ListItem Value="0">Linea</asp:ListItem>
						<asp:ListItem Value="1" Selected="True">Voce Riep.</asp:ListItem>
					</asp:RadioButtonList>
					<ASP:PANEL id="Pannello_Linee" style="POSITION: absolute; LEFT: 96px; Z-INDEX: 104; TOP: 16px"
						MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="472px" Height="115"
						BORDERWIDTH="2px" BORDERSTYLE="None" Visible="False">
						<asp:Label id="Label145" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 105; TOP: 0px" runat="server"
							BackColor="#E6F4FF" Width="174px" Height="13px" CssClass="Testo_08_Nero">Seleziona una linea:</asp:Label>
						<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_Linee" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 106; TOP: 16px"
							runat="server" Width="464px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>
						<asp:Label id="Label144" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 103; TOP: 40px" runat="server"
							BackColor="#E6F4FF" Width="174px" Height="13px" CssClass="Testo_08_Nero">Seleziona un prodotto:</asp:Label>
						<ASP:DROPDOWNLIST tabIndex="16" id="Cmb_MateriePrimeByLinee" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 104; TOP: 56px"
							runat="server" Width="464px" Height="18px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
					</ASP:PANEL>
					<asp:RadioButtonList id="Rbl_CauMov" style="POSITION: absolute; LEFT: 384px; Z-INDEX: 106; TOP: 0px"
						runat="server" BackColor="#E6F4FF" Width="88px" Height="3px" CssClass="Testo_08_Nero" AutoPostBack="True"
						RepeatDirection="Horizontal">
						<asp:ListItem Value="0" Selected="True">Tutti</asp:ListItem>
						<asp:ListItem Value="7300">Carichi</asp:ListItem>
						<asp:ListItem Value="7350">Scarichi</asp:ListItem>
					</asp:RadioButtonList>
				</ASP:PANEL>
				<ASP:LABEL id="LABEL39" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 114; TOP: 456px" runat="server"
					BackColor="#C0FFC0" Width="125px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green"> &nbsp;Verifica Registri:</ASP:LABEL>
			</ASP:PANEL>
			<ASP:PANEL id="Pannello_StampaEtichetteVasche" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 1096px; Z-INDEX: 111; TOP: 824px"
				MS_POSITIONING="GridLayout" runat="server" BackColor="White" Width="970px" Height="770px"
				BORDERWIDTH="2px" BORDERSTYLE="None">
				<ASP:LABEL id="LABEL46" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 113; TOP: 0px" runat="server"
					BackColor="#C0FFC0" Width="256px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green"> &nbsp;Stampa Etichette Vasche Enologiche:</ASP:LABEL>
				<ASP:PANEL id="Pannello_EtichetteVasche" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 0px; Z-INDEX: 115; TOP: 16px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="930px" Height="750px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BorderColor="#0000C0">
					<ASP:IMAGEBUTTON id="ImgBtn_StampaEtichetteVasche" style="POSITION: absolute; LEFT: 48px; Z-INDEX: 100; TOP: 312px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"
						ToolTip="Stampa il registro vuoto da vidimare"></ASP:IMAGEBUTTON>
					<asp:CheckBoxList id="CBL_InfoEtichetteVasche" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 32px"
						runat="server" BackColor="#E6F4FF" Width="300px" CssClass="Testo_08_Nero">
						<asp:ListItem Value="0" Selected="True">Identificativo</asp:ListItem>
						<asp:ListItem Value="1" Selected="True">Capacit&#224; HL</asp:ListItem>
						<asp:ListItem Value="2" Selected="True">Linea di produzione</asp:ListItem>
						<asp:ListItem Value="5" Selected="True">Colore</asp:ListItem>
						<asp:ListItem Value="6" Selected="True">Anno di produzione</asp:ListItem>
						<asp:ListItem Value="3" Selected="True">Grado Babo e/o Indice di Refrazione</asp:ListItem>
						<asp:ListItem Value="7" Selected="True">Regolamento</asp:ListItem>
						<asp:ListItem Value="8" Selected="True">Atto di approvazione doc/docg</asp:ListItem>
						<asp:ListItem Value="4" Selected="True">Fornitore del c/lavorazione</asp:ListItem>
						<asp:ListItem Value="9" Selected="True">Quantit&#224; attuale litri</asp:ListItem>
					</asp:CheckBoxList>
					<ASP:LABEL id="LABEL47" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 102; TOP: 8px" runat="server"
						BackColor="#E6F4FF" Width="308px" Height="15px" CssClass="Testo_08_Nero">Seleziona le informazioni che  si desidera stampare:</ASP:LABEL>
					<asp:CheckBoxList id="CBL_VascheElenco" style="POSITION: absolute; LEFT: 352px; Z-INDEX: 103; TOP: 56px"
						runat="server" BackColor="#E6F4FF" Width="518px" CssClass="Testo_08_Nero"></asp:CheckBoxList>
					<ASP:LABEL id="LABEL48" style="POSITION: absolute; LEFT: 352px; Z-INDEX: 104; TOP: 8px" runat="server"
						BackColor="#E6F4FF" Width="448px" Height="15px" CssClass="Testo_08_Nero">Seleziona le vasche di cui si desidera stampare l'etichetta:</ASP:LABEL>
					<ASP:LABEL id="LABEL49" style="POSITION: absolute; LEFT: 96px; Z-INDEX: 105; TOP: 320px" runat="server"
						BackColor="#E6F4FF" Width="112px" Height="15px" CssClass="Testo_08_Nero">Stampa le etichette</ASP:LABEL>
					<asp:imagebutton id="ImgBtn_SelezionaTutti" style="POSITION: absolute; LEFT: 352px; Z-INDEX: 106; TOP: 24px"
						runat="server" BackColor="#E6F4FF" ImageUrl="../../AB_Immagini/icone24/ValidazioneSI_24.ico"></asp:imagebutton>
					<asp:Label id="Lbl_SelezionaTutteSpecie" style="POSITION: absolute; LEFT: 384px; Z-INDEX: 108; TOP: 32px"
						runat="server" BackColor="#E6F4FF" Width="96px" CssClass="Testo_07_Nero">Seleziona Tutto</asp:Label>
					<asp:imagebutton id="ImgBtn_DeselezionaTutti" style="POSITION: absolute; LEFT: 504px; Z-INDEX: 109; TOP: 24px"
						runat="server" BackColor="#E6F4FF" ImageUrl="../../AB_Immagini/icone24/ValidazioneNO_24.ico"></asp:imagebutton>
					<asp:Label id="Lbl_DeselezionaTutteSpecie" style="POSITION: absolute; LEFT: 536px; Z-INDEX: 110; TOP: 32px"
						runat="server" BackColor="#E6F4FF" Width="96px" CssClass="Testo_07_Nero">Deseleziona Tutto</asp:Label>
				</ASP:PANEL>
			</ASP:PANEL><ASP:LABEL style="POSITION: absolute; LEFT: 1000px; Z-INDEX: 109; TOP: 104px" id="LABEL33"
				runat="server" Height="15px" Width="280px" BackColor="#E6F4FF" CssClass="Testo_08_Nero_Bold" Visible="False">Selezionare la Tipologia di Registro:</ASP:LABEL><ASP:DROPDOWNLIST style="POSITION: absolute; LEFT: 1000px; Z-INDEX: 101; TOP: 128px" id="Cmb_TipiRegistro"
				tabIndex="16" runat="server" Height="18px" Width="288px" CssClass="Testo_08_Blue" Visible="False" Enabled="False"></ASP:DROPDOWNLIST><ASP:PANEL style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 1104px; Z-INDEX: 108; TOP: 256px"
				id="Pannello_StampaDOCO" MS_POSITIONING="GridLayout" runat="server" BORDERSTYLE="None" BORDERWIDTH="2px" Height="532px" Width="968px" BackColor="White">
				<ASP:LABEL id="LABEL9" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 113; TOP: 0px" runat="server"
					BackColor="#C0FFC0" Width="360px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green"> &nbsp;Stampa il DOCO con o senza il layout del documento:</ASP:LABEL>
				<ASP:PANEL id="Pannello_Layout_DOCO" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 115; TOP: 16px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="360px" Height="112px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BorderColor="#0000C0">
					<asp:RadioButtonList id="Rbl_Layout" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 114; TOP: 8px" runat="server"
						BackColor="#E6F4FF" Width="166px" Height="64px" CssClass="Testo_08_Nero" BorderStyle="None">
						<asp:ListItem Value="1">Stampa Con il Layout</asp:ListItem>
						<asp:ListItem Value="2" Selected="True">Stampa Senza Layout</asp:ListItem>
					</asp:RadioButtonList>
					<ASP:IMAGEBUTTON id="ImgBtn_StampaSceltaLayout" style="POSITION: absolute; LEFT: 240px; Z-INDEX: 115; TOP: 56px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"
						ToolTip="Stampa il registro vuoto da vidimare"></ASP:IMAGEBUTTON>
					<asp:CheckBox id="Chk_StampaIntestazione_DOCO_Layout" style="POSITION: absolute; LEFT: 192px; Z-INDEX: 116; TOP: 16px"
						runat="server" BackColor="#E6F4FF" Width="138px" CssClass="Testo_08_Nero" Text="Stampa Intestazione"></asp:CheckBox>
				</ASP:PANEL>
				<ASP:PANEL id="Pannello_DOCO" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 116; TOP: 144px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="136px" Height="102px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BORDERCOLOR="#0000C0">
					<ASP:IMAGEBUTTON id="ImgBtn_StampaDOCO" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 64px"
						runat="server" BackColor="#E6F4FF" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/Icone32/stampa.ico"
						ToolTip="Stampa il DOCO"></ASP:IMAGEBUTTON>
					<asp:Label id="Label26" style="POSITION: absolute; LEFT: 48px; Z-INDEX: 102; TOP: 64px" runat="server"
						BackColor="#E6F4FF" Width="80px" Height="13px" CssClass="Testo_08_Rosso_Bold">Stampa il DOCO vuoto</asp:Label>
					<asp:CheckBox id="Chk_StampaIntestazione_DOCO_Vuoto" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 103; TOP: 8px"
						runat="server" BackColor="#E6F4FF" Width="80px" CssClass="Testo_08_Nero" Text="Stampa Intestazione"></asp:CheckBox>
				</ASP:PANEL>
			</ASP:PANEL><ASP:LABEL style="POSITION: absolute; LEFT: 2224px; Z-INDEX: 106; TOP: 1376px" id="LABEL27"
				runat="server" Height="53px" Width="154px" BackColor="#C0FFC0" CssClass="Testo_08_Rosso_Bold" ForeColor="Green"
				Visible="False"></ASP:LABEL><ASP:PANEL style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 96px"
				id="Pannello_TipoReport" MS_POSITIONING="GridLayout" runat="server" BORDERSTYLE="None" Height="112px" Width="968px" BackColor="White">
				<ASP:PANEL id="Pannello_Report" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #e6f4ff; POSITION: absolute; LEFT: 0px; Z-INDEX: 104; TOP: 16px"
					MS_POSITIONING="GridLayout" runat="server" BackColor="#E6F4FF" Width="960px" Height="96px"
					BORDERWIDTH="1px" BORDERSTYLE="Solid" BORDERCOLOR="#0000C0">
					<asp:RadioButtonList id="Rbl_Report" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 103; TOP: 24px" runat="server"
						BackColor="#E6F4FF" Width="920px" CssClass="Testo_08_Nero" AutoPostBack="True" CellSpacing="0" RepeatColumns="2"></asp:RadioButtonList>
					<ASP:LABEL id="LABEL34" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 105; TOP: 8px" runat="server"
						BackColor="#E6F4FF" Width="272px" Height="15px" CssClass="Testo_08_Nero_Bold">&nbsp;Selezionare il registro da stampare :</ASP:LABEL>
					<asp:Label id="Label44" style="POSITION: absolute; LEFT: 848px; Z-INDEX: 113; TOP: 8px" runat="server"
						BackColor="#E6F4FF" Width="96px" Height="13px" CssClass="Testo_08_Nero">Gestione OMNI:</asp:Label>
					<asp:RadioButtonList id="Rbl_OMNI" style="POSITION: absolute; LEFT: 840px; Z-INDEX: 114; TOP: 40px" runat="server"
						BackColor="#E6F4FF" Width="94px" Height="24px" CssClass="testo_08_nero" Enabled="False">
						<asp:ListItem Value="0">NON attiva</asp:ListItem>
						<asp:ListItem Value="1">ATTIVA</asp:ListItem>
					</asp:RadioButtonList>
				</ASP:PANEL>
				<ASP:LABEL id="LABEL32" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 105; TOP: 0px" runat="server"
					BackColor="#C0FFC0" Width="136px" Height="15px" CssClass="Testo_08_Rosso_Bold" ForeColor="Green">&nbsp;Selezione Registro:</ASP:LABEL>
			</ASP:PANEL>
			<table aria-hidden="true" style="HEIGHT: 41px; WIDTH: 771px; POSITION: absolute; LEFT: 122px; Z-INDEX: 102; TOP: 10px"
				id="Table3" border="0" cellSpacing="1" cellPadding="1" width="771">
				<TR>
					<TD style="WIDTH: 41px"><ASP:IMAGE id="IMAGE2" runat="server" Height="32px" Width="32px" ImageUrl="../../AB_Immagini/icone32/stampa.ico"></ASP:IMAGE></TD>
					<TD bgColor="#5c9ccc" vAlign="middle" align="left">&nbsp;
						<ASP:LABEL id="LABEL3" runat="server" Height="7px" Width="465px" BackColor="#5C9CCC" CssClass="Testo_12_Nero_Bold"> Registri e Stampe di Cantina</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<table aria-hidden="true" style="HEIGHT: 26px; POSITION: absolute; LEFT: 122px; Z-INDEX: 103; TOP: 58px" id="Table4"
				border="0" cellSpacing="1" cellPadding="1" width="849">
				<TR>
					<TD bgColor="#5c9ccc">&nbsp;
						<ASP:TEXTBOX id="Txt_RagSoc" runat="server" Height="16px" Width="808px" BackColor="#5C9CCC" CssClass="Testo_10_Nero_Bold"
							ReadOnly="True" MaxLength="10" BorderStyle="None"></ASP:TEXTBOX></TD>
				</TR>
			</TABLE>
			<ASP:IMAGEBUTTON style="POSITION: absolute; LEFT: 914px; Z-INDEX: 104; TOP: 18px" id="ImgBtnEsci"
				runat="server" Height="32px" Width="32px" ImageUrl="../../AB_Immagini/icone32/esci.bmp"></ASP:IMAGEBUTTON><ASP:IMAGE style="POSITION: absolute; LEFT: 2px; Z-INDEX: 105; TOP: 2px" id="ImageLogo" runat="server"
				Height="73px" Width="96px" ImageUrl="../../AB_Immagini/logo/Logo_GiasOnline_Mini.jpg"></ASP:IMAGE>
			</form>
	</body>
</HTML>
