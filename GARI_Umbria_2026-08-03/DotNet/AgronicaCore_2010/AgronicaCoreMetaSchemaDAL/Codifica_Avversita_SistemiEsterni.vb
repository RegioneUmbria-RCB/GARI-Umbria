Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class Codifica_Avversita_SistemiEsterni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal ID As Integer,
                          ByVal Sistema_Cod As Integer,
                          ByVal AV_Cod_Esterno As String,
                          ByVal AV_Cod As Integer,
                          ByVal AV_Gru As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Codifica_Avversita_SistemiEsterni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Codifica_Avversita_SistemiEsterni ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ID  = " & Agro_SQL_SaveNum(ID) & "   " + vbCrLf)
            End If

            If Sistema_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sistema_Cod  = " & Agro_SQL_SaveNum(Sistema_Cod) & "   " + vbCrLf)
            End If

            If AV_Cod <> 0 Then
                StrSQL.AppendLine(" AND AV_Cod  = " & Agro_SQL_SaveNum(AV_Cod) & "   " + vbCrLf)
            End If

            If AV_Gru <> 0 Then
                StrSQL.AppendLine(" AND AV_Gru  = " & Agro_SQL_SaveNum(AV_Gru) & "   " + vbCrLf)
            End If

            If AV_Cod_Esterno <> "" Then
                StrSQL.AppendLine(" AND AV_Cod_Esterno  = " & Agro_SQL_SaveText(AV_Cod_Esterno) & "   " + vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY AV_Des_Esterno ASC")
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
