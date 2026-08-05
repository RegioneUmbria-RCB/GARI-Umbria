Public Class Macchine_Consumi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                         ByVal classCode As String,
                         ByVal dittaCod As Integer,
                         ByVal potenza As Decimal,
                         ByVal potenzaUdmCod As Integer,
                         ByVal carCod As Integer,
                         ByVal udmCod As Integer,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Macchine_Consumi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Macchine_Consumi ")
            StrSQL.Append($" WHERE Validita_inizio <= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)} ")
            StrSQL.Append($" AND   Validita_Fine >= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio)} ")

            If classCode <> "" Then
                ' Controllo se il ClassCode specificato inizia con la colonna Class_Code
                StrSQL.Append($" AND '{Agro_SQL_SaveText(classCode)}' LIKE CONCAT(Class_Code, '%') ")
            End If
            If dittaCod <> 0 Then
                StrSQL.Append($" AND (Ditta_Cod = {Agro_SQL_SaveNum(dittaCod)} OR Ditta_Cod IS NULL) ")
            End If
            If potenza <> 0 Then
                StrSQL.Append($" AND Potenza_Min <= {Agro_SQL_SaveNum(potenza)} ")
                StrSQL.Append($" AND Potenza_Max >= {Agro_SQL_SaveNum(potenza)} ")
            End If
            If potenzaUdmCod <> 0 Then
                StrSQL.Append($" AND Potenza_Udm_Cod = {Agro_SQL_SaveNum(potenzaUdmCod)} ")
            End If
            If carCod <> 0 Then
                StrSQL.Append($" AND Car_Cod = {Agro_SQL_SaveNum(carCod)} ")
            End If
            If udmCod <> 0 Then
                StrSQL.Append($" AND Udm_Cod = {Agro_SQL_SaveNum(udmCod)} ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append($" AND {xFiltroAggiuntivo}")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append($" ORDER BY {xOrderBy}")
            Else
                StrSQL.Append(" ORDER BY Ditta_Cod DESC, LEN(Class_Code) DESC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception($"[{NomeRoutine}] : {MessaggioErrore}")
        End Try

        Return DT

    End Function

End Class
