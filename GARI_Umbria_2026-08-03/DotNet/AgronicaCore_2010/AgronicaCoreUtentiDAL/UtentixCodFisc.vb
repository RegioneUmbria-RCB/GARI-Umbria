Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD.profilazione

Public Class UtentixCodFisc_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
                         ByVal UserName As String,
                         ByVal CodFisc As String,
                         ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.UtentixCodFisc.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT * " + vbCrLf)
            strSQL.Append(" FROM UtentixCodFisc " + vbCrLf)
            strSQL.Append(" WHERE 1=1 " + vbCrLf)

            If UserName <> "" Then
                strSQL.Append(" AND [Username] = '" & Agro_SQL_SaveText(UserName) & "'" & vbCrLf)
            End If

            If CodFisc <> "" Then
                strSQL.Append(" AND CodFisc = '" & Agro_SQL_SaveText(CodFisc) & "'" & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        strSQL.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        strSQL.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
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


Public Class UtentixCodFisc_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(
                         ByVal UserName As String,
                         ByVal CodFisc As String,
                         ByVal Validita_Inizio As Date,
                         ByVal Validita_Fine As Date,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.UtentixCodFisc.Scrivi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim result As Boolean

        Try

            strSQL.Length = 0

            strSQL.Append("INSERT INTO [dbo].[UtentixCodFisc]
                               ([UserName]
                               ,[CodFisc]
                               ,[Data_Creazione]
                               ,[Data_Modifica]
                               ,[Validita_Inizio]
                               ,[Validita_Fine])
                         VALUES
                               (" & Agro_SQL_SaveText_NULL(UserName) & "
                               ," & Agro_SQL_SaveText_NULL(CodFisc) & "
                               ," & Agro_SQL_SaveDateTime(DateTime.Now) & "
                               ," & Agro_SQL_SaveDateTime(DateTime.Now) & "
                               ," & Agro_SQL_SaveDateTime(Validita_Inizio) & "
                               ," & Agro_SQL_SaveDateTime(Validita_Fine) & ") ")


            '--------------------------------------------------------------------------
            result = EseguiQuery_Scrittura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            result = False
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result

    End Function

    Public Function Modifica(
        DatiCorrenti As UtenteAccessoSPID, NuoviDati As UtenteAccessoSPID,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.UtentixCodFisc.Modifica()"
        Dim strSQL As New Text.StringBuilder
        Dim result As Boolean
        Try
            strSQL.AppendLine("UPDATE [dbo].[UtentixCodFisc] SET ")
            strSQL.AppendLine("     UserName = '" & Agro_SQL_SaveText(NuoviDati.UserName) & "' ")
            strSQL.AppendLine("   , CodFisc = '" & Agro_SQL_SaveText(NuoviDati.CfLogin) & "' ")
            strSQL.AppendLine("   , Data_Modifica = GETDATE() ")
            strSQL.AppendLine(" WHERE ")
            strSQL.AppendLine("     UserName = '" & Agro_SQL_SaveText(DatiCorrenti.UserName) & "' ")
            strSQL.AppendLine("     AND CodFisc = '" & Agro_SQL_SaveText(DatiCorrenti.CfLogin) & "' ")


            '--------------------------------------------------------------------------
            result = EseguiQuery_Scrittura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return result
    End Function


End Class
