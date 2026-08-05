Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class GruppoFinalita_Rer_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                             ByVal Regolamento_Cod As Long, _
                             ByVal GRFI_COD As Long, _
                             ByVal Veg_Cod As Integer, _
                             ByVal Cerca_GrfiDes As String, _
                                 ByVal xFiltroAggiuntivo As String, _
                                 ByVal xOrderBy As String, _
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoFinalita_Rer.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Append(" SELECT  SpecieVegetali.Veg_des, GruppoFinalitaxSpecieVegetalixConc.Veg_Cod,  GruppoFinalita_Rer.* ")
            StrSQL.Append(" FROM    GruppoFinalita_Rer ")
            StrSQL.Append(" INNER JOIN    GruppoFinalitaxSpecieVegetalixConc ON GruppoFinalitaxSpecieVegetalixConc.GRFI_COD = GruppoFinalita_Rer.GRFI_COD ")
            StrSQL.Append(" INNER JOIN    SpecieVegetali ON SpecieVegetali.Veg_Cod = GruppoFinalitaxSpecieVegetalixConc.Veg_Cod ")

            StrSQL.Append(" WHERE   GruppoFinalita_Rer.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     GruppoFinalita_Rer.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpecieVegetalixConc.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If GRFI_COD <> 0 Then
                StrSQL.Append(" AND GruppoFinalita_Rer.Grfi_Cod = " & Agro_SQL_SaveNum(GRFI_COD) & " ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpecieVegetalixConc.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Cerca_GrfiDes <> "" Then
                StrSQL.Append(" AND GruppoFinalita_Rer.Grfi_Des LIKE '%" & Agro_SQL_SaveText(Cerca_GrfiDes) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & " ")
            Else
                StrSQL.Append(" ORDER BY Veg_Des, Grfi_Des ")
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
