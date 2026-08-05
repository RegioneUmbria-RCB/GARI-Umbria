<%@ Page Language="vb" AutoEventWireup="false" Codebehind="GestioneNumerazioneCertificati.aspx.vb" Inherits="AgronicaStampe_2010.GestioneNumerazioneCertificati"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
	<HEAD>
		<title>GestioneNumerazioneCertificati</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../../App_Script/CSS/AgronicaStyle.css" type="text/css" rel="stylesheet">
		<!-- /////////////////////////////////////////////// --><BASE target="_self">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table aria-hidden="true" id="TableTitolo" style="Z-INDEX: 102; LEFT: 8px; WIDTH: 704px; POSITION: absolute; TOP: 8px; HEIGHT: 36px"
				cellSpacing="1" cellPadding="1" width="704" border="0">
				<TR>
					<TD style="WIDTH: 41px"><ASP:IMAGE id="ImgIcona" runat="server" Width="24px" Height="24px" ImageUrl="../../AB_Immagini/Icone24/Ope_Contabili_24.ico"></ASP:IMAGE></TD>
					<TD vAlign="middle" align="left" bgColor="#00bfff">&nbsp;
						<ASP:LABEL id="LblTitolo" runat="server" Width="404px" Height="7px" CssClass="Testo_12_Nero_Bold"
							BackColor="DeepSkyBlue" Font-Italic="True"> Gestione Numerazione Certificati</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<asp:label id="Label1" style="Z-INDEX: 107; LEFT: 880px; POSITION: absolute; TOP: 568px" runat="server"
				Width="72px" Height="88px" Visible="False"></asp:label><asp:panel id="Pannello_NumerazioneCertificati" style="Z-INDEX: 106; LEFT: 8px; POSITION: absolute; TOP: 48px"
				MS_POSITIONING="GridLayout" runat="server" Width="744px" Height="480px" BackColor="White" BorderColor="#0000C0" BorderWidth="2px" BorderStyle="Solid">
				<asp:TextBox id="Txt_DaInviare" style="Z-INDEX: 100; LEFT: 32px; POSITION: absolute; TOP: 128px"
					runat="server" Height="1px" Width="1px"></asp:TextBox>
				<asp:Label id="Label35" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 24px" runat="server"
					Height="16px" Width="208px" CssClass="Testo_08_Nero_bold">Ultimo certificato valorizzato:</asp:Label>
				<asp:TextBox id="Txt_UltimoNumCertificatoValorizzato" style="Z-INDEX: 102; LEFT: 224px; POSITION: absolute; TOP: 24px"
					runat="server" Height="16px" Width="64px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
					ReadOnly="True"></asp:TextBox>
				<asp:Label id="Label34" style="Z-INDEX: 103; LEFT: 312px; POSITION: absolute; TOP: 24px" runat="server"
					Height="16px" Width="176px" CssClass="Testo_08_Nero_bold">Riferito alla bolla numero:</asp:Label>
				<asp:TextBox id="Txt_UltimoNumBollaValorizzato" style="Z-INDEX: 105; LEFT: 552px; POSITION: absolute; TOP: 24px"
					runat="server" Height="16px" Width="104px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
					ReadOnly="True"></asp:TextBox>
				<asp:Button id="Btn_ImpostaNumerazione" style="Z-INDEX: 106; LEFT: 168px; POSITION: absolute; TOP: 120px"
					runat="server" Height="32px" Width="136px" Text="AVVIA"></asp:Button>
				<asp:Label id="Label2" style="Z-INDEX: 107; LEFT: 8px; POSITION: absolute; TOP: 80px" runat="server"
					Height="16px" Width="480px" CssClass="Testo_08_Nero_bold">Verrà valorizzata la numerazione dei certificati fino alla bolla numero:</asp:Label>
				<asp:TextBox id="Txt_UltimoNumBollaDaValorizzare" style="Z-INDEX: 108; LEFT: 552px; POSITION: absolute; TOP: 80px"
					runat="server" Height="16px" Width="104px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
					ReadOnly="True"></asp:TextBox>
				<asp:Label id="Label3" style="Z-INDEX: 109; LEFT: 320px; POSITION: absolute; TOP: 128px" runat="server"
					Height="16px" Width="328px" CssClass="Testo_08_rosso_bold">Avvia la valorizzazione dei numeri di certificato</asp:Label>
				<asp:TextBox id="Txt_UltimoNumBollaValorizzato_Suffisso" style="Z-INDEX: 110; LEFT: 664px; POSITION: absolute; TOP: 24px"
					runat="server" Height="16px" Width="40px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
					ReadOnly="True"></asp:TextBox>
				<asp:TextBox id="Txt_UltimoNumBollaDaValorizzare_Suffisso" style="Z-INDEX: 111; LEFT: 664px; POSITION: absolute; TOP: 80px"
					runat="server" Height="16px" Width="40px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
					ReadOnly="True"></asp:TextBox>
				<asp:TextBox id="Txt_UltimoNumBollaDaValorizzare_Prefisso" style="Z-INDEX: 112; LEFT: 504px; POSITION: absolute; TOP: 80px"
					runat="server" Height="16px" Width="40px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
					ReadOnly="True"></asp:TextBox>
				<asp:TextBox id="Txt_UltimoNumBollaValorizzato_Prefisso" style="Z-INDEX: 113; LEFT: 504px; POSITION: absolute; TOP: 24px"
					runat="server" Height="16px" Width="40px" BackColor="#C0FFC0" CssClass="Testo_08_Blue" BorderStyle="None"
					ReadOnly="True"></asp:TextBox>
				<asp:Panel id="Pannello_Magazzini" style="Z-INDEX: 115; LEFT: 8px; POSITION: absolute; TOP: 176px"
					runat="server" Height="280px" Width="720px" ms_positioning="gridlayout">&nbsp; 
<asp:TextBox id="Txt_Messaggio" style="Z-INDEX: 115; LEFT: 8px; POSITION: absolute; TOP: 8px"
						runat="server" Height="264px" Width="704px" CssClass="Testo_08_Blue"
						BorderStyle="None" ReadOnly="True" TextMode="MultiLine"></asp:TextBox></asp:Panel>
			</asp:panel><ASP:IMAGEBUTTON id="ImgBtnEsci" style="Z-INDEX: 105; LEFT: 720px; POSITION: absolute; TOP: 8px"
				runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/icone32/esci.bmp"></ASP:IMAGEBUTTON></form>
	</body>
</HTML>
