Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_PrecessioneColturale_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '######################################################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Int32, ByVal PUA_Tipo As Int32,
                          ByVal Pre_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM PC_PrecessioneColturale ")
            StrSQL.Append(" WHERE   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.Append(" AND   Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PUA_Tipo <> 0 Then
                StrSQL.Append(" AND PUA_Tipo =  " & Agro_SQL_SaveNum(PUA_Tipo) & "  ")
            End If

            If Pre_Cod <> 0 Then
                StrSQL.Append(" AND Pre_Cod =  " & Agro_SQL_SaveNum(Pre_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PC_PrecessioneColturale.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PC_PrecessioneColturale.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Pre_Des ASC ")
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


    Public Function Leggi_conSpecie(ByVal Regolamento_Cod As Int32, ByVal PUA_Tipo As Int32,
                          ByVal Pre_Cod As Int32, ByVal Veg_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R.Leggi_conSpecie()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM PC_PrecessioneColturaleXSpecieVegetali inner join PC_PrecessioneColturale ")
            StrSQL.Append(" ON PC_PrecessioneColturale.Regolamento_Cod= PC_PrecessioneColturaleXSpecieVegetali.Regolamento_Cod ")
            StrSQL.Append(" AND PC_PrecessioneColturale.Pre_Cod= PC_PrecessioneColturaleXSpecieVegetali.Pre_Cod ")

            StrSQL.Append(" WHERE   PC_PrecessioneColturaleXSpecieVegetali.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")


            StrSQL.Append(" AND   PC_PrecessioneColturaleXSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   PC_PrecessioneColturaleXSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Pre_Cod <> 0 Then
                StrSQL.Append(" AND PC_PrecessioneColturaleXSpecieVegetali.Pre_Cod =  " & Agro_SQL_SaveNum(Pre_Cod) & "  ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND PC_PrecessioneColturaleXSpecieVegetali.veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If PUA_Tipo <> 0 Then
                StrSQL.Append(" AND PC_PrecessioneColturale.PUA_Tipo =  " & Agro_SQL_SaveNum(PUA_Tipo) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PC_PrecessioneColturaleXSpecieVegetali.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PC_PrecessioneColturaleXSpecieVegetali.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Pre_Des ASC ")
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
