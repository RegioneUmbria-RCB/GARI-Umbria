Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_MatriciOrganicheXFrequenza_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '######################################################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Int32,
                          ByVal Id_Mat_O As Int32,
                          ByVal Id_Fre As Int32,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_MatriciOrganicheXFrequenza_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim intRapporto As Integer = 0

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT MOF.*, F.Frequenza_Des, MO.Mat_O_Des ")
            StrSQL.AppendLine(" FROM  PC_MatriciOrganicheXFrequenza MOF ")
            StrSQL.AppendLine(" INNER JOIN PC_Frequenza F ON F.Id_Fre = MOF.Id_Fre AND F.Regolamento_Cod = MOF.Regolamento_Cod  ")
            StrSQL.AppendLine(" INNER JOIN PC_MatriciOrganiche MO ON MO.Id_Mat_O = MOF.Id_Mat_O AND MO.Regolamento_Cod = MOF.Regolamento_Cod  ")
            StrSQL.AppendLine(" WHERE MOF.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.AppendLine(" AND F.Contesto = " & Enum_Contesto_PC_Frequenza.Fertilizzazioni & " ")
            StrSQL.AppendLine(" AND MOF.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND MOF.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Id_Fre <> 0 Then
                StrSQL.AppendLine(" AND MOF.Id_Fre = " & Agro_SQL_SaveNum(Id_Fre) & " ")
            End If

            If Id_Mat_O <> 0 Then
                StrSQL.AppendLine(" AND MOF.Id_Mat_O = " & Agro_SQL_SaveNum(Id_Mat_O) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   MOF.Inviato >= 0 ")
                    StrSQL.AppendLine(" AND   MO.Inviato >= 0 ")
                    StrSQL.AppendLine(" AND   F.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   MOF.Inviato =- 1 ")
                    StrSQL.AppendLine(" AND   MO.Inviato =- 1 ")
                    StrSQL.AppendLine(" AND   F.Inviato =- 1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
