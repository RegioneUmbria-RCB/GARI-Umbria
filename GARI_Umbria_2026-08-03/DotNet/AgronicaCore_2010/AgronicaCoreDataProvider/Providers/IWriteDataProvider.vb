Imports System.Data.Common

Public Interface IWriteDataProvider

    Function EseguiQuery_InsertParametrizzata(ByRef objParametri As AgronicaCoreParametri, Nome_Tabella As String, strCampi As String, strValori As String) As Boolean
    Function EseguiQuery_Scrittura(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String) As Boolean
    Function EseguiQuery_Scrittura(ByRef objConnessione As DbConnection, ByRef objTransazione As DbTransaction, StringaConnessione As String, StringaSQL As String, DirectoryLOG As String, FileLOG As String, IdentificatoreUtente As String, NomeRoutine As String) As Boolean
    Function EseguiQuery_Scrittura(ByRef stringaConnessione As String, StringaSQL As String, NomeRoutine As String) As Boolean
    Function EseguiQuery_Scrittura_ParamVarBinary(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String, CmdParameters As Dictionary(Of String, Byte())) As Boolean
    Function EseguiQuery_ScritturaNum(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String, ByRef NumeroRecordInteressati As Integer) As Boolean

    Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String, parametriCommand As List(Of DbParameter)) As Boolean
    Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri, ByRef objConnessione As DbConnection, ByRef objTransazione As DbTransaction, StringaConnessione As String, StringaSQL As String, NomeRoutine As String, parametriCommand As List(Of DbParameter)) As Boolean

End Interface
