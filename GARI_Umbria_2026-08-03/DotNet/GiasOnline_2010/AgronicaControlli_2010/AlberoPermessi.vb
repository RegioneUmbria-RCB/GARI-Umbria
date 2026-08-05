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



Public Class AlberoPermessi
    Inherits System.Web.UI.WebControls.WebControl
    Public Hidden As HiddenField
    Public Username As String

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        Hidden = New HiddenField
        Hidden.ID = "HiddenSelezionePermessi" & Me.ClientID

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
    ''' <summary>
    ''' Per Renderizzare il controllo
    ''' </summary>
    ''' <param name="writer"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Dim IDDiv As String = "treePermessi" & Me.ClientID

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

        StrSelect.AppendLine("  var nodi='';")
        StrSelect.AppendLine("  nodi =$(this).parent('li').attr('id'); ")

        'StrSelect.AppendLine("  alert(nodi); ")
        'da gestire la deselezione perchè viene duplicato un nodo Se il nodo è doppio è da rimuovere
        StrSelect.AppendLine("  nodi =$('#" & Hidden.ClientID & "').val(nodi); ")

        StrSelect.AppendLine("  }); ")

        StrSelect.AppendLine("});")

        StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
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
    Public Shared Function GetNodesPermessi(ByVal id As String, ByVal PathRoot As String) As String

        Dim Username = HttpContext.Current.Session("Username")
        Dim Tipologia_Cod = HttpContext.Current.Session("Tipologia_Cod")
        HttpContext.Current.Session("Username") = Nothing
        HttpContext.Current.Session("Tipologia_Cod") = Nothing

        Dim results As New List(Of AjaxTreeNodeJsonObject)

        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(
                                HttpContext.Current.Session("ASG_objParametri_Utenti"))

        If String.IsNullOrEmpty(id) Or id = "0" Then

            'FIltro i permessi dell'utente che li sta assegnando
            Dim FiltroPermessi As String = ""
            Dim i As Integer
            Dim DtPermessiUtenteCorrente As DataTable
            Dim objPermessiUtenteCorrente As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            DtPermessiUtenteCorrente = objPermessiUtenteCorrente.Leggi(HttpContext.Current.Session("ASG_Utente_Username"), 5, 0, 9999, 0, "", "", objParametri_Utenti)
            If Not DtPermessiUtenteCorrente Is Nothing Then
                For i = 0 To DtPermessiUtenteCorrente.Rows.Count - 1
                    If InStr(FiltroPermessi, " " & DtPermessiUtenteCorrente.Rows(i).Item("Id_Attivita") & ",") = 0 Then
                        FiltroPermessi += " " & DtPermessiUtenteCorrente.Rows(i).Item("Id_Attivita") & ","
                    End If
                Next
                If FiltroPermessi <> "" Then
                    FiltroPermessi = " Id_Attivita_Figlio IN (" & Left(FiltroPermessi, FiltroPermessi.Length - 1) & ") "
                End If
            End If


            Dim DtPermessi As DataTable
            Dim objPermessi As New AgronicaCoreUtentiDAL.Gerarchia_Attivita_R
            DtPermessi = objPermessi.Leggi(0, 0, 5, 0, 0, 0, FiltroPermessi, "", objParametri_Utenti)

            'se sono in modifica dell'utente leggo i suoi permessi
            Dim DtPermessiU_R As DataTable
            Dim DtPermessiU_W As DataTable
            If Not Username Is Nothing Then
                Dim objPermessiU As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                DtPermessiU_R = objPermessiU.Leggi(Username, 5, 0, enum_Security_Operazione.Lettura, 0, "", "", objParametri_Utenti)
                DtPermessiU_W = objPermessiU.Leggi(Username, 5, 0, enum_Security_Operazione.Modifica, 0, "", "", objParametri_Utenti)
            End If
            If Not Tipologia_Cod Is Nothing Then
                Dim objPermessiU As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R
                DtPermessiU_R = objPermessiU.Leggi(Tipologia_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, " id_operazione =0", "", objParametri_Utenti)
                DtPermessiU_W = objPermessiU.Leggi(Tipologia_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, " id_operazione =2", "", objParametri_Utenti)
            End If

            '----------------------------------------------------------------------------
            '----- Livello 1 ------------------------------------------------------------
            '----------------------------------------------------------------------------
            Dim i_1 As Integer
            Dim i_2 As Integer
            Dim i_3 As Integer
            Dim i_4 As Integer

            Dim DrL1() As DataRow
            Dim DrL2() As DataRow
            Dim DrL3() As DataRow
            Dim DrL4() As DataRow

            Dim Descrizione1 As String
            Dim Descrizione2 As String
            Dim Descrizione3 As String
            Dim Descrizione4 As String

            Dim Codice1 As Integer
            Dim Codice2 As Integer
            Dim Codice3 As Integer
            Dim Codice4 As Integer

            Dim DrPermesso_R() As DataRow
            Dim DrPermesso_W() As DataRow
            Dim Classe1 As String
            Dim Classe2 As String
            Dim Classe3 As String
            Dim Classe4 As String

            Dim ClasseVerde As String = "verde"
            Dim ClasseRosso As String = "rosso"
            Dim ClasseNessuno As String = "nessuno"

            DrL1 = DtPermessi.Select("Livello=1")

            Dim Livello1 As New List(Of AjaxTreeNodeJsonObject)

            If Not DrL1 Is Nothing Then

                For i_1 = 0 To DrL1.Length - 1

                    Descrizione1 = DrL1(i_1).Item("Attivita_Des_Figlio")
                    Codice1 = DrL1(i_1).Item("ID_Attivita_Figlio")

                    Classe1 = ClasseNessuno
                    If Not Username Is Nothing Or Not Tipologia_Cod Is Nothing Then
                        DrPermesso_W = DtPermessiU_W.Select("ID_Attivita=" & Codice1.ToString)
                        If Not DrPermesso_W Is Nothing AndAlso DrPermesso_W.Length > 0 Then
                            Classe1 = ClasseRosso
                        Else
                            DrPermesso_R = DtPermessiU_R.Select("ID_Attivita=" & Codice1.ToString)
                            If Not DrPermesso_R Is Nothing AndAlso DrPermesso_R.Length > 0 Then
                                Classe1 = ClasseVerde
                            End If
                        End If
                    End If


                    '----------------------------------------------------------------------------
                    '----- Livello 2 ------------------------------------------------------------
                    '----------------------------------------------------------------------------

                    DrL2 = DtPermessi.Select("ID_Attivita_Padre=" & Codice1.ToString)

                    Dim Livello2 As New List(Of AjaxTreeNodeJsonObject)

                    If Not DrL2 Is Nothing Then

                        For i_2 = 0 To DrL2.Length - 1

                            Descrizione2 = DrL2(i_2).Item("Attivita_Des_Figlio")
                            Codice2 = DrL2(i_2).Item("ID_Attivita_Figlio")

                            Classe2 = ClasseNessuno
                            If Not Username Is Nothing Or Not Tipologia_Cod Is Nothing Then

                                DrPermesso_W = DtPermessiU_W.Select("ID_Attivita=" & Codice2.ToString)
                                If Not DrPermesso_W Is Nothing AndAlso DrPermesso_W.Length > 0 Then
                                    Classe2 = ClasseRosso
                                Else
                                    DrPermesso_R = DtPermessiU_R.Select("ID_Attivita=" & Codice2.ToString)
                                    If Not DrPermesso_R Is Nothing AndAlso DrPermesso_R.Length > 0 Then
                                        Classe2 = ClasseVerde
                                    End If
                                End If
                            End If
                            '----------------------------------------------------------------------------
                            '----- Livello 3 ------------------------------------------------------------
                            '----------------------------------------------------------------------------

                            DrL3 = DtPermessi.Select("ID_Attivita_Padre=" & Codice2.ToString)

                            Dim Livello3 As New List(Of AjaxTreeNodeJsonObject)

                            If Not DrL3 Is Nothing Then

                                For i_3 = 0 To DrL3.Length - 1

                                    Descrizione3 = DrL3(i_3).Item("Attivita_Des_Figlio")
                                    Codice3 = DrL3(i_3).Item("ID_Attivita_Figlio")

                                    Classe3 = ClasseNessuno
                                    If Not Username Is Nothing Or Not Tipologia_Cod Is Nothing Then

                                        DrPermesso_W = DtPermessiU_W.Select("ID_Attivita=" & Codice3.ToString)
                                        If Not DrPermesso_W Is Nothing AndAlso DrPermesso_W.Length > 0 Then
                                            Classe3 = ClasseRosso
                                        Else
                                            DrPermesso_R = DtPermessiU_R.Select("ID_Attivita=" & Codice3.ToString)
                                            If Not DrPermesso_R Is Nothing AndAlso DrPermesso_R.Length > 0 Then
                                                Classe3 = ClasseVerde
                                            End If
                                        End If
                                    End If
                                    '----------------------------------------------------------------------------
                                    '----- Livello 4 ------------------------------------------------------------
                                    '----------------------------------------------------------------------------

                                    DrL4 = DtPermessi.Select("ID_Attivita_Padre=" & Codice3.ToString)

                                    Dim Livello4 As New List(Of AjaxTreeNodeJsonObject)

                                    If Not DrL4 Is Nothing Then

                                        For i_4 = 0 To DrL4.Length - 1

                                            Descrizione4 = DrL4(i_4).Item("Attivita_Des_Figlio")
                                            Codice4 = DrL4(i_4).Item("ID_Attivita_Figlio")

                                            Classe4 = ClasseNessuno
                                            If Not Username Is Nothing Or Not Tipologia_Cod Is Nothing Then

                                                DrPermesso_W = DtPermessiU_W.Select("ID_Attivita=" & Codice4.ToString)
                                                If Not DrPermesso_W Is Nothing AndAlso DrPermesso_W.Length > 0 Then
                                                    Classe4 = ClasseRosso
                                                Else
                                                    DrPermesso_R = DtPermessiU_R.Select("ID_Attivita=" & Codice4.ToString)
                                                    If Not DrPermesso_R Is Nothing AndAlso DrPermesso_R.Length > 0 Then
                                                        Classe4 = ClasseVerde
                                                    End If
                                                End If
                                            End If

                                            Dim Livello5 As New List(Of AjaxTreeNodeJsonObject)

                                            If Livello5.Count > 0 Then
                                                Dim L4 As New AjaxTreeNodeJsonObject(Codice4, Descrizione4, Classe4, "", "#", True)
                                                Livello4.Add(L4)
                                                Livello4(Livello4.Count - 1).children = Livello5
                                            Else
                                                Dim L4 As New AjaxTreeNodeJsonObject(Codice4, Descrizione4, Classe4, "", "#", False)
                                                Livello4.Add(L4)
                                            End If

                                        Next


                                    End If





                                    If Livello4.Count > 0 Then
                                        Dim L3 As New AjaxTreeNodeJsonObject(Codice3, Descrizione3, Classe3, "", "#", True)
                                        Livello3.Add(L3)
                                        Livello3(Livello3.Count - 1).children = Livello4
                                    Else
                                        Dim L3 As New AjaxTreeNodeJsonObject(Codice3, Descrizione3, Classe3, "", "#", False)
                                        Livello3.Add(L3)
                                    End If

                                Next

                            End If

                            If Livello3.Count > 0 Then
                                Dim L2 As New AjaxTreeNodeJsonObject(Codice2, Descrizione2, Classe2, "", "#", True)
                                Livello2.Add(L2)
                                Livello2(Livello2.Count - 1).children = Livello3
                            Else
                                Dim L2 As New AjaxTreeNodeJsonObject(Codice2, Descrizione2, Classe2, "", "#", False)
                                Livello2.Add(L2)
                            End If

                        Next

                    End If

                    If Livello2.Count > 0 Then
                        Dim L1 As New AjaxTreeNodeJsonObject(Codice1, Descrizione1, Classe1, "", "#", True)
                        Livello1.Add(L1)
                        Livello1(Livello1.Count - 1).children = Livello2
                    Else
                        Dim L1 As New AjaxTreeNodeJsonObject(Codice1, Descrizione1, Classe1, "", "#", False)
                        Livello1.Add(L1)
                    End If

                Next

            End If

            Dim j As Integer
            For j = 0 To Livello1.Count - 1
                results.Add(Livello1(j))
            Next

        End If

        Dim ser As New JavaScriptSerializer()
        Return ser.Serialize(results)

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

        Dim IdNodiAperti As String = "'1'"

        Dim path As String
        Dim objAgroWebConfig As New AgroWebConfig
        path = objAgroWebConfig.LinkAgronicaProfilazione.Replace("/GestioneRichieste.aspx", "")

        js.AppendLine("<script type='text/javascript'>")
        'js.AppendLine("function OnGetNodes(n){		")
        'js.AppendLine("     var obj = { ")
        'js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        'js.AppendLine("         , PathRoot : '" + path + "'")
        'js.AppendLine("			}")
        'js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        'js.AppendLine("     }")
         

        js.AppendLine("function OnGetNodesPermessi(n){		")
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

        'js.AppendLine("        plugins: ['themes', 'json_data',  'ui',  'hotkeys'],")
        js.AppendLine("        plugins: ['themes', 'json_data',  'ui', 'checkbox', 'hotkeys'],")
        'js.AppendLine("        plugins: ['themes', 'json_data',  'ui', 'cookies', 'hotkeys'],")
        js.AppendLine("        json_data: { ")
        js.AppendLine("		            ajax: {")
        js.AppendLine("                     url:    GetNameofPage() +'/GetNodesPermessi',")
        js.AppendLine("                     async: true,")
        js.AppendLine("                 contentType:  'application/json; charset=utf-8',")
        js.AppendLine("                 dataType:  'json',")
        js.AppendLine("                 type:   'POST',")
        js.AppendLine("                 data: function (n) { return OnGetNodesPermessi(n); },")
        js.AppendLine("                 success: function (data, textstatus, xhr) {")
        js.AppendLine("                     return OnNodesRetrievedSuccess(data, textstatus, xhr)")
        js.AppendLine("		            },")
        js.AppendLine("                 error: function (xhr, textstatus, errorThrown) {")
        js.AppendLine("                     OnNodesRetrievedError(xhr, textstatus, errorThrown)")
        js.AppendLine("                 }")
        js.AppendLine("             }")
        js.AppendLine("         }")
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


End Class
