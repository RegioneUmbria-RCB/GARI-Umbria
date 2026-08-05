Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Articoli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'se non si vuole filtrare per range temporale passare AGRODATAINIZIO e AGRODATAFINE
    Public Function LeggiFruttagelLabCQ(
                                  ByVal Codice As String,
                                  ByVal Marchio As String,
                                  ByVal MatPrima_Nome As String,
                                  ByVal MatPrima_VegCod As Integer?,
                                  ByVal MatPrima_CulCod As Integer?,
                                  ByVal Formato As String,
                                  ByVal CatMerceologica As String,
                                  ByVal TipoProduzione As String,
                                  ByVal RegolaScadenza_Tipo As String,
                                  ByVal RegolaScadenza_Qta As String,
                                  ByVal Stato As String,
                                  ByVal Cat_Cod As Integer?,
                                  ByVal Data_Inizio_filtroTemp As Date,
                                  ByVal Data_Fine_filtroTemp As Date,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreLabControlloQualitaDAL.Articoli_R.LeggiFruttagelLabCQ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT   mp.Piva as PivaSuperUser, mp.Cod_Articolo as Codice, mp.Mat_Des as Descrizione1, mp.Note as Descrizione2, mp.Veg_Cod as MatPrima_VegCod, mp.Cul_Cod as MatPrima_CulCod, mpd.Extra_Str1 as Marchio, mpd.Extra_Str2 as MatPrima_Nome, mpd.Extra_Str3 as Formato, mpd.Extra_Str4 as CatMerceologica, mpd.Extra_Str5 as TipoProduzione, mpd.Extra_Str6 as RegolaScadenza_Tipo, mpd.Extra_Str7 as RegolaScadenza_Qta, mpd.Extra_Str8 as Stato, mpd.Extra_Str9 as LineaPDZ ")
            StrSQL.AppendLine(" FROM materie_prime mp inner join Materie_Prime_Dettagli mpd ")
            StrSQL.AppendLine(" ON mp.Piva=mpd.Piva_SuperUser and mp.Mat_Cod=mpd.Mat_Cod ")
            StrSQL.AppendLine(" WHERE mp.Piva = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            StrSQL.AppendLine(" AND mp.Elem_Cod=210 " + vbCrLf)

            StrSQL.AppendLine(" AND mp.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Fine_filtroTemp) & " ")
            StrSQL.AppendLine(" AND mp.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio_filtroTemp) & " ")

            If Not IsNothing(Codice) Then
                StrSQL.AppendLine(" AND mp.Cod_Articolo = " & Agro_SQL_SaveText_NULL(Codice))
            End If

            If Not IsNothing(Marchio) Then
                StrSQL.AppendLine(" AND mpd.Extra_Str1 = " & Agro_SQL_SaveText_NULL(Marchio))
            End If

            If Not IsNothing(MatPrima_Nome) Then
                StrSQL.AppendLine(" AND mpd.Extra_Str2 = " & Agro_SQL_SaveText_NULL(MatPrima_Nome))
            End If

            If Not IsNothing(MatPrima_VegCod) Then
                StrSQL.AppendLine(" AND mp.Veg_Cod = " & Agro_SQL_SaveNum_NULL(MatPrima_VegCod))
            End If

            If Not IsNothing(MatPrima_CulCod) Then
                StrSQL.AppendLine(" AND mp.Cul_Cod = " & Agro_SQL_SaveNum_NULL(MatPrima_CulCod))
            End If

            If Not IsNothing(Cat_Cod) Then
                StrSQL.AppendLine(" AND mp.Cat_Cod = " & Agro_SQL_SaveNum_NULL(Cat_Cod))
            End If

            If Not IsNothing(Formato) Then
                StrSQL.AppendLine(" AND mpd.Extra_Str3 = " & Agro_SQL_SaveText_NULL(Formato))
            End If

            If Not IsNothing(CatMerceologica) Then
                StrSQL.AppendLine(" AND mpd.Extra_Str4 = " & Agro_SQL_SaveText_NULL(CatMerceologica))
            End If

            If Not IsNothing(TipoProduzione) Then
                StrSQL.AppendLine(" AND mpd.Extra_Str5 = " & Agro_SQL_SaveText_NULL(TipoProduzione))
            End If

            If Not IsNothing(RegolaScadenza_Tipo) Then
                StrSQL.AppendLine(" AND mpd.Extra_Str6 = " & Agro_SQL_SaveText_NULL(RegolaScadenza_Tipo))
            End If

            If Not IsNothing(RegolaScadenza_Qta) Then
                StrSQL.AppendLine(" AND mpd.Extra_Str7 = " & Agro_SQL_SaveText_NULL(RegolaScadenza_Qta))
            End If

            If Not IsNothing(Stato) Then
                StrSQL.AppendLine(" AND mpd.Extra_Str8 = " & Agro_SQL_SaveText_NULL(Stato))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   mp.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   mp.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY mp.Cod_Articolo Asc ")
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
