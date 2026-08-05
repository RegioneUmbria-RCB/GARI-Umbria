<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Filtro_Report_AccettazioneDaDiversi.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_Report_AccettazioneDaDiversi"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
	<HEAD>
		<title>Filtro_Report_AccettazioneDaDiversi</title>
		<meta name="vs_snapToGrid" content="True">
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK rel="stylesheet" type="text/css" href="../../Styles/AgronicaStyle.css">
		<SCRIPT language="vbscript">
'			sub BtnImpostaData1_Clikkalo()
'				a = window.showModalDialog("../../AA_Script/Controlli/AgroCalendario/AgroCalendario.aspx?dsel=" & document.all("Txt_ValiditaInizio").value,"","dialogWidth:280px;dialogHeight:350px;status:no; center:yes;edge:raised; help:no;")
'				'assegno il valore di ritorno della finestra modale
'				if a<>"" then
'					if a="-1" then
'						document.all("Txt_ValiditaInizio").value = ""
'					else
'						document.all("Txt_ValiditaInizio").value = a
'					end if
'				end if
'			end sub
'			sub BtnImpostaData2_Clikkalo()
'				a = window.showModalDialog("../../AA_Script/Controlli/AgroCalendario/AgroCalendario.aspx?dsel=" & document.all("Txt_ValiditaFine").value,"","dialogWidth:280px;dialogHeight:350px;status:no; center:yes;edge:raised; help:no;")
'				'assegno il valore di ritorno della finestra modale
'				if a<>"" then
'					if a="-1" then
'						document.all("Txt_ValiditaFine").value = ""
'					else
'						document.all("Txt_ValiditaFine").value = a
'					end if
'				end if
'			end sub
'			sub BtnImpostaDataPom_Clikkalo()
'				a = window.showModalDialog("../../AA_Script/Controlli/AgroCalendario/AgroCalendario.aspx?dsel=" & document.all("Txt_DataRegPom").value,"","dialogWidth:280px;dialogHeight:350px;status:no; center:yes;edge:raised; help:no;")
'				'assegno il valore di ritorno della finestra modale
'				if a<>"" then
'					if a="-1" then
'						document.all("Txt_DataRegPom").value = ""
'					else
'						document.all("Txt_DataRegPom").value = a
'					end if
'				end if
'			end sub
'			sub BtnImpostaDataGiacenza_Clikkalo()
'				a = window.showModalDialog("../../AA_Script/Controlli/AgroCalendario/AgroCalendario.aspx?dsel=" & document.all("Txt_DataGiacenza").value,"","dialogWidth:280px;dialogHeight:350px;status:no; center:yes;edge:raised; help:no;")
'				'assegno il valore di ritorno della finestra modale
'				if a<>"" then
'					if a="-1" then
'						document.all("Txt_DataGiacenza").value = ""
'					else
'						document.all("Txt_DataGiacenza").value = a
'					end if
'				end if
'			end sub
		</SCRIPT>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table aria-hidden="true" id="TableTitolo" style="HEIGHT: 41px; WIDTH: 904px; POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 8px"
				cellSpacing="1" cellPadding="1" width="904" border="0">
				<TR>
					<TD style="WIDTH: 41px"><ASP:IMAGE id="ImgIcona" runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/icone32/Stampa.ico"></ASP:IMAGE></TD>
					<TD bgColor="#00bfff" vAlign="middle" align="left">&nbsp;
						<ASP:LABEL id="LblTitolo" runat="server" Width="688px" Height="7px" CssClass="Testo_12_Nero_Bold"
							BackColor="DeepSkyBlue">Filtro Report Accettazione da Diversi</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<INPUT id="Txt_FlagInsertNumCert" style="HEIGHT: 1px; WIDTH: 1px; POSITION: absolute; LEFT: 960px; Z-INDEX: 107; TOP: 16px"
				size="1" name="Txt_FlagInsertNumCert" runat="server">
			<ASP:PANEL id="Pannello_CertificatiPomodoro" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #afeeee; POSITION: absolute; LEFT: 1064px; Z-INDEX: 106; TOP: 440px"
				MS_POSITIONING="GridLayout" runat="server" Width="440px" Height="352px" BackColor="White"
				BORDERWIDTH="2px" BORDERCOLOR="#0000C0" BORDERSTYLE="Solid" Visible="False">&nbsp; 
<asp:ImageButton id="ImgBtn_StampaCertificatoPomodoro" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 100; TOP: 240px; " TabIndex="12"
					runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico"></asp:ImageButton>
<asp:label id="Label31" style="POSITION: absolute; LEFT: 160px; Z-INDEX: 101; TOP: 248px" runat="server"
					Height="14px" Width="148px" CssClass="testo_08_blue_bold">Stampa il certificato</asp:label>
<asp:Label id="Label30" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 102; TOP: 16px" runat="server"
					Height="16px" Width="348px" CssClass="Testo_10_rosso_bold">Stampa Bolla e Certificati del Pomodoro</asp:Label>
<asp:RadioButtonList id="Rbl_Certificati" style="POSITION: absolute; LEFT: 224px; Z-INDEX: 104; TOP: 88px; " TabIndex="1"
					runat="server" Height="70px" Width="140px" CssClass="testo_08_nero" AutoPostBack="True"
					Enabled="False">
					<asp:ListItem Value="1" Selected="True">Certificato Interno</asp:ListItem>
					<asp:ListItem Value="2">Certificato Esterno</asp:ListItem>
				</asp:RadioButtonList>
<asp:RadioButtonList id="Rbl_StampaPomodoro" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 105; TOP: 64px; " TabIndex="1"
					runat="server" Height="70px" Width="196px" CssClass="testo_08_nero" AutoPostBack="True">
					<asp:ListItem Value="1">Stampa Bolla Accettazione</asp:ListItem>
					<asp:ListItem Value="2" Selected="True">Stampa dei Certificati</asp:ListItem>
				</asp:RadioButtonList></ASP:PANEL><ASP:PANEL id="Pannello_ConfermaStampaMassiva" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #afeeee; POSITION: absolute; LEFT: 1064px; Z-INDEX: 105; TOP: 72px"
				MS_POSITIONING="GridLayout" runat="server" Width="492px" Height="352px" BackColor="White" BORDERWIDTH="2px" BORDERCOLOR="#0000C0"
				BORDERSTYLE="Solid">&nbsp; 
<asp:ImageButton id="ImgBtn_StampaMassivaBolle" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 100; TOP: 296px; " TabIndex="12"
					runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico"></asp:ImageButton>
<asp:label id="lbl_stampamassiva" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 101; TOP: 304px"
					runat="server" Height="14px" Width="56px" CssClass="testo_08_blue_bold">Stampa</asp:label>
<asp:Label id="Lbl_ConfermaStampaMassiva" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 102; TOP: 8px"
					runat="server" Height="16px" Width="436px" CssClass="Testo_10_rosso_bold">Conferma Stampa Massiva Bolle</asp:Label>
<ASP:IMAGEBUTTON id="ImgBtn_AnnullaStampaMassiva" style="POSITION: absolute; LEFT: 272px; Z-INDEX: 103; TOP: 296px"
					runat="server" ImageUrl="../../AB_Immagini/icone32/esci.bmp" Height="32px" Width="32px"></ASP:IMAGEBUTTON>
<asp:label id="Label22" style="POSITION: absolute; LEFT: 312px; Z-INDEX: 104; TOP: 304px" runat="server"
					Height="14px" Width="56px" CssClass="testo_08_blue_bold">Annulla</asp:label>
<asp:Label id="Label23" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 105; TOP: 40px" runat="server"
					Height="16px" Width="236px" CssClass="Testo_08_Nero_bold">Stampante selezionata:</asp:Label>
<asp:TextBox id="Txt_RiepilogoBolle" style="SCROLLBAR-FACE-COLOR: #c0ffc0; POSITION: absolute; LEFT: 16px; Z-INDEX: 106; TOP: 104px"
					runat="server" Height="120px" Width="444px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
					TextMode="MultiLine" BorderStyle="None" ReadOnly="True"></asp:TextBox>
<asp:TextBox id="Txt_NomeStampante" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 107; TOP: 56px"
					runat="server" Height="16px" Width="444px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
					BorderStyle="None" ReadOnly="True"></asp:TextBox>
<asp:Label id="Lbl_SelezioneStampaMassiva" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 108; TOP: 88px"
					runat="server" Height="16px" Width="444px" CssClass="Testo_08_Nero_bold">Filtro selezionato per la stampa:</asp:Label>
<asp:Label id="Label25" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 109; TOP: 240px" runat="server"
					Height="16px" Width="188px" CssClass="Testo_08_Nero_bold">Numero copie impostate:</asp:Label>
<asp:TextBox id="Txt_NumeroCopie_Bis" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 110; TOP: 256px"
					runat="server" Height="16px" Width="164px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
					BorderStyle="None" ReadOnly="True"></asp:TextBox>
<asp:TextBox id="Txt_TipoStampa" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 111; TOP: 256px"
					runat="server" Height="16px" Width="102px" BackColor="Magenta" CssClass="Testo_08_Blue"
					Visible="False" BorderStyle="None" ReadOnly="True"></asp:TextBox></ASP:PANEL><asp:label id="Label6" style="POSITION: absolute; LEFT: 2080px; Z-INDEX: 104; TOP: 1144px"
				runat="server" Width="64px" Height="48px" Visible="False">Label</asp:label><asp:panel id="Pannello_Generale" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 103; TOP: 56px"
				MS_POSITIONING="GridLayout" runat="server" Width="956px" Height="684px" BackColor="White" BorderStyle="Solid" BorderColor="#0000C0" BorderWidth="2px">&nbsp; 
<ASP:LABEL id="LABEL1" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 16px" runat="server"
					Height="15px" Width="312px" CssClass="Testo_08_Nero_Bold">Selezionare il Report che si desidera stampare:</ASP:LABEL>
<asp:RadioButtonList id="Rbl_Report" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 32px; " TabIndex="1"
					runat="server" Height="200px" Width="256px" CssClass="testo_08_nero" AutoPostBack="true">
					<asp:ListItem Value="120" Selected="True">Riepilogo Conferimenti x Specie</asp:ListItem>
					<asp:ListItem Value="121">Estratto Conto Bolle di Accettazione</asp:ListItem>
					<asp:ListItem Value="122">Estratto Conto Imballi</asp:ListItem>
					<asp:ListItem Value="123">Saldo Imballi</asp:ListItem>
					<asp:ListItem Value="124">Esportazione Excel Bolle di Accettazione</asp:ListItem>
					<asp:ListItem Value="125">Esportazione Excel Trasportatori</asp:ListItem>
					<asp:ListItem Value="113">Stampa Massiva Bolle Accettazione</asp:ListItem>
					<asp:ListItem Value="136">Stampa Massiva Certificati Pomodoro</asp:ListItem>
					<asp:ListItem Value="143">Esportazione Excel Certificati Pomodoro</asp:ListItem>
					<asp:ListItem Value="174">Excel Tracciabilit&#224; Conferimenti</asp:ListItem>
					<asp:ListItem Value="139">Registro di Carico e Scarico - Pomodoro</asp:ListItem>
				</asp:RadioButtonList>
<asp:Panel id="Pannello_Date" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 102; TOP: 280px"
					runat="server" Height="32px" Width="476px" ms_positioning="gridlayout">
					<ASP:LABEL id="LABEL3" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 100; TOP: 8px" runat="server"
						Height="16px" Width="24px" CssClass="Testo_08_Nero_Bold">Dal:</ASP:LABEL>
					<ASP:LABEL id="Label72" style="POSITION: absolute; LEFT: 328px; Z-INDEX: 101; TOP: 8px" runat="server"
						Height="16px" Width="24px" CssClass="Testo_08_Nero_Bold">Al:</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_ValiditaInizio" style="POSITION: absolute; LEFT: 200px; Z-INDEX: 102; TOP: 7px; " TabIndex="2"
						runat="server" Height="16px" Width="88px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None" MaxLength="10"></ASP:TEXTBOX>
					<INPUT onclick="BtnImpostaData1_Clikkalo" id="BtnImpostaData1" class="Testo_08_Nero_Bold"
						style="HEIGHT: 17px; POSITION: absolute; LEFT: 296px; Z-INDEX: 103; TOP: 8px" height="17"
						size="16" type="button" width="25" value="..." name="BtnImpostaData1">
					<ASP:TEXTBOX id="Txt_ValiditaFine" style="POSITION: absolute; LEFT: 352px; Z-INDEX: 104; TOP: 7px; " TabIndex="3"
						runat="server" Height="16px" Width="88px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None" MaxLength="10"></ASP:TEXTBOX>
					<INPUT onclick="BtnImpostaData2_Clikkalo" id="BtnImpostaData2" class="Testo_08_Nero_Bold"
						style="HEIGHT: 17px; POSITION: absolute; LEFT: 448px; Z-INDEX: 105; TOP: 8px" height="17"
						size="16" type="button" width="25" value="..." name="BtnImpostaData2">
					<ASP:LABEL id="LABEL5" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 106; TOP: 8px" runat="server"
						Height="16px" Width="152px" CssClass="Testo_08_Nero_Bold">Intervallo Temporale:</ASP:LABEL>
				</asp:Panel>
<ASP:PANEL id="Pannello_Stampa" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #afeeee; POSITION: absolute; LEFT: 808px; Z-INDEX: 103; TOP: 32px"
					MS_POSITIONING="GridLayout" runat="server" Height="48px" Width="108px"
					BackColor="White" BORDERSTYLE="None" BORDERCOLOR="#0000C0" BORDERWIDTH="0px">&nbsp; 
<asp:ImageButton id="ImgBtn_Stampa" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 8px; " TabIndex="12"
						runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico"></asp:ImageButton>
<asp:label id="Lbl_Stampa" style="POSITION: absolute; LEFT: 48px; Z-INDEX: 102; TOP: 16px"
						runat="server" Height="14px" Width="56px" CssClass="testo_08_blue_bold">Stampa</asp:label></ASP:PANEL>
<asp:Panel id="Pannello_Conferente" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 105; TOP: 448px"
					runat="server" Height="62px" Width="928px" ms_positioning="gridlayout">&nbsp; 
<ASP:LABEL id="LABEL4" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px" runat="server"
						Height="15px" Width="88px" CssClass="Testo_08_Nero_Bold">Conferente:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_Da_CodiceConferente" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 101; TOP: 8px; " TabIndex="4"
						runat="server" Height="16px" Width="72px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL7" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 102; TOP: 8px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">Da:</ASP:LABEL>
<ASP:LABEL id="LABEL8" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 103; TOP: 32px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">A:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_A_CodiceConferente" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 104; TOP: 32px; " TabIndex="6"
						runat="server" Height="16px" Width="72px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></ASP:TEXTBOX>
<asp:Button id="Btn_Da_Conferente" style="POSITION: absolute; LEFT: 216px; Z-INDEX: 105; TOP: 8px; " TabIndex="5"
						runat="server" Height="20px" Width="48px" CssClass="Testo_08_nero" Text="Carica"></asp:Button>
<asp:Button id="Btn_A_Conferente" style="POSITION: absolute; LEFT: 216px; Z-INDEX: 106; TOP: 32px; " TabIndex="7"
						runat="server" Height="20px" Width="48px" CssClass="Testo_08_nero" Text="Carica"></asp:Button>
<ASP:TEXTBOX id="Txt_Da_Conferente" style="POSITION: absolute; LEFT: 272px; Z-INDEX: 107; TOP: 8px"
						runat="server" Height="16px" Width="320px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
<ASP:TEXTBOX id="Txt_A_Conferente" style="POSITION: absolute; LEFT: 272px; Z-INDEX: 108; TOP: 32px"
						runat="server" Height="16px" Width="320px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
<ASP:TEXTBOX id="Txt_SringaFiltroConferenti" style="POSITION: absolute; LEFT: 600px; Z-INDEX: 109; TOP: 8px"
						runat="server" Height="42px" Width="320px" BackColor="White" CssClass="Testo_08_Blue"
						TextMode="MultiLine" BorderStyle="None" ReadOnly="True" MaxLength="10"></ASP:TEXTBOX>
<ASP:TEXTBOX id="Txt_CodContatto_DA_Conferente" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 110; TOP: 24px; " TabIndex="4"
						runat="server" Height="16px" Width="40px" BackColor="LightCoral" CssClass="Testo_08_Blue"
						Visible="False" BorderStyle="None" MaxLength="10"></ASP:TEXTBOX>
<ASP:TEXTBOX id="Txt_CodContatto_A_Conferente" style="POSITION: absolute; LEFT: 56px; Z-INDEX: 111; TOP: 24px; " TabIndex="4"
						runat="server" Height="16px" Width="40px" BackColor="LightCoral" CssClass="Testo_08_Blue"
						Visible="False" BorderStyle="None" MaxLength="10"></ASP:TEXTBOX></asp:Panel>
<asp:Panel id="Pannello_Magazzini" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 106; TOP: 312px"
					runat="server" Height="36px" Width="928px" ms_positioning="gridlayout">&nbsp; 
<asp:label id="lbl_AnnoContabile" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px"
						runat="server" Height="14px" Width="80px" CssClass="testo_08_nero_bold">Magazzino:</asp:label>
<asp:dropdownlist id="Cmb_Magazzino" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 101; TOP: 8px; " TabIndex="12"
						runat="server" Height="23px" Width="504px" CssClass="testo_08_nero" AutoPostBack="True"></asp:dropdownlist></asp:Panel>
<asp:Panel id="Pannello_Specie" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 107; TOP: 352px"
					runat="server" Height="58px" Width="928px" ms_positioning="gridlayout">&nbsp; 
<ASP:LABEL id="LABEL12" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px" runat="server"
						Height="15px" Width="88px" CssClass="Testo_08_Nero_Bold">Specie:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_Da_CodiceSpecie" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 101; TOP: 8px; " TabIndex="8"
						runat="server" Height="16px" Width="72px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL11" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 102; TOP: 8px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">Da:</ASP:LABEL>
<ASP:LABEL id="lbl_A_Specie" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 103; TOP: 32px"
						runat="server" Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">A:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_A_CodiceSpecie" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 104; TOP: 32px; " TabIndex="10"
						runat="server" Height="16px" Width="72px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></ASP:TEXTBOX>
<asp:Button id="Btn_Da_Specie" style="POSITION: absolute; LEFT: 216px; Z-INDEX: 105; TOP: 8px; " TabIndex="9"
						runat="server" Height="20px" Width="48px" CssClass="Testo_08_nero" Text="Carica"></asp:Button>
<asp:Button id="Btn_A_Specie" style="POSITION: absolute; LEFT: 216px; Z-INDEX: 106; TOP: 32px; " TabIndex="11"
						runat="server" Height="20px" Width="48px" CssClass="Testo_08_nero" Text="Carica"></asp:Button>
<ASP:TEXTBOX id="Txt_Da_Specie" style="POSITION: absolute; LEFT: 272px; Z-INDEX: 107; TOP: 8px"
						runat="server" Height="16px" Width="320px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
<ASP:TEXTBOX id="Txt_A_Specie" style="POSITION: absolute; LEFT: 272px; Z-INDEX: 108; TOP: 32px"
						runat="server" Height="16px" Width="320px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
<ASP:TEXTBOX id="Txt_SringaFiltroSpecie" style="POSITION: absolute; LEFT: 600px; Z-INDEX: 110; TOP: 8px"
						runat="server" Height="42px" Width="320px" BackColor="White" CssClass="Testo_08_Blue"
						TextMode="MultiLine" BorderStyle="None" ReadOnly="True" MaxLength="10"></ASP:TEXTBOX></asp:Panel>
<asp:Panel id="Pannello_Produttore" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 108; TOP: 616px"
					runat="server" Height="58px" Width="928px" ms_positioning="gridlayout">&nbsp; 
<ASP:LABEL id="LABEL10" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px" runat="server"
						Height="15px" Width="88px" CssClass="Testo_08_Nero_Bold">Produttore:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_FiltroPiva_Produttore" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 101; TOP: 8px; " TabIndex="4"
						runat="server" Height="16px" Width="104px" BackColor="PaleTurquoise"
						CssClass="Testo_08_Blue" BorderStyle="None" MaxLength="11"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL9" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 103; TOP: 8px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">P.IVA:</ASP:LABEL>
<asp:Button id="Btn_Carica_Produttore" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 104; TOP: 16px; " TabIndex="5"
						runat="server" Height="20px" Width="48px" CssClass="Testo_08_nero" Text="Carica"></asp:Button>
<ASP:TEXTBOX id="Txt_RagSoc_Produttore" style="POSITION: absolute; LEFT: 336px; Z-INDEX: 105; TOP: 16px"
						runat="server" Height="16px" Width="320px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL2" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 106; TOP: 24px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">Ragione Sociale:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_FiltroRagSoc_Produttore" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 107; TOP: 32px; " TabIndex="4"
						runat="server" Height="16px" Width="104px" BackColor="PaleTurquoise"
						CssClass="Testo_08_Blue" BorderStyle="None"></ASP:TEXTBOX></asp:Panel>
<asp:Panel id="Pannello_Prodotti" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 109; TOP: 408px"
					runat="server" Height="36px" Width="928px" ms_positioning="gridlayout">&nbsp; 
<asp:label id="Label13" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 10px" runat="server"
						Height="14px" Width="80px" CssClass="testo_08_nero_bold">Prodotto:</asp:label>
<asp:dropdownlist id="Cmb_Prodotti" style="POSITION: absolute; LEFT: 352px; Z-INDEX: 101; TOP: 8px; " TabIndex="12"
						runat="server" Height="23px" Width="417px" CssClass="testo_08_nero"></asp:dropdownlist>
<ASP:LABEL id="lbl_filtro_prodotti" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 102; TOP: 11px"
						runat="server" Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">Descrizione:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_Filtro_MatDes" style="POSITION: absolute; LEFT: 200px; Z-INDEX: 103; TOP: 8px; " TabIndex="8"
						runat="server" Height="16px" Width="88px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></ASP:TEXTBOX>
<asp:Button id="Btn_Carica_Prodotti" style="POSITION: absolute; LEFT: 296px; Z-INDEX: 104; TOP: 7px; " TabIndex="5"
						runat="server" Height="20px" Width="48px" CssClass="Testo_08_nero" Text="Carica"></asp:Button></asp:Panel>
<asp:Panel id="Pannello_NumeriBolla" style="POSITION: absolute; LEFT: 485px; Z-INDEX: 110; TOP: 280px"
					runat="server" Height="51" Width="452px" ms_positioning="gridlayout">
					<ASP:LABEL id="LABEL16" style="POSITION: absolute; LEFT: 64px; Z-INDEX: 100; TOP: 9px" runat="server"
						Height="16px" Width="24px" CssClass="Testo_08_Nero_Bold">Da:</ASP:LABEL>
					<ASP:LABEL id="LABEL15" style="POSITION: absolute; LEFT: 264px; Z-INDEX: 101; TOP: 9px" runat="server"
						Height="16px" Width="12px" CssClass="Testo_08_Nero_Bold">A:</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_DaNumeroBolla" style="POSITION: absolute; LEFT: 158px; Z-INDEX: 102; TOP: 8px; " TabIndex="2"
						runat="server" Height="16px" Width="72px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX id="Txt_ANumeroBolla" style="POSITION: absolute; LEFT: 349px; Z-INDEX: 103; TOP: 8px; " TabIndex="3"
						runat="server" Height="16px" Width="72px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></ASP:TEXTBOX>
					<ASP:LABEL id="LABEL14" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 104; TOP: 3px" runat="server"
						Height="24px" Width="52px" CssClass="Testo_08_Nero_Bold">Numeri Bolla:</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_Suffisso_DaNumeroBolla" style="POSITION: absolute; LEFT: 232px; Z-INDEX: 105; TOP: 8px"
						runat="server" Height="16px" Width="28px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
						ReadOnly="True"></ASP:TEXTBOX>
					<ASP:TEXTBOX id="Txt_Suffisso_ANumeroBolla" style="POSITION: absolute; LEFT: 424px; Z-INDEX: 106; TOP: 8px"
						runat="server" Height="16px" Width="23px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
						ReadOnly="True"></ASP:TEXTBOX>
					<asp:dropdownlist id="Cmb_Prefisso_DaNumeroBolla" style="POSITION: absolute; LEFT: 89px; Z-INDEX: 107; TOP: 8px"
						runat="server" Height="23px" Width="64px" CssClass="testo_08_nero"></asp:dropdownlist>
					<asp:dropdownlist id="Cmb_Prefisso_ANumeroBolla" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 108; TOP: 8px"
						runat="server" Height="23px" Width="64px" CssClass="testo_08_nero"></asp:dropdownlist>
				</asp:Panel>
<asp:CheckBox id="Chk_TracciaImpianti" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 111; TOP: 132px"
					runat="server" CssClass="testo_08_nero" Text="Visualizza Tracciabilità Impianti"></asp:CheckBox>
<ASP:TEXTBOX id="Txt_SringaFiltroBolle" style="POSITION: absolute; LEFT: 528px; Z-INDEX: 112; TOP: 40px"
					runat="server" Height="26px" Width="266px" BackColor="White" CssClass="Testo_08_Blue"
					Visible="False" TextMode="MultiLine" BorderStyle="None" ReadOnly="True" MaxLength="10"></ASP:TEXTBOX>
<asp:Panel id="Pannello_OpzioniStampa" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 113; TOP: 176px"
					runat="server" Height="57px" Width="659" ms_positioning="gridlayout">
					<asp:CheckBox id="Chk_Fascicola" style="POSITION: absolute; LEFT: 368px; Z-INDEX: 100; TOP: 6px"
						runat="server" Height="16px" Width="88px" CssClass="testo_08_nero_bold" Visible="False" Text="Fascicola"
						Checked="True"></asp:CheckBox>
					<asp:Label id="Label17" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 101; TOP: 8px" runat="server"
						Height="16px" Width="130px" CssClass="Testo_08_Nero_Bold">Opzioni di stampa:</asp:Label>
					<asp:Label id="Label18" style="POSITION: absolute; LEFT: 176px; Z-INDEX: 102; TOP: 8px" runat="server"
						Height="16px" Width="114px" CssClass="Testo_08_Nero_bold">Numero di copie:</asp:Label>
					<asp:TextBox id="Txt_NumeroCopie" style="POSITION: absolute; LEFT: 296px; Z-INDEX: 104; TOP: 7px"
						runat="server" Height="16px" Width="34px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None">1</asp:TextBox>
					<asp:DropDownList id="Cmb_Stampante" style="POSITION: absolute; LEFT: 176px; Z-INDEX: 105; TOP: 32px"
						runat="server" Height="8px" Width="474px" CssClass="testo_08_nero"></asp:DropDownList>
					<asp:Label id="Label19" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 106; TOP: 32px" runat="server"
						Height="16px" Width="162px" CssClass="Testo_08_Nero_bold">Seleziona la stampante:</asp:Label>
				</asp:Panel>
<ASP:PANEL id="Pannello_CaricoScaricoPomodoro" style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #afeeee; POSITION: absolute; LEFT: 280px; Z-INDEX: 114; TOP: 240px"
					MS_POSITIONING="GridLayout" runat="server" Height="56" Width="660px" BackColor="White"
					BORDERSTYLE="None" BORDERCOLOR="#0000C0" BORDERWIDTH="0px">&nbsp; 
<ASP:LABEL id="LABEL26" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 101; TOP: 8px" runat="server"
						Height="16px" Width="104px" CssClass="Testo_08_Nero_Bold">Data di Stampa:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_DataRegPom" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 102; TOP: 7px; " TabIndex="2"
						runat="server" Height="16px" Width="88px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None" MaxLength="10"></ASP:TEXTBOX><INPUT onclick="BtnImpostaDataPom_Clikkalo" id="BtnImpostaDataPom" class="Testo_08_Nero_Bold"
						style="HEIGHT: 17px; POSITION: absolute; LEFT: 368px; Z-INDEX: 103; TOP: 8px" height="17" type="button" width="25" value="..." name="BtnImpostaDataPom"> 
<asp:Label id="Label27" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 104; TOP: 32px" runat="server"
						Height="16px" Width="52px" CssClass="Testo_08_Nero_bold">Pagina:</asp:Label>
<asp:TextBox id="Txt_PagRegPom" style="POSITION: absolute; LEFT: 224px; Z-INDEX: 105; TOP: 31px"
						runat="server" Height="16px" Width="34px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></asp:TextBox>
<asp:Label id="Label28" style="POSITION: absolute; LEFT: 264px; Z-INDEX: 106; TOP: 32px" runat="server"
						Height="16px" Width="52px" CssClass="Testo_08_Nero_bold">Riga:</asp:Label>
<asp:TextBox id="Txt_RigaRegPom" style="POSITION: absolute; LEFT: 304px; Z-INDEX: 107; TOP: 31px"
						runat="server" Height="16px" Width="34px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None"></asp:TextBox>
<asp:CheckBox id="Chk_SalvaNumPagRigaPom" style="POSITION: absolute; LEFT: 408px; Z-INDEX: 108; TOP: 30px"
						runat="server" Height="16px" Width="240px" CssClass="testo_08_nero_bold"
						Text="Salva Numero di Pagina e di Riga"></asp:CheckBox>
<asp:RadioButtonList id="Rbl_PomodoroContrattato" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 109; TOP: 0px"
						runat="server" Height="42px" Width="152px" CssClass="testo_08_nero" AutoPostBack="True">
						<asp:ListItem Value="0" Selected="True">Contrattato</asp:ListItem>
						<asp:ListItem Value="1">Non Contrattato</asp:ListItem>
					</asp:RadioButtonList>
<asp:Button id="Btn_CalcolaProgressivi" style="POSITION: absolute; LEFT: 400px; Z-INDEX: 110; TOP: 4px"
						runat="server" Height="24px" Width="250px" CssClass="testo_08_nero"
						Text="Ricava Progressivi Riga e Pagina"></asp:Button></ASP:PANEL>
<asp:Panel id="Pannello_NumeriCertificato" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 115; TOP: 120px"
					runat="server" Height="56px" Width="512px" Visible="False" ms_positioning="gridlayout">
					<ASP:LABEL id="LABEL33" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 100; TOP: 9px" runat="server"
						Height="16px" Width="24px" CssClass="Testo_08_Nero_Bold" Visible="False">Da:</ASP:LABEL>
					<ASP:LABEL id="LABEL32" style="POSITION: absolute; LEFT: 328px; Z-INDEX: 101; TOP: 9px" runat="server"
						Height="16px" Width="12px" CssClass="Testo_08_Nero_Bold" Visible="False">A:</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_DaNumeroCert" style="POSITION: absolute; LEFT: 200px; Z-INDEX: 102; TOP: 8px; " TabIndex="2"
						runat="server" Height="16px" Width="72px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						Visible="False" BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX id="Txt_ANumeroCert" style="POSITION: absolute; LEFT: 384px; Z-INDEX: 103; TOP: 8px; " TabIndex="3"
						runat="server" Height="16px" Width="72px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						Visible="False" BorderStyle="None"></ASP:TEXTBOX>
					<ASP:LABEL id="LABEL29" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 105; TOP: 9px" runat="server"
						Height="16px" Width="136px" CssClass="Testo_08_Nero_Bold" Visible="False">Numeri Certificato:</ASP:LABEL>
					<ASP:TEXTBOX id="Txt_Prefisso_DaNumeroCert" style="POSITION: absolute; LEFT: 160px; Z-INDEX: 106; TOP: 8px"
						runat="server" Height="16px" Width="35px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" Visible="False"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
					<ASP:TEXTBOX id="Txt_Prefisso_ANumeroCert" style="POSITION: absolute; LEFT: 344px; Z-INDEX: 107; TOP: 8px"
						runat="server" Height="16px" Width="35px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" Visible="False"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
					<ASP:TEXTBOX id="Txt_Suffisso_DaNumeroCert" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 108; TOP: 8px"
						runat="server" Height="16px" Width="40px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" Visible="False"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
					<ASP:TEXTBOX id="Txt_Suffisso_ANumeroCert" style="POSITION: absolute; LEFT: 464px; Z-INDEX: 109; TOP: 8px"
						runat="server" Height="16px" Width="38px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" Visible="False"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
					<asp:RadioButtonList id="Rbl_CertificatiStampaMassiva" style="POSITION: absolute; LEFT: 136px; Z-INDEX: 110; TOP: 24px"
						runat="server" Height="18px" Width="176px" CssClass="testo_08_nero" AutoPostBack="True" Enabled="False"
						RepeatDirection="Horizontal">
						<asp:ListItem Value="1" Selected="True">Interno</asp:ListItem>
						<asp:ListItem Value="2">Esterno</asp:ListItem>
					</asp:RadioButtonList>
					<ASP:LABEL id="LABEL37" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 111; TOP: 32px" runat="server"
						Height="16px" Width="120px" CssClass="Testo_08_Nero_Bold">Tipo Certificato:</ASP:LABEL>
				</asp:Panel>
<asp:Panel id="Pannello_Giacenza" style="POSITION: absolute; LEFT: 496px; Z-INDEX: 116; TOP: 280px"
					runat="server" Height="32px" Width="252px" ms_positioning="gridlayout">
<ASP:TEXTBOX id="Txt_DataGiacenza" style="POSITION: absolute; LEFT: 120px; Z-INDEX: 102; TOP: 7px; " TabIndex="2"
						runat="server" Height="16px" Width="88px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
						BorderStyle="None" MaxLength="10"></ASP:TEXTBOX><INPUT onclick="BtnImpostaDataGiacenza_Clikkalo" id="BtnImpostaDataGiacenza" class="Testo_08_Nero_Bold"
						style="HEIGHT: 17px; POSITION: absolute; LEFT: 216px; Z-INDEX: 103; TOP: 8px" height="17" size="16" type="button" width="25" value="..."
						name="BtnImpostaDataGiacenza">&nbsp; 
<ASP:LABEL id="LABEL20" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 106; TOP: 8px" runat="server"
						Height="16px" Width="112px" CssClass="Testo_08_Nero_Bold">Data Giacenza:</ASP:LABEL></asp:Panel>
<asp:Panel id="Pannello_Coop1" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 117; TOP: 504px"
					runat="server" Height="58px" Width="928px" ms_positioning="gridlayout">&nbsp; 
<ASP:LABEL id="LABEL34" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px" runat="server"
						Height="15px" Width="88px" CssClass="Testo_08_Nero_Bold">Cooperativa:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_FiltroPiva_Coop1" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 101; TOP: 8px; " TabIndex="4"
						runat="server" Height="16px" Width="104px" BackColor="PaleTurquoise"
						CssClass="Testo_08_Blue" BorderStyle="None" MaxLength="11"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL24" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 103; TOP: 8px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">P.IVA:</ASP:LABEL>
<asp:Button id="Btn_Carica_Coop1" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 104; TOP: 16px; " TabIndex="5"
						runat="server" Height="20px" Width="48px" CssClass="Testo_08_nero" Text="Carica"></asp:Button>
<ASP:TEXTBOX id="Txt_RagSoc_Coop1" style="POSITION: absolute; LEFT: 336px; Z-INDEX: 105; TOP: 16px"
						runat="server" Height="16px" Width="320px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL21" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 106; TOP: 24px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">Ragione Sociale:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_FiltroRagSoc_Coop1" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 107; TOP: 32px; " TabIndex="4"
						runat="server" Height="16px" Width="104px" BackColor="PaleTurquoise"
						CssClass="Testo_08_Blue" BorderStyle="None"></ASP:TEXTBOX></asp:Panel>
<asp:Panel id="Pannello_Coop2" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 118; TOP: 560px"
					runat="server" Height="58px" Width="928px" ms_positioning="gridlayout">&nbsp; 
<ASP:LABEL id="LABEL38" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px" runat="server"
						Height="15px" Width="88px" CssClass="Testo_08_Nero_Bold">Seconda Cooperativa:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_FiltroPiva_Coop2" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 101; TOP: 8px; " TabIndex="4"
						runat="server" Height="16px" Width="104px" BackColor="PaleTurquoise"
						CssClass="Testo_08_Blue" BorderStyle="None" MaxLength="11"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL36" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 103; TOP: 8px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">P.IVA:</ASP:LABEL>
<asp:Button id="Btn_Carica_Coop2" style="POSITION: absolute; LEFT: 280px; Z-INDEX: 104; TOP: 16px; " TabIndex="5"
						runat="server" Height="20px" Width="48px" CssClass="Testo_08_nero" Text="Carica"></asp:Button>
<ASP:TEXTBOX id="Txt_RagSoc_Coop2" style="POSITION: absolute; LEFT: 336px; Z-INDEX: 105; TOP: 16px"
						runat="server" Height="16px" Width="320px" BackColor="#C0FFC0" CssClass="Testo_08_Blue"
						BorderStyle="None" ReadOnly="True"></ASP:TEXTBOX>
<ASP:LABEL id="LABEL35" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 106; TOP: 24px" runat="server"
						Height="8px" Width="24px" CssClass="Testo_08_Nero_Bold">Ragione Sociale:</ASP:LABEL>
<ASP:TEXTBOX id="Txt_FiltroRagSoc_Coop2" style="POSITION: absolute; LEFT: 168px; Z-INDEX: 107; TOP: 32px; " TabIndex="4"
						runat="server" Height="16px" Width="104px" BackColor="PaleTurquoise"
						CssClass="Testo_08_Blue" BorderStyle="None"></ASP:TEXTBOX></asp:Panel>
<asp:Panel id="Pannello_Regolamento" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 119; TOP: 72px"
					runat="server" Height="36px" Width="928px" ms_positioning="gridlayout">&nbsp; 
<asp:label id="Label39" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px" runat="server"
						Height="14px" Width="80px" CssClass="testo_08_nero_bold">Regolamento:</asp:label>
<asp:dropdownlist id="Cmb_Regolamento" style="POSITION: absolute; LEFT: 112px; Z-INDEX: 101; TOP: 8px; " TabIndex="12"
						runat="server" Height="23px" Width="504px" CssClass="testo_08_nero" AutoPostBack="True">
						<asp:ListItem Value="0">Tutti</asp:ListItem>
						<asp:ListItem Value="1">Nessun regolamento</asp:ListItem>
						<asp:ListItem Value="10">Produzione Integrata</asp:ListItem>
						<asp:ListItem Value="4">Biologico</asp:ListItem>
					</asp:dropdownlist></asp:Panel></asp:panel><ASP:IMAGEBUTTON id="ImgBtnEsci" style="POSITION: absolute; LEFT: 920px; Z-INDEX: 102; TOP: 12px"
				runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/icone32/esci.bmp"></ASP:IMAGEBUTTON><asp:panel id="Panel1" style="POSITION: absolute; LEFT: 1072px; Z-INDEX: 108; TOP: 8px" runat="server"
				Width="248px" Height="48px" Visible="False">Panel 
<ASP:TEXTBOX id="Txt_Prefisso_DaNumeroBolla_D" runat="server" Height="16px" Width="35px" BackColor="PaleTurquoise"
					CssClass="Testo_08_Blue" BorderStyle="None"></ASP:TEXTBOX>
<ASP:TEXTBOX id="Txt_Prefisso_ANumeroBolla_D" runat="server" Height="16px" Width="35px" BackColor="PaleTurquoise"
					CssClass="Testo_08_Blue" BorderStyle="None"></ASP:TEXTBOX></asp:panel></form>
	</body>
</HTML>
