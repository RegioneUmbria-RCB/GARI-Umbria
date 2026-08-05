Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class FasiFenologichexSpecie_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_JoinCompleta
    ''' </summary>
    ''' <param name="FF_COD"></param>
    ''' <param name="VEG_COD"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	17/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal FF_COD As Integer, _
                          ByVal VEG_COD As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FasiFenologichexSpecie_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  FasiFenologichexSpecieVegetali , SpecieVegetali , FasiFenologiche ")
                    StrSQL.Append(" WHERE FasiFenologichexSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   FasiFenologichexSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   FasiFenologiche.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   FasiFenologiche.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   FasiFenologichexSpecieVegetali.FF_COD = FasiFenologiche.FF_COD ")
                    StrSQL.Append(" AND   FasiFenologichexSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")

                    If FF_COD <> 0 Then
                        StrSQL.Append(" AND FasiFenologichexSpecieVegetali.FF_COD =  " & Agro_SQL_SaveNum(FF_COD) & "  ")
                    End If

                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND FasiFenologichexSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   dbo.FasiFenologichexSpecieVegetali.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.SpecieVegetali.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.FasiFenologiche.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   dbo.FasiFenologichexSpecieVegetali.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.SpecieVegetali.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.FasiFenologiche.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------


                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY SpecieVegetali.VEG_DES ASC, FasiFenologiche.FF_DES ASC ")
                    End If

            End Select

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
    'usata da web service (a differenza della precedente legge eventualmente dalla tabella delle fasi personalizzate ()
    Public Function Leggi_WS(ByVal FF_COD As Integer,
                             ByVal VEG_COD As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByVal Personalizzate As Boolean = False,
                                    Optional ByVal Piva_SuperUser As String = ""
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FasiFenologichexSpecie_R.Leggi_WS()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  FasiFenologichexSpecieVegetali fs inner join SpecieVegetali s on fs.VEG_COD = s.VEG_COD ")
            StrSQL.Append(" inner join FasiFenologiche ff on fs.FF_COD = ff.FF_COD ")

            If Personalizzate = True Then
                StrSQL.Append(" inner join SuperUser_OperazioniPersonalizzate sp on sp.codice = fs.ff_cod ")
            End If

            StrSQL.Append(" WHERE fs.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   fs.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   s.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   s.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   ff.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   ff.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Personalizzate = True Then
                StrSQL.Append(" AND sp.Piva_superUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                StrSQL.Append(" AND sp.lav_cod = 79 ")
            End If

            If FF_COD <> 0 Then
                StrSQL.Append(" AND fs.FF_COD =  " & Agro_SQL_SaveNum(FF_COD) & "  ")
            End If

                    If VEG_COD <> 0 Then
                StrSQL.Append(" AND fs.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   fs.Inviato >=0 ")
                    StrSQL.Append(" AND   s.Inviato >=0 ")
                    StrSQL.Append(" AND   ff.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   fs.Inviato =-1 ")
                    StrSQL.Append(" AND   s.Inviato =-1 ")
                    StrSQL.Append(" AND   ff.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY s.VEG_DES ASC, ff.FF_DES ASC ")
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