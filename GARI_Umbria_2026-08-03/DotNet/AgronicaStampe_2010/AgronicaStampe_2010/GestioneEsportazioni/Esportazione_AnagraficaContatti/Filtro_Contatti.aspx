<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Filtro_Contatti.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_Contatti" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    		<title>Filtro Contatti</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK rel="stylesheet" type="text/css" href="../../App_Scripts/CSS/AgronicaStyle.css">
</head>
<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table aria-hidden="true" style="HEIGHT: 41px; WIDTH: 904px; POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px"
				id="TableTitolo" border="0" cellSpacing="1" cellPadding="1" width="904">
				<TR>
					<TD style="WIDTH: 41px"><ASP:IMAGE id="ImgIcona" runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/icone32/Stampa.ico"></ASP:IMAGE></TD>
					<TD bgColor="#042649" vAlign="middle" align="left">&nbsp;
						<ASP:LABEL id="LblTitolo" runat="server" Width="688px" Height="7px" CssClass="Testo_12_Bianco_Bold"
							BackColor="#042649"> Filtro Esportazione Contatti</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<asp:panel style="POSITION: absolute; LEFT: 8px; Z-INDEX: 106; TOP: 624px" id="Pannello_Filtri_AnagraficaContatti"
				MS_POSITIONING="GridLayout" runat="server" Width="798px" Height="152px" BackColor="White"
				BorderStyle="Solid" BorderWidth="2px" BorderColor="#0000C0">&nbsp; 
<ASP:LABEL id="LABEL7" style="POSITION: absolute; LEFT: 320px; Z-INDEX: 101; TOP: 8px" runat="server"
					Height="15px" Width="144px" CssClass="Testo_08_Nero_Bold">Tipologia Contatto :</ASP:LABEL>
<ASP:LABEL id="LABEL5" style="POSITION: absolute; LEFT: 520px; Z-INDEX: 102; TOP: 8px" runat="server"
					Height="15px" Width="240px" CssClass="Testo_08_Nero_Bold">Visibilità Contatto :</ASP:LABEL>
<asp:RadioButtonList id="Rbl_Visibilita_Export_Contatti" style="POSITION: absolute; LEFT: 520px; Z-INDEX: 103; TOP: 24px"
					runat="server" Height="32px" Width="260px" CssClass="testo_08_nero">
					<asp:ListItem Value="1" Selected="True">Solo i Contatti delle Imprese Selezionate</asp:ListItem>
					<asp:ListItem Value="2">Contatti dell'Imprese Selezionate e Contatti Pubblici</asp:ListItem>
				</asp:RadioButtonList>
<ASP:LABEL id="LABEL3" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 104; TOP: 8px" runat="server"
					Height="15px" Width="128px" CssClass="Testo_08_Nero_Bold">Rapporto Contabile:</ASP:LABEL>
<asp:radiobuttonlist id="Rbl_Tipologia_contatti" style="POSITION: absolute; LEFT: 320px; Z-INDEX: 108; TOP: 32px"
					runat="server" Height="44px" Width="184px" CssClass="Testo_08_Nero" BorderColor="White"
					BorderStyle="None" CellPadding="0" CellSpacing="0" AutoPostBack="false">
					<asp:ListItem Value="0">Persone Fisiche</asp:ListItem>
					<asp:ListItem Value="1">Persone Giuridiche</asp:ListItem>
					<asp:ListItem Value="2" Selected="True">Persone Fisiche e Giuridiche</asp:ListItem>
				</asp:radiobuttonlist>
<asp:panel id="Panel1" style="OVERFLOW: auto; POSITION: absolute; LEFT: 0px; Z-INDEX: 109; TOP: 24px"
					MS_POSITIONING="GridLayout" runat="server" Height="123px" Width="304px"
					BackColor="White" BorderColor="White" BorderWidth="0px">&nbsp; 
<asp:CheckBoxList id="Cbl_Rapporto_Contabile" style="POSITION: absolute; LEFT: 0px; Z-INDEX: 107; TOP: 0px"
						runat="server" Height="18px" Width="256px" CssClass="Testo_08_Nero"></asp:CheckBoxList></asp:panel></asp:panel><asp:label style="POSITION: absolute; LEFT: 1096px; Z-INDEX: 107; TOP: 752px" id="Label6" runat="server"
				Width="64px" Height="48px" Visible="False">Label</asp:label><ASP:PANEL style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #afeeee; POSITION: absolute; LEFT: 8px; Z-INDEX: 105; TOP: 216px"
				id="Pannello_Check" MS_POSITIONING="GridLayout" runat="server" Width="798px" Height="393px" BackColor="White" BORDERWIDTH="2px" BORDERCOLOR="#0000C0"
				BORDERSTYLE="Solid">&nbsp; 
<ASP:LABEL id="LblSeleziona" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 100; TOP: 16px"
					runat="server" Height="15px" Width="352px" BackColor="White" CssClass="Testo_08_Nero_Bold">Seleziona i campi che si desidera estarre :</ASP:LABEL>
<asp:CheckBoxList id="ChkGruppo1" style="POSITION: absolute; LEFT: 16px; Z-INDEX: 101; TOP: 40px"
					runat="server" Height="330px" Width="232px" BackColor="White" CssClass="Testo_08_Nero"
					AutoPostBack="True">
					<asp:ListItem Value="0">Partita IVA dell'Impresa che ha creato il contatto</asp:ListItem>
					<asp:ListItem Value="1">Ragione Sociale dell'Impresa che ha creato il contatto</asp:ListItem>
					<asp:ListItem Value="2">Partita IVA / Codice Fiscale</asp:ListItem>
					<asp:ListItem Value="3">Ragione Sociale / Nome e Cognome</asp:ListItem>
					<asp:ListItem Value="4">Visibilit&#224;</asp:ListItem>
					<asp:ListItem Value="5">Tipologia</asp:ListItem>
					<asp:ListItem Value="6">Convenevoli</asp:ListItem>
					<asp:ListItem Value="7">Tipo Indirizzo di Default per documenti contabili</asp:ListItem>
					<asp:ListItem Value="8">Sconto di Default per Documenti Contabili Emessi</asp:ListItem>
					<asp:ListItem Value="9">Codice GIAS</asp:ListItem>
					<asp:ListItem Value="10">Flag Spesometro</asp:ListItem>
				</asp:CheckBoxList>
<asp:CheckBoxList id="ChkGruppo2" style="POSITION: absolute; LEFT: 256px; Z-INDEX: 102; TOP: 37px"
					runat="server" Height="334px" Width="232px" BackColor="White" CssClass="Testo_08_Nero"
					AutoPostBack="True">
					<asp:ListItem Value="10">Progressivo</asp:ListItem>
					<asp:ListItem Value="11">Attivit&#224;</asp:ListItem>
					<asp:ListItem Value="12">Validita Inizio e Fine</asp:ListItem>
					<asp:ListItem Value="13">Rapporto Contabile</asp:ListItem>
                    <asp:ListItem Value="14">Dati Patentino</asp:ListItem>
					<asp:ListItem Value="18">Indirizzo, Frazione, CAP, Comune, Provincia, Stato</asp:ListItem>
					<asp:ListItem Value="19">Telefono</asp:ListItem>
				</asp:CheckBoxList>
                <!-- 
                SE SI DISATTIVANO CHECK, sistemare: Controllo_Selezione_Check, Tutti_Check_Deseleziona, Tutti_Check_Seleziona                								
					<asp:ListItem Value="17">Ore Settimanali, Giorni Malattia, Giorni Ferie, Giorni Goduti</asp:ListItem>
                <asp:ListItem Value="16">Corrispettivo</asp:ListItem>-->
<asp:CheckBoxList id="ChkGruppo3" style="POSITION: absolute; LEFT: 489px; Z-INDEX: 103; TOP: 32px"
					runat="server" Height="330px" Width="264px" BackColor="White" CssClass="Testo_08_Nero"
					AutoPostBack="True">
					<asp:ListItem Value="20">Fax</asp:ListItem>
					<asp:ListItem Value="21">Email</asp:ListItem>
					<asp:ListItem Value="22">Cellulare</asp:ListItem>
					<asp:ListItem Value="23">Persona Referente</asp:ListItem>
                    <asp:ListItem Value="27">Tipologia Prodotto Acquistato / Venduto</asp:ListItem>
					<asp:ListItem Value="30">Listino Vendita associato</asp:ListItem>
					<asp:ListItem Value="31">PEC</asp:ListItem>
					<asp:ListItem Value="32">Codice SDI</asp:ListItem>
				</asp:CheckBoxList>
                <!-- 
                					<asp:ListItem Value="24">Istituto di Credito</asp:ListItem>
					<asp:ListItem Value="25">Coordinate IBAN</asp:ListItem>
					<asp:ListItem Value="26">Data Apertura e Estinzione Conto</asp:ListItem>					
					<asp:ListItem Value="28">Conti Economici Direttamente Imputabili</asp:ListItem>
					<asp:ListItem Value="29">Codice GIAS pre importazione e Impresa da cui &#232; stato importato il Contatto</asp:ListItem>
                -->
<asp:ImageButton id="ImgBtnSelezionaTutto" style="POSITION: absolute; LEFT: 480px; Z-INDEX: 104; TOP: 8px"
					runat="server" ImageUrl="../../AB_Immagini/Icone16/cS.ico" Height="16px" Width="16px"
					BackColor="White"></asp:ImageButton>
<asp:Label id="LblSelezionaTutto" style="POSITION: absolute; LEFT: 504px; Z-INDEX: 105; TOP: 8px"
					runat="server" Height="16px" Width="88px" BackColor="White" CssClass="Testo_08_Nero">Seleziona tutto</asp:Label>
<asp:ImageButton id="ImgBtnDeselezionaTutto" style="POSITION: absolute; LEFT: 616px; Z-INDEX: 107; TOP: 8px"
					runat="server" ImageUrl="../../AB_Immagini/Icone16/cN.ico" Height="16px" Width="16px"
					BackColor="White"></asp:ImageButton>
<asp:Label id="LblDeselezionaTutto" style="POSITION: absolute; LEFT: 640px; Z-INDEX: 108; TOP: 8px"
					runat="server" Height="16px" Width="112px" BackColor="White" CssClass="Testo_08_Nero">Deseleziona tutto</asp:Label><INPUT onclick="BtnValiditaInizio_Clikkalo" tabIndex="7" id="BtnValiditaInizio" class="Testo_08_Nero_Bold"
					style="HEIGHT: 20px; WIDTH: 20px; POSITION: absolute; LEFT: -1000px; Z-INDEX: 109; TOP: -1000px" height="17" size="16" type="button" width="25" value="..." name="BtnValiditaInizio" runat="server"> 
            </ASP:PANEL><ASP:PANEL style="OVERFLOW: auto; SCROLLBAR-FACE-COLOR: #afeeee; POSITION: absolute; LEFT: 808px; Z-INDEX: 103; TOP: 56px"
				id="Pannello_Stampa" MS_POSITIONING="GridLayout" runat="server" Width="152px" Height="152px"
				BackColor="White" BORDERWIDTH="0px" BORDERCOLOR="#0000C0" BORDERSTYLE="None">&nbsp; 
<asp:ImageButton id="ImgBtn_Stampa" style="POSITION: absolute; LEFT: 24px; Z-INDEX: 101; TOP: 104px"
					runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico"></asp:ImageButton>
<asp:label id="Lbl_Stampa" style="POSITION: absolute; LEFT: 64px; Z-INDEX: 102; TOP: 112px"
					runat="server" Height="14px" Width="56px" CssClass="Testo_08_Blue_Bold">Stampa</asp:label>
<asp:RadioButtonList id="Rbl_Registri" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 107; TOP: 8px"
					runat="server" Height="32px" Width="136px" CssClass="Testo_08_Nero">
					<asp:ListItem Value="-2" Selected="True">Registro Clienti</asp:ListItem>
					<asp:ListItem Value="-3">Registro Fornitori</asp:ListItem>
					<asp:ListItem Value="0">Esportazione Excel</asp:ListItem>
				</asp:RadioButtonList></ASP:PANEL><asp:panel style="POSITION: absolute; LEFT: 8px; Z-INDEX: 102; TOP: 56px" id="Pannello_Filtri_PacchettoIgiene"
				MS_POSITIONING="GridLayout" runat="server" Width="798px" Height="152px" BackColor="White" BorderStyle="Solid" BorderWidth="2px"
				BorderColor="#0000C0">&nbsp; 
<ASP:LABEL id="LABEL9" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 100; TOP: 8px" runat="server"
					Height="15px" Width="72px" CssClass="Testo_08_Nero_Bold">Impresa :</ASP:LABEL>
<ASP:DROPDOWNLIST id="Cmb_Impresa" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 102; TOP: 24px"
					runat="server" Height="18px" Width="312px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
<ASP:LABEL id="LABEL13" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 103; TOP: 56px" runat="server"
					Height="15px" Width="120px" CssClass="Testo_08_Nero_Bold">Ricerca Impresa :</ASP:LABEL>
<ASP:TEXTBOX id="Txt_Impresa" style="POSITION: absolute; LEFT: 128px; Z-INDEX: 104; TOP: 56px"
					runat="server" Height="16px" Width="110px" BackColor="White" CssClass="Testo_08_Blue"
					BorderStyle="None" MaxLength="10" ToolTip="Data in cui verranno stampati solo gli impianti attivi in quella data"></ASP:TEXTBOX>
<ASP:IMAGEBUTTON id="ImgBtn_CaricaImprese" style="POSITION: absolute; LEFT: 248px; Z-INDEX: 105; TOP: 48px"
					runat="server" ImageUrl="../../AB_Immagini/icone32/lente.ico" Height="32px" Width="32px"></ASP:IMAGEBUTTON>
<ASP:LABEL id="LABEL1" style="POSITION: absolute; LEFT: 328px; Z-INDEX: 107; TOP: 8px" runat="server"
					Height="15px" Width="144px" CssClass="Testo_08_Nero_Bold">Tipologia Contatto :</ASP:LABEL>
<ASP:LABEL id="LABEL2" style="POSITION: absolute; LEFT: 520px; Z-INDEX: 108; TOP: 8px" runat="server"
					Height="15px" Width="240px" CssClass="Testo_08_Nero_Bold">Visibilità Contatto :</ASP:LABEL>
<asp:RadioButtonList id="Rbl_Visibilita" style="POSITION: absolute; LEFT: 520px; Z-INDEX: 109; TOP: 24px"
					runat="server" Height="32px" Width="256px" CssClass="Testo_08_Nero">
					<asp:ListItem Value="1">Solo i Contatti dell'Impresa selezionata</asp:ListItem>
					<asp:ListItem Value="2">Contatti dell'Impresa e Contatti Pubblici</asp:ListItem>
					<asp:ListItem Value="3" Selected="True">Tutti i Contatti</asp:ListItem>
				</asp:RadioButtonList>
<ASP:LABEL id="LABEL4" style="POSITION: absolute; LEFT: 8px; Z-INDEX: 110; TOP: 104px" runat="server"
					Height="15px" Width="64px" CssClass="Testo_08_Nero_Bold">Rapporto Contabile:</ASP:LABEL>
<ASP:DROPDOWNLIST id="Cmb_RapportiContabili" style="POSITION: absolute; LEFT: 80px; Z-INDEX: 111; TOP: 104px"
					runat="server" Height="18px" Width="240px" CssClass="Testo_08_Blue"></ASP:DROPDOWNLIST>
<asp:radiobuttonlist id="Rbl_Tipologia" style="POSITION: absolute; LEFT: 328px; Z-INDEX: 113; TOP: 32px"
					runat="server" Height="44px" Width="184px" CssClass="Testo_08_Nero" BorderColor="White"
					BorderStyle="None" CellPadding="0" CellSpacing="0" AutoPostBack="false">
					<asp:ListItem Value="0">Persone Fisiche</asp:ListItem>
					<asp:ListItem Value="1">Persone Giuridiche</asp:ListItem>
					<asp:ListItem Value="2" Selected="True">Persone Fisiche e Giuridiche</asp:ListItem>
				</asp:radiobuttonlist></asp:panel><ASP:IMAGEBUTTON style="POSITION: absolute; LEFT: 920px; Z-INDEX: 101; TOP: 16px" id="ImgBtn_Esci"
				runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/icone32/esci.bmp"></ASP:IMAGEBUTTON></form>
	</body>
</html>
