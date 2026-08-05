Imports System.Data.Common

Public Interface IDataProvider
    Inherits IReadDataProvider, IWriteDataProvider

    Property ByPassaLog As Boolean
    Property SqlDiretto As Boolean

    Function ApriConnessione(ByRef objParametri As AgronicaCoreParametri, ByRef strErr As String) As DbConnection
    Function ConnectToAccess(DataSource As String) As DbConnection
    Function FindConnessione_Su_Ini_O_Superserver(strPath2Ini As String, strKey As String, Optional objParametriSuperServer As AgronicaCoreParametri = Nothing) As String
    Function FindIniConnessioni(strPath2Ini As String, strKey As String) As String
    Function NomeDataBase_FromConnessione_ATTENZIONE_NON_USARE_PER_COMPATIBILITA_SUPERSERVER(strPathxFileIni As String, strKey As String, Optional strNomeAttributoDB As String = "DbParam") As String
    Function NomeDataBase_FromStringaConnessione(StringaConnessione As String) As String

    Function NomeIstanza_FromStringaConnessione(stringaConnessione As String) As String

    Function Scrivi_Dati_SuAccess(DataSource As String, StrSQL As String, ByRef strErr As String) As Object
    <Obsolete("Usare il metodo VersioneSqlServer_Major")>
    Function VersioneSqlServer_anno(objParametri As AgronicaCoreParametri) As Integer
    Function VersioneSqlServer_Major(objParametri As AgronicaCoreParametri) As Integer

    Function LivelloCompatibilita(objParametri As AgronicaCoreParametri) As Integer

End Interface
