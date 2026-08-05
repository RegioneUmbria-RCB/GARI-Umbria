Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class PC_EffluentiXFrequenza_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Regolamento_Cod As Int32,
                          ByVal Eff_Cod As Int32,
                          ByVal Id_Fre As Int32,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_EffluentiXFrequenza_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim intRapporto As Integer = 0

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT EF.*, F.Frequenza_Des, E.Eff_Des, E.Udm_Cod,udm.UDM_SIM, fer.n as n_titolo, fer.fer_cod, fer.fer_des ")
            StrSQL.Append(" FROM  PC_EffluentiXFrequenza EF ")
            StrSQL.Append(" INNER JOIN PC_Frequenza F ON F.Id_Fre = EF.Id_Fre AND F.Regolamento_Cod = EF.Regolamento_Cod  ")
            StrSQL.Append(" INNER JOIN Effluenti E ON E.Eff_Cod = EF.Eff_Cod AND E.Regolamento_Cod = EF.Regolamento_Cod  ")
            StrSQL.Append(" inner Join UnitaMisura udm on udm.UDM_COD=E.Udm_Cod ")
            StrSQL.Append(" inner Join EffluentixFertilizzanti efer on efer.Eff_Cod=e.Eff_Cod And efer.Regolamento_Cod=e.Regolamento_Cod ")
            StrSQL.Append(" inner Join Fertilizzanti fer on fer.Fer_Cod=efer.Fer_Cod ")

            StrSQL.Append(" WHERE EF.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.Append(" AND   EF.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EF.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Id_Fre <> 0 Then
                StrSQL.Append(" AND EF.Id_Fre = " & Agro_SQL_SaveNum(Id_Fre) & " ")
            End If

            If Eff_Cod <> 0 Then
                StrSQL.Append(" AND EF.Eff_Cod = " & Agro_SQL_SaveNum(Eff_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   EF.Inviato >=0 ")
                    StrSQL.Append(" AND   E.Inviato >=0 ")
                    StrSQL.Append(" AND   F.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   EF.Inviato =-1 ")
                    StrSQL.Append(" AND   E.Inviato =-1 ")
                    StrSQL.Append(" AND   F.Inviato =-1 ")
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
