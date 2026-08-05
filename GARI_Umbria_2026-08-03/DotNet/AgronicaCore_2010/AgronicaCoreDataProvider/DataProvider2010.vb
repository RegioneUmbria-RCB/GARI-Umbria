Imports System.Data
'Imports System.data.SqlClient
Imports System.Data.OleDb
Imports System.Web
Imports System.Configuration
Imports System.IO
Imports System.Data.SqlClient

Public Class DataProvider2010
    Inherits DataProvider

    Public Sub New()

    End Sub

    '##############################################################################################
    Public Function EseguiQuery_Lettura_XDoc(
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As XDocument

        Dim NomeRoutineLocale As String = "EseguiQuery_Lettura_xmlReader"

        Dim MessaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        'Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim DT As New XDocument


        Dim rdrOut As Xml.XmlReader

        Dim conn As New SqlClient.SqlConnection
        conn.ConnectionString = getConnectionStringFromOleToSql(objParametri.StringaConnessione)

        If conn.State = ConnectionState.Closed Then
            conn.Open()
        End If

        Try

            Dim cmd As New SqlClient.SqlCommand()
            cmd.Connection = conn

            'Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
            'Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin
            'Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            'If Parametrizza(StringaSQL, parameters) Then
            '    For Each p As SqlParameter In parameters
            '        xCommand.Parameters.Add(p)
            '    Next
            'End If
            cmd.CommandText = StringaSQL.ToOrigin(objParametri)

            cmd.CommandTimeout = 1200
            rdrOut = cmd.ExecuteXmlReader

            Try
                DT = XDocument.Load(rdrOut, LoadOptions.None)
            Catch ex As Exception
                DT = Nothing
            End Try


            conn.Close()

        Catch ex As Exception
            conn.Close()
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine & "." & NomeRoutineLocale, MessaggioErrore)
            DT = Nothing
            Throw New Exception(MessaggioErrore)

        Finally


        End Try

        Return DT

    End Function


    Private Function getConnectionStringFromOleToSql(ByVal sConnessioneOle As String) As String

        Dim separator(1) As Char
        separator(0) = ";"c
        separator(1) = "="c

        Dim rval As String = ""

        Dim DaTenere As String = "server,data source,initial catalog,user id,password"

        Dim arrayFromString() As String = sConnessioneOle.Split(separator)
        For i As Integer = 0 To arrayFromString.Length - 2 Step 2
            If DaTenere.IndexOf(arrayFromString(i).ToLower, 0) > -1 Then
                rval &= arrayFromString(i) & "=" & arrayFromString(i + 1) & ";"
            End If
        Next

        Return rval
    End Function

    'Private Function Parametrizza(ByRef StringaSql As String, ByRef parametriOutput As List(Of SqlParameter)) As Boolean

    '    Dim p As New Parametrizzatore()
    '    parametriOutput = New List(Of SqlParameter)
    '    p.Parametrizza(StringaSql, _parametri, parametriOutput)

    '    Return True

    'End Function

End Class