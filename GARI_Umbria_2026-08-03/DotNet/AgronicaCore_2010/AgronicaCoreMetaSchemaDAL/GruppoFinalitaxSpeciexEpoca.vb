Imports AgronicaCoreDataProvider.UtilityProvider
Public Class GruppoFinalitaxSpeciexEpoca_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Regolamento_Cod As Int32,
                                  ByVal Id_Gru As Int32,
                                  ByVal Veg_Cod As Int32,
                                   ByVal Grfi_Cod As Int32,
                                   ByVal Grfi_Cod_Gias As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpeciexEpoca.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM GruppoFinalitaxSpeciexEpoca ")
            StrSQL.Append(" INNER JOIN GruppoFinalitaMappatura ON GruppoFinalitaMappatura.grfi_cod_pua = GruppoFinalitaxSpeciexEpoca.grfi_cod ")
            StrSQL.Append(" AND GruppoFinalitaMappatura.veg_cod = GruppoFinalitaxSpeciexEpoca.veg_cod ")
            StrSQL.Append(" INNER JOIN GruppoFinalita_Rer ON GruppoFinalitaxSpeciexEpoca.GRFI_COD = GruppoFinalita_Rer.GRFI_COD ")

            StrSQL.Append(" WHERE  GruppoFinalitaxSpeciexEpoca.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")

            If Id_Gru <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.Id_Gru =  " & Agro_SQL_SaveNum(Id_Gru) & "  ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.veg_cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If
            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.Grfi_Cod =  " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
            End If
            If Grfi_Cod_Gias <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaMappatura.grfi_cod_gias =  " & Agro_SQL_SaveNum(Grfi_Cod_Gias) & "  ")
            End If

            '--------------------------------------------------------------------------
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

    Public Function Leggi_conLimitiAzotoxSpecie(ByVal Regolamento_Cod As Int32,
                                  ByVal Id_Gru As Int32,
                                  ByVal Veg_Cod As Int32,
                                   ByVal Grfi_Cod As Int32,
                                   ByVal Grfi_Cod_Gias As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpeciexEpoca.Leggi_conLimitiAzotoxSpecie()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM GruppoFinalitaxSpeciexEpoca ")
            StrSQL.Append(" INNER JOIN GruppoFinalitaMappatura ON GruppoFinalitaMappatura.grfi_cod_pua = GruppoFinalitaxSpeciexEpoca.grfi_cod ")
            StrSQL.Append(" AND GruppoFinalitaMappatura.veg_cod = GruppoFinalitaxSpeciexEpoca.veg_cod ")
            StrSQL.Append(" INNER JOIN GruppoFinalita_Rer ON GruppoFinalitaxSpeciexEpoca.GRFI_COD = GruppoFinalita_Rer.GRFI_COD ")

            StrSQL.Append(" INNER JOIN LimitiAzotoxSpecie ON GruppoFinalitaxSpeciexEpoca.Regolamento_Cod = LimitiAzotoxSpecie.Regolamento_Cod ")
            StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.veg_cod = LimitiAzotoxSpecie.veg_cod ")
            StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.grfi_cod = LimitiAzotoxSpecie.grfi_cod ")

            StrSQL.Append(" WHERE  GruppoFinalitaxSpeciexEpoca.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")

            If Id_Gru <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.Id_Gru =  " & Agro_SQL_SaveNum(Id_Gru) & "  ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.veg_cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If
            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.Grfi_Cod =  " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
            End If
            If Grfi_Cod_Gias <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaMappatura.grfi_cod_gias =  " & Agro_SQL_SaveNum(Grfi_Cod_Gias) & "  ")
            End If

            '--------------------------------------------------------------------------
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

End Class
