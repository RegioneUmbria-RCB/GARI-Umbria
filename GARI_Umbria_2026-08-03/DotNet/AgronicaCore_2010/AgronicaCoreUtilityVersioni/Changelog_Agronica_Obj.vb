Imports System.IO
Imports System.Text.RegularExpressions
Imports AgronicaCoreUtilityVersioni.Enumerativi
Imports Newtonsoft.Json

Public Class Changelog_Agronica_Obj

    Const SCHEMA_JSON As String = "../../schema/changelog.schema.json"

    <JsonProperty(PropertyName:="$schema")>
    Public Property schema As String
    Public Property progetto As String
    Public Property versioni As List(Of Versione_Obj)

    <JsonIgnore()>
    Public Property lastVersion As String = ""
    <JsonIgnore()>
    Public Property lastVersionNumber As String = ""
    <JsonIgnore()>
    Public Property hasUnreleased As Boolean = False
    <JsonIgnore()>
    Public Property giasVersioneCorrenteTxt As String = ""

    Public Sub New()
        Me.schema = SCHEMA_JSON
        Me.progetto = ""
        Me.versioni = New List(Of Versione_Obj)
    End Sub

    Public Sub New(ByVal projectName As String, Optional ByVal schema As String = "")
        Me.schema = If(Not String.IsNullOrEmpty(schema), schema, SCHEMA_JSON)
        Me.progetto = projectName
        Me.versioni = New List(Of Versione_Obj)
    End Sub

    Public Function IsVersioneConJson(ByVal sito As enum_AgronicaSitiRelease) As Boolean
        Select Case sito
            Case enum_AgronicaSitiRelease.CoreAPI,
                enum_AgronicaSitiRelease.ProfitosanAPI,
                enum_AgronicaSitiRelease.GiasNG,
                enum_AgronicaSitiRelease.NetCoreAPI,
                enum_AgronicaSitiRelease.NetCoreDataExchange,
                enum_AgronicaSitiRelease.QDCACompliance,
                enum_AgronicaSitiRelease.JobScheduler

                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Sub ValorizzaLastVersion()
        Dim last As Versione_Obj = Nothing
        Dim secondToLast As Versione_Obj = Nothing

        If Me.versioni IsNot Nothing AndAlso Me.versioni.Count > 0 Then
            last = Me.versioni.FirstOrDefault()

            If last IsNot Nothing AndAlso Not String.IsNullOrEmpty(last.data) Then
                If ListPossibleUnreleased.Exists(Function(s) last.data.ToLower().Contains(s)) Then
                    Me.hasUnreleased = True
                    If Me.versioni.Count > 1 Then
                        secondToLast = Me.versioni.Skip(1).FirstOrDefault()
                    End If
                End If

                Me.lastVersion = If(secondToLast IsNot Nothing, secondToLast.data, last.data)
                Me.lastVersionNumber = If(secondToLast IsNot Nothing, secondToLast.versione, last.versione)
            End If
        End If
    End Sub

    Public Sub ValorizzaLastVersionTxt(ByRef elencoChiaviVersioni As Dictionary(Of String, String).KeyCollection)
        Dim last As String = Nothing
        Dim secondToLast As String = Nothing

        If elencoChiaviVersioni IsNot Nothing AndAlso elencoChiaviVersioni.Count > 0 Then
            last = elencoChiaviVersioni.FirstOrDefault()

            If Not String.IsNullOrEmpty(last) Then
                If ListPossibleUnreleased.Exists(Function(s) last.ToLower().Contains(s)) Then
                    Me.hasUnreleased = True
                    If elencoChiaviVersioni.Count > 1 Then
                        secondToLast = elencoChiaviVersioni.Skip(1).FirstOrDefault()
                    End If
                End If

                Me.lastVersion = If(secondToLast IsNot Nothing, secondToLast, last)
            End If
        End If
    End Sub

    Public Function SerializzaJson() As String
        Dim jsonSettings As New JsonSerializerSettings With {
            .DefaultValueHandling = DefaultValueHandling.Ignore
        }

        Dim jSonString As String = JsonConvert.SerializeObject(Me, Formatting.Indented, jsonSettings)

        Return jSonString
    End Function

    Public Function ScriviJson(ByVal pathFileName As String, Optional ByVal flagSpostaBugs As Boolean = True) As Boolean
        Const nomeRoutine = "ScriviJson"
        Dim result As Boolean = False
        Try

            If flagSpostaBugs Then
                Me.SpostaTuttiBugs()
            End If

            Dim jSonString As String = Me.SerializzaJson()

            Using objWriter As New StreamWriter(pathFileName, False)
                objWriter.Write(jSonString)
                objWriter.Flush()
                objWriter.Close()
            End Using
            result = True
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Per evitare problemi che mi rimane il json scritto da altri che poi non lo riesco più a sovrascrivere, lo cancello
    ''' </summary>
    ''' <param name="pathFileName"></param>
    Public Function CancellaJson(ByVal pathFileName As String) As Boolean
        Const nomeRoutine = "CancellaJson"
        Dim result As Boolean = False
        Try

            If File.Exists(pathFileName) Then
                File.Delete(pathFileName)
            End If

            result = True
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return result
    End Function

    Public Shared Function DeSerializzaJson(ByVal jsonString As String) As Changelog_Agronica_Obj
        Return JsonConvert.DeserializeObject(Of Changelog_Agronica_Obj)(jsonString)
    End Function

    Public Shared Function LeggiDaFileJson(ByVal pathFileName As String) As Changelog_Agronica_Obj
        Const nomeRoutine = "LeggiDaFileJson"
        Dim jSonString As String = ""

        Try
            Using objReader As New StreamReader(pathFileName)
                jSonString = objReader.ReadToEnd()
                objReader.Close()
            End Using
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return DeSerializzaJson(jSonString)
    End Function

    Public Sub LeggiGiasVersioneCorrente(ByVal pathInput As String)
        Const nomeRoutine = "LeggiGiasVersioneCorrente"
        Try
            Dim contenutoVersioneCorrente As String = ""
            Dim pathVersioneCorrente As String = Path.Combine(pathInput, "GiasVersioneCorrente.txt")

            If File.Exists(pathVersioneCorrente) Then
                contenutoVersioneCorrente = File.ReadAllText(pathVersioneCorrente)
            End If

            Me.giasVersioneCorrenteTxt = contenutoVersioneCorrente
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Public Function GetFileNameJson() As String
        Return Me.progetto.Replace(" ", "") & ".json"
    End Function

    Public Sub SpostaTuttiBugs()
        For Each versione In Me.versioni
            versione.changelog.SpostaBugs()
        Next
    End Sub

    Public Function TrovaTuttiBugs() As String

        Dim allBugs As New List(Of Feature_Obj)
        Dim allDescrBugs As New List(Of String)

        For Each versione In Me.versioni
            Dim thisBugs As List(Of Feature_Obj) = versione.changelog.TrovaBugs()
            allBugs.AddRange(thisBugs)
            allDescrBugs.AddRange(thisBugs.Select(Function(x) x.descrizione))
        Next

        Dim res As String = "<li>" & String.Join("</li><li>", allDescrBugs) & "</li>"
        Return res
    End Function

    Public Sub Riga_Changelog(ByVal tipoChangelog As enum_Tipo_Changelog,
                              ByVal area As String,
                              ByVal descrizione As String,
                              ByVal cliente As String,
                              ByVal idTicketAssistenza As Integer,
                              ByVal idTicketSviluppo As Integer,
                              ByVal idTicketTesting As Integer,
                              ByVal autore As String,
                              Optional ByVal noteTecniche As String = "",
                              Optional ByVal noteTest As String = ""
                              )

            If IsNothing(cliente) Then
                Throw New ArgumentNullException("cliente non indicato")
            End If

            If IsNothing(autore) Then
                Throw New ArgumentNullException("autore non indicato")
            End If

            If IsNothing(noteTecniche) Then
                Throw New ArgumentNullException("noteTecniche non indicato")
            End If

            If IsNothing(noteTest) Then
                Throw New ArgumentNullException("noteTest non indicato")
            End If

            If Me IsNot Nothing Then

                Select Case tipoChangelog

                    Case enum_Tipo_Changelog.Bug

                        Dim newBug As New Bugfix_Obj(area, descrizione, cliente, idTicketAssistenza, noteTecniche, noteTest, idTicketSviluppo, idTicketTesting, autore)
                        Me.versioni.Last().changelog.bugfix.Add(newBug)

                    Case enum_Tipo_Changelog.Feature

                        Dim newFeat As New Feature_Obj(area, descrizione, cliente, idTicketAssistenza, noteTecniche, noteTest, idTicketSviluppo, idTicketTesting, autore)
                        Me.versioni.Last().changelog.feature.Add(newFeat)

                    Case enum_Tipo_Changelog.Security

                        Dim newSec As New Security_Obj(area, descrizione, cliente, idTicketAssistenza, noteTecniche, noteTest, idTicketSviluppo, idTicketTesting, autore)
                        Me.versioni.Last().changelog.security.Add(newSec)

                    Case enum_Tipo_Changelog.Performance

                        Dim newPerf As New Performance_Obj(area, descrizione, cliente, idTicketAssistenza, noteTecniche, noteTest, idTicketSviluppo, idTicketTesting, autore)
                        Me.versioni.Last().changelog.performance.Add(newPerf)

                End Select

            End If
        End Sub
End Class

Public Class Versione_Obj
    Public Property data As String
    Public Property versione As String
    Public Property requisiti As List(Of Requisito_Obj)
    Public Property changelog As Changelog_Versione_Obj

    Public Sub New()
        Me.data = ""
        Me.versione = ""
        Me.requisiti = New List(Of Requisito_Obj)
        Me.changelog = New Changelog_Versione_Obj()
    End Sub

    Public Sub New(ByVal data As String, Optional ByVal versione As String = "")
        Me.data = Fixes.FixData(data)

        If String.IsNullOrEmpty(versione) Then
            Me.versione = me.data
        Else
            Me.versione = versione
        End If

        Me.requisiti = New List(Of Requisito_Obj)
        Me.changelog = New Changelog_Versione_Obj()
    End Sub
End Class

Public Class Requisito_Obj
    Public Property prodotto As String

    <JsonProperty(NullValueHandling:=NullValueHandling.Include, DefaultValueHandling:=DefaultValueHandling.Include)>
    Public Property versione As String
    Public Property motivo As String

    Public Sub New()
        Me.prodotto = ""
        Me.motivo = ""
    End Sub

    Public Sub New(ByVal Titolo As String, ByVal Testo As String, Optional ByVal Ver As String = "")
        Me.prodotto = Fixes.FixProdottoRequisito(Titolo)
        Me.motivo = Fixes.FixDescrizione(Testo)

        If Ver <> "" Then
            Me.versione = Ver
        End If
    End Sub
End Class

Public Class Changelog_Versione_Obj
    Public Property feature As List(Of Feature_Obj)

    <JsonProperty(NullValueHandling:=NullValueHandling.Include, DefaultValueHandling:=DefaultValueHandling.Include)>
    Public Property bugfix As List(Of Bugfix_Obj)

    <JsonProperty(NullValueHandling:=NullValueHandling.Include, DefaultValueHandling:=DefaultValueHandling.Include)>
    Public Property security As List(Of Security_Obj)

    <JsonProperty(NullValueHandling:=NullValueHandling.Include, DefaultValueHandling:=DefaultValueHandling.Include)>
    Public Property performance As List(Of Performance_Obj)

    Public Sub New()
        Me.feature = New List(Of Feature_Obj)
        Me.bugfix = New List(Of Bugfix_Obj)
        Me.security = New List(Of Security_Obj)
        Me.performance = New List(Of Performance_Obj)
    End Sub

    Public Sub SpostaBugs()
        Dim listFeatures As List(Of Feature_Obj) = TrovaBugs()

        For Each feat In listFeatures.Where(Function(x) Not x.descrizione.Contains("- ") OrElse x.descrizione.Count(Function(y) y = "-") <= 1)
            Dim newBug As New Bugfix_Obj(feat.area, feat.descrizione, feat.cliente, feat.idTicketAssistenza, feat.noteTecniche, feat.noteTest, feat.idTicketSviluppo, feat.idTicketTesting, feat.autore)
            Me.bugfix.Add(newBug)
            Me.feature.Remove(feat)
        Next
    End Sub


    Public Function TrovaBugs() As List(Of Feature_Obj)
        Return Me.feature.FindAll(Function(x) ExactMatch(x.descrizione, {"Bug", "Fix", "BugFix", "Baco"}))
    End Function

    Private Shared Function ExactMatch(ByVal input As String, ByVal match As String) As Boolean
        Return Regex.IsMatch(input, String.Format("\b{0}\b", Regex.Escape(match)), RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3))
    End Function

    Private Shared Function ExactMatch(ByVal input As String, ByVal matches As IList(Of String)) As Boolean
        Dim pattern As String = String.Join("|", matches.Select(Function(x) String.Format("\b{0}\b", Regex.Escape(x))))
        Return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3))
    End Function
End Class

Public Class Change_Obj_Base

    Public Property area As String
    Public Property descrizione As String

    Public Property cliente As String

    <Obsolete("Sostituito da idTicketAssistenza + idTicketSviluppo + idTicketTesting")>
    Public Property idPerforma As Integer

    Public Property idTicketAssistenza As Integer
    Public Property idTicketSviluppo As Integer
    Public Property idTicketTesting As Integer

    Public Property autore As String

    <JsonProperty(DefaultValueHandling:=DefaultValueHandling.IgnoreAndPopulate)>
    Public Property noteTecniche As String

    <JsonProperty(DefaultValueHandling:=DefaultValueHandling.IgnoreAndPopulate)>
    Public Property noteTest As String

    <JsonIgnore()>
    Public ReadOnly Property hasTicket As Boolean
        Get
            If idTicketAssistenza > 0 OrElse idTicketSviluppo > 0 OrElse idTicketTesting > 0 OrElse idPerforma <> 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    <JsonIgnore()>
    Public ReadOnly Property hasNote As Boolean
        Get
            If Not String.IsNullOrEmpty(noteTecniche) OrElse Not String.IsNullOrEmpty(noteTest) OrElse Not String.IsNullOrEmpty(autore) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property


    Public Sub New()
        Me.area = ""
        Me.descrizione = ""

        Me.cliente = ""
        Me.idPerforma = 0

        Me.idTicketAssistenza = 0
        Me.idTicketSviluppo = 0
        Me.idTicketTesting = 0

        Me.autore = ""

        Me.noteTecniche = ""
        Me.noteTest = ""
    End Sub

    Public Sub New(ByVal area As String,
                   ByVal descrizione As String,
                   Optional ByVal cliente As String = "",
                   Optional ByVal idTicketAssistenza As Integer = 0,
                   Optional ByVal noteTecniche As String = "",
                   Optional ByVal noteTest As String = "",
                   Optional ByVal idTicketSviluppo As Integer = 0,
                   Optional ByVal idTicketTesting As Integer = 0,
                   Optional ByVal autore As String = "")

        Me.area = Fixes.FixArea(area)
        Me.descrizione = Fixes.FixDescrizione(descrizione)

        Me.cliente = cliente

        Me.idTicketAssistenza = idTicketAssistenza

        Me.idTicketSviluppo = idTicketSviluppo
        Me.idTicketTesting = idTicketTesting

        Me.autore = autore

        Me.noteTecniche = Fixes.FixDescrizione(noteTecniche)
        Me.noteTest = Fixes.FixDescrizione(noteTest)
    End Sub

End Class

Public Class Feature_Obj
    Inherits Change_Obj_Base

    Public Sub New()

        MyBase.New()
        
    End Sub

    Public Sub New(ByVal IS_OLD As Boolean,
                   ByVal Titolo As String,
                   ByVal Testo As String,
                   Optional ByVal cliente As String = "",
                   Optional ByVal idPerforma As Integer = 0,
                   Optional ByVal noteTecniche As String = "",
                   Optional ByVal noteTest As String = "")

        'IS_OLD è un parametro che non viene utilizzato, ma mi serve solo per mantenere la retrocompatibilità con l'idperforma

        Me.area = Fixes.FixArea(Titolo)
        Me.descrizione = Fixes.FixDescrizione(Testo)

        Me.cliente = cliente

        Me.idPerforma = idPerforma

        Me.noteTecniche = Fixes.FixDescrizione(noteTecniche)
        Me.noteTest = Fixes.FixDescrizione(noteTest)

    End Sub

    Public Sub New(ByVal area As String,
                   ByVal descrizione As String,
                   Optional ByVal cliente As String = "",
                   Optional ByVal idTicketAssistenza As Integer = 0,
                   Optional ByVal noteTecniche As String = "",
                   Optional ByVal noteTest As String = "",
                   Optional ByVal idTicketSviluppo As Integer = 0,
                   Optional ByVal idTicketTesting As Integer = 0,
                   Optional ByVal autore As String = "")

        MyBase.New(area, descrizione, cliente, idTicketAssistenza,
                   noteTecniche, noteTest,
                   idTicketSviluppo, idTicketTesting,
                   autore)

    End Sub

End Class

Public Class Bugfix_Obj
    Inherits Change_Obj_Base

    Public Sub New()

        MyBase.New()

    End Sub

    Public Sub New(ByVal IS_OLD As Boolean,
                   ByVal Titolo As String,
                   ByVal Testo As String,
                   Optional ByVal cliente As String = "",
                   Optional ByVal idPerforma As Integer = 0,
                   Optional ByVal noteTecniche As String = "",
                   Optional ByVal noteTest As String = "")

        'IS_OLD è un parametro che non viene utilizzato, ma mi serve solo per mantenere la retrocompatibilità con l'idperforma

        Me.area = Fixes.FixArea(Titolo)
        Me.descrizione = Fixes.FixDescrizione(Testo)

        Me.cliente = cliente

        Me.idPerforma = idPerforma

        Me.noteTecniche = Fixes.FixDescrizione(noteTecniche)
        Me.noteTest = Fixes.FixDescrizione(noteTest)

    End Sub

    Public Sub New(ByVal area As String,
                   ByVal descrizione As String,
                   Optional ByVal cliente As String = "",
                   Optional ByVal idTicketAssistenza As Integer = 0,
                   Optional ByVal noteTecniche As String = "",
                   Optional ByVal noteTest As String = "",
                   Optional ByVal idTicketSviluppo As Integer = 0,
                   Optional ByVal idTicketTesting As Integer = 0,
                   Optional ByVal autore As String = "")

        MyBase.New(area, descrizione, cliente, idTicketAssistenza,
                   noteTecniche, noteTest,
                   idTicketSviluppo, idTicketTesting,
                   autore)

    End Sub

End Class

Public Class Security_Obj
    Inherits Change_Obj_Base

    Public Sub New()

        MyBase.New()

    End Sub

    Public Sub New(ByVal area As String,
                   ByVal descrizione As String,
                   Optional ByVal cliente As String = "",
                   Optional ByVal idTicketAssistenza As Integer = 0,
                   Optional ByVal noteTecniche As String = "",
                   Optional ByVal noteTest As String = "",
                   Optional ByVal idTicketSviluppo As Integer = 0,
                   Optional ByVal idTicketTesting As Integer = 0,
                   Optional ByVal autore As String = "")

        MyBase.New(area, descrizione, cliente, idTicketAssistenza,
                   noteTecniche, noteTest,
                   idTicketSviluppo, idTicketTesting,
                   autore)

    End Sub

End Class

Public Class Performance_Obj
    Inherits Change_Obj_Base

    Public Sub New()

        MyBase.New()

    End Sub

    Public Sub New(ByVal area As String,
                   ByVal descrizione As String,
                   Optional ByVal cliente As String = "",
                   Optional ByVal idTicketAssistenza As Integer = 0,
                   Optional ByVal noteTecniche As String = "",
                   Optional ByVal noteTest As String = "",
                   Optional ByVal idTicketSviluppo As Integer = 0,
                   Optional ByVal idTicketTesting As Integer = 0,
                   Optional ByVal autore As String = "")

        MyBase.New(area, descrizione, cliente, idTicketAssistenza,
                   noteTecniche, noteTest,
                   idTicketSviluppo, idTicketTesting,
                   autore)

    End Sub

End Class
