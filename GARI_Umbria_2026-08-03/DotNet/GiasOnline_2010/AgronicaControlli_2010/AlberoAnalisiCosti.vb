Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports System.ComponentModel
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' </summary>
''' <remarks></remarks>


<DefaultProperty("Text"), ToolboxData("<{0}:AlberoAnalisiCosti runat=server></{0}:AlberoAnalisiCosti>")> _
Public Class AlberoAnalisiCosti
    Inherits System.Web.UI.WebControls.WebControl

    Public Hidden As HiddenField

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        Hidden = New HiddenField
        Hidden.ID = "HiddenSelezioneCosti" & Me.ClientID
       
        Me.Controls.Add(Hidden)
        MyBase.OnInit(e)
    End Sub


    Public Property Valore_Albero() As String
        Get

            Return Hidden.Value
        End Get
        Set(ByVal value As String)
            Hidden.Value = value
        End Set
    End Property




    Private Shared Function PathIcona(ByVal ico As String)
        Select Case ico
            Case "radice"
                Return "/AB_Immagini/Icone32/AnalisiCosti01.ico"
            Case "0"
                Return "/AB_Immagini/Icone24/PersonaArancio.ico"
            Case "1", "2"
                Return "/AB_Immagini/Icone24/ParcoMacchine24.ico"
            Case "3"
                Return "/AB_Immagini/Icone24/Formulati24.ico"
            Case "10"
                Return "/AB_Immagini/Icone24/Silos_01.ico"
            Case "191"
                Return "/AB_Immagini/Icone24/Formulati24.ico"
            Case "197"
                Return "/AB_Immagini/Icone24/Avversita24_a.ico"
            Case "201"
                Return "/AB_Immagini/Icone24/ImpiantiLavorazione_02.ico"
            Case "sfera"
                Return "/AB_Immagini/Icone24/Sfera_Azzurra_24.ico"
            Case "agenda"
                Return "/AB_Immagini/Icone24/Agenda24.ico"
            Case Else
                Return ""

        End Select

    End Function


    ''' <summary>
    ''' Funzione che viene invocata quando si richiede il caricamento del sottonodo
    ''' NB ricordarsi di far "l'overload" all'interno della pagina chiamante come nell'esempio seguente
    ''' WebMethod(EnableSession:=True)> _
    ''' Public Shared Function GetNodes(ByVal id As String) As String
    '''    Return AgronicaControlli_2010.ServerControl1.GetNodesAgenda(id)
    ''' End Function
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)> _
    Public Shared Function GetNodesAnalisiCosti(ByVal id As String, ByVal PathRoot As String) As String
        Dim results As New List(Of AjaxTreeNodeJsonObject)
        Dim i, j, z As Integer
        Dim PrimoLivello As New List(Of AjaxTreeNodeJsonObject)
        Dim SecondoLivello As New List(Of AjaxTreeNodeJsonObject)
        Dim TerzoLivello As List(Of AjaxTreeNodeJsonObject)
        ' Dim SecondoLivelloZ As New List(Of AjaxTreeNodeJsonObject)
        ' Dim SecondoLivelloE As New List(Of AjaxTreeNodeJsonObject)

        If String.IsNullOrEmpty(id) Or id = "0" Then

            'Carico la radice dell'albero
            Dim Radice As New AjaxTreeNodeJsonObject("C000", "Analisi Costi", "", "", "#", True)
            Radice.icon = PathRoot + PathIcona("radice")
            results.Add(Radice)

        Else

            HttpContext.Current.Session("AlberoCaricato") = "1"

            Dim Dt As DataTable
            Dt = HttpContext.Current.Session("vs_dtStampe")

            Dim Raggruppamento As Integer
            Raggruppamento = HttpContext.Current.Session("vs_Raggruppamento")

            Dim Dettaglio As Integer
            Dettaglio = HttpContext.Current.Session("vs_Dettaglio")

            Dim CostoTot As Decimal
            Dim CostoLavorazione As Decimal

            If Not IsNothing(Dt) Then

                If Raggruppamento = 1 Then
                    'raggruppamento x elementi

                    Dim objDistinct As New AgronicaCoreUtility.DatatableUtility
                    Dim ElemCod() As String = objDistinct.SelectDistinct(Dt, "Elem_Cod")
                    Dim Elem_Cod As String

                    Dim DrElemCod() As DataRow

                    If Not ElemCod Is Nothing Then

                        For j = 0 To ElemCod.Length - 1

                            CostoTot = 0

                            Elem_Cod = ElemCod(j)
                            DrElemCod = Dt.Select("Elem_Cod=" & Elem_Cod)

                            TerzoLivello = New List(Of AjaxTreeNodeJsonObject)

                            For z = 0 To DrElemCod.Length - 1

                                CostoTot += DrElemCod(z).Item("Costo")

                                ' se c'è la visualizzazione analitica carico i nodi, altrimenti no
                                If Dettaglio = 1 Then

                                    Dim Desc As String
                                    Desc = DrElemCod(z).Item("Descrizione") & " - " & _
                                           "Qta:" & Format(CDbl(DrElemCod(z).Item("Qta")), "0.00") & " " & DrElemCod(z).Item("Udm_Qta") & " - " & _
                                           "Costo Unit:" & Format(CDbl(DrElemCod(z).Item("Costo_Unitario")), "##,###,##0.00") & "EUR -  " & _
                                           "Costo TOT:" & Format(CDbl(DrElemCod(z).Item("Costo")), "##,###,##0.00") & " EUR "
                                    Dim z_node As New AjaxTreeNodeJsonObject(i, Desc, "", "", "#", False)
                                    z_node.icon = PathRoot + PathIcona("sfera")

                                    TerzoLivello.Add(z_node)
                                End If


                            Next

                            Dim j_node As New AjaxTreeNodeJsonObject(i, DrElemCod(0).Item("NomeComune") & " " & Format(CostoTot, "##,###,##0.00") & " EUR", "", "", "#", TerzoLivello)
                            j_node.icon = PathRoot + PathIcona(Elem_Cod.ToString)

                            results.Add(j_node)

                        Next

                    End If

                Else
                    ' Raggruppamento per Attivita

                    Dim objDistinct As New AgronicaCoreUtility.DatatableUtility
                    Dim objOp As New AgronicaCoreMetaSchemaDAL.Operazioni_R
                    Dim LavCod() As String = objDistinct.SelectDistinct(Dt, "Lav_Cod")
                    Dim Lav_Cod As String
                    Dim Elem_Cod As String

                    Dim DrLavCod() As DataRow
                    Dim DrElemCod() As DataRow

                    PrimoLivello = New List(Of AjaxTreeNodeJsonObject)

                    If Not LavCod Is Nothing Then

                        For j = 0 To LavCod.Length - 1

                            Lav_Cod = LavCod(j)
                            DrLavCod = Dt.Select("Lav_Cod=" & Lav_Cod)

                            Dim DtElemCod As DataTable = DrLavCod.CopyToDataTable
                            Dim ElemCod() As String = objDistinct.SelectDistinct(DtElemCod, "Elem_Cod")

                            SecondoLivello = New List(Of AjaxTreeNodeJsonObject)

                            If Not ElemCod Is Nothing Then

                                For l = 0 To ElemCod.Length - 1

                                    Elem_Cod = ElemCod(l)
                                    DrElemCod = DtElemCod.Select("Elem_Cod=" & Elem_Cod)

                                    TerzoLivello = New List(Of AjaxTreeNodeJsonObject)

                                    For z = 0 To DrElemCod.Length - 1

                                        CostoTot += DrElemCod(z).Item("Costo")

                                        ' se c'è la visualizzazione analitica carico i nodi, altrimenti no
                                        If Dettaglio = 1 Then

                                            Dim Desc As String
                                            Desc = DrElemCod(z).Item("Descrizione") & " - " & _
                                                    "Qta:" & Format(CDbl(DrElemCod(z).Item("Qta")), "0.00") & " " & DrElemCod(z).Item("Udm_Qta") & " - " & _
                                                    "Costo Unit:" & Format(CDbl(DrElemCod(z).Item("Costo_Unitario")), "##,###,##0.00") & "EUR -  " & _
                                                    "Costo TOT:" & Format(CDbl(DrElemCod(z).Item("Costo")), "##,###,##0.00") & " EUR "


                                            Dim z_node As New AjaxTreeNodeJsonObject(i, Desc, "", "", "#", False)
                                            z_node.icon = PathRoot + PathIcona("sfera")

                                            TerzoLivello.Add(z_node)

                                        End If
                                    Next

                                    Dim elem_node As New AjaxTreeNodeJsonObject(i, DrElemCod(0).Item("NomeComune") & " " & Format(CostoTot, "##,###,##0.00") & " EUR", "", "", "#", TerzoLivello)
                                    elem_node.icon = PathRoot + PathIcona(DrElemCod(0).Item("Elem_Cod"))

                                    SecondoLivello.Add(elem_node)

                                    CostoLavorazione += CostoTot
                                    CostoTot = 0

                                Next

                            End If

                            Dim Lav_Des As String = objOp.LavorazioneDes_from_LavorazioneCod(Lav_Cod, HttpContext.Current.Session("ASG_objParametri_Server"))

                            Dim j_node As New AjaxTreeNodeJsonObject(i, Lav_Des & " " & Format(CostoLavorazione, "##,###,##0.00") & " Eur", "", "", "#", SecondoLivello)
                            j_node.icon = PathRoot + PathIcona("agenda")

                            CostoLavorazione = 0

                            results.Add(j_node)

                        Next

                    End If

                End If

            End If

        End If

        Dim ser As New JavaScriptSerializer()
        Return ser.Serialize(results)

    End Function

    Public Shared Sub CaricaOperazioni(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                  ByVal Id As String,
                                                  ByVal PathRoot As String)
        'Identifico  il GRU_COD
        Dim GRU_COD As String
        GRU_COD = Id.Substring(1, Id.Length - 1)

        Dim i As Integer


        Dim Flag_FiltroOperazioniUtente As Boolean = False
        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim filtroUtente As String = ""
        Dim dt_FiltroUtente As DataTable

        dt_FiltroUtente = objUtente.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI, _
                                                       1, _
                                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                    "", "", _
                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))
        If dt_FiltroUtente.Rows.Count > 0 Then
            filtroUtente = " AND Operazioni.Lav_Cod in ("
            For i = 0 To dt_FiltroUtente.Rows.Count - 1

                If i <> 0 Then
                    filtroUtente = filtroUtente & " ,"
                End If
                filtroUtente = filtroUtente & dt_FiltroUtente.Rows(i).Item("ID_0")
            Next
            filtroUtente = filtroUtente & " )  "
            Flag_FiltroOperazioniUtente = True
        End If


        Dim Ordinamento As String



        Dim Gruppo_Temp As Integer = 0
        Dim Tipo_Temp As String = Id.Substring(0, 1)

        Ordinamento = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
        Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE & " AND GruppoOperazioni.Gru_Cod = " & GRU_COD & filtroUtente
        If Not IsNothing(HttpContext.Current.Session("Filtro_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_GruppoOperazioni") <> "" Then
            FiltroAggiuntivo &= "AND ( " & HttpContext.Current.Session("Filtro_GruppoOperazioni") & " ) "
        End If

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim DTOperazioni As DataTable
        DTOperazioni = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False, _
                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                    FiltroAggiuntivo, _
                                                    Ordinamento, _
                                                    HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim IconaOp As String = ""


        For i = 0 To DTOperazioni.Rows.Count - 1


            '-----------------------------------------------------
            '----- Operazioni
            '-----------------------------------------------------

            Select Case DTOperazioni.Rows(i).Item("Tipo")
                Case "C"
                    IconaOp = "IcoVeg3"
                Case "Z"
                    IconaOp = "IcoZoo3"
                Case "P"
                    IconaOp = "IcoMac3"
                Case "E"
                    IconaOp = "IcoContab3"
            End Select

            'dato che non ho ulteriori sottonodi metto a false il nodo dei figli
            Dim Nodo As New AjaxTreeNodeJsonObject("O" & DTOperazioni.Rows(i).Item("Lav_Cod"),
                                                          DTOperazioni.Rows(i).Item("Lav_Des"), "", "", "#", False)


            Nodo.icon = PathRoot + PathIcona(IconaOp)
            results.Add(Nodo)

        Next

    End Sub

    ''' <summary>
    ''' Per Renderizzare il controllo
    ''' </summary>
    ''' <param name="writer"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)


        Dim IDDiv As String = "treeAnalisiCosti" & Me.ClientID

        Dim provahtml As New StringBuilder
        provahtml.Append("<div id='" + IDDiv + "'></div>")
        writer.Write(GetJS(IDDiv) + GetScriptSelect(IDDiv) + provahtml.ToString)
        MyBase.Render(writer)

    End Sub

    Private Function GetScriptSelect(ByVal ID As String)
        Dim StrSelect As New StringBuilder

        StrSelect.AppendLine("<script type='text/javascript'>")
        StrSelect.AppendLine("$(document).ready(function () { ")

        StrSelect.AppendLine("  $('#" + ID + " ul').on('click','li a', function(){ ")
        'StrSelect.AppendLine("  $('#" + ID + " ul').delegate('li', 'click', function(){ ")
        StrSelect.AppendLine("      $('#" & Hidden.ClientID & "').val($(this).parent('li').attr('id')); ")

        StrSelect.AppendLine("  var nodi='';")
        StrSelect.AppendLine("  nodi =$(this).attr('id'); ")

        'da gestire la deselezione perchè viene duplicato un nodo Se il nodo è doppio è da rimuovere
        StrSelect.AppendLine("  nodi =$('#" & Hidden.ClientID & "').val(nodi); ")

        StrSelect.AppendLine("  }); ")

        StrSelect.AppendLine("});")

        StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function

    ''' <summary>
    ''' Funzione per Farsi Restituire la stringa contenente il codice JS da inserire all'interno della pagina
    ''' !!! Da estendere eventualmente con la funzione del precaricamento tramite cookies !!!
    ''' </summary>
    ''' <param name="IDDiv">ID del DIV su cui applicare il plugin</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetJS(ByVal IDDiv As String) As String
        Dim js As New StringBuilder

        Dim IdNodiAperti As String = "'C000'"

        Dim path As String
        Dim objAgroWebConfig As New AgroWebConfig
        path = objAgroWebConfig.LinkAgronicaAgenda2010.Replace("/GestioneRichieste.aspx", "")

        js.AppendLine("<script type='text/javascript'>")
        'js.AppendLine("function OnGetNodes(n){		")
        'js.AppendLine("     var obj = { ")
        'js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        'js.AppendLine("         , PathRoot : '" + path + "'")
        'js.AppendLine("			}")
        'js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        'js.AppendLine("     }")

        js.AppendLine("function OnGetNodesAnalisi(n){		")
        js.AppendLine("     var port = location.port; ")
        js.AppendLine("     if (port != '' ) { port = ':'+ port;} ")

        js.AppendLine("     var obj = { ")
        js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        js.AppendLine("         , PathRoot : 'http://' + location.hostname + port + '/" & path & "'")
        js.AppendLine("			}")
        js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        js.AppendLine("     }")



        js.AppendLine("function OnNodesRetrievedSuccess(data,textstatus,xhr){ ")

        ' js.AppendLine("     var pippo = data.d; ")
        'js.AppendLine("     var pippo2 = pippo.replace('EUR','euro'); ")
        ' js.AppendLine("     return Sys.Serialization.JavaScriptSerializer.deserialize(pippo2); ")
        js.AppendLine("     return Sys.Serialization.JavaScriptSerializer.deserialize(data.d); ")
        js.AppendLine("} ")

        js.AppendLine(" function OnNodesRetrievedError(xhr,textstatus,errorThrown){")
        js.AppendLine("     alert('ERRORE!!!' +errorThrown); ")
        js.AppendLine(" } ")


        js.AppendLine("$(document).ready(function () { ")

        js.AppendLine("    $('#" + IDDiv + "').jstree({ ")
        js.AppendLine("        core : { ")
        js.AppendLine("             'initially_open' : [ " & IdNodiAperti & "] ")
        js.AppendLine("        }, ")

        js.AppendLine("        ui : { ")
        js.AppendLine("             'select_limit' : 1,'initially_select' :[ '' ] ")
        js.AppendLine("        }, ")



        js.AppendLine("        themes : { ")
        js.AppendLine("        'theme' : 'apple' ")
        js.AppendLine("        }, ")

        js.AppendLine("        plugins: ['themes', 'json_data',  'ui',  'hotkeys'],")
        'js.AppendLine("        plugins: ['themes', 'json_data',  'ui', 'cookies', 'hotkeys'],")
        js.AppendLine("        json_data: { ")
        js.AppendLine("		            ajax: {")
        js.AppendLine("                     url:    GetNameofPage() +'/GetNodesAnalisiCosti',")
        js.AppendLine("                     async: true,")
        js.AppendLine("                 contentType:  'application/json; charset=utf-8',")
        js.AppendLine("                 dataType:  'json',")
        js.AppendLine("                 type:   'POST',")
        js.AppendLine("                 data: function (n) { return OnGetNodesAnalisi(n); },")
        js.AppendLine("                 success: function (data, textstatus, xhr) {")
        js.AppendLine("                     return OnNodesRetrievedSuccess(data, textstatus, xhr);  ")

        'js.AppendLine("                     return OnNodesRetrievedSuccess(data, textstatus, xhr)")
        js.AppendLine("		            },")
        js.AppendLine("                 error: function (xhr, textstatus, errorThrown) {")
        js.AppendLine("                     OnNodesRetrievedError(xhr, textstatus, errorThrown)")
        js.AppendLine("                 }")
        js.AppendLine("             }")
        js.AppendLine("         }")
        js.AppendLine("     });")

        'gestione del dbClick

        js.AppendLine("     $(document).on('click', '#" + IDDiv + " ul li a', function () { ")
        'js.AppendLine("     $('#" + IDDiv + " ul li a').live('click', function(){ ")
        'controllo se il nodo è aperto o chiuso
        js.AppendLine("         if($(this).parent().attr('class')=='jstree-open') { ")
        js.AppendLine("         $('#" + IDDiv + "').jstree('close_node', this); } else {")
        js.AppendLine("         $('#" + IDDiv + "').jstree('open_node', this); } ")
        js.AppendLine("     });")

        js.AppendLine(" });")


        js.AppendLine()
        js.AppendLine("function GetNameofPage() {")
        js.AppendLine(" var pa = new String(window.location.pathname); ")
        js.AppendLine("var p2 = pa.split('/');")
        js.AppendLine("    return p2[p2.length - 1]; ")
        js.AppendLine("}")

        js.AppendLine("</script>")
        Return js.ToString
    End Function



    'Private Sub AlberoAnalisiCosti_Init(sender As Object, e As System.EventArgs) Handles Me.Init
    '    Lingua.Gias_InizializzaCultura_DaSession()
    'End Sub



End Class
