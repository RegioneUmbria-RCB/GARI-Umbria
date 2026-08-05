Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Threading

Imports AgronicaExcel_DataProvider.DataObjectsHelpers

Public Class ImportazioneDaExcel

    'Private _StringaConnessione As String
    ' ''' <summary>
    ' ''' Stringa di connessione nel database di destinazione dell'importazione
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property StringaConnessione() As String
    '    Get
    '        Return _StringaConnessione
    '    End Get
    '    Set(ByVal value As String)
    '        _StringaConnessione = value
    '    End Set
    'End Property

    '###########################################
    Public Function FillTableFromExcel(ByVal queryExcel As String, _
                                        ByVal ConnectionStringExcel As String) As DataTable

        Dim dt As New DataTable

        Dim objsourceConn As OleDbConnection = New OleDbConnection(ConnectionStringExcel)

        objsourceConn.Open()

        Dim objCmdSelect As OleDbCommand = New OleDbCommand(queryExcel, objsourceConn)

        Dim readerPerDt As OleDbDataReader = objCmdSelect.ExecuteReader()

        Dim converti As New CustomAdapter
        converti.FillFromReader(dt, readerPerDt)

        objsourceConn.Close()

        Return dt

    End Function

   




End Class
