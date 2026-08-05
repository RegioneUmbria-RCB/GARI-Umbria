<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LiquidazioneIVA_Anteprima_3.aspx.vb" Inherits="AgronicaStampe_2010.LiquidazioneIVA_Anteprima_3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en">
	<HEAD>
		<title>Liquidazione Periodica IVA</title>
		<meta name="vs_showGrid" content="True">
		<meta name="vs_snapToGrid" content="True">
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK rel="stylesheet" type="text/css" href="../../../App_Styles/AgronicaStyle.css">
		<LINK rel="stylesheet" type="text/css" href="../../../App_Styles/jquery-ui-1.10.0.custom.min.css">
		<script type="text/javascript" src="../../../App_Scripts/jquery-1.9.0.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<script type="text/javascript" src="../../../App_Scripts/jquery-ui-1.10.0.custom.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<script type="text/javascript" src="../../../App_Scripts/jquery.ui.datepicker-it.js?<% =Application("GiasVersioneCorrente")%>"></script>
		<style>#ui-datepicker-div { Z-INDEX: 10000 }
		</style>
		<script type="text/javascript">
		    $(document).ready(function () {
		        $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
		    });
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table aria-hidden="true" style="Z-INDEX: 101; POSITION: absolute; WIDTH: 828px; HEIGHT: 40px; TOP: 8px; LEFT: 128px"
				id="TableTitolo" border="0" cellSpacing="1" cellPadding="1" width="828">
				<TR>
					<TD style="WIDTH: 41px"><ASP:IMAGE id="ImgIcona" runat="server" Height="24px" Width="24px" ImageUrl="../../../AB_Immagini/Icone32/Doc3.ico"></ASP:IMAGE></TD>
					<TD bgColor="#5c9ccc" vAlign="middle" align="left">&nbsp;
						<ASP:LABEL id="LblTitolo" runat="server" Height="7px" Width="712px" BackColor="#5C9CCC" CssClass="Testo_12_Nero_Bold">Liquidazione Periodica IVA</ASP:LABEL></TD>
				</TR>
			</TABLE>
			<ASP:PANEL style="Z-INDEX: 108; POSITION: absolute; SCROLLBAR-FACE-COLOR: #e6f4ff; OVERFLOW: auto; TOP: 608px; LEFT: 8px"
				id="Pannello_Conti" MS_POSITIONING="GridLayout" runat="server" Width="504px" Height="70px"
				BackColor="#E6F4FF" Visible="False" BORDERSTYLE="None" BORDERCOLOR="#0000C0" BORDERWIDTH="1px">&nbsp; 
<asp:label style="Z-INDEX: 106; POSITION: absolute; TOP: 10px; LEFT: 8px" id="lbl_AnnoContabile"
					runat="server" Width="104px" Height="14px" CssClass="Testo_08_Nero"
					BackColor="#E6F4FF">Anno Contabile:</asp:label>
<asp:dropdownlist style="Z-INDEX: 108; POSITION: absolute; TOP: 8px; LEFT: 120px" id="Cmb_AnnoContabile"
					runat="server" Width="80px" Height="23px" CssClass="Testo_08_Nero" AutoPostBack="True"></asp:dropdownlist>
<asp:label style="Z-INDEX: 110; POSITION: absolute; TOP: 10px; LEFT: 208px" id="Label24" runat="server"
					Width="120px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Riclassificazione :</asp:label>
<asp:dropdownlist style="Z-INDEX: 111; POSITION: absolute; TOP: 8px; LEFT: 336px" id="Cmb_Riclassificazione"
					runat="server" Width="162px" Height="23px" CssClass="Testo_08_Nero" AutoPostBack="True"></asp:dropdownlist>
<asp:dropdownlist style="Z-INDEX: 112; POSITION: absolute; TOP: 40px; LEFT: 80px" id="Cmb_Conti" runat="server"
					Width="418px" Height="23px" CssClass="Testo_08_Nero"></asp:dropdownlist>
<asp:label style="Z-INDEX: 113; POSITION: absolute; TOP: 42px; LEFT: 8px" id="Label25" runat="server"
					Width="54px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Conto :</asp:label></ASP:PANEL>
			<asp:radiobuttonlist style="Z-INDEX: 107; POSITION: absolute; TOP: 8px; LEFT: 1016px" id="Rbl_Temp" runat="server"
				Height="32px" Width="136px" BackColor="#E6F4FF" CssClass="Testo_08_Nero" Visible="False" AutoPostBack="True">
				<asp:ListItem Value="0" Selected="True">Anno contabile</asp:ListItem>
				<asp:ListItem Value="1">Periodo temporale</asp:ListItem>
			</asp:radiobuttonlist><ASP:IMAGEBUTTON style="Z-INDEX: 106; POSITION: absolute; TOP: 16px; LEFT: 960px" id="ImgBtnAnnulla"
				runat="server" Height="32px" Width="32px" ImageUrl="../../../AB_Immagini/Icone32/Esci.bmp"></ASP:IMAGEBUTTON><asp:label style="Z-INDEX: 105; POSITION: absolute; TOP: 608px; LEFT: 1192px" id="Label7" runat="server"
				Height="40px" Width="112px" Visible="False"></asp:label>
			<table aria-hidden="true" style="Z-INDEX: 104; POSITION: absolute; WIDTH: 870px; HEIGHT: 29px; TOP: 52px; LEFT: 128px"
				id="Table1" border="0" cellSpacing="1" cellPadding="1" width="870">
				<TR>
					<TD class="Testo_08_Nero" bgColor="#5c9ccc">&nbsp; Impresa :
						<ASP:TEXTBOX id="Txt_RagioneSociale" runat="server" Height="17px" Width="712px" BackColor="#5C9CCC"
							CssClass="Testo_08_Nero" BorderStyle="None" MAXLENGTH="120" ReadOnly="True"></ASP:TEXTBOX>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</TD>
				</TR>
			</TABLE>
			<ASP:IMAGE style="Z-INDEX: 103; POSITION: absolute; TOP: 8px; LEFT: 8px" id="ImageLogo" runat="server"
				Height="73px" Width="96px" ImageUrl="../../../AB_Immagini/logo/Logo_GiasOnline_Mini.jpg"></ASP:IMAGE><ASP:PANEL style="Z-INDEX: 102; POSITION: absolute; TOP: 88px; LEFT: 8px" id="Pannello_Generale"
				MS_POSITIONING="GridLayout" runat="server" Height="528px" Width="994px" BackColor="#E6F4FF" BORDERWIDTH="1px" BORDERCOLOR="#0000C0" BORDERSTYLE="Solid">&nbsp; 
<asp:Panel style="Z-INDEX: 101; POSITION: absolute; SCROLLBAR-FACE-COLOR: #e6f4ff; OVERFLOW: auto; TOP: 104px; LEFT: 8px"
					id="Pannello_Credito" MS_POSITIONING="GridLayout" runat="server" Width="402"
					Height="416px" BorderStyle="Solid" BorderWidth="1px" BorderColor="#0000C0">
					<asp:label style="Z-INDEX: 100; POSITION: absolute; TOP: 8px; LEFT: 112px" id="Label5" runat="server"
						Width="168px" Height="14px" CssClass="Testo_08_Nero" BackColor="#C0FFC0">IVA A CREDITO (Acquisti)</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 101; POSITION: absolute; TEXT-ALIGN: left; TOP: 216px; LEFT: 336px"
						id="Txt_NumMovCredito" runat="server" Width="42px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<asp:label style="Z-INDEX: 102; POSITION: absolute; TOP: 216px; LEFT: 8px" id="Label11" runat="server"
						Width="264px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Numero movimenti con IVA a Credito :</asp:label>
					<asp:label style="Z-INDEX: 103; POSITION: absolute; TOP: 242px; LEFT: 16px" id="Label12" runat="server"
						Width="128px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Fatture Ricevute :</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 104; POSITION: absolute; TEXT-ALIGN: right; TOP: 240px; LEFT: 336px"
						id="Txt_NumFattureRicevute" runat="server" Width="42px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<asp:label style="Z-INDEX: 105; POSITION: absolute; TOP: 314px; LEFT: 16px" id="Label13" runat="server"
						Width="144px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Altri Costi :</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 106; POSITION: absolute; TEXT-ALIGN: right; TOP: 312px; LEFT: 336px"
						id="Txt_NumAltriCosti" runat="server" Width="42px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<asp:label style="Z-INDEX: 107; POSITION: absolute; TOP: 290px; LEFT: 16px" id="Label14" runat="server"
						Width="152px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Corrispettivi di Acquisto :</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 108; POSITION: absolute; TEXT-ALIGN: right; TOP: 288px; LEFT: 336px"
						id="Txt_NumAcquisti" runat="server" Width="42px" Height="16px" CssClass="Testo_08_Blue" BackColor="#FFFFFF"
						ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX style="Z-INDEX: 109; POSITION: absolute; TEXT-ALIGN: right; TOP: 184px; LEFT: 248px"
						id="Txt_TotCredito" runat="server" Width="104px" Height="16px" CssClass="Testo_08_Nero" BackColor="#C0FFC0"
						ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<asp:label style="Z-INDEX: 110; POSITION: absolute; TOP: 184px; LEFT: 8px" id="Label22" runat="server"
						Width="152px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Totale IVA a credito:</asp:label>
					<ASP:PANEL style="Z-INDEX: 111; POSITION: absolute; SCROLLBAR-FACE-COLOR: #e6f4ff; OVERFLOW: auto; TOP: 24px; LEFT: 8px"
						id="Pannello_DataGridCredito" MS_POSITIONING="GridLayout" runat="server" Width="370px"
						Height="154px" BackColor="#E6F4FF" BORDERWIDTH="0px" BORDERCOLOR="#0000C0" BORDERSTYLE="None">&nbsp; 
<ASP:DATAGRID style="Z-INDEX: 102; POSITION: absolute; TOP: 0px; LEFT: 0px" id="DataGrid_Credito"
							runat="server" Width="350px" HorizontalAlign="Center" AutoGenerateColumns="False"
							CellSpacing="3" CellPadding="1" GridLines="None" ShowFooter="True">
							<FooterStyle Height="25px" BackColor="#5C9CCC"></FooterStyle>
							<AlternatingItemStyle Font-Size="8pt" Font-Names="Verdana" HorizontalAlign="Left" ForeColor="Black" VerticalAlign="Middle"
								BackColor="White"></AlternatingItemStyle>
							<ItemStyle Font-Size="8pt" Font-Names="Verdana" HorizontalAlign="Left" ForeColor="Black" VerticalAlign="Middle"
								BackColor="#E6F4FF"></ItemStyle>
							<HeaderStyle Font-Size="8pt" Font-Names="Verdana" Font-Bold="True" HorizontalAlign="Center" Height="25px"
								ForeColor="Black" VerticalAlign="Middle" BackColor="#5C9CCC"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="Imponibile_Netto" HeaderText="Imponibile (&amp;euro;)">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="Cod_Iva" HeaderText="Cod_Iva">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Aliquota_Des" HeaderText="Aliquota">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="IVA" HeaderText="IVA (&amp;euro;)">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
								</asp:BoundColumn>
							</Columns>
						</ASP:DATAGRID></ASP:PANEL>
					<asp:label style="Z-INDEX: 112; POSITION: absolute; TOP: 266px; LEFT: 16px" id="Label20" runat="server"
						Width="320px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF"> Note di Accredito Ricevute (Resi/Abbuoni su Acquisti) :</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 113; POSITION: absolute; TEXT-ALIGN: right; TOP: 264px; LEFT: 336px"
						id="Txt_NumResiAcquisti" runat="server" Width="42px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
				</asp:Panel>
<ASP:PANEL style="Z-INDEX: 102; POSITION: absolute; SCROLLBAR-FACE-COLOR: #e6f4ff; OVERFLOW: auto; TOP: 3px; LEFT: 8px"
					id="Pannello_Anno_Date" MS_POSITIONING="GridLayout" runat="server" Width="544px"
					Height="94px" BackColor="#E6F4FF" BORDERWIDTH="1px" BORDERCOLOR="#0000C0"
					BORDERSTYLE="None">&nbsp; 
<ASP:TEXTBOX style="Z-INDEX: 102; POSITION: absolute; TEXT-ALIGN: center; TOP: 45px; LEFT: 40px"
						id="Txt_DataFine" runat="server" Width="88px" Height="16px" CssClass="Testo_08_Blue datepicker"
						BackColor="#FFFFFF" BorderStyle="None"></ASP:TEXTBOX>
<ASP:TEXTBOX style="Z-INDEX: 103; POSITION: absolute; TEXT-ALIGN: center; TOP: 22px; LEFT: 40px"
						id="Txt_DataInizio" runat="server" Width="88px" Height="16px" CssClass="Testo_08_Blue datepicker"
						BackColor="#FFFFFF" BorderStyle="None"></ASP:TEXTBOX>
<ASP:LABEL style="Z-INDEX: 104; POSITION: absolute; TOP: 22px; LEFT: 8px" id="LABEL3" runat="server"
						Width="24px" Height="8px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Dal:</ASP:LABEL>
<ASP:LABEL style="Z-INDEX: 105; POSITION: absolute; TOP: 45px; LEFT: 8px" id="LABEL4" runat="server"
						Width="24px" Height="8px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Al:</ASP:LABEL>
<asp:label style="Z-INDEX: 109; POSITION: absolute; TOP: 3px; LEFT: 8px; height: 14px;" 
        id="Label1" runat="server"
						Width="164px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Periodo di Competenza :</asp:label>
<asp:label style="Z-INDEX: 114; POSITION: absolute; TOP: 3px; LEFT: 264px" id="Label27" runat="server"
						Width="112px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Sezionale:</asp:label>
<asp:dropdownlist style="Z-INDEX: 115; POSITION: absolute; TOP: 20px; LEFT: 264px" id="Cmb_Sezionali"
						runat="server" Width="272px" Height="16px" CssClass="Testo_08_Nero" AutoPostBack="True"></asp:dropdownlist>
<asp:label style="Z-INDEX: 114; POSITION: absolute; TOP: 45px; LEFT: 264px; width: 267px;" 
        id="LblRegimeIva" runat="server" Height="14px" CssClass="Testo_08_Nero" 
        BackColor="#E6F4FF"></asp:label>
<asp:label style="Z-INDEX: 114; POSITION: absolute; TOP: 59px; LEFT: 264px; width: 267px;" 
        id="LblLiquidazione" runat="server" Height="14px" CssClass="Testo_08_Nero" 
        BackColor="#E6F4FF"></asp:label>
<asp:label style="Z-INDEX: 114; POSITION: absolute; TOP: 73px; LEFT: 264px; width: 267px;" 
        id="LblEsigibilita" runat="server" Height="14px" CssClass="Testo_08_Nero" 
        BackColor="#E6F4FF"></asp:label>
<ASP:IMAGEBUTTON style="Z-INDEX: 116; POSITION: absolute; TOP: 24px; LEFT: 136px" id="ImgBtn_Indietro"
						runat="server" ImageUrl="../../../AB_Immagini/icone32/frecciasx.ico" Width="32px"
						Height="32px" BackColor="#E6F4FF" ToolTip="Annata Precedente"></ASP:IMAGEBUTTON>
<ASP:IMAGEBUTTON style="Z-INDEX: 117; POSITION: absolute; TOP: 24px; LEFT: 176px" id="ImgBtn_Avanti"
						runat="server" ImageUrl="../../../AB_Immagini/icone32/frecciadx.ico" Width="32px"
						Height="32px" BackColor="#E6F4FF" ToolTip="Annata Successiva"></ASP:IMAGEBUTTON></ASP:PANEL>
<asp:Panel style="Z-INDEX: 103; POSITION: absolute; SCROLLBAR-FACE-COLOR: #e6f4ff; OVERFLOW: auto; TOP: 104px; LEFT: 416px"
					id="Pannello_Debito" MS_POSITIONING="GridLayout" runat="server" Width="408px"
					Height="416px" BorderStyle="Solid" BorderWidth="1px" BorderColor="#0000C0">
					<asp:label style="Z-INDEX: 100; POSITION: absolute; TOP: 8px; LEFT: 128px" id="Label6" runat="server"
						Width="168px" Height="14px" CssClass="Testo_08_Nero" BackColor="#FFC0C0">IVA A DEBITO (Vendite)</asp:label>
					<asp:label style="Z-INDEX: 101; POSITION: absolute; TOP: 216px; LEFT: 8px" id="Label16" runat="server"
						Width="264px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Numero movimenti con IVA a Debito :</asp:label>
					<asp:label style="Z-INDEX: 102; POSITION: absolute; TOP: 242px; LEFT: 16px" id="Label17" runat="server"
						Width="200px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Fatture Emesse :</asp:label>
					<asp:label style="Z-INDEX: 103; POSITION: absolute; TOP: 386px; LEFT: 16px" id="Label18" runat="server"
						Width="144px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Altri Ricavi :</asp:label>
					<asp:label style="Z-INDEX: 104; POSITION: absolute; TOP: 338px; LEFT: 16px" id="Label19" runat="server"
						Width="168px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Corrispettivi di Vendita :</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 105; POSITION: absolute; TOP: 216px; LEFT: 328px" id="Txt_NumMovDebito"
						runat="server" Width="50px" Height="16px" CssClass="Testo_08_Blue" BackColor="#FFFFFF" ReadOnly="True"
						BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX style="Z-INDEX: 106; POSITION: absolute; TEXT-ALIGN: right; TOP: 240px; LEFT: 328px"
						id="Txt_NumFattureEmesse" runat="server" Width="51px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX style="Z-INDEX: 107; POSITION: absolute; TEXT-ALIGN: right; TOP: 384px; LEFT: 328px"
						id="Txt_NumAltriRicavi" runat="server" Width="50px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX style="Z-INDEX: 108; POSITION: absolute; TEXT-ALIGN: right; TOP: 336px; LEFT: 328px"
						id="Txt_NumVendite" runat="server" Width="50px" Height="16px" CssClass="Testo_08_Blue" BackColor="#FFFFFF"
						ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<ASP:TEXTBOX style="Z-INDEX: 109; POSITION: absolute; TEXT-ALIGN: right; TOP: 184px; LEFT: 248px"
						id="Txt_TotDebito" runat="server" Width="112px" Height="16px" CssClass="Testo_08_Nero" BackColor="#FFC0C0"
						ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<asp:label style="Z-INDEX: 110; POSITION: absolute; TOP: 184px; LEFT: 8px" id="Label23" runat="server"
						Width="136px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Totale IVA a debito:</asp:label>
					<asp:label style="Z-INDEX: 111; POSITION: absolute; TOP: 314px; LEFT: 16px" id="Label26" runat="server"
						Width="152px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Ricevute Fiscali Emesse :</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 113; POSITION: absolute; TEXT-ALIGN: right; TOP: 312px; LEFT: 328px"
						id="Txt_NumRicevuteFiscaliEmesse" runat="server" Width="50px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<ASP:PANEL style="Z-INDEX: 114; POSITION: absolute; SCROLLBAR-FACE-COLOR: #e6f4ff; OVERFLOW: auto; TOP: 24px; LEFT: 8px"
						id="Pannello_DataGridDebito" MS_POSITIONING="GridLayout" runat="server" Width="370px" Height="158px"
						BackColor="#E6F4FF" BORDERWIDTH="0px" BORDERCOLOR="#0000C0" BORDERSTYLE="None">
						<ASP:DATAGRID style="Z-INDEX: 101; POSITION: absolute; TOP: 0px; LEFT: 0px" id="DataGrid_Debito"
							runat="server" Width="350px" HorizontalAlign="Center" AutoGenerateColumns="False" CellSpacing="3"
							CellPadding="1" GridLines="None" ShowFooter="True">
							<FooterStyle Height="25px" BackColor="#5C9CCC"></FooterStyle>
							<AlternatingItemStyle Font-Size="8pt" Font-Names="Verdana" HorizontalAlign="Left" ForeColor="Black" VerticalAlign="Middle"
								BackColor="White"></AlternatingItemStyle>
							<ItemStyle Font-Size="8pt" Font-Names="Verdana" HorizontalAlign="Left" ForeColor="Black" VerticalAlign="Middle"
								BackColor="#E6F4FF"></ItemStyle>
							<HeaderStyle Font-Size="8pt" Font-Names="Verdana" Font-Bold="True" HorizontalAlign="Center" Height="25px"
								ForeColor="Black" VerticalAlign="Middle" BackColor="#5C9CCC"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="Imponibile_Netto" HeaderText="Imponibile (&amp;euro;)">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="Cod_Iva" HeaderText="Cod_Iva">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Aliquota_Des" HeaderText="Aliquota">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="IVA" HeaderText="IVA (&amp;euro;)">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Right" VerticalAlign="Middle"></ItemStyle>
								</asp:BoundColumn>
							</Columns>
						</ASP:DATAGRID>
					</ASP:PANEL>
					<asp:label style="Z-INDEX: 115; POSITION: absolute; TOP: 266px; LEFT: 16px" id="Label15" runat="server"
						Width="320px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Note di Accredito Emesse (Resi/Abbuoni su Vendite) :</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 116; POSITION: absolute; TEXT-ALIGN: right; TOP: 264px; LEFT: 328px"
						id="Txt_NumResiVendite" runat="server" Width="50px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<asp:label style="Z-INDEX: 117; POSITION: absolute; TOP: 362px; LEFT: 16px" id="Label28" runat="server"
						Width="160px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">DDT Contabilizzati Emessi:</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 118; POSITION: absolute; TEXT-ALIGN: right; TOP: 360px; LEFT: 328px"
						id="Txt_NumDDTContabilizzati_Emessi" runat="server" Width="50px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
					<asp:label style="Z-INDEX: 119; POSITION: absolute; TOP: 290px; LEFT: 16px" id="Label29" runat="server"
						Width="152px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Autoconsumo:</asp:label>
					<ASP:TEXTBOX style="Z-INDEX: 120; POSITION: absolute; TEXT-ALIGN: right; TOP: 288px; LEFT: 328px"
						id="Txt_NumAutoconsumo" runat="server" Width="50px" Height="16px" CssClass="Testo_08_Blue"
						BackColor="#FFFFFF" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
				</asp:Panel>
<asp:ImageButton style="Z-INDEX: 104; POSITION: absolute; TOP: 8px; LEFT: 560px" id="ImgBtnCaricaDati"
					runat="server" ImageUrl="../../../AB_Immagini/Icone32/Trova2.ico" BackColor="#E6F4FF"></asp:ImageButton>
<asp:label style="Z-INDEX: 105; POSITION: absolute; TOP: 48px; LEFT: 560px" id="Label2" runat="server"
					Width="84px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Carica i dati</asp:label>
<asp:label style="Z-INDEX: 106; POSITION: absolute; TOP: 119px; LEFT: 840px; height: 25px; width: 118px;" 
                    id="Label33" runat="server" CssClass="Testo_08_Nero" BackColor="#E6F4FF">IVA a credito da liquidazione precedente:</asp:label>
<ASP:TEXTBOX style="Z-INDEX: 107; POSITION: absolute; TEXT-ALIGN: right; TOP: 208px; LEFT: 840px"
					id="Txt_Credito" runat="server" Width="116px" Height="16px" CssClass="Testo_08_Nero"
					BackColor="#C0FFC0" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
<asp:label style="Z-INDEX: 108; POSITION: absolute; TOP: 234px; LEFT: 840px; bottom: 266px;" 
                    id="Label9" runat="server"
					Width="104px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">IVA a debito:</asp:label>
<ASP:TEXTBOX style="Z-INDEX: 109; POSITION: absolute; TEXT-ALIGN: right; TOP: 251px; LEFT: 840px"
					id="Txt_Debito" runat="server" Width="116px" Height="16px" CssClass="Testo_08_Nero"
					BackColor="#FFC0C0" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
<ASP:TEXTBOX style="Z-INDEX: 110; POSITION: absolute; TEXT-ALIGN: right; TOP: 389px; LEFT: 840px"
					id="Txt_Differenza" runat="server" Width="116px" Height="16px" CssClass="Testo_08_Nero"
					BackColor="#FFFFC0" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
                    <asp:label style="Z-INDEX: 111; POSITION: absolute; TOP: 413px; LEFT: 840px; width: 145px;" 
                    id="Label88" runat="server" Height="14px" CssClass="Testo_08_Nero" 
                    BackColor="#E6F4FF">Interesse a debito:</asp:label>
                    <ASP:TEXTBOX style="Z-INDEX: 110; POSITION: absolute; TEXT-ALIGN: right; TOP: 429px; LEFT: 911px; width: 59px;"
					id="Txt_InteresseDebito_Valore" runat="server" Height="16px" CssClass="Testo_08_Nero"
					BackColor="#FFFFC0" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
                       <asp:label style="Z-INDEX: 111; POSITION: absolute; TOP: 456px; LEFT: 840px; width: 145px;" 
                    id="Label8" runat="server" Height="14px" CssClass="Testo_08_Nero" 
                    BackColor="#E6F4FF">Iva da versare:</asp:label>
                    <ASP:TEXTBOX style="Z-INDEX: 110; POSITION: absolute; TEXT-ALIGN: right; TOP: 475px; LEFT: 840px"
					id="Txt_ImpostaDaVersare" runat="server" Width="116px" Height="16px" CssClass="Testo_08_Nero"
					BackColor="#FFFFC0" ReadOnly="True" BorderStyle="None"></ASP:TEXTBOX>
<asp:label style="Z-INDEX: 111; POSITION: absolute; TOP: 371px; LEFT: 840px" id="Label10" runat="server"
					Width="104px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Saldo:</asp:label>
<asp:ImageButton style="Z-INDEX: 112; POSITION: absolute; TOP: 8px; LEFT: 680px" id="ImgBtnStampa"
					runat="server" ImageUrl="../../../AB_Immagini/Icone32/Stampa.ico" BackColor="#E6F4FF"></asp:ImageButton>
<asp:label style="Z-INDEX: 113; POSITION: absolute; TOP: 16px; LEFT: 728px" id="Label21" runat="server"
					Width="84px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Stampa</asp:label>
<asp:CheckBox style="Z-INDEX: 114; POSITION: absolute; TOP: 8px; LEFT: 840px" id="Chk_DataStampa"
					runat="server" Width="150px" Height="38px" CssClass="Testo_08_Nero" BackColor="#E6F4FF"
					 Text="Visualizza data di stampa nel report"></asp:CheckBox>
<asp:CheckBox style="Z-INDEX: 114; POSITION: absolute; TOP: 75px; LEFT: 840px; height: 38px;" id="Chk_IVA_Precedente"
					runat="server" Width="150px" CssClass="Testo_08_Nero" BackColor="#E6F4FF"
					AutoPostBack="True" Text="Aggiungi IVA a Credito anno precedente" Enabled="False" 
                    Visible="False"></asp:CheckBox>
<ASP:TEXTBOX style="Z-INDEX: 115; POSITION: absolute; TEXT-ALIGN: right; TOP: 163px; LEFT: 840px"
					id="Txt_IVA_Precedente" runat="server" Width="116px" Height="16px" CssClass="Testo_08_Nero"
					BackColor="#C0FFC0" BorderStyle="None">0</ASP:TEXTBOX>
<asp:label style="Z-INDEX: 116; POSITION: absolute; TOP: 48px; LEFT: 672px" id="Label30" runat="server"
					Width="132px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Numero di pagina:</asp:label>
<ASP:TEXTBOX style="Z-INDEX: 117; POSITION: absolute; TOP: 48px; LEFT: 800px" id="Txt_NumPaginaRegIVA"
					runat="server" Width="45px" Height="18px" CssClass="Testo_08_Nero" BackColor="#FFFFFF"
					BorderStyle="None" MaxLength="10">1</ASP:TEXTBOX>
<asp:label style="Z-INDEX: 118; POSITION: absolute; TOP: 48px; LEFT: 848px" id="Label31" runat="server"
					Width="122px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">(dal quale iniziare la numerazione)</asp:label>
<ASP:TEXTBOX style="Z-INDEX: 119; POSITION: absolute; TEXT-ALIGN: right; TOP: 313px; LEFT: 840px"
					id="Txt_Acconto" runat="server" Width="116px" Height="16px" CssClass="Testo_08_Nero"
					BackColor="#C0FFC0" BorderStyle="None">0</ASP:TEXTBOX>
<asp:label style="Z-INDEX: 120; POSITION: absolute; TOP: 282px; LEFT: 840px" id="Label32" runat="server"
					Width="132px" Height="14px" CssClass="Testo_08_Nero" BackColor="#E6F4FF">Specifica un eventuale acconto versato:</asp:label>
<asp:Button style="Z-INDEX: 121; POSITION: absolute; TOP: 337px; LEFT: 848px" id="Btn_CalcolaSaldo"
					runat="server" Width="96px" Height="24px" Text="Calcola Saldo"></asp:Button>
                <asp:Label ID="Label34" runat="server" BackColor="#E6F4FF" 
                    CssClass="Testo_08_Nero" Height="14px" 
                    style="Z-INDEX: 106; POSITION: absolute; TOP: 190px; LEFT: 840px" Width="104px">IVA a credito:</asp:Label>
                <asp:TextBox ID="Txt_InteresseDebito_Perc" runat="server" BackColor="#FFFFC0" 
                    BorderStyle="None" CssClass="Testo_08_Nero" Height="16px" ReadOnly="True"                   
                   style="Z-INDEX: 110; POSITION: absolute; TEXT-ALIGN: right; TOP: 429px; LEFT: 843px; width: 32px;"></asp:TextBox>
                   <asp:label style="Z-INDEX: 111; POSITION: absolute; TOP: 429px; LEFT: 882px; width: 16px;" 
                    id="LblPerc" runat="server" Height="14px" CssClass="Testo_08_Nero" 
                    BackColor="#E6F4FF">%</asp:label>

            </ASP:PANEL></form>
	</body>
</HTML>

