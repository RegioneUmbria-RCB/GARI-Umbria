Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class PC_PrecessioneColturalexFrequenza_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '######################################################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Int32,
                          ByVal Pre_Cod As Int32,
                          ByVal Id_Fre As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturalexFrequenza_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT PCF.*, F.Frequenza_Des, PC.Pre_Des ")
            StrSQL.AppendLine(" FROM PC_PrecessioneColturalexFrequenza PCF ")
            StrSQL.AppendLine(" INNER JOIN PC_Frequenza F ON F.Id_Fre = PCF.Id_Fre AND F.Regolamento_Cod = PCF.Regolamento_Cod  ")
            StrSQL.AppendLine(" INNER JOIN PC_PrecessioneColturale PC ON PC.Pre_Cod = PCF.Pre_Cod AND PC.Regolamento_Cod = PCF.Regolamento_Cod  ")

            StrSQL.AppendLine(" WHERE   PCF.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.AppendLine(" AND   F.Contesto = " & Enum_Contesto_PC_Frequenza.Precessione_Colturale & " ")

            StrSQL.AppendLine(" AND   PCF.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   PCF.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Pre_Cod <> 0 Then
                StrSQL.AppendLine(" AND PCF.Pre_Cod =  " & Agro_SQL_SaveNum(Pre_Cod) & "  ")
            End If

            If Id_Fre <> 0 Then
                StrSQL.AppendLine(" AND PCF.Id_Fre =  " & Agro_SQL_SaveNum(Id_Fre) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   PCF.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   PCF.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PCF.Pre_Cod ASC ")
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
