Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text

Public Class ServizixSportello_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Servizio_Cod As Integer,
                          ByVal Configurazione_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.ServizixSportello_R.LeggiAttivoAllaData()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM ServizixSportello ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Servizio_Cod <> 0 Then
                StrSQL.Append(" AND Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Configurazione_Cod <> 0 Then
                StrSQL.Append(" AND Configurazione_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiAttivoAllaData(ByVal Servizio_Cod As Integer,
                                ByVal Data_Riferimento As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.ServizixSportello_R.LeggiAttivoAllaData()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            StrSQL.AppendLine(" SELECT Sementieri_Sportello_Configurazione.* ")
            StrSQL.AppendLine(" FROM ServizixSportello ")
            StrSQL.AppendLine(" JOIN Sementieri_Sportello_Configurazione On ServizixSportello.Configurazione_Cod = Sementieri_Sportello_Configurazione.Sementieri_Sportello_Configurazione_cod ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Data_Riferimento <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND Sementieri_Sportello_Configurazione.Validita_Inizio <= " & Agro_SQL_SaveDateTime(Data_Riferimento) & " ")
                StrSQL.AppendLine(" AND Sementieri_Sportello_Configurazione.Validita_Fine >= " & Agro_SQL_SaveDateTime(Data_Riferimento) & " ")
            End If

            If Servizio_Cod <> 0 Then
                StrSQL.Append(" AND ServizixSportello.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Sementieri_Sportello_Configurazione.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Sementieri_Sportello_Configurazione.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class


Public Class ServizixSportello_W
    Inherits AgronicaCoreDataProvider.DataProvider

End Class
