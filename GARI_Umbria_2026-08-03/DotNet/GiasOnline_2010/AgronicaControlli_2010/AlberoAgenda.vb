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
Imports System.Text.RegularExpressions

''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' </summary>
''' <remarks></remarks>


<DefaultProperty("Text"), ToolboxData("<{0}:AlberoAgenda runat=server></{0}:AlberoAgenda>")>
Public Class AlberoAgenda
    Inherits System.Web.UI.WebControls.WebControl


    'Public Class AlberoAgenda
    '    Inherits ScriptControl

    Public Hidden As HiddenField




    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        Hidden = New HiddenField
        Hidden.ID = "HiddenSelezioneAgenda" & Me.ClientID
        'Hidden.ClientIDMode = UI.ClientIDMode.Static


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
            Case "IcoVeg1"
                Return "/AB_Immagini/Icone24/ope_colturali_24.ico"
            Case "IcoVeg2"
                Return "/AB_Immagini/Icone24/Sfera2_Verde_24.ico"
            Case "IcoVeg3"
                Return "/AB_Immagini/Icone24/Sfera_Verde_24.ico"

            Case "IcoContab1"
                Return "/AB_Immagini/Icone24/Ope_Contabili_24.ico"
            Case "IcoContab2"
                Return "/AB_Immagini/Icone24/Sfera2_Verde_24.ico"
            Case "IcoContab3"
                Return "/AB_Immagini/Icone24/Sfera_Verde_24.ico"


            Case "IcoZoo1"
                Return "/AB_Immagini/Icone24/ope_zootecniche_24.ico"
            Case "IcoZoo2"
                Return "/AB_Immagini/Icone24/Sfera2_Rossa_24.ico"
            Case "IcoZoo3"
                Return "/AB_Immagini/Icone24/Sfera_Rossa_24.ico"


            Case "IcoMac1"
                Return "/AB_Immagini/Icone24/ParcoMacchine24.ico"
            Case "IcoMac2"
                Return "/AB_Immagini/Icone24/Sfera2_Arancio_24.ico"
            Case "IcoMac3"
                Return "/AB_Immagini/Icone24/Sfera_Arancio_24.ico"



            Case "IcoFeriale"
                Return "/AB_Immagini/Icone24/Sfera2_Azzurra_24.ico"
            Case "IcoPrefestivo"
                Return "/AB_Immagini/Icone24/Sfera2_Arancio_24.ico"
            Case "IcoFestivo"
                Return "/AB_Immagini/Icone24/Sfera2_Rossa_24.ico"

            Case "IcoFerialeGG"
                Return "/f/Icone24/Sfera_Azzurra_24.ico"
            Case "IcoPrefestivoGG"
                Return "/AB_Immagini/Icone24/Sfera_Arancio_24.ico"
            Case "IcoFestivoGG"
                Return "/AB_Immagini/Icone24/Sfera_Rossa_24.ico"

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
    <WebMethod(EnableSession:=True)>
    Public Shared Function GetNodesAgenda(ByVal id As String, ByVal PathRoot As String) As String
        Dim results As New List(Of AjaxTreeNodeJsonObject)
        Dim i As Integer
        Dim PrimoLivello As New List(Of AjaxTreeNodeJsonObject)
        Dim SecondoLivelloC As New List(Of AjaxTreeNodeJsonObject)
        Dim SecondoLivelloP As New List(Of AjaxTreeNodeJsonObject)
        Dim SecondoLivelloZ As New List(Of AjaxTreeNodeJsonObject)
        Dim SecondoLivelloE As New List(Of AjaxTreeNodeJsonObject)

        If String.IsNullOrEmpty(id) Or id = "0" Then

            'Carico la radice dell'albero
            'Dim Radice As New AjaxTreeNodeJsonObject("R000", "Operazioni Disponibili", "#", PrimoLivello)
            'Radice.icon = PathRoot + "/AB_Immagini/Icone24/agronica_24.ico"



            'Carico 
            Dim Flag_FiltroOperazioniUtente As Boolean = False

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim filtroUtente As String = ""
            Dim dt_FiltroUtente As DataTable

            dt_FiltroUtente = objUtente.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           1,
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "", "",
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

            Dim IconaGruppo As String

            Dim Gruppo_Temp As Integer = 0
            Dim Tipo_Temp As String = ""

            'Ordinamento = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE & filtroUtente
            If Not IsNothing(HttpContext.Current.Session("Filtro_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_GruppoOperazioni") <> "" Then
                FiltroAggiuntivo &= "AND ( " & HttpContext.Current.Session("Filtro_GruppoOperazioni") & " ) "
            End If



            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DT As DataTable
            DT = objOperazioniLeggi.LeggixCaricaAlbero(AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        FiltroAggiuntivo,
                                                        "",
                                                        HttpContext.Current.Session("ASG_objParametri_Server"))



            For i = 0 To DT.Rows.Count - 1
                'verifico se è cambiato il macro gruppo
                If Tipo_Temp <> DT.Rows(i).Item("Tipo") Then
                    Tipo_Temp = DT.Rows(i).Item("Tipo")

                    Select Case Tipo_Temp
                        Case "C"
                            '=====================================================
                            '----- Operazioni COLTURALI
                            '=====================================================

                            Dim C000 As New AjaxTreeNodeJsonObject("C000", My.Resources.AgronicaControlli_2010.OperazioniColturali, "", "", "#", True)
                            C000.icon = PathRoot + PathIcona("IcoVeg1")
                            PrimoLivello.Add(C000)
                            PrimoLivello(PrimoLivello.Count - 1).children = SecondoLivelloC
                        Case "Z"
                            '=====================================================
                            '----- Operazioni ZOOTECNICHE
                            '=====================================================
                            Dim Z000 As New AjaxTreeNodeJsonObject("Z000", My.Resources.AgronicaControlli_2010.OperazioniZootecniche, "", "", "#", True)
                            Z000.icon = PathRoot + PathIcona("IcoZoo1")
                            PrimoLivello.Add(Z000)
                            PrimoLivello(PrimoLivello.Count - 1).children = SecondoLivelloZ
                        Case "P"
                            '=====================================================
                            '----- Operazioni PARCO MACCHINE
                            '=====================================================
                            Dim P000 As New AjaxTreeNodeJsonObject("P000", My.Resources.AgronicaControlli_2010.OperazioniMacchineEAttrezzature, "", "", "#", True)
                            P000.icon = PathRoot + PathIcona("IcoMac1")
                            PrimoLivello.Add(P000)
                            PrimoLivello(PrimoLivello.Count - 1).children = SecondoLivelloP

                        Case "E"
                            '=====================================================
                            '----- Operazioni CONTABILI
                            '=====================================================
                            Dim E000 As New AjaxTreeNodeJsonObject("E000", My.Resources.AgronicaControlli_2010.OperazioniContabiliEMagazzino, "", "", "#", True)
                            E000.icon = PathRoot + PathIcona("IcoContab1")
                            PrimoLivello.Add(E000)
                            PrimoLivello(PrimoLivello.Count - 1).children = SecondoLivelloE

                    End Select
                End If



                '-----------------------------------------------------
                '---------------- Gruppo Operazioni ------------------
                '-----------------------------------------------------
                'verifico se è cambiato il gruppo
                If Gruppo_Temp <> DT.Rows(i).Item("Gru_Cod") Then
                    Gruppo_Temp = DT.Rows(i).Item("Gru_Cod")

                    Dim desc As String = DT.Rows(i).Item("Gru_Des")
                    If CStr(DT.Rows(i).Item("Gru_Cod")) = "10" Then
                        desc = desc & My.Resources.AgronicaControlli_2010.Magazzino
                    End If
                    Dim Nodo As New AjaxTreeNodeJsonObject("G" & DT.Rows(i).Item("Gru_Cod"),
                                                           desc, "", "", "#", True)
                    'identifico l'icona giusta
                    Select Case Tipo_Temp

                        Case "C"
                            IconaGruppo = "IcoVeg2"
                            Nodo.icon = PathRoot + PathIcona(IconaGruppo)
                            SecondoLivelloC.Add(Nodo)

                        Case "Z"
                            IconaGruppo = "IcoZoo2"
                            Nodo.icon = PathRoot + PathIcona(IconaGruppo)
                            SecondoLivelloZ.Add(Nodo)

                        Case "P"
                            IconaGruppo = "IcoMac2"
                            Nodo.icon = PathRoot + PathIcona(IconaGruppo)
                            SecondoLivelloP.Add(Nodo)

                        Case "E"
                            IconaGruppo = "IcoContab2"
                            Nodo.icon = PathRoot + PathIcona(IconaGruppo)
                            SecondoLivelloE.Add(Nodo)

                    End Select

                End If

            Next

            'results.Add(Radice)
            Dim j As Integer
            For j = 0 To PrimoLivello.Count - 1
                results.Add(PrimoLivello(j))
            Next



        Else

            'passo l'id trovato alla caricaOperazioni
            CaricaOperazioni(results, id, PathRoot)

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

        dt_FiltroUtente = objUtente.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                       1,
                                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    "", "",
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
        DTOperazioni = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    FiltroAggiuntivo,
                                                    Ordinamento,
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


        Dim IDDiv As String = "treeAgenda" & Me.ClientID

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

        'StrSelect.AppendLine(" $.jstree._reference('#" + ID + "').jstree('get_selected');  ")

        'StrSelect.AppendLine(" alert($('#" + ID + "').jstree('get_selected').attr('id'));")
        'StrSelect.AppendLine(" alert($('#" + ID + "').jstree('get_selected').children('a').attr('id'));")

        StrSelect.AppendLine("  var nodi='';")
        'StrSelect.AppendLine("  var app;")
        'StrSelect.AppendLine("  var trovato=false;")
        StrSelect.AppendLine("  nodi =$(this).parent('li').attr('id'); ")
        'StrSelect.AppendLine("  app =$(this).attr('id'); ")

        'StrSelect.AppendLine(" $('#" + ID + "').jstree('get_selected').each(function() { ")
        'StrSelect.AppendLine("     if (app!=$(this).attr('id')) { ")
        'StrSelect.AppendLine("    nodi = nodi +$(this).attr('id') +'|'; }")
        'StrSelect.AppendLine("     else { ")
        'StrSelect.AppendLine("    trovato = true;}")
        'StrSelect.AppendLine(" });")

        'StrSelect.AppendLine("  if (trovato==false) { ")
        'StrSelect.AppendLine("      nodi = nodi + $(this).attr('id')+'|' ; ")
        'StrSelect.AppendLine("  };")

        'StrSelect.AppendLine("  alert(nodi); ")
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

        ' Giulia: 20/2/2018:se su db il valore comincia con la porta, qui la devo togliere
        Dim pathSpl = path.Split("/")
        If RegularExpressions.Regex.IsMatch(pathSpl(0), ":[0-9]*$", RegexOptions.None, TimeSpan.FromSeconds(3)) Then
            Dim grandezzaPorta = pathSpl(0).Length
            path = path.Remove(0, grandezzaPorta + 1)
        End If

        js.AppendLine("<script type='text/javascript'>")
        js.AppendLine("function OnGetNodesAgenda(n){		")
        js.AppendLine("     var port = location.port; ")
        js.AppendLine("     if (port != '' ) { port = ':'+ port;} ")

        js.AppendLine("     var obj = { ")
        js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        js.AppendLine("         , PathRoot : 'http://' + location.hostname + port + '/" & path & "'")
        js.AppendLine("			}")
        js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        js.AppendLine("     }")


        js.AppendLine("function OnNodesRetrievedSuccess(data,textstatus,xhr){ ")
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
        js.AppendLine("                     url:    GetNameofPage() +'/GetNodesAgenda',")
        js.AppendLine("                     async: true,")
        js.AppendLine("                 contentType:  'application/json; charset=utf-8',")
        js.AppendLine("                 dataType:  'json',")
        js.AppendLine("                 type:   'POST',")
        js.AppendLine("                 data: function (n) { return OnGetNodesAgenda(n); },")
        js.AppendLine("                 success: function (data, textstatus, xhr) {")
        js.AppendLine("                     return OnNodesRetrievedSuccess(data, textstatus, xhr)")
        js.AppendLine("		            },")
        js.AppendLine("                 error: function (xhr, textstatus, errorThrown) {")
        js.AppendLine("                     OnNodesRetrievedError(xhr, textstatus, errorThrown)")
        js.AppendLine("                 }")
        js.AppendLine("             }")
        js.AppendLine("         }")
        js.AppendLine("     });")

        'gestione del dbClick
        js.AppendLine("     $('#" + IDDiv + " ul').on('click', 'li a', function(){ ")
        'js.AppendLine("     $('#" + IDDiv + " ul').delegate('li', 'click', function(){ ")
        'controllo se il nodo è aperto o chiuso
        js.AppendLine("         if($(this).parent('li').attr('class')=='jstree-open') { ")
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



    Private Sub AlberoAgenda_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

End Class
