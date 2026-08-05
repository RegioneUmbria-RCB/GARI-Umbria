Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Data.Common
Imports Microsoft.SqlServer
Imports System.Web
Imports System.Configuration

Public Class ComunicazioneGiasOnLine_Satelliti
    Inherits AgronicaCoreDataProvider.DataProvider


    Private _path2Ini As String
    Private _cnServer As String

    'per superserver
    Private _PivaSuperServer As String 'non serve, solo per differenziare il costruttore
    Private _StringaConnessione_Super_Server As String

    Public Sub New(ByVal PivaSuperServer As String, ByVal StringaConnessione_Super_Server As String, ByVal Id_DB_Server As String)
        _path2Ini = "" 'con superserver non deve esistere
        _PivaSuperServer = PivaSuperServer 'non serve, solo per differenziare il costruttore
        _StringaConnessione_Super_Server = StringaConnessione_Super_Server
        _cnServer = Id_DB_Server
    End Sub

    Public Sub New(ByVal path2FileIni As String, ByVal keyIniConnection As String)
        _path2Ini = path2FileIni 'imposto il path per l'ini
        _cnServer = keyIniConnection
    End Sub
    Public Sub New(ByVal Id_Sito As String)
        _path2Ini = CostantiPersonalizzate.PathFileINI
        _cnServer = Id_Sito
    End Sub
    Public Sub New()

    End Sub


    'ritorna la stringa o il datatable
    'Public Function LeggiParametriGias_(ByVal unid2read As String, ByVal delete_after_read As Boolean, Optional ByRef Stringa_JSon As String = "") As String

    '    Dim NomeRoutine As String = "AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti.LeggiParametriGias"
    '    Dim MessaggioErrore As String = ""
    '    Dim retParam As String = ""
    '    Dim retJSonString As String = ""
    '    Dim reader As DbDataReader
    '    Dim oleCon As DbConnection
    '    Dim strSql As String = ""
    '    Dim nomeParametro As String = ""

    '    Try

    '        Dim stringaConnessione As String = GetStringaConnessione()
    '        oleCon = DataProviderFactory.Instance.CreaNuovaConnessione(stringaConnessione)

    '        If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.OleDbProvider Then
    '            strSql = "SELECT stringa_parametri, Stringa_JSon FROM Web_Parametri WHERE Unid=?"
    '            nomeParametro = "?"
    '        Else
    '            strSql = "SELECT stringa_parametri, Stringa_JSon FROM Web_Parametri WHERE Unid=@"
    '            nomeParametro = "@"
    '        End If

    '        Dim cmdOle As DbCommand = DataProviderFactory.Instance.CreaCommand(strSql, oleCon)
    '        'aggiungo il parametro
    '        cmdOle.Parameters.Add(DataProviderFactory.Instance.CreaParameter(nomeParametro, unid2read))
    '        oleCon.Open()
    '        'Try

    '        ' Scattolin -   Scelta questa strada per gestire il nuovo campo JSon perchè diversamente
    '        '               sarebbe necessario aggiornare il migra per tutti i progetti
    '        Try
    '            reader = cmdOle.ExecuteReader()
    '            reader.Read()
    '            If Not reader Is Nothing And reader.HasRows() Then
    '                retParam = reader.GetValue(0).ToString()
    '                Stringa_JSon = reader.GetValue(1).ToString()
    '            End If
    '        Catch ex As Exception
    '            If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.OleDbProvider Then
    '                strSql = "SELECT stringa_parametri FROM Web_Parametri WHERE Unid=?"
    '            Else
    '                strSql = "SELECT stringa_parametri FROM Web_Parametri WHERE Unid=@"
    '            End If
    '            cmdOle = DataProviderFactory.Instance.CreaCommand(strSql, oleCon)
    '            'aggiungo il parametro
    '            cmdOle.Parameters.Add(DataProviderFactory.Instance.CreaParameter(nomeParametro, unid2read))
    '            reader = cmdOle.ExecuteReader()
    '            reader.Read()
    '            If Not reader Is Nothing And reader.HasRows() Then
    '                retParam = reader.GetValue(0).ToString()
    '            End If
    '        End Try

    '        If retParam = "" Then
    '            Throw New Exception("Non è stata recuperata la stringa xml dalla tabella Web_Parametri.")
    '        End If
    '        'eventualmente cancello il record
    '        If (delete_after_read) = True Then
    '            CancellaParametriGias(unid2read)
    '        End If
    '        'Catch ex As Exception
    '        '    MyBase.Scrivi_LOG("", "", "unknown", "LeggiParametriGias", ex.Message)
    '        'Finally
    '        '    oleCon.Close()
    '        'End Try

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    Finally
    '        If Not IsNothing(oleCon) Then
    '            oleCon.Close()
    '        End If
    '    End Try

    '    Return retParam 'ritorno i parametri...

    'End Function

    Public Function CancellaParametriGias(ByVal unid2delete As String, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim retVal As Integer
        Dim strSql As String = ""
        Dim nomeParametro As String = ""
        Dim stringaConnessione As String = objParametri.StringaConnessione

        If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.OleDbProvider Then
            strSql = "DELETE FROM Web_Parametri WHERE Unid=?"
            nomeParametro = "?"
        Else
            strSql = "DELETE FROM Web_Parametri WHERE Unid=@"
            nomeParametro = "@"
        End If

        Dim oleCon As DbConnection = DataProviderFactory.Instance.CreaNuovaConnessione(stringaConnessione)
        Dim cmdOle As DbCommand = DataProviderFactory.Instance.CreaCommand(strSql, oleCon)
        'aggiungo il parametro
        cmdOle.Parameters.Add(DataProviderFactory.Instance.CreaParameter(nomeParametro, unid2delete))
        oleCon.Open()
        Try
            retVal = cmdOle.ExecuteNonQuery()
        Catch ex As Exception
            MyBase.Scrivi_LOG(objParametri, "CancellaParametriGias", ex.Message)
        Finally
            oleCon.Close()
        End Try

        Return retVal 'ritorno i parametri...

    End Function

    Public Function LeggiParametriGias(ByVal unid2read As String,
                                       ByVal delete_after_read As Boolean,
                                       Optional ByRef Stringa_JSon As String = "") As String

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti.LeggiParametriGias"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim retParam As String = ""
        Dim objParametri As AgronicaCoreParametri = GetObjParametri()

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("SELECT * FROM Web_Parametri WHERE Unid = '" & Agro_SQL_SaveText(unid2read) & "' ")

            Dim DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

                If DT.Columns.Contains("stringa_parametri") Then

                    retParam = DT.Rows(0)("stringa_parametri").ToString()

                End If
                If DT.Columns.Contains("Stringa_JSon") Then

                    Stringa_JSon = DT.Rows(0)("Stringa_JSon").ToString()

                End If

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return ""

        End Try

        If retParam = "" Then
            Throw New Exception("Non è stata recuperata la stringa xml dalla tabella Web_Parametri.")
        End If

        'eventualmente cancello il record
        If (delete_after_read) = True Then
            CancellaParametriGias(unid2read, objParametri)
        End If

        Return retParam 'ritorno i parametri...

    End Function

    Public Function LeggiParametriGias(ByVal unid2read As String,
                                       ByVal delete_after_read As Boolean,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional ByRef Stringa_JSon As String = "") As String

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti.LeggiParametriGias"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim retParam As String = ""

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            StrSQL.Append("SELECT * FROM Web_Parametri WHERE Unid = '" & Agro_SQL_SaveText(unid2read) & "' ")

            Dim DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

                If DT.Columns.Contains("stringa_parametri") Then

                    retParam = DT.Rows(0)("stringa_parametri").ToString()

                End If
                If DT.Columns.Contains("Stringa_JSon") Then

                    Stringa_JSon = DT.Rows(0)("Stringa_JSon").ToString()

                End If

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return ""

        End Try

        If retParam = "" Then
            Throw New Exception("Non è stata recuperata la stringa xml dalla tabella Web_Parametri.")
        End If

        'eventualmente cancello il record
        If (delete_after_read) = True Then
            CancellaParametriGias(unid2read, objParametri)
        End If

        Return retParam 'ritorno i parametri...

    End Function

    Public Function PulisciParametriGias(ByRef objParametri As AgronicaCoreParametri, Optional ByVal num_param As Integer = 100) As Boolean

        Dim StrSQL As String
        Dim xRisp As Boolean = True
        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "ComunicazioneGiasOnLine_Satelliti.PulisciParametriGias"

        Try

            StrSQL = "SELECT TOP " & num_param & " Unid FROM Web_Parametri WHERE Data < GetDate() -1"
            Dim DT = EseguiQuery_Lettura(objParametri, StrSQL, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                For Each row In DT.Rows
                    StrSQL = "DELETE FROM Web_Parametri WHERE Unid='" & row.Item("Unid") & "'"
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL, NomeRoutine)
                Next
            End If

        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ScriviParametriGias(ByVal Stringa_Parametri As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal Stringa_JSon As String = "") As String

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "ComunicazioneGiasOnLine_Satelliti.ScriviParametriGias"
        Dim Unid As String
        Try



            Unid = System.Guid.NewGuid.ToString

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Web_Parametri ")
            StrSQL.Append("  (Unid, Stringa_Parametri, Data, Stringa_JSon) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Unid) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Stringa_Parametri) & "'  ")
            StrSQL.Append("         ," & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Stringa_JSon) & "')  ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return ""
        End Try

        Return Unid

    End Function

    Private Function GetStringaConnessione() As String
        Dim stringaconnessione As String

        'se ho la _StringaConnessione_Server allora non uso l'ini, che solo per versione vecchia
        If _StringaConnessione_Super_Server <> "" Then
            Dim connessioni As New AgronicaCoreDataProvider.Connessioni
            Dim objParametriHLP As New AgronicaCoreParametri_Helper
            Dim objparametrisuperserver As AgronicaCoreParametri = objParametriHLP.Crea_ObjParametri(New DateTime(&H851055320574000), New DateTime(&H9325D82E8380000),
                                                                        AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica,
                                                                        AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti,
                                                                        "", "", "", "", "", "", _StringaConnessione_Super_Server)

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DEBUG_LogUMAObj")) Then
                Dim filePath As String = HttpContext.Current.Server.MapPath("~/DebugObjParamUMA.txt")
                Using fstr As IO.FileStream = IO.File.Open(filePath, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
                    Dim sw As New IO.StreamWriter(fstr)
                    sw.WriteLine("---------- ComunicazioneGiasOnLine_Satelliti: " + If(IsNothing(objparametrisuperserver), "nope", objparametrisuperserver.ToString))
                    sw.Flush()
                    sw.Dispose()
                End Using
            End If
            stringaconnessione = Utility_Sicurezza.Leggi_Stringa_Connessione(CInt(_cnServer), objparametrisuperserver)
            'stringaconnessione = connessioni.Leggi_Stringa_Connessione(CInt(_cnServer), objparametrisuperserver)
        Else
            'versione con file ini
            stringaconnessione = (MyBase.FindConnessione_Su_Ini_O_Superserver(_path2Ini, _cnServer))
        End If
        Return stringaconnessione
    End Function

    Private Function GetObjParametri() As AgronicaCoreParametri

        Dim objParametriHLP As New AgronicaCoreParametri_Helper
        Dim stringaconnessione As String = GetStringaConnessione()
        Dim objparametri As AgronicaCoreParametri = objParametriHLP.Crea_ObjParametri(New DateTime(&H851055320574000), New DateTime(&H9325D82E8380000),
                                                                        AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica,
                                                                        AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti,
                                                                        "", "", "", "", "", "", stringaconnessione)
        Return objparametri
    End Function



End Class