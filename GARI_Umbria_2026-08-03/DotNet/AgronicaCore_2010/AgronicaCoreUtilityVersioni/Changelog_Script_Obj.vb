Imports System.IO
Imports System.Text.RegularExpressions
Imports AgronicaCoreUtilityVersioni.Enumerativi
Imports Newtonsoft.Json

Public Class Changelog_Script_Obj

    Const SCHEMA_JSON As String = "../../schema/changelogScript.schema.json"

    <JsonProperty(PropertyName:="$schema")>
    Public Property schema As String
    Public Property progetto As String
    Public Property versioni As List(Of Versione_Script_Obj)

    <JsonIgnore()>
    Public Property lastVersion As Integer = 0
    <JsonIgnore()>
    Public Property lastVersionDate As String = ""
    <JsonIgnore()>
    Public Property hasUnreleased As Boolean = False
    <JsonIgnore()>
    Public Property giasVersioneCorrenteTxt As String = ""

    Public Sub New()
        Me.schema = SCHEMA_JSON
        Me.progetto = ""
        Me.versioni = New List(Of Versione_Script_Obj)
    End Sub

    Public Sub New(ByVal projectName As String, Optional ByVal schema As String = "")
        Me.schema = If(Not String.IsNullOrEmpty(schema), schema, SCHEMA_JSON)
        Me.progetto = projectName
        Me.versioni = New List(Of Versione_Script_Obj)
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
        Dim last As Versione_Script_Obj = Nothing
        Dim secondToLast As Versione_Script_Obj = Nothing

        If Me.versioni IsNot Nothing AndAlso Me.versioni.Count > 0 Then
            last = Me.versioni.FirstOrDefault()

            If last IsNot Nothing AndAlso Not String.IsNullOrEmpty(last.data) Then
                If ListPossibleUnreleased.Exists(Function(s) last.data.ToLower().Contains(s)) Then
                    Me.hasUnreleased = True
                    If Me.versioni.Count > 1 Then
                        secondToLast = Me.versioni.Skip(1).FirstOrDefault()
                    End If
                End If

                Me.lastVersion = If(secondToLast IsNot Nothing, secondToLast.versione, last.versione)
                Me.lastVersionDate = If(secondToLast IsNot Nothing, secondToLast.data, last.data)

                Me.giasVersioneCorrenteTxt = Me.lastVersion
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

    Public Function ScriviJson(ByVal pathFileName As String) As Boolean
        Const nomeRoutine = "ScriviJson"
        Dim result As Boolean = False
        Try

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

    Public Shared Function DeSerializzaJson(ByVal jsonString As String) As Changelog_Script_Obj
        Return JsonConvert.DeserializeObject(Of Changelog_Script_Obj)(jsonString)
    End Function

    Public Shared Function LeggiDaFileJson(ByVal pathFileName As String) As Changelog_Script_Obj
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

End Class

Public Class Versione_Script_Obj
    Public Property versione As Integer
    Public Property data As String
    Public Property changelog As Changelog_Versione_Script_Obj

    Public Sub New()
        Me.versione = 0
        Me.data = ""
        Me.changelog = New Changelog_Versione_Script_Obj()
    End Sub

    Public Sub New(ByVal DataAggiornamento As String)
        Me.versione = 0
        Me.data = Fixes.FixData(DataAggiornamento)
        Me.changelog = New Changelog_Versione_Script_Obj()
    End Sub
End Class

Public Class Changelog_Versione_Script_Obj

    Public Property gias_super_server As List(Of Script_Detail_Obj)
    Public Property server As List(Of Script_Detail_Obj)

    '<JsonProperty(NullValueHandling:=NullValueHandling.Include, DefaultValueHandling:=DefaultValueHandling.Include)>
    Public Property utenti As List(Of Script_Detail_Obj)
    Public Property metaschema As List(Of Script_Detail_Obj)
    Public Property piano_concimazione_pua As List(Of Script_Detail_Obj)


    Public Sub New()
        Me.gias_super_server = New List(Of Script_Detail_Obj)
        Me.server = New List(Of Script_Detail_Obj)
        Me.utenti = New List(Of Script_Detail_Obj)
        Me.piano_concimazione_pua = New List(Of Script_Detail_Obj)
    End Sub

End Class


Public Class Script_Detail_Obj
    Public Property area As String
    Public Property descrizione As String

    Public Sub New()
        Me.area = ""
        Me.descrizione = ""
    End Sub

    Public Sub New(ByVal titolo As String, ByVal testo As String)
        Me.area = Fixes.FixArea(titolo)
        Me.descrizione = Fixes.FixDescrizione(testo)
    End Sub
End Class

