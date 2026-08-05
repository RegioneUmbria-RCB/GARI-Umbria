Imports System.Data.Common

Public Interface IReadDataProvider

    Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String, Optional chiamaScriviLog As Boolean = True) As DataTable
    Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String, ByRef DataSet2Fill As DataSet, strNomeDtNelDS As String) As Boolean
    Function EseguiQuery_Lettura(ByRef objConnessione As DbConnection, ByRef objTransazione As DbTransaction, StringaConnessione As String, StringaSQL As String, DirectoryLOG As String, FileLOG As String, IdentificatoreUtente As String, NomeRoutine As String) As DataTable
    Function EseguiQuery_Lettura(ByRef objConnessione As DbConnection, StringaConnessione As String, StringaSQL As String, DirectoryLOG As String, FileLOG As String, IdentificatoreUtente As String, NomeRoutine As String, Optional objTransazione As DbTransaction = Nothing) As DataTable
    Function EseguiQuery_Lettura(ByRef StringaConnessione As String, StringaSQL As String, NomeRoutine As String) As DataTable
    Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, ByVal parametriCommand As Dictionary(Of String, Object), NomeRoutine As String) As DataTable

    Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, ByVal parametriCommand As List(Of DbParameter), NomeRoutine As String) As DataTable
    Function EseguiQuery_Lettura_XML(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String) As String
    Function EseguiQuery_Lettura_jSon(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String) As String


End Interface
