Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_FasiCicloColturalexGruppoVegetale_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '######################################################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Int32, _
                          ByVal Veg_Cod As Int32, _
                          ByVal Id_Ciclo As Int32, _
                          ByVal Gru_Cod As Int32, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturalexGruppoVegetale_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT F.fase_Des, FGV.*  ")
            StrSQL.Append(" FROM  PC_FasiCicloColturalexGruppoVegetale FGV ")
            StrSQL.Append(" INNER JOIN  PC_FasiCicloColturale F ON F.Id_Fase=FGV.id_fase AND F.Regolamento_Cod=FGV.Regolamento_Cod  ")
            StrSQL.Append(" WHERE F.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.Append(" AND   FGV.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   FGV.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Id_Ciclo <> 0 Then
                StrSQL.Append(" AND FGV.Id_Ciclo =  " & Agro_SQL_SaveNum(Id_Ciclo) & "  ")
            End If

            If Gru_Cod <> 0 Then
                StrSQL.Append(" AND FGV.Gru_Cod =  " & Agro_SQL_SaveNum(Gru_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   FGV.Inviato >=0 ")
                    StrSQL.Append(" AND   F.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   FGV.Inviato =-1 ")
                    StrSQL.Append(" AND   F.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            '--------------------------------------------------------------------------ù
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

    'a differenza della precedente viene passata solo la specie
    Public Function Leggi(ByVal Regolamento_Cod As Int32,
                          ByVal Veg_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturalexGruppoVegetale_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT distinct F.fase_Des, F.id_fase  ")
            StrSQL.Append(" FROM  PC_FasiCicloColturalexGruppoVegetale FGV ")
            StrSQL.Append(" INNER JOIN  PC_FasiCicloColturale F ON F.Id_Fase=FGV.id_fase AND F.Regolamento_Cod=FGV.Regolamento_Cod  ")
            StrSQL.Append(" INNER JOIN  SpecieVegetali S on s.Gru_Cod= FGV.Gru_Cod  ")
            StrSQL.Append(" WHERE S.Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND F.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   FGV.Inviato >=0 ")
                    StrSQL.Append(" AND   F.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   FGV.Inviato =-1 ")
                    StrSQL.Append(" AND   F.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY F.Id_Fase desc")
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
