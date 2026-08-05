Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class GruppoFinalitaxSpecieVegetali_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal GRFI_COD As Integer, _
                            ByVal VEG_COD As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  GruppoFinalitaxSpecieVegetali ")
            StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If GRFI_COD <> 0 Then
                StrSQL.Append(" AND GRFI_COD = " & Agro_SQL_SaveNum(GRFI_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.Append(" AND VEG_COD = " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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


    Public Function PrimoGrfiCod_from_Vegcod(ByVal Veg_Cod As Integer, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As String

        Dim grfi_cod As Integer = 0

        Dim Dt As DataTable

        Dt = Leggi(0, _
                   Veg_Cod, _
                     "", " Grfi_Cod ASC", _
                     objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then
            grfi_cod = Dt.Rows(0).Item("Grfi_Cod")

        End If

        Return grfi_cod

    End Function

    Public Function Leggi_Con_GruppoFinalita_SpecieVegetali(ByVal GRFI_COD As Integer,
                                                            ByVal VEG_COD As Integer,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R.Leggi_Con_GruppoFinalita_SpecieVegetali()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  GruppoFinalitaxSpecieVegetali , GruppoFinalita, SpecieVegetali ")
            StrSQL.AppendLine(" WHERE GruppoFinalitaxSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDateTime(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   GruppoFinalitaxSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDateTime(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   GruppoFinalita.Validita_inizio < " & Agro_SQL_SaveDateTime(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   GruppoFinalita.Validita_Fine  > " & Agro_SQL_SaveDateTime(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   GruppoFinalitaxSpecieVegetali.GRFI_COD = GruppoFinalita.GRFI_COD ")
            StrSQL.AppendLine(" AND   GruppoFinalitaxSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")

            If GRFI_COD <> 0 Then
                StrSQL.AppendLine(" AND GruppoFinalitaxSpecieVegetali.GRFI_COD = " & Agro_SQL_SaveNum(GRFI_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND GruppoFinalitaxSpecieVegetali.VEG_COD = " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            StrSQL.Append(" ORDER BY GruppoFinalita.GRFI_DES ")
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
