Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.UtilityProvider

Public Enum StatoMVV_Gias
    NonDefinito = 0
    InLock = 1
    XMLNonGeneratoPerErrori = 300
    XMLNonGenerabile = 301
    XMLGenerato = 400
    XMLEsportato = 401
    XMLDaRigenerare = 402
    InviatoMVV_KO = 403
    InviatoMVV_OK = 500
End Enum

Public Enum StatoMVV_Sian
    NonDefinito = 0
    Bozza = 1
    Pesentato = 2
    Validato = 3
    NonValidato = 4
    Annullato = 5
    Accettato = 6
    Respinto = 7
    AccettatoConRevisione = 8
    ValidatoNoCodiceICQRF = 9
    ValidatoNonInCarico = 10
End Enum

Public Class MVV_Log_Helper

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _logR As MVV_Log_R
    Private ReadOnly _logW As MVV_Log_W
    Private ReadOnly _tracciato As String

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri, Optional ByVal tracciato As String = "")
        _objParametriServer = objParametriServer

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)

        Dim giasContext As New Gias_DeveloperServer_Entities(efConnString)

        _logR = New MVV_Log_R(objParametriServer)
        _logW = New MVV_Log_W(objParametriServer)
        _tracciato = tracciato

    End Sub

    Public Function ScriviLog(ByVal Piva As String,
                            ByVal Id_Agenda As Integer,
                            ByVal NumMVV As String,
                            ByVal MarcaTemp As String,
                            ByVal DataGenXML As Date,
                            ByVal NomeFileXML As String,
                            ByVal NomeFilePDF As String,
                            ByVal Gias_Status As Integer,
                            ByVal MVV_Status As Integer
                            ) As Integer


        Dim esito As Boolean
        Dim dt = _logR.LeggiMVVLog(Piva, Id_Agenda)

        If dt.Rows.Count > 0 Then
            esito = _logW.ModificaMVVLog(Piva, Id_Agenda, NumMVV, MarcaTemp, DataGenXML, NomeFileXML, NomeFilePDF, Gias_Status, MVV_Status, _tracciato)
        Else
            esito = _logW.ScriviMVVLog(Piva, Id_Agenda, NumMVV, MarcaTemp, DataGenXML, NomeFileXML, NomeFilePDF, Gias_Status, MVV_Status, _tracciato)
        End If

        Return esito

    End Function

    Public Function AggiornaLog(
                            ByVal Piva As String,
                            ByVal Id_Agenda As Integer,
                            ByVal NumMVV As String,
                            ByVal Gias_Status As Integer,
                            ByVal MVV_Status As Integer
                            ) As Integer


        Dim esito As Boolean
        Dim dt = _logR.LeggiMVVLog(Piva, Id_Agenda, NumMVV)

        If dt.Rows.Count > 0 Then
            esito = _logW.AggiornaStatoMVVLog(Piva, Id_Agenda, NumMVV, Gias_Status, MVV_Status)
        End If

        Return esito

    End Function

    Public Function ScriviLogDettaglio(ByVal Piva As String,
                            ByVal Id_Agenda As Integer,
                            ByVal NumMVV As String,
                            ByVal CodiceErrore As String,
                            ByVal TipoErrore As Integer,
                            ByVal Messaggio As String
                            ) As Integer


        Dim esito As Boolean
        Dim dt = _logR.LeggiMVVLog(Piva, Id_Agenda, NumMVV)

        If dt.Rows.Count > 0 Then
            esito = _logW.ScriviMVVLogDettaglio(CInt(dt.Rows(0).Item("Id_Log")), CodiceErrore, TipoErrore, Messaggio)
        End If

        Return esito

    End Function


End Class

Public Class MVV_Log_R : Inherits DALBase

    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Private ReadOnly _objParametriServer As AgronicaCoreParametri

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)

        _giasContext = New Gias_DeveloperServer_Entities(efConnString)

    End Sub


    Public Function LeggiMVVLog(ByVal Piva As String, ByVal Id_Agenda As Integer, Optional ByVal NumMVV As String = "") As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.LeggiMVVLog"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM MVV_Log ")
            StrSQL.AppendLine("WHERE MVV_Log.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine("AND MVV_Log.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            If Not String.IsNullOrEmpty(NumMVV) Then
                StrSQL.AppendLine("AND MVV_Log.NumMVV = '" & Agro_SQL_SaveText(NumMVV) & "' ")
            End If

            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

        Return dt

    End Function

    Public Function LeggiMVVLogDettaglio(ByVal Piva As String, ByVal Id_Agenda As Integer) As DataTable

        Dim nomeProcedura As String = "MVVElettronico_R.LeggiMVVLogDettaglio"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT Piva, Id_Agenda, MVV_Log_Dettaglio.Data_Creazione As DataLog, Messaggio, TipoErrore, CodiceErrore ")
            StrSQL.AppendLine("FROM MVV_Log ")
            StrSQL.AppendLine("INNER JOIN MVV_Log_Dettaglio On MVV_Log_Dettaglio.Id_Log = MVV_Log.Id_Log ")
            StrSQL.AppendLine("WHERE MVV_Log.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("AND MVV_Log.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            StrSQL.AppendLine("ORDER BY Id_Log_Dettaglio DESC ")

            dt = EseguiQuery_Lettura(_objParametriServer, StrSQL.ToString, nomeProcedura)

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

        Return dt

    End Function

End Class

Public Class MVV_Log_W : Inherits DALBase

    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Private ReadOnly _objParametriServer As AgronicaCoreParametri

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)

        _giasContext = New Gias_DeveloperServer_Entities(efConnString)

    End Sub

    Public Function ScriviMVVLog(ByVal Piva As String,
                            ByVal Id_Agenda As Integer,
                            ByVal NumMVV As String,
                            ByVal MarcaTemp As String,
                            ByVal DataGenXML As Date,
                            ByVal NomeFileXML As String,
                            ByVal NomeFilePDF As String,
                            ByVal Gias_Status As Integer,
                            ByVal MVV_Status As Integer,
                            ByVal TipoTracciato As String
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMVVDAL.MVVElettronico_W.ScriviLogMVV()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append("INSERT INTO MVV_Log( Piva, Id_Agenda, NumMVV, MarcaTemp, DataGenXML, NomeFileXML, NomeFilePDF, Gias_Status, MVV_Status, TipoTracciato ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(NumMVV) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(MarcaTemp) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(DataGenXML) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(NomeFileXML) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(NomeFilePDF) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Gias_Status) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(MVV_Status) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(TipoTracciato) & "' ")
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(_objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(_objParametriServer, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function ModificaMVVLog(ByVal Piva As String,
                            ByVal Id_Agenda As Integer,
                            ByVal NumMVV As String,
                            ByVal MarcaTemp As String,
                            ByVal DataGenXML As Date,
                            ByVal NomeFileXML As String,
                            ByVal NomeFilePDF As String,
                            ByVal Gias_Status As Integer,
                            ByVal MVV_Status As Integer,
                            ByVal TipoTracciato As String
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMVVDAL.MVVElettronico_W.ScriviLogMVV()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append("UPDATE MVV_Log SET ")

            StrSQL.Append(" NumMVV = '" & Agro_SQL_SaveText(NumMVV) & "',  ")
            StrSQL.Append(" MarcaTemp = '" & Agro_SQL_SaveText(MarcaTemp) & "',  ")
            StrSQL.Append(" DataGenXML = " & Agro_SQL_SaveDateTime(DataGenXML) & ", ")
            StrSQL.Append(" NomeFileXML = '" & Agro_SQL_SaveText(NomeFileXML) & "',  ")
            StrSQL.Append(" NomeFilePDF = '" & Agro_SQL_SaveText(NomeFilePDF) & "',  ")
            StrSQL.Append(" Gias_Status = " & Agro_SQL_SaveNum(Gias_Status) & ", ")
            StrSQL.Append(" MVV_Status = " & Agro_SQL_SaveNum(MVV_Status) & ", ")
            StrSQL.Append(" TipoTracciato = '" & Agro_SQL_SaveText(TipoTracciato) & "' ")

            StrSQL.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(_objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(_objParametriServer, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function AggiornaStatoMVVLog(ByVal Piva As String,
                            ByVal Id_Agenda As Integer,
                            ByVal NumMVV As String,
                            ByVal Gias_Status As Integer,
                            ByVal MVV_Status As Integer
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMVVDAL.MVVElettronico_W.ScriviLogMVV()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append("UPDATE MVV_Log SET ")

            StrSQL.Append(" Gias_Status = " & Agro_SQL_SaveNum(Gias_Status) & ", ")
            StrSQL.Append(" MVV_Status = " & Agro_SQL_SaveNum(MVV_Status) & " ")

            StrSQL.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("AND NumMVV = '" & Agro_SQL_SaveText(NumMVV) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(_objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(_objParametriServer, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function ScriviMVVLogDettaglio(
                            ByVal Id_Log As Integer,
                            ByVal CodiceErrore As String,
                            ByVal TipoErrore As Integer,
                            ByVal Messaggio As String
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMVVDAL.MVVElettronico_W.ScriviMVVLogDettaglio()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append("INSERT INTO MVV_Log_Dettaglio( Id_Log, CodiceErrore, TipoErrore, Messaggio ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          " & Agro_SQL_SaveNum(Id_Log) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(CodiceErrore) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(TipoErrore) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Messaggio) & "'  ")
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(_objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(_objParametriServer, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

End Class