<%@ Page Language="VB" AutoEventWireup="false" Inherits="Agro_GoogleMaps.MapFinder_Tester" Codebehind="MapFinder_Tester.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title>Pagina senza titolo</title>
    
        
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    
        <asp:Label ID="Label1" runat="server" Style="left: 38px; position: absolute; top: 35px"
            Text="Latitudine" Width="95px"></asp:Label>
        <asp:Label ID="Label2" runat="server" Style="left: 38px; position: absolute; top: 64px"
            Text="Longitudine" Width="95px"></asp:Label>
        <asp:Label ID="Label3" runat="server" Style="left: 38px; position: absolute; top: 93px"
            Text="Zoom" Width="95px"></asp:Label>

    
        <asp:TextBox ID="TxtLatitudine" runat="server"
            Style="left: 146px; position: absolute; top: 33px; text-align: center" 
            Width="134px">xxxxxxxxxxxxxxx</asp:TextBox>
        
        <asp:TextBox ID="TxtLongitudine" runat="server" 
            Style="left: 146px; position: absolute; top: 62px; text-align: center" 
            Width="134px">xxxxxxxxxxxxxxx</asp:TextBox>
        
        <asp:TextBox ID="TxtZoom" runat="server" 
            Style="left: 146px; position: absolute; top: 93px; text-align: center" 
            Width="134px">xxxxxxxxxxxxxxx</asp:TextBox>

        
        <input id="BtnMappa" 
            name="BtnMappa" 
            style="left: 250px; width: 116px; position: absolute; top: 353px" 
            type="button"
            value="Mappa" />
            
       <%--<input id="BtnMappa" 
            name="BtnMappa" 
            style="left: 250px; width: 116px; position: absolute; top: 353px" 
            type="button"
            value="Mappa" 
            onClick=OpenMyDialog() />--%>
            
       <input id="BtnCancella" 
            name="BtnCancella" 
            style="left: 250px; width: 116px; position: absolute; top: 383px" 
            type="button"
            value="Cancella" />
            
            
            
        <asp:Label ID="Label4" runat="server" Style="left: 40px; position: absolute; top: 206px"
            Text="Latitudine N" Width="95px"></asp:Label>
        <asp:Label ID="Label5" runat="server" Style="left: 40px; position: absolute; top: 235px"
            Text="Longitudine O" Width="95px"></asp:Label>
        <asp:Label ID="Label6" runat="server" Style="left: 339px; position: absolute; top: 483px"
            Text="Latitudine S" Width="95px"></asp:Label>
        <asp:Label ID="Label7" runat="server" Style="left: 339px; position: absolute; top: 512px"
            Text="Longitudine E" Width="95px"></asp:Label>
        
        
        <asp:TextBox ID="TxtLatitudineN" runat="server" Style="left: 147px; position: absolute;
            top: 205px; text-align: center" Width="170px">xxxxxxxxxxxxxxx</asp:TextBox>
        
        <asp:TextBox ID="TxtLongitudineO" runat="server" Style="left: 147px; position: absolute;
            top: 234px; text-align: center" Width="170px">xxxxxxxxxxxxxxx</asp:TextBox>
        
        <asp:TextBox ID="TxtLatitudineS" runat="server" Style="left: 446px; position: absolute;
            top: 482px; text-align: center" Width="170px">xxxxxxxxxxxxxxx</asp:TextBox>
        
        <asp:TextBox ID="TxtLongitudineE" runat="server" Style="left: 446px; position: absolute;
            top: 511px; text-align: center" Width="170px">xxxxxxxxxxxxxxx</asp:TextBox>
                        
        <div ID="Pannello" Style="left: 212px; position: absolute; top: 270px; width: 187px ; height: 187px; border-right: fuchsia 1px solid; border-top: fuchsia 1px solid; border-left: fuchsia 1px solid; border-bottom: fuchsia 1px solid;">
            </div>
        
        <input id="BtnMappaDebug" 
            name="BtnMappaDebug" 
            style="left: 250px; width: 116px; position: absolute; top: 323px" 
            type="button"
            value="Mappa x Debug" />
        
        <input id="RitaglioGoogle" name="RitaglioGoogle" style="left: 250px; width: 116px; position: absolute; top: 600px" type="hidden" runat="server" /></div>
        
    </form>


	<SCRIPT language="vbscript" event="onclick" for="BtnCancella">
			document.all("TxtLatitudine").value = ""
			document.all("TxtLongitudine").value = ""
			document.all("TxtZoom").value = ""
			
			document.all("TxtLatitudineN").value = ""
			document.all("TxtLongitudineO").value = ""
			document.all("TxtLatitudineS").value = ""
			document.all("TxtLongitudineE").value = ""
	</SCRIPT>
    

	<SCRIPT language="vbscript" event="onclick" for="BtnMappa">
			'a = window.showModalDialog("http://www.agronica.it/x_Esperimenti/Container.htm","","dialogWidth:800px; dialogHeight:800px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ")
			'a = window.showModalDialog(document.all("QsAnalisiCrypt").value,"","dialogWidth:1024px;dialogHeight:768px;status:no; center:yes;edge:raised; help:no;")
			a = window.showModalDialog("Container.aspx?p=" & "<%=piloro%>" &  "&s=" & "<%=piloro2%>","","dialogWidth:800px; dialogHeight:800px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ")
			'a = window.showModalDialog("MapFinder.htm","","dialogWidth:530px; dialogHeight:580px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ")
			'a = window.showModalDialog("http://www.agronica.it/x_Esperimenti/MapFinder.htm","","dialogWidth:530px; dialogHeight:580px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ")
			'assegno il valore di ritorno della finestra modale
			
'			alert(a)
			if a<>"" then
			    document.all("RitaglioGoogle").value = "1"
				if a="-1" then
				
					document.all("TxtLatitudine").value = ""
					document.all("TxtLongitudine").value = ""
					document.all("TxtZoom").value = ""
					
			        document.all("TxtLatitudineN").value = ""
			        document.all("TxtLongitudineO").value = ""
			        document.all("TxtLatitudineS").value = ""
			        document.all("TxtLongitudineE").value = ""
			        					
				else
				
				    v=split(a,"~")
  					document.all("TxtLatitudine").value = v(0)
					document.all("TxtLongitudine").value = v(1)
					document.all("TxtZoom").value = v(2)

			        document.all("TxtLatitudineN").value = v(3)
			        document.all("TxtLongitudineO").value = v(4)
			        document.all("TxtLatitudineS").value = v(5)
			        document.all("TxtLongitudineE").value = v(6)
					
				end if
				
			else
			    document.all("RitaglioGoogle").value = "1"
				document.getElementById("Form1").submit()
			end if
	</SCRIPT>
	
	
	<%--<script type="text/javascript">
	 function OpenMyDialog()
        {
            
            var a;
            a = window.showModalDialog("Container.htm","","dialogWidth:800px; dialogHeight:800px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ");
//			a = window.showModalDialog("MapFinder.htm","","dialogWidth:530px; dialogHeight:580px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ")
//			a = window.showModalDialog("http://www.agronica.it/x_Esperimenti/MapFinder.htm","","dialogWidth:530px; dialogHeight:580px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ")
//			assegno il valore di ritorno della finestra modale
			if (a!="")
				{if (a=="-1")
				
					document.all("TxtLatitudine").value = "";
					document.all("TxtLongitudine").value = "";
					document.all("TxtZoom").value = "";
					
			        document.all("TxtLatitudineN").value = "";
			        document.all("TxtLongitudineO").value = "";
			        document.all("TxtLatitudineS").value = "";
			        document.all("TxtLongitudineE").value = "";
			     };
			        					
			else
			    {
				
				    v=split(a,"~");
  					document.all("TxtLatitudine").value = v(0);
					document.all("TxtLongitudine").value = v(1);
					document.all("TxtZoom").value = v(2);

			        document.all("TxtLatitudineN").value = v(3);
			        document.all("TxtLongitudineO").value = v(4);
			        document.all("TxtLatitudineS").value = v(5);
			        document.all("TxtLongitudineE").value = v(6);
					
				};			
        }
    </SCRIPT>	--%>
	


	<SCRIPT language="vbscript" event="onclick" for="BtnMappaDebug">
			'a = window.showModalDialog("../CTRL_AgroCalendario/AgroCalendario.aspx?dsel=" & document.all("Txt_DataInizioImpianto").value,"","dialogWidth:330px; dialogHeight:310px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ")
			a = window.showModalDialog("MapFinder_Debug.htm","","dialogWidth:750px; dialogHeight:600px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; ")
			'assegno il valore di ritorno della finestra modale
			if a<>"" then
				if a="-1" then
				
					document.all("TxtLatitudine").value = ""
					document.all("TxtLongitudine").value = ""
					document.all("TxtZoom").value = ""
					
			        document.all("TxtLatitudineN").value = ""
			        document.all("TxtLongitudineO").value = ""
			        document.all("TxtLatitudineS").value = ""
			        document.all("TxtLongitudineE").value = ""
			        					
				else
				
				    v=split(a,"~")
  					document.all("TxtLatitudine").value = v(0)
					document.all("TxtLongitudine").value = v(1)
					document.all("TxtZoom").value = v(2)

			        document.all("TxtLatitudineN").value = v(3)
			        document.all("TxtLongitudineO").value = v(4)
			        document.all("TxtLatitudineS").value = v(5)
			        document.all("TxtLongitudineE").value = v(6)
					
				end if					
			end if
	</SCRIPT>




    
</body>


</html>
