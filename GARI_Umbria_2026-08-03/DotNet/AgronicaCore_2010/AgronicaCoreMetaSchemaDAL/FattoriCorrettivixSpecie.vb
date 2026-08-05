Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

Public Class FattoriCorrettivixSpecie_R
    Inherits AgronicaCoreDataProvider.DataProvider



    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal Regolamento_Cod As Integer, _
                          ByVal Fattore_Cod As Integer, _
                          ByVal Tipo As String, _
                          ByVal Variazione As String, _
                          ByVal Veg_Cod As Integer, _
                          ByVal Grfi_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT FC.Fattore_Cod, FC.Fattore_Des, FC.Variazione, FC.Tipo, FCS.Veg_Cod, FCS.Grfi_Cod, FCS.Valore, FC.Regolamento_cod, FC.Visibile ")
            StrSQL.Append(" FROM  FattoriCorrettivi FC ")
            StrSQL.Append(" INNER JOIN  FattoriCorrettivixSpecie FCS ON FC.Regolamento_Cod = FCS.Regolamento_Cod AND FC.Fattore_Cod = FCS.Fattore_Cod ")
            StrSQL.Append(" WHERE 1=1")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND FC.Regolamento_cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If Fattore_Cod <> 0 Then
                StrSQL.Append(" AND FC.Fattore_Cod =  " & Agro_SQL_SaveNum(Fattore_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND FCS.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND FCS.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            End If

            If Tipo <> "" Then
                StrSQL.Append(" AND FC.Tipo =  '" & Agro_SQL_SaveText(Tipo) & "'  ")
            End If

            If Variazione <> "" Then
                StrSQL.Append(" AND FC.Variazione = '" & Agro_SQL_SaveText(Variazione) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   FC.Inviato >=0 ")
                    StrSQL.Append(" AND   FCS.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   FC.Inviato =-1 ")
                    StrSQL.Append(" AND   FCS.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    'legge anche i nuovi campi x controllo schede
    Public Function Leggi2(ByVal Regolamento_Cod As Integer,
                          ByVal Fattore_Cod As Integer,
                          ByVal Tipo As String,
                          ByVal Variazione As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Grfi_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R.Leggi2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT FC.Fattore_Cod, FC.Fattore_Des, FC.Variazione, FC.Tipo, FCS.Veg_Cod, FCS.Grfi_Cod, FCS.Valore, FC.Regolamento_cod, FC.Visibile, ")
            StrSQL.Append(" ISNULL(FCS.Controllo_Valore,0) AS Controllo_Valore, ISNULL(FCS.Controllo_Funzione,'') AS Controllo_Funzione ")
            StrSQL.Append(" FROM  FattoriCorrettivi FC ")
            StrSQL.Append(" INNER JOIN  FattoriCorrettivixSpecie FCS ON FC.Regolamento_Cod = FCS.Regolamento_Cod AND FC.Fattore_Cod = FCS.Fattore_Cod ")


            StrSQL.Append(" WHERE 1=1")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND FC.Regolamento_cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If Fattore_Cod <> 0 Then
                StrSQL.Append(" AND FC.Fattore_Cod =  " & Agro_SQL_SaveNum(Fattore_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND FCS.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND FCS.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            End If

            If Tipo <> "" Then
                StrSQL.Append(" AND FC.Tipo =  '" & Agro_SQL_SaveText(Tipo) & "'  ")
            End If

            If Variazione <> "" Then
                StrSQL.Append(" AND FC.Variazione = '" & Agro_SQL_SaveText(Variazione) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   FC.Inviato >=0 ")
                    StrSQL.Append(" AND   FCS.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   FC.Inviato =-1 ")
                    StrSQL.Append(" AND   FCS.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_conFinalitaGias(ByVal Regolamento_Cod As Integer,
                          ByVal Fattore_Cod As Integer,
                          ByVal Tipo As String,
                          ByVal Variazione As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Grfi_Cod As Integer, ByVal Grfi_Cod_Gias As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R.Leggi2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT FC.Fattore_Cod, FC.Fattore_Des, FC.Variazione, FC.Tipo, FCS.Veg_Cod, FCS.Grfi_Cod, FM.grfi_cod_gias, FCS.Valore, FC.Regolamento_cod, FC.Visibile, ")
            StrSQL.Append(" ISNULL(FCS.Controllo_Valore,0) AS Controllo_Valore, ISNULL(FCS.Controllo_Funzione,'') AS Controllo_Funzione ")
            StrSQL.Append(" FROM  FattoriCorrettivi FC ")
            StrSQL.Append(" INNER JOIN  FattoriCorrettivixSpecie FCS ON FC.Regolamento_Cod = FCS.Regolamento_Cod AND FC.Fattore_Cod = FCS.Fattore_Cod ")
            StrSQL.Append(" INNER JOIN   GruppoFinalitaMappatura FM ON FM.grfi_cod_pua = FCS.Grfi_Cod and  FM.veg_cod = FCS.veg_Cod ")


            StrSQL.Append(" WHERE 1=1")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND FC.Regolamento_cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If Fattore_Cod <> 0 Then
                StrSQL.Append(" AND FC.Fattore_Cod =  " & Agro_SQL_SaveNum(Fattore_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND FCS.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND FCS.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            End If

            If Grfi_Cod_Gias <> 0 Then
                StrSQL.Append(" AND FM.grfi_cod_gias = " & Agro_SQL_SaveNum(Grfi_Cod_Gias) & " ")
                If Veg_Cod <> 0 Then
                    StrSQL.Append(" AND FM.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                End If
            End If

            If Tipo <> "" Then
                StrSQL.Append(" AND FC.Tipo =  '" & Agro_SQL_SaveText(Tipo) & "'  ")
            End If

            If Variazione <> "" Then
                StrSQL.Append(" AND FC.Variazione = '" & Agro_SQL_SaveText(Variazione) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   FC.Inviato >=0 ")
                    StrSQL.Append(" AND   FCS.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   FC.Inviato =-1 ")
                    StrSQL.Append(" AND   FCS.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiValoriVariazioni(ByVal Regolamento_Cod As Integer, _
                                         ByVal Fattore_Cod As Integer, _
                                         ByVal Tipo As String, _
                                         ByVal Variazione As String, _
                                         ByVal Veg_Cod As Integer, _
                                         ByVal Grfi_Cod As Integer, _
                                               ByVal xFiltroAggiuntivo As String, _
                                               ByVal xOrderBy As String, _
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                   ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT ")
            StrSQL.Append(" CASE WHEN Tipo = 't' THEN (SELECT TOP 1 FV.Fattore_Cod FROM FattoriCorrettivixSpecie FCSV  ")
            StrSQL.Append("                            INNER JOIN FattoriCorrettivi FV ON FV.Regolamento_Cod = FCSV.Regolamento_Cod AND FV.Fattore_Cod = FCSV.Fattore_Cod ")
            StrSQL.Append("                            WHERE FCS.Regolamento_Cod = FCSV.Regolamento_Cod ")
            StrSQL.Append(" 						   AND FCS.Fattore_Cod < FCSV.Fattore_Cod ")
            StrSQL.Append(" 						   AND FCS.Veg_Cod = FCSV.Veg_Cod ")
            StrSQL.Append(" 						   AND FV.Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
            StrSQL.Append(" 						   AND FCS.Grfi_Cod = FCSV.Grfi_Cod) ELSE FC.Fattore_Cod END AS Fattore_Cod, ")
            StrSQL.Append(" CASE WHEN Tipo = 't' THEN FC.Fattore_Des + ' ' + CONVERT(VARCHAR, Valore, 0) ELSE FC.Fattore_Des END AS Fattore_Des,  ")
            StrSQL.Append(" FC.Variazione,  '" & Agro_SQL_SaveText(Tipo) & "' AS Tipo, FCS.Veg_Cod, FCS.Grfi_Cod, ")
            StrSQL.Append(" CASE WHEN Tipo = 't' THEN (SELECT TOP 1 Valore FROM FattoriCorrettivixSpecie FCSV  ")
            StrSQL.Append("                            INNER JOIN FattoriCorrettivi FV ON FV.Regolamento_Cod = FCSV.Regolamento_Cod AND FV.Fattore_Cod = FCSV.Fattore_Cod ")
            StrSQL.Append("                            WHERE FCS.Regolamento_Cod = FCSV.Regolamento_Cod ")
            StrSQL.Append(" 						   AND FCS.Fattore_Cod < FCSV.Fattore_Cod ")
            StrSQL.Append(" 						   AND FCS.Veg_Cod = FCSV.Veg_Cod ")
            StrSQL.Append(" 						   AND FV.Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
            StrSQL.Append(" 						   AND FCS.Grfi_Cod = FCSV.Grfi_Cod) ELSE FCS.Valore END AS Valore ")


            StrSQL.Append(" FROM  FattoriCorrettivi FC ")
            StrSQL.Append(" INNER JOIN  FattoriCorrettivixSpecie FCS ON FC.Regolamento_Cod = FCS.Regolamento_Cod AND FC.Fattore_Cod = FCS.Fattore_Cod   ")

            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                StrSQL.Append(" WHERE FC.Tipo IN ('" & Agro_SQL_Save_Clausola_IN(Tipo, True) & "','t')  ")
            Else
                If DataProviderFactory.Instance.ParametrizzaQuery Then
                    StrSQL.Append(" WHERE FC.Tipo IN (" & Agro_SQL_Save_Clausola_IN(Tipo, True) & ",'t')  ")
                Else
                    StrSQL.Append(" WHERE FC.Tipo IN ('" & Agro_SQL_Save_Clausola_IN(Tipo, True) & "','t')  ")
                End If
            End If

            'StrSQL.Append(" AND   FCS.Valore > 0 ")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND FC.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If Fattore_Cod <> 0 Then
                StrSQL.Append(" AND FC.Fattore_Cod =  " & Agro_SQL_SaveNum(Fattore_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND FCS.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND FCS.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            End If

            If Variazione <> "" Then
                StrSQL.Append(" AND FC.Variazione = '" & Agro_SQL_SaveText(Variazione) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   FC.Inviato >=0 ")
                    StrSQL.Append(" AND   FCS.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   FC.Inviato =-1 ")
                    StrSQL.Append(" AND   FCS.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
